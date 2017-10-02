//******************************************************************************************************
//  PhasorGroup.cs
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
//  07/02/2013 - Kevin D. Jones
//       Fixed error in sequence component calculation.
//  08/10/2013 - Kevin D. Jones
//       Added I2-I1 ratio and measurement key to XML serialization. Temporary solution.
//  05/16/2014 - Kevin D. Jones
//       Added System.Numerics for complex numbers
//  05/17/2014 - Kevin D. Jones
//       Added Math.NET for complex matrices
//  07/07/2014 - Kevin D. Jones
//       Added Guid
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
using SynchrophasorAnalytics.Modeling;

namespace SynchrophasorAnalytics.Measurements
{
    /// <summary>
    /// Encapsulates a group of phasors measuring a flow in a +, A, B, C grouping and relates them to the network model.
    /// </summary>
    /// <seealso cref="LinearStateEstimator.Measurements.CurrentFlowPhasorGroup"/>
    /// <seealso cref="LinearStateEstimator.Measurements.CurrentInjectionPhasorGroup"/>
    /// <seealso cref="LinearStateEstimator.Measurements.VoltagePhasorGroup"/>
    /// <seealso cref="LinearStateEstimator.Measurements.PhasorType"/>
    [XmlRoot("PhasorGroup")]
    public class PhasorGroup : INetworkDescribable, IClearable
    {
        #region [ Constants ]

        private const int ZERO_SEQUENCE = 0;
        private const int POSITIVE_SEQUENCE = 1;
        private const int NEGATIVE_SEQUENCE = 2;
        private const int DEFAULT_INTERNAL_ID = 0;
        private const int DEFAULT_NUMBER = 0;
        private const string DEFAULT_NAME = "Undefined";
        private const string DEFAULT_DESCRIPTION = "Undefined";

        #endregion

        #region [ Private Members ]

        /// <summary>
        /// INetworkDescribable fields
        /// </summary>
        private Guid m_uniqueId;
        private int m_internalID;
        private int m_number;
        private string m_acronym;
        private string m_name;
        private string m_description;

        private bool m_enabled;
        private string m_measurementIsIncludedKey;

        private bool m_useStatusFlagForRemovingMeasurements;
        private StatusWord m_statusWord;
        private int m_statusWordID;

        private Phasor m_posSeq;
        private Phasor m_negSeq;
        private Phasor m_zeroSeq;
        private Phasor m_phaseA;
        private Phasor m_phaseB;
        private Phasor m_phaseC;

        private Complex ALPHA;
        private DenseMatrix A_INVERSE;

        private string m_negativeSequenceToPositiveSequenceRatioKey;
        private bool m_shouldSerializeEstimatedNegativeSequenceToPositiveSequenceRatio;

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
        /// The unique integer identifier for each instance of a <see cref="LinearStateEstimator.Measurements.PhasorGroup"/> object.
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
        /// A descriptive number for the <see cref="LinearStateEstimator.Measurements.PhasorGroup"/> object.
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
        /// A descriptive acronym for the <see cref="LinearStateEstimator.Measurements.PhasorGroup"/> object.
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
        /// A name for the <see cref="LinearStateEstimator.Measurements.PhasorGroup"/> object.
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
        /// A description of the <see cref="LinearStateEstimator.Measurements.PhasorGroup"/> object.
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
        /// A flag that represents whether the measurement is enabled.
        /// </summary>
        [XmlAttribute("Enabled")]
        public bool IsEnabled
        {
            get
            {
                return m_enabled;
            }
            set
            {
                m_enabled = value;
            }
        }

        /// <summary>
        /// A flag which represents whether to use the <see cref="LinearStateEstimator.Measurements.PhasorGroup.Status"/>to determine whether or not to include the phasor group in the estimator.
        /// </summary>
        [XmlAttribute("UseStatusFlag")]
        public bool UseStatusFlagForRemovingMeasurements
        {
            get 
            { 
                return m_useStatusFlagForRemovingMeasurements; 
            }
            set 
            {
                m_useStatusFlagForRemovingMeasurements = value; 
            }
        }

        /// <summary>
        /// The <see cref="LinearStateEstimator.Measurements.StatusWord"/> from the source device for the <see cref="LinearStateEstimator.Measurements.Phasor"/> objects in this <see cref="PhasorGroup"/>.
        /// </summary>
        [XmlIgnore()]
        public StatusWord Status
        {
            get
            { 
                return m_statusWord; 
            }
            set
            { 
                m_statusWord = value;
                m_statusWordID = value.InternalID;
            }
        }

