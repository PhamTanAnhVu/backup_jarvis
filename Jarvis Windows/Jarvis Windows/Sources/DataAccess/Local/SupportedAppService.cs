using Jarvis_Windows.Sources.MVVM.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis_Windows.Sources.DataAccess.Local
{
    public interface ISupportedAppService
    {
        public bool IsSupportedInjectionApp(string appName);
    }
    internal class SupportedAppService : ISupportedAppService
    {
        #region Constants
        private const string _configName= "supported_apps.json";
        #endregion

        #region Fields
        private List<SupportedApp>? _supportedInjectionApps;
        #endregion

        public SupportedAppService()
        {
            ReadConfig();
        }

        private void ReadConfig()
        {
            string relativePath = Path.Combine("AppSettings", "Configs", _configName);
            string fullPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath));
            string jsonContent = File.ReadAllText(fullPath);
            if(!string.IsNullOrEmpty(jsonContent))
            {
                var jsonObject = JsonConvert.DeserializeObject<Dictionary<string, List<SupportedApp>>>(jsonContent);
                if(jsonObject != null)
                    _supportedInjectionApps = jsonObject["injection"];
            }
            else
            {
                _supportedInjectionApps = new List<SupportedApp>();
                _supportedInjectionApps.Add(new SupportedApp { Name = "Telegram" });
                _supportedInjectionApps.Add(new SupportedApp { Name = "Messenger" });
                _supportedInjectionApps.Add(new SupportedApp { Name = "Slack" });
                _supportedInjectionApps.Add(new SupportedApp { Name = "Zalo" });
                _supportedInjectionApps.Add(new SupportedApp { Name = "Discord" });
                _supportedInjectionApps.Add(new SupportedApp { Name = "WhatsApp" });
                _supportedInjectionApps.Add(new SupportedApp { Name = "Microsoft Word" });
                _supportedInjectionApps.Add(new SupportedApp { Name = "Notepad" });
                _supportedInjectionApps.Add(new SupportedApp { Name = "Outlook" });
            }
        }

        public bool IsSupportedInjectionApp(string appName)
        {
            if(string.IsNullOrEmpty(appName))
            {
                return false;
            }

            if(_supportedInjectionApps == null || _supportedInjectionApps.Count == 0)
            {
                return true;
            }

            bool isExist = _supportedInjectionApps.Any(x => appName.Contains(x.Name));
            return isExist;
        }
    }
}
