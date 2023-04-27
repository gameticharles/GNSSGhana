using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ghGPS.Classes.rcv
{
    public static class GlobalMembersCrescent
    {
        /*------------------------------------------------------------------------------
        * crescent.c : hemisphere crescent/eclipse receiver dependent functions
        *
        *          Copyright (C) 2007-2014 by T.TAKASU, All rights reserved.
        *
        * reference :
        *     [1] Hemisphere GPS, Grescent Integrator's Manual, December, 2005
        *     [2] Hemisphere GPS, GPS Technical Reference, Part No. 875-0175-000,
        *         Rev.D1, 2008
        *     [3] Hemisphere GPS, Hemisphere GPS Technical Reference, 2014
        *
        * version : $Revision: 1.2 $ $Date: 2008/07/14 00:05:05 $
        * history : 2008/05/21 1.0 new
        *           2009/04/01 1.1 support sbas, set 0 to L2 observables
        *                          fix bug on getting doppler observables
        *           2009/10/19 1.2 support eclipse (message bin 76)
        *           2009/10/24 1.3 ignore vaild phase flag
        *           2011/05/27 1.4 add -EPHALL option
        *                          fix problem with ARM compiler
        *           2011/07/01 1.5 suppress warning
        *           2013/02/23 1.6 fix memory access violation problem on arm
        *           2014/05/13 1.7 support bin65 and bin66
        *                          add receiver option -TTCORR
        *           2014/06/21 1.8 move decode_glostr() to rcvraw.c
        *-----------------------------------------------------------------------------*/


        internal const string rcsid = "$Id: crescent.c,v 1.2 2008/07/14 00:05:05 TTAKA Exp $";


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

        /* checksum ------------------------------------------------------------------*/
        internal static int chksum(byte[] buff, int len)
        {
            ushort sum = 0;
            int i;

            for (i = 8; i < len - 4; i++)
            {
                sum += buff[i];
            }
            GlobalMembersRtkcmn.trace(4, "checksum=%02X%02X %02X%02X:%02X%02X\n", sum >> 8, sum & 0xFF, buff[len - 3], buff[len - 4], buff[len - 2], buff[len - 1]);
            return (sum >> 8) == buff[len - 3] && (sum & 0xFF) == buff[len - 4] && buff[len - 2] == 0x0D && buff[len - 1] == 0x0A;
        }
        /* decode bin 1 postion/velocity ---------------------------------------------*/
        internal static int decode_crespos(raw_t raw)
        {
            int ns;
            int week;
            int mode;
            double tow;
            double[] pos = new double[3];
            double[] vel = new double[3];
            double std;
            string tstr = new string(new char[64]);
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: byte *p=raw->buff+8;
            byte p = raw.buff + 8;

            GlobalMembersRtkcmn.trace(4, "decode_crespos: len=%d\n", raw.len);

            if (raw.len != 64)
            {
                GlobalMembersRtkcmn.trace(2, "crescent bin 1 message length error: len=%d\n", raw.len);
                return -1;
            }
            ns = ((byte)(p + 1));
            week = GlobalMembersBinex.U2(p + 2);
            tow = GlobalMembersBinex.R8(p + 4);
            pos[0] = GlobalMembersBinex.R8(p + 12);
            pos[1] = GlobalMembersBinex.R8(p + 20);
            pos[2] = GlobalMembersBinex.R4(p + 28);
            vel[0] = GlobalMembersBinex.R4(p + 32);
            vel[1] = GlobalMembersBinex.R4(p + 36);
            vel[2] = GlobalMembersBinex.R4(p + 40);
            std = GlobalMembersBinex.R4(p + 44);
            mode = GlobalMembersBinex.U2(p + 48);
            GlobalMembersRtkcmn.time2str(GlobalMembersRtkcmn.gpst2time(week, tow), ref tstr, 3);
            GlobalMembersRtkcmn.trace(3, "$BIN1 %s %13.9f %14.9f %10.4f %4d %3d %.3f\n", tstr, pos[0], pos[1], pos[2], mode == 6 ? 1 : (mode > 4 ? 2 : (mode > 1 ? 5 : 0)), ns, std);
            return 0;
        }
        /* decode bin 96 raw phase and code ------------------------------------------*/
        internal static int decode_cresraw(raw_t raw)
        {
            gtime_t time = new gtime_t();
            double tow;
            double tows;
            double toff = 0.0;
            double cp;
            double pr;
            double dop;
            double snr;
            int i;
            int j;
            int n;
            int prn;
            int sat;
            int week;
            int word2;
            int lli = 0;
            uint word1;
            uint sn;
            uint sc;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: byte *p=raw->buff+8;
            byte p = raw.buff + 8;

            GlobalMembersRtkcmn.trace(4, "decode_cresraw: len=%d\n", raw.len);

            if (raw.len != 312)
            {
                GlobalMembersRtkcmn.trace(2, "crescent bin 96 message length error: len=%d\n", raw.len);
                return -1;
            }
            week = GlobalMembersBinex.U2(p + 2);
            tow = GlobalMembersBinex.R8(p + 4);
            tows = Math.Floor(tow * 1000.0 + 0.5) / 1000.0; // round by 1ms
                                                            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                                                            //ORIGINAL LINE: time=gpst2time(week,tows);
            time.CopyFrom(GlobalMembersRtkcmn.gpst2time(week, tows));

            /* time tag offset correction */
            if (StringFunctions.StrStr(raw.opt, "-TTCORR"))
            {
                toff = DefineConstants.CLIGHT * (tows - tow);
            }
            for (i = n = 0, p + = 12; i < 12 && n < DefineConstants.MAXOBS; i++, p + = 24)
            {
                word1 = GlobalMembersBinex.U4(p);
                word2 = GlobalMembersBinex.I4(ref p + 4);
                if ((prn = word1 & 0xFF) == 0) // if 0, no data
                    continue;
                if ((sat = GlobalMembersRtkcmn.satno(prn <= DefineConstants.MAXPRNGPS ? DefineConstants.SYS_GPS : DefineConstants.SYS_SBS, prn)) == 0)
                {
                    GlobalMembersRtkcmn.trace(2, "creasent bin 96 satellite number error: prn=%d\n", prn);
                    continue;
                }
                pr = GlobalMembersBinex.R8(p + 8) - toff;
                cp = GlobalMembersBinex.R8(p + 16) - toff;
                if (!(word2 & 1)) // invalid phase
                {
                    cp = 0.0;
                }
                sn = (word1 >> 8) & 0xFF;
                snr = sn == 0 ? 0.0 : 10.0 * Math.Log10(0.8192 * sn) + DefineConstants.SNR2CN0_L1;
                sc = (uint)(word1 >> 24);
                if (raw.time.time != 0)
                {
                    lli = (int)((byte)sc - (byte)raw.lockt[sat - 1, 0]) > 0;
                }
                raw.lockt[sat - 1, 0] = (byte)sc;
                dop = word2 / 16 / 4096.0;

                raw.obs.data[n].time = time;
                raw.obs.data[n].sat = sat;
                raw.obs.data[n].P[0] = pr;
                raw.obs.data[n].L[0] = cp / GlobalMembersRtkcmn.lam_carr[0];
                raw.obs.data[n].D[0] = -(float)(dop / GlobalMembersRtkcmn.lam_carr[0]);
                raw.obs.data[n].SNR[0] = (byte)(snr * 4.0 + 0.5);
                raw.obs.data[n].LLI[0] = (byte)lli;
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
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: raw->time=time;
            raw.time.CopyFrom(time);
            raw.obs.n = n;
            return 1;
        }
        /* decode bin 76 dual-freq raw phase and code --------------------------------*/
        internal static int decode_cresraw2(raw_t raw)
        {
            gtime_t time = new gtime_t();
            double tow;
            double tows;
            double toff = 0.0;
            double[] cp = { 0, null };
            double pr1;
            double[] pr = { 0, null };
            double[] dop = { 0, null };
            double[] snr = { 0, null };
            int i;
            int j;
            int n = 0;
            int prn;
            int sat;
            int week;
            int[] lli = new int[2];
            uint word1;
            uint word2;
            uint word3;
            uint sc;
            uint sn;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: byte *p=raw->buff+8;
            byte p = raw.buff + 8;

            GlobalMembersRtkcmn.trace(4, "decode_cresraw2: len=%d\n", raw.len);

            if (raw.len != 460)
            {
                GlobalMembersRtkcmn.trace(2, "crescent bin 76 message length error: len=%d\n", raw.len);
                return -1;
            }
            tow = GlobalMembersBinex.R8(p);
            week = GlobalMembersBinex.U2(p + 8);
            tows = Math.Floor(tow * 1000.0 + 0.5) / 1000.0; // round by 1ms
                                                            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                                                            //ORIGINAL LINE: time=gpst2time(week,tows);
            time.CopyFrom(GlobalMembersRtkcmn.gpst2time(week, tows));

            /* time tag offset correction */
            if (StringFunctions.StrStr(raw.opt, "-TTCORR"))
            {
                toff = DefineConstants.CLIGHT * (tows - tow);
            }
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: if (fabs(timediff(time,raw->time))<1e-9)
            if (Math.Abs(GlobalMembersRtkcmn.timediff(new gtime_t(time), new gtime_t(raw.time))) < 1e-9)
            {
                n = raw.obs.n;
            }
            for (i = 0, p + = 16; i < 15 && n < DefineConstants.MAXOBS; i++)
            {
                word1 = GlobalMembersBinex.U4(p + 324 + 4 * i); // L1CACodeMSBsPRN
                if ((prn = word1 & 0xFF) == 0) // if 0, no data
                    continue;
                if ((sat = GlobalMembersRtkcmn.satno(prn <= DefineConstants.MAXPRNGPS ? DefineConstants.SYS_GPS : DefineConstants.SYS_SBS, prn)) == 0)
                {
                    GlobalMembersRtkcmn.trace(2, "creasent bin 76 satellite number error: prn=%d\n", prn);
                    continue;
                }
                pr1 = (word1 >> 13) * 256.0; // upper 19bit of L1CA pseudorange

                word1 = GlobalMembersBinex.U4(p + 144 + 12 * i); // L1CASatObs
                word2 = GlobalMembersBinex.U4(p + 148 + 12 * i);
                word3 = GlobalMembersBinex.U4(p + 152 + 12 * i);
                sn = word1 & 0xFFF;
                snr[0] = sn == 0 ? 0.0 : 10.0 * Math.Log10(0.1024 * sn) + DefineConstants.SNR2CN0_L1;
                sc = (uint)(word1 >> 24);
                if (raw.time.time != 0)
                {
                    lli[0] = (int)((byte)sc - (byte)raw.lockt[sat - 1, 0]) > 0;
                }
                else
                {
                    lli[0] = 0;
                }
                lli[0] |= ((word1 >> 12) & 7) ? 2 : 0;
                raw.lockt[sat - 1, 0] = (byte)sc;
                dop[0] = ((word2 >> 1) & 0x7FFFFF) / 512.0;
                if ((word2 >> 24) & 1)
                {
                    dop[0] = -dop[0];
                }
                pr[0] = pr1 + (word3 & 0xFFFF) / 256.0;
                cp[0] = Math.Floor(pr[0] / GlobalMembersRtkcmn.lam_carr[0] / 8192.0) * 8192.0;
                cp[0] += ((word2 & 0xFE000000) + ((word3 & 0xFFFF0000) >> 7)) / 524288.0;
                if (cp[0] - pr[0] / GlobalMembersRtkcmn.lam_carr[0] < -4096.0)
                {
                    cp[0] += 8192.0;
                }
                else if (cp[0] - pr[0] / GlobalMembersRtkcmn.lam_carr[0] > 4096.0)
                {
                    cp[0] -= 8192.0;
                }

                if (i < 12)
                {
                    word1 = GlobalMembersBinex.U4(p + 12 * i); // L2PSatObs
                    word2 = GlobalMembersBinex.U4(p + 4 + 12 * i);
                    word3 = GlobalMembersBinex.U4(p + 8 + 12 * i);
                    sn = word1 & 0xFFF;
                    snr[1] = sn == 0 ? 0.0 : 10.0 * Math.Log10(0.1164 * sn) + DefineConstants.SNR2CN0_L2;
                    sc = (uint)(word1 >> 24);
                    if (raw.time.time == 0)
                    {
                        lli[1] = (int)((byte)sc - (byte)raw.lockt[sat - 1, 1]) > 0;
                    }
                    else
                    {
                        lli[1] = 0;
                    }
                    lli[1] |= ((word1 >> 12) & 7) ? 2 : 0;
                    raw.lockt[sat - 1, 1] = (byte)sc;
                    dop[1] = ((word2 >> 1) & 0x7FFFFF) / 512.0;
                    if ((word2 >> 24) & 1)
                    {
                        dop[1] = -dop[1];
                    }
                    pr[1] = (word3 & 0xFFFF) / 256.0;
                    if (pr[1] != 0.0)
                    {
                        pr[1] += pr1;
                        if (pr[1] - pr[0] < -128.0)
                        {
                            pr[1] += 256.0;
                        }
                        else if (pr[1] - pr[0] > 128.0)
                        {
                            pr[1] -= 256.0;
                        }
                        cp[1] = Math.Floor(pr[1] / GlobalMembersRtkcmn.lam_carr[1] / 8192.0) * 8192.0;
                        cp[1] += ((word2 & 0xFE000000) + ((word3 & 0xFFFF0000) >> 7)) / 524288.0;
                        if (cp[1] - pr[1] / GlobalMembersRtkcmn.lam_carr[1] < -4096.0)
                        {
                            cp[1] += 8192.0;
                        }
                        else if (cp[1] - pr[1] / GlobalMembersRtkcmn.lam_carr[1] > 4096.0)
                        {
                            cp[1] -= 8192.0;
                        }
                    }
                    else
                    {
                        cp[1] = 0.0;
                    }
                }
                raw.obs.data[n].time = time;
                raw.obs.data[n].sat = sat;
                for (j = 0; j < DefineConstants.NFREQ; j++)
                {
                    if (j == 0 || (j == 1 && i < 12))
                    {
                        raw.obs.data[n].P[j] = pr[j] == 0.0 ? 0.0 : pr[j] - toff;
                        raw.obs.data[n].L[j] = cp[j] == 0.0 ? 0.0 : cp[j] - toff / GlobalMembersRtkcmn.lam_carr[j];
                        raw.obs.data[n].D[j] = -(float)dop[j];
                        raw.obs.data[n].SNR[j] = (byte)(snr[j] * 4.0 + 0.5);
                        raw.obs.data[n].LLI[j] = (byte)lli[j];
                        raw.obs.data[n].code[j] = j == 0 ? DefineConstants.CODE_L1C : DefineConstants.CODE_L2P;
                    }
                    else
                    {
                        raw.obs.data[n].L[j] = raw.obs.data[n].P[j] = 0.0;
                        raw.obs.data[n].D[j] = 0.0;
                        raw.obs.data[n].SNR[j] = raw.obs.data[n].LLI[j] = 0;
                        raw.obs.data[n].code[j] = DefineConstants.CODE_NONE;
                    }
                }
                n++;
            }
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: raw->time=time;
            raw.time.CopyFrom(time);
            raw.obs.n = n;
            if (StringFunctions.StrStr(raw.opt, "-ENAGLO")) // glonass follows
            {
                return 0;
            }
            return 1;
        }
        /* decode bin 95 ephemeris ---------------------------------------------------*/
        internal static int decode_creseph(raw_t raw)
        {
            eph_t eph = new eph_t();
            uint word;
            int i;
            int j;
            int k;
            int prn;
            int sat;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: byte *p=raw->buff+8,buff[90];
            byte p = raw.buff + 8;
            byte[] buff = new byte[90];

            GlobalMembersRtkcmn.trace(4, "decode_creseph: len=%d\n", raw.len);

            if (raw.len != 140)
            {
                GlobalMembersRtkcmn.trace(2, "crescent bin 95 message length error: len=%d\n", raw.len);
                return -1;
            }
            prn = GlobalMembersBinex.U2(p);
            if ((sat = GlobalMembersRtkcmn.satno(DefineConstants.SYS_GPS, prn)) == 0)
            {
                GlobalMembersRtkcmn.trace(2, "crescent bin 95 satellite number error: prn=%d\n", prn);
                return -1;
            }
            for (i = 0; i < 3; i++)
            {
                for (j = 0; j < 10; j++)
                {
                    word = GlobalMembersBinex.U4(p + 8 + i * 40 + j * 4) >> 6;
                    for (k = 0; k < 3; k++)
                    {
                        buff[i * 30 + j * 3 + k] = (byte)((word >> (8 * (2 - k))) & 0xFF);
                    }
                }
            }
            if (GlobalMembersRcvraw.decode_frame(buff, eph, null, null, null, null) != 1 || GlobalMembersRcvraw.decode_frame(buff + 30, eph, null, null, null, null) != 2 || GlobalMembersRcvraw.decode_frame(buff + 60, eph, null, null, null, null) != 3)
            {
                GlobalMembersRtkcmn.trace(2, "crescent bin 95 navigation frame error: prn=%d\n", prn);
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
        /* decode bin 94 ion/utc parameters ------------------------------------------*/
        internal static int decode_cresionutc(raw_t raw)
        {
            int i;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: byte *p=raw->buff+8;
            byte p = raw.buff + 8;

            GlobalMembersRtkcmn.trace(4, "decode_cresionutc: len=%d\n", raw.len);

            if (raw.len != 108)
            {
                GlobalMembersRtkcmn.trace(2, "crescent bin 94 message length error: len=%d\n", raw.len);
                return -1;
            }
            for (i = 0; i < 8; i++)
            {
                raw.nav.ion_gps[i] = GlobalMembersBinex.R8(p + i * 8);
            }
            raw.nav.utc_gps[0] = GlobalMembersBinex.R8(p + 64);
            raw.nav.utc_gps[1] = GlobalMembersBinex.R8(p + 72);
            raw.nav.utc_gps[2] = (double)GlobalMembersBinex.U4(p + 80);
            raw.nav.utc_gps[3] = (double)GlobalMembersBinex.U2(p + 84);
            raw.nav.leaps = GlobalMembersCrescent.I2(ref p + 90);
            return 9;
        }
        /* decode bin 80 waas messages -----------------------------------------------*/
        internal static int decode_creswaas(raw_t raw)
        {
            double tow;
            uint word;
            int i;
            int j;
            int k;
            int prn;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: byte *p=raw->buff+8;
            byte p = raw.buff + 8;

            GlobalMembersRtkcmn.trace(4, "decode_creswaas: len=%d\n", raw.len);

            if (raw.len != 52)
            {
                GlobalMembersRtkcmn.trace(2, "creasent bin 80 message length error: len=%d\n", raw.len);
                return -1;
            }
            prn = GlobalMembersBinex.U2(p);
            if (prn < DefineConstants.MINPRNSBS || DefineConstants.MAXPRNSBS < prn)
            {
                GlobalMembersRtkcmn.trace(2, "creasent bin 80 satellite number error: prn=%d\n", prn);
                return -1;
            }
            raw.sbsmsg.prn = prn;
            raw.sbsmsg.tow = GlobalMembersBinex.U4(p + 4);
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

            for (i = k = 0; i < 8 && k < 29; i++)
            {
                word = GlobalMembersBinex.U4(p + 8 + i * 4);
                for (j = 0; j < 4 && k < 29; j++)
                {
                    raw.sbsmsg.msg[k++] = (byte)(word >> (3 - j) * 8);
                }
            }
            raw.sbsmsg.msg[28] &= 0xC0;
            return 3;
        }
        /* decode bin 66 glonass L1/L2 code and carrier phase ------------------------*/
        internal static int decode_cresgloraw(raw_t raw)
        {
            gtime_t time = new gtime_t();
            double tow;
            double tows;
            double toff = 0.0;
            double[] cp = { 0, null };
            double pr1;
            double[] pr = { 0, null };
            double[] dop = { 0, null };
            double[] snr = { 0, null };
            int i;
            int j;
            int n = 0;
            int prn;
            int sat;
            int week;
            int[] lli = new int[2];
            uint word1;
            uint word2;
            uint word3;
            uint sc;
            uint sn;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: byte *p=raw->buff+8;
            byte p = raw.buff + 8;

            GlobalMembersRtkcmn.trace(4, "decode_cregloraw: len=%d\n", raw.len);

            if (!StringFunctions.StrStr(raw.opt, "-ENAGLO"))
            {
                return 0;
            }

            if (raw.len != 364)
            {
                GlobalMembersRtkcmn.trace(2, "crescent bin 66 message length error: len=%d\n", raw.len);
                return -1;
            }
            tow = GlobalMembersBinex.R8(p);
            week = GlobalMembersBinex.U2(p + 8);
            tows = Math.Floor(tow * 1000.0 + 0.5) / 1000.0; // round by 1ms
                                                            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                                                            //ORIGINAL LINE: time=gpst2time(week,tows);
            time.CopyFrom(GlobalMembersRtkcmn.gpst2time(week, tows));

            /* time tag offset correction */
            if (StringFunctions.StrStr(raw.opt, "-TTCORR"))
            {
                toff = DefineConstants.CLIGHT * (tows - tow);
            }
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: if (fabs(timediff(time,raw->time))<1e-9)
            if (Math.Abs(GlobalMembersRtkcmn.timediff(new gtime_t(time), new gtime_t(raw.time))) < 1e-9)
            {
                n = raw.obs.n;
            }
            for (i = 0, p + = 16; i < 12 && n < DefineConstants.MAXOBS; i++)
            {
                word1 = GlobalMembersBinex.U4(p + 288 + 4 * i); // L1CACodeMSBsSlot
                if ((prn = word1 & 0xFF) == 0) // if 0, no data
                    continue;
                if ((sat = GlobalMembersRtkcmn.satno(DefineConstants.SYS_GLO, prn)) == 0)
                {
                    GlobalMembersRtkcmn.trace(2, "creasent bin 66 satellite number error: prn=%d\n", prn);
                    continue;
                }
                pr1 = (word1 >> 13) * 256.0; // upper 19bit of L1CA pseudorange

                /* L1Obs */
                word1 = GlobalMembersBinex.U4(p + 12 * i);
                word2 = GlobalMembersBinex.U4(p + 4 + 12 * i);
                word3 = GlobalMembersBinex.U4(p + 8 + 12 * i);
                sn = word1 & 0xFFF;
                snr[0] = sn == 0 ? 0.0 : 10.0 * Math.Log10(0.1024 * sn) + DefineConstants.SNR2CN0_L1;
                sc = (uint)(word1 >> 24);
                if (raw.time.time != 0)
                {
                    lli[0] = (int)((byte)sc - (byte)raw.lockt[sat - 1, 0]) > 0;
                }
                else
                {
                    lli[0] = 0;
                }
                lli[0] |= ((word1 >> 12) & 7) ? 2 : 0;
                raw.lockt[sat - 1, 0] = (byte)sc;
                dop[0] = ((word2 >> 1) & 0x7FFFFF) / 512.0;
                if ((word2 >> 24) & 1)
                {
                    dop[0] = -dop[0];
                }
                pr[0] = pr1 + (word3 & 0xFFFF) / 256.0;
                cp[0] = Math.Floor(pr[0] / GlobalMembersRtkcmn.lam_carr[0] / 8192.0) * 8192.0;
                cp[0] += ((word2 & 0xFE000000) + ((word3 & 0xFFFF0000) >> 7)) / 524288.0;
                if (cp[0] - pr[0] / GlobalMembersRtkcmn.lam_carr[0] < -4096.0)
                {
                    cp[0] += 8192.0;
                }
                else if (cp[0] - pr[0] / GlobalMembersRtkcmn.lam_carr[0] > 4096.0)
                {
                    cp[0] -= 8192.0;
                }

                /* L2Obs */
                word1 = GlobalMembersBinex.U4(p + 144 + 12 * i);
                word2 = GlobalMembersBinex.U4(p + 148 + 12 * i);
                word3 = GlobalMembersBinex.U4(p + 152 + 12 * i);
                sn = word1 & 0xFFF;
                snr[1] = sn == 0 ? 0.0 : 10.0 * Math.Log10(0.1164 * sn) + DefineConstants.SNR2CN0_L2;
                sc = (uint)(word1 >> 24);
                if (raw.time.time == 0)
                {
                    lli[1] = (int)((byte)sc - (byte)raw.lockt[sat - 1, 1]) > 0;
                }
                else
                {
                    lli[1] = 0;
                }
                lli[1] |= ((word1 >> 12) & 7) ? 2 : 0;
                raw.lockt[sat - 1, 1] = (byte)sc;
                dop[1] = ((word2 >> 1) & 0x7FFFFF) / 512.0;
                if ((word2 >> 24) & 1)
                {
                    dop[1] = -dop[1];
                }
                pr[1] = (word3 & 0xFFFF) / 256.0;
                if (pr[1] != 0.0)
                {
                    pr[1] += pr1;
                    if (pr[1] - pr[0] < -128.0)
                    {
                        pr[1] += 256.0;
                    }
                    else if (pr[1] - pr[0] > 128.0)
                    {
                        pr[1] -= 256.0;
                    }
                    cp[1] = Math.Floor(pr[1] / GlobalMembersRtkcmn.lam_carr[1] / 8192.0) * 8192.0;
                    cp[1] += ((word2 & 0xFE000000) + ((word3 & 0xFFFF0000) >> 7)) / 524288.0;
                    if (cp[1] - pr[1] / GlobalMembersRtkcmn.lam_carr[1] < -4096.0)
                    {
                        cp[1] += 8192.0;
                    }
                    else if (cp[1] - pr[1] / GlobalMembersRtkcmn.lam_carr[1] > 4096.0)
                    {
                        cp[1] -= 8192.0;
                    }
                }
                raw.obs.data[n].time = time;
                raw.obs.data[n].sat = sat;
                for (j = 0; j < DefineConstants.NFREQ; j++)
                {
                    if (j == 0 || (j == 1 && i < 12))
                    {
                        raw.obs.data[n].P[j] = pr[j] == 0.0 ? 0.0 : pr[j] - toff;
                        raw.obs.data[n].L[j] = cp[j] == 0.0 ? 0.0 : cp[j] - toff / GlobalMembersRtkcmn.lam_carr[j];
                        raw.obs.data[n].D[j] = -(float)dop[j];
                        raw.obs.data[n].SNR[j] = (byte)(snr[j] * 4.0 + 0.5);
                        raw.obs.data[n].LLI[j] = (byte)lli[j];
                        raw.obs.data[n].code[j] = j == 0 ? DefineConstants.CODE_L1C : DefineConstants.CODE_L2P;
                    }
                    else
                    {
                        raw.obs.data[n].L[j] = raw.obs.data[n].P[j] = 0.0;
                        raw.obs.data[n].D[j] = 0.0;
                        raw.obs.data[n].SNR[j] = raw.obs.data[n].LLI[j] = 0;
                        raw.obs.data[n].code[j] = DefineConstants.CODE_NONE;
                    }
                }
                n++;
            }
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: raw->time=time;
            raw.time.CopyFrom(time);
            raw.obs.n = n;
            return 1;
        }
        /* decode bin 65 glonass ephemeris -------------------------------------------*/
        internal static int decode_cresgloeph(raw_t raw)
        {
            geph_t geph = new geph_t();
            //C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on this variable, so pointers on this variable are left unchanged:
            byte* p = raw.buff + 8;
            byte[] str = new byte[12];
            int i;
            int j;
            int k;
            int sat;
            int prn;
            int frq;
            int time;
            int no;

            GlobalMembersRtkcmn.trace(4, "decode_cregloeph: len=%d\n", raw.len);

            if (!StringFunctions.StrStr(raw.opt, "-ENAGLO"))
            {
                return 0;
            }

            prn = ((byte)(p));
            p += 1;
            frq = ((byte)(p)) - 8;
            p += 1 + 2;
            time = GlobalMembersBinex.U4(p);
            p += 4;

            if ((sat = GlobalMembersRtkcmn.satno(DefineConstants.SYS_GLO, prn)) == 0)
            {
                GlobalMembersRtkcmn.trace(2, "creasent bin 65 satellite number error: prn=%d\n", prn);
                return -1;
            }
            for (i = 0; i < 5; i++)
            {
                for (j = 0; j < 3; j++)
                {
                    for (k = 3; k >= 0; k--)
                    {
                        str[k + j * 4] = ((byte)(p++));
                    }
                }
                if ((no = GlobalMembersRtkcmn.getbitu(str, 1, 4)) != i + 1)
                {
                    GlobalMembersRtkcmn.trace(2, "creasent bin 65 string no error: sat=%2d no=%d %d\n", sat, i + 1, no);
                    return -1;
                }
                //C++ TO C# CONVERTER TODO TASK: The memory management function 'memcpy' has no equivalent in C#:
                memcpy(raw.subfrm[sat - 1] + 10 * i, str, 10);
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
        /* decode crescent raw message -----------------------------------------------*/
        internal static int decode_cres(raw_t raw)
        {
            int type = GlobalMembersBinex.U2(raw.buff + 4);

            GlobalMembersRtkcmn.trace(3, "decode_cres: type=%2d len=%d\n", type, raw.len);

            if (GlobalMembersCrescent.chksum(raw.buff, raw.len) == 0)
            {
                GlobalMembersRtkcmn.trace(2, "crescent checksum error: type=%2d len=%d\n", type, raw.len);
                return -1;
            }
            if (raw.outtype != 0)
            {
                raw.msgtype = string.Format("HEMIS {0,2:D} ({1,4:D}):", type, raw.len);
            }
            switch (type)
            {
                case DefineConstants.ID_CRESPOS:
                    return GlobalMembersCrescent.decode_crespos(raw);
                case DefineConstants.ID_CRESRAW:
                    return GlobalMembersCrescent.decode_cresraw(raw);
                case DefineConstants.ID_CRESRAW2:
                    return GlobalMembersCrescent.decode_cresraw2(raw);
                case DefineConstants.ID_CRESEPH:
                    return GlobalMembersCrescent.decode_creseph(raw);
                case DefineConstants.ID_CRESWAAS:
                    return GlobalMembersCrescent.decode_creswaas(raw);
                case DefineConstants.ID_CRESIONUTC:
                    return GlobalMembersCrescent.decode_cresionutc(raw);
                case DefineConstants.ID_CRESGLORAW:
                    return GlobalMembersCrescent.decode_cresgloraw(raw);
                case DefineConstants.ID_CRESGLOEPH:
                    return GlobalMembersCrescent.decode_cresgloeph(raw);
            }
            return 0;
        }
        /* sync code -----------------------------------------------------------------*/
        internal static int sync_cres(byte[] buff, byte data)
        {
            buff[0] = buff[1];
            buff[1] = buff[2];
            buff[2] = buff[3];
            buff[3] = data;
            return buff[0] == DefineConstants.CRESSYNC[0] && buff[1] == DefineConstants.CRESSYNC[1] && buff[2] == DefineConstants.CRESSYNC[2] && buff[3] == DefineConstants.CRESSYNC[3];
        }
        /* input cresent raw message ---------------------------------------------------
        * input next crescent raw message from stream
        * args   : raw_t *raw   IO     receiver raw data control struct
        *          unsigned char data I stream data (1 byte)
        * return : status (-1: error message, 0: no message, 1: input observation data,
        *                  2: input ephemeris, 3: input sbas message,
        *                  9: input ion/utc parameter)
        *
        * notes  : to specify input options, set raw->opt to the following option
        *          strings separated by spaces.
        *
        *          -EPHALL      : input all ephemerides
        *          -TTCORR      : time-tag offset correction
        *          -ENAGLO      : enable glonass messages
        *
        *-----------------------------------------------------------------------------*/
        public static int input_cres(raw_t raw, byte data)
        {
            GlobalMembersRtkcmn.trace(5, "input_cres: data=%02x\n", data);

            /* synchronize frame */
            if (raw.nbyte == 0)
            {
                if (GlobalMembersCrescent.sync_cres(raw.buff, data) == 0)
                {
                    return 0;
                }
                raw.nbyte = 4;
                return 0;
            }
            raw.buff[raw.nbyte++] = data;

            if (raw.nbyte == 8)
            {
                if ((raw.len = GlobalMembersBinex.U2(raw.buff + 6) + 12) > DefineConstants.MAXRAWLEN)
                {
                    GlobalMembersRtkcmn.trace(2, "cresent length error: len=%d\n", raw.len);
                    raw.nbyte = 0;
                    return -1;
                }
            }
            if (raw.nbyte < 8 || raw.nbyte < raw.len)
            {
                return 0;
            }
            raw.nbyte = 0;

            /* decode crescent raw message */
            return GlobalMembersCrescent.decode_cres(raw);
        }
        /* input crescent raw message from file ----------------------------------------
        * input next crescent raw message from file
        * args   : raw_t  *raw   IO     receiver raw data control struct
        *          FILE   *fp    I      file pointer
        * return : status(-2: end of file, -1...9: same as above)
        *-----------------------------------------------------------------------------*/
        public static int input_cresf(raw_t raw, FILE fp)
        {
            int i;
            int data;

            GlobalMembersRtkcmn.trace(4, "input_cresf:\n");

            /* synchronize frame */
            if (raw.nbyte == 0)
            {
                for (i = 0; ; i++)
                {
                    if ((data = fgetc(fp)) == EOF)
                    {
                        return -2;
                    }
                    if (GlobalMembersCrescent.sync_cres(raw.buff, (byte)data) != 0)
                        break;
                    if (i >= 4096)
                    {
                        return 0;
                    }
                }
            }
            if (fread(raw.buff + 4, 1, 4, fp) < 4)
            {
                return -2;
            }
            raw.nbyte = 8;

            if ((raw.len = GlobalMembersBinex.U2(raw.buff + 6) + 12) > DefineConstants.MAXRAWLEN)
            {
                GlobalMembersRtkcmn.trace(2, "crescent length error: len=%d\n", raw.len);
                raw.nbyte = 0;
                return -1;
            }
            if (fread(raw.buff + 8, 1, raw.len - 8, fp) < (uint)(raw.len - 8))
            {
                return -2;
            }
            raw.nbyte = 0;

            /* decode crescent raw message */
            return GlobalMembersCrescent.decode_cres(raw);
        }
    }
}
