using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynchrophasorAnalytics.Psse
{
    /// <summary>
    /// 
    /// </summary>
    public class Bus
    {
        #region [ Private Members ]

        private int m_number;
        private string m_name;
        private double m_baseKv;
        private int m_typeCode;
        private int m_areaNumber;
        private int m_ownerNumber;
        private int m_zoneNumber;
        private double m_perUnitVoltageMagnitude;
        private double m_voltageAngleInDegrees;
        private double m_normalHighVoltageLimit;
        private double m_normalLowVoltageLimit;
        private double m_emergencyHighVoltageLimit;
        private double m_emergencyLowVoltageLimit;

        #endregion

        #region [ Public Properties ]

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get
            {
                return m_name;
            }
            set
            {
                m_name = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
        public int TypeCode
        {
            get
            {
                return m_typeCode;
            }
            set
            {
                m_typeCode = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int AreaNumber
        {
            get
            {
                return m_areaNumber;
            }
            set
            {
                m_areaNumber = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int OwnerNumber
        {
            get
            {
                return m_ownerNumber;
            }
            set
            {
                m_ownerNumber = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int ZoneNumber
        {
            get
            {
                return m_zoneNumber;
            }
            set
            {
                m_zoneNumber = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public double PerUnitVoltageMagnitude
        {
            get
            {
                return m_perUnitVoltageMagnitude;
            }
            set
            {
                m_perUnitVoltageMagnitude = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public double VoltageAngleInDegrees
        {
            get
            {
                return m_voltageAngleInDegrees;
            }
            set
            {
                m_voltageAngleInDegrees = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public double NormalHighVoltageLimit
        {
            get
            {
                return m_normalHighVoltageLimit;
            }
            set
            {
                m_normalHighVoltageLimit = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public double NormalLowVoltageLimit
        {
            get
            {
                return m_normalLowVoltageLimit;
            }
            set
            {
                m_normalLowVoltageLimit = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public double EmergencyHighVoltageLimit
        {
            get
            {
                return m_emergencyHighVoltageLimit;
            }
            set
            {
                m_emergencyHighVoltageLimit = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public double EmergencyLowVoltageLimit
        {
            get
            {
                return m_emergencyLowVoltageLimit;
            }
            set
            {
                m_emergencyLowVoltageLimit = value;
            }
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// 
        /// </summary>
        public Bus()
        {
            // Do nothing yet
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static Bus Parse(string line)
        {
            Bus bus = new Bus();
            string[] data = line.Trim('\n').Split(',');
            bus.Number = Convert.ToInt32(data[0]);
            bus.Name = data[1].Trim('\'');
            bus.BaseKv = Convert.ToDouble(data[2]);
            bus.TypeCode = Convert.ToInt32(data[3]);
            bus.AreaNumber = Convert.ToInt32(data[4]);
            bus.OwnerNumber = Convert.ToInt32(data[5]);
            bus.ZoneNumber = Convert.ToInt32(data[6]);
            bus.PerUnitVoltageMagnitude = Convert.ToDouble(data[7]);
            bus.VoltageAngleInDegrees = Convert.ToDouble(data[8]);
            bus.NormalHighVoltageLimit = Convert.ToDouble(data[9]);
            bus.NormalLowVoltageLimit = Convert.ToDouble(data[10]);
            bus.EmergencyHighVoltageLimit = Convert.ToDouble(data[11]);
            bus.EmergencyLowVoltageLimit = Convert.ToDouble(data[12]);

            return bus;
        }


        public string ShortString()
        {
            return m_name;
        }

         

        #endregion
    }
}
