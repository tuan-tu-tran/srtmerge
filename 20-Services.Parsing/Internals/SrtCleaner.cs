using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace srtmerge.Parsing.Internals
{
    /// <summary>
    /// Pre-process a subtitle stream to clean out some anomalies
    /// </summary>
    class SrtCleaner : ISubtitleParser
    {
        private readonly ISubtitleParser _inner;
        private readonly ILogger<SrtCleaner> _logger;
        private readonly Encoding _encoding = Encoding.UTF8;

        public SrtCleaner(ISubtitleParser inner, ILogger<SrtCleaner> logger)
        {
            _inner = inner;
            _logger = logger;
        }

        public IEnumerable<SubtitleItem> ParseSubtitleStream(Stream stream)
        {
            var subs = _inner.ParseSubtitleStream(CleanStream(stream)).ToList();
            subs.ForEach(s => s.Lines = s.Lines.Select(NormalizeSubtitleLine).ToList());
            return subs;
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
            See the example jacobs-ladder, cue 103
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
                        if (!string.IsNullOrEmpty(prev2))
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
                string.Join("", output.Select(l => l + Environment.NewLine));
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
                .Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", "\r\n")
            ;
        }
    }
}
