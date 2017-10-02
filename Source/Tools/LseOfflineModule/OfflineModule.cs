using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Xml.Serialization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SynchrophasorAnalytics.Measurements;
using SynchrophasorAnalytics.Modeling;
using SynchrophasorAnalytics.Networks;
using SynchrophasorAnalytics.Testing;
using SynchrophasorAnalytics.Matrices;
using SynchrophasorAnalytics.Calibration;
using SynchrophasorAnalytics.DataConditioning;

namespace LinearStateEstimatorOfflineModule
{
    public partial class OfflineModule : Form
    {
        #region [ Private Fields ]

        private RawMeasurements m_rawMeasurements;
        private Network m_network;

        private bool m_networkIsInitialized;
        private bool m_measurementsAreMapped;
        private bool m_activeCurrentFlowsHaveBeenDetermined;
        private bool m_activeCurrentInjectionsHaveBeenDetermined;
        private bool m_observedBussesHaveBeenResolved;
        private bool m_stateWasComputed;

        #endregion

        #region [ Constructors ]

        public OfflineModule()
        {
            m_rawMeasurements = null;
            m_network = null;

            m_networkIsInitialized = false;
            m_measurementsAreMapped = false;
            m_activeCurrentFlowsHaveBeenDetermined = false;
            m_observedBussesHaveBeenResolved = false;
            m_stateWasComputed = false;

            InitializeComponent();

            DisableControls();
        }

        #endregion

        #region [ Private Methods ]

        #region [ Enable Controls ]

        private void EnableControls()
        {
            if (m_rawMeasurements != null && m_network != null)
            {
                InitializeNetworkModelButton.Enabled = true;
            }
            if (m_networkIsInitialized)
            {
                SerializeButton.Enabled = true;
                MapMeasurementsButton.Enabled = true;
                CheckComponentsButton.Enabled = true;
            }
            if (m_measurementsAreMapped)
            {
                DetermineActiveCurrentPhasorsButton.Enabled = true;
                ViewReceivedMeasurementsButton.Enabled = true;
                ViewUnreceivedMeasurementsButton.Enabled = true;

                ViewModeledVoltagesButton.Enabled = true;
                ViewExpectedVoltagesButton.Enabled = true;
                ViewActiveVoltagesButton.Enabled = true;
                ViewInactiveVoltagesButton.Enabled = true;
            }
            if (m_activeCurrentFlowsHaveBeenDetermined)
            {
                DetermineActiveCurrentInjectionsButton.Enabled = true;
                ViewCalculatedImpedancesButton.Enabled = true;
                ViewModeledCurrentFlowsButton.Enabled = true;
                ViewExpectedCurrentFlowsButton.Enabled = true;
                ViewActiveCurrentFlowsButton.Enabled = true;
                ViewInactiveCurrentFlowsButton.Enabled = true;
                ViewIncludedCurrentFlowsButton.Enabled = true;
                ViewExcludedCurrentFlowsButton.Enabled = true;

            }
            if (m_activeCurrentInjectionsHaveBeenDetermined)
            {
                ResolveToObservedBussesButton.Enabled = true;
                ResolveToSingleFlowBranchesButton.Enabled = true;
                ViewModeledCurrentInjectionsButton.Enabled = true;
                ViewExpectedCurrentInjectionsButton.Enabled = true;
                ViewActiveCurrentInjectionsButton.Enabled = true;
                ViewInactiveCurrentInjectionsButton.Enabled = true;
            }
            if (m_observedBussesHaveBeenResolved)
            {
                ViewModeledVoltagesButton.Enabled = true;
                ViewObservableNodesButton.Enabled = true;
                ViewSubstationAdjacencyList.Enabled = true;
                ViewAMatrixButton.Enabled = true;
                ViewIIMatrixButton.Enabled = true;
                ViewYMatrixButton.Enabled = true;
                ViewYsMatrixButton.Enabled = true;
                ViewYshMatrixButton.Enabled = true;
                ComputeSystemStateButton.Enabled = true;
            }
            if (m_stateWasComputed)
            {
                ComputeLineFlowsButton.Enabled = true;
                ComputeEstimatedInjectionsButton.Enabled = true;
                ComputePowerFlowsButton.Enabled = true;
                ViewOutputMeasurementsButton.Enabled = true;
                if (m_network.PhaseConfiguration == PhaseSelection.ThreePhase)
                {
                    ComputeSequenceComponentsButton.Enabled = true;
                }
            }
        }

        #endregion

        #region [ Disable Controls ]

        private void DisableControls()
        {
            DisableGeneralControls();
            DisableObservabilityAnalysisAndTopologyProcessingControls();
            DisableMatrixControls();
            DisableComputationControls();
            DisableViewParametersControls();
        }

        private void DisableGeneralControls()
        {
            InitializeNetworkModelButton.Enabled = false;
            SerializeButton.Enabled = false;
            MapMeasurementsButton.Enabled = false;
            ViewReceivedMeasurementsButton.Enabled = false;
            ViewUnreceivedMeasurementsButton.Enabled = false;
            CheckComponentsButton.Enabled = false;
        }

