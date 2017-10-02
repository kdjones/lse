using System;
using System.Collections.Generic;
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
    public class SnrMovingWindowCollection
    {
        #region [ Private Fields ]

        private List<SnrMovingWindow> m_movingWindows;
        private Dictionary<string, SnrMovingWindow> m_movingWindowsKeyedByInput;
        private Dictionary<string, SnrMovingWindow> m_movingWindowsKeyedByOutput;

        #endregion

        #region [ Properties ]

        [XmlArray("Signals")]
        public List<SnrMovingWindow> Windows
        {
            get
            {
                return m_movingWindows;
            }
            set
            {
                m_movingWindows = value;
            }
        }

        [XmlIgnore()]
        public Dictionary<string, SnrMovingWindow> WindowsKeyedByInput
        {
            get
            {
                return m_movingWindowsKeyedByInput;
            }
            set
            {
                m_movingWindowsKeyedByInput = value;
            }

        }

        [XmlIgnore()]
        public Dictionary<string, SnrMovingWindow> WindowsKeyedByOutput
        {
            get
            {
                return m_movingWindowsKeyedByOutput;
            }
            set
            {
                m_movingWindowsKeyedByOutput = value;
            }

        }

        [XmlIgnore()]
        public List<string> InputMeasurementKeys
        {
            get
            {
                List<string> inputMeasurementKeys = new List<string>();

                foreach (SnrMovingWindow window in Windows)
                {
                    inputMeasurementKeys.Add(window.InputMeasurementKey);
                }

                return inputMeasurementKeys;
            }
        }

        #endregion

        #region [ Constructors ]

        public SnrMovingWindowCollection()
        {
            m_movingWindows = new List<SnrMovingWindow>();
            m_movingWindowsKeyedByInput = new Dictionary<string, SnrMovingWindow>();
            m_movingWindowsKeyedByOutput = new Dictionary<string, SnrMovingWindow>();
        }

        #endregion

        #region [ Public Methods ]

        public void SerializeToXml(string pathName)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(SnrMovingWindowCollection));

                TextWriter writer = new StreamWriter(pathName);

                serializer.Serialize(writer, this);

                writer.Close();
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to serialize the moving window collection to the configuration file: " + exception.ToString());
            }
        }

        public static SnrMovingWindowCollection DeserializeFromXml(string pathName)
        {
            try
            {
                SnrMovingWindowCollection movingWindows = null;

                XmlSerializer deserializer = new XmlSerializer(typeof(SnrMovingWindowCollection));

                StreamReader reader = new StreamReader(pathName);

                movingWindows = (SnrMovingWindowCollection)deserializer.Deserialize(reader);

                reader.Close();

                return movingWindows;
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to deserialize the moving window collection from the configuration file: " + exception.ToString());
            }
        }

        public void AddNewMeasurement(IMeasurement measurement)
        {
            SnrMovingWindow movingWindow = null;

            if (m_movingWindowsKeyedByInput.TryGetValue(measurement.Key.ToString(), out movingWindow))
            {
                movingWindow.AddNewMeasurement(measurement);
            }
        }

        public void EnableDataSerialization()
        {
            foreach (SnrMovingWindow window in m_movingWindows)
            {
                window.EnableDataSerialization();
            }
        }

        public void DisableDataSerialization()
        {
            foreach (SnrMovingWindow window in m_movingWindows)
            {
                window.DisableDataSerialization();
            }
        }

        public void Initialize()
        {
            CreateDictionaryKeyedWithMeasurementKeys();
        }

        #endregion

        #region [ Private Methods ]

        private void CreateDictionaryKeyedWithMeasurementKeys()
        {
            m_movingWindowsKeyedByInput.Clear();
            m_movingWindowsKeyedByOutput.Clear();

            foreach (SnrMovingWindow window in m_movingWindows)
            {
                m_movingWindowsKeyedByInput.Add(window.InputMeasurementKey, window);
                m_movingWindowsKeyedByOutput.Add(window.OutputMeasurementKey, window);
            }
        }

        #endregion
    }
}
