using System;
using ECAClientFramework;
using MeasurementSampler.Model.ECA;
using MeasurementSampler.Model.LSE;
using GSF.Configuration;

namespace MeasurementSampler
{
    public static class Algorithm
    {
        internal class Output
        {
            public NullOutput OutputData = new NullOutput();
            public _NullOutputMeta OutputMeta = new _NullOutputMeta();
            public static Func<Output> CreateNew { get; set; } = () => new Output();
        }

        public static string GetConnectionString()
        {
            ConfigurationFile config = ConfigurationFile.Current;
            CategorizedSettingsElementCollection settings = config.Settings["systemSettings"];
            string connectionString = settings["ConnectionString"].Value;
            return connectionString;
        }

        public static void UpdateSystemSettings()
        {
            SystemSettings.InputMapping = "Input";
            SystemSettings.OutputMapping = "NullOutput";
            SystemSettings.ConnectionString = Algorithm.GetConnectionString();
            SystemSettings.FramesPerSecond = 30;
            SystemSettings.LagTime = 3;
            SystemSettings.LeadTime = 40;

        }
         


        internal static Output Execute(Input inputData, _InputMeta inputMeta)
        {
            Output output = Output.CreateNew();


            return output;
        }
    }
}
