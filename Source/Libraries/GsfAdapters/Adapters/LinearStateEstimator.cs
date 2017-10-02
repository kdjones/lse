//******************************************************************************************************
//  LinearStateEstimator.cs
//
//  Copyright © 2013, Kevin D. Jones.  All Rights Reserved.
//
//  This file is licensed to you under the Eclipse Public License -v 1.0 (the "License"); you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://www.opensource.org/licenses/eclipse-1.0.php
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  06/01/2013 - Kevin D. Jones
//       Generated original version of source code.
//  07/08/2013 - Kevin D. Jones
//       Added Stopwatch and Adapter Command to monitor execution time of each section of computation.
//  07/20/2013 - Kevin D. Jones
//      Several optimizations not included in third design now included. Added 'validInputMeasurementKeys'
//      check in Initialize() method to improve frame parsing speed. Added optimizations to measurement mapping
//      (especially for the positive sequence only estimator), added optimizations for observability 
//      analysis.
//  08/06/2013 - Kevin D. Jones
//      Swapped positions of active current phasor determination and observability analysis to resolve a bug
//      In determining observability of unmeasured substations indirectly observed via current flows.
//  04/11/2014 - Kevin D. Jones
//      Moved the execution of the Network Reconstruction Check optimization to resolve a bug in determining
//      the most up-to-date active current phasors.
//
//******************************************************************************************************
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Threading;
using System.Diagnostics;
using GSF.TimeSeries;
using GSF.TimeSeries.Adapters;
using GSF;
using GSF.PhasorProtocols;
using SynchrophasorAnalytics.Measurements;
using SynchrophasorAnalytics.Modeling;
using SynchrophasorAnalytics.Networks;

namespace SynchrophasorAnalytics.Adapters
{
    /// <summary>
    /// Processes streaming synchrophasor data to derive the optimal state of the system.
    /// </summary>
    [Description("Linear State Estimator : Processes streaming synchrophasor data to derive the optimal state of the system.")]
    public class LinearStateEstimator : ActionAdapterBase
    {
        #region [ Private Members ]

        private PhaseSelection m_phaseConfiguration;
        private bool m_overidePhaseConfiguration;
        private string m_configurationPathName;
        private string m_snapshotPathName;
        private string m_snapshotFileNamePrefix;
        private bool m_useUtcInFileName;
        private Network m_network;

        private Stopwatch m_stopwatch;
        private Stopwatch m_totalTimeStopwatch;
        private long m_refreshExecutionTime = 0;
        private long m_parsingExecutionTime = 0;
        private long m_measurementMappingExecutionTime = 0;
        private long m_observabilityAnalysisExecutionTime = 0;
        private long m_activeCurrentPhasorDetermintationExecutionTime = 0;
        private long m_stateComputationExecutionTime = 0;
        private long m_solutionRetrievalExecutionTime = 0;
        private long m_outputPreparationExecutionTime = 0;
        private long m_totalExecutionTimeInTicks = 0;
        private long m_totalExecutionTimeInMilliseconds = 0;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Determines whether to use the system as a full three phase network or a positive sequence approximation.
        /// </summary>
        [ConnectionStringParameter,
         Description("Determines whether to use the system as a full three phase network or a positive sequence approximation.")]
        public string PhaseConfiguration
        {
            get
            {
                return m_phaseConfiguration.ToString();
            }
            set
            {
                if (value.Equals("PositiveSequence"))
                {
                    m_phaseConfiguration = PhaseSelection.PositiveSequence;
                }
                else if (value.Equals("ThreePhase"))
                {
                    m_phaseConfiguration = PhaseSelection.ThreePhase;
                }
            }
        }

        /// <summary>
        /// The path name to the configuration file for the network model and measurements.
        /// </summary>
        [ConnectionStringParameter,
         Description("The path name (including the file name) to the configuration file for the network model and measurements."),
         DefaultValue("Network.xml")]
        public string ConfigurationPathName
        {
            get
            {
                return m_configurationPathName;
            }
            set
            {
                m_configurationPathName = value;
            }
        }

        /// <summary>
        /// The path name of the directory where Xml snapshots should be serialized to.
        /// </summary>
        [ConnectionStringParameter,
         Description("The path name of the directory to save snapshots.")]
        public string SnapshotPathName
        {
            get
            {
                return m_snapshotPathName;
            }
            set
            {
                m_snapshotPathName = value;
            }
        }

