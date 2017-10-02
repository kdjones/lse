//******************************************************************************************************
//  VertexCluster.cs
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
    /// 
    /// </summary>
    public class VertexCluster
    {
        #region [ Private Members ]

        private List<int> m_vertices;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// The list of integer vertices.
        /// </summary>
        public List<int> Vertices
        {
            get
            {
                return m_vertices;
            }
            set
            {
                m_vertices = value;
            }
        }

        #endregion

        #region [ Constructor ]

        /// <summary>
        /// A blank constructor with default values
        /// </summary>
        public VertexCluster()
            :this(new List<int>())
        {
        }

        /// <summary>
        /// Constructs a new instance of the <see cref="LinearStateEstimator.Graphs.VertexCluster"/> class with a single integer vertex.
        /// </summary>
        /// <param name="vertex">A unique integer vertex.</param>
        public VertexCluster(int vertex)
        {
            m_vertices = new List<int>();
            m_vertices.Add(vertex);
        }

        /// <summary>
        /// The designated constructor for the <see cref="LinearStateEstimator.Graphs.VertexCluster"/> class. Constructs a <see cref="LinearStateEstimator.Graphs.VertexCluster"/> with a list of integer vertices.
        /// </summary>
        /// <param name="vertices"></param>
        public VertexCluster(List<int> vertices)
        {
            m_vertices = vertices;
        }

        #endregion

        #region [ Public Methods ]

        /// <summary>
        /// A string representation of the instance of <see cref="LinearStateEstimator.Graphs.VertexCluster"/>.
        /// </summary>
        /// <returns>A string representation of the instance of <see cref="LinearStateEstimator.Graphs.VertexCluster"/>.</returns>
        public override string ToString()
        {
            string cluster = "";

            foreach (int vertex in m_vertices)
            {
                cluster += vertex.ToString() + ",";
            }

            if (cluster.Length > 1)
            {
                cluster = cluster.Substring(0, cluster.Length - 1);
                return "(" + cluster + ")";
            }
            else
            {
                return "(null)";
            }
        }

        /// <summary>
        /// Overridden to prevent compilation warnings.
        /// </summary>
        /// <returns>An integer hash code determined by the base class.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Determines equality between two <see cref="LinearStateEstimator.Graphs.VertexCluster"/> objects by checking the contens of the <see cref="VertexCluster.Vertices"/> property to make sure that the source and target both contain the same values.
        /// </summary>
        /// <param name="target">The object to compare with.</param>
        /// <returns>A boolen flag indicating that the object is equal to the other object or not.</returns>
        public override bool Equals(object target)
        {
            // If parameter is null return false.
            if (target == null)
            {
                return false;
            }

            // If parameter cannot be cast to VertexCluster return false.
            VertexCluster vertexCluster = target as VertexCluster;

            if ((object)vertexCluster == null)
            {
                return false;
            }

            if (m_vertices.Count != vertexCluster.Vertices.Count)
            {
                return false;
            }

            foreach (int vertex in m_vertices)
            {
                if (!vertexCluster.Vertices.Contains(vertex))
                {
                    return false;
                }
            }

            foreach (int vertex in vertexCluster.Vertices)
            {
                if (!m_vertices.Contains(vertex))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Merges the target <see cref="LinearStateEstimator.Graphs.VertexCluster"/> with this instance of <see cref="LinearStateEstimator.Graphs.VertexCluster"/>
        /// </summary>
        /// <param name="cluster">The target <see cref="LinearStateEstimator.Graphs.VertexCluster"/> to merge with.</param>
        public void MergeWith(VertexCluster cluster)
        {
            foreach (int vertex in cluster.Vertices)
            {
                m_vertices.Add(vertex);
            }
        }

        /// <summary>
        /// Merges two <see cref="LinearStateEstimator.Graphs.VertexCluster"/> and returns the result.
        /// </summary>
        /// <param name="cluster">The <see cref="LinearStateEstimator.Graphs.VertexCluster"/> to merge with.</param>
        /// <returns>A <see cref="LinearStateEstimator.Graphs.VertexCluster"/> which is a union set of the source and target <see cref="LinearStateEstimator.Graphs.VertexCluster"/> objects.</returns>
        public VertexCluster MergeToFormNewCluster(VertexCluster cluster)
        {
            this.MergeWith(cluster);
            return this;
        }

        /// <summary>
        /// Sorts the vertices in ascending integer order.
        /// </summary>
        public void Sort()
        {
            m_vertices.Sort();
        }

        /// <summary>
        /// Removes duplicate <see cref="Vertices"/> from the <see cref="LinearStateEstimator.Graphs.VertexCluster"/>
        /// </summary>
        public void RemoveDuplicateVertices()
        {
            m_vertices = m_vertices.Distinct().ToList();
        }

        #endregion

    }
}
