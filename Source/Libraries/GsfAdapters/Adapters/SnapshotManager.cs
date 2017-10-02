//******************************************************************************************************
//  SnapshotManager.cs
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
//  05/12/2014 - Kevin D. Jones
//       Removed references to Extreme Optimizations
//
//******************************************************************************************************
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using GSF;
using GSF.PhasorProtocols;
using GSF.TimeSeries;
using GSF.TimeSeries.Adapters;
using SynchrophasorAnalytics.Measurements;
using SynchrophasorAnalytics.Networks;
using SynchrophasorAnalytics.Testing;

namespace SynchrophasorAnalytics.Adapters
{
    /// <summary>
    /// Represents an adapter that manages automatic and manual snapshot retreival operations.
    /// </summary>
    [Description("Snapshot Manager : Controls Scheduled and Manual System Snapshots")]
    public class SnapshotManager : ActionAdapterBase
    {
        #region [ Private Members ]

        private string m_configurationPathName;
        private string m_snapshotPathName;
        private string m_snapshotFileNamePrefix;
        private bool m_useUtcInFileName;
        private Network m_network;
        private bool m_acceptsMeasurements;
        private bool m_acceptsEstimates;
        private PhaseSelection m_phaseConfiguration;
        private string m_schedule;
        private List<SnapshotRequest> m_snapshotRequests;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// A series of time stamps separated by commas which represent the set of scheduled snapshot requests.
        /// </summary>
        [ConnectionStringParameter,
         Description("A series of time stamps separated by commas which represent the set of scheduled snapshot requests.")]
        public string Schedule
        {
            get
            {
                return m_schedule;
            }
            set
            {
                m_schedule = value;
            }
        }

        /// <summary>
        /// A flag which represents whether the Snapshot Manager accepts measured values.
        /// </summary>
        [ConnectionStringParameter,
         Description("A flag which represents whether the Snapshot Manager accepts measured values."),
         DefaultValue(true)]
        public bool AcceptsMeasurements
        {
            get
            {
                return m_acceptsMeasurements;
            }
            set
            {
                m_acceptsMeasurements = value;
            }
        }

        /// <summary>
        /// A flag which represents whether the Snapshot Manager accepts estimated values.
        /// </summary>
        [ConnectionStringParameter,
         Description("A flag which represents whether the Snapshot Manager accepts estimated values."),
         DefaultValue(true)]
        public bool AcceptsEstimates
        {
            get
            {
                return m_acceptsEstimates;
            }
            set
            {
                m_acceptsEstimates = value;
            }
        }


        /// <summary>
        /// The path name to the configuration file for the network measurements.
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
         DefaultValue("Snapshot")]
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
            if (settings.TryGetValue("PhaseConfiguration", out setting))
            {
                if (setting.Equals("PositiveSequence"))
                {
                    m_phaseConfiguration = PhaseSelection.PositiveSequence;
                }
                else if (setting.Equals("ThreePhase"))
                {
                    m_phaseConfiguration = PhaseSelection.ThreePhase;
                }
            }
            else
            {
                m_phaseConfiguration = PhaseSelection.ThreePhase;
            }
            if (settings.TryGetValue("Schedule", out setting))
            {
                m_schedule = setting;

                CreateSnapshotRequests();
            }
            OnStatusMessage("Attempting to read configuration...");

            try
            {
                m_network = Network.DeserializeFromXml(m_configurationPathName);
                m_network.Initialize();
                m_network.Model.PhaseConfiguration = m_phaseConfiguration;
                m_network.Model.AcceptsEstimates = m_acceptsEstimates;
                m_network.Model.AcceptsMeasurements = m_acceptsMeasurements;
                m_network.SerializeData(true);
                OnStatusMessage("Successfully read configuration...");
            }
            catch (Exception exception)
            {
                OnStatusMessage(exception.InnerException.ToString());
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
            // Refresh the Network Model
            m_network.Model.InputKeyValuePairs.Clear();
            m_network.Model.ClearValues();

            // Extract the raw measurements from the frame
            foreach (IMeasurement measurement in frame.Measurements.Values)
            {
                m_network.Model.InputKeyValuePairs.Add(measurement.Key.ToString(), measurement.Value);
            }

            // Alert the Network Model that new measurements have arrived.
            m_network.Model.OnNewMeasurements();

            foreach (SnapshotRequest snapshotRequest in m_snapshotRequests)
            {
                if ((!snapshotRequest.IsComplete) && ((snapshotRequest.RequestTime - DateTime.Now).TotalSeconds == 0))
                {
                    TakeSnapshot();
                    snapshotRequest.IsComplete = true;
                }
            }

            if (DateTime.Now.Hour == 0 && DateTime.Now.Minute == 0 && DateTime.Now.Second == 0)
            {
                CreateSnapshotRequests();
            }
        }

        /// <summary>
        /// 
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

        #endregion


        private void CreateSnapshotRequests()
        {
            try
            {
                m_snapshotRequests.Clear();

                string[] requests = m_schedule.Split(',');

                for (int i = 0; i < requests.Count(); i++)
                {
                    string[] requestTime = requests[i].Split(':');
                    int hour = 0;
                    int minute = 0;
                    int second = 0;
                    if (requestTime.Count() > 0)
                    {
                        hour = Convert.ToInt32(requestTime[0]);
                    }
                    if (requestTime.Count() > 1)
                    {
                        minute = Convert.ToInt32(requestTime[1]);
                    }
                    if (requestTime.Count() > 2)
                    {
                        second = Convert.ToInt32(requestTime[2]);
                    }
                    if (!((DateTime.Now - new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, hour, minute, second)).TotalSeconds > 0))
                    {
                        m_snapshotRequests.Add(new SnapshotRequest(new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, hour, minute, second)));
                    }
                }
            }
            catch (Exception exception)
            {
                OnStatusMessage("Syntax error in snapshot schedule parameter: " + exception.ToString());
            }
        }
    }
}
