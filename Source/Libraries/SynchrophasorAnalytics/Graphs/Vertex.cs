//******************************************************************************************************
//  Vertex.cs
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
    /// An encapsulation of the vertex elements in the graph representation of the network elements in an electric power network.
    /// </summary>
    /// <typeparam name="Node">Any <see cref="LinearStateEstimator.Modeling.Node"/> network element.</typeparam>
    public class Vertex<Node>
    {
        #region [ Private Members ]

        private Node m_node;
        private List<Edge<ITwoTerminal>> m_edges;

        #endregion

        #region [ Properties ]

        public Node Value
        {
            get
            {
                return m_node;
            }
            set
            {
                m_node = value;
            }
        }

        public List<Edge<ITwoTerminal>> Edges
        {
            get
            {
                return m_edges;
            }
            set
            {
                m_edges = value;
            }
        }

        #endregion

        #region [ Constructors ]

        public Vertex() 
        {
        }

        public Vertex(Node data) 
            :this(data, null) 
        {
        }

        public Vertex(Node data, List<Edge<ITwoTerminal>> edges)
        {
            this.m_node = data;
            this.m_edges = edges;
        }

        #endregion

    }
}

