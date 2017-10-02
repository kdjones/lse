//******************************************************************************************************
//  TransformerDetailViewModel.cs
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
    public class TransformerDetailViewModel : ViewModelBase
    {
        #region [ Private Members ]

        private Transformer m_transformer;
        private List<Substation> m_substations;
        private List<TapConfiguration> m_tapConfigurations;

        #endregion

        #region [ Properties ]

        public int InternalId
        {
            get
            {
                return m_transformer.InternalID;
            }
            set
            {
                m_transformer.InternalID = value;
            }
        }

        public int Number
        {
            get
            {
                return m_transformer.Number;
            }
            set
            {
                m_transformer.Number = value;
            }
        }

        public string Name
        {
            get
            {
                return m_transformer.Name;
            }
            set
            {
                m_transformer.Name = value;
            }
        }

        public string Description
        {
            get
            {
                return m_transformer.Description;
            }
            set
            {
                m_transformer.Description = value;
            }
        }

        public ObservableCollection<Substation> Substations
        {
            get
            {
                return new ObservableCollection<Substation>(m_substations);
            }
        }

        public Substation ParentSubstation
        {
            get
            {
                return m_transformer.ParentSubstation;
            }
            set
            {
                m_transformer.ParentSubstation = value;
            }
        }
        
        public ObservableCollection<Node> Nodes
        {
            get
            {
                if (m_transformer.ParentSubstation != null)
                {
                    return new ObservableCollection<Node>(m_transformer.ParentSubstation.Nodes);
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
                return m_transformer.FromNode;
            }
            set
            {
                m_transformer.FromNode = value;
            }
        }

        public TransformerConnectionType FromNodeConnectionType
        {
            get
            {
                return m_transformer.FromNodeConnectionType;
            }
            set
            {
                m_transformer.FromNodeConnectionType = value;
            }
        }

        public Node ToNode
        {
            get
            {
                return m_transformer.ToNode;
            }
            set
            {
                m_transformer.ToNode = value;
            }
        }

        public TransformerConnectionType ToNodeConnectionType
        {
            get
            {
                return m_transformer.ToNodeConnectionType;
            }
            set
            {
                m_transformer.ToNodeConnectionType = value;
            }
        }
        
        public ObservableCollection<TapConfiguration> Taps
        {
            get
            {
                return new ObservableCollection<TapConfiguration>(m_tapConfigurations);
            }
        }

        public TapConfiguration Tap
        {
            get
            {
                return m_transformer.Tap;
            }
            set
            {
                m_transformer.Tap = value;
            }
        }
        
        public string TapPositionInputKey
        {
            get
            {
                return m_transformer.TapPositionInputKey;
            }
            set
            {
                m_transformer.TapPositionInputKey = value;
            }
        }

        public string TapPositionOutputKey
        {
            get
            {
                return m_transformer.TapPositionOutputKey;
            }
            set
            {
                m_transformer.TapPositionOutputKey = value;
            }
        }

        public int FixedTapPosition
        {
            get
            {
                return m_transformer.FixedTapPosition;
            }
            set
            {
                m_transformer.FixedTapPosition = value;
            }
        }
        
        public bool EnableUltc
        {
            get
            {
                return m_transformer.UltcIsEnabled;
            }
            set
            {
                m_transformer.UltcIsEnabled = value;
            }
        }

        #region [ Resistance ]

        public double R1
        {
            get
            {
                return m_transformer.RawImpedanceParameters.R1;
            }
            set
            {
                m_transformer.RawImpedanceParameters.R1 = value;
            }
        }

        public double R2
        {
            get
            {
                return m_transformer.RawImpedanceParameters.R2;
            }
            set
            {
                m_transformer.RawImpedanceParameters.R2 = value;
                OnPropertyChanged("R2");
            }
        }

        public double R3
        {
            get
            {
                return m_transformer.RawImpedanceParameters.R3;
            }
            set
            {
                m_transformer.RawImpedanceParameters.R3 = value;
            }
        }

        public double R4
        {
            get
            {
                return m_transformer.RawImpedanceParameters.R4;
            }
            set
            {
                m_transformer.RawImpedanceParameters.R4 = value;
                OnPropertyChanged("R4");
            }
        }

        public double R5
        {
            get
            {
                return m_transformer.RawImpedanceParameters.R5;
            }
            set
            {
                m_transformer.RawImpedanceParameters.R5 = value;
                OnPropertyChanged("R5");
            }
        }

        public double R6
        {
            get
            {
                return m_transformer.RawImpedanceParameters.R6;
            }
            set
            {
                m_transformer.RawImpedanceParameters.R6 = value;
            }
        }

        #endregion

        #region [ Reactance ]

        public double X1
        {
            get
            {
                return m_transformer.RawImpedanceParameters.X1;
            }
            set
            {
                m_transformer.RawImpedanceParameters.X1 = value;
            }
        }

        public double X2
        {
            get
            {
                return m_transformer.RawImpedanceParameters.X2;
            }
            set
            {
                m_transformer.RawImpedanceParameters.X2 = value;
                OnPropertyChanged("X2");
            }
        }

        public double X3
        {
            get
            {
                return m_transformer.RawImpedanceParameters.X3;
            }
            set
            {
                m_transformer.RawImpedanceParameters.X3 = value;
            }
        }

        public double X4
        {
            get
            {
                return m_transformer.RawImpedanceParameters.X4;
            }
            set
            {
                m_transformer.RawImpedanceParameters.X4 = value;
                OnPropertyChanged("X4");
            }
        }

        public double X5
        {
            get
            {
                return m_transformer.RawImpedanceParameters.X5;
            }
            set
            {
                m_transformer.RawImpedanceParameters.X5 = value;
                OnPropertyChanged("X5");
            }
        }

        public double X6
        {
            get
            {
                return m_transformer.RawImpedanceParameters.X6;
            }
            set
            {
                m_transformer.RawImpedanceParameters.X6 = value;
            }
        }

        #endregion

        #endregion

        #region [ Constructors ]

        public TransformerDetailViewModel()
        {
        }

        public TransformerDetailViewModel(object transformer)
        {
            if (transformer != null && transformer is Transformer)
            {
                m_transformer = transformer as Transformer;

                GetSubstations();
                GetTapConfigurations();
            }
        }

        #endregion

        #region [ Private Methods ]

        private void GetSubstations()
        {
            m_substations = new List<Substation>();

            foreach (Company company in m_transformer.ParentSubstation.ParentDivision.ParentCompany.ParentModel.Companies)
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

        private void GetTapConfigurations()
        {
            m_tapConfigurations = new List<TapConfiguration>();

            foreach (TapConfiguration tapConfiguration in m_transformer.ParentSubstation.ParentDivision.ParentCompany.ParentModel.TapConfigurations)
            {
                m_tapConfigurations.Add(tapConfiguration);
            }
        }

        #endregion
    }
}
