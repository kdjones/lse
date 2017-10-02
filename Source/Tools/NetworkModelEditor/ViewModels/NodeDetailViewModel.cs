//******************************************************************************************************
//  NodeDetailViewModel.cs
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
    public class NodeDetailViewModel : ViewModelBase
    {
        #region [ Private Members ]

        private Node m_node;

        #endregion

        #region [ Properties ]

        public int InternalID
        {
            get
            {
                return m_node.InternalID;
            }
            set
            {
                m_node.InternalID = value;
            }
        }

        public int Number
        {
            get
            {
                return m_node.Number;
            }
            set
            {
                m_node.Number = value;
            }
        }

        public string Name
        {
            get
            {
                return m_node.Name;
            }
            set
            {
                m_node.Name = value;
            }
        }

        public string Acronym
        {
            get
            {
                return m_node.Acronym;
            }
            set
            {
                m_node.Acronym = value;
            }
        }

        public string Description
        {
            get
            {
                return m_node.Description;
            }
            set
            {
                m_node.Description = value;
            }
        }

        public Substation ParentSubstation
        {
            get
            {
                return m_node.ParentSubstation;
            }
            set
            {
                if (m_node.ParentSubstation != value)
                {
                    // Remove the division from its current parent's ownership
                    m_node.ParentSubstation.Nodes.Remove(m_node);

                    // Give it a new parent
                    m_node.ParentSubstation = value;

                    // Make it a child of its new parent.
                    m_node.ParentSubstation.Nodes.Add(m_node);
                    OnPropertyChanged("ParentSubstation");
                }
            }
        }

        public List<Substation> Substations
        {
            get
            {
                List<Substation> substations = new List<Substation>();
                if (m_node.ParentSubstation != null)
                {
                    foreach (Company company in m_node.ParentSubstation.ParentDivision.ParentCompany.ParentModel.Companies)
                    {
                        foreach (Division division in company.Divisions)
                        {
                            foreach (Substation substation in division.Substations)
                            {
                                substations.Add(substation);
                            }
                        }
                    }
                }
                else if (m_node.ParentTransmissionLine != null)
                {
                    foreach (Company company in m_node.ParentTransmissionLine.ParentDivision.ParentCompany.ParentModel.Companies)
                    {
                        foreach (Division division in company.Divisions)
                        {
                            foreach (Substation substation in division.Substations)
                            {
                                substations.Add(substation);
                            }
                        }
                    }
                }
                return substations;
            }
        }

        public TransmissionLine ParentTransmissionLine
        {
            get
            {
                return m_node.ParentTransmissionLine;
            }
            set
            {
                if (m_node.ParentTransmissionLine != value)
                {
                    if (m_node.ParentSubstation != null)
                    {
                        m_node.ParentTransmissionLine = value;
                        m_node.ParentTransmissionLine.Nodes.Add(m_node);
                    }
                    else
                    {
                        // Remove the division from its current parent's ownership
                        m_node.ParentTransmissionLine.Nodes.Remove(m_node);
                        m_node.ParentTransmissionLine = value;
                        m_node.ParentTransmissionLine.Nodes.Add(m_node);
                    }
                    OnPropertyChanged("ParentTransmissionLine");
                }
            }
        }

        public List<TransmissionLine> TransmissionLines
        {
            get
            {
                List<TransmissionLine> transmissionLines = new List<TransmissionLine>();

                if (m_node.ParentTransmissionLine != null)
                {
                    foreach (Company company in m_node.ParentTransmissionLine.ParentDivision.ParentCompany.ParentModel.Companies)
                    {
                        foreach (Division division in company.Divisions)
                        {
                            foreach (TransmissionLine transmissionLine in division.TransmissionLines)
                            {
                                transmissionLines.Add(transmissionLine);
                            }
                        }
                    }
                }
                else if (m_node.ParentSubstation != null)
                {
                    foreach (Company company in m_node.ParentSubstation.ParentDivision.ParentCompany.ParentModel.Companies)
                    {
                        foreach (Division division in company.Divisions)
                        {
                            foreach (TransmissionLine transmissionLine in division.TransmissionLines)
                            {
                                transmissionLines.Add(transmissionLine);
                            }
                        }
                    }
                }
                return transmissionLines;
            }
        }

        public VoltageLevel BaseKV
        {
            get
            {
                return m_node.BaseKV;
            }
            set
            {
                m_node.BaseKV = value;
                if (m_node.Voltage != null)
                {
                    m_node.Voltage.PositiveSequence.Measurement.BaseKV = value;
                    m_node.Voltage.PositiveSequence.Estimate.BaseKV = value;
                    m_node.Voltage.PhaseA.Measurement.BaseKV = value;
                    m_node.Voltage.PhaseA.Estimate.BaseKV = value;
                    m_node.Voltage.PhaseB.Measurement.BaseKV = value;
                    m_node.Voltage.PhaseB.Estimate.BaseKV = value;
                    m_node.Voltage.PhaseC.Measurement.BaseKV = value;
                    m_node.Voltage.PhaseC.Estimate.BaseKV = value;
                }
            }
        }

        public List<VoltageLevel> VoltageLevels
        {
            get
            {
                if (m_node.ParentSubstation != null)
                {
                    return m_node.ParentSubstation.ParentDivision.ParentCompany.ParentModel.VoltageLevels;
                }
                else if (m_node.ParentTransmissionLine != null)
                {
                    return m_node.ParentTransmissionLine.ParentDivision.ParentCompany.ParentModel.VoltageLevels;
                }
                else
                {
                    return new List<VoltageLevel>();
                }
            }
        }

        public string ObservationStateKey
        {
            get
            {
                return m_node.ObservationStateKey;
            }
            set
            {
                m_node.ObservationStateKey = value;
            }
        }

        public string ObservedBusIdKey
        {
            get
            {
                return m_node.ObservedBusIdKey;
            }
            set
            {
                m_node.ObservedBusIdKey = value;
            }
        }

        #endregion

        #region [ Constructors ]

        public NodeDetailViewModel()
        {
        }

        public NodeDetailViewModel(object node)
        {
            if (node != null && node is Node)
            {
                m_node = node as Node;
            }
        }

        #endregion
    }
}
