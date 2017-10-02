using System;
using ECAClientFramework;
using TVA_LSETestHarness.Model.ECA;

namespace TVA_LSETestHarness
{
    public static class Algorithm
    {
        internal class Output
        {
            public PhasorCollection OutputData = new PhasorCollection();
            public _PhasorCollectionMeta OutputMeta = new _PhasorCollectionMeta();
            public static Func<Output> CreateNew { get; set; } = () => new Output();
        }

        public static void UpdateSystemSettings()
        {
            SystemSettings.InputMapping = "LSEDemo";
            SystemSettings.OutputMapping = "LSEDemoOutput";
            SystemSettings.ConnectionString = @"server=localhost:6190; interface=0.0.0.0";
            SystemSettings.FramesPerSecond = 30;
            SystemSettings.LagTime = 3;
            SystemSettings.LeadTime = 1;
        }

        internal static Output Execute(PhasorCollection inputData, _PhasorCollectionMeta inputMeta)
        {
            Output output = Output.CreateNew();

            try
            {
                // TODO: Implement your algorithm here...
                // You can also write messages to the main window:
                MainWindow.WriteMessage("Hello, World!");
            }
            catch (Exception ex)
            {
                // Display exceptions to the main window
                MainWindow.WriteError(new InvalidOperationException($"Algorithm exception: {ex.Message}", ex));
            }

            return output;
        }
    }
}
