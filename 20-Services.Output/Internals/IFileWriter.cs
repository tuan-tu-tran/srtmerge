using System.IO;

namespace srtmerge.Output.Internals
{
    interface IFileWriter
    {
        Stream OpenWrite(string path);
    }
}