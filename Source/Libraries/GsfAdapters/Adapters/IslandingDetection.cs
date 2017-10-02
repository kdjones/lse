//******************************************************************************************************
//  IslandingDetection.cs
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
//
//******************************************************************************************************
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using GSF.TimeSeries;
using GSF.TimeSeries.Adapters;
using GSF;
using GSF.PhasorProtocols;
using LinearStateEstimator.Islanding;
using LinearStateEstimator.Networks;

namespace LinearStateEstimator.Adapters
{
    /// <summary>
    /// Uses a predetermined decision tree to detect certain islanding conditions in the DVP network.
    /// </summary>
    [Description("Islanding Detection: Uses a predetermined decision tree to detect certain islanding conditions in the DVP network.")]
    public class IslandingDetection : ActionAdapterBase
    {
        #region [ Private Members ]

        private bool m_verboseInitialization;
        private bool m_useRealTimeData;
        private bool m_useSampleDataSet1;
        private bool m_useSampleDataSet2;
        private bool m_useSampleDataSet3;
        private bool m_useSampleDataSet4;
        private bool m_useSampleDataSet5;
        private bool m_useSampleDataSet6;
        private IslandingAnalyzer m_islandingAnalyzer;
        private string m_configurationPathName;
        private Network m_network;

        #endregion
        
        #region [ Properties ]

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
        /// A boolean flag which determines whether to output detailed information during initialization.
        /// </summary>
        [ConnectionStringParameter,
         Description("A boolean flag which determines whether to output detailed information during initialization."),
         DefaultValue("true")]
        public bool VerboseInitialization
        {
            get
            {
                return m_verboseInitialization;
            }
            set
            {
                m_verboseInitialization = value;
            }
        }

