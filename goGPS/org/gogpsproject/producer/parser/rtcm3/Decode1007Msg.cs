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

	public class Decode1007Msg : Decode
	{

		private RTCM3Client client;
		public Decode1007Msg(RTCM3Client client)
		{
			this.client = client;
		}

		public virtual object decode(bool[] bits, int referenceTS)
		{

			if (bits.Length < 32)
			{
				return null;
			}

			AntennaDescriptor antenna = new AntennaDescriptor();
			int start = 12;
			string desc = "";
			antenna.StationID = (int)Bits.bitsToUInt(Bits.subset(bits, start, 12));
			start += 12;
			int cnt = (int)Bits.bitsToUInt(Bits.subset(bits, start, 8));
			start += 8;
			if (bits.Length < 8 + 8 * cnt)
			{
				return null;
			}
			for (int i = 0; i < cnt; i++)
			{
				char value = (char) Bits.bitsToUInt(Bits.subset(bits, start, 8));
				desc += char.ToString(value);
				start += 8;
			}
			antenna.AntennaDescriptor = desc;
			antenna.SetupID = (int)Bits.bitsToUInt(Bits.subset(bits, start, 8));
			start += 8;

			client.AntennaDescriptor = antenna;
			//System.out.println(antenna);
			//System.out.println(start);
			return antenna;
		}


	}

}