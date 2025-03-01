namespace MagneticImage.LogicalDisk;

/// <summary>
/// A track as stored on a floppy disk that might be unformatted.
/// </summary>
public abstract class Track
{
    /// <summary>
    /// The track number as specified by the track.
    /// </summary>
    public uint Number { get; set; }

    /// <summary>
    /// The side this track belongs to as specified by the track.
    /// </summary>
    public byte Side { get; set; }
}