using System;
using System.Collections.Generic;
using System.Linq;

namespace srtmerge.Parsing.Internals
{
    public class LanguageIdentifier
    {
        internal Language IdentifyLanguage(List<SubtitleItem> subs)
        {
            foreach (var line in subs.SelectMany(item => item.Lines))
            {
                //var s = "ượếằộểứạủảữáđòỏềựíệớâăừơóô";
                if (line.Contains("đ"))
                    return Language.VN;
            }
            return Language.UNKNOWN;
        }
    }
}