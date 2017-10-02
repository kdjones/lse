//******************************************************************************************************
//  Switch.cs
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

namespace SynchrophasorAnalytics.Modeling
{
    /// <summary>
    /// Represents a switching device which does accept telemetry from its analagous field device.
    /// </summary>
    public class Switch : SwitchingDeviceBase
    {
        #region [ Private Members ]

        private bool m_isInDefaultMode;

        private Substation m_parentSubstation;
        private int m_parentSubstationID;

        private TransmissionLine m_parentTransmissionLine;
        private int m_parentTransmissionLineID;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// The status of the switching device.
        /// </summary>
        [XmlIgnore()]
        public bool IsInDefaultMode
        {
            get
            {
                return m_isInDefaultMode;
            }
            set
            {
                m_isInDefaultMode = value;
            }
        }

        /// <summary>
        /// The parent substation of the device.
        /// </summary>
        [XmlIgnore()]
        public Substation ParentSubstation
        {
            get
            {
                return m_parentSubstation;
            }
            set
            {
                m_parentSubstation = value;
                m_parentSubstationID = value.InternalID;
            }
        }

        /// <summary>
        /// The internal id of the parent substation of the device.
        /// </summary>
        [XmlAttribute("ParentSubstation")]
        public int ParentSubstationID
        {
            get
            {
                return m_parentSubstationID;
            }
            set
            {
                m_parentSubstationID = value;
            }
        }

        /// <summary>
        /// The parent transmission line of the device.
        /// </summary>
        [XmlIgnore()]
        public TransmissionLine ParentTransmissionLine
        {
            get
            {
                return m_parentTransmissionLine;
            }
            set
            {
                m_parentTransmissionLine = value;
                m_parentTransmissionLineID = value.InternalID;
            }
        }

        /// <summary>
        /// The internal id of the parent transmission line of the device.
        /// </summary>
        [XmlAttribute("ParentTransmissionLine")]
        public int ParentTransmissionLineID
        {
            get
            {
                return m_parentTransmissionLineID;
            }
            set
            {
                m_parentTransmissionLineID = value;
            }
        }

        /// <summary>
        /// A boolean flag which indicates whether the device is open or not.
        /// </summary>
        [XmlIgnore()]
        public bool IsOpen
        {
            get
            {
                if (InPruningMode)
                {
                    return true;
                }
                if (m_isInDefaultMode)
                {
                    if (NormalState.Equals(SwitchingDeviceNormalState.Open))
                    {
                        return true;
                    }
                    else if (NormalState.Equals(SwitchingDeviceNormalState.Closed))
                    {
                        return false;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    if (ActualState.Equals(SwitchingDeviceActualState.Open))
                    {
                        return true;
                    }
                    else if (ActualState.Equals(SwitchingDeviceActualState.Closed))
                    {
                        return false;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        /// <summary>
        /// A boolean flag which represents whether the device is closed or not.
        /// </summary>
        [XmlIgnore()]
        public bool IsClosed
        {
            get
            {
                return !IsOpen;
            }
        }

        [XmlIgnore()]
        public bool InPruningMode
        {
            get
            {
                return m_parentSubstation.ParentDivision.ParentCompany.ParentModel.InPruningMode;
            }
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// The designated constructor for the class.
        /// </summary>
        public Switch()
        {
            m_isInDefaultMode = true;
        }

        
        #endregion

        #region [ Public Methods ]

        /// <summary>
        /// A string representation of the <see cref="Switch"/>.
        /// </summary>
        /// <returns>A string representation of the <see cref="Switch"/>.</returns>
        public override string ToString()
        {
            return "Switch," + base.ToString() + "," + m_parentSubstationID.ToString() + "," + m_parentTransmissionLineID.ToString();
        }

        /// <summary>
        /// A verbose string representation of the <see cref="Switch"/>.
        /// </summary>
        /// <returns>A verbose string representation of the <see cref="Switch"/>.</returns>
        public new string ToVerboseString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine();
            stringBuilder.AppendFormat("----- Switch -------------------------------------------------------------------");
            stringBuilder.AppendLine();
            stringBuilder.AppendFormat("      InternalID: " + InternalID.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("          Number: " + Number.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("            Name: " + Name + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("     Description: " + Description + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("        Normally: " + NormalState.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("        Actually: " + ActualState.ToString() + "{0}", Environment.NewLine); stringBuilder.AppendFormat("   Inferred Open: " + IsInferredOpen.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat(" Inferred Closed: " + IsInferredClosed.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("         Is Open: " + IsOpen.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("       Is Closed: " + IsClosed.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat(" In Pruning Mode: " + InPruningMode.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("          Manual: " + InManualOverrideMode.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("        FromNode: " + FromNode.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("          ToNode: " + ToNode.ToString() + "{0}", Environment.NewLine);
            if (m_parentSubstation != null)
            {
                stringBuilder.AppendFormat("ParentSubstation: " + m_parentSubstation.ToString() + "{0}", Environment.NewLine);
            }
            if (m_parentTransmissionLine != null)
            {
                stringBuilder.AppendFormat("ParntTrnsmsnLine: " + m_parentTransmissionLine.ToString() + "{0}", Environment.NewLine);
            }
            stringBuilder.AppendFormat("   InDefaultMode: " + m_isInDefaultMode.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendLine();
            return stringBuilder.ToString();
        }

        #endregion
    }
}
