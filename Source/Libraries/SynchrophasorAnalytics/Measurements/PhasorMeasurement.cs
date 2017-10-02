//******************************************************************************************************
//  PhasorMeasurement.cs
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
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using SynchrophasorAnalytics.Calibration;
using SynchrophasorAnalytics.Modeling;

namespace SynchrophasorAnalytics.Measurements
{
    /// <summary>
    /// Represents a phasor value as a measurement.
    /// </summary>
    [Serializable()]
    public class PhasorMeasurement : PhasorBase, ICalibratable
    {
        #region [ Private Constants ]

        private const string DEFAULT_MEASUREMENT_KEY = "Undefined";

        #endregion

        #region [ Private Members ]

        private double m_measurementVariance;
        private bool m_measurementShouldBeCalibrated;
        private CalibrationSetting m_calibrationSetting;
        private double m_rcf;
        private double m_pacf;

        #endregion

        #region [ Properties ]

        [XmlIgnore()]
        public bool ExpectsInputMeasurement
        {
            get
            {
                if ((MagnitudeKey != "" && MagnitudeKey != "Undefined") || (AngleKey != "" && AngleKey != "Undefined"))
                {
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// A flag which represents whether or not to include the <see cref="LinearStateEstimator.Measurements.PhasorMeasurement"/> in the state estimator.
        /// </summary>
        [XmlIgnore()]
        public bool IncludeInEstimator
        {
            get 
            {
                if (MeasurementWasReported == false)
                {
                    return false;
                }
                else if (Type == PhasorType.VoltagePhasor && Magnitude < (0.2 * (BaseKV.Value * 1000)))
                {
                    return false;
                }
                else if (Type == PhasorType.CurrentPhasor && Magnitude < 10)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        /// <summary>
        /// The measurement variance for the <see cref="LinearStateEstimator.Measurements.PhasorMeasurement"/>.
        /// </summary>
        [XmlAttribute("Variance")]
        public double MeasurementVariance
        {
            get 
            { 
                return m_measurementVariance; 
            }
            set 
            { 
                m_measurementVariance = value; 
            }
        }

        /// <summary>
        /// The measurement covariance
        /// </summary>
        [XmlIgnore]
        public double MeasurementCovariance
        {
            get 
            { 
                return m_measurementVariance * m_measurementVariance; 
            }
        }

        /// <summary>
        /// The ratio correction factor (RCF) for the <see cref="LinearStateEstimator.Measurements.PhasorMeasurement"/>.
        /// </summary>
        [XmlAttribute("RCF")]
        public double RCF
        {
            get 
            { 
                return m_rcf; 
            }
            set 
            { 
                m_rcf = value; 
            }
        }

        /// <summary>
        /// The phase angle correction factor (PACF) for the <see cref="LinearStateEstimator.Measurements.PhasorMeasurement"/>.
        /// </summary>
        [XmlAttribute("PACF")]
        public double PACF
        {
            get 
            { 
                return m_pacf; 
            }
            set 
            { 
                m_pacf = value; 
            }
        }

        /// <summary>
        /// A flag representing whether or not to calibrate the measurement using the RCF and PACF during insertion.
        /// </summary>
        [XmlAttribute("Calibrated")]
        public bool MeasurementShouldBeCalibrated
        {
            get 
            { 
                return m_measurementShouldBeCalibrated; 
            }
            set 
            { 
                m_measurementShouldBeCalibrated = value; 
            }
        }

        /// <summary>
        /// The <see cref="LinearStateEstimator.Calibration.CalibrationSetting"/> type for the <see cref="LinearStateEstimator.Measurements.PhasorMeasurement"/>. Determines how the measurement is included in the calibration algorithm.
        /// </summary>
        [XmlAttribute("CalibrationType")]
        public CalibrationSetting InstrumentTransformerCalibrationSetting
        {
            get 
            { 
                return m_calibrationSetting; 
            }
            set 
            { 
                m_calibrationSetting = value; 
            }
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// A blank constructor with default values.
        /// </summary>
        public PhasorMeasurement()
            :this(DEFAULT_MEASUREMENT_KEY, DEFAULT_MEASUREMENT_KEY, PhasorType.VoltagePhasor, new VoltageLevel())
        {
        }

        /// <summary>
        /// A constructor for the <see cref="LinearStateEstimator.Measurements.PhasorMeasurement"/> which only specifies the measurement keys, phasor type, and base KV.
        /// </summary>
        /// <param name="magnitudeKey">The openPDC input measurement key for the <see cref="LinearStateEstimator.Measurements.PhasorBase.Magnitude"/> of the <see cref="LinearStateEstimator.Measurements.PhasorMeasurement"/>.</param>
        /// <param name="angleKey">The openPDC input measurement key for the <see cref="LinearStateEstimator.Measurements.PhasorBase.AngleInDegrees"/> of the <see cref="LinearStateEstimator.Measurements.PhasorMeasurement"/>.</param>
        /// <param name="type">Specifies whether the phasor measurement is a current phasor or a
        /// voltage phasor  or complex power with the <see cref="LinearStateEstimator.Measurements.PhasorType"/> enumeration, either <see cref="LinearStateEstimator.Measurements.PhasorType.VoltagePhasor"/>, 
        /// <see cref="PhasorType.CurrentPhasor"/>, or <see cref="LinearStateEstimator.Measurements.PhasorType.ComplexPower"/>.</param>
        /// <param name="baseKV">The <see cref="LinearStateEstimator.Modeling.VoltageLevel"/> of the phasor measurement.</param>
        public PhasorMeasurement(string magnitudeKey, string angleKey, PhasorType type, VoltageLevel baseKV)
            :this(magnitudeKey, angleKey, type, baseKV, 0.002)
        {
        }

        /// <summary>
        /// A constructor for the <see cref="LinearStateEstimator.Measurements.PhasorMeasurement"/> which only specifies the measurement keys, phasor type, base KV and measurement variance.
        /// </summary>
        /// <param name="magnitudeKey">The openPDC input measurement key for the <see cref="LinearStateEstimator.Measurements.PhasorBase.Magnitude"/> of the <see cref="LinearStateEstimator.Measurements.PhasorMeasurement"/>.</param>
        /// <param name="angleKey">The openPDC input measurement key for the <see cref="LinearStateEstimator.Measurements.PhasorBase.AngleInDegrees"/> of the <see cref="LinearStateEstimator.Measurements.PhasorMeasurement"/>.</param>
        /// <param name="type">Specifies whether the phasor measurement is a current phasor or a voltage phasor  or complex power with the <see cref="LinearStateEstimator.Measurements.PhasorType"/> enumeration, either <see cref="LinearStateEstimator.Measurements.PhasorType.VoltagePhasor"/>, <see cref="LinearStateEstimator.Measurements.PhasorType.CurrentPhasor"/>, or <see cref="LinearStateEstimator.Measurements.PhasorType.ComplexPower"/>.</param>
        /// <param name="baseKV">The <see cref="LinearStateEstimator.Modeling.VoltageLevel"/> of the phasor measurement.</param>
        /// <param name="variance">The measurement variance in per unit.</param>
        public PhasorMeasurement(string magnitudeKey, string angleKey, PhasorType type, VoltageLevel baseKV, double variance)
            :this(magnitudeKey, angleKey, type, baseKV, variance, 1, 0)
        {
        }

        /// <summary>
        /// A constructor for the <see cref="LinearStateEstimator.Measurements.PhasorMeasurement"/> which specifies the measurement keys, phasor type, base KV, measurement variance as well as RCF and PACF.
        /// </summary>
        /// <param name="magnitudeKey">The openPDC input measurement key for the <see cref="LinearStateEstimator.Measurements.PhasorBase.Magnitude"/> of the <see cref="LinearStateEstimator.Measurements.PhasorMeasurement"/>.</param>
        /// <param name="angleKey">The openPDC input measurement key for the <see cref="LinearStateEstimator.Measurements.PhasorBase.AngleInDegrees"/> of the <see cref="LinearStateEstimator.Measurements.PhasorMeasurement"/>.</param>
        /// <param name="type">Specifies whether the phasor measurement is a current phasor or a
        /// voltage phasor  or complex power with the <see cref="LinearStateEstimator.Measurements.PhasorType"/> enumeration, either <see cref="LinearStateEstimator.Measurements.PhasorType.VoltagePhasor"/>, 
        /// <see cref="LinearStateEstimator.Measurements.PhasorType.CurrentPhasor"/>, or <see cref="LinearStateEstimator.Measurements.PhasorType.ComplexPower"/>.</param>
        /// <param name="baseKV">The <see cref="LinearStateEstimator.Modeling.VoltageLevel"/> of the phasor measurement.</param>
        /// <param name="variance">The measurement variance in per unit.</param>
        /// <param name="rcf">The <b>Ratio Correction Factor (RCF)</b> for the measurement.</param>
        /// <param name="pacf">The <b>Phase Angle Correction Factor (PACF)</b> for the measurementin degrees</param>
        public PhasorMeasurement(string magnitudeKey, string angleKey, PhasorType type, VoltageLevel baseKV, double variance, double rcf, double pacf)
            :this(magnitudeKey, angleKey, type, baseKV, variance, rcf, pacf, CalibrationSetting.Inactive)
        {
        }

        /// <summary>
        /// The designated constructor for the <see cref="LinearStateEstimator.Measurements.PhasorMeasurement"/> class.
        /// </summary>
        /// <param name="magnitudeKey">The openPDC input measurement key for the <see cref="LinearStateEstimator.Measurements.PhasorBase.Magnitude"/> of the <see cref="LinearStateEstimator.Measurements.PhasorMeasurement"/>.</param>
        /// <param name="angleKey">The openPDC input measurement key for the <see cref="LinearStateEstimator.Measurements.PhasorBase.AngleInDegrees"/> of the <see cref="LinearStateEstimator.Measurements.PhasorMeasurement"/>.</param>
        /// <param name="type">Specifies whether the phasor measurement is a current phasor or a
        /// voltage phasor  or complex power with the <see cref="LinearStateEstimator.Measurements.PhasorType"/> enumeration, either <see cref="LinearStateEstimator.Measurements.PhasorType.VoltagePhasor"/>, 
        /// <see cref="LinearStateEstimator.Measurements.PhasorType.CurrentPhasor"/>, or <see cref="LinearStateEstimator.Measurements.PhasorType.ComplexPower"/>.</param>
        /// <param name="baseKV">The <see cref="LinearStateEstimator.Modeling.VoltageLevel"/> of the phasor measurement.</param>
        /// <param name="variance">The measurement variance in per unit.</param>
        /// <param name="rcf">The <b>Ratio Correction Factor (RCF)</b> for the measurement.</param>
        /// <param name="pacf">The <b>Phase Angle Correction Factor (PACF)</b> for the measurement in degrees.</param>
        /// <param name="calibrationSetting">The <see cref="LinearStateEstimator.Calibration.CalibrationSetting"/> type for the <see cref="LinearStateEstimator.Measurements.PhasorMeasurement"/>. Determines how the measurement is included in the calibration algorithm.</param>
        public PhasorMeasurement(string magnitudeKey, string angleKey, PhasorType type, VoltageLevel baseKV, double variance, double rcf, double pacf, CalibrationSetting calibrationSetting)
            :base(magnitudeKey, angleKey, type, baseKV)
        {
            m_measurementVariance = variance;
            m_rcf = rcf;
            m_pacf = pacf;
            m_calibrationSetting = calibrationSetting;
        }

        #endregion

        #region [ Public Methods ]

        /// <summary>
        /// A string representation of the instance of the class.
        /// </summary>
        /// <returns>A string representation of the instance of the class.</returns>
        public override string ToString()
        {
            return base.ToString() + "," + m_measurementVariance.ToString() + "," + m_rcf.ToString() + "," + m_pacf.ToString() + "," + m_measurementShouldBeCalibrated.ToString() + "," + m_calibrationSetting.ToString();
        }

        /// <summary>
        /// A verbose descriptive string representation of the <see cref="PhasorMeasurement"/> class.
        /// </summary>
        /// <returns>A verbose descriptive string representation of the <see cref="PhasorMeasurement"/> class.</returns>
        public new string ToVerboseString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine();
            stringBuilder.AppendFormat("----- Phasor Measurement -------------------------------------------------------");
            stringBuilder.AppendLine();
            stringBuilder.AppendFormat("            Type: " + Type.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("         Base KV: " + BaseKV.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("     MagnitudKey: " + MagnitudeKey + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("        AngleKey: " + AngleKey + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("       Magnitude: " + Magnitude.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("           Angle: " + AngleInDegrees.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("        Reported: " + MeasurementWasReported.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("        Variance: " + m_measurementVariance.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("             RCF: " + m_rcf.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("            PACF: " + m_pacf.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat(" ShouldCalibrate: " + m_measurementShouldBeCalibrated.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("CalibrationSttng: " + m_calibrationSetting.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendLine();
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Performs a deep copy of the <see cref="LinearStateEstimator.Measurements.PhasorMeasurement"/>.
        /// </summary>
        /// <returns>A deep copy of the target <see cref="LinearStateEstimator.Measurements.PhasorMeasurement"/>.</returns>
        public new PhasorMeasurement DeepCopy()
        {
            PhasorMeasurement copy = (PhasorMeasurement)this.MemberwiseClone();
            copy.BaseKV = BaseKV.DeepCopy();
            copy.Type = Type;
            return copy;
        }

        /// <summary>
        /// Determines equality between to <see cref="LinearStateEstimator.Measurements.PhasorMeasurement"/> objects
        /// </summary>
        /// <param name="target">The target object for equality testing.</param>
        /// <returns>A bool representing the result of the equality check.</returns>
        public override bool Equals(object target)
        {
            // If parameter is null return false.
            if (target == null)
            {
                return false;
            }

            // If parameter cannot be cast to PhasorMeasurement return false.
            PhasorMeasurement phasorMeasurement = target as PhasorMeasurement;

            if ((object)phasorMeasurement == null)
            {
                return false;
            }

            // Return true if the fields match:
            if (!base.Equals(target))
            {
                return false;
            }
            else if (!this.BaseKV.Equals(phasorMeasurement.BaseKV))
            {
                return false;
            }
            else if (this.m_measurementVariance != phasorMeasurement.MeasurementVariance)
            {
                return false;
            }
            else if (this.m_measurementShouldBeCalibrated != phasorMeasurement.MeasurementShouldBeCalibrated)
            {
                return false;
            }
            else if (this.m_rcf != phasorMeasurement.RCF)
            {
                return false;
            }
            else if (this.m_pacf != phasorMeasurement.PACF)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Overridden to prevent compilation warnings.
        /// </summary>
        /// <returns>An integer hash code determined by the base class.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Sets the flag for the use of the RCF and PACF.
        /// </summary>
        public void CalibratePhasors()
        {
            MeasurementShouldBeCalibrated = true;
        }

        public void Keyify(string rootKey)
        {
            AngleKey = $"{rootKey}.Ang.Meas";
            MagnitudeKey = $"{rootKey}.Mag.Meas";
        }
        
        public void Unkeyify()
        {
            AngleKey = DEFAULT_MEASUREMENT_KEY;
            MagnitudeKey = DEFAULT_MEASUREMENT_KEY;
        }
        #endregion
    }
}
