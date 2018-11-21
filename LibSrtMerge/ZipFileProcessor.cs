using System;
using System.IO;
using System.IO.Compression;
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
            s.Seek(0, SeekOrigin.Begin);

            //shoud be 0x04034b50 (little-endiand number)
            return header.SequenceEqual(new byte[]{
                0x50, 0x4b, 0x03, 0x04
            });
        }

        public Stream GetSrtStreamFromZip(Stream zipFileStream)
        {
            var archive = new ZipArchive(zipFileStream);
            var srtEntries = archive.Entries.Where(e => e.Name.EndsWith(".srt") || e.Name.EndsWith(".ass")).ToList();
            if (srtEntries.Count > 1)
                throw new ApplicationException("Multiple srt entries found: " + String.Join(" , ", srtEntries.Select(e => e.Name)));
            if (srtEntries.Count() == 0)
                throw new ApplicationException("No srt entry found : " + String.Join(" , ", archive.Entries.Select(e => e.Name)));
            var result = new MemoryStream();
            srtEntries.First().Open().CopyTo(result);
            result.Seek(0, SeekOrigin.Begin);
            return result;
        }

        public Stream GetSrtStream(Stream stream)
        {
            if(IsZipFile(stream))
            {
                return GetSrtStreamFromZip(stream);
            }
            else
            {
                return stream;
            }
        }

    }
}
