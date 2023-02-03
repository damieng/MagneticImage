using MagneticImage.LogicalDisk;
using System.IO;
using System.Linq;
using System.Xml;

namespace MagneticImage.Writer
{
    public enum BinaryTextMode
    {
        None,
        BinHex,
        Base64,
        QuotedPrintable,
        AsciiHexSwitch,
    };

    class WriteContext
    {
        public FormattedTrack DefaultTrack { get; set; }
        public Sector DefaultSector { get; set; }
    }

    public class XmlDiskWriter
    {
        private readonly XmlTextWriter xml;
        private readonly BinaryTextMode binaryTextMode;

        public XmlDiskWriter(TextWriter textWriter, BinaryTextMode binaryTextMode)
        {
            xml = new XmlTextWriter(textWriter) { Formatting = Formatting.Indented };
            this.binaryTextMode = binaryTextMode;
        }

        public void Write(DiskImage diskImage)
        {
            xml.WriteStartElement("Image");
            xml.WriteAttributeString("creator", diskImage.Creator);
            xml.WriteAttributeString("fileName", Path.GetFileName(diskImage.FileName));

            var context = CreateContext(diskImage);

            WriteDefaults(context);
            Write(diskImage.Disk, context);

            xml.WriteEndElement();
        }

        private WriteContext CreateContext(DiskImage image)
        {
            var tracks = image.Disk.Sides
                .SelectMany(s => s.Tracks)
                .OfType<FormattedTrack>()
                .ToList();

            var sectors = tracks
                .SelectMany(f => f.Sectors)
                .ToList();

            return new WriteContext
            {
                DefaultTrack = new FormattedTrack
                {
                    Gap = tracks.MostCommon(t => t.Gap),
                    Filler = tracks.MostCommon(t => t.Filler),
                    Size = tracks.MostCommon(t => t.Size)
                },
                DefaultSector = new Sector
                {
                    Size = sectors.MostCommon(s => s.Size)
                }
            };
        }

        private void Write(Disk disk, WriteContext context)
        {
            xml.WriteStartElement("Disk");

            foreach (var side in disk.Sides)
                Write(side, context);

            xml.WriteEndElement();
        }

        private void WriteDefaults(WriteContext context)
        {
            xml.WriteStartElement("Defaults");

            xml.WriteStartElement("Track");
            xml.WriteAttributeString("size", context.DefaultTrack.Size.ToString());
            xml.WriteAttributeString("gap", context.DefaultTrack.Gap.ToString());
            xml.WriteAttributeString("filler", context.DefaultTrack.Filler.ToString());
            xml.WriteEndElement();

            xml.WriteStartElement("Sector");
            xml.WriteAttributeString("size", context.DefaultSector.Size.ToString());
            xml.WriteEndElement();

            xml.WriteEndElement();
        }

        private void Write(Side side, WriteContext context)
        {
            xml.WriteStartElement("Side");
            xml.WriteAttributeString("number", side.Number.ToString());

            foreach (var track in side.Tracks)
                Write(track, context);

            xml.WriteEndElement();
        }

        private void Write(Track track, WriteContext context)
        {
            xml.WriteStartElement("Track");
            xml.WriteAttributeString("number", track.Number.ToString());

            if (track is FormattedTrack)
                Write((FormattedTrack)track, context);

            xml.WriteEndElement();
        }

        private void Write(FormattedTrack track, WriteContext context)
        {
            if (track.Size != context.DefaultTrack.Size)
                xml.WriteAttributeString("size", track.Size.ToString());

            if (track.Gap != context.DefaultTrack.Gap)
                xml.WriteAttributeString("gap", track.Gap.ToString("x"));

            if (track.Filler != context.DefaultTrack.Filler)
                xml.WriteAttributeString("filler", track.Filler.ToString());

            if (track is PD765FormattedTrack)
                Write((PD765FormattedTrack)track, context);

            foreach (var sector in track.Sectors)
                Write(sector, track, context);
        }

        private void Write(PD765FormattedTrack track, WriteContext context)
        {
            xml.WriteAttributeString("sectorSize", track.SectorSize.ToString());
            xml.WriteAttributeString("sectors", track.SectorCount.ToString());

            if (track.DataRate != 0)
                xml.WriteAttributeString("dataRate", track.DataRate.ToString());

            if (track.RecordingMode != 0)
                xml.WriteAttributeString("recordingMode", track.RecordingMode.ToString());
        }

        private void Write(Sector sector, FormattedTrack track, WriteContext context)
        {
            xml.WriteStartElement("Sector");
            xml.WriteAttributeString("id", sector.Id.ToString());

            if (sector.Size != context.DefaultSector.Size)
                xml.WriteAttributeString("size", sector.Size.ToString());

            Write(sector.Data, GetSectorDataLength(sector.Data, track.Filler));

            xml.WriteEndElement();
        }

        private int GetSectorDataLength(byte[] sectorData, byte filler)
        {
            for (var i = sectorData.Length; i > 0; i--)
                if (sectorData[i - 1] != filler)
                    return i;

            return 0;
        }

        private void Write(byte[] binary, int length)
        {
            switch (binaryTextMode)
            {
                case BinaryTextMode.Base64:
                    xml.WriteBase64(binary, 0, length);
                    break;
                case BinaryTextMode.BinHex:
                    xml.WriteBinHex(binary, 0, length);
                    break;
                case BinaryTextMode.QuotedPrintable:
                    xml.WriteString(QuotedPrintable.Encode(binary, 0, length));
                    break;
                case BinaryTextMode.AsciiHexSwitch:
                    xml.WriteString(AsciiHexSwitch.Encode(binary, 0, length));
                    break;
            }
        }
    }
}