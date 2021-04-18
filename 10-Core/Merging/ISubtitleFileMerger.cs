using System.Collections.Generic;

namespace srtmerge.Merging
{
    public interface ISubtitleFileMerger
    {
        MergeResult MergeSubtitles(SubtitleFile file1, SubtitleFile file2);
    }
}