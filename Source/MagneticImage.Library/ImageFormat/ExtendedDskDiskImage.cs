using System;
using MagneticImage.LogicalDisk;

namespace MagneticImage.ImageFormat
{
    public enum Density
    {
        Unknown,
        SingleOrDouble,
        High,
        Extended
    };

    public enum RecordingMode
    {
        Unknown,
        FM,
        MFM
    };

    public class ExtendedDskDiskImage : DiskImage
    {
        private static readonly int[] sectorSizes = new [] { 128, 256, 512, 1024, 2048, 4096, 8192, 16384, 32768 };

        public override int[] SectorSizes { get { return sectorSizes; } }

        public Density Density { get; set; }

        public RecordingMode RecordingMode { get; set; }

        internal static readonly string Signature = "EXTENDED CPC DSK File\r\nDisk-Info\r\n";
    }
}