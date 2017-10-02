//******************************************************************************************************
//  Substation.cs
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
//  01/22/2014 - Kevin D. Jones
//       Added 'Devices' Property for mapping input measurements function.
//  05/09/2014 - Kevin D. Jones
//       Added Shunts to children network elements.
//  06/09/2014 - Kevin D. Jones
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
using SynchrophasorAnalytics.Modeling;
using SynchrophasorAnalytics.Graphs;
using SynchrophasorAnalytics.Measurements;

namespace SynchrophasorAnalytics.Modeling
{
    /// <summary>
    /// A <see cref="LinearStateEstimator.Modeling.Substation"/> represents a collection of network elements which usually are enclosed within the same switchyard. This includes <see cref="LinearStateEstimator.Modeling.Node"/>, <see cref="LinearStateEstimator.Modeling.Transformer"/>, <see cref="LinearStateEstimator.Modeling.CircuitBreaker"/>, <see cref="LinearStateEstimator.Modeling.Switch"/>, and/or <see cref="LinearStateEstimator.Modeling.ShuntCompensator"/>
    /// </summary>
    [Serializable()]
    public class Substation : INetworkDescribable, IPrunable
    {
        #region [ Private Constants ]

        /// <summary>
        /// Default values
        /// </summary>
        private const int DEFAULT_INTERNAL_ID = 0;
        private const int DEFAULT_NUMBER = 0;
        private const string DEFAULT_ACRONYM = "SUB";
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
        private string m_observedBusCountKey;

        /// <summary>
        /// Parent
        /// </summary>
        private Division m_parentDivision;

        /// <summary>
        /// Children
        /// </summary>
        private List<Node> m_childrenNodes;
        private List<ShuntCompensator> m_childrenShunts;
        private List<Transformer> m_childrenTransformers;
        private List<CircuitBreaker> m_childrenCircuitBreakers;
        private List<Switch> m_childrenSwitches;

        private SubstationGraph m_graph;
        private List<VoltageLevelGroup> m_voltageLevelGroups;
        private double m_angleDeltaThresholdInDegrees;
        private double m_perUnitMagnitudeThreshold;
        private double m_totalVectorDeltaThreshold;
        private VoltageCoherencyDetectionMethod m_coherencyDetectionMethod;
        private TopologyEstimationLevel m_topologyLevel;

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
        /// An integer identifier for each <see cref="LinearStateEstimator.Modeling.Substation"/> which is intended to be unique among other objects of the same type.
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
        /// A descriptive integer for the instance of the <see cref="LinearStateEstimator.Modeling.Substation"/>. There are no restrictions on uniqueness. 
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
        /// A string acronym for the instance of the <see cref="LinearStateEstimator.Modeling.Substation"/>.
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
        /// The string name of the instance of the <see cref="LinearStateEstimator.Modeling.Substation"/>.
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
        /// A string description of the instance of the <see cref="LinearStateEstimator.Modeling.Substation"/>.
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

