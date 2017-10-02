//******************************************************************************************************
//  PhasorSignalConditioner.cs
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
using SynchrophasorAnalytics.Measurements;
using SynchrophasorAnalytics.Modeling;
using SynchrophasorAnalytics.DataConditioning.Smoothing;

namespace SynchrophasorAnalytics.Adapters
{
    /// <summary>
    /// Phasor Signal Conditioner Adapter: Mitigates drop-outs, repeated values, and outliers in streaming PMU data.
    /// </summary>
    [Description("Phasor Signal Conditioner : Mitigates drop-outs, repeated values, and outliers in streaming PMU data.")]
    public class PhasorSignalConditioner : ActionAdapterBase
    {
        #region [ Private Constants ]

        private const int MAX_BUFFER_SIZE = 3; // Marks the maximum number of measurements to keep in the measurement buffer
        private const int BUFFER_END_POSITION = 0; // Marks the index of the bottom of the measurement buffer
        private const int BUFFER_MIDDLE_POSITION = 1; // Marks the index of the middle of the measurement buffer
        private const int BUFFER_TOP_POSITION = 2; // Marks the index of the top of the measurement buffer

        #endregion

        #region [ Private Fields ]

        private string m_magnitudeInputKey;
        private string m_magnitudeOutputKey;
        private string m_angleInputKey;
        private string m_angleOutputKey;
        private Smoother m_signalSmoother;
        private PhasorMeasurement m_inputPhasor;
        private PhasorEstimate m_outputPhasor;
        private string m_phasorType;
        private double m_baseKv;
        private List<Ticks> m_timestampQueue;
        private bool m_allowTimestampReassignment;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// The measurement key for the magnitude input measurement.
        /// </summary>
        [ConnectionStringParameter,
         Description("The measurement key for the magnitude input measurement.")]
        public string MagnitudeInputKey
        {
            get
            {
                return m_magnitudeInputKey;
            }
            set
            {
                m_magnitudeInputKey = value;
            }
        }

        /// <summary>
        /// The measurement key for the angle input measurement.
        /// </summary>
        [ConnectionStringParameter,
         Description("The measurement key for the angle input measurement.")]
        public string AngleInputKey
        {
            get
            {
                return m_angleInputKey;
            }
            set
            {
                m_angleInputKey = value;
            }
        }

        /// <summary>
        /// The measurement key for the magnitude output measurement.
        /// </summary>
        [ConnectionStringParameter,
         Description("The measurement key for the magnitude output measurement.")]
        public string MagnitudeOutputKey
        {
            get
            {
                return m_magnitudeOutputKey;
            }
            set
            {
                m_magnitudeOutputKey = value;
            }
        }

        /// <summary>
        /// The measurement key for the angle output measurement.
        /// </summary>
        [ConnectionStringParameter,
         Description("The measurement key for the angle output measurement.")]
        public string AngleOutputKey
        {
            get
            {
                return m_angleOutputKey;
            }
            set
            {
                m_angleOutputKey = value;
            }
        }

        /// <summary>
        /// The Base KV for the phasor.
        /// </summary>
        [ConnectionStringParameter,
         Description("The Base KV for the phasor.")]
        public double BaseKv
        {
            get
            {
                return m_baseKv;
            }
            set
            {
                m_baseKv = value;
            }
        }

        /// <summary>
        /// The type of phasor (Voltage or Current).
        /// </summary>
        [ConnectionStringParameter,
         Description("The type of phasor (Voltage or Current).")]
        public string PhasorType
        {
            get
            {
                return m_phasorType;
            }
            set
            {
                m_phasorType = value;
            }
        }

