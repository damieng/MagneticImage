using System.Collections.Generic;
using System.Linq;

namespace MagneticImage.LogicalDisk;

/// <summary>
/// A logical representation of a floppy disk.
/// </summary>
public class Disk
{
    /// <summary>
    /// Total number of tracks in this disk.
    /// </summary>
    public int TrackCount { get { return Sides.Sum(s => s.Tracks.Count); } }

    /// <summary>
    /// The sides that make up this disk - 0 or 1.
    /// </summary>
    public List<Side> Sides { get; } = new();
}