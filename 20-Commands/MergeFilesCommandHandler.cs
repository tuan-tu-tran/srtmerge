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
        private readonly ISubtitleFileMerger _subtitleMerger;
        private readonly ISubtitleWriter _subtitleWriter;

        public MergeFilesCommandHandler(
            IFileParser fileParser,
            ISubtitleFileMerger subtitleMerger,
            ISubtitleWriter subtitleWriter
        )
        {
            _fileParser = fileParser;
            _subtitleMerger = subtitleMerger;
            _subtitleWriter = subtitleWriter;
        }

        public void MergeFiles(string file1, string file2)
        {
            SubtitleFile subs1 = _fileParser.ParseFile(file1);
            SubtitleFile subs2 = _fileParser.ParseFile(file2);


            SubtitleFile merge = _subtitleMerger.MergeSubtitles(subs1, subs2);

            _subtitleWriter.WriteFile(merge);
        }
    }
}
