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


	public class UBXMsgConfiguration
	{

		public readonly int uBloxPrefix1 = 0xB5;
		public readonly int uBloxPrefix2 = 0x62;
		private int classid;
		private int msgval;
		private UBXMessageType msgid;

		//private int rate;
		private int CK_A;
		private int CK_B;
		private List<int?> msg;

		public UBXMsgConfiguration(int classtype, int msgtype, bool enable)
		{
			msgid = new UBXMessageType(classtype, msgtype);
			classid = msgid.ClassOut;
			msgval = msgid.IdOut;
			// System.out.println("ID 1 >>:" + ID1 + "ID 2 >>:" + ID2);
			msg = new ArrayList();
			msg.Add(new int?(uBloxPrefix1));
			msg.Add(new int?(uBloxPrefix2));
			msg.Add(new int?(0x06)); // CFG
			msg.Add(new int?(0x01)); // MSG
			msg.Add(new int?(3)); // length low
			msg.Add(new int?(0)); // length hi
			msg.Add(new int?(classid));
			msg.Add(new int?(msgval));
			msg.Add(new int?(enable?0x01:0x00));
			checkSum();
			msg.Add(new int?(CK_A));
			msg.Add(new int?(CK_B));
		}
		public UBXMsgConfiguration(int classtype, int msgtype, int[] smsg)
		{
			msgid = new UBXMessageType(classtype, msgtype);
			classid = msgid.ClassOut;
			msgval = msgid.IdOut;
			// System.out.println("ID 1 >>:" + ID1 + "ID 2 >>:" + ID2);

			msg = new ArrayList();
			msg.Add(new int?(uBloxPrefix1));
			msg.Add(new int?(uBloxPrefix2));
			msg.Add(new int?(classid));
			msg.Add(new int?(msgval));
			int length1 = (smsg.Length) / 0xff;
			int length2 = (smsg.Length) & 0xff;
			msg.Add(new int?(length2));
			msg.Add(new int?(length1));
			for (int i = 0;i < smsg.Length;i++)
			{
				msg.Add(new int?(smsg[i]));
			}
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