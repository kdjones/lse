using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SynchrophasorAnalytics.Hdb.Records;

namespace SynchrophasorAnalytics.Hdb
{
    public class HdbContext
    {
        private ModelFiles m_modelFiles;
        private List<Area> m_areas;
        private List<CircuitBreaker> m_circuitBreakers;
        private List<CircuitBreakerExtension> m_circuitBreakerExtensions;
        private List<Company> m_companies;
        private List<Division> m_divisions;
        private List<LineSegment> m_lineSegments;
        private List<LineSegmentExtension> m_lineSegmentExtensions;
        private List<Node> m_nodes;
        private List<NodeExtension> m_nodeExtensions;
        private List<Shunt> m_shunts;
        private List<ShuntExtension> m_shuntExtensions;
        private List<Station> m_stations;
        private List<Transformer> m_transformers;
        private List<TransformerExtension> m_transformerExtensions;
        private List<ParentTransformer> m_parentTransformers;
        private List<TransformerTap> m_transformerTaps;
        private List<TransmissionLine> m_transmissionLines;

        public ModelFiles ModelFiles
        {
            get
            {
                return m_modelFiles;
            }
            set
            {
                m_modelFiles = value;
            }
        }

        public List<Area> Areas
        {
            get
            {
                return m_areas;
            }
            set
            {
                m_areas = value;
            }
        }

        public List<CircuitBreaker> CircuitBreakers
        {
            get
            {
                return m_circuitBreakers;
            }
            set
            {
                m_circuitBreakers = value;
            }
        }

        public List<CircuitBreakerExtension> CircuitBreakerExtensions
        {
            get
            {
                return m_circuitBreakerExtensions;
            }
            set
            {
                m_circuitBreakerExtensions = value;
            }
        }

        public List<Company> Companies
        {
            get
            {
                return m_companies;
            }
            set
            {
                m_companies = value;
            }
        }

        public List<Division> Divisions
        {
            get
            {
                return m_divisions;
            }
            set
            {
                m_divisions = value;
            }
        }

        public List<LineSegment> LineSegments
        {
            get
            {
                return m_lineSegments;
            }
            set
            {
                m_lineSegments = value;
            }
        }

