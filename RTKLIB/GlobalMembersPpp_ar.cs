using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ghGPS.Classes
{
    public static class GlobalMembersPpp_ar
    {
        /*------------------------------------------------------------------------------
        * ppp_ar.c : ppp ambiguity resolution
        *
        * options : -DREV_WL_FCB reversed polarity of WL FCB
        *
        * reference :
        *    [1] H.Okumura, C-gengo niyoru saishin algorithm jiten (in Japanese),
        *        Software Technology, 1991
        *
        *          Copyright (C) 2012-2013 by T.TAKASU, All rights reserved.
        *
        * version : $Revision:$ $Date:$
        * history : 2013/03/11 1.0  new
        *-----------------------------------------------------------------------------*/


        /* wave length of LC (m) -----------------------------------------------------*/
        internal static double lam_LC(int i, int j, int k)
        {
            const double f1 = DefineConstants.FREQ1;
            const double f2 = DefineConstants.FREQ2;
            const double f5 = DefineConstants.FREQ5;

            return DefineConstants.CLIGHT / (i * f1 + j * f2 + k * f5);
        }
        /* carrier-phase LC (m) ------------------------------------------------------*/
        internal static double L_LC(int i, int j, int k, double[] L)
        {
            const double f1 = DefineConstants.FREQ1;
            const double f2 = DefineConstants.FREQ2;
            const double f5 = DefineConstants.FREQ5;
            double L1;
            double L2;
            double L5;

            if ((i != 0 && !L[0]) || (j != 0 && !L[1]) || (k != 0 && !L[2]))
            {
                return 0.0;
            }
            L1 = DefineConstants.CLIGHT / f1 * L[0];
            L2 = DefineConstants.CLIGHT / f2 * L[1];
            L5 = DefineConstants.CLIGHT / f5 * L[2];
            return (i * f1 * L1 + j * f2 * L2 + k * f5 * L5) / (i * f1 + j * f2 + k * f5);
        }
        /* pseudorange LC (m) --------------------------------------------------------*/
        internal static double P_LC(int i, int j, int k, double[] P)
        {
            const double f1 = DefineConstants.FREQ1;
            const double f2 = DefineConstants.FREQ2;
            const double f5 = DefineConstants.FREQ5;
            double P1;
            double P2;
            double P5;

            if ((i != 0 && !P[0]) || (j != 0 && !P[1]) || (k != 0 && !P[2]))
            {
                return 0.0;
            }
            P1 = P[0];
            P2 = P[1];
            P5 = P[2];
            return (i * f1 * P1 + j * f2 * P2 + k * f5 * P5) / (i * f1 + j * f2 + k * f5);
        }
        /* noise variance of LC (m) --------------------------------------------------*/
        internal static double var_LC(int i, int j, int k, double sig)
        {
            const double f1 = DefineConstants.FREQ1;
            const double f2 = DefineConstants.FREQ2;
            const double f5 = DefineConstants.FREQ5;

            return (((i * f1) * (i * f1)) + ((j * f2) * (j * f2)) + ((k * f5) * (k * f5))) / ((i * f1 + j * f2 + k * f5) * (i * f1 + j * f2 + k * f5)) * ((sig) * (sig));
        }
        /* complementaty error function (ref [1] p.227-229) --------------------------*/
        internal static double q_gamma(double a, double x, double log_gamma_a)
        {
            double y;
            double w;
            double la = 1.0;
            double lb = x + 1.0 - a;
            double lc;
            int i;

            if (x < a + 1.0)
            {
                return 1.0 - GlobalMembersPpp_ar.p_gamma(a, x, log_gamma_a);
            }
            w = Math.Exp(-x + a * Math.Log(x) - log_gamma_a);
            y = w / lb;
            for (i = 2; i < 100; i++)
            {
                lc = ((i - 1 - a) * (lb - la) + (i + x) * lb) / i;
                la = lb;
                lb = lc;
                w *= (i - 1 - a) / i;
                y += w / la / lb;
                if (Math.Abs(w / la / lb) < 1E-15)
                    break;
            }
            return y;
        }
        internal static double p_gamma(double a, double x, double log_gamma_a)
        {
            double y;
            double w;
            int i;

            if (x == 0.0)
            {
                return 0.0;
            }
            if (x >= a + 1.0)
            {
                return 1.0 - GlobalMembersPpp_ar.q_gamma(a, x, log_gamma_a);
            }

            y = w = Math.Exp(a * Math.Log(x) - x - log_gamma_a) / a;

            for (i = 1; i < 100; i++)
            {
                w *= x / (a + i);
                y += w;
                if (Math.Abs(w) < 1E-15)
                    break;
            }
            return y;
        }
        internal static double f_erfc(double x)
        {
            return x >= 0.0 ? GlobalMembersPpp_ar.q_gamma(0.5, x * x, DefineConstants.LOG_PI / 2.0) : 1.0 + GlobalMembersPpp_ar.p_gamma(0.5, x * x, DefineConstants.LOG_PI / 2.0);
        }
        /* confidence function of integer ambiguity ----------------------------------*/
        internal static double conffunc(int N, double B, double sig)
        {
            double x;
            double p = 1.0;
            int i;

            x = Math.Abs(B - N);
            for (i = 1; i < 8; i++)
            {
                p -= GlobalMembersPpp_ar.f_erfc((i - x) / (DefineConstants.SQRT2 * sig)) - GlobalMembersPpp_ar.f_erfc((i + x) / (DefineConstants.SQRT2 * sig));
            }
            return p;
        }
        /* average LC ----------------------------------------------------------------*/
        internal static void average_LC(rtk_t rtk, obsd_t[] obs, int n, nav_t nav, double[] azel)
        {
            ambc_t amb;
            double LC1;
            double LC2;
            double LC3;
            double var1;
            double var2;
            double var3;
            double sig;
            int i;
            int j;
            int sat;

            for (i = 0; i < n; i++)
            {
                sat = obs[i].sat;

                if (azel[1 + 2 * i] < rtk.opt.elmin)
                    continue;

                if (GlobalMembersRtkcmn.satsys(sat, null) != DefineConstants.SYS_GPS)
                    continue;

                /* triple-freq carrier and code LC (m) */
                LC1 = GlobalMembersPpp_ar.L_LC(1, -1, 0, obs[i].L) - GlobalMembersPpp_ar.P_LC(1, 1, 0, obs[i].P);
                LC2 = GlobalMembersPpp_ar.L_LC(0, 1, -1, obs[i].L) - GlobalMembersPpp_ar.P_LC(0, 1, 1, obs[i].P);
                LC3 = GlobalMembersPpp_ar.L_LC(1, -6, 5, obs[i].L) - GlobalMembersPpp_ar.P_LC(1, 1, 0, obs[i].P);

                sig = Math.Sqrt(((rtk.opt.err[1]) * (rtk.opt.err[1])) + ((rtk.opt.err[2] / Math.Sin(azel[1 + 2 * i])) * (rtk.opt.err[2] / Math.Sin(azel[1 + 2 * i]))));

                /* measurement noise variance (m) */
                var1 = GlobalMembersPpp_ar.var_LC(1, 1, 0, sig * rtk.opt.eratio[0]);
                var2 = GlobalMembersPpp_ar.var_LC(0, 1, 1, sig * rtk.opt.eratio[0]);
                var3 = GlobalMembersPpp_ar.var_LC(1, 1, 0, sig * rtk.opt.eratio[0]);

                amb = rtk.ambc + sat - 1;

                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: if (rtk->ssat[sat-1].slip[0]||rtk->ssat[sat-1].slip[1]|| rtk->ssat[sat-1].slip[2]||amb->n[0]==0.0|| fabs(timediff(amb->epoch[0],obs[0].time))>DefineConstants.MIN_ARC_GAP)
                if (rtk.ssat[sat - 1].slip[0] != 0 || rtk.ssat[sat - 1].slip[1] != 0 || rtk.ssat[sat - 1].slip[2] != 0 || amb.n[0] == 0.0 || Math.Abs(GlobalMembersRtkcmn.timediff(new gtime_t(amb.epoch[0]), obs[0].time)) > DefineConstants.MIN_ARC_GAP)
                {

                    amb.n[0] = amb.n[1] = amb.n[2] = 0.0;
                    amb.LC[0] = amb.LC[1] = amb.LC[2] = 0.0;
                    amb.LCv[0] = amb.LCv[1] = amb.LCv[2] = 0.0;
                    amb.fixcnt = 0;
                    for (j = 0; j < DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1; j++)
                    {
                        amb.flags = amb.flags.Substring(0, j);
                    }
                }
                /* averaging */
                if (LC1 != 0)
                {
                    amb.n[0] += 1.0;
                    amb.LC[0] += (LC1 - amb.LC[0]) / amb.n[0];
                    amb.LCv[0] += (var1 - amb.LCv[0]) / amb.n[0];
                }
                if (LC2 != 0)
                {
                    amb.n[1] += 1.0;
                    amb.LC[1] += (LC2 - amb.LC[1]) / amb.n[1];
                    amb.LCv[1] += (var2 - amb.LCv[1]) / amb.n[1];
                }
                if (LC3 != 0)
                {
                    amb.n[2] += 1.0;
                    amb.LC[2] += (LC3 - amb.LC[2]) / amb.n[2];
                    amb.LCv[2] += (var3 - amb.LCv[2]) / amb.n[2];
                }
                amb.epoch[0] = obs[0].time;
            }
        }
        /* fix wide-lane ambiguity ---------------------------------------------------*/
        internal static int fix_amb_WL(rtk_t rtk, nav_t nav, int sat1, int sat2, ref int NW)
        {
            ambc_t amb1;
            ambc_t amb2;
            double BW;
            double vW;
            double lam_WL = GlobalMembersPpp_ar.lam_LC(1, -1, 0);

            amb1 = rtk.ambc + sat1 - 1;
            amb2 = rtk.ambc + sat2 - 1;
            if (amb1.n[0] == 0 || amb2.n[0] == 0)
            {
                return 0;
            }

            /* wide-lane ambiguity */
#if !REV_WL_FCB
            BW = (amb1.LC[0] - amb2.LC[0]) / lam_WL + nav.wlbias[sat1 - 1] - nav.wlbias[sat2 - 1];
#else
		BW = (amb1.LC[0] - amb2.LC[0]) / lam_WL - nav.wlbias[sat1 - 1] + nav.wlbias[sat2 - 1];
#endif
            NW = (int)Math.Floor((BW) + 0.5);

            /* variance of wide-lane ambiguity */
            vW = (amb1.LCv[0] / amb1.n[0] + amb2.LCv[0] / amb2.n[0]) / ((lam_WL) * (lam_WL));

            /* validation of integer wide-lane ambigyity */
            return Math.Abs(NW - BW) <= rtk.opt.thresar[2] && GlobalMembersPpp_ar.conffunc(NW, BW, Math.Sqrt(vW)) >= rtk.opt.thresar[1];
        }
        /* linear dependency check ---------------------------------------------------*/
        internal static int is_depend(int sat1, int sat2, int[] flgs, ref int max_flg)
        {
            int i;

            if (flgs[sat1 - 1] == 0 && flgs[sat2 - 1] == 0)
            {
                flgs[sat1 - 1] = flgs[sat2 - 1] = ++max_flg;
            }
            else if (flgs[sat1 - 1] == 0 && flgs[sat2 - 1] != 0)
            {
                flgs[sat1 - 1] = flgs[sat2 - 1];
            }
            else if (flgs[sat1 - 1] != 0 && flgs[sat2 - 1] == 0)
            {
                flgs[sat2 - 1] = flgs[sat1 - 1];
            }
            else if (flgs[sat1 - 1] > flgs[sat2 - 1])
            {
                for (i = 0; i < DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1; i++)
                {
                    if (flgs[i] == flgs[sat2 - 1])
                    {
                        flgs[i] = flgs[sat1 - 1];
                    }
                }
            }
            else if (flgs[sat1 - 1] < flgs[sat2 - 1])
            {
                for (i = 0; i < DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1; i++)
                {
                    if (flgs[i] == flgs[sat1 - 1])
                    {
                        flgs[i] = flgs[sat2 - 1];
                    }
                }
            }
            else // linear depenent
            {
                return 0;
            }
            return 1;
        }
        /* select fixed ambiguities --------------------------------------------------*/
        internal static int sel_amb(int[] sat1, int[] sat2, double[] N, double[] @var, int n)
        {
            int i;
            int j;
            int[] flgs = new int[DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1];
            int max_flg = 0;

            /* sort by variance */
            for (i = 0; i < n; i++)
            {
                for (j = 1; j < n - i; j++)
                {
                    if (@var[j] >= @var[j - 1])
                        continue;
                    do
                    {
                        int _z = sat1[j];
                        sat1[j] = sat1[j - 1];
                        sat1[j - 1] = _z;
                    } while (0);
                    do
                    {
                        int _z = sat2[j];
                        sat2[j] = sat2[j - 1];
                        sat2[j - 1] = _z;
                    } while (0);
                    do
                    {
                        double _z = N[j];
                        N[j] = N[j - 1];
                        N[j - 1] = _z;
                    } while (0);
                    do
                    {
                        double _z = @var[j];
                        @var[j] = @var[j - 1];
                        @var[j - 1] = _z;
                    } while (0);
                }
            }
            /* select linearly independent satellite pair */
            for (i = j = 0; i < n; i++)
            {
                if (GlobalMembersPpp_ar.is_depend(sat1[i], sat2[i], flgs, ref max_flg) == 0)
                    continue;
                sat1[j] = sat1[i];
                sat2[j] = sat2[i];
                N[j] = N[i];
                @var[j++] = @var[i];
            }
            return j;
        }
        /* fixed solution ------------------------------------------------------------*/
        internal static int fix_sol(rtk_t rtk, int[] sat1, int[] sat2, double[] NC, int n)
        {
            double[] v;
            double[] H;
            double[] R;
            int i;
            int j;
            int k;
            int info;

            if (n <= 0)
            {
                return 0;
            }

            v = GlobalMembersRtkcmn.zeros(n, 1);
            H = GlobalMembersRtkcmn.zeros(rtk.nx, n);
            R = GlobalMembersRtkcmn.zeros(n, n);

            /* constraints to fixed ambiguities */
            for (i = 0; i < n; i++)
            {
                j = ((((((rtk.opt).dynamics != 0 ? 9 : 3) + (0)) + DefineConstants.NSYSGPS + DefineConstants.NSYSGLO + DefineConstants.NSYSGAL + DefineConstants.NSYSQZS + DefineConstants.NSYSCMP + DefineConstants.NSYSLEO) + ((rtk.opt).tropopt < DefineConstants.TROPOPT_EST ? 0 : ((rtk.opt).tropopt == DefineConstants.TROPOPT_EST ? 1 : 3))) + (sat1[i]) - 1);
                k = ((((((rtk.opt).dynamics != 0 ? 9 : 3) + (0)) + DefineConstants.NSYSGPS + DefineConstants.NSYSGLO + DefineConstants.NSYSGAL + DefineConstants.NSYSQZS + DefineConstants.NSYSCMP + DefineConstants.NSYSLEO) + ((rtk.opt).tropopt < DefineConstants.TROPOPT_EST ? 0 : ((rtk.opt).tropopt == DefineConstants.TROPOPT_EST ? 1 : 3))) + (sat2[i]) - 1);
                v[i] = NC[i] - (rtk.x[j] - rtk.x[k]);
                H[j + i * rtk.nx] = 1.0;
                H[k + i * rtk.nx] = -1.0;
                R[i + i * n] = ((DefineConstants.CONST_AMB) * (DefineConstants.CONST_AMB));
            }
            /* update states with constraints */
            if ((info = GlobalMembersRtkcmn.filter(rtk.x, rtk.P, H, v, R, rtk.nx, n)) != 0)
            {
                GlobalMembersRtkcmn.trace(1, "filter error (info=%d)\n", info);
                //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
                free(v);
                //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
                free(H);
                //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
                free(R);
                return 0;
            }
            /* set solution */
            for (i = 0; i < rtk.na; i++)
            {
                rtk.xa[i] = rtk.x[i];
                for (j = 0; j < rtk.na; j++)
                {
                    rtk.Pa[i + j * rtk.na] = rtk.Pa[j + i * rtk.na] = rtk.P[i + j * rtk.nx];
                }
            }
            /* set flags */
            for (i = 0; i < n; i++)
            {
                rtk.ambc[sat1[i] - 1].flags[sat2[i] - 1] = 1;
                rtk.ambc[sat2[i] - 1].flags[sat1[i] - 1] = 1;
            }
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(v);
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(H);
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(R);
            return 1;
        }
        /* fix narrow-lane ambiguity by rounding -------------------------------------*/
        internal static int fix_amb_ROUND(rtk_t rtk, int[] sat1, int[] sat2, int[] NW, int n)
        {
            double C1;
            double C2;
            double B1;
            double v1;
            double BC;
            double v;
            double vc;
            double[] NC;
            double[] @var;
            double lam_NL = GlobalMembersPpp_ar.lam_LC(1, 1, 0);
            double lam1;
            double lam2;
            int i;
            int j;
            int k;
            int m = 0;
            int N1;
            int stat;

            lam1 = GlobalMembersRtkcmn.lam_carr[0];
            lam2 = GlobalMembersRtkcmn.lam_carr[1];

            C1 = ((lam2) * (lam2)) / (((lam2) * (lam2)) - ((lam1) * (lam1)));
            C2 = -((lam1) * (lam1)) / (((lam2) * (lam2)) - ((lam1) * (lam1)));

            NC = GlobalMembersRtkcmn.zeros(n, 1);
            @var = GlobalMembersRtkcmn.zeros(n, 1);

            for (i = 0; i < n; i++)
            {
                j = ((((((rtk.opt).dynamics != 0 ? 9 : 3) + (0)) + DefineConstants.NSYSGPS + DefineConstants.NSYSGLO + DefineConstants.NSYSGAL + DefineConstants.NSYSQZS + DefineConstants.NSYSCMP + DefineConstants.NSYSLEO) + ((rtk.opt).tropopt < DefineConstants.TROPOPT_EST ? 0 : ((rtk.opt).tropopt == DefineConstants.TROPOPT_EST ? 1 : 3))) + (sat1[i]) - 1);
                k = ((((((rtk.opt).dynamics != 0 ? 9 : 3) + (0)) + DefineConstants.NSYSGPS + DefineConstants.NSYSGLO + DefineConstants.NSYSGAL + DefineConstants.NSYSQZS + DefineConstants.NSYSCMP + DefineConstants.NSYSLEO) + ((rtk.opt).tropopt < DefineConstants.TROPOPT_EST ? 0 : ((rtk.opt).tropopt == DefineConstants.TROPOPT_EST ? 1 : 3))) + (sat2[i]) - 1);

                /* narrow-lane ambiguity */
                B1 = (rtk.x[j] - rtk.x[k] + C2 * lam2 * NW[i]) / lam_NL;
                N1 = (int)Math.Floor((B1) + 0.5);

                /* variance of narrow-lane ambiguity */
                @var[m] = rtk.P[j + j * rtk.nx] + rtk.P[k + k * rtk.nx] - 2.0 * rtk.P[j + k * rtk.nx];
                v1 = @var[m] / ((lam_NL) * (lam_NL));

                /* validation of narrow-lane ambiguity */
                if (Math.Abs(N1 - B1) > rtk.opt.thresar[2] || GlobalMembersPpp_ar.conffunc(N1, B1, Math.Sqrt(v1)) < rtk.opt.thresar[1])
                {
                    continue;
                }
                /* iono-free ambiguity (m) */
                BC = C1 * lam1 * N1 + C2 * lam2 * (N1 - NW[i]);

                /* check residuals */
                v = rtk.ssat[sat1[i] - 1].resc[0] - rtk.ssat[sat2[i] - 1].resc[0];
                vc = v + (BC - (rtk.x[j] - rtk.x[k]));
                if (Math.Abs(vc) > DefineConstants.THRES_RES)
                    continue;

                sat1[m] = sat1[i];
                sat2[m] = sat2[i];
                NC[m++] = BC;
            }
            /* select fixed ambiguities by dependancy check */
            m = GlobalMembersPpp_ar.sel_amb(sat1, sat2, NC, @var, m);

            /* fixed solution */
            stat = GlobalMembersPpp_ar.fix_sol(rtk, sat1, sat2, NC, m);

            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(NC);
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(@var);

            return stat && m >= 3;
        }
        /* fix narrow-lane ambiguity by ILS ------------------------------------------*/
        internal static int fix_amb_ILS(rtk_t rtk, int[] sat1, int[] sat2, int[] NW, int n)
        {
            double C1;
            double C2;
            double[] B1;
            double[] N1;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: double *NC;
            double NC;
            double[] D;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: double *E;
            double E;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: double *Q;
            double Q;
            double[] s = new double[2];
            double lam_NL = GlobalMembersPpp_ar.lam_LC(1, 1, 0);
            double lam1;
            double lam2;
            int i;
            int j;
            int k;
            int m = 0;
            int info;
            int stat;
            int[] flgs = new int[DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1];
            int max_flg = 0;

            lam1 = GlobalMembersRtkcmn.lam_carr[0];
            lam2 = GlobalMembersRtkcmn.lam_carr[1];

            C1 = ((lam2) * (lam2)) / (((lam2) * (lam2)) - ((lam1) * (lam1)));
            C2 = -((lam1) * (lam1)) / (((lam2) * (lam2)) - ((lam1) * (lam1)));

            B1 = GlobalMembersRtkcmn.zeros(n, 1);
            N1 = GlobalMembersRtkcmn.zeros(n, 2);
            D = GlobalMembersRtkcmn.zeros(rtk.nx, n);
            E = GlobalMembersRtkcmn.mat(n, rtk.nx);
            Q = GlobalMembersRtkcmn.mat(n, n);
            NC = GlobalMembersRtkcmn.mat(n, 1);

            for (i = 0; i < n; i++)
            {

                /* check linear independency */
                if (GlobalMembersPpp_ar.is_depend(sat1[i], sat2[i], flgs, ref max_flg) == 0)
                    continue;

                j = ((((((rtk.opt).dynamics != 0 ? 9 : 3) + (0)) + DefineConstants.NSYSGPS + DefineConstants.NSYSGLO + DefineConstants.NSYSGAL + DefineConstants.NSYSQZS + DefineConstants.NSYSCMP + DefineConstants.NSYSLEO) + ((rtk.opt).tropopt < DefineConstants.TROPOPT_EST ? 0 : ((rtk.opt).tropopt == DefineConstants.TROPOPT_EST ? 1 : 3))) + (sat1[i]) - 1);
                k = ((((((rtk.opt).dynamics != 0 ? 9 : 3) + (0)) + DefineConstants.NSYSGPS + DefineConstants.NSYSGLO + DefineConstants.NSYSGAL + DefineConstants.NSYSQZS + DefineConstants.NSYSCMP + DefineConstants.NSYSLEO) + ((rtk.opt).tropopt < DefineConstants.TROPOPT_EST ? 0 : ((rtk.opt).tropopt == DefineConstants.TROPOPT_EST ? 1 : 3))) + (sat2[i]) - 1);

                /* float narrow-lane ambiguity (cycle) */
                B1[m] = (rtk.x[j] - rtk.x[k] + C2 * lam2 * NW[i]) / lam_NL;
                N1[m] = (int)Math.Floor((B1[m]) + 0.5);

                /* validation of narrow-lane ambiguity */
                if (Math.Abs(N1[m] - B1[m]) > rtk.opt.thresar[2])
                    continue;

                /* narrow-lane ambiguity transformation matrix */
                D[j + m * rtk.nx] = 1.0 / lam_NL;
                D[k + m * rtk.nx] = -1.0 / lam_NL;

                sat1[m] = sat1[i];
                sat2[m] = sat2[i];
                NW[m++] = NW[i];
            }
            if (m < 3)
            {
                return 0;
            }

            /* covariance of narrow-lane ambiguities */
            GlobalMembersRtkcmn.matmul("TN", m, rtk.nx, rtk.nx, 1.0, D, rtk.P, 0.0, ref E);
            GlobalMembersRtkcmn.matmul("NN", m, m, rtk.nx, 1.0, E, D, 0.0, ref Q);

            /* integer least square */
            if ((info = GlobalMembersLambda.lambda(m, 2, B1, Q, ref N1, ref s)) != 0)
            {
                GlobalMembersRtkcmn.trace(2, "lambda error: info=%d\n", info);
                return 0;
            }
            if (s[0] <= 0.0)
            {
                return 0;
            }

            rtk.sol.ratio = (float)(((s[1] / s[0]) < (999.9) ? (s[1] / s[0]) : (999.9)));

            /* varidation by ratio-test */
            if (rtk.opt.thresar[0] > 0.0 && rtk.sol.ratio < rtk.opt.thresar[0])
            {
                GlobalMembersRtkcmn.trace(2, "varidation error: n=%2d ratio=%8.3f\n", m, rtk.sol.ratio);
                return 0;
            }
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: trace(2,"varidation ok: %s n=%2d ratio=%8.3f\n",time_str(rtk->sol.time,0),m, rtk->sol.ratio);
            GlobalMembersRtkcmn.trace(2, "varidation ok: %s n=%2d ratio=%8.3f\n", GlobalMembersRtkcmn.time_str(new gtime_t(rtk.sol.time), 0), m, rtk.sol.ratio);

            /* narrow-lane to iono-free ambiguity */
            for (i = 0; i < m; i++)
            {
                NC[i] = C1 * lam1 * N1[i] + C2 * lam2 * (N1[i] - NW[i]);
            }
            /* fixed solution */
            stat = GlobalMembersPpp_ar.fix_sol(rtk, sat1, sat2, NC, m);

            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(B1);
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(N1);
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(D);
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(E);
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(Q);
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(NC);

            return stat;
        }
        /* resolve integer ambiguity for ppp -----------------------------------------*/
        public static int pppamb(rtk_t rtk, obsd_t[] obs, int n, nav_t nav, double[] azel)
        {
            double elmask;
            int i;
            int j;
            int m = 0;
            int stat = 0;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: int *NW;
            int NW;
            int[] sat1;
            int[] sat2;

            if (n <= 0 || rtk.opt.ionoopt != DefineConstants.IONOOPT_IFLC || rtk.opt.nf < 2)
            {
                return 0;
            }

            GlobalMembersRtkcmn.trace(3, "pppamb: time=%s n=%d\n", GlobalMembersRtkcmn.time_str(obs[0].time, 0), n);

            elmask = rtk.opt.elmaskar > 0.0 ? rtk.opt.elmaskar : rtk.opt.elmin;

            sat1 = GlobalMembersRtkcmn.imat(n * n, 1);
            sat2 = GlobalMembersRtkcmn.imat(n * n, 1);
            NW = GlobalMembersRtkcmn.imat(n * n, 1);

            /* average LC */
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: average_LC(rtk,obs,n,nav,azel);
            GlobalMembersPpp_ar.average_LC(rtk, new obsd_t(obs), n, nav, azel);

            /* fix wide-lane ambiguity */
            for (i = 0; i < n - 1; i++)
            {
                for (j = i + 1; j < n; j++)
                {

                    if (rtk.ssat[obs[i].sat - 1].vsat[0] == 0 || rtk.ssat[obs[j].sat - 1].vsat[0] == 0 || azel[1 + i * 2] < elmask || azel[1 + j * 2] < elmask)
                        continue;
#if false
	// /* test already fixed */
	//        if (rtk->ambc[obs[i].sat-1].flags[obs[j].sat-1]&&
	//            rtk->ambc[obs[j].sat-1].flags[obs[i].sat-1]) continue;
#endif
                    sat1[m] = obs[i].sat;
                    sat2[m] = obs[j].sat;
                    if (GlobalMembersPpp_ar.fix_amb_WL(rtk, nav, sat1[m], sat2[m], ref NW + m) != 0)
                    {
                        m++;
                    }
                }
            }
            /* fix narrow-lane ambiguity */
            if (rtk.opt.modear == DefineConstants.ARMODE_PPPAR)
            {
                stat = GlobalMembersPpp_ar.fix_amb_ROUND(rtk, sat1, sat2, NW, m);
            }
            else if (rtk.opt.modear == DefineConstants.ARMODE_PPPAR_ILS)
            {
                stat = GlobalMembersPpp_ar.fix_amb_ILS(rtk, sat1, sat2, NW, m);
            }
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(sat1);
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(sat2);
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(NW);

            return stat;
        }
    }
}
