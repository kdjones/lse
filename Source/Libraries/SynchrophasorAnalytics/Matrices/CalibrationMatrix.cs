//******************************************************************************************************
//  CalibrationMatrix.cs
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
//  12/30/2009 - Joanna Wu
//       Created original algorithms in Matlab
//  07/20/2011 - Kevin D. Jones
//       Generated original version of source code.
//  07/25/2013 - Kevin D. Jones
//       Modified to accept new parameters in constructor and use new Network Model
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinearStateEstimator.Networks;
using LinearStateEstimator.Modeling;
using LinearStateEstimator.Measurements;
using LinearStateEstimator.Matrices;
using LinearStateEstimator.Calibration;
using Extreme.Mathematics;
using Extreme.Mathematics.LinearAlgebra;
using Extreme.Mathematics.LinearAlgebra.Complex;
using Extreme.Mathematics.LinearAlgebra.Providers;

namespace LinearStateEstimator.Matrices
{
    public class CalibrationMatrix
    {
        #region [ Private Members ]

        private ComplexMatrix m_calibrationMatrix;

        #endregion

        #region [ Properties ]

        public ComplexMatrix Matrix
        {
            get
            {
                return m_calibrationMatrix;
            }
        }

        #endregion

        #region [ Constructors ]

        public CalibrationMatrix(Network network)
        {
            if (network.Model.PhaseConfiguration == PhaseSelection.ThreePhase)
            {
                BuildMatrices(network);
            }
            else
            {
                throw new Exception("Cannot create a calibration matrix unless the network configuraiton is three phase.");
            }
        }

        #endregion


        #region [ Private Methods ]

        private void BuildMatrices(Network network)
        {
            // Create the current measurement bus incidence matrix
            ComplexMatrix A = (new CurrentFlowMeasurementBusIncidenceMatrix(network)).Matrix;

            // Create the series admittance matrix
            ComplexMatrix Y = (new SeriesAdmittanceMatrix(network)).Matrix;

            // Create the shunt susceptance matrix
            ComplexMatrix Ys = (new LineShuntSusceptanceMatrix(network)).Matrix;

            // Compute the lower partition of the system matrix
            ComplexMatrix K = Y * A + Ys;

            List<int> rowsToBeRemoved = RowsToRemove(network);
            int numberOfRowsToBeRemoved = rowsToBeRemoved.Count();
            for (int i = 0; i < numberOfRowsToBeRemoved; i++)
            {
                int rowToRemove = rowsToBeRemoved.Max();
                K = MatrixCalculationExtensions.RemoveRow(K, rowToRemove);
                rowsToBeRemoved.Remove(rowToRemove);
            }

            List<int> columnsToBeRemoved = ColumnsToRemove(network);
            int numberOfColumnsToBeRemoved = columnsToBeRemoved.Count();
            for (int i = 0; i < numberOfColumnsToBeRemoved; i++)
            {
                int columnToRemove = columnsToBeRemoved.Max();
                K = MatrixCalculationExtensions.RemoveColumn(K, columnToRemove);
                columnsToBeRemoved.Remove(columnToRemove);
            }
        }

        private List<int> RowsToRemove(Network network)
        {
            List<CurrentFlowPhasorGroup> currentsToBeRemoved = new List<CurrentFlowPhasorGroup>();
            List<int> indicesOfRowsToBeRemoved = new List<int>();

            // Find which phasor groups should not be included in the calibration
            foreach (CurrentFlowPhasorGroup currentPhasorGroup in network.Model.ActiveCurrentPhasors)
            {
                if (currentPhasorGroup.PhaseA.Measurement.InstrumentTransformerCalibrationSetting != CalibrationSetting.Active ||
                    currentPhasorGroup.PhaseB.Measurement.InstrumentTransformerCalibrationSetting != CalibrationSetting.Active ||
                    currentPhasorGroup.PhaseC.Measurement.InstrumentTransformerCalibrationSetting != CalibrationSetting.Active)
                {
                    currentsToBeRemoved.Add(currentPhasorGroup);
                }
            }

            // Get the numerical values of the rows to be removed
            foreach (CurrentFlowPhasorGroup currentPhasorGroup in currentsToBeRemoved)
            {
                indicesOfRowsToBeRemoved.Add(3 * (network.Model.ActiveCurrentPhasors.IndexOf(currentPhasorGroup) - 1));
                indicesOfRowsToBeRemoved.Add(3 * (network.Model.ActiveCurrentPhasors.IndexOf(currentPhasorGroup) - 1) + 1);
                indicesOfRowsToBeRemoved.Add(3 * (network.Model.ActiveCurrentPhasors.IndexOf(currentPhasorGroup) - 1) + 2);
            }

            return indicesOfRowsToBeRemoved;
        }

        private List<int> ColumnsToRemove(Network network)
        {
            List<ObservedBus> observedBussesToBeRemoved = new List<ObservedBus>();
            List<int> indicesOfColumnsToBeRemoved = new List<int>();

            foreach (ObservedBus observedBus in network.Model.ObservedBusses)
            {
                foreach (Node node in observedBus.Nodes)
                {
                    if ((node.Voltage.PhaseA.Measurement.InstrumentTransformerCalibrationSetting != CalibrationSetting.Active ||
                         node.Voltage.PhaseA.Measurement.InstrumentTransformerCalibrationSetting != CalibrationSetting.Perfect) &&
                        (node.Voltage.PhaseB.Measurement.InstrumentTransformerCalibrationSetting != CalibrationSetting.Active ||
                         node.Voltage.PhaseB.Measurement.InstrumentTransformerCalibrationSetting != CalibrationSetting.Perfect) &&
                        (node.Voltage.PhaseC.Measurement.InstrumentTransformerCalibrationSetting != CalibrationSetting.Active ||
                         node.Voltage.PhaseC.Measurement.InstrumentTransformerCalibrationSetting != CalibrationSetting.Perfect))
                    {
                        observedBussesToBeRemoved.Add(observedBus);
                    }
                }
            }

            foreach (ObservedBus observedBus in observedBussesToBeRemoved)
            {
                indicesOfColumnsToBeRemoved.Add(3 * (network.Model.ObservedBusses.IndexOf(observedBus) - 1));
                indicesOfColumnsToBeRemoved.Add(3 * (network.Model.ObservedBusses.IndexOf(observedBus) - 1) + 1);
                indicesOfColumnsToBeRemoved.Add(3 * (network.Model.ObservedBusses.IndexOf(observedBus) - 1) + 2);
            }

            return indicesOfColumnsToBeRemoved;
        }

        #endregion
    }
}
