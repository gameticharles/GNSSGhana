using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ghGPS.Classes
{
    public static class GlobalMembersQzslex
    {
        /*------------------------------------------------------------------------------
        * qzslex.c : qzss lex functions
        *
        * references :
        *     [1] IS-QZSS v.1.1, Quasi-Zenith Satellite System Navigation Service
        *         Interface Specification for QZSS, Japan Aerospace Exploration Agency,
        *         July 31, 2009
        *
        * version : $Revision: 1.1 $ $Date: 2008/07/17 21:48:06 $
        * history : 2011/05/27 1.0  new
        *           2011/07/01 1.1  support 24bytes header format for lexconvbin()
        *           2013/03/27 1.2  support message type 12
        *           2013/05/11 1.3  fix bugs on decoding message type 12
        *           2013/09/01 1.4  consolidate mt 12 handling codes provided by T.O.
        *-----------------------------------------------------------------------------*/


        /* ura value -----------------------------------------------------------------*/
        internal static double vareph(int ura)
        {
            double[] uraval = { 0.08, 0.11, 0.15, 0.21, 0.30, 0.43, 0.60, 0.85, 1.20, 1.70, 2.40, 3.40, 4.85, 6.85, 9.65, 9.65 };
            if (ura < 0 || 15 < ura)
            {
                ura = 15;
            }
            return uraval[ura];
        }
        /* get signed 33bit field ----------------------------------------------------*/
        internal static double getbits_33(byte buff, int pos)
        {
            return (double)GlobalMembersRtkcmn.getbits(buff, pos, 32) * 2.0 + GlobalMembersRtkcmn.getbitu(buff, pos + 32, 1);
        }
        /* decode tof and toe field (ref [1] 5.7.2.2.1.1) ----------------------------*/
        internal static int decode_lextof(byte buff, int i, gtime_t tof, gtime_t toe)
        {
            double tt;
            double toes;
            int tow;
            int week;
            string s1 = new string(new char[64]);
            string s2 = new string(new char[64]);

            GlobalMembersRtkcmn.trace(3, "decode_lextof:\n");

            tow = GlobalMembersRtkcmn.getbitu(buff, i, 20);
            i += 20;
            week = GlobalMembersRtkcmn.getbitu(buff, i, 13);
            i += 13;
            toes = GlobalMembersRtkcmn.getbitu(buff, i, 16) * 15.0;
            i += 16;
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: *tof=gpst2time(week,tow);
            tof.CopyFrom(GlobalMembersRtkcmn.gpst2time(week, tow));
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: *toe=gpst2time(week,toes);
            toe.CopyFrom(GlobalMembersRtkcmn.gpst2time(week, toes));

            GlobalMembersRtkcmn.trace(3, "decode_lextof: tow=%d week=%d toe=%d\n", tow, week, toes);

            tt = GlobalMembersRtkcmn.timediff(toe, tof);
            if (tt < -302400.0)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                //ORIGINAL LINE: *toe=timeadd(*toe, 604800.0);
                toe.CopyFrom(GlobalMembersRtkcmn.timeadd(toe, 604800.0));
            }
            else if (tt > 302400.0)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                //ORIGINAL LINE: *toe=timeadd(*toe,-604800.0);
                toe.CopyFrom(GlobalMembersRtkcmn.timeadd(toe, -604800.0));
            }

            GlobalMembersRtkcmn.time2str(tof, ref s1, 3);
            GlobalMembersRtkcmn.time2str(toe, ref s2, 3);
            GlobalMembersRtkcmn.trace(4, "decode_lextof: tof=%s toe=%s\n", s1, s2);
            return i;
        }
        /* decode signal health field (ref [1] 5.7.2.2.1.1) --------------------------*/
        internal static int decode_lexhealth(byte buff, int i, gtime_t tof, nav_t nav)
        {
            int j;
            int sat;
            byte health;

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: trace(3,"decode_lexhealth: tof=%s\n",time_str(tof,0));
            GlobalMembersRtkcmn.trace(3, "decode_lexhealth: tof=%s\n", GlobalMembersRtkcmn.time_str(new gtime_t(tof), 0));

            for (j = 0; j < 35; j++)
            {
                health = GlobalMembersRtkcmn.getbitu(buff, i, 5);
                i += 5;

                if (j < 3)
                {
                    sat = GlobalMembersRtkcmn.satno(DefineConstants.SYS_QZS, j + 193);
                }
                else
                {
                    sat = GlobalMembersRtkcmn.satno(DefineConstants.SYS_GPS, j - 2);
                }
                if (sat == 0)
                    continue;

                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                //ORIGINAL LINE: nav->lexeph[sat-1].tof=tof;
                nav.lexeph[sat - 1].tof.CopyFrom(tof);
                nav.lexeph[sat - 1].health = health;

                GlobalMembersRtkcmn.trace(4, "sat=%2d health=%d\n", sat, health);
            }
            return i;
        }
        /* decode ephemeris and sv clock field (ref [1] 5.7.2.2.1.2) -----------------*/
        internal static int decode_lexeph(byte buff, int i, gtime_t toe, nav_t nav)
        {
            lexeph_t eph = new lexeph_t({ 0 });
            gtime_t tof = new gtime_t();
            byte health;
            int j;
            int prn;
            int sat;

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: trace(3,"decode_lexeph: toe=%s\n",time_str(toe,0));
            GlobalMembersRtkcmn.trace(3, "decode_lexeph: toe=%s\n", GlobalMembersRtkcmn.time_str(new gtime_t(toe), 0));

            prn = GlobalMembersRtkcmn.getbitu(buff, i, 8);
            i += 8;
            eph.ura = GlobalMembersRtkcmn.getbitu(buff, i, 4);
            i += 4;
            eph.pos[0] = GlobalMembersQzslex.getbits_33(buff, i) * DefineConstants.P2_6;
            i += 33;
            eph.pos[1] = GlobalMembersQzslex.getbits_33(buff, i) * DefineConstants.P2_6;
            i += 33;
            eph.pos[2] = GlobalMembersQzslex.getbits_33(buff, i) * DefineConstants.P2_6;
            i += 33;
            eph.vel[0] = GlobalMembersRtkcmn.getbits(buff, i, 28) * 3.051757812500000E-0x5;
            i += 28;
            eph.vel[1] = GlobalMembersRtkcmn.getbits(buff, i, 28) * 3.051757812500000E-0x5;
            i += 28;
            eph.vel[2] = GlobalMembersRtkcmn.getbits(buff, i, 28) * 3.051757812500000E-0x5;
            i += 28;
            eph.acc[0] = GlobalMembersRtkcmn.getbits(buff, i, 24) * 5.960464477539063E-0x8;
            i += 24;
            eph.acc[1] = GlobalMembersRtkcmn.getbits(buff, i, 24) * 5.960464477539063E-0x8;
            i += 24;
            eph.acc[2] = GlobalMembersRtkcmn.getbits(buff, i, 24) * 5.960464477539063E-0x8;
            i += 24;
            eph.jerk[0] = GlobalMembersRtkcmn.getbits(buff, i, 20) * DefineConstants.P2_32;
            i += 20;
            eph.jerk[1] = GlobalMembersRtkcmn.getbits(buff, i, 20) * DefineConstants.P2_32;
            i += 20;
            eph.jerk[2] = GlobalMembersRtkcmn.getbits(buff, i, 20) * DefineConstants.P2_32;
            i += 20;
            eph.af0 = GlobalMembersRtkcmn.getbits(buff, i, 26) * DefineConstants.P2_35;
            i += 26;
            eph.af1 = GlobalMembersRtkcmn.getbits(buff, i, 20) * DefineConstants.P2_48;
            i += 20;
            eph.tgd = GlobalMembersRtkcmn.getbits(buff, i, 13) * DefineConstants.P2_35;
            i += 13;
            for (j = 0; j < 7; j++)
            {
                eph.isc[j] = GlobalMembersRtkcmn.getbits(buff, i, 13) * DefineConstants.P2_35;
                i += 13;
            }
            if (prn == 255) // no satellite
            {
                return i;
            }

            if (1 <= prn && prn <= 32)
            {
                sat = GlobalMembersRtkcmn.satno(DefineConstants.SYS_GPS, prn);
            }
            else if (193 <= prn && prn <= 195)
            {
                sat = GlobalMembersRtkcmn.satno(DefineConstants.SYS_QZS, prn);
            }
            else
            {
                GlobalMembersRtkcmn.trace(2, "lex ephemeris prn error prn=%d\n", prn);
                return i;
            }
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: eph.toe=toe;
            eph.toe.CopyFrom(toe);
            eph.sat = sat;
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: tof =nav->lexeph[sat-1].tof;
            tof.CopyFrom(nav.lexeph[sat - 1].tof);
            health = nav.lexeph[sat - 1].health;
            nav.lexeph[sat - 1] = eph;
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: nav->lexeph[sat-1].tof =tof;
            nav.lexeph[sat - 1].tof.CopyFrom(tof);
            nav.lexeph[sat - 1].health = health;

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: trace(4,"sat=%2d toe=%s pos=%.3f %.3f %.3f vel=%.5f %.5f %.5f\n", sat,time_str(toe,0),eph.pos[0],eph.pos[1],eph.pos[2], eph.vel[0],eph.vel[1],eph.vel[2]);
            GlobalMembersRtkcmn.trace(4, "sat=%2d toe=%s pos=%.3f %.3f %.3f vel=%.5f %.5f %.5f\n", sat, GlobalMembersRtkcmn.time_str(new gtime_t(toe), 0), eph.pos[0], eph.pos[1], eph.pos[2], eph.vel[0], eph.vel[1], eph.vel[2]);
            GlobalMembersRtkcmn.trace(4, "clk=%11.3f %8.5f tgd=%7.3f\n", eph.af0 * 1E9, eph.af1 * 1E9, eph.tgd * 1E9);
            GlobalMembersRtkcmn.trace(4, "isc=%6.3f %6.3f %6.3f %6.3f %6.3f %6.3f %6.3f\n", eph.isc[0] * 1E9, eph.isc[1] * 1E9, eph.isc[2] * 1E9, eph.isc[3] * 1E9, eph.isc[4] * 1E9, eph.isc[5] * 1E9, eph.isc[6] * 1E9);
            return i;
        }
        /* decode ionosphere correction field (ref [1] 5.7.2.2.1.3) ------------------*/
        internal static int decode_lexion(byte buff, int i, gtime_t tof, nav_t nav)
        {
            lexion_t ion = new lexion_t({ 0 });
            int tow;
            int week;

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: trace(3,"decode_lexion: tof=%s\n",time_str(tof,0));
            GlobalMembersRtkcmn.trace(3, "decode_lexion: tof=%s\n", GlobalMembersRtkcmn.time_str(new gtime_t(tof), 0));

            tow = GlobalMembersRtkcmn.getbitu(buff, i, 20);
            i += 20;

            if (tow == 0xFFFFF) // correction not available
            {
                return i + 192;
            }
            week = GlobalMembersRtkcmn.getbitu(buff, i, 13);
            i += 13;
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: ion.t0=gpst2time(week,tow);
            ion.t0.CopyFrom(GlobalMembersRtkcmn.gpst2time(week, tow));
            ion.tspan = GlobalMembersRtkcmn.getbitu(buff, i, 8) * 60.0; // time span (s)
            i += 8;
            ion.pos0[0] = GlobalMembersRtkcmn.getbits(buff, i, 19) * 1E-5; // latitude  (rad)
            i += 19;
            ion.pos0[1] = GlobalMembersRtkcmn.getbits(buff, i, 20) * 1E-5; // longitude (rad)
            i += 20;
            ion.coef[0, 0] = GlobalMembersRtkcmn.getbits(buff, i, 22) * 1E-3;
            i += 22;
            ion.coef[1, 0] = GlobalMembersRtkcmn.getbits(buff, i, 22) * 1E-2;
            i += 22;
            ion.coef[2, 0] = GlobalMembersRtkcmn.getbits(buff, i, 22) * 1E-2;
            i += 22;
            ion.coef[0, 1] = GlobalMembersRtkcmn.getbits(buff, i, 22) * 1E-2;
            i += 22;
            ion.coef[1, 1] = GlobalMembersRtkcmn.getbits(buff, i, 22) * 1E-2;
            i += 22;
            ion.coef[2, 1] = GlobalMembersRtkcmn.getbits(buff, i, 22) * 1E-1;
            i += 22;
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: nav->lexion=ion;
            nav.lexion.CopyFrom(ion);

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: trace(4,"t0=%s tspan=%.0f pos0=%.1f %.1f coef=%.3f %.3f %.3f %.3f %.3f %.3f\n", time_str(ion.t0,0),ion.tspan,ion.pos0[0]*180.0/DefineConstants.PI,ion.pos0[1]*180.0/DefineConstants.PI, ion.coef[0][0],ion.coef[1][0],ion.coef[2][0],ion.coef[0][1], ion.coef[1][1],ion.coef[2][1]);
            GlobalMembersRtkcmn.trace(4, "t0=%s tspan=%.0f pos0=%.1f %.1f coef=%.3f %.3f %.3f %.3f %.3f %.3f\n", GlobalMembersRtkcmn.time_str(new gtime_t(ion.t0), 0), ion.tspan, ion.pos0[0] * 180.0 / DefineConstants.PI, ion.pos0[1] * 180.0 / DefineConstants.PI, ion.coef[0, 0], ion.coef[1, 0], ion.coef[2, 0], ion.coef[0, 1], ion.coef[1, 1], ion.coef[2, 1]);
            return i;
        }
        /* decode type 10: ephemeris data and clock (ref [1] 5.7.2.2.1,1) ------------*/
        internal static int decode_lextype10(lexmsg_t msg, nav_t nav, gtime_t tof)
        {
            gtime_t toe = new gtime_t();
            int i = 0;
            int j;

            GlobalMembersRtkcmn.trace(3, "decode_lextype10:\n");

            /* decode tof and toe field */
            i = GlobalMembersQzslex.decode_lextof(msg.msg, i, tof, toe);

            /* decode signal health field */
            i = GlobalMembersQzslex.decode_lexhealth(msg.msg, i, tof, nav);

            /* decode ephemeris and sv clock field */
            for (j = 0; j < 3; j++)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: i=decode_lexeph(msg->msg,i,toe,nav);
                i = GlobalMembersQzslex.decode_lexeph(msg.msg, i, new gtime_t(toe), nav);
            }
            return 1;
        }
        /* decode type 11: ephemeris data and clock (ref [1] 5.7.2.2.1,1) ------------*/
        internal static int decode_lextype11(lexmsg_t msg, nav_t nav, gtime_t tof)
        {
            gtime_t toe = new gtime_t();
            int i = 0;
            int j;

            GlobalMembersRtkcmn.trace(3, "decode_lextype11:\n");

            /* decode tof and toe field */
            i = GlobalMembersQzslex.decode_lextof(msg.msg, i, tof, toe);

            /* decode signal health field */
            i = GlobalMembersQzslex.decode_lexhealth(msg.msg, i, tof, nav);

            /* decode ephemeris and sv clock field */
            for (j = 0; j < 2; j++)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: i=decode_lexeph(msg->msg,i,toe,nav);
                i = GlobalMembersQzslex.decode_lexeph(msg.msg, i, new gtime_t(toe), nav);
            }
            /* decode ionosphere correction field */
            GlobalMembersQzslex.decode_lexion(msg.msg, i, tof, nav);

            return 1;
        }
        /* convert lex type 12 to rtcm ssr message -----------------------------------*/
        internal static int lex2rtcm(byte msg, int i, byte[] buff)
        {
            uint crc;
            int j;
            int ns;
            int type;
            int n = 0;

            if (i + 12 >= DefineConstants.LEXFRMLEN - DefineConstants.LEXHDRLEN - DefineConstants.LEXRSLEN)
            {
                return 0;
            }

            switch ((type = GlobalMembersRtkcmn.getbitu(msg, i, 12)))
            {

                case 1057: // gps
                    ns = GlobalMembersRtkcmn.getbitu(msg, i + 62, 6);
                    n = 68 + ns * 135;
                    break;
                case 1058:
                    ns = GlobalMembersRtkcmn.getbitu(msg, i + 61, 6);
                    n = 67 + ns * 76;
                    break;
                case 1059:
                    ns = GlobalMembersRtkcmn.getbitu(msg, i + 61, 6);
                    n = 67;
                    for (j = 0; j < ns; j++)
                    {
                        n += 11 + GlobalMembersRtkcmn.getbitu(msg, i + n + 6, 5) * 19;
                    }
                    break;
                case 1060:
                    ns = GlobalMembersRtkcmn.getbitu(msg, i + 62, 6);
                    n = 68 + ns * 205;
                    break;
                case 1061:
                    ns = GlobalMembersRtkcmn.getbitu(msg, i + 61, 6);
                    n = 67 + ns * 12;
                    break;
                case 1062:
                    ns = GlobalMembersRtkcmn.getbitu(msg, i + 61, 6);
                    n = 67 + ns * 28;
                    break;
                case 1063: // glonass
                    ns = GlobalMembersRtkcmn.getbitu(msg, i + 59, 6);
                    n = 65 + ns * 134;
                    break;
                case 1064:
                    ns = GlobalMembersRtkcmn.getbitu(msg, i + 58, 6);
                    n = 64 + ns * 75;
                    break;
                case 1065:
                    ns = GlobalMembersRtkcmn.getbitu(msg, i + 58, 6);
                    n = 64;
                    for (j = 0; j < ns; j++)
                    {
                        n += 10 + GlobalMembersRtkcmn.getbitu(msg, i + n + 5, 5) * 19;
                    }
                    break;
                case 1066:
                    ns = GlobalMembersRtkcmn.getbitu(msg, i + 59, 6);
                    n = 65 + ns * 204;
                    break;
                case 1067:
                    ns = GlobalMembersRtkcmn.getbitu(msg, i + 58, 6);
                    n = 64 + ns * 11;
                    break;
                case 1068:
                    ns = GlobalMembersRtkcmn.getbitu(msg, i + 58, 6);
                    n = 64 + ns * 27;
                    break;
                case 1240: // galileo
                    ns = GlobalMembersRtkcmn.getbitu(msg, i + 62, 6);
                    n = 68 + ns * 135;
                    break;
                case 1241:
                    ns = GlobalMembersRtkcmn.getbitu(msg, i + 61, 6);
                    n = 67 + ns * 76;
                    break;
                case 1242:
                    ns = GlobalMembersRtkcmn.getbitu(msg, i + 61, 6);
                    n = 67;
                    for (j = 0; j < ns; j++)
                    {
                        n += 11 + GlobalMembersRtkcmn.getbitu(msg, i + n + 6, 5) * 19;
                    }
                    break;
                case 1243:
                    ns = GlobalMembersRtkcmn.getbitu(msg, i + 62, 6);
                    n = 68 + ns * 205;
                    break;
                case 1244:
                    ns = GlobalMembersRtkcmn.getbitu(msg, i + 61, 6);
                    n = 67 + ns * 12;
                    break;
                case 1245:
                    ns = GlobalMembersRtkcmn.getbitu(msg, i + 61, 6);
                    n = 67 + ns * 28;
                    break;
                case 1246: // qzss
                    ns = GlobalMembersRtkcmn.getbitu(msg, i + 62, 4);
                    n = 66 + ns * 133;
                    break;
                case 1247:
                    ns = GlobalMembersRtkcmn.getbitu(msg, i + 61, 4);
                    n = 65 + ns * 74;
                    break;
                case 1248:
                    ns = GlobalMembersRtkcmn.getbitu(msg, i + 61, 4);
                    n = 65;
                    for (j = 0; j < ns; j++)
                    {
                        n += 9 + GlobalMembersRtkcmn.getbitu(msg, i + n + 4, 5) * 19;
                    }
                    break;
                case 1249:
                    ns = GlobalMembersRtkcmn.getbitu(msg, i + 62, 4);
                    n = 66 + ns * 203;
                    break;
                case 1250:
                    ns = GlobalMembersRtkcmn.getbitu(msg, i + 61, 4);
                    n = 65 + ns * 10;
                    break;
                case 1251:
                    ns = GlobalMembersRtkcmn.getbitu(msg, i + 61, 4);
                    n = 65 + ns * 26;
                    break;
                default:
                    if (type != 0)
                    {
                        GlobalMembersRtkcmn.trace(2, "lex 12: unsupported type=%4d\n", type);
                    }
                    return 0;
            }
            n = (n + 7) / 8; // message length (bytes)

            if (i + n * 8 > DefineConstants.LEXFRMLEN - DefineConstants.LEXRSLEN)
            {
                GlobalMembersRtkcmn.trace(2, "lex 12: invalid ssr size: len=%4d\n", n);
                return 0;
            }
            /* save rtcm message to buffer */
            GlobalMembersRtkcmn.setbitu(buff, 0, 8, DefineConstants.RTCM3PREAMB);
            GlobalMembersRtkcmn.setbitu(buff, 8, 6, 0);
            GlobalMembersRtkcmn.setbitu(buff, 14, 10, n);
            for (j = 0; j < n; j++)
            {
                buff[j + 3] = GlobalMembersRtkcmn.getbitu(msg, i + j * 8, 8);
            }
            crc = GlobalMembersRtkcmn.crc24q(buff, 3 + n);
            GlobalMembersRtkcmn.setbitu(buff, 24 + n * 8, 24, crc);
            return n;
        }
        //C++ TO C# CONVERTER NOTE: This was formerly a static local variable declaration (not allowed in C#):
        private static rtcm_t decode_lextype12_stock_rtcm = new rtcm_t();
        /* decode type 12: madoca orbit and clock correction -------------------------*/
        internal static int decode_lextype12(lexmsg_t msg, nav_t nav, gtime_t tof)
        {
            //C++ TO C# CONVERTER NOTE: This static local variable declaration (not allowed in C#) has been moved just prior to the method:
            //	static rtcm_t stock_rtcm={0};
            rtcm_t rtcm = new rtcm_t();
            double tow;
            byte[] buff = new byte[1200];
            int i = 0;
            int j;
            int k;
            int l;
            int n;
            int week;

            GlobalMembersRtkcmn.trace(3, "decode_lextype12:\n");

            tow = GlobalMembersRtkcmn.getbitu(msg.msg, i, 20);
            i += 20;
            week = GlobalMembersRtkcmn.getbitu(msg.msg, i, 13);
            i += 13;
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: *tof=gpst2time(week,tow);
            tof.CopyFrom(GlobalMembersRtkcmn.gpst2time(week, tow));

            /* copy rtcm ssr corrections */
            for (k = 0; k < DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1; k++)
            {
                rtcm.ssr[k] = nav.ssr[k];
                rtcm.ssr[k].update = 0;
            }
            /* convert lex type 12 to rtcm ssr message */
            while ((n = GlobalMembersQzslex.lex2rtcm(msg.msg, i, buff)) != 0)
            {

                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                //ORIGINAL LINE: rtcm.time=*tof;
                rtcm.time.CopyFrom(tof);

                for (j = 0; j < n + 6; j++)
                {

                    /* input rtcm ssr message */
                    if (GlobalMembersRtcm.input_rtcm3(rtcm, buff[j]) == -1)
                        continue;

                    /* update ssr corrections in nav data */
                    for (k = 0; k < DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1; k++)
                    {
                        if (rtcm.ssr[k].update == 0)
                            continue;

                        rtcm.ssr[k].update = 0;

                        if (rtcm.ssr[k].t0[3].time != null) // ura
                        {
                            decode_lextype12_stock_rtcm.ssr[k].t0[3] = rtcm.ssr[k].t0[3];
                            decode_lextype12_stock_rtcm.ssr[k].udi[3] = rtcm.ssr[k].udi[3];
                            decode_lextype12_stock_rtcm.ssr[k].iod[3] = rtcm.ssr[k].iod[3];
                            decode_lextype12_stock_rtcm.ssr[k].ura = rtcm.ssr[k].ura;
                        }
                        if (rtcm.ssr[k].t0[2].time != null) // hr-clock correction
                        {

                            /* convert hr-clock correction to clock correction*/
                            decode_lextype12_stock_rtcm.ssr[k].t0[1] = rtcm.ssr[k].t0[2];
                            decode_lextype12_stock_rtcm.ssr[k].udi[1] = rtcm.ssr[k].udi[2];
                            decode_lextype12_stock_rtcm.ssr[k].iod[1] = rtcm.ssr[k].iod[2];
                            decode_lextype12_stock_rtcm.ssr[k].dclk[0] = rtcm.ssr[k].hrclk;
                            decode_lextype12_stock_rtcm.ssr[k].dclk[1] = decode_lextype12_stock_rtcm.ssr[k].dclk[2] = 0.0;

                            /* activate orbit correction(60.0s is tentative) */
                            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                            //ORIGINAL LINE: if((stock_rtcm.ssr[k].iod[0]==rtcm.ssr[k].iod[2]) && (timediff(stock_rtcm.ssr[k].t0[0],rtcm.ssr[k].t0[2]) < 60.0))
                            if ((decode_lextype12_stock_rtcm.ssr[k].iod[0] == rtcm.ssr[k].iod[2]) && (GlobalMembersRtkcmn.timediff(decode_lextype12_stock_rtcm.ssr[k].t0[0], new gtime_t(rtcm.ssr[k].t0[2])) < 60.0))
                            {
                                rtcm.ssr[k] = decode_lextype12_stock_rtcm.ssr[k];
                            }
                            else // not apply
                                continue;
                        }
                        else if (rtcm.ssr[k].t0[0].time != null) // orbit correction
                        {
                            decode_lextype12_stock_rtcm.ssr[k].t0[0] = rtcm.ssr[k].t0[0];
                            decode_lextype12_stock_rtcm.ssr[k].udi[0] = rtcm.ssr[k].udi[0];
                            decode_lextype12_stock_rtcm.ssr[k].iod[0] = rtcm.ssr[k].iod[0];
                            for (l = 0; l < 3; l++)
                            {
                                decode_lextype12_stock_rtcm.ssr[k].deph[l] = rtcm.ssr[k].deph[l];
                                decode_lextype12_stock_rtcm.ssr[k].ddeph[l] = rtcm.ssr[k].ddeph[l];
                            }
                            decode_lextype12_stock_rtcm.ssr[k].iode = rtcm.ssr[k].iode;
                            decode_lextype12_stock_rtcm.ssr[k].refd = rtcm.ssr[k].refd;

                            /* activate clock correction(60.0s is tentative) */
                            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                            //ORIGINAL LINE: if((stock_rtcm.ssr[k].iod[1]==rtcm.ssr[k].iod[0]) && (timediff(stock_rtcm.ssr[k].t0[1],rtcm.ssr[k].t0[0]) < 60.0))
                            if ((decode_lextype12_stock_rtcm.ssr[k].iod[1] == rtcm.ssr[k].iod[0]) && (GlobalMembersRtkcmn.timediff(decode_lextype12_stock_rtcm.ssr[k].t0[1], new gtime_t(rtcm.ssr[k].t0[0])) < 60.0))
                            {
                                rtcm.ssr[k] = decode_lextype12_stock_rtcm.ssr[k];
                            }
                            else // not apply
                                continue;
                        }
                        /* apply */
                        nav.ssr[k] = rtcm.ssr[k];
                    }
                }
                i += n * 8;
            }
            return 1;
        }
        /* decode type 20: gsi experiment message (ref [1] 5.7.2.2.2) ----------------*/
        internal static int decode_lextype20(lexmsg_t msg, nav_t nav, gtime_t tof)
        {
            GlobalMembersRtkcmn.trace(3, "decode_lextype20:\n");

            return 0; // not supported
        }
        /* update lex corrections ------------------------------------------------------
        * update lex correction parameters in navigation data with a lex message
        * args   : lexmsg_t *msg    I   lex message
        *          nav_t    *nav    IO  navigation data
        *          gtime_t  *tof    O   time of frame
        * return : status (1:ok,0:error or not supported type)
        *-----------------------------------------------------------------------------*/
        public static int lexupdatecorr(lexmsg_t msg, nav_t nav, gtime_t tof)
        {
            GlobalMembersRtkcmn.trace(3, "lexupdatecorr: type=%d\n", msg.type);

            switch (msg.type)
            {
                case 10: // jaxa
                    return GlobalMembersQzslex.decode_lextype10(msg, nav, tof);
                case 11: // jaxa
                    return GlobalMembersQzslex.decode_lextype11(msg, nav, tof);
                case 12: // jaxa
                    return GlobalMembersQzslex.decode_lextype12(msg, nav, tof);
                case 20: // gsi
                    return GlobalMembersQzslex.decode_lextype20(msg, nav, tof);
            }
            GlobalMembersRtkcmn.trace(2, "unsupported lex message: type=%2d\n", msg.type);
            return 0;
        }
        /* read qzss lex message log file ----------------------------------------------
        * read sbas message file
        * args   : char     *file   I   qzss lex message file
        *          int      sel     I   qzss lex satellite prn number selection (0:all)
        *          qzslex_t *lex    IO  qzss lex messages
        * return : status (1:ok,0:error)
        * notes  : only input file with extension .lex or .LEX.
        *-----------------------------------------------------------------------------*/
        public static int lexreadmsg(string file, int sel, lex_t lex)
        {
            lexmsg_t lex_msgs;
            int i;
            int prn;
            int type;
            int alert;
            uint b;
            string buff = new string(new char[1024]);
            string p;
            FILE fp;

            GlobalMembersRtkcmn.trace(3, "readmsgs: file=%s sel=%d\n", file, sel);

            if ((p = StringFunctions.StrRChr(file, '.')) == 0 || (string.Compare(p, ".lex") && string.Compare(p, ".LEX")))
            {
                return 0;
            }

            if ((fp = fopen(file, "r")) == null)
            {
                GlobalMembersRtkcmn.trace(2, "lex message log open error: %s\n", file);
                return 0;
            }
            while (fgets(buff, sizeof(sbyte), fp))
            {
                if (sscanf(buff, "%d %d %d", prn, type, alert) == 3 && (p = StringFunctions.StrStr(buff, ": ")) != 0)
                {
                    p += 2;
                }
                else
                {
                    GlobalMembersRtkcmn.trace(2, "invalid lex log: %s\n", buff);
                    continue;
                }
                if (sel != 0 && sel != prn)
                    continue;

                if (lex.n >= lex.nmax)
                {
                    lex.nmax = lex.nmax == 0 ? 1024 : lex.nmax * 2;
                    //C++ TO C# CONVERTER TODO TASK: The memory management function 'realloc' has no equivalent in C#:
                    if ((lex_msgs = (lexmsg_t)realloc(lex.msgs, lex.nmax * sizeof(lexmsg_t))) == null)
                    {
                        GlobalMembersRtkcmn.trace(1, "lexreadmsg malloc error: nmax=%d\n", lex.nmax);
                        //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
                        free(lex.msgs);
                        lex.msgs = null;
                        lex.n = lex.nmax = 0;
                        return 0;
                    }
                    lex.msgs = lex_msgs;
                }
                lex.msgs[lex.n].prn = prn;
                lex.msgs[lex.n].type = type;
                lex.msgs[lex.n].alert = alert;
                for (i = 0; i < 212; i++)
                {
                    lex.msgs[lex.n].msg[i] = 0;
                }
                for (i = 0; *(p - 1) && p != 0 && i < 212; p += 2, i++)
                {
                    if (sscanf(p, "%2X", b) == 1)
                    {
                        lex.msgs[lex.n].msg[i] = (byte)b;
                    }
                }
                lex.n++;
            }
            fclose(fp);

            return 1;
        }
        /* output lex messages ---------------------------------------------------------
        * output lex message record to output file in rtklib lex log format
        * args   : FILE   *fp       I   output file pointer
        *          lexmsg_t *lexmsg I   lex messages
        * return : none
        * notes  : see ref [1] 5.7.2.1
        *-----------------------------------------------------------------------------*/
        public static void lexoutmsg(FILE fp, lexmsg_t[] msg)
        {
            int i;

            GlobalMembersRtkcmn.trace(4, "lexoutmsg:\n");

            fprintf(fp, "%3d %2d %1d : ", msg.prn, msg.type, msg.alert);
            for (i = 0; i < 212; i++)
            {
                fprintf(fp, "%02X", msg.msg[i]);
            }
            fprintf(fp, "\n");
        }
        /* convert lex binary file to lex message log ----------------------------------
        * convert lex binary file to lex message log
        * args   : int    type      I   output type (0:all)
        *          int    format    I   lex binary format (0:no-headr,1:with-header)
        *          char   *infile   I   input file
        *          char   *outfile  I   output file
        * return : status (1:ok,0:no correction)
        * notes  : see ref [1] 5.7.2.1
        *-----------------------------------------------------------------------------*/
        public static int lexconvbin(int type, int format, string infile, string outfile)
        {
            FILE ifp;
            FILE ofp;
            lexmsg_t msg = new lexmsg_t();
            uint preamb;
            byte[] buff = new byte[DefineConstants.LEXHEADLEN + DefineConstants.LEXFRMLEN / 8];
            int i;
            int j;
            int n = 0;
            uint len = (format != 0 ? DefineConstants.LEXHEADLEN : 0) + DefineConstants.LEXFRMLEN / 8;

            GlobalMembersRtkcmn.trace(3, "lexconvbin:type=%d infile=%s outfile=%s\n", type, infile, outfile);

            if ((ifp = fopen(infile, "rb")) == null)
            {
                GlobalMembersRtkcmn.trace(1, "lexconvbin infile open error: %s\n", infile);
                return 0;
            }
            if ((ofp = fopen(outfile, "w")) == null)
            {
                GlobalMembersRtkcmn.trace(1, "lexconvbin outfile open error: %s\n", outfile);
                fclose(ifp);
                return 0;
            }
            while (fread(buff, 1, len, ifp) == len)
            {
                i = format != 0 ? DefineConstants.LEXHEADLEN * 8 : 0;
                preamb = GlobalMembersRtkcmn.getbitu(buff, i, 32);
                i += 32;
                msg.prn = GlobalMembersRtkcmn.getbitu(buff, i, 8);
                i += 8;
                msg.type = GlobalMembersRtkcmn.getbitu(buff, i, 8);
                i += 8;
                msg.alert = GlobalMembersRtkcmn.getbitu(buff, i, 1);
                i += 1;
                if (preamb != DefineConstants.LEXFRMPREAMB)
                {
                    GlobalMembersRtkcmn.trace(1, "lex frame preamble error: preamb=%08X\n", preamb);
                    continue;
                }
                for (j = 0; j < 212; j++)
                {
                    msg.msg[j] = (byte)GlobalMembersRtkcmn.getbitu(buff, i, 8);
                    i += 8;
                }
                msg.msg[211] &= 0xFE;

                fprintf(stderr, "frame=%5d prn=%d type=%d alert=%d\r", ++n, msg.prn, msg.type, msg.alert);

                if (type == 0 || type == msg.type)
                {
                    GlobalMembersQzslex.lexoutmsg(ofp, msg);
                }
            }
            fclose(ifp);
            fclose(ofp);
            fprintf(stderr, "\n");
            return 1;
        }
        /* lex satellite ephemeris and clock correction -------------------------------
        * satellite position by lex ephemeris
        * args   : gtime_t time     I   time (gpst)
        *          int    sat       I   satellite
        *          nav_t  *nav      I   navigation data
        *          double *rs       O   satellite position and velocity
        *                               {x,y,z,vx,vy,vz} (ecef) (m|m/s)
        *          double *dts      O   satellite clock {bias,drift} (s|s/s)
        *          double *var      O   satellite position and clock variance (m^2)
        * return : status (1:ok,0:no correction)
        * notes  : see ref [1] 5.7.2.2.1.2
        *          before calling the function, call lexupdatecorr() to set lex 
        *          corrections to navigation data
        *          dts includes relativistic effect correction
        *          dts does not include code bias correction
        *-----------------------------------------------------------------------------*/
        public static int lexeph2pos(gtime_t time, int sat, nav_t nav, double[] rs, double[] dts, ref double @var)
        {
            lexeph_t eph;
            double t;
            double t2;
            double t3;
            int i;

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: trace(3,"lexsatpos: time=%s sat=%2d\n",time_str(time,3),sat);
            GlobalMembersRtkcmn.trace(3, "lexsatpos: time=%s sat=%2d\n", GlobalMembersRtkcmn.time_str(new gtime_t(time), 3), sat);

            if (sat == 0)
            {
                return 0;
            }

            eph = nav.lexeph + sat - 1;

            if (eph.sat != sat || eph.toe.time == 0)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: trace(2,"no lex ephemeris: time=%s sat=%2d\n",time_str(time,0),sat);
                GlobalMembersRtkcmn.trace(2, "no lex ephemeris: time=%s sat=%2d\n", GlobalMembersRtkcmn.time_str(new gtime_t(time), 0), sat);
                return 0;
            }
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: if (fabs(t=timediff(time,eph->toe))>DefineConstants.LEXEPHMAXAGE)
            if (Math.Abs(t = GlobalMembersRtkcmn.timediff(new gtime_t(time), new gtime_t(eph.toe))) > DefineConstants.LEXEPHMAXAGE)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: trace(2,"lex ephemeris age error: time=%s sat=%2d t=%.3f\n", time_str(time,0),sat,t);
                GlobalMembersRtkcmn.trace(2, "lex ephemeris age error: time=%s sat=%2d t=%.3f\n", GlobalMembersRtkcmn.time_str(new gtime_t(time), 0), sat, t);
                return 0;
            }
