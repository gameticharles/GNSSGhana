using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ghGPS.Classes.rcv
{
    public static class GlobalMembersBinex
    {
        /*------------------------------------------------------------------------------
        * binex.c : binex dependent functions
        *
        *          Copyright (C) 2013 by T.TAKASU, All rights reserved.
        *
        * reference :
        *     [1] UNAVCO, BINEX: Binary exchange format
        *         (http://binex.unavco.org/binex.html)
        *
        * version : $Revision:$ $Date:$
        * history : 2013/02/20 1.0 new
        *           2013/04/15 1.1 support 0x01-05 beidou-2/compass ephemeris
        *           2013/05/18 1.2 fix bug on decoding obsflags in message 0x7f-05
        *           2014/04/27 1.3 fix bug on decoding iode for message 0x01-02
        *-----------------------------------------------------------------------------*/


        /* ura table -----------------------------------------------------------------*/
        internal readonly double[] ura_eph = { 2.4, 3.4, 4.85, 6.85, 9.65, 13.65, 24.0, 48.0, 96.0, 192.0, 384.0, 768.0, 1536.0, 3072.0, 6144.0, 0.0 };


        //C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on the parameter 'p', so pointers on this parameter are left unchanged:
        internal static ushort U2(byte* p)
        {
            ushort value;
            //C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on this variable, so pointers on this variable are left unchanged:
            byte* q = (byte)&value + 1;
            int i;
            for (i = 0; i < 2; i++)
            {
                *q-- = *p++;
            }
            return value;
        }
        //C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on the parameter 'p', so pointers on this parameter are left unchanged:
        internal static uint U4(byte* p)
        {
            uint value;
            //C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on this variable, so pointers on this variable are left unchanged:
            byte* q = (byte)&value + 3;
            int i;
            for (i = 0; i < 4; i++)
            {
                *q-- = *p++;
            }
            return value;
        }
        internal static int I4(ref byte p)
        {
            return (int)GlobalMembersBinex.U4(p);
        }
        //C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on the parameter 'p', so pointers on this parameter are left unchanged:
        internal static float R4(byte* p)
        {
            float value;
            //C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on this variable, so pointers on this variable are left unchanged:
            byte* q = (byte)&value + 3;
            int i;
            for (i = 0; i < 4; i++)
            {
                *q-- = *p++;
            }
            return value;
        }
        //C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on the parameter 'p', so pointers on this parameter are left unchanged:
        internal static double R8(byte* p)
        {
            double value;
            //C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on this variable, so pointers on this variable are left unchanged:
            byte* q = (byte)&value + 7;
            int i;
            for (i = 0; i < 8; i++)
            {
                *q-- = *p++;
            }
            return value;
        }
        /* get binex 1-4 byte unsigned integer (big endian) --------------------------*/
        internal static int getbnxi(byte[] p, ref uint val)
        {
            int i;

            for (val = 0, i = 0; i < 3; i++)
            {
                val = (val << 7) + (p[i] & 0x7F);
                if (!(p[i] & 0x80))
                {
                    return i + 1;
                }
            }
            val = (val << 8) + p[i];
            return 4;
        }
        /* checksum 8 parity ---------------------------------------------------------*/
        internal static byte csum8(byte[] buff, int len)
        {
            byte cs = 0;
            int i;

            for (i = 0; i < len; i++)
            {
                cs ^= buff[i];
            }
            return cs;
        }
        /* adjust weekly rollover of gps time ----------------------------------------*/
        internal static gtime_t adjweek(gtime_t time, double tow)
        {
            double tow_p;
            int week;
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: tow_p=time2gpst(time,&week);
            tow_p = GlobalMembersRtkcmn.time2gpst(new gtime_t(time), ref week);
            if (tow < tow_p - 302400.0)
            {
                tow += 604800.0;
            }
            else if (tow > tow_p + 302400.0)
            {
                tow -= 604800.0;
            }
            return GlobalMembersRtkcmn.gpst2time(week, tow);
        }
        /* adjust daily rollover of time ---------------------------------------------*/
        internal static gtime_t adjday(gtime_t time, double tod)
        {
            double[] ep = new double[6];
            double tod_p;
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: time2epoch(time,ep);
            GlobalMembersRtkcmn.time2epoch(new gtime_t(time), ep);
            tod_p = ep[3] * 3600.0 + ep[4] * 60.0 + ep[5];
            if (tod < tod_p - 43200.0)
            {
                tod += 86400.0;
            }
            else if (tod > tod_p + 43200.0)
            {
                tod -= 86400.0;
            }
            ep[3] = ep[4] = ep[5] = 0.0;
            return GlobalMembersRtkcmn.timeadd(GlobalMembersRtkcmn.epoch2time(ep), tod);
        }
        /* ura value (m) to ura index ------------------------------------------------*/
        internal static int uraindex(double value)
        {
            int i;
            for (i = 0; i < 15; i++)
            {
                if (ura_eph[i] >= value)
                    break;
            }
            return i;
        }
        /* decode binex mesaage 0x00-00: comment -------------------------------------*/
        internal static int decode_bnx_00_00(raw_t raw, ref byte buff, int len)
        {
            GlobalMembersRtkcmn.trace(2, "binex 0x00-00: not supported message\n");
            return 0;
        }
        /* decode binex mesaage 0x00-01: program or software package -----------------*/
        internal static int decode_bnx_00_01(raw_t raw, ref byte buff, int len)
        {
            GlobalMembersRtkcmn.trace(2, "binex 0x00-01: not supported message\n");
            return 0;
        }
        /* decode binex mesaage 0x00-02: program operator ----------------------------*/
        internal static int decode_bnx_00_02(raw_t raw, ref byte buff, int len)
        {
            GlobalMembersRtkcmn.trace(2, "binex 0x00-02: not supported message\n");
            return 0;
        }
        /* decode binex mesaage 0x00-03: reserved ------------------------------------*/
        internal static int decode_bnx_00_03(raw_t raw, ref byte buff, int len)
        {
            GlobalMembersRtkcmn.trace(2, "binex 0x00-03: not supported message\n");
            return 0;
        }
        /* decode binex mesaage 0x00-04: site name/description -----------------------*/
        internal static int decode_bnx_00_04(raw_t raw, ref byte buff, int len)
        {
            GlobalMembersRtkcmn.trace(2, "binex 0x00-04: not supported message\n");
            return 0;
        }
        /* decode binex mesaage 0x00-05: site number ---------------------------------*/
        internal static int decode_bnx_00_05(raw_t raw, ref byte buff, int len)
        {
            GlobalMembersRtkcmn.trace(2, "binex 0x00-05: not supported message\n");
            return 0;
        }
        /* decode binex mesaage 0x00-06: monumnent name ------------------------------*/
        internal static int decode_bnx_00_06(raw_t raw, ref byte buff, int len)
        {
            GlobalMembersRtkcmn.trace(2, "binex 0x00-06: not supported message\n");
            return 0;
        }
        /* decode binex mesaage 0x00-07: monumnent number ----------------------------*/
        internal static int decode_bnx_00_07(raw_t raw, ref byte buff, int len)
        {
            GlobalMembersRtkcmn.trace(2, "binex 0x00-07: not supported message\n");
            return 0;
        }
        /* decode binex mesaage 0x00-08: marker name ---------------------------------*/
        internal static int decode_bnx_00_08(raw_t raw, ref byte buff, int len)
        {
            GlobalMembersRtkcmn.trace(2, "binex 0x00-08: not supported message\n");
            return 0;
        }
        /* decode binex mesaage 0x00-09: marker number -------------------------------*/
        internal static int decode_bnx_00_09(raw_t raw, ref byte buff, int len)
        {
            GlobalMembersRtkcmn.trace(2, "binex 0x00-09: not supported message\n");
            return 0;
        }
        /* decode binex mesaage 0x00-0a: reference point name ------------------------*/
        internal static int decode_bnx_00_0a(raw_t raw, ref byte buff, int len)
        {
            GlobalMembersRtkcmn.trace(2, "binex 0x00-0a: not supported message\n");
            return 0;
        }
        /* decode binex mesaage 0x00-0b: reference point number ----------------------*/
        internal static int decode_bnx_00_0b(raw_t raw, ref byte buff, int len)
        {
            GlobalMembersRtkcmn.trace(2, "binex 0x00-0b: not supported message\n");
            return 0;
        }
        /* decode binex mesaage 0x00-0c: date esttablished ---------------------------*/
        internal static int decode_bnx_00_0c(raw_t raw, ref byte buff, int len)
        {
            GlobalMembersRtkcmn.trace(2, "binex 0x00-0c: not supported message\n");
            return 0;
        }
        /* decode binex mesaage 0x00-0d: reserved ------------------------------------*/
        internal static int decode_bnx_00_0d(raw_t raw, ref byte buff, int len)
        {
            GlobalMembersRtkcmn.trace(2, "binex 0x00-0d: not supported message\n");
            return 0;
        }
        /* decode binex mesaage 0x00-0e: reserved ------------------------------------*/
        internal static int decode_bnx_00_0e(raw_t raw, ref byte buff, int len)
        {
            GlobalMembersRtkcmn.trace(2, "binex 0x00-0e: not supported message\n");
            return 0;
        }
        /* decode binex mesaage 0x00-0f: 4-character id ------------------------------*/
        internal static int decode_bnx_00_0f(raw_t raw, ref byte buff, int len)
        {
            GlobalMembersRtkcmn.trace(2, "binex 0x00-0f: not supported message\n");
            return 0;
        }
        /* decode binex mesaage 0x00-10: project name --------------------------------*/
        internal static int decode_bnx_00_10(raw_t raw, ref byte buff, int len)
        {
            GlobalMembersRtkcmn.trace(2, "binex 0x00-10: not supported message\n");
            return 0;
        }
        /* decode binex mesaage 0x00-11: principal investigator for this project -----*/
        internal static int decode_bnx_00_11(raw_t raw, ref byte buff, int len)
        {
            GlobalMembersRtkcmn.trace(2, "binex 0x00-11: not supported message\n");
            return 0;
        }
        /* decode binex mesaage 0x00-12: pi's agency/institution ---------------------*/
        internal static int decode_bnx_00_12(raw_t raw, ref byte buff, int len)
        {
            GlobalMembersRtkcmn.trace(2, "binex 0x00-12: not supported message\n");
            return 0;
        }
        /* decode binex mesaage 0x00-13: pi's contact information --------------------*/
        internal static int decode_bnx_00_13(raw_t raw, ref byte buff, int len)
        {
            GlobalMembersRtkcmn.trace(2, "binex 0x00-13: not supported message\n");
            return 0;
        }
        /* decode binex mesaage 0x00-14: site operator -------------------------------*/
        internal static int decode_bnx_00_14(raw_t raw, ref byte buff, int len)
        {
            GlobalMembersRtkcmn.trace(2, "binex 0x00-14: not supported message\n");
            return 0;
        }
        /* decode binex mesaage 0x00-15: site operator's agency/institution ----------*/
        internal static int decode_bnx_00_15(raw_t raw, ref byte buff, int len)
        {
            GlobalMembersRtkcmn.trace(2, "binex 0x00-15: not supported message\n");
            return 0;
        }
        /* decode binex mesaage 0x00-16: site operator's contact information ---------*/
        internal static int decode_bnx_00_16(raw_t raw, ref byte buff, int len)
        {
            GlobalMembersRtkcmn.trace(2, "binex 0x00-16: not supported message\n");
            return 0;
        }
        /* decode binex mesaage 0x00-17: antenna type --------------------------------*/
        internal static int decode_bnx_00_17(raw_t raw, ref byte buff, int len)
        {
            GlobalMembersRtkcmn.trace(2, "binex 0x00-17: not supported message\n");
            return 0;
        }
        /* decode binex mesaage 0x00-18: antenna number ------------------------------*/
        internal static int decode_bnx_00_18(raw_t raw, ref byte buff, int len)
        {
            GlobalMembersRtkcmn.trace(2, "binex 0x00-18: not supported message\n");
            return 0;
        }
        /* decode binex mesaage 0x00-19: receiver type -------------------------------*/
        internal static int decode_bnx_00_19(raw_t raw, ref byte buff, int len)
        {
            GlobalMembersRtkcmn.trace(2, "binex 0x00-19: not supported message\n");
            return 0;
        }
        /* decode binex mesaage 0x00-1a: receiver number -----------------------------*/
        internal static int decode_bnx_00_1a(raw_t raw, ref byte buff, int len)
        {
            GlobalMembersRtkcmn.trace(2, "binex 0x00-1a: not supported message\n");
            return 0;
        }
        /* decode binex mesaage 0x00-1b: receiver firmware version -------------------*/
        internal static int decode_bnx_00_1b(raw_t raw, ref byte buff, int len)
        {
            GlobalMembersRtkcmn.trace(2, "binex 0x00-1b: not supported message\n");
            return 0;
        }
        /* decode binex mesaage 0x00-1c: antenna mount description -------------------*/
        internal static int decode_bnx_00_1c(raw_t raw, ref byte buff, int len)
        {
            GlobalMembersRtkcmn.trace(2, "binex 0x00-1c: not supported message\n");
            return 0;
        }
        /* decode binex mesaage 0x00-1d: antenna xyz position ------------------------*/
        internal static int decode_bnx_00_1d(raw_t raw, ref byte buff, int len)
        {
            GlobalMembersRtkcmn.trace(2, "binex 0x00-1d: not supported message\n");
            return 0;
        }
        /* decode binex mesaage 0x00-1e: antenna geographic position -----------------*/
        internal static int decode_bnx_00_1e(raw_t raw, ref byte buff, int len)
        {
            GlobalMembersRtkcmn.trace(2, "binex 0x00-1e: not supported message\n");
            return 0;
        }
        /* decode binex mesaage 0x00-1f: antenna offset from reference point ---------*/
        internal static int decode_bnx_00_1f(raw_t raw, ref byte buff, int len)
        {
            GlobalMembersRtkcmn.trace(2, "binex 0x00-1f: not supported message\n");
            return 0;
        }
        /* decode binex mesaage 0x00-20: antenna radome type -------------------------*/
        internal static int decode_bnx_00_20(raw_t raw, ref byte buff, int len)
        {
            GlobalMembersRtkcmn.trace(2, "binex 0x00-20: not supported message\n");
            return 0;
        }
        /* decode binex mesaage 0x00-21: antenna radome number -----------------------*/
        internal static int decode_bnx_00_21(raw_t raw, ref byte buff, int len)
        {
            GlobalMembersRtkcmn.trace(2, "binex 0x00-21: not supported message\n");
            return 0;
        }
        /* decode binex mesaage 0x00-22: geocode -------------------------------------*/
        internal static int decode_bnx_00_22(raw_t raw, ref byte buff, int len)
        {
            GlobalMembersRtkcmn.trace(2, "binex 0x00-22: not supported message\n");
            return 0;
        }
        /* decode binex mesaage 0x00-7f: notes/additional information ----------------*/
        internal static int decode_bnx_00_7f(raw_t raw, ref byte buff, int len)
        {
            GlobalMembersRtkcmn.trace(2, "binex 0x00-7f: not supported message\n");
            return 0;
        }
        /* decode binex mesaage 0x00: site/monument/marker/ref point/setup metadata --*/
        internal static int decode_bnx_00(raw_t raw, ref byte buff, int len)
        {
            double[] gpst0 = { 1980, 1, 6, 0, 0, 0 };
            string msg;
            //C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on this variable, so pointers on this variable are left unchanged:
            byte* p = buff;
            uint min;
            uint qsec;
            uint src;
            uint fid;
            int n = 6;

            min = GlobalMembersBinex.U4(p);
            p += 4;
            qsec = ((byte)(p));
            p += 1;
            src = ((byte)(p));
            p += 1;
            n += GlobalMembersBinex.getbnxi(p, ref fid);
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: raw->time=timeadd(epoch2time(gpst0),min *60.0+qsec *0.25);
            raw.time.CopyFrom(GlobalMembersRtkcmn.timeadd(GlobalMembersRtkcmn.epoch2time(gpst0), min * 60.0 + qsec * 0.25));

            if (raw.outtype != 0)
            {
                msg = raw.msgtype.Substring(raw.msgtype.Length);
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: sprintf(msg," fid=%02X time=%s src=%d",fid,time_str(raw->time,0),src);
                msg = string.Format(" fid={0:X2} time={1} src={2:D}", fid, GlobalMembersRtkcmn.time_str(new gtime_t(raw.time), 0), src);
            }
            switch (fid)
            {
                case 0x00:
                    return GlobalMembersBinex.decode_bnx_00_00(raw, ref buff + n, len - n);
                case 0x01:
                    return GlobalMembersBinex.decode_bnx_00_01(raw, ref buff + n, len - n);
                case 0x02:
                    return GlobalMembersBinex.decode_bnx_00_02(raw, ref buff + n, len - n);
                case 0x03:
                    return GlobalMembersBinex.decode_bnx_00_03(raw, ref buff + n, len - n);
                case 0x04:
                    return GlobalMembersBinex.decode_bnx_00_04(raw, ref buff + n, len - n);
                case 0x05:
                    return GlobalMembersBinex.decode_bnx_00_05(raw, ref buff + n, len - n);
                case 0x06:
                    return GlobalMembersBinex.decode_bnx_00_06(raw, ref buff + n, len - n);
                case 0x07:
                    return GlobalMembersBinex.decode_bnx_00_07(raw, ref buff + n, len - n);
                case 0x08:
                    return GlobalMembersBinex.decode_bnx_00_08(raw, ref buff + n, len - n);
                case 0x09:
                    return GlobalMembersBinex.decode_bnx_00_09(raw, ref buff + n, len - n);
                case 0x0A:
                    return GlobalMembersBinex.decode_bnx_00_0a(raw, ref buff + n, len - n);
                case 0x0B:
                    return GlobalMembersBinex.decode_bnx_00_0b(raw, ref buff + n, len - n);
                case 0x0C:
                    return GlobalMembersBinex.decode_bnx_00_0c(raw, ref buff + n, len - n);
                case 0x0D:
                    return GlobalMembersBinex.decode_bnx_00_0d(raw, ref buff + n, len - n);
                case 0x0E:
                    return GlobalMembersBinex.decode_bnx_00_0e(raw, ref buff + n, len - n);
                case 0x0F:
                    return GlobalMembersBinex.decode_bnx_00_0f(raw, ref buff + n, len - n);
                case 0x10:
                    return GlobalMembersBinex.decode_bnx_00_10(raw, ref buff + n, len - n);
                case 0x11:
                    return GlobalMembersBinex.decode_bnx_00_11(raw, ref buff + n, len - n);
                case 0x12:
                    return GlobalMembersBinex.decode_bnx_00_12(raw, ref buff + n, len - n);
                case 0x13:
                    return GlobalMembersBinex.decode_bnx_00_13(raw, ref buff + n, len - n);
                case 0x14:
                    return GlobalMembersBinex.decode_bnx_00_14(raw, ref buff + n, len - n);
                case 0x15:
                    return GlobalMembersBinex.decode_bnx_00_15(raw, ref buff + n, len - n);
                case 0x16:
                    return GlobalMembersBinex.decode_bnx_00_16(raw, ref buff + n, len - n);
                case 0x17:
                    return GlobalMembersBinex.decode_bnx_00_17(raw, ref buff + n, len - n);
                case 0x18:
                    return GlobalMembersBinex.decode_bnx_00_18(raw, ref buff + n, len - n);
                case 0x19:
                    return GlobalMembersBinex.decode_bnx_00_19(raw, ref buff + n, len - n);
                case 0x1A:
                    return GlobalMembersBinex.decode_bnx_00_1a(raw, ref buff + n, len - n);
                case 0x1B:
                    return GlobalMembersBinex.decode_bnx_00_1b(raw, ref buff + n, len - n);
                case 0x1C:
                    return GlobalMembersBinex.decode_bnx_00_1c(raw, ref buff + n, len - n);
                case 0x1D:
                    return GlobalMembersBinex.decode_bnx_00_1d(raw, ref buff + n, len - n);
                case 0x1E:
                    return GlobalMembersBinex.decode_bnx_00_1e(raw, ref buff + n, len - n);
                case 0x1F:
                    return GlobalMembersBinex.decode_bnx_00_1f(raw, ref buff + n, len - n);
                case 0x20:
                    return GlobalMembersBinex.decode_bnx_00_20(raw, ref buff + n, len - n);
                case 0x21:
                    return GlobalMembersBinex.decode_bnx_00_21(raw, ref buff + n, len - n);
                case 0x22:
                    return GlobalMembersBinex.decode_bnx_00_22(raw, ref buff + n, len - n);
                case 0x7F:
                    return GlobalMembersBinex.decode_bnx_00_7f(raw, ref buff + n, len - n);
            }
            return 0;
        }
        /* decode binex mesaage 0x01-00: coded (raw bytes) gnss ephemeris ------------*/
        internal static int decode_bnx_01_00(raw_t raw, ref byte buff, int len)
        {
            GlobalMembersRtkcmn.trace(2, "binex 0x01-00: not supported message\n");
            return 0;
        }
        /* decode binex mesaage 0x01-01: decoded gps ephmemeris ----------------------*/
        internal static int decode_bnx_01_01(raw_t raw, ref byte buff, int len)
        {
            eph_t eph = new eph_t();
            //C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on this variable, so pointers on this variable are left unchanged:
            byte* p = buff;
            double tow;
            double ura;
            double sqrtA;
            int prn;
            int flag;

            GlobalMembersRtkcmn.trace(4, "binex 0x01-01: len=%d\n", len);

            if (len >= 127)
            {
                prn = ((byte)(p)) + 1;
                p += 1;
                eph.week = GlobalMembersBinex.U2(p);
                p += 2;
                tow = GlobalMembersBinex.I4(ref p);
                p += 4;
                eph.toes = GlobalMembersBinex.I4(ref p);
                p += 4;
                eph.tgd[0] = GlobalMembersBinex.R4(p);
                p += 4;
                eph.iodc = GlobalMembersBinex.I4(ref p);
                p += 4;
                eph.f2 = GlobalMembersBinex.R4(p);
                p += 4;
                eph.f1 = GlobalMembersBinex.R4(p);
                p += 4;
                eph.f0 = GlobalMembersBinex.R4(p);
                p += 4;
                eph.iode = GlobalMembersBinex.I4(ref p);
                p += 4;
                eph.deln = GlobalMembersBinex.R4(p) * DefineConstants.SC2RAD;
                p += 4;
                eph.M0 = GlobalMembersBinex.R8(p);
                p += 8;
                eph.e = GlobalMembersBinex.R8(p);
                p += 8;
                sqrtA = GlobalMembersBinex.R8(p);
                p += 8;
                eph.cic = GlobalMembersBinex.R4(p);
                p += 4;
                eph.crc = GlobalMembersBinex.R4(p);
                p += 4;
                eph.cis = GlobalMembersBinex.R4(p);
                p += 4;
                eph.crs = GlobalMembersBinex.R4(p);
                p += 4;
                eph.cuc = GlobalMembersBinex.R4(p);
                p += 4;
                eph.cus = GlobalMembersBinex.R4(p);
                p += 4;
                eph.OMG0 = GlobalMembersBinex.R8(p);
                p += 8;
                eph.omg = GlobalMembersBinex.R8(p);
                p += 8;
                eph.i0 = GlobalMembersBinex.R8(p);
                p += 8;
                eph.OMGd = GlobalMembersBinex.R4(p) * DefineConstants.SC2RAD;
                p += 4;
                eph.idot = GlobalMembersBinex.R4(p) * DefineConstants.SC2RAD;
                p += 4;
                ura = GlobalMembersBinex.R4(p) * 0.1;
                p += 4;
                eph.svh = GlobalMembersBinex.U2(p);
                p += 2;
                flag = GlobalMembersBinex.U2(p);
            }
            else
            {
                GlobalMembersRtkcmn.trace(2, "binex 0x01-01: length error len=%d\n", len);
                return -1;
            }
            if ((eph.sat = GlobalMembersRtkcmn.satno(DefineConstants.SYS_GPS, prn)) == 0)
            {
                GlobalMembersRtkcmn.trace(2, "binex 0x01-01: satellite error prn=%d\n", prn);
                return -1;
            }
            eph.A = sqrtA * sqrtA;
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: eph.toe=gpst2time(eph.week,eph.toes);
            eph.toe.CopyFrom(GlobalMembersRtkcmn.gpst2time(eph.week, eph.toes));
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: eph.toc=gpst2time(eph.week,eph.toes);
            eph.toc.CopyFrom(GlobalMembersRtkcmn.gpst2time(eph.week, eph.toes));
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: eph.ttr=adjweek(eph.toe,tow);
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            eph.ttr.CopyFrom(GlobalMembersBinex.adjweek(new gtime_t(eph.toe), tow));
            eph.fit = flag & 0xFF;
            eph.flag = (flag >> 8) & 0x01;
            eph.code = (flag >> 9) & 0x03;
            eph.sva = GlobalMembersBinex.uraindex(ura);

            if (!StringFunctions.StrStr(raw.opt, "-EPHALL"))
            {
                if (raw.nav.eph[eph.sat - 1].iode == eph.iode && raw.nav.eph[eph.sat - 1].iodc == eph.iodc) // unchanged
                {
                    return 0;
                }
            }
            raw.nav.eph[eph.sat - 1] = eph;
            raw.ephsat = eph.sat;
            return 2;
        }
        /* decode binex mesaage 0x01-02: decoded glonass ephmemeris ------------------*/
        internal static int decode_bnx_01_02(raw_t raw, ref byte buff, int len)
        {
            geph_t geph = new geph_t();
            //C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on this variable, so pointers on this variable are left unchanged:
            byte* p = buff;
            double tod;
            double tof;
            double tau_gps;
            int prn;
            int day;
            int leap;

            GlobalMembersRtkcmn.trace(4, "binex 0x01-02: len=%d\n", len);

            if (len >= 119)
            {
                prn = ((byte)(p)) + 1;
                p += 1;
                day = GlobalMembersBinex.U2(p);
                p += 2;
                tod = GlobalMembersBinex.U4(p);
                p += 4;
                geph.taun = -GlobalMembersBinex.R8(p);
                p += 8;
                geph.gamn = GlobalMembersBinex.R8(p);
                p += 8;
                tof = GlobalMembersBinex.U4(p);
                p += 4;
                geph.pos[0] = GlobalMembersBinex.R8(p) * 1E3;
                p += 8;
                geph.vel[0] = GlobalMembersBinex.R8(p) * 1E3;
                p += 8;
                geph.acc[0] = GlobalMembersBinex.R8(p) * 1E3;
                p += 8;
                geph.pos[1] = GlobalMembersBinex.R8(p) * 1E3;
                p += 8;
                geph.vel[1] = GlobalMembersBinex.R8(p) * 1E3;
                p += 8;
                geph.acc[1] = GlobalMembersBinex.R8(p) * 1E3;
                p += 8;
                geph.pos[2] = GlobalMembersBinex.R8(p) * 1E3;
                p += 8;
                geph.vel[2] = GlobalMembersBinex.R8(p) * 1E3;
                p += 8;
                geph.acc[2] = GlobalMembersBinex.R8(p) * 1E3;
                p += 8;
                geph.svh = ((byte)(p)) & 0x1;
                p += 1;
                geph.frq = ((string)(p));
                p += 1;
                geph.age = ((byte)(p));
                p += 1;
                leap = ((byte)(p));
                p += 1;
                tau_gps = GlobalMembersBinex.R8(p);
                p += 8;
                geph.dtaun = GlobalMembersBinex.R8(p);
            }
            else
            {
                GlobalMembersRtkcmn.trace(2, "binex 0x01-02: length error len=%d\n", len);
                return -1;
            }
            if ((geph.sat = GlobalMembersRtkcmn.satno(DefineConstants.SYS_GLO, prn)) == 0)
            {
                GlobalMembersRtkcmn.trace(2, "binex 0x01-02: satellite error prn=%d\n", prn);
                return -1;
            }
            if (raw.time.time == 0)
            {
                return 0;
            }
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: geph.toe=utc2gpst(adjday(raw->time,tod-10800.0));
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            geph.toe.CopyFrom(GlobalMembersRtkcmn.utc2gpst(GlobalMembersBinex.adjday(new gtime_t(raw.time), tod - 10800.0)));
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: geph.tof=utc2gpst(adjday(raw->time,tof-10800.0));
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            geph.tof.CopyFrom(GlobalMembersRtkcmn.utc2gpst(GlobalMembersBinex.adjday(new gtime_t(raw.time), tof - 10800.0)));
            geph.iode = (int)(Math.IEEERemainder(tod, 86400.0) / 900.0 + 0.5);

            if (!StringFunctions.StrStr(raw.opt, "-EPHALL"))
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: if (fabs(timediff(geph.toe,raw->nav.geph[prn-DefineConstants.MINPRNGLO].toe))<1.0&& geph.svh==raw->nav.geph[prn-DefineConstants.MINPRNGLO].svh)
                if (Math.Abs(GlobalMembersRtkcmn.timediff(new gtime_t(geph.toe), raw.nav.geph[prn - DefineConstants.MINPRNGLO].toe)) < 1.0 && geph.svh == raw.nav.geph[prn - DefineConstants.MINPRNGLO].svh) // unchanged
                {
                    return 0;
                }
            }
            raw.nav.geph[prn - 1] = geph;
            raw.ephsat = geph.sat;
            return 2;
        }
        /* decode binex mesaage 0x01-03: decoded sbas ephmemeris ---------------------*/
        internal static int decode_bnx_01_03(raw_t raw, ref byte buff, int len)
        {
            seph_t seph = new seph_t();
            //C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on this variable, so pointers on this variable are left unchanged:
            byte* p = buff;
            double tow;
            double tod;
            double tof;
            int prn;
            int week;
            int iodn;

            GlobalMembersRtkcmn.trace(4, "binex 0x01-03: len=%d\n", len);

            if (len >= 98)
            {
                prn = ((byte)(p));
                p += 1;
                week = GlobalMembersBinex.U2(p);
                p += 2;
                tow = GlobalMembersBinex.U4(p);
                p += 4;
                seph.af0 = GlobalMembersBinex.R8(p);
                p += 8;
                tod = GlobalMembersBinex.R4(p);
                p += 4;
                tof = GlobalMembersBinex.U4(p);
                p += 4;
                seph.pos[0] = GlobalMembersBinex.R8(p) * 1E3;
                p += 8;
                seph.vel[0] = GlobalMembersBinex.R8(p) * 1E3;
                p += 8;
                seph.acc[0] = GlobalMembersBinex.R8(p) * 1E3;
                p += 8;
                seph.pos[1] = GlobalMembersBinex.R8(p) * 1E3;
                p += 8;
                seph.vel[1] = GlobalMembersBinex.R8(p) * 1E3;
                p += 8;
                seph.acc[1] = GlobalMembersBinex.R8(p) * 1E3;
                p += 8;
                seph.pos[2] = GlobalMembersBinex.R8(p) * 1E3;
                p += 8;
                seph.vel[2] = GlobalMembersBinex.R8(p) * 1E3;
                p += 8;
                seph.acc[2] = GlobalMembersBinex.R8(p) * 1E3;
                p += 8;
                seph.svh = ((byte)(p));
                p += 1;
                seph.sva = ((byte)(p));
                p += 1;
                iodn = ((byte)(p));
            }
            else
            {
                GlobalMembersRtkcmn.trace(2, "binex 0x01-03 length error: len=%d\n", len);
                return -1;
            }
            if ((seph.sat = GlobalMembersRtkcmn.satno(DefineConstants.SYS_SBS, prn)) == 0)
            {
                GlobalMembersRtkcmn.trace(2, "binex 0x01-03 satellite error: prn=%d\n", prn);
                return -1;
            }
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: seph.t0=gpst2time(week,tow);
            seph.t0.CopyFrom(GlobalMembersRtkcmn.gpst2time(week, tow));
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: seph.tof=adjweek(seph.t0,tof);
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            seph.tof.CopyFrom(GlobalMembersBinex.adjweek(new gtime_t(seph.t0), tof));

            if (!StringFunctions.StrStr(raw.opt, "-EPHALL"))
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: if (fabs(timediff(seph.t0,raw->nav.seph[prn-DefineConstants.MINPRNSBS].t0))<1.0&& seph.sva==raw->nav.seph[prn-DefineConstants.MINPRNSBS].sva)
                if (Math.Abs(GlobalMembersRtkcmn.timediff(new gtime_t(seph.t0), raw.nav.seph[prn - DefineConstants.MINPRNSBS].t0)) < 1.0 && seph.sva == raw.nav.seph[prn - DefineConstants.MINPRNSBS].sva) // unchanged
                {
                    return 0;
                }
            }
            raw.nav.seph[prn - DefineConstants.MINPRNSBS] = seph;
            raw.ephsat = seph.sat;
            return 2;
        }
        /* decode binex mesaage 0x01-04: decoded galileo ephmemeris ------------------*/
        internal static int decode_bnx_01_04(raw_t raw, ref byte buff, int len)
        {
            eph_t eph = new eph_t();
            //C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on this variable, so pointers on this variable are left unchanged:
            byte* p = buff;
            double tow;
            double ura;
            double sqrtA;
            int prn;

            GlobalMembersRtkcmn.trace(4, "binex 0x01-04: len=%d\n", len);

            if (len >= 127)
            {
                prn = ((byte)(p)) + 1;
                p += 1;
                eph.week = GlobalMembersBinex.U2(p);
                p += 2;
                tow = GlobalMembersBinex.I4(ref p);
                p += 4;
                eph.toes = GlobalMembersBinex.I4(ref p);
                p += 4;
                eph.tgd[0] = GlobalMembersBinex.R4(p); // BGD E5a/E1
                p += 4;
                eph.tgd[1] = GlobalMembersBinex.R4(p); // BGD E5b/E1
                p += 4;
                eph.iode = GlobalMembersBinex.I4(ref p); // IODnav
                p += 4;
                eph.f2 = GlobalMembersBinex.R4(p);
                p += 4;
                eph.f1 = GlobalMembersBinex.R4(p);
                p += 4;
                eph.f0 = GlobalMembersBinex.R4(p);
                p += 4;
                eph.deln = GlobalMembersBinex.R4(p) * DefineConstants.SC2RAD;
                p += 4;
                eph.M0 = GlobalMembersBinex.R8(p);
                p += 8;
                eph.e = GlobalMembersBinex.R8(p);
                p += 8;
                sqrtA = GlobalMembersBinex.R8(p);
                p += 8;
                eph.cic = GlobalMembersBinex.R4(p);
                p += 4;
                eph.crc = GlobalMembersBinex.R4(p);
                p += 4;
                eph.cis = GlobalMembersBinex.R4(p);
                p += 4;
                eph.crs = GlobalMembersBinex.R4(p);
                p += 4;
                eph.cuc = GlobalMembersBinex.R4(p);
                p += 4;
                eph.cus = GlobalMembersBinex.R4(p);
                p += 4;
                eph.OMG0 = GlobalMembersBinex.R8(p);
                p += 8;
                eph.omg = GlobalMembersBinex.R8(p);
                p += 8;
                eph.i0 = GlobalMembersBinex.R8(p);
                p += 8;
                eph.OMGd = GlobalMembersBinex.R4(p) * DefineConstants.SC2RAD;
                p += 4;
                eph.idot = GlobalMembersBinex.R4(p) * DefineConstants.SC2RAD;
                p += 4;
                ura = GlobalMembersBinex.R4(p) * 0.1;
                p += 4;
                eph.svh = GlobalMembersBinex.U2(p);
                p += 2;
                eph.code = GlobalMembersBinex.U2(p); // data source
            }
            else
            {
                GlobalMembersRtkcmn.trace(2, "binex 0x01-04: length error len=%d\n", len);
                return -1;
            }
            if ((eph.sat = GlobalMembersRtkcmn.satno(DefineConstants.SYS_GAL, prn)) == 0)
            {
                GlobalMembersRtkcmn.trace(2, "binex 0x01-04: satellite error prn=%d\n", prn);
                return -1;
            }
            eph.A = sqrtA * sqrtA;
            eph.iode = eph.iodc;
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: eph.toe=gpst2time(eph.week,eph.toes);
            eph.toe.CopyFrom(GlobalMembersRtkcmn.gpst2time(eph.week, eph.toes));
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: eph.toc=gpst2time(eph.week,eph.toes);
            eph.toc.CopyFrom(GlobalMembersRtkcmn.gpst2time(eph.week, eph.toes));
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: eph.ttr=adjweek(eph.toe,tow);
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            eph.ttr.CopyFrom(GlobalMembersBinex.adjweek(new gtime_t(eph.toe), tow));
            eph.sva = GlobalMembersBinex.uraindex(ura);

            if (!StringFunctions.StrStr(raw.opt, "-EPHALL"))
            {
                if (raw.nav.eph[eph.sat - 1].iode == eph.iode && raw.nav.eph[eph.sat - 1].iodc == eph.iodc) // unchanged
                {
                    return 0;
                }
            }
            raw.nav.eph[eph.sat - 1] = eph;
            raw.ephsat = eph.sat;
            return 2;
        }
        /* beidou signed 10 bit tgd -> sec -------------------------------------------*/
        internal static double bds_tgd(int tgd)
        {
            tgd &= 0x3FF;
            return ((tgd & 0x200) != 0) ? -1E10 * ((~tgd) & 0x1FF) : 1E-10 * (tgd & 0x1FF);
        }
        /* decode binex mesaage 0x01-05: decoded beidou-2/compass ephmemeris ---------*/
        internal static int decode_bnx_01_05(raw_t raw, ref byte buff, int len)
        {
            eph_t eph = new eph_t();
            //C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on this variable, so pointers on this variable are left unchanged:
            byte* p = buff;
            double tow;
            double toc;
            double sqrtA;
            int prn;
            int flag1;
            int flag2;

            GlobalMembersRtkcmn.trace(4, "binex 0x01-05: len=%d\n", len);

            if (len >= 117)
            {
                prn = ((byte)(p));
                p += 1;
                eph.week = GlobalMembersBinex.U2(p);
                p += 2;
                tow = GlobalMembersBinex.I4(ref p);
                p += 4;
                toc = GlobalMembersBinex.I4(ref p);
                p += 4;
                eph.toes = GlobalMembersBinex.I4(ref p);
                p += 4;
                eph.f2 = GlobalMembersBinex.R4(p);
                p += 4;
                eph.f1 = GlobalMembersBinex.R4(p);
                p += 4;
                eph.f0 = GlobalMembersBinex.R4(p);
                p += 4;
                eph.deln = GlobalMembersBinex.R4(p) * DefineConstants.SC2RAD;
                p += 4;
                eph.M0 = GlobalMembersBinex.R8(p);
                p += 8;
                eph.e = GlobalMembersBinex.R8(p);
                p += 8;
                sqrtA = GlobalMembersBinex.R8(p);
                p += 8;
                eph.cic = GlobalMembersBinex.R4(p);
                p += 4;
                eph.crc = GlobalMembersBinex.R4(p);
                p += 4;
                eph.cis = GlobalMembersBinex.R4(p);
                p += 4;
                eph.crs = GlobalMembersBinex.R4(p);
                p += 4;
                eph.cuc = GlobalMembersBinex.R4(p);
                p += 4;
                eph.cus = GlobalMembersBinex.R4(p);
                p += 4;
                eph.OMG0 = GlobalMembersBinex.R8(p);
                p += 8;
                eph.omg = GlobalMembersBinex.R8(p);
                p += 8;
                eph.i0 = GlobalMembersBinex.R8(p);
                p += 8;
                eph.OMGd = GlobalMembersBinex.R4(p) * DefineConstants.SC2RAD;
                p += 4;
                eph.idot = GlobalMembersBinex.R4(p) * DefineConstants.SC2RAD;
                p += 4;
                flag1 = GlobalMembersBinex.U2(p);
                p += 2;
                flag2 = GlobalMembersBinex.U4(p);
            }
            else
            {
                GlobalMembersRtkcmn.trace(2, "binex 0x01-05: length error len=%d\n", len);
                return -1;
            }
            if ((eph.sat = GlobalMembersRtkcmn.satno(DefineConstants.SYS_CMP, prn)) == 0)
            {
                GlobalMembersRtkcmn.trace(2, "binex 0x01-05: satellite error prn=%d\n", prn);
                return 0;
            }
            eph.A = sqrtA * sqrtA;
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: eph.toe=gpst2time(eph.week+1356,eph.toes+14.0);
            eph.toe.CopyFrom(GlobalMembersRtkcmn.gpst2time(eph.week + 1356, eph.toes + 14.0)); // bdt -> gpst
                                                                                               //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                                                                                               //ORIGINAL LINE: eph.toc=gpst2time(eph.week+1356,eph.toes+14.0);
            eph.toc.CopyFrom(GlobalMembersRtkcmn.gpst2time(eph.week + 1356, eph.toes + 14.0)); // bdt -> gpst
                                                                                               //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                                                                                               //ORIGINAL LINE: eph.ttr=adjweek(eph.toe,tow+14.0);
                                                                                               //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            eph.ttr.CopyFrom(GlobalMembersBinex.adjweek(new gtime_t(eph.toe), tow + 14.0)); // bdt -> gpst
            eph.iodc = (flag1 >> 1) & 0x1F;
            eph.iode = (flag1 >> 6) & 0x1F;
            eph.svh = flag1 & 0x01;
            eph.sva = flag2 & 0x0F; // ura index
            eph.tgd[0] = GlobalMembersBinex.bds_tgd(flag2 >> 4); // TGD1 (s)
            eph.tgd[1] = GlobalMembersBinex.bds_tgd(flag2 >> 14); // TGD2 (s)
            eph.flag = (flag1 >> 11) & 0x07; // nav type (0:unknown,1:IGSO/MEO,2:GEO)
            eph.code = (flag2 >> 25) & 0x7F;
            /* message source (0:unknown,1:B1I,2:B1Q,3:B2I,4:B2Q,5:B3I,6:B3Q)*/

            if (!StringFunctions.StrStr(raw.opt, "-EPHALL"))
            {
                if (raw.nav.eph[eph.sat - 1].iode == eph.iode && raw.nav.eph[eph.sat - 1].iodc == eph.iodc) // unchanged
                {
                    return 0;
                }
            }
            raw.nav.eph[eph.sat - 1] = eph;
            raw.ephsat = eph.sat;
            return 2;
        }
        /* decode binex mesaage 0x01-06: decoded qzss ephmemeris ---------------------*/
        internal static int decode_bnx_01_06(raw_t raw, ref byte buff, int len)
        {
            eph_t eph = new eph_t();
            //C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on this variable, so pointers on this variable are left unchanged:
            byte* p = buff;
            double tow;
            double ura;
            double sqrtA;
            int prn;
            int flag;

            GlobalMembersRtkcmn.trace(4, "binex 0x01-06: len=%d\n", len);

            if (len >= 127)
            {
                prn = ((byte)(p));
                p += 1;
                eph.week = GlobalMembersBinex.U2(p);
                p += 2;
                tow = GlobalMembersBinex.I4(ref p);
                p += 4;
                eph.toes = GlobalMembersBinex.I4(ref p);
                p += 4;
                eph.tgd[0] = GlobalMembersBinex.R4(p);
                p += 4;
                eph.iodc = GlobalMembersBinex.I4(ref p);
                p += 4;
                eph.f2 = GlobalMembersBinex.R4(p);
                p += 4;
                eph.f1 = GlobalMembersBinex.R4(p);
                p += 4;
                eph.f0 = GlobalMembersBinex.R4(p);
                p += 4;
                eph.iode = GlobalMembersBinex.I4(ref p);
                p += 4;
                eph.deln = GlobalMembersBinex.R4(p) * DefineConstants.SC2RAD;
                p += 4;
                eph.M0 = GlobalMembersBinex.R8(p);
                p += 8;
                eph.e = GlobalMembersBinex.R8(p);
                p += 8;
                sqrtA = GlobalMembersBinex.R8(p);
                p += 8;
                eph.cic = GlobalMembersBinex.R4(p);
                p += 4;
                eph.crc = GlobalMembersBinex.R4(p);
                p += 4;
                eph.cis = GlobalMembersBinex.R4(p);
                p += 4;
                eph.crs = GlobalMembersBinex.R4(p);
                p += 4;
                eph.cuc = GlobalMembersBinex.R4(p);
                p += 4;
                eph.cus = GlobalMembersBinex.R4(p);
                p += 4;
                eph.OMG0 = GlobalMembersBinex.R8(p);
                p += 8;
                eph.omg = GlobalMembersBinex.R8(p);
                p += 8;
                eph.i0 = GlobalMembersBinex.R8(p);
                p += 8;
                eph.OMGd = GlobalMembersBinex.R4(p) * DefineConstants.SC2RAD;
                p += 4;
                eph.idot = GlobalMembersBinex.R4(p) * DefineConstants.SC2RAD;
                p += 4;
                ura = GlobalMembersBinex.R4(p) * 0.1;
                p += 4;
                eph.svh = GlobalMembersBinex.U2(p);
                p += 2;
                flag = GlobalMembersBinex.U2(p);
            }
            else
            {
                GlobalMembersRtkcmn.trace(2, "binex 0x01-06: length error len=%d\n", len);
                return -1;
            }
            if ((eph.sat = GlobalMembersRtkcmn.satno(DefineConstants.SYS_QZS, prn)) == 0)
            {
                GlobalMembersRtkcmn.trace(2, "binex 0x01-06: satellite error prn=%d\n", prn);
                return 0;
            }
            eph.A = sqrtA * sqrtA;
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: eph.toe=gpst2time(eph.week,eph.toes);
            eph.toe.CopyFrom(GlobalMembersRtkcmn.gpst2time(eph.week, eph.toes));
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: eph.toc=gpst2time(eph.week,eph.toes);
            eph.toc.CopyFrom(GlobalMembersRtkcmn.gpst2time(eph.week, eph.toes));
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: eph.ttr=adjweek(eph.toe,tow);
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            eph.ttr.CopyFrom(GlobalMembersBinex.adjweek(new gtime_t(eph.toe), tow));
            eph.fit = ((flag & 0x01) != 0) ? 0.0 : 2.0; // 0:2hr,1:>2hr
            eph.sva = GlobalMembersBinex.uraindex(ura);
            eph.code = 2; // codes on L2 channel

            if (!StringFunctions.StrStr(raw.opt, "-EPHALL"))
            {
                if (raw.nav.eph[eph.sat - 1].iode == eph.iode && raw.nav.eph[eph.sat - 1].iodc == eph.iodc) // unchanged
                {
                    return 0;
                }
            }
            raw.nav.eph[eph.sat - 1] = eph;
            raw.ephsat = eph.sat;
            return 2;
        }
        /* decode binex mesaage 0x01: gnss navigaion informtion ----------------------*/
        internal static int decode_bnx_01(raw_t raw, ref byte buff, int len)
        {
            string msg;
            int srec = ((byte)(buff));
            int prn = ((byte)(buff + 1));

            if (raw.outtype != 0)
            {
                msg = raw.msgtype.Substring(raw.msgtype.Length);
                prn = srec == 0x01 || srec == 0x02 || srec == 0x04 ? prn + 1 : (srec == 0x00 ? 0 : prn);
                msg = string.Format(" subrec={0:X2} prn={1:D}", srec, prn);
            }
            switch (srec)
            {
                case 0x00:
                    return GlobalMembersBinex.decode_bnx_01_00(raw, ref buff + 1, len - 1);
                case 0x01:
                    return GlobalMembersBinex.decode_bnx_01_01(raw, ref buff + 1, len - 1);
                case 0x02:
                    return GlobalMembersBinex.decode_bnx_01_02(raw, ref buff + 1, len - 1);
                case 0x03:
                    return GlobalMembersBinex.decode_bnx_01_03(raw, ref buff + 1, len - 1);
                case 0x04:
                    return GlobalMembersBinex.decode_bnx_01_04(raw, ref buff + 1, len - 1);
                case 0x05:
                    return GlobalMembersBinex.decode_bnx_01_05(raw, ref buff + 1, len - 1);
                case 0x06:
                    return GlobalMembersBinex.decode_bnx_01_06(raw, ref buff + 1, len - 1);
            }
            return 0;
        }
        /* decode binex mesaage 0x02: generalized gnss data --------------------------*/
        internal static int decode_bnx_02(raw_t raw, ref byte buff, int len)
        {
            GlobalMembersRtkcmn.trace(2, "binex 0x02: not supported message\n");
            return 0;
        }
        /* decode binex mesaage 0x03: generalized ancillary site data ----------------*/
        internal static int decode_bnx_03(raw_t raw, ref byte buff, int len)
        {
            GlobalMembersRtkcmn.trace(2, "binex 0x03: not supported message\n");
            return 0;
        }
        /* decode binex mesaage 0x7d: receiver internal state prototyping ------------*/
        internal static int decode_bnx_7d(raw_t raw, ref byte buff, int len)
        {
            GlobalMembersRtkcmn.trace(2, "binex 0x7d: not supported message\n");
            return 0;
        }
        /* decode binex mesaage 0x7e: ancillary site data prototyping ----------------*/
        internal static int decode_bnx_7e(raw_t raw, ref byte buff, int len)
        {
            GlobalMembersRtkcmn.trace(2, "binex 0x7e: not supported message\n");
            return 0;
        }
        /* decode binex mesaage 0x7f-00: jpl fiducial site ---------------------------*/
        internal static int decode_bnx_7f_00(raw_t raw, ref byte buff, int len)
        {
            GlobalMembersRtkcmn.trace(2, "binex 0x7f-00: not supported message\n");
            return 0;
        }
        /* decode binex mesaage 0x7f-01: ucar cosmic ---------------------------------*/
        internal static int decode_bnx_7f_01(raw_t raw, ref byte buff, int len)
        {
            GlobalMembersRtkcmn.trace(2, "binex 0x7f-01: not supported message\n");
            return 0;
        }
        /* decode binex mesaage 0x7f-02: trimble 4700 --------------------------------*/
        internal static int decode_bnx_7f_02(raw_t raw, ref byte buff, int len)
        {
            GlobalMembersRtkcmn.trace(2, "binex 0x7f-02: not supported message\n");
            return 0;
        }
        /* decode binex mesaage 0x7f-03: trimble netrs -------------------------------*/
        internal static int decode_bnx_7f_03(raw_t raw, ref byte buff, int len)
        {
            GlobalMembersRtkcmn.trace(2, "binex 0x7f-03: not supported message\n");
            return 0;
        }
        /* decode binex mesaage 0x7f-04: trimble netrs -------------------------------*/
        internal static int decode_bnx_7f_04(raw_t raw, ref byte buff, int len)
        {
            GlobalMembersRtkcmn.trace(2, "binex 0x7f-04: not supported message\n");
            return 0;
        }
        /* decode binex mesaage 0x7f-05: trimble netr8 obs data ----------------------*/
        //C++ TO C# CONVERTER WARNING: C# has no equivalent to methods returning pointers to value types:
        //ORIGINAL LINE: static byte *decode_bnx_7f_05_obs(raw_t *raw, byte *buff, int sat, int nobs, obsd_t *data)
        internal static byte decode_bnx_7f_05_obs(raw_t raw, ref byte buff, int sat, int nobs, obsd_t data)
        {
            byte[] codes_gps = { DefineConstants.CODE_L1C, DefineConstants.CODE_L1C, DefineConstants.CODE_L1P, DefineConstants.CODE_L1W, DefineConstants.CODE_L1Y, DefineConstants.CODE_L1M, DefineConstants.CODE_L1X, DefineConstants.CODE_L1N, DefineConstants.CODE_NONE, DefineConstants.CODE_NONE, DefineConstants.CODE_L2W, DefineConstants.CODE_L2C, DefineConstants.CODE_L2D, DefineConstants.CODE_L2S, DefineConstants.CODE_L2L, DefineConstants.CODE_L2X, DefineConstants.CODE_L2P, DefineConstants.CODE_L2W, DefineConstants.CODE_L2Y, DefineConstants.CODE_L2M, DefineConstants.CODE_L2N, DefineConstants.CODE_NONE, DefineConstants.CODE_NONE, DefineConstants.CODE_L5X, DefineConstants.CODE_L5I, DefineConstants.CODE_L5Q, DefineConstants.CODE_L5X, 0, 0, 0, 0, 0 };
            byte[] codes_glo = { DefineConstants.CODE_L1C, DefineConstants.CODE_L1C, DefineConstants.CODE_L1P, DefineConstants.CODE_NONE, DefineConstants.CODE_NONE, DefineConstants.CODE_NONE, DefineConstants.CODE_NONE, DefineConstants.CODE_NONE, DefineConstants.CODE_NONE, DefineConstants.CODE_NONE, DefineConstants.CODE_L2C, DefineConstants.CODE_L2C, DefineConstants.CODE_L2P, DefineConstants.CODE_L3X, DefineConstants.CODE_L3I, DefineConstants.CODE_L3Q, DefineConstants.CODE_L3X, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            byte[] codes_gal = { DefineConstants.CODE_L1C, DefineConstants.CODE_L1A, DefineConstants.CODE_L1B, DefineConstants.CODE_L1C, DefineConstants.CODE_L1X, DefineConstants.CODE_L1Z, DefineConstants.CODE_L5X, DefineConstants.CODE_L5I, DefineConstants.CODE_L5Q, DefineConstants.CODE_L5X, DefineConstants.CODE_L7X, DefineConstants.CODE_L7I, DefineConstants.CODE_L7Q, DefineConstants.CODE_L7X, DefineConstants.CODE_L8X, DefineConstants.CODE_L8I, DefineConstants.CODE_L8Q, DefineConstants.CODE_L8X, DefineConstants.CODE_L6X, DefineConstants.CODE_L6A, DefineConstants.CODE_L6B, DefineConstants.CODE_L6C, DefineConstants.CODE_L6X, DefineConstants.CODE_L6Z, 0, 0, 0, 0, 0, 0, 0, 0 };
            byte[] codes_sbs = { DefineConstants.CODE_L1C, DefineConstants.CODE_L1C, DefineConstants.CODE_NONE, DefineConstants.CODE_NONE, DefineConstants.CODE_NONE, DefineConstants.CODE_NONE, DefineConstants.CODE_L5X, DefineConstants.CODE_L5I, DefineConstants.CODE_L5Q, DefineConstants.CODE_L5X, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            byte[] codes_cmp = { DefineConstants.CODE_L1X, DefineConstants.CODE_L1I, DefineConstants.CODE_L1Q, DefineConstants.CODE_L1X, DefineConstants.CODE_L7X, DefineConstants.CODE_L7I, DefineConstants.CODE_L7Q, DefineConstants.CODE_L7X, DefineConstants.CODE_L6X, DefineConstants.CODE_L6I, DefineConstants.CODE_L6Q, DefineConstants.CODE_L6X, DefineConstants.CODE_L1X, DefineConstants.CODE_L1S, DefineConstants.CODE_L1L, DefineConstants.CODE_L1X, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            byte[] codes_qzs = { DefineConstants.CODE_L1C, DefineConstants.CODE_L1C, DefineConstants.CODE_L1S, DefineConstants.CODE_L1L, DefineConstants.CODE_L1X, DefineConstants.CODE_NONE, DefineConstants.CODE_NONE, DefineConstants.CODE_L2X, DefineConstants.CODE_L2S, DefineConstants.CODE_L2L, DefineConstants.CODE_L2X, DefineConstants.CODE_NONE, DefineConstants.CODE_NONE, DefineConstants.CODE_L5X, DefineConstants.CODE_L5I, DefineConstants.CODE_L5Q, DefineConstants.CODE_L5X, DefineConstants.CODE_NONE, DefineConstants.CODE_NONE, DefineConstants.CODE_L6X, DefineConstants.CODE_L6S, DefineConstants.CODE_L6L, DefineConstants.CODE_L6X, DefineConstants.CODE_NONE, DefineConstants.CODE_NONE, DefineConstants.CODE_NONE, DefineConstants.CODE_NONE, DefineConstants.CODE_NONE, DefineConstants.CODE_NONE, DefineConstants.CODE_NONE, DefineConstants.CODE_L1Z, 0 };
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: const byte *codes=null;
            byte codes = null;
            double[] range = new double[8];
            double[] phase = new double[8];
            double[] cnr = new double[8];
            double[] dopp = { 0, null, null, null, null, null, null, null };
            double acc;
            double wl;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: byte *p=buff;
            byte p = buff;
            byte flag;
            byte[] flags = new byte[4];
            int i;
            int j;
            int k;
            int sys;
            int fcn = -10;
            int[] code = new int[8];
            int[] slip = new int[8];
            int[] pri = new int[8];
            int[] freq = new int[8];
            int[] slipcnt = new int[8];
            int[] mask = new int[8];

            GlobalMembersRtkcmn.trace(5, "decode_bnx_7f_05_obs: sat=%2d nobs=%2d\n", sat, nobs);

            sys = GlobalMembersRtkcmn.satsys(sat, null);

            switch (sys)
            {
                case DefineConstants.SYS_GPS:
                    codes = codes_gps;
                    break;
                case DefineConstants.SYS_GLO:
                    codes = codes_glo;
                    break;
                case DefineConstants.SYS_GAL:
                    codes = codes_gal;
                    break;
                case DefineConstants.SYS_QZS:
                    codes = codes_qzs;
                    break;
                case DefineConstants.SYS_SBS:
                    codes = codes_sbs;
                    break;
                case DefineConstants.SYS_CMP:
                    codes = codes_cmp;
                    break;
            }
            for (i = 0; i < nobs; i++)
            {

                flag = GlobalMembersRtkcmn.getbitu(p, 0, 1);
                slip[i] = GlobalMembersRtkcmn.getbitu(p, 2, 1);
                code[i] = GlobalMembersRtkcmn.getbitu(p, 3, 5);
                p++;

                for (j = 0; j < 4; j++)
                {
                    flags[j] = 0;
                }

                for (j = 0; flag && j < 4; j++)
                {
                    flag = ((byte)(p++));
                    flags[flag & 0x03] = flag & 0x7F;
                    flag &= 0x80;
                }
                if (flags[2] != 0)
                {
                    fcn = GlobalMembersRtkcmn.getbits(flags + 2, 2, 4);
                }
                acc = ((flags[0] & 0x20) != 0) ? 0.0001 : 0.00002; // phase accuracy

                cnr[i] = ((byte)(p++)) * 0.4;

                if (i == 0)
                {
                    cnr[i] += GlobalMembersRtkcmn.getbits(p, 0, 2) * 0.1;
                    range[i] = GlobalMembersRtkcmn.getbitu(p, 2, 32) * 0.064 + GlobalMembersRtkcmn.getbitu(p, 34, 6) * 0.001;
                    p += 5;
                }
                else if (flags[0] & 0x40)
                {
                    cnr[i] += GlobalMembersRtkcmn.getbits(p, 0, 2) * 0.1;
                    range[i] = range[0] + GlobalMembersRtkcmn.getbits(p, 4, 20) * 0.001;
                    p += 3;
                }
                else
                {
                    range[i] = range[0] + GlobalMembersRtkcmn.getbits(p, 0, 16) * 0.001;
                    p += 2;
                }
                if (flags[0] & 0x40)
                {
                    phase[i] = range[i] + GlobalMembersRtkcmn.getbits(p, 0, 24) * acc;
                    p += 3;
                }
                else
                {
                    cnr[i] += GlobalMembersRtkcmn.getbits(p, 0, 2) * 0.1;
                    phase[i] = range[i] + GlobalMembersRtkcmn.getbits(p, 2, 22) * acc;
                    p += 3;
                }
                if (flags[0] & 0x04)
                {
                    dopp[i] = GlobalMembersRtkcmn.getbits(p, 0, 24) / 256.0;
                    p += 3;
                }
                if (flags[0] & 0x08)
                {
                    if (flags[0] & 0x10)
                    {
                        slipcnt[i] = GlobalMembersBinex.U2(p);
                        p += 2;
                    }
                    else
                    {
                        slipcnt[i] = ((byte)(p));
                        p += 1;
                    }
                }
                GlobalMembersRtkcmn.trace(5, "(%d) CODE=%2d S=%d F=%02X %02X %02X %02X\n", i + 1, code[i], slip[i], flags[0], flags[1], flags[2], flags[3]);
                GlobalMembersRtkcmn.trace(5, "(%d) P=%13.3f L=%13.3f D=%7.1f SNR=%4.1f SCNT=%2d\n", i + 1, range[i], phase[i], dopp[i], cnr[i], slipcnt[i]);
            }
            if (codes == 0)
            {
                data.sat = 0;
                return p;
            }
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: data->time=raw->time;
            data.time.CopyFrom(raw.time);
            data.sat = sat;

            /* get code priority */
            for (i = 0; i < nobs; i++)
            {
                GlobalMembersRtkcmn.code2obs(codes[code[i] & 0x3F], ref freq + i);
                pri[i] = GlobalMembersRtkcmn.getcodepri(sys, codes[code[i] & 0x3F], raw.opt);

                /* frequency index for beidou */
                if (sys == DefineConstants.SYS_CMP)
                {
                    if (freq[i] == 5) // B2
                    {
                        freq[i] = 2;
                    }
                    else if (freq[i] == 4) // B3
                    {
                        freq[i] = 3;
                    }
                }
            }
            for (i = 0; i < DefineConstants.NFREQ; i++)
            {
                for (j = 0, k = -1; j < nobs; j++)
                {
                    if (freq[j] == i + 1 && (k < 0 || pri[j] > pri[k]))
                    {
                        k = j;
                    }
                }
                if (k < 0)
                {
                    data.P[i] = data.L[i] = 0.0;
                    data.D[i] = 0.0f;
                    data.SNR[i] = data.LLI[i] = 0;
                    data.code[i] = DefineConstants.CODE_NONE;
                }
                else
                {
                    wl = GlobalMembersRtkcmn.satwavelen(sat, i, raw.nav);
                    if (sys == DefineConstants.SYS_GLO && fcn >= -7 && freq[k] <= 2)
                    {
                        wl = DefineConstants.CLIGHT / (freq[k] == 1 ? DefineConstants.FREQ1_GLO + DefineConstants.DFRQ1_GLO * fcn : DefineConstants.FREQ2_GLO + DefineConstants.DFRQ2_GLO * fcn);
                    }
                    data.P[i] = range[k];
                    data.L[i] = wl <= 0.0 ? 0.0 : phase[k] / wl;
                    data.D[i] = dopp[k];
                    data.SNR[i] = (byte)(cnr[k] / 0.25 + 0.5);
                    data.code[i] = codes[code[k] & 0x3F];
                    data.LLI[i] = slip[k] != 0 ? 1 : 0;
                    mask[k] = 1;
                }
            }
            for (; i < DefineConstants.NFREQ + DefineConstants.NEXOBS; i++)
            {
                for (k = 0; k < nobs; k++)
                {
                    if (mask[k] == 0)
                        break;
                }
                if (k >= nobs)
                {
                    data.P[i] = data.L[i] = 0.0;
                    data.D[i] = 0.0f;
                    data.SNR[i] = data.LLI[i] = 0;
                    data.code[i] = DefineConstants.CODE_NONE;
                }
                else
                {
                    wl = GlobalMembersRtkcmn.satwavelen(sat, freq[k] - 1, raw.nav);
                    if (sys == DefineConstants.SYS_GLO && fcn >= -7 && freq[k] <= 2)
                    {
                        wl = DefineConstants.CLIGHT / (freq[k] == 1 ? DefineConstants.FREQ1_GLO + DefineConstants.DFRQ1_GLO * fcn : DefineConstants.FREQ2_GLO + DefineConstants.DFRQ2_GLO * fcn);
                    }
                    data.P[i] = range[k];
                    data.L[i] = wl <= 0.0 ? 0.0 : phase[k] / wl;
                    data.D[i] = dopp[k];
                    data.SNR[i] = (byte)(cnr[k] / 0.25 + 0.5);
                    data.code[i] = codes[code[k] & 0x3F];
                    data.LLI[i] = slip[k] != 0 ? 1 : 0;
                    mask[k] = 1;
                }
            }
            return p;
        }
        /* decode binex mesaage 0x7f-05: trimble netr8 -------------------------------*/
        internal static int decode_bnx_7f_05(raw_t raw, ref byte buff, int len)
        {
            obsd_t data = new obsd_t({ 0 });
            double clkoff = 0.0;
            double[] toff = { 0, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null };
            string msg;
            //C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on this variable, so pointers on this variable are left unchanged:
            byte* p = buff;
            uint flag;
            int i;
            int nsat;
            int nobs;
            int prn;
            int sys;
            int sat;
            int clkrst = 0;
            int rsys = 0;
            int nsys = 0;
            int[] tsys = new int[16];

            GlobalMembersRtkcmn.trace(4, "decode_bnx_7f_05\n");

            raw.obs.n = 0;
            flag = ((byte)(p++));
            nsat = (int)(flag & 0x3F) + 1;

            if ((flag & 0x80) != 0) // rxclkoff
            {
                clkrst = GlobalMembersRtkcmn.getbitu(p, 0, 2);
                clkoff = GlobalMembersRtkcmn.getbits(p, 2, 22) * 1E-9;
                p += 3;
            }
            if ((flag & 0x40) != 0) // systime
            {
                nsys = GlobalMembersRtkcmn.getbitu(p, 0, 4);
                rsys = GlobalMembersRtkcmn.getbitu(p, 4, 4);
                p++;
                for (i = 0; i < nsys; i++)
                {
                    toff[i] = GlobalMembersRtkcmn.getbits(p, 0, 24) * 1E-9;
                    tsys[i] = GlobalMembersRtkcmn.getbitu(p, 28, 4);
                    p += 4;
                }
            }
            for (i = 0; i < nsat; i++)
            {
                prn = ((byte)(p++));
                nobs = GlobalMembersRtkcmn.getbitu(p, 1, 3);
                sys = GlobalMembersRtkcmn.getbitu(p, 4, 4);
                p++;

                GlobalMembersRtkcmn.trace(5, "binex 0x7F-05 PRN=%3d SYS=%d NOBS=%d\n", prn, sys, nobs);

                switch (sys)
                {
                    case 0:
                        sat = GlobalMembersRtkcmn.satno(DefineConstants.SYS_GPS, prn);
                        break;
                    case 1:
                        sat = GlobalMembersRtkcmn.satno(DefineConstants.SYS_GLO, prn);
                        break;
                    case 2:
                        sat = GlobalMembersRtkcmn.satno(DefineConstants.SYS_SBS, prn);
                        break;
                    case 3:
                        sat = GlobalMembersRtkcmn.satno(DefineConstants.SYS_GAL, prn);
                        break;
                    case 4:
                        sat = GlobalMembersRtkcmn.satno(DefineConstants.SYS_CMP, prn);
                        break;
                    case 5:
                        sat = GlobalMembersRtkcmn.satno(DefineConstants.SYS_QZS, prn);
                        break;
                    default:
                        sat = 0;
                        break;
                }
                /* decode binex mesaage 0x7F-05 obs data */
                if ((p = GlobalMembersBinex.decode_bnx_7f_05_obs(raw, ref p, sat, nobs, data)) == 0)
                {
                    return -1;
                }

                if ((int)(p - buff) > len)
                {
                    GlobalMembersRtkcmn.trace(2, "binex 0x7F-05 length error: nsat=%2d len=%d\n", nsat, len);
                    return -1;
                }
                /* save obs data to obs buffer */
                if (data.sat != 0 && raw.obs.n < DefineConstants.MAXOBS)
                {
                    raw.obs.data[raw.obs.n++] = data;
                }
            }
            if (raw.outtype != 0)
            {
                msg = raw.msgtype.Substring(raw.msgtype.Length);
                msg = string.Format(" nsat={0,2:D}", nsat);
            }
            return raw.obs.n > 0 ? 1 : 0;
        }
        /* decode binex mesaage 0x7f: gnss data prototyping --------------------------*/
        internal static int decode_bnx_7f(raw_t raw, ref byte buff, int len)
        {
            double[] gpst0 = { 1980, 1, 6, 0, 0, 0 };
            string msg;
            //C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on this variable, so pointers on this variable are left unchanged:
            byte* p = buff;
            uint srec;
            uint min;
            uint msec;

            srec = ((byte)(p));
            p += 1;
            min = GlobalMembersBinex.U4(p);
            p += 4;
            msec = GlobalMembersBinex.U2(p);
            p += 2;
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: raw->time=timeadd(epoch2time(gpst0),min *60.0+msec *0.001);
            raw.time.CopyFrom(GlobalMembersRtkcmn.timeadd(GlobalMembersRtkcmn.epoch2time(gpst0), min * 60.0 + msec * 0.001));

            if (raw.outtype != 0)
            {
                msg = raw.msgtype.Substring(raw.msgtype.Length);
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: sprintf(msg," subrec=%02X time%s",srec,time_str(raw->time,3));
                msg = string.Format(" subrec={0:X2} time{1}", srec, GlobalMembersRtkcmn.time_str(new gtime_t(raw.time), 3));
            }
            switch (srec)
            {
                case 0x00:
                    return GlobalMembersBinex.decode_bnx_7f_00(raw, ref buff + 7, len - 7);
                case 0x01:
                    return GlobalMembersBinex.decode_bnx_7f_01(raw, ref buff + 7, len - 7);
                case 0x02:
                    return GlobalMembersBinex.decode_bnx_7f_02(raw, ref buff + 7, len - 7);
                case 0x03:
                    return GlobalMembersBinex.decode_bnx_7f_03(raw, ref buff + 7, len - 7);
                case 0x04:
                    return GlobalMembersBinex.decode_bnx_7f_04(raw, ref buff + 7, len - 7);
                case 0x05:
                    return GlobalMembersBinex.decode_bnx_7f_05(raw, ref buff + 7, len - 7);
            }
            return 0;
        }
        /* decode binex mesaage ------------------------------------------------------*/
        internal static int decode_bnx(raw_t raw)
        {
            uint len;
            uint cs1;
            uint cs2;
            int rec;
            int len_h;

            rec = raw.buff[1]; // record id

            /* record and header length */
            len_h = GlobalMembersBinex.getbnxi(raw.buff + 2, ref len);

            GlobalMembersRtkcmn.trace(5, "decode_bnx: rec=%02x len=%d\n", rec, len);

            /* check parity */
            if (raw.len - 1 < 128)
            {
                cs1 = ((byte)(raw.buff + raw.len));
                cs2 = GlobalMembersBinex.csum8(raw.buff + 1, raw.len - 1);
            }
            else
            {
                cs1 = GlobalMembersBinex.U2(raw.buff + raw.len);
                cs2 = GlobalMembersRtkcmn.crc16(raw.buff + 1, raw.len - 1);
            }
            if (cs1 != cs2)
            {
                GlobalMembersRtkcmn.trace(2, "binex 0x%02X parity error CS=%X %X\n", rec, cs1, cs2);
                return -1;
            }
            if (raw.outtype != 0)
            {
                raw.msgtype = string.Format("BINEX 0x{0:X2} ({1,4:D})", rec, raw.len);
            }
            /* decode binex message record */
            switch (rec)
            {
                case 0x00:
                    return GlobalMembersBinex.decode_bnx_00(raw, ref raw.buff + 2 + len_h, len);
                case 0x01:
                    return GlobalMembersBinex.decode_bnx_01(raw, ref raw.buff + 2 + len_h, len);
                case 0x02:
                    return GlobalMembersBinex.decode_bnx_02(raw, ref raw.buff + 2 + len_h, len);
                case 0x03:
                    return GlobalMembersBinex.decode_bnx_03(raw, ref raw.buff + 2 + len_h, len);
                case 0x7d:
                    return GlobalMembersBinex.decode_bnx_7d(raw, ref raw.buff + 2 + len_h, len);
                case 0x7e:
                    return GlobalMembersBinex.decode_bnx_7e(raw, ref raw.buff + 2 + len_h, len);
                case 0x7f:
                    return GlobalMembersBinex.decode_bnx_7f(raw, ref raw.buff + 2 + len_h, len);
            }
            return 0;
        }
        /* synchronize binex message -------------------------------------------------*/
        internal static int sync_bnx(byte[] buff, byte data)
        {
            buff[0] = buff[1];
            buff[1] = data;

            return buff[0] == DefineConstants.BNXSYNC2 && (buff[1] == 0x00 || buff[1] == 0x01 || buff[1] == 0x02 || buff[1] == 0x03 || buff[1] == 0x7D || buff[1] == 0x7E || buff[1] == 0x7F);
        }
        /* input binex message from stream ---------------------------------------------
        * fetch next binex data and input a message from stream
        * args   : raw_t *raw   IO     receiver raw data control struct
        *          unsigned char data I stream data (1 byte)
        * return : status (-1: error message, 0: no message, 1: input observation data,
        *                  2: input ephemeris)
        * notes  : support only the following message (ref [1])
        *
        *          - big-endian, regular CRC, forward record (sync=0xE2)
        *          - record-subrecord:
        *            0x01-01: decoded gps ephemeris
        *            0x01-02: decoded glonass ephemeris
        *            0x01-03: decoded sbas ephemeris
        *            0x01-04: decoded galileo ephemeris
        *            0x01-05: decoded beidou-2/compass ephemeris
        *            0x01-06: decoded qzss ephemeris
        *            0x7f-05: gnss data prototyping - trimble netr8
        *
        *          to specify input options, set rtcm->opt to the following option
        *          strings separated by spaces.
        *
        *          -EPHALL  : input all ephemerides
        *          -GLss    : select signal ss for GPS (ss=1C,1P,...)
        *          -RLss    : select signal ss for GLO (ss=1C,1P,...)
        *          -ELss    : select signal ss for GAL (ss=1C,1B,...)
        *          -JLss    : select signal ss for QZS (ss=1C,2C,...)
        *          -CLss    : select signal ss for BDS (ss=2I,2X,...)
        *
        *-----------------------------------------------------------------------------*/
        public static int input_bnx(raw_t raw, byte data)
        {
            uint len;
            int len_h;
            int len_c;

            GlobalMembersRtkcmn.trace(5, "input_bnx: data=%02x\n", data);

            /* synchronize binex message */
            if (raw.nbyte == 0)
            {
                if (GlobalMembersBinex.sync_bnx(raw.buff, data) == 0)
                {
                    return 0;
                }
                raw.nbyte = 2;
                return 0;
            }
            raw.buff[raw.nbyte++] = data;
            if (raw.nbyte < 4)
            {
                return 0;
            }

            len_h = GlobalMembersBinex.getbnxi(raw.buff + 2, ref len);

            raw.len = len + len_h + 2; // length without crc

            if (raw.len - 1 > 4096)
            {
                GlobalMembersRtkcmn.trace(2, "binex length error: len=%d\n", raw.len - 1);
                raw.nbyte = 0;
                return -1;
            }
            len_c = raw.len - 1 < 128 ? 1 : 2;

            if (raw.nbyte < (int)(raw.len + len_c))
            {
                return 0;
            }
            raw.nbyte = 0;

            /* decode binex message */
            return GlobalMembersBinex.decode_bnx(raw);
        }
        /* input binex message from file -----------------------------------------------
        * fetch next binex data and input a message from file
        * args   : raw_t  *raw   IO     receiver raw data control struct
        *          FILE   *fp    I      file pointer
        * return : status(-2: end of file, -1...9: same as above)
        *-----------------------------------------------------------------------------*/
        public static int input_bnxf(raw_t raw, FILE fp)
        {
            uint len;
            int i;
            int data;
            int len_h;
            int len_c;

            GlobalMembersRtkcmn.trace(4, "input_bnxf\n");

            if (raw.nbyte == 0)
            {
                for (i = 0; ; i++)
                {
                    if ((data = fgetc(fp)) == EOF)
                    {
                        return -2;
                    }
                    if (GlobalMembersBinex.sync_bnx(raw.buff, (byte)data) != 0)
                        break;
                    if (i >= 4096)
                    {
                        return 0;
                    }
                }
            }
            if (fread(raw.buff + 2, 1, 4, fp) < 4)
            {
                return -2;
            }

            len_h = GlobalMembersBinex.getbnxi(raw.buff + 2, ref len);

            raw.len = len + len_h + 2;

            if (raw.len - 1 > 4096)
            {
                GlobalMembersRtkcmn.trace(2, "binex length error: len=%d\n", raw.len - 1);
                raw.nbyte = 0;
                return -1;
            }
            len_c = raw.len - 1 < 128 ? 1 : 2;

            if (fread(raw.buff + 6, 1, raw.len + len_c - 6, fp) < (uint)(raw.len + len_c - 6))
            {
                return -2;
            }
            raw.nbyte = 0;

            /* decode binex message */
            return GlobalMembersBinex.decode_bnx(raw);
        }
    }

    internal static partial class DefineConstants
    {
        public const string VER_RTKLIB = "2.4.2";
        public const string PATCH_LEVEL = "p12";
        public const string COPYRIGHT_RTKLIB = "Copyright (C) 2007-2015 by T.Takasu\nAll rights reserved.";
        public const double PI = 3.1415926535897932;
        public const double CLIGHT = 299792458.0;
        public const double SC2RAD = 3.1415926535898;
        public const double AU = 149597870691.0;
        public const double OMGE = 7.2921151467E-5;
        public const double RE_WGS84 = 6378137.0;
        public const double HION = 350000.0;
        public const int MAXFREQ = 7;
        public const double FREQ1 = 1.57542E9;
        public const double FREQ2 = 1.22760E9;
        public const double FREQ5 = 1.17645E9;
        public const double FREQ6 = 1.27875E9;
        public const double FREQ7 = 1.20714E9;
        public const double FREQ8 = 1.191795E9;
        public const double FREQ1_GLO = 1.60200E9;
        public const double DFRQ1_GLO = 0.56250E6;
        public const double FREQ2_GLO = 1.24600E9;
        public const double DFRQ2_GLO = 0.43750E6;
        public const double FREQ3_GLO = 1.202025E9;
        public const double FREQ1_CMP = 1.561098E9;
        public const double FREQ2_CMP = 1.20714E9;
        public const double FREQ3_CMP = 1.26852E9;
        public const double EFACT_GPS = 1.0;
        public const double EFACT_GLO = 1.5;
        public const double EFACT_GAL = 1.0;
        public const double EFACT_QZS = 1.0;
        public const double EFACT_CMP = 1.0;
        public const double EFACT_SBS = 3.0;
        public const int SYS_NONE = 0x00;
        public const int SYS_GPS = 0x01;
        public const int SYS_SBS = 0x02;
        public const int SYS_GLO = 0x04;
        public const int SYS_GAL = 0x08;
        public const int SYS_QZS = 0x10;
        public const int SYS_CMP = 0x20;
        public const int SYS_LEO = 0x40;
        public const int SYS_ALL = 0xFF;
        public const int TSYS_GPS = 0;
        public const int TSYS_UTC = 1;
        public const int TSYS_GLO = 2;
        public const int TSYS_GAL = 3;
        public const int TSYS_QZS = 4;
        public const int TSYS_CMP = 5;
        public const int NFREQ = 3;
        public const int NFREQGLO = 2;
        public const int NEXOBS = 0;
        public const int MINPRNGPS = 1;
        public const int MAXPRNGPS = 32;
        public const int NSYSGPS = 1;

#if MINPRNGLO_ConditionalDefinition1
	public const int MINPRNGLO = 1;
#elif MINPRNGLO_ConditionalDefinition2
	public const int MINPRNGLO = 0;
#endif
#if MAXPRNGLO_ConditionalDefinition1
	public const int MAXPRNGLO = 24;
#elif MAXPRNGLO_ConditionalDefinition2
	public const int MAXPRNGLO = 0;
#endif
#if NSYSGLO_ConditionalDefinition1
	public const int NSYSGLO = 1;
#elif NSYSGLO_ConditionalDefinition2
	public const int NSYSGLO = 0;
#endif
        public const int NSATGLO = 0;
#if MINPRNGAL_ConditionalDefinition1
	public const int MINPRNGAL = 1;
#elif MINPRNGAL_ConditionalDefinition2
	public const int MINPRNGAL = 0;
#endif
#if MAXPRNGAL_ConditionalDefinition1
	public const int MAXPRNGAL = 27;
#elif MAXPRNGAL_ConditionalDefinition2
	public const int MAXPRNGAL = 0;
#endif
#if NSYSGAL_ConditionalDefinition1
	public const int NSYSGAL = 1;
#elif NSYSGAL_ConditionalDefinition2
	public const int NSYSGAL = 0;
#endif
        public const int NSATGAL = 0;
#if MINPRNQZS_ConditionalDefinition1
	public const int MINPRNQZS = 193;
#elif MINPRNQZS_ConditionalDefinition2
	public const int MINPRNQZS = 0;
#endif
#if MAXPRNQZS_ConditionalDefinition1
	public const int MAXPRNQZS = 199;
#elif MAXPRNQZS_ConditionalDefinition2
	public const int MAXPRNQZS = 0;
#endif
#if MINPRNQZS_S_ConditionalDefinition1
	public const int MINPRNQZS_S = 183;
#elif MINPRNQZS_S_ConditionalDefinition2
	public const int MINPRNQZS_S = 0;
#endif
#if MAXPRNQZS_S_ConditionalDefinition1
	public const int MAXPRNQZS_S = 189;
#elif MAXPRNQZS_S_ConditionalDefinition2
	public const int MAXPRNQZS_S = 0;
#endif
#if NSYSQZS_ConditionalDefinition1
	public const int NSYSQZS = 1;
#elif NSYSQZS_ConditionalDefinition2
	public const int NSYSQZS = 0;
#endif
        public const int NSATQZS = 0;
#if MINPRNCMP_ConditionalDefinition1
	public const int MINPRNCMP = 1;
#elif MINPRNCMP_ConditionalDefinition2
	public const int MINPRNCMP = 0;
#endif
#if MAXPRNCMP_ConditionalDefinition1
	public const int MAXPRNCMP = 35;
#elif MAXPRNCMP_ConditionalDefinition2
	public const int MAXPRNCMP = 0;
#endif
#if NSYSCMP_ConditionalDefinition1
	public const int NSYSCMP = 1;
#elif NSYSCMP_ConditionalDefinition2
	public const int NSYSCMP = 0;
#endif
        public const int NSATCMP = 0;
#if MINPRNLEO_ConditionalDefinition1
	public const int MINPRNLEO = 1;
#elif MINPRNLEO_ConditionalDefinition2
	public const int MINPRNLEO = 0;
#endif
#if MAXPRNLEO_ConditionalDefinition1
	public const int MAXPRNLEO = 10;
#elif MAXPRNLEO_ConditionalDefinition2
	public const int MAXPRNLEO = 0;
#endif
#if NSYSLEO_ConditionalDefinition1
	public const int NSYSLEO = 1;
#elif NSYSLEO_ConditionalDefinition2
	public const int NSYSLEO = 0;
#endif
        public const int NSATLEO = 0;
        public const int MINPRNSBS = 120;
        public const int MAXPRNSBS = 142;
        public const int MAXOBS = 64;
        public const int MAXRCV = 64;
        public const int MAXOBSTYPE = 64;
        public const double DTTOL = 0.005;
        public const double MAXDTOE = 7200.0;
        public const double MAXDTOE_QZS = 7200.0;
        public const double MAXDTOE_GAL = 10800.0;
        public const double MAXDTOE_CMP = 21600.0;
        public const double MAXDTOE_GLO = 1800.0;
        public const double MAXDTOE_SBS = 360.0;
        public const double MAXDTOE_S = 86400.0;
        public const double MAXGDOP = 300.0;
        public const double INT_SWAP_TRAC = 86400.0;
        public const double INT_SWAP_STAT = 86400.0;
        public const int MAXEXFILE = 1024;
        public const double MAXSBSAGEF = 30.0;
        public const double MAXSBSAGEL = 1800.0;
        public const int MAXSBSURA = 8;
        public const int MAXBAND = 10;
        public const int MAXNIGP = 201;
        public const int MAXNGEO = 4;
        public const int MAXCOMMENT = 10;
        public const int MAXSTRPATH = 1024;
        public const int MAXSTRMSG = 1024;
        public const int MAXSTRRTK = 8;
        public const int MAXSBSMSG = 32;
        public const int MAXSOLMSG = 4096;
        public const int MAXRAWLEN = 4096;
        public const int MAXERRMSG = 4096;
        public const int MAXANT = 64;
        public const int MAXSOLBUF = 256;
        public const int MAXOBSBUF = 128;
        public const int MAXNRPOS = 16;
        public const int MAXLEAPS = 64;
        public const double RNX2VER = 2.10;
        public const double RNX3VER = 3.00;
        public const int OBSTYPE_PR = 0x01;
        public const int OBSTYPE_CP = 0x02;
        public const int OBSTYPE_DOP = 0x04;
        public const int OBSTYPE_SNR = 0x08;
        public const int OBSTYPE_ALL = 0xFF;
        public const int FREQTYPE_L1 = 0x01;
        public const int FREQTYPE_L2 = 0x02;
        public const int FREQTYPE_L5 = 0x04;
        public const int FREQTYPE_L6 = 0x08;
        public const int FREQTYPE_L7 = 0x10;
        public const int FREQTYPE_L8 = 0x20;
        public const int FREQTYPE_ALL = 0xFF;
        public const int CODE_NONE = 0;
        public const int CODE_L1C = 1;
        public const int CODE_L1P = 2;
        public const int CODE_L1W = 3;
        public const int CODE_L1Y = 4;
        public const int CODE_L1M = 5;
        public const int CODE_L1N = 6;
        public const int CODE_L1S = 7;
        public const int CODE_L1L = 8;
        public const int CODE_L1E = 9;
        public const int CODE_L1A = 10;
        public const int CODE_L1B = 11;
        public const int CODE_L1X = 12;
        public const int CODE_L1Z = 13;
        public const int CODE_L2C = 14;
        public const int CODE_L2D = 15;
        public const int CODE_L2S = 16;
        public const int CODE_L2L = 17;
        public const int CODE_L2X = 18;
        public const int CODE_L2P = 19;
        public const int CODE_L2W = 20;
        public const int CODE_L2Y = 21;
        public const int CODE_L2M = 22;
        public const int CODE_L2N = 23;
        public const int CODE_L5I = 24;
        public const int CODE_L5Q = 25;
        public const int CODE_L5X = 26;
        public const int CODE_L7I = 27;
        public const int CODE_L7Q = 28;
        public const int CODE_L7X = 29;
        public const int CODE_L6A = 30;
        public const int CODE_L6B = 31;
        public const int CODE_L6C = 32;
        public const int CODE_L6X = 33;
        public const int CODE_L6Z = 34;
        public const int CODE_L6S = 35;
        public const int CODE_L6L = 36;
        public const int CODE_L8I = 37;
        public const int CODE_L8Q = 38;
        public const int CODE_L8X = 39;
        public const int CODE_L2I = 40;
        public const int CODE_L2Q = 41;
        public const int CODE_L6I = 42;
        public const int CODE_L6Q = 43;
        public const int CODE_L3I = 44;
        public const int CODE_L3Q = 45;
        public const int CODE_L3X = 46;
        public const int CODE_L1I = 47;
        public const int CODE_L1Q = 48;
        public const int MAXCODE = 48;
        public const int PMODE_SINGLE = 0;
        public const int PMODE_DGPS = 1;
        public const int PMODE_KINEMA = 2;
        public const int PMODE_STATIC = 3;
        public const int PMODE_MOVEB = 4;
        public const int PMODE_FIXED = 5;
        public const int PMODE_PPP_KINEMA = 6;
        public const int PMODE_PPP_STATIC = 7;
        public const int PMODE_PPP_FIXED = 8;
        public const int SOLF_LLH = 0;
        public const int SOLF_XYZ = 1;
        public const int SOLF_ENU = 2;
        public const int SOLF_NMEA = 3;
        public const int SOLF_GSIF = 4;
        public const int SOLQ_NONE = 0;
        public const int SOLQ_FIX = 1;
        public const int SOLQ_FLOAT = 2;
        public const int SOLQ_SBAS = 3;
        public const int SOLQ_DGPS = 4;
        public const int SOLQ_SINGLE = 5;
        public const int SOLQ_PPP = 6;
        public const int SOLQ_DR = 7;
        public const int MAXSOLQ = 7;
        public const int TIMES_GPST = 0;
        public const int TIMES_UTC = 1;
        public const int TIMES_JST = 2;
        public const int IONOOPT_OFF = 0;
        public const int IONOOPT_BRDC = 1;
        public const int IONOOPT_SBAS = 2;
        public const int IONOOPT_IFLC = 3;
        public const int IONOOPT_EST = 4;
        public const int IONOOPT_TEC = 5;
        public const int IONOOPT_QZS = 6;
        public const int IONOOPT_LEX = 7;
        public const int IONOOPT_STEC = 8;
        public const int TROPOPT_OFF = 0;
        public const int TROPOPT_SAAS = 1;
        public const int TROPOPT_SBAS = 2;
        public const int TROPOPT_EST = 3;
        public const int TROPOPT_ESTG = 4;
        public const int TROPOPT_COR = 5;
        public const int TROPOPT_CORG = 6;
        public const int EPHOPT_BRDC = 0;
        public const int EPHOPT_PREC = 1;
        public const int EPHOPT_SBAS = 2;
        public const int EPHOPT_SSRAPC = 3;
        public const int EPHOPT_SSRCOM = 4;
        public const int EPHOPT_LEX = 5;
        public const int ARMODE_OFF = 0;
        public const int ARMODE_CONT = 1;
        public const int ARMODE_INST = 2;
        public const int ARMODE_FIXHOLD = 3;
        public const int ARMODE_PPPAR = 4;
        public const int ARMODE_PPPAR_ILS = 5;
        public const int ARMODE_WLNL = 6;
        public const int ARMODE_TCAR = 7;
        public const int SBSOPT_LCORR = 1;
        public const int SBSOPT_FCORR = 2;
        public const int SBSOPT_ICORR = 4;
        public const int SBSOPT_RANGE = 8;
        public const int STR_NONE = 0;
        public const int STR_SERIAL = 1;
        public const int STR_FILE = 2;
        public const int STR_TCPSVR = 3;
        public const int STR_TCPCLI = 4;
        public const int STR_UDP = 5;
        public const int STR_NTRIPSVR = 6;
        public const int STR_NTRIPCLI = 7;
        public const int STR_FTP = 8;
        public const int STR_HTTP = 9;
        public const int STRFMT_RTCM2 = 0;
        public const int STRFMT_RTCM3 = 1;
        public const int STRFMT_OEM4 = 2;
        public const int STRFMT_OEM3 = 3;
        public const int STRFMT_UBX = 4;
        public const int STRFMT_SS2 = 5;
        public const int STRFMT_CRES = 6;
        public const int STRFMT_STQ = 7;
        public const int STRFMT_GW10 = 8;
        public const int STRFMT_JAVAD = 9;
        public const int STRFMT_NVS = 10;
        public const int STRFMT_BINEX = 11;
        public const int STRFMT_RT17 = 12;
        public const int STRFMT_LEXR = 13;
        public const int STRFMT_SEPT = 14;
        public const int STRFMT_RINEX = 15;
        public const int STRFMT_SP3 = 16;
        public const int STRFMT_RNXCLK = 17;
        public const int STRFMT_SBAS = 18;
        public const int STRFMT_NMEA = 19;
#if MAXRCVFMT_ConditionalDefinition1
	public const int MAXRCVFMT = 12;
#elif MAXRCVFMT_ConditionalDefinition2
	public const int MAXRCVFMT = 13;
#endif
        public const int STR_MODE_R = 0x1;
        public const int STR_MODE_W = 0x2;
        public const int STR_MODE_RW = 0x3;
        public const int GEOID_EMBEDDED = 0;
        public const int GEOID_EGM96_M150 = 1;
        public const int GEOID_EGM2008_M25 = 2;
        public const int GEOID_EGM2008_M10 = 3;
        public const int GEOID_GSI2000_M15 = 4;
        public const int GEOID_RAF09 = 5;
        public const string COMMENTH = "%";
        public const string MSG_DISCONN = "$_DISCONNECT\r\n";
        public const int DLOPT_FORCE = 0x01;
        public const int DLOPT_KEEPCMP = 0x02;
        public const int DLOPT_HOLDERR = 0x04;
        public const int DLOPT_HOLDLST = 0x08;
        public const double P2_5 = 0.03125;
        public const double P2_6 = 0.015625;
        public const double P2_30 = 9.313225746154785E-10;
        public const double P2_31 = 4.656612873077393E-10;
        public const double P2_32 = 2.328306436538696E-10;
        public const double P2_33 = 1.164153218269348E-10;
        public const double P2_35 = 2.910383045673370E-11;
        public const double P2_38 = 3.637978807091710E-12;
        public const double P2_39 = 1.818989403545856E-12;
        public const double P2_40 = 9.094947017729280E-13;
        public const double P2_43 = 1.136868377216160E-13;
        public const double P2_48 = 3.552713678800501E-15;
        public const double P2_50 = 8.881784197001252E-16;
        public const double P2_55 = 2.775557561562891E-17;
#if FILEPATHSEP_ConditionalDefinition1
	public const char FILEPATHSEP = '\\';
#elif FILEPATHSEP_ConditionalDefinition2
	public const char FILEPATHSEP = '/';
#endif
        public const int BNXSYNC1 = 0xC2;
        public const int BNXSYNC2 = 0xE2;
        public const int BNXSYNC3 = 0xC8;
        public const int BNXSYNC4 = 0xE8;
        public const int BNXSYNC1R = 0xD2;
        public const int BNXSYNC2R = 0xF2;
        public const int BNXSYNC3R = 0xD8;
        public const int BNXSYNC4R = 0xF8;
        public const string CRESSYNC = "$BIN";
        public const int ID_CRESPOS = 1;
        public const int ID_CRESGLOEPH = 65;
        public const int ID_CRESGLORAW = 66;
        public const int ID_CRESRAW2 = 76;
        public const int ID_CRESWAAS = 80;
        public const int ID_CRESIONUTC = 94;
        public const int ID_CRESEPH = 95;
        public const int ID_CRESRAW = 96;
        public const double SNR2CN0_L1 = 30.0;
        public const double SNR2CN0_L2 = 30.0;
        public const int GW10SYNC = 0x8B;
        public const int ID_GW10RAW = 0x08;
        public const int ID_GW10GPS = 0x02;
        public const int ID_GW10SBS = 0x03;
        public const int ID_GW10DGPS = 0x06;
        public const int ID_GW10REF = 0x07;
        public const int ID_GW10SOL = 0x20;
        public const int ID_GW10SATH = 0x22;
        public const int ID_GW10SATO = 0x23;
        public const int ID_GW10EPH = 0x24;
        public const int ID_GW10ALM = 0x25;
        public const int ID_GW10ION = 0x26;
        public const int ID_GW10REPH = 0x27;
        public const int LEN_GW10RAW = 379;
        public const int LEN_GW10GPS = 48;
        public const int LEN_GW10SBS = 40;
        public const int LEN_GW10DGPS = 21;
        public const int LEN_GW10REF = 22;
        public const int LEN_GW10SOL = 227;
        public const int LEN_GW10SATH = 17;
        public const int LEN_GW10SATO = 67;
        public const int LEN_GW10EPH = 68;
        public const int LEN_GW10ALM = 39;
        public const int LEN_GW10ION = 32;
        public const int LEN_GW10REPH = 98;
        public const int OFFWEEK = 1024;
        public const int PREAMB_CNAV = 0x8B;
        public const int OEM4SYNC1 = 0xAA;
        public const int OEM4SYNC2 = 0x44;
        public const int OEM4SYNC3 = 0x12;
        public const int OEM3SYNC1 = 0xAA;
        public const int OEM3SYNC2 = 0x44;
        public const int OEM3SYNC3 = 0x11;
        public const int OEM4HLEN = 28;
        public const int OEM3HLEN = 12;
        public const int ID_ALMANAC = 73;
        public const int ID_GLOALMANAC = 718;
        public const int ID_GLOEPHEMERIS = 723;
        public const int ID_IONUTC = 8;
        public const int ID_RANGE = 43;
        public const int ID_RANGECMP = 140;
        public const int ID_RAWALM = 74;
        public const int ID_RAWEPHEM = 41;
        public const int ID_RAWWAASFRAME = 287;
        public const int ID_QZSSIONUTC = 1347;
        public const int ID_QZSSRAWEPHEM = 1330;
        public const int ID_QZSSRAWSUBFRAME = 1331;
        public const int ID_RAWSBASFRAME = 973;
        public const int ID_GALEPHEMERIS = 1122;
        public const int ID_GALALMANAC = 1120;
        public const int ID_GALCLOCK = 1121;
        public const int ID_GALIONO = 1127;
        public const int ID_GALFNAVRAWPAGE = 1413;
        public const int ID_GALINAVRAWWORD = 1414;
        public const int ID_RAWCNAVFRAME = 1066;
        public const int ID_BDSEPHEMERIS = 1696;
        public const int ID_ALMB = 18;
        public const int ID_IONB = 16;
        public const int ID_UTCB = 17;
        public const int ID_FRMB = 54;
        public const int ID_RALB = 15;
        public const int ID_RASB = 66;
        public const int ID_REPB = 14;
        public const int ID_RGEB = 32;
        public const int ID_RGED = 65;
        public const double WL1 = 0.1902936727984;
        public const double WL2 = 0.2442102134246;
        public const double MAXVAL = 8388608.0;
        public const int OFF_FRQNO = -7;
        public const int NVSSYNC = 0x10;
        public const int NVSENDMSG = 0x03;
        public const int NVSCFG = 0x06;
        public const int ID_XF5RAW = 0xf5;
        public const int ID_X4AIONO = 0x4a;
        public const int ID_X4BTIME = 0x4b;
        public const int ID_XF7EPH = 0xf7;
        public const int ID_XE5BIT = 0xe5;
        public const int ID_XD7ADVANCED = 0xd7;
        public const int ID_X02RATEPVT = 0x02;
        public const int ID_XF4RATERAW = 0xf4;
        public const int ID_XD7SMOOTH = 0xd7;
        public const int ID_XD5BIT = 0xd5;
        public const int LEXFRMPREAMB = 0x1ACFFC1Du;
        public const int LEXRSYNC1 = 0xAA;
        public const int LEXRSYNC2 = 0x55;
        public const int MAXLEXRLEN = 8192;
        public const int ID_LEXRAW = 0x0002;
        public const int ID_LEXMSG = 0x0015;
        public const int STX = 2;
        public const int ETX = 3;
        public const int GENOUT = 0x40;
        public const int RETSVDATA = 0x55;
        public const int RAWDATA = 0x57;
        public const int BIG_ENDIAN = 1;
        public const int LITTLE_ENDIAN = 2;
        public const int M_CONCISE = 1;
        public const int M_ENHANCED = 2;
        public const int M_WEEK_OPTION = 1;
        public const int M_WEEK_SCAN = 2;
        public const int SBF_SYNC1 = 0x24;
        public const int SBF_SYNC2 = 0x40;
        public const int ID_MEASEPOCH = 4027;
        public const int ID_MEASEPOCHEXTRA = 4000;
        public const int ID_MEASEPOCH_END = 5922;
        public const int ID_GPSRAWCA = 4017;
        public const int ID_GPSRAWL2C = 4018;
        public const int ID_GPSRAWL5 = 4019;
        public const int ID_GLORAWCA = 4026;
        public const int ID_GALRAWFNAV = 4022;
        public const int ID_GALRAWINAV = 4023;
        public const int ID_GEORAWL1 = 4020;
        public const int ID_GEORAWL5 = 4021;
        public const int ID_COMPRAW = 4047;
        public const int ID_QZSSL1CA = 4066;
        public const int ID_QZSSL2C = 4067;
        public const int ID_QZSSL5 = 4068;
        public const int ID_GPSNAV = 5891;
        public const int ID_GPSALM = 5892;
        public const int ID_GPSION = 5893;
        public const int ID_GPSUTC = 5894;
        public const int ID_GLONAV = 4004;
        public const int ID_GLOALM = 4005;
        public const int ID_GLOTIME = 4036;
        public const int ID_GALNAV = 4002;
        public const int ID_GALALM = 4003;
        public const int ID_GALION = 4030;
        public const int ID_GALUTC = 4031;
        public const int ID_GALGSTGPS = 4032;
        public const int ID_GEOMTOO = 5925;
        public const int ID_GEONAV = 5896;
        public const int ID_PVTCART = 4006;
        public const int ID_PVTGEOD = 4007;
        public const int ID_DOP = 4001;
        public const int ID_PVTSATCART = 4008;
        public const int ID_ENDOFPVT = 5921;
        public const int ID_RXTIME = 5914;
        public const int ID_DIFFCORRIN = 5919;
        public const int ID_BASESTATION = 5949;
        public const int ID_CHNSTATUS = 4013;
        public const int ID_RXSTATUS = 4014;
        public const int ID_RXSETUP = 5902;
        public const int ID_COMMENT = 5936;
        public const int ID_SATVISIBILITY = 4012;
        public const int ID_BBSMPS = 4040;
        public const int STQSYNC1 = 0xA0;
        public const int STQSYNC2 = 0xA1;
        public const int ID_STQTIME = 0xDC;
        public const int ID_STQRAW = 0xDD;
        public const int ID_STQGPS = 0xE0;
        public const int ID_STQGLO = 0xE1;
        public const int ID_STQGLOE = 0x5C;
        public const int ID_STQBDSD1 = 0xE2;
        public const int ID_STQBDSD2 = 0xE3;
        public const int ID_RESTART = 0x01;
        public const int ID_CFGSERI = 0x05;
        public const int ID_CFGFMT = 0x09;
        public const int ID_CFGRATE = 0x12;
        public const int ID_CFGBIN = 0x1E;
        public const int ID_GETGLOEPH = 0x5B;
        public const int SS2SOH = 0x01;
        public const int ID_SS2LLH = 20;
        public const int ID_SS2ECEF = 21;
        public const int ID_SS2EPH = 22;
        public const int ID_SS2RAW = 23;
        public const int ID_SS2SBAS = 67;
        public const int UBXSYNC1 = 0xB5;
        public const int UBXSYNC2 = 0x62;
        public const int UBXCFG = 0x06;
        public const int ID_NAVSOL = 0x0106;
        public const int ID_NAVTIME = 0x0120;
        public const int ID_RXMRAW = 0x0210;
        public const int ID_RXMSFRB = 0x0211;
        public const int ID_RXMSFRBX = 0x0213;
        public const int ID_RXMRAWX = 0x0215;
        public const int ID_TRKD5 = 0x030A;
        public const int ID_TRKMEAS = 0x0310;
        public const int ID_TRKSFRBX = 0x030F;
        public const int FU1 = 1;
        public const int FU2 = 2;
        public const int FU4 = 3;
        public const int FI1 = 4;
        public const int FI2 = 5;
        public const int FI4 = 6;
        public const int FR4 = 7;
        public const int FR8 = 8;
        public const int FS32 = 9;
        public const double P2_10 = 0.0009765625;
        public const double SIZP = 0.2;
        public const double SIZR = 0.3;
        public const double TINT = 30.0;
        public const string MARKICON = "http://maps.google.com/mapfiles/kml/pal2/icon18.png";
        public const int NOUTFILE = 7;
        public const double TSTARTMARGIN = 60.0;
        public const int MAXPRM = 400000;
        public const int NMAX_STA = 2048;
        public const int NMAX_TYPE = 256;
        public const int NMAX_URL = 1024;
        public const string FTP_CMD = "wget";
#else
        public const int FTP_TIMEOUT = 30;
#endif
        public const string FTP_LISTING = ".listing";
        public const int FTP_NOFILE = 2048;
        public const int HTTP_NOFILE = 1;
        public const int FTP_RETRY = 3;
        public const double RE_GLO = 6378136.0;
        public const double MU_GPS = 3.9860050E14;
        public const double MU_GLO = 3.9860044E14;
        public const double MU_GAL = 3.986004418E14;
        public const double MU_CMP = 3.986004418E14;
        public const double J2_GLO = 1.0826257E-3;
        public const double OMGE_GLO = 7.292115E-5;
        public const double OMGE_GAL = 7.2921151467E-5;
        public const double OMGE_CMP = 7.292115E-5;
        public const double SIN_5 = -0.0871557427476582;
        public const double COS_5 = 0.9961946980917456;
        public const double ERREPH_GLO = 5.0;
        public const double TSTEP = 60.0;
        public const double RTOL_KEPLER = 1E-14;
        public const double DEFURASSR = 0.15;
        public const double MAXECORSSR = 10.0;
        public const double MAXAGESSR = 90.0;
        public const double MAXAGESSR_HRCLK = 10.0;
        public const double STD_BRDCCLK = 30.0;
        public const int MAX_ITER_KEPLER = 30;
        public const double MIN_EL = 0.0;
        public const double MIN_HGT = -1000.0;
        public const int LOOPMAX = 10000;
        public const string SWTOPT = "0:off,1:on";
        public const string MODOPT = "0:single,1:dgps,2:kinematic,3:static,4:movingbase,5:fixed,6:ppp-kine,7:ppp-static,8:ppp-fixed";
        public const string FRQOPT = "1:l1,2:l1+l2,3:l1+l2+l5,4:l1+l2+l5+l6,5:l1+l2+l5+l6+l7";
        public const string TYPOPT = "0:forward,1:backward,2:combined";
        public const string IONOPT = "0:off,1:brdc,2:sbas,3:dual-freq,4:est-stec,5:ionex-tec,6:qzs-brdc,7:qzs-lex,8:vtec_sf,9:vtec_ef,10:gtec";
        public const string TRPOPT = "0:off,1:saas,2:sbas,3:est-ztd,4:est-ztdgrad";
        public const string EPHOPT = "0:brdc,1:precise,2:brdc+sbas,3:brdc+ssrapc,4:brdc+ssrcom";
        public const string NAVOPT = "1:gps+2:sbas+4:glo+8:gal+16:qzs+32:comp";
        public const string GAROPT = "0:off,1:on,2:autocal";
        public const string SOLOPT = "0:llh,1:xyz,2:enu,3:nmea";
        public const string TSYOPT = "0:gpst,1:utc,2:jst";
        public const string TFTOPT = "0:tow,1:hms";
        public const string DFTOPT = "0:deg,1:dms";
        public const string HGTOPT = "0:ellipsoidal,1:geodetic";
        public const string GEOOPT = "0:internal,1:egm96,2:egm08_2.5,3:egm08_1,4:gsi2000";
        public const string STAOPT = "0:all,1:single";
        public const string STSOPT = "0:off,1:state,2:residual";
        public const string ARMOPT = "0:off,1:continuous,2:instantaneous,3:fix-and-hold";
        public const string POSOPT = "0:llh,1:xyz,2:single,3:posfile,4:rinexhead,5:rtcm";
        public const string TIDEOPT = "0:off,1:on,2:otl";
        public const int MAXITR = 10;
        public const double ERR_ION = 5.0;
        public const double ERR_TROP = 3.0;
        public const double ERR_SAAS = 0.3;
        public const double ERR_BRDCI = 0.5;
        public const double ERR_CBIAS = 0.3;
        public const double REL_HUMI = 0.7;
        public const int MAXPRCDAYS = 100;
        public const int MAXINFILE = 1000;
        public const double GME = 3.986004415E+14;
        public const double GMS = 1.327124E+20;
        public const double GMM = 4.902801E+12;
        public const double MIN_ARC_GAP = 300.0;
        public const double CONST_AMB = 0.001;
        public const double THRES_RES = 0.3;
        public const double LOG_PI = 1.14472988584940017;
        public const double SQRT2 = 1.41421356237309510;
        public const int NMAX = 10;
        public const double MAXDTE = 900.0;
        public const double EXTERR_CLK = 1E-3;
        public const double EXTERR_EPH = 5E-7;
        public const int LEXFRMLEN = 2000;
        public const int LEXHDRLEN = 49;
        public const int LEXRSLEN = 256;
        public const double LEXEPHMAXAGE = 360.0;
        public const double LEXIONMAXAGE = 3600.0;
        public const int RTCM3PREAMB = 0xD3;
        public const int LEXHEADLEN = 24;
        public const double P2_66 = 1.355252715606881E-20;
        public const int NUMSYS = 6;
        public const int MAXPOSHEAD = 1024;
        public const int MINFREQ_GLO = -7;
        public const int MAXFREQ_GLO = 13;
        public const int NINCOBS = 262144;
        public const int RTCM2PREAMB = 0x66;
        public const double PRUNIT_GPS = 299792.458;
        public const double PRUNIT_GLO = 599584.916;
        public const double P2_34 = 5.820766091346740E-11;
        public const double P2_46 = 1.421085471520200E-14;
        public const double P2_59 = 1.734723475976810E-18;
        public const int _POSIX_C_SOURCE = 199309;
        public const int POLYCRC32 = 0xEDB88320u;
        public const int POLYCRC24Q = 0x1864CFBu;
        public const double INIT_ZWD = 0.15;
        public const double PRN_HWBIAS = 1E-6;
        public const int GAP_RESION = 120;
        public const double MAXACC = 30.0;
        public const double VAR_HOLDAMB = 0.001;
        public const int WEEKOFFSET = 1024;
        public const int MAXFIELD = 64;
        public const int MAXNMEA = 256;
        public const double KNOT2M = 0.514444444;
        public const int TINTACT = 200;
        public const int SERIBUFFSIZE = 4096;
        public const int TIMETAGH_LEN = 64;
        public const int MAXCLI = 32;
        public const int MAXSTATMSG = 32;
        public const int NTRIP_CLI_PORT = 2101;
        public const int NTRIP_SVR_PORT = 80;
        public const int NTRIP_MAXRSP = 32768;
        public const int NTRIP_MAXSTR = 256;
        public const string NTRIP_RSP_OK_CLI = "ICY 200 OK\r\n";
        public const string NTRIP_RSP_OK_SVR = "OK\r\n";
        public const string NTRIP_RSP_SRCTBL = "SOURCETABLE 200 OK\r\n";
        public const string NTRIP_RSP_TBLEND = "ENDSOURCETABLE";
        public const string NTRIP_RSP_HTTP = "HTTP/";
        public const string NTRIP_RSP_ERROR = "ERROR";
        public const double DE2RA = 0.174532925E-1;
        public const double E6A = 1.E - 6;
        public const double PIO2 = 1.57079633;
        public const double QO = 120.0;
        public const double SO = 78.0;
        public const double TOTHRD = 0.66666667;
        public const double TWOPI = 6.2831853;
        public const double X3PIO2 = 4.71238898;
        public const double XJ2 = 1.082616E-3;
        public const double XJ3 = -0.253881E-5;
        public const double XJ4 = -1.65597E-6;
        public const double XKE = 0.743669161E-1;
        public const double XKMPER = 6378.135;
        public const double XMNPDA = 1440.0;
        public const double AE = 1.0;
        public const double CK2 = 5.413080E-4;
        public const double CK4 = 0.62098875E-6;
        public const double QOMS2T = 1.88027916E-9;
        public const double S = 1.01222928;
    }
}
