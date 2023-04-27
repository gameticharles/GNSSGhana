using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ghGPS.Classes.rcv
{
    public static class GlobalMembersGw10
    {
        /*------------------------------------------------------------------------------
        * gw10.c : furuno GW-10 receiver functions
        *
        *          Copyright (C) 2011-2012 by T.TAKASU, All rights reserved.
        *
        * reference :
        *     [1] Furuno, SBAS/GPS receiver type GW-10 III manual, July 2004
        *
        * version : $Revision:$ $Date:$
        * history : 2011/05/27  1.0  new
        *           2011/07/01  1.1  suppress warning
        *           2012/02/14  1.2  add decode of gps message (0x02)
        *-----------------------------------------------------------------------------*/


        internal const string rcsid = "$Id:$";



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
        /* message length ------------------------------------------------------------*/
        internal static int msglen(byte id)
        {
            switch (id)
            {
                case DefineConstants.ID_GW10RAW:
                    return DefineConstants.LEN_GW10RAW;
                case DefineConstants.ID_GW10GPS:
                    return DefineConstants.LEN_GW10GPS;
                case DefineConstants.ID_GW10SBS:
                    return DefineConstants.LEN_GW10SBS;
                case DefineConstants.ID_GW10DGPS:
                    return DefineConstants.LEN_GW10DGPS;
                case DefineConstants.ID_GW10REF:
                    return DefineConstants.LEN_GW10REF;
                case DefineConstants.ID_GW10SOL:
                    return DefineConstants.LEN_GW10SOL;
                case DefineConstants.ID_GW10SATH:
                    return DefineConstants.LEN_GW10SATH;
                case DefineConstants.ID_GW10SATO:
                    return DefineConstants.LEN_GW10SATO;
                case DefineConstants.ID_GW10EPH:
                    return DefineConstants.LEN_GW10EPH;
                case DefineConstants.ID_GW10ALM:
                    return DefineConstants.LEN_GW10ALM;
                case DefineConstants.ID_GW10ION:
                    return DefineConstants.LEN_GW10ION;
                case DefineConstants.ID_GW10REPH:
                    return DefineConstants.LEN_GW10REPH;
            }
            return 0;
        }
        /* compute checksum ----------------------------------------------------------*/
        internal static int chksum(byte[] buff, int n)
        {
            byte cs = 0;
            int i;
            for (i = 1; i < n - 1; i++)
            {
                cs += buff[i];
            }
            return buff[n - 1] == cs;
        }
        /* adjust weekly rollover of gps time ----------------------------------------*/
        internal static int adjweek(raw_t raw, double tow)
        {
            double tow_p;
            int week;

            if (raw.time.time == 0)
            {
                return 0;
            }
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: tow_p=time2gpst(raw->time,&week);
            tow_p = GlobalMembersRtkcmn.time2gpst(new gtime_t(raw.time), ref week);
            if (tow < tow_p - 302400.0)
            {
                tow += 604800.0;
            }
            else if (tow > tow_p + 302400.0)
            {
                tow -= 604800.0;
            }
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: raw->time=gpst2time(week,tow);
            raw.time.CopyFrom(GlobalMembersRtkcmn.gpst2time(week, tow));
            return 1;
        }
        /* bcd to number -------------------------------------------------------------*/
        internal static int bcd2num(byte bcd)
        {
            return (bcd >> 4) * 10 + (bcd & 0xF);
        }
        /* decode raw obs data -------------------------------------------------------*/
        internal static int decode_gw10raw(raw_t raw)
        {
            double tow;
            double tows;
            double toff;
            double pr;
            double cp;
            int i;
            int j;
            int n;
            int prn;
            int flg;
            int sat;
            int snr;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: byte *p=raw->buff+2;
            byte p = raw.buff + 2;

            GlobalMembersRtkcmn.trace(4, "decode_gw10raw: len=%d\n", raw.len);

            tow = GlobalMembersBinex.R8(p);
            tows = Math.Floor(tow * 1000.0 + 0.5) / 1000.0; // round by 10ms
            toff = DefineConstants.CLIGHT * (tows - tow); // time tag offset (m)
            if (GlobalMembersBinex.adjweek(raw, tows) == null)
            {
                GlobalMembersRtkcmn.trace(2, "decode_gw10raw: no gps week infomation\n");
                return 0;
            }
            for (i = n = 0, p += 8; i < 16 && n < DefineConstants.MAXOBS; i++, p += 23)
            {
                if (((byte)(p + 1)) != 1)
                    continue;
                prn = ((byte)(p));
                if ((sat = GlobalMembersRtkcmn.satno(prn <= DefineConstants.MAXPRNGPS ? DefineConstants.SYS_GPS : DefineConstants.SYS_SBS, prn)) == 0)
                {
                    GlobalMembersRtkcmn.trace(2, "gw10raw satellite number error: prn=%d\n", prn);
                    continue;
                }
                pr = GlobalMembersBinex.R8(p + 2) - toff;
                snr = GlobalMembersBinex.U2(p + 16);
                cp = -(int)(GlobalMembersBinex.U4(p + 18)) / 256.0 - toff / GlobalMembersRtkcmn.lam_carr[0];
                flg = ((byte)(p + 22));
                if ((flg & 0x3) != 0)
                {
                    GlobalMembersRtkcmn.trace(2, "gw10raw raw data invalid: prn=%d\n", prn);
                    continue;
                }
                raw.obs.data[n].time = raw.time;
                raw.obs.data[n].sat = sat;
                raw.obs.data[n].P[0] = pr;
                raw.obs.data[n].L[0] = ((flg & 0x80) != 0) ? 0.0 : (((flg & 0x40) != 0) ? cp - 0.5 : cp);
                raw.obs.data[n].D[0] = 0.0;
                raw.obs.data[n].SNR[0] = (byte)(snr * 4.0 + 0.5);
                raw.obs.data[n].LLI[0] = ((flg & 0x80) != 0) ? 1 : 0;
                raw.obs.data[n].code[0] = DefineConstants.CODE_L1C;

                for (j = 1; j < DefineConstants.NFREQ; j++)
                {
                    raw.obs.data[n].L[j] = raw.obs.data[n].P[j] = 0.0;
                    raw.obs.data[n].D[j] = 0.0;
                    raw.obs.data[n].SNR[j] = raw.obs.data[n].LLI[j] = 0;
                    raw.obs.data[n].code[j] = DefineConstants.CODE_NONE;
                }
                n++;
            }
            raw.obs.n = n;
            return 1;
        }
        /* check partity -------------------------------------------------------------*/
        public static int check_parity(uint word, byte[] data)
        {
            uint[] hamming = { 0xBB1F3480, 0x5D8F9A40, 0xAEC7CD00, 0x5763E680, 0x6BB1F340, 0x8B7A89C0 };
            uint parity = 0;
            uint w;
            int i;

            for (i = 0; i < 6; i++)
            {
                parity <<= 1;
                for (w = (word & hamming[i]) >> 6; w != 0; w >>= 1)
                {
                    parity ^= w & 1;
                }
            }
            if (parity != (word & 0x3F))
            {
                return 0;
            }

            for (i = 0; i < 3; i++)
            {
                data[i] = (byte)(word >> (22 - i * 8));
            }
            return 1;
        }
        /* decode gps message --------------------------------------------------------*/
        internal static int decode_gw10gps(raw_t raw)
        {
            eph_t eph = new eph_t();
            double tow;
            double[] ion = { 0, null, null, null, null, null, null, null };
            double[] utc = { 0, null, null, null };
            uint buff = 0;
            int i;
            int prn;
            int sat;
            int id;
            int leaps;
            //C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on this variable, so pointers on this variable are left unchanged:
            byte* p = raw.buff + 2;
            byte[] subfrm = new byte[30];

            GlobalMembersRtkcmn.trace(4, "decode_gw10gps: len=%d\n", raw.len);

            tow = GlobalMembersBinex.U4(p) / 1000.0;
            p += 4;
            prn = ((byte)(p));
            p += 1;
            if ((sat = GlobalMembersRtkcmn.satno(DefineConstants.SYS_GPS, prn)) == 0)
            {
                GlobalMembersRtkcmn.trace(2, "gw10 gps satellite number error: tow=%.1f prn=%d\n", tow, prn);
                return -1;
            }
            for (i = 0; i < 10; i++)
            {
                buff = (buff << 30) | GlobalMembersBinex.U4(p);
                p += 4;

                /* check parity of word */
                if (GlobalMembersGw10.check_parity(buff, subfrm + i * 3) == 0)
                {
                    GlobalMembersRtkcmn.trace(2, "gw10 gps frame parity error: tow=%.1f prn=%2d word=%2d\n", tow, prn, i + 1);
                    return -1;
                }
            }
            id = GlobalMembersRtkcmn.getbitu(subfrm, 43, 3); // subframe id

            if (id < 1 || 5 < id)
            {
                GlobalMembersRtkcmn.trace(2, "gw10 gps frame id error: tow=%.1f prn=%2d id=%d\n", tow, prn, id);
                return -1;
            }
            for (i = 0; i < 30; i++)
            {
                raw.subfrm[sat - 1, i + (id - 1) * 30] = subfrm[i];
            }

            if (id == 3) // decode ephemeris
            {
                if (GlobalMembersRcvraw.decode_frame(raw.subfrm[sat - 1], eph, null, null, null, null) != 1 || GlobalMembersRcvraw.decode_frame(raw.subfrm[sat - 1] + 30, eph, null, null, null, null) != 2 || GlobalMembersRcvraw.decode_frame(raw.subfrm[sat - 1] + 60, eph, null, null, null, null) != 3)
                {
                    return 0;
                }
                if (!StringFunctions.StrStr(raw.opt, "-EPHALL"))
                {
                    if (eph.iode == raw.nav.eph[sat - 1].iode) // unchanged
                    {
                        return 0;
                    }
                }
                eph.sat = sat;
                raw.nav.eph[sat - 1] = eph;
                raw.ephsat = sat;
                return 2;
            }
            else if (id == 4) // decode ion-utc parameters
            {
                if (GlobalMembersRcvraw.decode_frame(subfrm, null, null, ref ion, ref utc, ref leaps) != 4)
                {
                    return 0;
                }
                if (GlobalMembersRtkcmn.norm(ion, 8) > 0.0 && GlobalMembersRtkcmn.norm(utc, 4) > 0.0 && leaps != 0)
                {
                    for (i = 0; i < 8; i++)
                    {
                        raw.nav.ion_gps[i] = ion[i];
                    }
                    for (i = 0; i < 4; i++)
                    {
                        raw.nav.utc_gps[i] = utc[i];
                    }
                    raw.nav.leaps = leaps;
                    return 9;
                }
            }
            return 0;
        }
        /* decode waas messages ------------------------------------------------------*/
        internal static int decode_gw10sbs(raw_t raw)
        {
            double tow;
            int i;
            int prn;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: byte *p=raw->buff+2;
            byte p = raw.buff + 2;

            GlobalMembersRtkcmn.trace(4, "decode_gw10sbs : len=%d\n", raw.len);

            tow = GlobalMembersBinex.U4(p) / 1000.0;
            prn = ((byte)(p + 4));
            if (prn < DefineConstants.MINPRNSBS || DefineConstants.MAXPRNSBS < prn)
            {
                GlobalMembersRtkcmn.trace(2, "gw10 sbs satellite number error: prn=%d\n", prn);
                return -1;
            }
            raw.sbsmsg.prn = prn;
            raw.sbsmsg.tow = (int)tow;
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: tow=time2gpst(raw->time,&raw->sbsmsg.week);
            tow = GlobalMembersRtkcmn.time2gpst(new gtime_t(raw.time), ref raw.sbsmsg.week);
            if (raw.sbsmsg.tow < tow - 302400.0)
            {
                raw.sbsmsg.week++;
            }
            else if (raw.sbsmsg.tow > tow + 302400.0)
            {
                raw.sbsmsg.week--;
            }

            for (i = 0; i < 29; i++)
            {
                raw.sbsmsg.msg[i] = *(p + 5 + i);
            }
            raw.sbsmsg.msg[28] &= 0xC0;
            return 3;
        }
        /* decode raw ephemereris ----------------------------------------------------*/
        internal static int decode_gw10reph(raw_t raw)
        {
            eph_t eph = new eph_t();
            double tow;
            int i;
            int week;
            int prn;
            int sat;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: byte *p=raw->buff+2,buff[90];
            byte p = raw.buff + 2;
            byte[] buff = new byte[90];

            GlobalMembersRtkcmn.trace(4, "decode_gw10reph: len=%d\n", raw.len);

            prn = ((byte)(p));
            if ((sat = GlobalMembersRtkcmn.satno(DefineConstants.SYS_GPS, prn)) == 0)
            {
                GlobalMembersRtkcmn.trace(2, "gw10 raw ephemeris satellite number error: prn=%d\n", prn);
                return -1;
            }
            for (i = 0; i < 90; i++)
            {
                buff[i] = *(p + 1 + i);
            }
            if (GlobalMembersRcvraw.decode_frame(buff, eph, null, null, null, null) != 1 || GlobalMembersRcvraw.decode_frame(buff + 30, eph, null, null, null, null) != 2 || GlobalMembersRcvraw.decode_frame(buff + 60, eph, null, null, null, null) != 3)
            {
                GlobalMembersRtkcmn.trace(2, "gw10 raw ephemeris navigation frame error: prn=%d\n", prn);
                return -1;
            }
            /* set time if no time avaliable */
            if (raw.time.time == 0)
            {
                tow = GlobalMembersRtkcmn.getbitu(buff, 24, 17) * 6.0;
                week = GlobalMembersRtkcmn.getbitu(buff, 48, 10) + DefineConstants.OFFWEEK;
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                //ORIGINAL LINE: raw->time=timeadd(gpst2time(week,tow),24.0);
                raw.time.CopyFrom(GlobalMembersRtkcmn.timeadd(GlobalMembersRtkcmn.gpst2time(week, tow), 24.0));
            }
            if (!StringFunctions.StrStr(raw.opt, "-EPHALL"))
            {
                if (eph.iode == raw.nav.eph[sat - 1].iode) // unchanged
                {
                    return 0;
                }
            }
            eph.sat = sat;
            raw.nav.eph[sat - 1] = eph;
            raw.ephsat = sat;
            return 2;
        }
        /* decode solution -----------------------------------------------------------*/
        internal static int decode_gw10sol(raw_t raw)
        {
            gtime_t time = new gtime_t();
            double[] ep = { 0, null, null, null, null, null };
            double sec;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: byte *p=raw->buff+6;
            byte p = raw.buff + 6;

            GlobalMembersRtkcmn.trace(4, "decode_gw10sol : len=%d\n", raw.len);

            if (GlobalMembersBinex.U2(p + 42) & 0xC00) // time valid?
            {
                GlobalMembersRtkcmn.trace(2, "gw10 sol time/day invalid\n");
                return 0;
            }
            sec = GlobalMembersBinex.U4(p + 27) / 16384.0;
            sec = Math.Floor(sec * 1000.0 + 0.5) / 1000.0;
            ep[2] = GlobalMembersGw10.bcd2num(p[31]);
            ep[1] = GlobalMembersGw10.bcd2num(p[32]);
            ep[0] = GlobalMembersGw10.bcd2num(p[33]) * 100 + GlobalMembersGw10.bcd2num(p[34]);
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: time=utc2gpst(timeadd(epoch2time(ep),sec));
            time.CopyFrom(GlobalMembersRtkcmn.utc2gpst(GlobalMembersRtkcmn.timeadd(GlobalMembersRtkcmn.epoch2time(ep), sec)));

            /* set time if no time available */
            if (raw.time.time == 0)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                //ORIGINAL LINE: raw->time=time;
                raw.time.CopyFrom(time);
            }
            return 0;
        }
        /* decode gw10 raw message ---------------------------------------------------*/
        internal static int decode_gw10(raw_t raw)
        {
            int type = ((byte)(raw.buff + 1));

            GlobalMembersRtkcmn.trace(3, "decode_gw10: type=0x%02X len=%d\n", type, raw.len);

            if (raw.outtype != 0)
            {
                raw.msgtype = string.Format("GW10 0x{0:X2} ({1,4:D}):", type, raw.len);
            }
            switch (type)
            {
                case DefineConstants.ID_GW10RAW:
                    return GlobalMembersGw10.decode_gw10raw(raw);
                case DefineConstants.ID_GW10GPS:
                    return GlobalMembersGw10.decode_gw10gps(raw);
                case DefineConstants.ID_GW10SBS:
                    return GlobalMembersGw10.decode_gw10sbs(raw);
                case DefineConstants.ID_GW10REPH:
                    return GlobalMembersGw10.decode_gw10reph(raw);
                case DefineConstants.ID_GW10SOL:
                    return GlobalMembersGw10.decode_gw10sol(raw);
            }
            return 0;
        }
        /* input gw10 raw message ------------------------------------------------------
        * input next gw10 raw message from stream
        * args   : raw_t *raw   IO     receiver raw data control struct
        *          unsigned char data I stream data (1 byte)
        * return : status (-1: error message, 0: no message, 1: input observation data,
        *                  2: input ephemeris, 3: input sbas message,
        *                  9: input ion/utc parameter)
        *
        * notes  : to specify input options, set raw->opt to the following option
        *          strings separated by spaces.
        *
        *          -EPHALL    : input all ephemerides
        *
        *-----------------------------------------------------------------------------*/
        public static int input_gw10(raw_t raw, byte data)
        {
            int stat;
            GlobalMembersRtkcmn.trace(5, "input_gw10: data=%02x\n", data);

            raw.buff[raw.nbyte++] = data;

            /* synchronize frame */
            if (raw.buff[0] != DefineConstants.GW10SYNC)
            {
                raw.nbyte = 0;
                return 0;
            }
            if (raw.nbyte >= 2 && (raw.len = GlobalMembersGw10.msglen(raw.buff[1])) == 0)
            {
                raw.nbyte = 0;
                return 0;
            }
            if (raw.nbyte < 2 || raw.nbyte < raw.len)
            {
                return 0;
            }

            if (GlobalMembersCrescent.chksum(raw.buff, raw.len) == 0)
            {
                GlobalMembersRtkcmn.tracet(2, "gw10 message checksum error msg=%d\n", raw.buff[1]);
                raw.buff[0] = 0;
                raw.nbyte = 0;
                return -1;
            }
            /* decode gw10 raw message */
            stat = GlobalMembersGw10.decode_gw10(raw);

            raw.buff[0] = 0;
            raw.nbyte = 0;

            return stat;
        }
        /* input gw10 raw message from file --------------------------------------------
        * input next gw10 raw message from file
        * args   : raw_t  *raw   IO     receiver raw data control struct
        *          FILE   *fp    I      file pointer
        * return : status(-2: end of file, -1...9: same as above)
        *-----------------------------------------------------------------------------*/
        public static int input_gw10f(raw_t raw, FILE fp)
        {
            int i;
            int data;
            int ret;

            GlobalMembersRtkcmn.trace(4, "input_gw10f:\n");

            for (i = 0; i < 4096; i++)
            {
                if ((data = fgetc(fp)) == EOF)
                {
                    return -2;
                }
                if ((ret = GlobalMembersGw10.input_gw10(raw, (byte)data)) != 0)
                {
                    return ret;
                }
            }
            return 0; // return at every 4k bytes
        }
    }
}
