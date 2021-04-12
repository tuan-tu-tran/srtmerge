using System.Collections.Generic;

namespace srtmerge.Merging
{
    public interface ISubtitleMerger
    {
        SubtitleFile MergeSubtitles(SubtitleFile file1, SubtitleFile file2);
    }
}