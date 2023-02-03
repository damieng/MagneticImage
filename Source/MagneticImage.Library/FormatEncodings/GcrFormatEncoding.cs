using System.Collections.Generic;
using System.Linq;

namespace MagneticImage.FormatEncodings
{
    public class GcrFormatEncoding
    {
        private static List<BitPattern> table = new List<BitPattern> {
            new BitPattern("0000", "11001"),
            new BitPattern("1000", "11010"),
            new BitPattern("0001", "11011"),
            new BitPattern("1001", "01001"),
            new BitPattern("0010", "10010"),
            new BitPattern("1010", "01010"),
            new BitPattern("0011", "10011"),
            new BitPattern("1011", "01011"),
            new BitPattern("0100", "11101"),
            new BitPattern("1100", "11110"),
            new BitPattern("0101", "10101"),
            new BitPattern("1101", "01101"),
            new BitPattern("0110", "10110"),
            new BitPattern("1110", "01110"),
            new BitPattern("0111", "10111"),
            new BitPattern("1111", "01111")
        };

        private readonly Dictionary<byte, byte> encodingTable = table.ToDictionary(k => k.Decoded, k => k.Encoded);
        private readonly Dictionary<byte, byte> decodingTable = table.ToDictionary(k => k.Encoded, k => k.Decoded);
        private const int decodedBits = 4;
        private const int encodedBits = 5;
    }
}