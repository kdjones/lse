//******************************************************************************************************
//  SeriesBranchBase.cs
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
//  05/17/2014 - Kevin D. Jones
//       Added Math.NET for complex matrices
//  07/10/2014 - Kevin D. Jones
//       Added Guid
//
//******************************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Xml.Serialization;
using MathNet.Numerics;

using MathNet.Numerics.LinearAlgebra.Complex;

namespace SynchrophasorAnalytics.Modeling
{   
    /// <summary>
    /// Represents a series impedance branch in an electrical power system network.
    /// </summary>
    [Serializable()]
    public class SeriesBranchBase : ITwoTerminal, IImpedance, INetworkDescribable
    {
        #region [ Private Members ]

        private Guid m_uniqueId;
        private int m_internalID;
        private int m_number;
        private string m_name;
        private string m_description;
        private Impedance m_impedance;

        private Node m_fromNode;
        private int m_fromNodeID;

        private Node m_toNode;
        private int m_toNodeID;

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
        /// A unique integer identifier for the <see cref="SeriesBranchBase"/>.
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
        /// A descriptive number for the <see cref="SeriesBranchBase"/>.
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
        /// A descriptive acronym for the <see cref="SeriesBranchBase"/>. Will always return '(R + jX)'.
        /// </summary>
        [XmlIgnore()]
        public string Acronym
        {
            get 
            { 
                return "(R + jX)"; 
            }
            set 
            { 
            }
        }

        /// <summary>
        /// A descriptive name for the <see cref="SeriesBranchBase"/>.
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
        /// A description of the <see cref="SeriesBranchBase"/>.
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
        /// Gets the type of the object.
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
        /// The from <see cref="Node"/> of the <see cref="SeriesBranchBase"/>
        /// </summary>
        [XmlIgnore()]
        public Node FromNode
        {
            get
            {
                return m_fromNode;
            }
            set
            {
                m_fromNode = value;
                m_fromNodeID = value.InternalID;
            }
        }

        /// <summary>
        /// The <see cref="Node.InternalID"/> of the <see cref="FromNode"/> of the <see cref="SeriesBranchBase"/>
        /// </summary>
        [XmlAttribute("FromNode")]
        public int FromNodeID
        {
            get
            {
                return m_fromNodeID;
            }
            set
            {
                m_fromNodeID = value;
            }
        }

        /// <summary>
        /// The to <see cref="Node"/> of the <see cref="SeriesBranchBase"/>
        /// </summary>
        [XmlIgnore()]
        public Node ToNode
        {
            get
            {
                return m_toNode;
            }
            set
            {
                m_toNode = value;
                m_toNodeID = value.InternalID;
            }
        }

        /// <summary>
        /// The <see cref="Node.InternalID"/> of the <see cref="ToNode"/> of the <see cref="SeriesBranchBase"/>
        /// </summary>
        [XmlAttribute("ToNode")]
        public int ToNodeID
        {
            get
            {
                return m_toNodeID;
            }
            set
            {
                m_toNodeID = value;
            }
        }

        /// <summary>
        /// The complex (R + jX) series impedance.
        /// </summary>
        [XmlIgnore()]
        public Complex PositiveSequenceSeriesImpedance
        {
            get 
            { 
                return m_impedance.PositiveSequenceSeriesImpedance; 
            }
        }

        /// <summary>
        /// The complex 1/(R + jX) series admittance.
        /// </summary>
        [XmlIgnore()]
        public Complex PositiveSequenceSeriesAdmittance
        {
            get 
            { 
                return m_impedance.PositiveSequenceSeriesAdmittance; 
            }
        }

        /// <summary>
        /// The complex (G + jB) shunt susceptance.
        /// </summary>
        [XmlIgnore()]
        public Complex PositiveSequenceShuntSusceptance
        {
            get 
            { 
                return m_impedance.PositiveSequenceShuntSusceptance; 
            }
        }

        /// <summary>
        /// The complex (G + jB) shunt susceptance for a single side of the series branch
        /// </summary>
        [XmlIgnore()]
        public Complex PositiveSequenceSingleSidedShuntSusceptance
        {
            get
            {
                return m_impedance.PositiveSequenceSingleSidedShuntSusceptance;
            }
        }

