using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynchrophasorAnalytics.Hdb.Records
{
    public class CircuitBreakerExtension
    {
        private int m_number;
        private string m_id;
        private string m_stationName;
        private string m_deviceName;
        private string m_historianId;
        private string m_bitPosition;

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

        public string DeviceName
        {
            get
            {
                return m_deviceName;
            }
            set
            {
                m_deviceName = value;
            }
        }

        public string HistorianId
        {
            get
            {
                return m_historianId;
            }
            set
            {
                m_historianId = value;
            }
        }

        public string BitPosition
        {
            get
            {
                return m_bitPosition;
            }
            set
            {
                m_bitPosition = value;
            }
        }


        public CircuitBreakerExtension()
        {
        }

        public override string ToString()
        {
            return $"{StationName}_{Id}";
        }
    }
}
