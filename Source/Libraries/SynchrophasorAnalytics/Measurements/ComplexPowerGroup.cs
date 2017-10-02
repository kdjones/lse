//******************************************************************************************************
//  ComplexPowerGroup.cs
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
//
//******************************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Xml.Serialization;

namespace SynchrophasorAnalytics.Measurements
{
    /// <summary>
    /// Encapsulates the power calculations for a paired <see cref="LinearStateEstimator.Measurements.VoltagePhasorGroup"/> and <see cref="LinearStateEstimator.Measurements.CurrentFlowPhasorGroup"/>.
    /// </summary>
    /// <seealso cref="LinearStateEstimator.Measurements.PhasorGroup"/>
    /// <seealso cref="LinearStateEstimator.Measurements.VoltagePhasorGroup"/>
    /// <seealso cref="LinearStateEstimator.Measurements.CurrentFlowPhasorGroup"/>
    /// <see cref="LinearStateEstimator.Measurements.IPowerCalculatable"/>
    public class ComplexPowerGroup : PhasorGroup, IPowerCalculatable
    {
        #region [ Private Members ]

        private VoltagePhasorGroup m_voltage;
        private CurrentFlowPhasorGroup m_current;
        private bool m_useEstimatedValues;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// A flag which indicates whether to use the <see cref="LinearStateEstimator.Measurements.PhasorEstimate"/> or the <see cref="LinearStateEstimator.Measurements.PhasorMeasurement"/> to calculate the complex power.
        /// </summary>
        public bool UseEstimatedValues
        {
            get 
            {
                return m_useEstimatedValues; 
            }
            set 
            { 
                m_useEstimatedValues = value; 
            }
        }

        /// <summary>
        /// The <see cref="LinearStateEstimator.Measurements.VoltagePhasorGroup"/> used to calculate the complex power.
        /// </summary>
        public VoltagePhasorGroup Voltage
        {
            get 
            { 
                return m_voltage; 
            }
            set 
            {
                m_voltage = value;
            }
        }

        /// <summary>
        /// The <see cref="LinearStateEstimator.Measurements.CurrentFlowPhasorGroup"/> used to calculate the complex power.
        /// </summary>
        public CurrentFlowPhasorGroup Current
        {
            get 
            { 
                return m_current; 
            }
            set 
            { 
                m_current = value; 
            }
        }

        /// <summary>
        /// The three phase complex power
        /// </summary>
        public Complex ThreePhaseComplexPower
        {
            get 
            { 
                return PhaseAComplexPower + PhaseBComplexPower + PhaseCComplexPower; 
            }
        }

        /// <summary>
        /// The three phase apparent power (magnitude of the complex power).
        /// </summary>
        public double ThreePhaseApparentPower
        {
            get 
            { 
                return PhaseAApparentPower + PhaseBApparentPower + PhaseCApparentPower; 
            }
        }

        /// <summary>
        /// The three phase real power (real part of the complex power).
        /// </summary>
        public double ThreePhaseRealPower
        {
            get 
            { 
                return PhaseARealPower + PhaseBRealPower + PhaseCRealPower; 
            }
        }

        /// <summary>
        /// The three phase reactive power (imaginary part of the complex power).
        /// </summary>
        public double ThreePhaseReactivePower
        {
            get 
            { 
                return PhaseAReactivePower + PhaseBReactivePower + PhaseCReactivePower; 
            }
        }

        /// <summary>
        /// The positive sequence complex power
        /// </summary>
        public Complex PositiveSequenceComplexPower
        {
            get
            {
                if (m_useEstimatedValues)
                {
                    return PositiveSequence.Estimate.ComplexPhasor;
                }
                else
                {
                    return PositiveSequence.Measurement.ComplexPhasor;
                }
            }
        }

        /// <summary>
        /// The positive sequence apparent power (magnitude of the complex power).
        /// </summary>
        public double PositiveSequenceApparentPower
        {
            get
            {
                if (m_useEstimatedValues)
                {
                    return PositiveSequence.Estimate.Magnitude;
                }
                else
                {
                    return PositiveSequence.Measurement.Magnitude;
                }
            }
        }

        /// <summary>
        /// The positive sequence real power (real part of the complex power).
        /// </summary>
        public double PositiveSequenceRealPower
        {
            get
            {
                if (m_useEstimatedValues)
                {
                    return PositiveSequence.Estimate.ComplexPhasor.Real;
                }
                else
                {
                    return PositiveSequence.Measurement.ComplexPhasor.Real;
                }
            }
        }

