using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynchrophasorAnalytics.Hdb.Records
{
    public class TransformerTap
    {
        private int m_number;
        private string m_id;
        private int m_maximumPosition;
        private int m_minimumPosition;
        private int m_nominalPosition;
        private double m_stepSize;

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

        public int MaximumPosition
        {
            get
            {
                return m_maximumPosition;
            }
            set
            {
                m_maximumPosition = value;
            }
        }

        public int MinimumPosition
        {
            get
            {
                return m_minimumPosition;
            }
            set
            {
                m_minimumPosition = value;
            }
        }

        public int NominalPosition
        {
            get
            {
                return m_nominalPosition;
            }
            set
            {
                m_nominalPosition = value;
            }
        }

        public double StepSize
        {
            get
            {
                return m_stepSize;
            }
            set
            {
                m_stepSize = value;
            }
        }

        public TransformerTap()
        {
        }

        public override string ToString()
        {
            return Id;
        }

    }
}
