//******************************************************************************************************
//  Node.cs
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
using System.Numerics;
using System.Text;
using System.Xml.Serialization;
using SynchrophasorAnalytics.Modeling;
using SynchrophasorAnalytics.Measurements;

namespace SynchrophasorAnalytics.Modeling
{
    /// <summary>
    /// Represents a single electrical node in a power system.
    /// </summary>
    [Serializable()]
    public class Node : INetworkDescribable
    {
        #region [ Private Constants ]

        /// <summary>
        /// Default values
        /// </summary>
        private const int DEFAULT_INTERNAL_ID = 0;
        private const int DEFAULT_NUMBER = 0;
        private const string DEFAULT_ACRONYM = "ND";
        private const string DEFAULT_NAME = "Undefined";
        private const string DEFAULT_DESCRIPTION = "Uundefined";

        #endregion

        #region [ Private Members ]

        /// <summary>
        /// INetworkDescribable fields
        /// </summary>
        private Guid m_uniqueId;
        private int m_internalID;
        private int m_number;
        private string m_acronym;
        private string m_name;
        private string m_description;

        /// <summary>
        /// Voltage level
        /// </summary>
        private VoltageLevel m_voltageLevel;
        private int m_voltageLevelID;

        /// <summary>
        /// Parents
        /// </summary>
        private Substation m_parentSubstation;
        private int m_parentSubstationID;
        private TransmissionLine m_parentTransmissionLine;
        private int m_parentTransmissionLineID;

        /// <summary>
        /// Children
        /// </summary>
        private VoltagePhasorGroup m_voltage;
        private int m_voltagePhasorGroupID;

        private ObservationState m_observationState;
        private string m_observationStateKey;
        private string m_observedBusIdKey;
        private List<Node> m_adjacencies;

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
        /// An integer identifier for each <see cref="LinearStateEstimator.Modeling.Node"/> which is intended to be unique among other objects of the same type.
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
        /// A descriptive integer for the instance of the <see cref="LinearStateEstimator.Modeling.Node"/>. There are no restrictions on uniqueness.
        /// </summary>
        [XmlAttribute("Number")]
        public int Number
        {
            get 
            { 
                return m_number; 
            }
            set 
            { 
                m_number = value; 
            }
        }

        /// <summary>
        /// A string acronym for the instance of the <see cref="LinearStateEstimator.Modeling.Node"/>
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
        /// The string name of the instance of the <see cref="LinearStateEstimator.Modeling.Node"/>.
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
        /// A string description of the instance of the <see cref="LinearStateEstimator.Modeling.Node"/>. 
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
        /// The <see cref="LinearStateEstimator.Modeling.VoltageLevel"/> of the <see cref="LinearStateEstimator.Modeling.Node"/>.
        /// </summary>
        [XmlIgnore()]
        public VoltageLevel BaseKV
        {
            get 
            { 
                return m_voltageLevel; 
            }
            set 
            { 
                m_voltageLevel = value;
                m_voltageLevelID = value.InternalID;
            }
        }

        /// <summary>
        /// The <see cref="LinearStateEstimator.Modeling.VoltageLevel.InternalID"/> of the baseKV <see cref="LinearStateEstimator.Modeling.VoltageLevel"/>
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
        /// The parent/owner <see cref="LinearStateEstimator.Modeling.Substation"/> of the <see cref="LinearStateEstimator.Modeling.Node"/>.
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
        /// The <see cref="LinearStateEstimator.Modeling.Substation.InternalID"/> of the <see cref="LinearStateEstimator.Modeling.Node.ParentSubstation"/>.
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
        /// The parent/owner <see cref="LinearStateEstimator.Modeling.TransmissionLine"/> of the <see cref="LinearStateEstimator.Modeling.Node"/>.
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
        /// The <see cref="LinearStateEstimator.Modeling.TransmissionLine.InternalID"/> of the <see cref="LinearStateEstimator.Modeling.Node.ParentTransmissionLine"/>.
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
        /// The <see cref="LinearStateEstimator.Measurements.VoltagePhasorGroup"/> which measured and estimated voltage of the <see cref="LinearStateEstimator.Modeling.Node"/>.
        /// </summary>
        [XmlElement("Voltage")]
        public VoltagePhasorGroup Voltage
        {
            get 
            { 
                return m_voltage; 
            }
            set 
            { 
                m_voltage = value;
                m_voltagePhasorGroupID = value.InternalID;
                m_voltage.MeasuredNode = this;
            }
        }

