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
    public static string Encode(byte[] bytes, int offset, int length)
    {
        var binaryMode = false;
        var output = new StringBuilder();

        byte? lastBinary = null;
        int lastCount = 0;

        for (var i = offset; i < length; i++)
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