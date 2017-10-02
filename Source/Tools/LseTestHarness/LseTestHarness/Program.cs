// COMPILER GENERATED CODE
// THIS WILL BE OVERWRITTEN AT EACH GENERATION
// EDIT AT YOUR OWN RISK

using System;
using System.Windows.Forms;
using ECAClientFramework;
using LseTestHarness.Model;

namespace LseTestHarness
{
    static class Program
    {
        /// <summary>
        /// Main entry point for LseTestHarness.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Framework framework = new Framework(fw => new Mapper(fw));

            Algorithm.Mapper = framework.Mapper as Mapper;
            Algorithm.UpdateSystemSettings();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            MainWindow mainWindow = new MainWindow(framework);
            mainWindow.Text = "Linear State Estimator Test Harness";
            Application.Run(mainWindow);
        }
    }
}