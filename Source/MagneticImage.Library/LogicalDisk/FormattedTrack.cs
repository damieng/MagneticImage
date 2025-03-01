using System.Collections.Generic;
using System.Linq;

namespace MagneticImage.LogicalDisk;

/// <summary>
/// A track that has been formatted so contains sectors.
/// </summary>
public class FormattedTrack : Track
{
    /// <summary>
    /// The size of this track according to the track.
    /// </summary>
    public uint Size { get; set; }

    /// <summary>
    /// The filler/blank byte this track uses.
    /// </summary>
    public byte Filler { get; set; }

    /// <summary>
    /// The gap size between sectors on this track.
    /// </summary>
    public byte Gap { get; set; }

    /// <summary>
    /// The sectors contained on this track.
    /// </summary>
    public List<Sector> Sectors { get; } = new();

    private int? sectorCount;

    /// <summary>
    /// The sector count as determined by the sectors list
    /// unless overridden.
    /// </summary>
    public int? SectorCount
    {
        get => sectorCount ?? Sectors.Count;
        set => sectorCount = value;
    }

    /// <summary>
    /// Whether a given sector is considered blank because it
    /// consists of nothing but the track filler.
    /// </summary>
    /// <param name="sector">The <see cref="Sector"/> to consider.</param>
    /// <returns>
    ///     <see langword="true"/> if the sector is blank,
    ///     <see langword="false"/> if it contains data.
    /// </returns>
    public bool IsBlank(Sector sector)
    {
        return sector.Data.All(d => d == Filler);
    }
}