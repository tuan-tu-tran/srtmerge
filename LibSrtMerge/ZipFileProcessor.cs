using System.IO;
using System.Linq;

namespace LibSrtMerge
{
    public class ZipFileProcessor
    {
        public bool IsZipFile(Stream s)
        {
            if (s.Length < 4)
                return false;
            var header = new byte[4];
            s.Read(header, 0, 4);
            
            //shoud be 0x04034b50 (little-endiand number)
            return header.SequenceEqual(new byte[]{
                0x50, 0x4b, 0x03, 0x04
            });
        }
    }
}