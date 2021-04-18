using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using srtmerge.Commands;
using srtmerge.Merging;
using srtmerge.Output;
using srtmerge.Parsing;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ConsoleApp.Tests")]

namespace srtmerge.ConsoleApp
{
    class Program
    {
        static int Main(string[] args)
        {
            ServiceCollection services = MainProgram.ConfigureServices();
            var serviceProvider = services.BuildServiceProvider();

            var program = serviceProvider.GetRequiredService<MainProgram>();
            return program.Run(args, pause: true);
        }
    }

    internal class MainProgram
    {
        private readonly MergeFilesCommandHandler _mergeFilesCommandHandler;

        public MainProgram(MergeFilesCommandHandler mergeFilesCommandHandler)
        {
            _mergeFilesCommandHandler = mergeFilesCommandHandler;
        }

        internal static ServiceCollection ConfigureServices()
        {
            var services = new ServiceCollection();
            services.AddLogging(logging =>
                logging
                    .ClearProviders()
                    .AddNLog()
                    .SetMinimumLevel(LogLevel.Trace)
            );
            services.AddTransient<MergeFilesCommandHandler>();
            services.AddSubtitleParsing();
            services.AddSubtitlesOutput();
            services.AddSubtitlesMerging();
            services.AddTransient<MainProgram>();
            return services;
        }

        internal int Run(string[] args, bool pause = false)
        {
            if (args.Length != 2)
            {
                Console.Error.WriteLine("Error : wrong number of arguments: " + args.Length);
                Console.Error.WriteLine("usage : {0} FILE1 FILE2", Assembly.GetExecutingAssembly().GetName().Name);
                return 1;
            }

            _mergeFilesCommandHandler.MergeFiles(args[0], args[1]);
            if(pause)
            {
                Console.WriteLine("Press any key...");
                Console.ReadKey();
            }
            return 0;
        }
    }
}
