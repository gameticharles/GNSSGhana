using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ghGPS.Classes
{
    public static class GlobalMembersRtcm3
    {
        /*------------------------------------------------------------------------------
        * rtcm3.c : rtcm ver.3 message decorder functions
        *
        *          Copyright (C) 2009-2015 by T.TAKASU, All rights reserved.
        *
        * options :
        *     -DSSR_QZSS_DRAFT_V05: qzss ssr messages based on ref [16]
        *
        * references :
        *     see rtcm.c
        *
        * version : $Revision:$ $Date:$
        * history : 2012/05/14 1.0  separated from rtcm.c
        *           2012/12/12 1.1  support gal/qzs ephemeris, gal/qzs ssr, msm
        *                           add station id consistency test for obs data
        *           2012/12/25 1.2  change compass msm id table
        *           2013/01/31 1.3  change signal id by the latest draft (ref [13])
        *           2013/02/23 1.4  change reference for rtcm 3 message (ref [14])
        *           2013/05/19 1.5  gpst -> bdt of time-tag in beidou msm message
        *           2014/05/02 1.6  fix bug on dropping last field of ssr message
        *                           comply with rtcm 3.2 with amendment 1/2 (ref[15])
        *                           delete MT 1046 according to ref [15]
        *           2014/09/14 1.7  add receiver option -RT_INP
        *           2014/12/06 1.8  support SBAS/BeiDou SSR messages (ref [16])
        *           2015/03/22 1.9  add handling of iodcrc for beidou/sbas ssr messages
        *-----------------------------------------------------------------------------*/


        internal const string rcsid = "$Id:$";

        /* msm signal id table -------------------------------------------------------*/
        public static string[] msm_sig_gps = { "", "1C", "1P", "1W", "1Y", "1M", "", "2C", "2P", "2W", "2Y", "2M", "", "", "2S", "2L", "2X", "", "", "", "", "5I", "5Q", "5X", "", "", "", "", "", "1S", "1L", "1X" };
        public static string[] msm_sig_glo = { "", "1C", "1P", "", "", "", "", "2C", "2P", "", "3I", "3Q", "3X", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };
        public static string[] msm_sig_gal = { "", "1C", "1A", "1B", "1X", "1Z", "", "6C", "6A", "6B", "6X", "6Z", "", "7I", "7Q", "7X", "", "8I", "8Q", "8X", "", "5I", "5Q", "5X", "", "", "", "", "", "", "", "" };
        public static string[] msm_sig_qzs = { "", "1C", "", "", "", "", "", "", "6S", "6L", "6X", "", "", "", "2S", "2L", "2X", "", "", "", "", "5I", "5Q", "5X", "", "", "", "", "", "1S", "1L", "1X" };
        public static string[] msm_sig_sbs = { "", "1C", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "5I", "5Q", "5X", "", "", "", "", "", "", "", "" };
        public static string[] msm_sig_cmp = { "", "1I", "1Q", "1X", "", "", "", "6I", "6Q", "6X", "", "", "", "7I", "7Q", "7X", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };
        /* ssr update intervals ------------------------------------------------------*/
        internal readonly double[] ssrudint = { 1, 2, 5, 10, 15, 30, 60, 120, 240, 300, 600, 900, 1800, 3600, 7200, 10800 };
        /* get sign-magnitude bits ---------------------------------------------------*/
        internal static double getbitg(byte buff, int pos, int len)
        {
            double value = GlobalMembersRtkcmn.getbitu(buff, pos + 1, len - 1);
            return GlobalMembersRtkcmn.getbitu(buff, pos, 1) != 0 ? -value : value;
        }
        /* adjust weekly rollover of gps time ----------------------------------------*/
        internal static void adjweek(rtcm_t rtcm, double tow)
        {
            double tow_p;
            int week;

            /* if no time, get cpu time */
            if (rtcm.time.time == 0)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                //ORIGINAL LINE: rtcm->time=utc2gpst(timeget());
                rtcm.time.CopyFrom(GlobalMembersRtkcmn.utc2gpst(GlobalMembersRtkcmn.timeget()));
            }
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: tow_p=time2gpst(rtcm->time,&week);
            tow_p = GlobalMembersRtkcmn.time2gpst(new gtime_t(rtcm.time), ref week);
            if (tow < tow_p - 302400.0)
            {
                tow += 604800.0;
            }
            else if (tow > tow_p + 302400.0)
            {
                tow -= 604800.0;
            }
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: rtcm->time=gpst2time(week,tow);
            rtcm.time.CopyFrom(GlobalMembersRtkcmn.gpst2time(week, tow));
        }
        /* adjust weekly rollover of bdt time ----------------------------------------*/
        internal static int adjbdtweek(int week)
        {
            int w;
            ()GlobalMembersRtkcmn.time2bdt(GlobalMembersRtkcmn.gpst2bdt(GlobalMembersRtkcmn.utc2gpst(GlobalMembersRtkcmn.timeget())), ref w);
            if (w < 1) // use 2006/1/1 if time is earlier than 2006/1/1
            {
                w = 1;
            }
            return week + (w - week + 512) / 1024 * 1024;
        }
        /* adjust daily rollover of glonass time -------------------------------------*/
        internal static void adjday_glot(rtcm_t rtcm, double tod)
        {
            gtime_t time = new gtime_t();
            double tow;
            double tod_p;
            int week;

            if (rtcm.time.time == 0)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                //ORIGINAL LINE: rtcm->time=utc2gpst(timeget());
                rtcm.time.CopyFrom(GlobalMembersRtkcmn.utc2gpst(GlobalMembersRtkcmn.timeget()));
            }
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: time=timeadd(gpst2utc(rtcm->time),10800.0);
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            time.CopyFrom(GlobalMembersRtkcmn.timeadd(GlobalMembersRtkcmn.gpst2utc(new gtime_t(rtcm.time)), 10800.0)); // glonass time
                                                                                                                       //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                                                                                                                       //ORIGINAL LINE: tow=time2gpst(time,&week);
            tow = GlobalMembersRtkcmn.time2gpst(new gtime_t(time), ref week);
            tod_p = Math.IEEERemainder(tow, 86400.0);
            tow -= tod_p;
            if (tod < tod_p - 43200.0)
            {
                tod += 86400.0;
            }
            else if (tod > tod_p + 43200.0)
            {
                tod -= 86400.0;
            }
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: time=gpst2time(week,tow+tod);
            time.CopyFrom(GlobalMembersRtkcmn.gpst2time(week, tow + tod));
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: rtcm->time=utc2gpst(timeadd(time,-10800.0));
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            rtcm.time.CopyFrom(GlobalMembersRtkcmn.utc2gpst(GlobalMembersRtkcmn.timeadd(new gtime_t(time), -10800.0)));
        }
        /* adjust carrier-phase rollover ---------------------------------------------*/
        internal static double adjcp(rtcm_t rtcm, int sat, int freq, double cp)
        {
            if (rtcm.cp[sat - 1, freq] == 0.0)
            {
                ;
            }
            else if (cp < rtcm.cp[sat - 1, freq] - 750.0)
            {
                cp += 1500.0;
            }
            else if (cp > rtcm.cp[sat - 1, freq] + 750.0)
            {
                cp -= 1500.0;
            }
            rtcm.cp[sat - 1, freq] = cp;
            return cp;
        }
        /* loss-of-lock indicator ----------------------------------------------------*/
        internal static int lossoflock(rtcm_t rtcm, int sat, int freq, int @lock)
        {
            int lli = (@lock == 0 && rtcm.@lock[sat - 1, freq] == 0) || @lock < rtcm.@lock[sat - 1, freq];
            rtcm.@lock[sat - 1, freq] = @lock;
            return lli;
        }
        /* s/n ratio -----------------------------------------------------------------*/
        internal static byte snratio(double snr)
        {
            return (byte)(snr <= 0.0 || 255.5 <= snr != 0 ? 0.0 : snr * 4.0 + 0.5);
        }
        /* get observation data index ------------------------------------------------*/
        internal static int obsindex(obs_t obs, gtime_t time, int sat)
        {
            int i;
            int j;

            for (i = 0; i < obs.n; i++)
            {
                if (obs.data[i].sat == sat) // field already exists
                {
                    return i;
                }
            }
            if (i >= DefineConstants.MAXOBS) // overflow
            {
                return -1;
            }

            /* add new field */
            obs.data[i].time = time;
            obs.data[i].sat = sat;
            for (j = 0; j < DefineConstants.NFREQ + DefineConstants.NEXOBS; j++)
            {
                obs.data[i].L[j] = obs.data[i].P[j] = 0.0;
                obs.data[i].D[j] = 0.0;
                obs.data[i].SNR[j] = obs.data[i].LLI[j] = obs.data[i].code[j] = 0;
            }
            obs.n++;
            return i;
        }
        /* test station id consistency -----------------------------------------------*/
        internal static int test_staid(rtcm_t rtcm, int staid)
        {
            string p;
            int type;
            int id;

            /* test station id option */
            if ((p = StringFunctions.StrStr(rtcm.opt, "-STA=")) != 0 && sscanf(p, "-STA=%d", id) == 1)
            {
                if (staid != id)
                {
                    return 0;
                }
            }
            /* save station id */
            if (rtcm.staid == 0 || rtcm.obsflag != 0)
            {
                rtcm.staid = staid;
            }
            else if (staid != rtcm.staid)
            {
                type = GlobalMembersRtkcmn.getbitu(rtcm.buff, 24, 12);
                GlobalMembersRtkcmn.trace(2, "rtcm3 %d staid invalid id=%d %d\n", type, staid, rtcm.staid);

                /* reset station id if station id error */
                rtcm.staid = 0;
                return 0;
            }
            return 1;
        }
        /* decode type 1001-1004 message header --------------------------------------*/
        internal static int decode_head1001(rtcm_t rtcm, ref int sync)
        {
            double tow;
            string msg;
            int i = 24;
            int staid;
            int nsat;
            int type;

            type = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 12);
            i += 12;

            if (i + 52 <= rtcm.len * 8)
            {
                staid = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 12);
                i += 12;
                tow = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 30) * 0.001;
                i += 30;
                sync = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 1);
                i += 1;
                nsat = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 5);
            }
            else
            {
                GlobalMembersRtkcmn.trace(2, "rtcm3 %d length error: len=%d\n", type, rtcm.len);
                return -1;
            }
            /* test station id */
            if (GlobalMembersRtcm3.test_staid(rtcm, staid) == 0)
            {
                return -1;
            }

            GlobalMembersBinex.adjweek(rtcm, tow);

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: trace(4,"decode_head1001: time=%s nsat=%d sync=%d\n",time_str(rtcm->time,2), nsat,*sync);
            GlobalMembersRtkcmn.trace(4, "decode_head1001: time=%s nsat=%d sync=%d\n", GlobalMembersRtkcmn.time_str(new gtime_t(rtcm.time), 2), nsat, sync);

            if (rtcm.outtype != 0)
            {
                msg = rtcm.msgtype.Substring(rtcm.msgtype.Length);
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: sprintf(msg," %s nsat=%2d sync=%d",time_str(rtcm->time,2),nsat,*sync);
                msg = string.Format(" {0} nsat={1,2:D} sync={2:D}", GlobalMembersRtkcmn.time_str(new gtime_t(rtcm.time), 2), nsat, sync);
            }
            return nsat;
        }
        /* decode type 1001: L1-only gps rtk observation -----------------------------*/
        internal static int decode_type1001(rtcm_t rtcm)
        {
            int sync;
            if (GlobalMembersRtcm3.decode_head1001(rtcm, ref sync) < 0)
            {
                return -1;
            }
            rtcm.obsflag = sync == 0;
            return sync != 0 ? 0 : 1;
        }
        /* decode type 1002: extended L1-only gps rtk observables --------------------*/
        internal static int decode_type1002(rtcm_t rtcm)
        {
            double pr1;
            double cnr1;
            double tt;
            double cp1;
            int i = 24 + 64;
            int j;
            int index;
            int nsat;
            int sync;
            int prn;
            int code;
            int sat;
            int ppr1;
            int lock1;
            int amb;
            int sys;

            if ((nsat = GlobalMembersRtcm3.decode_head1001(rtcm, ref sync)) < 0)
            {
                return -1;
            }

            for (j = 0; j < nsat && rtcm.obs.n < DefineConstants.MAXOBS && i + 74 <= rtcm.len * 8; j++)
            {
                prn = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 6);
                i += 6;
                code = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 1);
                i += 1;
                pr1 = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 24);
                i += 24;
                ppr1 = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 20);
                i += 20;
                lock1 = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 7);
                i += 7;
                amb = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 8);
                i += 8;
                cnr1 = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 8);
                i += 8;
                if (prn < 40)
                {
                    sys = DefineConstants.SYS_GPS;
                }
                else
                {
                    sys = DefineConstants.SYS_SBS;
                    prn += 80;
                }
                if ((sat = GlobalMembersRtkcmn.satno(sys, prn)) == 0)
                {
                    GlobalMembersRtkcmn.trace(2, "rtcm3 1002 satellite number error: prn=%d\n", prn);
                    continue;
                }
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: tt=timediff(rtcm->obs.data[0].time,rtcm->time);
                tt = GlobalMembersRtkcmn.timediff(rtcm.obs.data[0].time, new gtime_t(rtcm.time));
                if (rtcm.obsflag != 0 || Math.Abs(tt) > 1E-9)
                {
                    rtcm.obs.n = rtcm.obsflag = 0;
                }
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: if ((index=obsindex(&rtcm->obs,rtcm->time,sat))<0)
                if ((index = GlobalMembersNovatel.obsindex(rtcm.obs, new gtime_t(rtcm.time), sat)) < 0)
                    continue;
                pr1 = pr1 * 0.02 + amb * DefineConstants.PRUNIT_GPS;
                if (ppr1 != (int)0xFFF80000)
                {
                    rtcm.obs.data[index].P[0] = pr1;
                    cp1 = GlobalMembersRtcm3.adjcp(rtcm, sat, 0, ppr1 * 0.0005 / GlobalMembersRtkcmn.lam_carr[0]);
                    rtcm.obs.data[index].L[0] = pr1 / GlobalMembersRtkcmn.lam_carr[0] + cp1;
                }
                rtcm.obs.data[index].LLI[0] = GlobalMembersRtcm3.lossoflock(rtcm, sat, 0, lock1);
                rtcm.obs.data[index].SNR[0] = GlobalMembersRtcm3.snratio(cnr1 * 0.25);
                rtcm.obs.data[index].code[0] = code != 0 ? DefineConstants.CODE_L1P : DefineConstants.CODE_L1C;
            }
            return sync != 0 ? 0 : 1;
        }
        /* decode type 1003: L1&L2 gps rtk observables -------------------------------*/
        internal static int decode_type1003(rtcm_t rtcm)
        {
            int sync;
            if (GlobalMembersRtcm3.decode_head1001(rtcm, ref sync) < 0)
            {
                return -1;
            }
            rtcm.obsflag = sync == 0;
            return sync != 0 ? 0 : 1;
        }
        /* decode type 1004: extended L1&L2 gps rtk observables ----------------------*/
        internal static int decode_type1004(rtcm_t rtcm)
        {
            int[] L2codes = { DefineConstants.CODE_L2C, DefineConstants.CODE_L2P, DefineConstants.CODE_L2W, DefineConstants.CODE_L2W };
            double pr1;
            double cnr1;
            double cnr2;
            double tt;
            double cp1;
            double cp2;
            int i = 24 + 64;
            int j;
            int index;
            int nsat;
            int sync;
            int prn;
            int sat;
            int code1;
            int code2;
            int pr21;
            int ppr1;
            int ppr2;
            int lock1;
            int lock2;
            int amb;
            int sys;

            if ((nsat = GlobalMembersRtcm3.decode_head1001(rtcm, ref sync)) < 0)
            {
                return -1;
            }

            for (j = 0; j < nsat && rtcm.obs.n < DefineConstants.MAXOBS && i + 125 <= rtcm.len * 8; j++)
            {
                prn = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 6);
                i += 6;
                code1 = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 1);
                i += 1;
                pr1 = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 24);
                i += 24;
                ppr1 = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 20);
                i += 20;
                lock1 = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 7);
                i += 7;
                amb = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 8);
                i += 8;
                cnr1 = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 8);
                i += 8;
                code2 = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 2);
                i += 2;
                pr21 = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 14);
                i += 14;
                ppr2 = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 20);
                i += 20;
                lock2 = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 7);
                i += 7;
                cnr2 = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 8);
                i += 8;
                if (prn < 40)
                {
                    sys = DefineConstants.SYS_GPS;
                }
                else
                {
                    sys = DefineConstants.SYS_SBS;
                    prn += 80;
                }
                if ((sat = GlobalMembersRtkcmn.satno(sys, prn)) == 0)
                {
                    GlobalMembersRtkcmn.trace(2, "rtcm3 1004 satellite number error: sys=%d prn=%d\n", sys, prn);
                    continue;
                }
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: tt=timediff(rtcm->obs.data[0].time,rtcm->time);
                tt = GlobalMembersRtkcmn.timediff(rtcm.obs.data[0].time, new gtime_t(rtcm.time));
                if (rtcm.obsflag != 0 || Math.Abs(tt) > 1E-9)
                {
                    rtcm.obs.n = rtcm.obsflag = 0;
                }
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: if ((index=obsindex(&rtcm->obs,rtcm->time,sat))<0)
                if ((index = GlobalMembersNovatel.obsindex(rtcm.obs, new gtime_t(rtcm.time), sat)) < 0)
                    continue;
                pr1 = pr1 * 0.02 + amb * DefineConstants.PRUNIT_GPS;
                if (ppr1 != (int)0xFFF80000)
                {
                    rtcm.obs.data[index].P[0] = pr1;
                    cp1 = GlobalMembersRtcm3.adjcp(rtcm, sat, 0, ppr1 * 0.0005 / GlobalMembersRtkcmn.lam_carr[0]);
                    rtcm.obs.data[index].L[0] = pr1 / GlobalMembersRtkcmn.lam_carr[0] + cp1;
                }
                rtcm.obs.data[index].LLI[0] = GlobalMembersRtcm3.lossoflock(rtcm, sat, 0, lock1);
                rtcm.obs.data[index].SNR[0] = GlobalMembersRtcm3.snratio(cnr1 * 0.25);
                rtcm.obs.data[index].code[0] = code1 != 0 ? DefineConstants.CODE_L1P : DefineConstants.CODE_L1C;

                if (pr21 != (int)0xFFFFE000)
                {
                    rtcm.obs.data[index].P[1] = pr1 + pr21 * 0.02;
                }
                if (ppr2 != (int)0xFFF80000)
                {
                    cp2 = GlobalMembersRtcm3.adjcp(rtcm, sat, 1, ppr2 * 0.0005 / GlobalMembersRtkcmn.lam_carr[1]);
                    rtcm.obs.data[index].L[1] = pr1 / GlobalMembersRtkcmn.lam_carr[1] + cp2;
                }
                rtcm.obs.data[index].LLI[1] = GlobalMembersRtcm3.lossoflock(rtcm, sat, 1, lock2);
                rtcm.obs.data[index].SNR[1] = GlobalMembersRtcm3.snratio(cnr2 * 0.25);
                rtcm.obs.data[index].code[1] = L2codes[code2];
            }
            rtcm.obsflag = sync == 0;
            return sync != 0 ? 0 : 1;
        }
        /* get signed 38bit field ----------------------------------------------------*/
        internal static double getbits_38(byte buff, int pos)
        {
            return (double)GlobalMembersRtkcmn.getbits(buff, pos, 32) * 64.0 + GlobalMembersRtkcmn.getbitu(buff, pos + 32, 6);
        }
        /* decode type 1005: stationary rtk reference station arp --------------------*/
        internal static int decode_type1005(rtcm_t rtcm)
        {
            double[] rr = new double[3];
            string msg;
            int i = 24 + 12;
            int j;
            int staid;
            int itrf;

            if (i + 140 == rtcm.len * 8)
            {
                staid = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 12);
                i += 12;
                itrf = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 6);
                i += 6 + 4;
                rr[0] = GlobalMembersRtcm3.getbits_38(rtcm.buff, i);
                i += 38 + 2;
                rr[1] = GlobalMembersRtcm3.getbits_38(rtcm.buff, i);
                i += 38 + 2;
                rr[2] = GlobalMembersRtcm3.getbits_38(rtcm.buff, i);
            }
            else
            {
                GlobalMembersRtkcmn.trace(2, "rtcm3 1005 length error: len=%d\n", rtcm.len);
                return -1;
            }
            if (rtcm.outtype != 0)
            {
                msg = rtcm.msgtype.Substring(rtcm.msgtype.Length);
                msg = string.Format(" staid={0,4:D}", staid);
            }
            /* test station id */
            if (GlobalMembersRtcm3.test_staid(rtcm, staid) == 0)
            {
                return -1;
            }

            rtcm.sta.deltype = 0; // xyz
            for (j = 0; j < 3; j++)
            {
                rtcm.sta.pos[j] = rr[j] * 0.0001;
                rtcm.sta.del[j] = 0.0;
            }
            rtcm.sta.hgt = 0.0;
            rtcm.sta.itrf = itrf;
            return 5;
        }
        /* decode type 1006: stationary rtk reference station arp with height --------*/
        internal static int decode_type1006(rtcm_t rtcm)
        {
            double[] rr = new double[3];
            double anth;
            string msg;
            int i = 24 + 12;
            int j;
            int staid;
            int itrf;

            if (i + 156 <= rtcm.len * 8)
            {
                staid = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 12);
                i += 12;
                itrf = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 6);
                i += 6 + 4;
                rr[0] = GlobalMembersRtcm3.getbits_38(rtcm.buff, i);
                i += 38 + 2;
                rr[1] = GlobalMembersRtcm3.getbits_38(rtcm.buff, i);
                i += 38 + 2;
                rr[2] = GlobalMembersRtcm3.getbits_38(rtcm.buff, i);
                i += 38;
                anth = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 16);
            }
            else
            {
                GlobalMembersRtkcmn.trace(2, "rtcm3 1006 length error: len=%d\n", rtcm.len);
                return -1;
            }
            if (rtcm.outtype != 0)
            {
                msg = rtcm.msgtype.Substring(rtcm.msgtype.Length);
                msg = string.Format(" staid={0,4:D}", staid);
            }
            /* test station id */
            if (GlobalMembersRtcm3.test_staid(rtcm, staid) == 0)
            {
                return -1;
            }

            rtcm.sta.deltype = 1; // xyz
            for (j = 0; j < 3; j++)
            {
                rtcm.sta.pos[j] = rr[j] * 0.0001;
                rtcm.sta.del[j] = 0.0;
            }
            rtcm.sta.hgt = anth * 0.0001;
            rtcm.sta.itrf = itrf;
            return 5;
        }
        /* decode type 1007: antenna descriptor --------------------------------------*/
        internal static int decode_type1007(rtcm_t rtcm)
        {
            string des = "";
            string msg;
            int i = 24 + 12;
            int j;
            int staid;
            int n;
            int setup;

            n = GlobalMembersRtkcmn.getbitu(rtcm.buff, i + 12, 8);

            if (i + 28 + 8 * n <= rtcm.len * 8)
            {
                staid = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 12);
                i += 12 + 8;
                for (j = 0; j < n && j < 31; j++)
                {
                    des[j] = (sbyte)GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 8);
                    i += 8;
                }
                setup = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 8);
            }
            else
            {
                GlobalMembersRtkcmn.trace(2, "rtcm3 1007 length error: len=%d\n", rtcm.len);
                return -1;
            }
            if (rtcm.outtype != 0)
            {
                msg = rtcm.msgtype.Substring(rtcm.msgtype.Length);
                msg = string.Format(" staid={0,4:D}", staid);
            }
            /* test station id */
            if (GlobalMembersRtcm3.test_staid(rtcm, staid) == 0)
            {
                return -1;
            }

            rtcm.sta.antdes = des.Substring(0, n);
            rtcm.sta.antdes[n] = '\0';
            rtcm.sta.antsetup = setup;
            rtcm.sta.antsno[0] = '\0';
            return 5;
        }
        /* decode type 1008: antenna descriptor & serial number ----------------------*/
        internal static int decode_type1008(rtcm_t rtcm)
        {
            string des = "";
            string sno = "";
            string msg;
            int i = 24 + 12;
            int j;
            int staid;
            int n;
            int m;
            int setup;

            n = GlobalMembersRtkcmn.getbitu(rtcm.buff, i + 12, 8);
            m = GlobalMembersRtkcmn.getbitu(rtcm.buff, i + 28 + 8 * n, 8);

            if (i + 36 + 8 * (n + m) <= rtcm.len * 8)
            {
                staid = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 12);
                i += 12 + 8;
                for (j = 0; j < n && j < 31; j++)
                {
                    des[j] = (sbyte)GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 8);
                    i += 8;
                }
                setup = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 8);
                i += 8 + 8;
                for (j = 0; j < m && j < 31; j++)
                {
                    sno[j] = (sbyte)GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 8);
                    i += 8;
                }
            }
            else
            {
                GlobalMembersRtkcmn.trace(2, "rtcm3 1008 length error: len=%d\n", rtcm.len);
                return -1;
            }
            if (rtcm.outtype != 0)
            {
                msg = rtcm.msgtype.Substring(rtcm.msgtype.Length);
                msg = string.Format(" staid={0,4:D}", staid);
            }
            /* test station id */
            if (GlobalMembersRtcm3.test_staid(rtcm, staid) == 0)
            {
                return -1;
            }

            rtcm.sta.antdes = des.Substring(0, n);
            rtcm.sta.antdes[n] = '\0';
            rtcm.sta.antsetup = setup;
            rtcm.sta.antsno = sno.Substring(0, m);
            rtcm.sta.antsno[m] = '\0';
            return 5;
        }
        /* decode type 1009-1012 message header --------------------------------------*/
        internal static int decode_head1009(rtcm_t rtcm, ref int sync)
        {
            double tod;
            string msg;
            int i = 24;
            int staid;
            int nsat;
            int type;

            type = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 12);
            i += 12;

            if (i + 49 <= rtcm.len * 8)
            {
                staid = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 12);
                i += 12;
                tod = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 27) * 0.001; // sec in a day
                i += 27;
                sync = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 1);
                i += 1;
                nsat = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 5);
            }
            else
            {
                GlobalMembersRtkcmn.trace(2, "rtcm3 %d length error: len=%d\n", type, rtcm.len);
                return -1;
            }
            /* test station id */
            if (GlobalMembersRtcm3.test_staid(rtcm, staid) == 0)
            {
                return -1;
            }

            GlobalMembersRtcm3.adjday_glot(rtcm, tod);

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: trace(4,"decode_head1009: time=%s nsat=%d sync=%d\n",time_str(rtcm->time,2), nsat,*sync);
            GlobalMembersRtkcmn.trace(4, "decode_head1009: time=%s nsat=%d sync=%d\n", GlobalMembersRtkcmn.time_str(new gtime_t(rtcm.time), 2), nsat, sync);

            if (rtcm.outtype != 0)
            {
                msg = rtcm.msgtype.Substring(rtcm.msgtype.Length);
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: sprintf(msg," %s nsat=%2d sync=%d",time_str(rtcm->time,2),nsat,*sync);
                msg = string.Format(" {0} nsat={1,2:D} sync={2:D}", GlobalMembersRtkcmn.time_str(new gtime_t(rtcm.time), 2), nsat, sync);
            }
            return nsat;
        }
        /* decode type 1009: L1-only glonass rtk observables -------------------------*/
        internal static int decode_type1009(rtcm_t rtcm)
        {
            int sync;
            if (GlobalMembersRtcm3.decode_head1009(rtcm, ref sync) < 0)
            {
                return -1;
            }
            rtcm.obsflag = sync == 0;
            return sync != 0 ? 0 : 1;
        }
        /* decode type 1010: extended L1-only glonass rtk observables ----------------*/
        internal static int decode_type1010(rtcm_t rtcm)
        {
            double pr1;
            double cnr1;
            double tt;
            double cp1;
            double lam1;
            int i = 24 + 61;
            int j;
            int index;
            int nsat;
            int sync;
            int prn;
            int sat;
            int code;
            int freq;
            int ppr1;
            int lock1;
            int amb;
            int sys = DefineConstants.SYS_GLO;

            if ((nsat = GlobalMembersRtcm3.decode_head1009(rtcm, ref sync)) < 0)
            {
                return -1;
            }

            for (j = 0; j < nsat && rtcm.obs.n < DefineConstants.MAXOBS && i + 79 <= rtcm.len * 8; j++)
            {
                prn = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 6);
                i += 6;
                code = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 1);
                i += 1;
                freq = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 5);
                i += 5;
                pr1 = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 25);
                i += 25;
                ppr1 = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 20);
                i += 20;
                lock1 = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 7);
                i += 7;
                amb = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 7);
                i += 7;
                cnr1 = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 8);
                i += 8;
                if ((sat = GlobalMembersRtkcmn.satno(sys, prn)) == 0)
                {
                    GlobalMembersRtkcmn.trace(2, "rtcm3 1010 satellite number error: prn=%d\n", prn);
                    continue;
                }
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: tt=timediff(rtcm->obs.data[0].time,rtcm->time);
                tt = GlobalMembersRtkcmn.timediff(rtcm.obs.data[0].time, new gtime_t(rtcm.time));
                if (rtcm.obsflag != 0 || Math.Abs(tt) > 1E-9)
                {
                    rtcm.obs.n = rtcm.obsflag = 0;
                }
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: if ((index=obsindex(&rtcm->obs,rtcm->time,sat))<0)
                if ((index = GlobalMembersNovatel.obsindex(rtcm.obs, new gtime_t(rtcm.time), sat)) < 0)
                    continue;
                pr1 = pr1 * 0.02 + amb * DefineConstants.PRUNIT_GLO;
                if (ppr1 != (int)0xFFF80000)
                {
                    rtcm.obs.data[index].P[0] = pr1;
                    lam1 = DefineConstants.CLIGHT / (DefineConstants.FREQ1_GLO + DefineConstants.DFRQ1_GLO * (freq - 7));
                    cp1 = GlobalMembersRtcm3.adjcp(rtcm, sat, 0, ppr1 * 0.0005 / lam1);
                    rtcm.obs.data[index].L[0] = pr1 / lam1 + cp1;
                }
                rtcm.obs.data[index].LLI[0] = GlobalMembersRtcm3.lossoflock(rtcm, sat, 0, lock1);
                rtcm.obs.data[index].SNR[0] = GlobalMembersRtcm3.snratio(cnr1 * 0.25);
                rtcm.obs.data[index].code[0] = code != 0 ? DefineConstants.CODE_L1P : DefineConstants.CODE_L1C;
            }
            return sync != 0 ? 0 : 1;
        }
        /* decode type 1011: L1&L2 glonass rtk observables ---------------------------*/
        internal static int decode_type1011(rtcm_t rtcm)
        {
            int sync;
            if (GlobalMembersRtcm3.decode_head1009(rtcm, ref sync) < 0)
            {
                return -1;
            }
            rtcm.obsflag = sync == 0;
            return sync != 0 ? 0 : 1;
        }
        /* decode type 1012: extended L1&L2 glonass rtk observables ------------------*/
        internal static int decode_type1012(rtcm_t rtcm)
        {
            double pr1;
            double cnr1;
            double cnr2;
            double tt;
            double cp1;
            double cp2;
            double lam1;
            double lam2;
            int i = 24 + 61;
            int j;
            int index;
            int nsat;
            int sync;
            int prn;
            int sat;
            int freq;
            int code1;
            int code2;
            int pr21;
            int ppr1;
            int ppr2;
            int lock1;
            int lock2;
            int amb;
            int sys = DefineConstants.SYS_GLO;

            if ((nsat = GlobalMembersRtcm3.decode_head1009(rtcm, ref sync)) < 0)
            {
                return -1;
            }

            for (j = 0; j < nsat && rtcm.obs.n < DefineConstants.MAXOBS && i + 130 <= rtcm.len * 8; j++)
            {
                prn = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 6);
                i += 6;
                code1 = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 1);
                i += 1;
                freq = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 5);
                i += 5;
                pr1 = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 25);
                i += 25;
                ppr1 = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 20);
                i += 20;
                lock1 = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 7);
                i += 7;
                amb = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 7);
                i += 7;
                cnr1 = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 8);
                i += 8;
                code2 = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 2);
                i += 2;
                pr21 = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 14);
                i += 14;
                ppr2 = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 20);
                i += 20;
                lock2 = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 7);
                i += 7;
                cnr2 = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 8);
                i += 8;
                if ((sat = GlobalMembersRtkcmn.satno(sys, prn)) == 0)
                {
                    GlobalMembersRtkcmn.trace(2, "rtcm3 1012 satellite number error: sys=%d prn=%d\n", sys, prn);
                    continue;
                }
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: tt=timediff(rtcm->obs.data[0].time,rtcm->time);
                tt = GlobalMembersRtkcmn.timediff(rtcm.obs.data[0].time, new gtime_t(rtcm.time));
                if (rtcm.obsflag != 0 || Math.Abs(tt) > 1E-9)
                {
                    rtcm.obs.n = rtcm.obsflag = 0;
                }
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: if ((index=obsindex(&rtcm->obs,rtcm->time,sat))<0)
                if ((index = GlobalMembersNovatel.obsindex(rtcm.obs, new gtime_t(rtcm.time), sat)) < 0)
                    continue;
                pr1 = pr1 * 0.02 + amb * DefineConstants.PRUNIT_GLO;
                if (ppr1 != (int)0xFFF80000)
                {
                    lam1 = DefineConstants.CLIGHT / (DefineConstants.FREQ1_GLO + DefineConstants.DFRQ1_GLO * (freq - 7));
                    rtcm.obs.data[index].P[0] = pr1;
                    cp1 = GlobalMembersRtcm3.adjcp(rtcm, sat, 0, ppr1 * 0.0005 / lam1);
                    rtcm.obs.data[index].L[0] = pr1 / lam1 + cp1;
                }
                rtcm.obs.data[index].LLI[0] = GlobalMembersRtcm3.lossoflock(rtcm, sat, 0, lock1);
                rtcm.obs.data[index].SNR[0] = GlobalMembersRtcm3.snratio(cnr1 * 0.25);
                rtcm.obs.data[index].code[0] = code1 != 0 ? DefineConstants.CODE_L1P : DefineConstants.CODE_L1C;

                if (pr21 != (int)0xFFFFE000)
                {
                    rtcm.obs.data[index].P[1] = pr1 + pr21 * 0.02;
                }
                if (ppr2 != (int)0xFFF80000)
                {
                    lam2 = DefineConstants.CLIGHT / (DefineConstants.FREQ2_GLO + DefineConstants.DFRQ2_GLO * (freq - 7));
                    cp2 = GlobalMembersRtcm3.adjcp(rtcm, sat, 1, ppr2 * 0.0005 / lam2);
                    rtcm.obs.data[index].L[1] = pr1 / lam2 + cp2;
                }
                rtcm.obs.data[index].LLI[1] = GlobalMembersRtcm3.lossoflock(rtcm, sat, 1, lock2);
                rtcm.obs.data[index].SNR[1] = GlobalMembersRtcm3.snratio(cnr2 * 0.25);
                rtcm.obs.data[index].code[1] = code2 != 0 ? DefineConstants.CODE_L2P : DefineConstants.CODE_L2C;
            }
            rtcm.obsflag = sync == 0;
            return sync != 0 ? 0 : 1;
        }
        /* decode type 1013: system parameters ---------------------------------------*/
        internal static int decode_type1013(rtcm_t rtcm)
        {
            return 0;
        }
        /* decode type 1019: gps ephemerides -----------------------------------------*/
        internal static int decode_type1019(rtcm_t rtcm)
        {
            eph_t eph = new eph_t();
            double toc;
            double sqrtA;
            string msg;
            int i = 24 + 12;
            int prn;
            int sat;
            int week;
            int sys = DefineConstants.SYS_GPS;

            if (i + 476 <= rtcm.len * 8)
            {
                prn = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 6);
                i += 6;
                week = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 10);
                i += 10;
                eph.sva = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 4);
                i += 4;
                eph.code = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 2);
                i += 2;
                eph.idot = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 14) * DefineConstants.P2_43 * DefineConstants.SC2RAD;
                i += 14;
                eph.iode = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 8);
                i += 8;
                toc = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 16) * 16.0;
                i += 16;
                eph.f2 = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 8) * DefineConstants.P2_55;
                i += 8;
                eph.f1 = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 16) * DefineConstants.P2_43;
                i += 16;
                eph.f0 = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 22) * DefineConstants.P2_31;
                i += 22;
                eph.iodc = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 10);
                i += 10;
                eph.crs = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 16) * DefineConstants.P2_5;
                i += 16;
                eph.deln = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 16) * DefineConstants.P2_43 * DefineConstants.SC2RAD;
                i += 16;
                eph.M0 = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 32) * DefineConstants.P2_31 * DefineConstants.SC2RAD;
                i += 32;
                eph.cuc = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 16) * 1.862645149230957E-0x9;
                i += 16;
                eph.e = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 32) * DefineConstants.P2_33;
                i += 32;
                eph.cus = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 16) * 1.862645149230957E-0x9;
                i += 16;
                sqrtA = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 32) * 1.907348632812500E-0x6;
                i += 32;
                eph.toes = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 16) * 16.0;
                i += 16;
                eph.cic = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 16) * 1.862645149230957E-0x9;
                i += 16;
                eph.OMG0 = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 32) * DefineConstants.P2_31 * DefineConstants.SC2RAD;
                i += 32;
                eph.cis = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 16) * 1.862645149230957E-0x9;
                i += 16;
                eph.i0 = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 32) * DefineConstants.P2_31 * DefineConstants.SC2RAD;
                i += 32;
                eph.crc = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 16) * DefineConstants.P2_5;
                i += 16;
                eph.omg = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 32) * DefineConstants.P2_31 * DefineConstants.SC2RAD;
                i += 32;
                eph.OMGd = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 24) * DefineConstants.P2_43 * DefineConstants.SC2RAD;
                i += 24;
                eph.tgd[0] = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 8) * DefineConstants.P2_31;
                i += 8;
                eph.svh = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 6);
                i += 6;
                eph.flag = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 1);
                i += 1;
                eph.fit = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 1) != 0 ? 0.0 : 4.0; // 0:4hr,1:>4hr
            }
            else
            {
                GlobalMembersRtkcmn.trace(2, "rtcm3 1019 length error: len=%d\n", rtcm.len);
                return -1;
            }
            if (prn >= 40)
            {
                sys = DefineConstants.SYS_SBS;
                prn += 80;
            }
            GlobalMembersRtkcmn.trace(4, "decode_type1019: prn=%d iode=%d toe=%.0f\n", prn, eph.iode, eph.toes);

            if (rtcm.outtype != 0)
            {
                msg = rtcm.msgtype.Substring(rtcm.msgtype.Length);
                msg = string.Format(" prn={0,2:D} iode={1,3:D} iodc={2,3:D} week={3:D} toe={4,6:f0} toc={5,6:f0} svh={6:X2}", prn, eph.iode, eph.iodc, week, eph.toes, toc, eph.svh);
            }
            if ((sat = GlobalMembersRtkcmn.satno(sys, prn)) == 0)
            {
                GlobalMembersRtkcmn.trace(2, "rtcm3 1019 satellite number error: prn=%d\n", prn);
                return -1;
            }
            eph.sat = sat;
            eph.week = GlobalMembersRtkcmn.adjgpsweek(week);
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: eph.toe=gpst2time(eph.week,eph.toes);
            eph.toe.CopyFrom(GlobalMembersRtkcmn.gpst2time(eph.week, eph.toes));
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: eph.toc=gpst2time(eph.week,toc);
            eph.toc.CopyFrom(GlobalMembersRtkcmn.gpst2time(eph.week, toc));
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: eph.ttr=rtcm->time;
            eph.ttr.CopyFrom(rtcm.time);
            eph.A = sqrtA * sqrtA;
            if (!StringFunctions.StrStr(rtcm.opt, "-EPHALL"))
            {
                if (eph.iode == rtcm.nav.eph[sat - 1].iode) // unchanged
                {
                    return 0;
                }
            }
            rtcm.nav.eph[sat - 1] = eph;
            rtcm.ephsat = sat;
            return 2;
        }
        /* decode type 1020: glonass ephemerides -------------------------------------*/
        internal static int decode_type1020(rtcm_t rtcm)
        {
            geph_t geph = new geph_t();
            double tk_h;
            double tk_m;
            double tk_s;
            double toe;
            double tow;
            double tod;
            double tof;
            string msg;
            int i = 24 + 12;
            int prn;
            int sat;
            int week;
            int tb;
            int bn;
            int sys = DefineConstants.SYS_GLO;

            if (i + 348 <= rtcm.len * 8)
            {
                prn = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 6);
                i += 6;
                geph.frq = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 5) - 7;
                i += 5 + 2 + 2;
                tk_h = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 5);
                i += 5;
                tk_m = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 6);
                i += 6;
                tk_s = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 1) * 30.0;
                i += 1;
                bn = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 1);
                i += 1 + 1;
                tb = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 7);
                i += 7;
                geph.vel[0] = GlobalMembersRcvraw.getbitg(rtcm.buff, i, 24) * 9.536743164062500E-0x7 * 1E3;
                i += 24;
                geph.pos[0] = GlobalMembersRcvraw.getbitg(rtcm.buff, i, 27) * 4.882812500000000E-0x4 * 1E3;
                i += 27;
                geph.acc[0] = GlobalMembersRcvraw.getbitg(rtcm.buff, i, 5) * DefineConstants.P2_30 * 1E3;
                i += 5;
                geph.vel[1] = GlobalMembersRcvraw.getbitg(rtcm.buff, i, 24) * 9.536743164062500E-0x7 * 1E3;
                i += 24;
                geph.pos[1] = GlobalMembersRcvraw.getbitg(rtcm.buff, i, 27) * 4.882812500000000E-0x4 * 1E3;
                i += 27;
                geph.acc[1] = GlobalMembersRcvraw.getbitg(rtcm.buff, i, 5) * DefineConstants.P2_30 * 1E3;
                i += 5;
                geph.vel[2] = GlobalMembersRcvraw.getbitg(rtcm.buff, i, 24) * 9.536743164062500E-0x7 * 1E3;
                i += 24;
                geph.pos[2] = GlobalMembersRcvraw.getbitg(rtcm.buff, i, 27) * 4.882812500000000E-0x4 * 1E3;
                i += 27;
                geph.acc[2] = GlobalMembersRcvraw.getbitg(rtcm.buff, i, 5) * DefineConstants.P2_30 * 1E3;
                i += 5 + 1;
                geph.gamn = GlobalMembersRcvraw.getbitg(rtcm.buff, i, 11) * DefineConstants.P2_40;
                i += 11 + 3;
                geph.taun = GlobalMembersRcvraw.getbitg(rtcm.buff, i, 22) * DefineConstants.P2_30;
            }
            else
            {
                GlobalMembersRtkcmn.trace(2, "rtcm3 1020 length error: len=%d\n", rtcm.len);
                return -1;
            }
            if ((sat = GlobalMembersRtkcmn.satno(sys, prn)) == 0)
            {
                GlobalMembersRtkcmn.trace(2, "rtcm3 1020 satellite number error: prn=%d\n", prn);
                return -1;
            }
            GlobalMembersRtkcmn.trace(4, "decode_type1020: prn=%d tk=%02.0f:%02.0f:%02.0f\n", prn, tk_h, tk_m, tk_s);

            if (rtcm.outtype != 0)
            {
                msg = rtcm.msgtype.Substring(rtcm.msgtype.Length);
                //C++ TO C# CONVERTER TODO TASK: Zero padding is not converted when combined with a minimum width specifier:
                //ORIGINAL LINE: sprintf(msg," prn=%2d tk=%02.0f:%02.0f:%02.0f frq=%2d bn=%d tb=%d", prn,tk_h,tk_m,tk_s,geph.frq,bn,tb);
                msg = string.Format(" prn={0,2:D} tk={1,2:f0}:{2,2:f0}:{3,2:f0} frq={4,2:D} bn={5:D} tb={6:D}", prn, tk_h, tk_m, tk_s, geph.frq, bn, tb);
            }
            geph.sat = sat;
            geph.svh = bn;
            geph.iode = tb & 0x7F;
            if (rtcm.time.time == 0)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                //ORIGINAL LINE: rtcm->time=utc2gpst(timeget());
                rtcm.time.CopyFrom(GlobalMembersRtkcmn.utc2gpst(GlobalMembersRtkcmn.timeget()));
            }
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: tow=time2gpst(gpst2utc(rtcm->time),&week);
            tow = GlobalMembersRtkcmn.time2gpst(GlobalMembersRtkcmn.gpst2utc(new gtime_t(rtcm.time)), ref week);
            tod = Math.IEEERemainder(tow, 86400.0);
            tow -= tod;
            tof = tk_h * 3600.0 + tk_m * 60.0 + tk_s - 10800.0; // lt->utc
            if (tof < tod - 43200.0)
            {
                tof += 86400.0;
            }
            else if (tof > tod + 43200.0)
            {
                tof -= 86400.0;
            }
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: geph.tof=utc2gpst(gpst2time(week,tow+tof));
            geph.tof.CopyFrom(GlobalMembersRtkcmn.utc2gpst(GlobalMembersRtkcmn.gpst2time(week, tow + tof)));
            toe = tb * 900.0 - 10800.0; // lt->utc
            if (toe < tod - 43200.0)
            {
                toe += 86400.0;
            }
            else if (toe > tod + 43200.0)
            {
                toe -= 86400.0;
            }
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: geph.toe=utc2gpst(gpst2time(week,tow+toe));
            geph.toe.CopyFrom(GlobalMembersRtkcmn.utc2gpst(GlobalMembersRtkcmn.gpst2time(week, tow + toe))); // utc->gpst

            if (!StringFunctions.StrStr(rtcm.opt, "-EPHALL"))
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: if (fabs(timediff(geph.toe,rtcm->nav.geph[prn-1].toe))<1.0&& geph.svh==rtcm->nav.geph[prn-1].svh)
                if (Math.Abs(GlobalMembersRtkcmn.timediff(new gtime_t(geph.toe), rtcm.nav.geph[prn - 1].toe)) < 1.0 && geph.svh == rtcm.nav.geph[prn - 1].svh) // unchanged
                {
                    return 0;
                }
            }
            rtcm.nav.geph[prn - 1] = geph;
            rtcm.ephsat = sat;
            return 2;
        }
        /* decode type 1021: helmert/abridged molodenski -----------------------------*/
        internal static int decode_type1021(rtcm_t rtcm)
        {
            GlobalMembersRtkcmn.trace(2, "rtcm3 1021: not supported message\n");
            return 0;
        }
        /* decode type 1022: moledenski-badekas transfromation -----------------------*/
        internal static int decode_type1022(rtcm_t rtcm)
        {
            GlobalMembersRtkcmn.trace(2, "rtcm3 1022: not supported message\n");
            return 0;
        }
        /* decode type 1023: residual, ellipoidal grid representation ----------------*/
        internal static int decode_type1023(rtcm_t rtcm)
        {
            GlobalMembersRtkcmn.trace(2, "rtcm3 1023: not supported message\n");
            return 0;
        }
        /* decode type 1024: residual, plane grid representation ---------------------*/
        internal static int decode_type1024(rtcm_t rtcm)
        {
            GlobalMembersRtkcmn.trace(2, "rtcm3 1024: not supported message\n");
            return 0;
        }
        /* decode type 1025: projection (types except LCC2SP,OM) ---------------------*/
        internal static int decode_type1025(rtcm_t rtcm)
        {
            GlobalMembersRtkcmn.trace(2, "rtcm3 1025: not supported message\n");
            return 0;
        }
        /* decode type 1026: projection (LCC2SP - lambert conic conformal (2sp)) -----*/
        internal static int decode_type1026(rtcm_t rtcm)
        {
            GlobalMembersRtkcmn.trace(2, "rtcm3 1026: not supported message\n");
            return 0;
        }
        /* decode type 1027: projection (type OM - oblique mercator) -----------------*/
        internal static int decode_type1027(rtcm_t rtcm)
        {
            GlobalMembersRtkcmn.trace(2, "rtcm3 1027: not supported message\n");
            return 0;
        }
        /* decode type 1030: network rtk residual ------------------------------------*/
        internal static int decode_type1030(rtcm_t rtcm)
        {
            GlobalMembersRtkcmn.trace(2, "rtcm3 1030: not supported message\n");
            return 0;
        }
        /* decode type 1031: glonass network rtk residual ----------------------------*/
        internal static int decode_type1031(rtcm_t rtcm)
        {
            GlobalMembersRtkcmn.trace(2, "rtcm3 1031: not supported message\n");
            return 0;
        }
        /* decode type 1032: physical reference station position information ---------*/
        internal static int decode_type1032(rtcm_t rtcm)
        {
            GlobalMembersRtkcmn.trace(2, "rtcm3 1032: not supported message\n");
            return 0;
        }
        /* decode type 1033: receiver and antenna descriptor -------------------------*/
        internal static int decode_type1033(rtcm_t rtcm)
        {
            string des = "";
            string sno = "";
            string rec = "";
            string ver = "";
            string rsn = "";
            string msg;
            int i = 24 + 12;
            int j;
            int staid;
            int n;
            int m;
            int n1;
            int n2;
            int n3;
            int setup;

            n = GlobalMembersRtkcmn.getbitu(rtcm.buff, i + 12, 8);
            m = GlobalMembersRtkcmn.getbitu(rtcm.buff, i + 28 + 8 * n, 8);
            n1 = GlobalMembersRtkcmn.getbitu(rtcm.buff, i + 36 + 8 * (n + m), 8);
            n2 = GlobalMembersRtkcmn.getbitu(rtcm.buff, i + 44 + 8 * (n + m + n1), 8);
            n3 = GlobalMembersRtkcmn.getbitu(rtcm.buff, i + 52 + 8 * (n + m + n1 + n2), 8);

            if (i + 60 + 8 * (n + m + n1 + n2 + n3) <= rtcm.len * 8)
            {
                staid = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 12);
                i += 12 + 8;
                for (j = 0; j < n && j < 31; j++)
                {
                    des[j] = (sbyte)GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 8);
                    i += 8;
                }
                setup = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 8);
                i += 8 + 8;
                for (j = 0; j < m && j < 31; j++)
                {
                    sno[j] = (sbyte)GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 8);
                    i += 8;
                }
                i += 8;
                for (j = 0; j < n1 && j < 31; j++)
                {
                    rec[j] = (sbyte)GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 8);
                    i += 8;
                }
                i += 8;
                for (j = 0; j < n2 && j < 31; j++)
                {
                    ver[j] = (sbyte)GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 8);
                    i += 8;
                }
                i += 8;
                for (j = 0; j < n3 && j < 31; j++)
                {
                    rsn[j] = (sbyte)GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 8);
                    i += 8;
                }
            }
            else
            {
                GlobalMembersRtkcmn.trace(2, "rtcm3 1033 length error: len=%d\n", rtcm.len);
                return -1;
            }
            if (rtcm.outtype != 0)
            {
                msg = rtcm.msgtype.Substring(rtcm.msgtype.Length);
                msg = string.Format(" staid={0,4:D}", staid);
            }
            /* test station id */
            if (GlobalMembersRtcm3.test_staid(rtcm, staid) == 0)
            {
                return -1;
            }

            rtcm.sta.antdes = des.Substring(0, n);
            rtcm.sta.antdes[n] = '\0';
            rtcm.sta.antsetup = setup;
            rtcm.sta.antsno = sno.Substring(0, m);
            rtcm.sta.antsno[m] = '\0';
            rtcm.sta.rectype = rec.Substring(0, n1);
            rtcm.sta.rectype[n1] = '\0';
            rtcm.sta.recver = ver.Substring(0, n2);
            rtcm.sta.recver[n2] = '\0';
            rtcm.sta.recsno = rsn.Substring(0, n3);
            rtcm.sta.recsno[n3] = '\0';

            GlobalMembersRtkcmn.trace(3, "rtcm3 1033: ant=%s:%s rec=%s:%s:%s\n", des, sno, rec, ver, rsn);
            return 5;
        }
        /* decode type 1034: gps network fkp gradient --------------------------------*/
        internal static int decode_type1034(rtcm_t rtcm)
        {
            GlobalMembersRtkcmn.trace(2, "rtcm3 1034: not supported message\n");
            return 0;
        }
        /* decode type 1035: glonass network fkp gradient ----------------------------*/
        internal static int decode_type1035(rtcm_t rtcm)
        {
            GlobalMembersRtkcmn.trace(2, "rtcm3 1035: not supported message\n");
            return 0;
        }
        /* decode type 1037: glonass network rtk ionospheric correction difference ---*/
        internal static int decode_type1037(rtcm_t rtcm)
        {
            GlobalMembersRtkcmn.trace(2, "rtcm3 1037: not supported message\n");
            return 0;
        }
        /* decode type 1038: glonass network rtk geometic correction difference ------*/
        internal static int decode_type1038(rtcm_t rtcm)
        {
            GlobalMembersRtkcmn.trace(2, "rtcm3 1038: not supported message\n");
            return 0;
        }
        /* decode type 1039: glonass network rtk combined correction difference ------*/
        internal static int decode_type1039(rtcm_t rtcm)
        {
            GlobalMembersRtkcmn.trace(2, "rtcm3 1039: not supported message\n");
            return 0;
        }
        /* decode type 1044: qzss ephemerides (ref [15]) -----------------------------*/
        internal static int decode_type1044(rtcm_t rtcm)
        {
            eph_t eph = new eph_t();
            double toc;
            double sqrtA;
            string msg;
            int i = 24 + 12;
            int prn;
            int sat;
            int week;
            int sys = DefineConstants.SYS_QZS;

            if (i + 473 <= rtcm.len * 8)
            {
                prn = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 4) + 192;
                i += 4;
                toc = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 16) * 16.0;
                i += 16;
                eph.f2 = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 8) * DefineConstants.P2_55;
                i += 8;
                eph.f1 = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 16) * DefineConstants.P2_43;
                i += 16;
                eph.f0 = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 22) * DefineConstants.P2_31;
                i += 22;
                eph.iode = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 8);
                i += 8;
                eph.crs = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 16) * DefineConstants.P2_5;
                i += 16;
                eph.deln = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 16) * DefineConstants.P2_43 * DefineConstants.SC2RAD;
                i += 16;
                eph.M0 = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 32) * DefineConstants.P2_31 * DefineConstants.SC2RAD;
                i += 32;
                eph.cuc = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 16) * 1.862645149230957E-0x9;
                i += 16;
                eph.e = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 32) * DefineConstants.P2_33;
                i += 32;
                eph.cus = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 16) * 1.862645149230957E-0x9;
                i += 16;
                sqrtA = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 32) * 1.907348632812500E-0x6;
                i += 32;
                eph.toes = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 16) * 16.0;
                i += 16;
                eph.cic = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 16) * 1.862645149230957E-0x9;
                i += 16;
                eph.OMG0 = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 32) * DefineConstants.P2_31 * DefineConstants.SC2RAD;
                i += 32;
                eph.cis = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 16) * 1.862645149230957E-0x9;
                i += 16;
                eph.i0 = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 32) * DefineConstants.P2_31 * DefineConstants.SC2RAD;
                i += 32;
                eph.crc = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 16) * DefineConstants.P2_5;
                i += 16;
                eph.omg = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 32) * DefineConstants.P2_31 * DefineConstants.SC2RAD;
                i += 32;
                eph.OMGd = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 24) * DefineConstants.P2_43 * DefineConstants.SC2RAD;
                i += 24;
                eph.idot = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 14) * DefineConstants.P2_43 * DefineConstants.SC2RAD;
                i += 14;
                eph.code = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 2);
                i += 2;
                week = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 10);
                i += 10;
                eph.sva = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 4);
                i += 4;
                eph.svh = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 6);
                i += 6;
                eph.tgd[0] = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 8) * DefineConstants.P2_31;
                i += 8;
                eph.iodc = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 10);
                i += 10;
                eph.fit = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 1) != 0 ? 0.0 : 2.0; // 0:2hr,1:>2hr
            }
            else
            {
                GlobalMembersRtkcmn.trace(2, "rtcm3 1044 length error: len=%d\n", rtcm.len);
                return -1;
            }
            GlobalMembersRtkcmn.trace(4, "decode_type1044: prn=%d iode=%d toe=%.0f\n", prn, eph.iode, eph.toes);

            if (rtcm.outtype != 0)
            {
                msg = rtcm.msgtype.Substring(rtcm.msgtype.Length);
                msg = string.Format(" prn={0,3:D} iode={1,3:D} iodc={2,3:D} week={3:D} toe={4,6:f0} toc={5,6:f0} svh={6:X2}", prn, eph.iode, eph.iodc, week, eph.toes, toc, eph.svh);
            }
            if ((sat = GlobalMembersRtkcmn.satno(sys, prn)) == 0)
            {
                GlobalMembersRtkcmn.trace(2, "rtcm3 1044 satellite number error: prn=%d\n", prn);
                return -1;
            }
            eph.sat = sat;
            eph.week = GlobalMembersRtkcmn.adjgpsweek(week);
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: eph.toe=gpst2time(eph.week,eph.toes);
            eph.toe.CopyFrom(GlobalMembersRtkcmn.gpst2time(eph.week, eph.toes));
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: eph.toc=gpst2time(eph.week,toc);
            eph.toc.CopyFrom(GlobalMembersRtkcmn.gpst2time(eph.week, toc));
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: eph.ttr=rtcm->time;
            eph.ttr.CopyFrom(rtcm.time);
            eph.A = sqrtA * sqrtA;
            if (!StringFunctions.StrStr(rtcm.opt, "-EPHALL"))
            {
                if (eph.iode == rtcm.nav.eph[sat - 1].iode && eph.iodc == rtcm.nav.eph[sat - 1].iodc) // unchanged
                {
                    return 0;
                }
            }
            rtcm.nav.eph[sat - 1] = eph;
            rtcm.ephsat = sat;
            return 2;
        }
        /* decode type 1045: galileo satellite ephemerides (ref [15]) ----------------*/
        internal static int decode_type1045(rtcm_t rtcm)
        {
            eph_t eph = new eph_t();
            double toc;
            double sqrtA;
            string msg;
            int i = 24 + 12;
            int prn;
            int sat;
            int week;
            int e5a_hs;
            int e5a_dvs;
            int rsv;
            int sys = DefineConstants.SYS_GAL;

            if (i + 484 <= rtcm.len * 8)
            {
                prn = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 6);
                i += 6;
                week = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 12);
                i += 12;
                eph.iode = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 10);
                i += 10;
                eph.sva = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 8);
                i += 8;
                eph.idot = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 14) * DefineConstants.P2_43 * DefineConstants.SC2RAD;
                i += 14;
                toc = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 14) * 60.0;
                i += 14;
                eph.f2 = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 6) * DefineConstants.P2_59;
                i += 6;
                eph.f1 = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 21) * DefineConstants.P2_46;
                i += 21;
                eph.f0 = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 31) * DefineConstants.P2_34;
                i += 31;
                eph.crs = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 16) * DefineConstants.P2_5;
                i += 16;
                eph.deln = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 16) * DefineConstants.P2_43 * DefineConstants.SC2RAD;
                i += 16;
                eph.M0 = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 32) * DefineConstants.P2_31 * DefineConstants.SC2RAD;
                i += 32;
                eph.cuc = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 16) * 1.862645149230957E-0x9;
                i += 16;
                eph.e = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 32) * DefineConstants.P2_33;
                i += 32;
                eph.cus = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 16) * 1.862645149230957E-0x9;
                i += 16;
                sqrtA = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 32) * 1.907348632812500E-0x6;
                i += 32;
                eph.toes = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 14) * 60.0;
                i += 14;
                eph.cic = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 16) * 1.862645149230957E-0x9;
                i += 16;
                eph.OMG0 = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 32) * DefineConstants.P2_31 * DefineConstants.SC2RAD;
                i += 32;
                eph.cis = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 16) * 1.862645149230957E-0x9;
                i += 16;
                eph.i0 = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 32) * DefineConstants.P2_31 * DefineConstants.SC2RAD;
                i += 32;
                eph.crc = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 16) * DefineConstants.P2_5;
                i += 16;
                eph.omg = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 32) * DefineConstants.P2_31 * DefineConstants.SC2RAD;
                i += 32;
                eph.OMGd = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 24) * DefineConstants.P2_43 * DefineConstants.SC2RAD;
                i += 24;
                eph.tgd[0] = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 10) * DefineConstants.P2_32; // E5a/E1
                i += 10;
                e5a_hs = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 2); // OSHS
                i += 2;
                e5a_dvs = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 1); // OSDVS
                i += 1;
                rsv = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 7);
            }
            else
            {
                GlobalMembersRtkcmn.trace(2, "rtcm3 1045 length error: len=%d\n", rtcm.len);
                return -1;
            }
            GlobalMembersRtkcmn.trace(4, "decode_type1045: prn=%d iode=%d toe=%.0f\n", prn, eph.iode, eph.toes);

            if (rtcm.outtype != 0)
            {
                msg = rtcm.msgtype.Substring(rtcm.msgtype.Length);
                msg = string.Format(" prn={0,2:D} iode={1,3:D} week={2:D} toe={3,6:f0} toc={4,6:f0} hs={5:D} dvs={6:D}", prn, eph.iode, week, eph.toes, toc, e5a_hs, e5a_dvs);
            }
            if ((sat = GlobalMembersRtkcmn.satno(sys, prn)) == 0)
            {
                GlobalMembersRtkcmn.trace(2, "rtcm3 1045 satellite number error: prn=%d\n", prn);
                return -1;
            }
            eph.sat = sat;
            eph.week = GlobalMembersRtkcmn.adjgpsweek(week % 1024);
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: eph.toe=gpst2time(eph.week,eph.toes);
            eph.toe.CopyFrom(GlobalMembersRtkcmn.gpst2time(eph.week, eph.toes));
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: eph.toc=gpst2time(eph.week,toc);
            eph.toc.CopyFrom(GlobalMembersRtkcmn.gpst2time(eph.week, toc));
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: eph.ttr=rtcm->time;
            eph.ttr.CopyFrom(rtcm.time);
            eph.A = sqrtA * sqrtA;
            eph.svh = (e5a_hs << 4) + (e5a_dvs << 3);
            eph.code = 2; // data source = f/nav e5a
            if (!StringFunctions.StrStr(rtcm.opt, "-EPHALL"))
            {
                if (eph.iode == rtcm.nav.eph[sat - 1].iode) // unchanged
                {
                    return 0;
                }
            }
            rtcm.nav.eph[sat - 1] = eph;
            rtcm.ephsat = sat;
            return 2;
        }
        /* decode type 1047: beidou ephemerides (tentative mt and format) ------------*/
        internal static int decode_type1047(rtcm_t rtcm)
        {
            eph_t eph = new eph_t();
            double toc;
            double sqrtA;
            string msg;
            int i = 24 + 12;
            int prn;
            int sat;
            int week;
            int sys = DefineConstants.SYS_CMP;

            if (i + 476 <= rtcm.len * 8)
            {
                prn = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 6);
                i += 6;
                week = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 10);
                i += 10;
                eph.sva = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 4);
                i += 4;
                eph.code = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 2);
                i += 2;
                eph.idot = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 14) * DefineConstants.P2_43 * DefineConstants.SC2RAD;
                i += 14;
                eph.iode = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 8);
                i += 8;
                toc = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 16) * 16.0;
                i += 16;
                eph.f2 = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 8) * DefineConstants.P2_55;
                i += 8;
                eph.f1 = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 16) * DefineConstants.P2_43;
                i += 16;
                eph.f0 = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 22) * DefineConstants.P2_31;
                i += 22;
                eph.iodc = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 10);
                i += 10;
                eph.crs = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 16) * DefineConstants.P2_5;
                i += 16;
                eph.deln = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 16) * DefineConstants.P2_43 * DefineConstants.SC2RAD;
                i += 16;
                eph.M0 = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 32) * DefineConstants.P2_31 * DefineConstants.SC2RAD;
                i += 32;
                eph.cuc = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 16) * 1.862645149230957E-0x9;
                i += 16;
                eph.e = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 32) * DefineConstants.P2_33;
                i += 32;
                eph.cus = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 16) * 1.862645149230957E-0x9;
                i += 16;
                sqrtA = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 32) * 1.907348632812500E-0x6;
                i += 32;
                eph.toes = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 16) * 16.0;
                i += 16;
                eph.cic = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 16) * 1.862645149230957E-0x9;
                i += 16;
                eph.OMG0 = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 32) * DefineConstants.P2_31 * DefineConstants.SC2RAD;
                i += 32;
                eph.cis = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 16) * 1.862645149230957E-0x9;
                i += 16;
                eph.i0 = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 32) * DefineConstants.P2_31 * DefineConstants.SC2RAD;
                i += 32;
                eph.crc = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 16) * DefineConstants.P2_5;
                i += 16;
                eph.omg = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 32) * DefineConstants.P2_31 * DefineConstants.SC2RAD;
                i += 32;
                eph.OMGd = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 24) * DefineConstants.P2_43 * DefineConstants.SC2RAD;
                i += 24;
                eph.tgd[0] = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 8) * DefineConstants.P2_31;
                i += 8;
                eph.svh = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 6);
                i += 6;
                eph.flag = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 1);
                i += 1;
                eph.fit = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 1) != 0 ? 0.0 : 4.0; // 0:4hr,1:>4hr
            }
            else
            {
                GlobalMembersRtkcmn.trace(2, "rtcm3 1047 length error: len=%d\n", rtcm.len);
                return -1;
            }
            GlobalMembersRtkcmn.trace(4, "decode_type1047: prn=%d iode=%d toe=%.0f\n", prn, eph.iode, eph.toes);

            if (rtcm.outtype != 0)
            {
                msg = rtcm.msgtype.Substring(rtcm.msgtype.Length);
                msg = string.Format(" prn={0,2:D} iode={1,3:D} iodc={2,3:D} week={3:D} toe={4,6:f0} toc={5,6:f0} svh={6:X2}", prn, eph.iode, eph.iodc, week, eph.toes, toc, eph.svh);
            }
            if ((sat = GlobalMembersRtkcmn.satno(sys, prn)) == 0)
            {
                GlobalMembersRtkcmn.trace(2, "rtcm3 1047 satellite number error: prn=%d\n", prn);
                return -1;
            }
            eph.sat = sat;
            eph.week = GlobalMembersRtcm3.adjbdtweek(week);
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: eph.toe=bdt2gpst(bdt2time(eph.week,eph.toes));
            eph.toe.CopyFrom(GlobalMembersRtkcmn.bdt2gpst(GlobalMembersRtkcmn.bdt2time(eph.week, eph.toes))); // bdt -> gpst
                                                                                                              //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                                                                                                              //ORIGINAL LINE: eph.toc=bdt2gpst(bdt2time(eph.week,toc));
            eph.toc.CopyFrom(GlobalMembersRtkcmn.bdt2gpst(GlobalMembersRtkcmn.bdt2time(eph.week, toc))); // bdt -> gpst
                                                                                                         //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                                                                                                         //ORIGINAL LINE: eph.ttr=rtcm->time;
            eph.ttr.CopyFrom(rtcm.time);
            eph.A = sqrtA * sqrtA;
            if (!StringFunctions.StrStr(rtcm.opt, "-EPHALL"))
            {
                if (eph.iode == rtcm.nav.eph[sat - 1].iode) // unchanged
                {
                    return 0;
                }
            }
            rtcm.nav.eph[sat - 1] = eph;
            rtcm.ephsat = sat;
            return 2;
        }
        /* decode ssr 1,4 message header ---------------------------------------------*/
        internal static int decode_ssr1_head(rtcm_t rtcm, int sys, ref int sync, ref int iod, ref double udint, ref int refd, ref int hsize)
        {
            double tod;
            double tow;
            string msg;
            int i = 24 + 12;
            int nsat;
            int udi;
            int provid = 0;
            int solid = 0;
            int ns = 6;

#if !SSR_QZSS_DRAFT_V05
            ns = sys == DefineConstants.SYS_QZS ? 4 : 6;
#endif
            if (i + (sys == DefineConstants.SYS_GLO ? 53 : 50 + ns) > rtcm.len * 8)
            {
                return -1;
            }

            if (sys == DefineConstants.SYS_GLO)
            {
                tod = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 17);
                i += 17;
                GlobalMembersRtcm3.adjday_glot(rtcm, tod);
            }
            else
            {
                tow = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 20);
                i += 20;
                GlobalMembersBinex.adjweek(rtcm, tow);
            }
            udi = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 4);
            i += 4;
            sync = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 1);
            i += 1;
            refd = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 1); // satellite ref datum
            i += 1;
            iod = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 4); // iod
            i += 4;
            provid = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 16); // provider id
            i += 16;
            solid = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 4); // solution id
            i += 4;
            nsat = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, ns);
            i += ns;
            udint = ssrudint[udi];

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: trace(4,"decode_ssr1_head: time=%s sys=%d nsat=%d sync=%d iod=%d provid=%d solid=%d\n", time_str(rtcm->time,2),sys,nsat,*sync,*iod,provid,solid);
            GlobalMembersRtkcmn.trace(4, "decode_ssr1_head: time=%s sys=%d nsat=%d sync=%d iod=%d provid=%d solid=%d\n", GlobalMembersRtkcmn.time_str(new gtime_t(rtcm.time), 2), sys, nsat, sync, iod, provid, solid);

            if (rtcm.outtype != 0)
            {
                msg = rtcm.msgtype.Substring(rtcm.msgtype.Length);
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: sprintf(msg," %s nsat=%2d iod=%2d udi=%2d sync=%d", time_str(rtcm->time,2),nsat,*iod,udi,*sync);
                msg = string.Format(" {0} nsat={1,2:D} iod={2,2:D} udi={3,2:D} sync={4:D}", GlobalMembersRtkcmn.time_str(new gtime_t(rtcm.time), 2), nsat, iod, udi, sync);
            }
            hsize = i;
            return nsat;
        }
        /* decode ssr 2,3,5,6 message header -----------------------------------------*/
        internal static int decode_ssr2_head(rtcm_t rtcm, int sys, ref int sync, ref int iod, ref double udint, ref int hsize)
        {
            double tod;
            double tow;
            string msg;
            int i = 24 + 12;
            int nsat;
            int udi;
            int provid = 0;
            int solid = 0;
            int ns = 6;

#if !SSR_QZSS_DRAFT_V05
            ns = sys == DefineConstants.SYS_QZS ? 4 : 6;
#endif
            if (i + (sys == DefineConstants.SYS_GLO ? 52 : 49 + ns) > rtcm.len * 8)
            {
                return -1;
            }

            if (sys == DefineConstants.SYS_GLO)
            {
                tod = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 17);
                i += 17;
                GlobalMembersRtcm3.adjday_glot(rtcm, tod);
            }
            else
            {
                tow = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 20);
                i += 20;
                GlobalMembersBinex.adjweek(rtcm, tow);
            }
            udi = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 4);
            i += 4;
            sync = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 1);
            i += 1;
            iod = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 4);
            i += 4;
            provid = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 16); // provider id
            i += 16;
            solid = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 4); // solution id
            i += 4;
            nsat = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, ns);
            i += ns;
            udint = ssrudint[udi];

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: trace(4,"decode_ssr2_head: time=%s sys=%d nsat=%d sync=%d iod=%d provid=%d solid=%d\n", time_str(rtcm->time,2),sys,nsat,*sync,*iod,provid,solid);
            GlobalMembersRtkcmn.trace(4, "decode_ssr2_head: time=%s sys=%d nsat=%d sync=%d iod=%d provid=%d solid=%d\n", GlobalMembersRtkcmn.time_str(new gtime_t(rtcm.time), 2), sys, nsat, sync, iod, provid, solid);

            if (rtcm.outtype != 0)
            {
                msg = rtcm.msgtype.Substring(rtcm.msgtype.Length);
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: sprintf(msg," %s nsat=%2d iod=%2d udi=%2d sync=%d", time_str(rtcm->time,2),nsat,*iod,udi,*sync);
                msg = string.Format(" {0} nsat={1,2:D} iod={2,2:D} udi={3,2:D} sync={4:D}", GlobalMembersRtkcmn.time_str(new gtime_t(rtcm.time), 2), nsat, iod, udi, sync);
            }
            hsize = i;
            return nsat;
        }
        /* decode ssr 1: orbit corrections -------------------------------------------*/
        internal static int decode_ssr1(rtcm_t rtcm, int sys)
        {
            double udint;
            double[] deph = new double[3];
            double[] ddeph = new double[3];
            int i;
            int j;
            int k;
            int type;
            int sync;
            int iod;
            int nsat;
            int prn;
            int sat;
            int iode;
            int iodcrc;
            int refd = 0;
            int np;
            int ni;
            int nj;
            int offp;

            type = GlobalMembersRtkcmn.getbitu(rtcm.buff, 24, 12);

            if ((nsat = GlobalMembersRtcm3.decode_ssr1_head(rtcm, sys, ref sync, ref iod, ref udint, ref refd, ref i)) < 0)
            {
                GlobalMembersRtkcmn.trace(2, "rtcm3 %d length error: len=%d\n", type, rtcm.len);
                return -1;
            }
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
                    return sync != 0 ? 0 : 10;
            }
            for (j = 0; j < nsat && i + 121 + np + ni + nj <= rtcm.len * 8; j++)
            {
                prn = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, np) + offp;
                i += np;
                iode = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, ni);
                i += ni;
                iodcrc = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, nj);
                i += nj;
                deph[0] = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 22) * 1E-4;
                i += 22;
                deph[1] = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 20) * 4E-4;
                i += 20;
                deph[2] = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 20) * 4E-4;
                i += 20;
                ddeph[0] = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 21) * 1E-6;
                i += 21;
                ddeph[1] = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 19) * 4E-6;
                i += 19;
                ddeph[2] = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 19) * 4E-6;
                i += 19;

                if ((sat = GlobalMembersRtkcmn.satno(sys, prn)) == 0)
                {
                    GlobalMembersRtkcmn.trace(2, "rtcm3 %d satellite number error: prn=%d\n", type, prn);
                    continue;
                }
                rtcm.ssr[sat - 1].t0[0] = rtcm.time;
                rtcm.ssr[sat - 1].udi[0] = udint;
                rtcm.ssr[sat - 1].iod[0] = iod;
                rtcm.ssr[sat - 1].iode = iode; // sbas/bds: toe/t0 modulo
                rtcm.ssr[sat - 1].iodcrc = iodcrc; // sbas/bds: iod crc
                rtcm.ssr[sat - 1].refd = refd;

                for (k = 0; k < 3; k++)
                {
                    rtcm.ssr[sat - 1].deph[k] = deph[k];
                    rtcm.ssr[sat - 1].ddeph[k] = ddeph[k];
                }
                rtcm.ssr[sat - 1].update = 1;
            }
            return sync != 0 ? 0 : 10;
        }
        /* decode ssr 2: clock corrections -------------------------------------------*/
        internal static int decode_ssr2(rtcm_t rtcm, int sys)
        {
            double udint;
            double[] dclk = new double[3];
            int i;
            int j;
            int k;
            int type;
            int sync;
            int iod;
            int nsat;
            int prn;
            int sat;
            int np;
            int offp;

            type = GlobalMembersRtkcmn.getbitu(rtcm.buff, 24, 12);

            if ((nsat = GlobalMembersRtcm3.decode_ssr2_head(rtcm, sys, ref sync, ref iod, ref udint, ref i)) < 0)
            {
                GlobalMembersRtkcmn.trace(2, "rtcm3 %d length error: len=%d\n", type, rtcm.len);
                return -1;
            }
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
                    return sync != 0 ? 0 : 10;
            }
            for (j = 0; j < nsat && i + 70 + np <= rtcm.len * 8; j++)
            {
                prn = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, np) + offp;
                i += np;
                dclk[0] = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 22) * 1E-4;
                i += 22;
                dclk[1] = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 21) * 1E-6;
                i += 21;
                dclk[2] = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 27) * 2E-8;
                i += 27;

                if ((sat = GlobalMembersRtkcmn.satno(sys, prn)) == 0)
                {
                    GlobalMembersRtkcmn.trace(2, "rtcm3 %d satellite number error: prn=%d\n", type, prn);
                    continue;
                }
                rtcm.ssr[sat - 1].t0[1] = rtcm.time;
                rtcm.ssr[sat - 1].udi[1] = udint;
                rtcm.ssr[sat - 1].iod[1] = iod;

                for (k = 0; k < 3; k++)
                {
                    rtcm.ssr[sat - 1].dclk[k] = dclk[k];
                }
                rtcm.ssr[sat - 1].update = 1;
            }
            return sync != 0 ? 0 : 10;
        }
        /* decode ssr 3: satellite code biases ---------------------------------------*/
        internal static int decode_ssr3(rtcm_t rtcm, int sys)
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
            double udint;
            double bias;
            double[] cbias = new double[DefineConstants.MAXCODE];
            int i;
            int j;
            int k;
            int type;
            int mode;
            int sync;
            int iod;
            int nsat;
            int prn;
            int sat;
            int nbias;
            int np;
            int offp;
            int ncode;

            type = GlobalMembersRtkcmn.getbitu(rtcm.buff, 24, 12);

            if ((nsat = GlobalMembersRtcm3.decode_ssr2_head(rtcm, sys, ref sync, ref iod, ref udint, ref i)) < 0)
            {
                GlobalMembersRtkcmn.trace(2, "rtcm3 %d length error: len=%d\n", type, rtcm.len);
                return -1;
            }
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
                    return sync != 0 ? 0 : 10;
            }
            for (j = 0; j < nsat && i + 5 + np <= rtcm.len * 8; j++)
            {
                prn = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, np) + offp;
                i += np;
                nbias = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 5);
                i += 5;

                for (k = 0; k < DefineConstants.MAXCODE; k++)
                {
                    cbias[k] = 0.0;
                }
                for (k = 0; k < nbias && i + 19 < rtcm.len * 8; k++)
                {
                    mode = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 5);
                    i += 5;
                    bias = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 14) * 0.01;
                    i += 14;
                    if (mode <= ncode)
                    {
                        cbias[codes[mode] - 1] = (float)bias;
                    }
                    else
                    {
                        GlobalMembersRtkcmn.trace(2, "rtcm3 %d not supported mode: mode=%d\n", type, mode);
                    }
                }
                if ((sat = GlobalMembersRtkcmn.satno(sys, prn)) == 0)
                {
                    GlobalMembersRtkcmn.trace(2, "rtcm3 %d satellite number error: prn=%d\n", type, prn);
                    continue;
                }
                rtcm.ssr[sat - 1].t0[4] = rtcm.time;
                rtcm.ssr[sat - 1].udi[4] = udint;
                rtcm.ssr[sat - 1].iod[4] = iod;

                for (k = 0; k < DefineConstants.MAXCODE; k++)
                {
                    rtcm.ssr[sat - 1].cbias[k] = (float)cbias[k];
                }
                rtcm.ssr[sat - 1].update = 1;
            }
            return sync != 0 ? 0 : 10;
        }
        /* decode ssr 4: combined orbit and clock corrections ------------------------*/
        internal static int decode_ssr4(rtcm_t rtcm, int sys)
        {
            double udint;
            double[] deph = new double[3];
            double[] ddeph = new double[3];
            double[] dclk = new double[3];
            int i;
            int j;
            int k;
            int type;
            int nsat;
            int sync;
            int iod;
            int prn;
            int sat;
            int iode;
            int iodcrc;
            int refd = 0;
            int np;
            int ni;
            int nj;
            int offp;

            type = GlobalMembersRtkcmn.getbitu(rtcm.buff, 24, 12);

            if ((nsat = GlobalMembersRtcm3.decode_ssr1_head(rtcm, sys, ref sync, ref iod, ref udint, ref refd, ref i)) < 0)
            {
                GlobalMembersRtkcmn.trace(2, "rtcm3 %d length error: len=%d\n", type, rtcm.len);
                return -1;
            }
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
                    return sync != 0 ? 0 : 10;
            }
            for (j = 0; j < nsat && i + 191 + np + ni + nj <= rtcm.len * 8; j++)
            {
                prn = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, np) + offp;
                i += np;
                iode = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, ni);
                i += ni;
                iodcrc = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, nj);
                i += nj;
                deph[0] = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 22) * 1E-4;
                i += 22;
                deph[1] = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 20) * 4E-4;
                i += 20;
                deph[2] = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 20) * 4E-4;
                i += 20;
                ddeph[0] = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 21) * 1E-6;
                i += 21;
                ddeph[1] = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 19) * 4E-6;
                i += 19;
                ddeph[2] = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 19) * 4E-6;
                i += 19;

                dclk[0] = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 22) * 1E-4;
                i += 22;
                dclk[1] = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 21) * 1E-6;
                i += 21;
                dclk[2] = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 27) * 2E-8;
                i += 27;

                if ((sat = GlobalMembersRtkcmn.satno(sys, prn)) == 0)
                {
                    GlobalMembersRtkcmn.trace(2, "rtcm3 %d satellite number error: prn=%d\n", type, prn);
                    continue;
                }
                rtcm.ssr[sat - 1].t0[0] = rtcm.ssr[sat - 1].t0[1] = rtcm.time;
                rtcm.ssr[sat - 1].udi[0] = rtcm.ssr[sat - 1].udi[1] = udint;
                rtcm.ssr[sat - 1].iod[0] = rtcm.ssr[sat - 1].iod[1] = iod;
                rtcm.ssr[sat - 1].iode = iode;
                rtcm.ssr[sat - 1].iodcrc = iodcrc;
                rtcm.ssr[sat - 1].refd = refd;

                for (k = 0; k < 3; k++)
                {
                    rtcm.ssr[sat - 1].deph[k] = deph[k];
                    rtcm.ssr[sat - 1].ddeph[k] = ddeph[k];
                    rtcm.ssr[sat - 1].dclk[k] = dclk[k];
                }
                rtcm.ssr[sat - 1].update = 1;
            }
            return sync != 0 ? 0 : 10;
        }
        /* decode ssr 5: ura ---------------------------------------------------------*/
        internal static int decode_ssr5(rtcm_t rtcm, int sys)
        {
            double udint;
            int i;
            int j;
            int type;
            int nsat;
            int sync;
            int iod;
            int prn;
            int sat;
            int ura;
            int np;
            int offp;

            type = GlobalMembersRtkcmn.getbitu(rtcm.buff, 24, 12);

            if ((nsat = GlobalMembersRtcm3.decode_ssr2_head(rtcm, sys, ref sync, ref iod, ref udint, ref i)) < 0)
            {
                GlobalMembersRtkcmn.trace(2, "rtcm3 %d length error: len=%d\n", type, rtcm.len);
                return -1;
            }
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
                    return sync != 0 ? 0 : 10;
            }
            for (j = 0; j < nsat && i + 6 + np <= rtcm.len * 8; j++)
            {
                prn = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, np) + offp;
                i += np;
                ura = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 6);
                i += 6;

                if ((sat = GlobalMembersRtkcmn.satno(sys, prn)) == 0)
                {
                    GlobalMembersRtkcmn.trace(2, "rtcm3 %d satellite number error: prn=%d\n", type, prn);
                    continue;
                }
                rtcm.ssr[sat - 1].t0[3] = rtcm.time;
                rtcm.ssr[sat - 1].udi[3] = udint;
                rtcm.ssr[sat - 1].iod[3] = iod;
                rtcm.ssr[sat - 1].ura = ura;
                rtcm.ssr[sat - 1].update = 1;
            }
            return sync != 0 ? 0 : 10;
        }
        /* decode ssr 6: high rate clock correction ----------------------------------*/
        internal static int decode_ssr6(rtcm_t rtcm, int sys)
        {
            double udint;
            double hrclk;
            int i;
            int j;
            int type;
            int nsat;
            int sync;
            int iod;
            int prn;
            int sat;
            int np;
            int offp;

            type = GlobalMembersRtkcmn.getbitu(rtcm.buff, 24, 12);

            if ((nsat = GlobalMembersRtcm3.decode_ssr2_head(rtcm, sys, ref sync, ref iod, ref udint, ref i)) < 0)
            {
                GlobalMembersRtkcmn.trace(2, "rtcm3 %d length error: len=%d\n", type, rtcm.len);
                return -1;
            }
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
                    return sync != 0 ? 0 : 10;
            }
            for (j = 0; j < nsat && i + 22 + np <= rtcm.len * 8; j++)
            {
                prn = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, np) + offp;
                i += np;
                hrclk = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 22) * 1E-4;
                i += 22;

                if ((sat = GlobalMembersRtkcmn.satno(sys, prn)) == 0)
                {
                    GlobalMembersRtkcmn.trace(2, "rtcm3 %d satellite number error: prn=%d\n", type, prn);
                    continue;
                }
                rtcm.ssr[sat - 1].t0[2] = rtcm.time;
                rtcm.ssr[sat - 1].udi[2] = udint;
                rtcm.ssr[sat - 1].iod[2] = iod;
                rtcm.ssr[sat - 1].hrclk = hrclk;
                rtcm.ssr[sat - 1].update = 1;
            }
            return sync != 0 ? 0 : 10;
        }
        /* get signal index ----------------------------------------------------------*/
        internal static void sigindex(int sys, byte[] code, int[] freq, int n, string opt, int[] ind)
        {
            int i;
            int nex;
            int pri;
            int[] pri_h = new int[8];
            int[] index = new int[8];
            int[] ex = new int[32];

            /* test code priority */
            for (i = 0; i < n; i++)
            {
                if (!code[i])
                    continue;

                if (freq[i] > DefineConstants.NFREQ) // save as extended signal if freq > NFREQ
                {
                    ex[i] = 1;
                    continue;
                }
                /* code priority */
                pri = GlobalMembersRtkcmn.getcodepri(sys, code[i], opt);

                /* select highest priority signal */
                if (pri > pri_h[freq[i] - 1])
                {
                    if (index[freq[i] - 1] != 0)
                    {
                        ex[index[freq[i] - 1] - 1] = 1;
                    }
                    pri_h[freq[i] - 1] = pri;
                    index[freq[i] - 1] = i + 1;
                }
                else
                {
                    ex[i] = 1;
                }
            }
            /* signal index in obs data */
            for (i = nex = 0; i < n; i++)
            {
                if (ex[i] == 0)
                {
                    ind[i] = freq[i] - 1;
                }
                else if (nex < DefineConstants.NEXOBS)
                {
                    ind[i] = DefineConstants.NFREQ + nex++;
                }
                else // no space in obs data
                {
                    GlobalMembersRtkcmn.trace(2, "rtcm msm: no space in obs data sys=%d code=%d\n", sys, code[i]);
                    ind[i] = -1;
                }
#if false
	//        trace(2,"sig pos: sys=%d code=%d ex=%d ind=%d\n",sys,code[i],ex[i],ind[i]);
#endif
            }
        }
        /* save obs data in msm message ----------------------------------------------*/
        internal static void save_msm_obs(rtcm_t rtcm, int sys, msm_h_t h, double[] r, double[] pr, double[] cp, double[] rr, double[] rrf, double[] cnr, int[] @lock, int[] ex, int[] half)
        {
            string[] sig = new string[32];
            double tt;
            double wl;
            byte[] code = new byte[32];
            string msm_type = "";
            string q = null;
            int i;
            int j;
            int k;
            int type;
            int prn;
            int sat;
            int fn;
            int index = 0;
            int[] freq = new int[32];
            int[] ind = new int[32];

            type = GlobalMembersRtkcmn.getbitu(rtcm.buff, 24, 12);

            switch (sys)
            {
                case DefineConstants.SYS_GPS:
                    msm_type = q = rtcm.msmtype[0];
                    break;
                case DefineConstants.SYS_GLO:
                    msm_type = q = rtcm.msmtype[1];
                    break;
                case DefineConstants.SYS_GAL:
                    msm_type = q = rtcm.msmtype[2];
                    break;
                case DefineConstants.SYS_QZS:
                    msm_type = q = rtcm.msmtype[3];
                    break;
                case DefineConstants.SYS_SBS:
                    msm_type = q = rtcm.msmtype[4];
                    break;
                case DefineConstants.SYS_CMP:
                    msm_type = q = rtcm.msmtype[5];
                    break;
            }
            /* id to signal */
            for (i = 0; i < h.nsig; i++)
            {
                switch (sys)
                {
                    case DefineConstants.SYS_GPS:
                        sig[i] = msm_sig_gps[h.sigs[i] - 1];
                        break;
                    case DefineConstants.SYS_GLO:
                        sig[i] = msm_sig_glo[h.sigs[i] - 1];
                        break;
                    case DefineConstants.SYS_GAL:
                        sig[i] = msm_sig_gal[h.sigs[i] - 1];
                        break;
                    case DefineConstants.SYS_QZS:
                        sig[i] = msm_sig_qzs[h.sigs[i] - 1];
                        break;
                    case DefineConstants.SYS_SBS:
                        sig[i] = msm_sig_sbs[h.sigs[i] - 1];
                        break;
                    case DefineConstants.SYS_CMP:
                        sig[i] = msm_sig_cmp[h.sigs[i] - 1];
                        break;
                    default:
                        sig[i] = "";
                        break;
                }
                /* signal to rinex obs type */
                code[i] = GlobalMembersRtkcmn.obs2code(sig[i], ref freq + i);

                /* freqency index for beidou */
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
                if (code[i] != DefineConstants.CODE_NONE)
                {
                    if (q != 0)
                    {
                        q += sprintf(q, "L%s%s", sig[i], i < h.nsig - 1 ? "," : "");
                    }
                }
                else
                {
                    if (q != 0)
                    {
                        q += sprintf(q, "(%d)%s", h.sigs[i], i < h.nsig - 1 ? "," : "");
                    }

                    GlobalMembersRtkcmn.trace(2, "rtcm3 %d: unknown signal id=%2d\n", type, h.sigs[i]);
                }
            }
            GlobalMembersRtkcmn.trace(3, "rtcm3 %d: signals=%s\n", type, msm_type);

            /* get signal index */
            GlobalMembersRtcm3.sigindex(sys, code, freq, h.nsig, rtcm.opt, ind);

            for (i = j = 0; i < h.nsat; i++)
            {

                prn = h.sats[i];
                if (sys == DefineConstants.SYS_QZS)
                {
                    prn += DefineConstants.MINPRNQZS - 1;
                }
                else if (sys == DefineConstants.SYS_SBS)
                {
                    prn += DefineConstants.MINPRNSBS - 1;
                }

                if ((sat = GlobalMembersRtkcmn.satno(sys, prn)) != 0)
                {
                    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                    //ORIGINAL LINE: tt=timediff(rtcm->obs.data[0].time,rtcm->time);
                    tt = GlobalMembersRtkcmn.timediff(rtcm.obs.data[0].time, new gtime_t(rtcm.time));
                    if (rtcm.obsflag != 0 || Math.Abs(tt) > 1E-9)
                    {
                        rtcm.obs.n = rtcm.obsflag = 0;
                    }
                    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                    //ORIGINAL LINE: index=obsindex(&rtcm->obs,rtcm->time,sat);
                    index = GlobalMembersNovatel.obsindex(rtcm.obs, new gtime_t(rtcm.time), sat);
                }
                else
                {
                    GlobalMembersRtkcmn.trace(2, "rtcm3 %d satellite error: prn=%d\n", type, prn);
                }
                for (k = 0; k < h.nsig; k++)
                {
                    if (h.cellmask[k + i * h.nsig] == 0)
                        continue;

                    if (sat != 0 && index >= 0 && ind[k] >= 0)
                    {

                        /* satellite carrier wave length */
                        wl = GlobalMembersRtkcmn.satwavelen(sat, freq[k] - 1, rtcm.nav);

                        /* glonass wave length by extended info */
                        if (sys == DefineConstants.SYS_GLO && ex != 0 && ex[i] <= 13)
                        {
                            fn = ex[i] - 7;
                            wl = DefineConstants.CLIGHT / ((freq[k] == 2 ? DefineConstants.FREQ2_GLO : DefineConstants.FREQ1_GLO) + (freq[k] == 2 ? DefineConstants.DFRQ2_GLO : DefineConstants.DFRQ1_GLO) * fn);
                        }
                        /* pseudorange (m) */
                        if (r[i] != 0.0 && pr[j] > -1E12)
                        {
                            rtcm.obs.data[index].P[ind[k]] = r[i] + pr[j];
                        }
                        /* carrier-phase (cycle) */
                        if (r[i] != 0.0 && cp[j] > -1E12 && wl > 0.0)
                        {
                            rtcm.obs.data[index].L[ind[k]] = (r[i] + cp[j]) / wl;
                        }
                        /* doppler (hz) */
                        if (rr != 0 && rrf != 0 && rrf[j] > -1E12 && wl > 0.0)
                        {
                            rtcm.obs.data[index].D[ind[k]] = (float)(-(rr[i] + rrf[j]) / wl);
                        }
                        rtcm.obs.data[index].LLI[ind[k]] = GlobalMembersRtcm3.lossoflock(rtcm, sat, ind[k], @lock[j]) + (half[j] ? 3 : 0);
                        rtcm.obs.data[index].SNR[ind[k]] = (byte)(cnr[j] * 4.0);
                        rtcm.obs.data[index].code[ind[k]] = code[k];
                    }
                    j++;
                }
            }
        }
        /* decode type msm message header --------------------------------------------*/
        internal static int decode_msm_head(rtcm_t rtcm, int sys, ref int sync, ref int iod, msm_h_t h, ref int hsize)
        {
            msm_h_t h0 = new msm_h_t();
            double tow;
            double tod;
            string msg;
            int i = 24;
            int j;
            int dow;
            int mask;
            int staid;
            int type;
            int ncell = 0;

            type = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 12);
            i += 12;

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: *h=h0;
            h.CopyFrom(h0);
            if (i + 157 <= rtcm.len * 8)
            {
                staid = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 12);
                i += 12;

                if (sys == DefineConstants.SYS_GLO)
                {
                    dow = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 3);
                    i += 3;
                    tod = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 27) * 0.001;
                    i += 27;
                    GlobalMembersRtcm3.adjday_glot(rtcm, tod);
                }
                else if (sys == DefineConstants.SYS_CMP)
                {
                    tow = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 30) * 0.001;
                    i += 30;
                    tow += 14.0; // BDT -> GPST
                    GlobalMembersBinex.adjweek(rtcm, tow);
                }
                else
                {
                    tow = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 30) * 0.001;
                    i += 30;
                    GlobalMembersBinex.adjweek(rtcm, tow);
                }
                sync = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 1);
                i += 1;
                iod = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 3);
                i += 3;
                h.time_s = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 7);
                i += 7;
                h.clk_str = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 2);
                i += 2;
                h.clk_ext = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 2);
                i += 2;
                h.smooth = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 1);
                i += 1;
                h.tint_s = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 3);
                i += 3;
                for (j = 1; j <= 64; j++)
                {
                    mask = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 1);
                    i += 1;
                    if (mask != 0)
                    {
                        h.sats[h.nsat++] = j;
                    }
                }
                for (j = 1; j <= 32; j++)
                {
                    mask = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 1);
                    i += 1;
                    if (mask != 0)
                    {
                        h.sigs[h.nsig++] = j;
                    }
                }
            }
            else
            {
                GlobalMembersRtkcmn.trace(2, "rtcm3 %d length error: len=%d\n", type, rtcm.len);
                return -1;
            }
            /* test station id */
            if (GlobalMembersRtcm3.test_staid(rtcm, staid) == 0)
            {
                return -1;
            }

            if (h.nsat * h.nsig > 64)
            {
                GlobalMembersRtkcmn.trace(2, "rtcm3 %d number of sats and sigs error: nsat=%d nsig=%d\n", type, h.nsat, h.nsig);
                return -1;
            }
            if (i + h.nsat * h.nsig > rtcm.len * 8)
            {
                GlobalMembersRtkcmn.trace(2, "rtcm3 %d length error: len=%d nsat=%d nsig=%d\n", type, rtcm.len, h.nsat, h.nsig);
                return -1;
            }
            for (j = 0; j < h.nsat * h.nsig; j++)
            {
                h.cellmask[j] = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 1);
                i += 1;
                if (h.cellmask[j] != 0)
                {
                    ncell++;
                }
            }
            hsize = i;

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: trace(4,"decode_head_msm: time=%s sys=%d staid=%d nsat=%d nsig=%d sync=%d iod=%d ncell=%d\n", time_str(rtcm->time,2),sys,staid,h->nsat,h->nsig,*sync,*iod,ncell);
            GlobalMembersRtkcmn.trace(4, "decode_head_msm: time=%s sys=%d staid=%d nsat=%d nsig=%d sync=%d iod=%d ncell=%d\n", GlobalMembersRtkcmn.time_str(new gtime_t(rtcm.time), 2), sys, staid, h.nsat, h.nsig, sync, iod, ncell);

            if (rtcm.outtype != 0)
            {
                msg = rtcm.msgtype.Substring(rtcm.msgtype.Length);
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: sprintf(msg," %s staid=%3d nsat=%2d nsig=%2d iod=%2d ncell=%2d sync=%d", time_str(rtcm->time,2),staid,h->nsat,h->nsig,*iod,ncell,*sync);
                msg = string.Format(" {0} staid={1,3:D} nsat={2,2:D} nsig={3,2:D} iod={4,2:D} ncell={5,2:D} sync={6:D}", GlobalMembersRtkcmn.time_str(new gtime_t(rtcm.time), 2), staid, h.nsat, h.nsig, iod, ncell, sync);
            }
            return ncell;
        }
        /* decode unsupported msm message --------------------------------------------*/
        internal static int decode_msm0(rtcm_t rtcm, int sys)
        {
            msm_h_t h = new msm_h_t();
            int i;
            int sync;
            int iod;
            if (GlobalMembersRtcm3.decode_msm_head(rtcm, sys, ref sync, ref iod, h, ref i) < 0)
            {
                return -1;
            }
            rtcm.obsflag = sync == 0;
            return sync != 0 ? 0 : 1;
        }
        /* decode msm 4: full pseudorange and phaserange plus cnr --------------------*/
        internal static int decode_msm4(rtcm_t rtcm, int sys)
        {
            msm_h_t h = new msm_h_t();
            double[] r = new double[64];
            double[] pr = new double[64];
            double[] cp = new double[64];
            double[] cnr = new double[64];
            int i;
            int j;
            int type;
            int sync;
            int iod;
            int ncell;
            int rng;
            int rng_m;
            int prv;
            int cpv;
            int[] @lock = new int[64];
            int[] half = new int[64];

            type = GlobalMembersRtkcmn.getbitu(rtcm.buff, 24, 12);

            /* decode msm header */
            if ((ncell = GlobalMembersRtcm3.decode_msm_head(rtcm, sys, ref sync, ref iod, h, ref i)) < 0)
            {
                return -1;
            }

            if (i + h.nsat * 18 + ncell * 48 > rtcm.len * 8)
            {
                GlobalMembersRtkcmn.trace(2, "rtcm3 %d length error: nsat=%d ncell=%d len=%d\n", type, h.nsat, ncell, rtcm.len);
                return -1;
            }
            for (j = 0; j < h.nsat; j++)
            {
                r[j] = 0.0;
            }
            for (j = 0; j < ncell; j++)
            {
                pr[j] = cp[j] = -1E16;
            }

            /* decode satellite data */
            for (j = 0; j < h.nsat; j++) // range
            {
                rng = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 8);
                i += 8;
                if (rng != 255)
                {
                    r[j] = rng * DefineConstants.CLIGHT * 0.001;
                }
            }
            for (j = 0; j < h.nsat; j++)
            {
                rng_m = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 10);
                i += 10;
                if (r[j] != 0.0)
                {
                    r[j] += rng_m * DefineConstants.P2_10 * DefineConstants.CLIGHT * 0.001;
                }
            }
            /* decode signal data */
            for (j = 0; j < ncell; j++) // pseudorange
            {
                prv = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 15);
                i += 15;
                if (prv != -16384)
                {
                    pr[j] = prv * 5.960464477539063E-0x8* DefineConstants.CLIGHT * 0.001;
                }
            }
            for (j = 0; j < ncell; j++) // phaserange
            {
                cpv = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 22);
                i += 22;
                if (cpv != -2097152)
                {
                    cp[j] = cpv * 1.862645149230957E-0x9* DefineConstants.CLIGHT * 0.001;
                }
            }
            for (j = 0; j < ncell; j++) // lock time
            {
                @lock[j] = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 4);
                i += 4;
            }
            for (j = 0; j < ncell; j++) // half-cycle ambiguity
            {
                half[j] = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 1);
                i += 1;
            }
            for (j = 0; j < ncell; j++) // cnr
            {
                cnr[j] = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 6) * 1.0;
                i += 6;
            }
            /* save obs data in msm message */
            GlobalMembersRtcm3.save_msm_obs(rtcm, sys, h, r, pr, cp, null, null, cnr, @lock, null, half);

            rtcm.obsflag = sync == 0;
            return sync != 0 ? 0 : 1;
        }
        /* decode msm 5: full pseudorange, phaserange, phaserangerate and cnr --------*/
        internal static int decode_msm5(rtcm_t rtcm, int sys)
        {
            msm_h_t h = new msm_h_t();
            double[] r = new double[64];
            double[] rr = new double[64];
            double[] pr = new double[64];
            double[] cp = new double[64];
            double[] rrf = new double[64];
            double[] cnr = new double[64];
            int i;
            int j;
            int type;
            int sync;
            int iod;
            int ncell;
            int rng;
            int rng_m;
            int rate;
            int prv;
            int cpv;
            int rrv;
            int[] @lock = new int[64];
            int[] ex = new int[64];
            int[] half = new int[64];

            type = GlobalMembersRtkcmn.getbitu(rtcm.buff, 24, 12);

            /* decode msm header */
            if ((ncell = GlobalMembersRtcm3.decode_msm_head(rtcm, sys, ref sync, ref iod, h, ref i)) < 0)
            {
                return -1;
            }

            if (i + h.nsat * 36 + ncell * 63 > rtcm.len * 8)
            {
                GlobalMembersRtkcmn.trace(2, "rtcm3 %d length error: nsat=%d ncell=%d len=%d\n", type, h.nsat, ncell, rtcm.len);
                return -1;
            }
            for (j = 0; j < h.nsat; j++)
            {
                r[j] = rr[j] = 0.0;
                ex[j] = 15;
            }
            for (j = 0; j < ncell; j++)
            {
                pr[j] = cp[j] = rrf[j] = -1E16;
            }

            /* decode satellite data */
            for (j = 0; j < h.nsat; j++) // range
            {
                rng = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 8);
                i += 8;
                if (rng != 255)
                {
                    r[j] = rng * DefineConstants.CLIGHT * 0.001;
                }
            }
            for (j = 0; j < h.nsat; j++) // extended info
            {
                ex[j] = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 4);
                i += 4;
            }
            for (j = 0; j < h.nsat; j++)
            {
                rng_m = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 10);
                i += 10;
                if (r[j] != 0.0)
                {
                    r[j] += rng_m * DefineConstants.P2_10 * DefineConstants.CLIGHT * 0.001;
                }
            }
            for (j = 0; j < h.nsat; j++) // phaserangerate
            {
                rate = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 14);
                i += 14;
                if (rate != -8192)
                {
                    rr[j] = rate * 1.0;
                }
            }
            /* decode signal data */
            for (j = 0; j < ncell; j++) // pseudorange
            {
                prv = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 15);
                i += 15;
                if (prv != -16384)
                {
                    pr[j] = prv * 5.960464477539063E-0x8* DefineConstants.CLIGHT * 0.001;
                }
            }
            for (j = 0; j < ncell; j++) // phaserange
            {
                cpv = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 22);
                i += 22;
                if (cpv != -2097152)
                {
                    cp[j] = cpv * 1.862645149230957E-0x9* DefineConstants.CLIGHT * 0.001;
                }
            }
            for (j = 0; j < ncell; j++) // lock time
            {
                @lock[j] = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 4);
                i += 4;
            }
            for (j = 0; j < ncell; j++) // half-cycle ambiguity
            {
                half[j] = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 1);
                i += 1;
            }
            for (j = 0; j < ncell; j++) // cnr
            {
                cnr[j] = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 6) * 1.0;
                i += 6;
            }
            for (j = 0; j < ncell; j++) // phaserangerate
            {
                rrv = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 15);
                i += 15;
                if (rrv != -16384)
                {
                    rrf[j] = rrv * 0.0001;
                }
            }
            /* save obs data in msm message */
            GlobalMembersRtcm3.save_msm_obs(rtcm, sys, h, r, pr, cp, rr, rrf, cnr, @lock, ex, half);

            rtcm.obsflag = sync == 0;
            return sync != 0 ? 0 : 1;
        }
        /* decode msm 6: full pseudorange and phaserange plus cnr (high-res) ---------*/
        internal static int decode_msm6(rtcm_t rtcm, int sys)
        {
            msm_h_t h = new msm_h_t();
            double[] r = new double[64];
            double[] pr = new double[64];
            double[] cp = new double[64];
            double[] cnr = new double[64];
            int i;
            int j;
            int type;
            int sync;
            int iod;
            int ncell;
            int rng;
            int rng_m;
            int prv;
            int cpv;
            int[] @lock = new int[64];
            int[] half = new int[64];

            type = GlobalMembersRtkcmn.getbitu(rtcm.buff, 24, 12);

            /* decode msm header */
            if ((ncell = GlobalMembersRtcm3.decode_msm_head(rtcm, sys, ref sync, ref iod, h, ref i)) < 0)
            {
                return -1;
            }

            if (i + h.nsat * 18 + ncell * 65 > rtcm.len * 8)
            {
                GlobalMembersRtkcmn.trace(2, "rtcm3 %d length error: nsat=%d ncell=%d len=%d\n", type, h.nsat, ncell, rtcm.len);
                return -1;
            }
            for (j = 0; j < h.nsat; j++)
            {
                r[j] = 0.0;
            }
            for (j = 0; j < ncell; j++)
            {
                pr[j] = cp[j] = -1E16;
            }

            /* decode satellite data */
            for (j = 0; j < h.nsat; j++) // range
            {
                rng = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 8);
                i += 8;
                if (rng != 255)
                {
                    r[j] = rng * DefineConstants.CLIGHT * 0.001;
                }
            }
            for (j = 0; j < h.nsat; j++)
            {
                rng_m = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 10);
                i += 10;
                if (r[j] != 0.0)
                {
                    r[j] += rng_m * DefineConstants.P2_10 * DefineConstants.CLIGHT * 0.001;
                }
            }
            /* decode signal data */
            for (j = 0; j < ncell; j++) // pseudorange
            {
                prv = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 20);
                i += 20;
                if (prv != -524288)
                {
                    pr[j] = prv * 1.862645149230957E-0x9* DefineConstants.CLIGHT * 0.001;
                }
            }
            for (j = 0; j < ncell; j++) // phaserange
            {
                cpv = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 24);
                i += 24;
                if (cpv != -8388608)
                {
                    cp[j] = cpv * DefineConstants.P2_31 * DefineConstants.CLIGHT * 0.001;
                }
            }
            for (j = 0; j < ncell; j++) // lock time
            {
                @lock[j] = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 10);
                i += 10;
            }
            for (j = 0; j < ncell; j++) // half-cycle ambiguity
            {
                half[j] = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 1);
                i += 1;
            }
            for (j = 0; j < ncell; j++) // cnr
            {
                cnr[j] = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 10) * 0.0625;
                i += 10;
            }
            /* save obs data in msm message */
            GlobalMembersRtcm3.save_msm_obs(rtcm, sys, h, r, pr, cp, null, null, cnr, @lock, null, half);

            rtcm.obsflag = sync == 0;
            return sync != 0 ? 0 : 1;
        }
        /* decode msm 7: full pseudorange, phaserange, phaserangerate and cnr (h-res) */
        internal static int decode_msm7(rtcm_t rtcm, int sys)
        {
            msm_h_t h = new msm_h_t();
            double[] r = new double[64];
            double[] rr = new double[64];
            double[] pr = new double[64];
            double[] cp = new double[64];
            double[] rrf = new double[64];
            double[] cnr = new double[64];
            int i;
            int j;
            int type;
            int sync;
            int iod;
            int ncell;
            int rng;
            int rng_m;
            int rate;
            int prv;
            int cpv;
            int rrv;
            int[] @lock = new int[64];
            int[] ex = new int[64];
            int[] half = new int[64];

            type = GlobalMembersRtkcmn.getbitu(rtcm.buff, 24, 12);

            /* decode msm header */
            if ((ncell = GlobalMembersRtcm3.decode_msm_head(rtcm, sys, ref sync, ref iod, h, ref i)) < 0)
            {
                return -1;
            }

            if (i + h.nsat * 36 + ncell * 80 > rtcm.len * 8)
            {
                GlobalMembersRtkcmn.trace(2, "rtcm3 %d length error: nsat=%d ncell=%d len=%d\n", type, h.nsat, ncell, rtcm.len);
                return -1;
            }
            for (j = 0; j < h.nsat; j++)
            {
                r[j] = rr[j] = 0.0;
                ex[j] = 15;
            }
            for (j = 0; j < ncell; j++)
            {
                pr[j] = cp[j] = rrf[j] = -1E16;
            }

            /* decode satellite data */
            for (j = 0; j < h.nsat; j++) // range
            {
                rng = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 8);
                i += 8;
                if (rng != 255)
                {
                    r[j] = rng * DefineConstants.CLIGHT * 0.001;
                }
            }
            for (j = 0; j < h.nsat; j++) // extended info
            {
                ex[j] = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 4);
                i += 4;
            }
            for (j = 0; j < h.nsat; j++)
            {
                rng_m = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 10);
                i += 10;
                if (r[j] != 0.0)
                {
                    r[j] += rng_m * DefineConstants.P2_10 * DefineConstants.CLIGHT * 0.001;
                }
            }
            for (j = 0; j < h.nsat; j++) // phaserangerate
            {
                rate = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 14);
                i += 14;
                if (rate != -8192)
                {
                    rr[j] = rate * 1.0;
                }
            }
            /* decode signal data */
            for (j = 0; j < ncell; j++) // pseudorange
            {
                prv = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 20);
                i += 20;
                if (prv != -524288)
                {
                    pr[j] = prv * 1.862645149230957E-0x9* DefineConstants.CLIGHT * 0.001;
                }
            }
            for (j = 0; j < ncell; j++) // phaserange
            {
                cpv = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 24);
                i += 24;
                if (cpv != -8388608)
                {
                    cp[j] = cpv * DefineConstants.P2_31 * DefineConstants.CLIGHT * 0.001;
                }
            }
            for (j = 0; j < ncell; j++) // lock time
            {
                @lock[j] = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 10);
                i += 10;
            }
            for (j = 0; j < ncell; j++) // half-cycle amiguity
            {
                half[j] = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 1);
                i += 1;
            }
            for (j = 0; j < ncell; j++) // cnr
            {
                cnr[j] = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 10) * 0.0625;
                i += 10;
            }
            for (j = 0; j < ncell; j++) // phaserangerate
            {
                rrv = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 15);
                i += 15;
                if (rrv != -16384)
                {
                    rrf[j] = rrv * 0.0001;
                }
            }
            /* save obs data in msm message */
            GlobalMembersRtcm3.save_msm_obs(rtcm, sys, h, r, pr, cp, rr, rrf, cnr, @lock, ex, half);

            rtcm.obsflag = sync == 0;
            return sync != 0 ? 0 : 1;
        }
        /* decode type 1230: glonass L1 and L2 code-phase biases ---------------------*/
        internal static int decode_type1230(rtcm_t rtcm)
        {
            GlobalMembersRtkcmn.trace(2, "rtcm3 1230: not supported message\n");
            return 0;
        }
        /* decode rtcm ver.3 message -------------------------------------------------*/
        public static int decode_rtcm3(rtcm_t rtcm)
        {
            int ret = 0;
            int type = GlobalMembersRtkcmn.getbitu(rtcm.buff, 24, 12);

            GlobalMembersRtkcmn.trace(3, "decode_rtcm3: len=%3d type=%d\n", rtcm.len, type);

            if (rtcm.outtype != 0)
            {
                rtcm.msgtype = string.Format("RTCM {0,4:D} ({1,4:D}):", type, rtcm.len);
            }
            /* real-time input option */
            if (StringFunctions.StrStr(rtcm.opt, "-RT_INP"))
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                //ORIGINAL LINE: rtcm->time=utc2gpst(timeget());
                rtcm.time.CopyFrom(GlobalMembersRtkcmn.utc2gpst(GlobalMembersRtkcmn.timeget()));
            }
            switch (type)
            {
                case 1001: // not supported
                    ret = GlobalMembersRtcm3.decode_type1001(rtcm);
                    break;
                case 1002:
                    ret = GlobalMembersRtcm3.decode_type1002(rtcm);
                    break;
                case 1003: // not supported
                    ret = GlobalMembersRtcm3.decode_type1003(rtcm);
                    break;
                case 1004:
                    ret = GlobalMembersRtcm3.decode_type1004(rtcm);
                    break;
                case 1005:
                    ret = GlobalMembersRtcm3.decode_type1005(rtcm);
                    break;
                case 1006:
                    ret = GlobalMembersRtcm3.decode_type1006(rtcm);
                    break;
                case 1007:
                    ret = GlobalMembersRtcm3.decode_type1007(rtcm);
                    break;
                case 1008:
                    ret = GlobalMembersRtcm3.decode_type1008(rtcm);
                    break;
                case 1009: // not supported
                    ret = GlobalMembersRtcm3.decode_type1009(rtcm);
                    break;
                case 1010:
                    ret = GlobalMembersRtcm3.decode_type1010(rtcm);
                    break;
                case 1011: // not supported
                    ret = GlobalMembersRtcm3.decode_type1011(rtcm);
                    break;
                case 1012:
                    ret = GlobalMembersRtcm3.decode_type1012(rtcm);
                    break;
                case 1013: // not supported
                    ret = GlobalMembersRtcm3.decode_type1013(rtcm);
                    break;
                case 1019:
                    ret = GlobalMembersRtcm3.decode_type1019(rtcm);
                    break;
                case 1020:
                    ret = GlobalMembersRtcm3.decode_type1020(rtcm);
                    break;
                case 1021: // not supported
                    ret = GlobalMembersRtcm3.decode_type1021(rtcm);
                    break;
                case 1022: // not supported
                    ret = GlobalMembersRtcm3.decode_type1022(rtcm);
                    break;
                case 1023: // not supported
                    ret = GlobalMembersRtcm3.decode_type1023(rtcm);
                    break;
                case 1024: // not supported
                    ret = GlobalMembersRtcm3.decode_type1024(rtcm);
                    break;
                case 1025: // not supported
                    ret = GlobalMembersRtcm3.decode_type1025(rtcm);
                    break;
                case 1026: // not supported
                    ret = GlobalMembersRtcm3.decode_type1026(rtcm);
                    break;
                case 1027: // not supported
                    ret = GlobalMembersRtcm3.decode_type1027(rtcm);
                    break;
                case 1030: // not supported
                    ret = GlobalMembersRtcm3.decode_type1030(rtcm);
                    break;
                case 1031: // not supported
                    ret = GlobalMembersRtcm3.decode_type1031(rtcm);
                    break;
                case 1032: // not supported
                    ret = GlobalMembersRtcm3.decode_type1032(rtcm);
                    break;
                case 1033:
                    ret = GlobalMembersRtcm3.decode_type1033(rtcm);
                    break;
                case 1034: // not supported
                    ret = GlobalMembersRtcm3.decode_type1034(rtcm);
                    break;
                case 1035: // not supported
                    ret = GlobalMembersRtcm3.decode_type1035(rtcm);
                    break;
                case 1037: // not supported
                    ret = GlobalMembersRtcm3.decode_type1037(rtcm);
                    break;
                case 1038: // not supported
                    ret = GlobalMembersRtcm3.decode_type1038(rtcm);
                    break;
                case 1039: // not supported
                    ret = GlobalMembersRtcm3.decode_type1039(rtcm);
                    break;
                case 1044:
                    ret = GlobalMembersRtcm3.decode_type1044(rtcm);
                    break;
                case 1045:
                    ret = GlobalMembersRtcm3.decode_type1045(rtcm);
                    break;
                case 1047: // tentative mt
                    ret = GlobalMembersRtcm3.decode_type1047(rtcm);
                    break;
                case 1057:
                    ret = GlobalMembersRtcm3.decode_ssr1(rtcm, DefineConstants.SYS_GPS);
                    break;
                case 1058:
                    ret = GlobalMembersRtcm3.decode_ssr2(rtcm, DefineConstants.SYS_GPS);
                    break;
                case 1059:
                    ret = GlobalMembersRtcm3.decode_ssr3(rtcm, DefineConstants.SYS_GPS);
                    break;
                case 1060:
                    ret = GlobalMembersRtcm3.decode_ssr4(rtcm, DefineConstants.SYS_GPS);
                    break;
                case 1061:
                    ret = GlobalMembersRtcm3.decode_ssr5(rtcm, DefineConstants.SYS_GPS);
                    break;
                case 1062:
                    ret = GlobalMembersRtcm3.decode_ssr6(rtcm, DefineConstants.SYS_GPS);
                    break;
                case 1063:
                    ret = GlobalMembersRtcm3.decode_ssr1(rtcm, DefineConstants.SYS_GLO);
                    break;
                case 1064:
                    ret = GlobalMembersRtcm3.decode_ssr2(rtcm, DefineConstants.SYS_GLO);
                    break;
                case 1065:
                    ret = GlobalMembersRtcm3.decode_ssr3(rtcm, DefineConstants.SYS_GLO);
                    break;
                case 1066:
                    ret = GlobalMembersRtcm3.decode_ssr4(rtcm, DefineConstants.SYS_GLO);
                    break;
                case 1067:
                    ret = GlobalMembersRtcm3.decode_ssr5(rtcm, DefineConstants.SYS_GLO);
                    break;
                case 1068:
                    ret = GlobalMembersRtcm3.decode_ssr6(rtcm, DefineConstants.SYS_GLO);
                    break;
                case 1071: // not supported
                    ret = GlobalMembersRtcm3.decode_msm0(rtcm, DefineConstants.SYS_GPS);
                    break;
                case 1072: // not supported
                    ret = GlobalMembersRtcm3.decode_msm0(rtcm, DefineConstants.SYS_GPS);
                    break;
                case 1073: // not supported
                    ret = GlobalMembersRtcm3.decode_msm0(rtcm, DefineConstants.SYS_GPS);
                    break;
                case 1074:
                    ret = GlobalMembersRtcm3.decode_msm4(rtcm, DefineConstants.SYS_GPS);
                    break;
                case 1075:
                    ret = GlobalMembersRtcm3.decode_msm5(rtcm, DefineConstants.SYS_GPS);
                    break;
                case 1076:
                    ret = GlobalMembersRtcm3.decode_msm6(rtcm, DefineConstants.SYS_GPS);
                    break;
                case 1077:
                    ret = GlobalMembersRtcm3.decode_msm7(rtcm, DefineConstants.SYS_GPS);
                    break;
                case 1081: // not supported
                    ret = GlobalMembersRtcm3.decode_msm0(rtcm, DefineConstants.SYS_GLO);
                    break;
                case 1082: // not supported
                    ret = GlobalMembersRtcm3.decode_msm0(rtcm, DefineConstants.SYS_GLO);
                    break;
                case 1083: // not supported
                    ret = GlobalMembersRtcm3.decode_msm0(rtcm, DefineConstants.SYS_GLO);
                    break;
                case 1084:
                    ret = GlobalMembersRtcm3.decode_msm4(rtcm, DefineConstants.SYS_GLO);
                    break;
                case 1085:
                    ret = GlobalMembersRtcm3.decode_msm5(rtcm, DefineConstants.SYS_GLO);
                    break;
                case 1086:
                    ret = GlobalMembersRtcm3.decode_msm6(rtcm, DefineConstants.SYS_GLO);
                    break;
                case 1087:
                    ret = GlobalMembersRtcm3.decode_msm7(rtcm, DefineConstants.SYS_GLO);
                    break;
                case 1091: // not supported
                    ret = GlobalMembersRtcm3.decode_msm0(rtcm, DefineConstants.SYS_GAL);
                    break;
                case 1092: // not supported
                    ret = GlobalMembersRtcm3.decode_msm0(rtcm, DefineConstants.SYS_GAL);
                    break;
                case 1093: // not supported
                    ret = GlobalMembersRtcm3.decode_msm0(rtcm, DefineConstants.SYS_GAL);
                    break;
                case 1094:
                    ret = GlobalMembersRtcm3.decode_msm4(rtcm, DefineConstants.SYS_GAL);
                    break;
                case 1095:
                    ret = GlobalMembersRtcm3.decode_msm5(rtcm, DefineConstants.SYS_GAL);
                    break;
                case 1096:
                    ret = GlobalMembersRtcm3.decode_msm6(rtcm, DefineConstants.SYS_GAL);
                    break;
                case 1097:
                    ret = GlobalMembersRtcm3.decode_msm7(rtcm, DefineConstants.SYS_GAL);
                    break;
                case 1101: // not supported
                    ret = GlobalMembersRtcm3.decode_msm0(rtcm, DefineConstants.SYS_SBS);
                    break;
                case 1102: // not supported
                    ret = GlobalMembersRtcm3.decode_msm0(rtcm, DefineConstants.SYS_SBS);
                    break;
                case 1103: // not supported
                    ret = GlobalMembersRtcm3.decode_msm0(rtcm, DefineConstants.SYS_SBS);
                    break;
                case 1104:
                    ret = GlobalMembersRtcm3.decode_msm4(rtcm, DefineConstants.SYS_SBS);
                    break;
                case 1105:
                    ret = GlobalMembersRtcm3.decode_msm5(rtcm, DefineConstants.SYS_SBS);
                    break;
                case 1106:
                    ret = GlobalMembersRtcm3.decode_msm6(rtcm, DefineConstants.SYS_SBS);
                    break;
                case 1107:
                    ret = GlobalMembersRtcm3.decode_msm7(rtcm, DefineConstants.SYS_SBS);
                    break;
                case 1111: // not supported
                    ret = GlobalMembersRtcm3.decode_msm0(rtcm, DefineConstants.SYS_QZS);
                    break;
                case 1112: // not supported
                    ret = GlobalMembersRtcm3.decode_msm0(rtcm, DefineConstants.SYS_QZS);
                    break;
                case 1113: // not supported
                    ret = GlobalMembersRtcm3.decode_msm0(rtcm, DefineConstants.SYS_QZS);
                    break;
                case 1114:
                    ret = GlobalMembersRtcm3.decode_msm4(rtcm, DefineConstants.SYS_QZS);
                    break;
                case 1115:
                    ret = GlobalMembersRtcm3.decode_msm5(rtcm, DefineConstants.SYS_QZS);
                    break;
                case 1116:
                    ret = GlobalMembersRtcm3.decode_msm6(rtcm, DefineConstants.SYS_QZS);
                    break;
                case 1117:
                    ret = GlobalMembersRtcm3.decode_msm7(rtcm, DefineConstants.SYS_QZS);
                    break;
                case 1121: // not supported
                    ret = GlobalMembersRtcm3.decode_msm0(rtcm, DefineConstants.SYS_CMP);
                    break;
                case 1122: // not supported
                    ret = GlobalMembersRtcm3.decode_msm0(rtcm, DefineConstants.SYS_CMP);
                    break;
                case 1123: // not supported
                    ret = GlobalMembersRtcm3.decode_msm0(rtcm, DefineConstants.SYS_CMP);
                    break;
                case 1124:
                    ret = GlobalMembersRtcm3.decode_msm4(rtcm, DefineConstants.SYS_CMP);
                    break;
                case 1125:
                    ret = GlobalMembersRtcm3.decode_msm5(rtcm, DefineConstants.SYS_CMP);
                    break;
                case 1126:
                    ret = GlobalMembersRtcm3.decode_msm6(rtcm, DefineConstants.SYS_CMP);
                    break;
                case 1127:
                    ret = GlobalMembersRtcm3.decode_msm7(rtcm, DefineConstants.SYS_CMP);
                    break;
                case 1230: // not supported
                    ret = GlobalMembersRtcm3.decode_type1230(rtcm);
                    break;
                case 1240:
                    ret = GlobalMembersRtcm3.decode_ssr1(rtcm, DefineConstants.SYS_GAL);
                    break;
                case 1241:
                    ret = GlobalMembersRtcm3.decode_ssr2(rtcm, DefineConstants.SYS_GAL);
                    break;
                case 1242:
                    ret = GlobalMembersRtcm3.decode_ssr3(rtcm, DefineConstants.SYS_GAL);
                    break;
                case 1243:
                    ret = GlobalMembersRtcm3.decode_ssr4(rtcm, DefineConstants.SYS_GAL);
                    break;
                case 1244:
                    ret = GlobalMembersRtcm3.decode_ssr5(rtcm, DefineConstants.SYS_GAL);
                    break;
                case 1245:
                    ret = GlobalMembersRtcm3.decode_ssr6(rtcm, DefineConstants.SYS_GAL);
                    break;
                case 1246:
                    ret = GlobalMembersRtcm3.decode_ssr1(rtcm, DefineConstants.SYS_QZS);
                    break;
                case 1247:
                    ret = GlobalMembersRtcm3.decode_ssr2(rtcm, DefineConstants.SYS_QZS);
                    break;
                case 1248:
                    ret = GlobalMembersRtcm3.decode_ssr3(rtcm, DefineConstants.SYS_QZS);
                    break;
                case 1249:
                    ret = GlobalMembersRtcm3.decode_ssr4(rtcm, DefineConstants.SYS_QZS);
                    break;
                case 1250:
                    ret = GlobalMembersRtcm3.decode_ssr5(rtcm, DefineConstants.SYS_QZS);
                    break;
                case 1251:
                    ret = GlobalMembersRtcm3.decode_ssr6(rtcm, DefineConstants.SYS_QZS);
                    break;
                case 1252:
                    ret = GlobalMembersRtcm3.decode_ssr1(rtcm, DefineConstants.SYS_SBS);
                    break;
                case 1253:
                    ret = GlobalMembersRtcm3.decode_ssr2(rtcm, DefineConstants.SYS_SBS);
                    break;
                case 1254:
                    ret = GlobalMembersRtcm3.decode_ssr3(rtcm, DefineConstants.SYS_SBS);
                    break;
                case 1255:
                    ret = GlobalMembersRtcm3.decode_ssr4(rtcm, DefineConstants.SYS_SBS);
                    break;
                case 1256:
                    ret = GlobalMembersRtcm3.decode_ssr5(rtcm, DefineConstants.SYS_SBS);
                    break;
                case 1257:
                    ret = GlobalMembersRtcm3.decode_ssr6(rtcm, DefineConstants.SYS_SBS);
                    break;
                case 1258:
                    ret = GlobalMembersRtcm3.decode_ssr1(rtcm, DefineConstants.SYS_CMP);
                    break;
                case 1259:
                    ret = GlobalMembersRtcm3.decode_ssr2(rtcm, DefineConstants.SYS_CMP);
                    break;
                case 1260:
                    ret = GlobalMembersRtcm3.decode_ssr3(rtcm, DefineConstants.SYS_CMP);
                    break;
                case 1261:
                    ret = GlobalMembersRtcm3.decode_ssr4(rtcm, DefineConstants.SYS_CMP);
                    break;
                case 1262:
                    ret = GlobalMembersRtcm3.decode_ssr5(rtcm, DefineConstants.SYS_CMP);
                    break;
                case 1263:
                    ret = GlobalMembersRtcm3.decode_ssr6(rtcm, DefineConstants.SYS_CMP);
                    break;
            }
            if (ret >= 0)
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

    /* type definition -----------------------------------------------------------*/

    public class msm_h_t // multi-signal-message header type
    {
        public byte iod; // issue of data station
        public byte time_s; // cumulative session transmitting time
        public byte clk_str; // clock steering indicator
        public byte clk_ext; // external clock indicator
        public byte smooth; // divergence free smoothing indicator
        public byte tint_s; // soothing interval
        public byte nsat; // number of satellites/signals
        public byte nsig;
        public byte[] sats = new byte[64]; // satellites
        public byte[] sigs = new byte[32]; // signals
        public byte[] cellmask = new byte[64]; // cell mask
    }
}
