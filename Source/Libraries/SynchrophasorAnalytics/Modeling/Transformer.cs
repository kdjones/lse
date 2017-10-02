//******************************************************************************************************
//  Transformer.cs
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
//  06/24/2014 - Kevin D. Jones
//       Added logic for 'CurrentFlowPostProcessingSetting' in methods for computing estimated flows.
//  
//******************************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using MathNet.Numerics;

using MathNet.Numerics.LinearAlgebra.Complex;
using SynchrophasorAnalytics.Measurements;
using SynchrophasorAnalytics.GIC;
using SynchrophasorAnalytics.Networks;

namespace SynchrophasorAnalytics.Modeling
{
    /// <summary>
    /// The class representation of a transformer in an electric power network.
    /// </summary>
    [Serializable()]
    public class Transformer : SeriesBranchBase, IGic
    {
        #region [ Private Constants ]

        /// <summary>
        /// Default values
        /// </summary>
        private const int DEFAULT_INTERNAL_ID = 0;
        private const int DEFAULT_NUMBER = 0;
        private const string DEFAULT_NAME = "Undefined";
        private const string DEFAULT_DESCRIPTION = "Uundefined";
        private const string DEFAULT_MEASUREMENT_KEY = "Undefined";

        #endregion

        #region [ Private Members ]

        private CurrentFlowPhasorGroup m_fromNodeCurrent;
        private int m_fromNodeCurrentPhasorGroupID;

        private CurrentFlowPhasorGroup m_toNodeCurrent;
        private int m_toNodeCurrentPhasorGroupID;

        private Substation m_parentSubstation;
        private int m_parentSubstationID;

        private TapConfiguration m_tapConfiguration;
        private int m_tapConfigurationID;
        private int m_fixedTapPosition;
        private string m_tapPositionInputKey;
        private int m_tapPositionMeasurement;
        private string m_tapPositionOutputKey;
        private bool m_ultcIsEnabled;

        private TransformerConnectionType m_fromNodeConnectionType;
        private TransformerConnectionType m_toNodeConnectionType;

        private bool m_complexPowerHasBeenInitialized;
        private ComplexPowerGroup m_fromNodeComplexPower;
        private ComplexPowerGroup m_toNodeComplexPower;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// The <see cref="TransformerConnectionType"/> of the From-<see cref="Node"/> of the <see cref="Transformer"/>.
        /// </summary>
        [XmlAttribute("FromNodeConnectionType")]
        public TransformerConnectionType FromNodeConnectionType
        {
            get
            {
                return m_fromNodeConnectionType;
            }
            set
            {
                m_fromNodeConnectionType = value;
            }
        }

        /// <summary>
        /// The <see cref="TransformerConnectionType"/> of the To-<see cref="Node"/> of the <see cref="Transformer"/>.
        /// </summary>
        [XmlAttribute("ToNodeConnectionType")]
        public TransformerConnectionType ToNodeConnectionType
        {
            get
            {
                return m_toNodeConnectionType;
            }
            set
            {
                m_toNodeConnectionType = value;
            }
        }

        /// <summary>
        /// The <see cref="LinearStateEstimator.Measurements.CurrentFlowPhasorGroup"/> that originates at the <see cref="LinearStateEstimator.Modeling.ITwoTerminal.FromNode"/>
        /// </summary>
        [XmlElement("FromNodeCurrent")]
        public CurrentFlowPhasorGroup FromNodeCurrent
        {
            get 
            { 
                return m_fromNodeCurrent; 
            }
            set 
            { 
                m_fromNodeCurrent = value;
                m_fromNodeCurrentPhasorGroupID = value.InternalID;
                if (this.FromNode != null)
                {
                    m_fromNodeCurrent.MeasuredFromNode = this.FromNode;
                    m_fromNodeCurrent.MeasuredToNode = this.ToNode;
                }
            }
        }

        /// <summary>
        /// The <see cref="LinearStateEstimator.Measurements.PhasorGroup.InternalID"/> of the <see cref="LinearStateEstimator.Modeling.Transformer.FromNodeCurrent"/>.
        /// </summary>
        [XmlAttribute("FromNodeCurrentPhasorGroup")]
        public int FromNodeCurrentPhasorGroupID
        {
            get 
            { 
                return m_fromNodeCurrentPhasorGroupID; 
            }
            set 
            {
                m_fromNodeCurrentPhasorGroupID = value; 
            }
        }

