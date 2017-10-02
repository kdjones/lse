//******************************************************************************************************
//  VoltageLevel.cs
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
//  06/08/2014 - Kevin D. Jones
//       Updated XML inline documentation.
//  07/07/2014 - Kevin D. Jones
//       Added Guid
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
    /// Represents the base kV of a <see cref="LinearStateEstimator.Modeling.Node"/> and of <see cref="LinearStateEstimator.Measurements.PhasorBase"/>.
    /// </summary>
    [Serializable()]
    public class VoltageLevel : INetworkDescribable
    {
        #region [ Private Constants ]

        private const int DEFAULT_INTERNAL_ID = 0;
        private const string DEFAULT_ACRONYM = "KV";
        #endregion

        #region [ Private Members ]

        private Guid m_uniqueId;
        private int m_internalID;
        private double m_voltageLevel;

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
        /// An integer identifier for each <see cref="LinearStateEstimator.Modeling.VoltageLevel"/> which is intended to be unique among other objects of the same type.
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
        /// A descriptive number for the <see cref="LinearStateEstimator.Modeling.VoltageLevel"/> object. Will always return the <see cref="LinearStateEstimator.Modeling.VoltageLevel.InternalID"/>.
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
        /// A descriptive acronym for the <see cref="LinearStateEstimator.Modeling.VoltageLevel"/> object. Will always return 'KV'.
        /// </summary>
        [XmlIgnore()]
        public string Acronym
        {
            get 
            { 
                return DEFAULT_ACRONYM; 
            }
            set 
            { 
            }
        }

        /// <summary>
        /// A name for the <see cref="LinearStateEstimator.Modeling.VoltageLevel"/> object. Will always return 'voltage + kV'. For example, '500 kV'.
        /// </summary>
        [XmlIgnore()]
        public string Name
        {
            get 
            { 
                return m_voltageLevel.ToString() + " kV"; 
            }
            set 
            { 
            }
        }

        /// <summary>
        /// A description of the <see cref="LinearStateEstimator.Modeling.VoltageLevel"/> object. Will always return 'voltage + kV Voltage Level Definition'. For example, '500 kV Voltage Level Definition'.
        /// </summary>
        [XmlIgnore()]
        public string Description
        {
            get 
            { 
                return m_voltageLevel.ToString() + " kV Voltage Level Definition"; 
            }
            set 
            { 
            }
        }

        /// <summary>
        /// Returns the type of the object as a string.
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
        /// The voltage level in line-line kilovolts.
        /// </summary>
        [XmlAttribute("KV")]
        public double Value
        {
            get 
            { 
                return m_voltageLevel; 
            }
            set 
            { 
                m_voltageLevel = value; 
            }
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// A blank constructor with default values.
        /// </summary>
        public VoltageLevel()
            :this(0, 0)
        {
        }

        /// <summary>
        /// The designated constructor for the <see cref="LinearStateEstimator.Modeling.VoltageLevel"/> class.
        /// </summary>
        /// <param name="internalID">The unique integer identifier for each instance of a <see cref="LinearStateEstimator.Modeling.VoltageLevel"/> object.</param>
        /// <param name="voltageLevel">The voltage level in line-line kilovolts.</param>
        public VoltageLevel(int internalID, double voltageLevel)
        {
            m_internalID = internalID;
            m_voltageLevel = voltageLevel;
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Performs a deep copy of the target <see cref="LinearStateEstimator.Modeling.VoltageLevel"/> object.
        /// </summary>
        /// <returns>A deep copy of the target <see cref="LinearStateEstimator.Modeling.VoltageLevel"/> object.</returns>
        public VoltageLevel DeepCopy()
        {
            VoltageLevel copy = (VoltageLevel)this.MemberwiseClone();
            return copy;
        }

        /// <summary>
        /// A descriptive string representation of the <see cref="LinearStateEstimator.Modeling.VoltageLevel"/> class instance. The format is <i>VoltageLevel,internalID,baseKv</i> and can be used for a rudimentary momento design pattern
        /// </summary>
        /// <returns>A descriptive string representation of the <see cref="LinearStateEstimator.Modeling.VoltageLevel"/> class instance.</returns>
        public override string ToString()
        {
            return "VoltageLevel," + m_internalID.ToString() + "," + m_voltageLevel.ToString();
        }

        /// <summary>
        /// A verbose descriptive string representation of the <see cref="LinearStateEstimator.Modeling.VoltageLevel"/> class instance.
        /// </summary>
        /// <returns>A verbose descriptive string representation of the <see cref="LinearStateEstimator.Modeling.VoltageLevel"/> class instance.</returns>
        public string ToVerboseString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine();
            stringBuilder.AppendFormat("----- Voltage Level ------------------------------------------------------------");
            stringBuilder.AppendLine();
            stringBuilder.AppendFormat("      InternalID: " + m_internalID.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("           Value: " + Name + "{0}", Environment.NewLine);
            stringBuilder.AppendLine();
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Overridden to prevent compilation warnings
        /// </summary>
        /// <returns>A integer hash code.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Determines equality between to <see cref="LinearStateEstimator.Modeling.VoltageLevel"/> objects
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
            VoltageLevel voltageLevel = target as VoltageLevel;

            if ((object)voltageLevel == null)
            {
                return false;
            }

            // Return true if the fields match:
            if (this.m_internalID != voltageLevel.InternalID)
            {
                return false;
            }
            else if (this.m_voltageLevel != voltageLevel.Value)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        
        #endregion
    }
}