        /// <summary>
        /// The <see cref="LinearStateEstimator.Modeling.INetworkDescribable.InternalID"/> of the source device for the measurements.
        /// </summary>
        [XmlAttribute("StatusWord")]
        public int StatusWordID
        {
            get 
            { 
                return m_statusWordID; 
            }
            set 
            { 
                m_statusWordID = value; 
            }
        }

        /// <summary>
        /// The <see cref="LinearStateEstimator.Measurements.Phasor"/> representing the positive sequence phasor in this <see cref="LinearStateEstimator.Measurements.PhasorGroup"/>.
        /// </summary>
        [XmlElement("PositiveSequence")]
        public Phasor PositiveSequence
        {
            get 
            { 
                return m_posSeq; 
            }
            set 
            { 
                m_posSeq = value; 
            }
        }

        /// <summary>
        /// The <see cref="Phasor"/> representing the negative sequence phasor in this <see cref="PhasorGroup"/>.
        /// </summary>
        [XmlIgnore()]
        public Phasor NegativeSequence
        {
            get 
            { 
                return m_negSeq; 
            }
            set 
            { 
                m_negSeq = value; 
            }
        }

        /// <summary>
        /// The <see cref="Phasor"/> representing the zero sequence phasor in this <see cref="PhasorGroup"/>.
        /// </summary>
        [XmlIgnore()]
        public Phasor ZeroSequence
        {
            get 
            { 
                return m_zeroSeq; 
            }
            set 
            { 
                m_zeroSeq = value; 
            }
        }

        /// <summary>
        /// The <see cref="Phasor"/> representing the A phase phasor in this <see cref="PhasorGroup"/>.
        /// </summary>
        [XmlElement("PhaseA")]
        public Phasor PhaseA
        {
            get 
            { 
                return m_phaseA; 
            }
            set 
            { 
                m_phaseA = value; 
            }
        }

        /// <summary>
        /// The <see cref="Phasor"/> representing the B phase phasor in this <see cref="PhasorGroup"/>.
        /// </summary>
        [XmlElement("PhaseB")]
        public Phasor PhaseB
        {
            get 
            { 
                return m_phaseB; 
            }
            set 
            { 
                m_phaseB = value; 
            }
        }

        /// <summary>
        /// The <see cref="Phasor"/> representing the C phase phasor in this <see cref="PhasorGroup"/>.
        /// </summary>
        [XmlElement("PhaseC")]
        public Phasor PhaseC
        {
            get 
            { 
                return m_phaseC; 
            }
            set 
            { 
                m_phaseC = value; 
            }
        }

        /// <summary>
        /// The ratio of negative sequence to positive sequence calculated with phase measurements.
        /// </summary>
        [XmlIgnore()]
        public double MeasuredNegativeSequenceToPositiveSequenceRatio
        {
            get
            {
                return m_negSeq.Measurement.Magnitude / m_posSeq.Measurement.Magnitude; 
            }
        }

        /// <summary>
        /// The ratio of negative sequence to positive sequence calculated with phase estimates.
        /// </summary>
        [XmlAttribute("I2I1Ratio")]
        public double EstimatedNegativeSequenceToPositiveSequenceRatio
        {
            get
            {
                return m_negSeq.Estimate.Magnitude / m_posSeq.Estimate.Magnitude;
            }
            set { }
        }

        /// <summary>
        /// A property which specifies whether the magnitude and angle data for the measurements and estimates should also be serialized when the object is serialized.
        /// </summary>
        [XmlIgnore()]
        public bool ShouldSerializeData
        {
            get
            {
                return m_shouldSerializeEstimatedNegativeSequenceToPositiveSequenceRatio;
            }
            set
            {
                m_shouldSerializeEstimatedNegativeSequenceToPositiveSequenceRatio = value;
            }
        }

        /// <summary>
        /// The ratio of zero sequence to positive sequence calculated with phase measurements.
        /// </summary>
        [XmlIgnore()]
        public double MeasuredZeroSequenceToPositiveSequenceRatio
        {
            get
            {
                return m_zeroSeq.Measurement.Magnitude / m_posSeq.Measurement.Magnitude;
            }
        }