        /// <summary>
        /// The <see cref="LinearStateEstimator.Measurements.CurrentFlowPhasorGroup"/> that originates at the <see cref="LinearStateEstimator.Modeling.SeriesBranchBase.ToNode"/> of the <see cref="LinearStateEstimator.Modeling.Transformer"/>
        /// </summary>
        [XmlElement("ToNodeCurrent")]
        public CurrentFlowPhasorGroup ToNodeCurrent
        {
            get 
            { 
                return m_toNodeCurrent; 
            }
            set 
            { 
                m_toNodeCurrent = value;
                m_toNodeCurrentPhasorGroupID = value.InternalID;
                if (this.ToNode != null)
                {
                    m_toNodeCurrent.MeasuredFromNode = this.ToNode;
                    m_toNodeCurrent.MeasuredToNode = this.FromNode;
                }
            }
        }

        /// <summary>
        /// The <see cref="LinearStateEstimator.Measurements.PhasorGroup.InternalID"/> of the <see cref="LinearStateEstimator.Modeling.Transformer.ToNodeCurrent"/>.
        /// </summary>
        [XmlAttribute("ToNodeCurrentPhasorGroup")]
        public int ToNodeCurrentPhasorGroupID
        {
            get 
            { 
                return m_toNodeCurrentPhasorGroupID; 
            }
            set 
            { 
                m_toNodeCurrentPhasorGroupID = value; 
            }
        }

        /// <summary>
        /// The <see cref="Substation"/> that contains this <see cref="Transformer"/>.
        /// </summary>
        [XmlIgnore()]
        public Substation ParentSubstation
        {
            get
            {
                return m_parentSubstation;
            }
            set
            {
                m_parentSubstation = value;
                m_parentSubstationID = value.InternalID;
            }
        }

        /// <summary>
        /// The <see cref="Substation.InternalID"/> of the <see cref="ParentSubstation"/>.
        /// </summary>
        [XmlAttribute("ParentSubstation")]
        public int ParentSubstationID
        {
            get
            {
                return m_parentSubstationID;
            }
            set
            {
                m_parentSubstationID = value;
            }
        }

        /// <summary>
        /// The nominal impedance determined by the <see cref="LinearStateEstimator.Modeling.SeriesBranchBase.RawImpedanceParameters"/>
        /// </summary>
        [XmlIgnore()]
        public Impedance NominalImpedance
        {
            get
            {
                return RawImpedanceParameters;
            }
        }

        /// <summary>
        /// The equivalent two-port pi-model impedance of the transformer assuming an off-nominal tap ratio. 
        /// </summary>
        [XmlIgnore()]
        public Impedance OffNominalImpedance
        {
            get
            {
                return ComputeOffNominalImpedance();
            }
        }

        /// <summary>
        /// The complex multiplier, a, for the primary side of the transformer
        /// </summary>
        [XmlIgnore()]
        public Complex EffectiveComplexMultiplier
        {
            get
            {
                return ComputeComplexMultiplierA();
            }
        }

        /// <summary>
        /// The <see cref="TapConfiguration"/> of the <see cref="Transformer"/>.
        /// </summary>
        [XmlIgnore()]
        public TapConfiguration Tap
        {
            get
            {
                return m_tapConfiguration;
            }
            set
            {
                m_tapConfiguration = value;
                m_tapConfigurationID = value.InternalID;
            }
        }

        /// <summary>
        /// The <see cref="TapConfiguration.InternalID"/> of the <see cref="Tap"/>.
        /// </summary>
        [XmlAttribute("TapConfiguration")]
        public int TapConfigurationID
        {
            get
            {
                return m_tapConfigurationID;
            }
            set
            {
                m_tapConfigurationID = value;
            }
        }

        /// <summary>
        /// The input measurement key for the tap position
        /// </summary>
        [XmlAttribute("TapPositionInputKey")]
        public string TapPositionInputKey
        {
            get
            {
                return m_tapPositionInputKey;
            }
            set
            {
                m_tapPositionInputKey = value;
            }
        }

        /// <summary>
        /// The input measurement for the tap position
        /// </summary>
        [XmlIgnore()]
        public int TapPositionMeasurement
        {
            get
            {
                return m_tapPositionMeasurement;
            }
            set
            {
                m_tapPositionMeasurement = value;
                if (m_tapConfiguration != null)
                {
                    m_tapConfiguration.CurrentPosition = value;
                }
            }
        }

        /// <summary>
        /// The output measurement key for the tap position
        /// </summary>
        [XmlAttribute("TapPositionOutputKey")]
        public string TapPositionOutputKey
        {
            get
            {
                return m_tapPositionOutputKey;
            }
            set
            {
                m_tapPositionOutputKey = value;
            }
        }

        /// <summary>
        /// The tap position actualy used by the model and calculations
        /// </summary>
        [XmlAttribute("AssumedTapPosition")]
        public int AssumedTapPosition
        {
            get
            {
                return m_tapConfiguration.CurrentPosition;
            }
        }

