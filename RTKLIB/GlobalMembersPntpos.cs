using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ghGPS.Classes
{
    public static class GlobalMembersPntpos
    {
        /*------------------------------------------------------------------------------
        * pntpos.c : standard positioning
        *
        *          Copyright (C) 2007-2015 by T.TAKASU, All rights reserved.
        *
        * version : $Revision:$ $Date:$
        * history : 2010/07/28 1.0  moved from rtkcmn.c
        *                           changed api:
        *                               pntpos()
        *                           deleted api:
        *                               pntvel()
        *           2011/01/12 1.1  add option to include unhealthy satellite
        *                           reject duplicated observation data
        *                           changed api: ionocorr()
        *           2011/11/08 1.2  enable snr mask for single-mode (rtklib_2.4.1_p3)
        *           2012/12/25 1.3  add variable snr mask
        *           2014/05/26 1.4  support galileo and beidou
        *           2015/03/19 1.5  fix bug on ionosphere correction for GLO and BDS
        *-----------------------------------------------------------------------------*/


        internal const string rcsid = "$Id:$";

        /* constants -----------------------------------------------------------------*/


        /* pseudorange measurement error variance ------------------------------------*/
        internal static double varerr(prcopt_t opt, double el, int sys)
        {
            double fact;
            double varr;
            fact = sys == DefineConstants.SYS_GLO ? DefineConstants.EFACT_GLO : (sys == DefineConstants.SYS_SBS ? DefineConstants.EFACT_SBS : DefineConstants.EFACT_GPS);
            varr = ((opt.err[0]) * (opt.err[0])) * (((opt.err[1]) * (opt.err[1])) + ((opt.err[2]) * (opt.err[2])) / Math.Sin(el));
            if (opt.ionoopt == DefineConstants.IONOOPT_IFLC) // iono-free
            {
                varr *= ((3.0) * (3.0));
            }
            return ((fact) * (fact)) * varr;
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
        /* psendorange with code bias correction -------------------------------------*/
        internal static double prange(obsd_t obs, nav_t nav, double[] azel, int iter, prcopt_t opt, ref double @var)
        {
            double[] lam = nav.lam[obs.sat - 1];
            double PC;
            double P1;
            double P2;
            double P1_P2;
            double P1_C1;
            double P2_C2;
            double gamma;
            int i = 0;
            int j = 1;
            int sys;

            @var = 0.0;

            if ((sys = GlobalMembersRtkcmn.satsys(obs.sat, null)) == 0)
            {
                return 0.0;
            }

            /* L1-L2 for GPS/GLO/QZS, L1-L5 for GAL/SBS */
            if (DefineConstants.NFREQ >= 3 && (sys & (DefineConstants.SYS_GAL | DefineConstants.SYS_SBS)))
            {
                j = 2;
            }

            if (DefineConstants.NFREQ < 2 || lam[i] == 0.0 || lam[j] == 0.0)
            {
                return 0.0;
            }

            /* test snr mask */
            if (iter > 0)
            {
                if (GlobalMembersRtkcmn.testsnr(0, i, azel[1], obs.SNR[i] * 0.25, opt.snrmask) != 0)
                {
                    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                    //ORIGINAL LINE: trace(4,"snr mask: %s sat=%2d el=%.1f snr=%.1f\n", time_str(obs->time,0),obs->sat,azel[1]*180.0/DefineConstants.PI,obs->SNR[i]*0.25);
                    GlobalMembersRtkcmn.trace(4, "snr mask: %s sat=%2d el=%.1f snr=%.1f\n", GlobalMembersRtkcmn.time_str(new gtime_t(obs.time), 0), obs.sat, azel[1] * 180.0 / DefineConstants.PI, obs.SNR[i] * 0.25);
                    return 0.0;
                }
                if (opt.ionoopt == DefineConstants.IONOOPT_IFLC)
                {
                    if (GlobalMembersRtkcmn.testsnr(0, j, azel[1], obs.SNR[j] * 0.25, opt.snrmask) != 0)
                    {
                        return 0.0;
                    }
                }
            }
            gamma = ((lam[j]) * (lam[j])) / ((lam[i]) * (lam[i]));
            P1 = obs.P[i];
            P2 = obs.P[j];
            P1_P2 = nav.cbias[obs.sat - 1, 0];
            P1_C1 = nav.cbias[obs.sat - 1, 1];
            P2_C2 = nav.cbias[obs.sat - 1, 2];

            /* if no P1-P2 DCB, use TGD instead */
            if (P1_P2 == 0.0 && (sys & (DefineConstants.SYS_GPS | DefineConstants.SYS_GAL | DefineConstants.SYS_QZS)))
            {
                P1_P2 = (1.0 - gamma) * GlobalMembersPntpos.gettgd(obs.sat, nav);
            }
            if (opt.ionoopt == DefineConstants.IONOOPT_IFLC) // dual-frequency
            {

                if (P1 == 0.0 || P2 == 0.0)
                {
                    return 0.0;
                }
                if (obs.code[i] == DefineConstants.CODE_L1C) // C1->P1
                {
                    P1 += P1_C1;
                }
                if (obs.code[j] == DefineConstants.CODE_L2C) // C2->P2
                {
                    P2 += P2_C2;
                }

                /* iono-free combination */
                PC = (gamma * P1 - P2) / (gamma - 1.0);
            }
            else // single-frequency
            {

                if (P1 == 0.0)
                {
                    return 0.0;
                }
                if (obs.code[i] == DefineConstants.CODE_L1C) // C1->P1
                {
                    P1 += P1_C1;
                }
                PC = P1 - P1_P2 / (1.0 - gamma);
            }
            if (opt.sateph == DefineConstants.EPHOPT_SBAS) // sbas clock based C1
            {
                PC -= P1_C1;
            }

            @var = ((DefineConstants.ERR_CBIAS) * (DefineConstants.ERR_CBIAS));

            return PC;
        }
        /* ionospheric correction ------------------------------------------------------
        * compute ionospheric correction
        * args   : gtime_t time     I   time
        *          nav_t  *nav      I   navigation data
        *          int    sat       I   satellite number
        *          double *pos      I   receiver position {lat,lon,h} (rad|m)
        *          double *azel     I   azimuth/elevation angle {az,el} (rad)
        *          int    ionoopt   I   ionospheric correction option (IONOOPT_???)
        *          double *ion      O   ionospheric delay (L1) (m)
        *          double *var      O   ionospheric delay (L1) variance (m^2)
        * return : status(1:ok,0:error)
        *-----------------------------------------------------------------------------*/
        public static int ionocorr(gtime_t time, nav_t nav, int sat, double[] pos, double[] azel, int ionoopt, ref double ion, ref double @var)
        {
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: trace(4,"ionocorr: time=%s opt=%d sat=%2d pos=%.3f %.3f azel=%.3f %.3f\n", time_str(time,3),ionoopt,sat,pos[0]*180.0/DefineConstants.PI,pos[1]*180.0/DefineConstants.PI,azel[0]*180.0/DefineConstants.PI, azel[1]*180.0/DefineConstants.PI);
            GlobalMembersRtkcmn.trace(4, "ionocorr: time=%s opt=%d sat=%2d pos=%.3f %.3f azel=%.3f %.3f\n", GlobalMembersRtkcmn.time_str(new gtime_t(time), 3), ionoopt, sat, pos[0] * 180.0 / DefineConstants.PI, pos[1] * 180.0 / DefineConstants.PI, azel[0] * 180.0 / DefineConstants.PI, azel[1] * 180.0 / DefineConstants.PI);

            /* broadcast model */
            if (ionoopt == DefineConstants.IONOOPT_BRDC)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: *ion=ionmodel(time,nav->ion_gps,pos,azel);
                ion = GlobalMembersRtkcmn.ionmodel(new gtime_t(time), nav.ion_gps, pos, azel);
                @var = ((ion * DefineConstants.ERR_BRDCI) * (ion * DefineConstants.ERR_BRDCI));
                return 1;
            }
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
            /* qzss broadcast model */
            if (ionoopt == DefineConstants.IONOOPT_QZS && GlobalMembersRtkcmn.norm(nav.ion_qzs, 8) > 0.0)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: *ion=ionmodel(time,nav->ion_qzs,pos,azel);
                ion = GlobalMembersRtkcmn.ionmodel(new gtime_t(time), nav.ion_qzs, pos, azel);
                @var = ((ion * DefineConstants.ERR_BRDCI) * (ion * DefineConstants.ERR_BRDCI));
                return 1;
            }
            /* lex ionosphere model */
            if (ionoopt == DefineConstants.IONOOPT_LEX)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: return lexioncorr(time,nav,pos,azel,ion,var);
                return GlobalMembersQzslex.lexioncorr(new gtime_t(time), nav, pos, azel, ion, ref @var);
            }
            ion = 0.0;
            @var = ionoopt == DefineConstants.IONOOPT_OFF ? ((DefineConstants.ERR_ION) * (DefineConstants.ERR_ION)) : 0.0;
            return 1;
        }
        /* tropospheric correction -----------------------------------------------------
        * compute tropospheric correction
        * args   : gtime_t time     I   time
        *          nav_t  *nav      I   navigation data
        *          double *pos      I   receiver position {lat,lon,h} (rad|m)
        *          double *azel     I   azimuth/elevation angle {az,el} (rad)
        *          int    tropopt   I   tropospheric correction option (TROPOPT_???)
        *          double *trp      O   tropospheric delay (m)
        *          double *var      O   tropospheric delay variance (m^2)
        * return : status(1:ok,0:error)
        *-----------------------------------------------------------------------------*/
        public static int tropcorr(gtime_t time, nav_t nav, double[] pos, double[] azel, int tropopt, ref double trp, ref double @var)
        {
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: trace(4,"tropcorr: time=%s opt=%d pos=%.3f %.3f azel=%.3f %.3f\n", time_str(time,3),tropopt,pos[0]*180.0/DefineConstants.PI,pos[1]*180.0/DefineConstants.PI,azel[0]*180.0/DefineConstants.PI, azel[1]*180.0/DefineConstants.PI);
            GlobalMembersRtkcmn.trace(4, "tropcorr: time=%s opt=%d pos=%.3f %.3f azel=%.3f %.3f\n", GlobalMembersRtkcmn.time_str(new gtime_t(time), 3), tropopt, pos[0] * 180.0 / DefineConstants.PI, pos[1] * 180.0 / DefineConstants.PI, azel[0] * 180.0 / DefineConstants.PI, azel[1] * 180.0 / DefineConstants.PI);

            /* saastamoinen model */
            if (tropopt == DefineConstants.TROPOPT_SAAS || tropopt == DefineConstants.TROPOPT_EST || tropopt == DefineConstants.TROPOPT_ESTG)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: *trp=tropmodel(time,pos,azel,DefineConstants.REL_HUMI);
                trp = GlobalMembersRtkcmn.tropmodel(new gtime_t(time), pos, azel, DefineConstants.REL_HUMI);
                @var = ((DefineConstants.ERR_SAAS / (Math.Sin(azel[1]) + 0.1)) * (DefineConstants.ERR_SAAS / (Math.Sin(azel[1]) + 0.1)));
                return 1;
            }
            /* sbas troposphere model */
            if (tropopt == DefineConstants.TROPOPT_SBAS)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: *trp=sbstropcorr(time,pos,azel,var);
                trp = GlobalMembersSbas.sbstropcorr(new gtime_t(time), pos, azel, ref @var);
                return 1;
            }
            /* no correction */
            trp = 0.0;
            @var = tropopt == DefineConstants.TROPOPT_OFF ? ((DefineConstants.ERR_TROP) * (DefineConstants.ERR_TROP)) : 0.0;
            return 1;
        }
        /* pseudorange residuals -----------------------------------------------------*/
        internal static int rescode(int iter, obsd_t[] obs, int n, double rs, double[] dts, double[] vare, int[] svh, nav_t nav, double[] x, prcopt_t opt, double[] v, double[] H, double[] @var, double[] azel, int[] vsat, double[] resp, ref int ns)
        {
            double r;
            double dion;
            double dtrp;
            double vmeas;
            double vion;
            double vtrp;
            double[] rr = new double[3];
            double[] pos = new double[3];
            double dtr;
            double[] e = new double[3];
            double P;
            double lam_L1;
            int i;
            int j;
            int nv = 0;
            int sys;
            int[] mask = new int[4];

            GlobalMembersRtkcmn.trace(3, "resprng : n=%d\n", n);

            for (i = 0; i < 3; i++)
            {
                rr[i] = x[i];
            }
            dtr = x[3];

            GlobalMembersRtkcmn.ecef2pos(rr, pos);

            for (i = ns = 0; i < n && i < DefineConstants.MAXOBS; i++)
            {
                vsat[i] = 0;
                azel[i * 2] = azel[1 + i * 2] = resp[i] = 0.0;

                if ((sys = GlobalMembersRtkcmn.satsys(obs[i].sat, null)) == 0)
                    continue;

                /* reject duplicated observation data */
                if (i < n - 1 && i < DefineConstants.MAXOBS - 1 && obs[i].sat == obs[i + 1].sat)
                {
                    GlobalMembersRtkcmn.trace(2, "duplicated observation data %s sat=%2d\n", GlobalMembersRtkcmn.time_str(obs[i].time, 3), obs[i].sat);
                    i++;
                    continue;
                }
                /* geometric distance/azimuth/elevation angle */
                if ((r = GlobalMembersRtkcmn.geodist(rs + i * 6, rr, e)) <= 0.0 || GlobalMembersRtkcmn.satazel(pos, e, azel + i * 2) < opt.elmin)
                    continue;

                /* psudorange with code bias correction */
                if ((P = GlobalMembersPntpos.prange(obs + i, nav, azel + i * 2, iter, opt, ref vmeas)) == 0.0)
                    continue;

                /* excluded satellite? */
                if (GlobalMembersRtkcmn.satexclude(obs[i].sat, svh[i], opt) != 0)
                    continue;

                /* ionospheric corrections */
                if (GlobalMembersPntpos.ionocorr(obs[i].time, nav, obs[i].sat, pos, azel + i * 2, iter > 0 ? opt.ionoopt : DefineConstants.IONOOPT_BRDC, ref dion, ref vion) == 0)
                    continue;

                /* GPS-L1 -> L1/B1 */
                if ((lam_L1 = nav.lam[obs[i].sat - 1, 0]) > 0.0)
                {
                    dion *= ((lam_L1 / GlobalMembersRtkcmn.lam_carr[0]) * (lam_L1 / GlobalMembersRtkcmn.lam_carr[0]));
                }
                /* tropospheric corrections */
                if (GlobalMembersPntpos.tropcorr(obs[i].time, nav, pos, azel + i * 2, iter > 0 ? opt.tropopt : DefineConstants.TROPOPT_SAAS, ref dtrp, ref vtrp) == 0)
                {
                    continue;
                }
                /* pseudorange residual */
                v[nv] = P - (r + dtr - DefineConstants.CLIGHT * dts[i * 2] + dion + dtrp);

                /* design matrix */
                for (j = 0; j < 4 + 3; j++)
                {
                    H[j + nv * 4 + 3] = j < 3 ? -e[j] : (j == 3 ? 1.0 : 0.0);
                }

                /* time system and receiver bias offset correction */
                if (sys == DefineConstants.SYS_GLO)
                {
                    v[nv] -= x[4];
                    H[4 + nv * 4 + 3] = 1.0;
                    mask[1] = 1;
                }
                else if (sys == DefineConstants.SYS_GAL)
                {
                    v[nv] -= x[5];
                    H[5 + nv * 4 + 3] = 1.0;
                    mask[2] = 1;
                }
                else if (sys == DefineConstants.SYS_CMP)
                {
                    v[nv] -= x[6];
                    H[6 + nv * 4 + 3] = 1.0;
                    mask[3] = 1;
                }
                else
                {
                    mask[0] = 1;
                }

                vsat[i] = 1;
                resp[i] = v[nv];
                ns++;

                /* error variance */
                @var[nv++] = GlobalMembersPntpos.varerr(opt, azel[1 + i * 2], sys) + vare[i] + vmeas + vion + vtrp;

                GlobalMembersRtkcmn.trace(4, "sat=%2d azel=%5.1f %4.1f res=%7.3f sig=%5.3f\n", obs[i].sat, azel[i * 2] * 180.0 / DefineConstants.PI, azel[1 + i * 2] * 180.0 / DefineConstants.PI, resp[i], Math.Sqrt(@var[nv - 1]));
            }
            /* constraint to avoid rank-deficient */
            for (i = 0; i < 4; i++)
            {
                if (mask[i] != 0)
                    continue;
                v[nv] = 0.0;
                for (j = 0; j < 4 + 3; j++)
                {
                    H[j + nv * 4 + 3] = j == i + 3 ? 1.0 : 0.0;
                }
                @var[nv++] = 0.01;
            }
            return nv;
        }
        /* validate solution ---------------------------------------------------------*/
        internal static int valsol(double[] azel, int[] vsat, int n, prcopt_t opt, double v, int nv, int nx, ref string msg)
        {
            double[] azels = new double[DefineConstants.MAXOBS * 2];
            double[] dop = new double[4];
            double vv;
            int i;
            int ns;

            GlobalMembersRtkcmn.trace(3, "valsol  : n=%d nv=%d\n", n, nv);

            /* chi-square validation of residuals */
            vv = GlobalMembersRtkcmn.dot(v, v, nv);
            if (nv > nx && vv > GlobalMembersRtkcmn.chisqr[nv - nx - 1])
            {
                msg = string.Format("chi-square error nv={0:D} vv={1:f1} cs={2:f1}", nv, vv, GlobalMembersRtkcmn.chisqr[nv - nx - 1]);
                return 0;
            }
            /* large gdop check */
            for (i = ns = 0; i < n; i++)
            {
                if (!vsat[i])
                    continue;
                azels[ns * 2] = azel[i * 2];
                azels[1 + ns * 2] = azel[1 + i * 2];
                ns++;
            }
            GlobalMembersRtkcmn.dops(ns, azels, opt.elmin, dop);
            if (dop[0] <= 0.0 || dop[0] > opt.maxgdop)
            {
                msg = string.Format("gdop error nv={0:D} gdop={1:f1}", nv, dop[0]);
                return 0;
            }
            return 1;
        }
        /* estimate receiver position ------------------------------------------------*/
        internal static int estpos(obsd_t[] obs, int n, double rs, double dts, double vare, int svh, nav_t nav, prcopt_t opt, sol_t sol, ref double azel, ref int vsat, ref double resp, ref string msg)
        {
            double[] x = { 0 };
            double[] dx = new double[4 + 3];
            double[] Q = new double[4 + 3 * 4 + 3];
            double[] v;
            double[] H;
            double[] @var;
            double sig;
            int i;
            int j;
            int k;
            int info;
            int stat;
            int nv;
            int ns;

            GlobalMembersRtkcmn.trace(3, "estpos  : n=%d\n", n);

            v = GlobalMembersRtkcmn.mat(n + 4, 1);
            H = GlobalMembersRtkcmn.mat(4 + 3, n + 4);
            @var = GlobalMembersRtkcmn.mat(n + 4, 1);

            for (i = 0; i < 3; i++)
            {
                x[i] = sol.rr[i];
            }

            for (i = 0; i < DefineConstants.MAXITR; i++)
            {

                /* pseudorange residuals */
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: nv=rescode(i,obs,n,rs,dts,vare,svh,nav,x,opt,v,H,var,azel,vsat,resp, &ns);
                nv = GlobalMembersPntpos.rescode(i, new obsd_t(obs), n, rs, dts, vare, svh, nav, x, opt, v, H, @var, azel, vsat, resp, ref ns);

                if (nv < 4 + 3)
                {
                    msg = string.Format("lack of valid sats ns={0:D}", nv);
                    break;
                }
                /* weight by variance */
                for (j = 0; j < nv; j++)
                {
                    sig = Math.Sqrt(@var[j]);
                    v[j] /= sig;
                    for (k = 0; k < 4 + 3; k++)
                    {
                        H[k + j * 4 + 3] /= sig;
                    }
                }
                /* least square estimation */
                if ((info = GlobalMembersRtkcmn.lsq(H, v, 4 + 3, nv, ref dx, ref Q)) != 0)
                {
                    msg = string.Format("lsq error info={0:D}", info);
                    break;
                }
                for (j = 0; j < 4 + 3; j++)
                {
                    x[j] += dx[j];
                }

                if (GlobalMembersRtkcmn.norm(dx, 4 + 3) < 1E-4)
                {
                    sol.type = 0;
                    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                    //ORIGINAL LINE: sol->time=timeadd(obs[0].time,-x[3]/DefineConstants.CLIGHT);
                    sol.time.CopyFrom(GlobalMembersRtkcmn.timeadd(obs[0].time, -x[3] / DefineConstants.CLIGHT));
                    sol.dtr[0] = x[3] / DefineConstants.CLIGHT; // receiver clock bias (s)
                    sol.dtr[1] = x[4] / DefineConstants.CLIGHT; // glo-gps time offset (s)
                    sol.dtr[2] = x[5] / DefineConstants.CLIGHT; // gal-gps time offset (s)
                    sol.dtr[3] = x[6] / DefineConstants.CLIGHT; // bds-gps time offset (s)
                    for (j = 0; j < 6; j++)
                    {
                        sol.rr[j] = j < 3 ? x[j] : 0.0;
                    }
                    for (j = 0; j < 3; j++)
                    {
                        sol.qr[j] = (float)Q[j + j * 4 + 3];
                    }
                    sol.qr[3] = (float)Q[1]; // cov xy
                    sol.qr[4] = (float)Q[2 + 4 + 3]; // cov yz
                    sol.qr[5] = (float)Q[2]; // cov zx
                    sol.ns = (byte)ns;
                    sol.age = sol.ratio = 0.0F;

                    /* validate solution */
                    if ((stat = GlobalMembersPntpos.valsol(azel, vsat, n, opt, v, nv, 4 + 3, ref msg)) != 0)
                    {
                        sol.stat = opt.sateph == DefineConstants.EPHOPT_SBAS ? DefineConstants.SOLQ_SBAS : DefineConstants.SOLQ_SINGLE;
                    }
                    //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
                    free(v);
                    //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
                    free(H);
                    //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
                    free(@var);

                    return stat;
                }
            }
            if (i >= DefineConstants.MAXITR)
            {
                msg = string.Format("iteration divergent i={0:D}", i);
            }

            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(v);
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(H);
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(@var);

            return 0;
        }
        /* raim fde (failure detection and exclution) -------------------------------*/
        internal static int raim_fde(obsd_t[] obs, int n, double rs, double dts, double[] vare, int[] svh, nav_t nav, prcopt_t opt, sol_t sol, ref double azel, int[] vsat, double[] resp, ref string msg)
        {
            obsd_t[] obs_e;
            sol_t sol_e = new sol_t({ 0 });
            string tstr = new string(new char[32]);
            string name = new string(new char[16]);
            string msg_e = new string(new char[128]);
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: double *rs_e,*dts_e,*vare_e,*azel_e,*resp_e,rms_e,rms=100.0;
            double rs_e;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: double *dts_e;
            double dts_e;
            double[] vare_e;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: double *azel_e;
            double azel_e;
            double[] resp_e;
            double rms_e;
            double rms = 100.0;
            int i;
            int j;
            int k;
            int nvsat;
            int stat = 0;
            int[] svh_e;
            int[] vsat_e;
            int sat = 0;

            GlobalMembersRtkcmn.trace(3, "raim_fde: %s n=%2d\n", GlobalMembersRtkcmn.time_str(obs[0].time, 0), n);

            if (!(obs_e = new obsd_t[n]))
            {
                return 0;
            }
            rs_e = GlobalMembersRtkcmn.mat(6, n);
            dts_e = GlobalMembersRtkcmn.mat(2, n);
            vare_e = GlobalMembersRtkcmn.mat(1, n);
            azel_e = GlobalMembersRtkcmn.zeros(2, n);
            svh_e = GlobalMembersRtkcmn.imat(1, n);
            vsat_e = GlobalMembersRtkcmn.imat(1, n);
            resp_e = GlobalMembersRtkcmn.mat(1, n);

            for (i = 0; i < n; i++)
            {

                /* satellite exclution */
                for (j = k = 0; j < n; j++)
                {
                    if (j == i)
                        continue;
                    obs_e[k] = obs[j];
                    GlobalMembersRtkcmn.matcpy(ref rs_e + 6 * k, rs + 6 * j, 6, 1);
                    GlobalMembersRtkcmn.matcpy(ref dts_e + 2 * k, dts + 2 * j, 2, 1);
                    vare_e[k] = vare[j];
                    svh_e[k++] = svh[j];
                }
                /* estimate receiver position without a satellite */
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: if (!estpos(obs_e,n-1,rs_e,dts_e,vare_e,svh_e,nav,opt,&sol_e,azel_e, vsat_e,resp_e,msg_e))
                if (GlobalMembersPntpos.estpos(new obsd_t(obs_e), n - 1, rs_e, dts_e, vare_e, svh_e, nav, opt, sol_e, ref azel_e, ref vsat_e, ref resp_e, ref msg_e) == 0)
                {
                    GlobalMembersRtkcmn.trace(3, "raim_fde: exsat=%2d (%s)\n", obs[i].sat, msg);
                    continue;
                }
                for (j = nvsat = 0, rms_e = 0.0; j < n - 1; j++)
                {
                    if (vsat_e[j] == 0)
                        continue;
                    rms_e += ((resp_e[j]) * (resp_e[j]));
                    nvsat++;
                }
                if (nvsat < 5)
                {
                    GlobalMembersRtkcmn.trace(3, "raim_fde: exsat=%2d lack of satellites nvsat=%2d\n", obs[i].sat, nvsat);
                    continue;
                }
                rms_e = Math.Sqrt(rms_e / nvsat);

                GlobalMembersRtkcmn.trace(3, "raim_fde: exsat=%2d rms=%8.3f\n", obs[i].sat, rms_e);

                if (rms_e > rms)
                    continue;

                /* save result */
                for (j = k = 0; j < n; j++)
                {
                    if (j == i)
                        continue;
                    GlobalMembersRtkcmn.matcpy(ref azel + 2 * j, azel_e + 2 * k, 2, 1);
                    vsat[j] = vsat_e[k];
                    resp[j] = resp_e[k++];
                }
                stat = 1;
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                //ORIGINAL LINE: *sol=sol_e;
                sol.CopyFrom(sol_e);
                sat = obs[i].sat;
                rms = rms_e;
                vsat[i] = 0;
                msg = msg_e;
            }
            if (stat != 0)
            {
                GlobalMembersRtkcmn.time2str(obs[0].time, ref tstr, 2);
                GlobalMembersRtkcmn.satno2id(sat, ref name);
                GlobalMembersRtkcmn.trace(2, "%s: %s excluded by raim\n", tstr.Substring(11), name);
            }
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(obs_e);
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(rs_e);
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(dts_e);
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(vare_e);
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(azel_e);
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(svh_e);
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(vsat_e);
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(resp_e);
            return stat;
        }
        /* doppler residuals ---------------------------------------------------------*/
        internal static int resdop(obsd_t[] obs, int n, double[] rs, double[] dts, nav_t nav, double[] rr, double[] x, double[] azel, int[] vsat, double[] v, double[] H)
        {
            double lam;
            double rate;
            double[] pos = new double[3];
            double[] E = new double[9];
            double[] a = new double[3];
            double[] e = new double[3];
            double[] vs = new double[3];
            double cosel;
            int i;
            int j;
            int nv = 0;

            GlobalMembersRtkcmn.trace(3, "resdop  : n=%d\n", n);

            GlobalMembersRtkcmn.ecef2pos(rr, pos);
            GlobalMembersRtkcmn.xyz2enu(pos, E);

            for (i = 0; i < n && i < DefineConstants.MAXOBS; i++)
            {

                lam = nav.lam[obs[i].sat - 1, 0];

                if (obs[i].D[0] == 0.0 || lam == 0.0 || !vsat[i] || GlobalMembersRtkcmn.norm(rs + 3 + i * 6, 3) <= 0.0)
                {
                    continue;
                }
                /* line-of-sight vector in ecef */
                cosel = Math.Cos(azel[1 + i * 2]);
                a[0] = Math.Sin(azel[i * 2]) * cosel;
                a[1] = Math.Cos(azel[i * 2]) * cosel;
                a[2] = Math.Sin(azel[1 + i * 2]);
                GlobalMembersRtkcmn.matmul("TN", 3, 1, 3, 1.0, E, a, 0.0, ref e);

                /* satellite velocity relative to receiver in ecef */
                for (j = 0; j < 3; j++)
                {
                    vs[j] = rs[j + 3 + i * 6] - x[j];
                }

                /* range rate with earth rotation correction */
                rate = GlobalMembersRtkcmn.dot(vs, e, 3) + DefineConstants.OMGE / DefineConstants.CLIGHT * (rs[4 + i * 6] * rr[0] + rs[1 + i * 6] * x[0] - rs[3 + i * 6] * rr[1] - rs[i * 6] * x[1]);

                /* doppler residual */
                v[nv] = -lam * obs[i].D[0] - (rate + x[3] - DefineConstants.CLIGHT * dts[1 + i * 2]);

                /* design matrix */
                for (j = 0; j < 4; j++)
                {
                    H[j + nv * 4] = j < 3 ? -e[j] : 1.0;
                }

                nv++;
            }
            return nv;
        }
        /* estimate receiver velocity ------------------------------------------------*/
        internal static void estvel(obsd_t obs, int n, double rs, double dts, nav_t nav, prcopt_t opt, sol_t sol, double azel, int vsat)
        {
            double[] x = { 0, null, null, null };
            double[] dx = new double[4];
            double[] Q = new double[16];
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: double *v;
            double v;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: double *H;
            double H;
            int i;
            int j;
            int nv;

            GlobalMembersRtkcmn.trace(3, "estvel  : n=%d\n", n);

            v = GlobalMembersRtkcmn.mat(n, 1);
            H = GlobalMembersRtkcmn.mat(4, n);

            for (i = 0; i < DefineConstants.MAXITR; i++)
            {

                /* doppler residuals */
                if ((nv = GlobalMembersPntpos.resdop(obs, n, rs, dts, nav, sol.rr, x, azel, vsat, v, H)) < 4)
                {
                    break;
                }
                /* least square estimation */
                if (GlobalMembersRtkcmn.lsq(H, v, 4, nv, ref dx, ref Q) != 0)
                    break;

                for (j = 0; j < 4; j++)
                {
                    x[j] += dx[j];
                }

                if (GlobalMembersRtkcmn.norm(dx, 4) < 1E-6)
                {
                    for (i = 0; i < 3; i++)
                    {
                        sol.rr[i + 3] = x[i];
                    }
                    break;
                }
            }
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(v);
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(H);
        }
        /* single-point positioning ----------------------------------------------------
        * compute receiver position, velocity, clock bias by single-point positioning
        * with pseudorange and doppler observables
        * args   : obsd_t *obs      I   observation data
        *          int    n         I   number of observation data
        *          nav_t  *nav      I   navigation data
        *          prcopt_t *opt    I   processing options
        *          sol_t  *sol      IO  solution
        *          double *azel     IO  azimuth/elevation angle (rad) (NULL: no output)
        *          ssat_t *ssat     IO  satellite status              (NULL: no output)
        *          char   *msg      O   error message for error exit
        * return : status(1:ok,0:error)
        * notes  : assuming sbas-gps, galileo-gps, qzss-gps, compass-gps time offset and
        *          receiver bias are negligible (only involving glonass-gps time offset
        *          and receiver bias)
        *-----------------------------------------------------------------------------*/
        public static int pntpos(obsd_t[] obs, int n, nav_t nav, prcopt_t opt, sol_t sol, double[] azel, ssat_t[] ssat, ref string msg)
        {
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: prcopt_t opt_=*opt;
            prcopt_t opt_ = new prcopt_t(opt);
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: double *rs,*dts,*var,*azel_,*resp;
            double rs;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: double *dts;
            double dts;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: double *var;
            double @var;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: double *azel_;
            double azel_;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: double *resp;
            double resp;
            int i;
            int stat;
            int[] vsat = new int[DefineConstants.MAXOBS];
            int[] svh = new int[DefineConstants.MAXOBS];

            sol.stat = DefineConstants.SOLQ_NONE;

            if (n <= 0)
            {
                msg = "no observation data";
                return 0;
            }

            GlobalMembersRtkcmn.trace(3, "pntpos  : tobs=%s n=%d\n", GlobalMembersRtkcmn.time_str(obs[0].time, 3), n);

            sol.time = obs[0].time;
            msg[0] = '\0';

            rs = GlobalMembersRtkcmn.mat(6, n);
            dts = GlobalMembersRtkcmn.mat(2, n);
            @var = GlobalMembersRtkcmn.mat(1, n);
            azel_ = GlobalMembersRtkcmn.zeros(2, n);
            resp = GlobalMembersRtkcmn.mat(1, n);

            if (opt_.mode != DefineConstants.PMODE_SINGLE) // for precise positioning
            {
#if false
	//        opt_.sateph =EPHOPT_BRDC;
#endif
                opt_.ionoopt = DefineConstants.IONOOPT_BRDC;
                opt_.tropopt = DefineConstants.TROPOPT_SAAS;
            }
            /* satellite positons, velocities and clocks */
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: satposs(sol->time,obs,n,nav,opt_.sateph,rs,dts,var,svh);
            GlobalMembersEphemeris.satposs(new gtime_t(sol.time), new obsd_t(obs), n, nav, opt_.sateph, rs, dts, @var, svh);

            /* estimate receiver position with pseudorange */
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: stat=estpos(obs,n,rs,dts,var,svh,nav,&opt_,sol,azel_,vsat,resp,msg);
            stat = GlobalMembersPntpos.estpos(new obsd_t(obs), n, rs, dts, @var, svh, nav, opt_, sol, ref azel_, ref vsat, ref resp, ref msg);

            /* raim fde */
            if (stat == 0 && n >= 6 && opt.posopt[4] != 0)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: stat=raim_fde(obs,n,rs,dts,var,svh,nav,&opt_,sol,azel_,vsat,resp,msg);
                stat = GlobalMembersPntpos.raim_fde(new obsd_t(obs), n, rs, dts, @var, svh, nav, opt_, sol, ref azel_, vsat, resp, ref msg);
            }
            /* estimate receiver velocity with doppler */
            if (stat != 0)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: estvel(obs,n,rs,dts,nav,&opt_,sol,azel_,vsat);
                GlobalMembersPntpos.estvel(new obsd_t(obs), n, rs, dts, nav, opt_, sol, azel_, vsat);
            }

            if (azel != 0)
            {
                for (i = 0; i < n * 2; i++)
                {
                    azel[i] = azel_[i];
                }
            }
            if (ssat != null)
            {
                for (i = 0; i < DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1; i++)
                {
                    ssat[i].vs = 0;
                    ssat[i].azel[0] = ssat[i].azel[1] = 0.0;
                    ssat[i].resp[0] = ssat[i].resc[0] = 0.0;
                    ssat[i].snr[0] = 0;
                }
                for (i = 0; i < n; i++)
                {
                    ssat[obs[i].sat - 1].azel[0] = azel_[i * 2];
                    ssat[obs[i].sat - 1].azel[1] = azel_[1 + i * 2];
                    ssat[obs[i].sat - 1].snr[0] = obs[i].SNR[0];
                    if (vsat[i] == 0)
                        continue;
                    ssat[obs[i].sat - 1].vs = 1;
                    ssat[obs[i].sat - 1].resp[0] = resp[i];
                }
            }
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(rs);
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(dts);
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(@var);
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(azel_);
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(resp);
            return stat;
        }
    }
}
