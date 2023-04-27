using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ghGPS.Classes
{
    public static class GlobalMembersStreamsvr
    {
        /*------------------------------------------------------------------------------
        * streamsvr.c : stream server functions
        *
        *          Copyright (C) 2010-2012 by T.TAKASU, All rights reserved.
        *
        * options : -DWIN32    use WIN32 API
        *
        * version : $Revision:$ $Date:$
        * history : 2010/07/18 1.0  moved from stream.c
        *           2011/01/18 1.1  change api strsvrstart()
        *           2012/12/04 1.2  add stream conversion function
        *           2012/12/25 1.3  fix bug on cyclic navigation data output
        *                           suppress warnings
        *           2013/05/08 1.4  fix bug on 1 s offset for javad -> rtcm conversion
        *           2014/10/16 1.5  support input from stdout
        *-----------------------------------------------------------------------------*/


        internal const string rcsid = "$Id$";

        /* test observation data message ---------------------------------------------*/
        internal static int is_obsmsg(int msg)
        {
            return (1001 <= msg && msg <= 1004) || (1009 <= msg && msg <= 1012) || (1071 <= msg && msg <= 1077) || (1081 <= msg && msg <= 1087) || (1091 <= msg && msg <= 1097) || (1101 <= msg && msg <= 1107) || (1111 <= msg && msg <= 1117) || (1121 <= msg && msg <= 1127);
        }
        /* test navigataion data message ---------------------------------------------*/
        internal static int is_navmsg(int msg)
        {
            return msg == 1019 || msg == 1020 || msg == 1044 || msg == 1045 || msg == 1046;
        }
        /* test station info message -------------------------------------------------*/
        internal static int is_stamsg(int msg)
        {
            return msg == 1005 || msg == 1006 || msg == 1007 || msg == 1008 || msg == 1033;
        }
        /* test time interval --------------------------------------------------------*/
        internal static int is_tint(gtime_t time, double tint)
        {
            if (tint <= 0.0)
            {
                return 1;
            }
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: return fmod(time2gpst(time,null)+DefineConstants.DTTOL,tint)<=2.0 *DefineConstants.DTTOL;
            return Math.IEEERemainder(GlobalMembersRtkcmn.time2gpst(new gtime_t(time), null) + DefineConstants.DTTOL, tint) <= 2.0 * DefineConstants.DTTOL;
        }
        /* new stream converter --------------------------------------------------------
        * generate new stream converter
        * args   : int    itype     I   input stream type  (STR_???)
        *          int    otype     I   output stream type (STR_???)
        *          char   *msgs     I   output message type and interval (, separated)
        *          int    staid     I   station id
        *          int    stasel    I   station info selection (0:remote,1:local)
        *          char   *opt      I   rtcm or receiver raw options
        * return : stream generator (NULL:error)
        *-----------------------------------------------------------------------------*/
        public static strconv_t strconvnew(int itype, int otype, string msgs, int staid, int stasel, string opt)
        {
            strconv_t conv;
            double tint;
            string buff = new string(new char[1024]);
            string p;
            int msg;

            //C++ TO C# CONVERTER TODO TASK: The memory management function 'malloc' has no equivalent in C#:
            if ((conv = (strconv_t)malloc(sizeof(strconv_t))) == null)
            {
                return null;
            }

            conv.nmsg = 0;
            buff = msgs;
            for (p = StringFunctions.StrTok(buff, ","); p != 0; p = StringFunctions.StrTok(null, ","))
            {
                tint = 0.0;
                if (sscanf(p, "%d(%lf)", msg, tint) < 1)
                    continue;
                conv.msgs[conv.nmsg] = msg;
                conv.tint[conv.nmsg] = tint;
                conv.tick[conv.nmsg] = GlobalMembersRtkcmn.tickget();
                conv.ephsat[conv.nmsg++] = 0;
                if (conv.nmsg >= 32)
                    break;
            }
            if (conv.nmsg <= 0)
            {
                //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
                free(conv);
                return null;
            }
            conv.itype = itype;
            conv.otype = otype;
            conv.stasel = stasel;
            if (GlobalMembersRtcm.init_rtcm(conv.rtcm) == 0 || GlobalMembersRtcm.init_rtcm(conv.@out) == 0)
            {
                //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
                free(conv);
                return null;
            }
            if (GlobalMembersRcvraw.init_raw(conv.raw) == 0)
            {
                GlobalMembersRtcm.free_rtcm(conv.rtcm);
                GlobalMembersRtcm.free_rtcm(conv.@out);
                //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
                free(conv);
                return null;
            }
            if (stasel != 0)
            {
                conv.@out.staid = staid;
            }
            conv.rtcm.opt = string.Format("-EPHALL {0}", opt);
            conv.raw.opt = string.Format("-EPHALL {0}", opt);
            return conv;
        }
        /* free stream converter -------------------------------------------------------
        * free stream converter
        * args   : strconv_t *conv  IO  stream converter
        * return : none
        *-----------------------------------------------------------------------------*/
        public static void strconvfree(strconv_t conv)
        {
            if (conv == null)
                return;
            GlobalMembersRtcm.free_rtcm(conv.rtcm);
            GlobalMembersRtcm.free_rtcm(conv.@out);
            GlobalMembersRcvraw.free_raw(conv.raw);
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(conv);
        }
        /* copy received data from receiver raw to rtcm ------------------------------*/
        internal static void raw2rtcm(rtcm_t @out, raw_t raw, int ret)
        {
            int i;
            int sat;
            int prn;

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: out->time=raw->time;
            @out.time.CopyFrom(raw.time);

            if (ret == 1)
            {
                for (i = 0; i < raw.obs.n; i++)
                {
                    @out.time = raw.obs.data[i].time;
                    @out.obs.data[i] = raw.obs.data[i];
                }
                @out.obs.n = raw.obs.n;
            }
            else if (ret == 2)
            {
                sat = raw.ephsat;
                switch (GlobalMembersRtkcmn.satsys(sat, ref prn))
                {
                    case DefineConstants.SYS_GLO:
                        @out.nav.geph[prn - 1] = raw.nav.geph[prn - 1];
                        break;
                    case DefineConstants.SYS_GPS:
                    case DefineConstants.SYS_GAL:
                    case DefineConstants.SYS_QZS:
                    case DefineConstants.SYS_CMP:
                        @out.nav.eph[sat - 1] = raw.nav.eph[sat - 1];
                        break;
                }
                @out.ephsat = sat;
            }
            else if (ret == 9)
            {
                GlobalMembersRtkcmn.matcpy(ref @out.nav.utc_gps, raw.nav.utc_gps, 4, 1);
                GlobalMembersRtkcmn.matcpy(ref @out.nav.utc_glo, raw.nav.utc_glo, 4, 1);
                GlobalMembersRtkcmn.matcpy(ref @out.nav.utc_gal, raw.nav.utc_gal, 4, 1);
                GlobalMembersRtkcmn.matcpy(ref @out.nav.utc_qzs, raw.nav.utc_qzs, 4, 1);
                GlobalMembersRtkcmn.matcpy(ref @out.nav.ion_gps, raw.nav.ion_gps, 8, 1);
                GlobalMembersRtkcmn.matcpy(ref @out.nav.ion_gal, raw.nav.ion_gal, 4, 1);
                GlobalMembersRtkcmn.matcpy(ref @out.nav.ion_qzs, raw.nav.ion_qzs, 8, 1);
                @out.nav.leaps = raw.nav.leaps;
            }
        }
        /* copy received data from receiver rtcm to rtcm -----------------------------*/
        internal static void rtcm2rtcm(rtcm_t @out, rtcm_t rtcm, int ret, int stasel)
        {
            int i;
            int sat;
            int prn;

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: out->time=rtcm->time;
            @out.time.CopyFrom(rtcm.time);

            if (stasel == 0)
            {
                @out.staid = rtcm.staid;
            }

            if (ret == 1)
            {
                for (i = 0; i < rtcm.obs.n; i++)
                {
                    @out.obs.data[i] = rtcm.obs.data[i];
                }
                @out.obs.n = rtcm.obs.n;
            }
            else if (ret == 2)
            {
                sat = rtcm.ephsat;
                switch (GlobalMembersRtkcmn.satsys(sat, ref prn))
                {
                    case DefineConstants.SYS_GLO:
                        @out.nav.geph[prn - 1] = rtcm.nav.geph[prn - 1];
                        break;
                    case DefineConstants.SYS_GPS:
                    case DefineConstants.SYS_GAL:
                    case DefineConstants.SYS_QZS:
                    case DefineConstants.SYS_CMP:
                        @out.nav.eph[sat - 1] = rtcm.nav.eph[sat - 1];
                        break;
                }
                @out.ephsat = sat;
            }
            else if (ret == 5)
            {
                if (stasel == 0)
                {
                    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                    //ORIGINAL LINE: out->sta=rtcm->sta;
                    @out.sta.CopyFrom(rtcm.sta);
                }
            }
            else if (ret == 9)
            {
                GlobalMembersRtkcmn.matcpy(ref @out.nav.utc_gps, rtcm.nav.utc_gps, 4, 1);
                GlobalMembersRtkcmn.matcpy(ref @out.nav.utc_glo, rtcm.nav.utc_glo, 4, 1);
                GlobalMembersRtkcmn.matcpy(ref @out.nav.utc_gal, rtcm.nav.utc_gal, 4, 1);
                GlobalMembersRtkcmn.matcpy(ref @out.nav.utc_qzs, rtcm.nav.utc_qzs, 4, 1);
                GlobalMembersRtkcmn.matcpy(ref @out.nav.ion_gps, rtcm.nav.ion_gps, 8, 1);
                GlobalMembersRtkcmn.matcpy(ref @out.nav.ion_gal, rtcm.nav.ion_gal, 4, 1);
                GlobalMembersRtkcmn.matcpy(ref @out.nav.ion_qzs, rtcm.nav.ion_qzs, 8, 1);
                @out.nav.leaps = rtcm.nav.leaps;
            }
        }
        /* write obs data messages ---------------------------------------------------*/
        internal static void write_obs(gtime_t time, stream_t str, strconv_t conv)
        {
            int i;
            int j = 0;

            for (i = 0; i < conv.nmsg; i++)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: if (!is_obsmsg(conv->msgs[i])||!is_tint(time,conv->tint[i]))
                if (GlobalMembersStreamsvr.is_obsmsg(conv.msgs[i]) == 0 || GlobalMembersStreamsvr.is_tint(new gtime_t(time), conv.tint[i]) == 0)
                    continue;

                j = i; // index of last message
            }
            for (i = 0; i < conv.nmsg; i++)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: if (!is_obsmsg(conv->msgs[i])||!is_tint(time,conv->tint[i]))
                if (GlobalMembersStreamsvr.is_obsmsg(conv.msgs[i]) == 0 || GlobalMembersStreamsvr.is_tint(new gtime_t(time), conv.tint[i]) == 0)
                    continue;

                /* generate messages */
                if (conv.otype == DefineConstants.STRFMT_RTCM2)
                {
                    if (GlobalMembersRtcm.gen_rtcm2(conv.@out, conv.msgs[i], i != j) == 0)
                        continue;
                }
                else if (conv.otype == DefineConstants.STRFMT_RTCM3)
                {
                    if (GlobalMembersRtcm.gen_rtcm3(conv.@out, conv.msgs[i], i != j) == 0)
                        continue;
                }
                else
                    continue;

                /* write messages to stream */
                strwrite(str, conv.@out.buff, conv.@out.nbyte);
            }
        }
        /* write nav data messages ---------------------------------------------------*/
        internal static void write_nav(gtime_t time, stream_t str, strconv_t conv)
        {
            int i;

            for (i = 0; i < conv.nmsg; i++)
            {
                if (GlobalMembersStreamsvr.is_navmsg(conv.msgs[i]) == 0 || conv.tint[i] > 0.0)
                    continue;

                /* generate messages */
                if (conv.otype == DefineConstants.STRFMT_RTCM2)
                {
                    if (GlobalMembersRtcm.gen_rtcm2(conv.@out, conv.msgs[i], 0) == 0)
                        continue;
                }
                else if (conv.otype == DefineConstants.STRFMT_RTCM3)
                {
                    if (GlobalMembersRtcm.gen_rtcm3(conv.@out, conv.msgs[i], 0) == 0)
                        continue;
                }
                else
                    continue;

                /* write messages to stream */
                strwrite(str, conv.@out.buff, conv.@out.nbyte);
            }
        }
        /* next ephemeris satellite --------------------------------------------------*/
        internal static int nextsat(nav_t nav, int sat, int msg)
        {
            int sys;
            int p;
            int p0;
            int p1;
            int p2;

            switch (msg)
            {
                case 1019:
                    sys = DefineConstants.SYS_GPS;
                    p1 = DefineConstants.MINPRNGPS;
                    p2 = DefineConstants.MAXPRNGPS;
                    break;
                case 1020:
                    sys = DefineConstants.SYS_GLO;
                    p1 = DefineConstants.MINPRNGLO;
                    p2 = DefineConstants.MAXPRNGLO;
                    break;
                case 1044:
                    sys = DefineConstants.SYS_QZS;
                    p1 = DefineConstants.MINPRNQZS;
                    p2 = DefineConstants.MAXPRNQZS;
                    break;
                case 1045:
                case 1046:
                    sys = DefineConstants.SYS_GAL;
                    p1 = DefineConstants.MINPRNGAL;
                    p2 = DefineConstants.MAXPRNGAL;
                    break;
                default:
                    return 0;
            }
            if (GlobalMembersRtkcmn.satsys(sat, ref p0) != sys)
            {
                return GlobalMembersRtkcmn.satno(sys, p1);
            }

            /* search next valid ephemeris */
            for (p = p0 > p2 != 0 ? p1 : p0 + 1; p != p0; p = p >= p2 != 0 ? p1 : p + 1)
            {

                if (sys == DefineConstants.SYS_GLO)
                {
                    sat = GlobalMembersRtkcmn.satno(sys, p);
                    if (nav.geph[p - 1].sat == sat)
                    {
                        return sat;
                    }
                }
                else
                {
                    sat = GlobalMembersRtkcmn.satno(sys, p);
                    if (nav.eph[sat - 1].sat == sat)
                    {
                        return sat;
                    }
                }
            }
            return 0;
        }
        /* write cyclic nav data messages --------------------------------------------*/
        internal static void write_nav_cycle(stream_t str, strconv_t conv)
        {
            uint tick = GlobalMembersRtkcmn.tickget();
            int i;
            int sat;
            int tint;

            for (i = 0; i < conv.nmsg; i++)
            {
                if (GlobalMembersStreamsvr.is_navmsg(conv.msgs[i]) == 0 || conv.tint[i] <= 0.0)
                    continue;

                /* output cycle */
                tint = (int)(conv.tint[i] * 1000.0);
                if ((int)(tick - conv.tick[i]) < tint)
                    continue;
                conv.tick[i] = tick;

                /* next satellite */
                if ((sat = GlobalMembersStreamsvr.nextsat(conv.@out.nav, conv.ephsat[i], conv.msgs[i])) == 0)
                {
                    continue;
                }
                conv.@out.ephsat = conv.ephsat[i] = sat;

                /* generate messages */
                if (conv.otype == DefineConstants.STRFMT_RTCM2)
                {
                    if (GlobalMembersRtcm.gen_rtcm2(conv.@out, conv.msgs[i], 0) == 0)
                        continue;
                }
                else if (conv.otype == DefineConstants.STRFMT_RTCM3)
                {
                    if (GlobalMembersRtcm.gen_rtcm3(conv.@out, conv.msgs[i], 0) == 0)
                        continue;
                }
                else
                    continue;

                /* write messages to stream */
                strwrite(str, conv.@out.buff, conv.@out.nbyte);
            }
        }
        /* write cyclic station info messages ----------------------------------------*/
        internal static void write_sta_cycle(stream_t str, strconv_t conv)
        {
            uint tick = GlobalMembersRtkcmn.tickget();
            int i;
            int tint;

            for (i = 0; i < conv.nmsg; i++)
            {
                if (GlobalMembersStreamsvr.is_stamsg(conv.msgs[i]) == 0)
                    continue;

                /* output cycle */
                tint = conv.tint[i] == 0.0 ? 30000 : (int)(conv.tint[i] * 1000.0);
                if ((int)(tick - conv.tick[i]) < tint)
                    continue;
                conv.tick[i] = tick;

                /* generate messages */
                if (conv.otype == DefineConstants.STRFMT_RTCM2)
                {
                    if (GlobalMembersRtcm.gen_rtcm2(conv.@out, conv.msgs[i], 0) == 0)
                        continue;
                }
                else if (conv.otype == DefineConstants.STRFMT_RTCM3)
                {
                    if (GlobalMembersRtcm.gen_rtcm3(conv.@out, conv.msgs[i], 0) == 0)
                        continue;
                }
                else
                    continue;

                /* write messages to stream */
                strwrite(str, conv.@out.buff, conv.@out.nbyte);
            }
        }
        /* convert stearm ------------------------------------------------------------*/
        internal static void strconv(stream_t str, strconv_t conv, byte[] buff, int n)
        {
            int i;
            int ret;

            for (i = 0; i < n; i++)
            {

                /* input rtcm 2 messages */
                if (conv.itype == DefineConstants.STRFMT_RTCM2)
                {
                    ret = GlobalMembersRtcm.input_rtcm2(conv.rtcm, buff[i]);
                    GlobalMembersStreamsvr.rtcm2rtcm(conv.@out, conv.rtcm, ret, conv.stasel);
                }
                /* input rtcm 3 messages */
                else if (conv.itype == DefineConstants.STRFMT_RTCM3)
                {
                    ret = GlobalMembersRtcm.input_rtcm3(conv.rtcm, buff[i]);
                    GlobalMembersStreamsvr.rtcm2rtcm(conv.@out, conv.rtcm, ret, conv.stasel);
                }
                /* input receiver raw messages */
                else
                {
                    ret = GlobalMembersRcvraw.input_raw(conv.raw, conv.itype, buff[i]);
                    GlobalMembersStreamsvr.raw2rtcm(conv.@out, conv.raw, ret);
                }
                /* write obs and nav data messages to stream */
                switch (ret)
                {
                    case 1:
                        //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                        //ORIGINAL LINE: write_obs(conv->out.time,str,conv);
                        GlobalMembersStreamsvr.write_obs(new gtime_t(conv.@out.time), str, conv);
                        break;
                    case 2:
                        //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                        //ORIGINAL LINE: write_nav(conv->out.time,str,conv);
                        GlobalMembersStreamsvr.write_nav(new gtime_t(conv.@out.time), str, conv);
                        break;
                }
            }
            /* write cyclic nav data and station info messages to stream */
            GlobalMembersStreamsvr.write_nav_cycle(str, conv);
            GlobalMembersStreamsvr.write_sta_cycle(str, conv);
        }
        /* stearm server thread ------------------------------------------------------*/
