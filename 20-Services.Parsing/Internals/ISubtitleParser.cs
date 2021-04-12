using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace srtmerge.Parsing.Internals
{
    interface ISubtitleParser
    {
        IEnumerable<SubtitleItem> ParseSubtitleStream(Stream stream);
    }
}
