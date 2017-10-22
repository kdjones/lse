//******************************************************************************************************
//  CircuitBreaker.cs
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
//
//******************************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using SynchrophasorAnalytics.Measurements;

namespace SynchrophasorAnalytics.Modeling
{
    /// <summary>
    /// Represents a circuit breaker in an electric power network which can accept a logical state from an input measurement.
    /// </summary>
    /// <seealso cref="LinearStateEstimator.Modeling.Switch"/>
    /// <seealso cref="LinearStateEstimator.Modeling.SwitchingDeviceBase"/>
    public class CircuitBreaker : SwitchingDeviceBase
    {
        #region [ Private Constants ]

        /// <summary>
        /// Default values
        /// </summary>
        private const int DEFAULT_INTERNAL_ID = 0;
        private const int DEFAULT_NUMBER = 0;
        private const string DEFAULT_NAME = "Undefined";
        private const string DEFAULT_DESCRIPTION = "Uundefined";
        #endregion

        #region [ Private Members ]

        private BreakerStatus m_breakerStatus;
        private int m_breakerStatusID;

        private Substation m_parentSubstation;
        private int m_parentSubstationID;

        #endregion

        public static string CSV_HEADER = $"InternalID,Number,Name,Description,Can Infer State,Group A Reported, Group B Reported,Normal,Actual,Measured,Measured Open,Measured Closed,Inferred Open,Inferred Closed,Open,Closed,Pruning Mode,From Node,To Node,Manual,Substation,Status{Environment.NewLine}";

        #region [ Properties ]

        /// <summary>
        /// The <see cref="LinearStateEstimator.Measurements.BreakerStatus"/> contains the logical state from an input measurement. Each <see cref="LinearStateEstimator.Modeling.CircuitBreaker"/> can reference only one <see cref="LinearStateEstimator.Measurements.BreakerStatus"/>. This property will be non-null in the absence of a modeled input signal and will remain in its initial state during operation unless its inner value is updated.
        /// </summary>
        [XmlIgnore()]
        public BreakerStatus Status
        {
            get 
            { 
                return m_breakerStatus; 
            }
            set 
            { 
                m_breakerStatus = value;
                m_breakerStatusID = value.InternalID;
            }
        }

        /// <summary>
        /// The <see cref="LinearStateEstimator.Measurements.BreakerStatus.InternalID"/> of the <see cref="LinearStateEstimator.Modeling.CircuitBreaker.Status"/> object. This integer identifier is stored as part of the <see cref="LinearStateEstimator.Modeling.CircuitBreaker"/> in order to preserve hierarchical relationships during serialization and reassign reference during network model initialization .
        /// </summary>
        [XmlAttribute("Status")]
        public int StatusID
        {
            get
            {
                return m_breakerStatusID;
            }
            set
            {
                m_breakerStatusID = value;
            }
        }

        /// <summary>
        /// The <see cref="LinearStateEstimator.Modeling.Substation"/> which owns the <see cref="LinearStateEstimator.Modeling.CircuitBreaker"/>.
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
        /// The <see cref="LinearStateEstimator.Modeling.Substation.InternalID"/> of the <see cref="LinearStateEstimator.Modeling.CircuitBreaker.ParentSubstation"/> object. This integer identifier is stored as part of the <see cref="LinearStateEstimator.Modeling.CircuitBreaker"/> in order to preserve hierarchical relationships during serialization and reassign reference during network model initialization .
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

        [XmlIgnore()]
        public bool InPruningMode
        {
            get
            {
                return m_parentSubstation.ParentDivision.ParentCompany.ParentModel.InPruningMode;
            }
        }

        [XmlAttribute("Measured")]
        public SwitchingDeviceMeasuredState MeasuredState
        {
            get
            {
                if (StatusID != 0 && Status != null)
                {
                    if (Status.IsEnabled && Status.Key != "Undefined")
                    {
                        if (Status.Value)
                        {
                            return SwitchingDeviceMeasuredState.Open;
                        }
                        return SwitchingDeviceMeasuredState.Closed;
                    }
                }
                return SwitchingDeviceMeasuredState.Unmeasured;
            }
        }

