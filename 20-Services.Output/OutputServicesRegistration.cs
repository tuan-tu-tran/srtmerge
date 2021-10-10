using Microsoft.Extensions.DependencyInjection;
using srtmerge.Output.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace srtmerge.Output
{
    public static class OutputServicesRegistration
    {
        public static IServiceCollection AddSubtitlesOutput(this IServiceCollection services)
        {
            //public
            services
                .AddTransient<ISubtitleWriter, SubtitleWriter>()
                .AddTransient<IFilenameManager, FilenameManager>()
            ;

            //internal
            services.AddTransient<IFileWriter, FileWriter>();
            return services;
        }
    }
}
