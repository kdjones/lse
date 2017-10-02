//******************************************************************************************************
//  ShuntDeviceSusceptanceMatrix.cs
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
//       Created original algorithms in Matlab
//  05/16/2014 - Kevin D. Jones
//       Added System.Numerics for complex numbers
//  05/17/2014 - Kevin D. Jones
//       Added Math.NET for complex matrices
//  06/07/2014 - Kevin D .Jones
//       Added Math.NET Matlab IO extensions for exporting matrices to Matlab format
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
    /// A matrix which represents the relationship between the current injection measurments and the observed busses via the impedance of the shunt devices the current injections originate from.
    /// </summary>
    public class ShuntDeviceSusceptanceMatrix
    {
        #region [ Private Members ]

        private DenseMatrix m_Ysh = DenseMatrix.OfArray(new Complex[1, 1]);
        private bool m_matrixIsValid;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// The shunt device susceptance matrix
        /// </summary>
        public DenseMatrix Matrix
        {
            get
            {
                return m_Ysh;
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
        /// The designated constructor for the <see cref="LinearStateEstimator.Matrices.ShuntDeviceSusceptanceMatrix"/> class.
        /// </summary>
        /// <param name="network">The virtualized <see cref="LinearStateEstimator.Networks.Network"/> model.</param>
        public ShuntDeviceSusceptanceMatrix(Network network)
        {
            BuildMatrices(network);
        }

        #endregion

        #region [ Private Methods ]

        /// <summary>
        /// Method that decides whether to build the <see cref="LinearStateEstimator.Networks.PhaseSelection.PositiveSequence"/> matrices or
        /// the <see cref="PhaseSelection.ThreePhase"/> matrices;
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
        /// Constructs the <see cref="LinearStateEstimator.Networks.PhaseSelection.PositiveSequence"/> version of the <see cref="LinearStateEstimator.Matrices.ShuntDeviceSusceptanceMatrix"/>.
        /// </summary>
        /// <param name="network">The virtualized <see cref="LinearStateEstimator.Networks.Network"/> model.</param>
        private void BuildPositiveSequenceMatrix(Network network)
        {
            List<ObservedBus> observedBusses = network.Model.ObservedBusses;
            List<CurrentInjectionPhasorGroup> measuredCurrents = network.Model.ActiveCurrentInjections;

            if (measuredCurrents.Count > 0 && observedBusses.Count > 0)
            {
                m_matrixIsValid = true;

                m_Ysh = DenseMatrix.OfArray(new Complex[measuredCurrents.Count, observedBusses.Count]);

                foreach (CurrentInjectionPhasorGroup measuredCurrent in measuredCurrents)
                {
                    foreach (ObservedBus observedBus in observedBusses)
                    {
                        if (observedBus.Nodes.Contains(measuredCurrent.MeasuredConnectedNode))
                        {
                            if (measuredCurrent.MeasuredBranch is ShuntCompensator)
                            {
                                if (measuredCurrent.MeasurementDirectionConvention == CurrentInjectionDirectionConvention.IntoTheShunt)
                                {
                                    m_Ysh[measuredCurrents.IndexOf(measuredCurrent), observedBusses.IndexOf(observedBus)] =
                                         (measuredCurrent.MeasuredBranch as ShuntCompensator).PositiveSequenceShuntSusceptance;
                                }
                                else if (measuredCurrent.MeasurementDirectionConvention == CurrentInjectionDirectionConvention.OutOfTheShunt)
                                {
                                    m_Ysh[measuredCurrents.IndexOf(measuredCurrent), observedBusses.IndexOf(observedBus)] =
                                         -(measuredCurrent.MeasuredBranch as ShuntCompensator).PositiveSequenceShuntSusceptance;
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

        /// <summary>
        /// Constructs the <see cref="LinearStateEstimator.Networks.PhaseSelection.ThreePhase"/> version of the <see cref="LinearStateEstimator.Matrices.ShuntDeviceSusceptanceMatrix"/>.
        /// </summary>
        /// <param name="network">The virtualized <see cref="LinearStateEstimator.Networks.Network"/> model.</param>
        private void BuildThreePhaseMatrix(Network network)
        {
            List<ObservedBus> observedBusses = network.Model.ObservedBusses;
            List<CurrentInjectionPhasorGroup> measuredCurrents = network.Model.ActiveCurrentInjections;

            if (measuredCurrents.Count > 0 && observedBusses.Count > 0)
            {
                m_matrixIsValid = true;

                m_Ysh = DenseMatrix.OfArray(new Complex[3 * measuredCurrents.Count, 3 * observedBusses.Count]);

                foreach (CurrentInjectionPhasorGroup measuredCurrent in measuredCurrents)
                {
                    foreach (ObservedBus observedBus in observedBusses)
                    {
                        if (observedBus.Nodes.Contains(measuredCurrent.MeasuredConnectedNode))
                        {
                            if (measuredCurrent.MeasuredBranch is ShuntCompensator)
                            {
                                DenseMatrix susceptance = (measuredCurrent.MeasuredBranch as ShuntCompensator).ThreePhaseShuntSusceptance;
                                for (int row = 0; row < 3; row++)
                                {
                                    for (int col = 0; col < 3; col++)
                                    {
                                        if (measuredCurrent.MeasurementDirectionConvention == CurrentInjectionDirectionConvention.IntoTheShunt)
                                        {
                                            m_Ysh[3 * measuredCurrents.IndexOf(measuredCurrent) + row, 3 * observedBusses.IndexOf(observedBus) + col] = susceptance[row, col];
                                        }
                                        else if (measuredCurrent.MeasurementDirectionConvention == CurrentInjectionDirectionConvention.OutOfTheShunt)
                                        {
                                            m_Ysh[3 * measuredCurrents.IndexOf(measuredCurrent) + row, 3 * observedBusses.IndexOf(observedBus) + col] = -susceptance[row, col];
                                        }
                                    }
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
        /// Converts the <see cref="LinearStateEstimator.Matrices.ShuntDeviceSusceptanceMatrix"/> into a string in CSV format.
        /// </summary>
        /// <returns>A CSV formatted string representation of the <see cref="LinearStateEstimator.Matrices.ShuntDeviceSusceptanceMatrix"/>.</returns>
        public string ToCsvString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < m_Ysh.RowCount; i++)
            {
                string csvRow = "";
                for (int j = 0; j < m_Ysh.ColumnCount; j++)
                {
                    if (j < m_Ysh.ColumnCount - 1)
                    {
                        csvRow += m_Ysh[i, j].Imaginary.ToString() + ",";
                    }
                    else
                    {
                        csvRow += m_Ysh[i, j].Imaginary.ToString();
                    }
                }
                stringBuilder.AppendFormat(csvRow + "{0}", Environment.NewLine);
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Method to print the <see cref="LinearStateEstimator.Matrices.ShuntDeviceSusceptanceMatrix"/> to a CSV file named 'Ys Matrix yyyy-MM-dd  hh-mm-ss.csv' where the date and time are in UTC time.
        /// </summary>
        /// <param name="pathName">The directory name (not including the file name) to the destination for the file to be written to.</param>
        public void WriteToCsvFile(string pathName)
        {
            using (StreamWriter outfile = new StreamWriter(pathName + @"\Ysh Matrix " + String.Format("{0:yyyy-MM-dd  hh-mm-ss}", DateTime.UtcNow) + ".csv"))
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
        //    using (MatlabMatrixWriter outfile = new MatlabMatrixWriter(pathName + @"\Ysh Matrix " + String.Format("{0:yyyy-MM-dd  hh-mm-ss}", DateTime.UtcNow) + ".mat"))
        //    {
        //        outfile.WriteMatrix<Complex>(m_Ysh, "Ysh");
        //    }
        //}

        #endregion
    }
}
