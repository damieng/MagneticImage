using System;
using MagneticImage.LogicalDisk;

namespace MagneticImage.ImageFormat
{
    public class StandardDskDiskImage : DiskImage
    {
        private static readonly int[] sectorSizes = new [] { 128, 256, 512, 1024, 2048, 4096, 8192 };

        public override int[] SectorSizes { get { return sectorSizes; } }

        internal static readonly string Signature = "MV - CPC";
    }
}