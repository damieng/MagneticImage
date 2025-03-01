namespace MagneticImage.LogicalDisk;

/// <summary>
/// A sector from a formatted track that may or may not contain data.
/// </summary>
public class Sector
{
    /// <summary>
    /// The track id this sector believes it belongs to.
    /// </summary>
    public int TrackId { get; set; }

    /// <summary>
    /// The side id this sector believes it belongs to.
    /// </summary>
    public int SideId { get; set; }

    /// <summary>
    /// The ID of this sector.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The size (in bytes) of this sector.
    /// </summary>
    public int Size { get; set; }

    /// <summary>
    /// The data bytes belonging to this sector.
    /// </summary>
    public byte[] Data { get; set; }

    /// <summary>
    /// Whether this sector is formatted or not.
    /// </summary>
    public bool IsFormatted
        => Data.Length > 0;
}