using System;
using System.Collections.Generic;
using System.IO;
using DskTool.ImageFormat;
using DskTool.LogicalDisk;

namespace DskTool.Readers
{
    public class DskDiskImageReader
    {
        private const string standardImageSignature = "MV - CPC";
        private const string extendedImageSignature = "EXTENDED CPC DSK File\r\nDisk-Info\r\n";
        private const string trackSignature = "Track-Info\r\n\0";

        public static Type Identify(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            var readSignature = stream.ReadText(34);

            if (readSignature.StartsWith(standardImageSignature))
                return typeof(StandardDskDiskImage);

            if (readSignature.StartsWith(extendedImageSignature))
                return typeof(ExtendedDskDiskImage);

            return null;
        }

        public static DiskImage Load(Stream rs)
        {
            var type = Identify(rs);

            if (type == null)
                throw new InvalidOperationException();

            rs.Seek(0x22, SeekOrigin.Begin);

            var image = Activator.CreateInstance(type) as DiskImage;
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
                rs.Ignore(2);
                for (var i = 0; i < trackSizes.Length; i++)
                    trackSizes[i] = (uint)(rs.ReadByte() * 256);
                getTrackSize = i => trackSizes[i];
            }

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

            var trackSig = rs.ReadText(trackSignature.Length);
            if (!trackSig.Equals(trackSignature))
                throw new InvalidDataException(String.Format("Missing track info at offset {0}", rs.Position));

            rs.Ignore(3);

            var track = new PD765FormattedTrack();
            track.Number = (uint)rs.ReadByte();
            track.Side = (byte)rs.ReadByte();
            track.Size = trackSize;

            track.DataRate = (byte)rs.ReadByte();
            track.RecordingMode = (byte)rs.ReadByte();
            track.SectorSize = CalculateSectorSize(image, rs.ReadByte());
            track.SectorCount = (byte)rs.ReadByte();

            track.Gap = (byte)rs.ReadByte();
            track.Filler = (byte)rs.ReadByte();

            for (var i = 0; i < track.SectorCount; i++)
                track.Sectors.Add(ReadSector(rs, image));

            rs.Seek(sectorData, SeekOrigin.Begin);

            foreach (var sector in track.Sectors)
                sector.Data = rs.ReadBlock(sector.Size);

            return track;
        }

        private static Sector ReadSector(Stream rs, DiskImage image)
        {
            var sector = new PD765Sector();
            sector.TrackId = rs.ReadByte();
            sector.SideId = rs.ReadByte();
            sector.Id = rs.ReadByte();
            sector.Size = CalculateSectorSize(image, rs.ReadByte());
            sector.Flag1 = (byte)rs.ReadByte();
            sector.Flag2 = (byte)rs.ReadByte();
            sector.DataLength = rs.ReadWord();
            return sector;
        }

        public static int CalculateSectorSize(DiskImage image, int index)
        {
            var sectorSizes = image.SectorSizes;
            var topIndex = sectorSizes.Length - 1;
            return (index <= topIndex) ? sectorSizes[index] : sectorSizes[topIndex];
        }
    }
}