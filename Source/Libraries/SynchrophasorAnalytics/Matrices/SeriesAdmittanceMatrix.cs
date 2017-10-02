//******************************************************************************************************
//  SeriesAdmittanceMatrix.cs
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
//       Removed unused methods from the class.
//       Added IsValid property to indicate when the matrix should not be used for computation.
//       Removed XML markup for class header to prevent misunderstanding since things have changed.
//  05/16/2014 - Kevin D. Jones
//       Added System.Numerics for complex numbers
//  05/17/2014 - Kevin D. Jones
//       Added Math.NET for complex matrices
//  06/07/2014 - Kevin D .Jones
//       Added Math.NET Matlab IO extensions for exporting matrices to Matlab format
//  06/24/2014 - Kevin D. Jones
//       Replaced 'ActiveCurrentPhasors' with 'IncludedCurrentPhasors'
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
using MathNet.Numerics.Data.Matlab;
using SynchrophasorAnalytics.Networks;
using SynchrophasorAnalytics.Modeling;
using SynchrophasorAnalytics.Measurements;

namespace SynchrophasorAnalytics.Matrices
{
    /// <summary>
    /// A matrix which represents the relationship between the current flow measurements and the impedance of the series branches that they measure.
    /// </summary>
    public class SeriesAdmittanceMatrix
    {
        #region [ Private Members ]

        private DenseMatrix m_Y = DenseMatrix.OfArray(new Complex[1, 1]);
        private bool m_matrixIsValid;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// The series admittance matrix
        /// </summary>
        public DenseMatrix Matrix
        {
            get 
            { 
                return m_Y; 
            }
        }

