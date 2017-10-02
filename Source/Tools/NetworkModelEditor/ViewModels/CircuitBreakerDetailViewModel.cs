//******************************************************************************************************
//  CircuitBreakerDetailViewModel.cs
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
using SynchrophasorAnalytics.Measurements;
using System.Windows;

namespace NetworkModelEditor.ViewModels
{
    public class CircuitBreakerDetailViewModel : ViewModelBase
    {
        #region [ Private Members ]

        private CircuitBreaker m_circuitBreaker;
        private List<Substation> m_substations;

        #endregion

        #region [ Properties ]

        public int InternalId
        {
            get
            {
                return m_circuitBreaker.InternalID;
            }
            set
            {
                m_circuitBreaker.InternalID = value;
            }
        }

        public int Number
        {
            get
            {
                return m_circuitBreaker.Number;
            }
            set
            {
                m_circuitBreaker.Number = value;
            }
        }

        public string Name
        {
            get
            {
                return m_circuitBreaker.Name;
            }
            set
            {
                m_circuitBreaker.Name = value;
            }
        }

        public string Description
        {
            get
            {
                return m_circuitBreaker.Description;
            }
            set
            {
                m_circuitBreaker.Description = value;
            }
        }

        public string Key
        {
            get
            {
                return m_circuitBreaker.MeasurementKey;
            }
            set
            {
                m_circuitBreaker.MeasurementKey = value;
            }
        }

        public SwitchingDeviceNormalState NormalState
        {
            get
            {
                return m_circuitBreaker.NormalState;
            }
            set
            {
                m_circuitBreaker.NormalState = value;
            }
        }

        public Node FromNode
        {
            get
            {
                return m_circuitBreaker.FromNode;
            }
            set
            {
                m_circuitBreaker.FromNode = value;
            }
        }

        public Node ToNode
        {
            get
            {
                return m_circuitBreaker.ToNode;
            }
            set
            {
                m_circuitBreaker.ToNode = value;
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
                return new ObservableCollection<Node>(m_circuitBreaker.ParentSubstation.Nodes);
            }
        }

        public Substation ParentSubstation
        {
            get
            {
                return m_circuitBreaker.ParentSubstation;
            }
            set
            {
                m_circuitBreaker.ParentSubstation = value;
            }
        }

        public BreakerStatus BreakerStatus
        {
            get
            {
                return m_circuitBreaker.Status;
            }
            set
            {
                m_circuitBreaker.Status = value;
            }
        }

        public ObservableCollection<BreakerStatus> BreakerStatuses
        {
            get
            {
                return new ObservableCollection<BreakerStatus>(m_circuitBreaker.ParentSubstation.ParentDivision.ParentCompany.ParentModel.BreakerStatuses);
            }
        }

        public double CrossDeviceAngleDeltaThresholdInDegrees
        {
            get
            {
                return m_circuitBreaker.CrossDeviceAngleDeltaThresholdInDegrees;
            }
            set
            {
                m_circuitBreaker.CrossDeviceAngleDeltaThresholdInDegrees = value;
            }
        }

        public double CrossDevicePerUnitMagnitudeDeltaThreshold
        {
            get
            {
                return m_circuitBreaker.CrossDevicePerUnitMagnitudeDeltaThreshold;
            }
            set
            {
                m_circuitBreaker.CrossDevicePerUnitMagnitudeDeltaThreshold = value;
            }
        }

        public double CrossDeviceTotalVectorDeltaThreshold
        {
            get
            {
                return m_circuitBreaker.CrossDeviceTotalVectorDeltaThreshold;
            }
            set
            {
                m_circuitBreaker.CrossDeviceTotalVectorDeltaThreshold = value;
            }
        }

        #endregion

        #region [ Constructors ]

        public CircuitBreakerDetailViewModel()
        {
        }

        public CircuitBreakerDetailViewModel(object circuitBreaker)
        {
            if (circuitBreaker != null && circuitBreaker is CircuitBreaker)
            {
                m_circuitBreaker = circuitBreaker as CircuitBreaker;

                GetSubstations();
            }
        }

        #endregion

        #region [ Private Methods ]

        private void GetSubstations()
        {
            m_substations = new List<Substation>();

            foreach (Company company in m_circuitBreaker.ParentSubstation.ParentDivision.ParentCompany.ParentModel.Companies)
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
