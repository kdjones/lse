//******************************************************************************************************
//  PhasorBase.cs
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
//  05/16/2014 - Kevin D. Jones
//       Added System.Numerics for complex numbers
//
//******************************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Xml.Serialization;
using SynchrophasorAnalytics.Modeling;

namespace SynchrophasorAnalytics.Measurements
{
    /// <summary>
    /// The base class for phasor values
    /// </summary>
    [Serializable()]
    public partial class PhasorBase : IPhasor, IClearable
    {
        #region [ Private Members ]

        private double m_magnitude;
        private string m_magnitudeKey;
        private bool m_magnitudeValueWasReported;

        private double m_angleInDegrees;
        private string m_angleKey;
        private bool m_angleValueWasReported;

        private PhasorType m_type;

        private VoltageLevel m_baseKV;
        private int m_voltageLevelID;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// The <see cref="LinearStateEstimator.Modeling.VoltageLevel"/> of the phasor measurement.
        /// </summary>
        [XmlIgnore()]
        public VoltageLevel BaseKV
        {
            get 
            { 
                return m_baseKV; 
            }
            set 
            { 
                m_baseKV = value;
                m_voltageLevelID = value.InternalID;
            }
        }

        /// <summary>
        /// The <see cref="LinearStateEstimator.Modeling.VoltageLevel.InternalID"/> of the <see cref="LinearStateEstimator.Modeling.VoltageLevel"/>.
        /// </summary>
        [XmlAttribute("BaseKV")]
        public int VoltageLevelID
        {
            get 
            { 
                return m_voltageLevelID; 
            }
            set 
            { 
                m_voltageLevelID = value; 
            }
        }

        /// <summary>
        /// The magnitude of the phasor measurement in line-to-neutral volts.
        /// </summary>
        [XmlAttribute("Magnitude")]
        public double Magnitude
        {
            get 
            { 
                return m_magnitude;
            }
            set 
            {
                m_magnitudeValueWasReported = true;
                m_magnitude = value;
            }
        }

        /// <summary>
        /// The magnitude of the phasor measurement in line-to-line per unit.
        /// </summary>
        [XmlIgnore()]
        public double PerUnitMagnitude
        {
            get
            {
                double perUnitValue = 0;
                if (m_type == PhasorType.CurrentPhasor)
                {
                    perUnitValue = m_magnitude / ((100 * 1000000) / (m_baseKV.Value * 1000 * Math.Sqrt(3)));
                }
                else if (m_type == PhasorType.VoltagePhasor)
                {
                    perUnitValue = m_magnitude * Math.Sqrt(3) / (m_baseKV.Value * 1000);
                }
                return perUnitValue;
            }
            set
            {
                if (m_type == PhasorType.CurrentPhasor)
                {
                    m_magnitudeValueWasReported = true;
                    m_magnitude = value * ((100 * 1000000) / (m_baseKV.Value * 1000 * Math.Sqrt(3)));
                }
                else if (m_type == PhasorType.VoltagePhasor)
                {
                    m_magnitudeValueWasReported = true;
                    m_magnitude = value * (m_baseKV.Value * 1000) / Math.Sqrt(3);
                }
            }
        }

        /// <summary>
        ///  The angle of the phasor measurement in degrees.
        /// </summary>
        [XmlAttribute("Angle")]
        public double AngleInDegrees
        {
            get 
            { 
                return m_angleInDegrees; 
            }
            set 
            {
                m_angleValueWasReported = true;
                m_angleInDegrees = value; 
            }
        }

        /// <summary>
        /// The angle of the phasor measurement in radians.
        /// </summary>
        [XmlIgnore()]
        public double AngleInRadians
        {
            get 
            { 
                return (m_angleInDegrees * Math.PI / 180); 
            }
            set 
            {
                m_angleValueWasReported = true;
                m_angleInDegrees = (value * 180 / Math.PI); 
            }
        }

        /// <summary>
        /// The complex value of the phasor measurement in line-to-neutral volts.
        /// </summary>
        [XmlIgnore()]
        public Complex ComplexPhasor
        {
            get
            {
                double realpart = m_magnitude * Math.Cos(m_angleInDegrees * Math.PI / 180);
                double imagPart = m_magnitude * Math.Sin(m_angleInDegrees * Math.PI / 180);
                return (new Complex(realpart, imagPart));
            }
            set
            {
                m_magnitudeValueWasReported = true;
                m_magnitude = Math.Sqrt(value.Real * value.Real + value.Imaginary * value.Imaginary);

                m_angleValueWasReported = true;
                m_angleInDegrees = Math.Atan2(value.Imaginary, value.Real) * 180 / Math.PI;
            }
        }

