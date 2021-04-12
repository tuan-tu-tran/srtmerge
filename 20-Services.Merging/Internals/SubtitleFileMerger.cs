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

        public SubtitleFileMerger(FilenameMerger filenameMerger, SubtitleColorizer subtitleColorizer)
        {
            _filenameMerger = filenameMerger;
            _subtitleColorizer = subtitleColorizer;
        }

        public SubtitleFile MergeSubtitles(SubtitleFile file1, SubtitleFile file2)
        {
            _subtitleColorizer.Colorize(file1.SubtitleItems, "ffff54");

            return new SubtitleFile
            {
                Path = _filenameMerger.GetMergeFilename(file1.Path),
                SubtitleItems = MergeSubtitles(file1.SubtitleItems, file2.SubtitleItems),
            };
        }

        private List<SubtitleItem> MergeSubtitles(IEnumerable<SubtitleItem> subs1, IEnumerable<SubtitleItem> subs2)
        {
            var q1 = new Queue<SubtitleItem>(subs1);
            var q2 = new Queue<SubtitleItem>(subs2);

            var result = new List<SubtitleItem>();
            SubtitleItem current = null;
            while (q1.Count > 0 && q2.Count > 0)
            {
                if (current == null)
                {
                    var s1 = q1.Peek();
                    var s2 = q2.Peek();
                    if (s1.Overlaps(s2))
                    {
                        current = s1.Clone();
                        current.Lines.AddRange(s2.Lines);
                        result.Add(current);
                        q1.Dequeue();
                        q2.Dequeue();
                    }
                    else if (s1.StartTime <= s2.StartTime)
                    {
                        current = s1.Clone();
                        result.Add(current);
                        q1.Dequeue();
                    }
                    else
                    {
                        current = q2.Dequeue().Clone();
                        result.Add(current);
                    }
                }
                else
                {
                    var s1 = q1.Peek();
                    var s2 = q2.Peek();
                    if (s2.Overlaps(current))
                    {
                        if (!s2.Overlaps(s1) || s2.GetOverlap(current) >= s2.GetOverlap(s1))
                        {
                            current.Lines.AddRange(s2.Lines);
                            q2.Dequeue();
                        }
                        else
                        {
                            current = s1.Clone();
                            current.Lines.AddRange(s2.Lines);
                            result.Add(current);
                            q1.Dequeue();
                            q2.Dequeue();
                        }
                    }
                    else
                    {
                        if (s1.Overlaps(s2))
                        {
                            current = s1.Clone();
                            current.Lines.AddRange(s2.Lines);
                            result.Add(current);
                            q1.Dequeue();
                            q2.Dequeue();
                        }
                        else if (s1.StartTime <= s2.StartTime)
                        {
                            current = s1.Clone();
                            result.Add(current);
                            q1.Dequeue();
                        }
                        else
                        {
                            current = q2.Dequeue().Clone();
                            result.Add(current);
                        }

                    }
                }
            }
            result.AddRange(q1);
            result.AddRange(q2);
            return result;
        }
    }
}
