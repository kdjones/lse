using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynchrophasorAnalytics.Hdb.Records
{
    public class LineSegmentExtension
    {
        private int m_number;
        private string m_id;
        private string m_transmissionLineId;
        private string m_divisionName;
        private string m_fromNodeDeviceName;
        private string m_fromNodeMagnitudeHistorianId;
        private string m_fromNodeAngleHistorianId;
        private string m_toNodeDeviceName;
        private string m_toNodeMagnitudeHistorianId;
        private string m_toNodeAngleHistorianId;
        
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

        public string TransmissionLineId
        {
            get
            {
                return m_transmissionLineId;
            }
            set
            {
                m_transmissionLineId = value;
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

        public LineSegmentExtension()
        {
        }

        public override string ToString()
        {
            return "Line Segment: " + TransmissionLineId;
        }
    }
}
