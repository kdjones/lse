using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynchrophasorAnalytics.Hdb.Records
{
    public class Shunt
    {
        private int m_number;
        private string m_id;
        private string m_stationName;
        private string m_nodeId;
        private double m_nominalMvar;
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

        public string NodeId
        {
            get
            {
                return m_nodeId;
            }
            set
            {
                m_nodeId = value;
            }
        }

        public double NominalMvar
        {
            get
            {
                return m_nominalMvar;
            }
            set
            {
                m_nominalMvar = value;
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

        public Shunt()
        {
        }

        public override string ToString()
        {
            return "Shunt:\n      Number:" + Convert.ToString(Number) + "\n          Id:" + Id + "\n     Station:" + StationName + "\n         Node:" + NodeId + "\nNominal Mvar:" + Convert.ToString(NominalMvar);
        }
    }
}
