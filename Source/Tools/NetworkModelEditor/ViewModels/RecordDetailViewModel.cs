//******************************************************************************************************
//  RecordDetailViewModel.cs
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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkModelEditor;

namespace NetworkModelEditor.ViewModels
{
    public class RecordDetailViewModel : ViewModelBase
    {
        #region [ Private Methods ]

        private ObservableCollection<ViewModelBase> m_detailViewModelCollection;
        private MainWindowViewModel m_mainWindow;

        #endregion

        #region [ Public Properties ]

        public ObservableCollection<ViewModelBase> DetailViewModelCollection
        {
            get
            {
                return m_detailViewModelCollection;
            }
            set
            {
                m_detailViewModelCollection = value;
            }
        }

        #endregion

        #region [ Constructor ]

        public RecordDetailViewModel(MainWindowViewModel mainWindow)
        {
            m_mainWindow = mainWindow;
            m_detailViewModelCollection = new ObservableCollection<ViewModelBase>();
        }

        #endregion

        #region [ Public Methods ]

        public void AddViewModel(NetworkElement networkElement)
        {
            m_detailViewModelCollection.Add(RecordDetailHelper.CreateAppropriateRecordViewModel(networkElement));
        }

        public void AddViewModel(ViewModelBase viewModel)
        {
            m_detailViewModelCollection.Add(viewModel);
        }

        public void ClearRecordDetailView()
        {
            m_detailViewModelCollection.Clear();
        }

        #endregion
    }
}
