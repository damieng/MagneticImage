using System;

namespace MagneticImage.FormatEncoding;

/// <summary>
/// A pair of corresponding bits for translating between encoded and decoded patterns
/// stored on magnetic media.
/// </summary>
public class BitPattern
{
    /// <summary>
    /// Create a <see cref="BitPattern"/> using <see cref="string"/> representations of the bit patterns.
    /// </summary>
    /// <param name="decoded">The string of binary digits as it will be in memory. </param>
    /// <param name="encoded">The string of binary digits as stored on magnetic media.</param>
    public BitPattern(string decoded, string encoded)
        : this((byte)Convert.ToInt32(decoded, 2), (byte)Convert.ToInt32(encoded, 2))
    {
    }

    /// <summary>
    /// Create a <see cref="BitPattern"/> using <see cref="byte"/> representations of the bit patterns.
    /// </summary>
    /// <param name="decoded">The byte of binary digits as it will be in memory. </param>
    /// <param name="encoded">The byte of binary digits as stored on magnetic media.</param>
    public BitPattern(byte decoded, byte encoded)
    {
        Decoded = decoded;
        Encoded = encoded;
    }

    /// <summary>
    /// The byte of binary digits as it will be in memory.
    /// </summary>
    public byte Decoded { get; }

    /// <summary>
    /// The byte of binary digits as stored on magnetic media.
    /// </summary>
    public byte Encoded { get; }
}