        /// <summary>
        /// The complex value of the phasor measurement in line-to-line per unit.
        /// </summary>
        [XmlIgnore()]
        public Complex PerUnitComplexPhasor
        {
            get
            {
                Complex perUnitComplexPhasor = new Complex(0, 0);
                if (m_type == PhasorType.CurrentPhasor)
                {
                    perUnitComplexPhasor = ComplexPhasor / ((100 * 1000000) / (m_baseKV.Value * 1000 * Math.Sqrt(3)));
                }
                else if (m_type == PhasorType.VoltagePhasor)
                {
                    perUnitComplexPhasor = ComplexPhasor * Math.Sqrt(3) / (m_baseKV.Value * 1000);
                }
                return perUnitComplexPhasor;
            }
            set
            {
                if (m_type == PhasorType.CurrentPhasor)
                {
                    ComplexPhasor = value * ((100 * 1000000) / (m_baseKV.Value * 1000 * Math.Sqrt(3)));
                }
                else if (m_type == PhasorType.VoltagePhasor)
                {
                    ComplexPhasor = value * (m_baseKV.Value * 1000) / Math.Sqrt(3);
                }
            }
        }

        /// <summary>
        /// The input measurement key for the <see cref="LinearStateEstimator.Measurements.PhasorBase.Magnitude"/> of the <see cref="LinearStateEstimator.Measurements.PhasorBase"/>.
        /// </summary>
        [XmlAttribute("MagnitudeKey")]
        public string MagnitudeKey
        {
            get 
            { 
                return m_magnitudeKey; 
            }
            set 
            { 
                m_magnitudeKey = value; 
            }
        }

        /// <summary>
        /// The input measurement key for the <see cref="LinearStateEstimator.Measurements.PhasorBase.AngleInDegrees"/> of the <see cref="LinearStateEstimator.Measurements.PhasorBase"/>.
        /// </summary>
        [XmlAttribute("AngleKey")]
        public string AngleKey
        {
            get 
            { 
                return m_angleKey; 
            }
            set 
            { 
                m_angleKey = value; 
            }
        }

        /// <summary>
        /// Specifies whether the phasor measurement is a current phasor or a voltage phasor  or complex power with the <see cref="LinearStateEstimator.Measurements.PhasorType"/> enumeration, either <see cref="LinearStateEstimator.Measurements.PhasorType.VoltagePhasor"/>, <see cref="LinearStateEstimator.Measurements.PhasorType.CurrentPhasor"/>, or <see cref="LinearStateEstimator.Measurements.PhasorType.ComplexPower"/>.
        /// </summary>
        [XmlAttribute("Type")]
        public PhasorType Type
        {
            get
            { 
                return m_type; 
            }
            set
            { 
                m_type = value; 
            }
        }

