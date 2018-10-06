using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SubtitlesParser.Classes;
using SubtitlesParser.Classes.Parsers;

namespace LibSrtMerge
{
    public class SrtMerger
    {

        public IEnumerable<SubtitleItem> ParseStream(Stream stream)
        {
            var parser = new SrtParser();
            return parser.ParseStream(stream, Encoding.UTF8);
        }

        public void Colorize(IEnumerable<SubtitleItem> items)
        {
        }

        public void WriteStream(Stream stream, IEnumerable<SubtitleItem> items)
        {
            using(var writer = new StreamWriter(stream))
            {
                var index = 0;
                foreach (var item in items)
                {
                    ++index;
                    if(index > 1)
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
    }
}
