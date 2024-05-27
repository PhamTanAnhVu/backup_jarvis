using System;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;

namespace Jarvis_Background_Service
{
    public partial class App : System.Windows.Application
    {
        private readonly int TEN_SECONDS = 1000 * 60 * 30;

        protected override void OnStartup(StartupEventArgs e)
        {
            DestroyOldProcess();

            base.OnStartup(e);

            //Start auto restart service
            AutoRestartService();
        }

        private void DestroyOldProcess()
        {
            Process[] foundProcesses = Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName);
            foreach (Process process in foundProcesses)
            {
                if (process.Id != Process.GetCurrentProcess().Id)
                {
                    process.Kill();
                }
            }
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

        private void AutoRestartService()
        {
            while (true)
            {
                //TODO: Find process by name
                bool isJarvisWindowsRunning = false;
                foreach (Process process in Process.GetProcesses())
                {
                    if (process.ProcessName == "Jarvis Windows")
                    {
                        isJarvisWindowsRunning = true;
                    }
                }

                if (isJarvisWindowsRunning)
                {
                    //Debug.WriteLine("Jarvis Windows is already running");
                }
                else
                {
                    Process jarvisWindows = new Process();
                    string? packagePath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
                    if(!IsDebugMode())
                    {
                        packagePath = packagePath?.Replace("Jarvis Background Service", "Jarvis Windows");
                        jarvisWindows.StartInfo.FileName = Path.Combine(packagePath, "Jarvis Windows.exe");
                        if (!File.Exists(jarvisWindows.StartInfo.FileName))
                        {
                            System.Windows.MessageBox.Show(jarvisWindows.StartInfo.FileName);
                        }
                        jarvisWindows.Start();
                    }
                }

                Thread.Sleep(TEN_SECONDS);
            }
        }
    }

}
