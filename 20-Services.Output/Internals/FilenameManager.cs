using System.IO;

namespace srtmerge.Output.Internals
{
    internal class FilenameManager : IFilenameManager
    {
        string IFilenameManager.GetMergeFilename(string path)
        {
            var folder = Path.GetDirectoryName(path);
            var fname = Path.GetFileNameWithoutExtension(path);
            fname = fname + ".merge.srt";
            return Path.Combine(folder, fname);
        }
    }
}