        /// <summary>
        /// The <see cref="LinearStateEstimator.Modeling.INetworkDescribable.InternalID"/> of the <see cref="LinearStateEstimator.Modeling.Node.Voltage"/>.
        /// </summary>
        [XmlIgnore()]
        public int VoltagePhasorGroupID
        {
            get
            {
                return m_voltagePhasorGroupID;
            }
            set
            {
                m_voltagePhasorGroupID = value;
            }
        }

        /// <summary>
        /// The present status of the observability of the <see cref="LinearStateEstimator.Modeling.Node"/> defined by the <see cref="LinearStateEstimator.Modeling.ObservationState"/>.
        /// </summary>
        [XmlIgnore()]
        public ObservationState Observability
        {
            get
            {
                return m_observationState;
            }
            set
            {
                m_observationState = value;
            }
        }

        /// <summary>
        /// A flag which indicates whether the node is either <see cref="LinearStateEstimator.Modeling.ObservationState.DirectlyObserved"/> or <see cref="LinearStateEstimator.Modeling.ObservationState.IndirectlyObserved"/>.
        /// </summary>
        [XmlIgnore()]
        public bool IsObserved
        {
            get
            {
                return (Observability == ObservationState.DirectlyObserved || Observability == ObservationState.IndirectlyObserved);
            }
        }

        /// <summary>
        /// A list of the adjacent <see cref="LinearStateEstimator.Modeling.Node"/> objects in the network. Computed during observability analysis.
        /// </summary>
        [XmlIgnore()]
        public List<Node> AdjacencyList
        {
            get
            {
                return m_adjacencies;
            }
            set
            {
                m_adjacencies = value;
            }
        }

        [XmlAttribute("ObservationStateKey")]
        public string ObservationStateKey
        {
            get
            {
                return m_observationStateKey;
            }
            set
            {
                m_observationStateKey = value;
            }
        }

        [XmlAttribute("ObservedBusIdKey")]
        public string ObservedBusIdKey
        {
            get
            {
                return m_observedBusIdKey;
            }
            set
            {
                m_observedBusIdKey = value;
            }
        }

        [XmlIgnore()]
        public int ObservedBusId
        {
            get
            {
                if (m_parentSubstation.Graph != null && m_parentSubstation.Graph.ObservedBuses != null)
                {
                    ObservedBus observedBus = m_parentSubstation.Graph.ObservedBuses.Find(bus => bus.Nodes.Contains(this));
                    if (observedBus != null)
                    {
                        return observedBus.InternalID;
                    }
                }
                return 0;
            }
        }


        #endregion

        #region [ Constructors ]

        /// <summary>
        /// A blank constructor with default values.
        /// </summary>
        public Node()
            :this(DEFAULT_INTERNAL_ID, DEFAULT_NUMBER, DEFAULT_ACRONYM, DEFAULT_NAME, DEFAULT_DESCRIPTION, 0)
        {
        }

