using System;

namespace DskTool.FormatEncodings
{
    public class BitPattern
    {
        private readonly Byte decoded;
        private readonly Byte encoded;

        public BitPattern(String decoded, String encoded)
            : this((Byte)Convert.ToInt32(decoded, 2), (Byte)Convert.ToInt32(encoded, 2))
        {
        }

        public BitPattern(Byte decoded, Byte encoded)
        {
            this.decoded = decoded;
            this.encoded = encoded;
        }

        public Byte Decoded { get { return decoded; } }

        public Byte Encoded { get { return encoded; } }
    }
}