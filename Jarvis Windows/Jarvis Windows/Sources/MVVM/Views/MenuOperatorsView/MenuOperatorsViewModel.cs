using Jarvis_Windows.Sources.Utils.Core;
using System;

namespace Jarvis_Windows.Sources.MVVM.Views.MenuOperatorsView
{
    internal class MenuOperatorsViewModel : ViewModelBase
    {
        #region Fields
        private bool _isShowMenu;
        private System.Windows.Point _screenPosition;
        #endregion

        #region Properties
        public bool IsShowMenu
        {
            get { return _isShowMenu; }
            set
            {
                _isShowMenu = value;
                OnPropertyChanged();
            }
        }

        public System.Windows.Point ScreenPosition
        {
            get { return _screenPosition; }
            set
            {
                _screenPosition = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand CloseMenuCommand { get; set; }
        public RelayCommand RedoCommand { get; set; }
        public RelayCommand UndoCommand { get; set; }
        public RelayCommand ReviseCommand { get; set; }
        public RelayCommand ShortenCommand { get; set; }
        public RelayCommand TranslateCommand { get; set; }
        #endregion

        public MenuOperatorsViewModel()
        {
            CloseMenuCommand = new RelayCommand(ExecuteCloseMenu, o => true);
            RedoCommand = new RelayCommand(ExecuteRedo, o => true);
            UndoCommand = new RelayCommand(ExecuteUndo, o => true);
            ReviseCommand = new RelayCommand(ExecuteRevise, o => true);
            ShortenCommand = new RelayCommand(ExecuteShorten, o => true);
            TranslateCommand = new RelayCommand(ExecuteTranslate, o => true);
        }

        private void ExecuteTranslate(object obj)
        {
            throw new NotImplementedException();
        }

        private void ExecuteShorten(object obj)
        {
            throw new NotImplementedException();
        }

        private void ExecuteRevise(object obj)
        {
            throw new NotImplementedException();
        }

        private void ExecuteUndo(object obj)
        {
            throw new NotImplementedException();
        }

        private void ExecuteRedo(object obj)
        {
            throw new NotImplementedException();
        }

        private void ExecuteCloseMenu(object obj)
        {
            throw new NotImplementedException();
        }
    }
}
