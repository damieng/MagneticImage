using System;
using System.IO;
using System.Linq;
using MagneticImage.ImageFormat;
using MagneticImage.LogicalDisk;
using MagneticImage.LogicalDisk.PD765;

namespace MagneticImage.Writer;

/// <summary>
/// Write a <see cref="DiskImage"/> back to a file.
/// </summary>
public class DskImageWriter
{
    private const string TrackSignature = "Track-Info\r\n\0";

    /// <summary>
    /// Write a <see cref="DiskImage"/> to a file as specified by the file name.
    /// </summary>
    /// <param name="diskImage">The <see cref="DiskImage"/> to write.</param>
    /// <param name="fileName">The name of the file to write the image to.</param>
    public static void Write(DiskImage diskImage, string fileName)
    {
        var output = File.OpenWrite(fileName);
        Write(diskImage, output);
    }

    /// <summary>
    /// Write a <see cref="DiskImage"/> to a <see cref="Stream"/>.
    /// </summary>
    /// <param name="diskImage">The <see cref="DiskImage"/> to write.</param>
    /// <param name="rs">The output <see cref="Stream"/> to write the image to.</param>
    public static void Write(DiskImage diskImage, Stream rs)
    {
        rs.Position = 0;
        rs.WriteText(diskImage is StandardDskDiskImage
            ? StandardDskDiskImage.Signature
            : ExtendedDskDiskImage.Signature);

        rs.Position = 0x22;
        rs.WriteText(diskImage.Creator + '\0');

        rs.Position = 0x30;
        rs.WriteByte((byte)diskImage.Disk.TrackCount);
        rs.WriteByte((byte)diskImage.Disk.Sides.Count);

        var allTracks = diskImage.Disk.Sides.SelectMany(s => s.Tracks).ToArray();

        if (diskImage is StandardDskDiskImage)
        {
            var maxTrackSize = allTracks.OfType<FormattedTrack>().Max(t => t.Size);
            rs.WriteWord((ushort)maxTrackSize);
        }

        if (diskImage is ExtendedDskDiskImage)
        {
            rs.Position += 2;
            foreach (var track in allTracks.OrderBy(t => t.Number).ThenBy(t => t.Side))
                if (track is FormattedTrack formattedTrack)
                {
                    var trackSize = formattedTrack.Size;
                    rs.WriteByte((byte)(trackSize / 256));
                }
                else
                    rs.WriteByte(0);
        }

        rs.Position = 0x100;

        foreach (var track in allTracks)
            WriteTrack(rs, track, diskImage);
    }

    private static void WriteTrack(Stream s, Track track, DiskImage diskImage)
    {
        var sectorDataPosition = s.Position + 0x100;

        s.WriteText(TrackSignature);
        s.Skip(3);

        s.WriteByte((byte)track.Number);
        s.WriteByte(track.Side);

        if (track is FormattedTrack formattedTrack)
            WriteFormattedTrack(s, formattedTrack, sectorDataPosition, diskImage);
    }

    private static byte GetSectorSize(int sectorSize, DiskImage diskImage)
    {
        for (byte i = 0; i < diskImage.SectorSizes.Length; i++)
        {
            if (diskImage.SectorSizes[i] >= sectorSize)
                return i;
        }

        throw new InvalidOperationException("Sector size could not be determined.");
    }

    private static void WriteFormattedTrack(Stream s, FormattedTrack track, long sectorDataPosition, DiskImage diskImage)
    {
        if (track is PD765FormattedTrack pdTrack)
        {
            s.WriteByte(pdTrack.DataRate);
            s.WriteByte(pdTrack.RecordingMode);
            s.WriteByte(GetSectorSize(pdTrack.SectorSize, diskImage));
        }
        else
        {
            s.WriteByte(0);
            s.WriteByte(0);
            s.WriteByte(0);
        }

        s.WriteByte((byte)track.Sectors.Count);
        s.WriteByte(track.Gap);
        s.WriteByte(track.Filler);

        foreach (var sector in track.Sectors)
            WriteSectorHeader(s, sector);

        s.Position = sectorDataPosition;

        foreach (var sector in track.Sectors)
            s.Write(sector.Data, 0, sector.Data.Length);
    }

    private static void WriteSectorHeader(Stream rs, Sector sector)
    {
        rs.WriteByte((byte)sector.TrackId);
        rs.WriteByte((byte)sector.SideId);
        rs.WriteByte((byte)sector.Id);
        rs.WriteByte((byte)(sector.Size / 256));

        if (sector is PD765Sector pdSector)
        {
            rs.WriteByte(pdSector.Flag1);
            rs.WriteByte(pdSector.Flag2);
            rs.WriteWord(pdSector.DataLength);
        }
    }
}