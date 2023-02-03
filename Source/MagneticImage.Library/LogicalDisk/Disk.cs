using System.Collections.Generic;
using System.Linq;

namespace MagneticImage.LogicalDisk
{
    public class Disk
    {
        public int TrackCount { get { return sides.Sum(s => s.Tracks.Count); } }

        private readonly List<Side> sides = new();

        public List<Side> Sides { get { return sides; } }
    }
}