using System.Collections.Generic;

namespace srtmerge
{
    public class MergeResult
    {
        public List<SubtitleItem> SubtitleItems { get; set; }
        public int TotalShift { get; set; }
        public double AverageShift { get; set; }
    }
}
