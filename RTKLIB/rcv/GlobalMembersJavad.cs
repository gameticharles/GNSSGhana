using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ghGPS.Classes.rcv
{
    public static class GlobalMembersJavad
    {
        /*------------------------------------------------------------------------------
        * javad.c : javad receiver dependent functions
        *
        *          Copyright (C) 2011-2014 by T.TAKASU, All rights reserved.
        *
        * reference :
        *     [1] Javad GNSS, GREIS GNSS Receiver External Interface Specification,
        *         Reflects Firmware Version 3.2.0, July 22, 2010
        *     [2] Javad navigation systemms, GPS Receiver Interface Language (GRIL)
        *         Reference Guide Rev 2.2, Reflects Firmware Version 2.6.0
        *     [3] Javad GNSS, User visible changes in the firmware vesion 3.4.0 since
        *         version 3.3.x (NEWS_3_4_0.txt)
        *     [4] Javad GNSS, GREIS GNSS Receiver External Interface Specification,
        *         Reflects Firmware Version 3.4.6, October 9, 2012
        *     [5] Javad GNSS, GREIS GNSS Receiver External Interface Specification,
        *         Reflects Firmware Version 3.5.4, January 30, 2014
        *
        * version : $Revision:$ $Date:$
        * history : 2011/05/27 1.0  new
        *           2011/07/07 1.1  fix QZSS IODC-only-update problem
        *           2012/07/17 1.2  change GALILEO scale factor for short pseudorange
        *           2012/10/18 1.3  change receiver options and rinex obs code
        *           2013/01/24 1.4  change compass factor for short pseudorange
        *                           add raw option -NOET
        *           2013/02/23 1.6  fix memory access violation problem on arm
        *           2013/05/08 1.7  fix bug on week number of galileo ephemeris
        *           2014/05/23 1.8  support beidou
        *           2014/06/23 1.9  support [lD] for glonass raw navigation data
        *           2014/08/26 1.10 fix bug on decoding iode in glonass ephemeris [NE]
        *           2014/10/20 1.11 fix bug on receiver option -GL*,-RL*,-JL*
        *-----------------------------------------------------------------------------*/


        internal const string rcsid = "$Id:$";

        internal static ushort U2(ref byte p)
        {
            ushort u;
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'memcpy' has no equivalent in C#:
            memcpy(u, p, 2);
            return u;
        }
        internal static uint U4(ref byte p)
        {
            uint u;
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'memcpy' has no equivalent in C#:
            memcpy(u, p, 4);
            return u;
        }
        internal static short I2(ref byte p)
        {
            short i;
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'memcpy' has no equivalent in C#:
            memcpy(i, p, 2);
            return i;
        }
        internal static int I4(ref byte p)
        {
            int i;
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'memcpy' has no equivalent in C#:
            memcpy(i, p, 4);
            return i;
        }

        internal static float R4(ref byte p)
        {
            if (GlobalMembersBinex.U4(p) == 0x7FC00000) // quiet nan
            {
                return 0.0f;
            }
            return *(float)p;
        }
        //C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on the parameter 'p', so pointers on this parameter are left unchanged:
        internal static double R8(byte* p)
        {
            double value;
            //C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on this variable, so pointers on this variable are left unchanged:
            byte* q = (byte)&value;
            int i;
            if (GlobalMembersBinex.U4(p + 4) == 0x7FF80000 && GlobalMembersBinex.U4(p) == 0) // quiet nan
            {
                return 0.0;
            }
            for (i = 0; i < 8; i++)
            {
                *q++ = *p++;
            }
            return value;
        }
        /* decode message length -----------------------------------------------------*/
        internal static int decodelen(byte[] buff)
        {
            uint len;
            if (!(('0' <= (buff[0]) && (buff[0]) <= '9') || ('A' <= (buff[0]) && (buff[0]) <= 'F')) || !(('0' <= (buff[1]) && (buff[1]) <= '9') || ('A' <= (buff[1]) && (buff[1]) <= 'F')) || !(('0' <= (buff[2]) && (buff[2]) <= '9') || ('A' <= (buff[2]) && (buff[2]) <= 'F')))
            {
                return 0;
            }
            if (sscanf((string)buff, "%3X", len) == 1)
            {
                return (int)len;
            }
            return 0;
        }
        /* test measurement data -----------------------------------------------------*/
        internal static int is_meas(sbyte sig)
        {
            return sig == 'c' || sig == 'C' || sig == '1' || sig == '2' || sig == '3' || sig == '5' || sig == 'l';
        }
        /* convert signal to frequency and obs type ----------------------------------*/
        internal static int tofreq(sbyte sig, int sys, ref int type)
        {
            byte[,] types = { { DefineConstants.CODE_L1C, DefineConstants.CODE_L1W, DefineConstants.CODE_L2W, DefineConstants.CODE_L2X, DefineConstants.CODE_L5X, DefineConstants.CODE_L1X }, { DefineConstants.CODE_L1C, DefineConstants.CODE_L1Z, DefineConstants.CODE_L6X, DefineConstants.CODE_L2X, DefineConstants.CODE_L5X, DefineConstants.CODE_L1X }, { DefineConstants.CODE_L1C, 0, 0, 0, DefineConstants.CODE_L5X, 0 }, { DefineConstants.CODE_L1X, DefineConstants.CODE_L8X, DefineConstants.CODE_L7X, DefineConstants.CODE_L6X, DefineConstants.CODE_L5X, 0 }, { DefineConstants.CODE_L1C, DefineConstants.CODE_L1P, DefineConstants.CODE_L2P, DefineConstants.CODE_L2C, DefineConstants.CODE_L3X, 0 }, { DefineConstants.CODE_L1I, 0, 0, 0, DefineConstants.CODE_L7I, 0 } }; // ref [5] table 3-7
            int[,] freqs = { { 1, 1, 2, 2, 3, 1 }, { 1, 1, 4, 2, 3, 1 }, { 1, 0, 0, 0, 3, 0 }, { 1, 6, 5, 4, 3, 0 }, { 1, 1, 2, 2, 3, 0 }, { 1, 0, 0, 0, 2, 0 } };
            int i;
            int j;

            switch (sig)
            {
                case 'c':
                case 'C':
                    i = 0;
                    break;
                case '1':
                    i = 1;
                    break;
                case '2':
                    i = 2;
                    break;
                case '3':
                    i = 3;
                    break;
                case '5':
                    i = 4;
                    break;
                case 'l':
                    i = 5;
                    break;
                default:
                    return -1;
            }
            switch (sys)
            {
                case DefineConstants.SYS_GPS:
                    j = 0;
                    break;
                case DefineConstants.SYS_QZS:
                    j = 1;
                    break;
                case DefineConstants.SYS_SBS:
                    j = 2;
                    break;
                case DefineConstants.SYS_GAL:
                    j = 3;
                    break;
                case DefineConstants.SYS_GLO:
                    j = 4;
                    break;
                case DefineConstants.SYS_CMP:
                    j = 5;
                    break;
                default:
                    return -1;
            }
            type = types[j, i];

            /* 0:L1,1:L2,2:L5,3:L6,4:L7,5:L8,-1:error */
            return freqs[j, i] <= DefineConstants.NFREQ ? freqs[j, i] - 1 : -1;
        }
        /* check code priority and return obs position -------------------------------*/
        internal static int checkpri(string opt, int sys, int code, int freq)
        {
            int nex = DefineConstants.NEXOBS; // number of extended obs data

            if (sys == DefineConstants.SYS_GPS)
            {
                if (StringFunctions.StrStr(opt, "-GL1W") && freq == 0)
                {
                    return code == DefineConstants.CODE_L1W ? 0 : -1;
                }
                if (StringFunctions.StrStr(opt, "-GL1X") && freq == 0)
                {
                    return code == DefineConstants.CODE_L1X ? 0 : -1;
                }
                if (StringFunctions.StrStr(opt, "-GL2X") && freq == 1)
                {
                    return code == DefineConstants.CODE_L2X ? 1 : -1;
                }
                if (code == DefineConstants.CODE_L1W)
                {
                    return nex < 1 ? -1 : DefineConstants.NFREQ;
                }
                if (code == DefineConstants.CODE_L2X)
                {
                    return nex < 2 ? -1 : DefineConstants.NFREQ + 1;
                }
                if (code == DefineConstants.CODE_L1X)
                {
                    return nex < 3 ? -1 : DefineConstants.NFREQ + 2;
                }
            }
            else if (sys == DefineConstants.SYS_GLO)
            {
                if (StringFunctions.StrStr(opt, "-RL1C") && freq == 0)
                {
                    return code == DefineConstants.CODE_L1C ? 0 : -1;
                }
                if (StringFunctions.StrStr(opt, "-RL2C") && freq == 1)
                {
                    return code == DefineConstants.CODE_L2C ? 1 : -1;
                }
                if (code == DefineConstants.CODE_L1C)
                {
                    return nex < 1 ? -1 : DefineConstants.NFREQ;
                }
                if (code == DefineConstants.CODE_L2C)
                {
                    return nex < 2 ? -1 : DefineConstants.NFREQ + 1;
                }
            }
            else if (sys == DefineConstants.SYS_QZS)
            {
                if (StringFunctions.StrStr(opt, "-JL1Z") && freq == 0)
                {
                    return code == DefineConstants.CODE_L1Z ? 0 : -1;
                }
                if (StringFunctions.StrStr(opt, "-JL1X") && freq == 0)
                {
                    return code == DefineConstants.CODE_L1X ? 0 : -1;
                }
                if (code == DefineConstants.CODE_L1Z)
                {
                    return nex < 1 ? -1 : DefineConstants.NFREQ;
                }
                if (code == DefineConstants.CODE_L1X)
                {
                    return nex < 2 ? -1 : DefineConstants.NFREQ + 1;
                }
            }
            return freq < DefineConstants.NFREQ ? freq : -1;
        }
        /* glonass carrier frequency -------------------------------------------------*/
        internal static double freq_glo(int freq, int freqn)
        {
            switch (freq)
            {
                case 0:
                    return DefineConstants.FREQ1_GLO + DefineConstants.DFRQ1_GLO * freqn;
                case 1:
                    return DefineConstants.FREQ2_GLO + DefineConstants.DFRQ2_GLO * freqn;
            }
            return 0.0;
        }
        /* checksum ------------------------------------------------------------------*/
        internal static int checksum(byte[] buff, int len)
        {
            byte cs = 0;
            int i;
            for (i = 0; i < len - 1; i++)
            {
                cs = (((cs) << 2) | ((cs) >> 6)) ^ buff[i];
            }
            cs = (((cs) << 2) | ((cs) >> 6));
            return cs == buff[len - 1];
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
        /* set time tag --------------------------------------------------------------*/
        internal static int settag(obsd_t data, gtime_t time)
        {
            string s1 = new string(new char[64]);
            string s2 = new string(new char[64]);

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: if (data->time.time!=0&&fabs(timediff(data->time,time))>5E-4)
            if (data.time.time != 0 && Math.Abs(GlobalMembersRtkcmn.timediff(new gtime_t(data.time), new gtime_t(time))) > 5E-4)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: time2str(data->time,s1,4);
                GlobalMembersRtkcmn.time2str(new gtime_t(data.time), ref s1, 4);
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: time2str(time,s2,4);
                GlobalMembersRtkcmn.time2str(new gtime_t(time), ref s2, 4);
                GlobalMembersRtkcmn.trace(2, "time inconsistent: time=%s %s sat=%2d\n", s1, s2, data.sat);
                return 0;
            }
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: data->time=time;
            data.time.CopyFrom(time);
            return 1;
        }
        /* flush observation data buffer ---------------------------------------------*/
        internal static int flushobuf(raw_t raw)
        {
            gtime_t time0 = new gtime_t();
            int i;
            int j;
            int n = 0;

            GlobalMembersRtkcmn.trace(3, "flushobuf: n=%d\n", raw.obuf.n);

            /* copy observation data buffer */
            for (i = 0; i < raw.obuf.n && i < DefineConstants.MAXOBS; i++)
            {
                if (GlobalMembersRtkcmn.satsys(raw.obuf.data[i].sat, null) == 0)
                    continue;
                if (raw.obuf.data[i].time.time == 0)
                    continue;
                raw.obs.data[n++] = raw.obuf.data[i];
            }
            raw.obs.n = n;

            /* clear observation data buffer */
            for (i = 0; i < DefineConstants.MAXOBS; i++)
            {
                raw.obuf.data[i].time = time0;
                for (j = 0; j < DefineConstants.NFREQ + DefineConstants.NEXOBS; j++)
                {
                    raw.obuf.data[i].L[j] = raw.obuf.data[i].P[j] = 0.0;
                    raw.obuf.data[i].D[j] = 0.0;
                    raw.obuf.data[i].SNR[j] = raw.obuf.data[i].LLI[j] = 0;
                    raw.obuf.data[i].code[j] = DefineConstants.CODE_NONE;
                }
            }
            for (i = 0; i < DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1; i++)
            {
                raw.prCA[i] = raw.dpCA[i] = 0.0;
            }
            return n > 0 ? 1 : 0;
        }
        /* decode [~~] receiver time -------------------------------------------------*/
        internal static int decode_RT(raw_t raw)
        {
            gtime_t time = new gtime_t();
            string msg;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: byte *p=raw->buff+5;
            byte p = raw.buff + 5;

            if (GlobalMembersJavad.checksum(raw.buff, raw.len) == 0)
            {
                GlobalMembersRtkcmn.trace(2, "javad RT error: len=%d\n", raw.len);
                return -1;
            }
            if (raw.len < 10)
            {
                GlobalMembersRtkcmn.trace(2, "javad RT length error: len=%d\n", raw.len);
                return -1;
            }
            raw.tod = GlobalMembersBinex.U4(p);

            if (raw.time.time == 0)
            {
                return 0;
            }

            /* update receiver time */
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: time=raw->time;
            time.CopyFrom(raw.time);
            if (raw.tbase >= 1) // gpst->utc
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                //ORIGINAL LINE: time=gpst2utc(time);
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                time.CopyFrom(GlobalMembersRtkcmn.gpst2utc(new gtime_t(time)));
            }
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: time=adjday(time,raw->tod *0.001);
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            time.CopyFrom(GlobalMembersBinex.adjday(new gtime_t(time), raw.tod * 0.001));
            if (raw.tbase >= 1) // utc->gpst
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                //ORIGINAL LINE: time=utc2gpst(time);
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                time.CopyFrom(GlobalMembersRtkcmn.utc2gpst(new gtime_t(time)));
            }
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: raw->time=time;
            raw.time.CopyFrom(time);

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: trace(3,"decode_RT: time=%s\n",time_str(time,3));
            GlobalMembersRtkcmn.trace(3, "decode_RT: time=%s\n", GlobalMembersRtkcmn.time_str(new gtime_t(time), 3));

            if (raw.outtype != 0)
            {
                msg = raw.msgtype.Substring(raw.msgtype.Length);
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: sprintf(msg," %s",time_str(time,3));
                msg = string.Format(" {0}", GlobalMembersRtkcmn.time_str(new gtime_t(time), 3));
            }
            /* flush observation data buffer */
            return GlobalMembersJavad.flushobuf(raw);
        }
        /* decode [::] epoch time ----------------------------------------------------*/
        internal static int decode_ET(raw_t raw)
        {
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: byte *p=raw->buff+5;
            byte p = raw.buff + 5;

            if (GlobalMembersJavad.checksum(raw.buff, raw.len) == 0)
            {
                GlobalMembersRtkcmn.trace(2, "javad ET checksum error: len=%d\n", raw.len);
                return -1;
            }
            if (raw.len < 10)
            {
                GlobalMembersRtkcmn.trace(2, "javad ET length error: len=%d\n", raw.len);
                return -1;
            }
            if (raw.tod != (int)GlobalMembersBinex.U4(p))
            {
                GlobalMembersRtkcmn.trace(2, "javad ET inconsistent tod: tod=%d %d\n", raw.tod, GlobalMembersBinex.U4(p));
                return -1;
            }
            raw.tod = -1; // end of epoch

            /* flush observation data buffer */
            return GlobalMembersJavad.flushobuf(raw);
        }
        /* decode [RD] receiver date -------------------------------------------------*/
        internal static int decode_RD(raw_t raw)
        {
            double[] ep = { 0, null, null, null, null, null };
            string msg;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: byte *p=raw->buff+5;
            byte p = raw.buff + 5;

            if (GlobalMembersJavad.checksum(raw.buff, raw.len) == 0)
            {
                GlobalMembersRtkcmn.trace(2, "javad RD checksum error: len=%d\n", raw.len);
                return -1;
            }
            if (raw.len < 11)
            {
                GlobalMembersRtkcmn.trace(2, "javad RD length error: len=%d\n", raw.len);
                return -1;
            }
            ep[0] = GlobalMembersBinex.U2(p);
            p += 2;
            ep[1] = ((byte)(p));
            p += 1;
            ep[2] = ((byte)(p));
            p += 1;
            raw.tbase = ((byte)(p));

            if (raw.outtype != 0)
            {
                msg = raw.msgtype.Substring(raw.msgtype.Length);
                //C++ TO C# CONVERTER TODO TASK: Zero padding is not converted when combined with a minimum width specifier:
                //ORIGINAL LINE: sprintf(msg," %04.0f/%02.0f/%02.0f base=%d",ep[0],ep[1],ep[2],raw->tbase);
                msg = string.Format(" {0,4:f0}/{1,2:f0}/{2,2:f0} base={3:D}", ep[0], ep[1], ep[2], raw.tbase);
            }
            if (raw.tod < 0)
            {
                GlobalMembersRtkcmn.trace(2, "javad RD lack of preceding RT\n");
                return 0;
            }
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: raw->time=timeadd(epoch2time(ep),raw->tod *0.001);
            raw.time.CopyFrom(GlobalMembersRtkcmn.timeadd(GlobalMembersRtkcmn.epoch2time(ep), raw.tod * 0.001));
            if (raw.tbase >= 1) // utc->gpst
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                //ORIGINAL LINE: raw->time=utc2gpst(raw->time);
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                raw.time.CopyFrom(GlobalMembersRtkcmn.utc2gpst(new gtime_t(raw.time)));
            }

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: trace(3,"decode_RD: time=%s\n",time_str(raw->time,3));
            GlobalMembersRtkcmn.trace(3, "decode_RD: time=%s\n", GlobalMembersRtkcmn.time_str(new gtime_t(raw.time), 3));

            return 0;
        }
        /* decode [SI] satellite indices ---------------------------------------------*/
        internal static int decode_SI(raw_t raw)
        {
            int i;
            int usi;
            int sat;
            string msg;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: byte *p=raw->buff+5;
            byte p = raw.buff + 5;

            if (GlobalMembersJavad.checksum(raw.buff, raw.len) == 0)
            {
                GlobalMembersRtkcmn.trace(2, "javad SI checksum error: len=%d\n", raw.len);
                return -1;
            }
            raw.obuf.n = raw.len - 6;

            for (i = 0; i < raw.obuf.n && i < DefineConstants.MAXOBS; i++)
            {
                usi = ((byte)(p));
                p += 1;

                if (usi <= 0) // ref [5] table 3-6
                {
                    sat = 0;
                }
                else if (usi <= 37) //   1- 37: GPS
                {
                    sat = GlobalMembersRtkcmn.satno(DefineConstants.SYS_GPS, usi);
                }
                else if (usi <= 70) //  38- 70: GLONASS
                {
                    sat = 255;
                }
                else if (usi <= 119) //  71-119: GALILEO
                {
                    sat = GlobalMembersRtkcmn.satno(DefineConstants.SYS_GAL, usi - 70);
                }
                else if (usi <= 142) // 120-142: SBAS
                {
                    sat = GlobalMembersRtkcmn.satno(DefineConstants.SYS_SBS, usi);
                }
                else if (usi <= 192)
                {
                    sat = 0;
                }
                else if (usi <= 197) // 193-197: QZSS
                {
                    sat = GlobalMembersRtkcmn.satno(DefineConstants.SYS_QZS, usi);
                }
                else if (usi <= 210)
                {
                    sat = 0;
                }
                else if (usi <= 240) // 211-240: BeiDou
                {
                    sat = GlobalMembersRtkcmn.satno(DefineConstants.SYS_CMP, usi - 210);
                }
                else
                {
                    sat = 0;
                }

                raw.obuf.data[i].time = raw.time;
                raw.obuf.data[i].sat = sat;

                /* glonass fcn (frequency channel number) */
                if (sat == 255)
                {
                    raw.freqn[i] = usi - 45;
                }
            }
            GlobalMembersRtkcmn.trace(4, "decode_SI: nsat=raw->obuf.n\n");

            if (raw.outtype != 0)
            {
                msg = raw.msgtype.Substring(raw.msgtype.Length);
                msg = string.Format(" nsat={0,2:D}", raw.obuf.n);
            }
            return 0;
        }
        /* decode [NN] glonass satellite system numbers ------------------------------*/
        internal static int decode_NN(raw_t raw)
        {
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: byte *p=raw->buff+5;
            byte p = raw.buff + 5;
            string msg;
            int i;
            int n;
            int ns;
            int slot;
            int sat;
            int[] index = new int[DefineConstants.MAXOBS];

            if (GlobalMembersJavad.checksum(raw.buff, raw.len) == 0)
            {
                GlobalMembersRtkcmn.trace(2, "javad NN checksum error: len=%d\n", raw.len);
                return -1;
            }
            for (i = n = 0; i < raw.obuf.n && i < DefineConstants.MAXOBS; i++)
            {
                if (raw.obuf.data[i].sat == 255)
                {
                    index[n++] = i;
                }
            }
            ns = raw.len - 6;

            for (i = 0; i < ns && i < n; i++)
            {
                slot = ((byte)(p));
                p += 1;
                sat = GlobalMembersRtkcmn.satno(DefineConstants.SYS_GLO, slot);
                raw.obuf.data[index[i]].sat = sat;
            }
            if (raw.outtype != 0)
            {
                msg = raw.msgtype.Substring(raw.msgtype.Length);
                msg = string.Format(" nsat={0,2:D}", ns);
            }
            return 0;
        }
        /* decode [GA] gps almanac ---------------------------------------------------*/
        internal static int decode_GA(raw_t raw)
        {
            GlobalMembersRtkcmn.trace(2, "javad GA not supported\n");

            return 0;
        }
        /* decode [NA] glonass almanac -----------------------------------------------*/
        internal static int decode_NA(raw_t raw)
        {
            GlobalMembersRtkcmn.trace(2, "javad NA not supported\n");

            return 0;
        }
        /* decode [EA] galileo almanac -----------------------------------------------*/
        internal static int decode_EA(raw_t raw)
        {
            GlobalMembersRtkcmn.trace(2, "javad EA not supported\n");

            return 0;
        }
        /* decode [WA] waas almanac --------------------------------------------------*/
        internal static int decode_WA(raw_t raw)
        {
            GlobalMembersRtkcmn.trace(2, "javad WA not supported\n");

            return 0;
        }
        /* decode [QA] qzss almanac --------------------------------------------------*/
        internal static int decode_QA(raw_t raw)
        {
            GlobalMembersRtkcmn.trace(2, "javad QA not supported\n");

            return 0;
        }
        /* decode gps/galileo/qzss ephemeris -----------------------------------------*/
        internal static int decode_eph(raw_t raw, int sys)
        {
            eph_t eph = new eph_t();
            double toc;
            double sqrtA;
            double tt;
            string msg;
            int prn;
            int tow;
            int flag;
            int week;
            //C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on this variable, so pointers on this variable are left unchanged:
            byte* p = raw.buff + 5;

            GlobalMembersRtkcmn.trace(3, "decode_eph: sys=%2d prn=%3d\n", sys, ((byte)(p)));

            prn = ((byte)(p));
            p += 1;
            tow = GlobalMembersBinex.U4(p);
            p += 4;
            flag = ((byte)(p));
            p += 1;
            eph.iodc = GlobalMembersCrescent.I2(ref p);
            p += 2;
            toc = GlobalMembersBinex.I4(ref p);
            p += 4;
            eph.sva = ((string)(p));
            p += 1;
            eph.svh = ((byte)(p));
            p += 1;
            week = GlobalMembersCrescent.I2(ref p);
            p += 2;
            eph.tgd[0] = GlobalMembersBinex.R4(p);
            p += 4;
            eph.f2 = GlobalMembersBinex.R4(p);
            p += 4;
            eph.f1 = GlobalMembersBinex.R4(p);
            p += 4;
            eph.f0 = GlobalMembersBinex.R4(p);
            p += 4;
            eph.toes = GlobalMembersBinex.I4(ref p);
            p += 4;
            eph.iode = GlobalMembersCrescent.I2(ref p);
            p += 2;
            sqrtA = GlobalMembersBinex.R8(p);
            p += 8;
            eph.e = GlobalMembersBinex.R8(p);
            p += 8;
            eph.M0 = GlobalMembersBinex.R8(p) * DefineConstants.SC2RAD;
            p += 8;
            eph.OMG0 = GlobalMembersBinex.R8(p) * DefineConstants.SC2RAD;
            p += 8;
            eph.i0 = GlobalMembersBinex.R8(p) * DefineConstants.SC2RAD;
            p += 8;
            eph.omg = GlobalMembersBinex.R8(p) * DefineConstants.SC2RAD;
            p += 8;
            eph.deln = GlobalMembersBinex.R4(p) * DefineConstants.SC2RAD;
            p += 4;
            eph.OMGd = GlobalMembersBinex.R4(p) * DefineConstants.SC2RAD;
            p += 4;
            eph.idot = GlobalMembersBinex.R4(p) * DefineConstants.SC2RAD;
            p += 4;
            eph.crc = GlobalMembersBinex.R4(p);
            p += 4;
            eph.crs = GlobalMembersBinex.R4(p);
            p += 4;
            eph.cuc = GlobalMembersBinex.R4(p);
            p += 4;
            eph.cus = GlobalMembersBinex.R4(p);
            p += 4;
            eph.cic = GlobalMembersBinex.R4(p);
            p += 4;
            eph.cis = GlobalMembersBinex.R4(p);
            p += 4;
            eph.A = sqrtA * sqrtA;

            if (raw.outtype != 0)
            {
                msg = raw.msgtype.Substring(raw.msgtype.Length);
                msg = string.Format(" prn={0,3:D} iode={1,3:D} iodc={2,3:D} toes={3,6:f0}", prn, eph.iode, eph.iodc, eph.toes);
            }
            if (sys == DefineConstants.SYS_GPS || sys == DefineConstants.SYS_QZS)
            {
                if ((eph.sat = GlobalMembersRtkcmn.satno(sys, prn)) == 0)
                {
                    GlobalMembersRtkcmn.trace(2, "javad ephemeris satellite error: sys=%d prn=%d\n", sys, prn);
                    return -1;
                }
                eph.flag = (flag >> 1) & 1;
                eph.code = (flag >> 2) & 3;
                eph.fit = flag & 1;
                eph.week = GlobalMembersRtkcmn.adjgpsweek(week);
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                //ORIGINAL LINE: eph.toe=gpst2time(eph.week,eph.toes);
                eph.toe.CopyFrom(GlobalMembersRtkcmn.gpst2time(eph.week, eph.toes));

                /* for week-handover problem */
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: tt=timediff(eph.toe,raw->time);
                tt = GlobalMembersRtkcmn.timediff(new gtime_t(eph.toe), new gtime_t(raw.time));
                if (tt < -302400.0)
                {
                    eph.week++;
                }
                else if (tt > 302400.0)
                {
                    eph.week--;
                }
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                //ORIGINAL LINE: eph.toe=gpst2time(eph.week,eph.toes);
                eph.toe.CopyFrom(GlobalMembersRtkcmn.gpst2time(eph.week, eph.toes));

                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                //ORIGINAL LINE: eph.toc=gpst2time(eph.week,toc);
                eph.toc.CopyFrom(GlobalMembersRtkcmn.gpst2time(eph.week, toc));
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                //ORIGINAL LINE: eph.ttr=adjweek(eph.toe,tow);
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                eph.ttr.CopyFrom(GlobalMembersBinex.adjweek(new gtime_t(eph.toe), tow));
            }
            else if (sys == DefineConstants.SYS_GAL)
            {
                if ((eph.sat = GlobalMembersRtkcmn.satno(sys, prn)) == 0)
                {
                    GlobalMembersRtkcmn.trace(2, "javad ephemeris satellite error: sys=%d prn=%d\n", sys, prn);
                    return -1;
                }
                eph.tgd[1] = GlobalMembersBinex.R4(p); // BGD: E1-E5A (s)
                p += 4;
                eph.tgd[2] = GlobalMembersBinex.R4(p); // BGD: E1-E5B (s)
                p += 4 + 13;
                eph.code = ((byte)(p));
                /*          3:GIOVE E1B,4:GIOVE E5A */

                /* gst week -> gps week */
                eph.week = week + 1024;
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                //ORIGINAL LINE: eph.toe=gpst2time(eph.week,eph.toes);
                eph.toe.CopyFrom(GlobalMembersRtkcmn.gpst2time(eph.week, eph.toes));

                /* for week-handover problem */
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: tt=timediff(eph.toe,raw->time);
                tt = GlobalMembersRtkcmn.timediff(new gtime_t(eph.toe), new gtime_t(raw.time));
                if (tt < -302400.0)
                {
                    eph.week++;
                }
                else if (tt > 302400.0)
                {
                    eph.week--;
                }
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                //ORIGINAL LINE: eph.toe=gpst2time(eph.week,eph.toes);
                eph.toe.CopyFrom(GlobalMembersRtkcmn.gpst2time(eph.week, eph.toes));

                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                //ORIGINAL LINE: eph.toc=gpst2time(eph.week,toc);
                eph.toc.CopyFrom(GlobalMembersRtkcmn.gpst2time(eph.week, toc));
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                //ORIGINAL LINE: eph.ttr=adjweek(eph.toe,tow);
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                eph.ttr.CopyFrom(GlobalMembersBinex.adjweek(new gtime_t(eph.toe), tow));
            }
            else if (sys == DefineConstants.SYS_CMP)
            {
                if ((eph.sat = GlobalMembersRtkcmn.satno(sys, prn)) == 0)
                {
                    GlobalMembersRtkcmn.trace(2, "javad ephemeris satellite error: sys=%d prn=%d\n", sys, prn);
                    return -1;
                }
                eph.tgd[1] = GlobalMembersBinex.R4(p); // TGD2 (s)
                p += 4;
                eph.code = ((byte)(p));

                eph.week = week;
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                //ORIGINAL LINE: eph.toe=bdt2time(week,eph.toes);
                eph.toe.CopyFrom(GlobalMembersRtkcmn.bdt2time(week, eph.toes)); // bdt -> gpst
                                                                                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                                                                                //ORIGINAL LINE: eph.toc=bdt2time(week,toc);
                eph.toc.CopyFrom(GlobalMembersRtkcmn.bdt2time(week, toc)); // bdt -> gpst
                                                                           //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                                                                           //ORIGINAL LINE: eph.ttr=adjweek(eph.toe,tow);
                                                                           //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                eph.ttr.CopyFrom(GlobalMembersBinex.adjweek(new gtime_t(eph.toe), tow));
            }
            else
            {
                return 0;
            }

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
        /* decode [GE] gps ephemeris -------------------------------------------------*/
        internal static int decode_GE(raw_t raw)
        {
            if (GlobalMembersJavad.checksum(raw.buff, raw.len) == 0)
            {
                GlobalMembersRtkcmn.trace(2, "javad GE checksum error: len=%d\n", raw.len);
                return -1;
            }
            if (raw.len < 128)
            {
                GlobalMembersRtkcmn.trace(2, "javad GE length error: len=%d\n", raw.len);
                return -1;
            }
            return GlobalMembersJavad.decode_eph(raw, DefineConstants.SYS_GPS);
        }
        /* decode [NE] glonass ephemeris ---------------------------------------------*/
        internal static int decode_NE(raw_t raw)
        {
            geph_t geph = new geph_t();
            double tt;
            string msg;
            int prn;
            int tk;
            int tb;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: byte *p=raw->buff+5;
            byte p = raw.buff + 5;

            if (GlobalMembersJavad.checksum(raw.buff, raw.len) == 0)
            {
                GlobalMembersRtkcmn.trace(2, "javad NE checksum error: len=%d\n", raw.len);
                return -1;
            }
            if (raw.len >= 85) // firmware v 2.6.0 [2]
            {
                prn = ((byte)(p));
                p += 1;
                geph.frq = ((string)(p));
                p += 1 + 2;
                tk = GlobalMembersBinex.I4(ref p);
                p += 4;
                tb = GlobalMembersBinex.I4(ref p);
                p += 4;
                geph.svh = ((byte)(p)) & 0x7;
                p += 1;
                geph.age = ((byte)(p));
                p += 1 + 1;
                geph.pos[0] = GlobalMembersBinex.R8(p) * 1E3;
                p += 8;
                geph.pos[1] = GlobalMembersBinex.R8(p) * 1E3;
                p += 8;
                geph.pos[2] = GlobalMembersBinex.R8(p) * 1E3;
                p += 8;
                geph.vel[0] = GlobalMembersBinex.R4(p) * 1E3;
                p += 4;
                geph.vel[1] = GlobalMembersBinex.R4(p) * 1E3;
                p += 4;
                geph.vel[2] = GlobalMembersBinex.R4(p) * 1E3;
                p += 4;
                geph.acc[0] = GlobalMembersBinex.R4(p) * 1E3;
                p += 4;
                geph.acc[1] = GlobalMembersBinex.R4(p) * 1E3;
                p += 4;
                geph.acc[2] = GlobalMembersBinex.R4(p) * 1E3;
                p += 4 + 8;
                geph.taun = GlobalMembersBinex.R4(p);
                p += 4;
                geph.gamn = GlobalMembersBinex.R4(p);
                p += 4;
            }
            else
            {
                GlobalMembersRtkcmn.trace(2, "javad NE length error: len=%d\n", raw.len);
                return -1;
            }
            if (raw.len >= 93) // firmware v 3.2.0 [1]
            {
                geph.dtaun = GlobalMembersBinex.R4(p);
                p += 4;
                geph.sva = ((byte)(p));
            }
            if (raw.outtype != 0)
            {
                msg = raw.msgtype.Substring(raw.msgtype.Length);
                msg = string.Format(" prn={0,2:D} frq={1,2:D} tk={2,6:D} tb={3,4:D}", prn, geph.frq, tk, tb);
            }
            if ((geph.sat = GlobalMembersRtkcmn.satno(DefineConstants.SYS_GLO, prn)) == 0)
            {
                GlobalMembersRtkcmn.trace(2, "javad NE satellite error: prn=%d\n", prn);
                return 0;
            }
            if (raw.time.time == 0)
            {
                return 0;
            }
            geph.iode = (tb / 900) & 0x7F;
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: geph.toe=utc2gpst(adjday(raw->time,tb-10800.0));
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            geph.toe.CopyFrom(GlobalMembersRtkcmn.utc2gpst(GlobalMembersBinex.adjday(new gtime_t(raw.time), tb - 10800.0)));
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: geph.tof=utc2gpst(adjday(raw->time,tk-10800.0));
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            geph.tof.CopyFrom(GlobalMembersRtkcmn.utc2gpst(GlobalMembersBinex.adjday(new gtime_t(raw.time), tk - 10800.0)));

            /* check illegal ephemeris by toe */
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: tt=timediff(raw->time,geph.toe);
            tt = GlobalMembersRtkcmn.timediff(new gtime_t(raw.time), new gtime_t(geph.toe));
            if (Math.Abs(tt) > 3600.0)
            {
                GlobalMembersRtkcmn.trace(3, "javad NE illegal toe: prn=%2d tt=%6.0f\n", prn, tt);
                return 0;
            }
            /* check illegal ephemeris by frequency number consistency */
            if (raw.nav.geph[prn - DefineConstants.MINPRNGLO].toe.time && geph.frq != raw.nav.geph[prn - DefineConstants.MINPRNGLO].frq)
            {
                GlobalMembersRtkcmn.trace(2, "javad NE illegal freq change: prn=%2d frq=%2d->%2d\n", prn, raw.nav.geph[prn - DefineConstants.MINPRNGLO].frq, geph.frq);
                return -1;
            }
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
        /* decode [EN] galileo ephemeris ---------------------------------------------*/
        internal static int decode_EN(raw_t raw)
        {
            if (GlobalMembersJavad.checksum(raw.buff, raw.len) == 0)
            {
                GlobalMembersRtkcmn.trace(2, "javad EN checksum error: len=%d\n", raw.len);
                return -1;
            }
            if (raw.len < 150)
            {
                GlobalMembersRtkcmn.trace(2, "javad EN length error: len=%d\n", raw.len);
                return -1;
            }
            return GlobalMembersJavad.decode_eph(raw, DefineConstants.SYS_GAL);
        }
        /* decode [WE] sbas ephemeris ------------------------------------------------*/
        internal static int decode_WE(raw_t raw)
        {
            seph_t seph = new seph_t();
            uint tod;
            uint tow;
            string msg;
            int i;
            int prn;
            int week;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: byte *p=raw->buff+5;
            byte p = raw.buff + 5;

            if (GlobalMembersJavad.checksum(raw.buff, raw.len) == 0)
            {
                GlobalMembersRtkcmn.trace(2, "javad WE checksum error: len=%d\n", raw.len);
                return -1;
            }
            if (raw.len < 44)
            {
                GlobalMembersRtkcmn.trace(2, "javad WE length error: len=%d\n", raw.len);
                return -1;
            }
            prn = ((byte)(p));
            p += 1 + 1 + 1;
            seph.sva = ((byte)(p));
            p += 1;
            tod = GlobalMembersBinex.U4(p);
            p += 4;
            for (i = 0; i < 3; i++)
            {
                seph.pos[i] = GlobalMembersBinex.R8(p);
                p += 8;
            }
            for (i = 0; i < 3; i++)
            {
                seph.vel[i] = GlobalMembersBinex.R4(p);
                p += 4;
            }
            for (i = 0; i < 3; i++)
            {
                seph.acc[i] = GlobalMembersBinex.R4(p);
                p += 4;
            }
            seph.af0 = GlobalMembersBinex.R4(p);
            p += 4;
            seph.af1 = GlobalMembersBinex.R4(p);
            p += 4;
            tow = GlobalMembersBinex.U4(p);
            p += 4;
            week = GlobalMembersBinex.U2(p);

            if (raw.outtype != 0)
            {
                msg = raw.msgtype.Substring(raw.msgtype.Length);
                msg = string.Format(" prn={0,3:D} tod={1,6:D}", prn, tod);
            }
            if ((seph.sat = GlobalMembersRtkcmn.satno(DefineConstants.SYS_SBS, prn)) == 0)
            {
                GlobalMembersRtkcmn.trace(2, "javad WE satellite error: prn=%d\n", prn);
                return -1;
            }
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: seph.tof=gpst2time(adjgpsweek(week),tow);
            seph.tof.CopyFrom(GlobalMembersRtkcmn.gpst2time(GlobalMembersRtkcmn.adjgpsweek(week), tow));
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: seph.t0=adjday(seph.tof,tod);
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            seph.t0.CopyFrom(GlobalMembersBinex.adjday(new gtime_t(seph.tof), tod));

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
        /* decode [QE] qzss ephemeris ------------------------------------------------*/
        internal static int decode_QE(raw_t raw)
        {
            if (GlobalMembersJavad.checksum(raw.buff, raw.len) == 0)
            {
                GlobalMembersRtkcmn.trace(2, "javad QE checksum error: len=%d\n", raw.len);
                return -1;
            }
            if (raw.len < 128)
            {
                GlobalMembersRtkcmn.trace(2, "javad QE length error: len=%d\n", raw.len);
                return -1;
            }
            return GlobalMembersJavad.decode_eph(raw, DefineConstants.SYS_QZS);
        }
        /* decode [CN] beidou ephemeris ----------------------------------------------*/
        internal static int decode_CN(raw_t raw)
        {
            if (GlobalMembersJavad.checksum(raw.buff, raw.len) == 0)
            {
                GlobalMembersRtkcmn.trace(2, "javad CN checksum error: len=%d\n", raw.len);
                return -1;
            }
            if (raw.len < 133)
            {
                GlobalMembersRtkcmn.trace(2, "javad QE length error: len=%d\n", raw.len);
                return -1;
            }
            return GlobalMembersJavad.decode_eph(raw, DefineConstants.SYS_CMP);
        }
        /* decode [UO] gps utc time parameters ---------------------------------------*/
        internal static int decode_UO(raw_t raw)
        {
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: byte *p=raw->buff+5;
            byte p = raw.buff + 5;

            if (GlobalMembersJavad.checksum(raw.buff, raw.len) == 0)
            {
                GlobalMembersRtkcmn.trace(2, "javad UO checksum error: len=%d\n", raw.len);
                return -1;
            }
            if (raw.len < 29)
            {
                GlobalMembersRtkcmn.trace(2, "javad UO length error: len=%d\n", raw.len);
                return -1;
            }
            raw.nav.utc_gps[0] = GlobalMembersBinex.R8(p);
            p += 8;
            raw.nav.utc_gps[1] = GlobalMembersBinex.R4(p);
            p += 4;
            raw.nav.utc_gps[2] = GlobalMembersBinex.U4(p);
            p += 4;
            raw.nav.utc_gps[3] = GlobalMembersRtkcmn.adjgpsweek((int)GlobalMembersBinex.U2(p));
            p += 2;
            raw.nav.leaps = ((string)(p));
            return 9;
        }
        /* decode [NU] glonass utc and gps time parameters ---------------------------*/
        internal static int decode_NU(raw_t raw)
        {
            GlobalMembersRtkcmn.trace(2, "javad NU not supported\n");

            return 0;
        }
        /* decode [EU] galileo utc and gps time parameters ---------------------------*/
        internal static int decode_EU(raw_t raw)
        {
            GlobalMembersRtkcmn.trace(2, "javad EU not supported\n");

            return 0;
        }
        /* decode [WU] waas utc time parameters --------------------------------------*/
        internal static int decode_WU(raw_t raw)
        {
            GlobalMembersRtkcmn.trace(2, "javad WU not supported\n");

            return 0;
        }
        /* decode [QU] qzss utc and gps time parameters ------------------------------*/
        internal static int decode_QU(raw_t raw)
        {
            GlobalMembersRtkcmn.trace(2, "javad QU not supported\n");

            return 0;
        }
        /* decode [IO] ionospheric parameters ----------------------------------------*/
        internal static int decode_IO(raw_t raw)
        {
            int i;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: byte *p=raw->buff+5;
            byte p = raw.buff + 5;

            if (GlobalMembersJavad.checksum(raw.buff, raw.len) == 0)
            {
                GlobalMembersRtkcmn.trace(2, "javad IO checksum error: len=%d\n", raw.len);
                return -1;
            }
            if (raw.len < 44)
            {
                GlobalMembersRtkcmn.trace(2, "javad IO length error: len=%d\n", raw.len);
                return -1;
            }
            p += 4 + 2;
            for (i = 0; i < 8; i++)
            {
                raw.nav.ion_gps[i] = GlobalMembersBinex.R4(p);
                p += 4;
            }
            return 9;
        }
        /* decode L1 NAV data --------------------------------------------------------*/
        internal static int decode_L1nav(ref byte buff, int len, int sat, raw_t raw)
        {
            eph_t eph = new eph_t();
            double[] ion = { 0, null, null, null, null, null, null, null };
            double[] utc = { 0, null, null, null };
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: byte *subfrm,*p;
            byte subfrm;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: byte *p;
            byte p;
            uint word;
            int i;
            int j;
            int sys;
            int week;
            int leaps = 0;
            int id = (GlobalMembersBinex.U4((byte)buff + 4) >> 8) & 7;

            if (id < 1 || 5 < id)
            {
                GlobalMembersRtkcmn.trace(2, "navigation subframe format error: id=%d\n", id);
                return 0;
            }
            subfrm = raw.subfrm[sat - 1];

            for (i = 0, p = subfrm + (id - 1) * 30; i < 10; i++)
            {
                word = GlobalMembersBinex.U4((byte)buff + i * 4) >> 6;
                for (j = 16; j >= 0; j -= 8)
                {
                    p++ = (word >> j) & 0xFF;
                }
            }
            if (id == 3) // ephemeris
            {
                eph.sat = sat;
                if (GlobalMembersRcvraw.decode_frame(subfrm, eph, null, null, null, null) != 1 || GlobalMembersRcvraw.decode_frame(subfrm + 30, eph, null, null, null, null) != 2 || GlobalMembersRcvraw.decode_frame(subfrm + 60, eph, null, null, null, null) != 3)
                {
                    return 0;
                }
                if (!StringFunctions.StrStr(raw.opt, "-EPHALL"))
                {
                    if (eph.iode == raw.nav.eph[sat - 1].iode && eph.iodc == raw.nav.eph[sat - 1].iodc) // unchanged
                    {
                        return 0;
                    }
                }
                raw.nav.eph[sat - 1] = eph;
                raw.ephsat = sat;
                return 2;
            }
            if (id == 4) // almanac or ion/utc parameters
            {
                if (GlobalMembersRcvraw.decode_frame(subfrm + 90, null, null, ref ion, ref utc, ref leaps) != 4)
                {
                    return 0;
                }
                if (GlobalMembersRtkcmn.norm(ion, 8) == 0.0 || GlobalMembersRtkcmn.norm(utc, 4) == 0.0 || raw.time.time == 0)
                {
                    return 0;
                }
                sys = GlobalMembersRtkcmn.satsys(sat, null);
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: time2gpst(raw->time,&week);
                GlobalMembersRtkcmn.time2gpst(new gtime_t(raw.time), ref week);
                utc[3] += Math.Floor((week - utc[3]) / 256.0 + 0.5) * 256.0;

                if (sys == DefineConstants.SYS_GPS)
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
                if (sys == DefineConstants.SYS_QZS)
                {
                    for (i = 0; i < 8; i++)
                    {
                        raw.nav.ion_qzs[i] = ion[i];
                    }
                    for (i = 0; i < 4; i++)
                    {
                        raw.nav.utc_qzs[i] = utc[i];
                    }
                    raw.nav.leaps = leaps;
                    return 9;
                }
            }
            return 0;
        }
        /* decode raw L2C CNAV data --------------------------------------------------*/
        internal static int decode_L2nav(byte[] buff, int len, int sat, raw_t raw)
        {
            byte[] msg = new byte[1024];
            int i;
            int j;
            int preamb;
            int prn;
            int msgid;
            int tow;
            int alert;

            GlobalMembersRtkcmn.trace(3, "decode_L2nav len=%2d sat=%2d L5 CNAV\n", len, sat);

            for (i = 0; i < len; i++)
            {
                for (j = 0; j < 4; j++)
                {
                    msg[3 - j + i * 4] = buff[j + i * 4];
                }
            }
            i = 0;
            preamb = GlobalMembersRtkcmn.getbitu(msg, i, 8);
            i += 8;
            prn = GlobalMembersRtkcmn.getbitu(msg, i, 6);
            i += 6;
            msgid = GlobalMembersRtkcmn.getbitu(msg, i, 6);
            i += 6;
            tow = GlobalMembersRtkcmn.getbitu(msg, i, 17);
            i += 17;
            alert = GlobalMembersRtkcmn.getbitu(msg, i, 1);
            i += 1;

            if (preamb != DefineConstants.PREAMB_CNAV)
            {
                GlobalMembersRtkcmn.trace(2, "javad *d sat=%2d L2 CNAV preamble error preamb=%02X\n", preamb);
                return -1;
            }
            GlobalMembersRtkcmn.trace(3, "L2CNAV: sat=%2d prn=%2d msgid=%2d tow=%6d alert=%d\n", sat, prn, msgid, tow, alert);

            return 0;
        }
        /* decode raw L5 CNAV data ---------------------------------------------------*/
        internal static int decode_L5nav(byte[] buff, int len, int sat, raw_t raw)
        {
            byte[] msg = new byte[1024];
            int i;
            int j;
            int preamb;
            int prn;
            int msgid;
            int tow;
            int alert;

            GlobalMembersRtkcmn.trace(3, "decode_L5nav len=%2d sat=%2d L5 CNAV\n", len, sat);

            for (i = 0; i < len; i++)
            {
                for (j = 0; j < 4; j++)
                {
                    msg[3 - j + i * 4] = buff[j + i * 4];
                }
            }
            i = 0;
            preamb = GlobalMembersRtkcmn.getbitu(msg, i, 8);
            i += 8;
            prn = GlobalMembersRtkcmn.getbitu(msg, i, 6);
            i += 6;
            msgid = GlobalMembersRtkcmn.getbitu(msg, i, 6);
            i += 6;
            tow = GlobalMembersRtkcmn.getbitu(msg, i, 17);
            i += 17;
            alert = GlobalMembersRtkcmn.getbitu(msg, i, 1);
            i += 1;

            if (preamb != DefineConstants.PREAMB_CNAV)
            {
                GlobalMembersRtkcmn.trace(2, "javad *d sat=%2d L5 CNAV preamble error preamb=%02X\n", preamb);
                return -1;
            }
            GlobalMembersRtkcmn.trace(3, "L5CNAV: sat=%2d prn=%2d msgid=%2d tow=%6d alert=%d\n", sat, prn, msgid, tow, alert);

            return 0;
        }
        /* decode raw L1C CNAV2 data -------------------------------------------------*/
        internal static int decode_L1Cnav(ref byte buff, int len, int sat, raw_t raw)
        {
            GlobalMembersRtkcmn.trace(2, "javad *d len=%2d sat=%2d L1C CNAV2 not supported\n", len, sat);

            return 0;
        }
        /* decode [*D] raw navigation data -------------------------------------------*/
        internal static int decode_nD(raw_t raw, int sys)
        {
            int i;
            int n;
            int siz;
            int sat;
            int prn;
            int stat = 0;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: byte *p=raw->buff+5;
            byte p = raw.buff + 5;

            if (GlobalMembersJavad.checksum(raw.buff, raw.len) == 0)
            {
                GlobalMembersRtkcmn.trace(2, "javad nD checksum error: sys=%d len=%d\n", sys, raw.len);
                return -1;
            }
            siz = ((byte)(p));
            p += 1;
            n = (raw.len - 7) / siz;

            if (n <= 0)
            {
                GlobalMembersRtkcmn.trace(2, "javad nD length error: sys=%d len=%d\n", sys, raw.len);
                return -1;
            }
            for (i = 0; i < n; i++, p += siz)
            {
                GlobalMembersRtkcmn.trace(3, "decode_*D: sys=%2d prn=%3d\n", sys, ((byte)(p)));

                prn = ((byte)(p));
                if ((sat = GlobalMembersRtkcmn.satno(sys, prn)) == 0)
                {
                    GlobalMembersRtkcmn.trace(2, "javad nD satellite error: sys=%d prn=%d\n", sys, prn);
                    continue;
                }
                stat = GlobalMembersJavad.decode_L1nav(ref p + 2, 0, sat, raw);
            }
            return stat;
        }
        /* decode [*d] raw navigation data -------------------------------------------*/
        internal static int decode_nd(raw_t raw, int sys)
        {
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: byte *p=raw->buff+5;
            byte p = raw.buff + 5;
            string msg;
            int sat;
            int prn;
            int time;
            int type;
            int len;

            if (GlobalMembersJavad.checksum(raw.buff, raw.len) == 0)
            {
                GlobalMembersRtkcmn.trace(2, "javad nd checksum error: sys=%d len=%d\n", sys, raw.len);
                return -1;
            }
            GlobalMembersRtkcmn.trace(3, "decode_*d: sys=%2d prn=%3d\n", sys, ((byte)(p)));

            prn = ((byte)(p));
            p += 1;
            time = GlobalMembersBinex.U4(p);
            p += 4;
            type = ((byte)(p));
            p += 1;
            len = ((byte)(p));
            p += 1;
            if (raw.len != 13 + len * 4)
            {
                GlobalMembersRtkcmn.trace(2, "javad nd length error: sys=%d len=%d\n", sys, raw.len);
                return -1;
            }
            if (raw.outtype != 0)
            {
                msg = raw.msgtype.Substring(raw.msgtype.Length);
                msg = string.Format(" prn={0,3:D} time={1,7:D} type={2:D}", prn, time, type);
            }
            if ((sat = GlobalMembersRtkcmn.satno(sys, prn)) == 0)
            {
                GlobalMembersRtkcmn.trace(2, "javad nd satellite error: sys=%d prn=%d\n", sys, prn);
                return 0;
            }
            GlobalMembersRtkcmn.trace(4, "sat=%2d time=%7d type=%d len=%3d\n", sat, time, type, len);

            switch (type)
            {
                case 0: // L1  NAV
                    return GlobalMembersJavad.decode_L1nav(ref p, len, sat, raw);
                case 1: // L2C CNAV
                    return GlobalMembersJavad.decode_L2nav(p, len, sat, raw);
                case 2: // L5  CNAV
                    return GlobalMembersJavad.decode_L5nav(p, len, sat, raw);
                case 3: // L1C CNAV2
                    return GlobalMembersJavad.decode_L1Cnav(ref p, len, sat, raw);
                case 4:
                    break;
            }
            return 0;
        }
        /* decode [LD] glonass raw navigation data -----------------------------------*/
        internal static int decode_LD(raw_t raw)
        {
            GlobalMembersRtkcmn.trace(2, "javad LD not supported\n");

            return 0;
        }
        /* decode [lD] glonass raw navigation data -----------------------------------*/
        internal static int decode_lD(raw_t raw)
        {
            geph_t geph = new geph_t();
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: byte *p=raw->buff+5;
            byte p = raw.buff + 5;
            string msg;
            int i;
            int sat;
            int prn;
            int frq;
            int time;
            int type;
            int len;
            int id;

            if (GlobalMembersJavad.checksum(raw.buff, raw.len) == 0)
            {
                GlobalMembersRtkcmn.trace(2, "javad lD checksum error: len=%d\n", raw.len);
                return -1;
            }
            GlobalMembersRtkcmn.trace(3, "decode_lD: prn=%3d\n", ((byte)(p)));

            prn = ((byte)(p));
            p += 1;
            frq = ((string)(p));
            p += 1;
            time = GlobalMembersBinex.U4(p);
            p += 4;
            type = ((byte)(p));
            p += 1;
            len = ((byte)(p));
            p += 1;

            if (raw.len != 14 + len * 4)
            {
                GlobalMembersRtkcmn.trace(2, "javad lD length error: len=%d\n", raw.len);
                return -1;
            }
            if (raw.outtype != 0)
            {
                msg = raw.msgtype.Substring(raw.msgtype.Length);
                msg = string.Format(" prn={0,2:D} frq={1,2:D} time={2,7:D} type={3:D}", prn, frq, time, type);
            }
            if ((sat = GlobalMembersRtkcmn.satno(DefineConstants.SYS_GLO, prn)) == 0)
            {
                GlobalMembersRtkcmn.trace(2, "javad lD satellite error: prn=%d\n", prn);
                return 0;
            }
            if (type != 0)
            {
                GlobalMembersRtkcmn.trace(3, "javad lD type not supported: type=%d\n", type);
                return 0;
            }
            if ((id = (GlobalMembersBinex.U4(p) >> 20) & 0xF) < 1)
            {
                return 0;
            }

            /* get 77 bit (25x3+2) in frame without hamming and time mark */
            for (i = 0; i < 4; i++)
            {
                GlobalMembersRtkcmn.setbitu(raw.subfrm[sat - 1] + (id - 1) * 10, i * 25, i < 3 ? 25 : 2, GlobalMembersBinex.U4(p + 4 * i) >> (i < 3 ? 0 : 23));
            }
            if (id != 4)
            {
                return 0;
            }

            /* decode glonass ephemeris strings */
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: geph.tof=raw->time;
            geph.tof.CopyFrom(raw.time);
            if (GlobalMembersRcvraw.decode_glostr(raw.subfrm[sat - 1], geph) == 0 || geph.sat != sat)
            {
                return -1;
            }
            geph.frq = frq;

            if (!StringFunctions.StrStr(raw.opt, "-EPHALL"))
            {
                if (geph.iode == raw.nav.geph[prn - 1].iode) // unchanged
                {
                    return 0;
                }
            }
            raw.nav.geph[prn - 1] = geph;
            raw.ephsat = sat;
            return 2;
        }
        /* decode [WD] waas raw navigation data --------------------------------------*/
        internal static int decode_WD(raw_t raw)
        {
            int i;
            int prn;
            int tow;
            int tow_p;
            int week;
            string msg;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: byte *p=raw->buff+5;
            byte p = raw.buff + 5;

            if (GlobalMembersJavad.checksum(raw.buff, raw.len) == 0)
            {
                GlobalMembersRtkcmn.trace(2, "javad WD checksum error: len=%d\n", raw.len);
                return -1;
            }
            if (raw.len < 45)
            {
                GlobalMembersRtkcmn.trace(2, "javad WD length error: len=%d\n", raw.len);
                return -1;
            }
            GlobalMembersRtkcmn.trace(3, "decode_WD: prn=%3d\n", ((byte)(p)));

            prn = ((byte)(p));
            p += 1;
            tow = GlobalMembersBinex.U4(p);
            p += 4 + 2;

            if (raw.outtype != 0)
            {
                msg = raw.msgtype.Substring(raw.msgtype.Length);
                msg = string.Format(" prn={0,3:D} tow={1,6:D}", prn, tow);
            }
            if ((prn < DefineConstants.MINPRNSBS || DefineConstants.MAXPRNSBS < prn) && (prn < DefineConstants.MINPRNQZS || DefineConstants.MAXPRNQZS < prn))
            {
                GlobalMembersRtkcmn.trace(2, "javad WD satellite error: prn=%d\n", prn);
                return 0;
            }
            raw.sbsmsg.prn = prn;
            raw.sbsmsg.tow = tow;

            if (raw.time.time == 0)
            {
                raw.sbsmsg.week = 0;
            }
            else
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: tow_p=(int)time2gpst(raw->time,&week);
                tow_p = (int)GlobalMembersRtkcmn.time2gpst(new gtime_t(raw.time), ref week);
                if (tow < tow_p - 302400.0)
                {
                    week++;
                }
                else if (tow > tow_p + 302400.0)
                {
                    week--;
                }
                raw.sbsmsg.week = week;
            }
            for (i = 0; i < 29; i++)
            {
                raw.sbsmsg.msg[i] = p++;
            }
            raw.sbsmsg.msg[28] &= 0xC0;
            return 3;
        }
        /* decode [R*] pseudoranges --------------------------------------------------*/
        internal static int decode_Rx(raw_t raw, sbyte code)
        {
            double pr;
            double prm;
            int i;
            int j;
            int freq;
            int type;
            int sat;
            int sys;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: byte *p=raw->buff+5;
            byte p = raw.buff + 5;

            if (GlobalMembersJavad.is_meas(code) == 0 || raw.tod < 0 || raw.obuf.n == 0)
            {
                return 0;
            }

            if (GlobalMembersJavad.checksum(raw.buff, raw.len) == 0)
            {
                GlobalMembersRtkcmn.trace(2, "javad R%c checksum error: len=%d\n", code, raw.len);
                return -1;
            }
            if (raw.len != raw.obuf.n * 8 + 6)
            {
                GlobalMembersRtkcmn.trace(2, "javad R%c length error: n=%d len=%d\n", code, raw.obuf.n, raw.len);
                return -1;
            }
            for (i = 0; i < raw.obuf.n && i < DefineConstants.MAXOBS; i++)
            {
                pr = GlobalMembersBinex.R8(p);
                p += 8;
                if (pr == 0.0)
                    continue;

                sat = raw.obuf.data[i].sat;
                if ((sys = GlobalMembersRtkcmn.satsys(sat, null)) == 0)
                    continue;

                prm = pr * DefineConstants.CLIGHT;

                if (code == 'C')
                {
                    raw.prCA[sat - 1] = prm;
                }

                if ((freq = GlobalMembersJavad.tofreq(code, sys, ref type)) < 0)
                    continue;

                if ((j = GlobalMembersJavad.checkpri(raw.opt, sys, type, freq)) >= 0)
                {
                    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                    //ORIGINAL LINE: if (!settag(raw->obuf.data+i,raw->time))
                    if (GlobalMembersJavad.settag(raw.obuf.data + i, new gtime_t(raw.time)) == 0)
                        continue;
                    raw.obuf.data[i].P[j] = prm;
                    raw.obuf.data[i].code[j] = type;
                }
            }
            return 0;
        }
        /* decode [r*] short pseudoranges --------------------------------------------*/
        internal static int decode_rx(raw_t raw, sbyte code)
        {
            double prm;
            int i;
            int j;
            int pr;
            int freq;
            int type;
            int sat;
            int sys;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: byte *p=raw->buff+5;
            byte p = raw.buff + 5;

            if (GlobalMembersJavad.is_meas(code) == 0 || raw.tod < 0 || raw.obuf.n == 0)
            {
                return 0;
            }

            if (GlobalMembersJavad.checksum(raw.buff, raw.len) == 0)
            {
                GlobalMembersRtkcmn.trace(2, "javad r%c checksum error: len=%d\n", code, raw.len);
                return -1;
            }
            if (raw.len != raw.obuf.n * 4 + 6)
            {
                GlobalMembersRtkcmn.trace(2, "javad r%c length error: n=%d len=%d\n", code, raw.obuf.n, raw.len);
                return -1;
            }
            for (i = 0; i < raw.obuf.n && i < DefineConstants.MAXOBS; i++)
            {
                pr = GlobalMembersBinex.I4(ref p);
                p += 4;
                sat = raw.obuf.data[i].sat;
                if ((sys = GlobalMembersRtkcmn.satsys(sat, null)) == 0)
                    continue;

                if (pr == 0x7FFFFFFF)
                {
                    GlobalMembersRtkcmn.trace(2, "javad r%c value missing: sat=%2d\n", code, sat);
                    continue;
                }
                if (sys == DefineConstants.SYS_SBS)
                {
                    prm = (pr * 1E-11 + 0.115) * DefineConstants.CLIGHT;
                }
                else if (sys == DefineConstants.SYS_QZS) // [3]
                {
                    prm = (pr * 2E-11 + 0.125) * DefineConstants.CLIGHT;
                }
                else if (sys == DefineConstants.SYS_CMP) // [4]
                {
                    prm = (pr * 2E-11 + 0.105) * DefineConstants.CLIGHT;
                }
                else if (sys == DefineConstants.SYS_GAL) // [3]
                {
                    prm = (pr * 1E-11 + 0.090) * DefineConstants.CLIGHT;
                }
                else
                {
                    prm = (pr * 1E-11 + 0.075) * DefineConstants.CLIGHT;
                }

                if (code == 'c')
                {
                    raw.prCA[sat - 1] = prm;
                }

                if ((freq = GlobalMembersJavad.tofreq(code, sys, ref type)) < 0)
                    continue;

                if ((j = GlobalMembersJavad.checkpri(raw.opt, sys, type, freq)) >= 0)
                {
                    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                    //ORIGINAL LINE: if (!settag(raw->obuf.data+i,raw->time))
                    if (GlobalMembersJavad.settag(raw.obuf.data + i, new gtime_t(raw.time)) == 0)
                        continue;
                    raw.obuf.data[i].P[j] = prm;
                    raw.obuf.data[i].code[j] = type;
                }
            }
            return 0;
        }
        /* decode [*R] relative pseudoranges -----------------------------------------*/
        internal static int decode_xR(raw_t raw, sbyte code)
        {
            float pr;
            int i;
            int j;
            int freq;
            int type;
            int sat;
            int sys;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: byte *p=raw->buff+5;
            byte p = raw.buff + 5;

            if (GlobalMembersJavad.is_meas(code) == 0 || raw.tod < 0 || raw.obuf.n == 0)
            {
                return 0;
            }

            if (GlobalMembersJavad.checksum(raw.buff, raw.len) == 0)
            {
                GlobalMembersRtkcmn.trace(2, "javad %cR checksum error: len=%d\n", code, raw.len);
                return -1;
            }
            if (raw.len != raw.obuf.n * 4 + 6)
            {
                GlobalMembersRtkcmn.trace(2, "javad %cR length error: n=%d len=%d\n", code, raw.obuf.n, raw.len);
                return -1;
            }
            for (i = 0; i < raw.obuf.n && i < DefineConstants.MAXOBS; i++)
            {
                pr = GlobalMembersBinex.R4(p);
                p += 4;
                if (pr == 0.0)
                    continue;

                sat = raw.obuf.data[i].sat;
                if ((sys = GlobalMembersRtkcmn.satsys(sat, null)) == 0 || raw.prCA[sat - 1] == 0.0)
                    continue;

                if ((freq = GlobalMembersJavad.tofreq(code, sys, ref type)) < 0)
                    continue;

                if ((j = GlobalMembersJavad.checkpri(raw.opt, sys, type, freq)) >= 0)
                {
                    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                    //ORIGINAL LINE: if (!settag(raw->obuf.data+i,raw->time))
                    if (GlobalMembersJavad.settag(raw.obuf.data + i, new gtime_t(raw.time)) == 0)
                        continue;
                    raw.obuf.data[i].P[j] = pr * DefineConstants.CLIGHT + raw.prCA[sat - 1];
                    raw.obuf.data[i].code[j] = type;
                }
            }
            return 0;
        }
        /* decode [*r] short relative pseudoranges -----------------------------------*/
        internal static int decode_xr(raw_t raw, sbyte code)
        {
            double prm;
            short pr;
            int i;
            int j;
            int freq;
            int type;
            int sat;
            int sys;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: byte *p=raw->buff+5;
            byte p = raw.buff + 5;

            if (GlobalMembersJavad.is_meas(code) == 0 || raw.tod < 0 || raw.obuf.n == 0)
            {
                return 0;
            }

            if (GlobalMembersJavad.checksum(raw.buff, raw.len) == 0)
            {
                GlobalMembersRtkcmn.trace(2, "javad %cr checksum error: len=%d\n", code, raw.len);
                return -1;
            }
            if (raw.len != raw.obuf.n * 2 + 6)
            {
                GlobalMembersRtkcmn.trace(2, "javad %cR length error: n=%d len=%d\n", code, raw.obuf.n, raw.len);
                return -1;
            }
            for (i = 0; i < raw.obuf.n && i < DefineConstants.MAXOBS; i++)
            {
                pr = GlobalMembersCrescent.I2(ref p);
                p += 2;
                if (pr == (short)0x7FFF)
                    continue;

                sat = raw.obuf.data[i].sat;
                if ((sys = GlobalMembersRtkcmn.satsys(sat, null)) == 0 || raw.prCA[sat - 1] == 0.0)
                    continue;

                prm = (pr * 1E-11 + 2E-7) * DefineConstants.CLIGHT + raw.prCA[sat - 1];

                if ((freq = GlobalMembersJavad.tofreq(code, sys, ref type)) < 0)
                    continue;

                if ((j = GlobalMembersJavad.checkpri(raw.opt, sys, type, freq)) >= 0)
                {
                    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                    //ORIGINAL LINE: if (!settag(raw->obuf.data+i,raw->time))
                    if (GlobalMembersJavad.settag(raw.obuf.data + i, new gtime_t(raw.time)) == 0)
                        continue;
                    raw.obuf.data[i].P[j] = prm;
                    raw.obuf.data[i].code[j] = type;
                }
            }
            return 0;
        }
        /* decode [P*] carrier phases ------------------------------------------------*/
        internal static int decode_Px(raw_t raw, sbyte code)
        {
            double cp;
            int i;
            int j;
            int freq;
            int type;
            int sys;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: byte *p=raw->buff+5;
            byte p = raw.buff + 5;

            if (GlobalMembersJavad.is_meas(code) == 0 || raw.tod < 0 || raw.obuf.n == 0)
            {
                return 0;
            }

            if (GlobalMembersJavad.checksum(raw.buff, raw.len) == 0)
            {
                GlobalMembersRtkcmn.trace(2, "javad P%c checksum error: len=%d\n", code, raw.len);
                return -1;
            }
            if (raw.len != raw.obuf.n * 8 + 6)
            {
                GlobalMembersRtkcmn.trace(2, "javad P%c length error: n=%d len=%d\n", code, raw.obuf.n, raw.len);
                return -1;
            }
            for (i = 0; i < raw.obuf.n && i < DefineConstants.MAXOBS; i++)
            {
                cp = GlobalMembersBinex.R8(p);
                p += 8;
                if (cp == 0.0)
                    continue;

                if ((sys = GlobalMembersRtkcmn.satsys(raw.obuf.data[i].sat, null)) == 0)
                    continue;

                if ((freq = GlobalMembersJavad.tofreq(code, sys, ref type)) < 0)
                    continue;

                if ((j = GlobalMembersJavad.checkpri(raw.opt, sys, type, freq)) >= 0)
                {
                    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                    //ORIGINAL LINE: if (!settag(raw->obuf.data+i,raw->time))
                    if (GlobalMembersJavad.settag(raw.obuf.data + i, new gtime_t(raw.time)) == 0)
                        continue;
                    raw.obuf.data[i].L[j] = cp;
                    raw.obuf.data[i].code[j] = type;
                }
            }
            return 0;
        }
        /* decode [p*] short carrier phases ------------------------------------------*/
        internal static int decode_px(raw_t raw, sbyte code)
        {
            uint cp;
            int i;
            int j;
            int freq;
            int type;
            int sys;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: byte *p=raw->buff+5;
            byte p = raw.buff + 5;

            if (GlobalMembersJavad.is_meas(code) == 0 || raw.tod < 0 || raw.obuf.n == 0)
            {
                return 0;
            }

            if (GlobalMembersJavad.checksum(raw.buff, raw.len) == 0)
            {
                GlobalMembersRtkcmn.trace(2, "javad p%c checksum error: len=%d\n", code, raw.len);
                return -1;
            }
            if (raw.len != raw.obuf.n * 4 + 6)
            {
                GlobalMembersRtkcmn.trace(2, "javad p%c length error: n=%d len=%d\n", code, raw.obuf.n, raw.len);
                return -1;
            }
            for (i = 0; i < raw.obuf.n && i < DefineConstants.MAXOBS; i++)
            {
                cp = GlobalMembersBinex.U4(p);
                p += 4;
                if (cp == 0xFFFFFFFF)
                    continue;

                if ((sys = GlobalMembersRtkcmn.satsys(raw.obuf.data[i].sat, null)) == 0)
                    continue;

                if ((freq = GlobalMembersJavad.tofreq(code, sys, ref type)) < 0)
                    continue;

                if ((j = GlobalMembersJavad.checkpri(raw.opt, sys, type, freq)) >= 0)
                {
                    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                    //ORIGINAL LINE: if (!settag(raw->obuf.data+i,raw->time))
                    if (GlobalMembersJavad.settag(raw.obuf.data + i, new gtime_t(raw.time)) == 0)
                        continue;
                    raw.obuf.data[i].L[j] = cp / 1024.0;
                    raw.obuf.data[i].code[j] = type;
                }
            }
            return 0;
        }
        /* decode [*P] short relative carrier phases ---------------------------------*/
        internal static int decode_xP(raw_t raw, sbyte code)
        {
            double cp;
            double rcp;
            double fn;
            int i;
            int j;
            int freq;
            int type;
            int sat;
            int sys;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: byte *p=raw->buff+5;
            byte p = raw.buff + 5;

            if (GlobalMembersJavad.is_meas(code) == 0 || raw.tod < 0 || raw.obuf.n == 0)
            {
                return 0;
            }

            if (GlobalMembersJavad.checksum(raw.buff, raw.len) == 0)
            {
                GlobalMembersRtkcmn.trace(2, "javad %cP checksum error: len=%d\n", code, raw.len);
                return -1;
            }
            if (raw.len != raw.obuf.n * 4 + 6)
            {
                GlobalMembersRtkcmn.trace(2, "javad %cP length error: n=%d len=%d\n", code, raw.obuf.n, raw.len);
                return -1;
            }
            for (i = 0; i < raw.obuf.n && i < DefineConstants.MAXOBS; i++)
            {
                rcp = GlobalMembersBinex.R4(p);
                p += 4;
                if (rcp == 0.0)
                    continue;

                sat = raw.obuf.data[i].sat;
                if ((sys = GlobalMembersRtkcmn.satsys(sat, null)) == 0 || raw.prCA[sat - 1] == 0.0)
                    continue;

                if ((freq = GlobalMembersJavad.tofreq(code, sys, ref type)) < 0)
                    continue;

                if ((j = GlobalMembersJavad.checkpri(raw.opt, sys, type, freq)) >= 0)
                {
                    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                    //ORIGINAL LINE: if (!settag(raw->obuf.data+i,raw->time))
                    if (GlobalMembersJavad.settag(raw.obuf.data + i, new gtime_t(raw.time)) == 0)
                        continue;

                    fn = sys == DefineConstants.SYS_GLO ? GlobalMembersJavad.freq_glo(freq, raw.freqn[i]) : DefineConstants.CLIGHT / GlobalMembersRtkcmn.lam_carr[freq];
                    cp = (rcp + raw.prCA[sat - 1] / DefineConstants.CLIGHT) * fn;

                    raw.obuf.data[i].L[j] = cp;
                    raw.obuf.data[i].code[j] = type;
                }
            }
            return 0;
        }
        /* decode [*p] short relative carrier phases ---------------------------------*/
        internal static int decode_xp(raw_t raw, sbyte code)
        {
            double cp;
            double fn;
            int i;
            int j;
            int rcp;
            int freq;
            int type;
            int sat;
            int sys;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: byte *p=raw->buff+5;
            byte p = raw.buff + 5;

            if (GlobalMembersJavad.is_meas(code) == 0 || raw.tod < 0 || raw.obuf.n == 0)
            {
                return 0;
            }

            if (GlobalMembersJavad.checksum(raw.buff, raw.len) == 0)
            {
                GlobalMembersRtkcmn.trace(2, "javad %cp checksum error: len=%d\n", code, raw.len);
                return -1;
            }
            if (raw.len != raw.obuf.n * 4 + 6)
            {
                GlobalMembersRtkcmn.trace(2, "javad %cp length error: n=%d len=%d\n", code, raw.obuf.n, raw.len);
                return -1;
            }
            for (i = 0; i < raw.obuf.n && i < DefineConstants.MAXOBS; i++)
            {
                rcp = GlobalMembersBinex.I4(ref p);
                p += 4;
                if (rcp == 0x7FFFFFFF)
                    continue;

                sat = raw.obuf.data[i].sat;
                if ((sys = GlobalMembersRtkcmn.satsys(sat, null)) == 0 || raw.prCA[sat - 1] == 0.0)
                    continue;

                if ((freq = GlobalMembersJavad.tofreq(code, sys, ref type)) < 0)
                    continue;

                if ((j = GlobalMembersJavad.checkpri(raw.opt, sys, type, freq)) >= 0)
                {
                    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                    //ORIGINAL LINE: if (!settag(raw->obuf.data+i,raw->time))
                    if (GlobalMembersJavad.settag(raw.obuf.data + i, new gtime_t(raw.time)) == 0)
                        continue;

                    fn = sys == DefineConstants.SYS_GLO ? GlobalMembersJavad.freq_glo(freq, raw.freqn[i]) : DefineConstants.CLIGHT / GlobalMembersRtkcmn.lam_carr[freq];
                    cp = (rcp * DefineConstants.P2_40 + raw.prCA[sat - 1] / DefineConstants.CLIGHT) * fn;

                    raw.obuf.data[i].L[j] = cp;
                    raw.obuf.data[i].code[j] = type;
                }
            }
            return 0;
        }
        /* decode [D*] doppler -------------------------------------------------------*/
        internal static int decode_Dx(raw_t raw, sbyte code)
        {
            double dop;
            int i;
            int j;
            int dp;
            int freq;
            int type;
            int sat;
            int sys;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: byte *p=raw->buff+5;
            byte p = raw.buff + 5;

            if (GlobalMembersJavad.is_meas(code) == 0 || raw.tod < 0 || raw.obuf.n == 0)
            {
                return 0;
            }

            if (GlobalMembersJavad.checksum(raw.buff, raw.len) == 0)
            {
                GlobalMembersRtkcmn.trace(2, "javad D%c checksum error: len=%d\n", code, raw.len);
                return -1;
            }
            if (raw.len != raw.obuf.n * 4 + 6)
            {
                GlobalMembersRtkcmn.trace(2, "javad D%c length error: n=%d len=%d\n", code, raw.obuf.n, raw.len);
                return -1;
            }
            for (i = 0; i < raw.obuf.n && i < DefineConstants.MAXOBS; i++)
            {
                dp = GlobalMembersBinex.I4(ref p);
                p += 4;
                if (dp == 0x7FFFFFFF)
                    continue;

                sat = raw.obuf.data[i].sat;
                if ((sys = GlobalMembersRtkcmn.satsys(sat, null)) == 0)
                    continue;

                dop = -dp * 1E-4;

                if (code == 'C')
                {
                    raw.dpCA[sat - 1] = dop;
                }

                if ((freq = GlobalMembersJavad.tofreq(code, sys, ref type)) < 0)
                    continue;

                if ((j = GlobalMembersJavad.checkpri(raw.opt, sys, type, freq)) >= 0)
                {
                    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                    //ORIGINAL LINE: if (!settag(raw->obuf.data+i,raw->time))
                    if (GlobalMembersJavad.settag(raw.obuf.data + i, new gtime_t(raw.time)) == 0)
                        continue;
                    raw.obuf.data[i].D[j] = (float)dop;
                }
            }
            return 0;
        }
        /* decode [*d] short relative doppler ----------------------------------------*/
        internal static int decode_xd(raw_t raw, sbyte code)
        {
            double dop;
            double f1;
            double fn;
            short rdp;
            int i;
            int j;
            int freq;
            int type;
            int sat;
            int sys;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: byte *p=raw->buff+5;
            byte p = raw.buff + 5;

            if (GlobalMembersJavad.is_meas(code) == 0 || raw.tod < 0 || raw.obuf.n == 0)
            {
                return 0;
            }

            if (GlobalMembersJavad.checksum(raw.buff, raw.len) == 0)
            {
                GlobalMembersRtkcmn.trace(2, "javad %cd checksum error: len=%d\n", code, raw.len);
                return -1;
            }
            if (raw.len != raw.obuf.n * 2 + 6)
            {
                GlobalMembersRtkcmn.trace(2, "javad %cd length error: n=%d len=%d\n", code, raw.obuf.n, raw.len);
                return -1;
            }
            for (i = 0; i < raw.obuf.n && i < DefineConstants.MAXOBS; i++)
            {
                rdp = GlobalMembersCrescent.I2(ref p);
                p += 2;
                if (rdp == (short)0x7FFF)
                    continue;

                sat = raw.obuf.data[i].sat;
                if ((sys = GlobalMembersRtkcmn.satsys(sat, null)) == 0 || raw.dpCA[sat - 1] == 0.0)
                    continue;

                if ((freq = GlobalMembersJavad.tofreq(code, sys, ref type)) < 0)
                    continue;

                if ((j = GlobalMembersJavad.checkpri(raw.opt, sys, type, freq)) >= 0)
                {
                    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                    //ORIGINAL LINE: if (!settag(raw->obuf.data+i,raw->time))
                    if (GlobalMembersJavad.settag(raw.obuf.data + i, new gtime_t(raw.time)) == 0)
                        continue;
                    f1 = sys == DefineConstants.SYS_GLO ? GlobalMembersJavad.freq_glo(0, raw.freqn[i]) : DefineConstants.CLIGHT / GlobalMembersRtkcmn.lam_carr[0];
                    fn = sys == DefineConstants.SYS_GLO ? GlobalMembersJavad.freq_glo(freq, raw.freqn[i]) : DefineConstants.CLIGHT / GlobalMembersRtkcmn.lam_carr[freq];
                    dop = (-rdp + raw.dpCA[sat - 1] * 1E4) * fn / f1 * 1E-4;

                    raw.obuf.data[i].D[j] = (float)dop;
                }
            }
            return 0;
        }
        /* decode [E*] carrier to noise ratio ----------------------------------------*/
        internal static int decode_Ex(raw_t raw, sbyte code)
        {
            byte cnr;
            int i;
            int j;
            int freq;
            int type;
            int sys;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: byte *p=raw->buff+5;
            byte p = raw.buff + 5;

            if (GlobalMembersJavad.is_meas(code) == 0 || raw.tod < 0 || raw.obuf.n == 0)
            {
                return 0;
            }

            if (GlobalMembersJavad.checksum(raw.buff, raw.len) == 0)
            {
                GlobalMembersRtkcmn.trace(2, "javad E%c checksum error: len=%d\n", code, raw.len);
                return -1;
            }
            if (raw.len != raw.obuf.n + 6)
            {
                GlobalMembersRtkcmn.trace(2, "javad E%c length error: n=%d len=%d\n", code, raw.obuf.n, raw.len);
                return -1;
            }
            for (i = 0; i < raw.obuf.n && i < DefineConstants.MAXOBS; i++)
            {
                cnr = ((byte)(p));
                p += 1;
                if (cnr == 255)
                    continue;

                if ((sys = GlobalMembersRtkcmn.satsys(raw.obuf.data[i].sat, null)) == 0)
                    continue;

                if ((freq = GlobalMembersJavad.tofreq(code, sys, ref type)) < 0)
                    continue;

                if ((j = GlobalMembersJavad.checkpri(raw.opt, sys, type, freq)) >= 0)
                {
                    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                    //ORIGINAL LINE: if (!settag(raw->obuf.data+i,raw->time))
                    if (GlobalMembersJavad.settag(raw.obuf.data + i, new gtime_t(raw.time)) == 0)
                        continue;
                    raw.obuf.data[i].SNR[j] = (byte)(cnr * 4.0 + 0.5);
                }
            }
            return 0;
        }
        /* decode [*E] carrier to noise ratio x 4 ------------------------------------*/
        internal static int decode_xE(raw_t raw, sbyte code)
        {
            byte cnr;
            int i;
            int j;
            int freq;
            int type;
            int sys;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: byte *p=raw->buff+5;
            byte p = raw.buff + 5;

            if (GlobalMembersJavad.is_meas(code) == 0 || raw.tod < 0 || raw.obuf.n == 0)
            {
                return 0;
            }

            if (GlobalMembersJavad.checksum(raw.buff, raw.len) == 0)
            {
                GlobalMembersRtkcmn.trace(2, "javad %cE checksum error: len=%d\n", code, raw.len);
                return -1;
            }
            if (raw.len != raw.obuf.n + 6)
            {
                GlobalMembersRtkcmn.trace(2, "javad %cE length error: n=%d len=%d\n", code, raw.obuf.n, raw.len);
                return -1;
            }
            for (i = 0; i < raw.obuf.n && i < DefineConstants.MAXOBS; i++)
            {
                cnr = ((byte)(p));
                p += 1;
                if (cnr == 255)
                    continue;

                if ((sys = GlobalMembersRtkcmn.satsys(raw.obuf.data[i].sat, null)) == 0)
                    continue;

                if ((freq = GlobalMembersJavad.tofreq(code, sys, ref type)) < 0)
                    continue;

                if ((j = GlobalMembersJavad.checkpri(raw.opt, sys, type, freq)) >= 0)
                {
                    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                    //ORIGINAL LINE: if (!settag(raw->obuf.data+i,raw->time))
                    if (GlobalMembersJavad.settag(raw.obuf.data + i, new gtime_t(raw.time)) == 0)
                        continue;
                    raw.obuf.data[i].SNR[j] = cnr;
                }
            }
            return 0;
        }
        /* decode [F*] signal lock loop flags ----------------------------------------*/
        internal static int decode_Fx(raw_t raw, sbyte code)
        {
            ushort flags;
            int i;
            int j;
            int freq;
            int type;
            int sat;
            int sys;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: byte *p=raw->buff+5;
            byte p = raw.buff + 5;

            if (GlobalMembersJavad.is_meas(code) == 0 || raw.tod < 0 || raw.obuf.n == 0)
            {
                return 0;
            }

            if (GlobalMembersJavad.checksum(raw.buff, raw.len) == 0)
            {
                GlobalMembersRtkcmn.trace(2, "javad F%c checksum error: len=%d\n", code, raw.len);
                return -1;
            }
            if (raw.len != raw.obuf.n * 2 + 6)
            {
                GlobalMembersRtkcmn.trace(2, "javad F%c length error: n=%d len=%d\n", code, raw.obuf.n, raw.len);
                return -1;
            }
            for (i = 0; i < raw.obuf.n && i < DefineConstants.MAXOBS; i++)
            {
                flags = GlobalMembersBinex.U2(p);
                p += 1;
                if (flags == 0xFFFF)
                    continue;

                sat = raw.obuf.data[i].sat;
                if ((sys = GlobalMembersRtkcmn.satsys(sat, null)) == 0)
                    continue;

                if ((freq = GlobalMembersJavad.tofreq(code, sys, ref type)) < 0)
                    continue;

                if ((j = GlobalMembersJavad.checkpri(raw.opt, sys, type, freq)) >= 0)
                {
                    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                    //ORIGINAL LINE: if (!settag(raw->obuf.data+i,raw->time))
                    if (GlobalMembersJavad.settag(raw.obuf.data + i, new gtime_t(raw.time)) == 0)
                        continue;
#if false
	//            if (flags&0x20) { // loss-of-lock potential 
	//                raw->obuf.data[i].LLI[j]|=1;
	//            }
	//            if (!(flags&0x40)||!(flags&0x100)) { // integral indicator 
	//                raw->obuf.data[i].LLI[j]|=2;
	//            }
#endif
                }
            }
            return 0;
        }
        /* decode [TC] CA/L1 continuous tracking time --------------------------------*/
        internal static int decode_TC(raw_t raw)
        {
            ushort tt;
            ushort tt_p;
            int i;
            int sat;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: byte *p=raw->buff+5;
            byte p = raw.buff + 5;

            if (raw.obuf.n == 0)
            {
                return 0;
            }

            if (GlobalMembersJavad.checksum(raw.buff, raw.len) == 0)
            {
                GlobalMembersRtkcmn.trace(2, "javad TC checksum error: len=%d\n", raw.len);
                return -1;
            }
            if (raw.len != raw.obuf.n * 2 + 6)
            {
                GlobalMembersRtkcmn.trace(2, "javad TC length error: n=%d len=%d\n", raw.obuf.n, raw.len);
                return -1;
            }
            for (i = 0; i < raw.obuf.n && i < DefineConstants.MAXOBS; i++)
            {
                tt = GlobalMembersBinex.U2(p);
                p += 2;
                if (tt == 0xFFFF)
                    continue;

                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: if (!settag(raw->obuf.data+i,raw->time))
                if (GlobalMembersJavad.settag(raw.obuf.data + i, new gtime_t(raw.time)) == 0)
                    continue;

                sat = raw.obuf.data[i].sat;
                tt_p = (ushort)raw.lockt[sat - 1, 0];

                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: trace(4,"%s: sat=%2d tt=%6d->%6d\n",time_str(raw->time,3),sat,tt_p,tt);
                GlobalMembersRtkcmn.trace(4, "%s: sat=%2d tt=%6d->%6d\n", GlobalMembersRtkcmn.time_str(new gtime_t(raw.time), 3), sat, tt_p, tt);

                /* loss-of-lock detected by lock-time counter */
                if (tt == 0 || tt < tt_p)
                {
                    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                    //ORIGINAL LINE: trace(3,"decode_TC: loss-of-lock detected: t=%s sat=%2d tt=%6d->%6d\n", time_str(raw->time,3),sat,tt_p,tt);
                    GlobalMembersRtkcmn.trace(3, "decode_TC: loss-of-lock detected: t=%s sat=%2d tt=%6d->%6d\n", GlobalMembersRtkcmn.time_str(new gtime_t(raw.time), 3), sat, tt_p, tt);
                    raw.obuf.data[i].LLI[0] |= 1;
                }
                raw.lockt[sat - 1, 0] = tt;
            }
            return 0;
        }
        /* decode javad raw message --------------------------------------------------*/
        internal static int decode_javad(raw_t raw)
        {
            string p = (string)raw.buff;

            GlobalMembersRtkcmn.trace(3, "decode_javad: type=%2.2s len=%3d\n", p, raw.len);

            if (raw.outtype != 0)
            {
                //C++ TO C# CONVERTER TODO TASK: C# does not allow setting maximum string width in format specifiers:
                //ORIGINAL LINE: sprintf(raw->msgtype,"JAVAD %2.2s (%4d)",p,raw->len);
                raw.msgtype = string.Format("JAVAD {0,2} ({1,4:D})", p, raw.len);
            }
            if (!string.Compare(p, 0, "~~", 0, 2)) // receiver time
            {
                return GlobalMembersJavad.decode_RT(raw);
            }

            if (StringFunctions.StrStr(raw.opt, "-NOET"))
            {
                if (!string.Compare(p, 0, "::", 0, 2)) // epoch time
                {
                    return GlobalMembersJavad.decode_ET(raw);
                }
            }
            if (!string.Compare(p, 0, "RD", 0, 2)) // receiver date
            {
                return GlobalMembersJavad.decode_RD(raw);
            }
            if (!string.Compare(p, 0, "SI", 0, 2)) // satellite indices
            {
                return GlobalMembersJavad.decode_SI(raw);
            }
            if (!string.Compare(p, 0, "NN", 0, 2)) // glonass slot numbers
            {
                return GlobalMembersJavad.decode_NN(raw);
            }

            if (!string.Compare(p, 0, "GA", 0, 2)) // gps almanac
            {
                return GlobalMembersJavad.decode_GA(raw);
            }
            if (!string.Compare(p, 0, "NA", 0, 2)) // glonass almanac
            {
                return GlobalMembersJavad.decode_NA(raw);
            }
            if (!string.Compare(p, 0, "EA", 0, 2)) // galileo almanac
            {
                return GlobalMembersJavad.decode_EA(raw);
            }
            if (!string.Compare(p, 0, "WA", 0, 2)) // sbas almanac
            {
                return GlobalMembersJavad.decode_WA(raw);
            }
            if (!string.Compare(p, 0, "QA", 0, 2)) // qzss almanac (ext)
            {
                return GlobalMembersJavad.decode_QA(raw);
            }

            if (!string.Compare(p, 0, "GE", 0, 2)) // gps ephemeris
            {
                return GlobalMembersJavad.decode_GE(raw);
            }
            if (!string.Compare(p, 0, "NE", 0, 2)) // glonass ephemeris
            {
                return GlobalMembersJavad.decode_NE(raw);
            }
            if (!string.Compare(p, 0, "EN", 0, 2)) // galileo ephemeris
            {
                return GlobalMembersJavad.decode_EN(raw);
            }
            if (!string.Compare(p, 0, "WE", 0, 2)) // waas ephemeris
            {
                return GlobalMembersJavad.decode_WE(raw);
            }
            if (!string.Compare(p, 0, "QE", 0, 2)) // qzss ephemeris (ext)
            {
                return GlobalMembersJavad.decode_QE(raw);
            }
            if (!string.Compare(p, 0, "CN", 0, 2)) // beidou ephemeris (ext)
            {
                return GlobalMembersJavad.decode_CN(raw);
            }

            if (!string.Compare(p, 0, "UO", 0, 2)) // gps utc time parameters
            {
                return GlobalMembersJavad.decode_UO(raw);
            }
            if (!string.Compare(p, 0, "NU", 0, 2)) // glonass utc and gps time par
            {
                return GlobalMembersJavad.decode_NU(raw);
            }
            if (!string.Compare(p, 0, "EU", 0, 2)) // galileo utc and gps time par
            {
                return GlobalMembersJavad.decode_EU(raw);
            }
            if (!string.Compare(p, 0, "WU", 0, 2)) // waas utc time parameters
            {
                return GlobalMembersJavad.decode_WU(raw);
            }
            if (!string.Compare(p, 0, "QU", 0, 2)) // qzss utc and gps time par
            {
                return GlobalMembersJavad.decode_QU(raw);
            }
            if (!string.Compare(p, 0, "IO", 0, 2)) // ionospheric parameters
            {
                return GlobalMembersJavad.decode_IO(raw);
            }

            if (!string.Compare(p, 0, "GD", 0, 2)) // raw navigation data
            {
                return GlobalMembersJavad.decode_nD(raw, DefineConstants.SYS_GPS);
            }
            if (!string.Compare(p, 0, "QD", 0, 2)) // raw navigation data
            {
                return GlobalMembersJavad.decode_nD(raw, DefineConstants.SYS_QZS);
            }
            if (!string.Compare(p, 0, "gd", 0, 2)) // raw navigation data
            {
                return GlobalMembersJavad.decode_nd(raw, DefineConstants.SYS_GPS);
            }
            if (!string.Compare(p, 0, "qd", 0, 2)) // raw navigation data
            {
                return GlobalMembersJavad.decode_nd(raw, DefineConstants.SYS_QZS);
            }
            if (!string.Compare(p, 0, "ED", 0, 2)) // raw navigation data
            {
                return GlobalMembersJavad.decode_nd(raw, DefineConstants.SYS_GAL);
            }
            if (!string.Compare(p, 0, "cd", 0, 2)) // raw navigation data
            {
                return GlobalMembersJavad.decode_nd(raw, DefineConstants.SYS_CMP);
            }
            if (!string.Compare(p, 0, "LD", 0, 2)) // glonass raw navigation data
            {
                return GlobalMembersJavad.decode_LD(raw);
            }
            if (!string.Compare(p, 0, "lD", 0, 2)) // glonass raw navigation data
            {
                return GlobalMembersJavad.decode_lD(raw);
            }
            if (!string.Compare(p, 0, "WD", 0, 2)) // sbas raw navigation data
            {
                return GlobalMembersJavad.decode_WD(raw);
            }

            if (!string.Compare(p, 0, "TC", 0, 2)) // CA/L1 continuous track time
            {
                return GlobalMembersJavad.decode_TC(raw);
            }

            if (p[0] == 'R') // pseudoranges
            {
                return GlobalMembersJavad.decode_Rx(raw, p[1]);
            }
            if (p[0] == 'r') // short pseudoranges
            {
                return GlobalMembersJavad.decode_rx(raw, p[1]);
            }
            if (p[1] == 'R') // relative pseudoranges
            {
                return GlobalMembersJavad.decode_xR(raw, p[0]);
            }
            if (p[1] == 'r') // short relative pseudoranges
            {
                return GlobalMembersJavad.decode_xr(raw, p[0]);
            }
            if (p[0] == 'P') // carrier phases
            {
                return GlobalMembersJavad.decode_Px(raw, p[1]);
            }
            if (p[0] == 'p') // short carrier phases
            {
                return GlobalMembersJavad.decode_px(raw, p[1]);
            }
            if (p[1] == 'P') // relative carrier phases
            {
                return GlobalMembersJavad.decode_xP(raw, p[0]);
            }
            if (p[1] == 'p') // relative carrier phases
            {
                return GlobalMembersJavad.decode_xp(raw, p[0]);
            }
            if (p[0] == 'D') // doppler
            {
                return GlobalMembersJavad.decode_Dx(raw, p[1]);
            }
            if (p[1] == 'd') // short relative doppler
            {
                return GlobalMembersJavad.decode_xd(raw, p[0]);
            }
            if (p[0] == 'E') // carrier to noise ratio
            {
                return GlobalMembersJavad.decode_Ex(raw, p[1]);
            }
            if (p[1] == 'E') // carrier to noise ratio x 4
            {
                return GlobalMembersJavad.decode_xE(raw, p[0]);
            }
            if (p[0] == 'F') // signal lock loop flags
            {
                return GlobalMembersJavad.decode_Fx(raw, p[1]);
            }

            return 0;
        }
        /* sync javad message --------------------------------------------------------*/
        internal static int sync_javad(byte[] buff, byte data)
        {
            byte p = buff[0];

            buff[0] = buff[1];
            buff[1] = buff[2];
            buff[2] = buff[3];
            buff[3] = buff[4];
            buff[4] = data;

            /* sync message header {\r|\n}IIHHH (II:id,HHH: hex length) */
            return (p == '\r' || p == '\n') && ('0' <= (buff[0]) && (buff[0]) <= '~') && ('0' <= (buff[1]) && (buff[1]) <= '~') && (('0' <= (buff[2]) && (buff[2]) <= '9') || ('A' <= (buff[2]) && (buff[2]) <= 'F')) && (('0' <= (buff[3]) && (buff[3]) <= '9') || ('A' <= (buff[3]) && (buff[3]) <= 'F')) && (('0' <= (buff[4]) && (buff[4]) <= '9') || ('A' <= (buff[4]) && (buff[4]) <= 'F'));
        }
        /* clear buffer --------------------------------------------------------------*/
        internal static void clearbuff(raw_t raw)
        {
            int i;
            for (i = 0; i < 5; i++)
            {
                raw.buff[i] = 0;
            }
            raw.len = raw.nbyte = 0;
        }
        /* input javad raw message from stream -----------------------------------------
        * fetch next javad raw data and input a mesasge from stream
        * args   : raw_t *raw   IO     receiver raw data control struct
        *          unsigned char data I stream data (1 byte)
        * return : status (-1: error message, 0: no message, 1: input observation data,
        *                  2: input ephemeris, 3: input sbas message,
        *                  9: input ion/utc parameter)
        *
        * notes  : to specify input options, set raw->opt to the following option
        *          strings separated by spaces.
        *
        *          -EPHALL : input all ephemerides
        *          -GL1W   : select 1W for GPS L1 (default 1C)
        *          -GL1X   : select 1X for GPS L1 (default 1C)
        *          -GL2X   : select 2X for GPS L2 (default 2W)
        *          -RL1C   : select 1C for GLO L1 (default 1P)
        *          -RL2C   : select 2C for GLO L2 (default 2P)
        *          -JL1Z   : select 1Z for QZS L1 (default 1C)
        *          -JL1X   : select 1X for QZS L1 (default 1C)
        *          -NOET   : discard epoch time message ET (::)
        *
        *-----------------------------------------------------------------------------*/
        public static int input_javad(raw_t raw, byte data)
        {
            int len;
            int stat;

            GlobalMembersRtkcmn.trace(5, "input_javad: data=%02x\n", data);

            /* synchronize message */
            if (raw.nbyte == 0)
            {
                if (GlobalMembersJavad.sync_javad(raw.buff, data) == 0)
                {
                    return 0;
                }
                if ((len = GlobalMembersJavad.decodelen(raw.buff + 2)) == 0 || len > DefineConstants.MAXRAWLEN - 5)
                {
                    GlobalMembersRtkcmn.trace(2, "javad message length error: len=%d\n", len);
                    GlobalMembersJavad.clearbuff(raw);
                    return -1;
                }
                raw.len = len + 5;
                raw.nbyte = 5;
                return 0;
            }
            raw.buff[raw.nbyte++] = data;

            if (raw.nbyte < raw.len)
            {
                return 0;
            }

            /* decode javad raw message */
            stat = GlobalMembersJavad.decode_javad(raw);

            GlobalMembersJavad.clearbuff(raw);
            return stat;
        }
        /* start input file ----------------------------------------------------------*/
        internal static void startfile(raw_t raw)
        {
            raw.tod = -1;
            raw.obuf.n = 0;
            raw.buff[4] = (byte)'\n';
        }
        /* end input file ------------------------------------------------------------*/
        internal static int endfile(raw_t raw)
        {
            /* flush observation data buffer */
            if (GlobalMembersJavad.flushobuf(raw) == 0)
            {
                return -2;
            }
            raw.obuf.n = 0;
            return 1;
        }
        /* input javad raw message from file -------------------------------------------
        * fetch next javad raw data and input a message from file
        * args   : raw_t  *raw   IO     receiver raw data control struct
        *          FILE   *fp    I      file pointer
        * return : status(-2: end of file, -1...9: same as above)
        *-----------------------------------------------------------------------------*/
        public static int input_javadf(raw_t raw, FILE fp)
        {
            int i;
            int data;
            int len;
            int stat;

            GlobalMembersRtkcmn.trace(4, "input_javadf:\n");

            /* start input file */
            if (raw.flag != 0)
            {
                GlobalMembersJavad.startfile(raw);
                raw.flag = 0;
            }
            /* synchronize message */
            if (raw.nbyte == 0)
            {
                for (i = 0; ; i++)
                {
                    if ((data = fgetc(fp)) == EOF)
                    {
                        return GlobalMembersJavad.endfile(raw);
                    }
                    if (GlobalMembersJavad.sync_javad(raw.buff, (byte)data) != 0)
                        break;
                    if (i >= 4096)
                    {
                        return 0;
                    }
                }
            }
            if ((len = GlobalMembersJavad.decodelen(raw.buff + 2)) == 0 || len > DefineConstants.MAXRAWLEN - 5)
            {
                GlobalMembersRtkcmn.trace(2, "javad message length error: len=%3.3s\n", raw.buff + 2);
                GlobalMembersJavad.clearbuff(raw);
                return -1;
            }
            raw.len = len + 5;
            raw.nbyte = 5;

            if (fread(raw.buff + 5, 1, raw.len - 5, fp) < (uint)(raw.len - 5))
            {
                return GlobalMembersJavad.endfile(raw);
            }
            /* decode javad raw message */
            stat = GlobalMembersJavad.decode_javad(raw);

            GlobalMembersJavad.clearbuff(raw);
            return stat;
        }
    }
}