        /// <summary>
        /// A tap position used to indicate when a transformer tap is permanently or semi-permantly fixed into a particular position
        /// </summary>
        [XmlAttribute("FixedTapPosition")]
        public int FixedTapPosition
        {
            get
            {
                return m_fixedTapPosition;
            }
            set
            {
                m_fixedTapPosition = value;
            }
        }

        /// <summary>
        /// A flag which determines whether the transformer should accept measurements to determine what the actual tap setting is.
        /// </summary>
        [XmlAttribute("EnableUltc")]
        public bool UltcIsEnabled
        {
            get
            {
                return m_ultcIsEnabled;
            }
            set
            {
                m_ultcIsEnabled = value;
            }
        }

        /// <summary>
        /// The complex power flow through the <see cref="LinearStateEstimator.Modeling.Transformer"/> calculated at the <see cref="LinearStateEstimator.Modeling.ITwoTerminal.FromNode"/>.
        /// </summary>
        [XmlIgnore()]
        public ComplexPowerGroup FromNodeComplexPower
        {
            get
            {
                if (m_complexPowerHasBeenInitialized)
                {
                    return m_fromNodeComplexPower;
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
                    m_fromNodeComplexPower = value;
                }
            }
        }

        /// <summary>
        /// The complex power flow through the <see cref="LinearStateEstimator.Modeling.Transformer"/> calculated at the <see cref="LinearStateEstimator.Modeling.ITwoTerminal.ToNode"/>.
        /// </summary>
        [XmlIgnore()]
        public ComplexPowerGroup ToNodeComplexPower
        {
            get
            {
                return m_toNodeComplexPower;
            }
            set
            {
                m_toNodeComplexPower = value;
            }
        }

        /// <summary>
        /// The GIC flow in positive sequence as computed by the <see cref="LinearStateEstimator.Modeling.ITwoTerminal.FromNode"/> of the <see cref="LinearStateEstimator.Modeling.Transformer"/>.
        /// </summary>
        [XmlIgnore()]
        public double PositiveSequenceGicQFlowByFromSide
        {
            get 
            {
                // Use only measured values for GIC calculation as impedance skews results
                m_fromNodeComplexPower.UseEstimatedValues = false;
                m_fromNodeComplexPower.Compute();

                m_toNodeComplexPower.UseEstimatedValues = false;
                m_toNodeComplexPower.Compute();

                double measuredQLoss = Math.Abs(m_fromNodeComplexPower.PositiveSequenceReactivePower - m_toNodeComplexPower.PositiveSequenceReactivePower);
                double qLossByImpedanceFromSide = 100 * m_fromNodeCurrent.PositiveSequence.Measurement.PerUnitMagnitude * m_fromNodeCurrent.PositiveSequence.Measurement.PerUnitMagnitude * PositiveSequenceSeriesImpedance.Imaginary;

                m_fromNodeComplexPower.UseEstimatedValues = true;
                m_toNodeComplexPower.UseEstimatedValues = true;

                return Math.Abs(measuredQLoss - qLossByImpedanceFromSide);
            }
        }

        /// <summary>
        /// The GIC flow in positive sequence as computed by the <see cref="LinearStateEstimator.Modeling.ITwoTerminal.ToNode"/> of the <see cref="LinearStateEstimator.Modeling.Transformer"/>.
        /// </summary>
        [XmlIgnore()]
        public double PositiveSequenceGicFlowByToSide
        {
            get
            {
                // Use only measured values for GIC calculation as impedance skews results
                m_fromNodeComplexPower.UseEstimatedValues = false;
                m_fromNodeComplexPower.Compute();

                m_toNodeComplexPower.UseEstimatedValues = false;
                m_toNodeComplexPower.Compute();

                double measuredQLoss = Math.Abs(m_fromNodeComplexPower.PositiveSequenceReactivePower - m_toNodeComplexPower.PositiveSequenceReactivePower);
                double qLossByImpedanceToSide = 100 * m_toNodeCurrent.PositiveSequence.Measurement.PerUnitMagnitude * m_toNodeCurrent.PositiveSequence.Measurement.PerUnitMagnitude * PositiveSequenceSeriesImpedance.Imaginary;

                m_fromNodeComplexPower.UseEstimatedValues = true;
                m_toNodeComplexPower.UseEstimatedValues = true;

                return Math.Abs(measuredQLoss - qLossByImpedanceToSide);
            }
        }

