using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using SubtitlesParser.Classes;
using SubtitlesParser.Classes.Parsers;

namespace LibSrtMerge
{
    public class SrtMerger
    {
        Encoding _encoding = Encoding.UTF8;
        private readonly ILogger<SrtMerger> _logger;

        public SrtMerger(ILogger<SrtMerger> logger)
        {
            _logger = logger;
        }

        public IEnumerable<SubtitleItem> ParseStream(Stream stream)
        {
            stream = CleanStream(stream);

            var parser = new SubParser();
            var items = parser.ParseStream(stream, Encoding.UTF8);
            foreach (var item in items)
            {
                item.Lines = item.Lines.Select(NormalizeSubtitleLine).ToList();
            }
            return items;
        }

        private Stream CleanStream(Stream stream)
        {
            /*
            Normally, an srt file looks like

            1
            00:02:17,440 --> 00:02:20,375
            Senator, we're making
            our final approach into Coruscant.

            2
            00:02:20,476 --> 00:02:22,501
            Very good, Lieutenant.

            But in some files, the empty line to separate the subtitles is missing.
            The parsing library then "merges" the 2 subtitles and weird text appears.

            This code tries to detect missing empty lines and add them.
            */
            List<string> output = new List<string>();
            var reader = new StreamReader(stream);
            var indexRegex = new Regex(@"^\d+$");
            var timeCodeRegex = new Regex(@"^\d\d:\d\d:\d\d,\d\d\d --> \d\d:\d\d:\d\d,\d\d\d$");
            string line, prev = null, prev2 = null;
            while ((line = reader.ReadLine()) != null)
            {
                try
                {
                    if (timeCodeRegex.IsMatch(line))
                    {
                        if (prev == null) throw new ApplicationException("did not expect prev to be null");
                        if (!indexRegex.IsMatch(prev)) throw new ApplicationException("did not expect prev to not be a number");
                        if (!String.IsNullOrEmpty(prev2))
                        {
                            _logger.LogWarning("a blank line had to be inserted before {prev} : {line}", prev, line);
                            output.Insert(output.Count - 1, "");
                        }
                    }
                    output.Add(line);
                    prev2 = prev;
                    prev = line;
                }
                catch
                {
                    _logger.LogError("Error while cleaning line: {line}", line);
                    throw;
                }

            }
            string content =
                String.Join("", output.Select(l => l + Environment.NewLine));
            _logger.LogTrace("cleaned content:\r\n{content}", content);
            return new MemoryStream(_encoding.GetBytes(
                content
            ));

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
