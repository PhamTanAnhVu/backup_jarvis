using Microsoft.Extensions.Configuration;
using System;

namespace Jarvis_Windows.Sources.DataAccess
{
    public static class DataConfiguration
    {
        private static string _appDirectory = AppDomain.CurrentDomain.BaseDirectory;

        #region Private Fields to get Configuration
        private static string[] _favoriteConversationColors = ["#64748B", "#FACC15"];
        private static string[] _favoriteConversationDatas =
        [
            "M2.14959 11.1381C2.09124 11.4707 2.41941 11.731 2.70924 11.582L6.00116 9.89008L9.29308 11.582C9.58291 11.731 9.91108 11.4707 9.85273 11.1381L9.23044 7.59095L11.8722 5.07347C12.1191 4.83819 11.9913 4.40802 11.6605 4.36104L7.98667 3.83914L6.34858 0.594229C6.20102 0.301924 5.8013 0.301924 5.65374 0.594229L4.01565 3.83914L0.341789 4.36104C0.0110467 4.40802 -0.116753 4.83819 0.130132 5.07347L2.77188 7.59095L2.14959 11.1381ZM5.82804 9.06196L3.06372 10.4827L3.58442 7.51478C3.60896 7.37487 3.56281 7.2316 3.46236 7.13588L1.28223 5.0583L4.32146 4.62655C4.44709 4.6087 4.5568 4.52846 4.61606 4.41106L6.00116 1.66731L7.38626 4.41106C7.44552 4.52846 7.55523 4.6087 7.68086 4.62655L10.7201 5.0583L8.53996 7.13588C8.43951 7.2316 8.39336 7.37487 8.4179 7.51478L8.93859 10.4827L6.17428 9.06196C6.06509 9.00584 5.93723 9.00584 5.82804 9.06196Z",
            "M2.70924 11.582C2.41941 11.731 2.09124 11.4707 2.14959 11.1381L2.77188 7.59095L0.130132 5.07347C-0.116753 4.83819 0.0110467 4.40802 0.341789 4.36104L4.01565 3.83914L5.65374 0.594229C5.8013 0.301924 6.20102 0.301924 6.34858 0.594229L7.98667 3.83914L11.6605 4.36104C11.9913 4.40802 12.1191 4.83819 11.8722 5.07347L9.23044 7.59095L9.85273 11.1381C9.91108 11.4707 9.58291 11.731 9.29308 11.582L6.00116 9.89008L2.70924 11.582Z",
        
        ];
        public static string GetEnvironment()
        {
            string environment;
            #if DEBUG
            environment = "dev";
            #elif RELEASE
            environment = "product";
            #elif DEV
            environment = "dev";
            #elif PRODUCT
            environment = "product";
            #else
            #error "Invalid build configuration"
            #endif

            return environment;
        }
        private static IConfigurationRoot GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(_appDirectory)
                .AddJsonFile($"AppSettings/Envs/settings.{GetEnvironment()}.json", true, true);
            return builder.Build();
        }
        #endregion 

        // Root app directory
        public static string AppDirectory
            => _appDirectory;
        public static string ApiUrl
            => GetConfiguration()["ApiUrl"];

        public static string MeasurementID
            => GetConfiguration()["MeasurementID"];

        public static string ApiSecret
            => GetConfiguration()["ApiSecret"];

        public static string AuthUrl
            => GetConfiguration()["AuthUrl"];

        public static string FilterConversationColor(int i)
        {
            return _favoriteConversationColors[i];
        }

        public static string FilterConversationData(int i)
        {
            return _favoriteConversationDatas[i];
        }


    }
}
