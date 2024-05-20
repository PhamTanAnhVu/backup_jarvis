using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Jarvis_Windows.Sources.Utils.Core
{
    public class BreathService : Process
    {
        private static BreathService? _instance;
        private string _processName = "Jarvis Breath Service";

        public static BreathService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new BreathService();
                }

                return _instance;
            }
        }

        private BreathService()
        {
        }

        public void StartBreath(CancellationToken cancellationToken)
        {
            Thread thread = new Thread(() => StartBreathThread(cancellationToken));
        }

        private async void StartBreathThread(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(1000 * 60 * 30);
                //Check if not found the process with the name "Jarvis Windows" then start the process
                if (Process.GetProcessesByName("Jarvis Windows").Length == 0)
                {
                    App app = new App();
                    app.InitializeComponent();
                    app.Run();
                }
            }
        }
    }
}
