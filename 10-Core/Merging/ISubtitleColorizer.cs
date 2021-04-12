using System.Collections.Generic;

namespace srtmerge.Merging
{
    public interface ISubtitleColorizer
    {
        void Colorize(IEnumerable<SubtitleItem> items, string hexColor);
    }
}