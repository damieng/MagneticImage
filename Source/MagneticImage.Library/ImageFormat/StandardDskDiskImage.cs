using MagneticImage.LogicalDisk;

namespace MagneticImage.ImageFormat;

/// <summary>
/// A standard DSK disk image as decoded from a standard "MV - CPC" DSK image file.
/// </summary>
public class StandardDskDiskImage : DiskImage
{
    private static readonly int[] sectorSizes = { 128, 256, 512, 1024, 2048, 4096, 8192 };

    public override int[] SectorSizes
        => sectorSizes;

    internal const string Signature = "MV - CPC";
}