using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynchrophasorAnalytics.Hdb.Records
{
    public class TransmissionLine
    {
        private int m_number;
        private string m_id;

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

        public TransmissionLine()
        {
        }

        public override string ToString()
        {
            return "Transmission Line:\n  Number:" + Convert.ToString(Number) + "\n      Id:" + Id;
        }
    }
}
