using Jarvis_Windows.Sources.MVVM.Views.MenuInjectionActionsView;
using Jarvis_Windows.Sources.Utils.Core;
using Jarvis_Windows.Sources.Utils.Services;
using System;
using System.Threading.Tasks;

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

        private PopupDictionaryService? _popupDictionaryService;
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

        #endregion

        public InjectionActionViewModel()
        {
            //Outboard services
            InitialOutboardServices();

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
        }

        private void ExecuteShowMenuOperationsCommand(object obj)
        {
            _menuInjectionActionsViewModel.ShowMenuOperationsCommand.Execute(null);
        }

        void InitialOutboardServices()
        {
            //_popupDictionaryService = DependencyInjection.GetService<PopupDictionaryService>();
            //_googleAnnalyticService = DependencyInjection.GetService<SendEventGA4>();
        }

        private void ExecutePinJarvisButtonCommand(object obj)
        {
            //_popupDictionaryService.MainWindow.PinJarvisButton();
            //PopupDictionaryService.HasPinnedJarvisButton = true;
        }
    }
}