        private void DisableObservabilityAnalysisAndTopologyProcessingControls()
        {
            DetermineActiveCurrentPhasorsButton.Enabled = false;
            DetermineActiveCurrentInjectionsButton.Enabled = false;
            ResolveToObservedBussesButton.Enabled = false;
            ResolveToSingleFlowBranchesButton.Enabled = false;
            ViewActiveCurrentFlowsButton.Enabled = false;
            ViewModeledVoltagesButton.Enabled = false;
            ViewObservableNodesButton.Enabled = false;
            ViewSubstationAdjacencyList.Enabled = false;
            ViewCalculatedImpedancesButton.Enabled = false;
        }

        private void DisableMatrixControls()
        {
            ViewAMatrixButton.Enabled = false;
            ViewIIMatrixButton.Enabled = false;
            ViewYMatrixButton.Enabled = false;
            ViewYsMatrixButton.Enabled = false;
            ViewYshMatrixButton.Enabled = false;
        }

        private void DisableComputationControls()
        {
            ComputeSystemStateButton.Enabled = false;
            ComputeLineFlowsButton.Enabled = false;
            ComputeEstimatedInjectionsButton.Enabled = false;
            ComputePowerFlowsButton.Enabled = false;
            ComputeSequenceComponentsButton.Enabled = false;
        }

        private void DisableViewParametersControls()
        {
            ViewOutputMeasurementsButton.Enabled = false;
            ViewExpectedVoltagesButton.Enabled = false;
            ViewActiveVoltagesButton.Enabled = false;
            ViewInactiveVoltagesButton.Enabled = false;

            ViewModeledCurrentFlowsButton.Enabled = false;
            ViewExpectedCurrentFlowsButton.Enabled = false;
            ViewActiveCurrentFlowsButton.Enabled = false;
            ViewInactiveCurrentFlowsButton.Enabled = false;
            ViewIncludedCurrentFlowsButton.Enabled = false;
            ViewExcludedCurrentFlowsButton.Enabled = false;

            ViewModeledCurrentInjectionsButton.Enabled = false;
            ViewExpectedCurrentInjectionsButton.Enabled = false;
            ViewActiveCurrentInjectionsButton.Enabled = false;
            ViewInactiveCurrentInjectionsButton.Enabled = false;
        }
        
        #endregion

        #region [ Data Input ]

        private void BrowseRawMeasurementsButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "Measurement Snapshot Files (.xml)|*.xml";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                RawMeasurementsTextBox.Text = openFileDialog.FileName;
                StatusLabel.Text = "Located raw measurements file.";
            }

        }

        private void UploadRawMeasurementsButton_Click(object sender, EventArgs e)
        {
            try
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(RawMeasurements));
                StreamReader reader = new StreamReader(RawMeasurementsTextBox.Text);
                m_rawMeasurements = (RawMeasurements)deserializer.Deserialize(reader);
                reader.Close();
                StatusLabel.Text = "Raw measurements loaded successfully.";
                RawMeasurementsTextBox.Enabled = false;
                BrowseRawMeasurementsButton.Enabled = false;
                EnableControls();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString(), "Error!");
            }
        }

        private void BrowseNetworkModelButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "Model Configuration Files (.xml)|*.xml";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                NetworkModelTextBox.Text = openFileDialog.FileName;
                StatusLabel.Text = "Located model configuration file.";
            }
        }

        private void UploadNetworkModelButton_Click(object sender, EventArgs e)
        {
            try
            {
                m_network = Network.DeserializeFromXml(NetworkModelTextBox.Text);
                m_network.SerializeData(true);
                StatusLabel.Text = "Network model loaded successfully.";
                NetworkModelTextBox.Enabled = false;
                BrowseNetworkModelButton.Enabled = false;
                EnableControls();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString(), "Error!");
            }
        }

        #endregion

        #region [ General ]

        private void ClearTestCaseButton_Click(object sender, EventArgs e)
        {
            // GOT IT
            DisableControls();

            // Clear Raw Measurements 
            m_rawMeasurements = new RawMeasurements();
            RawMeasurementsTextBox.Enabled = true;
            RawMeasurementsTextBox.Text = "";
            BrowseRawMeasurementsButton.Enabled = true;

            // Clear Network Model
            m_network = null;
            m_networkIsInitialized = false;
            NetworkModelTextBox.Enabled = true;
            NetworkModelTextBox.Text = "";
            BrowseNetworkModelButton.Enabled = true;

            m_measurementsAreMapped = false;
            m_activeCurrentFlowsHaveBeenDetermined = false;
            m_observedBussesHaveBeenResolved = false;
            m_stateWasComputed = false;

            StatusLabel.Text = "Test case cleared.";

        }

        private void InitializeNetworkModelButton_Click(object sender, EventArgs e)
        {
            // GOT IT
            try
            {
                m_network.Model.Initialize();
                m_network.Model.AcceptsMeasurements = true;
                m_networkIsInitialized = true;
                StatusLabel.Text = "Network model initialized successfully.";
                EnableControls();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString(), "Error!");
            }
        }

        private void SerializeButton_Click(object sender, EventArgs e)
        {
            // GOT IT
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                m_network.SerializeData(true);
                m_network.SerializeToXml(saveFileDialog.FileName);
                m_network.SerializeData(false);
                StatusLabel.Text = "Serialized snapshot to file: " + saveFileDialog.FileName;
            }
        }

        private void MapMeasurementsButton_Click(object sender, EventArgs e)
        {
            // GOT IT
            try
            {
                foreach (RawMeasurementsMeasurement measurement in m_rawMeasurements.Items)
                {
                    m_network.Model.InputKeyValuePairs.Add(measurement.Key, Convert.ToDouble(measurement.Value));
                }

                m_network.Model.OnNewMeasurements();

                m_measurementsAreMapped = true;

                StatusLabel.Text = "Measurements succcessfully mapped to network model.";
                EnableControls();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString(), "Error!");
            }
        }

        private void CheckComponentsButton_Click(object sender, EventArgs e)
        {
            // GOT IT
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(m_network.Model.ComponentList());
            File.WriteAllText(Directory.GetCurrentDirectory() + "/ComponentList.txt", stringBuilder.ToString());
            Process.Start(Directory.GetCurrentDirectory() + "/ComponentList.txt", stringBuilder.ToString());
        }

        private void ViewReceivedMeasurementsButton_Click(object sender, EventArgs e)
        {
            
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Received Measurements Key Value Pairs");
            Dictionary<string, double> receivedMeasurements = m_network.Model.GetReceivedMeasurements();
            foreach (KeyValuePair<string, double> keyValuePair in receivedMeasurements)
            {
                stringBuilder.AppendLine(keyValuePair.Key + ", " + keyValuePair.Value.ToString());
            }
            File.WriteAllText(Directory.GetCurrentDirectory() + "/ReceivedMeasurements.txt", stringBuilder.ToString());
            StatusLabel.Text = "Wrote to " + Directory.GetCurrentDirectory() + "/ReceivedMeasurements.txt";
            Process.Start(Directory.GetCurrentDirectory() + "/ReceivedMeasurements.txt");
        }

        private void ViewUnreceivedMeasurementsButton_Click(object sender, EventArgs e)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Unreceived Measurements Key Value Pairs");
            Dictionary<string, double> receivedMeasurements = m_network.Model.GetReceivedMeasurements();
            foreach (KeyValuePair<string, double> keyValuePair in receivedMeasurements)
            {
                double value = 0;
                if (!m_network.Model.InputKeyValuePairs.TryGetValue(keyValuePair.Key, out value))
                {
                    stringBuilder.AppendLine(keyValuePair.Key + ", " + keyValuePair.Value.ToString());
                }
            }
            File.WriteAllText(Directory.GetCurrentDirectory() + "/UnreceivedMeasurements.txt", stringBuilder.ToString());
            StatusLabel.Text = "Wrote to " + Directory.GetCurrentDirectory() + "/UnreceivedMeasurements.txt";
            Process.Start(Directory.GetCurrentDirectory() + "/UnreceivedMeasurements.txt");
        }

        #endregion

        #region [ Observability Analysis ]

        private void DetermineActiveCurrentFlowPhasorsButton_Click(object sender, EventArgs e)
        {
            try
            {
                m_network.Model.DetermineActiveCurrentFlows();
                m_activeCurrentFlowsHaveBeenDetermined = true;
                StatusLabel.Text = "Active current flow phasors determined successfully.";
                EnableControls();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString(), "Error!");
            }
        }

        private void DetermineActiveCurrentInjectionsButton_Click(object sender, EventArgs e)
        {
            try
            {
                m_network.Model.DetermineActiveCurrentInjections();
                m_activeCurrentInjectionsHaveBeenDetermined = true;
                StatusLabel.Text = "Active current injection phasors determined successfully.";
                EnableControls();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString(), "Error!");
            }
        }

        private void ResolveToObservedBussesButton_Click(object sender, EventArgs e)
        {
            try
            {
                m_network.Model.ResolveToObservedBuses();
                m_observedBussesHaveBeenResolved = true;
                StatusLabel.Text = "Observability analysis resolved nodes to observed nodes.";
                EnableControls();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString(), "Error!");
            }
        }

        private void ViewActiveCurrentPhasorsButton_Click(object sender, EventArgs e)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat(m_network.Model.ActiveCurrentFlows.Count.ToString() + " Active Current Flow Phasors");
            foreach (CurrentFlowPhasorGroup currentPhasorGroup in m_network.Model.ActiveCurrentFlows)
            {
                stringBuilder.AppendFormat(currentPhasorGroup.ToVerboseString() + "{0}", Environment.NewLine);
            }
            File.WriteAllText(Directory.GetCurrentDirectory() + "/ActiveCurrentFlows.txt", stringBuilder.ToString());
            StatusLabel.Text = "Wrote to " + Directory.GetCurrentDirectory() + "/ActiveCurrentFlows.txt";
            Process.Start(Directory.GetCurrentDirectory() + "/ActiveCurrentFlows.txt");
        }

        private void ViewSubstationAdjacencyList_Click(object sender, EventArgs e)
        {
            StringBuilder stringBuilder = new StringBuilder();

            foreach (Company company in m_network.Model.Companies)
            {
                foreach (Division division in company.Divisions)
                {
                    foreach (Substation substation in division.Substations)
                    {
                        stringBuilder.AppendFormat(substation.ToString() + "{0}", Environment.NewLine);
                        stringBuilder.AppendFormat(substation.Graph.AdjacencyList.ToString() + "{0}", Environment.NewLine);
                        stringBuilder.AppendLine();
                    }
                }
            }

            File.WriteAllText(Directory.GetCurrentDirectory() + "/SubstationAdjacencyLists.txt", stringBuilder.ToString());
            StatusLabel.Text = "Wrote to " + Directory.GetCurrentDirectory() + "/SubstationAdjacencyLists.txt";
            Process.Start(Directory.GetCurrentDirectory() + "/SubstationAdjacencyLists.txt");
        }

        private void ViewObservableNodesButton_Click(object sender, EventArgs e)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat(m_network.Model.ObservedBusses.Count.ToString() + " Observed Busses");
            foreach (ObservedBus observedBus in m_network.Model.ObservedBusses)
            {
                stringBuilder.AppendFormat(observedBus.ToVerboseString() + "{0}", Environment.NewLine);
            }

            File.WriteAllText(Directory.GetCurrentDirectory() + "/ObservedBusses.txt", stringBuilder.ToString());
            StatusLabel.Text = "Wrote to " + Directory.GetCurrentDirectory() + "/ObservedBusses.txt";
            Process.Start(Directory.GetCurrentDirectory() + "/ObservedBusses.txt");
        }

        private void ResolveToSingleFlowBranchesButton_Click(object sender, EventArgs e)
        {
            try
            {
                m_network.Model.ResolveToSingleFlowBranches();
                m_observedBussesHaveBeenResolved = true;
                StatusLabel.Text = "Observability analysis resolved transmission lines to single flow branches.";
                EnableControls();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString(), "Error!");
            }
        }

        private void ViewTransmissionLineAdjacencyListsButton_Click(object sender, EventArgs e)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (TransmissionLine transmissionLine in m_network.Model.TransmissionLines)
            {
                stringBuilder.AppendFormat(transmissionLine.ToVerboseString() + "{0}", Environment.NewLine);
                stringBuilder.AppendFormat("Directly Connected Adjacency List{0}", Environment.NewLine);
                stringBuilder.AppendFormat(transmissionLine.Graph.DirectlyConnectedAdjacencyList.ToString() + "{0}", Environment.NewLine);
                stringBuilder.AppendFormat("Series Impedance Connected Adjacency List{0}", Environment.NewLine);
                stringBuilder.AppendFormat(transmissionLine.Graph.SeriesImpedanceConnectedAdjacencyList.ToString() + "{0}", Environment.NewLine);
                stringBuilder.AppendFormat("Transmission Line Tree{0}", Environment.NewLine);
                stringBuilder.AppendFormat(transmissionLine.Graph.RootNode.ToSubtreeString() + "{0}", Environment.NewLine);
                stringBuilder.AppendFormat("Single Flow Branch Resolution{0}", Environment.NewLine);
                stringBuilder.AppendFormat("Number of Possible Impedance States: " + transmissionLine.NumberOfPossibleSeriesImpedanceStates.ToString() + "{0}", Environment.NewLine);
                stringBuilder.AppendFormat("Possible Impedance States:{0}", Environment.NewLine);
                
                foreach (Impedance impedance in transmissionLine.PossibleImpedanceValues)
                {
                    stringBuilder.AppendFormat(impedance.ToVerboseString() + "{0}", Environment.NewLine);
                }
                stringBuilder.AppendFormat(transmissionLine.Graph.ResolveToSingleSeriesBranch().RawImpedanceParameters.ToVerboseString() + "{0}", Environment.NewLine);
            }
            File.WriteAllText(Directory.GetCurrentDirectory() + "/TransmissionLineAdjacencyLists.txt", stringBuilder.ToString());
            StatusLabel.Text = "Wrote to " + Directory.GetCurrentDirectory() + "/TransmissionLineAdjacencyLists.txt";
            Process.Start(Directory.GetCurrentDirectory() + "/TransmissionLineAdjacencyLists.txt");
        }

        private void ViewCalculatedImpedancesButton_Click(object sender, EventArgs e)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (TransmissionLine transmissionLine in m_network.Model.TransmissionLines)
            {
                stringBuilder.AppendFormat(transmissionLine.ToVerboseString() + "{0}", Environment.NewLine);
                stringBuilder.AppendLine("Modeled Impedance Values");
                stringBuilder.AppendFormat(transmissionLine.Graph.ResolveToSingleSeriesBranch().RawImpedanceParameters.ToVerboseString() + "{0}", Environment.NewLine);
                stringBuilder.AppendLine("Calculated Impedance Values");
                stringBuilder.AppendFormat(transmissionLine.RealTimeCalculatedImpedance.ToVerboseString() + "{0}", Environment.NewLine);
            }
            File.WriteAllText(Directory.GetCurrentDirectory() + "/CalculatedImpedances.txt", stringBuilder.ToString());
            StatusLabel.Text = "Wrote to " + Directory.GetCurrentDirectory() + "/CalculatedImpedances.txt";
            Process.Start(Directory.GetCurrentDirectory() + "/CalculatedImpedances.txt");
        }

        private void ViewSeriesCompensatorInferenceData_Click(object sender, EventArgs e)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (TransmissionLine transmissionLine in m_network.Model.TransmissionLines)
            {
                if (transmissionLine.WillPerformSeriesCompensatorStatusInference)
                {
                    stringBuilder.AppendFormat(transmissionLine.ToVerboseString() + "{0}", Environment.NewLine);
                    stringBuilder.AppendFormat("Directly Connected Adjacency List{0}", Environment.NewLine);
                    stringBuilder.AppendFormat(transmissionLine.Graph.DirectlyConnectedAdjacencyList.ToString() + "{0}", Environment.NewLine);
                    stringBuilder.AppendFormat("Series Impedance Connected Adjacency List{0}", Environment.NewLine);
                    stringBuilder.AppendFormat(transmissionLine.Graph.SeriesImpedanceConnectedAdjacencyList.ToString() + "{0}", Environment.NewLine);
                    stringBuilder.AppendFormat("Transmission Line Tree{0}", Environment.NewLine);
                    stringBuilder.AppendFormat(transmissionLine.Graph.RootNode.ToSubtreeString() + "{0}", Environment.NewLine);
                    stringBuilder.AppendFormat("Number of Possible Impedance States: " + transmissionLine.NumberOfPossibleSeriesImpedanceStates.ToString() + "{0}", Environment.NewLine);
                    stringBuilder.AppendFormat("Possible Impedance States:{0}", Environment.NewLine);
                    foreach (Impedance impedance in transmissionLine.PossibleImpedanceValues)
                    {
                        stringBuilder.AppendFormat(impedance.ToVerboseString() + "{0}", Environment.NewLine);
                    }
                    stringBuilder.AppendFormat("Inferred Total Impedance:{0}", Environment.NewLine);
                    stringBuilder.AppendFormat(transmissionLine.InferredTotalImpedance.ToVerboseString());
                    stringBuilder.AppendFormat("Single Flow Branch Resolution:{0}", Environment.NewLine);
                    stringBuilder.AppendFormat(transmissionLine.Graph.ResolveToSingleSeriesBranch().RawImpedanceParameters.ToVerboseString() + "{0}", Environment.NewLine);
                    stringBuilder.AppendFormat("Asserted Total Branch Impedance:{0}", Environment.NewLine);
                    stringBuilder.AppendFormat(transmissionLine.FromSideImpedanceToDeepestObservability.ToVerboseString() + "{0}", Environment.NewLine);
                }

            }
            File.WriteAllText(Directory.GetCurrentDirectory() + "/SeriesCompensatorStatusInferencingData.txt", stringBuilder.ToString());
            StatusLabel.Text = "Wrote to " + Directory.GetCurrentDirectory() + "/SeriesCompensatorStatusInferencingData.txt";
            Process.Start(Directory.GetCurrentDirectory() + "/SeriesCompensatorStatusInferencingData.txt");
        }

        private void ViewSeriesCompensatorsButton_Click(object sender, EventArgs e)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (TransmissionLine transmissionLine in m_network.Model.TransmissionLines)
            {
                if (transmissionLine.WillPerformSeriesCompensatorStatusInference)
                {
                    foreach (SeriesCompensator seriesCompensator in transmissionLine.SeriesCompensators)
                    {
                        stringBuilder.AppendFormat(seriesCompensator.ToVerboseString() + "{0}", Environment.NewLine);
                    }
                }
            }
            File.WriteAllText(Directory.GetCurrentDirectory() + "/SeriesCompensators.txt", stringBuilder.ToString());
            StatusLabel.Text = "Wrote to " + Directory.GetCurrentDirectory() + "/SeriesCompensators.txt";
            Process.Start(Directory.GetCurrentDirectory() + "/SeriesCompensators.txt");
        }

        private void ViewTransformersButton_Click(object sender, EventArgs e)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (Transformer transformer in m_network.Model.Transformers)
            {
                stringBuilder.AppendFormat(transformer.ToVerboseString() + "{0}", Environment.NewLine);
            }
            File.WriteAllText(Directory.GetCurrentDirectory() + "/Transformers.txt", stringBuilder.ToString());
            StatusLabel.Text = "Wrote to " + Directory.GetCurrentDirectory() + "/Transformers.txt";
            Process.Start(Directory.GetCurrentDirectory() + "/Transformers.txt");
        }

        #endregion

        #region [ Matrices ]

        private void ViewAMatrixButton_Click(object sender, EventArgs e)
        {
            try
            {
                CurrentFlowMeasurementBusIncidenceMatrix A = new CurrentFlowMeasurementBusIncidenceMatrix(m_network);

                StringBuilder stringBuilder = new StringBuilder();

                stringBuilder.AppendFormat(A.ToCsvString());

                File.WriteAllText(Directory.GetCurrentDirectory() + "/A Matrix.csv", stringBuilder.ToString());
                StatusLabel.Text = "Wrote to " + Directory.GetCurrentDirectory() + "/A Matrix.csv";
                Process.Start(Directory.GetCurrentDirectory() + "/A Matrix.csv");
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString(), "Error!");
            }
        }

        private void ViewIIMatrixButton_Click(object sender, EventArgs e)
        {
            try
            {
                VoltageMeasurementBusIncidenceMatrix II = new VoltageMeasurementBusIncidenceMatrix(m_network);

                StringBuilder stringBuilder = new StringBuilder();

                stringBuilder.AppendFormat(II.ToCsvString());

                File.WriteAllText(Directory.GetCurrentDirectory() + "/II Matrix.csv", stringBuilder.ToString());
                StatusLabel.Text = "Wrote to " + Directory.GetCurrentDirectory() + "/II Matrix.csv";
                Process.Start(Directory.GetCurrentDirectory() + "/II Matrix.csv");
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString(), "Error!");
            }
        }

        private void ViewYMatrixButton_Click(object sender, EventArgs e)
        {
            try
            {
                SeriesAdmittanceMatrix Y = new SeriesAdmittanceMatrix(m_network);

                StringBuilder stringBuilder = new StringBuilder();

                stringBuilder.AppendFormat(Y.ToCsvString());

                File.WriteAllText(Directory.GetCurrentDirectory() + "/Y Matrix.csv", stringBuilder.ToString());
                StatusLabel.Text = "Wrote to " + Directory.GetCurrentDirectory() + "/Y Matrix.csv";
                Process.Start(Directory.GetCurrentDirectory() + "/Y Matrix.csv");
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString(), "Error!");
            }
        }

        private void ViewYsMatrixButton_Click(object sender, EventArgs e)
        {
            try
            {
                LineShuntSusceptanceMatrix Ys = new LineShuntSusceptanceMatrix(m_network);

                StringBuilder stringBuilder = new StringBuilder();

                stringBuilder.AppendFormat(Ys.ToCsvString());

                File.WriteAllText(Directory.GetCurrentDirectory() + "/Ys Matrix.csv", stringBuilder.ToString());
                StatusLabel.Text = "Wrote to " + Directory.GetCurrentDirectory() + "/Ys Matrix.csv";
                Process.Start(Directory.GetCurrentDirectory() + "/Ys Matrix.csv");
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString(), "Error!");
            }
        }

        private void ViewYshMatrixButton_Click(object sender, EventArgs e)
        {
            try
            {
                ShuntDeviceSusceptanceMatrix Ysh = new ShuntDeviceSusceptanceMatrix(m_network);

                StringBuilder stringBuilder = new StringBuilder();

                stringBuilder.AppendFormat(Ysh.ToCsvString());

                File.WriteAllText(Directory.GetCurrentDirectory() + "/Ysh Matrix.csv", stringBuilder.ToString());
                StatusLabel.Text = "Wrote to " + Directory.GetCurrentDirectory() + "/Ysh Matrix.csv";
                Process.Start(Directory.GetCurrentDirectory() + "/Ysh Matrix.csv");
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString(), "Error!");
            }
        }

        #endregion

        #region [ Computation ]

        private void ComputeSystemStateButton_Click(object sender, EventArgs e)
        {
            try
            {
                m_network.ComputeSystemState();
                m_stateWasComputed = true;
                StatusLabel.Text = "System state computed successfully.";
                EnableControls();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString(), "Error!");
            }
        }

        private void ComputeLineFlowsButton_Click(object sender, EventArgs e)
        {
            try
            {
                m_network.Model.ComputeEstimatedCurrentFlows();
                StatusLabel.Text = "Estimated line flows computed successfully.";
                EnableControls();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString(), "Error!");
            }
        }

        private void ComputeEstimatedInjectionsButton_Click(object sender, EventArgs e)
        {
            try
            {
                m_network.Model.ReturnsCurrentInjection = true;
                m_network.Model.ComputeEstimatedCurrentInjections();
                StatusLabel.Text = "Estimated injections computed successfully.";
                EnableControls();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString(), "Error!");
            }
        }

        private void ComputePowerFlowsButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This button has not yet been implemented", "Oh, snap!");
        }

        private void ComputeSequenceComponentsButton_Click(object sender, EventArgs e)
        {
            try
            {
                m_network.Model.ComputeSequenceValues();
                StatusLabel.Text = "Sequence values computed successfully.";
                EnableControls();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString(), "Error!");
            }
        }

        private void ViewOutputMeasurementsButton_Click(object sender, EventArgs e)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat(m_network.Model.OutputKeyValuePairs.Count.ToString() + " Output Measurements");

            foreach (KeyValuePair<string, double> outputMeasurement in m_network.Model.OutputKeyValuePairs)
            {
                stringBuilder.AppendFormat(outputMeasurement.Key + "\t" + outputMeasurement.Value.ToString() + "{0}", Environment.NewLine);
            }

            File.WriteAllText(Directory.GetCurrentDirectory() + "/OutputMeasurements.txt", stringBuilder.ToString());
            StatusLabel.Text = "Wrote to " + Directory.GetCurrentDirectory() + "/OutputMeasurements.txt";
            Process.Start(Directory.GetCurrentDirectory() + "/OutputMeasurements.txt");

        }

        #endregion

        #region [ View Voltage Phasors ]

        private void ViewModeledVoltagesButton_Click(object sender, EventArgs e)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat(m_network.Model.Voltages.Count.ToString() + " Modeled Voltage Phasors");
            foreach (VoltagePhasorGroup voltagePhasorGroup in m_network.Model.Voltages)
            {
                stringBuilder.AppendFormat(voltagePhasorGroup.ToVerboseString() + "{0}", Environment.NewLine);
            }

            File.WriteAllText(Directory.GetCurrentDirectory() + "/ModeledVoltages.txt", stringBuilder.ToString());
            StatusLabel.Text = "Wrote to " + Directory.GetCurrentDirectory() + "/ModeledVoltages.txt";
            Process.Start(Directory.GetCurrentDirectory() + "/ModeledVoltages.txt");
        }

        private void ViewExpectedVoltagesButton_Click(object sender, EventArgs e)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat(m_network.Model.ExpectedVoltages.Count.ToString() + " Expected Voltage Phasors");
            foreach (VoltagePhasorGroup voltagePhasorGroup in m_network.Model.ExpectedVoltages)
            {
                stringBuilder.AppendFormat(voltagePhasorGroup.ToVerboseString() + "{0}", Environment.NewLine);
            }

            File.WriteAllText(Directory.GetCurrentDirectory() + "/ExpectedVoltages.txt", stringBuilder.ToString());
            StatusLabel.Text = "Wrote to " + Directory.GetCurrentDirectory() + "/ExpectedVoltages.txt";
            Process.Start(Directory.GetCurrentDirectory() + "/ExpectedVoltages.txt");
        }

        private void ViewActiveVoltagesButton_Click(object sender, EventArgs e)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat(m_network.Model.ActiveVoltages.Count.ToString() + " Active Voltage Phasors");
            foreach (VoltagePhasorGroup voltagePhasorGroup in m_network.Model.ActiveVoltages)
            {
                stringBuilder.AppendFormat(voltagePhasorGroup.ToVerboseString() + "{0}", Environment.NewLine);
            }

            File.WriteAllText(Directory.GetCurrentDirectory() + "/ActiveVoltages.txt", stringBuilder.ToString());
            StatusLabel.Text = "Wrote to " + Directory.GetCurrentDirectory() + "/ActiveVoltages.txt";
            Process.Start(Directory.GetCurrentDirectory() + "/ActiveVoltages.txt");
        }

        private void ViewInactiveVoltagesButton_Click(object sender, EventArgs e)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("Inactive Voltage Phasors");
            foreach (VoltagePhasorGroup voltagePhasorGroup in m_network.Model.ExpectedVoltages)
            {
                if (!m_network.Model.ActiveVoltages.Contains(voltagePhasorGroup))
                {
                    stringBuilder.AppendFormat(voltagePhasorGroup.ToVerboseString() + "{0}", Environment.NewLine);
                }
            }
            File.WriteAllText(Directory.GetCurrentDirectory() + "/InactiveVoltages.txt", stringBuilder.ToString());
            StatusLabel.Text = "Wrote to " + Directory.GetCurrentDirectory() + "/InactiveVoltages.txt";
            Process.Start(Directory.GetCurrentDirectory() + "/InactiveVoltages.txt");
        }

        #endregion

        #region [ View Current Flow Phasors ]

        private void ViewModeledCurrentFlowsButton_Click(object sender, EventArgs e)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat(m_network.Model.CurrentFlows.Count.ToString() + " Modeled Current Flow Phasors");
            foreach (CurrentFlowPhasorGroup currentFlowPhasorGroup in m_network.Model.CurrentFlows)
            {
                stringBuilder.AppendFormat(currentFlowPhasorGroup.ToVerboseString() + "{0}", Environment.NewLine);
            }
            File.WriteAllText(Directory.GetCurrentDirectory() + "/ModeledCurrentFlows.txt", stringBuilder.ToString());
            StatusLabel.Text = "Wrote to " + Directory.GetCurrentDirectory() + "/ModeledCurrentFlows.txt";
            Process.Start(Directory.GetCurrentDirectory() + "/ModeledCurrentFlows.txt");
        }

        private void ViewExpectedCurrentFlowsButton_Click(object sender, EventArgs e)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat(m_network.Model.ExpectedCurrentFlows.Count.ToString() + " Expected Current Flow Phasors");
            foreach (CurrentFlowPhasorGroup currentFlowPhasorGroup in m_network.Model.ExpectedCurrentFlows)
            {
                stringBuilder.AppendFormat(currentFlowPhasorGroup.ToVerboseString() + "{0}", Environment.NewLine);
            }
            File.WriteAllText(Directory.GetCurrentDirectory() + "/ExpectedCurrentFlows.txt", stringBuilder.ToString());
            StatusLabel.Text = "Wrote to " + Directory.GetCurrentDirectory() + "/ExpectedCurrentFlows.txt";
            Process.Start(Directory.GetCurrentDirectory() + "/ExpectedCurrentFlows.txt");
        }

        private void ViewInactiveCurrentFlowsButton_Click(object sender, EventArgs e)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("Inactive Current Flow Phasors");
            foreach (CurrentFlowPhasorGroup currentFlowPhasorGroup in m_network.Model.ExpectedCurrentFlows)
            {
                if (!m_network.Model.ActiveCurrentFlows.Contains(currentFlowPhasorGroup))
                {
                    stringBuilder.AppendFormat(currentFlowPhasorGroup.ToVerboseString() + "{0}", Environment.NewLine);
                }
            }
            File.WriteAllText(Directory.GetCurrentDirectory() + "/InactiveCurrentFlows.txt", stringBuilder.ToString());
            StatusLabel.Text = "Wrote to " + Directory.GetCurrentDirectory() + "/InactiveCurrentFlows.txt";
            Process.Start(Directory.GetCurrentDirectory() + "/InactiveCurrentFlows.txt");
        }

        private void ViewIncludedCurrentFlowPhasorsButton_Click(object sender, EventArgs e)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat(m_network.Model.IncludedCurrentFlows.Count.ToString() + " Included Current Flow Phasors");
            foreach (CurrentFlowPhasorGroup currentPhasorGroup in m_network.Model.IncludedCurrentFlows)
            {
                stringBuilder.AppendFormat(currentPhasorGroup.ToVerboseString() + "{0}", Environment.NewLine);
            }

            File.WriteAllText(Directory.GetCurrentDirectory() + "/IncludedCurrentFlows.txt", stringBuilder.ToString());
            StatusLabel.Text = "Wrote to " + Directory.GetCurrentDirectory() + "/IncludedCurrentFlows.txt";
            Process.Start(Directory.GetCurrentDirectory() + "/IncludedCurrentFlows.txt");
        }

        private void ViewExcludedCurrentFlowsButton_Click(object sender, EventArgs e)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("Excluded Current Flow Phasors");
            foreach (CurrentFlowPhasorGroup currentFlowPhasorGroup in m_network.Model.IncludedCurrentFlows)
            {
                if (!m_network.Model.ActiveCurrentFlows.Contains(currentFlowPhasorGroup))
                {
                    stringBuilder.AppendFormat(currentFlowPhasorGroup.ToVerboseString() + "{0}", Environment.NewLine);
                }
            }
            File.WriteAllText(Directory.GetCurrentDirectory() + "/ExcludedCurrentFlows.txt", stringBuilder.ToString());
            StatusLabel.Text = "Wrote to " + Directory.GetCurrentDirectory() + "/ExcludedCurrentFlows.txt";
            Process.Start(Directory.GetCurrentDirectory() + "/ExcludedCurrentFlows.txt");
        }

        #endregion

        #region [ View Current Injection Phasors ]

        private void ViewModeledCurrentInjectionsButton_Click(object sender, EventArgs e)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat(m_network.Model.CurrentInjections.Count.ToString() + " Modeled Current Injections Phasors");
            foreach (CurrentInjectionPhasorGroup currentInjectionPhasorGroup in m_network.Model.CurrentInjections)
            {
                stringBuilder.AppendFormat(currentInjectionPhasorGroup.ToVerboseString() + "{0}", Environment.NewLine);
            }
            File.WriteAllText(Directory.GetCurrentDirectory() + "/ModeledCurrentInjections.txt", stringBuilder.ToString());
            Process.Start(Directory.GetCurrentDirectory() + "/ModeledCurrentInjections.txt");
        }

        private void ViewExpectedCurrentInjectionsButton_Click(object sender, EventArgs e)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat(m_network.Model.ExpectedCurrentInjections.Count.ToString() + " Expected Current Injection Phasors");
            foreach (CurrentInjectionPhasorGroup currentInjectionPhasorGroup in m_network.Model.ExpectedCurrentInjections)
            {
                stringBuilder.AppendFormat(currentInjectionPhasorGroup.ToVerboseString() + "{0}", Environment.NewLine);
            }
            File.WriteAllText(Directory.GetCurrentDirectory() + "/ExpectedCurrentInjections.txt", stringBuilder.ToString());
            Process.Start(Directory.GetCurrentDirectory() + "/ExpectedCurrentInjections.txt");
        }

        private void ViewActiveCurrentInjectionsButton_Click(object sender, EventArgs e)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat(m_network.Model.ActiveCurrentInjections.Count.ToString() + " Active Current Injection Phasors");
            foreach (CurrentInjectionPhasorGroup currentInjectionPhasorGroup in m_network.Model.ActiveCurrentInjections)
            {
                stringBuilder.AppendFormat(currentInjectionPhasorGroup.ToVerboseString() + "{0}", Environment.NewLine);
            }
            File.WriteAllText(Directory.GetCurrentDirectory() + "/ActiveCurrentInjections.txt", stringBuilder.ToString());
            Process.Start(Directory.GetCurrentDirectory() + "/ActiveCurrentInjections.txt");
        }

        private void ViewInactiveCurrentInjectionsButton_Click(object sender, EventArgs e)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("Inactive Current Injection Phasors");
            foreach (CurrentInjectionPhasorGroup currentInjectionPhasorGroup in m_network.Model.ExpectedCurrentInjections)
            {
                if (!m_network.Model.ActiveCurrentInjections.Contains(currentInjectionPhasorGroup))
                {
                    stringBuilder.AppendFormat(currentInjectionPhasorGroup.ToVerboseString() + "{0}", Environment.NewLine);
                }
            }
            File.WriteAllText(Directory.GetCurrentDirectory() + "/InactiveCurrentInjections.txt", stringBuilder.ToString());
            Process.Start(Directory.GetCurrentDirectory() + "/InactiveCurrentInjections.txt");
        }

        #endregion

        #endregion
    }
}
