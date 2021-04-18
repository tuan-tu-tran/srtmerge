using System;
using System.Collections.Generic;
using System.Linq;

namespace srtmerge.Merging.Internals
{
    public class SubtitleMerger
    {
        internal List<SubtitleItem> MergeSubtitles(
            IEnumerable<SubtitleItem> subs1,
            IEnumerable<SubtitleItem> subs2,
            out int totaShift,
            out double averageShift)
        {
            totaShift = 0;
            var q1 = new Queue<SubtitleItem>(subs1);
            var q2 = new Queue<SubtitleItem>(subs2);

            var result = new List<SubtitleItem>();
            SubtitleItem lastItem = null;
            while (q1.Count > 0 && q2.Count > 0)
            {
                if (lastItem == null)
                {
                    var s1 = q1.Peek();
                    var s2 = q2.Peek();
                    if (s1.Overlaps(s2))
                    {
                        lastItem = s1.Clone();
                        lastItem.Lines.AddRange(s2.Lines);
                        result.Add(lastItem);
                        totaShift += s1.GetShift(s2);
                        q1.Dequeue();
                        q2.Dequeue();
                    }
                    else if (s1.StartTime <= s2.StartTime)
                    {
                        lastItem = s1.Clone();
                        result.Add(lastItem);
                        q1.Dequeue();
                    }
                    else
                    {
                        lastItem = q2.Dequeue().Clone();
                        result.Add(lastItem);
                    }
                }
                else
                {
                    var s1 = q1.Peek();
                    var s2 = q2.Peek();
                    if (s2.Overlaps(lastItem))
                    {
                        if (!s2.Overlaps(s1) || s2.GetOverlap(lastItem) >= s2.GetOverlap(s1))
                        {
                            lastItem.Lines.AddRange(s2.Lines);
                            q2.Dequeue();
                        }
                        else //s2.Overlaps(s1) && s2.GetOverlap(s1) > s2.GetOverlap(lastItem)
                        {
                            lastItem = s1.Clone();
                            lastItem.Lines.AddRange(s2.Lines);
                            result.Add(lastItem);
                            totaShift += s1.GetShift(s2);
                            q1.Dequeue();
                            q2.Dequeue();
                        }
                    }
                    else
                    {
                        if (s1.Overlaps(s2))
                        {
                            lastItem = s1.Clone();
                            lastItem.Lines.AddRange(s2.Lines);
                            result.Add(lastItem);
                            totaShift += s1.GetShift(s2);
                            q1.Dequeue();
                            q2.Dequeue();
                        }
                        else if (s1.StartTime <= s2.StartTime)
                        {
                            lastItem = s1.Clone();
                            result.Add(lastItem);
                            q1.Dequeue();
                        }
                        else
                        {
                            lastItem = q2.Dequeue().Clone();
                            result.Add(lastItem);
                        }

                    }
                }
            }
            result.AddRange(q1);
            result.AddRange(q2);

            averageShift = 1.0 * totaShift / subs2.Count();
            return result;
        }
    }
}