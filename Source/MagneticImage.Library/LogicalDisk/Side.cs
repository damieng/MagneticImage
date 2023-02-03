using System.Collections.Generic;

namespace MagneticImage.LogicalDisk
{
    public class Side
    {
        private readonly List<Track> tracks = new List<Track>();

        public List<Track> Tracks { get { return tracks; } }

        public int Number { get; set; }
    }
}