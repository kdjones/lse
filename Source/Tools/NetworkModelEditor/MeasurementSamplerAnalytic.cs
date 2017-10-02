using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using GSF;
using GSF.Threading;
using ECAClientFramework;
using ECAClientUtilities;
using MeasurementSampler;
using MeasurementSampler.Model.LSE;
using SynchrophasorAnalytics.Testing;
using SynchrophasorAnalytics.Measurements;

namespace NetworkModelEditor
{
    public class MeasurementSamplerAnalytic : IAnalytic
    {
        #region [ Members ]

        // Fields
        private IMapper m_mapper;
        private Concentrator m_concentrator;
        private Subscriber m_subscriber;
        private object m_analyticHost;
        private Input m_inputData;
        private _InputMeta m_inputMeta;
        private bool m_shouldTakeSample;

        #endregion

        public Input InputData
        {
            get
            {
                return m_inputData;
            }
            set
            {
                m_inputData = value;
            }
        }

        public _InputMeta InputMeta
        {
            get
            {
                return m_inputMeta;
            }
            set
            {
                m_inputMeta = value;
            }
        }

        public bool ShouldTakeSample
        {
            get
            {
                return m_shouldTakeSample;
            }
            set
            {
                m_shouldTakeSample = value;
            }
        }

        public object Host
        {
            get
            {
                return m_analyticHost;
            }
        }

        #region [ Constructors ]

        public MeasurementSamplerAnalytic(object host)
        {
            m_analyticHost = host;
        }

        #endregion

        public void InitializeFramework(Framework framework)
        {
            m_mapper = framework.Mapper;
            m_concentrator = framework.Concentrator;
            m_subscriber = framework.Subscriber;
            m_shouldTakeSample = false;
        }

        public void UpdateStatus()
        {
            string subscriberStatus;
            string concentratorStatus;

            subscriberStatus = m_subscriber.Status;
            concentratorStatus = m_concentrator.Status;

            (m_analyticHost as IAnalyticHost).CommunicationStatus = subscriberStatus;
            //m_analyticHost.ActionStatus = concentratorStatus;
        }

        public void Start()
        {

            m_concentrator.ProcessException += Concentrator_ProcessException;
            m_concentrator.FramesPerSecond = SystemSettings.FramesPerSecond;
            m_concentrator.LagTime = SystemSettings.LagTime;
            m_concentrator.LeadTime = SystemSettings.LeadTime;
            m_concentrator.RoundToNearestTimestamp = true;
            m_concentrator.Start();

            m_subscriber.StatusMessage += Subscriber_StatusMessage;
            m_subscriber.ProcessException += Subscriber_ProcessException;
            m_subscriber.Start();

            new Action(UpdateStatus).DelayAndExecute(1000);
        }

        public void Execute()
        {
            if (ShouldTakeSample)
            {
                List<RawMeasurementsMeasurement> measurements = new List<RawMeasurementsMeasurement>();

                for (int i = 0; i < InputData.Digitals.Values.Length; i++)
                {
                    Guid key = InputMeta.Digitals.Values[i].ID;
                    string value = InputData.Digitals.Values[i].ToString();

                    if (key != Guid.Empty)
                    {
                        measurements.Add(new RawMeasurementsMeasurement()
                        {
                            Key = key.ToString(),
                            Value = value
                        });
                    }
                }

                for (int i = 0; i < InputData.StatusWords.Values.Length; i++)
                {
                    Guid key = InputMeta.StatusWords.Values[i].ID;
                    string value = InputData.StatusWords.Values[i].ToString();

                    if (key != Guid.Empty)
                    {
                        measurements.Add(new RawMeasurementsMeasurement()
                        {
                            Key = key.ToString(),
                            Value = value
                        });
                    }
                }

                for (int i = 0; i < InputData.VoltagePhasors.Phasors.Length; i++)
                {
                    Guid magnitudeKey = InputMeta.VoltagePhasors.Phasors[i].Magnitude.ID;
                    string magnitude = InputData.VoltagePhasors.Phasors[i].Magnitude.ToString();
                    Guid angleKey = InputMeta.VoltagePhasors.Phasors[i].Angle.ID;
                    string angle = InputData.VoltagePhasors.Phasors[i].Angle.ToString();

                    if (magnitudeKey != Guid.Empty)
                    {
                        measurements.Add(new RawMeasurementsMeasurement()
                        {
                            Key = magnitudeKey.ToString(),
                            Value = magnitude
                        });
                    }
                    if (angleKey != Guid.Empty)
                    {
                        measurements.Add(new RawMeasurementsMeasurement()
                        {
                            Key = angleKey.ToString(),
                            Value = angle
                        });
                    }
                }

                for (int i = 0; i < InputData.CurrentPhasors.Phasors.Length; i++)
                {
                    Guid magnitudeKey = InputMeta.CurrentPhasors.Phasors[i].Magnitude.ID;
                    string magnitude = InputData.CurrentPhasors.Phasors[i].Magnitude.ToString();
                    Guid angleKey = InputMeta.CurrentPhasors.Phasors[i].Angle.ID;
                    string angle = InputData.CurrentPhasors.Phasors[i].Angle.ToString();

                    if (magnitudeKey != Guid.Empty)
                    {
                        measurements.Add(new RawMeasurementsMeasurement()
                        {
                            Key = magnitudeKey.ToString(),
                            Value = magnitude
                        });
                    }
                    if (angleKey != Guid.Empty)
                    {
                        measurements.Add(new RawMeasurementsMeasurement()
                        {
                            Key = angleKey.ToString(),
                            Value = angle
                        });
                    }
                }

                RawMeasurements sample = new RawMeasurements();
                sample.Items = measurements.ToArray();

                (m_analyticHost as IAnalyticHost).AddMeasurementSample(sample);
                string message = GetStatusWordsStatisticsMessage(InputData, InputMeta);
                message += GetDigialsStatisticsMessage(InputData, InputMeta);
                message += GetVoltagePhasorStatisticsMessage(InputData, InputMeta);
                message += GetCurrentPhasorStatisticsMessage(InputData, InputMeta);
                (m_analyticHost as IAnalyticHost).CommunicationStatus = message;
                ShouldTakeSample = false;
            }
        }

