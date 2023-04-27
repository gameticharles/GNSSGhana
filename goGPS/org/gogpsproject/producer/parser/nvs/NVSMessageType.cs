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

namespace org.gogpsproject.producer.parser.nvs
{

	public class NVSMessageType
	{
		public const int CLASS_NAV = 0;
		public const int CLASS_RXM = 1;
		public const int CLASS_INF = 2;
		public const int CLASS_ACK = 3;
		public const int CLASS_CFG = 4;
		public const int CLASS_UPD = 5;
		public const int CLASS_MON = 6;
		public const int CLASS_AID = 7;
		public const int CLASS_TIM = 8;
		public const int CLASS_NMEA = 9;
		public const int CLASS_PUBX = 10;

		public const int ACK_ACK = 11;
		public const int ACK_NAK = 12;


		public const int AID_REQ = 13;
		public const int AID_DATA = 14;
		public const int AID_INI = 15;
		public const int AID_HUI = 16;
		public const int AID_ALM = 17;
		public const int AID_EPH = 18;

		public const int CFG_PRT = 19;
		public const int CFG_USB = 20;
		public const int CFG_MSG = 21;
		public const int CFG_NMEA = 22;
		public const int CFG_RATE = 23;
		public const int CFG_CFG = 24;
		public const int CFG_TP = 25;
		public const int CFG_NAV2 = 26;
		public const int CFG_DAT = 27;
		public const int CFG_INF = 28;
		public const int CFG_RST = 29;
		public const int CFG_RXM = 30;
		public const int CFG_ANT = 31;
		public const int CFG_FXN = 32;
		public const int CFG_SBAS = 33;
		public const int CFG_LIC = 34;
		public const int CFG_TM = 35;
		public const int CFG_TM2 = 36;
		public const int CFG_TMODE = 37;
		public const int CFG_EKF = 38;

		public const int INF_ERROR = 39;
		public const int INF_WARNING = 40;
		public const int INF_NOTICE = 41;
		public const int INF_TEST = 42;
		public const int INF_DEBUG = 43;
		public const int INF_USER = 44;

		public const int MON_SCHD = 45;
		public const int MON_IO = 46;
		public const int MON_MSGPP = 47;
		public const int MON_RXBUF = 48;
		public const int MON_TXBUF = 49;
		public const int MON_HW = 50;
		public const int MON_IPC = 51;
		public const int MON_USB = 52;
		public const int MON_VER = 53;
		public const int MON_EXCEPT = 54;

		public const int NAV_POSECEF = 55;
		public const int NAV_POSLLH = 56;
		public const int NAV_POSUTM = 57;
		public const int NAV_DOP = 58;
		public const int NAV_STATUS = 59;
		public const int NAV_SOL = 60;
		public const int NAV_VELECEF = 61;
		public const int NAV_VELNED = 62;
		public const int NAV_TIMEGPS = 63;
		public const int NAV_TIMEUTC = 64;
		public const int NAV_CLOCK = 65;
		public const int NAV_SVINFO = 66;
		public const int NAV_DGPS = 67;
		public const int NAV_SBAS = 68;
		public const int NAV_EKFSTATUS = 69;

		public const int NMEA_GGA = 70;
		public const int NMEA_GLL = 71;
		public const int NMEA_GSA = 72;
		public const int NMEA_GSV = 73;
		public const int NMEA_RMC = 74;
		public const int NMEA_VTG = 75;
		public const int NMEA_GRS = 76;
		public const int NMEA_GST = 77;
		public const int NMEA_ZDA = 78;
		public const int NMEA_GBS = 79;
		public const int NMEA_DTM = 80;

		public const int PUBX_A = 81;
		public const int PUBX_B = 82;
		public const int PUBX_C = 83;
		public const int PUBX_D = 84;

		public const int RXM_RAW = 85;
		public const int RXM_SVSI = 86;
		public const int RXM_SFRB = 87;
		public const int RXM_ALM = 88;
		public const int RXM_EPH = 89;
		public const int RXM_POSREQ = 90;
		public const int RXM_PMREQ = 98;