        /// <summary>
        /// A constructor for the <see cref="LinearStateEstimator.Modeling.Node"/> class which requires the <see cref="LinearStateEstimator.Modeling.VoltageLevel.InternalID"/> of the <see cref="LinearStateEstimator.Modeling.VoltageLevel"/> and the required properties defined by the <see cref="LinearStateEstimator.Modeling.INetworkDescribable"/> interface.
        /// </summary>
        /// <param name="internalID">An integer identifier for each <see cref="LinearStateEstimator.Modeling.Node"/> which is intended to be unique among other objects of the same type.</param>
        /// <param name="number">A descriptive integer for the instance of the <see cref="LinearStateEstimator.Modeling.Node"/>. There are no restrictions on uniqueness.</param>
        /// <param name="acronym">A string acronym for the instance of the <see cref="LinearStateEstimator.Modeling.Node"/>.</param>
        /// <param name="name">The string name of the instance of the <see cref="LinearStateEstimator.Modeling.Node"/>.</param>
        /// <param name="description">A string description of the instance of the <see cref="LinearStateEstimator.Modeling.Node"/>.</param>
        /// <param name="voltageLevelID">The <see cref="LinearStateEstimator.Modeling.VoltageLevel.InternalID"/> of the <see cref="LinearStateEstimator.Modeling.VoltageLevel"/> of the <see cref="LinearStateEstimator.Modeling.Node"/>.</param>
        public Node(int internalID, int number, string name, string acronym, string description, int voltageLevelID)
            :this(internalID, number, acronym, name, description, voltageLevelID, null, null)
        {
        }

        /// <summary>
        /// The designated initializer for the <see cref="LinearStateEstimator.Modeling.Node"/> class which requires the <see cref="LinearStateEstimator.Modeling.VoltageLevel.InternalID"/> of the <see cref="LinearStateEstimator.Modeling.VoltageLevel"/> and the required properties defined by the <see cref="LinearStateEstimator.Modeling.INetworkDescribable"/> interface as well as references to its parent <see cref="LinearStateEstimator.Modeling.Substation"/> and/or <see cref="LinearStateEstimator.Modeling.TransmissionLine"/>.
        /// </summary>
        /// <param name="internalID">An integer identifier for each <see cref="LinearStateEstimator.Modeling.Node"/> which is intended to be unique among other objects of the same type.</param>
        /// <param name="number">A descriptive integer for the instance of the <see cref="LinearStateEstimator.Modeling.Node"/>. There are no restrictions on uniqueness.</param>
        /// <param name="acronym">A string acronym for the instance of the <see cref="LinearStateEstimator.Modeling.Node"/>.</param>
        /// <param name="name">The string name of the instance of the <see cref="LinearStateEstimator.Modeling.Node"/>.</param>
        /// <param name="description">A string description of the instance of the <see cref="LinearStateEstimator.Modeling.Node"/>.</param>
        /// <param name="voltageLevelID">The <see cref="LinearStateEstimator.Modeling.VoltageLevel.InternalID"/> of the <see cref="LinearStateEstimator.Modeling.VoltageLevel"/> of the <see cref="LinearStateEstimator.Modeling.Node"/>.</param>
        /// <param name="parentSubstation">The parent <see cref="LinearStateEstimator.Modeling.Substation"/> of the <see cref="LinearStateEstimator.Modeling.Node"/>.</param>
        /// <param name="parentTransmissionLine">The parent <see cref="LinearStateEstimator.Modeling.TransmissionLine"/> of the <see cref="LinearStateEstimator.Modeling.Node"/>.</param>
        public Node(int internalID, int number, string acronym, string name, string description, int voltageLevelID, Substation parentSubstation, TransmissionLine parentTransmissionLine)
        {
            m_internalID = internalID;
            m_number = number;
            m_acronym = acronym;
            m_name = name;
            m_description = description;
            m_voltageLevelID = voltageLevelID;
            m_parentSubstation = parentSubstation;
            m_parentTransmissionLine = parentTransmissionLine;
            m_observationStateKey = "Undefined";
            m_observedBusIdKey = "Undefined";
        }

        #endregion

