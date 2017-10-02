//******************************************************************************************************
//  VoltageMeasurementBusIncidenceMatrix.cs
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
//       Removed XML markup for class header to prevent misunderstanding since things have changed.
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

namespace SynchrophasorAnalytics.Matrices
{
    /// <summary>
    /// A matrix which represents the relationship between voltage measurements and observed busses in the network.
    /// </summary>
    public class VoltageMeasurementBusIncidenceMatrix
    {
        #region [ Private Members ]

        private DenseMatrix m_II = DenseMatrix.OfArray(new Complex[1, 1]);
        private List<string> m_csvColumnHeaders;
        private List<string> m_csvRowHeaders;
        private PhaseSelection m_phaseSelection;
        private bool m_matrixIsValid;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// The voltage measurement-bus incidence matrix
        /// </summary>
        public DenseMatrix Matrix
        {
            get 
            { 
                return m_II; 
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
        /// The designated constructor for the <see cref="VoltageMeasurementBusIncidenceMatrix"/> class.
        /// </summary>
        /// <param name="network">The virtualized <see cref="Network"/> model.</param>
        public VoltageMeasurementBusIncidenceMatrix(Network network)
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
        /// Constructs the <see cref="LinearStateEstimator.Networks.PhaseSelection.PositiveSequence"/> version of the <see cref="LinearStateEstimator.Matrices.VoltageMeasurementBusIncidenceMatrix"/>.
        /// </summary>
        /// <param name="network">The virtualized <see cref="LinearStateEstimator.Networks.Network"/> model.</param>
        private void BuildPositiveSequenceMatrix(Network network)
        {
            m_csvRowHeaders = new List<string>();
            m_csvColumnHeaders = new List<string>();
            m_csvColumnHeaders.Add("II Matrix");

            // Resolve the Network into a list of ObservedBusses
            List<ObservedBus> observedBusses = network.Model.ObservedBusses;

            if (observedBusses.Count > 0)
            {
                m_matrixIsValid = true;

                // Create a list of directly measured nodes from the set of ObservedBusses
                List<Node> measuredNodes = new List<Node>();
                foreach (ObservedBus observedBus in observedBusses)
                {
                    string columnHeader = "Sub: " + observedBus.InternalID.ToString() + " with nodes: ";
                    foreach (Node node in observedBus.Nodes)
                    {
                        columnHeader += node.InternalID.ToString() + "-";
                        if (node.Observability == ObservationState.DirectlyObserved)
                        {
                            measuredNodes.Add(node);
                            m_csvRowHeaders.Add(node.Description);
                        }
                    }
                    m_csvColumnHeaders.Add(columnHeader);
                }

                // Construct an empty matrix to populate
                m_II = DenseMatrix.OfArray(new Complex[measuredNodes.Count, observedBusses.Count]);

                foreach (ObservedBus observedBus in observedBusses)
                {
                    foreach (Node node in observedBus.Nodes)
                    {
                        if (node.Observability == ObservationState.DirectlyObserved)
                        {
                            // If measurement X measures bus Y its element should be 1
                            m_II[measuredNodes.IndexOf(node), observedBusses.IndexOf(observedBus)] = new Complex(1, 0);
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
        /// Constructs the <see cref="LinearStateEstimator.Networks.PhaseSelection.ThreePhase"/> version of the <see cref="LinearStateEstimator.Matrices.VoltageMeasurementBusIncidenceMatrix"/>.
        /// </summary>
        /// <param name="network">The virtualized <see cref="LinearStateEstimator.Networks.Network"/> model.</param>
        private void BuildThreePhaseMatrix(Network network)
        {
            m_csvRowHeaders = new List<string>();
            m_csvColumnHeaders = new List<string>();
            m_csvColumnHeaders.Add("II Matrix");

            // Resolve the Network into a list of ObservedBusses
            List<ObservedBus> observedBusses = network.Model.ObservedBusses;

            if (observedBusses.Count > 0)
            {
                m_matrixIsValid = true;

                // Create a list of directly measured nodes from the set of ObservedBusses
                List<Node> measuredNodes = new List<Node>();
                foreach (ObservedBus observedBus in observedBusses)
                {
                    string columnHeader = "Sub: " + observedBus.InternalID.ToString() + " with nodes: ";
                    foreach (Node node in observedBus.Nodes)
                    {
                        columnHeader += node.InternalID.ToString() + "-";
                        if (node.Observability == ObservationState.DirectlyObserved)
                        {
                            measuredNodes.Add(node);
                            m_csvRowHeaders.Add(node.Description);
                        }
                    }
                    m_csvColumnHeaders.Add(columnHeader);
                }

                // Construct an empty matrix to populate
                m_II = DenseMatrix.OfArray(new Complex[3 * measuredNodes.Count, 3 * observedBusses.Count]);

                foreach (ObservedBus observedBus in observedBusses)
                {
                    foreach (Node node in observedBus.Nodes)
                    {
                        if (node.Observability == ObservationState.DirectlyObserved)
                        {
                            // If measurement X measures bus Y its element should be an identity matrix
                            m_II[(3 * measuredNodes.IndexOf(node)), (3 * observedBusses.IndexOf(observedBus))] = new Complex(1, 0);
                            m_II[(3 * measuredNodes.IndexOf(node)) + 1, (3 * observedBusses.IndexOf(observedBus)) + 1] = new Complex(1, 0);
                            m_II[(3 * measuredNodes.IndexOf(node)) + 2, (3 * observedBusses.IndexOf(observedBus)) + 2] = new Complex(1, 0);
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
        /// Converts the <see cref="LinearStateEstimator.Matrices.VoltageMeasurementBusIncidenceMatrix"/> into a string in CSV format.
        /// </summary>
        /// <returns>A CSV formatted string representation of the <see cref="LinearStateEstimator.Matrices.VoltageMeasurementBusIncidenceMatrix"/>.</returns>
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

            for (int i = 0; i < m_II.RowCount; i++)
            {
                string csvRow = "";

                if (m_phaseSelection == PhaseSelection.PositiveSequence)
                {
                    csvRow = m_csvRowHeaders[i] + ",";
                }

                for (int j = 0; j < m_II.ColumnCount; j++)
                {
                    if (j < m_II.ColumnCount - 1)
                    {
                        csvRow += m_II[i, j].Real.ToString() + ",";
                    }
                    else
                    {
                        csvRow += m_II[i, j].Real.ToString();
                    }
                }
                stringBuilder.AppendFormat(csvRow + "{0}", Environment.NewLine);
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Method to print the <see cref="LinearStateEstimator.Matrices.VoltageMeasurementBusIncidenceMatrix"/> to a CSV file named 'II Matrix yyyy-MM-dd  hh-mm-ss.csv' where the date and time are in UTC time.
        /// </summary>
        /// <param name="pathName">The directory name (not including the file name) to the destination for the file to be written to.</param>
        public void WriteToCsvFile(string pathName)
        {
            using (StreamWriter outfile = new StreamWriter(pathName + @"\II Matrix " + String.Format("{0:yyyy-MM-dd  hh-mm-ss}", DateTime.UtcNow) + ".csv"))
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
        //    using (MatlabMatrixWriter outfile = new MatlabMatrixWriter(pathName + @"\II Matrix " + String.Format("{0:yyyy-MM-dd  hh-mm-ss}", DateTime.UtcNow) + ".mat"))
        //    {
        //        outfile.WriteMatrix<Complex>(m_II, "II");
        //    }
        //}

        #endregion
    }
}
