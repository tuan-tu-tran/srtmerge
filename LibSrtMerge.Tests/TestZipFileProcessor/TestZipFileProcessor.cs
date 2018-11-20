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
            Assert.That(stream.Position, Is.EqualTo(0));
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
        [TestCase("sample5.zip")]
        public void ItFailsIfNoUniqueSrtFile(string archiveFile)
        {
            var archive = GetStream(archiveFile);
            var processor = new ZipFileProcessor();

            try
            {
                processor.GetSrtStreamFromZip(archive);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Caught exception: " + ex.Message);
                Assert.Pass();
            }
            Assert.Fail("no exception was thrown");
        }

        [Test]
        [TestCase("sample1.zip", "sample3.srt")]
        [TestCase("sample3.srt", "sample3.srt")]
        [TestCase("sample6.zip", "sample7.ass")]
        public void ItCanGetTheRightStreamForAnyKindOfInput(string inputStream, string expectedStream)
        {
            var content = new ZipFileProcessor().GetSrtStream(GetStream(inputStream)).ReadBytes();
            var expectedContent = GetStream(expectedStream).ReadBytes();

            Assert.That(content, Is.EqualTo(expectedContent));

        }

    }
}
