using System.Collections.Generic;

namespace srtmerge.Merging
{
    public interface ISubtitleFileMerger
    {
        SubtitleFile MergeSubtitles(SubtitleFile file1, SubtitleFile file2);
    }
}