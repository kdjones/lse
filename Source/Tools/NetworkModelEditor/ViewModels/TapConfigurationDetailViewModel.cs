//******************************************************************************************************
//  TapConfigurationDetailViewModel.cs
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
    public class TapConfigurationDetailViewModel : ViewModelBase
    {
        #region [ Private Fields ]

        private TapConfiguration m_tapConfiguration;

        #endregion

        #region [ Public Properties ]

        public int InternalId
        {
            get
            {
                return m_tapConfiguration.InternalID;
            }
            set
            {
                m_tapConfiguration.InternalID = value;
            }
        }

        public string Acronym
        {
            get
            {
                return m_tapConfiguration.Acronym;
            }
            set
            {
                m_tapConfiguration.Acronym = value;
            }
        }

        public string Name
        {
            get
            {
                return m_tapConfiguration.Name;
            }
            set
            {
                m_tapConfiguration.Name = value;
            }
        }

        public string Description
        {
            get
            {
                return m_tapConfiguration.Description;
            }
            set
            {
                m_tapConfiguration.Description = value;
            }
        }

        public double LowerBounds
        {
            get
            {
                return m_tapConfiguration.LowerBounds;
            }
            set
            {
                m_tapConfiguration.LowerBounds = value;
            }
        }

        public double UpperBounds
        {
            get
            {
                return m_tapConfiguration.UpperBounds;
            }
            set
            {
                m_tapConfiguration.UpperBounds = value;
            }
        }

        public int PositionLowerBounds
        {
            get
            {
                return m_tapConfiguration.PositionLowerBounds;
            }
            set
            {
                m_tapConfiguration.PositionLowerBounds = value;
            }
        }

        public int PositionUpperBounds
        {
            get
            {
                return m_tapConfiguration.PositionUpperBounds;
            }
            set
            {
                m_tapConfiguration.PositionUpperBounds = value;
            }
        }

        public int PositionNominal
        {
            get
            {
                return m_tapConfiguration.PositionNominal;
            }
            set
            {
                m_tapConfiguration.PositionNominal = value;
            }
        }

        #endregion

        #region [ Constructors ]

        public TapConfigurationDetailViewModel()
        {
        }

        public TapConfigurationDetailViewModel(object tapConfiguration)
        {
            if (tapConfiguration != null && tapConfiguration is TapConfiguration)
            {
                m_tapConfiguration = tapConfiguration as TapConfiguration;
            }
        }

        #endregion

    }
}
