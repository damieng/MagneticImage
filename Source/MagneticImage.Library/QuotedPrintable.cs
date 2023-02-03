using System.Text;

namespace MagneticImage
{
    public static class QuotedPrintable
    {
        public static string Encode(byte[] bytes)
        {
            return Encode(bytes, 0, bytes.Length);
        }

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
        {
            return b < 33 || b > 126 || b == '=';
        }
    }

    public static class AsciiHexSwitch
    {
        public static string Encode(byte[] bytes)
        {
            return Encode(bytes, 0, bytes.Length);
        }

        public static string Encode(byte[] bytes, int offset, int length)
        {
            var binaryMode = false;
            var output = new StringBuilder();

            byte? lastBinary = null;
            int lastCount = 0;

            void flushBinary()
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

            for (var i = offset; i < length; i++)
            {
                var b = bytes[i];
                var newBinaryMode = ShouldBeEncoded(b);
                if (binaryMode != newBinaryMode)
                {
                    flushBinary();
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
                        flushBinary();
                        output.Append(b.ToString("X2"));
                        lastBinary = b;
                    }
                }
                else
                    output.Append((char)b);
            }

            flushBinary();

            return output.ToString();
        }

        private static bool ShouldBeEncoded(byte b)
        {
            return b < 32 || b > 125;
        }
    }
}