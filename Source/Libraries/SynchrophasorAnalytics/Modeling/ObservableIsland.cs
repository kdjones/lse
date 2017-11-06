using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynchrophasorAnalytics.Modeling
{
    public class ObservableIsland
    {
        #region [ Private Members ]

        private List<Substation> m_substations;
        private List<TransmissionLine> m_transmissionLines;

        #endregion

        #region [ Properties ]

        public List<Substation> Substations
        {
            get
            {
                return m_substations;
            }
            set
            {
                m_substations = value;
            }
        }

        public List<TransmissionLine> TransmissionLines
        {
            get
            {
                return m_transmissionLines;
            }
            set
            {
                m_transmissionLines = value;
            }
        }


        #endregion

        #region [ Constructor ]

        public ObservableIsland()
        {
            m_substations = new List<Substation>();
            m_transmissionLines = new List<TransmissionLine>();
        }

        public ObservableIsland(Substation root)
            :this()
        {
            m_substations.Add(root);
        }

        #endregion

        #region [ Private Methods ]

        public TransmissionLine ConnectingLineBetween(ObservableIsland island, NetworkModel model)
        {
            foreach (TransmissionLine transmissionLine in model.TransmissionLines)
            {
                if (Substations.Contains(transmissionLine.FromSubstation) && island.Substations.Contains(transmissionLine.ToSubstation))
                {
                    return transmissionLine;
                }
                else if (island.Substations.Contains(transmissionLine.ToSubstation) && Substations.Contains(transmissionLine.ToSubstation))
                {
                    return transmissionLine;
                }
            }
            return null;
        }

        #endregion

        #region [ Public Methods ]

        public bool TryMerge(ObservableIsland island, NetworkModel model)
        {
            TransmissionLine connectingLine = ConnectingLineBetween(island, model);
            if (connectingLine != null)
            {
                m_substations.AddRange(island.Substations);
                return true;
            }
            return false;
        }

        public void AppendTransmissionLines(NetworkModel model)
        {
            foreach (TransmissionLine transmissionLine in model.TransmissionLines)
            {
                if (Substations.Contains(transmissionLine.FromSubstation))
                {
                    if (!TransmissionLines.Contains(transmissionLine))
                    {
                        TransmissionLines.Add(transmissionLine);
                    }
                }
            }
        }


        public void AppendTransmissionLinesAtVoltageLevel(NetworkModel model, VoltageLevel baseKv)
        {
            foreach (TransmissionLine transmissionLine in model.TransmissionLines)
            {
                if (transmissionLine.FromNode.BaseKV.InternalID == baseKv.InternalID)
                {
                    if (Substations.Contains(transmissionLine.FromSubstation))
                    {
                        if (!TransmissionLines.Contains(transmissionLine))
                        {
                            TransmissionLines.Add(transmissionLine);
                        }
                    }
                }
            }
        }

        public static List<Substation> FindConnectedSubstations(Substation root, List<Substation> connectedSubstations, NetworkModel model)
        {
            foreach (TransmissionLine transmissionLine in model.TransmissionLines)
            {
                if (transmissionLine.FromSubstation == root)
                {
                    if (!connectedSubstations.Contains(transmissionLine.ToSubstation))
                    {
                        connectedSubstations.Add(transmissionLine.ToSubstation);
                        List<Substation> subConnectedSubstations = ObservableIsland.FindConnectedSubstations(transmissionLine.ToSubstation, connectedSubstations, model);
                    }
                }
                else if (transmissionLine.ToSubstation == root)
                {
                    if (!connectedSubstations.Contains(transmissionLine.FromSubstation))
                    {
                        connectedSubstations.Add(transmissionLine.FromSubstation);
                        List<Substation> subConnectedSubstations = ObservableIsland.FindConnectedSubstations(transmissionLine.FromSubstation, connectedSubstations, model);
                    }
                }
            }
            return connectedSubstations;
        }

        public static List<Substation> FindConnectedSubstationsAtVoltageLevel(Substation root, List<Substation> connectedSubstations, NetworkModel model, VoltageLevel baseKv)
        {
            foreach (TransmissionLine transmissionLine in model.TransmissionLines)
            {
                if (transmissionLine.FromNode.BaseKV.InternalID == baseKv.InternalID)
                {
                    if (transmissionLine.FromSubstation == root)
                    {
                        if (!connectedSubstations.Contains(transmissionLine.ToSubstation))
                        {
                            connectedSubstations.Add(transmissionLine.ToSubstation);
                            List<Substation> subConnectedSubstations = ObservableIsland.FindConnectedSubstationsAtVoltageLevel(transmissionLine.ToSubstation, connectedSubstations, model, baseKv);
                        }
                    }
                    else if (transmissionLine.ToSubstation == root)
                    {
                        if (!connectedSubstations.Contains(transmissionLine.FromSubstation))
                        {
                            connectedSubstations.Add(transmissionLine.FromSubstation);
                            List<Substation> subConnectedSubstations = ObservableIsland.FindConnectedSubstationsAtVoltageLevel(transmissionLine.FromSubstation, connectedSubstations, model, baseKv);
                        }
                    }
                }
            }
            return connectedSubstations;
        }

        public static string Report(List<ObservableIsland> islands)
        {
            StringBuilder islandReport = new StringBuilder();
            islandReport.AppendLine($"{islands.Count} Islands{Environment.NewLine}");
            foreach (ObservableIsland island in islands)
            {
                islandReport.AppendLine($"--------- Island {islands.IndexOf(island) + 1} ---------{Environment.NewLine}");
                islandReport.AppendLine($"      --- Substations ---{Environment.NewLine}");
                foreach (Substation substation in island.Substations)
                {
                    islandReport.AppendLine($"            {substation.Name}");
                }
                islandReport.AppendLine($"{Environment.NewLine}");
                islandReport.AppendLine($"      --- Transmission Lines ---{Environment.NewLine}");
                foreach (TransmissionLine transmissionLine in island.TransmissionLines)
                {
                    islandReport.AppendLine($"            {transmissionLine.Name}: {transmissionLine.FromNode.Name} to {transmissionLine.ToNode.Name}");
                }
                islandReport.AppendLine($"{Environment.NewLine}");
            }
            return islandReport.ToString();
        }

        #endregion
    }
}
