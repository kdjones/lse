//******************************************************************************************************
//  NetworkModel.cs
//
//  Copyright © 2013, Kevin D. Jones.  All Rights Reserved.
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
//  06/01/2013 - Kevin D. Jones
//       Generated original version of source code.
//  07/10/2013 - Kevin D. Jones
//       Fixed issue with substations modeled without either direct or indirect observability. Before 
//       would cause a matrix singularity which is wrong. Correct LSE should never have a singularity. 
//       The observability analysis now removes those nodes from the problem. 
//  07/20/2013 - Kevin D. Jones
//       Added InputMeasurementKeys property for validating input measurement keys in the LSE.
//       Added AcceptsEstimates and AcceptsMeasurements for measurement mapping optimizations and
//       InputMeasurementKeys property. Added optimization to measurement mapping in Insert() methods.
//  08/10/2013 - Kevin D. Jones
//       Added I2-I1 ratio measurement keys to output measurements property.
//  06/23/2014 - Kevin D. Jones
//       Added 'CurrentFlowPostProcessingSetting' property.
//       Added 'IncludedCurrentPhasors' property with getter method.
//  06/01/2014 - Kevin D. Jones
//      Modifed the InputMeasurementKeys getter functions. Instead of Concat, used AddRange
//******************************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SynchrophasorAnalytics.Measurements;
using System.Xml.Serialization;
using SynchrophasorAnalytics.Networks;
using SynchrophasorAnalytics.Matrices;
using SynchrophasorAnalytics.Calibration;

namespace SynchrophasorAnalytics.Modeling
{
    /// <summary>
    /// A class which encapsulates the network elements and their hierarchy.
    /// </summary>
    [Serializable()]
    public class NetworkModel : INetworkDescribable
    {
        private const string UNDEFINED_KEY = "Undefined";

        #region [ Private Members ]

        #region [ INetworkDescribable fields ]

        private Guid m_uniqueId;
        private int m_internalId;
        private int m_number;
        private string m_acronym;
        private string m_name;
        private string m_description;

        #endregion

        private Dictionary<string, double> m_rawMeasurementKeyValuePairs;
        private PhaseSelection m_phaseSelection;
        private InputOutputSettings m_inputOutputSettings;
        private CurrentFlowPostProcessingSetting m_currentFlowPostProcessingSetting;
        private CurrentInjectionPostProcessingSetting m_currentInjectionPostProcessingSetting;

        /// <summary>
        /// Network Components
        /// </summary>
        private List<Company> m_companies;
        private List<Division> m_divisions;
        private List<Substation> m_substations;
        private List<TransmissionLine> m_transmissionLines;
        private List<Node> m_nodes;
        private List<Switch> m_switches;
        private List<CircuitBreaker> m_circuitBreakers;
        private List<ShuntCompensator> m_shuntCompensators;
        private List<SeriesCompensator> m_seriesCompensators;
        private List<LineSegment> m_lineSegments;
        private List<VoltageLevel> m_voltageLevels;
        private List<Transformer> m_transformers;
        private List<ObservedBus> m_observedBuses;
        private List<ObservedBus> m_potentiallyObservedBuses;
        private List<SwitchingDeviceBase> m_switchingDevices;
        private List<TapConfiguration> m_tapConfigurations;

        /// <summary>
        /// Network Measurements
        /// </summary>
        private List<VoltagePhasorGroup> m_voltages;
        private List<VoltagePhasorGroup> m_expectedVoltages;
        private List<CurrentFlowPhasorGroup> m_currentFlows;
        private List<CurrentFlowPhasorGroup> m_expectedCurrentFlows;
        private List<CurrentInjectionPhasorGroup> m_currentInjections;
        private List<CurrentInjectionPhasorGroup> m_expectedCurrentInjections;
        private List<BreakerStatus> m_breakerStatuses;
        private List<BreakerStatus> m_expectedBreakerStatuses;
        private List<StatusWord> m_statusWords;
        private List<StatusWord> m_expectedStatusWords;
        private List<CurrentFlowPhasorGroup> m_activeCurrentFlows;
        private List<CurrentFlowPhasorGroup> m_potentiallyActiveCurrentFlows;
        private List<CurrentInjectionPhasorGroup> m_activeCurrentInjections;
        private List<CurrentInjectionPhasorGroup> m_potentiallyActiveCurrentInjections;

        /// <summary>
        /// Parent
        /// </summary>
        private Network m_parentNetwork;

        private bool m_inPruningMode;

        #endregion

        #region [ Properties ]

        public bool InPruningMode
        {
            get
            {
                return m_inPruningMode;
            }
            set
            {
                m_inPruningMode = value;
            }
        }

        #region [ INetworkDescribable Properties ]

        /// <summary>
        /// A statistically unique identifier for the instance of the class.
        /// </summary>
        [XmlAttribute("Uid")]
        public Guid UniqueId
        {
            get
            {
                if (m_uniqueId == Guid.Empty)
                {
                    m_uniqueId = Guid.NewGuid();
                }
                return m_uniqueId;
            }
            set
            {
                m_uniqueId = value;
            }
        }

        /// <summary>
        /// An integer identifier for each <see cref="LinearStateEstimator.Modeling.NetworkModel"/> which is intended to be unique among other objects of the same type.
        /// </summary>
        [XmlAttribute("InternalID")]
        public int InternalID
        {
            get
            {
                return m_internalId;
            }
            set
            {
                m_internalId = value;
            }
        }

        /// <summary>
        /// A descriptive integer for the instance of the <see cref="LinearStateEstimator.Modeling.NetworkModel"/>. There are no restrictions on uniqueness. 
        /// </summary>
        [XmlAttribute("Number")]
        public int Number
        {
            get
            {
                return m_number;
            }
            set
            {
                m_number = value;
            }
        }

        /// <summary>
        /// A string acronym for the instance of the <see cref="LinearStateEstimator.Modeling.NetworkModel"/>.
        /// </summary>
        [XmlAttribute("Acronym")]
        public string Acronym
        {
            get
            {
                return m_acronym;
            }
            set
            {
                m_acronym = value;
            }
        }

        /// <summary>
        /// The string name of the instance of the <see cref="LinearStateEstimator.Modeling.NetworkModel"/>.
        /// </summary>
        [XmlAttribute("Name")]
        public string Name
        {
            get
            {
                return m_name;
            }
            set
            {
                m_name = value;
            }
        }

        /// <summary>
        /// A string description of the instance of the <see cref="LinearStateEstimator.Modeling.NetworkModel"/>.
        /// </summary>
        [XmlAttribute("Decsription")]
        public string Description
        {
            get
            {
                return m_description;
            }
            set
            {
                m_description = value;
            }
        }

        /// <summary>
        /// Gets the type of the object as a string
        /// </summary>
        [XmlIgnore()]
        public string ElementType
        {
            get 
            {
                return this.GetType().ToString();
            }
        }

        #endregion

        #region [ I/O ]

        /// <summary>
        /// A Dictionary with the raw measurements and other inputs paired with their respective measurement keys.
        /// </summary>
        [XmlIgnore()]
        public Dictionary<string, double> InputKeyValuePairs
        {
            get
            {
                return m_rawMeasurementKeyValuePairs;
            }
            set
            {
                m_rawMeasurementKeyValuePairs = value;
            }
        }

        /// <summary>
        /// A Dictionary with the raw estimates and other output values paired with their respective measurement keys.
        /// </summary>
        [XmlIgnore()]
        public Dictionary<string, double> OutputKeyValuePairs
        {
            get
            {
                Dictionary<string, double> rawEstimateKeyValuePairs = new Dictionary<string, double>();

                #region [ State Estimate ]

                if (m_inputOutputSettings.ReturnsStateEstimate)
                {
                    AddVoltageEstimatesToOutput(rawEstimateKeyValuePairs);
                }

                #endregion

                #region [ Current Flow Estimates ] 

                if (m_inputOutputSettings.ReturnsCurrentFlow)
                {
                    AddCurrentFlowEstimatesToOutput(rawEstimateKeyValuePairs);
                }

                #endregion

                #region [ Current Injection Estimates ] 

                if (m_inputOutputSettings.ReturnsCurrentInjection)
                {
                    AddCurrentInjectionsToOutput(rawEstimateKeyValuePairs);
                }

                #endregion

                #region [ Voltage Residuals ] 

                if (m_inputOutputSettings.ReturnsVoltageResiduals)
                {
                    AddVoltageMeasurementResidualsToOutput(rawEstimateKeyValuePairs);
                }

                #endregion

                #region [ Current Residuals ] 

                if (m_inputOutputSettings.ReturnsCurrentResiduals)
                {
                    AddCurrentMeasurementResidualsToOutput(rawEstimateKeyValuePairs);
                }

                #endregion

                #region [ Circuit Breaker Status ] 

                if (m_inputOutputSettings.ReturnsCircuitBreakerStatus)
                {
                    AddCircuitBreakerStatusesToOutput(rawEstimateKeyValuePairs);
                }

                #endregion

                #region [ Switch Status ] 

                if (m_inputOutputSettings.ReturnsSwitchStatus)
                {
                    AddSwitchStatusesToOutput(rawEstimateKeyValuePairs);
                }

                #endregion

                #region [ Performance Metrics ] 

                if (m_inputOutputSettings.ReturnsPerformanceMetrics)
                {
                    AddPerformanceMetricsToOutput(rawEstimateKeyValuePairs);
                }

                #endregion

                #region [ Topology Profiling Information ] 

                if (m_inputOutputSettings.ReturnsTopologyProfilingInformation)
                {
                    AddTopologyProfilingInformationToOutput(rawEstimateKeyValuePairs);
                }

                #endregion

                #region [ Measurement Validation Flags ] 

                if (m_inputOutputSettings.ReturnsMeasurementValidationFlags)
                {
                    AddMeasurementValidationFlagsToOutput(rawEstimateKeyValuePairs);
                }

                #endregion

                return rawEstimateKeyValuePairs;
            }
        }

        //[XmlIgnore()]
        //public Dictionary<string, double> PerformanceMetricOutput
        //{
        //    get
        //    {

        //    }
        //}

        /// <summary>
        /// A list of input measurements keys that are expectd by the modeled network elements.
        /// </summary>
        [XmlIgnore()]
        public List<string> InputMeasurementKeys
        {
            get
            {
                return GetInputMeasurementKeys();
            }
        }

        /// <summary>
        /// A list of the output measurement keys the network elements will use for output values.
        /// </summary>
        [XmlIgnore()]
        public string OutputMeasurementKeys
        {
            get
            {
                string outputMeasurementKeys = "outputMeasurements={";
                List<string> rawEstimateKeys = OutputKeyValuePairs.Keys.ToList();

                foreach (string key in rawEstimateKeys)
                {
                    if (!key.Equals("Undefined"))
                    {
                        outputMeasurementKeys += key + ";";
                    }
                }
                outputMeasurementKeys += "}";

                return outputMeasurementKeys;
            }
        }

        [XmlIgnore()]
        public List<OutputMeasurement> StateEstimateOutput
        {
            get
            {
                return GetVoltageEstimatesOutput();
            }
        }

        [XmlIgnore()]
        public List<OutputMeasurement> CurrentFlowEstimateOutput
        {
            get
            {
                return GetCurrentFlowEstimatesOutput();
            }
        }

        [XmlIgnore()]
        public List<OutputMeasurement> CurrentInjectionEstimateOutput
        {
            get
            {
                return GetCurrentInjectionEstimatesOutput();
            }
        }

        [XmlIgnore()]
        public List<OutputMeasurement> VoltageResidualOutput
        {
            get
            {
                return GetVoltageMeasurementResidualsOutput();
            }
        }

        [XmlIgnore()]
        public List<OutputMeasurement> CurrentResidualOutput
        {
            get
            {
                return GetCurrentMeasurementResidualsOutput();
            }
        }

        [XmlIgnore()]
        public List<OutputMeasurement> CircuitBreakerStatusOutput
        {
            get
            {
                return GetCircuitBreakerStatusesOutput();
            }
        }

        [XmlIgnore()]
        public List<OutputMeasurement> SwitchStatusOutput
        {
            get
            {
                return GetSwitchStatusesOutput();
            }
        }

        [XmlIgnore()]
        public List<OutputMeasurement> PerformanceMetricOutput
        {
            get
            {
                return GetPerformanceMetricsOutput();
            }
        }

        [XmlIgnore()]
        public List<OutputMeasurement> TopologyProfilingOutput
        {
            get
            {
                return GetTopologyProfilingInformationOutput();
            }
        }

        [XmlIgnore()]
        public List<OutputMeasurement> MeasurementValidationFlagOutput
        {
            get
            {
                return GetMeasurementValidationFlagsOutput();
            }
        }

        #endregion

        #region [ Settings ]

        /// <summary>
        /// Determines whether to treat the network as a <see cref="PhaseSelection.PositiveSequence"/> approximation or as a full <see cref="PhaseSelection.ThreePhase"/> representation.
        /// </summary>
        [XmlAttribute("PhaseConfiguration")]
        public PhaseSelection PhaseConfiguration
        {
            get
            {
                return m_phaseSelection;
            }
            set
            {
                m_phaseSelection = value;
            }
        }

        /// <summary>
        /// Determines whether to compute estimated current flow for only branches with at least 1 measured value or based on node observability irrespective of breaker status fidelity.
        /// </summary>
        [XmlAttribute("CurrentFlowPostProcessingSetting")]
        public CurrentFlowPostProcessingSetting CurrentFlowPostProcessingSetting
        {
            get
            {
                return m_currentFlowPostProcessingSetting;
            }
            set
            {
                m_currentFlowPostProcessingSetting = value;
            }
        }

        /// <summary>
        /// Determines whether to compute estimated current injections for only shunts where the measured value is of sufficient quality or based on node observability irrespective of breaker status fidelity.
        /// </summary>
        [XmlAttribute("CurrentInjectionPostProcessingSetting")]
        public CurrentInjectionPostProcessingSetting CurrentInjectionPostProcessingSetting
        {
            get
            {
                return m_currentInjectionPostProcessingSetting;
            }
            set
            {
                m_currentInjectionPostProcessingSetting = value;
            }
        }

        /// <summary>
        /// A flag that determines whether the model accepts measured values during OnNewMeasurements()
        /// </summary>
        [XmlAttribute("AcceptsMeasurements")]
        public bool AcceptsMeasurements
        {
            get
            {
                return m_inputOutputSettings.AcceptsMeasurements;
            }
            set
            {
                m_inputOutputSettings.AcceptsMeasurements = value;
            }
        }

        /// <summary>
        /// A flag that determines whether the model accepts estimated values during OnNewMeasurements()
        /// </summary>
        [XmlAttribute("AcceptsEstimates")]
        public bool AcceptsEstimates
        {
            get
            {
                return m_inputOutputSettings.AcceptsEstimates;
            }
            set
            {
                m_inputOutputSettings.AcceptsEstimates = value;
            }
        }

        /// <summary>
        /// A flag that determines whether the model returns state estimated values
        /// </summary>
        [XmlAttribute("ReturnsStateEstimate")]
        public bool ReturnsStateEstimate
        {
            get
            {
                return m_inputOutputSettings.ReturnsStateEstimate;
            }
            set
            {
                m_inputOutputSettings.ReturnsStateEstimate = value;
            }
        }

        /// <summary>
        /// A flag that determines whether the model returns estimated current flow values
        /// </summary>
        [XmlAttribute("ReturnsCurrentFlow")]
        public bool ReturnsCurrentFlow
        {
            get
            {
                return m_inputOutputSettings.ReturnsCurrentFlow;
            }
            set
            {
                m_inputOutputSettings.ReturnsCurrentFlow = value;
            }
        }

        /// <summary>
        /// A flag that determines whether the model returns estimated current injection values
        /// </summary>
        [XmlAttribute("ReturnsCurrentInjection")]
        public bool ReturnsCurrentInjection
        {
            get
            {
                return m_inputOutputSettings.ReturnsCurrentInjection;
            }
            set
            {
                m_inputOutputSettings.ReturnsCurrentInjection = value;
            }
        }

        /// <summary>
        /// A flag that determines whether the model returns voltage measurement residual values
        /// </summary>
        [XmlAttribute("ReturnsVoltageResiduals")]
        public bool ReturnsVoltageResiduals
        {
            get
            {
                return m_inputOutputSettings.ReturnsVoltageResiduals;
            }
            set
            {
                m_inputOutputSettings.ReturnsVoltageResiduals = value;
            }
        }

        /// <summary>
        /// A flag that determines whether the model returns current measurement residual values
        /// </summary>
        [XmlAttribute("ReturnsCurrentResiduals")]
        public bool ReturnsCurrentResiduals
        {
            get
            {
                return m_inputOutputSettings.ReturnsCurrentResiduals;
            }
            set
            {
                m_inputOutputSettings.ReturnsCurrentResiduals = value;
            }
        }

        /// <summary>
        /// A flag that determines whether the model returns circuit breaker statuses
        /// </summary>
        [XmlAttribute("ReturnsCircuitBreakerStatus")]
        public bool ReturnsCircuitBreakerStatus
        {
            get
            {
                return m_inputOutputSettings.ReturnsCircuitBreakerStatus;
            }
            set
            {
                m_inputOutputSettings.ReturnsCircuitBreakerStatus = value;
            }
        }

        /// <summary>
        /// A flag that determines whether the model returns switch status values
        /// </summary>
        [XmlAttribute("ReturnsSwitchStatus")]
        public bool ReturnsSwitchStatus
        {
            get
            {
                return m_inputOutputSettings.ReturnsSwitchStatus;
            }
            set
            {
                m_inputOutputSettings.ReturnsSwitchStatus = value;
            }
        }

        /// <summary>
        /// A flag that determines whether the model returns tap position values
        /// </summary>
        [XmlAttribute("ReturnsTapPositions")]
        public bool ReturnsTapPositions
        {
            get
            {
                return m_inputOutputSettings.ReturnsTapPositions;
            }
            set
            {
                m_inputOutputSettings.ReturnsTapPositions = value;
            }
        }

        /// <summary>
        /// A flag that determines whether the model returns the status of the series compensators.
        /// </summary>
        [XmlAttribute("ReturnsSeriesCompensatorStatus")]
        public bool ReturnsSeriesCompensatorStatus
        {
            get
            {
                return m_inputOutputSettings.ReturnsSeriesCompensatorStatus;
            }
            set
            {
                m_inputOutputSettings.ReturnsSeriesCompensatorStatus = value;
            }
        }

        [XmlAttribute("ReturnsPerformanceMetrics")]
        public bool ReturnsPerformanceMetrics
        {
            get
            {
                return m_inputOutputSettings.ReturnsPerformanceMetrics;
            }
            set
            {
                m_inputOutputSettings.ReturnsPerformanceMetrics = value;
            }
        }

        [XmlAttribute("ReturnsTopologyProfilingInformation")]
        public bool ReturnsTopologyProfilingInformation
        {
            get
            {
                return m_inputOutputSettings.ReturnsTopologyProfilingInformation;
            }
            set
            {
                m_inputOutputSettings.ReturnsTopologyProfilingInformation = value;
            }
        }

        [XmlAttribute("ReturnsMeasurementValidationFlags")]
        public bool ReturnsMeasurementValidationFlags
        {
            get
            {
                return m_inputOutputSettings.ReturnsMeasurementValidationFlags;
            }
            set
            {
                m_inputOutputSettings.ReturnsMeasurementValidationFlags = value;
            }
        }

        #endregion

        #region [ Network Components ]

        /// <summary>
        /// A list of <see cref="Company"/> objects in the <see cref="NetworkModel"/>
        /// </summary>
        [XmlArray("Companies")]
        public List<Company> Companies
        {
            get 
            { 
                return m_companies; 
            }
            set 
            { 
                m_companies = value; 
            }
        }

        /// <summary>
        /// A list of <see cref="LinearStateEstimator.Modeling.Division"/> objects in the <see cref="LinearStateEstimator.Modeling.NetworkModel"/>.
        /// </summary>
        [XmlIgnore()]
        public List<Division> Divisions
        {
            get
            {
                return m_divisions;
            }
            set
            {
                m_divisions = value;
            }
        }

        /// <summary>
        /// A list of <see cref="LinearStateEstimator.Modeling.Substation"/> objects in the <see cref="LinearStateEstimator.Modeling.NetworkModel"/>.
        /// </summary>
        [XmlIgnore()]
        public List<Substation> Substations
        {
            get
            {
                return m_substations;
            }
            set
            {
                m_substations = value;
            }
        }

        /// <summary>
        /// A list of <see cref="LinearStateEstimator.Modeling.TransmissionLine"/> objects in the <see cref="LinearStateEstimator.Modeling.NetworkModel"/>.
        /// </summary>
        [XmlIgnore()]
        public List<TransmissionLine> TransmissionLines
        {
            get
            {
                return m_transmissionLines;
            }
            set
            {
                m_transmissionLines = value;
            }
        }

        /// <summary>
        /// A list of <see cref="LinearStateEstimator.Modeling.Node"/> objects in the <see cref="LinearStateEstimator.Modeling.NetworkModel"/>.
        /// </summary>
        [XmlIgnore()]
        public List<Node> Nodes
        {
            get
            {
                return m_nodes;
            }
            set
            {
                m_nodes = value;
            }
        }

        /// <summary>
        /// A list of <see cref="LinearStateEstimator.Modeling.Switch"/> objects in the <see cref="LinearStateEstimator.Modeling.NetworkModel"/>.
        /// </summary>
        [XmlIgnore()]
        public List<Switch> Switches
        {
            get
            {
                return m_switches;
            }
            set
            {
                m_switches = value;
            }
        }

        /// <summary>
        /// A list of <see cref="LinearStateEstimator.Modeling.CircuitBreaker"/> objects in the <see cref="LinearStateEstimator.Modeling.NetworkModel"/>.
        /// </summary>
        [XmlIgnore()]
        public List<CircuitBreaker> CircuitBreakers
        {
            get
            {
                return m_circuitBreakers;
            }
            set
            {
                m_circuitBreakers = value;
            }
        }

