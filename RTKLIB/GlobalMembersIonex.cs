using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ghGPS.Classes
{
    public static class GlobalMembersIonex
    {
        /*------------------------------------------------------------------------------
        * ionex.c : ionex functions
        *
        *          Copyright (C) 2011-2013 by T.TAKASU, All rights reserved.
        *
        * references:
        *     [1] S.Schear, W.Gurtner and J.Feltens, IONEX: The IONosphere Map EXchange
        *         Format Version 1, February 25, 1998
        *     [2] S.Schaer, R.Markus, B.Gerhard and A.S.Timon, Daily Global Ionosphere
        *         Maps based on GPS Carrier Phase Data Routinely producted by CODE
        *         Analysis Center, Proceeding of the IGS Analysis Center Workshop, 1996
        *
        * version : $Revision:$ $Date:$
        * history : 2011/03/29 1.0 new
        *           2013/03/05 1.1 change api readtec()
        *                          fix problem in case of lat>85deg or lat<-85deg
        *           2014/02/22 1.2 fix problem on compiled as C++
        *-----------------------------------------------------------------------------*/


        /* get index -----------------------------------------------------------------*/
        internal static int getindex(double value, double[] range)
        {
            if (range[2] == 0.0)
            {
                return 0;
            }
            if (range[1] > 0.0 && (value < range[0] || range[1] < value))
            {
                return -1;
            }
            if (range[1] < 0.0 && (value < range[1] || range[0] < value))
            {
                return -1;
            }
            return (int)Math.Floor((value - range[0]) / range[2] + 0.5);
        }
        /* get number of items -------------------------------------------------------*/
        internal static int nitem(double[] range)
        {
            return GlobalMembersIonex.getindex(range[1], range) + 1;
        }
        /* data index (i:lat,j:lon,k:hgt) --------------------------------------------*/
        internal static int dataindex(int i, int j, int k, int[] ndata)
        {
            if (i < 0 || ndata[0] <= i || j < 0 || ndata[1] <= j || k < 0 || ndata[2] <= k)
            {
                return -1;
            }
            return i + ndata[0] * (j + ndata[1] * k);
        }
        /* add tec data to navigation data -------------------------------------------*/
        internal static tec_t addtec(double[] lats, double[] lons, double[] hgts, double rb, nav_t nav)
        {
            tec_t p;
            tec_t nav_tec;
            gtime_t time0 = new gtime_t();
            int i;
            int n;
            int[] ndata = new int[3];

            GlobalMembersRtkcmn.trace(3, "addtec  :\n");

            ndata[0] = GlobalMembersIonex.nitem(lats);
            ndata[1] = GlobalMembersIonex.nitem(lons);
            ndata[2] = GlobalMembersIonex.nitem(hgts);
            if (ndata[0] <= 1 || ndata[1] <= 1 || ndata[2] <= 0)
            {
                return null;
            }

            if (nav.nt >= nav.ntmax)
            {
                nav.ntmax += 256;
                //C++ TO C# CONVERTER TODO TASK: The memory management function 'realloc' has no equivalent in C#:
                if ((nav_tec = (tec_t)realloc(nav.tec, sizeof(tec_t) * nav.ntmax)) == null)
                {
                    GlobalMembersRtkcmn.trace(1, "readionex malloc error ntmax=%d\n", nav.ntmax);
                    //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
                    free(nav.tec);
                    nav.tec = null;
                    nav.nt = nav.ntmax = 0;
                    return null;
                }
                nav.tec = nav_tec;
            }
            p = nav.tec + nav.nt;
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: p->time=time0;
            p.time.CopyFrom(time0);
            p.rb = rb;
            for (i = 0; i < 3; i++)
            {
                p.ndata[i] = ndata[i];
                p.lats[i] = lats[i];
                p.lons[i] = lons[i];
                p.hgts[i] = hgts[i];
            }
            n = ndata[0] * ndata[1] * ndata[2];

            //C++ TO C# CONVERTER TODO TASK: The memory management function 'malloc' has no equivalent in C#:
            if ((p.data = (double)malloc(sizeof(double) * n)) == 0 || (p.rms = (float)malloc(sizeof(float) * n)) == 0F)
            {
                return null;
            }
            for (i = 0; i < n; i++)
            {
                p.data[i] = 0.0;
                p.rms[i] = 0.0f;
            }
            nav.nt++;
            return p;
        }
        /* read ionex dcb aux data ----------------------------------------------------*/
        internal static void readionexdcb(FILE fp, double[] dcb, double[] rms)
        {
            int i;
            int sat;
            string buff = new string(new char[1024]);
            string id = new string(new char[32]);
            string label;

            GlobalMembersRtkcmn.trace(3, "readionexdcb:\n");

            for (i = 0; i < DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1; i++)
            {
                dcb[i] = rms[i] = 0.0;
            }

            while (fgets(buff, sizeof(sbyte), fp))
            {
                if (buff.Length < 60)
                    continue;
                label = buff.Substring(60);

                if (StringFunctions.StrStr(label, "PRN / BIAS / RMS") == label)
                {

                    id = buff.Substring(3, 3);
                    id[3] = '\0';

                    if ((sat = GlobalMembersRtkcmn.satid2no(id)) == 0)
                    {
                        GlobalMembersRtkcmn.trace(2, "ionex invalid satellite: %s\n", id);
                        continue;
                    }
                    dcb[sat - 1] = GlobalMembersRtkcmn.str2num(buff, 6, 10);
                    rms[sat - 1] = GlobalMembersRtkcmn.str2num(buff, 16, 10);
                }
                else if (StringFunctions.StrStr(label, "END OF AUX DATA") == label)
                    break;
            }
        }
        /* read ionex header ---------------------------------------------------------*/
        internal static double readionexh(FILE fp, double[] lats, double[] lons, double[] hgts, ref double rb, ref double nexp, ref double dcb, ref double rms)
        {
            double ver = 0.0;
            string buff = new string(new char[1024]);
            string label;

            GlobalMembersRtkcmn.trace(3, "readionexh:\n");

            while (fgets(buff, sizeof(sbyte), fp))
            {

                if (buff.Length < 60)
                    continue;
                label = buff.Substring(60);

                if (StringFunctions.StrStr(label, "IONEX VERSION / TYPE") == label)
                {
                    if (buff[20] == 'I')
                    {
                        ver = GlobalMembersRtkcmn.str2num(buff, 0, 8);
                    }
                }
                else if (StringFunctions.StrStr(label, "BASE RADIUS") == label)
                {
                    rb = GlobalMembersRtkcmn.str2num(buff, 0, 8);
                }
                else if (StringFunctions.StrStr(label, "HGT1 / HGT2 / DHGT") == label)
                {
                    hgts[0] = GlobalMembersRtkcmn.str2num(buff, 2, 6);
                    hgts[1] = GlobalMembersRtkcmn.str2num(buff, 8, 6);
                    hgts[2] = GlobalMembersRtkcmn.str2num(buff, 14, 6);
                }
                else if (StringFunctions.StrStr(label, "LAT1 / LAT2 / DLAT") == label)
                {
                    lats[0] = GlobalMembersRtkcmn.str2num(buff, 2, 6);
                    lats[1] = GlobalMembersRtkcmn.str2num(buff, 8, 6);
                    lats[2] = GlobalMembersRtkcmn.str2num(buff, 14, 6);
                }
                else if (StringFunctions.StrStr(label, "LON1 / LON2 / DLON") == label)
                {
                    lons[0] = GlobalMembersRtkcmn.str2num(buff, 2, 6);
                    lons[1] = GlobalMembersRtkcmn.str2num(buff, 8, 6);
                    lons[2] = GlobalMembersRtkcmn.str2num(buff, 14, 6);
                }
                else if (StringFunctions.StrStr(label, "EXPONENT") == label)
                {
                    nexp = GlobalMembersRtkcmn.str2num(buff, 0, 6);
                }
                else if (StringFunctions.StrStr(label, "START OF AUX DATA") == label && StringFunctions.StrStr(buff, "DIFFERENTIAL CODE BIASES"))
                {
                    GlobalMembersIonex.readionexdcb(fp, dcb, rms);
                }
                else if (StringFunctions.StrStr(label, "END OF HEADER") == label)
                {
                    return ver;
                }
            }
            return 0.0;
        }
        /* read ionex body -----------------------------------------------------------*/
        internal static int readionexb(FILE fp, double lats, double lons, double hgts, double rb, double nexp, nav_t nav)
        {
            tec_t p = null;
            gtime_t time = new gtime_t();
            double lat;
            double[] lon = new double[3];
            double hgt;
            double x;
            int i;
            int j;
            int k;
            int n;
            int m;
            int index;
            int type = 0;
            string buff = new string(new char[1024]);
            string label = buff.Substring(60);

            GlobalMembersRtkcmn.trace(3, "readionexb:\n");

            while (fgets(buff, sizeof(sbyte), fp))
            {

                if (buff.Length < 60)
                    continue;

                if (StringFunctions.StrStr(label, "START OF TEC MAP") == label)
                {
                    if ((p = GlobalMembersIonex.addtec(lats, lons, hgts, rb, nav)) != null)
                    {
                        type = 1;
                    }
                }
                else if (StringFunctions.StrStr(label, "END OF TEC MAP") == label)
                {
                    type = 0;
                    p = null;
                }
                else if (StringFunctions.StrStr(label, "START OF RMS MAP") == label)
                {
                    type = 2;
                    p = null;
                }
                else if (StringFunctions.StrStr(label, "END OF RMS MAP") == label)
                {
                    type = 0;
                    p = null;
                }
                else if (StringFunctions.StrStr(label, "EPOCH OF CURRENT MAP") == label)
                {
                    if (GlobalMembersRtkcmn.str2time(buff, 0, 36, time) != 0)
                    {
                        GlobalMembersRtkcmn.trace(2, "ionex epoch invalid: %-36.36s\n", buff);
                        continue;
                    }
                    if (type == 2)
                    {
                        for (i = nav.nt - 1; i >= 0; i--)
                        {
                            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                            //ORIGINAL LINE: if (fabs(timediff(time,nav->tec[i].time))>=1.0)
                            if (Math.Abs(GlobalMembersRtkcmn.timediff(new gtime_t(time), nav.tec[i].time)) >= 1.0)
                                continue;
                            p = nav.tec + i;
                            break;
                        }
                    }
                    else if (p != null)
                    {
                        //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                        //ORIGINAL LINE: p->time=time;
                        p.time.CopyFrom(time);
                    }
                }
                else if (StringFunctions.StrStr(label, "LAT/LON1/LON2/DLON/H") == label && p != null)
                {
                    lat = GlobalMembersRtkcmn.str2num(buff, 2, 6);
                    lon[0] = GlobalMembersRtkcmn.str2num(buff, 8, 6);
                    lon[1] = GlobalMembersRtkcmn.str2num(buff, 14, 6);
                    lon[2] = GlobalMembersRtkcmn.str2num(buff, 20, 6);
                    hgt = GlobalMembersRtkcmn.str2num(buff, 26, 6);

                    i = GlobalMembersIonex.getindex(lat, p.lats);
                    k = GlobalMembersIonex.getindex(hgt, p.hgts);
                    n = GlobalMembersIonex.nitem(lon);

                    for (m = 0; m < n; m++)
                    {
                        if (m % 16 == 0 && !fgets(buff, sizeof(sbyte), fp))
                            break;

                        j = GlobalMembersIonex.getindex(lon[0] + lon[2] * m, p.lons);
                        if ((index = GlobalMembersIonex.dataindex(i, j, k, p.ndata)) < 0)
                            continue;

                        if ((x = GlobalMembersRtkcmn.str2num(buff, m % 16 * 5, 5)) == 9999.0)
                            continue;

                        if (type == 1)
                        {
                            p.data[index] = x * Math.Pow(10.0, nexp);
                        }
                        else
                        {
                            p.rms[index] = (float)(x * Math.Pow(10.0, nexp));
                        }
                    }
                }
            }
            return 1;
        }
        /* combine tec grid data -----------------------------------------------------*/
        internal static void combtec(nav_t nav)
        {
            tec_t tmp = new tec_t();
            int i;
            int j;
            int n = 0;

            GlobalMembersRtkcmn.trace(3, "combtec : nav->nt=%d\n", nav.nt);

            for (i = 0; i < nav.nt - 1; i++)
            {
                for (j = i + 1; j < nav.nt; j++)
                {
                    if (GlobalMembersRtkcmn.timediff(nav.tec[j].time, nav.tec[i].time) < 0.0)
                    {
                        tmp = nav.tec[i];
                        nav.tec[i] = nav.tec[j];
                        nav.tec[j] = tmp;
                    }
                }
            }
            for (i = 0; i < nav.nt; i++)
            {
                if (i > 0 && GlobalMembersRtkcmn.timediff(nav.tec[i].time, nav.tec[n - 1].time) == 0.0)
                {
                    //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
                    free(nav.tec[n - 1].data);
                    //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
                    free(nav.tec[n - 1].rms);
                    nav.tec[n - 1] = nav.tec[i];
                    continue;
                }
                nav.tec[n++] = nav.tec[i];
            }
            nav.nt = n;

            GlobalMembersRtkcmn.trace(4, "combtec : nav->nt=%d\n", nav.nt);
        }
        /* read ionex tec grid file ----------------------------------------------------
        * read ionex ionospheric tec grid file
        * args   : char   *file       I   ionex tec grid file
        *                                 (wind-card * is expanded)
        *          nav_t  *nav        IO  navigation data
        *                                 nav->nt, nav->ntmax and nav->tec are modified
        *          int    opt         I   read option (1: no clear of tec data,0:clear)
        * return : none
        * notes  : see ref [1]
        *-----------------------------------------------------------------------------*/
        public static void readtec(string file, nav_t nav, int opt)
        {
            FILE fp;
            double[] lats = { 0, null, null };
            double[] lons = { 0, null, null };
            double[] hgts = { 0, null, null };
            double rb = 0.0;
            double nexp = -1.0;
            double[] dcb = { 0 };
            double[] rms = { 0 };
            int i;
            int n;
            string[] efiles = new string[DefineConstants.MAXEXFILE];

            GlobalMembersRtkcmn.trace(3, "readtec : file=%s\n", file);

            /* clear of tec grid data option */
            if (opt == 0)
            {
                //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
                free(nav.tec);
                nav.tec = null;
                nav.nt = nav.ntmax = 0;
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
                    return;
                }
            }
            /* expand wild card in file path */
            n = GlobalMembersRtkcmn.expath(file, efiles, DefineConstants.MAXEXFILE);

            for (i = 0; i < n; i++)
            {
                if ((fp = fopen(efiles[i], "r")) == null)
                {
                    GlobalMembersRtkcmn.trace(2, "ionex file open error %s\n", efiles[i]);
                    continue;
                }
                /* read ionex header */
                if (GlobalMembersIonex.readionexh(fp, lats, lons, hgts, ref rb, ref nexp, ref dcb, ref rms) <= 0.0)
                {
                    GlobalMembersRtkcmn.trace(2, "ionex file format error %s\n", efiles[i]);
                    continue;
                }
                /* read ionex body */
                GlobalMembersIonex.readionexb(fp, lats, lons, hgts, rb, nexp, nav);

                fclose(fp);
            }
            for (i = 0; i < DefineConstants.MAXEXFILE; i++)
            {
                //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
                free(efiles[i]);
            }

            /* combine tec grid data */
            if (nav.nt > 0)
            {
                GlobalMembersIonex.combtec(nav);
            }

            /* P1-P2 dcb */
            for (i = 0; i < DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1; i++)
            {
                nav.cbias[i, 0] = DefineConstants.CLIGHT * dcb[i] * 1E-9; // ns->m
            }
        }
        /* interpolate tec grid data -------------------------------------------------*/
        //C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on the parameter 'value', so pointers on this parameter are left unchanged:
        internal static int interptec(tec_t tec, int k, double[] posp, double* value, double[] rms)
        {
            double dlat;
            double dlon;
            double a;
            double b;
            double[] d = { 0, null, null, null };
            double[] r = { 0, null, null, null };
            int i;
            int j;
            int n;
            int index;

            GlobalMembersRtkcmn.trace(3, "interptec: k=%d posp=%.2f %.2f\n", k, posp[0] * 180.0 / DefineConstants.PI, posp[1] * 180.0 / DefineConstants.PI);
            *value = rms = 0.0;

            if (tec.lats[2] == 0.0 || tec.lons[2] == 0.0)
            {
                return 0;
            }

            dlat = posp[0] * 180.0 / DefineConstants.PI - tec.lats[0];
            dlon = posp[1] * 180.0 / DefineConstants.PI - tec.lons[0];
            if (tec.lons[2] > 0.0) //  0<=dlon<360
            {
                dlon -= Math.Floor(dlon / 360) * 360.0;
            }
            else // -360<dlon<=0
            {
                dlon += Math.Floor(-dlon / 360) * 360.0;
            }

            a = dlat / tec.lats[2];
            b = dlon / tec.lons[2];
            i = (int)Math.Floor(a);
            a -= i;
            j = (int)Math.Floor(b);
            b -= j;

            /* get gridded tec data */
            for (n = 0; n < 4; n++)
            {
                if ((index = GlobalMembersIonex.dataindex(i + (n % 2), j + (n < 2 ? 0 : 1), k, tec.ndata)) < 0)
                    continue;
                d[n] = tec.data[index];
                r[n] = tec.rms[index];
            }
            if (d[0] > 0.0 && d[1] > 0.0 && d[2] > 0.0 && d[3] > 0.0)
            {

                /* bilinear interpolation (inside of grid) */
                *value = (1.0 - a) * (1.0 - b) * d[0] + a * (1.0 - b) * d[1] + (1.0 - a) * b * d[2] + a * b * d[3];
                *rms = (1.0 - a) * (1.0 - b) * r[0] + a * (1.0 - b) * r[1] + (1.0 - a) * b * r[2] + a * b * r[3];
            }
            /* nearest-neighbour extrapolation (outside of grid) */
            else if (a <= 0.5 && b <= 0.5 && d[0] > 0.0)
            {
                *value = d[0];
                *rms = r[0];
            }
            else if (a > 0.5 && b <= 0.5 && d[1] > 0.0)
            {
                *value = d[1];
                *rms = r[1];
            }
            else if (a <= 0.5 && b > 0.5 && d[2] > 0.0)
            {
                *value = d[2];
                *rms = r[2];
            }
            else if (a > 0.5 && b > 0.5 && d[3] > 0.0)
            {
                *value = d[3];
                *rms = r[3];
            }
            else
            {
                i = 0;
                for (n = 0; n < 4; n++)
                {
                    if (d[n] > 0.0)
                    {
                        i++;
                        *value += d[n];
                        *rms += r[n];
                    }
                }
                if (i == 0)
                {
                    return 0;
                }
                *value /= i;
                *rms /= i;
            }
            return 1;
        }
        /* ionosphere delay by tec grid data -----------------------------------------*/
        //C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on the parameter 'delay', so pointers on this parameter are left unchanged:
        //C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on the parameter '@var', so pointers on this parameter are left unchanged:
        internal static int iondelay(gtime_t time, tec_t tec, double[] pos, double[] azel, int opt, double* delay, double* @var)
        {
            double fact = 40.30E16 / DefineConstants.FREQ1 / DefineConstants.FREQ1; // tecu->L1 iono (m)
            double fs;
            double[] posp = { 0, null, null };
            double vtec;
            double rms;
            double hion;
            double rp;
            int i;

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: trace(3,"iondelay: time=%s pos=%.1f %.1f azel=%.1f %.1f\n",time_str(time,0), pos[0]*180.0/DefineConstants.PI,pos[1]*180.0/DefineConstants.PI,azel[0]*180.0/DefineConstants.PI,azel[1]*180.0/DefineConstants.PI);
            GlobalMembersRtkcmn.trace(3, "iondelay: time=%s pos=%.1f %.1f azel=%.1f %.1f\n", GlobalMembersRtkcmn.time_str(new gtime_t(time), 0), pos[0] * 180.0 / DefineConstants.PI, pos[1] * 180.0 / DefineConstants.PI, azel[0] * 180.0 / DefineConstants.PI, azel[1] * 180.0 / DefineConstants.PI);

            *delay = @var = 0.0;

            for (i = 0; i < tec.ndata[2]; i++) // for a layer
            {

                hion = tec.hgts[0] + tec.hgts[2] * i;

                /* ionospheric pierce point position */
                fs = GlobalMembersRtkcmn.ionppp(pos, azel, tec.rb, hion, posp);

                if ((opt & 2) != 0)
                {
                    /* modified single layer mapping function (M-SLM) ref [2] */
                    rp = tec.rb / (tec.rb + hion) * Math.Sin(0.9782 * (DefineConstants.PI / 2.0 - azel[1]));
                    fs = 1.0 / Math.Sqrt(1.0 - rp * rp);
                }
                if ((opt & 1) != 0)
                {
                    /* earth rotation correction (sun-fixed coordinate) */
                    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                    //ORIGINAL LINE: posp[1]+=2.0 *DefineConstants.PI *timediff(time,tec->time)/86400.0;
                    posp[1] += 2.0 * DefineConstants.PI * GlobalMembersRtkcmn.timediff(new gtime_t(time), new gtime_t(tec.time)) / 86400.0;
                }
                /* interpolate tec grid data */
                if (GlobalMembersIonex.interptec(tec, i, posp, vtec, rms) == 0)
                {
                    return 0;
                }

                *delay += fact * fs * vtec;
                *@var += fact * fact * fs * fs * rms * rms;
            }
            GlobalMembersRtkcmn.trace(4, "iondelay: delay=%7.2f std=%6.2f\n", *delay, Math.Sqrt(*@var));

            return 1;
        }
        /* ionosphere model by tec grid data -------------------------------------------
        * compute ionospheric delay by tec grid data
        * args   : gtime_t time     I   time (gpst)
        *          nav_t  *nav      I   navigation data
        *          double *pos      I   receiver position {lat,lon,h} (rad,m)
        *          double *azel     I   azimuth/elevation angle {az,el} (rad)
        *          int    opt       I   model option
        *                                bit0: 0:earth-fixed,1:sun-fixed
        *                                bit1: 0:single-layer,1:modified single-layer
        *          double *delay    O   ionospheric delay (L1) (m)
        *          double *var      O   ionospheric dealy (L1) variance (m^2)
        * return : status (1:ok,0:error)
        * notes  : before calling the function, read tec grid data by calling readtec()
        *          return ok with delay=0 and var=VAR_NOTEC if el<MIN_EL or h<MIN_HGT
        *-----------------------------------------------------------------------------*/
        public static int iontec(gtime_t time, nav_t nav, double[] pos, double[] azel, int opt, ref double delay, ref double @var)
        {
            double[] dels = new double[2];
            double[] vars = new double[2];
            double a;
            double tt;
            int i;
            int[] stat = new int[2];

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: trace(3,"iontec  : time=%s pos=%.1f %.1f azel=%.1f %.1f\n",time_str(time,0), pos[0]*180.0/DefineConstants.PI,pos[1]*180.0/DefineConstants.PI,azel[0]*180.0/DefineConstants.PI,azel[1]*180.0/DefineConstants.PI);
            GlobalMembersRtkcmn.trace(3, "iontec  : time=%s pos=%.1f %.1f azel=%.1f %.1f\n", GlobalMembersRtkcmn.time_str(new gtime_t(time), 0), pos[0] * 180.0 / DefineConstants.PI, pos[1] * 180.0 / DefineConstants.PI, azel[0] * 180.0 / DefineConstants.PI, azel[1] * 180.0 / DefineConstants.PI);

            if (azel[1] < DefineConstants.MIN_EL || pos[2] < DefineConstants.MIN_HGT)
            {
                delay = 0.0;
                @var = (30.0) * (30.0);
                return 1;
            }
            for (i = 0; i < nav.nt; i++)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: if (timediff(nav->tec[i].time,time)>0.0)
                if (GlobalMembersRtkcmn.timediff(nav.tec[i].time, new gtime_t(time)) > 0.0)
                    break;
            }
            if (i == 0 || i >= nav.nt)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: trace(2,"%s: tec grid out of period\n",time_str(time,0));
                GlobalMembersRtkcmn.trace(2, "%s: tec grid out of period\n", GlobalMembersRtkcmn.time_str(new gtime_t(time), 0));
                return 0;
            }
            if ((tt = GlobalMembersRtkcmn.timediff(nav.tec[i].time, nav.tec[i - 1].time)) == 0.0)
            {
                GlobalMembersRtkcmn.trace(2, "tec grid time interval error\n");
                return 0;
            }
            /* ionospheric delay by tec grid data */
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: stat[0]=iondelay(time,nav->tec+i-1,pos,azel,opt,dels,vars);
            stat[0] = GlobalMembersIonex.iondelay(new gtime_t(time), nav.tec + i - 1, pos, azel, opt, dels, vars);
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: stat[1]=iondelay(time,nav->tec+i,pos,azel,opt,dels+1,vars+1);
            stat[1] = GlobalMembersIonex.iondelay(new gtime_t(time), nav.tec + i, pos, azel, opt, dels + 1, vars + 1);

            if (stat[0] == 0 && stat[1] == 0)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: trace(2,"%s: tec grid out of area pos=%6.2f %7.2f azel=%6.1f %5.1f\n", time_str(time,0),pos[0]*180.0/DefineConstants.PI,pos[1]*180.0/DefineConstants.PI,azel[0]*180.0/DefineConstants.PI,azel[1]*180.0/DefineConstants.PI);
                GlobalMembersRtkcmn.trace(2, "%s: tec grid out of area pos=%6.2f %7.2f azel=%6.1f %5.1f\n", GlobalMembersRtkcmn.time_str(new gtime_t(time), 0), pos[0] * 180.0 / DefineConstants.PI, pos[1] * 180.0 / DefineConstants.PI, azel[0] * 180.0 / DefineConstants.PI, azel[1] * 180.0 / DefineConstants.PI);
                return 0;
            }
            if (stat[0] != 0 && stat[1] != 0) // linear interpolation by time
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: a=timediff(time,nav->tec[i-1].time)/tt;
                a = GlobalMembersRtkcmn.timediff(new gtime_t(time), nav.tec[i - 1].time) / tt;
                delay = dels[0] * (1.0 - a) + dels[1] * a;
                @var = vars[0] * (1.0 - a) + vars[1] * a;
            }
            else if (stat[0] != 0) // nearest-neighbour extrapolation by time
            {
                delay = dels[0];
                @var = vars[0];
            }
            else
            {
                delay = dels[1];
                @var = vars[1];
            }
            GlobalMembersRtkcmn.trace(3, "iontec  : delay=%5.2f std=%5.2f\n", delay, Math.Sqrt(@var));
            return 1;
        }
    }
}
