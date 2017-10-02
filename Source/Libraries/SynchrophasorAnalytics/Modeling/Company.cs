//******************************************************************************************************
//  Company.cs
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
    /// Represents a collection of electric power network elements with shared ownership. Gives heirarchy to the network structure to make modeling easier.
    /// </summary>
    [Serializable()]
    public class Company : INetworkDescribable
    {
        #region [ Private Constants ]

        /// <summary>
        /// Default values
        /// </summary>
        private const int DEFAULT_INTERNAL_ID = 0;
        private const int DEFAULT_NUMBER = 0;
        private const string DEFAULT_ACRONYM = "CO";
        private const string DEFAULT_NAME = "Undefined";
        private const string DEFAULT_DESCRIPTION = "Uundefined";

        #endregion

        #region [ Private Members ]

        /// <summary>
        /// INetworkDescribable fields
        /// </summary>
        private Guid m_uniqueId;
        private int m_internalId;
        private int m_number;
        private string m_acronym;
        private string m_name;
        private string m_description;

        /// <summary>
        /// Children
        /// </summary>
        private List<Division> m_childrenDivisions;

        /// <summary>
        /// Parent
        /// </summary>
        private NetworkModel m_parentModel;

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
        /// An integer identifier for each <see cref="LinearStateEstimator.Modeling.Company"/> which is intended to be unique among other objects of the same type.
        /// </summary>
        [XmlAttribute("ID")]
        public int InternalID
        {
            get 
            { 
                return m_internalId; 
            }
            set 
            { 
                m_internalId = value; 
            }
        }

        /// <summary>
        /// A descriptive integer for the instance of the <see cref="LinearStateEstimator.Modeling.Company"/>. There are no restrictions on uniqueness. 
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
        /// A string acronym for the instance of the <see cref="LinearStateEstimator.Modeling.Company"/>.
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
        /// The string name of the instance of the <see cref="LinearStateEstimator.Modeling.Company"/>.
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
        /// A string description of the instance of the <see cref="LinearStateEstimator.Modeling.Company"/>.
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
        /// All of the children <see cref="LinearStateEstimator.Modeling.Division"/> objects owned by the <see cref="LinearStateEstimator.Modeling.Company"/>. Will return an empty list if the underlying field has not been set by the user.
        /// </summary>
        [XmlArray("Divisions")]
        public List<Division> Divisions
        {
            get 
            { 
                return m_childrenDivisions; 
            }
            set 
            { 
                m_childrenDivisions = value;
                foreach (Division division in m_childrenDivisions)
                {
                    division.ParentCompany = this;
                }
            }
        }

        /// <summary>
        /// A reference to the <see cref="LinearStateEstimator.Modeling.NetworkModel"/> which owns the instance of <see cref="LinearStateEstimator.Modeling.Company"/>. This is a null reference until the <see cref="LinearStateEstimator.Modeling.Company"/> is given to a <see cref="LinearStateEstimator.Modeling.NetworkModel"/>.
        /// </summary>
        [XmlIgnore()]
        public NetworkModel ParentModel
        {
            get
            {
                return m_parentModel;
            }
            set
            {
                m_parentModel = value;
            }
        }

        [XmlIgnore()]
        public bool InPruningMode
        {
            get
            {
                return m_parentModel.InPruningMode;
            }
        }

        [XmlIgnore()]
        public bool RetainWhenPruning
        {
            get
            {
                if (InPruningMode)
                {
                    foreach (Division division in m_childrenDivisions)
                    {
                        if (division.RetainWhenPruning)
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
        public Company()
            :this(DEFAULT_INTERNAL_ID, DEFAULT_NUMBER, DEFAULT_ACRONYM, DEFAULT_NAME, DEFAULT_DESCRIPTION)
        {
        }

        /// <summary>
        /// A constructor for the <see cref="LinearStateEstimator.Modeling.Company"/> class which requires all of the properties defined by the <see cref="LinearStateEstimator.Modeling.INetworkDescribable"/> interface.
        /// </summary>
        /// <param name="internalID">An integer identifier for each <see cref="LinearStateEstimator.Modeling.Company"/> which is intended to be unique among other objects of the same type.</param>
        /// <param name="number">A descriptive integer for the instance of the <see cref="LinearStateEstimator.Modeling.Company"/>. There are no restrictions on uniqueness.</param>
        /// <param name="acronym">A string acronym for the instance of the <see cref="LinearStateEstimator.Modeling.Company"/>.</param>
        /// <param name="name">The string name of the instance of the <see cref="LinearStateEstimator.Modeling.Company"/>.</param>
        /// <param name="description">A string description of the instance of the <see cref="LinearStateEstimator.Modeling.Company"/>.</param>
        public Company(int internalID, int number, string acronym, string name, string description)
            :this(internalID, number, acronym, name, description, new List<Division>())
        {
        }

        /// <summary>
        /// The designated initializer for the <see cref="LinearStateEstimator.Modeling.Company"/> class. Requires all of the properties defined by the <see cref="INetworkDescribable"/> interface and a list of its children <see cref="Division"/> objects.
        /// </summary>
        /// <param name="internalID">An integer identifier for each <see cref="LinearStateEstimator.Modeling.Company"/> which is intended to be unique among other objects of the same type.</param>
        /// <param name="number">A descriptive integer for the instance of the <see cref="LinearStateEstimator.Modeling.Company"/>. There are no restrictions on uniqueness.</param>
        /// <param name="acronym">A string acronym for the instance of the <see cref="LinearStateEstimator.Modeling.Company"/>.</param>
        /// <param name="name">The string name of the instance of the <see cref="LinearStateEstimator.Modeling.Company"/>.</param>
        /// <param name="description">A string description of the instance of the <see cref="LinearStateEstimator.Modeling.Company"/>.</param>
        /// <param name="divisions">All of the children <see cref="LinearStateEstimator.Modeling.Division"/> objects owned by the <see cref="LinearStateEstimator.Modeling.Company"/>. Will return an empty list if the underlying field has not been set by the user.</param>
        public Company(int internalID, int number, string acronym, string name, string description, List<Division> divisions)
        {
            m_internalId = internalID;
            m_number = number;
            m_acronym = acronym;
            m_name = name;
            m_description = description;
            m_childrenDivisions = divisions;
            foreach (Division division in m_childrenDivisions)
            {
                division.ParentCompany = this;
            }
        }

        #endregion

        #region [ Public Methods ]

        /// <summary>
        /// A descriptive string representation of the <see cref="LinearStateEstimator.Modeling.Company"/> class instance. The format is <i>Company,internalId,number,acronym,name,description</i> and can be used for a rudimentary momento design pattern.
        /// </summary>
        /// <returns>A descriptive string representation of the <see cref="LinearStateEstimator.Modeling.Company"/> class instance.</returns>
        public override string ToString()
        {
            return "Company," + m_internalId.ToString() + "," + m_number.ToString() + "," + m_acronym + "," + m_name + "," + m_description;
        }

        /// <summary>
        /// A verbose descriptive string representation of the class instance and can be used for detailed text based output via a console or a text file.
        /// </summary>
        /// <returns>A verbose descriptive string representation of the class instance.</returns>
        public string ToVerboseString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine();
            stringBuilder.AppendFormat("----- Company ------------------------------------------------------------------");
            stringBuilder.AppendLine();
            stringBuilder.AppendFormat("      InternalID: " + m_internalId.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("          Number: " + m_number.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("         Acronym: " + m_acronym + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("            Name: " + m_name + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("     Description: " + m_description + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("       Divisions:  {0}", Environment.NewLine);
            foreach (Division division in m_childrenDivisions)
            {
                stringBuilder.AppendFormat("        " + division.ToString() + "{0}", Environment.NewLine);
            }
            stringBuilder.AppendLine();
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Determines equality between two <see cref="LinearStateEstimator.Modeling.Company"/> objects. Child <see cref="LinearStateEstimator.Modeling.Division"/> objects
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

            // If parameter cannot be cast to PhasorBase return false.
            Company company = target as Company;

            if ((object)company == null)
            {
                return false;
            }

            // Return true if the fields match:
            if (this.m_internalId != company.InternalID)
            {
                return false;
            }
            else if (this.m_number != company.Number)
            {
                return false;
            }
            else if (!this.m_acronym.Equals(company.Acronym))
            {
                return false;
            }
            else if (!this.m_name.Equals(company.Name))
            {
                return false;
            }
            else if (!this.m_description.Equals(company.Description))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Overridden to prevent compilation warnings
        /// </summary>
        /// <returns>An integer hash code of the base class</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Performs a deep copy of the target <see cref="LinearStateEstimator.Modeling.Company"/> object.
        /// </summary>
        /// <returns>A deep copy of the target <see cref="LinearStateEstimator.Modeling.Company"/> object.</returns>
        public Company DeepCopy()
        {
            Company copy = (Company)this.MemberwiseClone();

            List<Division> copyDivisions = new List<Division>();

            foreach (Division division in m_childrenDivisions)
            {
                copyDivisions.Add(division.DeepCopy());
            }

            foreach (Division division in copyDivisions)
            {
                division.ParentCompany = copy;
            }

            copy.Divisions = copyDivisions;

            return copy;
        }

        #endregion
    }
}
