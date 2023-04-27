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

	public class Base64
	{

		private const string base64code = "ABCDEFGHIJKLMNOPQRSTUVWXYZ" + "abcdefghijklmnopqrstuvwxyz" + "0123456789" + "+/";
		private const int splitLinesAt = 76;

		public static string encode(string @string)
		{

			string encoded = "";
			sbyte[] stringArray;
			try
			{
				stringArray = @string.GetBytes("UTF-8"); // use appropriate encoding
														// string!
			}
			catch (Exception)
			{
				stringArray = @string.GetBytes(); // use locale default rather than
													// croak
			}
			// determine how many padding bytes to add to the output
			int paddingCount = (3 - (stringArray.Length % 3)) % 3;
			// add any necessary padding to the input
			stringArray = zeroPad(stringArray.Length + paddingCount, stringArray);
			// process 3 bytes at a time, churning out 4 output bytes
			// worry about CRLF insertions later
			for (int i = 0; i < stringArray.Length; i += 3)
			{
				int j = ((stringArray[i] & 0xff) << 16) + ((stringArray[i + 1] & 0xff) << 8) + (stringArray[i + 2] & 0xff);
				encoded = encoded + base64code[(j >> 18) & 0x3f] + base64code[(j >> 12) & 0x3f] + base64code[(j >> 6) & 0x3f] + base64code[j & 0x3f];
			}
			// replace encoded padding nulls with "="
			return splitLines(encoded.Substring(0, encoded.Length - paddingCount) + "==".Substring(0, paddingCount));

		}

		public static string splitLines(string @string)
		{

			string lines = "";
			for (int i = 0; i < @string.Length; i += splitLinesAt)
			{

				lines += @string.Substring(i, Math.Min(@string.Length, splitLinesAt));
				lines += "\r\n";

			}
			return lines;

		}

		public static sbyte[] zeroPad(int length, sbyte[] bytes)
		{
			sbyte[] padded = new sbyte[length]; // initialized to zero by JVM
			Array.Copy(bytes, 0, padded, 0, bytes.Length);
			return padded;
		}

		public Base64()
		{

		}
	}

}