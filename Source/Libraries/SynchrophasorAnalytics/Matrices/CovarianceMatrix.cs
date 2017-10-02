//******************************************************************************************************
//  CovarianceMatrix.cs
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
//       Removed unused fields from the class.
//       Added IsValid property to indicate when the matrix should not be used for computation.
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
using SynchrophasorAnalytics.Networks;
using SynchrophasorAnalytics.Modeling;
using SynchrophasorAnalytics.Measurements;
using MathNet.Numerics;

using MathNet.Numerics.LinearAlgebra.Complex;
using MathNet.Numerics.Data.Matlab;

namespace SynchrophasorAnalytics.Matrices
{
    /// <summary>
    /// A matrix representation of the covariances of the measurements in the state estimation problem.
    /// </summary>
    public class CovarianceMatrix
    {
        #region [ Private Members ]

        private DenseMatrix m_covarianceMatrix;
        private bool m_matrixIsValid;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// The actual <see cref="DenseMatrix"/> of the <see cref="LinearStateEstimator.Matrices.CovarianceMatrix"/>
        /// </summary>
        public DenseMatrix Matrix
        {
            get 
            { 
                return m_covarianceMatrix; 
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
        /// The designated constructor for the <see cref="LinearStateEstimator.Matrices.CovarianceMatrix"/> class.
        /// </summary>
        /// <param name="network">The <see cref="LinearStateEstimator.Networks.Network"/> which is desired to represent as matrices.</param>
        public CovarianceMatrix(Network network)
        {
            BuildMatrices(network);
        }


        #endregion

        #region [ Private Methods ]

        /// <summary>
        /// Calls the methods for building the matrix representation depending on the <see cref="LinearStateEstimator.Networks.PhaseSelection"/> of the <see cref="LinearStateEstimator.Networks.Network"/>.
        /// </summary>
        /// <param name="network">The <see cref="LinearStateEstimator.Networks.Network"/> which is desired to represent as matrices.</param>
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
        /// Builds the covariance matrix to give weight to each measurement for the positive sequence formulation.
        /// </summary>
        /// <param name="network">The <see cref="LinearStateEstimator.Networks.Network"/> which is desired to represent as matrices.</param>
        private void BuildPositiveSequenceMatrix(Network network)
        {
            // Retrive the current phasor measurements
            //List<CurrentFlowPhasorGroup> measuredCurrents = network.Model.ActiveCurrentPhasors;
            List<CurrentFlowPhasorGroup> measuredCurrents = network.Model.IncludedCurrentFlows;
            List<CurrentInjectionPhasorGroup> measuredShuntCurrents = network.Model.ActiveCurrentInjections;

            // Resolve the Network into a list of ObservedBusses
            List<ObservedBus> observedBusses = network.Model.ObservedBusses;

            // Create a list of directly measured nodes from the set of ObservedBusses
            List<Node> measuredNodes = new List<Node>();
            foreach (ObservedBus observedBus in observedBusses)
            {
                foreach (Node node in observedBus.Nodes)
                {
                    if (node.Observability == ObservationState.DirectlyObserved)
                    {
                        measuredNodes.Add(node);
                    }
                }
            }

            if (measuredNodes.Count > 0 || measuredShuntCurrents.Count > 0)
            {
                m_matrixIsValid = true;
                m_covarianceMatrix = DenseMatrix.OfArray(new Complex[measuredNodes.Count + measuredCurrents.Count + measuredShuntCurrents.Count, measuredNodes.Count + measuredCurrents.Count + measuredShuntCurrents.Count]);

                for (int i = 0; i < m_covarianceMatrix.RowCount; i++)
                {
                    if (i < measuredNodes.Count)
                    {
                        m_covarianceMatrix[i, i] = new Complex(measuredNodes[i].Voltage.PositiveSequence.Measurement.MeasurementCovariance, 0);
                    }
                    else if (i < (measuredNodes.Count + measuredCurrents.Count))
                    {
                        m_covarianceMatrix[i, i] = new Complex(measuredCurrents[i - measuredNodes.Count].PositiveSequence.Measurement.MeasurementCovariance, 0);
                    }
                    else
                    {
                        m_covarianceMatrix[i, i] = new Complex(measuredShuntCurrents[i - (measuredNodes.Count + measuredCurrents.Count)].PositiveSequence.Measurement.MeasurementCovariance, 0);
                    }
                }
            }
            else
            {
                m_matrixIsValid = false;
            }
        }

        /// <summary>
        /// Builds the covariance matrix to give weight to each measurement for the three phase formulation.
        /// </summary>
        /// <param name="network">The <see cref="LinearStateEstimator.Networks.Network"/> which is desired to represent as matrices.</param>
        private void BuildThreePhaseMatrix(Network network)
        {
            //List<CurrentFlowPhasorGroup> measuredCurrents = network.Model.ActiveCurrentPhasors;
            List<CurrentFlowPhasorGroup> measuredCurrents = network.Model.IncludedCurrentFlows;

            List<CurrentInjectionPhasorGroup> measuredShuntCurrents = network.Model.ActiveCurrentInjections;

            // Resolve the Network into a list of ObservedBusses
            List<ObservedBus> observedBusses = network.Model.ObservedBusses;

            // Create a list of directly measured nodes from the set of ObservedBusses
            List<Node> measuredNodes = new List<Node>();
            foreach (ObservedBus observedBus in observedBusses)
            {
                foreach (Node node in observedBus.Nodes)
                {
                    if (node.Observability == ObservationState.DirectlyObserved)
                    {
                        measuredNodes.Add(node);
                    }
                }
            }


            if (measuredNodes.Count > 0 || measuredShuntCurrents.Count > 0)
            {
                m_matrixIsValid = true;
                m_covarianceMatrix = DenseMatrix.OfArray(new Complex[3 * (measuredNodes.Count + measuredCurrents.Count + measuredShuntCurrents.Count), 3 * (measuredNodes.Count + measuredCurrents.Count + measuredShuntCurrents.Count)]);

                for (int i = 0; i < measuredNodes.Count + measuredCurrents.Count; i++)
                {
                    if (i < measuredNodes.Count)
                    {
                        m_covarianceMatrix[3 * i, 3 * i] = new Complex(measuredNodes[i].Voltage.PhaseA.Measurement.MeasurementCovariance, 0);
                        m_covarianceMatrix[3 * i + 1, 3 * i + 1] = new Complex(measuredNodes[i].Voltage.PhaseB.Measurement.MeasurementCovariance, 0);
                        m_covarianceMatrix[3 * i + 2, 3 * i + 2] = new Complex(measuredNodes[i].Voltage.PhaseC.Measurement.MeasurementCovariance, 0);
                    }
                    else if (i < (measuredNodes.Count + measuredCurrents.Count))
                    {
                        m_covarianceMatrix[3 * i, 3 * i] = new Complex(measuredCurrents[i - measuredNodes.Count].PhaseA.Measurement.MeasurementCovariance, 0);
                        m_covarianceMatrix[3 * i + 1, 3 * i + 1] = new Complex(measuredCurrents[i - measuredNodes.Count].PhaseB.Measurement.MeasurementCovariance, 0);
                        m_covarianceMatrix[3 * i + 2, 3 * i + 2] = new Complex(measuredCurrents[i - measuredNodes.Count].PhaseC.Measurement.MeasurementCovariance, 0);
                    }
                    else
                    {
                        m_covarianceMatrix[3 * i, 3 * i] = new Complex(measuredShuntCurrents[i - (measuredNodes.Count + measuredCurrents.Count)].PhaseA.Measurement.MeasurementCovariance, 0);
                        m_covarianceMatrix[3 * i + 1, 3 * i + 1] = new Complex(measuredShuntCurrents[i - (measuredNodes.Count + measuredCurrents.Count)].PhaseB.Measurement.MeasurementCovariance, 0);
                        m_covarianceMatrix[3 * i + 2, 3 * i + 2] = new Complex(measuredShuntCurrents[i - (measuredNodes.Count + measuredCurrents.Count)].PhaseC.Measurement.MeasurementCovariance, 0);

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
        /// Converts the covariance matrix to a string which can be saved as a CSV file.
        /// </summary>
        /// <returns>A string which can be saved as a CSV file.</returns>
        public string ToCsvString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < m_covarianceMatrix.RowCount; i++)
            {
                string csvRow = "";
                for (int j = 0; j < m_covarianceMatrix.ColumnCount; j++)
                {
                    if (j < m_covarianceMatrix.ColumnCount - 1)
                    {
                        csvRow += m_covarianceMatrix[i, j].Real.ToString() + ",";
                    }
                    else
                    {
                        csvRow += m_covarianceMatrix[i, j].Real.ToString();
                    }
                }
                stringBuilder.AppendFormat(csvRow + "{0}", Environment.NewLine);
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Saves the Csv string version of the matrix to the specified file with a timestamp.
        /// </summary>
        /// <param name="pathName">The specified path name to save the Csv file representation of the matrix.</param>
        public void WriteToCsvFile(string pathName)
        {
            using (StreamWriter outfile = new StreamWriter(pathName + @"\Covariance Matrix " + String.Format("{0:yyyy-MM-dd  hh-mm-ss}", DateTime.UtcNow) + ".csv"))
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
        //    using (MatlabMatrixWriter outfile = new MatlabMatrixWriter(pathName + @"\Covariance Matrix " + String.Format("{0:yyyy-MM-dd  hh-mm-ss}", DateTime.UtcNow) + ".mat"))
        //    {
        //        outfile.WriteMatrix<Complex>(m_covarianceMatrix, "W");
        //    }
        //}

        #endregion
    }
}
