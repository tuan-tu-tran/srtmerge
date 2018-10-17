using System.Collections.Generic;
using System.IO;

namespace LibSrtMerge.Tests
{
    static class StreamExtensions
    {
        public static byte[] ReadBytes(this Stream s)
        {
            var result = new List<byte>();
            var b = s.ReadByte();
            while (b != -1)
            {
                result.Add((byte)b);
                b = s.ReadByte();
            }
            return result.ToArray();
        }
    }
}