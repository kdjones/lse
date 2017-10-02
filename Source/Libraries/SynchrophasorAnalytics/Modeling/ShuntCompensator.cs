//******************************************************************************************************
//  ShuntCompensator.cs
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
//  05/09/2014 - Kevin D. Jones
//       Removed the IImpedance interface to remove the impedance properties that dont make sense
//       as a shunt.
//  05/16/2014 - Kevin D. Jones
//       Added System.Numerics for complex numbers
//  05/17/2014 - Kevin D. Jones
//       Added Math.NET for complex matrices
//  06/09/2014 - Kevin D. Jones
//       Updated XML inline documentation.
//  07/27/2014 - Kevin D. Jones
//       Added impedance and mvar rating calculations.
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
using SynchrophasorAnalytics.Measurements;
using SynchrophasorAnalytics.Networks;

namespace SynchrophasorAnalytics.Modeling
{
    /// <summary>
    /// Represents a shunt capacitor or reactor in an electric power network.
    /// </summary>
    [Serializable()]
    public class ShuntCompensator : ISingleTerminal, INetworkDescribable
    {
        #region [ Private Members ]

        /// <summary>
        /// INetworkDescribable fields
        /// </summary>
        private int m_internalID;
        private int m_number;
        private string m_name;
        private string m_description;

        /// <summary>
        /// ISingleTerminal
        /// </summary>
        private Node m_connectedNode;
        private int m_connectedNodeID;

        /// <summary>
        /// Parent
        /// </summary>
        private Substation m_parentSubstation;
        private int m_parentSubstationID;

        /// <summary>
        /// Child
        /// </summary>
        private CurrentInjectionPhasorGroup m_current;
        private int m_currentId;

        private Impedance m_impedance;
        private Impedance m_realTimeCalculatedImpedance;
        private ShuntImpedanceCalculationMethod m_impedanceCalculationMethod;
        private double m_nominalMvar;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// An integer identifier for each <see cref="LinearStateEstimator.Modeling.ShuntCompensator"/> which is intended to be unique among other objects of the same type.
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
        /// A descriptive integer for the instance of the <see cref="LinearStateEstimator.Modeling.ShuntCompensator"/>. There are no restrictions on uniqueness. 
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
        /// A descriptive acronym for the <see cref="LinearStateEstimator.Modeling.ShuntCompensator"/> object. Will always return 'G + jB'.
        /// </summary>
        [XmlIgnore()]
        public string Acronym
        {
            get 
            { 
                return "G + jB"; 
            }
            set 
            { 
            }
        }

        /// <summary>
        /// The string name of the instance of the <see cref="LinearStateEstimator.Modeling.ShuntCompensator"/>.
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
        /// A string description of the instance of the <see cref="LinearStateEstimator.Modeling.ShuntCompensator"/>.
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
        /// Returns the type of the object as a string.
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
        /// The <see cref="LinearStateEstimator.Modeling.Node"/> in the network which the <see cref="LinearStateEstimator.Modeling.ShuntCompensator"/> connects to.
        /// </summary>
        [XmlIgnore()]
        public Node ConnectedNode
        {
            get
            {
                return m_connectedNode;
            }
            set
            {
                m_connectedNode = value;
                m_connectedNodeID = value.InternalID;
                if (m_current != null)
                {
                    m_current.MeasuredConnectedNode = m_connectedNode;
                }
            }
        }

        /// <summary>
        /// The rated MVAR output of the shunt device at the nominal voltage level dictated by the baseKV of the node its connected to.
        /// </summary>
        [XmlAttribute("MVAR")]
        public double NominalMvar
        {
            get
            {
                return m_nominalMvar;
            }
            set
            {
                m_nominalMvar = value;
            }
        }

        /// <summary>
        /// The <see cref="LinearStateEstimator.Modeling.Node.InternalID"/> of the <see cref="LinearStateEstimator.Modeling.ISingleTerminal.ConnectedNode"/>.
        /// </summary>
        [XmlAttribute("ConnectedNode")]
        public int ConnectedNodeID
        {
            get
            {
                return m_connectedNodeID;
            }
            set
            {
                m_connectedNodeID = value;
            }
        }

        /// <summary>
        /// The positive sequence complex (G + jB) shunt susceptance.
        /// </summary>
        [XmlIgnore()]
        public Complex PositiveSequenceShuntSusceptance
        {
            get 
            { 
                return m_impedance.PositiveSequenceShuntSusceptance; 
            }
        }

        /// <summary>
        /// The three phase complex (G + JB) shunt susceptance as a 3 x 3 complex matrix.
        /// </summary>
        [XmlIgnore()]
        public DenseMatrix ThreePhaseShuntSusceptance
        {
            get 
            { 
                return m_impedance.ThreePhaseShuntSusceptance; 
            }
        }

