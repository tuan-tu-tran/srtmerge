using System;

namespace CAppSrtMerge
{
    class Program
    {
        static void Main(string[] args)
        {
            LibSrtMerge.CommandLineInterface cli = new LibSrtMerge.CommandLineInterface();
            cli.Main(args);
        }
    }
}
