//******************************************************************************************************
//  SwitchDetailViewModel.cs
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

namespace NetworkModelEditor.ViewModels
{
    public class SwitchDetailViewModel : ViewModelBase
    {
        #region [ Private Members ]

        private Switch m_switch;
        private List<Substation> m_substations;

        #endregion

        #region [ Properties ]

        public int InternalId
        {
            get
            {
                return m_switch.InternalID;
            }
            set
            {
                m_switch.InternalID = value;
            }
        }

        public int Number
        {
            get
            {
                return m_switch.Number;
            }
            set
            {
                m_switch.Number = value;
            }
        }

        public string Name
        {
            get
            {
                return m_switch.Name;
            }
            set
            {
                m_switch.Name = value;
            }
        }

        public string Description
        {
            get
            {
                return m_switch.Description;
            }
            set
            {
                m_switch.Description = value;
            }
        }

        public string Key
        {
            get
            {
                return m_switch.MeasurementKey;
            }
            set
            {
                m_switch.MeasurementKey = value;
            }
        }

        public SwitchingDeviceNormalState NormalState
        {
            get
            {
                return m_switch.NormalState;
            }
            set
            {
                m_switch.NormalState = value;
            }
        }

        public Node FromNode
        {
            get
            {
                return m_switch.FromNode;
            }
            set
            {
                m_switch.FromNode = value;
            }
        }

        public Node ToNode
        {
            get
            {
                return m_switch.ToNode;
            }
            set
            {
                m_switch.ToNode = value;
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
                if (m_switch.ParentSubstation != null)
                {
                    return new ObservableCollection<Node>(m_switch.ParentSubstation.Nodes);
                }
                else if (m_switch.ParentTransmissionLine != null)
                {
                    ObservableCollection<Node> nodes = new ObservableCollection<Node>(m_switch.ParentTransmissionLine.Nodes);
                    nodes.Add(m_switch.ParentTransmissionLine.FromNode);
                    nodes.Add(m_switch.ParentTransmissionLine.ToNode);
                    return nodes;
                }
                else
                {
                    return new ObservableCollection<Node>();
                }
            }
        }

        public Substation ParentSubstation
        {
            get
            {
                return m_switch.ParentSubstation;
            }
            set
            {
                m_switch.ParentSubstation = value;
            }
        }

        public double CrossDeviceAngleDeltaThresholdInDegrees
        {
            get
            {
                return m_switch.CrossDeviceAngleDeltaThresholdInDegrees;
            }
            set
            {
                m_switch.CrossDeviceAngleDeltaThresholdInDegrees = value;
            }
        }

        public double CrossDevicePerUnitMagnitudeDeltaThreshold
        {
            get
            {
                return m_switch.CrossDevicePerUnitMagnitudeDeltaThreshold;
            }
            set
            {
                m_switch.CrossDevicePerUnitMagnitudeDeltaThreshold = value;
            }
        }

        public double CrossDeviceTotalVectorDeltaThreshold
        {
            get
            {
                return m_switch.CrossDeviceTotalVectorDeltaThreshold;
            }
            set
            {
                m_switch.CrossDeviceTotalVectorDeltaThreshold = value;
            }
        }

        #endregion

        #region [ Constructors ]

        public SwitchDetailViewModel()
        {
        }

        public SwitchDetailViewModel(object circuitSwitch)
        {
            if (circuitSwitch != null && circuitSwitch is Switch)
            {
                m_switch = circuitSwitch as Switch;
                GetSubstations();
            }
        }

        #endregion

        private void GetSubstations()
        {
            m_substations = new List<Substation>();

            if (m_switch.ParentSubstation != null)
            {
                foreach (Company company in m_switch.ParentSubstation.ParentDivision.ParentCompany.ParentModel.Companies)
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
        }

    }
}
