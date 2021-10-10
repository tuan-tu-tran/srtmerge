using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace srtmerge.Output
{
    public interface ISubtitleWriter
    {
        void WriteFile(IEnumerable<SubtitleItem> subtitles, string filename);
    }
}
