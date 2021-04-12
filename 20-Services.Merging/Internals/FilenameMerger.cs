using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace srtmerge.Merging.Internals
{
    class FilenameMerger
    {
        public string GetMergeFilename(string path1)
        {
            var folder = Path.GetDirectoryName(path1);
            var fname = Path.GetFileNameWithoutExtension(path1);
            fname = fname + ".merge.srt";
            return Path.Combine(folder, fname);
        }
    }
}
