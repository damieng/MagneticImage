using System;
using System.IO;
using MagneticImage.ImageFormat;
using MagneticImage.LogicalDisk;
using MagneticImage.LogicalDisk.PD765;

namespace MagneticImage.Reader;

/// <summary>
/// Can identify and read <see cref="StandardDskDiskImage"/> and <see cref="ExtendedDskDiskImage"/>.
/// </summary>
public class DskImageReader
{
    private const string TrackSignature = "Track-Info\r\n\0";

    /// <summary>
    /// Identify which kind of disk image the stream contains.
    /// </summary>
    /// <param name="stream">The <see cref="Stream"/> to be considered.</param>
    /// <returns>The type of disk image the stream contains or <see langword="null"/> if none identified.</returns>
    public static Type Identify(Stream stream)
    {
        stream.Position = 0;
        var readSignature = stream.ReadText(34);

        if (readSignature.StartsWith(StandardDskDiskImage.Signature))
            return typeof(StandardDskDiskImage);

        if (readSignature.StartsWith(ExtendedDskDiskImage.Signature))
            return typeof(ExtendedDskDiskImage);

        return null;
    }

    /// <summary>
    /// Read a DSK disk image from a specified file name.
    /// </summary>
    /// <param name="fileName">The name of the file to read.</param>
    /// <returns>The <see cref="DiskImage"/> if the image was read.</returns>
    /// <exception cref="NotSupportedException">If no DSK image was identified.</exception>
    /// <exception cref="InvalidOperationException">If errors occured attempting to read the DSK image.</exception>
    public static DiskImage Read(string fileName)
    {
        using var input = File.OpenRead(fileName);
        var diskImage = Read(input);
        diskImage.FileName = fileName;
        return diskImage;
    }

    /// <summary>
    /// Read a DSK disk image from a stream.
    /// </summary>
    /// <param name="rs">The <see cref="Stream"/> to read the file from.</param>
    /// <returns>The <see cref="DiskImage"/> if the image was read.</returns>
    /// <exception cref="NotSupportedException">If no DSK image was identified.</exception>
    /// <exception cref="InvalidOperationException">If errors occured attempting to read the DSK image.</exception>
    public static DiskImage Read(Stream rs)
    {
        var type = Identify(rs) ?? throw new NotSupportedException("Unknown disk format.");

        var image = Activator.CreateInstance(type) as DiskImage;
        if (image == null) throw new NotSupportedException($"Unable to create disk image for {type}.");

        rs.Seek(0x22, SeekOrigin.Begin);
        image.Creator = rs.ReadText(14).TrimEnd('\0');

        var trackCount = rs.ReadByte();
        var sideCount = rs.ReadByte();

        Func<int, uint> getTrackSize = null;

        if (type == typeof(StandardDskDiskImage)) {
            var trackSize = (uint)(rs.ReadWord() - 256);
            getTrackSize = i => trackSize;
        }

        if (type == typeof(ExtendedDskDiskImage)) {
            var trackSizes = new uint[trackCount * sideCount];
            rs.Skip(2);
            for (var i = 0; i < trackSizes.Length; i++)
                trackSizes[i] = (uint)(rs.ReadByte() * 256);
            getTrackSize = i => trackSizes[i];
        }

        if (getTrackSize == null) throw new InvalidOperationException();

        rs.Seek(0x100, SeekOrigin.Begin);

        for (var i = 0; i < sideCount; i++)
            image.Disk.Sides.Add(new Side { Number = i });

        for (var i = 0; i < trackCount; i++)
            image.Disk.Sides[i % sideCount].Tracks.Add(ReadTrack(rs, image, getTrackSize(i)));

        return image;
    }

    private static Track ReadTrack(Stream rs, DiskImage image, uint trackSize)
    {
        if (rs.Position == rs.Length)
            return new UnformattedTrack();

        var sectorData = rs.Position + 0x100;

        var trackSig = rs.ReadText(TrackSignature.Length);
        if (!trackSig.Equals(TrackSignature))
            throw new InvalidDataException($"Missing track info '{TrackSignature}' at offset {rs.Position}.");

        rs.Skip(3);

        var track = new PD765FormattedTrack
        {
            Number = (uint)rs.ReadByte(),
            Side = (byte)rs.ReadByte(),
            Size = trackSize,

            DataRate = (byte)rs.ReadByte(),
            RecordingMode = (byte)rs.ReadByte(),
            SectorSize = CalculateSectorSize(image, rs.ReadByte()),
            SectorCount = (byte)rs.ReadByte(),

            Gap = (byte)rs.ReadByte(),
            Filler = (byte)rs.ReadByte()
        };

        for (var i = 0; i < track.SectorCount; i++)
            track.Sectors.Add(ReadSector(rs, image));

        rs.Seek(sectorData, SeekOrigin.Begin);

        foreach (var sector in track.Sectors)
            sector.Data = rs.ReadBlock(sector.Size);

        return track;
    }

    private static Sector ReadSector(Stream rs, DiskImage image)
    {
        var sector = new PD765Sector
        {
            TrackId = rs.ReadByte(),
            SideId = rs.ReadByte(),
            Id = rs.ReadByte(),
            Size = CalculateSectorSize(image, rs.ReadByte()),
            Flag1 = (byte)rs.ReadByte(),
            Flag2 = (byte)rs.ReadByte(),
            DataLength = rs.ReadWord()
        };
        return sector;
    }

    private static int CalculateSectorSize(DiskImage image, int index)
    {
        var sectorSizes = image.SectorSizes;
        var topIndex = sectorSizes.Length - 1;
        return index <= topIndex ? sectorSizes[index] : sectorSizes[topIndex];
    }
}