        public string HistorianIdFromGuid(Guid guid)
        {
            if (guid != Guid.Empty)
            {
                return (m_mapper as MapperBase).MetdataCache.Tables["MeasurementDetail"].Select($"SignalID = '{guid}'")[0]["ID"].ToString();
            }
            return "Empty Guid";
        }

        public void Stop()
        {
            m_subscriber?.Stop();
            m_concentrator?.Stop();
            m_concentrator?.Dispose();
        }

        public static string GetStatusWordsStatisticsMessage(Input inputData, _InputMeta inputMeta)
        {
            int nonEmptyStatusWordCount = 0;
            int statusWordsWithoutErrorsCount = 0;
            int validStatusWordsForLSECount = 0;

            for (int i = 0; i < inputMeta.StatusWords.Values.Length; i++)
            {
                if (inputMeta.StatusWords.Values[i].ID != Guid.Empty)
                {
                    nonEmptyStatusWordCount++;
                    if (inputData.StatusWords.Values[i] == 0.0)
                    {
                        statusWordsWithoutErrorsCount++;
                    }
                    StatusWord statusWord = new StatusWord();
                    statusWord.BinaryValue = inputData.StatusWords.Values[i];
                    if (!(statusWord.DataIsValid || statusWord.SynchronizationIsValid))
                    {
                        validStatusWordsForLSECount++;
                    }
                }
            }

            return $"R{nonEmptyStatusWordCount}-G{statusWordsWithoutErrorsCount}-V{validStatusWordsForLSECount}/{inputMeta.StatusWords.Values.Length} ";
        }

        public static string GetVoltagePhasorStatisticsMessage(Input inputData, _InputMeta inputMeta)
        {
            int nonEmptyVoltagePhasorsCount = 0;

            for (int i = 0; i < inputMeta.VoltagePhasors.Phasors.Length; i++)
            {
                if (inputMeta.VoltagePhasors.Phasors[i].Magnitude.ID != Guid.Empty && inputMeta.VoltagePhasors.Phasors[i].Angle.ID != Guid.Empty)
                {
                    nonEmptyVoltagePhasorsCount++;
                }
            }

            return $"RV: {nonEmptyVoltagePhasorsCount}/{inputMeta.VoltagePhasors.Phasors.Length} ";
        }

        public static string GetCurrentPhasorStatisticsMessage(Input inputData, _InputMeta inputMeta)
        {
            int nonEmptyCurrentPhasorsCount = 0;

            for (int i = 0; i < inputMeta.CurrentPhasors.Phasors.Length; i++)
            {
                if (inputMeta.CurrentPhasors.Phasors[i].Magnitude.ID != Guid.Empty && inputMeta.CurrentPhasors.Phasors[i].Angle.ID != Guid.Empty)
                {
                    nonEmptyCurrentPhasorsCount++;
                }
            }

            return $"RI: {nonEmptyCurrentPhasorsCount}/{inputMeta.CurrentPhasors.Phasors.Length} ";
        }

        public static string GetDigialsStatisticsMessage(Input inputData, _InputMeta inputMeta)
        {
            int nonEmptyDigitalsCount = 0;

            for (int i = 0; i < inputMeta.Digitals.Values.Length; i++)
            {
                if (inputMeta.Digitals.Values[i].ID != Guid.Empty)
                {
                    nonEmptyDigitalsCount++;
                }
            }

            return $"RD: {nonEmptyDigitalsCount}/{inputMeta.Digitals.Values.Length} ";
        }

        private void Concentrator_ProcessException(object sender, EventArgs<Exception> args)
        {
            (m_analyticHost as IAnalyticHost).ActionStatus = args.Argument.ToString();
            MessageBox.Show(args.Argument.ToString());
        }

        private void Subscriber_StatusMessage(object sender, EventArgs<string> args)
        {
            (m_analyticHost as IAnalyticHost).CommunicationStatus = args.Argument;
        }

        private void Subscriber_ProcessException(object sender, EventArgs<Exception> args)
        {
            (m_analyticHost as IAnalyticHost).CommunicationStatus = args.Argument.Message;
            MessageBox.Show(args.Argument.ToString());
        }


    }
}
