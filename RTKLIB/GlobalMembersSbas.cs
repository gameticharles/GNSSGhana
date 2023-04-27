using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ghGPS.Classes
{
    public static class GlobalMembersSbas
    {
        /*------------------------------------------------------------------------------
        * sbas.c : sbas functions
        *
        *          Copyright (C) 2007-2011 by T.TAKASU, All rights reserved.
        *
        * option : -DRRCENA  enable rrc correction
        *          
        * references :
        *     [1] RTCA/DO-229C, Minimum operational performanc standards for global
        *         positioning system/wide area augmentation system airborne equipment,
        *         RTCA inc, November 28, 2001
        *     [2] IS-QZSS v.1.1, Quasi-Zenith Satellite System Navigation Service
        *         Interface Specification for QZSS, Japan Aerospace Exploration Agency,
        *         July 31, 2009
        *
        * version : $Revision: 1.1 $ $Date: 2008/07/17 21:48:06 $
        * history : 2007/10/14 1.0  new
        *           2009/01/24 1.1  modify sbspntpos() api
        *                           improve fast/ion correction update
        *           2009/04/08 1.2  move function crc24q() to rcvlog.c
        *                           support glonass, galileo and qzss
        *           2009/06/08 1.3  modify sbsupdatestat()
        *                           delete sbssatpos()
        *           2009/12/12 1.4  support glonass
        *           2010/01/22 1.5  support ems (egnos message service) format
        *           2010/06/10 1.6  added api:
        *                               sbssatcorr(),sbstropcorr(),sbsioncorr(),
        *                               sbsupdatecorr()
        *                           changed api:
        *                               sbsreadmsgt(),sbsreadmsg()
        *                           deleted api:
        *                               sbspntpos(),sbsupdatestat()
        *           2010/08/16 1.7  not reject udre==14 or give==15 correction message
        *                           (2.4.0_p4)
        *           2011/01/15 1.8  use api ionppp()
        *                           add prn mask of qzss for qzss L1SAIF
        *-----------------------------------------------------------------------------*/


        internal const string rcsid = "$Id: sbas.c,v 1.1 2008/07/17 21:48:06 ttaka Exp $";

        /* constants -----------------------------------------------------------------*/


        /* sbas igp definition -------------------------------------------------------*/
        internal readonly short[] x1 = { -75, -65, -55, -50, -45, -40, -35, -30, -25, -20, -15, -10, -5, 0, 5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 65, 75, 85 };
        public static readonly short[] x2 = { -55, -50, -45, -40, -35, -30, -25, -20, -15, -10, -5, 0, 5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55 };
        public static readonly short[] x3 = { -75, -65, -55, -50, -45, -40, -35, -30, -25, -20, -15, -10, -5, 0, 5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 65, 75 };
        public static readonly short[] x4 = { -85, -75, -65, -55, -50, -45, -40, -35, -30, -25, -20, -15, -10, -5, 0, 5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 65, 75 };
        public static readonly short[] x5 = { -180, -175, -170, -165, -160, -155, -150, -145, -140, -135, -130, -125, -120, -115, -110, -105, -100, -95, -90, -85, -80, -75, -70, -65, -60, -55, -50, -45, -40, -35, -30, -25, -20, -15, -10, -5, 0, 5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60, 65, 70, 75, 80, 85, 90, 95, 100, 105, 110, 115, 120, 125, 130, 135, 140, 145, 150, 155, 160, 165, 170, 175 };
        public static readonly short[] x6 = { -180, -170, -160, -150, -140, -130, -120, -110, -100, -90, -80, -70, -60, -50, -40, -30, -20, -10, 0, 10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 110, 120, 130, 140, 150, 160, 170 };
        public static readonly short[] x7 = { -180, -150, -120, -90, -60, -30, 0, 30, 60, 90, 120, 150 };
        public static readonly short[] x8 = { -170, -140, -110, -80, -50, -20, 10, 40, 70, 100, 130, 160 };

        public static readonly sbsigpband_t[,] igpband1 = { { { -180, x1, 1, 28 }, { -175, x2, 29, 51 }, { -170, x3, 52, 78 }, { -165, x2, 79, 101 }, { -160, x3, 102, 128 }, { -155, x2, 129, 151 }, { -150, x3, 152, 178 }, { -145, x2, 179, 201 } }, { { -140, x4, 1, 28 }, { -135, x2, 29, 51 }, { -130, x3, 52, 78 }, { -125, x2, 79, 101 }, { -120, x3, 102, 128 }, { -115, x2, 129, 151 }, { -110, x3, 152, 178 }, { -105, x2, 179, 201 } }, { { -100, x3, 1, 27 }, { -95, x2, 28, 50 }, { -90, x1, 51, 78 }, { -85, x2, 79, 101 }, { -80, x3, 102, 128 }, { -75, x2, 129, 151 }, { -70, x3, 152, 178 }, { -65, x2, 179, 201 } }, { { -60, x3, 1, 27 }, { -55, x2, 28, 50 }, { -50, x4, 51, 78 }, { -45, x2, 79, 101 }, { -40, x3, 102, 128 }, { -35, x2, 129, 151 }, { -30, x3, 152, 178 }, { -25, x2, 179, 201 } }, { { -20, x3, 1, 27 }, { -15, x2, 28, 50 }, { -10, x3, 51, 77 }, { -5, x2, 78, 100 }, { 0, x1, 101, 128 }, { 5, x2, 129, 151 }, { 10, x3, 152, 178 }, { 15, x2, 179, 201 } }, { { 20, x3, 1, 27 }, { 25, x2, 28, 50 }, { 30, x3, 51, 77 }, { 35, x2, 78, 100 }, { 40, x4, 101, 128 }, { 45, x2, 129, 151 }, { 50, x3, 152, 178 }, { 55, x2, 179, 201 } }, { { 60, x3, 1, 27 }, { 65, x2, 28, 50 }, { 70, x3, 51, 77 }, { 75, x2, 78, 100 }, { 80, x3, 101, 127 }, { 85, x2, 128, 150 }, { 90, x1, 151, 178 }, { 95, x2, 179, 201 } }, { { 100, x3, 1, 27 }, { 105, x2, 28, 50 }, { 110, x3, 51, 77 }, { 115, x2, 78, 100 }, { 120, x3, 101, 127 }, { 125, x2, 128, 150 }, { 130, x4, 151, 178 }, { 135, x2, 179, 201 } }, { { 140, x3, 1, 27 }, { 145, x2, 28, 50 }, { 150, x3, 51, 77 }, { 155, x2, 78, 100 }, { 160, x3, 101, 127 }, { 165, x2, 128, 150 }, { 170, x3, 151, 177 }, { 175, x2, 178, 200 } } }; // band 0-8
        public static readonly sbsigpband_t[,] igpband2 = { { { 60, x5, 1, 72 }, { 65, x6, 73, 108 }, { 70, x6, 109, 144 }, { 75, x6, 145, 180 }, { 85, x7, 181, 192 } }, { { -60, x5, 1, 72 }, { -65, x6, 73, 108 }, { -70, x6, 109, 144 }, { -75, x6, 145, 180 }, { -85, x8, 181, 192 } } }; // band 9-10
                                                                                                                                                                                                                                                                                               /* extract field from line ---------------------------------------------------*/
                                                                                                                                                                                                                                                                                               //C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on the parameter 'p', so pointers on this parameter are left unchanged:
        internal static string getfield(sbyte* p, int pos)
        {
            for (pos--; pos > 0; pos--, p++)
            {
                if ((p = StringFunctions.StrChr(p, ',')) == 0)
                {
                    return null;
                }
            }
            return p;
        }
        /* variance of fast correction (udre=UDRE+1) ---------------------------------*/
        internal static double varfcorr(int udre)
        {
            double[] @var = { 0.052, 0.0924, 0.1444, 0.283, 0.4678, 0.8315, 1.2992, 1.8709, 2.5465, 3.326, 5.1968, 20.7870, 230.9661, 2078.695 };
            return 0 < udre && udre <= 14 ? @var[udre - 1] : 0.0;
        }
        /* variance of ionosphere correction (give=GIVEI+1) --------------------------*/
        internal static double varicorr(int give)
        {
            double[] @var = { 0.0084, 0.0333, 0.0749, 0.1331, 0.2079, 0.2994, 0.4075, 0.5322, 0.6735, 0.8315, 1.1974, 1.8709, 3.326, 20.787, 187.0826 };
            return 0 < give && give <= 15 ? @var[give - 1] : 0.0;
        }
        /* fast correction degradation -----------------------------------------------*/
        internal static double degfcorr(int ai)
        {
            double[] degf = { 0.00000, 0.00005, 0.00009, 0.00012, 0.00015, 0.00020, 0.00030, 0.00045, 0.00060, 0.00090, 0.00150, 0.00210, 0.00270, 0.00330, 0.00460, 0.00580 };
            return 0 < ai && ai <= 15 ? degf[ai] : 0.0058;
        }
        /* decode type 1: prn masks --------------------------------------------------*/
        internal static int decode_sbstype1(sbsmsg_t msg, sbssat_t sbssat)
        {
            int i;
            int n;
            int sat;

            GlobalMembersRtkcmn.trace(4, "decode_sbstype1:\n");

            for (i = 1, n = 0; i <= 210 && n < DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1; i++)
            {
                if (GlobalMembersRtkcmn.getbitu(msg.msg, 13 + i, 1) != 0)
                {
                    if (i <= 37) //   0- 37: gps
                    {
                        sat = GlobalMembersRtkcmn.satno(DefineConstants.SYS_GPS, i);
                    }
                    else if (i <= 61) //  38- 61: glonass
                    {
                        sat = GlobalMembersRtkcmn.satno(DefineConstants.SYS_GLO, i - 37);
                    }
                    else if (i <= 119) //  62-119: future gnss
                    {
                        sat = 0;
                    }
                    else if (i <= 138) // 120-138: geo/waas
                    {
                        sat = GlobalMembersRtkcmn.satno(DefineConstants.SYS_SBS, i);
                    }
                    else if (i <= 182) // 139-182: reserved
                    {
                        sat = 0;
                    }
                    else if (i <= 192) // 183-192: qzss ref [2]
                    {
                        sat = GlobalMembersRtkcmn.satno(DefineConstants.SYS_SBS, i + 10);
                    }
                    else if (i <= 202) // 193-202: qzss ref [2]
                    {
                        sat = GlobalMembersRtkcmn.satno(DefineConstants.SYS_QZS, i);
                    }
                    else // 203-   : reserved
                    {
                        sat = 0;
                    }
                    sbssat.sat[n++].sat = sat;
                }
            }
            sbssat.iodp = GlobalMembersRtkcmn.getbitu(msg.msg, 224, 2);
            sbssat.nsat = n;

            GlobalMembersRtkcmn.trace(5, "decode_sbstype1: nprn=%d iodp=%d\n", n, sbssat.iodp);
            return 1;
        }
        /* decode type 2-5,0: fast corrections ---------------------------------------*/
        internal static int decode_sbstype2(sbsmsg_t msg, sbssat_t sbssat)
        {
            int i;
            int j;
            int iodf;
            int type;
            int udre;
            double prc;
            double dt;
            gtime_t t0 = new gtime_t();

            GlobalMembersRtkcmn.trace(4, "decode_sbstype2:\n");

            if (sbssat.iodp != (int)GlobalMembersRtkcmn.getbitu(msg.msg, 16, 2))
            {
                return 0;
            }

            type = GlobalMembersRtkcmn.getbitu(msg.msg, 8, 6);
            iodf = GlobalMembersRtkcmn.getbitu(msg.msg, 14, 2);

            for (i = 0; i < 13; i++)
            {
                if ((j = 13 * ((type == 0 ? 2 : type) - 2) + i) >= sbssat.nsat)
                    break;
                udre = GlobalMembersRtkcmn.getbitu(msg.msg, 174 + 4 * i, 4);
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                //ORIGINAL LINE: t0 =sbssat->sat[j].fcorr.t0;
                t0.CopyFrom(sbssat.sat[j].fcorr.t0);
                prc = sbssat.sat[j].fcorr.prc;
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                //ORIGINAL LINE: sbssat->sat[j].fcorr.t0=gpst2time(msg->week,msg->tow);
                sbssat.sat[j].fcorr.t0.CopyFrom(GlobalMembersRtkcmn.gpst2time(msg.week, msg.tow));
                sbssat.sat[j].fcorr.prc = GlobalMembersRtkcmn.getbits(msg.msg, 18 + i * 12, 12) * 0.125f;
                sbssat.sat[j].fcorr.udre = udre + 1;
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: dt=timediff(sbssat->sat[j].fcorr.t0,t0);
                dt = GlobalMembersRtkcmn.timediff(new gtime_t(sbssat.sat[j].fcorr.t0), new gtime_t(t0));
                if (t0.time == 0 || dt <= 0.0 || 18.0 < dt || sbssat.sat[j].fcorr.ai == 0)
                {
                    sbssat.sat[j].fcorr.rrc = 0.0;
                    sbssat.sat[j].fcorr.dt = 0.0;
                }
                else
                {
                    sbssat.sat[j].fcorr.rrc = (sbssat.sat[j].fcorr.prc - prc) / dt;
                    sbssat.sat[j].fcorr.dt = dt;
                }
                sbssat.sat[j].fcorr.iodf = iodf;
            }
            GlobalMembersRtkcmn.trace(5, "decode_sbstype2: type=%d iodf=%d\n", type, iodf);
            return 1;
        }
        /* decode type 6: integrity info ---------------------------------------------*/
        internal static int decode_sbstype6(sbsmsg_t msg, sbssat_t sbssat)
        {
            int i;
            int[] iodf = new int[4];
            int udre;

            GlobalMembersRtkcmn.trace(4, "decode_sbstype6:\n");

            for (i = 0; i < 4; i++)
            {
                iodf[i] = GlobalMembersRtkcmn.getbitu(msg.msg, 14 + i * 2, 2);
            }
            for (i = 0; i < sbssat.nsat && i < DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1; i++)
            {
                if (sbssat.sat[i].fcorr.iodf != iodf[i / 13])
                    continue;
                udre = GlobalMembersRtkcmn.getbitu(msg.msg, 22 + i * 4, 4);
                sbssat.sat[i].fcorr.udre = udre + 1;
            }
            GlobalMembersRtkcmn.trace(5, "decode_sbstype6: iodf=%d %d %d %d\n", iodf[0], iodf[1], iodf[2], iodf[3]);
            return 1;
        }
        /* decode type 7: fast correction degradation factor -------------------------*/
        internal static int decode_sbstype7(sbsmsg_t msg, sbssat_t sbssat)
        {
            int i;

            GlobalMembersRtkcmn.trace(4, "decode_sbstype7\n");

            if (sbssat.iodp != (int)GlobalMembersRtkcmn.getbitu(msg.msg, 18, 2))
            {
                return 0;
            }

            sbssat.tlat = GlobalMembersRtkcmn.getbitu(msg.msg, 14, 4);

            for (i = 0; i < sbssat.nsat && i < DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1; i++)
            {
                sbssat.sat[i].fcorr.ai = GlobalMembersRtkcmn.getbitu(msg.msg, 22 + i * 4, 4);
            }
            return 1;
        }
        /* decode type 9: geo navigation message -------------------------------------*/
        internal static int decode_sbstype9(sbsmsg_t msg, nav_t nav)
        {
            seph_t seph = new seph_t();
            int i;
            int sat;
            int t;

            GlobalMembersRtkcmn.trace(4, "decode_sbstype9:\n");

            if ((sat = GlobalMembersRtkcmn.satno(DefineConstants.SYS_SBS, msg.prn)) == 0)
            {
                GlobalMembersRtkcmn.trace(2, "invalid prn in sbas type 9: prn=%3d\n", msg.prn);
                return 0;
            }
            t = (int)GlobalMembersRtkcmn.getbitu(msg.msg, 22, 13) * 16 - (int)msg.tow % 86400;
            if (t <= -43200)
            {
                t += 86400;
            }
            else if (t > 43200)
            {
                t -= 86400;
            }
            seph.sat = sat;
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: seph.t0 =gpst2time(msg->week,msg->tow+t);
            seph.t0.CopyFrom(GlobalMembersRtkcmn.gpst2time(msg.week, msg.tow + t));
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: seph.tof=gpst2time(msg->week,msg->tow);
            seph.tof.CopyFrom(GlobalMembersRtkcmn.gpst2time(msg.week, msg.tow));
            seph.sva = GlobalMembersRtkcmn.getbitu(msg.msg, 35, 4);
            seph.svh = seph.sva == 15 ? 1 : 0; // unhealthy if ura==15

            seph.pos[0] = GlobalMembersRtkcmn.getbits(msg.msg, 39, 30) * 0.08;
            seph.pos[1] = GlobalMembersRtkcmn.getbits(msg.msg, 69, 30) * 0.08;
            seph.pos[2] = GlobalMembersRtkcmn.getbits(msg.msg, 99, 25) * 0.4;
            seph.vel[0] = GlobalMembersRtkcmn.getbits(msg.msg, 124, 17) * 0.000625;
            seph.vel[1] = GlobalMembersRtkcmn.getbits(msg.msg, 141, 17) * 0.000625;
            seph.vel[2] = GlobalMembersRtkcmn.getbits(msg.msg, 158, 18) * 0.004;
            seph.acc[0] = GlobalMembersRtkcmn.getbits(msg.msg, 176, 10) * 0.0000125;
            seph.acc[1] = GlobalMembersRtkcmn.getbits(msg.msg, 186, 10) * 0.0000125;
            seph.acc[2] = GlobalMembersRtkcmn.getbits(msg.msg, 196, 10) * 0.0000625;

            seph.af0 = GlobalMembersRtkcmn.getbits(msg.msg, 206, 12) * DefineConstants.P2_31;
            seph.af1 = GlobalMembersRtkcmn.getbits(msg.msg, 218, 8) * DefineConstants.P2_39 / 2.0;

            i = msg.prn - DefineConstants.MINPRNSBS;
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: if (!nav->seph||fabs(timediff(nav->seph[i].t0,seph.t0))<1E-3)
            if (nav.seph == null || Math.Abs(GlobalMembersRtkcmn.timediff(nav.seph[i].t0, new gtime_t(seph.t0))) < 1E-3) // not change
            {
                return 0;
            }
            nav.seph[DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + i] = nav.seph[i]; // previous
            nav.seph[i] = seph; // current

            GlobalMembersRtkcmn.trace(5, "decode_sbstype9: prn=%d\n", msg.prn);
            return 1;
        }
        /* decode type 18: ionospheric grid point masks ------------------------------*/
        internal static int decode_sbstype18(sbsmsg_t msg, sbsion_t[] sbsion)
        {
            sbsigpband_t p;
            int i;
            int j;
            int n;
            int m;
            int band = GlobalMembersRtkcmn.getbitu(msg.msg, 18, 4);

            GlobalMembersRtkcmn.trace(4, "decode_sbstype18:\n");

            if (0 <= band && band <= 8)
            {
                p = igpband1[band];
                m = 8;
            }
            else if (9 <= band && band <= 10)
            {
                p = igpband2[band - 9];
                m = 5;
            }
            else
            {
                return 0;
            }

            sbsion[band].iodi = (short)GlobalMembersRtkcmn.getbitu(msg.msg, 22, 2);

            for (i = 1, n = 0; i <= 201; i++)
            {
                if (GlobalMembersRtkcmn.getbitu(msg.msg, 23 + i, 1) == 0)
                    continue;
                for (j = 0; j < m; j++)
                {
                    if (i < p[j].bits || p[j].bite < i)
                        continue;
                    sbsion[band].igp[n].lat = band <= 8 ? p[j].y[i - p[j].bits] : p[j].x;
                    sbsion[band].igp[n++].lon = band <= 8 ? p[j].x : p[j].y[i - p[j].bits];
                    break;
                }
            }
            sbsion[band].nigp = n;

            GlobalMembersRtkcmn.trace(5, "decode_sbstype18: band=%d nigp=%d\n", band, n);
            return 1;
        }
        /* decode half long term correction (vel code=0) -----------------------------*/
        internal static int decode_longcorr0(sbsmsg_t msg, int p, sbssat_t sbssat)
        {
            int i;
            int n = GlobalMembersRtkcmn.getbitu(msg.msg, p, 6);

            GlobalMembersRtkcmn.trace(4, "decode_longcorr0:\n");

            if (n == 0 || n > DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1)
            {
                return 0;
            }

            sbssat.sat[n - 1].lcorr.iode = GlobalMembersRtkcmn.getbitu(msg.msg, p + 6, 8);

            for (i = 0; i < 3; i++)
            {
                sbssat.sat[n - 1].lcorr.dpos[i] = GlobalMembersRtkcmn.getbits(msg.msg, p + 14 + 9 * i, 9) * 0.125;
                sbssat.sat[n - 1].lcorr.dvel[i] = 0.0;
            }
            sbssat.sat[n - 1].lcorr.daf0 = GlobalMembersRtkcmn.getbits(msg.msg, p + 41, 10) * DefineConstants.P2_31;
            sbssat.sat[n - 1].lcorr.daf1 = 0.0;
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: sbssat->sat[n-1].lcorr.t0=gpst2time(msg->week,msg->tow);
            sbssat.sat[n - 1].lcorr.t0.CopyFrom(GlobalMembersRtkcmn.gpst2time(msg.week, msg.tow));

            GlobalMembersRtkcmn.trace(5, "decode_longcorr0:sat=%2d\n", sbssat.sat[n - 1].sat);
            return 1;
        }
        /* decode half long term correction (vel code=1) -----------------------------*/
        internal static int decode_longcorr1(sbsmsg_t msg, int p, sbssat_t sbssat)
        {
            int i;
            int n = GlobalMembersRtkcmn.getbitu(msg.msg, p, 6);
            int t;

            GlobalMembersRtkcmn.trace(4, "decode_longcorr1:\n");

            if (n == 0 || n > DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1)
            {
                return 0;
            }

            sbssat.sat[n - 1].lcorr.iode = GlobalMembersRtkcmn.getbitu(msg.msg, p + 6, 8);

            for (i = 0; i < 3; i++)
            {
                sbssat.sat[n - 1].lcorr.dpos[i] = GlobalMembersRtkcmn.getbits(msg.msg, p + 14 + i * 11, 11) * 0.125;
                sbssat.sat[n - 1].lcorr.dvel[i] = GlobalMembersRtkcmn.getbits(msg.msg, p + 58 + i * 8, 8) * 4.882812500000000E-0x4;
            }
            sbssat.sat[n - 1].lcorr.daf0 = GlobalMembersRtkcmn.getbits(msg.msg, p + 47, 11) * DefineConstants.P2_31;
            sbssat.sat[n - 1].lcorr.daf1 = GlobalMembersRtkcmn.getbits(msg.msg, p + 82, 8) * DefineConstants.P2_39;
            t = (int)GlobalMembersRtkcmn.getbitu(msg.msg, p + 90, 13) * 16 - (int)msg.tow % 86400;
            if (t <= -43200)
            {
                t += 86400;
            }
            else if (t > 43200)
            {
                t -= 86400;
            }
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: sbssat->sat[n-1].lcorr.t0=gpst2time(msg->week,msg->tow+t);
            sbssat.sat[n - 1].lcorr.t0.CopyFrom(GlobalMembersRtkcmn.gpst2time(msg.week, msg.tow + t));

            GlobalMembersRtkcmn.trace(5, "decode_longcorr1: sat=%2d\n", sbssat.sat[n - 1].sat);
            return 1;
        }
        /* decode half long term correction ------------------------------------------*/
        internal static int decode_longcorrh(sbsmsg_t msg, int p, sbssat_t sbssat)
        {
            GlobalMembersRtkcmn.trace(4, "decode_longcorrh:\n");

            if (GlobalMembersRtkcmn.getbitu(msg.msg, p, 1) == 0) // vel code=0
            {
                if (sbssat.iodp == (int)GlobalMembersRtkcmn.getbitu(msg.msg, p + 103, 2))
                {
                    return GlobalMembersSbas.decode_longcorr0(msg, p + 1, sbssat) && GlobalMembersSbas.decode_longcorr0(msg, p + 52, sbssat) != 0;
                }
            }
            else if (sbssat.iodp == (int)GlobalMembersRtkcmn.getbitu(msg.msg, p + 104, 2))
            {
                return GlobalMembersSbas.decode_longcorr1(msg, p + 1, sbssat);
            }
            return 0;
        }
        /* decode type 24: mixed fast/long term correction ---------------------------*/
        internal static int decode_sbstype24(sbsmsg_t msg, sbssat_t sbssat)
        {
            int i;
            int j;
            int iodf;
            int blk;
            int udre;

            GlobalMembersRtkcmn.trace(4, "decode_sbstype24:\n");

            if (sbssat.iodp != (int)GlobalMembersRtkcmn.getbitu(msg.msg, 110, 2)) // check IODP
            {
                return 0;
            }

            blk = GlobalMembersRtkcmn.getbitu(msg.msg, 112, 2);
            iodf = GlobalMembersRtkcmn.getbitu(msg.msg, 114, 2);

            for (i = 0; i < 6; i++)
            {
                if ((j = 13 * blk + i) >= sbssat.nsat)
                    break;
                udre = GlobalMembersRtkcmn.getbitu(msg.msg, 86 + 4 * i, 4);

                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                //ORIGINAL LINE: sbssat->sat[j].fcorr.t0 =gpst2time(msg->week,msg->tow);
                sbssat.sat[j].fcorr.t0.CopyFrom(GlobalMembersRtkcmn.gpst2time(msg.week, msg.tow));
                sbssat.sat[j].fcorr.prc = GlobalMembersRtkcmn.getbits(msg.msg, 14 + i * 12, 12) * 0.125f;
                sbssat.sat[j].fcorr.udre = udre + 1;
                sbssat.sat[j].fcorr.iodf = iodf;
            }
            return GlobalMembersSbas.decode_longcorrh(msg, 120, sbssat);
        }
        /* decode type 25: long term satellite error correction ----------------------*/
        internal static int decode_sbstype25(sbsmsg_t msg, sbssat_t sbssat)
        {
            GlobalMembersRtkcmn.trace(4, "decode_sbstype25:\n");

            return GlobalMembersSbas.decode_longcorrh(msg, 14, sbssat) && GlobalMembersSbas.decode_longcorrh(msg, 120, sbssat) != 0;
        }
        /* decode type 26: ionospheric deley corrections -----------------------------*/
        internal static int decode_sbstype26(sbsmsg_t msg, sbsion_t[] sbsion)
        {
            int i;
            int j;
            int block;
            int delay;
            int give;
            int band = GlobalMembersRtkcmn.getbitu(msg.msg, 14, 4);

            GlobalMembersRtkcmn.trace(4, "decode_sbstype26:\n");

            if (band > DefineConstants.MAXBAND || sbsion[band].iodi != (int)GlobalMembersRtkcmn.getbitu(msg.msg, 217, 2))
            {
                return 0;
            }

            block = GlobalMembersRtkcmn.getbitu(msg.msg, 18, 4);

            for (i = 0; i < 15; i++)
            {
                if ((j = block * 15 + i) >= sbsion[band].nigp)
                    continue;
                give = GlobalMembersRtkcmn.getbitu(msg.msg, 22 + i * 13 + 9, 4);

                delay = GlobalMembersRtkcmn.getbitu(msg.msg, 22 + i * 13, 9);
                sbsion[band].igp[j].t0 = GlobalMembersRtkcmn.gpst2time(msg.week, msg.tow);
                sbsion[band].igp[j].delay = delay == 0x1FF ? 0.0f : delay * 0.125f;
                sbsion[band].igp[j].give = give + 1;

                if (sbsion[band].igp[j].give >= 16)
                {
                    sbsion[band].igp[j].give = 0;
                }
            }
            GlobalMembersRtkcmn.trace(5, "decode_sbstype26: band=%d block=%d\n", band, block);
            return 1;
        }
        /* update sbas corrections -----------------------------------------------------
        * update sbas correction parameters in navigation data with a sbas message
        * args   : sbsmg_t  *msg    I   sbas message
        *          nav_t    *nav    IO  navigation data
        * return : message type (-1: error or not supported type)
        * notes  : nav->seph must point to seph[NSATSBS*2] (array of seph_t)
        *               seph[prn-MINPRNSBS+1]          : sat prn current epehmeris 
        *               seph[prn-MINPRNSBS+1+MAXPRNSBS]: sat prn previous epehmeris 
        *-----------------------------------------------------------------------------*/
        public static int sbsupdatecorr(sbsmsg_t msg, nav_t nav)
        {
            int type = GlobalMembersRtkcmn.getbitu(msg.msg, 8, 6);
            int stat = -1;

            GlobalMembersRtkcmn.trace(3, "sbsupdatecorr: type=%d\n", type);

            if (msg.week == 0)
            {
                return -1;
            }

            switch (type)
            {
                case 0:
                    stat = GlobalMembersSbas.decode_sbstype2(msg, nav.sbssat);
                    break;
                case 1:
                    stat = GlobalMembersSbas.decode_sbstype1(msg, nav.sbssat);
                    break;
                case 2:
                case 3:
                case 4:
                case 5:
                    stat = GlobalMembersSbas.decode_sbstype2(msg, nav.sbssat);
                    break;
                case 6:
                    stat = GlobalMembersSbas.decode_sbstype6(msg, nav.sbssat);
                    break;
                case 7:
                    stat = GlobalMembersSbas.decode_sbstype7(msg, nav.sbssat);
                    break;
                case 9:
                    stat = GlobalMembersSbas.decode_sbstype9(msg, nav);
                    break;
                case 18:
                    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                    //ORIGINAL LINE: stat=decode_sbstype18(msg,nav->sbsion);
                    stat = GlobalMembersSbas.decode_sbstype18(msg, new sbsion_t(nav.sbsion));
                    break;
                case 24:
                    stat = GlobalMembersSbas.decode_sbstype24(msg, nav.sbssat);
                    break;
                case 25:
                    stat = GlobalMembersSbas.decode_sbstype25(msg, nav.sbssat);
                    break;
                case 26:
                    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                    //ORIGINAL LINE: stat=decode_sbstype26(msg,nav->sbsion);
                    stat = GlobalMembersSbas.decode_sbstype26(msg, new sbsion_t(nav.sbsion));
                    break;
                case 63: // null message
                    break;

                    /*default: trace(2,"unsupported sbas message: type=%d\n",type); break;*/
            }
            return stat != 0 ? type : -1;
        }
        /* read sbas log file --------------------------------------------------------*/
        internal static void readmsgs(string file, int sel, gtime_t ts, gtime_t te, sbs_t sbs)
        {
            sbsmsg_t sbs_msgs;
            int i;
            int week;
            int prn;
            int ch;
            int msg;
            uint b;
            double tow;
            double[] ep = { 0, null, null, null, null, null };
            string buff = new string(new char[256]);
            string p;
            gtime_t time = new gtime_t();
            FILE fp;

            GlobalMembersRtkcmn.trace(3, "readmsgs: file=%s sel=%d\n", file, sel);

            if ((fp = fopen(file, "r")) == null)
            {
                GlobalMembersRtkcmn.trace(2, "sbas message file open error: %s\n", file);
                return;
            }
            while (fgets(buff, sizeof(sbyte), fp))
            {
                if (sscanf(buff, "%d %lf %d", week, tow, prn) == 3 && (p = StringFunctions.StrStr(buff, ": ")) != 0)
                {
                    p += 2; // rtklib form
                }
                else if (sscanf(buff, "%d %lf %lf %lf %lf %lf %lf %d", prn, ep, ep + 1, ep + 2, ep + 3, ep + 4, ep + 5, msg) == 8)
                {
                    /* ems (EGNOS Message Service) form */
                    ep[0] += ep[0] < 70.0 ? 2000.0 : 1900.0;
                    tow = GlobalMembersRtkcmn.time2gpst(GlobalMembersRtkcmn.epoch2time(ep), ref week);
                    p = buff + (msg >= 10 ? 25 : 24);
                }
                else if (!string.Compare(buff, 0, "#RAWWAASFRAMEA", 0, 14)) // NovAtel OEM4/V
                {
                    if ((p = GlobalMembersSbas.getfield(buff, 6)) == 0)
                        continue;
                    if (sscanf(p, "%d,%lf", week, tow) < 2)
                        continue;
                    if ((p = StringFunctions.StrChr(p.Substring(p), ';')) == 0)
                        continue;
                    if (sscanf(++p, "%d,%d", ch, prn) < 2)
                        continue;
                    if ((p = GlobalMembersSbas.getfield(p, 4)) == 0)
                        continue;
                }
                else if (!string.Compare(buff, 0, "$FRMA", 0, 5)) // NovAtel OEM3
                {
                    if ((p = GlobalMembersSbas.getfield(buff, 2)) == 0)
                        continue;
                    if (sscanf(p, "%d,%lf,%d", week, tow, prn) < 3)
                        continue;
                    if ((p = GlobalMembersSbas.getfield(p, 6)) == 0)
                        continue;
                    if (week < DefineConstants.WEEKOFFSET)
                    {
                        week += DefineConstants.WEEKOFFSET;
                    }
                }
                else
                    continue;

                if (sel != 0 && sel != prn)
                    continue;

                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                //ORIGINAL LINE: time=gpst2time(week,tow);
                time.CopyFrom(GlobalMembersRtkcmn.gpst2time(week, tow));

                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: if (!screent(time,ts,te,0.0))
                if (GlobalMembersRtkcmn.screent(new gtime_t(time), new gtime_t(ts), new gtime_t(te), 0.0) == 0)
                    continue;

                if (sbs.n >= sbs.nmax)
                {
                    sbs.nmax = sbs.nmax == 0 ? 1024 : sbs.nmax * 2;
                    //C++ TO C# CONVERTER TODO TASK: The memory management function 'realloc' has no equivalent in C#:
                    if ((sbs_msgs = (sbsmsg_t)realloc(sbs.msgs, sbs.nmax * sizeof(sbsmsg_t))) == null)
                    {
                        GlobalMembersRtkcmn.trace(1, "readsbsmsg malloc error: nmax=%d\n", sbs.nmax);
                        //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
                        free(sbs.msgs);
                        sbs.msgs = null;
                        sbs.n = sbs.nmax = 0;
                        return;
                    }
                    sbs.msgs = sbs_msgs;
                }
                sbs.msgs[sbs.n].week = week;
                sbs.msgs[sbs.n].tow = (int)(tow + 0.5);
                sbs.msgs[sbs.n].prn = prn;
                for (i = 0; i < 29; i++)
                {
                    sbs.msgs[sbs.n].msg[i] = 0;
                }
                for (i = 0; *(p - 1) && p != 0 && i < 29; p += 2, i++)
                {
                    if (sscanf(p, "%2X", b) == 1)
                    {
                        sbs.msgs[sbs.n].msg[i] = (byte)b;
                    }
                }
                sbs.msgs[sbs.n++].msg[28] &= 0xC0;
            }
            fclose(fp);
        }
        /* compare sbas messages -----------------------------------------------------*/
        internal static int cmpmsgs(object p1, object p2)
        {
            sbsmsg_t q1 = (sbsmsg_t)p1;
            sbsmsg_t q2 = (sbsmsg_t)p2;
            return q1.week != q2.week != 0 ? q1.week - q2.week : (q1.tow < q2.tow != 0 ? -1 : (q1.tow > q2.tow != 0 ? 1 : q1.prn - q2.prn));
        }
        /* read sbas message file ------------------------------------------------------
        * read sbas message file
        * args   : char     *file   I   sbas message file (wind-card * is expanded)
        *          int      sel     I   sbas satellite prn number selection (0:all)
        *         (gtime_t  ts      I   start time)
        *         (gtime_t  te      I   end time  )
        *          sbs_t    *sbs    IO  sbas messages
        * return : number of sbas messages
        * notes  : sbas message are appended and sorted. before calling the funciton, 
        *          sbs->n, sbs->nmax and sbs->msgs must be set properly. (initially
        *          sbs->n=sbs->nmax=0, sbs->msgs=NULL)
        *          only the following file extentions after wild card expanded are valid
        *          to read. others are skipped
        *          .sbs, .SBS, .ems, .EMS
        *-----------------------------------------------------------------------------*/
        public static int sbsreadmsgt(string file, int sel, gtime_t ts, gtime_t te, sbs_t sbs)
        {
            string[] efiles = { 0 };
            string ext;
            int i;
            int n;

            GlobalMembersRtkcmn.trace(3, "sbsreadmsgt: file=%s sel=%d\n", file, sel);

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
            /* expand wild card in file path */
            n = GlobalMembersRtkcmn.expath(file, efiles, DefineConstants.MAXEXFILE);

            for (i = 0; i < n; i++)
            {
                if ((ext = StringFunctions.StrRChr(efiles[i], '.')) == 0)
                    continue;
                if (string.Compare(ext, ".sbs") && string.Compare(ext, ".SBS") && string.Compare(ext, ".ems") && string.Compare(ext, ".EMS"))
                    continue;

                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: readmsgs(efiles[i],sel,ts,te,sbs);
                GlobalMembersSbas.readmsgs(efiles[i], sel, new gtime_t(ts), new gtime_t(te), sbs);
            }
            for (i = 0; i < DefineConstants.MAXEXFILE; i++)
            {
                //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
                free(efiles[i]);
            }

            /* sort messages */
            if (sbs.n > 0)
            {
                qsort(sbs.msgs, sbs.n, sizeof(sbsmsg_t), GlobalMembersSbas.cmpmsgs);
            }
            return sbs.n;
        }
        public static int sbsreadmsg(string file, int sel, sbs_t sbs)
        {
            gtime_t ts = new gtime_t();
            gtime_t te = new gtime_t();

            GlobalMembersRtkcmn.trace(3, "sbsreadmsg: file=%s sel=%d\n", file, sel);

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: return sbsreadmsgt(file,sel,ts,te,sbs);
            return GlobalMembersSbas.sbsreadmsgt(file, sel, new gtime_t(ts), new gtime_t(te), sbs);
        }
        /* output sbas messages --------------------------------------------------------
        * output sbas message record to output file in rtklib sbas log format
        * args   : FILE   *fp       I   output file pointer
        *          sbsmsg_t *sbsmsg I   sbas messages
        * return : none
        *-----------------------------------------------------------------------------*/
        public static void sbsoutmsg(FILE fp, sbsmsg_t sbsmsg)
        {
            int i;
            int type = sbsmsg.msg[1] >> 2;

            GlobalMembersRtkcmn.trace(4, "sbsoutmsg:\n");

            fprintf(fp, "%4d %6d %3d %2d : ", sbsmsg.week, sbsmsg.tow, sbsmsg.prn, type);
            for (i = 0; i < 29; i++)
            {
                fprintf(fp, "%02X", sbsmsg.msg[i]);
            }
            fprintf(fp, "\n");
        }
        /* search igps ---------------------------------------------------------------*/
        internal static void searchigp(gtime_t time, double[] pos, sbsion_t[] ion, sbsigp_t[][] igp, ref double x, ref double y)
        {
            int i;
            int[] latp = new int[2];
            int[] lonp = new int[4];
            double lat = pos[0] * 180.0 / DefineConstants.PI;
            double lon = pos[1] * 180.0 / DefineConstants.PI;
            sbsigp_t p;

            GlobalMembersRtkcmn.trace(4, "searchigp: pos=%.3f %.3f\n", pos[0] * 180.0 / DefineConstants.PI, pos[1] * 180.0 / DefineConstants.PI);

            if (lon >= 180.0)
            {
                lon -= 360.0;
            }
            if (-55.0 <= lat && lat < 55.0)
            {
                latp[0] = (int)Math.Floor(lat / 5.0) * 5;
                latp[1] = latp[0] + 5;
                lonp[0] = lonp[1] = (int)Math.Floor(lon / 5.0) * 5;
                lonp[2] = lonp[3] = lonp[0] + 5;
                x = (lon - lonp[0]) / 5.0;
                y = (lat - latp[0]) / 5.0;
            }
            else
            {
                latp[0] = (int)Math.Floor((lat - 5.0) / 10.0) * 10 + 5;
                latp[1] = latp[0] + 10;
                lonp[0] = lonp[1] = (int)Math.Floor(lon / 10.0) * 10;
                lonp[2] = lonp[3] = lonp[0] + 10;
                x = (lon - lonp[0]) / 10.0;
                y = (lat - latp[0]) / 10.0;
                if (75.0 <= lat && lat < 85.0)
                {
                    lonp[1] = (int)Math.Floor(lon / 90.0) * 90;
                    lonp[3] = lonp[1] + 90;
                }
                else if (-85.0 <= lat && lat < -75.0)
                {
                    lonp[0] = (int)Math.Floor((lon - 50.0) / 90.0) * 90 + 40;
                    lonp[2] = lonp[0] + 90;
                }
                else if (lat >= 85.0)
                {
                    for (i = 0; i < 4; i++)
                    {
                        lonp[i] = (int)Math.Floor(lon / 90.0) * 90;
                    }
                }
                else if (lat < -85.0)
                {
                    for (i = 0; i < 4; i++)
                    {
                        lonp[i] = (int)Math.Floor((lon - 50.0) / 90.0) * 90 + 40;
                    }
                }
            }
            for (i = 0; i < 4; i++)
            {
                if (lonp[i] == 180)
                {
                    lonp[i] = -180;
                }
            }
            for (i = 0; i <= DefineConstants.MAXBAND; i++)
            {
                for (p = ion[i].igp; p < ion[i].igp + ion[i].nigp; p++)
                {
                    if (p.t0.time == 0)
                        continue;
                    if (p.lat == latp[0] && p.lon == lonp[0] && p.give > 0)
                    {
                        igp[0] = p;
                    }
                    else if (p.lat == latp[1] && p.lon == lonp[1] && p.give > 0)
                    {
                        igp[1] = p;
                    }
                    else if (p.lat == latp[0] && p.lon == lonp[2] && p.give > 0)
                    {
                        igp[2] = p;
                    }
                    else if (p.lat == latp[1] && p.lon == lonp[3] && p.give > 0)
                    {
                        igp[3] = p;
                    }
                    if (igp[0] && igp[1] && igp[2] && igp[3])
                        return;
                }
            }
        }
        /* sbas ionospheric delay correction -------------------------------------------
        * compute sbas ionosphric delay correction
        * args   : gtime_t  time    I   time
        *          nav_t    *nav    I   navigation data
        *          double   *pos    I   receiver position {lat,lon,height} (rad/m)
        *          double   *azel   I   satellite azimuth/elavation angle (rad)
        *          double   *delay  O   slant ionospheric delay (L1) (m)
        *          double   *var    O   variance of ionospheric delay (m^2)
        * return : status (1:ok, 0:no correction)
        * notes  : before calling the function, sbas ionosphere correction parameters
        *          in navigation data (nav->sbsion) must be set by callig 
        *          sbsupdatecorr()
        *-----------------------------------------------------------------------------*/
        //C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on the parameter 'delay', so pointers on this parameter are left unchanged:
        //C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on the parameter '@var', so pointers on this parameter are left unchanged:
        public static int sbsioncorr(gtime_t time, nav_t nav, double[] pos, double[] azel, double* delay, double* @var)
        {
            const double re = 6378.1363;
            const double hion = 350.0;
            int i;
            int err = 0;
            double fp;
            double[] posp = new double[2];
            double x = 0.0;
            double y = 0.0;
            double t;
            double[] w = { 0, null, null, null };
            sbsigp_t[] igp = { 0, null, null, null }; // {ws,wn,es,en}

            GlobalMembersRtkcmn.trace(4, "sbsioncorr: pos=%.3f %.3f azel=%.3f %.3f\n", pos[0] * 180.0 / DefineConstants.PI, pos[1] * 180.0 / DefineConstants.PI, azel[0] * 180.0 / DefineConstants.PI, azel[1] * 180.0 / DefineConstants.PI);

            *delay = @var = 0.0;
            if (pos[2] < -100.0 || azel[1] <= 0)
            {
                return 1;
            }

            /* ipp (ionospheric pierce point) position */
            fp = GlobalMembersRtkcmn.ionppp(pos, azel, re, hion, posp);

            /* search igps around ipp */
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: searchigp(time,posp,nav->sbsion,igp,&x,&y);
            GlobalMembersSbas.searchigp(new gtime_t(time), posp, new sbsion_t(nav.sbsion), igp, ref x, ref y);

            /* weight of igps */
            if (igp[0] != null && igp[1] != null && igp[2] != null && igp[3] != null)
            {
                w[0] = (1.0 - x) * (1.0 - y);
                w[1] = (1.0 - x) * y;
                w[2] = x * (1.0 - y);
                w[3] = x * y;
            }
            else if (igp[0] != null && igp[1] != null && igp[2] != null)
            {
                w[1] = y;
                w[2] = x;
                if ((w[0] = 1.0 - w[1] - w[2]) < 0.0)
                {
                    err = 1;
                }
            }
            else if (igp[0] != null && igp[2] != null && igp[3] != null)
            {
                w[0] = 1.0 - x;
                w[3] = y;
                if ((w[2] = 1.0 - w[0] - w[3]) < 0.0)
                {
                    err = 1;
                }
            }
            else if (igp[0] != null && igp[1] != null && igp[3] != null)
            {
                w[0] = 1.0 - y;
                w[3] = x;
                if ((w[1] = 1.0 - w[0] - w[3]) < 0.0)
                {
                    err = 1;
                }
            }
            else if (igp[1] != null && igp[2] != null && igp[3] != null)
            {
                w[1] = 1.0 - x;
                w[2] = 1.0 - y;
                if ((w[3] = 1.0 - w[1] - w[2]) < 0.0)
                {
                    err = 1;
                }
            }
            else
            {
                err = 1;
            }

            if (err != 0)
            {
                GlobalMembersRtkcmn.trace(2, "no sbas iono correction: lat=%3.0f lon=%4.0f\n", posp[0] * 180.0 / DefineConstants.PI, posp[1] * 180.0 / DefineConstants.PI);
                return 0;
            }
            for (i = 0; i < 4; i++)
            {
                if (igp[i] == null)
                    continue;
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: t=timediff(time,igp[i]->t0);
                t = GlobalMembersRtkcmn.timediff(new gtime_t(time), new gtime_t(igp[i].t0));
                *delay += w[i] * igp[i].delay;
                *@var += w[i] * GlobalMembersSbas.varicorr(igp[i].give) * 9E-8 * Math.Abs(t);
            }
            *delay *= fp;
            *@var *= fp * fp;

            GlobalMembersRtkcmn.trace(5, "sbsioncorr: dion=%7.2f sig=%7.2f\n", *delay, Math.Sqrt(*@var));
            return 1;
        }
        /* get meterological parameters ----------------------------------------------*/
        internal static void getmet(double lat, double[] met)
        {
            double[,] metprm = { { 1013.25, 299.65, 26.31, 6.30E-3, 2.77, 0.00, 0.00, 0.00, 0.00E-3, 0.00 }, { 1017.25, 294.15, 21.79, 6.05E-3, 3.15, -3.75, 7.00, 8.85, 0.25E-3, 0.33 }, { 1015.75, 283.15, 11.66, 5.58E-3, 2.57, -2.25, 11.00, 7.24, 0.32E-3, 0.46 }, { 1011.75, 272.15, 6.78, 5.39E-3, 1.81, -1.75, 15.00, 5.36, 0.81E-3, 0.74 }, { 1013.00, 263.65, 4.11, 4.53E-3, 1.55, -0.50, 14.50, 3.39, 0.62E-3, 0.30 } }; // lat=15,30,45,60,75
            int i;
            int j;
            double a;
            lat = Math.Abs(lat);
            if (lat <= 15.0)
            {
                for (i = 0; i < 10; i++)
                {
                    met[i] = metprm[0, i];
                }
            }
            else if (lat >= 75.0)
            {
                for (i = 0; i < 10; i++)
                {
                    met[i] = metprm[4, i];
                }
            }
            else
            {
                j = (int)(lat / 15.0);
                a = (lat - j * 15.0) / 15.0;
                for (i = 0; i < 10; i++)
                {
                    met[i] = (1.0 - a) * metprm[j - 1, i] + a * metprm[j, i];
                }
            }
        }
        //C++ TO C# CONVERTER NOTE: This was formerly a static local variable declaration (not allowed in C#):
        private static double[] sbstropcorr_pos_ = { 0, null, null };
        double zh = 0.0;
        double zw = 0.0;
        /* tropospheric delay correction -----------------------------------------------
        * compute sbas tropospheric delay correction (mops model)
        * args   : gtime_t time     I   time
        *          double   *pos    I   receiver position {lat,lon,height} (rad/m)
        *          double   *azel   I   satellite azimuth/elavation (rad)
        *          double   *var    O   variance of troposphric error (m^2)
        * return : slant tropospheric delay (m)
        *-----------------------------------------------------------------------------*/
        public static double sbstropcorr(gtime_t time, double[] pos, double[] azel, ref double @var)
        {
            const double k1 = 77.604;
            const double k2 = 382000.0;
            const double rd = 287.054;
            const double gm = 9.784;
            const double g = 9.80665;
            //C++ TO C# CONVERTER NOTE: This static local variable declaration (not allowed in C#) has been moved just prior to the method:
            //	static double pos_[3]={0},zh=0.0,zw=0.0;
            int i;
            double c;
            double[] met = new double[10];
            double sinel = Math.Sin(azel[1]);
            double h = pos[2];
            double m;

            GlobalMembersRtkcmn.trace(4, "sbstropcorr: pos=%.3f %.3f azel=%.3f %.3f\n", pos[0] * 180.0 / DefineConstants.PI, pos[1] * 180.0 / DefineConstants.PI, azel[0] * 180.0 / DefineConstants.PI, azel[1] * 180.0 / DefineConstants.PI);

            if (pos[2] < -100.0 || 10000.0 < pos[2] || azel[1] <= 0)
            {
                @var = 0.0;
                return 0.0;
            }
            if (zh == 0.0 || Math.Abs(pos[0] - sbstropcorr_pos_[0]) > 1E-7 || Math.Abs(pos[1] - sbstropcorr_pos_[1]) > 1E-7 || Math.Abs(pos[2] - sbstropcorr_pos_[2]) > 1.0)
            {
                GlobalMembersSbas.getmet(pos[0] * 180.0 / DefineConstants.PI, met);
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: c=cos(2.0 *DefineConstants.PI*(time2doy(time)-(pos[0]>=0.0?28.0:211.0))/365.25);
                c = Math.Cos(2.0 * DefineConstants.PI * (GlobalMembersRtkcmn.time2doy(new gtime_t(time)) - (pos[0] >= 0.0 ? 28.0 : 211.0)) / 365.25);
                for (i = 0; i < 5; i++)
                {
                    met[i] -= met[i + 5] * c;
                }
                zh = 1E-6 * k1 * rd * met[0] / gm;
                zw = 1E-6 * k2 * rd / (gm * (met[4] + 1.0) - met[3] * rd) * met[2] / met[1];
                zh *= Math.Pow(1.0 - met[3] * h / met[1], g / (rd * met[3]));
                zw *= Math.Pow(1.0 - met[3] * h / met[1], (met[4] + 1.0) * g / (rd * met[3]) - 1.0);
                for (i = 0; i < 3; i++)
                {
                    sbstropcorr_pos_[i] = pos[i];
                }
            }
            m = 1.001 / Math.Sqrt(0.002001 + sinel * sinel);
            @var = 0.12 * 0.12 * m * m;
            return (zh + zw) * m;
        }
        /* long term correction ------------------------------------------------------*/
        internal static int sbslongcorr(gtime_t time, int sat, sbssat_t sbssat, double[] drs, ref double ddts)
        {
            //C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on this variable, so pointers on this variable are left unchanged:
            sbssatp_t* p = new sbssatp_t();
            double t;
            int i;

            GlobalMembersRtkcmn.trace(3, "sbslongcorr: sat=%2d\n", sat);

            for (p = sbssat.sat; p < sbssat.sat + sbssat.nsat; p++)
            {
                if (p.sat != sat || p.lcorr.t0.time == 0)
                    continue;
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: t=timediff(time,p->lcorr.t0);
                t = GlobalMembersRtkcmn.timediff(new gtime_t(time), new gtime_t(p.lcorr.t0));
                if (Math.Abs(t) > DefineConstants.MAXSBSAGEL)
                {
                    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                    //ORIGINAL LINE: trace(2,"sbas long-term correction expired: %s sat=%2d t=%5.0f\n", time_str(time,0),sat,t);
                    GlobalMembersRtkcmn.trace(2, "sbas long-term correction expired: %s sat=%2d t=%5.0f\n", GlobalMembersRtkcmn.time_str(new gtime_t(time), 0), sat, t);
                    return 0;
                }
                for (i = 0; i < 3; i++)
                {
                    drs[i] = p.lcorr.dpos[i] + p.lcorr.dvel[i] * t;
                }
                ddts = p.lcorr.daf0 + p.lcorr.daf1 * t;

                GlobalMembersRtkcmn.trace(5, "sbslongcorr: sat=%2d drs=%7.2f%7.2f%7.2f ddts=%7.2f\n", sat, drs[0], drs[1], drs[2], ddts * DefineConstants.CLIGHT);

                return 1;
            }
            /* if sbas satellite without correction, no correction applied */
            if (GlobalMembersRtkcmn.satsys(sat, null) == DefineConstants.SYS_SBS)
            {
                return 1;
            }

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: trace(2,"no sbas long-term correction: %s sat=%2d\n",time_str(time,0),sat);
            GlobalMembersRtkcmn.trace(2, "no sbas long-term correction: %s sat=%2d\n", GlobalMembersRtkcmn.time_str(new gtime_t(time), 0), sat);
            return 0;
        }
        /* fast correction -----------------------------------------------------------*/
        //C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on the parameter 'prc', so pointers on this parameter are left unchanged:
        internal static int sbsfastcorr(gtime_t time, int sat, sbssat_t sbssat, double* prc, ref double @var)
        {
            //C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on this variable, so pointers on this variable are left unchanged:
            sbssatp_t* p = new sbssatp_t();
            double t;

            GlobalMembersRtkcmn.trace(3, "sbsfastcorr: sat=%2d\n", sat);

            for (p = sbssat.sat; p < sbssat.sat + sbssat.nsat; p++)
            {
                if (p.sat != sat)
                    continue;
                if (p.fcorr.t0.time == 0)
                    break;
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: t=timediff(time,p->fcorr.t0)+sbssat->tlat;
                t = GlobalMembersRtkcmn.timediff(new gtime_t(time), new gtime_t(p.fcorr.t0)) + sbssat.tlat;

                /* expire age of correction or UDRE==14 (not monitored) */
                if (Math.Abs(t) > DefineConstants.MAXSBSAGEF || p.fcorr.udre >= 15)
                    continue;
                *prc = p.fcorr.prc;
#if RRCENA
			if (p.fcorr.ai > 0 && Math.Abs(t) <= 8.0 * p.fcorr.dt)
			{
				*prc += p.fcorr.rrc * t;
			}
#endif
                @var = GlobalMembersSbas.varfcorr(p.fcorr.udre) + GlobalMembersSbas.degfcorr(p.fcorr.ai) * t * t / 2.0;

                GlobalMembersRtkcmn.trace(5, "sbsfastcorr: sat=%3d prc=%7.2f sig=%7.2f t=%5.0f\n", sat, *prc, Math.Sqrt(@var), t);
                return 1;
            }
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: trace(2,"no sbas fast correction: %s sat=%2d\n",time_str(time,0),sat);
            GlobalMembersRtkcmn.trace(2, "no sbas fast correction: %s sat=%2d\n", GlobalMembersRtkcmn.time_str(new gtime_t(time), 0), sat);
            return 0;
        }
        /* sbas satellite ephemeris and clock correction -------------------------------
        * correct satellite position and clock bias with sbas satellite corrections
        * args   : gtime_t time     I   reception time
        *          int    sat       I   satellite
        *          nav_t  *nav      I   navigation data
        *          double *rs       IO  sat position and corrected {x,y,z} (ecef) (m)
        *          double *dts      IO  sat clock bias and corrected (s)
        *          double *var      O   sat position and clock variance (m^2)
        * return : status (1:ok,0:no correction)
        * notes  : before calling the function, sbas satellite correction parameters 
        *          in navigation data (nav->sbssat) must be set by callig
        *          sbsupdatecorr().
        *          satellite clock correction include long-term correction and fast
        *          correction.
        *          sbas clock correction is usually based on L1C/A code. TGD or DCB has
        *          to be considered for other codes
        *-----------------------------------------------------------------------------*/
        public static int sbssatcorr(gtime_t time, int sat, nav_t nav, double[] rs, double[] dts, ref double @var)
        {
            double[] drs = { 0, null, null };
            double dclk = 0.0;
            double prc = 0.0;
            int i;

            GlobalMembersRtkcmn.trace(3, "sbssatcorr : sat=%2d\n", sat);

            /* sbas long term corrections */
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: if (!sbslongcorr(time,sat,&nav->sbssat,drs,&dclk))
            if (GlobalMembersSbas.sbslongcorr(new gtime_t(time), sat, nav.sbssat, drs, ref dclk) == 0)
            {
                return 0;
            }
            /* sbas fast corrections */
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: if (!sbsfastcorr(time,sat,&nav->sbssat,&prc,var))
            if (GlobalMembersSbas.sbsfastcorr(new gtime_t(time), sat, nav.sbssat, prc, ref @var) == 0)
            {
                return 0;
            }
            for (i = 0; i < 3; i++)
            {
                rs[i] += drs[i];
            }

            dts[0] += dclk + prc / DefineConstants.CLIGHT;

            GlobalMembersRtkcmn.trace(5, "sbssatcorr: sat=%2d drs=%6.3f %6.3f %6.3f dclk=%.3f %.3f var=%.3f\n", sat, drs[0], drs[1], drs[2], dclk, prc / DefineConstants.CLIGHT, @var);

            return 1;
        }
        /* decode sbas message ---------------------------------------------------------
        * decode sbas message frame words and check crc
        * args   : gtime_t time     I   reception time
        *          int    prn       I   sbas satellite prn number
        *          unsigned int *word I message frame words (24bit x 10)
        *          sbsmsg_t *sbsmsg O   sbas message
        * return : status (1:ok,0:crc error)
        *-----------------------------------------------------------------------------*/
        public static int sbsdecodemsg(gtime_t time, int prn, uint[] words, sbsmsg_t sbsmsg)
        {
            int i;
            int j;
            byte[] f = new byte[29];
            double tow;

            GlobalMembersRtkcmn.trace(5, "sbsdecodemsg: prn=%d\n", prn);

            if (time.time == 0)
            {
                return 0;
            }
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: tow=time2gpst(time,&sbsmsg->week);
            tow = GlobalMembersRtkcmn.time2gpst(new gtime_t(time), ref sbsmsg.week);
            sbsmsg.tow = (int)(tow + DefineConstants.DTTOL);
            sbsmsg.prn = prn;
            for (i = 0; i < 7; i++)
            {
                for (j = 0; j < 4; j++)
                {
                    sbsmsg.msg[i * 4 + j] = (byte)(words[i] >> ((3 - j) * 8));
                }
            }
            sbsmsg.msg[28] = (byte)(words[7] >> 18) & 0xC0;
            for (i = 28; i > 0; i--)
            {
                f[i] = (sbsmsg.msg[i] >> 6) + (sbsmsg.msg[i - 1] << 2);
            }
            f[0] = sbsmsg.msg[0] >> 6;

            return GlobalMembersRtkcmn.crc24q(f, 29) == (words[7] & 0xFFFFFF); // check crc
        }
    }
}
