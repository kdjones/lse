//******************************************************************************************************
//  SystemMatrix.cs
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
//  12/30/2009 - Kevin D. Jones
//       Created original algorithms in Matlab
//  07/20/2011 - Kevin D. Jones
//       Generated original version of source code.
//  06/01/2013 - Kevin D. Jones
//       Modified to accept new parameters in constructor and use new Network Model
//  05/12/2014 - Kevin D. Jones
//       Used IsValid property to indicate when the matrix should not be used for computation.
//       Removed XML markup for class header to prevent misunderstanding since things have changed.
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
using System.Threading.Tasks;
using System.IO;
using MathNet.Numerics;

using MathNet.Numerics.LinearAlgebra.Complex;
using SynchrophasorAnalytics.Networks;
using SynchrophasorAnalytics.Modeling;
using SynchrophasorAnalytics.Measurements;
using System.Configuration;

namespace SynchrophasorAnalytics.Matrices
{
    /// <summary>
    /// Represents the complete state matrix defining the linear relationship between the measurement vector and state vector.
    /// </summary>
    public class SystemMatrix
    {
        #region [ Private Members ]

        private CurrentFlowMeasurementBusIncidenceMatrix m_A;
        private VoltageMeasurementBusIncidenceMatrix m_II;
        private SeriesAdmittanceMatrix m_Y;
        private LineShuntSusceptanceMatrix m_Ys;
        private ShuntDeviceSusceptanceMatrix m_Ysh;
        private CovarianceMatrix m_R;
        private DenseMatrix m_B = DenseMatrix.OfArray(new Complex[1, 1]);
        private DenseMatrix m_M = DenseMatrix.OfArray(new Complex[1, 1]);
        private DenseMatrix m_K = DenseMatrix.OfArray(new Complex[1, 1]);
        private DenseMatrix m_ExhaustiveK = DenseMatrix.OfArray(new Complex[1, 1]);
        private bool m_seriesPartitionIsValid;
        private bool m_systemMatrixIsValid;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// The <see cref="LinearStateEstimator.Matrices.CurrentFlowMeasurementBusIncidenceMatrix"/>.
        /// </summary>
        public CurrentFlowMeasurementBusIncidenceMatrix A
        {
            get
            {
                return m_A;
            }
        }

        /// <summary>
        /// The <see cref="LinearStateEstimator.Matrices.VoltageMeasurementBusIncidenceMatrix"/>.
        /// </summary>
        public VoltageMeasurementBusIncidenceMatrix II
        {
            get
            {
                return m_II;
            }
        }

        /// <summary>
        /// The <see cref="LinearStateEstimator.Matrices.SeriesAdmittanceMatrix"/>.
        /// </summary>
        public SeriesAdmittanceMatrix Y
        {
            get
            {
                return m_Y;
            }
        }

        /// <summary>
        /// The <see cref="LinearStateEstimator.Matrices.LineShuntSusceptanceMatrix"/>.
        /// </summary>
        public LineShuntSusceptanceMatrix Ys
        {
            get
            {
                return m_Ys;
            }
        }

        /// <summary>
        /// The <see cref="LinearStateEstimator.Matrices.ShuntDeviceSusceptanceMatrix"/>.
        /// </summary>
        public ShuntDeviceSusceptanceMatrix Ysh
        {
            get
            {
                return m_Ysh;
            }
        }

        /// <summary>
        /// An overdefined system which linearly relates voltage and current phasor measurements to state variables in a power system
        /// </summary>
        public DenseMatrix Matrix
        {
            get 
            { 
                return m_B; 
            }
        }

        /// <summary>
        /// The psuedo inverse of the system matrix which can be used to calculate the system state.
        /// </summary>
        public DenseMatrix PsuedoInverseOfMatrix
        {
            get 
            { 
                return m_M; 
            }
        }

        /// <summary>
        /// The lower partition of the un-inverted system matrix which relates current measurements to state variables in power system.
        /// </summary>
        public DenseMatrix LowerPartitionOfMatrix
        {
            get 
            { 
                return m_K; 
            }
        }

