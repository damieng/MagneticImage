using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MagneticImage
{
    static class Extensions
    {
        public static string ReadText(this Stream inputStream, int length)
        {
            var buffer = new byte[length];
            inputStream.Read(buffer, 0, length);
            return Encoding.ASCII.GetString(buffer);
        }

        public static void WriteText(this Stream outputStream, string text)
        {
            var buffer = Encoding.ASCII.GetBytes(text);
            outputStream.Write(buffer, 0, buffer.Length);
        }

        public static ushort ReadWord(this Stream inputStream)
        {
            return (ushort)(inputStream.ReadByte() + (inputStream.ReadByte() * 256));
        }

        public static void WriteWord(this Stream outputStream, ushort word)
        {
            outputStream.WriteByte((byte)(word % 256));
            outputStream.WriteByte((byte)(word / 256));
        }

        public static void Ignore(this Stream inputStream, int length)
        {
            inputStream.Seek(length, SeekOrigin.Current);
        }

        public static byte[] ReadBlock(this Stream inputStream, int length)
        {
            var result = new byte[length];
            inputStream.Read(result, 0, length);
            return result;
        }

        public static TReturn MostCommon<TReturn, T>(this IEnumerable<T> source, Func<T, TReturn> property)
        {
            return source
                    .GroupBy(s => property(s))
                    .OrderByDescending(g => g.Count())
                    .Select(g => g.Key)
                    .FirstOrDefault();
        }
    }
}