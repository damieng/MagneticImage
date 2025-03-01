using MagneticImage.LogicalDisk;

namespace MagneticImage.ImageFormat;

public enum Density
{
    Unknown,
    SingleOrDouble,
    High,
    Extended
}

public enum RecordingMode
{
    Unknown,
    FM,
    MFM
}

/// <summary>
/// An extended DSK disk image as decoded from a "EXTENDED CPC DSK File" DSK image file.
/// </summary>
public class ExtendedDskDiskImage : DiskImage
{
    private static readonly int[] sectorSizes = { 128, 256, 512, 1024, 2048, 4096, 8192, 16384, 32768 };

    public override int[] SectorSizes
        => sectorSizes;

    public Density Density { get; set; }

    public RecordingMode RecordingMode { get; set; }

    internal const string Signature = "EXTENDED CPC DSK File\r\nDisk-Info\r\n";
}