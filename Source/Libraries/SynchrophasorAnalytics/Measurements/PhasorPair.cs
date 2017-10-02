using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynchrophasorAnalytics.Measurements
{
    public class PhasorPair
    {
        #region [ Private Members ] 

        private PhasorBase m_phasorA;
        private PhasorBase m_phasorB;

        #endregion

        #region [ Public Properties ]

        public double AbsoluteAngleDeltaInDegrees
        {
            get
            {
                return Math.Abs(m_phasorA.AngleInDegrees - m_phasorB.AngleInDegrees);
            }
        }

        public double AbsoluteAngleDeltaInRadians
        {
            get
            {
                return Math.Abs(m_phasorA.AngleInRadians - m_phasorB.AngleInRadians);
            }
        }

        public double AbsolutePerUnitMagnitudeDelta
        {
            get
            {
                return Math.Abs(m_phasorA.PerUnitMagnitude - m_phasorB.PerUnitMagnitude);
            }
        }

        public double AbsoluteMagnitudeDelta
        {
            get
            {
                return Math.Abs(m_phasorA.Magnitude - m_phasorB.Magnitude);
            }
        }

        public double AngleDeltaInDegrees
        {
            get
            {
                return m_phasorA.AngleInDegrees - m_phasorB.AngleInDegrees;
            }
        }

        public double AngleDeltaInRadians
        {
            get
            {
                return m_phasorA.AngleInRadians - m_phasorB.AngleInRadians;
            }
        }

        public double PerUnitMagnitudeDelta
        {
            get
            {
                return m_phasorA.PerUnitMagnitude - m_phasorB.PerUnitMagnitude;
            }
        }

        public double MagnitudeDelta
        {
            get
            {
                return m_phasorA.Magnitude - m_phasorB.Magnitude;
            }
        }

        public double TotalVectorDelta
        {
            get
            {
                return Math.Abs((m_phasorA.PerUnitComplexPhasor - m_phasorB.PerUnitComplexPhasor).Magnitude) / m_phasorA.PerUnitMagnitude;
            }
        }

        #endregion

        #region [ Constructor ] 

        public PhasorPair(PhasorBase phasorA, PhasorBase phasorB)
        {
            m_phasorA = phasorA;
            m_phasorB = phasorB;
        }

        #endregion

    }
}
