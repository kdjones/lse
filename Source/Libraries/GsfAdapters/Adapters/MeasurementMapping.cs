//******************************************************************************************************
//  MeasurementMapping.cs
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
//  08/01/2012 - Kevin D. Jones
//       Generated original version of source code.
//  04/01/2013 - Kevin D. Jones
//       Switched to using Dictionary for sorting input and output measurements. Added error code 9999
//       to represent a problem or missing measurement.
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

namespace SynchrophasorAnalytics.Adapters
{
    /// <summary>
    /// Forwards/maps measurements in a FIFO basis relative to the input and output measurement definitions in the connection string.
    /// </summary>
    [Description("Measurement Mapping : Forwards/maps measurements in a FIFO basis.")]
    public class MeasurementMapping : ActionAdapterBase
    {
        #region [ Private Members ]

        private MeasurementKey[] m_inputMeasurementKeys;
        private IMeasurement[] m_outputMeasurements;
        private double[] m_unmappedMeasurements;
        private IFrame m_currentFrame;

        #endregion

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

        public override void Initialize()
        {
            // Initialize the adapter in openPDC
            base.Initialize();

            // Get the input measurements keys from the configuration information
            m_inputMeasurementKeys = InputMeasurementKeys;

            // Create a space to store the measurements
            m_unmappedMeasurements = new double[m_inputMeasurementKeys.Length];

            // Initialize the measurement values with zeros so they are never null;
            for (int i = 0; i < m_unmappedMeasurements.Length; i++)
            {
                m_unmappedMeasurements[i] = 0;
            }

            // Get the blank output measurements from the configuration information
            m_outputMeasurements = OutputMeasurements;
        }

        protected override void PublishFrame(IFrame frame, int index)
        {
            // Save the frame
            m_currentFrame = frame;

            // Create a dictionary to store the input measurement key value pairs 
            Dictionary<MeasurementKey, double> inputMeasurements = new Dictionary<MeasurementKey, double>();

            // Extract the raw measurements from the frame
            foreach (IMeasurement measurement in frame.Measurements.Values)
            {
                inputMeasurements.Add(measurement.Key, measurement.Value);
            }

            // Create an empty List<> to store the PJM measurements
            List<IMeasurement> translatedMeasurements = new List<IMeasurement>();

            // Use the input measurement keys to map measurements on a FIFO basis
            for (int i = 0; i < m_inputMeasurementKeys.Length; i++)
            {
                if (!inputMeasurements.TryGetValue(m_inputMeasurementKeys[i], out m_unmappedMeasurements[i]))
                {
                    m_unmappedMeasurements[i] = 9999;
                }
            }

            // If enough keys were specified to have a one-to-one mapping...
            if (m_outputMeasurements.Length == m_unmappedMeasurements.Length)
            {
                // Prepare the measurements to be output
                for (int i = 0; i < m_outputMeasurements.Length; i++)
                {
                    translatedMeasurements.Add(Measurement.Clone(m_outputMeasurements[i], m_unmappedMeasurements[i], frame.Timestamp));
                }
            }
            else // Output all 9999
            {
                for (int i = 0; i < m_outputMeasurements.Length; i++)
                {
                    translatedMeasurements.Add(Measurement.Clone(m_outputMeasurements[i], 9999, frame.Timestamp));
                }
            }

            // Output the translated measurements
            OnNewMeasurements(translatedMeasurements);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        public override string GetShortStatus(int maxLength)
        {
            return string.Format("Expecting: {0} Receiving: {1} Sending: {2}",
                                  m_inputMeasurementKeys.Length,
                                  m_currentFrame.Measurements.Count(),
                                  m_outputMeasurements.Length).PadLeft(maxLength);
        }

        /// <summary>
        /// 
        /// </summary>
        public override string Status
        {
            get
            {
                StringBuilder status = new StringBuilder();
                status.Append(base.Status);
                status.AppendLine();
                status.AppendFormat("   Basic Measurement Mapping Information");
                status.AppendLine();
                status.AppendLine();
                status.AppendFormat("   Specified {0} Input Measurements", m_inputMeasurementKeys.Length);
                status.AppendLine();
                status.AppendFormat("    Received {0} Input Measurements ", m_currentFrame.Measurements.Count());
                status.AppendLine();
                status.AppendFormat("   Specified {0} Output Measurements ", m_outputMeasurements.Length);
                status.AppendLine();

                return status.ToString();
            }
        }
    }
}