        #region [ Public Methods ]
        public bool IsCoherentWith(Node node)
        {
            if (m_parentSubstation.CoherencyDetectionMethod == VoltageCoherencyDetectionMethod.AngleDelta)
            {
                double a_cos = Math.Cos(m_voltage.PositiveSequence.Measurement.AngleInRadians);
                double a_sin = Math.Sin(m_voltage.PositiveSequence.Measurement.AngleInRadians);
                double b_cos = Math.Cos(node.Voltage.PositiveSequence.Measurement.AngleInRadians);
                double b_sin = Math.Sin(node.Voltage.PositiveSequence.Measurement.AngleInRadians);
                Complex a = new Complex(a_cos, a_sin);
                Complex b = new Complex(b_cos, b_sin);
                Complex delta = a - b;
                if ((delta.Phase * 180 / Math.PI) <= m_parentSubstation.AngleDeltaThresholdInDegrees)
                {
                    return true;
                }
            }
            else if (m_parentSubstation.CoherencyDetectionMethod == VoltageCoherencyDetectionMethod.MagnitudeDelta)
            {
                double a = m_voltage.PositiveSequence.Measurement.PerUnitMagnitude;
                double b = node.Voltage.PositiveSequence.Measurement.PerUnitMagnitude;
                double delta = a - b;
                if (delta <= m_parentSubstation.PerUnitMagnitudeDeltaThreshold)
                {
                    return true;
                }
            }
            else if (m_parentSubstation.CoherencyDetectionMethod == VoltageCoherencyDetectionMethod.TotalVectorDelta)
            {
                Complex a = m_voltage.PositiveSequence.Measurement.PerUnitComplexPhasor;
                Complex b = node.Voltage.PositiveSequence.Measurement.PerUnitComplexPhasor;
                Complex delta = a - b;
                if (delta.Magnitude <= m_parentSubstation.TotalVectorDeltaThreshold)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// A descriptive string representation of the <see cref="LinearStateEstimator.Modeling.Node"/> class instance. The format is <i>Node,internalId,number,acronym,name,description,parentSubstationInternalID,parentTransmissionLineInternalID,voltagePhasorGroupInternalID</i> and can be used for a rudimentary momento design pattern
        /// </summary>
        /// <returns>A string representation of the instance of the <see cref="LinearStateEstimator.Modeling.Node"/> class.</returns>
        public override string ToString()
        {
            return "Node," + m_internalID.ToString() + "," + m_number.ToString() + "," + m_acronym + "," + m_name + "," + m_description + "," + m_parentSubstationID.ToString() + "," + m_parentTransmissionLineID.ToString() + "," + m_voltagePhasorGroupID.ToString();
        }

        /// <summary>
        /// A verbose string representation of the instance of the <see cref="LinearStateEstimator.Modeling.Node"/> class and can be used for detailed text based output via a console or a text file.
        /// </summary>
        /// <returns>A verbose string representation of the instance of the <see cref="LinearStateEstimator.Modeling.Node"/> class.</returns>
        public string ToVerboseString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine();
            stringBuilder.AppendFormat("----- Node ---------------------------------------------------------------------");
            stringBuilder.AppendLine();
            stringBuilder.AppendFormat("      InternalID: " + InternalID.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("          Number: " + Number.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("            Name: " + Name + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("     Description: " + Description + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("          BaseKV: " + m_voltageLevel.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("ObservationState: " + m_observationState.ToString() + "{0}", Environment.NewLine);
            if (m_parentSubstation != null)
            {
                stringBuilder.AppendFormat("ParentSubstation: " + m_parentSubstation.ToString() + "{0}", Environment.NewLine);
            }
            if (m_parentTransmissionLine != null)
            {
                stringBuilder.AppendFormat("ParntTrnsmsnLine: " + m_parentTransmissionLine.ToString() + "{0}", Environment.NewLine);
            }

            stringBuilder.AppendFormat(m_voltage.ToVerboseString());
            stringBuilder.AppendLine();
            return stringBuilder.ToString();
        }

        public void Keyify()
        {
            m_observationStateKey = $"{ParentSubstation.Name}.{Name}.ObservationState";
            m_observedBusIdKey = $"{ParentSubstation.Name}.{Name}.ObservationBusId";
        }

        public void Unkeyify()
        {
            m_observationStateKey = "Undefined";
            m_observedBusIdKey = "Undefined";
        }

        #endregion
    }
}
