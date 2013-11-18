using System;
using System.IO;
using DskTool.Readers;
using DskTool.Writer;

namespace DskTool
{
    class Program
    {
        public static void Main(string[] args)
        {
            var arguments = new Arguments(args);
            if (!arguments.IsValid)
                return;

            if (!File.Exists(arguments.InputFile)) {
                Console.Error.WriteLine(String.Format("Input file \"{0}\" not found", arguments.InputFile));
                return;
            }

            var input = File.OpenRead(arguments.InputFile);
            var diskImage = DskDiskImageReader.Load(input);
            diskImage.FileName = arguments.InputFile;

            var output = new XmlDiskWriter(Console.Out, BinaryTextMode.AsciiHexSwitch);
            output.Write(diskImage);
        }
    }

    class Arguments
    {
        private readonly string inputFile;

        public string InputFile { get { return inputFile; } }

        public bool IsValid { get { return true; } }

        public Arguments(string[] args)
        {
            inputFile = args[0];
        }
    }
}
