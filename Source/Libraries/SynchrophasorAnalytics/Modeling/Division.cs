//******************************************************************************************************
//  Division.cs
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
    /// Represents a collection of electric power network elements with regional significance. Gives heirarchy to the network structure to make modeling easier. A <see cref="LinearStateEstimator.Modeling.Division"/> is a child of <see cref="LinearStateEstimator.Modeling.Company"/>.
    /// </summary>
    [Serializable()]
    public class Division : INetworkDescribable, IPrunable
    {
        #region [ Private Constants ]

        /// <summary>
        /// Default values
        /// </summary>
        private const int DEFAULT_INTERNAL_ID = 0;
        private const int DEFAULT_NUMBER = 0;
        private const string DEFAULT_ACRONYM = "DV";
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
        /// Parent
        /// </summary>
        private Company m_parentCompany;

        /// <summary>
        /// Children
        /// </summary>
        private List<Substation> m_childrenSubstations;
        private List<TransmissionLine> m_childrenTransmissionLines;

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
        /// An integer identifier for each <see cref="LinearStateEstimator.Modeling.Division"/> which is intended to be unique among other objects of the same type.
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
        /// A descriptive integer for the instance of the <see cref="LinearStateEstimator.Modeling.Division"/>. There are no restrictions on uniqueness. 
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
        /// A string acronym for the instance of the <see cref="LinearStateEstimator.Modeling.Division"/>.
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
        /// The string name of the instance of the <see cref="LinearStateEstimator.Modeling.Division"/>.
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
        /// A string description of the instance of the <see cref="LinearStateEstimator.Modeling.Division"/>.
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
        /// Returns the type of the object as a string
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
        /// The parent <see cref="LinearStateEstimator.Modeling.Company"/> of the <see cref="LinearStateEstimator.Modeling.Division"/>. This is a null reference until the <see cref="LinearStateEstimator.Modeling.Division"/> is given to a <see cref="LinearStateEstimator.Modeling.Company"/>.
        /// </summary>
        [XmlIgnore()]
        public Company ParentCompany
        {
            get 
            { 
                return m_parentCompany; 
            }
            set 
            { 
                m_parentCompany = value; 
            }
        }

        /// <summary>
        /// All of the <see cref="LinearStateEstimator.Modeling.Substation"/> objects owned by the <see cref="LinearStateEstimator.Modeling.Division"/>. Will return an empty list if the underlying field has not been set by the user.
        /// </summary>
        [XmlArray("Substations")]
        public List<Substation> Substations
        {
            get 
            { 
                return m_childrenSubstations; 
            }
            set 
            { 
                m_childrenSubstations = value;
                foreach (Substation substation in m_childrenSubstations)
                {
                    substation.ParentDivision = this;
                }
            }
        }

        /// <summary>
        /// All of the <see cref="LinearStateEstimator.Modeling.TransmissionLine"/> objects owned by the <see cref="LinearStateEstimator.Modeling.Division"/>. Will return an empty list if the underlying field has not been set by the user.
        /// </summary>
        [XmlArray("TransmissionLines")]
        public List<TransmissionLine> TransmissionLines
        {
            get 
            { 
                return m_childrenTransmissionLines; 
            }
            set 
            { 
                m_childrenTransmissionLines = value;
                foreach (TransmissionLine transmissionLine in m_childrenTransmissionLines)
                {
                    transmissionLine.ParentDivision = this;
                }
            }
        }

        [XmlIgnore()]
        public bool InPruningMode
        {
            get
            {
                return m_parentCompany.ParentModel.InPruningMode;
            }
        }

        [XmlIgnore()]
        public bool RetainWhenPruning
        {
            get
            {
                if (InPruningMode)
                {
                    foreach (Substation substation in m_childrenSubstations)
                    {
                        if (substation.RetainWhenPruning)
                        {
                            return true;
                        }
                    }
                    return false;
                }
                return true;
            }
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// A blank constructor with default values.
        /// </summary>
        public Division()
            :this(DEFAULT_INTERNAL_ID, DEFAULT_NUMBER, DEFAULT_ACRONYM, DEFAULT_NAME, DEFAULT_DESCRIPTION)
        {
        }

        /// <summary>
        /// A constructor for the <see cref="LinearStateEstimator.Modeling.Division"/> class which requires all of the properties defined by the <see cref="LinearStateEstimator.Modeling.INetworkDescribable"/> interface.
        /// </summary>
        /// <param name="internalID">An integer identifier for each <see cref="LinearStateEstimator.Modeling.Division"/> which is intended to be unique among other objects of the same type.</param>
        /// <param name="number">A descriptive integer for the instance of the <see cref="LinearStateEstimator.Modeling.Division"/>. There are no restrictions on uniqueness.</param>
        /// <param name="acronym">A string acronym for the instance of the <see cref="LinearStateEstimator.Modeling.Division"/>.</param>
        /// <param name="name">The string name of the instance of the <see cref="LinearStateEstimator.Modeling.Division"/>.</param>
        /// <param name="description">A string description of the instance of the <see cref="LinearStateEstimator.Modeling.Division"/>.</param>
        public Division(int internalID, int number, string acronym, string name, string description)
            :this(internalID, number, acronym, name, description, new List<Substation>(), new List<TransmissionLine>())
        {
        }

        /// <summary>
        /// The designated initializer for the <see cref="LinearStateEstimator.Modeling.Division"/> class. Requires all of the properties defined by the <see cref="LinearStateEstimator.Modeling.INetworkDescribable"/> interface and a list of its children <see cref="LinearStateEstimator.Modeling.Substation"/> and <see cref="LinearStateEstimator.Modeling.TransmissionLine"/> objects.
        /// </summary>
        /// <param name="internalID">An integer identifier for each <see cref="LinearStateEstimator.Modeling.Division"/> which is intended to be unique among other objects of the same type.</param>
        /// <param name="number">A descriptive integer for the instance of the <see cref="LinearStateEstimator.Modeling.Division"/>. There are no restrictions on uniqueness.</param>
        /// <param name="acronym">A string acronym for the instance of the <see cref="LinearStateEstimator.Modeling.Division"/>.</param>
        /// <param name="name">The string name of the instance of the <see cref="LinearStateEstimator.Modeling.Division"/>.</param>
        /// <param name="description">A string description of the instance of the <see cref="LinearStateEstimator.Modeling.Division"/>.</param>
        /// <param name="substations">A list of the children <see cref="LinearStateEstimator.Modeling.Substation"/> objects of the <see cref="LinearStateEstimator.Modeling.Division"/>.</param>
        /// <param name="transmissionLines">A list of the children <see cref="LinearStateEstimator.Modeling.TransmissionLine"/> objects of the <see cref="LinearStateEstimator.Modeling.Division"/>.</param>
        public Division(int internalID, int number, string acronym, string name, string description, List<Substation> substations, List<TransmissionLine> transmissionLines)
        {
            m_internalID = internalID;
            m_number = number;
            m_acronym = acronym;
            m_name = name;
            m_description = description;

            m_childrenSubstations = substations;
            foreach (Substation substation in m_childrenSubstations)
            {
                substation.ParentDivision = this;
            }

            m_childrenTransmissionLines = transmissionLines;
            foreach (TransmissionLine transmissionLine in m_childrenTransmissionLines)
            {
                transmissionLine.ParentDivision = this;
            }
        }

        #endregion

        #region [ Public Methods ]

        /// <summary>
        /// A descriptive string representation of the <see cref="LinearStateEstimator.Modeling.Division"/> class instance. The format is <i>Division,internalId,number,acronym,name,description</i> and can be used for a rudimentary momento design pattern.
        /// </summary>
        /// <returns>A string representation of the instance of the <see cref="LinearStateEstimator.Modeling.Division"/> class.</returns>
        public override string ToString()
        {
            return "Division," + m_internalID.ToString() + "," + m_number.ToString() + "," + m_acronym + "," + m_name + "," + m_description + "," + m_parentCompany.InternalID.ToString();
        }

        /// <summary>
        /// A verbose string representation of the instance of the <see cref="LinearStateEstimator.Modeling.Division"/> class and can be used for detailed text based output via a console or a text file.
        /// </summary>
        /// <returns>A verbose string representation of the instance of the <see cref="LinearStateEstimator.Modeling.Division"/> class.</returns>
        public string ToVerboseString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine();
            stringBuilder.AppendFormat("----- Division -----------------------------------------------------------------");
            stringBuilder.AppendLine();
            stringBuilder.AppendFormat("      InternalID: " + m_internalID.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("          Number: " + m_number.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("         Acronym: " + m_acronym + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("            Name: " + m_name + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("     Description: " + m_description + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("   ParentCompany: " + m_parentCompany.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("     Substations:  {0}", Environment.NewLine);
            foreach (Substation substation in m_childrenSubstations)
            {
                stringBuilder.AppendFormat("        " + substation.ToString() + "{0}", Environment.NewLine);
            }
            stringBuilder.AppendFormat("TrnsmissionLines:  {0}", Environment.NewLine);
            foreach (TransmissionLine transmissionLine in m_childrenTransmissionLines)
            {
                stringBuilder.AppendFormat("        " + transmissionLine.ToString() + "{0}", Environment.NewLine);
            }
            stringBuilder.AppendLine();
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Determines equality between to <see cref="LinearStateEstimator.Modeling.Division"/> objects. Child <see cref="LinearStateEstimator.Modeling.Substation"/> and <see cref="LinearStateEstimator.Modeling.TransmissionLine"/> objects
        /// are not included in the equality check.
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

            // If parameter cannot be cast to Division return false.
            Division division = target as Division;

            if ((object)division == null)
            {
                return false;
            }

            // Return true if the fields match:
            if (this.m_internalID != division.InternalID)
            {
                return false;
            }
            else if (this.m_number != division.Number)
            {
                return false;
            }
            else if (!this.m_acronym.Equals(division.Acronym))
            {
                return false;
            }
            else if (!this.m_name.Equals(division.Name))
            {
                return false;
            }
            else if (!this.m_description.Equals(division.Description))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// OVerridden to prevent compilation warnings.
        /// </summary>
        /// <returns>An integer hash code determined by the base class.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Performs a deep copy of the target <see cref="LinearStateEstimator.Modeling.Division"/> object.
        /// </summary>
        /// <returns>A deep copy of the target <see cref="LinearStateEstimator.Modeling.Division"/> object.</returns>
        public Division DeepCopy()
        {
            Division copy = (Division)this.MemberwiseClone();

            List<Substation> copySubstations = new List<Substation>();
            List<TransmissionLine> copyTransmissionLines = new List<TransmissionLine>();

            foreach (Substation substation in m_childrenSubstations)
            {
                copySubstations.Add(substation.DeepCopy());
            }

            foreach (Substation substation in copySubstations)
            {
                substation.ParentDivision = copy;
            }

            foreach (TransmissionLine transmissionLine in m_childrenTransmissionLines)
            {
                copyTransmissionLines.Add(transmissionLine.Copy());
            }

            foreach (TransmissionLine transmissionLine in copyTransmissionLines)
            {
                transmissionLine.ParentDivision = copy;
            }

            return copy;
        }

        #endregion


    }
}
