using Jarvis_Windows.Sources.Utils.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis_Windows.Sources.MVVM.ViewModels
{
    public class MenuInjectionActionsViewModel :ViewModelBase
    {
        public RelayCommand AICommand { get; set; }
        public RelayCommand ExpandCommand { get; set; }
        public RelayCommand OpenSettingsCommand { get; set; }
        public RelayCommand PinJarvisButtonCommand { get; set; }
        public RelayCommand UndoCommand { get; set; }
        public RelayCommand RedoCommand { get; set; }
        public RelayCommand UpgradePlanCommand { get; set; }

        public MenuInjectionActionsViewModel()
        {
            AICommand = new RelayCommand(ExecuteAICommand, o => true);
            ExpandCommand = new RelayCommand(ExecuteExpandCommand, o => true);
            OpenSettingsCommand = new RelayCommand(ExecuteOpenSettingsCommand, o => true);
            PinJarvisButtonCommand = new RelayCommand(ExecutePinJarvisButtonCommand, o => true);
            UndoCommand = new RelayCommand(ExecuteUndoCommand, o => true);
            RedoCommand = new RelayCommand(ExecuteRedoCommand, o => true);
            UpgradePlanCommand = new RelayCommand(ExecuteUpgradePlanCommand, o => true);
        }

        private void ExecuteUpgradePlanCommand(object obj)
        {
            throw new NotImplementedException();
        }

        private void ExecuteRedoCommand(object obj)
        {
            throw new NotImplementedException();
        }

        private void ExecuteUndoCommand(object obj)
        {
            throw new NotImplementedException();
        }

        private void ExecutePinJarvisButtonCommand(object obj)
        {
            throw new NotImplementedException();
        }

        private void ExecuteOpenSettingsCommand(object obj)
        {
            throw new NotImplementedException();
        }

        private void ExecuteExpandCommand(object obj)
        {
            throw new NotImplementedException();
        }

        private void ExecuteAICommand(object obj)
        {
            throw new NotImplementedException();
        }
    }
}
