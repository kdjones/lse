//******************************************************************************************************
//  CompanyDetailViewModel.cs
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SynchrophasorAnalytics.Modeling;

namespace NetworkModelEditor.ViewModels
{
    public class CompanyDetailViewModel : ViewModelBase
    {
        #region [ Private Members ]

        private Company m_company;

        #endregion

        #region [ Properties ]

        public int InternalID
        {
            get
            {
                return m_company.InternalID;
            }
            set
            {
                m_company.InternalID = value;
            }
        }

        public int Number
        {
            get
            {
                return m_company.Number;
            }
            set
            {
                m_company.Number = value;
            }
        }

        public string Name
        {
            get
            {
                return m_company.Name;
            }
            set
            {
                m_company.Name = value;
            }
        }

        public string Acronym
        {
            get
            {
                return m_company.Acronym;
            }
            set
            {
                m_company.Acronym = value;
            }
        }

        public string Description
        {
            get
            {
                return m_company.Description;
            }
            set
            {
                m_company.Description = value;
            }
        }

        #endregion

        #region [ Constructors ]

        public CompanyDetailViewModel()
        {
        }

        public CompanyDetailViewModel(object company)
        {
            if (company != null && company is Company)
            {
                m_company = company as Company;
            }
        }

        #endregion
    }
}
