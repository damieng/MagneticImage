using System;
using System.Text;

namespace MagneticImage.TextEncoding;

/// <summary>
/// Write out a series of bytes using the quoted printable mechanism. This
/// outputs text for values in the standard ASCII range except for "="
/// which is reserved for outputting non-ASCII values.
/// </summary>
public static class QuotedPrintable
{
    /// <summary>
    /// Write the sequence of bytes into a string using the quoted printable rules.
    /// </summary>
    /// <param name="bytes">The array of bytes to encode.</param>
    /// <param name="offset">The offset into the array to start at.</param>
    /// <param name="length">How many bytes to write.</param>
    /// <returns>A string containing the quoted-printable representation of the specified byte array.</returns>
    /// <exception cref="ArgumentOutOfRangeException">If the offset + length is beyond the byte array size or if offset or length are less than zero.</exception>
    public static string Encode(byte[] bytes, int offset, int length)
    {
        if (offset + length > bytes.Length)
            throw new ArgumentOutOfRangeException($"Offset + length is {offset + length} which is beyond the byte array length of {bytes.Length}.");
        if (offset < 0)
            throw new ArgumentOutOfRangeException(nameof(offset), offset, "Must not be less than zero.");
        if (length < 0)
            throw new ArgumentOutOfRangeException(nameof(length), length, "Must not be less than zero.");

        var output = new StringBuilder();
        for (var i = offset; i < offset + length; i++)
        {
            var b = bytes[i];
            if (!ShouldBeEncoded(b))
                output.Append((char)b);
            else
                output.Append("=" + b.ToString("X2"));
        }

        return output.ToString();
    }

    private static bool ShouldBeEncoded(byte b)
        => b < 32 || b > 126 || b == '=';
}