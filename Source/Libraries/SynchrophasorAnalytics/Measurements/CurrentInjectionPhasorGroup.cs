//******************************************************************************************************
//  ShuntCurrentPhasorGroup.cs
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
//  05/09/2014 - Kevin D. Jones
//       Generated original version of source code.
//  06/10/2014 - Kevin D. Jones
//       Updated XML inline documentation.
//
//******************************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using SynchrophasorAnalytics.Modeling;

namespace SynchrophasorAnalytics.Measurements
{
    /// <summary>
    /// Encapsulates a group of current phasors measuring an injection in a +, A, B, C grouping and relates them to the network model.
    /// </summary>
    /// <seealso cref="LinearStateEstimator.Measurements.PhasorGroup"/>
    /// <seealso cref="LinearStateEstimator.Measurements.CurrentFlowPhasorGroup"/>
    /// <seealso cref="LinearStateEstimator.Measurements.VoltagePhasorGroup"/>
    /// <seealso cref="LinearStateEstimator.Measurements.PhasorType"/>
    [Serializable()]
    public class CurrentInjectionPhasorGroup : PhasorGroup
    {
        #region [ Private Members ]

        private Node m_measuredConnectedNode;
        private int m_measuredConnectedNodeId;

        private ISingleTerminal m_measuredBranch;
        private int m_measuredBranchID;

        private CurrentInjectionDirectionConvention m_measurementDirectionConvention;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// The <see cref="LinearStateEstimator.Modeling.Node"/> from which the current measurement originates.
        /// </summary>
        [XmlIgnore()]
        public Node MeasuredConnectedNode
        {
            get
            {
                return m_measuredConnectedNode;
            }
            set
            {
                m_measuredConnectedNode = value;
                m_measuredConnectedNodeId = value.InternalID;
            }
        }

        /// <summary>
        /// The <see cref="LinearStateEstimator.Modeling.INetworkDescribable.InternalID"/> of the <see cref="LinearStateEstimator.Measurements.CurrentInjectionPhasorGroup.MeasuredConnectedNode"/>.
        /// </summary>
        [XmlAttribute("ConnectedNode")]
        public int MeasuredConnectedNodeId
        {
            get
            {
                return m_measuredConnectedNodeId;
            }
            set
            {
                m_measuredConnectedNodeId = value;
            }
        }

        /// <summary>
        /// The parent <see cref="LinearStateEstimator.Modeling.ISingleTerminal"/> branch which the <see cref="LinearStateEstimator.Measurements.CurrentInjectionPhasorGroup"/> is measuring.
        /// </summary>
        [XmlIgnore()]
        public ISingleTerminal MeasuredBranch
        {
            get
            {
                return m_measuredBranch;
            }
            set
            {
                if (value is ShuntCompensator)
                {
                    m_measuredBranch = value;
                    m_measuredBranchID = (value as ShuntCompensator).InternalID;
                }
            }
        }

        /// <summary>
        /// The internal id of the parent <see cref="LinearStateEstimator.Modeling.ISingleTerminal"/> branch which the <see cref="LinearStateEstimator.Measurements.CurrentInjectionPhasorGroup"/> is measuring.
        /// </summary>
        [XmlIgnore()]
        public int MeasuredBranchID
        {
            get
            {
                return m_measuredBranchID;
            }
            set
            {
                m_measuredBranchID = value;
            }
        }

        /// <summary>
        /// Specifies the convention to follow for the direction of the current phasor measuring an injection. Either <see cref="LinearStateEstimator.Measurements.CurrentInjectionDirectionConvention.IntoTheShunt"/> or <see cref="LinearStateEstimator.Measurements.CurrentInjectionDirectionConvention.OutOfTheShunt"/>.
        /// </summary>
        [XmlAttribute("DirectionConvention")]
        public CurrentInjectionDirectionConvention MeasurementDirectionConvention
        {
            get
            {
                return m_measurementDirectionConvention;
            }
            set
            {
                m_measurementDirectionConvention = value;
            }
        }

        [XmlIgnore()]
        public bool InPruningMode
        {
            get
            {
                return m_measuredConnectedNode.ParentSubstation.ParentDivision.ParentCompany.ParentModel.InPruningMode;
            }
        }

