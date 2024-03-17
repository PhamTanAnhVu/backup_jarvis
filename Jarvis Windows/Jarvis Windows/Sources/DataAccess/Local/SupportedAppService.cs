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
        public bool IsSupportedApp(string appName);
    }
    internal class SupportedAppService : ISupportedAppService
    {
        #region Constants
        private const string _configName= "supported_apps.json";
        #endregion

        #region Fields
        private List<SupportedApp>? _supportedApps;
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
                _supportedApps = JsonConvert.DeserializeObject<List<SupportedApp>>(jsonContent);
            }
            else
            {
                _supportedApps = new List<SupportedApp>();
            }
        }

        public bool IsSupportedApp(string appName)
        {
            if(string.IsNullOrEmpty(appName))
            {
                return false;
            }

            if(_supportedApps == null || _supportedApps.Count == 0)
            {
                return true;
            }

            bool isExist = _supportedApps.Any(x => appName.Contains(x.Name));
            return isExist;
        }
    }
}
