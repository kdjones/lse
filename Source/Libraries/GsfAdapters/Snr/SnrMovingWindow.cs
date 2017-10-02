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
    public class SnrMovingWindow : MovingWindow
    {
        #region [ Private Fields ]

        private SnrMeasurementType m_measurementType;
        private string m_inputMeasurementKey;
        private string m_outputMeasurementKey;
        private double m_arithmeticMean;
        private double m_standardDeviation;
        private double m_signalToNoiseRatio;
        private int m_kValue;
        private List<double> m_signal;
        private List<double> m_residuals;
        private List<double> m_derivativeOfSignal;
        private Queue<double> m_wrappedSignal;
        private double m_residualMean;
        private IMeasurement m_latestMeasurement;
        private bool m_shouldSerializeSignalData;

        #endregion

        #region [ Properties ]

        [XmlAttribute("MeasurementType")]
        public SnrMeasurementType MeasurementType
        {
            get
            {
                return m_measurementType;
            }
            set
            {
                m_measurementType = value;
            }
        }

        [XmlAttribute("Input")]
        public string InputMeasurementKey
        {
            get
            {
                return m_inputMeasurementKey;
            }
            set
            {
                m_inputMeasurementKey = value;
            }
        }

        [XmlAttribute("Output")]
        public string OutputMeasurementKey
        {
            get
            {
                return m_outputMeasurementKey;
            }
            set
            {
                m_outputMeasurementKey = value;
            }
        }

        [XmlAttribute("Mean")]
        public double ArithmeticMean
        {
            get
            {
                return m_arithmeticMean;
            }
            set
            {
            }
        }

        [XmlAttribute("StandardDeviation")]
        public double StandardDeviation
        {
            get
            {
                return m_standardDeviation;
            }
            set
            {
            }
        }

        [XmlAttribute("SignalToNoiseRatio")]
        public double SignalToNoiseRatio
        {
            get
            {
                return m_signalToNoiseRatio;
            }
            set
            {
            }
        }

        [XmlAttribute("K")]
        public int KValue
        {
            get
            {
                return m_kValue;
            }
        }

        [XmlArray("Signal")]
        public List<double> Signal
        {
            get
            {
                return m_signal;
            }
            set
            {
                m_signal = value;
            }
        }

        public bool ShouldSerializeSignal()
        {
            return m_shouldSerializeSignalData;
        }

        public bool ShouldSerializeKValue()
        {
            return m_shouldSerializeSignalData;
        }

        #endregion

        #region [ Constructors ]

        public SnrMovingWindow()
            : this(30)
        {
        }

        public SnrMovingWindow(int windowSize)
            : base(windowSize)
        {
            Initialize();
        }

        #endregion

        #region [ Public Methods ]

        public override void AddNewMeasurement(IMeasurement measurement)
        {
            if (m_measurementType == SnrMeasurementType.Magnitude)
            {
                AddNewMagnitudeMeasurement(measurement);

            }
            else if (m_measurementType == SnrMeasurementType.Angle)
            {
                AddNewAngleMeasurement(measurement);
            }

            ExecuteCalculations();
        }

        public void SerializeToXml(string pathName)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(SnrMovingWindow));

                TextWriter writer = new StreamWriter(pathName);

                serializer.Serialize(writer, this);

                writer.Close();
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to Serialize the MovingWindow to the Snapshot File: " + exception.ToString());
            }
        }

        public static SnrMovingWindow DeserializeFromXml(string pathName)
        {
            try
            {
                SnrMovingWindow movingWindow = null;

                XmlSerializer deserializer = new XmlSerializer(typeof(SnrMovingWindow));

                StreamReader reader = new StreamReader(pathName);

                movingWindow = (SnrMovingWindow)deserializer.Deserialize(reader);

                reader.Close();

                return movingWindow;
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to Deserialize the Moving Window from the Configuration File: " + exception.ToString());
            }
        }

        public void EnableDataSerialization()
        {
            m_shouldSerializeSignalData = true;
        }

        public void DisableDataSerialization()
        {
            m_shouldSerializeSignalData = false;
        }

        #endregion

        #region [ Private Methods ]

        private void Initialize()
        {
            m_shouldSerializeSignalData = false;
            m_residuals = new List<double>();
            m_signal = new List<double>();
            m_derivativeOfSignal = new List<double>();
            m_wrappedSignal = new Queue<double>();
        }

        private void ExecuteCalculations()
        {
            CopySignalFromWindowForCalculations();
            CalculateArithmeticMeanOfSignal();
            CalculateStandardDeviationOfSignal();
            CalculateSignalToNoiseRatio();
        }

        private void CopySignalFromWindowForCalculations()
        {
            CopyOriginalSignalFromWindow();
            if (m_signal.Count == 0)
            {
                throw new Exception("Signal is empty from before second copy");
            }
            if (m_measurementType == SnrMeasurementType.Angle)
            {
                CopyDerivativeOfSignalFromWindow();
            }

        }

        private void CalculateSignalToNoiseRatio()
        {
            m_signalToNoiseRatio = 10 * Math.Log10(Math.Abs(m_arithmeticMean) / m_standardDeviation);
        }

        private void AddNewMagnitudeMeasurement(IMeasurement measurement)
        {
            base.AddNewMeasurement(measurement);
        }

        private void AddNewAngleMeasurement(IMeasurement measurement)
        {
            SaveRawMeasurement(measurement);
            UnwrapPhaseAngle();
            SendUnwrappedMeasurementToMovingWindow();
        }

        private void CopyDerivativeOfSignalFromWindow()
        {
            if (m_signal.Count == 0)
            {
                throw new Exception("Signal is empty from before derivative");
            }
            CalculateDerivativeOfSignal();
            SaveDerivativeOfSignalAsSignal();
        }

        private void CopyOriginalSignalFromWindow()
        {
            m_signal.Clear();

            foreach (IMeasurement measurement in Window)
            {
                m_signal.Add(measurement.Value);
            }

        }

        private void CalculateArithmeticMeanOfSignal()
        {
            double summation = 0;

            foreach (double measurement in m_signal)
            {
                summation += measurement;
            }

            m_arithmeticMean = summation / m_signal.Count;
        }

        private void CalculateStandardDeviationOfSignal()
        {
            CalculateResidualsForStandardDeviation();

            CalculateArithmeticMeanOfResiduals();

            SaveStandardDeviationResults();
        }

        private void CalculateResidualsForStandardDeviation()
        {
            m_residuals.Clear();

            foreach (double measurement in m_signal)
            {
                m_residuals.Add((measurement - m_arithmeticMean) * (measurement - m_arithmeticMean));
            }
        }

        private void CalculateArithmeticMeanOfResiduals()
        {
            double summation = 0;

            foreach (double residual in m_residuals)
            {
                summation += residual;
            }

            m_residualMean = summation / m_residuals.Count;
        }

        private void SaveStandardDeviationResults()
        {
            m_standardDeviation = Math.Sqrt(m_residualMean);
        }

        private void SaveRawMeasurement(IMeasurement measurement)
        {
            AddNewWrappedMeasurementToQueue(measurement);
            CopyNewWrappedMeasurementLocally(measurement);
            RemoveOldestWrappedMeasurementIfWindowSizeIsExceeded();
        }

        private void AddNewWrappedMeasurementToQueue(IMeasurement measurement)
        {
            m_wrappedSignal.Enqueue(measurement.Value);
        }

        private void CopyNewWrappedMeasurementLocally(IMeasurement measurement)
        {
            m_latestMeasurement = measurement;
        }

        private void RemoveOldestWrappedMeasurementIfWindowSizeIsExceeded()
        {
            if (m_wrappedSignal.Count > MaxWindowSize)
            {
                RemoveOldestWrappedMeasurementFromQueue();
            }
        }

        private void RemoveOldestWrappedMeasurementFromQueue()
        {
            m_wrappedSignal.Dequeue();
        }

        private void UnwrapPhaseAngle()
        {
            CalculateKValue();
            CalculateUnwrappedPhaseAngle();
        }

        private void CalculateKValue()
        {
            List<double> phaseAngle = m_wrappedSignal.ToList();

            if (phaseAngle.Count > 2)
            {
                if (Math.Abs(phaseAngle[phaseAngle.Count - 1] - phaseAngle[phaseAngle.Count - 2]) > 180)
                {
                    if (phaseAngle[phaseAngle.Count - 1] < phaseAngle[phaseAngle.Count - 2])
                    {
                        m_kValue++;
                    }
                    else
                    {
                        m_kValue--;
                    }
                }
            }
        }

        private void CalculateUnwrappedPhaseAngle()
        {
            m_latestMeasurement.Value = m_wrappedSignal.ToList()[m_wrappedSignal.Count - 1] + 360 * m_kValue;
        }

        private void SendUnwrappedMeasurementToMovingWindow()
        {
            base.AddNewMeasurement(m_latestMeasurement);
        }

        private void CalculateDerivativeOfSignal()
        {
            m_derivativeOfSignal.Clear();

            if (m_signal.Count == 0)
            {
                throw new Exception("Signal is empty from derivative");
            }
            for (int i = 1; i < m_signal.Count; i++)
            {
                m_derivativeOfSignal.Add(Math.Abs(m_signal[i] - m_signal[i - 1]));
            }
        }

        private void SaveDerivativeOfSignalAsSignal()
        {
            double[] derivativeOfSignal = m_derivativeOfSignal.ToArray();
            m_signal = derivativeOfSignal.ToList();
        }

        #endregion
    }
}
