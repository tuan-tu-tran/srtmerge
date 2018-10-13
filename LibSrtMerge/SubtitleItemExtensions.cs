using System;
using SubtitlesParser.Classes;

namespace LibSrtMerge
{
    static class SubtitleItemExtensions
    {
        public static bool Overlaps(this SubtitleItem s1, SubtitleItem s2)
        {
            if (s1.StartTime <= s2.StartTime)
            {
                return s1.EndTime > s2.StartTime;
            }
            else return s2.Overlaps(s1);
        }

        public static SubtitleItem Clone(this SubtitleItem s1)
        {
            return new SubtitleItem()
            {
                StartTime = s1.StartTime,
                EndTime = s1.EndTime,
                Lines = new System.Collections.Generic.List<string>(s1.Lines),
            };
        }

        public static double GetOverlap(this SubtitleItem s1, SubtitleItem s2)
        {
            var overlap = Math.Min(s1.EndTime, s2.EndTime) - Math.Max(s1.StartTime, s2.StartTime);
            return Math.Max(overlap, 0);
        }

    }
}