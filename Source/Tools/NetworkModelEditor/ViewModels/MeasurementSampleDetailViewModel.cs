//******************************************************************************************************
//  DivisionDetailViewModel.cs
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
using SynchrophasorAnalytics.Modeling;
using SynchrophasorAnalytics.Testing;

namespace NetworkModelEditor.ViewModels
{
    public class MeasurementSampleDetailViewModel : ViewModelBase
    {
        #region [ Private Members ]

        private RawMeasurements m_measurementSample;

        #endregion

        #region [ Properties ]

        public List<RawMeasurementsMeasurement> Measurements
        {
            get
            {
                return m_measurementSample.Items.ToList();
            }
            set
            {
                m_measurementSample.Items = value.ToArray();
                OnPropertyChanged("Measurements");
            }
        }

        #endregion

        #region [ Constructors ]

        public MeasurementSampleDetailViewModel()
        {
        }

        public MeasurementSampleDetailViewModel(object measurementSample)
        {
            if (measurementSample != null && measurementSample is RawMeasurements)
            {
                m_measurementSample = measurementSample as RawMeasurements;
            }
        }

        #endregion
    }
}
