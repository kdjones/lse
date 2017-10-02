//******************************************************************************************************
//  NoiseInjector.cs
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
//      Generated original version of source code
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
    [Description("Noise Injector : Injects uniformly distributed noise into the phasor components at specified levels.")]
    public class NoiseInjector : ActionAdapterBase
    {
        #region [ Private Fields ]

        private double m_magnitudeNoiseMultiplier;
        private double m_magnitudeNoiseOffset;
        private double m_angleNoiseMultiplier;
        private double m_angleNoiseOffset;
        private string m_magnitudeInputKey;
        private string m_magnitudeOutputKey;
        private string m_angleInputKey;
        private string m_angleOutputKey;
        private Random m_randomNumberGenerator;

        #endregion

        #region [ Public Properties ]

        /// <summary>
        /// The random number (between 0 and 1) is multiplied by this number to scale the noise in the phasor magnitude.
        /// </summary>
        [ConnectionStringParameter,
         Description("The random number (between 0 and 1) is multiplied by this number to scale the noise in the phasor magnitude.")]
        public double MagnitudeNoiseMultiplier
        {
            get
            {
                return m_magnitudeNoiseMultiplier;
            }
            set
            {
                m_magnitudeNoiseMultiplier = value;
            }
        }

        /// <summary>
        /// The adder for the product of the magnitude noise multiplier and the random number between 0 and 1.
        /// </summary>
        [ConnectionStringParameter,
         Description("The adder for the product of the magnitude noise multiplier and the random number between 0 and 1.")]
        public double MagnitudeNoiseOffset
        {
            get
            {
                return m_magnitudeNoiseOffset;
            }
            set
            {
                m_magnitudeNoiseOffset = value;
            }
        }

        /// <summary>
        /// The random number (between 0 and 1) is multiplied by this number to scale the noise in the phasor angle.
        /// </summary>
        [ConnectionStringParameter,
         Description("The random number (between 0 and 1) is multiplied by this number to scale the noise in the phasor angle.")]
        public double AngleNoiseMultiplier
        {
            get
            {
                return m_angleNoiseMultiplier;
            }
            set
            {
                m_angleNoiseMultiplier = value;
            }
        }

        /// <summary>
        /// The adder for the product of the angle noise multiplier and the random number between 0 and 1.
        /// </summary>
        [ConnectionStringParameter,
         Description("The adder for the product of the angle noise multiplier and the random number between 0 and 1.")]
        public double AngleNoiseOffset
        {
            get
            {
                return m_angleNoiseOffset;
            }
            set
            {
                m_angleNoiseOffset = value;
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

        private double MagnitudeNoise
        {
            get
            {
                return m_randomNumberGenerator.NextDouble() * m_magnitudeNoiseMultiplier + m_magnitudeNoiseOffset;
            }
        }

        private double AngleNoise
        {
            get
            {
                return m_randomNumberGenerator.NextDouble() * m_angleNoiseMultiplier + m_angleNoiseOffset;
            }
        }

        #endregion

        #region [ Public Methods ]

        public override void Initialize()
        {
            base.Initialize();

            GetAdapterSettings();

            m_randomNumberGenerator = new Random();
        }

        protected override void PublishFrame(IFrame frame, int index)
        {
            double magnitude = 0;
            double angle = 0;

            foreach (IMeasurement measurement in frame.Measurements.Values)
            {
                if (measurement.Key.ToString().Equals(m_magnitudeInputKey))
                {
                    magnitude = measurement.Value + MagnitudeNoise;
                }
                else if (measurement.Key.ToString().Equals(m_angleInputKey))
                {
                    angle = measurement.Value + AngleNoise;
                }
            }

            // Prepare to clone the output measurements
            IMeasurement[] outputMeasurements = OutputMeasurements;

            // Create a list of IMeasurement objects for the output;
            List<IMeasurement> output = new List<IMeasurement>();

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
            if (settings.TryGetValue("MagnitudeNoiseMultiplier", out setting))
            {
                m_magnitudeNoiseMultiplier = Convert.ToDouble(setting);
            }
            if (settings.TryGetValue("MagnitudeNoiseOffset", out setting))
            {
                m_magnitudeNoiseOffset = Convert.ToDouble(setting);
            }
            if (settings.TryGetValue("AngleInputKey", out setting))
            {
                m_angleInputKey = setting;
            }
            if (settings.TryGetValue("AngleOutputKey", out setting))
            {
                m_angleOutputKey = setting;
            }
            if (settings.TryGetValue("AngleNoiseMultiplier", out setting))
            {
                m_angleNoiseMultiplier = Convert.ToDouble(setting);
            }
            if (settings.TryGetValue("AngleNoiseOffset", out setting))
            {
                m_angleNoiseOffset = Convert.ToDouble(setting);
            }
        }

        #endregion
    }
}