        /// <summary>
        /// The GIC flow in three phase as computed by the <see cref="LinearStateEstimator.Modeling.ITwoTerminal.FromNode"/> of the <see cref="LinearStateEstimator.Modeling.Transformer"/>.
        /// </summary>
        [XmlIgnore()]
        public double ThreePhaseGicQFlowByFromSide
        {
            get
            {
                // Use only measured values for GIC calculation as impedance skews results
                m_fromNodeComplexPower.UseEstimatedValues = false;
                m_fromNodeComplexPower.Compute();

                m_toNodeComplexPower.UseEstimatedValues = false;
                m_toNodeComplexPower.Compute();

                double measuredQLoss = Math.Abs(m_fromNodeComplexPower.ThreePhaseReactivePower - m_toNodeComplexPower.ThreePhaseReactivePower);

                DenseMatrix fromSideCurrentFlows = DenseMatrix.OfArray(new Complex[3, 1]);
                fromSideCurrentFlows[0, 0] = m_fromNodeCurrent.PhaseA.Measurement.PerUnitComplexPhasor;
                fromSideCurrentFlows[1, 0] = m_fromNodeCurrent.PhaseB.Measurement.PerUnitComplexPhasor;
                fromSideCurrentFlows[2, 0] = m_fromNodeCurrent.PhaseC.Measurement.PerUnitComplexPhasor;

                DenseMatrix perUnitZLossByImpedanceFromSide = (fromSideCurrentFlows.Transpose() * ThreePhaseSeriesImpedance * fromSideCurrentFlows) as DenseMatrix;

                m_fromNodeComplexPower.UseEstimatedValues = true;
                m_toNodeComplexPower.UseEstimatedValues = true;

                return Math.Abs(measuredQLoss - (100 * perUnitZLossByImpedanceFromSide[0, 0].Imaginary));
            }
        }

        /// <summary>
        /// The GIC flow in three phase as computed by the <see cref="LinearStateEstimator.Modeling.ITwoTerminal.ToNode"/> of the <see cref="LinearStateEstimator.Modeling.Transformer"/>.
        /// </summary>
        [XmlIgnore()]
        public double ThreePhaseGicFlowByToSide
        {
            get
            {
                // Use only measured values for GIC calculation as impedance skews results
                m_fromNodeComplexPower.UseEstimatedValues = false;
                m_fromNodeComplexPower.Compute();

                m_toNodeComplexPower.UseEstimatedValues = false;
                m_toNodeComplexPower.Compute();

                double measuredQLoss = Math.Abs(m_fromNodeComplexPower.ThreePhaseReactivePower - m_toNodeComplexPower.ThreePhaseReactivePower);

                DenseMatrix toSideCurrentFlows = DenseMatrix.OfArray(new Complex[3, 1]);
                toSideCurrentFlows[0, 0] = m_toNodeCurrent.PhaseA.Measurement.PerUnitComplexPhasor;
                toSideCurrentFlows[1, 0] = m_toNodeCurrent.PhaseB.Measurement.PerUnitComplexPhasor;
                toSideCurrentFlows[2, 0] = m_toNodeCurrent.PhaseC.Measurement.PerUnitComplexPhasor;

                DenseMatrix perUnitZLossByImpedanceToSide = (toSideCurrentFlows.Transpose() * ThreePhaseSeriesImpedance * toSideCurrentFlows) as DenseMatrix;

                m_fromNodeComplexPower.UseEstimatedValues = true;
                m_toNodeComplexPower.UseEstimatedValues = true;

                return Math.Abs(measuredQLoss - (100 * perUnitZLossByImpedanceToSide[0, 0].Imaginary));
            }
        }
        
        #endregion

        #region [ Constructors ]

        /// <summary>
        /// A default constructor with default values.
        /// </summary>
        public Transformer()
            :this(DEFAULT_INTERNAL_ID, DEFAULT_NUMBER, DEFAULT_NAME, DEFAULT_DESCRIPTION, new Impedance())
        {
        }

        /// <summary>
        /// A constructor for the <see cref="LinearStateEstimator.Modeling.Transformer"/> class which requires all of the properties defined by the <see cref="LinearStateEstimator.Modeling.INetworkDescribable"/> interface as well as the nominal <see cref="LinearStateEstimator.Modeling.Impedance"/> values.
        /// </summary>
        /// <param name="internalID">A unique integer identifier</param>
        /// <param name="number">A non-unique integer identifier</param>
        /// <param name="name">A string name for the object</param>
        /// <param name="description">A string description for the object</param>
        /// <param name="impedance">The nominal impedance for the transformer.</param>
        public Transformer(int internalID, int number, string name, string description, Impedance impedance)
            :this(internalID, number, name, description, impedance, 0, 0)
        {
        }

