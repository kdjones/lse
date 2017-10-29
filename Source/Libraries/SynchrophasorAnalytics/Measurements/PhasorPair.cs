using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
                double phaseAAngleInDegrees = m_phasorA.AngleInDegrees;
                double phaseBAngleInDegrees = m_phasorB.AngleInDegrees;
                bool phaseAIsPositive = phaseAAngleInDegrees >= 0;
                bool phaseBIsPositive = phaseBAngleInDegrees >= 0;

                if (phaseAIsPositive && !phaseBIsPositive)
                {
                    phaseBAngleInDegrees += 360;
                }
                else if (!phaseAIsPositive && phaseBIsPositive)
                {
                    phaseAAngleInDegrees += 360;
                }

                return Math.Abs(phaseAAngleInDegrees - phaseBAngleInDegrees);
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

        public double NormalizedTotalVectorDeltaMagnitude
        {
            get
            {
                Complex phaseAUnitVector = m_phasorA.PerUnitComplexPhasor / m_phasorA.PerUnitMagnitude;
                Complex phaseBUnitVector = m_phasorB.PerUnitComplexPhasor / m_phasorB.PerUnitMagnitude;
                return Math.Abs((phaseAUnitVector - phaseBUnitVector).Magnitude);
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
