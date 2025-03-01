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
    public static string Encode(byte[] bytes, int offset, int length)
    {
        var output = new StringBuilder();
        for (var i = offset; i < length; i++)
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
        => b < 33 || b > 126 || b == '=';
}