        /// <summary>
        /// Allows Timestamp Reassignment to prevent lag. Should only be applied if all measurements are smoothed.
        /// </summary>
        [ConnectionStringParameter,
         Description("Allows Timestamp Reassignment to prevent lag. Should only be applied if all measurements are smoothed.")]
        public bool AllowTimestampReassignment
        {
            get
            {
                return m_allowTimestampReassignment;
            }
            set
            {
                m_allowTimestampReassignment = value;
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
        /// The <see cref="Initialize"/> method for the <see cref="PhasorSignalConditioner"/> adapter.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            GetAdapterSettings();

            InitializePhasors();

            InitializeSmoother();
        }

        /// <summary>
        /// The <see cref="PublishFrame"/> method for the <see cref="PhasorSignalConditioner"/> adapter.
        /// </summary>
        /// <param name="frame">The frame of incoming measurements</param>
        /// <param name="index">An integer</param>
        protected override void PublishFrame(IFrame frame, int index)
        {
            // Hang on to the timestamps so we can reattach them to their original outgoing frames
            if (m_timestampQueue.Count < MAX_BUFFER_SIZE)
            {
                m_timestampQueue.Add(frame.Timestamp);
            }
            else if (m_timestampQueue.Count == MAX_BUFFER_SIZE)
            {
                m_timestampQueue.RemoveAt(BUFFER_TOP_POSITION);
                m_timestampQueue.Add(frame.Timestamp);
            }

            // Map the input measurments to the phasor
            foreach (IMeasurement measurement in frame.Measurements.Values)
            {
                if (measurement.Key.ToString().Equals(m_inputPhasor.MagnitudeKey))
                {
                    m_inputPhasor.Magnitude = measurement.Value;
                }
                else if (measurement.Key.ToString().Equals(m_inputPhasor.AngleKey))
                {
                    m_inputPhasor.AngleInDegrees = measurement.Value;
                }
            }

            // Smooth the phasor measurement
            m_signalSmoother.Smooth(m_inputPhasor);

            // Prepare to clone the output measurements
            IMeasurement[] outputMeasurements = OutputMeasurements;

            // Create a list of IMeasurement objects for the output;
            List<IMeasurement> output = new List<IMeasurement>();

            // Clone the measurements for output
            foreach (IMeasurement measurement in outputMeasurements)
            {
                if (measurement.Key.ToString().Equals(m_magnitudeOutputKey))
                {
                    if (m_allowTimestampReassignment)
                    {
                        output.Add(Measurement.Clone(measurement, m_signalSmoother.Output.Magnitude, frame.Timestamp));
                    }
                    else
                    {
                        output.Add(Measurement.Clone(measurement, m_signalSmoother.Output.Magnitude, m_timestampQueue[BUFFER_TOP_POSITION]));
                    }
                }
                else if (measurement.Key.ToString().Equals(m_angleOutputKey))
                {
                    if (m_allowTimestampReassignment)
                    {
                        output.Add(Measurement.Clone(measurement, m_signalSmoother.Output.AngleInDegrees, frame.Timestamp));
                    }
                    else
                    {
                        output.Add(Measurement.Clone(measurement, m_signalSmoother.Output.AngleInDegrees, m_timestampQueue[BUFFER_TOP_POSITION]));
                    }
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

            if (settings.TryGetValue("MagnitudeInputKey", out setting))
            {
                m_magnitudeInputKey = setting;
            }
            if (settings.TryGetValue("MagnitudeOutputKey", out setting))
            {
                m_magnitudeOutputKey = setting;
            }
            if (settings.TryGetValue("AngleInputKey", out setting))
            {
                m_angleInputKey = setting;
            }
            if (settings.TryGetValue("AngleOutputKey", out setting))
            {
                m_angleOutputKey = setting;
            }
            if (settings.TryGetValue("BaseKv", out setting))
            {
                m_baseKv = Convert.ToDouble(setting);
            }
            if (settings.TryGetValue("PhasorType", out setting))
            {
                m_phasorType = setting;
            }
            if (settings.TryGetValue("AllowTimestampReassignment", out setting))
            {
                m_allowTimestampReassignment = Convert.ToBoolean(setting);
            }
        }

        private void InitializeSmoother()
        {
            m_signalSmoother = new Smoother(m_outputPhasor);
            m_timestampQueue = new List<Ticks>();
        }

        private void InitializePhasors()
        {
            if (m_phasorType.Equals("Voltage"))
            {
                m_inputPhasor = new PhasorMeasurement(m_magnitudeInputKey, m_angleInputKey, Measurements.PhasorType.VoltagePhasor, new VoltageLevel(1, m_baseKv));
                m_outputPhasor = new PhasorEstimate(m_magnitudeOutputKey, m_angleOutputKey, Measurements.PhasorType.VoltagePhasor, new VoltageLevel(1, m_baseKv));
            }
            else if (m_phasorType.Equals("Current"))
            {
                m_inputPhasor = new PhasorMeasurement(m_magnitudeInputKey, m_angleInputKey, Measurements.PhasorType.CurrentPhasor, new VoltageLevel(1, m_baseKv));
                m_outputPhasor = new PhasorEstimate(m_magnitudeOutputKey, m_angleOutputKey, Measurements.PhasorType.CurrentPhasor, new VoltageLevel(1, m_baseKv));
            }
        }

        #endregion
    }
}
