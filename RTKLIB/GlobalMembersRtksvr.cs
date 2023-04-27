using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ghGPS.Classes
{
    public static class GlobalMembersRtksvr
    {
        /*------------------------------------------------------------------------------
        * rtksvr.c : rtk server functions
        *
        *          Copyright (C) 2007-2013 by T.TAKASU, All rights reserved.
        *
        * options : -DWIN32    use WIN32 API
        *
        * version : $Revision:$ $Date:$
        * history : 2009/01/07  1.0  new
        *           2009/06/02  1.1  support glonass
        *           2010/07/25  1.2  support correction input/log stream
        *                            supoort online change of output/log streams
        *                            supoort monitor stream
        *                            added api:
        *                                rtksvropenstr(),rtksvrclosestr()
        *                            changed api:
        *                                rtksvrstart()
        *           2010/08/25  1.3  fix problem of ephemeris time inversion (2.4.0_p6)
        *           2010/09/08  1.4  fix problem of ephemeris and ssr squence upset
        *                            (2.4.0_p8)
        *           2011/01/10  1.5  change api: rtksvrstart(),rtksvrostat()
        *           2011/06/21  1.6  fix ephemeris handover problem
        *           2012/05/14  1.7  fix bugs
        *           2013/03/28  1.8  fix problem on lack of glonass freq number in raw
        *                            fix problem on ephemeris with inverted toe
        *                            add api rtksvrfree()
        *           2014/06/28  1.9  fix probram on ephemeris update of beidou
        *-----------------------------------------------------------------------------*/


        internal const string rcsid = "$Id:$";

        /* write solution header to output stream ------------------------------------*/
        internal static void writesolhead(stream_t stream, solopt_t solopt)
        {
            byte[] buff = new byte[1024];
            int n;

            n = GlobalMembersSolution.outsolheads(ref buff, solopt);
            strwrite(stream, buff, n);
        }
        /* save output buffer --------------------------------------------------------*/
        internal static void saveoutbuf(rtksvr_t svr, ref byte buff, int n, int index)
        {
            GlobalMembersRtksvr.rtksvrlock(svr);

            n = n < svr.buffsize - svr.nsb[index] != 0 ? n : svr.buffsize - svr.nsb[index];
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'memcpy' has no equivalent in C#:
            memcpy(svr.sbuf[index] + svr.nsb[index], buff, n);
            svr.nsb[index] += n;

            GlobalMembersRtksvr.rtksvrunlock(svr);
        }
        /* write solution to output stream -------------------------------------------*/
        internal static void writesol(rtksvr_t svr, int index)
        {
            solopt_t solopt = GlobalMembersRtkcmn.solopt_default;
            byte[] buff = new byte[1024];
            int i;
            int n;

            GlobalMembersRtkcmn.tracet(4, "writesol: index=%d\n", index);

            for (i = 0; i < 2; i++)
            {
                /* output solution */
                n = GlobalMembersSolution.outsols(ref buff, svr.rtk.sol, svr.rtk.rb, svr.solopt + i);
                strwrite(svr.stream + i + 3, buff, n);

                /* save output buffer */
                GlobalMembersRtksvr.saveoutbuf(svr, ref buff, n, i);

                /* output extended solution */
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: n=outsolexs(buff,&svr->rtk.sol,svr->rtk.ssat,svr->solopt+i);
                n = GlobalMembersSolution.outsolexs(ref buff, svr.rtk.sol, new ssat_t(svr.rtk.ssat), svr.solopt + i);
                strwrite(svr.stream + i + 3, buff, n);

                /* save output buffer */
                GlobalMembersRtksvr.saveoutbuf(svr, ref buff, n, i);
            }
            /* output solution to monitor port */
            if (svr.moni != null)
            {
                n = GlobalMembersSolution.outsols(ref buff, svr.rtk.sol, svr.rtk.rb, solopt);
                strwrite(svr.moni, buff, n);
            }
            /* save solution buffer */
            if (svr.nsol < DefineConstants.MAXSOLBUF)
            {
                GlobalMembersRtksvr.rtksvrlock(svr);
                svr.solbuf[svr.nsol++] = svr.rtk.sol;
                GlobalMembersRtksvr.rtksvrunlock(svr);
            }
        }
        /* update navigation data ----------------------------------------------------*/
        internal static void updatenav(nav_t nav)
        {
            int i;
            int j;
            for (i = 0; i < DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1; i++)
            {
                for (j = 0; j < DefineConstants.NFREQ; j++)
                {
                    nav.lam[i, j] = GlobalMembersRtkcmn.satwavelen(i + 1, j, nav);
                }
            }
        }
        /* update glonass frequency channel number in raw data struct ----------------*/
        internal static void updatefcn(rtksvr_t svr)
        {
            int i;
            int j;
            int sat;
            int frq;

            for (i = 0; i < DefineConstants.MAXPRNGLO; i++)
            {
                sat = GlobalMembersRtkcmn.satno(DefineConstants.SYS_GLO, i + 1);

                for (j = 0, frq = -999; j < 3; j++)
                {
                    if (svr.raw[j].nav.geph[i].sat != sat)
                        continue;
                    frq = svr.raw[j].nav.geph[i].frq;
                }
                if (frq < -7 || frq > 6)
                    continue;

                for (j = 0; j < 3; j++)
                {
                    if (svr.raw[j].nav.geph[i].sat == sat)
                        continue;
                    svr.raw[j].nav.geph[i].sat = sat;
                    svr.raw[j].nav.geph[i].frq = frq;
                }
            }
        }
        /* update rtk server struct --------------------------------------------------*/
        internal static void updatesvr(rtksvr_t svr, int ret, obs_t[] obs, nav_t nav, int sat, sbsmsg_t[] sbsmsg, int index, int iobs)
        {
            eph_t eph1;
            eph_t eph2;
            eph_t eph3;
            geph_t geph1;
            geph_t geph2;
            geph_t geph3;
            gtime_t tof = new gtime_t();
            double[] pos = new double[3];
            double[] del = { 0, null, null };
            double[] dr = new double[3];
            int i;
            int n = 0;
            int prn;
            int sbssat = svr.rtk.opt.sbassatsel;
            int sys;
            int iode;

            GlobalMembersRtkcmn.tracet(4, "updatesvr: ret=%d sat=%2d index=%d\n", ret, sat, index);

            if (ret == 1) // observation data
            {
                if (iobs < DefineConstants.MAXOBSBUF)
                {
                    for (i = 0; i < obs.n; i++)
                    {
                        if (svr.rtk.opt.exsats[obs.data[i].sat - 1] == 1 || !(GlobalMembersRtkcmn.satsys(obs.data[i].sat, null) & svr.rtk.opt.navsys))
                            continue;
                        svr.obs[index, iobs].data[n] = obs.data[i];
                        svr.obs[index, iobs].data[n++].rcv = index + 1;
                    }
                    svr.obs[index, iobs].n = n;
                    GlobalMembersRtkcmn.sortobs(svr.obs[index, iobs]);
                }
                svr.nmsg[index, 0]++;
            }
            else if (ret == 2) // ephemeris
            {
                if (GlobalMembersRtkcmn.satsys(sat, ref prn) != DefineConstants.SYS_GLO)
                {
                    if (svr.navsel == 0 || svr.navsel == index + 1)
                    {
                        eph1 = nav.eph + sat - 1;
                        eph2 = svr.nav.eph + sat - 1;
                        eph3 = svr.nav.eph + sat - 1 + DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1;
                        //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                        //ORIGINAL LINE: if (eph2->ttr.time==0|| (eph1->iode!=eph3->iode&&eph1->iode!=eph2->iode)|| (timediff(eph1->toe,eph3->toe)!=0.0&& timediff(eph1->toe,eph2->toe)!=0.0))
                        if (eph2.ttr.time == 0 || (eph1.iode != eph3.iode && eph1.iode != eph2.iode) || (GlobalMembersRtkcmn.timediff(new gtime_t(eph1.toe), new gtime_t(eph3.toe)) != 0.0 && GlobalMembersRtkcmn.timediff(new gtime_t(eph1.toe), new gtime_t(eph2.toe)) != 0.0))
                        {
                            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                            //ORIGINAL LINE: *eph3=*eph2;
                            eph3.CopyFrom(eph2);
                            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                            //ORIGINAL LINE: *eph2=*eph1;
                            eph2.CopyFrom(eph1);
                            GlobalMembersRtksvr.updatenav(svr.nav);
                        }
                    }
                    svr.nmsg[index, 1]++;
                }
                else
                {
                    if (svr.navsel == 0 || svr.navsel == index + 1)
                    {
                        geph1 = nav.geph + prn - 1;
                        geph2 = svr.nav.geph + prn - 1;
                        geph3 = svr.nav.geph + prn - 1 + DefineConstants.MAXPRNGLO;
                        if (geph2.tof.time == 0 || (geph1.iode != geph3.iode && geph1.iode != geph2.iode))
                        {
                            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                            //ORIGINAL LINE: *geph3=*geph2;
                            geph3.CopyFrom(geph2);
                            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                            //ORIGINAL LINE: *geph2=*geph1;
                            geph2.CopyFrom(geph1);
                            GlobalMembersRtksvr.updatenav(svr.nav);
                            GlobalMembersRtksvr.updatefcn(svr);
                        }
                    }
                    svr.nmsg[index, 6]++;
                }
            }
            else if (ret == 3) // sbas message
            {
                if (sbsmsg != null && (sbssat == sbsmsg.prn || sbssat == 0))
                {
                    if (svr.nsbs < DefineConstants.MAXSBSMSG)
                    {
                        svr.sbsmsg[svr.nsbs++] = *sbsmsg;
                    }
                    else
                    {
                        for (i = 0; i < DefineConstants.MAXSBSMSG - 1; i++)
                        {
                            svr.sbsmsg[i] = svr.sbsmsg[i + 1];
                        }
                        svr.sbsmsg[i] = *sbsmsg;
                    }
                    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                    //ORIGINAL LINE: sbsupdatecorr(sbsmsg,&svr->nav);
                    GlobalMembersSbas.sbsupdatecorr(new sbsmsg_t(sbsmsg), svr.nav);
                }
                svr.nmsg[index, 3]++;
            }
            else if (ret == 9) // ion/utc parameters
            {
                if (svr.navsel == index || svr.navsel >= 3)
                {
                    for (i = 0; i < 8; i++)
                    {
                        svr.nav.ion_gps[i] = nav.ion_gps[i];
                    }
                    for (i = 0; i < 4; i++)
                    {
                        svr.nav.utc_gps[i] = nav.utc_gps[i];
                    }
                    for (i = 0; i < 4; i++)
                    {
                        svr.nav.ion_gal[i] = nav.ion_gal[i];
                    }
                    for (i = 0; i < 4; i++)
                    {
                        svr.nav.utc_gal[i] = nav.utc_gal[i];
                    }
                    for (i = 0; i < 8; i++)
                    {
                        svr.nav.ion_qzs[i] = nav.ion_qzs[i];
                    }
                    for (i = 0; i < 4; i++)
                    {
                        svr.nav.utc_qzs[i] = nav.utc_qzs[i];
                    }
                    svr.nav.leaps = nav.leaps;
                }
                svr.nmsg[index, 2]++;
            }
            else if (ret == 5) // antenna postion parameters
            {
                if (svr.rtk.opt.refpos == 4 && index == 1)
                {
                    for (i = 0; i < 3; i++)
                    {
                        svr.rtk.rb[i] = svr.rtcm[1].sta.pos[i];
                    }
                    /* antenna delta */
                    GlobalMembersRtkcmn.ecef2pos(svr.rtk.rb, pos);
                    if (svr.rtcm[1].sta.deltype != 0) // xyz
                    {
                        del[2] = svr.rtcm[1].sta.hgt;
                        GlobalMembersRtkcmn.enu2ecef(pos, del, ref dr);
                        for (i = 0; i < 3; i++)
                        {
                            svr.rtk.rb[i] += svr.rtcm[1].sta.del[i] + dr[i];
                        }
                    }
                    else // enu
                    {
                        GlobalMembersRtkcmn.enu2ecef(pos, svr.rtcm[1].sta.del, ref dr);
                        for (i = 0; i < 3; i++)
                        {
                            svr.rtk.rb[i] += dr[i];
                        }
                    }
                }
                svr.nmsg[index, 4]++;
            }
            else if (ret == 7) // dgps correction
            {
                svr.nmsg[index, 5]++;
            }
            else if (ret == 10) // ssr message
            {
                for (i = 0; i < DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1; i++)
                {
                    if (svr.rtcm[index].ssr[i].update == 0)
                        continue;
                    svr.rtcm[index].ssr[i].update = 0;

                    iode = svr.rtcm[index].ssr[i].iode;
                    sys = GlobalMembersRtkcmn.satsys(i + 1, ref prn);

                    /* check corresponding ephemeris exists */
                    if (sys == DefineConstants.SYS_GPS || sys == DefineConstants.SYS_GAL || sys == DefineConstants.SYS_QZS)
                    {
                        if (svr.nav.eph[i].iode != iode && svr.nav.eph[i + DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1].iode != iode)
                        {
                            continue;
                        }
                    }
                    else if (sys == DefineConstants.SYS_GLO)
                    {
                        if (svr.nav.geph[prn - 1].iode != iode && svr.nav.geph[prn - 1 + DefineConstants.MAXPRNGLO].iode != iode)
                        {
                            continue;
                        }
                    }
                    svr.nav.ssr[i] = svr.rtcm[index].ssr[i];
                }
                svr.nmsg[index, 7]++;
            }
            else if (ret == 31) // lex message
            {
                GlobalMembersQzslex.lexupdatecorr(svr.raw[index].lexmsg, svr.nav, tof);
                svr.nmsg[index, 8]++;
            }
            else if (ret == -1) // error
            {
                svr.nmsg[index, 9]++;
            }
        }
        /* decode receiver raw/rtcm data ---------------------------------------------*/
        internal static int decoderaw(rtksvr_t svr, int index)
        {
            //C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on this variable, so pointers on this variable are left unchanged:
            obs_t* obs = new obs_t();
            nav_t nav;
            sbsmsg_t sbsmsg = null;
            int i;
            int ret;
            int sat;
            int fobs = 0;

            GlobalMembersRtkcmn.tracet(4, "decoderaw: index=%d\n", index);

            GlobalMembersRtksvr.rtksvrlock(svr);

            for (i = 0; i < svr.nb[index]; i++)
            {

                /* input rtcm/receiver raw data from stream */
                if (svr.format[index] == DefineConstants.STRFMT_RTCM2)
                {
                    ret = GlobalMembersRtcm.input_rtcm2(svr.rtcm + index, svr.buff[index][i]);
                    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                    //ORIGINAL LINE: obs=&svr->rtcm[index].obs;
                    obs.CopyFrom(svr.rtcm[index].obs);
                    nav = svr.rtcm[index].nav;
                    sat = svr.rtcm[index].ephsat;
                }
                else if (svr.format[index] == DefineConstants.STRFMT_RTCM3)
                {
                    ret = GlobalMembersRtcm.input_rtcm3(svr.rtcm + index, svr.buff[index][i]);
                    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                    //ORIGINAL LINE: obs=&svr->rtcm[index].obs;
                    obs.CopyFrom(svr.rtcm[index].obs);
                    nav = svr.rtcm[index].nav;
                    sat = svr.rtcm[index].ephsat;
                }
                else
                {
                    ret = GlobalMembersRcvraw.input_raw(svr.raw + index, svr.format[index], svr.buff[index][i]);
                    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                    //ORIGINAL LINE: obs=&svr->raw[index].obs;
                    obs.CopyFrom(svr.raw[index].obs);
                    nav = svr.raw[index].nav;
                    sat = svr.raw[index].ephsat;
                    sbsmsg = svr.raw[index].sbsmsg;
                }
#if false
	//        if (ret==1) {
	//            trace(0,"%d %10d T=%s NS=%2d\n",index,tickget(),
	//                  time_str(obs->data[0].time,0),obs->n);
	//        }
#endif
                /* update rtk server */
                if (ret > 0)
                {
                    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                    //ORIGINAL LINE: updatesvr(svr,ret,obs,nav,sat,sbsmsg,index,fobs);
                    GlobalMembersRtksvr.updatesvr(svr, ret, new obs_t(obs), nav, sat, sbsmsg, index, fobs);
                }

                /* observation data received */
                if (ret == 1)
                {
                    if (fobs < DefineConstants.MAXOBSBUF)
                    {
                        fobs++;
                    }
                    else
                    {
                        svr.prcout++;
                    }
                }
            }
            svr.nb[index] = 0;

            GlobalMembersRtksvr.rtksvrunlock(svr);

            return fobs;
        }
        /* decode download file ------------------------------------------------------*/
        internal static void decodefile(rtksvr_t svr, int index)
        {
            nav_t nav = new nav_t();
            string file = new string(new char[1024]);
            int nb;

            GlobalMembersRtkcmn.tracet(4, "decodefile: index=%d\n", index);

            GlobalMembersRtksvr.rtksvrlock(svr);

            /* check file path completed */
            if ((nb = svr.nb[index]) <= 2 || svr.buff[index][nb - 2] != '\r' || svr.buff[index][nb - 1] != '\n')
            {
                GlobalMembersRtksvr.rtksvrunlock(svr);
                return;
            }
            file = Convert.ToString((string)svr.buff[index]).Substring(0, nb - 2);
            file[nb - 2] = '\0';
            svr.nb[index] = 0;

            GlobalMembersRtksvr.rtksvrunlock(svr);

            if (svr.format[index] == DefineConstants.STRFMT_SP3) // precise ephemeris
            {

                /* read sp3 precise ephemeris */
                GlobalMembersPreceph.readsp3(file, nav, 0);
                if (nav.ne <= 0)
                {
                    GlobalMembersRtkcmn.tracet(1, "sp3 file read error: %s\n", file);
                    return;
                }
                /* update precise ephemeris */
                GlobalMembersRtksvr.rtksvrlock(svr);

                if (svr.nav.peph != null)
                {
                    //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
                    free(svr.nav.peph);
                }
                svr.nav.ne = svr.nav.nemax = nav.ne;
                svr.nav.peph = nav.peph;
                svr.ftime[index] = GlobalMembersRtkcmn.utc2gpst(GlobalMembersRtkcmn.timeget());
                svr.files[index] = file;

                GlobalMembersRtksvr.rtksvrunlock(svr);
            }
            else if (svr.format[index] == DefineConstants.STRFMT_RNXCLK) // precise clock
            {

                /* read rinex clock */
                if (GlobalMembersRinex.readrnxc(file, nav) <= 0)
                {
                    GlobalMembersRtkcmn.tracet(1, "rinex clock file read error: %s\n", file);
                    return;
                }
                /* update precise clock */
                GlobalMembersRtksvr.rtksvrlock(svr);

                if (svr.nav.pclk != null)
                {
                    //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
                    free(svr.nav.pclk);
                }
                svr.nav.nc = svr.nav.ncmax = nav.nc;
                svr.nav.pclk = nav.pclk;
                svr.ftime[index] = GlobalMembersRtkcmn.utc2gpst(GlobalMembersRtkcmn.timeget());
                svr.files[index] = file;

                GlobalMembersRtksvr.rtksvrunlock(svr);
            }
        }
        /* rtk server thread ---------------------------------------------------------*/
#if WIN32
//C++ TO C# CONVERTER NOTE: WINAPI is not available in C#:
//ORIGINAL LINE: static uint WINAPI rtksvrthread(object* arg)
	internal static uint rtksvrthread(object arg)
#else
        internal static object rtksvrthread(object arg)
#endif
        {
            rtksvr_t svr = (rtksvr_t)arg;
            obs_t obs = new obs_t();
            obsd_t[] data = Arrays.InitializeWithDefaultInstances<obsd_t>(DefineConstants.MAXOBS * 2);
            double tt;
            uint tick;
            uint ticknmea;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: byte *p,*q;
            byte p;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: byte *q;
            byte q;
            int i;
            int j;
            int n;
            int[] fobs = new int[3];
            int cycle;
            int cputime;

            GlobalMembersRtkcmn.tracet(3, "rtksvrthread:\n");

            svr.state = 1;
            obs.data = data;
            svr.tick = GlobalMembersRtkcmn.tickget();
            ticknmea = svr.tick - 1000;

            for (cycle = 0; svr.state != 0; cycle++)
            {
                tick = GlobalMembersRtkcmn.tickget();

                for (i = 0; i < 3; i++)
                {
                    p = svr.buff[i] + svr.nb[i];
                    q = svr.buff[i] + svr.buffsize;

                    /* read receiver raw/rtcm data from input stream */
                    if ((n = strread(svr.stream + i, p, q - p)) <= 0)
                    {
                        continue;
                    }
                    /* write receiver raw/rtcm data to log stream */
                    strwrite(svr.stream + i + 5, p, n);
                    svr.nb[i] += n;

                    /* save peek buffer */
                    GlobalMembersRtksvr.rtksvrlock(svr);
                    n = n < svr.buffsize - svr.npb[i] != 0 ? n : svr.buffsize - svr.npb[i];
                    //C++ TO C# CONVERTER TODO TASK: The memory management function 'memcpy' has no equivalent in C#:
                    memcpy(svr.pbuf[i] + svr.npb[i], p, n);
                    svr.npb[i] += n;
                    GlobalMembersRtksvr.rtksvrunlock(svr);
                }
                for (i = 0; i < 3; i++)
                {
                    if (svr.format[i] == DefineConstants.STRFMT_SP3 || svr.format[i] == DefineConstants.STRFMT_RNXCLK)
                    {
                        /* decode download file */
                        GlobalMembersRtksvr.decodefile(svr, i);
                    }
                    else
                    {
                        /* decode receiver raw/rtcm data */
                        fobs[i] = GlobalMembersRtksvr.decoderaw(svr, i);
                    }
                }
                for (i = 0; i < fobs[0]; i++) // for each rover observation data
                {
                    obs.n = 0;
                    for (j = 0; j < svr.obs[0, i].n && obs.n < DefineConstants.MAXOBS * 2; j++)
                    {
                        obs.data[obs.n++] = svr.obs[0, i].data[j];
                    }
                    for (j = 0; j < svr.obs[1, 0].n && obs.n < DefineConstants.MAXOBS * 2; j++)
                    {
                        obs.data[obs.n++] = svr.obs[1, 0].data[j];
                    }
                    /* rtk positioning */
                    GlobalMembersRtksvr.rtksvrlock(svr);
                    GlobalMembersRtkpos.rtkpos(svr.rtk, obs.data, obs.n, svr.nav);
                    GlobalMembersRtksvr.rtksvrunlock(svr);

                    if (svr.rtk.sol.stat != DefineConstants.SOLQ_NONE)
                    {

                        /* adjust current time */
                        tt = (int)(GlobalMembersRtkcmn.tickget() - tick) / 1000.0 + DefineConstants.DTTOL;
                        //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                        //ORIGINAL LINE: timeset(gpst2utc(timeadd(svr->rtk.sol.time,tt)));
                        GlobalMembersRtkcmn.timeset(GlobalMembersRtkcmn.gpst2utc(GlobalMembersRtkcmn.timeadd(new gtime_t(svr.rtk.sol.time), tt)));

                        /* write solution */
                        GlobalMembersRtksvr.writesol(svr, i);
                    }
                    /* if cpu overload, inclement obs outage counter and break */
                    if ((int)(GlobalMembersRtkcmn.tickget() - tick) >= svr.cycle)
                    {
                        svr.prcout += fobs[0] - i - 1;
#if false
	//                break;
#endif
                    }
                }
                /* send null solution if no solution (1hz) */
                if (svr.rtk.sol.stat == DefineConstants.SOLQ_NONE && cycle % (1000 / svr.cycle) == 0)
                {
                    GlobalMembersRtksvr.writesol(svr, 0);
                }
                /* send nmea request to base/nrtk input stream */
                if (svr.nmeacycle > 0 && (int)(tick - ticknmea) >= svr.nmeacycle)
                {
                    if (svr.stream[1].state == 1)
                    {
                        if (svr.nmeareq == 1)
                        {
                            strsendnmea(svr.stream + 1, svr.nmeapos);
                        }
                        else if (svr.nmeareq == 2 && GlobalMembersRtkcmn.norm(svr.rtk.sol.rr, 3) > 0.0)
                        {
                            strsendnmea(svr.stream + 1, svr.rtk.sol.rr);
                        }
                    }
                    ticknmea = tick;
                }
                if ((cputime = (int)(GlobalMembersRtkcmn.tickget() - tick)) > 0)
                {
                    svr.cputime = cputime;
                }

                /* sleep until next cycle */
                GlobalMembersRtkcmn.sleepms(svr.cycle - cputime);
            }
            for (i = 0; i < DefineConstants.MAXSTRRTK; i++)
            {
                strclose(svr.stream + i);
            }
            for (i = 0; i < 3; i++)
            {
                svr.nb[i] = svr.npb[i] = 0;
                //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
                free(svr.buff[i]);
                svr.buff[i] = null;
                //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
                free(svr.pbuf[i]);
                svr.pbuf[i] = null;
                GlobalMembersRcvraw.free_raw(svr.raw + i);
                GlobalMembersRtcm.free_rtcm(svr.rtcm + i);
            }
            for (i = 0; i < 2; i++)
            {
                svr.nsb[i] = 0;
                //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
                free(svr.sbuf[i]);
                svr.sbuf[i] = null;
            }
            return 0;
        }
        /* initialize rtk server -------------------------------------------------------
        * initialize rtk server
        * args   : rtksvr_t *svr    IO rtk server
        * return : status (0:error,1:ok)
        *-----------------------------------------------------------------------------*/
        public static int rtksvrinit(rtksvr_t svr)
        {
            gtime_t time0 = new gtime_t();
            sol_t sol0 = new sol_t({ 0 });
            eph_t eph0 = new eph_t(0, -1, -1);
            geph_t geph0 = new geph_t(0, -1);
            seph_t seph0 = new seph_t();
            int i;
            int j;

            GlobalMembersRtkcmn.tracet(3, "rtksvrinit:\n");

            svr.state = svr.cycle = svr.nmeacycle = svr.nmeareq = 0;
            for (i = 0; i < 3; i++)
            {
                svr.nmeapos[i] = 0.0;
            }
            svr.buffsize = 0;
            for (i = 0; i < 3; i++)
            {
                svr.format[i] = 0;
            }
            for (i = 0; i < 2; i++)
            {
                svr.solopt[i] = GlobalMembersRtkcmn.solopt_default;
            }
            svr.navsel = svr.nsbs = svr.nsol = 0;
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: rtkinit(&svr->rtk,&prcopt_default);
            GlobalMembersRtkpos.rtkinit(svr.rtk, new prcopt_t(GlobalMembersRtkcmn.prcopt_default));
            for (i = 0; i < 3; i++)
            {
                svr.nb[i] = 0;
            }
            for (i = 0; i < 2; i++)
            {
                svr.nsb[i] = 0;
            }
            for (i = 0; i < 3; i++)
            {
                svr.npb[i] = 0;
            }
            for (i = 0; i < 3; i++)
            {
                svr.buff[i] = null;
            }
            for (i = 0; i < 2; i++)
            {
                svr.sbuf[i] = null;
            }
            for (i = 0; i < 3; i++)
            {
                svr.pbuf[i] = null;
            }
            for (i = 0; i < DefineConstants.MAXSOLBUF; i++)
            {
                svr.solbuf[i] = sol0;
            }
            for (i = 0; i < 3; i++)
            {
                for (j = 0; j < 10; j++)
                {
                    svr.nmsg[i, j] = 0;
                }
            }
            for (i = 0; i < 3; i++)
            {
                svr.ftime[i] = time0;
            }
            for (i = 0; i < 3; i++)
            {
                svr.files[i, 0] = (sbyte)'\0';
            }
            svr.moni = null;
            svr.tick = 0;
            svr.thread = 0;
            svr.cputime = svr.prcout = 0;

            //C++ TO C# CONVERTER TODO TASK: The memory management function 'malloc' has no equivalent in C#:
            if ((svr.nav.eph = (eph_t)malloc(sizeof(eph_t) * DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1 * 2)) == null || (svr.nav.geph = (geph_t)malloc(sizeof(geph_t) * DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 * 2)) == null || (svr.nav.seph = (seph_t)malloc(sizeof(seph_t) * DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 * 2)) == null)
            {
                GlobalMembersRtkcmn.tracet(1, "rtksvrinit: malloc error\n");
                return 0;
            }
            for (i = 0; i < DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1 * 2; i++)
            {
                svr.nav.eph[i] = eph0;
            }
            for (i = 0; i < DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 * 2; i++)
            {
                svr.nav.geph[i] = geph0;
            }
            for (i = 0; i < DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 * 2; i++)
            {
                svr.nav.seph[i] = seph0;
            }
            svr.nav.n = DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1 * 2;
            svr.nav.ng = DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 * 2;
            svr.nav.ns = DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 * 2;

            for (i = 0; i < 3; i++)
            {
                for (j = 0; j < DefineConstants.MAXOBSBUF; j++)
                {
                    //C++ TO C# CONVERTER TODO TASK: The memory management function 'malloc' has no equivalent in C#:
                    if ((svr.obs[i, j].data = (obsd_t)malloc(sizeof(obsd_t) * DefineConstants.MAXOBS)) == null)
                    {
                        GlobalMembersRtkcmn.tracet(1, "rtksvrinit: malloc error\n");
                        return 0;
                    }
                }
            }
            for (i = 0; i < 3; i++)
            {
                //C++ TO C# CONVERTER TODO TASK: The memory management function 'memset' has no equivalent in C#:
                memset(svr.raw + i, 0, sizeof(raw_t));
                //C++ TO C# CONVERTER TODO TASK: The memory management function 'memset' has no equivalent in C#:
                memset(svr.rtcm + i, 0, sizeof(rtcm_t));
            }
            for (i = 0; i < DefineConstants.MAXSTRRTK; i++)
            {
                strinit(svr.stream + i);
            }

#if initlock_ConditionalDefinition1
		InitializeCriticalSection(svr.@lock);
#elif initlock_ConditionalDefinition2
		pthread_mutex_init(svr.@lock, null);
#else
            initlock(svr.@lock);
#endif

            return 1;
        }
        /* free rtk server -------------------------------------------------------------
        * free rtk server
        * args   : rtksvr_t *svr    IO rtk server
        * return : none
        *-----------------------------------------------------------------------------*/
        public static void rtksvrfree(rtksvr_t svr)
        {
            int i;
            int j;

            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(svr.nav.eph);
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(svr.nav.geph);
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(svr.nav.seph);
            for (i = 0; i < 3; i++)
            {
                for (j = 0; j < DefineConstants.MAXOBSBUF; j++)
                {
                    //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
                    free(svr.obs[i, j].data);
                }
            }
        }
        /* lock/unlock rtk server ------------------------------------------------------
        * lock/unlock rtk server
        * args   : rtksvr_t *svr    IO rtk server
        * return : status (1:ok 0:error)
        *-----------------------------------------------------------------------------*/
        public static void rtksvrlock(rtksvr_t svr)
        {
#if lock_ConditionalDefinition1
		EnterCriticalSection(svr.@lock);
#elif lock_ConditionalDefinition2
		pthread_mutex_lock(svr.@lock);
#else
            lock (svr.lock) ;
#endif
        }
        public static void rtksvrunlock(rtksvr_t svr)
        {
#if unlock_ConditionalDefinition1
		LeaveCriticalSection(svr.@lock);
#elif unlock_ConditionalDefinition2
		pthread_mutex_unlock(svr.@lock);
#else
            unlock(svr.@lock);
#endif
        }

        /* start rtk server ------------------------------------------------------------
        * start rtk server thread
        * args   : rtksvr_t *svr    IO rtk server
        *          int     cycle    I  server cycle (ms)
        *          int     buffsize I  input buffer size (bytes)
        *          int     *strs    I  stream types (STR_???)
        *                              types[0]=input stream rover
        *                              types[1]=input stream base station
        *                              types[2]=input stream correction
        *                              types[3]=output stream solution 1
        *                              types[4]=output stream solution 2
        *                              types[5]=log stream rover
        *                              types[6]=log stream base station
        *                              types[7]=log stream correction
        *          char    *paths   I  input stream paths
        *          int     *format  I  input stream formats (STRFMT_???)
        *                              format[0]=input stream rover
        *                              format[1]=input stream base station
        *                              format[2]=input stream correction
        *          int     navsel   I  navigation message select
        *                              (0:rover,1:base,2:ephem,3:all)
        *          char    **cmds   I  input stream start commands
        *                              cmds[0]=input stream rover (NULL: no command)
        *                              cmds[1]=input stream base (NULL: no command)
        *                              cmds[2]=input stream corr (NULL: no command)
        *          char    **rcvopts I receiver options
        *                              rcvopt[0]=receiver option rover
        *                              rcvopt[1]=receiver option base
        *                              rcvopt[2]=receiver option corr
        *          int     nmeacycle I nmea request cycle (ms) (0:no request)
        *          int     nmeareq  I  nmea request type (0:no,1:base pos,2:single sol)
        *          double *nmeapos  I  transmitted nmea position (ecef) (m)
        *          prcopt_t *prcopt I  rtk processing options
        *          solopt_t *solopt I  solution options
        *                              solopt[0]=solution 1 options
        *                              solopt[1]=solution 2 options
        *          stream_t *moni   I  monitor stream (NULL: not used)
        * return : status (1:ok 0:error)
        *-----------------------------------------------------------------------------*/
        public static int rtksvrstart(rtksvr_t svr, int cycle, int buffsize, int[] strs, string[] paths, int[] formats, int navsel, string[] cmds, string[] rcvopts, int nmeacycle, int nmeareq, double[] nmeapos, prcopt_t prcopt, solopt_t[] solopt, stream_t moni)
        {
            gtime_t time = new gtime_t();
            gtime_t time0 = new gtime_t();
            int i;
            int j;
            int rw;

            GlobalMembersRtkcmn.tracet(3, "rtksvrstart: cycle=%d buffsize=%d navsel=%d nmeacycle=%d nmeareq=%d\n", cycle, buffsize, navsel, nmeacycle, nmeareq);

            if (svr.state != 0)
            {
                return 0;
            }

            strinitcom();
            svr.cycle = cycle > 1 ? cycle : 1;
            svr.nmeacycle = nmeacycle > 1000 ? nmeacycle : 1000;
            svr.nmeareq = nmeareq;
            for (i = 0; i < 3; i++)
            {
                svr.nmeapos[i] = nmeapos[i];
            }
            svr.buffsize = buffsize > 4096 ? buffsize : 4096;
            for (i = 0; i < 3; i++)
            {
                svr.format[i] = formats[i];
            }
            svr.navsel = navsel;
            svr.nsbs = 0;
            svr.nsol = 0;
            svr.prcout = 0;
            GlobalMembersRtkpos.rtkfree(svr.rtk);
            GlobalMembersRtkpos.rtkinit(svr.rtk, prcopt);

            for (i = 0; i < 3; i++) // input/log streams
            {
                svr.nb[i] = svr.npb[i] = 0;
                //C++ TO C# CONVERTER TODO TASK: The memory management function 'malloc' has no equivalent in C#:
                if (!(svr.buff[i] = (byte)malloc(buffsize)) || !(svr.pbuf[i] = (byte)malloc(buffsize)))
                {
                    GlobalMembersRtkcmn.tracet(1, "rtksvrstart: malloc error\n");
                    return 0;
                }
                for (j = 0; j < 10; j++)
                {
                    svr.nmsg[i, j] = 0;
                }
                for (j = 0; j < DefineConstants.MAXOBSBUF; j++)
                {
                    svr.obs[i, j].n = 0;
                }

                /* initialize receiver raw and rtcm control */
                GlobalMembersRcvraw.init_raw(svr.raw + i);
                GlobalMembersRtcm.init_rtcm(svr.rtcm + i);

                /* set receiver and rtcm option */
                svr.raw[i].opt = rcvopts[i];
                svr.rtcm[i].opt = rcvopts[i];

                /* connect dgps corrections */
                svr.rtcm[i].dgps = svr.nav.dgps;
            }
            for (i = 0; i < 2; i++) // output peek buffer
            {
                //C++ TO C# CONVERTER TODO TASK: The memory management function 'malloc' has no equivalent in C#:
                if (!(svr.sbuf[i] = (byte)malloc(buffsize)))
                {
                    GlobalMembersRtkcmn.tracet(1, "rtksvrstart: malloc error\n");
                    return 0;
                }
            }
            /* set solution options */
            for (i = 0; i < 2; i++)
            {
                svr.solopt[i] = solopt[i];
            }
            /* set base station position */
            for (i = 0; i < 6; i++)
            {
                svr.rtk.rb[i] = i < 3 ? prcopt.rb[i] : 0.0;
            }
            /* update navigation data */
            for (i = 0; i < DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1 * 2; i++)
            {
                svr.nav.eph[i].ttr = time0;
            }
            for (i = 0; i < DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 * 2; i++)
            {
                svr.nav.geph[i].tof = time0;
            }
            for (i = 0; i < DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 * 2; i++)
            {
                svr.nav.seph[i].tof = time0;
            }
            GlobalMembersRtksvr.updatenav(svr.nav);

            /* set monitor stream */
            svr.moni = moni;

            /* open input streams */
            for (i = 0; i < 8; i++)
            {
                rw = i < 3 ? DefineConstants.STR_MODE_R : DefineConstants.STR_MODE_W;
                if (strs[i] != DefineConstants.STR_FILE)
                {
                    rw |= DefineConstants.STR_MODE_W;
                }
                if (!stropen(svr.stream + i, strs[i], rw, paths[i]))
                {
                    for (i--; i >= 0; i--)
                    {
                        strclose(svr.stream + i);
                    }
                    return 0;
                }
                /* set initial time for rtcm and raw */
                if (i < 3)
                {
                    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                    //ORIGINAL LINE: time=utc2gpst(timeget());
                    time.CopyFrom(GlobalMembersRtkcmn.utc2gpst(GlobalMembersRtkcmn.timeget()));
                    svr.raw[i].time = strs[i] == DefineConstants.STR_FILE ? strgettime(svr.stream + i) : time;
                    svr.rtcm[i].time = strs[i] == DefineConstants.STR_FILE ? strgettime(svr.stream + i) : time;
                }
            }
            /* sync input streams */
            strsync(svr.stream, svr.stream + 1);
            strsync(svr.stream, svr.stream + 2);

            /* write start commands to input streams */
            for (i = 0; i < 3; i++)
            {
                if (cmds[i] != null)
                {
                    strsendcmd(svr.stream + i, cmds[i]);
                }
            }
            /* write solution header to solution streams */
            for (i = 3; i < 5; i++)
            {
                GlobalMembersRtksvr.writesolhead(svr.stream + i, svr.solopt + i - 3);
            }
            /* create rtk server thread */
#if WIN32
		if (!(svr.thread = CreateThread(null,0,GlobalMembersRtksvr.rtksvrthread,svr,0,null)))
		{
#else
            if (pthread_create(svr.thread, null, GlobalMembersRtksvr.rtksvrthread, svr))
            {
#endif
                for (i = 0; i < DefineConstants.MAXSTRRTK; i++)
                {
                    strclose(svr.stream + i);
                }
                return 0;
            }
            return 1;
        }
        /* stop rtk server -------------------------------------------------------------
        * start rtk server thread
        * args   : rtksvr_t *svr    IO rtk server
        *          char    **cmds   I  input stream stop commands
        *                              cmds[0]=input stream rover (NULL: no command)
        *                              cmds[1]=input stream base  (NULL: no command)
        *                              cmds[2]=input stream ephem (NULL: no command)
        * return : none
        *-----------------------------------------------------------------------------*/
        public static void rtksvrstop(rtksvr_t svr, string[] cmds)
        {
            int i;

            GlobalMembersRtkcmn.tracet(3, "rtksvrstop:\n");

            /* write stop commands to input streams */
            GlobalMembersRtksvr.rtksvrlock(svr);
            for (i = 0; i < 3; i++)
            {
                if (cmds[i] != null)
                {
                    strsendcmd(svr.stream + i, cmds[i]);
                }
            }
            GlobalMembersRtksvr.rtksvrunlock(svr);

            /* stop rtk server */
            svr.state = 0;

            /* free rtk server thread */
#if WIN32
		WaitForSingleObject(svr.thread,10000);
		CloseHandle(svr.thread);
#else
            pthread_join(svr.thread, null);
#endif
        }
        /* open output/log stream ------------------------------------------------------
        * open output/log stream
        * args   : rtksvr_t *svr    IO rtk server
        *          int     index    I  output/log stream index
        *                              (3:solution 1,4:solution 2,5:log rover,
        *                               6:log base station,7:log correction)
        *          int     str      I  output/log stream types (STR_???)
        *          char    *path    I  output/log stream path
        *          solopt_t *solopt I  solution options
        * return : status (1:ok 0:error)
        *-----------------------------------------------------------------------------*/
        public static int rtksvropenstr(rtksvr_t svr, int index, int str, string path, solopt_t[] solopt)
        {
            GlobalMembersRtkcmn.tracet(3, "rtksvropenstr: index=%d str=%d path=%s\n", index, str, path);

            if (index < 3 || index > 7 || svr.state == 0)
            {
                return 0;
            }

            GlobalMembersRtksvr.rtksvrlock(svr);

            if (svr.stream[index].state > 0)
            {
                GlobalMembersRtksvr.rtksvrunlock(svr);
                return 0;
            }
            if (!stropen(svr.stream + index, str, DefineConstants.STR_MODE_W, path))
            {
                GlobalMembersRtkcmn.tracet(2, "stream open error: index=%d\n", index);
                GlobalMembersRtksvr.rtksvrunlock(svr);
                return 0;
            }
            if (index <= 4)
            {
                svr.solopt[index - 3] = *solopt;

                /* write solution header to solution stream */
                GlobalMembersRtksvr.writesolhead(svr.stream + index, svr.solopt + index - 3);
            }
            GlobalMembersRtksvr.rtksvrunlock(svr);
            return 1;
        }
        /* close output/log stream -----------------------------------------------------
        * close output/log stream
        * args   : rtksvr_t *svr    IO rtk server
        *          int     index    I  output/log stream index
        *                              (3:solution 1,4:solution 2,5:log rover,
        *                               6:log base station,7:log correction)
        * return : none
        *-----------------------------------------------------------------------------*/
        public static void rtksvrclosestr(rtksvr_t svr, int index)
        {
            GlobalMembersRtkcmn.tracet(3, "rtksvrclosestr: index=%d\n", index);

            if (index < 3 || index > 7 || svr.state == 0)
                return;

            GlobalMembersRtksvr.rtksvrlock(svr);

            strclose(svr.stream + index);

            GlobalMembersRtksvr.rtksvrunlock(svr);
        }
        /* get observation data status -------------------------------------------------
        * get current observation data status
        * args   : rtksvr_t *svr    I  rtk server
        *          int     rcv      I  receiver (0:rover,1:base,2:ephem)
        *          gtime_t *time    O  time of observation data
        *          int     *sat     O  satellite prn numbers
        *          double  *az      O  satellite azimuth angles (rad)
        *          double  *el      O  satellite elevation angles (rad)
        *          int     **snr    O  satellite snr for each freq (dBHz)
        *                              snr[i][j] = sat i freq j snr
        *          int     *vsat    O  valid satellite flag
        * return : number of satellites
        *-----------------------------------------------------------------------------*/
        public static int rtksvrostat(rtksvr_t svr, int rcv, gtime_t time, int[] sat, double[] az, double[] el, int[][] snr, int[] vsat)
        {
            int i;
            int j;
            int ns;

            GlobalMembersRtkcmn.tracet(4, "rtksvrostat: rcv=%d\n", rcv);

            if (svr.state == 0)
            {
                return 0;
            }
            GlobalMembersRtksvr.rtksvrlock(svr);
            ns = svr.obs[rcv, 0].n;
            if (ns > 0)
            {
                time = svr.obs[rcv, 0].data[0].time;
            }
            for (i = 0; i < ns; i++)
            {
                sat[i] = svr.obs[rcv, 0].data[i].sat;
                az[i] = svr.rtk.ssat[sat[i] - 1].azel[0];
                el[i] = svr.rtk.ssat[sat[i] - 1].azel[1];
                for (j = 0; j < DefineConstants.NFREQ; j++)
                {
                    snr[i][j] = (int)(svr.obs[rcv, 0].data[i].SNR[j] * 0.25);
                }
                if (svr.rtk.sol.stat == DefineConstants.SOLQ_NONE || svr.rtk.sol.stat == DefineConstants.SOLQ_SINGLE)
                {
                    vsat[i] = svr.rtk.ssat[sat[i] - 1].vs;
                }
                else
                {
                    vsat[i] = svr.rtk.ssat[sat[i] - 1].vsat[0];
                }
            }
            GlobalMembersRtksvr.rtksvrunlock(svr);
            return ns;
        }
        /* get stream status -----------------------------------------------------------
        * get current stream status
        * args   : rtksvr_t *svr    I  rtk server
        *          int     *sstat   O  status of streams
        *          char    *msg     O  status messages
        * return : none
        *-----------------------------------------------------------------------------*/
        public static void rtksvrsstat(rtksvr_t svr, int[] sstat, ref string msg)
        {
            int i;
            string s = new string(new char[DefineConstants.MAXSTRMSG]);
            //C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on this variable, so pointers on this variable are left unchanged:
            sbyte* p = msg;

            GlobalMembersRtkcmn.tracet(4, "rtksvrsstat:\n");

            GlobalMembersRtksvr.rtksvrlock(svr);
            for (i = 0; i < DefineConstants.MAXSTRRTK; i++)
            {
                sstat[i] = strstat(svr.stream + i, s);
                if (*s)
                {
                    p += sprintf(p, "(%d) %s ", i + 1, s);
                }
            }
            GlobalMembersRtksvr.rtksvrunlock(svr);
        }
    }
}
