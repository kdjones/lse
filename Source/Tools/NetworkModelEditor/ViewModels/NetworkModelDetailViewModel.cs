//******************************************************************************************************
//  NetworkModelDetailViewModel.cs
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
//  07/27/2014 - Kevin D. Jones
//       Generated original version of source code.
//
//******************************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SynchrophasorAnalytics.Networks;
using SynchrophasorAnalytics.Modeling;

namespace NetworkModelEditor.ViewModels
{
    public class NetworkModelDetailViewModel : ViewModelBase
    {
        #region [ Private Members ]

        private NetworkModel m_model;

        #endregion

        #region [ Properties ]

        public int InternalID
        {
            get
            {
                return m_model.InternalID;
            }
            set
            {
                m_model.InternalID = value;
            }
        }

        public int Number
        {
            get
            {
                return m_model.Number;
            }
            set
            {
                m_model.Number = value;
            }
        }

        public string Name
        {
            get
            {
                return m_model.Name;
            }
            set
            {
                m_model.Name = value;
            }
        }

        public string Acronym
        {
            get
            {
                return m_model.Acronym;
            }
            set
            {
                m_model.Acronym = value;
            }
        }

        public string Description
        {
            get
            {
                return m_model.Description;
            }
            set
            {
                m_model.Description = value;
            }
        }

        public PhaseSelection PhaseConfiguration
        {
            get
            {
                return m_model.PhaseConfiguration;
            }
            set
            {
                m_model.PhaseConfiguration = value;
            }
        }

        public CurrentFlowPostProcessingSetting CurrentFlowPostProcessingSetting
        {
            get
            {
                return m_model.CurrentFlowPostProcessingSetting;
            }
            set
            {
                m_model.CurrentFlowPostProcessingSetting = value;
            }
        }

        public CurrentInjectionPostProcessingSetting CurrentInjectionPostProcessingSetting
        {
            get
            {
                return m_model.CurrentInjectionPostProcessingSetting;
            }
            set
            {
                m_model.CurrentInjectionPostProcessingSetting = value;
            }
        }

        public bool AcceptsMeasurements
        {
            get
            {
                return m_model.AcceptsMeasurements;
            }
            set
            {
                m_model.AcceptsMeasurements = value;
            }
        }

        public bool AcceptsEstimates
        {
            get
            {
                return m_model.AcceptsEstimates;
            }
            set
            {
                m_model.AcceptsEstimates = value;
            }
        }

        public bool ReturnsStateEstimate
        {
            get
            {
                return m_model.ReturnsStateEstimate;
            }
            set
            {
                m_model.ReturnsStateEstimate = value;
            }
        }

        public bool ReturnsCurrentFlow
        {
            get
            {
                return m_model.ReturnsCurrentFlow;
            }
            set
            {
                m_model.ReturnsCurrentFlow = value;
            }
        }

        public bool ReturnsCurrentInjection
        {
            get
            {
                return m_model.ReturnsCurrentInjection;
            }
            set
            {
                m_model.ReturnsCurrentInjection = value;
            }
        }

        public bool ReturnsVoltageResiduals
        {
            get
            {
                return m_model.ReturnsVoltageResiduals;
            }
            set
            {
                m_model.ReturnsVoltageResiduals = value;
            }
        }

        public bool ReturnsCurrentResiduals
        {
            get
            {
                return m_model.ReturnsCurrentResiduals;
            }
            set
            {
                m_model.ReturnsCurrentResiduals = value;
            }
        }

        public bool ReturnsCircuitBreakerStatus
        {
            get
            {
                return m_model.ReturnsCircuitBreakerStatus;
            }
            set
            {
                m_model.ReturnsCircuitBreakerStatus = value;
            }
        }

        public bool ReturnsSwitchStatus
        {
            get
            {
                return m_model.ReturnsSwitchStatus;
            }
            set
            {
                m_model.ReturnsSwitchStatus = value;
            }
        }

        public bool ReturnsTapPositions
        {
            get
            {
                return m_model.ReturnsTapPositions;
            }
            set
            {
                m_model.ReturnsTapPositions = value;
            }
        }

        public bool ReturnsSeriesCompensatorStatus
        {
            get
            {
                return m_model.ReturnsSeriesCompensatorStatus;
            }
            set
            {
                m_model.ReturnsSeriesCompensatorStatus = value;
            }
        }

        public bool ReturnsPerformanceMetrics
        {
            get
            {
                return m_model.ReturnsPerformanceMetrics;
            }
            set
            {
                m_model.ReturnsPerformanceMetrics = value;
            }
        }

        public bool ReturnsTopologyProfilingInformation
        {
            get
            {
                return m_model.ReturnsTopologyProfilingInformation;
            }
            set
            {
                m_model.ReturnsTopologyProfilingInformation = value;
            }
        }

        public bool ReturnsMeasurementValidationFlags
        {
            get
            {
                return m_model.ReturnsMeasurementValidationFlags;
            }
            set
            {
                m_model.ReturnsMeasurementValidationFlags = value;
            }
        }

