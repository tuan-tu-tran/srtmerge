using System;
using NUnit.Framework;

namespace LibSrtMerge.Tests.TestZipFileProcessor
{
    [TestFixture]
    public class TestZipFileProcessor
    {
        [Test]
        public void ItCanIdentifyZipFile()
        {
            var stream = Geekality.IO.EmbeddedResource.Get<TestZipFileProcessor>("sample1.zip", true);
            var processor = new ZipFileProcessor();
            Assert.That(processor.IsZipFile(stream), Is.True);
        }

    }
}