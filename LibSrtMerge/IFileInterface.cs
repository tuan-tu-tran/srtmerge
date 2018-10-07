using System;
using System.IO;

namespace LibSrtMerge
{
    public interface IFileInterface
    {
        Stream OpenRead(string path);
        Stream OpenWrite(string path);
    }
}