        /// <summary>
        /// A flag which represents whether the matrix representation will be valid based on the validity of its matrix components.
        /// </summary>
        public bool IsValid
        {
            get
            {
                return m_systemMatrixIsValid;
            }
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a new instance of the <see cref="LinearStateEstimator.Matrices.SystemMatrix"/> class.
        /// </summary>
        /// <remarks>
        /// Creates instances of the <see cref="LinearStateEstimator.Matrices.CurrentFlowMeasurementBusIncidenceMatrix"/> class, 
        /// the <see cref="LinearStateEstimator.Matrices.VoltageMeasurementBusIncidenceMatrix"/> class, the 
        /// <see cref="LinearStateEstimator.Matrices.SeriesAdmittanceMatrix"/> class, the <see cref="LinearStateEstimator.Matrices.LineShuntSusceptanceMatrix"/> and the <see cref="LinearStateEstimator.Matrices.ShuntDeviceSusceptanceMatrix"/>.
        /// class then calculates and arranges them in the full matrix. It also calculates the 
        /// psuedo-inverse of the system matrix.
        /// </remarks>
        /// <param name="network">The virtualized <see cref="LinearStateEstimator.Networks.Network"/> model.</param>
        public SystemMatrix(Network network)
        {
            BuildMatrices(network);
        }

        #endregion

        #region [ Private Methods ]

        /// <summary>
        /// Builds the system matrix
        /// </summary>
        /// <param name="network">The virtualized <see cref="LinearStateEstimator.Networks.Network"/> model.</param>
        private void BuildMatrices(Network network)
        {
            BuildSeriesPartitionOfSystemMatrix(network);
            BuildShuntPartitionOfSystemMatrix(network);
            BuildSystemMatrix(network);
            ComputePsuedoInverseOfSystemMatrix();
        }

        /// <summary>
        /// Builds the <see cref="LinearStateEstimator.Matrices.CurrentFlowMeasurementBusIncidenceMatrix"/>, the
        /// <see cref="LinearStateEstimator.Matrices.SeriesAdmittanceMatrix"/>, the <see cref="LinearStateEstimator.Matrices.LineShuntSusceptanceMatrix"/> the <see cref="LinearStateEstimator.Matrices.ShuntDeviceSusceptanceMatrix"/> and
        /// computes the <see cref="LinearStateEstimator.Matrices.SystemMatrix.LowerPartitionOfMatrix"/>.
        /// </summary>
        /// <param name="network">The virtualized <see cref="LinearStateEstimator.Networks.Network"/> model.</param>
        private void BuildSeriesPartitionOfSystemMatrix(Network network)
        {
            // Build the Current Measurement-Bus Incidence Matrix
            m_A = new CurrentFlowMeasurementBusIncidenceMatrix(network);

            // Build the Series Admittance Matrix
            m_Y = new SeriesAdmittanceMatrix(network);

            // Build the Shunt Susceptance Matrix
            m_Ys = new LineShuntSusceptanceMatrix(network);

            if (m_Y.IsValid && m_A.IsValid && m_Ys.IsValid)
            {
                m_seriesPartitionIsValid = true;
                // Compute the series partition of the system matrix
                m_K = m_Y.Matrix * m_A.Matrix + m_Ys.Matrix;
            }
            else
            {
                m_seriesPartitionIsValid = false;
            }
        }
        
        /// <summary>
        /// Builds the <see cref="LinearStateEstimator.Matrices.ShuntDeviceSusceptanceMatrix"/>.
        /// </summary>
        /// <param name="network">The virtualized <see cref="LinearStateEstimator.Networks.Network"/> model.</param>
        private void BuildShuntPartitionOfSystemMatrix(Network network)
        {
            m_Ysh = new ShuntDeviceSusceptanceMatrix(network);
        }

        /// <summary>
        /// Builds the <see cref="LinearStateEstimator.Matrices.VoltageMeasurementBusIncidenceMatrix"/>, the
        /// <see cref="LinearStateEstimator.Matrices.CovarianceMatrix"/> and concatenates the <see cref="LinearStateEstimator.Matrices.SystemMatrix.LowerPartitionOfMatrix"/>
        /// with the <see cref="LinearStateEstimator.Matrices.VoltageMeasurementBusIncidenceMatrix"/>.
        /// </summary>
        /// <param name="network">The virtualized <see cref="Network"/> model.</param>
        private void BuildSystemMatrix(Network network)
        {
            // Build the Voltage Measurement-Bus Incidence Matrix
            m_II = new VoltageMeasurementBusIncidenceMatrix(network);

            // Build the Measurement Covariance Matrix
            m_R = new CovarianceMatrix(network);

            if (m_II.IsValid && m_seriesPartitionIsValid)
            {
                // Build the SystemMatrix
                DenseMatrix upperSystemMatrix = MatrixCalculationExtensions.VerticallyConcatenate(m_II.Matrix, m_K);
                // Build the SystemMatrix
                if (m_Ysh.IsValid)
                {
                    m_B = MatrixCalculationExtensions.VerticallyConcatenate(upperSystemMatrix, m_Ysh.Matrix);
                }
                else
                {
                    m_B = upperSystemMatrix;
                }
                m_systemMatrixIsValid = true;
            }
            else if (m_II.IsValid)
            {
                if (m_Ysh.IsValid)
                {
                    DenseMatrix upperSystemMatrix = MatrixCalculationExtensions.VerticallyConcatenate(m_II.Matrix, m_Ysh.Matrix);
                    m_B = upperSystemMatrix;
                }
                else
                {
                    m_B = m_II.Matrix;
                }
                m_systemMatrixIsValid = true;
            }
            else if (m_Ysh.IsValid)
            {
                m_B = m_Ysh.Matrix;
                m_systemMatrixIsValid = true;
            }
            else
            {
                m_systemMatrixIsValid = false;
            }
        }

        /// <summary>
        /// Computes the psuedo-inverse of the system matrix
        /// </summary>
        private void ComputePsuedoInverseOfSystemMatrix()
        {
            if (m_systemMatrixIsValid)
            {
                DenseMatrix BTranspose = m_B.Transpose() as DenseMatrix;
                m_M = (((BTranspose * m_R.Matrix.Inverse() * m_B).Inverse()) * BTranspose * m_R.Matrix.Inverse()) as DenseMatrix;
            }
        }

        #endregion

        #region [ Public Methods ]

        /// <summary>
        /// Converts the uninverted <see cref="LinearStateEstimator.Matrices.SystemMatrix"/> into a string in CSV format.
        /// </summary>
        /// <returns>A CSV formatted string representation of the <see cref="LinearStateEstimator.Matrices.SystemMatrix"/>.</returns>
        public string ToCsvString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < m_M.RowCount; i++)
            {
                string csvRow = "";
                for (int j = 0; j < m_B.ColumnCount; j++)
                {
                    if (j < m_B.ColumnCount - 1)
                    {
                        csvRow += m_B[i, j].Real.ToString() + ", " + m_B[i, j].Imaginary.ToString() + ",";
                    }
                    else
                    {
                        csvRow += m_B[i, j].Real.ToString() + "," + m_B[i, j].Imaginary.ToString();
                    }
                }
                stringBuilder.AppendFormat(csvRow + "{0}", Environment.NewLine);
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Method to print the <see cref="LinearStateEstimator.Matrices.SystemMatrix"/> to a CSV file named 'System Matrix yyyy-MM-dd  hh-mm-ss.csv' where the date and time are in UTC time.
        /// </summary>
        /// <param name="pathName">The directory name (not including the file name) to the destination for the file to be written to.</param>
        public void WriteToDebugFile(string pathName)
        {
            using (StreamWriter outfile = new StreamWriter(pathName + @"\System Matrix " + String.Format("{0:yyyy-MM-dd  hh-mm-ss}", DateTime.UtcNow) + ".txt"))
            {
                outfile.Write(this.ToCsvString());
            }
        }

        #endregion
    }
}
