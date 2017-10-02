//******************************************************************************************************
//  MainWindowViewModel.cs
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
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System.Windows.Forms;
using System.Xml.Serialization;
using NetworkModelEditor.Commands;
using SynchrophasorAnalytics.Networks;
using SynchrophasorAnalytics.Matrices;
using SynchrophasorAnalytics.Measurements;
using SynchrophasorAnalytics.Modeling;
using SynchrophasorAnalytics.Testing;
using SynchrophasorAnalytics.Psse;
using SynchrophasorAnalytics.Hdb;
using ECAClientFramework;
using ECACommonUtilities;
using ECACommonUtilities.Model;
using GSF.Configuration;
using GSF.TimeSeries.Transport;
using MeasurementSampler;


namespace NetworkModelEditor.ViewModels
{
    public class MainWindowViewModel : ViewModelBase, IAnalyticHost, IAlgorithmHost
    {
        #region [ Private Members ] 

        #region [ Computational Milestones ] 

        private bool m_networkIsInitialized;
        private bool m_measurementsAreMapped;
        private bool m_activeCurrentFlowsHaveBeenDetermined;
        private bool m_activeCurrentInjectionsHaveBeenDetermined;
        private bool m_observedBussesHaveBeenResolved;
        private bool m_singleFlowBranchesHaveBeenResolved;
        private bool m_stateWasComputed;
        
        #endregion

        #region [ Data ] 

        private Network m_network;
        private List<RawMeasurements> m_measurementSamples;
        private RawMeasurements m_selectedMeasurementSample;
        private List<VoltageLevel> m_retainedVoltageLevels;
        private List<Substation> m_retainedSubstations;
        private List<Company> m_retainedCompanies;
        private OpenEcaConnection m_openEcaConnection;
        private List<SynchrophasorAnalytics.Hdb.Records.NodeExtension> m_nodeExtensions;
        private List<SynchrophasorAnalytics.Hdb.Records.LineSegmentExtension> m_lineSegmentExtensions;
        private List<SynchrophasorAnalytics.Hdb.Records.CircuitBreakerExtension> m_circuitBreakerExtensions;
        private List<SynchrophasorAnalytics.Hdb.Records.ShuntExtension> m_shuntExtensions;
        private List<SynchrophasorAnalytics.Hdb.Records.TransformerExtension> m_transformerExtensions;
        private MeasurementSamplerAnalytic m_measurementSampler;
        private LinearStateEstimatorAnalytic m_linearStateEstimator;

        #endregion

        #region [ Commands ] 

        #region [ File Opening ] 

        private RelayCommand m_openFileCommand;
        private RelayCommand m_openMeasurementSampleFileCommand;
        private RelayCommand m_openPsseRawFileCommand;
        private RelayCommand m_openHdbExportFilesCommand;
        private RelayCommand m_openTvaHdbExportFilesCommand;
        private RelayCommand m_openNodeExtensionFileCommand;
        private RelayCommand m_openLineSegmentExtensionFileCommand;
        private RelayCommand m_openBreakerExtensionFileCommand;
        private RelayCommand m_openShuntExtensionFileCommand;
        private RelayCommand m_openTransformerExtensionFileCommand;

        #endregion

        #region [ File Saving ] 

        private RelayCommand m_saveFileCommand;
        private RelayCommand m_saveMeasurementSampleFilesCommand;
        private RelayCommand m_saveNetworkSnapshotFileCommand;

        #endregion

        #region [ User Interface ] 

        private RelayCommand m_changeSelectedElementCommand;
        private RelayCommand m_viewDetailCommand;
        private RelayCommand m_refreshNetworkTreeCommand;

        #endregion

        #region [ Utilities ] 

        private RelayCommand m_unkeyifyModelCommand;
        private RelayCommand m_keyifyPerformanceMetricsCommand;
        private RelayCommand m_keyifyTopologyMetricsCommand;
        private RelayCommand m_generateMeasurementSamplesCommand;
        private RelayCommand m_pruneModelCommand;
        private RelayCommand m_pruneModelByVoltageLevelCommand;
        private RelayCommand m_pruneModelBySubstationCommand;
        private RelayCommand m_pruneModelByCompanyCommand;

        #endregion

        #region [ Setup ] 

        private RelayCommand m_initializeModelCommand;
        private RelayCommand m_selectMeasurementSampleCommand;
        private RelayCommand m_clearMeasurementsFromModelCommand;
        private RelayCommand m_mapMeasurementsToModelCommand;

        #endregion

        #region [ Observability Analysis ] 

        private RelayCommand m_determineActiveCurrentFlowsCommand;
        private RelayCommand m_determineActiveCurrentInjectionsCommand;
        private RelayCommand m_resolvetoObservedBusesCommand;
        private RelayCommand m_resolveToSingleFlowBranchesCommand;

        #endregion

        #region [ Computation ] 

        private RelayCommand m_computeSystemStateCommand;
        private RelayCommand m_computeLineFlowsCommand;
        private RelayCommand m_computeInjectionsCommand;
        private RelayCommand m_computePowerFlowsCommand;
        private RelayCommand m_computeSequenceComponentsCommand;

        #endregion

        #region [ Matrices ] 

        private RelayCommand m_viewAMatrixCommand;
        private RelayCommand m_viewIIMatrixCommand;
        private RelayCommand m_viewYMatrixCommand;
        private RelayCommand m_viewYsMatrixCommand;
        private RelayCommand m_viewYshMatrixCommand;

        #endregion

        #region [ Inspect ] 

        private RelayCommand m_viewStatusWordsCommand;
        private RelayCommand m_viewMappedStatusWordsCommand;
        private RelayCommand m_viewMappedMissingStatusWordsCommand;
        private RelayCommand m_viewMappedLSEInvalidStatusWordsCommand;
        private RelayCommand m_viewActiveStatusWordsCommand;

        private RelayCommand m_viewReceivedMeasurementsCommand;
        private RelayCommand m_viewUnreceivedMeasurementsCommand;
        private RelayCommand m_viewOutputMeasurementsCommand;
        private RelayCommand m_viewPerformanceMeticsCommand;
        private RelayCommand m_viewVoltageEstimateOutputCommand;
        private RelayCommand m_viewCurrentFlowEstimateOutputCommand;
        private RelayCommand m_viewCurrentInjectionEstimateOutputCommand;
        private RelayCommand m_viewVoltageResidualOutputCommand;
        private RelayCommand m_viewCurrentFlowResidualOutputCommand;
        private RelayCommand m_viewCurrentInjectionResidualOutputCommand;
        private RelayCommand m_viewCircuitBreakerStatusOutputCommand;
        private RelayCommand m_viewSwitchStatusOutputCommand;
        private RelayCommand m_viewTopologyProfilingOutputCommand;
        private RelayCommand m_viewMeasurementValidationFlagOutputCommand;

        private RelayCommand m_viewComponentsCommand;

        private RelayCommand m_viewObservableNodesCommand;
        private RelayCommand m_viewSubstationAdjacencyListCommand;
        private RelayCommand m_viewTransmissionLineAdjacencyListCommand;
        private RelayCommand m_viewCalculatedImpedancesCommand;
        private RelayCommand m_viewSeriesCompensatorInferenceDataCommand;
        private RelayCommand m_viewSeriesCompensatorsCommand;
        private RelayCommand m_viewTransformersCommand;

        private RelayCommand m_viewModeledVoltagesCommand;
        private RelayCommand m_viewExpectedVoltagesCommand;
        private RelayCommand m_viewActiveVoltagesCommand;
        private RelayCommand m_viewInactiveVoltagesCommand;
        private RelayCommand m_viewReportedVoltagesCommand;
        private RelayCommand m_viewUnreportedVoltagesCommand;
        private RelayCommand m_viewActiveVoltagesByStatusWordCommand;
        private RelayCommand m_viewInactiveVoltagesByStatusWordCommand;
        private RelayCommand m_viewInactiveVoltagesByMeasurementCommand;

        private RelayCommand m_viewModeledCurrentFlowsCommand;
        private RelayCommand m_viewExpectedCurrentFlowsCommand;
        private RelayCommand m_viewActiveCurrentFlowsCommand;
        private RelayCommand m_viewInactiveCurrentFlowsCommand;
        private RelayCommand m_viewIncludedCurrentFlowsCommand;
        private RelayCommand m_viewExcludedCurrentFlowsCommand;
        private RelayCommand m_viewReportedCurrentFlowsCommand;
        private RelayCommand m_viewUnreportedCurrentFlowsCommand;
        private RelayCommand m_viewActiveCurrentFlowsByStatusWordCommand;
        private RelayCommand m_viewInactiveCurrentFlowsByStatusWordCommand;
        private RelayCommand m_viewInactiveCurrentFlowsByMeasurementCommand;

        private RelayCommand m_viewModeledCurrentInjectionsCommand;
        private RelayCommand m_viewExpectedCurrentInjectionsCommand;
        private RelayCommand m_viewActiveCurrentInjectionsCommand;
        private RelayCommand m_viewInactiveCurrentInjectionsCommand;
        private RelayCommand m_viewReportedCurrentInjectionsCommand;
        private RelayCommand m_viewUnreportedCurrentInjectionsCommand;
        private RelayCommand m_viewActiveCurrentInjectionsByStatusWordCommand;
        private RelayCommand m_viewInactiveCurrentInjectionsByStatusWordCommand;
        private RelayCommand m_viewInactiveCurrentInjectionsByMeasurementCommand;

        private RelayCommand m_viewModeledBreakerStatusesCommand;
        private RelayCommand m_viewExpectedBreakerStatusesCommand;
        private RelayCommand m_viewReportedBreakerStatusesCommand;
        private RelayCommand m_viewCircuitBreakersCommand;
        private RelayCommand m_viewSwitchesCommand;
        private RelayCommand m_viewAllSwitchingDevicesCommand;


        #endregion

        #region [ openECA ]

        private RelayCommand m_connectToOpenEcaCommand;
        private RelayCommand m_refreshMetaDataCommand;
        private RelayCommand m_startMeasurementSamplerCommand;
        private RelayCommand m_stopMeasurementSamplerCommand;
        private RelayCommand m_takeMeasurementSampleCommand;
        private RelayCommand m_createStatusFlagsCommand;
        private RelayCommand m_createVoltageEstimateMeasurementsCommand;
        private RelayCommand m_createCurrentFlowEstimateMeasurementsCommand;
        private RelayCommand m_createCurrentInjectionEstimateMeasurementsCommand;
        private RelayCommand m_createVoltageResidualMeasurementsCommand;
        private RelayCommand m_createCurrentFlowResidualMeasurementsCommand;
        private RelayCommand m_createCurrentInjectionResidualMeasurementsCommand;
        private RelayCommand m_createCircuitBreakerStatusMeasurementsCommand;
        private RelayCommand m_createSwitchStatusMeasurementsCommand;
        private RelayCommand m_createPerformanceMetricMeasurementsCommand;
        private RelayCommand m_createTopologyProfilingMeasurementsCommand;
        private RelayCommand m_createMeasurementValidationFlagMeasurementsCommand;

        #endregion

        #endregion

        #region [ View Models ] 

        private List<MenuItemViewModel> m_menuBarItemViewModels;
        private NetworkTreeViewModel m_networkTreeViewModel;
        private RecordDetailViewModel m_recordDetailViewModel;
        private MeasurementSampleDetailViewModel m_measurementSampleViewModel;

        #endregion

        #region [ Menu Bar Items ]

        #region [ FILE ]
        private MenuItemViewModel m_fileMenuItem;

        private MenuItemViewModel m_openMenuItem;
        private MenuItemViewModel m_saveMenuItem;
        private MenuItemViewModel m_exitMenuItem;

        #endregion

        #region [ UTILITIES ] 

        private MenuItemViewModel m_utilitiesMenuItem;

        private MenuItemViewModel m_modelActionsMenuItem;
        private MenuItemViewModel m_unkeyifyModelMenuItem;
        private MenuItemViewModel m_keyifyPerformanceMetricsMenuItem;
        private MenuItemViewModel m_keyifyTopologyMetricsMenuItem;

        private MenuItemViewModel m_createMeasurementsMenuItem;
        private MenuItemViewModel m_createStatusFlagsMenuItem;
        private MenuItemViewModel m_createVoltageEstimateMeasurementsMenuItem;
        private MenuItemViewModel m_createCurrentFlowEstimateMeasurementsMenuItem;
        private MenuItemViewModel m_createCurrentInjectionEstimateMeasurementsMenuItem;
        private MenuItemViewModel m_createVoltageResidualMeasurementsMenuItem;
        private MenuItemViewModel m_createCurrentFlowResidualMeasurementsMenuItem;
        private MenuItemViewModel m_createCurrentInjectionResidualMeasurementsMenuItem;
        private MenuItemViewModel m_createCircuitBreakerStatusMeasurementsMenuItem;
        private MenuItemViewModel m_createSwitchStatusMeasurementsMenuItem;
        private MenuItemViewModel m_createPerformanceMetricsMeasurementsMenuItem;
        private MenuItemViewModel m_createTopologyProfilingMeasurementsMenuItem;
        private MenuItemViewModel m_createMeasurementValidationFlagMeasurementsMenuItem;

        private MenuItemViewModel m_mapCreatedMeasurementsMenuItem;
        private MenuItemViewModel m_mapVoltageEstimateMeasurementsMenuItem;
        private MenuItemViewModel m_mapCurrentFlowEstimateMeasurementsMenuItem;
        private MenuItemViewModel m_mapCurrentInjectionEstimateMeasurementsMenuItem;
        private MenuItemViewModel m_mapVoltageResidualMeasurementsMenuItem;
        private MenuItemViewModel m_mapCurrentFlowResidualMeasurementsMenuItem;
        private MenuItemViewModel m_mapCurrentInjectionResidualMeasurementsMenuItem;
        private MenuItemViewModel m_mapCircuitBreakerStatusMeasurementsMenuItem;
        private MenuItemViewModel m_mapSwitchStatusMeasurementsMenuItem;
        private MenuItemViewModel m_mapPerformanceMetricsMeasurementsMenuItem;
        private MenuItemViewModel m_mapTopologyProfilingMeasurementsMenuItem;
        private MenuItemViewModel m_mapMeasurementValidationFlagMeasurementsMenuItem;


        private MenuItemViewModel m_pruneModelMenuItem;
        private MenuItemViewModel m_pruneModelByVoltageLevelMenuItem;
        private MenuItemViewModel m_pruneModelBySubstationMenuItem;
        private MenuItemViewModel m_pruneModelByCompanyMenuItem;

        private MenuItemViewModel m_measurementActionsMenuItem;
        private MenuItemViewModel m_generateMeasurementSamplesMenuItem;

        #endregion

        #region [ OPENECA ]

        private MenuItemViewModel m_openEcaMenuItem;

        private MenuItemViewModel m_connectMenuItem;
        private MenuItemViewModel m_refreshMetaDataMenuItem;
        private MenuItemViewModel m_measurementSamplerMenuItem;
        private MenuItemViewModel m_startMeasurementSamplerMenuItem;
        private MenuItemViewModel m_stopMeasurementSamplerMenuItem;
        private MenuItemViewModel m_linearStateEstimatorMenuItem;
        private MenuItemViewModel m_startLinearStateEstimatorMenuItem;
        private MenuItemViewModel m_stopLinearStateEstimatorMenuItem;
        private MenuItemViewModel m_takeMeasurementSampleMenuItem;

        #endregion

        #region [ OFFLINE ANALYSIS ]

        private MenuItemViewModel m_offlineAnalysisMenuItem;

        private MenuItemViewModel m_setupMenuItem;
        private MenuItemViewModel m_observabilityMenuItem;
        private MenuItemViewModel m_matricesMenuItem;
        private MenuItemViewModel m_computationMenuItem;
        private MenuItemViewModel m_inspectMenuItem;

        private MenuItemViewModel m_openNetworkModelMenuItem;
        private MenuItemViewModel m_openMeasurementSampleMenuItem;
        private MenuItemViewModel m_openPsseRawFileMenuItem;
        private MenuItemViewModel m_openHdbExportFilesMenuItem;
        private MenuItemViewModel m_openTvaHdbExportFilesMenuItem;
        private MenuItemViewModel m_openExtensionFileMenuItem;
        private MenuItemViewModel m_openNodeExtensionFileMenuItem;
        private MenuItemViewModel m_openLineSegmentExtensionFileMenuItem;
        private MenuItemViewModel m_openBreakerExtensionFileMenuItem;
        private MenuItemViewModel m_openShuntExtensionFileMenuItem;
        private MenuItemViewModel m_openTransformerExtensionFileMenuItem;

        private MenuItemViewModel m_saveNetworkModelMenuItem;
        private MenuItemViewModel m_saveNetworkSnapshotMenuItem;
        private MenuItemViewModel m_saveMeasurementSampleFileMenuItem;

        private MenuItemViewModel m_clearMeasurementsMenuItem;
        private MenuItemViewModel m_initializeModelMenuItem;
        private MenuItemViewModel m_mapMeasurementsMenuItem;

        private MenuItemViewModel m_determineActiveCurrentFlowsMenuItem;
        private MenuItemViewModel m_determineActiveCurrentInjectionsMenuItem;
        private MenuItemViewModel m_resolveToObservedBusesMenuItem;
        private MenuItemViewModel m_resolveToSingleFlowBranchesMenuItem;
        
        private MenuItemViewModel m_viewAMatrixMenuItem;
        private MenuItemViewModel m_viewIIMatrixMenuItem;
        private MenuItemViewModel m_viewYMatrixMenuItem;
        private MenuItemViewModel m_viewYsMatrixMenuItem;
        private MenuItemViewModel m_viewYshMatrixMenuItem;

        private MenuItemViewModel m_computeSystemStateMenuItem;
        private MenuItemViewModel m_computeLineFlowsMenuItem;
        private MenuItemViewModel m_computeInjectionsMenuItem;
        private MenuItemViewModel m_computePowerFlowsMenuItem;
        private MenuItemViewModel m_computeSequenceComponentsMenuItem;

        private MenuItemViewModel m_viewComponentSummaryMenuItem;

        private MenuItemViewModel m_viewStatusWordsMenuItem;
        private MenuItemViewModel m_viewAllStatusWordsMenuItem;
        private MenuItemViewModel m_viewMappedStatusWordsMenuItem;
        private MenuItemViewModel m_viewMappedLSEInvalidStatusWordsMenuItem;
        private MenuItemViewModel m_viewMappedMissingStatusWordsMenuItem;
        private MenuItemViewModel m_viewActiveStatusWordsMenuItem;

        private MenuItemViewModel m_viewMeasurementsMenuItem;
        private MenuItemViewModel m_viewReceivedMeasurementsMenuItem;
        private MenuItemViewModel m_viewUnreceivedMeasurementsMenuItem;

        private MenuItemViewModel m_viewOutputMeasurementsMenuItem;
        private MenuItemViewModel m_viewAllOutputMeasurementsMenuItem;
        private MenuItemViewModel m_viewPerformanceMetricsOutputMenuItem;
        private MenuItemViewModel m_viewTopologyProfilingOutputMenuItem;
        private MenuItemViewModel m_viewMeasurementValidationFlagsOutputMenuItem;
        private MenuItemViewModel m_viewStateEstimateOutputMenuItem;
        private MenuItemViewModel m_viewCurrentFlowEstimateOutputMenuItem;
        private MenuItemViewModel m_viewCurrentInjectionEstimateOutputMenuItem;
        private MenuItemViewModel m_viewVoltageResidualOutputMenuItem;
        private MenuItemViewModel m_viewCurrentFlowResidualOutputMenuItem;
        private MenuItemViewModel m_viewCurrentInjectionResidualOutputMenuItem;
        private MenuItemViewModel m_viewCircuitBreakerStatusOutputMenuItem;
        private MenuItemViewModel m_viewSwitchStatusOutputMenuItem;
        private MenuItemViewModel m_viewTapPositionOutputMenuItem;
        private MenuItemViewModel m_viewSeriesCompensatorStatusOutputMenuItem;

        private MenuItemViewModel m_viewVoltagePhasorsMenuItem;
        private MenuItemViewModel m_viewModeledVoltagesMenuItem;
        private MenuItemViewModel m_viewExpectedVoltagesMenuItem;
        private MenuItemViewModel m_viewActiveVoltagesMenuItem;
        private MenuItemViewModel m_viewInactiveVoltagesMenuItem;
        private MenuItemViewModel m_viewReportedVoltagesMenuItem;
        private MenuItemViewModel m_viewUnreportedVoltagesMenuItem;
        private MenuItemViewModel m_viewActiveVoltagesByStatusWordMenuItem;
        private MenuItemViewModel m_viewInactiveVoltagesByStatusWordMenuItem;
        private MenuItemViewModel m_viewInactiveVoltagesByMeasurementMenuItem;

        private MenuItemViewModel m_viewCurrentFlowPhasorsMenuItem;
        private MenuItemViewModel m_viewModeledCurrentFlowsMenuItem;
        private MenuItemViewModel m_viewExpectedCurrentFlowsMenuItem;
        private MenuItemViewModel m_viewActiveCurrentFlowsMenuItem;
        private MenuItemViewModel m_viewInactiveCurrentFlowsMenuItem;
        private MenuItemViewModel m_viewIncludedCurrentFlowsMenuItem;
        private MenuItemViewModel m_viewExcludedCurrentFlowsMenuItem;
        private MenuItemViewModel m_viewReportedCurrentFlowsMenuItem;
        private MenuItemViewModel m_viewUnreportedCurrentFlowsMenuItem;
        private MenuItemViewModel m_viewActiveCurrentFlowsByStatusWordMenuItem;
        private MenuItemViewModel m_viewInactiveCurrentFlowsByStatusWordMenuItem;
        private MenuItemViewModel m_viewInactiveCurrentFlowsByMeasurementMenuItem;

        private MenuItemViewModel m_viewCurrentInjectionPhasorsMenuItem;
        private MenuItemViewModel m_viewModeledCurrentInjectionsMenuItem;
        private MenuItemViewModel m_viewExpectedCurrentInjectionsMenuItem;
        private MenuItemViewModel m_viewActiveCurrentInjectionsMenuItem;
        private MenuItemViewModel m_viewInactiveCurrentInjectionsMenuItem;
        private MenuItemViewModel m_viewReportedCurrentInjectionsMenuItem;
        private MenuItemViewModel m_viewUnreportedCurrentInjectionsMenuItem;
        private MenuItemViewModel m_viewActiveCurrentInjectionsByStatusWordMenuItem;
        private MenuItemViewModel m_viewInactiveCurrentInjectionsByStatusWordMenuItem;
        private MenuItemViewModel m_viewInactiveCurrentInjectionsByMeasurementMenuItem;


        private MenuItemViewModel m_viewObservableNodesMenuItem;
        private MenuItemViewModel m_viewSubstationAdjacencyListMenuItem;
        private MenuItemViewModel m_viewTransmissionLineAdjacencyListMenuItem;
        private MenuItemViewModel m_viewCalculatedImpedancesMenuItem;
        private MenuItemViewModel m_viewSeriesCompensatorInferenceDataMenuItem;
        private MenuItemViewModel m_viewSeriesCompensatorsMenuItem;
        private MenuItemViewModel m_viewTransformersMenuItem;

        private MenuItemViewModel m_viewSwitchingDeviceStatusesMenuItem;
        private MenuItemViewModel m_viewModeledBreakerStatusesMenuItem;
        private MenuItemViewModel m_viewExpectedBreakerStatusesMenuItem;
        private MenuItemViewModel m_viewReportedBreakerStatusesMenuItem;
        private MenuItemViewModel m_viewCircuitBreakersMenuItem;
        private MenuItemViewModel m_viewSwitchesMenuItem;
        private MenuItemViewModel m_viewAllSwitchingDevicesMenuItem;

        #endregion

        #endregion

        #region [ Status Bar ] 

        private string m_actionStatus;
        private string m_communicationStatus;
        private string m_specialStatus;

        #endregion

        #endregion

        #region [ Public Properties ] 

        #region [ View Models ] 

        public NetworkTreeViewModel NetworkTree
        {
            get
            {
                return m_networkTreeViewModel;
            }
            set
            {
                m_networkTreeViewModel = value;
            }
        }

        public RecordDetailViewModel RecordDetail
        {
            get
            {
                return m_recordDetailViewModel;
            }
            set
            {
                m_recordDetailViewModel = value;
            }
        }

        public ObservableCollection<MenuItemViewModel> MenuBarItems
        {
            get
            {
                return new ObservableCollection<MenuItemViewModel>(m_menuBarItemViewModels);
            }
        }

        public MeasurementSampleDetailViewModel MeasurementSample
        {
            get
            {
                return m_measurementSampleViewModel;
            }
            set
            {
                m_measurementSampleViewModel = value;
            }
        }

        #endregion

        #region [ Status Bar ]

        public string ActionStatus
        {
            get
            {
                return m_actionStatus;
            }
            set
            {
                m_actionStatus = value;
                OnPropertyChanged("ActionStatus");
            }
        }

        public string CommunicationStatus
        {
            get
            {
                return m_communicationStatus;
            }
            set
            {
                m_communicationStatus = value;
                OnPropertyChanged("CommunicationStatus");
            }
        }

        public string SpecialStatus
        {
            get
            {
                return m_specialStatus;
            }
            set
            {
                m_specialStatus = value;
                OnPropertyChanged("SpecialStatus");
            }
        }

        #endregion

        #region [ Data ]

        public RawMeasurements SelectedMeasurementSample
        {
            get
            {
                return m_selectedMeasurementSample;
            }
            set
            {
                SpecialStatus = $"Currently selected 'Sample {value.Identifier}'";
                m_selectedMeasurementSample = value;
            }
        }

        public List<Substation> RetainedSubstations
        {
            get
            {
                return m_retainedSubstations;
            }
            set
            {
                m_retainedSubstations = value;
            }
        }

        public List<VoltageLevel> RetainedVoltageLevels
        {
            get
            {
                return m_retainedVoltageLevels;
            }
            set
            {
                m_retainedVoltageLevels = value;
            }
        }

        public List<Company> RetainedCompanies
        {
            get
            {
                return m_retainedCompanies;
            }
            set
            {
                m_retainedCompanies = value;
            }
        }

        public OpenEcaConnection OpenEcaConnection
        {
            get
            {
                return m_openEcaConnection;
            }
        }

        public MeasurementSamplerAnalytic MeasurementSampler
        {
            get
            {
                return m_measurementSampler;
            }
        }

        public LinearStateEstimatorAnalytic LinearStateEstimator
        {
            get
            {
                return m_linearStateEstimator;
            }
        }

        #endregion

        #region [ Commands ]

        public ICommand OpenFileCommand
        {
            get 
            {
                if (m_openFileCommand == null)
                {
                    m_openFileCommand = new RelayCommand(param => this.OpenFile(), param => true);
                }
                return m_openFileCommand;
            }
        }

        public ICommand OpenMeasurementSampleFileCommand
        {
            get
            {
                if (m_openMeasurementSampleFileCommand == null)
                {
                    m_openMeasurementSampleFileCommand = new RelayCommand(param => this.OpenMeasurementSampleFile(), param => true);
                }
                return m_openMeasurementSampleFileCommand;
            }
        }

        public ICommand OpenPsseRawFileCommand
        {
            get
            {
                if (m_openPsseRawFileCommand == null)
                {
                    m_openPsseRawFileCommand = new RelayCommand(param => this.OpenPsseRawFile(), param => true);
                }
                return m_openPsseRawFileCommand;
            }
        }

        public ICommand OpenHdbExportFilesCommand
        {
            get
            {
                if (m_openHdbExportFilesCommand == null)
                {
                    m_openHdbExportFilesCommand = new RelayCommand(param => this.OpenHdbExportFiles(), param => true);
                }
                return m_openHdbExportFilesCommand;
            }

        }

        public ICommand OpenTvaHdbExportFilesCommand
        {
            get
            {
                if (m_openTvaHdbExportFilesCommand == null)
                {
                    m_openTvaHdbExportFilesCommand = new RelayCommand(param => this.OpenTvaHdbExportFiles(), param => true);
                }
                return m_openTvaHdbExportFilesCommand;
            }
        }

        public ICommand OpenNodeExtensionFileCommand
        {
            get
            {
                if (m_openNodeExtensionFileCommand == null)
                {
                    m_openNodeExtensionFileCommand = new RelayCommand(param => this.OpenNodeExtensionFile(), param => true);
                }
                return m_openNodeExtensionFileCommand;
            }
        }

        public ICommand OpenLineSegmentExtensionFileCommand
        {
            get
            {
                if (m_openLineSegmentExtensionFileCommand == null)
                {
                    m_openLineSegmentExtensionFileCommand = new RelayCommand(param => this.OpenLineSegmentExtensionFile(), param => true);
                }
                return m_openLineSegmentExtensionFileCommand;
            }
        }