        /// <summary>
        /// A flag which represents whether or not the measurement was received for the current timestamp.
        /// </summary>
        [XmlIgnore()]
        public bool MeasurementWasReported
        {
            get
            {
                if (m_magnitudeValueWasReported && m_angleValueWasReported)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// A flag which represents whether or not the magnitude measurement was received for the current timestamp.
        /// </summary>
        [XmlIgnore()]
        public bool MagnitudeValueWasReported
        {
            get
            {
                return m_magnitudeValueWasReported;
            }
        }

        /// <summary>
        /// A flag which represents whether or not the angle measurement was received for the current timestamp.
        /// </summary>
        [XmlIgnore()]
        public bool AngleValueWasReported
        {
            get
            {
                return m_angleValueWasReported;
            }
        }

        /// <summary>
        /// A pretty string representation of the phasor's value. Formatted as "XXX.X kv @ XX.X degrees".
        /// </summary>
        [XmlIgnore()]
        public string PrettyStringValue
        {
            get
            {
                return $"{((Magnitude)/1000).ToString("0.0")} kV @ {AngleInDegrees.ToString("0.0")} degrees";
            }
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// A blank constructor with default values.
        /// </summary>
        public PhasorBase()
            :this(PhasorType.VoltagePhasor, new VoltageLevel())
        {
        }

        /// <summary>
        /// A constructor with default values except for the <see cref="PhasorType"/> and the <see cref="VoltageLevel"/> of the phasor.
        /// </summary>
        /// <param name="type">The <see cref="PhasorType"/> of the phasor.</param>
        /// <param name="baseKV">The <see cref="VoltageLevel"/> of the phasor.</param>
        public PhasorBase(PhasorType type, VoltageLevel baseKV)
            :this("Undefined", "Undefined", type, baseKV)
        {
        }

        /// <summary>
        /// Designated constructor method for the <see cref="PhasorBase"/> object.
        /// </summary>
        /// <param name="magnitudeKey">The measurement key which corresponds to the magnitude of the phasor.</param>
        /// <param name="angleKey">The measurement key which corresponds to the  angle of the phasor.</param>
        /// <param name="type">The <see cref="PhasorType"/> of the phasor.</param>
        /// <param name="baseKV">The <see cref="VoltageLevel"/> of the phasor.</param>
        public PhasorBase(string magnitudeKey, string angleKey, PhasorType type, VoltageLevel baseKV)
        {
            m_magnitudeValueWasReported = false;
            m_angleValueWasReported = false;
            m_magnitude = 0;
            m_angleInDegrees = 0;
            m_magnitudeKey = magnitudeKey;
            m_angleKey = angleKey;
            m_type = type;
            m_baseKV = baseKV;
            m_shouldSerializeData = false;
        }

        #endregion

        #region [ Public Methods ]

        /// <summary>
        /// A descriptive string representation of the <see cref="LinearStateEstimator.Measurements.PhasorBase"/> class.
        /// </summary>
        /// <returns>A descriptive string representation of the <see cref="LinearStateEstimator.Measurements.PhasorBase"/> class.</returns>
        public override string ToString()
        {
            return m_magnitudeKey + "," + m_angleKey;
        }

        /// <summary>
        /// A verbose descriptive string representation of the <see cref="LinearStateEstimator.Measurements.PhasorBase"/> class.
        /// </summary>
        /// <returns>A verbose descriptive string representation of the <see cref="LinearStateEstimator.Measurements.PhasorBase"/> class.</returns>
        public string ToVerboseString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine();
            stringBuilder.AppendFormat("----- Phasor Base --------------------------------------------------------------");
            stringBuilder.AppendLine();
            stringBuilder.AppendFormat("            Type: " + m_type.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("         Base KV: " + m_baseKV.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("     MagnitudKey: " + m_magnitudeKey + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("        AngleKey: " + m_angleKey + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("       Magnitude: " + m_magnitude.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("           Angle: " + m_angleInDegrees.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("MagnitudeReportd: " + m_magnitudeValueWasReported.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("AngleWasReported: " + m_angleValueWasReported.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendLine();
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Clears the values of the magnitude and angle by setting them to 0 and resets the <see cref="LinearStateEstimator.Measurements.PhasorBase.MeasurementWasReported"/> flag to false.
        /// </summary>
        public void ClearValues()
        {
            m_magnitude = 0;
            m_magnitudeValueWasReported = false;

            m_angleInDegrees = 0;
            m_angleValueWasReported = false;
        }

        /// <summary>
        /// Determines whether the magnitude or angle respond to a certain measurement key.
        /// </summary>
        /// <param name="key">The target measurent key.</param>
        /// <returns>A bool representing whether the <see cref="LinearStateEstimator.Measurements.PhasorBase"/> will respond to the target measurement key.</returns>
        public bool ContainsKey(string key)
        {
            if (key.Equals(m_magnitudeKey) || key.Equals(m_angleKey))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Sets the value of the magnitude or angle base on the measurement key.
        /// </summary>
        /// <param name="key">The measurement key.</param>
        /// <param name="value">The magnitude or angle value to be set.</param>
        /// <returns>A bool representing the success of the insertion.</returns>
        public bool InsertValueForKey(string key, double value)
        {
            if (this.ContainsKey(key))
            {
                if (key.Equals(m_angleKey))
                {
                    m_angleValueWasReported = true;
                    m_angleInDegrees = value;
                    return true;
                }
                else if (key.Equals(m_magnitudeKey))
                {
                    m_magnitudeValueWasReported = true;
                    m_magnitude = value;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Performs a deep copy of the target <see cref="LinearStateEstimator.Measurements.PhasorBase"/> object
        /// </summary>
        /// <returns>A deep copy of the <see cref="LinearStateEstimator.Measurements.PhasorBase"/> object.</returns>
        public PhasorBase DeepCopy()
        {
            PhasorBase copy = (PhasorBase)this.MemberwiseClone();
            copy.BaseKV = m_baseKV.DeepCopy();
            copy.Type = m_type;
            return copy;
        }

        /// <summary>
        /// Determines equality between to <see cref="LinearStateEstimator.Measurements.PhasorBase"/> objects
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

            // If parameter cannot be cast to PhasorBase return false.
            PhasorBase phasorBase = target as PhasorBase;

            if ((object)phasorBase == null)
            {
                return false;
            }

            // Return true if the fields match:
            if (this.m_type != phasorBase.Type)
            {
                return false;
            }
            else if (!this.BaseKV.Equals(phasorBase.BaseKV))
            {
                return false;
            }
            else if (this.m_magnitude != phasorBase.Magnitude)
            {
                return false;
            }
            else if (this.m_angleInDegrees != phasorBase.AngleInDegrees)
            {
                return false;
            }
            else if (this.m_magnitudeKey != phasorBase.MagnitudeKey)
            {
                return false;
            }
            else if (this.m_angleKey != phasorBase.AngleKey)
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
        /// <returns>Integer hash code</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion

    }
}