        /// <summary>
        /// The positive sequence reactive power (imaginary part of the complex power).
        /// </summary>
        public double PositiveSequenceReactivePower
        {
            get
            {
                if (m_useEstimatedValues)
                {
                    return PositiveSequence.Estimate.ComplexPhasor.Imaginary;
                }
                else
                {
                    return PositiveSequence.Measurement.ComplexPhasor.Imaginary;
                }
            }
        }

        /// <summary>
        /// The A phase complex power
        /// </summary>
        public Complex PhaseAComplexPower
        {
            get
            {
                if (m_useEstimatedValues)
                {
                    return PhaseA.Estimate.ComplexPhasor.Imaginary;
                }
                else
                {
                    return PhaseA.Measurement.ComplexPhasor.Imaginary;
                }
            }
        }

        /// <summary>
        /// The A phase apparent power (magnitude of the complex power).
        /// </summary>
        public double PhaseAApparentPower
        {
            get
            {
                if (m_useEstimatedValues)
                {
                    return PhaseA.Estimate.Magnitude;
                }
                else
                {
                    return PhaseA.Measurement.Magnitude;
                }
            }
        }

        /// <summary>
        /// The A phase real power (real part of the complex power).
        /// </summary>
        public double PhaseARealPower
        {
            get
            {
                if (m_useEstimatedValues)
                {
                    return PhaseA.Estimate.ComplexPhasor.Real;
                }
                else
                {
                    return PhaseA.Measurement.ComplexPhasor.Real;
                }
            }
        }

        /// <summary>
        /// The A phase reactive power (imaginary part of the complex power).
        /// </summary>
        public double PhaseAReactivePower
        {
            get
            {
                if (m_useEstimatedValues)
                {
                    return PhaseA.Estimate.ComplexPhasor.Imaginary;
                }
                else
                {
                    return PhaseA.Measurement.ComplexPhasor.Imaginary;
                }
            }
        }

        /// <summary>
        /// The B phase complex power
        /// </summary>
        public Complex PhaseBComplexPower
        {
            get
            {
                if (m_useEstimatedValues)
                {
                    return PhaseB.Estimate.ComplexPhasor;
                }
                else
                {
                    return PhaseB.Measurement.ComplexPhasor;
                }
            }
        }

        /// <summary>
        /// The B phase apparent power (magnitude of the complex power).
        /// </summary>
        public double PhaseBApparentPower
        {
            get
            {
                if (m_useEstimatedValues)
                {
                    return PhaseB.Estimate.Magnitude;
                }
                else
                {
                    return PhaseB.Measurement.Magnitude;
                }
            }
        }

        /// <summary>
        /// The B phase real power (real part of the complex power).
        /// </summary>
        public double PhaseBRealPower
        {
            get
            {
                if (m_useEstimatedValues)
                {
                    return PhaseB.Estimate.ComplexPhasor.Real;
                }
                else
                {
                    return PhaseB.Measurement.ComplexPhasor.Real;
                }
            }
        }

        /// <summary>
        /// The B phase reactive power (imaginary part of the complex power).
        /// </summary>
        public double PhaseBReactivePower
        {
            get
            {
                if (m_useEstimatedValues)
                {
                    return PhaseB.Estimate.ComplexPhasor.Imaginary;
                }
                else
                {
                    return PhaseB.Measurement.ComplexPhasor.Imaginary;
                }
            }
        }

        /// <summary>
        /// The C phase complex power
        /// </summary>
        public Complex PhaseCComplexPower
        {
            get
            {
                if (m_useEstimatedValues)
                {
                    return PhaseC.Estimate.ComplexPhasor;
                }
                else
                {
                    return PhaseC.Measurement.ComplexPhasor;
                }
            }
        }

        /// <summary>
        /// The C phase apparent power (magnitude of the complex power).
        /// </summary>
        public double PhaseCApparentPower
        {
            get
            {
                if (m_useEstimatedValues)
                {
                    return PhaseC.Estimate.Magnitude;
                }
                else
                {
                    return PhaseC.Measurement.Magnitude;
                }
            }
        }

        /// <summary>
        /// The C phase real power (real part of the complex power).
        /// </summary>
        public double PhaseCRealPower
        {
            get
            {
                if (m_useEstimatedValues)
                {
                    return PhaseC.Estimate.ComplexPhasor.Real;
                }
                else
                {
                    return PhaseC.Measurement.ComplexPhasor.Real;
                }
            }
        }