        [XmlAttribute("ObservedBusCountKey")]
        public string ObservedBusCountKey
        {
            get
            {
                return m_observedBusCountKey;
            }
            set
            {
                m_observedBusCountKey = value;
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
        /// The parent <see cref="LinearStateEstimator.Modeling.Division"/> of the <see cref="LinearStateEstimator.Modeling.Substation"/>.
        /// </summary>
        [XmlIgnore()]
        public Division ParentDivision
        {
            get 
            { 
                return m_parentDivision; 
            }
            set 
            {
                m_parentDivision = value; 
            }
        }

        /// <summary>
        /// All of the children <see cref="LinearStateEstimator.Modeling.Node"/> objects of the <see cref="LinearStateEstimator.Modeling.Substation"/>.
        /// </summary>
        [XmlArray("Nodes")]
        public List<Node> Nodes
        {
            get 
            { 
                return m_childrenNodes; 
            }
            set 
            { 
                m_childrenNodes = value;
                foreach (Node node in m_childrenNodes)
                {
                    node.ParentSubstation = this;
                }
            }
        }

        /// <summary>
        /// All of the children <see cref="LinearStateEstimator.Modeling.ShuntCompensator"/> objects of the <see cref="LinearStateEstimator.Modeling.Substation"/>.
        /// </summary>
        [XmlArray("Shunts")]
        public List<ShuntCompensator> Shunts
        {
            get
            {
                return m_childrenShunts;
            }
            set
            {
                m_childrenShunts = value;
                foreach (ShuntCompensator shunt in m_childrenShunts)
                {
                    shunt.ParentSubstation = this;
                }
            }
        }

        /// <summary>
        /// All of the children <see cref="LinearStateEstimator.Modeling.Transformer"/> objects of the <see cref="LinearStateEstimator.Modeling.Substation"/>.
        /// </summary>
        [XmlArray("Transformers")]
        public List<Transformer> Transformers
        {
            get 
            { 
                return m_childrenTransformers; 
            }
            set 
            { 
                m_childrenTransformers = value;
                foreach (Transformer transformer in m_childrenTransformers)
                {
                    transformer.ParentSubstation = this;
                }
            }
        }

        /// <summary>
        /// All of the children <see cref="LinearStateEstimator.Modeling.CircuitBreaker"/> objects of the <see cref="LinearStateEstimator.Modeling.Substation"/>.
        /// </summary>
        [XmlArray("CircuitBreakers")]
        public List<CircuitBreaker> CircuitBreakers
        {
            get 
            { 
                return m_childrenCircuitBreakers; 
            }
            set 
            { 
                m_childrenCircuitBreakers = value;
                foreach (CircuitBreaker circuitBreaker in m_childrenCircuitBreakers)
                {
                    circuitBreaker.ParentSubstation = this;
                }
            }
        }

        /// <summary>
        /// All of the children <see cref="LinearStateEstimator.Modeling.Switch"/> objects of the <see cref="LinearStateEstimator.Modeling.Substation"/>.
        /// </summary>
        [XmlArray("Switches")]
        public List<Switch> Switches
        {
            get 
            { 
                return m_childrenSwitches; 
            }
            set 
            { 
                m_childrenSwitches = value;
                foreach (Switch switchingDevice in m_childrenSwitches)
                {
                    switchingDevice.ParentSubstation = this;
                }
            }
        }

        /// <summary>
        /// The graph representation of the nodal structure of the <see cref="LinearStateEstimator.Modeling.Substation"/>
        /// </summary>
        [XmlIgnore()]
        public SubstationGraph Graph
        {
            get 
            { 
                return m_graph; 
            }
            set 
            { 
                m_graph = value; 
            }
        }

        [XmlIgnore()]
        public bool InPruningMode
        {
            get
            {
                return m_parentDivision.ParentCompany.ParentModel.InPruningMode;
            }
        }

        [XmlIgnore()]
        public bool RetainWhenPruning
        {
            get
            {
                if (InPruningMode)
                {
                    foreach (Node node in m_childrenNodes)
                    {
                        if (node.Observability == ObservationState.DirectlyObserved)
                        {
                            return true;
                        }
                        else if (node.Observability == ObservationState.IndirectlyObserved)
                        {
                            return true;
                        }
                    }
                    return false;
                }
                return true;
            }
        }

        [XmlIgnore()]
        public int ObservedBusCount
        {
            get
            {
                if (m_graph != null && m_graph.ObservedBuses != null)
                {
                    return m_graph.ObservedBuses.Count;
                }
                return 0;
            }
        }

        [XmlIgnore()]
        public List<VoltageLevelGroup> VoltageLevelGroups
        {
            get
            {
                return m_voltageLevelGroups;
            }
            set
            {
                m_voltageLevelGroups = value;
            }
        }

        [XmlAttribute("CoherencyMethod")]
        public VoltageCoherencyDetectionMethod CoherencyDetectionMethod
        {
            get
            {
                return m_coherencyDetectionMethod;
            }
            set
            {
                m_coherencyDetectionMethod = value;
            }
        }

        [XmlAttribute("TopologyEstimationLevel")]
        public TopologyEstimationLevel TopologyLevel
        {
            get
            {
                return m_topologyLevel;
            }
            set
            {
                m_topologyLevel = value;
            }
        }

        [XmlAttribute("AngleDeltaThresholdInDegrees")]
        public double AngleDeltaThresholdInDegrees
        {
            get
            {
                return m_angleDeltaThresholdInDegrees;
            }
            set
            {
                m_angleDeltaThresholdInDegrees = value;
            }
        }

        [XmlAttribute("PerUnitMagnitudeDeltaThreshold")]
        public double PerUnitMagnitudeDeltaThreshold
        {
            get
            {
                return m_perUnitMagnitudeThreshold;
            }
            set
            {
                m_perUnitMagnitudeThreshold = value;
            }
        }

        [XmlAttribute("TotalVectorDeltaThreshold")]
        public double TotalVectorDeltaThreshold
        {
            get
            {
                return m_totalVectorDeltaThreshold;
            }
            set
            {
                m_totalVectorDeltaThreshold = value;
            }
        }



        #endregion

        #region [ Constructors ]

        /// <summary>
        /// A blank constructor with default values.
        /// </summary>
        public Substation()
            :this (DEFAULT_INTERNAL_ID, DEFAULT_NUMBER, DEFAULT_ACRONYM, DEFAULT_NAME, DEFAULT_DESCRIPTION)
        {
        }

        /// <summary>
        /// A constructor for the <see cref="LinearStateEstimator.Modeling.Substation"/> class which requires all of the properties defined by the <see cref="LinearStateEstimator.Modeling.INetworkDescribable"/> interface.
        /// </summary>
        /// <param name="internalID">An integer identifier for each <see cref="LinearStateEstimator.Modeling.Substation"/> which is intended to be unique among other objects of the same type.</param>
        /// <param name="number">A descriptive integer for the instance of the <see cref="LinearStateEstimator.Modeling.Substation"/>. There are no restrictions on uniqueness. </param>
        /// <param name="acronym">A string acronym for the instance of the <see cref="LinearStateEstimator.Modeling.Substation"/>.</param>
        /// <param name="name">The string name of the instance of the <see cref="LinearStateEstimator.Modeling.Substation"/>.</param>
        /// <param name="description">A string description of the instance of the <see cref="LinearStateEstimator.Modeling.Substation"/>.</param>
        public Substation(int internalID, int number, string acronym, string name, string description)
            :this(internalID, number, acronym, name, description, new List<Node>(), new List<ShuntCompensator>(), new List<Transformer>(), new List<CircuitBreaker>(), new List<Switch>())
        {
        }

        /// <summary>
        /// A constructor for the <see cref="LinearStateEstimator.Modeling.Substation"/> class which requires all of the properties defined by the <see cref="LinearStateEstimator.Modeling.INetworkDescribable"/> interface and all of its children network elements.
        /// </summary>
        /// <param name="internalID">An integer identifier for each <see cref="LinearStateEstimator.Modeling.Substation"/> which is intended to be unique among other objects of the same type.</param>
        /// <param name="number">A descriptive integer for the instance of the <see cref="LinearStateEstimator.Modeling.Substation"/>. There are no restrictions on uniqueness. </param>
        /// <param name="acronym">A string acronym for the instance of the <see cref="LinearStateEstimator.Modeling.Substation"/>.</param>
        /// <param name="name">The string name of the instance of the <see cref="LinearStateEstimator.Modeling.Substation"/>.</param>
        /// <param name="description">A string description of the instance of the <see cref="LinearStateEstimator.Modeling.Substation"/>.</param>
        /// <param name="nodes">All of the children <see cref="LinearStateEstimator.Modeling.Node"/> objects of the <see cref="LinearStateEstimator.Modeling.Substation"/>.</param>
        /// <param name="shunts">All of the children <see cref="LinearStateEstimator.Modeling.ShuntCompensator"/> objects of the <see cref="LinearStateEstimator.Modeling.Substation"/>.</param>
        /// <param name="transformers">All of the children <see cref="LinearStateEstimator.Modeling.Transformer"/> objects of the <see cref="LinearStateEstimator.Modeling.Substation"/>.</param>
        /// <param name="circuitBreakers">All of the children <see cref="LinearStateEstimator.Modeling.CircuitBreaker"/> objects of the <see cref="LinearStateEstimator.Modeling.Substation"/>.</param>
        /// <param name="switches">All of the children <see cref="LinearStateEstimator.Modeling.Switch"/> objects of the <see cref="LinearStateEstimator.Modeling.Substation"/>.</param>
        public Substation(int internalID, int number, string acronym, string name, string description, List<Node> nodes, List<ShuntCompensator> shunts, List<Transformer> transformers, List<CircuitBreaker> circuitBreakers, List<Switch> switches)
        {
            m_internalID = internalID;
            m_number = number;
            m_acronym = acronym;
            m_name = name;
            m_description = description;
            m_childrenNodes = nodes;
            m_childrenShunts = shunts;
            m_childrenTransformers = transformers;
            m_childrenCircuitBreakers = circuitBreakers;
            m_childrenSwitches = switches;
            m_observedBusCountKey = "Undefined";
            m_voltageLevelGroups = new List<VoltageLevelGroup>();
            m_perUnitMagnitudeThreshold = 0.03;
            m_angleDeltaThresholdInDegrees = 0.2;
            m_totalVectorDeltaThreshold = 0.03;
            m_coherencyDetectionMethod = VoltageCoherencyDetectionMethod.AngleDelta;
            m_topologyLevel = TopologyEstimationLevel.Zero;
        }

        #endregion

        #region [ Public Methods ]

        /// <summary>
        /// Initializes the <see cref="LinearStateEstimator.Modeling.Substation.Graph"/>.
        /// </summary>
        public void InitializeSubstationGraph()
        {
            m_graph = new SubstationGraph(this);
        }

        public void InitializeVoltageLevelGroups()
        {
            List<VoltageLevel> baseKvs = new List<VoltageLevel>();

            foreach (Node node in m_childrenNodes)
            {
                if (!baseKvs.Contains(node.BaseKV))
                {
                    baseKvs.Add(node.BaseKV);
                }
            }

            foreach (VoltageLevel baseKv in baseKvs)
            {
                m_voltageLevelGroups.Add(new VoltageLevelGroup(baseKv));
            }

            foreach (Node node in m_childrenNodes)
            {
                VoltageLevelGroup voltageLevelGroup = m_voltageLevelGroups.Find(group => group.BaseKV.InternalID == node.BaseKV.InternalID);
                voltageLevelGroup.Nodes.Add(node);
            }
        }

        /// <summary>
        /// A descriptive string representation of the <see cref="LinearStateEstimator.Modeling.Substation"/> class instance. The format is <i>Substation,internalId,number,acronym,name,description,parentDivisionInternalID</i> and can be used for a rudimentary momento design pattern.
        /// </summary>
        /// <returns>A string representation of an instance of the <see cref="LinearStateEstimator.Modeling.Substation"/> class.</returns>
        public override string ToString()
        {
            return "Substation," + m_internalID.ToString() + "," + m_number.ToString() + "," + m_acronym + "," + m_name + "," + m_description + "," + m_parentDivision.InternalID.ToString();
        }

        /// <summary>
        /// A verbose string representation of an instance of the <see cref="LinearStateEstimator.Modeling.Substation"/> class  and can be used for detailed text based output via a console or a text file.
        /// </summary>
        /// <returns>A verbose string representation of an instance of the <see cref="LinearStateEstimator.Modeling.Substation"/> class.</returns>
        public string ToVerboseString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine();
            stringBuilder.AppendFormat("----- Substation ---------------------------------------------------------------");
            stringBuilder.AppendLine();
            stringBuilder.AppendFormat("      InternalID: " + m_internalID.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("          Number: " + m_number.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("         Acronym: " + m_acronym + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("            Name: " + m_name + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("     Description: " + m_description + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("  ParentDivision: " + m_parentDivision.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("           Nodes:  {0}", Environment.NewLine);
            foreach (Node node in m_childrenNodes)
            {
                stringBuilder.AppendFormat(node.ToString() + "{0}", Environment.NewLine);
            }
            stringBuilder.AppendLine();
            stringBuilder.AppendFormat("    Transformers:  {0}", Environment.NewLine);
            foreach (Transformer transformer in m_childrenTransformers)
            {
                stringBuilder.AppendFormat(transformer.ToString() + "{0}", Environment.NewLine);
            }
            stringBuilder.AppendLine();
            stringBuilder.AppendFormat(" CircuitBreakers:  {0}", Environment.NewLine);
            foreach (CircuitBreaker circuitBreaker in m_childrenCircuitBreakers)
            {
                stringBuilder.AppendFormat(circuitBreaker.ToString() + "{0}", Environment.NewLine);
            }
            stringBuilder.AppendLine();
            stringBuilder.AppendFormat("        Switches:  {0}", Environment.NewLine);
            foreach (Switch circuitSwitch in m_childrenSwitches)
            {
                stringBuilder.AppendFormat(circuitSwitch.ToString() + "{0}", Environment.NewLine);
            }
            stringBuilder.AppendLine();
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Determines equality between to <see cref="LinearStateEstimator.Modeling.Substation"/> objects. Children objects are not included in the equality check.
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
            Substation substation = target as Substation;

            if ((object)substation == null)
            {
                return false;
            }

            // Return true if the fields match:
            if (this.m_internalID != substation.InternalID)
            {
                return false;
            }
            else if (this.m_number != substation.Number)
            {
                return false;
            }
            else if (!this.m_acronym.Equals(substation.Acronym))
            {
                return false;
            }
            else if (this.m_name != substation.Name)
            {
                return false;
            }
            else if (this.m_description != substation.Description)
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
        /// Performs a deep copy of the target <see cref="LinearStateEstimator.Modeling.Substation"/> object.
        /// </summary>
        /// <returns>A deep copy of the target <see cref="LinearStateEstimator.Modeling.Substation"/> object.</returns>
        public Substation DeepCopy()
        {
            // Incomplete
            Substation copy = (Substation)this.MemberwiseClone();

            List<Node> copyNodes = new List<Node>();
            List<Transformer> copyTransformers = new List<Transformer>();
            List<CircuitBreaker> copyCircuitBreakers = new List<CircuitBreaker>();

            foreach (Node node in m_childrenNodes)
            {
                //copyNodes.Add(node.Copy());
            }

            foreach (Transformer transformer in m_childrenTransformers)
            {
                //copyTransformers.Add(transformer.Copy());
            }

            foreach (CircuitBreaker circuitBreaker in m_childrenCircuitBreakers)
            {
                //copyCircuitBreakers.Add(circuitBreaker.Copy());
            }

            copy.Nodes = copyNodes;
            copy.Transformers = copyTransformers;
            copy.CircuitBreakers = copyCircuitBreakers;

            return copy; // Incomplete
        }

        public void Keyify()
        {
            m_observedBusCountKey = $"{Name}.ObservedBusCount";
        }

        public void UnKeyify()
        {
            m_observedBusCountKey = "Undefined";
        }
        #endregion
    }
}
