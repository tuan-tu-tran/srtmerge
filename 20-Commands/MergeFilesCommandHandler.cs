using Microsoft.Extensions.Logging;
using srtmerge.Merging;
using srtmerge.Output;
using srtmerge.Parsing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace srtmerge.Commands
{
    public class MergeFilesCommandHandler
    {
        private readonly IFileParser _fileParser;
        private readonly ISubtitleFileMerger _subtitleMerger;
        private readonly ILogger<MergeFilesCommandHandler> _logger;
        private readonly IFilenameManager _filenameManager;
        private readonly ISubtitleWriter _subtitleWriter;

        public MergeFilesCommandHandler(
            IFileParser fileParser,
            ISubtitleFileMerger subtitleMerger,
            ILogger<MergeFilesCommandHandler> logger,
            IFilenameManager filenameManager,
            ISubtitleWriter subtitleWriter
        )
        {
            _fileParser = fileParser;
            _subtitleMerger = subtitleMerger;
            _logger = logger;
            _filenameManager = filenameManager;
            _subtitleWriter = subtitleWriter;
        }

        public void MergeFiles(string[] filenames)
        {
            filenames = RemoveMergeFiles(filenames);
            if (filenames.Length < 2)
                throw new ArgumentException("there should be at least 2 files");

            var files = filenames.Select(fname => _fileParser.ParseFile(fname)).ToList();
            if (files.Count > 2)
                MergePairwiseByLanguage(files);
            else
                MergeTwoFiles(files);
        }

        private string[] RemoveMergeFiles(string[] filenames)
        {
            var mergeFilePattern = new Regex("\\.merge-[0-9]+\\.srt");
            return filenames.Where(f => !mergeFilePattern.IsMatch(f)).ToArray();
        }

        private void MergeTwoFiles(List<SubtitleFile> files)
        {
            var subs1 = files[0];
            var subs2 = files[1];

            MergeResult merge = _subtitleMerger.MergeSubtitles(subs1, subs2);

            _logger.LogInformation("merge result: Total shift: {total} | Avg: {avg}", merge.TotalShift, merge.AverageShift);

            _subtitleWriter.WriteFile(merge.SubtitleItems, _filenameManager.GetMergeFilename(subs1.Path));
        }

        private void MergePairwiseByLanguage(List<SubtitleFile> files)
        {
            if (files.Select(f => f.Lang).Distinct().Count() != 2)
                throw new InvalidOperationException("there should be exactly 2 languages");

            var referenceLanguage = files.First().Lang;

            var lang1 = files.Where(f => f.Lang == referenceLanguage);
            var lang2 = files.Where(f => f.Lang != referenceLanguage);
            var mergeResults = new List<MergeResult>();
            foreach (var f1 in lang1)
            {
                foreach (var f2 in lang2)
                {
                    mergeResults.Add(_subtitleMerger.MergeSubtitles(f1, f2));
                }
            }

            mergeResults = mergeResults.OrderBy(m => m.AverageShift).ThenBy(m => m.TotalShift).ToList();

            int i = 0;
            foreach (var merge in mergeResults.Take(7))
            {
                ++i;
                _logger.LogInformation("{i} : average: {avg} | total : {total}", i, merge.AverageShift, merge.TotalShift);
                var path = _filenameManager.GetMergeFilename(files.First().Path, i);
                _subtitleWriter.WriteFile(merge.SubtitleItems, path);
            }
        }
    }
}