        /// <summary>
        /// A flag which determines whether the impedance should be determined from the modeled impedance values or from the nominal MVAR rating.
        /// </summary>
        [XmlAttribute("UseImpedance")]
        public ShuntImpedanceCalculationMethod ImpedanceCalculationMethod
        {
            get
            {
                return m_impedanceCalculationMethod;
            }
            set
            {
                m_impedanceCalculationMethod = value;
            }
        }

        /// <summary>
        /// The parent/owner <see cref="LinearStateEstimator.Modeling.Substation"/>.
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
        /// The <see cref="LinearStateEstimator.Modeling.Substation.InternalID"/> of the parent <see cref="LinearStateEstimator.Modeling.ShuntCompensator.ParentSubstation"/>.
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
        /// The raw impedance parameters for the shunt. Only the B values are used.
        /// </summary>
        [XmlElement("Impedance")]
        public Impedance RawImpedanceParameters
        {
            get 
            { 
                return m_impedance; 
            }
            set 
            { 
                m_impedance = value; 
            }
        }

        /// <summary>
        /// The <see cref="LinearStateEstimator.Measurements.CurrentInjectionPhasorGroup"/> for the shunt device.
        /// </summary>
        [XmlElement("Current")]
        public CurrentInjectionPhasorGroup Current
        {
            get
            {
                return m_current;
            }
            set
            {
                m_current = value;
                m_currentId = value.InternalID;
                if (ConnectedNode != null)
                {
                    m_current.MeasuredConnectedNode = m_connectedNode;
                }
                m_current.MeasuredBranch = this;
            }
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// A blank constructor with default values.
        /// </summary>
        public ShuntCompensator()
            :this(0, 0, "Undefined Name","Undefined Description", new Impedance(), 0, 0, ShuntImpedanceCalculationMethod.UseModeledImpedance)
        {
        }

        /// <summary>
        /// The designated constructor for the <see cref="ShuntCompensator"/> class.
        /// </summary>
        /// <param name="internalID">The unique integer identifier for each instance of a <see cref="ShuntCompensator"/> object.</param>
        /// <param name="number">A descriptive number for the <see cref="ShuntCompensator"/> object.</param>
        /// <param name="name">A name for the <see cref="ShuntCompensator"/> object.</param>
        /// <param name="description">A description of the <see cref="ShuntCompensator"/> object. </param>
        /// <param name="impedance">The <see cref="Impedance"/> of the <see cref="ShuntCompensator"/>.</param>
        /// <param name="connectedNodeID">The <see cref="Node.InternalID"/> of the <see cref="ConnectedNode"/>.</param>
        /// <param name="nominalMvar">The nominal Mvar at the rated voltage for the <see cref="ConnectedNode"/>.</param>
        /// <param name="impedanceCalculationMethod">A flag which determines which impedance values to use.</param>
        public ShuntCompensator(int internalID, int number, string name, string description, Impedance impedance, int connectedNodeID, double nominalMvar, ShuntImpedanceCalculationMethod impedanceCalculationMethod)
        {
            m_internalID = internalID;
            m_number = number;
            m_name = name;
            m_description = description;
            m_impedance = impedance;
            m_connectedNodeID = connectedNodeID;
            this.RawImpedanceParameters.ParentElement = this;
        }

        #endregion

        #region [ Public Methods ]

        /// <summary>
        /// A descriptive string representation of the <see cref="LinearStateEstimator.Modeling.ShuntCompensator"/> class instance. The format is <i>Shunt,internalId,number,name,description,connectedNodeInternalId,parentSubstationInternalId,currentInjectionInternalId,nominalMvar,impedanceCalculationMethod,r1,r2,r3,r4,r5,r6,x1,x2,x3,x4,x5,x6,g1,g2,g3,g4,g5,g6,b1,b2,b3,b4,b5,b6</i> and can be used for a rudimentary momento design pattern.
        /// </summary>
        /// <returns>A string representation of the instance of the <see cref="LinearStateEstimator.Modeling.ShuntCompensator"/> class.</returns>
        public override string ToString()
        {
            return "Shunt," + m_internalID.ToString() + "," + m_number.ToString() + "," + m_name + "," + m_description + "," + m_connectedNodeID.ToString() + "," + m_parentSubstationID.ToString() + "," + m_currentId.ToString() + "," + m_nominalMvar.ToString() + "," + m_impedanceCalculationMethod.ToString() + "," + m_impedance.ToString();
        }

        /// <summary>
        /// A verbose string representation of the instance of the <see cref="LinearStateEstimator.Modeling.ShuntCompensator"/> class and can be used for descriptive console or text file output.
        /// </summary>
        /// <returns>A verbose string representation of the instance of the <see cref="LinearStateEstimator.Modeling.ShuntCompensator"/> class.</returns>
        public string ToVerboseString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine();
            stringBuilder.AppendFormat("----- Shunt Branch -------------------------------------------------------------");
            stringBuilder.AppendLine();
            stringBuilder.AppendFormat("      InternalID: " + m_internalID.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("          Number: " + m_number.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("            Name: " + m_name + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("     Description: " + m_description + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("   ConnectedNode: " + m_connectedNode.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat(" ImpedanceMethod: " + m_impedanceCalculationMethod.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("       RatedMVAR: " + m_nominalMvar.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendLine();
            stringBuilder.AppendFormat(m_impedance.ToVerboseString());
            stringBuilder.AppendLine();
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Computes the positive sequence current injection of the shunt from estimated values.
        /// </summary>
        public void ComputePositiveSequenceEstimatedCurrentInjection()
        {
            if (m_parentSubstation.ParentDivision.ParentCompany.ParentModel.CurrentInjectionPostProcessingSetting == CurrentInjectionPostProcessingSetting.ProcessShuntsByNodeObservability)
            {
                if (m_connectedNode.IsObserved)
                {
                    Complex V = ConnectedNode.Voltage.PositiveSequence.Estimate.PerUnitComplexPhasor;

                    Complex I = (PositiveSequenceShuntSusceptance) * V;

                    if (m_current.MeasurementDirectionConvention == CurrentInjectionDirectionConvention.IntoTheShunt)
                    {
                        Current.PositiveSequence.Estimate.PerUnitComplexPhasor = I;
                    }
                    else if (m_current.MeasurementDirectionConvention == CurrentInjectionDirectionConvention.OutOfTheShunt)
                    {
                        Current.PositiveSequence.Estimate.PerUnitComplexPhasor = -I;
                    }
                }
                else
                {
                    Current.PositiveSequence.Estimate.Magnitude = 0;
                    Current.PositiveSequence.Estimate.AngleInDegrees = 0;
                }
            }
            else if (m_parentSubstation.ParentDivision.ParentCompany.ParentModel.CurrentInjectionPostProcessingSetting == CurrentInjectionPostProcessingSetting.ProcessOnlyMeasuredShunts)
            {
                if (Current.IncludeInPositiveSequenceEstimator && m_connectedNode.IsObserved)
                {
                    Complex V = ConnectedNode.Voltage.PositiveSequence.Estimate.PerUnitComplexPhasor;

                    Complex I = (PositiveSequenceShuntSusceptance) * V;

                    if (m_current.MeasurementDirectionConvention == CurrentInjectionDirectionConvention.IntoTheShunt)
                    {
                        Current.PositiveSequence.Estimate.PerUnitComplexPhasor = I;
                    }
                    else if (m_current.MeasurementDirectionConvention == CurrentInjectionDirectionConvention.OutOfTheShunt)
                    {
                        Current.PositiveSequence.Estimate.PerUnitComplexPhasor = -I;
                    }
                }
                else
                {
                    Current.PositiveSequence.Estimate.Magnitude = 0;
                    Current.PositiveSequence.Estimate.AngleInDegrees = 0;
                }
            }
        }

        /// <summary>
        /// Computes the three phase current injection of the shunt from estimated values.
        /// </summary>
        public void ComputeThreePhaseEstimatedCurrentInjection()
        {
            if (m_parentSubstation.ParentDivision.ParentCompany.ParentModel.CurrentInjectionPostProcessingSetting == CurrentInjectionPostProcessingSetting.ProcessShuntsByNodeObservability)
            {
                if (m_connectedNode.IsObserved)
                {
                    Complex V = ConnectedNode.Voltage.PositiveSequence.Estimate.PerUnitComplexPhasor;

                    Complex I = (PositiveSequenceShuntSusceptance) * V;

                    if (m_current.MeasurementDirectionConvention == CurrentInjectionDirectionConvention.IntoTheShunt)
                    {
                        Current.PositiveSequence.Estimate.PerUnitComplexPhasor = I;
                    }
                    else if (m_current.MeasurementDirectionConvention == CurrentInjectionDirectionConvention.OutOfTheShunt)
                    {
                        Current.PositiveSequence.Estimate.PerUnitComplexPhasor = -I;
                    }
                }
                else
                {
                    Current.PositiveSequence.Estimate.Magnitude = 0;
                    Current.PositiveSequence.Estimate.AngleInDegrees = 0;
                }
            }
            else if (m_parentSubstation.ParentDivision.ParentCompany.ParentModel.CurrentInjectionPostProcessingSetting == CurrentInjectionPostProcessingSetting.ProcessOnlyMeasuredShunts)
            {
                if (Current.IncludeInPositiveSequenceEstimator && m_connectedNode.IsObserved)
                {
                    DenseMatrix V = DenseMatrix.OfArray(new Complex[3, 1]);
                    V[0, 0] = ConnectedNode.Voltage.PhaseA.Estimate.PerUnitComplexPhasor;
                    V[1, 0] = ConnectedNode.Voltage.PhaseB.Estimate.PerUnitComplexPhasor;
                    V[2, 0] = ConnectedNode.Voltage.PhaseC.Estimate.PerUnitComplexPhasor;

                    DenseMatrix I = (ThreePhaseShuntSusceptance) * V;

                    if (m_current.MeasurementDirectionConvention == CurrentInjectionDirectionConvention.IntoTheShunt)
                    {
                        Current.PhaseA.Estimate.PerUnitComplexPhasor = I[0, 0];
                        Current.PhaseB.Estimate.PerUnitComplexPhasor = I[1, 0];
                        Current.PhaseC.Estimate.PerUnitComplexPhasor = I[2, 0];
                    }
                    else if (m_current.MeasurementDirectionConvention == CurrentInjectionDirectionConvention.OutOfTheShunt)
                    {
                        Current.PhaseA.Estimate.PerUnitComplexPhasor = -I[0, 0];
                        Current.PhaseB.Estimate.PerUnitComplexPhasor = -I[1, 0];
                        Current.PhaseC.Estimate.PerUnitComplexPhasor = -I[2, 0];
                    }
                }
                else
                {
                    Current.PhaseA.Estimate.Magnitude = 0;
                    Current.PhaseA.Estimate.AngleInDegrees = 0;
                    Current.PhaseB.Estimate.Magnitude = 0;
                    Current.PhaseB.Estimate.AngleInDegrees = 0;
                    Current.PhaseC.Estimate.Magnitude = 0;
                    Current.PhaseC.Estimate.AngleInDegrees = 0;
                }
            }
        }

        /// <summary>
        /// Computes the real time impedance of the shunt based on available current and voltage phasors.
        /// </summary>
        public void ComputeRealTimePositiveSequenceImpedance()
        {
            PhasorMeasurement V = ConnectedNode.Voltage.PositiveSequence.Measurement;
            PhasorMeasurement I = Current.PositiveSequence.Measurement;

            if (V.IncludeInEstimator && I.IncludeInEstimator)
            {
                double conductance = 0;
                double susceptance = 0;

                if (Current.MeasurementDirectionConvention == CurrentInjectionDirectionConvention.IntoTheShunt)
                {
                    // Shunt Susceptance
                    conductance = 2 * (V.PerUnitComplexPhasor / I.PerUnitComplexPhasor.Conjugate()).Real;
                    susceptance = 2 * (V.PerUnitComplexPhasor / I.PerUnitComplexPhasor.Conjugate()).Imaginary;
                }
                else if (Current.MeasurementDirectionConvention == CurrentInjectionDirectionConvention.OutOfTheShunt)
                {
                    // Shunt Susceptance
                    conductance = 2 * (V.PerUnitComplexPhasor / (-I.PerUnitComplexPhasor.Conjugate())).Real;
                    susceptance = 2 * (V.PerUnitComplexPhasor / (-I.PerUnitComplexPhasor.Conjugate())).Imaginary;
                }

                // Only set the values for positive sequence equivalence
                Impedance impedance = new Impedance()
                {
                    G1 = conductance,
                    G3 = conductance,
                    G6 = conductance,
                    B1 = susceptance,
                    B3 = susceptance,
                    B6 = susceptance
                };

                m_realTimeCalculatedImpedance = impedance;
            }
        }

        /// <summary>
        /// Assigns the nominal mvar rating to the shunt based on the modeled impedance if the <see cref="ShuntImpedanceCalculationMethod"/> is set to <see cref="ShuntImpedanceCalculationMethod.UseModeledImpedance"/>.
        /// </summary>
        public void AssignMvarRatingIfImpedanceIsModeled()
        {
            if (m_impedanceCalculationMethod == ShuntImpedanceCalculationMethod.UseModeledImpedance)
            {
                m_nominalMvar = (m_connectedNode.BaseKV.Value * m_connectedNode.BaseKV.Value) / m_impedance.X1;
            }
        }

        /// <summary>
        /// Assigns the impedance of the shunt based on the modeled mvar rating if the <see cref="ShuntImpedanceCalculationMethod"/> is set to <see cref="ShuntImpedanceCalculationMethod.CalculateFromRating"/>.
        /// </summary>
        public void AssignImpedanceIfMvarRatingIsModeled()
        {
            if (m_impedanceCalculationMethod == ShuntImpedanceCalculationMethod.CalculateFromRating)
            {
                m_impedance.ClearImpedanceValues();
                m_impedance.B1 = 100 / m_nominalMvar;
                m_impedance.B3 = 100 / m_nominalMvar;
                m_impedance.B6 = 100 / m_nominalMvar;
            }
        }

        #endregion
    }
}
