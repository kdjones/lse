//******************************************************************************************************
//  DropoutInjector.cs
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
//  02/05/2014 - Kevin D. Jones
//      Generated original version of source code
//  04/06/2014 - Kevin D. Jones
//      Significant modifications to functionality. Reduced functionality to a single phasor per adapter.
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

namespace SynchrophasorAnalytics.Adapters
{
    /// <summary>
    /// Allows simulation of measurement dropouts on a synchrophasor stream by controlling whether measurements are
    /// forwarded through the adapter or not using a randomly generated uniform error distribution.
    /// </summary>
    [Description("Dropout Injector : Allows simulation of measurement dropouts on a synchrophasor stream.")]
    public class DropoutInjector : ActionAdapterBase
    {
        #region [ Private Fields ]

        private double m_dropoutRate = 0;
        private int m_numberOfPublishedFrames = 0;
        private int m_numberOfDroppedFrames = 0;
        private int m_totalNumberOfFrames = 0;
        private bool m_replaceDropoutsWithZeros;
        private Random m_randomNumberGenerator;
        private string m_magnitudeInputKey;
        private string m_magnitudeOutputKey;
        private string m_angleInputKey;
        private string m_angleOutputKey;

        #endregion

        #region [ Public Properties ]

        /// <summary>
        /// The dropout rate expressed as a percentage of uniformly distributed measurement loss.
        /// </summary>
        [ConnectionStringParameter,
         Description("The dropout rate expressed as a percentage of uniformly distributed measurement loss.")]
        public double DropoutRate
        {
            get
            {
                return m_dropoutRate;
            }
            set
            {
                m_dropoutRate = value;
            }
        }

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
        /// A flag which determines whether no measurements are published when a droput would occur or if the dropout will become a published measurement with a zero value.
        /// </summary>
        [ConnectionStringParameter,
         Description("A flag which determines whether no measurements are published when a droput would occur or if the dropout will become a published measurement with a zero value.")]
        public bool ReplaceDropoutsWithZeros
        {
            get
            {
                return m_replaceDropoutsWithZeros;
            }
            set
            {
                m_replaceDropoutsWithZeros = value;
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

        #region [ Private Properties ]
     
        /// <summary>
        /// The effective dropout rate expressed as a percentage over the total number of frames.
        /// </summary>
        private double EffectiveDropoutRate
        {
            get
            {
                return (Convert.ToDouble(m_numberOfDroppedFrames) / Convert.ToDouble(m_totalNumberOfFrames)) * 100;
            }
        }

        #endregion

        #region [ Public Methods ]

        public override void Initialize()
        {
            // Initialize the adapter in openPDC
            base.Initialize();

            // Initialize the random number generator
            m_randomNumberGenerator = new Random();

            GetAdapterSettings();
        }

        protected override void PublishFrame(IFrame frame, int index)
        {
            if (m_totalNumberOfFrames == Int16.MaxValue)
            {
                m_totalNumberOfFrames = 0;
                m_numberOfDroppedFrames = 0;
                m_numberOfPublishedFrames = 0;
            }

            m_totalNumberOfFrames++;

            double magnitude = 0;
            double angle = 0;

            foreach (IMeasurement measurement in frame.Measurements.Values)
            {
                if (measurement.Key.ToString().Equals(m_magnitudeInputKey))
                {
                    magnitude = measurement.Value;
                }
                else if (measurement.Key.ToString().Equals(m_angleInputKey))
                {
                    angle = measurement.Value;
                }
            }

            // Prepare to clone the output measurements
            IMeasurement[] outputMeasurements = OutputMeasurements;

            // Create a list of IMeasurement objects for the output;
            List<IMeasurement> output = new List<IMeasurement>();


            if ((m_randomNumberGenerator.NextDouble() * 100) > m_dropoutRate)
            {
                // Clone the measurements for output
                foreach (IMeasurement measurement in outputMeasurements)
                {
                    if (measurement.Key.ToString().Equals(m_magnitudeOutputKey))
                    {
                        output.Add(Measurement.Clone(measurement, magnitude, frame.Timestamp));
                    }
                    else if (measurement.Key.ToString().Equals(m_angleOutputKey))
                    {
                        output.Add(Measurement.Clone(measurement, angle, frame.Timestamp));
                    }
                }
                m_numberOfPublishedFrames++;
            }
            else
            {
                // Clone the measurements for output
                foreach (IMeasurement measurement in outputMeasurements)
                {
                    if (measurement.Key.ToString().Equals(m_magnitudeOutputKey))
                    {
                        output.Add(Measurement.Clone(measurement, 0, frame.Timestamp));
                    }
                    else if (measurement.Key.ToString().Equals(m_angleOutputKey))
                    {
                        output.Add(Measurement.Clone(measurement, 0, frame.Timestamp));
                    }
                }
                m_numberOfDroppedFrames++;
            }

            OnNewMeasurements(output);
        }

        /// <summary>
        /// Returns a short string status of the adapter to the console
        /// </summary>
        /// <param name="maxLength">The maximum length of the return string.</param>
        /// <returns>A short string status of the adapter to the console</returns>
        public override string GetShortStatus(int maxLength)
        {
            return string.Format("Effective dropout rate is {1}% over {2} frames.", m_dropoutRate, (Convert.ToDouble(m_numberOfDroppedFrames) / Convert.ToDouble(m_totalNumberOfFrames)) * 100, m_totalNumberOfFrames);
        }

        /// <summary>
        /// Returns the full status of the adapter to the console.
        /// </summary>
        public override string Status
        {
            get
            {
                StringBuilder status = new StringBuilder();
                status.Append(base.Status);
                status.AppendFormat("Dropout rate is currently set to {0}% and the effective dropout rate is {1}% over {2} frames.", m_dropoutRate, (Convert.ToDouble(m_numberOfDroppedFrames) / Convert.ToDouble(m_totalNumberOfFrames)) * 100, m_totalNumberOfFrames);
                status.AppendLine();

                return status.ToString();
            }
        }

        #endregion

        #region [ Private Methods ]

        /// <summary>
        /// Retrieves the adapter settings from the Settings dictionary in the base class.
        /// </summary>
        private void GetAdapterSettings()
        {
            Dictionary<string, string> settings = Settings;
            string setting;

            if (settings.TryGetValue("DropoutRate", out setting))
            {
                m_dropoutRate = Convert.ToDouble(setting);
            }
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
        }

        #endregion

        #region [ Adapter Commands ]

        /// <summary>
        /// Increases the dropout rate by 1 percent.
        /// </summary>
        [AdapterCommand("Increases the dropout rate by 1%")]
        public void Increase()
        {
            if (m_dropoutRate < 100)
            {
                m_dropoutRate++;
                OnStatusMessage("Dropout rate increased to {0}%.", m_dropoutRate);
            }
            else
            {
                m_dropoutRate = 100;
                OnStatusMessage("Dropout rate cannot increased above 100%");
            }
        }

        /// <summary>
        /// Decreases the dropout rate by 1 percent.
        /// </summary>
        [AdapterCommand("Decreases the dropout rate by 1%")]
        public void Decrease()
        {
            if (m_dropoutRate > 0)
            {
                m_dropoutRate--;
                OnStatusMessage("Dropout rate decreased to {0}%.", m_dropoutRate);
            }
            else
            {
                m_dropoutRate = 0;
                OnStatusMessage("Dropout rate cannot decreased below 0%");
            }
        }

        /// <summary>
        /// Prints a message with the current dropout rate.
        /// </summary>
        [AdapterCommand("Prints a message with the current dropout rate.")]
        public void GetRate()
        {
            OnStatusMessage("Dropout rate is currently set to {0}%.", m_dropoutRate);
            OnStatusMessage("The effective dropout rate is currently {0}% over {1} frames.", (Convert.ToDouble(m_numberOfDroppedFrames) / Convert.ToDouble(m_totalNumberOfFrames))*100, m_totalNumberOfFrames);
        }

        #endregion
    }
}
