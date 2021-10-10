using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace srtmerge.Merging.Internals
{
    class SubtitleFileMerger : ISubtitleFileMerger
    {
        private readonly FilenameMerger _filenameMerger;
        private readonly SubtitleColorizer _subtitleColorizer;
        private readonly SubtitleMerger _subtitleMerger;

        public SubtitleFileMerger(FilenameMerger filenameMerger, SubtitleColorizer subtitleColorizer, SubtitleMerger subtitleMerger)
        {
            _filenameMerger = filenameMerger;
            _subtitleColorizer = subtitleColorizer;
            _subtitleMerger = subtitleMerger;
        }

        public MergeResult MergeSubtitles(SubtitleFile file1, SubtitleFile file2)
        {
            _subtitleColorizer.Colorize(file1, file2);

            List<SubtitleItem> mergedSubs = _subtitleMerger.MergeSubtitles(file1.SubtitleItems, file2.SubtitleItems,
                out var totaShift, out var averageShift);
            return new MergeResult
            {
                SubtitleItems = mergedSubs,
                AverageShift = averageShift,
                TotalShift = totaShift,
            };
        }
    }
}
