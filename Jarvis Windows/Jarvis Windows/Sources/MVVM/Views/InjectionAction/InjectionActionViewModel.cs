using Jarvis_Windows.Sources.MVVM.Views.MenuInjectionActionsView;
using Jarvis_Windows.Sources.Utils.Core;
using Jarvis_Windows.Sources.Utils.Services;
using System;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace Jarvis_Windows.Sources.MVVM.Views.InjectionAction
{
    public class InjectionActionViewModel : ViewModelBase
    {
        #region Fields
        private bool _isSpinningJarvisIcon;
        private bool _isAnimationEnabled = true;
        private double _horizontalOffset;
        private double _verticalOffset;
        private MenuInjectionActionsViewModel? _menuInjectionActionsViewModel;
        private SendEventGA4? _googleAnnalyticService;
        #endregion

        #region Properties
        public bool IsSpinningJarvisIcon
        {
            get { return _isSpinningJarvisIcon; }
            set
            {
                _isSpinningJarvisIcon = value;
                OnPropertyChanged();
            }
        }

        public bool IsAnimationEnabled
        {
            get { return _isAnimationEnabled; }
            set
            {
                _isAnimationEnabled = value;
                OnPropertyChanged();
            }
        }

        public double HorizontalOffset
        {
            get => _horizontalOffset;
            set
            {
                _horizontalOffset = value;
                OnPropertyChanged();
            }
        }

        public double VerticalOffset
        {
            get => _verticalOffset;
            set
            {
                _verticalOffset = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Commands
        public RelayCommand? ShowMenuOperationsCommand { get; set; }
        public RelayCommand? PinJarvisButtonCommand { get; set; }
        public static RelayCommand? StartSpinJarvisIconCommand { get; set;}
        public static RelayCommand? StopSpinJarvisIconCommand { get; set;}

        #endregion

        public InjectionActionViewModel()
        {
            //Commands
            InitialCommands();

            //Menu Injection Actions
            MenuInjectionActionsView.MenuInjectionActionsView menuInjectionActionsView = new MenuInjectionActionsView.MenuInjectionActionsView();
            _menuInjectionActionsViewModel = (MenuInjectionActionsViewModel?)menuInjectionActionsView.DataContext;
        }

        private void InitialCommands()
        {
            ShowMenuOperationsCommand = new RelayCommand(ExecuteShowMenuOperationsCommand, o => true);
            PinJarvisButtonCommand = new RelayCommand(ExecutePinJarvisButtonCommand, o => true);
            StartSpinJarvisIconCommand = new RelayCommand(ExecuteSpinJarvisIcon, o => true);
            StopSpinJarvisIconCommand = new RelayCommand(ExecuteStopSpinJarvisIcon, o => true);
        }

        private void ExecuteStopSpinJarvisIcon(object obj)
        {
            IsSpinningJarvisIcon = false;
        }

        private void ExecuteSpinJarvisIcon(object obj)
        {
            IsSpinningJarvisIcon = true;
        }

        private void ExecuteShowMenuOperationsCommand(object obj)
        {
            _menuInjectionActionsViewModel.ShowMenuOperationsCommand.Execute(null);
        }

        private void ExecutePinJarvisButtonCommand(object obj)
        {
            //_popupDictionaryService.MainWindow.PinJarvisButton();
            //PopupDictionaryService.HasPinnedJarvisButton = true;
        }
    }
}

