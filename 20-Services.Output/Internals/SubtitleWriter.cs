using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace srtmerge.Output.Internals
{
    class SubtitleWriter : ISubtitleWriter
    {
        private readonly IFileWriter _fileWriter;

        public SubtitleWriter(IFileWriter fileWriter)
        {
            _fileWriter = fileWriter;
        }

        public void WriteFile(IEnumerable<SubtitleItem> subtitles, string path)
        {
            using (var stream = _fileWriter.OpenWrite(path))
            {
                WriteStream(stream, subtitles);
            }
        }

        private void WriteStream(Stream stream, IEnumerable<SubtitleItem> items)
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
    }
}
