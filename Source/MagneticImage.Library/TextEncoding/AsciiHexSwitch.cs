using System;
using System.Text;

namespace MagneticImage.TextEncoding;

/// <summary>
/// A text encoding schema that automatically switching between
/// ASCII and Hex to enable efficiency and readability of strings
/// within the source stream while still preserving non-ASCII
/// values.
/// </summary>
public static class AsciiHexSwitch
{
    /// <summary>
    /// Write the sequence of bytes into a string using the AsciiHexSwitch rules.
    /// </summary>
    /// <param name="bytes">The array of bytes to encode.</param>
    /// <param name="offset">The offset into the array to start at.</param>
    /// <param name="length">How many bytes to write.</param>
    /// <returns>A string containing the AsciiHexSwitch representation of the specified byte array.</returns>
    /// <exception cref="ArgumentOutOfRangeException">If the offset + length is beyond the byte array size or if offset or length are less than zero.</exception>
    public static string Encode(byte[] bytes, int offset, int length)
    {
        if (offset + length > bytes.Length)
            throw new ArgumentOutOfRangeException($"Offset + length is {offset + length} which is beyond the byte array length of {bytes.Length}.");
        if (offset < 0)
            throw new ArgumentOutOfRangeException(nameof(offset), offset, "Must not be less than zero.");
        if (length < 0)
            throw new ArgumentOutOfRangeException(nameof(length), length, "Must not be less than zero.");

        var binaryMode = false;
        var output = new StringBuilder();

        byte? lastBinary = null;
        int lastCount = 0;

        for (var i = offset; i < offset + length; i++)
        {
            var b = bytes[i];
            var newBinaryMode = ShouldBeEncoded(b);
            if (binaryMode != newBinaryMode)
            {
                FlushBinary();
                output.Append('~');
                binaryMode = newBinaryMode;
            }

            if (binaryMode)
            {
                if (lastBinary == b)
                {
                    lastCount++;
                }
                else
                {
                    FlushBinary();
                    output.Append(b.ToString("X2"));
                    lastBinary = b;
                }
            }
            else
                output.Append((char)b);
        }

        FlushBinary();

        return output.ToString();

        void FlushBinary()
        {
            if (binaryMode)
            {
                if (lastCount == 2)
                    output.Append(lastBinary.Value.ToString("X2"));
                if (lastCount > 2)
                    output.Append("x" + lastCount + ";");
            }
            lastCount = 1;
            lastBinary = null;
        }
    }

    private static bool ShouldBeEncoded(byte b)
        => b is < 32 or > 125;
}