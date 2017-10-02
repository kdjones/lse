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
    public class FixedShunt
    {

        #region [ Private Members ]

        private int m_bus_number;
        private int m_status;
        private string m_identifier;
        private double m_nominal_mw_shunt_value;
        private double m_nominal_mvar_shunt_value;

        #endregion

        #region [ Public Properties ]

        public int BusNumber
        {
            get
            {
                return m_bus_number;
            }
            set
            {
                m_bus_number = value;
            }
        }

        public int Status
        {
            get
            {
                return m_status;
            }
            set
            {
                m_status = value;
            }
        }

        public string Identifier
        {
            get
            {
                return m_identifier;
            }
            set
            {
                m_identifier = value;
            }
        }

        public double NominalMwShuntValue
        {
            get
            {
                return m_nominal_mw_shunt_value;
            }
            set
            {
                m_nominal_mw_shunt_value = value;
            }
        }

        public double NominalMvarShuntValue
        {
            get
            {
                return m_nominal_mvar_shunt_value;
            }
            set
            {
                m_nominal_mvar_shunt_value = value;
            }
        }


        #endregion

        #region [ Constructors ]

        public FixedShunt()
        {
            //
        }

        #endregion

        #region [ Static Methods ]

        public static FixedShunt Parse(string line)
        {
            FixedShunt fixedShunt = new FixedShunt();
            string[] data = line.Trim('\n').Split(',');
            fixedShunt.BusNumber = Convert.ToInt32(data[0]);
            fixedShunt.Identifier = data[1].Trim('\'');
            fixedShunt.Status = Convert.ToInt32(data[2]);
            fixedShunt.NominalMwShuntValue = Convert.ToDouble(data[3]);
            fixedShunt.NominalMvarShuntValue = Convert.ToDouble(data[4]);
            return fixedShunt;
        }

        #endregion
    }
}