#if WIN32
//C++ TO C# CONVERTER NOTE: WINAPI is not available in C#:
//ORIGINAL LINE: static uint WINAPI strsvrthread(object* arg)
	internal static uint strsvrthread(object arg)
#else
        internal static object strsvrthread(object arg)
#endif
        {
            strsvr_t svr = (strsvr_t)arg;
            uint tick;
            uint ticknmea;
            int i;
            int n;

            GlobalMembersRtkcmn.tracet(3, "strsvrthread:\n");

            svr.state = 1;
            svr.tick = GlobalMembersRtkcmn.tickget();
            ticknmea = svr.tick - 1000;

            while (svr.state != 0)
            {
                tick = GlobalMembersRtkcmn.tickget();

                /* read data from input stream */
                n = strread(svr.stream, svr.buff, svr.buffsize);

                /* write data to output streams */
                for (i = 1; i < svr.nstr; i++)
                {
                    if (svr.conv[i - 1] != null)
                    {
                        GlobalMembersStreamsvr.strconv(svr.stream + i, svr.conv[i - 1], svr.buff, n);
                    }
                    else
                    {
                        strwrite(svr.stream + i, svr.buff, n);
                    }
                }
                /* write nmea messages to input stream */
                if (svr.nmeacycle > 0 && (int)(tick - ticknmea) >= svr.nmeacycle)
                {
                    strsendnmea(svr.stream, svr.nmeapos);
                    ticknmea = tick;
                }
#if lock_ConditionalDefinition1
			EnterCriticalSection(svr.@lock);
#elif lock_ConditionalDefinition2
			pthread_mutex_lock(svr.@lock);
#else
                lock (svr.lock) ;
#endif
                for (i = 0; i < n && svr.npb < svr.buffsize; i++)
                {
                    svr.pbuf[svr.npb++] = svr.buff[i];
                }
#if unlock_ConditionalDefinition1
			LeaveCriticalSection(svr.@lock);
#elif unlock_ConditionalDefinition2
			pthread_mutex_unlock(svr.@lock);
#else
                unlock(svr.@lock);
#endif

                GlobalMembersRtkcmn.sleepms(svr.cycle - (int)(GlobalMembersRtkcmn.tickget() - tick));
            }
            for (i = 0; i < svr.nstr; i++)
            {
                strclose(svr.stream + i);
            }
            svr.npb = 0;
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(svr.buff);
            svr.buff = null;
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(svr.pbuf);
            svr.pbuf = null;

            return 0;
        }
        /* initialize stream server ----------------------------------------------------
        * initialize stream server
        * args   : strsvr_t *svr    IO  stream sever struct
        *          int    nout      I   number of output streams
        * return : none
        *-----------------------------------------------------------------------------*/
        public static void strsvrinit(strsvr_t svr, int nout)
        {
            int i;

            GlobalMembersRtkcmn.tracet(3, "strsvrinit: nout=%d\n", nout);

            svr.state = 0;
            svr.cycle = 0;
            svr.buffsize = 0;
            svr.nmeacycle = 0;
            svr.npb = 0;
            for (i = 0; i < 3; i++)
            {
                svr.nmeapos[i] = 0.0;
            }
            svr.buff = svr.pbuf = null;
            svr.tick = 0;
            for (i = 0; i < nout + 1 && i < 16; i++)
            {
                strinit(svr.stream + i);
            }
            svr.nstr = i;
            for (i = 0; i < 16; i++)
            {
                svr.conv[i] = null;
            }
            svr.thread = 0;
#if initlock_ConditionalDefinition1
		InitializeCriticalSection(svr.@lock);
#elif initlock_ConditionalDefinition2
		pthread_mutex_init(svr.@lock, null);
#else
            initlock(svr.@lock);
#endif
        }
        /* start stream server ---------------------------------------------------------
        * start stream server
        * args   : strsvr_t *svr    IO  stream sever struct
        *          int    *opts     I   stream options
        *              opts[0]= inactive timeout (ms)
        *              opts[1]= interval to reconnect (ms)
        *              opts[2]= averaging time of data rate (ms)
        *              opts[3]= receive/send buffer size (bytes);
        *              opts[4]= server cycle (ms)
        *              opts[5]= nmea request cycle (ms) (0:no)
        *              opts[6]= file swap margin (s)
        *          int    *strs     I   stream types (STR_???)
        *              strs[0]= input stream
        *              strs[1]= output stream 1
        *              strs[2]= output stream 2
        *              strs[3]= output stream 3
        *          char   **paths   I   stream paths
        *              paths[0]= input stream
        *              paths[1]= output stream 1
        *              paths[2]= output stream 2
        *              paths[3]= output stream 3
        *          strcnv **conv    I   stream converter
        *              conv[0]= output stream 1 converter
        *              conv[1]= output stream 2 converter
        *              conv[2]= output stream 3 converter
        *          char   *cmd      I   input stream start command (NULL: no cmd)
        *          double *nmeapos  I   nmea request position (ecef) (m) (NULL: no)
        * return : status (0:error,1:ok)
        *-----------------------------------------------------------------------------*/
        public static int strsvrstart(strsvr_t svr, int[] opts, int[] strs, string[] paths, strconv_t[][] conv, string cmd, double[] nmeapos)
        {
            int i;
            int rw;
            int[] stropt = new int[5];
            string file1 = new string(new char[DefineConstants.MAXSTRPATH]);
            string file2 = new string(new char[DefineConstants.MAXSTRPATH]);
            string p;

            GlobalMembersRtkcmn.tracet(3, "strsvrstart:\n");

            if (svr.state != 0)
            {
                return 0;
            }

            strinitcom();

            for (i = 0; i < 4; i++)
            {
                stropt[i] = opts[i];
            }
            stropt[4] = opts[6];
            strsetopt(stropt);
            svr.cycle = opts[4];
            svr.buffsize = opts[3] < 4096 ? 4096 : opts[3]; // >=4096byte
            svr.nmeacycle = 0 < opts[5] && opts[5] < 1000 ? 1000 : opts[5]; // >=1s
            for (i = 0; i < 3; i++)
            {
                svr.nmeapos[i] = nmeapos != 0 ? nmeapos[i] : 0.0;
            }

            for (i = 0; i < svr.nstr - 1; i++)
            {
                svr.conv[i] = conv[i];
            }

            //C++ TO C# CONVERTER TODO TASK: The memory management function 'malloc' has no equivalent in C#:
            if ((svr.buff = (byte)malloc(svr.buffsize)) == 0 || (svr.pbuf = (byte)malloc(svr.buffsize)) == 0)
            {
                //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
                free(svr.buff);
                //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
                free(svr.pbuf);
                return 0;
            }
            /* open streams */
            for (i = 0; i < svr.nstr; i++)
            {
                file1 = paths[0];
                if ((p = StringFunctions.StrStr(file1, "::")) != 0)
                {
                    p = (sbyte)'\0';
                }
                file2 = paths[i];
                if ((p = StringFunctions.StrStr(file2, "::")) != 0)
                {
                    p = (sbyte)'\0';
                }
                if (i > 0 && *file1 && !string.Compare(file1, file2))
                {
                    svr.stream[i].msg = string.Format("output path error: {0}", file2);
                    for (i--; i >= 0; i--)
                    {
                        strclose(svr.stream + i);
                    }
                    return 0;
                }
                rw = i == 0 ? DefineConstants.STR_MODE_R : DefineConstants.STR_MODE_W;
                if (strs[i] != DefineConstants.STR_FILE)
                {
                    rw |= DefineConstants.STR_MODE_W;
                }
                if (stropen(svr.stream + i, strs[i], rw, paths[i]))
                    continue;
                for (i--; i >= 0; i--)
                {
                    strclose(svr.stream + i);
                }
                return 0;
            }
            /* write start command to input stream */
            if (cmd != 0)
            {
                strsendcmd(svr.stream, cmd);
            }

            /* create stream server thread */
#if WIN32
		if (!(svr.thread = CreateThread(null,0,GlobalMembersStreamsvr.strsvrthread,svr,0,null)))
		{
#else
            if (pthread_create(svr.thread, null, GlobalMembersStreamsvr.strsvrthread, svr))
            {
#endif
                for (i = 0; i < svr.nstr; i++)
                {
                    strclose(svr.stream + i);
                }
                return 0;
            }
            return 1;
        }
        /* stop stream server ----------------------------------------------------------
        * start stream server
        * args   : strsvr_t *svr    IO  stream server struct
        *          char  *cmd       I   input stop command (NULL: no cmd)
        * return : none
        *-----------------------------------------------------------------------------*/
        public static void strsvrstop(strsvr_t svr, string cmd)
        {
            GlobalMembersRtkcmn.tracet(3, "strsvrstop:\n");

            if (cmd != 0)
            {
                strsendcmd(svr.stream, cmd);
            }

            svr.state = 0;

#if WIN32
		WaitForSingleObject(svr.thread,10000);
		CloseHandle(svr.thread);
#else
            pthread_join(svr.thread, null);
#endif
        }
        /* get stream server status ----------------------------------------------------
        * get status of stream server
        * args   : strsvr_t *svr    IO  stream sever struct
        *          int    *stat     O   stream status
        *          int    *byte     O   bytes received/sent
        *          int    *bps      O   bitrate received/sent
        *          char   *msg      O   messages
        * return : none
        *-----------------------------------------------------------------------------*/
        public static void strsvrstat(strsvr_t svr, int[] stat, ref int @byte, ref int bps, ref string msg)
        {
            string s = "";
            //C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on this variable, so pointers on this variable are left unchanged:
            sbyte* p = msg;
            int i;

            GlobalMembersRtkcmn.tracet(4, "strsvrstat:\n");

            for (i = 0; i < svr.nstr; i++)
            {
                if (i == 0)
                {
                    strsum(svr.stream, @byte, bps, null, null);
                    stat[i] = strstat(svr.stream, s);
                }
                else
                {
                    strsum(svr.stream + i, null, null, @byte + i, bps + i);
                    stat[i] = strstat(svr.stream + i, s);
                }
                if (*s)
                {
                    p += sprintf(p, "(%d) %s ", i, s);
                }
            }
        }
        /* peek input/output stream ----------------------------------------------------
        * peek input/output stream of stream server
        * args   : strsvr_t *svr    IO  stream sever struct
        *          unsigend char *msg O stream buff
        *          int    nmax      I  buffer size (bytes)
        * return : stream size (bytes)
        *-----------------------------------------------------------------------------*/
        public static int strsvrpeek(strsvr_t svr, ref byte buff, int nmax)
        {
            int n;

            if (svr.state == 0)
            {
                return 0;
            }

#if lock_ConditionalDefinition1
		EnterCriticalSection(svr.@lock);
#elif lock_ConditionalDefinition2
		pthread_mutex_lock(svr.@lock);
#else
            lock (svr.lock) ;
#endif
            n = svr.npb < nmax != 0 ? svr.npb : nmax;
            if (n > 0)
            {
                //C++ TO C# CONVERTER TODO TASK: The memory management function 'memcpy' has no equivalent in C#:
                memcpy(buff, svr.pbuf, n);
            }
            if (n < svr.npb)
            {
                //C++ TO C# CONVERTER TODO TASK: The memory management function 'memmove' has no equivalent in C#:
                memmove(svr.pbuf, svr.pbuf + n, svr.npb - n);
            }
            svr.npb -= n;
#if unlock_ConditionalDefinition1
		LeaveCriticalSection(svr.@lock);
#elif unlock_ConditionalDefinition2
		pthread_mutex_unlock(svr.@lock);
#else
            unlock(svr.@lock);
#endif
            return n;
        }
    }
}
