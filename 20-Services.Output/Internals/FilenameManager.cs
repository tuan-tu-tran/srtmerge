using System.IO;

namespace srtmerge.Output.Internals
{
    internal class FilenameManager : IFilenameManager
    {
        string IFilenameManager.GetMergeFilename(string path, int? offset)
        {
            var folder = Path.GetDirectoryName(path);
            var fname = Path.GetFileNameWithoutExtension(path);
            if (offset == null)
                fname = fname + ".merge.srt";
            else
                fname += $".merge-{offset}.srt";
            return Path.Combine(folder, fname);
        }
    }
}
