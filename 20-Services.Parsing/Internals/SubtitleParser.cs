using SubtitlesParser.Classes.Parsers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace srtmerge.Parsing.Internals
{
    class SubtitleParser : ISubtitleParser
    {
        public IEnumerable<SubtitleItem> ParseSubtitleStream(Stream stream)
        {
            var parser = new SubParser();
            var items = parser.ParseStream(stream, Encoding.UTF8);
            return items.Select(s => new SubtitleItem()
            {
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                Lines = s.Lines.ToList()
            }).ToList();
        }
    }
}