        /// <summary>
        /// A constructor for the <see cref="LinearStateEstimator.Modeling.Transformer"/> class which requires all of the properties defined by the <see cref="LinearStateEstimator.Modeling.INetworkDescribable"/> interface as well as the nominal <see cref="LinearStateEstimator.Modeling.Impedance"/> values as well as the internal id of the from and to node current flow measurements.
        /// </summary>
        /// <param name="internalID">A unique integer identifier</param>
        /// <param name="number">A non-unique integer identifier</param>
        /// <param name="name">A string name for the object</param>
        /// <param name="description">A string description for the object</param>
        /// <param name="impedance">The nominal impedance for the transformer.</param>
        /// <param name="fromNodeCurrentPhasorGroupID">The internal id of the current flow phasor group on the from node.</param>
        /// <param name="toNodeCurrentPhasorGroupID">The internal id of the current flow phasor group on the to node.</param>
        public Transformer(int internalID, int number, string name, string description, Impedance impedance, int fromNodeCurrentPhasorGroupID, int toNodeCurrentPhasorGroupID)
            : this(internalID, number, name, description, impedance, fromNodeCurrentPhasorGroupID, toNodeCurrentPhasorGroupID, 0)
        {
        }

        /// <summary>
        /// A constructor for the <see cref="LinearStateEstimator.Modeling.Transformer"/> class which requires all of the properties defined by the <see cref="LinearStateEstimator.Modeling.INetworkDescribable"/> interface as well as the nominal <see cref="LinearStateEstimator.Modeling.Impedance"/> values as well as the internal id of the from and to node current flow measurements and the tap model of the transformer.
        /// </summary>
        /// <param name="internalID">A unique integer identifier</param>
        /// <param name="number">A non-unique integer identifier</param>
        /// <param name="name">A string name for the object</param>
        /// <param name="description">A string description for the object</param>
        /// <param name="impedance">The nominal impedance for the transformer.</param>
        /// <param name="fromNodeCurrentPhasorGroupID">The internal id of the current flow phasor group on the from node.</param>
        /// <param name="toNodeCurrentPhasorGroupID">The internal id of the current flow phasor group on the to node.</param>
        /// <param name="tapConfigurationID">The tap model of the transformer.</param>
        public Transformer(int internalID, int number, string name, string description, Impedance impedance, int fromNodeCurrentPhasorGroupID, int toNodeCurrentPhasorGroupID, int tapConfigurationID)
            :base(internalID, number, name, description, impedance)
        {
            m_fromNodeCurrentPhasorGroupID = fromNodeCurrentPhasorGroupID;
            m_toNodeCurrentPhasorGroupID = toNodeCurrentPhasorGroupID;
            m_tapConfigurationID = tapConfigurationID;
            m_fromNodeConnectionType = TransformerConnectionType.Wye;
            m_toNodeConnectionType = TransformerConnectionType.Wye;
            m_tapPositionInputKey = "Undefined";
            m_tapPositionOutputKey = "Undefined";
            m_complexPowerHasBeenInitialized = false;
            this.RawImpedanceParameters.ParentElement = this;
        }

        #endregion

        #region [ Public Methods ]

        /// <summary>
        /// Initializes the references for computing complex power.
        /// </summary>
        public void InitializeComplexPower()
        {
            m_fromNodeComplexPower = new ComplexPowerGroup(FromNode.Voltage, FromNodeCurrent);
            m_fromNodeComplexPower.UseEstimatedValues = true;

            m_toNodeComplexPower = new ComplexPowerGroup(ToNode.Voltage, ToNodeCurrent);
            m_toNodeComplexPower.UseEstimatedValues = true;

            m_complexPowerHasBeenInitialized = true;
        }

