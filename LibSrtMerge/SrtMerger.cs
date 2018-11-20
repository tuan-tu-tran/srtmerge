using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SubtitlesParser.Classes;
using SubtitlesParser.Classes.Parsers;

namespace LibSrtMerge
{
    public class SrtMerger
    {

        public IEnumerable<SubtitleItem> ParseStream(Stream stream)
        {
            var parser = new SubParser();
            var items = parser.ParseStream(stream, Encoding.UTF8);
            foreach (var item in items)
            {
                item.Lines = item.Lines.Select(NormalizeSubtitleLine).ToList();
            }
            return items;
        }

        static string NormalizeSubtitleLine(string s)
        {
            return s
                .Replace("\\N", "\n")
                .Replace("{\\i1}", "<i>")
                .Replace("{\\i0}", "</i>")
            ;
        }

        public void Colorize(IEnumerable<SubtitleItem> items, string hexColor)
        {
            foreach (var item in items)
            {
                item.Lines[0] = "<font color=\"#" + hexColor + "\">" + item.Lines[0];
                item.Lines[item.Lines.Count - 1] += "</font>";
            }
        }

        public void WriteStream(Stream stream, IEnumerable<SubtitleItem> items)
        {
            using (var writer = new StreamWriter(stream))
            {
                var index = 0;
                foreach (var item in items)
                {
                    ++index;
                    if (index > 1)
                        writer.WriteLine();
                    writer.WriteLine(index);
                    writer.WriteLine("{0} --> {1}", FormatTimestamp(item.StartTime), FormatTimestamp(item.EndTime));
                    foreach (var line in item.Lines)
                    {
                        writer.WriteLine(line);
                    }
                }
            }
        }

        private string FormatTimestamp(int startTime)
        {
            return TimeSpan.FromMilliseconds(startTime).ToString("hh\\:mm\\:ss\\,fff");
        }

        public IEnumerable<SubtitleItem> MergeSubtitles(IEnumerable<SubtitleItem> subs1, IEnumerable<SubtitleItem> subs2)
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
