using System;
using System.IO;
using System.Text;
using NUnit.Framework;

namespace Tests
{
    public class TestParser
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            var parser = new SubtitlesParser.Classes.Parsers.SrtParser();
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream("LibSrtMerge.Tests.TestParser.sample.srt"))
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
    }
}