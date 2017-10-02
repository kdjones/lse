using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GSF.TimeSeries;
using GSF.TimeSeries.Adapters;
using GSF;
using GSF.PhasorProtocols;

namespace SynchrophasorAnalytics.DataConditioning.Snr
{
    [Serializable()]
    public class MovingWindow
    {
        #region [ Private Fields ]

        private Queue<IMeasurement> m_measurementQueue;
        private int m_windowSize;

        #endregion

        #region [ Properties ]

        [XmlIgnore()]
        public List<IMeasurement> Window
        {
            get
            {
                return m_measurementQueue.ToList();
            }
        }

        [XmlIgnore()]
        public List<double> RawWindow
        {
            get
            {
                List<double> rawWindow = new List<double>();
                foreach (IMeasurement measurement in Window)
                {
                    rawWindow.Add(measurement.Value);
                }
                return rawWindow;
            }
        }

        [XmlAttribute("IsFull")]
        public bool IsFull
        {
            get
            {
                if (m_measurementQueue.Count == m_windowSize)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        [XmlAttribute("WindowSizeInFrames")]
        public int MaxWindowSize
        {
            get
            {
                return m_windowSize;
            }
            set
            {
                m_windowSize = value;
            }
        }

        #endregion

        #region [ Constructors ]

        public MovingWindow()
            : this(30)
        {
        }

        public MovingWindow(int windowSize)
        {
            m_windowSize = windowSize;
            m_measurementQueue = new Queue<IMeasurement>();
        }

        #endregion

        #region [ Public Methods ]

        public virtual void AddNewMeasurement(IMeasurement measurement)
        {
            AddNewMeasurementToQueue(measurement);
            RemoveOldestMeasurementIfWindowSizeIsExceeded();
        }

        public void ClearWindow()
        {
            m_measurementQueue.Clear();
        }

        #endregion

        #region [ Private Methods ]

        private void AddNewMeasurementToQueue(IMeasurement measurement)
        {
            m_measurementQueue.Enqueue(measurement);
        }

        private void RemoveOldestMeasurementIfWindowSizeIsExceeded()
        {
            if (m_measurementQueue.Count > m_windowSize)
            {
                RemoveOldestMeasurementFromQueue();
            }
        }

        private void RemoveOldestMeasurementFromQueue()
        {
            m_measurementQueue.Dequeue();
        }

        #endregion
    }
}
