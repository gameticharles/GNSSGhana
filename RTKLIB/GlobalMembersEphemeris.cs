using ghGPS.Classes.rcv;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ghGPS.Classes
{
    public static class GlobalMembersEphemeris
    {
        /*------------------------------------------------------------------------------
        * ephemeris.c : satellite ephemeris and clock functions
        *
        *          Copyright (C) 2010-2014 by T.TAKASU, All rights reserved.
        *
        * references :
        *     [1] IS-GPS-200D, Navstar GPS Space Segment/Navigation User Interfaces,
        *         7 March, 2006
        *     [2] Global Navigation Satellite System GLONASS, Interface Control Document
        *         Navigational radiosignal In bands L1, L2, (Edition 5.1), 2008
        *     [3] RTCA/DO-229C, Minimum operational performanc standards for global
        *         positioning system/wide area augmentation system airborne equipment,
        *         RTCA inc, November 28, 2001
        *     [4] RTCM Paper, April 12, 2010, Proposed SSR Messages for SV Orbit Clock,
        *         Code Biases, URA
        *     [5] RTCM Paper 012-2009-SC104-528, January 28, 2009 (previous ver of [4])
        *     [6] RTCM Paper 012-2009-SC104-582, February 2, 2010 (previous ver of [4])
        *     [7] European GNSS (Galileo) Open Service Signal In Space Interface Control
        *         Document, Issue 1, February, 2010
        *     [8] Quasi-Zenith Satellite System Navigation Service Interface Control
        *         Specification for QZSS (IS-QZSS) V1.1, Japan Aerospace Exploration
        *         Agency, July 31, 2009
        *     [9] BeiDou navigation satellite system signal in space interface control
        *         document open service signal B1I (version 1.0), China Satellite
        *         Navigation office, December 2012
        *     [10] RTCM Standard 10403.1 - Amendment 5, Differential GNSS (Global
        *         Navigation Satellite Systems) Services - version 3, July 1, 2011
        *
        * version : $Revision:$ $Date:$
        * history : 2010/07/28 1.1  moved from rtkcmn.c
        *                           added api:
        *                               eph2clk(),geph2clk(),seph2clk(),satantoff()
        *                               satposs()
        *                           changed api:
        *                               eph2pos(),geph2pos(),satpos()
        *                           deleted api:
        *                               satposv(),satposiode()
        *           2010/08/26 1.2  add ephemeris option EPHOPT_LEX
        *           2010/09/09 1.3  fix problem when precise clock outage
        *           2011/01/12 1.4  add api alm2pos()
        *                           change api satpos(),satposs()
        *                           enable valid unhealthy satellites and output status
        *                           fix bug on exception by glonass ephem computation
        *           2013/01/10 1.5  support beidou (compass)
        *                           use newton's method to solve kepler eq.
        *                           update ssr correction algorithm
        *           2013/03/20 1.6  fix problem on ssr clock relativitic correction
        *           2013/09/01 1.7  support negative pseudorange
        *                           fix bug on variance in case of ura ssr = 63
        *           2013/11/11 1.8  change constant MAXAGESSR 70.0 -> 90.0
        *           2014/10/24 1.9  fix bug on return of var_uraeph() if ura<0||15<ura
        *           2014/12/07 1.10 modify MAXDTOE for qzss,gal and bds
        *                           test max number of iteration for Kepler
        *-----------------------------------------------------------------------------*/


        internal const string rcsid = "$Id:$";



        /* variance by ura ephemeris (ref [1] 20.3.3.3.1.1) --------------------------*/
        internal static double var_uraeph(int ura)
        {
            double[] ura_value = { 2.4, 3.4, 4.85, 6.85, 9.65, 13.65, 24.0, 48.0, 96.0, 192.0, 384.0, 768.0, 1536.0, 3072.0, 6144.0 };
            return ura < 0 || 15 < ura != 0 ? ((6144.0) * (6144.0)) : ((ura_value[ura]) * (ura_value[ura]));
        }
        /* variance by ura ssr (ref [4]) ---------------------------------------------*/
        internal static double var_urassr(int ura)
        {
            double std;
            if (ura <= 0)
            {
                return ((DefineConstants.DEFURASSR) * (DefineConstants.DEFURASSR));
            }
            if (ura >= 63)
            {
                return ((5.4665) * (5.4665));
            }
            std = (Math.Pow(3.0, (ura >> 3) & 7) * (1.0 + (ura & 7) / 4.0) - 1.0) * 1E-3;
            return ((std) * (std));
        }
        /* almanac to satellite position and clock bias --------------------------------
        * compute satellite position and clock bias with almanac (gps, galileo, qzss)
        * args   : gtime_t time     I   time (gpst)
        *          alm_t *alm       I   almanac
        *          double *rs       O   satellite position (ecef) {x,y,z} (m)
        *          double *dts      O   satellite clock bias (s)
        * return : none
        * notes  : see ref [1],[7],[8]
        *-----------------------------------------------------------------------------*/
        public static void alm2pos(gtime_t time, alm_t alm, double[] rs, ref double dts)
        {
            double tk;
            double M;
            double E;
            double Ek;
            double sinE;
            double cosE;
            double u;
            double r;
            double i;
            double O;
            double x;
            double y;
            double sinO;
            double cosO;
            double cosi;
            double mu;

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: trace(4,"alm2pos : time=%s sat=%2d\n",time_str(time,3),alm->sat);
            GlobalMembersRtkcmn.trace(4, "alm2pos : time=%s sat=%2d\n", GlobalMembersRtkcmn.time_str(new gtime_t(time), 3), alm.sat);

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: tk=timediff(time,alm->toa);
            tk = GlobalMembersRtkcmn.timediff(new gtime_t(time), new gtime_t(alm.toa));

            if (alm.A <= 0.0)
            {
                rs[0] = rs[1] = rs[2] = dts = 0.0;
                return;
            }
            mu = GlobalMembersRtkcmn.satsys(alm.sat, null) == DefineConstants.SYS_GAL ? DefineConstants.MU_GAL : DefineConstants.MU_GPS;

            M = alm.M0 + Math.Sqrt(mu / (alm.A * alm.A * alm.A)) * tk;
            for (E = M, sinE = Ek = 0.0; Math.Abs(E - Ek) > 1E-12;)
            {
                Ek = E;
                sinE = Math.Sin(Ek);
                E = M + alm.e * sinE;
            }
            cosE = Math.Cos(E);
            u = Math.Atan2(Math.Sqrt(1.0 - alm.e * alm.e) * sinE, cosE - alm.e) + alm.omg;
            r = alm.A * (1.0 - alm.e * cosE);
            i = alm.i0;
            O = alm.OMG0 + (alm.OMGd - DefineConstants.OMGE) * tk - DefineConstants.OMGE * alm.toas;
            x = r * Math.Cos(u);
            y = r * Math.Sin(u);
            sinO = Math.Sin(O);
            cosO = Math.Cos(O);
            cosi = Math.Cos(i);
            rs[0] = x * cosO - y * cosi * sinO;
            rs[1] = x * sinO + y * cosi * cosO;
            rs[2] = y * Math.Sin(i);
            dts = alm.f0 + alm.f1 * tk;
        }
        /* broadcast ephemeris to satellite clock bias ---------------------------------
        * compute satellite clock bias with broadcast ephemeris (gps, galileo, qzss)
        * args   : gtime_t time     I   time by satellite clock (gpst)
        *          eph_t *eph       I   broadcast ephemeris
        * return : satellite clock bias (s) without relativeity correction
        * notes  : see ref [1],[7],[8]
        *          satellite clock does not include relativity correction and tdg
        *-----------------------------------------------------------------------------*/
        public static double eph2clk(gtime_t time, eph_t eph)
        {
            double t;
            int i;

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: trace(4,"eph2clk : time=%s sat=%2d\n",time_str(time,3),eph->sat);
            GlobalMembersRtkcmn.trace(4, "eph2clk : time=%s sat=%2d\n", GlobalMembersRtkcmn.time_str(new gtime_t(time), 3), eph.sat);

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: t=timediff(time,eph->toc);
            t = GlobalMembersRtkcmn.timediff(new gtime_t(time), new gtime_t(eph.toc));

            for (i = 0; i < 2; i++)
            {
                t -= eph.f0 + eph.f1 * t + eph.f2 * t * t;
            }
            return eph.f0 + eph.f1 * t + eph.f2 * t * t;
        }
        /* broadcast ephemeris to satellite position and clock bias --------------------
        * compute satellite position and clock bias with broadcast ephemeris (gps,
        * galileo, qzss)
        * args   : gtime_t time     I   time (gpst)
        *          eph_t *eph       I   broadcast ephemeris
        *          double *rs       O   satellite position (ecef) {x,y,z} (m)
        *          double *dts      O   satellite clock bias (s)
        *          double *var      O   satellite position and clock variance (m^2)
        * return : none
        * notes  : see ref [1],[7],[8]
        *          satellite clock includes relativity correction without code bias
        *          (tgd or bgd)
        *-----------------------------------------------------------------------------*/
        //C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on the parameter 'dts', so pointers on this parameter are left unchanged:
        public static void eph2pos(gtime_t time, eph_t eph, double[] rs, double* dts, ref double @var)
        {
            double tk;
            double M;
            double E;
            double Ek;
            double sinE;
            double cosE;
            double u;
            double r;
            double i;
            double O;
            double sin2u;
            double cos2u;
            double x;
            double y;
            double sinO;
            double cosO;
            double cosi;
            double mu;
            double omge;
            double xg;
            double yg;
            double zg;
            double sino;
            double coso;
            int n;
            int sys;
            int prn;

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: trace(4,"eph2pos : time=%s sat=%2d\n",time_str(time,3),eph->sat);
            GlobalMembersRtkcmn.trace(4, "eph2pos : time=%s sat=%2d\n", GlobalMembersRtkcmn.time_str(new gtime_t(time), 3), eph.sat);

            if (eph.A <= 0.0)
            {
                rs[0] = rs[1] = rs[2] = *dts = @var = 0.0;
                return;
            }
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: tk=timediff(time,eph->toe);
            tk = GlobalMembersRtkcmn.timediff(new gtime_t(time), new gtime_t(eph.toe));

            switch ((sys = GlobalMembersRtkcmn.satsys(eph.sat, ref prn)))
            {
                case DefineConstants.SYS_GAL:
                    mu = DefineConstants.MU_GAL;
                    omge = DefineConstants.OMGE_GAL;
                    break;
                case DefineConstants.SYS_CMP:
                    mu = DefineConstants.MU_CMP;
                    omge = DefineConstants.OMGE_CMP;
                    break;
                default:
                    mu = DefineConstants.MU_GPS;
                    omge = DefineConstants.OMGE;
                    break;
            }
            M = eph.M0 + (Math.Sqrt(mu / (eph.A * eph.A * eph.A)) + eph.deln) * tk;

            for (n = 0, E = M, Ek = 0.0; Math.Abs(E - Ek) > DefineConstants.RTOL_KEPLER && n < DefineConstants.MAX_ITER_KEPLER; n++)
            {
                Ek = E;
                E -= (E - eph.e * Math.Sin(E) - M) / (1.0 - eph.e * Math.Cos(E));
            }
            if (n >= DefineConstants.MAX_ITER_KEPLER)
            {
                GlobalMembersRtkcmn.trace(2, "kepler iteration overflow sat=%2d\n", eph.sat);
                return;
            }
            sinE = Math.Sin(E);
            cosE = Math.Cos(E);

            GlobalMembersRtkcmn.trace(4, "kepler: sat=%2d e=%8.5f n=%2d del=%10.3e\n", eph.sat, eph.e, n, E - Ek);

            u = Math.Atan2(Math.Sqrt(1.0 - eph.e * eph.e) * sinE, cosE - eph.e) + eph.omg;
            r = eph.A * (1.0 - eph.e * cosE);
            i = eph.i0 + eph.idot * tk;
            sin2u = Math.Sin(2.0 * u);
            cos2u = Math.Cos(2.0 * u);
            u += eph.cus * sin2u + eph.cuc * cos2u;
            r += eph.crs * sin2u + eph.crc * cos2u;
            i += eph.cis * sin2u + eph.cic * cos2u;
            x = r * Math.Cos(u);
            y = r * Math.Sin(u);
            cosi = Math.Cos(i);

            /* beidou geo satellite (ref [9]) */
            if (sys == DefineConstants.SYS_CMP && prn <= 5)
            {
                O = eph.OMG0 + eph.OMGd * tk - omge * eph.toes;
                sinO = Math.Sin(O);
                cosO = Math.Cos(O);
                xg = x * cosO - y * cosi * sinO;
                yg = x * sinO + y * cosi * cosO;
                zg = y * Math.Sin(i);
                sino = Math.Sin(omge * tk);
                coso = Math.Cos(omge * tk);
                rs[0] = xg * coso + yg * sino * DefineConstants.COS_5 + zg * sino * DefineConstants.SIN_5;
                rs[1] = -xg * sino + yg * coso * DefineConstants.COS_5 + zg * coso * DefineConstants.SIN_5;
                rs[2] = -yg * DefineConstants.SIN_5 + zg * DefineConstants.COS_5;
            }
            else
            {
                O = eph.OMG0 + (eph.OMGd - omge) * tk - omge * eph.toes;
                sinO = Math.Sin(O);
                cosO = Math.Cos(O);
                rs[0] = x * cosO - y * cosi * sinO;
                rs[1] = x * sinO + y * cosi * cosO;
                rs[2] = y * Math.Sin(i);
            }
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: tk=timediff(time,eph->toc);
            tk = GlobalMembersRtkcmn.timediff(new gtime_t(time), new gtime_t(eph.toc));
            *dts = eph.f0 + eph.f1 * tk + eph.f2 * tk * tk;

            /* relativity correction */
            *dts -= 2.0 * Math.Sqrt(mu * eph.A) * eph.e * sinE / ((DefineConstants.CLIGHT) * (DefineConstants.CLIGHT));

            /* position and clock error variance */
            @var = GlobalMembersEphemeris.var_uraeph(eph.sva);
        }
        /* glonass orbit differential equations --------------------------------------*/
        internal static void deq(double[] x, double[] xdot, double[] acc)
        {
            double a;
            double b;
            double c;
            double r2 = GlobalMembersRtkcmn.dot(x, x, 3);
            double r3 = r2 * Math.Sqrt(r2);
            double omg2 = ((DefineConstants.OMGE_GLO) * (DefineConstants.OMGE_GLO));

            if (r2 <= 0.0)
            {
                xdot[0] = xdot[1] = xdot[2] = xdot[3] = xdot[4] = xdot[5] = 0.0;
                return;
            }
            /* ref [2] A.3.1.2 with bug fix for xdot[4],xdot[5] */
            a = 1.5 * DefineConstants.J2_GLO * DefineConstants.MU_GLO * ((DefineConstants.RE_GLO) * (DefineConstants.RE_GLO)) / r2 / r3;
            b = 5.0 * x[2] * x[2] / r2; // 5*z^2/r^2
            c = -DefineConstants.MU_GLO / r3 - a * (1.0 - b); // -mu/r^3-a(1-b)
            xdot[0] = x[3];
            xdot[1] = x[4];
            xdot[2] = x[5];
            xdot[3] = (c + omg2) * x[0] + 2.0 * DefineConstants.OMGE_GLO * x[4] + acc[0];
            xdot[4] = (c + omg2) * x[1] - 2.0 * DefineConstants.OMGE_GLO * x[3] + acc[1];
            xdot[5] = (c - 2.0 * a) * x[2] + acc[2];
        }
        /* glonass position and velocity by numerical integration --------------------*/
        internal static void glorbit(double t, double[] x, double acc)
        {
            double[] k1 = new double[6];
            double[] k2 = new double[6];
            double[] k3 = new double[6];
            double[] k4 = new double[6];
            double[] w = new double[6];
            int i;

            GlobalMembersEphemeris.deq(x, k1, acc);
            for (i = 0; i < 6; i++)
            {
                w[i] = x[i] + k1[i] * t / 2.0;
            }
            GlobalMembersEphemeris.deq(w, k2, acc);
            for (i = 0; i < 6; i++)
            {
                w[i] = x[i] + k2[i] * t / 2.0;
            }
            GlobalMembersEphemeris.deq(w, k3, acc);
            for (i = 0; i < 6; i++)
            {
                w[i] = x[i] + k3[i] * t;
            }
            GlobalMembersEphemeris.deq(w, k4, acc);
            for (i = 0; i < 6; i++)
            {
                x[i] += (k1[i] + 2.0 * k2[i] + 2.0 * k3[i] + k4[i]) * t / 6.0;
            }
        }
        /* glonass ephemeris to satellite clock bias -----------------------------------
        * compute satellite clock bias with glonass ephemeris
        * args   : gtime_t time     I   time by satellite clock (gpst)
        *          geph_t *geph     I   glonass ephemeris
        * return : satellite clock bias (s)
        * notes  : see ref [2]
        *-----------------------------------------------------------------------------*/
        public static double geph2clk(gtime_t time, geph_t geph)
        {
            double t;
            int i;

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: trace(4,"geph2clk: time=%s sat=%2d\n",time_str(time,3),geph->sat);
            GlobalMembersRtkcmn.trace(4, "geph2clk: time=%s sat=%2d\n", GlobalMembersRtkcmn.time_str(new gtime_t(time), 3), geph.sat);

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: t=timediff(time,geph->toe);
            t = GlobalMembersRtkcmn.timediff(new gtime_t(time), new gtime_t(geph.toe));

            for (i = 0; i < 2; i++)
            {
                t -= -geph.taun + geph.gamn * t;
            }
            return -geph.taun + geph.gamn * t;
        }
        /* glonass ephemeris to satellite position and clock bias ----------------------
        * compute satellite position and clock bias with glonass ephemeris
        * args   : gtime_t time     I   time (gpst)
        *          geph_t *geph     I   glonass ephemeris
        *          double *rs       O   satellite position {x,y,z} (ecef) (m)
        *          double *dts      O   satellite clock bias (s)
        *          double *var      O   satellite position and clock variance (m^2)
        * return : none
        * notes  : see ref [2]
        *-----------------------------------------------------------------------------*/
        public static void geph2pos(gtime_t time, geph_t geph, double[] rs, ref double dts, ref double @var)
        {
            double t;
            double tt;
            double[] x = new double[6];
            int i;

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: trace(4,"geph2pos: time=%s sat=%2d\n",time_str(time,3),geph->sat);
            GlobalMembersRtkcmn.trace(4, "geph2pos: time=%s sat=%2d\n", GlobalMembersRtkcmn.time_str(new gtime_t(time), 3), geph.sat);

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: t=timediff(time,geph->toe);
            t = GlobalMembersRtkcmn.timediff(new gtime_t(time), new gtime_t(geph.toe));

            dts = -geph.taun + geph.gamn * t;

            for (i = 0; i < 3; i++)
            {
                x[i] = geph.pos[i];
                x[i + 3] = geph.vel[i];
            }
            for (tt = t < 0.0 ? -DefineConstants.TSTEP : DefineConstants.TSTEP; Math.Abs(t) > 1E-9; t -= tt)
            {
                if (Math.Abs(t) < DefineConstants.TSTEP)
                {
                    tt = t;
                }
                GlobalMembersEphemeris.glorbit(tt, x, geph.acc);
            }
            for (i = 0; i < 3; i++)
            {
                rs[i] = x[i];
            }

            @var = ((DefineConstants.ERREPH_GLO) * (DefineConstants.ERREPH_GLO));
        }
        /* sbas ephemeris to satellite clock bias --------------------------------------
        * compute satellite clock bias with sbas ephemeris
        * args   : gtime_t time     I   time by satellite clock (gpst)
        *          seph_t *seph     I   sbas ephemeris
        * return : satellite clock bias (s)
        * notes  : see ref [3]
        *-----------------------------------------------------------------------------*/
        public static double seph2clk(gtime_t time, seph_t seph)
        {
            double t;
            int i;

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: trace(4,"seph2clk: time=%s sat=%2d\n",time_str(time,3),seph->sat);
            GlobalMembersRtkcmn.trace(4, "seph2clk: time=%s sat=%2d\n", GlobalMembersRtkcmn.time_str(new gtime_t(time), 3), seph.sat);

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: t=timediff(time,seph->t0);
            t = GlobalMembersRtkcmn.timediff(new gtime_t(time), new gtime_t(seph.t0));

            for (i = 0; i < 2; i++)
            {
                t -= seph.af0 + seph.af1 * t;
            }
            return seph.af0 + seph.af1 * t;
        }
        /* sbas ephemeris to satellite position and clock bias -------------------------
        * compute satellite position and clock bias with sbas ephemeris
        * args   : gtime_t time     I   time (gpst)
        *          seph_t  *seph    I   sbas ephemeris
        *          double  *rs      O   satellite position {x,y,z} (ecef) (m)
        *          double  *dts     O   satellite clock bias (s)
        *          double  *var     O   satellite position and clock variance (m^2)
        * return : none
        * notes  : see ref [3]
        *-----------------------------------------------------------------------------*/
        public static void seph2pos(gtime_t time, seph_t seph, double[] rs, ref double dts, ref double @var)
        {
            double t;
            int i;

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: trace(4,"seph2pos: time=%s sat=%2d\n",time_str(time,3),seph->sat);
            GlobalMembersRtkcmn.trace(4, "seph2pos: time=%s sat=%2d\n", GlobalMembersRtkcmn.time_str(new gtime_t(time), 3), seph.sat);

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: t=timediff(time,seph->t0);
            t = GlobalMembersRtkcmn.timediff(new gtime_t(time), new gtime_t(seph.t0));

            for (i = 0; i < 3; i++)
            {
                rs[i] = seph.pos[i] + seph.vel[i] * t + seph.acc[i] * t * t / 2.0;
            }
            dts = seph.af0 + seph.af1 * t;

            @var = GlobalMembersEphemeris.var_uraeph(seph.sva);
        }
        /* select ephememeris --------------------------------------------------------*/
        internal static eph_t seleph(gtime_t time, int sat, int iode, nav_t nav)
        {
            double t;
            double tmax;
            double tmin;
            int i;
            int j = -1;

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: trace(4,"seleph  : time=%s sat=%2d iode=%d\n",time_str(time,3),sat,iode);
            GlobalMembersRtkcmn.trace(4, "seleph  : time=%s sat=%2d iode=%d\n", GlobalMembersRtkcmn.time_str(new gtime_t(time), 3), sat, iode);

            switch (GlobalMembersRtkcmn.satsys(sat, null))
            {
                case DefineConstants.SYS_QZS:
                    tmax = DefineConstants.MAXDTOE_QZS + 1.0;
                    break;
                case DefineConstants.SYS_GAL:
                    tmax = DefineConstants.MAXDTOE_GAL + 1.0;
                    break;
                case DefineConstants.SYS_CMP:
                    tmax = DefineConstants.MAXDTOE_CMP + 1.0;
                    break;
                default:
                    tmax = DefineConstants.MAXDTOE + 1.0;
                    break;
            }
            tmin = tmax + 1.0;

            for (i = 0; i < nav.n; i++)
            {
                if (nav.eph[i].sat != sat)
                    continue;
                if (iode >= 0 && nav.eph[i].iode != iode)
                    continue;
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: if ((t=fabs(timediff(nav->eph[i].toe,time)))>tmax)
                if ((t = Math.Abs(GlobalMembersRtkcmn.timediff(nav.eph[i].toe, new gtime_t(time)))) > tmax)
                    continue;
                if (iode >= 0)
                {
                    return nav.eph + i;
                }
                if (t <= tmin) // toe closest to time
                {
                    j = i;
                    tmin = t;
                }
            }
            if (iode >= 0 || j < 0)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: trace(2,"no broadcast ephemeris: %s sat=%2d iode=%3d\n",time_str(time,0), sat,iode);
                GlobalMembersRtkcmn.trace(2, "no broadcast ephemeris: %s sat=%2d iode=%3d\n", GlobalMembersRtkcmn.time_str(new gtime_t(time), 0), sat, iode);
                return null;
            }
            return nav.eph + j;
        }
        /* select glonass ephememeris ------------------------------------------------*/
        internal static geph_t selgeph(gtime_t time, int sat, int iode, nav_t nav)
        {
            double t;
            double tmax = DefineConstants.MAXDTOE_GLO;
            double tmin = tmax + 1.0;
            int i;
            int j = -1;

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: trace(4,"selgeph : time=%s sat=%2d iode=%2d\n",time_str(time,3),sat,iode);
            GlobalMembersRtkcmn.trace(4, "selgeph : time=%s sat=%2d iode=%2d\n", GlobalMembersRtkcmn.time_str(new gtime_t(time), 3), sat, iode);

            for (i = 0; i < nav.ng; i++)
            {
                if (nav.geph[i].sat != sat)
                    continue;
                if (iode >= 0 && nav.geph[i].iode != iode)
                    continue;
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: if ((t=fabs(timediff(nav->geph[i].toe,time)))>tmax)
                if ((t = Math.Abs(GlobalMembersRtkcmn.timediff(nav.geph[i].toe, new gtime_t(time)))) > tmax)
                    continue;
                if (iode >= 0)
                {
                    return nav.geph + i;
                }
                if (t <= tmin) // toe closest to time
                {
                    j = i;
                    tmin = t;
                }
            }
            if (iode >= 0 || j < 0)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: trace(3,"no glonass ephemeris  : %s sat=%2d iode=%2d\n",time_str(time,0), sat,iode);
                GlobalMembersRtkcmn.trace(3, "no glonass ephemeris  : %s sat=%2d iode=%2d\n", GlobalMembersRtkcmn.time_str(new gtime_t(time), 0), sat, iode);
                return null;
            }
            return nav.geph + j;
        }
        /* select sbas ephememeris ---------------------------------------------------*/
        internal static seph_t selseph(gtime_t time, int sat, nav_t nav)
        {
            double t;
            double tmax = DefineConstants.MAXDTOE_SBS;
            double tmin = tmax + 1.0;
            int i;
            int j = -1;

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: trace(4,"selseph : time=%s sat=%2d\n",time_str(time,3),sat);
            GlobalMembersRtkcmn.trace(4, "selseph : time=%s sat=%2d\n", GlobalMembersRtkcmn.time_str(new gtime_t(time), 3), sat);

            for (i = 0; i < nav.ns; i++)
            {
                if (nav.seph[i].sat != sat)
                    continue;
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: if ((t=fabs(timediff(nav->seph[i].t0,time)))>tmax)
                if ((t = Math.Abs(GlobalMembersRtkcmn.timediff(nav.seph[i].t0, new gtime_t(time)))) > tmax)
                    continue;
                if (t <= tmin) // toe closest to time
                {
                    j = i;
                    tmin = t;
                }
            }
            if (j < 0)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: trace(3,"no sbas ephemeris     : %s sat=%2d\n",time_str(time,0),sat);
                GlobalMembersRtkcmn.trace(3, "no sbas ephemeris     : %s sat=%2d\n", GlobalMembersRtkcmn.time_str(new gtime_t(time), 0), sat);
                return null;
            }
            return nav.seph + j;
        }
        /* satellite clock with broadcast ephemeris ----------------------------------*/
        internal static int ephclk(gtime_t time, gtime_t teph, int sat, nav_t nav, ref double dts)
        {
            eph_t eph;
            geph_t geph;
            seph_t seph;
            int sys;

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: trace(4,"ephclk  : time=%s sat=%2d\n",time_str(time,3),sat);
            GlobalMembersRtkcmn.trace(4, "ephclk  : time=%s sat=%2d\n", GlobalMembersRtkcmn.time_str(new gtime_t(time), 3), sat);

            sys = GlobalMembersRtkcmn.satsys(sat, null);

            if (sys == DefineConstants.SYS_GPS || sys == DefineConstants.SYS_GAL || sys == DefineConstants.SYS_QZS || sys == DefineConstants.SYS_CMP)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: if (!(eph=seleph(teph,sat,-1,nav)))
                if ((eph = GlobalMembersEphemeris.seleph(new gtime_t(teph), sat, -1, nav)) == null)
                {
                    return 0;
                }
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: *dts=eph2clk(time,eph);
                dts = GlobalMembersEphemeris.eph2clk(new gtime_t(time), eph);
            }
            else if (sys == DefineConstants.SYS_GLO)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: if (!(geph=selgeph(teph,sat,-1,nav)))
                if ((geph = GlobalMembersEphemeris.selgeph(new gtime_t(teph), sat, -1, nav)) == null)
                {
                    return 0;
                }
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: *dts=geph2clk(time,geph);
                dts = GlobalMembersEphemeris.geph2clk(new gtime_t(time), geph);
            }
            else if (sys == DefineConstants.SYS_SBS)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: if (!(seph=selseph(teph,sat,nav)))
                if ((seph = GlobalMembersEphemeris.selseph(new gtime_t(teph), sat, nav)) == null)
                {
                    return 0;
                }
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: *dts=seph2clk(time,seph);
                dts = GlobalMembersEphemeris.seph2clk(new gtime_t(time), seph);
            }
            else
            {
                return 0;
            }

            return 1;
        }
        /* satellite position and clock by broadcast ephemeris -----------------------*/
        internal static int ephpos(gtime_t time, gtime_t teph, int sat, nav_t nav, int iode, double[] rs, double[] dts, ref double @var, ref int svh)
        {
            eph_t eph;
            geph_t geph;
            seph_t seph;
            double[] rst = new double[3];
            double[] dtst = new double[1];
            double tt = 1E-3;
            int i;
            int sys;

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: trace(4,"ephpos  : time=%s sat=%2d iode=%d\n",time_str(time,3),sat,iode);
            GlobalMembersRtkcmn.trace(4, "ephpos  : time=%s sat=%2d iode=%d\n", GlobalMembersRtkcmn.time_str(new gtime_t(time), 3), sat, iode);

            sys = GlobalMembersRtkcmn.satsys(sat, null);

            svh = -1;

            if (sys == DefineConstants.SYS_GPS || sys == DefineConstants.SYS_GAL || sys == DefineConstants.SYS_QZS || sys == DefineConstants.SYS_CMP)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: if (!(eph=seleph(teph,sat,iode,nav)))
                if ((eph = GlobalMembersEphemeris.seleph(new gtime_t(teph), sat, iode, nav)) == null)
                {
                    return 0;
                }

                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: eph2pos(time,eph,rs,dts,var);
                GlobalMembersEphemeris.eph2pos(new gtime_t(time), eph, rs, dts, ref @var);
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                //ORIGINAL LINE: time=timeadd(time,tt);
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                time.CopyFrom(GlobalMembersRtkcmn.timeadd(new gtime_t(time), tt));
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: eph2pos(time,eph,rst,dtst,var);
                GlobalMembersEphemeris.eph2pos(new gtime_t(time), eph, rst, dtst, ref @var);
                svh = eph.svh;
            }
            else if (sys == DefineConstants.SYS_GLO)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: if (!(geph=selgeph(teph,sat,iode,nav)))
                if ((geph = GlobalMembersEphemeris.selgeph(new gtime_t(teph), sat, iode, nav)) == null)
                {
                    return 0;
                }
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: geph2pos(time,geph,rs,dts,var);
                GlobalMembersEphemeris.geph2pos(new gtime_t(time), geph, rs, ref dts, ref @var);
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                //ORIGINAL LINE: time=timeadd(time,tt);
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                time.CopyFrom(GlobalMembersRtkcmn.timeadd(new gtime_t(time), tt));
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: geph2pos(time,geph,rst,dtst,var);
                GlobalMembersEphemeris.geph2pos(new gtime_t(time), geph, rst, ref dtst, ref @var);
                svh = geph.svh;
            }
            else if (sys == DefineConstants.SYS_SBS)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: if (!(seph=selseph(teph,sat,nav)))
                if ((seph = GlobalMembersEphemeris.selseph(new gtime_t(teph), sat, nav)) == null)
                {
                    return 0;
                }

                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: seph2pos(time,seph,rs,dts,var);
                GlobalMembersEphemeris.seph2pos(new gtime_t(time), seph, rs, ref dts, ref @var);
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                //ORIGINAL LINE: time=timeadd(time,tt);
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                time.CopyFrom(GlobalMembersRtkcmn.timeadd(new gtime_t(time), tt));
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: seph2pos(time,seph,rst,dtst,var);
                GlobalMembersEphemeris.seph2pos(new gtime_t(time), seph, rst, ref dtst, ref @var);
                svh = seph.svh;
            }
            else
            {
                return 0;
            }

            /* satellite velocity and clock drift by differential approx */
            for (i = 0; i < 3; i++)
            {
                rs[i + 3] = (rst[i] - rs[i]) / tt;
            }
            dts[1] = (dtst[0] - dts[0]) / tt;

            return 1;
        }
        /* satellite position and clock with sbas correction -------------------------*/
        internal static int satpos_sbas(gtime_t time, gtime_t teph, int sat, nav_t nav, ref double rs, ref double dts, ref double @var, ref int svh)
        {
            sbssatp_t sbs;
            int i;

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: trace(4,"satpos_sbas: time=%s sat=%2d\n",time_str(time,3),sat);
            GlobalMembersRtkcmn.trace(4, "satpos_sbas: time=%s sat=%2d\n", GlobalMembersRtkcmn.time_str(new gtime_t(time), 3), sat);

            /* search sbas satellite correciton */
            for (i = 0; i < nav.sbssat.nsat; i++)
            {
                sbs = nav.sbssat.sat + i;
                if (sbs.sat == sat)
                    break;
            }
            if (i >= nav.sbssat.nsat)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: trace(2,"no sbas correction for orbit: %s sat=%2d\n",time_str(time,0),sat);
                GlobalMembersRtkcmn.trace(2, "no sbas correction for orbit: %s sat=%2d\n", GlobalMembersRtkcmn.time_str(new gtime_t(time), 0), sat);
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: ephpos(time,teph,sat,nav,-1,rs,dts,var,svh);
                GlobalMembersEphemeris.ephpos(new gtime_t(time), new gtime_t(teph), sat, nav, -1, rs, dts, ref @var, ref svh);
                svh = -1;
                return 0;
            }
            /* satellite postion and clock by broadcast ephemeris */
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: if (!ephpos(time,teph,sat,nav,sbs->lcorr.iode,rs,dts,var,svh))
            if (GlobalMembersEphemeris.ephpos(new gtime_t(time), new gtime_t(teph), sat, nav, sbs.lcorr.iode, rs, dts, ref @var, ref svh) == 0)
            {
                return 0;
            }

            /* sbas satellite correction (long term and fast) */
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: if (sbssatcorr(time,sat,nav,rs,dts,var))
            if (GlobalMembersSbas.sbssatcorr(new gtime_t(time), sat, nav, rs, dts, ref @var) != 0)
            {
                return 1;
            }
            svh = -1;
            return 0;
        }
        /* satellite position and clock with ssr correction --------------------------*/
        internal static int satpos_ssr(gtime_t time, gtime_t teph, int sat, nav_t nav, int opt, double[] rs, double[] dts, ref double @var, ref int svh)
        {
            ssr_t ssr;
            eph_t eph;
            double t1;
            double t2;
            double t3;
            double[] er = new double[3];
            double[] ea = new double[3];
            double[] ec = new double[3];
            double[] rc = new double[3];
            double[] deph = new double[3];
            double dclk;
            double[] dant = { 0, null, null };
            double tk;
            int i;
            int sys;

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: trace(4,"satpos_ssr: time=%s sat=%2d\n",time_str(time,3),sat);
            GlobalMembersRtkcmn.trace(4, "satpos_ssr: time=%s sat=%2d\n", GlobalMembersRtkcmn.time_str(new gtime_t(time), 3), sat);

            ssr = nav.ssr + sat - 1;

            if (ssr.t0[0].time == null)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: trace(2,"no ssr orbit correction: %s sat=%2d\n",time_str(time,0),sat);
                GlobalMembersRtkcmn.trace(2, "no ssr orbit correction: %s sat=%2d\n", GlobalMembersRtkcmn.time_str(new gtime_t(time), 0), sat);
                return 0;
            }
            if (ssr.t0[1].time == null)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: trace(2,"no ssr clock correction: %s sat=%2d\n",time_str(time,0),sat);
                GlobalMembersRtkcmn.trace(2, "no ssr clock correction: %s sat=%2d\n", GlobalMembersRtkcmn.time_str(new gtime_t(time), 0), sat);
                return 0;
            }
            /* inconsistency between orbit and clock correction */
            if (ssr.iod[0] != ssr.iod[1])
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: trace(2,"inconsist ssr correction: %s sat=%2d iod=%d %d\n", time_str(time,0),sat,ssr->iod[0],ssr->iod[1]);
                GlobalMembersRtkcmn.trace(2, "inconsist ssr correction: %s sat=%2d iod=%d %d\n", GlobalMembersRtkcmn.time_str(new gtime_t(time), 0), sat, ssr.iod[0], ssr.iod[1]);
                svh = -1;
                return 0;
            }
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: t1=timediff(time,ssr->t0[0]);
            t1 = GlobalMembersRtkcmn.timediff(new gtime_t(time), new gtime_t(ssr.t0[0]));
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: t2=timediff(time,ssr->t0[1]);
            t2 = GlobalMembersRtkcmn.timediff(new gtime_t(time), new gtime_t(ssr.t0[1]));
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: t3=timediff(time,ssr->t0[2]);
            t3 = GlobalMembersRtkcmn.timediff(new gtime_t(time), new gtime_t(ssr.t0[2]));

            /* ssr orbit and clock correction (ref [4]) */
            if (Math.Abs(t1) > DefineConstants.MAXAGESSR || Math.Abs(t2) > DefineConstants.MAXAGESSR)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: trace(2,"age of ssr error: %s sat=%2d t=%.0f %.0f\n",time_str(time,0), sat,t1,t2);
                GlobalMembersRtkcmn.trace(2, "age of ssr error: %s sat=%2d t=%.0f %.0f\n", GlobalMembersRtkcmn.time_str(new gtime_t(time), 0), sat, t1, t2);
                svh = -1;
                return 0;
            }
            if (ssr.udi[0] >= 1.0)
            {
                t1 -= ssr.udi[0] / 2.0;
            }
            if (ssr.udi[1] >= 1.0)
            {
                t2 -= ssr.udi[0] / 2.0;
            }

            for (i = 0; i < 3; i++)
            {
                deph[i] = ssr.deph[i] + ssr.ddeph[i] * t1;
            }
            dclk = ssr.dclk[0] + ssr.dclk[1] * t2 + ssr.dclk[2] * t2 * t2;

            /* ssr highrate clock correction (ref [4]) */
            if (ssr.iod[0] == ssr.iod[2] && ssr.t0[2].time != null && Math.Abs(t3) < DefineConstants.MAXAGESSR_HRCLK)
            {
                dclk += ssr.hrclk;
            }
            if (GlobalMembersRtkcmn.norm(deph, 3) > DefineConstants.MAXECORSSR || Math.Abs(dclk) > 1E-6 * DefineConstants.CLIGHT)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: trace(3,"invalid ssr correction: %s deph=%.1f dclk=%.1f\n", time_str(time,0),norm(deph,3),dclk);
                GlobalMembersRtkcmn.trace(3, "invalid ssr correction: %s deph=%.1f dclk=%.1f\n", GlobalMembersRtkcmn.time_str(new gtime_t(time), 0), GlobalMembersRtkcmn.norm(deph, 3), dclk);
                svh = -1;
                return 0;
            }
            /* satellite postion and clock by broadcast ephemeris */
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: if (!ephpos(time,teph,sat,nav,ssr->iode,rs,dts,var,svh))
            if (GlobalMembersEphemeris.ephpos(new gtime_t(time), new gtime_t(teph), sat, nav, ssr.iode, rs, dts, ref @var, ref svh) == 0)
            {
                return 0;
            }

            /* satellite clock for gps, galileo and qzss */
            sys = GlobalMembersRtkcmn.satsys(sat, null);
            if (sys == DefineConstants.SYS_GPS || sys == DefineConstants.SYS_GAL || sys == DefineConstants.SYS_QZS || sys == DefineConstants.SYS_CMP)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: if (!(eph=seleph(teph,sat,ssr->iode,nav)))
                if ((eph = GlobalMembersEphemeris.seleph(new gtime_t(teph), sat, ssr.iode, nav)) == null)
                {
                    return 0;
                }

                /* satellite clock by clock parameters */
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: tk=timediff(time,eph->toc);
                tk = GlobalMembersRtkcmn.timediff(new gtime_t(time), new gtime_t(eph.toc));
                dts[0] = eph.f0 + eph.f1 * tk + eph.f2 * tk * tk;
                dts[1] = eph.f1 + 2.0 * eph.f2 * tk;

                /* relativity correction */
                dts[0] -= 2.0 * GlobalMembersRtkcmn.dot(rs, rs + 3, 3) / DefineConstants.CLIGHT / DefineConstants.CLIGHT;
            }
            /* radial-along-cross directions in ecef */
            if (GlobalMembersRtkcmn.normv3(rs + 3, ea) == 0)
            {
                return 0;
            }
            GlobalMembersRtkcmn.cross3(rs, rs + 3, rc);
            if (GlobalMembersRtkcmn.normv3(rc, ec) == 0)
            {
                svh = -1;
                return 0;
            }
            GlobalMembersRtkcmn.cross3(ea, ec, er);

            /* satellite antenna offset correction */
            if (opt != 0)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: satantoff(time,rs,sat,nav,dant);
                GlobalMembersPreceph.satantoff(new gtime_t(time), rs, sat, nav, dant);
            }
            for (i = 0; i < 3; i++)
            {
                rs[i] += -(er[i] * deph[0] + ea[i] * deph[1] + ec[i] * deph[2]) + dant[i];
            }
            /* t_corr = t_sv - (dts(brdc) + dclk(ssr) / CLIGHT) (ref [10] eq.3.12-7) */
            dts[0] += dclk / DefineConstants.CLIGHT;

            /* variance by ssr ura */
            @var = GlobalMembersEphemeris.var_urassr(ssr.ura);

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: trace(5,"satpos_ssr: %s sat=%2d deph=%6.3f %6.3f %6.3f er=%6.3f %6.3f %6.3f dclk=%6.3f var=%6.3f\n", time_str(time,2),sat,deph[0],deph[1],deph[2],er[0],er[1],er[2],dclk,*var);
            GlobalMembersRtkcmn.trace(5, "satpos_ssr: %s sat=%2d deph=%6.3f %6.3f %6.3f er=%6.3f %6.3f %6.3f dclk=%6.3f var=%6.3f\n", GlobalMembersRtkcmn.time_str(new gtime_t(time), 2), sat, deph[0], deph[1], deph[2], er[0], er[1], er[2], dclk, @var);

            return 1;
        }
        /* satellite position and clock ------------------------------------------------
        * compute satellite position, velocity and clock
        * args   : gtime_t time     I   time (gpst)
        *          gtime_t teph     I   time to select ephemeris (gpst)
        *          int    sat       I   satellite number
        *          nav_t  *nav      I   navigation data
        *          int    ephopt    I   ephemeris option (EPHOPT_???)
        *          double *rs       O   sat position and velocity (ecef)
        *                               {x,y,z,vx,vy,vz} (m|m/s)
        *          double *dts      O   sat clock {bias,drift} (s|s/s)
        *          double *var      O   sat position and clock error variance (m^2)
        *          int    *svh      O   sat health flag (-1:correction not available)
        * return : status (1:ok,0:error)
        * notes  : satellite position is referenced to antenna phase center
        *          satellite clock does not include code bias correction (tgd or bgd)
        *-----------------------------------------------------------------------------*/
        public static int satpos(gtime_t time, gtime_t teph, int sat, int ephopt, nav_t nav, ref double rs, ref double dts, ref double @var, ref int svh)
        {
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: trace(4,"satpos  : time=%s sat=%2d ephopt=%d\n",time_str(time,3),sat,ephopt);
            GlobalMembersRtkcmn.trace(4, "satpos  : time=%s sat=%2d ephopt=%d\n", GlobalMembersRtkcmn.time_str(new gtime_t(time), 3), sat, ephopt);

            svh = 0;

            switch (ephopt)
            {
                case DefineConstants.EPHOPT_BRDC:
                    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                    //ORIGINAL LINE: return ephpos(time,teph,sat,nav,-1,rs,dts,var,svh);
                    return GlobalMembersEphemeris.ephpos(new gtime_t(time), new gtime_t(teph), sat, nav, -1, rs, dts, ref @var, ref svh);
                case DefineConstants.EPHOPT_SBAS:
                    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                    //ORIGINAL LINE: return satpos_sbas(time,teph,sat,nav, rs,dts,var,svh);
                    return GlobalMembersEphemeris.satpos_sbas(new gtime_t(time), new gtime_t(teph), sat, nav, ref rs, ref dts, ref @var, ref svh);
                case DefineConstants.EPHOPT_SSRAPC:
                    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                    //ORIGINAL LINE: return satpos_ssr(time,teph,sat,nav, 0,rs,dts,var,svh);
                    return GlobalMembersEphemeris.satpos_ssr(new gtime_t(time), new gtime_t(teph), sat, nav, 0, rs, dts, ref @var, ref svh);
                case DefineConstants.EPHOPT_SSRCOM:
                    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                    //ORIGINAL LINE: return satpos_ssr(time,teph,sat,nav, 1,rs,dts,var,svh);
                    return GlobalMembersEphemeris.satpos_ssr(new gtime_t(time), new gtime_t(teph), sat, nav, 1, rs, dts, ref @var, ref svh);
                case DefineConstants.EPHOPT_PREC:
                    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                    //ORIGINAL LINE: if (!peph2pos(time,sat,nav,1,rs,dts,var))
                    if (GlobalMembersPreceph.peph2pos(new gtime_t(time), sat, nav, 1, rs, dts, ref @var) == 0)
                        break;
                    else
                    {
                        return 1;
                    }
                case DefineConstants.EPHOPT_LEX:
                    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                    //ORIGINAL LINE: if (!lexeph2pos(time,sat,nav,rs,dts,var))
                    if (GlobalMembersQzslex.lexeph2pos(new gtime_t(time), sat, nav, rs, dts, ref @var) == 0)
                        break;
                    else
                    {
                        return 1;
                    }
            }
            svh = -1;
            return 0;
        }
        /* satellite positions and clocks ----------------------------------------------
        * compute satellite positions, velocities and clocks
        * args   : gtime_t teph     I   time to select ephemeris (gpst)
        *          obsd_t *obs      I   observation data
        *          int    n         I   number of observation data
        *          nav_t  *nav      I   navigation data
        *          int    ephopt    I   ephemeris option (EPHOPT_???)
        *          double *rs       O   satellite positions and velocities (ecef)
        *          double *dts      O   satellite clocks
        *          double *var      O   sat position and clock error variances (m^2)
        *          int    *svh      O   sat health flag (-1:correction not available)
        * return : none
        * notes  : rs [(0:2)+i*6]= obs[i] sat position {x,y,z} (m)
        *          rs [(3:5)+i*6]= obs[i] sat velocity {vx,vy,vz} (m/s)
        *          dts[(0:1)+i*2]= obs[i] sat clock {bias,drift} (s|s/s)
        *          var[i]        = obs[i] sat position and clock error variance (m^2)
        *          svh[i]        = obs[i] sat health flag
        *          if no navigation data, set 0 to rs[], dts[], var[] and svh[]
        *          satellite position and clock are values at signal transmission time
        *          satellite position is referenced to antenna phase center
        *          satellite clock does not include code bias correction (tgd or bgd)
        *          any pseudorange and broadcast ephemeris are always needed to get
        *          signal transmission time
        *-----------------------------------------------------------------------------*/
        public static void satposs(gtime_t teph, obsd_t[] obs, int n, nav_t nav, int ephopt, double[] rs, double[] dts, double[] @var, int[] svh)
        {
            gtime_t[] time = { { 0 } };
            double dt;
            double pr;
            int i;
            int j;

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: trace(3,"satposs : teph=%s n=%d ephopt=%d\n",time_str(teph,3),n,ephopt);
            GlobalMembersRtkcmn.trace(3, "satposs : teph=%s n=%d ephopt=%d\n", GlobalMembersRtkcmn.time_str(new gtime_t(teph), 3), n, ephopt);

            for (i = 0; i < n && i < DefineConstants.MAXOBS; i++)
            {
                for (j = 0; j < 6; j++)
                {
                    rs[j + i * 6] = 0.0;
                }
                for (j = 0; j < 2; j++)
                {
                    dts[j + i * 2] = 0.0;
                }
                @var[i] = 0.0;
                svh[i] = 0;

                /* search any psuedorange */
                for (j = 0, pr = 0.0; j < DefineConstants.NFREQ; j++)
                {
                    if ((pr = obs[i].P[j]) != 0.0)
                        break;
                }

                if (j >= DefineConstants.NFREQ)
                {
                    GlobalMembersRtkcmn.trace(2, "no pseudorange %s sat=%2d\n", GlobalMembersRtkcmn.time_str(obs[i].time, 3), obs[i].sat);
                    continue;
                }
                /* transmission time by satellite clock */
                time[i] = GlobalMembersRtkcmn.timeadd(obs[i].time, -pr / DefineConstants.CLIGHT);

                /* satellite clock bias by broadcast ephemeris */
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: if (!ephclk(time[i],teph,obs[i].sat,nav,&dt))
                if (GlobalMembersEphemeris.ephclk(new gtime_t(time[i]), new gtime_t(teph), obs[i].sat, nav, ref dt) == 0)
                {
                    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                    //ORIGINAL LINE: trace(2,"no broadcast clock %s sat=%2d\n",time_str(time[i],3),obs[i].sat);
                    GlobalMembersRtkcmn.trace(2, "no broadcast clock %s sat=%2d\n", GlobalMembersRtkcmn.time_str(new gtime_t(time[i]), 3), obs[i].sat);
                    continue;
                }
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: time[i]=timeadd(time[i],-dt);
                time[i] = GlobalMembersRtkcmn.timeadd(new gtime_t(time[i]), -dt);

                /* satellite position and clock at transmission time */
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: if (!satpos(time[i],teph,obs[i].sat,ephopt,nav,rs+i *6,dts+i *2,var+i, svh+i))
                if (GlobalMembersEphemeris.satpos(new gtime_t(time[i]), new gtime_t(teph), obs[i].sat, ephopt, nav, ref rs + i * 6, ref dts + i * 2, ref @var + i, ref svh + i) == 0)
                {
                    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                    //ORIGINAL LINE: trace(2,"no ephemeris %s sat=%2d\n",time_str(time[i],3),obs[i].sat);
                    GlobalMembersRtkcmn.trace(2, "no ephemeris %s sat=%2d\n", GlobalMembersRtkcmn.time_str(new gtime_t(time[i]), 3), obs[i].sat);
                    continue;
                }
                /* if no precise clock available, use broadcast clock instead */
                if (dts[i * 2] == 0.0)
                {
                    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                    //ORIGINAL LINE: if (!ephclk(time[i],teph,obs[i].sat,nav,dts+i *2))
                    if (GlobalMembersEphemeris.ephclk(new gtime_t(time[i]), new gtime_t(teph), obs[i].sat, nav, ref dts + i * 2) == 0)
                        continue;
                    dts[1 + i * 2] = 0.0;
                    *@var = ((DefineConstants.STD_BRDCCLK) * (DefineConstants.STD_BRDCCLK));
                }
            }
            for (i = 0; i < n && i < DefineConstants.MAXOBS; i++)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: trace(4,"%s sat=%2d rs=%13.3f %13.3f %13.3f dts=%12.3f var=%7.3f svh=%02X\n", time_str(time[i],6),obs[i].sat,rs[i *6],rs[1+i *6],rs[2+i *6], dts[i *2]*1E9,var[i],svh[i]);
                GlobalMembersRtkcmn.trace(4, "%s sat=%2d rs=%13.3f %13.3f %13.3f dts=%12.3f var=%7.3f svh=%02X\n", GlobalMembersRtkcmn.time_str(new gtime_t(time[i]), 6), obs[i].sat, rs[i * 6], rs[1 + i * 6], rs[2 + i * 6], dts[i * 2] * 1E9, @var[i], svh[i]);
            }
        }
    }
}
