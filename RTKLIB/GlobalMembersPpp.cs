using ghGPS.Classes.rcv;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ghGPS.Classes
{
    public static class GlobalMembersPpp
    {
        /*------------------------------------------------------------------------------
        * ppp.c : precise point positioning
        *
        *          Copyright (C) 2010-2013 by T.TAKASU, All rights reserved.
        *
        * options : -DIERS_MODEL use IERS tide model
        *
        * references :
        *     [1] D.D.McCarthy, IERS Technical Note 21, IERS Conventions 1996, July 1996
        *     [2] D.D.McCarthy and G.Petit, IERS Technical Note 32, IERS Conventions
        *         2003, November 2003
        *     [3] D.A.Vallado, Fundamentals of Astrodynamics and Applications 2nd ed,
        *         Space Technology Library, 2004
        *     [4] J.Kouba, A Guide to using International GNSS Service (IGS) products,
        *         May 2009
        *     [5] RTCM Paper, April 12, 2010, Proposed SSR Messages for SV Orbit Clock,
        *         Code Biases, URA
        *     [6] MacMillan et al., Atmospheric gradients and the VLBI terrestrial and
        *         celestial reference frames, Geophys. Res. Let., 1997
        *     [7] G.Petit and B.Luzum (eds), IERS Technical Note No. 36, IERS
        *         Conventions (2010), 2010
        *
        * version : $Revision:$ $Date:$
        * history : 2010/07/20 1.0  new
        *                           added api:
        *                               tidedisp()
        *           2010/12/11 1.1  enable exclusion of eclipsing satellite
        *           2012/02/01 1.2  add gps-glonass h/w bias correction
        *                           move windupcorr() to rtkcmn.c
        *           2013/03/11 1.3  add otl and pole tides corrections
        *                           involve iers model with -DIERS_MODEL
        *                           change initial variances
        *                           suppress acos domain error
        *           2013/09/01 1.4  pole tide model by iers 2010
        *                           add mode of ionosphere model off
        *           2014/05/23 1.5  add output of trop gradient in solution status
        *           2014/10/13 1.6  fix bug on P0(a[3]) computation in tide_oload()
        *                           fix bug on m2 computation in tide_pole()
        *-----------------------------------------------------------------------------*/

        /* function prototypes -------------------------------------------------------*/
#if IERS_MODEL
	//int dehanttideinel_(ref double xsta, ref int year, ref int mon, ref int day, ref double fhr, ref double xsun, ref double xmon, ref double dxtide);
#endif

        /* output solution status for PPP --------------------------------------------*/
        public static void pppoutsolstat(rtk_t rtk, int level, FILE fp)
        {
            ssat_t ssat;
            double tow;
            double[] pos = new double[3];
            double[] vel = new double[3];
            double[] acc = new double[3];
            int i;
            int j;
            int week;
            int nfreq = 1;
            string id = new string(new char[32]);

            if (level <= 0 || fp == null)
                return;

            GlobalMembersRtkcmn.trace(3, "pppoutsolstat:\n");

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: tow=time2gpst(rtk->sol.time,&week);
            tow = GlobalMembersRtkcmn.time2gpst(new gtime_t(rtk.sol.time), ref week);

            /* receiver position */
            fprintf(fp, "$POS,%d,%.3f,%d,%.4f,%.4f,%.4f,%.4f,%.4f,%.4f\n", week, tow, rtk.sol.stat, rtk.x[0], rtk.x[1], rtk.x[2], 0.0, 0.0, 0.0);

            /* receiver velocity and acceleration */
            if (rtk.opt.dynamics != 0)
            {
                GlobalMembersRtkcmn.ecef2pos(rtk.sol.rr, pos);
                GlobalMembersRtkcmn.ecef2enu(pos, rtk.x + 3, ref vel);
                GlobalMembersRtkcmn.ecef2enu(pos, rtk.x + 6, ref acc);
                fprintf(fp, "$VELACC,%d,%.3f,%d,%.4f,%.4f,%.4f,%.5f,%.5f,%.5f,%.4f,%.4f,%.4f,%.5f,%.5f,%.5f\n", week, tow, rtk.sol.stat, vel[0], vel[1], vel[2], acc[0], acc[1], acc[2], 0.0, 0.0, 0.0, 0.0, 0.0, 0.0);
            }
            /* receiver clocks */
            i = (((rtk.opt).dynamics != 0 ? 9 : 3) + (0));
            fprintf(fp, "$CLK,%d,%.3f,%d,%d,%.3f,%.3f,%.3f,%.3f\n", week, tow, rtk.sol.stat, 1, rtk.x[i] * 1E9 / DefineConstants.CLIGHT, rtk.x[i + 1] * 1E9 / DefineConstants.CLIGHT, 0.0, 0.0);

            /* tropospheric parameters */
            if (rtk.opt.tropopt == DefineConstants.TROPOPT_EST || rtk.opt.tropopt == DefineConstants.TROPOPT_ESTG)
            {
                i = ((((rtk.opt).dynamics != 0 ? 9 : 3) + (0)) + DefineConstants.NSYSGPS + DefineConstants.NSYSGLO + DefineConstants.NSYSGAL + DefineConstants.NSYSQZS + DefineConstants.NSYSCMP + DefineConstants.NSYSLEO);
                fprintf(fp, "$TROP,%d,%.3f,%d,%d,%.4f,%.4f\n", week, tow, rtk.sol.stat, 1, rtk.x[i], 0.0);
            }
            if (rtk.opt.tropopt == DefineConstants.TROPOPT_ESTG)
            {
                i = ((((rtk.opt).dynamics != 0 ? 9 : 3) + (0)) + DefineConstants.NSYSGPS + DefineConstants.NSYSGLO + DefineConstants.NSYSGAL + DefineConstants.NSYSQZS + DefineConstants.NSYSCMP + DefineConstants.NSYSLEO);
                fprintf(fp, "$TRPG,%d,%.3f,%d,%d,%.5f,%.5f,%.5f,%.5f\n", week, tow, rtk.sol.stat, 1, rtk.x[i + 1], rtk.x[i + 2], 0.0, 0.0);
            }
            if (rtk.sol.stat == DefineConstants.SOLQ_NONE || level <= 1)
                return;

            /* residuals and status */
            for (i = 0; i < DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1; i++)
            {
                ssat = rtk.ssat + i;
                if (ssat.vs == 0)
                    continue;
                GlobalMembersRtkcmn.satno2id(i + 1, ref id);
                for (j = 0; j < nfreq; j++)
                {
                    fprintf(fp, "$SAT,%d,%.3f,%s,%d,%.1f,%.1f,%.4f,%.4f,%d,%.0f,%d,%d,%d,%d,%d,%d\n", week, tow, id, j + 1, ssat.azel[0] * 180.0 / DefineConstants.PI, ssat.azel[1] * 180.0 / DefineConstants.PI, ssat.resp[j], ssat.resc[j], ssat.vsat[j], ssat.snr[j] * 0.25, ssat.fix[j], ssat.slip[j] & 3, ssat.@lock[j], ssat.outc[j], ssat.slipc[j], ssat.rejc[j]);
                }
            }
        }
        /* solar/lunar tides (ref [2] 7) ---------------------------------------------*/
        internal static void tide_pl(double[] eu, double[] rp, double GMp, double[] pos, double[] dr)
        {
            const double H3 = 0.292;
            const double L3 = 0.015;
            double r;
            double[] ep = new double[3];
            double latp;
            double lonp;
            double p;
            double K2;
            double K3;
            double a;
            double H2;
            double L2;
            double dp;
            double du;
            double cosp;
            double sinl;
            double cosl;
            int i;

            GlobalMembersRtkcmn.trace(4, "tide_pl : pos=%.3f %.3f\n", pos[0] * 180.0 / DefineConstants.PI, pos[1] * 180.0 / DefineConstants.PI);

            if ((r = GlobalMembersRtkcmn.norm(rp, 3)) <= 0.0)
                return;

            for (i = 0; i < 3; i++)
            {
                ep[i] = rp[i] / r;
            }

            K2 = GMp / DefineConstants.GME * ((DefineConstants.RE_WGS84) * (DefineConstants.RE_WGS84)) * ((DefineConstants.RE_WGS84) * (DefineConstants.RE_WGS84)) / (r * r * r);
            K3 = K2 * DefineConstants.RE_WGS84 / r;
            latp = Math.Asin(ep[2]);
            lonp = Math.Atan2(ep[1], ep[0]);
            cosp = Math.Cos(latp);
            sinl = Math.Sin(pos[0]);
            cosl = Math.Cos(pos[0]);

            /* step1 in phase (degree 2) */
            p = (3.0 * sinl * sinl - 1.0) / 2.0;
            H2 = 0.6078 - 0.0006 * p;
            L2 = 0.0847 + 0.0002 * p;
            a = GlobalMembersRtkcmn.dot(ep, eu, 3);
            dp = K2 * 3.0 * L2 * a;
            du = K2 * (H2 * (1.5 * a * a - 0.5) - 3.0 * L2 * a * a);

            /* step1 in phase (degree 3) */
            dp += K3 * L3 * (7.5 * a * a - 1.5);
            du += K3 * (H3 * (2.5 * a * a * a - 1.5 * a) - L3 * (7.5 * a * a - 1.5) * a);

            /* step1 out-of-phase (only radial) */
            du += 3.0 / 4.0 * 0.0025 * K2 * Math.Sin(2.0 * latp) * Math.Sin(2.0 * pos[0]) * Math.Sin(pos[1] - lonp);
            du += 3.0 / 4.0 * 0.0022 * K2 * cosp * cosp * cosl * cosl * Math.Sin(2.0 * (pos[1] - lonp));

            dr[0] = dp * ep[0] + du * eu[0];
            dr[1] = dp * ep[1] + du * eu[1];
            dr[2] = dp * ep[2] + du * eu[2];

            GlobalMembersRtkcmn.trace(5, "tide_pl : dr=%.3f %.3f %.3f\n", dr[0], dr[1], dr[2]);
        }
        /* displacement by solid earth tide (ref [2] 7) ------------------------------*/
        internal static void tide_solid(double rsun, double rmoon, double[] pos, double[] E, double gmst, int opt, double[] dr)
        {
            double[] dr1 = new double[3];
            double[] dr2 = new double[3];
            double[] eu = new double[3];
            double du;
            double dn;
            double sinl;
            double sin2l;

            GlobalMembersRtkcmn.trace(3, "tide_solid: pos=%.3f %.3f opt=%d\n", pos[0] * 180.0 / DefineConstants.PI, pos[1] * 180.0 / DefineConstants.PI, opt);

            /* step1: time domain */
            eu[0] = E[2];
            eu[1] = E[5];
            eu[2] = E[8];
            GlobalMembersPpp.tide_pl(eu, rsun, DefineConstants.GMS, pos, dr1);
            GlobalMembersPpp.tide_pl(eu, rmoon, DefineConstants.GMM, pos, dr2);

            /* step2: frequency domain, only K1 radial */
            sin2l = Math.Sin(2.0 * pos[0]);
            du = -0.012 * sin2l * Math.Sin(gmst + pos[1]);

            dr[0] = dr1[0] + dr2[0] + du * E[2];
            dr[1] = dr1[1] + dr2[1] + du * E[5];
            dr[2] = dr1[2] + dr2[2] + du * E[8];

            /* eliminate permanent deformation */
            if ((opt & 8) != 0)
            {
                sinl = Math.Sin(pos[0]);
                du = 0.1196 * (1.5 * sinl * sinl - 0.5);
                dn = 0.0247 * sin2l;
                dr[0] += du * E[2] + dn * E[1];
                dr[1] += du * E[5] + dn * E[4];
                dr[2] += du * E[8] + dn * E[7];
            }
            GlobalMembersRtkcmn.trace(5, "tide_solid: dr=%.3f %.3f %.3f\n", dr[0], dr[1], dr[2]);
        }
        /* displacement by ocean tide loading (ref [2] 7) ----------------------------*/
        internal static void tide_oload(gtime_t tut, double[] odisp, double[] denu)
        {
            double[,] args = { { 1.40519E-4, 2.0, -2.0, 0.0, 0.00 }, { 1.45444E-4, 0.0, 0.0, 0.0, 0.00 }, { 1.37880E-4, 2.0, -3.0, 1.0, 0.00 }, { 1.45842E-4, 2.0, 0.0, 0.0, 0.00 }, { 0.72921E-4, 1.0, 0.0, 0.0, 0.25 }, { 0.67598E-4, 1.0, -2.0, 0.0, -0.25 }, { 0.72523E-4, -1.0, 0.0, 0.0, -0.25 }, { 0.64959E-4, 1.0, -3.0, 1.0, -0.25 }, { 0.53234E-5, 0.0, 2.0, 0.0, 0.00 }, { 0.26392E-5, 0.0, 1.0, -1.0, 0.00 }, { 0.03982E-5, 2.0, 0.0, 0.0, 0.00 } };
            double[] ep1975 = { 1975, 1, 1, 0, 0, 0 };
            double[] ep = new double[6];
            double fday;
            double days;
            double t;
            double t2;
            double t3;
            double[] a = new double[5];
            double ang;
            double[] dp = { 0, null, null };
            int i;
            int j;

            GlobalMembersRtkcmn.trace(3, "tide_oload:\n");

            /* angular argument: see subroutine arg.f for reference [1] */
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: time2epoch(tut,ep);
            GlobalMembersRtkcmn.time2epoch(new gtime_t(tut), ep);
            fday = ep[3] * 3600.0 + ep[4] * 60.0 + ep[5];
            ep[3] = ep[4] = ep[5] = 0.0;
            days = GlobalMembersRtkcmn.timediff(GlobalMembersRtkcmn.epoch2time(ep), GlobalMembersRtkcmn.epoch2time(ep1975)) / 86400.0;
            t = (27392.500528 + 1.000000035 * days) / 36525.0;
            t2 = t * t;
            t3 = t2 * t;

            a[0] = fday;
            a[1] = (279.69668 + 36000.768930485 * t + 3.03E-4 * t2) * DefineConstants.PI / 180.0; // H0
            a[2] = (270.434358 + 481267.88314137 * t - 0.001133 * t2 + 1.9E-6 * t3) * DefineConstants.PI / 180.0; // S0
            a[3] = (334.329653 + 4069.0340329577 * t - 0.010325 * t2 - 1.2E-5 * t3) * DefineConstants.PI / 180.0; // P0
            a[4] = 2.0 * DefineConstants.PI;

            /* displacements by 11 constituents */
            for (i = 0; i < 11; i++)
            {
                ang = 0.0;
                for (j = 0; j < 5; j++)
                {
                    ang += a[j] * args[i, j];
                }
                for (j = 0; j < 3; j++)
                {
                    dp[j] += odisp[j + i * 6] * Math.Cos(ang - odisp[j + 3 + i * 6] * DefineConstants.PI / 180.0);
                }
            }
            denu[0] = -dp[1];
            denu[1] = -dp[2];
            denu[2] = dp[0];

            GlobalMembersRtkcmn.trace(5, "tide_oload: denu=%.3f %.3f %.3f\n", denu[0], denu[1], denu[2]);
        }
        /* iers mean pole (ref [7] eq.7.25) ------------------------------------------*/
        internal static void iers_mean_pole(gtime_t tut, ref double xp_bar, ref double yp_bar)
        {
            double[] ep2000 = { 2000, 1, 1, 0, 0, 0 };
            double y;
            double y2;
            double y3;

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: y=timediff(tut,epoch2time(ep2000))/86400.0/365.25;
            y = GlobalMembersRtkcmn.timediff(new gtime_t(tut), GlobalMembersRtkcmn.epoch2time(ep2000)) / 86400.0 / 365.25;

            if (y < 3653.0 / 365.25) // until 2010.0
            {
                y2 = y * y;
                y3 = y2 * y;
                xp_bar = 55.974 + 1.8243 * y + 0.18413 * y2 + 0.007024 * y3; // (mas)
                yp_bar = 346.346 + 1.7896 * y - 0.10729 * y2 - 0.000908 * y3;
            }
            else // after 2010.0
            {
                xp_bar = 23.513 + 7.6141 * y; // (mas)
                yp_bar = 358.891 - 0.6287 * y;
            }
        }
        /* displacement by pole tide (ref [7] eq.7.26) --------------------------------*/
        internal static void tide_pole(gtime_t tut, double[] pos, double[] erpv, double[] denu)
        {
            double xp_bar;
            double yp_bar;
            double m1;
            double m2;
            double cosl;
            double sinl;

            GlobalMembersRtkcmn.trace(3, "tide_pole: pos=%.3f %.3f\n", pos[0] * 180.0 / DefineConstants.PI, pos[1] * 180.0 / DefineConstants.PI);

            /* iers mean pole (mas) */
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: iers_mean_pole(tut,&xp_bar,&yp_bar);
            GlobalMembersPpp.iers_mean_pole(new gtime_t(tut), ref xp_bar, ref yp_bar);

            /* ref [7] eq.7.24 */
            m1 = erpv[0] / DefineConstants.PI / 180.0 / 3600.0 - xp_bar * 1E-3; // (as)
            m2 = -erpv[1] / DefineConstants.PI / 180.0 / 3600.0 + yp_bar * 1E-3;

            /* sin(2*theta) = sin(2*phi), cos(2*theta)=-cos(2*phi) */
            cosl = Math.Cos(pos[1]);
            sinl = Math.Sin(pos[1]);
            denu[0] = 9E-3 * Math.Sin(pos[0]) * (m1 * sinl - m2 * cosl); // de= Slambda (m)
            denu[1] = -9E-3 * Math.Cos(2.0 * pos[0]) * (m1 * cosl + m2 * sinl); // dn=-Stheta  (m)
            denu[2] = -33E-3 * Math.Sin(2.0 * pos[0]) * (m1 * cosl + m2 * sinl); // du= Sr      (m)

            GlobalMembersRtkcmn.trace(5, "tide_pole : denu=%.3f %.3f %.3f\n", denu[0], denu[1], denu[2]);
        }
        /* tidal displacement ----------------------------------------------------------
        * displacements by earth tides
        * args   : gtime_t tutc     I   time in utc
        *          double *rr       I   site position (ecef) (m)
        *          int    opt       I   options (or of the followings)
        *                                 1: solid earth tide
        *                                 2: ocean tide loading
        *                                 4: pole tide
        *                                 8: elimate permanent deformation
        *          double *erp      I   earth rotation parameters (NULL: not used)
        *          double *odisp    I   ocean loading parameters  (NULL: not used)
        *                                 odisp[0+i*6]: consituent i amplitude radial(m)
        *                                 odisp[1+i*6]: consituent i amplitude west  (m)
        *                                 odisp[2+i*6]: consituent i amplitude south (m)
        *                                 odisp[3+i*6]: consituent i phase radial  (deg)
        *                                 odisp[4+i*6]: consituent i phase west    (deg)
        *                                 odisp[5+i*6]: consituent i phase south   (deg)
        *                                (i=0:M2,1:S2,2:N2,3:K2,4:K1,5:O1,6:P1,7:Q1,
        *                                   8:Mf,9:Mm,10:Ssa)
        *          double *dr       O   displacement by earth tides (ecef) (m)
        * return : none
        * notes  : see ref [1], [2] chap 7
        *          see ref [4] 5.2.1, 5.2.2, 5.2.3
        *          ver.2.4.0 does not use ocean loading and pole tide corrections
        *-----------------------------------------------------------------------------*/
        public static void tidedisp(gtime_t tutc, double[] rr, int opt, erp_t erp, double odisp, double[] dr)
        {
            gtime_t tut = new gtime_t();
            double[] pos = new double[2];
            double[] E = new double[9];
            double[] drt = new double[3];
            double[] denu = new double[3];
            double[] rs = new double[3];
            double[] rm = new double[3];
            double gmst;
            double[] erpv = { 0, null, null, null, null };
            int i;
#if IERS_MODEL
		double[] ep = new double[6];
		double fhr;
		int year;
		int mon;
		int day;
#endif

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: trace(3,"tidedisp: tutc=%s\n",time_str(tutc,0));
            GlobalMembersRtkcmn.trace(3, "tidedisp: tutc=%s\n", GlobalMembersRtkcmn.time_str(new gtime_t(tutc), 0));

            if (erp != null)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: geterp(erp,tutc,erpv);
                GlobalMembersRtkcmn.geterp(erp, new gtime_t(tutc), erpv);
            }

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: tut=timeadd(tutc,erpv[2]);
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            tut.CopyFrom(GlobalMembersRtkcmn.timeadd(new gtime_t(tutc), erpv[2]));

            dr[0] = dr[1] = dr[2] = 0.0;

            if (GlobalMembersRtkcmn.norm(rr, 3) <= 0.0)
                return;

            pos[0] = Math.Asin(rr[2] / GlobalMembersRtkcmn.norm(rr, 3));
            pos[1] = Math.Atan2(rr[1], rr[0]);
            GlobalMembersRtkcmn.xyz2enu(pos, E);

            if ((opt & 1) != 0) // solid earth tides
            {

                /* sun and moon position in ecef */
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: sunmoonpos(tutc,erpv,rs,rm,&gmst);
                GlobalMembersRtkcmn.sunmoonpos(new gtime_t(tutc), erpv, ref rs, ref rm, ref gmst);

#if IERS_MODEL
//C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
//ORIGINAL LINE: time2epoch(tutc,ep);
			GlobalMembersRtkcmn.time2epoch(new gtime_t(tutc), ep);
			year = (int)ep[0];
			mon = (int)ep[1];
			day = (int)ep[2];
			fhr = ep[3] + ep[4] / 60.0 + ep[5] / 3600.0;

			/* call DEHANTTIDEINEL */
			dehanttideinel_(ref (double)rr, ref year, ref mon, ref day, ref fhr, ref rs, ref rm, ref drt);
#else
                GlobalMembersPpp.tide_solid(rs, rm, pos, E, gmst, opt, drt);
#endif
                for (i = 0; i < 3; i++)
                {
                    dr[i] += drt[i];
                }
            }
            if ((opt & 2) && odisp != 0) // ocean tide loading
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: tide_oload(tut,odisp,denu);
                GlobalMembersPpp.tide_oload(new gtime_t(tut), odisp, denu);
                GlobalMembersRtkcmn.matmul("TN", 3, 1, 3, 1.0, E, denu, 0.0, ref drt);
                for (i = 0; i < 3; i++)
                {
                    dr[i] += drt[i];
                }
            }
            if ((opt & 4) && erp != null) // pole tide
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: tide_pole(tut,pos,erpv,denu);
                GlobalMembersPpp.tide_pole(new gtime_t(tut), pos, erpv, denu);
                GlobalMembersRtkcmn.matmul("TN", 3, 1, 3, 1.0, E, denu, 0.0, ref drt);
                for (i = 0; i < 3; i++)
                {
                    dr[i] += drt[i];
                }
            }
            GlobalMembersRtkcmn.trace(5, "tidedisp: dr=%.3f %.3f %.3f\n", dr[0], dr[1], dr[2]);
        }
        /* exclude meas of eclipsing satellite (block IIA) ---------------------------*/
        internal static void testeclipse(obsd_t[] obs, int n, nav_t nav, double[] rs)
        {
            double[] rsun = new double[3];
            double[] esun = new double[3];
            double r;
            double ang;
            double[] erpv = { 0, null, null, null, null };
            double cosa;
            int i;
            int j;
            string type;

            GlobalMembersRtkcmn.trace(3, "testeclipse:\n");

            /* unit vector of sun direction (ecef) */
            GlobalMembersRtkcmn.sunmoonpos(GlobalMembersRtkcmn.gpst2utc(obs[0].time), erpv, ref rsun, null, null);
            GlobalMembersRtkcmn.normv3(rsun, esun);

            for (i = 0; i < n; i++)
            {
                type = nav.pcvs[obs[i].sat - 1].type;

                if ((r = GlobalMembersRtkcmn.norm(rs + i * 6, 3)) <= 0.0)
                    continue;
#if 1
			/* only block IIA */
			if (type != 0 && !StringFunctions.StrStr(type,"BLOCK IIA"))
				continue;
#endif
                /* sun-earth-satellite angle */
                cosa = GlobalMembersRtkcmn.dot(rs + i * 6, esun, 3) / r;
                cosa = cosa < -1.0 ? -1.0 : (cosa > 1.0 ? 1.0 : cosa);
                ang = Math.Acos(cosa);

                /* test eclipse */
                if (ang < DefineConstants.PI / 2.0 || r * Math.Sin(ang) > DefineConstants.RE_WGS84)
                    continue;

                GlobalMembersRtkcmn.trace(2, "eclipsing sat excluded %s sat=%2d\n", GlobalMembersRtkcmn.time_str(obs[0].time, 0), obs[i].sat);

                for (j = 0; j < 3; j++)
                {
                    rs[j + i * 6] = 0.0;
                }
            }
        }
        /* measurement error variance ------------------------------------------------*/
        internal static double varerr(int sat, int sys, double el, int type, prcopt_t opt)
        {
            double a;
            double b;
            double a2;
            double b2;
            double fact = 1.0;
            double sinel = Math.Sin(el);
            int i = sys == DefineConstants.SYS_GLO ? 1 : (sys == DefineConstants.SYS_GAL ? 2 : 0);

            /* extended error model */
            if (type == 1 && opt.exterr.ena[0] != 0) // code
            {
                a = opt.exterr.cerr[i, 0];
                b = opt.exterr.cerr[i, 1];
                if (opt.ionoopt == DefineConstants.IONOOPT_IFLC)
                {
                    a2 = opt.exterr.cerr[i, 2];
                    b2 = opt.exterr.cerr[i, 3];
                    a = Math.Sqrt(((2.55) * (2.55)) * a * a + ((1.55) * (1.55)) * a2 * a2);
                    b = Math.Sqrt(((2.55) * (2.55)) * b * b + ((1.55) * (1.55)) * b2 * b2);
                }
            }
            else if (type == 0 && opt.exterr.ena[1] != 0) // phase
            {
                a = opt.exterr.perr[i, 0];
                b = opt.exterr.perr[i, 1];
                if (opt.ionoopt == DefineConstants.IONOOPT_IFLC)
                {
                    a2 = opt.exterr.perr[i, 2];
                    b2 = opt.exterr.perr[i, 3];
                    a = Math.Sqrt(((2.55) * (2.55)) * a * a + ((1.55) * (1.55)) * a2 * a2);
                    b = Math.Sqrt(((2.55) * (2.55)) * b * b + ((1.55) * (1.55)) * b2 * b2);
                }
            }
            else // normal error model
            {
                if (type == 1)
                {
                    fact *= opt.eratio[0];
                }
                fact *= sys == DefineConstants.SYS_GLO ? DefineConstants.EFACT_GLO : (sys == DefineConstants.SYS_SBS ? DefineConstants.EFACT_SBS : DefineConstants.EFACT_GPS);
                if (opt.ionoopt == DefineConstants.IONOOPT_IFLC)
                {
                    fact *= 3.0;
                }
                a = fact * opt.err[1];
                b = fact * opt.err[2];
            }
            return a * a + b * b / sinel / sinel;
        }
        /* initialize state and covariance -------------------------------------------*/
        internal static void initx(rtk_t rtk, double xi, double @var, int i)
        {
            int j;
            rtk.x[i] = xi;
            for (j = 0; j < rtk.nx; j++)
            {
                rtk.P[i + j * rtk.nx] = rtk.P[j + i * rtk.nx] = i == j != 0 ? @var : 0.0;
            }
        }
        /* dual-frequency iono-free measurements -------------------------------------*/
        internal static int ifmeas(obsd_t obs, nav_t nav, double[] azel, prcopt_t opt, double[] dantr, double[] dants, double phw, double[] meas, double[] @var)
        {
            double[] lam = nav.lam[obs.sat - 1];
            double c1;
            double c2;
            double L1;
            double L2;
            double P1;
            double P2;
            double P1_C1;
            double P2_C2;
            double gamma;
            int i = 0;
            int j = 1;
            int k;

            GlobalMembersRtkcmn.trace(4, "ifmeas  :\n");

            /* L1-L2 for GPS/GLO/QZS, L1-L5 for GAL/SBS */
            if (DefineConstants.NFREQ >= 3 && (GlobalMembersRtkcmn.satsys(obs.sat, null) & (DefineConstants.SYS_GAL | DefineConstants.SYS_SBS)))
            {
                j = 2;
            }

            if (DefineConstants.NFREQ < 2 || lam[i] == 0.0 || lam[j] == 0.0)
            {
                return 0;
            }

            /* test snr mask */
            if (GlobalMembersRtkcmn.testsnr(0, i, azel[1], obs.SNR[i] * 0.25, opt.snrmask) != 0 || GlobalMembersRtkcmn.testsnr(0, j, azel[1], obs.SNR[j] * 0.25, opt.snrmask) != 0)
            {
                return 0;
            }
            gamma = ((lam[j]) * (lam[j])) / ((lam[i]) * (lam[i]));
            c1 = gamma / (gamma - 1.0); //  f1^2/(f1^2-f2^2)
            c2 = -1.0 / (gamma - 1.0); // -f2^2/(f1^2-f2^2)

            L1 = obs.L[i] * lam[i]; // cycle -> m
            L2 = obs.L[j] * lam[j];
            P1 = obs.P[i];
            P2 = obs.P[j];
            P1_C1 = nav.cbias[obs.sat - 1, 1];
            P2_C2 = nav.cbias[obs.sat - 1, 2];
            if (opt.sateph == DefineConstants.EPHOPT_LEX)
            {
                P1_C1 = nav.lexeph[obs.sat - 1].isc[0] * DefineConstants.CLIGHT; // ISC_L1C/A
            }
            if (L1 == 0.0 || L2 == 0.0 || P1 == 0.0 || P2 == 0.0)
            {
                return 0;
            }

            /* iono-free phase with windup correction */
            meas[0] = c1 * L1 + c2 * L2 - (c1 * lam[i] + c2 * lam[j]) * phw;

            /* iono-free code with dcb correction */
            if (obs.code[i] == DefineConstants.CODE_L1C) // C1->P1
            {
                P1 += P1_C1;
            }
            if (obs.code[j] == DefineConstants.CODE_L2C) // C2->P2
            {
                P2 += P2_C2;
            }
            meas[1] = c1 * P1 + c2 * P2;
            @var[1] = ((DefineConstants.ERR_CBIAS) * (DefineConstants.ERR_CBIAS));

            if (opt.sateph == DefineConstants.EPHOPT_SBAS) // sbas clock based C1
            {
                meas[1] -= P1_C1;
            }

            /* gps-glonass h/w bias correction for code */
            if (opt.exterr.ena[3] != 0 && GlobalMembersRtkcmn.satsys(obs.sat, null) == DefineConstants.SYS_GLO)
            {
                meas[1] += c1 * opt.exterr.gpsglob[0] + c2 * opt.exterr.gpsglob[1];
            }
            /* antenna phase center variation correction */
            for (k = 0; k < 2; k++)
            {
                if (dants != 0)
                {
                    meas[k] -= c1 * dants[i] + c2 * dants[j];
                }
                if (dantr != 0)
                {
                    meas[k] -= c1 * dantr[i] + c2 * dantr[j];
                }
            }
            return 1;
        }
        /* get tgd parameter (m) -----------------------------------------------------*/
        internal static double gettgd(int sat, nav_t nav)
        {
            int i;
            for (i = 0; i < nav.n; i++)
            {
                if (nav.eph[i].sat != sat)
                    continue;
                return DefineConstants.CLIGHT * nav.eph[i].tgd[0];
            }
            return 0.0;
        }
        /* slant ionospheric delay ---------------------------------------------------*/
        internal static int corr_ion(gtime_t time, nav_t nav, int sat, double pos, double azel, int ionoopt, ref double ion, ref double @var, ref int brk)
        {
#if EXTSTEC
		double rate;
#endif
            /* sbas ionosphere model */
            if (ionoopt == DefineConstants.IONOOPT_SBAS)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: return sbsioncorr(time,nav,pos,azel,ion,var);
                return GlobalMembersSbas.sbsioncorr(new gtime_t(time), nav, pos, azel, ion, @var);
            }
            /* ionex tec model */
            if (ionoopt == DefineConstants.IONOOPT_TEC)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: return iontec(time,nav,pos,azel,1,ion,var);
                return GlobalMembersIonex.iontec(new gtime_t(time), nav, pos, azel, 1, ref ion, ref @var);
            }
