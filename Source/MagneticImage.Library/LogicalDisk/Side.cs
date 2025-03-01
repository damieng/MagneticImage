using System.Collections.Generic;

namespace MagneticImage.LogicalDisk;

/// <summary>
/// A side of a floppy disk that contains a number of tracks.
/// </summary>
public class Side
{
    /// <summary>
    /// The tracks that belong to this side.
    /// </summary>
    public List<Track> Tracks { get; } = new();

    /// <summary>
    /// The side number of this side (0 or 1).
    /// </summary>
    public int Number { get; set; }
}