        /// <summary>
        /// A boolen flag which is high if the <see cref="LinearStateEstimator.Modeling.CircuitBreaker"/> is in an open state defined by <see cref="LinearStateEstimator.Modeling.SwitchingDeviceActualState"/>.
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
                if (InManualOverrideMode)
                {
                    if (ActualState == SwitchingDeviceActualState.Open)
                    {
                        return true;
                    }
                    else if (ActualState == SwitchingDeviceActualState.Closed)
                    {
                        return false;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (StatusID != 0)
                {
                    return Status.Value;
                }
                else if (UseInferredStateAsActualProxy)
                {
                    if (ActualState == SwitchingDeviceActualState.Open)
                    {
                        return true;
                    }
                    else if (ActualState == SwitchingDeviceActualState.Closed)
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
            }
        }

        /// <summary>
        /// A boolen flag which is high if the <see cref="LinearStateEstimator.Modeling.CircuitBreaker"/> is in an closed state defined by <see cref="LinearStateEstimator.Modeling.SwitchingDeviceActualState"/>.
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
        public bool IsMeasuredOpen
        {
            get
            {
                if (MeasuredState.Equals(SwitchingDeviceMeasuredState.Open))
                {
                    return true;
                }
                return false;
            }
        }

        [XmlIgnore()]
        public bool IsMeasuredClosed
        {
            get
            {
                if (MeasuredState.Equals(SwitchingDeviceMeasuredState.Closed))
                {
                    return true;
                }
                return false;
            }
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// A blank constructor with default values. The default physical state of a <see cref="LinearStateEstimator.Modeling.CircuitBreaker"/> is <see cref="LinearStateEstimator.Modeling.SwitchingDeviceActualState.Open"/>.
        /// </summary>
        public CircuitBreaker()
            :this(DEFAULT_INTERNAL_ID, DEFAULT_NUMBER, DEFAULT_NAME, DEFAULT_DESCRIPTION, SwitchingDeviceNormalState.Open)
        {
        }

        /// <summary>
        /// A constructor for the <see cref="LinearStateEstimator.Modeling.CircuitBreaker"/> class which requires all of the properties defined by the <see cref="LinearStateEstimator.Modeling.INetworkDescribable"/> interface but requires no information regarding its input signal or its location in the network.  
        /// </summary>
        /// <param name="internalID">A integer identifier for the instance of the <see cref="LinearStateEstimator.Modeling.CircuitBreaker"/> which is intended to be unique among other objects of the same type.</param>
        /// <param name="number">A descriptive integer for the instance of the <see cref="LinearStateEstimator.Modeling.CircuitBreaker"/>. There are no restrictions on uniqueness.</param>
        /// <param name="name">A string name of the instance of the <see cref="LinearStateEstimator.Modeling.CircuitBreaker"/>.</param>
        /// <param name="description">A string description of the <see cref="LinearStateEstimator.Modeling.CircuitBreaker"/>.</param>
        /// <param name="normalState">The <i>NORMAL</i> physical state of the <see cref="LinearStateEstimator.Modeling.CircuitBreaker"/>; the state the breaker was designed to operate in most of the time. Defined by <see cref="LinearStateEstimator.Modeling.SwitchingDeviceNormalState"/>.</param>
        public CircuitBreaker(int internalID, int number, string name, string description, SwitchingDeviceNormalState normalState)
            :this(internalID, number, name, description,  normalState, 0, 0)
        {
        }

        /// <summary>
        /// A constructor for the <see cref="LinearStateEstimator.Modeling.CircuitBreaker"/> class which requires all of the properties defined by the <see cref="LinearStateEstimator.Modeling.INetworkDescribable"/> interface and requires the input signal reference but does not require informatino about its location in the network.
        /// </summary>
        /// <param name="internalID">A integer identifier for the instance of the <see cref="LinearStateEstimator.Modeling.CircuitBreaker"/> which is intended to be unique among other objects of the same type.</param>
        /// <param name="number">A descriptive integer for the instance of the <see cref="LinearStateEstimator.Modeling.CircuitBreaker"/>. There are no restrictions on uniqueness.</param>
        /// <param name="name">A string name of the instance of the <see cref="LinearStateEstimator.Modeling.CircuitBreaker"/>.</param>
        /// <param name="description">A string description of the <see cref="LinearStateEstimator.Modeling.CircuitBreaker"/>.</param>
        /// <param name="normalState">The <i>NORMAL</i> physical state of the <see cref="LinearStateEstimator.Modeling.CircuitBreaker"/>; the state the breaker was designed to operate in most of the time. Defined by <see cref="LinearStateEstimator.Modeling.SwitchingDeviceNormalState"/>.</param>
        /// <param name="status">Contains the logical state from an input measurement. Each <see cref="LinearStateEstimator.Modeling.CircuitBreaker"/> can reference only one <see cref="LinearStateEstimator.Measurements.BreakerStatus"/>.</param>
        public CircuitBreaker(int internalID, int number, string name, string description, SwitchingDeviceNormalState normalState, BreakerStatus status)
            :this(internalID, number, name, description, normalState, 0, 0, status)
        {
        }

        /// <summary>
        /// A constructor for the <see cref="LinearStateEstimator.Modeling.CircuitBreaker"/> class which requires all of the properties defined by the <see cref="LinearStateEstimator.Modeling.INetworkDescribable"/> interface and requires the <b>From</b> and <b>To</b> node to be identified but does not require information about the input signal.
        /// </summary>
        /// <param name="internalID">A integer identifier for the instance of the <see cref="LinearStateEstimator.Modeling.CircuitBreaker"/> which is intended to be unique among other objects of the same type.</param>
        /// <param name="number">A descriptive integer for the instance of the <see cref="LinearStateEstimator.Modeling.CircuitBreaker"/>. There are no restrictions on uniqueness.</param>
        /// <param name="name">A string name of the instance of the <see cref="LinearStateEstimator.Modeling.CircuitBreaker"/>.</param>
        /// <param name="description">A string description of the <see cref="LinearStateEstimator.Modeling.CircuitBreaker"/>.</param>
        /// <param name="normalState">The <i>NORMAL</i> physical state of the <see cref="LinearStateEstimator.Modeling.CircuitBreaker"/>; the state the breaker was designed to operate in most of the time. Defined by <see cref="LinearStateEstimator.Modeling.SwitchingDeviceNormalState"/>.</param>
        /// <param name="fromNodeID">The <see cref="LinearStateEstimator.Modeling.Node.InternalID"/> of the <b>From</b> <see cref="LinearStateEstimator.Modeling.Node"/></param>
        /// <param name="toNodeID">The <see cref="LinearStateEstimator.Modeling.Node.InternalID"/> of the <b>To</b> <see cref="LinearStateEstimator.Modeling.Node"/></param>
        public CircuitBreaker(int internalID, int number, string name, string description, SwitchingDeviceNormalState normalState, int fromNodeID, int toNodeID)
            :this(internalID, number, name, description, normalState, fromNodeID, toNodeID, new BreakerStatus())
        {
        }

        /// <summary>
        /// A constructor for the <see cref="LinearStateEstimator.Modeling.CircuitBreaker"/> class which requires all of the properties defined by the <see cref="LinearStateEstimator.Modeling.INetworkDescribable"/> interface and requires both information about the input signal and information about its location in the network.
        /// </summary>
        /// <param name="internalID">A integer identifier for the instance of the <see cref="LinearStateEstimator.Modeling.CircuitBreaker"/> which is intended to be unique among other objects of the same type.</param>
        /// <param name="number">A descriptive integer for the instance of the <see cref="LinearStateEstimator.Modeling.CircuitBreaker"/>. There are no restrictions on uniqueness.</param>
        /// <param name="name">A string name of the instance of the <see cref="LinearStateEstimator.Modeling.CircuitBreaker"/>.</param>
        /// <param name="description">A string description of the <see cref="LinearStateEstimator.Modeling.CircuitBreaker"/>.</param>
        /// <param name="normalState">The <i>NORMAL</i> physical state of the <see cref="LinearStateEstimator.Modeling.CircuitBreaker"/>; the state the breaker was designed to operate in most of the time. Defined by <see cref="LinearStateEstimator.Modeling.SwitchingDeviceNormalState"/>.</param>
        /// <param name="fromNodeID">The <see cref="LinearStateEstimator.Modeling.Node.InternalID"/> of the <b>From</b> <see cref="LinearStateEstimator.Modeling.Node"/></param>
        /// <param name="toNodeID">The <see cref="LinearStateEstimator.Modeling.Node.InternalID"/> of the <b>To</b> <see cref="LinearStateEstimator.Modeling.Node"/></param>
        /// <param name="status">Contains the logical state from an input measurement. Each <see cref="LinearStateEstimator.Modeling.CircuitBreaker"/> can reference only one <see cref="LinearStateEstimator.Measurements.BreakerStatus"/>.</param>
        public CircuitBreaker(int internalID, int number, string name, string description, SwitchingDeviceNormalState normalState, int fromNodeID, int toNodeID, BreakerStatus status)
            :base(internalID, number, name, description, normalState, fromNodeID, toNodeID)
        {
            m_breakerStatus = status;
        }

        #endregion

        #region [ Public Methods ]



        /// <summary>
        /// A string representation of the <see cref="LinearStateEstimator.Modeling.CircuitBreaker"/>. The format is <i>CircuitBreaker,baseProperty1, baseProperty2, ... ,statusInternalId,parentSubstationInternalId</i> and can be used for a rudimentary momento design pattern.
        /// </summary>
        /// <returns>A string representation of the <see cref="LinearStateEstimator.Modeling.CircuitBreaker"/>.</returns>
        public override string ToString()
        {
            return "CircuitBreaker," + base.ToString() + "," + m_breakerStatusID.ToString() + "," + m_parentSubstationID.ToString();
        }

        /// <summary>
        /// A verbose string representation of the <see cref="LinearStateEstimator.Modeling.CircuitBreaker"/> and can be used for detailed text based output via a console or a text file.
        /// </summary>
        /// <returns>A verbose string representation of the <see cref="LinearStateEstimator.Modeling.CircuitBreaker"/>.</returns>
        public new string ToVerboseString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine();
            stringBuilder.AppendFormat("----- Circuit Breaker ----------------------------------------------------------");
            stringBuilder.AppendLine();
            stringBuilder.AppendFormat("      InternalID: " + InternalID.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("          Number: " + Number.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("            Name: " + Name + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("     Description: " + Description + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat(" Can Infer State: " + CanInferState.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("Group A Reported: " + CrossDevicePhasors.GroupAWasReported + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("Group B Reported: " + CrossDevicePhasors.GroupBWasReported + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("          Normal: " + NormalState.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("          Actual: " + ActualState.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("        Measured: " + MeasuredState.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("   Measured Open: " + IsMeasuredOpen.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat(" Measured Closed: " + IsMeasuredClosed.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("   Inferred Open: " + IsInferredOpen.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat(" Inferred Closed: " + IsInferredClosed.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("         Is Open: " + IsOpen.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("       Is Closed: " + IsClosed.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat(" In Pruning Mode: " + InPruningMode.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("          Manual: " + InManualOverrideMode.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("        FromNode: " + FromNode.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("          ToNode: " + ToNode.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("ParentSubstation: " + m_parentSubstation.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("          Status: {0}", Environment.NewLine);
            stringBuilder.AppendFormat(m_breakerStatus.ToStatusString() + "{0}", Environment.NewLine);
            stringBuilder.AppendLine();
            return stringBuilder.ToString();
        }

        public new string ToCsvLineString()
        {
            StringBuilder csvLine = new StringBuilder();
            csvLine.AppendFormat($"{InternalID},");
            csvLine.AppendFormat($"{Number},");
            csvLine.AppendFormat($"{Name},");
            csvLine.AppendFormat($"{Description},");
            csvLine.AppendFormat($"{CanInferState},");
            csvLine.AppendFormat($"{CrossDevicePhasors.GroupAWasReported},");
            csvLine.AppendFormat($"{CrossDevicePhasors.GroupBWasReported},");
            csvLine.AppendFormat($"{NormalState},");
            csvLine.AppendFormat($"{ActualState},");
            csvLine.AppendFormat($"{MeasuredState},");
            csvLine.AppendFormat($"{IsMeasuredOpen},");
            csvLine.AppendFormat($"{IsMeasuredClosed},");
            csvLine.AppendFormat($"{IsInferredOpen},");
            csvLine.AppendFormat($"{IsInferredClosed},");
            csvLine.AppendFormat($"{IsOpen},");
            csvLine.AppendFormat($"{IsClosed},");
            csvLine.AppendFormat($"{InPruningMode},");
            csvLine.AppendFormat($"{InManualOverrideMode},");
            csvLine.AppendFormat($"{FromNode.Name},");
            csvLine.AppendFormat($"{ToNode.Name},");
            csvLine.AppendFormat($"{ParentSubstation.Name},");
            csvLine.AppendFormat($"{Status}{Environment.NewLine}");
            return csvLine.ToString();
        }

        #endregion
    }
}
