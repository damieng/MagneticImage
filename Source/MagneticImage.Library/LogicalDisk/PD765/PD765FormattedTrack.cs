namespace MagneticImage.LogicalDisk.PD765;

/// <summary>
/// A <see cref="FormattedTrack"/> that is compatible with the NEC uPD765
/// floppy disk controller so contains additional metadata.
/// </summary>
public class PD765FormattedTrack : FormattedTrack
{
    /// <summary>
    /// The data rate of the formatted track.
    /// </summary>
    public byte DataRate { get; set; }

    /// <summary>
    /// The recording mode of the formatted track.
    /// </summary>
    public byte RecordingMode { get; set; }

    /// <summary>
    /// The sector size of the formatted track.
    /// </summary>
    public int SectorSize { get; set; }
}