        /// <summary>
        /// A flag which represents whether the matrix representation will be valid based on the number of available measurements.
        /// </summary>
        public bool IsValid
        {
            get
            {
                return m_matrixIsValid;
            }
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// The designated constructor for the <see cref="LinearStateEstimator.Matrices.SeriesAdmittanceMatrix"/> class.
        /// </summary>
        /// <param name="network">The virtualized <see cref="LinearStateEstimator.Networks.Network"/> model.</param>
        public SeriesAdmittanceMatrix(Network network)
        {
            BuildMatrices(network);
        }

        #endregion

        #region [ Private Methods ]

        /// <summary>
        /// Method that decides whether to build the <see cref="LinearStateEstimator.Networks.PhaseSelection.PositiveSequence"/> matrices or
        /// the <see cref="LinearStateEstimator.Networks.PhaseSelection.ThreePhase"/> matrices;
        /// </summary>
        /// <param name="network">The virtualized <see cref="LinearStateEstimator.Networks.Network"/>.</param>
        private void BuildMatrices(Network network)
        {
            if (network.Model.PhaseConfiguration == PhaseSelection.PositiveSequence)
            {
                BuildPositiveSequenceMatrix(network);
            }
            else if (network.Model.PhaseConfiguration == PhaseSelection.ThreePhase)
            {
                BuildThreePhaseMatrix(network);
            }
        }

        /// <summary>
        /// Constructs the <see cref="LinearStateEstimator.Networks.PhaseSelection.PositiveSequence"/> version of the <see cref="LinearStateEstimator.Matrices.SeriesAdmittanceMatrix"/>.
        /// </summary>
        /// <param name="network">The virtualized <see cref="LinearStateEstimator.Networks.Network"/> model.</param>
        private void BuildPositiveSequenceMatrix(Network network)
        {
            //List<CurrentFlowPhasorGroup> measuredCurrents = network.Model.ActiveCurrentPhasors;
            List<CurrentFlowPhasorGroup> measuredCurrents = network.Model.IncludedCurrentFlows;
            // Create an empty series admittance matrix with rows and columns equal in number to the number of current phasors in the network.

            if (measuredCurrents.Count > 0)
            {
                m_matrixIsValid = true;

                // Instantiate a properly sized matrix
                m_Y = DenseMatrix.OfArray(new Complex[measuredCurrents.Count, measuredCurrents.Count]);

                foreach (CurrentFlowPhasorGroup measuredCurrent in measuredCurrents)
                {
                    // To reduce the operatiosn in half for setting the matrix element index of the measured current, just find its index once.
                    int indexOfMeasuredCurrent = measuredCurrents.IndexOf(measuredCurrent);

                    // The matrix element Z, the number of the current phasor, should be the 
                    // positive sequence admittance of the branch measured by measurement Z
                    if (measuredCurrent.MeasuredBranch is Transformer)
                    {
                        Transformer transformer = measuredCurrent.MeasuredBranch as Transformer;
                        if (measuredCurrent.InternalID == transformer.FromNodeCurrentPhasorGroupID)
                        {
                            m_Y[indexOfMeasuredCurrent, indexOfMeasuredCurrent] = transformer.PositiveSequenceSeriesAdmittance;
                        }
                        else if (measuredCurrent.InternalID == transformer.ToNodeCurrentPhasorGroupID)
                        {
                            m_Y[indexOfMeasuredCurrent, indexOfMeasuredCurrent] = transformer.PositiveSequenceSeriesAdmittance;
                        }
                    }
                    else if (measuredCurrent.MeasuredBranch is TransmissionLine)
                    {
                        if (measuredCurrent.InternalID == (measuredCurrent.MeasuredBranch as TransmissionLine).FromSubstationCurrentID)
                        {
                            m_Y[indexOfMeasuredCurrent, indexOfMeasuredCurrent] =
                                (measuredCurrent.MeasuredBranch as TransmissionLine).FromSideImpedanceToDeepestObservability.PositiveSequenceSeriesAdmittance;
                        }
                        else if (measuredCurrent.InternalID == (measuredCurrent.MeasuredBranch as TransmissionLine).ToSubstationCurrentID)
                        {
                            m_Y[indexOfMeasuredCurrent, indexOfMeasuredCurrent] =
                                (measuredCurrent.MeasuredBranch as TransmissionLine).ToSideImpedanceToDeepestObservability.PositiveSequenceSeriesAdmittance;
                        }
                    }
                }
            }
            else
            {
                m_matrixIsValid = false;
            }
        }

        /// <summary>
        /// Constructs the <see cref="LinearStateEstimator.Networks.PhaseSelection.ThreePhase"/> version of the <see cref="LinearStateEstimator.Matrices.SeriesAdmittanceMatrix"/>.
        /// </summary>
        /// <param name="network">The virtualized <see cref="LinearStateEstimator.Networks.Network"/> model.</param>
        private void BuildThreePhaseMatrix(Network network)
        {
            //List<CurrentFlowPhasorGroup> measuredCurrents = network.Model.ActiveCurrentPhasors;
            List<CurrentFlowPhasorGroup> measuredCurrents = network.Model.IncludedCurrentFlows;

            // Create an empty series admitanc ematrix with rows and columns equal in number to the number of current phasors in the network. The NetworkModel
            // organizes phasors into their A, B, C, Pos Sequence groups so the count must be multiplied by three for three phase matrices.

            if (measuredCurrents.Count > 0)
            {
                m_matrixIsValid = true;

                m_Y = DenseMatrix.OfArray(new Complex[3 * measuredCurrents.Count, 3 * measuredCurrents.Count]);

                foreach (CurrentFlowPhasorGroup measuredCurrent in measuredCurrents)
                {
                    // To reduce the operatiosn in half for setting the matrix element index of the measured current, just find its index once.
                    int indexOfMeasuredCurrent = measuredCurrents.IndexOf(measuredCurrent);

                    if (measuredCurrent.MeasuredBranch is Transformer)
                    {
                        // Get the 3 by 3 three phase admittance matrix for the branch measured by this set of current phasors
                        DenseMatrix seriesAdmittance = (measuredCurrent.MeasuredBranch as Transformer).ThreePhaseSeriesAdmittance;

                        // The block matrix element Z, the number of the current phasor, should be the 
                        // three phase admittance of the branch measured by measurement Z
                        for (int row = 0; row < 3; row++)
                        {
                            for (int col = 0; col < 3; col++)
                            {
                                m_Y[3 * indexOfMeasuredCurrent + row, 3 * indexOfMeasuredCurrent + col] = seriesAdmittance[row, col];
                            }
                        }
                    }
                    else if (measuredCurrent.MeasuredBranch is TransmissionLine)
                    {
                        if (measuredCurrent.InternalID == (measuredCurrent.MeasuredBranch as TransmissionLine).FromSubstationCurrentID)
                        {
                            // Get the 3 by 3 three phase admittance matrix for the branch measured by this set of current phasors
                            DenseMatrix seriesAdmittance = (measuredCurrent.MeasuredBranch as TransmissionLine).FromSideImpedanceToDeepestObservability.ThreePhaseSeriesAdmittance;

                            // The block matrix element Z, the number of the current phasor, should be the 
                            // three phase admittance of the branch measured by measurement Zw
                            for (int row = 0; row < 3; row++)
                            {
                                for (int col = 0; col < 3; col++)
                                {
                                    m_Y[3 * indexOfMeasuredCurrent + row, 3 * indexOfMeasuredCurrent + col] = seriesAdmittance[row, col];
                                }
                            }
                        }
                        else if (measuredCurrent.InternalID == (measuredCurrent.MeasuredBranch as TransmissionLine).ToSubstationCurrentID)
                        {
                            // Get the 3 by 3 three phase admittance matrix for the branch measured by this set of current phasors
                            DenseMatrix seriesAdmittance = (measuredCurrent.MeasuredBranch as TransmissionLine).ToSideImpedanceToDeepestObservability.ThreePhaseSeriesAdmittance;

                            // The block matrix element Z, the number of the current phasor, should be the 
                            // three phase admittance of the branch measured by measurement Zw
                            for (int row = 0; row < 3; row++)
                            {
                                for (int col = 0; col < 3; col++)
                                {
                                    m_Y[3 * indexOfMeasuredCurrent + row, 3 * indexOfMeasuredCurrent + col] = seriesAdmittance[row, col];
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                m_matrixIsValid = false;
            }
        }

        #endregion

        #region [ Public Methods ]

        /// <summary>
        /// Converts the <see cref="LinearStateEstimator.Matrices.SeriesAdmittanceMatrix"/> into a string in CSV format.
        /// </summary>
        /// <returns>A CSV formatted string representation of the <see cref="LinearStateEstimator.Matrices.SeriesAdmittanceMatrix"/>.</returns>
        public string ToCsvString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < m_Y.RowCount; i++)
            {
                string csvRow = "";
                for (int j = 0; j < m_Y.ColumnCount; j++)
                {
                    if (j < m_Y.ColumnCount - 1)
                    {
                        csvRow += m_Y[i, j].Real.ToString() + ", " + m_Y[i, j].Imaginary.ToString() + ",";
                    }
                    else
                    {
                        csvRow += m_Y[i, j].Real.ToString() + "," + m_Y[i, j].Imaginary.ToString();
                    }
                }
                stringBuilder.AppendFormat(csvRow + "{0}", Environment.NewLine);
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Method to print the <see cref="LinearStateEstimator.Matrices.SeriesAdmittanceMatrix"/> to a CSV file named 'Y Matrix yyyy-MM-dd  hh-mm-ss.csv' where the date and time are in UTC time.
        /// </summary>
        /// <param name="pathName">The directory name (not including the file name) to the destination for the file to be written to.</param>
        public void WriteToCsvFile(string pathName)
        {
            using (StreamWriter outfile = new StreamWriter(pathName + @"\Y Matrix " + String.Format("{0:yyyy-MM-dd  hh-mm-ss}", DateTime.UtcNow) + ".csv"))
            {
                outfile.Write(this.ToCsvString());
            }
        }

        ///// <summary>
        ///// Method to store the current value of the matrix to a Matlab supported format for external processing.
        ///// </summary>
        ///// <param name="pathName">The path name (not including the file name) where the *.mat file should be saved.</param>
        //public void WriteToMatlabFormat(string pathName)
        //{
        //    using (MatlabMatrixWriter outfile = new MatlabMatrixWriter(pathName + @"\Y Matrix " + String.Format("{0:yyyy-MM-dd  hh-mm-ss}", DateTime.UtcNow) + ".mat"))
        //    {
        //        outfile.WriteMatrix<Complex>(m_Y, "Y");
        //    }
        //}

        #endregion
    }
}
