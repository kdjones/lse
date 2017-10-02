using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SynchrophasorAnalytics.Networks;


namespace SynchrophasorAnalytics.Modeling
{
    public class VoltageLevelGroup : INetworkDescribable
    {
        #region [ Private Constants ]

        /// <summary>
        /// Default values
        /// </summary>
        private const int DEFAULT_INTERNAL_ID = 0;

        #endregion

        #region [ Private Members ]

        private int m_internalID;
        private List<Node> m_nodes;
        private List<Node> m_directlyObservedNodes;
        private VoltageLevel m_baseKv;
        private List<ObservedBus> m_observedBuses;

        #endregion

        #region [ Properties ]
        
        public int InternalID
        {
            get
            {
                return m_internalID;
            }
            set
            {
                m_internalID = value;
            }
        }
        
        public int Number
        {
            get
            {
                return m_internalID;
            }
            set
            {
                // Does nothing but must exist do to INetworkDescribable interface.
            }
        }
        
        public string Acronym
        {
            get
            {
                StringBuilder stringBuilder = new StringBuilder();
                foreach (Node node in m_nodes)
                {
                    stringBuilder.AppendFormat(node.Acronym + "{0}", Environment.NewLine);
                }
                return stringBuilder.ToString();
            }
            set
            {
            }
        }
        
        public string Name
        {
            get
            {
                StringBuilder stringBuilder = new StringBuilder();
                foreach (Node node in m_nodes)
                {
                    stringBuilder.AppendFormat(node.Name + "{0}", Environment.NewLine);
                }
                return stringBuilder.ToString();
            }
            set
            {
            }
        }
        
        public string Description
        {
            get
            {
                StringBuilder stringBuilder = new StringBuilder();
                foreach (Node node in m_nodes)
                {
                    stringBuilder.AppendFormat(node.Description + "{0}", Environment.NewLine);
                }
                return stringBuilder.ToString();
            }
            set
            {
            }
        }
        
        public string ElementType
        {
            get
            {
                return this.GetType().ToString();
            }
        }
        
        public List<Node> Nodes
        {
            get
            {
                return m_nodes;
            }
            set
            {
                m_nodes = value;
            }
        }
        
        public VoltageLevel BaseKV
        {
            get
            {
                return m_baseKv;
            }
        }

        #endregion

        #region [ Constructors ]
        
        public VoltageLevelGroup(VoltageLevel baseKv)
            :this(DEFAULT_INTERNAL_ID, baseKv, new List<Node>())
        {
        }
        
        public VoltageLevelGroup(int internalID, VoltageLevel baseKv, Node observedNode)
        {
            m_internalID = internalID;
            m_nodes = new List<Node>();
            m_nodes.Add(observedNode);
            m_baseKv = baseKv.DeepCopy();
        }
        
        public VoltageLevelGroup(int internalID, VoltageLevel baseKv, List<Node> observedNodes)
        {
            m_internalID = internalID;
            m_nodes = observedNodes;
            m_baseKv = baseKv.DeepCopy();
            m_observedBuses = new List<ObservedBus>();
            m_directlyObservedNodes = new List<Node>();
        }

        #endregion

        #region [ Public Methods ]

        public PhaseSelection GetPhaseConfiguration(Node node)
        {
            return node.ParentSubstation.ParentDivision.ParentCompany.ParentModel.PhaseConfiguration;
        }

        public void AssembleDirectlyObservedNodes()
        {
            m_directlyObservedNodes = new List<Node>();

            foreach (Node node in m_nodes)
            {
                PhaseSelection phaseConfiguration = GetPhaseConfiguration(node);
                if (phaseConfiguration == PhaseSelection.PositiveSequence)
                {
                    if (node.Voltage.IncludeInPositiveSequenceEstimator)
                    {
                        node.Observability = ObservationState.DirectlyObserved;
                        m_directlyObservedNodes.Add(node);
                    }
                    else
                    {
                        node.Observability = ObservationState.Unobserved;
                    }
                }
                else if (phaseConfiguration == PhaseSelection.ThreePhase)
                {
                    if (node.Voltage.IncludeInEstimator)
                    {
                        node.Observability = ObservationState.DirectlyObserved;
                        m_directlyObservedNodes.Add(node);
                    }
                    else
                    {
                        node.Observability = ObservationState.Unobserved;
                    }
                }
            }
        }

        public void ResolveToObservedBuses()
        {
            AssembleDirectlyObservedNodes();

            m_observedBuses = new List<ObservedBus>();

            foreach (Node node in m_directlyObservedNodes)
            {
                PlaceNodeInObservedBuses(node);
            }
        }

        public void PlaceNodeInObservedBuses(Node node)
        {
            foreach (ObservedBus observedBus in m_observedBuses)
            {
                if (observedBus.IsCoherentWith(node))
                {
                    observedBus.Nodes.Add(node);
                    return;
                }
            }
            m_observedBuses.Add(new ObservedBus(node.InternalID, node));
        }

        public ObservedBus MergeObservedBuses(ObservedBus busA, ObservedBus busB)
        {
            busA.MergeWith(busB);
            return busA;
        }

        public override string ToString()
        {
            string voltageLevelGroupAsString = "";
            foreach (Node node in m_nodes)
            {
                voltageLevelGroupAsString += node.InternalID + "|" + node.Name + ",";
            }
            voltageLevelGroupAsString.Remove(voltageLevelGroupAsString.Length - 1);
            return "-VLG- ID: " + m_internalID.ToString() + "  " + voltageLevelGroupAsString;
        }
        
        public string ToVerboseString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine();
            stringBuilder.AppendFormat("----- Voltage Level Group ------------------------------------------------------");
            stringBuilder.AppendLine();
            stringBuilder.AppendFormat("      InternalID: " + InternalID.ToString() + "{0}", Environment.NewLine);
            stringBuilder.AppendLine();
            foreach (Node node in m_nodes)
            {
                stringBuilder.AppendFormat(node.ToVerboseString() + "{0}", Environment.NewLine);
            }

            stringBuilder.AppendLine();
            return stringBuilder.ToString();
        }

        #endregion
    }
}
