using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ghGPS.Classes
{
    public static class GlobalMembersRtkpos
    {
        /*------------------------------------------------------------------------------
        * rtkpos.c : precise positioning
        *
        *          Copyright (C) 2007-2013 by T.TAKASU, All rights reserved.
        *
        * version : $Revision: 1.1 $ $Date: 2008/07/17 21:48:06 $
        * history : 2007/01/12 1.0  new
        *           2007/03/13 1.1  add slip detection by LLI flag
        *           2007/04/18 1.2  add antenna pcv correction
        *                           change rtkpos argin
        *           2008/07/18 1.3  refactored
        *           2009/01/02 1.4  modify rtk positioning api
        *           2009/03/09 1.5  support glonass, gallileo and qzs
        *           2009/08/27 1.6  fix bug on numerical exception
        *           2009/09/03 1.7  add check of valid satellite number
        *                           add check time sync for moving-base
        *           2009/11/23 1.8  add api rtkopenstat(),rtkclosestat()
        *                           add receiver h/w bias estimation
        *                           add solution status output
        *           2010/04/04 1.9  support ppp-kinematic and ppp-static modes
        *                           support earth tide correction
        *                           changed api:
        *                               rtkpos()
        *           2010/09/07 1.10 add elevation mask to hold ambiguity
        *           2012/02/01 1.11 add extended receiver error model
        *                           add glonass interchannel bias correction
        *                           add slip detectior by L1-L5 gf jump
        *                           output snr of rover receiver in residuals
        *           2013/03/10 1.12 add otl and pole tides corrections
        *           2014/05/26 1.13 support beidou and galileo
        *                           add output of gal-gps and bds-gps time offset
        *           2014/05/28 1.14 fix bug on memory exception with many sys and freq
        *           2014/08/26 1.15 add functino to swap sol-stat file with keywords
        *           2014/10/21 1.16 fix bug on beidou amb-res with pos2-bdsarmode=0
        *           2014/11/08 1.17 fix bug on ar-degradation by unhealthy satellites
        *           2015/03/23 1.18 residuals referenced to reference satellite
        *-----------------------------------------------------------------------------*/

#if EXTGSI

	public static int resamb_WLNL(rtk_t rtk, obsd_t obs, int sat, int iu, int ir, int ns, nav_t nav, double azel)
	{
		return 0;
	}
	public static int resamb_TCAR(rtk_t rtk, obsd_t obs, int sat, int iu, int ir, int ns, nav_t nav, double azel)
	{
		return 0;
	}
#else
#endif

        /* global variables ----------------------------------------------------------*/
        internal static int statlevel = 0; // rtk status output level (0:off)
        internal static FILE fp_stat = null; // rtk status file pointer
        internal static string file_stat = ""; // rtk status file original path
        internal static gtime_t time_stat = new gtime_t(); // rtk status file time

        /* open solution status file ---------------------------------------------------
        * open solution status file and set output level
        * args   : char     *file   I   rtk status file
        *          int      level   I   rtk status level (0: off)
        * return : status (1:ok,0:error)
        * notes  : file can constain time keywords (%Y,%y,%m...) defined in reppath().
        *          The time to replace keywords is based on UTC of CPU time.
        * output : solution status file record format
        *
        *   $POS,week,tow,stat,posx,posy,posz,posxf,posyf,poszf
        *          week/tow : gps week no/time of week (s)
        *          stat     : solution status
        *          posx/posy/posz    : position x/y/z ecef (m) float
        *          posxf/posyf/poszf : position x/y/z ecef (m) fixed
        *
        *   $VELACC,week,tow,stat,vele,veln,velu,acce,accn,accu,velef,velnf,veluf,accef,accnf,accuf
        *          week/tow : gps week no/time of week (s)
        *          stat     : solution status
        *          vele/veln/velu    : velocity e/n/u (m/s) float
        *          acce/accn/accu    : acceleration e/n/u (m/s^2) float
        *          velef/velnf/veluf : velocity e/n/u (m/s) fixed
        *          accef/accnf/accuf : acceleration e/n/u (m/s^2) fixed
        *
        *   $CLK,week,tow,stat,clk1,clk2,clk3,clk4
        *          week/tow : gps week no/time of week (s)
        *          stat     : solution status
        *          clk1     : receiver clock bias GPS (ns)
        *          clk2     : receiver clock bias GLO-GPS (ns)
        *          clk3     : receiver clock bias GAL-GPS (ns)
        *          clk4     : receiver clock bias BDS-GPS (ns)
        *
        *   $ION,week,tow,stat,sat,az,el,ion,ion-fixed
        *          week/tow : gps week no/time of week (s)
        *          stat     : solution status
        *          sat      : satellite id
        *          az/el    : azimuth/elevation angle(deg)
        *          ion      : vertical ionospheric delay L1 (m) float
        *          ion-fixed: vertical ionospheric delay L1 (m) fixed
        *
        *   $TROP,week,tow,stat,rcv,ztd,ztdf
        *          week/tow : gps week no/time of week (s)
        *          stat     : solution status
        *          rcv      : receiver (1:rover,2:base station)
        *          ztd      : zenith total delay (m) float
        *          ztdf     : zenith total delay (m) fixed
        *
        *   $HWBIAS,week,tow,stat,frq,bias,biasf
        *          week/tow : gps week no/time of week (s)
        *          stat     : solution status
        *          frq      : frequency (1:L1,2:L2,...)
        *          bias     : h/w bias coefficient (m/MHz) float
        *          biasf    : h/w bias coefficient (m/MHz) fixed
        *
        *   $SAT,week,tow,sat,frq,az,el,resp,resc,vsat,snr,fix,slip,lock,outc,slipc,rejc
        *          week/tow : gps week no/time of week (s)
        *          sat/frq  : satellite id/frequency (1:L1,2:L2,...)
        *          az/el    : azimuth/elevation angle (deg)
        *          resp     : pseudorange residual (m)
        *          resc     : carrier-phase residual (m)
        *          vsat     : valid data flag (0:invalid,1:valid)
        *          snr      : signal strength (dbHz)
        *          fix      : ambiguity flag  (0:no data,1:float,2:fixed,3:hold,4:ppp)
        *          slip     : cycle-slip flag (bit1:slip,bit2:parity unknown)
        *          lock     : carrier-lock count
        *          outc     : data outage count
        *          slipc    : cycle-slip count
        *          rejc     : data reject (outlier) count
        *
        *-----------------------------------------------------------------------------*/
        public static int rtkopenstat(string file, int level)
        {
            gtime_t time = GlobalMembersRtkcmn.utc2gpst(GlobalMembersRtkcmn.timeget());
            string path = new string(new char[1024]);

            GlobalMembersRtkcmn.trace(3, "rtkopenstat: file=%s level=%d\n", file, level);

            if (level <= 0)
            {
                return 0;
            }

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: reppath(file,path,time,"","");
            GlobalMembersRtkcmn.reppath(file, ref path, new gtime_t(time), "", "");

            if ((fp_stat = fopen(path, "w")) == null)
            {
                GlobalMembersRtkcmn.trace(1, "rtkopenstat: file open error path=%s\n", path);
                return 0;
            }
            file_stat = file;
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: time_stat=time;
            time_stat.CopyFrom(time);
            statlevel = level;
            return 1;
        }
        /* close solution status file --------------------------------------------------
        * close solution status file
        * args   : none
        * return : none
        *-----------------------------------------------------------------------------*/
        public static void rtkclosestat()
        {
            GlobalMembersRtkcmn.trace(3, "rtkclosestat:\n");

            if (fp_stat != null)
            {
                fclose(fp_stat);
            }
            fp_stat = null;
            file_stat[0] = '\0';
            statlevel = 0;
        }
        /* swap solution status file -------------------------------------------------*/
        internal static void swapsolstat()
        {
            gtime_t time = GlobalMembersRtkcmn.utc2gpst(GlobalMembersRtkcmn.timeget());
            string path = new string(new char[1024]);

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: if ((int)(time2gpst(time,null)/DefineConstants.INT_SWAP_STAT)== (int)(time2gpst(time_stat,null)/DefineConstants.INT_SWAP_STAT))
            if ((int)(GlobalMembersRtkcmn.time2gpst(new gtime_t(time), null) / DefineConstants.INT_SWAP_STAT) == (int)(GlobalMembersRtkcmn.time2gpst(new gtime_t(time_stat), null) / DefineConstants.INT_SWAP_STAT))
            {
                return;
            }
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: time_stat=time;
            time_stat.CopyFrom(time);

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: if (!reppath(file_stat,path,time,"",""))
            if (GlobalMembersRtkcmn.reppath(file_stat, ref path, new gtime_t(time), "", "") == 0)
            {
                return;
            }
            if (fp_stat != null)
            {
                fclose(fp_stat);
            }

            if ((fp_stat = fopen(path, "w")) == null)
            {
                GlobalMembersRtkcmn.trace(2, "swapsolstat: file open error path=%s\n", path);
                return;
            }
            GlobalMembersRtkcmn.trace(3, "swapsolstat: path=%s\n", path);
        }
        /* output solution status ----------------------------------------------------*/
        internal static void outsolstat(rtk_t rtk)
        {
            ssat_t ssat;
            double tow;
            double[] pos = new double[3];
            double[] vel = new double[3];
            double[] acc = new double[3];
            double[] vela = { 0, null, null };
            double[] acca = { 0, null, null };
            double[] xa = new double[3];
            int i;
            int j;
            int week;
            int est;
            int nfreq;
            int nf = ((rtk.opt).ionoopt == DefineConstants.IONOOPT_IFLC ? 1 : (rtk.opt).nf);
            string id = new string(new char[32]);

            if (statlevel <= 0 || fp_stat == null)
                return;

            GlobalMembersRtkcmn.trace(3, "outsolstat:\n");

            /* swap solution status file */
            GlobalMembersRtkpos.swapsolstat();

            est = rtk.opt.mode >= DefineConstants.PMODE_DGPS;
            nfreq = est != 0 ? nf : 1;
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: tow=time2gpst(rtk->sol.time,&week);
            tow = GlobalMembersRtkcmn.time2gpst(new gtime_t(rtk.sol.time), ref week);

            /* receiver position */
            if (est != 0)
            {
                for (i = 0; i < 3; i++)
                {
                    xa[i] = i < rtk.na != 0 ? rtk.xa[i] : 0.0;
                }
                fprintf(fp_stat, "$POS,%d,%.3f,%d,%.4f,%.4f,%.4f,%.4f,%.4f,%.4f\n", week, tow, rtk.sol.stat, rtk.x[0], rtk.x[1], rtk.x[2], xa[0], xa[1], xa[2]);
            }
            else
            {
                fprintf(fp_stat, "$POS,%d,%.3f,%d,%.4f,%.4f,%.4f,%.4f,%.4f,%.4f\n", week, tow, rtk.sol.stat, rtk.sol.rr[0], rtk.sol.rr[1], rtk.sol.rr[2], 0.0, 0.0, 0.0);
            }
            /* receiver velocity and acceleration */
            if (est != 0 && rtk.opt.dynamics != 0)
            {
                GlobalMembersRtkcmn.ecef2pos(rtk.sol.rr, pos);
                GlobalMembersRtkcmn.ecef2enu(pos, rtk.x + 3, ref vel);
                GlobalMembersRtkcmn.ecef2enu(pos, rtk.x + 6, ref acc);
                if (rtk.na >= 6)
                {
                    GlobalMembersRtkcmn.ecef2enu(pos, rtk.xa + 3, ref vela);
                }
                if (rtk.na >= 9)
                {
                    GlobalMembersRtkcmn.ecef2enu(pos, rtk.xa + 6, ref acca);
                }
                fprintf(fp_stat, "$VELACC,%d,%.3f,%d,%.4f,%.4f,%.4f,%.5f,%.5f,%.5f,%.4f,%.4f,%.4f,%.5f,%.5f,%.5f\n", week, tow, rtk.sol.stat, vel[0], vel[1], vel[2], acc[0], acc[1], acc[2], vela[0], vela[1], vela[2], acca[0], acca[1], acca[2]);
            }
            else
            {
                GlobalMembersRtkcmn.ecef2pos(rtk.sol.rr, pos);
                GlobalMembersRtkcmn.ecef2enu(pos, rtk.sol.rr + 3, ref vel);
                fprintf(fp_stat, "$VELACC,%d,%.3f,%d,%.4f,%.4f,%.4f,%.5f,%.5f,%.5f,%.4f,%.4f,%.4f,%.5f,%.5f,%.5f\n", week, tow, rtk.sol.stat, vel[0], vel[1], vel[2], 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0);
            }
            /* receiver clocks */
            fprintf(fp_stat, "$CLK,%d,%.3f,%d,%d,%.3f,%.3f,%.3f,%.3f\n", week, tow, rtk.sol.stat, 1, rtk.sol.dtr[0] * 1E9, rtk.sol.dtr[1] * 1E9, rtk.sol.dtr[2] * 1E9, rtk.sol.dtr[3] * 1E9);

            /* ionospheric parameters */
            if (est != 0 && rtk.opt.ionoopt == DefineConstants.IONOOPT_EST)
            {
                for (i = 0; i < DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1; i++)
                {
                    ssat = rtk.ssat + i;
                    if (ssat.vs == 0)
                        continue;
                    GlobalMembersRtkcmn.satno2id(i + 1, ref id);
                    j = (((rtk.opt).dynamics == 0 ? 3 : 9) + (i + 1) - 1);
                    xa[0] = j < rtk.na != 0 ? rtk.xa[j] : 0.0;
                    fprintf(fp_stat, "$ION,%d,%.3f,%d,%s,%.1f,%.1f,%.4f,%.4f\n", week, tow, rtk.sol.stat, id, ssat.azel[0] * 180.0 / DefineConstants.PI, ssat.azel[1] * 180.0 / DefineConstants.PI, rtk.x[j], xa[0]);
                }
            }
            /* tropospheric parameters */
            if (est != 0 && (rtk.opt.tropopt == DefineConstants.TROPOPT_EST || rtk.opt.tropopt == DefineConstants.TROPOPT_ESTG))
            {
                for (i = 0; i < 2; i++)
                {
                    j = (((rtk.opt).dynamics == 0 ? 3 : 9) + ((rtk.opt).ionoopt != DefineConstants.IONOOPT_EST ? 0 : DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1) + ((rtk.opt).tropopt < DefineConstants.TROPOPT_EST ? 0 : ((rtk.opt).tropopt < DefineConstants.TROPOPT_ESTG ? 2 : 6)) / 2 * (i));
                    xa[0] = j < rtk.na != 0 ? rtk.xa[j] : 0.0;
                    fprintf(fp_stat, "$TROP,%d,%.3f,%d,%d,%.4f,%.4f\n", week, tow, rtk.sol.stat, i + 1, rtk.x[j], xa[0]);
                }
            }
            /* receiver h/w bias */
            if (est != 0 && rtk.opt.glomodear == 2)
            {
                for (i = 0; i < nfreq; i++)
                {
                    j = (((rtk.opt).dynamics == 0 ? 3 : 9) + ((rtk.opt).ionoopt != DefineConstants.IONOOPT_EST ? 0 : DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1) + ((rtk.opt).tropopt < DefineConstants.TROPOPT_EST ? 0 : ((rtk.opt).tropopt < DefineConstants.TROPOPT_ESTG ? 2 : 6)) + (i));
                    xa[0] = j < rtk.na != 0 ? rtk.xa[j] : 0.0;
                    fprintf(fp_stat, "$HWBIAS,%d,%.3f,%d,%d,%.4f,%.4f\n", week, tow, rtk.sol.stat, i + 1, rtk.x[j], xa[0]);
                }
            }
            if (rtk.sol.stat == DefineConstants.SOLQ_NONE || statlevel <= 1)
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
                    fprintf(fp_stat, "$SAT,%d,%.3f,%s,%d,%.1f,%.1f,%.4f,%.4f,%d,%.0f,%d,%d,%d,%d,%d,%d\n", week, tow, id, j + 1, ssat.azel[0] * 180.0 / DefineConstants.PI, ssat.azel[1] * 180.0 / DefineConstants.PI, ssat.resp[j], ssat.resc[j], ssat.vsat[j], ssat.snr[j] * 0.25, ssat.fix[j], ssat.slip[j] & 3, ssat.@lock[j], ssat.outc[j], ssat.slipc[j], ssat.rejc[j]);
                }
            }
        }
        /* save error message --------------------------------------------------------*/
        internal static void errmsg(rtk_t rtk, string format, params object[] LegacyParamArray)
        {
            string buff = new string(new char[256]);
            string tstr = new string(new char[32]);
            int n;
            //	va_list ap;
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: time2str(rtk->sol.time,tstr,2);
            GlobalMembersRtkcmn.time2str(new gtime_t(rtk.sol.time), ref tstr, 2);
            buff = string.Format("{0}: ", tstr.Substring(11));
            n = buff.Length;
            int ParamCount = -1;
            //	va_start(ap,format);
            n += vsprintf(buff.Substring(n), format, ap);
            //	va_end(ap);
            n = n < DefineConstants.MAXERRMSG - rtk.neb != 0 ? n : DefineConstants.MAXERRMSG - rtk.neb;
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'memcpy' has no equivalent in C#:
            memcpy(rtk.errbuf.Substring(rtk.neb), buff, n);
            rtk.neb += n;
            GlobalMembersRtkcmn.trace(2, "%s", buff);
        }
        /* single-differenced observable ---------------------------------------------*/
        internal static double sdobs(obsd_t[] obs, int i, int j, int f)
        {
            double pi = f < DefineConstants.NFREQ ? obs[i].L[f] : obs[i].P[f - DefineConstants.NFREQ];
            double pj = f < DefineConstants.NFREQ ? obs[j].L[f] : obs[j].P[f - DefineConstants.NFREQ];
            return pi == 0.0 || pj == 0.0 ? 0.0 : pi - pj;
        }
        /* single-differenced geometry-free linear combination of phase --------------*/
        internal static double gfobs_L1L2(obsd_t obs, int i, int j, double[] lam)
        {
            double pi = GlobalMembersRtkpos.sdobs(obs, i, j, 0) * lam[0];
            double pj = GlobalMembersRtkpos.sdobs(obs, i, j, 1) * lam[1];
            return pi == 0.0 || pj == 0.0 ? 0.0 : pi - pj;
        }
        internal static double gfobs_L1L5(obsd_t obs, int i, int j, double[] lam)
        {
            double pi = GlobalMembersRtkpos.sdobs(obs, i, j, 0) * lam[0];
            double pj = GlobalMembersRtkpos.sdobs(obs, i, j, 2) * lam[2];
            return pi == 0.0 || pj == 0.0 ? 0.0 : pi - pj;
        }
        /* single-differenced measurement error variance -----------------------------*/
        internal static double varerr(int sat, int sys, double el, double bl, double dt, int f, prcopt_t opt)
        {
            double a;
            double b;
            double c = opt.err[3] * bl / 1E4;
            double d = DefineConstants.CLIGHT * opt.sclkstab * dt;
            double fact = 1.0;
            double sinel = Math.Sin(el);
            int i = sys == DefineConstants.SYS_GLO ? 1 : (sys == DefineConstants.SYS_GAL ? 2 : 0);
            int nf = ((opt).ionoopt == DefineConstants.IONOOPT_IFLC ? 1 : (opt).nf);

            /* extended error model */
            if (f >= nf && opt.exterr.ena[0] != 0) // code
            {
                a = opt.exterr.cerr[i, (f - nf) * 2];
                b = opt.exterr.cerr[i, 1 + (f - nf) * 2];
                if (sys == DefineConstants.SYS_SBS)
                {
                    a *= DefineConstants.EFACT_SBS;
                    b *= DefineConstants.EFACT_SBS;
                }
            }
            else if (f < nf && opt.exterr.ena[1] != 0) // phase
            {
                a = opt.exterr.perr[i, f * 2];
                b = opt.exterr.perr[i, 1 + f * 2];
                if (sys == DefineConstants.SYS_SBS)
                {
                    a *= DefineConstants.EFACT_SBS;
                    b *= DefineConstants.EFACT_SBS;
                }
            }
            else // normal error model
            {
                if (f >= nf)
                {
                    fact = opt.eratio[f - nf];
                }
                if (fact <= 0.0)
                {
                    fact = opt.eratio[0];
                }
                fact *= sys == DefineConstants.SYS_GLO ? DefineConstants.EFACT_GLO : (sys == DefineConstants.SYS_SBS ? DefineConstants.EFACT_SBS : DefineConstants.EFACT_GPS);
                a = fact * opt.err[1];
                b = fact * opt.err[2];
            }
            return 2.0 * (opt.ionoopt == DefineConstants.IONOOPT_IFLC ? 3.0 : 1.0) * (a * a + b * b / sinel / sinel + c * c) + d * d;
        }
        /* baseline length -----------------------------------------------------------*/
        internal static double baseline(double[] ru, double[] rb, double[] dr)
        {
            int i;
            for (i = 0; i < 3; i++)
            {
                dr[i] = ru[i] - rb[i];
            }
            return GlobalMembersRtkcmn.norm(dr, 3);
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
        /* select common satellites between rover and reference station --------------*/
        internal static int selsat(obsd_t[] obs, double[] azel, int nu, int nr, prcopt_t opt, int[] sat, int[] iu, int[] ir)
        {
            int i;
            int j;
            int k = 0;

            GlobalMembersRtkcmn.trace(3, "selsat  : nu=%d nr=%d\n", nu, nr);

            for (i = 0, j = nu; i < nu && j < nu + nr; i++, j++)
            {
                if (obs[i].sat < obs[j].sat)
                {
                    j--;
                }
                else if (obs[i].sat > obs[j].sat)
                {
                    i--;
                }
                else if (azel[1 + j * 2] >= opt.elmin) // elevation at base station
                {
                    sat[k] = obs[i].sat;
                    iu[k] = i;
                    ir[k++] = j;
                    GlobalMembersRtkcmn.trace(4, "(%2d) sat=%3d iu=%2d ir=%2d\n", k - 1, obs[i].sat, i, j);
                }
            }
            return k;
        }
        /* temporal update of position/velocity/acceleration -------------------------*/
        internal static void udpos(rtk_t rtk, double tt)
        {
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: double *F,*FP,*xp,pos[3],Q[9]={0},Qv[9],var=0.0;
            double F;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: double *FP;
            double FP;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: double *xp;
            double xp;
            double[] pos = new double[3];
            double[] Q = { 0, null, null, null, null, null, null, null, null };
            double[] Qv = new double[9];
            double @var = 0.0;
            int i;
            int j;

            GlobalMembersRtkcmn.trace(3, "udpos   : tt=%.3f\n", tt);

            /* fixed mode */
            if (rtk.opt.mode == DefineConstants.PMODE_FIXED)
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
                    GlobalMembersPpp.initx(rtk, rtk.sol.rr[i], (30.0) * (30.0), i);
                }
                if (rtk.opt.dynamics != 0)
                {
                    for (i = 3; i < 6; i++)
                    {
                        GlobalMembersPpp.initx(rtk, rtk.sol.rr[i], (10.0) * (10.0), i);
                    }
                    for (i = 6; i < 9; i++)
                    {
                        GlobalMembersPpp.initx(rtk, 1E-6, (10.0) * (10.0), i);
                    }
                }
            }
            /* static mode */
            if (rtk.opt.mode == DefineConstants.PMODE_STATIC)
                return;

            /* kinmatic mode without dynamics */
            if (rtk.opt.dynamics == 0)
            {
                for (i = 0; i < 3; i++)
                {
                    GlobalMembersPpp.initx(rtk, rtk.sol.rr[i], (30.0) * (30.0), i);
                }
                return;
            }
            /* check variance of estimated postion */
            for (i = 0; i < 3; i++)
            {
                @var += rtk.P[i + i * rtk.nx];
            }
            @var /= 3.0;

            if (@var > (30.0) * (30.0))
            {
                /* reset position with large variance */
                for (i = 0; i < 3; i++)
                {
                    GlobalMembersPpp.initx(rtk, rtk.sol.rr[i], (30.0) * (30.0), i);
                }
                for (i = 3; i < 6; i++)
                {
                    GlobalMembersPpp.initx(rtk, rtk.sol.rr[i], (10.0) * (10.0), i);
                }
                for (i = 6; i < 9; i++)
                {
                    GlobalMembersPpp.initx(rtk, 1E-6, (10.0) * (10.0), i);
                }
                GlobalMembersRtkcmn.trace(2, "reset rtk position due to large variance: var=%.3f\n", @var);
                return;
            }
            /* state transition of position/velocity/acceleration */
            F = GlobalMembersRtkcmn.eye(rtk.nx);
            FP = GlobalMembersRtkcmn.mat(rtk.nx, rtk.nx);
            xp = GlobalMembersRtkcmn.mat(rtk.nx, 1);

            for (i = 0; i < 6; i++)
            {
                F[i + (i + 3) * rtk.nx] = tt;
            }
            /* x=F*x, P=F*P*F+Q */
            GlobalMembersRtkcmn.matmul("NN", rtk.nx, 1, rtk.nx, 1.0, F, rtk.x, 0.0, ref xp);
            GlobalMembersRtkcmn.matcpy(ref rtk.x, xp, rtk.nx, 1);
            GlobalMembersRtkcmn.matmul("NN", rtk.nx, rtk.nx, rtk.nx, 1.0, F, rtk.P, 0.0, ref FP);
            GlobalMembersRtkcmn.matmul("NT", rtk.nx, rtk.nx, rtk.nx, 1.0, FP, F, 0.0, ref rtk.P);

            /* process noise added to only acceleration */
            Q[0] = Q[4] = ((rtk.opt.prn[3]) * (rtk.opt.prn[3]));
            Q[8] = ((rtk.opt.prn[4]) * (rtk.opt.prn[4]));
            GlobalMembersRtkcmn.ecef2pos(rtk.x, pos);
            GlobalMembersRtkcmn.covecef(pos, Q, ref Qv);
            for (i = 0; i < 3; i++)
            {
                for (j = 0; j < 3; j++)
                {
                    rtk.P[i + 6 + (j + 6) * rtk.nx] += Qv[i + j * 3];
                }
            }
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(F);
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(FP);
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(xp);
        }
        /* temporal update of ionospheric parameters ---------------------------------*/
        internal static void udion(rtk_t rtk, double tt, double bl, int[] sat, int ns)
        {
            double el;
            double fact;
            int i;
            int j;

            GlobalMembersRtkcmn.trace(3, "udion   : tt=%.1f bl=%.0f ns=%d\n", tt, bl, ns);

            for (i = 1; i <= DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1; i++)
            {
                j = (((rtk.opt).dynamics == 0 ? 3 : 9) + (i) - 1);
                if (rtk.x[j] != 0.0 && rtk.ssat[i - 1].outc[0] > DefineConstants.GAP_RESION && rtk.ssat[i - 1].outc[1] > DefineConstants.GAP_RESION)
                {
                    rtk.x[j] = 0.0;
                }
            }
            for (i = 0; i < ns; i++)
            {
                j = (((rtk.opt).dynamics == 0 ? 3 : 9) + (sat[i]) - 1);

                if (rtk.x[j] == 0.0)
                {
                    GlobalMembersPpp.initx(rtk, 1E-6, ((rtk.opt.std[1] * bl / 1E4) * (rtk.opt.std[1] * bl / 1E4)), j);
                }
                else
                {
                    /* elevation dependent factor of process noise */
                    el = rtk.ssat[sat[i] - 1].azel[1];
                    fact = Math.Cos(el);
                    rtk.P[j + j * rtk.nx] += ((rtk.opt.prn[1] * bl / 1E4 * fact) * (rtk.opt.prn[1] * bl / 1E4 * fact)) * tt;
                }
            }
        }
        /* temporal update of tropospheric parameters --------------------------------*/
        internal static void udtrop(rtk_t rtk, double tt, double bl)
        {
            int i;
            int j;
            int k;

            GlobalMembersRtkcmn.trace(3, "udtrop  : tt=%.1f\n", tt);

            for (i = 0; i < 2; i++)
            {
                j = (((rtk.opt).dynamics == 0 ? 3 : 9) + ((rtk.opt).ionoopt != DefineConstants.IONOOPT_EST ? 0 : DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1) + ((rtk.opt).tropopt < DefineConstants.TROPOPT_EST ? 0 : ((rtk.opt).tropopt < DefineConstants.TROPOPT_ESTG ? 2 : 6)) / 2 * (i));

                if (rtk.x[j] == 0.0)
                {
                    GlobalMembersPpp.initx(rtk, DefineConstants.INIT_ZWD, ((rtk.opt.std[2]) * (rtk.opt.std[2])), j);

                    if (rtk.opt.tropopt >= DefineConstants.TROPOPT_ESTG)
                    {
                        for (k = 0; k < 2; k++)
                        {
                            GlobalMembersPpp.initx(rtk, 1E-6, (0.001) * (0.001), ++j);
                        }
                    }
                }
                else
                {
                    rtk.P[j + j * rtk.nx] += ((rtk.opt.prn[2]) * (rtk.opt.prn[2])) * tt;

                    if (rtk.opt.tropopt >= DefineConstants.TROPOPT_ESTG)
                    {
                        for (k = 0; k < 2; k++)
                        {
                            rtk.P[++j * (1 + rtk.nx)] += ((rtk.opt.prn[2] * 0.3) * (rtk.opt.prn[2] * 0.3)) * Math.Abs(rtk.tt);
                        }
                    }
                }
            }
        }
        /* temporal update of receiver h/w biases ------------------------------------*/
        internal static void udrcvbias(rtk_t rtk, double tt)
        {
            int i;
            int j;

            GlobalMembersRtkcmn.trace(3, "udrcvbias: tt=%.1f\n", tt);

            for (i = 0; i < DefineConstants.NFREQGLO; i++)
            {
                j = (((rtk.opt).dynamics == 0 ? 3 : 9) + ((rtk.opt).ionoopt != DefineConstants.IONOOPT_EST ? 0 : DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1) + ((rtk.opt).tropopt < DefineConstants.TROPOPT_EST ? 0 : ((rtk.opt).tropopt < DefineConstants.TROPOPT_ESTG ? 2 : 6)) + (i));

                if (rtk.x[j] == 0.0)
                {
                    GlobalMembersPpp.initx(rtk, 1E-6, (1.0) * (1.0), j);
                }
                /* hold to fixed solution */
                else if (rtk.nfix >= rtk.opt.minfix && rtk.sol.ratio > rtk.opt.thresar[0])
                {
                    GlobalMembersPpp.initx(rtk, rtk.xa[j], rtk.Pa[j + j * rtk.na], j);
                }
                else
                {
                    rtk.P[j + j * rtk.nx] += ((DefineConstants.PRN_HWBIAS) * (DefineConstants.PRN_HWBIAS)) * tt;
                }
            }
        }
        /* detect cycle slip by LLI --------------------------------------------------*/
        internal static void detslp_ll(rtk_t rtk, obsd_t[] obs, int i, int rcv)
        {
            byte slip;
            byte LLI1;
            byte LLI2;
            byte LLI;
            int f;
            int sat = obs[i].sat;

            GlobalMembersRtkcmn.trace(3, "detslp_ll: i=%d rcv=%d\n", i, rcv);

            for (f = 0; f < rtk.opt.nf; f++)
            {

                if (obs[i].L[f] == 0.0)
                    continue;

                /* restore previous LLI */
                LLI1 = (rtk.ssat[sat - 1].slip[f] >> 6) & 3;
                LLI2 = (rtk.ssat[sat - 1].slip[f] >> 4) & 3;
                LLI = rcv == 1 ? LLI1 : LLI2;

                /* detect slip by cycle slip flag */
                slip = (rtk.ssat[sat - 1].slip[f] | obs[i].LLI[f]) & 3;

                if (obs[i].LLI[f] & 1)
                {
                    GlobalMembersRtkpos.errmsg(rtk, "slip detected (sat=%2d rcv=%d LLI%d=%x)\n", sat, rcv, f + 1, obs[i].LLI[f]);
                }
                /* detect slip by parity unknown flag transition */
                if (((LLI & 2) && !(obs[i].LLI[f] & 2)) || (!(LLI & 2) && (obs[i].LLI[f] & 2)))
                {
                    GlobalMembersRtkpos.errmsg(rtk, "slip detected (sat=%2d rcv=%d LLI%d=%x->%x)\n", sat, rcv, f + 1, LLI, obs[i].LLI[f]);
                    slip |= 1;
                }
                /* save current LLI and slip flag */
                if (rcv == 1)
                {
                    rtk.ssat[sat - 1].slip[f] = (obs[i].LLI[f] << 6) | (LLI2 << 4) | slip;
                }
                else
                {
                    rtk.ssat[sat - 1].slip[f] = (obs[i].LLI[f] << 4) | (LLI1 << 6) | slip;
                }
            }
        }
        /* detect cycle slip by L1-L2 geometry free phase jump -----------------------*/
        internal static void detslp_gf_L1L2(rtk_t rtk, obsd_t[] obs, int i, int j, nav_t nav)
        {
            int sat = obs[i].sat;
            double g0;
            double g1;

            GlobalMembersRtkcmn.trace(3, "detslp_gf_L1L2: i=%d j=%d\n", i, j);

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: if (rtk->opt.nf<=1||(g1=gfobs_L1L2(obs,i,j,nav->lam[sat-1]))==0.0)
            if (rtk.opt.nf <= 1 || (g1 = GlobalMembersRtkpos.gfobs_L1L2(new obsd_t(obs), i, j, nav.lam[sat - 1])) == 0.0)
                return;

            g0 = rtk.ssat[sat - 1].gf;
            rtk.ssat[sat - 1].gf = g1;

            if (g0 != 0.0 && Math.Abs(g1 - g0) > rtk.opt.thresslip)
            {

                rtk.ssat[sat - 1].slip[0] |= 1;
                rtk.ssat[sat - 1].slip[1] |= 1;

                GlobalMembersRtkpos.errmsg(rtk, "slip detected (sat=%2d GF_L1_L2=%.3f %.3f)\n", sat, g0, g1);
            }
        }
        /* detect cycle slip by L1-L5 geometry free phase jump -----------------------*/
        internal static void detslp_gf_L1L5(rtk_t rtk, obsd_t[] obs, int i, int j, nav_t nav)
        {
            int sat = obs[i].sat;
            double g0;
            double g1;

            GlobalMembersRtkcmn.trace(3, "detslp_gf_L1L5: i=%d j=%d\n", i, j);

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: if (rtk->opt.nf<=2||(g1=gfobs_L1L5(obs,i,j,nav->lam[sat-1]))==0.0)
            if (rtk.opt.nf <= 2 || (g1 = GlobalMembersRtkpos.gfobs_L1L5(new obsd_t(obs), i, j, nav.lam[sat - 1])) == 0.0)
                return;

            g0 = rtk.ssat[sat - 1].gf2;
            rtk.ssat[sat - 1].gf2 = g1;

            if (g0 != 0.0 && Math.Abs(g1 - g0) > rtk.opt.thresslip)
            {

                rtk.ssat[sat - 1].slip[0] |= 1;
                rtk.ssat[sat - 1].slip[2] |= 1;

                GlobalMembersRtkpos.errmsg(rtk, "slip detected (sat=%2d GF_L1_L5=%.3f %.3f)\n", sat, g0, g1);
            }
        }
        /* detect cycle slip by doppler and phase difference -------------------------*/
        internal static void detslp_dop(rtk_t rtk, obsd_t obs, int i, int rcv, nav_t nav)
        {
            /* detection with doppler disabled because of clock-jump issue (v.2.3.0) */
#if false
	//    int f,sat=obs[i].sat;
	//    double tt,dph,dpt,lam,thres;
	//    
	//    trace(3,"detslp_dop: i=%d rcv=%d\n",i,rcv);
	//    
	//    for (f=0;f<rtk->opt.nf;f++) {
	//        if (obs[i].L[f]==0.0||obs[i].D[f]==0.0||rtk->ph[rcv-1][sat-1][f]==0.0) {
	//            continue;
	//        }
	//        if (fabs(tt=timediff(obs[i].time,rtk->pt[rcv-1][sat-1][f]))<DTTOL) continue;
	//        if ((lam=nav->lam[sat-1][f])<=0.0) continue;
	//        
	// /* cycle slip threshold (cycle) */
	//        thres=MAXACC*tt*tt/2.0/lam+rtk->opt.err[4]*fabs(tt)*4.0;
	//        
	// /* phase difference and doppler x time (cycle) */
	//        dph=obs[i].L[f]-rtk->ph[rcv-1][sat-1][f];
	//        dpt=-obs[i].D[f]*tt;
	//        
	//        if (fabs(dph-dpt)<=thres) continue;
	//        
	//        rtk->slip[sat-1][f]|=1;
	//        
	//        errmsg(rtk,"slip detected (sat=%2d rcv=%d L%d=%.3f %.3f thres=%.3f)\n",
	//               sat,rcv,f+1,dph,dpt,thres);
	//    }
#endif
        }
        /* temporal update of phase biases -------------------------------------------*/
        //C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on the parameter 'rtk', so pointers on this parameter are left unchanged:
        internal static void udbias(rtk_t* rtk, double tt, obsd_t obs, int[] sat, int[] iu, int[] ir, int ns, nav_t nav)
        {
            double cp;
            double pr;
            double cp1;
            double cp2;
            double pr1;
            double pr2;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: double *bias;
            double bias;
            double offset;
            double lami;
            double lam1;
            double lam2;
            double C1;
            double C2;
            int i;
            int j;
            int f;
            int slip;
            int reset;
            int nf = ((rtk.opt).ionoopt == DefineConstants.IONOOPT_IFLC ? 1 : (rtk.opt).nf);

            GlobalMembersRtkcmn.trace(3, "udbias  : tt=%.1f ns=%d\n", tt, ns);

            for (i = 0; i < ns; i++)
            {

                /* detect cycle slip by LLI */
                for (f = 0; f < rtk.opt.nf; f++)
                {
                    rtk.ssat[sat[i] - 1].slip[f] &= 0xFC;
                }
                GlobalMembersPpp.detslp_ll(rtk, obs, iu[i], 1);
                GlobalMembersPpp.detslp_ll(rtk, obs, ir[i], 2);

                /* detect cycle slip by geometry-free phase jump */
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: detslp_gf_L1L2(rtk,obs,iu[i],ir[i],nav);
                GlobalMembersRtkpos.detslp_gf_L1L2(new rtk_t(rtk), obs, iu[i], ir[i], nav);
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: detslp_gf_L1L5(rtk,obs,iu[i],ir[i],nav);
                GlobalMembersRtkpos.detslp_gf_L1L5(new rtk_t(rtk), obs, iu[i], ir[i], nav);

                /* detect cycle slip by doppler and phase difference */
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: detslp_dop(rtk,obs,iu[i],1,nav);
                GlobalMembersRtkpos.detslp_dop(new rtk_t(rtk), obs, iu[i], 1, nav);
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: detslp_dop(rtk,obs,ir[i],2,nav);
                GlobalMembersRtkpos.detslp_dop(new rtk_t(rtk), obs, ir[i], 2, nav);
            }
            for (f = 0; f < nf; f++)
            {
                /* reset phase-bias if instantaneous AR or expire obs outage counter */
                for (i = 1; i <= DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1; i++)
                {

                    reset = ++rtk.ssat[i - 1].outc[f] > (uint)rtk.opt.maxout;

                    if (rtk.opt.modear == DefineConstants.ARMODE_INST && rtk.x[((((rtk.opt).dynamics == 0 ? 3 : 9) + ((rtk.opt).ionoopt != DefineConstants.IONOOPT_EST ? 0 : DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1) + ((rtk.opt).tropopt < DefineConstants.TROPOPT_EST ? 0 : ((rtk.opt).tropopt < DefineConstants.TROPOPT_ESTG ? 2 : 6)) + ((rtk.opt).glomodear != 2 ? 0 : DefineConstants.NFREQGLO)) + DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1 * (f) + (i) - 1)] != 0.0)
                    {
                        //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                        //ORIGINAL LINE: initx(rtk,0.0,0.0,((((&rtk->opt)->dynamics==0?3:9)+((&rtk->opt)->ionoopt!=DefineConstants.IONOOPT_EST?0:DefineConstants.MAXPRNGPS-DefineConstants.MINPRNGPS+1+DefineConstants.MAXPRNGLO-DefineConstants.MINPRNGLO+1+DefineConstants.MAXPRNGAL-DefineConstants.MINPRNGAL+1+DefineConstants.MAXPRNQZS-DefineConstants.MINPRNQZS+1+DefineConstants.MAXPRNCMP-DefineConstants.MINPRNCMP+1+DefineConstants.MAXPRNSBS-DefineConstants.MINPRNSBS+1+DefineConstants.MAXPRNLEO-DefineConstants.MINPRNLEO+1)+((&rtk->opt)->tropopt<DefineConstants.TROPOPT_EST?0:((&rtk->opt)->tropopt<DefineConstants.TROPOPT_ESTG?2:6))+((&rtk->opt)->glomodear!=2?0:DefineConstants.NFREQGLO))+DefineConstants.MAXPRNGPS-DefineConstants.MINPRNGPS+1+DefineConstants.MAXPRNGLO-DefineConstants.MINPRNGLO+1+DefineConstants.MAXPRNGAL-DefineConstants.MINPRNGAL+1+DefineConstants.MAXPRNQZS-DefineConstants.MINPRNQZS+1+DefineConstants.MAXPRNCMP-DefineConstants.MINPRNCMP+1+DefineConstants.MAXPRNSBS-DefineConstants.MINPRNSBS+1+DefineConstants.MAXPRNLEO-DefineConstants.MINPRNLEO+1*(f)+(i)-1));
                        GlobalMembersPpp.initx(new rtk_t(rtk), 0.0, 0.0, ((((rtk.opt).dynamics == 0 ? 3 : 9) + ((rtk.opt).ionoopt != DefineConstants.IONOOPT_EST ? 0 : DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1) + ((rtk.opt).tropopt < DefineConstants.TROPOPT_EST ? 0 : ((rtk.opt).tropopt < DefineConstants.TROPOPT_ESTG ? 2 : 6)) + ((rtk.opt).glomodear != 2 ? 0 : DefineConstants.NFREQGLO)) + DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1 * (f) + (i) - 1));
                    }
                    else if (reset != 0 && rtk.x[((((rtk.opt).dynamics == 0 ? 3 : 9) + ((rtk.opt).ionoopt != DefineConstants.IONOOPT_EST ? 0 : DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1) + ((rtk.opt).tropopt < DefineConstants.TROPOPT_EST ? 0 : ((rtk.opt).tropopt < DefineConstants.TROPOPT_ESTG ? 2 : 6)) + ((rtk.opt).glomodear != 2 ? 0 : DefineConstants.NFREQGLO)) + DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1 * (f) + (i) - 1)] != 0.0)
                    {
                        //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                        //ORIGINAL LINE: initx(rtk,0.0,0.0,((((&rtk->opt)->dynamics==0?3:9)+((&rtk->opt)->ionoopt!=DefineConstants.IONOOPT_EST?0:DefineConstants.MAXPRNGPS-DefineConstants.MINPRNGPS+1+DefineConstants.MAXPRNGLO-DefineConstants.MINPRNGLO+1+DefineConstants.MAXPRNGAL-DefineConstants.MINPRNGAL+1+DefineConstants.MAXPRNQZS-DefineConstants.MINPRNQZS+1+DefineConstants.MAXPRNCMP-DefineConstants.MINPRNCMP+1+DefineConstants.MAXPRNSBS-DefineConstants.MINPRNSBS+1+DefineConstants.MAXPRNLEO-DefineConstants.MINPRNLEO+1)+((&rtk->opt)->tropopt<DefineConstants.TROPOPT_EST?0:((&rtk->opt)->tropopt<DefineConstants.TROPOPT_ESTG?2:6))+((&rtk->opt)->glomodear!=2?0:DefineConstants.NFREQGLO))+DefineConstants.MAXPRNGPS-DefineConstants.MINPRNGPS+1+DefineConstants.MAXPRNGLO-DefineConstants.MINPRNGLO+1+DefineConstants.MAXPRNGAL-DefineConstants.MINPRNGAL+1+DefineConstants.MAXPRNQZS-DefineConstants.MINPRNQZS+1+DefineConstants.MAXPRNCMP-DefineConstants.MINPRNCMP+1+DefineConstants.MAXPRNSBS-DefineConstants.MINPRNSBS+1+DefineConstants.MAXPRNLEO-DefineConstants.MINPRNLEO+1*(f)+(i)-1));
                        GlobalMembersPpp.initx(new rtk_t(rtk), 0.0, 0.0, ((((rtk.opt).dynamics == 0 ? 3 : 9) + ((rtk.opt).ionoopt != DefineConstants.IONOOPT_EST ? 0 : DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1) + ((rtk.opt).tropopt < DefineConstants.TROPOPT_EST ? 0 : ((rtk.opt).tropopt < DefineConstants.TROPOPT_ESTG ? 2 : 6)) + ((rtk.opt).glomodear != 2 ? 0 : DefineConstants.NFREQGLO)) + DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1 * (f) + (i) - 1));
                        GlobalMembersRtkcmn.trace(3, "udbias : obs outage counter overflow (sat=%3d L%d n=%d)\n", i, f + 1, rtk.ssat[i - 1].outc[f]);
                    }
                    if (rtk.opt.modear != DefineConstants.ARMODE_INST && reset != 0)
                    {
                        rtk.ssat[i - 1].@lock[f] = -rtk.opt.minlock;
                    }
                }
                /* reset phase-bias if detecting cycle slip */
                for (i = 0; i < ns; i++)
                {
                    j = ((((rtk.opt).dynamics == 0 ? 3 : 9) + ((rtk.opt).ionoopt != DefineConstants.IONOOPT_EST ? 0 : DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1) + ((rtk.opt).tropopt < DefineConstants.TROPOPT_EST ? 0 : ((rtk.opt).tropopt < DefineConstants.TROPOPT_ESTG ? 2 : 6)) + ((rtk.opt).glomodear != 2 ? 0 : DefineConstants.NFREQGLO)) + DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1 * (f) + (sat[i]) - 1);
                    rtk.P[j + j * rtk.nx] += rtk.opt.prn[0] * rtk.opt.prn[0] * tt;
                    slip = rtk.ssat[sat[i] - 1].slip[f];
                    if (rtk.opt.ionoopt == DefineConstants.IONOOPT_IFLC)
                    {
                        slip |= rtk.ssat[sat[i] - 1].slip[1];
                    }
                    if (rtk.opt.modear == DefineConstants.ARMODE_INST || !(slip & 1))
                        continue;
                    rtk.x[j] = 0.0;
                    rtk.ssat[sat[i] - 1].@lock[f] = -rtk.opt.minlock;
                }
                bias = GlobalMembersRtkcmn.zeros(ns, 1);

                /* estimate approximate phase-bias by phase - code */
                for (i = j = 0, offset = 0.0; i < ns; i++)
                {

                    if (rtk.opt.ionoopt != DefineConstants.IONOOPT_IFLC)
                    {
                        cp = GlobalMembersRtkpos.sdobs(obs, iu[i], ir[i], f); // cycle
                        pr = GlobalMembersRtkpos.sdobs(obs, iu[i], ir[i], f + DefineConstants.NFREQ);
                        lami = nav.lam[sat[i] - 1, f];
                        if (cp == 0.0 || pr == 0.0 || lami <= 0.0)
                            continue;

                        bias[i] = cp - pr / lami;
                    }
                    else
                    {
                        cp1 = GlobalMembersRtkpos.sdobs(obs, iu[i], ir[i], 0);
                        cp2 = GlobalMembersRtkpos.sdobs(obs, iu[i], ir[i], 1);
                        pr1 = GlobalMembersRtkpos.sdobs(obs, iu[i], ir[i], DefineConstants.NFREQ);
                        pr2 = GlobalMembersRtkpos.sdobs(obs, iu[i], ir[i], DefineConstants.NFREQ + 1);
                        lam1 = nav.lam[sat[i] - 1, 0];
                        lam2 = nav.lam[sat[i] - 1, 1];
                        if (cp1 == 0.0 || cp2 == 0.0 || pr1 == 0.0 || pr2 == 0.0 || lam1 <= 0.0 || lam2 <= 0.0)
                            continue;

                        C1 = ((lam2) * (lam2)) / (((lam2) * (lam2)) - ((lam1) * (lam1)));
                        C2 = -((lam1) * (lam1)) / (((lam2) * (lam2)) - ((lam1) * (lam1)));
                        bias[i] = (C1 * lam1 * cp1 + C2 * lam2 * cp2) - (C1 * pr1 + C2 * pr2);
                    }
                    if (rtk.x[((((rtk.opt).dynamics == 0 ? 3 : 9) + ((rtk.opt).ionoopt != DefineConstants.IONOOPT_EST ? 0 : DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1) + ((rtk.opt).tropopt < DefineConstants.TROPOPT_EST ? 0 : ((rtk.opt).tropopt < DefineConstants.TROPOPT_ESTG ? 2 : 6)) + ((rtk.opt).glomodear != 2 ? 0 : DefineConstants.NFREQGLO)) + DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1 * (f) + (sat[i]) - 1)] != 0.0)
                    {
                        offset += bias[i] - rtk.x[((((rtk.opt).dynamics == 0 ? 3 : 9) + ((rtk.opt).ionoopt != DefineConstants.IONOOPT_EST ? 0 : DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1) + ((rtk.opt).tropopt < DefineConstants.TROPOPT_EST ? 0 : ((rtk.opt).tropopt < DefineConstants.TROPOPT_ESTG ? 2 : 6)) + ((rtk.opt).glomodear != 2 ? 0 : DefineConstants.NFREQGLO)) + DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1 * (f) + (sat[i]) - 1)];
                        j++;
                    }
                }
                /* correct phase-bias offset to enssure phase-code coherency */
                if (j > 0)
                {
                    for (i = 1; i <= DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1; i++)
                    {
                        if (rtk.x[((((rtk.opt).dynamics == 0 ? 3 : 9) + ((rtk.opt).ionoopt != DefineConstants.IONOOPT_EST ? 0 : DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1) + ((rtk.opt).tropopt < DefineConstants.TROPOPT_EST ? 0 : ((rtk.opt).tropopt < DefineConstants.TROPOPT_ESTG ? 2 : 6)) + ((rtk.opt).glomodear != 2 ? 0 : DefineConstants.NFREQGLO)) + DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1 * (f) + (i) - 1)] != 0.0)
                        {
                            rtk.x[((((rtk.opt).dynamics == 0 ? 3 : 9) + ((rtk.opt).ionoopt != DefineConstants.IONOOPT_EST ? 0 : DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1) + ((rtk.opt).tropopt < DefineConstants.TROPOPT_EST ? 0 : ((rtk.opt).tropopt < DefineConstants.TROPOPT_ESTG ? 2 : 6)) + ((rtk.opt).glomodear != 2 ? 0 : DefineConstants.NFREQGLO)) + DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1 * (f) + (i) - 1)] += offset / j;
                        }
                    }
                }
                /* set initial states of phase-bias */
                for (i = 0; i < ns; i++)
                {
                    if (bias[i] == 0.0 || rtk.x[((((rtk.opt).dynamics == 0 ? 3 : 9) + ((rtk.opt).ionoopt != DefineConstants.IONOOPT_EST ? 0 : DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1) + ((rtk.opt).tropopt < DefineConstants.TROPOPT_EST ? 0 : ((rtk.opt).tropopt < DefineConstants.TROPOPT_ESTG ? 2 : 6)) + ((rtk.opt).glomodear != 2 ? 0 : DefineConstants.NFREQGLO)) + DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1 * (f) + (sat[i]) - 1)] != 0.0)
                        continue;
                    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                    //ORIGINAL LINE: initx(rtk,bias[i],((rtk->opt.std[0])*(rtk->opt.std[0])),((((&rtk->opt)->dynamics==0?3:9)+((&rtk->opt)->ionoopt!=DefineConstants.IONOOPT_EST?0:DefineConstants.MAXPRNGPS-DefineConstants.MINPRNGPS+1+DefineConstants.MAXPRNGLO-DefineConstants.MINPRNGLO+1+DefineConstants.MAXPRNGAL-DefineConstants.MINPRNGAL+1+DefineConstants.MAXPRNQZS-DefineConstants.MINPRNQZS+1+DefineConstants.MAXPRNCMP-DefineConstants.MINPRNCMP+1+DefineConstants.MAXPRNSBS-DefineConstants.MINPRNSBS+1+DefineConstants.MAXPRNLEO-DefineConstants.MINPRNLEO+1)+((&rtk->opt)->tropopt<DefineConstants.TROPOPT_EST?0:((&rtk->opt)->tropopt<DefineConstants.TROPOPT_ESTG?2:6))+((&rtk->opt)->glomodear!=2?0:DefineConstants.NFREQGLO))+DefineConstants.MAXPRNGPS-DefineConstants.MINPRNGPS+1+DefineConstants.MAXPRNGLO-DefineConstants.MINPRNGLO+1+DefineConstants.MAXPRNGAL-DefineConstants.MINPRNGAL+1+DefineConstants.MAXPRNQZS-DefineConstants.MINPRNQZS+1+DefineConstants.MAXPRNCMP-DefineConstants.MINPRNCMP+1+DefineConstants.MAXPRNSBS-DefineConstants.MINPRNSBS+1+DefineConstants.MAXPRNLEO-DefineConstants.MINPRNLEO+1*(f)+(sat[i])-1));
                    GlobalMembersPpp.initx(new rtk_t(rtk), bias[i], ((rtk.opt.std[0]) * (rtk.opt.std[0])), ((((rtk.opt).dynamics == 0 ? 3 : 9) + ((rtk.opt).ionoopt != DefineConstants.IONOOPT_EST ? 0 : DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1) + ((rtk.opt).tropopt < DefineConstants.TROPOPT_EST ? 0 : ((rtk.opt).tropopt < DefineConstants.TROPOPT_ESTG ? 2 : 6)) + ((rtk.opt).glomodear != 2 ? 0 : DefineConstants.NFREQGLO)) + DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1 * (f) + (sat[i]) - 1));
                }
                //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
                free(bias);
            }
        }
        /* temporal update of states --------------------------------------------------*/
        internal static void udstate(rtk_t rtk, obsd_t obs, int sat, int iu, int ir, int ns, nav_t nav)
        {
            double tt = Math.Abs(rtk.tt);
            double bl;
            double[] dr = new double[3];

            GlobalMembersRtkcmn.trace(3, "udstate : ns=%d\n", ns);

            /* temporal update of position/velocity/acceleration */
            GlobalMembersRtkpos.udpos(rtk, tt);

            /* temporal update of ionospheric parameters */
            if (rtk.opt.ionoopt >= DefineConstants.IONOOPT_EST)
            {
                bl = GlobalMembersRtkpos.baseline(rtk.x, rtk.rb, dr);
                GlobalMembersRtkpos.udion(rtk, tt, bl, sat, ns);
            }
            /* temporal update of tropospheric parameters */
            if (rtk.opt.tropopt >= DefineConstants.TROPOPT_EST)
            {
                GlobalMembersRtkpos.udtrop(rtk, tt, bl);
            }
            /* temporal update of eceiver h/w bias */
            if (rtk.opt.glomodear == 2 && (rtk.opt.navsys & DefineConstants.SYS_GLO))
            {
                GlobalMembersRtkpos.udrcvbias(rtk, tt);
            }
            /* temporal update of phase-bias */
            if (rtk.opt.mode > DefineConstants.PMODE_DGPS)
            {
                GlobalMembersRtkpos.udbias(rtk, tt, obs, sat, iu, ir, ns, nav);
            }
        }
        /* undifferenced phase/code residual for satellite ---------------------------*/
        internal static void zdres_sat(int @base, double r, obsd_t obs, nav_t nav, double[] azel, double[] dant, prcopt_t opt, double[] y)
        {
            double[] lam = nav.lam[obs.sat - 1];
            double f1;
            double f2;
            double C1;
            double C2;
            double dant_if;
            int i;
            int nf = ((opt).ionoopt == DefineConstants.IONOOPT_IFLC ? 1 : (opt).nf);

            if (opt.ionoopt == DefineConstants.IONOOPT_IFLC) // iono-free linear combination
            {
                if (lam[0] == 0.0 || lam[1] == 0.0)
                    return;

                if (GlobalMembersRtkcmn.testsnr(@base, 0, azel[1], obs.SNR[0] * 0.25, opt.snrmask) != 0 || GlobalMembersRtkcmn.testsnr(@base, 1, azel[1], obs.SNR[1] * 0.25, opt.snrmask) != 0)
                    return;

                f1 = DefineConstants.CLIGHT / lam[0];
                f2 = DefineConstants.CLIGHT / lam[1];
                C1 = ((f1) * (f1)) / (((f1) * (f1)) - ((f2) * (f2)));
                C2 = -((f2) * (f2)) / (((f1) * (f1)) - ((f2) * (f2)));
                dant_if = C1 * dant[0] + C2 * dant[1];

                if (obs.L[0] != 0.0 && obs.L[1] != 0.0)
                {
                    y[0] = C1 * obs.L[0] * lam[0] + C2 * obs.L[1] * lam[1] - r - dant_if;
                }
                if (obs.P[0] != 0.0 && obs.P[1] != 0.0)
                {
                    y[1] = C1 * obs.P[0] + C2 * obs.P[1] - r - dant_if;
                }
            }
            else
            {
                for (i = 0; i < nf; i++)
                {
                    if (lam[i] == 0.0)
                        continue;

                    /* check snr mask */
                    if (GlobalMembersRtkcmn.testsnr(@base, i, azel[1], obs.SNR[i] * 0.25, opt.snrmask) != 0)
                    {
                        continue;
                    }
                    /* residuals = observable - pseudorange */
                    if (obs.L[i] != 0.0)
                    {
                        y[i] = obs.L[i] * lam[i] - r - dant[i];
                    }
                    if (obs.P[i] != 0.0)
                    {
                        y[i + nf] = obs.P[i] - r - dant[i];
                    }
                }
            }
        }
        /* undifferenced phase/code residuals ----------------------------------------*/
        internal static int zdres(int @base, obsd_t[] obs, int n, double[] rs, double[] dts, int[] svh, nav_t nav, double[] rr, prcopt_t opt, int index, double[] y, ref double e, double[] azel)
        {
            double r;
            double[] rr_ = new double[3];
            double[] pos = new double[3];
            double[] dant = { 0 };
            double[] disp = new double[3];
            double zhd;
            double[] zazel = { 0.0, 90.0 * DefineConstants.PI / 180.0 };
            int i;
            int nf = ((opt).ionoopt == DefineConstants.IONOOPT_IFLC ? 1 : (opt).nf);

            GlobalMembersRtkcmn.trace(3, "zdres   : n=%d\n", n);

            for (i = 0; i < n * nf * 2; i++)
            {
                y[i] = 0.0;
            }

            if (GlobalMembersRtkcmn.norm(rr, 3) <= 0.0) // no receiver position
            {
                return 0;
            }

            for (i = 0; i < 3; i++)
            {
                rr_[i] = rr[i];
            }

            /* earth tide correction */
            if (opt.tidecorr != 0)
            {
                GlobalMembersPpp.tidedisp(GlobalMembersRtkcmn.gpst2utc(obs[0].time), rr_, opt.tidecorr, nav.erp, opt.odisp[@base], disp);
                for (i = 0; i < 3; i++)
                {
                    rr_[i] += disp[i];
                }
            }
            GlobalMembersRtkcmn.ecef2pos(rr_, pos);

            for (i = 0; i < n; i++)
            {
                /* compute geometric-range and azimuth/elevation angle */
                if ((r = GlobalMembersRtkcmn.geodist(rs + i * 6, rr_, e + i * 3)) <= 0.0)
                    continue;
                if (GlobalMembersRtkcmn.satazel(pos, e + i * 3, azel + i * 2) < opt.elmin)
                    continue;

                /* excluded satellite? */
                if (GlobalMembersRtkcmn.satexclude(obs[i].sat, svh[i], opt) != 0)
                    continue;

                /* satellite clock-bias */
                r += -DefineConstants.CLIGHT * dts[i * 2];

                /* troposphere delay model (hydrostatic) */
                zhd = GlobalMembersRtkcmn.tropmodel(obs[0].time, pos, zazel, 0.0);
                r += GlobalMembersRtkcmn.tropmapf(obs[i].time, pos, azel + i * 2, null) * zhd;

                /* receiver antenna phase center correction */
                GlobalMembersRtkcmn.antmodel(opt.pcvr + index, opt.antdel[index], azel + i * 2, opt.posopt[1], dant);

                /* undifferenced phase/code residual for satellite */
                GlobalMembersRtkpos.zdres_sat(@base, r, obs + i, nav, azel + i * 2, dant, opt, y + i * nf * 2);
            }
            GlobalMembersRtkcmn.trace(4, "rr_=%.3f %.3f %.3f\n", rr_[0], rr_[1], rr_[2]);
            GlobalMembersRtkcmn.trace(4, "pos=%.9f %.9f %.3f\n", pos[0] * 180.0 / DefineConstants.PI, pos[1] * 180.0 / DefineConstants.PI, pos[2]);
            for (i = 0; i < n; i++)
            {
                GlobalMembersRtkcmn.trace(4, "sat=%2d %13.3f %13.3f %13.3f %13.10f %6.1f %5.1f\n", obs[i].sat, rs[i * 6], rs[1 + i * 6], rs[2 + i * 6], dts[i * 2], azel[i * 2] * 180.0 / DefineConstants.PI, azel[1 + i * 2] * 180.0 / DefineConstants.PI);
            }
            GlobalMembersRtkcmn.trace(4, "y=\n");
            GlobalMembersRtkcmn.tracemat(4, y, nf * 2, n, 13, 3);

            return 1;
        }
        /* test valid observation data -----------------------------------------------*/
        internal static int validobs(int i, int j, int f, int nf, double[] y)
        {
            /* if no phase observable, psudorange is also unusable */
            return y[f + i * nf * 2] != 0.0 && y[f + j * nf * 2] != 0.0 && (f < nf || (y[f - nf + i * nf * 2] != 0.0 && y[f - nf + j * nf * 2] != 0.0));
        }
        /* double-differenced measurement error covariance ---------------------------*/
        internal static void ddcov(int[] nb, int n, double[] Ri, double[] Rj, int nv, double[] R)
        {
            int i;
            int j;
            int k = 0;
            int b;

            GlobalMembersRtkcmn.trace(3, "ddcov   : n=%d\n", n);

            for (i = 0; i < nv * nv; i++)
            {
                R[i] = 0.0;
            }
            for (b = 0; b < n; k += nb[b++])
            {

                for (i = 0; i < nb[b]; i++)
                {
                    for (j = 0; j < nb[b]; j++)
                    {
                        R[k + i + (k + j) * nv] = Ri[k + i] + (i == j != 0 ? Rj[k + i] : 0.0);
                    }
                }
            }
            GlobalMembersRtkcmn.trace(5, "R=\n");
            GlobalMembersRtkcmn.tracemat(5, R, nv, nv, 8, 6);
        }
        /* baseline length constraint ------------------------------------------------*/
        internal static int constbl(rtk_t rtk, double[] x, double[] P, double[] v, double[] H, double[] Ri, double[] Rj, int index)
        {
            const double thres = 0.1; // threshold for nonliearity (v.2.3.0)
            double[] xb = new double[3];
            double[] b = new double[3];
            double bb;
            double @var = 0.0;
            int i;

            GlobalMembersRtkcmn.trace(3, "constbl : \n");

            /* no constraint */
            if (rtk.opt.baseline[0] <= 0.0)
            {
                return 0;
            }

            /* time-adjusted baseline vector and length */
            for (i = 0; i < 3; i++)
            {
                xb[i] = rtk.rb[i] + rtk.rb[i + 3] * rtk.sol.age;
                b[i] = x[i] - xb[i];
            }
            bb = GlobalMembersRtkcmn.norm(b, 3);

            /* approximate variance of solution */
            if (P != 0)
            {
                for (i = 0; i < 3; i++)
                {
                    @var += P[i + i * rtk.nx];
                }
                @var /= 3.0;
            }
            /* check nonlinearity */
            if (@var > thres * thres * bb * bb)
            {
                GlobalMembersRtkcmn.trace(3, "constbl : equation nonlinear (bb=%.3f var=%.3f)\n", bb, @var);
                return 0;
            }
            /* constraint to baseline length */
            v[index] = rtk.opt.baseline[0] - bb;
            if (H != 0)
            {
                for (i = 0; i < 3; i++)
                {
                    H[i + index * rtk.nx] = b[i] / bb;
                }
            }
            Ri[index] = 0.0;
            Rj[index] = ((rtk.opt.baseline[1]) * (rtk.opt.baseline[1]));

            GlobalMembersRtkcmn.trace(4, "baseline len   v=%13.3f R=%8.6f %8.6f\n", v[index], Ri[index], Rj[index]);

            return 1;
        }
        /* precise tropspheric model -------------------------------------------------*/
        internal static double prectrop(gtime_t time, double pos, int r, double[] azel, prcopt_t opt, double[] x, double[] dtdx)
        {
            double m_w = 0.0;
            double cotz;
            double grad_n;
            double grad_e;
            int i = (((opt).dynamics == 0 ? 3 : 9) + ((opt).ionoopt != DefineConstants.IONOOPT_EST ? 0 : DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1) + ((opt).tropopt < DefineConstants.TROPOPT_EST ? 0 : ((opt).tropopt < DefineConstants.TROPOPT_ESTG ? 2 : 6)) / 2 * (r));

            /* wet mapping function */
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: tropmapf(time,pos,azel,&m_w);
            GlobalMembersRtkcmn.tropmapf(new gtime_t(time), pos, azel, ref m_w);

            if (opt.tropopt >= DefineConstants.TROPOPT_ESTG && azel[1] > 0.0)
            {

                /* m_w=m_0+m_0*cot(el)*(Gn*cos(az)+Ge*sin(az)): ref [6] */
                cotz = 1.0 / Math.Tan(azel[1]);
                grad_n = m_w * cotz * Math.Cos(azel[0]);
                grad_e = m_w * cotz * Math.Sin(azel[0]);
                m_w += grad_n * x[i + 1] + grad_e * x[i + 2];
                dtdx[1] = grad_n * x[i];
                dtdx[2] = grad_e * x[i];
            }
            else
            {
                dtdx[1] = dtdx[2] = 0.0;
            }
            dtdx[0] = m_w;
            return m_w * x[i];
        }
        /* glonass inter-channel bias correction -------------------------------------*/
        internal static double gloicbcorr(int sat1, int sat2, prcopt_t opt, double lam1, double lam2, int f)
        {
            double dfreq;

            if (f >= DefineConstants.NFREQGLO || f >= opt.nf || opt.exterr.ena[2] == 0)
            {
                return 0.0;
            }

            dfreq = (DefineConstants.CLIGHT / lam1 - DefineConstants.CLIGHT / lam2) / (f == 0 ? DefineConstants.DFRQ1_GLO : DefineConstants.DFRQ2_GLO);

            return opt.exterr.gloicb[f] * 0.01 * dfreq; // (m)
        }
        /* test navi system (m=0:gps/qzs/sbs,1:glo,2:gal,3:bds) ----------------------*/
        internal static int test_sys(int sys, int m)
        {
            switch (sys)
            {
                case DefineConstants.SYS_GPS:
                    return m == 0;
                case DefineConstants.SYS_QZS:
                    return m == 0;
                case DefineConstants.SYS_SBS:
                    return m == 0;
                case DefineConstants.SYS_GLO:
                    return m == 1;
                case DefineConstants.SYS_GAL:
                    return m == 2;
                case DefineConstants.SYS_CMP:
                    return m == 3;
            }
            return 0;
        }
        /* double-differenced phase/code residuals -----------------------------------*/
        internal static int ddres(rtk_t rtk, nav_t nav, double dt, double[] x, double P, int[] sat, double[] y, double[] e, double[] azel, int[] iu, int[] ir, int ns, double[] v, ref double H, ref double R, int[] vflg)
        {
            prcopt_t opt = rtk.opt;
            double bl;
            double[] dr = new double[3];
            double[] posu = new double[3];
            double[] posr = new double[3];
            double didxi = 0.0;
            double didxj = 0.0;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: double *im;
            double im;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: double *tropr,*tropu,*dtdxr,*dtdxu,*Ri,*Rj,lami,lamj,fi,fj,df,*Hi=null;
            double tropr;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: double *tropu;
            double tropu;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: double *dtdxr;
            double dtdxr;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: double *dtdxu;
            double dtdxu;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: double *Ri;
            double Ri;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: double *Rj;
            double Rj;
            double lami;
            double lamj;
            double fi;
            double fj;
            double df;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: double *Hi=null;
            double Hi = null;
            int i;
            int j;
            int k;
            int m;
            int f;
            int ff;
            int nv = 0;
            int[] nb = new int[DefineConstants.NFREQ * 4 * 2 + 2];
            int b = 0;
            int sysi;
            int sysj;
            int nf = ((opt).ionoopt == DefineConstants.IONOOPT_IFLC ? 1 : (opt).nf);

            GlobalMembersRtkcmn.trace(3, "ddres   : dt=%.1f nx=%d ns=%d\n", dt, rtk.nx, ns);

            bl = GlobalMembersRtkpos.baseline(x, rtk.rb, dr);
            GlobalMembersRtkcmn.ecef2pos(x, posu);
            GlobalMembersRtkcmn.ecef2pos(rtk.rb, posr);

            Ri = GlobalMembersRtkcmn.mat(ns * nf * 2 + 2, 1);
            Rj = GlobalMembersRtkcmn.mat(ns * nf * 2 + 2, 1);
            im = GlobalMembersRtkcmn.mat(ns, 1);
            tropu = GlobalMembersRtkcmn.mat(ns, 1);
            tropr = GlobalMembersRtkcmn.mat(ns, 1);
            dtdxu = GlobalMembersRtkcmn.mat(ns, 3);
            dtdxr = GlobalMembersRtkcmn.mat(ns, 3);

            for (i = 0; i < DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1; i++)
            {
                for (j = 0; j < DefineConstants.NFREQ; j++)
                {
                    rtk.ssat[i].resp[j] = rtk.ssat[i].resc[j] = 0.0;
                }
            }
            /* compute factors of ionospheric and tropospheric delay */
            for (i = 0; i < ns; i++)
            {
                if (opt.ionoopt >= DefineConstants.IONOOPT_EST)
                {
                    im[i] = (GlobalMembersRtkcmn.ionmapf(posu, azel + iu[i] * 2) + GlobalMembersRtkcmn.ionmapf(posr, azel + ir[i] * 2)) / 2.0;
                }
                if (opt.tropopt >= DefineConstants.TROPOPT_EST)
                {
                    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                    //ORIGINAL LINE: tropu[i]=prectrop(rtk->sol.time,posu,0,azel+iu[i]*2,opt,x,dtdxu+i *3);
                    tropu[i] = GlobalMembersPpp.prectrop(new gtime_t(rtk.sol.time), posu, 0, azel + iu[i] * 2, opt, x, ref dtdxu + i * 3);
                    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                    //ORIGINAL LINE: tropr[i]=prectrop(rtk->sol.time,posr,1,azel+ir[i]*2,opt,x,dtdxr+i *3);
                    tropr[i] = GlobalMembersPpp.prectrop(new gtime_t(rtk.sol.time), posr, 1, azel + ir[i] * 2, opt, x, ref dtdxr + i * 3);
                }
            }
            for (m = 0; m < 4; m++) // m=0:gps/qzs/sbs,1:glo,2:gal,3:bds

            {
                for (f = opt.mode > DefineConstants.PMODE_DGPS ? 0 : nf; f < nf * 2; f++)
                {

                    /* search reference satellite with highest elevation */
                    for (i = -1, j = 0; j < ns; j++)
                    {
                        sysi = rtk.ssat[sat[j] - 1].sys;
                        if (GlobalMembersRtkpos.test_sys(sysi, m) == 0)
                            continue;
                        if (GlobalMembersRtkpos.validobs(iu[j], ir[j], f, nf, y) == 0)
                            continue;
                        if (i < 0 || azel[1 + iu[j] * 2] >= azel[1 + iu[i] * 2])
                        {
                            i = j;
                        }
                    }
                    if (i < 0)
                        continue;

                    /* make double difference */
                    for (j = 0; j < ns; j++)
                    {
                        if (i == j)
                            continue;
                        sysi = rtk.ssat[sat[i] - 1].sys;
                        sysj = rtk.ssat[sat[j] - 1].sys;
                        if (GlobalMembersRtkpos.test_sys(sysj, m) == 0)
                            continue;
                        if (GlobalMembersRtkpos.validobs(iu[j], ir[j], f, nf, y) == 0)
                            continue;

                        ff = f % nf;
                        lami = nav.lam[sat[i] - 1, ff];
                        lamj = nav.lam[sat[j] - 1, ff];
                        if (lami <= 0.0 || lamj <= 0.0)
                            continue;
                        if (H != 0)
                        {
                            Hi = H + nv * rtk.nx;
                        }

                        /* double-differenced residual */
                        v[nv] = (y[f + iu[i] * nf * 2] - y[f + ir[i] * nf * 2]) - (y[f + iu[j] * nf * 2] - y[f + ir[j] * nf * 2]);

                        /* partial derivatives by rover position */
                        if (H != 0)
                        {
                            for (k = 0; k < 3; k++)
                            {
                                Hi[k] = -e[k + iu[i] * 3] + e[k + iu[j] * 3];
                            }
                        }
                        /* double-differenced ionospheric delay term */
                        if (opt.ionoopt == DefineConstants.IONOOPT_EST)
                        {
                            fi = lami / GlobalMembersRtkcmn.lam_carr[0];
                            fj = lamj / GlobalMembersRtkcmn.lam_carr[0];
                            didxi = (f < nf != 0 ? -1.0 : 1.0) * fi * fi * im[i];
                            didxj = (f < nf != 0 ? -1.0 : 1.0) * fj * fj * im[j];
                            v[nv] -= didxi * x[(((opt).dynamics == 0 ? 3 : 9) + (sat[i]) - 1)] - didxj * x[(((opt).dynamics == 0 ? 3 : 9) + (sat[j]) - 1)];
                            if (H != 0)
                            {
                                Hi[(((opt).dynamics == 0 ? 3 : 9) + (sat[i]) - 1)] = didxi;
                                Hi[(((opt).dynamics == 0 ? 3 : 9) + (sat[j]) - 1)] = -didxj;
                            }
                        }
                        /* double-differenced tropospheric delay term */
                        if (opt.tropopt == DefineConstants.TROPOPT_EST || opt.tropopt == DefineConstants.TROPOPT_ESTG)
                        {
                            v[nv] -= (tropu[i] - tropu[j]) - (tropr[i] - tropr[j]);
                            for (k = 0; k < (opt.tropopt < DefineConstants.TROPOPT_ESTG ? 1 : 3); k++)
                            {
                                if (H == 0)
                                    continue;
                                Hi[(((opt).dynamics == 0 ? 3 : 9) + ((opt).ionoopt != DefineConstants.IONOOPT_EST ? 0 : DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1) + ((opt).tropopt < DefineConstants.TROPOPT_EST ? 0 : ((opt).tropopt < DefineConstants.TROPOPT_ESTG ? 2 : 6)) / 2 * (0)) + k] = (dtdxu[k + i * 3] - dtdxu[k + j * 3]);
                                Hi[(((opt).dynamics == 0 ? 3 : 9) + ((opt).ionoopt != DefineConstants.IONOOPT_EST ? 0 : DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1) + ((opt).tropopt < DefineConstants.TROPOPT_EST ? 0 : ((opt).tropopt < DefineConstants.TROPOPT_ESTG ? 2 : 6)) / 2 * (1)) + k] = -(dtdxr[k + i * 3] - dtdxr[k + j * 3]);
                            }
                        }
                        /* double-differenced phase-bias term */
                        if (f < nf)
                        {
                            if (opt.ionoopt != DefineConstants.IONOOPT_IFLC)
                            {
                                v[nv] -= lami * x[((((opt).dynamics == 0 ? 3 : 9) + ((opt).ionoopt != DefineConstants.IONOOPT_EST ? 0 : DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1) + ((opt).tropopt < DefineConstants.TROPOPT_EST ? 0 : ((opt).tropopt < DefineConstants.TROPOPT_ESTG ? 2 : 6)) + ((opt).glomodear != 2 ? 0 : DefineConstants.NFREQGLO)) + DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1 * (f) + (sat[i]) - 1)] - lamj * x[((((opt).dynamics == 0 ? 3 : 9) + ((opt).ionoopt != DefineConstants.IONOOPT_EST ? 0 : DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1) + ((opt).tropopt < DefineConstants.TROPOPT_EST ? 0 : ((opt).tropopt < DefineConstants.TROPOPT_ESTG ? 2 : 6)) + ((opt).glomodear != 2 ? 0 : DefineConstants.NFREQGLO)) + DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1 * (f) + (sat[j]) - 1)];
                                if (H != 0)
                                {
                                    Hi[((((opt).dynamics == 0 ? 3 : 9) + ((opt).ionoopt != DefineConstants.IONOOPT_EST ? 0 : DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1) + ((opt).tropopt < DefineConstants.TROPOPT_EST ? 0 : ((opt).tropopt < DefineConstants.TROPOPT_ESTG ? 2 : 6)) + ((opt).glomodear != 2 ? 0 : DefineConstants.NFREQGLO)) + DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1 * (f) + (sat[i]) - 1)] = lami;
                                    Hi[((((opt).dynamics == 0 ? 3 : 9) + ((opt).ionoopt != DefineConstants.IONOOPT_EST ? 0 : DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1) + ((opt).tropopt < DefineConstants.TROPOPT_EST ? 0 : ((opt).tropopt < DefineConstants.TROPOPT_ESTG ? 2 : 6)) + ((opt).glomodear != 2 ? 0 : DefineConstants.NFREQGLO)) + DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1 * (f) + (sat[j]) - 1)] = -lamj;
                                }
                            }
                            else
                            {
                                v[nv] -= x[((((opt).dynamics == 0 ? 3 : 9) + ((opt).ionoopt != DefineConstants.IONOOPT_EST ? 0 : DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1) + ((opt).tropopt < DefineConstants.TROPOPT_EST ? 0 : ((opt).tropopt < DefineConstants.TROPOPT_ESTG ? 2 : 6)) + ((opt).glomodear != 2 ? 0 : DefineConstants.NFREQGLO)) + DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1 * (f) + (sat[i]) - 1)] - x[((((opt).dynamics == 0 ? 3 : 9) + ((opt).ionoopt != DefineConstants.IONOOPT_EST ? 0 : DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1) + ((opt).tropopt < DefineConstants.TROPOPT_EST ? 0 : ((opt).tropopt < DefineConstants.TROPOPT_ESTG ? 2 : 6)) + ((opt).glomodear != 2 ? 0 : DefineConstants.NFREQGLO)) + DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1 * (f) + (sat[j]) - 1)];
                                if (H != 0)
                                {
                                    Hi[((((opt).dynamics == 0 ? 3 : 9) + ((opt).ionoopt != DefineConstants.IONOOPT_EST ? 0 : DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1) + ((opt).tropopt < DefineConstants.TROPOPT_EST ? 0 : ((opt).tropopt < DefineConstants.TROPOPT_ESTG ? 2 : 6)) + ((opt).glomodear != 2 ? 0 : DefineConstants.NFREQGLO)) + DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1 * (f) + (sat[i]) - 1)] = 1.0;
                                    Hi[((((opt).dynamics == 0 ? 3 : 9) + ((opt).ionoopt != DefineConstants.IONOOPT_EST ? 0 : DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1) + ((opt).tropopt < DefineConstants.TROPOPT_EST ? 0 : ((opt).tropopt < DefineConstants.TROPOPT_ESTG ? 2 : 6)) + ((opt).glomodear != 2 ? 0 : DefineConstants.NFREQGLO)) + DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1 * (f) + (sat[j]) - 1)] = -1.0;
                                }
                            }
                        }
                        /* glonass receiver h/w bias term */
                        if (rtk.opt.glomodear == 2 && sysi == DefineConstants.SYS_GLO && sysj == DefineConstants.SYS_GLO && ff < DefineConstants.NFREQGLO)
                        {
                            df = (DefineConstants.CLIGHT / lami - DefineConstants.CLIGHT / lamj) / 1E6; // freq-difference (MHz)
                            v[nv] -= df * x[(((opt).dynamics == 0 ? 3 : 9) + ((opt).ionoopt != DefineConstants.IONOOPT_EST ? 0 : DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1) + ((opt).tropopt < DefineConstants.TROPOPT_EST ? 0 : ((opt).tropopt < DefineConstants.TROPOPT_ESTG ? 2 : 6)) + (ff))];
                            if (H != 0)
                            {
                                Hi[(((opt).dynamics == 0 ? 3 : 9) + ((opt).ionoopt != DefineConstants.IONOOPT_EST ? 0 : DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1) + ((opt).tropopt < DefineConstants.TROPOPT_EST ? 0 : ((opt).tropopt < DefineConstants.TROPOPT_ESTG ? 2 : 6)) + (ff))] = df;
                            }
                        }
                        /* glonass interchannel bias correction */
                        else if (sysi == DefineConstants.SYS_GLO && sysj == DefineConstants.SYS_GLO)
                        {

                            v[nv] -= GlobalMembersRtkpos.gloicbcorr(sat[i], sat[j], rtk.opt, lami, lamj, f);
                        }
                        if (f < nf)
                        {
                            rtk.ssat[sat[j] - 1].resc[f] = v[nv];
                        }
                        else
                        {
                            rtk.ssat[sat[j] - 1].resp[f - nf] = v[nv];
                        }

                        /* test innovation */
                        if (opt.maxinno > 0.0 && Math.Abs(v[nv]) > opt.maxinno)
                        {
                            if (f < nf)
                            {
                                rtk.ssat[sat[i] - 1].rejc[f]++;
                                rtk.ssat[sat[j] - 1].rejc[f]++;
                            }
                            GlobalMembersRtkpos.errmsg(rtk, "outlier rejected (sat=%3d-%3d %s%d v=%.3f)\n", sat[i], sat[j], f < nf != 0 ? "L" : "P", f % nf + 1, v[nv]);
                            continue;
                        }
                        /* single-differenced measurement error variances */
                        Ri[nv] = GlobalMembersPntpos.varerr(sat[i], sysi, azel[1 + iu[i] * 2], bl, dt, f, opt);
                        Rj[nv] = GlobalMembersPntpos.varerr(sat[j], sysj, azel[1 + iu[j] * 2], bl, dt, f, opt);

                        /* set valid data flags */
                        if (opt.mode > DefineConstants.PMODE_DGPS)
                        {
                            if (f < nf)
                            {
                                rtk.ssat[sat[i] - 1].vsat[f] = rtk.ssat[sat[j] - 1].vsat[f] = 1;
                            }
                        }
                        else
                        {
                            rtk.ssat[sat[i] - 1].vsat[f - nf] = rtk.ssat[sat[j] - 1].vsat[f - nf] = 1;
                        }
                        GlobalMembersRtkcmn.trace(4, "sat=%3d-%3d %s%d v=%13.3f R=%8.6f %8.6f\n", sat[i], sat[j], f < nf != 0 ? "L" : "P", f % nf + 1, v[nv], Ri[nv], Rj[nv]);

                        vflg[nv++] = (sat[i] << 16) | (sat[j] << 8) | ((f < nf != 0 ? 0 : 1) << 4) | (f % nf);
                        nb[b]++;
                    }
#if false
	// /* restore single-differenced residuals assuming sum equal zero */
	//        if (f<nf) {
	//            for (j=0,s=0.0;j<MAXSAT;j++) s+=rtk->ssat[j].resc[f];
	//            s/=nb[b]+1;
	//            for (j=0;j<MAXSAT;j++) {
	//                if (j==sat[i]-1||rtk->ssat[j].resc[f]!=0.0) rtk->ssat[j].resc[f]-=s;
	//            }
	//        }
	//        else {
	//            for (j=0,s=0.0;j<MAXSAT;j++) s+=rtk->ssat[j].resp[f-nf];
	//            s/=nb[b]+1;
	//            for (j=0;j<MAXSAT;j++) {
	//                if (j==sat[i]-1||rtk->ssat[j].resp[f-nf]!=0.0)
	//                    rtk->ssat[j].resp[f-nf]-=s;
	//            }
	//        }
#endif
                    b++;
                }
            }
            /* end of system loop */

            /* baseline length constraint for moving baseline */
            if (opt.mode == DefineConstants.PMODE_MOVEB && GlobalMembersRtkpos.constbl(rtk, x, P, v, H, Ri, Rj, nv) != 0)
            {
                vflg[nv++] = 3 << 4;
                nb[b++]++;
            }
            if (H != 0)
            {
                GlobalMembersRtkcmn.trace(5, "H=\n");
                GlobalMembersRtkcmn.tracemat(5, H, rtk.nx, nv, 7, 4);
            }

            /* double-differenced measurement error covariance */
            GlobalMembersRtkpos.ddcov(nb, b, Ri, Rj, nv, R);

            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(Ri);
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(Rj);
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(im);
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(tropu);
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(tropr);
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(dtdxu);
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(dtdxr);

            return nv;
        }
        //C++ TO C# CONVERTER NOTE: This was formerly a static local variable declaration (not allowed in C#):
        private static obsd_t[] intpres_obsb = Arrays.InitializeWithDefaultInstances<obsd_t>(DefineConstants.MAXOBS);
        //C++ TO C# CONVERTER NOTE: This was formerly a static local variable declaration (not allowed in C#):
        private static double[] intpres_yb = new double[DefineConstants.MAXOBS * DefineConstants.NFREQ * 2];
        double[] rs = new double[DefineConstants.MAXOBS * 6];
        double[] dts = new double[DefineConstants.MAXOBS * 2];
        double[] @var = new double[DefineConstants.MAXOBS];
        //C++ TO C# CONVERTER NOTE: This was formerly a static local variable declaration (not allowed in C#):
        private static double[] intpres_e = new double[DefineConstants.MAXOBS * 3];
        double[] azel = new double[DefineConstants.MAXOBS * 2];
        //C++ TO C# CONVERTER NOTE: This was formerly a static local variable declaration (not allowed in C#):
        private static int intpres_nb = 0;
        int[] svh = new int[DefineConstants.MAXOBS * 2];
        /* time-interpolation of residuals (for post-mission) ------------------------*/
        internal static double intpres(gtime_t time, obsd_t[] obs, int n, nav_t nav, rtk_t rtk, ref double y)
        {
            //C++ TO C# CONVERTER NOTE: This static local variable declaration (not allowed in C#) has been moved just prior to the method:
            //	static obsd_t obsb[DefineConstants.MAXOBS];
            //C++ TO C# CONVERTER NOTE: This static local variable declaration (not allowed in C#) has been moved just prior to the method:
            //	static double yb[DefineConstants.MAXOBS *DefineConstants.NFREQ *2],rs[DefineConstants.MAXOBS *6],dts[DefineConstants.MAXOBS *2],@var[DefineConstants.MAXOBS];
            //C++ TO C# CONVERTER NOTE: This static local variable declaration (not allowed in C#) has been moved just prior to the method:
            //	static double e[DefineConstants.MAXOBS *3],azel[DefineConstants.MAXOBS *2];
            //C++ TO C# CONVERTER NOTE: This static local variable declaration (not allowed in C#) has been moved just prior to the method:
            //	static int nb=0,svh[DefineConstants.MAXOBS *2];
            prcopt_t opt = rtk.opt;
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: double tt=timediff(time,obs[0].time),ttb,*p,*q;
            double tt = GlobalMembersRtkcmn.timediff(new gtime_t(time), obs[0].time);
            double ttb;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: double *p;
            double p;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: double *q;
            double q;
            int i;
            int j;
            int k;
            int nf = ((opt).ionoopt == DefineConstants.IONOOPT_IFLC ? 1 : (opt).nf);

            GlobalMembersRtkcmn.trace(3, "intpres : n=%d tt=%.1f\n", n, tt);

            if (intpres_nb == 0 || Math.Abs(tt) < DefineConstants.DTTOL)
            {
                intpres_nb = n;
                for (i = 0; i < n; i++)
                {
                    intpres_obsb[i] = obs[i];
                }
                return tt;
            }
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: ttb=timediff(time,obsb[0].time);
            ttb = GlobalMembersRtkcmn.timediff(new gtime_t(time), intpres_obsb[0].time);
            if (Math.Abs(ttb) > opt.maxtdiff * 2.0 || ttb == tt)
            {
                return tt;
            }

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: satposs(time,obsb,nb,nav,opt->sateph,rs,dts,var,svh);
            GlobalMembersEphemeris.satposs(new gtime_t(time), intpres_obsb, intpres_nb, nav, opt.sateph, rs, dts, @var, svh);

            if (GlobalMembersRtkpos.zdres(1, intpres_obsb, intpres_nb, rs, dts, svh, nav, rtk.rb, opt, 1, intpres_yb, ref intpres_e, azel) == 0)
            {
                return tt;
            }
            for (i = 0; i < n; i++)
            {
                for (j = 0; j < intpres_nb; j++)
                {
                    if (intpres_obsb[j].sat == obs[i].sat)
                        break;
                }
                if (j >= intpres_nb)
                    continue;
                for (k = 0, p = y + i * nf * 2, q = intpres_yb + j * nf * 2; k < nf * 2; k++, p++, q++)
                {
                    if (p == 0.0 || q == 0.0)
                    {
                        p = 0.0;
                    }
                    else
                    {
                        p = (ttb * p - tt * q) / (ttb - tt);
                    }
                }
            }
            return Math.Abs(ttb) > Math.Abs(tt) ? ttb : tt;
        }
        /* single to double-difference transformation matrix (D') --------------------*/
        internal static int ddmat(rtk_t rtk, double[] D)
        {
            int i;
            int j;
            int k;
            int m;
            int f;
            int nb = 0;
            int nx = rtk.nx;
            int na = rtk.na;
            int nf = ((rtk.opt).ionoopt == DefineConstants.IONOOPT_IFLC ? 1 : (rtk.opt).nf);

            GlobalMembersRtkcmn.trace(3, "ddmat   :\n");

            for (i = 0; i < DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1; i++)
            {
                for (j = 0; j < DefineConstants.NFREQ; j++)
                {
                    rtk.ssat[i].fix[j] = 0;
                }
            }
            for (i = 0; i < na; i++)
            {
                D[i + i * nx] = 1.0;
            }

            for (m = 0; m < 4; m++) // m=0:gps/qzs/sbs,1:glo,2:gal,3:bds
            {

                if (m == 1 && rtk.opt.glomodear == 0)
                    continue;
                if (m == 3 && rtk.opt.bdsmodear == 0)
                    continue;

                for (f = 0, k = na; f < nf; f++, k += DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1)
                {

                    for (i = k; i < k + DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1; i++)
                    {
                        if (rtk.x[i] == 0.0 || GlobalMembersRtkpos.test_sys(rtk.ssat[i - k].sys, m) == 0 || rtk.ssat[i - k].vsat[f] == 0)
                        {
                            continue;
                        }
                        if (rtk.ssat[i - k].@lock[f] > 0 && !(rtk.ssat[i - k].slip[f] & 2) && rtk.ssat[i - k].azel[1] >= rtk.opt.elmaskar)
                        {
                            rtk.ssat[i - k].fix[f] = 2; // fix
                            break;
                        }
                        else
                        {
                            rtk.ssat[i - k].fix[f] = 1;
                        }
                    }
                    for (j = k; j < k + DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1; j++)
                    {
                        if (i == j || rtk.x[j] == 0.0 || GlobalMembersRtkpos.test_sys(rtk.ssat[j - k].sys, m) == 0 || rtk.ssat[j - k].vsat[f] == 0)
                        {
                            continue;
                        }
                        if (rtk.ssat[j - k].@lock[f] > 0 && !(rtk.ssat[j - k].slip[f] & 2) && rtk.ssat[i - k].vsat[f] != 0 && rtk.ssat[j - k].azel[1] >= rtk.opt.elmaskar)
                        {
                            D[i + (na + nb) * nx] = 1.0;
                            D[j + (na + nb) * nx] = -1.0;
                            nb++;
                            rtk.ssat[j - k].fix[f] = 2; // fix
                        }
                        else
                        {
                            rtk.ssat[j - k].fix[f] = 1;
                        }
                    }
                }
            }
            GlobalMembersRtkcmn.trace(5, "D=\n");
            GlobalMembersRtkcmn.tracemat(5, D, nx, na + nb, 2, 0);
            return nb;
        }
        /* restore single-differenced ambiguity --------------------------------------*/
        internal static void restamb(rtk_t rtk, double[] bias, int nb, double[] xa)
        {
            int i;
            int n;
            int m;
            int f;
            int[] index = new int[DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1];
            int nv = 0;
            int nf = ((rtk.opt).ionoopt == DefineConstants.IONOOPT_IFLC ? 1 : (rtk.opt).nf);

            GlobalMembersRtkcmn.trace(3, "restamb :\n");

            for (i = 0; i < rtk.nx; i++)
            {
                xa[i] = rtk.x[i];
            }
            for (i = 0; i < rtk.na; i++)
            {
                xa[i] = rtk.xa[i];
            }

            for (m = 0; m < 4; m++)
            {
                for (f = 0; f < nf; f++)
                {

                    for (n = i = 0; i < DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1; i++)
                    {
                        if (GlobalMembersRtkpos.test_sys(rtk.ssat[i].sys, m) == 0 || rtk.ssat[i].fix[f] != 2)
                        {
                            continue;
                        }
                        index[n++] = ((((rtk.opt).dynamics == 0 ? 3 : 9) + ((rtk.opt).ionoopt != DefineConstants.IONOOPT_EST ? 0 : DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1) + ((rtk.opt).tropopt < DefineConstants.TROPOPT_EST ? 0 : ((rtk.opt).tropopt < DefineConstants.TROPOPT_ESTG ? 2 : 6)) + ((rtk.opt).glomodear != 2 ? 0 : DefineConstants.NFREQGLO)) + DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1 * (f) + (i + 1) - 1);
                    }
                    if (n < 2)
                        continue;

                    xa[index[0]] = rtk.x[index[0]];

                    for (i = 1; i < n; i++)
                    {
                        xa[index[i]] = xa[index[0]] - bias[nv++];
                    }
                }
            }
        }
        /* hold integer ambiguity ----------------------------------------------------*/
        internal static void holdamb(rtk_t rtk, double[] xa)
        {
            double[] v;
            double[] H;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: double *R;
            double R;
            int i;
            int n;
            int m;
            int f;
            int info;
            int[] index = new int[DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1];
            int nb = rtk.nx - rtk.na;
            int nv = 0;
            int nf = ((rtk.opt).ionoopt == DefineConstants.IONOOPT_IFLC ? 1 : (rtk.opt).nf);

            GlobalMembersRtkcmn.trace(3, "holdamb :\n");

            v = GlobalMembersRtkcmn.mat(nb, 1);
            H = GlobalMembersRtkcmn.zeros(nb, rtk.nx);

            for (m = 0; m < 4; m++)
            {
                for (f = 0; f < nf; f++)
                {

                    for (n = i = 0; i < DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1; i++)
                    {
                        if (GlobalMembersRtkpos.test_sys(rtk.ssat[i].sys, m) == 0 || rtk.ssat[i].fix[f] != 2 || rtk.ssat[i].azel[1] < rtk.opt.elmaskhold)
                        {
                            continue;
                        }
                        index[n++] = ((((rtk.opt).dynamics == 0 ? 3 : 9) + ((rtk.opt).ionoopt != DefineConstants.IONOOPT_EST ? 0 : DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1) + ((rtk.opt).tropopt < DefineConstants.TROPOPT_EST ? 0 : ((rtk.opt).tropopt < DefineConstants.TROPOPT_ESTG ? 2 : 6)) + ((rtk.opt).glomodear != 2 ? 0 : DefineConstants.NFREQGLO)) + DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1 * (f) + (i + 1) - 1);
                        rtk.ssat[i].fix[f] = 3; // hold
                    }
                    /* constraint to fixed ambiguity */
                    for (i = 1; i < n; i++)
                    {
                        v[nv] = (xa[index[0]] - xa[index[i]]) - (rtk.x[index[0]] - rtk.x[index[i]]);

                        H[index[0] + nv * rtk.nx] = 1.0;
                        H[index[i] + nv * rtk.nx] = -1.0;
                        nv++;
                    }
                }
            }
            if (nv > 0)
            {
                R = GlobalMembersRtkcmn.zeros(nv, nv);
                for (i = 0; i < nv; i++)
                {
                    R[i + i * nv] = DefineConstants.VAR_HOLDAMB;
                }

                /* update states with constraints */
                if ((info = GlobalMembersRtkcmn.filter(rtk.x, rtk.P, H, v, R, rtk.nx, nv)) != 0)
                {
                    GlobalMembersRtkpos.errmsg(rtk, "filter error (info=%d)\n", info);
                }
                //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
                free(R);
            }
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(v);
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(H);
        }
        /* resolve integer ambiguity by LAMBDA ---------------------------------------*/
        internal static int resamb_LAMBDA(rtk_t rtk, double[] bias, double[] xa)
        {
            prcopt_t opt = rtk.opt;
            int i;
            int j;
            int ny;
            int nb;
            int info;
            int nx = rtk.nx;
            int na = rtk.na;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: double *D,*DP,*y,*Qy,*b,*db,*Qb,*Qab,*QQ,s[2];
            double D;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: double *DP;
            double DP;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: double *y;
            double y;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: double *Qy;
            double Qy;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: double *b;
            double b;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: double *db;
            double db;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: double *Qb;
            double Qb;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: double *Qab;
            double Qab;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: double *QQ;
            double QQ;
            double[] s = new double[2];

            GlobalMembersRtkcmn.trace(3, "resamb_LAMBDA : nx=%d\n", nx);

            rtk.sol.ratio = 0.0F;

            if (rtk.opt.mode <= DefineConstants.PMODE_DGPS || rtk.opt.modear == DefineConstants.ARMODE_OFF || rtk.opt.thresar[0] < 1.0)
            {
                return 0;
            }
            /* single to double-difference transformation matrix (D') */
            D = GlobalMembersRtkcmn.zeros(nx, nx);
            if ((nb = GlobalMembersRtkpos.ddmat(rtk, D)) <= 0)
            {
                GlobalMembersRtkpos.errmsg(rtk, "no valid double-difference\n");
                //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
                free(D);
                return 0;
            }
            ny = na + nb;
            y = GlobalMembersRtkcmn.mat(ny, 1);
            Qy = GlobalMembersRtkcmn.mat(ny, ny);
            DP = GlobalMembersRtkcmn.mat(ny, nx);
            b = GlobalMembersRtkcmn.mat(nb, 2);
            db = GlobalMembersRtkcmn.mat(nb, 1);
            Qb = GlobalMembersRtkcmn.mat(nb, nb);
            Qab = GlobalMembersRtkcmn.mat(na, nb);
            QQ = GlobalMembersRtkcmn.mat(na, nb);

            /* transform single to double-differenced phase-bias (y=D'*x, Qy=D'*P*D) */
            GlobalMembersRtkcmn.matmul("TN", ny, 1, nx, 1.0, D, rtk.x, 0.0, ref y);
            GlobalMembersRtkcmn.matmul("TN", ny, nx, nx, 1.0, D, rtk.P, 0.0, ref DP);
            GlobalMembersRtkcmn.matmul("NN", ny, ny, nx, 1.0, DP, D, 0.0, ref Qy);

            /* phase-bias covariance (Qb) and real-parameters to bias covariance (Qab) */
            for (i = 0; i < nb; i++)
            {
                for (j = 0; j < nb; j++)
                {
                    Qb[i + j * nb] = Qy[na + i + (na + j) * ny];
                }
            }
            for (i = 0; i < na; i++)
            {
                for (j = 0; j < nb; j++)
                {
                    Qab[i + j * na] = Qy[i + (na + j) * ny];
                }
            }

            GlobalMembersRtkcmn.trace(4, "N(0)=");
            GlobalMembersRtkcmn.tracemat(4, y + na, 1, nb, 10, 3);

            /* lambda/mlambda integer least-square estimation */
            if ((info = GlobalMembersLambda.lambda(nb, 2, y + na, Qb, ref b, ref s)) == 0)
            {

                GlobalMembersRtkcmn.trace(4, "N(1)=");
                GlobalMembersRtkcmn.tracemat(4, b, 1, nb, 10, 3);
                GlobalMembersRtkcmn.trace(4, "N(2)=");
                GlobalMembersRtkcmn.tracemat(4, b + nb, 1, nb, 10, 3);

                rtk.sol.ratio = s[0] > 0 ? (float)(s[1] / s[0]) : 0.0f;
                if (rtk.sol.ratio > 999.9)
                {
                    rtk.sol.ratio = 999.9f;
                }

                /* validation by popular ratio-test */
                if (s[0] <= 0.0 || s[1] / s[0] >= opt.thresar[0])
                {

                    /* transform float to fixed solution (xa=xa-Qab*Qb\(b0-b)) */
                    for (i = 0; i < na; i++)
                    {
                        rtk.xa[i] = rtk.x[i];
                        for (j = 0; j < na; j++)
                        {
                            rtk.Pa[i + j * na] = rtk.P[i + j * nx];
                        }
                    }
                    for (i = 0; i < nb; i++)
                    {
                        bias[i] = b[i];
                        y[na + i] -= b[i];
                    }
                    if (GlobalMembersRtkcmn.matinv(ref Qb, nb) == 0)
                    {
                        GlobalMembersRtkcmn.matmul("NN", nb, 1, nb, 1.0, Qb, y + na, 0.0, ref db);
                        GlobalMembersRtkcmn.matmul("NN", na, 1, nb, -1.0, Qab, db, 1.0, ref rtk.xa);

                        /* covariance of fixed solution (Qa=Qa-Qab*Qb^-1*Qab') */
                        GlobalMembersRtkcmn.matmul("NN", na, nb, nb, 1.0, Qab, Qb, 0.0, ref QQ);
                        GlobalMembersRtkcmn.matmul("NT", na, na, nb, -1.0, QQ, Qab, 1.0, ref rtk.Pa);

                        GlobalMembersRtkcmn.trace(3, "resamb : validation ok (nb=%d ratio=%.2f s=%.2f/%.2f)\n", nb, s[0] == 0.0 ? 0.0 : s[1] / s[0], s[0], s[1]);

                        /* restore single-differenced ambiguity */
                        GlobalMembersRtkpos.restamb(rtk, bias, nb, xa);
                    }
                    else
                    {
                        nb = 0;
                    }
                }
                else // validation failed
                {
                    GlobalMembersRtkpos.errmsg(rtk, "ambiguity validation failed (nb=%d ratio=%.2f s=%.2f/%.2f)\n", nb, s[1] / s[0], s[0], s[1]);
                    nb = 0;
                }
            }
            else
            {
                GlobalMembersRtkpos.errmsg(rtk, "lambda error (info=%d)\n", info);
            }
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(D);
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(y);
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(Qy);
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(DP);
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(b);
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(db);
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(Qb);
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(Qab);
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(QQ);

            return nb; // number of ambiguities
        }
        /* validation of solution ----------------------------------------------------*/
        internal static int valpos(rtk_t rtk, double[] v, double[] R, int[] vflg, int nv, double thres)
        {
#if false
	//    prcopt_t *opt=&rtk->opt;
	//    double vv=0.0;
#endif
            double fact = thres * thres;
            int i;
            int stat = 1;
            int sat1;
            int sat2;
            int type;
            int freq;
            string stype;

            GlobalMembersRtkcmn.trace(3, "valpos  : nv=%d thres=%.1f\n", nv, thres);

            /* post-fit residual test */
            for (i = 0; i < nv; i++)
            {
                if (v[i] * v[i] <= fact * R[i + i * nv])
                    continue;
                sat1 = (vflg[i] >> 16) & 0xFF;
                sat2 = (vflg[i] >> 8) & 0xFF;
                type = (vflg[i] >> 4) & 0xF;
                freq = vflg[i] & 0xF;
                stype = type == 0 ? "L" : (type == 1 ? "L" : "C");
                GlobalMembersRtkpos.errmsg(rtk, "large residual (sat=%2d-%2d %s%d v=%6.3f sig=%.3f)\n", sat1, sat2, stype, freq + 1, v[i], ((R[i + i * nv]) <= 0.0 ? 0.0 : Math.Sqrt(R[i + i * nv])));
            }
#if false
	//    if (stat&&nv>NP(opt)) {
	//        
	// /* chi-square validation */
	//        for (i=0;i<nv;i++) vv+=v[i]*v[i]/R[i+i*nv];
	//        
	//        if (vv>chisqr[nv-NP(opt)-1]) {
	//            errmsg(rtk,"residuals validation failed (nv=%d np=%d vv=%.2f cs=%.2f)\n",
	//                   nv,NP(opt),vv,chisqr[nv-NP(opt)-1]);
	//            stat=0;
	//        }
	//        else {
	//            trace(3,"valpos : validation ok (%s nv=%d np=%d vv=%.2f cs=%.2f)\n",
	//                  rtk->tstr,nv,NP(opt),vv,chisqr[nv-NP(opt)-1]);
	//        }
	//    }
#endif
            return stat;
        }
        /* relative positioning ------------------------------------------------------*/
        //C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on the parameter 'rtk', so pointers on this parameter are left unchanged:
        internal static int relpos(rtk_t* rtk, obsd_t[] obs, int nu, int nr, nav_t nav)
        {
            prcopt_t opt = rtk.opt;
            gtime_t time = obs[0].time;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: double *rs,*dts,*var,*y,*e,*azel,*v,*H,*R,*xp,*Pp,*xa,*bias,dt;
            double rs;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: double *dts;
            double dts;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: double *var;
            double @var;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: double *y;
            double y;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: double *e;
            double e;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: double *azel;
            double azel;
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
            //ORIGINAL LINE: double *xp;
            double xp;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: double *Pp;
            double Pp;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: double *xa;
            double xa;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: double *bias;
            double bias;
            double dt;
            int i;
            int j;
            int f;
            int n = nu + nr;
            int ns;
            int ny;
            int nv;
            int[] sat = new int[DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1];
            int[] iu = new int[DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1];
            int[] ir = new int[DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1];
            int niter;
            int info;
            int[] vflg = new int[DefineConstants.MAXOBS * DefineConstants.NFREQ * 2 + 1];
            int[] svh = new int[DefineConstants.MAXOBS * 2];
            int stat = rtk.opt.mode <= DefineConstants.PMODE_DGPS ? DefineConstants.SOLQ_DGPS : DefineConstants.SOLQ_FLOAT;
            int nf = opt.ionoopt == DefineConstants.IONOOPT_IFLC ? 1 : opt.nf;

            GlobalMembersRtkcmn.trace(3, "relpos  : nx=%d nu=%d nr=%d\n", rtk.nx, nu, nr);

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: dt=timediff(time,obs[nu].time);
            dt = GlobalMembersRtkcmn.timediff(new gtime_t(time), obs[nu].time);

            rs = GlobalMembersRtkcmn.mat(6, n);
            dts = GlobalMembersRtkcmn.mat(2, n);
            @var = GlobalMembersRtkcmn.mat(1, n);
            y = GlobalMembersRtkcmn.mat(nf * 2, n);
            e = GlobalMembersRtkcmn.mat(3, n);
            azel = GlobalMembersRtkcmn.zeros(2, n);

            for (i = 0; i < DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1; i++)
            {
                rtk.ssat[i].sys = GlobalMembersRtkcmn.satsys(i + 1, null);
                for (j = 0; j < DefineConstants.NFREQ; j++)
                {
                    rtk.ssat[i].vsat[j] = rtk.ssat[i].snr[j] = 0;
                }
            }
            /* satellite positions/clocks */
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: satposs(time,obs,n,nav,opt->sateph,rs,dts,var,svh);
            GlobalMembersEphemeris.satposs(new gtime_t(time), new obsd_t(obs), n, nav, opt.sateph, rs, dts, @var, svh);

            /* undifferenced residuals for base station */
            if (GlobalMembersRtkpos.zdres(1, obs + nu, nr, rs + nu * 6, dts + nu * 2, svh + nu, nav, rtk.rb, opt, 1, y + nu * nf * 2, ref e + nu * 3, azel + nu * 2) == 0)
            {
                GlobalMembersRtkpos.errmsg(rtk, "initial base station position error\n");

                //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
                free(rs);
                //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
                free(dts);
                //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
                free(@var);
                //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
                free(y);
                //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
                free(e);
                //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
                free(azel);
                return 0;
            }
            /* time-interpolation of residuals (for post-processing) */
            if (opt.intpref != 0)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: dt=intpres(time,obs+nu,nr,nav,rtk,y+nu *nf *2);
                dt = GlobalMembersRtkpos.intpres(new gtime_t(time), obs + nu, nr, nav, new rtk_t(rtk), ref y + nu * nf * 2);
            }
            /* select common satellites between rover and base-station */
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: if ((ns=selsat(obs,azel,nu,nr,opt,sat,iu,ir))<=0)
            if ((ns = GlobalMembersRtkpos.selsat(new obsd_t(obs), azel, nu, nr, opt, sat, iu, ir)) <= 0)
            {
                GlobalMembersRtkpos.errmsg(rtk, "no common satellite\n");

                //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
                free(rs);
                //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
                free(dts);
                //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
                free(@var);
                //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
                free(y);
                //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
                free(e);
                //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
                free(azel);
                return 0;
            }
            /* temporal update of states */
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: udstate(rtk,obs,sat,iu,ir,ns,nav);
            GlobalMembersRtkpos.udstate(new rtk_t(rtk), new obsd_t(obs), sat, iu, ir, ns, nav);

            GlobalMembersRtkcmn.trace(4, "x(0)=");
            GlobalMembersRtkcmn.tracemat(4, rtk.x, 1, (((opt).dynamics == 0 ? 3 : 9) + ((opt).ionoopt != DefineConstants.IONOOPT_EST ? 0 : DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1) + ((opt).tropopt < DefineConstants.TROPOPT_EST ? 0 : ((opt).tropopt < DefineConstants.TROPOPT_ESTG ? 2 : 6)) + ((opt).glomodear != 2 ? 0 : DefineConstants.NFREQGLO)), 13, 4);

            xp = GlobalMembersRtkcmn.mat(rtk.nx, 1);
            Pp = GlobalMembersRtkcmn.zeros(rtk.nx, rtk.nx);
            xa = GlobalMembersRtkcmn.mat(rtk.nx, 1);
            GlobalMembersRtkcmn.matcpy(ref xp, rtk.x, rtk.nx, 1);

            ny = ns * nf * 2 + 2;
            v = GlobalMembersRtkcmn.mat(ny, 1);
            H = GlobalMembersRtkcmn.zeros(rtk.nx, ny);
            R = GlobalMembersRtkcmn.mat(ny, ny);
            bias = GlobalMembersRtkcmn.mat(rtk.nx, 1);

            /* add 2 iterations for baseline-constraint moving-base */
            niter = opt.niter + (opt.mode == DefineConstants.PMODE_MOVEB && opt.baseline[0] > 0.0 ? 2 : 0);

            for (i = 0; i < niter; i++)
            {
                /* undifferenced residuals for rover */
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: if (!zdres(0,obs,nu,rs,dts,svh,nav,xp,opt,0,y,e,azel))
                if (GlobalMembersRtkpos.zdres(0, new obsd_t(obs), nu, rs, dts, svh, nav, xp, opt, 0, y, ref e, azel) == 0)
                {
                    GlobalMembersRtkpos.errmsg(rtk, "rover initial position error\n");
                    stat = DefineConstants.SOLQ_NONE;
                    break;
                }
                /* double-differenced residuals and partial derivatives */
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: if ((nv=ddres(rtk,nav,dt,xp,Pp,sat,y,e,azel,iu,ir,ns,v,H,R,vflg))<1)
                if ((nv = GlobalMembersRtkpos.ddres(new rtk_t(rtk), nav, dt, xp, Pp, sat, y, e, azel, iu, ir, ns, v, ref H, ref R, vflg)) < 1)
                {
                    GlobalMembersRtkpos.errmsg(rtk, "no double-differenced residual\n");
                    stat = DefineConstants.SOLQ_NONE;
                    break;
                }
                /* kalman filter measurement update */
                GlobalMembersRtkcmn.matcpy(ref Pp, rtk.P, rtk.nx, rtk.nx);
                if ((info = GlobalMembersRtkcmn.filter(xp, Pp, H, v, R, rtk.nx, nv)) != 0)
                {
                    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                    //ORIGINAL LINE: errmsg(rtk,"filter error (info=%d)\n",info);
                    GlobalMembersRtkpos.errmsg(new rtk_t(rtk), "filter error (info=%d)\n", info);
                    stat = DefineConstants.SOLQ_NONE;
                    break;
                }
                GlobalMembersRtkcmn.trace(4, "x(%d)=", i + 1);
                GlobalMembersRtkcmn.tracemat(4, xp, 1, (((opt).dynamics == 0 ? 3 : 9) + ((opt).ionoopt != DefineConstants.IONOOPT_EST ? 0 : DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1) + ((opt).tropopt < DefineConstants.TROPOPT_EST ? 0 : ((opt).tropopt < DefineConstants.TROPOPT_ESTG ? 2 : 6)) + ((opt).glomodear != 2 ? 0 : DefineConstants.NFREQGLO)), 13, 4);
            }
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: if (stat!=DefineConstants.SOLQ_NONE&&zdres(0,obs,nu,rs,dts,svh,nav,xp,opt,0,y,e,azel))
            if (stat != DefineConstants.SOLQ_NONE && GlobalMembersRtkpos.zdres(0, new obsd_t(obs), nu, rs, dts, svh, nav, xp, opt, 0, y, ref e, azel) != 0)
            {

                /* post-fit residuals for float solution */
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: nv=ddres(rtk,nav,dt,xp,Pp,sat,y,e,azel,iu,ir,ns,v,null,R,vflg);
                nv = GlobalMembersRtkpos.ddres(new rtk_t(rtk), nav, dt, xp, Pp, sat, y, e, azel, iu, ir, ns, v, null, ref R, vflg);

                /* validation of float solution */
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: if (valpos(rtk,v,R,vflg,nv,4.0))
                if (GlobalMembersRtkpos.valpos(new rtk_t(rtk), v, R, vflg, nv, 4.0) != 0)
                {

                    /* update state and covariance matrix */
                    GlobalMembersRtkcmn.matcpy(ref rtk.x, xp, rtk.nx, 1);
                    GlobalMembersRtkcmn.matcpy(ref rtk.P, Pp, rtk.nx, rtk.nx);

                    /* update ambiguity control struct */
                    rtk.sol.ns = 0;
                    for (i = 0; i < ns; i++)
                    {
                        for (f = 0; f < nf; f++)
                        {
                            if (rtk.ssat[sat[i] - 1].vsat[f] == 0)
                                continue;
                            rtk.ssat[sat[i] - 1].@lock[f]++;
                            rtk.ssat[sat[i] - 1].outc[f] = 0;
                            if (f == 0) // valid satellite count by L1
                            {
                                rtk.sol.ns++;
                            }
                        }
                    }
                    /* lack of valid satellites */
                    if (rtk.sol.ns < 4)
                    {
                        stat = DefineConstants.SOLQ_NONE;
                    }
                }
                else
                {
                    stat = DefineConstants.SOLQ_NONE;
                }
            }
            /* resolve integer ambiguity by WL-NL */
            if (stat != DefineConstants.SOLQ_NONE && rtk.opt.modear == DefineConstants.ARMODE_WLNL)
            {

                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: if (resamb_WLNL(rtk,obs,sat,iu,ir,ns,nav,azel))
                if (GlobalMembersRtkpos.resamb_WLNL(new rtk_t(rtk), new obsd_t(obs), sat, iu, ir, ns, nav, azel) != 0)
                {
                    stat = DefineConstants.SOLQ_FIX;
                }
            }
            /* resolve integer ambiguity by TCAR */
            else if (stat != DefineConstants.SOLQ_NONE && rtk.opt.modear == DefineConstants.ARMODE_TCAR)
            {

                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: if (resamb_TCAR(rtk,obs,sat,iu,ir,ns,nav,azel))
                if (GlobalMembersRtkpos.resamb_TCAR(new rtk_t(rtk), new obsd_t(obs), sat, iu, ir, ns, nav, azel) != 0)
                {
                    stat = DefineConstants.SOLQ_FIX;
                }
            }
            /* resolve integer ambiguity by LAMBDA */
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: else if (stat!=DefineConstants.SOLQ_NONE&&resamb_LAMBDA(rtk,bias,xa)>1)
            else if (stat != DefineConstants.SOLQ_NONE && GlobalMembersRtkpos.resamb_LAMBDA(new rtk_t(rtk), bias, xa) > 1)
            {

                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: if (zdres(0,obs,nu,rs,dts,svh,nav,xa,opt,0,y,e,azel))
                if (GlobalMembersRtkpos.zdres(0, new obsd_t(obs), nu, rs, dts, svh, nav, xa, opt, 0, y, ref e, azel) != 0)
                {

                    /* post-fit reisiduals for fixed solution */
                    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                    //ORIGINAL LINE: nv=ddres(rtk,nav,dt,xa,null,sat,y,e,azel,iu,ir,ns,v,null,R,vflg);
                    nv = GlobalMembersRtkpos.ddres(new rtk_t(rtk), nav, dt, xa, null, sat, y, e, azel, iu, ir, ns, v, null, ref R, vflg);

                    /* validation of fixed solution */
                    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                    //ORIGINAL LINE: if (valpos(rtk,v,R,vflg,nv,4.0))
                    if (GlobalMembersRtkpos.valpos(new rtk_t(rtk), v, R, vflg, nv, 4.0) != 0)
                    {

                        /* hold integer ambiguity */
                        if (++rtk.nfix >= rtk.opt.minfix && rtk.opt.modear == DefineConstants.ARMODE_FIXHOLD)
                        {
                            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                            //ORIGINAL LINE: holdamb(rtk,xa);
                            GlobalMembersRtkpos.holdamb(new rtk_t(rtk), xa);
                        }
                        stat = DefineConstants.SOLQ_FIX;
                    }
                }
            }
            /* save solution status */
            if (stat == DefineConstants.SOLQ_FIX)
            {
                for (i = 0; i < 3; i++)
                {
                    rtk.sol.rr[i] = rtk.xa[i];
                    rtk.sol.qr[i] = (float)rtk.Pa[i + i * rtk.na];
                }
                rtk.sol.qr[3] = (float)rtk.Pa[1];
                rtk.sol.qr[4] = (float)rtk.Pa[1 + 2 * rtk.na];
                rtk.sol.qr[5] = (float)rtk.Pa[2];
            }
            else
            {
                for (i = 0; i < 3; i++)
                {
                    rtk.sol.rr[i] = rtk.x[i];
                    rtk.sol.qr[i] = (float)rtk.P[i + i * rtk.nx];
                }
                rtk.sol.qr[3] = (float)rtk.P[1];
                rtk.sol.qr[4] = (float)rtk.P[1 + 2 * rtk.nx];
                rtk.sol.qr[5] = (float)rtk.P[2];
                rtk.nfix = 0;
            }
            for (i = 0; i < n; i++)
            {
                for (j = 0; j < nf; j++)
                {
                    if (obs[i].L[j] == 0.0)
                        continue;
                    rtk.ssat[obs[i].sat - 1].pt[obs[i].rcv - 1, j] = obs[i].time;
                    rtk.ssat[obs[i].sat - 1].ph[obs[i].rcv - 1, j] = obs[i].L[j];
                }
            }
            for (i = 0; i < ns; i++)
            {
                for (j = 0; j < nf; j++)
                {

                    /* output snr of rover receiver */
                    rtk.ssat[sat[i] - 1].snr[j] = obs[iu[i]].SNR[j];
                }
            }
            for (i = 0; i < DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1; i++)
            {
                for (j = 0; j < nf; j++)
                {
                    if (rtk.ssat[i].fix[j] == 2 && stat != DefineConstants.SOLQ_FIX)
                    {
                        rtk.ssat[i].fix[j] = 1;
                    }
                    if (rtk.ssat[i].slip[j] & 1)
                    {
                        rtk.ssat[i].slipc[j]++;
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
            free(y);
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(e);
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(azel);
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(xp);
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(Pp);
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(xa);
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(v);
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(H);
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(R);
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(bias);

            if (stat != DefineConstants.SOLQ_NONE)
            {
                rtk.sol.stat = stat;
            }

            return stat != DefineConstants.SOLQ_NONE;
        }
        /* initialize rtk control ------------------------------------------------------
        * initialize rtk control struct
        * args   : rtk_t    *rtk    IO  rtk control/result struct
        *          prcopt_t *opt    I   positioning options (see rtklib.h)
        * return : none
        *-----------------------------------------------------------------------------*/
        public static void rtkinit(rtk_t rtk, prcopt_t opt)
        {
            sol_t sol0 = new sol_t({ 0 });
            ambc_t ambc0 = new ambc_t({ { 0 } });
            ssat_t ssat0 = new ssat_t();
            int i;

            GlobalMembersRtkcmn.trace(3, "rtkinit :\n");

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: rtk->sol=sol0;
            rtk.sol.CopyFrom(sol0);
            for (i = 0; i < 6; i++)
            {
                rtk.rb[i] = 0.0;
            }
            rtk.nx = opt.mode <= DefineConstants.PMODE_FIXED ? ((((opt).dynamics == 0 ? 3 : 9) + ((opt).ionoopt != DefineConstants.IONOOPT_EST ? 0 : DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1) + ((opt).tropopt < DefineConstants.TROPOPT_EST ? 0 : ((opt).tropopt < DefineConstants.TROPOPT_ESTG ? 2 : 6)) + ((opt).glomodear != 2 ? 0 : DefineConstants.NFREQGLO)) + ((opt).mode <= DefineConstants.PMODE_DGPS ? 0 : DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1 * ((opt).ionoopt == DefineConstants.IONOOPT_IFLC ? 1 : (opt).nf))) : GlobalMembersPpp.pppnx(opt);
            rtk.na = opt.mode <= DefineConstants.PMODE_FIXED ? (((opt).dynamics == 0 ? 3 : 9) + ((opt).ionoopt != DefineConstants.IONOOPT_EST ? 0 : DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1) + ((opt).tropopt < DefineConstants.TROPOPT_EST ? 0 : ((opt).tropopt < DefineConstants.TROPOPT_ESTG ? 2 : 6)) + ((opt).glomodear != 2 ? 0 : DefineConstants.NFREQGLO)) : 0;
            rtk.tt = 0.0;
            rtk.x = GlobalMembersRtkcmn.zeros(rtk.nx, 1);
            rtk.P = GlobalMembersRtkcmn.zeros(rtk.nx, rtk.nx);
            rtk.xa = GlobalMembersRtkcmn.zeros(rtk.na, 1);
            rtk.Pa = GlobalMembersRtkcmn.zeros(rtk.na, rtk.na);
            rtk.nfix = rtk.neb = 0;
            for (i = 0; i < DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1; i++)
            {
                rtk.ambc[i] = ambc0;
                rtk.ssat[i] = ssat0;
            }
            for (i = 0; i < DefineConstants.MAXERRMSG; i++)
            {
                rtk.errbuf = rtk.errbuf.Substring(0, i);
            }
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: rtk->opt=*opt;
            rtk.opt.CopyFrom(opt);
        }
        /* free rtk control ------------------------------------------------------------
        * free memory for rtk control struct
        * args   : rtk_t    *rtk    IO  rtk control/result struct
        * return : none
        *-----------------------------------------------------------------------------*/
        public static void rtkfree(rtk_t rtk)
        {
            GlobalMembersRtkcmn.trace(3, "rtkfree :\n");

            rtk.nx = rtk.na = 0;
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(rtk.x);
            rtk.x = null;
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(rtk.P);
            rtk.P = null;
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(rtk.xa);
            rtk.xa = null;
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(rtk.Pa);
            rtk.Pa = null;
        }
        /* precise positioning ---------------------------------------------------------
        * input observation data and navigation message, compute rover position by 
        * precise positioning
        * args   : rtk_t *rtk       IO  rtk control/result struct
        *            rtk->sol       IO  solution
        *                .time      O   solution time
        *                .rr[]      IO  rover position/velocity
        *                               (I:fixed mode,O:single mode)
        *                .dtr[0]    O   receiver clock bias (s)
        *                .dtr[1]    O   receiver glonass-gps time offset (s)
        *                .Qr[]      O   rover position covarinace
        *                .stat      O   solution status (SOLQ_???)
        *                .ns        O   number of valid satellites
        *                .age       O   age of differential (s)
        *                .ratio     O   ratio factor for ambiguity validation
        *            rtk->rb[]      IO  base station position/velocity
        *                               (I:relative mode,O:moving-base mode)
        *            rtk->nx        I   number of all states
        *            rtk->na        I   number of integer states
        *            rtk->ns        O   number of valid satellite
        *            rtk->tt        O   time difference between current and previous (s)
        *            rtk->x[]       IO  float states pre-filter and post-filter
        *            rtk->P[]       IO  float covariance pre-filter and post-filter
        *            rtk->xa[]      O   fixed states after AR
        *            rtk->Pa[]      O   fixed covariance after AR
        *            rtk->ssat[s]   IO  sat(s+1) status
        *                .sys       O   system (SYS_???)
        *                .az   [r]  O   azimuth angle   (rad) (r=0:rover,1:base)
        *                .el   [r]  O   elevation angle (rad) (r=0:rover,1:base)
        *                .vs   [r]  O   data valid single     (r=0:rover,1:base)
        *                .resp [f]  O   freq(f+1) pseudorange residual (m)
        *                .resc [f]  O   freq(f+1) carrier-phase residual (m)
        *                .vsat [f]  O   freq(f+1) data vaild (0:invalid,1:valid)
        *                .fix  [f]  O   freq(f+1) ambiguity flag
        *                               (0:nodata,1:float,2:fix,3:hold)
        *                .slip [f]  O   freq(f+1) slip flag
        *                               (bit8-7:rcv1 LLI, bit6-5:rcv2 LLI,
        *                                bit2:parity unknown, bit1:slip)
        *                .lock [f]  IO  freq(f+1) carrier lock count
        *                .outc [f]  IO  freq(f+1) carrier outage count
        *                .slipc[f]  IO  freq(f+1) cycle slip count
        *                .rejc [f]  IO  freq(f+1) data reject count
        *                .gf        IO  geometry-free phase (L1-L2) (m)
        *                .gf2       IO  geometry-free phase (L1-L5) (m)
        *            rtk->nfix      IO  number of continuous fixes of ambiguity
        *            rtk->neb       IO  bytes of error message buffer
        *            rtk->errbuf    IO  error message buffer
        *            rtk->tstr      O   time string for debug
        *            rtk->opt       I   processing options
        *          obsd_t *obs      I   observation data for an epoch
        *                               obs[i].rcv=1:rover,2:reference
        *                               sorted by receiver and satellte
        *          int    n         I   number of observation data
        *          nav_t  *nav      I   navigation messages
        * return : status (0:no solution,1:valid solution)
        * notes  : before calling function, base station position rtk->sol.rb[] should
        *          be properly set for relative mode except for moving-baseline
        *-----------------------------------------------------------------------------*/
        public static int rtkpos(rtk_t rtk, obsd_t[] obs, int n, nav_t nav)
        {
            prcopt_t opt = rtk.opt;
            sol_t solb = new sol_t({ 0 });
            gtime_t time = new gtime_t();
            int i;
            int nu;
            int nr;
            string msg = "";

            GlobalMembersRtkcmn.trace(3, "rtkpos  : time=%s n=%d\n", GlobalMembersRtkcmn.time_str(obs[0].time, 3), n);
            GlobalMembersRtkcmn.trace(4, "obs=\n");
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: traceobs(4,obs,n);
            GlobalMembersRtkcmn.traceobs(4, new obsd_t(obs), n);
            /*trace(5,"nav=\n"); tracenav(5,nav);*/

            /* set base staion position */
            if (opt.refpos <= 3 && opt.mode != DefineConstants.PMODE_SINGLE && opt.mode != DefineConstants.PMODE_MOVEB)
            {
                for (i = 0; i < 6; i++)
                {
                    rtk.rb[i] = i < 3 ? opt.rb[i] : 0.0;
                }
            }
            /* count rover/base station observations */
            for (nu = 0; nu < n && obs[nu].rcv == 1; nu++)
            {
                ;
            }
            for (nr = 0; nu + nr < n && obs[nu + nr].rcv == 2; nr++)
            {
                ;
            }

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: time=rtk->sol.time;
            time.CopyFrom(rtk.sol.time); // previous epoch

            /* rover position by single point positioning */
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: if (!pntpos(obs,nu,nav,&rtk->opt,&rtk->sol,null,rtk->ssat,msg))
            if (GlobalMembersPntpos.pntpos(new obsd_t(obs), nu, nav, rtk.opt, rtk.sol, null, new ssat_t(rtk.ssat), ref msg) == 0)
            {
                GlobalMembersRtkpos.errmsg(rtk, "point pos error (%s)\n", msg);

                if (rtk.opt.dynamics == 0)
                {
                    GlobalMembersRtkpos.outsolstat(rtk);
                    return 0;
                }
            }
            if (time.time != 0)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: rtk->tt=timediff(rtk->sol.time,time);
                rtk.tt = GlobalMembersRtkcmn.timediff(new gtime_t(rtk.sol.time), new gtime_t(time));
            }

            /* single point positioning */
            if (opt.mode == DefineConstants.PMODE_SINGLE)
            {
                GlobalMembersRtkpos.outsolstat(rtk);
                return 1;
            }
            /* precise point positioning */
            if (opt.mode >= DefineConstants.PMODE_PPP_KINEMA)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: pppos(rtk,obs,nu,nav);
                GlobalMembersPpp.pppos(rtk, new obsd_t(obs), nu, nav);
                GlobalMembersPpp.pppoutsolstat(rtk, statlevel, fp_stat);
                return 1;
            }
            /* check number of data of base station and age of differential */
            if (nr == 0)
            {
                GlobalMembersRtkpos.errmsg(rtk, "no base station observation data for rtk\n");
                GlobalMembersRtkpos.outsolstat(rtk);
                return 1;
            }
            if (opt.mode == DefineConstants.PMODE_MOVEB) //  moving baseline
            {

                /* estimate position/velocity of base station */
                if (GlobalMembersPntpos.pntpos(obs + nu, nr, nav, rtk.opt, solb, null, null, ref msg) == 0)
                {
                    GlobalMembersRtkpos.errmsg(rtk, "base station position error (%s)\n", msg);
                    return 0;
                }
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: rtk->sol.age=(float)timediff(rtk->sol.time,solb.time);
                rtk.sol.age = (float)GlobalMembersRtkcmn.timediff(new gtime_t(rtk.sol.time), new gtime_t(solb.time));

                if (Math.Abs(rtk.sol.age) > 1.0 + 2 * DefineConstants.DTTOL)
                {
                    GlobalMembersRtkpos.errmsg(rtk, "time sync error for moving-base (age=%.1f)\n", rtk.sol.age);
                    return 0;
                }
                for (i = 0; i < 6; i++)
                {
                    rtk.rb[i] = solb.rr[i];
                }

                /* time-synchronized position of base station */
                for (i = 0; i < 3; i++)
                {
                    rtk.rb[i] += rtk.rb[i + 3] * rtk.sol.age;
                }
            }
            else
            {
                rtk.sol.age = (float)GlobalMembersRtkcmn.timediff(obs[0].time, obs[nu].time);

                if (Math.Abs(rtk.sol.age) > opt.maxtdiff)
                {
                    GlobalMembersRtkpos.errmsg(rtk, "age of differential error (age=%.1f)\n", rtk.sol.age);
                    GlobalMembersRtkpos.outsolstat(rtk);
                    return 1;
                }
            }
            /* relative potitioning */
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: relpos(rtk,obs,nu,nr,nav);
            GlobalMembersRtkpos.relpos(rtk, new obsd_t(obs), nu, nr, nav);
            GlobalMembersRtkpos.outsolstat(rtk);

            return 1;
        }
    }
}
