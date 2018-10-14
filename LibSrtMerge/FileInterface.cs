using System.IO;

namespace LibSrtMerge
{
    internal class FileInterface : IFileInterface
    {
        Stream IFileInterface.Open(string path, FileMode mode)
        {
            return File.Open(path, mode);
        }

        Stream IFileInterface.OpenRead(string path)
        {
            return File.OpenRead(path);
        }

        Stream IFileInterface.OpenWrite(string path)
        {
            return File.OpenWrite(path);
        }
    }
}