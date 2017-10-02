//******************************************************************************************************
//  LineSegment.cs
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

namespace SynchrophasorAnalytics.Modeling
{
    /// <summary>
    /// <see cref="LinearStateEstimator.Modeling.LineSegment"/> represents a two-port pi-model of a section of a <see cref="LinearStateEstimator.Modeling.TransmissionLine"/>.
    /// </summary>
    /// <seealso cref="LinearStateEstimator.Modeling.TransmissionLine"/>
    [Serializable()]
    public class LineSegment : SeriesBranchBase
    {
        #region [ Private Constants ]

        /// <summary>
        /// Default values
        /// </summary>
        private const int DEFAULT_INTERNAL_ID = 0;
        private const int DEFAULT_NUMBER = 0;
        private const string DEFAULT_ACRONYM = "LN";
        private const string DEFAULT_NAME = "Undefined";
        private const string DEFAULT_DESCRIPTION = "Uundefined";

        #endregion

        #region [ Private Members ]

        /// <summary>
        /// Parent
        /// </summary>
        private TransmissionLine m_parentTransmissionLine;
        private int m_parentTransmissionLineID;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// The parent <see cref="LinearStateEstimator.Modeling.TransmissionLine"/> of the <see cref="LinearStateEstimator.Modeling.LineSegment"/>. This property would return a null reference until the <see cref="LinearStateEstimator.Modeling.LineSegment"/> has been assigned to a <see cref="LinearStateEstimator.Modeling.TransmissionLine"/>.
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
                m_parentTransmissionLineID = m_parentTransmissionLine.InternalID;
            }
        }

        /// <summary>
        /// The <see cref="LinearStateEstimator.Modeling.TransmissionLine.InternalID"/> of the <see cref="LinearStateEstimator.Modeling.LineSegment.ParentTransmissionLine"/>
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

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// A blank constructor with default values.
        /// </summary>
        public LineSegment()
            :this(DEFAULT_INTERNAL_ID, DEFAULT_NUMBER, DEFAULT_NAME, DEFAULT_DESCRIPTION, new Impedance())
        {
        }

        /// <summary>
        /// A constructor for the <see cref="LinearStateEstimator.Modeling.LineSegment"/> class which requires all of the properties defined by the <see cref="LinearStateEstimator.Modeling.INetworkDescribable"/> interface and an <see cref="LinearStateEstimator.Modeling.Impedance"/>.
        /// </summary>
        /// <param name="internalID">An integer identifier for each <see cref="LinearStateEstimator.Modeling.LineSegment"/> which is intended to be unique among other objects of the same type.</param>
        /// <param name="number">A descriptive integer for the instance of the <see cref="LinearStateEstimator.Modeling.LineSegment"/>. There are no restrictions on uniqueness.</param>
        /// <param name="name">The string name of the instance of the <see cref="LinearStateEstimator.Modeling.LineSegment"/>.</param>
        /// <param name="description">A string description of the instance of the <see cref="LinearStateEstimator.Modeling.LineSegment"/>.</param>
        /// <param name="impedance">The two-port pi-model <see cref="LinearStateEstimator.Modeling.Impedance"/> for the <see cref="LinearStateEstimator.Modeling.LineSegment"/>.</param>
        public LineSegment(int internalID, int number, string name, string description, Impedance impedance)
            :this(internalID, number, name, description, impedance, 0)
        {
        }

        /// <summary>
        /// A constructor for the <see cref="LinearStateEstimator.Modeling.LineSegment"/> class which requires all of the properties defined by the <see cref="LinearStateEstimator.Modeling.INetworkDescribable"/> interface and an <see cref="LinearStateEstimator.Modeling.Impedance"/> and the <see cref="LinearStateEstimator.Modeling.TransmissionLine.InternalID"/> of the <see cref="LinearStateEstimator.Modeling.LineSegment.ParentTransmissionLine"/>.
        /// </summary>
        /// <param name="internalID">An integer identifier for each <see cref="LinearStateEstimator.Modeling.LineSegment"/> which is intended to be unique among other objects of the same type.</param>
        /// <param name="number">A descriptive integer for the instance of the <see cref="LinearStateEstimator.Modeling.LineSegment"/>. There are no restrictions on uniqueness.</param>
        /// <param name="name">The string name of the instance of the <see cref="LinearStateEstimator.Modeling.LineSegment"/>.</param>
        /// <param name="description">A string description of the instance of the <see cref="LinearStateEstimator.Modeling.LineSegment"/>.</param>
        /// <param name="impedance">The two-port pi-model <see cref="LinearStateEstimator.Modeling.Impedance"/> for the <see cref="LinearStateEstimator.Modeling.LineSegment"/>.</param>
        /// <param name="parentTransmissionLineID">The <see cref="LinearStateEstimator.Modeling.TransmissionLine.InternalID"/> of the <see cref="LinearStateEstimator.Modeling.LineSegment.ParentTransmissionLine"/>.</param>
        public LineSegment(int internalID, int number, string name, string description, Impedance impedance, int parentTransmissionLineID)
            :base(internalID, number, name, description, impedance)
        {
            this.RawImpedanceParameters.ParentElement = this;
        }

        #endregion

        #region [ Public Methods ]

        /// <summary>
        /// A string representation of the instance of the <see cref="LinearStateEstimator.Modeling.LineSegment"/> class. The format is <i>LineSegment,internalID,number,name,description,fromNodeInternalID,toNodeInternalID,r1,r2,r3,r4,r5,r6,x1,x2,x3,x4,x5,x6,b1,b2,b3,b4,b5,b6,g1,g2,g3,g4,g5,g6,parentTransmissionLineInternalId</i> and can be used for a rudimentary momento design pattern.
        /// </summary>
        /// <returns>A string representation of the instance of the <see cref="LinearStateEstimator.Modeling.LineSegment"/> class.</returns>
        public override string ToString()
        {
            return "LineSegment," + base.ToString() + "," + m_parentTransmissionLine.Number.ToString();
        }

        /// <summary>
        /// A verbose string representation of the instance of the <see cref="LinearStateEstimator.Modeling.LineSegment"/> class and can be used for detailed text based output via a console or a text file.
        /// </summary>
        /// <returns>A verbose string representation of the instance of the <see cref="LinearStateEstimator.Modeling.LineSegment"/> class.</returns>
        public new string ToVerboseString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine();
            stringBuilder.AppendFormat("----- Line Segment -------------------------------------------------------------");
            stringBuilder.AppendLine();
            stringBuilder.AppendFormat("      InternalID: " + InternalID.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("          Number: " + Number.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("            Name: " + Name + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("     Description: " + Description + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("ParntTrnsmsnLine: " + m_parentTransmissionLine.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("        FromNode: " + FromNode.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("          ToNode: " + ToNode.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat(RawImpedanceParameters.ToVerboseString());
            stringBuilder.AppendLine();
            return stringBuilder.ToString();

        }

        #endregion
        
    }
}
