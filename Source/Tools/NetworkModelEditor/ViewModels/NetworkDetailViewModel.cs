//******************************************************************************************************
//  NetworkDetailViewModel.cs
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
//  06/23/2014 - Kevin D. Jones
//       Added 'CurrentFlowPostProcessingSetting' property
//
//******************************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SynchrophasorAnalytics.Networks;

namespace NetworkModelEditor.ViewModels
{
    public class NetworkDetailViewModel : ViewModelBase
    {
        #region [ Private Members ]

        private Network m_network;

        #endregion

        #region [ Properties ]

        public PhaseSelection PhaseConfiguration
        {
            get
            {
                return m_network.PhaseConfiguration;
            }
            set
            {
                m_network.PhaseConfiguration = value;
            }
        }

        public CurrentFlowPostProcessingSetting CurrentFlowPostProcessingSetting
        {
            get
            {
                return m_network.Model.CurrentFlowPostProcessingSetting;
            }
            set
            {
                m_network.Model.CurrentFlowPostProcessingSetting = value;
            }
        }

        public bool AcceptsMeasurements
        {
            get
            {
                return m_network.Model.AcceptsMeasurements;
            }
            set
            {
                m_network.Model.AcceptsMeasurements = value;
            }
        }

        public bool AcceptsEstimates
        {
            get
            {
                return m_network.Model.AcceptsEstimates;
            }
            set
            {
                m_network.Model.AcceptsEstimates = value;
            }
        }

        public bool ReturnsStateEstimate
        {
            get
            {
                return m_network.Model.ReturnsStateEstimate;
            }
            set
            {
                m_network.Model.ReturnsStateEstimate = value;
            }
        }

        public bool ReturnsCurrentFlow
        {
            get
            {
                return m_network.Model.ReturnsCurrentFlow;
            }
            set
            {
                m_network.Model.ReturnsCurrentFlow = value;
            }
        }

        public bool ReturnsVoltageResiduals
        {
            get
            {
                return m_network.Model.ReturnsVoltageResiduals;
            }
            set
            {
                m_network.Model.ReturnsVoltageResiduals = value;
            }
        }

        public bool ReturnsCurrentResiduals
        {
            get
            {
                return m_network.Model.ReturnsCurrentResiduals;
            }
            set
            {
                m_network.Model.ReturnsCurrentResiduals = value;
            }
        }

        public bool ReturnsCircuitBreakerStatus
        {
            get
            {
                return m_network.Model.ReturnsCircuitBreakerStatus;
            }
            set
            {
                m_network.Model.ReturnsCircuitBreakerStatus = value;
            }
        }

        public bool ReturnsSwitchStatus
        {
            get
            {
                return m_network.Model.ReturnsSwitchStatus;
            }
            set
            {
                m_network.Model.ReturnsSwitchStatus = value;
            }
        }
        

        #endregion

        #region [ Constructors ]

        public NetworkDetailViewModel()
        {
        }

        public NetworkDetailViewModel(object network)
        {
            if (network != null && network is Network)
            {
                m_network = network as Network;
            }
        }

        #endregion
    }
}
