using System.Collections.Generic;
using System.IO;
using System.Text;

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

        public static string ReadToEnd(this Stream s)
        {
            return new StreamReader(s).ReadToEnd();
        }

        public static string AsString(this byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }

        public static string WithLF(this string s)
        {
            return s.Replace("\r\n", "\n");
        }
    }
}