        /// <summary>
        /// A boolean flag which determines whether to use the pre-programmed sample cases as a demonstration.
        /// </summary>
        [ConnectionStringParameter,
         Description("A boolean flag which determines whether to use the pre-programmed sample cases as a demonstration."),
         DefaultValue("true")]
        public bool UseRealTimeData
        {
            get
            {
                return m_useRealTimeData;
            }
            set
            {
                m_useRealTimeData = value;
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
        /// 
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            Dictionary<string, string> settings = Settings;
            string setting;

            if (settings.TryGetValue("VerboseInitialization", out setting))
            {
                m_verboseInitialization = Convert.ToBoolean(setting);
                if (m_verboseInitialization)
                {
                    OnStatusMessage("VerboseInitialization: " + setting);
                }
            }
            else
            {
                m_verboseInitialization = false;
            }
            if (settings.TryGetValue("UseRealTimeData", out setting))
            {
                m_useRealTimeData = Convert.ToBoolean(setting);
                if (m_verboseInitialization)
                {
                    OnStatusMessage("UseRealTimeData: " + setting);
                }
            }
            else
            {
                m_useRealTimeData = true;
            }
            if (settings.TryGetValue("ConfigurationPathName", out setting))
            {
                m_configurationPathName = setting;
                if (m_verboseInitialization)
                {
                    OnStatusMessage("ConfigurationPathName: " + setting);
                }
            }

            // Initialize the data set controls
            if (m_useRealTimeData)
            {
                m_useSampleDataSet1 = false;
                m_useSampleDataSet2 = false;
                m_useSampleDataSet3 = false;
                m_useSampleDataSet4 = false;
                m_useSampleDataSet5 = false;
                m_useSampleDataSet6 = false;
            }
            else
            {
                m_useSampleDataSet1 = true;
                m_useSampleDataSet2 = false;
                m_useSampleDataSet3 = false;
                m_useSampleDataSet4 = false;
                m_useSampleDataSet5 = false;
                m_useSampleDataSet6 = false;
            }

            m_islandingAnalyzer = new IslandingAnalyzer();

            //if (m_verboseInitialization)
            //{
            //    OnStatusMessage("Attempting to read configuration...");
            //}
            //try
            //{
            //    m_network = Network.DeserializeFromXml(m_configurationPathName);
            //    m_network.Initialize();
            //    if (m_verboseInitialization)
            //    {
            //        OnStatusMessage("Successfully read configuration...");
            //    }
            //}
            //catch (Exception exception)
            //{
            //    OnStatusMessage(exception.InnerException.ToString());
            //}
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="index"></param>
        protected override void PublishFrame(IFrame frame, int index)
        {

            double[] data = new double[116];

            if (m_useRealTimeData)
            {
                //foreach (IMeasurement measurement in frame.Measurements.Values)
                //{
                //    m_network.Model.RawMeasurementKeyValuePairs.Add(measurement.Key.ToString(), measurement.Value);
                //}

                //m_network.Model.OnNewMeasurements();

                // Angles are unwrapped and referenced and other values are in per unit.
                //
                // data[2] = Voltage Angle at Bristers Substation
                // data[4] = Voltage Angle at Bath County Substation
                // data[8] = Voltage Angle at Chickahominy Substation
                // data[14] = Voltage Angle at Clover Substation
                // data[40] = Voltage Angle at Ox Substation
                // data[42] = Voltage Angle at Possum Point Substation
                // data[46] = Voltage Angle at Surry Substation
                // data[50] = Voltage Angle at Valley Substation
                // data[54] = Voltage Angle at Suffolk Substation
                // data[55] = Real Part of the Current flowing from Bristers Substation to Ox Substation
                // data[59] = Real Part of the Current flowing from Bristers Substation to Chancellor Substation
                // data[61] = Real Part of teh Current flowing from Bath County Pump Storage to Lexington Substation
                // data[62] = Imaginary Part of the Current flowing from Bath County Pump Storage to Valley Substation
                // data[63] = Real Part of the Current flowing from Bath County Pump Storage to Lexington Substation
                // data[70] = Imaginary Part of the Current flowing from Carson Substation to Midlothian Substation
                // data[74] = Imaginary Part of the Current flowing from Chickahominy Substation to Elmont Substation
                // data[77] = Real Part of the Current flowing rom Clifton to Loudoun
                // data[80] = Imaginary Part of the Current flowing form Chancellor Substation to Ladysmith substation
                // data[82] = Imaginary Part of the Current flowing from Dooms Substation to Valley Substation
                // data[83] = Real Part of the Curren tflowing from Dooms Substation to Lexington Substation
                // data[84] = Imaginary Part of the Current flowing from Dooms Substation to LExington Substation.
            }
            else if (m_useSampleDataSet1)
            {
                data = m_islandingAnalyzer.SampleData1;
            }
            else if (m_useSampleDataSet2)
            {
                data = m_islandingAnalyzer.SampleData2;
            }
            else if (m_useSampleDataSet3)
            {
                data = m_islandingAnalyzer.SampleData3;
            }
            else if (m_useSampleDataSet4)
            {
                data = m_islandingAnalyzer.SampleData4;
            }
            else if (m_useSampleDataSet5)
            {
                data = m_islandingAnalyzer.SampleData5;
            }
            else if (m_useSampleDataSet6)
            {
                data = m_islandingAnalyzer.SampleData6;
            }

            // Process the measurements for the decision tree;
            double[] processedMeasurements = m_islandingAnalyzer.varCovariance(data);

            // Use the decision tree to determine the presence of, location of, severity of , and stability of an islanding condition
            m_islandingAnalyzer.islandingJudgment(processedMeasurements);
            m_islandingAnalyzer.severityRanking(processedMeasurements);

            // Prepare to clone the output measurements
            IMeasurement[] outputMeasurements = OutputMeasurements;

            // Create a list of IMeasurement objects for the output;
            List<IMeasurement> output = new List<IMeasurement>();

            // Clone the measurements for output
            output.Add(Measurement.Clone(outputMeasurements[0], (int)m_islandingAnalyzer.Islanding, frame.Timestamp));
            output.Add(Measurement.Clone(outputMeasurements[1], (int)m_islandingAnalyzer.Location, frame.Timestamp));
            output.Add(Measurement.Clone(outputMeasurements[2], (int)m_islandingAnalyzer.Severity, frame.Timestamp));
            output.Add(Measurement.Clone(outputMeasurements[3], (int)m_islandingAnalyzer.Stability, frame.Timestamp));

            // Output the newest set of islanding flags
            OnNewMeasurements(output);
        }

        /// <summary>
        /// Switches between real-time data and six sample data sets.
        /// </summary>
        [AdapterCommand("Switches between real-time data and six sample data sets.")]
        public void ToggleDataStream()
        {
            if (m_useRealTimeData)
            {
                m_useRealTimeData = false;
                m_useSampleDataSet1 = true;
                m_useSampleDataSet2 = false;
                m_useSampleDataSet3 = false;
                m_useSampleDataSet4 = false;
                m_useSampleDataSet5 = false;
                m_useSampleDataSet6 = false;
                OnStatusMessage("Will now use Sample Data Set 1...");
            }
            else if (m_useSampleDataSet1)
            {
                m_useRealTimeData = false;
                m_useSampleDataSet1 = false;
                m_useSampleDataSet2 = true;
                m_useSampleDataSet3 = false;
                m_useSampleDataSet4 = false;
                m_useSampleDataSet5 = false;
                m_useSampleDataSet6 = false;
                OnStatusMessage("Will now use Sample Data Set 2...");
            }
            else if (m_useSampleDataSet2)
            {
                m_useRealTimeData = false;
                m_useSampleDataSet1 = false;
                m_useSampleDataSet2 = false;
                m_useSampleDataSet3 = true;
                m_useSampleDataSet4 = false;
                m_useSampleDataSet5 = false;
                m_useSampleDataSet6 = false;
                OnStatusMessage("Will now use Sample Data Set 3...");
            }
            else if (m_useSampleDataSet3)
            {
                m_useRealTimeData = false;
                m_useSampleDataSet1 = false;
                m_useSampleDataSet2 = false;
                m_useSampleDataSet3 = false;
                m_useSampleDataSet4 = true;
                m_useSampleDataSet5 = false;
                m_useSampleDataSet6 = false;
                OnStatusMessage("Will now use Sample Data Set 4...");
            }
            else if (m_useSampleDataSet4)
            {
                m_useRealTimeData = false;
                m_useSampleDataSet1 = false;
                m_useSampleDataSet2 = false;
                m_useSampleDataSet3 = false;
                m_useSampleDataSet4 = false;
                m_useSampleDataSet5 = true;
                m_useSampleDataSet6 = false;
                OnStatusMessage("Will now use Sample Data Set 5...");
            }
            else if (m_useSampleDataSet5)
            {
                m_useRealTimeData = false;
                m_useSampleDataSet1 = false;
                m_useSampleDataSet2 = false;
                m_useSampleDataSet3 = false;
                m_useSampleDataSet4 = false;
                m_useSampleDataSet5 = false;
                m_useSampleDataSet6 = true;
                OnStatusMessage("Will now use Sample Data Set 6...");
            }
            else if (m_useSampleDataSet6)
            {
                m_useRealTimeData = true;
                m_useSampleDataSet1 = false;
                m_useSampleDataSet2 = false;
                m_useSampleDataSet3 = false;
                m_useSampleDataSet4 = false;
                m_useSampleDataSet5 = false;
                m_useSampleDataSet6 = false;
                OnStatusMessage("Will now use Real Time Data...");
            }
        }

        /// <summary>
        /// Shows current status of all of the islanding flags on the console.
        /// </summary>
        [AdapterCommand("Shows current status of all of the islanding flags on the console.")]
        public void ShowIslandingStatus()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine();
            stringBuilder.AppendFormat("---------- Islanding Detection Status ------------------------------------------");
            stringBuilder.AppendLine();
            stringBuilder.AppendFormat("     Islanding Condition: " + m_islandingAnalyzer.Islanding.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("          Severity Index: " + m_islandingAnalyzer.Severity.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("                Location: " + m_islandingAnalyzer.Location.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("               Stability: " + m_islandingAnalyzer.Stability.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendLine();
            stringBuilder.AppendFormat("--------------------------------------------------------------------------------");
            OnStatusMessage(stringBuilder.ToString());
        }

        #endregion
    }
}
