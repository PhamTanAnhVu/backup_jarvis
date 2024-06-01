using Jarvis_Windows.Sources.Utils.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Input;

namespace Jarvis_Windows.Sources.MVVM.Views.PromptLibrary
{
    public class PromptLibraryViewModel : ViewModelBase
    {
        #region Fields
        private object? _currentPromptPage;
        private Dictionary<string, object> _promptPages = new Dictionary<string, object>();
        #endregion

        #region Properties
        public object? CurrentPromptPage 
        { 
            get => _currentPromptPage; 
            set
            {
                _currentPromptPage = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Commands
        public RelayCommand NavigatePromptViewCommand { get; set; }
        #endregion

        public PromptLibraryViewModel()
        {
            _promptPages.Add("PublicPrompt", new PublicPromptViewModel());
            _promptPages.Add("MyPrompt", new MyPromptViewModel());
            _currentPromptPage = _promptPages["MyPrompt"];

            NavigatePromptViewCommand = new RelayCommand(ExecuteNavigatePromptViewCommand, o => true);
        }

        private void ExecuteNavigatePromptViewCommand(object obj)
        {
            System.Windows.Controls.Button? pressedButton = obj as System.Windows.Controls.Button;
            if (pressedButton != null)
            {
                string token = "btnNavigate";
                string targetViewModel = pressedButton.Name.ToString().Substring(token.Length);

                if (_promptPages.ContainsKey(targetViewModel))
                    CurrentPromptPage = _promptPages[targetViewModel];
            }
            else if (obj is string)
            {
                string targetViewModel = (string)obj;
                if (_promptPages.ContainsKey(targetViewModel))
                    CurrentPromptPage = _promptPages[targetViewModel];
            }
        }
    }
}
