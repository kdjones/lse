using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynchrophasorAnalytics.Hdb.Records
{
    public class LineSegment
    {
        private int m_number;
        private string m_id;
        private string m_transmissionLineId;
        private string m_divisionName;
        private string m_fromNodeId;
        private string m_toNodeId;
        private string m_fromStationName;
        private string m_toStationName;
        private double m_resistance;
        private double m_reactance;
        private double m_lineCharing;
        private string m_isRemoved;

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

        public string FromStationName
        {
            get
            {
                return m_fromStationName;
            }
            set
            {
                m_fromStationName = value;
            }
        }

        public string ToStationName
        {
            get
            {
                return m_toStationName;
            }
            set
            {
                m_toStationName = value;
            }
        }

        public double Resistance
        {
            get
            {
                return m_resistance;
            }
            set
            {
                m_resistance = value;
            }
        }

        public double Reactance
        {
            get
            {
                return m_reactance;
            }
            set
            {
                m_reactance = value;
            }
        }

        public double LineCharging
        {
            get
            {
                return m_lineCharing;
            }
            set
            {
                m_lineCharing = value;
            }
        }

        public string IsRemoved
        {
            get
            {
                return m_isRemoved;
            }
            set
            {
                m_isRemoved = value;
            }
        }

        public LineSegment()
        {
        }

        public override string ToString()
        {
            return "Line Segment: " + TransmissionLineId;
        }
    }
}
