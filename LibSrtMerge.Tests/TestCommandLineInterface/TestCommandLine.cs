using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Tests;

namespace LibSrtMerge.Tests.TestCommandLineInterface
{
    [TestFixture]
    public class TestCommandLine
    {
        private CommandLineInterface _sut;
        private List<(string Filename, MemoryStream Stream)> _filesWritten;

        [SetUp]
        public void Init()
        {
            var fsMock = new Mock<IFileSystem>(MockBehavior.Strict);
            fsMock.Setup(fs => fs.File.OpenRead(It.IsAny<string>())).Returns<string>(GetStream);
            _filesWritten = new List<(string Filename, MemoryStream Stream)>();
            fsMock.Setup(fs => fs.File.Open(It.IsAny<string>(), FileMode.Create)).Returns((string fname, FileMode _) =>
            {
                var stream = new MemoryStream();
                _filesWritten.Add((fname, stream));
                return stream;
            });
            _sut = new CommandLineInterface()
            {
                FileSystem = fsMock.Object,
            };
        }
        Stream GetStream(string name) => Geekality.IO.EmbeddedResource.Get<TestCommandLine>(name, true);

        [Test]
        [TestCase("sample3.srt", "sample4.srt", "sample10.srt","sample3.merge.srt")]
        [TestCase("sample1.zip", "sample2.zip", "sample10.srt","sample1.merge.srt")]
        [TestCase("sample3.srt", "sample2.zip", "sample10.srt","sample3.merge.srt")]
        [TestCase("sample1.zip", "sample4.srt", "sample10.srt","sample1.merge.srt")]
        public void ItCanMergeSrtFiles(string input1, string input2, string expectedOutput, string expectedOutputFilename)
        {
            var fsMock = new Mock<IFileSystem>(MockBehavior.Strict);
            fsMock.Setup(fs => fs.File.OpenRead(It.IsAny<string>())).Returns<string>(GetStream);
            var output = new MemoryStream();
            fsMock.Setup(fs => fs.File.Open(expectedOutputFilename, FileMode.Create)).Returns(output);

            var cli = new CommandLineInterface()
            {
                FileSystem = fsMock.Object,
            };

            cli.Main(new string[]{
                input1, input2
            });
            Assert.That(output.ToArray().AsString(), Is.EqualTo(GetStream(expectedOutput).ReadToEnd()));
        }

        [Test]
        public void ItHandlesSpecialItalics()
        {
            //Given an input file that contains special italics markers (see also harry-potter.ass, for a real case, line 24)
            //When I call merge that file with itself 
            //Then the output contains standard italics marker

            Merging_Files_Should_Produce_Expected_Result("sample05.srt", null, "sample07.srt");
        }

        [Test]
        public void ItCorrectsBadNewLines()
        {
            //Given an input file that contains bad new lines \N (see also harry-potter.ass, for a real case, line 24)
            //When I call merge that file with itself 
            //Then the output contains corrected new lines

            Merging_Files_Should_Produce_Expected_Result("sample08.srt", null, "sample09.srt");
        }

        private void Merging_Files_Should_Produce_Expected_Result(string input1, string input2, string expectedOutput)
        {
            //Arrange
            input2= input2 ?? input1; //merge input1 with itself if input2 is null

            //Act
            _sut.Main(new[] { input1, input2 });

            //Assert
            _filesWritten.Should().ContainSingle();
            var output = _filesWritten.First().Stream.ToArray().AsString().WithLF();
            File.WriteAllText("actual.srt", output); //for debugging
            output.Should().Be(GetStream(expectedOutput).ReadToEnd().WithLF());
        }

        [Test]
        public void ItCorrectsMissingNewLines()
        {
            //Given an input file that contains missing new lines between subtitles
            //(see also jacobs-ladder, for a real case, line 389)
            //When I call merge that file with itself 
            //Then the output contains subtitles correctly split out

            Merging_Files_Should_Produce_Expected_Result("sample11.srt", null, "sample12.srt");
        }
    }
}
