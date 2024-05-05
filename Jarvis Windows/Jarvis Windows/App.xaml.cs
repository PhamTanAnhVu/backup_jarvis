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
using System.Diagnostics;
using Jarvis_Windows.Sources.MVVM.Views.MainView;
using Jarvis_Windows.Sources.MVVM.Views.MainNavigationView;
using System.IO;
using Windows.ApplicationModel.Contacts;

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

            MainNavigationView mainNavigationView = new MainNavigationView();
            mainNavigationView.Show();

            AccessibilityService.GetInstance().SubscribeToElementFocusChanged();

            PopupDictionaryService.Instance().InitInjectionAction();
            PopupDictionaryService.Instance().InitMenuInjectionActions();
            PopupDictionaryService.Instance().InitMenuSelectionActions();
            PopupDictionaryService.Instance().InitMenuSelectionPopupList();
            PopupDictionaryService.Instance().InitMenuSelectionResponse();

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
                    authService?.RefreshTokens();

                    //TODO: Move to main navigation view
                    //MainViewModel mainViewModel = DependencyInjection.GetService<MainViewModel>();
                    MainNavigationViewModel mainViewModel = (MainNavigationViewModel)mainNavigationView.DataContext;
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

            //Decoupling breath service
            ///Lock test
            Process processBreath = new Process();
            string? packagePath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule?.FileName);
            if(!IsDebugMode())
            {
                packagePath = packagePath?.Replace("Jarvis Windows", "Jarvis Background Service");
                processBreath.StartInfo.FileName = Path.Combine(packagePath, "Jarvis Background Service.exe");
                if (!File.Exists(processBreath.StartInfo.FileName))
                {
                    MessageBox.Show(processBreath.StartInfo.FileName);
                }
                processBreath.Start();
            }
        }

        private void SingleInstanceWatcher()
        {
            /*try
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
                    }));
                }
            })
            .Start();*/
        }

        protected void Activate(IActivatedEventArgs e)
        {
            /*if (e.Kind == ActivationKind.Protocol)
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
            }*/
        }

        static void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            if (e.Mode == PowerModes.Suspend)
            {
                AccessibilityService.GetInstance().UnSubscribeToElementFocusChanged();
            }
            else if (e.Mode == PowerModes.Resume)
            {
                AccessibilityService.GetInstance().SubscribeToElementFocusChanged();
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
            Process[] foundProcesses = Process.GetProcessesByName(currentProcessName);
            foreach (Process process in foundProcesses)
            {
                if (process.Id != Process.GetCurrentProcess().Id)
                {
                    process.Kill();
                }
            }
        }   
    }
}


