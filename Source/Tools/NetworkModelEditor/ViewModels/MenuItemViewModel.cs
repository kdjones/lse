//******************************************************************************************************
//  MenuItemViewModel.cs
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
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NetworkModelEditor.ViewModels
{
    public class MenuItemViewModel : ViewModelBase
    {
        private string m_text;
        private ICommand m_command;
        private bool m_isChecked;
        private bool m_isCheckable;
        private bool m_isEnabled = true;
        private List<MenuItemViewModel> m_children;

        public string Text
        {
            get
            {
                return m_text;
            }
        }

        public bool IsCheckable
        {
            get
            {
                return m_isCheckable;
            }
        }

        public bool IsEnabled
        {
            get
            {
                return m_isEnabled;
            }
            set
            {
                m_isEnabled = value;
            }
        }

        public bool IsChecked
        {
            get
            {
                return m_isChecked;
            }
            set
            {
                m_isChecked = value;
            }
        }

        public ICommand Command
        {
            get
            {
                return m_command;
            }
        }

        public ObservableCollection<MenuItemViewModel> Children
        {
            get
            {
                return new ObservableCollection<MenuItemViewModel>(m_children);
            }
        }
        
        public MenuItemViewModel(string displayName, ICommand command)
        {
            m_text = displayName;
            m_command = command;
            m_children = new List<MenuItemViewModel>();
        }

        public void AddMenuItem(MenuItemViewModel menuItem)
        {
            m_children.Add(menuItem);
        }
    }
}
