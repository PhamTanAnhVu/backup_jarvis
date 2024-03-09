using Jarvis_Windows.Sources.Utils.Core;
using Jarvis_Windows.Sources.Utils.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis_Windows.Sources.MVVM.Views.JarvisActionView
{
    public class JarvisActionViewModel : ViewModelBase
    {
        public RelayCommand ShowMenuOperationsCommand { get; set; }
        private PopupDictionaryService _popupDictionaryService;

        public PopupDictionaryService PopupDictionaryService
        {
            get { return _popupDictionaryService; }
            set => _popupDictionaryService = value;
        }

    public JarvisActionViewModel(PopupDictionaryService popupDictionaryService)
    {
        PopupDictionaryService = popupDictionaryService;
        ShowMenuOperationsCommand = new RelayCommand(ExecuteShowMenuCommand, o => true);
    }

    private void ExecuteShowMenuCommand(object obj)
    {
        /*if(!PopupDictionaryService.IsDragging)
            PopupDictionaryService.ShowMenuOperations(true);*/
    }

        public JarvisActionViewModel()
        {
        }
    }

}

