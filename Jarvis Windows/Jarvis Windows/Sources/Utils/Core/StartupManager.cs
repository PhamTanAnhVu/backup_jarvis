using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace Jarvis_Windows.Sources.Utils.Core
{
    public class StartupManager
    {
        #region Constants
        private const string STARTUP_TASK_NAME = "JarvisStartupTask";
        #endregion

        public static async Task<bool> RegisterStartupAsync()
        {
            StartupTask startupTask = await StartupTask.GetAsync(STARTUP_TASK_NAME);

            if (startupTask.State != StartupTaskState.Enabled)
            {
                StartupTaskState state = await startupTask.RequestEnableAsync();
                return state == StartupTaskState.Enabled;
            }

            return true;
        }

        public static async Task<bool> UnregisterStartupAsync()
        {
            StartupTask startupTask = await StartupTask.GetAsync(STARTUP_TASK_NAME);

            if (startupTask.State == StartupTaskState.Enabled)
            {
                startupTask.Disable();
                return true;
            }

            return false;
        }
    }
}