        public List<LineSegmentExtension> LineSegmentExtensions
        {
            get
            {
                return m_lineSegmentExtensions;
            }
            set
            {
                m_lineSegmentExtensions = value;
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

        public List<NodeExtension> NodeExtensions
        {
            get
            {
                return m_nodeExtensions;
            }
            set
            {
                m_nodeExtensions = value;
            }
        }

        public List<Shunt> Shunts
        {
            get
            {
                return m_shunts;
            }
            set
            {
                m_shunts = value;
            }
        }

        public List<ShuntExtension> ShuntExtensions
        {
            get
            {
                return m_shuntExtensions;
            }
            set
            {
                m_shuntExtensions = value;
            }
        }

        public List<Station> Stations
        {
            get
            {
                return m_stations;
            }
            set
            {
                m_stations = value;
            }
        }

        public List<Transformer> Transformers
        {
            get
            {
                return m_transformers;
            }
            set
            {
                m_transformers = value;
            }
        }

        public List<TransformerExtension> TransformerExtensions
        {
            get
            {
                return m_transformerExtensions;
            }
            set
            {
                m_transformerExtensions = value;
            }
        }

        public List<ParentTransformer> ParentTransformers
        {
            get
            {
                return m_parentTransformers;
            }
            set
            {
                m_parentTransformers = value;
            }
        }

        public List<TransformerTap> TransformerTaps
        {
            get
            {
                return m_transformerTaps;
            }
            set
            {
                m_transformerTaps = value;
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
        
        public HdbContext(ModelFiles modelFiles)
        {
            m_modelFiles = modelFiles;
            SyncContext();
        }

        public void SyncContext()
        {
            m_areas = HdbReader.ReadAreaFile(m_modelFiles.AreaFile);
            m_circuitBreakers = HdbReader.ReadCircuitBreakerFile(m_modelFiles.CircuitBreakerFile);
            m_circuitBreakerExtensions = HdbReader.ReadCircuitBreakerExtensionFile(m_modelFiles.CircuitBreakerExtensionFile);
            m_companies = HdbReader.ReadCompanyFile(m_modelFiles.CompanyFile);
            m_divisions = HdbReader.ReadDivisionFile(m_modelFiles.DivisionFile);
            m_lineSegments = HdbReader.ReadLineSegmentFile(m_modelFiles.LineSegmentFile);
            m_lineSegmentExtensions = HdbReader.ReadLineSegmentExtensionFile(m_modelFiles.LineSegmentExtensionFile);
            m_nodes = HdbReader.ReadNodeFile(m_modelFiles.NodeFile);
            m_nodeExtensions = HdbReader.ReadNodeExtensionFile(m_modelFiles.NodeExtensionFile);
            m_shunts = HdbReader.ReadShuntFile(m_modelFiles.ShuntFile);
            m_shuntExtensions = HdbReader.ReadShuntExtensionFile(m_modelFiles.ShuntExtensionFile);
            m_stations = HdbReader.ReadStationFile(m_modelFiles.StationFile);
            m_transformers = HdbReader.ReadTransformerFile(m_modelFiles.TransformerFile);
            m_transformerExtensions = HdbReader.ReadTransformerExtensionFile(m_modelFiles.TransformerExtensionFile);
            m_parentTransformers = HdbReader.ReadParentTransformerFile(m_modelFiles.ParentTransformerFile);
            m_transformerTaps = HdbReader.ReadTransformerTapFile(m_modelFiles.TransformerTapFile);
            m_transmissionLines = HdbReader.ReadTransmissionLineFile(m_modelFiles.TransmissionLineFile);
        }

        public Company GetStationCompany(Station station)
        {
            List<Node> substationNodes = m_nodes.FindAll(x => x.StationName == station.Name);
            return m_companies.Find(x => x.Name == substationNodes[0].CompanyName);
        }

        public Company GetNodeCompany(Node node)
        {
            return m_companies.Find(x => x.Name == node.CompanyName);
        }

        public void Filter(List<string> companyNames)
        {
            List<Company> retainedCompanies = new List<Company>();
            List<Division> retainedDivisions = new List<Division>();
            List<Station> retainedStations = new List<Station>();
            List<Station> stationsToRemove = new List<Station>();
            List<Node> retainedNodes = new List<Node>();
            List<Node> nodesToRemove = new List<Node>();
            List<LineSegment> lineSegmentsToRemove = new List<LineSegment>();
            List<TransmissionLine> transmissionLinesToRemove = new List<TransmissionLine>();
            List<CircuitBreaker> circuitBreakersToRemove = new List<CircuitBreaker>();
            List<Shunt> shuntsToRemove = new List<Shunt>();
            List<Transformer> transformersToRemove = new List<Transformer>();
            List<TransformerTap> retainedTaps = new List<TransformerTap>();
            List<ParentTransformer> parentTransformersToRemove = new List<ParentTransformer>();

            Console.WriteLine("1");
            // Line segments, companies, stations
            Parallel.ForEach(m_lineSegments, (lineSegment) =>
           {
               string fromNodeId = $"{lineSegment.FromStationName}_{lineSegment.FromNodeId}";
               string toNodeId = $"{lineSegment.ToStationName}_{lineSegment.ToNodeId}";
               Node fromNode = m_nodes.Find(x => $"{x.StationName}_{x.Id}" == fromNodeId);
               Node toNode = m_nodes.Find(x => $"{x.StationName}_{x.Id}" == toNodeId);
               Company fromCompany = GetNodeCompany(fromNode);
               Company toCompany = GetNodeCompany(toNode);

               Station fromStation = m_stations.Find(x => x.Name == lineSegment.FromStationName);
               Station toStation = m_stations.Find(x => x.Name == lineSegment.ToStationName);

               if (companyNames.Contains(fromCompany.Name) || companyNames.Contains(toCompany.Name))
               {
                   if (!retainedStations.Contains(fromStation))
                   {
                       retainedStations.Add(fromStation);
                   }
                   if (!retainedStations.Contains(toStation))
                   {
                       retainedStations.Add(toStation);
                   }
                   if (!retainedNodes.Contains(fromNode))
                   {
                       retainedNodes.Add(fromNode);
                   }
                   if (!retainedNodes.Contains(toNode))
                   {
                       retainedNodes.Add(toNode);
                   }
                   if (!retainedCompanies.Contains(fromCompany))
                   {
                       retainedCompanies.Add(fromCompany);
                   }
                   if (!retainedCompanies.Contains(toCompany))
                   {
                       retainedCompanies.Add(toCompany);
                   }
               }
               else
               {
                   if (!lineSegmentsToRemove.Contains(lineSegment))
                   {
                       lineSegmentsToRemove.Add(lineSegment);
                   }
                   if (!stationsToRemove.Contains(fromStation))
                   {
                       stationsToRemove.Add(fromStation);
                   }
                   if (!stationsToRemove.Contains(toStation))
                   {
                       stationsToRemove.Add(toStation);
                   }
               }
           });

            Console.WriteLine("2");
            // Transmission Lines
            foreach (LineSegment lineSegment in lineSegmentsToRemove)
            {
                TransmissionLine transmissionLine = m_transmissionLines.Find(x => x.Id == lineSegment.TransmissionLineId);
                if (!transmissionLinesToRemove.Contains(transmissionLine))
                {
                    transmissionLinesToRemove.Add(transmissionLine);
                }
            }

            Console.WriteLine("3");
            // Nodes
            foreach (Station station in stationsToRemove)
            {
                List<Node> stationNodes = m_nodes.FindAll(x => x.StationName == station.Name);
                foreach (Node node in stationNodes)
                {
                    if (!nodesToRemove.Contains(node))
                    {
                        nodesToRemove.Add(node);
                    }
                }
            }

            Console.WriteLine("4");
            // Divisions
            foreach (Station station in retainedStations)
            {
                List<Node> stationNodes = m_nodes.FindAll(x => x.StationName == station.Name);
                foreach (Node node in stationNodes)
                {
                    Division division = m_divisions.Find(x => x.Name == node.DivisionName);
                    
                    if (!retainedDivisions.Contains(division))
                    {
                        retainedDivisions.Add(division);
                    }
                }
            }

            Console.WriteLine("5");
            // Circuit breakers
            foreach (CircuitBreaker breaker in m_circuitBreakers)
            {
                Station parentStation = m_stations.Find(x => x.Name == breaker.StationName);
                if (stationsToRemove.Contains(parentStation))
                {
                    if (!circuitBreakersToRemove.Contains(breaker))
                    {
                        circuitBreakersToRemove.Add(breaker);
                    }
                }
            }


            Console.WriteLine("6");
            // shunts
            foreach (Shunt shunt in m_shunts)
            {
                Station parentStation = m_stations.Find(x => x.Name == shunt.StationName);
                if (stationsToRemove.Contains(parentStation))
                {
                    if (!shuntsToRemove.Contains(shunt))
                    {
                        shuntsToRemove.Add(shunt);
                    }
                }
            }

            Console.WriteLine("7");
            // Transformers and Transformer Taps
            foreach (Transformer transformer in m_transformers)
            {
                Station parentStation = m_stations.Find(x => x.Name == transformer.StationName);
                if (stationsToRemove.Contains(parentStation))
                {
                    if (!transformersToRemove.Contains(transformer))
                    {
                        transformersToRemove.Add(transformer);
                    }
                }
                else
                {
                    TransformerTap fromTap = m_transformerTaps.Find(x => x.Id == transformer.FromNodeTap);
                    TransformerTap toTap = m_transformerTaps.Find(x => x.Id == transformer.ToNodeTap);
                    if (!retainedTaps.Contains(fromTap))
                    {
                        retainedTaps.Add(fromTap);
                    }
                    if (!retainedTaps.Contains(toTap))
                    {
                        retainedTaps.Add(toTap);
                    }
                }
            }
            Console.WriteLine("8");
            // Parent Transformer
            foreach (Transformer transformer in transformersToRemove)
            {
                ParentTransformer parentTransformer = m_parentTransformers.Find(x => x.Id == transformer.Parent);
                if (!parentTransformersToRemove.Contains(parentTransformer))
                {
                    parentTransformersToRemove.Add(parentTransformer);
                }
            }
            m_companies = retainedCompanies;
            m_divisions = retainedDivisions;
            m_transformerTaps = retainedTaps;

            Console.WriteLine("9");
            foreach (Station station in stationsToRemove)
            {
                m_stations.Remove(station);
            }

            foreach (LineSegment lineSegment in lineSegmentsToRemove)
            {
                m_lineSegments.Remove(lineSegment);
            }
            
            foreach (Node node in nodesToRemove)
            {
                m_nodes.Remove(node);
            }
            
            foreach (Shunt shunt in shuntsToRemove)
            {
                m_shunts.Remove(shunt);
            }

            foreach (CircuitBreaker breaker in circuitBreakersToRemove)
            {
                m_circuitBreakers.Remove(breaker);
            }

            foreach (Transformer transformer in transformersToRemove)
            {
                m_transformers.Remove(transformer);
            }

            foreach (ParentTransformer transformer in parentTransformersToRemove)
            {
                m_parentTransformers.Remove(transformer);
            }

            foreach (TransmissionLine transmissionLine in transmissionLinesToRemove)
            {
                m_transmissionLines.Remove(transmissionLine);
            }
        }
    }
}
