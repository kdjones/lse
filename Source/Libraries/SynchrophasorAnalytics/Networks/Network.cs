//******************************************************************************************************
//  Network.cs
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
//  05/16/2014 - Kevin D. Jones
//       Added System.Numerics for complex numbers
//  05/17/2014 - Kevin D. Jones
//       Added Math.NET for complex matrices
//
//******************************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.IO;
using System.Xml.Serialization;
using MathNet.Numerics;

using MathNet.Numerics.LinearAlgebra.Complex;
using SynchrophasorAnalytics.Graphs;
using SynchrophasorAnalytics.Modeling;
using SynchrophasorAnalytics.Measurements;
using SynchrophasorAnalytics.Matrices;
using SynchrophasorAnalytics.Calibration;
using SynchrophasorAnalytics.Psse;
using SynchrophasorAnalytics.Hdb;

namespace SynchrophasorAnalytics.Networks
{
    /// <summary>
    /// The class representation of an electric power system network with all of the methods needed to perform linear state estimation and other analytical functions.
    /// </summary>
    [Serializable()]
    public class Network
    {
        #region [ Private Members ]

        private SystemMatrix m_systemMatrix;
        private NetworkModel m_networkModel;
        private PerformanceMetrics m_performanceMetrics;
        private bool m_hasChangedSincePreviousFrame;
        private bool[] m_pastDiscreteVoltagePhasorState;
        private bool[] m_pastDiscreteCurrentPhasorState;
        private bool[] m_pastDiscreteShuntCurrentPhasorState;
        private int[] m_pastBreakerStatusState;
        private double[] m_pastStatusWordState;

        #endregion

        #region [ Properties ]

        [XmlElement("PerformanceMetrics")]
        public PerformanceMetrics PerformanceMetrics
        {
            get
            {
                if (m_performanceMetrics == null)
                {
                    m_performanceMetrics = new PerformanceMetrics();
                }
                return m_performanceMetrics;
            }
            set
            {
                m_performanceMetrics = value;
            }
        }

        /// <summary>
        /// Determines whether to treat the network as a <see cref="PhaseSelection.PositiveSequence"/> approximation or as a full <see cref="PhaseSelection.ThreePhase"/> representation.
        /// </summary>
        [XmlIgnore()]
        public PhaseSelection PhaseConfiguration
        {
            get 
            {
                return m_networkModel.PhaseConfiguration; 
            }
            set 
            {
                m_networkModel.PhaseConfiguration = value; 
            }
        }

        /// <summary>
        /// The physical representation of the network.
        /// </summary>
        [XmlElement("Model")]
        public NetworkModel Model
        {
            get 
            { 
                return m_networkModel; 
            }
            set 
            { 
                m_networkModel = value;
                m_networkModel.ParentNetwork = this; 
            }
        }

        /// <summary>
        /// The present state matrix for the system.
        /// </summary>
        [XmlIgnore()]
        public SystemMatrix Matrix
        {
            get
            {
                return m_systemMatrix;
            }
        }

        /// <summary>
        /// A boolean flag which indicates whether the discrete representations of the measurements have changed since the previous execution
        /// </summary>
        [XmlIgnore()]
        public bool HasChangedSincePreviousFrame
        {
            get
            {
                return m_hasChangedSincePreviousFrame;
            }
        }
        
        /// <summary>
        /// The <see cref="LinearStateEstimator.Modeling.ObservedBus"/> which contains the assumed perfect voltage measurement to do 24 hour CT and PT calibration. Possibility of decpreciation.
        /// </summary>
        [XmlIgnore()]
        public ObservedBus PerfectPtBusForPrimaryPhasorCalibration
        {
            get
            {
                foreach (ObservedBus observedBus in m_networkModel.ObservedBusses)
                {
                    foreach (Node node in observedBus.Nodes)
                    {
                        if (node.Voltage.PhaseA.Measurement.InstrumentTransformerCalibrationSetting == CalibrationSetting.Perfect &&
                            node.Voltage.PhaseB.Measurement.InstrumentTransformerCalibrationSetting == CalibrationSetting.Perfect &&
                            node.Voltage.PhaseC.Measurement.InstrumentTransformerCalibrationSetting == CalibrationSetting.Perfect)
                        {
                            return observedBus;
                        }
                    }
                }
                return null;
            }
        }

        #region [ State Vector ]

        /// <summary>
        /// A <see cref="DenseMatrix"/> of the state vector of the <see cref="LinearStateEstimator.Networks.Network"/> in line-to-neutral volts.
        /// </summary>
        [XmlIgnore()]
        public DenseMatrix StateVector
        {
            get
            {
                bool usePerUnit = false;
                if (m_networkModel.PhaseConfiguration == PhaseSelection.PositiveSequence)
                {
                    return GetPositiveSequenceStateVectorFromModel(usePerUnit);
                }
                else if (m_networkModel.PhaseConfiguration == PhaseSelection.ThreePhase)
                {
                    return GetThreePhaseStateVectorFromModel(usePerUnit);
                }
                else
                {
                    return null;
                }
            }
            set
            {
                bool isPerUnit = false;
                if (m_networkModel.PhaseConfiguration == PhaseSelection.PositiveSequence)
                {
                    SendPositiveSequenceStateVectorToModel(value, isPerUnit);
                }
                else if (m_networkModel.PhaseConfiguration == PhaseSelection.ThreePhase)
                {
                    SendThreePhaseStateVectorToModel(value, isPerUnit);
                }
            }
        }

        /// <summary>
        /// A <see cref="DenseMatrix"/> of the state vector of the <see cref="LinearStateEstimator.Networks.Network"/> in per unit volts.
        /// </summary>
        [XmlIgnore()]
        public DenseMatrix PerUnitStateVector
        {
            get
            {
                bool usePerUnit = true;
                if (m_networkModel.PhaseConfiguration == PhaseSelection.PositiveSequence)
                {
                    return GetPositiveSequenceStateVectorFromModel(usePerUnit);
                }
                else if (m_networkModel.PhaseConfiguration == PhaseSelection.ThreePhase)
                {
                    return GetThreePhaseStateVectorFromModel(usePerUnit);
                }
                else
                {
                    return null;
                }
            }
            set
            {
                bool isPerUnit = true;
                if (m_networkModel.PhaseConfiguration == PhaseSelection.PositiveSequence)
                {
                    SendPositiveSequenceStateVectorToModel(value, isPerUnit);
                }
                else if (m_networkModel.PhaseConfiguration == PhaseSelection.ThreePhase)
                {
                    SendThreePhaseStateVectorToModel(value, isPerUnit);
                }
            }
        }

        /// <summary>
        /// A state vector of the <see cref="LinearStateEstimator.Networks.Network"/> in line-to-neutral voltage magnitude.
        /// </summary>
        [XmlIgnore()]
        public double[] StateVectorMagnitude
        {
            get
            {
                DenseMatrix stateVector = StateVector;
                double[] stateVectorMagnitude = new double[stateVector.RowCount];
                for (int i = 0; i < stateVector.RowCount; i++)
                {
                    stateVectorMagnitude[i] = Math.Sqrt(stateVector[i, 0].Real * stateVector[i, 0].Real + stateVector[i, 0].Imaginary * stateVector[i, 0].Imaginary);
                }
                return stateVectorMagnitude;
            }
        }

        /// <summary>
        /// A state vector of the <see cref="LinearStateEstimator.Networks.Network"/> in per unit voltage magnitude.
        /// </summary>
        [XmlIgnore()]
        public double[] PerUnitStateVectorMagnitude
        {
            get
            {
                DenseMatrix perUnitStateVector = PerUnitStateVector;
                double[] stateVectorMagnitude = new double[perUnitStateVector.RowCount];
                for (int i = 0; i < perUnitStateVector.RowCount; i++)
                {
                    stateVectorMagnitude[i] = Math.Sqrt(perUnitStateVector[i, 0].Real * perUnitStateVector[i, 0].Real + perUnitStateVector[i, 0].Imaginary * perUnitStateVector[i, 0].Imaginary);
                }
                return stateVectorMagnitude;
            }
        }

        /// <summary>
        /// A state vector of the <see cref="LinearStateEstimator.Networks.Network"/> voltage angles in degrees.
        /// </summary>
        [XmlIgnore()]
        public double[] StateVectorAngleInDegrees
        {
            get
            {
                DenseMatrix stateVector = StateVector;
                double[] stateVectorAngleInDegrees = new double[stateVector.RowCount];
                for (int i = 0; i < stateVector.RowCount; i++)
                {
                    PhasorBase phasorPlaceHolder = new PhasorBase();
                    phasorPlaceHolder.ComplexPhasor = stateVector[i, 0];
                    stateVectorAngleInDegrees[i] = phasorPlaceHolder.AngleInDegrees;
                }
                return stateVectorAngleInDegrees;
            }
        }

        /// <summary>
        /// A state vector of the <see cref="LinearStateEstimator.Networks.Network"/> voltage angles in radians.
        /// </summary>
        [XmlIgnore()]
        public double[] StateVectorAngleInRadians
        {
            get
            {
                DenseMatrix stateVector = StateVector;
                double[] stateVectorAngleInRadians = new double[stateVector.RowCount];
                List<PhasorBase> phasorContainer = new List<PhasorBase>();
                for (int i = 0; i < stateVector.RowCount; i++)
                {
                    PhasorBase phasorBase = new PhasorBase();
                    phasorBase.ComplexPhasor = stateVector[i, 0];
                    stateVectorAngleInRadians[i] = phasorBase.AngleInRadians;
                }
                return stateVectorAngleInRadians;
            }
        }

        #endregion

        #region [ Measurement Vector ]

