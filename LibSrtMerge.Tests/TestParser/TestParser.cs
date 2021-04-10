using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FluentAssertions;
using LibSrtMerge;
using LibSrtMerge.Tests;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using SubtitlesParser.Classes;

namespace Tests
{
    public class TestParser
    {
        SrtMerger _sut = new SrtMerger(TestHelper.GetLogger<SrtMerger>());
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        [TestCase("sample.srt", "sample.srt")]
        [TestCase("sample14.ass", "sample15.srt")]
        public void ItCanParseAndWriteAStream(string inputResource, string expectedOutputResource)
        {
            using (var stream = GetResourceStream(inputResource))
            {
                var items = _sut.ParseStream(stream);
                using(var output = new MemoryStream())
                {
                    _sut.WriteStream(output, items);
                    Assert.That(output.ToArray().AsString().WithLF(), Is.EqualTo(GetResourceStream(expectedOutputResource).ReadToEnd().WithLF()));
                }
            }
        }

        [Test]
        public void ItCanWriteAStream()
        {
            var sample = "sample.srt";
            string content = ReadResourceContent(sample);
            var items = GetSubtitlesFromResource(_sut, sample);
            string output;
            output = GetOutputToStream(_sut, items);
            Console.WriteLine(output);
            Assert.That(output, Is.EqualTo(content));
        }

        [Test]
        public void ItCanColorizeAStream()
        {
            var items = GetSubtitlesFromResource(_sut, "sample.srt");
            _sut.Colorize(items, "FFFF00");
            var output = GetOutputToStream(_sut, items);
            var expectedOutput = ReadResourceContent("sample2.srt");
            Console.WriteLine(output);
            Assert.That(output, Is.EqualTo(expectedOutput), "wrong output");
        }

        [Test]
        [TestCase("sample3.srt", "sample4.srt", "sample5.srt")]
        [TestCase("sample3.srt", "sample6.srt", "sample7.srt")]
        [TestCase("sample8.srt", "sample4.srt", "sample9.srt")]
        [TestCase("sample11.srt", "sample12.srt", "sample13.srt")]
        public void ItCanMergeSubtitles(string input1, string input2, string expected)
        {
            var subs1 = GetSubtitlesFromResource(_sut, input1);
            var subs2 = GetSubtitlesFromResource(_sut, input2);

            _sut.Colorize(subs2, "FFEA00");

            var result = _sut.MergeSubtitles(subs1, subs2);

            var output = GetOutputToStream(_sut, result);
            var expectedOutput = ReadResourceContent(expected);
            Assert.That(output, Is.EqualTo(expectedOutput), "wrong output");

        }

        private static string GetOutputToStream(SrtMerger merger, IEnumerable<SubtitleItem> items)
        {
            string output;
            using (var outputStream = new MemoryStream())
            {
                merger.WriteStream(outputStream, items);
                output = Encoding.UTF8.GetString(outputStream.ToArray());
            }

            return output;
        }

        private IEnumerable<SubtitleItem> GetSubtitlesFromResource(SrtMerger merger, string sample)
        {
            using (var stream = GetResourceStream(sample))
            {
                return merger.ParseStream(stream);
            }
        }

        public static string ReadResourceContent(string sample)
        {
            string content;
            using (Stream stream = GetResourceStream(sample))
            {
                using (var reader = new StreamReader(stream))
                {
                    content = reader.ReadToEnd();
                }
            }

            return content;
        }

        public static Stream GetResourceStream(string v)
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            return assembly.GetManifestResourceStream("LibSrtMerge.Tests.TestParser." + v);
        }

        [Test]
        public void ItCanParseSlightlyMalformedFiles()
        {
            //Arrange
            string content = @"
00:00:20,000 -- > 00:00:40,000
some text

1
00:03:36,000 --> 00:03:40,000
some other text
and anotherline
3
00:03:45,727 --> 00:03:48,731
Notice the missing blank line

4
00:03:48,731 --> 00:03:50,732
There should still be 4 subtitles
";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(content.Trim()));

            //Act
            var result = _sut.ParseStream(stream);

            //Assert
            result.Should().HaveCount(4);
        }
    }
}