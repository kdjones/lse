//******************************************************************************************************
//  PhasorEstimate.cs
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
using SynchrophasorAnalytics.Modeling;

namespace SynchrophasorAnalytics.Measurements
{
    /// <summary>
    /// Represents the estimate of a phasor value
    /// </summary>
    [Serializable()]
    public class PhasorEstimate : PhasorBase
    {
        #region [ Private Constants ]

        private const string DEFAULT_MEASUREMENT_KEY = "Undefined";

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// A blank constructor with default values
        /// </summary>
        public PhasorEstimate()
            :this(DEFAULT_MEASUREMENT_KEY, DEFAULT_MEASUREMENT_KEY, PhasorType.VoltagePhasor, new VoltageLevel())
        {
        }

        /// <summary>
        /// The designated constructor for the <see cref="LinearStateEstimator.Measurements.PhasorEstimate"/> class.
        /// </summary>
        /// <param name="magnitudeKey">The openPDC input measurement key for the <see cref="LinearStateEstimator.Measurements.PhasorBase.Magnitude"/> of the <see cref="LinearStateEstimator.Measurements.PhasorEstimate"/>.</param>
        /// <param name="angleKey">The openPDC input measurement key for the <see cref="LinearStateEstimator.Measurements.PhasorBase.Magnitude"/> of the <see cref="LinearStateEstimator.Measurements.PhasorEstimate"/>.</param>
        /// <param name="type">Specifies whether the phasor measurement is a current phasor or a voltage phasor  or complex power with the <see cref="LinearStateEstimator.Measurements.PhasorType"/> enumeration, either <see cref="LinearStateEstimator.Measurements.PhasorType.VoltagePhasor"/>, <see cref="LinearStateEstimator.Measurements.PhasorType.CurrentPhasor"/>, or <see cref="LinearStateEstimator.Measurements.PhasorType.ComplexPower"/>.</param>
        /// <param name="baseKV">The <see cref="LinearStateEstimator.Modeling.VoltageLevel"/> of the phasor measurement.</param>
        public PhasorEstimate(string magnitudeKey, string angleKey, PhasorType type, VoltageLevel baseKV)
            :base(magnitudeKey, angleKey, type, baseKV)
        {
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Performs a deep copy of the <see cref="LinearStateEstimator.Measurements.PhasorEstimate"/> object.
        /// </summary>
        /// <returns>A deep copy of the target <see cref="LinearStateEstimator.Measurements.PhasorEstimate"/> object.</returns>
        public PhasorEstimate Copy()
        {
            PhasorEstimate copy = (PhasorEstimate)this.MemberwiseClone();
            copy.BaseKV = BaseKV.DeepCopy();
            copy.Type = Type;
            return copy;
        }

        /// <summary>
        /// Determines equality between to <see cref="LinearStateEstimator.Measurements.PhasorEstimate"/> objects
        /// </summary>
        /// <param name="target">The target object for equality testing.</param>
        /// <returns>A bool representing the result of the equality check.</returns>
        public override bool Equals(object target)
        {
            if (!base.Equals(target))
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
        /// A string representation of the instance of the class.
        /// </summary>
        /// <returns>A string representation of the instance of the class.</returns>
        public override string ToString()
        {
            return base.ToString();
        }

        /// <summary>
        /// A verbose descriptive string representation of the <see cref="LinearStateEstimator.Measurements.PhasorEstimate"/> class.
        /// </summary>
        /// <returns>A verbose descriptive string representation of the <see cref="LinearStateEstimator.Measurements.PhasorEstimate"/> class.</returns>
        public new string ToVerboseString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine();
            stringBuilder.AppendFormat("----- Phasor Estimate ----------------------------------------------------------");
            stringBuilder.AppendLine();
            stringBuilder.AppendFormat("            Type: " + Type.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("         Base KV: " + BaseKV.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("     MagnitudKey: " + MagnitudeKey + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("        AngleKey: " + AngleKey + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("       Magnitude: " + Magnitude.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("           Angle: " + AngleInDegrees.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("        Reported: " + MeasurementWasReported.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendLine();
            return stringBuilder.ToString();
        }

        public void Keyify(string rootKey)
        {
            AngleKey = $"{rootKey}.Ang.Est";
            MagnitudeKey = $"{rootKey}.Mag.Est";
        }

        public void Unkeyify()
        {
            AngleKey = "Undefined";
            MagnitudeKey = "Undefined";
        }
        #endregion
    }
}