        /// <summary>
        /// The positive sequence complex (G + jB) shunt susceptance for the <b>From</b> side of the series branch
        /// </summary>
        [XmlIgnore()]
        public Complex PositiveSequenceFromSideShuntSusceptance
        {
            get
            {
                return m_impedance.PositiveSequenceFromSideShuntSusceptance;
            }
        }

        /// <summary>
        /// The positive sequence complex (G + jB) shunt susceptance for the <b>To</b> side of the series branch
        /// </summary>
        [XmlIgnore()]
        public Complex PositiveSequenceToSideShuntSusceptance
        {
            get
            {
                return m_impedance.PositiveSequenceToSideShuntSusceptance;
            }
        }

        /// <summary>
        /// The three phase complex (R +jX) series impedance as a 3 x 3 complex matrix.
        /// </summary>
        [XmlIgnore()]
        public DenseMatrix ThreePhaseSeriesImpedance
        {
            get 
            { 
                return m_impedance.ThreePhaseSeriesImpedance; 
            }
        }

        /// <summary>
        /// The three phase complex 1/(R +jX) series admittance as a 3 x 3 complex matrix.
        /// </summary>
        [XmlIgnore()]
        public DenseMatrix ThreePhaseSeriesAdmittance
        {
            get 
            { 
                return m_impedance.ThreePhaseSeriesAdmittance; 
            }
        }

        /// <summary>
        /// The three phase complex (G + JB) shunt susceptance as a 3 x 3 complex matrix.
        /// </summary>
        [XmlIgnore()]
        public DenseMatrix ThreePhaseShuntSusceptance
        {
            get 
            { 
                return m_impedance.ThreePhaseShuntSusceptance; 
            }
        }

        /// <summary>
        /// The three phase complex (G + JB) shunt susceptance as a 3 x 3 complex matrix for a single side of the series branch (divided by 2)
        /// </summary>
        [XmlIgnore()]
        public DenseMatrix ThreePhaseSingleSidedShuntSusceptance
        {
            get
            {
                return m_impedance.ThreePhaseSingleSidedShuntSusceptance;
            }
        }

        /// <summary>
        /// The three phase complex (G + jB) shunt susceptance for the <b>From</b> side of the series branch
        /// </summary>
        [XmlIgnore()]
        public DenseMatrix ThreePhaseFromSideShuntSusceptance
        {
            get
            {
                return m_impedance.ThreePhaseToSideShuntSusceptance;
            }
        }

        /// <summary>
        /// The three phase complex (G + jB) shunt susceptance for the <b>To</b> side of the series branch
        /// </summary>
        [XmlIgnore()]
        public DenseMatrix ThreePhaseToSideShuntSusceptance
        {
            get
            {
                return m_impedance.ThreePhaseToSideShuntSusceptance;
            }
        }

