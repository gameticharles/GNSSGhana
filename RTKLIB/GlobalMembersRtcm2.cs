using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ghGPS.Classes
{
    public static class GlobalMembersRtcm2
    {
        /*------------------------------------------------------------------------------
        * rtcm2.c : rtcm ver.2 message functions
        *
        *          Copyright (C) 2009-2014 by T.TAKASU, All rights reserved.
        *
        * references :
        *     see rtcm.c
        *
        * version : $Revision:$ $Date:$
        * history : 2011/11/28 1.0  separated from rtcm.c
        *           2014/10/21 1.1  fix problem on week rollover in rtcm 2 type 14
        *-----------------------------------------------------------------------------*/


        internal const string rcsid = "$Id:$";

        /* adjust hourly rollover of rtcm 2 time -------------------------------------*/
        internal static void adjhour(rtcm_t rtcm, double zcnt)
        {
            double tow;
            double hour;
            double sec;
            int week;

            /* if no time, get cpu time */
            if (rtcm.time.time == 0)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                //ORIGINAL LINE: rtcm->time=utc2gpst(timeget());
                rtcm.time.CopyFrom(GlobalMembersRtkcmn.utc2gpst(GlobalMembersRtkcmn.timeget()));
            }
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: tow=time2gpst(rtcm->time,&week);
            tow = GlobalMembersRtkcmn.time2gpst(new gtime_t(rtcm.time), ref week);
            hour = Math.Floor(tow / 3600.0);
            sec = tow - hour * 3600.0;
            if (zcnt < sec - 1800.0)
            {
                zcnt += 3600.0;
            }
            else if (zcnt > sec + 1800.0)
            {
                zcnt -= 3600.0;
            }
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: rtcm->time=gpst2time(week,hour *3600+zcnt);
            rtcm.time.CopyFrom(GlobalMembersRtkcmn.gpst2time(week, hour * 3600 + zcnt));
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
            for (j = 0; j < DefineConstants.NFREQ; j++)
            {
                obs.data[i].L[j] = obs.data[i].P[j] = 0.0;
                obs.data[i].D[j] = 0.0;
                obs.data[i].SNR[j] = obs.data[i].LLI[j] = obs.data[i].code[j] = 0;
            }
            obs.n++;
            return i;
        }
        /* decode type 1/9: differential gps correction/partial correction set -------*/
        internal static int decode_type1(rtcm_t rtcm)
        {
            int i = 48;
            int fact;
            int udre;
            int prn;
            int sat;
            int iod;
            double prc;
            double rrc;

            GlobalMembersRtkcmn.trace(4, "decode_type1: len=%d\n", rtcm.len);

            while (i + 40 <= rtcm.len * 8)
            {
                fact = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 1);
                i += 1;
                udre = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 2);
                i += 2;
                prn = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 5);
                i += 5;
                prc = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 16);
                i += 16;
                rrc = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 8);
                i += 8;
                iod = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 8);
                i += 8;
                if (prn == 0)
                {
                    prn = 32;
                }
                if (prc == 0x80000000 || rrc == 0xFFFF8000)
                {
                    GlobalMembersRtkcmn.trace(2, "rtcm2 1 prc/rrc indicates satellite problem: prn=%d\n", prn);
                    continue;
                }
                if (rtcm.dgps != null)
                {
                    sat = GlobalMembersRtkcmn.satno(DefineConstants.SYS_GPS, prn);
                    rtcm.dgps[sat - 1].t0 = rtcm.time;
                    rtcm.dgps[sat - 1].prc = prc * (fact != 0 ? 0.32 : 0.02);
                    rtcm.dgps[sat - 1].rrc = rrc * (fact != 0 ? 0.032 : 0.002);
                    rtcm.dgps[sat - 1].iod = iod;
                    rtcm.dgps[sat - 1].udre = udre;
                }
            }
            return 7;
        }
        /* decode type 3: reference station parameter --------------------------------*/
        internal static int decode_type3(rtcm_t rtcm)
        {
            int i = 48;

            GlobalMembersRtkcmn.trace(4, "decode_type3: len=%d\n", rtcm.len);

            if (i + 96 <= rtcm.len * 8)
            {
                rtcm.sta.pos[0] = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 32) * 0.01;
                i += 32;
                rtcm.sta.pos[1] = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 32) * 0.01;
                i += 32;
                rtcm.sta.pos[2] = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 32) * 0.01;
            }
            else
            {
                GlobalMembersRtkcmn.trace(2, "rtcm2 3 length error: len=%d\n", rtcm.len);
                return -1;
            }
            return 5;
        }
        /* decode type 14: gps time of week ------------------------------------------*/
        internal static int decode_type14(rtcm_t rtcm)
        {
            double zcnt;
            int i = 48;
            int week;
            int hour;
            int leaps;

            GlobalMembersRtkcmn.trace(4, "decode_type14: len=%d\n", rtcm.len);

            zcnt = GlobalMembersRtkcmn.getbitu(rtcm.buff, 24, 13);
            if (i + 24 <= rtcm.len * 8)
            {
                week = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 10);
                i += 10;
                hour = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 8);
                i += 8;
                leaps = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 6);
            }
            else
            {
                GlobalMembersRtkcmn.trace(2, "rtcm2 14 length error: len=%d\n", rtcm.len);
                return -1;
            }
            week = GlobalMembersRtkcmn.adjgpsweek(week);
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: rtcm->time=gpst2time(week,hour *3600.0+zcnt *0.6);
            rtcm.time.CopyFrom(GlobalMembersRtkcmn.gpst2time(week, hour * 3600.0 + zcnt * 0.6));
            rtcm.nav.leaps = leaps;
            return 6;
        }
        /* decode type 16: gps special message ---------------------------------------*/
        internal static int decode_type16(rtcm_t rtcm)
        {
            int i = 48;
            int n = 0;

            GlobalMembersRtkcmn.trace(4, "decode_type16: len=%d\n", rtcm.len);

            while (i + 8 <= rtcm.len * 8 && n < 90)
            {
                rtcm.msg[n++] = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 8);
                i += 8;
            }
            rtcm.msg[n] = '\0';

            GlobalMembersRtkcmn.trace(3, "rtcm2 16 message: %s\n", rtcm.msg);
            return 9;
        }
        /* decode type 17: gps ephemerides -------------------------------------------*/
        internal static int decode_type17(rtcm_t rtcm)
        {
            eph_t eph = new eph_t();
            double toc;
            double sqrtA;
            int i = 48;
            int week;
            int prn;
            int sat;

            GlobalMembersRtkcmn.trace(4, "decode_type17: len=%d\n", rtcm.len);

            if (i + 480 <= rtcm.len * 8)
            {
                week = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 10);
                i += 10;
                eph.idot = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 14) * DefineConstants.P2_43 * DefineConstants.SC2RAD;
                i += 14;
                eph.iode = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 8);
                i += 8;
                toc = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 16) * 16.0;
                i += 16;
                eph.f1 = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 16) * DefineConstants.P2_43;
                i += 16;
                eph.f2 = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 8) * DefineConstants.P2_55;
                i += 8;
                eph.crs = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 16) * DefineConstants.P2_5;
                i += 16;
                eph.deln = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 16) * DefineConstants.P2_43 * DefineConstants.SC2RAD;
                i += 16;
                eph.cuc = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 16) * 1.862645149230957E-0x9;
                i += 16;
                eph.e = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 32) * DefineConstants.P2_33;
                i += 32;
                eph.cus = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 16);
                i += 16;
                sqrtA = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 32) * 1.907348632812500E-0x6;
                i += 32;
                eph.toes = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 16);
                i += 16;
                eph.OMG0 = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 32) * DefineConstants.P2_31 * DefineConstants.SC2RAD;
                i += 32;
                eph.cic = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 16) * 1.862645149230957E-0x9;
                i += 16;
                eph.i0 = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 32) * DefineConstants.P2_31 * DefineConstants.SC2RAD;
                i += 32;
                eph.cis = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 16) * 1.862645149230957E-0x9;
                i += 16;
                eph.omg = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 32) * DefineConstants.P2_31 * DefineConstants.SC2RAD;
                i += 32;
                eph.crc = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 16) * DefineConstants.P2_5;
                i += 16;
                eph.OMGd = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 24) * DefineConstants.P2_43 * DefineConstants.SC2RAD;
                i += 24;
                eph.M0 = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 32) * DefineConstants.P2_31 * DefineConstants.SC2RAD;
                i += 32;
                eph.iodc = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 10);
                i += 10;
                eph.f0 = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 22) * DefineConstants.P2_31;
                i += 22;
                prn = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 5);
                i += 5 + 3;
                eph.tgd[0] = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 8) * DefineConstants.P2_31;
                i += 8;
                eph.code = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 2);
                i += 2;
                eph.sva = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 4);
                i += 4;
                eph.svh = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 6);
                i += 6;
                eph.flag = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 1);
            }
            else
            {
                GlobalMembersRtkcmn.trace(2, "rtcm2 17 length error: len=%d\n", rtcm.len);
                return -1;
            }
            if (prn == 0)
            {
                prn = 32;
            }
            sat = GlobalMembersRtkcmn.satno(DefineConstants.SYS_GPS, prn);
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
            rtcm.nav.eph[sat - 1] = eph;
            rtcm.ephsat = sat;
            return 2;
        }
        /* decode type 18: rtk uncorrected carrier-phase -----------------------------*/
        internal static int decode_type18(rtcm_t rtcm)
        {
            gtime_t time = new gtime_t();
            double usec;
            double cp;
            double tt;
            int i = 48;
            int index;
            int freq;
            int sync = 1;
            int code;
            int sys;
            int prn;
            int sat;
            int loss;

            GlobalMembersRtkcmn.trace(4, "decode_type18: len=%d\n", rtcm.len);

            if (i + 24 <= rtcm.len * 8)
            {
                freq = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 2);
                i += 2 + 2;
                usec = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 20);
                i += 20;
            }
            else
            {
                GlobalMembersRtkcmn.trace(2, "rtcm2 18 length error: len=%d\n", rtcm.len);
                return -1;
            }
            if ((freq & 0x1) != 0)
            {
                GlobalMembersRtkcmn.trace(2, "rtcm2 18 not supported frequency: freq=%d\n", freq);
                return -1;
            }
            freq >>= 1;

            while (i + 48 <= rtcm.len * 8 && rtcm.obs.n < DefineConstants.MAXOBS)
            {
                sync = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 1);
                i += 1;
                code = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 1);
                i += 1;
                sys = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 1);
                i += 1;
                prn = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 5);
                i += 5 + 3;
                loss = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 5);
                i += 5;
                cp = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 32);
                i += 32;
                if (prn == 0)
                {
                    prn = 32;
                }
                if ((sat = GlobalMembersRtkcmn.satno(sys != 0 ? DefineConstants.SYS_GLO : DefineConstants.SYS_GPS, prn)) == 0)
                {
                    GlobalMembersRtkcmn.trace(2, "rtcm2 18 satellite number error: sys=%d prn=%d\n", sys, prn);
                    continue;
                }
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                //ORIGINAL LINE: time=timeadd(rtcm->time,usec *1E-6);
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                time.CopyFrom(GlobalMembersRtkcmn.timeadd(new gtime_t(rtcm.time), usec * 1E-6));
                if (sys != 0) // convert glonass time to gpst
                {
                    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                    //ORIGINAL LINE: time=utc2gpst(time);
                    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                    time.CopyFrom(GlobalMembersRtkcmn.utc2gpst(new gtime_t(time)));
                }

                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: tt=timediff(rtcm->obs.data[0].time,time);
                tt = GlobalMembersRtkcmn.timediff(rtcm.obs.data[0].time, new gtime_t(time));
                if (rtcm.obsflag != 0 || Math.Abs(tt) > 1E-9)
                {
                    rtcm.obs.n = rtcm.obsflag = 0;
                }
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: if ((index=obsindex(&rtcm->obs,time,sat))>=0)
                if ((index = GlobalMembersNovatel.obsindex(rtcm.obs, new gtime_t(time), sat)) >= 0)
                {
                    rtcm.obs.data[index].L[freq] = -cp / 256.0;
                    rtcm.obs.data[index].LLI[freq] = rtcm.loss[sat - 1, freq] != loss;
                    rtcm.obs.data[index].code[freq] = freq == 0 ? (code != 0 ? DefineConstants.CODE_L1P : DefineConstants.CODE_L1C) : (code != 0 ? DefineConstants.CODE_L2P : DefineConstants.CODE_L2C);
                    rtcm.loss[sat - 1, freq] = loss;
                }
            }
            rtcm.obsflag = sync == 0;
            return sync != 0 ? 0 : 1;
        }
        /* decode type 19: rtk uncorrected pseudorange -------------------------------*/
        internal static int decode_type19(rtcm_t rtcm)
        {
            gtime_t time = new gtime_t();
            double usec;
            double pr;
            double tt;
            int i = 48;
            int index;
            int freq;
            int sync = 1;
            int code;
            int sys;
            int prn;
            int sat;

            GlobalMembersRtkcmn.trace(4, "decode_type19: len=%d\n", rtcm.len);

            if (i + 24 <= rtcm.len * 8)
            {
                freq = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 2);
                i += 2 + 2;
                usec = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 20);
                i += 20;
            }
            else
            {
                GlobalMembersRtkcmn.trace(2, "rtcm2 19 length error: len=%d\n", rtcm.len);
                return -1;
            }
            if ((freq & 0x1) != 0)
            {
                GlobalMembersRtkcmn.trace(2, "rtcm2 19 not supported frequency: freq=%d\n", freq);
                return -1;
            }
            freq >>= 1;

            while (i + 48 <= rtcm.len * 8 && rtcm.obs.n < DefineConstants.MAXOBS)
            {
                sync = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 1);
                i += 1;
                code = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 1);
                i += 1;
                sys = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 1);
                i += 1;
                prn = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 5);
                i += 5 + 8;
                pr = GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 32);
                i += 32;
                if (prn == 0)
                {
                    prn = 32;
                }
                if ((sat = GlobalMembersRtkcmn.satno(sys != 0 ? DefineConstants.SYS_GLO : DefineConstants.SYS_GPS, prn)) == 0)
                {
                    GlobalMembersRtkcmn.trace(2, "rtcm2 19 satellite number error: sys=%d prn=%d\n", sys, prn);
                    continue;
                }
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                //ORIGINAL LINE: time=timeadd(rtcm->time,usec *1E-6);
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                time.CopyFrom(GlobalMembersRtkcmn.timeadd(new gtime_t(rtcm.time), usec * 1E-6));
                if (sys != 0) // convert glonass time to gpst
                {
                    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                    //ORIGINAL LINE: time=utc2gpst(time);
                    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                    time.CopyFrom(GlobalMembersRtkcmn.utc2gpst(new gtime_t(time)));
                }

                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: tt=timediff(rtcm->obs.data[0].time,time);
                tt = GlobalMembersRtkcmn.timediff(rtcm.obs.data[0].time, new gtime_t(time));
                if (rtcm.obsflag != 0 || Math.Abs(tt) > 1E-9)
                {
                    rtcm.obs.n = rtcm.obsflag = 0;
                }
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: if ((index=obsindex(&rtcm->obs,time,sat))>=0)
                if ((index = GlobalMembersNovatel.obsindex(rtcm.obs, new gtime_t(time), sat)) >= 0)
                {
                    rtcm.obs.data[index].P[freq] = pr * 0.02;
                    rtcm.obs.data[index].code[freq] = freq == 0 ? (code != 0 ? DefineConstants.CODE_L1P : DefineConstants.CODE_L1C) : (code != 0 ? DefineConstants.CODE_L2P : DefineConstants.CODE_L2C);
                }
            }
            rtcm.obsflag = sync == 0;
            return sync != 0 ? 0 : 1;
        }
        /* decode type 22: extended reference station parameter ----------------------*/
        internal static int decode_type22(rtcm_t rtcm)
        {
            double[,] del = { { 0, null, null } };
            double hgt = 0.0;
            int i = 48;
            int j;
            int noh;

            GlobalMembersRtkcmn.trace(4, "decode_type22: len=%d\n", rtcm.len);

            if (i + 24 <= rtcm.len * 8)
            {
                del[0, 0] = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 8) / 25600.0;
                i += 8;
                del[0, 1] = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 8) / 25600.0;
                i += 8;
                del[0, 2] = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 8) / 25600.0;
                i += 8;
            }
            else
            {
                GlobalMembersRtkcmn.trace(2, "rtcm2 22 length error: len=%d\n", rtcm.len);
                return -1;
            }
            if (i + 24 <= rtcm.len * 8)
            {
                i += 5;
                noh = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 1);
                i += 1;
                hgt = noh != 0 ? 0.0 : GlobalMembersRtkcmn.getbitu(rtcm.buff, i, 18) / 25600.0;
                i += 18;
            }
            if (i + 24 <= rtcm.len * 8)
            {
                del[1, 0] = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 8) / 1600.0;
                i += 8;
                del[1, 1] = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 8) / 1600.0;
                i += 8;
                del[1, 2] = GlobalMembersRtkcmn.getbits(rtcm.buff, i, 8) / 1600.0;
            }
            rtcm.sta.deltype = 1; // xyz
            for (j = 0; j < 3; j++)
            {
                rtcm.sta.del[j] = del[0, j];
            }
            rtcm.sta.hgt = hgt;
            return 5;
        }
        /* decode type 23: antenna type definition record ----------------------------*/
        internal static int decode_type23(rtcm_t rtcm)
        {
            return 0;
        }
        /* decode type 24: antenna reference point (arp) -----------------------------*/
        internal static int decode_type24(rtcm_t rtcm)
        {
            return 0;
        }
        /* decode type 31: differential glonass correction ---------------------------*/
        internal static int decode_type31(rtcm_t rtcm)
        {
            return 0;
        }
        /* decode type 32: differential glonass reference station parameters ---------*/
        internal static int decode_type32(rtcm_t rtcm)
        {
            return 0;
        }
        /* decode type 34: glonass partial differential correction set ---------------*/
        internal static int decode_type34(rtcm_t rtcm)
        {
            return 0;
        }
        /* decode type 36: glonass special message -----------------------------------*/
        internal static int decode_type36(rtcm_t rtcm)
        {
            return 0;
        }
        /* decode type 37: gnss system time offset -----------------------------------*/
        internal static int decode_type37(rtcm_t rtcm)
        {
            return 0;
        }
        /* decode type 59: proprietary message ---------------------------------------*/
        internal static int decode_type59(rtcm_t rtcm)
        {
            return 0;
        }
        /* decode rtcm ver.2 message -------------------------------------------------*/
        public static int decode_rtcm2(rtcm_t rtcm)
        {
            double zcnt;
            int staid;
            int seqno;
            int stah;
            int ret = 0;
            int type = GlobalMembersRtkcmn.getbitu(rtcm.buff, 8, 6);

            GlobalMembersRtkcmn.trace(3, "decode_rtcm2: type=%2d len=%3d\n", type, rtcm.len);

            if ((zcnt = GlobalMembersRtkcmn.getbitu(rtcm.buff, 24, 13) * 0.6) >= 3600.0)
            {
                GlobalMembersRtkcmn.trace(2, "rtcm2 modified z-count error: zcnt=%.1f\n", zcnt);
                return -1;
            }
            GlobalMembersRtcm2.adjhour(rtcm, zcnt);
            staid = GlobalMembersRtkcmn.getbitu(rtcm.buff, 14, 10);
            seqno = GlobalMembersRtkcmn.getbitu(rtcm.buff, 37, 3);
            stah = GlobalMembersRtkcmn.getbitu(rtcm.buff, 45, 3);
            if (seqno - rtcm.seqno != 1 && seqno - rtcm.seqno != -7)
            {
                GlobalMembersRtkcmn.trace(2, "rtcm2 message outage: seqno=%d->%d\n", rtcm.seqno, seqno);
            }
            rtcm.seqno = seqno;
            rtcm.stah = stah;

            if (rtcm.outtype != 0)
            {
                rtcm.msgtype = string.Format("RTCM {0,2:D} ({1,4:D}) zcnt={2,7:f1} staid={3,3:D} seqno={4:D}", type, rtcm.len, zcnt, staid, seqno);
            }
            if (type == 3 || type == 22 || type == 23 || type == 24)
            {
                if (rtcm.staid != 0 && staid != rtcm.staid)
                {
                    GlobalMembersRtkcmn.trace(2, "rtcm2 station id changed: %d->%d\n", rtcm.staid, staid);
                }
                rtcm.staid = staid;
            }
            if (rtcm.staid != 0 && staid != rtcm.staid)
            {
                GlobalMembersRtkcmn.trace(2, "rtcm2 station id invalid: %d %d\n", staid, rtcm.staid);
                return -1;
            }
            switch (type)
            {
                case 1:
                    ret = GlobalMembersRtcm2.decode_type1(rtcm);
                    break;
                case 3:
                    ret = GlobalMembersRtcm2.decode_type3(rtcm);
                    break;
                case 9:
                    ret = GlobalMembersRtcm2.decode_type1(rtcm);
                    break;
                case 14:
                    ret = GlobalMembersRtcm2.decode_type14(rtcm);
                    break;
                case 16:
                    ret = GlobalMembersRtcm2.decode_type16(rtcm);
                    break;
                case 17:
                    ret = GlobalMembersRtcm2.decode_type17(rtcm);
                    break;
                case 18:
                    ret = GlobalMembersRtcm2.decode_type18(rtcm);
                    break;
                case 19:
                    ret = GlobalMembersRtcm2.decode_type19(rtcm);
                    break;
                case 22:
                    ret = GlobalMembersRtcm2.decode_type22(rtcm);
                    break;
                case 23: // not supported
                    ret = GlobalMembersRtcm2.decode_type23(rtcm);
                    break;
                case 24: // not supported
                    ret = GlobalMembersRtcm2.decode_type24(rtcm);
                    break;
                case 31: // not supported
                    ret = GlobalMembersRtcm2.decode_type31(rtcm);
                    break;
                case 32: // not supported
                    ret = GlobalMembersRtcm2.decode_type32(rtcm);
                    break;
                case 34: // not supported
                    ret = GlobalMembersRtcm2.decode_type34(rtcm);
                    break;
                case 36: // not supported
                    ret = GlobalMembersRtcm2.decode_type36(rtcm);
                    break;
                case 37: // not supported
                    ret = GlobalMembersRtcm2.decode_type37(rtcm);
                    break;
                case 59: // not supported
                    ret = GlobalMembersRtcm2.decode_type59(rtcm);
                    break;
            }
            if (ret >= 0)
            {
                if (1 <= type && type <= 99)
                {
                    rtcm.nmsg2[type]++;
                }
                else
                {
                    rtcm.nmsg2[0]++;
                }
            }
            return ret;
        }
    }
}
