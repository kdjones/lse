//******************************************************************************************************
//  Phasor.cs
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
//  05/07/2014 - Kevin D. Jones
//       Added fields, properties for residual output keys.
//  07/09/2014 - Kevin D. Jones
//       Added Guid
//
//******************************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SynchrophasorAnalytics.Measurements
{
    /// <summary>
    /// A phasor measurement and estimate pair with specifications for calculated residual output.
    /// </summary>
    [Serializable()]
    public class Phasor
    {
        #region [ Private Members ]

        private Guid m_uniqueId;
        private PhasorMeasurement m_phasorMeasurement;
        private PhasorEstimate m_phasorEstimate;
        private string m_magnitudeResidualOutputKey;
        private string m_angleResidualOutputKey;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// A statistically unique identifier for the instance of the class.
        /// </summary>
        [XmlAttribute("Uid")]
        public Guid UniqueId
        {
            get
            {
                if (m_uniqueId == Guid.Empty)
                {
                    m_uniqueId = Guid.NewGuid();
                }
                return m_uniqueId;
            }
            set
            {
                m_uniqueId = value;
            }
        }

        /// <summary>
        /// A <see cref="LinearStateEstimator.Measurements.PhasorMeasurement"/> - a measured phasor value.
        /// </summary>
        [XmlElement("Measurement")]
        public PhasorMeasurement Measurement
        {
            get 
            { 
                return m_phasorMeasurement; 
            }
            set 
            { 
                m_phasorMeasurement = value; 
            }
        }

        /// <summary>
        /// A <see cref="LinearStateEstimator.Measurements.PhasorEstimate"/>. - an estimated phasor value.
        /// </summary>
        [XmlElement("Estimate")]
        public PhasorEstimate Estimate
        {
            get 
            { 
                return m_phasorEstimate; 
            }
            set 
            { 
                m_phasorEstimate = value; 
            }
        }

        /// <summary>
        /// The output measurement key for the magnitude of the residual value.
        /// </summary>
        [XmlAttribute("MagnitudeResidualKey")]
        public string MagnitudeResidualKey
        {
            get
            {
                return m_magnitudeResidualOutputKey;
            }
            set
            {
                m_magnitudeResidualOutputKey = value;
            }
        }

        /// <summary>
        /// The residual of the measured and estimated phasor magnitude in line-to-neutral kV.
        /// </summary>
        [XmlIgnore]
        public double MagnitudeResidual
        {
            get 
            { 
                return m_phasorMeasurement.Magnitude - m_phasorEstimate.Magnitude; 
            }
        }

        /// <summary>
        /// The residual of the measured and estimated phasor magnitude in per unit.
        /// </summary>
        [XmlIgnore]
        public double PerUnitMagnitudeResidual
        {
            get 
            { 
                return m_phasorMeasurement.PerUnitMagnitude - m_phasorEstimate.PerUnitMagnitude; 
            }
        }

        /// <summary>
        /// The output measurement key for the magnitude of the residual value.
        /// </summary>
        [XmlAttribute("AngleResidualKey")]
        public string AngleResidualKey
        {
            get
            {
                return m_angleResidualOutputKey;
            }
            set
            {
                m_angleResidualOutputKey = value;
            }
        }

        /// <summary>
        /// The residual of the measured and estimated phasor angle in degrees.
        /// </summary>
        [XmlIgnore]
        public double AngleResidualInDegrees
        {
            get 
            { 
                return m_phasorMeasurement.AngleInDegrees - m_phasorEstimate.AngleInDegrees; 
            }
        }

        /// <summary>
        /// The residual of the measured and estimated phasor anglge in radians.
        /// </summary>
        [XmlIgnore]
        public double AngleResidualInRadians
        {
            get 
            { 
                return m_phasorMeasurement.AngleInRadians - m_phasorEstimate.AngleInRadians; 
            }
        }

        /// <summary>
        /// The total vector error as defined by |V_measured - V_ideal|/|V_ideal|
        /// </summary>
        [XmlIgnore()]
        public double TotalVectorError
        {
            get
            {
                return Math.Abs((m_phasorMeasurement.PerUnitComplexPhasor - m_phasorEstimate.PerUnitComplexPhasor).Magnitude) / m_phasorEstimate.PerUnitMagnitude;
            }
        }

        /// <summary>
        /// The statistical cost of this <see cref="LinearStateEstimator.Measurements.Phasor"/>.
        /// </summary>
        [XmlIgnore]
        public double Cost
        {
            get
            {
                //if (m_phasorMeasurement.Type == PhasorMeasurementType.VoltagePhasor)
                //{
                //    double magnitudeCost = (PerUnitMagnitudeResidual * PerUnitMagnitudeResidual) / m_phasorMeasurement.MeasurementCovariance;
                //    double angleCost = (AngleResidualInDegrees * AngleResidualInDegrees) / 0.01;
                //    return magnitudeCost;// +angleCost;
                //}
                //else if (m_phasorMeasurement.Type == PhasorMeasurementType.CurrentPhasor)
                //{
                //    double magnitudeCost = ((PerUnitMagnitudeResidual / m_phasorMeasurement.PerUnitMagnitude) * (PerUnitMagnitudeResidual / m_phasorMeasurement.PerUnitMagnitude)) / m_phasorMeasurement.MeasurementCovariance;
                //    double angleCost = (AngleResidualInDegrees * AngleResidualInDegrees) / 0.01;
                //    return magnitudeCost;// +angleCost;
                //}
                //else
                //{
                //    return 99999;
                //}
                throw new NotImplementedException();
            }
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// A blank constructor with default values.
        /// </summary>
        public Phasor()
            :this(new PhasorMeasurement(), new PhasorEstimate())
        {
        }

        /// <summary>
        /// A constructor for the <see cref="Phasor"/> class which only specifies the <see cref="PhasorMeasurement"/>.
        /// </summary>
        /// <param name="measurement"></param>
        public Phasor(PhasorMeasurement measurement)
            :this(measurement, new PhasorEstimate())
        {
        }

        /// <summary>
        /// The designated constructor for the class.
        /// </summary>
        /// <param name="measurement">A measured phasor - <see cref="PhasorMeasurement"/>.</param>
        /// <param name="estimate">An estimated phasor - <see cref="PhasorEstimate"/>.</param>
        public Phasor(PhasorMeasurement measurement, PhasorEstimate estimate)
        {
            m_phasorMeasurement = measurement;
            m_phasorEstimate = estimate;
            m_magnitudeResidualOutputKey = "Undefined";
            m_angleResidualOutputKey = "Undefined";
        }

        #endregion

        #region [ Public Methods ]

        /// <summary>
        /// A descriptive string representation of the <see cref="Phasor"/> object
        /// </summary>
        /// <returns>A descriptive string representation of the <see cref="Phasor"/> object</returns>
        public override string ToString()
        {
            return m_phasorMeasurement.ToString() + "," + m_phasorEstimate.ToString() + "," + m_magnitudeResidualOutputKey + "," + m_angleResidualOutputKey;
        }

        /// <summary>
        /// A verbose descriptive string representation of the <see cref="Phasor"/> object.
        /// </summary>
        /// <returns>A verbose descriptive string representation of the <see cref="Phasor"/> object.</returns>
        public string ToVerboseString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine();
            stringBuilder.AppendFormat("----- Phasor -------------------------------------------------------------------");
            stringBuilder.AppendLine();
            stringBuilder.AppendFormat("     Measurement: " + m_phasorMeasurement.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("        Estimate: " + m_phasorEstimate.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("Mag Residual Key: " + m_magnitudeResidualOutputKey + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("Ang Residual Key: " + m_angleResidualOutputKey + "{0}", Environment.NewLine);
            stringBuilder.AppendLine();
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Performs a deep copy of the <see cref="Phasor"/> object.
        /// </summary>
        /// <returns>A deep copy of the target <see cref="Phasor"/> object.</returns>
        public Phasor Copy()
        {
            Phasor copy = new Phasor();
            copy.Measurement = m_phasorMeasurement.DeepCopy();
            copy.Estimate = m_phasorEstimate.Copy();
            copy.MagnitudeResidualKey = m_magnitudeResidualOutputKey;
            copy.AngleResidualKey = m_angleResidualOutputKey;
            return copy;
        }

        public void Keyify(string rootKey)
        {
            AngleResidualKey = $"{rootKey}.Ang.Res";
            MagnitudeResidualKey = $"{rootKey}.Mag.Res";
            Measurement.Keyify(rootKey);
            Estimate.Keyify(rootKey);
        }

        public void Unkeyify()
        {
            AngleResidualKey = "Undefined";
            MagnitudeResidualKey = "Undefined";
            Measurement.Unkeyify();
            Estimate.Unkeyify();
        }

        #endregion
    }
}
