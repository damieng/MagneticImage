using System;
using System.Collections.Generic;
using System.Linq;

namespace DskTool.LogicalDisk
{
    public class Disk
    {
        public int TrackCount { get { return sides.Sum(s => s.Tracks.Count()); } }

        private readonly List<Side> sides = new List<Side>();

        public List<Side> Sides { get { return sides; } }
    }
}