		public const int TIM_TM = 90;
		public const int TIM_TM2 = 91;
		public const int TIM_TP = 92;
		public const int TIM_SVIN = 93;

		public const int UPD_DOWNL = 94;
		public const int UPD_UPLOAD = 95;
		public const int UPD_EXEC = 96;
		public const int UPD_MEMCPY = 97;

		private static int classOut = 0;

		private static int idOut = 0;

		private static int ClassOut
		{
			set
			{
				NVSMessageType.classOut = value;
			}
			get
			{
				return classOut;
			}
		}

		private static int IdOut
		{
			set
			{
				NVSMessageType.idOut = value;
			}
			get
			{
				return idOut;
			}
		}

		public NVSMessageType(int mclass, int msgtype)
		{
			getMsg(mclass, msgtype);
		}

		public NVSMessageType(string mclass, string msgtype)
		{
			getMsg(mclass, msgtype);
		}



		private int getMsg(int mclass, int msgtype)
		{
			try
			{
				switch (mclass)
				{

				case CLASS_NAV:
					ClassOut = 0x01;
					switch (msgtype)
					{
					case NAV_POSECEF:
						IdOut = 0x01;
						return 0;
					case NAV_POSLLH:
						IdOut = 0x02;
						return 0;
					case NAV_POSUTM:
						IdOut = 0x08;
						return 0;
					case NAV_DOP:
						IdOut = 0x04;
						return 0;
					case NAV_STATUS:
						IdOut = 0x03;
						return 0;
					case NAV_SOL:
						IdOut = 0x06;
						return 0;
					case NAV_VELECEF:
						IdOut = 0x11;
						return 0;
					case NAV_VELNED:
						IdOut = 0x12;
						return 0;
					case NAV_TIMEGPS:
						IdOut = 0x20;
						return 0;
					case NAV_TIMEUTC:
						IdOut = 0x21;
						return 0;
					case NAV_CLOCK:
						IdOut = 0x22;
						return 0;
					case NAV_SVINFO:
						IdOut = 0x30;
						return 0;
					case NAV_DGPS:
						IdOut = 0x31;
						return 0;
					case NAV_SBAS:
						IdOut = 0x32;
						return 0;
					case NAV_EKFSTATUS:
						IdOut = 0x40;
						return 0;
					}

				case CLASS_RXM:
					ClassOut = 0x02;
					switch (msgtype)
					{
						case RXM_RAW:
							IdOut = 0x10;
							return 0;
						case RXM_SVSI:
							IdOut = 0x20;
							return 0;
						case RXM_SFRB:
							IdOut = 0x11;
							return 0;
						case RXM_ALM:
							IdOut = 0x30;
							return 0;
						case RXM_EPH:
							IdOut = 0x31;
							return 0;
						case RXM_POSREQ:
							IdOut = 0x40;
							return 0;
						case RXM_PMREQ:
							IdOut = 0x41;
							return 0;
					}

				case CLASS_INF:
					ClassOut = 0x04;
					switch (msgtype)
					{
					case INF_ERROR:
						IdOut = 0x00;
						return 0;
					case INF_WARNING:
						IdOut = 0x01;
						return 0;
					case INF_NOTICE:
						IdOut = 0x02;
						return 0;
					case INF_TEST:
						IdOut = 0x03;
						return 0;
					case INF_DEBUG:
						IdOut = 0x04;
						return 0;
					case INF_USER:
						IdOut = 0x07;
						return 0;
					}
				case CLASS_ACK:
					ClassOut = 0x05;
					switch (msgtype)
					{
					case ACK_ACK:
						IdOut = 0x01;
						return 0;
					case ACK_NAK:
						IdOut = 0x00;
						return 0;
					}

				case CLASS_CFG:
					ClassOut = 0x06;
					switch (msgtype)
					{
					case CFG_PRT:
						IdOut = 0x00;
						return 0;
					case CFG_USB:
						IdOut = 0x1B;
						return 0;
					case CFG_MSG:
						IdOut = 0x01;
						return 0;
					case CFG_NMEA:
						IdOut = 0x17;
						return 0;
					case CFG_RATE:
						IdOut = 0x08;
						return 0;
					case CFG_CFG:
						IdOut = 0x09;
						return 0;
					case CFG_TP:
						IdOut = 0x07;
						return 0;
					case CFG_NAV2:
						IdOut = 0x1A;
						return 0;
					case CFG_DAT:
						IdOut = 0x06;
						return 0;
					case CFG_INF:
						IdOut = 0x02;
						return 0;
					case CFG_RST:
						IdOut = 0x04;
						return 0;
					case CFG_RXM:
						IdOut = 0x11;
						return 0;
					case CFG_ANT:
						IdOut = 0x13;
						return 0;
					case CFG_FXN:
						IdOut = 0x0E;
						return 0;
					case CFG_SBAS:
						IdOut = 0x16;
						return 0;
					case CFG_LIC:
						IdOut = 0x80;
						return 0;
					case CFG_TM:
						IdOut = 0x10;
						return 0;
					case CFG_TM2:
						IdOut = 0x19;
						return 0;
					case CFG_TMODE:
						IdOut = 0x1D;
						return 0;
					case CFG_EKF:
						IdOut = 0x12;
						return 0;
					}

				case CLASS_UPD:
					ClassOut = 0x09;
					switch (msgtype)
					{
					case UPD_DOWNL:
						IdOut = 0x01;
						return 0;
					case UPD_UPLOAD:
						IdOut = 0x02;
						return 0;
					case UPD_EXEC:
						IdOut = 0x03;
						return 0;
					case UPD_MEMCPY:
						IdOut = 0x04;
						return 0;
					}

				case CLASS_MON:
					ClassOut = 0x0A;
					switch (msgtype)
					{
					case MON_SCHD:
						IdOut = 0x01;
						return 0;
					case MON_IO:
						IdOut = 0x02;
						return 0;
					case MON_MSGPP:
						IdOut = 0x06;
						return 0;
					case MON_RXBUF:
						IdOut = 0x07;
						return 0;
					case MON_TXBUF:
						IdOut = 0x08;
						return 0;
					case MON_HW:
						IdOut = 0x09;
						return 0;
					case MON_IPC:
						IdOut = 0x03;
						return 0;
					case MON_USB:
						IdOut = 0x0A;
						return 0;
					case MON_VER:
						IdOut = 0x04;
						return 0;
					case MON_EXCEPT:
						IdOut = 0x05;
						return 0;
					}

				case CLASS_AID:
					ClassOut = 0x0B;
					switch (msgtype)
					{
					case AID_REQ:
						IdOut = 0x00;
						return 0;
					case AID_DATA:
						IdOut = 0x10;
						return 0;
					case AID_INI:
						IdOut = 0x01;
						return 0;
					case AID_HUI:
						IdOut = 0x02;
						return 0;
					case AID_ALM:
						IdOut = 0x30;
						return 0;
					case AID_EPH:
						IdOut = 0x31;
						return 0;
					}

				case CLASS_TIM:
					ClassOut = 0x0D;
					switch (msgtype)
					{
					case TIM_TM:
						IdOut = 0x02;
						return 0;
					case TIM_TM2:
						IdOut = 0x03;
						return 0;
					case TIM_TP:
						IdOut = 0x01;
						return 0;
					case TIM_SVIN:
						IdOut = 0x04;
						return 0;
					}
				case CLASS_NMEA:
					ClassOut = 0xF0;
					switch (msgtype)
					{
					case NMEA_GGA:
						IdOut = 0x00;
						return 0;
					case NMEA_GLL:
						IdOut = 0x01;
						return 0;
					case NMEA_GSA:
						IdOut = 0x02;
						return 0;
					case NMEA_GSV:
						IdOut = 0x03;
						return 0;
					case NMEA_RMC:
						IdOut = 0x04;
						return 0;
					case NMEA_VTG:
						IdOut = 0x05;
						return 0;
					case NMEA_GRS:
						IdOut = 0x06;
						return 0;
					case NMEA_GST:
						IdOut = 0x07;
						return 0;
					case NMEA_ZDA:
						IdOut = 0x08;
						return 0;
					case NMEA_GBS:
						IdOut = 0x09;
						return 0;
					case NMEA_DTM:
						IdOut = 0x0A;
						return 0;
					}

				case CLASS_PUBX:
					ClassOut = 0xF1;
					switch (msgtype)
					{
					case PUBX_A:
						IdOut = 0x00;
						return 0;
					case PUBX_B:
						IdOut = 0x01;
						return 0;
					case PUBX_C:
						IdOut = 0x03;
						return 0;
					case PUBX_D:
						IdOut = 0x04;
						return 0;
					}
					// return 0;
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			return 1;

		}

		private int getMsg(string mclass, string msgtype)
		{
			try
			{
				if (mclass.Equals("NAV"))
				{
					ClassOut = CLASS_NAV;
					if (msgtype.Equals("POSECEF"))
					{
						IdOut = NAV_POSECEF;
						return 0;
					}
					else if (msgtype.Equals("POSLLH"))
					{
						IdOut = NAV_POSLLH;
						return 0;
					}
					else if (msgtype.Equals("POSUTM"))
					{
						IdOut = NAV_POSUTM;
						return 0;
					}
					else if (msgtype.Equals("DOP"))
					{
						IdOut = NAV_DOP;
						return 0;
					}
					else if (msgtype.Equals("STATUS"))
					{
						IdOut = NAV_STATUS;
						return 0;
					}
					else if (msgtype.Equals("SOL"))
					{
						IdOut = NAV_SOL;
						return 0;
					}
					else if (msgtype.Equals("VELECEF"))
					{
						IdOut = NAV_VELECEF;
						return 0;
					}
					else if (msgtype.Equals("VELNED"))
					{
						IdOut = NAV_VELNED;
						return 0;
					}
					else if (msgtype.Equals("TIMEGPS"))
					{
						IdOut = NAV_TIMEGPS;
						return 0;
					}
					else if (msgtype.Equals("TIMEUTC"))
					{
						IdOut = NAV_TIMEUTC;
						return 0;
					}
					else if (msgtype.Equals("CLOCK"))
					{
						IdOut = NAV_CLOCK;
						return 0;
					}
					else if (msgtype.Equals("SVINFO"))
					{
						IdOut = NAV_SVINFO;
						return 0;
					}
					else if (msgtype.Equals("DGPS"))
					{
						IdOut = NAV_DGPS;
						return 0;
					}
					else if (msgtype.Equals("SBAS"))
					{
						IdOut = NAV_SBAS;
						return 0;
					}
					else if (msgtype.Equals("EKFSTATUS"))
					{
						IdOut = NAV_EKFSTATUS;
						return 0;
					}
				}
				else if (mclass.Equals("RXM"))
				{
					ClassOut = CLASS_RXM;
					if (msgtype.Equals("RAW"))
					{
						IdOut = RXM_RAW;
						return 0;
					}
					else if (msgtype.Equals("SVSI"))
					{
						IdOut = RXM_SVSI;
						return 0;
					}
					else if (msgtype.Equals("SFRB"))
					{
						IdOut = RXM_SFRB;
						return 0;
					}
					else if (msgtype.Equals("ALM"))
					{
						IdOut = RXM_ALM;
						return 0;
					}
					else if (msgtype.Equals("EPH"))
					{
						IdOut = RXM_EPH;
						return 0;
					}
					else if (msgtype.Equals("POSREQ"))
					{
						IdOut = RXM_POSREQ;
						return 0;
					}
					else if (msgtype.Equals("PMREQ"))
					{
						IdOut = RXM_PMREQ;
						return 0;
					}

				}
				else if (mclass.Equals("INF"))
				{
					ClassOut = CLASS_INF;
					if (msgtype.Equals("ERROR"))
					{
						IdOut = INF_ERROR;
						return 0;
					}
					else if (msgtype.Equals("WARNING"))
					{
						IdOut = INF_WARNING;
						return 0;
					}
					else if (msgtype.Equals("NOTICE"))
					{
						IdOut = INF_NOTICE;
						return 0;
					}
					else if (msgtype.Equals("TEST"))
					{
						IdOut = INF_TEST;
						return 0;
					}
					else if (msgtype.Equals("DEBUG"))
					{
						IdOut = INF_DEBUG;
						return 0;
					}
					else if (msgtype.Equals("USER"))
					{
						IdOut = INF_USER;
						return 0;
					}
				}
				else if (mclass.Equals("ACK"))
				{
					ClassOut = CLASS_ACK;
					if (msgtype.Equals("ACK"))
					{
						IdOut = ACK_ACK;
						return 0;
					}
					else if (msgtype.Equals("NAK"))
					{
						IdOut = ACK_NAK;
						return 0;
					}

				}
				else if (mclass.Equals("CFG"))
				{
					ClassOut = CLASS_CFG;
					if (msgtype.Equals("PRT"))
					{
						IdOut = CFG_PRT;
						return 0;
					}
					else if (msgtype.Equals("USB"))
					{
						IdOut = CFG_USB;
						return 0;
					}
					else if (msgtype.Equals("MSG"))
					{
						IdOut = CFG_MSG;
						return 0;
					}
					else if (msgtype.Equals("NMEA"))
					{
						IdOut = CFG_NMEA;
						return 0;
					}
					else if (msgtype.Equals("RATE"))
					{
						IdOut = CFG_RATE;
						return 0;
					}
					else if (msgtype.Equals("CFG"))
					{
						IdOut = CFG_CFG;
						return 0;
					}
					else if (msgtype.Equals("TP"))
					{
						IdOut = CFG_TP;
						return 0;
					}
					else if (msgtype.Equals("NAV2"))
					{
						IdOut = CFG_NAV2;
						return 0;
					}
					else if (msgtype.Equals("DAT"))
					{
						IdOut = CFG_DAT;
						return 0;
					}
					else if (msgtype.Equals("INF"))
					{
						IdOut = CFG_INF;
						return 0;
					}
					else if (msgtype.Equals("RST"))
					{
						IdOut = CFG_RST;
						return 0;
					}
					else if (msgtype.Equals("RXM"))
					{
						IdOut = CFG_RXM;
						return 0;
					}
					else if (msgtype.Equals("ANT"))
					{
						IdOut = CFG_ANT;
						return 0;
					}
					else if (msgtype.Equals("FXN"))
					{
						IdOut = CFG_FXN;
						return 0;
					}
					else if (msgtype.Equals("SBAS"))
					{
						IdOut = CFG_SBAS;
						return 0;
					}
					else if (msgtype.Equals("LIC"))
					{
						IdOut = CFG_LIC;
						return 0;
					}
					else if (msgtype.Equals("TM"))
					{
						IdOut = CFG_TM;
						return 0;
					}
					else if (msgtype.Equals("TM2"))
					{
						IdOut = CFG_TM2;
						return 0;
					}
					else if (msgtype.Equals("TMODE"))
					{
						IdOut = CFG_TMODE;
						return 0;
					}
					else if (msgtype.Equals("EKF"))
					{
						IdOut = CFG_EKF;
						return 0;
					}

				}
				else if (mclass.Equals("UPD"))
				{
					ClassOut = CLASS_UPD;
					if (msgtype.Equals("DOWNL"))
					{
						IdOut = UPD_DOWNL;
						return 0;
					}
					else if (msgtype.Equals("UPLOAD"))
					{
						IdOut = UPD_UPLOAD;
						return 0;
					}
					else if (msgtype.Equals("EXEC"))
					{
						IdOut = UPD_EXEC;
						return 0;
					}
					else if (msgtype.Equals("MEMCPY"))
					{
						IdOut = UPD_MEMCPY;
						return 0;
					}

				}
				else if (mclass.Equals("MON"))
				{
					ClassOut = CLASS_MON;
					if (msgtype.Equals("SCHD"))
					{
						IdOut = MON_SCHD;
						return 0;
					}
					else if (msgtype.Equals("IO"))
					{
						IdOut = MON_IO;
						return 0;
					}
					else if (msgtype.Equals("MSGPP"))
					{
						IdOut = MON_MSGPP;
						return 0;
					}
					else if (msgtype.Equals("RXBUF"))
					{
						IdOut = MON_RXBUF;
						return 0;
					}
					else if (msgtype.Equals("TXBUF"))
					{
						IdOut = MON_TXBUF;
						return 0;
					}
					else if (msgtype.Equals("HW"))
					{
						IdOut = MON_HW;
						return 0;
					}
					else if (msgtype.Equals("IPC"))
					{
						IdOut = MON_IPC;
						return 0;
					}
					else if (msgtype.Equals("USB"))
					{
						IdOut = MON_USB;
						return 0;
					}
					else if (msgtype.Equals("VER"))
					{
						IdOut = MON_VER;
						return 0;
					}
					else if (msgtype.Equals("EXCEPT"))
					{
						IdOut = MON_EXCEPT;
						return 0;
					}

				}
				else if (mclass.Equals("AID"))
				{
					ClassOut = CLASS_AID;
					if (msgtype.Equals("REQ"))
					{
						IdOut = AID_REQ;
						return 0;
					}
					else if (msgtype.Equals("DATA"))
					{
						IdOut = AID_DATA;
						return 0;
					}
					else if (msgtype.Equals("INI"))
					{
						IdOut = AID_INI;
						return 0;
					}
					else if (msgtype.Equals("HUI"))
					{
						IdOut = AID_HUI;
						return 0;
					}
					else if (msgtype.Equals("ALM"))
					{
						IdOut = AID_ALM;
						return 0;
					}
					else if (msgtype.Equals("EPH"))
					{
						IdOut = AID_EPH;
						return 0;
					}

				}
				else if (mclass.Equals("TIM"))
				{
					ClassOut = CLASS_TIM;
					if (msgtype.Equals("TM"))
					{
						IdOut = TIM_TM;
						return 0;
					}
					else if (msgtype.Equals("TM2"))
					{
						IdOut = TIM_TM2;
						return 0;
					}
					else if (msgtype.Equals("TP"))
					{
						IdOut = TIM_TP;
						return 0;
					}
					else if (msgtype.Equals("SVIN"))
					{
						IdOut = TIM_SVIN;
						return 0;
					}
				}
				else if (mclass.Equals("NMEA"))
				{
					ClassOut = CLASS_NMEA;
					if (msgtype.Equals("GGA"))
					{
						IdOut = NMEA_GGA;
						return 0;
					}
					else if (msgtype.Equals("GLL"))
					{
						IdOut = NMEA_GLL;
						return 0;
					}
					else if (msgtype.Equals("GSA"))
					{
						IdOut = NMEA_GSA;
						return 0;
					}
					else if (msgtype.Equals("GSV"))
					{
						IdOut = NMEA_GSV;
						return 0;
					}
					else if (msgtype.Equals("RMC"))
					{
						IdOut = NMEA_RMC;
						return 0;
					}
					else if (msgtype.Equals("VTG"))
					{
						IdOut = NMEA_VTG;
						return 0;
					}
					else if (msgtype.Equals("GRS"))
					{
						IdOut = NMEA_GRS;
						return 0;
					}
					else if (msgtype.Equals("GST"))
					{
						IdOut = NMEA_GST;
						return 0;
					}
					else if (msgtype.Equals("ZDA"))
					{
						IdOut = NMEA_ZDA;
						return 0;
					}
					else if (msgtype.Equals("GBS"))
					{
						IdOut = NMEA_GBS;
						return 0;
					}
					else if (msgtype.Equals("DTM"))
					{
						IdOut = NMEA_DTM;
						return 0;
					}

				}
				else if (mclass.Equals("PUBX"))
				{
					ClassOut = CLASS_PUBX;
					if (msgtype.Equals("A"))
					{
						IdOut = PUBX_A;
						return 0;
					}
					else if (msgtype.Equals("B"))
					{
						IdOut = PUBX_B;
						return 0;
					}
					else if (msgtype.Equals("C"))
					{
						IdOut = PUBX_C;
						return 0;
					}
					else if (msgtype.Equals("D"))
					{
						IdOut = PUBX_D;
						return 0;
					}
					// return 0;
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			return 1;

		}

	}

}