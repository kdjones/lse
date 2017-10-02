//******************************************************************************************************
//  MatrixCalculationExtensions.cs
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
//  07/20/2011 - Kevin D. Jones
//       Generated original version of source code as part of SystemMatrix class
//  06/01/2013 - Kevin D. Jones
//       Removed concatenation from SystemMatrix class and made it its own set of extensions.
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
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra.Complex;
using MathNet.Numerics.LinearAlgebra;

namespace SynchrophasorAnalytics.Matrices
{
    /// <summary>
    /// Extensions of the <see cref="DenseMatrix"/> class for easily concatenation and row and column manipulation.
    /// </summary>
    public static class MatrixCalculationExtensions
    {
        /// <summary>
        /// Vertically concatenates two <see cref="DenseMatrix"/> objects into one <see cref="DenseMatrix"/> object.
        /// </summary>
        /// <param name="A"> Top partition of desired matrix. </param>
        /// <param name="B"> Botton partition of desired matrix. </param>
        /// <returns> <see cref="DenseMatrix"/> object which is the vertical concatenation of two <see cref="DenseMatrix"/> objects. </returns>
        public static DenseMatrix VerticallyConcatenate(DenseMatrix A, DenseMatrix B)
        {
            if (A.ColumnCount == B.ColumnCount)
            {
                DenseMatrix X = DenseMatrix.OfArray(new Complex[A.RowCount + B.RowCount, A.ColumnCount]);

                for (int i = 0; i < A.RowCount; i++)
                {
                    for (int j = 0; j < A.ColumnCount; j++)
                    {
                        X[i, j] = A[i, j];
                    }
                }

                for (int i = 0; i < B.RowCount; i++)
                {
                    for (int j = 0; j < B.ColumnCount; j++)
                    {
                        X[i + A.RowCount, j] = B[i, j];
                    }
                }
                return X;
            }
            else
            {
                throw new Exception("Matrix dimensions do not align: A - (" + A.RowCount.ToString() + " x " + A.ColumnCount.ToString() + ")  B - ("+ B.RowCount.ToString() + " x " + B.ColumnCount.ToString() + ")");
            }
        }

        /// <summary>
        /// Horizontally concatenates two <see cref="DenseMatrix"/> objects into one <see cref="DenseMatrix"/> object.</summary>
        /// <param name="A">Left partition of desired matrix.</param>
        /// <param name="B">Right partition of desired matrix.</param>
        /// <returns><see cref="DenseMatrix"/> object which is the horizontal concatenation of two <see cref="DenseMatrix"/> objects.</returns>
        public static DenseMatrix HorizontallyConcatenate(DenseMatrix A, DenseMatrix B)
        {
            if (A.RowCount == B.RowCount)
            {
                DenseMatrix X = DenseMatrix.OfArray(new Complex[A.RowCount, A.ColumnCount + B.ColumnCount]);

                for (int i = 0; i < A.RowCount; i++)
                {
                    for (int j = 0; j < A.ColumnCount; j++)
                    {
                        X[i, j] = A[i, j];
                    }
                }

                for (int i = 0; i < B.RowCount; i++)
                {
                    for (int j = 0; j < B.ColumnCount; j++)
                    {
                        X[i, j + A.ColumnCount] = B[i, j];
                    }
                }

                return X;
            }
            else
            {
                throw new Exception("Matrix dimensions do not align: A - (" + A.RowCount.ToString() + " x " + A.ColumnCount.ToString() + ")  B - (" + B.RowCount.ToString() + " x " + B.ColumnCount.ToString() + ")");
            }
        }

