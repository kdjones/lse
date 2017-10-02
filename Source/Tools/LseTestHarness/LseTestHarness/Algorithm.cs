using System;
using System.Collections.Generic;
using ECAClientFramework;
using LseTestHarness.Model.ECA;
using SynchrophasorAnalytics.Modeling;
using SynchrophasorAnalytics.Networks;
using LseTestHarness.Model;

namespace LseTestHarness
{
    static class Algorithm
    {
        #region [ Private Members ]

        private static PhaseSelection m_phaseConfiguration;
        private static bool m_overridePhaseConfiguration;
        private static string m_configurationPathName;
        private static Network m_network;
        private static int m_frameCount;

        #endregion

        #region [ Properties ]

        public static Mapper Mapper;

        public static string PhaseConfiguration
        {
            get
            {
                return m_phaseConfiguration.ToString();
            }
            set
            {
                if (value.Equals("PositiveSequence"))
                {
                    m_phaseConfiguration = PhaseSelection.PositiveSequence;
                }
                else if (value.Equals("ThreePhase"))
                {
                    m_phaseConfiguration = PhaseSelection.ThreePhase;
                }
            }
        }

        public static bool OverridePhaseConfiguration
        {
            get
            {
                return m_overridePhaseConfiguration;
            }
            set
            {
                m_overridePhaseConfiguration = value;
            }
        }

        public static string ConfigurationPathName
        {
            get
            {
                return m_configurationPathName;
            }
            set
            {
                m_configurationPathName = value;
            }
        }

        #endregion

        internal class Output
        {
            public NullOutput OutputData = new NullOutput();
            public _NullOutputMeta OutputMeta = new _NullOutputMeta();
        }

        #region [ Constructor ] 

        static Algorithm()
        {
            m_phaseConfiguration = PhaseSelection.PositiveSequence;
            MainWindow.WriteMessage("Set for positive sequence phase configuration.");
            m_configurationPathName = @"C:\Users\kevin\OneDrive\Documents\VT Data\Models\IEEE 118 Bus - Nodal Variety (PRUNED-KEYED-NOSF-RCF-v1).xml";
            m_frameCount = 0;
        }

        #endregion

        public static void UpdateSystemSettings()
        {
            SystemSettings.ConnectionString = @"server=localhost:6190; interface=0.0.0.0";
            SystemSettings.FramesPerSecond = 30;
            SystemSettings.LagTime = 3;
            SystemSettings.LeadTime = 1;

            try
            {
                m_network = Network.DeserializeFromXml(m_configurationPathName);

                if (m_network == null)
                {
                    MainWindow.WriteMessage("Failed to deserialize the network model from: " + m_configurationPathName);
                }

                m_network.Initialize();
                if (m_overridePhaseConfiguration)
                {
                    m_network.Model.PhaseConfiguration = m_phaseConfiguration;
                }
                m_network.Model.AcceptsEstimates = false;
                m_network.Model.AcceptsMeasurements = true;
                m_network.SerializeData(true);
                MainWindow.WriteMessage("Successfully read configuration...");
            }
            catch (Exception ex)
            {
                // Display exceptions to the main window
                MainWindow.WriteError(new InvalidOperationException($"Update settings exception: {ex.Message}", ex));
            }
        }

        public static Output Execute(PhasorCollection inputData, _PhasorCollectionMeta inputMeta)
        {
            Output output = new Output();
            if (m_frameCount == 100)
            {
                m_frameCount = 0;
            }
            try
            {
                m_network.Model.InputKeyValuePairs.Clear();
                m_network.Model.ClearValues();

                for (int i = 0; i < inputData.Phasors.Length; i++)
                {
                    Phasor phasor = inputData.Phasors[i];
                    _PhasorMeta metaData = inputMeta.Phasors[i];


                    string magnitudeKey = HistorianIdFromGuid(inputMeta.Phasors[i].Magnitude.ID);
                    string angleKey = HistorianIdFromGuid(inputMeta.Phasors[i].Angle.ID);
                    m_network.Model.InputKeyValuePairs.Add(magnitudeKey, phasor.Magnitude);
                    m_network.Model.InputKeyValuePairs.Add(angleKey, phasor.Angle);
                }

                m_network.Model.OnNewMeasurements();

                m_network.RunNetworkReconstructionCheck();
                if (m_network.HasChangedSincePreviousFrame)
                {
                    m_network.Model.DetermineActiveCurrentFlows();
                    m_network.Model.DetermineActiveCurrentInjections();
                }

                if (m_network.HasChangedSincePreviousFrame)
                {
                    m_network.Model.ResolveToObservedBuses();
                    m_network.Model.ResolveToSingleFlowBranches();

                }

                m_network.ComputeSystemState();
                

                if (m_frameCount % 33 == 0)
                {

                    // UNCOMMENT FOR TEST HARNESS DEMO

                    //MainWindow.WriteMessage("");
                    //MainWindow.WriteMessage("Sampled Output");
                    //MainWindow.WriteMessage("");
                    //foreach (Substation substation in m_network.Model.Substations)
                    //{
                    //    MainWindow.WriteMessage($"{substation.Name}");
                    //    foreach (Node node in substation.Nodes)
                    //    {
                    //        MainWindow.WriteMessage($"{node.Name}: {node.Voltage.PositiveSequence.Estimate.PrettyStringValue}");
                    //    }
                    //    MainWindow.WriteMessage("");
                    //}

                    // UNCOMMENT FOR TOPOLOGY DETECTION DEMO

                    MainWindow.WriteMessage("");
                    MainWindow.WriteMessage("Level 2 Topology Assessment");
                    MainWindow.WriteMessage("");
                    foreach (Substation substation in m_network.Model.Substations)
                    {
                        if (substation.Name == "Rannish")
                        {
                            MainWindow.WriteMessage($"{substation.Name}");
                            foreach (CircuitBreaker breaker in substation.CircuitBreakers)
                            {
                                if (breaker.CrossDevicePhasors != null)
                                {
                                    if (breaker.CrossDevicePhasors.GroupA != null && breaker.CrossDevicePhasors.GroupB != null)
                                    {
                                        MainWindow.WriteMessage($"{breaker.FromNode.Name} to {breaker.ToNode.Name}: {breaker.InferredState}");

                                    }
                                }
                            }
                        }
                    }

                }


            }
            catch (Exception ex)
            {
                // Display exceptions to the main window
                MainWindow.WriteError(new InvalidOperationException($"Algorithm exception: {ex.Message}", ex));
            }

            m_frameCount++;
            return output;
        }

        public static string HistorianIdFromGuid(Guid guid)
        {
            return Mapper.MetdataCache.Tables["MeasurementDetail"].Select($"SignalID = '{guid}'")[0]["ID"].ToString();
        }
    }
}
