using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace MagneticImage
{
	static class Extensions
	{
		public static string ReadText(this Stream input, int length)
		{
			var buffer = new byte[length];
			input.Read(buffer, 0, length);
			return Encoding.ASCII.GetString(buffer);
		}

		public static ushort ReadWord(this Stream input)
		{
			return (ushort)(input.ReadByte() + (input.ReadByte() * 256));
		}

		public static void Ignore(this Stream input, int length)
		{
			input.Seek(length, SeekOrigin.Current);
		}

		public static byte[] ReadBlock(this Stream input, int length)
		{
			var result = new byte[length];
			input.Read(result, 0, length);
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