using Jarvis_Windows.Sources.Utils.Core;
using System;
using System.Windows;

namespace Jarvis_Windows.Sources.MVVM.Views.ExhaustedTokens;

public class ExhautedTokensViewModel
{
    private string _authUrl;

    public RelayCommand UpgradePlanCommand { get; set; }

    public ExhautedTokensViewModel()
    {
        UpgradePlanCommand = new RelayCommand(ExecuteUpgradePlanCommand, o => true);
    }

    public async void ExecuteUpgradePlanCommand(object obj)
    {
        try
        {
            string websiteUrl = _authUrl;
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = websiteUrl,
                UseShellExecute = true
            });

            Application.Current.Shutdown();
        }
        catch (Exception ex)
        {
            return;
        }
    }
}
