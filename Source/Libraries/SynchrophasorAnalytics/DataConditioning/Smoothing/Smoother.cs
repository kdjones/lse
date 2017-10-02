//******************************************************************************************************
//  Smoother.cs
//
//  Copyright © 2012, Kevin D. Jones.  All Rights Reserved.
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
//  11/01/2012 - Kevin D. Jones
//       Generated original version of source code in Matlab
//  04/05/2014 - Kevin D. Jones
//       Translated and refactored original version or source code into C#
//  05/16/2014 - Kevin D. Jones
//       Added System.Numerics for complex numbers
//
//******************************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using SynchrophasorAnalytics.Measurements;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra.Complex;

namespace SynchrophasorAnalytics.DataConditioning.Smoothing
{
    /// <summary>
    /// An encapsulation of the smoothing algorithm for synchrophasor data.
    /// </summary>
    public class Smoother
    {
        #region [ Private Constants ]

        private const int MAX_BUFFER_SIZE = 3; // Marks the maximum number of measurements to keep in the measurement buffer
        private const int BUFFER_END_POSITION = 0; // Marks the index of the bottom of the measurement buffer
        private const int BUFFER_MIDDLE_POSITION = 1; // Marks the index of the middle of the measurement buffer
        private const int BUFFER_TOP_POSITION = 2; // Marks the index of the top of the measurement buffer

        #endregion

        #region [ Private Members ]

        private DenseMatrix m_p; // Error Covariance Matrix
        private DenseMatrix m_q; // Process Noise Covariance Matrix
        private DenseMatrix m_r; // Measurement Noise Covariance Matrix
        private DenseMatrix m_gamma; // 
        private DenseMatrix m_phi; // State Transition Matrix
        private DenseMatrix m_h; // Observation Matrix
        private DenseMatrix m_defaultK; // Default Steady State Kalman Gain
        private DenseMatrix m_defaultP; // Default Steady State Error Covariance
        private DenseMatrix m_k; // Kalman Gain
        private DenseMatrix m_x; // State Vector
        private DenseMatrix m_z; // Measurement Vector
        private DenseMatrix m_identity; // The identity matrix

        private PhasorEstimate m_output; // The final output asserted by the algorithm based on stability
        private PhasorMeasurement m_observationResidual; // The complex difference between the observed phasor and the prediced phasor
        private PhasorMeasurement m_optimalPredictedEstimate; // The predicted phasor measurement based on all past history
        private PhasorMeasurement m_optimalFilteredEstimate; // The corrected/filtered phasor based on all past history and latest measurement
        private PhasorMeasurement m_optimalSmoothedEstimate; // The corrected/smoothed phasor based on all past history and the three latest measurements

        private bool m_isStable; // Indicates whether the algorithm is stable or unstable based on its ability to accurately predict the next measurement
        private bool m_isResetting; // Indicates when the algorithm is resetting itself to deal with an instability
        private int m_numberOfSuboptimalDataPoints; // The number of sub optimal data points in the current processing window
        private int m_countdownToStabilization; // The number of trustworthy frames of data to elapse before the algorithm is considered stable
        private List<PhasorMeasurement> m_assertedMeasurementBuffer; // A buffer of phasor measurements which have been asserted by the algorithm. 
        private List<PhasorMeasurement> m_rawMeasurementBuffer; // A buffer of phasor measurements unprocessed by the algorithm.
        private List<AssertedMeasurementType> m_assertedMeasurementTypeBuffer; // A buffer of flags which represent which measurements were asserted to the buffer.

        private double m_tolerance; // The tolerance, in per unit, which classifies the observation residual magnitude as suspect or not.
        private double m_numericalToleranceForRepeatedValues; // The tolerance, in per unit, which classifies the observation residual as suspect or not in the presence of repeated values.
        private int m_maxNumberOfSubOptimalDataPoints; // The number of sub optimal data points allowed before resetting the algorihtm

        #endregion

        #region [ Private Properties ]

