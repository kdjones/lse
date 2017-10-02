using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ECACommonUtilities;
using ECACommonUtilities.Model;
using GSF.Configuration;
using GSF.TimeSeries.Transport;
using NetworkModelEditor.ViewModels;
using SynchrophasorAnalytics.Measurements;

namespace NetworkModelEditor
{
    public class OpenEcaConnection
    {
        private string m_connectionString;
        private DataSet m_metadata;
        private MainWindowViewModel m_parent;
        private bool m_isConnected = false;
        private DataSubscriber m_subscriber;

        public DataSubscriber Subscriber
        {
            get
            {
                return m_subscriber;
            }
        }

        public bool IsConnected
        {
            get
            {
                return m_isConnected;
            }
        }

        public string ConnectionString
        {
            get
            {
                return m_connectionString;
            }
        }

        public DataSet Metadata
        {
            get
            {
                return m_metadata;
            }
        }

        public static string GetConnectionString()
        {
            ConfigurationFile config = ConfigurationFile.Current;
            CategorizedSettingsElementCollection settings = config.Settings["systemSettings"];
            string connectionString = settings["ConnectionString"].Value;
            return connectionString;
        }

        public OpenEcaConnection(MainWindowViewModel parent)
            :this(OpenEcaConnection.GetConnectionString(), parent)
        {
            
        }

        public OpenEcaConnection(string connectionString, MainWindowViewModel parent)
        {
            m_connectionString = connectionString;
            m_parent = parent;
        }

        public void Connect()
        {
            m_parent.CommunicationStatus = "Attempting Connection...";

            m_subscriber = new DataSubscriber();
            
            m_subscriber.ConnectionString = ConnectionString;

            m_subscriber.ConnectionEstablished += (sender, arg) =>
            {
                // Can send commands through here
                Console.WriteLine("Connection Established");
                m_parent.CommunicationStatus = $"Connection Established with {ConnectionString}";
                m_subscriber.SendServerCommand(ServerCommand.MetaDataRefresh);
                m_isConnected = true;
            };

            m_subscriber.MetaDataReceived += (sender, arg) =>
            {
                m_metadata = arg.Argument;

                Console.WriteLine("Received Metadata");
                m_parent.CommunicationStatus = $"Received MetaData from {ConnectionString}";

            };

            m_subscriber.Initialize();
            m_subscriber.Start();
            
        }

        public void CreateMeasurement(OutputMeasurement measurement)
        {
            MetaSignal metaSignal = new MetaSignal()
            {
                AnalyticProjectName = measurement.DevicePrefix,
                AnalyticInstanceName = measurement.DeviceSuffix,
                SignalType = measurement.SignalType,
                PointTag = measurement.PointTag,
                Description = measurement.Description
            };

            string message = new ConnectionStringParser<SettingAttribute>().ComposeConnectionString(metaSignal);
            Subscriber.SendServerCommand((ServerCommand)ECAServerCommand.MetaSignal, message);
        }

        public void RefreshMetaData()
        {
            Subscriber.SendServerCommand(ServerCommand.MetaDataRefresh);
        }
    }
}
