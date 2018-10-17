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
        [TestCase("sample3.srt", "sample4.srt", "sample10.srt")]
        [TestCase("sample1.zip", "sample2.zip", "sample10.srt")]
        [TestCase("sample3.srt", "sample2.zip", "sample10.srt")]
        [TestCase("sample1.zip", "sample4.srt", "sample10.srt")]
        public void ItCanMergeSrtFiles(string input1, string input2, string expectedOutput)
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
                input1, input2
            });
            Assert.That(output.ToArray(), Is.EqualTo(GetStream(expectedOutput).ReadBytes()));
        }
    }
}