        /// <summary>
        /// The raw singe value impedance parameters for resistance, reactance, and susceptance.
        /// </summary>
        [XmlElement("Impedance")]
        public Impedance RawImpedanceParameters
        {
            get 
            { 
                return m_impedance; 
            }
            set 
            { 
                m_impedance = value; 
            }
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// A blank constructor with default values.
        /// </summary>
        public SeriesBranchBase()
            :this(0, 0, "Undefined","Undefined", new Impedance())
        {
        }

        /// <summary>
        ///  A constructor for the <see cref="SeriesBranchBase"/> class. Requires properties defined in the <see cref="INetworkDescribable"/> interface and the raw <see cref="Impedance"/> values.
        /// </summary>
        /// <param name="internalID">A unique integer identifier for the <see cref="SeriesBranchBase"/>.</param>
        /// <param name="number">A descriptive number for the <see cref="SeriesBranchBase"/>.</param>
        /// <param name="name">A descriptive name for the <see cref="SeriesBranchBase"/>.</param>
        /// <param name="description">A description of the <see cref="SeriesBranchBase"/>.</param>
        /// <param name="impedance">The raw singe value impedance parameters for resistance, reactance, and susceptance.</param>
        public SeriesBranchBase(int internalID, int number, string name, string description, Impedance impedance)
            :this(internalID, number, name, description, impedance, 0, 0)
        {
        }

        /// <summary>
        /// The designated constructor for the <see cref="SeriesBranchBase"/> class. Requires properties defined in the <see cref="INetworkDescribable"/> interface, raw <see cref="Impedance"/> values and the <see cref="Node.InternalID"/> of the <see cref="FromNode"/> and <see cref="ToNode"/>.
        /// </summary>
        /// <param name="internalID">A unique integer identifier for the <see cref="SeriesBranchBase"/>.</param>
        /// <param name="number">A descriptive number for the <see cref="SeriesBranchBase"/>.</param>
        /// <param name="name">A descriptive name for the <see cref="SeriesBranchBase"/>.</param>
        /// <param name="description">A description of the <see cref="SeriesBranchBase"/>.</param>
        /// <param name="impedance">The raw singe value impedance parameters for resistance, reactance, and susceptance.</param>
        /// <param name="fromNodeID">The <see cref="Node.InternalID"/> of the <see cref="FromNode"/> of the <see cref="SeriesBranchBase"/></param>
        /// <param name="toNodeID">The <see cref="Node.InternalID"/> of the <see cref="ToNode"/> of the <see cref="SeriesBranchBase"/></param>
        public SeriesBranchBase(int internalID, int number, string name, string description, Impedance impedance, int fromNodeID, int toNodeID)
        {
            m_internalID = internalID;
            m_number = number;
            m_name = name;
            m_description = description;
            m_impedance = impedance;
            m_fromNodeID = fromNodeID;
            m_toNodeID = toNodeID;
            this.RawImpedanceParameters.ParentElement = this;
        }

        #endregion

        #region [ Public Methods ]

        /// <summary>
        /// A string representation of the instance of the <see cref="SeriesBranchBase"/> class.
        /// </summary>
        /// <returns>A string representation of the instance of the <see cref="SeriesBranchBase"/> class.</returns>
        public override string ToString()
        {
            return m_internalID.ToString() + "," + m_number.ToString() + "," + m_name + "," + m_description + "," + m_fromNodeID.ToString() + "," + m_toNodeID.ToString() + "," + m_impedance.ToString();
        }

        /// <summary>
        /// A verbose string representation of the instance of the <see cref="SeriesBranchBase"/> class.
        /// </summary>
        /// <returns>A verbose string representation of the instance of the <see cref="SeriesBranchBase"/> class.</returns>
        public string ToVerboseString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine();
            stringBuilder.AppendFormat("----- Series Branch Base -------------------------------------------------------");
            stringBuilder.AppendLine();
            stringBuilder.AppendFormat("      InternalID: " + InternalID.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("          Number: " + Number.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("            Name: " + Name + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("     Description: " + Description + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("        FromNode: " + FromNode.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("          ToNode: " + ToNode.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat(m_impedance.ToVerboseString());
            stringBuilder.AppendLine();
            return stringBuilder.ToString();

        }

        /// <summary>
        /// Determines equality between to <see cref="SeriesBranchBase"/> objects
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
            SeriesBranchBase seriesBranchBase = target as SeriesBranchBase;

            if ((object)seriesBranchBase == null)
            {
                return false;
            }

            // Return true if the fields match:
            if (this.m_internalID != seriesBranchBase.InternalID)
            {
                return false;
            }
            else if (this.m_number != seriesBranchBase.Number)
            {
                return false;
            }
            else if (this.m_name != seriesBranchBase.Name)
            {
                return false;
            }
            else if (this.m_description != seriesBranchBase.Description)
            {
                return false;
            }
            else if (!this.m_impedance.Equals(seriesBranchBase.RawImpedanceParameters))
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
        /// <returns>An integer hash code from the base class.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Performs a deep copy of the target <see cref="SeriesBranchBase"/> object.
        /// </summary>
        /// <returns>A deep copy of the target <see cref="SeriesBranchBase"/> object.</returns>
        public SeriesBranchBase Copy()
        {
            SeriesBranchBase copy = (SeriesBranchBase)this.MemberwiseClone();
            copy.RawImpedanceParameters = this.m_impedance.Copy();
            return copy;
        }

        #endregion

    }
}
