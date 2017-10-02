//******************************************************************************************************
//  ShuntDetailViewModel.cs
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
//  05/12/2014 - Kevin D. Jones
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
using SynchrophasorAnalytics.Measurements;
using System.Windows;

namespace NetworkModelEditor.ViewModels
{
    public class ShuntDetailViewModel : ViewModelBase
    {
        #region [ Private Fields ]

        private ShuntCompensator m_shunt;
        private List<Substation> m_substations;

        #endregion

        #region [ Properties ]

        public int InternalID
        {
            get
            {
                return m_shunt.InternalID;
            }
            set
            {
                m_shunt.InternalID = value;
            }
        }

        public int Number
        {
            get
            {
                return m_shunt.Number;
            }
            set
            {
                m_shunt.Number = value;
            }
        }

        public string Name
        {
            get
            {
                return m_shunt.Name;
            }
            set
            {
                m_shunt.Name = value;
            }
        }

        public string Description
        {
            get
            {
                return m_shunt.Description;
            }
            set
            {
                m_shunt.Description = value;
            }
        }

        public Substation ParentSubstation
        {
            get
            {
                return m_shunt.ParentSubstation;
            }
            set
            {
                if (m_shunt.ParentSubstation != value)
                {
                    // Remove the division from its current parent's ownership
                    m_shunt.ParentSubstation.Shunts.Remove(m_shunt);

                    // Give it a new parent
                    m_shunt.ParentSubstation = value;

                    // Make it a child of its new parent.
                    m_shunt.ParentSubstation.Shunts.Add(m_shunt);
                    OnPropertyChanged("ParentSubstation");
                }
            }
        }

        public Node ConnectedNode
        {
            get
            {
                return m_shunt.ConnectedNode;
            }
            set
            {
                m_shunt.ConnectedNode = value;
            }
        }

        public ShuntImpedanceCalculationMethod ImpedanceCalculationMethod
        {
            get
            {
                return m_shunt.ImpedanceCalculationMethod;
            }
            set
            {
                m_shunt.ImpedanceCalculationMethod = value;
                OnPropertyChanged("ImpedanceTableIsEnabled");
            }
        }

        public double NominalMvar
        {
            get
            {
                return m_shunt.NominalMvar;
            }
            set
            {
                m_shunt.NominalMvar = value;
            }
        }

        public ObservableCollection<Substation> Substations
        {
            get
            {
                return new ObservableCollection<Substation>(m_substations);
            }
        }

        public ObservableCollection<Node> Nodes
        {
            get
            {
                return new ObservableCollection<Node>(m_shunt.ParentSubstation.Nodes);
            }
        }

        #region [ Shunt Susceptance ]

        public double B1
        {
            get
            {
                return m_shunt.RawImpedanceParameters.B1;
            }
            set
            {
                m_shunt.RawImpedanceParameters.B1 = value;
            }
        }

        public double B2
        {
            get
            {
                return m_shunt.RawImpedanceParameters.B2;
            }
            set
            {
                m_shunt.RawImpedanceParameters.B2 = value;
                OnPropertyChanged("B2");
            }
        }

        public double B3
        {
            get
            {
                return m_shunt.RawImpedanceParameters.B3;
            }
            set
            {
                m_shunt.RawImpedanceParameters.B3 = value;
            }
        }

        public double B4
        {
            get
            {
                return m_shunt.RawImpedanceParameters.B4;
            }
            set
            {
                m_shunt.RawImpedanceParameters.B4 = value;
                OnPropertyChanged("B4");
            }
        }

        public double B5
        {
            get
            {
                return m_shunt.RawImpedanceParameters.B5;
            }
            set
            {
                m_shunt.RawImpedanceParameters.B5 = value;
                OnPropertyChanged("B5");
            }
        }

        public double B6
        {
            get
            {
                return m_shunt.RawImpedanceParameters.B6;
            }
            set
            {
                m_shunt.RawImpedanceParameters.B6 = value;
            }
        }

        #endregion

        public bool ImpedanceTableIsEnabled
        {
            get
            {
                if (m_shunt.ImpedanceCalculationMethod == ShuntImpedanceCalculationMethod.UseModeledImpedance)
                {
                    return true;
                }
                else if (m_shunt.ImpedanceCalculationMethod == ShuntImpedanceCalculationMethod.CalculateFromRating)
                {
                    return false;
                }
                return true;
            }
        }

        #endregion

        #region [ Constructors ]

        public ShuntDetailViewModel()
        {
        }

        public ShuntDetailViewModel(object shunt)
        {
            if (shunt != null && shunt is ShuntCompensator)
            {
                m_shunt = shunt as ShuntCompensator;

                GetSubstations();
            }
        }

        #endregion

        #region [ Private Methods ]

        private void GetSubstations()
        {
            m_substations = new List<Substation>();

            foreach (Company company in m_shunt.ParentSubstation.ParentDivision.ParentCompany.ParentModel.Companies)
            {
                foreach (Division division in company.Divisions)
                {
                    foreach (Substation substation in division.Substations)
                    {
                        m_substations.Add(substation);
                    }
                }
            }
        }

        #endregion

    }
}
