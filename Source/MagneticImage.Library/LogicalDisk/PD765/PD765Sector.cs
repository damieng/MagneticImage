namespace MagneticImage.LogicalDisk.PD765;

/// <summary>
/// A <see cref="Sector"/> that is compatible with the NEC uPD765
/// floppy disk controller so contains additional metadata.
/// </summary>
public class PD765Sector : Sector
{
    /// <summary>
    /// The floppy disk controller flag byte #1.
    /// </summary>
    public byte Flag1 { get; set; }

    /// <summary>
    /// The floppy disk controller flag byte #2.
    /// </summary>
    public byte Flag2 { get; set; }

    /// <summary>
    /// The data length as stored in the header.
    /// </summary>
    public ushort DataLength { get; set; }
}