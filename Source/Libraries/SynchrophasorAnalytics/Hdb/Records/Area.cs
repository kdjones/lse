using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynchrophasorAnalytics.Hdb.Records
{
    public class Area
    {
        private int m_number;
        private string m_name;

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

        public Area()
        {
        }

        public override string ToString()
        {
            return $"Area  Number: {Number}    Name: {Name}";
        }


    }
}