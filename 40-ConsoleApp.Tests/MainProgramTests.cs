using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using srtmerge.Output.Internals;
using srtmerge.Parsing.Internals;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace srtmerge.ConsoleApp.Tests
{
    [TestClass]
    public class MainProgramTests
    {
        private List<(string Filename, MemoryStream Stream)> _filesWritten;
        private MainProgram _sut;

        [TestInitialize]
        public void Init()
        {
            Mock<IFileReader> fileReaderMock = new Mock<IFileReader>(MockBehavior.Strict);
            Mock<IFileWriter> fileWriterMock = new Mock<IFileWriter>(MockBehavior.Strict);

            var services = MainProgram.ConfigureServices();
            services.AddSingleton(fileReaderMock.Object);
            services.AddSingleton(fileWriterMock.Object);

            fileReaderMock.Setup(fs => fs.OpenRead(It.IsAny<string>())).Returns<string>(GetSample);

            _filesWritten = new List<(string Filename, MemoryStream Stream)>();
            fileWriterMock
                .Setup(fs => fs.OpenWrite(It.IsAny<string>()))
                .Returns((string fname) =>
                {
                    var stream = new MemoryStream();
                    _filesWritten.Add((fname, stream));
                    return stream;
                });

            var servicesProvider = services.BuildServiceProvider();
            _sut = servicesProvider.GetRequiredService<MainProgram>();
        }

        Stream GetSample(string name)
        {
            var type = this.GetType();
            name = Path.GetFileName(name);
            return type.Assembly.GetManifestResourceStream(type.Namespace + ".samples." + name)
                ?? throw new InvalidOperationException("could not get stream " + name);
        }

        [TestMethod]
        [DataRow("sample01.zip", "sample02.zip", "sample10.srt")]
        [DataRow("sample02.zip", "sample01.zip", "sample13.srt")]
        [DataRow("sample03.srt", "sample04.srt", "sample10.srt")]
        [DataRow("sample04.srt", "sample03.srt", "sample13.srt")]
        [DataRow("sample02.zip", "sample03.srt", "sample13.srt")]
        [DataRow("sample03.srt", "sample02.zip", "sample10.srt")]
        [DataRow("sample01.zip", "sample04.srt", "sample10.srt")]
        [DataRow("sample04.srt", "sample01.zip", "sample13.srt")]
        public void ItCanMergeFilesCorrectly(string input1, string input2, string expectedOutput)
        {
            Merging_Files_Should_Produce_Expected_Result(input1, input2, expectedOutput);
        }

        public void Merging_Files_Should_Produce_Expected_Result(string input1, string input2, string expectedOutput)
        {
            //Arrange
            input2 ??= input1;

            //Act
            _sut.Run(new[] { input1, input2 });

            //Assert
            _filesWritten.Should().ContainSingle();
            var output = _filesWritten.First().Stream.ToArray().AsString().WithLF();
            File.WriteAllText("actual.srt", output); //for debugging
            output.Should().Be(GetSample(expectedOutput).ReadToEnd().WithLF());
        }

        [TestMethod]
        public void ItHandlesSpecialItalics()
        {
            //Given an input file that contains special italics markers (see also harry-potter.ass, for a real case, line 24)
            //When I call merge that file with itself 
            //Then the output contains standard italics marker

            Merging_Files_Should_Produce_Expected_Result("sample05.srt", null, "sample07.srt");
        }

        [TestMethod]
        public void ItCorrectsBadNewLines()
        {
            //Given an input file that contains bad new lines \N (see also harry-potter.ass, for a real case, line 24)
            //When I call merge that file with itself 
            //Then the output contains corrected new lines

            Merging_Files_Should_Produce_Expected_Result("sample08.srt", null, "sample09.srt");
        }

        [TestMethod]
        public void ItCorrectsMissingNewLines()
        {
            //Given an input file that contains missing new lines between subtitles
            //(see also jacobs-ladder, for a real case, line 389)
            //When I call merge that file with itself 
            //Then the output contains subtitles correctly split out

            Merging_Files_Should_Produce_Expected_Result("sample11.srt", null, "sample12.srt");
        }

        [TestMethod]
        public void ItProducesTheCorrectOutputPath()
        {
            //Given an input file in one folder
            //And another input file in another folder
            //When I merge the files
            //Then the output file is produced is the same folder as the first input file
            //And the output filename is an srt file with the same name as the first input file, but with the suffix .merge

            //Act
            _sut.Run(new[] { @"some\path1\sample01.zip", @"some-other-path\sample02.zip" });

            //Assert
            _filesWritten.Should().ContainSingle().Which.Filename.Should().Be(@"some\path1\sample01.merge.srt");
        }

        [TestMethod]
        public void When_MultipleOutputFiles_Then_FilenamesAreCorrect()
        {
            //Given an input file in one folder (in one language)
            //And other input files in other folders (in another language)
            //When I merge the files
            //Then the output files are produced is the same folder as the first input file
            //And the output filenames are srt files with the same name as the first input file, but with the suffix .merge
            //And the merge suffix have the correct offsets

            //Act
            _sut.Run(new[] {
                @"some\path1\sample03.srt",
                @"some-other-path\sample04.srt",
                @"yet-another-path\sample05.srt",
            });

            //Assert
            _filesWritten.Select(f => f.Filename).Should().BeEquivalentTo(new[]{
                @"some\path1\sample03.merge-1.srt",
                @"some\path1\sample03.merge-2.srt",
            });
        }
    }
}
