using System;

namespace DskTool.LogicalDisk
{
    public abstract class DiskImage
    {
        public string Creator { get; set; }

        public Disk Disk { get; set; }

        public string FileName { get; set; }

        public abstract int[] SectorSizes { get; }

        public DiskImage()
        {
            Disk = new Disk();
        }
    }
}