        /// <summary>
        /// Computes an estimated value of positive sequence current flow through the transformer based on estimated values of the high side and low side node voltages.
        /// </summary>
        public void ComputePositiveSequenceEstimatedCurrentFlow()
        {
            if (m_parentSubstation.ParentDivision.ParentCompany.ParentModel.CurrentFlowPostProcessingSetting == CurrentFlowPostProcessingSetting.ProcessOnlyMeasuredBranches)
            {
                if ((FromNodeCurrent.IncludeInPositiveSequenceEstimator || ToNodeCurrent.IncludeInPositiveSequenceEstimator) && FromNode.IsObserved && ToNode.IsObserved)
                {
                    Complex Vi = FromNode.Voltage.PositiveSequence.Estimate.PerUnitComplexPhasor;
                    Complex Vj = ToNode.Voltage.PositiveSequence.Estimate.PerUnitComplexPhasor;

                    Complex Iij = (PositiveSequenceSeriesAdmittance / (EffectiveComplexMultiplier.Magnitude * EffectiveComplexMultiplier.Magnitude)) * Vi - PositiveSequenceSeriesAdmittance / EffectiveComplexMultiplier.Conjugate() * Vj;
                    Complex Iji = -(PositiveSequenceSeriesAdmittance / EffectiveComplexMultiplier) * Vi + PositiveSequenceSeriesAdmittance * Vj;

                    FromNodeCurrent.PositiveSequence.Estimate.PerUnitComplexPhasor = Iij;
                    ToNodeCurrent.PositiveSequence.Estimate.PerUnitComplexPhasor = Iji;
                }
                else
                {
                    FromNodeCurrent.PositiveSequence.Estimate.Magnitude = 0;
                    FromNodeCurrent.PositiveSequence.Estimate.AngleInDegrees = 0;
                    ToNodeCurrent.PositiveSequence.Estimate.Magnitude = 0;
                    ToNodeCurrent.PositiveSequence.Estimate.AngleInDegrees = 0;
                }
            }
            else if (m_parentSubstation.ParentDivision.ParentCompany.ParentModel.CurrentFlowPostProcessingSetting == CurrentFlowPostProcessingSetting.ProcessBranchesByNodeObservability)
            {
                if (FromNode.IsObserved && ToNode.IsObserved)
                {
                    Complex Vi = FromNode.Voltage.PositiveSequence.Estimate.PerUnitComplexPhasor;
                    Complex Vj = ToNode.Voltage.PositiveSequence.Estimate.PerUnitComplexPhasor;

                    Complex Iij = (PositiveSequenceSeriesAdmittance / (EffectiveComplexMultiplier.Magnitude * EffectiveComplexMultiplier.Magnitude)) * Vi - PositiveSequenceSeriesAdmittance / EffectiveComplexMultiplier.Conjugate() * Vj;
                    Complex Iji = -(PositiveSequenceSeriesAdmittance / EffectiveComplexMultiplier) * Vi + PositiveSequenceSeriesAdmittance * Vj;

                    FromNodeCurrent.PositiveSequence.Estimate.PerUnitComplexPhasor = Iij;
                    ToNodeCurrent.PositiveSequence.Estimate.PerUnitComplexPhasor = Iji;
                }
                else
                {
                    FromNodeCurrent.PositiveSequence.Estimate.Magnitude = 0;
                    FromNodeCurrent.PositiveSequence.Estimate.AngleInDegrees = 0;
                    ToNodeCurrent.PositiveSequence.Estimate.Magnitude = 0;
                    ToNodeCurrent.PositiveSequence.Estimate.AngleInDegrees = 0;
                }
            }
        }

