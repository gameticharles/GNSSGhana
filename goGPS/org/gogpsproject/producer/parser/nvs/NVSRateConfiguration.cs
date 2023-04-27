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

namespace org.gogpsproject.producer.parser.nvs
{

	using UnsignedOperation = org.gogpsproject.util.UnsignedOperation;


	public class NVSRateConfiguration
	{

		public readonly int nvsPrefix = 0x10;
		public readonly int nvsSuffix = 0x03;

		private List<int?> msg;

		public NVSRateConfiguration(int rate)
		{
			msg = new ArrayList();
			msg.Add(new int?(nvsPrefix));
			msg.Add(new int?(0xD7)); // D7h
			msg.Add(new int?(0x02)); // 02 --> navigation rate
			msg.Add(new int?(rate));
			msg.Add(new int?(nvsPrefix));
			msg.Add(new int?(nvsSuffix));

			msg.Add(new int?(nvsPrefix));
			msg.Add(new int?(0xF4)); // F4h
			rate = 1; //to be further checked...
			msg.Add(new int?((int)((1 / (double) rate) / 0.1)));
			msg.Add(new int?(nvsPrefix));
			msg.Add(new int?(nvsSuffix));
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