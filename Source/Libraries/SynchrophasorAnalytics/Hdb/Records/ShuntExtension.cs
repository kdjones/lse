using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynchrophasorAnalytics.Hdb.Records
{
    public class ShuntExtension
    {
        private int m_number;
        private string m_id;
        private string m_stationName;
        private string m_deviceName;
        private string m_magnitudeHistorianId;
        private string m_angleHistorianId;

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

        public string MagnitudeHistorianId
        {
            get
            {
                return m_magnitudeHistorianId;
            }
            set
            {
                m_magnitudeHistorianId = value;
            }
        }

        public string AngleHistorianId
        {
            get
            {
                return m_angleHistorianId;
            }
            set
            {
                m_angleHistorianId = value;
            }
        }

        public ShuntExtension()
        {
        }

        public override string ToString()
        {
            return "Shunt:\n      Number:" + Convert.ToString(Number) + "\n          Id:" + Id + "\n     Station:" + StationName;
        }
    }
}
