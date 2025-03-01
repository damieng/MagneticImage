using System;
using System.IO;
using MagneticImage.Reader;
using MagneticImage.Writer;

namespace MagneticImage.Sandbox;

class Program
{
    public static void Main(string[] args)
    {
        var arguments = new Arguments(args);
        if (!arguments.IsValid)
            return;

        if (!File.Exists(arguments.InputFile))
        {
            Console.Error.WriteLine($"Input file \"{arguments.InputFile}\" not found");
            return;
        }

        var diskImage = DskImageReader.Read(arguments.InputFile);

        using var outputXml = File.Create("Testing.xmldisk");
        var writer = new StreamWriter(outputXml);

        var xmlOutput = new XmlDiskWriter(writer, BinaryTextMode.AsciiHexSwitch);
        xmlOutput.Write(diskImage);

        DskImageWriter.Write(diskImage, "testing.dsk");
    }
}