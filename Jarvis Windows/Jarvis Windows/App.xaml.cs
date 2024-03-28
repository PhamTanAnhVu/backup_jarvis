using Jarvis_Windows.Sources.MVVM.Views.MainView;
using Jarvis_Windows.Sources.Utils.Services;
using Microsoft.Win32;
using System;
using System.Windows;
using System.Threading.Tasks;
using System.Threading;
using Jarvis_Windows.Sources.Utils.Accessibility;
using Jarvis_Windows.Sources.DataAccess.Network;
using System.Web;
using Jarvis_Windows.Sources.MVVM.Models;
using System.Reflection;

namespace Jarvis_Windows
{
    public partial class App : Application
    {
        private const string _uniqueEventName = "Jarvis Windows";
        private EventWaitHandle? _eventWaitHandle;

        public App()
        {
            SingleInstanceWatcher();
            //RegisterUriScheme("jarvis-windows");
            DependencyInjection.RegisterServices();
            SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            MainView mainView = DependencyInjection.GetService<MainView>();
            mainView.Show();
            DependencyInjection.GetService<PopupDictionaryService>().MainWindow = mainView;

            //URI Activation
            if (e.Args.Length > 0)
            {
                UriBuilder builder = new UriBuilder(e.Args[0]);
                var result = HttpUtility.ParseQueryString(builder.Query);
                var source = result["source"];
                var accessToken = result["token"];
                var refreshToken = result["refreshToken"];
                if (!String.IsNullOrEmpty(accessToken) && !String.IsNullOrEmpty(refreshToken))
                {
                    WindowLocalStorage.WriteLocalStorage("access_token", accessToken);
                    WindowLocalStorage.WriteLocalStorage("refresh_token", refreshToken);
                    AuthenticationService? authService = DependencyInjection.GetService<IAuthenticationService>() as AuthenticationService;
                    authService.RefreshTokens();
                    MainViewModel mainViewModel = DependencyInjection.GetService<MainViewModel>();
                    Task<Account?> account = authService.GetMe();
                    if (account != null)
                    {
                        mainViewModel.Account = account.Result;
                    }
                }
                DependencyInjection.GetService<MainView>().Show();
            }
        }

        private void SingleInstanceWatcher()
        {
            try
            {
                this._eventWaitHandle = EventWaitHandle.OpenExisting(_uniqueEventName);
                this._eventWaitHandle.Set();
                this.Shutdown();
            }
            catch (WaitHandleCannotBeOpenedException)
            {
                this._eventWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset, _uniqueEventName);
            }

            new Task(() =>
            {
                while (this._eventWaitHandle.WaitOne())
                {
                    Current.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        if (!Current.MainWindow.Equals(null))
                        {                            
                            var mw = Current.MainWindow;

                            if (mw.WindowState == WindowState.Minimized || mw.Visibility != Visibility.Visible)
                            {
                                mw.Show();
                                mw.WindowState = WindowState.Normal;
                            }
                            mw.Activate();
                            mw.Topmost = true;
                            mw.Topmost = false;
                            mw.Focus();
                        }
                    }));
                }
            })
            .Start();
        }


        static void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            if (e.Mode == PowerModes.Suspend)
            {
                UIElementDetector.GetInstance().UnSubscribeToElementFocusChanged();
            }
            else if (e.Mode == PowerModes.Resume)
            {
                UIElementDetector.GetInstance().SubscribeToElementFocusChanged();
            }
        }

        private void RegisterUriScheme(string scheme)
        {
            string executablePath = Assembly.GetExecutingAssembly().Location;
            string friendlyName = "jarvis-windows";

            using (RegistryKey key = Registry.CurrentUser.CreateSubKey($"SOFTWARE\\Classes\\{scheme}"))
            {
                key.SetValue("", $"URL:{friendlyName}");
                key.SetValue("URL Protocol", "");

                using (RegistryKey defaultIconKey = key.CreateSubKey("DefaultIcon"))
                {
                    defaultIconKey.SetValue("", $"\"{executablePath}\",1");
                }

                using (RegistryKey commandKey = key.CreateSubKey(@"shell\open\command"))
                {
                    commandKey.SetValue("", $"\"{executablePath}\" \"%1\"");
                }
            }
        }
    }
}


