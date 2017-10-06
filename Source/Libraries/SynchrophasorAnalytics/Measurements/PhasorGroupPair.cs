using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynchrophasorAnalytics.Measurements
{
    public class PhasorGroupPair
    {
        #region [ Private Members ] 

        private PhasorGroup m_groupA;
        private PhasorGroup m_groupB;

        private PhasorPair m_posSeqMeasPair;
        private PhasorPair m_negSeqMeasPair;
        private PhasorPair m_zeroSeqMeasPair;
        private PhasorPair m_phaseAMeasPair;
        private PhasorPair m_phaseBMeasPair;
        private PhasorPair m_phaseCMeasPair;
        private PhasorPair m_posSeqEstPair;
        private PhasorPair m_negSeqEstPair;
        private PhasorPair m_zeroSeqEstPair;
        private PhasorPair m_phaseAEstPair;
        private PhasorPair m_phaseBEstPair;
        private PhasorPair m_phaseCEstPair;

        #endregion

        #region [ Public Properties ]

        public bool IsValid
        {
            get
            {
                if (m_groupA.IncludeInPositiveSequenceEstimator && m_groupB.IncludeInPositiveSequenceEstimator)
                {
                    return true;
                }
                return false;
            }
        }

        public bool GroupAWasReported
        {
            get
            {
                return m_groupA.PositiveSequence.Measurement.MeasurementWasReported;
            }
        }

        public bool GroupBWasReported
        {
            get
            {
                return m_groupB.PositiveSequence.Measurement.MeasurementWasReported;
            }
        }

        public bool MeasurementPairWasReported
        {
            get
            {
                return GroupAWasReported && GroupBWasReported;
            }

        }

        public PhasorGroup GroupA
        {
            get
            {
                return m_groupA;
            }
            set
            {
                m_groupA = value;
                Initialize();
            }
        }

        public PhasorGroup GroupB
        {
            get
            {
                return m_groupB;
            }
            set
            {
                m_groupB = value;
                Initialize();
            }
        }

        public PhasorPair PositiveSequenceMeasurementPair
        {
            get
            {
                return m_posSeqMeasPair;
            }
        }
        
        public PhasorPair NegativeSequenceMeasurementPair
        {
            get
            {
                return m_negSeqMeasPair;
            }
        }
        
        public PhasorPair ZeroSequenceMeasurementPair
        {
            get
            {
                return m_zeroSeqMeasPair;
            }
        }
        
        public PhasorPair PhaseAMeasurementPair
        {
            get
            {
                return m_phaseAMeasPair;
            }
        }

        public PhasorPair PhaseBMeasurementPair
        {
            get
            {
                return m_phaseBMeasPair;
            }
        }
        
        public PhasorPair PhaseCMeasurementPair
        {
            get
            {
                return m_phaseCMeasPair;
            }
        }

        public PhasorPair PositiveSequenceEstimatePair
        {
            get
            {
                return m_posSeqEstPair;
            }
        }

        public PhasorPair NegativeSequenceEstimatePair
        {
            get
            {
                return m_negSeqEstPair;
            }
        }

        public PhasorPair ZeroSequenceEstimatePair
        {
            get
            {
                return m_zeroSeqEstPair;
            }
        }

        public PhasorPair PhaseAEstimatePair
        {
            get
            {
                return m_phaseAEstPair;
            }
        }

        public PhasorPair PhaseBEstimatePair
        {
            get
            {
                return m_phaseBEstPair;
            }
        }

        public PhasorPair PhaseCEstimatePair
        {
            get
            {
                return m_phaseCEstPair;
            }
        }

        #endregion

        #region [ Constructor ]

        public PhasorGroupPair(PhasorGroup groupA, PhasorGroup groupB)
        {
            m_groupA = groupA;
            m_groupB = groupB;
            Initialize();
        }

        #endregion

        #region [ Public Methods ]

        public void Initialize()
        {
            if (m_groupA != null && m_groupB != null)
            {
                m_posSeqMeasPair = new PhasorPair(m_groupA.PositiveSequence.Measurement, m_groupB.PositiveSequence.Measurement);
                m_negSeqMeasPair = new PhasorPair(m_groupA.NegativeSequence.Measurement, m_groupB.NegativeSequence.Measurement);
                m_zeroSeqMeasPair = new PhasorPair(m_groupA.ZeroSequence.Measurement, m_groupB.ZeroSequence.Measurement);
                m_phaseAMeasPair = new PhasorPair(m_groupA.PhaseA.Measurement, m_groupB.PhaseA.Measurement);
                m_phaseBMeasPair = new PhasorPair(m_groupA.PhaseB.Measurement, m_groupB.PhaseB.Measurement);
                m_phaseCMeasPair = new PhasorPair(m_groupA.PhaseC.Measurement, m_groupB.PhaseC.Measurement);
                m_posSeqEstPair = new PhasorPair(m_groupA.PositiveSequence.Estimate, m_groupB.PositiveSequence.Estimate);
                m_negSeqEstPair = new PhasorPair(m_groupA.NegativeSequence.Estimate, m_groupB.NegativeSequence.Estimate);
                m_zeroSeqEstPair = new PhasorPair(m_groupA.ZeroSequence.Estimate, m_groupB.ZeroSequence.Estimate);
                m_phaseAEstPair = new PhasorPair(m_groupA.PhaseA.Estimate, m_groupB.PhaseA.Estimate);
                m_phaseBEstPair = new PhasorPair(m_groupA.PhaseB.Estimate, m_groupB.PhaseB.Estimate);
                m_phaseCEstPair = new PhasorPair(m_groupA.PhaseC.Estimate, m_groupB.PhaseC.Estimate);
            }
        }

        #endregion
    }
}