#if EXTSTEC
		/* slant tec model */
		if (ionoopt == DefineConstants.IONOOPT_STEC)
		{
			return stec_ion(time, nav, sat, pos, azel, ion, rate, @var, brk);
		}
#endif
            /* broadcast model */
            if (ionoopt == DefineConstants.IONOOPT_BRDC)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: *ion=ionmodel(time,nav->ion_gps,pos,azel);
                ion = GlobalMembersRtkcmn.ionmodel(new gtime_t(time), nav.ion_gps, pos, azel);
                @var = ((ion * DefineConstants.ERR_BRDCI) * (ion * DefineConstants.ERR_BRDCI));
                return 1;
            }
            /* ionosphere model off */
            ion = 0.0;
            @var = (10.0) * (10.0);
            return 1;
        }
        /* ionosphere and antenna corrected measurements -----------------------------*/
        internal static int corrmeas(obsd_t obs, nav_t nav, double pos, double[] azel, prcopt_t opt, double[] dantr, double[] dants, double phw, double[] meas, double[] @var, ref int brk)
        {
            double[] lam = nav.lam[obs.sat - 1];
            double ion = 0.0;
            double L1;
            double P1;
            double PC;
            double P1_P2;
            double P1_C1;
            double vari;
            double gamma;
            int i;

            GlobalMembersRtkcmn.trace(4, "corrmeas:\n");

            meas[0] = meas[1] = @var[0] = @var[1] = 0.0;

            /* iono-free LC */
            if (opt.ionoopt == DefineConstants.IONOOPT_IFLC)
            {
                return GlobalMembersPpp.ifmeas(obs, nav, azel, opt, dantr, dants, phw, meas, @var);
            }
            if (lam[0] == 0.0 || obs.L[0] == 0.0 || obs.P[0] == 0.0)
            {
                return 0;
            }

            if (GlobalMembersRtkcmn.testsnr(0, 0, azel[1], obs.SNR[0] * 0.25, opt.snrmask) != 0)
            {
                return 0;
            }

            L1 = obs.L[0] * lam[0];
            P1 = obs.P[0];

            /* dcb correction */
            gamma = ((lam[1] / lam[0]) * (lam[1] / lam[0]));
            P1_P2 = nav.cbias[obs.sat - 1, 0];
            P1_C1 = nav.cbias[obs.sat - 1, 1];
            if (P1_P2 == 0.0 && (GlobalMembersRtkcmn.satsys(obs.sat, null) & (DefineConstants.SYS_GPS | DefineConstants.SYS_GAL | DefineConstants.SYS_QZS)))
            {
                P1_P2 = (1.0 - gamma) * GlobalMembersPntpos.gettgd(obs.sat, nav);
            }
            if (obs.code[0] == DefineConstants.CODE_L1C) // C1->P1
            {
                P1 += P1_C1;
            }
            PC = P1 - P1_P2 / (1.0 - gamma); // P1->PC

            /* slant ionospheric delay L1 (m) */
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: if (!corr_ion(obs->time,nav,obs->sat,pos,azel,opt->ionoopt,&ion,&vari,brk))
            if (GlobalMembersPpp.corr_ion(new gtime_t(obs.time), nav, obs.sat, pos, azel, opt.ionoopt, ref ion, ref vari, ref brk) == 0)
            {

                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: trace(2,"iono correction error: time=%s sat=%2d ionoopt=%d\n", time_str(obs->time,2),obs->sat,opt->ionoopt);
                GlobalMembersRtkcmn.trace(2, "iono correction error: time=%s sat=%2d ionoopt=%d\n", GlobalMembersRtkcmn.time_str(new gtime_t(obs.time), 2), obs.sat, opt.ionoopt);
                return 0;
            }
            /* ionosphere and windup corrected phase and code */
            meas[0] = L1 + ion - lam[0] * phw;
            meas[1] = PC - ion;

            @var[0] += vari;
            @var[1] += vari + ((DefineConstants.ERR_CBIAS) * (DefineConstants.ERR_CBIAS));

            /* antenna phase center variation correction */
            for (i = 0; i < 2; i++)
            {
                if (dants != 0)
                {
                    meas[i] -= dants[0];
                }
                if (dantr != 0)
                {
                    meas[i] -= dantr[0];
                }
            }
            return 1;
        }
        /* L1/L2 geometry-free phase measurement -------------------------------------*/
        internal static double gfmeas(obsd_t obs, nav_t nav)
        {
            double[] lam = nav.lam[obs.sat - 1];

            if (lam[0] == 0.0 || lam[1] == 0.0 || obs.L[0] == 0.0 || obs.L[1] == 0.0)
            {
                return 0.0;
            }

            return lam[0] * obs.L[0] - lam[1] * obs.L[1];
        }
        /* temporal update of position -----------------------------------------------*/
        internal static void udpos_ppp(rtk_t rtk)
        {
            int i;

            GlobalMembersRtkcmn.trace(3, "udpos_ppp:\n");

            /* fixed mode */
            if (rtk.opt.mode == DefineConstants.PMODE_PPP_FIXED)
            {
                for (i = 0; i < 3; i++)
                {
                    GlobalMembersPpp.initx(rtk, rtk.opt.ru[i], 1E-8, i);
                }
                return;
            }
            /* initialize position for first epoch */
            if (GlobalMembersRtkcmn.norm(rtk.x, 3) <= 0.0)
            {
                for (i = 0; i < 3; i++)
                {
                    GlobalMembersPpp.initx(rtk, rtk.sol.rr[i], (100.0) * (100.0), i);
                }
            }
            /* static ppp mode */
            if (rtk.opt.mode == DefineConstants.PMODE_PPP_STATIC)
                return;

            /* kinmatic mode without dynamics */
            for (i = 0; i < 3; i++)
            {
                GlobalMembersPpp.initx(rtk, rtk.sol.rr[i], (100.0) * (100.0), i);
            }
        }
        /* temporal update of clock --------------------------------------------------*/
        internal static void udclk_ppp(rtk_t rtk)
        {
            double dtr;
            int i;

            GlobalMembersRtkcmn.trace(3, "udclk_ppp:\n");

            /* initialize every epoch for clock (white noise) */
            for (i = 0; i < DefineConstants.NSYSGPS + DefineConstants.NSYSGLO + DefineConstants.NSYSGAL + DefineConstants.NSYSQZS + DefineConstants.NSYSCMP + DefineConstants.NSYSLEO; i++)
            {
                if (rtk.opt.sateph == DefineConstants.EPHOPT_PREC)
                {
                    /* time of prec ephemeris is based gpst */
                    /* negelect receiver inter-system bias  */
                    dtr = rtk.sol.dtr[0];
                }
                else
                {
                    dtr = i == 0 ? rtk.sol.dtr[0] : rtk.sol.dtr[0] + rtk.sol.dtr[i];
                }
                GlobalMembersPpp.initx(rtk, DefineConstants.CLIGHT * dtr, (100.0) * (100.0), (((rtk.opt).dynamics != 0 ? 9 : 3) + (i)));
            }
        }
        /* temporal update of tropospheric parameters --------------------------------*/
        internal static void udtrop_ppp(rtk_t rtk)
        {
            double[] pos = new double[3];
            double[] azel = { 0.0, DefineConstants.PI / 2.0 };
            double ztd;
            double @var;
            int i = ((((rtk.opt).dynamics != 0 ? 9 : 3) + (0)) + DefineConstants.NSYSGPS + DefineConstants.NSYSGLO + DefineConstants.NSYSGAL + DefineConstants.NSYSQZS + DefineConstants.NSYSCMP + DefineConstants.NSYSLEO);
            int j;

            GlobalMembersRtkcmn.trace(3, "udtrop_ppp:\n");

            if (rtk.x[i] == 0.0)
            {
                GlobalMembersRtkcmn.ecef2pos(rtk.sol.rr, pos);
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: ztd=sbstropcorr(rtk->sol.time,pos,azel,&var);
                ztd = GlobalMembersSbas.sbstropcorr(new gtime_t(rtk.sol.time), pos, azel, ref @var);
                GlobalMembersPpp.initx(rtk, ztd, @var, i);

                if (rtk.opt.tropopt >= DefineConstants.TROPOPT_ESTG)
                {
                    for (j = 0; j < 2; j++)
                    {
                        GlobalMembersPpp.initx(rtk, 1E-6, (0.001) * (0.001), ++i);
                    }
                }
            }
            else
            {
                rtk.P[i * (1 + rtk.nx)] += ((rtk.opt.prn[2]) * (rtk.opt.prn[2])) * Math.Abs(rtk.tt);

                if (rtk.opt.tropopt >= DefineConstants.TROPOPT_ESTG)
                {
                    for (j = 0; j < 2; j++)
                    {
                        rtk.P[++i * (1 + rtk.nx)] += ((rtk.opt.prn[2] * 0.1) * (rtk.opt.prn[2] * 0.1)) * Math.Abs(rtk.tt);
                    }
                }
            }
        }
        /* detect cycle slip by LLI --------------------------------------------------*/
        internal static void detslp_ll(rtk_t rtk, obsd_t[] obs, int n)
        {
            int i;
            int j;

            GlobalMembersRtkcmn.trace(3, "detslp_ll: n=%d\n", n);

            for (i = 0; i < n && i < DefineConstants.MAXOBS; i++)
            {
                for (j = 0; j < rtk.opt.nf; j++)
                {
                    if (obs[i].L[j] == 0.0 || !(obs[i].LLI[j] & 3))
                        continue;

                    GlobalMembersRtkcmn.trace(3, "detslp_ll: slip detected sat=%2d f=%d\n", obs[i].sat, j + 1);

                    rtk.ssat[obs[i].sat - 1].slip[j] = 1;
                }
            }
        }
        /* detect cycle slip by geometry free phase jump -----------------------------*/
        internal static void detslp_gf(rtk_t rtk, obsd_t[] obs, int n, nav_t nav)
        {
            double g0;
            double g1;
            int i;
            int j;

            GlobalMembersRtkcmn.trace(3, "detslp_gf: n=%d\n", n);

            for (i = 0; i < n && i < DefineConstants.MAXOBS; i++)
            {

                if ((g1 = GlobalMembersPpp.gfmeas(obs + i, nav)) == 0.0)
                    continue;

                g0 = rtk.ssat[obs[i].sat - 1].gf;
                rtk.ssat[obs[i].sat - 1].gf = g1;

                GlobalMembersRtkcmn.trace(4, "detslip_gf: sat=%2d gf0=%8.3f gf1=%8.3f\n", obs[i].sat, g0, g1);

                if (g0 != 0.0 && Math.Abs(g1 - g0) > rtk.opt.thresslip)
                {
                    GlobalMembersRtkcmn.trace(3, "detslip_gf: slip detected sat=%2d gf=%8.3f->%8.3f\n", obs[i].sat, g0, g1);

                    for (j = 0; j < rtk.opt.nf; j++)
                    {
                        rtk.ssat[obs[i].sat - 1].slip[j] |= 1;
                    }
                }
            }
        }
        /* temporal update of phase biases -------------------------------------------*/
        //C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on the parameter 'rtk', so pointers on this parameter are left unchanged:
        internal static void udbias_ppp(rtk_t* rtk, obsd_t[] obs, int n, nav_t nav)
        {
            double[] meas = new double[2];
            double[] @var = new double[2];
            double[] bias = { 0 };
            double offset = 0.0;
            double[] pos = { 0, null, null };
            int i;
            int j;
            int k;
            int sat;
            int brk = 0;

            GlobalMembersRtkcmn.trace(3, "udbias  : n=%d\n", n);

            for (i = 0; i < DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1; i++)
            {
                for (j = 0; j < rtk.opt.nf; j++)
                {
                    rtk.ssat[i].slip[j] = 0;
                }
            }
            /* detect cycle slip by LLI */
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: detslp_ll(rtk,obs,n);
            GlobalMembersPpp.detslp_ll(new rtk_t(rtk), new obsd_t(obs), n);

            /* detect cycle slip by geometry-free phase jump */
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: detslp_gf(rtk,obs,n,nav);
            GlobalMembersPpp.detslp_gf(new rtk_t(rtk), new obsd_t(obs), n, nav);

            /* reset phase-bias if expire obs outage counter */
            for (i = 0; i < DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1; i++)
            {
                if (++rtk.ssat[i].outc[0] > (uint)rtk.opt.maxout)
                {
                    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                    //ORIGINAL LINE: initx(rtk,0.0,0.0,((((((&rtk->opt)->dynamics?9:3)+(0))+DefineConstants.NSYSGPS+DefineConstants.NSYSGLO+DefineConstants.NSYSGAL+DefineConstants.NSYSQZS+DefineConstants.NSYSCMP+DefineConstants.NSYSLEO)+((&rtk->opt)->tropopt<DefineConstants.TROPOPT_EST?0:((&rtk->opt)->tropopt==DefineConstants.TROPOPT_EST?1:3)))+(i+1)-1));
                    GlobalMembersPpp.initx(new rtk_t(rtk), 0.0, 0.0, ((((((rtk.opt).dynamics != 0 ? 9 : 3) + (0)) + DefineConstants.NSYSGPS + DefineConstants.NSYSGLO + DefineConstants.NSYSGAL + DefineConstants.NSYSQZS + DefineConstants.NSYSCMP + DefineConstants.NSYSLEO) + ((rtk.opt).tropopt < DefineConstants.TROPOPT_EST ? 0 : ((rtk.opt).tropopt == DefineConstants.TROPOPT_EST ? 1 : 3))) + (i + 1) - 1));
                }
            }
            GlobalMembersRtkcmn.ecef2pos(rtk.sol.rr, pos);

            for (i = k = 0; i < n && i < DefineConstants.MAXOBS; i++)
            {
                sat = obs[i].sat;
                j = ((((((rtk.opt).dynamics != 0 ? 9 : 3) + (0)) + DefineConstants.NSYSGPS + DefineConstants.NSYSGLO + DefineConstants.NSYSGAL + DefineConstants.NSYSQZS + DefineConstants.NSYSCMP + DefineConstants.NSYSLEO) + ((rtk.opt).tropopt < DefineConstants.TROPOPT_EST ? 0 : ((rtk.opt).tropopt == DefineConstants.TROPOPT_EST ? 1 : 3))) + (sat) - 1);
                if (GlobalMembersPpp.corrmeas(obs + i, nav, pos, rtk.ssat[sat - 1].azel, rtk.opt, null, null, 0.0, meas, @var, ref brk) == 0)
                    continue;

                if (brk != 0)
                {
                    rtk.ssat[sat - 1].slip[0] = 1;
                    GlobalMembersRtkcmn.trace(2, "%s: sat=%2d correction break\n", GlobalMembersRtkcmn.time_str(obs[i].time, 0), sat);
                }
                bias[i] = meas[0] - meas[1];
                if (rtk.x[j] == 0.0 || rtk.ssat[sat - 1].slip[0] != 0 || rtk.ssat[sat - 1].slip[1] != 0)
                    continue;
                offset += bias[i] - rtk.x[j];
                k++;
            }
            /* correct phase-code jump to enssure phase-code coherency */
            if (k >= 2 && Math.Abs(offset / k) > 0.0005 * DefineConstants.CLIGHT)
            {
                for (i = 0; i < DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1; i++)
                {
                    j = ((((((rtk.opt).dynamics != 0 ? 9 : 3) + (0)) + DefineConstants.NSYSGPS + DefineConstants.NSYSGLO + DefineConstants.NSYSGAL + DefineConstants.NSYSQZS + DefineConstants.NSYSCMP + DefineConstants.NSYSLEO) + ((rtk.opt).tropopt < DefineConstants.TROPOPT_EST ? 0 : ((rtk.opt).tropopt == DefineConstants.TROPOPT_EST ? 1 : 3))) + (i + 1) - 1);
                    if (rtk.x[j] != 0.0)
                    {
                        rtk.x[j] += offset / k;
                    }
                }
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: trace(2,"phase-code jump corrected: %s n=%2d dt=%12.9fs\n", time_str(rtk->sol.time,0),k,offset/k/DefineConstants.CLIGHT);
                GlobalMembersRtkcmn.trace(2, "phase-code jump corrected: %s n=%2d dt=%12.9fs\n", GlobalMembersRtkcmn.time_str(new gtime_t(rtk.sol.time), 0), k, offset / k / DefineConstants.CLIGHT);
            }
            for (i = 0; i < n && i < DefineConstants.MAXOBS; i++)
            {
                sat = obs[i].sat;
                j = ((((((rtk.opt).dynamics != 0 ? 9 : 3) + (0)) + DefineConstants.NSYSGPS + DefineConstants.NSYSGLO + DefineConstants.NSYSGAL + DefineConstants.NSYSQZS + DefineConstants.NSYSCMP + DefineConstants.NSYSLEO) + ((rtk.opt).tropopt < DefineConstants.TROPOPT_EST ? 0 : ((rtk.opt).tropopt == DefineConstants.TROPOPT_EST ? 1 : 3))) + (sat) - 1);

                rtk.P[j + j * rtk.nx] += ((rtk.opt.prn[0]) * (rtk.opt.prn[0])) * Math.Abs(rtk.tt);

                if (rtk.x[j] != 0.0 && rtk.ssat[sat - 1].slip[0] == 0 && rtk.ssat[sat - 1].slip[1] == 0)
                    continue;

                if (bias[i] == 0.0)
                    continue;

                /* reinitialize phase-bias if detecting cycle slip */
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: initx(rtk,bias[i],(100.0)*(100.0),((((((&rtk->opt)->dynamics?9:3)+(0))+DefineConstants.NSYSGPS+DefineConstants.NSYSGLO+DefineConstants.NSYSGAL+DefineConstants.NSYSQZS+DefineConstants.NSYSCMP+DefineConstants.NSYSLEO)+((&rtk->opt)->tropopt<DefineConstants.TROPOPT_EST?0:((&rtk->opt)->tropopt==DefineConstants.TROPOPT_EST?1:3)))+(sat)-1));
                GlobalMembersPpp.initx(new rtk_t(rtk), bias[i], (100.0) * (100.0), ((((((rtk.opt).dynamics != 0 ? 9 : 3) + (0)) + DefineConstants.NSYSGPS + DefineConstants.NSYSGLO + DefineConstants.NSYSGAL + DefineConstants.NSYSQZS + DefineConstants.NSYSCMP + DefineConstants.NSYSLEO) + ((rtk.opt).tropopt < DefineConstants.TROPOPT_EST ? 0 : ((rtk.opt).tropopt == DefineConstants.TROPOPT_EST ? 1 : 3))) + (sat) - 1));

                GlobalMembersRtkcmn.trace(5, "udbias_ppp: sat=%2d bias=%.3f\n", sat, meas[0] - meas[1]);
            }
        }
        /* temporal update of states --------------------------------------------------*/
        internal static void udstate_ppp(rtk_t rtk, obsd_t obs, int n, nav_t nav)
        {
            GlobalMembersRtkcmn.trace(3, "udstate_ppp: n=%d\n", n);

            /* temporal update of position */
            GlobalMembersPpp.udpos_ppp(rtk);

            /* temporal update of clock */
            GlobalMembersPpp.udclk_ppp(rtk);

            /* temporal update of tropospheric parameters */
            if (rtk.opt.tropopt >= DefineConstants.TROPOPT_EST)
            {
                GlobalMembersPpp.udtrop_ppp(rtk);
            }
            /* temporal update of phase-bias */
            GlobalMembersPpp.udbias_ppp(rtk, obs, n, nav);
        }
        /* satellite antenna phase center variation ----------------------------------*/
        internal static void satantpcv(double[] rs, double[] rr, pcv_t pcv, ref double dant)
        {
            double[] ru = new double[3];
            double[] rz = new double[3];
            double[] eu = new double[3];
            double[] ez = new double[3];
            double nadir;
            double cosa;
            int i;

            for (i = 0; i < 3; i++)
            {
                ru[i] = rr[i] - rs[i];
                rz[i] = -rs[i];
            }
            if (GlobalMembersRtkcmn.normv3(ru, eu) == 0 || GlobalMembersRtkcmn.normv3(rz, ez) == 0)
                return;

            cosa = GlobalMembersRtkcmn.dot(eu, ez, 3);
            cosa = cosa < -1.0 ? -1.0 : (cosa > 1.0 ? 1.0 : cosa);
            nadir = Math.Acos(cosa);

            GlobalMembersRtkcmn.antmodel_s(pcv, nadir, dant);
        }
        /* precise tropospheric model ------------------------------------------------*/
        internal static double prectrop(gtime_t time, double pos, double[] azel, prcopt_t opt, double[] x, double[] dtdx, ref double @var)
        {
            double[] zazel = { 0.0, DefineConstants.PI / 2.0 };
            double zhd;
            double m_h;
            double m_w;
            double cotz;
            double grad_n;
            double grad_e;

            /* zenith hydrostatic delay */
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: zhd=tropmodel(time,pos,zazel,0.0);
            zhd = GlobalMembersRtkcmn.tropmodel(new gtime_t(time), pos, zazel, 0.0);

            /* mapping function */
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: m_h=tropmapf(time,pos,azel,&m_w);
            m_h = GlobalMembersRtkcmn.tropmapf(new gtime_t(time), pos, azel, ref m_w);

            if ((opt.tropopt == DefineConstants.TROPOPT_ESTG || opt.tropopt == DefineConstants.TROPOPT_CORG) && azel[1] > 0.0)
            {

                /* m_w=m_0+m_0*cot(el)*(Gn*cos(az)+Ge*sin(az)): ref [6] */
                cotz = 1.0 / Math.Tan(azel[1]);
                grad_n = m_w * cotz * Math.Cos(azel[0]);
                grad_e = m_w * cotz * Math.Sin(azel[0]);
                m_w += grad_n * x[1] + grad_e * x[2];
                dtdx[1] = grad_n * (x[0] - zhd);
                dtdx[2] = grad_e * (x[0] - zhd);
            }
            dtdx[0] = m_w;
            @var = ((0.01) * (0.01));
            return m_h * zhd + m_w * (x[0] - zhd);
        }
        /* phase and code residuals --------------------------------------------------*/
        internal static int res_ppp(int iter, obsd_t[] obs, int n, double rs, double[] dts, double[] vare, int[] svh, nav_t nav, double[] x, rtk_t rtk, double[] v, double[] H, double[] R, double[] azel)
        {
            prcopt_t opt = rtk.opt;
            double r;
            double[] rr = new double[3];
            double[] disp = new double[3];
            double[] pos = new double[3];
            double[] e = new double[3];
            double[] meas = new double[2];
            double[] dtdx = new double[3];
            double[] dantr = { 0 };
            double[] dants = { 0 };
            double[] @var = new double[DefineConstants.MAXOBS * 2];
            double dtrp = 0.0;
            double vart = 0.0;
            double[] varm = { 0, null };
            int i;
            int j;
            int k;
            int sat;
            int sys;
            int nv = 0;
            int nx = rtk.nx;
            int brk;
            int tideopt;

            GlobalMembersRtkcmn.trace(3, "res_ppp : n=%d nx=%d\n", n, nx);

            for (i = 0; i < DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1; i++)
            {
                rtk.ssat[i].vsat[0] = 0;
            }

            for (i = 0; i < 3; i++)
            {
                rr[i] = x[i];
            }

            /* earth tides correction */
            if (opt.tidecorr != 0)
            {
                tideopt = opt.tidecorr == 1 ? 1 : 7; // 1:solid, 2:solid+otl+pole

                GlobalMembersPpp.tidedisp(GlobalMembersRtkcmn.gpst2utc(obs[0].time), rr, tideopt, nav.erp, opt.odisp[0], disp);
                for (i = 0; i < 3; i++)
                {
                    rr[i] += disp[i];
                }
            }
            GlobalMembersRtkcmn.ecef2pos(rr, pos);

            for (i = 0; i < n && i < DefineConstants.MAXOBS; i++)
            {
                sat = obs[i].sat;
                if ((sys = GlobalMembersRtkcmn.satsys(sat, null)) == 0 || rtk.ssat[sat - 1].vs == 0)
                    continue;

                /* geometric distance/azimuth/elevation angle */
                if ((r = GlobalMembersRtkcmn.geodist(rs + i * 6, rr, e)) <= 0.0 || GlobalMembersRtkcmn.satazel(pos, e, azel + i * 2) < opt.elmin)
                    continue;

                /* excluded satellite? */
                if (GlobalMembersRtkcmn.satexclude(obs[i].sat, svh[i], opt) != 0)
                    continue;

                /* tropospheric delay correction */
                if (opt.tropopt == DefineConstants.TROPOPT_SAAS)
                {
                    dtrp = GlobalMembersRtkcmn.tropmodel(obs[i].time, pos, azel + i * 2, DefineConstants.REL_HUMI);
                    vart = ((DefineConstants.ERR_SAAS) * (DefineConstants.ERR_SAAS));
                }
                else if (opt.tropopt == DefineConstants.TROPOPT_SBAS)
                {
                    dtrp = GlobalMembersSbas.sbstropcorr(obs[i].time, pos, azel + i * 2, ref vart);
                }
                else if (opt.tropopt == DefineConstants.TROPOPT_EST || opt.tropopt == DefineConstants.TROPOPT_ESTG)
                {
                    dtrp = GlobalMembersPpp.prectrop(obs[i].time, pos, azel + i * 2, opt, x + ((((opt).dynamics != 0 ? 9 : 3) + (0)) + DefineConstants.NSYSGPS + DefineConstants.NSYSGLO + DefineConstants.NSYSGAL + DefineConstants.NSYSQZS + DefineConstants.NSYSCMP + DefineConstants.NSYSLEO), dtdx, ref vart);
                }
                else if (opt.tropopt == DefineConstants.TROPOPT_COR || opt.tropopt == DefineConstants.TROPOPT_CORG)
                {
                    dtrp = GlobalMembersPpp.prectrop(obs[i].time, pos, azel + i * 2, opt, x, dtdx, ref vart);
                }
                /* satellite antenna model */
                if (opt.posopt[0] != 0)
                {
                    GlobalMembersPpp.satantpcv(rs + i * 6, rr, nav.pcvs + sat - 1, ref dants);
                }
                /* receiver antenna model */
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: antmodel(opt->pcvr,opt->antdel[0],azel+i *2,opt->posopt[1],dantr);
                GlobalMembersRtkcmn.antmodel(new pcv_t(opt.pcvr), opt.antdel[0], azel + i * 2, opt.posopt[1], dantr);

                /* phase windup correction */
                if (opt.posopt[2] != 0)
                {
                    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                    //ORIGINAL LINE: windupcorr(rtk->sol.time,rs+i *6,rr,&rtk->ssat[sat-1].phw);
                    GlobalMembersRtkcmn.windupcorr(new gtime_t(rtk.sol.time), rs + i * 6, rr, ref rtk.ssat[sat - 1].phw);
                }
                /* ionosphere and antenna phase corrected measurements */
                if (GlobalMembersPpp.corrmeas(obs + i, nav, pos, azel + i * 2, rtk.opt, dantr, dants, rtk.ssat[sat - 1].phw, meas, varm, ref brk) == 0)
                {
                    continue;
                }
                /* satellite clock and tropospheric delay */
                r += -DefineConstants.CLIGHT * dts[i * 2] + dtrp;

                GlobalMembersRtkcmn.trace(5, "sat=%2d azel=%6.1f %5.1f dtrp=%.3f dantr=%6.3f %6.3f dants=%6.3f %6.3f phw=%6.3f\n", sat, azel[i * 2] * 180.0 / DefineConstants.PI, azel[1 + i * 2] * 180.0 / DefineConstants.PI, dtrp, dantr[0], dantr[1], dants[0], dants[1], rtk.ssat[sat - 1].phw);

                for (j = 0; j < 2; j++) // for phase and code
                {

                    if (meas[j] == 0.0)
                        continue;

                    for (k = 0; k < nx; k++)
                    {
                        H[k + nx * nv] = 0.0;
                    }

                    v[nv] = meas[j] - r;

                    for (k = 0; k < 3; k++)
                    {
                        H[k + nx * nv] = -e[k];
                    }

                    if (sys != DefineConstants.SYS_GLO)
                    {
                        v[nv] -= x[(((opt).dynamics != 0 ? 9 : 3) + (0))];
                        H[(((opt).dynamics != 0 ? 9 : 3) + (0)) + nx * nv] = 1.0;
                    }
                    else
                    {
                        v[nv] -= x[(((opt).dynamics != 0 ? 9 : 3) + (1))];
                        H[(((opt).dynamics != 0 ? 9 : 3) + (1)) + nx * nv] = 1.0;
                    }
                    if (opt.tropopt >= DefineConstants.TROPOPT_EST)
                    {
                        for (k = 0; k < (opt.tropopt >= DefineConstants.TROPOPT_ESTG ? 3 : 1); k++)
                        {
                            H[((((opt).dynamics != 0 ? 9 : 3) + (0)) + DefineConstants.NSYSGPS + DefineConstants.NSYSGLO + DefineConstants.NSYSGAL + DefineConstants.NSYSQZS + DefineConstants.NSYSCMP + DefineConstants.NSYSLEO) + k + nx * nv] = dtdx[k];
                        }
                    }
                    if (j == 0)
                    {
                        v[nv] -= x[((((((opt).dynamics != 0 ? 9 : 3) + (0)) + DefineConstants.NSYSGPS + DefineConstants.NSYSGLO + DefineConstants.NSYSGAL + DefineConstants.NSYSQZS + DefineConstants.NSYSCMP + DefineConstants.NSYSLEO) + ((opt).tropopt < DefineConstants.TROPOPT_EST ? 0 : ((opt).tropopt == DefineConstants.TROPOPT_EST ? 1 : 3))) + (obs[i].sat) - 1)];
                        H[((((((opt).dynamics != 0 ? 9 : 3) + (0)) + DefineConstants.NSYSGPS + DefineConstants.NSYSGLO + DefineConstants.NSYSGAL + DefineConstants.NSYSQZS + DefineConstants.NSYSCMP + DefineConstants.NSYSLEO) + ((opt).tropopt < DefineConstants.TROPOPT_EST ? 0 : ((opt).tropopt == DefineConstants.TROPOPT_EST ? 1 : 3))) + (obs[i].sat) - 1) + nx * nv] = 1.0;
                    }
                    @var[nv] = GlobalMembersPntpos.varerr(obs[i].sat, sys, azel[1 + i * 2], j, opt) + varm[j] + vare[i] + vart;

                    if (j == 0)
                    {
                        rtk.ssat[sat - 1].resc[0] = v[nv];
                    }
                    else
                    {
                        rtk.ssat[sat - 1].resp[0] = v[nv];
                    }

                    /* test innovation */
#if false
	//            if (opt->maxinno>0.0&&fabs(v[nv])>opt->maxinno) {
#else
                    if (opt.maxinno > 0.0 && Math.Abs(v[nv]) > opt.maxinno && sys != DefineConstants.SYS_GLO)
                    {
#endif
                        GlobalMembersRtkcmn.trace(2, "ppp outlier rejected %s sat=%2d type=%d v=%.3f\n", GlobalMembersRtkcmn.time_str(obs[i].time, 0), sat, j, v[nv]);
                        rtk.ssat[sat - 1].rejc[0]++;
                        continue;
                    }
                    if (j == 0)
                    {
                        rtk.ssat[sat - 1].vsat[0] = 1;
                    }
                    nv++;
                }
            }
            for (i = 0; i < nv; i++)
            {
                for (j = 0; j < nv; j++)
                {
                    R[i + j * nv] = i == j != 0 ? @var[i] : 0.0;
                }
            }
            GlobalMembersRtkcmn.trace(5, "x=\n");
            GlobalMembersRtkcmn.tracemat(5, x, 1, nx, 8, 3);
            GlobalMembersRtkcmn.trace(5, "v=\n");
            GlobalMembersRtkcmn.tracemat(5, v, 1, nv, 8, 3);
            GlobalMembersRtkcmn.trace(5, "H=\n");
            GlobalMembersRtkcmn.tracemat(5, H, nx, nv, 8, 3);
            GlobalMembersRtkcmn.trace(5, "R=\n");
            GlobalMembersRtkcmn.tracemat(5, R, nv, nv, 8, 5);
            return nv;
        }
        /* number of estimated states ------------------------------------------------*/
        public static int pppnx(prcopt_t opt)
        {
            return (((((((opt).dynamics != 0 ? 9 : 3) + (0)) + DefineConstants.NSYSGPS + DefineConstants.NSYSGLO + DefineConstants.NSYSGAL + DefineConstants.NSYSQZS + DefineConstants.NSYSCMP + DefineConstants.NSYSLEO) + ((opt).tropopt < DefineConstants.TROPOPT_EST ? 0 : ((opt).tropopt == DefineConstants.TROPOPT_EST ? 1 : 3))) + (DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1) - 1) + 1);
        }
        /* precise point positioning -------------------------------------------------*/
        public static void pppos(rtk_t rtk, obsd_t[] obs, int n, nav_t nav)
        {
            prcopt_t opt = rtk.opt;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: double *rs,*dts,*var,*v,*H,*R,*azel,*xp,*Pp;
            double rs;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: double *dts;
            double dts;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: double *var;
            double @var;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: double *v;
            double v;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: double *H;
            double H;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: double *R;
            double R;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: double *azel;
            double azel;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: double *xp;
            double xp;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: double *Pp;
            double Pp;
            int i;
            int nv;
            int info;
            int[] svh = new int[DefineConstants.MAXOBS];
            int stat = DefineConstants.SOLQ_SINGLE;

            GlobalMembersRtkcmn.trace(3, "pppos   : nx=%d n=%d\n", rtk.nx, n);

            rs = GlobalMembersRtkcmn.mat(6, n);
            dts = GlobalMembersRtkcmn.mat(2, n);
            @var = GlobalMembersRtkcmn.mat(1, n);
            azel = GlobalMembersRtkcmn.zeros(2, n);

            for (i = 0; i < DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1; i++)
            {
                rtk.ssat[i].fix[0] = 0;
            }

            /* temporal update of states */
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: udstate_ppp(rtk,obs,n,nav);
            GlobalMembersPpp.udstate_ppp(rtk, new obsd_t(obs), n, nav);

            GlobalMembersRtkcmn.trace(4, "x(0)=");
            GlobalMembersRtkcmn.tracemat(4, rtk.x, 1, (((((opt).dynamics != 0 ? 9 : 3) + (0)) + DefineConstants.NSYSGPS + DefineConstants.NSYSGLO + DefineConstants.NSYSGAL + DefineConstants.NSYSQZS + DefineConstants.NSYSCMP + DefineConstants.NSYSLEO) + ((opt).tropopt < DefineConstants.TROPOPT_EST ? 0 : ((opt).tropopt == DefineConstants.TROPOPT_EST ? 1 : 3))), 13, 4);

            /* satellite positions and clocks */
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: satposs(obs[0].time,obs,n,nav,rtk->opt.sateph,rs,dts,var,svh);
            GlobalMembersEphemeris.satposs(obs[0].time, new obsd_t(obs), n, nav, rtk.opt.sateph, rs, dts, @var, svh);

            /* exclude measurements of eclipsing satellite */
            if (rtk.opt.posopt[3] != 0)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: testeclipse(obs,n,nav,rs);
                GlobalMembersPpp.testeclipse(new obsd_t(obs), n, nav, rs);
            }
            xp = GlobalMembersRtkcmn.mat(rtk.nx, 1);
            Pp = GlobalMembersRtkcmn.zeros(rtk.nx, rtk.nx);
            GlobalMembersRtkcmn.matcpy(ref xp, rtk.x, rtk.nx, 1);
            nv = n * rtk.opt.nf * 2;
            v = GlobalMembersRtkcmn.mat(nv, 1);
            H = GlobalMembersRtkcmn.mat(rtk.nx, nv);
            R = GlobalMembersRtkcmn.mat(nv, nv);

            for (i = 0; i < rtk.opt.niter; i++)
            {

                /* phase and code residuals */
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: if ((nv=res_ppp(i,obs,n,rs,dts,var,svh,nav,xp,rtk,v,H,R,azel))<=0)
                if ((nv = GlobalMembersPpp.res_ppp(i, new obsd_t(obs), n, rs, dts, @var, svh, nav, xp, rtk, v, H, R, azel)) <= 0)
                    break;

                /* measurement update */
                GlobalMembersRtkcmn.matcpy(ref Pp, rtk.P, rtk.nx, rtk.nx);

                if ((info = GlobalMembersRtkcmn.filter(xp, Pp, H, v, R, rtk.nx, nv)) != 0)
                {
                    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                    //ORIGINAL LINE: trace(2,"ppp filter error %s info=%d\n",time_str(rtk->sol.time,0), info);
                    GlobalMembersRtkcmn.trace(2, "ppp filter error %s info=%d\n", GlobalMembersRtkcmn.time_str(new gtime_t(rtk.sol.time), 0), info);
                    break;
                }
                GlobalMembersRtkcmn.trace(4, "x(%d)=", i + 1);
                GlobalMembersRtkcmn.tracemat(4, xp, 1, (((((opt).dynamics != 0 ? 9 : 3) + (0)) + DefineConstants.NSYSGPS + DefineConstants.NSYSGLO + DefineConstants.NSYSGAL + DefineConstants.NSYSQZS + DefineConstants.NSYSCMP + DefineConstants.NSYSLEO) + ((opt).tropopt < DefineConstants.TROPOPT_EST ? 0 : ((opt).tropopt == DefineConstants.TROPOPT_EST ? 1 : 3))), 13, 4);

                stat = DefineConstants.SOLQ_PPP;
            }
            if (stat == DefineConstants.SOLQ_PPP)
            {
                /* postfit residuals */
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: res_ppp(1,obs,n,rs,dts,var,svh,nav,xp,rtk,v,H,R,azel);
                GlobalMembersPpp.res_ppp(1, new obsd_t(obs), n, rs, dts, @var, svh, nav, xp, rtk, v, H, R, azel);

                /* update state and covariance matrix */
                GlobalMembersRtkcmn.matcpy(ref rtk.x, xp, rtk.nx, 1);
                GlobalMembersRtkcmn.matcpy(ref rtk.P, Pp, rtk.nx, rtk.nx);

                /* ambiguity resolution in ppp */
                if (opt.modear == DefineConstants.ARMODE_PPPAR || opt.modear == DefineConstants.ARMODE_PPPAR_ILS)
                {
                    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                    //ORIGINAL LINE: if (pppamb(rtk,obs,n,nav,azel))
                    if (GlobalMembersPpp_ar.pppamb(rtk, new obsd_t(obs), n, nav, azel) != 0)
                    {
                        stat = DefineConstants.SOLQ_FIX;
                    }
                }
                /* update solution status */
                rtk.sol.ns = 0;
                for (i = 0; i < n && i < DefineConstants.MAXOBS; i++)
                {
                    if (rtk.ssat[obs[i].sat - 1].vsat[0] == 0)
                        continue;
                    rtk.ssat[obs[i].sat - 1].@lock[0]++;
                    rtk.ssat[obs[i].sat - 1].outc[0] = 0;
                    rtk.ssat[obs[i].sat - 1].fix[0] = 4;
                    rtk.sol.ns++;
                }
                rtk.sol.stat = stat;

                for (i = 0; i < 3; i++)
                {
                    rtk.sol.rr[i] = rtk.x[i];
                    rtk.sol.qr[i] = (float)rtk.P[i + i * rtk.nx];
                }
                rtk.sol.qr[3] = (float)rtk.P[1];
                rtk.sol.qr[4] = (float)rtk.P[2 + rtk.nx];
                rtk.sol.qr[5] = (float)rtk.P[2];
                rtk.sol.dtr[0] = rtk.x[(((opt).dynamics != 0 ? 9 : 3) + (0))];
                rtk.sol.dtr[1] = rtk.x[(((opt).dynamics != 0 ? 9 : 3) + (1))] - rtk.x[(((opt).dynamics != 0 ? 9 : 3) + (0))];
                for (i = 0; i < n && i < DefineConstants.MAXOBS; i++)
                {
                    rtk.ssat[obs[i].sat - 1].snr[0] = ((obs[i].SNR[0]) <= (obs[i].SNR[1]) ? (obs[i].SNR[0]) : (obs[i].SNR[1]));
                }
                for (i = 0; i < DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1; i++)
                {
                    if (rtk.ssat[i].slip[0] & 3)
                    {
                        rtk.ssat[i].slipc[0]++;
                    }
                }
            }
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(rs);
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(dts);
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(@var);
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(azel);
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(xp);
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(Pp);
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(v);
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(H);
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(R);
        }
    }
}
