//******************************************************************************************************
//  StatusWordDetailViewModel.cs
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SynchrophasorAnalytics.Measurements;

namespace NetworkModelEditor.ViewModels
{
    public class StatusWordDetailViewModel : ViewModelBase
    {
        #region [ Private Members ]

        private StatusWord m_statusWord;

        #endregion

        #region [ Properties ]

        public int InternalID
        {
            get
            {
                return m_statusWord.InternalID;
            }
            set
            {
                m_statusWord.InternalID = value;
            }
        }

        public int Number
        {
            get
            {
                return m_statusWord.Number;
            }
            set
            {
                m_statusWord.Number = value;
            }
        }

        public string Description
        {
            get
            {
                return m_statusWord.Description;
            }
            set
            {
                m_statusWord.Description = value;
            }
        }

        public string Key
        {
            get
            {
                return m_statusWord.Key;
            }
            set
            {
                m_statusWord.Key = value;
            }
        }

        public bool IsEnabled
        {
            get
            {
                return m_statusWord.IsEnabled;
            }
            set
            {
                m_statusWord.IsEnabled = value;
            }
        }

        #endregion

        #region [ Constructors ]

        public StatusWordDetailViewModel()
        {
        }

        public StatusWordDetailViewModel(object statusWord)
        {
            if (statusWord != null && statusWord is StatusWord)
            {
                m_statusWord = statusWord as StatusWord;
            }
        }

        #endregion
    }
}