        /// <summary>
        /// The prefix used for the filename of the snapshot file.
        /// </summary>
        [ConnectionStringParameter,
         Description("The prefix used for the filename of the snapshot file."),
         DefaultValue("SE Snapshot ")]
        public string SnapshotFileNamePrefix
        {
            get
            {
                return m_snapshotFileNamePrefix;
            }
            set
            {
                m_snapshotFileNamePrefix = value;
            }
        }

        /// <summary>
        /// A flag which determines whether the timestamp in the filename is UTC time.
        /// </summary>
        [ConnectionStringParameter,
         Description("A flag which determines whether the timestamp in the filename is UTC time."),
         DefaultValue("true")]
        public bool UseUtcInFileNameTimestamp
        {
            get
            {
                return m_useUtcInFileName;
            }
            set
            {
                m_useUtcInFileName = value;
            }
        }

        /// <summary>
        /// A flag which represents whether the adapter supports temporal processing.
        /// </summary>
        public override bool SupportsTemporalProcessing
        {
            get
            {
                return false;
            }
        }

        #endregion

        #region [ Public Methods ]

        /// <summary>
        /// The <see cref="Initialize"/> method for the <see cref="LinearStateEstimator"/> adapter.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            m_stopwatch = new Stopwatch();
            m_totalTimeStopwatch = new Stopwatch();

            Dictionary<string, string> settings = Settings;
            string setting;

            //OnStatusMessage("Beginning reading settings...");

            if (settings.TryGetValue("ConfigurationPathName", out setting))
            {
                m_configurationPathName = setting;
            }
            if (settings.TryGetValue("SnapshotPathName", out setting))
            {
                m_snapshotPathName = setting;
            }
            if (settings.TryGetValue("SnapshotFileNamePrefix", out setting))
            {
                m_snapshotFileNamePrefix = setting;
            }
            else
            {
                m_snapshotFileNamePrefix = "SE Snapshot ";
            }
            if (settings.TryGetValue("UseUtcInFilenameTimestamp", out setting))
            {
                m_useUtcInFileName = Convert.ToBoolean(setting);
            }
            if (settings.TryGetValue("PhaseConfiguration", out setting))
            {
                m_overidePhaseConfiguration = false;
                if (setting.Equals("PositiveSequence"))
                {
                    m_phaseConfiguration = PhaseSelection.PositiveSequence;
                    m_overidePhaseConfiguration = true;
                }
                else if (setting.Equals("ThreePhase"))
                {
                    m_phaseConfiguration = PhaseSelection.ThreePhase;
                    m_overidePhaseConfiguration = true;
                }
            }

            //OnStatusMessage("Beginning network model initialization...");

            try
            {
                m_network = Network.DeserializeFromXml(m_configurationPathName);

                if (m_network == null)
                {
                    OnStatusMessage("Failed to deserialize the network model from: " + m_configurationPathName);
                }

                m_network.Initialize();
                if (m_overidePhaseConfiguration)
                {
                    m_network.Model.PhaseConfiguration = m_phaseConfiguration;
                }
                m_network.Model.AcceptsEstimates = false;
                m_network.Model.AcceptsMeasurements = true;
                m_network.SerializeData(true);
                OnStatusMessage("Successfully read configuration...");
            }
            catch (Exception exception)
            {
                OnStatusMessage(exception.ToString());
            }

            OnStatusMessage("Setting Input Measurement Keys");

            List<MeasurementKey> validInputMeasurementKeys = new List<MeasurementKey>();

            for (int i = 0; i < InputMeasurementKeys.Length; i++)
            {
                if (m_network.Model.InputMeasurementKeys.Contains(InputMeasurementKeys[i].ToString()))
                {
                    validInputMeasurementKeys.Add(InputMeasurementKeys[i]);
                }
            }

