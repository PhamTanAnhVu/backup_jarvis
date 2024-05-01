using Jarvis_Windows.Sources.MVVM.Views.MenuInjectionActionsView;
using Jarvis_Windows.Sources.Utils.Core;
using Jarvis_Windows.Sources.Utils.Services;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using Windows.Devices.Geolocation;

namespace Jarvis_Windows.Sources.MVVM.Views.InjectionAction
{
    public class InjectionActionViewModel : ViewModelBase
    {
        #region Fields
        private bool? _isSpinningJarvisIcon;
        private bool? _isAnimationEnabled = true;
        private double? _horizontalOffset;
        private double? _verticalOffset;
        private CornerRadius? _customCornerRadius;
        #endregion

        #region Properties
        public bool? IsSpinningJarvisIcon
        {
            get { return _isSpinningJarvisIcon; }
            set
            {
                _isSpinningJarvisIcon = value;
                OnPropertyChanged();
            }
        }
        
        public bool? IsAnimationEnabled
        {
            get { return _isAnimationEnabled; }
            set
            {
                _isAnimationEnabled = value;
                OnPropertyChanged();
            }
        }

        public double? HorizontalOffset
        { 
            get => _horizontalOffset;
            set
            {
                _horizontalOffset = value;
                OnPropertyChanged();
            } 
        }

        public double? VerticalOffset
        { 
            get => _verticalOffset; 
            set
            {
                _verticalOffset = value;
                OnPropertyChanged();
            }
        }

        public CornerRadius? CustomCornerRadius 
        { 
            get => _customCornerRadius; 
            set
            {
                _customCornerRadius = value;
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
            //Base
            _isSpinningJarvisIcon = false;
            _isAnimationEnabled = true;
            _horizontalOffset = 0;
            _verticalOffset = 0;
            _customCornerRadius = new CornerRadius(15, 15, 15, 5);

            //Commands
            InitialCommands();
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

        private void ExecutePinJarvisButtonCommand(object obj)
        {
            if(!PopupDictionaryService.Instance().IsDragging)
            {
                PopupDictionaryService.Instance().ShowMenuOperations(true);
                PopupDictionaryService.Instance().ShowJarvisAction(false);
                _ = GoogleAnalyticService.Instance().SendEvent("open_input_actions");
            }
        }

        private void ExecutePinJarvisButtonCommand(object obj)
        {
            PopupDictionaryService.Instance().PinJarvisButton();
            PopupDictionaryService.HasPinnedJarvisButton = true;
            CustomCornerRadius = new CornerRadius(15, 15, 15, 5);
        }
    }
}

