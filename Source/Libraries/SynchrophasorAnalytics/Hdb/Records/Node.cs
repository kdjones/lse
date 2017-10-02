using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynchrophasorAnalytics.Hdb.Records
{
    public class Node
    {
        private int m_number;
        private string m_id;
        private double m_baseKv;
        private double m_baseKvId;
        private string m_companyName;
        private string m_divisionName;
        private string m_stationName;
        private int m_busNumber;

        public int Number
        {
            get
            {
                return m_number;
            }
            set
            {
                m_number = value;
            }
        }

        public string Id
        {
            get
            {
                return m_id;
            }
            set
            {
                m_id = value;
            }
        }

        public double BaseKv
        {
            get
            {
                return m_baseKv;
            }
            set
            {
                m_baseKv = value;
            }
        }

        public double BaseKvId
        {
            get
            {
                return m_baseKvId;
            }
            set
            {
                m_baseKvId = value;
            }
        }

        public string CompanyName
        {
            get
            {
                return m_companyName;
            }
            set
            {
                m_companyName = value;
            }
        }

        public string DivisionName
        {
            get
            {
                return m_divisionName;
            }
            set
            {
                m_divisionName = value;
            }
        }

        public string StationName
        {
            get
            {
                return m_stationName;
            }
            set
            {
                m_stationName = value;
            }
        }

        public int BusNumber
        {
            get
            {
                return m_busNumber;
            }
            set
            {
                m_busNumber = value;
            }
        }

        public Node()
        {
        }

        public override string ToString()
        {
            return "Node: Number:" + Convert.ToString(Number) + " Id:" + Id + " Company:" + CompanyName + " Division:" + DivisionName + " Station:" + StationName + " Base KV:" + Convert.ToString(BaseKv) + " Bus:" + Convert.ToString(BusNumber);
        }
    }
}
