//******************************************************************************************************
//  Tree.cs
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
using System.Threading.Tasks;

namespace LinearStateEstimator.Graph
{
    public class Tree
    {
        private TreeNode m_root;

        public TreeNode Root
        {
            get
            {
                return m_root;
            }
        }

        public Tree()
            :this(new TreeNode())
        {
        }

        public Tree(TreeNode root)
        {
            m_root = root;
        }

        public bool ContainsChild(VertexCluster vertexCluster)
        {
            return false;
        }
    }
}
