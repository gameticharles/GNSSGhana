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

namespace org.gogpsproject.producer.parser.rtcm3
{

	using Bits = org.gogpsproject.util.Bits;


	public class Decode1012Msg : Decode
	{
		public Decode1012Msg()
		{

		}
		public virtual object decode(bool[] bits, int referenceTS)
		{
			int start = 12;
			GlonassHeader glonassh = new GlonassHeader();
			GlonassSatellite satellite = new GlonassSatellite();

			glonassh.Stationid = (int)Bits.bitsToUInt(Bits.subset(bits, start, 12)); // 12
			start += 12;
			glonassh.EpochTime = Bits.bitsToUInt(Bits.subset(bits, start, 27)); // 27
			start += 27;
			glonassh.Flag = (int)Bits.bitsToUInt(Bits.subset(bits, start, 1)); // 1
			start++;
			glonassh.NumberOfSatellites = (int)Bits.bitsToUInt(Bits.subset(bits, start, 5)); // 5
			start += 5;
			glonassh.SmoothIndicator = (int)Bits.bitsToUInt(Bits.subset(bits, start, 1)); // 1
			start += 1;
			glonassh.SmoothInterval = (int)Bits.bitsToUInt(Bits.subset(bits, start, 3)); // 3
			start += 3;
			//System.out.println(glonassh);
			for (int i = 0; i < glonassh.NumberOfSatellites; i++)
			{
				satellite.SatID = (int)Bits.bitsToUInt(Bits.subset(bits, start, 6));
				start += 6;
				satellite.L1code = (int)Bits.bitsToUInt(Bits.subset(bits, start, 1));
				start += 1;
				satellite.SatFrequency = (int)Bits.bitsToUInt(Bits.subset(bits, start, 5));
				start += 5;
				satellite.L1pseudorange = Bits.bitsTwoComplement(Bits.subset(bits,start, 25));
				start += 25;
				satellite.L1phaserange = Bits.bitsToUInt(Bits.subset(bits, start, 20));
				start += 20;
				satellite.L1locktime = (int)Bits.bitsTwoComplement(Bits.subset(bits, start, 7));
				start += 7;
				satellite.L1psedorangemod = (int)Bits.bitsToUInt(Bits.subset(bits, start, 7));
				start += 7;
				satellite.L1CNR = (int)Bits.bitsToUInt(Bits.subset(bits, start, 8));
				start += 8;
				satellite.L2code = (int)Bits.bitsToUInt(Bits.subset(bits, start, 2));
				start += 2;
				satellite.L2l1psedorangeDif = Bits.bitsTwoComplement(Bits.subset(bits, start, 14));
				start += 14;
				satellite.L2l1phaserangeDif = Bits.bitsTwoComplement(Bits.subset(bits, start, 20));
				start += 20;
				satellite.L2locktime = (int)Bits.bitsToUInt(Bits.subset(bits, start, 7));
				start += 7;
				satellite.L2CNR = (int)Bits.bitsToUInt(Bits.subset(bits, start, 8));
				start += 8;
			}
			return null;
		}


	//	public String toString() {
	//		return "Decode1012Msg [bits=" + Arrays.toString(bits) + ", glonassh=" + glonassh + ", start=" + start
	//				+ "]";
	//	}
	}

}