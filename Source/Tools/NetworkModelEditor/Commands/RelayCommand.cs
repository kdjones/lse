using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NetworkModelEditor.Commands
{
    public class RelayCommand : ICommand
    {
        #region [ Fields ]
        
        readonly Action<object> m_execute; 
        readonly Predicate<object> m_canExecute; 
        
        #endregion 
        
        #region Constructors 
        
        public RelayCommand(Action<object> execute) 
            :this(execute, null) 
        { 
        } 
        
        public RelayCommand(Action<object> execute, Predicate<object> canExecute) 
        { 
            if (execute == null) throw new ArgumentNullException("execute"); 
            m_execute = execute; 
            m_canExecute = canExecute; 
        } 
        
        #endregion 
        
        #region ICommand Members 
         
        public bool CanExecute(object parameter) 
        {
            return m_canExecute == null ? true : m_canExecute(parameter); 
        } 

        public event EventHandler CanExecuteChanged 
        { 
            add 
            { 
                CommandManager.RequerySuggested += value; 
            } 
            remove 
            { 
                CommandManager.RequerySuggested -= value; 
            } 
        }

        public void Execute(object parameter) 
        { 
            m_execute(parameter); 
        } 
        #endregion 
    }
}
