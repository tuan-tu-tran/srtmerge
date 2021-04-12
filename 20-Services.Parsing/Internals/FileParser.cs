using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace srtmerge.Parsing.Internals
{
    class FileParser : IFileParser
    {
        private readonly ZipFileService _zipFileService;
        private readonly ISubtitleParser _subtitleParser;
        private readonly LanguageIdentifier _languageIdentifier;
        private readonly IFileReader _fileReader;

        public FileParser(
            ZipFileService zipFileProcessor,
            ISubtitleParser subtitleParser,
            LanguageIdentifier languageIdentifier,
            IFileReader fileReader)
        {
            _zipFileService = zipFileProcessor;
            _subtitleParser = subtitleParser;
            _languageIdentifier = languageIdentifier;
            _fileReader = fileReader;
        }
        public SubtitleFile ParseFile(string path)
        {
            using (var stream = _fileReader.OpenRead(path))
            {
                var subs = ParseFile(stream, path).ToList();
                return new SubtitleFile
                {
                    Path = path,
                    SubtitleItems = subs,
                    Lang = _languageIdentifier.IdentifyLanguage(subs),
                };
            }
        }

        private IEnumerable<SubtitleItem> ParseFile(Stream stream, string path)
        {
            Stream subtitleStream;
            if (_zipFileService.IsZipFile(stream))
            {
                subtitleStream = _zipFileService.GetSubtitleStreamFromZip(stream);
            }
            else
            {
                subtitleStream = stream;
            }
            return _subtitleParser.ParseSubtitleStream(subtitleStream);
        }
    }
}
