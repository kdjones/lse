//******************************************************************************************************
//  Edge.cs
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
//  06/14/2014 - Kevin D. Jones
//       Updated XML inline documentation.
//
//******************************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinearStateEstimator.Modeling;

namespace LinearStateEstimator.Graph
{
    /// <summary>
    /// An encapsulation of the edge elements in the graph representation of the electric power network.
    /// </summary>
    /// <typeparam name="ITwoTerminal">Any <see cref="LinearStateEstimator.Modeling.ITwoTerminal"/> network element.</typeparam>
    public class Edge<ITwoTerminal>
    {
        #region [ Private Members ]

        private Vertex<Node> m_fromVertex;
        private Vertex<Node> m_toVertex;
        private ITwoTerminal m_edge;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// The edge object reference
        /// </summary>
        public ITwoTerminal Value
        {
            get
            {
                return m_edge;
            }
            set
            {
                m_edge = value;
            }
        }

        /// <summary>
        /// The vertex object reference to the from side of the edge.
        /// </summary>
        public Vertex<Node> FromVertex
        {
            get
            {
                return m_fromVertex;
            }
            set
            {
                m_fromVertex = value;
            }
        }

        /// <summary>
        /// The vertex objet reference to the to side of the edge.
        /// </summary>
        public Vertex<Node> ToVertex
        {
            get
            {
                return m_toVertex;
            }
            set
            {
                m_toVertex = value;
            }
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// The designated constructor for the <see cref="LinearStateEstimator.Graph.Edge"/> class. Requires a reference to the edge object and the two vertices.
        /// </summary>
        /// <param name="edge">The <see cref="LinearStateEstimator.Modeling.ITwoTerminal"/> edge object.</param>
        /// <param name="fromVertex">The vertex object reference to the from side of the edge.</param>
        /// <param name="toVertex">The vertex objet reference to the to side of the edge.</param>
        public Edge(ITwoTerminal edge, Vertex<Node> fromVertex, Vertex<Node> toVertex)
        {
            m_edge = edge;
            m_fromVertex = fromVertex;
            m_toVertex = toVertex;
        }

        #endregion
    }
}
