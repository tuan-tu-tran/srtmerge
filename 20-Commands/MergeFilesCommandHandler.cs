using srtmerge.Merging;
using srtmerge.Output;
using srtmerge.Parsing;
using System;
using System.Collections.Generic;

namespace srtmerge.Commands
{
    public class MergeFilesCommandHandler
    {
        private readonly IFileParser _fileParser;
        private readonly ISubtitleColorizer _subtitleColorizer;
        private readonly ISubtitleMerger _subtitleMerger;
        private readonly ISubtitleWriter _subtitleWriter;

        public MergeFilesCommandHandler(
            IFileParser fileParser,
            ISubtitleColorizer subtitleColorizer,
            ISubtitleMerger subtitleMerger,
            ISubtitleWriter subtitleWriter
        )
        {
            _fileParser = fileParser;
            _subtitleColorizer = subtitleColorizer;
            _subtitleMerger = subtitleMerger;
            _subtitleWriter = subtitleWriter;
        }

        public void MergeFiles(string file1, string file2)
        {
            SubtitleFile subs1 = _fileParser.ParseFile(file1);
            SubtitleFile subs2 = _fileParser.ParseFile(file2);

            _subtitleColorizer.Colorize(subs1.SubtitleItems, "ffff54");

            SubtitleFile merge = _subtitleMerger.MergeSubtitles(subs1, subs2);

            _subtitleWriter.WriteFile(merge);
        }
    }
}
