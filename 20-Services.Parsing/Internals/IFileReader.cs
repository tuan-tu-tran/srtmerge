using System;
using System.IO;

namespace srtmerge.Parsing.Internals
{
    interface IFileReader
    {
        Stream OpenRead(string path);
    }
}