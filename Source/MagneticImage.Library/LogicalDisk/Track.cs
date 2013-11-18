using System;
using System.Collections.Generic;
using System.Linq;

namespace DskTool.LogicalDisk
{
    public class Track
    {
        public uint Number { get; set; }

        public byte Side { get; set; }
    }

    public class FormattedTrack : Track
    {
        public uint Size { get; set; }

        public byte Filler { get; set; }

        public byte Gap { get; set; }

        private readonly List<Sector> sectors = new List<Sector>();

        public List<Sector> Sectors { get { return sectors; } }

        public bool IsBlank(Sector sector)
        {
            return sector.Data.All(d => d == Filler);
        }
    }

    public class PD765FormattedTrack : FormattedTrack
    {
        public byte DataRate { get; set; }

        public byte RecordingMode { get; set; }

        public int SectorSize { get; set; }

        public byte SectorCount { get; set; }
    }

    public class UnformattedTrack : Track
    {
    }
}