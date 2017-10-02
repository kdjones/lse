//******************************************************************************************************
//  VertexAdjacencyList.cs
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
//
//******************************************************************************************************
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using SynchrophasorAnalytics.Modeling;

namespace SynchrophasorAnalytics.Graphs
{
    /// <summary>
    /// The adjacency list representation of a network component graph
    /// </summary>
    public class VertexAdjacencyList
    {
        #region [ Private Members ]

        private List<VertexAdjacencyRow> m_adjacencyRows;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// A list of the rows in the adjacency list
        /// </summary>
        public List<VertexAdjacencyRow> Rows
        {
            get
            {
                return m_adjacencyRows;
            }
            set
            {
                m_adjacencyRows = value;
            }
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// The default constructor for the class
        /// </summary>
        public VertexAdjacencyList()
            :this(new List<VertexAdjacencyRow>())
        {
        }

        /// <summary>
        /// The designated constructor for the class
        /// </summary>
        /// <param name="adjacencyRows">A list of adjacency rows</param>
        public VertexAdjacencyList(List<VertexAdjacencyRow> adjacencyRows)
        {
            m_adjacencyRows = adjacencyRows;
        }

        #endregion

        #region [ Public Methods ]

        /// <summary>
        /// A string representation of the adjacency list.
        /// </summary>
        /// <returns>A string representation of the adjacency list.</returns>
        public override string ToString()
        {
            if (m_adjacencyRows.Count != 0)
            {
                StringBuilder stringBuilder = new StringBuilder();

                stringBuilder.AppendLine(" ------------------- Adjacency List -------------------- ");
                stringBuilder.AppendLine("   Format => (Vertex) | (Adjacency 1),...,(Adjacency n)");
                List<int> dividerIndices = new List<int>();

                foreach (VertexAdjacencyRow row in m_adjacencyRows)
                {
                    dividerIndices.Add(row.ToString().IndexOf('|'));
                }

                int dividerIndexMaximum = dividerIndices.Max();

                foreach (VertexAdjacencyRow row in m_adjacencyRows)
                {
                    int lengthOfWhiteSpaceToAppend = dividerIndexMaximum - row.ToString().IndexOf('|');

                    string whiteSpaceToAppend = "";

                    for (int i = 0; i < lengthOfWhiteSpaceToAppend + 1; i++)
                    {
                        whiteSpaceToAppend += " ";
                    }

                    stringBuilder.AppendLine(whiteSpaceToAppend + row.ToString());
                }

                return stringBuilder.ToString();
            }
            else
            {
                return "(null set)";
            }
        }

        /// <summary>
        /// Sorts the contents of each of the rows in the adjacency list.
        /// </summary>
        public void Sort()
        {
            foreach (VertexAdjacencyRow row in m_adjacencyRows)
            {
                row.Sort();
            }
        }

        /// <summary>
        /// Removes duplicate vertices from each of the vertex clusters in each of the rows in the adjacency list.
        /// </summary>
        public void RemoveDuplicateVertices()
        {
            foreach (VertexAdjacencyRow row in m_adjacencyRows)
            {
                row.RemoveDuplicateVertices();
            }
        }

        /// <summary>
        /// Removes a specified row from the list.
        /// </summary>
        /// <param name="row">The row to remove.</param>
        public void RemoveRow(VertexAdjacencyRow row)
        {
            m_adjacencyRows.Remove(row);
        }

        /// <summary>
        /// Finds the row with the specified header vertex cluster.
        /// </summary>
        /// <param name="header">The specified header vertex cluster.</param>
        /// <returns>The row with the specified header.</returns>
        public VertexAdjacencyRow RowWithHeader(VertexCluster header)
        {
            foreach (VertexAdjacencyRow row in m_adjacencyRows)
            {
                if (row.Header.Equals(header))
                {
                    return row;
                }
            }

            return null;
        }

        #endregion
    }
}
