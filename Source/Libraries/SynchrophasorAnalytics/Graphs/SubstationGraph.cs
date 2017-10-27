//******************************************************************************************************
//  SubstationGraph.cs
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
//  07/02/2013 - Kevin D. Jones
//       Fixed error in casting switching devices when resolving observed busses.
//  06/14/2014 - Kevin D. Jones
//       Updated XML inline documentation.
//
//******************************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SynchrophasorAnalytics.Modeling;
using SynchrophasorAnalytics.Measurements;

namespace SynchrophasorAnalytics.Graphs
{
    /// <summary>
    /// A class which represents a graph representation of the network elements in a <see cref="LinearStateEstimator.Modeling.Substation"/>.
    /// </summary>
    public class SubstationGraph
    {
        #region [ Private Members ]

        private int m_internalID;
        private double m_angleDeltaThresholdInDegrees;
        private List<Node> m_vertexSet;
        private List<Node> m_expectedVertexSet;
        private List<SwitchingDeviceBase> m_edgeSet;
        private VertexAdjacencyList m_adjacencyList;
        private List<ObservedBus> m_observedBuses;
        private TopologyEstimationLevel m_topologyLevel;
        private bool m_topologyErrorDetected;
        private PhasorPair[,] m_phasorPairMatrix;
        private bool[,] m_pastConnectivityMatrix;

        #endregion 

        #region [ Properties ]

        public bool TopologyErrorDetected
        {
            get
            {
                return m_topologyErrorDetected;
            }
            set
            {
                m_topologyErrorDetected = value;
            }
        }

        /// <summary>
        /// The set of <see cref="LinearStateEstimator.Modeling.Node"/> vertices.
        /// </summary>
        public List<Node> VertexSet
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

        /// <summary>
        /// The set of <see cref="LinearStateEstimator.Modeling.SwitchingDeviceBase"/> edges.
        /// </summary>
        public List<SwitchingDeviceBase> EdgeSet
        {
            get
            {
                return m_edgeSet;
            }
            set
            {
                m_edgeSet = value;
            }
        }

        /// <summary>
        /// The adjacency list representation of the vertices and edges.
        /// </summary>
        public VertexAdjacencyList AdjacencyList
        {
            get
            {
                return m_adjacencyList;
            }
            set
            {
                m_adjacencyList = value;
            }
        }

        public List<ObservedBus> ObservedBuses
        {
            get
            {
                return m_observedBuses;
            }
        }

        public TopologyEstimationLevel TopologyLevel
        {
            get
            {
                return m_topologyLevel;
            }
            set
            {
                m_topologyLevel = value;
            }
        }

        public double AngleDeltaThresholdInDegrees
        {
            get
            {
                return m_angleDeltaThresholdInDegrees;
            }
            set
            {
                m_angleDeltaThresholdInDegrees = value;
            }
        }

        public List<Node> ExpectedVertexSet
        {
            get
            {
                if (m_expectedVertexSet == null)
                {
                    m_expectedVertexSet = VertexSet.FindAll(x => x.Voltage.ExpectsMeasurements);
                }
                return m_expectedVertexSet;
            }
        }

        public PhasorPair[,] PhasorPairMatrix
        {
            get
            {
                return m_phasorPairMatrix;
            }
        }

