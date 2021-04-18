using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace srtmerge
{
    public class SubtitleFile
    {
        public string Path { get; set; }
        public List<SubtitleItem> SubtitleItems { get; set; }
        public Language Lang { get; set; } = Language.UNKNOWN;
    }
}
