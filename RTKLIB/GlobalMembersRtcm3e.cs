using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ghGPS.Classes
{
    public static class GlobalMembersRtcm3e
    {
        /*------------------------------------------------------------------------------
        * rtcm3e.c : rtcm ver.3 message encoder functions
        *
        *          Copyright (C) 2012-2015 by T.TAKASU, All rights reserved.
        *
        * options :
        *     -DSSR_QZSS_DRAFT_V05: qzss ssr messages based on ref [16]
        *
        * references :
        *     see rtcm.c
        *
        * version : $Revision:$ $Date:$
        * history : 2012/12/05 1.0  new
        *           2012/12/16 1.1  fix bug on ssr high rate clock correction
        *           2012/12/24 1.2  fix bug on msm carrier-phase offset correction
        *                           fix bug on SBAS sat id in 1001-1004
        *                           fix bug on carrier-phase in 1001-1004,1009-1012
        *           2012/12/28 1.3  fix bug on compass carrier wave length
        *           2013/01/18 1.4  fix bug on ssr message generation
        *           2013/05/11 1.5  change type of arg value of setbig()
        *           2013/05/19 1.5  gpst -> bdt of time-tag in beidou msm message
        *           2013/04/27 1.7  comply with rtcm 3.2 with amendment 1/2 (ref[15])
        *                           delete MT 1046 according to ref [15]
        *           2014/05/15 1.8  set NT field in MT 1020 glonass ephemeris
        *           2014/12/06 1.9  support SBAS/BeiDou SSR messages (ref [16])
        *                           fix bug on invalid staid in qzss ssr messages
        *           2015/03/22 1.9  add handling of iodcrc for beidou/sbas ssr messages
        *-----------------------------------------------------------------------------*/


        internal const string rcsid = "$Id:$";
             

        /* ssr update intervals ------------------------------------------------------*/
        internal readonly double[] ssrudint = { 1, 2, 5, 10, 15, 30, 60, 120, 240, 300, 600, 900, 1800, 3600, 7200, 10800 };
        /* set sign-magnitude bits ---------------------------------------------------*/
        internal static void setbitg(ref byte buff, int pos, int len, int value)
        {
            GlobalMembersRtkcmn.setbitu(buff, pos, 1, value < 0 ? 1 : 0);
            GlobalMembersRtkcmn.setbitu(buff, pos + 1, len - 1, value < 0 ? -value : value);
        }
        /* set signed 38 bit field ---------------------------------------------------*/
        internal static void set38bits(ref byte buff, int pos, double value)
        {
            int word_h = (int)Math.Floor(value / 64.0);
            uint word_l = (uint)(value - word_h * 64.0);
            GlobalMembersRtkcmn.setbits(ref buff, pos, 32, word_h);
            GlobalMembersRtkcmn.setbitu(buff, pos + 32, 6, word_l);
        }
        /* lock time -----------------------------------------------------------------*/
        internal static int locktime(gtime_t time, gtime_t lltime, byte LLI)
        {
            if (lltime.time == null || (LLI & 1))
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                //ORIGINAL LINE: *lltime=time;
                lltime.CopyFrom(time);
            }
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: return (int)timediff(time,*lltime);
            return (int)GlobalMembersRtkcmn.timediff(new gtime_t(time), lltime);
        }
        /* glonass frequency channel number in rtcm (0-13,-1:error) ------------------*/
        internal static int fcn_glo(int sat, rtcm_t rtcm)
        {
            int prn;
            if (GlobalMembersRtkcmn.satsys(sat, ref prn) != DefineConstants.SYS_GLO || rtcm.nav.geph[prn - 1].sat != sat)
            {
                return -1;
            }
            return rtcm.nav.geph[prn - 1].frq + 7;
        }
        /* lock time indicator (ref [2] table 3.4-2) ---------------------------------*/
        internal static int to_lock(int @lock)
        {
            if (@lock < 0)
            {
                return 0;
            }
            if (@lock < 24)
            {
                return @lock;
            }
            if (@lock < 72)
            {
                return (@lock + 24) / 2;
            }
            if (@lock < 168)
            {
                return (@lock + 120) / 4;
            }
            if (@lock < 360)
            {
                return (@lock + 408) / 8;
            }
            if (@lock < 744)
            {
                return (@lock + 1176) / 16;
            }
            if (@lock < 937)
            {
                return (@lock + 3096) / 32;
            }
            return 127;
        }
        /* msm lock time indicator (ref [11] table 3.4-1D) ---------------------------*/
        internal static int to_msm_lock(int @lock)
        {
            if (@lock < 32)
            {
                return 0;
            }
            if (@lock < 64)
            {
                return 1;
            }
            if (@lock < 128)
            {
                return 2;
            }
            if (@lock < 256)
            {
                return 3;
            }
            if (@lock < 512)
            {
                return 4;
            }
            if (@lock < 1024)
            {
                return 5;
            }
            if (@lock < 2048)
            {
                return 6;
            }
            if (@lock < 4096)
            {
                return 7;
            }
            if (@lock < 8192)
            {
                return 8;
            }
            if (@lock < 16384)
            {
                return 9;
            }
            if (@lock < 32768)
            {
                return 10;
            }
            if (@lock < 65536)
            {
                return 11;
            }
            if (@lock < 131072)
            {
                return 12;
            }
            if (@lock < 262144)
            {
                return 13;
            }
            if (@lock < 524288)
            {
                return 14;
            }
            return 15;
        }
        /* msm lock time indicator with extended-resolution (ref [11] table 3.4-1E) --*/
        internal static int to_msm_lock_ex(int @lock)
        {
            if (@lock < 0)
            {
                return 0;
            }
            if (@lock < 64)
            {
                return @lock;
            }
            if (@lock < 128)
            {
                return (@lock + 64) / 2;
            }
            if (@lock < 256)
            {
                return (@lock + 256) / 4;
            }
            if (@lock < 512)
            {
                return (@lock + 768) / 8;
            }
            if (@lock < 1024)
            {
                return (@lock + 2048) / 16;
            }
            if (@lock < 2048)
            {
                return (@lock + 5120) / 32;
            }
            if (@lock < 4096)
            {
                return (@lock + 12288) / 64;
            }
            if (@lock < 8192)
            {
                return (@lock + 28672) / 128;
            }
            if (@lock < 16384)
            {
                return (@lock + 65536) / 256;
            }
            if (@lock < 32768)
            {
                return (@lock + 147456) / 512;
            }
            if (@lock < 65536)
            {
                return (@lock + 327680) / 1024;
            }
            if (@lock < 131072)
            {
                return (@lock + 720896) / 2048;
            }
            if (@lock < 262144)
            {
                return (@lock + 1572864) / 4096;
            }
            if (@lock < 524288)
            {
                return (@lock + 3407872) / 8192;
            }
            if (@lock < 1048576)
            {
                return (@lock + 7340032) / 16384;
            }
            if (@lock < 2097152)
            {
                return (@lock + 15728640) / 32768;
            }
            if (@lock < 4194304)
            {
                return (@lock + 33554432) / 65536;
            }
            if (@lock < 8388608)
            {
                return (@lock + 71303168) / 131072;
            }
            if (@lock < 16777216)
            {
                return (@lock + 150994944) / 262144;
            }
            if (@lock < 33554432)
            {
                return (@lock + 318767104) / 524288;
            }
            if (@lock < 67108864)
            {
                return (@lock + 671088640) / 1048576;
            }
            return 704;
        }
        /* L1 code indicator gps -----------------------------------------------------*/
        internal static int to_code1_gps(byte code)
        {
            switch (code)
            {
                case DefineConstants.CODE_L1C: // L1 C/A
                    return 0;
                case DefineConstants.CODE_L1P:
                case DefineConstants.CODE_L1W:
                case DefineConstants.CODE_L1Y:
                case DefineConstants.CODE_L1N: // L1 P(Y) direct
                    return 1;
            }
            return 0;
        }
        /* L2 code indicator gps -----------------------------------------------------*/
        internal static int to_code2_gps(byte code)
        {
            switch (code)
            {
                case DefineConstants.CODE_L2C:
                case DefineConstants.CODE_L2S:
                case DefineConstants.CODE_L2L:
                case DefineConstants.CODE_L2X: // L2 C/A or L2C
                    return 0;
                case DefineConstants.CODE_L2P:
                case DefineConstants.CODE_L2Y: // L2 P(Y) direct
                    return 1;
                case DefineConstants.CODE_L2D: // L2 P(Y) cross-correlated
                    return 2;
                case DefineConstants.CODE_L2W:
                case DefineConstants.CODE_L2N: // L2 correlated P/Y
                    return 3;
            }
            return 0;
        }
        /* L1 code indicator glonass -------------------------------------------------*/
        internal static int to_code1_glo(byte code)
        {
            switch (code)
            {
                case DefineConstants.CODE_L1C: // L1 C/A
                    return 0;
                case DefineConstants.CODE_L1P: // L1 P
                    return 1;
            }
            return 0;
        }
        /* L2 code indicator glonass -------------------------------------------------*/
        internal static int to_code2_glo(byte code)
        {
            switch (code)
            {
                case DefineConstants.CODE_L2C: // L2 C/A
                    return 0;
                case DefineConstants.CODE_L2P: // L2 P
                    return 1;
            }
            return 0;
        }
        /* carrier-phase - pseudorange in cycle --------------------------------------*/
        internal static double cp_pr(double cp, double pr_cyc)
        {
            return Math.IEEERemainder(cp - pr_cyc + 1500.0, 3000.0) - 1500.0;
        }
        /* generate obs field data gps -----------------------------------------------*/
        internal static void gen_obs_gps(rtcm_t rtcm, obsd_t data, ref int code1, ref int pr1, ref int ppr1, ref int lock1, ref int amb, ref int cnr1, ref int code2, ref int pr21, ref int ppr2, ref int lock2, ref int cnr2)
        {
            double lam1;
            double lam2;
            double pr1c = 0.0;
            double ppr;
            int lt1;
            int lt2;

            lam1 = DefineConstants.CLIGHT / DefineConstants.FREQ1;
            lam2 = DefineConstants.CLIGHT / DefineConstants.FREQ2;
            pr1 = amb = 0;
            if (ppr1 != 0) // invalid values
            {
                ppr1 = 0xFFF80000;
            }
            if (pr21 != 0)
            {
                pr21 = 0xFFFFE000;
            }
            if (ppr2 != 0)
            {
                ppr2 = 0xFFF80000;
            }

            /* L1 peudorange */
            if (data.P[0] != 0.0 && data.code[0] != 0)
            {
                amb = (int)Math.Floor(data.P[0] / DefineConstants.PRUNIT_GPS);
                pr1 = ((int)Math.Floor(((data.P[0] - amb * DefineConstants.PRUNIT_GPS) / 0.02) + 0.5));
                pr1c = pr1 * 0.02 + amb * DefineConstants.PRUNIT_GPS;
            }
            /* L1 phaserange - L1 pseudorange */
            if (data.P[0] != 0.0 && data.L[0] != 0.0 && data.code[0] != 0)
            {
                ppr = GlobalMembersRtcm3e.cp_pr(data.L[0], pr1c / lam1);
                if (ppr1 != 0)
                {
                    ppr1 = ((int)Math.Floor((ppr * lam1 / 0.0005) + 0.5));
                }
            }
            /* L2 -L1 pseudorange */
            if (data.P[0] != 0.0 && data.P[1] != 0.0 && data.code[0] != 0 && data.code[1] != 0 && Math.Abs(data.P[1] - pr1c) <= 163.82)
            {
                if (pr21 != 0)
                {
                    pr21 = ((int)Math.Floor(((data.P[1] - pr1c) / 0.02) + 0.5));
                }
            }
            /* L2 phaserange - L1 pseudorange */
            if (data.P[0] != 0.0 && data.L[1] != 0.0 && data.code[0] != 0 && data.code[1] != 0)
            {
                ppr = GlobalMembersRtcm3e.cp_pr(data.L[1], pr1c / lam2);
                if (ppr2 != 0)
                {
                    ppr2 = ((int)Math.Floor((ppr * lam2 / 0.0005) + 0.5));
                }
            }
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: lt1=locktime(data->time,rtcm->lltime[data->sat-1],data->LLI[0]);
            lt1 = GlobalMembersRtcm3e.locktime(new gtime_t(data.time), new gtime_t(rtcm.lltime[data.sat - 1]), data.LLI[0]);
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: lt2=locktime(data->time,rtcm->lltime[data->sat-1]+1,data->LLI[1]);
            lt2 = GlobalMembersRtcm3e.locktime(new gtime_t(data.time), rtcm.lltime[data.sat - 1] + 1, data.LLI[1]);

            if (lock1 != 0)
            {
                lock1 = GlobalMembersRtcm3e.to_lock(lt1);
            }
            if (lock2 != 0)
            {
                lock2 = GlobalMembersRtcm3e.to_lock(lt2);
            }
            if (cnr1 != 0)
            {
                cnr1 = data.SNR[0];
            }
            if (cnr2 != 0)
            {
                cnr2 = data.SNR[1];
            }
            if (code1 != 0)
            {
                code1 = GlobalMembersRtcm3e.to_code1_gps(data.code[0]);
            }
            if (code2 != 0)
            {
                code2 = GlobalMembersRtcm3e.to_code2_gps(data.code[1]);
            }
        }
        /* generate obs field data glonass -------------------------------------------*/
        internal static void gen_obs_glo(rtcm_t rtcm, obsd_t data, int fcn, ref int code1, ref int pr1, ref int ppr1, ref int lock1, ref int amb, ref int cnr1, ref int code2, ref int pr21, ref int ppr2, ref int lock2, ref int cnr2)
        {
            double lam1 = 0.0;
            double lam2 = 0.0;
            double pr1c = 0.0;
            double ppr;
            int lt1;
            int lt2;

            if (fcn >= 0)
            {
                lam1 = DefineConstants.CLIGHT / (DefineConstants.FREQ1_GLO + DefineConstants.DFRQ1_GLO * (fcn - 7));
                lam2 = DefineConstants.CLIGHT / (DefineConstants.FREQ2_GLO + DefineConstants.DFRQ2_GLO * (fcn - 7));
            }
            pr1 = amb = 0;
            if (ppr1 != 0) // invalid values
            {
                ppr1 = 0xFFF80000;
            }
            if (pr21 != 0)
            {
                pr21 = 0xFFFFE000;
            }
            if (ppr2 != 0)
            {
                ppr2 = 0xFFF80000;
            }

            /* L1 peudorange */
            if (data.P[0] != 0.0)
            {
                amb = (int)Math.Floor(data.P[0] / DefineConstants.PRUNIT_GLO);
                pr1 = ((int)Math.Floor(((data.P[0] - amb * DefineConstants.PRUNIT_GLO) / 0.02) + 0.5));
                pr1c = pr1 * 0.02 + amb * DefineConstants.PRUNIT_GLO;
            }
            /* L1 phaserange - L1 pseudorange */
            if (data.P[0] != 0.0 && data.L[0] != 0.0 && data.code[0] != 0 && lam1 > 0.0)
            {
                ppr = GlobalMembersRtcm3e.cp_pr(data.L[0], pr1c / lam1);
                if (ppr1 != 0)
                {
                    ppr1 = ((int)Math.Floor((ppr * lam1 / 0.0005) + 0.5));
                }
            }
            /* L2 -L1 pseudorange */
            if (data.P[0] != 0.0 && data.P[1] != 0.0 && data.code[0] != 0 && data.code[1] != 0 && Math.Abs(data.P[1] - pr1c) <= 163.82)
            {
                if (pr21 != 0)
                {
                    pr21 = ((int)Math.Floor(((data.P[1] - pr1c) / 0.02) + 0.5));
                }
            }
            /* L2 phaserange - L1 pseudorange */
            if (data.P[0] != 0.0 && data.L[1] != 0.0 && data.code[0] != 0 && data.code[1] != 0 && lam2 > 0.0)
            {
                ppr = GlobalMembersRtcm3e.cp_pr(data.L[1], pr1c / lam2);
                if (ppr2 != 0)
                {
                    ppr2 = ((int)Math.Floor((ppr * lam2 / 0.0005) + 0.5));
                }
            }
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: lt1=locktime(data->time,rtcm->lltime[data->sat-1],data->LLI[0]);
            lt1 = GlobalMembersRtcm3e.locktime(new gtime_t(data.time), new gtime_t(rtcm.lltime[data.sat - 1]), data.LLI[0]);
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: lt2=locktime(data->time,rtcm->lltime[data->sat-1]+1,data->LLI[1]);
            lt2 = GlobalMembersRtcm3e.locktime(new gtime_t(data.time), rtcm.lltime[data.sat - 1] + 1, data.LLI[1]);

            if (lock1 != 0)
            {
                lock1 = GlobalMembersRtcm3e.to_lock(lt1);
            }
            if (lock2 != 0)
            {
                lock2 = GlobalMembersRtcm3e.to_lock(lt2);
            }
            if (cnr1 != 0)
            {
                cnr1 = data.SNR[0];
            }
            if (cnr2 != 0)
            {
                cnr2 = data.SNR[1];
            }
            if (code1 != 0)
            {
                code1 = GlobalMembersRtcm3e.to_code1_glo(data.code[0]);
            }
            if (code2 != 0)
            {
                code2 = GlobalMembersRtcm3e.to_code2_glo(data.code[1]);
            }
        }
        /* encode rtcm header --------------------------------------------------------*/
        internal static int encode_head(int type, rtcm_t rtcm, int sys, int sync, int nsat)
        {
            double tow;
            int i = 24;
            int week;
            int epoch;

            GlobalMembersRtkcmn.trace(4, "encode_head: type=%d sync=%d sys=%d nsat=%d\n", type, sync, sys, nsat);

            GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 12, type); // message no
            i += 12;
            GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 12, rtcm.staid); // ref station id
            i += 12;

            if (sys == DefineConstants.SYS_GLO)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: tow=time2gpst(timeadd(gpst2utc(rtcm->time),10800.0),&week);
                tow = GlobalMembersRtkcmn.time2gpst(GlobalMembersRtkcmn.timeadd(GlobalMembersRtkcmn.gpst2utc(new gtime_t(rtcm.time)), 10800.0), ref week);
                epoch = ((int)Math.Floor((Math.IEEERemainder(tow, 86400.0) / 0.001) + 0.5));
                GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 27, epoch); // glonass epoch time
                i += 27;
            }
            else
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: tow=time2gpst(rtcm->time,&week);
                tow = GlobalMembersRtkcmn.time2gpst(new gtime_t(rtcm.time), ref week);
                epoch = ((int)Math.Floor((tow / 0.001) + 0.5));
                GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 30, epoch); // gps epoch time
                i += 30;
            }
            GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 1, sync); // synchronous gnss flag
            i += 1;
            GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 5, nsat); // no of satellites
            i += 5;
            GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 1, 0); // smoothing indicator
            i += 1;
            GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 3, 0); // smoothing interval
            i += 3;
            return i;
        }
        /* encode type 1001: basic L1-only gps rtk observables -----------------------*/
        internal static int encode_type1001(rtcm_t rtcm, int sync)
        {
            int i;
            int j;
            int nsat = 0;
            int sys;
            int prn;
            int code1;
            int pr1;
            int ppr1;
            int lock1;
            int amb;

            GlobalMembersRtkcmn.trace(3, "encode_type1001: sync=%d\n", sync);

            for (j = 0; j < rtcm.obs.n && nsat < DefineConstants.MAXOBS; j++)
            {
                sys = GlobalMembersRtkcmn.satsys(rtcm.obs.data[j].sat, ref prn);
                if (!(sys & (DefineConstants.SYS_GPS | DefineConstants.SYS_SBS)))
                    continue;
                nsat++;
            }
            /* encode header */
            i = GlobalMembersRtcm3e.encode_head(1001, rtcm, DefineConstants.SYS_GPS, sync, nsat);

            for (j = 0; j < rtcm.obs.n && nsat < DefineConstants.MAXOBS; j++)
            {
                sys = GlobalMembersRtkcmn.satsys(rtcm.obs.data[j].sat, ref prn);
                if (!(sys & (DefineConstants.SYS_GPS | DefineConstants.SYS_SBS)))
                    continue;

                if (sys == DefineConstants.SYS_SBS) // 40-58: sbas 120-138
                {
                    prn -= 80;
                }

                /* generate obs field data gps */
                GlobalMembersRtcm3e.gen_obs_gps(rtcm, rtcm.obs.data + j, ref code1, ref pr1, ref ppr1, ref lock1, ref amb, null, null, null, null, null, null);

                GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 6, prn);
                i += 6;
                GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 1, code1);
                i += 1;
                GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 24, pr1);
                i += 24;
                GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 20, ppr1);
                i += 20;
                GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 7, lock1);
                i += 7;
            }
            rtcm.nbit = i;
            return 1;
        }
        /* encode type 1002: extended L1-only gps rtk observables --------------------*/
        internal static int encode_type1002(rtcm_t rtcm, int sync)
        {
            int i;
            int j;
            int nsat = 0;
            int sys;
            int prn;
            int code1;
            int pr1;
            int ppr1;
            int lock1;
            int amb;
            int cnr1;

            GlobalMembersRtkcmn.trace(3, "encode_type1002: sync=%d\n", sync);

            for (j = 0; j < rtcm.obs.n && nsat < DefineConstants.MAXOBS; j++)
            {
                sys = GlobalMembersRtkcmn.satsys(rtcm.obs.data[j].sat, ref prn);
                if (!(sys & (DefineConstants.SYS_GPS | DefineConstants.SYS_SBS)))
                    continue;
                nsat++;
            }
            /* encode header */
            i = GlobalMembersRtcm3e.encode_head(1002, rtcm, DefineConstants.SYS_GPS, sync, nsat);

            for (j = 0; j < rtcm.obs.n && nsat < DefineConstants.MAXOBS; j++)
            {
                sys = GlobalMembersRtkcmn.satsys(rtcm.obs.data[j].sat, ref prn);
                if (!(sys & (DefineConstants.SYS_GPS | DefineConstants.SYS_SBS)))
                    continue;

                if (sys == DefineConstants.SYS_SBS) // 40-58: sbas 120-138
                {
                    prn -= 80;
                }

                /* generate obs field data gps */
                GlobalMembersRtcm3e.gen_obs_gps(rtcm, rtcm.obs.data + j, ref code1, ref pr1, ref ppr1, ref lock1, ref amb, ref cnr1, null, null, null, null, null);

                GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 6, prn);
                i += 6;
                GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 1, code1);
                i += 1;
                GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 24, pr1);
                i += 24;
                GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 20, ppr1);
                i += 20;
                GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 7, lock1);
                i += 7;
                GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 8, amb);
                i += 8;
                GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 8, cnr1);
                i += 8;
            }
            rtcm.nbit = i;
            return 1;
        }
        /* encode type 1003: basic L1&L2 gps rtk observables -------------------------*/
        internal static int encode_type1003(rtcm_t rtcm, int sync)
        {
            int i;
            int j;
            int nsat = 0;
            int sys;
            int prn;
            int code1;
            int pr1;
            int ppr1;
            int lock1;
            int amb;
            int code2;
            int pr21;
            int ppr2;
            int lock2;

            GlobalMembersRtkcmn.trace(3, "encode_type1003: sync=%d\n", sync);

            for (j = 0; j < rtcm.obs.n && nsat < DefineConstants.MAXOBS; j++)
            {
                sys = GlobalMembersRtkcmn.satsys(rtcm.obs.data[j].sat, ref prn);
                if (!(sys & (DefineConstants.SYS_GPS | DefineConstants.SYS_SBS)))
                    continue;
                nsat++;
            }
            /* encode header */
            i = GlobalMembersRtcm3e.encode_head(1003, rtcm, DefineConstants.SYS_GPS, sync, nsat);

            for (j = 0; j < rtcm.obs.n && nsat < DefineConstants.MAXOBS; j++)
            {
                sys = GlobalMembersRtkcmn.satsys(rtcm.obs.data[j].sat, ref prn);
                if (!(sys & (DefineConstants.SYS_GPS | DefineConstants.SYS_SBS)))
                    continue;

                if (sys == DefineConstants.SYS_SBS) // 40-58: sbas 120-138
                {
                    prn -= 80;
                }

                /* generate obs field data gps */
                GlobalMembersRtcm3e.gen_obs_gps(rtcm, rtcm.obs.data + j, ref code1, ref pr1, ref ppr1, ref lock1, ref amb, null, ref code2, ref pr21, ref ppr2, ref lock2, null);

                GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 6, prn);
                i += 6;
                GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 1, code1);
                i += 1;
                GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 24, pr1);
                i += 24;
                GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 20, ppr1);
                i += 20;
                GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 7, lock1);
                i += 7;
                GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 2, code2);
                i += 2;
                GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 14, pr21);
                i += 14;
                GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 20, ppr2);
                i += 20;
                GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 7, lock2);
                i += 7;
            }
            rtcm.nbit = i;
            return 1;
        }
        /* encode type 1004: extended L1&L2 gps rtk observables ----------------------*/
        internal static int encode_type1004(rtcm_t rtcm, int sync)
        {
            int i;
            int j;
            int nsat = 0;
            int sys;
            int prn;
            int code1;
            int pr1;
            int ppr1;
            int lock1;
            int amb;
            int cnr1;
            int code2;
            int pr21;
            int ppr2;
            int lock2;
            int cnr2;

            GlobalMembersRtkcmn.trace(3, "encode_type1004: sync=%d\n", sync);

            for (j = 0; j < rtcm.obs.n && nsat < DefineConstants.MAXOBS; j++)
            {
                sys = GlobalMembersRtkcmn.satsys(rtcm.obs.data[j].sat, ref prn);
                if (!(sys & (DefineConstants.SYS_GPS | DefineConstants.SYS_SBS)))
                    continue;
                nsat++;
            }
            /* encode header */
            i = GlobalMembersRtcm3e.encode_head(1004, rtcm, DefineConstants.SYS_GPS, sync, nsat);

            for (j = 0; j < rtcm.obs.n && nsat < DefineConstants.MAXOBS; j++)
            {
                sys = GlobalMembersRtkcmn.satsys(rtcm.obs.data[j].sat, ref prn);
                if (!(sys & (DefineConstants.SYS_GPS | DefineConstants.SYS_SBS)))
                    continue;

                if (sys == DefineConstants.SYS_SBS) // 40-58: sbas 120-138
                {
                    prn -= 80;
                }

                /* generate obs field data gps */
                GlobalMembersRtcm3e.gen_obs_gps(rtcm, rtcm.obs.data + j, ref code1, ref pr1, ref ppr1, ref lock1, ref amb, ref cnr1, ref code2, ref pr21, ref ppr2, ref lock2, ref cnr2);

                GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 6, prn);
                i += 6;
                GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 1, code1);
                i += 1;
                GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 24, pr1);
                i += 24;
                GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 20, ppr1);
                i += 20;
                GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 7, lock1);
                i += 7;
                GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 8, amb);
                i += 8;
                GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 8, cnr1);
                i += 8;
                GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 2, code2);
                i += 2;
                GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 14, pr21);
                i += 14;
                GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 20, ppr2);
                i += 20;
                GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 7, lock2);
                i += 7;
                GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 8, cnr2);
                i += 8;
            }
            rtcm.nbit = i;
            return 1;
        }
        /* encode type 1005: stationary rtk reference station arp --------------------*/
        internal static int encode_type1005(rtcm_t rtcm, int sync)
        {
            double[] p = rtcm.sta.pos;
            int i = 24;

            GlobalMembersRtkcmn.trace(3, "encode_type1005: sync=%d\n", sync);

            GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 12, 1005); // message no
            i += 12;
            GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 12, rtcm.staid); // ref station id
            i += 12;
            GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 6, 0); // itrf realization year
            i += 6;
            GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 1, 1); // gps indicator
            i += 1;
            GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 1, 1); // glonass indicator
            i += 1;
            GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 1, 0); // galileo indicator
            i += 1;
            GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 1, 0); // ref station indicator
            i += 1;
            GlobalMembersRtcm3e.set38bits(ref rtcm.buff, i, p[0] / 0.0001); // antenna ref point ecef-x
            i += 38;
            GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 1, 1); // oscillator indicator
            i += 1;
            GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 1, 0); // reserved
            i += 1;
            GlobalMembersRtcm3e.set38bits(ref rtcm.buff, i, p[1] / 0.0001); // antenna ref point ecef-y
            i += 38;
            GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 2, 0); // quarter cycle indicator
            i += 2;
            GlobalMembersRtcm3e.set38bits(ref rtcm.buff, i, p[2] / 0.0001); // antenna ref point ecef-z
            i += 38;
            rtcm.nbit = i;
            return 1;
        }
        /* encode type 1006: stationary rtk reference station arp with height --------*/
        internal static int encode_type1006(rtcm_t rtcm, int sync)
        {
            double[] p = rtcm.sta.pos;
            int i = 24;
            int hgt = 0;

            GlobalMembersRtkcmn.trace(3, "encode_type1006: sync=%d\n", sync);

            if (0.0 <= rtcm.sta.hgt && rtcm.sta.hgt <= 6.5535)
            {
                hgt = ((int)Math.Floor((rtcm.sta.hgt / 0.0001) + 0.5));
            }
            else
            {
                GlobalMembersRtkcmn.trace(2, "antenna height error: h=%.4f\n", rtcm.sta.hgt);
            }
            GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 12, 1006); // message no
            i += 12;
            GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 12, rtcm.staid); // ref station id
            i += 12;
            GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 6, 0); // itrf realization year
            i += 6;
            GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 1, 1); // gps indicator
            i += 1;
            GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 1, 1); // glonass indicator
            i += 1;
            GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 1, 0); // galileo indicator
            i += 1;
            GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 1, 0); // ref station indicator
            i += 1;
            GlobalMembersRtcm3e.set38bits(ref rtcm.buff, i, p[0] / 0.0001); // antenna ref point ecef-x
            i += 38;
            GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 1, 1); // oscillator indicator
            i += 1;
            GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 1, 0); // reserved
            i += 1;
            GlobalMembersRtcm3e.set38bits(ref rtcm.buff, i, p[1] / 0.0001); // antenna ref point ecef-y
            i += 38;
            GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 2, 0); // quarter cycle indicator
            i += 2;
            GlobalMembersRtcm3e.set38bits(ref rtcm.buff, i, p[2] / 0.0001); // antenna ref point ecef-z
            i += 38;
            GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 16, hgt); // antenna height
            i += 16;
            rtcm.nbit = i;
            return 1;
        }
        /* encode type 1007: antenna descriptor --------------------------------------*/
        internal static int encode_type1007(rtcm_t rtcm, int sync)
        {
            int i = 24;
            int j;
            int antsetup = rtcm.sta.antsetup;
            int n = ((rtcm.sta.antdes.Length) < (31) ? (rtcm.sta.antdes.Length) : (31));

            GlobalMembersRtkcmn.trace(3, "encode_type1007: sync=%d\n", sync);

            GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 12, 1007); // message no
            i += 12;
            GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 12, rtcm.staid); // ref station id
            i += 12;

            /* antenna descriptor */
            GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 8, n);
            i += 8;
            for (j = 0; j < n; j++)
            {
                GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 8, rtcm.sta.antdes[j]);
                i += 8;
            }
            GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 8, antsetup); // antetnna setup id
            i += 8;
            rtcm.nbit = i;
            return 1;
        }
        /* encode type 1008: antenna descriptor & serial number ----------------------*/
        internal static int encode_type1008(rtcm_t rtcm, int sync)
        {
            int i = 24;
            int j;
            int antsetup = rtcm.sta.antsetup;
            int n = ((rtcm.sta.antdes.Length) < (31) ? (rtcm.sta.antdes.Length) : (31));
            int m = ((rtcm.sta.antsno.Length) < (31) ? (rtcm.sta.antsno.Length) : (31));

            GlobalMembersRtkcmn.trace(3, "encode_type1008: sync=%d\n", sync);

            GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 12, 1008); // message no
            i += 12;
            GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 12, rtcm.staid); // ref station id
            i += 12;

            /* antenna descriptor */
            GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 8, n);
            i += 8;
            for (j = 0; j < n; j++)
            {
                GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 8, rtcm.sta.antdes[j]);
                i += 8;
            }
            GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 8, antsetup); // antenna setup id
            i += 8;

            /* antenna serial number */
            GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 8, m);
            i += 8;
            for (j = 0; j < m; j++)
            {
                GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 8, rtcm.sta.antsno[j]);
                i += 8;
            }
            rtcm.nbit = i;
            return 1;
        }
        /* encode type 1009: basic L1-only glonass rtk observables -------------------*/
        internal static int encode_type1009(rtcm_t rtcm, int sync)
        {
            int i;
            int j;
            int nsat = 0;
            int sat;
            int prn;
            int fcn;
            int code1;
            int pr1;
            int ppr1;
            int lock1;
            int amb;

            for (j = 0; j < rtcm.obs.n && nsat < DefineConstants.MAXOBS; j++)
            {
                sat = rtcm.obs.data[j].sat;
                if (GlobalMembersRtkcmn.satsys(sat, ref prn) != DefineConstants.SYS_GLO)
                    continue;
                nsat++;
            }
            /* encode header */
            i = GlobalMembersRtcm3e.encode_head(1009, rtcm, DefineConstants.SYS_GLO, sync, nsat);

            for (j = 0; j < rtcm.obs.n && nsat < DefineConstants.MAXOBS; j++)
            {
                sat = rtcm.obs.data[j].sat;
                if (GlobalMembersRtkcmn.satsys(sat, ref prn) != DefineConstants.SYS_GLO)
                    continue;
                fcn = GlobalMembersRtcm3e.fcn_glo(sat, rtcm);

                /* generate obs field data glonass */
                GlobalMembersRtcm3e.gen_obs_glo(rtcm, rtcm.obs.data + j, fcn, ref code1, ref pr1, ref ppr1, ref lock1, ref amb, null, null, null, null, null, null);

                if (fcn < 0)
                {
                    fcn = 0;
                }
                GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 6, prn);
                i += 6;
                GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 1, code1);
                i += 1;
                GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 5, fcn);
                i += 5;
                GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 25, pr1);
                i += 25;
                GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 20, ppr1);
                i += 20;
                GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 7, lock1);
                i += 7;
            }
            rtcm.nbit = i;
            return 1;
        }
        /* encode type 1010: extended L1-only glonass rtk observables ----------------*/
        internal static int encode_type1010(rtcm_t rtcm, int sync)
        {
            int i;
            int j;
            int nsat = 0;
            int sat;
            int prn;
            int fcn;
            int code1;
            int pr1;
            int ppr1;
            int lock1;
            int amb;
            int cnr1;

            GlobalMembersRtkcmn.trace(3, "encode_type1010: sync=%d\n", sync);

            for (j = 0; j < rtcm.obs.n && nsat < DefineConstants.MAXOBS; j++)
            {
                sat = rtcm.obs.data[j].sat;
                if (GlobalMembersRtkcmn.satsys(sat, ref prn) != DefineConstants.SYS_GLO)
                    continue;
                nsat++;
            }
            /* encode header */
            i = GlobalMembersRtcm3e.encode_head(1010, rtcm, DefineConstants.SYS_GLO, sync, nsat);

            for (j = 0; j < rtcm.obs.n && nsat < DefineConstants.MAXOBS; j++)
            {
                sat = rtcm.obs.data[j].sat;
                if (GlobalMembersRtkcmn.satsys(sat, ref prn) != DefineConstants.SYS_GLO)
                    continue;
                fcn = GlobalMembersRtcm3e.fcn_glo(sat, rtcm);

                /* generate obs field data glonass */
                GlobalMembersRtcm3e.gen_obs_glo(rtcm, rtcm.obs.data + j, fcn, ref code1, ref pr1, ref ppr1, ref lock1, ref amb, ref cnr1, null, null, null, null, null);

                if (fcn < 0)
                {
                    fcn = 0;
                }
                GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 6, prn);
                i += 6;
                GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 1, code1);
                i += 1;
                GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 5, fcn);
                i += 5;
                GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 25, pr1);
                i += 25;
                GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 20, ppr1);
                i += 20;
                GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 7, lock1);
                i += 7;
                GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 7, amb);
                i += 7;
                GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 8, cnr1);
                i += 8;
            }
            rtcm.nbit = i;
            return 1;
        }
        /* encode type 1011: basic  L1&L2 glonass rtk observables --------------------*/
        internal static int encode_type1011(rtcm_t rtcm, int sync)
        {
            int i;
            int j;
            int nsat = 0;
            int sat;
            int prn;
            int fcn;
            int code1;
            int pr1;
            int ppr1;
            int lock1;
            int amb;
            int code2;
            int pr21;
            int ppr2;
            int lock2;

            GlobalMembersRtkcmn.trace(3, "encode_type1011: sync=%d\n", sync);

            for (j = 0; j < rtcm.obs.n && nsat < DefineConstants.MAXOBS; j++)
            {
                sat = rtcm.obs.data[j].sat;
                if (GlobalMembersRtkcmn.satsys(sat, ref prn) != DefineConstants.SYS_GLO)
                    continue;
                nsat++;
            }
            /* encode header */
            i = GlobalMembersRtcm3e.encode_head(1011, rtcm, DefineConstants.SYS_GLO, sync, nsat);

            for (j = 0; j < rtcm.obs.n && nsat < DefineConstants.MAXOBS; j++)
            {
                sat = rtcm.obs.data[j].sat;
                if (GlobalMembersRtkcmn.satsys(sat, ref prn) != DefineConstants.SYS_GLO)
                    continue;
                fcn = GlobalMembersRtcm3e.fcn_glo(sat, rtcm);

                /* generate obs field data glonass */
                GlobalMembersRtcm3e.gen_obs_glo(rtcm, rtcm.obs.data + j, fcn, ref code1, ref pr1, ref ppr1, ref lock1, ref amb, null, ref code2, ref pr21, ref ppr2, ref lock2, null);

                if (fcn < 0)
                {
                    fcn = 0;
                }
                GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 6, prn);
                i += 6;
                GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 1, code1);
                i += 1;
                GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 5, fcn);
                i += 5;
                GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 25, pr1);
                i += 25;
                GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 20, ppr1);
                i += 20;
                GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 7, lock1);
                i += 7;
                GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 2, code2);
                i += 2;
                GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 14, pr21);
                i += 14;
                GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 20, ppr2);
                i += 20;
                GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 7, lock2);
                i += 7;
            }
            rtcm.nbit = i;
            return 1;
        }
        /* encode type 1012: extended L1&L2 glonass rtk observables ------------------*/
        internal static int encode_type1012(rtcm_t rtcm, int sync)
        {
            int i;
            int j;
            int nsat = 0;
            int sat;
            int prn;
            int fcn;
            int code1;
            int pr1;
            int ppr1;
            int lock1;
            int amb;
            int cnr1;
            int code2;
            int pr21;
            int ppr2;
            int lock2;
            int cnr2;

            GlobalMembersRtkcmn.trace(3, "encode_type1012: sync=%d\n", sync);

            for (j = 0; j < rtcm.obs.n && nsat < DefineConstants.MAXOBS; j++)
            {
                sat = rtcm.obs.data[j].sat;
                if (GlobalMembersRtkcmn.satsys(sat, ref prn) != DefineConstants.SYS_GLO)
                    continue;
                nsat++;
            }
            /* encode header */
            i = GlobalMembersRtcm3e.encode_head(1012, rtcm, DefineConstants.SYS_GLO, sync, nsat);

            for (j = 0; j < rtcm.obs.n && nsat < DefineConstants.MAXOBS; j++)
            {
                sat = rtcm.obs.data[j].sat;
                if (GlobalMembersRtkcmn.satsys(sat, ref prn) != DefineConstants.SYS_GLO)
                    continue;
                fcn = GlobalMembersRtcm3e.fcn_glo(sat, rtcm);

                /* generate obs field data glonass */
                GlobalMembersRtcm3e.gen_obs_glo(rtcm, rtcm.obs.data + j, fcn, ref code1, ref pr1, ref ppr1, ref lock1, ref amb, ref cnr1, ref code2, ref pr21, ref ppr2, ref lock2, ref cnr2);

                if (fcn < 0)
                {
                    fcn = 0;
                }
                GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 6, prn);
                i += 6;
                GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 1, code1);
                i += 1;
                GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 5, fcn);
                i += 5;
                GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 25, pr1);
                i += 25;
                GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 20, ppr1);
                i += 20;
                GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 7, lock1);
                i += 7;
                GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 7, amb);
                i += 7;
                GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 8, cnr1);
                i += 8;
                GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 2, code2);
                i += 2;
                GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 14, pr21);
                i += 14;
                GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 20, ppr2);
                i += 20;
                GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 7, lock2);
                i += 7;
                GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 8, cnr2);
                i += 8;
            }
            rtcm.nbit = i;
            return 1;
        }
        /* encode type 1019: gps ephemerides -----------------------------------------*/
        internal static int encode_type1019(rtcm_t rtcm, int sync)
        {
            eph_t eph;
            uint sqrtA;
            uint e;
            int i = 24;
            int prn;
            int week;
            int toe;
            int toc;
            int i0;
            int OMG0;
            int omg;
            int M0;
            int deln;
            int idot;
            int OMGd;
            int crs;
            int crc;
            int cus;
            int cuc;
            int cis;
            int cic;
            int af0;
            int af1;
            int af2;
            int tgd;

            GlobalMembersRtkcmn.trace(3, "encode_type1019: sync=%d\n", sync);

            if (GlobalMembersRtkcmn.satsys(rtcm.ephsat, ref prn) != DefineConstants.SYS_GPS)
            {
                return 0;
            }
            eph = rtcm.nav.eph + rtcm.ephsat - 1;
            if (eph.sat != rtcm.ephsat)
            {
                return 0;
            }
            week = eph.week % 1024;
            toe = ((int)Math.Floor((eph.toes / 16.0) + 0.5));
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: toc =((int)floor((time2gpst(eph->toc,null)/16.0)+0.5));
            toc = ((int)Math.Floor((GlobalMembersRtkcmn.time2gpst(new gtime_t(eph.toc), null) / 16.0) + 0.5));
            sqrtA = ((uint)Math.Floor((Math.Sqrt(eph.A) / 1.907348632812500E-0x6) + 0.5));
            e = ((uint)Math.Floor((eph.e / DefineConstants.P2_33) + 0.5));
            i0 = ((int)Math.Floor((eph.i0 / DefineConstants.P2_31 / DefineConstants.SC2RAD) + 0.5));
            OMG0 = ((int)Math.Floor((eph.OMG0 / DefineConstants.P2_31 / DefineConstants.SC2RAD) + 0.5));
            omg = ((int)Math.Floor((eph.omg / DefineConstants.P2_31 / DefineConstants.SC2RAD) + 0.5));
            M0 = ((int)Math.Floor((eph.M0 / DefineConstants.P2_31 / DefineConstants.SC2RAD) + 0.5));
            deln = ((int)Math.Floor((eph.deln / DefineConstants.P2_43 / DefineConstants.SC2RAD) + 0.5));
            idot = ((int)Math.Floor((eph.idot / DefineConstants.P2_43 / DefineConstants.SC2RAD) + 0.5));
            OMGd = ((int)Math.Floor((eph.OMGd / DefineConstants.P2_43 / DefineConstants.SC2RAD) + 0.5));
            crs = ((int)Math.Floor((eph.crs / DefineConstants.P2_5) + 0.5));
            crc = ((int)Math.Floor((eph.crc / DefineConstants.P2_5) + 0.5));
            cus = ((int)Math.Floor((eph.cus / 1.862645149230957E-0x9) + 0.5));
            cuc = ((int)Math.Floor((eph.cuc / 1.862645149230957E-0x9) + 0.5));
            cis = ((int)Math.Floor((eph.cis / 1.862645149230957E-0x9) + 0.5));
            cic = ((int)Math.Floor((eph.cic / 1.862645149230957E-0x9) + 0.5));
            af0 = ((int)Math.Floor((eph.f0 / DefineConstants.P2_31) + 0.5));
            af1 = ((int)Math.Floor((eph.f1 / DefineConstants.P2_43) + 0.5));
            af2 = ((int)Math.Floor((eph.f2 / DefineConstants.P2_55) + 0.5));
            tgd = ((int)Math.Floor((eph.tgd[0] / DefineConstants.P2_31) + 0.5));

            GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 12, 1019);
            i += 12;
            GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 6, prn);
            i += 6;
            GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 10, week);
            i += 10;
            GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 4, eph.sva);
            i += 4;
            GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 2, eph.code);
            i += 2;
            GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 14, idot);
            i += 14;
            GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 8, eph.iode);
            i += 8;
            GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 16, toc);
            i += 16;
            GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 8, af2);
            i += 8;
            GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 16, af1);
            i += 16;
            GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 22, af0);
            i += 22;
            GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 10, eph.iodc);
            i += 10;
            GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 16, crs);
            i += 16;
            GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 16, deln);
            i += 16;
            GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 32, M0);
            i += 32;
            GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 16, cuc);
            i += 16;
            GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 32, e);
            i += 32;
            GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 16, cus);
            i += 16;
            GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 32, sqrtA);
            i += 32;
            GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 16, toe);
            i += 16;
            GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 16, cic);
            i += 16;
            GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 32, OMG0);
            i += 32;
            GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 16, cis);
            i += 16;
            GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 32, i0);
            i += 32;
            GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 16, crc);
            i += 16;
            GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 32, omg);
            i += 32;
            GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 24, OMGd);
            i += 24;
            GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 8, tgd);
            i += 8;
            GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 6, eph.svh);
            i += 6;
            GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 1, eph.flag);
            i += 1;
            GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 1, eph.fit > 0.0 ? 0 : 1);
            i += 1;
            rtcm.nbit = i;
            return 1;
        }
        /* encode type 1020: glonass ephemerides -------------------------------------*/
        internal static int encode_type1020(rtcm_t rtcm, int sync)
        {
            geph_t geph;
            gtime_t time = new gtime_t();
            double[] ep = new double[6];
            int i = 24;
            int j;
            int prn;
            int tk_h;
            int tk_m;
            int tk_s;
            int tb;
            int[] pos = new int[3];
            int[] vel = new int[3];
            int[] acc = new int[3];
            int gamn;
            int taun;
            int dtaun;
            int fcn;
            int NT;

            GlobalMembersRtkcmn.trace(3, "encode_type1020: sync=%d\n", sync);

            if (GlobalMembersRtkcmn.satsys(rtcm.ephsat, ref prn) != DefineConstants.SYS_GLO)
            {
                return 0;
            }
            geph = rtcm.nav.geph + prn - 1;
            if (geph.sat != rtcm.ephsat)
            {
                return 0;
            }
            fcn = geph.frq + 7;

            /* time of frame within day (utc(su) + 3 hr) */
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: time=timeadd(gpst2utc(geph->tof),10800.0);
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            time.CopyFrom(GlobalMembersRtkcmn.timeadd(GlobalMembersRtkcmn.gpst2utc(new gtime_t(geph.tof)), 10800.0));
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: time2epoch(time,ep);
            GlobalMembersRtkcmn.time2epoch(new gtime_t(time), ep);
            tk_h = (int)ep[3];
            tk_m = (int)ep[4];
            tk_s = ((int)Math.Floor((ep[5] / 30.0) + 0.5));

            /* # of days since jan 1 in leap year */
            ep[0] = Math.Floor(ep[0] / 4.0) * 4.0;
            ep[1] = ep[2] = 1.0;
            ep[3] = ep[4] = ep[5] = 0.0;
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: NT=(int)floor(timediff(time,epoch2time(ep))/86400.0+1.0);
            NT = (int)Math.Floor(GlobalMembersRtkcmn.timediff(new gtime_t(time), GlobalMembersRtkcmn.epoch2time(ep)) / 86400.0 + 1.0);

            /* index of time interval within day (utc(su) + 3 hr) */
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: time=timeadd(gpst2utc(geph->toe),10800.0);
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            time.CopyFrom(GlobalMembersRtkcmn.timeadd(GlobalMembersRtkcmn.gpst2utc(new gtime_t(geph.toe)), 10800.0));
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: time2epoch(time,ep);
            GlobalMembersRtkcmn.time2epoch(new gtime_t(time), ep);
            tb = ((int)Math.Floor(((ep[3] * 3600.0 + ep[4] * 60.0 + ep[5]) / 900.0) + 0.5));

            for (j = 0; j < 3; j++)
            {
                pos[j] = ((int)Math.Floor((geph.pos[j] / 4.882812500000000E-0x4 / 1E3) + 0.5));
            vel[j] = ((int)Math.Floor((geph.vel[j] / 9.536743164062500E-0x7 / 1E3) + 0.5));
            acc[j] = ((int)Math.Floor((geph.acc[j] / DefineConstants.P2_30 / 1E3) + 0.5));
        }
        gamn = ((int) Math.Floor((geph.gamn / DefineConstants.P2_40) + 0.5));
		taun = ((int) Math.Floor((geph.taun / DefineConstants.P2_30) + 0.5));
		dtaun = ((int) Math.Floor((geph.dtaun / DefineConstants.P2_30) + 0.5));

		GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 12, 1020);
		i += 12;
		GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 6, prn);
		i += 6;
		GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 5, fcn);
		i += 5;
		GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 4, 0); // almanac health,P1
		i += 4;
		GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 5, tk_h);
		i += 5;
		GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 6, tk_m);
		i += 6;
		GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 1, tk_s);
		i += 1;
		GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 1, geph.svh); // Bn
		i += 1;
		GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 1, 0); // P2
		i += 1;
		GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 7, tb);
		i += 7;
		GlobalMembersRtcm3e.setbitg(ref rtcm.buff, i, 24, vel[0]);
		i += 24;
		GlobalMembersRtcm3e.setbitg(ref rtcm.buff, i, 27, pos[0]);
		i += 27;
		GlobalMembersRtcm3e.setbitg(ref rtcm.buff, i, 5, acc[0]);
		i += 5;
		GlobalMembersRtcm3e.setbitg(ref rtcm.buff, i, 24, vel[1]);
		i += 24;
		GlobalMembersRtcm3e.setbitg(ref rtcm.buff, i, 27, pos[1]);
		i += 27;
		GlobalMembersRtcm3e.setbitg(ref rtcm.buff, i, 5, acc[1]);
		i += 5;
		GlobalMembersRtcm3e.setbitg(ref rtcm.buff, i, 24, vel[2]);
		i += 24;
		GlobalMembersRtcm3e.setbitg(ref rtcm.buff, i, 27, pos[2]);
		i += 27;
		GlobalMembersRtcm3e.setbitg(ref rtcm.buff, i, 5, acc[2]);
		i += 5;
		GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 1, 0); // P3
		i += 1;
		GlobalMembersRtcm3e.setbitg(ref rtcm.buff, i, 11, gamn);
		i += 11;
		GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 3, 0); // P,ln
		i += 3;
		GlobalMembersRtcm3e.setbitg(ref rtcm.buff, i, 22, taun);
		i += 22;
		GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 5, dtaun);
		i += 5;
		GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 5, geph.age); // En
		i += 5;
		GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 1, 0); // P4
		i += 1;
		GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 4, 0); // FT
		i += 4;
		GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 11, NT);
		i += 11;
		GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 2, 0); // M
		i += 2;
		GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 1, 0); // flag for addtional data
		i += 1;
		GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 11, 0); // NA
		i += 11;
		GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 32, 0); // tauc
		i += 32;
		GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 5, 0); // N4
		i += 5;
		GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 22, 0); // taugps
		i += 22;
		GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 1, 0); // ln
		i += 1;
		GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 7, 0);
		i += 7;
		rtcm.nbit = i;
		return 1;
	}
    /* encode type 1033: receiver and antenna descriptor -------------------------*/
    internal static int encode_type1033(rtcm_t rtcm, int sync)
    {
        int i = 24;
        int j;
        int antsetup = rtcm.sta.antsetup;
        int n = ((rtcm.sta.antdes.Length) < (31) ? (rtcm.sta.antdes.Length) : (31));
        int m = ((rtcm.sta.antsno.Length) < (31) ? (rtcm.sta.antsno.Length) : (31));
        int I = ((rtcm.sta.rectype.Length) < (31) ? (rtcm.sta.rectype.Length) : (31));
        int J = ((rtcm.sta.recver.Length) < (31) ? (rtcm.sta.recver.Length) : (31));
        int K = ((rtcm.sta.recsno.Length) < (31) ? (rtcm.sta.recsno.Length) : (31));

        GlobalMembersRtkcmn.trace(3, "encode_type1033: sync=%d\n", sync);

        GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 12, 1033);
        i += 12;
        GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 12, rtcm.staid);
        i += 12;

        GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 8, n);
        i += 8;
        for (j = 0; j < n; j++)
        {
            GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 8, rtcm.sta.antdes[j]);
            i += 8;
        }
        GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 8, antsetup);
        i += 8;

        GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 8, m);
        i += 8;
        for (j = 0; j < m; j++)
        {
            GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 8, rtcm.sta.antsno[j]);
            i += 8;
        }
        GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 8, I);
        i += 8;
        for (j = 0; j < I; j++)
        {
            GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 8, rtcm.sta.rectype[j]);
            i += 8;
        }
        GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 8, J);
        i += 8;
        for (j = 0; j < J; j++)
        {
            GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 8, rtcm.sta.recver[j]);
            i += 8;
        }
        GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 8, K);
        i += 8;
        for (j = 0; j < K; j++)
        {
            GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 8, rtcm.sta.recsno[j]);
            i += 8;
        }
        rtcm.nbit = i;
        return 1;
    }
    /* encode type 1044: qzss ephemerides (ref [15]) -----------------------------*/
    internal static int encode_type1044(rtcm_t rtcm, int sync)
    {
        eph_t eph;
        uint sqrtA;
        uint e;
        int i = 24;
        int prn;
        int week;
        int toe;
        int toc;
        int i0;
        int OMG0;
        int omg;
        int M0;
        int deln;
        int idot;
        int OMGd;
        int crs;
        int crc;
        int cus;
        int cuc;
        int cis;
        int cic;
        int af0;
        int af1;
        int af2;
        int tgd;

        GlobalMembersRtkcmn.trace(3, "encode_type1044: sync=%d\n", sync);

        if (GlobalMembersRtkcmn.satsys(rtcm.ephsat, ref prn) != DefineConstants.SYS_QZS)
        {
            return 0;
        }
        eph = rtcm.nav.eph + rtcm.ephsat - 1;
        if (eph.sat != rtcm.ephsat)
        {
            return 0;
        }
        week = eph.week % 1024;
        toe = ((int)Math.Floor((eph.toes / 16.0) + 0.5));
        //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
        //ORIGINAL LINE: toc =((int)floor((time2gpst(eph->toc,null)/16.0)+0.5));
        toc = ((int)Math.Floor((GlobalMembersRtkcmn.time2gpst(new gtime_t(eph.toc), null) / 16.0) + 0.5));
        sqrtA = ((uint)Math.Floor((Math.Sqrt(eph.A) / 1.907348632812500E-0x6) + 0.5));
        e = ((uint)Math.Floor((eph.e / DefineConstants.P2_33) + 0.5));
        i0 = ((int)Math.Floor((eph.i0 / DefineConstants.P2_31 / DefineConstants.SC2RAD) + 0.5));
        OMG0 = ((int)Math.Floor((eph.OMG0 / DefineConstants.P2_31 / DefineConstants.SC2RAD) + 0.5));
        omg = ((int)Math.Floor((eph.omg / DefineConstants.P2_31 / DefineConstants.SC2RAD) + 0.5));
        M0 = ((int)Math.Floor((eph.M0 / DefineConstants.P2_31 / DefineConstants.SC2RAD) + 0.5));
        deln = ((int)Math.Floor((eph.deln / DefineConstants.P2_43 / DefineConstants.SC2RAD) + 0.5));
        idot = ((int)Math.Floor((eph.idot / DefineConstants.P2_43 / DefineConstants.SC2RAD) + 0.5));
        OMGd = ((int)Math.Floor((eph.OMGd / DefineConstants.P2_43 / DefineConstants.SC2RAD) + 0.5));
        crs = ((int)Math.Floor((eph.crs / DefineConstants.P2_5) + 0.5));
        crc = ((int)Math.Floor((eph.crc / DefineConstants.P2_5) + 0.5));
        cus = ((int)Math.Floor((eph.cus / 1.862645149230957E-0x9) + 0.5));
        cuc = ((int)Math.Floor((eph.cuc / 1.862645149230957E-0x9) + 0.5));
        cis = ((int)Math.Floor((eph.cis / 1.862645149230957E-0x9) + 0.5));
        cic = ((int)Math.Floor((eph.cic / 1.862645149230957E-0x9) + 0.5));
        af0 = ((int)Math.Floor((eph.f0 / DefineConstants.P2_31) + 0.5));
        af1 = ((int)Math.Floor((eph.f1 / DefineConstants.P2_43) + 0.5));
        af2 = ((int)Math.Floor((eph.f2 / DefineConstants.P2_55) + 0.5));
        tgd = ((int)Math.Floor((eph.tgd[0] / DefineConstants.P2_31) + 0.5));

        GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 12, 1044);
        i += 12;
        GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 4, prn - 192);
        i += 4;
        GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 16, toc);
        i += 16;
        GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 8, af2);
        i += 8;
        GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 16, af1);
        i += 16;
        GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 22, af0);
        i += 22;
        GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 8, eph.iode);
        i += 8;
        GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 16, crs);
        i += 16;
        GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 16, deln);
        i += 16;
        GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 32, M0);
        i += 32;
        GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 16, cuc);
        i += 16;
        GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 32, e);
        i += 32;
        GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 16, cus);
        i += 16;
        GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 32, sqrtA);
        i += 32;
        GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 16, toe);
        i += 16;
        GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 16, cic);
        i += 16;
        GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 32, OMG0);
        i += 32;
        GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 16, cis);
        i += 16;
        GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 32, i0);
        i += 32;
        GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 16, crc);
        i += 16;
        GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 32, omg);
        i += 32;
        GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 24, OMGd);
        i += 24;
        GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 14, idot);
        i += 14;
        GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 2, eph.code);
        i += 2;
        GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 10, week);
        i += 10;
        GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 4, eph.sva);
        i += 4;
        GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 6, eph.svh);
        i += 6;
        GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 8, tgd);
        i += 8;
        GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 10, eph.iodc);
        i += 10;
        GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 1, eph.fit == 2.0 ? 0 : 1);
        i += 1;
        rtcm.nbit = i;
        return 1;
    }
    /* encode type 1045: galileo satellite ephemerides (ref [15]) ----------------*/
    internal static int encode_type1045(rtcm_t rtcm, int sync)
    {
        eph_t eph;
        uint sqrtA;
        uint e;
        int i = 24;
        int prn;
        int week;
        int toe;
        int toc;
        int i0;
        int OMG0;
        int omg;
        int M0;
        int deln;
        int idot;
        int OMGd;
        int crs;
        int crc;
        int cus;
        int cuc;
        int cis;
        int cic;
        int af0;
        int af1;
        int af2;
        int bgd1;
        int bgd2;
        int oshs;
        int osdvs;

        GlobalMembersRtkcmn.trace(3, "encode_type1045: sync=%d\n", sync);

        if (GlobalMembersRtkcmn.satsys(rtcm.ephsat, ref prn) != DefineConstants.SYS_GAL)
        {
            return 0;
        }
        eph = rtcm.nav.eph + rtcm.ephsat - 1;
        if (eph.sat != rtcm.ephsat)
        {
            return 0;
        }
        week = eph.week % 4092;
        toe = ((int)Math.Floor((eph.toes / 60.0) + 0.5));
        //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
        //ORIGINAL LINE: toc =((int)floor((time2gpst(eph->toc,null)/60.0)+0.5));
        toc = ((int)Math.Floor((GlobalMembersRtkcmn.time2gpst(new gtime_t(eph.toc), null) / 60.0) + 0.5));
        sqrtA = ((uint)Math.Floor((Math.Sqrt(eph.A) / 1.907348632812500E-0x6) + 0.5));
        e = ((uint)Math.Floor((eph.e / DefineConstants.P2_33) + 0.5));
        i0 = ((int)Math.Floor((eph.i0 / DefineConstants.P2_31 / DefineConstants.SC2RAD) + 0.5));
        OMG0 = ((int)Math.Floor((eph.OMG0 / DefineConstants.P2_31 / DefineConstants.SC2RAD) + 0.5));
        omg = ((int)Math.Floor((eph.omg / DefineConstants.P2_31 / DefineConstants.SC2RAD) + 0.5));
        M0 = ((int)Math.Floor((eph.M0 / DefineConstants.P2_31 / DefineConstants.SC2RAD) + 0.5));
        deln = ((int)Math.Floor((eph.deln / DefineConstants.P2_43 / DefineConstants.SC2RAD) + 0.5));
        idot = ((int)Math.Floor((eph.idot / DefineConstants.P2_43 / DefineConstants.SC2RAD) + 0.5));
        OMGd = ((int)Math.Floor((eph.OMGd / DefineConstants.P2_43 / DefineConstants.SC2RAD) + 0.5));
        crs = ((int)Math.Floor((eph.crs / DefineConstants.P2_5) + 0.5));
        crc = ((int)Math.Floor((eph.crc / DefineConstants.P2_5) + 0.5));
        cus = ((int)Math.Floor((eph.cus / 1.862645149230957E-0x9) + 0.5));
        cuc = ((int)Math.Floor((eph.cuc / 1.862645149230957E-0x9) + 0.5));
        cis = ((int)Math.Floor((eph.cis / 1.862645149230957E-0x9) + 0.5));
        cic = ((int)Math.Floor((eph.cic / 1.862645149230957E-0x9) + 0.5));
        af0 = ((int)Math.Floor((eph.f0 / DefineConstants.P2_34) + 0.5));
        af1 = ((int)Math.Floor((eph.f1 / DefineConstants.P2_46) + 0.5));
        af2 = ((int)Math.Floor((eph.f2 / DefineConstants.P2_59) + 0.5));
        bgd1 = ((int)Math.Floor((eph.tgd[0] / DefineConstants.P2_32) + 0.5));
        bgd2 = ((int)Math.Floor((eph.tgd[1] / DefineConstants.P2_32) + 0.5));
        oshs = (eph.svh >> 4) & 3;
        osdvs = (eph.svh >> 3) & 1;
        GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 12, 1045);
        i += 12;
        GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 6, prn);
        i += 6;
        GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 12, week);
        i += 12;
        GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 10, eph.iode);
        i += 10;
        GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 8, eph.sva);
        i += 8;
        GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 14, idot);
        i += 14;
        GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 14, toc);
        i += 14;
        GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 6, af2);
        i += 6;
        GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 21, af1);
        i += 21;
        GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 31, af0);
        i += 31;
        GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 16, crs);
        i += 16;
        GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 16, deln);
        i += 16;
        GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 32, M0);
        i += 32;
        GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 16, cuc);
        i += 16;
        GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 32, e);
        i += 32;
        GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 16, cus);
        i += 16;
        GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 32, sqrtA);
        i += 32;
        GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 14, toe);
        i += 14;
        GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 16, cic);
        i += 16;
        GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 32, OMG0);
        i += 32;
        GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 16, cis);
        i += 16;
        GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 32, i0);
        i += 32;
        GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 16, crc);
        i += 16;
        GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 32, omg);
        i += 32;
        GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 24, OMGd);
        i += 24;
        GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 10, bgd1);
        i += 10;
        GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 2, oshs);
        i += 2;
        GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 1, osdvs);
        i += 1;
        GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 7, 0); // reserved
        i += 7;
        rtcm.nbit = i;
        return 1;
    }
    /* encode type 1047: beidou ephemerides (tentative mt and format) ------------*/
    internal static int encode_type1047(rtcm_t rtcm, int sync)
    {
        eph_t eph;
        uint sqrtA;
        uint e;
        int i = 24;
        int prn;
        int week;
        int toe;
        int toc;
        int i0;
        int OMG0;
        int omg;
        int M0;
        int deln;
        int idot;
        int OMGd;
        int crs;
        int crc;
        int cus;
        int cuc;
        int cis;
        int cic;
        int af0;
        int af1;
        int af2;
        int tgd;

        GlobalMembersRtkcmn.trace(3, "encode_type1047: sync=%d\n", sync);

        if (GlobalMembersRtkcmn.satsys(rtcm.ephsat, ref prn) != DefineConstants.SYS_CMP)
        {
            return 0;
        }
        eph = rtcm.nav.eph + rtcm.ephsat - 1;
        if (eph.sat != rtcm.ephsat)
        {
            return 0;
        }
        week = eph.week % 1024;
        toe = ((int)Math.Floor((eph.toes / 16.0) + 0.5));
        //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
        //ORIGINAL LINE: toc =((int)floor((time2bdt(gpst2bdt(eph->toc),null)/16.0)+0.5));
        toc = ((int)Math.Floor((GlobalMembersRtkcmn.time2bdt(GlobalMembersRtkcmn.gpst2bdt(new gtime_t(eph.toc)), null) / 16.0) + 0.5));
        sqrtA = ((uint)Math.Floor((Math.Sqrt(eph.A) / 1.907348632812500E-0x6) + 0.5));
        e = ((uint)Math.Floor((eph.e / DefineConstants.P2_33) + 0.5));
        i0 = ((int)Math.Floor((eph.i0 / DefineConstants.P2_31 / DefineConstants.SC2RAD) + 0.5));
        OMG0 = ((int)Math.Floor((eph.OMG0 / DefineConstants.P2_31 / DefineConstants.SC2RAD) + 0.5));
        omg = ((int)Math.Floor((eph.omg / DefineConstants.P2_31 / DefineConstants.SC2RAD) + 0.5));
        M0 = ((int)Math.Floor((eph.M0 / DefineConstants.P2_31 / DefineConstants.SC2RAD) + 0.5));
        deln = ((int)Math.Floor((eph.deln / DefineConstants.P2_43 / DefineConstants.SC2RAD) + 0.5));
        idot = ((int)Math.Floor((eph.idot / DefineConstants.P2_43 / DefineConstants.SC2RAD) + 0.5));
        OMGd = ((int)Math.Floor((eph.OMGd / DefineConstants.P2_43 / DefineConstants.SC2RAD) + 0.5));
        crs = ((int)Math.Floor((eph.crs / DefineConstants.P2_5) + 0.5));
        crc = ((int)Math.Floor((eph.crc / DefineConstants.P2_5) + 0.5));
        cus = ((int)Math.Floor((eph.cus / 1.862645149230957E-0x9) + 0.5));
        cuc = ((int)Math.Floor((eph.cuc / 1.862645149230957E-0x9) + 0.5));
        cis = ((int)Math.Floor((eph.cis / 1.862645149230957E-0x9) + 0.5));
        cic = ((int)Math.Floor((eph.cic / 1.862645149230957E-0x9) + 0.5));
        af0 = ((int)Math.Floor((eph.f0 / DefineConstants.P2_31) + 0.5));
        af1 = ((int)Math.Floor((eph.f1 / DefineConstants.P2_43) + 0.5));
        af2 = ((int)Math.Floor((eph.f2 / DefineConstants.P2_55) + 0.5));
        tgd = ((int)Math.Floor((eph.tgd[0] / DefineConstants.P2_31) + 0.5));

        GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 12, 1047);
        i += 12;
        GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 6, prn);
        i += 6;
        GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 10, week);
        i += 10;
        GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 4, eph.sva);
        i += 4;
        GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 2, eph.code);
        i += 2;
        GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 14, idot);
        i += 14;
        GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 8, eph.iode);
        i += 8;
        GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 16, toc);
        i += 16;
        GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 8, af2);
        i += 8;
        GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 16, af1);
        i += 16;
        GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 22, af0);
        i += 22;
        GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 10, eph.iodc);
        i += 10;
        GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 16, crs);
        i += 16;
        GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 16, deln);
        i += 16;
        GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 32, M0);
        i += 32;
        GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 16, cuc);
        i += 16;
        GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 32, e);
        i += 32;
        GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 16, cus);
        i += 16;
        GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 32, sqrtA);
        i += 32;
        GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 16, toe);
        i += 16;
        GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 16, cic);
        i += 16;
        GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 32, OMG0);
        i += 32;
        GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 16, cis);
        i += 16;
        GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 32, i0);
        i += 32;
        GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 16, crc);
        i += 16;
        GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 32, omg);
        i += 32;
        GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 24, OMGd);
        i += 24;
        GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 8, tgd);
        i += 8;
        GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 6, eph.svh);
        i += 6;
        GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 1, eph.flag);
        i += 1;
        GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 1, eph.fit > 0.0 ? 0 : 1);
        i += 1;
        rtcm.nbit = i;
        return 1;
    }
    /* encode ssr header ---------------------------------------------------------*/
    internal static int encode_ssr_head(int type, rtcm_t rtcm, int sys, int nsat, int sync, int iod, double udint, int refd, int provid, int solid)
    {
        double tow;
        int i = 24;
        int msgno;
        int epoch;
        int week;
        int udi;
        int ns = 6;

        GlobalMembersRtkcmn.trace(4, "encode_ssr_head: type=%d sys=%d nsat=%d sync=%d iod=%d udint=%.0f\n", type, sys, nsat, sync, iod, udint);

#if !SSR_QZSS_DRAFT_V05
        ns = sys == DefineConstants.SYS_QZS ? 4 : 6;
#endif
        switch (sys)
        {
            case DefineConstants.SYS_GPS:
                msgno = 1056 + type;
                break;
            case DefineConstants.SYS_GLO:
                msgno = 1062 + type;
                break;
            case DefineConstants.SYS_GAL:
                msgno = 1239 + type;
                break;
            case DefineConstants.SYS_QZS:
                msgno = 1245 + type;
                break;
            case DefineConstants.SYS_CMP:
                msgno = 1257 + type;
                break;
            case DefineConstants.SYS_SBS:
                msgno = 1251 + type;
                break;
            default:
                return 0;
        }
        GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 12, msgno); // message number
        i += 12;

        if (sys == DefineConstants.SYS_GLO)
        {
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: tow=time2gpst(timeadd(gpst2utc(rtcm->time),10800.0),&week);
            tow = GlobalMembersRtkcmn.time2gpst(GlobalMembersRtkcmn.timeadd(GlobalMembersRtkcmn.gpst2utc(new gtime_t(rtcm.time)), 10800.0), ref week);
            epoch = ((int)Math.Floor((Math.IEEERemainder(tow, 86400.0)) + 0.5));
            GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 17, epoch); // glonass epoch time
            i += 17;
        }
        else
        {
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: tow=time2gpst(rtcm->time,&week);
            tow = GlobalMembersRtkcmn.time2gpst(new gtime_t(rtcm.time), ref week);
            epoch = ((int)Math.Floor((tow) + 0.5));
            GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 20, epoch); // gps epoch time
            i += 20;
        }
        for (udi = 0; udi < 15; udi++)
        {
            if (ssrudint[udi] >= udint)
                break;
        }
        GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 4, udi); // update interval
        i += 4;
        GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 1, sync); // multiple message indicator
        i += 1;
        if (type == 1 || type == 4)
        {
            GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 1, refd); // satellite ref datum
            i += 1;
        }
        GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 4, iod); // iod ssr
        i += 4;
        GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 16, provid); // provider id
        i += 16;
        GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 4, solid); // solution id
        i += 4;
        GlobalMembersRtkcmn.setbitu(rtcm.buff, i, ns, nsat); // no of satellites
        i += ns;
        return i;
    }
    /* encode ssr 1: orbit corrections -------------------------------------------*/
    internal static int encode_ssr1(rtcm_t rtcm, int sys, int sync)
    {
        double udint = 0.0;
        int i;
        int j;
        int iod = 0;
        int nsat;
        int prn;
        int iode;
        int iodcrc;
        int refd = 0;
        int np;
        int ni;
        int nj;
        int offp;
        int[] deph = new int[3];
        int[] ddeph = new int[3];

        GlobalMembersRtkcmn.trace(3, "encode_ssr1: sys=%d sync=%d\n", sys, sync);

        switch (sys)
        {
            case DefineConstants.SYS_GPS:
                np = 6;
                ni = 8;
                nj = 0;
                offp = 0;
                break;
            case DefineConstants.SYS_GLO:
                np = 5;
                ni = 8;
                nj = 0;
                offp = 0;
                break;
            case DefineConstants.SYS_GAL:
                np = 6;
                ni = 10;
                nj = 0;
                offp = 0;
                break;
            case DefineConstants.SYS_QZS:
                np = 4;
                ni = 8;
                nj = 0;
                offp = 192;
                break;
            case DefineConstants.SYS_CMP:
                np = 6;
                ni = 10;
                nj = 24;
                offp = 1;
                break;
            case DefineConstants.SYS_SBS:
                np = 6;
                ni = 9;
                nj = 24;
                offp = 120;
                break;
            default:
                return 0;
        }
        /* number of satellites */
        for (j = nsat = 0; j < DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1; j++)
        {
            if (GlobalMembersRtkcmn.satsys(j + 1, ref prn) != sys || rtcm.ssr[j].update == 0)
                continue;
            nsat++;
            udint = rtcm.ssr[j].udi[0];
            iod = rtcm.ssr[j].iod[0];
            refd = rtcm.ssr[j].refd;
        }
        /* encode ssr header */
        i = GlobalMembersRtcm3e.encode_ssr_head(1, rtcm, sys, nsat, sync, iod, udint, refd, 0, 0);

        for (j = 0; j < DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1; j++)
        {
            if (GlobalMembersRtkcmn.satsys(j + 1, ref prn) != sys || rtcm.ssr[j].update == 0)
                continue;

            iode = rtcm.ssr[j].iode; // sbas/bds: toe/t0 modulo
            iodcrc = rtcm.ssr[j].iodcrc; // sbas/bds: iod crc

            deph[0] = ((int)Math.Floor((rtcm.ssr[j].deph[0] / 1E-4) + 0.5));
            deph[1] = ((int)Math.Floor((rtcm.ssr[j].deph[1] / 4E-4) + 0.5));
            deph[2] = ((int)Math.Floor((rtcm.ssr[j].deph[2] / 4E-4) + 0.5));
            ddeph[0] = ((int)Math.Floor((rtcm.ssr[j].ddeph[0] / 1E-6) + 0.5));
            ddeph[1] = ((int)Math.Floor((rtcm.ssr[j].ddeph[1] / 4E-6) + 0.5));
            ddeph[2] = ((int)Math.Floor((rtcm.ssr[j].ddeph[2] / 4E-6) + 0.5));

            GlobalMembersRtkcmn.setbitu(rtcm.buff, i, np, prn - offp); // satellite id
            i += np;
            GlobalMembersRtkcmn.setbitu(rtcm.buff, i, ni, iode); // iode
            i += ni;
            GlobalMembersRtkcmn.setbitu(rtcm.buff, i, nj, iodcrc); // iodcrc
            i += nj;
            GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 22, deph[0]); // delta radial
            i += 22;
            GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 20, deph[1]); // delta along-track
            i += 20;
            GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 20, deph[2]); // delta cross-track
            i += 20;
            GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 21, ddeph[0]); // dot delta radial
            i += 21;
            GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 19, ddeph[1]); // dot delta along-track
            i += 19;
            GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 19, ddeph[2]); // dot delta cross-track
            i += 19;
        }
        rtcm.nbit = i;
        return 1;
    }
    /* encode ssr 2: clock corrections -------------------------------------------*/
    internal static int encode_ssr2(rtcm_t rtcm, int sys, int sync)
    {
        double udint = 0.0;
        int i;
        int j;
        int iod = 0;
        int nsat;
        int prn;
        int np;
        int offp;
        int iode;
        int[] dclk = new int[3];

        GlobalMembersRtkcmn.trace(3, "encode_ssr2: sys=%d sync=%d\n", sys, sync);

        switch (sys)
        {
            case DefineConstants.SYS_GPS:
                np = 6;
                offp = 0;
                break;
            case DefineConstants.SYS_GLO:
                np = 5;
                offp = 0;
                break;
            case DefineConstants.SYS_GAL:
                np = 6;
                offp = 0;
                break;
            case DefineConstants.SYS_QZS:
                np = 4;
                offp = 192;
                break;
            case DefineConstants.SYS_CMP:
                np = 6;
                offp = 1;
                break;
            case DefineConstants.SYS_SBS:
                np = 6;
                offp = 120;
                break;
            default:
                return 0;
        }
        /* number of satellites */
        for (j = nsat = 0; j < DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1; j++)
        {
            if (GlobalMembersRtkcmn.satsys(j + 1, ref prn) != sys || rtcm.ssr[j].update == 0)
                continue;
            nsat++;
            udint = rtcm.ssr[j].udi[1];
            iod = rtcm.ssr[j].iod[1];
        }
        /* encode ssr header */
        i = GlobalMembersRtcm3e.encode_ssr_head(2, rtcm, sys, nsat, sync, iod, udint, 0, 0, 0);

        for (j = 0; j < DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1; j++)
        {
            if (GlobalMembersRtkcmn.satsys(j + 1, ref prn) != sys || rtcm.ssr[j].update == 0)
                continue;

            iode = rtcm.ssr[j].iode;

            dclk[0] = ((int)Math.Floor((rtcm.ssr[j].dclk[0] / 1E-4) + 0.5));
            dclk[1] = ((int)Math.Floor((rtcm.ssr[j].dclk[1] / 1E-6) + 0.5));
            dclk[2] = ((int)Math.Floor((rtcm.ssr[j].dclk[2] / 1E-8) + 0.5));

            GlobalMembersRtkcmn.setbitu(rtcm.buff, i, np, prn - offp); // satellite id
            i += np;
            GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 22, dclk[0]); // delta clock c0
            i += 22;
            GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 21, dclk[1]); // delta clock c1
            i += 21;
            GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 27, dclk[2]); // delta clock c2
            i += 27;
        }
        rtcm.nbit = i;
        return 1;
    }
    /* encode ssr 3: satellite code biases ---------------------------------------*/
    internal static int encode_ssr3(rtcm_t rtcm, int sys, int sync)
    {
        int[] codes_gps = { DefineConstants.CODE_L1C, DefineConstants.CODE_L1P, DefineConstants.CODE_L1W, DefineConstants.CODE_L1Y, DefineConstants.CODE_L1M, DefineConstants.CODE_L2C, DefineConstants.CODE_L2D, DefineConstants.CODE_L2S, DefineConstants.CODE_L2L, DefineConstants.CODE_L2X, DefineConstants.CODE_L2P, DefineConstants.CODE_L2W, DefineConstants.CODE_L2Y, DefineConstants.CODE_L2M, DefineConstants.CODE_L5I, DefineConstants.CODE_L5Q, DefineConstants.CODE_L5X };
        int[] codes_glo = { DefineConstants.CODE_L1C, DefineConstants.CODE_L1P, DefineConstants.CODE_L2C, DefineConstants.CODE_L2P };
        int[] codes_gal = { DefineConstants.CODE_L1A, DefineConstants.CODE_L1B, DefineConstants.CODE_L1C, DefineConstants.CODE_L1X, DefineConstants.CODE_L1Z, DefineConstants.CODE_L5I, DefineConstants.CODE_L5Q, DefineConstants.CODE_L5X, DefineConstants.CODE_L7I, DefineConstants.CODE_L7Q, DefineConstants.CODE_L7X, DefineConstants.CODE_L8I, DefineConstants.CODE_L8Q, DefineConstants.CODE_L8X, DefineConstants.CODE_L6A, DefineConstants.CODE_L6B, DefineConstants.CODE_L6C, DefineConstants.CODE_L6X, DefineConstants.CODE_L6Z };
        int[] codes_qzs = { DefineConstants.CODE_L1C, DefineConstants.CODE_L1S, DefineConstants.CODE_L1L, DefineConstants.CODE_L2S, DefineConstants.CODE_L2L, DefineConstants.CODE_L2X, DefineConstants.CODE_L5I, DefineConstants.CODE_L5Q, DefineConstants.CODE_L5X, DefineConstants.CODE_L6S, DefineConstants.CODE_L6L, DefineConstants.CODE_L6X, DefineConstants.CODE_L1X };
        int[] codes_bds = { DefineConstants.CODE_L1I, DefineConstants.CODE_L1Q, DefineConstants.CODE_L1X, DefineConstants.CODE_L7I, DefineConstants.CODE_L7Q, DefineConstants.CODE_L7X, DefineConstants.CODE_L6I, DefineConstants.CODE_L6Q, DefineConstants.CODE_L6X };
        int[] codes_sbs = { DefineConstants.CODE_L1C, DefineConstants.CODE_L5I, DefineConstants.CODE_L5Q, DefineConstants.CODE_L5X };
        //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
        //ORIGINAL LINE: const int *codes;
        int codes;
        double udint = 0.0;
        int i;
        int j;
        int k;
        int iod = 0;
        int nsat;
        int prn;
        int nbias;
        int np;
        int offp;
        int ncode;
        int[] code = new int[DefineConstants.MAXCODE];
        int[] bias = new int[DefineConstants.MAXCODE];

        GlobalMembersRtkcmn.trace(3, "encode_ssr3: sys=%d sync=%d\n", sys, sync);

        switch (sys)
        {
            case DefineConstants.SYS_GPS:
                np = 6;
                offp = 0;
                codes = codes_gps;
                ncode = 16;
                break;
            case DefineConstants.SYS_GLO:
                np = 5;
                offp = 0;
                codes = codes_glo;
                ncode = 3;
                break;
            case DefineConstants.SYS_GAL:
                np = 6;
                offp = 0;
                codes = codes_gal;
                ncode = 19;
                break;
            case DefineConstants.SYS_QZS:
                np = 4;
                offp = 192;
                codes = codes_qzs;
                ncode = 12;
                break;
            case DefineConstants.SYS_CMP:
                np = 6;
                offp = 1;
                codes = codes_bds;
                ncode = 9;
                break;
            case DefineConstants.SYS_SBS:
                np = 6;
                offp = 120;
                codes = codes_sbs;
                ncode = 4;
                break;
            default:
                return 0;
        }
        /* number of satellites */
        for (j = nsat = 0; j < DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1; j++)
        {
            if (GlobalMembersRtkcmn.satsys(j + 1, ref prn) != sys || rtcm.ssr[j].update == 0)
                continue;
            nsat++;
            udint = rtcm.ssr[j].udi[4];
            iod = rtcm.ssr[j].iod[4];
        }
        /* encode ssr header */
        i = GlobalMembersRtcm3e.encode_ssr_head(3, rtcm, sys, nsat, sync, iod, udint, 0, 0, 0);

        for (j = nsat = 0; j < DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1; j++)
        {
            if (GlobalMembersRtkcmn.satsys(j + 1, ref prn) != sys || rtcm.ssr[j].update == 0)
                continue;

            for (k = nbias = 0; k < ncode; k++)
            {
                if (rtcm.ssr[j].cbias[codes[k] - 1] == 0.0)
                    continue;
                code[nbias] = k;
                bias[nbias++] = ((int)Math.Floor((rtcm.ssr[j].cbias[codes[k] - 1] / 0.01) + 0.5));
            }
            GlobalMembersRtkcmn.setbitu(rtcm.buff, i, np, prn - offp); // satellite id
            i += np;
            GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 5, nbias); // number of code biases
            i += 5;

            for (k = 0; k < nbias; k++)
            {
                GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 5, code[k]); // signal indicator
                i += 5;
                GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 14, bias[k]); // code bias
                i += 14;
            }
        }
        rtcm.nbit = i;
        return 1;
    }
    /* encode ssr 4: combined orbit and clock corrections ------------------------*/
    internal static int encode_ssr4(rtcm_t rtcm, int sys, int sync)
    {
        double udint = 0.0;
        int i;
        int j;
        int iod = 0;
        int nsat;
        int prn;
        int iode;
        int iodcrc;
        int refd = 0;
        int np;
        int ni;
        int nj;
        int offp;
        int[] deph = new int[3];
        int[] ddeph = new int[3];
        int[] dclk = new int[3];

        GlobalMembersRtkcmn.trace(3, "encode_ssr4: sys=%d sync=%d\n", sys, sync);

        switch (sys)
        {
            case DefineConstants.SYS_GPS:
                np = 6;
                ni = 8;
                nj = 0;
                offp = 0;
                break;
            case DefineConstants.SYS_GLO:
                np = 5;
                ni = 8;
                nj = 0;
                offp = 0;
                break;
            case DefineConstants.SYS_GAL:
                np = 6;
                ni = 10;
                nj = 0;
                offp = 0;
                break;
            case DefineConstants.SYS_QZS:
                np = 4;
                ni = 8;
                nj = 0;
                offp = 192;
                break;
            case DefineConstants.SYS_CMP:
                np = 6;
                ni = 10;
                nj = 24;
                offp = 1;
                break;
            case DefineConstants.SYS_SBS:
                np = 6;
                ni = 9;
                nj = 24;
                offp = 120;
                break;
            default:
                return 0;
        }
        /* number of satellites */
        for (j = nsat = 0; j < DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1; j++)
        {
            if (GlobalMembersRtkcmn.satsys(j + 1, ref prn) != sys || rtcm.ssr[j].update == 0)
                continue;
            nsat++;
            udint = rtcm.ssr[j].udi[0];
            iod = rtcm.ssr[j].iod[0];
            refd = rtcm.ssr[j].refd;
        }
        /* encode ssr header */
        i = GlobalMembersRtcm3e.encode_ssr_head(4, rtcm, sys, nsat, sync, iod, udint, refd, 0, 0);

        for (j = 0; j < DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1; j++)
        {
            if (GlobalMembersRtkcmn.satsys(j + 1, ref prn) != sys || rtcm.ssr[j].update == 0)
                continue;

            iode = rtcm.ssr[j].iode;
            iodcrc = rtcm.ssr[j].iodcrc;

            deph[0] = ((int)Math.Floor((rtcm.ssr[j].deph[0] / 1E-4) + 0.5));
            deph[1] = ((int)Math.Floor((rtcm.ssr[j].deph[1] / 4E-4) + 0.5));
            deph[2] = ((int)Math.Floor((rtcm.ssr[j].deph[2] / 4E-4) + 0.5));
            ddeph[0] = ((int)Math.Floor((rtcm.ssr[j].ddeph[0] / 1E-6) + 0.5));
            ddeph[1] = ((int)Math.Floor((rtcm.ssr[j].ddeph[1] / 4E-6) + 0.5));
            ddeph[2] = ((int)Math.Floor((rtcm.ssr[j].ddeph[2] / 4E-6) + 0.5));
            dclk[0] = ((int)Math.Floor((rtcm.ssr[j].dclk[0] / 1E-4) + 0.5));
            dclk[1] = ((int)Math.Floor((rtcm.ssr[j].dclk[1] / 1E-6) + 0.5));
            dclk[2] = ((int)Math.Floor((rtcm.ssr[j].dclk[2] / 1E-8) + 0.5));

            GlobalMembersRtkcmn.setbitu(rtcm.buff, i, np, prn - offp); // satellite id
            i += np;
            GlobalMembersRtkcmn.setbitu(rtcm.buff, i, ni, iode); // iode
            i += ni;
            GlobalMembersRtkcmn.setbitu(rtcm.buff, i, nj, iodcrc); // iodcrc
            i += nj;
            GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 22, deph[0]); // delta raidal
            i += 22;
            GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 20, deph[1]); // delta along-track
            i += 20;
            GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 20, deph[2]); // delta cross-track
            i += 20;
            GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 21, ddeph[0]); // dot delta radial
            i += 21;
            GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 19, ddeph[1]); // dot delta along-track
            i += 19;
            GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 19, ddeph[2]); // dot delta cross-track
            i += 19;
            GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 22, dclk[0]); // delta clock c0
            i += 22;
            GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 21, dclk[1]); // delta clock c1
            i += 21;
            GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 27, dclk[2]); // delta clock c2
            i += 27;
        }
        rtcm.nbit = i;
        return 1;
    }
    /* encode ssr 5: ura ---------------------------------------------------------*/
    internal static int encode_ssr5(rtcm_t rtcm, int sys, int sync)
    {
        double udint = 0.0;
        int i;
        int j;
        int nsat;
        int iod = 0;
        int prn;
        int ura;
        int np;
        int offp;

        GlobalMembersRtkcmn.trace(3, "encode_ssr5: sys=%d sync=%d\n", sys, sync);

        switch (sys)
        {
            case DefineConstants.SYS_GPS:
                np = 6;
                offp = 0;
                break;
            case DefineConstants.SYS_GLO:
                np = 5;
                offp = 0;
                break;
            case DefineConstants.SYS_GAL:
                np = 6;
                offp = 0;
                break;
            case DefineConstants.SYS_QZS:
                np = 4;
                offp = 192;
                break;
            case DefineConstants.SYS_CMP:
                np = 6;
                offp = 1;
                break;
            case DefineConstants.SYS_SBS:
                np = 6;
                offp = 120;
                break;
            default:
                return 0;
        }
        /* number of satellites */
        for (j = nsat = 0; j < DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1; j++)
        {
            if (GlobalMembersRtkcmn.satsys(j + 1, ref prn) != sys || rtcm.ssr[j].update == 0)
                continue;
            nsat++;
            udint = rtcm.ssr[j].udi[3];
            iod = rtcm.ssr[j].iod[3];
        }
        /* encode ssr header */
        i = GlobalMembersRtcm3e.encode_ssr_head(5, rtcm, sys, nsat, sync, iod, udint, 0, 0, 0);

        for (j = 0; j < DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1; j++)
        {
            if (GlobalMembersRtkcmn.satsys(j + 1, ref prn) != sys || rtcm.ssr[j].update == 0)
                continue;

            ura = rtcm.ssr[j].ura;
            GlobalMembersRtkcmn.setbitu(rtcm.buff, i, np, prn - offp); // satellite id
            i += np;
            GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 6, ura); // ssr ura
            i += 6;
        }
        rtcm.nbit = i;
        return 1;
    }
    /* encode ssr 6: high rate clock correction ----------------------------------*/
    internal static int encode_ssr6(rtcm_t rtcm, int sys, int sync)
    {
        double udint = 0.0;
        int i;
        int j;
        int nsat;
        int iod = 0;
        int prn;
        int hrclk;
        int np;
        int offp;

        GlobalMembersRtkcmn.trace(3, "encode_ssr6: sys=%d sync=%d\n", sys, sync);

        switch (sys)
        {
            case DefineConstants.SYS_GPS:
                np = 6;
                offp = 0;
                break;
            case DefineConstants.SYS_GLO:
                np = 5;
                offp = 0;
                break;
            case DefineConstants.SYS_GAL:
                np = 6;
                offp = 0;
                break;
            case DefineConstants.SYS_QZS:
                np = 4;
                offp = 192;
                break;
            case DefineConstants.SYS_CMP:
                np = 6;
                offp = 1;
                break;
            case DefineConstants.SYS_SBS:
                np = 6;
                offp = 120;
                break;
            default:
                return 0;
        }
        /* number of satellites */
        for (j = nsat = 0; j < DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1; j++)
        {
            if (GlobalMembersRtkcmn.satsys(j + 1, ref prn) != sys || rtcm.ssr[j].update == 0)
                continue;
            nsat++;
            udint = rtcm.ssr[j].udi[2];
            iod = rtcm.ssr[j].iod[2];
        }
        /* encode ssr header */
        i = GlobalMembersRtcm3e.encode_ssr_head(6, rtcm, sys, nsat, sync, iod, udint, 0, 0, 0);

        for (j = 0; j < DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1; j++)
        {
            if (GlobalMembersRtkcmn.satsys(j + 1, ref prn) != sys || rtcm.ssr[j].update == 0)
                continue;

            hrclk = ((int)Math.Floor((rtcm.ssr[j].hrclk / 1E-4) + 0.5));

            GlobalMembersRtkcmn.setbitu(rtcm.buff, i, np, prn - offp); // satellite id
            i += np;
            GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 22, hrclk); // high rate clock corr
            i += 22;
        }
        rtcm.nbit = i;
        return 1;
    }
    /* satellite no to msm satellite id ------------------------------------------*/
    internal static int to_satid(int sys, int sat)
    {
        int prn;

        if (GlobalMembersRtkcmn.satsys(sat, ref prn) != sys)
        {
            return 0;
        }

        if (sys == DefineConstants.SYS_QZS)
        {
            prn -= DefineConstants.MINPRNQZS - 1;
        }
        else if (sys == DefineConstants.SYS_SBS)
        {
            prn -= DefineConstants.MINPRNSBS - 1;
        }

        return prn;
    }
    /* observation code to msm signal id -----------------------------------------*/
    internal static int to_sigid(int sys, byte code, ref int freq)
    {
        string[] msm_sig;
        string sig;
        int i;

        /* signal conversion for undefined signal by rtcm */
        if (sys == DefineConstants.SYS_GPS)
        {
            if (code == DefineConstants.CODE_L1Y)
            {
                code = DefineConstants.CODE_L1P;
            }
            else if (code == DefineConstants.CODE_L1M)
            {
                code = DefineConstants.CODE_L1P;
            }
            else if (code == DefineConstants.CODE_L1N)
            {
                code = DefineConstants.CODE_L1P;
            }
            else if (code == DefineConstants.CODE_L2D)
            {
                code = DefineConstants.CODE_L2P;
            }
            else if (code == DefineConstants.CODE_L2Y)
            {
                code = DefineConstants.CODE_L2P;
            }
            else if (code == DefineConstants.CODE_L2M)
            {
                code = DefineConstants.CODE_L2P;
            }
            else if (code == DefineConstants.CODE_L2N)
            {
                code = DefineConstants.CODE_L2P;
            }
        }
        if ((sig = GlobalMembersRtkcmn.code2obs(code, ref freq)) == 0)
        {
            return 0;
        }

        switch (sys)
        {
            case DefineConstants.SYS_GPS:
                msm_sig = GlobalMembersRtcm3.msm_sig_gps;
                break;
            case DefineConstants.SYS_GLO:
                msm_sig = GlobalMembersRtcm3.msm_sig_glo;
                break;
            case DefineConstants.SYS_GAL:
                msm_sig = GlobalMembersRtcm3.msm_sig_gal;
                break;
            case DefineConstants.SYS_QZS:
                msm_sig = GlobalMembersRtcm3.msm_sig_qzs;
                break;
            case DefineConstants.SYS_SBS:
                msm_sig = GlobalMembersRtcm3.msm_sig_sbs;
                break;
            case DefineConstants.SYS_CMP:
                msm_sig = GlobalMembersRtcm3.msm_sig_cmp;
                break;
            default:
                return 0;
        }
        for (i = 0; i < 32; i++)
        {
            if (!string.Compare(sig, msm_sig[i]))
            {
                return i + 1;
            }
        }
        return 0;
    }
    /* generate msm satellite, signal and cell index -----------------------------*/
    internal static void gen_msm_index(rtcm_t rtcm, int sys, ref int nsat, ref int nsig, ref int ncell, byte[] sat_ind, byte[] sig_ind, byte[] cell_ind)
    {
        int i;
        int j;
        int sat;
        int sig;
        int cell;
        int f;

        nsat = nsig = ncell = 0;

        /* generate satellite and signal index */
        for (i = 0; i < rtcm.obs.n; i++)
        {
            if ((sat = GlobalMembersRtcm3e.to_satid(sys, rtcm.obs.data[i].sat)) == 0)
                continue;

            for (j = 0; j < DefineConstants.NFREQ + DefineConstants.NEXOBS; j++)
            {
                if ((sig = GlobalMembersRtcm3e.to_sigid(sys, rtcm.obs.data[i].code[j], ref f)) == 0)
                    continue;

                sat_ind[sat - 1] = sig_ind[sig - 1] = 1;
            }
        }
        for (i = 0; i < 64; i++)
        {
            if (sat_ind[i])
            {
                sat_ind[i] = ++nsat;
            }
        }
        for (i = 0; i < 32; i++)
        {
            if (sig_ind[i])
            {
                sig_ind[i] = ++nsig;
            }
        }
        /* generate cell index */
        for (i = 0; i < rtcm.obs.n; i++)
        {
            if ((sat = GlobalMembersRtcm3e.to_satid(sys, rtcm.obs.data[i].sat)) == 0)
                continue;

            for (j = 0; j < DefineConstants.NFREQ + DefineConstants.NEXOBS; j++)
            {
                if ((sig = GlobalMembersRtcm3e.to_sigid(sys, rtcm.obs.data[i].code[j], ref f)) == 0)
                    continue;

                cell = sig_ind[sig - 1] - 1 + (sat_ind[sat - 1] - 1) * nsig;
                cell_ind[cell] = 1;
            }
        }
        for (i = 0; i < nsat * nsig; i++)
        {
            if (cell_ind[i] && ncell < 64)
            {
                cell_ind[i] = ++ncell;
            }
        }
    }
    /* generate msm satellite data fields ----------------------------------------*/
    internal static void gen_msm_sat(rtcm_t rtcm, int sys, int nsat, byte[] sat_ind, double[] rrng, double[] rrate, byte[] info)
    {
        obsd_t data;
        double lambda;
        double rrng_s;
        double rrate_s;
        int i;
        int j;
        int k;
        int sat;
        int sig;
        int f;
        int fcn;

        for (i = 0; i < 64; i++)
        {
            rrng[i] = rrate[i] = 0.0;
        }

        for (i = 0; i < rtcm.obs.n; i++)
        {
            data = rtcm.obs.data + i;
            if ((sat = GlobalMembersRtcm3e.to_satid(sys, data.sat)) == 0)
                continue;
            fcn = GlobalMembersRtcm3e.fcn_glo(data.sat, rtcm);

            for (j = 0; j < DefineConstants.NFREQ + DefineConstants.NEXOBS; j++)
            {
                if ((sig = GlobalMembersRtcm3e.to_sigid(sys, data.code[j], ref f)) == 0)
                    continue;
                k = sat_ind[sat - 1] - 1;
                lambda = GlobalMembersRtkcmn.satwavelen(data.sat, f - 1, rtcm.nav);

                /* rough range (ms) and rough phase-range-rate (m/s) */
                rrng_s = ((int)Math.Floor((data.P[j] / DefineConstants.CLIGHT * 0.001 / DefineConstants.P2_10) + 0.5)) * DefineConstants.CLIGHT * 0.001 * DefineConstants.P2_10;
                rrate_s = ((int)Math.Floor((-data.D[j] * lambda) + 0.5)) * 1.0;
                if (rrng[k] == 0.0 && data.P[j] != 0.0)
                {
                    rrng[k] = rrng_s;
                }
                if (rrate[k] == 0.0 && data.D[j] != 0.0)
                {
                    rrate[k] = rrate_s;
                }

                /* extended satellite info */
                info[k] = sys != DefineConstants.SYS_GLO ? 0 : (fcn < 0 ? 15 : fcn);
            }
        }
    }
    /* generate msm signal data fields -------------------------------------------*/
    internal static void gen_msm_sig(rtcm_t rtcm, int sys, int nsat, int nsig, int ncell, byte[] sat_ind, byte[] sig_ind, byte[] cell_ind, double[] rrng, double[] rrate, double[] psrng, double[] phrng, double[] rate, int[] @lock, byte[] half, float[] cnr)
    {
        obsd_t data;
        double lambda;
        double psrng_s;
        double phrng_s;
        double rate_s;
        int i;
        int j;
        int k;
        int sat;
        int sig;
        int cell;
        int f;
        int lt;
        int LLI;

        for (i = 0; i < ncell; i++)
        {
            if (psrng != 0)
            {
                psrng[i] = 0.0;
            }
            if (phrng != 0)
            {
                phrng[i] = 0.0;
            }
            if (rate != 0)
            {
                rate[i] = 0.0;
            }
        }
        for (i = 0; i < rtcm.obs.n; i++)
        {
            data = rtcm.obs.data + i;

            if ((sat = GlobalMembersRtcm3e.to_satid(sys, data.sat)) == 0)
                continue;

            for (j = 0; j < DefineConstants.NFREQ + DefineConstants.NEXOBS; j++)
            {
                if ((sig = GlobalMembersRtcm3e.to_sigid(sys, data.code[j], ref f)) == 0)
                    continue;
                k = sat_ind[sat - 1] - 1;
                if ((cell = cell_ind[sig_ind[sig - 1] - 1 + k * nsig]) >= 64)
                    continue;

                lambda = GlobalMembersRtkcmn.satwavelen(data.sat, f - 1, rtcm.nav);
                psrng_s = data.P[j] == 0.0 ? 0.0 : data.P[j] - rrng[k];
                phrng_s = data.L[j] == 0.0 || lambda <= 0.0 ? 0.0 : data.L[j] * lambda - rrng[k];
                rate_s = data.D[j] == 0.0 || lambda <= 0.0 ? 0.0 : -data.D[j] * lambda - rrate[k];

                /* subtract phase - psudorange integer cycle offset */
                LLI = data.LLI[j];
                if ((LLI & 1) || Math.Abs(phrng_s - rtcm.cp[data.sat - 1, j]) > 1171.0)
                {
                    rtcm.cp[data.sat - 1, j] = ((int)Math.Floor((phrng_s / lambda) + 0.5)) * lambda;
                    LLI |= 1;
                }
                phrng_s -= rtcm.cp[data.sat - 1, j];

                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: lt=locktime(data->time,rtcm->lltime[data->sat-1]+j,LLI);
                lt = GlobalMembersRtcm3e.locktime(new gtime_t(data.time), rtcm.lltime[data.sat - 1] + j, LLI);

                if (psrng != 0 && psrng_s != 0.0)
                {
                    psrng[cell - 1] = psrng_s;
                }
                if (phrng != 0 && phrng_s != 0.0)
                {
                    phrng[cell - 1] = phrng_s;
                }
                if (rate != 0 && rate_s != 0.0)
                {
                    rate[cell - 1] = rate_s;
                }
                if (@lock != 0)
                {
                    @lock[cell - 1] = lt;
                }
                if (half != 0)
                {
                    half[cell - 1] = (data.LLI[j] & 2) ? 1 : 0;
                }
                if (cnr != 0F)
                {
                    cnr[cell - 1] = (float)(data.SNR[j] * 0.25);
                }
            }
        }
    }
    /* encode msm header ---------------------------------------------------------*/
    internal static int encode_msm_head(int type, rtcm_t rtcm, int sys, int sync, ref int nsat, ref int ncell, ref double rrng, ref double rrate, ref byte info, ref double psrng, ref double phrng, ref double rate, ref int @lock, ref byte half, ref float cnr)
    {
        double tow;
        byte[] sat_ind = new byte[64];
        byte[] sig_ind = new byte[32];
        byte[] cell_ind = new byte[32 * 64];
        uint dow;
        uint epoch;
        int i = 24;
        int j;
        int tt;
        int nsig = 0;

        switch (sys)
        {
            case DefineConstants.SYS_GPS:
                type += 1070;
                break;
            case DefineConstants.SYS_GLO:
                type += 1080;
                break;
            case DefineConstants.SYS_GAL:
                type += 1090;
                break;
            case DefineConstants.SYS_QZS:
                type += 1110;
                break;
            case DefineConstants.SYS_SBS:
                type += 1100;
                break;
            case DefineConstants.SYS_CMP:
                type += 1120;
                break;
            default:
                return 0;
        }
        /* generate msm satellite, signal and cell index */
        GlobalMembersRtcm3e.gen_msm_index(rtcm, sys, ref nsat, ref nsig, ref ncell, sat_ind, sig_ind, cell_ind);

        if (sys == DefineConstants.SYS_GLO)
        {
            /* glonass time (dow + tod-ms) */
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: tow=time2gpst(timeadd(gpst2utc(rtcm->time),10800.0),null);
            tow = GlobalMembersRtkcmn.time2gpst(GlobalMembersRtkcmn.timeadd(GlobalMembersRtkcmn.gpst2utc(new gtime_t(rtcm.time)), 10800.0), null);
            dow = (uint)(tow / 86400.0);
            epoch = (dow << 27) + ((uint)Math.Floor((Math.IEEERemainder(tow, 86400.0) * 1E3) + 0.5));
        }
        else if (sys == DefineConstants.SYS_CMP)
        {
            /* beidou time (tow-ms) */
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: epoch=((uint)floor((time2gpst(gpst2bdt(rtcm->time),null)*1E3)+0.5));
            epoch = ((uint)Math.Floor((GlobalMembersRtkcmn.time2gpst(GlobalMembersRtkcmn.gpst2bdt(new gtime_t(rtcm.time)), null) * 1E3) + 0.5));
        }
        else
        {
            /* gps, qzs and galileo time (tow-ms) */
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: epoch=((uint)floor((time2gpst(rtcm->time,null)*1E3)+0.5));
            epoch = ((uint)Math.Floor((GlobalMembersRtkcmn.time2gpst(new gtime_t(rtcm.time), null) * 1E3) + 0.5));
        }
        /* cumulative session transmitting time (s) */
        //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
        //ORIGINAL LINE: tt=locktime(rtcm->time,&rtcm->time_s,0);
        tt = GlobalMembersRtcm3e.locktime(new gtime_t(rtcm.time), rtcm.time_s, 0);

        /* encode msm header (ref [11] table 3.5-73) */
        GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 12, type); // message number
        i += 12;
        GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 12, rtcm.staid); // reference station id
        i += 12;
        GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 30, epoch); // epoch time
        i += 30;
        GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 1, sync); // multiple message bit
        i += 1;
        GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 3, rtcm.seqno); // issue of data station
        i += 3;
        GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 7, GlobalMembersRtcm3e.to_lock(tt)); // session time indicator
        i += 7;
        GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 2, 0); // clock streering indicator
        i += 2;
        GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 2, 0); // external clock indicator
        i += 2;
        GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 1, 0); // smoothing indicator
        i += 1;
        GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 3, 0); // smoothing interval
        i += 3;

        /* satellite mask */
        for (j = 0; j < 64; j++)
        {
            GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 1, sat_ind[j] != 0 ? 1 : 0);
            i += 1;
        }
        /* signal mask */
        for (j = 0; j < 32; j++)
        {
            GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 1, sig_ind[j] != 0 ? 1 : 0);
            i += 1;
        }
        /* cell mask */
        for (j = 0; j < nsat * nsig && j < 64; j++)
        {
            GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 1, cell_ind[j] != 0 ? 1 : 0);
            i += 1;
        }
        /* generate msm satellite data fields */
        GlobalMembersRtcm3e.gen_msm_sat(rtcm, sys, nsat, sat_ind, rrng, rrate, info);

        /* generate msm signal data fields */
        GlobalMembersRtcm3e.gen_msm_sig(rtcm, sys, nsat, nsig, ncell, sat_ind, sig_ind, cell_ind, rrng, rrate, psrng, phrng, rate, @lock, half, cnr);

        return i;
    }
    /* encode rough range integer ms ---------------------------------------------*/
    internal static int encode_msm_int_rrng(rtcm_t rtcm, int i, double[] rrng, int nsat)
    {
        uint int_ms;
        int j;

        for (j = 0; j < nsat; j++)
        {
            if (rrng[j] == 0.0)
            {
                int_ms = 255;
            }
            else if (rrng[j] < 0.0 || rrng[j] > DefineConstants.CLIGHT * 0.001 * 255.0)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: trace(2,"msm rough range overflow %s rrng=%.3f\n", time_str(rtcm->time,0),rrng[j]);
                GlobalMembersRtkcmn.trace(2, "msm rough range overflow %s rrng=%.3f\n", GlobalMembersRtkcmn.time_str(new gtime_t(rtcm.time), 0), rrng[j]);
                int_ms = 255;
            }
            else
            {
                int_ms = ((uint)Math.Floor((rrng[j] / DefineConstants.CLIGHT * 0.001 / DefineConstants.P2_10) + 0.5)) >> 10;
            }
            GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 8, int_ms);
            i += 8;
        }
        return i;
    }
    /* encode rough range modulo 1 ms --------------------------------------------*/
    internal static int encode_msm_mod_rrng(rtcm_t rtcm, int i, double[] rrng, int nsat)
    {
        uint mod_ms;
        int j;

        for (j = 0; j < nsat; j++)
        {
            if (rrng[j] <= 0.0 || rrng[j] > DefineConstants.CLIGHT * 0.001 * 255.0)
            {
                mod_ms = 0;
            }
            else
            {
                mod_ms = ((uint)Math.Floor((rrng[j] / DefineConstants.CLIGHT * 0.001 / DefineConstants.P2_10) + 0.5)) & 0x3FFu;
            }
            GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 10, mod_ms);
            i += 10;
        }
        return i;
    }
    /* encode extended satellite info --------------------------------------------*/
    internal static int encode_msm_info(rtcm_t rtcm, int i, byte[] info, int nsat)
    {
        int j;

        for (j = 0; j < nsat; j++)
        {
            GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 4, info[j]);
            i += 4;
        }
        return i;
    }
    /* encode rough phase-range-rate ---------------------------------------------*/
    internal static int encode_msm_rrate(rtcm_t rtcm, int i, double[] rrate, int nsat)
    {
        int j;
        int rrate_val;

        for (j = 0; j < nsat; j++)
        {
            if (Math.Abs(rrate[j]) > 8191.0)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: trace(2,"msm rough phase-range-rate overflow %s rrate=%.4f\n", time_str(rtcm->time,0),rrate[j]);
                GlobalMembersRtkcmn.trace(2, "msm rough phase-range-rate overflow %s rrate=%.4f\n", GlobalMembersRtkcmn.time_str(new gtime_t(rtcm.time), 0), rrate[j]);
                rrate_val = -8192;
            }
            else
            {
                rrate_val = ((int)Math.Floor((rrate[j] / 1.0) + 0.5));
            }
            GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 14, rrate_val);
            i += 14;
        }
        return i;
    }
    /* encode fine pseudorange ---------------------------------------------------*/
    internal static int encode_msm_psrng(rtcm_t rtcm, int i, double[] psrng, int ncell)
    {
        int j;
        int psrng_val;

        for (j = 0; j < ncell; j++)
        {
            if (psrng[j] == 0.0)
            {
                psrng_val = -16384;
            }
            else if (Math.Abs(psrng[j]) > 292.7)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: trace(2,"msm fine pseudorange overflow %s psrng=%.3f\n", time_str(rtcm->time,0),psrng[j]);
                GlobalMembersRtkcmn.trace(2, "msm fine pseudorange overflow %s psrng=%.3f\n", GlobalMembersRtkcmn.time_str(new gtime_t(rtcm.time), 0), psrng[j]);
                psrng_val = -16384;
            }
            else
            {
                psrng_val = ((int)Math.Floor((psrng[j] / DefineConstants.CLIGHT * 0.001 / 5.960464477539063E-0x8) + 0.5));
    }
    GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 15, psrng_val);
			i += 15;
		}
		return i;
	}
	/* encode fine pseudorange with extended resolution --------------------------*/
	internal static int encode_msm_psrng_ex(rtcm_t rtcm, int i, double[] psrng, int ncell)
{
    int j;
    int psrng_val;

    for (j = 0; j < ncell; j++)
    {
        if (psrng[j] == 0.0)
        {
            psrng_val = -524288;
        }
        else if (Math.Abs(psrng[j]) > 292.7)
        {
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: trace(2,"msm fine pseudorange ext overflow %s psrng=%.3f\n", time_str(rtcm->time,0),psrng[j]);
            GlobalMembersRtkcmn.trace(2, "msm fine pseudorange ext overflow %s psrng=%.3f\n", GlobalMembersRtkcmn.time_str(new gtime_t(rtcm.time), 0), psrng[j]);
            psrng_val = -524288;
        }
        else
        {
            psrng_val = ((int)Math.Floor((psrng[j] / DefineConstants.CLIGHT * 0.001 / 1.862645149230957E-0x9) + 0.5));
}
GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 20, psrng_val);
			i += 20;
		}
		return i;
	}
	/* encode fine phase-range ---------------------------------------------------*/
	internal static int encode_msm_phrng(rtcm_t rtcm, int i, double[] phrng, int ncell)
{
    int j;
    int phrng_val;

    for (j = 0; j < ncell; j++)
    {
        if (phrng[j] == 0.0)
        {
            phrng_val = -2097152;
        }
        else if (Math.Abs(phrng[j]) > 1171.0)
        {
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: trace(2,"msm fine phase-range overflow %s phrng=%.3f\n", time_str(rtcm->time,0),phrng[j]);
            GlobalMembersRtkcmn.trace(2, "msm fine phase-range overflow %s phrng=%.3f\n", GlobalMembersRtkcmn.time_str(new gtime_t(rtcm.time), 0), phrng[j]);
            phrng_val = -2097152;
        }
        else
        {
            phrng_val = ((int)Math.Floor((phrng[j] / DefineConstants.CLIGHT * 0.001 / 1.862645149230957E-0x9) + 0.5));
}
GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 22, phrng_val);
			i += 22;
		}
		return i;
	}
	/* encode fine phase-range with extended resolution --------------------------*/
	internal static int encode_msm_phrng_ex(rtcm_t rtcm, int i, double[] phrng, int ncell)
{
    int j;
    int phrng_val;

    for (j = 0; j < ncell; j++)
    {
        if (phrng[j] == 0.0)
        {
            phrng_val = -8388608;
        }
        else if (Math.Abs(phrng[j]) > 1171.0)
        {
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: trace(2,"msm fine phase-range ext overflow %s phrng=%.3f\n", time_str(rtcm->time,0),phrng[j]);
            GlobalMembersRtkcmn.trace(2, "msm fine phase-range ext overflow %s phrng=%.3f\n", GlobalMembersRtkcmn.time_str(new gtime_t(rtcm.time), 0), phrng[j]);
            phrng_val = -8388608;
        }
        else
        {
            phrng_val = ((int)Math.Floor((phrng[j] / DefineConstants.CLIGHT * 0.001 / DefineConstants.P2_31) + 0.5));
        }
        GlobalMembersRtkcmn.setbits(ref rtcm.buff, i, 24, phrng_val);
        i += 24;
    }
    return i;
}
/* encode lock-time indicator ------------------------------------------------*/
internal static int encode_msm_lock(rtcm_t rtcm, int i, int[] @lock, int ncell)
{
    int j;
    int lock_val;

    for (j = 0; j < ncell; j++)
    {
        lock_val = GlobalMembersRtcm3e.to_msm_lock(@lock[j]);
        GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 4, lock_val);
        i += 4;
    }
    return i;
}
/* encode lock-time indicator with extended range and resolution -------------*/
internal static int encode_msm_lock_ex(rtcm_t rtcm, int i, int[] @lock, int ncell)
{
    int j;
    int lock_val;

    for (j = 0; j < ncell; j++)
    {
        lock_val = GlobalMembersRtcm3e.to_msm_lock_ex(@lock[j]);
        GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 10, lock_val);
        i += 10;
    }
    return i;
}
/* encode half-cycle-ambiguity indicator -------------------------------------*/
internal static int encode_msm_half_amb(rtcm_t rtcm, int i, byte[] half, int ncell)
{
    int j;

    for (j = 0; j < ncell; j++)
    {
        GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 1, half[j]);
        i += 1;
    }
    return i;
}
/* encode signal cnr ---------------------------------------------------------*/
internal static int encode_msm_cnr(rtcm_t rtcm, int i, float[] cnr, int ncell)
{
    int j;
    int cnr_val;

    for (j = 0; j < ncell; j++)
    {
        cnr_val = ((int)Math.Floor((cnr[j] / 1.0) + 0.5));
        GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 6, cnr_val);
        i += 6;
    }
    return i;
}
/* encode signal cnr with extended resolution --------------------------------*/
internal static int encode_msm_cnr_ex(rtcm_t rtcm, int i, float[] cnr, int ncell)
{
    int j;
    int cnr_val;

    for (j = 0; j < ncell; j++)
    {
        cnr_val = ((int)Math.Floor((cnr[j] / 0.0625) + 0.5));
        GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 10, cnr_val);
        i += 10;
    }
    return i;
}
/* encode fine phase-range-rate ----------------------------------------------*/
internal static int encode_msm_rate(rtcm_t rtcm, int i, double[] rate, int ncell)
{
    int j;
    int rate_val;

    for (j = 0; j < ncell; j++)
    {
        if (rate[j] == 0.0)
        {
            rate_val = -16384;
        }
        else if (Math.Abs(rate[j]) > 1.6384)
        {
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: trace(2,"msm fine phase-range-rate overflow %s rate=%.3f\n", time_str(rtcm->time,0),rate[j]);
            GlobalMembersRtkcmn.trace(2, "msm fine phase-range-rate overflow %s rate=%.3f\n", GlobalMembersRtkcmn.time_str(new gtime_t(rtcm.time), 0), rate[j]);
            rate_val = -16384;
        }
        else
        {
            rate_val = ((int)Math.Floor((rate[j] / 0.0001) + 0.5));
        }
        GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 15, rate_val);
        i += 15;
    }
    return i;
}
/* encode msm 1: compact pseudorange -----------------------------------------*/
internal static int encode_msm1(rtcm_t rtcm, int sys, int sync)
{
    double[] rrng = new double[64];
    double[] rrate = new double[64];
    double[] psrng = new double[64];
    int i;
    int nsat;
    int ncell;

    GlobalMembersRtkcmn.trace(3, "encode_msm1: sys=%d sync=%d\n", sys, sync);

    /* encode msm header */
    if ((i = GlobalMembersRtcm3e.encode_msm_head(1, rtcm, sys, sync, ref nsat, ref ncell, ref rrng, ref rrate, null, ref psrng, null, null, null, null, null)) == 0)
    {
        return 0;
    }
    /* encode msm satellite data */
    i = GlobalMembersRtcm3e.encode_msm_mod_rrng(rtcm, i, rrng, nsat); // rough range modulo 1 ms

    /* encode msm signal data */
    i = GlobalMembersRtcm3e.encode_msm_psrng(rtcm, i, psrng, ncell); // fine pseudorange

    rtcm.nbit = i;
    return 1;
}
/* encode msm 2: compact phaserange ------------------------------------------*/
internal static int encode_msm2(rtcm_t rtcm, int sys, int sync)
{
    double[] rrng = new double[64];
    double[] rrate = new double[64];
    double[] phrng = new double[64];
    byte[] half = new byte[64];
    int i;
    int nsat;
    int ncell;
    int[] @lock = new int[64];

    GlobalMembersRtkcmn.trace(3, "encode_msm2: sys=%d sync=%d\n", sys, sync);

    /* encode msm header */
    if ((i = GlobalMembersRtcm3e.encode_msm_head(2, rtcm, sys, sync, ref nsat, ref ncell, ref rrng, ref rrate, null, null, ref phrng, null, ref @lock, ref half, null)) == 0)
    {
        return 0;
    }
    /* encode msm satellite data */
    i = GlobalMembersRtcm3e.encode_msm_mod_rrng(rtcm, i, rrng, nsat); // rough range modulo 1 ms

    /* encode msm signal data */
    i = GlobalMembersRtcm3e.encode_msm_phrng(rtcm, i, phrng, ncell); // fine phase-range
    i = GlobalMembersRtcm3e.encode_msm_lock(rtcm, i, @lock, ncell); // lock-time indicator
    i = GlobalMembersRtcm3e.encode_msm_half_amb(rtcm, i, half, ncell); // half-cycle-amb indicator

    rtcm.nbit = i;
    return 1;
}
/* encode msm 3: compact pseudorange and phaserange --------------------------*/
internal static int encode_msm3(rtcm_t rtcm, int sys, int sync)
{
    double[] rrng = new double[64];
    double[] rrate = new double[64];
    double[] psrng = new double[64];
    double[] phrng = new double[64];
    byte[] half = new byte[64];
    int i;
    int nsat;
    int ncell;
    int[] @lock = new int[64];

    GlobalMembersRtkcmn.trace(3, "encode_msm3: sys=%d sync=%d\n", sys, sync);

    /* encode msm header */
    if ((i = GlobalMembersRtcm3e.encode_msm_head(3, rtcm, sys, sync, ref nsat, ref ncell, ref rrng, ref rrate, null, ref psrng, ref phrng, null, ref @lock, ref half, null)) == 0)
    {
        return 0;
    }
    /* encode msm satellite data */
    i = GlobalMembersRtcm3e.encode_msm_mod_rrng(rtcm, i, rrng, nsat); // rough range modulo 1 ms

    /* encode msm signal data */
    i = GlobalMembersRtcm3e.encode_msm_psrng(rtcm, i, psrng, ncell); // fine pseudorange
    i = GlobalMembersRtcm3e.encode_msm_phrng(rtcm, i, phrng, ncell); // fine phase-range
    i = GlobalMembersRtcm3e.encode_msm_lock(rtcm, i, @lock, ncell); // lock-time indicator
    i = GlobalMembersRtcm3e.encode_msm_half_amb(rtcm, i, half, ncell); // half-cycle-amb indicator

    rtcm.nbit = i;
    return 1;
}
/* encode msm 4: full pseudorange and phaserange plus cnr --------------------*/
internal static int encode_msm4(rtcm_t rtcm, int sys, int sync)
{
    double[] rrng = new double[64];
    double[] rrate = new double[64];
    double[] psrng = new double[64];
    double[] phrng = new double[64];
    float[] cnr = new float[64];
    byte[] half = new byte[64];
    int i;
    int nsat;
    int ncell;
    int[] @lock = new int[64];

    GlobalMembersRtkcmn.trace(3, "encode_msm4: sys=%d sync=%d\n", sys, sync);

    /* encode msm header */
    if ((i = GlobalMembersRtcm3e.encode_msm_head(4, rtcm, sys, sync, ref nsat, ref ncell, ref rrng, ref rrate, null, ref psrng, ref phrng, null, ref @lock, ref half, ref cnr)) == 0)
    {
        return 0;
    }
    /* encode msm satellite data */
    i = GlobalMembersRtcm3e.encode_msm_int_rrng(rtcm, i, rrng, nsat); // rough range integer ms
    i = GlobalMembersRtcm3e.encode_msm_mod_rrng(rtcm, i, rrng, nsat); // rough range modulo 1 ms

    /* encode msm signal data */
    i = GlobalMembersRtcm3e.encode_msm_psrng(rtcm, i, psrng, ncell); // fine pseudorange
    i = GlobalMembersRtcm3e.encode_msm_phrng(rtcm, i, phrng, ncell); // fine phase-range
    i = GlobalMembersRtcm3e.encode_msm_lock(rtcm, i, @lock, ncell); // lock-time indicator
    i = GlobalMembersRtcm3e.encode_msm_half_amb(rtcm, i, half, ncell); // half-cycle-amb indicator
    i = GlobalMembersRtcm3e.encode_msm_cnr(rtcm, i, cnr, ncell); // signal cnr
    rtcm.nbit = i;
    return 1;
}
/* encode msm 5: full pseudorange, phaserange, phaserangerate and cnr --------*/
internal static int encode_msm5(rtcm_t rtcm, int sys, int sync)
{
    double[] rrng = new double[64];
    double[] rrate = new double[64];
    double[] psrng = new double[64];
    double[] phrng = new double[64];
    double[] rate = new double[64];
    float[] cnr = new float[64];
    byte[] info = new byte[64];
    byte[] half = new byte[64];
    int i;
    int nsat;
    int ncell;
    int[] @lock = new int[64];

    GlobalMembersRtkcmn.trace(3, "encode_msm5: sys=%d sync=%d\n", sys, sync);

    /* encode msm header */
    if ((i = GlobalMembersRtcm3e.encode_msm_head(5, rtcm, sys, sync, ref nsat, ref ncell, ref rrng, ref rrate, ref info, ref psrng, ref phrng, ref rate, ref @lock, ref half, ref cnr)) == 0)
    {
        return 0;
    }
    /* encode msm satellite data */
    i = GlobalMembersRtcm3e.encode_msm_int_rrng(rtcm, i, rrng, nsat); // rough range integer ms
    i = GlobalMembersRtcm3e.encode_msm_info(rtcm, i, info, nsat); // extended satellite info
    i = GlobalMembersRtcm3e.encode_msm_mod_rrng(rtcm, i, rrng, nsat); // rough range modulo 1 ms
    i = GlobalMembersRtcm3e.encode_msm_rrate(rtcm, i, rrate, nsat); // rough phase-range-rate

    /* encode msm signal data */
    i = GlobalMembersRtcm3e.encode_msm_psrng(rtcm, i, psrng, ncell); // fine pseudorange
    i = GlobalMembersRtcm3e.encode_msm_phrng(rtcm, i, phrng, ncell); // fine phase-range
    i = GlobalMembersRtcm3e.encode_msm_lock(rtcm, i, @lock, ncell); // lock-time indicator
    i = GlobalMembersRtcm3e.encode_msm_half_amb(rtcm, i, half, ncell); // half-cycle-amb indicator
    i = GlobalMembersRtcm3e.encode_msm_cnr(rtcm, i, cnr, ncell); // signal cnr
    i = GlobalMembersRtcm3e.encode_msm_rate(rtcm, i, rate, ncell); // fine phase-range-rate
    rtcm.nbit = i;
    return 1;
}
/* encode msm 6: full pseudorange and phaserange plus cnr (high-res) ---------*/
internal static int encode_msm6(rtcm_t rtcm, int sys, int sync)
{
    double[] rrng = new double[64];
    double[] rrate = new double[64];
    double[] psrng = new double[64];
    double[] phrng = new double[64];
    float[] cnr = new float[64];
    byte[] half = new byte[64];
    int i;
    int nsat;
    int ncell;
    int[] @lock = new int[64];

    GlobalMembersRtkcmn.trace(3, "encode_msm6: sys=%d sync=%d\n", sys, sync);

    /* encode msm header */
    if ((i = GlobalMembersRtcm3e.encode_msm_head(6, rtcm, sys, sync, ref nsat, ref ncell, ref rrng, ref rrate, null, ref psrng, ref phrng, null, ref @lock, ref half, ref cnr)) == 0)
    {
        return 0;
    }
    /* encode msm satellite data */
    i = GlobalMembersRtcm3e.encode_msm_int_rrng(rtcm, i, rrng, nsat); // rough range integer ms
    i = GlobalMembersRtcm3e.encode_msm_mod_rrng(rtcm, i, rrng, nsat); // rough range modulo 1 ms

    /* encode msm signal data */
    i = GlobalMembersRtcm3e.encode_msm_psrng_ex(rtcm, i, psrng, ncell); // fine pseudorange ext
    i = GlobalMembersRtcm3e.encode_msm_phrng_ex(rtcm, i, phrng, ncell); // fine phase-range ext
    i = GlobalMembersRtcm3e.encode_msm_lock_ex(rtcm, i, @lock, ncell); // lock-time indicator ext
    i = GlobalMembersRtcm3e.encode_msm_half_amb(rtcm, i, half, ncell); // half-cycle-amb indicator
    i = GlobalMembersRtcm3e.encode_msm_cnr_ex(rtcm, i, cnr, ncell); // signal cnr ext
    rtcm.nbit = i;
    return 1;
}
/* encode msm 7: full pseudorange, phaserange, phaserangerate and cnr (h-res) */
internal static int encode_msm7(rtcm_t rtcm, int sys, int sync)
{
    double[] rrng = new double[64];
    double[] rrate = new double[64];
    double[] psrng = new double[64];
    double[] phrng = new double[64];
    double[] rate = new double[64];
    float[] cnr = new float[64];
    byte[] info = new byte[64];
    byte[] half = new byte[64];
    int i;
    int nsat;
    int ncell;
    int[] @lock = new int[64];

    GlobalMembersRtkcmn.trace(3, "encode_msm7: sys=%d sync=%d\n", sys, sync);

    /* encode msm header */
    if ((i = GlobalMembersRtcm3e.encode_msm_head(7, rtcm, sys, sync, ref nsat, ref ncell, ref rrng, ref rrate, ref info, ref psrng, ref phrng, ref rate, ref @lock, ref half, ref cnr)) == 0)
    {
        return 0;
    }
    /* encode msm satellite data */
    i = GlobalMembersRtcm3e.encode_msm_int_rrng(rtcm, i, rrng, nsat); // rough range integer ms
    i = GlobalMembersRtcm3e.encode_msm_info(rtcm, i, info, nsat); // extended satellite info
    i = GlobalMembersRtcm3e.encode_msm_mod_rrng(rtcm, i, rrng, nsat); // rough range modulo 1 ms
    i = GlobalMembersRtcm3e.encode_msm_rrate(rtcm, i, rrate, nsat); // rough phase-range-rate

    /* encode msm signal data */
    i = GlobalMembersRtcm3e.encode_msm_psrng_ex(rtcm, i, psrng, ncell); // fine pseudorange ext
    i = GlobalMembersRtcm3e.encode_msm_phrng_ex(rtcm, i, phrng, ncell); // fine phase-range ext
    i = GlobalMembersRtcm3e.encode_msm_lock_ex(rtcm, i, @lock, ncell); // lock-time indicator ext
    i = GlobalMembersRtcm3e.encode_msm_half_amb(rtcm, i, half, ncell); // half-cycle-amb indicator
    i = GlobalMembersRtcm3e.encode_msm_cnr_ex(rtcm, i, cnr, ncell); // signal cnr ext
    i = GlobalMembersRtcm3e.encode_msm_rate(rtcm, i, rate, ncell); // fine phase-range-rate
    rtcm.nbit = i;
    return 1;
}
/* encode rtcm ver.3 message -------------------------------------------------*/
public static int encode_rtcm3(rtcm_t rtcm, int type, int sync)
{
    int ret = 0;

    GlobalMembersRtkcmn.trace(3, "encode_rtcm3: type=%d sync=%d\n", type, sync);

    switch (type)
    {
        case 1001:
            ret = GlobalMembersRtcm3e.encode_type1001(rtcm, sync);
            break;
        case 1002:
            ret = GlobalMembersRtcm3e.encode_type1002(rtcm, sync);
            break;
        case 1003:
            ret = GlobalMembersRtcm3e.encode_type1003(rtcm, sync);
            break;
        case 1004:
            ret = GlobalMembersRtcm3e.encode_type1004(rtcm, sync);
            break;
        case 1005:
            ret = GlobalMembersRtcm3e.encode_type1005(rtcm, sync);
            break;
        case 1006:
            ret = GlobalMembersRtcm3e.encode_type1006(rtcm, sync);
            break;
        case 1007:
            ret = GlobalMembersRtcm3e.encode_type1007(rtcm, sync);
            break;
        case 1008:
            ret = GlobalMembersRtcm3e.encode_type1008(rtcm, sync);
            break;
        case 1009:
            ret = GlobalMembersRtcm3e.encode_type1009(rtcm, sync);
            break;
        case 1010:
            ret = GlobalMembersRtcm3e.encode_type1010(rtcm, sync);
            break;
        case 1011:
            ret = GlobalMembersRtcm3e.encode_type1011(rtcm, sync);
            break;
        case 1012:
            ret = GlobalMembersRtcm3e.encode_type1012(rtcm, sync);
            break;
        case 1019:
            ret = GlobalMembersRtcm3e.encode_type1019(rtcm, sync);
            break;
        case 1020:
            ret = GlobalMembersRtcm3e.encode_type1020(rtcm, sync);
            break;
        case 1033:
            ret = GlobalMembersRtcm3e.encode_type1033(rtcm, sync);
            break;
        case 1044:
            ret = GlobalMembersRtcm3e.encode_type1044(rtcm, sync);
            break;
        case 1045:
            ret = GlobalMembersRtcm3e.encode_type1045(rtcm, sync);
            break;
        case 1047: // tentative mt
            ret = GlobalMembersRtcm3e.encode_type1047(rtcm, sync);
            break;
        case 1057:
            ret = GlobalMembersRtcm3e.encode_ssr1(rtcm, DefineConstants.SYS_GPS, sync);
            break;
        case 1058:
            ret = GlobalMembersRtcm3e.encode_ssr2(rtcm, DefineConstants.SYS_GPS, sync);
            break;
        case 1059:
            ret = GlobalMembersRtcm3e.encode_ssr3(rtcm, DefineConstants.SYS_GPS, sync);
            break;
        case 1060:
            ret = GlobalMembersRtcm3e.encode_ssr4(rtcm, DefineConstants.SYS_GPS, sync);
            break;
        case 1061:
            ret = GlobalMembersRtcm3e.encode_ssr5(rtcm, DefineConstants.SYS_GPS, sync);
            break;
        case 1062:
            ret = GlobalMembersRtcm3e.encode_ssr6(rtcm, DefineConstants.SYS_GPS, sync);
            break;
        case 1063:
            ret = GlobalMembersRtcm3e.encode_ssr1(rtcm, DefineConstants.SYS_GLO, sync);
            break;
        case 1064:
            ret = GlobalMembersRtcm3e.encode_ssr2(rtcm, DefineConstants.SYS_GLO, sync);
            break;
        case 1065:
            ret = GlobalMembersRtcm3e.encode_ssr3(rtcm, DefineConstants.SYS_GLO, sync);
            break;
        case 1066:
            ret = GlobalMembersRtcm3e.encode_ssr4(rtcm, DefineConstants.SYS_GLO, sync);
            break;
        case 1067:
            ret = GlobalMembersRtcm3e.encode_ssr5(rtcm, DefineConstants.SYS_GLO, sync);
            break;
        case 1068:
            ret = GlobalMembersRtcm3e.encode_ssr6(rtcm, DefineConstants.SYS_GLO, sync);
            break;
        case 1071:
            ret = GlobalMembersRtcm3e.encode_msm1(rtcm, DefineConstants.SYS_GPS, sync);
            break;
        case 1072:
            ret = GlobalMembersRtcm3e.encode_msm2(rtcm, DefineConstants.SYS_GPS, sync);
            break;
        case 1073:
            ret = GlobalMembersRtcm3e.encode_msm3(rtcm, DefineConstants.SYS_GPS, sync);
            break;
        case 1074:
            ret = GlobalMembersRtcm3e.encode_msm4(rtcm, DefineConstants.SYS_GPS, sync);
            break;
        case 1075:
            ret = GlobalMembersRtcm3e.encode_msm5(rtcm, DefineConstants.SYS_GPS, sync);
            break;
        case 1076:
            ret = GlobalMembersRtcm3e.encode_msm6(rtcm, DefineConstants.SYS_GPS, sync);
            break;
        case 1077:
            ret = GlobalMembersRtcm3e.encode_msm7(rtcm, DefineConstants.SYS_GPS, sync);
            break;
        case 1081:
            ret = GlobalMembersRtcm3e.encode_msm1(rtcm, DefineConstants.SYS_GLO, sync);
            break;
        case 1082:
            ret = GlobalMembersRtcm3e.encode_msm2(rtcm, DefineConstants.SYS_GLO, sync);
            break;
        case 1083:
            ret = GlobalMembersRtcm3e.encode_msm3(rtcm, DefineConstants.SYS_GLO, sync);
            break;
        case 1084:
            ret = GlobalMembersRtcm3e.encode_msm4(rtcm, DefineConstants.SYS_GLO, sync);
            break;
        case 1085:
            ret = GlobalMembersRtcm3e.encode_msm5(rtcm, DefineConstants.SYS_GLO, sync);
            break;
        case 1086:
            ret = GlobalMembersRtcm3e.encode_msm6(rtcm, DefineConstants.SYS_GLO, sync);
            break;
        case 1087:
            ret = GlobalMembersRtcm3e.encode_msm7(rtcm, DefineConstants.SYS_GLO, sync);
            break;
        case 1091:
            ret = GlobalMembersRtcm3e.encode_msm1(rtcm, DefineConstants.SYS_GAL, sync);
            break;
        case 1092:
            ret = GlobalMembersRtcm3e.encode_msm2(rtcm, DefineConstants.SYS_GAL, sync);
            break;
        case 1093:
            ret = GlobalMembersRtcm3e.encode_msm3(rtcm, DefineConstants.SYS_GAL, sync);
            break;
        case 1094:
            ret = GlobalMembersRtcm3e.encode_msm4(rtcm, DefineConstants.SYS_GAL, sync);
            break;
        case 1095:
            ret = GlobalMembersRtcm3e.encode_msm5(rtcm, DefineConstants.SYS_GAL, sync);
            break;
        case 1096:
            ret = GlobalMembersRtcm3e.encode_msm6(rtcm, DefineConstants.SYS_GAL, sync);
            break;
        case 1097:
            ret = GlobalMembersRtcm3e.encode_msm7(rtcm, DefineConstants.SYS_GAL, sync);
            break;
        case 1101:
            ret = GlobalMembersRtcm3e.encode_msm1(rtcm, DefineConstants.SYS_SBS, sync);
            break;
        case 1102:
            ret = GlobalMembersRtcm3e.encode_msm2(rtcm, DefineConstants.SYS_SBS, sync);
            break;
        case 1103:
            ret = GlobalMembersRtcm3e.encode_msm3(rtcm, DefineConstants.SYS_SBS, sync);
            break;
        case 1104:
            ret = GlobalMembersRtcm3e.encode_msm4(rtcm, DefineConstants.SYS_SBS, sync);
            break;
        case 1105:
            ret = GlobalMembersRtcm3e.encode_msm5(rtcm, DefineConstants.SYS_SBS, sync);
            break;
        case 1106:
            ret = GlobalMembersRtcm3e.encode_msm6(rtcm, DefineConstants.SYS_SBS, sync);
            break;
        case 1107:
            ret = GlobalMembersRtcm3e.encode_msm7(rtcm, DefineConstants.SYS_SBS, sync);
            break;
        case 1111:
            ret = GlobalMembersRtcm3e.encode_msm1(rtcm, DefineConstants.SYS_QZS, sync);
            break;
        case 1112:
            ret = GlobalMembersRtcm3e.encode_msm2(rtcm, DefineConstants.SYS_QZS, sync);
            break;
        case 1113:
            ret = GlobalMembersRtcm3e.encode_msm3(rtcm, DefineConstants.SYS_QZS, sync);
            break;
        case 1114:
            ret = GlobalMembersRtcm3e.encode_msm4(rtcm, DefineConstants.SYS_QZS, sync);
            break;
        case 1115:
            ret = GlobalMembersRtcm3e.encode_msm5(rtcm, DefineConstants.SYS_QZS, sync);
            break;
        case 1116:
            ret = GlobalMembersRtcm3e.encode_msm6(rtcm, DefineConstants.SYS_QZS, sync);
            break;
        case 1117:
            ret = GlobalMembersRtcm3e.encode_msm7(rtcm, DefineConstants.SYS_QZS, sync);
            break;
        case 1121:
            ret = GlobalMembersRtcm3e.encode_msm1(rtcm, DefineConstants.SYS_CMP, sync);
            break;
        case 1122:
            ret = GlobalMembersRtcm3e.encode_msm2(rtcm, DefineConstants.SYS_CMP, sync);
            break;
        case 1123:
            ret = GlobalMembersRtcm3e.encode_msm3(rtcm, DefineConstants.SYS_CMP, sync);
            break;
        case 1124:
            ret = GlobalMembersRtcm3e.encode_msm4(rtcm, DefineConstants.SYS_CMP, sync);
            break;
        case 1125:
            ret = GlobalMembersRtcm3e.encode_msm5(rtcm, DefineConstants.SYS_CMP, sync);
            break;
        case 1126:
            ret = GlobalMembersRtcm3e.encode_msm6(rtcm, DefineConstants.SYS_CMP, sync);
            break;
        case 1127:
            ret = GlobalMembersRtcm3e.encode_msm7(rtcm, DefineConstants.SYS_CMP, sync);
            break;
        case 1240:
            ret = GlobalMembersRtcm3e.encode_ssr1(rtcm, DefineConstants.SYS_GAL, sync);
            break;
        case 1241:
            ret = GlobalMembersRtcm3e.encode_ssr2(rtcm, DefineConstants.SYS_GAL, sync);
            break;
        case 1242:
            ret = GlobalMembersRtcm3e.encode_ssr3(rtcm, DefineConstants.SYS_GAL, sync);
            break;
        case 1243:
            ret = GlobalMembersRtcm3e.encode_ssr4(rtcm, DefineConstants.SYS_GAL, sync);
            break;
        case 1244:
            ret = GlobalMembersRtcm3e.encode_ssr5(rtcm, DefineConstants.SYS_GAL, sync);
            break;
        case 1245:
            ret = GlobalMembersRtcm3e.encode_ssr6(rtcm, DefineConstants.SYS_GAL, sync);
            break;
        case 1246:
            ret = GlobalMembersRtcm3e.encode_ssr1(rtcm, DefineConstants.SYS_QZS, sync);
            break;
        case 1247:
            ret = GlobalMembersRtcm3e.encode_ssr2(rtcm, DefineConstants.SYS_QZS, sync);
            break;
        case 1248:
            ret = GlobalMembersRtcm3e.encode_ssr3(rtcm, DefineConstants.SYS_QZS, sync);
            break;
        case 1249:
            ret = GlobalMembersRtcm3e.encode_ssr4(rtcm, DefineConstants.SYS_QZS, sync);
            break;
        case 1250:
            ret = GlobalMembersRtcm3e.encode_ssr5(rtcm, DefineConstants.SYS_QZS, sync);
            break;
        case 1251:
            ret = GlobalMembersRtcm3e.encode_ssr6(rtcm, DefineConstants.SYS_QZS, sync);
            break;
        case 1252:
            ret = GlobalMembersRtcm3e.encode_ssr1(rtcm, DefineConstants.SYS_SBS, sync);
            break;
        case 1253:
            ret = GlobalMembersRtcm3e.encode_ssr2(rtcm, DefineConstants.SYS_SBS, sync);
            break;
        case 1254:
            ret = GlobalMembersRtcm3e.encode_ssr3(rtcm, DefineConstants.SYS_SBS, sync);
            break;
        case 1255:
            ret = GlobalMembersRtcm3e.encode_ssr4(rtcm, DefineConstants.SYS_SBS, sync);
            break;
        case 1256:
            ret = GlobalMembersRtcm3e.encode_ssr5(rtcm, DefineConstants.SYS_SBS, sync);
            break;
        case 1257:
            ret = GlobalMembersRtcm3e.encode_ssr6(rtcm, DefineConstants.SYS_SBS, sync);
            break;
        case 1258:
            ret = GlobalMembersRtcm3e.encode_ssr1(rtcm, DefineConstants.SYS_CMP, sync);
            break;
        case 1259:
            ret = GlobalMembersRtcm3e.encode_ssr2(rtcm, DefineConstants.SYS_CMP, sync);
            break;
        case 1260:
            ret = GlobalMembersRtcm3e.encode_ssr3(rtcm, DefineConstants.SYS_CMP, sync);
            break;
        case 1261:
            ret = GlobalMembersRtcm3e.encode_ssr4(rtcm, DefineConstants.SYS_CMP, sync);
            break;
        case 1262:
            ret = GlobalMembersRtcm3e.encode_ssr5(rtcm, DefineConstants.SYS_CMP, sync);
            break;
        case 1263:
            ret = GlobalMembersRtcm3e.encode_ssr6(rtcm, DefineConstants.SYS_CMP, sync);
            break;
    }
    if (ret > 0)
    {
        type -= 1000;
        if (1 <= type && type <= 299)
        {
            rtcm.nmsg3[type]++;
        }
        else
        {
            rtcm.nmsg3[0]++;
        }
    }
    return ret;
}
}
}