        public ICommand OpenBreakerExtensionFileCommand
        {
            get
            {
                if (m_openBreakerExtensionFileCommand == null)
                {
                    m_openBreakerExtensionFileCommand = new RelayCommand(param => this.OpenBreakerExtensionFile(), param => true);
                }
                return m_openBreakerExtensionFileCommand;
            }
        }

        public ICommand OpenShuntExtensionFileCommand
        {
            get
            {
                if (m_openShuntExtensionFileCommand == null)
                {
                    m_openShuntExtensionFileCommand = new RelayCommand(param => this.OpenShuntExtensionFile(), param => true);
                }
                return m_openShuntExtensionFileCommand;
            }
        }

        public ICommand OpenTransformerExtensionFileCommand
        {
            get
            {
                if (m_openTransformerExtensionFileCommand == null)
                {
                    m_openTransformerExtensionFileCommand = new RelayCommand(param => this.OpenTransformerExtensionFile(), param => true);
                }
                return m_openTransformerExtensionFileCommand;
            }
        }

        public ICommand SaveFileCommand
        {
            get
            {
                if (m_saveFileCommand == null)
                {
                    m_saveFileCommand = new RelayCommand(param => this.SaveFile(), param => true);
                }
                return m_saveFileCommand;
            }
        }

        public ICommand ChangeSelectedElementCommand
        {
            get
            {
                if (m_changeSelectedElementCommand == null)
                {
                    m_changeSelectedElementCommand = new RelayCommand(param => this.ChangeSelectedElement(), param => true);
                }
                return m_changeSelectedElementCommand;
            }
        }

        public ICommand ViewDetailCommand
        {
            get
            {
                if (m_viewDetailCommand == null)
                {
                    m_viewDetailCommand = new RelayCommand(param => this.ViewDetail(), param => true);
                }
                return m_viewDetailCommand;
            }
        }

        public ICommand UpdateTreeViewCommand
        {
            get
            {
                if (m_refreshNetworkTreeCommand == null)
                {
                    m_refreshNetworkTreeCommand = new RelayCommand(param => this.RefreshNetworkTree(), param => true);
                }
                return m_refreshNetworkTreeCommand;
            }
        }

        public ICommand UnkeyifyModelCommand
        {
            get
            {
                if (m_unkeyifyModelCommand == null)
                {
                    m_unkeyifyModelCommand = new RelayCommand(param => this.UnkeyifyModel(), param => true);
                }
                return m_unkeyifyModelCommand;
            }
        }

        public ICommand KeyifyPerformanceMetricsCommand
        {
            get
            {
                if (m_keyifyPerformanceMetricsCommand == null)
                {
                    m_keyifyPerformanceMetricsCommand = new RelayCommand(param => this.KeyifyPerformanceMetrics(), param => true);
                }
                return m_keyifyPerformanceMetricsCommand;
            }
        }

        public ICommand KeyifyTopologyMetricsCommand
        {
            get
            {
                if (m_keyifyTopologyMetricsCommand == null)
                {
                    m_keyifyTopologyMetricsCommand = new RelayCommand(param => this.KeyifyTopologyMetrics(), param => true);
                }
                return m_keyifyTopologyMetricsCommand;
            }
        }

        public ICommand PruneModelCommand
        {
            get
            {
                if (m_pruneModelCommand == null)
                {
                    m_pruneModelCommand = new RelayCommand(param => this.PruneModel(), param => true);
                }
                return m_pruneModelCommand;
            }
        }

        public ICommand PruneModelByVoltageLevelCommand
        {
            get
            {
                if (m_pruneModelByVoltageLevelCommand == null)
                {
                    m_pruneModelByVoltageLevelCommand = new RelayCommand(param => this.PruneModelByVoltageLevel(), param => true);
                }
                return m_pruneModelByVoltageLevelCommand;
            }
        }

        public ICommand PruneModelBySubstationCommand
        {
            get
            {
                if (m_pruneModelBySubstationCommand == null)
                {
                    m_pruneModelBySubstationCommand = new RelayCommand(param => this.PruneModelBySubstation(), param => true);
                }
                return m_pruneModelBySubstationCommand;
            }
        }

        public ICommand PruneModelByCompanyCommand
        {
            get
            {
                if (m_pruneModelByCompanyCommand == null)
                {
                    m_pruneModelByCompanyCommand = new RelayCommand(param => this.PruneModelByCompany(), param => true);
                }
                return m_pruneModelByCompanyCommand;
            }
        }

        public ICommand ConnectToOpenEcaCommand
        {
            get
            {
                if (m_connectToOpenEcaCommand == null)
                {
                    m_connectToOpenEcaCommand = new RelayCommand(param => this.ConnectToOpenEca(), param => true);
                }
                return m_connectToOpenEcaCommand;
            }
        }

        public ICommand CreateStatusFlagsCommand
        {
            get
            {
                if (m_createStatusFlagsCommand == null)
                {
                    m_createStatusFlagsCommand = new RelayCommand(param => this.CreateStatusFlags(), param => true);
                }
                return m_createStatusFlagsCommand;
            }
        }

        public ICommand StartMeasurementSamplerCommand
        {
            get
            {
                if (m_startMeasurementSamplerCommand == null)
                {
                    m_startMeasurementSamplerCommand = new RelayCommand(param => this.StartMeasurementSampler(), param => true);
                }
                return m_startMeasurementSamplerCommand;
            }
        }

        public ICommand StopMeasurementSamplerCommand
        {
            get
            {
                if (m_stopMeasurementSamplerCommand == null)
                {
                    m_stopMeasurementSamplerCommand = new RelayCommand(param => this.StopMeasurementSampler(), param => true);
                }
                return m_stopMeasurementSamplerCommand;
            }
        }

        public ICommand StartLinearStateEstimatorCommand
        {
            get
            {
                return new RelayCommand(param => this.StartLinearStateEstimator(), param => true);
            }
        }

        public ICommand StopLinearStateEstimatorCommand
        {
            get
            {
                return new RelayCommand(param => this.StopLinearStateEstimator(), param => true);
            }
        }

        public ICommand TakeMeasurementSampleCommand
        {
            get
            {
                if (m_takeMeasurementSampleCommand == null)
                {
                    m_takeMeasurementSampleCommand = new RelayCommand(param => this.TakeSample(), param => true);
                }
                return m_takeMeasurementSampleCommand;
            }
        }

        public ICommand ViewPerformanceMetricsCommand
        {
            get
            {
                if (m_viewPerformanceMeticsCommand == null)
                {
                    m_viewPerformanceMeticsCommand = new RelayCommand(param => this.ViewPerformanceMetrics(), param => true);
                }
                return m_viewPerformanceMeticsCommand;
            }
        }

        public ICommand RefreshMetaDataCommand
        {
            get
            {
                if (m_refreshMetaDataCommand == null)
                {
                    m_refreshMetaDataCommand = new RelayCommand(param => this.RefreshMetaData(), param => true);
                }
                return m_refreshMetaDataCommand;
            }
        }

        public ICommand ViewVoltageEstimateOutputCommand
        {
            get
            {
                if (m_viewVoltageEstimateOutputCommand == null)
                {
                    m_viewVoltageEstimateOutputCommand = new RelayCommand(param => this.ViewVoltageEstimates(), param => true);
                }
                return m_viewVoltageEstimateOutputCommand;
            }
        }

        public ICommand ViewCurrentFlowEstimateOutputCommand
        {
            get
            {
                if (m_viewCurrentFlowEstimateOutputCommand == null)
                {
                    m_viewCurrentFlowEstimateOutputCommand = new RelayCommand(param => this.ViewCurrentFlowEstimates(), param => true);
                }
                return m_viewCurrentFlowEstimateOutputCommand;
            }
        }

        public ICommand ViewCurrentInjectionEstimateOutputCommand
        {
            get
            {
                if (m_viewCurrentInjectionEstimateOutputCommand == null)
                {
                    m_viewCurrentInjectionEstimateOutputCommand = new RelayCommand(param => this.ViewCurrentInjectionEstimates(), param => true);
                }
                return m_viewCurrentInjectionEstimateOutputCommand;
            }
        }

        public ICommand ViewVoltageResidualOutputCommand
        {
            get
            {
                if (m_viewVoltageResidualOutputCommand == null)
                {
                    m_viewVoltageResidualOutputCommand = new RelayCommand(param => this.ViewVoltageResiduals(), param => true);
                }
                return m_viewVoltageResidualOutputCommand;
            }
        }

        public ICommand ViewCurrentFlowResidualOutputCommand
        {
            get
            {
                if (m_viewCurrentFlowResidualOutputCommand == null)
                {
                    m_viewCurrentFlowResidualOutputCommand = new RelayCommand(param => this.ViewCurrentFlowResiduals(), param => true);
                }
                return m_viewCurrentFlowResidualOutputCommand;
            }
        }

        public ICommand ViewCurrentInjectionResidualOutputCommand
        {
            get
            {
                if (m_viewCurrentInjectionResidualOutputCommand == null)
                {
                    m_viewCurrentInjectionResidualOutputCommand = new RelayCommand(param => this.ViewCurrentInjectionResiduals(), param => true);
                }
                return m_viewCurrentInjectionResidualOutputCommand;
            }
        }

        public ICommand ViewCircuitBreakerStatusOutputCommand
        {
            get
            {
                if (m_viewCircuitBreakerStatusOutputCommand == null)
                {
                    m_viewCircuitBreakerStatusOutputCommand = new RelayCommand(param => this.ViewCircuitBreakerStatuses(), param => true);
                }
                return m_viewCircuitBreakerStatusOutputCommand;
            }
        }

        public ICommand ViewSwitchStatusOutputCommand
        {
            get
            {
                if (m_viewSwitchStatusOutputCommand == null)
                {
                    m_viewSwitchStatusOutputCommand = new RelayCommand(param => this.ViewSwitchStatuses(), param => true);
                }
                return m_viewSwitchStatusOutputCommand;
            }
        }

        public ICommand ViewTopologyProfilingOutputCommand
        {
            get
            {
                if (m_viewTopologyProfilingOutputCommand == null)
                {
                    m_viewTopologyProfilingOutputCommand = new RelayCommand(param => this.ViewTopologyProfiling(), param => true);
                }
                return m_viewTopologyProfilingOutputCommand;
            }
        }

        public ICommand ViewMeasurementValidationFlagOutputCommand
        {
            get
            {
                if (m_viewMeasurementValidationFlagOutputCommand == null)
                {
                    m_viewMeasurementValidationFlagOutputCommand = new RelayCommand(param => this.ViewMeasurementValidationFlags(), param => true);
                }
                return m_viewMeasurementValidationFlagOutputCommand;
            }
        }
        
        public ICommand CreateVoltageEstimateMeasurementsCommand
        {
            get
            {
                if (m_createVoltageEstimateMeasurementsCommand == null)
                {
                    m_createVoltageEstimateMeasurementsCommand = new RelayCommand(param => this.CreateVoltageEstimates(), param => true);
                }
                return m_createVoltageEstimateMeasurementsCommand;
            }
        }

        public ICommand CreateCurrentFlowEstimateMeasurementsCommand
        {
            get
            {
                return new RelayCommand(param => this.CreateCurrentFlowEstimates(), param => true);
            }
        }

        public ICommand CreateCurrentInjectionEstimateMeasurementsCommand
        {
            get
            {
                return new RelayCommand(param => this.CreateCurrentInjectionEstimates(), param => true);
            }
        }

        public ICommand CreateVoltageResidualMeasurementsCommand
        {
            get
            {
                return new RelayCommand(param => this.CreateVoltageResiduals(), param => true);
            }
        }

        public ICommand CreateCurrentFlowResidualMeasurementsCommand
        {
            get
            {
                return new RelayCommand(param => this.CreateCurrentFlowResiduals(), param => true);
            }
        }

        //public ICommand CreateCurrentInjectionResidualMeasurementsCommand
        //{
        //    get
        //    {
        //        return new RelayCommand(param => this.CreateCurrentInjectionRes)
        //    }
        //}

        public ICommand CreateCircuitBreakerStatusMeasurementsCommand
        {
            get
            {
                return new RelayCommand(param => this.CreateCircuitBreakerStatuses(), param => true);
            }
        }

        public ICommand CreateSwitchStatusMeasurementsCommand
        {
            get
            {
                return new RelayCommand(param => this.CreateSwitchStatuses(), param => true);
            }
        }

        public ICommand CreatePerformanceMetricMeasurementsCommand
        {
            get
            {
                if (m_createPerformanceMetricMeasurementsCommand == null)
                {
                    m_createPerformanceMetricMeasurementsCommand = new RelayCommand(param => this.CreatePerformanceMetrics(), param => true);
                }
                return m_createPerformanceMetricMeasurementsCommand;
            }
        }

        public ICommand CreateTopologyProfilingMeasurementsCommand
        {
            get
            {
                return new RelayCommand(param => this.CreateTopologyProfiling(), param => true);
            }
        }

        public ICommand CreateMeasurementValidationFlagMeasurementsCommand
        {
            get
            {
                return new RelayCommand(param => this.CreateMeasurementValidationFlags(), param => true);
            }
        }

        public ICommand MapVoltageEstimateMeasurementsCommand
        {
            get
            {
                return new RelayCommand(param => this.MapVoltageEstimates(), param => true);
            }
        }

        public ICommand MapCurrentFlowEstimateMeasurementsCommand
        {
            get
            {
                return new RelayCommand(param => this.MapCurrentFlowEstimates(), param => true);
            }
        }

        public ICommand MapCurrentInjectionEstimateMeasurementsCommand
        {
            get
            {
                return new RelayCommand(param => this.MapCurrentInjectionEstimates(), param => true);
            }
        }

        public ICommand MapVoltageResidualMeasurementsCommand
        {
            get
            {
                return new RelayCommand(param => this.MapVoltageResiduals(), param => true);
            }
        }

        public ICommand MapCurrentFlowResidualMeasurementsCommand
        {
            get
            {
                return new RelayCommand(param => this.MapCurrentResiduals(), param => true);
            }
        }

        //public ICommand MapCurrentInjectionResidualMeasurementsCommand
        //{
        //    get
        //    {
        //        return new RelayCommand(param => this.MapCurrentInjectionRes)
        //    }
        //}

        public ICommand MapCircuitBreakerStatusMeasurementsCommand
        {
            get
            {
                return new RelayCommand(param => this.MapCircuitBreakerStatuses(), param => true);
            }
        }

        public ICommand MapSwitchStatusMeasurementsCommand
        {
            get
            {
                return new RelayCommand(param => this.MapSwitchStatuses(), param => true);
            }
        }

        public ICommand MapPerformanceMetricMeasurementsCommand
        {
            get
            {
                return new RelayCommand(param => this.MapPerformanceMetrics(), param => true);
            }
        }

        public ICommand MapTopologyProfilingMeasurementsCommand
        {
            get
            {
                return new RelayCommand(param => this.MapTopologyProfiling(), param => true);
            }
        }

        public ICommand MapMeasurementValidationFlagMeasurementsCommand
        {
            get
            {
                return new RelayCommand(param => this.MapMeasurementValidationFlags(), param => true);
            }
        }
        
        #region [ Setup Commands ] 

        public ICommand InitializeModelCommand
        {
            get
            {
                if (m_initializeModelCommand == null)
                {
                    m_initializeModelCommand = new RelayCommand(param => this.InitializeModel(), param => true);
                }
                return m_initializeModelCommand;
            }
        }

        public ICommand SaveMeasurementSampleFilesCommand
        {
            get
            {
                if (m_saveMeasurementSampleFilesCommand == null)
                {
                    m_saveMeasurementSampleFilesCommand = new RelayCommand(param => this.SaveMeasurementSampleFile(), param => true);
                }
                return m_saveMeasurementSampleFilesCommand;
            }
        }

        public ICommand SaveNetworkSnapshotFileCommand
        {
            get
            {
                if (m_saveNetworkSnapshotFileCommand == null)
                {
                    m_saveNetworkSnapshotFileCommand = new RelayCommand(param => this.SaveNetworkSnapshotFile(), param => true);
                }
                return m_saveNetworkSnapshotFileCommand;
            }
        }

        public ICommand SelectMeasurementSampleCommand
        {
            get
            {
                if (m_selectMeasurementSampleCommand == null)
                {
                    m_selectMeasurementSampleCommand = new RelayCommand(param => this.SelectMeasurementSample(), param => true);
                }
                return m_selectMeasurementSampleCommand;
            }
        }

        public ICommand ClearMeasurementsFromModelCommand
        {
            get
            {
                if (m_clearMeasurementsFromModelCommand == null)
                {
                    m_clearMeasurementsFromModelCommand = new RelayCommand(param => this.ClearMeasurementsFromModel(), param => true);
                }
                return m_clearMeasurementsFromModelCommand;
            }
        }

        public ICommand MapMeasurementsToModelCommand
        {
            get
            {
                if (m_mapMeasurementsToModelCommand == null)
                {
                    m_mapMeasurementsToModelCommand = new RelayCommand(param => this.MapMeasurementsToModel(), param => true);
                }
                return m_mapMeasurementsToModelCommand;
            }
        }

        public ICommand ViewUnreceivedMeasurementsCommand
        {
            get
            {
                if (m_viewUnreceivedMeasurementsCommand == null)
                {
                    m_viewUnreceivedMeasurementsCommand = new RelayCommand(param => this.ViewUnreceivedMeasurements(), param => true);
                }
                return m_viewUnreceivedMeasurementsCommand;
            }
        }

        public ICommand ViewReceivedMeasurementsCommand
        {
            get
            {
                if (m_viewReceivedMeasurementsCommand == null)
                {
                    m_viewReceivedMeasurementsCommand = new RelayCommand(param => this.ViewReceivedMeasurements(), param => true);
                }
                return m_viewReceivedMeasurementsCommand;
            }
        }

        public ICommand ViewComponentsCommand
        {
            get
            {
                if (m_viewComponentsCommand == null)
                {
                    m_viewComponentsCommand = new RelayCommand(param => this.ViewComponents(), param => true);
                }
                return m_viewComponentsCommand;
            }
        }

        public ICommand GenerateMeasurementSamplesCommand
        {
            get
            {
                if (m_generateMeasurementSamplesCommand == null)
                {
                    m_generateMeasurementSamplesCommand = new RelayCommand(param => this.GenerateMeasurementSamplesFromCsv(), param => true);
                }
                return m_generateMeasurementSamplesCommand;
            }
        }

        #endregion

        #region [ Observability Analysis Commands ] 

        public ICommand DetermineActiveCurrentFlowsCommand
        {
            get
            {
                if (m_determineActiveCurrentFlowsCommand == null)
                {
                    m_determineActiveCurrentFlowsCommand = new RelayCommand(param => this.DetermineActiveCurrentFlows(), param => true);
                }
                return m_determineActiveCurrentFlowsCommand;
            }
        }

        public ICommand DetermineActiveCurrentInjectionsCommand
        {
            get
            {
                if (m_determineActiveCurrentInjectionsCommand == null)
                {
                    m_determineActiveCurrentInjectionsCommand = new RelayCommand(param => this.DetermineActiveCurrentInjections(), param => true);
                }
                return m_determineActiveCurrentInjectionsCommand;
            }
        }

        public ICommand ResolvetoObservedBusesCommand
        {
            get
            {
                if (m_resolvetoObservedBusesCommand == null)
                {
                    m_resolvetoObservedBusesCommand = new RelayCommand(param => this.ResolveToObservedBuses(), param => true);
                }
                return m_resolvetoObservedBusesCommand;
            }
        }

        public ICommand ResolveToSingleFlowBranchesCommand
        {
            get
            {
                if (m_resolveToSingleFlowBranchesCommand == null)
                {
                    m_resolveToSingleFlowBranchesCommand = new RelayCommand(param => this.ResolveToSingleFlowBranches(), param => true);
                }
                return m_resolveToSingleFlowBranchesCommand;
            }
        }
        
        #endregion

        #region [ Computation Commands ] 

        public ICommand ComputeSystemStateCommand
        {
            get
            {
                if (m_computeSystemStateCommand == null)
                {
                    m_computeSystemStateCommand = new RelayCommand(param => this.ComputeSystemState(), param => true);
                }
                return m_computeSystemStateCommand;
            }
        }

        public ICommand ComputeLineFlowsCommand
        {
            get
            {
                if (m_computeLineFlowsCommand == null)
                {
                    m_computeLineFlowsCommand = new RelayCommand(param => this.ComputeLineFlows(), param => true);
                }
                return m_computeLineFlowsCommand;
            }
        }

        public ICommand ComputeInjectionsCommand
        {
            get
            {
                if (m_computeInjectionsCommand == null)
                {
                    m_computeInjectionsCommand = new RelayCommand(param => this.ComputeInjections(), param => true);
                }
                return m_computeInjectionsCommand;
            }
        }

        public ICommand ComputePowerFlowsCommand
        {
            get
            {
                if (m_computePowerFlowsCommand == null)
                {
                    m_computePowerFlowsCommand = new RelayCommand(param => this.ComputePowerFlows(), param => true);
                }
                return m_computePowerFlowsCommand;
            }
        }

        public ICommand ComputeSequenceComponentsCommand
        {
            get
            {
                if (m_computeSequenceComponentsCommand == null)
                {
                    m_computeSequenceComponentsCommand = new RelayCommand(param => this.ComputeSequenceComponents(), param => true);
                }
                return m_computeSequenceComponentsCommand;
            }
        }

        #endregion

        #region [ View Matrix Commands ]

        public ICommand ViewAMatrixCommand
        {
            get
            {
                if (m_viewAMatrixCommand == null)
                {
                    m_viewAMatrixCommand = new RelayCommand(param => this.ViewAMatrix(), param => true);
                }
                return m_viewAMatrixCommand;
            }
        }

        public ICommand ViewIIMatrixCommand
        {
            get
            {
                if (m_viewIIMatrixCommand == null)
                {
                    m_viewIIMatrixCommand = new RelayCommand(param => this.ViewIIMatrix(), param => true);
                }
                return m_viewIIMatrixCommand;
            }
        }

        public ICommand ViewYMatrixCommand
        {
            get
            {
                if (m_viewYMatrixCommand == null)
                {
                    m_viewYMatrixCommand = new RelayCommand(param => this.ViewYMatrix(), param => true);
                }
                return m_viewYMatrixCommand;
            }
        }

        public ICommand ViewYsMatrixCommand
        {
            get
            {
                if (m_viewYsMatrixCommand == null)
                {
                    m_viewYsMatrixCommand = new RelayCommand(param => this.ViewYsMatrix(), param => true);
                }
                return m_viewYsMatrixCommand;           
            }
        }

        public ICommand ViewYshMatrixCommand
        {
            get
            {
                if (m_viewYshMatrixCommand == null)
                {
                    m_viewYshMatrixCommand = new RelayCommand(param => this.ViewYshMatrix(), param => true);
                }
                return m_viewYshMatrixCommand;
            }
        }

        #endregion

        #region [ Inspect Commands ] 

        public ICommand ViewObservableNodesCommand
        {
            get
            {
                if (m_viewObservableNodesCommand == null)
                {
                    m_viewObservableNodesCommand = new RelayCommand(param => this.ViewObservableNodes(), param => true);
                }
                return m_viewObservableNodesCommand;
            }
        }

        public ICommand ViewSubstationAdjacencyListCommand
        {
            get
            {
                if (m_viewSubstationAdjacencyListCommand == null)
                {
                    m_viewSubstationAdjacencyListCommand = new RelayCommand(param => this.ViewSubstationAdjacencyList(), param => true);
                }
                return m_viewSubstationAdjacencyListCommand;
            }
        }

        public ICommand ViewTransmissionLineAdjacencyListCommand
        {
            get
            {
                if (m_viewTransmissionLineAdjacencyListCommand == null)
                {
                    m_viewTransmissionLineAdjacencyListCommand = new RelayCommand(param => this.ViewTransmissionLineAdjacencyLists(), param => true);
                }
                return m_viewTransmissionLineAdjacencyListCommand;
            }
        }

        public ICommand ViewCalculatedImpedancesCommand
        {
            get
            {
                if (m_viewCalculatedImpedancesCommand == null)
                {
                    m_viewCalculatedImpedancesCommand = new RelayCommand(param => this.ViewCalculatedImpedances(), param => true);
                }
                return m_viewCalculatedImpedancesCommand;
            }
        }

        public ICommand ViewSeriesCompensatorInferenceDataCommand
        {
            get
            {
                if (m_viewSeriesCompensatorInferenceDataCommand == null)
                {
                    m_viewSeriesCompensatorInferenceDataCommand = new RelayCommand(param => this.ViewSeriesCompensatorInferenceData(), param => true);
                }
                return m_viewSeriesCompensatorInferenceDataCommand;
            }
        }

        public ICommand ViewSeriesCompensatorsCommand
        {
            get
            {
                if (m_viewSeriesCompensatorsCommand == null)
                {
                    m_viewSeriesCompensatorsCommand = new RelayCommand(param => this.ViewSeriesCompensators(), param => true);
                }
                return m_viewSeriesCompensatorsCommand;
            }
        }

        public ICommand ViewTransformersCommand
        {
            get
            {
                if (m_viewTransformersCommand == null)
                {
                    m_viewTransformersCommand = new RelayCommand(param => this.ViewTransformers(), param => true);
                }
                return m_viewTransformersCommand;
            }
        }

        public ICommand ViewOutputMeasurementsCommand
        {
            get
            {
                if (m_viewOutputMeasurementsCommand == null)
                {
                    m_viewOutputMeasurementsCommand = new RelayCommand(param => this.ViewOutputMeasurements(), param => true);
                }
                return m_viewOutputMeasurementsCommand;
            }
        }

        public ICommand ViewModeledVoltagesCommand
        {
            get
            {
                if (m_viewModeledVoltagesCommand == null)
                {
                    m_viewModeledVoltagesCommand = new RelayCommand(param => this.ViewModeledVoltages(), param => true);
                }
                return m_viewModeledVoltagesCommand;
            }
        }

        public ICommand ViewExpectedVoltagesCommand
        {
            get
            {
                if (m_viewExpectedVoltagesCommand == null)
                {
                    m_viewExpectedVoltagesCommand = new RelayCommand(param => this.ViewExpectedVoltages(), param => true);
                }
                return m_viewExpectedVoltagesCommand;
            }
        }

        public ICommand ViewActiveVoltagesCommand
        {
            get
            {
                if (m_viewActiveVoltagesCommand == null)
                {
                    m_viewActiveVoltagesCommand = new RelayCommand(param => this.ViewActiveVoltages(), param => true);
                }
                return m_viewActiveVoltagesCommand;
            }
        }

        public ICommand ViewInactiveVoltagesCommand
        {
            get
            {
                if (m_viewInactiveVoltagesCommand == null)
                {
                    m_viewInactiveVoltagesCommand = new RelayCommand(param => this.ViewInactiveVoltages(), param => true);
                }
                return m_viewInactiveVoltagesCommand;
            }
        }

        public ICommand ViewModeledCurrentFlowsCommand
        {
            get
            {
                if (m_viewModeledCurrentFlowsCommand == null)
                {
                    m_viewModeledCurrentFlowsCommand = new RelayCommand(param => this.ViewModeledCurrentFlows(), param => true);
                }
                return m_viewModeledCurrentFlowsCommand;
            }
        }

        public ICommand ViewExpectedCurrentFlowsCommand
        {
            get
            {
                if (m_viewExpectedCurrentFlowsCommand == null)
                {
                    m_viewExpectedCurrentFlowsCommand = new RelayCommand(param => this.ViewExpectedCurrentFlows(), param => true);
                }
                return m_viewExpectedCurrentFlowsCommand;
            }
        }

        public ICommand ViewActiveCurrentFlowsCommand
        {
            get
            {
                if (m_viewActiveCurrentFlowsCommand == null)
                {
                    m_viewActiveCurrentFlowsCommand = new RelayCommand(param => this.ViewActiveCurrentFlows(), param => true);
                }
                return m_viewActiveCurrentFlowsCommand;
            }
        }

        public ICommand ViewInactiveCurrentFlowsCommand
        {
            get
            {
                if (m_viewInactiveCurrentFlowsCommand == null)
                {
                    m_viewInactiveCurrentFlowsCommand = new RelayCommand(param => this.ViewInactiveCurrentFlows(), param => true);
                }
                return m_viewInactiveCurrentFlowsCommand;
            }
        }

        public ICommand ViewIncludedCurrentFlowsCommand
        {
            get
            {
                if (m_viewIncludedCurrentFlowsCommand == null)
                {
                    m_viewIncludedCurrentFlowsCommand = new RelayCommand(param => this.ViewIncludedCurrentFlows(), param => true);
                }
                return m_viewIncludedCurrentFlowsCommand;
            }
        }

