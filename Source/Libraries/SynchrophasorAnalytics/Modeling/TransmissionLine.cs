//******************************************************************************************************
//  TransmissionLine.cs
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
//  09/17/2013 - Kevin D. Jones
//       Added m_possibleImpedanceValues field.
//       Added m_childrenSeriesCompensationDevices field & associated property (SeriesCompensationDevices)
//       Added new declaration for m_childrenSeriesCompensationDevices in designated constructor.
//       Added public method InitializePossibleImpedanceValues()
//       Added private method ComputeBaseCaseImpedance()
//  05/16/2014 - Kevin D. Jones
//       Added System.Numerics for complex numbers
//  05/17/2014 - Kevin D. Jones
//       Added Math.NET for complex matrices
//  06/24/2014 - Kevin D. Jones
//       Added logic for 'CurrentFlowPostProcessingSetting' in methods for computing estimated flows.
//  07/04/2014 - Kevin D. Jones
//       Added parameters, logic, methods, for series compensator status inference.
//  07/07/2014 - Kevin D. Jones
//       Added Guid
//  07/29/2014 - Kevin D. Jones
//       Added additional necessary parameters for series compensator status inference and real time
//       impedance calculation.
// 
//******************************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Xml.Serialization;
using MathNet.Numerics;

using MathNet.Numerics.LinearAlgebra.Complex;
using SynchrophasorAnalytics.Graphs;
using SynchrophasorAnalytics.Measurements;
using SynchrophasorAnalytics.Networks;

namespace SynchrophasorAnalytics.Modeling
{
    /// <summary>
    /// A <see cref="LinearStateEstimator.Modeling.TransmissionLine"/> represents a collection of network elements which usually are enclosed within the same right-of-way and connect two <see cref="LinearStateEstimator.Modeling.Substation"/>s. This includes <see cref="LinearStateEstimator.Modeling.Node"/>, <see cref="LinearStateEstimator.Modeling.Switch"/>, <see cref="LinearStateEstimator.Modeling.LineSegment"/>, and/or <see cref="LinearStateEstimator.Modeling.SeriesCompensator"/>
    /// </summary>
    [Serializable()]
    public class TransmissionLine : INetworkDescribable, ITwoTerminal, IPrunable
    {
        #region [ Private Constants ]

        /// <summary>
        /// <see cref="LinearStateEstimator.Modeling.INetworkDescribable"/> defaults.
        /// </summary>
        private const int DEFAULT_INTERNALID = 0;
        private const int DEFAULT_NUMBER = 0;
        private const string DEFAULT_ACRONYM = "TL";
        private const string DEFAULT_NAME = "Undefined";
        private const string DEFAULT_DESCRIPTION = "Undefined";

        #endregion

        #region [ Private Members ]

        /// <summary>
        /// <see cref="LinearStateEstimator.Modeling.INetworkDescribable"/> fields.
        /// </summary>
        private Guid m_uniqueId;
        private int m_internalID;
        private int m_number;
        private string m_acronym;
        private string m_name;
        private string m_description;

        private bool m_complexPowerHasBeenInitialized;

        /// <summary>
        /// Parent
        /// </summary>
        private Division m_parentDivision;

        /// <summary>
        /// Connections
        /// </summary>
        private Substation m_fromSubstation;
        private int m_fromSubstationID;

        private Node m_fromNode;
        private int m_fromNodeID;

        private Substation m_toSubstation;
        private int m_toSubstationID;

        private Node m_toNode;
        private int m_toNodeID;

        /// <summary>
        /// Children
        /// </summary>
        private List<Node> m_childrenNodes;
        private List<LineSegment> m_childrenLineSegments;
        private List<SeriesCompensator> m_childrenSeriesCompensators;
        private List<Switch> m_childrenSwitches;

        /// <summary>
        /// Measurements
        /// </summary>
        private CurrentFlowPhasorGroup m_fromSubstationCurrent;
        private int m_fromSubstationCurrentID;

        private CurrentFlowPhasorGroup m_toSubstationCurrent;
        private int m_toSubstationCurrentID;

        private ComplexPowerGroup m_fromSubstationComplexPower;
        private ComplexPowerGroup m_toSubstationComplexPower;

        /// <summary>
        /// Impedance and Series Compensator Inference
        /// </summary>
        private Impedance m_previousRealTimeCalculatedImpedance;
        private Impedance m_realTimeCalculatedImpedance;
        private Impedance m_fromSideImpedanceToDeepestObservability;
        private Impedance m_toSideImpedanceToDeepestObservability;
        private Impedance m_inferredTotalImpedance;
        private List<Impedance> m_possibleImpedanceValues;
        private int m_numberOfPossibleSeriesCompensators;
        private bool m_seriesCompensationStatusInferenceIsEnabled;
        private bool m_realTimeImpedanceCalculationIsEnabled;
        private double m_calculatedImpedanceChangeThreshold;

        /// <summary>
        /// Graph
        /// </summary>
        private TransmissionLineGraph m_graph;

        #endregion

        #region [ Properties ]

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
        /// An integer identifier for each <see cref="LinearStateEstimator.Modeling.TransmissionLine"/> which is intended to be unique among other objects of the same type.
        /// </summary>
        [XmlAttribute("ID")]
        public int InternalID
        {
            get 
            { 
                return m_internalID; 
            }
            set 
            { 
                m_internalID = value; 
            }
        }