        [XmlIgnore()]
        public bool IncludeInEstimator
        {
            get
            {
                if (InPruningMode)
                {
                    return ExpectsMeasurements;
                }
                return base.IncludeInEstimator;
            }
        }

        [XmlIgnore()]
        public bool IncludeInPositiveSequenceEstimator
        {
            get
            {
                if (InPruningMode)
                {
                    return ExpectsPositiveSequenceMeasurements;
                }
                return base.IncludeInPositiveSequenceEstimator;
            }
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// A blank constructor with default values.
        /// </summary>
        public CurrentInjectionPhasorGroup()
            : this(0, 0, "Undefined Name", "Undefined Description", CurrentInjectionDirectionConvention.IntoTheShunt)
        {
        }

        /// <summary>
        /// A constructor which specifies only the information required by the <see cref="LinearStateEstimator.Modeling.INetworkDescribable"/> interface. The <see cref="Phasor"/>, <see cref="StatusWord"/>, and <see cref="Node"/> objects are instantiated with default initializers to prevent null references.
        /// </summary>
        /// <param name="internalID">The unique integer identifier for each instance of a <see cref="LinearStateEstimator.Measurements.CurrentInjectionPhasorGroup"/> object.</param>
        /// <param name="number">A descriptive number for the <see cref="LinearStateEstimator.Measurements.CurrentInjectionPhasorGroup"/> object.</param>
        /// <param name="name">A descriptive name for the <see cref="LinearStateEstimator.Measurements.CurrentInjectionPhasorGroup"/> object.</param>
        /// <param name="description">A description of the <see cref="LinearStateEstimator.Measurements.CurrentInjectionPhasorGroup"/> object.</param>
        /// <param name="directionConvention">Specifies whether the <see cref="LinearStateEstimator.Measurements.CurrentInjectionPhasorGroup"/> is measured <see cref="LinearStateEstimator.Measurements.CurrentInjectionDirectionConvention.IntoTheShunt"/> or <see cref="LinearStateEstimator.Measurements.CurrentInjectionDirectionConvention.OutOfTheShunt"/>.</param>
        public CurrentInjectionPhasorGroup(int internalID, int number, string name, string description, CurrentInjectionDirectionConvention directionConvention)
            : this(internalID, number, name, description, new Phasor(), new Phasor(), new Phasor(), new Phasor(), directionConvention)
        {
        }

        /// <summary>
        /// A constructor which specifies the information required by the <see cref="LinearStateEstimator.Modeling.INetworkDescribable"/> interface and the <see cref="Phasor"/> objects for +, A, B, and C. 
        /// </summary>
        /// <param name="internalID">The unique integer identifier for each instance of a <see cref="LinearStateEstimator.Measurements.CurrentInjectionPhasorGroup"/> object.</param>
        /// <param name="number">A descriptive number for the <see cref="LinearStateEstimator.Measurements.CurrentInjectionPhasorGroup"/> object.</param>
        /// <param name="name">A descriptive name for the <see cref="LinearStateEstimator.Measurements.CurrentInjectionPhasorGroup"/> object.</param>
        /// <param name="description">A description of the <see cref="LinearStateEstimator.Measurements.CurrentInjectionPhasorGroup"/> object.</param>
        /// <param name="positiveSequence">The <see cref="LinearStateEstimator.Measurements.Phasor"/> representing positive sequence.</param>
        /// <param name="phaseA">The <see cref="LinearStateEstimator.Measurements.Phasor"/> representing phase A.</param>
        /// <param name="phaseB">The <see cref="LinearStateEstimator.Measurements.Phasor"/> representing phase B.</param>
        /// <param name="phaseC">The <see cref="LinearStateEstimator.Measurements.Phasor"/> representing phase C.</param>
        /// <param name="directionConvention">Specifies whether the <see cref="LinearStateEstimator.Measurements.CurrentInjectionPhasorGroup"/> is measured <see cref="LinearStateEstimator.Measurements.CurrentInjectionDirectionConvention.IntoTheShunt"/> or <see cref="LinearStateEstimator.Measurements.CurrentInjectionDirectionConvention.OutOfTheShunt"/>.</param>
        public CurrentInjectionPhasorGroup(int internalID, int number, string name, string description, Phasor positiveSequence, Phasor phaseA, Phasor phaseB, Phasor phaseC, CurrentInjectionDirectionConvention directionConvention)
            : this(internalID, number, name, description, positiveSequence, phaseA, phaseB, phaseC, new StatusWord(), directionConvention)
        {
        }

        /// <summary>
        /// A constructor which specifies the information required by the <see cref="LinearStateEstimator.Modeling.INetworkDescribable"/> interface and the <see cref="LinearStateEstimator.Measurements.Phasor"/> objects for +, A, B, and C and the <see cref="LinearStateEstimator.Modeling.INetworkDescribable.InternalID"/> of the <see cref="LinearStateEstimator.Measurements.CurrentInjectionPhasorGroup.MeasuredConnectedNode"/>.
        /// </summary>
        /// <param name="internalID">The unique integer identifier for each instance of a <see cref="LinearStateEstimator.Measurements.CurrentInjectionPhasorGroup"/> object.</param>
        /// <param name="number">A descriptive number for the <see cref="LinearStateEstimator.Measurements.CurrentInjectionPhasorGroup"/> object.</param>
        /// <param name="name">A descriptive name for the <see cref="LinearStateEstimator.Measurements.CurrentInjectionPhasorGroup"/> object.</param>
        /// <param name="description">A description of the <see cref="LinearStateEstimator.Measurements.CurrentInjectionPhasorGroup"/> object.</param>
        /// <param name="positiveSequence">The <see cref="LinearStateEstimator.Measurements.Phasor"/> representing positive sequence.</param>
        /// <param name="phaseA">The <see cref="LinearStateEstimator.Measurements.Phasor"/> representing phase A.</param>
        /// <param name="phaseB">The <see cref="LinearStateEstimator.Measurements.Phasor"/> representing phase B.</param>
        /// <param name="phaseC">The <see cref="LinearStateEstimator.Measurements.Phasor"/> representing phase C.</param>
        /// <param name="measuredConnectedNodeId">The <see cref="LinearStateEstimator.Modeling.INetworkDescribable.InternalID"/> of the <see cref="LinearStateEstimator.Measurements.CurrentInjectionPhasorGroup.MeasuredConnectedNode"/>.</param>
        /// <param name="directionConvention">Specifies whether the <see cref="LinearStateEstimator.Measurements.CurrentInjectionPhasorGroup"/> is measured <see cref="LinearStateEstimator.Measurements.CurrentInjectionDirectionConvention.IntoTheShunt"/> or <see cref="LinearStateEstimator.Measurements.CurrentInjectionDirectionConvention.OutOfTheShunt"/>.</param>
        public CurrentInjectionPhasorGroup(int internalID, int number, string name, string description, Phasor positiveSequence, Phasor phaseA, Phasor phaseB, Phasor phaseC, int measuredConnectedNodeId, CurrentInjectionDirectionConvention directionConvention)
            : this(internalID, number, name, description, positiveSequence, phaseA, phaseB, phaseC, new StatusWord(), measuredConnectedNodeId, directionConvention)
        {
        }

        /// <summary>
        /// A constructor which specifies the information required by the <see cref="LinearStateEstimator.Modeling.INetworkDescribable"/> interface and the <see cref="LinearStateEstimator.Measurements.Phasor"/> objects for +, A, B, and C, and the <see cref="LinearStateEstimator.Measurements.StatusWord"/> 
        /// </summary>
        /// <param name="internalID">The unique integer identifier for each instance of a <see cref="LinearStateEstimator.Measurements.CurrentInjectionPhasorGroup"/> object.</param>
        /// <param name="number">A descriptive number for the <see cref="LinearStateEstimator.Measurements.CurrentInjectionPhasorGroup"/> object.</param>
        /// <param name="name">A descriptive name for the <see cref="LinearStateEstimator.Measurements.CurrentInjectionPhasorGroup"/> object.</param>
        /// <param name="description">A description of the <see cref="LinearStateEstimator.Measurements.CurrentInjectionPhasorGroup"/> object.</param>
        /// <param name="positiveSequence">The <see cref="LinearStateEstimator.Measurements.Phasor"/> representing positive sequence.</param>
        /// <param name="phaseA">The <see cref="LinearStateEstimator.Measurements.Phasor"/> representing phase A.</param>
        /// <param name="phaseB">The <see cref="LinearStateEstimator.Measurements.Phasor"/> representing phase B.</param>
        /// <param name="phaseC">The <see cref="LinearStateEstimator.Measurements.Phasor"/> representing phase C.</param>
        /// <param name="statusWord">The <see cref="LinearStateEstimator.Measurements.StatusWord"/> from the source device for the <see cref="LinearStateEstimator.Measurements.Phasor"/> objects in this <see cref="LinearStateEstimator.Measurements.PhasorGroup"/>.</param>
        /// <param name="directionConvention">Specifies whether the <see cref="LinearStateEstimator.Measurements.CurrentInjectionPhasorGroup"/> is measured <see cref="LinearStateEstimator.Measurements.CurrentInjectionDirectionConvention.IntoTheShunt"/> or <see cref="LinearStateEstimator.Measurements.CurrentInjectionDirectionConvention.OutOfTheShunt"/>.</param>
        public CurrentInjectionPhasorGroup(int internalID, int number, string name, string description, Phasor positiveSequence, Phasor phaseA, Phasor phaseB, Phasor phaseC, StatusWord statusWord, CurrentInjectionDirectionConvention directionConvention)
            : this(internalID, number, name, description, positiveSequence, phaseA, phaseB, phaseC, statusWord, 0, directionConvention)
        {
        }

        /// <summary>
        /// The designated constructor for the <see cref="LinearStateEstimator.Measurements.CurrentInjectionPhasorGroup"/> class which specifies the information required by the <see cref="LinearStateEstimator.Modeling.INetworkDescribable"/> interface and the <see cref="Phasor"/> objects for +, A, B, and C, the <see cref="LinearStateEstimator.Measurements.StatusWord"/> and the <see cref="LinearStateEstimator.Modeling.INetworkDescribable.InternalID"/> of the <see cref="LinearStateEstimator.Measurements.CurrentInjectionPhasorGroup.MeasuredConnectedNode"/>.
        /// </summary>
        /// <param name="internalID">The unique integer identifier for each instance of a <see cref="LinearStateEstimator.Measurements.CurrentInjectionPhasorGroup"/> object.</param>
        /// <param name="number">A descriptive number for the <see cref="LinearStateEstimator.Measurements.CurrentInjectionPhasorGroup"/> object.</param>
        /// <param name="name">A descriptive name for the <see cref="LinearStateEstimator.Measurements.CurrentInjectionPhasorGroup"/> object.</param>
        /// <param name="description">A description of the <see cref="LinearStateEstimator.Measurements.CurrentInjectionPhasorGroup"/> object.</param>
        /// <param name="positiveSequence">The <see cref="Phasor"/> representing positive sequence.</param>
        /// <param name="phaseA">The <see cref="LinearStateEstimator.Measurements.Phasor"/> representing phase A.</param>
        /// <param name="phaseB">The <see cref="LinearStateEstimator.Measurements.Phasor"/> representing phase B.</param>
        /// <param name="phaseC">The <see cref="LinearStateEstimator.Measurements.Phasor"/> representing phase C.</param>
        /// <param name="statusWord">The <see cref="LinearStateEstimator.Measurements.StatusWord"/> from the source device for the <see cref="LinearStateEstimator.Measurements.Phasor"/> objects in this <see cref="LinearStateEstimator.Measurements.PhasorGroup"/>.</param>
        /// <param name="measuredConnectedNodeId">The <see cref="LinearStateEstimator.Modeling.INetworkDescribable.InternalID"/> of the <see cref="LinearStateEstimator.Measurements.CurrentInjectionPhasorGroup.MeasuredConnectedNode"/>.</param>
        /// <param name="directionConvention">Specifies whether the <see cref="LinearStateEstimator.Measurements.CurrentInjectionPhasorGroup"/> is measured <see cref="LinearStateEstimator.Measurements.CurrentInjectionDirectionConvention.IntoTheShunt"/> or <see cref="LinearStateEstimator.Measurements.CurrentInjectionDirectionConvention.OutOfTheShunt"/>.</param>
        public CurrentInjectionPhasorGroup(int internalID, int number, string name, string description, Phasor positiveSequence, Phasor phaseA, Phasor phaseB, Phasor phaseC, StatusWord statusWord, int measuredConnectedNodeId, CurrentInjectionDirectionConvention directionConvention)
            : base(internalID, number, name, description, positiveSequence, phaseA, phaseB, phaseC, statusWord)
        {
            m_measuredConnectedNodeId = measuredConnectedNodeId;
            m_measurementDirectionConvention = directionConvention;

            PositiveSequence.Measurement.Type = PhasorType.CurrentPhasor;
            PositiveSequence.Estimate.Type = PhasorType.CurrentPhasor;

            NegativeSequence.Measurement.Type = PhasorType.CurrentPhasor;
            NegativeSequence.Estimate.Type = PhasorType.CurrentPhasor;

            ZeroSequence.Measurement.Type = PhasorType.CurrentPhasor;
            ZeroSequence.Estimate.Type = PhasorType.CurrentPhasor;

            PhaseA.Measurement.Type = PhasorType.CurrentPhasor;
            PhaseA.Estimate.Type = PhasorType.CurrentPhasor;

            PhaseB.Measurement.Type = PhasorType.CurrentPhasor;
            PhaseB.Estimate.Type = PhasorType.CurrentPhasor;

            PhaseC.Measurement.Type = PhasorType.CurrentPhasor;
            PhaseC.Estimate.Type = PhasorType.CurrentPhasor;
        }

        #endregion

        #region [ Public Methods ]

        /// <summary>
        /// A string representation of the instance of the <see cref="LinearStateEstimator.Measurements.CurrentInjectionPhasorGroup"/> class.
        /// </summary>
        /// <returns>A string representation of the instance of the <see cref="LinearStateEstimator.Measurements.CurrentInjectionPhasorGroup"/> class.</returns>
        public override string ToString()
        {
            return "CurrentInjection," + base.ToString() + "," + m_measurementDirectionConvention.ToString();
        }

        /// <summary>
        /// A verbose string representation of the instance of the <see cref="LinearStateEstimator.Measurements.CurrentInjectionPhasorGroup"/> class.
        /// </summary>
        /// <returns>A verbose string representation of the instance of the <see cref="LinearStateEstimator.Measurements.CurrentInjectionPhasorGroup"/> class.</returns>
        public new string ToVerboseString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine();
            stringBuilder.AppendFormat("----- Shunt Current Phasor Group -----------------------------------------------");
            stringBuilder.AppendLine();
            stringBuilder.AppendFormat($"      InternalID: { InternalID.ToString()}{Environment.NewLine}");
            stringBuilder.AppendFormat($"          Number: {Number.ToString()}{Environment.NewLine}");
            stringBuilder.AppendFormat($"         Acronym: {Acronym}{Environment.NewLine}");
            stringBuilder.AppendFormat($"            Name: {Name}{Environment.NewLine}");
            stringBuilder.AppendFormat($"     Description: {Description}{Environment.NewLine}");
            stringBuilder.AppendFormat($"         Enabled: {IsEnabled}{Environment.NewLine}");
            stringBuilder.AppendFormat($"       UseStatus: {UseStatusFlagForRemovingMeasurements.ToString()}{Environment.NewLine}");
            stringBuilder.AppendFormat($"          Status: {Status.ToString()}{Environment.NewLine}");
            stringBuilder.AppendFormat($"     PosSeqMKeys: {PositiveSequence.Measurement.MagnitudeKey} | {PositiveSequence.Measurement.AngleKey}{Environment.NewLine}");
            stringBuilder.AppendFormat($"     PosSeqEKeys: {PositiveSequence.Estimate.MagnitudeKey} | {PositiveSequence.Estimate.AngleKey}{Environment.NewLine}");
            stringBuilder.AppendFormat($"     PhaseAMKeys: {PhaseA.Measurement.MagnitudeKey} | {PhaseA.Measurement.AngleKey}{Environment.NewLine}");
            stringBuilder.AppendFormat($"     PhaseAEKeys: {PhaseA.Estimate.MagnitudeKey} | {PhaseA.Estimate.AngleKey}{Environment.NewLine}");
            stringBuilder.AppendFormat($"     PhaseBMKeys: {PhaseB.Measurement.MagnitudeKey} | {PhaseB.Measurement.AngleKey}{Environment.NewLine}");
            stringBuilder.AppendFormat($"     PhaseBEKeys: {PhaseB.Estimate.MagnitudeKey} | {PhaseB.Estimate.AngleKey}{Environment.NewLine}");
            stringBuilder.AppendFormat($"     PhaseCMKeys: {PhaseC.Measurement.MagnitudeKey} | {PhaseC.Measurement.AngleKey}{Environment.NewLine}");
            stringBuilder.AppendFormat($"     PhaseCEKeys: {PhaseC.Estimate.MagnitudeKey} | {PhaseC.Estimate.AngleKey}{Environment.NewLine}");
            stringBuilder.AppendFormat($"      PosSeqMeas: {PositiveSequence.Measurement.Magnitude} | {PositiveSequence.Measurement.AngleInDegrees}{Environment.NewLine}");
            stringBuilder.AppendFormat($"       PosSeqEst: {PositiveSequence.Estimate.Magnitude} | {PositiveSequence.Estimate.AngleInDegrees}{Environment.NewLine}");
            stringBuilder.AppendFormat($"      PhaseAMeas: {PhaseA.Measurement.Magnitude} | {PhaseA.Measurement.AngleInDegrees}{Environment.NewLine}");
            stringBuilder.AppendFormat($"       PhaseAEst: {PhaseA.Estimate.Magnitude} | {PhaseA.Estimate.AngleInDegrees}{Environment.NewLine}");
            stringBuilder.AppendFormat($"      PhaseBMeas: {PhaseB.Measurement.Magnitude} | {PhaseB.Measurement.AngleInDegrees}{Environment.NewLine}");
            stringBuilder.AppendFormat($"       PhaseBEst: {PhaseB.Estimate.Magnitude} | {PhaseB.Estimate.AngleInDegrees}{Environment.NewLine}");
            stringBuilder.AppendFormat($"      PhaseCMeas: {PhaseC.Measurement.Magnitude} | {PhaseC.Measurement.AngleInDegrees}{Environment.NewLine}");
            stringBuilder.AppendFormat($"       PhaseCEst: {PhaseC.Estimate.Magnitude} | {PhaseC.Estimate.AngleInDegrees}{Environment.NewLine}");
            stringBuilder.AppendFormat($"   ConnectedNode: {MeasuredConnectedNode.InternalID.ToString()} | {MeasuredConnectedNode.Name} | {MeasuredConnectedNode.BaseKV.Description}{Environment.NewLine}");
            stringBuilder.AppendFormat($"      Convention: {MeasurementDirectionConvention.ToString()} | {Environment.NewLine}");
            stringBuilder.AppendFormat($"+SeqMeasReported: {PositiveSequence.Measurement.MeasurementWasReported}{Environment.NewLine}");
            stringBuilder.AppendFormat($"APhsMeasReported: {PhaseA.Measurement.MeasurementWasReported}{Environment.NewLine}");
            stringBuilder.AppendFormat($"BPhsMeasReported: {PhaseB.Measurement.MeasurementWasReported}{Environment.NewLine}");
            stringBuilder.AppendFormat($"CPhsMeasReported: {PhaseC.Measurement.MeasurementWasReported}{Environment.NewLine}");
            stringBuilder.AppendFormat($"UseInEstimator +: {IncludeInPositiveSequenceEstimator}{Environment.NewLine}");
            stringBuilder.AppendFormat($"UseInEstimator 3: {IncludeInEstimator.ToString()}{Environment.NewLine}");
            stringBuilder.AppendLine();
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Performs a deep copy of the <see cref="LinearStateEstimator.Measurements.CurrentInjectionPhasorGroup"/> object.
        /// </summary>
        /// <returns>A deep copy of the target <see cref="LinearStateEstimator.Measurements.CurrentInjectionPhasorGroup"/>.</returns>
        public CurrentInjectionPhasorGroup Copy()
        {
            CurrentInjectionPhasorGroup copy = (CurrentInjectionPhasorGroup)this.MemberwiseClone();
            copy.Status = this.Status.Copy();
            copy.PositiveSequence = PositiveSequence.Copy();
            copy.PhaseA = PhaseA.Copy();
            copy.PhaseB = PhaseB.Copy();
            copy.PhaseC = PhaseC.Copy();
            return copy;
        }

        public void Keyify(string rootKey)
        {
            base.Keyify($"{rootKey}.IInj");
        }

        public void Unkeyify()
        {
            base.Unkeyify();
        }

        #endregion
    }
}
