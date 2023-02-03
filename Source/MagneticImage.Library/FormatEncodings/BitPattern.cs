using System;

namespace MagneticImage.FormatEncodings
{
    public class BitPattern
    {
        private readonly byte decoded;
        private readonly byte encoded;

        public BitPattern(string decoded, string encoded)
            : this((byte)Convert.ToInt32(decoded, 2), (byte)Convert.ToInt32(encoded, 2))
        {
        }

        public BitPattern(byte decoded, byte encoded)
        {
            this.decoded = decoded;
            this.encoded = encoded;
        }

        public byte Decoded { get { return decoded; } }
        public byte Encoded { get { return encoded; } }
    }
}