        public ICommand ViewExcludedCurrentFlowsCommand
        {
            get
            {
                if (m_viewExcludedCurrentFlowsCommand == null)
                {
                    m_viewExcludedCurrentFlowsCommand = new RelayCommand(param => this.ViewExcludedCurrentFlows(), param => true);
                }
                return m_viewExcludedCurrentFlowsCommand;
            }
        }

        public ICommand ViewModeledCurrentInjectionsCommand
        {
            get
            {
                if (m_viewModeledCurrentInjectionsCommand == null)
                {
                    m_viewModeledCurrentInjectionsCommand = new RelayCommand(param => this.ViewModeledCurrentInjections(), param => true);
                }
                return m_viewModeledCurrentInjectionsCommand;
            }
        }

        public ICommand ViewExpectedCurrentInjectionsCommand
        {
            get
            {
                if (m_viewExpectedCurrentInjectionsCommand == null)
                {
                    m_viewExpectedCurrentInjectionsCommand = new RelayCommand(param => this.ViewExpectedCurrentInjections(), param => true);
                }
                return m_viewExpectedCurrentInjectionsCommand;
            }
        }

        public ICommand ViewActiveCurrentInjectionsCommand
        {
            get
            {
                if (m_viewActiveCurrentInjectionsCommand == null)
                {
                    m_viewActiveCurrentInjectionsCommand = new RelayCommand(param => this.ViewActiveCurrentInjections(), param => true);
                }
                return m_viewActiveCurrentInjectionsCommand;
            }
        }

        public ICommand ViewInactiveCurrentInjectionsCommand
        {
            get
            {
                if (m_viewInactiveCurrentInjectionsCommand == null)
                {
                    m_viewInactiveCurrentInjectionsCommand = new RelayCommand(param => this.ViewInactiveCurrentInjections(), param => true);
                }
                return m_viewInactiveCurrentInjectionsCommand;
            }
        }

        public ICommand ViewStatusWordsCommand
        {
            get
            {
                if (m_viewStatusWordsCommand == null)
                {
                    m_viewStatusWordsCommand = new RelayCommand(param => this.ViewStatusWords(), param => true);
                }
                return m_viewStatusWordsCommand;
            }
        }

        public ICommand ViewMappedStatusWordsCommand
        {
            get
            {
                if (m_viewMappedStatusWordsCommand == null)
                {
                    m_viewMappedStatusWordsCommand = new RelayCommand(param => this.ViewMappedStatusWords(), param => true);
                }
                return m_viewMappedStatusWordsCommand;
            }
        }

        public ICommand ViewMappedLSEInvalidStatusWordsCommand
        {
            get
            {
                if (m_viewMappedLSEInvalidStatusWordsCommand == null)
                {
                    m_viewMappedLSEInvalidStatusWordsCommand = new RelayCommand(param => this.ViewMappedLSEInvalidStatusWords(), param => true);
                }
                return m_viewMappedLSEInvalidStatusWordsCommand;
            }
        }

        public ICommand ViewMappedMissingStatusWordsCommand
        {
            get
            {
                if (m_viewMappedMissingStatusWordsCommand == null)
                {
                    m_viewMappedMissingStatusWordsCommand = new RelayCommand(param => this.ViewMappedMissingStatusWords(), param => true);
                }
                return m_viewMappedMissingStatusWordsCommand;
            }
        }

        public ICommand ViewActiveStatusWordsCommand
        {
            get
            {
                if (m_viewActiveStatusWordsCommand == null)
                {
                    m_viewActiveStatusWordsCommand = new RelayCommand(param => this.ViewActiveStatusWords(), param => true);
                }
                return m_viewActiveStatusWordsCommand;
            }
        }

        public ICommand ViewReportedVoltagesCommand
        {
            get
            {
                if (m_viewReportedVoltagesCommand == null)
                {
                    m_viewReportedVoltagesCommand = new RelayCommand(param => this.ViewReportedVoltages(), param => true);
                }
                return m_viewReportedVoltagesCommand;
            }
        }

        public ICommand ViewUnreportedVoltagesCommand
        {
            get
            {
                if (m_viewUnreportedVoltagesCommand == null)
                {
                    m_viewUnreportedVoltagesCommand = new RelayCommand(param => this.ViewUnreportedVoltages(), param => true);
                }
                return m_viewUnreportedVoltagesCommand;
            }
        }

        public ICommand ViewActiveVoltagesByStatusWordCommand
        {
            get
            {
                if (m_viewActiveVoltagesByStatusWordCommand == null)
                {
                    m_viewActiveVoltagesByStatusWordCommand = new RelayCommand(param => this.ViewActiveVoltagesByStatusWord(), param => true);
                }
                return m_viewActiveVoltagesByStatusWordCommand;
            }
        }

        public ICommand ViewInactiveVoltagesByStatusWordCommand
        {
            get
            {
                if (m_viewInactiveVoltagesByStatusWordCommand == null)
                {
                    m_viewInactiveVoltagesByStatusWordCommand = new RelayCommand(param => this.ViewInactiveVoltagesByStatusWord(), param => true);
                }
                return m_viewInactiveVoltagesByStatusWordCommand;
            }
        }

        public ICommand ViewInactiveVoltagesByMeasurementCommand
        {
            get
            {
                if (m_viewInactiveVoltagesByMeasurementCommand == null)
                {
                    m_viewInactiveVoltagesByMeasurementCommand = new RelayCommand(param => this.ViewInactiveVoltagesByMeasurement(), param => true);
                }
                return m_viewInactiveVoltagesByMeasurementCommand;
            }
        }

        public ICommand ViewReportedCurrentFlowsCommand
        {
            get
            {
                if (m_viewReportedCurrentFlowsCommand == null)
                {
                    m_viewReportedCurrentFlowsCommand = new RelayCommand(param => this.ViewReportedCurrentFlows(), param => true);
                }
                return m_viewReportedCurrentFlowsCommand;
            }
        }

        public ICommand ViewUnreportedCurrentFlowsCommand
        {
            get
            {
                if (m_viewUnreportedCurrentFlowsCommand == null)
                {
                    m_viewUnreportedCurrentFlowsCommand = new RelayCommand(param => this.ViewUnreportedCurrentFlows(), param => true);
                }
                return m_viewUnreportedCurrentFlowsCommand;
            }
        }

        public ICommand ViewActiveCurrentFlowsByStatusWordCommand
        {
            get
            {
                if (m_viewActiveCurrentFlowsByStatusWordCommand == null)
                {
                    m_viewActiveCurrentFlowsByStatusWordCommand = new RelayCommand(param => this.ViewActiveCurrentFlowsByStatusWord(), param => true);
                }
                return m_viewActiveCurrentFlowsByStatusWordCommand;
            }
        }

        public ICommand ViewInactiveCurrentFlowsByStatusWordCommand
        {
            get
            {
                if (m_viewInactiveCurrentFlowsByStatusWordCommand == null)
                {
                    m_viewInactiveCurrentFlowsByStatusWordCommand = new RelayCommand(param => this.ViewInactiveCurrentFlowsByStatusWord(), param => true);
                }
                return m_viewInactiveCurrentFlowsByStatusWordCommand;
            }
        }

        public ICommand ViewInactiveCurrentFlowsByMeasurementCommand
        {
            get
            {
                if (m_viewInactiveCurrentFlowsByMeasurementCommand == null)
                {
                    m_viewInactiveCurrentFlowsByMeasurementCommand = new RelayCommand(param => this.ViewInactiveCurrentFlowsByMeasurement(), param => true);
                }
                return m_viewInactiveCurrentFlowsByMeasurementCommand;
            }
        }

        public ICommand ViewReportedCurrentInjectionsCommand
        {
            get
            {
                if (m_viewReportedCurrentInjectionsCommand == null)
                {
                    m_viewReportedCurrentInjectionsCommand = new RelayCommand(param => this.ViewReportedCurrentInjections(), param => true);
                }
                return m_viewReportedCurrentInjectionsCommand;
            }
        }

        public ICommand ViewUnreportedCurrentInjectionsCommand
        {
            get
            {
                if (m_viewUnreportedCurrentInjectionsCommand == null)
                {
                    m_viewUnreportedCurrentInjectionsCommand = new RelayCommand(param => this.ViewUnreportedCurrentInjections(), param => true);
                }
                return m_viewUnreportedCurrentInjectionsCommand;
            }
        }

        public ICommand ViewActiveCurrentInjectionsByStatusWordCommand
        {
            get
            {
                if (m_viewActiveCurrentInjectionsByStatusWordCommand == null)
                {
                    m_viewActiveCurrentInjectionsByStatusWordCommand = new RelayCommand(param => this.ViewActiveCurrentInjectionsByStatusWord(), param => true);
                }
                return m_viewActiveCurrentInjectionsByStatusWordCommand;
            }
        }

        public ICommand ViewInactiveCurrentInjectionsByStatusWordCommand
        {
            get
            {
                if (m_viewInactiveCurrentInjectionsByStatusWordCommand == null)
                {
                    m_viewInactiveCurrentInjectionsByStatusWordCommand = new RelayCommand(param => this.ViewInactiveCurrentInjectionsByStatusWord(), param => true);
                }
                return m_viewInactiveCurrentInjectionsByStatusWordCommand;
            }
        }

        public ICommand ViewInactiveCurrentInjectionsByMeasurementCommand
        {
            get
            {
                if (m_viewInactiveCurrentInjectionsByMeasurementCommand == null)
                {
                    m_viewInactiveCurrentInjectionsByMeasurementCommand = new RelayCommand(param => this.ViewInactiveCurrentInjectionsByMeasurement(), param => true);
                }
                return m_viewInactiveCurrentInjectionsByMeasurementCommand;
            }
        }

        public ICommand ViewModeledBreakerStatusesCommand
        {
            get
            {
                if (m_viewModeledBreakerStatusesCommand == null)
                {
                    m_viewModeledBreakerStatusesCommand = new RelayCommand(param => this.ViewModeledBreakerStatuses(), param => true);
                }
                return m_viewModeledBreakerStatusesCommand;
            }
        }

        public ICommand ViewExpectedBreakerStatusesCommand
        {
            get
            {
                if (m_viewExpectedBreakerStatusesCommand == null)
                {
                    m_viewExpectedBreakerStatusesCommand = new RelayCommand(param => this.ViewExpectedBreakerStatuses(), param => true);
                }
                return m_viewExpectedBreakerStatusesCommand;
            }
        }

        public ICommand ViewReportedBreakerStatusesCommand
        {
            get
            {
                if (m_viewReportedBreakerStatusesCommand == null)
                {
                    m_viewReportedBreakerStatusesCommand = new RelayCommand(param => this.ViewReportedBreakerStatuses(), param => true);
                }
                return m_viewReportedBreakerStatusesCommand;
            }
        }

        public ICommand ViewCircuitBreakersCommand
        {
            get
            {
                if (m_viewCircuitBreakersCommand == null)
                {
                    m_viewCircuitBreakersCommand = new RelayCommand(param => this.ViewCircuitBreakers(), param => true);
                }
                return m_viewCircuitBreakersCommand;
            }
        }

        public ICommand ViewSwitchesCommand
        {
            get
            {
                if (m_viewSwitchesCommand == null)
                {
                    m_viewSwitchesCommand = new RelayCommand(param => this.ViewSwitches(), param => true);
                }
                return m_viewSwitchesCommand;
            }
        }

        public ICommand ViewAllSwitchingDevicesCommand
        {
            get
            {
                if (m_viewAllSwitchingDevicesCommand == null)
                {
                    m_viewAllSwitchingDevicesCommand = new RelayCommand(param => this.ViewAllSwitchingDevices(), param => true);
                }
                return m_viewAllSwitchingDevicesCommand;
            }
        }

        #endregion

        #endregion

        #endregion

        #region [ Constructor ]

        public MainWindowViewModel()
        {
            InitializeMenuBar();
            InitializeNetworkTree();
            InitializeRecordDetail();
            InitializeMeasurementSampler();
            //InitializeLinearStateEstimator();

            m_networkIsInitialized = false;
            m_measurementsAreMapped = false;
            m_activeCurrentFlowsHaveBeenDetermined = false;
            m_activeCurrentInjectionsHaveBeenDetermined = false;
            m_observedBussesHaveBeenResolved = false;
            m_singleFlowBranchesHaveBeenResolved = false;
            m_stateWasComputed = false;

            DisableControls();
        }

        #endregion

        #region [ Enable Controls ]

        private void EnableControls()
        {
            if (m_selectedMeasurementSample != null && m_network != null)
            {
                m_initializeModelMenuItem.IsEnabled = true;
            }
            if (m_networkIsInitialized)
            {
                m_saveNetworkSnapshotMenuItem.IsEnabled = true;
                m_mapMeasurementsMenuItem.IsEnabled = true;
                m_viewComponentSummaryMenuItem.IsEnabled = true;
                m_viewSeriesCompensatorsMenuItem.IsEnabled = true;
                m_viewTransformersMenuItem.IsEnabled = true;
            }
            if (m_measurementsAreMapped)
            {
                m_determineActiveCurrentFlowsMenuItem.IsEnabled = true;
                m_viewReceivedMeasurementsMenuItem.IsEnabled = true;
                m_viewUnreceivedMeasurementsMenuItem.IsEnabled = true;
                m_viewAllStatusWordsMenuItem.IsEnabled = true;
                m_viewMappedStatusWordsMenuItem.IsEnabled = true;
                m_viewMappedLSEInvalidStatusWordsMenuItem.IsEnabled = true;
                m_viewMappedMissingStatusWordsMenuItem.IsEnabled = true;
                m_viewModeledVoltagesMenuItem.IsEnabled = true;
                m_viewExpectedVoltagesMenuItem.IsEnabled = true;
                m_viewActiveVoltagesMenuItem.IsEnabled = true;
                m_viewInactiveVoltagesMenuItem.IsEnabled = true;
                m_viewActiveStatusWordsMenuItem.IsEnabled = true;
                m_viewReportedVoltagesMenuItem.IsEnabled = true;
                m_viewUnreportedVoltagesMenuItem.IsEnabled = true;
                m_viewActiveVoltagesByStatusWordMenuItem.IsEnabled = true;
                m_viewInactiveVoltagesByStatusWordMenuItem.IsEnabled = true;
                m_viewInactiveVoltagesByMeasurementMenuItem.IsEnabled = true;
                m_viewModeledBreakerStatusesMenuItem.IsEnabled = true;
                m_viewExpectedBreakerStatusesMenuItem.IsEnabled = true;
                m_viewReportedBreakerStatusesMenuItem.IsEnabled = true;
                m_viewCircuitBreakersMenuItem.IsEnabled = true;
                m_viewSwitchesMenuItem.IsEnabled = true;
                m_viewAllSwitchingDevicesMenuItem.IsEnabled = true;
            }
            if (m_activeCurrentFlowsHaveBeenDetermined)
            {
                m_determineActiveCurrentInjectionsMenuItem.IsEnabled = true;
                m_viewCalculatedImpedancesMenuItem.IsEnabled = true;
                m_viewModeledCurrentFlowsMenuItem.IsEnabled = true;
                m_viewExpectedCurrentFlowsMenuItem.IsEnabled = true;
                m_viewActiveCurrentFlowsMenuItem.IsEnabled = true;
                m_viewInactiveCurrentFlowsMenuItem.IsEnabled = true;
                m_viewIncludedCurrentFlowsMenuItem.IsEnabled = true;
                m_viewExcludedCurrentFlowsMenuItem.IsEnabled = true;
                m_viewReportedCurrentFlowsMenuItem.IsEnabled = true;
                m_viewUnreportedCurrentFlowsMenuItem.IsEnabled = true;
                m_viewActiveCurrentFlowsByStatusWordMenuItem.IsEnabled = true;
                m_viewInactiveCurrentFlowsByStatusWordMenuItem.IsEnabled = true;
                m_viewInactiveCurrentFlowsByMeasurementMenuItem.IsEnabled = true;

            }
            if (m_activeCurrentInjectionsHaveBeenDetermined)
            {
                m_resolveToObservedBusesMenuItem.IsEnabled = true;
                m_resolveToSingleFlowBranchesMenuItem.IsEnabled = true;
                m_viewModeledCurrentInjectionsMenuItem.IsEnabled = true;
                m_viewExpectedCurrentInjectionsMenuItem.IsEnabled = true;
                m_viewActiveCurrentInjectionsMenuItem.IsEnabled = true;
                m_viewInactiveCurrentInjectionsMenuItem.IsEnabled = true;
                m_viewReportedCurrentInjectionsMenuItem.IsEnabled = true;
                m_viewUnreportedCurrentInjectionsMenuItem.IsEnabled = true;
                m_viewActiveCurrentInjectionsByStatusWordMenuItem.IsEnabled = true;
                m_viewInactiveCurrentInjectionsByStatusWordMenuItem.IsEnabled = true;
                m_viewInactiveCurrentInjectionsByMeasurementMenuItem.IsEnabled = true;
            }
            if (m_observedBussesHaveBeenResolved)
            {
                m_viewModeledVoltagesMenuItem.IsEnabled = true;
                m_viewObservableNodesMenuItem.IsEnabled = true;
                m_viewSubstationAdjacencyListMenuItem.IsEnabled = true;
            }
            if (m_singleFlowBranchesHaveBeenResolved)
            {
                m_viewTransmissionLineAdjacencyListMenuItem.IsEnabled = true;
                m_viewSeriesCompensatorInferenceDataMenuItem.IsEnabled = true;
            }
            if (m_observedBussesHaveBeenResolved && m_singleFlowBranchesHaveBeenResolved)
            {
                m_viewAMatrixMenuItem.IsEnabled = true;
                m_viewIIMatrixMenuItem.IsEnabled = true;
                m_viewYMatrixMenuItem.IsEnabled = true;
                m_viewYsMatrixMenuItem.IsEnabled = true;
                m_viewYshMatrixMenuItem.IsEnabled = true;
                m_computeSystemStateMenuItem.IsEnabled = true;
            }
            if (m_stateWasComputed)
            {
                m_computeLineFlowsMenuItem.IsEnabled = true;
                m_computeInjectionsMenuItem.IsEnabled = true;
                m_computePowerFlowsMenuItem.IsEnabled = true;
                m_viewAllOutputMeasurementsMenuItem.IsEnabled = true;
                m_viewPerformanceMetricsOutputMenuItem.IsEnabled = true;
                m_viewTopologyProfilingOutputMenuItem.IsEnabled = true;
                m_viewMeasurementValidationFlagsOutputMenuItem.IsEnabled = true;
                m_viewStateEstimateOutputMenuItem.IsEnabled = true;
                m_viewCurrentFlowEstimateOutputMenuItem.IsEnabled = true;
                m_viewCurrentInjectionEstimateOutputMenuItem.IsEnabled = true;
                m_viewVoltageResidualOutputMenuItem.IsEnabled = true;
                m_viewCurrentFlowResidualOutputMenuItem.IsEnabled = true;
                m_viewCurrentInjectionResidualOutputMenuItem.IsEnabled = true;
                m_viewCircuitBreakerStatusOutputMenuItem.IsEnabled = true;
                m_viewSwitchStatusOutputMenuItem.IsEnabled = true;
                m_viewTapPositionOutputMenuItem.IsEnabled = true;
                m_viewSeriesCompensatorStatusOutputMenuItem.IsEnabled = true;
                if (m_network.PhaseConfiguration == PhaseSelection.ThreePhase)
                {
                    m_computeSequenceComponentsMenuItem.IsEnabled = true;
                }
            }
            OnPropertyChanged("MenuBarItems");
        }

        #endregion

        #region [ Disable Controls ]

        private void DisableControls()
        {
            DisableSetupMenuBarControls();
            DisableObservabilityAnalysisMenuBarControls();
            DisableMatricesMenuBarControls();
            DisableComputationMenuBarControls();
            DisableInspectMenuBarControls();
            OnPropertyChanged("MenuBarItems");
        }

        private void DisableSetupMenuBarControls()
        {
            m_initializeModelMenuItem.IsEnabled = false;
            m_saveNetworkSnapshotMenuItem.IsEnabled = false;
            m_mapMeasurementsMenuItem.IsEnabled = false;
            m_viewReceivedMeasurementsMenuItem.IsEnabled = false;
            m_viewUnreceivedMeasurementsMenuItem.IsEnabled = false;
            m_viewComponentSummaryMenuItem.IsEnabled = false;
        }

        private void DisableObservabilityAnalysisMenuBarControls()
        {
            m_determineActiveCurrentFlowsMenuItem.IsEnabled = false;
            m_determineActiveCurrentInjectionsMenuItem.IsEnabled = false;
            m_resolveToObservedBusesMenuItem.IsEnabled = false;
            m_resolveToSingleFlowBranchesMenuItem.IsEnabled = false;
            m_viewActiveCurrentFlowsMenuItem.IsEnabled = false;
            m_viewModeledVoltagesMenuItem.IsEnabled = false;
            m_viewObservableNodesMenuItem.IsEnabled = false;
            m_viewSubstationAdjacencyListMenuItem.IsEnabled = false;
            m_viewTransmissionLineAdjacencyListMenuItem.IsEnabled = false;
            m_viewCalculatedImpedancesMenuItem.IsEnabled = false;
        }

        private void DisableMatricesMenuBarControls()
        {
            m_viewAMatrixMenuItem.IsEnabled = false;
            m_viewIIMatrixMenuItem.IsEnabled = false;
            m_viewYMatrixMenuItem.IsEnabled = false;
            m_viewYsMatrixMenuItem.IsEnabled = false;
            m_viewYshMatrixMenuItem.IsEnabled = false;
        }

        private void DisableComputationMenuBarControls()
        {
            m_computeSystemStateMenuItem.IsEnabled = false;
            m_computeLineFlowsMenuItem.IsEnabled = false;
            m_computeInjectionsMenuItem.IsEnabled = false;
            m_computePowerFlowsMenuItem.IsEnabled = false;
            m_computeSequenceComponentsMenuItem.IsEnabled = false;
        }

        private void DisableInspectMenuBarControls()
        {
            m_viewSeriesCompensatorInferenceDataMenuItem.IsEnabled = false;
            m_viewTransformersMenuItem.IsEnabled = false;
            m_viewSeriesCompensatorsMenuItem.IsEnabled = false;
            m_viewExpectedVoltagesMenuItem.IsEnabled = false;
            m_viewActiveVoltagesMenuItem.IsEnabled = false;
            m_viewInactiveVoltagesMenuItem.IsEnabled = false;

            m_viewAllStatusWordsMenuItem.IsEnabled = false;
            m_viewMappedStatusWordsMenuItem.IsEnabled = false;
            m_viewMappedLSEInvalidStatusWordsMenuItem.IsEnabled = false;
            m_viewMappedMissingStatusWordsMenuItem.IsEnabled = false;
            m_viewModeledCurrentFlowsMenuItem.IsEnabled = false;
            m_viewExpectedCurrentFlowsMenuItem.IsEnabled = false;
            m_viewActiveCurrentFlowsMenuItem.IsEnabled = false;
            m_viewInactiveCurrentFlowsMenuItem.IsEnabled = false;
            m_viewIncludedCurrentFlowsMenuItem.IsEnabled = false;
            m_viewExcludedCurrentFlowsMenuItem.IsEnabled = false;

            m_viewModeledCurrentInjectionsMenuItem.IsEnabled = false;
            m_viewExpectedCurrentInjectionsMenuItem.IsEnabled = false;
            m_viewActiveCurrentInjectionsMenuItem.IsEnabled = false;
            m_viewInactiveCurrentInjectionsMenuItem.IsEnabled = false;

            m_viewActiveStatusWordsMenuItem.IsEnabled = false;
            m_viewReportedVoltagesMenuItem.IsEnabled = false;
            m_viewUnreportedVoltagesMenuItem.IsEnabled = false;
            m_viewActiveVoltagesByStatusWordMenuItem.IsEnabled = false;
            m_viewInactiveVoltagesByStatusWordMenuItem.IsEnabled = false;
            m_viewInactiveVoltagesByMeasurementMenuItem.IsEnabled = false;
            m_viewReportedCurrentFlowsMenuItem.IsEnabled = false;
            m_viewUnreportedCurrentFlowsMenuItem.IsEnabled = false;
            m_viewActiveCurrentFlowsByStatusWordMenuItem.IsEnabled = false;
            m_viewInactiveCurrentFlowsByStatusWordMenuItem.IsEnabled = false;
            m_viewInactiveCurrentFlowsByMeasurementMenuItem.IsEnabled = false;
            m_viewReportedCurrentInjectionsMenuItem.IsEnabled = false;
            m_viewUnreportedCurrentInjectionsMenuItem.IsEnabled = false;
            m_viewActiveCurrentInjectionsByStatusWordMenuItem.IsEnabled = false;
            m_viewInactiveCurrentInjectionsByStatusWordMenuItem.IsEnabled = false;
            m_viewInactiveCurrentInjectionsByMeasurementMenuItem.IsEnabled = false;
            m_viewAllOutputMeasurementsMenuItem.IsEnabled = false;
            m_viewPerformanceMetricsOutputMenuItem.IsEnabled = false;
            m_viewTopologyProfilingOutputMenuItem.IsEnabled = false;
            m_viewMeasurementValidationFlagsOutputMenuItem.IsEnabled = false;
            m_viewStateEstimateOutputMenuItem.IsEnabled = false;
            m_viewCurrentFlowEstimateOutputMenuItem.IsEnabled = false;
            m_viewCurrentInjectionEstimateOutputMenuItem.IsEnabled = false;
            m_viewVoltageResidualOutputMenuItem.IsEnabled = false;
            m_viewCurrentFlowResidualOutputMenuItem.IsEnabled = false;
            m_viewCurrentInjectionResidualOutputMenuItem.IsEnabled = false;
            m_viewCircuitBreakerStatusOutputMenuItem.IsEnabled = false;
            m_viewSwitchStatusOutputMenuItem.IsEnabled = false;
            m_viewTapPositionOutputMenuItem.IsEnabled = false;
            m_viewSeriesCompensatorStatusOutputMenuItem.IsEnabled = false;
            m_viewModeledBreakerStatusesMenuItem.IsEnabled = false;
            m_viewExpectedBreakerStatusesMenuItem.IsEnabled = false;
            m_viewReportedBreakerStatusesMenuItem.IsEnabled = false;
            m_viewCircuitBreakersMenuItem.IsEnabled = false;
            m_viewSwitchesMenuItem.IsEnabled = false;
            m_viewAllSwitchingDevicesMenuItem.IsEnabled = false;

        }

        #endregion

        public bool CanOpenFile
        {
            get
            {
                return true;
            }
        }

        public void DeleteMeasurementSample(RawMeasurements measurementSample)
        {
            m_measurementSamples.Remove(measurementSample);
            m_networkTreeViewModel.MeasurementSamples = m_measurementSamples;
            ActionStatus = $"Deleted 'Sample {measurementSample.Identifier}'";
        }
        
        public void InitializeMeasurementSampler()
        {
            Algorithm.UpdateSystemSettings();
            SpecialStatus = "Initializing Measurement Sampler...";
            m_measurementSampler = new MeasurementSamplerAnalytic(this);
            Framework framework = FrameworkFactory.Create(MeasurementSampler);
            MeasurementSampler.InitializeFramework(framework);
        }

        public void InitializeLinearStateEstimator()
        {
            Algorithm.UpdateSystemSettings();
            SpecialStatus = "Initializing Linear State Estimator...";
            m_linearStateEstimator = new LinearStateEstimatorAnalytic(this);
            Framework framework = FrameworkFactory.Create(LinearStateEstimator);
            LinearStateEstimator.InitializeFramework(framework);
        }

        #region [ Initializing View Model Methods ] 

