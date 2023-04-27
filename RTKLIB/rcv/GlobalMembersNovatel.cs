using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ghGPS.Classes.rcv
{
    public static class GlobalMembersNovatel
    {
        /*------------------------------------------------------------------------------
        * notvatel.c : NovAtel OEM6/OEM5/OEM4/OEM3 receiver functions
        *
        *          Copyright (C) 2007-2014 by T.TAKASU, All rights reserved.
        *
        * reference :
        *     [1] NovAtel, OM-20000094 Rev6 OEMV Family Firmware Reference Manual, 2008
        *     [2] NovAtel, OM-20000053 Rev2 MiLLennium GPSCard Software Versions 4.503
        *         and 4.52 Command Descriptions Manual, 2001
        *     [3] NovAtel, OM-20000129 Rev2 OEM6 Family Firmware Reference Manual, 2011
        *     [4] NovAtel, OM-20000127 Rev1 OEMStar Firmware Reference Manual, 2009
        *     [5] NovAtel, OM-20000129 Rev6 OEM6 Family Firmware Reference Manual, 2014
        *
        * version : $Revision: 1.2 $ $Date: 2008/07/14 00:05:05 $
        * history : 2007/10/08 1.0 new
        *           2008/05/09 1.1 fix bug lli flag outage
        *           2008/06/16 1.2 separate common functions to rcvcmn.c
        *           2009/04/01 1.3 add prn number check for raw obs data
        *           2009/04/10 1.4 refactored
        *                          add oem3, oem4 rangeb support
        *           2009/06/06 1.5 fix bug on numerical exception with illegal snr
        *                          support oem3 regd message
        *           2009/12/09 1.6 support oem4 gloephemerisb message
        *                          invalid if parity unknown in GLONASS range
        *                          fix bug of dopper polarity inversion for oem3 regd
        *           2010/04/29 1.7 add tod field in geph_t
        *           2011/05/27 1.8 support RAWALM for oem4/v
        *                          add almanac decoding
        *                          add -EPHALL option
        *                          fix problem on ARM compiler
        *           2012/05/02 1.9 support OEM6,L5,QZSS
        *           2012/10/18 1.10 change obs codes
        *                           support Galileo
        *                           support rawsbasframeb,galephemerisb,galalmanacb,
        *                           galclockb,galionob
        *           2012/11/08 1.11 support galfnavrawpageb, galinavrawword
        *           2012/11/19 1.12 fix bug on decodeing rangeb
        *           2013/02/23 1.13 fix memory access violation problem on arm
        *           2013/03/28 1.14 fix invalid phase if glonass wavelen unavailable
        *           2013/06/02 1.15 fix bug on reading galephemrisb,galalmanacb,
        *                           galclockb,galionob
        *                           fix bug on decoding rawwaasframeb for qzss-saif
        *           2014/05/24 1.16 support beidou
        *           2014/07/01 1.17 fix problem on decoding of bdsephemerisb
        *                           fix bug on beidou tracking codes
        *           2014/10/20 1.11 fix bug on receiver option -GL*,-RL*,-EL*
        *-----------------------------------------------------------------------------*/

        /* get fields (little-endian) ------------------------------------------------*/
        //C++ TO C# CONVERTER NOTE: The following #define macro was replaced in-line:
        //ORIGINAL LINE: #define U1(p) (*((unsigned char *)(p)))

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
        internal static int I4(ref byte p)
        {
            int i;
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'memcpy' has no equivalent in C#:
            memcpy(i, p, 4);
            return i;
        }
        internal static float R4(ref byte p)
        {
            float r;
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'memcpy' has no equivalent in C#:
            memcpy(r, p, 4);
            return r;
        }
        internal static double R8(ref byte p)
        {
            double r;
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'memcpy' has no equivalent in C#:
            memcpy(r, p, 8);
            return r;
        }

        /* extend sign ---------------------------------------------------------------*/
        internal static int exsign(uint v, int bits)
        {
            return (int)((v & (1 << (bits - 1))) != 0 ? v | (~0u << bits) : v);
        }
        /* checksum ------------------------------------------------------------------*/
        internal static byte chksum(byte[] buff, int len)
        {
            byte sum = 0;
            int i;
            for (i = 0; i < len; i++)
            {
                sum ^= buff[i];
            }
            return sum;
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
        /* get observation data index ------------------------------------------------*/
        internal static int obsindex(obs_t obs, gtime_t time, int sat)
        {
            int i;
            int j;

            if (obs.n >= DefineConstants.MAXOBS)
            {
                return -1;
            }
            for (i = 0; i < obs.n; i++)
            {
                if (obs.data[i].sat == sat)
                {
                    return i;
                }
            }
            obs.data[i].time = time;
            obs.data[i].sat = sat;
            for (j = 0; j < DefineConstants.NFREQ + DefineConstants.NEXOBS; j++)
            {
                obs.data[i].L[j] = obs.data[i].P[j] = 0.0;
                obs.data[i].D[j] = 0.0;
                obs.data[i].SNR[j] = obs.data[i].LLI[j] = 0;
                obs.data[i].code[j] = DefineConstants.CODE_NONE;
            }
            obs.n++;
            return i;
        }
        /* ura value (m) to ura index ------------------------------------------------*/
        internal static int uraindex(double value)
        {
            double[] ura_eph = { 2.4, 3.4, 4.85, 6.85, 9.65, 13.65, 24.0, 48.0, 96.0, 192.0, 384.0, 768.0, 1536.0, 3072.0, 6144.0, 0.0 };
            int i;
            for (i = 0; i < 15; i++)
            {
                if (ura_eph[i] >= value)
                    break;
            }
            return i;
        }
        /* decode oem4 tracking status -------------------------------------------------
        * deocode oem4 tracking status
        * args   : unsigned int stat I  tracking status field
        *          int    *sys   O      system (SYS_???)
        *          int    *code  O      signal code (CODE_L??)
        *          int    *track O      tracking state
        *                         (oem4/5)
        *                         0=L1 idle                   8=L2 idle
        *                         1=L1 sky search             9=L2 p-code align
        *                         2=L1 wide freq pull-in     10=L2 search
        *                         3=L1 narrow freq pull-in   11=L2 pll
        *                         4=L1 pll                   12=L2 steering
        *                         5=L1 reacq
        *                         6=L1 steering
        *                         7=L1 fll
        *                         (oem6)
        *                         0=idle                      7=freq-lock loop
        *                         2=wide freq band pull-in    9=channel alignment
        *                         3=narrow freq band pull-in 10=code search
        *                         4=phase lock loop          11=aided phase lock loop
        *          int    *plock O      phase-lock flag   (0=not locked, 1=locked)
        *          int    *clock O      code-lock flag    (0=not locked, 1=locked)
        *          int    *parity O     parity known flag (0=not known,  1=known)
        *          int    *halfc O      phase measurement (0=half-cycle not added,
        *                                                  1=added)
        * return : signal frequency (0:L1,1:L2,2:L5,3:L6,4:L7,5:L8,-1:error)
        * notes  : refer [1][3]
        *-----------------------------------------------------------------------------*/
        internal static int decode_trackstat(uint stat, ref int sys, ref int code, ref int track, ref int plock, ref int clock, ref int parity, ref int halfc)
        {
            int satsys;
            int sigtype;
            int freq = 0;

            track = stat & 0x1F;
            plock = (stat >> 10) & 1;
            parity = (stat >> 11) & 1;
            clock = (stat >> 12) & 1;
            satsys = (stat >> 16) & 7;
            halfc = (stat >> 28) & 1;
            sigtype = (stat >> 21) & 0x1F;

            switch (satsys)
            {
                case 0:
                    sys = DefineConstants.SYS_GPS;
                    break;
                case 1:
                    sys = DefineConstants.SYS_GLO;
                    break;
                case 2:
                    sys = DefineConstants.SYS_SBS;
                    break;
                case 3: // OEM6
                    sys = DefineConstants.SYS_GAL;
                    break;
                case 4: // OEM6 F/W 6.400
                    sys = DefineConstants.SYS_CMP;
                    break;
                case 5: // OEM6
                    sys = DefineConstants.SYS_QZS;
                    break;
                default:
                    GlobalMembersRtkcmn.trace(2, "oem4 unknown system: sys=%d\n", satsys);
                    return -1;
            }
            if (sys == DefineConstants.SYS_GPS || sys == DefineConstants.SYS_QZS)
            {
                switch (sigtype)
                {
                    case 0: // L1C/A
                        freq = 0;
                        code = DefineConstants.CODE_L1C;
                        break;
                    case 5: // L1P
                        freq = 0;
                        code = DefineConstants.CODE_L1P;
                        break;
                    case 9: // L2Pcodeless
                        freq = 1;
                        code = DefineConstants.CODE_L2D;
                        break;
                    case 14: // L5Q (OEM6)
                        freq = 2;
                        code = DefineConstants.CODE_L5Q;
                        break;
                    case 17: // L2C(M+L)
                        freq = 1;
                        code = DefineConstants.CODE_L2X;
                        break;
                    default:
                        freq = -1;
                        break;
                }
            }
            else if (sys == DefineConstants.SYS_GLO)
            {
                switch (sigtype)
                {
                    case 0: // L1C/A
                        freq = 0;
                        code = DefineConstants.CODE_L1C;
                        break;
                    case 1: // L2C/A (OEM6)
                        freq = 1;
                        code = DefineConstants.CODE_L2C;
                        break;
                    case 5: // L2P
                        freq = 1;
                        code = DefineConstants.CODE_L2P;
                        break;
                    default:
                        freq = -1;
                        break;
                }
            }
            else if (sys == DefineConstants.SYS_GAL)
            {
                switch (sigtype)
                {
                    case 1: // E1B  (OEM6)
                        freq = 0;
                        code = DefineConstants.CODE_L1B;
                        break;
                    case 2: // E1C  (OEM6)
                        freq = 0;
                        code = DefineConstants.CODE_L1C;
                        break;
                    case 12: // E5aQ (OEM6)
                        freq = 2;
                        code = DefineConstants.CODE_L5Q;
                        break;
                    case 17: // E5bQ (OEM6)
                        freq = 4;
                        code = DefineConstants.CODE_L7Q;
                        break;
                    case 20: // AltBOCQ (OEM6)
                        freq = 5;
                        code = DefineConstants.CODE_L8Q;
                        break;
                    default:
                        freq = -1;
                        break;
                }
            }
            else if (sys == DefineConstants.SYS_CMP)
            {
                switch (sigtype)
                {
                    case 0: // B1 with D1 (OEM6)
                        freq = 0;
                        code = DefineConstants.CODE_L1I;
                        break;
                    case 1: // B2 with D1 (OEM6)
                        freq = 1;
                        code = DefineConstants.CODE_L7I;
                        break;
                    case 4: // B1 with D2 (OEM6)
                        freq = 0;
                        code = DefineConstants.CODE_L1I;
                        break;
                    case 5: // B2 with D2 (OEM6)
                        freq = 1;
                        code = DefineConstants.CODE_L7I;
                        break;
                    default:
                        freq = -1;
                        break;
                }
            }
            else if (sys == DefineConstants.SYS_SBS)
            {
                switch (sigtype)
                {
                    case 0: // L1C/A
                        freq = 0;
                        code = DefineConstants.CODE_L1C;
                        break;
                    case 6: // L5I (OEM6)
                        freq = 2;
                        code = DefineConstants.CODE_L5I;
                        break;
                    default:
                        freq = -1;
                        break;
                }
            }
            if (freq < 0)
            {
                GlobalMembersRtkcmn.trace(2, "oem4 signal type error: sys=%d sigtype=%d\n", sys, sigtype);
                return -1;
            }
            return freq;
        }
        /* check code priority and return obs position -------------------------------*/
        internal static int checkpri(string opt, int sys, int code, int freq)
        {
            int nex = DefineConstants.NEXOBS; // number of extended obs data

            if (sys == DefineConstants.SYS_GPS)
            {
                if (StringFunctions.StrStr(opt, "-GL1P") && freq == 0)
                {
                    return code == DefineConstants.CODE_L1P ? 0 : -1;
                }
                if (StringFunctions.StrStr(opt, "-GL2X") && freq == 1)
                {
                    return code == DefineConstants.CODE_L2X ? 1 : -1;
                }
                if (code == DefineConstants.CODE_L1P)
                {
                    return nex < 1 ? -1 : DefineConstants.NFREQ;
                }
                if (code == DefineConstants.CODE_L2X)
                {
                    return nex < 2 ? -1 : DefineConstants.NFREQ + 1;
                }
            }
            else if (sys == DefineConstants.SYS_GLO)
            {
                if (StringFunctions.StrStr(opt, "-RL2C") && freq == 1)
                {
                    return code == DefineConstants.CODE_L2C ? 1 : -1;
                }
                if (code == DefineConstants.CODE_L2C)
                {
                    return nex < 1 ? -1 : DefineConstants.NFREQ;
                }
            }
            else if (sys == DefineConstants.SYS_GAL)
            {
                if (StringFunctions.StrStr(opt, "-EL1B") && freq == 0)
                {
                    return code == DefineConstants.CODE_L1B ? 0 : -1;
                }
                if (code == DefineConstants.CODE_L1B)
                {
                    return nex < 1 ? -1 : DefineConstants.NFREQ;
                }
                if (code == DefineConstants.CODE_L7Q)
                {
                    return nex < 2 ? -1 : DefineConstants.NFREQ + 1;
                }
                if (code == DefineConstants.CODE_L8Q)
                {
                    return nex < 3 ? -1 : DefineConstants.NFREQ + 2;
                }
            }
            return freq < DefineConstants.NFREQ ? freq : -1;
        }
        /* decode rangecmpb ----------------------------------------------------------*/
        internal static int decode_rangecmpb(raw_t raw)
        {
            double psr;
            double adr;
            double adr_rolls;
            double lockt;
            double tt;
            double dop;
            double snr;
            double wavelen;
            int i;
            int index;
            int nobs;
            int prn;
            int sat;
            int sys;
            int code;
            int freq;
            int pos;
            int track;
            int plock;
            int clock;
            int parity;
            int halfc;
            int lli;
            string msg;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: byte *p=raw->buff+DefineConstants.OEM4HLEN;
            byte p = raw.buff + DefineConstants.OEM4HLEN;

            GlobalMembersRtkcmn.trace(3, "decode_rangecmpb: len=%d\n", raw.len);

            nobs = GlobalMembersBinex.U4(p);

            if (raw.outtype != 0)
            {
                msg = raw.msgtype.Substring(raw.msgtype.Length);
                msg = string.Format(" nobs={0,2:D}", nobs);
            }
            if (raw.len < DefineConstants.OEM4HLEN + 4 + nobs * 24)
            {
                GlobalMembersRtkcmn.trace(2, "oem4 rangecmpb length error: len=%d nobs=%d\n", raw.len, nobs);
                return -1;
            }
            for (i = 0, p += 4; i < nobs; i++, p += 24)
            {

                /* decode tracking status */
                if ((freq = GlobalMembersNovatel.decode_trackstat(GlobalMembersBinex.U4(p), ref sys, ref code, ref track, ref plock, ref clock, ref parity, ref halfc)) < 0)
                    continue;

                /* obs position */
                if ((pos = GlobalMembersJavad.checkpri(raw.opt, sys, code, freq)) < 0)
                    continue;

                prn = ((byte)(p + 17));
                if (sys == DefineConstants.SYS_GLO)
                {
                    prn -= 37;
                }

                if ((sat = GlobalMembersRtkcmn.satno(sys, prn)) == 0)
                {
                    GlobalMembersRtkcmn.trace(3, "oem4 rangecmpb satellite number error: sys=%d,prn=%d\n", sys, prn);
                    continue;
                }
                if (sys == DefineConstants.SYS_GLO && parity == 0) // invalid if GLO parity unknown
                    continue;

                dop = GlobalMembersNovatel.exsign(GlobalMembersBinex.U4(p + 4) & 0xFFFFFFF, 28) / 256.0;
                psr = (GlobalMembersBinex.U4(p + 7) >> 4) / 128.0 + ((byte)(p + 11)) * 2097152.0;

                if ((wavelen = GlobalMembersRtkcmn.satwavelen(sat, freq, raw.nav)) <= 0.0)
                {
                    if (sys == DefineConstants.SYS_GLO)
                    {
                        wavelen = DefineConstants.CLIGHT / (freq == 0 ? DefineConstants.FREQ1_GLO : DefineConstants.FREQ2_GLO);
                    }
                    else
                    {
                        wavelen = GlobalMembersRtkcmn.lam_carr[freq];
                    }
                }
                adr = GlobalMembersBinex.I4(ref p + 12) / 256.0;
                adr_rolls = (psr / wavelen + adr) / DefineConstants.MAXVAL;
                adr = -adr + DefineConstants.MAXVAL * Math.Floor(adr_rolls + (adr_rolls <= 0 ? -0.5 : 0.5));

                lockt = (GlobalMembersBinex.U4(p + 18) & 0x1FFFFF) / 32.0; // lock time

                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: tt=timediff(raw->time,raw->tobs);
                tt = GlobalMembersRtkcmn.timediff(new gtime_t(raw.time), new gtime_t(raw.tobs));
                if (raw.tobs.time != 0)
                {
                    lli = (lockt < 65535.968 && lockt - raw.lockt[sat - 1, pos] + 0.05 <= tt) || halfc != raw.halfc[sat - 1, pos];
                }
                else
                {
                    lli = 0;
                }
                if (parity == 0)
                {
                    lli |= 2;
                }
                raw.lockt[sat - 1, pos] = lockt;
                raw.halfc[sat - 1, pos] = halfc;

                snr = ((GlobalMembersBinex.U2(p + 20) & 0x3FF) >> 5) + 20.0;
                if (clock == 0) // code unlock
                {
                    psr = 0.0;
                }
                if (plock == 0) // phase unlock
                {
                    adr = dop = 0.0;
                }

                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: if (fabs(timediff(raw->obs.data[0].time,raw->time))>1E-9)
                if (Math.Abs(GlobalMembersRtkcmn.timediff(raw.obs.data[0].time, new gtime_t(raw.time))) > 1E-9)
                {
                    raw.obs.n = 0;
                }
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: if ((index=obsindex(&raw->obs,raw->time,sat))>=0)
                if ((index = GlobalMembersNovatel.obsindex(raw.obs, new gtime_t(raw.time), sat)) >= 0)
                {
                    raw.obs.data[index].L[pos] = adr;
                    raw.obs.data[index].P[pos] = psr;
                    raw.obs.data[index].D[pos] = (float)dop;
                    raw.obs.data[index].SNR[pos] = 0.0 <= snr && snr < 255.0 ? (byte)(snr * 4.0 + 0.5) : 0;
                    raw.obs.data[index].LLI[pos] = (byte)lli;
                    raw.obs.data[index].code[pos] = code;
#if false
	// /* L2C phase shift correction (L2C->L2P) */
	//            if (code==CODE_L2X) {
	//                raw->obs.data[index].L[pos]+=0.25;
	//                trace(3,"oem4 L2C phase shift corrected: prn=%2d\n",prn);
	//            }
#endif
                }
            }
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: raw->tobs=raw->time;
            raw.tobs.CopyFrom(raw.time);
            return 1;
        }
        /* decode rangeb -------------------------------------------------------------*/
        internal static int decode_rangeb(raw_t raw)
        {
            double psr;
            double adr;
            double dop;
            double snr;
            double lockt;
            double tt;
            string msg;
            int i;
            int index;
            int nobs;
            int prn;
            int sat;
            int sys;
            int code;
            int freq;
            int pos;
            int track;
            int plock;
            int clock;
            int parity;
            int halfc;
            int lli;
            int gfrq;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: byte *p=raw->buff+DefineConstants.OEM4HLEN;
            byte p = raw.buff + DefineConstants.OEM4HLEN;

            GlobalMembersRtkcmn.trace(3, "decode_rangeb: len=%d\n", raw.len);

            nobs = GlobalMembersBinex.U4(p);

            if (raw.outtype != 0)
            {
                msg = raw.msgtype.Substring(raw.msgtype.Length);
                msg = string.Format(" nobs={0,2:D}", nobs);
            }
            if (raw.len < DefineConstants.OEM4HLEN + 4 + nobs * 44)
            {
                GlobalMembersRtkcmn.trace(2, "oem4 rangeb length error: len=%d nobs=%d\n", raw.len, nobs);
                return -1;
            }
            for (i = 0, p += 4; i < nobs; i++, p += 44)
            {

                /* decode tracking status */
                if ((freq = GlobalMembersNovatel.decode_trackstat(GlobalMembersBinex.U4(p + 40), ref sys, ref code, ref track, ref plock, ref clock, ref parity, ref halfc)) < 0)
                    continue;

                /* obs position */
                if ((pos = GlobalMembersJavad.checkpri(raw.opt, sys, code, freq)) < 0)
                    continue;

                prn = GlobalMembersBinex.U2(p);
                if (sys == DefineConstants.SYS_GLO)
                {
                    prn -= 37;
                }

                if ((sat = GlobalMembersRtkcmn.satno(sys, prn)) == 0)
                {
                    GlobalMembersRtkcmn.trace(3, "oem4 rangeb satellite number error: sys=%d,prn=%d\n", sys, prn);
                    continue;
                }
                if (sys == DefineConstants.SYS_GLO && parity == 0) // invalid if GLO parity unknown
                    continue;

                gfrq = GlobalMembersBinex.U2(p + 2);
                psr = GlobalMembersBinex.R8(p + 4);
                adr = GlobalMembersBinex.R8(p + 16);
                dop = GlobalMembersBinex.R4(p + 28);
                snr = GlobalMembersBinex.R4(p + 32);
                lockt = GlobalMembersBinex.R4(p + 36);

                /* set glonass frequency channel number */
                if (sys == DefineConstants.SYS_GLO && raw.nav.geph[prn - 1].sat != sat)
                {
                    raw.nav.geph[prn - 1].frq = gfrq - 7;
                }
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: tt=timediff(raw->time,raw->tobs);
                tt = GlobalMembersRtkcmn.timediff(new gtime_t(raw.time), new gtime_t(raw.tobs));
                if (raw.tobs.time != 0)
                {
                    lli = lockt - raw.lockt[sat - 1, pos] + 0.05 <= tt || halfc != raw.halfc[sat - 1, pos];
                }
                else
                {
                    lli = 0;
                }
                if (parity == 0)
                {
                    lli |= 2;
                }
                raw.lockt[sat - 1, pos] = lockt;
                raw.halfc[sat - 1, pos] = halfc;
                if (clock == 0) // code unlock
                {
                    psr = 0.0;
                }
                if (plock == 0) // phase unlock
                {
                    adr = dop = 0.0;
                }

                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: if (fabs(timediff(raw->obs.data[0].time,raw->time))>1E-9)
                if (Math.Abs(GlobalMembersRtkcmn.timediff(raw.obs.data[0].time, new gtime_t(raw.time))) > 1E-9)
                {
                    raw.obs.n = 0;
                }
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: if ((index=obsindex(&raw->obs,raw->time,sat))>=0)
                if ((index = GlobalMembersNovatel.obsindex(raw.obs, new gtime_t(raw.time), sat)) >= 0)
                {
                    raw.obs.data[index].L[pos] = -adr;
                    raw.obs.data[index].P[pos] = psr;
                    raw.obs.data[index].D[pos] = (float)dop;
                    raw.obs.data[index].SNR[pos] = 0.0 <= snr && snr < 255.0 ? (byte)(snr * 4.0 + 0.5) : 0;
                    raw.obs.data[index].LLI[pos] = (byte)lli;
                    raw.obs.data[index].code[pos] = code;
#if false
	// /* L2C phase shift correction */
	//            if (code==CODE_L2X) {
	//                raw->obs.data[index].L[pos]+=0.25;
	//                trace(3,"oem4 L2C phase shift corrected: prn=%2d\n",prn);
	//            }
#endif
                }
            }
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: raw->tobs=raw->time;
            raw.tobs.CopyFrom(raw.time);
            return 1;
        }
        /* decode rawephemb ----------------------------------------------------------*/
        internal static int decode_rawephemb(raw_t raw)
        {
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: byte *p=raw->buff+DefineConstants.OEM4HLEN;
            byte p = raw.buff + DefineConstants.OEM4HLEN;
            eph_t eph = new eph_t();
            int prn;
            int sat;

            GlobalMembersRtkcmn.trace(3, "decode_rawephemb: len=%d\n", raw.len);

            if (raw.len < DefineConstants.OEM4HLEN + 102)
            {
                GlobalMembersRtkcmn.trace(2, "oem4 rawephemb length error: len=%d\n", raw.len);
                return -1;
            }
            prn = GlobalMembersBinex.U4(p);
            if ((sat = GlobalMembersRtkcmn.satno(DefineConstants.SYS_GPS, prn)) == 0)
            {
                GlobalMembersRtkcmn.trace(2, "oem4 rawephemb satellite number error: prn=%d\n", prn);
                return -1;
            }
            if (GlobalMembersRcvraw.decode_frame(p + 12, eph, null, null, null, null) != 1 || GlobalMembersRcvraw.decode_frame(p + 42, eph, null, null, null, null) != 2 || GlobalMembersRcvraw.decode_frame(p + 72, eph, null, null, null, null) != 3)
            {
                GlobalMembersRtkcmn.trace(2, "oem4 rawephemb subframe error: prn=%d\n", prn);
                return -1;
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
            GlobalMembersRtkcmn.trace(4, "decode_rawephemb: sat=%2d\n", sat);
            return 2;
        }
        /* decode ionutcb ------------------------------------------------------------*/
        internal static int decode_ionutcb(raw_t raw)
        {
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: byte *p=raw->buff+DefineConstants.OEM4HLEN;
            byte p = raw.buff + DefineConstants.OEM4HLEN;
            int i;

            GlobalMembersRtkcmn.trace(3, "decode_ionutcb: len=%d\n", raw.len);

            if (raw.len < DefineConstants.OEM4HLEN + 108)
            {
                GlobalMembersRtkcmn.trace(2, "oem4 ionutcb length error: len=%d\n", raw.len);
                return -1;
            }
            for (i = 0; i < 8; i++)
            {
                raw.nav.ion_gps[i] = GlobalMembersBinex.R8(p + i * 8);
            }
            raw.nav.utc_gps[0] = GlobalMembersBinex.R8(p + 72);
            raw.nav.utc_gps[1] = GlobalMembersBinex.R8(p + 80);
            raw.nav.utc_gps[2] = GlobalMembersBinex.U4(p + 68);
            raw.nav.utc_gps[3] = GlobalMembersBinex.U4(p + 64);
            raw.nav.leaps = GlobalMembersBinex.I4(ref p + 96);
            return 9;
        }
        /* decode rawwaasframeb ------------------------------------------------------*/
        internal static int decode_rawwaasframeb(raw_t raw)
        {
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: byte *p=raw->buff+DefineConstants.OEM4HLEN;
            byte p = raw.buff + DefineConstants.OEM4HLEN;
            int i;
            int prn;

            GlobalMembersRtkcmn.trace(3, "decode_rawwaasframeb: len=%d\n", raw.len);

            if (raw.len < DefineConstants.OEM4HLEN + 48)
            {
                GlobalMembersRtkcmn.trace(2, "oem4 rawwaasframeb length error: len=%d\n", raw.len);
                return -1;
            }
            prn = GlobalMembersBinex.U4(p + 4);

            if (DefineConstants.MINPRNQZS_S <= prn && prn <= DefineConstants.MAXPRNQZS_S)
            {
                prn += 10; // QZSS SAIF PRN -> QZSS PRN
            }
            else if (prn < DefineConstants.MINPRNSBS || DefineConstants.MAXPRNSBS < prn)
            {
                return 0;
            }

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: raw->sbsmsg.tow=(int)time2gpst(raw->time,&raw->sbsmsg.week);
            raw.sbsmsg.tow = (int)GlobalMembersRtkcmn.time2gpst(new gtime_t(raw.time), ref raw.sbsmsg.week);
            raw.sbsmsg.prn = prn;
            for (i = 0, p += 12; i < 29; i++, p++)
            {
                raw.sbsmsg.msg[i] = p;
            }
            return 3;
        }
        /* decode rawsbasframeb ------------------------------------------------------*/
        internal static int decode_rawsbasframeb(raw_t raw)
        {
            GlobalMembersRtkcmn.trace(3, "decode_rawsbasframeb: len=%d\n", raw.len);

            /* format same as rawwaasframeb */
            return GlobalMembersNovatel.decode_rawwaasframeb(raw);
        }
        /* decode gloephemerisb ------------------------------------------------------*/
        internal static int decode_gloephemerisb(raw_t raw)
        {
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: byte *p=raw->buff+DefineConstants.OEM4HLEN;
            byte p = raw.buff + DefineConstants.OEM4HLEN;
            geph_t geph = new geph_t();
            string msg;
            double tow;
            double tof;
            double toff;
            int prn;
            int sat;
            int week;

            GlobalMembersRtkcmn.trace(3, "decode_gloephemerisb: len=%d\n", raw.len);

            if (raw.len < DefineConstants.OEM4HLEN + 144)
            {
                GlobalMembersRtkcmn.trace(2, "oem4 gloephemerisb length error: len=%d\n", raw.len);
                return -1;
            }
            prn = GlobalMembersBinex.U2(p) - 37;

            if (raw.outtype != 0)
            {
                msg = raw.msgtype.Substring(raw.msgtype.Length);
                msg = string.Format(" prn={0,3:D}", prn);
            }
            if ((sat = GlobalMembersRtkcmn.satno(DefineConstants.SYS_GLO, prn)) == 0)
            {
                GlobalMembersRtkcmn.trace(2, "oem4 gloephemerisb prn error: prn=%d\n", prn);
                return -1;
            }
            geph.frq = GlobalMembersBinex.U2(p + 2) + DefineConstants.OFF_FRQNO;
            week = GlobalMembersBinex.U2(p + 6);
            tow = Math.Floor(GlobalMembersBinex.U4(p + 8) / 1000.0 + 0.5); // rounded to integer sec
            toff = GlobalMembersBinex.U4(p + 12);
            geph.iode = GlobalMembersBinex.U4(p + 20) & 0x7F;
            geph.svh = GlobalMembersBinex.U4(p + 24);
            geph.pos[0] = GlobalMembersBinex.R8(p + 28);
            geph.pos[1] = GlobalMembersBinex.R8(p + 36);
            geph.pos[2] = GlobalMembersBinex.R8(p + 44);
            geph.vel[0] = GlobalMembersBinex.R8(p + 52);
            geph.vel[1] = GlobalMembersBinex.R8(p + 60);
            geph.vel[2] = GlobalMembersBinex.R8(p + 68);
            geph.acc[0] = GlobalMembersBinex.R8(p + 76);
            geph.acc[1] = GlobalMembersBinex.R8(p + 84);
            geph.acc[2] = GlobalMembersBinex.R8(p + 92);
            geph.taun = GlobalMembersBinex.R8(p + 100);
            geph.gamn = GlobalMembersBinex.R8(p + 116);
            tof = GlobalMembersBinex.U4(p + 124) - toff; // glonasst->gpst
            geph.age = GlobalMembersBinex.U4(p + 136);
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: geph.toe=gpst2time(week,tow);
            geph.toe.CopyFrom(GlobalMembersRtkcmn.gpst2time(week, tow));
            tof += Math.Floor(tow / 86400.0) * 86400;
            if (tof < tow - 43200.0)
            {
                tof += 86400.0;
            }
            else if (tof > tow + 43200.0)
            {
                tof -= 86400.0;
            }
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: geph.tof=gpst2time(week,tof);
            geph.tof.CopyFrom(GlobalMembersRtkcmn.gpst2time(week, tof));

            if (!StringFunctions.StrStr(raw.opt, "-EPHALL"))
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: if (fabs(timediff(geph.toe,raw->nav.geph[prn-1].toe))<1.0&& geph.svh==raw->nav.geph[prn-1].svh)
                if (Math.Abs(GlobalMembersRtkcmn.timediff(new gtime_t(geph.toe), raw.nav.geph[prn - 1].toe)) < 1.0 && geph.svh == raw.nav.geph[prn - 1].svh) // unchanged
                {
                    return 0;
                }
            }
            geph.sat = sat;
            raw.nav.geph[prn - 1] = geph;
            raw.ephsat = sat;
            return 2;
        }
        /* decode qzss rawephemb -----------------------------------------------------*/
        internal static int decode_qzssrawephemb(raw_t raw)
        {
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: byte *p=raw->buff+DefineConstants.OEM4HLEN,*q;
            byte p = raw.buff + DefineConstants.OEM4HLEN;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: byte *q;
            byte q;
            eph_t eph = new eph_t();
            string msg;
            int i;
            int prn;
            int id;
            int sat;

            GlobalMembersRtkcmn.trace(3, "decode_qzssrawephemb: len=%d\n", raw.len);

            if (raw.len < DefineConstants.OEM4HLEN + 44)
            {
                GlobalMembersRtkcmn.trace(2, "oem4 qzssrawephemb length error: len=%d\n", raw.len);
                return -1;
            }
            prn = GlobalMembersBinex.U4(p);
            id = GlobalMembersBinex.U4(p + 4);

            if (raw.outtype != 0)
            {
                msg = raw.msgtype.Substring(raw.msgtype.Length);
                msg = string.Format(" prn={0,3:D} id={1:D}", prn, id);
            }
            if ((sat = GlobalMembersRtkcmn.satno(DefineConstants.SYS_QZS, prn)) == 0)
            {
                GlobalMembersRtkcmn.trace(2, "oem4 qzssrawephemb satellite number error: prn=%d\n", prn);
                return -1;
            }
            if (id < 1 || 3 < id)
            {
                return 0;
            }

            q = raw.subfrm[sat - 1] + (id - 1) * 30;
            for (i = 0; i < 30; i++)
            {
                q++ = p[8 + i];
            }

            if (id < 3)
            {
                return 0;
            }
            if (GlobalMembersRcvraw.decode_frame(raw.subfrm[sat - 1], eph, null, null, null, null) != 1 || GlobalMembersRcvraw.decode_frame(raw.subfrm[sat - 1] + 30, eph, null, null, null, null) != 2 || GlobalMembersRcvraw.decode_frame(raw.subfrm[sat - 1] + 60, eph, null, null, null, null) != 3)
            {
                return 0;
            }
            if (!StringFunctions.StrStr(raw.opt, "-EPHALL"))
            {
                if (eph.iodc == raw.nav.eph[sat - 1].iodc && eph.iode == raw.nav.eph[sat - 1].iode) // unchanged
                {
                    return 0;
                }
            }
            eph.sat = sat;
            raw.nav.eph[sat - 1] = eph;
            raw.ephsat = sat;
            GlobalMembersRtkcmn.trace(4, "decode_qzssrawephemb: sat=%2d\n", sat);
            return 2;
        }
        /* decode qzss rawsubframeb --------------------------------------------------*/
        internal static int decode_qzssrawsubframeb(raw_t raw)
        {
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: byte *p=raw->buff+DefineConstants.OEM4HLEN;
            byte p = raw.buff + DefineConstants.OEM4HLEN;
            eph_t eph = new eph_t();
            string msg;
            int prn;
            int sat;

            GlobalMembersRtkcmn.trace(3, "decode_qzssrawsubframeb: len=%d\n", raw.len);

            if (raw.len < DefineConstants.OEM4HLEN + 44)
            {
                GlobalMembersRtkcmn.trace(2, "oem4 qzssrawsubframeb length error: len=%d\n", raw.len);
                return -1;
            }
            prn = GlobalMembersBinex.U4(p);

            if (raw.outtype != 0)
            {
                msg = raw.msgtype.Substring(raw.msgtype.Length);
                msg = string.Format(" prn={0,3:D}", prn);
            }
            if ((sat = GlobalMembersRtkcmn.satno(DefineConstants.SYS_QZS, prn)) == 0)
            {
                GlobalMembersRtkcmn.trace(2, "oem4 qzssrawephemb satellite number error: prn=%d\n", prn);
                return -1;
            }
            if (GlobalMembersRcvraw.decode_frame(p + 12, eph, null, null, null, null) != 1 || GlobalMembersRcvraw.decode_frame(p + 42, eph, null, null, null, null) != 2 || GlobalMembersRcvraw.decode_frame(p + 72, eph, null, null, null, null) != 3)
            {
                return 0;
            }
            if (!StringFunctions.StrStr(raw.opt, "-EPHALL"))
            {
                if (eph.iodc == raw.nav.eph[sat - 1].iodc && eph.iode == raw.nav.eph[sat - 1].iode) // unchanged
                {
                    return 0;
                }
            }
            eph.sat = sat;
            raw.nav.eph[sat - 1] = eph;
            raw.ephsat = sat;
            GlobalMembersRtkcmn.trace(4, "decode_qzssrawsubframeb: sat=%2d\n", sat);
            return 2;
        }
        /* decode qzssionutcb --------------------------------------------------------*/
        internal static int decode_qzssionutcb(raw_t raw)
        {
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: byte *p=raw->buff+DefineConstants.OEM4HLEN;
            byte p = raw.buff + DefineConstants.OEM4HLEN;
            int i;

            GlobalMembersRtkcmn.trace(3, "decode_qzssionutcb: len=%d\n", raw.len);

            if (raw.len < DefineConstants.OEM4HLEN + 108)
            {
                GlobalMembersRtkcmn.trace(2, "oem4 qzssionutcb length error: len=%d\n", raw.len);
                return -1;
            }
            for (i = 0; i < 8; i++)
            {
                raw.nav.ion_qzs[i] = GlobalMembersBinex.R8(p + i * 8);
            }
            raw.nav.utc_qzs[0] = GlobalMembersBinex.R8(p + 72);
            raw.nav.utc_qzs[1] = GlobalMembersBinex.R8(p + 80);
            raw.nav.utc_qzs[2] = GlobalMembersBinex.U4(p + 68);
            raw.nav.utc_qzs[3] = GlobalMembersBinex.U4(p + 64);
            raw.nav.leaps = GlobalMembersBinex.I4(ref p + 96);
            return 9;
        }
        /* decode galephemerisb ------------------------------------------------------*/
        internal static int decode_galephemerisb(raw_t raw)
        {
            eph_t eph = new eph_t();
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: byte *p=raw->buff+DefineConstants.OEM4HLEN;
            byte p = raw.buff + DefineConstants.OEM4HLEN;
            double tow;
            double sqrtA;
            double af0_fnav;
            double af1_fnav;
            double af2_fnav;
            double af0_inav;
            double af1_inav;
            double af2_inav;
            double tt;
            string msg;
            int prn;
            int rcv_fnav;
            int rcv_inav;
            int svh_e1b;
            int svh_e5a;
            int svh_e5b;
            int dvs_e1b;
            int dvs_e5a;
            int dvs_e5b;
            int toc_fnav;
            int toc_inav;
            int week;

            GlobalMembersRtkcmn.trace(3, "decode_galephemerisb: len=%d\n", raw.len);

            if (raw.len < DefineConstants.OEM4HLEN + 220)
            {
                GlobalMembersRtkcmn.trace(2, "oem4 galephemrisb length error: len=%d\n", raw.len);
                return -1;
            }
            prn = GlobalMembersBinex.U4(p);
            p += 4;
            rcv_fnav = GlobalMembersBinex.U4(p) & 1;
            p += 4;
            rcv_inav = GlobalMembersBinex.U4(p) & 1;
            p += 4;
            svh_e1b = ((byte)(p)) & 3;
            p += 1;
            svh_e5a = ((byte)(p)) & 3;
            p += 1;
            svh_e5b = ((byte)(p)) & 3;
            p += 1;
            dvs_e1b = ((byte)(p)) & 1;
            p += 1;
            dvs_e5a = ((byte)(p)) & 1;
            p += 1;
            dvs_e5b = ((byte)(p)) & 1;
            p += 1;
            eph.sva = ((byte)(p));
            p += 1 + 1;
            eph.iode = GlobalMembersBinex.U4(p); // IODNav
            p += 4;
            eph.toes = GlobalMembersBinex.U4(p);
            p += 4;
            sqrtA = GlobalMembersBinex.R8(p);
            p += 8;
            eph.deln = GlobalMembersBinex.R8(p);
            p += 8;
            eph.M0 = GlobalMembersBinex.R8(p);
            p += 8;
            eph.e = GlobalMembersBinex.R8(p);
            p += 8;
            eph.omg = GlobalMembersBinex.R8(p);
            p += 8;
            eph.cuc = GlobalMembersBinex.R8(p);
            p += 8;
            eph.cus = GlobalMembersBinex.R8(p);
            p += 8;
            eph.crc = GlobalMembersBinex.R8(p);
            p += 8;
            eph.crs = GlobalMembersBinex.R8(p);
            p += 8;
            eph.cic = GlobalMembersBinex.R8(p);
            p += 8;
            eph.cis = GlobalMembersBinex.R8(p);
            p += 8;
            eph.i0 = GlobalMembersBinex.R8(p);
            p += 8;
            eph.idot = GlobalMembersBinex.R8(p);
            p += 8;
            eph.OMG0 = GlobalMembersBinex.R8(p);
            p += 8;
            eph.OMGd = GlobalMembersBinex.R8(p);
            p += 8;
            toc_fnav = GlobalMembersBinex.U4(p);
            p += 4;
            af0_fnav = GlobalMembersBinex.R8(p);
            p += 8;
            af1_fnav = GlobalMembersBinex.R8(p);
            p += 8;
            af2_fnav = GlobalMembersBinex.R8(p);
            p += 8;
            toc_inav = GlobalMembersBinex.U4(p);
            p += 4;
            af0_inav = GlobalMembersBinex.R8(p);
            p += 8;
            af1_inav = GlobalMembersBinex.R8(p);
            p += 8;
            af2_inav = GlobalMembersBinex.R8(p);
            p += 8;
            eph.tgd[0] = GlobalMembersBinex.R8(p); // BGD: E5A-E1 (s)
            p += 8;
            eph.tgd[1] = GlobalMembersBinex.R8(p); // BGD: E5B-E1 (s)
            eph.iodc = eph.iode;
            eph.svh = (svh_e5b << 7) | (dvs_e5b << 6) | (svh_e5a << 4) | (dvs_e5a << 3) | (svh_e1b << 1) | dvs_e1b;
            eph.code = rcv_fnav != 0 ? 1 : 0; // 0:INAV,1:FNAV
            eph.A = sqrtA * sqrtA;
            eph.f0 = rcv_fnav != 0 ? af0_fnav : af0_inav;
            eph.f1 = rcv_fnav != 0 ? af1_fnav : af1_inav;
            eph.f2 = rcv_fnav != 0 ? af2_fnav : af2_inav;

            if (raw.outtype != 0)
            {
                msg = raw.msgtype.Substring(raw.msgtype.Length);
                msg = string.Format(" prn={0,3:D} iod={1,3:D} toes={2,6:f0}", prn, eph.iode, eph.toes);
            }
            if ((eph.sat = GlobalMembersRtkcmn.satno(DefineConstants.SYS_GAL, prn)) == 0)
            {
                GlobalMembersRtkcmn.trace(2, "oemv galephemeris satellite error: prn=%d\n", prn);
                return -1;
            }
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: tow=time2gpst(raw->time,&week);
            tow = GlobalMembersRtkcmn.time2gpst(new gtime_t(raw.time), ref week);
            eph.week = week; // gps week
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
            //ORIGINAL LINE: eph.toc=adjweek(eph.toe,rcv_fnav?toc_fnav:toc_inav);
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            eph.toc.CopyFrom(GlobalMembersBinex.adjweek(new gtime_t(eph.toe), rcv_fnav != 0 ? toc_fnav : toc_inav));
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: eph.ttr=adjweek(eph.toe,tow);
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            eph.ttr.CopyFrom(GlobalMembersBinex.adjweek(new gtime_t(eph.toe), tow));

            if (!StringFunctions.StrStr(raw.opt, "-EPHALL"))
            {
                if (raw.nav.eph[eph.sat - 1].iode == eph.iode && raw.nav.eph[eph.sat - 1].code == eph.code) // unchanged
                {
                    return 0;
                }
            }
            raw.nav.eph[eph.sat - 1] = eph;
            raw.ephsat = eph.sat;
            return 2;
        }
        /* decode galalmanacb --------------------------------------------------------*/
        internal static int decode_galalmanacb(raw_t raw)
        {
            alm_t alm = new alm_t();
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: byte *p=raw->buff+DefineConstants.OEM4HLEN;
            byte p = raw.buff + DefineConstants.OEM4HLEN;
            double dsqrtA;
            double sqrtA = Math.Sqrt(29601297.0);
            int prn;
            int rcv_fnav;
            int rcv_inav;
            int svh_e1b;
            int svh_e5a;
            int svh_e5b;
            int ioda;

            GlobalMembersRtkcmn.trace(3, "decode_galalmanacb: len=%d\n", raw.len);

            if (raw.len < DefineConstants.OEM4HLEN + 100)
            {
                GlobalMembersRtkcmn.trace(2, "oem4 galephemrisb length error: len=%d\n", raw.len);
                return -1;
            }
            prn = GlobalMembersBinex.U4(p);
            p += 4;
            rcv_fnav = GlobalMembersBinex.U4(p) & 1;
            p += 4;
            rcv_inav = GlobalMembersBinex.U4(p) & 1;
            p += 4;
            svh_e1b = ((byte)(p)) & 3;
            p += 1;
            svh_e5a = ((byte)(p)) & 3;
            p += 1;
            svh_e5b = ((byte)(p)) & 3;
            p += 1 + 1;
            ioda = GlobalMembersBinex.U4(p);
            p += 4;
            alm.week = GlobalMembersBinex.U4(p); // gst week
            p += 4;
            alm.toas = GlobalMembersBinex.U4(p);
            p += 4;
            alm.e = GlobalMembersBinex.R8(p);
            p += 8;
            alm.OMGd = GlobalMembersBinex.R8(p);
            p += 8;
            alm.OMG0 = GlobalMembersBinex.R8(p);
            p += 8;
            alm.omg = GlobalMembersBinex.R8(p);
            p += 8;
            alm.M0 = GlobalMembersBinex.R8(p);
            p += 8;
            alm.f0 = GlobalMembersBinex.R8(p);
            p += 8;
            alm.f1 = GlobalMembersBinex.R8(p);
            p += 8;
            dsqrtA = GlobalMembersBinex.R8(p);
            p += 8;
            alm.i0 = (GlobalMembersBinex.R8(p) + 56.0) * DefineConstants.PI / 180.0;
            alm.svh = (svh_e5b << 7) | (svh_e5a << 4) | (svh_e1b << 1);
            alm.A = (sqrtA + dsqrtA) * (sqrtA + dsqrtA);

            if ((alm.sat = GlobalMembersRtkcmn.satno(DefineConstants.SYS_GAL, prn)) == 0)
            {
                GlobalMembersRtkcmn.trace(2, "oemv galalmanac satellite error: prn=%d\n", prn);
                return -1;
            }
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: alm.toa=gst2time(alm.week,alm.toas);
            alm.toa.CopyFrom(GlobalMembersRtkcmn.gst2time(alm.week, alm.toas));
            raw.nav.alm[alm.sat - 1] = alm;
            return 0;
        }
        /* decode galclockb ----------------------------------------------------------*/
        internal static int decode_galclockb(raw_t raw)
        {
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: byte *p=raw->buff+DefineConstants.OEM4HLEN;
            byte p = raw.buff + DefineConstants.OEM4HLEN;
            double a0;
            double a1;
            double a0g;
            double a1g;
            int leaps;
            int tot;
            int wnt;
            int wnlsf;
            int dn;
            int dtlsf;
            int t0g;
            int wn0g;

            GlobalMembersRtkcmn.trace(3, "decode_galclockb: len=%d\n", raw.len);

            if (raw.len < DefineConstants.OEM4HLEN + 64)
            {
                GlobalMembersRtkcmn.trace(2, "oem4 galclockb length error: len=%d\n", raw.len);
                return -1;
            }
            a0 = GlobalMembersBinex.R8(p);
            p += 8;
            a1 = GlobalMembersBinex.R8(p);
            p += 8;
            leaps = GlobalMembersBinex.I4(ref p);
            p += 4;
            tot = GlobalMembersBinex.U4(p);
            p += 4;
            wnt = GlobalMembersBinex.U4(p);
            p += 4;
            wnlsf = GlobalMembersBinex.U4(p);
            p += 4;
            dn = GlobalMembersBinex.U4(p);
            p += 4;
            dtlsf = GlobalMembersBinex.U4(p);
            p += 4;
            a0g = GlobalMembersBinex.R8(p);
            p += 8;
            a1g = GlobalMembersBinex.R8(p);
            p += 8;
            t0g = GlobalMembersBinex.U4(p);
            p += 4;
            wn0g = GlobalMembersBinex.U4(p);

            raw.nav.utc_gal[0] = a0;
            raw.nav.utc_gal[1] = a1;
            raw.nav.utc_gal[2] = tot; // utc reference tow (s)
            raw.nav.utc_gal[3] = wnt; // utc reference week
            return 9;
        }
        /* decode galionob -----------------------------------------------------------*/
        internal static int decode_galionob(raw_t raw)
        {
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: byte *p=raw->buff+DefineConstants.OEM4HLEN;
            byte p = raw.buff + DefineConstants.OEM4HLEN;
            double[] ai = new double[3];
            int i;
            int[] sf = new int[5];

            GlobalMembersRtkcmn.trace(3, "decode_galionob: len=%d\n", raw.len);

            if (raw.len < DefineConstants.OEM4HLEN + 29)
            {
                GlobalMembersRtkcmn.trace(2, "oem4 galionob length error: len=%d\n", raw.len);
                return -1;
            }
            ai[0] = GlobalMembersBinex.R8(p);
            p += 8;
            ai[1] = GlobalMembersBinex.R8(p);
            p += 8;
            ai[2] = GlobalMembersBinex.R8(p);
            p += 8;
            sf[0] = ((byte)(p));
            p += 1;
            sf[1] = ((byte)(p));
            p += 1;
            sf[2] = ((byte)(p));
            p += 1;
            sf[3] = ((byte)(p));
            p += 1;
            sf[4] = ((byte)(p));

            for (i = 0; i < 3; i++)
            {
                raw.nav.ion_gal[i] = ai[i];
            }
            return 9;
        }
        /* decode galfnavrawpageb ----------------------------------------------------*/
        internal static int decode_galfnavrawpageb(raw_t raw)
        {
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: byte *p=raw->buff+DefineConstants.OEM4HLEN;
            byte p = raw.buff + DefineConstants.OEM4HLEN;
            byte[] buff = new byte[27];
            int i;
            int sigch;
            int satid;
            int page;

            GlobalMembersRtkcmn.trace(3, "decode_galfnavrawpageb: len=%d\n", raw.len);

            if (raw.len < DefineConstants.OEM4HLEN + 35)
            {
                GlobalMembersRtkcmn.trace(2, "oem4 galfnavrawpageb length error: len=%d\n", raw.len);
                return -1;
            }
            sigch = GlobalMembersBinex.U4(p);
            p += 4;
            satid = GlobalMembersBinex.U4(p);
            p += 4;
            for (i = 0; i < 27; i++)
            {
                buff[i] = ((byte)(p));
                p += 1;
            }
            page = GlobalMembersRtkcmn.getbitu(buff, 0, 6);

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: trace(3,"%s E%2d FNAV     (%2d) ",time_str(raw->time,0),satid,page);
            GlobalMembersRtkcmn.trace(3, "%s E%2d FNAV     (%2d) ", GlobalMembersRtkcmn.time_str(new gtime_t(raw.time), 0), satid, page);
            GlobalMembersRtkcmn.traceb(3, buff, 27);

            return 0;
        }
        /* decode galinavrawwordb ----------------------------------------------------*/
        internal static int decode_galinavrawwordb(raw_t raw)
        {
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: byte *p=raw->buff+DefineConstants.OEM4HLEN;
            byte p = raw.buff + DefineConstants.OEM4HLEN;
            byte[] buff = new byte[16];
            gtime_t time = raw.time;
            string sig;
            int i;
            int sigch;
            int satid;
            int sigtype;
            int type;
            int week = 0;
            int tow = 0;

            GlobalMembersRtkcmn.trace(3, "decode_galinavrawwordb: len=%d\n", raw.len);

            if (raw.len < DefineConstants.OEM4HLEN + 28)
            {
                GlobalMembersRtkcmn.trace(2, "oem4 galinavrawwordb length error: len=%d\n", raw.len);
                return -1;
            }
            sigch = GlobalMembersBinex.U4(p);
            p += 4;
            satid = GlobalMembersBinex.U4(p);
            p += 4;
            sigtype = GlobalMembersBinex.U4(p);
            p += 4;

            switch (sigtype)
            {
                case 10433:
                    sig = "E1 ";
                    break;
                case 10466:
                    sig = "E5A";
                    break;
                case 10499:
                    sig = "E5B";
                    break;
                default:
                    sig = "???";
                    break;
            }
            for (i = 0; i < 16; i++)
            {
                buff[i] = ((byte)(p));
                p += 1;
            }
            type = GlobalMembersRtkcmn.getbitu(buff, 0, 6);
            if (type == 0 && GlobalMembersRtkcmn.getbitu(buff, 6, 2) == 2)
            {
                week = GlobalMembersRtkcmn.getbitu(buff, 96, 12); // gst week
                tow = GlobalMembersRtkcmn.getbitu(buff, 108, 20);
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                //ORIGINAL LINE: time=gst2time(week,tow);
                time.CopyFrom(GlobalMembersRtkcmn.gst2time(week, tow));
            }
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: trace(3,"%s E%2d INAV-%s (%2d) ",time_str(time,0),satid,sig,type);
            GlobalMembersRtkcmn.trace(3, "%s E%2d INAV-%s (%2d) ", GlobalMembersRtkcmn.time_str(new gtime_t(time), 0), satid, sig, type);
            GlobalMembersRtkcmn.traceb(3, buff, 16);

            return 0;
        }
        /* decode rawcnavframeb ------------------------------------------------------*/
        internal static int decode_rawcnavframeb(raw_t raw)
        {
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: byte *p=raw->buff+DefineConstants.OEM4HLEN;
            byte p = raw.buff + DefineConstants.OEM4HLEN;
            byte[] buff = new byte[38];
            int i;
            int sigch;
            int prn;
            int frmid;

            GlobalMembersRtkcmn.trace(3, "decode_rawcnavframeb: len=%d\n", raw.len);

            if (raw.len < DefineConstants.OEM4HLEN + 50)
            {
                GlobalMembersRtkcmn.trace(2, "oem4 rawcnavframeb length error: len=%d\n", raw.len);
                return -1;
            }
            sigch = GlobalMembersBinex.U4(p);
            p += 4;
            prn = GlobalMembersBinex.U4(p);
            p += 4;
            frmid = GlobalMembersBinex.U4(p);
            p += 4;

            for (i = 0; i < 38; i++)
            {
                buff[i] = ((byte)(p));
                p += 1;
            }
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: trace(3,"%s PRN=%3d FRMID=%2d ",time_str(raw->time,0),prn,frmid);
            GlobalMembersRtkcmn.trace(3, "%s PRN=%3d FRMID=%2d ", GlobalMembersRtkcmn.time_str(new gtime_t(raw.time), 0), prn, frmid);
            GlobalMembersRtkcmn.traceb(3, buff, 38);

            return 0;
        }
        /* decode bdsephemerisb ------------------------------------------------------*/
        internal static int decode_bdsephemerisb(raw_t raw)
        {
            eph_t eph = new eph_t();
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: byte *p=raw->buff+DefineConstants.OEM4HLEN;
            byte p = raw.buff + DefineConstants.OEM4HLEN;
            double ura;
            double sqrtA;
            string msg;
            int prn;
            int toc;

            GlobalMembersRtkcmn.trace(3, "decode_bdsephemerisb: len=%d\n", raw.len);

            if (raw.len < DefineConstants.OEM4HLEN + 196)
            {
                GlobalMembersRtkcmn.trace(2, "oem4 bdsephemrisb length error: len=%d\n", raw.len);
                return -1;
            }
            prn = GlobalMembersBinex.U4(p);
            p += 4;
            eph.week = GlobalMembersBinex.U4(p);
            p += 4;
            ura = GlobalMembersBinex.R8(p);
            p += 8;
            eph.svh = GlobalMembersBinex.U4(p) & 1;
            p += 4;
            eph.tgd[0] = GlobalMembersBinex.R8(p); // TGD1 for B1 (s)
            p += 8;
            eph.tgd[1] = GlobalMembersBinex.R8(p); // TGD2 for B2 (s)
            p += 8;
            eph.iodc = GlobalMembersBinex.U4(p); // AODC
            p += 4;
            toc = GlobalMembersBinex.U4(p);
            p += 4;
            eph.f0 = GlobalMembersBinex.R8(p);
            p += 8;
            eph.f1 = GlobalMembersBinex.R8(p);
            p += 8;
            eph.f2 = GlobalMembersBinex.R8(p);
            p += 8;
            eph.iode = GlobalMembersBinex.U4(p); // AODE
            p += 4;
            eph.toes = GlobalMembersBinex.U4(p);
            p += 4;
            sqrtA = GlobalMembersBinex.R8(p);
            p += 8;
            eph.e = GlobalMembersBinex.R8(p);
            p += 8;
            eph.omg = GlobalMembersBinex.R8(p);
            p += 8;
            eph.deln = GlobalMembersBinex.R8(p);
            p += 8;
            eph.M0 = GlobalMembersBinex.R8(p);
            p += 8;
            eph.OMG0 = GlobalMembersBinex.R8(p);
            p += 8;
            eph.OMGd = GlobalMembersBinex.R8(p);
            p += 8;
            eph.i0 = GlobalMembersBinex.R8(p);
            p += 8;
            eph.idot = GlobalMembersBinex.R8(p);
            p += 8;
            eph.cuc = GlobalMembersBinex.R8(p);
            p += 8;
            eph.cus = GlobalMembersBinex.R8(p);
            p += 8;
            eph.crc = GlobalMembersBinex.R8(p);
            p += 8;
            eph.crs = GlobalMembersBinex.R8(p);
            p += 8;
            eph.cic = GlobalMembersBinex.R8(p);
            p += 8;
            eph.cis = GlobalMembersBinex.R8(p);
            eph.A = sqrtA * sqrtA;
            eph.sva = GlobalMembersBinex.uraindex(ura);

            if (raw.outtype != 0)
            {
                msg = raw.msgtype.Substring(raw.msgtype.Length);
                msg = string.Format(" prn={0,3:D} iod={1,3:D} toes={2,6:f0}", prn, eph.iode, eph.toes);
            }
            if ((eph.sat = GlobalMembersRtkcmn.satno(DefineConstants.SYS_CMP, prn)) == 0)
            {
                GlobalMembersRtkcmn.trace(2, "oemv bdsephemeris satellite error: prn=%d\n", prn);
                return -1;
            }
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: eph.toe=bdt2gpst(bdt2time(eph.week,eph.toes));
            eph.toe.CopyFrom(GlobalMembersRtkcmn.bdt2gpst(GlobalMembersRtkcmn.bdt2time(eph.week, eph.toes))); // bdt -> gpst
                                                                                                              //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                                                                                                              //ORIGINAL LINE: eph.toc=bdt2gpst(bdt2time(eph.week,toc));
            eph.toc.CopyFrom(GlobalMembersRtkcmn.bdt2gpst(GlobalMembersRtkcmn.bdt2time(eph.week, toc))); // bdt -> gpst
                                                                                                         //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                                                                                                         //ORIGINAL LINE: eph.ttr=raw->time;
            eph.ttr.CopyFrom(raw.time);

            if (!StringFunctions.StrStr(raw.opt, "-EPHALL"))
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: if (timediff(raw->nav.eph[eph.sat-1].toe,eph.toe)==0.0)
                if (GlobalMembersRtkcmn.timediff(raw.nav.eph[eph.sat - 1].toe, new gtime_t(eph.toe)) == 0.0) // unchanged
                {
                    return 0;
                }
            }
            raw.nav.eph[eph.sat - 1] = eph;
            raw.ephsat = eph.sat;
            return 2;
        }
        /* decode rgeb ---------------------------------------------------------------*/
        internal static int decode_rgeb(raw_t raw)
        {
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: byte *p=raw->buff+DefineConstants.OEM3HLEN;
            byte p = raw.buff + DefineConstants.OEM3HLEN;
            double tow;
            double psr;
            double adr;
            double tt;
            double lockt;
            double dop;
            double snr;
            int i;
            int week;
            int nobs;
            int prn;
            int sat;
            int stat;
            int sys;
            int parity;
            int lli;
            int index;
            int freq;

            GlobalMembersRtkcmn.trace(3, "decode_rgeb: len=%d\n", raw.len);

            week = GlobalMembersRtkcmn.adjgpsweek(GlobalMembersBinex.U4(p));
            tow = GlobalMembersBinex.R8(p + 4);
            nobs = GlobalMembersBinex.U4(p + 12);
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: raw->time=gpst2time(week,tow);
            raw.time.CopyFrom(GlobalMembersRtkcmn.gpst2time(week, tow));

            if (raw.len != DefineConstants.OEM3HLEN + 20 + nobs * 44)
            {
                GlobalMembersRtkcmn.trace(2, "oem3 regb length error: len=%d nobs=%d\n", raw.len, nobs);
                return -1;
            }
            for (i = 0, p += 20; i < nobs; i++, p += 44)
            {
                prn = GlobalMembersBinex.U4(p);
                psr = GlobalMembersBinex.R8(p + 4);
                adr = GlobalMembersBinex.R8(p + 16);
                dop = GlobalMembersBinex.R4(p + 28);
                snr = GlobalMembersBinex.R4(p + 32);
                lockt = GlobalMembersBinex.R4(p + 36); // lock time (s)
                stat = GlobalMembersBinex.I4(ref p + 40); // tracking status
                freq = (stat >> 20) & 1; // L1:0,L2:1
                sys = (stat >> 15) & 7; // satellite sys (0:GPS,1:GLONASS,2:WAAS)
                parity = (stat >> 10) & 1; // parity known
                if ((sat = GlobalMembersRtkcmn.satno(sys == 1 ? DefineConstants.SYS_GLO : (sys == 2 ? DefineConstants.SYS_SBS : DefineConstants.SYS_GPS), prn)) == 0)
                {
                    GlobalMembersRtkcmn.trace(2, "oem3 regb satellite number error: sys=%d prn=%d\n", sys, prn);
                    continue;
                }
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: tt=timediff(raw->time,raw->tobs);
                tt = GlobalMembersRtkcmn.timediff(new gtime_t(raw.time), new gtime_t(raw.tobs));
                if (raw.tobs.time != 0)
                {
                    lli = lockt - raw.lockt[sat - 1, freq] + 0.05 < tt || parity != raw.halfc[sat - 1, freq];
                }
                else
                {
                    lli = 0;
                }
                if (parity == 0)
                {
                    lli |= 2;
                }
                raw.lockt[sat - 1, freq] = lockt;
                raw.halfc[sat - 1, freq] = parity;

                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: if (fabs(timediff(raw->obs.data[0].time,raw->time))>1E-9)
                if (Math.Abs(GlobalMembersRtkcmn.timediff(raw.obs.data[0].time, new gtime_t(raw.time))) > 1E-9)
                {
                    raw.obs.n = 0;
                }
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: if ((index=obsindex(&raw->obs,raw->time,sat))>=0)
                if ((index = GlobalMembersNovatel.obsindex(raw.obs, new gtime_t(raw.time), sat)) >= 0)
                {
                    raw.obs.data[index].L[freq] = -adr; // flip sign
                    raw.obs.data[index].P[freq] = psr;
                    raw.obs.data[index].D[freq] = (float)dop;
                    raw.obs.data[index].SNR[freq] = 0.0 <= snr && snr < 255.0 ? (byte)(snr * 4.0 + 0.5) : 0;
                    raw.obs.data[index].LLI[freq] = (byte)lli;
                    raw.obs.data[index].code[freq] = freq == 0 ? DefineConstants.CODE_L1C : DefineConstants.CODE_L2P;
                }
            }
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: raw->tobs=raw->time;
            raw.tobs.CopyFrom(raw.time);
            return 1;
        }
        /* decode rged ---------------------------------------------------------------*/
        internal static int decode_rged(raw_t raw)
        {
            uint word;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: byte *p=raw->buff+DefineConstants.OEM3HLEN;
            byte p = raw.buff + DefineConstants.OEM3HLEN;
            double tow;
            double psrh;
            double psrl;
            double psr;
            double adr;
            double adr_rolls;
            double tt;
            double lockt;
            double dop;
            int i;
            int week;
            int nobs;
            int prn;
            int sat;
            int stat;
            int sys;
            int parity;
            int lli;
            int index;
            int freq;
            int snr;

            GlobalMembersRtkcmn.trace(3, "decode_rged: len=%d\n", raw.len);

            nobs = GlobalMembersBinex.U2(p);
            week = GlobalMembersRtkcmn.adjgpsweek(GlobalMembersBinex.U2(p + 2));
            tow = GlobalMembersBinex.U4(p + 4) / 100.0;
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: raw->time=gpst2time(week,tow);
            raw.time.CopyFrom(GlobalMembersRtkcmn.gpst2time(week, tow));
            if (raw.len != DefineConstants.OEM3HLEN + 12 + nobs * 20)
            {
                GlobalMembersRtkcmn.trace(2, "oem3 regd length error: len=%d nobs=%d\n", raw.len, nobs);
                return -1;
            }
            for (i = 0, p += 12; i < nobs; i++, p += 20)
            {
                word = GlobalMembersBinex.U4(p);
                prn = word & 0x3F;
                snr = ((word >> 6) & 0x1F) + 20;
                lockt = (word >> 11) / 32.0;
                adr = -GlobalMembersBinex.I4(ref p + 4) / 256.0;
                word = GlobalMembersBinex.U4(p + 8);
                psrh = word & 0xF;
                dop = GlobalMembersNovatel.exsign(word >> 4, 28) / 256.0;
                psrl = GlobalMembersBinex.U4(p + 12);
                stat = GlobalMembersBinex.U4(p + 16) >> 8;
                freq = (stat >> 20) & 1; // L1:0,L2:1
                sys = (stat >> 15) & 7; // satellite sys (0:GPS,1:GLONASS,2:WAAS)
                parity = (stat >> 10) & 1; // parity known
                if ((sat = GlobalMembersRtkcmn.satno(sys == 1 ? DefineConstants.SYS_GLO : (sys == 2 ? DefineConstants.SYS_SBS : DefineConstants.SYS_GPS), prn)) == 0)
                {
                    GlobalMembersRtkcmn.trace(2, "oem3 regd satellite number error: sys=%d prn=%d\n", sys, prn);
                    continue;
                }
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: tt=timediff(raw->time,raw->tobs);
                tt = GlobalMembersRtkcmn.timediff(new gtime_t(raw.time), new gtime_t(raw.tobs));
                psr = (psrh * 4294967296.0 + psrl) / 128.0;
                adr_rolls = Math.Floor((psr / (freq == 0 ? DefineConstants.WL1 : DefineConstants.WL2) - adr) / DefineConstants.MAXVAL + 0.5);
                adr = adr + DefineConstants.MAXVAL * adr_rolls;

                if (raw.tobs.time != 0)
                {
                    lli = lockt - raw.lockt[sat - 1, freq] + 0.05 < tt || parity != raw.halfc[sat - 1, freq];
                }
                else
                {
                    lli = 0;
                }
                if (parity == 0)
                {
                    lli |= 2;
                }
                raw.lockt[sat - 1, freq] = lockt;
                raw.halfc[sat - 1, freq] = parity;

                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: if (fabs(timediff(raw->obs.data[0].time,raw->time))>1E-9)
                if (Math.Abs(GlobalMembersRtkcmn.timediff(raw.obs.data[0].time, new gtime_t(raw.time))) > 1E-9)
                {
                    raw.obs.n = 0;
                }
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: if ((index=obsindex(&raw->obs,raw->time,sat))>=0)
                if ((index = GlobalMembersNovatel.obsindex(raw.obs, new gtime_t(raw.time), sat)) >= 0)
                {
                    raw.obs.data[index].L[freq] = adr;
                    raw.obs.data[index].P[freq] = psr;
                    raw.obs.data[index].D[freq] = (float)dop;
                    raw.obs.data[index].SNR[freq] = (byte)(snr * 4.0 + 0.5);
                    raw.obs.data[index].LLI[freq] = (byte)lli;
                    raw.obs.data[index].code[freq] = freq == 0 ? DefineConstants.CODE_L1C : DefineConstants.CODE_L2P;
                }
            }
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: raw->tobs=raw->time;
            raw.tobs.CopyFrom(raw.time);
            return 1;
        }
        /* decode repb ---------------------------------------------------------------*/
        internal static int decode_repb(raw_t raw)
        {
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: byte *p=raw->buff+DefineConstants.OEM3HLEN;
            byte p = raw.buff + DefineConstants.OEM3HLEN;
            eph_t eph = new eph_t();
            int prn;
            int sat;

            GlobalMembersRtkcmn.trace(3, "decode_repb: len=%d\n", raw.len);

            if (raw.len != DefineConstants.OEM3HLEN + 96)
            {
                GlobalMembersRtkcmn.trace(2, "oem3 repb length error: len=%d\n", raw.len);
                return -1;
            }
            prn = GlobalMembersBinex.U4(p);
            if ((sat = GlobalMembersRtkcmn.satno(DefineConstants.SYS_GPS, prn)) == 0)
            {
                GlobalMembersRtkcmn.trace(2, "oem3 repb satellite number error: prn=%d\n", prn);
                return -1;
            }
            if (GlobalMembersRcvraw.decode_frame(p + 4, eph, null, null, null, null) != 1 || GlobalMembersRcvraw.decode_frame(p + 34, eph, null, null, null, null) != 2 || GlobalMembersRcvraw.decode_frame(p + 64, eph, null, null, null, null) != 3)
            {
                GlobalMembersRtkcmn.trace(2, "oem3 repb subframe error: prn=%d\n", prn);
                return -1;
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
        /* decode frmb --------------------------------------------------------------*/
        internal static int decode_frmb(raw_t raw)
        {
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: byte *p=raw->buff+DefineConstants.OEM3HLEN;
            byte p = raw.buff + DefineConstants.OEM3HLEN;
            double tow;
            int i;
            int week;
            int prn;
            int nbit;

            GlobalMembersRtkcmn.trace(3, "decode_frmb: len=%d\n", raw.len);

            week = GlobalMembersRtkcmn.adjgpsweek(GlobalMembersBinex.U4(p));
            tow = GlobalMembersBinex.R8(p + 4);
            prn = GlobalMembersBinex.U4(p + 12);
            nbit = GlobalMembersBinex.U4(p + 20);
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: raw->time=gpst2time(week,tow);
            raw.time.CopyFrom(GlobalMembersRtkcmn.gpst2time(week, tow));
            if (nbit != 250)
            {
                return 0;
            }
            if (prn < DefineConstants.MINPRNSBS || DefineConstants.MAXPRNSBS < prn)
            {
                GlobalMembersRtkcmn.trace(2, "oem3 frmb satellite number error: prn=%d\n", prn);
                return -1;
            }
            raw.sbsmsg.week = week;
            raw.sbsmsg.tow = (int)tow;
            raw.sbsmsg.prn = prn;
            for (i = 0; i < 29; i++)
            {
                raw.sbsmsg.msg[i] = p[24 + i];
            }
            return 3;
        }
        /* decode ionb ---------------------------------------------------------------*/
        internal static int decode_ionb(raw_t raw)
        {
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: byte *p=raw->buff+DefineConstants.OEM3HLEN;
            byte p = raw.buff + DefineConstants.OEM3HLEN;
            int i;

            if (raw.len != 64 + DefineConstants.OEM3HLEN)
            {
                GlobalMembersRtkcmn.trace(2, "oem3 ionb length error: len=%d\n", raw.len);
                return -1;
            }
            for (i = 0; i < 8; i++)
            {
                raw.nav.ion_gps[i] = GlobalMembersBinex.R8(p + i * 8);
            }
            return 9;
        }
        /* decode utcb ---------------------------------------------------------------*/
        internal static int decode_utcb(raw_t raw)
        {
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: byte *p=raw->buff+DefineConstants.OEM3HLEN;
            byte p = raw.buff + DefineConstants.OEM3HLEN;

            GlobalMembersRtkcmn.trace(3, "decode_utcb: len=%d\n", raw.len);

            if (raw.len != 40 + DefineConstants.OEM3HLEN)
            {
                GlobalMembersRtkcmn.trace(2, "oem3 utcb length error: len=%d\n", raw.len);
                return -1;
            }
            raw.nav.utc_gps[0] = GlobalMembersBinex.R8(p);
            raw.nav.utc_gps[1] = GlobalMembersBinex.R8(p + 8);
            raw.nav.utc_gps[2] = GlobalMembersBinex.U4(p + 16);
            raw.nav.utc_gps[3] = GlobalMembersRtkcmn.adjgpsweek(GlobalMembersBinex.U4(p + 20));
            raw.nav.leaps = GlobalMembersBinex.I4(ref p + 28);
            return 9;
        }
        /* decode oem4 message -------------------------------------------------------*/
        internal static int decode_oem4(raw_t raw)
        {
            double tow;
            int msg;
            int week;
            int type = GlobalMembersBinex.U2(raw.buff + 4);

            GlobalMembersRtkcmn.trace(3, "decode_oem4: type=%3d len=%d\n", type, raw.len);

            /* check crc32 */
            if (GlobalMembersRtkcmn.crc32(raw.buff, raw.len) != GlobalMembersBinex.U4(raw.buff + raw.len))
            {
                GlobalMembersRtkcmn.trace(2, "oem4 crc error: type=%3d len=%d\n", type, raw.len);
                return -1;
            }
            msg = (((byte)(raw.buff + 6)) >> 4) & 0x3;
            week = GlobalMembersRtkcmn.adjgpsweek(GlobalMembersBinex.U2(raw.buff + 14));
            tow = GlobalMembersBinex.U4(raw.buff + 16) * 0.001;
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: raw->time=gpst2time(week,tow);
            raw.time.CopyFrom(GlobalMembersRtkcmn.gpst2time(week, tow));

            if (raw.outtype != 0)
            {
                raw.msgtype = string.Format("OEM4 {0,4:D} ({1,4:D}): msg={2:D} {3}", type, raw.len, msg, GlobalMembersRtkcmn.time_str(GlobalMembersRtkcmn.gpst2time(week, tow), 2));
            }
            if (msg != 0) // message type: 0=binary,1=ascii
            {
                return 0;
            }

            switch (type)
            {
                case DefineConstants.ID_RANGECMP:
                    return GlobalMembersNovatel.decode_rangecmpb(raw);
                case DefineConstants.ID_RANGE:
                    return GlobalMembersNovatel.decode_rangeb(raw);
                case DefineConstants.ID_RAWEPHEM:
                    return GlobalMembersNovatel.decode_rawephemb(raw);
                case DefineConstants.ID_RAWWAASFRAME:
                    return GlobalMembersNovatel.decode_rawwaasframeb(raw);
                case DefineConstants.ID_RAWSBASFRAME:
                    return GlobalMembersNovatel.decode_rawsbasframeb(raw);
                case DefineConstants.ID_IONUTC:
                    return GlobalMembersNovatel.decode_ionutcb(raw);
                case DefineConstants.ID_GLOEPHEMERIS:
                    return GlobalMembersNovatel.decode_gloephemerisb(raw);
                case DefineConstants.ID_QZSSRAWEPHEM:
                    return GlobalMembersNovatel.decode_qzssrawephemb(raw);
                case DefineConstants.ID_QZSSRAWSUBFRAME:
                    return GlobalMembersNovatel.decode_qzssrawsubframeb(raw);
                case DefineConstants.ID_QZSSIONUTC:
                    return GlobalMembersNovatel.decode_qzssionutcb(raw);
                case DefineConstants.ID_GALEPHEMERIS:
                    return GlobalMembersNovatel.decode_galephemerisb(raw);
                case DefineConstants.ID_GALALMANAC:
                    return GlobalMembersNovatel.decode_galalmanacb(raw);
                case DefineConstants.ID_GALCLOCK:
                    return GlobalMembersNovatel.decode_galclockb(raw);
                case DefineConstants.ID_GALIONO:
                    return GlobalMembersNovatel.decode_galionob(raw);
                case DefineConstants.ID_GALFNAVRAWPAGE:
                    return GlobalMembersNovatel.decode_galfnavrawpageb(raw);
                case DefineConstants.ID_GALINAVRAWWORD:
                    return GlobalMembersNovatel.decode_galinavrawwordb(raw);
                case DefineConstants.ID_RAWCNAVFRAME:
                    return GlobalMembersNovatel.decode_rawcnavframeb(raw);
                case DefineConstants.ID_BDSEPHEMERIS:
                    return GlobalMembersNovatel.decode_bdsephemerisb(raw);
            }
            return 0;
        }
        /* decode oem3 message -------------------------------------------------------*/
        internal static int decode_oem3(raw_t raw)
        {
            int type = GlobalMembersBinex.U4(raw.buff + 4);

            GlobalMembersRtkcmn.trace(3, "decode_oem3: type=%3d len=%d\n", type, raw.len);

            /* checksum */
            if (GlobalMembersCrescent.chksum(raw.buff, raw.len) != 0)
            {
                GlobalMembersRtkcmn.trace(2, "oem3 checksum error: type=%3d len=%d\n", type, raw.len);
                return -1;
            }
            if (raw.outtype != 0)
            {
                raw.msgtype = string.Format("OEM3 {0,4:D} ({1,4:D}):", type, raw.len);
            }
            switch (type)
            {
                case DefineConstants.ID_RGEB:
                    return GlobalMembersNovatel.decode_rgeb(raw);
                case DefineConstants.ID_RGED:
                    return GlobalMembersNovatel.decode_rged(raw);
                case DefineConstants.ID_REPB:
                    return GlobalMembersNovatel.decode_repb(raw);
                case DefineConstants.ID_FRMB:
                    return GlobalMembersNovatel.decode_frmb(raw);
                case DefineConstants.ID_IONB:
                    return GlobalMembersNovatel.decode_ionb(raw);
                case DefineConstants.ID_UTCB:
                    return GlobalMembersNovatel.decode_utcb(raw);
            }
            return 0;
        }
        /* sync header ---------------------------------------------------------------*/
        internal static int sync_oem4(byte[] buff, byte data)
        {
            buff[0] = buff[1];
            buff[1] = buff[2];
            buff[2] = data;
            return buff[0] == DefineConstants.OEM4SYNC1 && buff[1] == DefineConstants.OEM4SYNC2 && buff[2] == DefineConstants.OEM4SYNC3;
        }
        internal static int sync_oem3(byte[] buff, byte data)
        {
            buff[0] = buff[1];
            buff[1] = buff[2];
            buff[2] = data;
            return buff[0] == DefineConstants.OEM3SYNC1 && buff[1] == DefineConstants.OEM3SYNC2 && buff[2] == DefineConstants.OEM3SYNC3;
        }
        /* input oem4/oem3 raw data from stream ----------------------------------------
        * fetch next novatel oem4/oem3 raw data and input a mesasge from stream
        * args   : raw_t *raw   IO     receiver raw data control struct
        *          unsigned char data I stream data (1 byte)
        * return : status (-1: error message, 0: no message, 1: input observation data,
        *                  2: input ephemeris, 3: input sbas message,
        *                  9: input ion/utc parameter)
        *
        * notes  : to specify input options for oem4, set raw->opt to the following
        *          option strings separated by spaces.
        *
        *          -EPHALL : input all ephemerides
        *          -GL1P   : select 1P for GPS L1 (default 1C)
        *          -GL2X   : select 2X for GPS L2 (default 2W)
        *          -RL2C   : select 2C for GLO L2 (default 2P)
        *          -EL2C   : select 2C for GAL L2 (default 2C)
        *
        *-----------------------------------------------------------------------------*/
        public static int input_oem4(raw_t raw, byte data)
        {
            GlobalMembersRtkcmn.trace(5, "input_oem4: data=%02x\n", data);

            /* synchronize frame */
            if (raw.nbyte == 0)
            {
                if (GlobalMembersNovatel.sync_oem4(raw.buff, data) != 0)
                {
                    raw.nbyte = 3;
                }
                return 0;
            }
            raw.buff[raw.nbyte++] = data;

            if (raw.nbyte == 10 && (raw.len = GlobalMembersBinex.U2(raw.buff + 8) + DefineConstants.OEM4HLEN) > DefineConstants.MAXRAWLEN - 4)
            {
                GlobalMembersRtkcmn.trace(2, "oem4 length error: len=%d\n", raw.len);
                raw.nbyte = 0;
                return -1;
            }
            if (raw.nbyte < 10 || raw.nbyte < raw.len + 4)
            {
                return 0;
            }
            raw.nbyte = 0;

            /* decode oem4 message */
            return GlobalMembersNovatel.decode_oem4(raw);
        }
        public static int input_oem3(raw_t raw, byte data)
        {
            GlobalMembersRtkcmn.trace(5, "input_oem3: data=%02x\n", data);

            /* synchronize frame */
            if (raw.nbyte == 0)
            {
                if (GlobalMembersNovatel.sync_oem3(raw.buff, data) != 0)
                {
                    raw.nbyte = 3;
                }
                return 0;
            }
            raw.buff[raw.nbyte++] = data;

            if (raw.nbyte == 12 && (raw.len = GlobalMembersBinex.U4(raw.buff + 8)) > DefineConstants.MAXRAWLEN)
            {
                GlobalMembersRtkcmn.trace(2, "oem3 length error: len=%d\n", raw.len);
                raw.nbyte = 0;
                return -1;
            }
            if (raw.nbyte < 12 || raw.nbyte < raw.len)
            {
                return 0;
            }
            raw.nbyte = 0;

            /* decode oem3 message */
            return GlobalMembersNovatel.decode_oem3(raw);
        }
        /* input oem4/oem3 raw data from file ------------------------------------------
        * fetch next novatel oem4/oem3 raw data and input a message from file
        * args   : raw_t  *raw   IO     receiver raw data control struct
        *          int    format I      receiver raw data format (STRFMT_???)
        *          FILE   *fp    I      file pointer
        * return : status(-2: end of file, -1...9: same as above)
        *-----------------------------------------------------------------------------*/
        public static int input_oem4f(raw_t raw, FILE fp)
        {
            int i;
            int data;

            GlobalMembersRtkcmn.trace(4, "input_oem4f:\n");

            /* synchronize frame */
            if (raw.nbyte == 0)
            {
                for (i = 0; ; i++)
                {
                    if ((data = fgetc(fp)) == EOF)
                    {
                        return -2;
                    }
                    if (GlobalMembersNovatel.sync_oem4(raw.buff, (byte)data) != 0)
                        break;
                    if (i >= 4096)
                    {
                        return 0;
                    }
                }
            }
            if (fread(raw.buff + 3, 7, 1, fp) < 1)
            {
                return -2;
            }
            raw.nbyte = 10;

            if ((raw.len = GlobalMembersBinex.U2(raw.buff + 8) + DefineConstants.OEM4HLEN) > DefineConstants.MAXRAWLEN - 4)
            {
                GlobalMembersRtkcmn.trace(2, "oem4 length error: len=%d\n", raw.len);
                raw.nbyte = 0;
                return -1;
            }
            if (fread(raw.buff + 10, raw.len - 6, 1, fp) < 1)
            {
                return -2;
            }
            raw.nbyte = 0;

            /* decode oem4 message */
            return GlobalMembersNovatel.decode_oem4(raw);
        }
        public static int input_oem3f(raw_t raw, FILE fp)
        {
            int i;
            int data;

            GlobalMembersRtkcmn.trace(4, "input_oem3f:\n");

            /* synchronize frame */
            if (raw.nbyte == 0)
            {
                for (i = 0; ; i++)
                {
                    if ((data = fgetc(fp)) == EOF)
                    {
                        return -2;
                    }
                    if (GlobalMembersNovatel.sync_oem3(raw.buff, (byte)data) != 0)
                        break;
                    if (i >= 4096)
                    {
                        return 0;
                    }
                }
            }
            if (fread(raw.buff + 3, 1, 9, fp) < 9)
            {
                return -2;
            }
            raw.nbyte = 12;

            if ((raw.len = GlobalMembersBinex.U4(raw.buff + 8)) > DefineConstants.MAXRAWLEN)
            {
                GlobalMembersRtkcmn.trace(2, "oem3 length error: len=%d\n", raw.len);
                raw.nbyte = 0;
                return -1;
            }
            if (fread(raw.buff + 12, 1, raw.len - 12, fp) < (uint)(raw.len - 12))
            {
                return -2;
            }
            raw.nbyte = 0;

            /* decode oem3 message */
            return GlobalMembersNovatel.decode_oem3(raw);
        }
    }
}
