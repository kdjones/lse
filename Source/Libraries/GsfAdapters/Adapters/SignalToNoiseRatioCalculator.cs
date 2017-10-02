//******************************************************************************************************
//  SignalToNoiseRatioCalculator.cs
//
//  Copyright © 2014, Kevin D. Jones.  All Rights Reserved.
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
//  04/05/2014 - Kevin D. Jones
//       Generated original version of source code in C#
//
//******************************************************************************************************
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GSF.TimeSeries;
using GSF.TimeSeries.Adapters;
using GSF;
using GSF.PhasorProtocols;
using SynchrophasorAnalytics.DataConditioning.Snr;

namespace SynchrophasorAnalytics.Adapters
{
    [Description("Signal-to-Noise Ratio Calculator : Calculates SNR for given input signals.")]
    public class SignalToNoiseRatioCalculator : ActionAdapterBase
    {
        #region [ Private Fields ]

        private const int NOT_MODELED = 8888;
        private string m_configurationPathName;
        private string m_snapshotPathName;
        private string m_snapshotFileNamePrefix;
        private bool m_useUtcInFileName;
        private SnrMovingWindowCollection m_movingWindows;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// The path name to the configuration file for the network measurements.
        /// </summary>
        [ConnectionStringParameter,
         Description("The path name (including the file name) to the configuration file for the network model and measurements."),
         DefaultValue("SnrCalculatorConfiguration.xml")]
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
         DefaultValue("Snr Moving Window ")]
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
         DefaultValue(true)]
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

        public override void Initialize()
        {
            base.Initialize();

            GetAdapterSettings();

            InitializeMovingWindowCollection();

            SetValidInputMeasuremenetKeys();
        }

        protected override void PublishFrame(IFrame frame, int index)
        {
            foreach (IMeasurement measurement in frame.Measurements.Values)
            {
                m_movingWindows.AddNewMeasurement(measurement);
            }

            // Prepare to clone the output measurements
            IMeasurement[] outputMeasurements = OutputMeasurements;

            // Create a list of IMeasurement objects for the output;
            List<IMeasurement> output = new List<IMeasurement>();

            // Clone the measurements for output
            foreach (IMeasurement measurement in outputMeasurements)
            {
                SnrMovingWindow window = null;
                if (m_movingWindows.WindowsKeyedByOutput.TryGetValue(measurement.Key.ToString(), out window))
                {
                    output.Add(Measurement.Clone(measurement, window.SignalToNoiseRatio, frame.Timestamp));
                }
                else
                {
                    output.Add(Measurement.Clone(measurement, NOT_MODELED, frame.Timestamp));
                }
            }

            OnNewMeasurements(output);
        }

        #endregion

        #region [ Private Methods ]

        private void GetAdapterSettings()
        {
            Dictionary<string, string> settings = Settings;
            string setting;

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
            if (settings.TryGetValue("UseUtcInFileNameTimestamp", out setting))
            {
                m_useUtcInFileName = Convert.ToBoolean(setting);
            }
        }

        private void InitializeMovingWindowCollection()
        {
            try
            {
                m_movingWindows = SnrMovingWindowCollection.DeserializeFromXml(m_configurationPathName);

                if (m_movingWindows == null)
                {
                    OnStatusMessage("Failed to deserialize the moving window collection from: " + m_configurationPathName);
                }

                m_movingWindows.Initialize();

                OnStatusMessage("Successfully read configuration...");
            }
            catch (Exception exception)
            {
                OnStatusMessage(exception.ToString());
            }
        }

        private void SetValidInputMeasuremenetKeys()
        {
            List<MeasurementKey> validInputMeasurementKeys = new List<MeasurementKey>();
            List<string> movingWindowMeasurementKeys = m_movingWindows.InputMeasurementKeys;

            for (int i = 0; i < InputMeasurementKeys.Length; i++)
            {
                if (movingWindowMeasurementKeys.Contains(InputMeasurementKeys[i].ToString()))
                {
                    validInputMeasurementKeys.Add(InputMeasurementKeys[i]);
                }
            }

            InputMeasurementKeys = validInputMeasurementKeys.ToArray();

            OnStatusMessage("Set Valid Input Measurement Keys...");
        }

        #endregion

        #region [ Adapter Commands ]

        [AdapterCommand("Serializes the current state of the moving window collection.")]
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
                m_movingWindows.SerializeToXml(m_snapshotPathName + "/" + fileName);
                OnStatusMessage("Saved snapshot: " + m_snapshotPathName + "/" + fileName);
            }
            catch (Exception exception)
            {
                OnStatusMessage("Failed to serialize the snapshot: " + exception.ToString());
                if (exception.InnerException != null)
                {
                    OnStatusMessage(exception.InnerException.ToString());
                }
            }
        }

        [AdapterCommand("Enables data serialization for the signals in the moving windows.")]
        public void EnableData()
        {
            m_movingWindows.EnableDataSerialization();
        }

        [AdapterCommand("Disables data serialization for the signals in the moving windows.")]
        public void DisableData()
        {
            m_movingWindows.DisableDataSerialization();
        }

        [AdapterCommand("Computes the average of the moving window.")]
        public void Mean()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("-------------------- MEAN --------------------");
            stringBuilder.AppendLine(" Type       InputKey            OutputKey               Value");
            stringBuilder.AppendLine("");
            foreach (SnrMovingWindow movingWindow in m_movingWindows.Windows)
            {
                string type = movingWindow.MeasurementType.ToString();
                string inputKey = movingWindow.InputMeasurementKey;
                string outputKey = movingWindow.OutputMeasurementKey;
                string value = movingWindow.ArithmeticMean.ToString();
                stringBuilder.AppendFormat(" {0,-11}{1,-20}{2,-20}{3}\n", type, inputKey, outputKey, value);
            }

            OnStatusMessage(stringBuilder.ToString());
        }

        [AdapterCommand("Computes the standard deviation of the moving window.")]
        public void Std()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("------------- STANDARD DEVIATION -------------");
            stringBuilder.AppendLine(" Type       InputKey            OutputKey               Value");
            stringBuilder.AppendLine("");
            foreach (SnrMovingWindow movingWindow in m_movingWindows.Windows)
            {
                string type = movingWindow.MeasurementType.ToString();
                string inputKey = movingWindow.InputMeasurementKey;
                string outputKey = movingWindow.OutputMeasurementKey;
                string value = movingWindow.StandardDeviation.ToString();
                stringBuilder.AppendFormat(" {0,-11}{1,-20}{2,-20}{3}\n", type, inputKey, outputKey, value);
            }

            OnStatusMessage(stringBuilder.ToString());
        }

        [AdapterCommand("Computes the signal-to-noise ratio of the moving window.")]
        public void Snr()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("------------ SIGNAL-TO-NOISE RATIO -----------");
            stringBuilder.AppendLine(" Type       InputKey            OutputKey               Value");
            stringBuilder.AppendLine("");
            foreach (SnrMovingWindow movingWindow in m_movingWindows.Windows)
            {
                string type = movingWindow.MeasurementType.ToString();
                string inputKey = movingWindow.InputMeasurementKey;
                string outputKey = movingWindow.OutputMeasurementKey;
                string value = movingWindow.SignalToNoiseRatio.ToString();
                stringBuilder.AppendFormat(" {0,-11}{1,-20}{2,-20}{3}\n", type, inputKey, outputKey, value);
            }

            OnStatusMessage(stringBuilder.ToString());
        }

        #endregion
    }
}