        /// <summary>
        /// Computes an estimated value of three phase current flow through the transformer based on estimated values of the high side and low side node voltages.
        /// </summary>
        public void ComputeThreePhaseEstimatedCurrentFlow()
        {
            if (m_parentSubstation.ParentDivision.ParentCompany.ParentModel.CurrentFlowPostProcessingSetting == CurrentFlowPostProcessingSetting.ProcessOnlyMeasuredBranches)
            {
                if ((FromNodeCurrent.IncludeInEstimator || ToNodeCurrent.IncludeInEstimator) && FromNode.IsObserved && ToNode.IsObserved)
                {
                    DenseMatrix Vi = DenseMatrix.OfArray(new Complex[3, 1]);
                    DenseMatrix Vj = DenseMatrix.OfArray(new Complex[3, 1]);

                    Vi[0, 0] = FromNode.Voltage.PhaseA.Estimate.PerUnitComplexPhasor;
                    Vi[1, 0] = FromNode.Voltage.PhaseB.Estimate.PerUnitComplexPhasor;
                    Vi[2, 0] = FromNode.Voltage.PhaseC.Estimate.PerUnitComplexPhasor;

                    Vj[0, 0] = ToNode.Voltage.PhaseA.Estimate.PerUnitComplexPhasor;
                    Vj[1, 0] = ToNode.Voltage.PhaseB.Estimate.PerUnitComplexPhasor;
                    Vj[2, 0] = ToNode.Voltage.PhaseC.Estimate.PerUnitComplexPhasor;

                    DenseMatrix Iij = (ThreePhaseSeriesAdmittance * (1 / (EffectiveComplexMultiplier.Magnitude * EffectiveComplexMultiplier.Magnitude))) * Vi - ThreePhaseSeriesAdmittance * (1 / EffectiveComplexMultiplier.Conjugate()) * Vj;
                    DenseMatrix Iji = -(ThreePhaseSeriesAdmittance * (1 / EffectiveComplexMultiplier)) * Vi + ThreePhaseSeriesAdmittance * Vj;

                    FromNodeCurrent.PhaseA.Estimate.PerUnitComplexPhasor = Iij[0, 0];
                    FromNodeCurrent.PhaseB.Estimate.PerUnitComplexPhasor = Iij[1, 0];
                    FromNodeCurrent.PhaseC.Estimate.PerUnitComplexPhasor = Iij[2, 0];

                    ToNodeCurrent.PhaseA.Estimate.PerUnitComplexPhasor = Iji[0, 0];
                    ToNodeCurrent.PhaseB.Estimate.PerUnitComplexPhasor = Iji[1, 0];
                    ToNodeCurrent.PhaseC.Estimate.PerUnitComplexPhasor = Iji[2, 0];
                }
                else
                {
                    FromNodeCurrent.PhaseA.Estimate.Magnitude = 0;
                    FromNodeCurrent.PhaseA.Estimate.AngleInDegrees = 0;
                    ToNodeCurrent.PhaseA.Estimate.Magnitude = 0;
                    ToNodeCurrent.PhaseA.Estimate.AngleInDegrees = 0;

                    FromNodeCurrent.PhaseB.Estimate.Magnitude = 0;
                    FromNodeCurrent.PhaseB.Estimate.AngleInDegrees = 0;
                    ToNodeCurrent.PhaseB.Estimate.Magnitude = 0;
                    ToNodeCurrent.PhaseB.Estimate.AngleInDegrees = 0;

                    FromNodeCurrent.PhaseC.Estimate.Magnitude = 0;
                    FromNodeCurrent.PhaseC.Estimate.AngleInDegrees = 0;
                    ToNodeCurrent.PhaseC.Estimate.Magnitude = 0;
                    ToNodeCurrent.PhaseC.Estimate.AngleInDegrees = 0;
                }
            }
            else if (m_parentSubstation.ParentDivision.ParentCompany.ParentModel.CurrentFlowPostProcessingSetting == CurrentFlowPostProcessingSetting.ProcessBranchesByNodeObservability)
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

                    DenseMatrix Iij = (ThreePhaseSeriesAdmittance * (1 / (EffectiveComplexMultiplier.Magnitude * EffectiveComplexMultiplier.Magnitude))) * Vi - ThreePhaseSeriesAdmittance * (1 / EffectiveComplexMultiplier.Conjugate()) * Vj;
                    DenseMatrix Iji = -(ThreePhaseSeriesAdmittance * (1 / EffectiveComplexMultiplier)) * Vi + ThreePhaseSeriesAdmittance * Vj;

                    FromNodeCurrent.PhaseA.Estimate.PerUnitComplexPhasor = Iij[0, 0];
                    FromNodeCurrent.PhaseB.Estimate.PerUnitComplexPhasor = Iij[1, 0];
                    FromNodeCurrent.PhaseC.Estimate.PerUnitComplexPhasor = Iij[2, 0];

                    ToNodeCurrent.PhaseA.Estimate.PerUnitComplexPhasor = Iji[0, 0];
                    ToNodeCurrent.PhaseB.Estimate.PerUnitComplexPhasor = Iji[1, 0];
                    ToNodeCurrent.PhaseC.Estimate.PerUnitComplexPhasor = Iji[2, 0];
                }
                else
                {
                    FromNodeCurrent.PhaseA.Estimate.Magnitude = 0;
                    FromNodeCurrent.PhaseA.Estimate.AngleInDegrees = 0;
                    ToNodeCurrent.PhaseA.Estimate.Magnitude = 0;
                    ToNodeCurrent.PhaseA.Estimate.AngleInDegrees = 0;

                    FromNodeCurrent.PhaseB.Estimate.Magnitude = 0;
                    FromNodeCurrent.PhaseB.Estimate.AngleInDegrees = 0;
                    ToNodeCurrent.PhaseB.Estimate.Magnitude = 0;
                    ToNodeCurrent.PhaseB.Estimate.AngleInDegrees = 0;

                    FromNodeCurrent.PhaseC.Estimate.Magnitude = 0;
                    FromNodeCurrent.PhaseC.Estimate.AngleInDegrees = 0;
                    ToNodeCurrent.PhaseC.Estimate.Magnitude = 0;
                    ToNodeCurrent.PhaseC.Estimate.AngleInDegrees = 0;
                }
            }
        }

        /// <summary>
        /// A string representation of the instance of the <see cref="SeriesBranchBase"/> class.
        /// </summary>
        /// <returns>A string representation of the instance of the <see cref="SeriesBranchBase"/> class.</returns>
        public override string ToString()
        {
            return "-TX- ID: " + InternalID.ToString() + " Number: " + Number.ToString();
        }

        /// <summary>
        /// A verbose string representation of the instance of the <see cref="SeriesBranchBase"/> class.
        /// </summary>
        /// <returns>A verbose string representation of the instance of the <see cref="SeriesBranchBase"/> class.</returns>
        public new string ToVerboseString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine();
            stringBuilder.AppendFormat("----- Transformer --------------------------------------------------------------");
            stringBuilder.AppendLine();
            stringBuilder.AppendFormat("                 Internal ID: " + InternalID.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("                      Number: " + Number.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("                        Name: " + Name + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("                 Description: " + Description + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("                   From Node: " + FromNode.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("                     To Node: " + ToNode.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("           Parent Substation: " + m_parentSubstation.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("           From Node Current: " + m_fromNodeCurrent.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("             To Node Current: " + m_toNodeCurrent.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("   From Node Connection Type: " + m_fromNodeConnectionType.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("     To Node Connection Type: " + m_toNodeConnectionType.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("      Tap Position Input Key: " + m_tapPositionInputKey + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("    Tap Position Measurement: " + m_tapPositionMeasurement + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("     Tap Position Output Key: " + m_tapPositionOutputKey + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("        Assumed Tap Position: " + AssumedTapPosition.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("          Fixed Tap Position: " + m_fixedTapPosition.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("             ULTC Is Enabled: " + m_ultcIsEnabled.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("           Phase Shift (deg): " + ComputePhaseShift().ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("Off Nominal Tap Ratio (p.u.): " + ComputeOffNominalTapRatio().ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("Effective Complex Multiplier: " + EffectiveComplexMultiplier.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat(m_tapConfiguration.ToVerboseString());
            stringBuilder.AppendFormat(RawImpedanceParameters.ToVerboseString());
            stringBuilder.AppendLine();
            return stringBuilder.ToString();

        }

        public void Keyify(string rootKey)
        {
            TapPositionInputKey = $"{rootKey}.Tap.Meas";
            TapPositionOutputKey = $"{rootKey}.Tap.Est";
        }

        public void Unkeyify()
        {
            TapPositionInputKey = DEFAULT_MEASUREMENT_KEY;
            TapPositionOutputKey = DEFAULT_MEASUREMENT_KEY;
        }

        #endregion

        #region [ Private Methods ]

        private Complex ComputeComplexMultiplierA()
        {
            double phaseShift = ComputePhaseShift();
            double offNominalTapRatio = ComputeOffNominalTapRatio();

            return offNominalTapRatio * (new Complex(Math.Cos(phaseShift), Math.Sin(phaseShift)));
        }

        private double ComputePhaseShift()
        {
            // From side is considered primary side. ANSI Standards dictate primary side voltage always leads secondary for delta-wye
            if (m_fromNodeConnectionType == TransformerConnectionType.Delta && m_toNodeConnectionType == TransformerConnectionType.Delta)
            {
                return 0;
            }
            else if (m_fromNodeConnectionType == TransformerConnectionType.Wye && m_toNodeConnectionType == TransformerConnectionType.Wye)
            {
                return 0;
            }
            else if (m_fromNodeConnectionType == TransformerConnectionType.Delta && m_toNodeConnectionType == TransformerConnectionType.Wye)
            {
                return 30;
            }
            else if (m_fromNodeConnectionType == TransformerConnectionType.Wye && m_toNodeConnectionType == TransformerConnectionType.Delta)
            {
                return 30;
            }
            else
            {
                return 0;
            }
        }

        private double ComputeOffNominalTapRatio()
        {
            if (m_ultcIsEnabled)
            {
                m_tapConfiguration.CurrentPosition = m_tapPositionMeasurement;
            }
            else
            {
                m_tapConfiguration.CurrentPosition = m_fixedTapPosition;
            }
            return m_tapConfiguration.CurrentMultiplier;
        }

        private Impedance ComputeOffNominalImpedance()
        {
            Impedance offNominalImpedance = new Impedance() { ParentElement = this };
            
            // Scale the series impedance (R + jX) * a
            offNominalImpedance.ThreePhaseSeriesImpedance = NominalImpedance.ThreePhaseSeriesImpedance * EffectiveComplexMultiplier.Magnitude;

            // Compute the equivalent shunt susceptance on the from side of the transformer
            offNominalImpedance.ThreePhaseFromSideShuntSusceptance = NominalImpedance.ThreePhaseSeriesAdmittance * ((1 - EffectiveComplexMultiplier.Magnitude) / (EffectiveComplexMultiplier.Magnitude * EffectiveComplexMultiplier.Magnitude));

            // Compute the equivalent shunt susceptance on the to side of the transformer
            offNominalImpedance.ThreePhaseToSideShuntSusceptance = NominalImpedance.ThreePhaseSeriesAdmittance * ((EffectiveComplexMultiplier.Magnitude - 1) / EffectiveComplexMultiplier.Magnitude);

            return offNominalImpedance;
        }

        #endregion
    }
}
