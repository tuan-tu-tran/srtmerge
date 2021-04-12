using System;
using System.IO;

namespace srtmerge.Output.Internals
{
    class FileWriter : IFileWriter
    {
        public Stream OpenWrite(string path)
        {
            return File.OpenWrite(path);
        }
    }

}