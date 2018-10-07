namespace LibSrtMerge
{
    internal class FileSystem : IFileSystem
    {
        IFileInterface IFileSystem.File => new FileInterface();
    }
}