        /// <summary>
        /// A list of <see cref="LinearStateEstimator.Modeling.Transformer"/> objects in the <see cref="LinearStateEstimator.Modeling.NetworkModel"/>.
        /// </summary>
        [XmlIgnore()]
        public List<Transformer> Transformers
        {
            get
            {
                return m_transformers;
            }
            set
            {
                m_transformers = value;
            }
        }

        /// <summary>
        /// A list of <see cref="LinearStateEstimator.Modeling.ShuntCompensator"/> objects in the <see cref="LinearStateEstimator.Modeling.NetworkModel"/>.
        /// </summary>
        [XmlIgnore()]
        public List<ShuntCompensator> ShuntCompensators
        {
            get
            {
                return m_shuntCompensators;
            }
            set
            {
                m_shuntCompensators = value;
            }
        }

        /// <summary>
        /// A list of <see cref="LinearStateEstimator.Modeling.LineSegment"/> objects in the <see cref="LinearStateEstimator.Modeling.NetworkModel"/>.
        /// </summary>
        [XmlIgnore()]
        public List<LineSegment> LineSegments
        {
            get
            {
                return m_lineSegments;
            }
            set
            {
                m_lineSegments = value;
            }
        }

        /// <summary>
        /// A list of <see cref="LinearStateEstimator.Modeling.SeriesCompensator"/> objects in the <see cref="LinearStateEstimator.Modeling.NetworkModel"/>.
        /// </summary>
        [XmlIgnore()]
        public List<SeriesCompensator> SeriesCompensators
        {
            get
            {
                return m_seriesCompensators;
            }
            set
            {
                m_seriesCompensators = value;
            }
        }

        /// <summary>
        /// A list of <see cref="VoltageLevels"/> in the <see cref="NetworkModel"/>
        /// </summary>
        [XmlArray("VoltageLevels")]
        public List<VoltageLevel> VoltageLevels
        {
            get
            {
                return m_voltageLevels;
            }
            set
            {
                m_voltageLevels = value;
            }
        }

        /// <summary>
        /// A list of the tap models used by the transformers in the network model.
        /// </summary>
        [XmlArray("TapConfigurations")]
        public List<TapConfiguration> TapConfigurations
        {
            get
            {
                return m_tapConfigurations;
            }
            set
            {
                m_tapConfigurations = value;
            }
        }

        /// <summary>
        /// A list of the present observed busses (collections of nodes) in the network based on topology processing and observability analysis.
        /// </summary>
        [XmlIgnore()]
        public List<ObservedBus> ObservedBusses
        {
            get
            {
                return m_observedBuses;
            }
            set
            {
                m_observedBuses = value;
            }
        }

        #endregion

        #region [ Network Measurements ]

        /// <summary>
        /// A list of all of the collections of voltage phasors modeled in the network.
        /// </summary>
        [XmlIgnore()]
        public List<VoltagePhasorGroup> Voltages
        {
            get
            {
                return m_voltages;
            }
            set
            {
                m_voltages = value;
            }
        }

        /// <summary>
        /// A list of all of the current flow measurements modeled in the network.
        /// </summary>
        [XmlIgnore()]
        public List<CurrentFlowPhasorGroup> CurrentFlows
        {
            get
            {
                return m_currentFlows;
            }
            set
            {
                m_currentFlows = value;
            }
        }

        /// <summary>
        /// A list of all of the shunt current injections modeled in the network.
        /// </summary>
        [XmlIgnore()]
        public List<CurrentInjectionPhasorGroup> CurrentInjections
        {
            get
            {
                return m_currentInjections;
            }
            set
            {
                m_currentInjections = value;
            }
        }

        /// <summary>
        /// A list of all of the breaker statuses modeled in the network.
        /// </summary>
        [XmlArray("BreakerStatuses")]
        public List<BreakerStatus> BreakerStatuses
        {
            get
            {
                return m_breakerStatuses;
            }
            set
            {
                m_breakerStatuses = value;
            }
        }

        [XmlIgnore()]
        public List<BreakerStatus> ExpectedBreakerStatuses
        {
            get
            {
                return m_expectedBreakerStatuses;
            }
            set
            {
                m_expectedBreakerStatuses = value;
            }
        }

        /// <summary>
        /// A list of all of the status words from all of the devices streaming phasor measurements in the network.
        /// </summary>
        [XmlArray("StatusWords")]
        public List<StatusWord> StatusWords
        {
            get
            {
                return m_statusWords;
            }
            set
            {
                m_statusWords = value;
            }
        }

        [XmlIgnore()]
        public List<StatusWord> ExpectedStatusWords
        {
            get
            {
                return m_expectedStatusWords;
            }
            set
            {
                m_expectedStatusWords = value;
            }
        }

        /// <summary>
        /// A list of all of the voltage phasor measurements which have been modeled to accept measurements. (measurement keys not equal to the 'Undefined' keyword)
        /// </summary>
        [XmlIgnore()]
        public List<VoltagePhasorGroup> ExpectedVoltages
        {
            get
            {
                return m_expectedVoltages;
            }
            set
            {
                m_expectedVoltages = value;
            }
        }

        /// <summary>
        /// A list of all of the current flow phasor measurements which have been modeled to accept measurements. (measurement keys not equal to the 'Undefined' keyword)
        /// </summary>
        [XmlIgnore()]
        public List<CurrentFlowPhasorGroup> ExpectedCurrentFlows
        {
            get
            {
                return m_expectedCurrentFlows;
            }
            set
            {
                m_expectedCurrentFlows = value;
            }
        }

        /// <summary>
        /// A list of all of the current injection phasor measurements which have been modeled to accept measurements. (measurement keys not equal to the 'Undefined' keyword)
        /// </summary>
        [XmlIgnore()]
        public List<CurrentInjectionPhasorGroup> ExpectedCurrentInjections
        {
            get
            {
                return m_expectedCurrentInjections;
            }
            set
            {
                m_expectedCurrentInjections = value;
            }
        }

        /// <summary>
        /// A list of all of the voltage phasor measurements which are of sufficient quality to be considered for inclusion in the state estimation problem. A subset of the total list of voltage phasor measurements.
        /// </summary>
        [XmlIgnore()]
        public List<VoltagePhasorGroup> ActiveVoltages
        {
            get
            {
                if (m_phaseSelection == PhaseSelection.PositiveSequence)
                {
                    return m_voltages.FindAll(voltage => voltage.IncludeInPositiveSequenceEstimator);
                }
                else if (m_phaseSelection == PhaseSelection.ThreePhase)
                {
                    return m_voltages.FindAll(voltage => voltage.IncludeInEstimator);
                }
                else
                {
                    return new List<VoltagePhasorGroup>();
                }
            }
        }

        /// <summary>
        /// A list of all of the current flow phasor measurements which are of sufficient quality to be considered for inclusion in the state estimation problem. A subset of the total list of current flow phasor measurements.
        /// </summary>
        [XmlIgnore()]
        public List<CurrentFlowPhasorGroup> ActiveCurrentFlows
        {
            get
            {
                return m_activeCurrentFlows;
            }
            set
            {
                m_activeCurrentFlows = value;
            }
        }

        /// <summary>
        /// A list of all of the current flow phasor measurements which satisfy necessary criteria to be included in the state estimation problem. Quality and observability is taken into consideration. It is a subset of the <see cref="LinearStateEstimator.Modeling.NetworkModel.ActiveCurrentFlows"/>.
        /// </summary>
        [XmlIgnore()]
        public List<CurrentFlowPhasorGroup> IncludedCurrentFlows
        {
            get
            {
                return ActiveCurrentFlows.FindAll(x => x.MeasuredFromNode.IsObserved);
            }
        }

        /// <summary>
        /// A list of all of the current injection phasor measurements which satisfy necessary crieteria to be included in the state estimation problem. A subst of the tital list of current injection phasor measurements.
        /// </summary>
        [XmlIgnore()]
        public List<CurrentInjectionPhasorGroup> ActiveCurrentInjections
        {
            get
            {
                if (m_phaseSelection == PhaseSelection.PositiveSequence)
                {
                    return m_currentInjections.FindAll(current => current.IncludeInPositiveSequenceEstimator);
                }
                else if (m_phaseSelection == PhaseSelection.ThreePhase)
                {
                    return m_currentInjections.FindAll(current => current.IncludeInEstimator);
                }
                else
                {
                    return new List<CurrentInjectionPhasorGroup>();
                }
                //return m_activeCurrentInjections;
            }
            set
            {
                m_activeCurrentInjections = value;
            }
        }

        #endregion

        #region [ Parent ]

        /// <summary>
        /// The parent of the network model
        /// </summary>
        [XmlIgnore()]
        public Network ParentNetwork
        {
            get
            {
                return m_parentNetwork;
            }
            set
            {
                m_parentNetwork = value;
            }
        }

        #endregion

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// The designated constructor method for the <see cref="LinearStateEstimator.Modeling.NetworkModel"/> class.
        /// </summary>
        public NetworkModel()
        {
            m_internalId = 1;
            m_number = 1;
            m_name = "Model";
            m_acronym = "MODEL";
            m_description = "Network Model Description";
            m_rawMeasurementKeyValuePairs = new Dictionary<string, double>();
            InitializeComponentLists();
        }

        #endregion

        #region [ Public Methods ]

        /// <summary>
        /// Reconstitues the references between parent, child, and sibling objects in 
        /// the <see cref="NetworkModel"/> as well as several other initialization functions.
        /// </summary>
        public void Initialize()
        {
            LinkTransmissionLineReferencesToSubstations();
            LinkHierarchicalReferences();
            LinkVoltageLevelReferences();
            LinkTapConfigurationReferences();
            ListNetworkComponents();
            ListNetworkMeasurements();
            ListExpectedBreakerStatuses();
            ListExpectedStatusWords();
            LinkBreakerStatusToCircuitBreakers();
            LinkStatusWordsToPhasorGroups();
            LinkVoltageLevelsToPhasorGroups();
            InitializeComplexPowerCalculations();
            InitializeSubstationVoltageLevelGroups();
            m_observedBuses = new List<ObservedBus>();
            m_potentiallyObservedBuses = new List<ObservedBus>();
            m_activeCurrentFlows = new List<CurrentFlowPhasorGroup>();
            m_potentiallyActiveCurrentFlows = new List<CurrentFlowPhasorGroup>();
            m_activeCurrentInjections = new List<CurrentInjectionPhasorGroup>();
            m_potentiallyActiveCurrentInjections = new List<CurrentInjectionPhasorGroup>();

        }

        public void ListExpectedBreakerStatuses()
        {
            m_expectedBreakerStatuses = new List<BreakerStatus>();

            foreach (CircuitBreaker breaker in CircuitBreakers)
            {
                if (breaker.Status != null && BreakerStatuses.Contains(breaker.Status))
                {
                    m_expectedBreakerStatuses.Add(breaker.Status);
                }
            }
        }

        public void ListExpectedStatusWords()
        {
            m_expectedStatusWords = new List<StatusWord>();
            
            foreach (VoltagePhasorGroup voltage in ExpectedVoltages)
            {
                if (voltage.Status != null && StatusWords.Contains(voltage.Status))
                {
                    if (!m_expectedStatusWords.Contains(voltage.Status))
                    {
                        m_expectedStatusWords.Add(voltage.Status);
                    }
                }
            }
            foreach (CurrentFlowPhasorGroup current in ExpectedCurrentFlows)
            {
                if (current.Status != null && StatusWords.Contains(current.Status))
                {
                    if (!m_expectedStatusWords.Contains(current.Status))
                    {
                        m_expectedStatusWords.Add(current.Status);
                    }
                }
            }
            foreach (CurrentInjectionPhasorGroup current in ExpectedCurrentInjections)
            {
                if (current.Status != null && StatusWords.Contains(current.Status))
                {
                    if (!m_expectedStatusWords.Contains(current.Status))
                    {
                        m_expectedStatusWords.Add(current.Status);
                    }
                }
            }
        }

        public void Unkeyify()
        {
            foreach (VoltagePhasorGroup voltage in m_voltages)
            {
                voltage.Unkeyify();
            }

            foreach (CurrentFlowPhasorGroup current in m_currentFlows)
            {
                current.Unkeyify();
            }

            foreach (CurrentInjectionPhasorGroup current in m_currentInjections)
            {
                current.Unkeyify();
            }

            foreach (BreakerStatus breakerStatus in m_breakerStatuses)
            {
                breakerStatus.Unkeyify();
            }

            foreach (StatusWord statusWord in m_statusWords)
            {
                statusWord.Unkeyify();
            }

            foreach (Transformer transformer in m_transformers)
            {
                transformer.Unkeyify();
            }

            foreach (Switch ciruitSwitch in m_switches)
            {
                ciruitSwitch.Unkeyify();
            }

            foreach (CircuitBreaker circuitBreaker in m_circuitBreakers)
            {
                circuitBreaker.Unkeyify();
            }

            foreach (SeriesCompensator seriesCompensator in m_seriesCompensators)
            {
                seriesCompensator.Unkeyify();
            }

            foreach (Substation substation in m_substations)
            {
                substation.UnKeyify();
            }

            foreach (Node node in m_nodes)
            {
                node.Unkeyify();
            }
        }

        /// <summary>
        /// Clears the values before receiving a new set of measurements
        /// </summary>
        public void ClearValues()
        {
            foreach (VoltagePhasorGroup voltagePhasorGroup in ExpectedVoltages)
            {
                voltagePhasorGroup.ClearValues();
            }

            foreach (CurrentFlowPhasorGroup currentPhasorGroup in ExpectedCurrentFlows)
            {
                currentPhasorGroup.ClearValues();
            }

            foreach (CurrentInjectionPhasorGroup currentInjectionPhasorGroup in ExpectedCurrentInjections)
            {
                currentInjectionPhasorGroup.ClearValues();
            }

            foreach (BreakerStatus breakerStatus in ExpectedBreakerStatuses)
            {
                breakerStatus.ClearValues();
            }

            foreach (StatusWord statusWord in ExpectedStatusWords)
            {
                statusWord.ClearValues();
            }
        }

        /// <summary>
        /// Called when a full set of measurements has been passed to the <see cref="NetworkModel"/>
        /// </summary>
        public void OnNewMeasurements()
        {
            InsertVoltageMeasurements();
            InsertCurrentFlowMeasurements();
            InsertCurrentInjectionMeasurements();
            InsertBreakerStatuses();
            InsertStatusWords();
            InsertTransformerTapPositions();
        }

        /// <summary>
        /// The key value pairs of the measurements which were successfully assigned to their network element.
        /// </summary>
        /// <returns>A dictionary of the key value pairs.</returns>
        public Dictionary<string, double> GetReceivedMeasurements()
        {
            Dictionary<string, double> receivedMeasurements = new Dictionary<string, double>();

            AppendReceivedVoltageMeasurements(receivedMeasurements);
            AppendReceivedCurrentFlowMeasurements(receivedMeasurements);
            AppendReceivedCurrentInjectionMeasurements(receivedMeasurements);

            return receivedMeasurements;
        }

        //public void ResolveToLevelZeroObservedBuses()
        //{

        //}

        //public void ResolveToLevelOneObservedBuses()
        //{

        //}

        //public void ResolveToLevelTwoObservedBuses()
        //{

        //}

        //public void ResolveToLevelThreeObservedBuses()
        //{

        //}

        //public void ResolveToLevelFourObservedBuses()
        //{

        //}

        //private void OriginalResolveToObservedBuses()
        //{

        //}

        /// <summary>
        /// Resolves the network from breaker/switch/node to bus/branch.
        /// </summary>
        /// <returns></returns>
        public void ResolveToObservedBuses()
        {
            m_observedBuses.Clear();

            List<List<ObservedBus>> substationBuses = new List<List<ObservedBus>>();

            foreach (Substation substation in m_substations)
            {
                substation.InitializeSubstationGraph();
                substation.Graph.ResolveAdjacencies();
                List<ObservedBus> observedBuses = substation.Graph.ResolveToObservedBuses();
                if (observedBuses != null)
                {
                    substationBuses.Add(observedBuses);
                }
            }
            foreach (List<ObservedBus> observedBuses in substationBuses)
            {
                MarkDirectObservability(observedBuses);
            }
            foreach (List<ObservedBus> observedBuses in substationBuses)
            {
                MarkDirectObservabilityByCurrentInjection(observedBuses);
            }
            foreach (List<ObservedBus> observedBuses in substationBuses)
            {
                MarkIndirectObservabilityByVoltage(observedBuses);
            }
            foreach (List<ObservedBus> observedBuses in substationBuses)
            {
                MarkIndirectObservabilityByCurrentFlow(observedBuses);
            }
            foreach (List<ObservedBus> observedBuses in substationBuses)
            {
                RemoveUnobservedBuses(observedBuses);
            }
            foreach (List<ObservedBus> observedBuses in substationBuses)
            {
                foreach (ObservedBus bus in observedBuses)
                {
                    m_observedBuses.Add(bus);
                }
            }

        }

        /// <summary>
        /// 
        /// </summary>
        public void ResolveToSingleFlowBranches()
        {
            foreach (TransmissionLine transmissionLine in m_transmissionLines)
            {
                transmissionLine.RunTopologyProcessing();
                transmissionLine.ComputeRealTimePositiveSequenceImpedance();
                transmissionLine.RunSeriesCompensatorStatusInference();
                transmissionLine.SetFinalImpedanceValues();
            }
        }

        /// <summary>
        /// Determines which phasors are eligible for state computation.
        /// </summary>
        /// <returns></returns>
        public void DetermineActiveCurrentFlows()
        {
            m_activeCurrentFlows.Clear();

            if (m_phaseSelection == PhaseSelection.PositiveSequence)
            {
                foreach (CurrentFlowPhasorGroup currentPhasorGroup in ExpectedCurrentFlows)
                {
                    if (currentPhasorGroup.IncludeInPositiveSequenceEstimator)
                    {
                        m_activeCurrentFlows.Add(currentPhasorGroup);
                    }
                }
            }
            else if (m_phaseSelection == PhaseSelection.ThreePhase)
            {
                foreach (CurrentFlowPhasorGroup currentPhasorGroup in ExpectedCurrentFlows)
                {
                    if (currentPhasorGroup.IncludeInEstimator)
                    {
                        m_activeCurrentFlows.Add(currentPhasorGroup);
                    }
                }
            }
        }

        /// <summary>
        /// Determines which phasors are eligible for state computation.
        /// </summary>
        /// <returns></returns>
        public void DetermineActiveCurrentInjections()
        {
            m_activeCurrentInjections.Clear();

            if (m_phaseSelection == PhaseSelection.PositiveSequence)
            {
                foreach (CurrentInjectionPhasorGroup currentPhasorGroup in ExpectedCurrentInjections)
                {
                    if (currentPhasorGroup.IncludeInPositiveSequenceEstimator)
                    {
                        m_activeCurrentInjections.Add(currentPhasorGroup);
                    }
                }
            }
            else if (m_phaseSelection == PhaseSelection.ThreePhase)
            {
                foreach (CurrentInjectionPhasorGroup currentPhasorGroup in ExpectedCurrentInjections)
                {
                    if (currentPhasorGroup.IncludeInEstimator)
                    {
                        m_activeCurrentInjections.Add(currentPhasorGroup);
                    }
                }
            }
        }

        /// <summary>
        /// Computes the sequence component values for the measured and estimated phasors
        /// </summary>
        public void ComputeSequenceValues()
        {
            ComputeVoltageSequenceValues();
            ComputeCurrentFlowSequenceValues();
            ComputeCurrentInjectionSequenceValues();
        }
        
        /// <summary>
        /// Computes the current flows on series branches based on the results of the linear state estimator.
        /// </summary>
        public void ComputeEstimatedCurrentFlows()
        {
            foreach (Transformer transformer in m_transformers)
            {
                if (m_phaseSelection == PhaseSelection.PositiveSequence)
                {
                    transformer.ComputePositiveSequenceEstimatedCurrentFlow();
                }
                else if (m_phaseSelection == PhaseSelection.ThreePhase)
                {
                    transformer.ComputeThreePhaseEstimatedCurrentFlow();
                }
            }

            foreach (TransmissionLine transmissionLine in m_transmissionLines)
            {
                if (m_phaseSelection == PhaseSelection.PositiveSequence)
                {
                    transmissionLine.ComputePositiveSequenceEstimatedCurrentFlow();
                }
                else if (m_phaseSelection == PhaseSelection.ThreePhase)
                {
                    transmissionLine.ComputeThreePhaseEstimatedCurrentFlow();
                }
            }
        }

        /// <summary>
        /// Computes the current injections of shunt devices based on the results from the linear state estimator.
        /// </summary>
        public void ComputeEstimatedCurrentInjections()
        {
            foreach (ShuntCompensator shunt in m_shuntCompensators)
            {
                if (m_phaseSelection == PhaseSelection.PositiveSequence)
                {
                    shunt.ComputePositiveSequenceEstimatedCurrentInjection();
                }
                else if (m_phaseSelection == PhaseSelection.ThreePhase)
                {
                    shunt.ComputeThreePhaseEstimatedCurrentInjection();
                }
            }
        }

        /// <summary>
        /// Changes the status of a specified switching device to a specified state.
        /// </summary>
        /// <param name="switchingDeviceName">The name of the switching device.</param>
        /// <param name="desiredActualState">The desired state for the switching device.</param>
        /// <returns>A boolean flag representing the success of the action.</returns>
        public bool ToggleSwitchingDeviceStatus(string switchingDeviceName, SwitchingDeviceActualState desiredActualState)
        {
            Dictionary<string, SwitchingDeviceBase> switchingDevices = m_switchingDevices.ToDictionary(x => x.Name, x => x);

            SwitchingDeviceBase switchingDevice = null;

            if (switchingDevices.TryGetValue(switchingDeviceName, out switchingDevice))
            {
                switchingDevice.ManuallySwitchTo(desiredActualState);

                return true;
            }
            else
            {
                return false;
            }

        }

