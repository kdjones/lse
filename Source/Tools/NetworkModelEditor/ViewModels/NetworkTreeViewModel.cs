//******************************************************************************************************
//  NetworkTreeViewModel.cs
//
//  Copyright © 2014, Kevin D. Jones.  All Rights Reserved.
//
//  This file is licensed to you under the Eclipse Public License -v 1.0 (the "License"); you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://www.opensource.org/licenses/eclipse-1.0.php
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  02/01/2014 - Kevin D. Jones
//       Generated original version of source code.
//
//******************************************************************************************************
using System;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using NetworkModelEditor.Commands;
using SynchrophasorAnalytics.Networks;
using SynchrophasorAnalytics.Modeling;
using SynchrophasorAnalytics.Testing;


namespace NetworkModelEditor.ViewModels
{
    public class NetworkTreeViewModel : ViewModelBase
    {

        #region [ Private Members ]

        #region [ Data ] 

        private Network m_network;
        private List<RawMeasurements> m_measurementSamples;

        #endregion

        #region [ View Models ] 

        private MainWindowViewModel m_mainWindow;
        private NetworkElementViewModel m_networkModelRoot;
        private NetworkElementViewModel m_measurementSamplesRoot;
        private NetworkElementViewModel m_selectedElement;
        private ObservableCollection<NetworkElementViewModel> m_firstGeneration;
        private ObservableCollection<NetworkElementViewModel> m_secondGeneration;

        #endregion

        #region [ Commands ] 

        private RelayCommand m_viewDetailCommand;

        #endregion

        #endregion

        #region [ Properties ]

        #region [ Data ] 

        public Network Network
        {
            get
            {
                return m_network;
            }
            set
            {
                m_network = value;
                InitializeNetworkTree(value);
            }
        }

        public List<RawMeasurements> MeasurementSamples
        {
            get
            {
                return m_measurementSamples;
            }
            set
            {
                m_measurementSamples = value;
                InitializeMeasurementTree(value);
            }
        }

        #endregion

        #region [ View Models ] 

        public ObservableCollection<NetworkElementViewModel> FirstGeneration
        {
            get 
            { 
                return m_firstGeneration; 
            }
        }

        public ObservableCollection<NetworkElementViewModel> SecondGeneration
        {
            get
            {
                return m_secondGeneration;
            }
        }

        public NetworkElementViewModel SelectedElement
        {
            get
            {
                return m_selectedElement;
            }
            set
            {
                m_selectedElement = value;
            }
        }

        public ObservableCollection<MenuItem> ContextMenuItems
        {
            get
            {
                List<MenuItem> contextMenuItems = new List<MenuItem>();
                contextMenuItems.Add(new MenuItem() { Header = "Item 1" });
                contextMenuItems.Add(new MenuItem() { Header = "Item 2" });
                return new ObservableCollection<MenuItem>(contextMenuItems);
            }
        }

        public ViewModelBase MainWindow
        {
            get
            {
                return m_mainWindow;
            }
            set
            {
                if (value is MainWindowViewModel)
                {
                    m_mainWindow = value as MainWindowViewModel;
                }
            }
        }

        #endregion

        #region [ Commands ]

        public ICommand ViewDetailCommand
        {
            get
            {
                if (m_viewDetailCommand == null)
                {
                    m_viewDetailCommand = new RelayCommand(param => this.ViewDetail(), param => true);
                }
                return m_viewDetailCommand;
            }
        }



        #endregion

        #endregion

        #region [ Constructor ]

        public NetworkTreeViewModel(MainWindowViewModel mainWindow)
            :this(mainWindow, null, null)
        {
        }

        public NetworkTreeViewModel(MainWindowViewModel mainWindow, Network network, List<RawMeasurements> measurementSamples)
        {
            m_mainWindow = mainWindow;
            if (network != null)
            {
                InitializeNetworkTree(network);
            }
            else
            {
                m_network = new Network();
                InitializeNetworkTree(m_network);

            }
            if (measurementSamples != null)
            {
                InitializeMeasurementTree(measurementSamples);
            }
            else
            {
                m_measurementSamples = new List<RawMeasurements>();
                InitializeMeasurementTree(m_measurementSamples);
            }
        }

        #endregion

        #region [ Private Methods ] 

        private void InitializeNetworkTree(Network network)
        {
            network.Initialize();

            m_networkModelRoot = new NetworkElementViewModel(this, new NetworkElement(network));
            
            m_firstGeneration = new ObservableCollection<NetworkElementViewModel>(new NetworkElementViewModel[] { m_networkModelRoot });
            OnPropertyChanged("FirstGeneration");
        }

        private void InitializeMeasurementTree(List<RawMeasurements> measurementSamples)
        {
            m_measurementSamplesRoot = new NetworkElementViewModel(this, new NetworkElement(measurementSamples));

            m_secondGeneration = new ObservableCollection<NetworkElementViewModel>(new NetworkElementViewModel[] { m_measurementSamplesRoot });
            OnPropertyChanged("SecondGeneration");
        }
        
        #endregion

        internal void HandleRightMouseButtonClick()
        {
            if (SelectedElement != null)
            {
                MessageBox.Show(SelectedElement.Name);
            }
        }

        #region [ Commands ] 

        private void ViewDetail()
        {
            m_mainWindow.ViewDetailCommand.Execute(null);
        }

        #endregion

        #region [ Public Methods ] 

        public void RefreshNetworkTree()
        {
            InitializeNetworkTree(m_network);
            InitializeMeasurementTree(m_measurementSamples);
        }

        #endregion
    }
}
