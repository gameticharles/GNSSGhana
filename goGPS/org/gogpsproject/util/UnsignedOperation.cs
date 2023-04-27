using System;

/*
 * Copyright (c) 2010 Eugenio Realini, Mirko Reguzzoni, Cryms sagl - Switzerland. All Rights Reserved.
 *
 * This file is part of goGPS Project (goGPS).
 *
 * goGPS is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as
 * published by the Free Software Foundation, either version 3
 * of the License, or (at your option) any later version.
 *
 * goGPS is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with goGPS.  If not, see <http://www.gnu.org/licenses/>.
 *
 */

namespace org.gogpsproject.util
{

	public class UnsignedOperation
	{
		public static readonly int SIZEOF_LONG = sizeof(long) / sizeof(sbyte);
		public static readonly int SIZEOF_INT = sizeof(int) / sizeof(sbyte);

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: private static IllegalArgumentException explainWrongLengthOrOffset(final byte[] bytes, final int offset, final int length, final int expectedLength)
		private static System.ArgumentException explainWrongLengthOrOffset(sbyte[] bytes, int offset, int length, int expectedLength)
		{
			string reason;
			if (length != expectedLength)
			{
				reason = "Wrong length: " + length + ", expected " + expectedLength;
			}
			else
			{
				reason = "offset (" + offset + ") + length (" + length + ") exceed the" + " capacity of the array: " + bytes.Length;
			}
			return new System.ArgumentException(reason);
		}

		public static void Main(string[] args)
		{
			int[] val = new int[] {2, 2};
			Console.WriteLine(unsignedByteToIntto(val));
		}

		public static int putDouble(sbyte[] bytes, int offset, double d)
		{
			return putLong(bytes, offset, double.doubleToLongBits(d));
		}

		public static int putFloat(sbyte[] bytes, int offset, float f)
		{
			return putInt(bytes, offset, float.floatToRawIntBits(f));
		}

		public static int putInt(sbyte[] bytes, int offset, int val)
		{
			if (bytes.Length - offset < SIZEOF_INT)
			{
				throw new System.ArgumentException("Not enough room to put an int at" + " offset " + offset + " in a " + bytes.Length + " byte array");
			}
			for (int i = offset + 3; i > offset; i--)
			{
				bytes[i] = (sbyte) val;
				val = (int)((uint)val >> 8);
			}
			bytes[offset] = (sbyte) val;
			return offset + SIZEOF_INT;
		}

		public static int putLong(sbyte[] bytes, int offset, long val)
		{
			if (bytes.Length - offset < SIZEOF_LONG)
			{
				throw new System.ArgumentException("Not enough room to put a long at" + " offset " + offset + " in a " + bytes.Length + " byte array");
			}
			for (int i = offset + 7; i > offset; i--)
			{
				bytes[i] = (sbyte) val;
				val = (long)((ulong)val >> 8);
			}
			bytes[offset] = (sbyte) val;
			return offset + SIZEOF_LONG;
		}

		public static sbyte[] toBytes(int val)
		{
			sbyte[] b = new sbyte[4];
			for (int i = 3; i > 0; i--)
			{
				b[i] = (sbyte) val;
				val = (int)((uint)val >> 8);
			}
			b[0] = (sbyte) val;
			return b;

		}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static double toDouble(final byte[] bytes)
		public static double toDouble(sbyte[] bytes)
		{
			return toDouble(bytes, 0);
		}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static double toDouble(final byte[] bytes, final int offset)
		public static double toDouble(sbyte[] bytes, int offset)
		{
			return double.longBitsToDouble(toLong(bytes, offset, SIZEOF_LONG));
		}

		public static float toFloat(sbyte[] bytes)
		{
			return toFloat(bytes, 0);
		}

		public static float toFloat(sbyte[] bytes, int offset)
		{
			return float.intBitsToFloat(toInt(bytes, offset, SIZEOF_INT));
		}

		public static int toInt(sbyte[] bytes)
		{
			return toInt(bytes, 0, SIZEOF_INT);
		}

		public static int toInt(sbyte[] bytes, int offset)
		{
			return toInt(bytes, offset, SIZEOF_INT);
		}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static int toInt(byte[] bytes, int offset, final int length)
		public static int toInt(sbyte[] bytes, int offset, int length)
		{
			if (length != SIZEOF_INT || offset + length > bytes.Length)
			{
				throw explainWrongLengthOrOffset(bytes, offset, length, SIZEOF_INT);
			}
			int n = 0;
			for (int i = offset; i < (offset + length); i++)
			{
				n <<= 8;
				n ^= bytes[i] & 0xFF;
			}
			return n;
		}

		public static long toLong(sbyte[] bytes)
		{
			return toLong(bytes, 0, SIZEOF_LONG);
		}

		public static long toLong(sbyte[] bytes, int offset)
		{
			return toLong(bytes, offset, SIZEOF_LONG);
		}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static long toLong(byte[] bytes, int offset, final int length)
		public static long toLong(sbyte[] bytes, int offset, int length)
		{
			if (length != SIZEOF_LONG || offset + length > bytes.Length)
			{
				throw explainWrongLengthOrOffset(bytes, offset, length, SIZEOF_LONG);
			}
			long l = 0;
			for (int i = offset; i < offset + length; i++)
			{
				l <<= 8;
				l ^= bytes[i] & 0xFF;
			}
			return l;
		}

		public static int unsignedByteToInt(sbyte b)
		{
			return b & 0xFF;
		}

		public static int[] unsignedByteToInt(sbyte[] b)
		{
			int[] i = new int[b.Length];
			for (int x = 0; x < b.Length; x++)
			{
				i[x] = b[x] & 0xFF;
			}
			return i;
		}

		public static long unsignedByteToIntto(int[] i)
		{
			long val = 0L;
			int x = i.Length - 1;
			for (int j = 0; j < i.Length; j++)
			{
				val += i[j];
				val = val << x * 8;
				Console.WriteLine((val));
				x--;
			}
			return val;
		}

		public static sbyte unsignedIntToByte(int b)
		{
			return unchecked((sbyte)(b & 0xFF));
		}

		public static sbyte[] unsignedIntToByte(int[] i)
		{
			sbyte[] b = new sbyte[i.Length];
			for (int x = 0; x < i.Length; x++)
			{
				b[x] = unchecked((sbyte)(i[x] & 0xFF));
			}
			return b;
		}

	}
}