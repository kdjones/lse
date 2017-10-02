//******************************************************************************************************
//  BreakerStatusDetailViewModel.cs
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
    public class BreakerStatusDetailViewModel : ViewModelBase
    {
        #region [ Private Members ]

        private BreakerStatus m_breakerStatus;

        #endregion

        #region [ Properties ]

        public int InternalID
        {
            get
            {
                return m_breakerStatus.InternalID;
            }
            set
            {
                m_breakerStatus.InternalID = value;
            }
        }

        public int Number
        {
            get
            {
                return m_breakerStatus.Number;
            }
            set
            {
                m_breakerStatus.Number = value;
            }
        }

        public string Name
        {
            get
            {
                return m_breakerStatus.Name;
            }
            set
            {
                m_breakerStatus.Name = value;
            }
        }


        public string Description
        {
            get
            {
                return m_breakerStatus.Description;
            }
            set
            {
                m_breakerStatus.Description = value;
            }
        }

        public string Key
        {
            get
            {
                return m_breakerStatus.Key;
            }
            set
            {
                m_breakerStatus.Key = value;
            }
        }

        public BreakerStatusBit BitPosition
        {
            get
            {
                return m_breakerStatus.BitPosition;
            }
            set
            {
                m_breakerStatus.BitPosition = value;
            }
        }

        public bool IsEnabled
        {
            get
            {
                return m_breakerStatus.IsEnabled;
            }
            set
            {
                m_breakerStatus.IsEnabled = value;
            }
        }

        #endregion

        #region [ Constructors ]

        public BreakerStatusDetailViewModel()
        {
        }

        public BreakerStatusDetailViewModel(object breakerStatus)
        {
            if (breakerStatus != null && breakerStatus is BreakerStatus)
            {
                m_breakerStatus = breakerStatus as BreakerStatus;
            }
        }

        #endregion


    }
}
