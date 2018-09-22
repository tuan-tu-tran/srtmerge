using System;
using System.IO;
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
            Console.WriteLine(String.Join(",", assembly.GetManifestResourceNames()));
            using (var stream = assembly.GetManifestResourceStream("LibSrtMerge.Tests.TestParser.sample.srt"))
            {
                using(var reader = new StreamReader(stream))
                {
                    Console.WriteLine(reader.ReadToEnd());
                }
                
            }
            // parser.ParseStream();
            Assert.Pass();
        }
    }
}