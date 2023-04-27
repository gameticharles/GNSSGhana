using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ghGPS.Classes
{
    public static class GlobalMembersSolution
    {
        /*------------------------------------------------------------------------------
        * solution.c : solution functions
        *
        *          Copyright (C) 2007-2015 by T.TAKASU, All rights reserved.
        *
        * reference :
        *     [1] National Marine Electronic Association and International Marine
        *         Electronics Association, NMEA 0183 version 4.10, August 1, 2012
        *
        * version : $Revision: 1.1 $ $Date: 2008/07/17 21:48:06 $
        * history : 2007/11/03  1.0 new
        *           2009/01/05  1.1  add function outsols(), outsolheads(),
        *                            setsolformat(), outsolexs, outsolex
        *           2009/04/02  1.2  add dummy fields in NMEA mesassage
        *                            fix bug to format lat/lon as deg-min-sec
        *           2009/04/14  1.3  add age and ratio field to solution
        *           2009/11/25  1.4  add function readsolstat()
        *           2010/02/14  1.5  fix bug on output of gpstime at week boundary
        *           2010/07/05  1.6  added api:
        *                                initsolbuf(),freesolbuf(),addsol(),getsol(),
        *                                inputsol(),outprcopts(),outprcopt()
        *                            modified api:
        *                                readsol(),readsolt(),readsolstat(),
        *                                readsolstatt(),outsolheads(),outsols(),
        *                                outsolexs(),outsolhead(),outsol(),outsolex(),
        *                                outnmea_rmc(),outnmea_gga(),outnmea_gsa(),
        *                                outnmea_gsv()
        *                            deleted api:
        *                                setsolopt(),setsolformat()
        *           2010/08/14  1.7  fix bug on initialize solution buffer (2.4.0_p2)
        *                            suppress enu-solution if base pos not available
        *                            (2.4.0_p3)
        *           2010/08/16  1.8  suppress null record if solution is not avalilable
        *                            (2.4.0_p4)
        *           2011/01/23  1.9  fix bug on reading nmea solution data
        *                            add api freesolstatbuf()
        *           2012/02/05  1.10 fix bug on output nmea gpgsv
        *           2013/02/18  1.11 support nmea GLGSA,GAGSA,GLCSV,GACSV sentence
        *           2013/09/01  1.12 fix bug on presentation of nmea time tag
        *           2015/02/11  1.13 fix bug on checksum of $GLGSA and $GAGSA
        *                            fix bug on satellite id of $GAGSA
        *-----------------------------------------------------------------------------*/


        internal const string rcsid = "$Id: solution.c,v 1.1 2008/07/17 21:48:06 ttaka Exp $";



        internal readonly int[] solq_nmea = { DefineConstants.SOLQ_NONE, DefineConstants.SOLQ_SINGLE, DefineConstants.SOLQ_DGPS, DefineConstants.SOLQ_PPP, DefineConstants.SOLQ_FIX, DefineConstants.SOLQ_FLOAT, DefineConstants.SOLQ_DR, DefineConstants.SOLQ_NONE, DefineConstants.SOLQ_NONE, DefineConstants.SOLQ_NONE }; // nmea quality flags to rtklib sol quality
                                                                                                                                                                                                                                                                                                                             /* solution option to field separator ----------------------------------------*/
        internal static string opt2sep(solopt_t opt)
        {
            if (!*opt.sep)
            {
                return " ";
            }
            else if (!string.Compare(opt.sep, "\\t"))
            {
                return "\t";
            }
            return opt.sep;
        }
        /* separate fields -----------------------------------------------------------*/
        internal static int tonum(ref string buff, string sep, double[] v)
        {
            int n;
            int len = (int)sep.Length;
            string p;
            string q;

            for (p = buff, n = 0; n < DefineConstants.MAXFIELD; p = q.Substring(len))
            {
                if ((q = StringFunctions.StrStr(p, sep)) != 0)
                {
                    q = (sbyte)'\0';
                }
                if (p != 0)
                {
                    v[n++] = Convert.ToDouble(p);
                }
                if (q == 0)
                    break;
            }
            return n;
        }
        /* sqrt of covariance --------------------------------------------------------*/
        internal static double sqvar(double covar)
        {
            return covar < 0.0 ? -Math.Sqrt(-covar) : Math.Sqrt(covar);
        }
        /* convert ddmm.mm in nmea format to deg -------------------------------------*/
        internal static double dmm2deg(double dmm)
        {
            return Math.Floor(dmm / 100.0) + Math.IEEERemainder(dmm, 100.0) / 60.0;
        }
        /* convert time in nmea format to time ---------------------------------------*/
        internal static void septime(double t, ref double t1, ref double t2, ref double t3)
        {
            t1 = Math.Floor(t / 10000.0);
            t -= t1 * 10000.0;
            t2 = Math.Floor(t / 100.0);
            t3 = t - t2 * 100.0;
        }
        /* solution to covariance ----------------------------------------------------*/
        internal static void soltocov(sol_t sol, double[] P)
        {
            P[0] = sol.qr[0]; // xx or ee
            P[4] = sol.qr[1]; // yy or nn
            P[8] = sol.qr[2]; // zz or uu
            P[1] = P[3] = sol.qr[3]; // xy or en
            P[5] = P[7] = sol.qr[4]; // yz or nu
            P[2] = P[6] = sol.qr[5]; // zx or ue
        }
        /* covariance to solution ----------------------------------------------------*/
        internal static void covtosol(double[] P, sol_t sol)
        {
            sol.qr[0] = (float)P[0]; // xx or ee
            sol.qr[1] = (float)P[4]; // yy or nn
            sol.qr[2] = (float)P[8]; // zz or uu
            sol.qr[3] = (float)P[1]; // xy or en
            sol.qr[4] = (float)P[5]; // yz or nu
            sol.qr[5] = (float)P[2]; // zx or ue
        }
        /* decode nmea gprmc: recommended minumum data for gps -----------------------*/
        internal static int decode_nmearmc(string[] val, int n, sol_t sol)
        {
            double tod = 0.0;
            double lat = 0.0;
            double lon = 0.0;
            double vel = 0.0;
            double dir = 0.0;
            double date = 0.0;
            double ang = 0.0;
            double[] ep = new double[6];
            double[] pos = { 0, null, null };
            sbyte act = (sbyte)' ';
            sbyte ns = (sbyte)'N';
            sbyte ew = (sbyte)'E';
            sbyte mew = (sbyte)'E';
            sbyte mode = (sbyte)'A';
            int i;

            GlobalMembersRtkcmn.trace(4, "decode_nmearmc: n=%d\n", n);

            for (i = 0; i < n; i++)
            {
                switch (i)
                {
                    case 0: // time in utc (hhmmss)
                        tod = Convert.ToDouble(val[i]);
                        break;
                    case 1: // A=active,V=void
                        act = val[i];
                        break;
                    case 2: // latitude (ddmm.mmm)
                        lat = Convert.ToDouble(val[i]);
                        break;
                    case 3: // N=north,S=south
                        ns = val[i];
                        break;
                    case 4: // longitude (dddmm.mmm)
                        lon = Convert.ToDouble(val[i]);
                        break;
                    case 5: // E=east,W=west
                        ew = val[i];
                        break;
                    case 6: // speed (knots)
                        vel = Convert.ToDouble(val[i]);
                        break;
                    case 7: // track angle (deg)
                        dir = Convert.ToDouble(val[i]);
                        break;
                    case 8: // date (ddmmyy)
                        date = Convert.ToDouble(val[i]);
                        break;
                    case 9: // magnetic variation
                        ang = Convert.ToDouble(val[i]);
                        break;
                    case 10: // E=east,W=west
                        mew = val[i];
                        break;
                    case 11: // mode indicator (>nmea 2)
                        mode = val[i];
                        break;
                        /* A=autonomous,D=differential */
                        /* E=estimated,N=not valid,S=simulator */
                }
            }
            if ((act != 'A' && act != 'V') || (ns != 'N' && ns != 'S') || (ew != 'E' && ew != 'W'))
            {
                GlobalMembersRtkcmn.trace(2, "invalid nmea gprmc format\n");
                return 0;
            }
            pos[0] = (ns == 'S' ? -1.0 : 1.0) * GlobalMembersSolution.dmm2deg(lat) * DefineConstants.PI / 180.0;
            pos[1] = (ew == 'W' ? -1.0 : 1.0) * GlobalMembersSolution.dmm2deg(lon) * DefineConstants.PI / 180.0;
            GlobalMembersSolution.septime(date, ref ep + 2, ref ep + 1, ref ep);
            GlobalMembersSolution.septime(tod, ref ep + 3, ref ep + 4, ref ep + 5);
            ep[0] += ep[0] < 80.0 ? 2000.0 : 1900.0;
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: sol->time=utc2gpst(epoch2time(ep));
            sol.time.CopyFrom(GlobalMembersRtkcmn.utc2gpst(GlobalMembersRtkcmn.epoch2time(ep)));
            GlobalMembersRtkcmn.pos2ecef(pos, sol.rr);
            sol.stat = mode == 'D' ? DefineConstants.SOLQ_DGPS : DefineConstants.SOLQ_SINGLE;
            sol.ns = 0;

            sol.type = 0; // postion type = xyz

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: trace(5,"decode_nmearmc: %s rr=%.3f %.3f %.3f stat=%d ns=%d vel=%.2f dir=%.0f ang=%.0f mew=%c mode=%c\n", time_str(sol->time,0),sol->rr[0],sol->rr[1],sol->rr[2],sol->stat,sol->ns, vel,dir,ang,mew,mode);
            GlobalMembersRtkcmn.trace(5, "decode_nmearmc: %s rr=%.3f %.3f %.3f stat=%d ns=%d vel=%.2f dir=%.0f ang=%.0f mew=%c mode=%c\n", GlobalMembersRtkcmn.time_str(new gtime_t(sol.time), 0), sol.rr[0], sol.rr[1], sol.rr[2], sol.stat, sol.ns, vel, dir, ang, mew, mode);

            return 1;
        }
        /* decode nmea gpgga: fix information ----------------------------------------*/
        internal static int decode_nmeagga(string[] val, int n, sol_t sol)
        {
            gtime_t time = new gtime_t();
            double tod = 0.0;
            double lat = 0.0;
            double lon = 0.0;
            double hdop = 0.0;
            double alt = 0.0;
            double msl = 0.0;
            double[] ep = new double[6];
            double tt;
            double[] pos = { 0, null, null };
            sbyte ns = (sbyte)'N';
            sbyte ew = (sbyte)'E';
            sbyte ua = (sbyte)' ';
            sbyte um = (sbyte)' ';
            int i;
            int solq = 0;
            int nrcv = 0;

            GlobalMembersRtkcmn.trace(4, "decode_nmeagga: n=%d\n", n);

            for (i = 0; i < n; i++)
            {
                switch (i)
                {
                    case 0: // time in utc (hhmmss)
                        tod = Convert.ToDouble(val[i]);
                        break;
                    case 1: // latitude (ddmm.mmm)
                        lat = Convert.ToDouble(val[i]);
                        break;
                    case 2: // N=north,S=south
                        ns = val[i];
                        break;
                    case 3: // longitude (dddmm.mmm)
                        lon = Convert.ToDouble(val[i]);
                        break;
                    case 4: // E=east,W=west
                        ew = val[i];
                        break;
                    case 5: // fix quality
                        solq = Convert.ToInt32(val[i]);
                        break;
                    case 6: // # of satellite tracked
                        nrcv = Convert.ToInt32(val[i]);
                        break;
                    case 7: // hdop
                        hdop = Convert.ToDouble(val[i]);
                        break;
                    case 8: // altitude in msl
                        alt = Convert.ToDouble(val[i]);
                        break;
                    case 9: // unit (M)
                        ua = val[i];
                        break;
                    case 10: // height of geoid
                        msl = Convert.ToDouble(val[i]);
                        break;
                    case 11: // unit (M)
                        um = val[i];
                        break;
                }
            }
            if ((ns != 'N' && ns != 'S') || (ew != 'E' && ew != 'W'))
            {
                GlobalMembersRtkcmn.trace(2, "invalid nmea gpgga format\n");
                return 0;
            }
            if (sol.time.time == 0.0)
            {
                GlobalMembersRtkcmn.trace(2, "no date info for nmea gpgga\n");
                return 0;
            }
            pos[0] = (ns == 'N' ? 1.0 : -1.0) * GlobalMembersSolution.dmm2deg(lat) * DefineConstants.PI / 180.0;
            pos[1] = (ew == 'E' ? 1.0 : -1.0) * GlobalMembersSolution.dmm2deg(lon) * DefineConstants.PI / 180.0;
            pos[2] = alt + msl;

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: time2epoch(sol->time,ep);
            GlobalMembersRtkcmn.time2epoch(new gtime_t(sol.time), ep);
            GlobalMembersSolution.septime(tod, ref ep + 3, ref ep + 4, ref ep + 5);
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: time=utc2gpst(epoch2time(ep));
            time.CopyFrom(GlobalMembersRtkcmn.utc2gpst(GlobalMembersRtkcmn.epoch2time(ep)));
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: tt=timediff(time,sol->time);
            tt = GlobalMembersRtkcmn.timediff(new gtime_t(time), new gtime_t(sol.time));
            if (tt < -43200.0)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                //ORIGINAL LINE: sol->time=timeadd(time, 86400.0);
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                sol.time.CopyFrom(GlobalMembersRtkcmn.timeadd(new gtime_t(time), 86400.0));
            }
            else if (tt > 43200.0)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                //ORIGINAL LINE: sol->time=timeadd(time,-86400.0);
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                sol.time.CopyFrom(GlobalMembersRtkcmn.timeadd(new gtime_t(time), -86400.0));
            }
            else
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                //ORIGINAL LINE: sol->time=time;
                sol.time.CopyFrom(time);
            }
            GlobalMembersRtkcmn.pos2ecef(pos, sol.rr);
            sol.stat = 0 <= solq && solq <= 8 ? solq_nmea[solq] : DefineConstants.SOLQ_NONE;
            sol.ns = nrcv;

            sol.type = 0; // postion type = xyz

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: trace(5,"decode_nmeagga: %s rr=%.3f %.3f %.3f stat=%d ns=%d hdop=%.1f ua=%c um=%c\n", time_str(sol->time,0),sol->rr[0],sol->rr[1],sol->rr[2],sol->stat,sol->ns, hdop,ua,um);
            GlobalMembersRtkcmn.trace(5, "decode_nmeagga: %s rr=%.3f %.3f %.3f stat=%d ns=%d hdop=%.1f ua=%c um=%c\n", GlobalMembersRtkcmn.time_str(new gtime_t(sol.time), 0), sol.rr[0], sol.rr[1], sol.rr[2], sol.stat, sol.ns, hdop, ua, um);

            return 1;
        }
        /* decode nmea ---------------------------------------------------------------*/
        internal static int decode_nmea(ref string buff, sol_t sol)
        {
            string p;
            string q;
            string[] val = new string[DefineConstants.MAXFIELD];
            int n = 0;

            GlobalMembersRtkcmn.trace(4, "decode_nmea: buff=%s\n", buff);

            /* parse fields */
            for (p = buff; p && n < DefineConstants.MAXFIELD; p = q.Substring(1))
            {
                if ((q = StringFunctions.StrChr(p, ',')) != 0 || (q = StringFunctions.StrChr(p, '*')) != 0)
                {
                    val[n++] = p;
                    q = (sbyte)'\0';
                }
                else
                    break;
            }
            /* decode nmea sentence */
            if (!string.Compare(val[0], "$GPRMC"))
            {
                return GlobalMembersSolution.decode_nmearmc(val + 1, n - 1, sol);
            }
            else if (!string.Compare(val[0], "$GPGGA"))
            {
                return GlobalMembersSolution.decode_nmeagga(val + 1, n - 1, sol);
            }
            return 0;
        }
        /* decode solution time ------------------------------------------------------*/
        internal static string decode_soltime(ref string buff, solopt_t opt, gtime_t time)
        {
            double[] v = new double[DefineConstants.MAXFIELD];
            //C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on this variable, so pointers on this variable are left unchanged:
            sbyte* p;
            string q;
            string s = " ";
            int n;
            int len;

            GlobalMembersRtkcmn.trace(4, "decode_soltime:\n");

            if (!string.Compare(opt.sep, "\\t"))
            {
                s = "\t";
            }
            else if (*opt.sep)
            {
                s = opt.sep;
            }
            len = (int)s.Length;

            /* yyyy/mm/dd hh:mm:ss or yyyy mm dd hh:mm:ss */
            if (sscanf(buff, "%lf/%lf/%lf %lf:%lf:%lf", v, v + 1, v + 2, v + 3, v + 4, v + 5) >= 6)
            {
                if (v[0] < 100.0)
                {
                    v[0] += v[0] < 80.0 ? 2000.0 : 1900.0;
                }
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                //ORIGINAL LINE: *time=epoch2time(v);
                time.CopyFrom(GlobalMembersRtkcmn.epoch2time(v));
                if (opt.times == DefineConstants.TIMES_UTC)
                {
                    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                    //ORIGINAL LINE: *time=utc2gpst(*time);
                    time.CopyFrom(GlobalMembersRtkcmn.utc2gpst(time));
                }
                else if (opt.times == DefineConstants.TIMES_JST)
                {
                    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                    //ORIGINAL LINE: *time=utc2gpst(timeadd(*time,-9 *3600.0));
                    time.CopyFrom(GlobalMembersRtkcmn.utc2gpst(GlobalMembersRtkcmn.timeadd(time, -9 * 3600.0)));
                }
                if ((p = StringFunctions.StrChr(buff, ':')) == 0 || (p = StringFunctions.StrChr(p + 1, ':')) == 0)
                {
                    return null;
                }
                for (p++; char.IsDigit((int)*p) || *p == '.';)
                {
                    p++;
                }
                return p + len;
            }
            if (opt.posf == DefineConstants.SOLF_GSIF)
            {
                if (sscanf(buff, "%lf %lf %lf %lf:%lf:%lf", v, v + 1, v + 2, v + 3, v + 4, v + 5) < 6)
                {
                    return null;
                }
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                //ORIGINAL LINE: *time=timeadd(epoch2time(v),-12.0 *3600.0);
                time.CopyFrom(GlobalMembersRtkcmn.timeadd(GlobalMembersRtkcmn.epoch2time(v), -12.0 * 3600.0));
                if ((p = StringFunctions.StrChr(buff, ':')) == 0 || (p = StringFunctions.StrChr(p + 1, ':')) == 0)
                {
                    return null;
                }
                for (p++; char.IsDigit((int)*p) || *p == '.';)
                {
                    p++;
                }
                return p + len;
            }
            /* wwww ssss */
            for (p = buff, n = 0; n < 2; p = q.Substring(len))
            {
                if ((q = StringFunctions.StrStr(p, s)) != 0)
                {
                    q = (sbyte)'\0';
                }
                if (*p)
                {
                    v[n++] = Convert.ToDouble(p);
                }
                if (q == 0)
                    break;
            }
            if (n >= 2 && 0.0 <= v[0] && v[0] <= 3000.0 && 0.0 <= v[1] && v[1] < 604800.0)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                //ORIGINAL LINE: *time=gpst2time((int)v[0],v[1]);
                time.CopyFrom(GlobalMembersRtkcmn.gpst2time((int)v[0], v[1]));
                return p;
            }
            return null;
        }
        /* decode x/y/z-ecef ---------------------------------------------------------*/
        internal static int decode_solxyz(ref string buff, solopt_t opt, sol_t sol)
        {
            double[] val = new double[DefineConstants.MAXFIELD];
            double[] P = { 0, null, null, null, null, null, null, null, null };
            int i = 0;
            int j;
            int n;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: const sbyte *sep=opt2sep(opt);
            sbyte sep = GlobalMembersSolution.opt2sep(opt);

            GlobalMembersRtkcmn.trace(4, "decode_solxyz:\n");

            if ((n = GlobalMembersSolution.tonum(ref buff, sep, val)) < 3)
            {
                return 0;
            }

            for (j = 0; j < 3; j++)
            {
                sol.rr[j] = val[i++]; // xyz
            }
            if (i < n)
            {
                sol.stat = (byte)val[i++];
            }
            if (i < n)
            {
                sol.ns = (byte)val[i++];
            }
            if (i + 3 < n)
            {
                P[0] = val[i] * val[i]; // sdx
                i++;
                P[4] = val[i] * val[i]; // sdy
                i++;
                P[8] = val[i] * val[i]; // sdz
                i++;
                if (i + 3 < n)
                {
                    P[1] = P[3] = ((val[i]) < 0.0 ? -(val[i]) * (val[i]) : (val[i]) * (val[i]));
                    i++;
                    P[5] = P[7] = ((val[i]) < 0.0 ? -(val[i]) * (val[i]) : (val[i]) * (val[i]));
                    i++;
                    P[2] = P[6] = ((val[i]) < 0.0 ? -(val[i]) * (val[i]) : (val[i]) * (val[i]));
                    i++;
                }
                GlobalMembersSolution.covtosol(P, sol);
            }
            if (i < n)
            {
                sol.age = (float)val[i++];
            }
            if (i < n)
            {
                sol.ratio = (float)val[i];
            }

            sol.type = 0; // postion type = xyz

            if (DefineConstants.MAXSOLQ < sol.stat)
            {
                sol.stat = DefineConstants.SOLQ_NONE;
            }
            return 1;
        }
        /* decode lat/lon/height -----------------------------------------------------*/
        internal static int decode_solllh(ref string buff, solopt_t opt, sol_t sol)
        {
            double[] val = new double[DefineConstants.MAXFIELD];
            double[] pos = new double[3];
            double[] Q = { 0, null, null, null, null, null, null, null, null };
            double[] P = new double[9];
            int i = 0;
            int n;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: const sbyte *sep=opt2sep(opt);
            sbyte sep = GlobalMembersSolution.opt2sep(opt);

            GlobalMembersRtkcmn.trace(4, "decode_solllh:\n");

            n = GlobalMembersSolution.tonum(ref buff, sep, val);

            if (opt.degf == 0)
            {
                if (n < 3)
                {
                    return 0;
                }
                pos[0] = val[i++] * DefineConstants.PI / 180.0; // lat/lon/hgt (ddd.ddd)
                pos[1] = val[i++] * DefineConstants.PI / 180.0;
                pos[2] = val[i++];
            }
            else
            {
                if (n < 7)
                {
                    return 0;
                }
                pos[0] = GlobalMembersRtkcmn.dms2deg(val) * DefineConstants.PI / 180.0; // lat/lon/hgt (ddd mm ss)
                pos[1] = GlobalMembersRtkcmn.dms2deg(val + 3) * DefineConstants.PI / 180.0;
                pos[2] = val[6];
                i += 7;
            }
            GlobalMembersRtkcmn.pos2ecef(pos, sol.rr);
            if (i < n)
            {
                sol.stat = (byte)val[i++];
            }
            if (i < n)
            {
                sol.ns = (byte)val[i++];
            }
            if (i + 3 < n)
            {
                Q[4] = val[i] * val[i]; // sdn
                i++;
                Q[0] = val[i] * val[i]; // sde
                i++;
                Q[8] = val[i] * val[i]; // sdu
                i++;
                if (i + 3 < n)
                {
                    Q[1] = Q[3] = ((val[i]) < 0.0 ? -(val[i]) * (val[i]) : (val[i]) * (val[i]));
                    i++;
                    Q[2] = Q[6] = ((val[i]) < 0.0 ? -(val[i]) * (val[i]) : (val[i]) * (val[i]));
                    i++;
                    Q[5] = Q[7] = ((val[i]) < 0.0 ? -(val[i]) * (val[i]) : (val[i]) * (val[i]));
                    i++;
                }
                GlobalMembersRtkcmn.covecef(pos, Q, ref P);
                GlobalMembersSolution.covtosol(P, sol);
            }
            if (i < n)
            {
                sol.age = (float)val[i++];
            }
            if (i < n)
            {
                sol.ratio = (float)val[i];
            }

            sol.type = 0; // postion type = xyz

            if (DefineConstants.MAXSOLQ < sol.stat)
            {
                sol.stat = DefineConstants.SOLQ_NONE;
            }
            return 1;
        }
        /* decode e/n/u-baseline -----------------------------------------------------*/
        internal static int decode_solenu(ref string buff, solopt_t opt, sol_t sol)
        {
            double[] val = new double[DefineConstants.MAXFIELD];
            double[] Q = { 0, null, null, null, null, null, null, null, null };
            int i = 0;
            int j;
            int n;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: const sbyte *sep=opt2sep(opt);
            sbyte sep = GlobalMembersSolution.opt2sep(opt);

            GlobalMembersRtkcmn.trace(4, "decode_solenu:\n");

            if ((n = GlobalMembersSolution.tonum(ref buff, sep, val)) < 3)
            {
                return 0;
            }

            for (j = 0; j < 3; j++)
            {
                sol.rr[j] = val[i++]; // enu
            }
            if (i < n)
            {
                sol.stat = (byte)val[i++];
            }
            if (i < n)
            {
                sol.ns = (byte)val[i++];
            }
            if (i + 3 < n)
            {
                Q[0] = val[i] * val[i]; // sde
                i++;
                Q[4] = val[i] * val[i]; // sdn
                i++;
                Q[8] = val[i] * val[i]; // sdu
                i++;
                if (i + 3 < n)
                {
                    Q[1] = Q[3] = ((val[i]) < 0.0 ? -(val[i]) * (val[i]) : (val[i]) * (val[i]));
                    i++;
                    Q[5] = Q[7] = ((val[i]) < 0.0 ? -(val[i]) * (val[i]) : (val[i]) * (val[i]));
                    i++;
                    Q[2] = Q[6] = ((val[i]) < 0.0 ? -(val[i]) * (val[i]) : (val[i]) * (val[i]));
                    i++;
                }
                GlobalMembersSolution.covtosol(Q, sol);
            }
            if (i < n)
            {
                sol.age = (float)val[i++];
            }
            if (i < n)
            {
                sol.ratio = (float)val[i];
            }

            sol.type = 1; // postion type = enu

            if (DefineConstants.MAXSOLQ < sol.stat)
            {
                sol.stat = DefineConstants.SOLQ_NONE;
            }
            return 1;
        }
        /* decode gsi f solution -----------------------------------------------------*/
        internal static int decode_solgsi(ref string buff, solopt_t opt, sol_t sol)
        {
            double[] val = new double[DefineConstants.MAXFIELD];
            int i = 0;
            int j;

            GlobalMembersRtkcmn.trace(4, "decode_solgsi:\n");

            if (GlobalMembersSolution.tonum(ref buff, " ", val) < 3)
            {
                return 0;
            }

            for (j = 0; j < 3; j++)
            {
                sol.rr[j] = val[i++]; // xyz
            }
            sol.stat = DefineConstants.SOLQ_FIX;
            return 1;
        }
        /* decode solution position --------------------------------------------------*/
        internal static int decode_solpos(ref string buff, solopt_t opt, sol_t sol)
        {
            sol_t sol0 = new sol_t({ 0 });
            string p = buff;

            GlobalMembersRtkcmn.trace(4, "decode_solpos: buff=%s\n", buff);

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: *sol=sol0;
            sol.CopyFrom(sol0);

            /* decode solution time */
            if ((p = GlobalMembersSolution.decode_soltime(ref p, opt, sol.time)) == 0)
            {
                return 0;
            }
            /* decode solution position */
            switch (opt.posf)
            {
                case DefineConstants.SOLF_XYZ:
                    return GlobalMembersSolution.decode_solxyz(ref p, opt, sol);
                case DefineConstants.SOLF_LLH:
                    return GlobalMembersSolution.decode_solllh(ref p, opt, sol);
                case DefineConstants.SOLF_ENU:
                    return GlobalMembersSolution.decode_solenu(ref p, opt, sol);
                case DefineConstants.SOLF_GSIF:
                    return GlobalMembersSolution.decode_solgsi(ref p, opt, sol);
            }
            return 0;
        }
        /* decode reference position -------------------------------------------------*/
        internal static void decode_refpos(ref string buff, solopt_t opt, double[] rb)
        {
            double[] val = new double[DefineConstants.MAXFIELD];
            double[] pos = new double[3];
            int i;
            int n;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: const sbyte *sep=opt2sep(opt);
            sbyte sep = GlobalMembersSolution.opt2sep(opt);

            GlobalMembersRtkcmn.trace(3, "decode_refpos: buff=%s\n", buff);

            if ((n = GlobalMembersSolution.tonum(ref buff, sep, val)) < 3)
                return;

            if (opt.posf == DefineConstants.SOLF_XYZ) // xyz
            {
                for (i = 0; i < 3; i++)
                {
                    rb[i] = val[i];
                }
            }
            else if (opt.degf == 0) // lat/lon/hgt (ddd.ddd)
            {
                pos[0] = val[0] * DefineConstants.PI / 180.0;
                pos[1] = val[1] * DefineConstants.PI / 180.0;
                pos[2] = val[2];
                GlobalMembersRtkcmn.pos2ecef(pos, rb);
            }
            else if (opt.degf == 1 && n >= 7) // lat/lon/hgt (ddd mm ss)
            {
                pos[0] = GlobalMembersRtkcmn.dms2deg(val) * DefineConstants.PI / 180.0;
                pos[1] = GlobalMembersRtkcmn.dms2deg(val + 3) * DefineConstants.PI / 180.0;
                pos[2] = val[6];
                GlobalMembersRtkcmn.pos2ecef(pos, rb);
            }
        }
        /* decode solution -----------------------------------------------------------*/
        internal static int decode_sol(ref string buff, solopt_t opt, sol_t sol, ref double rb)
        {
            string p;

            GlobalMembersRtkcmn.trace(4, "decode_sol: buff=%s\n", buff);

            if (!string.Compare(buff, 0, DefineConstants.COMMENTH, 0, 1)) // reference position
            {
                if (!StringFunctions.StrStr(buff, "ref pos") && !StringFunctions.StrStr(buff, "slave pos"))
                {
                    return 0;
                }
                if ((p = StringFunctions.StrChr(buff, ':')) == 0)
                {
                    return 0;
                }
                GlobalMembersSolution.decode_refpos(ref p.Substring(1), opt, rb);
                return 0;
            }
            if (!string.Compare(buff, 0, "$GP", 0, 3)) // decode nmea
            {
                if (GlobalMembersSolution.decode_nmea(ref buff, sol) == 0)
                {
                    return 0;
                }

                /* for time update only */
                if (opt.posf != DefineConstants.SOLF_NMEA && !string.Compare(buff, 0, "$GPRMC", 0, 6))
                {
                    return 2;
                }
            }
            else // decode position record
            {
                if (GlobalMembersSolution.decode_solpos(ref buff, opt, sol) == 0)
                {
                    return 0;
                }
            }
            return 1;
        }
        /* decode solution options ---------------------------------------------------*/
        internal static void decode_solopt(ref string buff, solopt_t opt)
        {
            string p;

            GlobalMembersRtkcmn.trace(4, "decode_solhead: buff=%s\n", buff);

            if (string.Compare(buff, 0, DefineConstants.COMMENTH, 0, 1) && string.Compare(buff, 0, ", ", 1))
                return;

            if (StringFunctions.StrStr(buff, "GPST"))
            {
                opt.times = DefineConstants.TIMES_GPST;
            }
            else if (StringFunctions.StrStr(buff, "UTC"))
            {
                opt.times = DefineConstants.TIMES_UTC;
            }
            else if (StringFunctions.StrStr(buff, "JST"))
            {
                opt.times = DefineConstants.TIMES_JST;
            }

            if ((p = StringFunctions.StrStr(buff, "x-ecef(m)")) != 0)
            {
                opt.posf = DefineConstants.SOLF_XYZ;
                opt.degf = 0;
                opt.sep = p.Substring(9, 1);
                opt.sep[1] = '\0';
            }
            else if ((p = StringFunctions.StrStr(buff, "latitude(d'\")")) != 0)
            {
                opt.posf = DefineConstants.SOLF_LLH;
                opt.degf = 1;
                opt.sep = p.Substring(14, 1);
                opt.sep[1] = '\0';
            }
            else if ((p = StringFunctions.StrStr(buff, "latitude(deg)")) != 0)
            {
                opt.posf = DefineConstants.SOLF_LLH;
                opt.degf = 0;
                opt.sep = p.Substring(13, 1);
                opt.sep[1] = '\0';
            }
            else if ((p = StringFunctions.StrStr(buff, "e-baseline(m)")) != 0)
            {
                opt.posf = DefineConstants.SOLF_ENU;
                opt.degf = 0;
                opt.sep = p.Substring(13, 1);
                opt.sep[1] = '\0';
            }
            else if ((p = StringFunctions.StrStr(buff, "+SITE/INF")) != 0) // gsi f2/f3 solution
            {
                opt.times = DefineConstants.TIMES_GPST;
                opt.posf = DefineConstants.SOLF_GSIF;
                opt.degf = 0;
                opt.sep = " ";
            }
        }
        /* read solution option ------------------------------------------------------*/
        internal static void readsolopt(FILE fp, solopt_t opt)
        {
            string buff = new string(new char[DefineConstants.MAXSOLMSG + 1]);
            int i;

            GlobalMembersRtkcmn.trace(3, "readsolopt:\n");

            for (i = 0; fgets(buff, sizeof(sbyte), fp) && i < 100; i++) // only 100 lines
            {

                /* decode solution options */
                GlobalMembersSolution.decode_solopt(ref buff, opt);
            }
        }
        /* input solution data from stream ---------------------------------------------
        * input solution data from stream
        * args   : unsigned char data I stream data
        *          gtime_t ts       I  start time (ts.time==0: from start)
        *          gtime_t te       I  end time   (te.time==0: to end)
        *          double tint      I  time interval (0: all)
        *          int    qflag     I  quality flag  (0: all)
        *          solbuf_t *solbuf IO solution buffer
        * return : status (1:solution received,0:no solution,-1:disconnect received)
        *-----------------------------------------------------------------------------*/
        public static int inputsol(byte data, gtime_t ts, gtime_t te, double tint, int qflag, solopt_t opt, solbuf_t solbuf)
        {
            sol_t sol = new sol_t({ 0 });
            int stat;

            GlobalMembersRtkcmn.trace(4, "inputsol: data=0x%02x\n", data);

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: sol.time=solbuf->time;
            sol.time.CopyFrom(solbuf.time);

            if (data == '$' || (!isprint(data) && data != '\r' && data != '\n')) // sync header
            {
                solbuf.nb = 0;
            }
            solbuf.buff[solbuf.nb++] = data;
            if (data != '\n' && solbuf.nb < DefineConstants.MAXSOLMSG) // sync trailer
            {
                return 0;
            }

            solbuf.buff[solbuf.nb] = (byte)'\0';
            solbuf.nb = 0;

            /* check disconnect message */
            if (!string.Compare((string)solbuf.buff, DefineConstants.MSG_DISCONN))
            {
                GlobalMembersRtkcmn.trace(3, "disconnect received\n");
                return -1;
            }
            /* decode solution */
            if ((stat = GlobalMembersSolution.decode_sol(ref (string)solbuf.buff, opt, sol, ref solbuf.rb)) > 0)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                //ORIGINAL LINE: solbuf->time=sol.time;
                solbuf.time.CopyFrom(sol.time); // update current time
            }
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: if (stat!=1||!screent(sol.time,ts,te,tint)||(qflag&&sol.stat!=qflag))
            if (stat != 1 || GlobalMembersRtkcmn.screent(new gtime_t(sol.time), new gtime_t(ts), new gtime_t(te), tint) == 0 || (qflag != 0 && sol.stat != qflag))
            {
                return 0;
            }
            /* add solution to solution buffer */
            return GlobalMembersSolution.addsol(solbuf, sol);
        }
        /* read solution data --------------------------------------------------------*/
        internal static int readsoldata(FILE fp, gtime_t ts, gtime_t te, double tint, int qflag, solopt_t opt, solbuf_t solbuf)
        {
            int c;

            GlobalMembersRtkcmn.trace(3, "readsoldata:\n");

            while ((c = fgetc(fp)) != EOF)
            {

                /* input solution */
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: inputsol((byte)c,ts,te,tint,qflag,opt,solbuf);
                GlobalMembersSolution.inputsol((byte)c, new gtime_t(ts), new gtime_t(te), tint, qflag, opt, solbuf);
            }
            return solbuf.n > 0;
        }
        /* compare solution data -----------------------------------------------------*/
        internal static int cmpsol(object p1, object p2)
        {
            sol_t q1 = (sol_t)p1;
            sol_t q2 = (sol_t)p2;
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: double tt=timediff(q1->time,q2->time);
            double tt = GlobalMembersRtkcmn.timediff(new gtime_t(q1.time), new gtime_t(q2.time));
            return tt < -0.0 ? -1 : (tt > 0.0 ? 1 : 0);
        }
        /* sort solution data --------------------------------------------------------*/
        internal static int sort_solbuf(solbuf_t solbuf)
        {
            sol_t solbuf_data;

            GlobalMembersRtkcmn.trace(4, "sort_solbuf: n=%d\n", solbuf.n);

            if (solbuf.n <= 0)
            {
                return 0;
            }

            //C++ TO C# CONVERTER TODO TASK: The memory management function 'realloc' has no equivalent in C#:
            if ((solbuf_data = (sol_t)realloc(solbuf.data, sizeof(sol_t) * solbuf.n)) == null)
            {
                GlobalMembersRtkcmn.trace(1, "sort_solbuf: memory allocation error\n");
                //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
                free(solbuf.data);
                solbuf.data = null;
                solbuf.n = solbuf.nmax = 0;
                return 0;
            }
            solbuf.data = solbuf_data;
            qsort(solbuf.data, solbuf.n, sizeof(sol_t), GlobalMembersSolution.cmpsol);
            solbuf.nmax = solbuf.n;
            solbuf.start = 0;
            solbuf.end = solbuf.n - 1;
            return 1;
        }
        /* read solutions data from solution files -------------------------------------
        * read solution data from soluiton files
        * args   : char   *files[]  I  solution files
        *          int    nfile     I  number of files
        *         (gtime_t ts)      I  start time (ts.time==0: from start)
        *         (gtime_t te)      I  end time   (te.time==0: to end)
        *         (double tint)     I  time interval (0: all)
        *         (int    qflag)    I  quality flag  (0: all)
        *          solbuf_t *solbuf O  solution buffer
        * return : status (1:ok,0:no data or error)
        *-----------------------------------------------------------------------------*/
        public static int readsolt(string[] files, int nfile, gtime_t ts, gtime_t te, double tint, int qflag, solbuf_t solbuf)
        {
            FILE fp;
            solopt_t opt = GlobalMembersRtkcmn.solopt_default;
            int i;

            GlobalMembersRtkcmn.trace(3, "readsolt: nfile=%d\n", nfile);

            GlobalMembersSolution.initsolbuf(solbuf, 0, 0);

            for (i = 0; i < nfile; i++)
            {
                if ((fp = fopen(files[i], "rb")) == null)
                {
                    GlobalMembersRtkcmn.trace(1, "readsolt: file open error %s\n", files[i]);
                    continue;
                }
                /* read solution options in header */
                GlobalMembersSolution.readsolopt(fp, opt);
                rewind(fp);

                /* read solution data */
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: if (!readsoldata(fp,ts,te,tint,qflag,&opt,solbuf))
                if (GlobalMembersSolution.readsoldata(fp, new gtime_t(ts), new gtime_t(te), tint, qflag, opt, solbuf) == 0)
                {
                    GlobalMembersRtkcmn.trace(1, "readsolt: no solution in %s\n", files[i]);
                }
                fclose(fp);
            }
            return GlobalMembersSolution.sort_solbuf(solbuf);
        }
        public static int readsol(string[] files, int nfile, solbuf_t sol)
        {
            gtime_t time = new gtime_t();

            GlobalMembersRtkcmn.trace(3, "readsol: nfile=%d\n", nfile);

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: return readsolt(files,nfile,time,time,0.0,0,sol);
            return GlobalMembersSolution.readsolt(files, nfile, new gtime_t(time), new gtime_t(time), 0.0, 0, sol);
        }
        /* add solution data to solution buffer ----------------------------------------
        * add solution data to solution buffer
        * args   : solbuf_t *solbuf IO solution buffer
        *          sol_t  *sol      I  solution data
        * return : status (1:ok,0:error)
        *-----------------------------------------------------------------------------*/
        //C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on the parameter 'solbuf', so pointers on this parameter are left unchanged:
        //C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on the parameter 'sol', so pointers on this parameter are left unchanged:
        public static int addsol(solbuf_t* solbuf, sol_t* sol)
        {
            sol_t solbuf_data;

            GlobalMembersRtkcmn.trace(4, "addsol:\n");

            if (solbuf.cyclic != 0) // ring buffer
            {
                if (solbuf.nmax <= 1)
                {
                    return 0;
                }
                solbuf.data[solbuf.end] = *sol;
                if (++solbuf.end >= solbuf.nmax)
                {
                    solbuf.end = 0;
                }
                if (solbuf.start == solbuf.end)
                {
                    if (++solbuf.start >= solbuf.nmax)
                    {
                        solbuf.start = 0;
                    }
                }
                else
                {
                    solbuf.n++;
                }

                return 1;
            }
            if (solbuf.n >= solbuf.nmax)
            {
                solbuf.nmax = solbuf.nmax == 0 ? 8192 : solbuf.nmax * 2;
                //C++ TO C# CONVERTER TODO TASK: The memory management function 'realloc' has no equivalent in C#:
                if ((solbuf_data = (sol_t)realloc(solbuf.data, sizeof(sol_t) * solbuf.nmax)) == null)
                {
                    GlobalMembersRtkcmn.trace(1, "addsol: memory allocation error\n");
                    //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
                    free(solbuf.data);
                    solbuf.data = null;
                    solbuf.n = solbuf.nmax = 0;
                    return 0;
                }
                solbuf.data = solbuf_data;
            }
            solbuf.data[solbuf.n++] = *sol;
            return 1;
        }
        /* get solution data from solution buffer --------------------------------------
        * get solution data by index from solution buffer
        * args   : solbuf_t *solbuf I  solution buffer
        *          int    index     I  index of solution (0...)
        * return : solution data pointer (NULL: no solution, out of range)
        *-----------------------------------------------------------------------------*/
        public static sol_t getsol(solbuf_t solbuf, int index)
        {
            GlobalMembersRtkcmn.trace(4, "getsol: index=%d\n", index);

            if (index < 0 || solbuf.n <= index)
            {
                return null;
            }
            if ((index = solbuf.start + index) >= solbuf.nmax)
            {
                index -= solbuf.nmax;
            }
            return solbuf.data + index;
        }
        /* initialize solution buffer --------------------------------------------------
        * initialize position solutions
        * args   : solbuf_t *solbuf I  solution buffer
        *          int    cyclic    I  solution data buffer type (0:linear,1:cyclic)
        *          int    nmax      I  initial number of solution data
        * return : status (1:ok,0:error)
        *-----------------------------------------------------------------------------*/
        public static void initsolbuf(solbuf_t solbuf, int cyclic, int nmax)
        {
            gtime_t time0 = new gtime_t();

            GlobalMembersRtkcmn.trace(3, "initsolbuf: cyclic=%d nmax=%d\n", cyclic, nmax);

            solbuf.n = solbuf.nmax = solbuf.start = solbuf.end = 0;
            solbuf.cyclic = cyclic;
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: solbuf->time=time0;
            solbuf.time.CopyFrom(time0);
            solbuf.data = null;
            if (cyclic != 0)
            {
                if (nmax <= 2)
                {
                    nmax = 2;
                }
                //C++ TO C# CONVERTER TODO TASK: The memory management function 'malloc' has no equivalent in C#:
                if ((solbuf.data = malloc(sizeof(sol_t) * nmax)) == null)
                {
                    GlobalMembersRtkcmn.trace(1, "initsolbuf: memory allocation error\n");
                    return;
                }
                solbuf.nmax = nmax;
            }
        }
        /* free solution ---------------------------------------------------------------
        * free memory for solution buffer
        * args   : solbuf_t *solbuf I  solution buffer
        * return : none
        *-----------------------------------------------------------------------------*/
        public static void freesolbuf(solbuf_t solbuf)
        {
            GlobalMembersRtkcmn.trace(3, "freesolbuf: n=%d\n", solbuf.n);

            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(solbuf.data);
            solbuf.n = solbuf.nmax = solbuf.start = solbuf.end = 0;
            solbuf.data = null;
        }
        public static void freesolstatbuf(solstatbuf_t solstatbuf)
        {
            GlobalMembersRtkcmn.trace(3, "freesolstatbuf: n=%d\n", solstatbuf.n);

            solstatbuf.n = solstatbuf.nmax = 0;
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(solstatbuf.data);
            solstatbuf.data = null;
        }
        /* compare solution status ---------------------------------------------------*/
        internal static int cmpsolstat(object p1, object p2)
        {
            solstat_t q1 = (solstat_t)p1;
            solstat_t q2 = (solstat_t)p2;
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: double tt=timediff(q1->time,q2->time);
            double tt = GlobalMembersRtkcmn.timediff(new gtime_t(q1.time), new gtime_t(q2.time));
            return tt < -0.0 ? -1 : (tt > 0.0 ? 1 : 0);
        }
        /* sort solution data --------------------------------------------------------*/
        internal static int sort_solstat(solstatbuf_t statbuf)
        {
            solstat_t statbuf_data;

            GlobalMembersRtkcmn.trace(4, "sort_solstat: n=%d\n", statbuf.n);

            if (statbuf.n <= 0)
            {
                return 0;
            }

            //C++ TO C# CONVERTER TODO TASK: The memory management function 'realloc' has no equivalent in C#:
            if ((statbuf_data = realloc(statbuf.data, sizeof(solstat_t) * statbuf.n)) == null)
            {
                GlobalMembersRtkcmn.trace(1, "sort_solstat: memory allocation error\n");
                //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
                free(statbuf.data);
                statbuf.data = null;
                statbuf.n = statbuf.nmax = 0;
                return 0;
            }
            statbuf.data = statbuf_data;
            qsort(statbuf.data, statbuf.n, sizeof(solstat_t), GlobalMembersSolution.cmpsolstat);
            statbuf.nmax = statbuf.n;
            return 1;
        }
        /* decode solution status ----------------------------------------------------*/
        internal static int decode_solstat(ref string buff, solstat_t stat)
        {
            const solstat_t stat0 = new solstat_t({ 0 });
            double tow;
            double az;
            double el;
            double resp;
            double resc;
            int n;
            int week;
            int sat;
            int frq;
            int vsat;
            int snr;
            int fix;
            int slip;
            int @lock;
            int outc;
            int slipc;
            int rejc;
            string id = "";
            //C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on this variable, so pointers on this variable are left unchanged:
            sbyte* p;

            GlobalMembersRtkcmn.trace(4, "decode_solstat: buff=%s\n", buff);

            if (StringFunctions.StrStr(buff, "$SAT") != buff)
            {
                return 0;
            }

            for (p = buff; *p; p++)
            {
                if (*p == ',')
                {
                    *p = (sbyte)' ';
                }
            }

            n = sscanf(buff, "$SAT%d%lf%s%d%lf%lf%lf%lf%d%d%d%d%d%d%d%d", week, tow, id, frq, az, el, resp, resc, vsat, snr, fix, slip, @lock, outc, slipc, rejc);

            if (n < 15)
            {
                GlobalMembersRtkcmn.trace(2, "invalid format of solution status: %s\n", buff);
                return 0;
            }
            if ((sat = GlobalMembersRtkcmn.satid2no(id)) <= 0)
            {
                GlobalMembersRtkcmn.trace(2, "invalid satellite in solution status: %s\n", id);
                return 0;
            }
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: *stat=stat0;
            stat.CopyFrom(stat0);
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: stat->time=gpst2time(week,tow);
            stat.time.CopyFrom(GlobalMembersRtkcmn.gpst2time(week, tow));
            stat.sat = (byte)sat;
            stat.frq = (byte)frq;
            stat.az = (float)(az * DefineConstants.PI / 180.0);
            stat.el = (float)(el * DefineConstants.PI / 180.0);
            stat.resp = (float)resp;
            stat.resc = (float)resc;
            stat.flag = (byte)((vsat << 5) + (slip << 3) + fix);
            stat.snr = (byte)(snr * 4.0 + 0.5);
            stat.@lock = (ushort)@lock;
            stat.outc = (ushort)outc;
            stat.slipc = (ushort)slipc;
            stat.rejc = (ushort)rejc;
            return 1;
        }
        /* add solution status data --------------------------------------------------*/
        internal static void addsolstat(solstatbuf_t statbuf, solstat_t stat)
        {
            solstat_t statbuf_data;

            GlobalMembersRtkcmn.trace(4, "addsolstat:\n");

            if (statbuf.n >= statbuf.nmax)
            {
                statbuf.nmax = statbuf.nmax == 0 ? 8192 : statbuf.nmax * 2;
                //C++ TO C# CONVERTER TODO TASK: The memory management function 'realloc' has no equivalent in C#:
                if ((statbuf_data = (solstat_t)realloc(statbuf.data, sizeof(solstat_t) * statbuf.nmax)) == null)
                {
                    GlobalMembersRtkcmn.trace(1, "addsolstat: memory allocation error\n");
                    //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
                    free(statbuf.data);
                    statbuf.data = null;
                    statbuf.n = statbuf.nmax = 0;
                    return;
                }
                statbuf.data = statbuf_data;
            }
            statbuf.data[statbuf.n++] = stat;
        }
        /* read solution status data -------------------------------------------------*/
        internal static int readsolstatdata(FILE fp, gtime_t ts, gtime_t te, double tint, solstatbuf_t statbuf)
        {
            solstat_t stat = new solstat_t({ 0 });
            string buff = new string(new char[DefineConstants.MAXSOLMSG + 1]);

            GlobalMembersRtkcmn.trace(3, "readsolstatdata:\n");

            while (fgets(buff, sizeof(sbyte), fp))
            {

                /* decode solution status */
                if (GlobalMembersSolution.decode_solstat(ref buff, stat) == 0)
                    continue;

                /* add solution to solution buffer */
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: if (screent(stat.time,ts,te,tint))
                if (GlobalMembersRtkcmn.screent(new gtime_t(stat.time), new gtime_t(ts), new gtime_t(te), tint) != 0)
                {
                    GlobalMembersSolution.addsolstat(statbuf, stat);
                }
            }
            return statbuf.n > 0;
        }
        /* read solution status --------------------------------------------------------
        * read solution status from solution status files
        * args   : char   *files[]  I  solution status files
        *          int    nfile     I  number of files
        *         (gtime_t ts)      I  start time (ts.time==0: from start)
        *         (gtime_t te)      I  end time   (te.time==0: to end)
        *         (double tint)     I  time interval (0: all)
        *          solstatbuf_t *statbuf O  solution status buffer
        * return : status (1:ok,0:no data or error)
        *-----------------------------------------------------------------------------*/
        public static int readsolstatt(string[] files, int nfile, gtime_t ts, gtime_t te, double tint, solstatbuf_t statbuf)
        {
            FILE fp;
            string path = new string(new char[1024]);
            int i;

            GlobalMembersRtkcmn.trace(3, "readsolstatt: nfile=%d\n", nfile);

            statbuf.n = statbuf.nmax = 0;
            statbuf.data = null;

            for (i = 0; i < nfile; i++)
            {
                path = string.Format("{0}.stat", files[i]);
                if ((fp = fopen(path, "r")) == null)
                {
                    GlobalMembersRtkcmn.trace(1, "readsolstatt: file open error %s\n", path);
                    continue;
                }
                /* read solution status data */
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: if (!readsolstatdata(fp,ts,te,tint,statbuf))
                if (GlobalMembersSolution.readsolstatdata(fp, new gtime_t(ts), new gtime_t(te), tint, statbuf) == 0)
                {
                    GlobalMembersRtkcmn.trace(1, "readsolt: no solution in %s\n", path);
                }
                fclose(fp);
            }
            return GlobalMembersSolution.sort_solstat(statbuf);
        }
        public static int readsolstat(string[] files, int nfile, solstatbuf_t statbuf)
        {
            gtime_t time = new gtime_t();

            GlobalMembersRtkcmn.trace(3, "readsolstat: nfile=%d\n", nfile);

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: return readsolstatt(files,nfile,time,time,0.0,statbuf);
            return GlobalMembersSolution.readsolstatt(files, nfile, new gtime_t(time), new gtime_t(time), 0.0, statbuf);
        }
        /* output solution as the form of x/y/z-ecef ---------------------------------*/
        internal static int outecef(ref byte buff, string s, sol_t sol, solopt_t opt)
        {
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: const sbyte *sep=opt2sep(opt);
            sbyte sep = GlobalMembersSolution.opt2sep(opt);
            //C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on this variable, so pointers on this variable are left unchanged:
            sbyte* p = (string)buff;

            GlobalMembersRtkcmn.trace(3, "outecef:\n");

            p += sprintf(p, "%s%s%14.4f%s%14.4f%s%14.4f%s%3d%s%3d%s%8.4f%s%8.4f%s%8.4f%s%8.4f%s%8.4f%s%8.4f%s%6.2f%s%6.1f\n", s, sep, sol.rr[0], sep, sol.rr[1], sep, sol.rr[2], sep, sol.stat, sep, sol.ns, sep, ((sol.qr[0]) < 0.0 ? 0.0 : Math.Sqrt(sol.qr[0])), sep, ((sol.qr[1]) < 0.0 ? 0.0 : Math.Sqrt(sol.qr[1])), sep, ((sol.qr[2]) < 0.0 ? 0.0 : Math.Sqrt(sol.qr[2])), sep, GlobalMembersSolution.sqvar(sol.qr[3]), sep, GlobalMembersSolution.sqvar(sol.qr[4]), sep, GlobalMembersSolution.sqvar(sol.qr[5]), sep, sol.age, sep, sol.ratio);
            return p - (string)buff;
        }
        /* output solution as the form of lat/lon/height -----------------------------*/
        internal static int outpos(ref byte buff, string s, sol_t sol, solopt_t opt)
        {
            double[] pos = new double[3];
            double[] dms1 = new double[3];
            double[] dms2 = new double[3];
            double[] P = new double[9];
            double[] Q = new double[9];
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: const sbyte *sep=opt2sep(opt);
            sbyte sep = GlobalMembersSolution.opt2sep(opt);
            string p = (string)buff;

            GlobalMembersRtkcmn.trace(3, "outpos  :\n");

            GlobalMembersRtkcmn.ecef2pos(sol.rr, pos);
            GlobalMembersSolution.soltocov(sol, P);
            GlobalMembersRtkcmn.covenu(pos, P, ref Q);
            if (opt.height == 1) // geodetic height
            {
                pos[2] -= GlobalMembersGeoid.geoidh(pos);
            }
            if (opt.degf != 0)
            {
                GlobalMembersRtkcmn.deg2dms(pos[0] * 180.0 / DefineConstants.PI, dms1);
                GlobalMembersRtkcmn.deg2dms(pos[1] * 180.0 / DefineConstants.PI, dms2);
                p += sprintf(p, "%s%s%4.0f%s%02.0f%s%08.5f%s%4.0f%s%02.0f%s%08.5f", s, sep, dms1[0], sep, dms1[1], sep, dms1[2], sep, dms2[0], sep, dms2[1], sep, dms2[2]);
            }
            else
            {
                p += sprintf(p, "%s%s%14.9f%s%14.9f", s, sep, pos[0] * 180.0 / DefineConstants.PI, sep, pos[1] * 180.0 / DefineConstants.PI);
            }
            p += sprintf(p, "%s%10.4f%s%3d%s%3d%s%8.4f%s%8.4f%s%8.4f%s%8.4f%s%8.4f%s%8.4f%s%6.2f%s%6.1f\n", sep, pos[2], sep, sol.stat, sep, sol.ns, sep, ((Q[4]) < 0.0 ? 0.0 : Math.Sqrt(Q[4])), sep, ((Q[0]) < 0.0 ? 0.0 : Math.Sqrt(Q[0])), sep, ((Q[8]) < 0.0 ? 0.0 : Math.Sqrt(Q[8])), sep, GlobalMembersSolution.sqvar(Q[1]), sep, GlobalMembersSolution.sqvar(Q[2]), sep, GlobalMembersSolution.sqvar(Q[5]), sep, sol.age, sep, sol.ratio);
            return p - (string)buff;
        }
        /* output solution as the form of e/n/u-baseline -----------------------------*/
        internal static int outenu(ref byte buff, string s, sol_t sol, double[] rb, solopt_t opt)
        {
            double[] pos = new double[3];
            double[] rr = new double[3];
            double[] enu = new double[3];
            double[] P = new double[9];
            double[] Q = new double[9];
            int i;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: const sbyte *sep=opt2sep(opt);
            sbyte sep = GlobalMembersSolution.opt2sep(opt);
            //C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on this variable, so pointers on this variable are left unchanged:
            sbyte* p = (string)buff;

            GlobalMembersRtkcmn.trace(3, "outenu  :\n");

            for (i = 0; i < 3; i++)
            {
                rr[i] = sol.rr[i] - rb[i];
            }
            GlobalMembersRtkcmn.ecef2pos(rb, pos);
            GlobalMembersSolution.soltocov(sol, P);
            GlobalMembersRtkcmn.covenu(pos, P, ref Q);
            GlobalMembersRtkcmn.ecef2enu(pos, rr, ref enu);
            p += sprintf(p, "%s%s%14.4f%s%14.4f%s%14.4f%s%3d%s%3d%s%8.4f%s%8.4f%s%8.4f%s%8.4f%s%8.4f%s%8.4f%s%6.2f%s%6.1f\n", s, sep, enu[0], sep, enu[1], sep, enu[2], sep, sol.stat, sep, sol.ns, sep, ((Q[0]) < 0.0 ? 0.0 : Math.Sqrt(Q[0])), sep, ((Q[4]) < 0.0 ? 0.0 : Math.Sqrt(Q[4])), sep, ((Q[8]) < 0.0 ? 0.0 : Math.Sqrt(Q[8])), sep, GlobalMembersSolution.sqvar(Q[1]), sep, GlobalMembersSolution.sqvar(Q[5]), sep, GlobalMembersSolution.sqvar(Q[2]), sep, sol.age, sep, sol.ratio);
            return p - (string)buff;
        }
        //C++ TO C# CONVERTER NOTE: This was formerly a static local variable declaration (not allowed in C#):
        private static double outnmea_rmc_dirp = 0.0;
        /* output solution in the form of nmea RMC sentence --------------------------*/
        public static int outnmea_rmc(ref byte buff, sol_t sol)
        {
            //C++ TO C# CONVERTER NOTE: This static local variable declaration (not allowed in C#) has been moved just prior to the method:
            //	static double dirp=0.0;
            gtime_t time = new gtime_t();
            double[] ep = new double[6];
            double[] pos = new double[3];
            double[] enuv = new double[3];
            double[] dms1 = new double[3];
            double[] dms2 = new double[3];
            double vel;
            double dir;
            double amag = 0.0;
            //C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on this variable, so pointers on this variable are left unchanged:
            sbyte* p = (string)buff;
            //C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on this variable, so pointers on this variable are left unchanged:
            sbyte* q;
            sbyte sum;
            string emag = "E";

            GlobalMembersRtkcmn.trace(3, "outnmea_rmc:\n");

            if (sol.stat <= DefineConstants.SOLQ_NONE)
            {
                p += sprintf(p, "$GPRMC,,,,,,,,,,,,");
                for (q = (string)buff + 1, sum = 0; *q; q++)
                {
                    sum ^= *q;
                }
                p += sprintf(p, "*%02X%c%c", sum, 0x0D, 0x0A);
                return p - (string)buff;
            }
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: time=gpst2utc(sol->time);
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            time.CopyFrom(GlobalMembersRtkcmn.gpst2utc(new gtime_t(sol.time)));
            if (time.sec >= 0.995)
            {
                time.time++;
                time.sec = 0.0;
            }
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: time2epoch(time,ep);
            GlobalMembersRtkcmn.time2epoch(new gtime_t(time), ep);
            GlobalMembersRtkcmn.ecef2pos(sol.rr, pos);
            GlobalMembersRtkcmn.ecef2enu(pos, sol.rr + 3, ref enuv);
            vel = GlobalMembersRtkcmn.norm(enuv, 3);
            if (vel >= 1.0)
            {
                dir = Math.Atan2(enuv[0], enuv[1]) * 180.0 / DefineConstants.PI;
                if (dir < 0.0)
                {
                    dir += 360.0;
                }
                outnmea_rmc_dirp = dir;
            }
            else
            {
                dir = outnmea_rmc_dirp;
            }
            GlobalMembersRtkcmn.deg2dms(Math.Abs(pos[0]) * 180.0 / DefineConstants.PI, dms1);
            GlobalMembersRtkcmn.deg2dms(Math.Abs(pos[1]) * 180.0 / DefineConstants.PI, dms2);
            p += sprintf(p, "$GPRMC,%02.0f%02.0f%05.2f,A,%02.0f%010.7f,%s,%03.0f%010.7f,%s,%4.2f,%4.2f,%02.0f%02.0f%02d,%.1f,%s,%s", ep[3], ep[4], ep[5], dms1[0], dms1[1] + dms1[2] / 60.0, pos[0] >= 0 ? "N" : "S", dms2[0], dms2[1] + dms2[2] / 60.0, pos[1] >= 0 ? "E" : "W", vel / DefineConstants.KNOT2M, dir, ep[2], ep[1], (int)ep[0] % 100, amag, emag, sol.stat == DefineConstants.SOLQ_DGPS || sol.stat == DefineConstants.SOLQ_FLOAT || sol.stat == DefineConstants.SOLQ_FIX ? "D" : "A");
            for (q = (string)buff + 1, sum = 0; *q; q++) // check-sum
            {
                sum ^= *q;
            }
            p += sprintf(p, "*%02X%c%c", sum, 0x0D, 0x0A);
            return p - (string)buff;
        }
        /* output solution in the form of nmea GGA sentence --------------------------*/
        public static int outnmea_gga(ref byte buff, sol_t sol)
        {
            gtime_t time = new gtime_t();
            double h;
            double[] ep = new double[6];
            double[] pos = new double[3];
            double[] dms1 = new double[3];
            double[] dms2 = new double[3];
            double dop = 1.0;
            int solq;
            //C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on this variable, so pointers on this variable are left unchanged:
            sbyte* p = (string)buff;
            //C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on this variable, so pointers on this variable are left unchanged:
            sbyte* q;
            sbyte sum;

            GlobalMembersRtkcmn.trace(3, "outnmea_gga:\n");

            if (sol.stat <= DefineConstants.SOLQ_NONE)
            {
                p += sprintf(p, "$GPGGA,,,,,,,,,,,,,,");
                for (q = (string)buff + 1, sum = 0; *q; q++)
                {
                    sum ^= *q;
                }
                p += sprintf(p, "*%02X%c%c", sum, 0x0D, 0x0A);
                return p - (string)buff;
            }
            for (solq = 0; solq < 8; solq++)
            {
                if (solq_nmea[solq] == sol.stat)
                    break;
            }
            if (solq >= 8)
            {
                solq = 0;
            }
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: time=gpst2utc(sol->time);
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            time.CopyFrom(GlobalMembersRtkcmn.gpst2utc(new gtime_t(sol.time)));
            if (time.sec >= 0.995)
            {
                time.time++;
                time.sec = 0.0;
            }
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: time2epoch(time,ep);
            GlobalMembersRtkcmn.time2epoch(new gtime_t(time), ep);
            GlobalMembersRtkcmn.ecef2pos(sol.rr, pos);
            h = GlobalMembersGeoid.geoidh(pos);
            GlobalMembersRtkcmn.deg2dms(Math.Abs(pos[0]) * 180.0 / DefineConstants.PI, dms1);
            GlobalMembersRtkcmn.deg2dms(Math.Abs(pos[1]) * 180.0 / DefineConstants.PI, dms2);
            p += sprintf(p, "$GPGGA,%02.0f%02.0f%05.2f,%02.0f%010.7f,%s,%03.0f%010.7f,%s,%d,%02d,%.1f,%.3f,M,%.3f,M,%.1f,", ep[3], ep[4], ep[5], dms1[0], dms1[1] + dms1[2] / 60.0, pos[0] >= 0 ? "N" : "S", dms2[0], dms2[1] + dms2[2] / 60.0, pos[1] >= 0 ? "E" : "W", solq, sol.ns, dop, pos[2] - h, h, sol.age);
            for (q = (string)buff + 1, sum = 0; *q; q++) // check-sum
            {
                sum ^= *q;
            }
            p += sprintf(p, "*%02X%c%c", sum, 0x0D, 0x0A);
            return p - (string)buff;
        }
        /* output solution in the form of nmea GSA sentences -------------------------*/
        public static int outnmea_gsa(ref byte buff, sol_t sol, ssat_t[] ssat)
        {
            double[] azel = new double[DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1 * 2];
            double[] dop = new double[4];
            int i;
            int sat;
            int sys;
            int nsat;
            int[] prn = new int[DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1];
            //C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on this variable, so pointers on this variable are left unchanged:
            sbyte* p = (string)buff;
            //C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on this variable, so pointers on this variable are left unchanged:
            sbyte* q;
            string s;
            sbyte sum;

            GlobalMembersRtkcmn.trace(3, "outnmea_gsa:\n");

            if (sol.stat <= DefineConstants.SOLQ_NONE)
            {
                p += sprintf(p, "$GPGSA,A,1,,,,,,,,,,,,,,,");
                for (q = (string)buff + 1, sum = 0; *q; q++)
                {
                    sum ^= *q;
                }
                p += sprintf(p, "*%02X%c%c", sum, 0x0D, 0x0A);
                return p - (string)buff;
            }
            /* GPGSA: gps/sbas */
            for (sat = 1, nsat = 0; sat <= DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1 && nsat < 12; sat++)
            {
                if (!ssat[sat - 1].vs || ssat[sat - 1].azel[1] <= 0.0)
                    continue;
                sys = GlobalMembersRtkcmn.satsys(sat, ref prn + nsat);
                if (sys != DefineConstants.SYS_GPS && sys != DefineConstants.SYS_SBS)
                    continue;
                if (sys == DefineConstants.SYS_SBS)
                {
                    prn[nsat] += 33 - DefineConstants.MINPRNSBS;
                }
                for (i = 0; i < 2; i++)
                {
                    azel[i + nsat * 2] = ssat[sat - 1].azel[i];
                }
                nsat++;
            }
            if (nsat > 0)
            {
                s = p;
                p += sprintf(p, "$GPGSA,A,%d", sol.stat <= 0 ? 1 : 3);
                for (i = 0; i < 12; i++)
                {
                    if (i < nsat)
                    {
                        p += sprintf(p, ",%02d", prn[i]);
                    }
                    else
                    {
                        p += sprintf(p, ",");
                    }
                }
                GlobalMembersRtkcmn.dops(nsat, azel, 0.0, dop);
                p += sprintf(p, ",%3.1f,%3.1f,%3.1f,1", dop[1], dop[2], dop[3]);
                for (q = s.Substring(1), sum = 0; *q; q++) // check-sum
                {
                    sum ^= *q;
                }
                p += sprintf(p, "*%02X%c%c", sum, 0x0D, 0x0A);
            }
            /* GLGSA: glonass */
            for (sat = 1, nsat = 0; sat <= DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1 && nsat < 12; sat++)
            {
                if (!ssat[sat - 1].vs || ssat[sat - 1].azel[1] <= 0.0)
                    continue;
                if (GlobalMembersRtkcmn.satsys(sat, ref prn + nsat) != DefineConstants.SYS_GLO)
                    continue;
                for (i = 0; i < 2; i++)
                {
                    azel[i + nsat * 2] = ssat[sat - 1].azel[i];
                }
                nsat++;
            }
            if (nsat > 0)
            {
                s = p;
                p += sprintf(p, "$GLGSA,A,%d", sol.stat <= 0 ? 1 : 3);
                for (i = 0; i < 12; i++)
                {
                    if (i < nsat)
                    {
                        p += sprintf(p, ",%02d", prn[i] + 64);
                    }
                    else
                    {
                        p += sprintf(p, ",");
                    }
                }
                GlobalMembersRtkcmn.dops(nsat, azel, 0.0, dop);
                p += sprintf(p, ",%3.1f,%3.1f,%3.1f,2", dop[1], dop[2], dop[3]);
                for (q = s.Substring(1), sum = 0; *q; q++) // check-sum
                {
                    sum ^= *q;
                }
                p += sprintf(p, "*%02X%c%c", sum, 0x0D, 0x0A);
            }
            /* GAGSA: galileo */
            for (sat = 1, nsat = 0; sat <= DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1 && nsat < 12; sat++)
            {
                if (!ssat[sat - 1].vs || ssat[sat - 1].azel[1] <= 0.0)
                    continue;
                if (GlobalMembersRtkcmn.satsys(sat, ref prn + nsat) != DefineConstants.SYS_GAL)
                    continue;
                for (i = 0; i < 2; i++)
                {
                    azel[i + nsat * 2] = ssat[sat - 1].azel[i];
                }
                nsat++;
            }
            if (nsat > 0)
            {
                s = p;
                p += sprintf(p, "$GAGSA,A,%d", sol.stat <= 0 ? 1 : 3);
                for (i = 0; i < 12; i++)
                {
                    if (i < nsat)
                    {
                        p += sprintf(p, ",%02d", prn[i]);
                    }
                    else
                    {
                        p += sprintf(p, ",");
                    }
                }
                GlobalMembersRtkcmn.dops(nsat, azel, 0.0, dop);
                p += sprintf(p, ",%3.1f,%3.1f,%3.1f,3", dop[1], dop[2], dop[3]);
                for (q = s.Substring(1), sum = 0; *q; q++) // check-sum
                {
                    sum ^= *q;
                }
                p += sprintf(p, "*%02X%c%c", sum, 0x0D, 0x0A);
            }
            return p - (string)buff;
        }
        /* output solution in the form of nmea GSV sentence --------------------------*/
        public static int outnmea_gsv(ref byte buff, sol_t sol, ssat_t[] ssat)
        {
            double az;
            double el;
            double snr;
            int i;
            int j;
            int k;
            int n;
            int sat;
            int prn;
            int sys;
            int nmsg;
            int[] sats = new int[DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1];
            //C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on this variable, so pointers on this variable are left unchanged:
            sbyte* p = (string)buff;
            //C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on this variable, so pointers on this variable are left unchanged:
            sbyte* q;
            string s;
            sbyte sum;

            GlobalMembersRtkcmn.trace(3, "outnmea_gsv:\n");

            if (sol.stat <= DefineConstants.SOLQ_NONE)
            {
                p += sprintf(p, "$GPGSV,1,1,0,,,,,,,,,,,,,,,,");
                for (q = (string)buff + 1, sum = 0; *q; q++)
                {
                    sum ^= *q;
                }
                p += sprintf(p, "*%02X%c%c", sum, 0x0D, 0x0A);
                return p - (string)buff;
            }
            /* GPGSV: gps/sbas */
            for (sat = 1, n = 0; sat < DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1 && n < 12; sat++)
            {
                sys = GlobalMembersRtkcmn.satsys(sat, ref prn);
                if (sys != DefineConstants.SYS_GPS && sys != DefineConstants.SYS_SBS)
                    continue;
                if (ssat[sat - 1].vs && ssat[sat - 1].azel[1] > 0.0)
                {
                    sats[n++] = sat;
                }
            }
            nmsg = n <= 0 ? 0 : (n - 1) / 4 + 1;

            for (i = k = 0; i < nmsg; i++)
            {
                s = p;
                p += sprintf(p, "$GPGSV,%d,%d,%02d", nmsg, i + 1, n);

                for (j = 0; j < 4; j++, k++)
                {
                    if (k < n)
                    {
                        if (GlobalMembersRtkcmn.satsys(sats[k], ref prn) == DefineConstants.SYS_SBS)
                        {
                            prn += 33 - DefineConstants.MINPRNSBS;
                        }
                        az = ssat[sats[k] - 1].azel[0] * 180.0 / DefineConstants.PI;
                        if (az < 0.0)
                        {
                            az += 360.0;
                        }
                        el = ssat[sats[k] - 1].azel[1] * 180.0 / DefineConstants.PI;
                        snr = ssat[sats[k] - 1].snr[0] * 0.25;
                        p += sprintf(p, ",%02d,%02.0f,%03.0f,%02.0f", prn, el, az, snr);
                    }
                    else
                    {
                        p += sprintf(p, ",,,,");
                    }
                }
                p += sprintf(p, ",1"); // L1C/A
                for (q = s.Substring(1), sum = 0; *q; q++) // check-sum
                {
                    sum ^= *q;
                }
                p += sprintf(p, "*%02X%c%c", sum, 0x0D, 0x0A);
            }
            /* GLGSV: glonass */
            for (sat = 1, n = 0; sat < DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1 && n < 12; sat++)
            {
                if (GlobalMembersRtkcmn.satsys(sat, ref prn) != DefineConstants.SYS_GLO)
                    continue;
                if (ssat[sat - 1].vs && ssat[sat - 1].azel[1] > 0.0)
                {
                    sats[n++] = sat;
                }
            }
            nmsg = n <= 0 ? 0 : (n - 1) / 4 + 1;

            for (i = k = 0; i < nmsg; i++)
            {
                s = p;
                p += sprintf(p, "$GLGSV,%d,%d,%02d", nmsg, i + 1, n);

                for (j = 0; j < 4; j++, k++)
                {
                    if (k < n)
                    {
                        GlobalMembersRtkcmn.satsys(sats[k], ref prn); // 65-99
                        prn += 64;
                        az = ssat[sats[k] - 1].azel[0] * 180.0 / DefineConstants.PI;
                        if (az < 0.0)
                        {
                            az += 360.0;
                        }
                        el = ssat[sats[k] - 1].azel[1] * 180.0 / DefineConstants.PI;
                        snr = ssat[sats[k] - 1].snr[0] * 0.25;
                        p += sprintf(p, ",%02d,%02.0f,%03.0f,%02.0f", prn, el, az, snr);
                    }
                    else
                    {
                        p += sprintf(p, ",,,,");
                    }
                }
                p += sprintf(p, ",1"); // L1C/A
                for (q = s.Substring(1), sum = 0; *q; q++) // check-sum
                {
                    sum ^= *q;
                }
                p += sprintf(p, "*%02X%c%c", sum, 0x0D, 0x0A);
            }
            /* GAGSV: galileo */
            for (sat = 1, n = 0; sat < DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1 && n < 12; sat++)
            {
                if (GlobalMembersRtkcmn.satsys(sat, ref prn) != DefineConstants.SYS_GAL)
                    continue;
                if (ssat[sat - 1].vs && ssat[sat - 1].azel[1] > 0.0)
                {
                    sats[n++] = sat;
                }
            }
            nmsg = n <= 0 ? 0 : (n - 1) / 4 + 1;

            for (i = k = 0; i < nmsg; i++)
            {
                s = p;
                p += sprintf(p, "$GAGSV,%d,%d,%02d", nmsg, i + 1, n);

                for (j = 0; j < 4; j++, k++)
                {
                    if (k < n)
                    {
                        GlobalMembersRtkcmn.satsys(sats[k], ref prn); // 1-36
                        az = ssat[sats[k] - 1].azel[0] * 180.0 / DefineConstants.PI;
                        if (az < 0.0)
                        {
                            az += 360.0;
                        }
                        el = ssat[sats[k] - 1].azel[1] * 180.0 / DefineConstants.PI;
                        snr = ssat[sats[k] - 1].snr[0] * 0.25;
                        p += sprintf(p, ",%02d,%02.0f,%03.0f,%02.0f", prn, el, az, snr);
                    }
                    else
                    {
                        p += sprintf(p, ",,,,");
                    }
                }
                p += sprintf(p, ",7"); // L1BC
                for (q = s.Substring(1), sum = 0; *q; q++) // check-sum
                {
                    sum ^= *q;
                }
                p += sprintf(p, "*%02X%c%c", sum, 0x0D, 0x0A);
            }
            return p - (string)buff;
        }
        /* output processing options ---------------------------------------------------
        * output processing options to buffer
        * args   : unsigned char *buff IO output buffer
        *          prcopt_t *opt    I   processign options
        * return : number of output bytes
        *-----------------------------------------------------------------------------*/
        public static int outprcopts(ref byte buff, prcopt_t opt)
        {
            int[] sys = { DefineConstants.SYS_GPS, DefineConstants.SYS_GLO, DefineConstants.SYS_GAL, DefineConstants.SYS_QZS, DefineConstants.SYS_SBS, 0 };
            string[] s1 = { "single", "dgps", "kinematic", "static", "moving-base", "fixed", "ppp-kinematic", "ppp-static", "ppp-fixed", "" };
            string[] s2 = { "L1", "L1+L2", "L1+L2+L5", "L1+L2+L5+L6", "L1+L2+L5+L6+L7", "L1+L2+L5+L6+L7+L8", "" };
            string[] s3 = { "forward", "backward", "combined" };
            string[] s4 = { "off", "broadcast", "sbas", "iono-free", "estimation", "ionex tec", "qzs", "lex", "vtec_sf", "vtec_ef", "gtec", "" };
            string[] s5 = { "off", "saastamoinen", "sbas", "est ztd", "est ztd+grad", "" };
            string[] s6 = { "broadcast", "precise", "broadcast+sbas", "broadcast+ssr apc", "broadcast+ssr com", "qzss lex", "" };
            string[] s7 = { "gps", "glonass", "galileo", "qzss", "sbas", "" };
            string[] s8 = { "off", "continuous", "instantaneous", "fix and hold", "" };
            string[] s9 = { "off", "on", "auto calib", "external calib", "" };
            int i;
            //C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on this variable, so pointers on this variable are left unchanged:
            sbyte* p = (string)buff;

            GlobalMembersRtkcmn.trace(3, "outprcopts:\n");

            p += sprintf(p, "%s pos mode  : %s\n", DefineConstants.COMMENTH, s1[opt.mode]);

            if (DefineConstants.PMODE_DGPS <= opt.mode && opt.mode <= DefineConstants.PMODE_FIXED)
            {
                p += sprintf(p, "%s freqs     : %s\n", DefineConstants.COMMENTH, s2[opt.nf - 1]);
            }
            if (opt.mode > DefineConstants.PMODE_SINGLE)
            {
                p += sprintf(p, "%s solution  : %s\n", DefineConstants.COMMENTH, s3[opt.soltype]);
            }
            p += sprintf(p, "%s elev mask : %.1f deg\n", DefineConstants.COMMENTH, opt.elmin * 180.0 / DefineConstants.PI);
            if (opt.mode > DefineConstants.PMODE_SINGLE)
            {
                p += sprintf(p, "%s dynamics  : %s\n", DefineConstants.COMMENTH, opt.dynamics != 0 ? "on" : "off");
                p += sprintf(p, "%s tidecorr  : %s\n", DefineConstants.COMMENTH, opt.tidecorr != 0 ? "on" : "off");
            }
            if (opt.mode <= DefineConstants.PMODE_FIXED)
            {
                p += sprintf(p, "%s ionos opt : %s\n", DefineConstants.COMMENTH, s4[opt.ionoopt]);
            }
            p += sprintf(p, "%s tropo opt : %s\n", DefineConstants.COMMENTH, s5[opt.tropopt]);
            p += sprintf(p, "%s ephemeris : %s\n", DefineConstants.COMMENTH, s6[opt.sateph]);
            if (opt.navsys != DefineConstants.SYS_GPS)
            {
                p += sprintf(p, "%s navi sys  :", DefineConstants.COMMENTH);
                for (i = 0; sys[i] != 0; i++)
                {
                    if ((opt.navsys & sys[i]) != 0)
                    {
                        p += sprintf(p, " %s", s7[i]);
                    }
                }
                p += sprintf(p, "\n");
            }
            if (DefineConstants.PMODE_KINEMA <= opt.mode && opt.mode <= DefineConstants.PMODE_FIXED)
            {
                p += sprintf(p, "%s amb res   : %s\n", DefineConstants.COMMENTH, s8[opt.modear]);
                if ((opt.navsys & DefineConstants.SYS_GLO) != 0)
                {
                    p += sprintf(p, "%s amb glo   : %s\n", DefineConstants.COMMENTH, s9[opt.glomodear]);
                }
                if (opt.thresar[0] > 0.0)
                {
                    p += sprintf(p, "%s val thres : %.1f\n", DefineConstants.COMMENTH, opt.thresar[0]);
                }
            }
            if (opt.mode == DefineConstants.PMODE_MOVEB && opt.baseline[0] > 0.0)
            {
                p += sprintf(p, "%s baseline  : %.4f %.4f m\n", DefineConstants.COMMENTH, opt.baseline[0], opt.baseline[1]);
            }
            for (i = 0; i < 2; i++)
            {
                if (opt.mode == DefineConstants.PMODE_SINGLE || (i >= 1 && opt.mode > DefineConstants.PMODE_FIXED))
                    continue;
                p += sprintf(p, "%s antenna%d  : %-21s (%7.4f %7.4f %7.4f)\n", DefineConstants.COMMENTH, i + 1, opt.anttype[i], opt.antdel[i, 0], opt.antdel[i, 1], opt.antdel[i, 2]);
            }
            return p - (string)buff;
        }
        /* output solution header ------------------------------------------------------
        * output solution header to buffer
        * args   : unsigned char *buff IO output buffer
        *          solopt_t *opt    I   solution options
        * return : number of output bytes
        *-----------------------------------------------------------------------------*/
        public static int outsolheads(ref byte buff, solopt_t opt)
        {
            string[] s1 = { "WGS84", "Tokyo" };
            string[] s2 = { "ellipsoidal", "geodetic" };
            string[] s3 = { "GPST", "UTC ", "JST " };
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: sbyte *sep=opt2sep(opt);
            sbyte sep = GlobalMembersSolution.opt2sep(opt);
            //C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on this variable, so pointers on this variable are left unchanged:
            sbyte* p = (string)buff;
            int timeu = opt.timeu < 0 ? 0 : (opt.timeu > 20 ? 20 : opt.timeu);

            GlobalMembersRtkcmn.trace(3, "outsolheads:\n");

            if (opt.posf == DefineConstants.SOLF_NMEA)
            {
                return 0;
            }

            if (opt.outhead != 0)
            {
                p += sprintf(p, "%s (", DefineConstants.COMMENTH);
                if (opt.posf == DefineConstants.SOLF_XYZ)
                {
                    p += sprintf(p, "x/y/z-ecef=WGS84");
                }
                else if (opt.posf == DefineConstants.SOLF_ENU)
                {
                    p += sprintf(p, "e/n/u-baseline=WGS84");
                }
                else
                {
                    p += sprintf(p, "lat/lon/height=%s/%s", s1[opt.datum], s2[opt.height]);
                }
                p += sprintf(p, ",Q=1:fix,2:float,3:sbas,4:dgps,5:single,6:ppp,ns=# of satellites)\n");
            }
            p += sprintf(p, "%s  %-*s%s", DefineConstants.COMMENTH, (opt.timef != 0 ? 16 : 8) + timeu + 1, s3[opt.times], sep);

            if (opt.posf == DefineConstants.SOLF_LLH) // lat/lon/hgt
            {
                if (opt.degf != 0)
                {
                    p += sprintf(p, "%16s%s%16s%s%10s%s%3s%s%3s%s%8s%s%8s%s%8s%s%8s%s%8s%s%8s%s%6s%s%6s\n", "latitude(d'\")", sep, "longitude(d'\")", sep, "height(m)", sep, "Q", sep, "ns", sep, "sdn(m)", sep, "sde(m)", sep, "sdu(m)", sep, "sdne(m)", sep, "sdeu(m)", sep, "sdue(m)", sep, "age(s)", sep, "ratio");
                }
                else
                {
                    p += sprintf(p, "%14s%s%14s%s%10s%s%3s%s%3s%s%8s%s%8s%s%8s%s%8s%s%8s%s%8s%s%6s%s%6s\n", "latitude(deg)", sep, "longitude(deg)", sep, "height(m)", sep, "Q", sep, "ns", sep, "sdn(m)", sep, "sde(m)", sep, "sdu(m)", sep, "sdne(m)", sep, "sdeu(m)", sep, "sdun(m)", sep, "age(s)", sep, "ratio");
                }
            }
            else if (opt.posf == DefineConstants.SOLF_XYZ) // x/y/z-ecef
            {
                p += sprintf(p, "%14s%s%14s%s%14s%s%3s%s%3s%s%8s%s%8s%s%8s%s%8s%s%8s%s%8s%s%6s%s%6s\n", "x-ecef(m)", sep, "y-ecef(m)", sep, "z-ecef(m)", sep, "Q", sep, "ns", sep, "sdx(m)", sep, "sdy(m)", sep, "sdz(m)", sep, "sdxy(m)", sep, "sdyz(m)", sep, "sdzx(m)", sep, "age(s)", sep, "ratio");
            }
            else if (opt.posf == DefineConstants.SOLF_ENU) // e/n/u-baseline
            {
                p += sprintf(p, "%14s%s%14s%s%14s%s%3s%s%3s%s%8s%s%8s%s%8s%s%8s%s%8s%s%8s%s%6s%s%6s\n", "e-baseline(m)", sep, "n-baseline(m)", sep, "u-baseline(m)", sep, "Q", sep, "ns", sep, "sde(m)", sep, "sdn(m)", sep, "sdu(m)", sep, "sden(m)", sep, "sdnu(m)", sep, "sdue(m)", sep, "age(s)", sep, "ratio");
            }
            return p - (string)buff;
        }
        /* output solution body --------------------------------------------------------
        * output solution body to buffer
        * args   : unsigned char *buff IO output buffer
        *          sol_t  *sol      I   solution
        *          double *rb       I   base station position {x,y,z} (ecef) (m)
        *          solopt_t *opt    I   solution options
        * return : number of output bytes
        *-----------------------------------------------------------------------------*/
        public static int outsols(ref byte buff, sol_t sol, double rb, solopt_t opt)
        {
            gtime_t time = new gtime_t();
            gtime_t ts = new gtime_t();
            double gpst;
            int week;
            int timeu;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: const sbyte *sep=opt2sep(opt);
            sbyte sep = GlobalMembersSolution.opt2sep(opt);
            string s = new string(new char[64]);
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: byte *p=buff;
            byte p = buff;

            GlobalMembersRtkcmn.trace(3, "outsols :\n");

            if (opt.posf == DefineConstants.SOLF_NMEA)
            {
                if (opt.nmeaintv[0] < 0.0)
                {
                    return 0;
                }
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: if (!screent(sol->time,ts,ts,opt->nmeaintv[0]))
                if (GlobalMembersRtkcmn.screent(new gtime_t(sol.time), new gtime_t(ts), new gtime_t(ts), opt.nmeaintv[0]) == 0)
                {
                    return 0;
                }
            }
            if (sol.stat <= DefineConstants.SOLQ_NONE || (opt.posf == DefineConstants.SOLF_ENU && GlobalMembersRtkcmn.norm(rb, 3) <= 0.0))
            {
                return 0;
            }
            timeu = opt.timeu < 0 ? 0 : (opt.timeu > 20 ? 20 : opt.timeu);

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: time=sol->time;
            time.CopyFrom(sol.time);
            if (opt.times >= DefineConstants.TIMES_UTC)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                //ORIGINAL LINE: time=gpst2utc(time);
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                time.CopyFrom(GlobalMembersRtkcmn.gpst2utc(new gtime_t(time)));
            }
            if (opt.times == DefineConstants.TIMES_JST)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                //ORIGINAL LINE: time=timeadd(time,9 *3600.0);
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                time.CopyFrom(GlobalMembersRtkcmn.timeadd(new gtime_t(time), 9 * 3600.0));
            }

            if (opt.timef != 0)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: time2str(time,s,timeu);
                GlobalMembersRtkcmn.time2str(new gtime_t(time), ref s, timeu);
            }
            else
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: gpst=time2gpst(time,&week);
                gpst = GlobalMembersRtkcmn.time2gpst(new gtime_t(time), ref week);
                if (86400 * 7 - gpst < 0.5 / Math.Pow(10.0, timeu))
                {
                    week++;
                    gpst = 0.0;
                }
                //C++ TO C# CONVERTER TODO TASK: The following line has a C format specifier which cannot be directly translated to C#:
                //ORIGINAL LINE: sprintf(s,"%4d%s%*.*f",week,sep,6+(timeu<=0?0:timeu+1),timeu,gpst);
                s = string.Format("{0,4:D}{1}%*.*f", week, sep, 6 + (timeu <= 0 ? 0 : timeu + 1), timeu, gpst);
            }
            switch (opt.posf)
            {
                case DefineConstants.SOLF_LLH:
                    p += GlobalMembersSolution.outpos(ref p, s, sol, opt);
                    break;
                case DefineConstants.SOLF_XYZ:
                    p += GlobalMembersSolution.outecef(ref p, s, sol, opt);
                    break;
                case DefineConstants.SOLF_ENU:
                    p += GlobalMembersSolution.outenu(ref p, s, sol, rb, opt);
                    break;
                case DefineConstants.SOLF_NMEA:
                    p += GlobalMembersSolution.outnmea_rmc(ref p, sol);
                    p += GlobalMembersSolution.outnmea_gga(ref p, sol);
                    break;
            }
            return p - buff;
        }
        /* output solution extended ----------------------------------------------------
        * output solution exteneded infomation
        * args   : unsigned char *buff IO output buffer
        *          sol_t  *sol      I   solution
        *          ssat_t *ssat     I   satellite status
        *          solopt_t *opt    I   solution options
        * return : number of output bytes
        * notes  : only support nmea
        *-----------------------------------------------------------------------------*/
        public static int outsolexs(ref byte buff, sol_t sol, ssat_t ssat, solopt_t opt)
        {
            gtime_t ts = new gtime_t();
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: byte *p=buff;
            byte p = buff;

            GlobalMembersRtkcmn.trace(3, "outsolexs:\n");

            if (opt.posf == DefineConstants.SOLF_NMEA)
            {
                if (opt.nmeaintv[1] < 0.0)
                {
                    return 0;
                }
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: if (!screent(sol->time,ts,ts,opt->nmeaintv[1]))
                if (GlobalMembersRtkcmn.screent(new gtime_t(sol.time), new gtime_t(ts), new gtime_t(ts), opt.nmeaintv[1]) == 0)
                {
                    return 0;
                }
            }
            if (opt.posf == DefineConstants.SOLF_NMEA)
            {
                p += GlobalMembersSolution.outnmea_gsa(ref p, sol, ssat);
                p += GlobalMembersSolution.outnmea_gsv(ref p, sol, ssat);
            }
            return p - buff;
        }
        /* output processing option ----------------------------------------------------
        * output processing option to file
        * args   : FILE   *fp       I   output file pointer
        *          prcopt_t *opt    I   processing options
        * return : none
        *-----------------------------------------------------------------------------*/
        public static void outprcopt(FILE fp, prcopt_t opt)
        {
            byte[] buff = new byte[DefineConstants.MAXSOLMSG + 1];
            int n;

            GlobalMembersRtkcmn.trace(3, "outprcopt:\n");

            if ((n = GlobalMembersSolution.outprcopts(ref buff, opt)) > 0)
            {
                fwrite(buff, n, 1, fp);
            }
        }
        /* output solution header ------------------------------------------------------
        * output solution heade to file
        * args   : FILE   *fp       I   output file pointer
        *          solopt_t *opt    I   solution options
        * return : none
        *-----------------------------------------------------------------------------*/
        public static void outsolhead(FILE fp, solopt_t opt)
        {
            byte[] buff = new byte[DefineConstants.MAXSOLMSG + 1];
            int n;

            GlobalMembersRtkcmn.trace(3, "outsolhead:\n");

            if ((n = GlobalMembersSolution.outsolheads(ref buff, opt)) > 0)
            {
                fwrite(buff, n, 1, fp);
            }
        }
        /* output solution body --------------------------------------------------------
        * output solution body to file
        * args   : FILE   *fp       I   output file pointer
        *          sol_t  *sol      I   solution
        *          double *rb       I   base station position {x,y,z} (ecef) (m)
        *          solopt_t *opt    I   solution options
        * return : none
        *-----------------------------------------------------------------------------*/
        public static void outsol(FILE fp, sol_t sol, double rb, solopt_t opt)
        {
            byte[] buff = new byte[DefineConstants.MAXSOLMSG + 1];
            int n;

            GlobalMembersRtkcmn.trace(3, "outsol  :\n");

            if ((n = GlobalMembersSolution.outsols(ref buff, sol, rb, opt)) > 0)
            {
                fwrite(buff, n, 1, fp);
            }
        }
        /* output solution extended ----------------------------------------------------
        * output solution exteneded infomation to file
        * args   : FILE   *fp       I   output file pointer
        *          sol_t  *sol      I   solution
        *          ssat_t *ssat     I   satellite status
        *          solopt_t *opt    I   solution options
        * return : output size (bytes)
        * notes  : only support nmea
        *-----------------------------------------------------------------------------*/
        public static void outsolex(FILE fp, sol_t sol, ssat_t ssat, solopt_t opt)
        {
            byte[] buff = new byte[DefineConstants.MAXSOLMSG + 1];
            int n;

            GlobalMembersRtkcmn.trace(3, "outsolex:\n");

            if ((n = GlobalMembersSolution.outsolexs(ref buff, sol, ssat, opt)) > 0)
            {
                fwrite(buff, n, 1, fp);
            }
        }
    }
}
