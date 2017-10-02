using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynchrophasorAnalytics.Hdb.Records
{
    public class TransformerExtension
    {
        private int m_number;
        private string m_id;
        private string m_parent;
        private string m_stationName;
        private string m_fromNodeDeviceName;
        private string m_fromNodeMagnitudeHistorianId;
        private string m_fromNodeAngleHistorianId;
        private string m_toNodeDeviceName;
        private string m_toNodeMagnitudeHistorianId;
        private string m_toNodeAngleHistorianId;
        private string m_tapDeviceName;
        private string m_tapHistorianId;

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

        public string Parent
        {
            get
            {
                return m_parent;
            }
            set
            {
                m_parent = value;
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

        public string FromNodeDeviceName
        {
            get
            {
                return m_fromNodeDeviceName;
            }
            set
            {
                m_fromNodeDeviceName = value;
            }
        }

        public string FromNodeMagnitudeHistorianId
        {
            get
            {
                return m_fromNodeMagnitudeHistorianId;
            }
            set
            {
                m_fromNodeMagnitudeHistorianId = value;
            }
        }

        public string FromNodeAngleHistorianId
        {
            get
            {
                return m_fromNodeAngleHistorianId;
            }
            set
            {
                m_fromNodeAngleHistorianId = value;
            }
        }

        public string ToNodeDeviceName
        {
            get
            {
                return m_toNodeDeviceName;
            }
            set
            {
                m_toNodeDeviceName = value;
            }
        }

        public string ToNodeMagnitudeHistorianId
        {
            get
            {
                return m_toNodeMagnitudeHistorianId;
            }
            set
            {
                m_toNodeMagnitudeHistorianId = value;
            }
        }
        
        public string ToNodeAngleHistorianId
        {
            get
            {
                return m_toNodeAngleHistorianId;
            }
            set
            {
                m_toNodeAngleHistorianId = value;
            }
        }

        public string TapDeviceName
        {
            get
            {
                return m_tapDeviceName;
            }
            set
            {
                m_tapDeviceName = value;
            }
        }

        public string TapHistorianId
        {
            get
            {
                return m_tapHistorianId;
            }
            set
            {
                m_tapHistorianId = value;
            }
        }

        public TransformerExtension()
        {
        }

        public override string ToString()
        {
            return StationName + "_" + Id;
        }
    }
}