        /// <summary>
        /// Removes undesired column from specified matrix.
        /// </summary>
        /// <param name="A">The matrix to have the column removed from.</param>
        /// <param name="column">The integer index of the undesired column.</param>
        /// <returns><see cref="DenseMatrix"/> object which has had a specified column removed.</returns>
        public static DenseMatrix RemoveColumn(DenseMatrix A, int column)
        {
            DenseMatrix newMatrix = DenseMatrix.OfArray(new Complex[A.RowCount, A.ColumnCount - 1]);

            if (column == 0)
            {
                newMatrix = A.SubMatrix(0, A.RowCount - 1, 1, A.ColumnCount - 1) as DenseMatrix;
            }
            else if (column == 1)
            {
                Vector<Complex> leftColumn = A.Column(0);
                DenseMatrix rightMatrix = A.SubMatrix(0, A.RowCount - 1, 2, A.ColumnCount - 1) as DenseMatrix;
                DenseMatrix leftColumnAsMatrix = DenseMatrix.OfArray(new Complex[leftColumn.Count(), 1]);

                for (int i = 0; i < leftColumn.Count(); i++)
                {
                    leftColumnAsMatrix[i, 0] = leftColumn[i];
                }

                newMatrix = HorizontallyConcatenate(leftColumnAsMatrix, rightMatrix);
            }
            else if (column == A.ColumnCount - 2)
            {
                Vector<Complex> rightColumn = A.Column(A.ColumnCount - 1);
                DenseMatrix leftMatrix = A.SubMatrix(0, A.RowCount - 1, 0, A.ColumnCount - 3) as DenseMatrix;
                DenseMatrix rightColumnAsMatrix = DenseMatrix.OfArray(new Complex[rightColumn.Count(), 1]);

                for (int i = 0; i < rightColumn.Count(); i++)
                {
                    rightColumnAsMatrix[i, 0] = rightColumn[i];
                }

                newMatrix = HorizontallyConcatenate(leftMatrix, rightColumnAsMatrix);
            }
            else if (column == A.ColumnCount - 1)
            {
                newMatrix = A.SubMatrix(0, A.RowCount - 1, 0, A.ColumnCount - 2) as DenseMatrix;
            }
            else if (column > 1 && column < A.ColumnCount - 2)
            {
                DenseMatrix leftMatrix = A.SubMatrix(0, A.RowCount - 1, 0, column - 1) as DenseMatrix;
                DenseMatrix rightMatrix = A.SubMatrix(0, A.RowCount - 1, column + 1, A.ColumnCount - 1) as DenseMatrix;

                newMatrix = HorizontallyConcatenate(leftMatrix, rightMatrix);
            }

            return newMatrix;
        }

        /// <summary>
        /// Removes undesired row from specified matrix.
        /// </summary>
        /// <param name="A">The matrix to have the row removed from.</param>
        /// <param name="row">The integer index of the undesired row.</param>
        /// <returns><see cref="DenseMatrix"/> object which has had a specified row removed.</returns>
        public static DenseMatrix RemoveRow(DenseMatrix A, int row)
        {
            DenseMatrix newMatrix = DenseMatrix.OfArray(new Complex[A.RowCount - 1, A.ColumnCount]);

            if (row == 0)
            {
                newMatrix = A.SubMatrix(1, A.RowCount - 1, 0, A.ColumnCount - 1) as DenseMatrix;
            }
            else if (row == 1)
            {
                Vector<Complex> topRow = A.Row(0);
                DenseMatrix bottomMatrix = A.SubMatrix(2, A.RowCount - 1, 0, A.ColumnCount - 1) as DenseMatrix;
                DenseMatrix topRowAsMatrix = DenseMatrix.OfArray(new Complex[1, topRow.Count()]);

                for (int i = 0; i < topRow.Count(); i++)
                {
                    topRowAsMatrix[0, i] = topRow[i];
                }

                newMatrix = VerticallyConcatenate(topRowAsMatrix, bottomMatrix);
            }
            else if (row == A.RowCount - 2)
            {
                Vector<Complex> bottomRow = A.Row(A.RowCount - 1);
                DenseMatrix topMatrix = A.SubMatrix(0, A.RowCount - 3, 0, A.ColumnCount - 1) as DenseMatrix;
                DenseMatrix bottomRowAsMatrix = DenseMatrix.OfArray(new Complex[1, bottomRow.Count()]);

                for (int i = 0; i < bottomRow.Count(); i++)
                {
                    bottomRowAsMatrix[0, i] = bottomRow[i];
                }

                newMatrix = VerticallyConcatenate(topMatrix, bottomRowAsMatrix);
            }
            else if (row == A.RowCount - 1)
            {
                newMatrix = A.SubMatrix(0, A.RowCount - 2, 0, A.ColumnCount - 1) as DenseMatrix;
            }
            else if (row > 1 && row < A.RowCount - 2)
            {
                DenseMatrix topMatrix = A.SubMatrix(0, row - 1, 0, A.ColumnCount - 1) as DenseMatrix;
                DenseMatrix bottomMatrix = A.SubMatrix(row + 1, A.RowCount - 1, 0, A.ColumnCount - 1) as DenseMatrix;

                newMatrix = VerticallyConcatenate(topMatrix, bottomMatrix);
            }
            return newMatrix;
        }
       
    }
}
