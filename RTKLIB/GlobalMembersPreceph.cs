using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ghGPS.Classes
{
    public static class GlobalMembersPreceph
    {
        /*------------------------------------------------------------------------------
        * preceph.c : precise ephemeris and clock functions
        *
        *          Copyright (C) 2007-2013 by T.TAKASU, All rights reserved.
        *
        * references :
        *     [1] S.Hilla, The Extended Standard Product 3 Orbit Format (SP3-c),
        *         12 February, 2007
        *     [2] J.Ray, W.Gurtner, RINEX Extensions to Handle Clock Information,
        *         27 August, 1998
        *     [3] D.D.McCarthy, IERS Technical Note 21, IERS Conventions 1996, July 1996
        *     [4] D.A.Vallado, Fundamentals of Astrodynamics and Applications 2nd ed,
        *         Space Technology Library, 2004
        *
        * version : $Revision: 1.1 $ $Date: 2008/07/17 21:48:06 $
        * history : 2009/01/18 1.0  new
        *           2009/01/31 1.1  fix bug on numerical error to read sp3a ephemeris
        *           2009/05/15 1.2  support glonass,galileo,qzs
        *           2009/12/11 1.3  support wild-card expansion of file path
        *           2010/07/21 1.4  added api:
        *                               eci2ecef(),sunmoonpos(),peph2pos(),satantoff(),
        *                               readdcb()
        *                           changed api:
        *                               readsp3()
        *                           deleted api:
        *                               eph2posp()
        *           2010/09/09 1.5  fix problem when precise clock outage
        *           2011/01/23 1.6  support qzss satellite code
        *           2011/09/12 1.7  fix problem on precise clock outage
        *                           move sunmmonpos() to rtkcmn.c
        *           2011/12/01 1.8  modify api readsp3()
        *                           precede later ephemeris if ephemeris is NULL
        *                           move eci2ecef() to rtkcmn.c
        *           2013/05/08 1.9  fix bug on computing std-dev of precise clocks
        *           2013/11/20 1.10 modify option for api readsp3()
        *           2014/04/03 1.11 accept extenstion including sp3,eph,SP3,EPH
        *           2014/05/23 1.12 add function to read sp3 velocity records
        *                           change api: satantoff()
        *           2014/08/31 1.13 add member cov and vco in peph_t sturct
        *           2014/10/13 1.14 fix bug on clock error variance in peph2pos()
        *-----------------------------------------------------------------------------*/


        /* satellite code to satellite system ----------------------------------------*/
        internal static int code2sys(sbyte code)
        {
            if (code == 'G' || code == ' ')
            {
                return DefineConstants.SYS_GPS;
            }
            if (code == 'R')
            {
                return DefineConstants.SYS_GLO;
            }
            if (code == 'E') // extension to sp3-c
            {
                return DefineConstants.SYS_GAL;
            }
            if (code == 'J') // extension to sp3-c
            {
                return DefineConstants.SYS_QZS;
            }
            if (code == 'C') // extension to sp3-c
            {
                return DefineConstants.SYS_CMP;
            }
            if (code == 'L') // extension to sp3-c
            {
                return DefineConstants.SYS_LEO;
            }
            return DefineConstants.SYS_NONE;
        }
        /* read sp3 header -----------------------------------------------------------*/
        internal static int readsp3h(FILE fp, gtime_t time, ref string type, int[] sats, double[] bfact, ref string tsys)
        {
            int i;
            int j;
            int k = 0;
            int ns = 0;
            int sys;
            int prn;
            string buff = new string(new char[1024]);

            GlobalMembersRtkcmn.trace(3, "readsp3h:\n");

            for (i = 0; i < 22; i++)
            {
                if (!fgets(buff, sizeof(sbyte), fp))
                    break;

                if (i == 0)
                {
                    type = buff[2];
                    if (GlobalMembersRtkcmn.str2time(buff, 3, 28, time) != 0)
                    {
                        return 0;
                    }
                }
                else if (2 <= i && i <= 6)
                {
                    if (i == 2)
                    {
                        ns = (int)GlobalMembersRtkcmn.str2num(buff, 4, 2);
                    }
                    for (j = 0; j < 17 && k < ns; j++)
                    {
                        sys = GlobalMembersPreceph.code2sys(buff[9 + 3 * j]);
                        prn = (int)GlobalMembersRtkcmn.str2num(buff, 10 + 3 * j, 2);
                        if (k < DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1)
                        {
                            sats[k++] = GlobalMembersRtkcmn.satno(sys, prn);
                        }
                    }
                }
                else if (i == 12)
                {
                    tsys = buff.Substring(9, 3);
                    tsys[3] = '\0';
                }
                else if (i == 14)
                {
                    bfact[0] = GlobalMembersRtkcmn.str2num(buff, 3, 10);
                    bfact[1] = GlobalMembersRtkcmn.str2num(buff, 14, 12);
                }
            }
            return ns;
        }
        /* add precise ephemeris -----------------------------------------------------*/
        internal static int addpeph(nav_t nav, peph_t[] peph)
        {
            peph_t nav_peph;

            if (nav.ne >= nav.nemax)
            {
                nav.nemax += 256;
                //C++ TO C# CONVERTER TODO TASK: The memory management function 'realloc' has no equivalent in C#:
                if ((nav_peph = (peph_t)realloc(nav.peph, sizeof(peph_t) * nav.nemax)) == null)
                {
                    GlobalMembersRtkcmn.trace(1, "readsp3b malloc error n=%d\n", nav.nemax);
                    //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
                    free(nav.peph);
                    nav.peph = null;
                    nav.ne = nav.nemax = 0;
                    return 0;
                }
                nav.peph = nav_peph;
            }
            nav.peph[nav.ne++] = *peph;
            return 1;
        }
        /* read sp3 body -------------------------------------------------------------*/
        internal static void readsp3b(FILE fp, sbyte type, ref int sats, int ns, double[] bfact, ref string tsys, int index, int opt, nav_t nav)
        {
            peph_t peph = new peph_t();
            gtime_t time = new gtime_t();
            double val;
            double std;
            double @base;
            int i;
            int j;
            int sat;
            int sys;
            int prn;
            int n = ns * (type == 'P' ? 1 : 2);
            int pred_o;
            int pred_c;
            int v;
            string buff = new string(new char[1024]);

            GlobalMembersRtkcmn.trace(3, "readsp3b: type=%c ns=%d index=%d opt=%d\n", type, ns, index, opt);

            while (fgets(buff, sizeof(sbyte), fp))
            {

                if (!string.Compare(buff, 0, "EOF", 0, 3))
                    break;

                if (buff[0] != '*' || GlobalMembersRtkcmn.str2time(buff, 3, 28, time) != 0)
                {
                    GlobalMembersRtkcmn.trace(2, "sp3 invalid epoch %31.31s\n", buff);
                    continue;
                }
                if (!string.Compare(tsys, "UTC")) // utc->gpst
                {
                    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                    //ORIGINAL LINE: time=utc2gpst(time);
                    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                    time.CopyFrom(GlobalMembersRtkcmn.utc2gpst(new gtime_t(time)));
                }
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                //ORIGINAL LINE: peph.time =time;
                peph.time.CopyFrom(time);
                peph.index = index;

                for (i = 0; i < DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1; i++)
                {
                    for (j = 0; j < 4; j++)
                    {
                        peph.pos[i, j] = 0.0;
                        peph.std[i, j] = 0.0f;
                        peph.vel[i, j] = 0.0;
                        peph.vst[i, j] = 0.0f;
                    }
                    for (j = 0; j < 3; j++)
                    {
                        peph.cov[i, j] = 0.0f;
                        peph.vco[i, j] = 0.0f;
                    }
                }
                for (i = pred_o = pred_c = v = 0; i < n && fgets(buff, sizeof(sbyte), fp); i++)
                {

                    if (buff.Length < 4 || (buff[0] != 'P' && buff[0] != 'V'))
                        continue;

                    sys = buff[1] == ' ' ? DefineConstants.SYS_GPS : GlobalMembersPreceph.code2sys(buff[1]);
                    prn = (int)GlobalMembersRtkcmn.str2num(buff, 2, 2);
                    if (sys == DefineConstants.SYS_SBS)
                    {
                        prn += 100;
                    }
                    else if (sys == DefineConstants.SYS_QZS) // extension to sp3-c
                    {
                        prn += 192;
                    }

                    if ((sat = GlobalMembersRtkcmn.satno(sys, prn)) == 0)
                        continue;

                    if (buff[0] == 'P')
                    {
                        pred_c = buff.Length >= 76 && buff[75] == 'P';
                        pred_o = buff.Length >= 80 && buff[79] == 'P';
                    }
                    for (j = 0; j < 4; j++)
                    {

                        /* read option for predicted value */
                        if (j < 3 && (opt & 1) && pred_o != 0)
                            continue;
                        if (j < 3 && (opt & 2) && pred_o == 0)
                            continue;
                        if (j == 3 && (opt & 1) && pred_c != 0)
                            continue;
                        if (j == 3 && (opt & 2) && pred_c == 0)
                            continue;

                        val = GlobalMembersRtkcmn.str2num(buff, 4 + j * 14, 14);
                        std = GlobalMembersRtkcmn.str2num(buff, 61 + j * 3, j < 3 ? 2 : 3);

                        if (buff[0] == 'P') // position
                        {
                            if (val != 0.0 && Math.Abs(val - 999999.999999) >= 1E-6)
                            {
                                peph.pos[sat - 1, j] = val * (j < 3 ? 1000.0 : 1E-6);
                                v = 1; // valid epoch
                            }
                            if ((@base = bfact[j < 3 ? 0 : 1]) > 0.0 && std > 0.0)
                            {
                                peph.std[sat - 1, j] = (float)(Math.Pow(@base, std) * (j < 3 ? 1E-3 : 1E-12));
                            }
                        }
                        else if (v != 0) // velocity
                        {
                            if (val != 0.0 && Math.Abs(val - 999999.999999) >= 1E-6)
                            {
                                peph.vel[sat - 1, j] = val * (j < 3 ? 0.1 : 1E-10);
                            }
                            if ((@base = bfact[j < 3 ? 0 : 1]) > 0.0 && std > 0.0)
                            {
                                peph.vst[sat - 1, j] = (float)(Math.Pow(@base, std) * (j < 3 ? 1E-7 : 1E-16));
                            }
                        }
                    }
                }
                if (v != 0)
                {
                    if (GlobalMembersPreceph.addpeph(nav, peph) == 0)
                        return;
                }
            }
        }
        /* compare precise ephemeris -------------------------------------------------*/
        internal static int cmppeph(object p1, object p2)
        {
            peph_t q1 = (peph_t)p1;
            peph_t q2 = (peph_t)p2;
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: double tt=timediff(q1->time,q2->time);
            double tt = GlobalMembersRtkcmn.timediff(new gtime_t(q1.time), new gtime_t(q2.time));
            return tt < -1E-9 ? -1 : (tt > 1E-9 ? 1 : q1.index - q2.index);
        }
        /* combine precise ephemeris -------------------------------------------------*/
        internal static void combpeph(nav_t nav, int opt)
        {
            int i;
            int j;
            int k;
            int m;

            GlobalMembersRtkcmn.trace(3, "combpeph: ne=%d\n", nav.ne);

            qsort(nav.peph, nav.ne, sizeof(peph_t), GlobalMembersPreceph.cmppeph);

            if ((opt & 4) != 0)
                return;

            for (i = 0, j = 1; j < nav.ne; j++)
            {

                if (Math.Abs(GlobalMembersRtkcmn.timediff(nav.peph[i].time, nav.peph[j].time)) < 1E-9)
                {

                    for (k = 0; k < DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1; k++)
                    {
                        if (GlobalMembersRtkcmn.norm(nav.peph[j].pos[k], 4) <= 0.0)
                            continue;
                        for (m = 0; m < 4; m++)
                        {
                            nav.peph[i].pos[k][m] = nav.peph[j].pos[k][m];
                        }
                        for (m = 0; m < 4; m++)
                        {
                            nav.peph[i].std[k][m] = nav.peph[j].std[k][m];
                        }
                        for (m = 0; m < 4; m++)
                        {
                            nav.peph[i].vel[k][m] = nav.peph[j].vel[k][m];
                        }
                        for (m = 0; m < 4; m++)
                        {
                            nav.peph[i].vst[k][m] = nav.peph[j].vst[k][m];
                        }
                    }
                }
                else if (++i < j)
                {
                    nav.peph[i] = nav.peph[j];
                }
            }
            nav.ne = i + 1;

            GlobalMembersRtkcmn.trace(4, "combpeph: ne=%d\n", nav.ne);
        }
        /* read sp3 precise ephemeris file ---------------------------------------------
        * read sp3 precise ephemeris/clock files and set them to navigation data
        * args   : char   *file       I   sp3-c precise ephemeris file
        *                                 (wind-card * is expanded)
        *          nav_t  *nav        IO  navigation data
        *          int    opt         I   options (1: only observed + 2: only predicted +
        *                                 4: not combined)
        * return : none
        * notes  : see ref [1]
        *          precise ephemeris is appended and combined
        *          nav->peph and nav->ne must by properly initialized before calling the
        *          function
        *          only files with extensions of .sp3, .SP3, .eph* and .EPH* are read
        *-----------------------------------------------------------------------------*/
        public static void readsp3(string file, nav_t nav, int opt)
        {
            FILE fp;
            gtime_t time = new gtime_t();
            double[] bfact = { 0, null };
            int i;
            int j;
            int n;
            int ns;
            int[] sats = new int[DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1];
            string[] efiles = new string[DefineConstants.MAXEXFILE];
            string ext;
            sbyte type = (sbyte)' ';
            string tsys = "";

            GlobalMembersRtkcmn.trace(3, "readpephs: file=%s\n", file);

            for (i = 0; i < DefineConstants.MAXEXFILE; i++)
            {
                //C++ TO C# CONVERTER TODO TASK: The memory management function 'malloc' has no equivalent in C#:
                if (!(efiles[i] = (string)malloc(1024)))
                {
                    for (i--; i >= 0; i--)
                    {
                        //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
                        free(efiles[i]);
                    }
                    return;
                }
            }
            /* expand wild card in file path */
            n = GlobalMembersRtkcmn.expath(file, efiles, DefineConstants.MAXEXFILE);

            for (i = j = 0; i < n; i++)
            {
                if ((ext = StringFunctions.StrRChr(efiles[i], '.')) == 0)
                    continue;

                if (!StringFunctions.StrStr(ext.Substring(1), "sp3") && !StringFunctions.StrStr(ext.Substring(1), ".SP3") && !StringFunctions.StrStr(ext.Substring(1), "eph") && !StringFunctions.StrStr(ext.Substring(1), ".EPH"))
                    continue;

                if ((fp = fopen(efiles[i], "r")) == null)
                {
                    GlobalMembersRtkcmn.trace(2, "sp3 file open error %s\n", efiles[i]);
                    continue;
                }
                /* read sp3 header */
                ns = GlobalMembersPreceph.readsp3h(fp, time, ref type, sats, bfact, ref tsys);

                /* read sp3 body */
                GlobalMembersPreceph.readsp3b(fp, type, ref sats, ns, bfact, ref tsys, j++, opt, nav);

                fclose(fp);
            }
            for (i = 0; i < DefineConstants.MAXEXFILE; i++)
            {
                //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
                free(efiles[i]);
            }

            /* combine precise ephemeris */
            if (nav.ne > 0)
            {
                GlobalMembersPreceph.combpeph(nav, opt);
            }
        }
        /* read satellite antenna parameters -------------------------------------------
        * read satellite antenna parameters
        * args   : char   *file       I   antenna parameter file
        *          gtime_t time       I   time
        *          nav_t  *nav        IO  navigation data
        * return : status (1:ok,0:error)
        * notes  : only support antex format for the antenna parameter file
        *-----------------------------------------------------------------------------*/
        public static int readsap(string file, gtime_t time, nav_t nav)
        {
            pcvs_t pcvs = new pcvs_t();
            pcv_t pcv0 = new pcv_t();
            pcv_t pcv;
            int i;

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: trace(3,"readsap : file=%s time=%s\n",file,time_str(time,0));
            GlobalMembersRtkcmn.trace(3, "readsap : file=%s time=%s\n", file, GlobalMembersRtkcmn.time_str(new gtime_t(time), 0));

            if (GlobalMembersRtkcmn.readpcv(file, pcvs) == 0)
            {
                return 0;
            }

            for (i = 0; i < DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1; i++)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: pcv=searchpcv(i+1,"",time,&pcvs);
                pcv = GlobalMembersRtkcmn.searchpcv(i + 1, "", new gtime_t(time), pcvs);
                nav.pcvs[i] = pcv != null ? pcv : pcv0;
            }
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(pcvs.pcv);
            return 1;
        }
        /* read dcb parameters file --------------------------------------------------*/
        internal static int readdcbf(string file, nav_t nav)
        {
            FILE fp;
            double cbias;
            int sat;
            int type = 0;
            string buff = new string(new char[256]);

            GlobalMembersRtkcmn.trace(3, "readdcbf: file=%s\n", file);

            if ((fp = fopen(file, "r")) == null)
            {
                GlobalMembersRtkcmn.trace(2, "dcb parameters file open error: %s\n", file);
                return 0;
            }
            while (fgets(buff, sizeof(sbyte), fp))
            {

                if (StringFunctions.StrStr(buff, "DIFFERENTIAL (P1-P2) CODE BIASES"))
                {
                    type = 1;
                }
                else if (StringFunctions.StrStr(buff, "DIFFERENTIAL (P1-C1) CODE BIASES"))
                {
                    type = 2;
                }
                else if (StringFunctions.StrStr(buff, "DIFFERENTIAL (P2-C2) CODE BIASES"))
                {
                    type = 3;
                }

                if (type == 0)
                    continue;

                if ((sat = GlobalMembersRtkcmn.satid2no(buff)) == 0 || (cbias = GlobalMembersRtkcmn.str2num(buff, 26, 9)) == 0.0)
                    continue;

                nav.cbias[sat - 1, type - 1] = cbias * 1E-9 * DefineConstants.CLIGHT; // ns -> m
            }
            fclose(fp);

            return 1;
        }
        /* read dcb parameters ---------------------------------------------------------
        * read differential code bias (dcb) parameters
        * args   : char   *file       I   dcb parameters file (wild-card * expanded)
        *          nav_t  *nav        IO  navigation data
        * return : status (1:ok,0:error)
        * notes  : currently only p1-c1 bias of code *.dcb file
        *-----------------------------------------------------------------------------*/
        public static int readdcb(string file, nav_t nav)
        {
            int i;
            int j;
            int n;
            string[] efiles = { 0 };

            GlobalMembersRtkcmn.trace(3, "readdcb : file=%s\n", file);

            for (i = 0; i < DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1; i++)
            {
                for (j = 0; j < 3; j++)
                {
                    nav.cbias[i, j] = 0.0;
                }
            }
            for (i = 0; i < DefineConstants.MAXEXFILE; i++)
            {
                //C++ TO C# CONVERTER TODO TASK: The memory management function 'malloc' has no equivalent in C#:
                if (!(efiles[i] = (string)malloc(1024)))
                {
                    for (i--; i >= 0; i--)
                    {
                        //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
                        free(efiles[i]);
                    }
                    return 0;
                }
            }
            n = GlobalMembersRtkcmn.expath(file, efiles, DefineConstants.MAXEXFILE);

            for (i = 0; i < n; i++)
            {
                GlobalMembersPreceph.readdcbf(efiles[i], nav);
            }
            for (i = 0; i < DefineConstants.MAXEXFILE; i++)
            {
                //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
                free(efiles[i]);
            }

            return 1;
        }
        /* polynomial interpolation by Neville's algorithm ---------------------------*/
        internal static double interppol(double[] x, double[] y, int n)
        {
            int i;
            int j;

            for (j = 1; j < n; j++)
            {
                for (i = 0; i < n - j; i++)
                {
                    y[i] = (x[i + j] * y[i] - x[i] * y[i + 1]) / (x[i + j] - x[i]);
                }
            }
            return y[0];
        }
        /* satellite position by precise ephemeris -----------------------------------*/
        internal static int pephpos(gtime_t time, int sat, nav_t nav, double[] rs, double[] dts, ref double vare, ref double varc)
        {
            double[] t = new double[DefineConstants.NMAX + 1];
            double[,] p = new double[3, DefineConstants.NMAX + 1];
            double[] c = new double[2];
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: double *pos;
            double pos;
            double std = 0.0;
            double[] s = new double[3];
            double sinl;
            double cosl;
            int i;
            int j;
            int k;
            int index;

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: trace(4,"pephpos : time=%s sat=%2d\n",time_str(time,3),sat);
            GlobalMembersRtkcmn.trace(4, "pephpos : time=%s sat=%2d\n", GlobalMembersRtkcmn.time_str(new gtime_t(time), 3), sat);

            rs[0] = rs[1] = rs[2] = dts[0] = 0.0;

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: if (nav->ne<DefineConstants.NMAX+1|| timediff(time,nav->peph[0].time)<-DefineConstants.MAXDTE|| timediff(time,nav->peph[nav->ne-1].time)>DefineConstants.MAXDTE)
            if (nav.ne < DefineConstants.NMAX + 1 || GlobalMembersRtkcmn.timediff(new gtime_t(time), nav.peph[0].time) < -DefineConstants.MAXDTE || GlobalMembersRtkcmn.timediff(new gtime_t(time), nav.peph[nav.ne - 1].time) > DefineConstants.MAXDTE)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: trace(2,"no prec ephem %s sat=%2d\n",time_str(time,0),sat);
                GlobalMembersRtkcmn.trace(2, "no prec ephem %s sat=%2d\n", GlobalMembersRtkcmn.time_str(new gtime_t(time), 0), sat);
                return 0;
            }
            /* binary search */
            for (i = 0, j = nav.ne - 1; i < j;)
            {
                k = (i + j) / 2;
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: if (timediff(nav->peph[k].time,time)<0.0)
                if (GlobalMembersRtkcmn.timediff(nav.peph[k].time, new gtime_t(time)) < 0.0)
                {
                    i = k + 1;
                }
                else
                {
                    j = k;
                }
            }
            index = i <= 0 ? 0 : i - 1;

            /* polynomial interpolation for orbit */
            i = index - (DefineConstants.NMAX + 1) / 2;
            if (i < 0)
            {
                i = 0;
            }
            else if (i + DefineConstants.NMAX >= nav.ne)
            {
                i = nav.ne - DefineConstants.NMAX - 1;
            }

            for (j = 0; j <= DefineConstants.NMAX; j++)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: t[j]=timediff(nav->peph[i+j].time,time);
                t[j] = GlobalMembersRtkcmn.timediff(nav.peph[i + j].time, new gtime_t(time));
                if (GlobalMembersRtkcmn.norm(nav.peph[i + j].pos[sat - 1], 3) <= 0.0)
                {
                    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                    //ORIGINAL LINE: trace(2,"prec ephem outage %s sat=%2d\n",time_str(time,0),sat);
                    GlobalMembersRtkcmn.trace(2, "prec ephem outage %s sat=%2d\n", GlobalMembersRtkcmn.time_str(new gtime_t(time), 0), sat);
                    return 0;
                }
            }
            for (j = 0; j <= DefineConstants.NMAX; j++)
            {
                pos = nav.peph[i + j].pos[sat - 1];
#if false
	//        p[0][j]=pos[0];
	//        p[1][j]=pos[1];
#else
                /* correciton for earh rotation ver.2.4.0 */
                sinl = Math.Sin(DefineConstants.OMGE * t[j]);
                cosl = Math.Cos(DefineConstants.OMGE * t[j]);
                p[0, j] = cosl * pos[0] - sinl * pos[1];
                p[1, j] = sinl * pos[0] + cosl * pos[1];
#endif
                p[2, j] = pos[2];
            }
            for (i = 0; i < 3; i++)
            {
                rs[i] = GlobalMembersPreceph.interppol(t, p[i], DefineConstants.NMAX + 1);
            }
            if (vare != 0)
            {
                for (i = 0; i < 3; i++)
                {
                    s[i] = nav.peph[index].std[sat - 1][i];
                }
                std = GlobalMembersRtkcmn.norm(s, 3);

                /* extrapolation error for orbit */
                if (t[0] > 0.0)
                {
                    std += DefineConstants.EXTERR_EPH * ((t[0]) * (t[0])) / 2.0;
                }
                else if (t[DefineConstants.NMAX] < 0.0)
                {
                    std += DefineConstants.EXTERR_EPH * ((t[DefineConstants.NMAX]) * (t[DefineConstants.NMAX])) / 2.0;
                }
                vare = ((std) * (std));
            }
            /* linear interpolation for clock */
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: t[0]=timediff(time,nav->peph[index].time);
            t[0] = GlobalMembersRtkcmn.timediff(new gtime_t(time), nav.peph[index].time);
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: t[1]=timediff(time,nav->peph[index+1].time);
            t[1] = GlobalMembersRtkcmn.timediff(new gtime_t(time), nav.peph[index + 1].time);
            c[0] = nav.peph[index].pos[sat - 1][3];
            c[1] = nav.peph[index + 1].pos[sat - 1][3];

            if (t[0] <= 0.0)
            {
                if ((dts[0] = c[0]) != 0.0)
                {
                    std = nav.peph[index].std[sat - 1][3] * DefineConstants.CLIGHT - DefineConstants.EXTERR_CLK * t[0];
                }
            }
            else if (t[1] >= 0.0)
            {
                if ((dts[0] = c[1]) != 0.0)
                {
                    std = nav.peph[index + 1].std[sat - 1][3] * DefineConstants.CLIGHT + DefineConstants.EXTERR_CLK * t[1];
                }
            }
            else if (c[0] != 0.0 && c[1] != 0.0)
            {
                dts[0] = (c[1] * t[0] - c[0] * t[1]) / (t[0] - t[1]);
                i = t[0] < -t[1] != 0 ? 0 : 1;
                std = nav.peph[index + i].std[sat - 1][3] + DefineConstants.EXTERR_CLK * Math.Abs(t[i]);
            }
            else
            {
                dts[0] = 0.0;
            }
            if (varc != 0)
            {
                varc = ((std) * (std));
            }
            return 1;
        }
        /* satellite clock by precise clock ------------------------------------------*/
        internal static int pephclk(gtime_t time, int sat, nav_t nav, double[] dts, ref double varc)
        {
            double[] t = new double[2];
            double[] c = new double[2];
            double std;
            int i;
            int j;
            int k;
            int index;

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: trace(4,"pephclk : time=%s sat=%2d\n",time_str(time,3),sat);
            GlobalMembersRtkcmn.trace(4, "pephclk : time=%s sat=%2d\n", GlobalMembersRtkcmn.time_str(new gtime_t(time), 3), sat);

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: if (nav->nc<2|| timediff(time,nav->pclk[0].time)<-DefineConstants.MAXDTE|| timediff(time,nav->pclk[nav->nc-1].time)>DefineConstants.MAXDTE)
            if (nav.nc < 2 || GlobalMembersRtkcmn.timediff(new gtime_t(time), nav.pclk[0].time) < -DefineConstants.MAXDTE || GlobalMembersRtkcmn.timediff(new gtime_t(time), nav.pclk[nav.nc - 1].time) > DefineConstants.MAXDTE)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: trace(3,"no prec clock %s sat=%2d\n",time_str(time,0),sat);
                GlobalMembersRtkcmn.trace(3, "no prec clock %s sat=%2d\n", GlobalMembersRtkcmn.time_str(new gtime_t(time), 0), sat);
                return 1;
            }
            /* binary search */
            for (i = 0, j = nav.nc - 1; i < j;)
            {
                k = (i + j) / 2;
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: if (timediff(nav->pclk[k].time,time)<0.0)
                if (GlobalMembersRtkcmn.timediff(nav.pclk[k].time, new gtime_t(time)) < 0.0)
                {
                    i = k + 1;
                }
                else
                {
                    j = k;
                }
            }
            index = i <= 0 ? 0 : i - 1;

            /* linear interpolation for clock */
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: t[0]=timediff(time,nav->pclk[index].time);
            t[0] = GlobalMembersRtkcmn.timediff(new gtime_t(time), nav.pclk[index].time);
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: t[1]=timediff(time,nav->pclk[index+1].time);
            t[1] = GlobalMembersRtkcmn.timediff(new gtime_t(time), nav.pclk[index + 1].time);
            c[0] = nav.pclk[index].clk[sat - 1][0];
            c[1] = nav.pclk[index + 1].clk[sat - 1][0];

            if (t[0] <= 0.0)
            {
                if ((dts[0] = c[0]) == 0.0)
                {
                    return 0;
                }
                std = nav.pclk[index].std[sat - 1][0] * DefineConstants.CLIGHT - DefineConstants.EXTERR_CLK * t[0];
            }
            else if (t[1] >= 0.0)
            {
                if ((dts[0] = c[1]) == 0.0)
                {
                    return 0;
                }
                std = nav.pclk[index + 1].std[sat - 1][0] * DefineConstants.CLIGHT + DefineConstants.EXTERR_CLK * t[1];
            }
            else if (c[0] != 0.0 && c[1] != 0.0)
            {
                dts[0] = (c[1] * t[0] - c[0] * t[1]) / (t[0] - t[1]);
                i = t[0] < -t[1] != 0 ? 0 : 1;
                std = nav.pclk[index + i].std[sat - 1][0] * DefineConstants.CLIGHT + DefineConstants.EXTERR_CLK * Math.Abs(t[i]);
            }
            else
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: trace(3,"prec clock outage %s sat=%2d\n",time_str(time,0),sat);
                GlobalMembersRtkcmn.trace(3, "prec clock outage %s sat=%2d\n", GlobalMembersRtkcmn.time_str(new gtime_t(time), 0), sat);
                return 0;
            }
            if (varc != 0)
            {
                varc = ((std) * (std));
            }
            return 1;
        }
        /* satellite antenna phase center offset ---------------------------------------
        * compute satellite antenna phase center offset in ecef
        * args   : gtime_t time       I   time (gpst)
        *          double *rs         I   satellite position and velocity (ecef)
        *                                 {x,y,z,vx,vy,vz} (m|m/s)
        *          int    sat         I   satellite number
        *          nav_t  *nav        I   navigation data
        *          double *dant       I   satellite antenna phase center offset (ecef)
        *                                 {dx,dy,dz} (m) (iono-free LC value)
        * return : none
        *-----------------------------------------------------------------------------*/
        public static void satantoff(gtime_t time, double[] rs, int sat, nav_t nav, double[] dant)
        {
            double[] lam = nav.lam[sat - 1];
            pcv_t pcv = nav.pcvs + sat - 1;
            double[] ex = new double[3];
            double[] ey = new double[3];
            double[] ez = new double[3];
            double[] es = new double[3];
            double[] r = new double[3];
            double[] rsun = new double[3];
            double gmst;
            double[] erpv = { 0, null, null, null, null };
            double gamma;
            double C1;
            double C2;
            double dant1;
            double dant2;
            int i;
            int j = 0;
            int k = 1;

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: trace(4,"satantoff: time=%s sat=%2d\n",time_str(time,3),sat);
            GlobalMembersRtkcmn.trace(4, "satantoff: time=%s sat=%2d\n", GlobalMembersRtkcmn.time_str(new gtime_t(time), 3), sat);

            /* sun position in ecef */
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: sunmoonpos(gpst2utc(time),erpv,rsun,null,&gmst);
            GlobalMembersRtkcmn.sunmoonpos(GlobalMembersRtkcmn.gpst2utc(new gtime_t(time)), erpv, ref rsun, null, ref gmst);

            /* unit vectors of satellite fixed coordinates */
            for (i = 0; i < 3; i++)
            {
                r[i] = -rs[i];
            }
            if (GlobalMembersRtkcmn.normv3(r, ez) == 0)
                return;
            for (i = 0; i < 3; i++)
            {
                r[i] = rsun[i] - rs[i];
            }
            if (GlobalMembersRtkcmn.normv3(r, es) == 0)
                return;
            GlobalMembersRtkcmn.cross3(ez, es, r);
            if (GlobalMembersRtkcmn.normv3(r, ey) == 0)
                return;
            GlobalMembersRtkcmn.cross3(ey, ez, ex);

            if (DefineConstants.NFREQ >= 3 && (GlobalMembersRtkcmn.satsys(sat, null) & (DefineConstants.SYS_GAL | DefineConstants.SYS_SBS)))
            {
                k = 2;
            }

            if (DefineConstants.NFREQ < 2 || lam[j] == 0.0 || lam[k] == 0.0)
                return;

            gamma = ((lam[k]) * (lam[k])) / ((lam[j]) * (lam[j]));
            C1 = gamma / (gamma - 1.0);
            C2 = -1.0 / (gamma - 1.0);

            /* iono-free LC */
            for (i = 0; i < 3; i++)
            {
                dant1 = pcv.off[j, 0] * ex[i] + pcv.off[j, 1] * ey[i] + pcv.off[j, 2] * ez[i];
                dant2 = pcv.off[k, 0] * ex[i] + pcv.off[k, 1] * ey[i] + pcv.off[k, 2] * ez[i];
                dant[i] = C1 * dant1 + C2 * dant2;
            }
        }
        /* satellite position/clock by precise ephemeris/clock -------------------------
        * compute satellite position/clock with precise ephemeris/clock
        * args   : gtime_t time       I   time (gpst)
        *          int    sat         I   satellite number
        *          nav_t  *nav        I   navigation data
        *          int    opt         I   sat postion option
        *                                 (0: center of mass, 1: antenna phase center)
        *          double *rs         O   sat position and velocity (ecef)
        *                                 {x,y,z,vx,vy,vz} (m|m/s)
        *          double *dts        O   sat clock {bias,drift} (s|s/s)
        *          double *var        IO  sat position and clock error variance (m)
        *                                 (NULL: no output)
        * return : status (1:ok,0:error or data outage)
        * notes  : clock includes relativistic correction but does not contain code bias
        *          before calling the function, nav->peph, nav->ne, nav->pclk and
        *          nav->nc must be set by calling readsp3(), readrnx() or readrnxt()
        *          if precise clocks are not set, clocks in sp3 are used instead
        *-----------------------------------------------------------------------------*/
        public static int peph2pos(gtime_t time, int sat, nav_t nav, int opt, double[] rs, double[] dts, ref double @var)
        {
            double[] rss = new double[3];
            double[] rst = new double[3];
            double[] dtss = new double[1];
            double[] dtst = new double[1];
            double[] dant = { 0, null, null };
            double vare = 0.0;
            double varc = 0.0;
            double tt = 1E-3;
            int i;

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: trace(4,"peph2pos: time=%s sat=%2d opt=%d\n",time_str(time,3),sat,opt);
            GlobalMembersRtkcmn.trace(4, "peph2pos: time=%s sat=%2d opt=%d\n", GlobalMembersRtkcmn.time_str(new gtime_t(time), 3), sat, opt);

            if (sat <= 0 || DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1 < sat)
            {
                return 0;
            }

            /* satellite position and clock bias */
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: if (!pephpos(time,sat,nav,rss,dtss,&vare,&varc)|| !pephclk(time,sat,nav,dtss,&varc))
            if (GlobalMembersPreceph.pephpos(new gtime_t(time), sat, nav, rss, dtss, ref vare, ref varc) == 0 || GlobalMembersPreceph.pephclk(new gtime_t(time), sat, nav, dtss, ref varc) == 0)
            {
                return 0;
            }

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: time=timeadd(time,tt);
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            time.CopyFrom(GlobalMembersRtkcmn.timeadd(new gtime_t(time), tt));
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: if (!pephpos(time,sat,nav,rst,dtst,null,null)|| !pephclk(time,sat,nav,dtst,null))
            if (GlobalMembersPreceph.pephpos(new gtime_t(time), sat, nav, rst, dtst, null, null) == 0 || GlobalMembersPreceph.pephclk(new gtime_t(time), sat, nav, dtst, null) == 0)
            {
                return 0;
            }

            /* satellite antenna offset correction */
            if (opt != 0)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: satantoff(time,rss,sat,nav,dant);
                GlobalMembersPreceph.satantoff(new gtime_t(time), rss, sat, nav, dant);
            }
            for (i = 0; i < 3; i++)
            {
                rs[i] = rss[i] + dant[i];
                rs[i + 3] = (rst[i] - rss[i]) / tt;
            }
            /* relativistic effect correction */
            if (dtss[0] != 0.0)
            {
                dts[0] = dtss[0] - 2.0 * GlobalMembersRtkcmn.dot(rs, rs + 3, 3) / DefineConstants.CLIGHT / DefineConstants.CLIGHT;
                dts[1] = (dtst[0] - dtss[0]) / tt;
            }
            else // no precise clock
            {
                dts[0] = dts[1] = 0.0;
            }
            if (@var != 0)
            {
                @var = vare + varc;
            }

            return 1;
        }
    }
}
