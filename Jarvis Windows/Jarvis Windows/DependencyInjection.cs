﻿using Jarvis_Windows.Sources.DataAccess.Local;
using Jarvis_Windows.Sources.DataAccess.Network;
using Jarvis_Windows.Sources.MVVM.Views.JarvisActionView;
using Jarvis_Windows.Sources.MVVM.Views.MainView;
using Jarvis_Windows.Sources.Utils.Accessibility;
using Jarvis_Windows.Sources.Utils.Core;
using Jarvis_Windows.Sources.Utils.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis_Windows
{
    public class DependencyInjection
    {
        private static ServiceProvider _serviceProvider;

        public static void RegisterServices()
        {
            IServiceCollection services = new ServiceCollection();

            services.AddSingleton<Func<Type, ViewModelBase>>(serviceProvider => viewModelType => (ViewModelBase)serviceProvider.GetRequiredService(viewModelType));
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<PopupDictionaryService>();
            services.AddSingleton<SendEventGA4>();
            services.AddScoped<IAutomationElementValueService, AutomationElementValueService>();
            services.AddSingleton<ISupportedAppService, SupportedAppService>();
            services.AddSingleton<ITokenLocalService, TokenLocalService>();
            services.AddSingleton<IAuthenticationService, AuthenticationService>();

            services.AddSingleton<UIElementDetector>(provider => new UIElementDetector
            {
                PopupDictionaryService = provider.GetRequiredService<PopupDictionaryService>(),
                SendEventGA4 = provider.GetRequiredService<SendEventGA4>(),
                AutomationElementValueService = provider.GetRequiredService<IAutomationElementValueService>(),
                SupportedAppService = provider.GetRequiredService<ISupportedAppService>()
            });

            services.AddSingleton<MainViewModel>();
            services.AddSingleton<MainView>(provider => new MainView
            {
                DataContext = provider.GetRequiredService<MainViewModel>(),
                SendEventGA4 = provider.GetRequiredService<SendEventGA4>(),
                PopupDictionaryService = provider.GetRequiredService<PopupDictionaryService>()
            });

            // Logging.Log("After MainView MainviewModel\n");

            services.AddSingleton<JarvisActionViewModel>(provider => new JarvisActionViewModel
            {
                PopupDictionaryService = provider.GetRequiredService<PopupDictionaryService>()
            });

            _serviceProvider = services.BuildServiceProvider();
        }

        public static T GetService<T>()
        {
            return _serviceProvider.GetRequiredService<T>();
        }
    }
}