        private void InitializeMenuBar()
        {
            #region [ MENU BAR --> OFFLINE ANALYSIS --> SETUP ]
            m_clearMeasurementsMenuItem = new MenuItemViewModel("Clear Measurements From Model", ClearMeasurementsFromModelCommand);
            m_initializeModelMenuItem = new MenuItemViewModel("Initialize Model", InitializeModelCommand);
            m_mapMeasurementsMenuItem = new MenuItemViewModel("Map Measurements", MapMeasurementsToModelCommand);
            m_setupMenuItem = new MenuItemViewModel("Setup", null);
            m_setupMenuItem.AddMenuItem(m_clearMeasurementsMenuItem);
            m_setupMenuItem.AddMenuItem(m_initializeModelMenuItem);
            m_setupMenuItem.AddMenuItem(m_mapMeasurementsMenuItem);
            #endregion

            #region [ MENU BAR --> OFFLINE ANALYSIS --> OBSERVABILITY ANALYSIS ]
            m_determineActiveCurrentFlowsMenuItem = new MenuItemViewModel("Determine Active Current Flows", DetermineActiveCurrentFlowsCommand);
            m_determineActiveCurrentInjectionsMenuItem = new MenuItemViewModel("Determine Active Current Injections", DetermineActiveCurrentInjectionsCommand);
            m_resolveToObservedBusesMenuItem = new MenuItemViewModel("Resolve to Observed Buses", ResolvetoObservedBusesCommand);
            m_resolveToSingleFlowBranchesMenuItem = new MenuItemViewModel("Resolve to Single Flow Branches", ResolveToSingleFlowBranchesCommand);
            m_observabilityMenuItem = new MenuItemViewModel("Observability", null);
            m_observabilityMenuItem.AddMenuItem(m_determineActiveCurrentFlowsMenuItem);
            m_observabilityMenuItem.AddMenuItem(m_determineActiveCurrentInjectionsMenuItem);
            m_observabilityMenuItem.AddMenuItem(m_resolveToObservedBusesMenuItem);
            m_observabilityMenuItem.AddMenuItem(m_resolveToSingleFlowBranchesMenuItem);
            #endregion

            #region [ MENU BAR --> OFFLINE ANALYSIS --> MATRICES ]
            m_viewAMatrixMenuItem = new MenuItemViewModel("View 'A' Matrix", ViewAMatrixCommand);
            m_viewIIMatrixMenuItem = new MenuItemViewModel("View 'II' Matrix", ViewIIMatrixCommand);
            m_viewYMatrixMenuItem = new MenuItemViewModel("View 'Y' Matrix", ViewYMatrixCommand);
            m_viewYsMatrixMenuItem = new MenuItemViewModel("View 'Ys' Matrix", ViewYsMatrixCommand);
            m_viewYshMatrixMenuItem = new MenuItemViewModel("View 'Ysh' Matrix", ViewYshMatrixCommand);
            m_matricesMenuItem = new MenuItemViewModel("Matrices", null);
            m_matricesMenuItem.AddMenuItem(m_viewAMatrixMenuItem);
            m_matricesMenuItem.AddMenuItem(m_viewIIMatrixMenuItem);
            m_matricesMenuItem.AddMenuItem(m_viewYMatrixMenuItem);
            m_matricesMenuItem.AddMenuItem(m_viewYsMatrixMenuItem);
            m_matricesMenuItem.AddMenuItem(m_viewYshMatrixMenuItem);
            #endregion

            #region [ MENU BAR --> OFFLINE ANALYSIS --> COMPUTATION ]
            m_computeSystemStateMenuItem = new MenuItemViewModel("Compute System State", ComputeSystemStateCommand);
            m_computeLineFlowsMenuItem = new MenuItemViewModel("Compute Line Flows", ComputeLineFlowsCommand);
            m_computeInjectionsMenuItem = new MenuItemViewModel("Compute Injections", ComputeInjectionsCommand);
            m_computePowerFlowsMenuItem = new MenuItemViewModel("Compute Power Flows", ComputePowerFlowsCommand);
            m_computeSequenceComponentsMenuItem = new MenuItemViewModel("Compute Sequence Components", ComputeSequenceComponentsCommand);
            m_computationMenuItem = new MenuItemViewModel("Computation", null);
            m_computationMenuItem.AddMenuItem(m_computeSystemStateMenuItem);
            m_computationMenuItem.AddMenuItem(m_computeLineFlowsMenuItem);
            m_computationMenuItem.AddMenuItem(m_computeInjectionsMenuItem);
            m_computationMenuItem.AddMenuItem(m_computePowerFlowsMenuItem);
            m_computationMenuItem.AddMenuItem(m_computeSequenceComponentsMenuItem);
            #endregion

            #region [ MENU BAR --> OFFLINE ANALYSIS --> INSPECT ]

            m_viewComponentSummaryMenuItem = new MenuItemViewModel("Component Summary", ViewComponentsCommand);

            #region [ View Measurements ]
            m_viewMeasurementsMenuItem = new MenuItemViewModel("View Measurements", null);
            m_viewReceivedMeasurementsMenuItem = new MenuItemViewModel("Received", ViewReceivedMeasurementsCommand);
            m_viewUnreceivedMeasurementsMenuItem = new MenuItemViewModel("Unreceived", ViewUnreceivedMeasurementsCommand);
            m_viewAllOutputMeasurementsMenuItem = new MenuItemViewModel("All Output", ViewOutputMeasurementsCommand);
            m_viewPerformanceMetricsOutputMenuItem = new MenuItemViewModel("Performance Metrics", ViewPerformanceMetricsCommand);
            m_viewTopologyProfilingOutputMenuItem = new MenuItemViewModel("Topology Profiling", ViewTopologyProfilingOutputCommand);
            m_viewMeasurementValidationFlagsOutputMenuItem = new MenuItemViewModel("Measurement Validation Flags", ViewMeasurementValidationFlagOutputCommand);
            m_viewStateEstimateOutputMenuItem = new MenuItemViewModel("State Estimate", ViewVoltageEstimateOutputCommand);
            m_viewCurrentFlowEstimateOutputMenuItem = new MenuItemViewModel("Current Flow Estimate", ViewCurrentFlowEstimateOutputCommand);
            m_viewCurrentInjectionEstimateOutputMenuItem = new MenuItemViewModel("Current Injection Estimate", ViewCurrentInjectionEstimateOutputCommand);
            m_viewVoltageResidualOutputMenuItem = new MenuItemViewModel("Voltage Residuals", ViewVoltageResidualOutputCommand);
            m_viewCurrentFlowResidualOutputMenuItem = new MenuItemViewModel("Current Flow Residuals", ViewCurrentFlowResidualOutputCommand);
            m_viewCurrentInjectionResidualOutputMenuItem = new MenuItemViewModel("Current Injection Residuals", ViewCurrentInjectionResidualOutputCommand);
            m_viewCircuitBreakerStatusOutputMenuItem = new MenuItemViewModel("Circuit Breaker Statuses", ViewCircuitBreakerStatusOutputCommand);
            m_viewSwitchStatusOutputMenuItem = new MenuItemViewModel("Switch Statuses", ViewSwitchStatusOutputCommand);
            m_viewTapPositionOutputMenuItem = new MenuItemViewModel("Tap Positions", null);
            m_viewSeriesCompensatorStatusOutputMenuItem = new MenuItemViewModel("Series Compensator Statuses", null);
            m_viewOutputMeasurementsMenuItem = new MenuItemViewModel("Output", null);
            m_viewOutputMeasurementsMenuItem.AddMenuItem(m_viewAllOutputMeasurementsMenuItem);
            m_viewOutputMeasurementsMenuItem.AddMenuItem(m_viewPerformanceMetricsOutputMenuItem);
            m_viewOutputMeasurementsMenuItem.AddMenuItem(m_viewTopologyProfilingOutputMenuItem);
            m_viewOutputMeasurementsMenuItem.AddMenuItem(m_viewMeasurementValidationFlagsOutputMenuItem);
            m_viewOutputMeasurementsMenuItem.AddMenuItem(m_viewStateEstimateOutputMenuItem);
            m_viewOutputMeasurementsMenuItem.AddMenuItem(m_viewCurrentFlowEstimateOutputMenuItem);
            m_viewOutputMeasurementsMenuItem.AddMenuItem(m_viewCurrentInjectionEstimateOutputMenuItem);
            m_viewOutputMeasurementsMenuItem.AddMenuItem(m_viewVoltageResidualOutputMenuItem);
            m_viewOutputMeasurementsMenuItem.AddMenuItem(m_viewCurrentFlowResidualOutputMenuItem);
            m_viewOutputMeasurementsMenuItem.AddMenuItem(m_viewCurrentInjectionResidualOutputMenuItem);
            m_viewOutputMeasurementsMenuItem.AddMenuItem(m_viewCircuitBreakerStatusOutputMenuItem);
            m_viewOutputMeasurementsMenuItem.AddMenuItem(m_viewSwitchStatusOutputMenuItem);
            m_viewOutputMeasurementsMenuItem.AddMenuItem(m_viewTapPositionOutputMenuItem);
            m_viewOutputMeasurementsMenuItem.AddMenuItem(m_viewSeriesCompensatorStatusOutputMenuItem);
            m_viewMeasurementsMenuItem.AddMenuItem(m_viewReceivedMeasurementsMenuItem);
            m_viewMeasurementsMenuItem.AddMenuItem(m_viewUnreceivedMeasurementsMenuItem);
            m_viewMeasurementsMenuItem.AddMenuItem(m_viewOutputMeasurementsMenuItem);
            #endregion

            #region [ View Status Words ]
            m_viewAllStatusWordsMenuItem = new MenuItemViewModel("Modeled", ViewStatusWordsCommand);
            m_viewMappedStatusWordsMenuItem = new MenuItemViewModel("Expected", ViewMappedStatusWordsCommand);
            m_viewMappedLSEInvalidStatusWordsMenuItem = new MenuItemViewModel("Inactive", ViewMappedLSEInvalidStatusWordsCommand);
            m_viewMappedMissingStatusWordsMenuItem = new MenuItemViewModel("Unreported", ViewMappedMissingStatusWordsCommand);
            m_viewActiveStatusWordsMenuItem = new MenuItemViewModel("Active", ViewActiveStatusWordsCommand);
            m_viewStatusWordsMenuItem = new MenuItemViewModel("View Status Words", null);
            m_viewStatusWordsMenuItem.AddMenuItem(m_viewAllStatusWordsMenuItem);
            m_viewStatusWordsMenuItem.AddMenuItem(m_viewMappedStatusWordsMenuItem);
            m_viewStatusWordsMenuItem.AddMenuItem(m_viewMappedMissingStatusWordsMenuItem);
            m_viewStatusWordsMenuItem.AddMenuItem(m_viewMappedLSEInvalidStatusWordsMenuItem);
            m_viewStatusWordsMenuItem.AddMenuItem(m_viewActiveStatusWordsMenuItem);
            #endregion

            #region [ View Voltage Phasors ]
            m_viewModeledVoltagesMenuItem = new MenuItemViewModel("Modeled", ViewModeledVoltagesCommand);
            m_viewExpectedVoltagesMenuItem = new MenuItemViewModel("Expected", ViewExpectedVoltagesCommand);
            m_viewActiveVoltagesMenuItem = new MenuItemViewModel("Active", ViewActiveVoltagesCommand);
            m_viewInactiveVoltagesMenuItem = new MenuItemViewModel("Inactive", ViewInactiveVoltagesCommand);
            m_viewReportedVoltagesMenuItem = new MenuItemViewModel("Reported", ViewReportedVoltagesCommand);
            m_viewUnreportedVoltagesMenuItem = new MenuItemViewModel("Unreported", ViewUnreportedVoltagesCommand);
            m_viewActiveVoltagesByStatusWordMenuItem = new MenuItemViewModel("Active by Status Word", ViewActiveVoltagesByStatusWordCommand);
            m_viewInactiveVoltagesByStatusWordMenuItem = new MenuItemViewModel("Inactive by Status Word", ViewInactiveVoltagesByStatusWordCommand);
            m_viewInactiveVoltagesByMeasurementMenuItem = new MenuItemViewModel("Inactive by Measurement", ViewInactiveVoltagesByMeasurementCommand);
            m_viewVoltagePhasorsMenuItem = new MenuItemViewModel("View Voltages", null);
            m_viewVoltagePhasorsMenuItem.AddMenuItem(m_viewModeledVoltagesMenuItem);
            m_viewVoltagePhasorsMenuItem.AddMenuItem(m_viewExpectedVoltagesMenuItem);
            m_viewVoltagePhasorsMenuItem.AddMenuItem(m_viewInactiveVoltagesByStatusWordMenuItem);
            m_viewVoltagePhasorsMenuItem.AddMenuItem(m_viewInactiveVoltagesByMeasurementMenuItem);
            m_viewVoltagePhasorsMenuItem.AddMenuItem(m_viewInactiveVoltagesMenuItem);
            m_viewVoltagePhasorsMenuItem.AddMenuItem(m_viewActiveVoltagesByStatusWordMenuItem);
            m_viewVoltagePhasorsMenuItem.AddMenuItem(m_viewActiveVoltagesMenuItem);
            #endregion

            #region [ View Current Flow Phasors ]
            m_viewModeledCurrentFlowsMenuItem = new MenuItemViewModel("Modeled", ViewModeledCurrentFlowsCommand);
            m_viewExpectedCurrentFlowsMenuItem = new MenuItemViewModel("Expected", ViewExpectedCurrentFlowsCommand);
            m_viewActiveCurrentFlowsMenuItem = new MenuItemViewModel("Active", ViewActiveCurrentFlowsCommand);
            m_viewInactiveCurrentFlowsMenuItem = new MenuItemViewModel("Inactive", ViewInactiveCurrentFlowsCommand);
            m_viewIncludedCurrentFlowsMenuItem = new MenuItemViewModel("Included", ViewIncludedCurrentFlowsCommand);
            m_viewExcludedCurrentFlowsMenuItem = new MenuItemViewModel("Excluded", ViewExcludedCurrentFlowsCommand);
            m_viewReportedCurrentFlowsMenuItem = new MenuItemViewModel("Reported", ViewReportedCurrentFlowsCommand);
            m_viewUnreportedCurrentFlowsMenuItem = new MenuItemViewModel("Unreported", ViewUnreportedCurrentFlowsCommand);
            m_viewActiveCurrentFlowsByStatusWordMenuItem = new MenuItemViewModel("Active by Status Word", ViewActiveCurrentFlowsByStatusWordCommand);
            m_viewInactiveCurrentFlowsByStatusWordMenuItem = new MenuItemViewModel("Inactive by Status Word", ViewInactiveCurrentFlowsByStatusWordCommand);
            m_viewInactiveCurrentFlowsByMeasurementMenuItem = new MenuItemViewModel("Inactive by Measurement", ViewInactiveCurrentFlowsByMeasurementCommand);
            m_viewCurrentFlowPhasorsMenuItem = new MenuItemViewModel("View Current Flows", null);
            m_viewCurrentFlowPhasorsMenuItem.AddMenuItem(m_viewModeledCurrentFlowsMenuItem);
            m_viewCurrentFlowPhasorsMenuItem.AddMenuItem(m_viewExpectedCurrentFlowsMenuItem);
            m_viewCurrentFlowPhasorsMenuItem.AddMenuItem(m_viewInactiveCurrentFlowsByStatusWordMenuItem);
            m_viewCurrentFlowPhasorsMenuItem.AddMenuItem(m_viewInactiveCurrentFlowsByMeasurementMenuItem);
            m_viewCurrentFlowPhasorsMenuItem.AddMenuItem(m_viewInactiveCurrentFlowsMenuItem);
            m_viewCurrentFlowPhasorsMenuItem.AddMenuItem(m_viewActiveCurrentFlowsByStatusWordMenuItem);
            m_viewCurrentFlowPhasorsMenuItem.AddMenuItem(m_viewActiveCurrentFlowsMenuItem);
            m_viewCurrentFlowPhasorsMenuItem.AddMenuItem(m_viewIncludedCurrentFlowsMenuItem);
            m_viewCurrentFlowPhasorsMenuItem.AddMenuItem(m_viewExcludedCurrentFlowsMenuItem);
            #endregion
            
            #region [ View Current Injection Phasors ]
            m_viewModeledCurrentInjectionsMenuItem = new MenuItemViewModel("Modeled", ViewModeledCurrentInjectionsCommand);
            m_viewExpectedCurrentInjectionsMenuItem = new MenuItemViewModel("Expected", ViewExpectedCurrentInjectionsCommand);
            m_viewActiveCurrentInjectionsMenuItem = new MenuItemViewModel("Active", ViewActiveCurrentInjectionsCommand);
            m_viewInactiveCurrentInjectionsMenuItem = new MenuItemViewModel("Inactive", ViewInactiveCurrentInjectionsCommand);
            m_viewReportedCurrentInjectionsMenuItem = new MenuItemViewModel("Reported", ViewReportedCurrentInjectionsCommand);
            m_viewUnreportedCurrentInjectionsMenuItem = new MenuItemViewModel("Unreported", ViewUnreportedCurrentInjectionsCommand);
            m_viewActiveCurrentInjectionsByStatusWordMenuItem = new MenuItemViewModel("Active by Status Word", ViewActiveCurrentInjectionsByStatusWordCommand);
            m_viewInactiveCurrentInjectionsByStatusWordMenuItem = new MenuItemViewModel("Inactive by Status Word", ViewInactiveCurrentInjectionsByStatusWordCommand);
            m_viewInactiveCurrentInjectionsByMeasurementMenuItem = new MenuItemViewModel("Inactive by Measurement", ViewInactiveCurrentInjectionsByMeasurementCommand);
            m_viewCurrentInjectionPhasorsMenuItem = new MenuItemViewModel("View Current Injections", null);
            m_viewCurrentInjectionPhasorsMenuItem.AddMenuItem(m_viewModeledCurrentInjectionsMenuItem);
            m_viewCurrentInjectionPhasorsMenuItem.AddMenuItem(m_viewExpectedCurrentInjectionsMenuItem);
            m_viewCurrentInjectionPhasorsMenuItem.AddMenuItem(m_viewInactiveCurrentInjectionsByStatusWordMenuItem);
            m_viewCurrentInjectionPhasorsMenuItem.AddMenuItem(m_viewInactiveCurrentInjectionsByMeasurementMenuItem);
            m_viewCurrentInjectionPhasorsMenuItem.AddMenuItem(m_viewInactiveCurrentInjectionsMenuItem);
            m_viewCurrentInjectionPhasorsMenuItem.AddMenuItem(m_viewActiveCurrentInjectionsByStatusWordMenuItem);
            m_viewCurrentInjectionPhasorsMenuItem.AddMenuItem(m_viewActiveCurrentInjectionsMenuItem);
            #endregion

            m_viewObservableNodesMenuItem = new MenuItemViewModel("View Observable Nodes", ViewObservableNodesCommand);
            m_viewSubstationAdjacencyListMenuItem = new MenuItemViewModel("View Substation Adjacency List", ViewSubstationAdjacencyListCommand);
            m_viewTransmissionLineAdjacencyListMenuItem = new MenuItemViewModel("View Transmission Line Adjacency List", ViewTransmissionLineAdjacencyListCommand);
            m_viewCalculatedImpedancesMenuItem = new MenuItemViewModel("View Calculated Impedances", ViewCalculatedImpedancesCommand);
            m_viewSeriesCompensatorInferenceDataMenuItem = new MenuItemViewModel("View Series Compensator Inference Data", m_viewSeriesCompensatorInferenceDataCommand);
            m_viewSeriesCompensatorsMenuItem = new MenuItemViewModel("View Series Compensators", ViewSeriesCompensatorsCommand);
            m_viewTransformersMenuItem = new MenuItemViewModel("View Transformers", ViewTransformersCommand);

            m_viewStatusWordsMenuItem = new MenuItemViewModel("View Status Words", null);
            m_viewStatusWordsMenuItem.AddMenuItem(m_viewAllStatusWordsMenuItem);
            m_viewStatusWordsMenuItem.AddMenuItem(m_viewMappedStatusWordsMenuItem);
            m_viewStatusWordsMenuItem.AddMenuItem(m_viewMappedMissingStatusWordsMenuItem);
            m_viewStatusWordsMenuItem.AddMenuItem(m_viewMappedLSEInvalidStatusWordsMenuItem);

            m_viewModeledBreakerStatusesMenuItem = new MenuItemViewModel("View Modeled Breaker Statuses", ViewModeledBreakerStatusesCommand);
            m_viewExpectedBreakerStatusesMenuItem = new MenuItemViewModel("View Expected Breaker Statuses", ViewExpectedBreakerStatusesCommand);
            m_viewReportedBreakerStatusesMenuItem = new MenuItemViewModel("View Reported BreakerStatuses", ViewReportedBreakerStatusesCommand);
            m_viewCircuitBreakersMenuItem = new MenuItemViewModel("View Circuit Breakers", ViewCircuitBreakersCommand);
            m_viewSwitchesMenuItem = new MenuItemViewModel("View Switches", ViewSwitchesCommand);
            m_viewAllSwitchingDevicesMenuItem = new MenuItemViewModel("View All Switching Devices", ViewAllSwitchingDevicesCommand);

            m_viewSwitchingDeviceStatusesMenuItem = new MenuItemViewModel("View Switching Device Statuses", null);
            m_viewSwitchingDeviceStatusesMenuItem.AddMenuItem(m_viewModeledBreakerStatusesMenuItem);
            m_viewSwitchingDeviceStatusesMenuItem.AddMenuItem(m_viewExpectedBreakerStatusesMenuItem);
            m_viewSwitchingDeviceStatusesMenuItem.AddMenuItem(m_viewReportedBreakerStatusesMenuItem);
            m_viewSwitchingDeviceStatusesMenuItem.AddMenuItem(m_viewCircuitBreakersMenuItem);
            m_viewSwitchingDeviceStatusesMenuItem.AddMenuItem(m_viewSwitchesMenuItem);
            m_viewSwitchingDeviceStatusesMenuItem.AddMenuItem(m_viewAllSwitchingDevicesMenuItem);

            m_inspectMenuItem = new MenuItemViewModel("Inspect", null);
            m_inspectMenuItem.AddMenuItem(m_viewComponentSummaryMenuItem);
            m_inspectMenuItem.AddMenuItem(m_viewMeasurementsMenuItem);
            m_inspectMenuItem.AddMenuItem(m_viewStatusWordsMenuItem);
            m_inspectMenuItem.AddMenuItem(m_viewVoltagePhasorsMenuItem);
            m_inspectMenuItem.AddMenuItem(m_viewCurrentFlowPhasorsMenuItem);
            m_inspectMenuItem.AddMenuItem(m_viewCurrentInjectionPhasorsMenuItem);
            m_inspectMenuItem.AddMenuItem(m_viewSwitchingDeviceStatusesMenuItem);
            m_inspectMenuItem.AddMenuItem(m_viewObservableNodesMenuItem);
            m_inspectMenuItem.AddMenuItem(m_viewSubstationAdjacencyListMenuItem);
            m_inspectMenuItem.AddMenuItem(m_viewTransmissionLineAdjacencyListMenuItem);
            m_inspectMenuItem.AddMenuItem(m_viewCalculatedImpedancesMenuItem);
            m_inspectMenuItem.AddMenuItem(m_viewSeriesCompensatorInferenceDataMenuItem);
            m_inspectMenuItem.AddMenuItem(m_viewSeriesCompensatorsMenuItem);
            m_inspectMenuItem.AddMenuItem(m_viewTransformersMenuItem);

            #endregion

            #region [ MENU BAR --> OFFLINE ANALYSIS ]
            m_offlineAnalysisMenuItem = new MenuItemViewModel("Offline Analysis", null);
            m_offlineAnalysisMenuItem.AddMenuItem(m_setupMenuItem);
            m_offlineAnalysisMenuItem.AddMenuItem(m_observabilityMenuItem);
            m_offlineAnalysisMenuItem.AddMenuItem(m_matricesMenuItem);
            m_offlineAnalysisMenuItem.AddMenuItem(m_computationMenuItem);
            m_offlineAnalysisMenuItem.AddMenuItem(m_inspectMenuItem);
            #endregion

            #region [ MENU BAR --> UTILITIES --> MODEL ACTIONS ]
            m_keyifyPerformanceMetricsMenuItem = new MenuItemViewModel("Keyify Performance Metrics", KeyifyPerformanceMetricsCommand);
            m_keyifyTopologyMetricsMenuItem = new MenuItemViewModel("Keyify Topology Metrics", KeyifyTopologyMetricsCommand);
            m_unkeyifyModelMenuItem = new MenuItemViewModel("Unkeyify Model", UnkeyifyModelCommand);
            m_pruneModelMenuItem = new MenuItemViewModel("Prune Model", PruneModelCommand);
            m_pruneModelByVoltageLevelMenuItem = new MenuItemViewModel("Prune Model by Voltage Level", PruneModelByVoltageLevelCommand);
            m_pruneModelBySubstationMenuItem = new MenuItemViewModel("Prune Model by Substation", PruneModelBySubstationCommand);
            m_pruneModelByCompanyMenuItem = new MenuItemViewModel("Prune Model by Company", PruneModelByCompanyCommand);


            m_createVoltageEstimateMeasurementsMenuItem = new MenuItemViewModel("Voltage Estimates", CreateVoltageEstimateMeasurementsCommand);
            m_createCurrentFlowEstimateMeasurementsMenuItem = new MenuItemViewModel("Current Flow Estimates", CreateCurrentFlowEstimateMeasurementsCommand);
            m_createCurrentInjectionEstimateMeasurementsMenuItem = new MenuItemViewModel("Current Injection Estimates", CreateCurrentInjectionEstimateMeasurementsCommand);
            m_createVoltageResidualMeasurementsMenuItem = new MenuItemViewModel("Voltage Residuals", CreateVoltageResidualMeasurementsCommand);
            m_createCurrentFlowResidualMeasurementsMenuItem = new MenuItemViewModel("Current Flow Residuals", CreateCurrentFlowResidualMeasurementsCommand);
            m_createCurrentInjectionResidualMeasurementsMenuItem = new MenuItemViewModel("Current Injection Residuals", null);
            m_createCircuitBreakerStatusMeasurementsMenuItem = new MenuItemViewModel("Circuit Breaker Statuses", CreateCircuitBreakerStatusMeasurementsCommand);
            m_createSwitchStatusMeasurementsMenuItem = new MenuItemViewModel("Switch Statuses", CreateSwitchStatusMeasurementsCommand);
            m_createPerformanceMetricsMeasurementsMenuItem = new MenuItemViewModel("Performance Metrics", CreatePerformanceMetricMeasurementsCommand);
            m_createTopologyProfilingMeasurementsMenuItem = new MenuItemViewModel("Topology Profiling", CreateTopologyProfilingMeasurementsCommand);
            m_createMeasurementValidationFlagMeasurementsMenuItem = new MenuItemViewModel("Measurement Validation Flags", CreateMeasurementValidationFlagMeasurementsCommand);
            m_createStatusFlagsMenuItem = new MenuItemViewModel("Status Flags", CreateStatusFlagsCommand);
            m_createMeasurementsMenuItem = new MenuItemViewModel("Create Measurements", null);
            m_createMeasurementsMenuItem.AddMenuItem(m_createVoltageEstimateMeasurementsMenuItem);
            m_createMeasurementsMenuItem.AddMenuItem(m_createCurrentFlowEstimateMeasurementsMenuItem);
            m_createMeasurementsMenuItem.AddMenuItem(m_createCurrentInjectionEstimateMeasurementsMenuItem);
            m_createMeasurementsMenuItem.AddMenuItem(m_createVoltageResidualMeasurementsMenuItem);
            m_createMeasurementsMenuItem.AddMenuItem(m_createCurrentFlowResidualMeasurementsMenuItem);
            m_createMeasurementsMenuItem.AddMenuItem(m_createCurrentInjectionResidualMeasurementsMenuItem);
            m_createMeasurementsMenuItem.AddMenuItem(m_createCircuitBreakerStatusMeasurementsMenuItem);
            m_createMeasurementsMenuItem.AddMenuItem(m_createSwitchStatusMeasurementsMenuItem);
            m_createMeasurementsMenuItem.AddMenuItem(m_createPerformanceMetricsMeasurementsMenuItem);
            m_createMeasurementsMenuItem.AddMenuItem(m_createTopologyProfilingMeasurementsMenuItem);
            m_createMeasurementsMenuItem.AddMenuItem(m_createMeasurementValidationFlagMeasurementsMenuItem);
            m_createMeasurementsMenuItem.AddMenuItem(m_createStatusFlagsMenuItem);

            m_mapVoltageEstimateMeasurementsMenuItem = new MenuItemViewModel("Voltage Estimates", MapVoltageEstimateMeasurementsCommand);
            m_mapCurrentFlowEstimateMeasurementsMenuItem = new MenuItemViewModel("Current Flow Estimates", MapCurrentFlowEstimateMeasurementsCommand);
            m_mapCurrentInjectionEstimateMeasurementsMenuItem = new MenuItemViewModel("Current Injection Estimates", MapCurrentInjectionEstimateMeasurementsCommand);
            m_mapVoltageResidualMeasurementsMenuItem = new MenuItemViewModel("Voltage Residuals", MapVoltageResidualMeasurementsCommand);
            m_mapCurrentFlowResidualMeasurementsMenuItem = new MenuItemViewModel("Current Flow Residuals", MapCurrentFlowResidualMeasurementsCommand);
            m_mapCurrentInjectionResidualMeasurementsMenuItem = new MenuItemViewModel("Current Injection Residuals", null);
            m_mapCircuitBreakerStatusMeasurementsMenuItem = new MenuItemViewModel("Circuit Breaker Statuses", MapCircuitBreakerStatusMeasurementsCommand);
            m_mapSwitchStatusMeasurementsMenuItem = new MenuItemViewModel("Switch Statuses", MapSwitchStatusMeasurementsCommand);
            m_mapPerformanceMetricsMeasurementsMenuItem = new MenuItemViewModel("Performance Metrics", MapPerformanceMetricMeasurementsCommand);
            m_mapTopologyProfilingMeasurementsMenuItem = new MenuItemViewModel("Topology Profiling", MapTopologyProfilingMeasurementsCommand);
            m_mapMeasurementValidationFlagMeasurementsMenuItem = new MenuItemViewModel("Measurement Validation Flags", MapMeasurementValidationFlagMeasurementsCommand);
            m_mapCreatedMeasurementsMenuItem = new MenuItemViewModel("Map Created Measurements", null);
            m_mapCreatedMeasurementsMenuItem.AddMenuItem(m_mapVoltageEstimateMeasurementsMenuItem);
            m_mapCreatedMeasurementsMenuItem.AddMenuItem(m_mapCurrentFlowEstimateMeasurementsMenuItem);
            m_mapCreatedMeasurementsMenuItem.AddMenuItem(m_mapCurrentInjectionEstimateMeasurementsMenuItem);
            m_mapCreatedMeasurementsMenuItem.AddMenuItem(m_mapVoltageResidualMeasurementsMenuItem);
            m_mapCreatedMeasurementsMenuItem.AddMenuItem(m_mapCurrentFlowResidualMeasurementsMenuItem);
            m_mapCreatedMeasurementsMenuItem.AddMenuItem(m_mapCurrentInjectionResidualMeasurementsMenuItem);
            m_mapCreatedMeasurementsMenuItem.AddMenuItem(m_mapCircuitBreakerStatusMeasurementsMenuItem);
            m_mapCreatedMeasurementsMenuItem.AddMenuItem(m_mapSwitchStatusMeasurementsMenuItem);
            m_mapCreatedMeasurementsMenuItem.AddMenuItem(m_mapPerformanceMetricsMeasurementsMenuItem);
            m_mapCreatedMeasurementsMenuItem.AddMenuItem(m_mapTopologyProfilingMeasurementsMenuItem);
            m_mapCreatedMeasurementsMenuItem.AddMenuItem(m_mapMeasurementValidationFlagMeasurementsMenuItem);

            m_modelActionsMenuItem = new MenuItemViewModel("Model Actions", null);
            m_modelActionsMenuItem.AddMenuItem(m_keyifyPerformanceMetricsMenuItem);
            m_modelActionsMenuItem.AddMenuItem(m_keyifyTopologyMetricsMenuItem);
            m_modelActionsMenuItem.AddMenuItem(m_unkeyifyModelMenuItem);
            m_modelActionsMenuItem.AddMenuItem(m_createMeasurementsMenuItem);
            m_modelActionsMenuItem.AddMenuItem(m_mapCreatedMeasurementsMenuItem);
            m_modelActionsMenuItem.AddMenuItem(m_pruneModelMenuItem);
            m_modelActionsMenuItem.AddMenuItem(m_pruneModelByVoltageLevelMenuItem);
            m_modelActionsMenuItem.AddMenuItem(m_pruneModelBySubstationMenuItem);
            m_modelActionsMenuItem.AddMenuItem(m_pruneModelByCompanyMenuItem);
            #endregion

            #region [ MENU BAR --> UTILITIES --> MEASUREMENT ACTIONS ]
            m_generateMeasurementSamplesMenuItem = new MenuItemViewModel("Generate Measurement Samples From CSV", GenerateMeasurementSamplesCommand);
            m_measurementActionsMenuItem = new MenuItemViewModel("Measurement Actions", null);
            m_measurementActionsMenuItem.AddMenuItem(m_generateMeasurementSamplesMenuItem);
            #endregion

            #region [ MENU BAR --> UTILITIES ]
            m_utilitiesMenuItem = new MenuItemViewModel("Utilities", null);
            m_utilitiesMenuItem.AddMenuItem(m_modelActionsMenuItem);
            m_utilitiesMenuItem.AddMenuItem(m_measurementActionsMenuItem);
            #endregion

            #region [ MENU BAR --> OPENECA ]
            m_connectMenuItem = new MenuItemViewModel("Connect", ConnectToOpenEcaCommand);
            m_refreshMetaDataMenuItem = new MenuItemViewModel("Refresh Meta Data", RefreshMetaDataCommand);
            m_openEcaMenuItem = new MenuItemViewModel("openECA", null);
            m_startMeasurementSamplerMenuItem = new MenuItemViewModel("Start", StartMeasurementSamplerCommand);
            m_takeMeasurementSampleMenuItem = new MenuItemViewModel("Take Sample", TakeMeasurementSampleCommand);
            m_stopMeasurementSamplerMenuItem = new MenuItemViewModel("Stop", StopMeasurementSamplerCommand);
            m_measurementSamplerMenuItem = new MenuItemViewModel("Measurement Sampler", null);
            m_measurementSamplerMenuItem.AddMenuItem(m_startMeasurementSamplerMenuItem);
            m_measurementSamplerMenuItem.AddMenuItem(m_takeMeasurementSampleMenuItem);
            m_measurementSamplerMenuItem.AddMenuItem(m_stopMeasurementSamplerMenuItem);
            m_startLinearStateEstimatorMenuItem = new MenuItemViewModel("Start", StartLinearStateEstimatorCommand);
            m_stopLinearStateEstimatorMenuItem = new MenuItemViewModel("Stop", StopLinearStateEstimatorCommand);
            m_linearStateEstimatorMenuItem = new MenuItemViewModel("Linear State Estimator", null);
            m_linearStateEstimatorMenuItem.AddMenuItem(m_startLinearStateEstimatorMenuItem);
            m_linearStateEstimatorMenuItem.AddMenuItem(m_stopLinearStateEstimatorMenuItem);
            m_openEcaMenuItem.AddMenuItem(m_connectMenuItem);
            m_openEcaMenuItem.AddMenuItem(m_refreshMetaDataMenuItem);
            m_openEcaMenuItem.AddMenuItem(m_measurementSamplerMenuItem);
            //m_openEcaMenuItem.AddMenuItem(m_linearStateEstimatorMenuItem);
            #endregion

            #region [ MENU BAR --> FILE --> OPEN ]
            m_openNetworkModelMenuItem = new MenuItemViewModel("Xml Network Model", OpenFileCommand);
            m_openMeasurementSampleMenuItem = new MenuItemViewModel("Xml Measurement Sample", OpenMeasurementSampleFileCommand);
            m_openPsseRawFileMenuItem = new MenuItemViewModel("PSSE *.raw File", OpenPsseRawFileCommand);
            m_openHdbExportFilesMenuItem = new MenuItemViewModel("Hdb Export List File", OpenHdbExportFilesCommand);
            m_openTvaHdbExportFilesMenuItem = new MenuItemViewModel("TVA Hdb Export List File", OpenTvaHdbExportFilesCommand);
            m_openNodeExtensionFileMenuItem = new MenuItemViewModel("Node Extension File", OpenNodeExtensionFileCommand);
            m_openLineSegmentExtensionFileMenuItem = new MenuItemViewModel("Line Segment Extension File", OpenLineSegmentExtensionFileCommand);
            m_openBreakerExtensionFileMenuItem = new MenuItemViewModel("Circuit Breaker Extension File", OpenBreakerExtensionFileCommand);
            m_openShuntExtensionFileMenuItem = new MenuItemViewModel("Shunt Extension File", OpenShuntExtensionFileCommand);
            m_openTransformerExtensionFileMenuItem = new MenuItemViewModel("Transformer Extension File", OpenTransformerExtensionFileCommand);
            m_openMenuItem = new MenuItemViewModel("Open", null);
            m_openExtensionFileMenuItem = new MenuItemViewModel("Extension File", null);
            m_openExtensionFileMenuItem.AddMenuItem(m_openNodeExtensionFileMenuItem);
            m_openExtensionFileMenuItem.AddMenuItem(m_openLineSegmentExtensionFileMenuItem);
            m_openExtensionFileMenuItem.AddMenuItem(m_openBreakerExtensionFileMenuItem);
            m_openExtensionFileMenuItem.AddMenuItem(m_openShuntExtensionFileMenuItem);
            m_openExtensionFileMenuItem.AddMenuItem(m_openTransformerExtensionFileMenuItem);
            m_openMenuItem.AddMenuItem(m_openNetworkModelMenuItem);
            m_openMenuItem.AddMenuItem(m_openMeasurementSampleMenuItem);
            m_openMenuItem.AddMenuItem(m_openHdbExportFilesMenuItem);
            m_openMenuItem.AddMenuItem(m_openTvaHdbExportFilesMenuItem);
            m_openMenuItem.AddMenuItem(m_openExtensionFileMenuItem);
            m_openMenuItem.AddMenuItem(m_openPsseRawFileMenuItem);
            #endregion

            #region [ MENU BAR --> FILE --> SAVE ]
            m_saveNetworkModelMenuItem = new MenuItemViewModel("Xml Network Model", SaveFileCommand);
            m_saveNetworkSnapshotMenuItem = new MenuItemViewModel("Xml Network Snapshot", SaveNetworkSnapshotFileCommand);
            m_saveMeasurementSampleFileMenuItem = new MenuItemViewModel("Xml Measurement Sample", SaveMeasurementSampleFilesCommand);
            m_saveMenuItem = new MenuItemViewModel("Save", null);
            m_saveMenuItem.AddMenuItem(m_saveNetworkModelMenuItem);
            m_saveMenuItem.AddMenuItem(m_saveNetworkSnapshotMenuItem);
            m_saveMenuItem.AddMenuItem(m_saveMeasurementSampleFileMenuItem);
            #endregion

            #region [ MENU BAR --> FILE --> EXIT ]
            m_exitMenuItem = new MenuItemViewModel("Exit", null);
            #endregion

            #region [ MENU BAR --> FILE ]
            m_fileMenuItem = new MenuItemViewModel("File", null);
            m_fileMenuItem.AddMenuItem(m_openMenuItem);
            m_fileMenuItem.AddMenuItem(m_saveMenuItem);
            //m_fileMenuItem.AddMenuItem(m_exitMenuItem);
            #endregion

            #region [ MENU BAR ]
            m_menuBarItemViewModels = new List<MenuItemViewModel>();
            m_menuBarItemViewModels.Add(m_fileMenuItem);
            m_menuBarItemViewModels.Add(m_openEcaMenuItem);
            m_menuBarItemViewModels.Add(m_utilitiesMenuItem);
            m_menuBarItemViewModels.Add(m_offlineAnalysisMenuItem);
            #endregion

        }

