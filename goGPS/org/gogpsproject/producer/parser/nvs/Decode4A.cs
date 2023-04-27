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

namespace org.gogpsproject.producer.parser.nvs
{


	using EphGps = org.gogpsproject.ephemeris.EphGps;
	using Time = org.gogpsproject.positioning.Time;
	using Bits = org.gogpsproject.util.Bits;
	using UnsignedOperation = org.gogpsproject.util.UnsignedOperation;


	public class Decode4A
	{

	//	private BufferedInputStream in;
		internal InputStream @in;

		public Decode4A(InputStream _in)
		{
	//	public Decode4A(BufferedInputStream _in) {
			@in = _in;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.gogpsproject.producer.parser.IonoGps decode() throws java.io.IOException,NVSException
		public virtual IonoGps decode()
		{
			// parse little Endian data

			IonoGps iono = new IonoGps();

			sbyte[] bytes;

	//		System.out.println("+----------------  Start of 4A  ------------------+");

			/*  Alpha, 4 bytes each  */		
			bytes = new sbyte[4];
			float[] alpha = new float[4];
			for (int i = 0;i < alpha.Length;i++)
			{
				@in.read(bytes, 0, bytes.Length);
				alpha[i] = Bits.byteToIEEE754Float(bytes);
	//			System.out.println("Alpha" +i + ": " + alpha[i]);
			}


			/*  Beta, 4 bytes each  */		
			bytes = new sbyte[4];
			float[] beta = new float[4];
			for (int i = 0;i < beta.Length;i++)
			{
				@in.read(bytes, 0, bytes.Length);
				beta[i] = Bits.byteToIEEE754Float(bytes);
	//			System.out.println("Beta" +i + ": " + beta[i]);
			}


			int reliable_sign = @in.read();

			if (reliable_sign == 255) // 255 - the data is reliable
			{
	//          System.out.println("Reliable Sign: "+ reliable_sign); 

				iono.Alpha = alpha;
				iono.Beta = beta;
			}


	//        System.out.println("Reliable Sign: "+ reliable_sign); 
	//		System.out.println("+-----------------  End of 4A  -------------------+");

	//        in.read(); // DLE
	//        in.read(); // ETX

			return iono;
		}


	}

}