using System;
using System.Text;

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

	public class Bits
	{

	//	public static void main(String args[]){
	//
	//		System.out.println(bitsToInt(new boolean[]{true,false,true}) + " " + bitsToInt(new boolean[]{true,false,true}));
	//		System.out.println(bitsToInt(new boolean[]{true,false,false}) + " " + bitsToInt(new boolean[]{true,false,false}));
	//		System.out.println(bitsToInt(new boolean[]{false,false,true}) + " " + bitsToInt(new boolean[]{false,false,true}));
	//
	//	}
	//	public static int bitsToInt(boolean[] bits) {
	//		int result = 0;
	//
	//		for (int i = 0; i < bits.length; i++) {
	//			if (bits[i]) {
	//				result = result
	//						+ (int) java.lang.Math.pow(2, (bits.length - i - 1));
	//			}
	//		}
	//
	//		return result;
	//	}
		public static long bitsToUInt(bool[] bits)
		{
			long result = 0;



			long pow2 = 1;
			for (int i = bits.Length - 1; i >= 0; i--)
			{
				if (bits[i])
				{
					result = result + pow2; //(int) java.lang.Math.pow(2, (bits.length - i - 1));
				}
				pow2 = pow2 * 2;
			}

			return result;
		}
		public static long bitsToULong(bool[] bits)
		{
			long result = 0;



			long pow2 = 1;
			for (int i = bits.Length - 1; i >= 0; i--)
			{
				if (bits[i])
				{
					result = result + pow2; //(int) java.lang.Math.pow(2, (bits.length - i - 1));
				}
				pow2 = pow2 * 2;
			}

			return result;
		}
		/// <summary>
		/// convert bits to String
		/// </summary>
		/// <param name="b">
		///            byte to convert
		/// </param>
		/// <returns> String of 0's and 1's </returns>
		public static string bitsToStr(bool[] b)
		{
			string result = "";
			for (int i = 0; i < b.Length; i++)
			{
				if (b[i])
				{
					result = result + "1";
				}
				else
				{
					result = result + "0";
				}
			}

			return result;
		}

		public static long bitsTwoComplement(bool[] bits)
		{
			long result;

			if (!bits[0])
			{
				// If the most significant bit are 0 then the integer is positive
				result = bitsToUInt(bits);
			}
			else
			{
				// If the most significant bit are 1 then the integer is negative
				// and the bits must be inverted and added 1 in order to get the
				// correct negative integer
				bool[] b = new bool[bits.Length];
				for (int i = 0; i < bits.Length; i++)
				{
					b[i] = !bits[i];
				}
				result = -1 * (bitsToUInt(b) + 1);
			}

			return result;
		}

		/// <summary>
		/// convert a integer (byte) to a bit String
		/// </summary>
		/// <param name="b">
		///            integer to convert, only the first byte are used </param>
		public static string byteToStr(int b)
		{
			return bitsToStr(rollByteToBits(b));
		}

		/// <summary>
		/// compares two bit arrays for idendeical length and bits
		/// </summary>
		/// <param name="b1">
		///            bit array to compare </param>
		/// <param name="b2">
		///            bit array to compare
		/// </param>
		/// <returns> <code>true</code> if the bit arrays are identical,
		///         <code>false</code> otherwise </returns>
		public static bool compare(bool[] b1, bool[] b2)
		{
			// The length of the bit arrays is first compared
			bool result = b1.Length != 0 && (b1.Length == b2.Length);

			// Only bit arrays of equal length are further examined
			int i = 0;
			while (result && i < b1.Length)
			{
				result = (b1[i] == b2[i]);
				i++;
			}

			return result;
		}

		/// <summary>
		/// concatinates two bit arrays into one new
		/// </summary>
		/// <param name="b1">
		///            the first bit array. Data from here are the first in the new
		///            array
		/// @paran b2 the second bit array </param>
		public static bool[] concat(bool[] b1, bool[] b2)
		{
			// As there is no check of nullity an exception will be thrown by the
			// JVM if one of the arrays is null
			bool[] result = new bool[b1.Length + b2.Length];
			for (int i = 0; i < b1.Length; i++)
			{
				result[i] = b1[i];
			}
			for (int i = 0; i < b2.Length; i++)
			{
				result[i + b1.Length] = b2[i];
			}

			return result;
		}

		/// <summary>
		/// copies an entire bit array into a new bit array
		/// </summary>
		/// <param name="b">
		///            the bit array to copy </param>
		public static bool[] copy(bool[] b)
		{
			// Function just uses subset to copy
			return subset(b, 0, b.Length);
		}


		/// <summary>
		/// convert a byte to bits
		/// </summary>
		/// <param name="in">
		///            byte to convert </param>
		/// <param name="length">
		///            how many bits to use, must be between 1 and 32 </param>

		public static bool[] byteToBits(sbyte b, int length)
		{
			int @in = getUInt(b);
			bool[] result = new bool[length];
			for (int i = 0; i < length; i++)
			{
				result[length - 1 - i] = (@in % 2 == 1);
				@in = @in / 2;
			}
			return result;
		}


		/// <summary>
		/// convert a integer to bits
		/// </summary>
		/// <param name="in">
		///            integer to convert </param>
		/// <param name="length">
		///            how many bits to use, must be between 1 and 32 </param>

		public static bool[] intToBits(int @in, int length)
		{
			bool[] result = new bool[length];
			for (int i = 0; i < length; i++)
			{
				result[length - 1 - i] = (@in % 2 == 1);
				@in = @in / 2;
			}
			return result;
		}

		/// <summary>
		/// convert a byte (given as an integer) to bits with all bits turned
		/// </summary>
		/// <param name="in">
		///            integer to convert, only the first byte are used </param>
		public static bool[] rollByteToBits(int @in)
		{
			bool[] result = new bool[8];
			for (int i = 7; i > -1; i--)
			{
				result[i] = (@in % 2 == 1);
				@in = @in / 2;
			}

			// int ct = 10000000;
			// for (int i = 0; i < 8; i++) {
			// result[i] = (in / ct == 1);
			// ct /= 10;
			// }
			// System.out.println("" + Bits.subsetBin(result, 0, 8));
			return result;

		}

		/// <summary>
		/// copies a subset from a bit array into a new bit array
		/// </summary>
		/// <param name="b">
		///            bit array to copy from </param>
		/// <param name="start">
		///            the index to start from </param>
		/// <param name="length">
		///            the length of the subset
		/// </param>
		/// <exception cref="ArrayIndexOutOfBoundsException">
		///             if subset exceeds the original arrays length (not
		///             declared) </exception>
		public static bool[] subset(bool[] b, int start, int length)
		{
			bool[] result;

			if (start >= b.Length || start + length > b.Length)
			{
				// Exception is thrown if the index starts before 0, or exceeds
				// the length of the original array
				result = null;
				throw new System.IndexOutOfRangeException("Invalid subset: exceeds length of " + b.Length + ":\nstart of subset: " + start + ", length of subset: " + length);
			}
			else
			{
				result = new bool[length];
				for (int i = 0; i < length; i++)
				{
					result[i] = b[start + i];
				}
			}

			return result;
		}

		public static string subsetBin(bool[] b, int start, int length)
		{
			string result = "b://";

			if (start >= b.Length || start + length > b.Length)
			{
				// Exception is thrown if the index starts before 0, or succseeds
				// the
				// length of the original array
				result = null;
				throw new System.IndexOutOfRangeException("Invalid subset: Succseeds length of " + b.Length + ":\nstart of subset: " + start + ", length of subset: " + length);
			}
			else
			{
				for (int i = 0; i < length; i++)
				{
					if (b[start + i])
					{
						result += "1";
					}
					else
					{
						result += "0";
					}

				}
			}

			return result;
		}

		public static sbyte[] tobytes(bool[] bits)
		{
			sbyte[] bytes = new sbyte[bits.Length / 8];
			int indice = 0;
			for (int i = 0; i < bits.Length / 8; i++)
			{
				bytes[i] = (sbyte) bitsToUInt(subset(bits, indice, 8));
				indice += 8;
			}
			return bytes;
		}

		public static int getUInt(sbyte b)
		{
			return b >= 0?b:256 + b;
		}

		public static double byteToIEEE754Double(sbyte[] l)
		{

			long bits = 0;
			for (int i = l.Length - 1;i >= 0;i--)
			{
				bits = bits << 8;
				 bits = bits | getUInt(l[i]);
			}

			return Convert.ToDouble(bits);
		}
		public static float byteToIEEE754Float(sbyte[] l)
		{
			int bits = 0;
			for (int i = l.Length - 1;i >= 0;i--)
			{
				bits = bits << 8;
				bits = bits | getUInt(l[i]);
			}
			return (float)Convert.ToDouble(bits);
        }
		public static long byteToLong(sbyte[] l)
		{
			long bits = 0;
			for (int i = l.Length - 1;i >= 0;i--)
			{
				bits = bits << 8;
				bits = bits | getUInt(l[i]);
			}

			return bits;
		}
		public static int byteToInt(sbyte[] l)
		{
			int bits = 0;
			for (int i = l.Length - 1;i >= 0;i--)
			{
				bits = bits << 8;
				bits = bits | getUInt(l[i]);
			}

			return bits;
		}
		public static double byteToIEEE754DoubleBigEndian(sbyte[] l)
		{

			long bits = 0;
			for (int i = 0;i < l.Length;i++)
			{
				bits = bits << 8;
				 bits = bits | getUInt(l[i]);
			}

			return Convert.ToDouble(bits);
		}
		public static float byteToIEEE754FloatBigEndian(sbyte[] l)
		{
			int bits = 0;
			for (int i = 0;i < l.Length;i++)
			{
				bits = bits << 8;
				bits = bits | getUInt(l[i]);
			}
			return (float)Convert.ToDouble(bits);
		}
		public static long byteToLongBigEndian(sbyte[] l)
		{
			int bits = 0;
			for (int i = 0;i < l.Length;i++)
			{
				bits = bits << 8;
				bits = bits | getUInt(l[i]);
			}

			return bits;
		}
		public static int byteToIntBigEndian(sbyte[] l)
		{
			int bits = 0;
			for (int i = 0;i < l.Length;i++)
			{
				bits = bits << 8;
				bits = bits | getUInt(l[i]);
			}

			return bits;
		}

		/// <summary>
		/// Convert a specified number of bytes from a byte array into a readable hex string.
		/// </summary>
		/// <param name="byteArray"> the array of data. </param>
		/// <param name="length"> the number of bytes of data to include int he string.
		/// </param>
		/// <returns> the string representation of the byte array. </returns>
		public static string toHexString(sbyte[] byteArray, int length)
		{
			StringBuilder buffer = new StringBuilder();
			for (int i = 0; i < length; i++)
			{
				buffer.Append(toHex(byteArray[i]));
			}
			return buffer.ToString();
		}

		/// <summary>
		/// Convert a byte into a readable hex string.
		/// </summary>
		/// <param name="b"> the data to convert into a string.
		/// </param>
		/// <returns> a string representation of the byte in hex. </returns>
		public static string toHex(sbyte b)
		{
			int? I = new int?((int)((uint)(b << 24) >> 24));
			int i = (int)I;

			if (i < (sbyte)16)
			{
				return "0" + Convert.ToString(i,16).ToUpper();
			}
			return Convert.ToString(i,16).ToUpper();
		}
	}

}