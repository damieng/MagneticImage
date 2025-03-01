using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MagneticImage;

/// <summary>
/// Various helpers to make dealing with streams etc. shorter and more concise.
/// </summary>
static class Extensions
{
    /// <summary>
    /// Read ASCII text from the stream.
    /// </summary>
    /// <param name="inputStream">The <see cref="Stream"/> to read the text from.</param>
    /// <param name="length">The length of the text to read.</param>
    /// <returns>A string containing the text read from the stream.</returns>
    /// <exception cref="InvalidOperationException">If there are not enough bytes left in the stream to read the length specified.</exception>
    public static string ReadText(this Stream inputStream, int length)
    {
        var buffer = new byte[length];
        var readCount = inputStream.Read(buffer, 0, length);
        if (readCount != length)
            throw new InvalidOperationException($"Could not read {length} bytes, only {readCount} bytes were available.");

        return Encoding.ASCII.GetString(buffer);
    }

    /// <summary>
    /// Write the supplied text into the output stream using ASCII encoding.
    /// </summary>
    /// <param name="outputStream">The <see cref="Stream"/> to write into.</param>
    /// <param name="text">The text string to write into the stream.</param>
    public static void WriteText(this Stream outputStream, string text)
    {
        var buffer = Encoding.ASCII.GetBytes(text);
        outputStream.Write(buffer, 0, buffer.Length);
    }

    /// <summary>
    /// Read a 16-bit value from the stream using little-endianness.
    /// </summary>
    /// <param name="inputStream">The <see cref="Stream"/> to read the word from.</param>
    /// <returns>The 16-bit value read from the stream.</returns>
    public static ushort ReadWord(this Stream inputStream)
        => (ushort)(inputStream.ReadByte() + inputStream.ReadByte() * 256);

    /// <summary>
    /// Write a 16-bit value into a stream with little-endianness.
    /// </summary>
    /// <param name="outputStream">The <see cref="Stream"/> to write the word to.</param>
    /// <param name="word">The 16-bit value to write to the stream.</param>
    public static void WriteWord(this Stream outputStream, ushort word)
    {
        outputStream.WriteByte((byte)(word % 256));
        outputStream.WriteByte((byte)(word / 256));
    }

    /// <summary>
    /// Skip over a given number of bytes in a stream.
    /// </summary>
    /// <param name="inputStream">The <see cref="Stream"/> to skip.</param>
    /// <param name="byteCount">The number of bytes to skip.</param>
    public static void Skip(this Stream inputStream, int byteCount)
        => inputStream.Seek(byteCount, SeekOrigin.Current);

    /// <summary>
    /// Read a byte array from an input <see cref="Stream"/>.
    /// </summary>
    /// <param name="inputStream">The input <see cref="Stream"/> to read from.</param>
    /// <param name="length">The number of bytes to read.</param>
    /// <returns>A byte array containing the number of bytes.</returns>
    /// <exception cref="InvalidOperationException">If there are not enough bytes left in the stream to read the length specified.</exception>
    public static byte[] ReadBlock(this Stream inputStream, int length)
    {
        var result = new byte[length];
        var readCount = inputStream.Read(result, 0, length);
        if (readCount != length)
            throw new InvalidOperationException($"Could not read {length} bytes, only {readCount} bytes were available.");

        return result;
    }

    /// <summary>
    /// Determine eh most common value for a given property in an enumeration of objects.
    /// </summary>
    /// <param name="source">The objects to consider.</param>
    /// <param name="property">The selector to specify the property access.</param>
    /// <typeparam name="TProperty">The type of property values..</typeparam>
    /// <typeparam name="T">The type of objects in the enumeration.</typeparam>
    /// <returns>The most common value for this property.</returns>
    public static TProperty MostCommon<TProperty, T>(this IEnumerable<T> source, Func<T, TProperty> property)
        => source
            .GroupBy(property)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key)
            .FirstOrDefault();
}