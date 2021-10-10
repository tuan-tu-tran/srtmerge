namespace srtmerge.Output
{
    public interface IFilenameManager
    {
        string GetMergeFilename(string basepath, int? offset = null);
    }
}
