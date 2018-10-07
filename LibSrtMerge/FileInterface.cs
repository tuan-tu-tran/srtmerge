using System.IO;

namespace LibSrtMerge
{
    internal class FileInterface : IFileInterface
    {
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