#if false
	//    if (eph->health&0x18) {
	//         trace(2,"lex ephemeris unhealthy: sat=%2d health=0x%02X\n",sat,eph->health);
	//         return 0;
	//    }
#endif
            t2 = t * t / 2.0;
            t3 = t2 * t / 3.0;
            for (i = 0; i < 3; i++)
            {
                rs[i] = eph.pos[i] + eph.vel[i] * t + eph.acc[i] * t2 + eph.jerk[i] * t3;
                rs[i + 3] = eph.vel[i] + eph.acc[i] * t + eph.jerk[i] * t2;
            }
            dts[0] = eph.af0 + eph.af1 * t;
            dts[1] = eph.af1;

            /* relativistic effect correction */
            dts[0] -= 2.0 * GlobalMembersRtkcmn.dot(rs, rs + 3, 3) / DefineConstants.CLIGHT / DefineConstants.CLIGHT;

            @var = GlobalMembersQzslex.vareph(eph.ura);
            return 1;
        }
        /* lex ionosphere correction --------------------------------------------------
        * ionosphere correction by lex correction
        * args   : gtime_t  time    I   time
        *          nav_t    *nav    I   navigation data
        *          double   *pos    I   receiver position {lat,lon,height} (rad/m)
        *          double   *azel   I   satellite azimuth/elavation angle (rad)
        *          double   *delay  O   slant ionospheric delay (L1) (m)
        *          double   *var    O   variance of ionospheric delay (m^2)
        * return : status (1:ok, 0:no correction)
        * notes  : see ref [1] 5.7.2.2.1.3
        *          before calling the function, call lexupdatecorr() to set lex 
        *          corrections to navigation data
        *-----------------------------------------------------------------------------*/
        //C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on the parameter 'delay', so pointers on this parameter are left unchanged:
        public static int lexioncorr(gtime_t time, nav_t nav, double[] pos, double[] azel, double* delay, ref double @var)
        {
            const double re = 6378.137;
            const double hion = 350.0;
#if false
	//    const double dl1=(141.0-129.0)/(45.5-34.7);
	//    const double dl2=(129.0-126.7)/(34.7-26.0);
#endif
            double tt;
            double sinlat;
            double coslat;
            double sinaz;
            double cosaz;
            double cosel;
            double rp;
            double ap;
            double sinap;
            double cosap;
            double latpp;
            double lonpp;
            double dlat;
            double dlon;
            double Enm;
            double F;
            int n;
            int m;

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: trace(4,"lexioncorr: time=%s pos=%.3f %.3f azel=%.3f %.3f\n",time_str(time,3), pos[0]*180.0/DefineConstants.PI,pos[1]*180.0/DefineConstants.PI,azel[0]*180.0/DefineConstants.PI,azel[1]*180.0/DefineConstants.PI);
            GlobalMembersRtkcmn.trace(4, "lexioncorr: time=%s pos=%.3f %.3f azel=%.3f %.3f\n", GlobalMembersRtkcmn.time_str(new gtime_t(time), 3), pos[0] * 180.0 / DefineConstants.PI, pos[1] * 180.0 / DefineConstants.PI, azel[0] * 180.0 / DefineConstants.PI, azel[1] * 180.0 / DefineConstants.PI);

            *delay = @var = 0.0;

            if (pos[2] < -100.0 || azel[1] <= 0.0)
            {
                return 1;
            }

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: tt=timediff(time,nav->lexion.t0);
            tt = GlobalMembersRtkcmn.timediff(new gtime_t(time), new gtime_t(nav.lexion.t0));

            /* check time span */
            if (Math.Abs(tt) > nav.lexion.tspan)
            {
                GlobalMembersRtkcmn.trace(2, "lex iono age error: tt=%.0f tspan=%.0f\n", tt, nav.lexion.tspan);
                return 0;
            }
            /* check user position range (ref [1] 4.1.5) */
#if false
	//    if (pos[0]> 45.5*D2R||pos[0]< 26.0*D2R||
	//        pos[1]>146.0*D2R||
	//        pos[1]<129.0*D2R+dl1*(pos[0]-34.7*D2R)||
	//        pos[1]<126.7*D2R+dl2*(pos[0]-26.0*D2R)) {
	//        trace(2,"lex iono out of coverage pos=%.3f %.3f\n",pos[0]*R2D,pos[1]*R2D);
	//        return 0;
	//    }
#endif
            /* ionospheric pierce point position */
            sinlat = Math.Sin(pos[0]);
            coslat = Math.Cos(pos[0]);
            sinaz = Math.Sin(azel[0]);
            cosaz = Math.Cos(azel[0]);
            cosel = Math.Cos(azel[1]);
            rp = re / (re + hion) * cosel;
            ap = DefineConstants.PI / 2.0 - azel[1] - Math.Asin(rp);
            sinap = Math.Sin(ap);
            cosap = Math.Cos(ap);
            latpp = Math.Asin(sinlat * cosap + coslat * sinap * cosaz);
            lonpp = pos[1] + Math.Atan(sinap * sinaz / (cosap * coslat - sinap * cosaz * sinlat));

            GlobalMembersRtkcmn.trace(4, "lexioncorr: pppos=%.3f %.3f\n", latpp * 180.0 / DefineConstants.PI, lonpp * 180.0 / DefineConstants.PI);

            /* inclination factor */
            F = 1.0 / Math.Sqrt(1.0 - rp * rp);

            /* delta latitude/longitude (rad) */
            dlat = latpp - nav.lexion.pos0[0];
            dlon = lonpp - nav.lexion.pos0[1];
            GlobalMembersRtkcmn.trace(4, "lexioncorr: pos0=%.1f %.1f dlat=%.1f dlon=%.1f\n", nav.lexion.pos0[0] * 180.0 / DefineConstants.PI, nav.lexion.pos0[1] * 180.0 / DefineConstants.PI, dlat * 180.0 / DefineConstants.PI, dlon * 180.0 / DefineConstants.PI);

            /* slant ionosphere delay (L1) */
            for (n = 0; n <= 2; n++)
            {
                for (m = 0; m <= 1; m++)
                {
                    Enm = nav.lexion.coef[n, m];
                    *delay += F * Enm * Math.Pow(dlat, n) * Math.Pow(dlon, m);

                    GlobalMembersRtkcmn.trace(5, "lexioncorr: F=%8.3f Enm[%d][%d]=%8.3f delay=%8.3f\n", F, n, m, Enm, F * Enm * Math.Pow(dlat, n) * Math.Pow(dlon, m));
                }
            }
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: trace(4,"lexioncorr: time=%s delay=%.3f\n",time_str(time,0),*delay);
            GlobalMembersRtkcmn.trace(4, "lexioncorr: time=%s delay=%.3f\n", GlobalMembersRtkcmn.time_str(new gtime_t(time), 0), *delay);

            return 1;
        }
    }
}