            InputMeasurementKeys = validInputMeasurementKeys.ToArray();

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="index"></param>
        protected override void PublishFrame(IFrame frame, int index)
        {
            try
            {
                m_totalTimeStopwatch.Start();

                // ------------------- Refreshing the Network Model ------------------- //
                m_stopwatch.Reset();
                m_stopwatch.Start();

                // Refresh the Network Model
                m_network.Model.InputKeyValuePairs.Clear();
                m_network.Model.ClearValues();

                m_stopwatch.Stop();
                m_refreshExecutionTime = m_stopwatch.ElapsedTicks;

                // ------------------- Frame Parsing ------------------- //

                m_stopwatch.Restart();

                // Extract the raw measurements from the frame
                foreach (IMeasurement measurement in frame.Measurements.Values)
                {
                    m_network.Model.InputKeyValuePairs.Add(measurement.Key.ToString(), measurement.Value);
                }

                m_stopwatch.Stop();
                m_parsingExecutionTime = m_stopwatch.ElapsedTicks;
                m_stopwatch.Reset();

                // ------------------- Measurement Mapping ------------------- //

                m_stopwatch.Start();

                // Alert the Network Model that new measurements have arrived.
                m_network.Model.OnNewMeasurements();

                m_stopwatch.Stop();
                m_measurementMappingExecutionTime = m_stopwatch.ElapsedTicks;
                m_stopwatch.Reset();

                // ------------------- Active Current Phasor Determination ------------------- //

                m_stopwatch.Start();
                m_network.RunNetworkReconstructionCheck();
                // Locate the current phasors measurements that are active
                if (m_network.HasChangedSincePreviousFrame)
                {
                    m_network.Model.DetermineActiveCurrentFlows();
                    m_network.Model.DetermineActiveCurrentInjections();
                }


                m_stopwatch.Stop();
                m_activeCurrentPhasorDetermintationExecutionTime = m_stopwatch.ElapsedTicks;
                m_stopwatch.Reset();

                // ------------------- Observability Analysis ------------------- //

                m_stopwatch.Start();

                // Perform observability checks and resolve network nodes to coherent observations.

                if (m_network.HasChangedSincePreviousFrame)
                {
                    m_network.Model.ResolveToObservedBuses();
                    m_network.Model.ResolveToSingleFlowBranches();
                }

                m_stopwatch.Stop();
                m_observabilityAnalysisExecutionTime = m_stopwatch.ElapsedTicks;
                m_stopwatch.Reset();


                // ------------------- State Computation ------------------- //

                m_stopwatch.Start();

                // Compute the current state of the system
                m_network.ComputeSystemState();

                m_stopwatch.Stop();
                m_stateComputationExecutionTime = m_stopwatch.ElapsedTicks;
                m_stopwatch.Reset();

                // Compute the estimated current flows from the newly computed system state
                m_network.Model.ComputeEstimatedCurrentFlows();

                // Compute the sequence values for the measured and estimated phase quantities
                if (m_network.Model.PhaseConfiguration == PhaseSelection.ThreePhase)
                {
                    m_network.Model.ComputeSequenceValues();
                }

                // ------------------- Solution Retrieval ------------------- //

                m_stopwatch.Start();

                // Get the node voltages and currents from the network
                Dictionary<string, double> rawEstimateKeyValuePairs = m_network.Model.OutputKeyValuePairs;

                m_stopwatch.Stop();
                m_solutionRetrievalExecutionTime = m_stopwatch.ElapsedTicks;
                m_stopwatch.Reset();

                // ------------------- Output Measurement Preparation ------------------- //

                m_stopwatch.Start();

                // Prepare to clone the output measurements
                IMeasurement[] outputMeasurements = OutputMeasurements;

                // Create a list of IMeasurement objects for the output;
                List<IMeasurement> output = new List<IMeasurement>();

                // Clone the measurements for output
                foreach (IMeasurement measurement in outputMeasurements)
                {
                    double value = 0;
                    if (rawEstimateKeyValuePairs.TryGetValue(measurement.Key.ToString(), out value))
                    {
                        output.Add(Measurement.Clone(measurement, value, frame.Timestamp));
                    }
                    else
                    {
                        output.Add(Measurement.Clone(measurement, 0, frame.Timestamp));
                    }
                }

                m_stopwatch.Stop();
                m_outputPreparationExecutionTime = m_stopwatch.ElapsedTicks;
                m_stopwatch.Reset();

                // ------------------- Publish Output Measurements ------------------- //

                // Output the newest set of state variables
                OnNewMeasurements(output);

                m_totalTimeStopwatch.Stop();
                m_totalExecutionTimeInTicks = m_totalTimeStopwatch.ElapsedTicks;
                m_totalExecutionTimeInMilliseconds = m_totalTimeStopwatch.ElapsedMilliseconds;
                m_totalTimeStopwatch.Reset();
            }
            catch (Exception exception)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine(exception.ToString());
                if (exception.InnerException != null)
                {
                    stringBuilder.AppendLine(exception.InnerException.ToString());
                }
                else if (exception.InnerException.InnerException != null)
                {
                    stringBuilder.AppendLine(exception.InnerException.InnerException.ToString());
                }
                using (StreamWriter outfile = new StreamWriter(m_snapshotPathName + @"\Error Log " + String.Format("{0:yyyy-MM-dd  hh-mm-ss}", DateTime.UtcNow) + ".txt"))
                {
                    outfile.Write(stringBuilder.ToString());
                }
            }
        }

