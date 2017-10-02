using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynchrophasorAnalytics.Graphs
{
    /// <summary>
    /// Represents a collection of children tree nodes.
    /// </summary>
    /// <typeparam name="T">A generic type.</typeparam>
    public class TreeNodeList<T> : List<TreeNode<T>>
    {
        #region [ Private Members ]

        private TreeNode<T> m_parent;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// The parent tree node of the children in this collection.
        /// </summary>
        public TreeNode<T> Parent
        {
            get
            {
                return m_parent;
            }
            set
            {
                m_parent = value;
            }
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// The default constructor for the class
        /// </summary>
        /// <param name="parent">The parent tree node of the children in this collection.</param>
        public TreeNodeList(TreeNode<T> parent)
        {
            m_parent = parent;
        }

        #endregion

        #region [ Public Methods ]

        /// <summary>
        /// Adds another child node.
        /// </summary>
        /// <param name="node">The child noe to add.</param>
        /// <returns>The same node</returns>
        public new TreeNode<T> Add(TreeNode<T> node)
        {
            base.Add(node);
            node.Parent = m_parent;
            return node;
        }

        /// <summary>
        /// Adds another child node
        /// </summary>
        /// <param name="value">The child node to add</param>
        /// <returns>The same node.</returns>
        public TreeNode<T> Add(T value)
        {
            return Add(new TreeNode<T>(value));
        }

        /// <summary>
        /// A string representation of the child tree nodes. Shows the number of nodes.
        /// </summary>
        /// <returns>"Count = 'numberOfNodes';"</returns>
        public override string ToString()
        {
            return "Count = " + Count.ToString() + ";";
        }

        #endregion
    }
}
