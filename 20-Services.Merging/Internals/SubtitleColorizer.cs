using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace srtmerge.Merging.Internals
{
    class SubtitleColorizer
    {
        const string hexColor = "ffff54";
        private const string _openingTag = "<font color=\"#" + hexColor + "\">";

        public void Colorize(SubtitleFile file1, SubtitleFile file2)
        {
            var fileToColorize = (file1.Lang, file2.Lang) switch
            {
                (Language.VN, Language.UNKNOWN) => file1,
                (Language.UNKNOWN, Language.VN) => file2,
                _ => file1
            };

            foreach (var item in fileToColorize.SubtitleItems)
            {
                if (!item.Lines[0].StartsWith(_openingTag))
                {
                    item.Lines[0] = _openingTag + item.Lines[0];
                    item.Lines[item.Lines.Count - 1] += "</font>";
                }
            }
        }
    }
}
