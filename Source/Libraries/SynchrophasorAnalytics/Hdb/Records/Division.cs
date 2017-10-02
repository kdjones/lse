using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynchrophasorAnalytics.Hdb.Records
{
    public class Division
    {
        private int m_number;
        private string m_name;
        private int m_areaNumber;

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

        public Division()
        {
        }

        public override string ToString()
        {
            return "Division: Number:" + Convert.ToString(Number) + " Name:" + Name + " Area:" + Convert.ToString(AreaNumber);
        }
    }
}
