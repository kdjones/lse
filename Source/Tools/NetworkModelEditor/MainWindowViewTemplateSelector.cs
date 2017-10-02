//******************************************************************************************************
//  MainWindowViewTemplateSelector.cs
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
    public class MainWindowViewTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;

            if (element != null && item != null && item is ViewModelBase)
            {
                if (item is NetworkTreeViewModel)
                {
                    return element.FindResource("NetworkTreeViewTemplate") as DataTemplate;
                }
                else if (item is RecordDetailViewModel)
                {
                    return element.FindResource("RecordDetailViewTemplate") as DataTemplate;
                }
                else
                {
                    return element.FindResource("RecordDetailViewTemplate") as DataTemplate;
                }
            }

            return null;
        }
    }
}
