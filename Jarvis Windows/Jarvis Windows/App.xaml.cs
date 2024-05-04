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
using Windows.ApplicationModel.Activation;
using System.Linq;
using WinRT;
using System.Diagnostics;
using Jarvis_Windows.Sources.MVVM.Views.MainView;
using Jarvis_Windows.Sources.MVVM.Views.SettingView;
using Jarvis_Windows.Sources.MVVM.Views.MainNavigationView;

namespace Jarvis_Windows
{
    public partial class App : Application
    {
        private const string _uniqueEventName = "Jarvis Windows";
        private EventWaitHandle? _eventWaitHandle;

        public App()
        {
            if (IsDebugMode())
                RegisterUriSchemeForDebugMode("jarvis-windows");
            DependencyInjection.RegisterServices();
            SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;
        }

        private static bool IsDebugMode()
        {
            bool isDebug = false;
            #if DEBUG
                isDebug = true;
            #else
                isDebug = Debugger.IsAttached;
            #endif
            return isDebug;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            DestroyOldProcesses(); //Single instance application

            //MainView mainView = DependencyInjection.GetService<MainView>();
            //SettingView settingView = DependencyInjection.GetService<SettingView>();
            //mainView.Show();
            //DependencyInjection.GetService<PopupDictionaryService>().MainWindow = mainView;

            MainNavigationView mainNavigationView = new MainNavigationView();
            mainNavigationView.Show();

            PopupDictionaryService.Instance().InitInjectionAction();
            PopupDictionaryService.Instance().InitMenuSelectionActions();
            PopupDictionaryService.Instance().InitMenuSelectionPopupList();
            PopupDictionaryService.Instance().InitMenuSelectionResponse();

            //Test register Accessibility Service
            UIElementDetector accessibilityService = DependencyInjection.GetService<UIElementDetector>();
            accessibilityService.SubscribeToElementFocusChanged();

            if (e.Args.Length > 0) //Activation from URI scheme
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
                    if (account != null && account.Result != null && account.Result.Username != null)
                    {
                        mainViewModel.Account = account.Result;
                        mainViewModel.UsernameFirstLetter = account.Result.Username[0].ToString();

                        //tODO:call remaining usage 
                        _ = JarvisApi.Instance.APIUsageHandler();
                        mainViewModel.RemainingAPIUsage = $"{WindowLocalStorage.ReadLocalStorage("ApiUsageRemaining")} ??";
                    }
                }
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
                        string[] args = Environment.GetCommandLineArgs();
                        if(args.Length > 0)
                        {
                            string uri = args[0];
                            var result = HttpUtility.ParseQueryString(uri);
                            var source = result["source"];
                            var accessToken = result["token"];
                            var refreshToken = result["refreshToken"];
                            if (!String.IsNullOrEmpty(accessToken) && !String.IsNullOrEmpty(refreshToken))
                            {
                                WindowLocalStorage.WriteLocalStorage("access_token", accessToken);
                                WindowLocalStorage.WriteLocalStorage("refresh_token", refreshToken);
                                AuthenticationService? authService = DependencyInjection.GetService<IAuthenticationService>() as AuthenticationService;
                                if(authService != null)
                                {
                                    authService.RefreshTokens();
                                    MainViewModel mainViewModel = DependencyInjection.GetService<MainViewModel>();
                                    Task<Account?> account = authService.GetMe();
                                    if (account != null && account.Result != null && account.Result.Username != null)
                                    {
                                        mainViewModel.Account = account.Result;
                                        mainViewModel.UsernameFirstLetter = account.Result.Username[0].ToString();
                                    }
                                }
                            }
                        }   

                        /*if (Current.MainWindow != null)
                        {
                            var mw = Current.MainWindow;
                            if (mw.WindowState == WindowState.Minimized || mw.Visibility != Visibility.Visible)
                            {
                                mw.Show();
                            }
                        }*/
                    }));
                }
            })
            .Start();
        }

        protected void Activate(IActivatedEventArgs e)
        {
            if (e.Kind == ActivationKind.Protocol)
            {
                ProtocolActivatedEventArgs protocolArgs = e as ProtocolActivatedEventArgs;
                if (protocolArgs != null)
                {
                    Uri uri = protocolArgs.Uri;
                    var result = HttpUtility.ParseQueryString(uri.Query);
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

        private void RegisterUriSchemeForDebugMode(string scheme)
        {
            string executablePath = Assembly.GetExecutingAssembly().Location;
            executablePath = executablePath.Remove(executablePath.Length - 4, 4);
            executablePath = executablePath + ".exe";
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

        private void DestroyOldProcesses()
        {
            string currentProcessName = Process.GetCurrentProcess().ProcessName;
            Process[] processes = Process.GetProcessesByName(currentProcessName);
            foreach (Process process in processes)
            {
                if (process.Id != Process.GetCurrentProcess().Id)
                {
                    process.Kill();
                }
            }
        }   
    }
}


