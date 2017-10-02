//******************************************************************************************************
//  NetworkGraph.cs
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
using LinearStateEstimator.Modeling;

namespace LinearStateEstimator.Graph
{
    public class NetworkGraph
    {
        private List<Vertex<Node>> m_vertexSet;

        public List<Vertex<Node>> VertexSet
        {
            get
            {
                return m_vertexSet;
            }
            set
            {
                m_vertexSet = value;
            }
        }

        public Vertex<Node> FindByValue(Node node)
        {
            // search the list for the value
            foreach (Vertex<Node> vertex in m_vertexSet)
            {
                if (vertex.Value.Equals(node))
                {
                    return vertex;
                }
            }

            // if we reached here, we didn't find a matching node
            return null;
        }

        public bool Contains(Node node)
        {
            return FindByValue(node) != null;
        }
    }
}