        public bool[,] PastConnectivityMatrix
        {
            get
            {
                return m_pastConnectivityMatrix;
            }
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// The designated constructor for the <see cref="LinearStateEstimator.Graphs.SubstationGraph"/> class. Requires a reference to a <see cref="LinearStateEstimator.Modeling.Substation"/> of interest.
        /// </summary>
        /// <param name="substation">The <see cref="LinearStateEstimator.Modeling.Substation"/> desired to represent as a graph.</param>
        public SubstationGraph(Substation substation)
        {
            m_internalID = substation.InternalID;
            m_topologyLevel = substation.TopologyLevel;
            m_angleDeltaThresholdInDegrees = substation.AngleDeltaThresholdInDegrees;
            BuildVertexSet(substation);
            BuildeEdgeSet(substation);
            InitializeAdjacencyList();
            m_observedBuses = new List<ObservedBus>();
            CreatePhasorPairMatrix();
            InitializeConnectivityMatrix();
        }

        #endregion

        #region [ Public Methods ]

        public void ResolveAdjacencies()
        {
            TopologyErrorDetected = false;

            if (TopologyLevel == TopologyEstimationLevel.Zero)
            {
                ExecuteLevelZeroAdjacencyResolution();
            }
            else if (TopologyLevel == TopologyEstimationLevel.One)
            {
                ExecuteLevelOneAdjacencyResolution();
            }
            else if (TopologyLevel == TopologyEstimationLevel.Two)
            {
                ExecuteLevelTwoAdjacencyResolution();
            }
            else if (TopologyLevel == TopologyEstimationLevel.Three)
            {
                ExecuteLevelThreeAdjacencyResolution();
            }
            else if (TopologyLevel == TopologyEstimationLevel.Four)
            {
                ExecuteLevelFourAdjacencyResolution();
                TopologyErrorDetected = CheckForTopologyErrors();
            }
            else if (TopologyLevel == TopologyEstimationLevel.Five)
            {
                ExecuteLevelFiveAdjacencyResolution();
                TopologyErrorDetected = CheckForTopologyErrors();
            }
        }

        private bool CheckForEdgeConnectionError(SwitchingDeviceBase edge)
        {
            foreach (VertexAdjacencyRow row in m_adjacencyList.Rows)
            {
                int fromVertex = edge.FromNodeID;
                int toVertex = edge.ToNodeID;
                if (row.Header.Vertices.Contains(fromVertex) && !row.Header.Vertices.Contains(toVertex))
                {
                    return true;
                }
                else if (!row.Header.Vertices.Contains(fromVertex) && row.Header.Vertices.Contains(toVertex))
                {
                    return true;
                }
            }
            return false;
        }

        private bool CheckForTopologyErrors()
        {
            foreach (SwitchingDeviceBase edge in m_edgeSet)
            {
                if (edge is CircuitBreaker)
                {
                    CircuitBreaker circuitBreaker = (CircuitBreaker)edge;
                    if (circuitBreaker.IsMeasuredClosed || circuitBreaker.IsInferredClosed)
                    {
                        if (CheckForEdgeConnectionError(edge))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private void ExecuteLevelZeroAdjacencyResolution()
        {
            // do nothing ??
        }

        private void ExecuteLevelOneAdjacencyResolution()
        {
            while (ExecutingLevelOneAdjacencyResolution()) { }
        }

        private bool ExecutingLevelOneAdjacencyResolution()
        {
            foreach (VertexAdjacencyRow row in m_adjacencyList.Rows)
            {
                foreach (VertexCluster adjacency in row.Adjacencies)
                {
                    SwitchingDeviceBase connectingEdge = ConnectingEdgeBetween(row.Header, adjacency);

                    if (connectingEdge is CircuitBreaker)
                    {
                        CircuitBreaker circuitBreaker = (CircuitBreaker)connectingEdge;
                        if (circuitBreaker.IsMeasuredClosed)
                        {
                            ConnectionEstablished(row.Header, adjacency);
                            return true;
                        }
                    }
                    else if (connectingEdge is Switch)
                    {
                        Switch circuitSwitch = (Switch)connectingEdge;
                        if (circuitSwitch.IsClosed)
                        {
                            ConnectionEstablished(row.Header, adjacency);
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private void ExecuteLevelTwoAdjacencyResolution()
        {
            while (ExecutingLevelTwoAdjacencyResolution()) { }
        }

        private bool ExecutingLevelTwoAdjacencyResolution()
        {
            foreach (VertexAdjacencyRow row in m_adjacencyList.Rows)
            {
                foreach (VertexCluster adjacency in row.Adjacencies)
                {
                    SwitchingDeviceBase connectingEdge = ConnectingEdgeBetween(row.Header, adjacency);

                    if (connectingEdge is CircuitBreaker)
                    {
                        CircuitBreaker circuitBreaker = (CircuitBreaker)connectingEdge;
                        if (circuitBreaker.IsMeasuredClosed || circuitBreaker.IsInferredClosed)
                        {
                            ConnectionEstablished(row.Header, adjacency);
                            return true;
                        }
                    }
                    else if (connectingEdge is Switch)
                    {
                        Switch circuitSwitch = (Switch)connectingEdge;
                        if (circuitSwitch.IsClosed)
                        {
                            ConnectionEstablished(row.Header, adjacency);
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private void ExecuteLevelThreeAdjacencyResolution()
        {
            while (ExecutingLevelThreeAdjacencyResolution()) { }
        }

        private bool ExecutingLevelThreeAdjacencyResolution()
        {
            foreach (VertexAdjacencyRow row in m_adjacencyList.Rows)
            {
                foreach (VertexCluster adjacency in row.Adjacencies)
                {
                    if (CoherencyExistsBetween(row.Header, adjacency))
                    {
                        ConnectionEstablished(row.Header, adjacency);
                        return true;
                    }
                }
            }

            return false;
        }

        private void ExecuteLevelFourAdjacencyResolution()
        {
            while (ExecutingLevelThreeAdjacencyResolution()) { }
            while (ExecutingLevelTwoAdjacencyResolution()) { }
        }

        private void ExecuteLevelFiveAdjacencyResolution()
        {
            while (ExecutingLevelThreeAdjacencyResolution()) { }
            while (ExecutingLevelFourAdjacencyResolution()) { }
        }

        private bool ExecutingLevelFourAdjacencyResolution()
        {
            foreach (VertexAdjacencyRow row in m_adjacencyList.Rows)
            {
                foreach (VertexCluster adjacency in row.Adjacencies)
                {
                    SwitchingDeviceBase connectingEdge = ConnectingEdgeBetween(row.Header, adjacency);

                    if (!connectingEdge.CrossDevicePhasors.MeasurementPairWasReported)
                    {
                        if (connectingEdge is CircuitBreaker)
                        {
                            CircuitBreaker circuitBreaker = (CircuitBreaker)connectingEdge;
                            if (circuitBreaker.IsMeasuredClosed)
                            {
                                ConnectionEstablished(row.Header, adjacency);
                                return true;
                            }
                        }
                        else if (connectingEdge is Switch)
                        {
                            Switch circuitSwitch = (Switch)connectingEdge;
                            if (circuitSwitch.IsClosed)
                            {
                                ConnectionEstablished(row.Header, adjacency);
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// A method which resolves vertices connected through logical devices such as circuit breakers and switches which are energized.
        /// </summary>
        public void ResolveDirectlyConnectedAdjacencies()
        {
            while (ResolvingDirectlyConnectedAdjacencies()) { }
        }

        /// <summary>
        /// Intneded to function as a recursive function for use with <see cref="LinearStateEstimator.Graphs.SubstationGraph.ResolveDirectlyConnectedAdjacencies"/> which calls this method as the parameter of a while loop. This method will be called over and over again until all of the adjacencies which have direct electrical connections are resolved into clusters of directly connected nodes.
        /// </summary>
        /// <returns></returns>
        public bool ResolvingDirectlyConnectedAdjacencies()
        {
            foreach (VertexAdjacencyRow row in m_adjacencyList.Rows)
            {
                foreach (VertexCluster adjacency in row.Adjacencies)
                {
                    SwitchingDeviceBase connectingEdge = ConnectingEdgeBetween(row.Header, adjacency);

                    if (connectingEdge is CircuitBreaker)
                    {
                        CircuitBreaker circuitBreaker = (CircuitBreaker)connectingEdge;
                        if (circuitBreaker.IsClosed)
                        {
                            ConnectionEstablished(row.Header, adjacency);
                            return true;
                        }
                    }
                    else if (connectingEdge is Switch)
                    {
                        Switch circuitSwitch = (Switch)connectingEdge;
                        if (circuitSwitch.IsClosed)
                        {
                            ConnectionEstablished(row.Header, adjacency);
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// A method used to take the groups of directly connected nodes and convert them into <see cref="LinearStateEstimator.Modeling.ObservedBus"/> objects.
        /// </summary>
        /// <returns></returns>
        public List<ObservedBus> ResolveToObservedBuses()
        {
            m_observedBuses.Clear();
            List<ObservedBus> observedBuses = new List<ObservedBus>();
            foreach (VertexAdjacencyRow vertexAdjacencyRow in m_adjacencyList.Rows)
            {
                List<Node> nodeCluster = new List<Node>();
                foreach (int nodeInternalID in vertexAdjacencyRow.Header.Vertices)
                {
                    nodeCluster.Add(m_vertexSet.Find(x => x.InternalID == nodeInternalID));
                }
                observedBuses.Add(new ObservedBus(m_internalID, nodeCluster));
                m_observedBuses.Add(new ObservedBus(m_internalID, nodeCluster));
            }
            return observedBuses;
        }

        /// <summary>
        /// Mergers two vertices together once a direct connection has been determined.
        /// </summary>
        /// <param name="fromVertexCluster">The vertex on the from side of the edge.</param>
        /// <param name="toVertexCluster">The vertex on the to side of the edge.</param>
        public void ConnectionEstablished(VertexCluster fromVertexCluster, VertexCluster toVertexCluster)
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

            VertexAdjacencyRow source = m_adjacencyList.RowWithHeader(fromVertexCluster);
            VertexAdjacencyRow target = m_adjacencyList.RowWithHeader(toVertexCluster);

            // Merge the two rows into one
            source.MergeWith(target);

            // Remove the old from the list
            m_adjacencyList.RemoveRow(target);

            // Update the vertices in the rest of the table
            foreach (VertexAdjacencyRow row in m_adjacencyList.Rows)
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

        public bool CoherencyExistsBetween(VertexCluster fromVertexCluster, VertexCluster toVertexCluster)
        {
            foreach (int fromVertex in fromVertexCluster.Vertices)
            {
                Node fromNode = m_vertexSet.Find(x => x.InternalID == fromVertex);
                foreach (int toVertex in toVertexCluster.Vertices)
                {
                    Node toNode = m_vertexSet.Find(x => x.InternalID == toVertex);
                    if (fromNode.Voltage.PositiveSequence.Measurement.IncludeInEstimator &&
                        toNode.Voltage.PositiveSequence.Measurement.IncludeInEstimator)
                    {
                        PhasorPair vertexPair = new PhasorPair(fromNode.Voltage.PositiveSequence.Measurement, toNode.Voltage.PositiveSequence.Measurement);
                        if (vertexPair.AbsoluteAngleDeltaInDegrees < AngleDeltaThresholdInDegrees)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Finds and returns the edge which connects to vertices.
        /// </summary>
        /// <param name="fromVertexCluster">The vertex for the from side of the potential edge.</param>
        /// <param name="toVertexCluster">The vertex for the to side of the potential edge.</param>
        /// <returns></returns>
        public SwitchingDeviceBase ConnectingEdgeBetween(VertexCluster fromVertexCluster, VertexCluster toVertexCluster)
        {
            foreach (SwitchingDeviceBase switchingDevice in m_edgeSet)
            {
                foreach (int fromVertex in fromVertexCluster.Vertices)
                {
                    foreach (int toVertex in toVertexCluster.Vertices)
                    {
                        if (switchingDevice.FromNode.InternalID == fromVertex && switchingDevice.ToNode.InternalID == toVertex)
                        {
                            return switchingDevice;
                        }
                        else if (switchingDevice.FromNode.InternalID == toVertex && switchingDevice.ToNode.InternalID == fromVertex)
                        {
                            return switchingDevice;
                        }
                    }
                }
            }

            return null;
        }

        #endregion

        public void CreatePhasorPairMatrix()
        {
            m_phasorPairMatrix = new PhasorPair[ExpectedVertexSet.Count, ExpectedVertexSet.Count];

            foreach (Node i in ExpectedVertexSet)
            {
                foreach (Node j in ExpectedVertexSet)
                {
                    PhasorPair pair = new PhasorPair(i.Voltage.PositiveSequence.Measurement, j.Voltage.PositiveSequence.Measurement);
                    m_phasorPairMatrix[ExpectedVertexSet.IndexOf(i), ExpectedVertexSet.IndexOf(j)] = pair;
                }
            }
        }

        public void InitializeConnectivityMatrix()
        {
            m_pastConnectivityMatrix = new bool[ExpectedVertexSet.Count, ExpectedVertexSet.Count];

            for (int i = 0; i < ExpectedVertexSet.Count; i++)
            {
                for (int j = 0; j < ExpectedVertexSet.Count; j++)
                {
                    m_pastConnectivityMatrix[i, j] = (m_phasorPairMatrix[i, j].AbsoluteAngleDeltaInDegrees < AngleDeltaThresholdInDegrees);
                }
            }
        }

        public void UpdateConnectivityMatrix()
        {
            for (int i = 0; i < ExpectedVertexSet.Count; i++)
            {
                for (int j = 0; j < ExpectedVertexSet.Count; j++)
                {
                    m_pastConnectivityMatrix[i, j] = (m_phasorPairMatrix[i, j].AbsoluteAngleDeltaInDegrees < AngleDeltaThresholdInDegrees);
                }
            }
        }

        public bool ComparePastAndPresentConnectivityMatrices()
        {
            for (int i = 0; i < ExpectedVertexSet.Count; i++)
            {
                for (int j = 0; j < ExpectedVertexSet.Count; j++)
                {
                    if (m_pastConnectivityMatrix[i, j] != (m_phasorPairMatrix[i, j].AbsoluteAngleDeltaInDegrees < AngleDeltaThresholdInDegrees))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        #region [ Private Methods ]

        private void BuildVertexSet(Substation substation)
        {
            m_vertexSet = new List<Node>();

            // Build Vertex Set
            foreach (Node node in substation.Nodes)
            {
                m_vertexSet.Add(node);
            }
        }

        private void BuildeEdgeSet(Substation substation)
        {
            m_edgeSet = new List<SwitchingDeviceBase>();

            // Build Edge Set
            foreach (CircuitBreaker circuitBreaker in substation.CircuitBreakers)
            {
                m_edgeSet.Add(circuitBreaker);
            }
            foreach (Switch circuitSwitch in substation.Switches)
            {
                m_edgeSet.Add(circuitSwitch);
            }
        }

        private void InitializeAdjacencyList()
        {
            m_adjacencyList = new VertexAdjacencyList();

            List<VertexCluster> vertexClusters = new List<VertexCluster>();
            List<VertexAdjacencyRow> adjacencyRows = new List<VertexAdjacencyRow>();

            foreach (Node node in m_vertexSet)
            {
                VertexCluster vertexCluster = new VertexCluster(node.InternalID);
                List<VertexCluster> vertexAdjacencies = new List<VertexCluster>();

                foreach (SwitchingDeviceBase switchingDevice in m_edgeSet)
                {
                    if (node.InternalID == switchingDevice.FromNode.InternalID)
                    {
                        vertexAdjacencies.Add(new VertexCluster(switchingDevice.ToNode.InternalID));
                    }
                    else if (node.InternalID == switchingDevice.ToNode.InternalID)
                    {
                        vertexAdjacencies.Add(new VertexCluster(switchingDevice.FromNode.InternalID));
                    }
                }

                m_adjacencyList.Rows.Add(new VertexAdjacencyRow(vertexCluster, vertexAdjacencies));
            }
        }

        #endregion
    }
}
