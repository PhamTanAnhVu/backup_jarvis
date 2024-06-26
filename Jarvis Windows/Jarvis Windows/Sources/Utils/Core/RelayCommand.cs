using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Jarvis_Windows.Sources.Utils.Core
{
    public class RelayCommand : ICommand
    {
        private RelayCommand? executeReviseCommand;
        private Func<object, bool> value;

        public event EventHandler? CanExecuteChanged;

        private Action<object> _Excute { get; set; }
        private Predicate<object> _CanExcute { get; set; }

        public RelayCommand(Action<object> ExcuteMethod, Predicate<object> CanExcuteMethod)
        {
            _Excute = ExcuteMethod;
            _CanExcute = CanExcuteMethod;
        }

        public RelayCommand(RelayCommand? executeReviseCommand, Func<object, bool> value)
        {
            this.executeReviseCommand = executeReviseCommand;
            this.value = value;
        }

        public event EventHandler CanExecuteChange
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object? parameter)
        {
            return _CanExcute(parameter);
        }

        public void Execute(object? parameter)
        {
            _Excute(parameter);
        }
    }
}
