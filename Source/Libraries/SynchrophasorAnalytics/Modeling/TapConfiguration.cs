//******************************************************************************************************
//  TapConfiguration.cs
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
//  04/25/2014 - Kevin D. Jones
//       Added CurrentPosition and CurrentMultiplier properties.
//  06/08/2014 - Kevin D. Jones
//       Updated XML inline documentation.
//  08/09/2014 - Kevin D. Jones
//       Removed Equals() method to resolve bug in Network Model Editor.
//
//******************************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SynchrophasorAnalytics.Modeling
{
    /// <summary>
    /// Represents the model of how a <see cref="LinearStateEstimator.Modeling.Transformer"/> tap behaves.
    /// </summary>
    [Serializable()]
    public class TapConfiguration : INetworkDescribable
    {
        #region [ Private Constants ]

        /// <summary>
        /// Default values
        /// </summary>
        private const double DEFAULT_LOWER_BOUNDS = 0.95;
        private const double DEFAULT_UPPER_BOUNDS = 1.05;
        private const int DEFAULT_POSITION_LOWER_BOUNDS = -16;
        private const int DEFAULT_POSITION_UPPER_BOUNDS = 16;
        private const int DEFAULT_NOMINAL_POSITION = 0;

        #endregion

        #region [ Private Members ]

        /// <summary>
        /// INetworkDescribable fields
        /// </summary>
        private int m_internalID;
        private string m_acronym;
        private string m_name;
        private string m_description;

        private double m_lowerBounds;
        private double m_upperBounds;
        private int m_positionLowerBounds;
        private int m_positionUpperBounds;
        private int m_positionNominal;
        private int m_currentPosition;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// An integer identifier for each <see cref="LinearStateEstimator.Modeling.TapConfiguration"/> which is intended to be unique among other objects of the same type.
        /// </summary>
        [XmlAttribute("ID")]
        public int InternalID
        {
            get
            {
                return m_internalID;
            }
            set
            {
                m_internalID = value;
            }
        }

        /// <summary>
        /// A descriptive integer for the instance of the <see cref="LinearStateEstimator.Modeling.TapConfiguration"/>. Will always return <see cref="LinearStateEstimator.Modeling.TapConfiguration.InternalID"/>.
        /// </summary>
        [XmlIgnore()]
        public int Number
        {
            get
            {
                return m_internalID;
            }
            set
            {
            }
        }

        /// <summary>
        /// A string acronym for the instance of the <see cref="LinearStateEstimator.Modeling.TapConfiguration"/>.
        /// </summary>
        [XmlAttribute("Acronym")]
        public string Acronym
        {
            get
            {
                return m_acronym;
            }
            set
            {
                m_acronym = value;
            }
        }

        /// <summary>
        /// The string name of the instance of the <see cref="LinearStateEstimator.Modeling.TapConfiguration"/>.
        /// </summary>
        [XmlAttribute("Name")]
        public string Name
        {
            get
            {
                return m_name;
            }
            set
            {
                m_name = value;
            }
        }

        /// <summary>
        /// A string description of the instance of the <see cref="LinearStateEstimator.Modeling.TapConfiguration"/>.
        /// </summary>
        [XmlAttribute("Description")]
        public string Description
        {
            get
            {
                return m_description;
            }
            set
            {
                m_description = value;
            }
        }

        /// <summary>
        /// Gets the type of the object as a string.
        /// </summary>
        [XmlIgnore()]
        public string ElementType
        {
            get 
            {
                return this.GetType().ToString();
            }
        }

        /// <summary>
        /// The lower bounds of a decimal value representing a percentage of the nominal tap ratio
        /// </summary>
        [XmlAttribute("LowerBounds")]
        public double LowerBounds
        {
            get
            {
                return m_lowerBounds;
            }
            set
            {
                m_lowerBounds = value;
            }
        }

        /// <summary>
        /// The upper bounds of a decimal value representing a percentage of the nominal tap ratio.
        /// </summary>
        [XmlAttribute("UpperBounds")]
        public double UpperBounds
        {
            get
            {
                return m_upperBounds;
            }
            set
            {
                m_upperBounds = value;
            }
        }

        /// <summary>
        /// The lower bounds of an integer value which represents the position of the tap.
        /// </summary>
        [XmlAttribute("PositionLowerBounds")]
        public int PositionLowerBounds
        {
            get
            {
                return m_positionLowerBounds;
            }
            set
            {
                m_positionLowerBounds = value;
            }
        }

        /// <summary>
        /// The upper bounds of an integer value which represents the position of the tap.
        /// </summary>
        [XmlAttribute("PositionUpperBounds")]
        public int PositionUpperBounds
        {
            get
            {
                return m_positionUpperBounds;
            }
            set
            {
                m_positionUpperBounds = value;
            }
        }

        /// <summary>
        /// The integer value which represents the position of the tap that is considered nominal.
        /// </summary>
        [XmlAttribute("PositionNominal")]
        public int PositionNominal
        {
            get
            {
                return m_positionNominal;
            }
            set
            {
                m_positionNominal = value;
            }
        }

        /// <summary>
        /// The integer value which represents the current position of the tap inside of the upper and lower bounds.
        /// </summary>
        [XmlIgnore()]
        public int CurrentPosition
        {
            get
            {
                return m_currentPosition;
            }
            set
            {
                if (value < m_positionLowerBounds)
                {
                    m_currentPosition = PositionLowerBounds;
                }
                else if (value > m_positionUpperBounds)
                {
                    m_currentPosition = PositionUpperBounds;
                }
                else
                {
                    m_currentPosition = value;
                }
            }
        }

        /// <summary>
        /// The multiplier which represents the off nominal transformer tap ratios.
        /// </summary>
        [XmlIgnore()]
        public double CurrentMultiplier
        {
            get
            {
                double currentMultiplier = 1;
                if (Multipliers.TryGetValue(m_currentPosition, out currentMultiplier))
                {
                    return currentMultiplier;
                }
                return currentMultiplier;
            }
        }

        /// <summary>
        /// The discretized set of multipliers that represent the complete set of possible off nominal transformer tap ratios.
        /// </summary>
        [XmlIgnore()]
        public Dictionary<int, double> Multipliers
        {
            get
            {
                Dictionary<int, double> multipliers = new Dictionary<int, double>();

                // Calculate the range.
                double boundsDifference = m_upperBounds - m_lowerBounds;
                double enumeratedBoundsDifference = m_positionUpperBounds - m_positionLowerBounds;
                
                // The incremental change between tap positions is equal to the slope of the linear relationship between the enumeration and the upper and lower bounds
                double increment = boundsDifference / enumeratedBoundsDifference;

                // Store the multipliers in a Dictionary that can be accessed with the integer tap position.
                for (int i = 0; i <= enumeratedBoundsDifference; i++)
                {
                    multipliers.Add(m_positionLowerBounds + i, m_lowerBounds + i * increment);
                }

                return multipliers;
            }
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// A blank constructor with default values.
        /// </summary>
        public TapConfiguration()
            :this(DEFAULT_LOWER_BOUNDS, DEFAULT_UPPER_BOUNDS, DEFAULT_POSITION_LOWER_BOUNDS, DEFAULT_POSITION_UPPER_BOUNDS, DEFAULT_NOMINAL_POSITION)
        {
        }

        /// <summary>
        /// The designated constructor for the <see cref="LinearStateEstimator.Modeling.TapConfiguration"/> class.
        /// </summary>
        /// <param name="lowerBounds">The lower bounds of a decimal value representing a percentage of the nominal tap ratio</param>
        /// <param name="upperBounds">The upper bounds of a decimal value representing a percentage of the nominal tap ratio.</param>
        /// <param name="enumeratedLowerBounds">The lower bounds of an integer value which represents the position of the tap.</param>
        /// <param name="enumeratedUpperBounds">The upper bounds of an integer value which represents the position of the tap.</param>
        /// <param name="enumeratedNominal">The integer value which represents the position of the tap that is considered nominal.</param>
        public TapConfiguration(double lowerBounds, double upperBounds, int enumeratedLowerBounds, int enumeratedUpperBounds, int enumeratedNominal)
        {
            m_lowerBounds = lowerBounds;
            m_upperBounds = upperBounds;
            m_positionLowerBounds = enumeratedLowerBounds;
            m_positionUpperBounds = enumeratedUpperBounds;
            m_positionNominal = enumeratedNominal;
            m_currentPosition = m_positionNominal;
        }

        #endregion

        #region [ Public Methods ]

        /// <summary>
        /// A descriptive string representation of the <see cref="LinearStateEstimator.Modeling.TapConfiguration"/> class instance. The format is <i>TapConfiguration,internalId,acronym,name,description,lowerBounds,upperBounds,lowerPosition,upperPosition,nominalPosition</i> and can be used for a rudimentary momento design pattern.
        /// </summary>
        /// <returns>A string representation of the <see cref="LinearStateEstimator.Modeling.TapConfiguration"/>.</returns>
        public override string ToString()
        {
            return "Tap," + m_internalID.ToString() + "," + m_acronym + "," + m_name + "," + m_description + "," + m_lowerBounds.ToString() + "," + m_upperBounds.ToString() + "," + m_positionLowerBounds.ToString() + "," + m_positionUpperBounds.ToString() + "," + m_positionNominal.ToString();
        }

        /// <summary>
        /// A verbose string representation of the <see cref="LinearStateEstimator.Modeling.TapConfiguration"/>.
        /// </summary>
        /// <returns>A verbose string representation of the <see cref="LinearStateEstimator.Modeling.TapConfiguration"/>.</returns>
        public string ToVerboseString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine();
            stringBuilder.AppendFormat("----- Tap Configuration --------------------------------------------------------");
            stringBuilder.AppendLine();
            stringBuilder.AppendFormat("      InternalID: " + m_internalID.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("         Acronym: " + m_acronym + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("            Name: " + m_name + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("     Description: " + m_description + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("          Bounds: {0} to {1}{2}", m_lowerBounds, m_upperBounds, Environment.NewLine);
            stringBuilder.AppendFormat("       Positions: {0} to {1}{2}", m_positionLowerBounds, m_positionUpperBounds, Environment.NewLine);
            stringBuilder.AppendFormat("         Nominal: {0}{1}", m_positionNominal, Environment.NewLine);
            stringBuilder.AppendLine();
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Determines equality between to <see cref="LinearStateEstimator.Modeling.TapConfiguration"/> objects
        /// </summary>
        /// <param name="target">The target object for equality testing.</param>
        /// <returns>A bool representing the result of the equality check.</returns>
        //public override bool Equals(object target)
        //{
        //    // If parameter is null return false.
        //    if (target == null)
        //    {
        //        return false;
        //    }

        //    // If parameter cannot be cast to TapConfiguration return false.
        //    TapConfiguration tapConfiguration = target as TapConfiguration;

        //    if ((object)tapConfiguration == null)
        //    {
        //        return false;
        //    }

        //    // Return true if all of the fields match:
        //    if (this.m_internalID != tapConfiguration.InternalID)
        //    {
        //        return false;
        //    }
        //    else if (this.m_acronym != tapConfiguration.Acronym)
        //    {
        //        return false;
        //    }
        //    else if (this.m_name != tapConfiguration.Name)
        //    {
        //        return false;
        //    }
        //    else if (this.m_description != tapConfiguration.Description)
        //    {
        //        return false;
        //    }
        //    else if (this.m_lowerBounds != tapConfiguration.LowerBounds)
        //    {
        //        return false;
        //    }
        //    else if (this.m_upperBounds != tapConfiguration.UpperBounds)
        //    {
        //        return false;
        //    }
        //    else if (this.m_positionLowerBounds != tapConfiguration.PositionLowerBounds)
        //    {
        //        return false;
        //    }
        //    else if (this.m_positionLowerBounds != tapConfiguration.PositionUpperBounds)
        //    {
        //        return false;
        //    }
        //    else if (this.m_positionNominal != tapConfiguration.PositionNominal)
        //    {
        //        return false;
        //    }
        //    else
        //    {
        //        return true;
        //    }
        //}

        /// <summary>
        /// Overridden to prevent compilation warnings.
        /// </summary>
        /// <returns>An integer hash code determined by the base class.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Performs a deep copy of the <see cref="LinearStateEstimator.Modeling.TapConfiguration"/>.
        /// </summary>
        /// <returns>A deep copy of the <see cref="LinearStateEstimator.Modeling.TapConfiguration"/>.</returns>
        public TapConfiguration DeepCopy()
        {
            return (TapConfiguration)this.MemberwiseClone();
        }

        #endregion
    }
}
