using System.Collections.Generic;

namespace srtmerge.Parsing
{
    public interface IFileParser
    {
        SubtitleFile ParseFile(string path);
    }
}