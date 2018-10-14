using System;
using System.IO;
using System.Text;
using Moq;
using NUnit.Framework;

namespace LibSrtMerge
{
    [TestFixture]
    public class TestCommandLine
    {
        [Test]
        public void ItCanMergeSrtFiles()
        {
            var fsMock = new Mock<IFileSystem>(MockBehavior.Strict);
            fsMock.Setup(fs => fs.File.OpenRead(It.IsAny<string>())).Returns<string>(Tests.TestParser.GetResourceStream);
            var output = new MemoryStream();
            fsMock.Setup(fs => fs.File.Open("output.srt", FileMode.Create)).Returns(output);

            var cli = new CommandLineInterface()
            {
                FileSystem = fsMock.Object,
            };

            cli.Main(new string[]{
                "sample3.srt", "sample4.srt"
            });
            Assert.That(Encoding.UTF8.GetString(output.ToArray()), Is.EqualTo(Tests.TestParser.ReadResourceContent("sample10.srt")));
        }
    }
}