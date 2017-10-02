//******************************************************************************************************
//  VoltageLevelDetailViewModel.cs
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
using SynchrophasorAnalytics.Modeling;

namespace NetworkModelEditor.ViewModels
{
    public class VoltageLevelDetailViewModel : ViewModelBase
    {
        #region [ Private Members ]

        private VoltageLevel m_voltageLevel;

        #endregion

        #region [ Properties ]

        public int InternalID
        {
            get
            {
                return m_voltageLevel.InternalID;
            }
            set
            {
                m_voltageLevel.InternalID = value;
            }
        }

        public double BaseKV
        {
            get
            {
                return m_voltageLevel.Value;
            }
            set
            {
                m_voltageLevel.Value = value;
            }
        }

        #endregion

        #region [ Constructors ]

        public VoltageLevelDetailViewModel()
        {
        }

        public VoltageLevelDetailViewModel(object voltageLevel)
        {
            if (voltageLevel != null && voltageLevel is VoltageLevel)
            {
                m_voltageLevel = voltageLevel as VoltageLevel;
            }
        }

        #endregion


    }
}