        public string ActiveVoltageCountKey
        {
            get
            {
                return m_model.ParentNetwork.PerformanceMetrics.ActiveVoltageCountKey;
            }
            set
            {
                m_model.ParentNetwork.PerformanceMetrics.ActiveVoltageCountKey = value;
            }
        }

        public string ActiveCurrentFlowCountKey
        {
            get
            {
                return m_model.ParentNetwork.PerformanceMetrics.ActiveCurrentFlowCountKey;
            }
            set
            {
                m_model.ParentNetwork.PerformanceMetrics.ActiveCurrentFlowCountKey = value;
            }
        }

        public string ActiveCurrentInjectionCountKey
        {
            get
            {
                return m_model.ParentNetwork.PerformanceMetrics.ActiveCurrentInjectionCountKey;
            }
            set
            {
                m_model.ParentNetwork.PerformanceMetrics.ActiveCurrentInjectionCountKey = value;
            }
        }

        public string ObservedBusCountKey
        {
            get
            {
                return m_model.ParentNetwork.PerformanceMetrics.ObservedBusCountKey;
            }
            set
            {
                m_model.ParentNetwork.PerformanceMetrics.ObservedBusCountKey = value;
            }
        }

        public string RefreshExecutionTimeKey
        {
            get
            {
                return m_model.ParentNetwork.PerformanceMetrics.RefreshExecutionTimeKey;
            }
            set
            {
                m_model.ParentNetwork.PerformanceMetrics.RefreshExecutionTimeKey = value;
            }
        }

        public string ParsingExecutionTimeKey
        {
            get
            {
                return m_model.ParentNetwork.PerformanceMetrics.ParsingExecutionTimeKey;
            }
            set
            {
                m_model.ParentNetwork.PerformanceMetrics.ParsingExecutionTimeKey = value;
            }
        }

        public string MeasurementMappingExecutionTimeKey
        {
            get
            {
                return m_model.ParentNetwork.PerformanceMetrics.MeasurementMappingExecutionTimeKey;
            }
            set
            {
                m_model.ParentNetwork.PerformanceMetrics.MeasurementMappingExecutionTimeKey = value;
            }
        }

        public string ObservabilityAnalysisExecutionTimeKey
        {
            get
            {
                return m_model.ParentNetwork.PerformanceMetrics.ObservabilityAnalysisExecutionTimeKey;
            }
            set
            {
                m_model.ParentNetwork.PerformanceMetrics.ObservabilityAnalysisExecutionTimeKey = value;
            }
        }

        public string ActiveCurrentPhasorDeterminationExecutionTimeKey
        {
            get
            {
                return m_model.ParentNetwork.PerformanceMetrics.ActiveCurrentPhasorDeterminationExecutionTimeKey;
            }
            set
            {
                m_model.ParentNetwork.PerformanceMetrics.ActiveCurrentPhasorDeterminationExecutionTimeKey = value;
            }
        }

        public string StateComputationExecutionTimeKey
        {
            get
            {
                return m_model.ParentNetwork.PerformanceMetrics.StateComputationExecutionTimeKey;
            }
            set
            {
                m_model.ParentNetwork.PerformanceMetrics.StateComputationExecutionTimeKey = value;
            }
        }

        public string SolutionRetrievalExecutionTimeKey
        {
            get
            {
                return m_model.ParentNetwork.PerformanceMetrics.SolutionRetrievalExecutionTimeKey;
            }
            set
            {
                m_model.ParentNetwork.PerformanceMetrics.SolutionRetrievalExecutionTimeKey = value;
            }
        }

        public string OutputPreparationExecutionTimeKey
        {
            get
            {
                return m_model.ParentNetwork.PerformanceMetrics.OutputPreparationExecutionTimeKey;
            }
            set
            {
                m_model.ParentNetwork.PerformanceMetrics.OutputPreparationExecutionTimeKey = value;
            }
        }

        public string TotalExecutionTimeInTicksKey
        {
            get
            {
                return m_model.ParentNetwork.PerformanceMetrics.TotalExecutionTimeInTicksKey;
            }
            set
            {
                m_model.ParentNetwork.PerformanceMetrics.TotalExecutionTimeInTicksKey = value;
            }
        }

        public string TotalExecutionTimeInMillisecondsKey
        {
            get
            {
                return m_model.ParentNetwork.PerformanceMetrics.TotalExecutionTimeInMillisecondsKey;
            }
            set
            {
                m_model.ParentNetwork.PerformanceMetrics.TotalExecutionTimeInMillisecondsKey = value;
            }
        }

        #endregion

        #region [ Constructors ]

        public NetworkModelDetailViewModel()
        {
        }

        public NetworkModelDetailViewModel(object model)
        {
            if (model != null && model is NetworkModel)
            {
                m_model = model as NetworkModel;
            }
        }

        #endregion
    }
}
