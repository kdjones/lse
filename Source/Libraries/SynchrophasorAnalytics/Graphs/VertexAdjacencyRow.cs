//******************************************************************************************************
//  VertexAdjacencyRows.cs
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
using System.Linq;
using System.Text;

namespace SynchrophasorAnalytics.Graphs
{
    /// <summary>
    /// Represents a single row in a <see cref="LinearStateEstimator.Graphs.VertexAdjacencyList"/>.
    /// </summary>
    public class VertexAdjacencyRow
    {
        #region [ Private Members ]

        private VertexCluster m_rowHeader;
        private List<VertexCluster> m_adjacencies;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// The central vertex cluster which all of the <see cref="LinearStateEstimator.Graphs.VertexAdjacencyRow.Adjacencies"/> are connected to.
        /// </summary>
        public VertexCluster Header
        {
            get
            {
                return m_rowHeader;
            }
            set
            {
                m_rowHeader = value;
            }
        }

        /// <summary>
        /// All of the vertex clusters which are connected to the header vertex cluster.
        /// </summary>
        public List<VertexCluster> Adjacencies
        {
            get
            {
                return m_adjacencies;
            }
            set
            {
                m_adjacencies = value;
            }
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// The default constructor for the class.
        /// </summary>
        public VertexAdjacencyRow()
            :this(new VertexCluster(), new List<VertexCluster>())
        {
        }

        /// <summary>
        /// The designated constructor for the class.
        /// </summary>
        /// <param name="rowHeader">The vertex cluster header.</param>
        /// <param name="adjacencies">The vertex cluster adjacencies.</param>
        public VertexAdjacencyRow(VertexCluster rowHeader, List<VertexCluster> adjacencies)
        {
            m_rowHeader = rowHeader;
            m_adjacencies = adjacencies;
        }

        #endregion

        #region [ Public Methods ]

        /// <summary>
        /// A string representation of the <see cref="LinearStateEstimator.Graphs.VertexAdjacencyRow"/>.
        /// </summary>
        /// <returns>A string representation of the <see cref="LinearStateEstimator.Graphs.VertexAdjacencyRow"/>.</returns>
        public override string ToString()
        {
            string header = m_rowHeader.ToString();
            string adjacencies = "";

            foreach (VertexCluster cluster in m_adjacencies)
            {
                adjacencies += cluster.ToString() + ",";
            }

            if (adjacencies.Length > 1)
            {
                adjacencies = adjacencies.Substring(0, adjacencies.Length - 1);
                return header + " | " + adjacencies;
            }
            else
            {
                return header + " | (null)";
            }
        }

        /// <summary>
        /// Merges this row with the specified row.
        /// </summary>
        /// <param name="adjacencyRow">The row to merge with this row.</param>
        public void MergeWith(VertexAdjacencyRow adjacencyRow)
        {
            // Remove the headers of the merging rows from their respective adjacency lists.
            this.RemoveVertex(adjacencyRow.Header);
            adjacencyRow.RemoveVertex(this.Header);

            // OR-Wise Merging of Adjacencies
            foreach (VertexCluster adjacency in adjacencyRow.Adjacencies)
            {
                if (!this.ContainsAdjacency(adjacency))
                {
                    m_adjacencies.Add(adjacency);
                }
            }

            m_rowHeader.MergeWith(adjacencyRow.Header);
        }

        /// <summary>
        /// Merges this row with the specified row and returns the resulting row
        /// </summary>
        /// <param name="adjacencyRow">The row to merge with this row.</param>
        /// <returns>A row which is the OR-wise union of two rows.</returns>
        public VertexAdjacencyRow MergeToFormNewRow(VertexAdjacencyRow adjacencyRow)
        {
            this.MergeWith(adjacencyRow);
            return this;
        }

        /// <summary>
        /// Removes vertex clusters which are duplicates from individual clusters.
        /// </summary>
        public void RemoveDuplicateVertices()
        {
            m_rowHeader.RemoveDuplicateVertices();

            foreach (VertexCluster cluster in m_adjacencies)
            {
                cluster.RemoveDuplicateVertices();
            }
        }

        /// <summary>
        /// Removes duplicate vertex clusters from the adjacencies.
        /// </summary>
        public void RemoveDuplicateVertexClusters()
        {
            VertexAdjacencyRow temporaryRow = new VertexAdjacencyRow();

            foreach (VertexCluster vertexCluster in m_adjacencies)
            {
                if (!temporaryRow.ContainsAdjacency(vertexCluster))
                {
                    temporaryRow.Adjacencies.Add(vertexCluster);
                }
            }

            m_adjacencies = temporaryRow.Adjacencies;
        }

        /// <summary>
        /// Removes the specified vertex cluster from the list of adjacencies.
        /// </summary>
        /// <param name="vertexCluster">The resulting vertex cluster.</param>
        public void RemoveVertex(VertexCluster vertexCluster)
        {
            List<VertexCluster> updatedVertices = new List<VertexCluster>();

            foreach (VertexCluster adjacency in m_adjacencies)
            {
                if (!adjacency.Equals(vertexCluster))
                {
                    updatedVertices.Add(adjacency);
                }
            }

            m_adjacencies = updatedVertices;
        }

        /// <summary>
        /// Returns a boolen flag indicating whether the specified vertex cluster is an adjacency of the row.
        /// </summary>
        /// <param name="vertexCluster">The vertex cluster of interest.</param>
        /// <returns>A boolean flag indicating whether the specified vertex cluster is an adjacency of the row.</returns>
        public bool ContainsAdjacency(VertexCluster vertexCluster)
        {
            foreach (VertexCluster adjacency in m_adjacencies)
            {
                if (adjacency.Equals(vertexCluster))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Sorts each of the vertex clusters in the header and the adjacencies to order the vertices contained in each.
        /// </summary>
        public void Sort()
        {
            m_rowHeader.Sort();

            foreach (VertexCluster cluster in m_adjacencies)
            {
                cluster.Sort();
            }
        }

        #endregion
    }
}
