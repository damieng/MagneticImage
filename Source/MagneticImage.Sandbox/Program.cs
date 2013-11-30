using System;
using System.IO;
using MagneticImage.Reader;
using MagneticImage.Writer;

namespace MagneticImage.Sandbox
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

            var diskImage = DskImageReader.Read(arguments.InputFile);

            var xmlOutput = new XmlDiskWriter(Console.Out, BinaryTextMode.AsciiHexSwitch);
            xmlOutput.Write(diskImage);

            DskImageWriter.Write(diskImage, "testing.dsk");
        }
    }
}