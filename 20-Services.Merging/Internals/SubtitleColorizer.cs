using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace srtmerge.Merging.Internals
{
    class SubtitleColorizer : ISubtitleColorizer
    {
        public void Colorize(IEnumerable<SubtitleItem> items, string hexColor)
        {
            foreach (var item in items)
            {
                item.Lines[0] = "<font color=\"#" + hexColor + "\">" + item.Lines[0];
                item.Lines[item.Lines.Count - 1] += "</font>";
            }
        }

    }
}
