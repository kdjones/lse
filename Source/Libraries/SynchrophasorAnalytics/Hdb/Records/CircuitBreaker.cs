using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynchrophasorAnalytics.Hdb.Records
{
    public class CircuitBreaker
    {
        private int m_number;
        private string m_id;
        private string m_type;
        private string m_stationName;
        private string m_fromNodeId;
        private string m_toNodeId;
        private string m_isNormallyOpen;
        private string m_isOpen;

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

        public string Type
        {
            get
            {
                return m_type;
            }
            set
            {
                m_type = value;
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

        public string FromNodeId
        {
            get
            {
                return m_fromNodeId;
            }
            set
            {
                m_fromNodeId = value;
            }
        }

        public string ToNodeId
        {
            get
            {
                return m_toNodeId;
            }
            set
            {
                m_toNodeId = value;
            }
        }

        public string IsNormallyOpen
        {
            get
            {
                return m_isNormallyOpen;
            }
            set
            {
                m_isNormallyOpen = value;
            }
        }

        public string IsOpen
        {
            get
            {
                return m_isOpen;
            }
            set
            {
                m_isOpen = value;
            }
        }

        public CircuitBreaker()
        {
        }

        public override string ToString()
        {
            return $"{StationName}_{Id} ({Type})";
        }
    }
}
