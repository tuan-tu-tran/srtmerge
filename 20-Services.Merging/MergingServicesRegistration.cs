using Microsoft.Extensions.DependencyInjection;
using srtmerge.Merging.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace srtmerge.Merging
{
    public static class MergingServicesRegistration
    {
        public static IServiceCollection AddSubtitlesMerging(this IServiceCollection services)
        {
            services
                //public
                .AddTransient<ISubtitleMerger, SubtitleMerger>()

                //internal
                .AddTransient<FilenameMerger>()
                .AddTransient<SubtitleColorizer>()
            ;
            return services;
        }
    }
}
