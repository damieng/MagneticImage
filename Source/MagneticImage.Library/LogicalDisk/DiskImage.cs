namespace MagneticImage.LogicalDisk;

/// <summary>
/// Logical representation of a disk image.
/// </summary>
public abstract class DiskImage
{
    /// <summary>
    /// Name of the tool that created this disk image.
    /// </summary>
    public string Creator { get; set; }

    /// <summary>
    /// The disk contained within this image.
    /// </summary>
    public Disk Disk { get; set; } = new();

    /// <summary>
    /// The original file name of this disk image.
    /// </summary>
    public string FileName { get; set; }

    /// <summary>
    /// The sector sizes this disk image uses.
    /// </summary>
    public abstract int[] SectorSizes { get; }
}