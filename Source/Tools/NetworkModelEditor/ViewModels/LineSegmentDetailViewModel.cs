//******************************************************************************************************
//  LineSegmentDetailViewModel.cs
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
    public class LineSegmentDetailViewModel : ViewModelBase
    {
        #region [ Private Members ]

        private LineSegment m_lineSegment;
        private List<TransmissionLine> m_transmissionLines;

        #endregion

        #region [ Properties ]

        public int InternalId
        {
            get
            {
                return m_lineSegment.InternalID;
            }
            set
            {
                m_lineSegment.InternalID = value;
            }
        }

        public int Number
        {
            get
            {
                return m_lineSegment.Number;
            }
            set
            {
                m_lineSegment.Number = value;
            }
        }

        public string Name
        {
            get
            {
                return m_lineSegment.Name;
            }
            set
            {
                m_lineSegment.Name = value;
            }
        }

        public string Acronym
        {
            get
            {
                return m_lineSegment.Acronym;
            }
            set
            {
                m_lineSegment.Acronym = value;
            }
        }

        public string Description
        {
            get
            {
                return m_lineSegment.Description;
            }
            set
            {
                m_lineSegment.Description = value;
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
                return m_lineSegment.ParentTransmissionLine;
            }
            set
            {
                m_lineSegment.ParentTransmissionLine = value;
            }
        }

        public ObservableCollection<Node> FromNodes
        {
            get
            {
                if (m_lineSegment.ParentTransmissionLine.FromSubstation != null)
                {
                    ObservableCollection<Node> toNodes = new ObservableCollection<Node>(m_lineSegment.ParentTransmissionLine.Nodes);
                    toNodes.Add(m_lineSegment.ParentTransmissionLine.FromNode);
                    toNodes.Add(m_lineSegment.ParentTransmissionLine.ToNode);
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
                return m_lineSegment.FromNode;
            }
            set
            {
                m_lineSegment.FromNode = value;
            }
        }

        public ObservableCollection<Node> ToNodes
        {
            get
            {
                if (m_lineSegment.ParentTransmissionLine.ToSubstation != null)
                {
                    ObservableCollection<Node> toNodes = new ObservableCollection<Node>(m_lineSegment.ParentTransmissionLine.Nodes);
                    toNodes.Add(m_lineSegment.ParentTransmissionLine.FromNode);
                    toNodes.Add(m_lineSegment.ParentTransmissionLine.ToNode);
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
                return m_lineSegment.ToNode;
            }
            set
            {
                m_lineSegment.ToNode = value;
            }
        }

        #region [ Resistance ]

        public double R1
        {
            get
            {
                return m_lineSegment.RawImpedanceParameters.R1;
            }
            set
            {
                m_lineSegment.RawImpedanceParameters.R1 = value;
            }
        }

        public double R2
        {
            get
            {
                return m_lineSegment.RawImpedanceParameters.R2;
            }
            set
            {
                m_lineSegment.RawImpedanceParameters.R2 = value;
                OnPropertyChanged("R2");
            }
        }

        public double R3
        {
            get
            {
                return m_lineSegment.RawImpedanceParameters.R3;
            }
            set
            {
                m_lineSegment.RawImpedanceParameters.R3 = value;
            }
        }

        public double R4
        {
            get
            {
                return m_lineSegment.RawImpedanceParameters.R4;
            }
            set
            {
                m_lineSegment.RawImpedanceParameters.R4 = value;
                OnPropertyChanged("R4");
            }
        }

        public double R5
        {
            get
            {
                return m_lineSegment.RawImpedanceParameters.R5;
            }
            set
            {
                m_lineSegment.RawImpedanceParameters.R5 = value;
                OnPropertyChanged("R5");
            }
        }

        public double R6
        {
            get
            {
                return m_lineSegment.RawImpedanceParameters.R6;
            }
            set
            {
                m_lineSegment.RawImpedanceParameters.R6 = value;
            }
        }

        #endregion

        #region [ Reactance ]

        public double X1
        {
            get
            {
                return m_lineSegment.RawImpedanceParameters.X1;
            }
            set
            {
                m_lineSegment.RawImpedanceParameters.X1 = value;
            }
        }

        public double X2
        {
            get
            {
                return m_lineSegment.RawImpedanceParameters.X2;
            }
            set
            {
                m_lineSegment.RawImpedanceParameters.X2 = value;
                OnPropertyChanged("X2");
            }
        }

        public double X3
        {
            get
            {
                return m_lineSegment.RawImpedanceParameters.X3;
            }
            set
            {
                m_lineSegment.RawImpedanceParameters.X3 = value;
            }
        }

        public double X4
        {
            get
            {
                return m_lineSegment.RawImpedanceParameters.X4;
            }
            set
            {
                m_lineSegment.RawImpedanceParameters.X4 = value;
                OnPropertyChanged("X4");
            }
        }

        public double X5
        {
            get
            {
                return m_lineSegment.RawImpedanceParameters.X5;
            }
            set
            {
                m_lineSegment.RawImpedanceParameters.X5 = value;
                OnPropertyChanged("X5");
            }
        }

        public double X6
        {
            get
            {
                return m_lineSegment.RawImpedanceParameters.X6;
            }
            set
            {
                m_lineSegment.RawImpedanceParameters.X6 = value;
            }
        }

        #endregion

        #region [ Conductance ]

        public double G1
        {
            get
            {
                return m_lineSegment.RawImpedanceParameters.G1;
            }
            set
            {
                m_lineSegment.RawImpedanceParameters.G1 = value;
            }
        }

        public double G2
        {
            get
            {
                return m_lineSegment.RawImpedanceParameters.G2;
            }
            set
            {
                m_lineSegment.RawImpedanceParameters.G2 = value;
            }
        }

        public double G3
        {
            get
            {
                return m_lineSegment.RawImpedanceParameters.G3;
            }
            set
            {
                m_lineSegment.RawImpedanceParameters.G3 = value;
            }
        }

        public double G4
        {
            get
            {
                return m_lineSegment.RawImpedanceParameters.G4;
            }
            set
            {
                m_lineSegment.RawImpedanceParameters.G4 = value;
            }
        }

        public double G5
        {
            get
            {
                return m_lineSegment.RawImpedanceParameters.G5;
            }
            set
            {
                m_lineSegment.RawImpedanceParameters.G5 = value;
            }
        }

        public double G6
        {
            get
            {
                return m_lineSegment.RawImpedanceParameters.G6;
            }
            set
            {
                m_lineSegment.RawImpedanceParameters.G6 = value;
            }
        }

        #endregion

        #region [ Shunt Susceptance ]

        public double B1
        {
            get
            {
                return m_lineSegment.RawImpedanceParameters.B1;
            }
            set
            {
                m_lineSegment.RawImpedanceParameters.B1 = value;
            }
        }

        public double B2
        {
            get
            {
                return m_lineSegment.RawImpedanceParameters.B2;
            }
            set
            {
                m_lineSegment.RawImpedanceParameters.B2 = value;
                OnPropertyChanged("B2");
            }
        }

        public double B3
        {
            get
            {
                return m_lineSegment.RawImpedanceParameters.B3;
            }
            set
            {
                m_lineSegment.RawImpedanceParameters.B3 = value;
            }
        }

        public double B4
        {
            get
            {
                return m_lineSegment.RawImpedanceParameters.B4;
            }
            set
            {
                m_lineSegment.RawImpedanceParameters.B4 = value;
                OnPropertyChanged("B4");
            }
        }

        public double B5
        {
            get
            {
                return m_lineSegment.RawImpedanceParameters.B5;
            }
            set
            {
                m_lineSegment.RawImpedanceParameters.B5 = value;
                OnPropertyChanged("B5");
            }
        }

        public double B6
        {
            get
            {
                return m_lineSegment.RawImpedanceParameters.B6;
            }
            set
            {
                m_lineSegment.RawImpedanceParameters.B6 = value;
            }
        }

        #endregion

        #endregion

        #region [ Constructors ]

        public LineSegmentDetailViewModel()
        {
        }

        public LineSegmentDetailViewModel(object lineSegment)
        {
            if (lineSegment != null && lineSegment is LineSegment)
            {
                m_lineSegment = lineSegment as LineSegment;

                GetTransmissionLines();
            }
        }

        private void GetTransmissionLines()
        {
            m_transmissionLines = new List<TransmissionLine>();

            foreach (Company company in m_lineSegment.ParentTransmissionLine.ParentDivision.ParentCompany.ParentModel.Companies)
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
