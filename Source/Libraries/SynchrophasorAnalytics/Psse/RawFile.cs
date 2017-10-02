using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SynchrophasorAnalytics.Psse
{
    /// <summary>
    /// 
    /// </summary>
    public class RawFile
    {
        #region [ Private Members ]

        private List<Bus> m_buses = new List<Bus>();
        private List<Branch> m_branches = new List<Branch>();
        private List<FixedShunt> m_fixedShunts = new List<FixedShunt>();
        private List<AdhocBusGroup> m_busGroups = new List<AdhocBusGroup>();
        private double m_frequency;
        private int m_version;
        private string m_firstTitleLine;
        private string m_secondTitleLine;
        private string m_comment;
        private double m_baseMva;
        private string m_path;

        #endregion

        #region [ Public Properties ]

        public string Path
        {
            get
            {
                return m_path;
            }
            set
            {
                m_path = value;
            }
        }

        public string FileNameWithExtension
        {
            get
            {
                string[] pathData = m_path.Split('\\');
                return pathData[pathData.Length - 1];
            }
        }

        public string FileName
        {
            get
            {
                return FileNameWithExtension.Split('.')[0];
            }
        }

        public List<Bus> Buses
        {
            get
            {
                return m_buses;
            }
            set
            {
                m_buses = value;
            }
        }

        public List<Branch> Branches
        {
            get
            {
                return m_branches;
            }
            set
            {
                m_branches = value;
            }
        }

        public List<FixedShunt> FixedShunts
        {
            get
            {
                return m_fixedShunts;
            }
            set
            {
                m_fixedShunts = value;
            }
        }

        public int Version
        {
            get
            {
                return m_version;
            }
            set
            {
                m_version = value;
            }
        }

        public double Frequency
        {
            get
            {
                return m_frequency;
            }
            set
            {
                m_frequency = value;
            }
        }

        public double BaseMva
        {
            get
            {
                return m_baseMva;
            }
            set
            {
                m_baseMva = value;
            }
        }

        public string Comment
        {
            get
            {
                return m_comment;
            }
            set
            {
                m_comment = value;
            }
        }

        public string FirstTitleLine
        {
            get
            {
                return m_firstTitleLine;
            }
            set
            {
                m_firstTitleLine = value;
            }
        }

        public string SecondTitleLine
        {
            get
            {
                return m_secondTitleLine;
            }
            set
            {
                m_secondTitleLine = value;
            }
        }

        public List<List<int>> SubstationBusNumberGroups
        {
            get
            {
                return GroupBusesIntoSubstations();
            }
        }


        #endregion

        #region [ Constructor ]

        /// <summary>
        /// 
        /// </summary>
        public RawFile()
        {
            m_firstTitleLine = "EMPTY";
            m_secondTitleLine = "EMPTY";
        }

        #endregion

        #region [ Static Methods ]

        public static RawFile Read(string fileName)
        {
            string[] lines = File.ReadAllLines(fileName);

            RawFile rawFile = new RawFile();
            rawFile.Path = fileName;

            string readingRecordType = RecordTypes.Bus;

            foreach (string line in lines)
            {
                if (rawFile.Frequency == 0)
                {
                    string[] header = line.Split('/');
                    string data = header[0];
                    rawFile.Comment = header[1];
                    rawFile.BaseMva = Convert.ToDouble(data.Split(',')[1]);
                    rawFile.Frequency = Convert.ToDouble(data.Split(',')[5]);
                    rawFile.Version = Convert.ToInt32(data.Split(',')[2]);
                }
                else if (rawFile.FirstTitleLine == "EMPTY")
                {
                    rawFile.FirstTitleLine = line;
                }
                else if (rawFile.SecondTitleLine == "EMPTY")
                {
                    rawFile.SecondTitleLine = line;
                }
                else
                {
                    string[] content = line.Split('/');

                    if (content[0].Trim() == "0")
                    {
                        if (content[1].Contains("BEGIN LOAD"))
                        {
                            readingRecordType = RecordTypes.Load;
                        }
                        else if (content[1].Contains("BEGIN FIXED SHUNT"))
                        {
                            readingRecordType = RecordTypes.FixedShunt;
                        }
                        else if (content[1].Contains("BEGIN GENERATOR"))
                        {
                            readingRecordType = RecordTypes.Generator;
                        }
                        else if (content[1].Contains("BEGIN BRANCH"))
                        {
                            readingRecordType = RecordTypes.Branch;
                        }
                        else if (content[1].Contains("BEGIN TRANSFORMER"))
                        {
                            readingRecordType = RecordTypes.Transformer;
                        }
                        else if (content[1].Contains("BEGIN AREA"))
                        {
                            readingRecordType = RecordTypes.Area;
                        }
                        else if (content[1].Contains("BEGIN ZONE"))
                        {
                            readingRecordType = RecordTypes.Zone;
                        }
                        else if (content[1].Contains("BEGIN OWNER"))
                        {
                            readingRecordType = RecordTypes.Owner;
                        }
                        else if (content[1].Contains("BEGIN SWITCHED SHUNT"))
                        {
                            readingRecordType = RecordTypes.SwitchedShunt;
                        }
                    }
                    else
                    {
                        if (readingRecordType == RecordTypes.Bus)
                        {
                            rawFile.Buses.Add(Bus.Parse(line));
                        }
                        else if (readingRecordType == RecordTypes.Load)
                        {
                            rawFile.FixedShunts.Add(FixedShunt.Parse(line));
                        }
                        else if (readingRecordType == RecordTypes.FixedShunt)
                        {

                        }
                        else if (readingRecordType == RecordTypes.Generator)
                        {
                            //
                        }
                        else if (readingRecordType == RecordTypes.Branch)
                        {
                            rawFile.Branches.Add(Branch.Parse(line));
                        }
                        else if (readingRecordType == RecordTypes.Transformer)
                        {
                            //
                        }
                        else if (readingRecordType == RecordTypes.Area)
                        {
                            //
                        }
                        else if (readingRecordType == RecordTypes.Zone)
                        {
                            //
                        }
                        else if (readingRecordType == RecordTypes.Owner)
                        {
                            //
                        }
                        else if (readingRecordType == RecordTypes.SwitchedShunt)
                        {
                            //
                        }
                    }
                }

            }

            return rawFile;
        }

        #endregion

        #region [ Public Methods ]
        
        public Bus GetBusWithNumber(int busNumber)
        {
            foreach (Bus bus in m_buses)
            {
                if (bus.Number == busNumber)
                {
                    return bus;
                }
            }
            return null;
        }

        public List<int> GetBusNumbers()
        {
            List<int> busNumbers = new List<int>();

            foreach (Bus bus in m_buses)
            {
                busNumbers.Add(bus.Number);
            }
            return busNumbers;
        }

        public List<List<int>> GroupBusesIntoSubstations()
        {
            List<int> busNumbers = GetBusNumbers();
            List<List<int>> substations = new List<List<int>>();
            
            
            while (busNumbers.Count > 0)
            {
                int previousNumberOfLogicalConnections = 0;

                List<Branch> logicalConnections = FindLogicalConnections(busNumbers[0]);

                if (logicalConnections.Count != 0)
                {
                    while (previousNumberOfLogicalConnections < logicalConnections.Count)
                    {
                        previousNumberOfLogicalConnections = logicalConnections.Count;
                        logicalConnections = FindSecondLevelLogicalConnections(logicalConnections);
                    }

                    List<int> busNumbersFromLogicalConnection = GetBusNumbersFromLogicalConnections(logicalConnections);

                    foreach (int busNumber in busNumbersFromLogicalConnection)
                    {
                        busNumbers.Remove(busNumber);
                    }

                    if (busNumbersFromLogicalConnection.Count > 0)
                    {
                        substations.Add(busNumbersFromLogicalConnection);
                    }
                }
                else
                {
                    List<int> bus = new List<int>();
                    bus.Add(busNumbers[0]);
                    substations.Add(bus);
                    busNumbers.Remove(busNumbers[0]);
                }
                

            }
            
            return substations;
        }

        public List<int> GetBusNumbersFromLogicalConnections(List<Branch> logicalConnections)
        {
            List<int> busNumbers = new List<int>();

            foreach (Branch branch in logicalConnections)
            {
                if (!busNumbers.Contains(branch.FromBusNumber))
                {
                    busNumbers.Add(branch.FromBusNumber);
                }
                if (!busNumbers.Contains(branch.ToBusNumber))
                {
                    busNumbers.Add(branch.ToBusNumber);
                }
            }

            return busNumbers;
        }

        public List<Branch> FindLogicalConnections(int busNumber)
        {
            List<Branch> logicalConnections = new List<Branch>();

            foreach (Branch branch in m_branches)
            {
                if (branch.IsBreaker || branch.IsSwitch)
                {
                    if (branch.FromBusNumber == busNumber || branch.ToBusNumber == busNumber)
                    {
                        logicalConnections.Add(branch);
                    }
                }
            }
            return logicalConnections;
        }

        public List<Branch> FindSecondLevelLogicalConnections(List<Branch> logicalConnections)
        {
            List<Branch> secondLevelLogicalConnections = new List<Branch>();

            foreach (Branch branch in logicalConnections)
            {
                secondLevelLogicalConnections.Add(branch);
            }

            foreach (Branch branch in logicalConnections)
            {
                List<Branch> fromBusPossibleLogicalConnections = FindLogicalConnections(branch.FromBusNumber);
                List<Branch> toBusPossibleLogicalConnections = FindLogicalConnections(branch.ToBusNumber);

                foreach (Branch possibleBranch in fromBusPossibleLogicalConnections)
                {
                    if (!secondLevelLogicalConnections.Contains(possibleBranch))
                    {
                        secondLevelLogicalConnections.Add(possibleBranch);
                    }
                }
                foreach (Branch possibleBranch in toBusPossibleLogicalConnections)
                {
                    if (!secondLevelLogicalConnections.Contains(possibleBranch))
                    {
                        secondLevelLogicalConnections.Add(possibleBranch);
                    }
                }

            }
            return secondLevelLogicalConnections;
        }

        #endregion
    }
}
