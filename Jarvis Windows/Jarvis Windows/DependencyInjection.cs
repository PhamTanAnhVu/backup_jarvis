using Jarvis_Windows.Sources.DataAccess.Local;
using Jarvis_Windows.Sources.DataAccess.Network;
using Jarvis_Windows.Sources.MVVM.ViewModels;
using Jarvis_Windows.Sources.MVVM.Views.InjectionAction;
using Jarvis_Windows.Sources.MVVM.Views.MainView;
using Jarvis_Windows.Sources.MVVM.Views.SettingView;
using Jarvis_Windows.Sources.Utils.Accessibility;
using Jarvis_Windows.Sources.Utils.Core;
using Jarvis_Windows.Sources.Utils.Services;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Jarvis_Windows
{
    public class DependencyInjection
    {
        private static ServiceProvider _serviceProvider;

        public static void RegisterServices()
        {
            IServiceCollection services = new ServiceCollection();

            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<SendEventGA4>();
            services.AddScoped<IAutomationElementValueService, AutomationElementValueService>();
            services.AddSingleton<ISupportedAppService, SupportedAppService>();
            services.AddSingleton<ITokenLocalService, TokenLocalService>();
            services.AddSingleton<IAuthenticationService, AuthenticationService>();

            services.AddSingleton<UIElementDetector>(provider => new UIElementDetector
            {
                SendEventGA4 = provider.GetRequiredService<SendEventGA4>(),
                AutomationElementValueService = provider.GetRequiredService<IAutomationElementValueService>(),
                SupportedAppService = provider.GetRequiredService<ISupportedAppService>()
            });

            _serviceProvider = services.BuildServiceProvider();
        }

        public static T GetService<T>()
        {
            return _serviceProvider.GetRequiredService<T>();
        }
    }
}
