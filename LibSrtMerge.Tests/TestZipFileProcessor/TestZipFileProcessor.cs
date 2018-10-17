using System;
using System.IO;
using NUnit.Framework;

namespace LibSrtMerge.Tests.TestZipFileProcessor
{
    [TestFixture]
    public class TestZipFileProcessor
    {
        Stream GetStream(string name) => Geekality.IO.EmbeddedResource.Get<TestZipFileProcessor>(name, true);

        [Test]
        [TestCase("sample1.zip", true)]
        [TestCase("sample2.srt", false)]
        public void ItCanIdentifyZipFile(string name, bool isZipFile)
        {
            var stream = Geekality.IO.EmbeddedResource.Get<TestZipFileProcessor>(name, true);
            var processor = new ZipFileProcessor();
            Assert.That(processor.IsZipFile(stream), Is.EqualTo(isZipFile));
        }

        [Test]
        public void ItCanExtractTheSrtFile()
        {
            var archive = GetStream("sample1.zip");
            var processor = new ZipFileProcessor();

            var result = processor.GetSrtStreamFromZip(archive).ReadBytes();
            var expected = GetStream("sample3.srt").ReadBytes();

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        [TestCase("sample4.zip")]
        public void ItFailsIfNoUniqueSrtFile(string archiveFile)
        {
            var archive = GetStream(archiveFile);
            var processor = new ZipFileProcessor();

            Assert.That(() => processor.GetSrtStreamFromZip(archive), Throws.Exception);

        }

    }
}
