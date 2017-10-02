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
    public class SeriesCompensatorDetailViewModel : ViewModelBase
    {
        #region [ Private Fields ]

        private SeriesCompensator m_seriesCompensator;
        private List<TransmissionLine> m_transmissionLines;

        #endregion

        #region [ Properties ]

        public int InternalId
        {
            get
            {
                return m_seriesCompensator.InternalID;
            }
            set
            {
                m_seriesCompensator.InternalID = value;
            }
        }

        public int Number
        {
            get
            {
                return m_seriesCompensator.Number;
            }
            set
            {
                m_seriesCompensator.Number = value;
            }
        }

        public string Name
        {
            get
            {
                return m_seriesCompensator.Name;
            }
            set
            {
                m_seriesCompensator.Name = value;
            }
        }

        public string Acronym
        {
            get
            {
                return m_seriesCompensator.Acronym;
            }
            set
            {
                m_seriesCompensator.Acronym = value;
            }
        }

        public string Description
        {
            get
            {
                return m_seriesCompensator.Description;
            }
            set
            {
                m_seriesCompensator.Description = value;
            }
        }

        public ObservableCollection<TransmissionLine> TransmissionLines
        {
            get
            {
                return new ObservableCollection<TransmissionLine>(m_transmissionLines);
            }
        }

        public TransmissionLine ParentTransmissionLine
        {
            get
            {
                return m_seriesCompensator.ParentTransmissionLine;
            }
            set
            {
                m_seriesCompensator.ParentTransmissionLine = value;
            }
        }

        public ObservableCollection<Node> FromNodes
        {
            get
            {
                if (m_seriesCompensator.ParentTransmissionLine.FromSubstation != null)
                {
                    ObservableCollection<Node> toNodes = new ObservableCollection<Node>(m_seriesCompensator.ParentTransmissionLine.Nodes);
                    toNodes.Add(m_seriesCompensator.ParentTransmissionLine.FromNode);
                    toNodes.Add(m_seriesCompensator.ParentTransmissionLine.ToNode);
                    return toNodes;
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
                return m_seriesCompensator.FromNode;
            }
            set
            {
                m_seriesCompensator.FromNode = value;
            }
        }

        public ObservableCollection<Node> ToNodes
        {
            get
            {
                if (m_seriesCompensator.ParentTransmissionLine.ToSubstation != null)
                {
                    ObservableCollection<Node> toNodes = new ObservableCollection<Node>(m_seriesCompensator.ParentTransmissionLine.Nodes);
                    toNodes.Add(m_seriesCompensator.ParentTransmissionLine.FromNode);
                    toNodes.Add(m_seriesCompensator.ParentTransmissionLine.ToNode);
                    return toNodes;
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
                return m_seriesCompensator.ToNode;
            }
            set
            {
                m_seriesCompensator.ToNode = value;
            }
        }

        public string OutputMeasurementKey
        {
            get
            {
                return m_seriesCompensator.OutputMeasurementKey;
            }
            set
            {
                m_seriesCompensator.OutputMeasurementKey = value;
            }
        }

        public SeriesCompensatorType Type
        {
            get
            {
                return m_seriesCompensator.Type;
            }
            set
            {
                m_seriesCompensator.Type = value;
            }
        }

        #region [ Resistance ]

        public double R1
        {
            get
            {
                return m_seriesCompensator.RawImpedanceParameters.R1;
            }
            set
            {
                m_seriesCompensator.RawImpedanceParameters.R1 = value;
            }
        }

        public double R2
        {
            get
            {
                return m_seriesCompensator.RawImpedanceParameters.R2;
            }
            set
            {
                m_seriesCompensator.RawImpedanceParameters.R2 = value;
                OnPropertyChanged("R2");
            }
        }

        public double R3
        {
            get
            {
                return m_seriesCompensator.RawImpedanceParameters.R3;
            }
            set
            {
                m_seriesCompensator.RawImpedanceParameters.R3 = value;
            }
        }

        public double R4
        {
            get
            {
                return m_seriesCompensator.RawImpedanceParameters.R4;
            }
            set
            {
                m_seriesCompensator.RawImpedanceParameters.R4 = value;
                OnPropertyChanged("R4");
            }
        }

        public double R5
        {
            get
            {
                return m_seriesCompensator.RawImpedanceParameters.R5;
            }
            set
            {
                m_seriesCompensator.RawImpedanceParameters.R5 = value;
                OnPropertyChanged("R5");
            }
        }

        public double R6
        {
            get
            {
                return m_seriesCompensator.RawImpedanceParameters.R6;
            }
            set
            {
                m_seriesCompensator.RawImpedanceParameters.R6 = value;
            }
        }

        #endregion

        #region [ Reactance ]

        public double X1
        {
            get
            {
                return m_seriesCompensator.RawImpedanceParameters.X1;
            }
            set
            {
                m_seriesCompensator.RawImpedanceParameters.X1 = value;
            }
        }

        public double X2
        {
            get
            {
                return m_seriesCompensator.RawImpedanceParameters.X2;
            }
            set
            {
                m_seriesCompensator.RawImpedanceParameters.X2 = value;
                OnPropertyChanged("X2");
            }
        }

        public double X3
        {
            get
            {
                return m_seriesCompensator.RawImpedanceParameters.X3;
            }
            set
            {
                m_seriesCompensator.RawImpedanceParameters.X3 = value;
            }
        }

        public double X4
        {
            get
            {
                return m_seriesCompensator.RawImpedanceParameters.X4;
            }
            set
            {
                m_seriesCompensator.RawImpedanceParameters.X4 = value;
                OnPropertyChanged("X4");
            }
        }

        public double X5
        {
            get
            {
                return m_seriesCompensator.RawImpedanceParameters.X5;
            }
            set
            {
                m_seriesCompensator.RawImpedanceParameters.X5 = value;
                OnPropertyChanged("X5");
            }
        }

        public double X6
        {
            get
            {
                return m_seriesCompensator.RawImpedanceParameters.X6;
            }
            set
            {
                m_seriesCompensator.RawImpedanceParameters.X6 = value;
            }
        }

        #endregion

        #region [ Conductance ]

        public double G1
        {
            get
            {
                return m_seriesCompensator.RawImpedanceParameters.G1;
            }
            set
            {
                m_seriesCompensator.RawImpedanceParameters.G1 = value;
            }
        }

        public double G2
        {
            get
            {
                return m_seriesCompensator.RawImpedanceParameters.G2;
            }
            set
            {
                m_seriesCompensator.RawImpedanceParameters.G2 = value;
            }
        }

        public double G3
        {
            get
            {
                return m_seriesCompensator.RawImpedanceParameters.G3;
            }
            set
            {
                m_seriesCompensator.RawImpedanceParameters.G3 = value;
            }
        }

        public double G4
        {
            get
            {
                return m_seriesCompensator.RawImpedanceParameters.G4;
            }
            set
            {
                m_seriesCompensator.RawImpedanceParameters.G4 = value;
            }
        }

        public double G5
        {
            get
            {
                return m_seriesCompensator.RawImpedanceParameters.G5;
            }
            set
            {
                m_seriesCompensator.RawImpedanceParameters.G5 = value;
            }
        }

        public double G6
        {
            get
            {
                return m_seriesCompensator.RawImpedanceParameters.G6;
            }
            set
            {
                m_seriesCompensator.RawImpedanceParameters.G6 = value;
            }
        }

        #endregion

        #region [ Shunt Susceptance ]

        public double B1
        {
            get
            {
                return m_seriesCompensator.RawImpedanceParameters.B1;
            }
            set
            {
                m_seriesCompensator.RawImpedanceParameters.B1 = value;
            }
        }

        public double B2
        {
            get
            {
                return m_seriesCompensator.RawImpedanceParameters.B2;
            }
            set
            {
                m_seriesCompensator.RawImpedanceParameters.B2 = value;
                OnPropertyChanged("B2");
            }
        }

        public double B3
        {
            get
            {
                return m_seriesCompensator.RawImpedanceParameters.B3;
            }
            set
            {
                m_seriesCompensator.RawImpedanceParameters.B3 = value;
            }
        }

        public double B4
        {
            get
            {
                return m_seriesCompensator.RawImpedanceParameters.B4;
            }
            set
            {
                m_seriesCompensator.RawImpedanceParameters.B4 = value;
                OnPropertyChanged("B4");
            }
        }

        public double B5
        {
            get
            {
                return m_seriesCompensator.RawImpedanceParameters.B5;
            }
            set
            {
                m_seriesCompensator.RawImpedanceParameters.B5 = value;
                OnPropertyChanged("B5");
            }
        }

        public double B6
        {
            get
            {
                return m_seriesCompensator.RawImpedanceParameters.B6;
            }
            set
            {
                m_seriesCompensator.RawImpedanceParameters.B6 = value;
            }
        }

        #endregion

        #endregion

        #region [ Constructors ]
        
        public SeriesCompensatorDetailViewModel()
        {
        }

        public SeriesCompensatorDetailViewModel(object seriesCompensator)
        {
            if (seriesCompensator != null && seriesCompensator is SeriesCompensator)
            {
                m_seriesCompensator = seriesCompensator as SeriesCompensator;

                GetTransmissionLines();
            }
        }

        #endregion

        #region [ Private Methods ]

        private void GetTransmissionLines()
        {
            m_transmissionLines = new List<TransmissionLine>();

            foreach (Company company in m_seriesCompensator.ParentTransmissionLine.ParentDivision.ParentCompany.ParentModel.Companies)
            {
                foreach (Division division in company.Divisions)
                {
                    foreach (TransmissionLine transmissionLine in division.TransmissionLines)
                    {
                        m_transmissionLines.Add(transmissionLine);
                    }
                }
            }
        }

        #endregion
    }
}
