using System.IO;

namespace srtmerge.Parsing.Internals
{
    class FileReader : IFileReader
    {
        public Stream OpenRead(string path)
        {
            return File.OpenRead(path);
        }
    }
}