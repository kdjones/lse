// COMPILER GENERATED CODE
// THIS WILL BE OVERWRITTEN AT EACH GENERATION
// EDIT AT YOUR OWN RISK

using System;
using System.Windows.Forms;
using ECAClientFramework;
using TVA_LSETestHarness.Model;

namespace TVA_LSETestHarness
{
    static class Program
    {
        /// <summary>
        /// Main entry point for TVA_LSETestHarness.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Algorithm.UpdateSystemSettings();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            Framework framework = FrameworkFactory.Create();
            MainWindow mainWindow = new MainWindow(framework);
            mainWindow.Text = "C# TVA_LSETestHarness Test Harness";
            Application.Run(mainWindow);
        }
    }
}