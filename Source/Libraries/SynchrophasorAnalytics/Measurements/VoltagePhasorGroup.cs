//******************************************************************************************************
//  VoltagePhasorGroup.cs
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
//
//******************************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using SynchrophasorAnalytics.Modeling;

namespace SynchrophasorAnalytics.Measurements
{
    /// <summary>
    /// Encapsulates a group of voltage phasors measuring a flow in a +, A, B, C grouping and relates them to the network model.
    /// </summary>
    /// <seealso cref="LinearStateEstimator.Measurements.PhasorGroup"/>
    /// <seealso cref="LinearStateEstimator.Measurements.CurrentFlowPhasorGroup"/>
    /// <seealso cref="LinearStateEstimator.Measurements.CurrentInjectionPhasorGroup"/>
    /// <seealso cref="LinearStateEstimator.Measurements.PhasorType"/>
    [Serializable()]
    public class VoltagePhasorGroup : PhasorGroup
    {
        #region [ Private Members ]

        private Node m_measuredNode;
        private int m_measuredNodeID;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// The <see cref="LinearStateEstimator.Modeling.Node"/> measured by
        /// this <see cref="VoltagePhasorGroup"/>. The setter also sets the
        /// <see cref="VoltagePhasorGroup.MeasuredNodeID"/> based on the
        /// <see cref="PhasorGroup.InternalID"/>.
        /// </summary>
        [XmlIgnore()]
        public Node MeasuredNode
        {
            get 
            { 
                return m_measuredNode; 
            }
            set 
            { 
                m_measuredNode = value;
                m_measuredNodeID = value.InternalID;
                //SetBaseKvOfChildrenPhasors();
            }
        }
        
        /// <summary>
        /// The unique integer identifier of the <see cref="LinearStateEstimator.Modeling.Node"/>
        /// measured by this <see cref="VoltagePhasorGroup"/>. For re-linking references after deserializing.
        /// </summary>
        [XmlIgnore()]
        public int MeasuredNodeID
        {
            get
            {
                return m_measuredNodeID;
            }
            set
            {
                m_measuredNodeID = value;
            }
        }

