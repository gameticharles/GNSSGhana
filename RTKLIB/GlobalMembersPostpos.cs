using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ghGPS.Classes
{
    public static class GlobalMembersPostpos
    {
        /*------------------------------------------------------------------------------
        * postpos.c : post-processing positioning
        *
        *          Copyright (C) 2007-2014 by T.TAKASU, All rights reserved.
        *
        * version : $Revision: 1.1 $ $Date: 2008/07/17 21:48:06 $
        * history : 2007/05/08  1.0  new
        *           2008/06/16  1.1  support binary inputs
        *           2009/01/02  1.2  support new rtk positioing api
        *           2009/09/03  1.3  fix bug on combined mode of moving-baseline
        *           2009/12/04  1.4  fix bug on obs data buffer overflow
        *           2010/07/26  1.5  support ppp-kinematic and ppp-static
        *                            support multiple sessions
        *                            support sbas positioning
        *                            changed api:
        *                                postpos()
        *                            deleted api:
        *                                postposopt()
        *           2010/08/16  1.6  fix bug sbas message synchronization (2.4.0_p4)
        *           2010/12/09  1.7  support qzss lex and ssr corrections
        *           2011/02/07  1.8  fix bug on sbas navigation data conflict
        *           2011/03/22  1.9  add function reading g_tec file
        *           2011/08/20  1.10 fix bug on freez if solstatic=single and combined
        *           2011/09/15  1.11 add function reading stec file
        *           2012/02/01  1.12 support keyword expansion of rtcm ssr corrections
        *           2013/03/11  1.13 add function reading otl and erp data
        *           2014/06/29  1.14 fix problem on overflow of # of satellites
        *           2015/03/23  1.15 fix bug on ant type replacement by rinex header
        *                            fix bug on combined filter for moving-base mode
        *-----------------------------------------------------------------------------*/


        internal const string rcsid = "$Id: postpos.c,v 1.1 2008/07/17 21:48:06 ttaka Exp $";



        /* constants/global variables ------------------------------------------------*/

        internal static pcvs_t pcvss = new pcvs_t(); // receiver antenna parameters
        internal static pcvs_t pcvsr = new pcvs_t(); // satellite antenna parameters
        internal static obs_t obss = new obs_t(); // observation data
        internal static nav_t navs = new nav_t(); // navigation data
        internal static sbs_t sbss = new sbs_t(); // sbas messages
        internal static lex_t lexs = new lex_t(); // lex messages
        internal static sta_t[] stas = Arrays.InitializeWithDefaultInstances<sta_t>(DefineConstants.MAXRCV); // station infomation
        internal static int nepoch = 0; // number of observation epochs
        internal static int iobsu = 0; // current rover observation data index
        internal static int iobsr = 0; // current reference observation data index
        internal static int isbs = 0; // current sbas message index
        internal static int ilex = 0; // current lex message index
        internal static int revs = 0; // analysis direction (0:forward,1:backward)
        internal static int aborts = 0; // abort status
        internal static sol_t solf; // forward solutions
        internal static sol_t solb; // backward solutions
                                    //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
                                    //ORIGINAL LINE: static double *rbf;
        internal static double rbf; // forward base positions
                                    //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
                                    //ORIGINAL LINE: static double *rbb;
        internal static double rbb; // backward base positions
        internal static int isolf = 0; // current forward solutions index
        internal static int isolb = 0; // current backward solutions index
        internal static string proc_rov = ""; // rover for current processing
        internal static string proc_base = ""; // base station for current processing
        internal static string rtcm_file = ""; // rtcm data file
        internal static string rtcm_path = ""; // rtcm data path
        internal static rtcm_t rtcm = new rtcm_t(); // rtcm control struct
        internal static FILE fp_rtcm = null; // rtcm data file pointer

        /* show message and check break ----------------------------------------------*/
        internal static int checkbrk(string format, params object[] LegacyParamArray)
        {
            //	va_list arg;
            string buff = new string(new char[1024]);
            //C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on this variable, so pointers on this variable are left unchanged:
            sbyte* p = buff;
            if (format == 0)
            {
                return GlobalMembersRtkcmn.showmsg("");
            }
            int ParamCount = -1;
            //	va_start(arg,format);
            p += vsprintf(p, format, arg);
            //	va_end(arg);
            if (*proc_rov && *proc_base)
            {
                p = string.Format(" ({0}-{1})", proc_rov, proc_base);
            }
            else if (*proc_rov)
            {
                p = string.Format(" ({0})", proc_rov);
            }
            else if (*proc_base)
            {
                p = string.Format(" ({0})", proc_base);
            }
            return GlobalMembersRtkcmn.showmsg(buff);
        }
        /* output reference position -------------------------------------------------*/
        internal static void outrpos(FILE fp, double[] r, solopt_t opt)
        {
            double[] pos = new double[3];
            double[] dms1 = new double[3];
            double[] dms2 = new double[3];
            string sep = opt.sep;

            GlobalMembersRtkcmn.trace(3, "outrpos :\n");

            if (opt.posf == DefineConstants.SOLF_LLH || opt.posf == DefineConstants.SOLF_ENU)
            {
                GlobalMembersRtkcmn.ecef2pos(r, pos);
                if (opt.degf != 0)
                {
                    GlobalMembersRtkcmn.deg2dms(pos[0] * 180.0 / DefineConstants.PI, dms1);
                    GlobalMembersRtkcmn.deg2dms(pos[1] * 180.0 / DefineConstants.PI, dms2);
                    fprintf(fp, "%3.0f%s%02.0f%s%08.5f%s%4.0f%s%02.0f%s%08.5f%s%10.4f", dms1[0], sep, dms1[1], sep, dms1[2], sep, dms2[0], sep, dms2[1], sep, dms2[2], sep, pos[2]);
                }
                else
                {
                    fprintf(fp, "%13.9f%s%14.9f%s%10.4f", pos[0] * 180.0 / DefineConstants.PI, sep, pos[1] * 180.0 / DefineConstants.PI, sep, pos[2]);
                }
            }
            else if (opt.posf == DefineConstants.SOLF_XYZ)
            {
                fprintf(fp, "%14.4f%s%14.4f%s%14.4f", r[0], sep, r[1], sep, r[2]);
            }
        }
        /* output header -------------------------------------------------------------*/
        internal static void outheader(FILE fp, string[] file, int n, prcopt_t popt, solopt_t sopt)
        {
            string[] s1 = { "GPST", "UTC", "JST" };
            gtime_t ts = new gtime_t();
            gtime_t te = new gtime_t();
            double t1;
            double t2;
            int i;
            int j;
            int w1;
            int w2;
            string s2 = new string(new char[32]);
            string s3 = new string(new char[32]);

            GlobalMembersRtkcmn.trace(3, "outheader: n=%d\n", n);

            if (sopt.posf == DefineConstants.SOLF_NMEA)
                return;

            if (sopt.outhead != 0)
            {
                if (!*sopt.prog)
                {
                    fprintf(fp, "%s program   : RTKLIB ver.%s\n", DefineConstants.COMMENTH, DefineConstants.VER_RTKLIB);
                }
                else
                {
                    fprintf(fp, "%s program   : %s\n", DefineConstants.COMMENTH, sopt.prog);
                }
                for (i = 0; i < n; i++)
                {
                    fprintf(fp, "%s inp file  : %s\n", DefineConstants.COMMENTH, file[i]);
                }
                for (i = 0; i < obss.n; i++)
                {
                    if (obss.data[i].rcv == 1)
                        break;
                }
                for (j = obss.n - 1; j >= 0; j--)
                {
                    if (obss.data[j].rcv == 1)
                        break;
                }
                if (j < i)
                {
                    fprintf(fp, "\n%s no rover obs data\n", DefineConstants.COMMENTH);
                    return;
                }
                ts = obss.data[i].time;
                te = obss.data[j].time;
                t1 = GlobalMembersRtkcmn.time2gpst(new gtime_t(ts), ref w1);
                t2 = GlobalMembersRtkcmn.time2gpst(new gtime_t(te), ref w2);
                if (sopt.times >= 1)
                {
                    ts.CopyFrom(GlobalMembersRtkcmn.gpst2utc(new gtime_t(ts)));
                }
                if (sopt.times >= 1)
                {
                    te.CopyFrom(GlobalMembersRtkcmn.gpst2utc(new gtime_t(te)));
                }
                if (sopt.times == 2)
                {
                    ts.CopyFrom(GlobalMembersRtkcmn.timeadd(new gtime_t(ts), 9 * 3600.0));
                }
                if (sopt.times == 2)
                {
                    te.CopyFrom(GlobalMembersRtkcmn.timeadd(new gtime_t(te), 9 * 3600.0));
                }
       
                GlobalMembersRtkcmn.time2str(new gtime_t(ts), ref s2, 1);
                
                GlobalMembersRtkcmn.time2str(new gtime_t(te), ref s3, 1);
                fprintf(fp, "%s obs start : %s %s (week%04d %8.1fs)\n", DefineConstants.COMMENTH, s2, s1[sopt.times], w1, t1);
                fprintf(fp, "%s obs end   : %s %s (week%04d %8.1fs)\n", DefineConstants.COMMENTH, s3, s1[sopt.times], w2, t2);
            }
            if (sopt.outopt != 0)
            {
                GlobalMembersSolution.outprcopt(fp, popt);
            }
            if (DefineConstants.PMODE_DGPS <= popt.mode && popt.mode <= DefineConstants.PMODE_FIXED && popt.mode != DefineConstants.PMODE_MOVEB)
            {
                fprintf(fp, "%s ref pos   :", DefineConstants.COMMENTH);
                GlobalMembersPostpos.outrpos(fp, popt.rb, sopt);
                fprintf(fp, "\n");
            }
            if (sopt.outhead != 0 || sopt.outopt != 0)
            {
                fprintf(fp, "%s\n", DefineConstants.COMMENTH);
            }

            GlobalMembersSolution.outsolhead(fp, sopt);
        }
        /* search next observation data index ----------------------------------------*/
        internal static int nextobsf(obs_t obs, ref int i, int rcv)
        {
            double tt;
            int n;

            for (; i < obs.n; i++)
            {
                if (obs.data[i].rcv == rcv)
                    break;
            }
            for (n = 0; i + n < obs.n; n++)
            {
                tt = GlobalMembersRtkcmn.timediff(obs.data[i + n].time, obs.data[i].time);
                if (obs.data[i + n].rcv != rcv || tt > DefineConstants.DTTOL)
                    break;
            }
            return n;
        }
        internal static int nextobsb(obs_t obs, ref int i, int rcv)
        {
            double tt;
            int n;

            for (; i >= 0; i--)
            {
                if (obs.data[i].rcv == rcv)
                    break;
            }
            for (n = 0; i - n >= 0; n++)
            {
                tt = GlobalMembersRtkcmn.timediff(obs.data[i - n].time, obs.data[i].time);
                if (obs.data[i - n].rcv != rcv || tt < -DefineConstants.DTTOL)
                    break;
            }
            return n;
        }
        /* input obs data, navigation messages and sbas correction -------------------*/
        internal static int inputobs(obsd_t[] obs, int solq, prcopt_t popt)
        {
            gtime_t time = new gtime_t();
            string path = new string(new char[1024]);
            int i;
            int nu;
            int nr;
            int n = 0;

            GlobalMembersRtkcmn.trace(3, "infunc  : revs=%d iobsu=%d iobsr=%d isbs=%d\n", revs, iobsu, iobsr, isbs);

            if (0 <= iobsu && iobsu < obss.n)
            {
                GlobalMembersRtkcmn.settime((time = obss.data[iobsu].time));
                if (GlobalMembersPostpos.checkbrk("processing : %s Q=%d", GlobalMembersRtkcmn.time_str(new gtime_t(time), 0), solq))
                {
                    aborts = 1;
                    GlobalMembersRtkcmn.showmsg("aborted");
                    return -1;
                }
            }
            if (revs == 0) // input forward data
            {
                if ((nu = GlobalMembersPostpos.nextobsf(obss, ref iobsu, 1)) <= 0)
                {
                    return -1;
                }
                if (popt.intpref != 0)
                {
                    for (; (nr = GlobalMembersPostpos.nextobsf(obss, ref iobsr, 2)) > 0; iobsr += nr)
                    {
                        if (GlobalMembersRtkcmn.timediff(obss.data[iobsr].time, obss.data[iobsu].time) > -DefineConstants.DTTOL)
                            break;
                    }
                }
                else
                {
                    for (i = iobsr; (nr = GlobalMembersPostpos.nextobsf(obss, ref i, 2)) > 0; iobsr = i, i += nr)
                    {
                        if (GlobalMembersRtkcmn.timediff(obss.data[i].time, obss.data[iobsu].time) > DefineConstants.DTTOL)
                            break;
                    }
                }
                nr = GlobalMembersPostpos.nextobsf(obss, ref iobsr, 2);
                for (i = 0; i < nu && n < DefineConstants.MAXOBS * 2; i++)
                {
                    obs[n++] = obss.data[iobsu + i];
                }
                for (i = 0; i < nr && n < DefineConstants.MAXOBS * 2; i++)
                {
                    obs[n++] = obss.data[iobsr + i];
                }
                iobsu += nu;

                /* update sbas corrections */
                while (isbs < sbss.n)
                {
                    time.CopyFrom(GlobalMembersRtkcmn.gpst2time(sbss.msgs[isbs].week, sbss.msgs[isbs].tow));

                    if (GlobalMembersRtkcmn.getbitu(sbss.msgs[isbs].msg, 8, 6) != 9) // except for geo nav
                    {
                        GlobalMembersSbas.sbsupdatecorr(sbss.msgs + isbs, navs);
                    }
                    if (GlobalMembersRtkcmn.timediff(new gtime_t(time), obs[0].time) > -1.0 - DefineConstants.DTTOL)
                        break;
                    isbs++;
                }
                /* update lex corrections */
                while (ilex < lexs.n)
                {
                    if (GlobalMembersQzslex.lexupdatecorr(lexs.msgs + ilex, navs, time) != 0)
                    {
                        if (GlobalMembersRtkcmn.timediff(new gtime_t(time), obs[0].time) > -1.0 - DefineConstants.DTTOL)
                            break;
                    }
                    ilex++;
                }
                /* update rtcm corrections */
                if (*rtcm_file)
                {

                    /* open or swap rtcm file */
                    GlobalMembersRtkcmn.reppath(rtcm_file, ref path, obs[0].time, "", "");

                    if (string.Compare(path, rtcm_path))
                    {
                        rtcm_path = path;

                        if (fp_rtcm != null)
                        {
                            fclose(fp_rtcm);
                        }
                        fp_rtcm = fopen(path, "rb");
                        if (fp_rtcm != null)
                        {
                            rtcm.time = obs[0].time;
                            GlobalMembersRtcm.input_rtcm3f(rtcm, fp_rtcm);
                            GlobalMembersRtkcmn.trace(2, "rtcm file open: %s\n", path);
                        }
                    }
                    if (fp_rtcm != null)
                    {
                        while (GlobalMembersRtkcmn.timediff(new gtime_t(rtcm.time), obs[0].time) < 0.0)
                        {
                            if (GlobalMembersRtcm.input_rtcm3f(rtcm, fp_rtcm) < -1)
                                break;
                        }
                        for (i = 0; i < DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1; i++)
                        {
                            navs.ssr[i] = rtcm.ssr[i];
                        }
                    }
                }
            }
            else // input backward data
            {
                if ((nu = GlobalMembersPostpos.nextobsb(obss, ref iobsu, 1)) <= 0)
                {
                    return -1;
                }
                if (popt.intpref != 0)
                {
                    for (; (nr = GlobalMembersPostpos.nextobsb(obss, ref iobsr, 2)) > 0; iobsr -= nr)
                    {
                        if (GlobalMembersRtkcmn.timediff(obss.data[iobsr].time, obss.data[iobsu].time) < DefineConstants.DTTOL)
                            break;
                    }
                }
                else
                {
                    for (i = iobsr; (nr = GlobalMembersPostpos.nextobsb(obss, ref i, 2)) > 0; iobsr = i, i -= nr)
                    {
                        if (GlobalMembersRtkcmn.timediff(obss.data[i].time, obss.data[iobsu].time) < -DefineConstants.DTTOL)
                            break;
                    }
                }
                nr = GlobalMembersPostpos.nextobsb(obss, ref iobsr, 2);
                for (i = 0; i < nu && n < DefineConstants.MAXOBS * 2; i++)
                {
                    obs[n++] = obss.data[iobsu - nu + 1 + i];
                }
                for (i = 0; i < nr && n < DefineConstants.MAXOBS * 2; i++)
                {
                    obs[n++] = obss.data[iobsr - nr + 1 + i];
                }
                iobsu -= nu;

                /* update sbas corrections */
                while (isbs >= 0)
                {
                    time.CopyFrom(GlobalMembersRtkcmn.gpst2time(sbss.msgs[isbs].week, sbss.msgs[isbs].tow));

                    if (GlobalMembersRtkcmn.getbitu(sbss.msgs[isbs].msg, 8, 6) != 9) // except for geo nav
                    {
                        GlobalMembersSbas.sbsupdatecorr(sbss.msgs + isbs, navs);
                    }
                    if (GlobalMembersRtkcmn.timediff(new gtime_t(time), obs[0].time) < 1.0 + DefineConstants.DTTOL)
                        break;
                    isbs--;
                }
                /* update lex corrections */
                while (ilex >= 0)
                {
                    if (GlobalMembersQzslex.lexupdatecorr(lexs.msgs + ilex, navs, time) != 0)
                    {
                        if (GlobalMembersRtkcmn.timediff(new gtime_t(time), obs[0].time) < 1.0 + DefineConstants.DTTOL)
                            break;
                    }
                    ilex--;
                }
            }
            return n;
        }
        /* process positioning -------------------------------------------------------*/
        internal static void procpos(FILE fp, prcopt_t popt, solopt_t sopt, int mode)
        {
            gtime_t time = new gtime_t();
            sol_t sol = new sol_t({ 0 });
            rtk_t rtk = new rtk_t();
            obsd_t[] obs = Arrays.InitializeWithDefaultInstances<obsd_t>(DefineConstants.MAXOBS * 2); // for rover and base
            double[] rb = { 0, null, null };
            int i;
            int nobs;
            int n;
            int solstatic;
            int[] pri = { 0, 1, 2, 3, 4, 5, 1, 6 };

            GlobalMembersRtkcmn.trace(3, "procpos : mode=%d\n", mode);

            solstatic = sopt.solstatic && (popt.mode == DefineConstants.PMODE_STATIC || popt.mode == DefineConstants.PMODE_PPP_STATIC);

            GlobalMembersRtkpos.rtkinit(rtk, popt);
            rtcm_path[0] = '\0';

            while ((nobs = GlobalMembersPostpos.inputobs(new obsd_t(obs), rtk.sol.stat, popt)) >= 0)
            {

                /* exclude satellites */
                for (i = n = 0; i < nobs; i++)
                {
                    if ((GlobalMembersRtkcmn.satsys(obs[i].sat, null) & popt.navsys) && popt.exsats[obs[i].sat - 1] != 1)
                    {
                        obs[n++] = obs[i];
                    }
                }
                if (n <= 0)
                    continue;

                if (GlobalMembersRtkpos.rtkpos(rtk, new obsd_t(obs), n, navs) == 0)
                    continue;

                if (mode == 0) // forward/backward
                {
                    if (solstatic == 0)
                    {
                        GlobalMembersSolution.outsol(fp, rtk.sol, rtk.rb, sopt);
                    }
                    else if (time.time == 0 || pri[rtk.sol.stat] <= pri[sol.stat])
                    {
                        sol.CopyFrom(rtk.sol);
                        for (i = 0; i < 3; i++)
                        {
                            rb[i] = rtk.rb[i];
                        }
                        if (time.time == 0 || GlobalMembersRtkcmn.timediff(new gtime_t(rtk.sol.time), new gtime_t(time)) < 0.0)
                        {
                            time.CopyFrom(rtk.sol.time);
                        }
                    }
                }
                else if (revs == 0) // combined-forward
                {
                    if (isolf >= nepoch)
                        return;
                    solf[isolf] = rtk.sol;
                    for (i = 0; i < 3; i++)
                    {
                        rbf[i + isolf * 3] = rtk.rb[i];
                    }
                    isolf++;
                }
                else // combined-backward
                {
                    if (isolb >= nepoch)
                        return;
                    solb[isolb] = rtk.sol;
                    for (i = 0; i < 3; i++)
                    {
                        rbb[i + isolb * 3] = rtk.rb[i];
                    }
                    isolb++;
                }
            }
            if (mode == 0 && solstatic != 0 && time.time != 0.0)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                //ORIGINAL LINE: sol.time=time;
                sol.time.CopyFrom(time);
                GlobalMembersSolution.outsol(fp, sol, rb, sopt);
            }
            GlobalMembersRtkpos.rtkfree(rtk);
        }
        /* validation of combined solutions ------------------------------------------*/
        internal static int valcomb(sol_t solf, sol_t solb)
        {
            double[] dr = new double[3];
            double[] @var = new double[3];
            int i;
            string tstr = new string(new char[32]);

            GlobalMembersRtkcmn.trace(3, "valcomb :\n");

            /* compare forward and backward solution */
            for (i = 0; i < 3; i++)
            {
                dr[i] = solf.rr[i] - solb.rr[i];
                @var[i] = solf.qr[i] + solb.qr[i];
            }
            for (i = 0; i < 3; i++)
            {
                if (dr[i] * dr[i] <= 16.0 * @var[i]) // ok if in 4-sigma
                    continue;

                GlobalMembersRtkcmn.time2str(new gtime_t(solf.time), ref tstr, 2);
                GlobalMembersRtkcmn.trace(2, "degrade fix to float: %s dr=%.3f %.3f %.3f std=%.3f %.3f %.3f\n", tstr.Substring(11), dr[0], dr[1], dr[2], ((@var[0]) <= 0.0 ? 0.0 : Math.Sqrt(@var[0])), ((@var[1]) <= 0.0 ? 0.0 : Math.Sqrt(@var[1])), ((@var[2]) <= 0.0 ? 0.0 : Math.Sqrt(@var[2])));
                return 0;
            }
            return 1;
        }
        /* combine forward/backward solutions and output results ---------------------*/
        internal static void combres(FILE fp, prcopt_t popt, solopt_t sopt)
        {
            gtime_t time = new gtime_t();
            sol_t sols = new sol_t({ 0 });
            sol_t sol = new sol_t({ 0 });
            double tt;
            double[] Qf = new double[9];
            double[] Qb = new double[9];
            double[] Qs = new double[9];
            double[] rbs = { 0, null, null };
            double[] rb = { 0, null, null };
            double[] rr_f = new double[3];
            double[] rr_b = new double[3];
            double[] rr_s = new double[3];
            int i;
            int j;
            int k;
            int solstatic;
            int[] pri = { 0, 1, 2, 3, 4, 5, 1, 6 };

            GlobalMembersRtkcmn.trace(3, "combres : isolf=%d isolb=%d\n", isolf, isolb);

            solstatic = sopt.solstatic && (popt.mode == DefineConstants.PMODE_STATIC || popt.mode == DefineConstants.PMODE_PPP_STATIC);

            for (i = 0, j = isolb - 1; i < isolf && j >= 0; i++, j--)
            {

                if ((tt = GlobalMembersRtkcmn.timediff(solf[i].time, solb[j].time)) < -DefineConstants.DTTOL)
                {
                    sols = solf[i];
                    for (k = 0; k < 3; k++)
                    {
                        rbs[k] = rbf[k + i * 3];
                    }
                    j++;
                }
                else if (tt > DefineConstants.DTTOL)
                {
                    sols = solb[j];
                    for (k = 0; k < 3; k++)
                    {
                        rbs[k] = rbb[k + j * 3];
                    }
                    i--;
                }
                else if (solf[i].stat < solb[j].stat)
                {
                    sols = solf[i];
                    for (k = 0; k < 3; k++)
                    {
                        rbs[k] = rbf[k + i * 3];
                    }
                }
                else if (solf[i].stat > solb[j].stat)
                {
                    sols = solb[j];
                    for (k = 0; k < 3; k++)
                    {
                        rbs[k] = rbb[k + j * 3];
                    }
                }
                else
                {
                    sols = solf[i];
                    sols.time.CopyFrom(GlobalMembersRtkcmn.timeadd(new gtime_t(sols.time), -tt / 2.0));

                    if ((popt.mode == DefineConstants.PMODE_KINEMA || popt.mode == DefineConstants.PMODE_MOVEB) && sols.stat == DefineConstants.SOLQ_FIX)
                    {

                        /* degrade fix to float if validation failed */
                        if (GlobalMembersPostpos.valcomb(solf + i, solb + j) == 0)
                        {
                            sols.stat = DefineConstants.SOLQ_FLOAT;
                        }
                    }
                    for (k = 0; k < 3; k++)
                    {
                        Qf[k + k * 3] = solf[i].qr[k];
                        Qb[k + k * 3] = solb[j].qr[k];
                    }
                    Qf[1] = Qf[3] = solf[i].qr[3];
                    Qf[5] = Qf[7] = solf[i].qr[4];
                    Qf[2] = Qf[6] = solf[i].qr[5];
                    Qb[1] = Qb[3] = solb[j].qr[3];
                    Qb[5] = Qb[7] = solb[j].qr[4];
                    Qb[2] = Qb[6] = solb[j].qr[5];

                    if (popt.mode == DefineConstants.PMODE_MOVEB)
                    {
                        for (k = 0; k < 3; k++)
                        {
                            rr_f[k] = solf[i].rr[k] - rbf[k + i * 3];
                        }
                        for (k = 0; k < 3; k++)
                        {
                            rr_b[k] = solb[j].rr[k] - rbb[k + j * 3];
                        }
                        if (GlobalMembersRtkcmn.smoother(rr_f, Qf, rr_b, Qb, 3, ref rr_s, Qs) != 0)
                            continue;
                        for (k = 0; k < 3; k++)
                        {
                            sols.rr[k] = rbs[k] + rr_s[k];
                        }
                    }
                    else
                    {
                        if (GlobalMembersRtkcmn.smoother(solf[i].rr, Qf, solb[j].rr, Qb, 3, ref sols.rr, Qs) != 0)
                            continue;
                    }
                    sols.qr[0] = (float)Qs[0];
                    sols.qr[1] = (float)Qs[4];
                    sols.qr[2] = (float)Qs[8];
                    sols.qr[3] = (float)Qs[1];
                    sols.qr[4] = (float)Qs[5];
                    sols.qr[5] = (float)Qs[2];
                }
                if (solstatic == 0)
                {
                    GlobalMembersSolution.outsol(fp, sols, rbs, sopt);
                }
                else if (time.time == 0 || pri[sols.stat] <= pri[sol.stat])
                {
                    sol.CopyFrom(sols);
                    for (k = 0; k < 3; k++)
                    {
                        rb[k] = rbs[k];
                    }
                    if (time.time == 0 || GlobalMembersRtkcmn.timediff(new gtime_t(sols.time), new gtime_t(time)) < 0.0)
                    {
                        time.CopyFrom(sols.time);
                    }
                }
            }
            if (solstatic != 0 && time.time != 0.0)
            {
                sol.time.CopyFrom(time);
                GlobalMembersSolution.outsol(fp, sol, rb, sopt);
            }
        }
        /* read prec ephemeris, sbas data, lex data, tec grid and open rtcm ----------*/
        internal static void readpreceph(string[] infile, int n, prcopt_t prcopt, nav_t nav, sbs_t sbs, lex_t lex)
        {
            seph_t seph0 = new seph_t();
            int i;
            string ext;

            GlobalMembersRtkcmn.trace(3, "readpreceph: n=%d\n", n);

            nav.ne = nav.nemax = 0;
            nav.nc = nav.ncmax = 0;
            sbs.n = sbs.nmax = 0;
            lex.n = lex.nmax = 0;

            /* read precise ephemeris files */
            for (i = 0; i < n; i++)
            {
                if (StringFunctions.StrStr(infile[i], "%r") || StringFunctions.StrStr(infile[i], "%b"))
                    continue;
                GlobalMembersPreceph.readsp3(infile[i], nav, 0);
            }
            /* read precise clock files */
            for (i = 0; i < n; i++)
            {
                if (StringFunctions.StrStr(infile[i], "%r") || StringFunctions.StrStr(infile[i], "%b"))
                    continue;
                GlobalMembersRinex.readrnxc(infile[i], nav);
            }
            /* read sbas message files */
            for (i = 0; i < n; i++)
            {
                if (StringFunctions.StrStr(infile[i], "%r") || StringFunctions.StrStr(infile[i], "%b"))
                    continue;
                GlobalMembersSbas.sbsreadmsg(infile[i], prcopt.sbassatsel, sbs);
            }
            /* read lex message files */
            for (i = 0; i < n; i++)
            {
                if (StringFunctions.StrStr(infile[i], "%r") || StringFunctions.StrStr(infile[i], "%b"))
                    continue;
                GlobalMembersQzslex.lexreadmsg(infile[i], 0, lex);
            }
            /* allocate sbas ephemeris */
            nav.ns = nav.nsmax = DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 * 2;
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'malloc' has no equivalent in C#:
            if ((nav.seph = (seph_t)malloc(sizeof(seph_t) * nav.ns)) == null)
            {
                GlobalMembersRtkcmn.showmsg("error : sbas ephem memory allocation");
                GlobalMembersRtkcmn.trace(1, "error : sbas ephem memory allocation");
                return;
            }
            for (i = 0; i < nav.ns; i++)
            {
                nav.seph[i] = seph0;
            }

            /* set rtcm file and initialize rtcm struct */
            rtcm_file[0] = rtcm_path[0] = '\0';
            fp_rtcm = null;

            for (i = 0; i < n; i++)
            {
                if ((ext = StringFunctions.StrRChr(infile[i], '.')) != 0 && (!string.Compare(ext, ".rtcm3") || !string.Compare(ext, ".RTCM3")))
                {
                    rtcm_file = infile[i];
                    GlobalMembersRtcm.init_rtcm(rtcm);
                    break;
                }
            }
        }
        /* free prec ephemeris and sbas data -----------------------------------------*/
        internal static void freepreceph(nav_t nav, sbs_t sbs, lex_t lex)
        {
            int i;

            GlobalMembersRtkcmn.trace(3, "freepreceph:\n");

            
            free(nav.peph);
            nav.peph = null;
            nav.ne = nav.nemax = 0;
            
            free(nav.pclk);
            nav.pclk = null;
            nav.nc = nav.ncmax = 0;
            
            free(nav.seph);
            nav.seph = null;
            nav.ns = nav.nsmax = 0;
           
            free(sbs.msgs);
            sbs.msgs = null;
            sbs.n = sbs.nmax = 0;
           
            free(lex.msgs);
            lex.msgs = null;
            lex.n = lex.nmax = 0;
            for (i = 0; i < nav.nt; i++)
            {
                
                free(nav.tec[i].data);
                
                free(nav.tec[i].rms);
            }
            
            free(nav.tec);
            nav.tec = null;
            nav.nt = nav.ntmax = 0;

#if EXTSTEC
		stec_free(nav);
#endif

            if (fp_rtcm != null)
            {
                fclose(fp_rtcm);
            }
            GlobalMembersRtcm.free_rtcm(rtcm);
        }
        /* read obs and nav data -----------------------------------------------------*/
        internal static int readobsnav(gtime_t ts, gtime_t te, double ti, string[] infile, int[] index, int n, prcopt_t prcopt, obs_t obs, nav_t nav, sta_t sta)
        {
            int i;
            int j;
            int ind = 0;
            int nobs = 0;
            int rcv = 1;

            GlobalMembersRtkcmn.trace(3, "readobsnav: ts=%s n=%d\n", GlobalMembersRtkcmn.time_str(new gtime_t(ts), 0), n);

            obs.data = null;
            obs.n = obs.nmax = 0;
            nav.eph = null;
            nav.n = nav.nmax = 0;
            nav.geph = null;
            nav.ng = nav.ngmax = 0;
            nav.seph = null;
            nav.ns = nav.nsmax = 0;
            nepoch = 0;

            for (i = 0; i < n; i++)
            {
                if (GlobalMembersPostpos.checkbrk(""))
                {
                    return 0;
                }

                if (index[i] != ind)
                {
                    if (obs.n > nobs)
                    {
                        rcv++;
                    }
                    ind = index[i];
                    nobs = obs.n;
                }
                /* read rinex obs and nav file */
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: if (readrnxt(infile[i],rcv,ts,te,ti,prcopt->rnxopt[rcv<=1?0:1],obs,nav, rcv<=2?sta+rcv-1:null)<0)
                if (GlobalMembersRinex.readrnxt(infile[i], rcv, new gtime_t(ts), new gtime_t(te), ti, prcopt.rnxopt[rcv <= 1 ? 0 : 1], obs, nav, rcv <= 2 ? sta + rcv - 1 : null) < 0)
                {
                    GlobalMembersPostpos.checkbrk("error : insufficient memory");
                    GlobalMembersRtkcmn.trace(1, "insufficient memory\n");
                    return 0;
                }
            }
            if (obs.n <= 0)
            {
                GlobalMembersPostpos.checkbrk("error : no obs data");
                GlobalMembersRtkcmn.trace(1, "no obs data\n");
                return 0;
            }
            if (nav.n <= 0 && nav.ng <= 0 && nav.ns <= 0)
            {
                GlobalMembersPostpos.checkbrk("error : no nav data");
                GlobalMembersRtkcmn.trace(1, "no nav data\n");
                return 0;
            }
            /* sort observation data */
            nepoch = GlobalMembersRtkcmn.sortobs(obs);

            /* delete duplicated ephemeris */
            GlobalMembersRtkcmn.uniqnav(nav);

            /* set time span for progress display */
            if (ts.time == 0 || te.time == 0)
            {
                for (i = 0; i < obs.n; i++)
                {
                    if (obs.data[i].rcv == 1)
                        break;
                }
                for (j = obs.n - 1; j >= 0; j--)
                {
                    if (obs.data[j].rcv == 1)
                        break;
                }
                if (i < j)
                {
                    if (ts.time == 0)
                    {
                        ts = obs.data[i].time;
                    }
                    if (te.time == 0)
                    {
                        te = obs.data[j].time;
                    }
                    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                    //ORIGINAL LINE: settspan(ts,te);
                    GlobalMembersRtkcmn.settspan(new gtime_t(ts), new gtime_t(te));
                }
            }
            return 1;
        }
        /* free obs and nav data -----------------------------------------------------*/
        internal static void freeobsnav(obs_t obs, nav_t nav)
        {
            GlobalMembersRtkcmn.trace(3, "freeobsnav:\n");

            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(obs.data);
            obs.data = null;
            obs.n = obs.nmax = 0;
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(nav.eph);
            nav.eph = null;
            nav.n = nav.nmax = 0;
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(nav.geph);
            nav.geph = null;
            nav.ng = nav.ngmax = 0;
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(nav.seph);
            nav.seph = null;
            nav.ns = nav.nsmax = 0;
        }
        /* average of single position ------------------------------------------------*/
        //C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on the parameter 'obs', so pointers on this parameter are left unchanged:
        internal static int avepos(double[] ra, int rcv, obs_t* obs, nav_t nav, prcopt_t opt)
        {
            obsd_t[] data = Arrays.InitializeWithDefaultInstances<obsd_t>(DefineConstants.MAXOBS);
            gtime_t ts = new gtime_t();
            sol_t sol = new sol_t({ 0 });
            int i;
            int j;
            int n = 0;
            int m;
            int iobs;
            string msg = new string(new char[128]);

            GlobalMembersRtkcmn.trace(3, "avepos: rcv=%d obs.n=%d\n", rcv, obs.n);

            for (i = 0; i < 3; i++)
            {
                ra[i] = 0.0;
            }

            for (iobs = 0; (m = GlobalMembersPostpos.nextobsf(obs, ref iobs, rcv)) > 0; iobs += m)
            {

                for (i = j = 0; i < m && i < DefineConstants.MAXOBS; i++)
                {
                    data[j] = obs.data[iobs + i];
                    if ((GlobalMembersRtkcmn.satsys(data[j].sat, null) & opt.navsys) && opt.exsats[data[j].sat - 1] != 1)
                    {
                        j++;
                    }
                }
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: if (j<=0||!screent(data[0].time,ts,ts,1.0))
                if (j <= 0 || GlobalMembersRtkcmn.screent(new gtime_t(data[0].time), new gtime_t(ts), new gtime_t(ts), 1.0) == 0) // only 1 hz
                    continue;

                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: if (!pntpos(data,j,nav,opt,&sol,null,null,msg))
                if (GlobalMembersPntpos.pntpos(new obsd_t(data), j, nav, opt, sol, null, null, ref msg) == 0)
                    continue;

                for (i = 0; i < 3; i++)
                {
                    ra[i] += sol.rr[i];
                }
                n++;
            }
            if (n <= 0)
            {
                GlobalMembersRtkcmn.trace(1, "no average of base station position\n");
                return 0;
            }
            for (i = 0; i < 3; i++)
            {
                ra[i] /= n;
            }
            return 1;
        }
        /* station position from file ------------------------------------------------*/
        internal static int getstapos(string file, ref string name, ref double r)
        {
            FILE fp;
            string buff = new string(new char[256]);
            string sname = new string(new char[256]);
            string p;
            string q;
            double[] pos = new double[3];

            GlobalMembersRtkcmn.trace(3, "getstapos: file=%s name=%s\n", file, name);

            if ((fp = fopen(file, "r")) == null)
            {
                GlobalMembersRtkcmn.trace(1, "station position file open error: %s\n", file);
                return 0;
            }
            while (fgets(buff, sizeof(sbyte), fp))
            {
                if ((p = StringFunctions.StrChr(buff, '%')) != 0)
                {
                    p = (sbyte)'\0';
                }

                if (sscanf(buff, "%lf %lf %lf %s", pos, pos + 1, pos + 2, sname) < 4)
                    continue;

                for (p = sname, q = name; p && q != 0; p++, q++)
                {
                    if (char.ToUpper((int)p) != char.ToUpper((int)q))
                        break;
                }
                if (p == 0)
                {
                    pos[0] *= DefineConstants.PI / 180.0;
                    pos[1] *= DefineConstants.PI / 180.0;
                    GlobalMembersRtkcmn.pos2ecef(pos, r);
                    fclose(fp);
                    return 1;
                }
            }
            fclose(fp);
            GlobalMembersRtkcmn.trace(1, "no station position: %s %s\n", name, file);
            return 0;
        }
        /* antenna phase center position ---------------------------------------------*/
        internal static int antpos(prcopt_t opt, int rcvno, obs_t obs, nav_t nav, sta_t sta, string posfile)
        {
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: double *rr=rcvno==1?opt->ru:opt->rb,del[3],pos[3],dr[3]={0};
            double rr = rcvno == 1 ? opt.ru : opt.rb;
            double[] del = new double[3];
            double[] pos = new double[3];
            double[] dr = { 0, null, null };
            int i;
            int postype = rcvno == 1 ? opt.rovpos : opt.refpos;
            string name;

            GlobalMembersRtkcmn.trace(3, "antpos  : rcvno=%d\n", rcvno);

            if (postype == 1) // average of single position
            {
                if (GlobalMembersPostpos.avepos(rr, rcvno, obs, nav, opt) == 0)
                {
                    GlobalMembersRtkcmn.showmsg("error : station pos computation");
                    return 0;
                }
            }
            else if (postype == 2) // read from position file
            {
                name = stas[rcvno == 1 ? 0 : 1].name;
                if (GlobalMembersPostpos.getstapos(posfile, ref name, ref rr) == 0)
                {
                    GlobalMembersRtkcmn.showmsg("error : no position of %s in %s", name, posfile);
                    return 0;
                }
            }
            else if (postype == 3) // get from rinex header
            {
                if (GlobalMembersRtkcmn.norm(stas[rcvno == 1 ? 0 : 1].pos, 3) <= 0.0)
                {
                    GlobalMembersRtkcmn.showmsg("error : no position in rinex header");
                    GlobalMembersRtkcmn.trace(1, "no position position in rinex header\n");
                    return 0;
                }
                /* antenna delta */
                if (stas[rcvno == 1 ? 0 : 1].deltype == 0) // enu
                {
                    for (i = 0; i < 3; i++)
                    {
                        del[i] = stas[rcvno == 1 ? 0 : 1].del[i];
                    }
                    del[2] += stas[rcvno == 1 ? 0 : 1].hgt;
                    GlobalMembersRtkcmn.ecef2pos(stas[rcvno == 1 ? 0 : 1].pos, pos);
                    GlobalMembersRtkcmn.enu2ecef(pos, del, ref dr);
                }
                else // xyz
                {
                    for (i = 0; i < 3; i++)
                    {
                        dr[i] = stas[rcvno == 1 ? 0 : 1].del[i];
                    }
                }
                for (i = 0; i < 3; i++)
                {
                    rr[i] = stas[rcvno == 1 ? 0 : 1].pos[i] + dr[i];
                }
            }
            return 1;
        }
        /* open procssing session ----------------------------------------------------*/
        internal static int openses(prcopt_t popt, solopt_t sopt, filopt_t fopt, nav_t nav, pcvs_t pcvs, pcvs_t pcvr)
        {
            string ext;

            GlobalMembersRtkcmn.trace(3, "openses :\n");

            /* read satellite antenna parameters */
            if (*fopt.satantp && (GlobalMembersRtkcmn.readpcv(fopt.satantp, pcvs)) == 0)
            {
                GlobalMembersRtkcmn.showmsg("error : no sat ant pcv in %s", fopt.satantp);
                GlobalMembersRtkcmn.trace(1, "sat antenna pcv read error: %s\n", fopt.satantp);
                return 0;
            }
            /* read receiver antenna parameters */
            if (*fopt.rcvantp && (GlobalMembersRtkcmn.readpcv(fopt.rcvantp, pcvr)) == 0)
            {
                GlobalMembersRtkcmn.showmsg("error : no rec ant pcv in %s", fopt.rcvantp);
                GlobalMembersRtkcmn.trace(1, "rec antenna pcv read error: %s\n", fopt.rcvantp);
                return 0;
            }
            /* read dcb parameters */
            if (*fopt.dcb)
            {
                GlobalMembersPreceph.readdcb(fopt.dcb, nav);
            }
            /* read ionosphere data file */
            if (*fopt.iono && (ext = StringFunctions.StrRChr(fopt.iono, '.')) != 0)
            {
                if (ext.Length == 4 && (ext[3] == 'i' || ext[3] == 'I'))
                {
                    GlobalMembersIonex.readtec(fopt.iono, nav, 0);
                }
#if EXTSTEC
			else if (!string.Compare(ext,".stec") || !string.Compare(ext,".STEC"))
			{
				stec_read(fopt.iono,nav);
			}
#endif
            }
            /* open geoid data */
            if (sopt.geoid > 0 && *fopt.geoid)
            {
                if (GlobalMembersGeoid.opengeoid(sopt.geoid, fopt.geoid) == 0)
                {
                    GlobalMembersRtkcmn.showmsg("error : no geoid data %s", fopt.geoid);
                    GlobalMembersRtkcmn.trace(2, "no geoid data %s\n", fopt.geoid);
                }
            }
            /* read erp data */
            if (*fopt.eop)
            {
                if (GlobalMembersRtkcmn.readerp(fopt.eop, nav.erp) == 0)
                {
                    GlobalMembersRtkcmn.showmsg("error : no erp data %s", fopt.eop);
                    GlobalMembersRtkcmn.trace(2, "no erp data %s\n", fopt.eop);
                }
            }
            return 1;
        }
        /* close procssing session ---------------------------------------------------*/
        internal static void closeses(nav_t nav, pcvs_t pcvs, pcvs_t pcvr)
        {
            GlobalMembersRtkcmn.trace(3, "closeses:\n");

            /* free antenna parameters */
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(pcvs.pcv);
            pcvs.pcv = null;
            pcvs.n = pcvs.nmax = 0;
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(pcvr.pcv);
            pcvr.pcv = null;
            pcvr.n = pcvr.nmax = 0;

            /* close geoid data */
            GlobalMembersGeoid.closegeoid();

            /* free erp data */
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(nav.erp.data);
            nav.erp.data = null;
            nav.erp.n = nav.erp.nmax = 0;

            /* close solution statistics and debug trace */
            GlobalMembersRtkpos.rtkclosestat();
            GlobalMembersRtkcmn.traceclose();
        }
        /* set antenna parameters ----------------------------------------------------*/
        internal static void setpcv(gtime_t time, prcopt_t popt, nav_t nav, pcvs_t[] pcvs, pcvs_t[] pcvr, sta_t[] sta)
        {
            pcv_t pcv;
            double[] pos = new double[3];
            double[] del = new double[3];
            int i;
            int j;
            int mode = DefineConstants.PMODE_DGPS <= popt.mode && popt.mode <= DefineConstants.PMODE_FIXED;
            string id = new string(new char[64]);

            /* set satellite antenna parameters */
            for (i = 0; i < DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1; i++)
            {
                if (!(GlobalMembersRtkcmn.satsys(i + 1, null) & popt.navsys))
                    continue;
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: if (!(pcv=searchpcv(i+1,"",time,pcvs)))
                if ((pcv = GlobalMembersRtkcmn.searchpcv(i + 1, "", new gtime_t(time), new pcvs_t(pcvs))) == null)
                {
                    GlobalMembersRtkcmn.satno2id(i + 1, ref id);
                    GlobalMembersRtkcmn.trace(2, "no satellite antenna pcv: %s\n", id);
                    continue;
                }
                nav.pcvs[i] = pcv;
            }
            for (i = 0; i < (mode != 0 ? 2 : 1); i++)
            {
                if (!string.Compare(popt.anttype[i], "*")) // set by station parameters
                {
                    popt.anttype[i] = sta[i].antdes;
                    if (sta[i].deltype == 1) // xyz
                    {
                        if (GlobalMembersRtkcmn.norm(sta[i].pos, 3) > 0.0)
                        {
                            GlobalMembersRtkcmn.ecef2pos(sta[i].pos, pos);
                            GlobalMembersRtkcmn.ecef2enu(pos, sta[i].del, ref del);
                            for (j = 0; j < 3; j++)
                            {
                                popt.antdel[i, j] = del[j];
                            }
                        }
                    }
                    else // enu
                    {
                        for (j = 0; j < 3; j++)
                        {
                            popt.antdel[i, j] = stas[i].del[j];
                        }
                    }
                }
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: if (!(pcv=searchpcv(0,popt->anttype[i],time,pcvr)))
                if ((pcv = GlobalMembersRtkcmn.searchpcv(0, popt.anttype[i], new gtime_t(time), new pcvs_t(pcvr))) == null)
                {
                    GlobalMembersRtkcmn.trace(2, "no receiver antenna pcv: %s\n", popt.anttype[i]);
                    popt.anttype[i] = (sbyte)'\0';
                    continue;
                }
                popt.anttype[i] = pcv.type;
                popt.pcvr[i] = pcv;
            }
        }
        /* read ocean tide loading parameters ----------------------------------------*/
        internal static void readotl(prcopt_t popt, string file, sta_t[] sta)
        {
            int i;
            int mode = DefineConstants.PMODE_DGPS <= popt.mode && popt.mode <= DefineConstants.PMODE_FIXED;

            for (i = 0; i < (mode != 0 ? 2 : 1); i++)
            {
                GlobalMembersRtkcmn.readblq(file, sta[i].name, ref popt.odisp[i]);
            }
        }
        /* write header to output file -----------------------------------------------*/
        internal static int outhead(string outfile, string[] infile, int n, prcopt_t popt, solopt_t sopt)
        {
            FILE fp = stdout;

            GlobalMembersRtkcmn.trace(3, "outhead: outfile=%s n=%d\n", outfile, n);

            if (outfile != 0)
            {
                GlobalMembersRtkcmn.createdir(outfile);

                if ((fp = fopen(outfile, "w")) == null)
                {
                    GlobalMembersRtkcmn.showmsg("error : open output file %s", outfile);
                    return 0;
                }
            }
            /* output header */
            GlobalMembersPostpos.outheader(fp, infile, n, popt, sopt);

            if (outfile != 0)
            {
                fclose(fp);
            }

            return 1;
        }
        /* open output file for append -----------------------------------------------*/
        internal static FILE openfile(string outfile)
        {
            GlobalMembersRtkcmn.trace(3, "openfile: outfile=%s\n", outfile);

            return outfile == 0 ? stdout : fopen(outfile, "a");
        }
        /* execute processing session ------------------------------------------------*/
        internal static int execses(gtime_t ts, gtime_t te, double ti, prcopt_t popt, solopt_t sopt, filopt_t fopt, int flag, string[] infile, int index, int n, ref string outfile)
        {
            FILE fp;
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: prcopt_t popt_=*popt;
            prcopt_t popt_ = new prcopt_t(popt);
            string tracefile = new string(new char[1024]);
            string statfile = new string(new char[1024]);

            GlobalMembersRtkcmn.trace(3, "execses : n=%d outfile=%s\n", n, outfile);

            /* open debug trace */
            if (flag != 0 && sopt.trace > 0)
            {
                if (outfile != 0)
                {
                    tracefile = outfile;
                    tracefile += ".trace";
                }
                else
                {
                    tracefile = fopt.trace;
                }
                GlobalMembersRtkcmn.traceclose();
                GlobalMembersRtkcmn.traceopen(tracefile);
                GlobalMembersRtkcmn.tracelevel(sopt.trace);
            }
            /* read obs and nav data */
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: if (!readobsnav(ts,te,ti,infile,index,n,&popt_,&obss,&navs,stas))
            if (GlobalMembersPostpos.readobsnav(new gtime_t(ts), new gtime_t(te), ti, infile, index, n, popt_, obss, navs, new sta_t(stas)) == 0)
            {
                return 0;
            }

            /* set antenna paramters */
            if (popt_.mode != DefineConstants.PMODE_SINGLE)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: setpcv(obss.n>0?obss.data[0].time:timeget(),&popt_,&navs,&pcvss,&pcvsr, stas);
                GlobalMembersPostpos.setpcv(obss.n > 0 ? obss.data[0].time : GlobalMembersRtkcmn.timeget(), popt_, navs, pcvss, pcvsr, new sta_t(stas));
            }
            /* read ocean tide loading parameters */
            if (popt_.mode > DefineConstants.PMODE_SINGLE && fopt.blq != 0)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: readotl(&popt_,fopt->blq,stas);
                GlobalMembersPostpos.readotl(popt_, fopt.blq, new sta_t(stas));
            }
            /* rover/reference fixed position */
            if (popt_.mode == DefineConstants.PMODE_FIXED)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: if (!antpos(&popt_,1,&obss,&navs,stas,fopt->stapos))
                if (GlobalMembersPostpos.antpos(popt_, 1, obss, navs, new sta_t(stas), fopt.stapos) == 0)
                {
                    GlobalMembersPostpos.freeobsnav(obss, navs);
                    return 0;
                }
            }
            else if (DefineConstants.PMODE_DGPS <= popt_.mode && popt_.mode <= DefineConstants.PMODE_STATIC)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: if (!antpos(&popt_,2,&obss,&navs,stas,fopt->stapos))
                if (GlobalMembersPostpos.antpos(popt_, 2, obss, navs, new sta_t(stas), fopt.stapos) == 0)
                {
                    GlobalMembersPostpos.freeobsnav(obss, navs);
                    return 0;
                }
            }
            /* open solution statistics */
            if (flag != 0 && sopt.sstat > 0)
            {
                statfile = outfile;
                statfile += ".stat";
                GlobalMembersRtkpos.rtkclosestat();
                GlobalMembersRtkpos.rtkopenstat(statfile, sopt.sstat);
            }
            /* write header to output file */
            if (flag != 0 && GlobalMembersPostpos.outhead(outfile, infile, n, popt_, sopt) == 0)
            {
                GlobalMembersPostpos.freeobsnav(obss, navs);
                return 0;
            }
            iobsu = iobsr = isbs = ilex = revs = aborts = 0;

            if (popt_.mode == DefineConstants.PMODE_SINGLE || popt_.soltype == 0)
            {
                if ((fp = GlobalMembersConvrnx.openfile(outfile)) != null)
                {
                    GlobalMembersPostpos.procpos(fp, popt_, sopt, 0); // forward
                    fclose(fp);
                }
            }
            else if (popt_.soltype == 1)
            {
                if ((fp = GlobalMembersConvrnx.openfile(outfile)) != null)
                {
                    revs = 1;
                    iobsu = iobsr = obss.n - 1;
                    isbs = sbss.n - 1;
                    ilex = lexs.n - 1;
                    GlobalMembersPostpos.procpos(fp, popt_, sopt, 0); // backward
                    fclose(fp);
                }
            }
            else // combined
            {
                //C++ TO C# CONVERTER TODO TASK: The memory management function 'malloc' has no equivalent in C#:
                solf = (sol_t)malloc(sizeof(sol_t) * nepoch);
                //C++ TO C# CONVERTER TODO TASK: The memory management function 'malloc' has no equivalent in C#:
                solb = (sol_t)malloc(sizeof(sol_t) * nepoch);
                //C++ TO C# CONVERTER TODO TASK: The memory management function 'malloc' has no equivalent in C#:
                rbf = (double)malloc(sizeof(double) * nepoch * 3);
                //C++ TO C# CONVERTER TODO TASK: The memory management function 'malloc' has no equivalent in C#:
                rbb = (double)malloc(sizeof(double) * nepoch * 3);

                if (solf != null && solb != null)
                {
                    isolf = isolb = 0;
                    GlobalMembersPostpos.procpos(null, popt_, sopt, 1); // forward
                    revs = 1;
                    iobsu = iobsr = obss.n - 1;
                    isbs = sbss.n - 1;
                    ilex = lexs.n - 1;
                    GlobalMembersPostpos.procpos(null, popt_, sopt, 1); // backward

                    /* combine forward/backward solutions */
                    if (aborts == 0 && (fp = GlobalMembersConvrnx.openfile(outfile)) != null)
                    {
                        GlobalMembersPostpos.combres(fp, popt_, sopt);
                        fclose(fp);
                    }
                }
                else
                {
                    GlobalMembersRtkcmn.showmsg("error : memory allocation");
                }
                //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
                free(solf);
                //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
                free(solb);
                //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
                free(rbf);
                //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
                free(rbb);
            }
            /* free obs and nav data */
            GlobalMembersPostpos.freeobsnav(obss, navs);

            return aborts != 0 ? 1 : 0;
        }
        /* execute processing session for each rover ---------------------------------*/
        internal static int execses_r(gtime_t ts, gtime_t te, double ti, prcopt_t popt, solopt_t sopt, filopt_t fopt, int flag, string[] infile, int index, int n, ref string outfile, string rov)
        {
            gtime_t t0 = new gtime_t();
            int i;
            int stat = 0;
            string[] ifile = new string[DefineConstants.MAXINFILE];
            string ofile = new string(new char[1024]);
            string rov_;
            string p;
            string q;
            string s = "";

            GlobalMembersRtkcmn.trace(3, "execses_r: n=%d outfile=%s\n", n, outfile);

            for (i = 0; i < n; i++)
            {
                if (StringFunctions.StrStr(infile[i], "%r"))
                    break;
            }

            if (i < n) // include rover keywords
            {
                //C++ TO C# CONVERTER TODO TASK: The memory management function 'malloc' has no equivalent in C#:
                if ((rov_ = (string)malloc(rov.Length + 1)) == 0)
                {
                    return 0;
                }
                rov_ = rov;

                for (i = 0; i < n; i++)
                {
                    //C++ TO C# CONVERTER TODO TASK: The memory management function 'malloc' has no equivalent in C#:
                    if (!(ifile[i] = (string)malloc(1024)))
                    {
                        //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
                        free(rov_);
                        for (; i >= 0; i--)
                        {
                            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
                            free(ifile[i]);
                        }
                        return 0;
                    }
                }
                for (p = rov_; ; p = q.Substring(1)) // for each rover
                {
                    if ((q = StringFunctions.StrChr(p, ' ')) != 0)
                    {
                        q = (sbyte)'\0';
                    }

                    if (p != 0)
                    {
                        proc_rov = p;
                        if (ts.time != null)
                        {
                            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                            //ORIGINAL LINE: time2str(ts,s,0);
                            GlobalMembersRtkcmn.time2str(new gtime_t(ts), ref s, 0);
                        }
                        else
                        {
                            *s = (sbyte)'\0';
                        }
                        if (GlobalMembersPostpos.checkbrk("reading    : %s", s) != 0)
                        {
                            stat = 1;
                            break;
                        }
                        for (i = 0; i < n; i++)
                        {
                            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                            //ORIGINAL LINE: reppath(infile[i],ifile[i],t0,p,"");
                            GlobalMembersRtkcmn.reppath(infile[i], ref ifile[i], new gtime_t(t0), p, "");
                        }
                        //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                        //ORIGINAL LINE: reppath(outfile,ofile,t0,p,"");
                        GlobalMembersRtkcmn.reppath(outfile, ref ofile, new gtime_t(t0), p, "");

                        /* execute processing session */
                        //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                        //ORIGINAL LINE: stat=execses(ts,te,ti,popt,sopt,fopt,flag,ifile,index,n,ofile);
                        stat = GlobalMembersPostpos.execses(new gtime_t(ts), new gtime_t(te), ti, popt, sopt, fopt, flag, ifile, index, n, ref ofile);
                    }
                    if (stat == 1 || q == 0)
                        break;
                }
                //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
                free(rov_);
                for (i = 0; i < n; i++)
                {
                    //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
                    free(ifile[i]);
                }
            }
            else
            {
                /* execute processing session */
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: stat=execses(ts,te,ti,popt,sopt,fopt,flag,infile,index,n,outfile);
                stat = GlobalMembersPostpos.execses(new gtime_t(ts), new gtime_t(te), ti, popt, sopt, fopt, flag, infile, index, n, ref outfile);
            }
            return stat;
        }
        /* execute processing session for each base station --------------------------*/
        internal static int execses_b(gtime_t ts, gtime_t te, double ti, prcopt_t popt, solopt_t sopt, filopt_t fopt, int flag, string[] infile, int index, int n, ref string outfile, string rov, string @base)
        {
            gtime_t t0 = new gtime_t();
            int i;
            int stat = 0;
            string[] ifile = new string[DefineConstants.MAXINFILE];
            string ofile = new string(new char[1024]);
            string base_;
            string p;
            string q;
            string s = new string(new char[64]);

            GlobalMembersRtkcmn.trace(3, "execses_b: n=%d outfile=%s\n", n, outfile);

            /* read prec ephemeris and sbas data */
            GlobalMembersPostpos.readpreceph(infile, n, popt, navs, sbss, lexs);

            for (i = 0; i < n; i++)
            {
                if (StringFunctions.StrStr(infile[i], "%b"))
                    break;
            }

            if (i < n) // include base station keywords
            {
                //C++ TO C# CONVERTER TODO TASK: The memory management function 'malloc' has no equivalent in C#:
                if ((base_ = (string)malloc(@base.Length + 1)) == 0)
                {
                    GlobalMembersPostpos.freepreceph(navs, sbss, lexs);
                    return 0;
                }
                base_ = @base;

                for (i = 0; i < n; i++)
                {
                    //C++ TO C# CONVERTER TODO TASK: The memory management function 'malloc' has no equivalent in C#:
                    if (!(ifile[i] = (string)malloc(1024)))
                    {
                        //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
                        free(base_);
                        for (; i >= 0; i--)
                        {
                            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
                            free(ifile[i]);
                        }
                        GlobalMembersPostpos.freepreceph(navs, sbss, lexs);
                        return 0;
                    }
                }
                for (p = base_; ; p = q.Substring(1)) // for each base station
                {
                    if ((q = StringFunctions.StrChr(p, ' ')) != 0)
                    {
                        q = (sbyte)'\0';
                    }

                    if (p != 0)
                    {
                        proc_base = p;
                        if (ts.time != null)
                        {
                            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                            //ORIGINAL LINE: time2str(ts,s,0);
                            GlobalMembersRtkcmn.time2str(new gtime_t(ts), ref s, 0);
                        }
                        else
                        {
                            *s = (sbyte)'\0';
                        }
                        if (GlobalMembersPostpos.checkbrk("reading    : %s", s) != 0)
                        {
                            stat = 1;
                            break;
                        }
                        for (i = 0; i < n; i++)
                        {
                            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                            //ORIGINAL LINE: reppath(infile[i],ifile[i],t0,"",p);
                            GlobalMembersRtkcmn.reppath(infile[i], ref ifile[i], new gtime_t(t0), "", p);
                        }
                        //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                        //ORIGINAL LINE: reppath(outfile,ofile,t0,"",p);
                        GlobalMembersRtkcmn.reppath(outfile, ref ofile, new gtime_t(t0), "", p);

                        //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                        //ORIGINAL LINE: stat=execses_r(ts,te,ti,popt,sopt,fopt,flag,ifile,index,n,ofile,rov);
                        stat = GlobalMembersPostpos.execses_r(new gtime_t(ts), new gtime_t(te), ti, popt, sopt, fopt, flag, ifile, index, n, ref ofile, rov);
                    }
                    if (stat == 1 || q == 0)
                        break;
                }
                //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
                free(base_);
                for (i = 0; i < n; i++)
                {
                    //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
                    free(ifile[i]);
                }
            }
            else
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: stat=execses_r(ts,te,ti,popt,sopt,fopt,flag,infile,index,n,outfile,rov);
                stat = GlobalMembersPostpos.execses_r(new gtime_t(ts), new gtime_t(te), ti, popt, sopt, fopt, flag, infile, index, n, ref outfile, rov);
            }
            /* free prec ephemeris and sbas data */
            GlobalMembersPostpos.freepreceph(navs, sbss, lexs);

            return stat;
        }
        /* post-processing positioning -------------------------------------------------
        * post-processing positioning
        * args   : gtime_t ts       I   processing start time (ts.time==0: no limit)
        *        : gtime_t te       I   processing end time   (te.time==0: no limit)
        *          double ti        I   processing interval  (s) (0:all)
        *          double tu        I   processing unit time (s) (0:all)
        *          prcopt_t *popt   I   processing options
        *          solopt_t *sopt   I   solution options
        *          filopt_t *fopt   I   file options
        *          char   **infile  I   input files (see below)
        *          int    n         I   number of input files
        *          char   *outfile  I   output file ("":stdout, see below)
        *          char   *rov      I   rover id list        (separated by " ")
        *          char   *base     I   base station id list (separated by " ")
        * return : status (0:ok,0>:error,1:aborted)
        * notes  : input files should contain observation data, navigation data, precise 
        *          ephemeris/clock (optional), sbas log file (optional), ssr message
        *          log file (optional) and tec grid file (optional). only the first 
        *          observation data file in the input files is recognized as the rover
        *          data.
        *
        *          the type of an input file is recognized by the file extention as ]
        *          follows:
        *              .sp3,.SP3,.eph*,.EPH*: precise ephemeris (sp3c)
        *              .sbs,.SBS,.ems,.EMS  : sbas message log files (rtklib or ems)
        *              .lex,.LEX            : qzss lex message log files
        *              .rtcm3,.RTCM3        : ssr message log files (rtcm3)
        *              .*i,.*I              : tec grid files (ionex)
        *              others               : rinex obs, nav, gnav, hnav, qnav or clock
        *
        *          inputs files can include wild-cards (*). if an file includes
        *          wild-cards, the wild-card expanded multiple files are used.
        *
        *          inputs files can include keywords. if an file includes keywords,
        *          the keywords are replaced by date, time, rover id and base station
        *          id and multiple session analyses run. refer reppath() for the
        *          keywords.
        *
        *          the output file can also include keywords. if the output file does
        *          not include keywords. the results of all multiple session analyses
        *          are output to a single output file.
        *
        *          ssr corrections are valid only for forward estimation.
        *-----------------------------------------------------------------------------*/
        public static int postpos(gtime_t ts, gtime_t te, double ti, double tu, prcopt_t popt, solopt_t sopt, filopt_t fopt, string[] infile, int n, ref string outfile, string rov, string @base)
        {
            gtime_t tts = new gtime_t();
            gtime_t tte = new gtime_t();
            gtime_t ttte = new gtime_t();
            double tunit;
            double tss;
            int i;
            int j;
            int k;
            int nf;
            int stat = 0;
            int week;
            int flag = 1;
            int[] index = new int[DefineConstants.MAXINFILE];
            string[] ifile = new string[DefineConstants.MAXINFILE];
            string ofile = new string(new char[1024]);
            string ext;

            GlobalMembersRtkcmn.trace(3, "postpos : ti=%.0f tu=%.0f n=%d outfile=%s\n", ti, tu, n, outfile);

            /* open processing session */
            if (GlobalMembersPostpos.openses(popt, sopt, fopt, navs, pcvss, pcvsr) == 0)
            {
                return -1;
            }

            if (ts.time != 0 && te.time != 0 && tu >= 0.0)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: if (timediff(te,ts)<0.0)
                if (GlobalMembersRtkcmn.timediff(new gtime_t(te), new gtime_t(ts)) < 0.0)
                {
                    GlobalMembersRtkcmn.showmsg("error : no period");
                    GlobalMembersPostpos.closeses(navs, pcvss, pcvsr);
                    return 0;
                }
                for (i = 0; i < DefineConstants.MAXINFILE; i++)
                {
                    //C++ TO C# CONVERTER TODO TASK: The memory management function 'malloc' has no equivalent in C#:
                    if (!(ifile[i] = (string)malloc(1024)))
                    {
                        for (; i >= 0; i--)
                        {
                            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
                            free(ifile[i]);
                        }
                        GlobalMembersPostpos.closeses(navs, pcvss, pcvsr);
                        return -1;
                    }
                }
                if (tu == 0.0 || tu > 86400.0 * DefineConstants.MAXPRCDAYS)
                {
                    tu = 86400.0 * DefineConstants.MAXPRCDAYS;
                }
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: settspan(ts,te);
                GlobalMembersRtkcmn.settspan(new gtime_t(ts), new gtime_t(te));
                tunit = tu < 86400.0 ? tu : 86400.0;
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: tss=tunit*(int)floor(time2gpst(ts,&week)/tunit);
                tss = tunit * (int)Math.Floor(GlobalMembersRtkcmn.time2gpst(new gtime_t(ts), ref week) / tunit);

                for (i = 0; ; i++) // for each periods
                {
                    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                    //ORIGINAL LINE: tts=gpst2time(week,tss+i *tu);
                    tts.CopyFrom(GlobalMembersRtkcmn.gpst2time(week, tss + i * tu));
                    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                    //ORIGINAL LINE: tte=timeadd(tts,tu-DefineConstants.DTTOL);
                    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                    tte.CopyFrom(GlobalMembersRtkcmn.timeadd(new gtime_t(tts), tu - DefineConstants.DTTOL));
                    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                    //ORIGINAL LINE: if (timediff(tts,te)>0.0)
                    if (GlobalMembersRtkcmn.timediff(new gtime_t(tts), new gtime_t(te)) > 0.0)
                        break;
                    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                    //ORIGINAL LINE: if (timediff(tts,ts)<0.0)
                    if (GlobalMembersRtkcmn.timediff(new gtime_t(tts), new gtime_t(ts)) < 0.0)
                    {
                        //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                        //ORIGINAL LINE: tts=ts;
                        tts.CopyFrom(ts);
                    }
                    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                    //ORIGINAL LINE: if (timediff(tte,te)>0.0)
                    if (GlobalMembersRtkcmn.timediff(new gtime_t(tte), new gtime_t(te)) > 0.0)
                    {
                        //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                        //ORIGINAL LINE: tte=te;
                        tte.CopyFrom(te);
                    }

                    proc_rov = "";
                    proc_base = "";
                    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                    //ORIGINAL LINE: if (checkbrk("reading    : %s",time_str(tts,0)))
                    if (GlobalMembersPostpos.checkbrk("reading    : %s", GlobalMembersRtkcmn.time_str(new gtime_t(tts), 0)) != 0)
                    {
                        stat = 1;
                        break;
                    }
                    for (j = k = nf = 0; j < n; j++)
                    {

                        ext = StringFunctions.StrRChr(infile[j], '.');

                        if (ext != 0 && (!string.Compare(ext, ".rtcm3") || !string.Compare(ext, ".RTCM3")))
                        {
                            ifile[nf++] = infile[j];
                        }
                        else
                        {
                            /* include next day precise ephemeris or rinex brdc nav */
                            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                            //ORIGINAL LINE: ttte=tte;
                            ttte.CopyFrom(tte);
                            if (ext != 0 && (!string.Compare(ext, ".sp3") || !string.Compare(ext, ".SP3") || !string.Compare(ext, ".eph") || !string.Compare(ext, ".EPH")))
                            {
                                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                                //ORIGINAL LINE: ttte=timeadd(ttte,3600.0);
                                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                                ttte.CopyFrom(GlobalMembersRtkcmn.timeadd(new gtime_t(ttte), 3600.0));
                            }
                            else if (StringFunctions.StrStr(infile[j], "brdc"))
                            {
                                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                                //ORIGINAL LINE: ttte=timeadd(ttte,7200.0);
                                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                                ttte.CopyFrom(GlobalMembersRtkcmn.timeadd(new gtime_t(ttte), 7200.0));
                            }
                            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                            //ORIGINAL LINE: nf+=reppaths(infile[j],ifile+nf,DefineConstants.MAXINFILE-nf,tts,ttte,"","");
                            nf += GlobalMembersRtkcmn.reppaths(infile[j], ifile + nf, DefineConstants.MAXINFILE - nf, new gtime_t(tts), new gtime_t(ttte), "", "");
                        }
                        while (k < nf)
                        {
                            index[k++] = j;
                        }

                        if (nf >= DefineConstants.MAXINFILE)
                        {
                            GlobalMembersRtkcmn.trace(2, "too many input files. trancated\n");
                            break;
                        }
                    }
                    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                    //ORIGINAL LINE: if (!reppath(outfile,ofile,tts,"","")&&i>0)
                    if (GlobalMembersRtkcmn.reppath(outfile, ref ofile, new gtime_t(tts), "", "") == 0 && i > 0)
                    {
                        flag = 0;
                    }

                    /* execute processing session */
                    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                    //ORIGINAL LINE: stat=execses_b(tts,tte,ti,popt,sopt,fopt,flag,ifile,index,nf,ofile, rov,base);
                    stat = GlobalMembersPostpos.execses_b(new gtime_t(tts), new gtime_t(tte), ti, popt, sopt, fopt, flag, ifile, index, nf, ref ofile, rov, @base);

                    if (stat == 1)
                        break;
                }
                for (i = 0; i < DefineConstants.MAXINFILE; i++)
                {
                    //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
                    free(ifile[i]);
                }
            }
            else if (ts.time != 0)
            {
                for (i = 0; i < n && i < DefineConstants.MAXINFILE; i++)
                {
                    //C++ TO C# CONVERTER TODO TASK: The memory management function 'malloc' has no equivalent in C#:
                    if (!(ifile[i] = (string)malloc(1024)))
                    {
                        for (; i >= 0; i--)
                        {
                            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
                            free(ifile[i]);
                        }
                        return -1;
                    }
                    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                    //ORIGINAL LINE: reppath(infile[i],ifile[i],ts,"","");
                    GlobalMembersRtkcmn.reppath(infile[i], ref ifile[i], new gtime_t(ts), "", "");
                    index[i] = i;
                }
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: reppath(outfile,ofile,ts,"","");
                GlobalMembersRtkcmn.reppath(outfile, ref ofile, new gtime_t(ts), "", "");

                /* execute processing session */
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: stat=execses_b(ts,te,ti,popt,sopt,fopt,1,ifile,index,n,ofile,rov, base);
                stat = GlobalMembersPostpos.execses_b(new gtime_t(ts), new gtime_t(te), ti, popt, sopt, fopt, 1, ifile, index, n, ref ofile, rov, @base);

                for (i = 0; i < n && i < DefineConstants.MAXINFILE; i++)
                {
                    //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
                    free(ifile[i]);
                }
            }
            else
            {
                for (i = 0; i < n; i++)
                {
                    index[i] = i;
                }

                /* execute processing session */
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: stat=execses_b(ts,te,ti,popt,sopt,fopt,1,infile,index,n,outfile,rov, base);
                stat = GlobalMembersPostpos.execses_b(new gtime_t(ts), new gtime_t(te), ti, popt, sopt, fopt, 1, infile, index, n, ref outfile, rov, @base);
            }
            /* close processing session */
            GlobalMembersPostpos.closeses(navs, pcvss, pcvsr);

            return stat;
        }
    }
