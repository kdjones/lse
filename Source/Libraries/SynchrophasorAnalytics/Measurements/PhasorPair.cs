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
                double delta = Math.Abs(m_phasorA.AngleInDegrees - m_phasorB.AngleInDegrees);

                if (delta > 180)
                {
                    if (delta < 0)
                    {
                        delta += 360;
                    }
                    else 
                    {
                        delta -= 360;
                    }
                }

                return delta;
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

        /// <summary>
        /// can create a setting to treat angle threshold as TVE magnitude in the future
        /// </summary>
        /// <param name="angleThresholdInDegrees"></param>
        /// <returns></returns>
        public double ComputeEquivalentTotalVectorMagnitudeDelta(double angleThresholdInDegrees)
        {
            double a = Math.Cos(0) - Math.Cos(angleThresholdInDegrees * Math.PI / 180);
            double b = Math.Sin(0) - Math.Sin(angleThresholdInDegrees * Math.PI / 180);

            return Math.Sqrt(a * a + b * b);
        }

    }
}