        [XmlIgnore()]
        public bool InPruningMode
        {
            get
            {
                return m_measuredNode.ParentSubstation.ParentDivision.ParentCompany.ParentModel.InPruningMode;
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
        public VoltagePhasorGroup()
            :this(0, 0, "Undefined Name", "Undefined Description")
        {
        }

        /// <summary>
        /// A constructor which specifies only the information required by the <see cref="INetworkDescribable"/> interface. The <see cref="Phasor"/>, <see cref="StatusWord"/>, and <see cref="Node"/> objects are instantiated with default initializers to prevent null references.
        /// </summary>
        /// <param name="internalID">The unique integer identifier for each instance of a <see cref="VoltagePhasorGroup"/> object.</param>
        /// <param name="number">A descriptive number for the <see cref="VoltagePhasorGroup"/> object.</param>
        /// <param name="name">A descriptive name for the <see cref="VoltagePhasorGroup"/> object.</param>
        /// <param name="description">A description of the <see cref="VoltagePhasorGroup"/> object.</param>
        public VoltagePhasorGroup(int internalID, int number, string name, string description)
            :this(internalID, number, name, description, new Phasor(), new Phasor(), new Phasor(), new Phasor())
        {
        }

        /// <summary>
        /// A constructor which specifies the information required by the <see cref="LinearStateEstimator.Modeling.INetworkDescribable"/> interface and the <see cref="LinearStateEstimator.Measurements.Phasor"/> objects for +, A, B, and C. The <see cref="LinearStateEstimator.Measurements.StatusWord"/> and <see cref="LinearStateEstimator.Modeling.Node"/> objects are instantiated with the default initializer to prevent null references.
        /// </summary>
        /// <param name="internalID">The unique integer identifier for each instance of a <see cref="LinearStateEstimator.Measurements.VoltagePhasorGroup"/> object.</param>
        /// <param name="number">A descriptive number for the <see cref="LinearStateEstimator.Measurements.VoltagePhasorGroup"/> object.</param>
        /// <param name="name">A descriptive name for the <see cref="LinearStateEstimator.Measurements.VoltagePhasorGroup"/> object.</param>
        /// <param name="description">A description of the <see cref="LinearStateEstimator.Measurements.VoltagePhasorGroup"/> object.</param>
        /// <param name="positiveSequence">The <see cref="LinearStateEstimator.Measurements.Phasor"/> representing positive sequence.</param>
        /// <param name="phaseA">The <see cref="LinearStateEstimator.Measurements.Phasor"/> representing phase A.</param>
        /// <param name="phaseB">The <see cref="LinearStateEstimator.Measurements.Phasor"/> representing phase B.</param>
        /// <param name="phaseC">The <see cref="LinearStateEstimator.Measurements.Phasor"/> representing phase C.</param>
        public VoltagePhasorGroup(int internalID, int number, string name, string description, Phasor positiveSequence, Phasor phaseA, Phasor phaseB, Phasor phaseC)
            :this(internalID, number, name, description, positiveSequence, phaseA, phaseB, phaseC, new StatusWord())
        {
        }

        /// <summary>
        /// A constructor which specifies the information require dby the <see cref="LinearStateEstimator.Modeling.INetworkDescribable"/> interface, the <see cref="LinearStateEstimator.Measurements.Phasor"/> objects for +, A, B, and C, and the measured <see cref="LinearStateEstimator.Modeling.Node"/>. The measured <see cref="LinearStateEstimator.Measurements.StatusWord"/> is instantiated with the default initializer to prevent null references.
        /// </summary>
        /// <param name="internalID">The unique integer identifier for each instance of a <see cref="LinearStateEstimator.Measurements.VoltagePhasorGroup"/> object.</param>
        /// <param name="number">A descriptive number for the <see cref="LinearStateEstimator.Measurements.VoltagePhasorGroup"/> object.</param>
        /// <param name="name">A descriptive name for the <see cref="LinearStateEstimator.Measurements.VoltagePhasorGroup"/> object.</param>
        /// <param name="description">A description of the <see cref="LinearStateEstimator.Measurements.VoltagePhasorGroup"/> object.</param>
        /// <param name="positiveSequence">The <see cref="LinearStateEstimator.Measurements.Phasor"/> representing positive sequence.</param>
        /// <param name="phaseA">The <see cref="LinearStateEstimator.Measurements.Phasor"/> representing phase A.</param>
        /// <param name="phaseB">The <see cref="LinearStateEstimator.Measurements.Phasor"/> representing phase B.</param>
        /// <param name="phaseC">The <see cref="LinearStateEstimator.Measurements.Phasor"/> representing phase C.</param>
        /// <param name="measuredNode">The <see cref="LinearStateEstimator.Modeling.Node"/> measured by this <see cref="LinearStateEstimator.Measurements.VoltagePhasorGroup"/></param>
        public VoltagePhasorGroup(int internalID, int number, string name, string description, Phasor positiveSequence, Phasor phaseA, Phasor phaseB, Phasor phaseC, Node measuredNode)
            : this(internalID, number, name, description, positiveSequence, phaseA, phaseB, phaseC, new StatusWord(), measuredNode)
        {
        }

        /// <summary>
        /// A constructor which specifies the information require dby the <see cref="LinearStateEstimator.Modeling.INetworkDescribable"/> interface, the <see cref="LinearStateEstimator.Measurements.Phasor"/> objects for +, A, B, and C, and the measured <see cref="LinearStateEstimator.Measurements.StatusWord"/>. The measured <see cref="LinearStateEstimator.Modeling.Node"/> is instantiated with the default initializer to prevent null references.
        /// </summary>
        /// <param name="internalID">The unique integer identifier for each instance of a <see cref="LinearStateEstimator.Measurements.VoltagePhasorGroup"/> object.</param>
        /// <param name="number">A descriptive number for the <see cref="LinearStateEstimator.Measurements.VoltagePhasorGroup"/> object.</param>
        /// <param name="name">A descriptive name for the <see cref="LinearStateEstimator.Measurements.VoltagePhasorGroup"/> object.</param>
        /// <param name="description">A description of the <see cref="LinearStateEstimator.Measurements.VoltagePhasorGroup"/> object.</param>
        /// <param name="positiveSequence">The <see cref="LinearStateEstimator.Measurements.Phasor"/> representing positive sequence.</param>
        /// <param name="phaseA">The <see cref="LinearStateEstimator.Measurements.Phasor"/> representing phase A.</param>
        /// <param name="phaseB">The <see cref="LinearStateEstimator.Measurements.Phasor"/> representing phase B.</param>
        /// <param name="phaseC">The <see cref="LinearStateEstimator.Measurements.Phasor"/> representing phase C.</param>
        /// <param name="statusWord">The <see cref="LinearStateEstimator.Measurements.StatusWord"/> from the source device for the <see cref="LinearStateEstimator.Measurements.Phasor"/> objects in this <see cref="LinearStateEstimator.Measurements.PhasorGroup"/>.</param>
        public VoltagePhasorGroup(int internalID, int number, string name, string description, Phasor positiveSequence, Phasor phaseA, Phasor phaseB, Phasor phaseC, StatusWord statusWord)
            :this(internalID, number, name, description, positiveSequence, phaseA, phaseB, phaseC, statusWord, new Node())
        {
        }

        /// <summary>
        /// The designated constructor for the <see cref="LinearStateEstimator.Measurements.VoltagePhasorGroup"/> class.
        /// </summary>
        /// <param name="internalID">The unique integer identifier for each instance of a <see cref="LinearStateEstimator.Measurements.VoltagePhasorGroup"/> object.</param>
        /// <param name="number">A descriptive number for the <see cref="LinearStateEstimator.Measurements.VoltagePhasorGroup"/> object.</param>
        /// <param name="name">A descriptive name for the <see cref="LinearStateEstimator.Measurements.VoltagePhasorGroup"/> object.</param>
        /// <param name="description">A description of the <see cref="LinearStateEstimator.Measurements.VoltagePhasorGroup"/> object.</param>
        /// <param name="positiveSequence">The <see cref="LinearStateEstimator.Measurements.Phasor"/> representing positive sequence.</param>
        /// <param name="phaseA">The <see cref="LinearStateEstimator.Measurements.Phasor"/> representing phase A.</param>
        /// <param name="phaseB">The <see cref="LinearStateEstimator.Measurements.Phasor"/> representing phase B.</param>
        /// <param name="phaseC">The <see cref="LinearStateEstimator.Measurements.Phasor"/> representing phase C.</param>
        /// <param name="statusWord">The <see cref="LinearStateEstimator.Measurements.StatusWord"/> from the source device for the <see cref="LinearStateEstimator.Measurements.Phasor"/> objects in this <see cref="LinearStateEstimator.Measurements.PhasorGroup"/>.</param>
        /// <param name="measuredNode">The <see cref="LinearStateEstimator.Modeling.Node"/> measured by this <see cref="LinearStateEstimator.Measurements.VoltagePhasorGroup"/>.</param>
        public VoltagePhasorGroup(int internalID, int number, string name, string description, Phasor positiveSequence, Phasor phaseA, Phasor phaseB, Phasor phaseC, StatusWord statusWord, Node measuredNode)
            : base(internalID, number, name, description, positiveSequence, phaseA, phaseB, phaseC, statusWord)
        {
            MeasuredNode = measuredNode;

            PositiveSequence.Measurement.Type = PhasorType.VoltagePhasor;
            PositiveSequence.Estimate.Type = PhasorType.VoltagePhasor;

            NegativeSequence.Measurement.Type = PhasorType.VoltagePhasor;
            NegativeSequence.Estimate.Type = PhasorType.VoltagePhasor;

            ZeroSequence.Measurement.Type = PhasorType.VoltagePhasor;
            ZeroSequence.Estimate.Type = PhasorType.VoltagePhasor;

            PhaseA.Measurement.Type = PhasorType.VoltagePhasor;
            PhaseA.Estimate.Type = PhasorType.VoltagePhasor;

            PhaseB.Measurement.Type = PhasorType.VoltagePhasor;
            PhaseB.Estimate.Type = PhasorType.VoltagePhasor;

            PhaseC.Measurement.Type = PhasorType.VoltagePhasor;
            PhaseC.Estimate.Type = PhasorType.VoltagePhasor;
        }

        #endregion

        #region [ Public Methods ]

        /// <summary>
        /// A descriptive string representation of the instance of the class that can be used for a rudimentary momento design pattern.
        /// </summary>
        /// <returns>A descriptive string representation of the instance of the class.</returns>
        public override string ToString()
        {
            return "Voltage," + base.ToString();
        }

        /// <summary>
        /// A verbose descriptive string representation of the instance of the class that can be used for descriptive textual output on the console or in a text file.
        /// </summary>
        /// <returns>A verbose descriptive string representation of the instance of the class.</returns>
        public new string ToVerboseString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine();
            stringBuilder.AppendFormat("----- Voltage Phasor Group -----------------------------------------------------");
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
            stringBuilder.AppendFormat($"    MeasuredNode: {MeasuredNode.InternalID.ToString()} | {MeasuredNode.Name} | {MeasuredNode.BaseKV.Description}{Environment.NewLine}");
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
        /// Overridden to prevent compilation warnings.
        /// </summary>
        /// <returns>An integer hash code.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Determines equality between two <see cref="VoltagePhasorGroup"/> objects
        /// </summary>
        /// <param name="target">The target object for equality testing.</param>
        /// <returns>A bool representing the result of the equality check.</returns>
        public override bool Equals(object target)
        {
            // If parameter is null return false.
            if (target == null)
            {
                return false;
            }

            // If parameter cannot be cast to PhasorBase return false.
            VoltagePhasorGroup voltagePhasorGroup = target as VoltagePhasorGroup;

            if ((object)voltagePhasorGroup == null)
            {
                return false;
            }

            // Return true if the fields match:
            if (!base.Equals(voltagePhasorGroup))
            {
                return false;
            }
            else if (this.m_measuredNodeID != voltagePhasorGroup.MeasuredNodeID)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Performs a deep copy of the <see cref="VoltagePhasorGroup"/> object.
        /// </summary>
        /// <returns>A deep copy of the target <see cref="VoltagePhasorGroup"/>.</returns>
        public VoltagePhasorGroup Copy()
        {
            VoltagePhasorGroup copy = (VoltagePhasorGroup)this.MemberwiseClone();
            copy.Status = this.Status.Copy();
            copy.PositiveSequence = PositiveSequence.Copy();
            copy.PhaseA = PhaseA.Copy();
            copy.PhaseB = PhaseB.Copy();
            copy.PhaseC = PhaseC.Copy();
            return copy;
        }

        public void Keyify(string rootKey)
        {
            base.Keyify($"{rootKey}.V");
        }

        public void Unkeyify()
        {
            base.Unkeyify();
        }

        #endregion

        #region [ Private Methods ]

        private void SetBaseKvOfChildrenPhasors()
        {
            ZeroSequence.Measurement.BaseKV = m_measuredNode.BaseKV;
            ZeroSequence.Estimate.BaseKV = m_measuredNode.BaseKV;
            NegativeSequence.Measurement.BaseKV = m_measuredNode.BaseKV;
            NegativeSequence.Estimate.BaseKV = m_measuredNode.BaseKV;
            PositiveSequence.Measurement.BaseKV = m_measuredNode.BaseKV;
            PositiveSequence.Estimate.BaseKV = m_measuredNode.BaseKV;
            PhaseA.Measurement.BaseKV = m_measuredNode.BaseKV;
            PhaseA.Estimate.BaseKV = m_measuredNode.BaseKV;
            PhaseB.Measurement.BaseKV = m_measuredNode.BaseKV;
            PhaseB.Estimate.BaseKV = m_measuredNode.BaseKV;
            PhaseC.Measurement.BaseKV = m_measuredNode.BaseKV;
            PhaseC.Estimate.BaseKV = m_measuredNode.BaseKV;
        }

        #endregion
    }
}
