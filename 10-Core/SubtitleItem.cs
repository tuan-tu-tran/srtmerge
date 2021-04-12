using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace srtmerge
{
    public class SubtitleItem
    {
        public int StartTime { get; set; }
        public int EndTime { get; set; }
        public List<string> Lines { get; set; }
    }
}
