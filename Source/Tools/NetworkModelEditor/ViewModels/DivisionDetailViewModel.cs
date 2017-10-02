//******************************************************************************************************
//  DivisionDetailViewModel.cs
//
//  Copyright © 2014, Kevin D. Jones.  All Rights Reserved.
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
//  02/01/2014 - Kevin D. Jones
//       Generated original version of source code.
//
//******************************************************************************************************
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SynchrophasorAnalytics.Modeling;

namespace NetworkModelEditor.ViewModels
{
    public class DivisionDetailViewModel : ViewModelBase
    {
        #region [ Private Members ]

        private Division m_division;

        #endregion

        #region [ Properties ]

        public int InternalID
        {
            get
            {
                return m_division.InternalID;
            }
            set
            {
                m_division.InternalID = value;
            }
        }

        public int Number
        {
            get
            {
                return m_division.Number;
            }
            set
            {
                m_division.Number = value;
            }
        }

        public string Name
        {
            get
            {
                return m_division.Name;
            }
            set
            {
                m_division.Name = value;
            }
        }

        public string Acronym
        {
            get
            {
                return m_division.Acronym;
            }
            set
            {
                m_division.Acronym = value;
            }
        }

        public string Description
        {
            get
            {
                return m_division.Description;
            }
            set
            {
                m_division.Description = value;
            }
        }

        public Company ParentCompany
        {
            get
            {
                return m_division.ParentCompany;
            }
            set
            {
                if (m_division.ParentCompany != value)
                {
                    // Remove the division from its current parent's ownership
                    m_division.ParentCompany.Divisions.Remove(m_division);

                    // Give it a new parent
                    m_division.ParentCompany = value;

                    // Make it a child of its new parent.
                    m_division.ParentCompany.Divisions.Add(m_division);
                    OnPropertyChanged("ParentCompany");
                }
            }
        }

        public List<Company> Companies
        {
            get
            {
                return m_division.ParentCompany.ParentModel.Companies;
            }
        }

        #endregion

        #region [ Constructors ]

        public DivisionDetailViewModel()
        {
        }

        public DivisionDetailViewModel(object division)
        {
            if (division != null && division is Division)
            {
                m_division = division as Division;
            }
        }

        #endregion
    }
}