        /// <summary>
        /// A <see cref="DenseMatrix"/> of the measurement vector of the <see cref="LinearStateEstimator.Networks.Network"/> in nominal values.
        /// </summary>
        [XmlIgnore()]
        public DenseMatrix MeasurementVector
        {
            get
            {
                bool usePerUnit = false;
                if (m_networkModel.PhaseConfiguration == PhaseSelection.PositiveSequence)
                {
                    return GetPositiveSequenceMeasurementVectorFromModel(usePerUnit);
                }
                else if (m_networkModel.PhaseConfiguration == PhaseSelection.ThreePhase)
                {
                    return GetThreePhaseMeasurementVectorFromModel(usePerUnit);
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// A <see cref="DenseMatrix"/> of the measurement vector of the <see cref="LinearStateEstimator.Networks.Network"/> in per unit values.
        /// </summary>
        [XmlIgnore()]
        public DenseMatrix PerUnitMeasurementVector
        {
            get
            {
                bool usePerUnit = true;
                if (m_networkModel.PhaseConfiguration == PhaseSelection.PositiveSequence)
                {
                    return GetPositiveSequenceMeasurementVectorFromModel(usePerUnit);
                }
                else if (m_networkModel.PhaseConfiguration == PhaseSelection.ThreePhase)
                {
                    return GetThreePhaseMeasurementVectorFromModel(usePerUnit);
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// A measuremrent vector of the <see cref="LinearStateEstimator.Networks.Network"/> in phasor magnitude values.
        /// </summary>
        [XmlIgnore()]
        public double[] MeasurementVectorMagnitude
        {
            get
            {
                DenseMatrix measurementVector = MeasurementVector;
                double[] measurementVectorMagnitude = new double[measurementVector.RowCount];
                for (int i = 0; i < measurementVector.RowCount; i++)
                {
                    measurementVectorMagnitude[i] = Math.Sqrt(measurementVector[i, 0].Real * measurementVector[i, 0].Real + measurementVector[i, 0].Imaginary * measurementVector[i, 0].Imaginary);
                }
                return measurementVectorMagnitude;
            }
        }

        /// <summary>
        /// A measuremrent vector of the <see cref="LinearStateEstimator.Networks.Network"/> in per unit values.
        /// </summary>
        [XmlIgnore()]
        public double[] PerUnitMeasurementVectorMagnitude
        {
            get
            {
                DenseMatrix perUnitMeasurementVector = PerUnitMeasurementVector;
                double[] measurementVectorMagnitude = new double[perUnitMeasurementVector.RowCount];
                for (int i = 0; i < perUnitMeasurementVector.RowCount; i++)
                {
                    measurementVectorMagnitude[i] = Math.Sqrt(perUnitMeasurementVector[i, 0].Real * perUnitMeasurementVector[i, 0].Real + perUnitMeasurementVector[i, 0].Imaginary * perUnitMeasurementVector[i, 0].Imaginary);
                }
                return measurementVectorMagnitude;
            }
        }

        /// <summary>
        /// A measuremrent vector of the <see cref="LinearStateEstimator.Networks.Network"/> angles in degrees.
        /// </summary>
        [XmlIgnore()]
        public double[] MeasurementVectorAngleInDegrees
        {
            get
            {
                DenseMatrix measurementVector = MeasurementVector;
                double[] measurementVectorAngleInDegrees = new double[measurementVector.RowCount];
                for (int i = 0; i < measurementVector.RowCount; i++)
                {
                    PhasorBase phasorPlaceHolder = new PhasorBase();
                    phasorPlaceHolder.ComplexPhasor = measurementVector[i, 0];
                    measurementVectorAngleInDegrees[i] = phasorPlaceHolder.AngleInDegrees;
                }
                return measurementVectorAngleInDegrees;
            }
        }

        /// <summary>
        /// A measuremrent vector of the <see cref="LinearStateEstimator.Networks.Network"/> angles in radians.
        /// </summary>
        [XmlIgnore()]
        public double[] MeasurementVectorAngleInRadians
        {
            get
            {
                DenseMatrix measurementVector = MeasurementVector;
                double[] measurementVectorAngleInRadians = new double[measurementVector.RowCount];
                for (int i = 0; i < measurementVector.RowCount; i++)
                {
                    PhasorBase phasorPlaceHolder = new PhasorBase();
                    phasorPlaceHolder.ComplexPhasor = measurementVector[i, 0];
                    measurementVectorAngleInRadians[i] = phasorPlaceHolder.AngleInRadians;
                }
                return measurementVectorAngleInRadians;
            }
        }

        /// <summary>
        /// A <see cref="DenseMatrix"/> of the current measurement vector of the <see cref="LinearStateEstimator.Networks.Network"/> in per unit values used for primary phasor calibration. Possibility of depreciation.
        /// </summary>
        [XmlIgnore()]
        public DenseMatrix CurrentMeasurementVectorForPrimaryPhasorCalibration
        {
            get
            {
                List<CurrentFlowPhasorGroup> currentPhasorsForPrimaryPhasorCalibration = new List<CurrentFlowPhasorGroup>();

                foreach (CurrentFlowPhasorGroup currentPhasorGroup in m_networkModel.ActiveCurrentFlows)
                {
                    if (currentPhasorGroup.PhaseA.Measurement.InstrumentTransformerCalibrationSetting == CalibrationSetting.Active ||
                        currentPhasorGroup.PhaseB.Measurement.InstrumentTransformerCalibrationSetting == CalibrationSetting.Active ||
                        currentPhasorGroup.PhaseC.Measurement.InstrumentTransformerCalibrationSetting == CalibrationSetting.Active)
                    {
                        currentPhasorsForPrimaryPhasorCalibration.Add(currentPhasorGroup);
                    }
                }

                DenseMatrix currentMeasurementVectorForPrimaryPhasorCalibration = DenseMatrix.OfArray(new Complex[3 * currentPhasorsForPrimaryPhasorCalibration.Count(), 1]);

                for (int i = 0; i < currentMeasurementVectorForPrimaryPhasorCalibration.RowCount / 3; i++)
                {
                    currentMeasurementVectorForPrimaryPhasorCalibration[3 * i, 0] = currentPhasorsForPrimaryPhasorCalibration[i].PhaseA.Measurement.PerUnitComplexPhasor;
                    currentMeasurementVectorForPrimaryPhasorCalibration[3 * i + 1, 0] = currentPhasorsForPrimaryPhasorCalibration[i].PhaseB.Measurement.PerUnitComplexPhasor;
                    currentMeasurementVectorForPrimaryPhasorCalibration[3 * i + 2, 0] = currentPhasorsForPrimaryPhasorCalibration[i].PhaseC.Measurement.PerUnitComplexPhasor;
                }

                return currentMeasurementVectorForPrimaryPhasorCalibration;
            }
        }

        /// <summary>
        /// A <see cref="DenseMatrix"/> of the voltage measurement vector of the <see cref="LinearStateEstimator.Networks.Network"/> in per unit values used for primary phasor calibration. Possibility of depreciation.
        /// </summary>
        [XmlIgnore()]
        public DenseMatrix VoltageMeasurementVectorForPrimaryPhasorCalibration
        {
            get
            {
                List<VoltagePhasorGroup> voltagePhasorForPrimaryPhasorCalibration = new List<VoltagePhasorGroup>();

                foreach (ObservedBus observedBus in m_networkModel.ObservedBusses)
                {
                    foreach (Node node in observedBus.Nodes)
                    {
                        if ((node.Voltage.PhaseA.Measurement.InstrumentTransformerCalibrationSetting == CalibrationSetting.Active ||
                             node.Voltage.PhaseA.Measurement.InstrumentTransformerCalibrationSetting == CalibrationSetting.Perfect) &&
                            (node.Voltage.PhaseB.Measurement.InstrumentTransformerCalibrationSetting == CalibrationSetting.Active ||
                             node.Voltage.PhaseB.Measurement.InstrumentTransformerCalibrationSetting == CalibrationSetting.Perfect) &&
                            (node.Voltage.PhaseC.Measurement.InstrumentTransformerCalibrationSetting == CalibrationSetting.Active ||
                             node.Voltage.PhaseC.Measurement.InstrumentTransformerCalibrationSetting == CalibrationSetting.Perfect))
                        {
                            voltagePhasorForPrimaryPhasorCalibration.Add(node.Voltage);
                        }
                    }
                }

                DenseMatrix voltageMeasurementVectorForPrimaryPhasorCalibration = DenseMatrix.OfArray(new Complex[3 * voltagePhasorForPrimaryPhasorCalibration.Count(), 1]);

                for (int i = 0; i < voltageMeasurementVectorForPrimaryPhasorCalibration.RowCount / 3; i++)
                {
                    voltageMeasurementVectorForPrimaryPhasorCalibration[3 * i, 0] = voltagePhasorForPrimaryPhasorCalibration[i].PhaseA.Measurement.PerUnitComplexPhasor;
                    voltageMeasurementVectorForPrimaryPhasorCalibration[3 * i + 1, 0] = voltagePhasorForPrimaryPhasorCalibration[i].PhaseB.Measurement.PerUnitComplexPhasor;
                    voltageMeasurementVectorForPrimaryPhasorCalibration[3 * i + 2, 0] = voltagePhasorForPrimaryPhasorCalibration[i].PhaseC.Measurement.PerUnitComplexPhasor;
                }

                return voltageMeasurementVectorForPrimaryPhasorCalibration;
            }
        }

        #endregion

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// A blank constructor with default values.
        /// </summary>
        public Network()
            :this(new NetworkModel())
        {
        }

        /// <summary>
        /// The designated constructor for the <see cref="Network"/> class.
        /// </summary>
        /// <param name="networkModel">The virtualized <see cref="NetworkModel"/>.</param>
        public Network(NetworkModel networkModel)
        {
            m_networkModel = networkModel;
            m_performanceMetrics = new PerformanceMetrics(this);
            m_networkModel.ParentNetwork = this;
            m_hasChangedSincePreviousFrame = true;
        }

        #endregion

        #region [ Private Methods ]

        private DenseMatrix GetPositiveSequenceStateVectorFromModel(bool usePerUnit)
        {
            List<VoltagePhasorGroup> nodeVoltages = m_networkModel.Voltages;

            DenseMatrix stateVector = DenseMatrix.OfArray(new Complex[nodeVoltages.Count, 1]);

            if (usePerUnit)
            {
                for (int i = 0; i < nodeVoltages.Count; i++)
                {
                    stateVector[i, 0] = nodeVoltages[i].PositiveSequence.Estimate.PerUnitComplexPhasor;
                }
            }
            else
            {
                for (int i = 0; i < nodeVoltages.Count; i++)
                {
                    stateVector[i, 0] = nodeVoltages[i].PositiveSequence.Estimate.ComplexPhasor;
                }
            }

            return stateVector;
        }

        private DenseMatrix GetThreePhaseStateVectorFromModel(bool usePerUnit)
        {
            List<VoltagePhasorGroup> nodeVoltages = m_networkModel.Voltages;

            DenseMatrix stateVector = DenseMatrix.OfArray(new Complex[3 * nodeVoltages.Count, 1]);

            if (usePerUnit)
            {
                for (int i = 0; i < nodeVoltages.Count; i++)
                {
                    stateVector[3 * i, 0] = nodeVoltages[i].PhaseA.Estimate.PerUnitComplexPhasor;
                    stateVector[3 * i + 1, 0] = nodeVoltages[i].PhaseB.Estimate.PerUnitComplexPhasor;
                    stateVector[3 * i + 2, 0] = nodeVoltages[i].PhaseC.Estimate.PerUnitComplexPhasor;
                }
            }
            else
            {
                for (int i = 0; i < nodeVoltages.Count; i++)
                {
                    stateVector[3 * i, 0] = nodeVoltages[i].PhaseA.Estimate.ComplexPhasor;
                    stateVector[3 * i + 1, 0] = nodeVoltages[i].PhaseB.Estimate.ComplexPhasor;
                    stateVector[3 * i + 2, 0] = nodeVoltages[i].PhaseC.Estimate.ComplexPhasor;
                }
            }

            return stateVector;
        }

        #region [ Get Positive Sequence Measurement Vector Methods ]

        private DenseMatrix GetPositiveSequenceMeasurementVectorFromModel(bool usePerUnit)
        {
            DenseMatrix voltageMeasurementVector = GetPositiveSequenceVoltageMeasurementVectorFromModel(usePerUnit);
            DenseMatrix currentFlowMeasurementVector = GetPositiveSequenceCurrentFlowMeasurementVectorFromModel(usePerUnit);
            DenseMatrix currentInjectionMeasurementVector = GetPositiveSequenceCurrentInjectionMeasurementVectorFromModel(usePerUnit);

            DenseMatrix measurementVector = voltageMeasurementVector;

            if (currentFlowMeasurementVector != null)
            {
                measurementVector = MatrixCalculationExtensions.VerticallyConcatenate(measurementVector, currentFlowMeasurementVector);
            }

            if (currentInjectionMeasurementVector != null)
            {
                measurementVector = MatrixCalculationExtensions.VerticallyConcatenate(measurementVector, currentInjectionMeasurementVector);
            }

            return measurementVector;

        }

        private DenseMatrix GetPositiveSequenceVoltageMeasurementVectorFromModel(bool usePerUnit)
        {
            // Resolve the Network into a list of ObservedBusses
            List<ObservedBus> observedBusses = m_networkModel.ObservedBusses;

            // Create a list of directly measured nodes from the set of ObservedBusses
            List<Node> measuredNodes = new List<Node>();

            foreach (ObservedBus observedBus in observedBusses)
            {
                foreach (Node node in observedBus.Nodes)
                {
                    if (node.Observability == ObservationState.DirectlyObserved)
                    {
                        measuredNodes.Add(node);
                    }
                }
            }

            if (measuredNodes.Count > 0)
            {
                DenseMatrix voltageMeasurementVector = DenseMatrix.OfArray(new Complex[measuredNodes.Count, 1]);

                if (usePerUnit)
                {
                    for (int i = 0; i < measuredNodes.Count; i++)
                    {
                        voltageMeasurementVector[i, 0] = measuredNodes[i].Voltage.PositiveSequence.Measurement.PerUnitComplexPhasor;
                    }

                }
                else
                {
                    for (int i = 0; i < measuredNodes.Count; i++)
                    {
                        voltageMeasurementVector[i, 0] = measuredNodes[i].Voltage.PositiveSequence.Measurement.ComplexPhasor;
                    }

                }

                return voltageMeasurementVector;
            }
            else
            {
                throw new Exception("No voltages to include in the measurement vector.");
            }
        }

        private DenseMatrix GetPositiveSequenceCurrentFlowMeasurementVectorFromModel(bool usePerUnit)
        {
            List<CurrentFlowPhasorGroup> measuredCurrentFlows = m_networkModel.IncludedCurrentFlows;

            if (measuredCurrentFlows.Count > 0)
            {
                DenseMatrix currentFlowMeasurementVector = DenseMatrix.OfArray(new Complex[measuredCurrentFlows.Count, 1]);

                if (usePerUnit)
                {
                    for (int i = 0; i < measuredCurrentFlows.Count; i++)
                    {
                        currentFlowMeasurementVector[i, 0] = measuredCurrentFlows[i].PositiveSequence.Measurement.PerUnitComplexPhasor;
                    }
                }
                else
                {
                    for (int i = 0; i < measuredCurrentFlows.Count; i++)
                    {
                        currentFlowMeasurementVector[i, 0] = measuredCurrentFlows[i].PositiveSequence.Measurement.ComplexPhasor;
                    }
                }

                return currentFlowMeasurementVector;
            }
            else
            {
                return null;
            }
        }

        private DenseMatrix GetPositiveSequenceCurrentInjectionMeasurementVectorFromModel(bool usePerUnit)
        {
            // Get the currrent measurements from shunt injections
            List<CurrentInjectionPhasorGroup> measuredCurrentInjections = m_networkModel.ActiveCurrentInjections;

            if (measuredCurrentInjections.Count > 0)
            {
                DenseMatrix currentInjectionMeasurementVector = DenseMatrix.OfArray(new Complex[measuredCurrentInjections.Count, 1]);

                if (usePerUnit)
                {
                    for (int i = 0; i < measuredCurrentInjections.Count; i++)
                    {
                        currentInjectionMeasurementVector[i, 0] = measuredCurrentInjections[i].PositiveSequence.Measurement.PerUnitComplexPhasor;
                    }
                }
                else
                {
                    for (int i = 0; i < measuredCurrentInjections.Count; i++)
                    {
                        currentInjectionMeasurementVector[i, 0] = measuredCurrentInjections[i].PositiveSequence.Measurement.ComplexPhasor;
                    }
                }

                return currentInjectionMeasurementVector;
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region [ Get Three Phase Measurement Vector Methods ]

        private DenseMatrix GetThreePhaseMeasurementVectorFromModel(bool usePerUnit)
        {
            DenseMatrix voltageMeasurementVector = GetThreePhaseVoltageMeasurementVectorFromModel(usePerUnit);
            DenseMatrix currentFlowMeasurementVector = GetThreePhaseCurrentFlowMeasurementVectorFromModel(usePerUnit);
            DenseMatrix currentInjectionMeasurementVector = GetThreePhaseCurrentInjectionMeasurementVectorFromModel(usePerUnit);

            DenseMatrix measurementVector = voltageMeasurementVector;

            if (currentFlowMeasurementVector != null)
            {
                measurementVector = MatrixCalculationExtensions.VerticallyConcatenate(measurementVector, currentFlowMeasurementVector);
            }

            if (currentInjectionMeasurementVector != null)
            {
                measurementVector = MatrixCalculationExtensions.VerticallyConcatenate(measurementVector, currentInjectionMeasurementVector);
            }

            return measurementVector;
        }

        private DenseMatrix GetThreePhaseVoltageMeasurementVectorFromModel(bool usePerUnit)
        {
            // Resolve the Network into a list of ObservedBusses
            List<ObservedBus> observedBusses = m_networkModel.ObservedBusses;

            // Create a list of directly measured nodes from the set of ObservedBusses
            List<Node> measuredNodes = new List<Node>();
            foreach (ObservedBus observedBus in observedBusses)
            {
                foreach (Node node in observedBus.Nodes)
                {
                    if (node.Observability == ObservationState.DirectlyObserved)
                    {
                        measuredNodes.Add(node);
                    }
                }
            }

            if (measuredNodes.Count > 0)
            {
                DenseMatrix voltageMeasurementVector = DenseMatrix.OfArray(new Complex[3 * measuredNodes.Count, 1]);

                if (usePerUnit)
                {
                    for (int i = 0; i < measuredNodes.Count; i++)
                    {
                        voltageMeasurementVector[3 * i, 0] = measuredNodes[i].Voltage.PhaseA.Measurement.PerUnitComplexPhasor;
                        voltageMeasurementVector[3 * i + 1, 0] = measuredNodes[i].Voltage.PhaseB.Measurement.PerUnitComplexPhasor;
                        voltageMeasurementVector[3 * i + 2, 0] = measuredNodes[i].Voltage.PhaseC.Measurement.PerUnitComplexPhasor;
                    }
                }
                else
                {
                    for (int i = 0; i < measuredNodes.Count; i++)
                    {
                        voltageMeasurementVector[3 * i, 0] = measuredNodes[i].Voltage.PhaseA.Measurement.ComplexPhasor;
                        voltageMeasurementVector[3 * i + 1, 0] = measuredNodes[i].Voltage.PhaseB.Measurement.ComplexPhasor;
                        voltageMeasurementVector[3 * i + 2, 0] = measuredNodes[i].Voltage.PhaseC.Measurement.ComplexPhasor;
                    }
                }

                return voltageMeasurementVector;
            }
            else
            {
                throw new Exception("No voltages to include in the measurement vector.");
            }
        }

        private DenseMatrix GetThreePhaseCurrentFlowMeasurementVectorFromModel(bool usePerUnit)
        {
            List<CurrentFlowPhasorGroup> measuredCurrentFlows = m_networkModel.IncludedCurrentFlows;

            if (measuredCurrentFlows.Count > 0)
            {
                DenseMatrix currentFlowMeasurementVector = DenseMatrix.OfArray(new Complex[3 * measuredCurrentFlows.Count, 1]);

                if (usePerUnit)
                {
                    for (int i = 0; i < measuredCurrentFlows.Count; i++)
                    {
                        currentFlowMeasurementVector[3 * i, 0] = measuredCurrentFlows[i].PhaseA.Measurement.PerUnitComplexPhasor;
                        currentFlowMeasurementVector[3 * i + 1, 0] = measuredCurrentFlows[i].PhaseB.Measurement.PerUnitComplexPhasor;
                        currentFlowMeasurementVector[3 * i + 2, 0] = measuredCurrentFlows[i].PhaseC.Measurement.PerUnitComplexPhasor;
                    }
                }
                else
                {
                    for (int i = 0; i < measuredCurrentFlows.Count; i++)
                    {
                        currentFlowMeasurementVector[3 * i, 0] = measuredCurrentFlows[i].PhaseA.Measurement.ComplexPhasor;
                        currentFlowMeasurementVector[3 * i + 1, 0] = measuredCurrentFlows[i].PhaseB.Measurement.ComplexPhasor;
                        currentFlowMeasurementVector[3 * i + 2, 0] = measuredCurrentFlows[i].PhaseC.Measurement.ComplexPhasor;
                    }
                }

                return currentFlowMeasurementVector;
            }
            else
            {
                return null;
            }
        }

        private DenseMatrix GetThreePhaseCurrentInjectionMeasurementVectorFromModel(bool usePerUnit)
        {
            // Get the currrent measurements from shunt injections
            List<CurrentInjectionPhasorGroup> measuredCurrentInjections = m_networkModel.ActiveCurrentInjections;

            if (measuredCurrentInjections.Count > 0)
            {
                DenseMatrix currentInjectionMeasurementVector = DenseMatrix.OfArray(new Complex[3 * measuredCurrentInjections.Count, 1]);

                if (usePerUnit)
                {
                    for (int i = 0; i < measuredCurrentInjections.Count; i++)
                    {
                        currentInjectionMeasurementVector[3 * i, 0] = measuredCurrentInjections[i].PhaseA.Measurement.PerUnitComplexPhasor;
                        currentInjectionMeasurementVector[3 * i + 1, 0] = measuredCurrentInjections[i].PhaseB.Measurement.PerUnitComplexPhasor;
                        currentInjectionMeasurementVector[3 * i + 2, 0] = measuredCurrentInjections[i].PhaseC.Measurement.PerUnitComplexPhasor;
                    }
                }
                else
                {
                    for (int i = 0; i < measuredCurrentInjections.Count; i++)
                    {
                        currentInjectionMeasurementVector[3 * i, 0] = measuredCurrentInjections[i].PhaseA.Measurement.ComplexPhasor;
                        currentInjectionMeasurementVector[3 * i + 1, 0] = measuredCurrentInjections[i].PhaseB.Measurement.ComplexPhasor;
                        currentInjectionMeasurementVector[3 * i + 2, 0] = measuredCurrentInjections[i].PhaseC.Measurement.ComplexPhasor;
                    }
                }

                return currentInjectionMeasurementVector;
            }
            else
            {
                return null;
            }
        }

        #endregion

        private void SendPositiveSequenceStateVectorToModel(DenseMatrix stateVector, bool isPerUnit)
        {
            List<ObservedBus> observedBusses = m_networkModel.ObservedBusses;

            for (int i = 0; i < observedBusses.Count; i++)
            {
                VoltagePhasorGroup voltagePhasorGroup = new VoltagePhasorGroup();
                voltagePhasorGroup.PositiveSequence.Estimate.BaseKV = observedBusses[i].BaseKV;
                voltagePhasorGroup.PositiveSequence.Estimate.PerUnitComplexPhasor = stateVector[i, 0];
                observedBusses[i].Value = voltagePhasorGroup;
            }
        }

        private void SendThreePhaseStateVectorToModel(DenseMatrix stateVector, bool isPerUnit)
        {
            List<ObservedBus> observedBusses = m_networkModel.ObservedBusses;

            for (int i = 0; i < observedBusses.Count; i++)
            {
                VoltagePhasorGroup voltagePhasorGroup = new VoltagePhasorGroup();
                voltagePhasorGroup.PhaseA.Estimate.BaseKV = observedBusses[i].BaseKV;
                voltagePhasorGroup.PhaseB.Estimate.BaseKV = observedBusses[i].BaseKV;
                voltagePhasorGroup.PhaseC.Estimate.BaseKV = observedBusses[i].BaseKV;
                voltagePhasorGroup.PhaseA.Estimate.PerUnitComplexPhasor = stateVector[3 * i, 0];
                voltagePhasorGroup.PhaseB.Estimate.PerUnitComplexPhasor = stateVector[3 * i + 1, 0];
                voltagePhasorGroup.PhaseC.Estimate.PerUnitComplexPhasor = stateVector[3 * i + 2, 0];
                voltagePhasorGroup.ComputeSequenceComponents();
                observedBusses[i].Value = voltagePhasorGroup;
            }
        }

        #endregion

        #region [ Public Methods ]

        /// <summary>
        /// Initializes the <see cref="LinearStateEstimator.Networks.Network"/> by re-establishing references that are not preserved hierarchically in the configuration file.
        /// </summary>
        public void Initialize()
        {
            // Initialize children
            m_networkModel.Initialize();
            m_performanceMetrics.SetNetwork(this);
            InitializePastDiscreteStates();
        }

        /// <summary>
        /// Computes the current state of the network
        /// </summary>
        public void ComputeSystemState()
        {
            if (m_systemMatrix == null || m_hasChangedSincePreviousFrame)
            {
                m_systemMatrix = new SystemMatrix(this);
            }
            
            PerUnitStateVector = m_systemMatrix.PsuedoInverseOfMatrix * PerUnitMeasurementVector;
        }

        /// <summary>
        /// A method which checks to see if the matrix representation of the networks needs to be reperformed and sets the flag for <see cref="LinearStateEstimator.Networks.Network.HasChangedSincePreviousFrame"/>.
        /// </summary>
        public void RunNetworkReconstructionCheck()
        {
            if (ComparePresentAndPastDiscreteStates())
            {
                m_hasChangedSincePreviousFrame = true;
                UpdatePastDiscreteStates();
            }
            else
            {
                m_hasChangedSincePreviousFrame = false;
            }
        }

        #endregion

        #region [ Private Methods ]

        private void UpdatePastDiscreteStates()
        {

            for (int i = 0; i < m_networkModel.ExpectedVoltages.Count(); i++)
            {
                if (m_networkModel.PhaseConfiguration == PhaseSelection.ThreePhase)
                {
                    m_pastDiscreteVoltagePhasorState[i] = m_networkModel.ExpectedVoltages[i].IncludeInEstimator;
                }
                else if (m_networkModel.PhaseConfiguration == PhaseSelection.PositiveSequence)
                {
                    m_pastDiscreteVoltagePhasorState[i] = m_networkModel.ExpectedVoltages[i].IncludeInPositiveSequenceEstimator;
                }
            }

            for (int i = 0; i < m_networkModel.ExpectedCurrentFlows.Count(); i++)
            {
                if (m_networkModel.PhaseConfiguration == PhaseSelection.ThreePhase)
                {
                    m_pastDiscreteCurrentPhasorState[i] = m_networkModel.ExpectedCurrentFlows[i].IncludeInEstimator;
                }
                else if (m_networkModel.PhaseConfiguration == PhaseSelection.PositiveSequence)
                {
                    m_pastDiscreteCurrentPhasorState[i] = m_networkModel.ExpectedCurrentFlows[i].IncludeInPositiveSequenceEstimator;
                }
            }

            for (int i = 0; i < m_networkModel.ExpectedCurrentInjections.Count(); i++)
            {
                if (m_networkModel.PhaseConfiguration == PhaseSelection.ThreePhase)
                {
                    m_pastDiscreteShuntCurrentPhasorState[i] = m_networkModel.ExpectedCurrentInjections[i].IncludeInEstimator;
                }
                else if (m_networkModel.PhaseConfiguration == PhaseSelection.PositiveSequence)
                {
                    m_pastDiscreteShuntCurrentPhasorState[i] = m_networkModel.ExpectedCurrentInjections[i].IncludeInPositiveSequenceEstimator;
                }
            }

            for (int i = 0; i < m_networkModel.ExpectedBreakerStatuses.Count(); i++)
            {
                m_pastBreakerStatusState[i] = m_networkModel.ExpectedBreakerStatuses[i].BinaryValue;
            }

            for (int i = 0; i < m_networkModel.ExpectedStatusWords.Count(); i++)
            {
                m_pastStatusWordState[i] = m_networkModel.ExpectedStatusWords[i].BinaryValue;
            }

        }

        private void InitializePastDiscreteStates()
        {

            m_pastDiscreteVoltagePhasorState = new bool[m_networkModel.ExpectedVoltages.Count()];
            m_pastDiscreteCurrentPhasorState = new bool[m_networkModel.ExpectedCurrentFlows.Count()];
            m_pastDiscreteShuntCurrentPhasorState = new bool[m_networkModel.ExpectedCurrentInjections.Count()];
            m_pastBreakerStatusState = new int[m_networkModel.ExpectedBreakerStatuses.Count()];
            m_pastStatusWordState = new double[m_networkModel.ExpectedStatusWords.Count()];

            for (int i = 0; i < m_pastDiscreteVoltagePhasorState.Length; i++)
            {
                m_pastDiscreteVoltagePhasorState[i] = true;
            }

            if (m_pastDiscreteCurrentPhasorState.Length > 0)
            {
                for (int i = 0; i < m_pastDiscreteCurrentPhasorState.Length; i++)
                {
                    m_pastDiscreteCurrentPhasorState[i] = true;
                }
            }

            if (m_pastDiscreteShuntCurrentPhasorState.Length > 0)
            {
                for (int i = 0; i < m_pastDiscreteShuntCurrentPhasorState.Length; i++)
                {
                    m_pastDiscreteShuntCurrentPhasorState[i] = true;
                }
            }

            if (m_pastBreakerStatusState.Length > 0)
            {
                for (int i = 0; i < m_pastBreakerStatusState.Length; i++)
                {
                    m_pastBreakerStatusState[i] = 0;
                }
            }

            if (m_pastStatusWordState.Length > 0)
            {
                for (int i = 0; i < m_pastStatusWordState.Length; i++)
                {
                    m_pastStatusWordState[i] = 0;
                }
            }
        }

        private bool ComparePresentAndPastDiscreteStates()
        {
            for (int i = 0; i < m_networkModel.ExpectedVoltages.Count(); i++)
            {
                if (m_networkModel.PhaseConfiguration == PhaseSelection.ThreePhase)
                {
                    if (m_pastDiscreteVoltagePhasorState[i] != m_networkModel.ExpectedVoltages[i].IncludeInEstimator)
                    {
                        return true;
                    }
                }
                else if (m_networkModel.PhaseConfiguration == PhaseSelection.PositiveSequence)
                {
                    if (m_pastDiscreteVoltagePhasorState[i] != m_networkModel.ExpectedVoltages[i].IncludeInPositiveSequenceEstimator)
                    {
                        return true;
                    }
                }
            }

            for (int i = 0; i < m_networkModel.ExpectedCurrentFlows.Count(); i++)
            {
                if (m_networkModel.PhaseConfiguration == PhaseSelection.ThreePhase)
                {
                    if (m_pastDiscreteCurrentPhasorState[i] != m_networkModel.ExpectedCurrentFlows[i].IncludeInEstimator)
                    {
                        return true;
                    }
                }
                else if (m_networkModel.PhaseConfiguration == PhaseSelection.PositiveSequence)
                {
                    if (m_pastDiscreteCurrentPhasorState[i] != m_networkModel.ExpectedCurrentFlows[i].IncludeInPositiveSequenceEstimator)
                    {
                        return true;
                    }
                }
            }

            for (int i = 0; i < m_networkModel.ExpectedCurrentInjections.Count(); i++)
            {
                if (m_networkModel.PhaseConfiguration == PhaseSelection.ThreePhase)
                {
                    if (m_pastDiscreteCurrentPhasorState[i] != m_networkModel.ExpectedCurrentInjections[i].IncludeInEstimator)
                    {
                        return true;
                    }
                }
                else if (m_networkModel.PhaseConfiguration == PhaseSelection.PositiveSequence)
                {
                    if (m_pastDiscreteCurrentPhasorState[i] != m_networkModel.ExpectedCurrentInjections[i].IncludeInPositiveSequenceEstimator)
                    {
                        return true;
                    }
                }
            }

            for (int i = 0; i < m_networkModel.ExpectedBreakerStatuses.Count(); i++)
            {
                if (m_pastBreakerStatusState[i] != m_networkModel.ExpectedBreakerStatuses[i].BinaryValue)
                {
                    return true;
                }
            }

            for (int i = 0; i < m_networkModel.ExpectedStatusWords.Count(); i++)
            {
                if (m_pastStatusWordState[i] != m_networkModel.ExpectedStatusWords[i].BinaryValue)
                {
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region [ Xml Serialization/Deserialization methods ]

        /// <summary>
        /// Creates a new <see cref="LinearStateEstimator.Networks.Network"/> by deserializing the configuration file from the specified location.
        /// </summary>
        /// <param name="pathName">The location of the configuration file including the file name.</param>
        /// <returns>A new (but uninitialized) <see cref="LinearStateEstimator.Networks.Network"/> based on the configuration file. Must call <see cref="LinearStateEstimator.Modeling.NetworkModel.Initialize"/> method 
        /// to reconstitute parent, child, and sibling references in the <see cref="LinearStateEstimator.Networks.Network"/>.</returns>
        public static Network DeserializeFromXml(string pathName)
        {
            try
            {
                // Create an empy NetworkMeasurements object reference.
                Network network = null;

                // Create an XmlSerializer with the type of NetworkMeasurements.
                XmlSerializer deserializer = new XmlSerializer(typeof(Network));

                // Read the data in from the file.
                StreamReader reader = new StreamReader(pathName);

                // Cast the deserialized data as a NetworkMeasurements object.
                network = (Network)deserializer.Deserialize(reader);

                // Close the connection.
                reader.Close();
                
                return network;
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to Deserialize the Network from the Configuration File: " + exception.ToString());
            }
        }

        /// <summary>
        /// Serialized the <see cref="LinearStateEstimator.Networks.Network"/> to the specified file.
        /// </summary>
        /// <param name="pathName">The directory name included the file name of the desired location for Xml Serialization.</param>
        public void SerializeToXml(string pathName)
        {
            try
            {
                // Create an XmlSerializer with the type of Network
                XmlSerializer serializer = new XmlSerializer(typeof(Network));
                
                // Open a connection to the file and path.
                TextWriter writer = new StreamWriter(pathName);

                // Serialize this instance of NetworkMeasurements
                serializer.Serialize(writer, this);

                // Close the connection
                writer.Close();
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to Serialize the Network to the Configuration File: " + exception.ToString());
            }
        }

        /// <summary>
        /// Sends command based on boolen argument to either serialize the operating point with the model or not.
        /// </summary>
        /// <param name="serializationOption">A flag which represents whether or not to serialize the operating point with the model data.</param>
        public void SerializeData(bool serializationOption)
        {
            foreach (StatusWord statusWord in m_networkModel.StatusWords)
            {
                statusWord.ShouldSerializeData = serializationOption;
            }

            foreach (VoltagePhasorGroup voltagePhasorGroup in m_networkModel.Voltages)
            {
                voltagePhasorGroup.ShouldSerializeData = serializationOption;

                voltagePhasorGroup.PositiveSequence.Measurement.ShouldSerializeData = serializationOption;
                voltagePhasorGroup.PositiveSequence.Estimate.ShouldSerializeData = serializationOption;

                voltagePhasorGroup.PhaseA.Measurement.ShouldSerializeData = serializationOption;
                voltagePhasorGroup.PhaseA.Estimate.ShouldSerializeData = serializationOption;

                voltagePhasorGroup.PhaseB.Measurement.ShouldSerializeData = serializationOption;
                voltagePhasorGroup.PhaseB.Estimate.ShouldSerializeData = serializationOption;

                voltagePhasorGroup.PhaseC.Measurement.ShouldSerializeData = serializationOption;
                voltagePhasorGroup.PhaseC.Estimate.ShouldSerializeData = serializationOption;
            }

            foreach (CurrentFlowPhasorGroup currentPhasorGroup in m_networkModel.CurrentFlows)
            {
                currentPhasorGroup.ShouldSerializeData = serializationOption;

                currentPhasorGroup.PositiveSequence.Measurement.ShouldSerializeData = serializationOption;
                currentPhasorGroup.PositiveSequence.Estimate.ShouldSerializeData = serializationOption;

                currentPhasorGroup.PhaseA.Measurement.ShouldSerializeData = serializationOption;
                currentPhasorGroup.PhaseA.Estimate.ShouldSerializeData = serializationOption;

                currentPhasorGroup.PhaseB.Measurement.ShouldSerializeData = serializationOption;
                currentPhasorGroup.PhaseB.Estimate.ShouldSerializeData = serializationOption;

                currentPhasorGroup.PhaseC.Measurement.ShouldSerializeData = serializationOption;
                currentPhasorGroup.PhaseC.Estimate.ShouldSerializeData = serializationOption;
            }

            foreach (CurrentInjectionPhasorGroup shuntCurrentPhasorGroup in m_networkModel.CurrentInjections)
            {
                shuntCurrentPhasorGroup.ShouldSerializeData = serializationOption;

                shuntCurrentPhasorGroup.PositiveSequence.Measurement.ShouldSerializeData = serializationOption;
                shuntCurrentPhasorGroup.PositiveSequence.Estimate.ShouldSerializeData = serializationOption;

                shuntCurrentPhasorGroup.PhaseA.Measurement.ShouldSerializeData = serializationOption;
                shuntCurrentPhasorGroup.PhaseA.Estimate.ShouldSerializeData = serializationOption;

                shuntCurrentPhasorGroup.PhaseB.Measurement.ShouldSerializeData = serializationOption;
                shuntCurrentPhasorGroup.PhaseB.Estimate.ShouldSerializeData = serializationOption;

                shuntCurrentPhasorGroup.PhaseC.Measurement.ShouldSerializeData = serializationOption;
                shuntCurrentPhasorGroup.PhaseC.Estimate.ShouldSerializeData = serializationOption;
            }
        }

        /// <summary>
        /// Saves the current state of the <see cref="LinearStateEstimator.Networks.Network"/> object to a *.txt file to be used as a rudimentary momento design pattern. This is an alternative to Xml serialization. Honestly, I don't know why anyone would use plain text for a network model but you never know :P
        /// </summary>
        /// <param name="pathName">The path where the *.txt file should be saved.</param>
        public void ToTextFile(string pathName)
        {
            try
            {
                StringBuilder stringBuilder = new StringBuilder();

                foreach (Company company in m_networkModel.Companies)
                {
                    stringBuilder.AppendLine(company.ToString());
                    foreach (Division division in company.Divisions)
                    {
                        stringBuilder.AppendLine(division.ToString());
                        foreach (Substation substation in division.Substations)
                        {
                            stringBuilder.AppendLine(substation.ToString());
                            foreach (Node node in substation.Nodes)
                            {
                                stringBuilder.AppendLine(node.ToString());
                                stringBuilder.AppendLine(node.Voltage.ToString());
                            }
                            foreach (CircuitBreaker circuitBreaker in substation.CircuitBreakers)
                            {
                                stringBuilder.AppendLine(circuitBreaker.ToString());
                            }
                            foreach (Switch circuitSwitch in substation.Switches)
                            {
                                stringBuilder.AppendLine(circuitSwitch.ToString());
                            }
                            foreach (ShuntCompensator shunt in substation.Shunts)
                            {
                                stringBuilder.AppendLine(shunt.ToString());
                            }
                        }

                        foreach (TransmissionLine transmissionLine in division.TransmissionLines)
                        {
                            stringBuilder.AppendLine(transmissionLine.ToString());
                            stringBuilder.AppendLine(transmissionLine.FromSubstationCurrent.ToString());
                            stringBuilder.AppendLine(transmissionLine.ToSubstationCurrent.ToString());
                            foreach (LineSegment lineSegment in transmissionLine.LineSegments)
                            {
                                stringBuilder.AppendLine(lineSegment.ToString());
                            }
                        }
                    }
                }
                File.WriteAllText(pathName, stringBuilder.ToString());
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to cache the network as a text file: " + exception.ToString());
            }
        }

        /// <summary>
        /// Will throw a <b>NotImplementedException</b>
        /// </summary>
        /// <param name="pathName">The path to retrieve the *.txt file from</param>
        /// <returns>A reinstantiated <see cref="LinearStateEstimator.Networks.Network"/> object.</returns>
        public static Network FromTextFile(string pathName)
        {
            throw new NotImplementedException();
        }
        
        public static Network FromPsseRawFile(string pathName, string version)
        {
            try
            {
                if (version == "33")
                {

                    // Read the raw file from the disk
                    RawFile rawFile = RawFile.Read(pathName);

                    // Create some placeholder objects
                    List<VoltageLevel> voltageLevels = new List<VoltageLevel>();
                    List<Node> nodes = new List<Node>();
                    List<LineSegment> lineSegments = new List<LineSegment>();
                    List<Switch> switches = new List<Switch>();
                    List<CircuitBreaker> breakers = new List<CircuitBreaker>();
                    List<Substation> substations = new List<Substation>();
                    List<TransmissionLine> transmissionLines = new List<TransmissionLine>();
                    List<Company> companies = new List<Company>();
                    List<Division> divisions = new List<Division>();
                    List<Double> baseKvs = new List<Double>();

                    #region [ Modeling VoltageLevels ]

                    foreach (Bus bus in rawFile.Buses)
                    {
                        if (!baseKvs.Contains(bus.BaseKv))
                        {
                            baseKvs.Add(bus.BaseKv);
                        }
                    }

                    for (int i = 0; i < baseKvs.Count(); i++)
                    {
                        voltageLevels.Add(new VoltageLevel()
                        {
                            InternalID = i + 1,
                            Value = baseKvs[i]
                        });
                    }

                    #endregion

                    #region [ Modeling Nodes]

                    foreach (Bus bus in rawFile.Buses)
                    {
                        VoltageLevel baseKv = null;

                        foreach (VoltageLevel voltageLevel in voltageLevels)
                        {
                            if (bus.BaseKv == voltageLevel.Value)
                            {
                                baseKv = voltageLevel;
                            }
                        }

                        string name = bus.Name.Trim().ToLower();
                        name = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(name);

                        Node node = new Node()
                        {
                            InternalID = bus.Number,
                            Number = bus.Number,
                            Acronym = name.ToUpper().Trim(),
                            Name = name,
                            Description = name,
                            BaseKV = baseKv
                        };

                        VoltagePhasorGroup voltage = new VoltagePhasorGroup()
                        {
                            InternalID = bus.Number,
                            Number = bus.Number,
                            Acronym = bus.Name.ToUpper().Trim(),
                            Name = bus.Name.Trim() + " Voltage Phasor Group",
                            Description = bus.Name.Trim() + " Voltage Phasor Group",
                            IsEnabled = true,
                            UseStatusFlagForRemovingMeasurements = true,
                            MeasuredNode = node,
                        };

                        voltage.ZeroSequence.Measurement.BaseKV = node.BaseKV;
                        voltage.ZeroSequence.Estimate.BaseKV = node.BaseKV;
                        voltage.NegativeSequence.Measurement.BaseKV = node.BaseKV;
                        voltage.NegativeSequence.Estimate.BaseKV = node.BaseKV;
                        voltage.PositiveSequence.Measurement.BaseKV = node.BaseKV;
                        voltage.PositiveSequence.Estimate.BaseKV = node.BaseKV;
                        voltage.PhaseA.Measurement.BaseKV = node.BaseKV;
                        voltage.PhaseA.Estimate.BaseKV = node.BaseKV;
                        voltage.PhaseB.Measurement.BaseKV = node.BaseKV;
                        voltage.PhaseB.Estimate.BaseKV = node.BaseKV;
                        voltage.PhaseC.Measurement.BaseKV = node.BaseKV;
                        voltage.PhaseC.Estimate.BaseKV = node.BaseKV;

                        node.Voltage = voltage;

                        nodes.Add(node);
                    }

                    #endregion

                    #region [ Modeling Circuit Breakers, Switches, and Line Segments ]

                    foreach (Branch branch in rawFile.Branches)
                    {
                        Node fromNode = null;
                        Node toNode = null;

                        foreach (Node node in nodes)
                        {
                            if (node.InternalID == branch.FromBusNumber)
                            {
                                fromNode = node;
                            }
                            if (node.InternalID == branch.ToBusNumber)
                            {
                                toNode = node;
                            }
                        }

                        if (branch.IsBreaker)
                        {
                            CircuitBreaker breaker = new CircuitBreaker()
                            {
                                InternalID = rawFile.Branches.IndexOf(branch) + 1,
                                Number = rawFile.Branches.IndexOf(branch) + 1,
                                Name = $"From: {branch.FromBusNumber} To: {branch.ToBusNumber} Id: {branch.CircuitId}",
                                Description = $"From: {branch.FromBusNumber} To: {branch.ToBusNumber} Id: {branch.CircuitId}",
                                NormalState = SwitchingDeviceNormalState.Closed,
                                ActualState = SwitchingDeviceActualState.Closed,
                                MeasurementKey = "Undefined",
                                FromNode = fromNode,
                                ToNode = toNode,
                            };
                            breakers.Add(breaker);
                        }
                        else if (branch.IsSwitch)
                        {
                            Switch switchingDevice = new Switch()
                            {
                                InternalID = rawFile.Branches.IndexOf(branch) + 1,
                                Number = rawFile.Branches.IndexOf(branch) + 1,
                                Name = $"From: {branch.FromBusNumber} To: {branch.ToBusNumber} Id: {branch.CircuitId}",
                                Description = $"From: {branch.FromBusNumber} To: {branch.ToBusNumber} Id: {branch.CircuitId}",
                                NormalState = SwitchingDeviceNormalState.Closed,
                                ActualState = SwitchingDeviceActualState.Closed,
                                MeasurementKey = "Undefined",
                                FromNode = fromNode,
                                ToNode = toNode,
                            };
                            switches.Add(switchingDevice);
                        }
                        else
                        {
                            Impedance impedance = new Impedance()
                            {
                                R1 = branch.Resistance,
                                X1 = branch.Reactance,
                                B1 = branch.Charging,
                                R3 = branch.Resistance,
                                X3 = branch.Reactance,
                                B3 = branch.Charging,
                                R6 = branch.Resistance,
                                X6 = branch.Reactance,
                                B6 = branch.Charging,
                            };

                            // a line segment
                            LineSegment lineSegment = new LineSegment()
                            {
                                InternalID = rawFile.Branches.IndexOf(branch),
                                Number = rawFile.Branches.IndexOf(branch),
                                Name = $"From: {branch.FromBusNumber} To: {branch.ToBusNumber} Id: {branch.CircuitId}",
                                Description = $"From: {branch.FromBusNumber} To: {branch.ToBusNumber} Id: {branch.CircuitId}",
                                RawImpedanceParameters = impedance,
                                FromNode = fromNode,
                                ToNode = toNode,
                            };

                            lineSegments.Add(lineSegment);
                        }
                    }

                    #endregion

                    #region [ Modeling Substations ] 

                    List<List<int>> substationBusNumberGroups = rawFile.SubstationBusNumberGroups;

                    foreach (List<int> substationGroup in substationBusNumberGroups)
                    {
                        List<Node> substationNodes = new List<Node>();
                        List<CircuitBreaker> substationBreakers = new List<CircuitBreaker>();
                        List<Switch> substationSwitches = new List<Switch>();

                        foreach (int busNumber in substationGroup)
                        {
                            foreach (Node node in nodes)
                            {
                                if (node.InternalID == busNumber)
                                {
                                    substationNodes.Add(node);
                                }
                            }

                            foreach (CircuitBreaker breaker in breakers)
                            {
                                if (breaker.FromNode.InternalID == busNumber)
                                {
                                    substationBreakers.Add(breaker);
                                }
                            }

                            foreach (Switch switchingDevice in switches)
                            {
                                if (switchingDevice.FromNode.InternalID == busNumber)
                                {
                                    substationSwitches.Add(switchingDevice);
                                }
                            }
                        }

                        string name = substationNodes[0].Name.Split('_')[0].ToLower();
                        name = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(name);
                        substations.Add(new Substation()
                        {
                            InternalID = substationBusNumberGroups.IndexOf(substationGroup) + 1,
                            Number = substationBusNumberGroups.IndexOf(substationGroup) + 1,
                            Acronym = name.ToUpper(),
                            Name = name,
                            Description = $"{name} Substation",
                            Nodes = substationNodes,
                            CircuitBreakers = substationBreakers,
                            Switches = substationSwitches,
                        });

                        foreach (Node node in substationNodes)
                        {
                            node.Name = node.Name.Replace('_', ' ');
                            node.Acronym = node.Acronym.Replace('_', ' ');
                            node.Description = node.Description.Replace('_', ' ');
                            node.Voltage.Acronym = node.Acronym + "-V";
                            node.Voltage.Name = node.Name + " Voltage Phasor Group";
                            node.Voltage.Description = node.Name + " Voltage Phasor Group";
                        }
                    }

                    #endregion

                    #region [ Modeling TransmissionLines ]

                    int currentFlowIntegerIndex = 1;

                    foreach (LineSegment lineSegment in lineSegments)
                    {
                        TransmissionLine transmissionLine = new TransmissionLine()
                        {
                            InternalID = lineSegment.InternalID,
                            Number = lineSegment.Number,
                            Acronym = lineSegment.Acronym,
                            Name = lineSegment.Name,
                            Description = lineSegment.Description,
                            FromNode = lineSegment.FromNode,
                            ToNode = lineSegment.ToNode,
                            FromSubstation = lineSegment.FromNode.ParentSubstation,
                            ToSubstation = lineSegment.ToNode.ParentSubstation,
                        };

                        transmissionLine.LineSegments.Add(lineSegment);

                        transmissionLine.FromNode.ParentTransmissionLine = transmissionLine;
                        transmissionLine.ToNode.ParentTransmissionLine = transmissionLine;

                        transmissionLine.FromSubstationCurrent = new CurrentFlowPhasorGroup()
                        {
                            InternalID = currentFlowIntegerIndex,
                            Number = currentFlowIntegerIndex,
                            Acronym = transmissionLine.FromNode.Acronym + "-I",
                            Name = transmissionLine.FromNode.Name + " Current Phasor Group",
                            Description = transmissionLine.FromNode.Name + " Current Phasor Group",
                            IsEnabled = true,
                            UseStatusFlagForRemovingMeasurements = true,
                            MeasuredBranch = transmissionLine,
                            MeasuredFromNode = transmissionLine.FromNode,
                            MeasuredToNode = transmissionLine.ToNode
                        };

                        transmissionLine.FromSubstationCurrent.ZeroSequence.Measurement.BaseKV = transmissionLine.FromNode.BaseKV;
                        transmissionLine.FromSubstationCurrent.ZeroSequence.Estimate.BaseKV = transmissionLine.FromNode.BaseKV;
                        transmissionLine.FromSubstationCurrent.NegativeSequence.Measurement.BaseKV = transmissionLine.FromNode.BaseKV;
                        transmissionLine.FromSubstationCurrent.NegativeSequence.Estimate.BaseKV = transmissionLine.FromNode.BaseKV;
                        transmissionLine.FromSubstationCurrent.PositiveSequence.Measurement.BaseKV = transmissionLine.FromNode.BaseKV;
                        transmissionLine.FromSubstationCurrent.PositiveSequence.Estimate.BaseKV = transmissionLine.FromNode.BaseKV;
                        transmissionLine.FromSubstationCurrent.PhaseA.Measurement.BaseKV = transmissionLine.FromNode.BaseKV;
                        transmissionLine.FromSubstationCurrent.PhaseA.Estimate.BaseKV = transmissionLine.FromNode.BaseKV;
                        transmissionLine.FromSubstationCurrent.PhaseB.Measurement.BaseKV = transmissionLine.FromNode.BaseKV;
                        transmissionLine.FromSubstationCurrent.PhaseB.Estimate.BaseKV = transmissionLine.FromNode.BaseKV;
                        transmissionLine.FromSubstationCurrent.PhaseC.Measurement.BaseKV = transmissionLine.FromNode.BaseKV;
                        transmissionLine.FromSubstationCurrent.PhaseC.Estimate.BaseKV = transmissionLine.FromNode.BaseKV;

                        transmissionLine.ToSubstationCurrent = new CurrentFlowPhasorGroup()
                        {
                            InternalID = currentFlowIntegerIndex + 1,
                            Number = currentFlowIntegerIndex + 1,
                            Acronym = transmissionLine.ToNode.Acronym + "-I",
                            Name = transmissionLine.ToNode.Name + " Current Phasor Group",
                            Description = transmissionLine.ToNode.Name + " Current Phasor Group",
                            IsEnabled = true,
                            UseStatusFlagForRemovingMeasurements = true,
                            MeasuredBranch = transmissionLine,
                            MeasuredFromNode = transmissionLine.ToNode,
                            MeasuredToNode = transmissionLine.FromNode
                        };

                        transmissionLine.ToSubstationCurrent.ZeroSequence.Measurement.BaseKV = transmissionLine.ToNode.BaseKV;
                        transmissionLine.ToSubstationCurrent.ZeroSequence.Estimate.BaseKV = transmissionLine.ToNode.BaseKV;
                        transmissionLine.ToSubstationCurrent.NegativeSequence.Measurement.BaseKV = transmissionLine.ToNode.BaseKV;
                        transmissionLine.ToSubstationCurrent.NegativeSequence.Estimate.BaseKV = transmissionLine.ToNode.BaseKV;
                        transmissionLine.ToSubstationCurrent.PositiveSequence.Measurement.BaseKV = transmissionLine.ToNode.BaseKV;
                        transmissionLine.ToSubstationCurrent.PositiveSequence.Estimate.BaseKV = transmissionLine.ToNode.BaseKV;
                        transmissionLine.ToSubstationCurrent.PhaseA.Measurement.BaseKV = transmissionLine.ToNode.BaseKV;
                        transmissionLine.ToSubstationCurrent.PhaseA.Estimate.BaseKV = transmissionLine.ToNode.BaseKV;
                        transmissionLine.ToSubstationCurrent.PhaseB.Measurement.BaseKV = transmissionLine.ToNode.BaseKV;
                        transmissionLine.ToSubstationCurrent.PhaseB.Estimate.BaseKV = transmissionLine.ToNode.BaseKV;
                        transmissionLine.ToSubstationCurrent.PhaseC.Measurement.BaseKV = transmissionLine.ToNode.BaseKV;
                        transmissionLine.ToSubstationCurrent.PhaseC.Estimate.BaseKV = transmissionLine.ToNode.BaseKV;


                        transmissionLines.Add(transmissionLine);

                        currentFlowIntegerIndex += 2;
                    }



                    #endregion

                    #region [ Modeling Divisions ]

                    divisions.Add(new Division()
                    {
                        InternalID = 1,
                        Number = 1,
                        Acronym = "SUPREMEDIV",
                        Name = "Division Supreme",
                        Description = "The Division with ALL the Toppings",
                        Substations = substations,
                        TransmissionLines = transmissionLines,
                    });

                    #endregion

                    #region [ Modeling Companies ] 

                    companies.Add(new Company()
                    {
                        InternalID = 1,
                        Number = 1,
                        Acronym = "AWESOMECO",
                        Name = "Company Awesome",
                        Description = "The Most Awesome Company Ever",
                        Divisions = divisions
                    });

                    #endregion

                    #region [ Composing the Network Model ] 

                    NetworkModel networkModel = new NetworkModel();
                    networkModel.Name = rawFile.FileName;
                    networkModel.Description = $"{rawFile.FirstTitleLine} {rawFile.SecondTitleLine}";
                    networkModel.VoltageLevels = voltageLevels;
                    networkModel.Companies = companies;

                    #endregion

                    Network network = new Network(networkModel);

                    return network;
                }
                else
                {
                    throw new Exception("Unsupported raw file version.");
                }
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to Deserialize the Network from the Configuration File: " + exception.ToString());
            }
            
        }

        public static void ConvertFromPsseRawFile(string xmlPathName, string rawPathName, string version)
        {
            FromPsseRawFile(rawPathName, version).SerializeToXml(xmlPathName);
        }

        public static Network FromHdbExport(string modelFilesConfigPathName, bool keyify, List<string> companyFilter, bool useAreaLinks)
        {
            // Deserialize the list of model files from the xml file
            ModelFiles modelFiles = ModelFiles.DeserializeFromXml(modelFilesConfigPathName);

            // Aggregate the data from all of the files into the context
            HdbContext context = new HdbContext(modelFiles);

            // Create some placeholder objects for the model build
            NetworkModel networkModel = new NetworkModel();
            List<Company> companies = new List<Company>();
            List<Division> divisions = new List<Division>();
            List<Substation> substations = new List<Substation>();
            List<Node> nodes = new List<Node>();
            List<VoltageLevel> voltageLevels = new List<VoltageLevel>();
            List<CircuitBreaker> breakers = new List<CircuitBreaker>();
            List<Switch> switches = new List<Switch>();
            List<ShuntCompensator> shunts = new List<ShuntCompensator>();
            List<LineSegment> lineSegments = new List<LineSegment>();
            List<TransmissionLine> transmissionLines = new List<TransmissionLine>();
            List<Transformer> transformers = new List<Transformer>();
            List<TapConfiguration> taps = new List<TapConfiguration>();
            List<BreakerStatus> breakerStatuses = new List<BreakerStatus>();

            #region [ Modeling Companies ]

            Console.WriteLine("Adding Companies...");

            for (int i = 0; i < context.Companies.Count; i++)
            {
                companies.Add(new Company()
                {
                    InternalID = context.Companies[i].Number,
                    Number = context.Companies[i].Number,
                    Acronym = context.Companies[i].Name.ToUpper(),
                    Name = context.Companies[i].Name,
                    Description = context.Companies[i].ToString(),
                });
            }

            #endregion

            #region [ Modeling Divisions ]

            Console.WriteLine("Adding Divisions...");

            for (int i = 0; i < context.Divisions.Count; i++)
            {

                Division division = new Division()
                {
                    InternalID = context.Divisions[i].Number,
                    Number = context.Divisions[i].Number,
                    Acronym = context.Divisions[i].Name.ToUpper(),
                    Name = context.Divisions[i].Name,
                    Description = context.Divisions[i].Name,
                };

                divisions.Add(division);

                if (useAreaLinks)
                {
                    string areaName = context.Areas.Find(x => x.Number == context.Divisions[i].AreaNumber).Name;
                    Company parentCompany = companies.Find(x => x.Name == areaName);
                    division.ParentCompany = parentCompany;
                    parentCompany.Divisions.Add(division);
                }
            }

            #endregion

            #region [ Modeling Substations ]

            Console.WriteLine("Adding Substations...");


            for (int i = 0; i < context.Stations.Count; i++)
            {
                var record = context.Stations[i];

                substations.Add(new Substation()
                {
                    InternalID = record.Number,
                    Number = record.Number,
                    Acronym = record.Name.ToUpper(),
                    Name = record.Name,
                    Description = $"{record.Name} Substation"
                });
            }

            #endregion

            #region [ Modeling Nodes, Voltage Phasor Groups,  & Voltage Levels ]

            Console.WriteLine("Adding Nodes...");

            for (int i = 0; i < context.Nodes.Count; i++)
            {
                var record = context.Nodes[i];
                Substation parentSubstation = substations.Find(x => x.Name == record.StationName);
                Company parentCompany = companies.Find(x => x.Name == record.CompanyName);

                Division parentDivision = null;

                if (useAreaLinks)
                {
                    parentDivision = parentCompany.Divisions.Find(x => x.Name == record.DivisionName);
                }
                else
                {
                    parentDivision = divisions.Find(x => x.Name == record.DivisionName);
                }

                VoltageLevel voltageLevel = null;
                lock (voltageLevels)
                {
                    voltageLevel = voltageLevels.Find(x => x.Value == record.BaseKv);
                    if (voltageLevel == null)
                    {
                        voltageLevel = new VoltageLevel(voltageLevels.Count + 1, record.BaseKv);
                        voltageLevels.Add(voltageLevel);
                    }
                }

                Node node = new Node()
                {
                    InternalID = record.Number,
                    Number = record.Number,
                    Acronym = record.Id.ToUpper(),
                    Name = record.StationName + "_" + record.Id,
                    Description = record.StationName + "_" + record.Id,
                    ParentSubstation = parentSubstation,
                    BaseKV = voltageLevel
                };

                VoltagePhasorGroup voltage = new VoltagePhasorGroup()
                {
                    InternalID = node.InternalID,
                    Number = node.Number,
                    Acronym = node.Name.ToUpper() + "-V",
                    Name = node.Name + " Voltage Phasor Group",
                    Description = node.Description + " Voltage Phasor Group",
                    IsEnabled = true,
                    UseStatusFlagForRemovingMeasurements = true,
                    MeasuredNode = node,
                };

                voltage.ZeroSequence.Measurement.BaseKV = node.BaseKV;
                voltage.ZeroSequence.Estimate.BaseKV = node.BaseKV;
                voltage.NegativeSequence.Measurement.BaseKV = node.BaseKV;
                voltage.NegativeSequence.Estimate.BaseKV = node.BaseKV;
                voltage.PositiveSequence.Measurement.BaseKV = node.BaseKV;
                voltage.PositiveSequence.Estimate.BaseKV = node.BaseKV;
                voltage.PhaseA.Measurement.BaseKV = node.BaseKV;
                voltage.PhaseA.Estimate.BaseKV = node.BaseKV;
                voltage.PhaseB.Measurement.BaseKV = node.BaseKV;
                voltage.PhaseB.Estimate.BaseKV = node.BaseKV;
                voltage.PhaseC.Measurement.BaseKV = node.BaseKV;
                voltage.PhaseC.Estimate.BaseKV = node.BaseKV;

                if (keyify)
                {
                    voltage.Keyify(node.Name);
                }

                node.Voltage = voltage;
                lock (nodes)
                {
                    nodes.Add(node);
                }

                lock (parentSubstation.Nodes)
                {
                    parentSubstation.Nodes.Add(node);
                }
                if (!parentDivision.Substations.Contains(parentSubstation))
                {
                    lock (parentDivision)
                    {
                        parentDivision.Substations.Add(parentSubstation);
                    }
                }
                if (!parentCompany.Divisions.Contains(parentDivision))
                {
                    lock (parentCompany)
                    {
                        parentCompany.Divisions.Add(parentDivision);
                    }
                }

            }

            #endregion

            #region [ Modeling Circuit Breakers & Switches ]

            Console.WriteLine("Adding Circuit Breakers and Switches");

            for (int i = 0; i < context.CircuitBreakers.Count; i++)
            {

                var record = context.CircuitBreakers[i];
                string fromNodeName = record.StationName + "_" + record.FromNodeId;
                string toNodeName = record.StationName + "_" + record.ToNodeId;

                Node fromNode = nodes.Find(x => x.Name == fromNodeName);
                Node toNode = nodes.Find(x => x.Name == toNodeName);

                string measurementKey = "Undefined";

                if (keyify)
                {
                    measurementKey = $"{record.StationName}.{record.Id}.{record.Type}";
                }

                if (record.Type == "CB" || record.Type == "C")
                {

                    CircuitBreaker circuitBreaker = new CircuitBreaker()
                    {
                        InternalID = record.Number,
                        Number = record.Number,
                        Name = record.StationName + "_" + record.Id,
                        Description = record.ToString(),
                        FromNode = fromNode,
                        ToNode = toNode,
                        ParentSubstation = fromNode.ParentSubstation,
                        NormalState = SwitchingDeviceNormalState.Closed,
                        ActualState = SwitchingDeviceActualState.Closed,
                        MeasurementKey = measurementKey,
                    };

                    if (record.IsNormallyOpen == "T")
                    {
                        circuitBreaker.NormalState = SwitchingDeviceNormalState.Open;
                    }

                    BreakerStatus breakerStatus = new BreakerStatus()
                    {
                        InternalID = circuitBreaker.InternalID,
                        Number = circuitBreaker.Number,
                        Name = circuitBreaker.Name,
                        Description = circuitBreaker.Description,
                        BitPosition = BreakerStatusBit.PSV64,
                        ParentCircuitBreaker = circuitBreaker,
                        IsEnabled = false,
                    };

                    if (keyify)
                    {
                        breakerStatus.Keyify($"{circuitBreaker.Name}");
                    }

                    circuitBreaker.Status = breakerStatus;

                    lock (breakerStatuses)
                    {
                        breakerStatuses.Add(breakerStatus);
                    }
                    lock (fromNode.ParentSubstation.CircuitBreakers)
                    {
                        fromNode.ParentSubstation.CircuitBreakers.Add(circuitBreaker);
                    }
                    lock (breakers)
                    {
                        breakers.Add(circuitBreaker);
                    }
                }
                else
                {
                    // switch
                    Switch circuitSwitch = new Switch()
                    {
                        InternalID = record.Number,
                        Number = record.Number,
                        Name = record.StationName + "_" + record.Id,
                        Description = record.ToString(),
                        FromNode = fromNode,
                        ToNode = toNode,
                        ParentSubstation = fromNode.ParentSubstation,
                        NormalState = SwitchingDeviceNormalState.Closed,
                        ActualState = SwitchingDeviceActualState.Closed,
                        MeasurementKey = measurementKey,
                    };

                    if (record.IsNormallyOpen == "T")
                    {
                        circuitSwitch.NormalState = SwitchingDeviceNormalState.Open;
                    }

                    lock (fromNode.ParentSubstation.Switches)
                    {
                        fromNode.ParentSubstation.Switches.Add(circuitSwitch);
                    }
                    lock (switches)
                    {
                        switches.Add(circuitSwitch);
                    }
                }
            }

            #endregion

            #region [ Modeling Shunt Compensators ]

            Console.WriteLine("Adding Shunt Compensators...");

            for (int i = 0; i < context.Shunts.Count; i++)
            {
                string nodeName = context.Shunts[i].StationName + "_" + context.Shunts[i].NodeId;

                Node node = nodes.Find(x => x.Name == nodeName);

                ShuntCompensator shunt = new ShuntCompensator()
                {
                    InternalID = context.Shunts[i].Number,
                    Number = context.Shunts[i].Number,
                    Name = context.Shunts[i].StationName + "_" + context.Shunts[i].Id,
                    Description = $"{context.Shunts[i].StationName}_{context.Shunts[i].Id} ({context.Shunts[i].NominalMvar})",
                    ConnectedNode = node,
                    NominalMvar = context.Shunts[i].NominalMvar,
                    ParentSubstation = node.ParentSubstation,
                    ImpedanceCalculationMethod = ShuntImpedanceCalculationMethod.CalculateFromRating,
                    RawImpedanceParameters = new Impedance(),
                };

                shunt.Current = new CurrentInjectionPhasorGroup()
                {
                    InternalID = shunt.InternalID,
                    Number = shunt.Number,
                    Acronym = shunt.Name.ToUpper(),
                    Name = shunt.Name + " Current Injection Phasor Group",
                    Description = shunt.Name + " Current Injection Phasor Group",
                    IsEnabled = true,
                    UseStatusFlagForRemovingMeasurements = true,
                    MeasuredBranch = shunt,
                    MeasuredConnectedNode = shunt.ConnectedNode
                };

                shunt.Current.ZeroSequence.Measurement.BaseKV = shunt.ConnectedNode.BaseKV;
                shunt.Current.ZeroSequence.Estimate.BaseKV = shunt.ConnectedNode.BaseKV;
                shunt.Current.NegativeSequence.Measurement.BaseKV = shunt.ConnectedNode.BaseKV;
                shunt.Current.NegativeSequence.Estimate.BaseKV = shunt.ConnectedNode.BaseKV;
                shunt.Current.PositiveSequence.Measurement.BaseKV = shunt.ConnectedNode.BaseKV;
                shunt.Current.PositiveSequence.Estimate.BaseKV = shunt.ConnectedNode.BaseKV;
                shunt.Current.PhaseA.Measurement.BaseKV = shunt.ConnectedNode.BaseKV;
                shunt.Current.PhaseA.Estimate.BaseKV = shunt.ConnectedNode.BaseKV;
                shunt.Current.PhaseB.Measurement.BaseKV = shunt.ConnectedNode.BaseKV;
                shunt.Current.PhaseB.Estimate.BaseKV = shunt.ConnectedNode.BaseKV;
                shunt.Current.PhaseC.Measurement.BaseKV = shunt.ConnectedNode.BaseKV;
                shunt.Current.PhaseC.Estimate.BaseKV = shunt.ConnectedNode.BaseKV;

                if (keyify)
                {
                    shunt.Current.Keyify($"{shunt.Name}");
                }
                node.ParentSubstation.Shunts.Add(shunt);
                shunts.Add(shunt);
            }


            #endregion

            #region [ Modeling Line Segments, Transmission Lines, & Current Flow Phasor Groups ]

            Console.WriteLine("Adding Line Segments, Transmission Lines, & Current Flow Phasor Groups...");

            int currentFlowIntegerIndex = 1;

            for (int i = 0; i < context.LineSegments.Count; i++)
            {
                string fromNodeName = $"{context.LineSegments[i].FromStationName}_{context.LineSegments[i].FromNodeId}";
                string toNodeName = $"{context.LineSegments[i].ToStationName}_{context.LineSegments[i].ToNodeId}";

                Node fromNode = nodes.Find(x => x.Name == fromNodeName);
                Node toNode = nodes.Find(x => x.Name == toNodeName);

                Division parentDivision = divisions.Find(x => x.Name == context.LineSegments[i].DivisionName);

                Impedance impedance = new Impedance()
                {
                    R1 = context.LineSegments[i].Resistance / 100,
                    X1 = context.LineSegments[i].Reactance / 100,
                    B1 = context.LineSegments[i].LineCharging / 100,
                    R3 = context.LineSegments[i].Resistance / 100,
                    X3 = context.LineSegments[i].Reactance / 100,
                    B3 = context.LineSegments[i].LineCharging / 100,
                    R6 = context.LineSegments[i].Resistance / 100,
                    X6 = context.LineSegments[i].Reactance / 100,
                    B6 = context.LineSegments[i].LineCharging / 100,
                };

                LineSegment lineSegment = new LineSegment()
                {
                    InternalID = context.LineSegments[i].Number,
                    Number = context.LineSegments[i].Number,
                    Acronym = context.LineSegments[i].TransmissionLineId,
                    Name = context.LineSegments[i].TransmissionLineId,
                    Description = context.LineSegments[i].TransmissionLineId,
                    FromNode = fromNode,
                    ToNode = toNode,
                    RawImpedanceParameters = impedance,
                };

                var line = context.TransmissionLines.Find(x => x.Id == context.LineSegments[i].TransmissionLineId);

                TransmissionLine transmissionLine = new TransmissionLine()
                {
                    InternalID = line.Number,
                    Number = line.Number,
                    Name = lineSegment.Name,
                    Acronym = lineSegment.Acronym,
                    Description = lineSegment.Description,
                    FromNode = fromNode,
                    ToNode = toNode,
                    FromSubstation = fromNode.ParentSubstation,
                    ToSubstation = toNode.ParentSubstation,
                    ParentDivision = parentDivision,
                };

                transmissionLine.FromNode.ParentTransmissionLine = transmissionLine;
                transmissionLine.ToNode.ParentTransmissionLine = transmissionLine;

                transmissionLine.FromSubstationCurrent = new CurrentFlowPhasorGroup()
                {
                    InternalID = currentFlowIntegerIndex,
                    Number = currentFlowIntegerIndex,
                    Acronym = fromNode.Acronym + "-I",
                    Name = fromNode.Name + " Current Flow Phasor Group",
                    Description = fromNode.Description + " Current Flow Phasor Group",
                    IsEnabled = true,
                    UseStatusFlagForRemovingMeasurements = true,
                    MeasuredBranch = transmissionLine,
                    MeasuredFromNode = transmissionLine.FromNode,
                    MeasuredToNode = transmissionLine.ToNode
                };

                transmissionLine.FromSubstationCurrent.ZeroSequence.Measurement.BaseKV = transmissionLine.FromNode.BaseKV;
                transmissionLine.FromSubstationCurrent.ZeroSequence.Estimate.BaseKV = transmissionLine.FromNode.BaseKV;
                transmissionLine.FromSubstationCurrent.NegativeSequence.Measurement.BaseKV = transmissionLine.FromNode.BaseKV;
                transmissionLine.FromSubstationCurrent.NegativeSequence.Estimate.BaseKV = transmissionLine.FromNode.BaseKV;
                transmissionLine.FromSubstationCurrent.PositiveSequence.Measurement.BaseKV = transmissionLine.FromNode.BaseKV;
                transmissionLine.FromSubstationCurrent.PositiveSequence.Estimate.BaseKV = transmissionLine.FromNode.BaseKV;
                transmissionLine.FromSubstationCurrent.PhaseA.Measurement.BaseKV = transmissionLine.FromNode.BaseKV;
                transmissionLine.FromSubstationCurrent.PhaseA.Estimate.BaseKV = transmissionLine.FromNode.BaseKV;
                transmissionLine.FromSubstationCurrent.PhaseB.Measurement.BaseKV = transmissionLine.FromNode.BaseKV;
                transmissionLine.FromSubstationCurrent.PhaseB.Estimate.BaseKV = transmissionLine.FromNode.BaseKV;
                transmissionLine.FromSubstationCurrent.PhaseC.Measurement.BaseKV = transmissionLine.FromNode.BaseKV;
                transmissionLine.FromSubstationCurrent.PhaseC.Estimate.BaseKV = transmissionLine.FromNode.BaseKV;

                transmissionLine.ToSubstationCurrent = new CurrentFlowPhasorGroup()
                {
                    InternalID = currentFlowIntegerIndex + 1,
                    Number = currentFlowIntegerIndex + 1,
                    Acronym = toNode.Acronym + "-I",
                    Name = toNode.Name + " Current Flow Phasor Group",
                    Description = toNode.Description + " Current Flow Phasor Group",
                    IsEnabled = true,
                    UseStatusFlagForRemovingMeasurements = true,
                    MeasuredBranch = transmissionLine,
                    MeasuredFromNode = transmissionLine.ToNode,
                    MeasuredToNode = transmissionLine.FromNode
                };

                transmissionLine.ToSubstationCurrent.ZeroSequence.Measurement.BaseKV = transmissionLine.ToNode.BaseKV;
                transmissionLine.ToSubstationCurrent.ZeroSequence.Estimate.BaseKV = transmissionLine.ToNode.BaseKV;
                transmissionLine.ToSubstationCurrent.NegativeSequence.Measurement.BaseKV = transmissionLine.ToNode.BaseKV;
                transmissionLine.ToSubstationCurrent.NegativeSequence.Estimate.BaseKV = transmissionLine.ToNode.BaseKV;
                transmissionLine.ToSubstationCurrent.PositiveSequence.Measurement.BaseKV = transmissionLine.ToNode.BaseKV;
                transmissionLine.ToSubstationCurrent.PositiveSequence.Estimate.BaseKV = transmissionLine.ToNode.BaseKV;
                transmissionLine.ToSubstationCurrent.PhaseA.Measurement.BaseKV = transmissionLine.ToNode.BaseKV;
                transmissionLine.ToSubstationCurrent.PhaseA.Estimate.BaseKV = transmissionLine.ToNode.BaseKV;
                transmissionLine.ToSubstationCurrent.PhaseB.Measurement.BaseKV = transmissionLine.ToNode.BaseKV;
                transmissionLine.ToSubstationCurrent.PhaseB.Estimate.BaseKV = transmissionLine.ToNode.BaseKV;
                transmissionLine.ToSubstationCurrent.PhaseC.Measurement.BaseKV = transmissionLine.ToNode.BaseKV;
                transmissionLine.ToSubstationCurrent.PhaseC.Estimate.BaseKV = transmissionLine.ToNode.BaseKV;

                if (keyify)
                {
                    transmissionLine.FromSubstationCurrent.Keyify($"{fromNode.Name}");
                    transmissionLine.ToSubstationCurrent.Keyify($"{toNode.Name}");
                }

                lineSegment.ParentTransmissionLine = transmissionLine;

                transmissionLine.LineSegments.Add(lineSegment);
                parentDivision.TransmissionLines.Add(transmissionLine);

                transmissionLines.Add(transmissionLine);
                lineSegments.Add(lineSegment);

                currentFlowIntegerIndex += 2;
            }

            #endregion

            #region [ Modeling Transformer Taps ]

            Console.WriteLine("Adding Transformer Taps...");

            for (int i = 0; i < context.TransformerTaps.Count; i++)
            {
                int minPosition = context.TransformerTaps[i].MinimumPosition;
                int maxPosition = context.TransformerTaps[i].MaximumPosition;
                int nominalPosition = context.TransformerTaps[i].NominalPosition;
                double stepSize = context.TransformerTaps[i].StepSize;

                TapConfiguration tap = new TapConfiguration()
                {
                    InternalID = context.TransformerTaps[i].Number,
                    Number = context.TransformerTaps[i].Number,
                    Acronym = context.TransformerTaps[i].Id.ToUpper(),
                    Name = context.TransformerTaps[i].Id,
                    Description = context.TransformerTaps[i].Id,
                    PositionLowerBounds = minPosition,
                    PositionUpperBounds = maxPosition,
                    PositionNominal = nominalPosition,
                    LowerBounds = 1.0 + (minPosition - nominalPosition) * stepSize,
                    UpperBounds = 1.0 + (maxPosition - nominalPosition) * stepSize,
                };

                taps.Add(tap);
            }

            #endregion

            #region [ Modeling Transformers ] 

            Console.WriteLine("Adding Transformers...");

            for (int i = 0; i < context.Transformers.Count; i++)
            {
                string fromNodeName = $"{context.Transformers[i].StationName}_{context.Transformers[i].FromNodeId}";
                string toNodeName = $"{context.Transformers[i].StationName}_{context.Transformers[i].ToNodeId}";

                Node fromNode = nodes.Find(x => x.Name == fromNodeName);
                Node toNode = nodes.Find(x => x.Name == toNodeName);

                Impedance impedance = new Impedance()
                {
                    R1 = context.Transformers[i].Resistance / 100,
                    X1 = context.Transformers[i].Reactance / 100,
                    G1 = context.Transformers[i].MagnetizingConductance / 100,
                    B1 = context.Transformers[i].MagnetizingSusceptance / 100,
                    R3 = context.Transformers[i].Resistance / 100,
                    X3 = context.Transformers[i].Reactance / 100,
                    G3 = context.Transformers[i].MagnetizingConductance / 100,
                    B3 = context.Transformers[i].MagnetizingSusceptance / 100,
                    R6 = context.Transformers[i].Resistance / 100,
                    X6 = context.Transformers[i].Reactance / 100,
                    G6 = context.Transformers[i].MagnetizingConductance / 100,
                    B6 = context.Transformers[i].MagnetizingSusceptance / 100,
                };


                Transformer transformer = new Transformer()
                {
                    InternalID = context.Transformers[i].Number,
                    Number = context.Transformers[i].Number,
                    Name = $"{context.Transformers[i].StationName}_{context.Transformers[i].Parent}",
                    Description = $"{context.Transformers[i].StationName}_{context.Transformers[i].Parent}",
                    FromNode = fromNode,
                    ToNode = toNode,
                    ParentSubstation = fromNode.ParentSubstation,
                    RawImpedanceParameters = impedance,
                    UltcIsEnabled = false,
                    FromNodeConnectionType = TransformerConnectionType.Wye,
                    ToNodeConnectionType = TransformerConnectionType.Wye,
                };

                TapConfiguration tap = taps.Find(x => x.Name == context.Transformers[i].FromNodeTap);

                if (tap != null)
                {
                    transformer.Tap = tap;
                }

                transformer.FromNodeCurrent = new CurrentFlowPhasorGroup()
                {
                    InternalID = currentFlowIntegerIndex,
                    Number = currentFlowIntegerIndex,
                    Acronym = fromNode.Acronym + "-I",
                    Name = fromNode.Name + " Current Flow Phasor Group",
                    Description = fromNode.Description + " Current Flow Phasor Group",
                    IsEnabled = true,
                    UseStatusFlagForRemovingMeasurements = true,
                    MeasuredBranch = transformer,
                    MeasuredFromNode = transformer.FromNode,
                    MeasuredToNode = transformer.ToNode
                };

                transformer.FromNodeCurrent.ZeroSequence.Measurement.BaseKV = transformer.FromNode.BaseKV;
                transformer.FromNodeCurrent.ZeroSequence.Estimate.BaseKV = transformer.FromNode.BaseKV;
                transformer.FromNodeCurrent.NegativeSequence.Measurement.BaseKV = transformer.FromNode.BaseKV;
                transformer.FromNodeCurrent.NegativeSequence.Estimate.BaseKV = transformer.FromNode.BaseKV;
                transformer.FromNodeCurrent.PositiveSequence.Measurement.BaseKV = transformer.FromNode.BaseKV;
                transformer.FromNodeCurrent.PositiveSequence.Estimate.BaseKV = transformer.FromNode.BaseKV;
                transformer.FromNodeCurrent.PhaseA.Measurement.BaseKV = transformer.FromNode.BaseKV;
                transformer.FromNodeCurrent.PhaseA.Estimate.BaseKV = transformer.FromNode.BaseKV;
                transformer.FromNodeCurrent.PhaseB.Measurement.BaseKV = transformer.FromNode.BaseKV;
                transformer.FromNodeCurrent.PhaseB.Estimate.BaseKV = transformer.FromNode.BaseKV;
                transformer.FromNodeCurrent.PhaseC.Measurement.BaseKV = transformer.FromNode.BaseKV;
                transformer.FromNodeCurrent.PhaseC.Estimate.BaseKV = transformer.FromNode.BaseKV;

                transformer.ToNodeCurrent = new CurrentFlowPhasorGroup()
                {
                    InternalID = currentFlowIntegerIndex + 1,
                    Number = currentFlowIntegerIndex + 1,
                    Acronym = toNode.Acronym + "-I",
                    Name = toNode.Name + " Current Flow Phasor Group",
                    Description = toNode.Description + " Current Flow Phasor Group",
                    IsEnabled = true,
                    UseStatusFlagForRemovingMeasurements = true,
                    MeasuredBranch = transformer,
                    MeasuredFromNode = transformer.ToNode,
                    MeasuredToNode = transformer.FromNode
                };

                transformer.ToNodeCurrent.ZeroSequence.Measurement.BaseKV = transformer.ToNode.BaseKV;
                transformer.ToNodeCurrent.ZeroSequence.Estimate.BaseKV = transformer.ToNode.BaseKV;
                transformer.ToNodeCurrent.NegativeSequence.Measurement.BaseKV = transformer.ToNode.BaseKV;
                transformer.ToNodeCurrent.NegativeSequence.Estimate.BaseKV = transformer.ToNode.BaseKV;
                transformer.ToNodeCurrent.PositiveSequence.Measurement.BaseKV = transformer.ToNode.BaseKV;
                transformer.ToNodeCurrent.PositiveSequence.Estimate.BaseKV = transformer.ToNode.BaseKV;
                transformer.ToNodeCurrent.PhaseA.Measurement.BaseKV = transformer.ToNode.BaseKV;
                transformer.ToNodeCurrent.PhaseA.Estimate.BaseKV = transformer.ToNode.BaseKV;
                transformer.ToNodeCurrent.PhaseB.Measurement.BaseKV = transformer.ToNode.BaseKV;
                transformer.ToNodeCurrent.PhaseB.Estimate.BaseKV = transformer.ToNode.BaseKV;
                transformer.ToNodeCurrent.PhaseC.Measurement.BaseKV = transformer.ToNode.BaseKV;
                transformer.ToNodeCurrent.PhaseC.Estimate.BaseKV = transformer.ToNode.BaseKV;

                if (keyify)
                {
                    transformer.FromNodeCurrent.Keyify($"{fromNode.Name}");
                    transformer.ToNodeCurrent.Keyify($"{toNode.Name}");
                    transformer.Keyify($"{transformer.Name}");
                }

                currentFlowIntegerIndex += 2;

                fromNode.ParentSubstation.Transformers.Add(transformer);
                transformers.Add(transformer);
            }

            #endregion

            #region [ Composing the Network Model ]


            networkModel.Companies = companies;

            networkModel.VoltageLevels = voltageLevels;
            networkModel.TapConfigurations = taps;
            networkModel.BreakerStatuses = breakerStatuses;

            #endregion

            Network network = new Network(networkModel);

            return network;
        }

        #endregion

    }
}