        /// <summary>
        /// A flag which represents that the residual tolerance has been violated.
        /// </summary>
        private bool ToleranceIsViolated
        {
            get
            {
                if (m_observationResidual.PerUnitMagnitude > m_tolerance)
                {
                    return true;
                }
                else if (m_observationResidual.PerUnitMagnitude < m_numericalToleranceForRepeatedValues)
                {
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// A flag which represents that the algorithm has had too many measurements based soley on predictions to continue to make valid predictions. Divergence expected if no action is taken.
        /// </summary>
        private bool TooMuchConsecutiveBadDataToContinue
        {
            get
            {
                return (m_numberOfSuboptimalDataPoints > m_maxNumberOfSubOptimalDataPoints);
            }
        }

        #endregion

        #region [ Public Properties ]

        /// <summary>
        /// The optimal predicted estimate, x(k + 1), of the algorithm.
        /// </summary>
        public PhasorMeasurement OptimalPredictedEstimate
        {
            get
            {
                return m_optimalPredictedEstimate;
            }
        }

        /// <summary>
        /// The optimal filtered estimate, x(k), of the algorith
        /// </summary>
        public PhasorMeasurement OptimalFilteredEstimate
        {
            get
            {
                return m_optimalFilteredEstimate;
            }
        }

        /// <summary>
        /// The optimal smoothed estimate, x(k - 1), of the algorithm.
        /// </summary>
        public PhasorMeasurement OptimalSmoothedEstimate
        {
            get
            {
                return m_optimalSmoothedEstimate;
            }
        }

        /// <summary>
        /// The observation residual of the most recent measurement.
        /// </summary>
        public PhasorMeasurement ObservationResidual
        {
            get
            {
                return m_observationResidual;
            }
        }

        /// <summary>
        /// The most recent measurement to be received by the algorithm.
        /// </summary>
        public PhasorMeasurement RawMeasurement
        {
            get
            {
                return m_rawMeasurementBuffer[BUFFER_END_POSITION];
            }
        }

        /// <summary>
        /// The output phasor of the algorithm.
        /// </summary>
        public PhasorEstimate Output
        {
            get
            {

                if (m_isStable)
                {
                    m_output.PerUnitComplexPhasor = m_optimalSmoothedEstimate.PerUnitComplexPhasor;
                }
                else
                {
                    m_output.PerUnitComplexPhasor = m_rawMeasurementBuffer[BUFFER_END_POSITION].PerUnitComplexPhasor;
                }

                return m_output;
            }
        }

        /// <summary>
        /// A flag which represents the stability of the algorithm.
        /// </summary>
        public bool IsStable
        {
            get
            {
                return m_isStable;
            }
        }

        /// <summary>
        /// A flag which represents if the algorithm is resetting itself.
        /// </summary>
        public bool IsResetting
        {
            get
            {
                return m_isResetting;
            }
        }

        /// <summary>
        /// The number of good measurements the algorithm must receive before returning to a stable condition.
        /// </summary>
        public int StabilizationCount
        {
            get
            {
                return m_countdownToStabilization;
            }
        }

        #endregion

        #region [ Constructor ]

        /// <summary>
        /// The default constructor for the <see cref="LinearStateEstimator.DataConditioning.Smoothing.Smoother"/> class. Uses a default tolerance of 0.05 p.u.
        /// </summary>
        /// <param name="destinationPhasor">The <see cref="LinearStateEstimator.Measurements.PhasorEstimate"/> where the results will be pushed to.</param>
        public Smoother(PhasorEstimate destinationPhasor)
            : this(destinationPhasor, 0.05, 0.000001, MAX_BUFFER_SIZE)
        {
        }

        /// <summary>
        /// A constructor method for the <see cref="LinearStateEstimator.DataConditioning.Smoothing.Smoother"/> class. 
        /// </summary>
        /// <param name="destinationPhasor">The <see cref="LinearStateEstimator.Measurements.PhasorEstimate"/> where the results will be pushed to.</param>
        /// <param name="tolerance">The tolerance, in per unit, for the algorithm.</param>
        /// <param name="numericalTolerance">The numerical tolerance for repeated values.</param>
        /// <param name="maxNumberOfBadDataBeforeReset">The number of sub optimal predictions that can be made before the algorithm will reset itself.</param>
        public Smoother(PhasorEstimate destinationPhasor, double tolerance, double numericalTolerance, int maxNumberOfBadDataBeforeReset)
        {
            m_output = destinationPhasor;
            m_tolerance = tolerance;
            m_numericalToleranceForRepeatedValues = numericalTolerance;
            m_maxNumberOfSubOptimalDataPoints = maxNumberOfBadDataBeforeReset;

            // Allow the Kalman filter to begin tracking before imposing data quality restrictions
            m_isStable = false;
            m_countdownToStabilization = 10;

            Initialize();
        }

        #endregion

        #region [ Public Methods ]

        /// <summary>
        /// The main function of the object. Accepts the latest phasor measurement and smooths it. The side effect is that the corresponding
        /// output of the <see cref="Smoother"/> will appear two frames later (smoothed of course).
        /// </summary>
        /// <param name="phasorMeasurement">The incoming <see cref="LinearStateEstimator.Measurements.PhasorMeasurement"/> which holds the latest raw data.</param>
        public void Smooth(PhasorMeasurement phasorMeasurement)
        {
            // Time Update - Make Prediction
            ProjectSystemState();
            ProjectErrorCovariance();

            // Measurement Update - Get actual measurement
            UpdateKalmanGain();
            GetMeasurement(phasorMeasurement);

            // Compare
            CalculateObservationResidual();

            if (ToleranceIsViolated)
            {
                // Increment the number of sub optimal data points to reflect the number of times the tolerance was violated.
                m_numberOfSuboptimalDataPoints++;

                // If the magnitude of the observation residual is above a predefined threshold
                if (m_isStable)
                {
                    // and if the algorithm has stabilized, utilize the optimal predicted estimate to replace the bad measurement.
                    ReplaceLatestMeasurement(m_optimalPredictedEstimate);
                }
            }
            else
            {
                ResetStabilityCountWithStableParameters();
            }

            UpdateSystemState();
            UpdateErrorCovariance();

            if (TooMuchConsecutiveBadDataToContinue)
            {
                Reset();
            }
        }

        #endregion

        #region [ Private Methods ]

        #region [ Initialization Methods ]

        private void Initialize()
        {
            InitializeMatrices();
            InitializePhasorValues();
        }

        #region [ Matrix Initialization ]

        private void InitializeMatrices()
        {
            InitializeStateVector();

            InitializeMeasurementVector();

            InitializeStateTransitionMatrix();

            InitializeObservationMatrix();

            InitializeDefaultErrorCovarianceMatrix();
            InitializeErrorCovarianceMatrix();

            InitializeProcessNoiseCovarianceMatrix();

            InitializeMeasurementNoiseCovarianceMatrix();

            InitializeDefaultKalmanGain();
            InitializeKalmanGain();

            InitializeDisturbanceTransitionMatrix();

            InitializeIdentityMatrix();
        }

        private void InitializeStateVector()
        {
            m_x = DenseMatrix.OfArray(new Complex[3, 1]);
        }

        private void InitializeMeasurementVector()
        {
            m_z = DenseMatrix.OfArray(new Complex[1, 1]);
        }

        private Complex FillWithValue(int real, int imaginary)
        {
            return new Complex(real, imaginary);
        }

        private void InitializeStateTransitionMatrix()
        {
            m_phi = DenseMatrix.OfArray(new Complex[3, 3]);
            m_phi[0, 0] = new Complex(3, 0);
            m_phi[0, 1] = new Complex(-3, 0);
            m_phi[0, 2] = new Complex(1, 0);

            m_phi[1, 0] = new Complex(1, 0);
            m_phi[1, 1] = new Complex(0, 0);
            m_phi[1, 2] = new Complex(0, 0);

            m_phi[2, 0] = new Complex(0, 0);
            m_phi[2, 1] = new Complex(1, 0);
            m_phi[2, 2] = new Complex(0, 0);
        }

        private void InitializeObservationMatrix()
        {
            m_h = DenseMatrix.OfArray(new Complex[1, 3]);
            m_h[0, 0] = new Complex(1, 0);
            m_h[0, 1] = new Complex(0, 0);
            m_h[0, 2] = new Complex(0, 0);
        }

        private void InitializeDefaultErrorCovarianceMatrix()
        {
            m_defaultP = DenseMatrix.OfArray(new Complex[3, 3]);
            m_defaultP[0, 0] = new Complex(0.128989470429623, 0);
            m_defaultP[0, 1] = new Complex(0.0478039583406026, 0);
            m_defaultP[0, 2] = new Complex(0.00389973786408199, 0);

            m_defaultP[1, 0] = new Complex(0.0478039583406026, 0);
            m_defaultP[1, 1] = new Complex(0.0192805210373786, 0);
            m_defaultP[1, 2] = new Complex(0.00343939423560925, 0);

            m_defaultP[2, 0] = new Complex(0.00389973786408199, 0);
            m_defaultP[2, 1] = new Complex(0.00343939423560925, 0);
            m_defaultP[2, 2] = new Complex(0.00283885516178131, 0);
        }

        private void InitializeErrorCovarianceMatrix()
        {
            m_p = m_defaultP;
        }

        private void InitializeProcessNoiseCovarianceMatrix()
        {
            m_q = DenseMatrix.OfArray(new Complex[1, 1]);
            m_q[0, 0] = new Complex(0.01, 0);
        }

        private void InitializeMeasurementNoiseCovarianceMatrix()
        {
            m_r = DenseMatrix.OfArray(new Complex[1, 1]);
            m_r[0, 0] = new Complex(0.01, 0);
        }

        private void InitializeDefaultKalmanGain()
        {
            m_defaultK = DenseMatrix.OfArray(new Complex[3, 1]);
            m_defaultK[0, 0] = new Complex(0.928052103737862, 0);
            m_defaultK[1, 0] = new Complex(0.343939423560925, 0);
            m_defaultK[2, 0] = new Complex(0.0280577935294503, 0);
        }

        private void InitializeKalmanGain()
        {
            m_k = m_defaultK;
        }

        private void InitializeDisturbanceTransitionMatrix()
        {
            m_gamma = DenseMatrix.OfArray(new Complex[3, 1]);
            m_gamma[0, 0] = new Complex(1, 0);
            m_gamma[1, 0] = new Complex(0, 0);
            m_gamma[2, 0] = new Complex(0, 0);
        }

        private void InitializeIdentityMatrix()
        {
            m_identity = DenseMatrix.OfArray(new Complex[3, 3]);
            m_identity[0, 0] = new Complex(1, 0);
            m_identity[0, 1] = new Complex(0, 0);
            m_identity[0, 2] = new Complex(0, 0);

            m_identity[1, 0] = new Complex(0, 0);
            m_identity[1, 1] = new Complex(1, 0);
            m_identity[1, 2] = new Complex(0, 0);

            m_identity[2, 0] = new Complex(0, 0);
            m_identity[2, 1] = new Complex(0, 0);
            m_identity[2, 2] = new Complex(1, 0);
        }

        #endregion

        #region [ Phasor Initialization ]

        private void InitializePhasorValues()
        {
            InitializeOptimalPredictedEstimate();
            InitializeOptimalFilteredEstimate();
            InitializeOptimalSmoothedEstimate();
            InitializeObservationResidual();

            InitializeMeasurementBuffer();
        }

        private void InitializeOptimalPredictedEstimate()
        {
            m_optimalPredictedEstimate = new PhasorMeasurement(m_output.MagnitudeKey, m_output.AngleKey, m_output.Type, m_output.BaseKV.DeepCopy());
        }

        private void InitializeOptimalFilteredEstimate()
        {
            m_optimalFilteredEstimate = new PhasorMeasurement(m_output.MagnitudeKey, m_output.AngleKey, m_output.Type, m_output.BaseKV.DeepCopy());
        }

        private void InitializeOptimalSmoothedEstimate()
        {
            m_optimalSmoothedEstimate = new PhasorMeasurement(m_output.MagnitudeKey, m_output.AngleKey, m_output.Type, m_output.BaseKV.DeepCopy());
        }

        private void InitializeObservationResidual()
        {
            m_observationResidual = new PhasorMeasurement(m_output.MagnitudeKey, m_output.AngleKey, m_output.Type, m_output.BaseKV.DeepCopy());
        }

        #endregion

        #region [ Measurement Buffer Initialization ]

        private void InitializeMeasurementBuffer()
        {
            InitializeRawMeasurementBuffer();
            InitializeAssertedMeasurementBuffer();
            InitializeAssertedMeaseurementTypeBuffer();
        }

        private void InitializeRawMeasurementBuffer()
        {
            m_rawMeasurementBuffer = new List<PhasorMeasurement>();
            m_rawMeasurementBuffer.Add(new PhasorMeasurement());
            m_rawMeasurementBuffer.Add(new PhasorMeasurement());
            m_rawMeasurementBuffer.Add(new PhasorMeasurement());
        }

        private void InitializeAssertedMeasurementBuffer()
        {
            m_assertedMeasurementBuffer = new List<PhasorMeasurement>();
            m_assertedMeasurementBuffer.Add(new PhasorMeasurement());
            m_assertedMeasurementBuffer.Add(new PhasorMeasurement());
            m_assertedMeasurementBuffer.Add(new PhasorMeasurement());
        }

        private void InitializeAssertedMeaseurementTypeBuffer()
        {
            m_assertedMeasurementTypeBuffer = new List<AssertedMeasurementType>();
            m_assertedMeasurementTypeBuffer.Add(AssertedMeasurementType.RawMeasurement);
            m_assertedMeasurementTypeBuffer.Add(AssertedMeasurementType.RawMeasurement);
            m_assertedMeasurementTypeBuffer.Add(AssertedMeasurementType.RawMeasurement);
        }

        #endregion

        #endregion

        #region [ Smoothing Methods ]

        private void ProjectSystemState()
        {
            m_x = m_phi * m_x;
            m_optimalPredictedEstimate.PerUnitComplexPhasor = m_x[0, 0];
        }

        private void ProjectErrorCovariance()
        {
            m_p = (m_phi * m_p * m_phi.Transpose() + m_phi * m_gamma * m_q * m_gamma.Transpose() * m_phi.Transpose()) as DenseMatrix;
        }

        private void UpdateKalmanGain()
        {
            m_k = (m_p * m_h.Transpose() * (m_h * m_p * m_h.Transpose() + m_r).Inverse()) as DenseMatrix;
        }

        private void GetMeasurement(PhasorMeasurement phasorMeasurement)
        {
            if (m_assertedMeasurementBuffer.Count < MAX_BUFFER_SIZE)
            {
                // If the measurement buffer is filling up, contine to add measurements.
                AddNewMeasurementToBuffer(phasorMeasurement);
            }
            else if (m_assertedMeasurementBuffer.Count == MAX_BUFFER_SIZE)
            {
                // If the measurement buffer is full, remove older (FIFO).
                RemoveOldestMeasurementFromBuffer();
                AddNewMeasurementToBuffer(phasorMeasurement);

                CheckAlgorithmStability();
            }
            else
            {
                throw new Exception("Measurement buffer size exceeded.");
            }
        }

        private void AddNewMeasurementToBuffer(PhasorMeasurement phasorMeasurement)
        {
            m_rawMeasurementBuffer.Add(phasorMeasurement.DeepCopy());
            m_assertedMeasurementBuffer.Add(phasorMeasurement.DeepCopy());
            m_z[0, 0] = phasorMeasurement.PerUnitComplexPhasor;

            AssertRawMeasurement();
        }

        private void AssertRawMeasurement()
        {
            if (m_assertedMeasurementTypeBuffer.Count < MAX_BUFFER_SIZE)
            {
                // If the measurement buffer is filling up, continue to add measurements.
                m_assertedMeasurementTypeBuffer.Add(AssertedMeasurementType.RawMeasurement);
            }
            else if (m_assertedMeasurementTypeBuffer.Count == MAX_BUFFER_SIZE)
            {
                // If the measurement buffer is full, remove oldest (FIFO).
                m_assertedMeasurementTypeBuffer.RemoveAt(BUFFER_END_POSITION);
                m_assertedMeasurementTypeBuffer.Add(AssertedMeasurementType.RawMeasurement);
            }
            else
            {
                throw new Exception("Asserted-type buffer size exceed.");
            }
        }

        private void RemoveOldestMeasurementFromBuffer()
        {
            m_rawMeasurementBuffer.RemoveAt(BUFFER_END_POSITION);
            m_assertedMeasurementBuffer.RemoveAt(BUFFER_END_POSITION);
        }

        private void CheckAlgorithmStability()
        {
            if (m_countdownToStabilization > 0)
            {
                m_countdownToStabilization--;
                m_isStable = false;
                m_isResetting = true;
            }
            else
            {
                m_isStable = true;
                m_isResetting = false;
            }
        }

        private void CalculateObservationResidual()
        {
            m_observationResidual.PerUnitComplexPhasor = m_assertedMeasurementBuffer[BUFFER_TOP_POSITION].PerUnitComplexPhasor - m_optimalPredictedEstimate.PerUnitComplexPhasor;
        }

        private void ReplaceLatestMeasurement(PhasorMeasurement phasorMeasurement)
        {
            if (m_assertedMeasurementBuffer.Count < MAX_BUFFER_SIZE)
            {
                // If the measurement buffer is filling up, overwrite the measurement in the highest position
                m_assertedMeasurementBuffer[m_assertedMeasurementBuffer.Count - 1] = phasorMeasurement.DeepCopy();
                m_z[0, 0] = phasorMeasurement.PerUnitComplexPhasor;
                ReplaceLatestAssertment();
            }
            else if (m_assertedMeasurementBuffer.Count == MAX_BUFFER_SIZE)
            {
                // If the measurement buffer is full, overwrite the measurement in the highest position
                m_assertedMeasurementBuffer[BUFFER_TOP_POSITION] = phasorMeasurement.DeepCopy();
                m_z[0, 0] = phasorMeasurement.PerUnitComplexPhasor;
                ReplaceLatestAssertment();
            }
            else
            {
                throw new Exception("Measurement buffer size exceeded.");
            }
        }

        private void ReplaceLatestAssertment()
        {
            if (m_assertedMeasurementTypeBuffer.Count < MAX_BUFFER_SIZE)
            {
                // If the measurement buffer is filling up, overwrite the measurement in the highest position
                m_assertedMeasurementTypeBuffer[m_assertedMeasurementBuffer.Count - 1] = AssertedMeasurementType.OptimalPredictedEstimate;
            }
            else if (m_assertedMeasurementBuffer.Count == MAX_BUFFER_SIZE)
            {
                // If the measurement buffer is full, overwrite the measurement in the highest position
                m_assertedMeasurementTypeBuffer[BUFFER_TOP_POSITION] = AssertedMeasurementType.OptimalPredictedEstimate;
            }
            else
            {
                throw new Exception("Asserted-type buffer size exceeded.");
            }
        }

        private void ResetStabilityCountWithStableParameters()
        {
            // Reset the number of suboptimal data points to zero since a good data point arrived.
            m_numberOfSuboptimalDataPoints = 0;

            // Decrease the stabilization counter until it reaches zero.
            m_countdownToStabilization--;

            // When it reaches zero, the algorithm has stabilized.
            if (m_countdownToStabilization <= 0)
            {
                m_countdownToStabilization = 0;
                m_isStable = true;
            }
        }

        private void UpdateSystemState()
        {
            m_x = m_x + m_k * (m_z - m_h * m_x);
            m_optimalFilteredEstimate.PerUnitComplexPhasor = m_x[0, 0];
            m_optimalSmoothedEstimate.PerUnitComplexPhasor = m_x[2, 0];
        }

        private void UpdateErrorCovariance()
        {
            m_p = (m_identity - m_k * m_h) * m_p;
        }

        private void Reset()
        {
            ResetKalmanGainAndErrorCovariance();

            ReinstantiateStateVectorFromMeasurementBuffer();

            ResetStabilityCount();
        }

        private void ResetKalmanGainAndErrorCovariance()
        {
            // Use steady state values of K and P to speed up the algorithm reset
            m_k = m_defaultK;
            m_p = m_defaultP;
        }

        private void ReinstantiateStateVectorFromMeasurementBuffer()
        {
            // Reinstantiate the state vector using the measurement buffer as a best guess
            m_x[0, 0] = m_assertedMeasurementBuffer[BUFFER_TOP_POSITION].PerUnitComplexPhasor;
            m_x[1, 0] = m_assertedMeasurementBuffer[BUFFER_MIDDLE_POSITION].PerUnitComplexPhasor;
            m_x[2, 0] = m_assertedMeasurementBuffer[BUFFER_END_POSITION].PerUnitComplexPhasor;
        }

        private void ResetStabilityCount()
        {
            // Raise the Algorithm Resetting flag
            m_isResetting = true;

            // Lower the stability flag
            m_isStable = false;

            // Tell the algorithm how long to wait to begin processing data again.
            m_countdownToStabilization = 5;
        }

        #endregion

        #endregion

    }
}
