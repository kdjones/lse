//******************************************************************************************************
//  CurrentMeasurementBusIncidenceMatrix.cs
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
//       Removed unused fields, methods from the class.
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
    /// A matrix representation of the relationship between the location of current flow measurements and observed busses in the observed network.
    /// </summary>
    public class CurrentFlowMeasurementBusIncidenceMatrix
    {
        #region [ Private Members ]

        private DenseMatrix m_A = DenseMatrix.OfArray(new Complex[1,1]);
        private List<string> m_csvRowHeaders;
        private List<string> m_csvColumnHeaders;
        private PhaseSelection m_phaseSelection;
        private bool m_matrixIsValid;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// The current measurement-bus incidence matrix
        /// </summary>
        public DenseMatrix Matrix
        {
            get 
            { 
                return m_A; 
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
        /// The designated constructor for the <see cref="CurrentFlowMeasurementBusIncidenceMatrix"/> class.
        /// </summary>
        /// <param name="network">The virtualized <see cref="Network"/> model.</param>
        public CurrentFlowMeasurementBusIncidenceMatrix(Network network)
        {
            BuildMatrices(network);
            m_phaseSelection = network.Model.PhaseConfiguration;
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
        /// Constructs the <see cref="LinearStateEstimator.Networks.PhaseSelection.PositiveSequence"/> version of the <see cref="LinearStateEstimator.Matrices.CurrentFlowMeasurementBusIncidenceMatrix"/>.
        /// </summary>
        /// <param name="network">The virtualized <see cref="LinearStateEstimator.Networks.Network"/> model.</param>
        private void BuildPositiveSequenceMatrix(Network network)
        {
            m_csvRowHeaders = new List<string>();
            m_csvColumnHeaders = new List<string>();
            m_csvColumnHeaders.Add("A Matrix");

            List<ObservedBus> observedBusses = network.Model.ObservedBusses;
            //List<CurrentFlowPhasorGroup> measuredCurrents = network.Model.ActiveCurrentPhasors;
            List<CurrentFlowPhasorGroup> measuredCurrents = network.Model.IncludedCurrentFlows;

            if (observedBusses.Count > 0 && measuredCurrents.Count > 0)
            {
                m_matrixIsValid = true;

                // Create an empty current measurement-bus incidence matrix rows equal in number to the number of current phasors and
                // columns equal in number to the number of observed busses in the network.
                m_A = DenseMatrix.OfArray(new Complex[measuredCurrents.Count, observedBusses.Count]);

                foreach (CurrentFlowPhasorGroup measuredCurrent in measuredCurrents)
                {
                    m_csvRowHeaders.Add(measuredCurrent.Description);
                    foreach (ObservedBus observedBus in observedBusses)
                    {
                        if (measuredCurrents.IndexOf(measuredCurrent) == measuredCurrents.Count - 1)
                        {
                            m_csvColumnHeaders.Add("Sub: " + observedBus.InternalID.ToString());
                        }
                        foreach (Node node in observedBus.Nodes)
                        {
                            if (node.InternalID == (measuredCurrent.MeasuredFromNodeID))
                            {
                                // If measurement X leaves bus Y its element should be 1
                                if (measuredCurrent.MeasuredBranch is Transformer)
                                {
                                    Transformer transformer = measuredCurrent.MeasuredBranch as Transformer;
                                    if (node.InternalID == transformer.FromNode.InternalID)
                                    {
                                        double aSquared = transformer.EffectiveComplexMultiplier.Magnitude * transformer.EffectiveComplexMultiplier.Magnitude;
                                        m_A[measuredCurrents.IndexOf(measuredCurrent), observedBusses.IndexOf(observedBus)] = new Complex(1, 0) / aSquared;
                                    }
                                    else if (node.InternalID == transformer.ToNode.InternalID)
                                    {
                                        m_A[measuredCurrents.IndexOf(measuredCurrent), observedBusses.IndexOf(observedBus)] = new Complex(1, 0);
                                    }
                                }
                                else if (measuredCurrent.MeasuredBranch is TransmissionLine)
                                {
                                    m_A[measuredCurrents.IndexOf(measuredCurrent), observedBusses.IndexOf(observedBus)] = new Complex(1, 0);
                                }
                            }
                            else if (node.InternalID == (measuredCurrent.MeasuredToNodeID))
                            {
                                // If measurement X points toward bus Z its element should be -1
                                if (measuredCurrent.MeasuredBranch is Transformer)
                                {
                                    Transformer transformer = measuredCurrent.MeasuredBranch as Transformer;
                                    if (node.InternalID == transformer.FromNode.InternalID)
                                    {
                                        m_A[measuredCurrents.IndexOf(measuredCurrent), observedBusses.IndexOf(observedBus)] = new Complex(-1, 0) / transformer.EffectiveComplexMultiplier.Conjugate();
                                    }
                                    else if (node.InternalID == transformer.ToNode.InternalID)
                                    {
                                        m_A[measuredCurrents.IndexOf(measuredCurrent), observedBusses.IndexOf(observedBus)] = new Complex(-1, 0) / transformer.EffectiveComplexMultiplier;
                                    }
                                }
                                else if (measuredCurrent.MeasuredBranch is TransmissionLine)
                                {
                                    m_A[measuredCurrents.IndexOf(measuredCurrent), observedBusses.IndexOf(observedBus)] = new Complex(-1, 0);
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
        /// Constructs the <see cref="LinearStateEstimator.Networks.PhaseSelection.ThreePhase"/> version of the <see cref="LinearStateEstimator.Matrices.CurrentFlowMeasurementBusIncidenceMatrix"/>.
        /// </summary>
        /// <param name="network">The virtualized <see cref="LinearStateEstimator.Networks.Network"/> model.</param>
        private void BuildThreePhaseMatrix(Network network)
        {
            List<ObservedBus> observedBusses = network.Model.ObservedBusses;
            //List<CurrentFlowPhasorGroup> measuredCurrents = network.Model.ActiveCurrentPhasors;
            List<CurrentFlowPhasorGroup> measuredCurrents = network.Model.IncludedCurrentFlows;

            if (observedBusses.Count > 0 && measuredCurrents.Count > 0)
            {
                m_matrixIsValid = true;
                // Create an empty current measurement-bus incidence matrix rows equal in number to the number of current phasors and
                // columns equal in number to the number of observed busses in the network.
                m_A = DenseMatrix.OfArray(new Complex[3 * measuredCurrents.Count, 3 * observedBusses.Count]);

                foreach (CurrentFlowPhasorGroup measuredCurrent in measuredCurrents)
                {
                    foreach (ObservedBus observedBus in observedBusses)
                    {
                        if (observedBus.Nodes.Contains(measuredCurrent.MeasuredFromNode))
                        {
                            // If measurement X leaves bus Y its element should be a 3 by 3 identity matrix.
                            m_A[(3 * measuredCurrents.IndexOf(measuredCurrent)), (3 * observedBusses.IndexOf(observedBus))] = new Complex(1, 0);
                            m_A[(3 * measuredCurrents.IndexOf(measuredCurrent)) + 1, (3 * observedBusses.IndexOf(observedBus)) + 1] = new Complex(1, 0);
                            m_A[(3 * measuredCurrents.IndexOf(measuredCurrent)) + 2, (3 * observedBusses.IndexOf(observedBus)) + 2] = new Complex(1, 0);
                        }
                        else if (observedBus.Nodes.Contains(measuredCurrent.MeasuredToNode))
                        {
                            // If measurement X points toward bus Z its element should be a 3 by 3 negative identity matrix
                            m_A[(3 * measuredCurrents.IndexOf(measuredCurrent)), (3 * observedBusses.IndexOf(observedBus))] = new Complex(-1, 0);
                            m_A[(3 * measuredCurrents.IndexOf(measuredCurrent)) + 1, (3 * observedBusses.IndexOf(observedBus)) + 1] = new Complex(-1, 0);
                            m_A[(3 * measuredCurrents.IndexOf(measuredCurrent)) + 2, (3 * observedBusses.IndexOf(observedBus)) + 2] = new Complex(-1, 0);
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
        /// Converts the <see cref="LinearStateEstimator.Matrices.CurrentFlowMeasurementBusIncidenceMatrix"/> into a string in CSV format.
        /// </summary>
        /// <returns>A CSV formatted string representation of the <see cref="LinearStateEstimator.Matrices.CurrentFlowMeasurementBusIncidenceMatrix"/>.</returns>
        public string ToCsvString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            if (m_phaseSelection == PhaseSelection.PositiveSequence)
            {
                string columnHeaders = "";
                foreach (string header in m_csvColumnHeaders)
                {
                    columnHeaders += header + ",";
                }

                columnHeaders = columnHeaders.Substring(0, columnHeaders.Length - 1);
                stringBuilder.AppendFormat(columnHeaders + "{0}", Environment.NewLine);
            }

            for (int i = 0; i < m_A.RowCount; i++)
            {
                string csvRow = "";

                if (m_phaseSelection == PhaseSelection.PositiveSequence)
                {
                    csvRow = m_csvRowHeaders[i] + ",";
                }
                for (int j = 0; j < m_A.ColumnCount; j++)
                {
                    if (j < m_A.ColumnCount - 1)
                    {
                        csvRow += m_A[i, j].Real.ToString() + ",";
                    }
                    else
                    {
                        csvRow += m_A[i, j].Real.ToString();
                    }
                }
                stringBuilder.AppendFormat(csvRow + "{0}", Environment.NewLine);
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Method to print the <see cref="LinearStateEstimator.Matrices.CurrentFlowMeasurementBusIncidenceMatrix"/> to a CSV file named 'A Matrix yyyy-MM-dd  hh-mm-ss.csv' where the date and time are in UTC time.
        /// </summary>
        /// <param name="pathName">The directory name (not including the file name) to the destination for the file to be written to.</param>
        public void WriteToCsvFile(string pathName)
        {
            using (StreamWriter outfile = new StreamWriter(pathName + @"\A Matrix " + String.Format("{0:yyyy-MM-dd  hh-mm-ss}", DateTime.UtcNow) + ".csv"))
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
        //    MatlabWriter.Write<Complex>(m_A, "A");

        //    using (MatlabWriter outfile = new MatlabWriter(pathName + @"\A Matrix " + String.Format("{0:yyyy-MM-dd  hh-mm-ss}", DateTime.UtcNow) + ".mat"))
        //    {
        //        outfile.WriteMatrix<Complex>(m_A, "A");
        //    }
        //}

        #endregion
    }
}
