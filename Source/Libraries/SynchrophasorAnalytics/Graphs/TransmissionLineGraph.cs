//******************************************************************************************************
//  TransmissionLineGraph.cs
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
//  06/25/2014 - Kevin D. Jones
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
    /// A class which represents a graph representation of the network elements in a <see cref="LinearStateEstimator.Modeling.TransmissionLine"/>.
    /// </summary>
    public class TransmissionLineGraph
    {
        #region [ Private Members ]

        /// <summary>
        /// Transmission Line to represent as a graph
        /// </summary>
        private TransmissionLine m_transmissionLine;

        /// <summary>
        /// Components of the transmission line graph
        /// </summary>
        private List<Node> m_vertexSet;
        private List<ITwoTerminal> m_edgeSet;

        /// <summary>
        /// Representations of the transmission line as a tree and two types of adjacency lists
        /// </summary>
        private TreeNode<VertexCluster> m_rootNode;
        private VertexAdjacencyList m_directlyConnectedAdjacencyList;
        private VertexAdjacencyList m_seriesImpedanceConnectedAdjacencyList;

        /// <summary>
        /// A boolean flag for indicating whether certain operations are allowed
        /// </summary>
        private bool m_adjacenciesHaveBeenResolved = false;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// The <see cref="LinearStateEstimator.Modeling.INetworkDescribable.InternalID"/> of the associated <see cref="LinearStateEstimator.Modeling.TransmissionLine"/>.
        /// </summary>
        public int InternalId
        {
            get
            {
                return m_transmissionLine.InternalID;
            }
        }

        /// <summary>
        /// The <see cref="LinearStateEstimator.Graphs.VertexCluster"/> which contains the internal id of the <see cref="LinearStateEstimator.Modeling.TransmissionLine.FromNode"/>.
        /// </summary>
        public VertexCluster FromVertex
        {
            get
            {
                foreach (VertexAdjacencyRow row in m_directlyConnectedAdjacencyList.Rows)
                {
                    if (row.Header.Vertices.Contains(m_transmissionLine.FromNode.InternalID))
                    {
                        return row.Header;
                    }
                }
                return new VertexCluster();
            }
        }

        /// <summary>
        /// The <see cref="LinearStateEstimator.Graphs.VertexCluster"/>  which contains the internal id of the <see cref="LinearStateEstimator.Modeling.TransmissionLine.ToNode"/>.
        /// </summary>
        public VertexCluster ToVertex
        {
            get
            {
                foreach (VertexAdjacencyRow row in m_directlyConnectedAdjacencyList.Rows)
                {
                    if (row.Header.Vertices.Contains(m_transmissionLine.ToNode.InternalID))
                    {
                        return row.Header;
                    }
                }
                return new VertexCluster();
            }
        }

        /// <summary>
        /// The root node of the tree representation of the <see cref="LinearStateEstimator.Modeling.TransmissionLine"/>.
        /// </summary>
        public TreeNode<VertexCluster> RootNode
        {
            get
            {
                return m_rootNode;
            }
        }

        /// <summary>
        /// The adjacency list which represents nodal connections resolved by the presence of directly connected adjacencies such as a closed logical device.
        /// </summary>
        public VertexAdjacencyList DirectlyConnectedAdjacencyList
        {
            get
            {
                return m_directlyConnectedAdjacencyList;
            }
        }

        /// <summary>
        /// The adjacency list which represents nodal connections resolved by the presence of any series or directly connected adjacency except for open logical devices.
        /// </summary>
        public VertexAdjacencyList SeriesImpedanceConnectedAdjacencyList
        {
            get
            {
                return m_seriesImpedanceConnectedAdjacencyList;
            }
        }

        /// <summary>
        /// The list of <see cref="SeriesBranchBase"/> components which form one continuous series connection from the <see cref="LinearStateEstimator.Modeling.TransmissionLine.FromNode"/> and the <see cref="LinearStateEstimator.Modeling.TransmissionLine.ToNode"/>.
        /// </summary>
        public List<SeriesBranchBase> SingleFlowPathBranches
        {
            get
            {
                return GetSingleFlowPathBranches();
            }
        }

        /// <summary>
        /// A boolean flag which indicates whether the <see cref="LinearStateEstimator.Modeling.TransmissionLine"/> has at least one series flow path between the <see cref="LinearStateEstimator.Modeling.TransmissionLine.FromNode"/> and the <see cref="LinearStateEstimator.Modeling.TransmissionLine.ToNode"/>.
        /// </summary>
        public bool HasAtLeastOneFlowPath
        {
            get
            {
                return CheckForAtLeastOneFlowPath();
            }
        }
        
        #endregion

        #region [ Constructor ]

        /// <summary>
        /// The designated constructor for the <see cref="LinearStateEstimator.Graphs.TransmissionLineGraph"/> class. Requires a reference to a <see cref="LinearStateEstimator.Modeling.TransmissionLine"/> of interest.
        /// </summary>
        /// <param name="transmissionLine">The <see cref="LinearStateEstimator.Modeling.TransmissionLine"/> desired to represent as a graph.</param>
        public TransmissionLineGraph(TransmissionLine transmissionLine)
        {
            m_transmissionLine = transmissionLine;
            BuildVertexSet(transmissionLine);
            BuildEdgeSet(transmissionLine);
        }

        #endregion

        #region [ Public Methods ]

        public void ResolveAdjacencies()
        {

        }

        /// <summary>
        /// Initializes the two adjacency lists representing the graph. One adjacency list can resolve directly connected adjacencies such as switches and circuit breakers. The second adjacency list can resolve any series connectin adjacency.
        /// </summary>
        public void InitializeAdjacencyLists()
        {
            InitializeDirectlyConnectedAdjacencyList();
            InitializeSeriesImpedanceAdjacencyList();
        }

        /// <summary>
        /// Resolves adjacency connections for both adjacency lists.
        /// </summary>
        public void ResolveConnectedAdjacencies()
        {
            ResolveDirectlyConnectedAdjacencies();
            ResolveSeriesImpedanceConnectedAdjacencies();
            AcknowledgeAdjacencyResolution();
        }

        /// <summary>
        /// Initializes the tree only after the adjacencies have been resolved.
        /// </summary>
        public void InitializeTree()
        {
            if (m_adjacenciesHaveBeenResolved)
            {
                foreach (VertexAdjacencyRow row in m_directlyConnectedAdjacencyList.Rows)
                {
                    if (row.Header.Vertices.Contains(m_transmissionLine.FromNodeID))
                    {
                        m_rootNode = new TreeNode<VertexCluster>(row.Header, null);
                    }
                }

                AppendChildrenNodes(m_rootNode);
            }
        }

        /// <summary>
        /// Finds and returns the edge which connects to vertices.
        /// </summary>
        /// <param name="fromVertexCluster">The vertex for the from side of the potential edge.</param>
        /// <param name="toVertexCluster">The vertex for the to side of the potential edge.</param>
        /// <returns>The edge which connects to two vertices.</returns>
        public List<ITwoTerminal> ConnectingEdgeBetween(VertexCluster fromVertexCluster, VertexCluster toVertexCluster)
        {
            List<ITwoTerminal> seriesBranches = new List<ITwoTerminal>();

            foreach (ITwoTerminal seriesBranch in m_edgeSet)
            {
                foreach (int fromVertex in fromVertexCluster.Vertices)
                {
                    foreach (int toVertex in toVertexCluster.Vertices)
                    {
                        if (seriesBranch.FromNode.InternalID == fromVertex && seriesBranch.ToNode.InternalID == toVertex)
                        {
                            seriesBranches.Add(seriesBranch);
                        }
                        else if (seriesBranch.FromNode.InternalID == toVertex && seriesBranch.ToNode.InternalID == fromVertex)
                        {
                            seriesBranches.Add(seriesBranch);
                        }
                    }
                }
            }

            return seriesBranches;
        }

        /// <summary>
        /// FOR TROUBLESHOOTING PURPOSES ONLY
        /// </summary>
        /// <returns>A single <see cref="SeriesBranchBase"/> which is a lumped impedance of the <see cref="LinearStateEstimator.Graphs.TransmissionLineGraph.SingleFlowPathBranches"/>.</returns>
        public SeriesBranchBase ResolveToSingleSeriesBranch()
        {
            Impedance equivalentSeriesImpedance = new Impedance();
            
            if (this.HasAtLeastOneFlowPath)
            {
                Impedance lumpedImpedance = new Impedance();

                foreach (SeriesBranchBase seriesBranch in SingleFlowPathBranches)
                {
                    if ((seriesBranch is SeriesCompensator))
                    {
                        if ((seriesBranch as SeriesCompensator).Status == SeriesCompensatorStatus.Energized)
                        {
                            equivalentSeriesImpedance += seriesBranch.RawImpedanceParameters;
                        }
                    }
                    else if (seriesBranch is LineSegment)
                    {
                        equivalentSeriesImpedance += seriesBranch.RawImpedanceParameters;
                    }
                }

            }

            return new SeriesBranchBase()
            {
                InternalID = m_transmissionLine.InternalID,
                Number = m_transmissionLine.Number,
                Name = "Virtual Branch for " + m_transmissionLine.Name,
                Description = "Virtual Branch for " + m_transmissionLine.Name,
                FromNode = m_transmissionLine.FromNode,
                ToNode = m_transmissionLine.ToNode,
                RawImpedanceParameters = equivalentSeriesImpedance
            };
        }

        #endregion

        #region [ Private Methods ]

        private void BuildVertexSet(TransmissionLine transmissionLine)
        {
            m_vertexSet = new List<Node>();

            foreach (Node node in transmissionLine.Nodes)
            {
                m_vertexSet.Add(node);
            }

            // The FromNode and ToNode are children of the FromSubstation and ToSubstation respectively but should be included in this vertex set.
            m_vertexSet.Add(transmissionLine.FromNode);
            m_vertexSet.Add(transmissionLine.ToNode);
        }

        private void BuildEdgeSet(TransmissionLine transmissionLine)
        {
            m_edgeSet = new List<ITwoTerminal>();

            foreach (LineSegment lineSegment in transmissionLine.LineSegments)
            {
                m_edgeSet.Add(lineSegment);
            }

            foreach (Switch circuitSwitch in transmissionLine.Switches)
            {
                m_edgeSet.Add(circuitSwitch);
            }

            foreach (SeriesCompensator seriesCompensationDevice in transmissionLine.SeriesCompensators)
            {
                m_edgeSet.Add(seriesCompensationDevice);
            }
        }

        private void InitializeDirectlyConnectedAdjacencyList()
        {
            m_directlyConnectedAdjacencyList = new VertexAdjacencyList();
            m_seriesImpedanceConnectedAdjacencyList = new VertexAdjacencyList();

            List<VertexCluster> vertexClusters = new List<VertexCluster>();
            List<VertexAdjacencyRow> adjacencyRows = new List<VertexAdjacencyRow>();

            foreach (Node node in m_vertexSet)
            {
                VertexCluster vertexCluster = new VertexCluster(node.InternalID);
                List<VertexCluster> vertexAdjacencies = new List<VertexCluster>();

                foreach (ITwoTerminal seriesBranch in m_edgeSet)
                {
                    if (node.InternalID == seriesBranch.FromNode.InternalID)
                    {
                        vertexAdjacencies.Add(new VertexCluster(seriesBranch.ToNode.InternalID));
                    }
                    else if (node.InternalID == seriesBranch.ToNode.InternalID)
                    {
                        vertexAdjacencies.Add(new VertexCluster(seriesBranch.FromNode.InternalID));
                    }
                }

                m_directlyConnectedAdjacencyList.Rows.Add(new VertexAdjacencyRow(vertexCluster, vertexAdjacencies));
                m_seriesImpedanceConnectedAdjacencyList.Rows.Add(new VertexAdjacencyRow(vertexCluster, vertexAdjacencies));
            }
        }

        private void InitializeSeriesImpedanceAdjacencyList()
        {
            m_seriesImpedanceConnectedAdjacencyList = new VertexAdjacencyList();

            List<VertexCluster> vertexClusters = new List<VertexCluster>();
            List<VertexAdjacencyRow> adjacencyRows = new List<VertexAdjacencyRow>();

            foreach (Node node in m_vertexSet)
            {
                VertexCluster vertexCluster = new VertexCluster(node.InternalID);
                List<VertexCluster> vertexAdjacencies = new List<VertexCluster>();

                foreach (ITwoTerminal seriesBranch in m_edgeSet)
                {
                    if (node.InternalID == seriesBranch.FromNode.InternalID)
                    {
                        vertexAdjacencies.Add(new VertexCluster(seriesBranch.ToNode.InternalID));
                    }
                    else if (node.InternalID == seriesBranch.ToNode.InternalID)
                    {
                        vertexAdjacencies.Add(new VertexCluster(seriesBranch.FromNode.InternalID));
                    }
                }

                m_seriesImpedanceConnectedAdjacencyList.Rows.Add(new VertexAdjacencyRow(vertexCluster, vertexAdjacencies));
            }
        }

        private void ResolveDirectlyConnectedAdjacencies()
        {
            while (ResolvingDirectlyConnectedAdjacencies()) { }
        }

        private bool ResolvingDirectlyConnectedAdjacencies()
        {
            foreach (VertexAdjacencyRow row in m_directlyConnectedAdjacencyList.Rows)
            {
                foreach (VertexCluster adjacency in row.Adjacencies)
                {
                    List<ITwoTerminal> connectingEdges = ConnectingEdgeBetween(row.Header, adjacency);

                    foreach (ITwoTerminal connectingEdge in connectingEdges)
                    {
                        if (connectingEdge is Switch)
                        {
                            if (((Switch)connectingEdge).IsClosed)
                            {
                                ConnectionEstablished(m_directlyConnectedAdjacencyList, row.Header, adjacency);
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        private void ResolveSeriesImpedanceConnectedAdjacencies()
        {
            while (ResolvingSeriesImpedanceConnectedAdjacencies()) { };
        }

        private bool ResolvingSeriesImpedanceConnectedAdjacencies()
        {
            foreach (VertexAdjacencyRow row in m_seriesImpedanceConnectedAdjacencyList.Rows)
            {
                foreach (VertexCluster adjacency in row.Adjacencies)
                {
                    List<ITwoTerminal> connectingEdges = ConnectingEdgeBetween(row.Header, adjacency);

                    foreach (ITwoTerminal connectingEdge in connectingEdges)
                    {
                        if (connectingEdge is Switch)
                        {
                            if (((Switch)connectingEdge).IsClosed)
                            {
                                ConnectionEstablished(m_seriesImpedanceConnectedAdjacencyList, row.Header, adjacency);
                                return true;
                            }
                        }
                        else if (connectingEdge is SeriesBranchBase)
                        {
                            ConnectionEstablished(m_seriesImpedanceConnectedAdjacencyList, row.Header, adjacency);
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private void ConnectionEstablished(VertexAdjacencyList adjacencyList, VertexCluster fromVertexCluster, VertexCluster toVertexCluster)
        {
            List<int> fromVertices = new List<int>();
            List<int> toVertices = new List<int>();

            foreach (int vertex in fromVertexCluster.Vertices)
            {
                fromVertices.Add(vertex);
            }

            foreach (int vertex in toVertexCluster.Vertices)
            {
                toVertices.Add(vertex);
            }

            VertexCluster fromCluster = new VertexCluster(fromVertices);
            VertexCluster toCluster = new VertexCluster(toVertices);

            VertexAdjacencyRow source = adjacencyList.RowWithHeader(fromVertexCluster);
            VertexAdjacencyRow target = adjacencyList.RowWithHeader(toVertexCluster);

            // Merge the two rows into one
            source.MergeWith(target);

            // Remove the old from the list
            adjacencyList.RemoveRow(target);

            // Update the vertices in the rest of the table
            foreach (VertexAdjacencyRow row in adjacencyList.Rows)
            {
                foreach (VertexCluster vertexCluster in row.Adjacencies)
                {
                    if (vertexCluster.Equals(fromCluster) || vertexCluster.Equals(toCluster))
                    {
                        vertexCluster.Vertices = source.Header.Vertices;
                    }
                }
                row.RemoveDuplicateVertexClusters();
            }
        }

        private void AcknowledgeAdjacencyResolution()
        {
            m_adjacenciesHaveBeenResolved = true;
        }

        private void AppendChildrenNodes(TreeNode<VertexCluster> parentNode)
        {
            foreach (VertexAdjacencyRow row in m_directlyConnectedAdjacencyList.Rows)
            {
                if (row.Header.Equals(parentNode.Value))
                {
                    foreach (VertexCluster adjacency in row.Adjacencies)
                    {
                        if ((parentNode.Parent == null || !AlreadyContainsNode(adjacency)))
                        {
                            parentNode.Children.Add(adjacency);
                        }
                    }
                }
            }

            foreach (TreeNode<VertexCluster> childNode in parentNode.Children)
            {
                AppendChildrenNodes(childNode);
            }
        }

        private bool AlreadyContainsNode(VertexCluster vertex)
        {
            return HasChildOrDescendantNode(m_rootNode, vertex);
        }

        private bool HasChildOrDescendantNode(TreeNode<VertexCluster> currentNode, VertexCluster vertex)
        {
            if (currentNode.Value.Equals(vertex))
            { 
                return true;
            }
            else
            {
                foreach (TreeNode<VertexCluster> childNode in currentNode.Children)
                {
                    if (childNode.Value.Equals(vertex))
                    {
                        return true;
                    }
                    else
                    {
                        return HasChildOrDescendantNode(childNode, vertex);
                    }
                }
            }
            return false;
        }

        private List<SeriesBranchBase> GetSingleFlowPathBranches()
        {
            if (this.HasAtLeastOneFlowPath)
            {
                List<SeriesBranchBase> singleFlowPathBranches = new List<SeriesBranchBase>();
                TreeNode<VertexCluster> toTreeNode = null;
                foreach (VertexAdjacencyRow row in m_directlyConnectedAdjacencyList.Rows)
                {
                    if (row.Header.Vertices.Contains(m_transmissionLine.ToNodeID))
                    {
                        toTreeNode = FindNodeInTreeWithValue(row.Header);
                    }
                }
                List<ITwoTerminal> pathToRoot = FindPathToRoot(toTreeNode);

                return pathToRoot.OfType<SeriesBranchBase>().ToList();
            }
            else
            {
                return new List<SeriesBranchBase>();
            }
        }

        private TreeNode<VertexCluster> FindNodeInTreeWithValue(VertexCluster value)
        {
            return m_rootNode.GetNodeAndAllSubtreeNodes().FirstOrDefault(node => node.Value.Equals(value));
        }

        private List<ITwoTerminal> FindPathToRoot(TreeNode<VertexCluster> treeNode)
        {
            List<ITwoTerminal> edgePath = new List<ITwoTerminal>();
            TreeNode<VertexCluster> currentNode = treeNode;
            while (currentNode.Parent != null)
            {
                List<ITwoTerminal> connectingEdges = ConnectingEdgeBetween(currentNode.Value, currentNode.Parent.Value);
                foreach (ITwoTerminal seriesBranch in connectingEdges)
                {
                    if (seriesBranch is SeriesBranchBase)
                    {
                        edgePath.Add(seriesBranch as SeriesBranchBase);
                    }
                }
                currentNode = currentNode.Parent;
            }
            return edgePath;
        }

        private bool CheckForAtLeastOneFlowPath()
        {
            foreach (VertexAdjacencyRow row in m_seriesImpedanceConnectedAdjacencyList.Rows)
            {
                if (row.Header.Vertices.Contains(m_transmissionLine.FromNodeID) && row.Header.Vertices.Contains(m_transmissionLine.ToNodeID))
                {
                    return true;
                }
            }
            return false;
        }

        #endregion
    }
}
