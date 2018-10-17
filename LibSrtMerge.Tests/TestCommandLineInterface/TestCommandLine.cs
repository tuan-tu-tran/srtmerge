using System;
using System.IO;
using System.Text;
using Moq;
using NUnit.Framework;
using Tests;

namespace LibSrtMerge.Tests.TestCommandLineInterface
{
    [TestFixture]
    public class TestCommandLine
    {
        Stream GetStream(string name) => Geekality.IO.EmbeddedResource.Get<TestCommandLine>(name, true);
        [Test]
        public void ItCanMergeSrtFiles()
        {
            var fsMock = new Mock<IFileSystem>(MockBehavior.Strict);
            fsMock.Setup(fs => fs.File.OpenRead(It.IsAny<string>())).Returns<string>(GetStream);
            var output = new MemoryStream();
            fsMock.Setup(fs => fs.File.Open("output.srt", FileMode.Create)).Returns(output);

            var cli = new CommandLineInterface()
            {
                FileSystem = fsMock.Object,
            };

            cli.Main(new string[]{
                "sample3.srt", "sample4.srt"
            });
            Assert.That(output.ToArray(), Is.EqualTo(GetStream("sample10.srt").ReadBytes()));
        }
    }
}