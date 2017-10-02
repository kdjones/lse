//******************************************************************************************************
//  RecordDetailHelper.cs
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
using NetworkModelEditor.ViewModels;
using SynchrophasorAnalytics.Networks;
using SynchrophasorAnalytics.Modeling;
using SynchrophasorAnalytics.Measurements;
using SynchrophasorAnalytics.Testing;

namespace NetworkModelEditor
{
    public class RecordDetailHelper
    {
        public static ViewModelBase CreateAppropriateRecordViewModel(NetworkElement networkElement)
        {
            if (networkElement.Element is Network)
            {
                return new NetworkDetailViewModel(networkElement.Element);
            }
            else if (networkElement.Element is NetworkModel)
            {
                return new NetworkModelDetailViewModel(networkElement.Element);
            }
            else if (networkElement.Element is Company)
            {
                return new CompanyDetailViewModel(networkElement.Element);
            }
            else if (networkElement.Element is Division)
            {
                return new DivisionDetailViewModel(networkElement.Element);
            }
            else if (networkElement.Element is Substation)
            {
                return new SubstationDetailViewModel(networkElement.Element);
            }
            else if (networkElement.Element is TransmissionLine)
            {
                return new TransmissionLineDetailViewModel(networkElement.Element);
            }
            else if (networkElement.Element is Node)
            {
                return new NodeDetailViewModel(networkElement.Element);
            }
            else if (networkElement.Element is Transformer)
            {
                return new TransformerDetailViewModel(networkElement.Element);
            }
            else if (networkElement.Element is ShuntCompensator)
            {
                return new ShuntDetailViewModel(networkElement.Element);
            }
            else if (networkElement.Element is SeriesCompensator)
            {
                return new SeriesCompensatorDetailViewModel(networkElement.Element);
            }
            else if (networkElement.Element is LineSegment)
            {
                return new LineSegmentDetailViewModel(networkElement.Element);
            }
            else if (networkElement.Element is CircuitBreaker)
            {
                return new CircuitBreakerDetailViewModel(networkElement.Element);
            }
            else if (networkElement.Element is Switch)
            {
                return new SwitchDetailViewModel(networkElement.Element);
            }
            else if (networkElement.Element is VoltageLevel)
            {
                return new VoltageLevelDetailViewModel(networkElement.Element);
            }
            else if (networkElement.Element is StatusWord)
            {
                return new StatusWordDetailViewModel(networkElement.Element);
            }
            else if (networkElement.Element is BreakerStatus)
            {
                return new BreakerStatusDetailViewModel(networkElement.Element);
            }
            else if (networkElement.Element is TapConfiguration)
            {
                return new TapConfigurationDetailViewModel(networkElement.Element);
            }
            else if (networkElement.Element is VoltagePhasorGroup)
            {
                return new VoltagePhasorGroupDetailViewModel(networkElement.Element);
            }
            else if (networkElement.Element is CurrentFlowPhasorGroup)
            {
                return new CurrentPhasorGroupDetailViewModel(networkElement.Element);
            }
            else if (networkElement.Element is CurrentInjectionPhasorGroup)
            {
                return new CurrentInjectionPhasorGroupDetailViewModel(networkElement.Element);
            }
            else if (networkElement.Element is RawMeasurements)
            {
                return new MeasurementSampleDetailViewModel(networkElement.Element);
            }
            else
            {
                return null;
            }
        }
    }
}
