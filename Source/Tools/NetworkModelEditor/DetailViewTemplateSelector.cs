//******************************************************************************************************
//  DetailViewTemplateSelector.cs
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
using System.Windows;
using System.Windows.Controls;
using NetworkModelEditor.ViewModels;

namespace NetworkModelEditor
{
    public class DetailViewTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;

            if (element != null && item != null && item is ViewModelBase)
            {
                ViewModelBase viewModel = item as ViewModelBase;
                if (item is NetworkDetailViewModel)
                {
                    return element.FindResource("NetworkDetailTemplate") as DataTemplate;
                }
                else if (item is NetworkModelDetailViewModel)
                {
                    return element.FindResource("ModelDetailTemplate") as DataTemplate;
                }
                else if (item is CompanyDetailViewModel)
                {
                    return element.FindResource("CompanyDetailTemplate") as DataTemplate;
                }
                else if (item is DivisionDetailViewModel)
                {
                    return element.FindResource("DivisionDetailTemplate") as DataTemplate;
                }
                else if (item is SubstationDetailViewModel)
                {
                    return element.FindResource("SubstationDetailTemplate") as DataTemplate;
                }
                else if (item is TransmissionLineDetailViewModel)
                {
                    return element.FindResource("TransmissionLineDetailTemplate") as DataTemplate;
                }
                else if (item is NodeDetailViewModel)
                {
                    return element.FindResource("NodeDetailTemplate") as DataTemplate;
                }
                else if (item is TransformerDetailViewModel)
                {
                    return element.FindResource("TransformerDetailTemplate") as DataTemplate;
                }
                else if (item is ShuntDetailViewModel)
                {
                    return element.FindResource("ShuntDetailTemplate") as DataTemplate;
                }
                else if (item is VoltageLevelDetailViewModel)
                {
                    return element.FindResource("VoltageLevelDetailTemplate") as DataTemplate;
                }
                else if (item is StatusWordDetailViewModel)
                {
                    return element.FindResource("StatusWordDetailTemplate") as DataTemplate;
                }
                else if (item is VoltagePhasorGroupDetailViewModel)
                {
                    return element.FindResource("VoltagePhasorGroupDetailTemplate") as DataTemplate;
                }
                else if (item is CurrentPhasorGroupDetailViewModel)
                {
                    return element.FindResource("CurrentPhasorGroupDetailTemplate") as DataTemplate;
                }
                else if (item is CurrentInjectionPhasorGroupDetailViewModel)
                {
                    return element.FindResource("CurrentInjectionPhasorGroupDetailTemplate") as DataTemplate;
                }
                else if (item is CircuitBreakerDetailViewModel)
                {
                    return element.FindResource("CircuitBreakerDetailTemplate") as DataTemplate;
                }
                else if (item is SwitchDetailViewModel)
                {
                    return element.FindResource("SwitchDetailTemplate") as DataTemplate;
                }
                else if (item is LineSegmentDetailViewModel)
                {
                    return element.FindResource("LineSegmentDetailTemplate") as DataTemplate;
                }
                else if (item is SeriesCompensatorDetailViewModel)
                {
                    return element.FindResource("SeriesCompensatorDetailTemplate") as DataTemplate;
                }
                else if (item is BreakerStatusDetailViewModel)
                {
                    return element.FindResource("BreakerStatusDetailTemplate") as DataTemplate;
                }
                else if (item is TapConfigurationDetailViewModel)
                {
                    return element.FindResource("TapConfigurationDetailTemplate") as DataTemplate;
                }
                else if (item is MeasurementSampleDetailViewModel)
                {
                    return element.FindResource("MeasurementSampleDetailTemplate") as DataTemplate;
                }
            }

            return null;
        }
    }
}
