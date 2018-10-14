using System.IO;

namespace LibSrtMerge
{
    public class CommandLineInterface
    {
        private const string _PROG_NAME = "srtmerge";
        public string Color { get; set; } = "FFFF00";

        public TextWriter Out { get; set; } = System.Console.Out;
        public IFileSystem FileSystem { get; set; } = new FileSystem();

        public int Main(string[] args)
        {
            if (args.Length != 2)
            {
                Out.WriteLine("Error : wrong number of arguments: " + args.Length);
                Out.WriteLine("usage : {0} FILE1 FILE2", _PROG_NAME);
                return 1;
            }
            string outputPath = "output.srt";
            var merger = new SrtMerger();
            using (var f1 = FileSystem.File.OpenRead(args[0]))
            {
                var s1 = merger.ParseStream(f1);
                using (var f2 = FileSystem.File.OpenRead(args[1]))
                {
                    var s2 = merger.ParseStream(f2);
                    merger.Colorize(s2, Color);

                    var result = merger.MergeSubtitles(s1, s2);

                    using (var output = FileSystem.File.Open(outputPath, FileMode.Create))
                    {
                        merger.WriteStream(output, result);
                    }
                }
            }
            return 0;
        }
    }
}