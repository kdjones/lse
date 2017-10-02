//******************************************************************************************************
//  TransmissionLineDetailViewModel.cs
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
    public class TransmissionLineDetailViewModel : ViewModelBase
    {
        #region [ Private Members ]

        private TransmissionLine m_transmissionLine;
        private List<Division> m_divisions;
        private List<Substation> m_substations;

        #endregion

        #region [ Properties ]

        public int InternalId
        {
            get
            {
                return m_transmissionLine.InternalID;
            }
            set
            {
                m_transmissionLine.InternalID = value;
            }
        }

        public int Number
        {
            get
            {
                return m_transmissionLine.Number;
            }
            set
            {
                m_transmissionLine.Number = value;
            }
        }

        public string Name
        {
            get
            {
                return m_transmissionLine.Name;
            }
            set
            {
                m_transmissionLine.Name = value;
            }
        }

        public string Acronym
        {
            get
            {
                return m_transmissionLine.Acronym;
            }
            set
            {
                m_transmissionLine.Acronym = value;
            }
        }

        public string Description
        {
            get
            {
                return m_transmissionLine.Description;
            }
            set
            {
                m_transmissionLine.Description = value;
            }
        }

        public ObservableCollection<Division> Divisions
        {
            get
            {
                return new ObservableCollection<Division>(m_divisions);
            }
        }

        public Division ParentDivision
        {
            get
            {
                return m_transmissionLine.ParentDivision;
            }
            set
            {
                m_transmissionLine.ParentDivision = value;
            }
        }

        public ObservableCollection<Substation> Substations
        {
            get
            {
                return new ObservableCollection<Substation>(m_substations);
            }
        }

        public Substation FromSubstation
        {
            get
            {
                return m_transmissionLine.FromSubstation;
            }
            set
            {
                m_transmissionLine.FromSubstation = value;
                OnPropertyChanged("FromNodes");
            }
        }

        public ObservableCollection<Node> FromNodes
        {
            get
            {
                if (m_transmissionLine.FromSubstation != null)
                {
                    return new ObservableCollection<Node>(m_transmissionLine.FromSubstation.Nodes);
                }
                else
                {
                    return new ObservableCollection<Node>();
                }
            }
        }

        public Node FromNode
        {
            get
            {
                return m_transmissionLine.FromNode;
            }
            set
            {
                m_transmissionLine.FromNode = value; 
                //if (m_transmissionLine.FromNode.Voltage != null)
                //{
                //    m_transmissionLine.FromSubstationCurrent.PositiveSequence.Measurement.BaseKV = value.BaseKV;
                //    m_transmissionLine.FromSubstationCurrent.PositiveSequence.Estimate.BaseKV = value.BaseKV;
                //    m_transmissionLine.FromSubstationCurrent.PhaseA.Measurement.BaseKV = value.BaseKV;
                //    m_transmissionLine.FromSubstationCurrent.PhaseA.Estimate.BaseKV = value.BaseKV;
                //    m_transmissionLine.FromSubstationCurrent.PhaseB.Measurement.BaseKV = value.BaseKV;
                //    m_transmissionLine.FromSubstationCurrent.PhaseB.Estimate.BaseKV = value.BaseKV;
                //    m_transmissionLine.FromSubstationCurrent.PhaseC.Measurement.BaseKV = value.BaseKV;
                //    m_transmissionLine.FromSubstationCurrent.PhaseC.Estimate.BaseKV = value.BaseKV;
                //}
            }
        }

        public Substation ToSubstation
        {
            get
            {
                return m_transmissionLine.ToSubstation;
            }
            set
            {
                m_transmissionLine.ToSubstation = value;
                OnPropertyChanged("ToNodes");
            }
        }

        public ObservableCollection<Node> ToNodes
        {
            get
            {
                if (m_transmissionLine.ToSubstation != null)
                {
                    return new ObservableCollection<Node>(m_transmissionLine.ToSubstation.Nodes);
                }
                else
                {
                    return new ObservableCollection<Node>();
                }
            }
        }

        public Node ToNode
        {
            get
            {
                return m_transmissionLine.ToNode;
            }
            set
            {
                m_transmissionLine.ToNode = value;
                //if (m_transmissionLine.ToNode.Voltage != null)
                //{
                //    m_transmissionLine.ToSubstationCurrent.PositiveSequence.Measurement.BaseKV = value.BaseKV;
                //    m_transmissionLine.ToSubstationCurrent.PositiveSequence.Estimate.BaseKV = value.BaseKV;
                //    m_transmissionLine.ToSubstationCurrent.PhaseA.Measurement.BaseKV = value.BaseKV;
                //    m_transmissionLine.ToSubstationCurrent.PhaseA.Estimate.BaseKV = value.BaseKV;
                //    m_transmissionLine.ToSubstationCurrent.PhaseB.Measurement.BaseKV = value.BaseKV;
                //    m_transmissionLine.ToSubstationCurrent.PhaseB.Estimate.BaseKV = value.BaseKV;
                //    m_transmissionLine.ToSubstationCurrent.PhaseC.Measurement.BaseKV = value.BaseKV;
                //    m_transmissionLine.ToSubstationCurrent.PhaseC.Estimate.BaseKV = value.BaseKV;
                //}
            }
        }

        public bool RealTimeImpedanceCalculationIsEnabled
        {
            get
            {
                return m_transmissionLine.RealTimeImpedanceCalculationIsEnabled;
            }
            set
            {
                m_transmissionLine.RealTimeImpedanceCalculationIsEnabled = value;
            }
        }

        public bool SeriesCompensatorStatusInferenceIsEnabled
        {
            get
            {
                return m_transmissionLine.SeriesCompensatorStatusInferenceIsEnabled;
            }
            set
            {
                m_transmissionLine.SeriesCompensatorStatusInferenceIsEnabled = value;
            }
        }
        
        public double CalculatedImpedanceChangeThreshold
        {
            get
            {
                return m_transmissionLine.CalculatedImpedanceChangeThreshold;
            }
            set
            {
                m_transmissionLine.CalculatedImpedanceChangeThreshold = value;
            }
        }
        
        #endregion

        #region [ Constructors ]

        public TransmissionLineDetailViewModel()
        {
        }

        public TransmissionLineDetailViewModel(object transmissionLine)
        {
            if (transmissionLine != null && transmissionLine is TransmissionLine)
            {
                m_transmissionLine = transmissionLine as TransmissionLine;

                GetDivisions();
                GetSubstations();
            }
        }


        #endregion

        #region [ Private Methods ]

        private void GetSubstations()
        {
            m_substations = new List<Substation>();

            foreach (Company company in m_transmissionLine.ParentDivision.ParentCompany.ParentModel.Companies)
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

        private void GetDivisions()
        {
            m_divisions = new List<Division>();

            foreach (Company company in m_transmissionLine.ParentDivision.ParentCompany.ParentModel.Companies)
            {
                foreach (Division division in company.Divisions)
                {
                    m_divisions.Add(division);
                }
            }
        }

        #endregion
    }
}