        private void InitializeNetworkTree()
        {
            m_network = new Network();
            m_network.Initialize();
            m_network.Model = new NetworkModel();
            m_measurementSamples = new List<RawMeasurements>();
            m_retainedVoltageLevels = new List<VoltageLevel>();
            m_retainedSubstations = new List<Substation>();
            m_retainedCompanies = new List<Company>();
            m_networkTreeViewModel = new NetworkTreeViewModel(this, m_network, m_measurementSamples);
        }
        
        private void InitializeRecordDetail()
        {
            m_recordDetailViewModel = new RecordDetailViewModel(this);
        }

        #endregion

        #region [ Methods ]

        #region [ File Opening ] 

        private void OpenFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.DefaultExt = ".xml";
            openFileDialog.Filter = "Network Model (.xml)|*.xml";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    m_network = Network.DeserializeFromXml(openFileDialog.FileName);
                    ActionStatus = $"Opened network model from {openFileDialog.FileName}";

                    m_networkTreeViewModel.Network = m_network;
                    EnableControls();
                }
                catch (Exception exception)
                {
                    if (exception != null)
                    {
                        System.Windows.MessageBox.Show(exception.ToString(), "Failed to load selected file.");
                    }
                }
            }
        }

        private void OpenHdbExportFiles()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.DefaultExt = ".xml";
            openFileDialog.Filter = "Hdb Export List File (.xml)|*.xml";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    m_network = Network.FromHdbExport(openFileDialog.FileName, true, new List<string>(), useAreaLinks:false);
                    ActionStatus = $"Opened EMS network model from {openFileDialog.FileName}";

                    m_networkTreeViewModel.Network = m_network;
                    EnableControls();
                }
                catch (Exception exception)
                {
                    if (exception != null)
                    {
                        System.Windows.MessageBox.Show(exception.ToString(), "Failed to load selected file.");
                    }
                }
            }
        }

        private void OpenTvaHdbExportFiles()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.DefaultExt = ".xml";
            openFileDialog.Filter = "TVA Hdb Export List File (.xml)|*.xml";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    m_network = Network.FromHdbExport(openFileDialog.FileName, true, new List<string>(), useAreaLinks:true);
                    ActionStatus = $"Opened TVA network model from {openFileDialog.FileName}";

                    m_networkTreeViewModel.Network = m_network;
                    EnableControls();
    }
                catch (Exception exception)
                {
                    if (exception != null)
                    {
                        System.Windows.MessageBox.Show(exception.ToString(), "Failed to load selected file.");
                    }
                }
            }
        }

        private void OpenPsseRawFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.DefaultExt = ".raw";
            openFileDialog.Filter = "PSSE (.raw)|*.raw";
            EnableControls();

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    RawFile rawFile = RawFile.Read(openFileDialog.FileName);
                    m_network = Network.FromPsseRawFile(openFileDialog.FileName, rawFile.Version.ToString());
                    ActionStatus = $"Opened network model from {openFileDialog.FileName}";

                    m_networkTreeViewModel.Network = m_network;
                }
                catch (Exception exception)
                {
                    if (exception != null)
                    {
                        System.Windows.MessageBox.Show(exception.ToString(), "Failed to load selected file.");
                    }
                }
            }
        }

        private void TakeSample()
        {
            MeasurementSampler.ShouldTakeSample = true;
        }

        public void AddMeasurementSample(RawMeasurements sample)
        {
            m_measurementSamples.Add(sample);
            m_networkTreeViewModel.MeasurementSamples = m_measurementSamples;
            EnableControls();
        }

        private void OpenMeasurementSampleFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.DefaultExt = ".xml";
            openFileDialog.Filter = "Measurement Sample (.xml)|*.xml";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    m_measurementSamples.Add(RawMeasurements.DeserializeFromXml(openFileDialog.FileName));

                    m_networkTreeViewModel.MeasurementSamples = m_measurementSamples;
                    ActionStatus = $"Opened measurement sample from {openFileDialog.FileName}";
                    EnableControls();

                }
                catch (Exception exception)
                {
                    if (exception != null)
                    {
                        System.Windows.MessageBox.Show(exception.ToString(), "Failed to load selected file.");
                    }
                }
            }
        }
        
        private void OpenNodeExtensionFile()
        {
            if (OpenEcaConnection != null && OpenEcaConnection.IsConnected)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();

                openFileDialog.DefaultExt = ".txt";
                openFileDialog.Filter = "txt files (*.txt)|*.txt|csv files (*.csv)|*.csv|out files (*.out)|*.out";


                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    m_nodeExtensions = HdbReader.ReadNodeExtensionFile(openFileDialog.FileName);
                    ApplyNodeExtensions();
                    ActionStatus = $"Opened Node Extension File from {openFileDialog.FileName}";
                    //try
                    //{
                    //    m_nodeExtensions = HdbReader.ReadNodeExtensionFile(openFileDialog.FileName);
                    //    ApplyNodeExtensions();
                    //    ActionStatus = $"Opened Node Extension File from {openFileDialog.FileName}";
                    //}
                    //catch (Exception exception)
                    //{
                    //    if (exception != null)
                    //    {
                    //        System.Windows.MessageBox.Show(exception.ToString(), "Failed to load selected file.");
                    //    }
                    //}
                }
            }
            else
            {
                CommunicationStatus = "No Connection to openECA";
            }
        }

        private void ApplyNodeExtensions()
        {
            int unmappedExtensionCount = 0;
            foreach (SynchrophasorAnalytics.Hdb.Records.NodeExtension nodeExtension in m_nodeExtensions)
            {
                Node node = m_network.Model.Nodes.Find(x => x.Name == $"{nodeExtension.StationName}_{nodeExtension.Id}");
                if (node != null)
                {
                    node.Voltage.PositiveSequence.Measurement.MagnitudeKey = nodeExtension.MagnitudeHistorianId;
                    node.Voltage.PositiveSequence.Measurement.AngleKey = nodeExtension.AngleHistorianId;
                    DataRow row = m_openEcaConnection.Metadata.Tables["MeasurementDetail"].Select($"SignalAcronym = 'FLAG' AND SignalReference LIKE '{nodeExtension.DeviceName}*'").Single();
                    string guid = row["SignalID"].ToString();
                    StatusWord statusWord = m_network.Model.StatusWords.Find(x => x.Key == guid);
                    node.Voltage.Status = statusWord;
                }
                else
                {
                    unmappedExtensionCount++;
                }
            }
            SpecialStatus = $"Failed to map {unmappedExtensionCount} extension(s).";
        }

        private void OpenLineSegmentExtensionFile()
        {
            if (OpenEcaConnection != null && OpenEcaConnection.IsConnected)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();

                openFileDialog.DefaultExt = ".txt";
                openFileDialog.Filter = "txt files (*.txt)|*.txt|csv files (*.csv)|*.csv|out files (*.out)|*.out";


                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    m_lineSegmentExtensions = HdbReader.ReadLineSegmentExtensionFile(openFileDialog.FileName);
                    ApplyLineSegmentExtensions();
                    ActionStatus = $"Opened Line Segment Extension File from {openFileDialog.FileName}";
                    //try
                    //{
                    //    m_lineSegmentExtensions = HdbReader.ReadLineSegmentExtensionFile(openFileDialog.FileName);
                    //    ApplyLineSegmentExtensions();
                    //    ActionStatus = $"Opened Line Segment Extension File from {openFileDialog.FileName}";
                    //}
                    //    catch (Exception exception)
                    //{
                    //    if (exception != null)
                    //    {
                    //        System.Windows.MessageBox.Show(exception.ToString(), "Failed to load selected file.");
                    //    }
                    //}
                }
            }
            else
            {
                CommunicationStatus = "No Connection to openECA";
            }
        }

        private void ApplyLineSegmentExtensions()
        {
            int unmappedExtensionCount = 0;

            foreach (SynchrophasorAnalytics.Hdb.Records.LineSegmentExtension lineSegmentExtension in m_lineSegmentExtensions)
            {
                LineSegment lineSegment = m_network.Model.LineSegments.Find(x => x.InternalID == lineSegmentExtension.Number);
                if (lineSegment != null)
                {
                    CurrentFlowPhasorGroup fromSubstationCurrent = lineSegment.ParentTransmissionLine.FromSubstationCurrent;
                    fromSubstationCurrent.PositiveSequence.Measurement.MagnitudeKey = lineSegmentExtension.FromNodeMagnitudeHistorianId;
                    fromSubstationCurrent.PositiveSequence.Measurement.AngleKey = lineSegmentExtension.FromNodeAngleHistorianId;
                    CurrentFlowPhasorGroup toSubstationCurrent = lineSegment.ParentTransmissionLine.ToSubstationCurrent;
                    toSubstationCurrent.PositiveSequence.Measurement.MagnitudeKey = lineSegmentExtension.ToNodeMagnitudeHistorianId;
                    toSubstationCurrent.PositiveSequence.Measurement.AngleKey = lineSegmentExtension.ToNodeAngleHistorianId;
                    if (lineSegmentExtension.FromNodeDeviceName != "Undefined")
                    {
                        DataRow row = m_openEcaConnection.Metadata.Tables["MeasurementDetail"].Select($"SignalAcronym = 'FLAG' AND SignalReference LIKE '{lineSegmentExtension.FromNodeDeviceName}*'").Single();
                        string guid = row["SignalID"].ToString();
                        StatusWord statusWord = m_network.Model.StatusWords.Find(x => x.Key == guid);
                        fromSubstationCurrent.Status = statusWord;
                    }
                    if (lineSegmentExtension.ToNodeDeviceName != "Undefined")
                    {
                        DataRow row = m_openEcaConnection.Metadata.Tables["MeasurementDetail"].Select($"SignalAcronym = 'FLAG' AND SignalReference LIKE '{lineSegmentExtension.ToNodeDeviceName}*'").Single();
                        string guid = row["SignalID"].ToString();
                        StatusWord statusWord = m_network.Model.StatusWords.Find(x => x.Key == guid);
                        toSubstationCurrent.Status = statusWord;
                    }
                }
                else
                {
                    unmappedExtensionCount++;
                }
            }
            SpecialStatus = $"Failed to map {unmappedExtensionCount} extension(s).";
        }

        private void OpenBreakerExtensionFile()
        {
            if (OpenEcaConnection != null && OpenEcaConnection.IsConnected)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();

                openFileDialog.DefaultExt = ".txt";
                openFileDialog.Filter = "txt files (*.txt)|*.txt|csv files (*.csv)|*.csv|out files (*.out)|*.out";


                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    m_circuitBreakerExtensions = HdbReader.ReadCircuitBreakerExtensionFile(openFileDialog.FileName);
                    ApplyCircuitBreakerExtensions();
                    ActionStatus = $"Opened Circuit Breaker Extension File from {openFileDialog.FileName}";
                    //try
                    //{
                    //    m_circuitBreakerExtensions = HdbReader.ReadCircuitBreakerExtensionFile(openFileDialog.FileName);
                    //    ApplyCircuitBreakerExtensions();
                    //    ActionStatus = $"Opened Circuit Breaker Extension File from {openFileDialog.FileName}";
                    //}
                    //    catch (Exception exception)
                    //{
                    //    if (exception != null)
                    //    {
                    //        System.Windows.MessageBox.Show(exception.ToString(), "Failed to load selected file.");
                    //    }
                    //}
                }
            }
            else
            {
                CommunicationStatus = "No Connection to openECA";
            }
        }

        private void ApplyCircuitBreakerExtensions()
        {

        }

        private void OpenShuntExtensionFile()
        {
            if (OpenEcaConnection != null && OpenEcaConnection.IsConnected)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();

                openFileDialog.DefaultExt = ".txt";
                openFileDialog.Filter = "txt files (*.txt)|*.txt|csv files (*.csv)|*.csv|out files (*.out)|*.out";


                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    m_shuntExtensions = HdbReader.ReadShuntExtensionFile(openFileDialog.FileName);
                    ApplyShuntExtensions();
                    ActionStatus = $"Opened Shunt Extension File from {openFileDialog.FileName}";
                    //try
                    //{
                    //    m_shuntExtensions = HdbReader.ReadShuntExtensionFile(openFileDialog.FileName);
                    //    ApplyShuntExtensions();
                    //    ActionStatus = $"Opened Shunt Extension File from {openFileDialog.FileName}";
                    //}
                    //    catch (Exception exception)
                    //{
                    //    if (exception != null)
                    //    {
                    //        System.Windows.MessageBox.Show(exception.ToString(), "Failed to load selected file.");
                    //    }
                    //}
                }
            }
            else
            {
                CommunicationStatus = "No connection to openECA";
            }
        }

        private void ApplyShuntExtensions()
        {

        }

        private void OpenTransformerExtensionFile()
        {
            if (OpenEcaConnection != null && OpenEcaConnection.IsConnected)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();

                openFileDialog.DefaultExt = ".txt";
                openFileDialog.Filter = "txt files (*.txt)|*.txt|csv files (*.csv)|*.csv|out files (*.out)|*.out";


                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    m_transformerExtensions = HdbReader.ReadTransformerExtensionFile(openFileDialog.FileName);
                    ApplyTransformerExtensions();
                    ActionStatus = $"Opened Transformer Extension File from {openFileDialog.FileName}";
                    //try
                    //{
                    //    m_transformerExtensions = HdbReader.ReadTransformerExtensionFile(openFileDialog.FileName);
                    //    ApplyTransformerExtensions();
                    //    ActionStatus = $"Opened Transformer Extension File from {openFileDialog.FileName}";
                    //}
                    //    catch (Exception exception)
                    //{
                    //    if (exception != null)
                    //    {
                    //        System.Windows.MessageBox.Show(exception.ToString(), "Failed to load selected file.");
                    //    }
                    //}
                }
            }
            else
            {
                CommunicationStatus = "No connection to openECA";
            }
        }

        private void ApplyTransformerExtensions()
        {

        }

        #endregion

        #region [ User Interface ]

        private void ChangeSelectedElement()
        {
            System.Windows.MessageBox.Show("Selection Changed");
        }

        private void ViewDetail()
        {
            m_recordDetailViewModel.ClearRecordDetailView();
            m_recordDetailViewModel.AddViewModel(m_networkTreeViewModel.SelectedElement.Value);
        }


        private void RefreshNetworkTree()
        {
            m_networkTreeViewModel.RefreshNetworkTree();
        }

        #endregion

        #region [ File Savings ] 

        private void SaveFile()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.DefaultExt = ".xml";
            saveFileDialog.Filter = "Network Model (.xml)|*.xml";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    m_network.Initialize();
                    m_network.SerializeToXml(saveFileDialog.FileName);
                    ActionStatus = $"Saved network model to {saveFileDialog.FileName}";
                }
                catch (Exception exception)
                {
                    if (exception != null)
                    {
                        System.Windows.MessageBox.Show(exception.ToString(), "Failed to save xml file.");
                    }
                }
            }
        }

        private void SaveMeasurementSampleFile()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.DefaultExt = ".xml";
            saveFileDialog.Filter = "Measurement Sample (.xml)|*.xml";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if (m_networkTreeViewModel.SelectedElement.Value.Element is RawMeasurements)
                    {
                        // Create an XmlSerializer with the type of Network
                        XmlSerializer serializer = new XmlSerializer(typeof(RawMeasurements));

                        // Open a connection to the file and path.
                        TextWriter writer = new StreamWriter(saveFileDialog.FileName);

                        // Serialize this instance of NetworkMeasurements
                        serializer.Serialize(writer, (m_networkTreeViewModel.SelectedElement.Value.Element as RawMeasurements));

                        // Close the connection
                        writer.Close();
                        ActionStatus = $"Saved measurement sample to {saveFileDialog.FileName}";
                    }                    
                }
                catch (Exception exception)
                {
                    if (exception != null)
                    {
                        System.Windows.MessageBox.Show(exception.ToString(), "Failed to save xml file.");
                    }
                }
            }
        }

        private void SaveNetworkSnapshotFile()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.DefaultExt = ".xml";
            saveFileDialog.Filter = "Network Model Snapshot (.xml)|*.xml";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    m_network.SerializeData(true);
                    m_network.SerializeToXml(saveFileDialog.FileName);
                    ActionStatus = $"Saved network model snapshot to {saveFileDialog.FileName}";
                }
                catch (Exception exception)
                {
                    if (exception != null)
                    {
                        System.Windows.MessageBox.Show(exception.ToString(), "Failed to save xml file.");
                    }
                }
            }
        }

        #endregion

        #region [ Measurement Sampler ]

        private void StartMeasurementSampler()
        {
            MeasurementSampler.Start();
        }

        private void StopMeasurementSampler()
        {
            MeasurementSampler.Stop();
        }

        #endregion

        #region [ Linear State Estimator ]

        private void StartLinearStateEstimator()
        {

        }

        private void StopLinearStateEstimator()
        {

        }

        #endregion

        #region [ Keyification ]

        private void KeyifyPerformanceMetrics()
        {
            if (m_network != null && m_network.Model != null)
            {
                m_network.PerformanceMetrics.Keyify();
                ActionStatus = "Keyified LSE Performance Metrics.";
            }
        }

        private void KeyifyTopologyMetrics()
        {
            if (m_network != null && m_network.Model != null)
            {
                foreach (Substation substation in m_network.Model.Substations)
                {
                    substation.Keyify();
                }
                foreach (Node node in m_network.Model.Nodes)
                {
                    node.Keyify();
                }
                ActionStatus = "Keyified LSE Topology Metrics.";
            }
        }

        private void KeyifyValidationFlags()
        {
            if (m_network != null && m_network.Model != null)
            {
                foreach (VoltagePhasorGroup voltage in m_network.Model.Voltages)
                {
                    voltage.KeyifyValidationFlag(voltage.MeasuredNode.Name);
                }

            }
        }

        private void KeyifyCircuitBreakers()
        {
            if (m_network != null && m_network.Model != null)
            {
                foreach (CircuitBreaker breaker in m_network.Model.CircuitBreakers)
                {
                    breaker.MeasurementKey = $"{breaker.ParentSubstation.Name}.{breaker.Name}.CB";
                }
            }
        }

        private void KeyifySwitches()
        {
            if (m_network != null && m_network.Model != null)
            {
                foreach (SynchrophasorAnalytics.Modeling.Switch switchingDevice in m_network.Model.Switches)
                {
                    switchingDevice.MeasurementKey = $"{switchingDevice.ParentSubstation.Name}.{switchingDevice.Name}.CB";
                }
            }
        }

        private void UnkeyifyModel()
        {
            if (m_network != null && m_network.Model != null)
            {
                m_network.Model.Unkeyify();
                ActionStatus = "Set all keys to 'Undefined'";
            }
        }

        #endregion

        private void GenerateMeasurementSamplesFromCsv()
        {
            string columnMappingFile = null;
            string csvFile = null;

            OpenFileDialog columnMappingOpenFileDialog = new OpenFileDialog();

            columnMappingOpenFileDialog.DefaultExt = ".txt";
            columnMappingOpenFileDialog.Filter = "Column Mapping File (.txt)|*.txt";

            if (columnMappingOpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    columnMappingFile = columnMappingOpenFileDialog.FileName;
                    ActionStatus = "Selected column mapping file.";

                }
                catch (Exception exception)
                {
                    if (exception != null)
                    {
                        System.Windows.MessageBox.Show(exception.ToString(), "Failed to load selected file.");
                    }
                }
            }

            OpenFileDialog csvOpenFileDialog = new OpenFileDialog();

            csvOpenFileDialog.DefaultExt = ".csv";
            csvOpenFileDialog.Filter = "Column Mapping File (.csv)|*.csv";

            if (csvOpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    csvFile = csvOpenFileDialog.FileName;
                    ActionStatus = "Selected csv file.";

                }
                catch (Exception exception)
                {
                    if (exception != null)
                    {
                        System.Windows.MessageBox.Show(exception.ToString(), "Failed to load selected file.");
                    }
                }
            }


            if (csvFile != null && columnMappingFile != null)
            {
                List<RawMeasurements> measurementSamples = RawMeasurements.FromCsv(csvFile, columnMappingFile, true);

                foreach (RawMeasurements sample in measurementSamples)
                {
                    m_measurementSamples.Add(sample);
                }
                ActionStatus = $"Created {m_measurementSamples.Count} measurement samples from CSV file";
                m_networkTreeViewModel.MeasurementSamples = m_measurementSamples;
            }

        }

        #region [ Model Pruning ]

        private void PruneModel()
        {
            ActionStatus = "Entering Pruning Mode...";
            bool wasInPruningMode = m_network.Model.InPruningMode;
            m_network.Model.InPruningMode = true;

            ActionStatus = "Determining Active Current Flows...";
            m_network.Model.DetermineActiveCurrentFlows();
            ActionStatus = "Determining Active Current Injections...";
            m_network.Model.DetermineActiveCurrentInjections();
            ActionStatus = "Resolving Observed Buses...";
            m_network.Model.ResolveToObservedBuses();
            ActionStatus = "Resolving Single Flow Branches...";
            m_network.Model.ResolveToSingleFlowBranches();

            ActionStatus = "Pruning Model...";
            m_network.Model.Prune();
            m_network.Model.InPruningMode = wasInPruningMode;
            ActionStatus = $"Exited Pruning Mode.{m_network.Model.ObservedBusses.Count}";
        }

        private void PruneModelByVoltageLevel()
        {
            List<int> voltageLevelFilter = new List<int>();
            foreach (VoltageLevel voltageLevel in m_retainedVoltageLevels)
            {
                voltageLevelFilter.Add(voltageLevel.InternalID);
            }
            int totalVoltageLevelCount = m_network.Model.VoltageLevels.Count;
            int retainedVoltageLevelCount = voltageLevelFilter.Count;
            m_network.Model.PruneByVoltageLevels(voltageLevelFilter);
            m_networkTreeViewModel.Network = m_network;
            ActionStatus = $"Retained {retainedVoltageLevelCount} of {totalVoltageLevelCount} Voltage Levels.";
        }

        private void PruneModelBySubstation()
        {
            List<int> substationFilter = new List<int>();
            foreach (Substation substation in m_retainedSubstations)
            {
                substationFilter.Add(substation.InternalID);
            }
            int totalSubstationCount = m_network.Model.Substations.Count;
            int retainedSubstationCount = substationFilter.Count;
            m_network.Model.PruneBySubstations(substationFilter);
            m_networkTreeViewModel.Network = m_network;
            ActionStatus = $"Retained {retainedSubstationCount} of {totalSubstationCount} Substations.";
        }

        private void PruneModelByCompany()
        {
            List<int> companyFilter = new List<int>();
            foreach (Company company in m_retainedCompanies)
            {
                companyFilter.Add(company.InternalID);
            }
            int totalCompanyCount = m_network.Model.Companies.Count;
            int retainedCompanyCount = companyFilter.Count;
            m_network.Model.PruneByCompanies(companyFilter);
            m_networkTreeViewModel.Network = m_network;
            ActionStatus = $"Retained {retainedCompanyCount} of {totalCompanyCount} Companies.";
        }

        #endregion

        private void EnableInferredStateAsActualProxy()
        {
            m_network.Model.InPruningMode = false;
            m_network.Model.EnableInferredStateAsActualProxy();
        }

        #region [ Measurement Creation ]

        private void ConnectToOpenEca()
        {
            m_openEcaConnection = new OpenEcaConnection(this);
            m_openEcaConnection.Connect();
        }

        private void CreateStatusFlags()
        {
            if (OpenEcaConnection != null && OpenEcaConnection.IsConnected)
            {
                DataRow[] statusFlagData = OpenEcaConnection.Metadata.Tables["MeasurementDetail"].Select("SignalAcronym = 'FLAG'");
                if (m_network != null && m_network.Model != null)
                {
                    foreach (DataRow statusFlag in statusFlagData)
                    {
                        var id = statusFlag["ID"];
                        var signalID = statusFlag["SignalID"];
                        var pointTag = statusFlag["PointTag"];
                        var signalReference = statusFlag["SignalReference"];
                        var signalAcronym = statusFlag["SignalAcronym"];
                        var description = statusFlag["Description"];
                        var enabled = statusFlag["Enabled"];

                        StatusWord existingStatusWord = m_network.Model.StatusWords.Find(x => x.UniqueId.ToString() == signalID.ToString());

                        if (existingStatusWord != null)
                        {
                            existingStatusWord.Acronym = (string)pointTag;
                            existingStatusWord.Description = (string)description;
                            existingStatusWord.IsEnabled = (bool)enabled;
                            existingStatusWord.Key = signalID.ToString();
                            existingStatusWord.Name = (string)signalReference;
                        }
                        else
                        {
                            StatusWord newStatusWord = new StatusWord()
                            {
                                UniqueId = (Guid)signalID,
                                InternalID = Convert.ToInt32(id.ToString().Split(':')[1]),
                                Number = Convert.ToInt32(id.ToString().Split(':')[1]),
                                Acronym = (string)pointTag,
                                Description = (string)description,
                                IsEnabled = (bool)enabled,
                                Key = signalID.ToString(),
                                Name = (string)signalReference,
                            };
                            m_network.Model.StatusWords.Add(newStatusWord);
                        }
                    }
                    m_networkTreeViewModel.Network = m_network;
                }
            }
            else
            {
                CommunicationStatus = "No connection to openECA";
            }
        }

        private void CreateMeasurements(List<OutputMeasurement> measurements)
        {
            if (OpenEcaConnection != null && OpenEcaConnection.IsConnected)
            {
                SpecialStatus = $"Creating {measurements.Count} measurement(s)";
                foreach (OutputMeasurement measurement in measurements)
                {
                    OpenEcaConnection.CreateMeasurement(measurement);
                }
                RefreshMetaData();
            }
            else
            {
                CommunicationStatus = "No connection to openECA";
            }
        }

        private void CreateVoltageEstimates()
        {
            CreateMeasurements(m_network.Model.StateEstimateOutput);
        }

        private void MapVoltageEstimates()
        {
            List<OutputMeasurement> output = m_network.Model.StateEstimateOutput;

            if (OpenEcaConnection != null && OpenEcaConnection.IsConnected)
            {
                int mappedMeasurementCount = 0;
                foreach (OutputMeasurement measurement in output)
                {
                    DataRow newMeasurement = OpenEcaConnection.Metadata.Tables["MeasurementDetail"].Select($"PointTag = '{measurement.PointTag}'").Single();
                    VoltagePhasorGroup voltage = m_network.Model.Voltages.Find(x => x.InternalID == measurement.InternalId);
                    if (newMeasurement != null)
                    {
                        if (measurement.SignalType == "VPHM")
                        {
                            voltage.PositiveSequence.Estimate.MagnitudeKey = newMeasurement["SignalID"].ToString();
                            mappedMeasurementCount++;
                        }
                        else if (measurement.SignalType == "VPHA")
                        {
                            voltage.PositiveSequence.Estimate.AngleKey = newMeasurement["SignalID"].ToString();
                            mappedMeasurementCount++;
                        }
                    }
                }
                SpecialStatus = $"Mapped {mappedMeasurementCount} of {output.Count} output measurements.";

            }
            else
            {
                CommunicationStatus = "No connection to openECA";
            }
        }

        private void CreateCurrentFlowEstimates()
        {
            CreateMeasurements(m_network.Model.CurrentFlowEstimateOutput);
        }

        private void MapCurrentFlowEstimates()
        {

            if (OpenEcaConnection != null && OpenEcaConnection.IsConnected)
            {
                List<OutputMeasurement> output = m_network.Model.CurrentFlowEstimateOutput;
                int mappedMeasurementCount = 0;
                foreach (OutputMeasurement measurement in output)
                {
                    DataRow newMeasurement = OpenEcaConnection.Metadata.Tables["MeasurementDetail"].Select($"PointTag = '{measurement.PointTag}'").Single();
                    CurrentFlowPhasorGroup current = m_network.Model.CurrentFlows.Find(x => x.InternalID == measurement.InternalId);
                    if (newMeasurement != null)
                    {
                        if (measurement.SignalType == "IPHM")
                        {
                            current.PositiveSequence.Estimate.MagnitudeKey = newMeasurement["SignalID"].ToString();
                            mappedMeasurementCount++;
                        }
                        else if (measurement.SignalType == "IPHA")
                        {
                            current.PositiveSequence.Estimate.AngleKey = newMeasurement["SignalID"].ToString();
                            mappedMeasurementCount++;
                        }
                    }
                }
                SpecialStatus = $"Mapped {mappedMeasurementCount} of {output.Count} output measurements.";

            }
            else
            {
                CommunicationStatus = "No connection to openECA";
            }
        }

        private void CreateCurrentInjectionEstimates()
        {
            CreateMeasurements(m_network.Model.CurrentInjectionEstimateOutput);
        }

        private void MapCurrentInjectionEstimates()
        {
            if (OpenEcaConnection != null && OpenEcaConnection.IsConnected)
            {
                List<OutputMeasurement> output = m_network.Model.CurrentInjectionEstimateOutput;
                int mappedMeasurementCount = 0;
                foreach (OutputMeasurement measurement in output)
                {
                    DataRow newMeasurement = OpenEcaConnection.Metadata.Tables["MeasurementDetail"].Select($"PointTag = '{measurement.PointTag}'").Single();
                    CurrentInjectionPhasorGroup current = m_network.Model.CurrentInjections.Find(x => x.InternalID == measurement.InternalId);
                    if (newMeasurement != null)
                    {
                        if (measurement.SignalType == "IPHM")
                        {
                            current.PositiveSequence.Estimate.MagnitudeKey = newMeasurement["SignalID"].ToString();
                            mappedMeasurementCount++;
                        }
                        else if (measurement.SignalType == "IPHA")
                        {
                            current.PositiveSequence.Estimate.AngleKey = newMeasurement["SignalID"].ToString();
                            mappedMeasurementCount++;
                        }
                    }
                }
                SpecialStatus = $"Mapped {mappedMeasurementCount} of {output.Count} output measurements.";

            }
            else
            {
                CommunicationStatus = "No connection to openECA";
            }
        }

        private void CreateVoltageResiduals()
        {
            CreateMeasurements(m_network.Model.VoltageResidualOutput);
        }

        private void MapVoltageResiduals()
        {
            if (OpenEcaConnection != null && OpenEcaConnection.IsConnected)
            {
                List<OutputMeasurement> output = m_network.Model.VoltageResidualOutput;
                int mappedMeasurementCount = 0;
                foreach (OutputMeasurement measurement in output)
                {
                    DataRow newMeasurement = OpenEcaConnection.Metadata.Tables["MeasurementDetail"].Select($"PointTag = '{measurement.PointTag}'").Single();
                    VoltagePhasorGroup voltage = m_network.Model.Voltages.Find(x => x.InternalID == measurement.InternalId);
                    if (newMeasurement != null)
                    {
                        string key = newMeasurement["SignalID"].ToString();
                        if (measurement.OutputType == OutputType.VoltageMagnitudeResidual)
                        {
                            voltage.PositiveSequence.MagnitudeResidualKey = key;
                            mappedMeasurementCount++;
                        }
                        else if (measurement.OutputType == OutputType.VoltageAngleResidual)
                        {
                            voltage.PositiveSequence.AngleResidualKey = key;
                            mappedMeasurementCount++;
                        }
                    }
                }
                SpecialStatus = $"Mapped {mappedMeasurementCount} of {output.Count} output measurements.";

            }
            else
            {
                CommunicationStatus = "No connection to openECA";
            }
        }

        private void CreateCurrentFlowResiduals()
        {
            CreateMeasurements(m_network.Model.CurrentResidualOutput);
        }

        private void MapCurrentResiduals()
        {
            if (OpenEcaConnection != null && OpenEcaConnection.IsConnected)
            {
                List<OutputMeasurement> output = m_network.Model.CurrentResidualOutput;
                int mappedMeasurementCount = 0;
                foreach (OutputMeasurement measurement in output)
                {
                    DataRow newMeasurement = OpenEcaConnection.Metadata.Tables["MeasurementDetail"].Select($"PointTag = '{measurement.PointTag}'").Single();
                    CurrentFlowPhasorGroup current = m_network.Model.CurrentFlows.Find(x => x.InternalID == measurement.InternalId);
                    if (newMeasurement != null)
                    {
                        string key = newMeasurement["SignalID"].ToString();
                        if (measurement.OutputType == OutputType.CurrentFlowMagnitudeResidual)
                        {
                            current.PositiveSequence.MagnitudeResidualKey = key;
                            mappedMeasurementCount++;
                        }
                        else if (measurement.OutputType == OutputType.CurrentFlowAngleResidual)
                        {
                            current.PositiveSequence.AngleResidualKey = key;
                            mappedMeasurementCount++;
                        }
                    }
                }
                SpecialStatus = $"Mapped {mappedMeasurementCount} of {output.Count} output measurements.";

            }
            else
            {
                CommunicationStatus = "No connection to openECA";
            }
        }

        //private void CreateCurrentInjectionResiduals()
        //{

        //}

        //private void MapCurrentInjectionResiduals()
        //{

        //}

        private void CreateCircuitBreakerStatuses()
        {
            CreateMeasurements(m_network.Model.CircuitBreakerStatusOutput);
        }

        private void MapCircuitBreakerStatuses()
        {
            if (OpenEcaConnection != null && OpenEcaConnection.IsConnected)
            {
                List<OutputMeasurement> output = m_network.Model.CircuitBreakerStatusOutput;
                int mappedMeasurementCount = 0;
                foreach (OutputMeasurement measurement in output)
                {
                    DataRow newMeasurement = OpenEcaConnection.Metadata.Tables["MeasurementDetail"].Select($"PointTag = '{measurement.PointTag}'").Single();
                    CircuitBreaker breaker = m_network.Model.CircuitBreakers.Find(x => x.InternalID == measurement.InternalId);
                    if (newMeasurement != null)
                    {
                        string key = newMeasurement["SignalID"].ToString();
                        if (measurement.OutputType == OutputType.CircuitBreakerStatus)
                        {
                            breaker.MeasurementKey = key;
                            mappedMeasurementCount++;
                        }
                        else if (measurement.OutputType == OutputType.CircuitBreakerStatus)
                        {
                            breaker.MeasurementKey = key;
                            mappedMeasurementCount++;
                        }
                    }
                }
                SpecialStatus = $"Mapped {mappedMeasurementCount} of {output.Count} output measurements.";

            }
            else
            {
                CommunicationStatus = "No connection to openECA";
            }
        }

        private void CreateSwitchStatuses()
        {
            CreateMeasurements(m_network.Model.SwitchStatusOutput);
        }

        private void MapSwitchStatuses()
        {
            if (OpenEcaConnection != null && OpenEcaConnection.IsConnected)
            {
                List<OutputMeasurement> output = m_network.Model.SwitchStatusOutput;
                int mappedMeasurementCount = 0;
                foreach (OutputMeasurement measurement in output)
                {
                    DataRow newMeasurement = OpenEcaConnection.Metadata.Tables["MeasurementDetail"].Select($"PointTag = '{measurement.PointTag}'").Single();
                    SynchrophasorAnalytics.Modeling.Switch switchingDevice = m_network.Model.Switches.Find(x => x.InternalID == measurement.InternalId);
                    if (newMeasurement != null)
                    {
                        string key = newMeasurement["SignalID"].ToString();
                        if (measurement.OutputType == OutputType.SwitchStatus)
                        {
                            switchingDevice.MeasurementKey = key;
                            mappedMeasurementCount++;
                        }
                        else if (measurement.OutputType == OutputType.SwitchStatus)
                        {
                            switchingDevice.MeasurementKey = key;
                            mappedMeasurementCount++;
                        }
                    }
                }
                SpecialStatus = $"Mapped {mappedMeasurementCount} of {output.Count} output measurements.";

            }
            else
            {
                CommunicationStatus = "No connection to openECA";
            }
        }

        private void CreateTopologyProfiling()
        {
            CreateMeasurements(m_network.Model.TopologyProfilingOutput);
        }

        private void MapTopologyProfiling()
        {
            if (OpenEcaConnection != null && OpenEcaConnection.IsConnected)
            {
                List<OutputMeasurement> output = m_network.Model.TopologyProfilingOutput;
                int mappedMeasurementCount = 0;
                foreach (OutputMeasurement measurement in output)
                {
                    DataRow newMeasurement = OpenEcaConnection.Metadata.Tables["MeasurementDetail"].Select($"PointTag = '{measurement.PointTag}'").Single();

                    if (newMeasurement != null)
                    {
                        string key = newMeasurement["SignalID"].ToString();
                        if (measurement.MeasuredDeviceType == MeasuredDeviceType.Substation)
                        {
                            Substation substation = m_network.Model.Substations.Find(x => x.InternalID == measurement.InternalId);
                            substation.ObservedBusCountKey = key;
                            mappedMeasurementCount++;
                        }
                        else if (measurement.MeasuredDeviceType == MeasuredDeviceType.Node)
                        {
                            Node node = m_network.Model.Nodes.Find(x => x.InternalID == measurement.InternalId);
                            if (measurement.PointTag.Contains("TOPOLOGY_STATE"))
                            {
                                node.ObservationStateKey = key;
                                mappedMeasurementCount++;
                            }
                            else if (measurement.PointTag.Contains("TOPOLOGY_ID"))
                            {
                                node.ObservedBusIdKey = key;
                                mappedMeasurementCount++;
                            }
                        }
                    }
                }
                SpecialStatus = $"Mapped {mappedMeasurementCount} of {output.Count} output measurements.";
            }
            else
            {
                CommunicationStatus = "No connection to openECA";
            }
        }

        private void CreateMeasurementValidationFlags()
        {
            CreateMeasurements(m_network.Model.MeasurementValidationFlagOutput);
        }

        private void MapMeasurementValidationFlags()
        {
            if (OpenEcaConnection != null && OpenEcaConnection.IsConnected)
            {
                List<OutputMeasurement> output = m_network.Model.MeasurementValidationFlagOutput;
                int mappedMeasurementCount = 0;
                foreach (OutputMeasurement measurement in output)
                {
                    DataRow newMeasurement = OpenEcaConnection.Metadata.Tables["MeasurementDetail"].Select($"PointTag = '{measurement.PointTag}'").Single();
                    if (newMeasurement != null)
                    {
                        PhasorGroup phasorGroup = null;
                        if (measurement.OutputType == OutputType.VoltageMeasurementValidationFlag)
                        {
                            phasorGroup = m_network.Model.Voltages.Find(x => x.InternalID == measurement.InternalId);
                        }
                        else if (measurement.OutputType == OutputType.CurrentFlowMeasurementValidationFlag)
                        {
                            phasorGroup = m_network.Model.CurrentFlows.Find(x => x.InternalID == measurement.InternalId);
                        }
                        else if (measurement.OutputType == OutputType.CurrentInjectionMeasurementValidationFlag)
                        {
                            phasorGroup = m_network.Model.CurrentInjections.Find(x => x.InternalID == measurement.InternalId);
                        }
                        if (phasorGroup != null)
                        {
                            phasorGroup.MeasurementIsIncludedKey = newMeasurement["SignalID"].ToString();
                            mappedMeasurementCount++;
                        }
                    }
                }
                SpecialStatus = $"Mapped {mappedMeasurementCount} of {output.Count} output measurements.";
            }
            else
            {
                CommunicationStatus = "No connection to openECA";
            }
        }

        private void CreatePerformanceMetrics()
        {
            CreateMeasurements(m_network.Model.PerformanceMetricOutput);
        }

        private void MapPerformanceMetrics()
        {
            
            if (OpenEcaConnection != null && OpenEcaConnection.IsConnected)
            {
                List<OutputMeasurement> output = m_network.Model.PerformanceMetricOutput;
                int mappedMeasurementCount = 0;
                foreach (OutputMeasurement measurement in output)
                {
                    DataRow newMeasurement = OpenEcaConnection.Metadata.Tables["MeasurementDetail"].Select($"PointTag = '{measurement.PointTag}'").Single();
                    if (newMeasurement != null)
                    {
                        PerformanceMetrics metrics = m_network.PerformanceMetrics;
                        string key = newMeasurement["SignalID"].ToString();
                        switch (measurement.InternalId)
                        {
                            case 1:
                                metrics.ActiveVoltageCountKey = key;
                                mappedMeasurementCount++;
                                break;
                            case 2:
                                metrics.ActiveCurrentFlowCountKey = key;
                                mappedMeasurementCount++;
                                break;
                            case 3:
                                metrics.ActiveCurrentInjectionCountKey = key;
                                mappedMeasurementCount++;
                                break;
                            case 4:
                                metrics.ObservedBusCountKey = key;
                                mappedMeasurementCount++;
                                break;
                            case 5:
                                metrics.RefreshExecutionTimeKey = key;
                                mappedMeasurementCount++;
                                break;
                            case 6:
                                metrics.ParsingExecutionTimeKey = key;
                                mappedMeasurementCount++;
                                break;
                            case 7:
                                metrics.MeasurementMappingExecutionTimeKey = key;
                                mappedMeasurementCount++;
                                break;
                            case 8:
                                metrics.ActiveCurrentPhasorDeterminationExecutionTimeKey = key;
                                mappedMeasurementCount++;
                                break;
                            case 9:
                                metrics.ObservabilityAnalysisExecutionTimeKey = key;
                                mappedMeasurementCount++;
                                break;
                            case 10:
                                metrics.StateComputationExecutionTimeKey = key;
                                mappedMeasurementCount++;
                                break;
                            case 11:
                                metrics.OutputPreparationExecutionTimeKey = key;
                                mappedMeasurementCount++;
                                break;
                            case 12:
                                metrics.TotalExecutionTimeInTicksKey = key;
                                mappedMeasurementCount++;
                                break;
                            case 13:
                                metrics.TotalExecutionTimeInMillisecondsKey = key;
                                mappedMeasurementCount++;
                                break;
                        }
                    }
                }
                SpecialStatus = $"Mapped {mappedMeasurementCount} of {output.Count} output measurements.";
            }
            else
            {
                CommunicationStatus = "No connection to openECA";
            }
        }

        private void RefreshMetaData()
        {
            if (OpenEcaConnection != null && OpenEcaConnection.IsConnected)
            {
                CommunicationStatus = "Refreshing Meta Data...";
                OpenEcaConnection.RefreshMetaData();
            }
            else
            {
                CommunicationStatus = "No connection to openECA";
            }
        }

        #endregion

        #region [ Setup ]

        private void SelectMeasurementSample()
        {
            SelectedMeasurementSample = m_networkTreeViewModel.SelectedElement.Value.Element as RawMeasurements;
            EnableControls();
            ActionStatus = "User selected a measurement sample.";
        }

        private void InitializeModel()
        {
            if (m_network != null && m_network.Model != null)
            {
                m_network.Initialize();
                m_networkIsInitialized = true;
                EnableInferredStateAsActualProxy();
                EnableControls();
                ActionStatus = "User initialized the network model";
            }
        }
        
        private void ClearMeasurementsFromModel()
        {
            if (m_network != null && m_network.Model != null)
            {
                m_network.Model.ClearValues();
                m_network.Model.InputKeyValuePairs = new Dictionary<string, double>();
                m_networkIsInitialized = false;
                m_measurementsAreMapped = false;
                m_activeCurrentFlowsHaveBeenDetermined = false;
                m_observedBussesHaveBeenResolved = false;
                m_stateWasComputed = false;
                DisableControls();
                ActionStatus = "User cleared measurement values from network model.";
            }
        }

        private void MapMeasurementsToModel()
        {
            try
            {
                foreach (RawMeasurementsMeasurement measurement in SelectedMeasurementSample.Items)
                {
                    m_network.Model.InputKeyValuePairs.Add(measurement.Key, Convert.ToDouble(measurement.Value));
                    CommunicationStatus = $"Mapping measurement {measurement.Key} with value {measurement.Value}";
                }
                CommunicationStatus = "";
                m_network.Model.OnNewMeasurements();

                m_measurementsAreMapped = true;
                EnableControls();
                ActionStatus = "Measurements succcessfully mapped to network model.";
            }
            catch (Exception exception)
            {
                System.Windows.MessageBox.Show(exception.ToString(), "Error!");
            }
        }

        #endregion

        #region [ Observability Analysis ] 

        private void DetermineActiveCurrentFlows()
        {
            try
            {
                m_network.Model.DetermineActiveCurrentFlows();
                m_activeCurrentFlowsHaveBeenDetermined = true;
                ActionStatus = "Active current flow phasors determined successfully.";
                EnableControls();
            }
            catch (Exception exception)
            {
                System.Windows.MessageBox.Show(exception.ToString(), "Error!");
            }
        }

        private void DetermineActiveCurrentInjections()
        {
            try
            {
                m_network.Model.DetermineActiveCurrentInjections();
                m_activeCurrentInjectionsHaveBeenDetermined = true;
                ActionStatus = "Active current injection phasors determined successfully.";
                EnableControls();
            }
            catch (Exception exception)
            {
                System.Windows.MessageBox.Show(exception.ToString(), "Error!");
            }
        }

        private void ResolveToObservedBuses()
        {
            try
            {
                m_network.Model.ResolveToObservedBuses();
                m_observedBussesHaveBeenResolved = true;
                ActionStatus = "Observability analysis resolved nodes to observed nodes.";
                EnableControls();
            }
            catch (Exception exception)
            {
                System.Windows.MessageBox.Show(exception.ToString(), "Error!");
            }
        }

        private void ResolveToSingleFlowBranches()
        {
            try
            {
                m_network.Model.ResolveToSingleFlowBranches();
                m_singleFlowBranchesHaveBeenResolved = true;
                ActionStatus = "Observability analysis resolved transmission lines to single flow branches.";
                EnableControls();
            }
            catch (Exception exception)
            {
                System.Windows.MessageBox.Show(exception.ToString(), "Error!");
            }
        }

        #endregion

        #region [ Matrices ]

        private void ViewAMatrix()
        {
            try
            {
                CurrentFlowMeasurementBusIncidenceMatrix A = new CurrentFlowMeasurementBusIncidenceMatrix(m_network);

                StringBuilder stringBuilder = new StringBuilder();

                stringBuilder.AppendFormat(A.ToCsvString());

                File.WriteAllText(Directory.GetCurrentDirectory() + "/A Matrix.csv", stringBuilder.ToString());
                ActionStatus = "Wrote to " + Directory.GetCurrentDirectory() + "/A Matrix.csv";
                Process.Start(Directory.GetCurrentDirectory() + "/A Matrix.csv");
            }
            catch (Exception exception)
            {
                System.Windows.MessageBox.Show(exception.ToString(), "Error!");
            }
        }

        private void ViewIIMatrix()
        {
            try
            {
                VoltageMeasurementBusIncidenceMatrix II = new VoltageMeasurementBusIncidenceMatrix(m_network);

                StringBuilder stringBuilder = new StringBuilder();

                stringBuilder.AppendFormat(II.ToCsvString());

                File.WriteAllText(Directory.GetCurrentDirectory() + "/II Matrix.csv", stringBuilder.ToString());
                ActionStatus = "Wrote to " + Directory.GetCurrentDirectory() + "/II Matrix.csv";
                Process.Start(Directory.GetCurrentDirectory() + "/II Matrix.csv");
            }
            catch (Exception exception)
            {
                System.Windows.MessageBox.Show(exception.ToString(), "Error!");
            }
        }

        private void ViewYMatrix()
        {
            try
            {
                SeriesAdmittanceMatrix Y = new SeriesAdmittanceMatrix(m_network);

                StringBuilder stringBuilder = new StringBuilder();

                stringBuilder.AppendFormat(Y.ToCsvString());

                File.WriteAllText(Directory.GetCurrentDirectory() + "/Y Matrix.csv", stringBuilder.ToString());
                ActionStatus = "Wrote to " + Directory.GetCurrentDirectory() + "/Y Matrix.csv";
                Process.Start(Directory.GetCurrentDirectory() + "/Y Matrix.csv");
            }
            catch (Exception exception)
            {
                System.Windows.MessageBox.Show(exception.ToString(), "Error!");
            }
        }

        private void ViewYsMatrix()
        {
            try
            {
                LineShuntSusceptanceMatrix Ys = new LineShuntSusceptanceMatrix(m_network);

                StringBuilder stringBuilder = new StringBuilder();

                stringBuilder.AppendFormat(Ys.ToCsvString());

                File.WriteAllText(Directory.GetCurrentDirectory() + "/Ys Matrix.csv", stringBuilder.ToString());
                ActionStatus = "Wrote to " + Directory.GetCurrentDirectory() + "/Ys Matrix.csv";
                Process.Start(Directory.GetCurrentDirectory() + "/Ys Matrix.csv");
            }
            catch (Exception exception)
            {
                System.Windows.MessageBox.Show(exception.ToString(), "Error!");
            }
        }

        private void ViewYshMatrix()
        {
            try
            {
                ShuntDeviceSusceptanceMatrix Ysh = new ShuntDeviceSusceptanceMatrix(m_network);

                StringBuilder stringBuilder = new StringBuilder();

                stringBuilder.AppendFormat(Ysh.ToCsvString());

                File.WriteAllText(Directory.GetCurrentDirectory() + "/Ysh Matrix.csv", stringBuilder.ToString());
                ActionStatus = "Wrote to " + Directory.GetCurrentDirectory() + "/Ysh Matrix.csv";
                Process.Start(Directory.GetCurrentDirectory() + "/Ysh Matrix.csv");
            }
            catch (Exception exception)
            {
                System.Windows.MessageBox.Show(exception.ToString(), "Error!");
            }
        }
        
        #endregion

        #region [ Computation ]

        private void ComputeSystemState()
        {
            try
            {
                m_network.ComputeSystemState();
                m_stateWasComputed = true;
                ActionStatus = "System state computed successfully.";
                EnableControls();
            }
            catch (Exception exception)
            {
                System.Windows.MessageBox.Show(exception.ToString(), "Error!");
            }
        }

        private void ComputeLineFlows()
        {
            try
            {
                m_network.Model.ComputeEstimatedCurrentFlows();
                ActionStatus = "Estimated line flows computed successfully.";
                EnableControls();
            }
            catch (Exception exception)
            {
                System.Windows.MessageBox.Show(exception.ToString(), "Error!");
            }
        }

        private void ComputeInjections()
        {
            try
            {
                m_network.Model.ReturnsCurrentInjection = true;
                m_network.Model.ComputeEstimatedCurrentInjections();
                ActionStatus = "Estimated injections computed successfully.";
                EnableControls();
            }
            catch (Exception exception)
            {
                System.Windows.MessageBox.Show(exception.ToString(), "Error!");
            }
        }

        private void ComputePowerFlows()
        {
            System.Windows.MessageBox.Show("This button has not yet been implemented", "Oh, snap!");
        }

        private void ComputeSequenceComponents()
        {
            try
            {
                m_network.Model.ComputeSequenceValues();
                ActionStatus = "Sequence values computed successfully.";
                EnableControls();
            }
            catch (Exception exception)
            {
                System.Windows.MessageBox.Show(exception.ToString(), "Error!");
            }
        }

        #endregion

        #region [ Inpsect ] 

        private void ViewComponents()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(m_network.Model.ComponentList());
            File.WriteAllText(Directory.GetCurrentDirectory() + "/ComponentList.txt", stringBuilder.ToString());
            ActionStatus = "Wrote to " + Directory.GetCurrentDirectory() + "/ComponentList.txt";
            Process.Start(Directory.GetCurrentDirectory() + "/ComponentList.txt", stringBuilder.ToString());
        }

        #region [ View Status Words ]

        private void ViewStatusWords()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat(m_network.Model.StatusWords.Count.ToString() + " Modeled Status Words");
            foreach (StatusWord statusWord in m_network.Model.StatusWords)
            {
                stringBuilder.AppendLine(statusWord.ToVerboseString());
            }
            File.WriteAllText(Directory.GetCurrentDirectory() + "/ModeledStatusWords.txt", stringBuilder.ToString());
            ActionStatus = "Wrote to " + Directory.GetCurrentDirectory() + "/ModeledStatusWords.txt";
            Process.Start(Directory.GetCurrentDirectory() + "/ModeledStatusWords.txt");
        }

        private void ViewMappedStatusWords()
        {
            StringBuilder stringBuilder = new StringBuilder();
            List<StatusWord> mappedStatusWords = new List<StatusWord>();
            foreach (VoltagePhasorGroup voltage in m_network.Model.Voltages)
            {
                if (voltage.Status != null && m_network.Model.StatusWords.Contains(voltage.Status))
                {
                    if (!mappedStatusWords.Contains(voltage.Status))
                    {
                        mappedStatusWords.Add(voltage.Status);
                    }
                }
            }
            foreach (CurrentFlowPhasorGroup current in m_network.Model.CurrentFlows)
            {
                if (current.Status != null && m_network.Model.StatusWords.Contains(current.Status))
                {
                    if (!mappedStatusWords.Contains(current.Status))
                    {
                        mappedStatusWords.Add(current.Status);
                    }
                }
            }
            foreach (CurrentInjectionPhasorGroup current in m_network.Model.CurrentInjections)
            {
                if (current.Status != null && m_network.Model.StatusWords.Contains(current.Status))
                {
                    if (!mappedStatusWords.Contains(current.Status))
                    {
                        mappedStatusWords.Add(current.Status);
                    }
                }
            }
            stringBuilder.AppendFormat(mappedStatusWords.Count.ToString() + " Mapped Status Words");
            foreach (StatusWord statusWord in mappedStatusWords)
            {
                stringBuilder.AppendLine(statusWord.ToVerboseString());
            }
            File.WriteAllText(Directory.GetCurrentDirectory() + "/MappedStatusWords.txt", stringBuilder.ToString());
            ActionStatus = "Wrote to " + Directory.GetCurrentDirectory() + "/MappedStatusWords.txt";
            Process.Start(Directory.GetCurrentDirectory() + "/MappedStatusWords.txt");
        }

        private void ViewMappedLSEInvalidStatusWords()
        {
            StringBuilder stringBuilder = new StringBuilder();
            List<StatusWord> mappedStatusWords = new List<StatusWord>();
            foreach (VoltagePhasorGroup voltage in m_network.Model.Voltages)
            {
                if (voltage.Status != null && m_network.Model.StatusWords.Contains(voltage.Status))
                {
                    StatusWord status = voltage.Status;
                    if (status.DataIsValid || status.SynchronizationIsValid )
                    {
                        if (!mappedStatusWords.Contains(voltage.Status))
                        {
                            mappedStatusWords.Add(voltage.Status);
                        }
                    }
                }
            }
            foreach (CurrentFlowPhasorGroup current in m_network.Model.CurrentFlows)
            {
                if (current.Status != null && m_network.Model.StatusWords.Contains(current.Status))
                {
                    StatusWord status = current.Status;
                    if (status.DataIsValid || status.SynchronizationIsValid)
                    {
                        if (!mappedStatusWords.Contains(current.Status))
                        {
                            mappedStatusWords.Add(current.Status);
                        }
                    }
                }
            }
            foreach (CurrentInjectionPhasorGroup current in m_network.Model.CurrentInjections)
            {
                if (current.Status != null && m_network.Model.StatusWords.Contains(current.Status))
                {
                    StatusWord status = current.Status;
                    if (status.DataIsValid || status.SynchronizationIsValid)
                    {
                        if (!mappedStatusWords.Contains(current.Status))
                        {
                            mappedStatusWords.Add(current.Status);
                        }
                    }
                }
            }
            stringBuilder.AppendFormat(mappedStatusWords.Count.ToString() + " Mapped LSE Invalid Status Words");
            foreach (StatusWord statusWord in mappedStatusWords)
            {
                stringBuilder.AppendLine(statusWord.ToVerboseString());
            }
            File.WriteAllText(Directory.GetCurrentDirectory() + "/MappedLSEInvalidStatusWords.txt", stringBuilder.ToString());
            ActionStatus = "Wrote to " + Directory.GetCurrentDirectory() + "/MappedLSEInvalidStatusWords.txt";
            Process.Start(Directory.GetCurrentDirectory() + "/MappedLSEInvalidStatusWords.txt");
        }

        private void ViewMappedMissingStatusWords()
        {
            StringBuilder stringBuilder = new StringBuilder();
            List<StatusWord> mappedStatusWords = new List<StatusWord>();
            foreach (VoltagePhasorGroup voltage in m_network.Model.Voltages)
            {
                if (voltage.Status != null && m_network.Model.StatusWords.Contains(voltage.Status))
                {
                    if (!voltage.Status.StatusWordWasReported)
                    {
                        if (!mappedStatusWords.Contains(voltage.Status))
                        {
                            mappedStatusWords.Add(voltage.Status);
                        }
                    }
                }
            }

            foreach (CurrentFlowPhasorGroup current in m_network.Model.CurrentFlows)
            {
                if (current.Status != null && m_network.Model.StatusWords.Contains(current.Status))
                {
                    if (!current.Status.StatusWordWasReported)
                    {
                        if (!mappedStatusWords.Contains(current.Status))
                        {
                            mappedStatusWords.Add(current.Status);
                        }
                    }
                }
            }

            foreach (CurrentInjectionPhasorGroup current in m_network.Model.CurrentInjections)
            {
                if (current.Status != null && m_network.Model.StatusWords.Contains(current.Status))
                {
                    if (!current.Status.StatusWordWasReported)
                    {
                        if (!mappedStatusWords.Contains(current.Status))
                        {
                            mappedStatusWords.Add(current.Status);
                        }
                    }
                }
            }
            stringBuilder.AppendFormat(mappedStatusWords.Count.ToString() + " Mapped Missing Status Words");
            foreach (StatusWord statusWord in mappedStatusWords)
            {
                stringBuilder.AppendLine(statusWord.ToVerboseString());
            }
            File.WriteAllText(Directory.GetCurrentDirectory() + "/MappedMissingStatusWords.txt", stringBuilder.ToString());
            ActionStatus = "Wrote to " + Directory.GetCurrentDirectory() + "/MappedMissingStatusWords.txt";
            Process.Start(Directory.GetCurrentDirectory() + "/MappedMissingStatusWords.txt");
        }

        private void ViewActiveStatusWords()
        {
            StringBuilder stringBuilder = new StringBuilder();
            List<StatusWord> activeStatusWords = new List<StatusWord>();
            foreach (VoltagePhasorGroup voltage in m_network.Model.Voltages)
            {
                if (voltage.Status != null && m_network.Model.StatusWords.Contains(voltage.Status))
                {
                    StatusWord status = voltage.Status;
                    if (status.StatusWordWasReported)
                    {
                        if (!(status.DataIsValid || status.SynchronizationIsValid))
                        {
                            if (!activeStatusWords.Contains(voltage.Status))
                            {
                                activeStatusWords.Add(voltage.Status);
                            }
                        }
                    }
                }
            }
            foreach (CurrentFlowPhasorGroup current in m_network.Model.CurrentFlows)
            {
                if (current.Status != null && m_network.Model.StatusWords.Contains(current.Status))
                {
                    StatusWord status = current.Status;
                    if (status.StatusWordWasReported)
                    {
                        if (!(status.DataIsValid || status.SynchronizationIsValid))
                        {
                            if (!activeStatusWords.Contains(current.Status))
                            {
                                activeStatusWords.Add(current.Status);
                            }
                        }
                    }
                }
            }
            foreach (CurrentInjectionPhasorGroup current in m_network.Model.CurrentInjections)
            {
                if (current.Status != null && m_network.Model.StatusWords.Contains(current.Status))
                {
                    StatusWord status = current.Status;
                    if (status.StatusWordWasReported)
                    {
                        if (!(status.DataIsValid || status.SynchronizationIsValid))
                        {
                            if (!activeStatusWords.Contains(current.Status))
                            {
                                activeStatusWords.Add(current.Status);
                            }
                        }
                    }
                }
            }
            stringBuilder.AppendFormat(activeStatusWords.Count.ToString() + " Active Status Words");
            foreach (StatusWord statusWord in activeStatusWords)
            {
                stringBuilder.AppendLine(statusWord.ToVerboseString());
            }
            File.WriteAllText(Directory.GetCurrentDirectory() + "/ActiveStatusWords.txt", stringBuilder.ToString());
            ActionStatus = "Wrote to " + Directory.GetCurrentDirectory() + "/ActiveStatusWords.txt";
            Process.Start(Directory.GetCurrentDirectory() + "/ActiveStatusWords.txt");
        }

        #endregion

        #region [ View Measurements ] 

        private void ViewReceivedMeasurements()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Received Measurements Key Value Pairs");
            Dictionary<string, double> receivedMeasurements = m_network.Model.GetReceivedMeasurements();
            foreach (KeyValuePair<string, double> keyValuePair in receivedMeasurements)
            {
                stringBuilder.AppendLine(keyValuePair.Key + ", " + keyValuePair.Value.ToString());
            }
            File.WriteAllText(Directory.GetCurrentDirectory() + "/ReceivedMeasurements.txt", stringBuilder.ToString());
            ActionStatus = "Wrote to " + Directory.GetCurrentDirectory() + "/ReceivedMeasurements.txt";
            Process.Start(Directory.GetCurrentDirectory() + "/ReceivedMeasurements.txt");
        }

        private void ViewUnreceivedMeasurements()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Unreceived Measurements Key Value Pairs");
            Dictionary<string, double> receivedMeasurements = m_network.Model.GetReceivedMeasurements();
            foreach (KeyValuePair<string, double> keyValuePair in m_network.Model.InputKeyValuePairs)
            {
                double value = 0;
                if (!receivedMeasurements.TryGetValue(keyValuePair.Key, out value))
                {
                    stringBuilder.AppendLine(keyValuePair.Key + ", " + keyValuePair.Value.ToString());
                }
            }
            File.WriteAllText(Directory.GetCurrentDirectory() + "/UnreceivedMeasurements.txt", stringBuilder.ToString());
            ActionStatus = "Wrote to " + Directory.GetCurrentDirectory() + "/UnreceivedMeasurements.txt";
            Process.Start(Directory.GetCurrentDirectory() + "/UnreceivedMeasurements.txt");
        }

        private void ViewOutputMeasurements()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat(m_network.Model.OutputKeyValuePairs.Count.ToString() + " Output Measurements");

            foreach (KeyValuePair<string, double> outputMeasurement in m_network.Model.OutputKeyValuePairs)
            {
                stringBuilder.AppendFormat(outputMeasurement.Key + "\t" + outputMeasurement.Value.ToString() + "{0}", Environment.NewLine);
            }

            File.WriteAllText(Directory.GetCurrentDirectory() + "/OutputMeasurements.txt", stringBuilder.ToString());
            ActionStatus = "Wrote to " + Directory.GetCurrentDirectory() + "/OutputMeasurements.txt";
            Process.Start(Directory.GetCurrentDirectory() + "/OutputMeasurements.txt");
        }

        private void ShowOutputMeasurementReport(string fileName, List<OutputMeasurement> output)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (OutputMeasurement measurement in output)
            {
                stringBuilder.Append($"{measurement.ToVerboseString()}{Environment.NewLine}");
            }
            ShowTextReport(fileName, stringBuilder.ToString());
        }

        private void ViewPerformanceMetrics()
        {
            ShowOutputMeasurementReport("PerformanceMetricOutput.txt", m_network.Model.PerformanceMetricOutput);
        }

        private void ViewVoltageEstimates()
        {
            ShowOutputMeasurementReport("VoltageEstimateOutput.txt", m_network.Model.StateEstimateOutput);
        }

        private void ViewCurrentFlowEstimates()
        {
            ShowOutputMeasurementReport("CurrentFlowEstimateOutput.txt", m_network.Model.CurrentFlowEstimateOutput);
        }

        private void ViewCurrentInjectionEstimates()
        {
            ShowOutputMeasurementReport("CurrentInjectionEstimateOutput.txt", m_network.Model.CurrentInjectionEstimateOutput);
        }

        private void ViewVoltageResiduals()
        {
            ShowOutputMeasurementReport("VoltageResidualOutput.txt", m_network.Model.VoltageResidualOutput);
        }

        private void ViewCurrentFlowResiduals()
        {
            ShowOutputMeasurementReport("CurrentFlowResidualOutput.txt", m_network.Model.CurrentResidualOutput);
        }

        private void ViewCurrentInjectionResiduals()
        {
            //ShowOutputMeasurementReport("CurrentInjectionResidualOutput.txt", m_network.Model.CurrentInjectionResidualOutput);
        }

        private void ViewCircuitBreakerStatuses()
        {
            ShowOutputMeasurementReport("CircuitBreakerStatusOutput.txt", m_network.Model.CircuitBreakerStatusOutput);
        }

        private void ViewSwitchStatuses()
        {
            ShowOutputMeasurementReport("SwitchStatusOutput.txt", m_network.Model.SwitchStatusOutput);
        }

        private void ViewTopologyProfiling()
        {
            ShowOutputMeasurementReport("TopologyProfilingOutput.txt", m_network.Model.TopologyProfilingOutput);
        }

        private void ViewMeasurementValidationFlags()
        {
            ShowOutputMeasurementReport("MeasurementValidationFlagOutput.txt", m_network.Model.MeasurementValidationFlagOutput);
        }


        #endregion

        #region [ View Observability Analysis Data ] 

        private void ViewObservableNodes()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat(m_network.Model.ObservedBusses.Count.ToString() + " Observed Busses");
            foreach (ObservedBus observedBus in m_network.Model.ObservedBusses)
            {
                stringBuilder.AppendFormat(observedBus.ToVerboseString() + "{0}", Environment.NewLine);
            }

            ShowTextReport("ObservedBusses.txt", stringBuilder.ToString());
        }
        
        private void ViewSubstationAdjacencyList()
        {
            StringBuilder stringBuilder = new StringBuilder();

            foreach (Company company in m_network.Model.Companies)
            {
                foreach (Division division in company.Divisions)
                {
                    foreach (Substation substation in division.Substations)
                    {
                        stringBuilder.AppendFormat(substation.ToString() + "{0}", Environment.NewLine);
                        stringBuilder.AppendFormat(substation.Graph.AdjacencyList.ToString() + "{0}", Environment.NewLine);
                        stringBuilder.AppendLine();
                    }
                }
            }

            File.WriteAllText(Directory.GetCurrentDirectory() + "/SubstationAdjacencyLists.txt", stringBuilder.ToString());
            ActionStatus = "Wrote to " + Directory.GetCurrentDirectory() + "/SubstationAdjacencyLists.txt";
            Process.Start(Directory.GetCurrentDirectory() + "/SubstationAdjacencyLists.txt");
        }
        
        private void ViewTransmissionLineAdjacencyLists()
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (TransmissionLine transmissionLine in m_network.Model.TransmissionLines)
            {
                stringBuilder.AppendFormat(transmissionLine.ToVerboseString() + "{0}", Environment.NewLine);
                stringBuilder.AppendFormat("Directly Connected Adjacency List{0}", Environment.NewLine);
                stringBuilder.AppendFormat(transmissionLine.Graph.DirectlyConnectedAdjacencyList.ToString() + "{0}", Environment.NewLine);
                stringBuilder.AppendFormat("Series Impedance Connected Adjacency List{0}", Environment.NewLine);
                stringBuilder.AppendFormat(transmissionLine.Graph.SeriesImpedanceConnectedAdjacencyList.ToString() + "{0}", Environment.NewLine);
                stringBuilder.AppendFormat("Transmission Line Tree{0}", Environment.NewLine);
                stringBuilder.AppendFormat(transmissionLine.Graph.RootNode.ToSubtreeString() + "{0}", Environment.NewLine);
                stringBuilder.AppendFormat("Single Flow Branch Resolution{0}", Environment.NewLine);
                stringBuilder.AppendFormat("Number of Possible Impedance States: " + transmissionLine.NumberOfPossibleSeriesImpedanceStates.ToString() + "{0}", Environment.NewLine);
                stringBuilder.AppendFormat("Possible Impedance States:{0}", Environment.NewLine);

                foreach (Impedance impedance in transmissionLine.PossibleImpedanceValues)
                {
                    stringBuilder.AppendFormat(impedance.ToVerboseString() + "{0}", Environment.NewLine);
                }
                stringBuilder.AppendFormat(transmissionLine.Graph.ResolveToSingleSeriesBranch().RawImpedanceParameters.ToVerboseString() + "{0}", Environment.NewLine);
            }
            File.WriteAllText(Directory.GetCurrentDirectory() + "/TransmissionLineAdjacencyLists.txt", stringBuilder.ToString());
            ActionStatus = "Wrote to " + Directory.GetCurrentDirectory() + "/TransmissionLineAdjacencyLists.txt";
            Process.Start(Directory.GetCurrentDirectory() + "/TransmissionLineAdjacencyLists.txt");
        }

        private void ViewObservableSubstations()
        {
            StringBuilder stringBuilder = new StringBuilder();
            List<Substation> observableSubstations = new List<Substation>();

            foreach (ObservedBus observedBus in m_network.Model.ObservedBusses)
            {
                foreach (Node node in observedBus.Nodes)
                {
                    if (!observableSubstations.Contains(node.ParentSubstation))
                    {
                        observableSubstations.Add(node.ParentSubstation);
                    }
                }
            }

            stringBuilder.AppendFormat($"{observableSubstations.Count} Observed Buses");
            foreach (Substation substation in observableSubstations)
            {
                stringBuilder.AppendFormat(substation.ToVerboseString());
                List<ObservedBus> substationBuses = new List<ObservedBus>();
                foreach (ObservedBus observedBus in m_network.Model.ObservedBusses)
                {
                    foreach (Node node in observedBus.Nodes)
                    {
                        if (!observableSubstations.Contains(node.ParentSubstation))
                        {
                            if (!substationBuses.Contains(observedBus))
                            {
                                substationBuses.Add(observedBus);
                            }
                        }
                    }
                }
                foreach (ObservedBus substationBus in substationBuses)
                {
                    stringBuilder.AppendFormat(substationBus.ToString());
                }
            }
            ShowTextReport("ObservedBuses.txt", stringBuilder.ToString());
        }
        
        #endregion

        private void ViewCalculatedImpedances()
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (TransmissionLine transmissionLine in m_network.Model.TransmissionLines)
            {
                stringBuilder.AppendFormat(transmissionLine.ToVerboseString() + "{0}", Environment.NewLine);
                stringBuilder.AppendLine("Modeled Impedance Values");
                stringBuilder.AppendFormat(transmissionLine.Graph.ResolveToSingleSeriesBranch().RawImpedanceParameters.ToVerboseString() + "{0}", Environment.NewLine);
                stringBuilder.AppendLine("Calculated Impedance Values");
                stringBuilder.AppendFormat(transmissionLine.RealTimeCalculatedImpedance.ToVerboseString() + "{0}", Environment.NewLine);
            }
            File.WriteAllText(Directory.GetCurrentDirectory() + "/CalculatedImpedances.txt", stringBuilder.ToString());
            ActionStatus = "Wrote to " + Directory.GetCurrentDirectory() + "/CalculatedImpedances.txt";
            Process.Start(Directory.GetCurrentDirectory() + "/CalculatedImpedances.txt");
        }
        
        private void ViewSeriesCompensatorInferenceData()
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (TransmissionLine transmissionLine in m_network.Model.TransmissionLines)
            {
                if (transmissionLine.WillPerformSeriesCompensatorStatusInference)
                {
                    stringBuilder.AppendFormat(transmissionLine.ToVerboseString() + "{0}", Environment.NewLine);
                    stringBuilder.AppendFormat("Directly Connected Adjacency List{0}", Environment.NewLine);
                    stringBuilder.AppendFormat(transmissionLine.Graph.DirectlyConnectedAdjacencyList.ToString() + "{0}", Environment.NewLine);
                    stringBuilder.AppendFormat("Series Impedance Connected Adjacency List{0}", Environment.NewLine);
                    stringBuilder.AppendFormat(transmissionLine.Graph.SeriesImpedanceConnectedAdjacencyList.ToString() + "{0}", Environment.NewLine);
                    stringBuilder.AppendFormat("Transmission Line Tree{0}", Environment.NewLine);
                    stringBuilder.AppendFormat(transmissionLine.Graph.RootNode.ToSubtreeString() + "{0}", Environment.NewLine);
                    stringBuilder.AppendFormat("Number of Possible Impedance States: " + transmissionLine.NumberOfPossibleSeriesImpedanceStates.ToString() + "{0}", Environment.NewLine);
                    stringBuilder.AppendFormat("Possible Impedance States:{0}", Environment.NewLine);
                    foreach (Impedance impedance in transmissionLine.PossibleImpedanceValues)
                    {
                        stringBuilder.AppendFormat(impedance.ToVerboseString() + "{0}", Environment.NewLine);
                    }
                    stringBuilder.AppendFormat("Inferred Total Impedance:{0}", Environment.NewLine);
                    stringBuilder.AppendFormat(transmissionLine.InferredTotalImpedance.ToVerboseString());
                    stringBuilder.AppendFormat("Single Flow Branch Resolution:{0}", Environment.NewLine);
                    stringBuilder.AppendFormat(transmissionLine.Graph.ResolveToSingleSeriesBranch().RawImpedanceParameters.ToVerboseString() + "{0}", Environment.NewLine);
                    stringBuilder.AppendFormat("Asserted Total Branch Impedance:{0}", Environment.NewLine);
                    stringBuilder.AppendFormat(transmissionLine.FromSideImpedanceToDeepestObservability.ToVerboseString() + "{0}", Environment.NewLine);
                }

            }
            File.WriteAllText(Directory.GetCurrentDirectory() + "/SeriesCompensatorStatusInferencingData.txt", stringBuilder.ToString());
            ActionStatus = "Wrote to " + Directory.GetCurrentDirectory() + "/SeriesCompensatorStatusInferencingData.txt";
            Process.Start(Directory.GetCurrentDirectory() + "/SeriesCompensatorStatusInferencingData.txt");
        }
        
        private void ViewSeriesCompensators()
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (TransmissionLine transmissionLine in m_network.Model.TransmissionLines)
            {
                if (transmissionLine.WillPerformSeriesCompensatorStatusInference)
                {
                    foreach (SeriesCompensator seriesCompensator in transmissionLine.SeriesCompensators)
                    {
                        stringBuilder.AppendFormat(seriesCompensator.ToVerboseString() + "{0}", Environment.NewLine);
                    }
                }
            }
            File.WriteAllText(Directory.GetCurrentDirectory() + "/SeriesCompensators.txt", stringBuilder.ToString());
            ActionStatus = "Wrote to " + Directory.GetCurrentDirectory() + "/SeriesCompensators.txt";
            Process.Start(Directory.GetCurrentDirectory() + "/SeriesCompensators.txt");
        }
        
        private void ViewTransformers()
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (Transformer transformer in m_network.Model.Transformers)
            {
                stringBuilder.AppendFormat(transformer.ToVerboseString() + "{0}", Environment.NewLine);
            }
            File.WriteAllText(Directory.GetCurrentDirectory() + "/Transformers.txt", stringBuilder.ToString());
            ActionStatus = "Wrote to " + Directory.GetCurrentDirectory() + "/Transformers.txt";
            Process.Start(Directory.GetCurrentDirectory() + "/Transformers.txt");
        }

        #region [ View Voltage Phasors ]

        private void ViewModeledVoltages()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat(m_network.Model.Voltages.Count.ToString() + " Modeled Voltage Phasors");
            foreach (VoltagePhasorGroup voltagePhasorGroup in m_network.Model.Voltages)
            {
                stringBuilder.AppendFormat(voltagePhasorGroup.ToVerboseString() + "{0}", Environment.NewLine);
            }

            File.WriteAllText(Directory.GetCurrentDirectory() + "/ModeledVoltages.txt", stringBuilder.ToString());
            ActionStatus = "Wrote to " + Directory.GetCurrentDirectory() + "/ModeledVoltages.txt";
            Process.Start(Directory.GetCurrentDirectory() + "/ModeledVoltages.txt");
        }

        private void ViewExpectedVoltages()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat(m_network.Model.ExpectedVoltages.Count.ToString() + " Expected Voltage Phasors");
            foreach (VoltagePhasorGroup voltagePhasorGroup in m_network.Model.ExpectedVoltages)
            {
                stringBuilder.AppendFormat(voltagePhasorGroup.ToVerboseString() + "{0}", Environment.NewLine);
            }

            File.WriteAllText(Directory.GetCurrentDirectory() + "/ExpectedVoltages.txt", stringBuilder.ToString());
            ActionStatus = "Wrote to " + Directory.GetCurrentDirectory() + "/ExpectedVoltages.txt";
            Process.Start(Directory.GetCurrentDirectory() + "/ExpectedVoltages.txt");
        }

        private void ViewActiveVoltages()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat(m_network.Model.ActiveVoltages.Count.ToString() + " Active Voltage Phasors");
            foreach (VoltagePhasorGroup voltagePhasorGroup in m_network.Model.ActiveVoltages)
            {
                stringBuilder.AppendFormat(voltagePhasorGroup.ToVerboseString() + "{0}", Environment.NewLine);
            }

            File.WriteAllText(Directory.GetCurrentDirectory() + "/ActiveVoltages.txt", stringBuilder.ToString());
            ActionStatus = "Wrote to " + Directory.GetCurrentDirectory() + "/ActiveVoltages.txt";
            Process.Start(Directory.GetCurrentDirectory() + "/ActiveVoltages.txt");
        }

        private void ViewInactiveVoltages()
        {
            StringBuilder stringBuilder = new StringBuilder();
            List<VoltagePhasorGroup> inactiveVoltages = new List<VoltagePhasorGroup>();

            foreach (VoltagePhasorGroup voltage in m_network.Model.ExpectedVoltages)
            {
                if (!m_network.Model.ActiveVoltages.Contains(voltage))
                {
                    inactiveVoltages.Add(voltage);
                }
            }
            stringBuilder.AppendFormat(inactiveVoltages.Count.ToString() + " Inactive Voltage Phasors");
            foreach (VoltagePhasorGroup voltage in inactiveVoltages)
            {
                stringBuilder.AppendFormat(voltage.ToVerboseString() + "{0}", Environment.NewLine);
            }
            File.WriteAllText(Directory.GetCurrentDirectory() + "/InactiveVoltages.txt", stringBuilder.ToString());
            ActionStatus = "Wrote to " + Directory.GetCurrentDirectory() + "/InactiveVoltages.txt";
            Process.Start(Directory.GetCurrentDirectory() + "/InactiveVoltages.txt");
        }

        private void ViewReportedVoltages()
        {
            StringBuilder stringBuilder = new StringBuilder();
            List<VoltagePhasorGroup> reportedVoltages = new List<VoltagePhasorGroup>();
            foreach (VoltagePhasorGroup voltage in m_network.Model.ExpectedVoltages)
            {
                if (voltage.PositiveSequence.Measurement.MeasurementWasReported)
                {
                    reportedVoltages.Add(voltage);
                }
            }
            stringBuilder.AppendFormat(reportedVoltages.Count.ToString() + " Reported Voltage Phasors");
            foreach (VoltagePhasorGroup voltage in reportedVoltages)
            {
                stringBuilder.AppendFormat(voltage.ToVerboseString() + "{0}", Environment.NewLine);
            }
            ShowTextReport("ReportedVoltages.txt", stringBuilder.ToString());
        }

        private void ViewUnreportedVoltages()
        {
            StringBuilder stringBuilder = new StringBuilder();
            List<VoltagePhasorGroup> unreportedVoltages = new List<VoltagePhasorGroup>();
            foreach (VoltagePhasorGroup voltage in m_network.Model.ExpectedVoltages)
            {
                if (!voltage.PositiveSequence.Measurement.MeasurementWasReported)
                {
                    unreportedVoltages.Add(voltage);
                }
            }
            stringBuilder.AppendFormat(unreportedVoltages.Count.ToString() + " Unreported Voltage Phasors");
            foreach (VoltagePhasorGroup voltage in unreportedVoltages)
            {
                stringBuilder.AppendFormat(voltage.ToVerboseString() + "{0}", Environment.NewLine);
            }
            ShowTextReport("UnreportedVoltages.txt", stringBuilder.ToString());
        }

        private void ViewActiveVoltagesByStatusWord()
        {
            StringBuilder stringBuilder = new StringBuilder();
            List<VoltagePhasorGroup> activeVoltages = new List<VoltagePhasorGroup>();
            foreach (VoltagePhasorGroup voltage in m_network.Model.ExpectedVoltages)
            {
                StatusWord status = voltage.Status;
                if (status != null && m_network.Model.StatusWords.Contains(status))
                {
                    if (!(status.DataIsValid || status.SynchronizationIsValid))
                    {
                        activeVoltages.Add(voltage);
                    }
                }
            }
            stringBuilder.AppendFormat(activeVoltages.Count.ToString() + " Active Voltage Phasors by Status Word");
            foreach (VoltagePhasorGroup voltage in activeVoltages)
            {
                stringBuilder.AppendFormat(voltage.ToVerboseString() + "{0}", Environment.NewLine);
            }
            ShowTextReport("ActiveVoltagesByStatusWord.txt", stringBuilder.ToString());
        }

        private void ViewInactiveVoltagesByStatusWord()
        {
            StringBuilder stringBuilder = new StringBuilder();
            List<VoltagePhasorGroup> inactiveVoltages = new List<VoltagePhasorGroup>();
            foreach (VoltagePhasorGroup voltage in m_network.Model.ExpectedVoltages)
            {
                StatusWord status = voltage.Status;
                if (status != null && m_network.Model.StatusWords.Contains(status))
                {
                    if (status.DataIsValid || status.SynchronizationIsValid)
                    {
                        inactiveVoltages.Add(voltage);
                    }
                }
            }
            stringBuilder.AppendFormat(inactiveVoltages.Count.ToString() + " Inactive Voltage Phasors by Status Word");
            foreach (VoltagePhasorGroup voltage in inactiveVoltages)
            {
                stringBuilder.AppendFormat(voltage.ToVerboseString() + "{0}", Environment.NewLine);
            }
            ShowTextReport("InactiveVoltagesByStatusWord.txt", stringBuilder.ToString());
        }

        private void ViewInactiveVoltagesByMeasurement()
        {
            StringBuilder stringBuilder = new StringBuilder();
            List<VoltagePhasorGroup> inactiveVoltages = new List<VoltagePhasorGroup>();
            foreach (VoltagePhasorGroup voltage in m_network.Model.ExpectedVoltages)
            {
                if (!m_network.Model.ActiveVoltages.Contains(voltage))
                {
                    StatusWord status = voltage.Status;
                    if (status != null && m_network.Model.StatusWords.Contains(status))
                    {
                        if (!(status.DataIsValid || status.SynchronizationIsValid))
                        {
                            inactiveVoltages.Add(voltage);
                        }
                    }
                }
            }

            stringBuilder.AppendFormat(inactiveVoltages.Count.ToString() + " Inactive Voltage Phasors by Measurement");
            foreach (VoltagePhasorGroup voltage in inactiveVoltages)
            {
                stringBuilder.AppendFormat(voltage.ToVerboseString() + "{0}", Environment.NewLine);
            }
            ShowTextReport("InactiveVoltagesByMeasurement.txt", stringBuilder.ToString());
        }

        #endregion

        #region [ View Current Flow Phasors ]

        private void ViewModeledCurrentFlows()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat(m_network.Model.CurrentFlows.Count.ToString() + " Modeled Current Flow Phasors");
            foreach (CurrentFlowPhasorGroup currentFlowPhasorGroup in m_network.Model.CurrentFlows)
            {
                stringBuilder.AppendFormat(currentFlowPhasorGroup.ToVerboseString() + "{0}", Environment.NewLine);
            }
            File.WriteAllText(Directory.GetCurrentDirectory() + "/ModeledCurrentFlows.txt", stringBuilder.ToString());
            ActionStatus = "Wrote to " + Directory.GetCurrentDirectory() + "/ModeledCurrentFlows.txt";
            Process.Start(Directory.GetCurrentDirectory() + "/ModeledCurrentFlows.txt");
        }

        private void ViewExpectedCurrentFlows()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat(m_network.Model.ExpectedCurrentFlows.Count.ToString() + " Expected Current Flow Phasors");
            foreach (CurrentFlowPhasorGroup currentFlowPhasorGroup in m_network.Model.ExpectedCurrentFlows)
            {
                stringBuilder.AppendFormat(currentFlowPhasorGroup.ToVerboseString() + "{0}", Environment.NewLine);
            }
            File.WriteAllText(Directory.GetCurrentDirectory() + "/ExpectedCurrentFlows.txt", stringBuilder.ToString());
            ActionStatus = "Wrote to " + Directory.GetCurrentDirectory() + "/ExpectedCurrentFlows.txt";
            Process.Start(Directory.GetCurrentDirectory() + "/ExpectedCurrentFlows.txt");
        }

        private void ViewInactiveCurrentFlows()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("Inactive Current Flow Phasors");
            foreach (CurrentFlowPhasorGroup currentFlowPhasorGroup in m_network.Model.ExpectedCurrentFlows)
            {
                if (!m_network.Model.ActiveCurrentFlows.Contains(currentFlowPhasorGroup))
                {
                    stringBuilder.AppendFormat(currentFlowPhasorGroup.ToVerboseString() + "{0}", Environment.NewLine);
                }
            }
            File.WriteAllText(Directory.GetCurrentDirectory() + "/InactiveCurrentFlows.txt", stringBuilder.ToString());
            ActionStatus = "Wrote to " + Directory.GetCurrentDirectory() + "/InactiveCurrentFlows.txt";
            Process.Start(Directory.GetCurrentDirectory() + "/InactiveCurrentFlows.txt");
        }

        private void ViewIncludedCurrentFlows()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat(m_network.Model.IncludedCurrentFlows.Count.ToString() + " Included Current Flow Phasors");
            foreach (CurrentFlowPhasorGroup currentPhasorGroup in m_network.Model.IncludedCurrentFlows)
            {
                stringBuilder.AppendFormat(currentPhasorGroup.ToVerboseString() + "{0}", Environment.NewLine);
            }

            File.WriteAllText(Directory.GetCurrentDirectory() + "/IncludedCurrentFlows.txt", stringBuilder.ToString());
            ActionStatus = "Wrote to " + Directory.GetCurrentDirectory() + "/IncludedCurrentFlows.txt";
            Process.Start(Directory.GetCurrentDirectory() + "/IncludedCurrentFlows.txt");
        }

        private void ViewExcludedCurrentFlows()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("Excluded Current Flow Phasors");
            foreach (CurrentFlowPhasorGroup currentFlowPhasorGroup in m_network.Model.IncludedCurrentFlows)
            {
                if (!m_network.Model.ActiveCurrentFlows.Contains(currentFlowPhasorGroup))
                {
                    stringBuilder.AppendFormat(currentFlowPhasorGroup.ToVerboseString() + "{0}", Environment.NewLine);
                }
            }
            File.WriteAllText(Directory.GetCurrentDirectory() + "/ExcludedCurrentFlows.txt", stringBuilder.ToString());
            ActionStatus = "Wrote to " + Directory.GetCurrentDirectory() + "/ExcludedCurrentFlows.txt";
            Process.Start(Directory.GetCurrentDirectory() + "/ExcludedCurrentFlows.txt");
        }
        
        private void ViewActiveCurrentFlows()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat(m_network.Model.ActiveCurrentFlows.Count.ToString() + " Active Current Flow Phasors");
            foreach (CurrentFlowPhasorGroup currentPhasorGroup in m_network.Model.ActiveCurrentFlows)
            {
                stringBuilder.AppendFormat(currentPhasorGroup.ToVerboseString() + "{0}", Environment.NewLine);
            }

            File.WriteAllText(Directory.GetCurrentDirectory() + "/ActiveCurrentFlows.txt", stringBuilder.ToString());
            ActionStatus = "Wrote to " + Directory.GetCurrentDirectory() + "/ActiveCurrentFlows.txt";
            Process.Start(Directory.GetCurrentDirectory() + "/ActiveCurrentFlows.txt");
        }

        private void ViewReportedCurrentFlows()
        {
            StringBuilder stringBuilder = new StringBuilder();
            List<CurrentFlowPhasorGroup> reportedCurrents = new List<CurrentFlowPhasorGroup>();
            foreach (CurrentFlowPhasorGroup current in m_network.Model.ExpectedCurrentFlows)
            {
                if (current.PositiveSequence.Measurement.MeasurementWasReported)
                {
                    reportedCurrents.Add(current);
                }
            }
            stringBuilder.AppendFormat(reportedCurrents.Count.ToString() + " Reported Current Flow Phasors");
            foreach (CurrentFlowPhasorGroup current in reportedCurrents)
            {
                stringBuilder.AppendFormat(current.ToVerboseString() + "{0}", Environment.NewLine);
            }
            ShowTextReport("ReportedCurrentFlows.txt", stringBuilder.ToString());
        }

        private void ViewUnreportedCurrentFlows()
        {
            StringBuilder stringBuilder = new StringBuilder();
            List<CurrentFlowPhasorGroup> unreportedCurrents = new List<CurrentFlowPhasorGroup>();
            foreach (CurrentFlowPhasorGroup current in m_network.Model.ExpectedCurrentFlows)
            {
                if (!current.PositiveSequence.Measurement.MeasurementWasReported)
                {
                    unreportedCurrents.Add(current);
                }
            }
            stringBuilder.AppendFormat(unreportedCurrents.Count.ToString() + " Unreported Current Flow Phasors");
            foreach (CurrentFlowPhasorGroup current in unreportedCurrents)
            {
                stringBuilder.AppendFormat(current.ToVerboseString() + "{0}", Environment.NewLine);
            }
            ShowTextReport("UnreportedCurrentFlows.txt", stringBuilder.ToString());
        }

        private void ViewActiveCurrentFlowsByStatusWord()
        {
            StringBuilder stringBuilder = new StringBuilder();
            List<CurrentFlowPhasorGroup> activeCurrents = new List<CurrentFlowPhasorGroup>();
            foreach (CurrentFlowPhasorGroup current in m_network.Model.ExpectedCurrentFlows)
            {
                StatusWord status = current.Status;
                if (status != null && m_network.Model.StatusWords.Contains(status))
                {
                    if (!(status.DataIsValid || status.SynchronizationIsValid))
                    {
                        activeCurrents.Add(current);
                    }
                }
            }
            stringBuilder.AppendFormat(activeCurrents.Count.ToString() + " Active Current Flow Phasors by Status Word");
            foreach (CurrentFlowPhasorGroup current in activeCurrents)
            {
                stringBuilder.AppendFormat(current.ToVerboseString() + "{0}", Environment.NewLine);
            }
            ShowTextReport("ActiveCurrentFlowsByStatusWord.txt", stringBuilder.ToString());
        }

        private void ViewInactiveCurrentFlowsByStatusWord()
        {
            StringBuilder stringBuilder = new StringBuilder();
            List<CurrentFlowPhasorGroup> inactiveCurrents = new List<CurrentFlowPhasorGroup>();
            foreach (CurrentFlowPhasorGroup current in m_network.Model.ExpectedCurrentFlows)
            {
                StatusWord status = current.Status;
                if (status != null && m_network.Model.StatusWords.Contains(status))
                {
                    if (status.DataIsValid || status.SynchronizationIsValid)
                    {
                        inactiveCurrents.Add(current);
                    }
                }
            }
            stringBuilder.AppendFormat(inactiveCurrents.Count.ToString() + " Inactive Current Flow Phasors by Status Word");
            foreach (CurrentFlowPhasorGroup current in inactiveCurrents)
            {
                stringBuilder.AppendFormat(current.ToVerboseString() + "{0}", Environment.NewLine);
            }
            ShowTextReport("InactiveCurrentFlowsByStatusWord.txt", stringBuilder.ToString());
        }

        private void ViewInactiveCurrentFlowsByMeasurement()
        {
            StringBuilder stringBuilder = new StringBuilder();
            List<CurrentFlowPhasorGroup> inactiveCurrents = new List<CurrentFlowPhasorGroup>();
            foreach (CurrentFlowPhasorGroup current in m_network.Model.ExpectedCurrentFlows)
            {
                if (!m_network.Model.ActiveCurrentFlows.Contains(current))
                {
                    StatusWord status = current.Status;
                    if (status != null && m_network.Model.StatusWords.Contains(status))
                    {
                        if (!(status.DataIsValid || status.SynchronizationIsValid))
                        {
                            inactiveCurrents.Add(current);
                        }
                    }
                }
            }

            stringBuilder.AppendFormat(inactiveCurrents.Count.ToString() + " Inactive Current Flow Phasors by Measurement");
            foreach (CurrentFlowPhasorGroup current in inactiveCurrents)
            {
                stringBuilder.AppendFormat(current.ToVerboseString() + "{0}", Environment.NewLine);
            }
            ShowTextReport("InactiveCurrentFlowsByMeasurement.txt", stringBuilder.ToString());
        }
        #endregion

        #region [ View Current Injection Phasors ]

        private void ViewModeledCurrentInjections()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat(m_network.Model.CurrentInjections.Count.ToString() + " Modeled Current Injections Phasors");
            foreach (CurrentInjectionPhasorGroup currentInjectionPhasorGroup in m_network.Model.CurrentInjections)
            {
                stringBuilder.AppendFormat(currentInjectionPhasorGroup.ToVerboseString() + "{0}", Environment.NewLine);
            }
            File.WriteAllText(Directory.GetCurrentDirectory() + "/ModeledCurrentInjections.txt", stringBuilder.ToString());
            ActionStatus = "Wrote to " + Directory.GetCurrentDirectory() + "/ModeledCurrentInjections.txt";
            Process.Start(Directory.GetCurrentDirectory() + "/ModeledCurrentInjections.txt");
        }

        private void ViewExpectedCurrentInjections()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat(m_network.Model.ExpectedCurrentInjections.Count.ToString() + " Expected Current Injection Phasors");
            foreach (CurrentInjectionPhasorGroup currentInjectionPhasorGroup in m_network.Model.ExpectedCurrentInjections)
            {
                stringBuilder.AppendFormat(currentInjectionPhasorGroup.ToVerboseString() + "{0}", Environment.NewLine);
            }
            File.WriteAllText(Directory.GetCurrentDirectory() + "/ExpectedCurrentInjections.txt", stringBuilder.ToString());
            ActionStatus = "Wrote to " + Directory.GetCurrentDirectory() + "/ExpectedCurrentInjections.txt";
            Process.Start(Directory.GetCurrentDirectory() + "/ExpectedCurrentInjections.txt");
        }

        private void ViewActiveCurrentInjections()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat(m_network.Model.ActiveCurrentInjections.Count.ToString() + " Active Current Injection Phasors");
            foreach (CurrentInjectionPhasorGroup currentInjectionPhasorGroup in m_network.Model.ActiveCurrentInjections)
            {
                stringBuilder.AppendFormat(currentInjectionPhasorGroup.ToVerboseString() + "{0}", Environment.NewLine);
            }
            File.WriteAllText(Directory.GetCurrentDirectory() + "/ActiveCurrentInjections.txt", stringBuilder.ToString());
            ActionStatus = "Wrote to " + Directory.GetCurrentDirectory() + "/ActiveCurrentInjections.txt";
            Process.Start(Directory.GetCurrentDirectory() + "/ActiveCurrentInjections.txt");
        }

        private void ViewInactiveCurrentInjections()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("Inactive Current Injection Phasors");
            foreach (CurrentInjectionPhasorGroup currentInjectionPhasorGroup in m_network.Model.ExpectedCurrentInjections)
            {
                if (!m_network.Model.ActiveCurrentInjections.Contains(currentInjectionPhasorGroup))
                {
                    stringBuilder.AppendFormat(currentInjectionPhasorGroup.ToVerboseString() + "{0}", Environment.NewLine);
                }
            }
            File.WriteAllText(Directory.GetCurrentDirectory() + "/InactiveCurrentInjections.txt", stringBuilder.ToString());
            ActionStatus = "Wrote to " + Directory.GetCurrentDirectory() + "/InactiveCurrentInjections.txt";
            Process.Start(Directory.GetCurrentDirectory() + "/InactiveCurrentInjections.txt");
        }

        private void ViewReportedCurrentInjections()
        {
            StringBuilder stringBuilder = new StringBuilder();
            List<CurrentInjectionPhasorGroup> reportedCurrents = new List<CurrentInjectionPhasorGroup>();
            foreach (CurrentInjectionPhasorGroup current in m_network.Model.ExpectedCurrentInjections)
            {
                if (current.PositiveSequence.Measurement.MeasurementWasReported)
                {
                    reportedCurrents.Add(current);
                }
            }
            stringBuilder.AppendFormat(reportedCurrents.Count.ToString() + " Reported Current Injection Phasors");
            foreach (CurrentInjectionPhasorGroup current in reportedCurrents)
            {
                stringBuilder.AppendFormat(current.ToVerboseString() + "{0}", Environment.NewLine);
            }
            ShowTextReport("ReportedCurrentInjections.txt", stringBuilder.ToString());
        }

        private void ViewUnreportedCurrentInjections()
        {
            StringBuilder stringBuilder = new StringBuilder();
            List<CurrentInjectionPhasorGroup> unreportedCurrents = new List<CurrentInjectionPhasorGroup>();
            foreach (CurrentInjectionPhasorGroup current in m_network.Model.ExpectedCurrentInjections)
            {
                if (!current.PositiveSequence.Measurement.MeasurementWasReported)
                {
                    unreportedCurrents.Add(current);
                }
            }
            stringBuilder.AppendFormat(unreportedCurrents.Count.ToString() + " Unreported Current Injection Phasors");
            foreach (CurrentInjectionPhasorGroup current in unreportedCurrents)
            {
                stringBuilder.AppendFormat(current.ToVerboseString() + "{0}", Environment.NewLine);
            }
            ShowTextReport("UnreportedCurrentInjections.txt", stringBuilder.ToString());
        }

        private void ViewActiveCurrentInjectionsByStatusWord()
        {
            StringBuilder stringBuilder = new StringBuilder();
            List<CurrentInjectionPhasorGroup> activeCurrents = new List<CurrentInjectionPhasorGroup>();
            foreach (CurrentInjectionPhasorGroup current in m_network.Model.ExpectedCurrentInjections)
            {
                StatusWord status = current.Status;
                if (status != null && m_network.Model.StatusWords.Contains(status))
                {
                    if (!(status.DataIsValid || status.SynchronizationIsValid))
                    {
                        activeCurrents.Add(current);
                    }
                }
            }
            stringBuilder.AppendFormat(activeCurrents.Count.ToString() + " Active Current Injection Phasors by Status Word");
            foreach (CurrentInjectionPhasorGroup current in activeCurrents)
            {
                stringBuilder.AppendFormat(current.ToVerboseString() + "{0}", Environment.NewLine);
            }
            ShowTextReport("ActiveCurrentInjectionsByStatusWord.txt", stringBuilder.ToString());
        }

        private void ViewInactiveCurrentInjectionsByStatusWord()
        {
            StringBuilder stringBuilder = new StringBuilder();
            List<CurrentInjectionPhasorGroup> inactiveCurrents = new List<CurrentInjectionPhasorGroup>();
            foreach (CurrentInjectionPhasorGroup current in m_network.Model.ExpectedCurrentInjections)
            {
                StatusWord status = current.Status;
                if (status != null && m_network.Model.StatusWords.Contains(status))
                {
                    if (status.DataIsValid || status.SynchronizationIsValid)
                    {
                        inactiveCurrents.Add(current);
                    }
                }
            }
            stringBuilder.AppendFormat(inactiveCurrents.Count.ToString() + " Inactive Current Injection Phasors by Status Word");
            foreach (CurrentInjectionPhasorGroup current in inactiveCurrents)
            {
                stringBuilder.AppendFormat(current.ToVerboseString() + "{0}", Environment.NewLine);
            }
            ShowTextReport("InactiveCurrentInjectionsByStatusWord.txt", stringBuilder.ToString());
        }

        private void ViewInactiveCurrentInjectionsByMeasurement()
        {
            StringBuilder stringBuilder = new StringBuilder();
            List<CurrentInjectionPhasorGroup> inactiveCurrents = new List<CurrentInjectionPhasorGroup>();
            foreach (CurrentInjectionPhasorGroup current in m_network.Model.ExpectedCurrentInjections)
            {
                if (!m_network.Model.ActiveCurrentInjections.Contains(current))
                {
                    StatusWord status = current.Status;
                    if (status != null && m_network.Model.StatusWords.Contains(status))
                    {
                        if (!(status.DataIsValid || status.SynchronizationIsValid))
                        {
                            inactiveCurrents.Add(current);
                        }
                    }
                }
            }

            stringBuilder.AppendFormat(inactiveCurrents.Count.ToString() + " Inactive Current Injection Phasors by Measurement");
            foreach (CurrentInjectionPhasorGroup current in inactiveCurrents)
            {
                stringBuilder.AppendFormat(current.ToVerboseString() + "{0}", Environment.NewLine);
            }
            ShowTextReport("InactiveCurrentInjectionsByMeasurement.txt", stringBuilder.ToString());
        }
        #endregion

        #region [ View Switching Device Statuses ]

        public void ViewModeledBreakerStatuses()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat(m_network.Model.BreakerStatuses.Count.ToString() + " Modeled Breaker Statuses");

            foreach (BreakerStatus breakerStatus in m_network.Model.BreakerStatuses)
            {
                stringBuilder.AppendFormat(breakerStatus.ToVerboseString() + "{0}", Environment.NewLine);
            }

            ShowTextReport("ModeledBreakerStatuses.txt", stringBuilder.ToString());
        }

        public void ViewExpectedBreakerStatuses()
        {
            StringBuilder stringBuilder = new StringBuilder();

            foreach (BreakerStatus breakerStatus in m_network.Model.BreakerStatuses)
            {
                if (breakerStatus.IsEnabled && breakerStatus.Key != "Undefined")
                {
                    stringBuilder.AppendFormat(breakerStatus.ToVerboseString() + "{0}", Environment.NewLine);
                }
            }

            ShowTextReport("ExpectedBreakerStatuses.txt", stringBuilder.ToString());
        }

        public void ViewReportedBreakerStatuses()
        {
            StringBuilder stringBuilder = new StringBuilder();

            foreach (BreakerStatus breakerStatus in m_network.Model.BreakerStatuses)
            {
                if (breakerStatus.WasReported)
                {
                    stringBuilder.AppendFormat(breakerStatus.ToVerboseString() + "{0}", Environment.NewLine);
                }
            }

            ShowTextReport("ReportedBreakerStatuses.txt", stringBuilder.ToString());
        }

        public void ViewCircuitBreakers()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat(m_network.Model.CircuitBreakers.Count.ToString() + " Circuit Breakers");

            foreach (CircuitBreaker breaker in m_network.Model.CircuitBreakers)
            {
                stringBuilder.AppendFormat(breaker.ToVerboseString() + "{0}", Environment.NewLine);
            }

            ShowTextReport("CircuitBreakers.txt", stringBuilder.ToString());
        }

        public void ViewSwitches()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat(m_network.Model.Switches.Count.ToString() + " Switches");

            foreach (SynchrophasorAnalytics.Modeling.Switch circuitSwitch in m_network.Model.Switches)
            {
                stringBuilder.AppendFormat(circuitSwitch.ToVerboseString() + "{0}", Environment.NewLine);
            }

            ShowTextReport("Switches.txt", stringBuilder.ToString());
        }

        public void ViewAllSwitchingDevices()
        {
            StringBuilder stringBuilder = new StringBuilder();
            int count = m_network.Model.CircuitBreakers.Count + m_network.Model.Switches.Count;
            stringBuilder.AppendFormat(count.ToString() + " Switching Devices");

            foreach (CircuitBreaker breaker in m_network.Model.CircuitBreakers)
            {
                stringBuilder.AppendFormat(breaker.ToVerboseString() + "{0}", Environment.NewLine);
            }

            foreach (SynchrophasorAnalytics.Modeling.Switch circuitSwitch in m_network.Model.Switches)
            {
                stringBuilder.AppendFormat(circuitSwitch.ToVerboseString() + "{0}", Environment.NewLine);
            }

            ShowTextReport("SwitchingDevices.txt", stringBuilder.ToString());
        }

        #endregion

        private void ShowTextReport(string fileName, string content)
        {
            string absolutePath = Directory.GetCurrentDirectory() + $"/{fileName}";
            File.WriteAllText(absolutePath, content);
            ActionStatus = $"Wrote to {absolutePath}";
            Process.Start(absolutePath);
        }

        #endregion

        #endregion
    }
}