        /// <summary>
        /// A descriptive integer for the instance of the <see cref="LinearStateEstimator.Modeling.TransmissionLine"/>. There are no restrictions on uniqueness.
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
        /// A string acronym for the instance of the <see cref="LinearStateEstimator.Modeling.TransmissionLine"/>.
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
                m_acronym = value.ToUpper(); 
            }
        }

        /// <summary>
        /// The string name of the instance of the <see cref="LinearStateEstimator.Modeling.TransmissionLine"/>.
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
        /// A string description of the instance of the <see cref="LinearStateEstimator.Modeling.TransmissionLine"/>.
        /// </summary>
        [XmlAttribute("Description")]
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
        /// Gets the type of the object as a string.
        /// </summary>
        [XmlIgnore()]
        public string ElementType
        {
            get 
            { 
                return this.GetType().ToString(); 
            }
        }

        /// <summary>
        /// The parent <see cref="LinearStateEstimator.Modeling.Division"/> of the <see cref="LinearStateEstimator.Modeling.TransmissionLine"/>.
        /// </summary>
        [XmlIgnore()]
        public Division ParentDivision
        {
            get 
            { 
                return m_parentDivision; 
            }
            set 
            { 
                m_parentDivision = value; 
            }
        }

        /// <summary>
        /// All of the children <see cref="LinearStateEstimator.Modeling.Node"/> objects of the <see cref="LinearStateEstimator.Modeling.TransmissionLine"/>. Does not contain references to the objects <see cref="LinearStateEstimator.Modeling.TransmissionLine.FromNode"/> and <see cref="LinearStateEstimator.Modeling.TransmissionLine.ToNode"/>.
        /// </summary>
        [XmlArray("Nodes")]
        public List<Node> Nodes
        {
            get
            {
                return m_childrenNodes;
            }
            set
            {
                m_childrenNodes = value;
                foreach (Node node in m_childrenNodes)
                {
                    node.ParentTransmissionLine = this;
                }
            }
        }

        /// <summary>
        /// All of the children <see cref="LinearStateEstimator.Modeling.LineSegment"/> objects of the <see cref="LinearStateEstimator.Modeling.TransmissionLine"/>. 
        /// </summary>
        [XmlArray("LineSegments")]
        public List<LineSegment> LineSegments
        {
            get
            {
                return m_childrenLineSegments;
            }
            set
            {
                m_childrenLineSegments = value;
                foreach (LineSegment lineSegment in m_childrenLineSegments)
                {
                    lineSegment.ParentTransmissionLine = this;
                }
            }
        }

        /// <summary>
        /// All of the children <see cref="LinearStateEstimator.Modeling.SeriesCompensator"/> objects of the <see cref="LinearStateEstimator.Modeling.TransmissionLine"/>. 
        /// </summary>
        [XmlArray("SeriesCompensators")]
        public List<SeriesCompensator> SeriesCompensators
        {

            get
            {
                return m_childrenSeriesCompensators;
            }
            set
            {
                m_childrenSeriesCompensators = value;
                foreach (SeriesCompensator seriesCompensationDevice in m_childrenSeriesCompensators)
                {
                    seriesCompensationDevice.ParentTransmissionLine = this;
                }
            }
        }

        /// <summary>
        /// All of the children <see cref="LinearStateEstimator.Modeling.Switch"/> objects of the <see cref="LinearStateEstimator.Modeling.TransmissionLine"/>. 
        /// </summary>
        [XmlArray("Switches")]
        public List<Switch> Switches
        {
            get
            {
                return m_childrenSwitches;
            }
            set
            {
                m_childrenSwitches = value;
                foreach (Switch lineSwitch in m_childrenSwitches)
                {
                    lineSwitch.ParentTransmissionLine = this;
                }
            }
        }

        /// <summary>
        /// The <see cref="LinearStateEstimator.Modeling.Node"/> in the <see cref="LinearStateEstimator.Modeling.TransmissionLine.FromSubstation"/> where the <see cref="LinearStateEstimator.Modeling.TransmissionLine"/> is said to originate.
        /// </summary>
        [XmlIgnore()]
        public Node FromNode
        {
            get 
            {
                return m_fromNode;
            }
            set
            {
                m_fromNode = value;
                m_fromNodeID = m_fromNode.InternalID;
            }
        }

        /// <summary>
        /// The <see cref="LinearStateEstimator.Modeling.INetworkDescribable.InternalID"/> of the <see cref="LinearStateEstimator.Modeling.Node"/> in the <see cref="LinearStateEstimator.Modeling.TransmissionLine.FromSubstation"/> where the <see cref="LinearStateEstimator.Modeling.TransmissionLine"/> is said to originate.
        /// </summary>
        [XmlAttribute("FromNode")]
        public int FromNodeID
        {
            get
            {
                return m_fromNodeID;
            }
            set
            {
                m_fromNodeID = value;
            }
        }

        /// <summary>
        /// The <see cref="LinearStateEstimator.Modeling.Node"/> in the <see cref="LinearStateEstimator.Modeling.TransmissionLine.ToSubstation"/> where the <see cref="LinearStateEstimator.Modeling.TransmissionLine"/> is said to terminate.
        /// </summary>
        [XmlIgnore()]
        public Node ToNode
        {
            get
            {
                return m_toNode;
            }
            set
            {
                m_toNode = value;
                m_toNodeID = m_toNode.InternalID;
            }
        }

        /// <summary>
        /// The <see cref="LinearStateEstimator.Modeling.INetworkDescribable.InternalID"/> of the <see cref="LinearStateEstimator.Modeling.Node"/> in the <see cref="LinearStateEstimator.Modeling.TransmissionLine.ToSubstation"/> where the <see cref="LinearStateEstimator.Modeling.TransmissionLine"/> is said to terminate.
        /// </summary>
        [XmlAttribute("ToNode")]
        public int ToNodeID
        {
            get
            {
                return m_toNodeID;
            }
            set
            {
                m_toNodeID = value;
            }
        }

        /// <summary>
        /// The <see cref="LinearStateEstimator.Modeling.Substation"/> where the <see cref="LinearStateEstimator.Modeling.TransmissionLine"/> is said to originate.
        /// </summary>
        [XmlIgnore()]
        public Substation FromSubstation
        {
            get
            {
                return m_fromSubstation;
            }
            set
            {
                m_fromSubstation = value;
                m_fromSubstationID = value.InternalID;
            }
        }

        /// <summary>
        /// The <see cref="LinearStateEstimator.Modeling.INetworkDescribable.InternalID"/> of the <see cref="LinearStateEstimator.Modeling.Substation"/> where the <see cref="LinearStateEstimator.Modeling.TransmissionLine"/> is said to originate.
        /// </summary>
        [XmlAttribute("FromSubstation")]
        public int FromSubstationID
        {
            get
            {
                return m_fromSubstationID;
            }
            set
            {
                m_fromSubstationID = value;
            }
        }

        /// <summary>
        /// The <see cref="LinearStateEstimator.Modeling.Substation"/> where the <see cref="LinearStateEstimator.Modeling.TransmissionLine"/> is said to terminate.
        /// </summary>
        [XmlIgnore()]
        public Substation ToSubstation
        {
            get
            {
                return m_toSubstation;
            }
            set
            {
                m_toSubstation = value;
                m_toSubstationID = value.InternalID;
            }
        }

        /// <summary>
        /// The <see cref="LinearStateEstimator.Modeling.INetworkDescribable.InternalID"/> of the <see cref="LinearStateEstimator.Modeling.Substation"/> where the <see cref="LinearStateEstimator.Modeling.TransmissionLine"/> is said to terminate.
        /// </summary>
        [XmlAttribute("ToSubstation")]
        public int ToSubstationID
        {
            get
            {
                return m_toSubstationID;
            }
            set
            {
                m_toSubstationID = value;
            }
        }

        /// <summary>
        /// The current flow on the <see cref="LinearStateEstimator.Modeling.TransmissionLine"/> as measured from the <see cref="LinearStateEstimator.Modeling.TransmissionLine.FromSubstation"/>.
        /// </summary>
        [XmlElement("FromSubstationCurrent")]
        public CurrentFlowPhasorGroup FromSubstationCurrent
        {
            get 
            { 
                return m_fromSubstationCurrent; 
            }
            set 
            { 
                m_fromSubstationCurrent = value;
                m_fromSubstationCurrentID = value.InternalID;
                if (FromNode != null)
                {
                    m_fromSubstationCurrent.MeasuredFromNode = FromNode;
                }
            }
        }

        /// <summary>
        /// The <see cref="LinearStateEstimator.Modeling.INetworkDescribable.InternalID"/> of the current flow on the <see cref="LinearStateEstimator.Modeling.TransmissionLine"/> as measured from the <see cref="LinearStateEstimator.Modeling.TransmissionLine.FromSubstation"/>.
        /// </summary>
        [XmlAttribute("FromSubstationCurrentPhasorGroup")]
        public int FromSubstationCurrentID
        {
            get
            {
                return m_fromSubstationCurrentID;
            }
            set
            {
                m_fromSubstationCurrentID = value;
            }
        }

        /// <summary>
        /// The current flow on the <see cref="LinearStateEstimator.Modeling.TransmissionLine"/> as measured from the <see cref="LinearStateEstimator.Modeling.TransmissionLine.ToSubstation"/>.
        /// </summary>
        [XmlElement("ToSubstationCurrent")]
        public CurrentFlowPhasorGroup ToSubstationCurrent
        {
            get 
            { 
                return m_toSubstationCurrent; 
            }
            set 
            { 
                m_toSubstationCurrent = value;
                m_toSubstationCurrentID = value.InternalID;
                if (this.ToNode != null)
                {
                    m_toSubstationCurrent.MeasuredToNode = this.ToNode;
                }
            }
        }

        /// <summary>
        /// The <see cref="LinearStateEstimator.Modeling.INetworkDescribable.InternalID"/> current flow on the <see cref="LinearStateEstimator.Modeling.TransmissionLine"/> as measured from the <see cref="LinearStateEstimator.Modeling.TransmissionLine.ToSubstation"/>.
        /// </summary>
        [XmlAttribute("ToSubstationCurrentPhasorGroup")]
        public int ToSubstationCurrentID
        {
            get
            {
                return m_toSubstationCurrentID;
            }
            set
            {
                m_toSubstationCurrentID = value;
            }
        }

        /// <summary>
        /// A boolean flag which indicates whether the algorithm for inferring the status of series compensation devices has been enabled or not.
        /// </summary>
        [XmlAttribute("SeriesCompensatorInferenceIsEnabled")]
        public bool SeriesCompensatorStatusInferenceIsEnabled
        {
            get
            {
                return m_seriesCompensationStatusInferenceIsEnabled;
            }
            set
            {
                m_seriesCompensationStatusInferenceIsEnabled = value;
            }
        }

        /// <summary>
        /// A boolean flag which indicates whether the <see cref="LinearStateEstimator.Modeling.TransmissionLine"/> is capable of performing this procedure based on the quality of its terminal measurements.
        /// </summary>
        [XmlIgnore()]
        public bool CanPerformSeriesCompensatorStatusInference
        {
            get
            {
                return SeriesCompensatorStatusInferenceIsEnabled && CanPerformRealTimeImpedanceCalculation;
            }
        }

        /// <summary>
        /// A boolean flag which indicates whether the <see cref="LinearStateEstimator.Modeling.TransmissionLine"/> will perform this procedure based on its ability and whether or not is has been enabled.
        /// </summary>
        [XmlIgnore()]
        public bool WillPerformSeriesCompensatorStatusInference
        {
            get
            {
                return m_seriesCompensationStatusInferenceIsEnabled && CanPerformSeriesCompensatorStatusInference;
            }
        }

        /// <summary>
        /// The number of possible combinations of series impedances. Equal to 2^n where n is the total number of possible series compensators to include in the problem. If a series compensator is directly bypassed by topology, it is not included in the collection totalling n in quantity.
        /// </summary>
        [XmlIgnore()]
        public int NumberOfPossibleSeriesImpedanceStates
        {
            get
            {
                // Computes 2^nwhere n is number of possible compensators
                int numberOfPossibleSeriesImpedanceStates = 1;
                for (int i = 0; i < m_numberOfPossibleSeriesCompensators; i++)
                {
                    numberOfPossibleSeriesImpedanceStates *= 2;
                }
                return numberOfPossibleSeriesImpedanceStates;
            }
        }

        /// <summary>
        /// A list of all of the possible impedance values for the transmission line based on the possible states of all series compensators.
        /// </summary>
        [XmlIgnore()]
        public List<Impedance> PossibleImpedanceValues
        {
            get
            {
                return m_possibleImpedanceValues;
            }
        }

        /// <summary>
        /// A lumped impedance of the series branches that make up the single flow path from one end to the other excluding bypassed series compensators.
        /// </summary>
        [XmlIgnore()]
        public Impedance InferredTotalImpedance
        {
            get
            {
                return m_inferredTotalImpedance;
            }
        }

        /// <summary>
        /// The impedance of the transmission line as seen by its <b>From</b> terminal to the deepest observable node.
        /// </summary>
        [XmlIgnore()]
        public Impedance FromSideImpedanceToDeepestObservability
        {
            get
            {
                return m_fromSideImpedanceToDeepestObservability;
            }
            set
            {
                m_fromSideImpedanceToDeepestObservability = value;
            }
        }

        /// <summary>
        /// The impedance of the transmission line as seen by its <b>To</b> terminal to the deepest observable node.
        /// </summary>
        [XmlIgnore()]
        public Impedance ToSideImpedanceToDeepestObservability
        {
            get
            {
                return m_toSideImpedanceToDeepestObservability;
            }
            set
            {
                m_toSideImpedanceToDeepestObservability = value;
            }
        }

        /// <summary>
        /// The complex power flow through the transmission line as measured at its <b>From</b> terminal.
        /// </summary>
        [XmlIgnore()]
        public ComplexPowerGroup FromSubstationComplexPower
        {
            get
            {
                if (m_complexPowerHasBeenInitialized)
                {
                    return m_fromSubstationComplexPower;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (m_complexPowerHasBeenInitialized)
                {
                    m_fromSubstationComplexPower = value;
                }
            }
        }

        /// <summary>
        /// The complex power flow through the transmission line as measured at its <b>To</b> terminal.
        /// </summary>
        [XmlIgnore()]
        public ComplexPowerGroup ToSubstationComplexPower
        {
            get
            {
                return m_toSubstationComplexPower;
            }
            set
            {
                m_toSubstationComplexPower = value;
            }
        }

        /// <summary>
        /// The real power (P) losses on the transmission line in positive sequence quantities
        /// </summary>
        [XmlIgnore()]
        public double PositiveSequenceRealPowerLosses
        {
            get
            {
                return Math.Abs(FromSubstationComplexPower.PositiveSequenceRealPower - ToSubstationComplexPower.PositiveSequenceRealPower);
            }
        }

        /// <summary>
        /// The reactive power (Q) losses on the transmission line in positive sequence quantities
        /// </summary>
        [XmlIgnore()]
        public double PositiveSequenceReactivePowerLosses
        {
            get
            {
                return Math.Abs(FromSubstationComplexPower.PositiveSequenceReactivePower - ToSubstationComplexPower.PositiveSequenceReactivePower);
            }
        }

        /// <summary>
        /// The real power losses (P) on the transmission line in three phase quantities
        /// </summary>
        [XmlIgnore()]
        public double ThreePhaseRealPowerLosses
        {
            get
            {
                return Math.Abs(FromSubstationComplexPower.ThreePhaseRealPower - ToSubstationComplexPower.ThreePhaseRealPower);
            }
        }

        /// <summary>
        /// The reactive power losses (Q) on the transmission line in three phase quantities
        /// </summary>
        [XmlIgnore()]
        public double ThreePhaseReactivePowerLosses
        {
            get
            {
                return Math.Abs(FromSubstationComplexPower.ThreePhaseReactivePower - ToSubstationComplexPower.ThreePhaseReactivePower);
            }
        }

        /// <summary>
        /// The graph representation of the nodal structure of the <see cref="LinearStateEstimator.Modeling.TransmissionLine"/>
        /// </summary>
        [XmlIgnore()]
        public TransmissionLineGraph Graph
        {
            get
            {
                return m_graph;
            }
            set
            {
                m_graph = value;
            }
        }

        /// <summary>
        /// A boolean flag which indicates whether the <see cref="LinearStateEstimator.Modeling.TransmissionLine"/> is capable of performing this procedure based on the quality of its terminal measurements.
        /// </summary>
        [XmlIgnore()]
        public bool CanPerformRealTimeImpedanceCalculation
        {
            get
            {
                return BothSidesHaveActiveVoltageMeasurements && (FromSideHasActiveCurrentFlowMeasurement || ToSideHasActiveCurrentFlowMeasurement);
            }
        }

        /// <summary>
        /// A boolean flag which indicates whether the <see cref="LinearStateEstimator.Modeling.TransmissionLine"/> will perform this procedure based on its ability and whether or not is has been enabled.
        /// </summary>
        [XmlIgnore()]
        public bool WillPerformRealTimeImpedanceCalculation
        {
            get
            {
                return CanPerformRealTimeImpedanceCalculation && (m_realTimeImpedanceCalculationIsEnabled || m_seriesCompensationStatusInferenceIsEnabled);
            }
        }

        /// <summary>
        /// The positive sequence equivalent impedance as calculated by real-time measurements.
        /// </summary>
        [XmlIgnore()]
        public Impedance RealTimeCalculatedImpedance
        {
            get
            {
                return m_realTimeCalculatedImpedance;
            }
        }

        /// <summary>
        /// The positive sequence equivalent impedance as calculated by real-time measurements in the previous frame.
        /// </summary>
        [XmlIgnore()]
        public Impedance PreviousRealTimeCalcualtedImpedance
        {
            get
            {
                return m_previousRealTimeCalculatedImpedance;
            }
        }

        /// <summary>
        /// A boolean flag which indicates whether the algorithm for calculating the real time impedance has been enabled or not.
        /// </summary>
        [XmlAttribute("RealTimeImpedanceCalculationIsEnabled")]
        public bool RealTimeImpedanceCalculationIsEnabled
        {
            get
            {
                return m_realTimeImpedanceCalculationIsEnabled;
            }
            set
            {
                m_realTimeImpedanceCalculationIsEnabled = value;
            }
        }

        /// <summary>
        /// A threshold that below which, the difference in present and previous calculated impedance will not appear as a change.
        /// </summary>
        [XmlAttribute("ImpedanceChangeThreshold")]
        public double CalculatedImpedanceChangeThreshold
        {
            get
            {
                return m_calculatedImpedanceChangeThreshold;
            }
            set
            {
                m_calculatedImpedanceChangeThreshold = value;
            }
        }
        
        /// <summary>
        /// A boolean flag which indicates if the difference in the calculated real-time impedance has changed more than a pre-defined threshold since the last frame. Indicates a possible change in state of the overall line.
        /// </summary>
        [XmlIgnore()]
        public bool RealTimeImpedanceHasChanged
        {
            get
            {
                if ((m_realTimeCalculatedImpedance.PositiveSequenceSeriesImpedance - m_previousRealTimeCalculatedImpedance.PositiveSequenceSeriesImpedance).Magnitude > m_calculatedImpedanceChangeThreshold)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        [XmlIgnore()]
        private bool BothSidesHaveActiveVoltageMeasurements
        {
            get
            {
                if (m_parentDivision.ParentCompany.ParentModel.PhaseConfiguration == PhaseSelection.PositiveSequence)
                {
                    return m_fromNode.Voltage.IncludeInPositiveSequenceEstimator && m_toNode.Voltage.IncludeInPositiveSequenceEstimator;
                }
                else if (m_parentDivision.ParentCompany.ParentModel.PhaseConfiguration == PhaseSelection.ThreePhase)
                {
                    return m_fromNode.Voltage.IncludeInEstimator && m_toNode.Voltage.IncludeInEstimator;
                }
                return false;
            }
        }

        [XmlIgnore()]
        private bool FromSideHasActiveCurrentFlowMeasurement
        {
            get
            {
                if (m_parentDivision.ParentCompany.ParentModel.PhaseConfiguration == PhaseSelection.PositiveSequence)
                {
                    return m_fromSubstationCurrent.IncludeInPositiveSequenceEstimator;
                }
                else if (m_parentDivision.ParentCompany.ParentModel.PhaseConfiguration == PhaseSelection.ThreePhase)
                {
                    return m_fromSubstationCurrent.IncludeInEstimator;
                }
                return false;
            }
        }

        [XmlIgnore()]
        private bool ToSideHasActiveCurrentFlowMeasurement
        {
            get
            {
                if (m_parentDivision.ParentCompany.ParentModel.PhaseConfiguration == PhaseSelection.PositiveSequence)
                {
                    return m_toSubstationCurrent.IncludeInPositiveSequenceEstimator;
                }
                else if (m_parentDivision.ParentCompany.ParentModel.PhaseConfiguration == PhaseSelection.ThreePhase)
                {
                    return m_toSubstationCurrent.IncludeInPositiveSequenceEstimator;
                }
                return false;
            }
        }

        [XmlIgnore()]
        private bool BothSidesHaveActiveCurrentFlowMeasurements
        {
            get
            {
                return FromSideHasActiveCurrentFlowMeasurement && ToSideHasActiveCurrentFlowMeasurement;
            }
        }

        [XmlIgnore()]
        public bool InPruningMode
        {
            get
            {
                return m_parentDivision.ParentCompany.ParentModel.InPruningMode;
            }
        }

        [XmlIgnore()]
        public bool RetainWhenPruning
        {
            get
            {
                if (InPruningMode)
                {
                    if (m_fromSubstation.RetainWhenPruning && m_toSubstation.RetainWhenPruning)
                    {
                        return true;
                    }
                    return false;
                }
                return true;
            }
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// A blank constructor with default parameters.
        /// </summary>
        public TransmissionLine()
            :this(DEFAULT_INTERNALID, DEFAULT_NUMBER, DEFAULT_ACRONYM, DEFAULT_NAME, DEFAULT_DESCRIPTION)
        {
        }

        /// <summary>
        /// A constructor for the <see cref="LinearStateEstimator.Modeling.TransmissionLine"/> class which requires all of the properties defined by the <see cref="LinearStateEstimator.Modeling.INetworkDescribable"/> interface.
        /// </summary>
        /// <param name="internalID">An integer identifier for each <see cref="LinearStateEstimator.Modeling.TransmissionLine"/> which is intended to be unique among other objects of the same type.</param>
        /// <param name="number">A descriptive integer for the instance of the <see cref="LinearStateEstimator.Modeling.TransmissionLine"/>. There are no restrictions on uniqueness. </param>
        /// <param name="acronym">A string acronym for the instance of the <see cref="LinearStateEstimator.Modeling.TransmissionLine"/>.</param>
        /// <param name="name">The string name of the instance of the <see cref="LinearStateEstimator.Modeling.TransmissionLine"/>.</param>
        /// <param name="description">A string description of the instance of the <see cref="LinearStateEstimator.Modeling.TransmissionLine"/>.</param>
        public TransmissionLine(int internalID, int number, string acronym, string name, string description)
            :this(internalID, number, acronym, name, description, null, null)
        {
        }

        /// <summary>
        /// A constructor for the <see cref="LinearStateEstimator.Modeling.TransmissionLine"/> class which requires all of the properties defined by the <see cref="LinearStateEstimator.Modeling.INetworkDescribable"/> interface as well as the From <see cref="LinearStateEstimator.Modeling.Substation"/> and the To <see cref="LinearStateEstimator.Modeling.Substation"/>.
        /// </summary>
        /// <param name="internalID">An integer identifier for each <see cref="LinearStateEstimator.Modeling.TransmissionLine"/> which is intended to be unique among other objects of the same type.</param>
        /// <param name="number">A descriptive integer for the instance of the <see cref="LinearStateEstimator.Modeling.TransmissionLine"/>. There are no restrictions on uniqueness. </param>
        /// <param name="acronym">A string acronym for the instance of the <see cref="LinearStateEstimator.Modeling.TransmissionLine"/>.</param>
        /// <param name="name">The string name of the instance of the <see cref="LinearStateEstimator.Modeling.TransmissionLine"/>.</param>
        /// <param name="description">A string description of the instance of the <see cref="LinearStateEstimator.Modeling.TransmissionLine"/>.</param>
        /// <param name="fromSubstation">The <see cref="LinearStateEstimator.Modeling.Substation"/> where the <see cref="LinearStateEstimator.Modeling.TransmissionLine"/> is said to originate.</param>
        /// <param name="toSubstation">The <see cref="LinearStateEstimator.Modeling.Substation"/> where the <see cref="LinearStateEstimator.Modeling.TransmissionLine"/> is said to terminate.</param>
        public TransmissionLine(int internalID, int number, string acronym, string name, string description, Substation fromSubstation, Substation toSubstation)
        {
            m_internalID = internalID;
            m_number = number;
            m_acronym = acronym;
            m_name = name;
            m_description = description;
            m_fromSubstation = fromSubstation;
            m_toSubstation = toSubstation;
            m_childrenLineSegments = new List<LineSegment>();
            m_childrenSeriesCompensators = new List<SeriesCompensator>();
            m_childrenNodes = new List<Node>();
            m_childrenSwitches = new List<Switch>();
            m_complexPowerHasBeenInitialized = false;
            m_realTimeCalculatedImpedance = new Impedance();
            m_previousRealTimeCalculatedImpedance = new Impedance();
        }

        #endregion

        #region [ Public Methods ]

        /// <summary>
        /// Sets the required references between the node voltages and current flows for computing complex power flows.
        /// </summary>
        public void InitializeComplexPower()
        {
            if (m_fromNode != null && m_toNode != null)
            {
                m_fromSubstationComplexPower = new ComplexPowerGroup(FromNode.Voltage, FromSubstationCurrent);
                m_fromSubstationComplexPower.UseEstimatedValues = true;

                m_toSubstationComplexPower = new ComplexPowerGroup(ToNode.Voltage, ToSubstationCurrent);
                m_toSubstationComplexPower.UseEstimatedValues = true;

                m_complexPowerHasBeenInitialized = true;
            }
        }

        /// <summary>
        /// Initializes the <see cref="LinearStateEstimator.Modeling.TransmissionLine.Graph"/>.
        /// </summary>
        public void InitializeGraph()
        {
            m_graph = new TransmissionLineGraph(this);
        }

        /// <summary>
        /// Creates a list of <see cref="LinearStateEstimator.Modeling.Impedance"/>s which represent all of the possible values for impedance based on the model and topology processing.
        /// </summary>
        public void InitializePossibleImpedanceValues()
        {
            m_possibleImpedanceValues = new List<Impedance>();

            Impedance baseCaseImpedance = ComputeBaseCaseImpedance();

            List<SeriesBranchBase> singleFlowPath = m_graph.SingleFlowPathBranches;

            for (int impedanceStateIndex = 0; impedanceStateIndex < NumberOfPossibleSeriesImpedanceStates; impedanceStateIndex++)
            {
                Impedance impedance = baseCaseImpedance.Copy();

                foreach (SeriesCompensator seriesCompensator in m_childrenSeriesCompensators)
                {
                    if (singleFlowPath.Contains(seriesCompensator as SeriesBranchBase))
                    {
                        int seriesCompensatorIndex = m_childrenSeriesCompensators.IndexOf(seriesCompensator);
                        if ((impedanceStateIndex & Convert.ToInt16(Math.Pow(2, seriesCompensatorIndex))) > 0)
                        {
                            impedance += seriesCompensator.RawImpedanceParameters;
                        }
                    }
                }

                m_possibleImpedanceValues.Add(impedance);
            }
        }

        /// <summary>
        /// Runs the necessary topology processing
        /// </summary>
        public void RunTopologyProcessing()
        {
            InitializeGraph();
            m_graph.InitializeAdjacencyLists();
            m_graph.ResolveConnectedAdjacencies();
            if (m_graph.HasAtLeastOneFlowPath)
            {
                m_graph.InitializeTree();
            }
        }
        
        /// <summary>
        /// Runs the algorithms for inferring the status of the series capacitors on the transmission line
        /// </summary>
        public void RunSeriesCompensatorStatusInference()
        {
            if (WillPerformSeriesCompensatorStatusInference)
            {
                InitializePossibleImpedanceValues();
                m_inferredTotalImpedance = GetClosestMatchingImpedanceValueAndAssignInferredStatusToSeriesCompensators();
            }
        }

        /// <summary>
        /// Computes the real time positive sequence impedance value based on phasor measurements.
        /// </summary>
        public void ComputeRealTimePositiveSequenceImpedance()
        {
            if (CanPerformRealTimeImpedanceCalculation)
            {
                if (BothSidesHaveActiveCurrentFlowMeasurements && BothSidesHaveActiveVoltageMeasurements)
                {
                    ComputeRealTimePositiveSequenceImpedanceWithBothVoltagesAndBothCurrentFlows();
                }
                else if (FromSideHasActiveCurrentFlowMeasurement && BothSidesHaveActiveVoltageMeasurements)
                {
                    ComputeRealTimePositiveSequenceImpedanceWithBothVoltagesAndFromSideCurrentFlow();
                }
                else if (ToSideHasActiveCurrentFlowMeasurement && BothSidesHaveActiveVoltageMeasurements)
                {
                    ComputeRealTimePositiveSequenceImpedanceWithBothVoltagesAndToSideCurrentFlow();
                }
            }
        }

        /// <summary>
        /// Computes the estimated positive sequence current flow from estimated positive sequence voltages and impedances.
        /// </summary>
        public void ComputePositiveSequenceEstimatedCurrentFlow()
        {
            if (m_parentDivision.ParentCompany.ParentModel.CurrentFlowPostProcessingSetting == CurrentFlowPostProcessingSetting.ProcessOnlyMeasuredBranches)
            {
                if ((FromSubstationCurrent.IncludeInPositiveSequenceEstimator || ToSubstationCurrent.IncludeInPositiveSequenceEstimator) && FromNode.IsObserved && ToNode.IsObserved)
                {
                    Complex Vi = FromNode.Voltage.PositiveSequence.Estimate.PerUnitComplexPhasor;
                    Complex Vj = ToNode.Voltage.PositiveSequence.Estimate.PerUnitComplexPhasor;

                    Complex Iij = (m_inferredTotalImpedance.PositiveSequenceSeriesAdmittance + m_inferredTotalImpedance.PositiveSequenceSingleSidedShuntSusceptance) * Vi - m_inferredTotalImpedance.PositiveSequenceSeriesAdmittance * Vj;
                    Complex Iji = (m_inferredTotalImpedance.PositiveSequenceSeriesAdmittance + m_inferredTotalImpedance.PositiveSequenceSingleSidedShuntSusceptance) * Vj - m_inferredTotalImpedance.PositiveSequenceSeriesAdmittance * Vi;

                    FromSubstationCurrent.PositiveSequence.Estimate.PerUnitComplexPhasor = Iij;
                    ToSubstationCurrent.PositiveSequence.Estimate.PerUnitComplexPhasor = Iji;
                }
                else
                {
                    FromSubstationCurrent.PositiveSequence.Estimate.Magnitude = 0;
                    FromSubstationCurrent.PositiveSequence.Estimate.AngleInDegrees = 0;
                    ToSubstationCurrent.PositiveSequence.Estimate.Magnitude = 0;
                    ToSubstationCurrent.PositiveSequence.Estimate.AngleInDegrees = 0;
                }
            }
            else if (m_parentDivision.ParentCompany.ParentModel.CurrentFlowPostProcessingSetting == CurrentFlowPostProcessingSetting.ProcessBranchesByNodeObservability)
            {
                if (FromNode.IsObserved && ToNode.IsObserved)
                {
                    Complex Vi = FromNode.Voltage.PositiveSequence.Estimate.PerUnitComplexPhasor;
                    Complex Vj = ToNode.Voltage.PositiveSequence.Estimate.PerUnitComplexPhasor;

                    Complex Iij = (m_inferredTotalImpedance.PositiveSequenceSeriesAdmittance + m_inferredTotalImpedance.PositiveSequenceSingleSidedShuntSusceptance) * Vi - m_inferredTotalImpedance.PositiveSequenceSeriesAdmittance * Vj;
                    Complex Iji = (m_inferredTotalImpedance.PositiveSequenceSeriesAdmittance + m_inferredTotalImpedance.PositiveSequenceSingleSidedShuntSusceptance) * Vj - m_inferredTotalImpedance.PositiveSequenceSeriesAdmittance * Vi;

                    FromSubstationCurrent.PositiveSequence.Estimate.PerUnitComplexPhasor = Iij;
                    ToSubstationCurrent.PositiveSequence.Estimate.PerUnitComplexPhasor = Iji;
                }
                else
                {
                    FromSubstationCurrent.PositiveSequence.Estimate.Magnitude = 0;
                    FromSubstationCurrent.PositiveSequence.Estimate.AngleInDegrees = 0;
                    ToSubstationCurrent.PositiveSequence.Estimate.Magnitude = 0;
                    ToSubstationCurrent.PositiveSequence.Estimate.AngleInDegrees = 0;
                }
            }
        }

        /// <summary>
        /// Computes estimated three phase current flows from estimated three phase voltages and impedances.
        /// </summary>
        public void ComputeThreePhaseEstimatedCurrentFlow()
        {
            if (m_parentDivision.ParentCompany.ParentModel.CurrentFlowPostProcessingSetting == CurrentFlowPostProcessingSetting.ProcessOnlyMeasuredBranches)
            {
                if ((FromSubstationCurrent.IncludeInEstimator || ToSubstationCurrent.IncludeInEstimator) && FromNode.IsObserved && ToNode.IsObserved)
                {
                    DenseMatrix Vi = DenseMatrix.OfArray(new Complex[3, 1]);
                    DenseMatrix Vj = DenseMatrix.OfArray(new Complex[3, 1]);

                    Vi[0, 0] = FromNode.Voltage.PhaseA.Estimate.PerUnitComplexPhasor;
                    Vi[1, 0] = FromNode.Voltage.PhaseB.Estimate.PerUnitComplexPhasor;
                    Vi[2, 0] = FromNode.Voltage.PhaseC.Estimate.PerUnitComplexPhasor;

                    Vj[0, 0] = ToNode.Voltage.PhaseA.Estimate.PerUnitComplexPhasor;
                    Vj[1, 0] = ToNode.Voltage.PhaseB.Estimate.PerUnitComplexPhasor;
                    Vj[2, 0] = ToNode.Voltage.PhaseC.Estimate.PerUnitComplexPhasor;

                    DenseMatrix Iij = (m_inferredTotalImpedance.ThreePhaseSeriesAdmittance + m_inferredTotalImpedance.ThreePhaseSingleSidedShuntSusceptance) * Vi - m_inferredTotalImpedance.ThreePhaseSeriesAdmittance * Vj;
                    DenseMatrix Iji = (m_inferredTotalImpedance.ThreePhaseSeriesAdmittance + m_inferredTotalImpedance.ThreePhaseSingleSidedShuntSusceptance) * Vj - m_inferredTotalImpedance.ThreePhaseSeriesAdmittance * Vi;

                    FromSubstationCurrent.PhaseA.Estimate.PerUnitComplexPhasor = Iij[0, 0];
                    FromSubstationCurrent.PhaseB.Estimate.PerUnitComplexPhasor = Iij[1, 0];
                    FromSubstationCurrent.PhaseC.Estimate.PerUnitComplexPhasor = Iij[2, 0];

                    ToSubstationCurrent.PhaseA.Estimate.PerUnitComplexPhasor = Iji[0, 0];
                    ToSubstationCurrent.PhaseB.Estimate.PerUnitComplexPhasor = Iji[1, 0];
                    ToSubstationCurrent.PhaseC.Estimate.PerUnitComplexPhasor = Iji[2, 0];
                }
                else
                {
                    FromSubstationCurrent.PhaseA.Estimate.Magnitude = 0;
                    FromSubstationCurrent.PhaseA.Estimate.AngleInDegrees = 0;
                    ToSubstationCurrent.PhaseA.Estimate.Magnitude = 0;
                    ToSubstationCurrent.PhaseA.Estimate.AngleInDegrees = 0;

                    FromSubstationCurrent.PhaseB.Estimate.Magnitude = 0;
                    FromSubstationCurrent.PhaseB.Estimate.AngleInDegrees = 0;
                    ToSubstationCurrent.PhaseB.Estimate.Magnitude = 0;
                    ToSubstationCurrent.PhaseB.Estimate.AngleInDegrees = 0;

                    FromSubstationCurrent.PhaseC.Estimate.Magnitude = 0;
                    FromSubstationCurrent.PhaseC.Estimate.AngleInDegrees = 0;
                    ToSubstationCurrent.PhaseC.Estimate.Magnitude = 0;
                    ToSubstationCurrent.PhaseC.Estimate.AngleInDegrees = 0;
                }
            }
            else if (m_parentDivision.ParentCompany.ParentModel.CurrentFlowPostProcessingSetting == CurrentFlowPostProcessingSetting.ProcessBranchesByNodeObservability)
            {
                if (FromNode.IsObserved && ToNode.IsObserved)
                {
                    DenseMatrix Vi = DenseMatrix.OfArray(new Complex[3, 1]);
                    DenseMatrix Vj = DenseMatrix.OfArray(new Complex[3, 1]);

                    Vi[0, 0] = FromNode.Voltage.PhaseA.Estimate.PerUnitComplexPhasor;
                    Vi[1, 0] = FromNode.Voltage.PhaseB.Estimate.PerUnitComplexPhasor;
                    Vi[2, 0] = FromNode.Voltage.PhaseC.Estimate.PerUnitComplexPhasor;

                    Vj[0, 0] = ToNode.Voltage.PhaseA.Estimate.PerUnitComplexPhasor;
                    Vj[1, 0] = ToNode.Voltage.PhaseB.Estimate.PerUnitComplexPhasor;
                    Vj[2, 0] = ToNode.Voltage.PhaseC.Estimate.PerUnitComplexPhasor;

                    DenseMatrix Iij = (m_inferredTotalImpedance.ThreePhaseSeriesAdmittance + m_inferredTotalImpedance.ThreePhaseSingleSidedShuntSusceptance) * Vi - m_inferredTotalImpedance.ThreePhaseSeriesAdmittance * Vj;
                    DenseMatrix Iji = (m_inferredTotalImpedance.ThreePhaseSeriesAdmittance + m_inferredTotalImpedance.ThreePhaseSingleSidedShuntSusceptance) * Vj - m_inferredTotalImpedance.ThreePhaseSeriesAdmittance * Vi;

                    FromSubstationCurrent.PhaseA.Estimate.PerUnitComplexPhasor = Iij[0, 0];
                    FromSubstationCurrent.PhaseB.Estimate.PerUnitComplexPhasor = Iij[1, 0];
                    FromSubstationCurrent.PhaseC.Estimate.PerUnitComplexPhasor = Iij[2, 0];

                    ToSubstationCurrent.PhaseA.Estimate.PerUnitComplexPhasor = Iji[0, 0];
                    ToSubstationCurrent.PhaseB.Estimate.PerUnitComplexPhasor = Iji[1, 0];
                    ToSubstationCurrent.PhaseC.Estimate.PerUnitComplexPhasor = Iji[2, 0];
                }
                else
                {
                    FromSubstationCurrent.PhaseA.Estimate.Magnitude = 0;
                    FromSubstationCurrent.PhaseA.Estimate.AngleInDegrees = 0;
                    ToSubstationCurrent.PhaseA.Estimate.Magnitude = 0;
                    ToSubstationCurrent.PhaseA.Estimate.AngleInDegrees = 0;

                    FromSubstationCurrent.PhaseB.Estimate.Magnitude = 0;
                    FromSubstationCurrent.PhaseB.Estimate.AngleInDegrees = 0;
                    ToSubstationCurrent.PhaseB.Estimate.Magnitude = 0;
                    ToSubstationCurrent.PhaseB.Estimate.AngleInDegrees = 0;

                    FromSubstationCurrent.PhaseC.Estimate.Magnitude = 0;
                    FromSubstationCurrent.PhaseC.Estimate.AngleInDegrees = 0;
                    ToSubstationCurrent.PhaseC.Estimate.Magnitude = 0;
                    ToSubstationCurrent.PhaseC.Estimate.AngleInDegrees = 0;
                }
            }
        }

        /// <summary>
        /// This method sets the lumped impedance value of the transmission line based on topology processing and series compensator inference if enabled.
        /// </summary>
        public void SetFinalImpedanceValues()
        {
            // This is where the total line segment value will be set after topology processing.
            if (m_graph.HasAtLeastOneFlowPath)
            {
                Impedance lumpedImpedance = new Impedance();

                foreach (SeriesBranchBase seriesBranch in m_graph.SingleFlowPathBranches)
                {
                    if ((seriesBranch is SeriesCompensator))
                    {
                        if ((seriesBranch as SeriesCompensator).Status == SeriesCompensatorStatus.Energized)
                        {
                            lumpedImpedance += seriesBranch.RawImpedanceParameters;
                        }
                    }
                    else if (seriesBranch is LineSegment)
                    {
                        lumpedImpedance += seriesBranch.RawImpedanceParameters;
                    }
                }

                m_fromSideImpedanceToDeepestObservability = lumpedImpedance;
                m_toSideImpedanceToDeepestObservability = lumpedImpedance;
                m_inferredTotalImpedance = lumpedImpedance;
            }
        }

        /// <summary>
        /// A string representation of an instance of the <see cref="TransmissionLine"/> class.
        /// </summary>
        /// <returns>A string representation of an instance of the <see cref="TransmissionLine"/> class.</returns>
        public override string ToString()
        {
            return "TransmissionLine," + m_internalID.ToString() + "," + m_number.ToString() + "," + m_acronym + "," + m_name + "," + m_description + "," + m_parentDivision.InternalID.ToString() + "," + m_fromSubstationID.ToString() + "," + m_fromNodeID.ToString() + "," + m_fromSubstationCurrentID.ToString() + "," + m_toSubstationID.ToString() + "," + m_toNodeID.ToString() + "," + m_toSubstationCurrentID.ToString();
        }

        /// <summary>
        /// A verbose string representation of an instance of the <see cref="TransmissionLine"/> class.
        /// </summary>
        /// <returns>A verbose string representation of an instance of the <see cref="TransmissionLine"/> class.</returns>
        public string ToVerboseString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine();
            stringBuilder.AppendFormat("----- Transmission Line --------------------------------------------------------");
            stringBuilder.AppendLine();
            stringBuilder.AppendFormat("      InternalID: " + m_internalID.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("          Number: " + m_number.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("         Acronym: " + m_acronym + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("            Name: " + m_name + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("     Description: " + m_description + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("  FromSubstation: " + m_fromSubstation.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("    ToSubstation: " + m_toSubstation.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("        FromNode: " + FromNode.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("          ToNode: " + ToNode.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("  ParentDivision: " + m_parentDivision.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("Can Perform RTIC: " + CanPerformRealTimeImpedanceCalculation.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("  Both Voltages?: " + BothSidesHaveActiveVoltageMeasurements.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("FromSideCurrent?: " + FromSideHasActiveCurrentFlowMeasurement.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("To Side Current?: " + ToSideHasActiveCurrentFlowMeasurement.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat(" Can Perform SCI: " + CanPerformSeriesCompensatorStatusInference.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("Will Perform SCI: " + WillPerformSeriesCompensatorStatusInference.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("           Nodes:  {0}", Environment.NewLine);
            stringBuilder.AppendLine();
            foreach (Node node in m_childrenNodes)
            {
                stringBuilder.AppendFormat(node.ToString() + "{0}", Environment.NewLine);
            }
            stringBuilder.AppendFormat("    Transformers:  {0}", Environment.NewLine);
            stringBuilder.AppendLine();
            foreach (LineSegment lineSegments in m_childrenLineSegments)
            {
                stringBuilder.AppendFormat(lineSegments.ToString() + "{0}", Environment.NewLine);
            }
            stringBuilder.AppendFormat("        Switches:  {0}", Environment.NewLine);
            stringBuilder.AppendLine();
            foreach (Switch circuitSwitch in m_childrenSwitches)
            {
                stringBuilder.AppendFormat(circuitSwitch.ToString() + "{0}", Environment.NewLine);
            }
            stringBuilder.AppendLine();
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Creates a deep copy of the <see cref="LinearStateEstimator.Modeling.TransmissionLine"/> object.
        /// </summary>
        /// <returns>A deep copy of the <see cref="LinearStateEstimator.Modeling.TransmissionLine"/> object.</returns>
        public TransmissionLine Copy()
        {
            TransmissionLine copy = (TransmissionLine)this.MemberwiseClone();
            // Incomplete
            return copy;
        }

        #endregion

        #region [ Private Methods ]

        private Impedance ComputeBaseCaseImpedance()
        {
            List<SeriesBranchBase> singleFlowPath = m_graph.SingleFlowPathBranches;
            m_numberOfPossibleSeriesCompensators = 0;
            // Calculate the base case with no series compensation
            Impedance impedance = new Impedance();

            foreach (SeriesBranchBase seriesBranch in singleFlowPath)
            {
                if (seriesBranch is SeriesCompensator)
                {
                    m_numberOfPossibleSeriesCompensators++;
                }
                else if (seriesBranch is LineSegment)
                {
                    impedance += seriesBranch.RawImpedanceParameters;
                }
            }

            return impedance;
        }

        /// <summary>
        /// Finds the <see cref="LinearStateEstimator.Modeling.Impedance"/> that is closest in value to the real-time calculated impedance.
        /// </summary>
        /// <returns>The <see cref="LinearStateEstimator.Modeling.Impedance"/> from the set of possible values which is closest in value to the real-time calculated impedance.</returns>
        private Impedance GetClosestMatchingImpedanceValueAndAssignInferredStatusToSeriesCompensators()
        {
            List<Impedance> impedanceDifferences = new List<Impedance>();

            // Compute the difference between the real time impedance and all of the possible states
            foreach (Impedance possibleImpedance in m_possibleImpedanceValues)
            {
                impedanceDifferences.Add(possibleImpedance - m_realTimeCalculatedImpedance);
            }

            // Use the index of the lowest valued difference to find the associated impedance state
            int impedanceStateIndex = impedanceDifferences.IndexOf(impedanceDifferences.Min());

            // Use the impedance state index to set the inferred status of the series compensators
            AssignInferredStatusToSeriesCompensators(impedanceStateIndex);

            return m_possibleImpedanceValues[impedanceStateIndex];
        }

        private void AssignInferredStatusToSeriesCompensators(int impedanceStateIndex)
        {
            List<SeriesBranchBase> singleFlowPath = m_graph.SingleFlowPathBranches;

            foreach (SeriesCompensator seriesCompensator in m_childrenSeriesCompensators)
            {
                if (singleFlowPath.Contains(seriesCompensator as SeriesBranchBase))
                {
                    int seriesCompensatorIndex = m_childrenSeriesCompensators.IndexOf(seriesCompensator);

                    if ((impedanceStateIndex & Convert.ToInt16(Math.Pow(2, seriesCompensatorIndex))) > 0)
                    {
                        seriesCompensator.Status = SeriesCompensatorStatus.Energized;
                    }
                    else
                    {
                        seriesCompensator.Status = SeriesCompensatorStatus.Bypassed;
                    }
                }
                else
                {
                    seriesCompensator.Status = SeriesCompensatorStatus.Bypassed;
                }
            }
        }

        private void ComputeRealTimePositiveSequenceImpedanceWithBothVoltagesAndBothCurrentFlows()
        {
            m_previousRealTimeCalculatedImpedance = m_realTimeCalculatedImpedance.Copy();

            if (m_parentDivision.ParentCompany.ParentModel.PhaseConfiguration == PhaseSelection.ThreePhase)
            {
                m_fromNode.Voltage.ComputeSequenceComponents();
                m_toNode.Voltage.ComputeSequenceComponents();
                m_fromSubstationCurrent.ComputeSequenceComponents();
                m_toSubstationCurrent.ComputeSequenceComponents();
            }

            PhasorMeasurement Vs = m_fromNode.Voltage.PositiveSequence.Measurement;
            PhasorMeasurement Vr = m_toNode.Voltage.PositiveSequence.Measurement;
            PhasorMeasurement Is = m_fromSubstationCurrent.PositiveSequence.Measurement;
            PhasorMeasurement Ir = m_toSubstationCurrent.PositiveSequence.Measurement;

            // Series Impedance
            double resistance = ((Vs.PerUnitComplexPhasor * Vs.PerUnitComplexPhasor - Vr.PerUnitComplexPhasor * Vr.PerUnitComplexPhasor) / (Vs.PerUnitComplexPhasor * Is.PerUnitComplexPhasor - Vr.PerUnitComplexPhasor * Ir.PerUnitComplexPhasor)).Real;
            double reactance = ((Vs.PerUnitComplexPhasor * Vs.PerUnitComplexPhasor - Vr.PerUnitComplexPhasor * Vr.PerUnitComplexPhasor) / (Vs.PerUnitComplexPhasor * Is.PerUnitComplexPhasor - Vr.PerUnitComplexPhasor * Ir.PerUnitComplexPhasor)).Imaginary;

            // Shunt Susceptance
            double conductance = 2 * ((Ir.PerUnitComplexPhasor + Is.PerUnitComplexPhasor) / (Vr.PerUnitComplexPhasor + Vs.PerUnitComplexPhasor)).Real;
            double susceptance = 2 * ((Ir.PerUnitComplexPhasor + Is.PerUnitComplexPhasor) / (Vr.PerUnitComplexPhasor + Vs.PerUnitComplexPhasor)).Imaginary;

            // Only set the values for positive sequence equivalence
            Impedance impedance = new Impedance()
            {
                R1 = resistance,
                R3 = resistance,
                R6 = resistance,
                X1 = reactance,
                X3 = reactance,
                X6 = reactance,
                G1 = conductance,
                G3 = conductance,
                G6 = conductance,
                B1 = susceptance,
                B3 = susceptance,
                B6 = susceptance
            };

            m_realTimeCalculatedImpedance = impedance;
        }

        private void ComputeRealTimePositiveSequenceImpedanceWithBothVoltagesAndFromSideCurrentFlow()
        {
            m_previousRealTimeCalculatedImpedance = m_realTimeCalculatedImpedance.Copy();

            PhasorMeasurement Vs = m_fromNode.Voltage.PositiveSequence.Measurement;
            PhasorMeasurement Vr = m_toNode.Voltage.PositiveSequence.Measurement;
            PhasorMeasurement Is = m_fromSubstationCurrent.PositiveSequence.Measurement;

            // Series Impedance
            double resistance = ((Vs.PerUnitComplexPhasor - Vr.PerUnitComplexPhasor) / (Is.PerUnitComplexPhasor)).Real;
            double reactance = ((Vs.PerUnitComplexPhasor - Vr.PerUnitComplexPhasor) / (Is.PerUnitComplexPhasor)).Imaginary;

            // Only set the values for positive sequence equivalence
            Impedance impedance = new Impedance()
            {
                R1 = resistance,
                R3 = resistance,
                R6 = resistance,
                X1 = reactance,
                X3 = reactance,
                X6 = reactance
            };

            m_realTimeCalculatedImpedance = impedance;
        }

        private void ComputeRealTimePositiveSequenceImpedanceWithBothVoltagesAndToSideCurrentFlow()
        {
            m_previousRealTimeCalculatedImpedance = m_realTimeCalculatedImpedance.Copy();

            PhasorMeasurement Vs = m_fromNode.Voltage.PositiveSequence.Measurement;
            PhasorMeasurement Vr = m_toNode.Voltage.PositiveSequence.Measurement;
            PhasorMeasurement Ir = m_toSubstationCurrent.PositiveSequence.Measurement;

            // Series Impedance
            double resistance = ((Vr.PerUnitComplexPhasor - Vs.PerUnitComplexPhasor) / (Ir.PerUnitComplexPhasor)).Real;
            double reactance = ((Vr.PerUnitComplexPhasor - Vs.PerUnitComplexPhasor) / (Ir.PerUnitComplexPhasor)).Imaginary;

            // Only set the values for positive sequence equivalence
            Impedance impedance = new Impedance()
            {
                R1 = resistance,
                R3 = resistance,
                R6 = resistance,
                X1 = reactance,
                X3 = reactance,
                X6 = reactance
            };

            m_realTimeCalculatedImpedance = impedance;
        }

        #endregion
    }
}