        /// <summary>
        /// The ratio of zero sequence to positive sequence calculated with phase estimates.
        /// </summary>
        [XmlIgnore()]
        public double EstimatedZeroSequenceToPositiveSequenceRatio
        {
            get
            {
                return m_zeroSeq.Estimate.Magnitude / m_posSeq.Estimate.Magnitude;
            }
        }

        [XmlAttribute("MeasurementIsIncludedKey")]
        public string MeasurementIsIncludedKey
        {
            get
            {
                return m_measurementIsIncludedKey;
            }
            set
            {
                m_measurementIsIncludedKey = value;
            }
        }

        /// <summary>
        /// A flag which represents whether or not to include the phasor group in the
        /// positive sequence estimator.
        /// </summary>
        [XmlIgnore()]
        public bool IncludeInPositiveSequenceEstimator
        {
            get
            {
                if (m_enabled)
                {
                    if (m_useStatusFlagForRemovingMeasurements)
                    {
                        if (m_posSeq.Measurement.IncludeInEstimator == false ||
                                           m_statusWord.DataIsValid == true ||
                                m_statusWord.SynchronizationIsValid == true)
                        { 
                            return false; 
                        }
                        else 
                        { 
                            return true; 
                        }
                    }
                    else
                    {
                        if (m_posSeq.Measurement.IncludeInEstimator == false) 
                        { 
                            return false; 
                        }
                        else 
                        { 
                            return true; 
                        }
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// A flag which represents whether or not to include the phasor group in the estimator
        /// </summary>
        [XmlIgnore()]
        public bool IncludeInEstimator
        {
            get
            {
                if (m_enabled)
                {
                    if (m_useStatusFlagForRemovingMeasurements)
                    {
                        if (m_phaseA.Measurement.IncludeInEstimator == false ||
                            m_phaseB.Measurement.IncludeInEstimator == false ||
                            m_phaseC.Measurement.IncludeInEstimator == false ||
                                           m_statusWord.DataIsValid == true ||
                                              m_statusWord.PMUError == true ||
                                m_statusWord.SynchronizationIsValid == true)
                        { 
                            return false; 
                        }
                        else 
                        { 
                            return true; 
                        }
                    }
                    else
                    {
                        if (m_phaseA.Measurement.IncludeInEstimator == false ||
                            m_phaseB.Measurement.IncludeInEstimator == false ||
                            m_phaseC.Measurement.IncludeInEstimator == false)
                        { 
                            return false; 
                        }
                        else 
                        { 
                            return true; 
                        }
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        [XmlIgnore()]
        public bool ExpectsPositiveSequenceMeasurements
        {
            get
            {
                return m_posSeq.Measurement.ExpectsInputMeasurement;
            }
        }

        [XmlIgnore()]
        public bool ExpectsMeasurements
        {
            get
            {
                if (m_posSeq.Measurement.ExpectsInputMeasurement)
                {
                    return true;
                }
                else if (m_phaseA.Measurement.ExpectsInputMeasurement)
                {
                    return true;
                }
                else if (m_phaseB.Measurement.ExpectsInputMeasurement)
                {
                    return true;
                }
                else if (m_phaseC.Measurement.ExpectsInputMeasurement)
                {
                    return true;
                }
                return false;
            }
        }


        /// <summary>
        /// The output measurement key for the ratio of negative sequence magnitude to positive sequence magnitude.
        /// </summary>
        [XmlAttribute("NegativeSequenceKey")]
        public string NegativeSequenceToPositiveSequenceRatioMeasurementKey
        {
            get
            {
                return m_negativeSequenceToPositiveSequenceRatioKey;
            }
            set
            {
                m_negativeSequenceToPositiveSequenceRatioKey = value;
            }
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// A blank constructor with default values.
        /// </summary>
        public PhasorGroup()
            :this(DEFAULT_INTERNAL_ID, DEFAULT_NUMBER, DEFAULT_NAME, DEFAULT_DESCRIPTION)
        {
        }

        /// <summary>
        /// A constructor which specifies only the information required by the <see cref="LinearStateEstimator.Modeling.INetworkDescribable"/> interface. The <see cref="Phasor"/> objects for and <see cref="StatusWord"/> objects are instantiated with default initializers to prevent null references.
        /// </summary>
        /// <param name="internalID">The unique integer identifier for each instance of a <see cref="LinearStateEstimator.Measurements.PhasorGroup"/> object.</param>
        /// <param name="number">A descriptive number for the <see cref="LinearStateEstimator.Measurements.PhasorGroup"/> object.</param>
        /// <param name="name">A descriptive name for the <see cref="LinearStateEstimator.Measurements.PhasorGroup"/> object.</param>
        /// <param name="description">A description of the <see cref="LinearStateEstimator.Measurements.PhasorGroup"/> object.</param>
        public PhasorGroup(int internalID, int number, string name, string description)
            :this(internalID, number, name, description, new Phasor(), new Phasor(), new Phasor(), new Phasor())
        {
        }

        /// <summary>
        /// A constructor which specifies the information required by the <see cref="LinearStateEstimator.Modeling.INetworkDescribable"/> interface and the <see cref="LinearStateEstimator.Measurements.Phasor"/> objects for +, A, B, and C. The <see cref="LinearStateEstimator.Measurements.StatusWord"/> is instantiated with a default initializer to prevent null references.
        /// </summary>
        /// <param name="internalID">The unique integer identifier for each instance of a <see cref="LinearStateEstimator.Measurements.PhasorGroup"/> object.</param>
        /// <param name="number">A descriptive number for the <see cref="LinearStateEstimator.Measurements.PhasorGroup"/> object.</param>
        /// <param name="name">A descriptive name for the <see cref="LinearStateEstimator.Measurements.PhasorGroup"/> object.</param>
        /// <param name="description">A description of the <see cref="LinearStateEstimator.Measurements.PhasorGroup"/> object.</param>
        /// <param name="positiveSequence">The <see cref="LinearStateEstimator.Measurements.Phasor"/> representing positive sequence.</param>
        /// <param name="phaseA">The <see cref="LinearStateEstimator.Measurements.Phasor"/> representing phase A.</param>
        /// <param name="phaseB">The <see cref="LinearStateEstimator.Measurements.Phasor"/> representing phase B.</param>
        /// <param name="phaseC">The <see cref="LinearStateEstimator.Measurements.Phasor"/> representing phase C.</param>
        public PhasorGroup(int internalID, int number, string name, string description, Phasor positiveSequence, Phasor phaseA, Phasor phaseB, Phasor phaseC)
            :this(internalID, number, name, description, positiveSequence, phaseA, phaseB, phaseC, new StatusWord())
        {
        }

        /// <summary>
        /// The designated constructor for the <see cref="LinearStateEstimator.Measurements.PhasorGroup"/> class.
        /// </summary>
        /// <param name="internalID">The unique integer identifier for each instance of a <see cref="LinearStateEstimator.Measurements.PhasorGroup"/> object.</param>
        /// <param name="number">A descriptive number for the <see cref="LinearStateEstimator.Measurements.PhasorGroup"/> object.</param>
        /// <param name="name">A descriptive name for the <see cref="LinearStateEstimator.Measurements.PhasorGroup"/> object.</param>
        /// <param name="description">A description of the <see cref="LinearStateEstimator.Measurements.PhasorGroup"/> object.</param>
        /// <param name="positiveSequence">The <see cref="LinearStateEstimator.Measurements.Phasor"/> representing positive sequence.</param>
        /// <param name="phaseA">The <see cref="LinearStateEstimator.Measurements.Phasor"/> representing phase A.</param>
        /// <param name="phaseB">The <see cref="LinearStateEstimator.Measurements.Phasor"/> representing phase B.</param>
        /// <param name="phaseC">The <see cref="LinearStateEstimator.Measurements.Phasor"/> representing phase C.</param>
        /// <param name="statusWord">The <see cref="LinearStateEstimator.Measurements.StatusWord"/> from the source device for the <see cref="LinearStateEstimator.Measurements.Phasor"/> objects in this <see cref="PhasorGroup"/>.</param>
        public PhasorGroup(int internalID, int number, string name, string description, Phasor positiveSequence, Phasor phaseA, Phasor phaseB, Phasor phaseC, StatusWord statusWord)
        {
            m_internalID = internalID;
            m_number = number;
            m_name = name;
            m_description = description;
            m_posSeq = positiveSequence;
            m_negSeq = new Phasor();
            m_negSeq.Measurement.BaseKV = m_posSeq.Measurement.BaseKV;
            m_negSeq.Estimate.BaseKV = m_posSeq.Estimate.BaseKV;
            m_zeroSeq = new Phasor();
            m_zeroSeq.Measurement.BaseKV = m_posSeq.Measurement.BaseKV;
            m_zeroSeq.Estimate.BaseKV = m_posSeq.Estimate.BaseKV;
            m_phaseA = phaseA;
            m_phaseB = phaseB;
            m_phaseC = phaseC;
            m_statusWord = statusWord;
            m_negativeSequenceToPositiveSequenceRatioKey = "Undefined";
            m_measurementIsIncludedKey = "Undefined";
            InitializeDefaultParameters();
        }

        #endregion

        #region [ Public Methods ]

        /// <summary>
        /// Computes the positive, negative, and zero sequence component phasors given the phase components are valid.
        /// </summary>
        public void ComputeSequenceComponents()
        {
            DenseMatrix measuredPhaseComponents = DenseMatrix.OfArray(new Complex[,] {{m_phaseA.Measurement.ComplexPhasor},
                                                                                               {m_phaseB.Measurement.ComplexPhasor},
                                                                                               {m_phaseC.Measurement.ComplexPhasor}});

            DenseMatrix estimatedPhaseComponents = DenseMatrix.OfArray(new Complex[,] {{m_phaseA.Estimate.ComplexPhasor},
                                                                                                {m_phaseB.Estimate.ComplexPhasor},
                                                                                                {m_phaseC.Estimate.ComplexPhasor}});

            DenseMatrix measuredSequenceComponents = A_INVERSE * measuredPhaseComponents;
            DenseMatrix estimatedSequenceComponents = A_INVERSE * estimatedPhaseComponents;

            m_posSeq.Measurement.ComplexPhasor = measuredSequenceComponents[POSITIVE_SEQUENCE, 0] / 3;
            m_posSeq.Estimate.ComplexPhasor = estimatedSequenceComponents[POSITIVE_SEQUENCE, 0] / 3;

            m_negSeq.Measurement.ComplexPhasor = measuredSequenceComponents[NEGATIVE_SEQUENCE, 0] / 3;
            m_negSeq.Estimate.ComplexPhasor = estimatedSequenceComponents[NEGATIVE_SEQUENCE, 0] / 3;

            m_zeroSeq.Measurement.ComplexPhasor = measuredSequenceComponents[ZERO_SEQUENCE, 0] / 3;
            m_zeroSeq.Estimate.ComplexPhasor = estimatedSequenceComponents[ZERO_SEQUENCE, 0] / 3;
        }

        /// <summary>
        /// Clears the magnitude and angle values of each of the phasors (+, -, 0, A, B, C) by setting them to 0.
        /// </summary>
        public void ClearValues()
        {
            m_posSeq.Measurement.ClearValues();
            m_posSeq.Estimate.ClearValues();

            m_phaseA.Measurement.ClearValues();
            m_phaseA.Estimate.ClearValues();

            m_phaseB.Measurement.ClearValues();
            m_phaseB.Estimate.ClearValues();

            m_phaseC.Measurement.ClearValues();
            m_phaseC.Estimate.ClearValues();
        }

        /// <summary>
        /// A descriptive string representation of the <see cref="LinearStateEstimator.Measurements.PhasorGroup"/>.
        /// </summary>
        /// <returns>A descriptive string representation of the <see cref="LinearStateEstimator.Measurements.PhasorGroup"/>.</returns>
        public override string ToString()
        {
            return m_internalID.ToString() + "," + m_number + "," + m_acronym + "," + m_name + "," + m_description + "," + m_enabled.ToString() + "," + m_useStatusFlagForRemovingMeasurements.ToString() + "," + m_statusWordID.ToString() + "," + m_posSeq.ToString() + "," + m_phaseA.ToString() + "," + m_phaseB.ToString() + "," + m_phaseC.ToString();
        }

        /// <summary>
        /// A verbose descriptive string representation of the <see cref="LinearStateEstimator.Measurements.PhasorGroup"/>.
        /// </summary>
        /// <returns>A verbose descriptive string representation of the <see cref="LinearStateEstimator.Measurements.PhasorGroup"/>.</returns>
        public string ToVerboseString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine();
            stringBuilder.AppendFormat("----- Phasor Group -------------------------------------------------------------");
            stringBuilder.AppendLine();
            stringBuilder.AppendFormat("      InternalID: " + m_internalID.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("          Number: " + m_number.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("         Acronym: " + m_acronym + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("            Name: " + m_name + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("     Description: " + m_description + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("       UseStatus: " + m_useStatusFlagForRemovingMeasurements.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("          Status: " + m_statusWord.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("     PosSeqMKeys: " + PositiveSequence.Measurement.MagnitudeKey + " " + PositiveSequence.Measurement.AngleKey + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("     PosSeqEKeys: " + PositiveSequence.Estimate.MagnitudeKey + " " + PositiveSequence.Estimate.AngleKey + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("     PhaseAMKeys: " + PhaseA.Measurement.MagnitudeKey + " " + PhaseA.Measurement.AngleKey + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("     PhaseAEKeys: " + PhaseA.Estimate.MagnitudeKey + " " + PhaseA.Estimate.AngleKey + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("     PhaseBMKeys: " + PhaseB.Measurement.MagnitudeKey + " " + PhaseB.Measurement.AngleKey + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("     PhaseBEKeys: " + PhaseB.Estimate.MagnitudeKey + " " + PhaseB.Estimate.AngleKey + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("     PhaseCMKeys: " + PhaseC.Measurement.MagnitudeKey + " " + PhaseC.Measurement.AngleKey + "{0}", Environment.NewLine);
            stringBuilder.AppendFormat("     PhaseCEKeys: " + PhaseC.Estimate.MagnitudeKey + " " + PhaseC.Estimate.AngleKey + "{0}", Environment.NewLine);
            stringBuilder.AppendLine();
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Performs a deep copy of the target <see cref="LinearStateEstimator.Measurements.PhasorGroup"/> object.
        /// </summary>
        /// <returns>A deep copy of the target <see cref="LinearStateEstimator.Measurements.PhasorGroup"/> object.</returns>
        public PhasorGroup DeepCopy()
        {
            PhasorGroup copy = (PhasorGroup)this.MemberwiseClone();
            copy.Status = m_statusWord.Copy();
            copy.PositiveSequence = m_posSeq.Copy();
            copy.NegativeSequence = m_negSeq.Copy();
            copy.ZeroSequence = m_zeroSeq.Copy();
            copy.PhaseA = m_phaseA.Copy();
            copy.PhaseB = m_phaseB.Copy();
            copy.PhaseC = m_phaseC.Copy();
            return copy;
        }

        /// <summary>
        /// A method which returns a flag which indicates whether to serialize the estimated negative sequenc to positive sequence ratio.
        /// </summary>
        /// <returns>A flag which indicates whether to serialize the estimated negative sequenc to positive sequence ratio.</returns>
        public bool ShouldSerializeEstimatedNegativeSequenceToPositiveSequenceRatio()
        {
            return m_shouldSerializeEstimatedNegativeSequenceToPositiveSequenceRatio;
        }

        public void KeyifyValidationFlag(string rootKey)
        {
            MeasurementIsIncludedKey = $"{rootKey}.Included";
        }

        public void UnkeyifyValidationFlag()
        {
            MeasurementIsIncludedKey = "Undefined";
        }

        public void Keyify(string rootKey)
        {
            PositiveSequence.Keyify($"{rootKey}.+");
            NegativeSequence.Keyify($"{rootKey}.-");
            ZeroSequence.Keyify($"{rootKey}.0");
            PhaseA.Keyify($"{rootKey}.A");
            PhaseB.Keyify($"{rootKey}.B");
            PhaseC.Keyify($"{rootKey}.C");
            KeyifyValidationFlag(rootKey);
        }

        public void Unkeyify()
        {
            PositiveSequence.Unkeyify();
            NegativeSequence.Unkeyify();
            ZeroSequence.Unkeyify();
            PhaseA.Unkeyify();
            PhaseB.Unkeyify();
            PhaseC.Unkeyify();
            MeasurementIsIncludedKey = "Undefined";
        }
        
        #endregion

        #region [ Private Methods ]

        /// <summary>
        /// Initializes default parameters that cannot be set as constants due to their type.
        /// </summary>
        private void InitializeDefaultParameters()
        {
            ALPHA = new Complex(Math.Cos(2 * Math.PI / 3), Math.Sin(2 * Math.PI / 3));
            A_INVERSE = DenseMatrix.OfArray(new Complex[,] {{ 1, 1, 1 }, 
                                                            { 1, ALPHA, ALPHA * ALPHA }, 
                                                            { 1, ALPHA * ALPHA, ALPHA}});
        }

        #endregion
    }
}
