using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynchrophasorAnalytics.Hdb.Records
{
    public class Transformer
    {       
        private int m_number;
        private string m_id;
        private string m_parent;
        private string m_stationName;
        private string m_fromNodeId;
        private string m_toNodeId;
        private string m_regulatedNodeId;
        private double m_fromNodeNominalKv;
        private string m_fromNodeTap;
        private int m_fromNodeTapPosition;
        private double m_toNodeNominalKv;
        private string m_toNodeTap;
        private int m_toNodeTapPosition;
        private string m_isRemoved;
        private double m_resistance;
        private double m_reactance;
        private double m_magnetizingConductance;
        private double m_magnetizingSusceptance;

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
        public string RegulatedNodeId
        {
            get
            {
                return m_regulatedNodeId;
            }
            set
            {
                m_regulatedNodeId = value;
            }
        }
        public double FromNodeNominalKv
        {
            get
            {
                return m_fromNodeNominalKv;
            }
            set
            {
                m_fromNodeNominalKv = value;
            }
        }
        public string FromNodeTap
        {
            get
            {
                return m_fromNodeTap;
            }
            set
            {
                m_fromNodeTap = value;
            }
        }
        public int FromNodeTapPosition
        {
            get
            {
                return m_fromNodeTapPosition;
            }
            set
            {
                m_fromNodeTapPosition = value;
            }
        }
        public double ToNodeNominalKv
        {
            get
            {
                return m_toNodeNominalKv;
            }
            set
            {
                m_toNodeNominalKv = value;
            }
        }
        public string ToNodeTap
        {
            get
            {
                return m_toNodeTap;
            }
            set
            {
                m_toNodeTap = value;
            }
        }
        public int ToNodeTapPosition
        {
            get
            {
                return m_toNodeTapPosition;
            }
            set
            {
                m_toNodeTapPosition = value;
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
        public double MagnetizingConductance
        {
            get
            {
                return m_magnetizingConductance;
            }
            set
            {
                m_magnetizingConductance = value;
            }
        }
        public double MagnetizingSusceptance
        {
            get
            {
                return m_magnetizingSusceptance;
            }
            set
            {
                m_magnetizingSusceptance = value;
            }
        }

        public Transformer()
        {
        }

        public override string ToString()
        {
            return StationName + "_" + Id;
        }
    }
}
