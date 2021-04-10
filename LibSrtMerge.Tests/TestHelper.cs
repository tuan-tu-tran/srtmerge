using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibSrtMerge.Tests
{
    static class TestHelper
    {
        private static ServiceProvider serviceProvider;

        static TestHelper()
        {
            var services = new ServiceCollection();
            services.AddLogging(logging =>
                logging
                    .ClearProviders()
                    .AddNLog()
                    .SetMinimumLevel(LogLevel.Trace)
            );
            serviceProvider = services.BuildServiceProvider();
        }

        public static ILogger<T> GetLogger<T>()
        {
            return serviceProvider.GetRequiredService<ILogger<T>>();
        }
    }
}