        /// <summary>
        /// Removes the specified switching device from manual override status and specifies either to revert to a default/normal state or to remain in its present state until otherwise updated by measurements.
        /// </summary>
        /// <param name="switchingDeviceName">The specified switching device to be removed from manual override status.</param>
        /// <param name="revertToDefaultState">A boolean flag which represents whether to revert the switching device into a default/normal state or not.</param>
        /// <returns>A boolean flag representing the success of the action.</returns>
        public bool RemoveSwitchingDeviceFromManualWithPreserveOrDefault(string switchingDeviceName, bool revertToDefaultState)
        {
            Dictionary<string, SwitchingDeviceBase> switchingDevices = m_switchingDevices.ToDictionary(x => x.Name, x => x);

            SwitchingDeviceBase switchingDevice = null;

            if (switchingDevices.TryGetValue(switchingDeviceName, out switchingDevice))
            {
                if (revertToDefaultState)
                {
                    switchingDevice.RemoveFromManualAndRevertToDefault();
                }
                else
                {
                    switchingDevice.RemoveFromManualAndPreserveStateUntilUpdated();
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a list of the number of each network component and measurement.
        /// </summary>
        /// <returns>Returns the list.</returns>
        public string ComponentList()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("    ---- Component List ---- ");
            stringBuilder.AppendLine("           Companies: " + m_companies.Count.ToString());
            stringBuilder.AppendLine("           Divisions: " + m_divisions.Count.ToString());
            stringBuilder.AppendLine("         Substations: " + m_substations.Count.ToString());
            stringBuilder.AppendLine("  Transmission Lines: " + m_transmissionLines.Count.ToString());
            stringBuilder.AppendLine("               Nodes: " + m_nodes.Count.ToString());
            stringBuilder.AppendLine("            Switches: " + m_switches.Count.ToString());
            stringBuilder.AppendLine("    Circuit Breakers: " + m_circuitBreakers.Count.ToString());
            stringBuilder.AppendLine("        Transformers: " + m_transformers.Count.ToString());
            stringBuilder.AppendLine("  Shunt Compensators: " + m_shuntCompensators.Count.ToString());
            stringBuilder.AppendLine("       Line Segments: " + m_lineSegments.Count.ToString());
            stringBuilder.AppendLine(" Series Compensators: " + m_seriesCompensators.Count.ToString());
            stringBuilder.AppendLine("      Voltage Levels: " + m_voltageLevels.Count.ToString());
            stringBuilder.AppendLine("  Tap Configurations: " + m_tapConfigurations.Count.ToString());
            stringBuilder.AppendLine("            Voltages: " + m_voltages.Count.ToString());
            stringBuilder.AppendLine("       Current Flows: " + m_currentFlows.Count.ToString());
            stringBuilder.AppendLine("  Current Injections: " + m_currentInjections.Count.ToString());
            stringBuilder.AppendLine("      Breaker Status: " + m_breakerStatuses.Count.ToString());
            stringBuilder.AppendLine("        Status Words: " + m_statusWords.Count.ToString());

            return stringBuilder.ToString();
        }

        /// <summary>
        /// A summary of the measurement inclusion for the most recent set of network measurements.
        /// </summary>
        /// <returns>A string summary of each of the three phasor group types.</returns>
        public string MeasurementInclusionStatusList()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("      ---- Voltage Phasor Group Status ---- ");
            stringBuilder.AppendLine("  Modeled Voltages: " + Voltages.Count.ToString() + " groups");
            stringBuilder.AppendLine(" Expected Voltages: " + ExpectedVoltages.Count.ToString() + " groups");
            stringBuilder.AppendLine("   Active Voltages: " + ActiveVoltages.Count.ToString() + " groups");
            stringBuilder.AppendLine("");
            stringBuilder.AppendLine("    ---- Current Flow Phasor Group Status ---- ");
            stringBuilder.AppendLine("  Modeled Currents: " + CurrentFlows.Count.ToString() + " groups");
            stringBuilder.AppendLine(" Expected Currents: " + ExpectedCurrentFlows.Count.ToString() + " groups");
            stringBuilder.AppendLine("   Active Currents: " + ActiveCurrentFlows.Count.ToString() + " groups");
            stringBuilder.AppendLine(" Included Currents: " + IncludedCurrentFlows.Count.ToString() + " groups");
            stringBuilder.AppendLine("");
            stringBuilder.AppendLine("  ---- Current Injection Phasor Group Status ---- ");
            stringBuilder.AppendLine("  Modeled Currents: " + CurrentInjections.Count.ToString() + " groups");
            stringBuilder.AppendLine(" Expected Currents: " + ExpectedCurrentInjections.Count.ToString() + " groups");
            stringBuilder.AppendLine("   Active Currents: " + ActiveCurrentInjections.Count.ToString() + " groups");
            stringBuilder.AppendLine("");

            return stringBuilder.ToString();
        }

        public void Prune()
        {
            if (InPruningMode)
            {
                List<Company> companiesToPrune = new List<Company>();
                List<Division> divisionsToPrune = new List<Division>();
                List<Substation> substationsToPrune = new List<Substation>();
                List<TransmissionLine> transmissionLinesToPrune = new List<TransmissionLine>();

                // Identify prunable companies
                foreach (Company company in m_companies)
                {
                    if (!company.RetainWhenPruning)
                    {
                        companiesToPrune.Add(company);
                    }
                }

                // prune companies
                foreach (Company company in companiesToPrune)
                {
                    m_companies.Remove(company);
                }

                // identify prunable divisions
                foreach (Company company in m_companies)
                {
                    foreach (Division division in company.Divisions)
                    {
                        if (!division.RetainWhenPruning)
                        {
                            divisionsToPrune.Add(division);
                        }
                    }
                }

                // prune divisions
                foreach (Company company in m_companies)
                {
                    foreach (Division division in divisionsToPrune)
                    {
                        if (company.Divisions.Contains(division))
                        {
                            company.Divisions.Remove(division);
                        }
                    }
                }

                // identify prunable substations
                foreach (Company company in m_companies)
                {
                    foreach (Division division in company.Divisions)
                    {
                        foreach (Substation substation in division.Substations)
                        {
                            if (!substation.RetainWhenPruning)
                            {
                                substationsToPrune.Add(substation);
                            }
                        }
                    }
                }

                // prune substations
                foreach (Company company in m_companies)
                {
                    foreach (Division division in company.Divisions)
                    {
                        foreach (Substation substation in substationsToPrune)
                        {
                            if (division.Substations.Contains(substation))
                            {
                                division.Substations.Remove(substation);
                            }
                        }
                    }
                }

                // identify prunable transmission lines
                foreach (Company company in m_companies)
                {
                    foreach (Division division in company.Divisions)
                    {
                        foreach (TransmissionLine transmissionLine in division.TransmissionLines)
                        {
                            if (!transmissionLine.RetainWhenPruning)
                            {
                                transmissionLinesToPrune.Add(transmissionLine);
                            }
                        }
                    }
                }

                // prune transmission lines
                foreach (Company company in m_companies)
                {
                    foreach (Division division in company.Divisions)
                    {
                        foreach (TransmissionLine transmissionLine in transmissionLinesToPrune)
                        {
                            if (division.TransmissionLines.Contains(transmissionLine))
                            {
                                division.TransmissionLines.Remove(transmissionLine);
                            }
                        }
                    }
                }

                Initialize();
            }
        }

        public void PruneByVoltageLevels(List<int> retainedVoltageLevelInternalIds)
        {
            List<Node> nodesToPrune = new List<Node>();
            List<TransmissionLine> transmissionLinesToPrune = new List<TransmissionLine>();
            List<CircuitBreaker> retainedCircuitBreakers = new List<CircuitBreaker>();

            foreach (Node node in m_nodes)
            {
                if (!retainedVoltageLevelInternalIds.Contains(node.VoltageLevelID))
                {
                    nodesToPrune.Add(node);
                }
            }

            foreach (Company company in m_companies)
            {
                foreach (Division division in company.Divisions)
                {
                    division.TransmissionLines.RemoveAll(line => (nodesToPrune.Contains(line.FromNode) || nodesToPrune.Contains(line.ToNode)));
                    foreach (Substation substation in division.Substations)
                    {
                        substation.Shunts.RemoveAll(shunt => nodesToPrune.Contains(shunt.ConnectedNode));
                        substation.CircuitBreakers.RemoveAll(breaker => (nodesToPrune.Contains(breaker.FromNode) || nodesToPrune.Contains(breaker.ToNode)));
                        substation.Switches.RemoveAll(device => (nodesToPrune.Contains(device.FromNode) || nodesToPrune.Contains(device.ToNode)));
                        substation.Transformers.RemoveAll(transformer => (nodesToPrune.Contains(transformer.FromNode) || nodesToPrune.Contains(transformer.ToNode)));
                        substation.Nodes.RemoveAll(node => nodesToPrune.Contains(node));
                        retainedCircuitBreakers.AddRange(substation.CircuitBreakers);
                    }
                    division.Substations.RemoveAll(substation => substation.Nodes.Count == 0);
                }
                company.Divisions.RemoveAll(division => division.Substations.Count == 0);
            }
            m_companies.RemoveAll(company => company.Divisions.Count == 0);

            m_voltageLevels.RemoveAll(baseKv => !retainedVoltageLevelInternalIds.Contains(baseKv.InternalID));

            m_breakerStatuses.RemoveAll(status => !retainedCircuitBreakers.Contains(status.ParentCircuitBreaker));
        }

        public void PruneBySubstations(List<int> retainedSubstationInternalIds)
        {
            List<Company> companiesToPrune = new List<Company>();
            List<Division> divisionsToPrune = new List<Division>();
            List<Substation> substationsToPrune = new List<Substation>();
            List<TransmissionLine> transmissionLinesToPrune = new List<TransmissionLine>();

            // identify prunable transmission lines
            foreach (Company company in m_companies)
            {
                foreach (Division division in company.Divisions)
                {
                    foreach (TransmissionLine transmissionLine in division.TransmissionLines)
                    {
                        if (!(retainedSubstationInternalIds.Contains(transmissionLine.FromSubstationID) && retainedSubstationInternalIds.Contains(transmissionLine.ToSubstationID)))
                        {
                            transmissionLinesToPrune.Add(transmissionLine);
                        }
                    }
                }
            }

            // identify prunable substations
            foreach (Company company in m_companies)
            {
                foreach (Division division in company.Divisions)
                {
                    foreach (Substation substation in division.Substations)
                    {
                        if (!retainedSubstationInternalIds.Contains(substation.InternalID))
                        {
                            substationsToPrune.Add(substation);
                        }
                    }
                }
            }

            // prune transmission lines
            foreach (Company company in m_companies)
            {
                foreach (Division division in company.Divisions)
                {
                    foreach (TransmissionLine transmissionLine in transmissionLinesToPrune)
                    {
                        if (division.TransmissionLines.Contains(transmissionLine))
                        {
                            division.TransmissionLines.Remove(transmissionLine);
                        }
                    }
                }
            }

            // prune substations
            foreach (Company company in m_companies)
            {
                foreach (Division division in company.Divisions)
                {
                    foreach (Substation substation in substationsToPrune)
                    {
                        if (division.Substations.Contains(substation))
                        {
                            division.Substations.Remove(substation);
                        }
                    }
                }
            }

            // identify prunable divisions
            foreach (Company company in m_companies)
            {
                foreach (Division division in company.Divisions)
                {
                    if (division.Substations.Count == 0)
                    {
                        divisionsToPrune.Add(division);
                    }
                }
            }

            // prune divisions
            foreach (Company company in m_companies)
            {
                foreach (Division division in divisionsToPrune)
                {
                    if (company.Divisions.Contains(division))
                    {
                        company.Divisions.Remove(division);
                    }
                }
            }

            // Identify prunable companies
            foreach (Company company in m_companies)
            {
                if (company.Divisions.Count == 0)
                {
                    companiesToPrune.Add(company);
                }
            }

            // prune companies
            foreach (Company company in companiesToPrune)
            {
                m_companies.Remove(company);
            }
        }

        public void PruneByCompanies(List<int> retainedCompanyInternalIds)
        {
            foreach (Company company in m_companies)
            {
                foreach (Division division in company.Divisions)
                {
                    division.TransmissionLines.RemoveAll(line => !retainedCompanyInternalIds.Contains(line.FromSubstation.ParentDivision.ParentCompany.InternalID));
                    division.TransmissionLines.RemoveAll(line => !retainedCompanyInternalIds.Contains(line.ToSubstation.ParentDivision.ParentCompany.InternalID));
                }
            }

            m_companies.RemoveAll(company => !retainedCompanyInternalIds.Contains(company.InternalID));
        }

        public void EnableInferredStateAsActualProxy()
        {
            foreach (SwitchingDeviceBase device in m_switchingDevices)
            {
                device.UseInferredStateAsActualProxy = true;
            }
        }

        public void DisableInferredStateAsActualProxy()
        {
            foreach (SwitchingDeviceBase device in m_switchingDevices)
            {
                device.UseInferredStateAsActualProxy = false;
            }
        }

        #endregion

        #region [ Private Methods ]

        private void InitializeComponentLists()
        {
            // Network Components
            m_companies = new List<Company>();
            m_divisions = new List<Division>();
            m_substations = new List<Substation>();
            m_transmissionLines = new List<TransmissionLine>();
            m_nodes = new List<Node>();
            m_switches = new List<Switch>();
            m_circuitBreakers = new List<CircuitBreaker>();
            m_switchingDevices = new List<SwitchingDeviceBase>();
            m_shuntCompensators = new List<ShuntCompensator>();
            m_transformers = new List<Transformer>();
            m_lineSegments = new List<LineSegment>();
            m_seriesCompensators = new List<SeriesCompensator>();
            m_voltageLevels = new List<VoltageLevel>();
            m_tapConfigurations = new List<TapConfiguration>();

            // Network Measurements
            m_voltages = new List<VoltagePhasorGroup>();
            m_currentFlows = new List<CurrentFlowPhasorGroup>();
            m_currentInjections = new List<CurrentInjectionPhasorGroup>();
            m_breakerStatuses = new List<BreakerStatus>();
            m_statusWords = new List<StatusWord>();
            m_activeCurrentFlows = new List<CurrentFlowPhasorGroup>();
            m_activeCurrentInjections = new List<CurrentInjectionPhasorGroup>();

            // Network Settings
            m_inputOutputSettings = new InputOutputSettings();
        }

        /// <summary>
        /// Uses the <see cref="LinearStateEstimator.Modeling.TransmissionLine.FromSubstationID"/> and <see cref="LinearStateEstimator.Modeling.TransmissionLine.ToSubstationID"/>
        /// and the <see cref="LinearStateEstimator.Modeling.INetworkDescribable.InternalID"/> to link the <see cref="LinearStateEstimator.Modeling.Substation"/> records with their associated <see cref="LinearStateEstimator.Modeling.TransmissionLine"/>.
        /// </summary>
        private void LinkTransmissionLineReferencesToSubstations()
        {
            //Transmission Line From and To Substations
            Dictionary<int, Substation> substations = new Dictionary<int, Substation>();

            foreach (Company company in m_companies)
            {
                foreach (Division division in company.Divisions)
                {
                    foreach (Substation substation in division.Substations)
                    {
                        if (!substations.Keys.Contains(substation.InternalID))
                        {
                            substations.Add(substation.InternalID, substation);
                        }
                    }
                }
            }

            foreach (Company company in m_companies)
            {
                foreach (Division division in company.Divisions)
                {
                    foreach (TransmissionLine transmissionLine in division.TransmissionLines)
                    {
                        Substation value = null;
                        if (substations.TryGetValue(transmissionLine.FromSubstationID, out value))
                        {
                            transmissionLine.FromSubstation = value;
                            value = null;
                        }
                        if (substations.TryGetValue(transmissionLine.ToSubstationID, out value))
                        {
                            transmissionLine.ToSubstation = value;
                            value = null;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Establishes the parent/child relationship between entities in the <see cref="NetworkModel"/>.
        /// </summary>
        private void LinkHierarchicalReferences()
        {
            Dictionary<int, Node> substationNodes = new Dictionary<int, Node>();
            foreach (Company company in m_companies)
            {
                company.ParentModel = this;
                foreach (Division division in company.Divisions)
                {
                    division.ParentCompany = company;
                    foreach (Substation substation in division.Substations)
                    {
                        substation.ParentDivision = division;
                        foreach (Node node in substation.Nodes)
                        {
                            node.ParentSubstation = substation;
                            node.Voltage.MeasuredNode = node;
                            substationNodes.Add(node.InternalID, node);
                        }

                        Dictionary<int, Node> localNodes = substation.Nodes.ToDictionary(x => x.InternalID, x => x);
                        foreach (CircuitBreaker circuitBreaker in substation.CircuitBreakers)
                        {
                            circuitBreaker.ParentSubstation = substation;

                            Node value = null;
                            if (localNodes.TryGetValue(circuitBreaker.FromNodeID, out value))
                            {
                                circuitBreaker.FromNode = value;
                                value = null;
                            }
                            if (localNodes.TryGetValue(circuitBreaker.ToNodeID, out value))
                            {
                                circuitBreaker.ToNode = value;
                                value = null;
                            }
                        }

                        foreach (Switch circuitSwitch in substation.Switches)
                        {
                            circuitSwitch.ParentSubstation = substation;

                            Node value = null;
                            if (localNodes.TryGetValue(circuitSwitch.FromNodeID, out value))
                            {
                                circuitSwitch.FromNode = value;
                                value = null;
                            }
                            if (localNodes.TryGetValue(circuitSwitch.ToNodeID, out value))
                            {
                                circuitSwitch.ToNode = value;
                                value = null;
                            }
                        }

                        foreach (Transformer transformer in substation.Transformers)
                        {
                            transformer.ParentSubstation = substation;

                            Node value = null;
                            if (localNodes.TryGetValue(transformer.FromNodeID, out value))
                            {
                                transformer.FromNode = value;
                                transformer.FromNodeCurrent.MeasuredFromNode = value;
                                transformer.ToNodeCurrent.MeasuredToNode = value;
                                value = null;
                            }
                            if (localNodes.TryGetValue(transformer.ToNodeID, out value))
                            {
                                transformer.ToNode = value;
                                transformer.ToNodeCurrent.MeasuredFromNode = value;
                                transformer.FromNodeCurrent.MeasuredToNode = value;
                                value = null;
                            }
                        }

                        foreach (ShuntCompensator shunt in substation.Shunts)
                        {
                            shunt.ParentSubstation = substation;

                            Node value = null;
                            if (localNodes.TryGetValue(shunt.ConnectedNodeID, out value))
                            {
                                shunt.ConnectedNode = value;
                                value = null;
                            }
                        }
                    }
                }
            }

            foreach (Company company in m_companies)
            {
                foreach (Division division in company.Divisions)
                {
                    foreach (TransmissionLine transmissionLine in division.TransmissionLines)
                    {
                        transmissionLine.ParentDivision = division;
                        //transmissionLine.SetFinalImpedanceValues();
                        Dictionary<int, Node> transmissionLineNodes = transmissionLine.Nodes.ToDictionary(x => x.InternalID, x => x);
                        Node value = null;
                        if (substationNodes.TryGetValue(transmissionLine.FromNodeID, out value))
                        {
                            transmissionLine.FromNode = value;
                            transmissionLine.FromNode.ParentTransmissionLine = transmissionLine;
                            transmissionLine.FromSubstationCurrent.MeasuredFromNode = value;
                            transmissionLine.ToSubstationCurrent.MeasuredToNode = value;
                            transmissionLineNodes.Add(value.InternalID, value);
                            value = null;
                        }

                        if (substationNodes.TryGetValue(transmissionLine.ToNodeID, out value))
                        {
                            transmissionLine.ToNode = value;
                            transmissionLine.ToNode.ParentTransmissionLine = transmissionLine;
                            transmissionLine.ToSubstationCurrent.MeasuredFromNode = value;
                            transmissionLine.FromSubstationCurrent.MeasuredToNode = value;
                            transmissionLineNodes.Add(value.InternalID, value);
                            value = null;
                        }

                        foreach (Node node in transmissionLine.Nodes)
                        {
                            node.ParentTransmissionLine = transmissionLine;
                        }


                        foreach (Switch lineSwitch in transmissionLine.Switches)
                        {
                            lineSwitch.ParentTransmissionLine = transmissionLine;

                            value = null;
                            if (transmissionLineNodes.TryGetValue(lineSwitch.FromNodeID, out value))
                            {
                                lineSwitch.FromNode = value;
                                value = null;
                            }
                            if (transmissionLineNodes.TryGetValue(lineSwitch.ToNodeID, out value))
                            {
                                lineSwitch.ToNode = value;
                                value = null;
                            }
                        }

                        foreach (LineSegment lineSegment in transmissionLine.LineSegments)
                        {
                            lineSegment.ParentTransmissionLine = transmissionLine;

                            value = null;
                            if (transmissionLineNodes.TryGetValue(lineSegment.FromNodeID, out value))
                            {
                                lineSegment.FromNode = value;
                                value = null;
                            }
                            if (transmissionLineNodes.TryGetValue(lineSegment.ToNodeID, out value))
                            {
                                lineSegment.ToNode = value;
                                value = null;
                            }
                        }

                        foreach (SeriesCompensator seriesCompensator in transmissionLine.SeriesCompensators)
                        {
                            seriesCompensator.ParentTransmissionLine = transmissionLine;

                            value = null;
                            if (transmissionLineNodes.TryGetValue(seriesCompensator.FromNodeID, out value))
                            {
                                seriesCompensator.FromNode = value;
                                value = null;
                            }
                            if (transmissionLineNodes.TryGetValue(seriesCompensator.ToNodeID, out value))
                            {
                                seriesCompensator.ToNode = value;
                                value = null;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Uses the <see cref="Node.VoltageLevelID"/> and the <see cref="VoltageLevel.InternalID"/> to link
        /// the <see cref="VoltageLevel"/> records with their associated <see cref="Node"/>.
        /// </summary>
        private void LinkVoltageLevelReferences()
        {
            Dictionary<int, VoltageLevel> voltageLevels = m_voltageLevels.ToDictionary(x => x.InternalID, x => x);

            foreach (Company company in m_companies)
            {
                foreach (Division division in company.Divisions)
                {
                    foreach (Substation substation in division.Substations)
                    {
                        foreach (Node node in substation.Nodes)
                        {
                            VoltageLevel value = null;
                            if (voltageLevels.TryGetValue(node.VoltageLevelID, out value))
                            {
                                node.BaseKV = value;
                                value = null;
                            }
                        }
                    }

                    foreach (TransmissionLine transmissionLine in division.TransmissionLines)
                    {
                        foreach (Node node in transmissionLine.Nodes)
                        {
                            VoltageLevel value = null;
                            if (voltageLevels.TryGetValue(node.VoltageLevelID, out value))
                            {
                                node.BaseKV = value;
                                value = null;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Uses the <see cref="Transformer.TapConfigurationID"/> and the <see cref="TapConfiguration.InternalID"/> to link
        /// the <see cref="TapConfiguration"/> objects with their associated <see cref="Transformer.Tap"/>.
        /// </summary>
        private void LinkTapConfigurationReferences()
        {
            Dictionary<int, TapConfiguration> tapConfigurations = m_tapConfigurations.ToDictionary(x => x.InternalID, x => x);

            foreach (Company company in m_companies)
            {
                foreach (Division division in company.Divisions)
                {
                    foreach (Substation substation in division.Substations)
                    {
                        foreach (Transformer transformer in substation.Transformers)
                        {
                            TapConfiguration value = null;
                            if (tapConfigurations.TryGetValue(transformer.TapConfigurationID, out value))
                            {
                                transformer.Tap = value;
                                value = null;
                            }
                        }
                    }
                }
            }
        }

        private void ListNetworkComponents()
        {
            m_divisions = new List<Division>();
            m_substations = new List<Substation>();
            m_nodes = new List<Node>();
            m_switches = new List<Switch>();
            m_switchingDevices = new List<SwitchingDeviceBase>();
            m_circuitBreakers = new List<CircuitBreaker>();
            m_shuntCompensators = new List<ShuntCompensator>();
            m_transformers = new List<Transformer>();
            m_transmissionLines = new List<TransmissionLine>();
            m_lineSegments = new List<LineSegment>();
            m_seriesCompensators = new List<SeriesCompensator>();

            foreach (Company company in m_companies)
            {
                foreach (Division division in company.Divisions)
                {
                    m_divisions.Add(division);

                    foreach (Substation substation in division.Substations)
                    {
                        m_substations.Add(substation);

                        foreach (Node node in substation.Nodes)
                        {
                            m_nodes.Add(node);
                        }

                        foreach (Switch disconnect in substation.Switches)
                        {
                            m_switches.Add(disconnect);
                            m_switchingDevices.Add(disconnect as SwitchingDeviceBase);
                        }

                        foreach (CircuitBreaker circuitBreaker in substation.CircuitBreakers)
                        {
                            m_circuitBreakers.Add(circuitBreaker);
                            m_switchingDevices.Add(circuitBreaker as SwitchingDeviceBase);
                        }

                        foreach (ShuntCompensator shuntCompensator in substation.Shunts)
                        {
                            m_shuntCompensators.Add(shuntCompensator);
                        }

                        foreach (Transformer transformer in substation.Transformers)
                        {
                            m_transformers.Add(transformer);
                        }
                    }

                    foreach (TransmissionLine transmissionLine in division.TransmissionLines)
                    {
                        m_transmissionLines.Add(transmissionLine);

                        foreach (Node node in transmissionLine.Nodes)
                        {
                            m_nodes.Add(node);
                        }

                        foreach (Switch disconnect in transmissionLine.Switches)
                        {
                            m_switches.Add(disconnect);
                            m_switchingDevices.Add(disconnect as SwitchingDeviceBase);
                        }

                        foreach (LineSegment lineSegment in transmissionLine.LineSegments)
                        {
                            m_lineSegments.Add(lineSegment);
                        }

                        foreach (SeriesCompensator seriesCompensator in transmissionLine.SeriesCompensators)
                        {
                            m_seriesCompensators.Add(seriesCompensator);
                        }
                    }
                }
            }
        }

        private void ListNetworkMeasurements()
        {
            m_voltages = new List<VoltagePhasorGroup>();
            m_expectedVoltages = new List<VoltagePhasorGroup>();
            m_currentFlows = new List<CurrentFlowPhasorGroup>();
            m_expectedCurrentFlows = new List<CurrentFlowPhasorGroup>();
            m_currentInjections = new List<CurrentInjectionPhasorGroup>();
            m_expectedCurrentInjections = new List<CurrentInjectionPhasorGroup>();

            foreach (Company company in m_companies)
            {
                foreach (Division division in company.Divisions)
                {
                    foreach (Substation substation in division.Substations)
                    {
                        foreach (Node node in substation.Nodes)
                        {
                            m_voltages.Add(node.Voltage);
                            if (node.Voltage.ExpectsMeasurements)
                            {
                                m_expectedVoltages.Add(node.Voltage);
                            }
                        }

                        foreach (Transformer transformer in substation.Transformers)
                        {
                            // Add the current phasor groups to the list
                            m_currentFlows.Add(transformer.FromNodeCurrent);
                            m_currentFlows.Add(transformer.ToNodeCurrent);

                            // Set the measured branch of each current phasor group
                            transformer.FromNodeCurrent.MeasuredBranch = transformer;
                            transformer.ToNodeCurrent.MeasuredBranch = transformer;

                            if (transformer.FromNodeCurrent.ExpectsMeasurements)
                            {
                                m_expectedCurrentFlows.Add(transformer.FromNodeCurrent);
                            }
                            if (transformer.ToNodeCurrent.ExpectsMeasurements)
                            {
                                m_expectedCurrentFlows.Add(transformer.ToNodeCurrent);
                            }
                        }

                        foreach (ShuntCompensator shunt in substation.Shunts)
                        {
                            m_currentInjections.Add(shunt.Current);

                            shunt.Current.MeasuredBranch = shunt;

                            if (shunt.Current.ExpectsMeasurements)
                            {
                                m_expectedCurrentInjections.Add(shunt.Current);
                            }
                        }
                    }
                    foreach (TransmissionLine transmissionLine in division.TransmissionLines)
                    {
                        // Add the current phasor groups to the list
                        m_currentFlows.Add(transmissionLine.FromSubstationCurrent);
                        m_currentFlows.Add(transmissionLine.ToSubstationCurrent);

                        // Set the measured branch of each current phasor group
                        transmissionLine.FromSubstationCurrent.MeasuredBranch = transmissionLine;
                        transmissionLine.ToSubstationCurrent.MeasuredBranch = transmissionLine;

                        if (transmissionLine.FromSubstationCurrent.ExpectsMeasurements)
                        {
                            m_expectedCurrentFlows.Add(transmissionLine.FromSubstationCurrent);
                        }
                        if (transmissionLine.ToSubstationCurrent.ExpectsMeasurements)
                        {
                            m_expectedCurrentFlows.Add(transmissionLine.ToSubstationCurrent);
                        }

                        foreach (Node node in transmissionLine.Nodes)
                        {
                            if (!m_voltages.Contains(node.Voltage))
                            {
                                m_voltages.Add(node.Voltage);
                                if (node.Voltage.ExpectsMeasurements)
                                {
                                    m_expectedVoltages.Add(node.Voltage);
                                }

                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Links the <see cref="BreakerStatus"/> objects to their parent <see cref="CircuitBreaker"/>.
        /// </summary>
        private void LinkBreakerStatusToCircuitBreakers()
        {
            Dictionary<int, BreakerStatus> breakerStatuses = m_breakerStatuses.ToDictionary(x => x.InternalID, x => x);
            foreach (Company company in m_companies)
            {
                foreach (Division division in company.Divisions)
                {
                    foreach (Substation substation in division.Substations)
                    {
                        foreach (CircuitBreaker circuitBreaker in substation.CircuitBreakers)
                        {
                            BreakerStatus value = null;
                            if (circuitBreaker.StatusID != 0 && breakerStatuses.TryGetValue(circuitBreaker.StatusID, out value))
                            {
                                circuitBreaker.Status = value;
                                circuitBreaker.Status.ParentCircuitBreaker = circuitBreaker;
                                value = null;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Uses the <see cref="PhasorGroup.StatusWordID"/> and the <see cref="StatusWord.InternalID"/> to link
        /// the <see cref="StatusWord"/> measurements with their associated <see cref="PhasorGroup"/>.
        /// </summary>
        private void LinkStatusWordsToPhasorGroups()
        {
            Dictionary<int, StatusWord> statusWords = m_statusWords.ToDictionary(x => x.InternalID, x => x);

            foreach (VoltagePhasorGroup voltagePhasorGroup in m_voltages)
            {
                StatusWord value = null;

                if (voltagePhasorGroup.StatusWordID != 0 && statusWords.TryGetValue(voltagePhasorGroup.StatusWordID, out value))
                {
                    voltagePhasorGroup.Status = m_statusWords.Find(x => x.InternalID == voltagePhasorGroup.StatusWordID);
                    value = null;
                }
                else
                {
                    voltagePhasorGroup.Status = new StatusWord();
                }
            }

            foreach (CurrentFlowPhasorGroup currentPhasorGroup in m_currentFlows)
            {
                StatusWord value = null;
                if (currentPhasorGroup.StatusWordID != 0 && statusWords.TryGetValue(currentPhasorGroup.StatusWordID, out value))
                {
                    currentPhasorGroup.Status = m_statusWords.Find(x => x.InternalID == currentPhasorGroup.StatusWordID);
                    value = null;
                }
                else
                {
                    currentPhasorGroup.Status = new StatusWord();
                }
            }
        }

        /// <summary>
        /// Uses the <see cref="PhasorBase.VoltageLevelID"/> and the <see cref="VoltageLevel.InternalID"/> to link
        /// the <see cref="VoltageLevel"/> records with their associated <see cref="PhasorBase"/>.
        /// </summary>
        private void LinkVoltageLevelsToPhasorGroups()
        {
            foreach (VoltagePhasorGroup voltage in m_voltages)
            {
                voltage.PositiveSequence.Measurement.BaseKV = voltage.MeasuredNode.BaseKV;
                voltage.PositiveSequence.Estimate.BaseKV = voltage.MeasuredNode.BaseKV;
                if (m_phaseSelection == PhaseSelection.ThreePhase)
                {
                    voltage.PhaseA.Measurement.BaseKV = voltage.MeasuredNode.BaseKV;
                    voltage.PhaseA.Estimate.BaseKV = voltage.MeasuredNode.BaseKV;
                    voltage.PhaseB.Measurement.BaseKV = voltage.MeasuredNode.BaseKV;
                    voltage.PhaseB.Estimate.BaseKV = voltage.MeasuredNode.BaseKV;
                    voltage.PhaseC.Measurement.BaseKV = voltage.MeasuredNode.BaseKV;
                    voltage.PhaseC.Estimate.BaseKV = voltage.MeasuredNode.BaseKV;
                }
            }

            foreach (CurrentFlowPhasorGroup current in m_currentFlows)
            {
                current.PositiveSequence.Measurement.BaseKV = current.MeasuredFromNode.BaseKV;
                current.PositiveSequence.Estimate.BaseKV = current.MeasuredFromNode.BaseKV;
                if (m_phaseSelection == PhaseSelection.ThreePhase)
                {
                    current.PhaseA.Measurement.BaseKV = current.MeasuredFromNode.BaseKV;
                    current.PhaseA.Estimate.BaseKV = current.MeasuredFromNode.BaseKV;
                    current.PhaseB.Measurement.BaseKV = current.MeasuredFromNode.BaseKV;
                    current.PhaseB.Estimate.BaseKV = current.MeasuredFromNode.BaseKV;
                    current.PhaseC.Measurement.BaseKV = current.MeasuredFromNode.BaseKV;
                    current.PhaseC.Estimate.BaseKV = current.MeasuredFromNode.BaseKV;
                }
            }

            foreach (CurrentInjectionPhasorGroup current in m_currentInjections)
            {
                current.PositiveSequence.Measurement.BaseKV = current.MeasuredConnectedNode.BaseKV;
                current.PositiveSequence.Estimate.BaseKV = current.MeasuredConnectedNode.BaseKV;
                if (m_phaseSelection == PhaseSelection.ThreePhase)
                {
                    current.PhaseA.Measurement.BaseKV = current.MeasuredConnectedNode.BaseKV;
                    current.PhaseA.Estimate.BaseKV = current.MeasuredConnectedNode.BaseKV;
                    current.PhaseB.Measurement.BaseKV = current.MeasuredConnectedNode.BaseKV;
                    current.PhaseB.Estimate.BaseKV = current.MeasuredConnectedNode.BaseKV;
                    current.PhaseC.Measurement.BaseKV = current.MeasuredConnectedNode.BaseKV;
                    current.PhaseC.Estimate.BaseKV = current.MeasuredConnectedNode.BaseKV;
                }
            }
        }

        private List<string> GetInputMeasurementKeys()
        {
            List<string> inputMeasurementKeys = new List<string>();

            inputMeasurementKeys.AddRange(GetVoltagePhasorGroupInputKeys());
            inputMeasurementKeys.AddRange(GetCurrentFlowPhasorGroupInputKeys());
            inputMeasurementKeys.AddRange(GetCurrentInjectionPhasorGroupInputKeys());
            inputMeasurementKeys.AddRange(GetBreakerStatusInputKeys());
            inputMeasurementKeys.AddRange(GetStatusWordInputKeys());
            inputMeasurementKeys.AddRange(GetTransformerTapInputKeys());

            return inputMeasurementKeys;
        }

        private List<string> GetVoltagePhasorGroupInputKeys()
        {
            List<string> inputMeasurementKeys = new List<string>();

            foreach (VoltagePhasorGroup voltagePhasorGroup in m_voltages)
            {
                if (m_inputOutputSettings.AcceptsMeasurements)
                {
                    inputMeasurementKeys.Add(voltagePhasorGroup.PositiveSequence.Measurement.MagnitudeKey);
                    inputMeasurementKeys.Add(voltagePhasorGroup.PositiveSequence.Measurement.AngleKey);

                    if (m_phaseSelection == PhaseSelection.ThreePhase)
                    {
                        inputMeasurementKeys.Add(voltagePhasorGroup.PhaseA.Measurement.MagnitudeKey);
                        inputMeasurementKeys.Add(voltagePhasorGroup.PhaseA.Measurement.AngleKey);
                        inputMeasurementKeys.Add(voltagePhasorGroup.PhaseB.Measurement.MagnitudeKey);
                        inputMeasurementKeys.Add(voltagePhasorGroup.PhaseB.Measurement.AngleKey);
                        inputMeasurementKeys.Add(voltagePhasorGroup.PhaseC.Measurement.MagnitudeKey);
                        inputMeasurementKeys.Add(voltagePhasorGroup.PhaseC.Measurement.AngleKey);
                    }
                }

                if (m_inputOutputSettings.AcceptsEstimates)
                {
                    inputMeasurementKeys.Add(voltagePhasorGroup.PositiveSequence.Estimate.MagnitudeKey);
                    inputMeasurementKeys.Add(voltagePhasorGroup.PositiveSequence.Estimate.AngleKey);

                    if (m_phaseSelection == PhaseSelection.ThreePhase)
                    {
                        inputMeasurementKeys.Add(voltagePhasorGroup.PhaseA.Estimate.MagnitudeKey);
                        inputMeasurementKeys.Add(voltagePhasorGroup.PhaseA.Estimate.AngleKey);
                        inputMeasurementKeys.Add(voltagePhasorGroup.PhaseB.Estimate.MagnitudeKey);
                        inputMeasurementKeys.Add(voltagePhasorGroup.PhaseB.Estimate.AngleKey);
                        inputMeasurementKeys.Add(voltagePhasorGroup.PhaseC.Estimate.MagnitudeKey);
                        inputMeasurementKeys.Add(voltagePhasorGroup.PhaseC.Estimate.AngleKey);
                    }
                }
            }

            return inputMeasurementKeys;
        }

        private List<string> GetCurrentFlowPhasorGroupInputKeys()
        {
            List<string> inputMeasurementKeys = new List<string>();

            foreach (CurrentFlowPhasorGroup currentFlowPhasorGroup in m_currentFlows)
            {
                if (m_inputOutputSettings.AcceptsMeasurements)
                {
                    inputMeasurementKeys.Add(currentFlowPhasorGroup.PositiveSequence.Measurement.MagnitudeKey);
                    inputMeasurementKeys.Add(currentFlowPhasorGroup.PositiveSequence.Measurement.AngleKey);

                    if (m_phaseSelection == PhaseSelection.ThreePhase)
                    {
                        inputMeasurementKeys.Add(currentFlowPhasorGroup.PhaseA.Measurement.MagnitudeKey);
                        inputMeasurementKeys.Add(currentFlowPhasorGroup.PhaseA.Measurement.AngleKey);
                        inputMeasurementKeys.Add(currentFlowPhasorGroup.PhaseB.Measurement.MagnitudeKey);
                        inputMeasurementKeys.Add(currentFlowPhasorGroup.PhaseB.Measurement.AngleKey);
                        inputMeasurementKeys.Add(currentFlowPhasorGroup.PhaseC.Measurement.MagnitudeKey);
                        inputMeasurementKeys.Add(currentFlowPhasorGroup.PhaseC.Measurement.AngleKey);
                    }
                }

                if (m_inputOutputSettings.AcceptsEstimates)
                {
                    inputMeasurementKeys.Add(currentFlowPhasorGroup.PositiveSequence.Estimate.MagnitudeKey);
                    inputMeasurementKeys.Add(currentFlowPhasorGroup.PositiveSequence.Estimate.AngleKey);

                    if (m_phaseSelection == PhaseSelection.ThreePhase)
                    {
                        inputMeasurementKeys.Add(currentFlowPhasorGroup.PhaseA.Estimate.MagnitudeKey);
                        inputMeasurementKeys.Add(currentFlowPhasorGroup.PhaseA.Estimate.AngleKey);
                        inputMeasurementKeys.Add(currentFlowPhasorGroup.PhaseB.Estimate.MagnitudeKey);
                        inputMeasurementKeys.Add(currentFlowPhasorGroup.PhaseB.Estimate.AngleKey);
                        inputMeasurementKeys.Add(currentFlowPhasorGroup.PhaseC.Estimate.MagnitudeKey);
                        inputMeasurementKeys.Add(currentFlowPhasorGroup.PhaseC.Estimate.AngleKey);
                    }
                }
            }

            return inputMeasurementKeys;
        }

        private List<string> GetCurrentInjectionPhasorGroupInputKeys()
        {
            List<string> inputMeasurementKeys = new List<string>();

            foreach (CurrentInjectionPhasorGroup currentInjectionPhasorGroup in m_currentInjections)
            {
                if (m_inputOutputSettings.AcceptsMeasurements)
                {
                    inputMeasurementKeys.Add(currentInjectionPhasorGroup.PositiveSequence.Measurement.MagnitudeKey);
                    inputMeasurementKeys.Add(currentInjectionPhasorGroup.PositiveSequence.Measurement.AngleKey);

                    if (m_phaseSelection == PhaseSelection.ThreePhase)
                    {
                        inputMeasurementKeys.Add(currentInjectionPhasorGroup.PhaseA.Measurement.MagnitudeKey);
                        inputMeasurementKeys.Add(currentInjectionPhasorGroup.PhaseA.Measurement.AngleKey);
                        inputMeasurementKeys.Add(currentInjectionPhasorGroup.PhaseB.Measurement.MagnitudeKey);
                        inputMeasurementKeys.Add(currentInjectionPhasorGroup.PhaseB.Measurement.AngleKey);
                        inputMeasurementKeys.Add(currentInjectionPhasorGroup.PhaseC.Measurement.MagnitudeKey);
                        inputMeasurementKeys.Add(currentInjectionPhasorGroup.PhaseC.Measurement.AngleKey);
                    }
                }

                if (m_inputOutputSettings.AcceptsEstimates)
                {
                    inputMeasurementKeys.Add(currentInjectionPhasorGroup.PositiveSequence.Estimate.MagnitudeKey);
                    inputMeasurementKeys.Add(currentInjectionPhasorGroup.PositiveSequence.Estimate.AngleKey);

                    if (m_phaseSelection == PhaseSelection.ThreePhase)
                    {
                        inputMeasurementKeys.Add(currentInjectionPhasorGroup.PhaseA.Estimate.MagnitudeKey);
                        inputMeasurementKeys.Add(currentInjectionPhasorGroup.PhaseA.Estimate.AngleKey);
                        inputMeasurementKeys.Add(currentInjectionPhasorGroup.PhaseB.Estimate.MagnitudeKey);
                        inputMeasurementKeys.Add(currentInjectionPhasorGroup.PhaseB.Estimate.AngleKey);
                        inputMeasurementKeys.Add(currentInjectionPhasorGroup.PhaseC.Estimate.MagnitudeKey);
                        inputMeasurementKeys.Add(currentInjectionPhasorGroup.PhaseC.Estimate.AngleKey);
                    }
                }
            }

            return inputMeasurementKeys;
        }

        private List<string> GetBreakerStatusInputKeys()
        {
            List<string> inputMeasurementKeys = new List<string>();

            foreach (BreakerStatus breakerStatus in m_breakerStatuses)
            {
                inputMeasurementKeys.Add(breakerStatus.Key);
            }

            return inputMeasurementKeys;
        }

        private List<string> GetStatusWordInputKeys()
        {
            List<string> inputMeasurementKeys = new List<string>();

            foreach (StatusWord statusWord in m_statusWords)
            {
                inputMeasurementKeys.Add(statusWord.Key);
            }

            return inputMeasurementKeys;
        }

        private List<string> GetTransformerTapInputKeys()
        {
            List<string> inputMeasurementKeys = new List<string>();

            foreach (Transformer transformer in m_transformers)
            {
                inputMeasurementKeys.Add(transformer.TapPositionInputKey);
            }

            return inputMeasurementKeys;
        }

        /// <summary>
        /// Determines the <see cref="ObservationState"/> of each of the <see cref="Node"/> objects in the <see cref="NetworkModel"/>
        /// THIS IS DESIGNED FOR ONLY ONE SUBSTATIONS OBSERVED BUSES AT A TIME. NEEDS TO BE REFACTORED?RENAMED TO EXPRESS THIS
        /// </summary>
        /// <param name="observedBusses">The set of <see cref="ObservedBus"/> objects to check the observability of.</param>
        /// <returns>The set of <see cref="ObservedBus"/> objects passed to function updated with the present <see cref="ObservationState"/>.</returns>
        private List<ObservedBus> CheckObservability(List<ObservedBus> observedBusses)
        {
            int totalNumberOfDirectlyObservedNodes = 0;
            int numberOfIndirectlyObservedNodes = 0;

            // Mark the nodes that are directly observed by a voltage phasor
            foreach (ObservedBus observedBus in observedBusses)
            {
                if (m_phaseSelection == PhaseSelection.PositiveSequence)
                {
                    foreach (Node node in observedBus.Nodes)
                    {
                        if (node.Voltage.IncludeInPositiveSequenceEstimator)
                        {
                            node.Observability = ObservationState.DirectlyObserved;
                            totalNumberOfDirectlyObservedNodes++;
                        }
                        else
                        {
                            node.Observability = ObservationState.Unobserved;
                        }
                    }
                }
                else if (m_phaseSelection == PhaseSelection.ThreePhase)
                {
                    foreach (Node node in observedBus.Nodes)
                    {
                        if (node.Voltage.IncludeInEstimator)
                        {
                            node.Observability = ObservationState.DirectlyObserved;
                            totalNumberOfDirectlyObservedNodes++;
                        }
                        else
                        {
                            node.Observability = ObservationState.Unobserved;
                        }
                    }
                }
            }

            // Mark the remaining nodes in the coherent group based on if they are observed with at least one current phasor
            foreach (ObservedBus observedBus in observedBusses)
            {
                foreach (Node node in observedBus.Nodes)
                {
                    if (node.Observability == ObservationState.Unobserved)
                    {
                        foreach (CurrentFlowPhasorGroup currentPhasorGroup in m_activeCurrentFlows)
                        {
                            if (currentPhasorGroup.MeasuredToNodeID == node.InternalID)
                            {
                                node.Observability = ObservationState.IndirectlyObserved;
                                numberOfIndirectlyObservedNodes++;
                            }
                        }
                    }
                }
            }


            List<ObservedBus> unobservedBuses = new List<ObservedBus>();

            // Mark the remaining nodes in the coherent group based on if their is at least one voltage phasor in on the bus
            foreach (ObservedBus observedBus in observedBusses)
            {
                int observedNodeCount = 0;

                foreach (Node node in observedBus.Nodes)
                {
                    if (node.Observability != ObservationState.Unobserved)
                    {
                        observedNodeCount++;
                    }
                }
                foreach (Node node in observedBus.Nodes)
                {
                    if (observedNodeCount == 0)
                    {
                        node.Observability = ObservationState.Unobserved;
                        if (!unobservedBuses.Contains(observedBus))
                        {
                            unobservedBuses.Add(observedBus);
                        }
                    }
                    else if (node.Observability == ObservationState.Unobserved)
                    {
                        node.Observability = ObservationState.IndirectlyObserved;
                    }
                }
                //foreach (Node node in observedBus.Nodes)
                //{
                //    if (totalNumberOfDirectlyObservedNodes > 0 || numberOfIndirectlyObservedNodes > 0)
                //    {
                //        if (node.Observability != ObservationState.DirectlyObserved && node.Observability != ObservationState.IndirectlyObserved)
                //        {
                //            node.Observability = ObservationState.IndirectlyObserved;
                //            numberOfIndirectlyObservedNodes++;
                //        }
                //    }
                //}
            }


            foreach (ObservedBus unobservedBus in unobservedBuses)
            {
                observedBusses.Remove(unobservedBus);
            }

            if (totalNumberOfDirectlyObservedNodes == 0 && numberOfIndirectlyObservedNodes == 0)
            {
                return null;
            }
            else
            {
                return observedBusses;
            }
        }

        private void MarkDirectObservability(List<ObservedBus> observedBuses)
        {
            // Mark the nodes that are directly observed by a voltage phasor
            foreach (ObservedBus observedBus in observedBuses)
            {
                if (m_phaseSelection == PhaseSelection.PositiveSequence)
                {
                    foreach (Node node in observedBus.Nodes)
                    {
                        if (node.Voltage.IncludeInPositiveSequenceEstimator)
                        {
                            node.Observability = ObservationState.DirectlyObserved;
                        }
                        else
                        {
                            node.Observability = ObservationState.Unobserved;
                        }
                    }
                }
                else if (m_phaseSelection == PhaseSelection.ThreePhase)
                {
                    foreach (Node node in observedBus.Nodes)
                    {
                        if (node.Voltage.IncludeInEstimator)
                        {
                            node.Observability = ObservationState.DirectlyObserved;
                        }
                        else
                        {
                            node.Observability = ObservationState.Unobserved;
                        }
                    }
                }
            }
        }

        private void MarkIndirectObservabilityByVoltage(List<ObservedBus> observedBuses)
        {
            foreach (ObservedBus observedBus in observedBuses)
            {
                bool busHasDirectObservation = false;
                foreach (Node node in observedBus.Nodes)
                {
                    if (node.Observability == ObservationState.DirectlyObserved)
                    {
                        busHasDirectObservation = true;
                    }
                }
                if (busHasDirectObservation)
                {
                    foreach (Node node in observedBus.Nodes)
                    {
                        if (node.Observability != ObservationState.DirectlyObserved)
                        {
                            node.Observability = ObservationState.IndirectlyObserved;
                        }
                    }
                }
            }
        }

        private void MarkIndirectObservabilityByCurrentFlow(List<ObservedBus> observedBuses)
        {
            foreach (ObservedBus observedBus in observedBuses)
            {
                foreach (Node node in observedBus.Nodes)
                {
                    if (node.Observability == ObservationState.Unobserved)
                    {
                        foreach (CurrentFlowPhasorGroup current in m_activeCurrentFlows)
                        {
                            if (node == current.MeasuredToNode && current.MeasuredFromNode.Observability != ObservationState.Unobserved)
                            {
                                node.Observability = ObservationState.IndirectlyObserved;
                            }
                        }
                    }
                }
            }
            foreach (ObservedBus observedBus in observedBuses)
            {
                bool busIsObserved = false;
                foreach (Node node in observedBus.Nodes)
                {
                    if (node.Observability != ObservationState.Unobserved)
                    {
                        busIsObserved = true;
                    }
                }
                if (busIsObserved)
                {
                    foreach (Node node in observedBus.Nodes)
                    {
                        if (node.Observability != ObservationState.DirectlyObserved)
                        {
                            node.Observability = ObservationState.IndirectlyObserved;
                        }
                    }
                }
            }
        }

        private void MarkDirectObservabilityByCurrentInjection(List<ObservedBus> observedBuses)
        {
            foreach (ObservedBus observedBus in observedBuses)
            {
                foreach (Node node in observedBus.Nodes)
                {
                    if (node.Observability != ObservationState.DirectlyObserved)
                    {
                        foreach (CurrentInjectionPhasorGroup current in m_activeCurrentInjections)
                        {
                            if (node == current.MeasuredConnectedNode)
                            {
                                node.Observability = ObservationState.DirectlyObserved;
                            }
                        }
                    }
                }
            }
        }

        private void RemoveUnobservedBuses(List<ObservedBus> observedBuses)
        {
            List<ObservedBus> unobservedBuses = new List<ObservedBus>();
            foreach (ObservedBus observedBus in observedBuses)
            {
                List<Node> unobservedNodes = observedBus.Nodes.FindAll(x => x.Observability == ObservationState.Unobserved);
                if (unobservedNodes.Count > 0)
                {
                    unobservedBuses.Add(observedBus);
                }
            }

            foreach (ObservedBus unobservedBus in unobservedBuses)
            {
                observedBuses.Remove(unobservedBus);
            }
        }

        public List<ObservedBus> CheckPotentialObservability(List<ObservedBus> observedBuses)
        {
            int numberOfDirectlyObservedNodes = 0;
            int numberOfIndirectlyObservedNodes = 0;

            // Mark the nodes that are directly observed by a voltage phasor
            foreach (ObservedBus observedBus in observedBuses)
            {
                if (m_phaseSelection == PhaseSelection.PositiveSequence)
                {
                    foreach (Node node in observedBus.Nodes)
                    {
                        if (node.Voltage.ExpectsPositiveSequenceMeasurements)
                        {
                            node.Observability = ObservationState.DirectlyObserved;
                            numberOfDirectlyObservedNodes++;
                        }
                        else
                        {
                            node.Observability = ObservationState.Unobserved;
                        }
                    }
                }
                else if (m_phaseSelection == PhaseSelection.ThreePhase)
                {
                    foreach (Node node in observedBus.Nodes)
                    {
                        if (node.Voltage.ExpectsMeasurements)
                        {
                            node.Observability = ObservationState.DirectlyObserved;
                            numberOfDirectlyObservedNodes++;
                        }
                        else
                        {
                            node.Observability = ObservationState.Unobserved;
                        }
                    }
                }
            }

            // Mark the remaining nodes in the coherent group based on if they are observed with at least one current phasor
            foreach (ObservedBus observedBus in observedBuses)
            {
                foreach (Node node in observedBus.Nodes)
                {
                    if (node.Observability == ObservationState.Unobserved)
                    {
                        foreach (CurrentFlowPhasorGroup currentPhasorGroup in m_potentiallyActiveCurrentFlows)
                        {
                            if (currentPhasorGroup.MeasuredToNodeID == node.InternalID)
                            {
                                node.Observability = ObservationState.IndirectlyObserved;
                                numberOfIndirectlyObservedNodes++;
                            }
                        }
                    }
                }
            }

            // Mark the remaining nodes in the coherent group based on if their is at least one voltage phasor in on the bus
            foreach (ObservedBus observedBus in observedBuses)
            {
                foreach (Node node in observedBus.Nodes)
                {
                    if (numberOfDirectlyObservedNodes > 0 || numberOfIndirectlyObservedNodes > 0)
                    {
                        if (node.Observability != ObservationState.DirectlyObserved && node.Observability != ObservationState.IndirectlyObserved)
                        {
                            node.Observability = ObservationState.IndirectlyObserved;
                            numberOfIndirectlyObservedNodes++;
                        }
                    }
                }
            }

            if (numberOfDirectlyObservedNodes == 0 && numberOfIndirectlyObservedNodes == 0)
            {
                return null;
            }
            else
            {
                return observedBuses;
            }
        }

        /// <summary>
        /// Establishes the necessesary references for complex power calculations to occurr.
        /// </summary>
        private void InitializeComplexPowerCalculations()
        {
            foreach (Transformer transformer in m_transformers)
            {
                transformer.InitializeComplexPower();
            }
            foreach (TransmissionLine transmissionLine in m_transmissionLines)
            {
                transmissionLine.InitializeComplexPower();
            }
        }

        public void InitializeSubstationVoltageLevelGroups()
        {
            foreach (Substation substation in m_substations)
            {
                substation.InitializeVoltageLevelGroups();
            }
        }

        /// <summary>
        /// Takes the <see cref="Node.Voltage"/> from all of the <see cref="Node"/> objects in the model
        /// and collects them into one list to make updating values much easier.
        /// </summary>
        private void ListVoltagePhasors()
        {
            foreach (Company company in m_companies)
            {
                foreach (Division division in company.Divisions)
                {
                    foreach (Substation substation in division.Substations)
                    {
                        foreach (Node node in substation.Nodes)
                        {
                            m_voltages.Add(node.Voltage);
                        }
                    }
                    foreach (TransmissionLine transmissionLine in division.TransmissionLines)
                    {
                        foreach (Node node in transmissionLine.Nodes)
                        {
                            if (!m_voltages.Contains(node.Voltage))
                            {
                                m_voltages.Add(node.Voltage);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Takes the <see cref="Transformer.FromNodeCurrent"/>, <see cref="Transformer.ToNodeCurrent"/>,
        /// <see cref="TransmissionLine.FromSubstationCurrent"/>, and <see cref="TransmissionLine.ToSubstationCurrent"/>
        /// from all of the <see cref="Transformer"/> and <see cref="TransmissionLine"/> objects in the model
        /// and collects them into one list to make updating values much easier.
        /// </summary>
        private void ListCurrentPhasors()
        {
            foreach (Company company in m_companies)
            {
                foreach (Division division in company.Divisions)
                {
                    foreach (Substation substation in division.Substations)
                    {
                        foreach (Transformer transformer in substation.Transformers)
                        {
                            // Add the current phasor groups to the list
                            m_currentFlows.Add(transformer.FromNodeCurrent);
                            m_currentFlows.Add(transformer.ToNodeCurrent);

                            // Set the measured branch of each current phasor group
                            transformer.FromNodeCurrent.MeasuredBranch = transformer;
                            transformer.ToNodeCurrent.MeasuredBranch = transformer;
                        }
                        foreach (ShuntCompensator shunt in substation.Shunts)
                        {
                            m_currentInjections.Add(shunt.Current);

                            shunt.Current.MeasuredBranch = shunt;
                        }
                    }
                    foreach (TransmissionLine transmissionLine in division.TransmissionLines)
                    {

                        // Add the current phasor groups to the list
                        m_currentFlows.Add(transmissionLine.FromSubstationCurrent);
                        m_currentFlows.Add(transmissionLine.ToSubstationCurrent);

                        // Set the measured branch of each current phasor group
                        transmissionLine.FromSubstationCurrent.MeasuredBranch = transmissionLine;
                        transmissionLine.ToSubstationCurrent.MeasuredBranch = transmissionLine;
                    }
                }
            }
        }

        /// <summary>
        /// Maps the raw voltage measurement values to their virtual counterpart
        /// </summary>
        private void InsertVoltageMeasurements()
        {
            foreach (VoltagePhasorGroup voltagePhasorGroup in ExpectedVoltages)
            {
                double value = 0;

                if (m_inputOutputSettings.AcceptsMeasurements)
                {
                    //                                    Positive Sequence Magnitude Measurement
                    if (m_rawMeasurementKeyValuePairs.TryGetValue(voltagePhasorGroup.PositiveSequence.Measurement.MagnitudeKey, out value))
                    {
                        voltagePhasorGroup.PositiveSequence.Measurement.Magnitude = value;
                        value = 0;
                    }

                    //                                    Positive Sequence Angle Measurement
                    if (m_rawMeasurementKeyValuePairs.TryGetValue(voltagePhasorGroup.PositiveSequence.Measurement.AngleKey, out value))
                    {
                        voltagePhasorGroup.PositiveSequence.Measurement.AngleInDegrees = value;
                        value = 0;
                    }

                    if (m_phaseSelection == PhaseSelection.ThreePhase)
                    {
                        //                                    Phase A Magnitude Measurement
                        if (m_rawMeasurementKeyValuePairs.TryGetValue(voltagePhasorGroup.PhaseA.Measurement.MagnitudeKey, out value))
                        {
                            voltagePhasorGroup.PhaseA.Measurement.Magnitude = value;
                            value = 0;
                        }

                        //                                    Phase A Angle Measurement
                        if (m_rawMeasurementKeyValuePairs.TryGetValue(voltagePhasorGroup.PhaseA.Measurement.AngleKey, out value))
                        {
                            voltagePhasorGroup.PhaseA.Measurement.AngleInDegrees = value;
                            value = 0;
                        }

                        //                                    Phase B Magnitude Measurement
                        if (m_rawMeasurementKeyValuePairs.TryGetValue(voltagePhasorGroup.PhaseB.Measurement.MagnitudeKey, out value))
                        {
                            voltagePhasorGroup.PhaseB.Measurement.Magnitude = value;
                            value = 0;
                        }

                        //                                    Phase B Angle Measurement
                        if (m_rawMeasurementKeyValuePairs.TryGetValue(voltagePhasorGroup.PhaseB.Measurement.AngleKey, out value))
                        {
                            voltagePhasorGroup.PhaseB.Measurement.AngleInDegrees = value;
                            value = 0;
                        }

                        //                                    Phase C Magnitude Measurement
                        if (m_rawMeasurementKeyValuePairs.TryGetValue(voltagePhasorGroup.PhaseC.Measurement.MagnitudeKey, out value))
                        {
                            voltagePhasorGroup.PhaseC.Measurement.Magnitude = value;
                            value = 0;
                        }

                        //                                    Phase C Angle Measurement
                        if (m_rawMeasurementKeyValuePairs.TryGetValue(voltagePhasorGroup.PhaseC.Measurement.AngleKey, out value))
                        {
                            voltagePhasorGroup.PhaseC.Measurement.AngleInDegrees = value;
                            value = 0;
                        }
                    }
                }

                if (m_inputOutputSettings.AcceptsEstimates)
                {
                    //                                    Positive Sequence Magnitude Estimate
                    if (m_rawMeasurementKeyValuePairs.TryGetValue(voltagePhasorGroup.PositiveSequence.Estimate.MagnitudeKey, out value))
                    {
                        voltagePhasorGroup.PositiveSequence.Estimate.Magnitude = value;
                        value = 0;
                    }

                    //                                    Positive Sequence Angle Estimate
                    if (m_rawMeasurementKeyValuePairs.TryGetValue(voltagePhasorGroup.PositiveSequence.Estimate.AngleKey, out value))
                    {
                        voltagePhasorGroup.PositiveSequence.Estimate.AngleInDegrees = value;
                        value = 0;
                    }

                    if (m_phaseSelection == PhaseSelection.ThreePhase)
                    {
                        //                                    Phase A Magnitude Estimate
                        if (m_rawMeasurementKeyValuePairs.TryGetValue(voltagePhasorGroup.PhaseA.Estimate.MagnitudeKey, out value))
                        {
                            voltagePhasorGroup.PhaseA.Estimate.Magnitude = value;
                            value = 0;
                        }


                        //                                    Phase A Angle Estimate
                        if (m_rawMeasurementKeyValuePairs.TryGetValue(voltagePhasorGroup.PhaseA.Estimate.AngleKey, out value))
                        {
                            voltagePhasorGroup.PhaseA.Estimate.AngleInDegrees = value;
                            value = 0;
                        }


                        //                                    Phase B Magnitude Estimate
                        if (m_rawMeasurementKeyValuePairs.TryGetValue(voltagePhasorGroup.PhaseB.Estimate.MagnitudeKey, out value))
                        {
                            voltagePhasorGroup.PhaseB.Estimate.Magnitude = value;
                            value = 0;
                        }



                        //                                    Phase B Angle Estimate
                        if (m_rawMeasurementKeyValuePairs.TryGetValue(voltagePhasorGroup.PhaseB.Estimate.AngleKey, out value))
                        {
                            voltagePhasorGroup.PhaseB.Estimate.AngleInDegrees = value;
                            value = 0;
                        }


                        //                                    Phase C Magnitude Estimate
                        if (m_rawMeasurementKeyValuePairs.TryGetValue(voltagePhasorGroup.PhaseC.Estimate.MagnitudeKey, out value))
                        {
                            voltagePhasorGroup.PhaseC.Estimate.Magnitude = value;
                            value = 0;
                        }


                        //                                    Phase C Angle Estimate
                        if (m_rawMeasurementKeyValuePairs.TryGetValue(voltagePhasorGroup.PhaseC.Estimate.AngleKey, out value))
                        {
                            voltagePhasorGroup.PhaseC.Estimate.AngleInDegrees = value;
                            value = 0;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Maps the raw current measurement values to their virtual counterpart
        /// </summary>
        private void InsertCurrentFlowMeasurements()
        {
            foreach (CurrentFlowPhasorGroup currentPhasorGroup in ExpectedCurrentFlows)
            {
                double value = 0;

                if (m_inputOutputSettings.AcceptsMeasurements)
                {
                    //                                    Positive Sequence Magnitude Measurement
                    if (m_rawMeasurementKeyValuePairs.TryGetValue(currentPhasorGroup.PositiveSequence.Measurement.MagnitudeKey, out value))
                    {
                        currentPhasorGroup.PositiveSequence.Measurement.Magnitude = value;
                        value = 0;
                    }

                    //                                    Positive Sequence Angle Measurement
                    if (m_rawMeasurementKeyValuePairs.TryGetValue(currentPhasorGroup.PositiveSequence.Measurement.AngleKey, out value))
                    {
                        currentPhasorGroup.PositiveSequence.Measurement.AngleInDegrees = value;
                        value = 0;
                    }

                    if (m_phaseSelection == PhaseSelection.ThreePhase)
                    {
                        //                                    Phase A Magnitude Measurement
                        if (m_rawMeasurementKeyValuePairs.TryGetValue(currentPhasorGroup.PhaseA.Measurement.MagnitudeKey, out value))
                        {
                            currentPhasorGroup.PhaseA.Measurement.Magnitude = value;
                            value = 0;
                        }

                        //                                    Phase A Angle Measurement
                        if (m_rawMeasurementKeyValuePairs.TryGetValue(currentPhasorGroup.PhaseA.Measurement.AngleKey, out value))
                        {
                            currentPhasorGroup.PhaseA.Measurement.AngleInDegrees = value;
                            value = 0;
                        }

                        //                                    Phase B Magnitude Measurement
                        if (m_rawMeasurementKeyValuePairs.TryGetValue(currentPhasorGroup.PhaseB.Measurement.MagnitudeKey, out value))
                        {
                            currentPhasorGroup.PhaseB.Measurement.Magnitude = value;
                            value = 0;
                        }

                        //                                    Phase B Angle Measurement
                        if (m_rawMeasurementKeyValuePairs.TryGetValue(currentPhasorGroup.PhaseB.Measurement.AngleKey, out value))
                        {
                            currentPhasorGroup.PhaseB.Measurement.AngleInDegrees = value;
                            value = 0;
                        }

                        //                                    Phase C Magnitude Measurement
                        if (m_rawMeasurementKeyValuePairs.TryGetValue(currentPhasorGroup.PhaseC.Measurement.MagnitudeKey, out value))
                        {
                            currentPhasorGroup.PhaseC.Measurement.Magnitude = value;
                            value = 0;
                        }

                        //                                    Phase C Angle Measurement
                        if (m_rawMeasurementKeyValuePairs.TryGetValue(currentPhasorGroup.PhaseC.Measurement.AngleKey, out value))
                        {
                            currentPhasorGroup.PhaseC.Measurement.AngleInDegrees = value;
                            value = 0;
                        }
                    }
                }


                if (m_inputOutputSettings.AcceptsEstimates)
                {
                //                                    Positive Sequence Magnitude Estimate
                    if (m_rawMeasurementKeyValuePairs.TryGetValue(currentPhasorGroup.PositiveSequence.Estimate.MagnitudeKey, out value))
                    {
                        currentPhasorGroup.PositiveSequence.Estimate.Magnitude = value;
                        value = 0;
                    }

                    //                                    Positive Sequence Angle Estimate
                    if (m_rawMeasurementKeyValuePairs.TryGetValue(currentPhasorGroup.PositiveSequence.Estimate.AngleKey, out value))
                    {
                        currentPhasorGroup.PositiveSequence.Estimate.AngleInDegrees = value;
                        value = 0;
                    }


                    if (m_phaseSelection == PhaseSelection.ThreePhase)
                    {
                        //                                    Phase A Magnitude Estimate
                        if (m_rawMeasurementKeyValuePairs.TryGetValue(currentPhasorGroup.PhaseA.Estimate.MagnitudeKey, out value))
                        {
                            currentPhasorGroup.PhaseA.Estimate.Magnitude = value;
                            value = 0;
                        }

                        //                                    Phase A Angle Estimate
                        if (m_rawMeasurementKeyValuePairs.TryGetValue(currentPhasorGroup.PhaseA.Estimate.AngleKey, out value))
                        {
                            currentPhasorGroup.PhaseA.Estimate.AngleInDegrees = value;
                            value = 0;
                        }

                        //                                    Phase B Magnitude Estimate
                        if (m_rawMeasurementKeyValuePairs.TryGetValue(currentPhasorGroup.PhaseB.Estimate.MagnitudeKey, out value))
                        {
                            currentPhasorGroup.PhaseB.Estimate.Magnitude = value;
                            value = 0;
                        }

                        //                                    Phase B Angle Estimate
                        if (m_rawMeasurementKeyValuePairs.TryGetValue(currentPhasorGroup.PhaseB.Estimate.AngleKey, out value))
                        {
                            currentPhasorGroup.PhaseB.Estimate.AngleInDegrees = value;
                            value = 0;
                        }

                        //                                    Phase C Magnitude Estimate
                        if (m_rawMeasurementKeyValuePairs.TryGetValue(currentPhasorGroup.PhaseC.Estimate.MagnitudeKey, out value))
                        {
                            currentPhasorGroup.PhaseC.Estimate.Magnitude = value;
                            value = 0;
                        }

                        //                                    Phase C Angle Estimate
                        if (m_rawMeasurementKeyValuePairs.TryGetValue(currentPhasorGroup.PhaseC.Estimate.AngleKey, out value))
                        {
                            currentPhasorGroup.PhaseC.Estimate.AngleInDegrees = value;
                            value = 0;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Maps the raw current measurement values to their virtual counterpart
        /// </summary>
        private void InsertCurrentInjectionMeasurements()
        {
            foreach (CurrentInjectionPhasorGroup currentPhasorGroup in ExpectedCurrentInjections)
            {
                double value = 0;

                if (m_inputOutputSettings.AcceptsMeasurements)
                {
                    //                                    Positive Sequence Magnitude Measurement
                    if (m_rawMeasurementKeyValuePairs.TryGetValue(currentPhasorGroup.PositiveSequence.Measurement.MagnitudeKey, out value))
                    {
                        currentPhasorGroup.PositiveSequence.Measurement.Magnitude = value;
                        value = 0;
                    }

                    //                                    Positive Sequence Angle Measurement
                    if (m_rawMeasurementKeyValuePairs.TryGetValue(currentPhasorGroup.PositiveSequence.Measurement.AngleKey, out value))
                    {
                        currentPhasorGroup.PositiveSequence.Measurement.AngleInDegrees = value;
                        value = 0;
                    }

                    if (m_phaseSelection == PhaseSelection.ThreePhase)
                    {
                        //                                    Phase A Magnitude Measurement
                        if (m_rawMeasurementKeyValuePairs.TryGetValue(currentPhasorGroup.PhaseA.Measurement.MagnitudeKey, out value))
                        {
                            currentPhasorGroup.PhaseA.Measurement.Magnitude = value;
                            value = 0;
                        }

                        //                                    Phase A Angle Measurement
                        if (m_rawMeasurementKeyValuePairs.TryGetValue(currentPhasorGroup.PhaseA.Measurement.AngleKey, out value))
                        {
                            currentPhasorGroup.PhaseA.Measurement.AngleInDegrees = value;
                            value = 0;
                        }

                        //                                    Phase B Magnitude Measurement
                        if (m_rawMeasurementKeyValuePairs.TryGetValue(currentPhasorGroup.PhaseB.Measurement.MagnitudeKey, out value))
                        {
                            currentPhasorGroup.PhaseB.Measurement.Magnitude = value;
                            value = 0;
                        }

                        //                                    Phase B Angle Measurement
                        if (m_rawMeasurementKeyValuePairs.TryGetValue(currentPhasorGroup.PhaseB.Measurement.AngleKey, out value))
                        {
                            currentPhasorGroup.PhaseB.Measurement.AngleInDegrees = value;
                            value = 0;
                        }

                        //                                    Phase C Magnitude Measurement
                        if (m_rawMeasurementKeyValuePairs.TryGetValue(currentPhasorGroup.PhaseC.Measurement.MagnitudeKey, out value))
                        {
                            currentPhasorGroup.PhaseC.Measurement.Magnitude = value;
                            value = 0;
                        }

                        //                                    Phase C Angle Measurement
                        if (m_rawMeasurementKeyValuePairs.TryGetValue(currentPhasorGroup.PhaseC.Measurement.AngleKey, out value))
                        {
                            currentPhasorGroup.PhaseC.Measurement.AngleInDegrees = value;
                            value = 0;
                        }
                    }
                }


                if (m_inputOutputSettings.AcceptsEstimates)
                {
                    //                                    Positive Sequence Magnitude Estimate
                    if (m_rawMeasurementKeyValuePairs.TryGetValue(currentPhasorGroup.PositiveSequence.Estimate.MagnitudeKey, out value))
                    {
                        currentPhasorGroup.PositiveSequence.Estimate.Magnitude = value;
                        value = 0;
                    }

                    //                                    Positive Sequence Angle Estimate
                    if (m_rawMeasurementKeyValuePairs.TryGetValue(currentPhasorGroup.PositiveSequence.Estimate.AngleKey, out value))
                    {
                        currentPhasorGroup.PositiveSequence.Estimate.AngleInDegrees = value;
                        value = 0;
                    }


                    if (m_phaseSelection == PhaseSelection.ThreePhase)
                    {
                        //                                    Phase A Magnitude Estimate
                        if (m_rawMeasurementKeyValuePairs.TryGetValue(currentPhasorGroup.PhaseA.Estimate.MagnitudeKey, out value))
                        {
                            currentPhasorGroup.PhaseA.Estimate.Magnitude = value;
                            value = 0;
                        }

                        //                                    Phase A Angle Estimate
                        if (m_rawMeasurementKeyValuePairs.TryGetValue(currentPhasorGroup.PhaseA.Estimate.AngleKey, out value))
                        {
                            currentPhasorGroup.PhaseA.Estimate.AngleInDegrees = value;
                            value = 0;
                        }

                        //                                    Phase B Magnitude Estimate
                        if (m_rawMeasurementKeyValuePairs.TryGetValue(currentPhasorGroup.PhaseB.Estimate.MagnitudeKey, out value))
                        {
                            currentPhasorGroup.PhaseB.Estimate.Magnitude = value;
                            value = 0;
                        }

                        //                                    Phase B Angle Estimate
                        if (m_rawMeasurementKeyValuePairs.TryGetValue(currentPhasorGroup.PhaseB.Estimate.AngleKey, out value))
                        {
                            currentPhasorGroup.PhaseB.Estimate.AngleInDegrees = value;
                            value = 0;
                        }

                        //                                    Phase C Magnitude Estimate
                        if (m_rawMeasurementKeyValuePairs.TryGetValue(currentPhasorGroup.PhaseC.Estimate.MagnitudeKey, out value))
                        {
                            currentPhasorGroup.PhaseC.Estimate.Magnitude = value;
                            value = 0;
                        }

                        //                                    Phase C Angle Estimate
                        if (m_rawMeasurementKeyValuePairs.TryGetValue(currentPhasorGroup.PhaseC.Estimate.AngleKey, out value))
                        {
                            currentPhasorGroup.PhaseC.Estimate.AngleInDegrees = value;
                            value = 0;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Maps the raw digital values to their virtual counterpart
        /// </summary>
        private void InsertBreakerStatuses()
        {
            foreach (BreakerStatus breakerStatus in ExpectedBreakerStatuses)
            {
                double value = 0;
                // Breaker Statuses
                if (m_rawMeasurementKeyValuePairs.TryGetValue(breakerStatus.Key, out value))
                {
                    breakerStatus.BinaryValue = Convert.ToInt32(value);
                    value = 0;
                }
            }
        }

        /// <summary>
        /// Maps the raw status word values to their virtual counterpart
        /// </summary>
        private void InsertStatusWords()
        {
            foreach (StatusWord statusWord in StatusWords)
            {
                double value = 0;

                if (m_rawMeasurementKeyValuePairs.TryGetValue(statusWord.Key, out value))
                {
                    statusWord.BinaryValue = value;
                    value = 0;
                }
            }
        }

        /// <summary>
        /// Maps the raw tap position values to their virtual counterpart
        /// </summary>
        private void InsertTransformerTapPositions()
        {
            foreach (Transformer transformer in m_transformers)
            {
                double value = 0;

                if (m_rawMeasurementKeyValuePairs.TryGetValue(transformer.TapPositionInputKey, out value))
                {
                    transformer.TapPositionMeasurement = Convert.ToInt32(value);
                }
            }
        }

        private List<OutputMeasurement> GetVoltageEstimatesOutput()
        {
            List<OutputMeasurement> output = new List<OutputMeasurement>();

            foreach (VoltagePhasorGroup voltage in m_voltages)
            {
                output.Add(new OutputMeasurement()
                {
                    InternalId = voltage.InternalID,
                    SubstationName = voltage.MeasuredNode.ParentSubstation.Name,
                    MeasuredDeviceType = MeasuredDeviceType.Node,
                    OutputType = OutputType.VoltageMagnitudeEstimate,
                    DeviceId = voltage.MeasuredNode.Name,
                    DeviceSuffix = voltage.MeasuredNode.ParentSubstation.Name,
                    Key = voltage.PositiveSequence.Estimate.MagnitudeKey,
                    Value = voltage.PositiveSequence.Estimate.Magnitude,
                    Description = $"{voltage.Description} Positive Sequence Magnitude"
                });
                output.Add(new OutputMeasurement()
                {
                    InternalId = voltage.InternalID,
                    SubstationName = voltage.MeasuredNode.ParentSubstation.Name,
                    MeasuredDeviceType = MeasuredDeviceType.Node,
                    OutputType = OutputType.VoltageAngleEstimate,
                    DeviceId = voltage.MeasuredNode.Name,
                    DeviceSuffix = voltage.MeasuredNode.ParentSubstation.Name,
                    Key = voltage.PositiveSequence.Estimate.AngleKey,
                    Value = voltage.PositiveSequence.Estimate.AngleInDegrees,
                    Description = $"{voltage.Description} Positive Sequence Angle In Degrees"
                });
            }

            return output;
        }

        /// <summary>
        /// Adds the estimated voltage phasor components to the output with their associated keys.
        /// </summary>
        /// <param name="output">The Dictionary that contains the output keys and values.</param>
        private void AddVoltageEstimatesToOutput(Dictionary<string, double> output)
        {
            foreach (VoltagePhasorGroup voltagePhasorGroup in m_voltages)
            {
                if (!output.ContainsKey(voltagePhasorGroup.NegativeSequenceToPositiveSequenceRatioMeasurementKey))
                {
                    output.Add(voltagePhasorGroup.NegativeSequenceToPositiveSequenceRatioMeasurementKey, voltagePhasorGroup.EstimatedNegativeSequenceToPositiveSequenceRatio);
                }

                if (!output.ContainsKey(voltagePhasorGroup.PositiveSequence.Estimate.MagnitudeKey))
                {
                    output.Add(voltagePhasorGroup.PositiveSequence.Estimate.MagnitudeKey, voltagePhasorGroup.PositiveSequence.Estimate.Magnitude);
                }
                if (!output.ContainsKey(voltagePhasorGroup.PositiveSequence.Estimate.AngleKey))
                {
                    output.Add(voltagePhasorGroup.PositiveSequence.Estimate.AngleKey, voltagePhasorGroup.PositiveSequence.Estimate.AngleInDegrees);
                }

                if (m_phaseSelection == PhaseSelection.ThreePhase)
                {
                    if (!output.ContainsKey(voltagePhasorGroup.PhaseA.Estimate.MagnitudeKey))
                    {
                        output.Add(voltagePhasorGroup.PhaseA.Estimate.MagnitudeKey, voltagePhasorGroup.PhaseA.Estimate.Magnitude);
                    }
                    if (!output.ContainsKey(voltagePhasorGroup.PhaseA.Estimate.AngleKey))
                    {
                        output.Add(voltagePhasorGroup.PhaseA.Estimate.AngleKey, voltagePhasorGroup.PhaseA.Estimate.AngleInDegrees);
                    }

                    if (!output.ContainsKey(voltagePhasorGroup.PhaseB.Estimate.MagnitudeKey))
                    {
                        output.Add(voltagePhasorGroup.PhaseB.Estimate.MagnitudeKey, voltagePhasorGroup.PhaseB.Estimate.Magnitude);
                    }
                    if (!output.ContainsKey(voltagePhasorGroup.PhaseB.Estimate.AngleKey))
                    {
                        output.Add(voltagePhasorGroup.PhaseB.Estimate.AngleKey, voltagePhasorGroup.PhaseB.Estimate.AngleInDegrees);
                    }

                    if (!output.ContainsKey(voltagePhasorGroup.PhaseC.Estimate.MagnitudeKey))
                    {
                        output.Add(voltagePhasorGroup.PhaseC.Estimate.MagnitudeKey, voltagePhasorGroup.PhaseC.Estimate.Magnitude);
                    }
                    if (!output.ContainsKey(voltagePhasorGroup.PhaseC.Estimate.AngleKey))
                    {
                        output.Add(voltagePhasorGroup.PhaseC.Estimate.AngleKey, voltagePhasorGroup.PhaseC.Estimate.AngleInDegrees);
                    }
                }
            }
        }

        private List<OutputMeasurement> GetCurrentFlowEstimatesOutput()
        {
            List<OutputMeasurement> output = new List<OutputMeasurement>();

            foreach (CurrentFlowPhasorGroup current in m_currentFlows)
            {
                output.Add(new OutputMeasurement()
                {
                    InternalId = current.InternalID,
                    SubstationName = current.MeasuredFromNode.ParentSubstation.Name,
                    MeasuredDeviceType = MeasuredDeviceType.Line,
                    OutputType = OutputType.CurrentFlowMagnitudeEstimate,
                    DeviceId = (current.MeasuredBranch as INetworkDescribable).Name,
                    DeviceSuffix = current.MeasuredFromNode.ParentSubstation.Name,
                    Key = current.PositiveSequence.Estimate.MagnitudeKey,
                    Value = current.PositiveSequence.Estimate.Magnitude,
                    Description = $"{current.Description} Positive Sequence Magnitude"
                });
                output.Add(new OutputMeasurement()
                {
                    InternalId = current.InternalID,
                    SubstationName = current.MeasuredFromNode.ParentSubstation.Name,
                    MeasuredDeviceType = MeasuredDeviceType.Line,
                    OutputType = OutputType.CurrentFlowAngleEstimate,
                    DeviceId = (current.MeasuredBranch as INetworkDescribable).Name,
                    DeviceSuffix = current.MeasuredFromNode.ParentSubstation.Name,
                    Key = current.PositiveSequence.Estimate.AngleKey,
                    Value = current.PositiveSequence.Estimate.AngleInDegrees,
                    Description = $"{current.Description} Positive Sequence Angle In Degrees"
                });
            }

            return output;
        }

        /// <summary>
        /// Adds the estimated current flow phasor components to the output with their associated keys.
        /// </summary>
        /// <param name="output">The Dictionary that contains the output keys and values.</param>
        private void AddCurrentFlowEstimatesToOutput(Dictionary<string, double> output)
        {
            foreach (CurrentFlowPhasorGroup currentFlowPhasorGroup in m_currentFlows)
            {
                if (!output.ContainsKey(currentFlowPhasorGroup.NegativeSequenceToPositiveSequenceRatioMeasurementKey))
                {
                    output.Add(currentFlowPhasorGroup.NegativeSequenceToPositiveSequenceRatioMeasurementKey, currentFlowPhasorGroup.EstimatedNegativeSequenceToPositiveSequenceRatio);
                }

                if (!output.ContainsKey(currentFlowPhasorGroup.PositiveSequence.Estimate.MagnitudeKey))
                {
                    output.Add(currentFlowPhasorGroup.PositiveSequence.Estimate.MagnitudeKey, currentFlowPhasorGroup.PositiveSequence.Estimate.Magnitude);
                }
                if (!output.ContainsKey(currentFlowPhasorGroup.PositiveSequence.Estimate.AngleKey))
                {
                    output.Add(currentFlowPhasorGroup.PositiveSequence.Estimate.AngleKey, currentFlowPhasorGroup.PositiveSequence.Estimate.AngleInDegrees);
                }

                if (m_phaseSelection == PhaseSelection.ThreePhase)
                {
                    if (!output.ContainsKey(currentFlowPhasorGroup.PhaseA.Estimate.MagnitudeKey))
                    {
                        output.Add(currentFlowPhasorGroup.PhaseA.Estimate.MagnitudeKey, currentFlowPhasorGroup.PhaseA.Estimate.Magnitude);
                    }
                    if (!output.ContainsKey(currentFlowPhasorGroup.PhaseA.Estimate.AngleKey))
                    {
                        output.Add(currentFlowPhasorGroup.PhaseA.Estimate.AngleKey, currentFlowPhasorGroup.PhaseA.Estimate.AngleInDegrees);
                    }

                    if (!output.ContainsKey(currentFlowPhasorGroup.PhaseB.Estimate.MagnitudeKey))
                    {
                        output.Add(currentFlowPhasorGroup.PhaseB.Estimate.MagnitudeKey, currentFlowPhasorGroup.PhaseB.Estimate.Magnitude);
                    }
                    if (!output.ContainsKey(currentFlowPhasorGroup.PhaseB.Estimate.AngleKey))
                    {
                        output.Add(currentFlowPhasorGroup.PhaseB.Estimate.AngleKey, currentFlowPhasorGroup.PhaseB.Estimate.AngleInDegrees);
                    }

                    if (!output.ContainsKey(currentFlowPhasorGroup.PhaseC.Estimate.MagnitudeKey))
                    {
                        output.Add(currentFlowPhasorGroup.PhaseC.Estimate.MagnitudeKey, currentFlowPhasorGroup.PhaseC.Estimate.Magnitude);
                    }
                    if (!output.ContainsKey(currentFlowPhasorGroup.PhaseC.Estimate.AngleKey))
                    {
                        output.Add(currentFlowPhasorGroup.PhaseC.Estimate.AngleKey, currentFlowPhasorGroup.PhaseC.Estimate.AngleInDegrees);
                    }
                }
            }
        }

        private List<OutputMeasurement> GetCurrentInjectionEstimatesOutput()
        {
            List<OutputMeasurement> output = new List<OutputMeasurement>();

            foreach (CurrentInjectionPhasorGroup current in m_currentInjections)
            {
                output.Add(new OutputMeasurement()
                {
                    InternalId = current.InternalID,
                    SubstationName = current.MeasuredConnectedNode.ParentSubstation.Name,
                    MeasuredDeviceType = MeasuredDeviceType.Shunt,
                    OutputType = OutputType.CurrentInjectionMagnitudeEstimate,
                    DeviceId = (current.MeasuredBranch as INetworkDescribable).Name,
                    DeviceSuffix = current.MeasuredConnectedNode.ParentSubstation.Name,
                    Key = current.PositiveSequence.Estimate.MagnitudeKey,
                    Value = current.PositiveSequence.Estimate.Magnitude,
                    Description = $"{current.Description} Positive Sequence Magnitude"
                });
                output.Add(new OutputMeasurement()
                {
                    InternalId = current.InternalID,
                    SubstationName = current.MeasuredConnectedNode.ParentSubstation.Name,
                    MeasuredDeviceType = MeasuredDeviceType.Shunt,
                    OutputType = OutputType.CurrentInjectionAngleEstimate,
                    DeviceId = (current.MeasuredBranch as INetworkDescribable).Name,
                    DeviceSuffix = current.MeasuredConnectedNode.ParentSubstation.Name,
                    Key = current.PositiveSequence.Estimate.AngleKey,
                    Value = current.PositiveSequence.Estimate.AngleInDegrees,
                    Description = $"{current.Description} Positive Sequence Angle In Degrees"
                });
            }

            return output;
        }

        /// <summary>
        /// Adds the estimated current injection phasor components to the output with their associated keys.
        /// </summary>
        /// <param name="output">The Dictionary that contains the output keys and values.</param>
        private void AddCurrentInjectionsToOutput(Dictionary<string, double> output)
        {
            foreach (CurrentInjectionPhasorGroup currentInjectionPhasorGroup in m_currentInjections)
            {
                if (!output.ContainsKey(currentInjectionPhasorGroup.NegativeSequenceToPositiveSequenceRatioMeasurementKey))
                {
                    output.Add(currentInjectionPhasorGroup.NegativeSequenceToPositiveSequenceRatioMeasurementKey, currentInjectionPhasorGroup.EstimatedNegativeSequenceToPositiveSequenceRatio);
                }

                if (!output.ContainsKey(currentInjectionPhasorGroup.PositiveSequence.Estimate.MagnitudeKey))
                {
                    output.Add(currentInjectionPhasorGroup.PositiveSequence.Estimate.MagnitudeKey, currentInjectionPhasorGroup.PositiveSequence.Estimate.Magnitude);
                }
                if (!output.ContainsKey(currentInjectionPhasorGroup.PositiveSequence.Estimate.AngleKey))
                {
                    output.Add(currentInjectionPhasorGroup.PositiveSequence.Estimate.AngleKey, currentInjectionPhasorGroup.PositiveSequence.Estimate.AngleInDegrees);
                }

                if (m_phaseSelection == PhaseSelection.ThreePhase)
                {
                    if (!output.ContainsKey(currentInjectionPhasorGroup.PhaseA.Estimate.MagnitudeKey))
                    {
                        output.Add(currentInjectionPhasorGroup.PhaseA.Estimate.MagnitudeKey, currentInjectionPhasorGroup.PhaseA.Estimate.Magnitude);
                    }
                    if (!output.ContainsKey(currentInjectionPhasorGroup.PhaseA.Estimate.AngleKey))
                    {
                        output.Add(currentInjectionPhasorGroup.PhaseA.Estimate.AngleKey, currentInjectionPhasorGroup.PhaseA.Estimate.AngleInDegrees);
                    }

                    if (!output.ContainsKey(currentInjectionPhasorGroup.PhaseB.Estimate.MagnitudeKey))
                    {
                        output.Add(currentInjectionPhasorGroup.PhaseB.Estimate.MagnitudeKey, currentInjectionPhasorGroup.PhaseB.Estimate.Magnitude);
                    }
                    if (!output.ContainsKey(currentInjectionPhasorGroup.PhaseB.Estimate.AngleKey))
                    {
                        output.Add(currentInjectionPhasorGroup.PhaseB.Estimate.AngleKey, currentInjectionPhasorGroup.PhaseB.Estimate.AngleInDegrees);
                    }

                    if (!output.ContainsKey(currentInjectionPhasorGroup.PhaseC.Estimate.MagnitudeKey))
                    {
                        output.Add(currentInjectionPhasorGroup.PhaseC.Estimate.MagnitudeKey, currentInjectionPhasorGroup.PhaseC.Estimate.Magnitude);
                    }
                    if (!output.ContainsKey(currentInjectionPhasorGroup.PhaseC.Estimate.AngleKey))
                    {
                        output.Add(currentInjectionPhasorGroup.PhaseC.Estimate.AngleKey, currentInjectionPhasorGroup.PhaseC.Estimate.AngleInDegrees);
                    }
                }
            }
        }

        private List<OutputMeasurement> GetVoltageMeasurementResidualsOutput()
        {
            List<OutputMeasurement> output = new List<OutputMeasurement>();

            List<string> keys = new List<string>();

            foreach (VoltagePhasorGroup voltage in m_voltages)
            {
                if (!keys.Contains(voltage.PositiveSequence.MagnitudeResidualKey) &&
                    !keys.Contains(voltage.PositiveSequence.AngleResidualKey))
                {
                    keys.Add(voltage.PositiveSequence.MagnitudeResidualKey);
                    keys.Add(voltage.PositiveSequence.AngleResidualKey);

                    output.Add(new OutputMeasurement()
                    {
                        InternalId = voltage.InternalID,
                        SubstationName = voltage.MeasuredNode.ParentSubstation.Name,
                        MeasuredDeviceType = MeasuredDeviceType.Node,
                        OutputType = OutputType.VoltageMagnitudeResidual,
                        DeviceId = voltage.MeasuredNode.Name,
                        DeviceSuffix = voltage.MeasuredNode.ParentSubstation.Name,
                        Key = voltage.PositiveSequence.MagnitudeResidualKey,
                        Value = voltage.PositiveSequence.MagnitudeResidual,
                        Description = $"{voltage.Description} Positive Sequence Magnitude Residual"
                    });
                    output.Add(new OutputMeasurement()
                    {
                        InternalId = voltage.InternalID,
                        SubstationName = voltage.MeasuredNode.ParentSubstation.Name,
                        MeasuredDeviceType = MeasuredDeviceType.Node,
                        OutputType = OutputType.VoltageAngleEstimate,
                        DeviceId = voltage.MeasuredNode.Name,
                        DeviceSuffix = voltage.MeasuredNode.ParentSubstation.Name,
                        Key = voltage.PositiveSequence.AngleResidualKey,
                        Value = voltage.PositiveSequence.AngleResidualInDegrees,
                        Description = $"{voltage.Description} Positive Sequence Angle Residual In Degrees"
                    });
                }
            }

            return output;
        }

        /// <summary>
        /// Adds the voltage measurement residual components to the output with their associated keys.
        /// </summary>
        /// <param name="output">The Dictionary that contains the output keys and values.</param>
        private void AddVoltageMeasurementResidualsToOutput(Dictionary<string, double> output)
        {
            foreach (VoltagePhasorGroup voltagePhasorGroup in m_voltages)
            {
                if (!output.ContainsKey(voltagePhasorGroup.PositiveSequence.MagnitudeResidualKey))
                {
                    output.Add(voltagePhasorGroup.PositiveSequence.MagnitudeResidualKey, voltagePhasorGroup.PositiveSequence.MagnitudeResidual);
                }
                if (!output.ContainsKey(voltagePhasorGroup.PositiveSequence.AngleResidualKey))
                {
                    output.Add(voltagePhasorGroup.PositiveSequence.AngleResidualKey, voltagePhasorGroup.PositiveSequence.AngleResidualInDegrees);
                }

                if (m_phaseSelection == PhaseSelection.ThreePhase)
                {
                    if (!output.ContainsKey(voltagePhasorGroup.PhaseA.MagnitudeResidualKey))
                    {
                        output.Add(voltagePhasorGroup.PhaseA.MagnitudeResidualKey, voltagePhasorGroup.PhaseA.MagnitudeResidual);
                    }
                    if (!output.ContainsKey(voltagePhasorGroup.PhaseA.AngleResidualKey))
                    {
                        output.Add(voltagePhasorGroup.PhaseA.AngleResidualKey, voltagePhasorGroup.PhaseA.AngleResidualInDegrees);
                    }

                    if (!output.ContainsKey(voltagePhasorGroup.PhaseB.MagnitudeResidualKey))
                    {
                        output.Add(voltagePhasorGroup.PhaseB.MagnitudeResidualKey, voltagePhasorGroup.PhaseB.MagnitudeResidual);
                    }
                    if (!output.ContainsKey(voltagePhasorGroup.PhaseB.AngleResidualKey))
                    {
                        output.Add(voltagePhasorGroup.PhaseB.AngleResidualKey, voltagePhasorGroup.PhaseB.AngleResidualInDegrees);
                    }

                    if (!output.ContainsKey(voltagePhasorGroup.PhaseC.MagnitudeResidualKey))
                    {
                        output.Add(voltagePhasorGroup.PhaseC.MagnitudeResidualKey, voltagePhasorGroup.PhaseC.MagnitudeResidual);
                    }
                    if (!output.ContainsKey(voltagePhasorGroup.PhaseC.AngleResidualKey))
                    {
                        output.Add(voltagePhasorGroup.PhaseC.AngleResidualKey, voltagePhasorGroup.PhaseC.AngleResidualInDegrees);
                    }
                }
            }
        }

        private List<OutputMeasurement> GetCurrentMeasurementResidualsOutput()
        {
            List<OutputMeasurement> output = new List<OutputMeasurement>();

            List<string> keys = new List<string>();

            foreach (CurrentFlowPhasorGroup current in m_currentFlows)
            {
                if (!keys.Contains(current.PositiveSequence.MagnitudeResidualKey) &&
                    !keys.Contains(current.PositiveSequence.AngleResidualKey))
                {
                    keys.Add(current.PositiveSequence.MagnitudeResidualKey);
                    keys.Add(current.PositiveSequence.AngleResidualKey);

                    output.Add(new OutputMeasurement()
                    {
                        InternalId = current.InternalID,
                        SubstationName = current.MeasuredFromNode.ParentSubstation.Name,
                        MeasuredDeviceType = MeasuredDeviceType.Line,
                        OutputType = OutputType.CurrentFlowMagnitudeResidual,
                        DeviceId = (current.MeasuredBranch as INetworkDescribable).Name,
                        DeviceSuffix = current.MeasuredFromNode.ParentSubstation.Name,
                        Key = current.PositiveSequence.MagnitudeResidualKey,
                        Value = current.PositiveSequence.MagnitudeResidual,
                        Description = $"{current.Description} Positive Sequence Magnitude Residual"
                    });
                    output.Add(new OutputMeasurement()
                    {
                        InternalId = current.InternalID,
                        SubstationName = current.MeasuredFromNode.ParentSubstation.Name,
                        MeasuredDeviceType = MeasuredDeviceType.Line,
                        OutputType = OutputType.CurrentFlowAngleResidual,
                        DeviceId = (current.MeasuredBranch as INetworkDescribable).Name,
                        DeviceSuffix = current.MeasuredFromNode.ParentSubstation.Name,
                        Key = current.PositiveSequence.AngleResidualKey,
                        Value = current.PositiveSequence.AngleResidualInDegrees,
                        Description = $"{current.Description} Positive Sequence Angle Residual In Degrees"
                    });
                }
            }

            foreach (CurrentInjectionPhasorGroup current in m_currentInjections)
            {
                output.Add(new OutputMeasurement()
                {
                    InternalId = current.InternalID,
                    SubstationName = current.MeasuredConnectedNode.ParentSubstation.Name,
                    MeasuredDeviceType = MeasuredDeviceType.Shunt,
                    OutputType = OutputType.CurrentInjectionMagnitudeResidual,
                    DeviceId = (current.MeasuredBranch as INetworkDescribable).Name,
                    DeviceSuffix = current.MeasuredConnectedNode.ParentSubstation.Name,
                    Key = current.PositiveSequence.MagnitudeResidualKey,
                    Value = current.PositiveSequence.MagnitudeResidual,
                    Description = $"{current.Description} Positive Sequence Magnitude Residual"
                });
                output.Add(new OutputMeasurement()
                {
                    InternalId = current.InternalID,
                    SubstationName = current.MeasuredConnectedNode.ParentSubstation.Name,
                    MeasuredDeviceType = MeasuredDeviceType.Shunt,
                    OutputType = OutputType.CurrentInjectionAngleResidual,
                    DeviceId = (current.MeasuredBranch as INetworkDescribable).Name,
                    DeviceSuffix = current.MeasuredConnectedNode.ParentSubstation.Name,
                    Key = current.PositiveSequence.AngleResidualKey,
                    Value = current.PositiveSequence.AngleResidualInDegrees,
                    Description = $"{current.Description} Positive Sequence Angle Residual In Degrees"
                });
            }
            return output;
        }

        /// <summary>
        /// Adds the current measurement residual components to the output with their associated keys.
        /// </summary>
        /// <param name="output">The Dictionary that contains the output keys and values.</param>
        private void AddCurrentMeasurementResidualsToOutput(Dictionary<string, double> output)
        {
            foreach (CurrentFlowPhasorGroup currentPhasorGroup in m_currentFlows)
            {
                if (!output.ContainsKey(currentPhasorGroup.PositiveSequence.MagnitudeResidualKey))
                {
                    output.Add(currentPhasorGroup.PositiveSequence.MagnitudeResidualKey, currentPhasorGroup.PositiveSequence.MagnitudeResidual);
                }
                if (!output.ContainsKey(currentPhasorGroup.PositiveSequence.AngleResidualKey))
                {
                    output.Add(currentPhasorGroup.PositiveSequence.AngleResidualKey, currentPhasorGroup.PositiveSequence.AngleResidualInDegrees);
                }

                if (m_phaseSelection == PhaseSelection.ThreePhase)
                {
                    if (!output.ContainsKey(currentPhasorGroup.PhaseA.MagnitudeResidualKey))
                    {
                        output.Add(currentPhasorGroup.PhaseA.MagnitudeResidualKey, currentPhasorGroup.PhaseA.MagnitudeResidual);
                    }
                    if (!output.ContainsKey(currentPhasorGroup.PhaseA.AngleResidualKey))
                    {
                        output.Add(currentPhasorGroup.PhaseA.AngleResidualKey, currentPhasorGroup.PhaseA.AngleResidualInDegrees);
                    }

                    if (!output.ContainsKey(currentPhasorGroup.PhaseB.MagnitudeResidualKey))
                    {
                        output.Add(currentPhasorGroup.PhaseB.MagnitudeResidualKey, currentPhasorGroup.PhaseB.MagnitudeResidual);
                    }
                    if (!output.ContainsKey(currentPhasorGroup.PhaseB.AngleResidualKey))
                    {
                        output.Add(currentPhasorGroup.PhaseB.AngleResidualKey, currentPhasorGroup.PhaseB.AngleResidualInDegrees);
                    }

                    if (!output.ContainsKey(currentPhasorGroup.PhaseC.MagnitudeResidualKey))
                    {
                        output.Add(currentPhasorGroup.PhaseC.MagnitudeResidualKey, currentPhasorGroup.PhaseC.MagnitudeResidual);
                    }
                    if (!output.ContainsKey(currentPhasorGroup.PhaseC.AngleResidualKey))
                    {
                        output.Add(currentPhasorGroup.PhaseC.AngleResidualKey, currentPhasorGroup.PhaseC.AngleResidualInDegrees);
                    }
                }
            }
        }

        private List<OutputMeasurement> GetCircuitBreakerStatusesOutput()
        {
            List<OutputMeasurement> output = new List<OutputMeasurement>();
            List<string> keys = new List<string>();

            foreach (CircuitBreaker breaker in m_circuitBreakers)
            {
                if (!keys.Contains(breaker.MeasurementKey))
                {
                    keys.Add(breaker.MeasurementKey);

                    output.Add(new OutputMeasurement()
                    {
                        InternalId = breaker.InternalID,
                        SubstationName = breaker.ParentSubstation.Name,
                        MeasuredDeviceType = MeasuredDeviceType.CircuitBreaker,
                        OutputType = OutputType.CircuitBreakerStatus,
                        DeviceId = breaker.Name,
                        DeviceSuffix = breaker.ParentSubstation.Name,
                        Key = breaker.MeasurementKey,
                        Value = (double)breaker.ActualState,
                        Description = $"{breaker.Description} Actual State"
                    });
                }
            }

            return output;
        }

        /// <summary>
        /// Adds the status of the circuit breakers with their associated keys.
        /// </summary>
        /// <param name="output">The Dictionary that contains the output keys and values.</param>
        private void AddCircuitBreakerStatusesToOutput(Dictionary<string, double> output)
        {
            foreach (SwitchingDeviceBase switchingDevice in m_switchingDevices)
            {
                if (switchingDevice is CircuitBreaker)
                {
                    if (!output.ContainsKey(switchingDevice.MeasurementKey))
                    {
                        output.Add(switchingDevice.MeasurementKey, (int)switchingDevice.ActualState);
                    }
                }
            }
        }

        private List<OutputMeasurement> GetSwitchStatusesOutput()
        {
            List<OutputMeasurement> output = new List<OutputMeasurement>();
            List<string> keys = new List<string>();


            foreach (Switch switchingDevice in m_switches)
            {
                if (!keys.Contains(switchingDevice.MeasurementKey))
                {
                    keys.Add(switchingDevice.MeasurementKey);

                    output.Add(new OutputMeasurement()
                    {
                        InternalId = switchingDevice.InternalID,
                        SubstationName = switchingDevice.ParentSubstation.Name,
                        MeasuredDeviceType = MeasuredDeviceType.Switch,
                        OutputType = OutputType.SwitchStatus,
                        DeviceId = switchingDevice.Name,
                        DeviceSuffix = switchingDevice.ParentSubstation.Name,
                        Key = switchingDevice.MeasurementKey,
                        Value = (double)switchingDevice.ActualState,
                        Description = $"{switchingDevice.Description} Actual State"
                    });
                }

            }

            return output;
        }

        /// <summary>
        /// Adds the status of the circuit switches with their associated keys.
        /// </summary>
        /// <param name="output">The Dictionary that contains the output keys and values.</param>
        private void AddSwitchStatusesToOutput(Dictionary<string, double> output)
        {
            foreach (SwitchingDeviceBase switchingDevice in m_switchingDevices)
            {
                if (switchingDevice is Switch)
                {
                    if (!output.ContainsKey(switchingDevice.MeasurementKey))
                    {
                        output.Add(switchingDevice.MeasurementKey, (int)switchingDevice.ActualState);
                    }
                }
            }
        }

        private List<OutputMeasurement> GetPerformanceMetricsOutput()
        {
            List<OutputMeasurement> output = new List<OutputMeasurement>();

            PerformanceMetrics metrics = m_parentNetwork.PerformanceMetrics;

            output.Add(new OutputMeasurement()
            {
                InternalId = 1,
                MeasuredDeviceType = MeasuredDeviceType.PerformanceMetric,
                OutputType = OutputType.PerformanceMetric,
                DeviceId = "ACTIVE_VOLTAGE_COUNT",
                DeviceSuffix = "PERFORMANCE_METRICS",
                Key = metrics.ActiveVoltageCountKey,
                Value = metrics.ActiveVoltageCount,
                Description = "Active Voltage Count"
            });
            output.Add(new OutputMeasurement()
            {
                InternalId = 2,
                MeasuredDeviceType = MeasuredDeviceType.PerformanceMetric,
                OutputType = OutputType.PerformanceMetric,
                DeviceId = "ACTIVE_FLOW_COUNT",
                DeviceSuffix = "PERFORMANCE_METRICS",
                Key = metrics.ActiveCurrentFlowCountKey,
                Value = metrics.ActiveCurrentFlowCount,
                Description = "Active Current Flow Count"
            });
            output.Add(new OutputMeasurement()
            {
                InternalId = 3,
                MeasuredDeviceType = MeasuredDeviceType.PerformanceMetric,
                OutputType = OutputType.PerformanceMetric,
                DeviceId = "ACTIVE_INJECTION_COUNT",
                DeviceSuffix = "PERFORMANCE_METRICS",
                Key = metrics.ActiveCurrentInjectionCountKey,
                Value = metrics.ActiveCurrentInjectionCount,
                Description = "Active Current Injection Count"
            });
            output.Add(new OutputMeasurement()
            {
                InternalId = 4,
                MeasuredDeviceType = MeasuredDeviceType.PerformanceMetric,
                OutputType = OutputType.PerformanceMetric,
                DeviceId = "OBSERVED_BUS_COUNT",
                DeviceSuffix = "PERFORMANCE_METRICS",
                Key = metrics.ObservedBusCountKey,
                Value = metrics.ObservedBusCount,
                Description = "Observed Bus Count"
            });
            output.Add(new OutputMeasurement()
            {
                InternalId = 5,
                MeasuredDeviceType = MeasuredDeviceType.PerformanceMetric,
                OutputType = OutputType.PerformanceMetric,
                DeviceId = "REFRESH_TIME",
                DeviceSuffix = "PERFORMANCE_METRICS",
                Key = metrics.RefreshExecutionTimeKey,
                Value = metrics.RefreshExecutionTime,
                Description = "Refresh Time"
            });
            output.Add(new OutputMeasurement()
            {
                InternalId = 6,
                SubstationName = "PERFORMANCE_METRICS",
                MeasuredDeviceType = MeasuredDeviceType.PerformanceMetric,
                OutputType = OutputType.PerformanceMetric,
                DeviceId = "PARSING_TIME",
                DeviceSuffix = "PERFORMANCE_METRICS",
                Key = metrics.ParsingExecutionTimeKey,
                Value = metrics.ParsingExecutionTime,
                Description = "Parsing Time"
            });
            output.Add(new OutputMeasurement()
            {
                InternalId = 7,
                MeasuredDeviceType = MeasuredDeviceType.PerformanceMetric,
                OutputType = OutputType.PerformanceMetric,
                DeviceId = "MEASUREMENT_MAPPING_TIME",
                DeviceSuffix = "PERFORMANCE_METRICS",
                Key = metrics.MeasurementMappingExecutionTimeKey,
                Value = metrics.MeasurementMappingExecutionTime,
                Description = "Measurement Mapping Time"
            });
            output.Add(new OutputMeasurement()
            {
                InternalId = 8,
                MeasuredDeviceType = MeasuredDeviceType.PerformanceMetric,
                OutputType = OutputType.PerformanceMetric,
                DeviceId = "ACTIVE_CURRENT_DETERMINATION_TIME",
                DeviceSuffix = "PERFORMANCE_METRICS",
                Key = metrics.ActiveCurrentPhasorDeterminationExecutionTimeKey,
                Value = metrics.ActiveCurrentPhasorDeterminationExecutionTime,
                Description = "Active Current Determination Time"
            });
            output.Add(new OutputMeasurement()
            {
                InternalId = 9,
                MeasuredDeviceType = MeasuredDeviceType.PerformanceMetric,
                OutputType = OutputType.PerformanceMetric,
                DeviceId = "OBSERVABILITY_ANALYSIS_TIME",
                DeviceSuffix = "PERFORMANCE_METRICS",
                Key = metrics.ObservabilityAnalysisExecutionTimeKey,
                Value = metrics.ObservabilityAnalysisExecutionTime,
                Description = "Observability Analysis Time"
            });
            output.Add(new OutputMeasurement()
            {
                InternalId = 10,
                MeasuredDeviceType = MeasuredDeviceType.PerformanceMetric,
                OutputType = OutputType.PerformanceMetric,
                DeviceId = "STATE_COMPUTATION_TIME",
                DeviceSuffix = "PERFORMANCE_METRICS",
                Key = metrics.StateComputationExecutionTimeKey,
                Value = metrics.StateComputationExecutionTime,
                Description = "State Computation Time"
            });
            output.Add(new OutputMeasurement()
            {
                InternalId = 11,
                MeasuredDeviceType = MeasuredDeviceType.PerformanceMetric,
                OutputType = OutputType.PerformanceMetric,
                DeviceId = "OUTPUT_PREP_TIME",
                DeviceSuffix = "PERFORMANCE_METRICS",
                Key = metrics.OutputPreparationExecutionTimeKey,
                Value = metrics.OutputPreparationExecutionTime,
                Description = "Output Preparation Time"
            });
            output.Add(new OutputMeasurement()
            {
                InternalId = 12,
                MeasuredDeviceType = MeasuredDeviceType.PerformanceMetric,
                OutputType = OutputType.PerformanceMetric,
                DeviceId = "TIME_IN_TICKS",
                DeviceSuffix = "PERFORMANCE_METRICS",
                Key = metrics.TotalExecutionTimeInTicksKey,
                Value = metrics.TotalExecutionTimeInTicks,
                Description = "Total Time in Ticks"
            });
            output.Add(new OutputMeasurement()
            {
                InternalId = 13,
                MeasuredDeviceType = MeasuredDeviceType.PerformanceMetric,
                OutputType = OutputType.PerformanceMetric,
                DeviceId = "TIME_IN_MS",
                DeviceSuffix = "PERFORMANCE_METRICS",
                Key = metrics.TotalExecutionTimeInMillisecondsKey,
                Value = metrics.TotalExecutionTimeInMilliseconds,
                Description = "Total Time in Miliseconds"
            });

            return output;
        }

        private void AddPerformanceMetricsToOutput(Dictionary<string, double> output)
        {
            PerformanceMetrics metrics = m_parentNetwork.PerformanceMetrics;
            output.Add(metrics.ActiveVoltageCountKey, metrics.ActiveVoltageCount);
            output.Add(metrics.ActiveCurrentFlowCountKey, metrics.ActiveCurrentFlowCount);
            output.Add(metrics.ActiveCurrentInjectionCountKey, metrics.ActiveCurrentInjectionCount);
            output.Add(metrics.ObservedBusCountKey, metrics.ObservedBusCount);
            output.Add(metrics.RefreshExecutionTimeKey, metrics.RefreshExecutionTime);
            output.Add(metrics.ParsingExecutionTimeKey, metrics.ParsingExecutionTime);
            output.Add(metrics.MeasurementMappingExecutionTimeKey, metrics.MeasurementMappingExecutionTime);
            output.Add(metrics.ObservabilityAnalysisExecutionTimeKey, metrics.ObservabilityAnalysisExecutionTime);
            output.Add(metrics.ActiveCurrentPhasorDeterminationExecutionTimeKey, metrics.ActiveCurrentPhasorDeterminationExecutionTime);
            output.Add(metrics.StateComputationExecutionTimeKey, metrics.StateComputationExecutionTime);
            output.Add(metrics.SolutionRetrievalExecutionTimeKey, metrics.SolutionRetrievalExecutionTime);
            output.Add(metrics.OutputPreparationExecutionTimeKey, metrics.OutputPreparationExecutionTime);
            output.Add(metrics.TotalExecutionTimeInTicksKey, metrics.TotalExecutionTimeInTicks);
            output.Add(metrics.TotalExecutionTimeInMillisecondsKey, metrics.TotalExecutionTimeInMilliseconds);
        }

        private List<OutputMeasurement> GetTopologyProfilingInformationOutput()
        {
            List<OutputMeasurement> output = new List<OutputMeasurement>();

            List<string> keys = new List<string>();

            foreach (Substation substation in m_substations)
            {
                if (!keys.Contains(substation.ObservedBusCountKey))
                {
                    output.Add(new OutputMeasurement()
                    {
                        InternalId = substation.InternalID,
                        SubstationName = substation.Name,
                        MeasuredDeviceType = MeasuredDeviceType.Substation,
                        OutputType = OutputType.TopologyProfiling,
                        DeviceId = $"{substation.Name}",
                        DeviceSuffix = substation.Name,
                        Key = substation.ObservedBusCountKey,
                        Value = substation.ObservedBusCount,
                        Description = $"{substation.Name} Observed Bus Count"
                    });
                }
                foreach (Node node in substation.Nodes)
                {
                    if (!keys.Contains(node.ObservationStateKey) &&
                        !keys.Contains(node.ObservedBusIdKey))
                    {

                        output.Add(new OutputMeasurement()
                        {
                            InternalId = node.InternalID,
                            SubstationName = substation.Name,
                            MeasuredDeviceType = MeasuredDeviceType.Node,
                            OutputType = OutputType.TopologyProfiling,
                            DeviceId = $"{node.Name}_TOPOOGY_STATE",
                            DeviceSuffix = substation.Name,
                            Key = node.ObservationStateKey,
                            Value = (double)node.Observability,
                            Description = $"{node.Name} Observation State"
                        });
                        output.Add(new OutputMeasurement()
                        {
                            InternalId = node.InternalID,
                            SubstationName = substation.Name,
                            MeasuredDeviceType = MeasuredDeviceType.Node,
                            OutputType = OutputType.TopologyProfiling,
                            DeviceId = $"{node.Name}_TOPOLOGY_ID",
                            DeviceSuffix = substation.Name,
                            Key = node.ObservedBusIdKey,
                            Value = (double)node.ObservedBusId,
                            Description = $"{node.Name} Observed Bus Id"
                        });
                    }
                }
            }
            return output;
        }

        private void AddTopologyProfilingInformationToOutput(Dictionary<string, double> output)
        {
            foreach (Substation substation in m_substations)
            {
                output.Add(substation.ObservedBusCountKey, substation.ObservedBusCount);
                foreach (Node node in substation.Nodes)
                {
                    output.Add(node.ObservationStateKey, Convert.ToDouble(node.Observability));
                    output.Add(node.ObservedBusIdKey, node.ObservedBusId);
                }
            }
        }

        private List<OutputMeasurement> GetMeasurementValidationFlagsOutput()
        {
            List<OutputMeasurement> output = new List<OutputMeasurement>();

            foreach (VoltagePhasorGroup voltage in m_voltages)
            {
                output.Add(new OutputMeasurement()
                {
                    InternalId = voltage.InternalID,
                    SubstationName = voltage.MeasuredNode.ParentSubstation.Name,
                    MeasuredDeviceType = MeasuredDeviceType.Node,
                    OutputType = OutputType.VoltageMeasurementValidationFlag,
                    DeviceId = voltage.MeasuredNode.Name,
                    DeviceSuffix = voltage.MeasuredNode.ParentSubstation.Name,
                    Key = voltage.MeasurementIsIncludedKey,
                    Value = Convert.ToDouble(voltage.IncludeInPositiveSequenceEstimator),
                    Description = $"{voltage.Description} Measurement Validation Flag"
                });
            }
            foreach (CurrentFlowPhasorGroup current in m_currentFlows)
            {
                output.Add(new OutputMeasurement()
                {
                    InternalId = current.InternalID,
                    SubstationName = current.MeasuredFromNode.ParentSubstation.Name,
                    MeasuredDeviceType = MeasuredDeviceType.Node,
                    OutputType = OutputType.CurrentFlowMeasurementValidationFlag,
                    DeviceId = current.MeasuredFromNode.Name,
                    DeviceSuffix = current.MeasuredFromNode.ParentSubstation.Name,
                    Key = current.MeasurementIsIncludedKey,
                    Value = Convert.ToDouble(current.IncludeInPositiveSequenceEstimator),
                    Description = $"{current.Description} Measurement Validation Flag"
                });
            }
            foreach (CurrentInjectionPhasorGroup current in m_currentInjections)
            {
                output.Add(new OutputMeasurement()
                {
                    InternalId = current.InternalID,
                    SubstationName = current.MeasuredConnectedNode.ParentSubstation.Name,
                    MeasuredDeviceType = MeasuredDeviceType.Node,
                    OutputType = OutputType.CurrentInjectionMeasurementValidationFlag,
                    DeviceId = current.MeasuredConnectedNode.Name,
                    DeviceSuffix = current.MeasuredConnectedNode.ParentSubstation.Name,
                    Key = current.MeasurementIsIncludedKey,
                    Value = Convert.ToDouble(current.IncludeInPositiveSequenceEstimator),
                    Description = $"{current.Description} Measurement Validation Flag"
                });
            }

            return output;
        }

        private void AddMeasurementValidationFlagsToOutput(Dictionary<string, double> output)
        {
            foreach (VoltagePhasorGroup voltage in m_voltages)
            {
                if (m_phaseSelection == PhaseSelection.PositiveSequence)
                {
                    output.Add(voltage.MeasurementIsIncludedKey, Convert.ToDouble(voltage.IncludeInPositiveSequenceEstimator));
                }
                else if (m_phaseSelection == PhaseSelection.ThreePhase)
                {
                    output.Add(voltage.MeasurementIsIncludedKey, Convert.ToDouble(voltage.IncludeInEstimator));
                }
            }
            foreach (CurrentFlowPhasorGroup currentFlow in m_currentFlows)
            {
                if (m_phaseSelection == PhaseSelection.PositiveSequence)
                {
                    output.Add(currentFlow.MeasurementIsIncludedKey, Convert.ToDouble(currentFlow.IncludeInPositiveSequenceEstimator));
                }
                else if (m_phaseSelection == PhaseSelection.ThreePhase)
                {
                    output.Add(currentFlow.MeasurementIsIncludedKey, Convert.ToDouble(currentFlow.IncludeInEstimator));
                }
            }
            foreach (CurrentInjectionPhasorGroup currentInjection in m_currentInjections)
            {
                if (m_phaseSelection == PhaseSelection.PositiveSequence)
                {
                    output.Add(currentInjection.MeasurementIsIncludedKey, Convert.ToDouble(currentInjection.IncludeInPositiveSequenceEstimator));
                }
                else if (m_phaseSelection == PhaseSelection.ThreePhase)
                {
                    output.Add(currentInjection.MeasurementIsIncludedKey, Convert.ToDouble(currentInjection.IncludeInEstimator));
                }
            }
        }

        private void ComputeVoltageSequenceValues()
        {
            foreach (VoltagePhasorGroup voltagePhasorGroup in m_voltages)
            {
                voltagePhasorGroup.ComputeSequenceComponents();
            }
        }

        private void ComputeCurrentFlowSequenceValues()
        {
            foreach (CurrentFlowPhasorGroup currentFlowPhasorGroup in m_currentFlows)
            {
                currentFlowPhasorGroup.ComputeSequenceComponents();
            }
        }

        private void ComputeCurrentInjectionSequenceValues()
        {
            foreach (CurrentInjectionPhasorGroup currentInjectionPhasorGroup in m_currentInjections)
            {
                currentInjectionPhasorGroup.ComputeSequenceComponents();
            }
        }

        private void AppendReceivedVoltageMeasurements(Dictionary<string, double> receivedMeasurements)
        {
            foreach (VoltagePhasorGroup voltagePhasorGroup in m_voltages)
            {
                if (m_phaseSelection == PhaseSelection.PositiveSequence)
                {
                    if (voltagePhasorGroup.PositiveSequence.Measurement.MagnitudeValueWasReported)
                    {
                        receivedMeasurements.Add(voltagePhasorGroup.PositiveSequence.Measurement.MagnitudeKey, voltagePhasorGroup.PositiveSequence.Measurement.Magnitude);
                    }
                    if (voltagePhasorGroup.PositiveSequence.Measurement.AngleValueWasReported)
                    {
                        receivedMeasurements.Add(voltagePhasorGroup.PositiveSequence.Measurement.AngleKey, voltagePhasorGroup.PositiveSequence.Measurement.AngleInDegrees);
                    }
                }
                else if (m_phaseSelection == PhaseSelection.ThreePhase)
                {
                    if (voltagePhasorGroup.PhaseA.Measurement.MagnitudeValueWasReported)
                    {
                        receivedMeasurements.Add(voltagePhasorGroup.PositiveSequence.Measurement.MagnitudeKey, voltagePhasorGroup.PositiveSequence.Measurement.Magnitude);
                    }
                    if (voltagePhasorGroup.PhaseA.Measurement.AngleValueWasReported)
                    {
                        receivedMeasurements.Add(voltagePhasorGroup.PositiveSequence.Measurement.AngleKey, voltagePhasorGroup.PositiveSequence.Measurement.AngleInDegrees);
                    }
                    if (voltagePhasorGroup.PhaseB.Measurement.MagnitudeValueWasReported)
                    {
                        receivedMeasurements.Add(voltagePhasorGroup.PositiveSequence.Measurement.MagnitudeKey, voltagePhasorGroup.PositiveSequence.Measurement.Magnitude);
                    }
                    if (voltagePhasorGroup.PhaseB.Measurement.AngleValueWasReported)
                    {
                        receivedMeasurements.Add(voltagePhasorGroup.PositiveSequence.Measurement.AngleKey, voltagePhasorGroup.PositiveSequence.Measurement.AngleInDegrees);
                    }
                    if (voltagePhasorGroup.PhaseC.Measurement.MagnitudeValueWasReported)
                    {
                        receivedMeasurements.Add(voltagePhasorGroup.PositiveSequence.Measurement.MagnitudeKey, voltagePhasorGroup.PositiveSequence.Measurement.Magnitude);
                    }
                    if (voltagePhasorGroup.PhaseC.Measurement.AngleValueWasReported)
                    {
                        receivedMeasurements.Add(voltagePhasorGroup.PositiveSequence.Measurement.AngleKey, voltagePhasorGroup.PositiveSequence.Measurement.AngleInDegrees);
                    }
                }
            }
        }

        private void AppendReceivedCurrentFlowMeasurements(Dictionary<string, double> receivedMeasurements)
        {
            foreach (CurrentFlowPhasorGroup currentFlowPhasorGroup in m_currentFlows)
            {
                if (m_phaseSelection == PhaseSelection.PositiveSequence)
                {
                    if (currentFlowPhasorGroup.PositiveSequence.Measurement.MagnitudeValueWasReported)
                    {
                        receivedMeasurements.Add(currentFlowPhasorGroup.PositiveSequence.Measurement.MagnitudeKey, currentFlowPhasorGroup.PositiveSequence.Measurement.Magnitude);
                    }
                    if (currentFlowPhasorGroup.PositiveSequence.Measurement.AngleValueWasReported)
                    {
                        receivedMeasurements.Add(currentFlowPhasorGroup.PositiveSequence.Measurement.AngleKey, currentFlowPhasorGroup.PositiveSequence.Measurement.AngleInDegrees);
                    }
                }
                else if (m_phaseSelection == PhaseSelection.ThreePhase)
                {
                    if (currentFlowPhasorGroup.PhaseA.Measurement.MagnitudeValueWasReported)
                    {
                        receivedMeasurements.Add(currentFlowPhasorGroup.PositiveSequence.Measurement.MagnitudeKey, currentFlowPhasorGroup.PositiveSequence.Measurement.Magnitude);
                    }
                    if (currentFlowPhasorGroup.PhaseA.Measurement.AngleValueWasReported)
                    {
                        receivedMeasurements.Add(currentFlowPhasorGroup.PositiveSequence.Measurement.AngleKey, currentFlowPhasorGroup.PositiveSequence.Measurement.AngleInDegrees);
                    }
                    if (currentFlowPhasorGroup.PhaseB.Measurement.MagnitudeValueWasReported)
                    {
                        receivedMeasurements.Add(currentFlowPhasorGroup.PositiveSequence.Measurement.MagnitudeKey, currentFlowPhasorGroup.PositiveSequence.Measurement.Magnitude);
                    }
                    if (currentFlowPhasorGroup.PhaseB.Measurement.AngleValueWasReported)
                    {
                        receivedMeasurements.Add(currentFlowPhasorGroup.PositiveSequence.Measurement.AngleKey, currentFlowPhasorGroup.PositiveSequence.Measurement.AngleInDegrees);
                    }
                    if (currentFlowPhasorGroup.PhaseC.Measurement.MagnitudeValueWasReported)
                    {
                        receivedMeasurements.Add(currentFlowPhasorGroup.PositiveSequence.Measurement.MagnitudeKey, currentFlowPhasorGroup.PositiveSequence.Measurement.Magnitude);
                    }
                    if (currentFlowPhasorGroup.PhaseC.Measurement.AngleValueWasReported)
                    {
                        receivedMeasurements.Add(currentFlowPhasorGroup.PositiveSequence.Measurement.AngleKey, currentFlowPhasorGroup.PositiveSequence.Measurement.AngleInDegrees);
                    }
                }
            }
        }

        private void AppendReceivedCurrentInjectionMeasurements(Dictionary<string, double> receivedMeasurements)
        {
            foreach (CurrentInjectionPhasorGroup currentInjectionPhasorGroup in m_currentInjections)
            {
                if (m_phaseSelection == PhaseSelection.PositiveSequence)
                {
                    if (currentInjectionPhasorGroup.PositiveSequence.Measurement.MagnitudeValueWasReported)
                    {
                        receivedMeasurements.Add(currentInjectionPhasorGroup.PositiveSequence.Measurement.MagnitudeKey, currentInjectionPhasorGroup.PositiveSequence.Measurement.Magnitude);
                    }
                    if (currentInjectionPhasorGroup.PositiveSequence.Measurement.AngleValueWasReported)
                    {
                        receivedMeasurements.Add(currentInjectionPhasorGroup.PositiveSequence.Measurement.AngleKey, currentInjectionPhasorGroup.PositiveSequence.Measurement.AngleInDegrees);
                    }
                }
                else if (m_phaseSelection == PhaseSelection.ThreePhase)
                {
                    if (currentInjectionPhasorGroup.PhaseA.Measurement.MagnitudeValueWasReported)
                    {
                        receivedMeasurements.Add(currentInjectionPhasorGroup.PositiveSequence.Measurement.MagnitudeKey, currentInjectionPhasorGroup.PositiveSequence.Measurement.Magnitude);
                    }
                    if (currentInjectionPhasorGroup.PhaseA.Measurement.AngleValueWasReported)
                    {
                        receivedMeasurements.Add(currentInjectionPhasorGroup.PositiveSequence.Measurement.AngleKey, currentInjectionPhasorGroup.PositiveSequence.Measurement.AngleInDegrees);
                    }
                    if (currentInjectionPhasorGroup.PhaseB.Measurement.MagnitudeValueWasReported)
                    {
                        receivedMeasurements.Add(currentInjectionPhasorGroup.PositiveSequence.Measurement.MagnitudeKey, currentInjectionPhasorGroup.PositiveSequence.Measurement.Magnitude);
                    }
                    if (currentInjectionPhasorGroup.PhaseB.Measurement.AngleValueWasReported)
                    {
                        receivedMeasurements.Add(currentInjectionPhasorGroup.PositiveSequence.Measurement.AngleKey, currentInjectionPhasorGroup.PositiveSequence.Measurement.AngleInDegrees);
                    }
                    if (currentInjectionPhasorGroup.PhaseC.Measurement.MagnitudeValueWasReported)
                    {
                        receivedMeasurements.Add(currentInjectionPhasorGroup.PositiveSequence.Measurement.MagnitudeKey, currentInjectionPhasorGroup.PositiveSequence.Measurement.Magnitude);
                    }
                    if (currentInjectionPhasorGroup.PhaseC.Measurement.AngleValueWasReported)
                    {
                        receivedMeasurements.Add(currentInjectionPhasorGroup.PositiveSequence.Measurement.AngleKey, currentInjectionPhasorGroup.PositiveSequence.Measurement.AngleInDegrees);
                    }
                }
            }
        }

        #endregion

        #region [ Xml Serialization/Deserialization ]

        /// <summary>
        /// Creates a new <see cref="NetworkModel"/> by deserializing the configuration file from the specified location.
        /// </summary>
        /// <param name="pathName">The location of the configuration file including the file name.</param>
        /// <returns>A new <see cref="NetworkModel"/> based on the configuration file.</returns>
        public static NetworkModel DeserializeFromXml(string pathName)
        {
            try
            {
                // Create an empy NetworkModel object reference.
                NetworkModel networkModel = null;

                // Create an XmlSerializer with the type of NetworkModel.
                XmlSerializer deserializer = new XmlSerializer(typeof(NetworkModel));

                // Read the data in from the file.
                StreamReader reader = new StreamReader(pathName);

                // Cast the deserialized data as a NetworkModel object.
                networkModel = (NetworkModel)deserializer.Deserialize(reader);

                // Close the connection.
                reader.Close();

                return networkModel;
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to Deserialize the NetworkModel from the Configuration File: " + exception.ToString());
            }
        }

        /// <summary>
        /// Serialized the <see cref="NetworkModel"/> to the specified file.
        /// </summary>
        /// <param name="pathName">The directory name included the file name of the desired location for Xml Serialization.</param>
        public void SerializeToXml(string pathName)
        {
            try
            {
                // Create an XmlSerializer with the type of NetworkModel
                XmlSerializer serializer = new XmlSerializer(typeof(NetworkModel));

                // Open a connection to the file and path.
                TextWriter writer = new StreamWriter(pathName);

                // Serialize this instance of NetworkModel
                serializer.Serialize(writer, this);

                // Close the connection
                writer.Close();
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to Serialize the NetworkModel to the Configuration File: " + exception.ToString());
            }
        }

        #endregion
    }
}
