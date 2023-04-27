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

namespace org.gogpsproject.producer.parser.skytraq
{


	using Bits = org.gogpsproject.util.Bits;


	public class DecodeRAWMEAS
	{
		private InputStream @in;
		private Observations o;

		public DecodeRAWMEAS(InputStream @in, Observations o)
		{
			this.@in = @in;
			this.o = o;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.gogpsproject.producer.Observations decode(int len) throws java.io.IOException, STQException
		public virtual Observations decode(int len)
		{

			sbyte[] bytes;

			/* IOD, 1 byte */				
			bytes = new sbyte[1];
			@in.read(bytes, 0, bytes.Length);
			int IOD = Bits.byteToIntBigEndian(bytes);

			bytes = new sbyte[1];
			@in.read(bytes, 0, bytes.Length);
			int NMEAS = Bits.byteToIntBigEndian(bytes);

			int gpsCounter = 0;
			bool anomalousValues = false;
			for (int k = 0; k < NMEAS; k++)
			{

				ObservationSet os = new ObservationSet();

				/* Satellite Number, 1 byte */				
				bytes = new sbyte[1];
				@in.read(bytes, 0, bytes.Length);
				int satID = Bits.byteToIntBigEndian(bytes);
				os.SatID = satID;

				/* signal-to-noise ratio, 1 byte */				
				bytes = new sbyte[1];
				@in.read(bytes, 0, bytes.Length);
				int CN0 = Bits.byteToIntBigEndian(bytes);

				/* C/A Pseudorange (m), 8 bytes  */
				bytes = new sbyte[8];
				@in.read(bytes, 0, bytes.Length);
				double pseudoRange = Bits.byteToIEEE754DoubleBigEndian(bytes);
	//			if (pseudoRange < 1e6 || pseudoRange > 6e7) {
	//	        	anomalousValues = true;
	//	        }

				/* Carrier phase (cycles), 8 bytes  */
				bytes = new sbyte[8];
				@in.read(bytes, 0, bytes.Length);
				double carrierPhase = Bits.byteToIEEE754DoubleBigEndian(bytes);

				/*  Doppler Frequency(Hz), 4 bytes  */
				bytes = new sbyte[4];
				@in.read(bytes, 0, bytes.Length);
				float doppler = Bits.byteToIEEE754FloatBigEndian(bytes);

				/* channel indicator, 1 byte */				
				bytes = new sbyte[1];
				@in.read(bytes, 0, bytes.Length);

				if (o.IssueOfData == IOD && os.SatID <= 32 && !anomalousValues)
				{
					os.SatType = 'G';
					os.setSignalStrength(ObservationSet.L1, CN0);
					os.setCodeC(ObservationSet.L1, pseudoRange);
					os.setPhaseCycles(ObservationSet.L1, carrierPhase);
					os.setDoppler(ObservationSet.L1, doppler);
					o.setGps(gpsCounter, os);
					gpsCounter++;
				}
			}

			if (o.NumSat == 0)
			{
				o = null;
			}

			return o;
		}
	}

}