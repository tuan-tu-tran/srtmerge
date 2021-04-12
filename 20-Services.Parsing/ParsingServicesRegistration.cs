using Microsoft.Extensions.DependencyInjection;
using srtmerge.Parsing.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace srtmerge.Parsing
{
    public static class ParsingServicesRegistration
    {
        public static IServiceCollection AddSubtitleParsing(this IServiceCollection services)
        {
            //public
            services.AddTransient<IFileParser, FileParser>();

            //internal
            services.AddTransient<IFileReader, FileReader>();
            services.AddTransient<ISubtitleParser, SubtitleParser>();
            services.Decorate<ISubtitleParser, SrtCleaner>();
            services.AddTransient<ZipFileService>();
            return services;
        }
    }
}
