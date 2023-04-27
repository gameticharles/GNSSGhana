using System.Collections;
using System.Collections.Generic;

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

namespace org.gogpsproject.producer.parser.ublox
{

	using UnsignedOperation = org.gogpsproject.util.UnsignedOperation;


	public class UBXRateConfiguration
	{

		public readonly int uBloxPrefix1 = 0xB5;
		public readonly int uBloxPrefix2 = 0x62;

		//private int rate;
		private int CK_A;
		private int CK_B;
		private List<int?> msg;

		public UBXRateConfiguration(int measRate, int navRate, int timeRef)
		{
			sbyte[] measRateBytes = ByteBuffer.allocate(4).putInt(measRate).array();
			sbyte[] navRateBytes = ByteBuffer.allocate(4).putInt(navRate).array();
			sbyte[] timeRefBytes = ByteBuffer.allocate(4).putInt(timeRef).array();
			msg = new ArrayList();
			msg.Add(new int?(uBloxPrefix1));
			msg.Add(new int?(uBloxPrefix2));
			msg.Add(new int?(0x06)); // CFG
			msg.Add(new int?(0x08)); // RATE
			msg.Add(new int?(6)); // length low
			msg.Add(new int?(0)); // length hi
			msg.Add(new int?(measRateBytes[3]));
			msg.Add(new int?(measRateBytes[2]));
			msg.Add(new int?(navRateBytes[3]));
			msg.Add(new int?(navRateBytes[2]));
			msg.Add(new int?(timeRefBytes[3]));
			msg.Add(new int?(timeRefBytes[2]));
			checkSum();
			msg.Add(new int?(CK_A));
			msg.Add(new int?(CK_B));
		}

		private void checkSum()
		{
			CK_A = 0;
			CK_B = 0;
			for (int i = 2; i < msg.Count; i++)
			{
				CK_A = CK_A + (int)((int?) msg[i]);
				CK_B = CK_B + CK_A;

			}
			CK_A = CK_A & 0xFF;
			CK_B = CK_B & 0xFF;
		}

		public virtual sbyte[] Byte
		{
			get
			{
				sbyte[] bytes = new sbyte[msg.Count];
				for (int i = 0; i < msg.Count; i++)
				{
					bytes[i] = UnsignedOperation.unsignedIntToByte((int)((int?)msg[i]));
				}
				return bytes;
			}
		}

	}

}