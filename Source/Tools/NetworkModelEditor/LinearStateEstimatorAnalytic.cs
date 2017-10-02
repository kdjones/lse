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
    public class LinearStateEstimatorAnalytic : IAnalytic
    {
        #region [ Members ]

        // Fields
        private IMapper m_mapper;
        private Concentrator m_concentrator;
        private Subscriber m_subscriber;
        private object m_analyticHost;
        private Input m_inputData;
        private _InputMeta m_inputMeta;

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

        public object Host
        {
            get
            {
                return m_analyticHost;
            }
        }

        #region [ Constructors ]

        public LinearStateEstimatorAnalytic(object host)
        {
            m_analyticHost = host;
        }

        #endregion

        public void InitializeFramework(Framework framework)
        {
            m_mapper = framework.Mapper;
            m_concentrator = framework.Concentrator;
            m_subscriber = framework.Subscriber;
        }

        public void UpdateStatus()
        {
            string subscriberStatus;
            string concentratorStatus;

            subscriberStatus = m_subscriber.Status;
            concentratorStatus = m_concentrator.Status;

            (m_analyticHost as IAnalyticHost).CommunicationStatus = subscriberStatus;
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

        }

        public void Stop()
        {
            m_subscriber?.Stop();
            m_concentrator?.Stop();
            m_concentrator?.Dispose();
        }

        private void Concentrator_ProcessException(object sender, EventArgs<Exception> args)
        {
            (m_analyticHost as IAnalyticHost).ActionStatus = args.Argument.ToString();
        }

        private void Subscriber_StatusMessage(object sender, EventArgs<string> args)
        {
            (m_analyticHost as IAnalyticHost).CommunicationStatus = args.Argument;
        }

        private void Subscriber_ProcessException(object sender, EventArgs<Exception> args)
        {
            (m_analyticHost as IAnalyticHost).CommunicationStatus = args.Argument.Message;
        }

    }
}
