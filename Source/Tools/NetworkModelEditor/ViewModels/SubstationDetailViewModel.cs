//******************************************************************************************************
//  SubstationDetailViewModel.cs
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
using System.Windows.Input;
using SynchrophasorAnalytics.Graphs;
using SynchrophasorAnalytics.Modeling;
using SynchrophasorAnalytics.Measurements;
using NetworkModelEditor.Commands;

namespace NetworkModelEditor.ViewModels
{
    public class SubstationDetailViewModel : ViewModelBase
    {
        #region [ Private Members ]

        private Substation m_substation;
        private ViewModelBase m_mainWindow;


        #endregion

        #region [ Properties ]

        public int InternalID
        {
            get
            {
                return m_substation.InternalID;
            }
            set
            {
                m_substation.InternalID = value;
            }
        }

        public int Number
        {
            get
            {
                return m_substation.Number;
            }
            set
            {
                m_substation.Number = value;
            }
        }

        public string Name
        {
            get
            {
                return m_substation.Name;
            }
            set
            {
                m_substation.Name = value;
            }
        }

        public string Acronym
        {
            get
            {
                return m_substation.Acronym;
            }
            set
            {
                m_substation.Acronym = value;
            }
        }

        public string Description
        {
            get
            {
                return m_substation.Description;
            }
            set
            {
                m_substation.Description = value;
            }
        }

        public Division ParentDivision
        {
            get
            {
                return m_substation.ParentDivision;
            }
            set
            {
                if (m_substation.ParentDivision != value)
                {
                    // Remove the division from its current parent's ownership
                    m_substation.ParentDivision.Substations.Remove(m_substation);

                    // Give it a new parent
                    m_substation.ParentDivision = value;

                    // Make it a child of its new parent.
                    m_substation.ParentDivision.Substations.Add(m_substation);
                    OnPropertyChanged("ParentDivision");
                }
            }
        }

        public List<Division> Divisions
        {
            get
            {
                List<Division> divisions = new List<Division>();
                foreach (Company company in m_substation.ParentDivision.ParentCompany.ParentModel.Companies)
                {
                    foreach (Division division in company.Divisions)
                    {
                        divisions.Add(division);
                    }
                }
                return divisions;
            }
        }

        public double AngleDeltaThresholdInDegrees
        {
            get
            {
                return m_substation.AngleDeltaThresholdInDegrees;
            }
            set
            {
                m_substation.AngleDeltaThresholdInDegrees = value;
            }
        }

        public double PerUnitMagnitudeDeltaThreshold
        {
            get
            {
                return m_substation.PerUnitMagnitudeDeltaThreshold;
            }
            set
            {
                m_substation.PerUnitMagnitudeDeltaThreshold = value;
            }
        }

        public double TotalVectorDeltaThreshold
        {
            get
            {
                return m_substation.TotalVectorDeltaThreshold;
            }
            set
            {
                m_substation.TotalVectorDeltaThreshold = value;
            }
        }

        public VoltageCoherencyDetectionMethod CoherencyDetectionMethod
        {
            get
            {
                return m_substation.CoherencyDetectionMethod;
            }
            set
            {
                m_substation.CoherencyDetectionMethod = value;
            }
        }

        public TopologyEstimationLevel TopologyLevel
        {
            get
            {
                return m_substation.TopologyLevel;
            }
            set
            {
                m_substation.TopologyLevel = value;
            }
        }

        public string ObservedBusCountKey
        {
            get
            {
                return m_substation.ObservedBusCountKey;
            }
            set
            {
                m_substation.ObservedBusCountKey = value;
            }
        }

        #endregion

        #region [ Constructors ]

        public SubstationDetailViewModel()
        {
        }

        public SubstationDetailViewModel(object substation)
        {
            if (substation != null && substation is Substation)
            {
                m_substation = substation as Substation;
            }
        }

        #endregion


    }
}
