//******************************************************************************************************
//  MeasurementSampler.cs
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
//  09/24/2013 - Kevin D. Jones
//       Added periodic snapshot capability
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
using SynchrophasorAnalytics.Measurements;
using SynchrophasorAnalytics.Testing;

namespace SynchrophasorAnalytics.Adapters
{
    /// <summary>
    /// Represents an adapter that manages automatic and manual raw measurement retreival operations.
    /// </summary>
    [Description("Measurement Sampler : Controls Scheduled and Manual Measurement Samples")]
    public class MeasurementSampler : ActionAdapterBase
    {
        #region [ Private Members ]

        private string m_rawMeasurementsPathName;
        private string m_rawMeasurementsFileNamePrefix;
        private bool m_useUtcInFileName;
        private RawMeasurements m_rawMeasurements;
        private bool m_takePeriodicSnapshots;
        private int m_snapshotPeriodicity;
        private int m_totalFrames;

        #endregion

        #region [ Properties ]
        
        /// <summary>
        /// The path name of the directory where serialized raw measurements should be saved to.
        /// </summary>
        [ConnectionStringParameter,
         Description("The path name of the directory to save serialized raw measurements.")]
        public string RawMeasurementsPathName
        {
            get
            {
                return m_rawMeasurementsPathName;
            }
            set
            {
                m_rawMeasurementsPathName = value;
            }
        }

        /// <summary>
        /// The prefix used for the filename of the snapshot file.
        /// </summary>
        [ConnectionStringParameter,
         Description("The prefix used for the filename of the snapshot file."),
         DefaultValue("RawMeasurements")]
        public string RawMeasurementsFileNamePrefix
        {
            get
            {
                return m_rawMeasurementsFileNamePrefix;
            }
            set
            {
                m_rawMeasurementsFileNamePrefix = value;
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
        /// The period of periodically scheduled snapshots in seconds.
        /// </summary>
        [ConnectionStringParameter,
         Description("The period of periodically scheduled snapshots in seconds."),
         DefaultValue("3600")]
        public int SnapshotPeriodicity
        {
            get
            {
                return m_snapshotPeriodicity;
            }
            set
            {
                m_snapshotPeriodicity = value;
            }
        }

        /// <summary>
        /// A boolean flag which represents whether the measurement sampler should take snapshots periodically based on the SnapshotPeriodicity parameter
        /// </summary>
        [ConnectionStringParameter,
         Description("A boolean flag which represents whether the measurement sampler should take snapshots periodically based on the SnapshotPeriodicity parameter"),
         DefaultValue("true")]
        public bool ExecutePeriodically
        {
            get
            {
                return m_takePeriodicSnapshots;
            }
            set
            {
                m_takePeriodicSnapshots = value;
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
            m_rawMeasurements = new RawMeasurements();

            Dictionary<string, string> settings = Settings;
            string setting;

            if (settings.TryGetValue("RawMeasurementsPathName", out setting))
            {
                m_rawMeasurementsPathName = setting;
                OnStatusMessage("Loaded RawMeasurementsPathName: " + setting);
            }
            if (settings.TryGetValue("RawMeasurementsFileNamePrefix", out setting))
            {
                m_rawMeasurementsFileNamePrefix = setting;
                OnStatusMessage("Loaded RawMeasurementsFileNamePrefix: " + setting);
            }
            else
            {
                m_rawMeasurementsFileNamePrefix = "RawMeasurements";
            }
            if (settings.TryGetValue("UseUtcInFileNameTimestamp", out setting))
            {
                m_useUtcInFileName = Convert.ToBoolean(setting);
                OnStatusMessage("Loaded UseUtcInFileNameTimestamp: " + setting);
            }
            if (settings.TryGetValue("SnapshotPeriodicity", out setting))
            {
                m_snapshotPeriodicity = Convert.ToInt16(setting);
            }
            else
            {
                m_snapshotPeriodicity = 3600; // hourly
            }
            if (settings.TryGetValue("ExecutePeriodically", out setting))
            {
                m_takePeriodicSnapshots = Convert.ToBoolean(setting);
                OnStatusMessage("Loaded ExecutePeriodically: " + setting);
            }
            else
            {
                m_takePeriodicSnapshots = true;
            }

            m_totalFrames = 0;
        }

        protected override void PublishFrame(IFrame frame, int index)
        {
            List<RawMeasurementsMeasurement> rawMeasurementsMeasurement = new List<RawMeasurementsMeasurement>();

            foreach (IMeasurement measurement in frame.Measurements.Values)
            {
                RawMeasurementsMeasurement newMeasurement = new RawMeasurementsMeasurement();
                newMeasurement.Key = measurement.Key.ToString();
                newMeasurement.Value = measurement.Value.ToString();
                rawMeasurementsMeasurement.Add(newMeasurement);
            }

            m_rawMeasurements.Items = rawMeasurementsMeasurement.ToArray();
            
            m_totalFrames++;

            if (m_takePeriodicSnapshots && (m_totalFrames % FramesPerSecond == 0) && (((m_totalFrames / FramesPerSecond) % m_snapshotPeriodicity) == 0))
            {
                GetSample();
            }
        }

        /// <summary>
        /// Serializes the current set of raw measurements of the network to Xml.
        /// </summary>
        [AdapterCommand("Serializes the current set of raw measurements of the network to Xml.")]
        public void GetSample()
        {
            try
            {
                string fileName = "";
                if (m_useUtcInFileName)
                {
                    fileName = m_rawMeasurementsFileNamePrefix + String.Format(" {0:yyyy-MM-dd  hh-mm-ss}", DateTime.UtcNow) + ".xml";
                }
                else
                {
                    fileName = m_rawMeasurementsFileNamePrefix + String.Format(" {0:yyyy-MM-dd  hh-mm-ss}", DateTime.Now) + ".xml";
                }

                // Create an XmlSerializer with the type of Network
                XmlSerializer serializer = new XmlSerializer(typeof(RawMeasurements));

                // Open a connection to the file and path.
                TextWriter writer = new StreamWriter(m_rawMeasurementsPathName + "/" + fileName);

                // Serialize this instance of NetworkMeasurements
                serializer.Serialize(writer, m_rawMeasurements);

                // Close the connection
                writer.Close();

                OnStatusMessage("Saved snapshot: " + m_rawMeasurementsPathName + "/" + fileName);
            }
            catch (Exception exception)
            {
                OnStatusMessage("Failed to serialize the snapshot: " + exception.InnerException.ToString());
            }
        }

        #endregion

    }
}
