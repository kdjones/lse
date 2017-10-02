//******************************************************************************************************
//  TreeNode.cs
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
using SynchrophasorAnalytics.Modeling;

namespace SynchrophasorAnalytics.Graphs
{
    /// <summary>
    /// A tree node
    /// </summary>
    /// <typeparam name="T">A generic type</typeparam>
    public class TreeNode<T>
    {
        #region [ Private Fields ]

        private T m_value;
        private TreeNode<T> m_parent;
        private TreeNodeList<T> m_children;
        private int m_dividerIndexMaximum;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// The number of branches from the root node to this node.
        /// </summary>
        public int Depth
        {
            get
            {
                int depth = 0;
                TreeNode<T> node = this;
                while (node.Parent != null)
                {
                    node = node.Parent;
                    depth++;
                }
                return depth;
            }
        }

        /// <summary>
        /// The value held by the tree node.
        /// </summary>
        public T Value
        {
            get
            {
                return m_value;
            }        
            set
            {
                m_value = value;
            }
        }

        /// <summary>
        /// The parent of the tree node.
        /// </summary>
        public TreeNode<T> Parent
        {
            get
            {
                return m_parent;
            }
            set
            {
                if (value == m_parent)
                {
                    return;
                }

                if (m_parent != null)
                {
                    m_parent.Children.Remove(this);
                }

                if (value != null && !value.Children.Contains(this))
                {
                    value.Children.Add(this);
                }

                m_parent = value;
            }
        }

        /// <summary>
        /// The children of the tree node.
        /// </summary>
        public TreeNodeList<T> Children
        {
            get
            {
                return m_children;
            }
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// The default constructor for the class.
        /// </summary>
        /// <param name="value">The value for the tree node to hold.</param>
        public TreeNode(T value)
        {
            m_value = value;
            m_parent = null;
            m_children = new TreeNodeList<T>(this);
        }

        /// <summary>
        /// The designated constructor for the tree node class.
        /// </summary>
        /// <param name="value">The value for the tree node to hold.</param>
        /// <param name="parent">The parent of the tree node.</param>
        public TreeNode(T value, TreeNode<T> parent)
        {
            m_value = value;
            m_parent = parent;
            m_children = new TreeNodeList<T>(this);
        }

        #endregion

        #region [ Public Methods ]

        /// <summary>
        /// Searches the tree to find the subtree of this node.
        /// </summary>
        /// <returns>A list of all of the nodes in the subtree of this node.</returns>
        public IEnumerable<TreeNode<T>> GetNodeAndAllSubtreeNodes()
        {
            return new[] { this }.Concat(m_children.SelectMany(child => child.GetNodeAndAllSubtreeNodes()));
        }

        /// <summary>
        /// A string representation of the tree node.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (TreeNode<T> child in m_children)
            {
                stringBuilder.Append(child.Value.ToString() + ",");
            }

            if (m_children.Count > 0)
            {
                return m_value.ToString() + " | " + stringBuilder.ToString().TrimEnd(',');
            }
            else
            {
                return m_value.ToString() + " | (null)";
            }
        }

        /// <summary>
        /// A string representation of the subtree of the tree node.
        /// </summary>
        /// <returns></returns>
        public string ToSubtreeString()
        {
            List<TreeNode<T>> nodeAndAllSubtreeNodes = GetNodeAndAllSubtreeNodes().ToList();

            if (nodeAndAllSubtreeNodes != null && nodeAndAllSubtreeNodes.Count > 0)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine(" --------- Transmission Line Tree List --------- ");
                stringBuilder.AppendLine("   Format => (Parent) | (Child 1),...,(Child n)");
                List<int> dividerIndices = new List<int>();

                foreach (TreeNode<T> node in nodeAndAllSubtreeNodes)
                {
                    dividerIndices.Add(node.ToString().IndexOf('|'));
                }

                int dividerIndexMaximum = dividerIndices.Max();

                foreach (TreeNode<T> node in nodeAndAllSubtreeNodes)
                {
                    int lengthOfWhiteSpaceToAppend = dividerIndexMaximum - node.ToString().IndexOf('|');

                    string whiteSpaceToAppend = "";

                    for (int i = 0; i < lengthOfWhiteSpaceToAppend + 1; i++)
                    {
                        whiteSpaceToAppend += " ";
                    }

                    stringBuilder.AppendLine(whiteSpaceToAppend + node.ToString());
                }
                return stringBuilder.ToString();
            }
            else
            {
                return "(null set)";
            }
            
        }

        #endregion

        #region [ Private Methods ]

        private string GetChildrenStrings(TreeNode<T> parentNode)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (TreeNode<T> childNode in parentNode.Children)
            {
                string childNodeString = childNode.ToString();
                int dividerIndex = childNodeString.IndexOf('|');
                if (m_dividerIndexMaximum < dividerIndex)
                {
                    m_dividerIndexMaximum = dividerIndex;
                }
                stringBuilder.AppendLine(childNode.ToString());
                stringBuilder.AppendLine(GetChildrenStrings(childNode));
            }
            return stringBuilder.ToString();
        }

        #endregion
    }
}
