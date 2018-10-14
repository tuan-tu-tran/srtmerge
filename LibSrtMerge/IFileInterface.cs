using System;
using System.IO;

namespace LibSrtMerge
{
    public interface IFileInterface
    {
        Stream OpenRead(string path);
        Stream OpenWrite(string path);
        Stream Open(string path, FileMode mode);
    }
}