        /// <summary>
        /// Serializes the current state of the system.
        /// </summary>
        [AdapterCommand("Serializes the current state of the network to Xml")]
        public void TakeSnapshot()
        {
            try
            {
                string fileName = "";
                if (m_useUtcInFileName)
                {
                    fileName = m_snapshotFileNamePrefix + String.Format("{0:yyyy-MM-dd  hh-mm-ss}", DateTime.UtcNow) + ".xml";
                }
                else
                {
                    fileName = m_snapshotFileNamePrefix + String.Format("{0:yyyy-MM-dd  hh-mm-ss}", DateTime.Now) + ".xml";
                }
                m_network.SerializeToXml(m_snapshotPathName + "/" + fileName);
                OnStatusMessage("Saved snapshot: " + m_snapshotPathName + "/" + fileName);
            }
            catch (Exception exception)
            {
                OnStatusMessage("Failed to serialize the snapshot: " + exception.InnerException.ToString());
            }
        }

        /// <summary>
        /// Writes each of the 5 matrix components of the System Matrix to UTC timestamped CSV file.
        /// </summary>
        [AdapterCommand("Writes each of the 5 matrix components of the System Matrix to UTC timestamped CSV file.")]
        public void DebugMatrices()
        {
            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                // Write to a file here
                m_network.Matrix.A.WriteToCsvFile(m_snapshotPathName);
                m_network.Matrix.II.WriteToCsvFile(m_snapshotPathName);
                m_network.Matrix.Y.WriteToCsvFile(m_snapshotPathName);
                m_network.Matrix.Ys.WriteToDebugFile(m_snapshotPathName);
                stopwatch.Stop();
                OnStatusMessage("Wrote matrices to *.csv files at " + m_snapshotPathName + " in " + stopwatch.ElapsedMilliseconds.ToString() + " ms");

            }
            catch (Exception exception)
            {
                OnStatusMessage("Failed to write the matrices to a file: " + exception.InnerException.ToString());
            }
        }

        /// <summary>
        /// Controls manual overriding of switching devices in the network model.
        /// </summary>
        [AdapterCommand("Controls manual overriding of switching devices in the network model.")]
        public void SwitchingDeviceCommand(string switchingDeviceName, string command)
        {
            if (command.ToUpper().Equals("OPEN"))
            {
                if (m_network.Model.ToggleSwitchingDeviceStatus(switchingDeviceName, SwitchingDeviceActualState.Open))
                {
                    OnStatusMessage("Successfully opened " + switchingDeviceName + ". Device now in manual override.");
                }
                else
                {
                    OnStatusMessage("Failed to open " + switchingDeviceName + ". Device remains unchanged.");
                }
            }
            else if (command.ToUpper().Equals("CLOSED"))
            {
                if (m_network.Model.ToggleSwitchingDeviceStatus(switchingDeviceName, SwitchingDeviceActualState.Closed))
                {
                    OnStatusMessage("Successfully close " + switchingDeviceName + ". Device now in manual override.");
                }
                else
                {
                    OnStatusMessage("Failed to close " + switchingDeviceName + ". Device remains unchanged.");
                }
            }
            else if (command.ToUpper().Equals("DEFAULT"))
            {
                if (m_network.Model.RemoveSwitchingDeviceFromManualWithPreserveOrDefault(switchingDeviceName, true))
                {
                    OnStatusMessage("Successfully returned " + switchingDeviceName + " to its default state. Device no longer in manual override.");
                }
                else
                {
                    OnStatusMessage("Failed to remove " + switchingDeviceName + " from manual override. Device remains unchanged.");
                }
            }
            else if (command.ToUpper().Equals("REVERTANDPRESERVE"))
            {
                if (m_network.Model.RemoveSwitchingDeviceFromManualWithPreserveOrDefault(switchingDeviceName, false))
                {
                    OnStatusMessage("Successfully allowed " + switchingDeviceName + " to receive real time data. It's present state will be preserved until it is updated by a measurement.");
                }
                else
                {
                    OnStatusMessage("Failed to remove " + switchingDeviceName + " from manual override. Device remains unchanged.");
                }
            }
        }

        /// <summary>
        /// Prints execution time of each step of state computation.
        /// </summary>
        [AdapterCommand("Prints execution time of each step of state computation.")]
        public void ShowExecutionTime()
        {
            StringBuilder status = new StringBuilder();
            status.AppendLine();
            status.AppendFormat(" Linear State Estimator Execution Time Status");
            status.AppendLine();
            status.AppendLine();
            status.AppendFormat("           Model Refresh: " + ((Convert.ToDouble(m_refreshExecutionTime)/Convert.ToDouble(m_totalExecutionTimeInTicks))*100).ToString().Substring(0, 3) + "% ( " + m_refreshExecutionTime.ToString() + " ticks ){0}", Environment.NewLine);
            status.AppendFormat("           Frame Parsing: " + ((Convert.ToDouble(m_parsingExecutionTime) / Convert.ToDouble(m_totalExecutionTimeInTicks)) * 100).ToString().Substring(0, 3) + "% ( " + m_parsingExecutionTime.ToString() + " ticks ){0}", Environment.NewLine);
            status.AppendFormat("     Measurement Mapping: " + ((Convert.ToDouble(m_measurementMappingExecutionTime) / Convert.ToDouble(m_totalExecutionTimeInTicks)) * 100).ToString().Substring(0, 3) + "% ( " + m_measurementMappingExecutionTime.ToString() + " ticks ){0}", Environment.NewLine);
            status.AppendFormat("  Observability Analysis: " + ((Convert.ToDouble(m_observabilityAnalysisExecutionTime) / Convert.ToDouble(m_totalExecutionTimeInTicks)) * 100).ToString().Substring(0, 3) + "% ( " + m_observabilityAnalysisExecutionTime.ToString() + " ticks ){0}", Environment.NewLine);
            status.AppendFormat("  Active Current Phasors: " + ((Convert.ToDouble(m_activeCurrentPhasorDetermintationExecutionTime) / Convert.ToDouble(m_totalExecutionTimeInTicks)) * 100).ToString().Substring(0, 3) + " % ( " + m_activeCurrentPhasorDetermintationExecutionTime.ToString() + " ticks ){0}", Environment.NewLine);
            status.AppendFormat("       State Computation: " + ((Convert.ToDouble(m_stateComputationExecutionTime) / Convert.ToDouble(m_totalExecutionTimeInTicks)) * 100).ToString().Substring(0, 3) + "% ( " + m_stateComputationExecutionTime.ToString() + " ticks ){0}", Environment.NewLine);
            status.AppendFormat("      Solution Retrieval: " + ((Convert.ToDouble(m_solutionRetrievalExecutionTime) / Convert.ToDouble(m_totalExecutionTimeInTicks)) * 100).ToString().Substring(0, 3) + "% ( " + m_solutionRetrievalExecutionTime.ToString() + " ticks ){0}", Environment.NewLine);
            status.AppendFormat("      Output Preparation: " + ((Convert.ToDouble(m_outputPreparationExecutionTime) / Convert.ToDouble(m_totalExecutionTimeInTicks)) * 100).ToString().Substring(0, 3) + "% ( " + m_outputPreparationExecutionTime.ToString() + " ticks ){0}", Environment.NewLine);
            status.AppendLine();
            status.AppendFormat("    Total Execution Time: " + m_totalExecutionTimeInMilliseconds.ToString() + " ms | ( " + m_totalExecutionTimeInTicks.ToString() + " ){0}", Environment.NewLine);
            status.AppendLine();

            OnStatusMessage(status.ToString());
        }

        #endregion
    }
}
