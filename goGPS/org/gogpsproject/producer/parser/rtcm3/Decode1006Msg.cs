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

	using Coordinates = org.gogpsproject.positioning.Coordinates;
	using Bits = org.gogpsproject.util.Bits;

	public class Decode1006Msg : Decode
	{


		private RTCM3Client client;
		public Decode1006Msg(RTCM3Client client)
		{
			this.client = client;
		}


		public virtual object decode(bool[] bits, int referenceTS)
		{
			int start = 12;
			//System.out.println("Debug : Decode 1006");
			StationaryAntenna stationaryantenne = new StationaryAntenna();

			stationaryantenne.StationID = (int)Bits.bitsToUInt(Bits.subset(bits, start, 12));
			start += 12;
			stationaryantenne.Itrl = (int)Bits.bitsToUInt(Bits.subset(bits, start, 6));
			start += 6;
			stationaryantenne.GpsIndicator = (int)Bits.bitsToUInt(Bits.subset(bits, start, 1));
			start += 1;
			stationaryantenne.GlonassIndicator = (int)Bits.bitsToUInt(Bits.subset(bits, start, 1));
			start += 1;
			stationaryantenne.RgalileoIndicator = (int)Bits.bitsToUInt(Bits.subset(bits, start, 1));
			start += 1;
			stationaryantenne.RstationIndicator = (int)Bits.bitsToUInt(Bits.subset(bits, start, 1));
			start += 1;
			//System.out.println("x"+Bits.bitsToStr(Bits.subset(bits, start, 38)));
			stationaryantenne.AntennaRefPointX = Bits.bitsTwoComplement(Bits.subset(bits, start, 38)) * 0.0001;
			start += 38;
			stationaryantenne.SreceiverOscillator = (int)Bits.bitsToUInt(Bits.subset(bits, start, 1));
			start += 1;
			stationaryantenne.Reserved1 = (int)Bits.bitsToUInt(Bits.subset(bits, start, 1));
			start += 1;
			//System.out.println("y"+Bits.bitsToStr(Bits.subset(bits, start, 38)));
			stationaryantenne.AntennaRefPointY = Bits.bitsTwoComplement(Bits.subset(bits, start, 38)) * 0.0001;
			start += 38;
			stationaryantenne.Reserved2 = (int)Bits.bitsToUInt(Bits.subset(bits, start, 2));
			start += 2;
			//System.out.println("z"+Bits.bitsToStr(Bits.subset(bits, start, 38)));
			stationaryantenne.AntennaRefPointZ = Bits.bitsTwoComplement(Bits.subset(bits, start, 38)) * 0.0001;
			start += 38;
			stationaryantenne.AntennaHeight = Bits.bitsToUInt(Bits.subset(bits, start, 16)) * 0.0001;
			start += 16;

			Coordinates c = Coordinates.globalXYZInstance(stationaryantenne.AntennaRefPointX, stationaryantenne.AntennaRefPointY, stationaryantenne.AntennaRefPointZ);
			client.MasterPosition = c;
			//System.out.println(stationaryantenne);
			//System.out.println("Debug length: " + start);
			return c;
		}

	}

}