        /// <summary>
        /// The C phase reactive power (imaginary part of the complex power).
        /// </summary>
        public double PhaseCReactivePower
        {
            get 
            {
                if (m_useEstimatedValues)
                {
                    return PhaseC.Estimate.ComplexPhasor.Imaginary;
                }
                else
                {
                    return PhaseC.Measurement.ComplexPhasor.Imaginary;
                }
            }
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// The designated constructor for the <see cref="LinearStateEstimator.Measurements.ComplexPowerGroup"/> class.
        /// </summary>
        /// <param name="voltage">The <see cref="LinearStateEstimator.Measurements.VoltagePhasorGroup"/> to use to calculate the complex power.</param>
        /// <param name="current">The <see cref="LinearStateEstimator.Measurements.CurrentFlowPhasorGroup"/> to use to calculate the complex power.</param>
        public ComplexPowerGroup(VoltagePhasorGroup voltage, CurrentFlowPhasorGroup current)
        {
            m_voltage = voltage;
            m_current = current;
            PositiveSequence.Measurement.BaseKV = m_voltage.PositiveSequence.Measurement.BaseKV;
            PositiveSequence.Estimate.BaseKV = m_voltage.PositiveSequence.Estimate.BaseKV;

            PhaseA.Measurement.BaseKV = m_voltage.PhaseA.Measurement.BaseKV;
            PhaseA.Estimate.BaseKV = m_voltage.PhaseA.Estimate.BaseKV;

            PhaseB.Measurement.BaseKV = m_voltage.PhaseB.Measurement.BaseKV;
            PhaseB.Estimate.BaseKV = m_voltage.PhaseB.Estimate.BaseKV;

            PhaseC.Measurement.BaseKV = m_voltage.PhaseC.Measurement.BaseKV;
            PhaseC.Estimate.BaseKV = m_voltage.PhaseC.Estimate.BaseKV;

            InternalID = m_voltage.InternalID;
            Acronym = "POWER";
            Description = m_voltage.Description + " | " + m_current.Description;
            Name = m_voltage.Name + " | " + m_current.Name;
            Number = m_voltage.Number;
        }

        #endregion

        #region [ Public Methods ]

        /// <summary>
        /// Uses the <see cref="LinearStateEstimator.Measurements.ComplexPowerGroup.Voltage"/> and <see cref="ComplexPowerGroup.Current"/> properties to compute the complex power.
        /// </summary>
        public void Compute()
        {
            PositiveSequence.Measurement.ComplexPhasor = Conjugate(m_voltage.PositiveSequence.Measurement.ComplexPhasor * m_current.PositiveSequence.Measurement.ComplexPhasor);
            PositiveSequence.Estimate.ComplexPhasor = Conjugate(m_voltage.PositiveSequence.Estimate.ComplexPhasor * m_current.PositiveSequence.Estimate.ComplexPhasor);

            PhaseA.Measurement.ComplexPhasor = Conjugate(m_voltage.PhaseA.Measurement.ComplexPhasor * m_current.PhaseA.Measurement.ComplexPhasor);
            PhaseA.Estimate.ComplexPhasor = Conjugate(m_voltage.PhaseA.Estimate.ComplexPhasor * m_current.PhaseA.Estimate.ComplexPhasor);

            PhaseB.Measurement.ComplexPhasor = Conjugate(m_voltage.PhaseB.Measurement.ComplexPhasor * m_current.PhaseB.Measurement.ComplexPhasor);
            PhaseB.Estimate.ComplexPhasor = Conjugate(m_voltage.PhaseB.Estimate.ComplexPhasor * m_current.PhaseB.Estimate.ComplexPhasor);

            PhaseC.Measurement.ComplexPhasor = Conjugate(m_voltage.PhaseC.Measurement.ComplexPhasor * m_current.PhaseC.Measurement.ComplexPhasor);
            PhaseC.Estimate.ComplexPhasor = Conjugate(m_voltage.PhaseC.Estimate.ComplexPhasor * m_current.PhaseC.Estimate.ComplexPhasor);
        }

        #endregion

        #region [ Private Methods ]

        /// <summary>
        /// Computes the complex conjugate of the complex number.
        /// </summary>
        /// <param name="complex">A complex number.</param>
        /// <returns>The complex conjugate of the input.</returns>
        private Complex Conjugate(Complex complex)
        {
            return new Complex(complex.Real, -complex.Imaginary);
        }

        #endregion
    }
}
