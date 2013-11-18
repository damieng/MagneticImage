using System;
using System.Collections.Generic;
using System.Linq;

namespace DskTool.LogicalDisk
{
    public class Side
    {
        private readonly List<Track> tracks = new List<Track>();

        public List<Track> Tracks { get { return tracks; } }

        public int Number { get; set; }
    }
}