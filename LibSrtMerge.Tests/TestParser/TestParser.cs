using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LibSrtMerge;
using NUnit.Framework;
using SubtitlesParser.Classes;

namespace Tests
{
    public class TestParser
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ItCanParseAStream()
        {
            var parser = new SubtitlesParser.Classes.Parsers.SrtParser();
            using (var stream = GetResourceStream("sample.srt"))
            {
                var items = parser.ParseStream(stream, Encoding.UTF8);
                foreach (var item in items)
                {
                    Console.WriteLine($"item from {item.StartTime} to {item.EndTime} with {item.Lines.Count} lines");
                    foreach (var line in item.Lines)
                    {
                        Console.WriteLine(line);
                    }
                }
            }
            Assert.Pass();
        }

        [Test]
        public void ItCanWriteAStream()
        {
            var sample = "sample.srt";
            string content = ReadResourceContent(sample);
            var merger = new SrtMerger();
            var items = GetSubtitlesFromResource(merger, sample);
            string output;
            output = GetOutputToStream(merger, items);
            Console.WriteLine(output);
            Assert.That(output, Is.EqualTo(content));
        }

        [Test]
        public void ItCanColorizeAStream()
        {
            var merger = new SrtMerger();
            var items = GetSubtitlesFromResource(merger, "sample.srt");
            merger.Colorize(items, "FFFF00");
            var output = GetOutputToStream(merger, items);
            var expectedOutput = ReadResourceContent("sample2.srt");
            Console.WriteLine(output);
            Assert.That(output, Is.EqualTo(expectedOutput), "wrong output");
        }

        [Test]
        [TestCase("sample3.srt", "sample4.srt", "sample5.srt")]
        [TestCase("sample3.srt", "sample6.srt", "sample7.srt")]
        [TestCase("sample8.srt", "sample4.srt", "sample9.srt")]
        public void ItCanMergeSubtitles(string input1, string input2, string expected)
        {
            var merger = new SrtMerger();
            var subs1 = GetSubtitlesFromResource(merger, input1);
            var subs2 = GetSubtitlesFromResource(merger, input2);

            merger.Colorize(subs2, "FFEA00");

            var result = merger.MergeSubtitles(subs1, subs2);

            var output = GetOutputToStream(merger, result);
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

        private string ReadResourceContent(string sample)
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

        private Stream GetResourceStream(string v)
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            return assembly.GetManifestResourceStream("LibSrtMerge.Tests.TestParser." + v);
        }
    }
}