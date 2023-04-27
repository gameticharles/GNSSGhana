using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ghGPS.Classes
{
    public static class GlobalMembersRinex
    {
        /*------------------------------------------------------------------------------
        * rinex.c : rinex functions
        *
        *          Copyright (C) 2007-2014 by T.TAKASU, All rights reserved.
        *
        * reference :
        *     [1] W.Gurtner and L.Estey, RINEX The Receiver Independent Exchange Format
        *         Version 2.11, December 10, 2007
        *     [2] W.Gurtner and L.Estey, RINEX The Receiver Independent Exchange Format
        *         Version 3.00, November 28, 2007
        *     [3] IS-GPS-200D, Navstar GPS Space Segment/Navigation User Interfaces,
        *         7 March, 2006
        *     [4] W.Gurtner and L.Estey, RINEX The Receiver Independent Exchange Format
        *         Version 2.12, June 23, 2009
        *     [5] W.Gurtner and L.Estey, RINEX The Receiver Independent Exchange Format
        *         Version 3.01, June 22, 2009
        *     [6] J.Ray and W.Gurtner, RINEX extentions to handle clock information
        *         version 3.02, September 2, 2010
        *     [7] RINEX The Receiver Independent Exchange Format Version 3.02,
        *         International GNSS Service (IGS), RINEX Working Group and Radio
        *         Technical Commission for Maritime Services Special Committee 104
        *         (RTCM-SC104), December 10, 2012
        *
        * version : $Revision:$
        * history : 2006/01/16 1.0  new
        *           2007/03/14 1.1  read P1 if no obstype of C1
        *           2007/04/27 1.2  add readrnxt() function
        *           2007/05/25 1.3  add support of file path with wild-card (*)
        *                           add support of compressed files
        *           2007/11/02 1.4  support sbas/geo satellite
        *                           support doppler observables
        *                           support rinex bug of week handover
        *                           add rinex obs/nav output functions
        *           2008/06/16 1.5  export readrnxf(), add compress()
        *                           separate sortobs(), uniqeph(), screent()
        *           2008/10/28 1.6  fix bug on reading rinex obs header types of observ
        *           2009/04/09 1.7  support rinex 2.11
        *                           change api of outrnxobsh(),outrnxobsb(),outrnxnavb()
        *           2009/06/02 1.8  add api outrnxgnavb()
        *           2009/08/15 1.9  support glonass
        *                           add slip save/restore functions
        *           2010/03/03 1.10 fix bug of array access by disabled satellite
        *           2010/07/21 1.11 support rinex ver.2.12, 3.00
        *                           support rinex extension for qzss
        *                           support geo navigation messages
        *                           added api:
        *                               setrnxcodepri(),outrnxhnavh(),outrnxhnavb(),
        *                           changed api:
        *                               readrnx(),readrnxt(),outrnxnavh(),outrnxgnavh()
        *           2010/05/29 1.12 fix bug on skipping invalid satellite data
        *                           fix bug on frequency number overflow
        *                           output P1 instead of C1 if rnxopt.rcvopt=-L1P
        *                           output C2 instead of P2 if rnxopt.rcvopt=-L2C
        *                           change api:
        *                               outrnxgnavh(),outrnxhnavh(),readrnx(),
        *                               readrnxt()
        *                           add api:
        *                               outrnxlnavh(), outrnxqnav()
        *                           move uniqeph(),uniqgeph,uniqseph()
        *           2010/08/19 1.13 suppress warning
        *           2012/03/01 1.14 add function to read cnes widelane fcb in rnxclk
        *                           support compass rinex nav
        *                           change api: setcodepri()
        *           2012/10/17 1.15 support ver.2.12, ver.3.01
        *                           add api init_rnxctr(),free_rnxctr(),open_rnxctr(),
        *                           input_rnxctr()
        *                           change api readrnxt(),readrnx()
        *                           delete api setrnxcodepri()
        *                           fix bug on message frama time in v.3 glonass nav
        *           2013/02/09 1.16 add reading geph.iode derived from toe
        *           2013/02/23 1.17 support rinex 3.02 (ref [7])
        *                           change api outrnxobsh()
        *                           add api outrnxcnavh()
        *                           fix bug on output of fit interval
        *           2013/05/08 1.18 fix bug on reading glo and geo nav in rinex 3
        *           2013/09/01 1.19 fix bug on reading galileo "C1" in rinex 2.12
        *           2013/12/16 1.20 reject C1 for 2.12
        *           2014/05/26 1.21 fix bug on reading gps "C2" in rinex 2.11 or 2.12
        *                           fix problem on type imcompatibility
        *                           support beidou
        *           2014/08/29 1.22 fix bug on reading gps "C2" in rinex 2.11 or 2.12
        *           2014/10/20 1.23 recognize "C2" in 2.12 as "C2W" instead of "C2D"
        *           2014/12/07 1.24 add read rinex option -SYS=...
        *-----------------------------------------------------------------------------*/


        internal const string rcsid = "$Id:$";



        internal readonly int[] navsys = { DefineConstants.SYS_GPS, DefineConstants.SYS_GLO, DefineConstants.SYS_GAL, DefineConstants.SYS_QZS, DefineConstants.SYS_SBS, DefineConstants.SYS_CMP, 0 }; // satellite systems
        internal const string syscodes = "GREJSC"; // satellite system codes

        internal const string obscodes = "CLDS"; // obs type codes

        internal const string frqcodes = "125678"; // frequency codes

        internal readonly double[] ura_eph = { 2.4, 3.4, 4.85, 6.85, 9.65, 13.65, 24.0, 48.0, 96.0, 192.0, 384.0, 768.0, 1536.0, 3072.0, 6144.0, 0.0 }; // ura values (ref [3] 20.3.3.3.1.1)

        /* set string without tail space ---------------------------------------------*/
        internal static void setstr(ref string dst, string src, int n)
        {
            //C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on this variable, so pointers on this variable are left unchanged:
            sbyte* p = dst;
            //C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on this variable, so pointers on this variable are left unchanged:
            sbyte* q = src;
            while (*q && q < src.Substring(n))
            {
                *p++ = *q++;
            }
            *p-- = '\0';
            while (p >= dst && *p == ' ')
            {
                *p-- = '\0';
            }
        }
        /* adjust time considering week handover -------------------------------------*/
        internal static gtime_t adjweek(gtime_t t, gtime_t t0)
        {
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: double tt=timediff(t,t0);
            double tt = GlobalMembersRtkcmn.timediff(new gtime_t(t), new gtime_t(t0));
            if (tt < -302400.0)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: return timeadd(t, 604800.0);
                return GlobalMembersRtkcmn.timeadd(new gtime_t(t), 604800.0);
            }
            if (tt > 302400.0)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: return timeadd(t,-604800.0);
                return GlobalMembersRtkcmn.timeadd(new gtime_t(t), -604800.0);
            }
            return t;
        }
        /* adjust time considering week handover -------------------------------------*/
        internal static gtime_t adjday(gtime_t t, gtime_t t0)
        {
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: double tt=timediff(t,t0);
            double tt = GlobalMembersRtkcmn.timediff(new gtime_t(t), new gtime_t(t0));
            if (tt < -43200.0)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: return timeadd(t, 86400.0);
                return GlobalMembersRtkcmn.timeadd(new gtime_t(t), 86400.0);
            }
            if (tt > 43200.0)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: return timeadd(t,-86400.0);
                return GlobalMembersRtkcmn.timeadd(new gtime_t(t), -86400.0);
            }
            return t;
        }
        /* time string for ver.3 (yyyymmdd hhmmss UTC) -------------------------------*/
        internal static void timestr_rnx(ref string str)
        {
            gtime_t time = new gtime_t();
            double[] ep = new double[6];
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: time=timeget();
            time.CopyFrom(GlobalMembersRtkcmn.timeget());
            time.sec = 0.0;
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: time2epoch(time,ep);
            GlobalMembersRtkcmn.time2epoch(new gtime_t(time), ep);
            //C++ TO C# CONVERTER TODO TASK: Zero padding is not converted when combined with a minimum width specifier:
            //ORIGINAL LINE: sprintf(str,"%04.0f%02.0f%02.0f %02.0f%02.0f%02.0f UTC",ep[0],ep[1],ep[2], ep[3],ep[4],ep[5]);
            str = string.Format("{0,4:f0}{1,2:f0}{2,2:f0} {3,2:f0}{4,2:f0}{5,2:f0} UTC", ep[0], ep[1], ep[2], ep[3], ep[4], ep[5]);
        }
        /* satellite to satellite code -----------------------------------------------*/
        internal static int sat2code(int sat, ref string code)
        {
            int prn;
            switch (GlobalMembersRtkcmn.satsys(sat, ref prn))
            {
                case DefineConstants.SYS_GPS:
                    code = string.Format("G{0,2:D}", prn - DefineConstants.MINPRNGPS + 1);
                    break;
                case DefineConstants.SYS_GLO:
                    code = string.Format("R{0,2:D}", prn - DefineConstants.MINPRNGLO + 1);
                    break;
                case DefineConstants.SYS_GAL:
                    code = string.Format("E{0,2:D}", prn - DefineConstants.MINPRNGAL + 1);
                    break;
                case DefineConstants.SYS_SBS:
                    code = string.Format("S{0,2:D}", prn - 100);
                    break;
                case DefineConstants.SYS_QZS:
                    code = string.Format("J{0,2:D}", prn - DefineConstants.MINPRNQZS + 1);
                    break;
                case DefineConstants.SYS_CMP:
                    code = string.Format("C{0,2:D}", prn - DefineConstants.MINPRNCMP + 1);
                    break;
                default:
                    return 0;
            }
            return 1;
        }
        /* ura index to ura value (m) ------------------------------------------------*/
        internal static double uravalue(int sva)
        {
            return 0 <= sva && sva < 15 ? ura_eph[sva] : 32767.0;
        }
        /* ura value (m) to ura index ------------------------------------------------*/
        internal static int uraindex(double value)
        {
            int i;
            for (i = 0; i < 15; i++)
            {
                if (ura_eph[i] >= value)
                    break;
            }
            return i;
        }
        /* initialize station parameter ----------------------------------------------*/
        internal static void init_sta(sta_t sta)
        {
            int i;
            *sta.name = (sbyte)'\0';
            *sta.marker = (sbyte)'\0';
            *sta.antdes = (sbyte)'\0';
            *sta.antsno = (sbyte)'\0';
            *sta.rectype = (sbyte)'\0';
            *sta.recver = (sbyte)'\0';
            *sta.recsno = (sbyte)'\0';
            sta.antsetup = sta.itrf = sta.deltype = 0;
            for (i = 0; i < 3; i++)
            {
                sta.pos[i] = 0.0;
            }
            for (i = 0; i < 3; i++)
            {
                sta.del[i] = 0.0;
            }
            sta.hgt = 0.0;
        }
        /*------------------------------------------------------------------------------
        * input rinex functions
        *-----------------------------------------------------------------------------*/

        /* convert rinex obs type ver.2 -> ver.3 -------------------------------------*/
        internal static void convcode(double ver, int sys, string str, ref string type)
        {
            type = "   ";

            if (!string.Compare(str, "P1")) // ver.2.11 GPS L1PY,GLO L2P
            {
                if (sys == DefineConstants.SYS_GPS)
                {
                    type = string.Format("{0}1W", 'C');
                }
                else if (sys == DefineConstants.SYS_GLO)
                {
                    type = string.Format("{0}1P", 'C');
                }
            }
            else if (!string.Compare(str, "P2")) // ver.2.11 GPS L2PY,GLO L2P
            {
                if (sys == DefineConstants.SYS_GPS)
                {
                    type = string.Format("{0}2W", 'C');
                }
                else if (sys == DefineConstants.SYS_GLO)
                {
                    type = string.Format("{0}2P", 'C');
                }
            }
            else if (!string.Compare(str, "C1")) // ver.2.11 GPS L1C,GLO L1C/A
            {
                if (ver >= 2.12) // reject C1 for 2.12
                {
                    ;
                }
                else if (sys == DefineConstants.SYS_GPS)
                {
                    type = string.Format("{0}1C", 'C');
                }
                else if (sys == DefineConstants.SYS_GLO)
                {
                    type = string.Format("{0}1C", 'C');
                }
                else if (sys == DefineConstants.SYS_GAL) // ver.2.12
                {
                    type = string.Format("{0}1X", 'C');
                }
                else if (sys == DefineConstants.SYS_QZS)
                {
                    type = string.Format("{0}1C", 'C');
                }
                else if (sys == DefineConstants.SYS_SBS)
                {
                    type = string.Format("{0}1C", 'C');
                }
            }
            else if (!string.Compare(str, "C2"))
            {
                if (sys == DefineConstants.SYS_GPS)
                {
                    if (ver >= 2.12) // L2P(Y)
                    {
                        type = string.Format("{0}2W", 'C');
                    }
                    else // L2C
                    {
                        type = string.Format("{0}2X", 'C');
                    }
                }
                else if (sys == DefineConstants.SYS_GLO)
                {
                    type = string.Format("{0}2C", 'C');
                }
                else if (sys == DefineConstants.SYS_QZS)
                {
                    type = string.Format("{0}2X", 'C');
                }
                else if (sys == DefineConstants.SYS_CMP) // ver.2.12 B1
                {
                    type = string.Format("{0}1X", 'C');
                }
            }
            else if (ver >= 2.12 && str[1] == 'A') // ver.2.12 L1C/A
            {
                if (sys == DefineConstants.SYS_GPS)
                {
                    type = string.Format("{0}1C", str[0]);
                }
                else if (sys == DefineConstants.SYS_GLO)
                {
                    type = string.Format("{0}1C", str[0]);
                }
                else if (sys == DefineConstants.SYS_QZS)
                {
                    type = string.Format("{0}1C", str[0]);
                }
                else if (sys == DefineConstants.SYS_SBS)
                {
                    type = string.Format("{0}1C", str[0]);
                }
            }
            else if (ver >= 2.12 && str[1] == 'B') // ver.2.12 GPS L1C
            {
                if (sys == DefineConstants.SYS_GPS)
                {
                    type = string.Format("{0}1X", str[0]);
                }
                else if (sys == DefineConstants.SYS_QZS)
                {
                    type = string.Format("{0}1X", str[0]);
                }
            }
            else if (ver >= 2.12 && str[1] == 'C') // ver.2.12 GPS L2C
            {
                if (sys == DefineConstants.SYS_GPS)
                {
                    type = string.Format("{0}2X", str[0]);
                }
                else if (sys == DefineConstants.SYS_QZS)
                {
                    type = string.Format("{0}2X", str[0]);
                }
            }
            else if (ver >= 2.12 && str[1] == 'D') // ver.2.12 GLO L2C/A
            {
                if (sys == DefineConstants.SYS_GLO)
                {
                    type = string.Format("{0}2C", str[0]);
                }
            }
            else if (ver >= 2.12 && str[1] == '1') // ver.2.12 GPS L1PY,GLO L1P
            {
                if (sys == DefineConstants.SYS_GPS)
                {
                    type = string.Format("{0}1W", str[0]);
                }
                else if (sys == DefineConstants.SYS_GLO)
                {
                    type = string.Format("{0}1P", str[0]);
                }
                else if (sys == DefineConstants.SYS_GAL) // tentative
                {
                    type = string.Format("{0}1X", str[0]);
                }
                else if (sys == DefineConstants.SYS_CMP) // extension
                {
                    type = string.Format("{0}1X", str[0]);
                }
            }
            else if (ver < 2.12 && str[1] == '1')
            {
                if (sys == DefineConstants.SYS_GPS)
                {
                    type = string.Format("{0}1C", str[0]);
                }
                else if (sys == DefineConstants.SYS_GLO)
                {
                    type = string.Format("{0}1C", str[0]);
                }
                else if (sys == DefineConstants.SYS_GAL) // tentative
                {
                    type = string.Format("{0}1X", str[0]);
                }
                else if (sys == DefineConstants.SYS_QZS)
                {
                    type = string.Format("{0}1C", str[0]);
                }
                else if (sys == DefineConstants.SYS_SBS)
                {
                    type = string.Format("{0}1C", str[0]);
                }
            }
            else if (str[1] == '2')
            {
                if (sys == DefineConstants.SYS_GPS)
                {
                    type = string.Format("{0}2W", str[0]);
                }
                else if (sys == DefineConstants.SYS_GLO)
                {
                    type = string.Format("{0}2P", str[0]);
                }
                else if (sys == DefineConstants.SYS_QZS)
                {
                    type = string.Format("{0}2X", str[0]);
                }
                else if (sys == DefineConstants.SYS_CMP) // ver.2.12 B1
                {
                    type = string.Format("{0}1X", str[0]);
                }
            }
            else if (str[1] == '5')
            {
                if (sys == DefineConstants.SYS_GPS)
                {
                    type = string.Format("{0}5X", str[0]);
                }
                else if (sys == DefineConstants.SYS_GAL)
                {
                    type = string.Format("{0}5X", str[0]);
                }
                else if (sys == DefineConstants.SYS_QZS)
                {
                    type = string.Format("{0}5X", str[0]);
                }
                else if (sys == DefineConstants.SYS_SBS)
                {
                    type = string.Format("{0}5X", str[0]);
                }
            }
            else if (str[1] == '6')
            {
                if (sys == DefineConstants.SYS_GAL)
                {
                    type = string.Format("{0}6X", str[0]);
                }
                else if (sys == DefineConstants.SYS_QZS)
                {
                    type = string.Format("{0}6X", str[0]);
                }
                else if (sys == DefineConstants.SYS_CMP) // ver.2.12 B3
                {
                    type = string.Format("{0}6X", str[0]);
                }
            }
            else if (str[1] == '7')
            {
                if (sys == DefineConstants.SYS_GAL)
                {
                    type = string.Format("{0}7X", str[0]);
                }
                else if (sys == DefineConstants.SYS_CMP) // ver.2.12 B2
                {
                    type = string.Format("{0}7X", str[0]);
                }
            }
            else if (str[1] == '8')
            {
                if (sys == DefineConstants.SYS_GAL)
                {
                    type = string.Format("{0}8X", str[0]);
                }
            }
            GlobalMembersRtkcmn.trace(3, "convcode: ver=%.2f sys=%2d type= %s -> %s\n", ver, sys, str, type);
        }
        /* decode obs header ---------------------------------------------------------*/
        internal static void decode_obsh(FILE fp, ref string buff, double ver, ref int tsys, string[,] tobs, nav_t nav, sta_t sta)
        {
            /* default codes for unknown code */
            string[] defcodes = { "CWX   ", "CC    ", "X XXXX", "CXXX  ", "C X   ", "X  XX " };
            double[] del = new double[3];
            int i;
            int j;
            int k;
            int n;
            int nt;
            int prn;
            int fcn;
            string p;
            string label = buff.Substring(60);
            string str = new string(new char[4]);

            GlobalMembersRtkcmn.trace(4, "decode_obsh: ver=%.2f\n", ver);

            if (StringFunctions.StrStr(label, "MARKER NAME"))
            {
                if (sta != null)
                {
                    GlobalMembersRinex.setstr(ref sta.name, buff, 60);
                }
            }
            else if (StringFunctions.StrStr(label, "MARKER NUMBER")) // opt
            {
                if (sta != null)
                {
                    GlobalMembersRinex.setstr(ref sta.marker, buff, 20);
                }
            }
            else if (StringFunctions.StrStr(label, "MARKER TYPE")) // ver.3
            {
                ;
            }
            else if (StringFunctions.StrStr(label, "OBSERVER / AGENCY"))
            {
                ;
            }
            else if (StringFunctions.StrStr(label, "REC # / TYPE / VERS"))
            {
                if (sta != null)
                {
                    GlobalMembersRinex.setstr(ref sta.recsno, buff, 20);
                    GlobalMembersRinex.setstr(ref sta.rectype, buff.Substring(20), 20);
                    GlobalMembersRinex.setstr(ref sta.recver, buff.Substring(40), 20);
                }
            }
            else if (StringFunctions.StrStr(label, "ANT # / TYPE"))
            {
                if (sta != null)
                {
                    GlobalMembersRinex.setstr(ref sta.antsno, buff, 20);
                    GlobalMembersRinex.setstr(ref sta.antdes, buff.Substring(20), 20);
                }
            }
            else if (StringFunctions.StrStr(label, "APPROX POSITION XYZ"))
            {
                if (sta != null)
                {
                    for (i = 0, j = 0; i < 3; i++, j += 14)
                    {
                        sta.pos[i] = GlobalMembersRtkcmn.str2num(buff, j, 14);
                    }
                }
            }
            else if (StringFunctions.StrStr(label, "ANTENNA: DELTA H/E/N"))
            {
                if (sta != null)
                {
                    for (i = 0, j = 0; i < 3; i++, j += 14)
                    {
                        del[i] = GlobalMembersRtkcmn.str2num(buff, j, 14);
                    }
                    sta.del[2] = del[0]; // h
                    sta.del[0] = del[1]; // e
                    sta.del[1] = del[2]; // n
                }
            }
            else if (StringFunctions.StrStr(label, "ANTENNA: DELTA X/Y/Z")) // opt ver.3
            {
                ;
            }
            else if (StringFunctions.StrStr(label, "ANTENNA: PHASECENTER")) // opt ver.3
            {
                ;
            }
            else if (StringFunctions.StrStr(label, "ANTENNA: B.SIGHT XYZ")) // opt ver.3
            {
                ;
            }
            else if (StringFunctions.StrStr(label, "ANTENNA: ZERODIR AZI")) // opt ver.3
            {
                ;
            }
            else if (StringFunctions.StrStr(label, "ANTENNA: ZERODIR XYZ")) // opt ver.3
            {
                ;
            }
            else if (StringFunctions.StrStr(label, "CENTER OF MASS: XYZ")) // opt ver.3
            {
                ;
            }
            else if (StringFunctions.StrStr(label, "SYS / # / OBS TYPES")) // ver.3
            {
                if ((p = StringFunctions.StrChr(syscodes, buff[0])) == 0)
                {
                    GlobalMembersRtkcmn.trace(2, "invalid system code: sys=%c\n", buff[0]);
                    return;
                }
                i = (int)(p - syscodes);
                n = (int)GlobalMembersRtkcmn.str2num(buff, 3, 3);
                for (j = nt = 0, k = 7; j < n; j++, k += 4)
                {
                    if (k > 58)
                    {
                        if (!fgets(buff, 16 * DefineConstants.MAXOBSTYPE + 4, fp))
                            break;
                        k = 7;
                    }
                    if (nt < DefineConstants.MAXOBSTYPE - 1)
                    {
                        GlobalMembersRinex.setstr(ref tobs[i, nt++], buff.Substring(k), 3);
                    }
                }
                tobs[i, nt] = '\0';

                /* change beidou B1 code: 3.02 draft -> 3.02 */
                if (i == 5)
                {
                    for (j = 0; j < nt; j++)
                    {
                        if (tobs[i, j, 1] == '2')
                        {
                            tobs[i, j, 1] = '1';
                        }
                    }
                }
                /* if unknown code in ver.3, set default code */
                for (j = 0; j < nt; j++)
                {
                    if (tobs[i, j, 2] != null)
                        continue;
                    if ((p = StringFunctions.StrChr(frqcodes, tobs[i, j, 1])) == 0)
                        continue;
                    tobs[i, j, 2] = ((char)defcodes[i][(int)(p - frqcodes)]).ToString();
                    GlobalMembersRtkcmn.trace(2, "set default for unknown code: sys=%c code=%s\n", buff[0], tobs[i, j]);
                }
            }
            else if (StringFunctions.StrStr(label, "WAVELENGTH FACT L1/2")) // opt ver.2
            {
                ;
            }
            else if (StringFunctions.StrStr(label, "# / TYPES OF OBSERV")) // ver.2
            {
                n = (int)GlobalMembersRtkcmn.str2num(buff, 0, 6);
                for (i = nt = 0, j = 10; i < n; i++, j += 6)
                {
                    if (j > 58)
                    {
                        if (!fgets(buff, 16 * DefineConstants.MAXOBSTYPE + 4, fp))
                            break;
                        j = 10;
                    }
                    if (nt >= DefineConstants.MAXOBSTYPE - 1)
                        continue;
                    if (ver <= 2.99)
                    {
                        GlobalMembersRinex.setstr(ref str, buff.Substring(j), 2);
                        GlobalMembersConvrnx.convcode(ver, DefineConstants.SYS_GPS, str, tobs[0, nt]);
                        GlobalMembersConvrnx.convcode(ver, DefineConstants.SYS_GLO, str, tobs[1, nt]);
                        GlobalMembersConvrnx.convcode(ver, DefineConstants.SYS_GAL, str, tobs[2, nt]);
                        GlobalMembersConvrnx.convcode(ver, DefineConstants.SYS_QZS, str, tobs[3, nt]);
                        GlobalMembersConvrnx.convcode(ver, DefineConstants.SYS_SBS, str, tobs[4, nt]);
                        GlobalMembersConvrnx.convcode(ver, DefineConstants.SYS_CMP, str, tobs[5, nt]);
                    }
                    nt++;
                }
                tobs[0, nt] = '\0';
            }
            else if (StringFunctions.StrStr(label, "SIGNAL STRENGTH UNIT")) // opt ver.3
            {
                ;
            }
            else if (StringFunctions.StrStr(label, "INTERVAL")) // opt
            {
                ;
            }
            else if (StringFunctions.StrStr(label, "TIME OF FIRST OBS"))
            {
                if (!string.Compare(buff, 48, "GPS", 0, 3))
                {
                    tsys = DefineConstants.TSYS_GPS;
                }
                else if (!string.Compare(buff, 48, "GLO", 0, 3))
                {
                    tsys = DefineConstants.TSYS_UTC;
                }
                else if (!string.Compare(buff, 48, "GAL", 0, 3))
                {
                    tsys = DefineConstants.TSYS_GAL;
                }
                else if (!string.Compare(buff, 48, "QZS", 0, 3)) // ver.3.02
                {
                    tsys = DefineConstants.TSYS_QZS;
                }
                else if (!string.Compare(buff, 48, "BDT", 0, 3)) // ver.3.02
                {
                    tsys = DefineConstants.TSYS_CMP;
                }
            }
            else if (StringFunctions.StrStr(label, "TIME OF LAST OBS")) // opt
            {
                ;
            }
            else if (StringFunctions.StrStr(label, "RCV CLOCK OFFS APPL")) // opt
            {
                ;
            }
            else if (StringFunctions.StrStr(label, "SYS / DCBS APPLIED")) // opt ver.3
            {
                ;
            }
            else if (StringFunctions.StrStr(label, "SYS / PCVS APPLIED")) // opt ver.3
            {
                ;
            }
            else if (StringFunctions.StrStr(label, "SYS / SCALE FACTOR")) // opt ver.3
            {
                ;
            }
            else if (StringFunctions.StrStr(label, "SYS / PHASE SHIFTS")) // ver.3.01
            {
                ;
            }
            else if (StringFunctions.StrStr(label, "GLONASS SLOT / FRQ #")) // ver.3.02
            {
                if (nav != null)
                {
                    for (i = 0, p = buff.Substring(4); i < 8; i++, p += 8)
                    {
                        if (sscanf(p, "R%2d %2d", prn, fcn) < 2)
                            continue;
                        if (1 <= prn && prn <= DefineConstants.MAXPRNGLO)
                        {
                            nav.glo_fcn[prn - 1] = fcn + 8;
                        }
                    }
                }
            }
            else if (StringFunctions.StrStr(label, "GLONASS COD/PHS/BIS")) // ver.3.02
            {
                if (nav != null)
                {
                    for (i = 0, p = buff; i < 4; i++, p += 13)
                    {
                        if (string.Compare(p, 1, "C1C", 0, 3))
                        {
                            nav.glo_cpbias[0] = GlobalMembersRtkcmn.str2num(p, 5, 8);
                        }
                        else if (string.Compare(p, 1, "C1P", 0, 3))
                        {
                            nav.glo_cpbias[1] = GlobalMembersRtkcmn.str2num(p, 5, 8);
                        }
                        else if (string.Compare(p, 1, "C2C", 0, 3))
                        {
                            nav.glo_cpbias[2] = GlobalMembersRtkcmn.str2num(p, 5, 8);
                        }
                        else if (string.Compare(p, 1, "C2P", 0, 3))
                        {
                            nav.glo_cpbias[3] = GlobalMembersRtkcmn.str2num(p, 5, 8);
                        }
                    }
                }
            }
            else if (StringFunctions.StrStr(label, "LEAP SECONDS")) // opt
            {
                if (nav != null)
                {
                    nav.leaps = (int)GlobalMembersRtkcmn.str2num(buff, 0, 6);
                }
            }
            else if (StringFunctions.StrStr(label, "# OF SALTELLITES")) // opt
            {
                ;
            }
            else if (StringFunctions.StrStr(label, "PRN / # OF OBS")) // opt
            {
                ;
            }
        }
        /* decode nav header ---------------------------------------------------------*/
        internal static void decode_navh(ref string buff, nav_t nav)
        {
            int i;
            int j;
            string label = buff.Substring(60);

            GlobalMembersRtkcmn.trace(4, "decode_navh:\n");

            if (StringFunctions.StrStr(label, "ION ALPHA")) // opt ver.2
            {
                if (nav != null)
                {
                    for (i = 0, j = 2; i < 4; i++, j += 12)
                    {
                        nav.ion_gps[i] = GlobalMembersRtkcmn.str2num(buff, j, 12);
                    }
                }
            }
            else if (StringFunctions.StrStr(label, "ION BETA")) // opt ver.2
            {
                if (nav != null)
                {
                    for (i = 0, j = 2; i < 4; i++, j += 12)
                    {
                        nav.ion_gps[i + 4] = GlobalMembersRtkcmn.str2num(buff, j, 12);
                    }
                }
            }
            else if (StringFunctions.StrStr(label, "DELTA-UTC: A0,A1,T,W")) // opt ver.2
            {
                if (nav != null)
                {
                    for (i = 0, j = 3; i < 2; i++, j += 19)
                    {
                        nav.utc_gps[i] = GlobalMembersRtkcmn.str2num(buff, j, 19);
                    }
                    for (; i < 4; i++, j += 9)
                    {
                        nav.utc_gps[i] = GlobalMembersRtkcmn.str2num(buff, j, 9);
                    }
                }
            }
            else if (StringFunctions.StrStr(label, "IONOSPHERIC CORR")) // opt ver.3
            {
                if (nav != null)
                {
                    if (!string.Compare(buff, 0, "GPSA", 0, 4))
                    {
                        for (i = 0, j = 5; i < 4; i++, j += 12)
                        {
                            nav.ion_gps[i] = GlobalMembersRtkcmn.str2num(buff, j, 12);
                        }
                    }
                    else if (!string.Compare(buff, 0, "GPSB", 0, 4))
                    {
                        for (i = 0, j = 5; i < 4; i++, j += 12)
                        {
                            nav.ion_gps[i + 4] = GlobalMembersRtkcmn.str2num(buff, j, 12);
                        }
                    }
                    else if (!string.Compare(buff, 0, "GAL", 0, 3))
                    {
                        for (i = 0, j = 5; i < 4; i++, j += 12)
                        {
                            nav.ion_gal[i] = GlobalMembersRtkcmn.str2num(buff, j, 12);
                        }
                    }
                    else if (!string.Compare(buff, 0, "QZSA", 0, 4)) // v.3.02
                    {
                        for (i = 0, j = 5; i < 4; i++, j += 12)
                        {
                            nav.ion_qzs[i] = GlobalMembersRtkcmn.str2num(buff, j, 12);
                        }
                    }
                    else if (!string.Compare(buff, 0, "QZSB", 0, 4)) // v.3.02
                    {
                        for (i = 0, j = 5; i < 4; i++, j += 12)
                        {
                            nav.ion_qzs[i + 4] = GlobalMembersRtkcmn.str2num(buff, j, 12);
                        }
                    }
                    else if (!string.Compare(buff, 0, "BDSA", 0, 4)) // v.3.02
                    {
                        for (i = 0, j = 5; i < 4; i++, j += 12)
                        {
                            nav.ion_cmp[i] = GlobalMembersRtkcmn.str2num(buff, j, 12);
                        }
                    }
                    else if (!string.Compare(buff, 0, "BDSB", 0, 4)) // v.3.02
                    {
                        for (i = 0, j = 5; i < 4; i++, j += 12)
                        {
                            nav.ion_cmp[i + 4] = GlobalMembersRtkcmn.str2num(buff, j, 12);
                        }
                    }
                }
            }
            else if (StringFunctions.StrStr(label, "TIME SYSTEM CORR")) // opt ver.3
            {
                if (nav != null)
                {
                    if (!string.Compare(buff, 0, "GPUT", 0, 4))
                    {
                        nav.utc_gps[0] = GlobalMembersRtkcmn.str2num(buff, 5, 17);
                        nav.utc_gps[1] = GlobalMembersRtkcmn.str2num(buff, 22, 16);
                        nav.utc_gps[2] = GlobalMembersRtkcmn.str2num(buff, 38, 7);
                        nav.utc_gps[3] = GlobalMembersRtkcmn.str2num(buff, 45, 5);
                    }
                    else if (!string.Compare(buff, 0, "GLUT", 0, 4))
                    {
                        nav.utc_glo[0] = GlobalMembersRtkcmn.str2num(buff, 5, 17);
                        nav.utc_glo[1] = GlobalMembersRtkcmn.str2num(buff, 22, 16);
                    }
                    else if (!string.Compare(buff, 0, "GAUT", 0, 4)) // v.3.02
                    {
                        nav.utc_gal[0] = GlobalMembersRtkcmn.str2num(buff, 5, 17);
                        nav.utc_gal[1] = GlobalMembersRtkcmn.str2num(buff, 22, 16);
                        nav.utc_gal[2] = GlobalMembersRtkcmn.str2num(buff, 38, 7);
                        nav.utc_gal[3] = GlobalMembersRtkcmn.str2num(buff, 45, 5);
                    }
                    else if (!string.Compare(buff, 0, "QZUT", 0, 4)) // v.3.02
                    {
                        nav.utc_qzs[0] = GlobalMembersRtkcmn.str2num(buff, 5, 17);
                        nav.utc_qzs[1] = GlobalMembersRtkcmn.str2num(buff, 22, 16);
                        nav.utc_qzs[2] = GlobalMembersRtkcmn.str2num(buff, 38, 7);
                        nav.utc_qzs[3] = GlobalMembersRtkcmn.str2num(buff, 45, 5);
                    }
                    else if (!string.Compare(buff, 0, "BDUT", 0, 4)) // v.3.02
                    {
                        nav.utc_cmp[0] = GlobalMembersRtkcmn.str2num(buff, 5, 17);
                        nav.utc_cmp[1] = GlobalMembersRtkcmn.str2num(buff, 22, 16);
                        nav.utc_cmp[2] = GlobalMembersRtkcmn.str2num(buff, 38, 7);
                        nav.utc_cmp[3] = GlobalMembersRtkcmn.str2num(buff, 45, 5);
                    }
                    else if (!string.Compare(buff, 0, "SBUT", 0, 4)) // v.3.02
                    {
                        nav.utc_cmp[0] = GlobalMembersRtkcmn.str2num(buff, 5, 17);
                        nav.utc_cmp[1] = GlobalMembersRtkcmn.str2num(buff, 22, 16);
                        nav.utc_cmp[2] = GlobalMembersRtkcmn.str2num(buff, 38, 7);
                        nav.utc_cmp[3] = GlobalMembersRtkcmn.str2num(buff, 45, 5);
                    }
                }
            }
            else if (StringFunctions.StrStr(label, "LEAP SECONDS")) // opt
            {
                if (nav != null)
                {
                    nav.leaps = (int)GlobalMembersRtkcmn.str2num(buff, 0, 6);
                }
            }
        }
        /* decode gnav header --------------------------------------------------------*/
        internal static void decode_gnavh(ref string buff, nav_t nav)
        {
            string label = buff.Substring(60);

            GlobalMembersRtkcmn.trace(4, "decode_gnavh:\n");

            if (StringFunctions.StrStr(label, "CORR TO SYTEM TIME")) // opt
            {
                ;
            }
            else if (StringFunctions.StrStr(label, "LEAP SECONDS")) // opt
            {
                if (nav != null)
                {
                    nav.leaps = (int)GlobalMembersRtkcmn.str2num(buff, 0, 6);
                }
            }
        }
        /* decode geo nav header -----------------------------------------------------*/
        internal static void decode_hnavh(ref string buff, nav_t nav)
        {
            string label = buff.Substring(60);

            GlobalMembersRtkcmn.trace(4, "decode_hnavh:\n");

            if (StringFunctions.StrStr(label, "CORR TO SYTEM TIME")) // opt
            {
                ;
            }
            else if (StringFunctions.StrStr(label, "D-UTC A0,A1,T,W,S,U")) // opt
            {
                ;
            }
            else if (StringFunctions.StrStr(label, "LEAP SECONDS")) // opt
            {
                if (nav != null)
                {
                    nav.leaps = (int)GlobalMembersRtkcmn.str2num(buff, 0, 6);
                }
            }
        }
        /* read rinex header ---------------------------------------------------------*/
        internal static int readrnxh(FILE fp, ref double ver, ref string type, ref int sys, ref int tsys, string[,] tobs, nav_t nav, sta_t sta)
        {
            double bias;
            string buff = new string(new char[16 * DefineConstants.MAXOBSTYPE + 4]);
            string label = buff.Substring(60);
            int i = 0;
            int block = 0;
            int sat;

            GlobalMembersRtkcmn.trace(3, "readrnxh:\n");

            ver = 2.10;
            type = (sbyte)' ';
            sys = DefineConstants.SYS_GPS;
            tsys = DefineConstants.TSYS_GPS;

            while (fgets(buff, 16 * DefineConstants.MAXOBSTYPE + 4, fp))
            {

                if (buff.Length <= 60)
                    continue;

                else if (StringFunctions.StrStr(label, "RINEX VERSION / TYPE"))
                {
                    ver = GlobalMembersRtkcmn.str2num(buff, 0, 9);
                    type = *(buff.Substring(20));

                    /* satellite system */
                    switch (*(buff.Substring(40)))
                    {
                        case ' ':
                        case 'G':
                            sys = DefineConstants.SYS_GPS;
                            tsys = DefineConstants.TSYS_GPS;
                            break;
                        case 'R':
                            sys = DefineConstants.SYS_GLO;
                            tsys = DefineConstants.TSYS_UTC;
                            break;
                        case 'E': // v.2.12
                            sys = DefineConstants.SYS_GAL;
                            tsys = DefineConstants.TSYS_GAL;
                            break;
                        case 'S':
                            sys = DefineConstants.SYS_SBS;
                            tsys = DefineConstants.TSYS_GPS;
                            break;
                        case 'J': // v.3.02
                            sys = DefineConstants.SYS_QZS;
                            tsys = DefineConstants.TSYS_QZS;
                            break;
                        case 'C': // v.2.12
                            sys = DefineConstants.SYS_CMP;
                            tsys = DefineConstants.TSYS_CMP;
                            break;
                        case 'M': // mixed
                            sys = DefineConstants.SYS_NONE;
                            tsys = DefineConstants.TSYS_GPS;
                            break;
                        default:
                            GlobalMembersRtkcmn.trace(2, "not supported satellite system: %c\n", *(buff.Substring(40)));
                            break;
                    }
                    continue;
                }
                else if (StringFunctions.StrStr(label, "PGM / RUN BY / DATE"))
                    continue;
                else if (StringFunctions.StrStr(label, "COMMENT")) // opt
                {

                    /* read cnes wl satellite fractional bias */
                    if (StringFunctions.StrStr(buff, "WIDELANE SATELLITE FRACTIONAL BIASES") || StringFunctions.StrStr(buff, "WIDELANE SATELLITE FRACTIONNAL BIASES"))
                    {
                        block = 1;
                    }
                    else if (block != 0)
                    {
                        /* cnes/cls grg clock */
                        if (!string.Compare(buff, 0, "WL", 0, 2) && (sat = GlobalMembersRtkcmn.satid2no(buff.Substring(3))) != 0 && sscanf(buff.Substring(40), "%lf", bias) == 1)
                        {
                            nav.wlbias[sat - 1] = bias;
                        }
                        /* cnes ppp-wizard clock */
                        else if ((sat = GlobalMembersRtkcmn.satid2no(buff.Substring(1))) != 0 && sscanf(buff.Substring(6), "%lf", bias) == 1)
                        {
                            nav.wlbias[sat - 1] = bias;
                        }
                    }
                    continue;
                }
                /* file type */
                switch (type)
                {
                    case 'O':
                        GlobalMembersRinex.decode_obsh(fp, ref buff, ver, ref tsys, tobs, nav, sta);
                        break;
                    case 'N':
                        GlobalMembersRinex.decode_navh(ref buff, nav);
                        break;
                    case 'G':
                        GlobalMembersRinex.decode_gnavh(ref buff, nav);
                        break;
                    case 'H':
                        GlobalMembersRinex.decode_hnavh(ref buff, nav);
                        break;
                    case 'J': // extension
                        GlobalMembersRinex.decode_navh(ref buff, nav);
                        break;
                    case 'L': // extension
                        GlobalMembersRinex.decode_navh(ref buff, nav);
                        break;
                }
                if (StringFunctions.StrStr(label, "END OF HEADER"))
                {
                    return 1;
                }

                if (++i >= DefineConstants.MAXPOSHEAD && type == ' ') // no rinex file
                    break;
            }
            return 0;
        }
        /* decode obs epoch ----------------------------------------------------------*/
        internal static int decode_obsepoch(FILE fp, ref string buff, double ver, gtime_t time, ref int flag, int[] sats)
        {
            int i;
            int j;
            int n;
            string satid = "";

            GlobalMembersRtkcmn.trace(4, "decode_obsepoch: ver=%.2f\n", ver);

            if (ver <= 2.99) // ver.2
            {
                if ((n = (int)GlobalMembersRtkcmn.str2num(buff, 29, 3)) <= 0)
                {
                    return 0;
                }

                /* epoch flag: 3:new site,4:header info,5:external event */
                flag = (int)GlobalMembersRtkcmn.str2num(buff, 28, 1);

                if (3 <= flag && flag <= 5)
                {
                    return n;
                }

                if (GlobalMembersRtkcmn.str2time(buff, 0, 26, time) != 0)
                {
                    GlobalMembersRtkcmn.trace(2, "rinex obs invalid epoch: epoch=%26.26s\n", buff);
                    return 0;
                }
                for (i = 0, j = 32; i < n; i++, j += 3)
                {
                    if (j >= 68)
                    {
                        if (!fgets(buff, 16 * DefineConstants.MAXOBSTYPE + 4, fp))
                            break;
                        j = 32;
                    }
                    if (i < DefineConstants.MAXOBS)
                    {
                        satid = buff.Substring(j, 3);
                        sats[i] = GlobalMembersRtkcmn.satid2no(satid);
                    }
                }
            }
            else // ver.3
            {
                if ((n = (int)GlobalMembersRtkcmn.str2num(buff, 32, 3)) <= 0)
                {
                    return 0;
                }

                flag = (int)GlobalMembersRtkcmn.str2num(buff, 31, 1);

                if (3 <= flag && flag <= 5)
                {
                    return n;
                }

                if (buff[0] != '>' || GlobalMembersRtkcmn.str2time(buff, 1, 28, time) != 0)
                {
                    GlobalMembersRtkcmn.trace(2, "rinex obs invalid epoch: epoch=%29.29s\n", buff);
                    return 0;
                }
            }
            GlobalMembersRtkcmn.trace(4, "decode_obsepoch: time=%s flag=%d\n", GlobalMembersRtkcmn.time_str(time, 3), flag);
            return n;
        }
        /* decode obs data -----------------------------------------------------------*/
        internal static int decode_obsdata(FILE fp, ref string buff, double ver, int mask, sigind_t index, obsd_t obs)
        {
            sigind_t ind;
            double[] val = { 0 };
            byte[] lli = new byte[DefineConstants.MAXOBSTYPE];
            string satid = "";
            int i;
            int j;
            int n;
            int m;
            int stat = 1;
            int[] p = new int[DefineConstants.MAXOBSTYPE];
            int[] k = new int[16];
            int[] l = new int[16];

            GlobalMembersRtkcmn.trace(4, "decode_obsdata: ver=%.2f\n", ver);

            if (ver > 2.99) // ver.3
            {
                satid = buff.Substring(0, 3);
                obs.sat = (byte)GlobalMembersRtkcmn.satid2no(satid);
            }
            if (obs.sat == 0)
            {
                GlobalMembersRtkcmn.trace(4, "decode_obsdata: unsupported sat sat=%s\n", satid);
                stat = 0;
            }
            else if (!(GlobalMembersRtkcmn.satsys(obs.sat, null) & mask))
            {
                stat = 0;
            }
            /* read obs data fields */
            switch (GlobalMembersRtkcmn.satsys(obs.sat, null))
            {
                case DefineConstants.SYS_GLO:
                    ind = index + 1;
                    break;
                case DefineConstants.SYS_GAL:
                    ind = index + 2;
                    break;
                case DefineConstants.SYS_QZS:
                    ind = index + 3;
                    break;
                case DefineConstants.SYS_SBS:
                    ind = index + 4;
                    break;
                case DefineConstants.SYS_CMP:
                    ind = index + 5;
                    break;
                default:
                    ind = index;
                    break;
            }
            for (i = 0, j = ver <= 2.99 ? 0 : 3; i < ind.n; i++, j += 16)
            {

                if (ver <= 2.99 && j >= 80) // ver.2
                {
                    if (!fgets(buff, 16 * DefineConstants.MAXOBSTYPE + 4, fp))
                        break;
                    j = 0;
                }
                if (stat != 0)
                {
                    val[i] = GlobalMembersRtkcmn.str2num(buff, j, 14) + ind.shift[i];
                    lli[i] = (byte)GlobalMembersRtkcmn.str2num(buff, j + 14, 1) & 3;
                }
            }
            if (stat == 0)
            {
                return 0;
            }

            for (i = 0; i < DefineConstants.NFREQ + DefineConstants.NEXOBS; i++)
            {
                obs.P[i] = obs.L[i] = 0.0;
                obs.D[i] = 0.0f;
                obs.SNR[i] = obs.LLI[i] = obs.code[i] = 0;
            }
            /* assign position in obs data */
            for (i = n = m = 0; i < ind.n; i++)
            {

                p[i] = ver <= 2.11 ? ind.frq[i] - 1 : ind.pos[i];

                if (ind.type[i] == 0 && p[i] == 0) // C1? index
                {
                    k[n++] = i;
                }
                if (ind.type[i] == 0 && p[i] == 1) // C2? index
                {
                    l[m++] = i;
                }
            }
            if (ver <= 2.11)
            {

                /* if multiple codes (C1/P1,C2/P2), select higher priority */
                if (n >= 2)
                {
                    if (val[k[0]] == 0.0 && val[k[1]] == 0.0)
                    {
                        p[k[0]] = -1;
                        p[k[1]] = -1;
                    }
                    else if (val[k[0]] != 0.0 && val[k[1]] == 0.0)
                    {
                        p[k[0]] = 0;
                        p[k[1]] = -1;
                    }
                    else if (val[k[0]] == 0.0 && val[k[1]] != 0.0)
                    {
                        p[k[0]] = -1;
                        p[k[1]] = 0;
                    }
                    else if (ind.pri[k[1]] > ind.pri[k[0]])
                    {
                        p[k[1]] = 0;
                        p[k[0]] = DefineConstants.NEXOBS < 1 ? -1 : DefineConstants.NFREQ;
                    }
                    else
                    {
                        p[k[0]] = 0;
                        p[k[1]] = DefineConstants.NEXOBS < 1 ? -1 : DefineConstants.NFREQ;
                    }
                }
                if (m >= 2)
                {
                    if (val[l[0]] == 0.0 && val[l[1]] == 0.0)
                    {
                        p[l[0]] = -1;
                        p[l[1]] = -1;
                    }
                    else if (val[l[0]] != 0.0 && val[l[1]] == 0.0)
                    {
                        p[l[0]] = 1;
                        p[l[1]] = -1;
                    }
                    else if (val[l[0]] == 0.0 && val[l[1]] != 0.0)
                    {
                        p[l[0]] = -1;
                        p[l[1]] = 1;
                    }
                    else if (ind.pri[l[1]] > ind.pri[l[0]])
                    {
                        p[l[1]] = 1;
                        p[l[0]] = DefineConstants.NEXOBS < 2 ? -1 : DefineConstants.NFREQ + 1;
                    }
                    else
                    {
                        p[l[0]] = 1;
                        p[l[1]] = DefineConstants.NEXOBS < 2 ? -1 : DefineConstants.NFREQ + 1;
                    }
                }
            }
            /* save obs data */
            for (i = 0; i < ind.n; i++)
            {
                if (p[i] < 0 || val[i] == 0.0)
                    continue;
                switch (ind.type[i])
                {
                    case 0:
                        obs.P[p[i]] = val[i];
                        obs.code[p[i]] = ind.code[i];
                        break;
                    case 1:
                        obs.L[p[i]] = val[i];
                        obs.LLI[p[i]] = lli[i];
                        break;
                    case 2:
                        obs.D[p[i]] = (float)val[i];
                        break;
                    case 3:
                        obs.SNR[p[i]] = (byte)(val[i] * 4.0 + 0.5);
                        break;
                }
            }
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: trace(4,"decode_obsdata: time=%s sat=%2d\n",time_str(obs->time,0),obs->sat);
            GlobalMembersRtkcmn.trace(4, "decode_obsdata: time=%s sat=%2d\n", GlobalMembersRtkcmn.time_str(new gtime_t(obs.time), 0), obs.sat);
            return 1;
        }
        /* save slips ----------------------------------------------------------------*/
        internal static void saveslips(byte[,] slips, obsd_t data)
        {
            int i;
            for (i = 0; i < DefineConstants.NFREQ; i++)
            {
                if (data.LLI[i] & 1)
                {
                    slips[data.sat - 1, i] |= 1;
                }
            }
        }
        /* restore slips -------------------------------------------------------------*/
        internal static void restslips(byte[,] slips, obsd_t data)
        {
            int i;
            for (i = 0; i < DefineConstants.NFREQ; i++)
            {
                if (slips[data.sat - 1, i] & 1)
                {
                    data.LLI[i] |= 1;
                }
                slips[data.sat - 1, i] = 0;
            }
        }
        /* add obs data --------------------------------------------------------------*/
        internal static int addobsdata(obs_t obs, obsd_t[] data)
        {
            obsd_t obs_data;

            if (obs.nmax <= obs.n)
            {
                if (obs.nmax <= 0)
                {
                    obs.nmax = DefineConstants.NINCOBS;
                }
                else
                {
                    obs.nmax *= 2;
                }
                //C++ TO C# CONVERTER TODO TASK: The memory management function 'realloc' has no equivalent in C#:
                if ((obs_data = (obsd_t)realloc(obs.data, sizeof(obsd_t) * obs.nmax)) == null)
                {
                    GlobalMembersRtkcmn.trace(1, "addobsdata: memalloc error n=%dx%d\n", sizeof(obsd_t), obs.nmax);
                    //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
                    free(obs.data);
                    obs.data = null;
                    obs.n = obs.nmax = 0;
                    return -1;
                }
                obs.data = obs_data;
            }
            obs.data[obs.n++] = *data;
            return 1;
        }
        /* set system mask -----------------------------------------------------------*/
        internal static int set_sysmask(string opt)
        {
            //C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on this variable, so pointers on this variable are left unchanged:
            sbyte* p;
            int mask = DefineConstants.SYS_NONE;

            if ((p = StringFunctions.StrStr(opt, "-SYS=")) == 0)
            {
                return DefineConstants.SYS_ALL;
            }

            for (p + = 5; *p && *p != ' '; p++)
            {
                switch (*p)
                {
                    case 'G':
                        mask |= DefineConstants.SYS_GPS;
                        break;
                    case 'R':
                        mask |= DefineConstants.SYS_GLO;
                        break;
                    case 'E':
                        mask |= DefineConstants.SYS_GAL;
                        break;
                    case 'J':
                        mask |= DefineConstants.SYS_QZS;
                        break;
                    case 'C':
                        mask |= DefineConstants.SYS_CMP;
                        break;
                    case 'S':
                        mask |= DefineConstants.SYS_SBS;
                        break;
                }
            }
            return mask;
        }
        /* set signal index ----------------------------------------------------------*/
        internal static void set_index(double ver, int sys, string opt, string[] tobs, sigind_t ind)
        {
            string p;
            string str = new string(new char[8]);
            string optstr = "";
            double shift;
            int i;
            int j;
            int k;
            int n;

            for (i = n = 0; *tobs[i]; i++, n++)
            {
                ind.code[i] = GlobalMembersRtkcmn.obs2code(tobs[i] + 1, ref ind.frq + i);
                ind.type[i] = (p = StringFunctions.StrChr(obscodes, tobs[i, 0])) != 0 ? (int)(p - obscodes) : 0;
                ind.pri[i] = GlobalMembersRtkcmn.getcodepri(sys, ind.code[i], opt);
                ind.pos[i] = -1;

                /* frequency index for beidou */
                if (sys == DefineConstants.SYS_CMP)
                {
                    if (ind.frq[i] == 5) // B2
                    {
                        ind.frq[i] = 2;
                    }
                    else if (ind.frq[i] == 4) // B3
                    {
                        ind.frq[i] = 3;
                    }
                }
            }
            /* parse phase shift options */
            switch (sys)
            {
                case DefineConstants.SYS_GPS:
                    optstr = "-GL%2s=%lf";
                    break;
                case DefineConstants.SYS_GLO:
                    optstr = "-RL%2s=%lf";
                    break;
                case DefineConstants.SYS_GAL:
                    optstr = "-EL%2s=%lf";
                    break;
                case DefineConstants.SYS_QZS:
                    optstr = "-JL%2s=%lf";
                    break;
                case DefineConstants.SYS_SBS:
                    optstr = "-SL%2s=%lf";
                    break;
                case DefineConstants.SYS_CMP:
                    optstr = "-CL%2s=%lf";
                    break;
            }
            for (p = opt; p && (p = StringFunctions.StrChr(p, '-')); p++)
            {
                if (sscanf(p, optstr, str, shift) < 2)
                    continue;
                for (i = 0; i < n; i++)
                {
                    if (string.Compare(GlobalMembersRtkcmn.code2obs(ind.code[i], null), str))
                        continue;
                    ind.shift[i] = shift;
                    GlobalMembersRtkcmn.trace(2, "phase shift: sys=%2d tobs=%s shift=%.3f\n", sys, tobs[i], shift);
                }
            }
            /* assign index for highest priority code */
            for (i = 0; i < DefineConstants.NFREQ; i++)
            {
                for (j = 0, k = -1; j < n; j++)
                {
                    if (ind.frq[j] == i + 1 && ind.pri[j] != 0 && (k < 0 || ind.pri[j] > ind.pri[k]))
                    {
                        k = j;
                    }
                }
                if (k < 0)
                    continue;

                for (j = 0; j < n; j++)
                {
                    if (ind.code[j] == ind.code[k])
                    {
                        ind.pos[j] = i;
                    }
                }
            }
            /* assign index of extended obs data */
            for (i = 0; i < DefineConstants.NEXOBS; i++)
            {
                for (j = 0; j < n; j++)
                {
                    if (ind.code[j] != 0 && ind.pri[j] != 0 && ind.pos[j] < 0)
                        break;
                }
                if (j >= n)
                    break;

                for (k = 0; k < n; k++)
                {
                    if (ind.code[k] == ind.code[j])
                    {
                        ind.pos[k] = DefineConstants.NFREQ + i;
                    }
                }
            }
            for (i = 0; i < n; i++)
            {
                if (ind.code[i] == 0 || ind.pri[i] == 0 || ind.pos[i] >= 0)
                    continue;
                GlobalMembersRtkcmn.trace(4, "reject obs type: sys=%2d, obs=%s\n", sys, tobs[i]);
            }
            ind.n = n;

#if false
	//    for (i=0;i<n;i++) {
	//        trace(2,"set_index: sys=%2d,tobs=%s code=%2d pri=%2d frq=%d pos=%d shift=%5.2f\n",
	//              sys,tobs[i],ind->code[i],ind->pri[i],ind->frq[i],ind->pos[i],
	//              ind->shift[i]);
	//    }
#endif
        }
        /* read rinex obs data body --------------------------------------------------*/
        internal static int readrnxobsb(FILE fp, string opt, double ver, string[,] tobs, ref int flag, obsd_t[] data)
        {
            gtime_t time = new gtime_t();
            sigind_t[] index = { { 0 } };
            string buff = new string(new char[16 * DefineConstants.MAXOBSTYPE + 4]);
            int i = 0;
            int n = 0;
            int nsat = 0;
            int[] sats = new int[DefineConstants.MAXOBS];
            int mask;

            /* set system mask */
            mask = GlobalMembersRinex.set_sysmask(opt);

            /* set signal index */
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: set_index(ver,DefineConstants.SYS_GPS,opt,tobs[0],index);
            GlobalMembersRinex.set_index(ver, DefineConstants.SYS_GPS, opt, tobs[0], new sigind_t(index));
            GlobalMembersRinex.set_index(ver, DefineConstants.SYS_GLO, opt, tobs[1], index + 1);
            GlobalMembersRinex.set_index(ver, DefineConstants.SYS_GAL, opt, tobs[2], index + 2);
            GlobalMembersRinex.set_index(ver, DefineConstants.SYS_QZS, opt, tobs[3], index + 3);
            GlobalMembersRinex.set_index(ver, DefineConstants.SYS_SBS, opt, tobs[4], index + 4);
            GlobalMembersRinex.set_index(ver, DefineConstants.SYS_CMP, opt, tobs[5], index + 5);

            /* read record */
            while (fgets(buff, 16 * DefineConstants.MAXOBSTYPE + 4, fp))
            {

                /* decode obs epoch */
                if (i == 0)
                {
                    if ((nsat = GlobalMembersRinex.decode_obsepoch(fp, ref buff, ver, time, ref flag, sats)) <= 0)
                    {
                        continue;
                    }
                }
                else if (flag <= 2 || flag == 6)
                {

                    data[n].time = time;
                    data[n].sat = (byte)sats[i - 1];

                    /* decode obs data */
                    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                    //ORIGINAL LINE: if (decode_obsdata(fp,buff,ver,mask,index,data+n)&&n<DefineConstants.MAXOBS)
                    if (GlobalMembersRinex.decode_obsdata(fp, ref buff, ver, mask, new sigind_t(index), data + n) != 0 && n < DefineConstants.MAXOBS)
                    {
                        n++;
                    }
                }
                if (++i > nsat)
                {
                    return n;
                }
            }
            return -1;
        }
        /* read rinex obs ------------------------------------------------------------*/
        internal static int readrnxobs(FILE fp, gtime_t ts, gtime_t te, double tint, string opt, int rcv, double ver, int tsys, string[,] tobs, obs_t obs)
        {
            obsd_t[] data;
            byte[,] slips = new byte[DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1, DefineConstants.NFREQ];
            int i;
            int n;
            int flag = 0;
            int stat = 0;

            GlobalMembersRtkcmn.trace(4, "readrnxobs: rcv=%d ver=%.2f tsys=%d\n", rcv, ver, tsys);

            if (obs == null || rcv > DefineConstants.MAXRCV)
            {
                return 0;
            }

            if (!(data = new obsd_t[DefineConstants.MAXOBS]))
            {
                return 0;
            }

            /* read rinex obs data body */
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: while ((n=readrnxobsb(fp,opt,ver,tobs,&flag,data))>=0&&stat>=0)
            while ((n = GlobalMembersRinex.readrnxobsb(fp, opt, ver, tobs, ref flag, new obsd_t(data))) >= 0 && stat >= 0)
            {

                for (i = 0; i < n; i++)
                {

                    /* utc -> gpst */
                    if (tsys == DefineConstants.TSYS_UTC)
                    {
                        //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                        //ORIGINAL LINE: data[i].time=utc2gpst(data[i].time);
                        //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                        data[i].time.CopyFrom(GlobalMembersRtkcmn.utc2gpst(new gtime_t(data[i].time)));
                    }

                    /* save cycle-slip */
                    GlobalMembersConvrnx.saveslips(slips, data + i);
                }
                /* screen data by time */
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: if (n>0&&!screent(data[0].time,ts,te,tint))
                if (n > 0 && GlobalMembersRtkcmn.screent(new gtime_t(data[0].time), new gtime_t(ts), new gtime_t(te), tint) == 0)
                    continue;

                for (i = 0; i < n; i++)
                {

                    /* restore cycle-slip */
                    GlobalMembersConvrnx.restslips(slips, data + i);

                    data[i].rcv = (byte)rcv;

                    /* save obs data */
                    if ((stat = GlobalMembersRinex.addobsdata(obs, data + i)) < 0)
                        break;
                }
            }
            GlobalMembersRtkcmn.trace(4, "readrnxobs: nobs=%d stat=%d\n", obs.n, stat);

            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(data);

            return stat;
        }
        /* decode ephemeris ----------------------------------------------------------*/
        internal static int decode_eph(double ver, int sat, gtime_t toc, double[] data, eph_t eph)
        {
            eph_t eph0 = new eph_t();
            int sys;

            GlobalMembersRtkcmn.trace(4, "decode_eph: ver=%.2f sat=%2d\n", ver, sat);

            sys = GlobalMembersRtkcmn.satsys(sat, null);

            if (!(sys & (DefineConstants.SYS_GPS | DefineConstants.SYS_GAL | DefineConstants.SYS_QZS | DefineConstants.SYS_CMP)))
            {
                GlobalMembersRtkcmn.trace(2, "ephemeris error: invalid satellite sat=%2d\n", sat);
                return 0;
            }
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: *eph=eph0;
            eph.CopyFrom(eph0);

            eph.sat = sat;
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: eph->toc=toc;
            eph.toc.CopyFrom(toc);

            eph.f0 = data[0];
            eph.f1 = data[1];
            eph.f2 = data[2];

            eph.A = ((data[10]) * (data[10]));
            eph.e = data[8];
            eph.i0 = data[15];
            eph.OMG0 = data[13];
            eph.omg = data[17];
            eph.M0 = data[6];
            eph.deln = data[5];
            eph.OMGd = data[18];
            eph.idot = data[19];
            eph.crc = data[16];
            eph.crs = data[4];
            eph.cuc = data[7];
            eph.cus = data[9];
            eph.cic = data[12];
            eph.cis = data[14];

            if (sys == DefineConstants.SYS_GPS || sys == DefineConstants.SYS_QZS)
            {
                eph.iode = (int)data[3]; // IODE
                eph.iodc = (int)data[26]; // IODC
                eph.toes = data[11]; // toe (s) in gps week
                eph.week = (int)data[21]; // gps week
                                          //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                                          //ORIGINAL LINE: eph->toe=adjweek(gpst2time(eph->week,data[11]),toc);
                                          //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                eph.toe.CopyFrom(GlobalMembersBinex.adjweek(GlobalMembersRtkcmn.gpst2time(eph.week, data[11]), new gtime_t(toc)));
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                //ORIGINAL LINE: eph->ttr=adjweek(gpst2time(eph->week,data[27]),toc);
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                eph.ttr.CopyFrom(GlobalMembersBinex.adjweek(GlobalMembersRtkcmn.gpst2time(eph.week, data[27]), new gtime_t(toc)));

                eph.code = (int)data[20]; // GPS: codes on L2 ch
                eph.svh = (int)data[24]; // sv health
                eph.sva = GlobalMembersBinex.uraindex(data[23]); // ura (m->index)
                eph.flag = (int)data[22]; // GPS: L2 P data flag

                eph.tgd[0] = data[25]; // TGD
                eph.fit = data[28]; // fit interval
            }
            else if (sys == DefineConstants.SYS_GAL) // GAL ver.3
            {
                eph.iode = (int)data[3]; // IODnav
                eph.toes = data[11]; // toe (s) in galileo week
                eph.week = (int)data[21]; // gal week = gps week
                                          //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                                          //ORIGINAL LINE: eph->toe=adjweek(gpst2time(eph->week,data[11]),toc);
                                          //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                eph.toe.CopyFrom(GlobalMembersBinex.adjweek(GlobalMembersRtkcmn.gpst2time(eph.week, data[11]), new gtime_t(toc)));
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                //ORIGINAL LINE: eph->ttr=adjweek(gpst2time(eph->week,data[27]),toc);
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                eph.ttr.CopyFrom(GlobalMembersBinex.adjweek(GlobalMembersRtkcmn.gpst2time(eph.week, data[27]), new gtime_t(toc)));

                eph.code = (int)data[20]; // data sources
                                          /* bit 0 set: I/NAV E1-B */
                                          /* bit 1 set: F/NAV E5a-I */
                                          /* bit 2 set: F/NAV E5b-I */
                                          /* bit 8 set: af0-af2 toc are for E5a.E1 */
                                          /* bit 9 set: af0-af2 toc are for E5b.E1 */
                eph.svh = (int)data[24]; // sv health
                                         /* bit     0: E1B DVS */
                                         /* bit   1-2: E1B HS */
                                         /* bit     3: E5a DVS */
                                         /* bit   4-5: E5a HS */
                                         /* bit     6: E5b DVS */
                                         /* bit   7-8: E5b HS */
                eph.sva = GlobalMembersBinex.uraindex(data[23]); // ura (m->index)

                eph.tgd[0] = data[25]; // BGD E5a/E1
                eph.tgd[1] = data[26]; // BGD E5b/E1
            }
            else if (sys == DefineConstants.SYS_CMP) // BeiDou v.3.02
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                //ORIGINAL LINE: eph->toc=bdt2gpst(eph->toc);
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                eph.toc.CopyFrom(GlobalMembersRtkcmn.bdt2gpst(new gtime_t(eph.toc))); // bdt -> gpst
                eph.iode = (int)data[3]; // AODE
                eph.iodc = (int)data[28]; // AODC
                eph.toes = data[11]; // toe (s) in bdt week
                eph.week = (int)data[21]; // bdt week
                                          //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                                          //ORIGINAL LINE: eph->toe=bdt2gpst(bdt2time(eph->week,data[11]));
                eph.toe.CopyFrom(GlobalMembersRtkcmn.bdt2gpst(GlobalMembersRtkcmn.bdt2time(eph.week, data[11]))); // bdt -> gpst
                                                                                                                  //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                                                                                                                  //ORIGINAL LINE: eph->ttr=bdt2gpst(bdt2time(eph->week,data[27]));
                eph.ttr.CopyFrom(GlobalMembersRtkcmn.bdt2gpst(GlobalMembersRtkcmn.bdt2time(eph.week, data[27]))); // bdt -> gpst
                                                                                                                  //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                                                                                                                  //ORIGINAL LINE: eph->toe=adjweek(eph->toe,toc);
                                                                                                                  //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                eph.toe.CopyFrom(GlobalMembersBinex.adjweek(new gtime_t(eph.toe), new gtime_t(toc)));
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                //ORIGINAL LINE: eph->ttr=adjweek(eph->ttr,toc);
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                eph.ttr.CopyFrom(GlobalMembersBinex.adjweek(new gtime_t(eph.ttr), new gtime_t(toc)));

                eph.svh = (int)data[24]; // satH1
                eph.sva = GlobalMembersBinex.uraindex(data[23]); // ura (m->index)

                eph.tgd[0] = data[25]; // TGD1 B1/B3
                eph.tgd[1] = data[26]; // TGD2 B2/B3
            }
            if (eph.iode < 0 || 1023 < eph.iode)
            {
                GlobalMembersRtkcmn.trace(2, "rinex nav invalid: sat=%2d iode=%d\n", sat, eph.iode);
            }
            if (eph.iodc < 0 || 1023 < eph.iodc)
            {
                GlobalMembersRtkcmn.trace(2, "rinex nav invalid: sat=%2d iodc=%d\n", sat, eph.iodc);
            }
            return 1;
        }
        /* decode glonass ephemeris --------------------------------------------------*/
        internal static int decode_geph(double ver, int sat, gtime_t toc, double[] data, geph_t geph)
        {
            geph_t geph0 = new geph_t();
            gtime_t tof = new gtime_t();
            double tow;
            double tod;
            int week;
            int dow;

            GlobalMembersRtkcmn.trace(4, "decode_geph: ver=%.2f sat=%2d\n", ver, sat);

            if (GlobalMembersRtkcmn.satsys(sat, null) != DefineConstants.SYS_GLO)
            {
                GlobalMembersRtkcmn.trace(2, "glonass ephemeris error: invalid satellite sat=%2d\n", sat);
                return 0;
            }
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: *geph=geph0;
            geph.CopyFrom(geph0);

            geph.sat = sat;

            /* toc rounded by 15 min in utc */
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: tow=time2gpst(toc,&week);
            tow = GlobalMembersRtkcmn.time2gpst(new gtime_t(toc), ref week);
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: toc=gpst2time(week,floor((tow+450.0)/900.0)*900);
            toc.CopyFrom(GlobalMembersRtkcmn.gpst2time(week, Math.Floor((tow + 450.0) / 900.0) * 900));
            dow = (int)Math.Floor(tow / 86400.0);

            /* time of frame in utc */
            tod = ver <= 2.99 ? data[2] : Math.IEEERemainder(data[2], 86400.0); // tod (v.2), tow (v.3) in utc
                                                                                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                                                                                //ORIGINAL LINE: tof=gpst2time(week,tod+dow *86400.0);
            tof.CopyFrom(GlobalMembersRtkcmn.gpst2time(week, tod + dow * 86400.0));
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: tof=adjday(tof,toc);
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            tof.CopyFrom(GlobalMembersBinex.adjday(new gtime_t(tof), new gtime_t(toc)));

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: geph->toe=utc2gpst(toc);
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            geph.toe.CopyFrom(GlobalMembersRtkcmn.utc2gpst(new gtime_t(toc))); // toc (gpst)
                                                                               //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                                                                               //ORIGINAL LINE: geph->tof=utc2gpst(tof);
                                                                               //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            geph.tof.CopyFrom(GlobalMembersRtkcmn.utc2gpst(new gtime_t(tof))); // tof (gpst)

            /* iode = tb (7bit), tb =index of UTC+3H within current day */
            geph.iode = (int)(Math.IEEERemainder(tow + 10800.0, 86400.0) / 900.0 + 0.5);

            geph.taun = -data[0]; // -taun
            geph.gamn = data[1]; // +gamman

            geph.pos[0] = data[3] * 1E3;
            geph.pos[1] = data[7] * 1E3;
            geph.pos[2] = data[11] * 1E3;
            geph.vel[0] = data[4] * 1E3;
            geph.vel[1] = data[8] * 1E3;
            geph.vel[2] = data[12] * 1E3;
            geph.acc[0] = data[5] * 1E3;
            geph.acc[1] = data[9] * 1E3;
            geph.acc[2] = data[13] * 1E3;

            geph.svh = (int)data[6];
            geph.frq = (int)data[10];
            geph.age = (int)data[14];

            /* some receiver output >128 for minus frequency number */
            if (geph.frq > 128)
            {
                geph.frq -= 256;
            }

            if (geph.frq < DefineConstants.MINFREQ_GLO || DefineConstants.MAXFREQ_GLO < geph.frq)
            {
                GlobalMembersRtkcmn.trace(2, "rinex gnav invalid freq: sat=%2d fn=%d\n", sat, geph.frq);
            }
            return 1;
        }
        /* decode geo ephemeris ------------------------------------------------------*/
        internal static int decode_seph(double ver, int sat, gtime_t toc, double[] data, seph_t seph)
        {
            seph_t seph0 = new seph_t();
            int week;

            GlobalMembersRtkcmn.trace(4, "decode_seph: ver=%.2f sat=%2d\n", ver, sat);

            if (GlobalMembersRtkcmn.satsys(sat, null) != DefineConstants.SYS_SBS)
            {
                GlobalMembersRtkcmn.trace(2, "geo ephemeris error: invalid satellite sat=%2d\n", sat);
                return 0;
            }
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: *seph=seph0;
            seph.CopyFrom(seph0);

            seph.sat = sat;
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: seph->t0 =toc;
            seph.t0.CopyFrom(toc);

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: time2gpst(toc,&week);
            GlobalMembersRtkcmn.time2gpst(new gtime_t(toc), ref week);
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: seph->tof=adjweek(gpst2time(week,data[2]),toc);
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            seph.tof.CopyFrom(GlobalMembersBinex.adjweek(GlobalMembersRtkcmn.gpst2time(week, data[2]), new gtime_t(toc)));

            seph.af0 = data[0];
            seph.af1 = data[1];

            seph.pos[0] = data[3] * 1E3;
            seph.pos[1] = data[7] * 1E3;
            seph.pos[2] = data[11] * 1E3;
            seph.vel[0] = data[4] * 1E3;
            seph.vel[1] = data[8] * 1E3;
            seph.vel[2] = data[12] * 1E3;
            seph.acc[0] = data[5] * 1E3;
            seph.acc[1] = data[9] * 1E3;
            seph.acc[2] = data[13] * 1E3;

            seph.svh = (int)data[6];
            seph.sva = GlobalMembersBinex.uraindex(data[10]);

            return 1;
        }
        /* read rinex navigation data body -------------------------------------------*/
        internal static int readrnxnavb(FILE fp, string opt, double ver, int sys, ref int type, eph_t eph, geph_t geph, seph_t seph)
        {
            gtime_t toc = new gtime_t();
            double[] data = new double[64];
            int i = 0;
            int j;
            int prn;
            int sat = 0;
            int sp = 3;
            int mask;
            string buff = new string(new char[16 * DefineConstants.MAXOBSTYPE + 4]);
            string id = "";
            //C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on this variable, so pointers on this variable are left unchanged:
            sbyte* p;

            GlobalMembersRtkcmn.trace(4, "readrnxnavb: ver=%.2f sys=%d\n", ver, sys);

            /* set system mask */
            mask = GlobalMembersRinex.set_sysmask(opt);

            while (fgets(buff, 16 * DefineConstants.MAXOBSTYPE + 4, fp))
            {

                if (i == 0)
                {

                    /* decode satellite field */
                    if (ver >= 3.0 || sys == DefineConstants.SYS_GAL || sys == DefineConstants.SYS_QZS) // ver.3 or GAL/QZS
                    {
                        id = buff.Substring(0, 3);
                        sat = GlobalMembersRtkcmn.satid2no(id);
                        sp = 4;
                        if (ver >= 3.0)
                        {
                            sys = GlobalMembersRtkcmn.satsys(sat, null);
                        }
                    }
                    else
                    {
                        prn = (int)GlobalMembersRtkcmn.str2num(buff, 0, 2);

                        if (sys == DefineConstants.SYS_SBS)
                        {
                            sat = GlobalMembersRtkcmn.satno(DefineConstants.SYS_SBS, prn + 100);
                        }
                        else if (sys == DefineConstants.SYS_GLO)
                        {
                            sat = GlobalMembersRtkcmn.satno(DefineConstants.SYS_GLO, prn);
                        }
                        else if (93 <= prn && prn <= 97) // extension
                        {
                            sat = GlobalMembersRtkcmn.satno(DefineConstants.SYS_QZS, prn + 100);
                        }
                        else
                        {
                            sat = GlobalMembersRtkcmn.satno(DefineConstants.SYS_GPS, prn);
                        }
                    }
                    /* decode toc field */
                    if (GlobalMembersRtkcmn.str2time(buff.Substring(sp), 0, 19, toc) != 0)
                    {
                        GlobalMembersRtkcmn.trace(2, "rinex nav toc error: %23.23s\n", buff);
                        return 0;
                    }
                    /* decode data fields */
                    for (j = 0, p = buff.Substring(sp) + 19; j < 3; j++, p += 19)
                    {
                        data[i++] = GlobalMembersRtkcmn.str2num(p, 0, 19);
                    }
                }
                else
                {
                    /* decode data fields */
                    for (j = 0, p = buff.Substring(sp); j < 4; j++, p += 19)
                    {
                        data[i++] = GlobalMembersRtkcmn.str2num(p, 0, 19);
                    }
                    /* decode ephemeris */
                    if (sys == DefineConstants.SYS_GLO && i >= 15)
                    {
                        if (!(mask & sys))
                        {
                            return 0;
                        }
                        type = 1;
                        //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                        //ORIGINAL LINE: return decode_geph(ver,sat,toc,data,geph);
                        return GlobalMembersRinex.decode_geph(ver, sat, new gtime_t(toc), data, geph);
                    }
                    else if (sys == DefineConstants.SYS_SBS && i >= 15)
                    {
                        if (!(mask & sys))
                        {
                            return 0;
                        }
                        type = 2;
                        //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                        //ORIGINAL LINE: return decode_seph(ver,sat,toc,data,seph);
                        return GlobalMembersRinex.decode_seph(ver, sat, new gtime_t(toc), data, seph);
                    }
                    else if (i >= 31)
                    {
                        if (!(mask & sys))
                        {
                            return 0;
                        }
                        type = 0;
                        return GlobalMembersJavad.decode_eph(ver, sat, toc, data, eph);
                    }
                }
            }
            return -1;
        }
        /* add ephemeris to navigation data ------------------------------------------*/
        internal static int add_eph(nav_t nav, eph_t[] eph)
        {
            eph_t nav_eph;

            if (nav.nmax <= nav.n)
            {
                nav.nmax += 1024;
                //C++ TO C# CONVERTER TODO TASK: The memory management function 'realloc' has no equivalent in C#:
                if ((nav_eph = (eph_t)realloc(nav.eph, sizeof(eph_t) * nav.nmax)) == null)
                {
                    GlobalMembersRtkcmn.trace(1, "decode_eph malloc error: n=%d\n", nav.nmax);
                    //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
                    free(nav.eph);
                    nav.eph = null;
                    nav.n = nav.nmax = 0;
                    return 0;
                }
                nav.eph = nav_eph;
            }
            nav.eph[nav.n++] = *eph;
            return 1;
        }
        internal static int add_geph(nav_t nav, geph_t[] geph)
        {
            geph_t nav_geph;

            if (nav.ngmax <= nav.ng)
            {
                nav.ngmax += 1024;
                //C++ TO C# CONVERTER TODO TASK: The memory management function 'realloc' has no equivalent in C#:
                if ((nav_geph = (geph_t)realloc(nav.geph, sizeof(geph_t) * nav.ngmax)) == null)
                {
                    GlobalMembersRtkcmn.trace(1, "decode_geph malloc error: n=%d\n", nav.ngmax);
                    //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
                    free(nav.geph);
                    nav.geph = null;
                    nav.ng = nav.ngmax = 0;
                    return 0;
                }
                nav.geph = nav_geph;
            }
            nav.geph[nav.ng++] = *geph;
            return 1;
        }
        internal static int add_seph(nav_t nav, seph_t[] seph)
        {
            seph_t nav_seph;

            if (nav.nsmax <= nav.ns)
            {
                nav.nsmax += 1024;
                //C++ TO C# CONVERTER TODO TASK: The memory management function 'realloc' has no equivalent in C#:
                if ((nav_seph = (seph_t)realloc(nav.seph, sizeof(seph_t) * nav.nsmax)) == null)
                {
                    GlobalMembersRtkcmn.trace(1, "decode_seph malloc error: n=%d\n", nav.nsmax);
                    //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
                    free(nav.seph);
                    nav.seph = null;
                    nav.ns = nav.nsmax = 0;
                    return 0;
                }
                nav.seph = nav_seph;
            }
            nav.seph[nav.ns++] = *seph;
            return 1;
        }
        /* read rinex nav/gnav/geo nav -----------------------------------------------*/
        internal static int readrnxnav(FILE fp, string opt, double ver, int sys, nav_t nav)
        {
            eph_t eph = new eph_t();
            geph_t geph = new geph_t();
            seph_t seph = new seph_t();
            int stat;
            int type;

            GlobalMembersRtkcmn.trace(3, "readrnxnav: ver=%.2f sys=%d\n", ver, sys);

            if (nav == null)
            {
                return 0;
            }

            /* read rinex navigation data body */
            while ((stat = GlobalMembersRinex.readrnxnavb(fp, opt, ver, sys, ref type, eph, geph, seph)) >= 0)
            {

                /* add ephemeris to navigation data */
                if (stat != 0)
                {
                    switch (type)
                    {
                        case 1:
                            stat = GlobalMembersRinex.add_geph(nav, geph);
                            break;
                        case 2:
                            stat = GlobalMembersRinex.add_seph(nav, seph);
                            break;
                        default:
                            stat = GlobalMembersRinex.add_eph(nav, eph);
                            break;
                    }
                    if (stat == 0)
                    {
                        return 0;
                    }
                }
            }
            return nav.n > 0 || nav.ng > 0 || nav.ns > 0;
        }
        /* read rinex clock ----------------------------------------------------------*/
        internal static int readrnxclk(FILE fp, string opt, int index, nav_t nav)
        {
            pclk_t nav_pclk;
            gtime_t time = new gtime_t();
            double[] data = new double[2];
            int i;
            int j;
            int sat;
            int mask;
            string buff = new string(new char[16 * DefineConstants.MAXOBSTYPE + 4]);
            string satid = "";

            GlobalMembersRtkcmn.trace(3, "readrnxclk: index=%d\n", index);

            if (nav == null)
            {
                return 0;
            }

            /* set system mask */
            mask = GlobalMembersRinex.set_sysmask(opt);

            while (fgets(buff, sizeof(sbyte), fp))
            {

                if (GlobalMembersRtkcmn.str2time(buff, 8, 26, time) != 0)
                {
                    GlobalMembersRtkcmn.trace(2, "rinex clk invalid epoch: %34.34s\n", buff);
                    continue;
                }
                satid = buff.Substring(3, 4);

                /* only read AS (satellite clock) record */
                if (string.Compare(buff, 0, "AS", 0, 2) || (sat = GlobalMembersRtkcmn.satid2no(satid)) == 0)
                    continue;

                if (!(GlobalMembersRtkcmn.satsys(sat, null) & mask))
                    continue;

                for (i = 0, j = 40; i < 2; i++, j += 20)
                {
                    data[i] = GlobalMembersRtkcmn.str2num(buff, j, 19);
                }

                if (nav.nc >= nav.ncmax)
                {
                    nav.ncmax += 1024;
                    //C++ TO C# CONVERTER TODO TASK: The memory management function 'realloc' has no equivalent in C#:
                    if ((nav_pclk = (pclk_t)realloc(nav.pclk, sizeof(pclk_t) * (nav.ncmax))) == null)
                    {
                        GlobalMembersRtkcmn.trace(1, "readrnxclk malloc error: nmax=%d\n", nav.ncmax);
                        //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
                        free(nav.pclk);
                        nav.pclk = null;
                        nav.nc = nav.ncmax = 0;
                        return -1;
                    }
                    nav.pclk = nav_pclk;
                }
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: if (nav->nc<=0||fabs(timediff(time,nav->pclk[nav->nc-1].time))>1E-9)
                if (nav.nc <= 0 || Math.Abs(GlobalMembersRtkcmn.timediff(new gtime_t(time), nav.pclk[nav.nc - 1].time)) > 1E-9)
                {
                    nav.nc++;
                    nav.pclk[nav.nc - 1].time = time;
                    nav.pclk[nav.nc - 1].index = index;
                    for (i = 0; i < DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1; i++)
                    {
                        nav.pclk[nav.nc - 1].clk[i][0] = 0.0;
                        nav.pclk[nav.nc - 1].std[i][0] = 0.0f;
                    }
                }
                nav.pclk[nav.nc - 1].clk[sat - 1][0] = data[0];
                nav.pclk[nav.nc - 1].std[sat - 1][0] = (float)data[1];
            }
            return nav.nc > 0;
        }
        /* read rinex file -----------------------------------------------------------*/
        internal static int readrnxfp(FILE fp, gtime_t ts, gtime_t te, double tint, string opt, int flag, int index, ref string type, obs_t obs, nav_t nav, sta_t sta)
        {
            double ver;
            int sys;
            int tsys;
            sbyte[,,] tobs = { { "" } };

            GlobalMembersRtkcmn.trace(3, "readrnxfp: flag=%d index=%d\n", flag, index);

            /* read rinex header */
            if (GlobalMembersRinex.readrnxh(fp, ref ver, ref type, ref sys, ref tsys, tobs, nav, sta) == 0)
            {
                return 0;
            }

            /* flag=0:except for clock,1:clock */
            if ((flag == 0 && type == 'C') || (flag != 0 && type != 'C'))
            {
                return 0;
            }

            /* read rinex body */
            switch (type)
            {
                case 'O':
                    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                    //ORIGINAL LINE: return readrnxobs(fp,ts,te,tint,opt,index,ver,tsys,tobs,obs);
                    return GlobalMembersRinex.readrnxobs(fp, new gtime_t(ts), new gtime_t(te), tint, opt, index, ver, tsys, tobs, obs);
                case 'N':
                    return GlobalMembersRinex.readrnxnav(fp, opt, ver, sys, nav);
                case 'G':
                    return GlobalMembersRinex.readrnxnav(fp, opt, ver, DefineConstants.SYS_GLO, nav);
                case 'H':
                    return GlobalMembersRinex.readrnxnav(fp, opt, ver, DefineConstants.SYS_SBS, nav);
                case 'J': // extension
                    return GlobalMembersRinex.readrnxnav(fp, opt, ver, DefineConstants.SYS_QZS, nav);
                case 'L': // extension
                    return GlobalMembersRinex.readrnxnav(fp, opt, ver, DefineConstants.SYS_GAL, nav);
                case 'C':
                    return GlobalMembersRinex.readrnxclk(fp, opt, index, nav);
            }
            GlobalMembersRtkcmn.trace(2, "unsupported rinex type ver=%.2f type=%c\n", ver, type);
            return 0;
        }
        /* uncompress and read rinex file --------------------------------------------*/
        internal static int readrnxfile(string file, gtime_t ts, gtime_t te, double tint, string opt, int flag, int index, ref string type, obs_t obs, nav_t nav, sta_t sta)
        {
            FILE fp;
            int cstat;
            int stat;
            string tmpfile = new string(new char[1024]);

            GlobalMembersRtkcmn.trace(3, "readrnxfile: file=%s flag=%d index=%d\n", file, flag, index);

            if (sta != null)
            {
                GlobalMembersRinex.init_sta(sta);
            }

            /* uncompress file */
            if ((cstat = GlobalMembersRtkcmn.uncompress(file, ref tmpfile)) < 0)
            {
                GlobalMembersRtkcmn.trace(2, "rinex file uncompact error: %s\n", file);
                return 0;
            }
            if ((fp = fopen(cstat != 0 ? tmpfile : file, "r")) == null)
            {
                GlobalMembersRtkcmn.trace(2, "rinex file open error: %s\n", cstat != 0 ? tmpfile : file);
                return 0;
            }
            /* read rinex file */
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: stat=readrnxfp(fp,ts,te,tint,opt,flag,index,type,obs,nav,sta);
            stat = GlobalMembersRinex.readrnxfp(fp, new gtime_t(ts), new gtime_t(te), tint, opt, flag, index, ref type, obs, nav, sta);

            fclose(fp);

            /* delete temporary file */
            if (cstat != 0)
            {
                remove(tmpfile);
            }

            return stat;
        }
        /* read rinex obs and nav files ------------------------------------------------
        * read rinex obs and nav files
        * args   : char *file    I      file (wild-card * expanded) ("": stdin)
        *          int   rcv     I      receiver number for obs data
        *         (gtime_t ts)   I      observation time start (ts.time==0: no limit)
        *         (gtime_t te)   I      observation time end   (te.time==0: no limit)
        *         (double tint)  I      observation time interval (s) (0:all)
        *          char  *opt    I      rinex options (see below,"": no option)
        *          obs_t *obs    IO     observation data   (NULL: no input)
        *          nav_t *nav    IO     navigation data    (NULL: no input)
        *          sta_t *sta    IO     station parameters (NULL: no input)
        * return : status (1:ok,0:no data,-1:error)
        * notes  : read data are appended to obs and nav struct
        *          before calling the function, obs and nav should be initialized.
        *          observation data and navigation data are not sorted.
        *          navigation data may be duplicated.
        *          call sortobs() or uniqnav() to sort data or delete duplicated eph.
        *
        *          read rinex options (separated by spaces) :
        *
        *            -GLss[=shift]: select GPS signal ss (ss: RINEX 3 code, "1C","2W"...)
        *            -RLss[=shift]: select GLO signal ss
        *            -ELss[=shift]: select GAL signal ss
        *            -JLss[=shift]: select QZS signal ss
        *            -CLss[=shift]: select BDS signal ss
        *            -SLss[=shift]: select SBS signal ss
        *
        *                 shift: carrier phase shift to be added (cycle)
        *            
        *            -SYS=sys[,sys...]: select navi systems
        *                               (sys=G:GPS,R:GLO,E:GAL,J:QZS,C:BDS,S:SBS)
        *
        *-----------------------------------------------------------------------------*/
        public static int readrnxt(string file, int rcv, gtime_t ts, gtime_t te, double tint, string opt, obs_t obs, nav_t nav, sta_t sta)
        {
            int i;
            int n;
            int stat = 0;
            string p;
            sbyte type = (sbyte)' ';
            string[] files = { 0 };

            GlobalMembersRtkcmn.trace(3, "readrnxt: file=%s rcv=%d\n", file, rcv);

            if (file == 0)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: return readrnxfp(stdin,ts,te,tint,opt,0,1,&type,obs,nav,sta);
                return GlobalMembersRinex.readrnxfp(stdin, new gtime_t(ts), new gtime_t(te), tint, opt, 0, 1, ref type, obs, nav, sta);
            }
            for (i = 0; i < DefineConstants.MAXEXFILE; i++)
            {
                //C++ TO C# CONVERTER TODO TASK: The memory management function 'malloc' has no equivalent in C#:
                if (!(files[i] = (string)malloc(1024)))
                {
                    for (i--; i >= 0; i--)
                    {
                        //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
                        free(files[i]);
                    }
                    return -1;
                }
            }
            /* expand wild-card */
            if ((n = GlobalMembersRtkcmn.expath(file, files, DefineConstants.MAXEXFILE)) <= 0)
            {
                for (i = 0; i < DefineConstants.MAXEXFILE; i++)
                {
                    //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
                    free(files[i]);
                }
                return 0;
            }
            /* read rinex files */
            for (i = 0; i < n && stat >= 0; i++)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: stat=readrnxfile(files[i],ts,te,tint,opt,0,rcv,&type,obs,nav,sta);
                stat = GlobalMembersRinex.readrnxfile(files[i], new gtime_t(ts), new gtime_t(te), tint, opt, 0, rcv, ref type, obs, nav, sta);
            }
            /* if station name empty, set 4-char name from file head */
            if (type == 'O' && sta != null)
            {
                if ((p = StringFunctions.StrRChr(file, DefineConstants.FILEPATHSEP)) == 0)
                {
                    p = file - 1;
                }
                if (!*sta.name)
                {
                    GlobalMembersRinex.setstr(ref sta.name, p.Substring(1), 4);
                }
            }
            for (i = 0; i < DefineConstants.MAXEXFILE; i++)
            {
                //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
                free(files[i]);
            }

            return stat;
        }
        public static int readrnx(string file, int rcv, string opt, obs_t obs, nav_t nav, sta_t sta)
        {
            gtime_t t = new gtime_t();

            GlobalMembersRtkcmn.trace(3, "readrnx : file=%s rcv=%d\n", file, rcv);

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: return readrnxt(file,rcv,t,t,0.0,opt,obs,nav,sta);
            return GlobalMembersRinex.readrnxt(file, rcv, new gtime_t(t), new gtime_t(t), 0.0, opt, obs, nav, sta);
        }
        /* compare precise clock -----------------------------------------------------*/
        internal static int cmppclk(object p1, object p2)
        {
            pclk_t q1 = (pclk_t)p1;
            pclk_t q2 = (pclk_t)p2;
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: double tt=timediff(q1->time,q2->time);
            double tt = GlobalMembersRtkcmn.timediff(new gtime_t(q1.time), new gtime_t(q2.time));
            return tt < -1E-9 ? -1 : (tt > 1E-9 ? 1 : q1.index - q2.index);
        }
        /* combine precise clock -----------------------------------------------------*/
        internal static void combpclk(nav_t nav)
        {
            pclk_t nav_pclk;
            int i;
            int j;
            int k;

            GlobalMembersRtkcmn.trace(3, "combpclk: nc=%d\n", nav.nc);

            if (nav.nc <= 0)
                return;

            qsort(nav.pclk, nav.nc, sizeof(pclk_t), GlobalMembersRinex.cmppclk);

            for (i = 0, j = 1; j < nav.nc; j++)
            {
                if (Math.Abs(GlobalMembersRtkcmn.timediff(nav.pclk[i].time, nav.pclk[j].time)) < 1E-9)
                {
                    for (k = 0; k < DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1; k++)
                    {
                        if (nav.pclk[j].clk[k][0] == 0.0)
                            continue;
                        nav.pclk[i].clk[k][0] = nav.pclk[j].clk[k][0];
                        nav.pclk[i].std[k][0] = nav.pclk[j].std[k][0];
                    }
                }
                else if (++i < j)
                {
                    nav.pclk[i] = nav.pclk[j];
                }
            }
            nav.nc = i + 1;

            //C++ TO C# CONVERTER TODO TASK: The memory management function 'realloc' has no equivalent in C#:
            if ((nav_pclk = (pclk_t)realloc(nav.pclk, sizeof(pclk_t) * nav.nc)) == null)
            {
                //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
                free(nav.pclk);
                nav.pclk = null;
                nav.nc = nav.ncmax = 0;
                GlobalMembersRtkcmn.trace(1, "combpclk malloc error nc=%d\n", nav.nc);
                return;
            }
            nav.pclk = nav_pclk;
            nav.ncmax = nav.nc;

            GlobalMembersRtkcmn.trace(4, "combpclk: nc=%d\n", nav.nc);
        }
        /* read rinex clock files ------------------------------------------------------
        * read rinex clock files
        * args   : char *file    I      file (wild-card * expanded)
        *          nav_t *nav    IO     navigation data    (NULL: no input)
        * return : number of precise clock
        *-----------------------------------------------------------------------------*/
        public static int readrnxc(string file, nav_t nav)
        {
            gtime_t t = new gtime_t();
            int i;
            int n;
            int index = 0;
            int stat = 1;
            string[] files = { 0 };
            sbyte type;

            GlobalMembersRtkcmn.trace(3, "readrnxc: file=%s\n", file);

            for (i = 0; i < DefineConstants.MAXEXFILE; i++)
            {
                //C++ TO C# CONVERTER TODO TASK: The memory management function 'malloc' has no equivalent in C#:
                if (!(files[i] = (string)malloc(1024)))
                {
                    for (i--; i >= 0; i--)
                    {
                        //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
                        free(files[i]);
                    }
                    return 0;
                }
            }
            /* expand wild-card */
            n = GlobalMembersRtkcmn.expath(file, files, DefineConstants.MAXEXFILE);

            /* read rinex clock files */
            for (i = 0; i < n; i++)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: if (readrnxfile(files[i],t,t,0.0,"",1,index++,&type,null,nav,null))
                if (GlobalMembersRinex.readrnxfile(files[i], new gtime_t(t), new gtime_t(t), 0.0, "", 1, index++, ref type, null, nav, null) != 0)
                {
                    continue;
                }
                stat = 0;
                break;
            }
            for (i = 0; i < DefineConstants.MAXEXFILE; i++)
            {
                //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
                free(files[i]);
            }

            if (stat == 0)
            {
                return 0;
            }

            /* unique and combine ephemeris and precise clock */
            GlobalMembersRinex.combpclk(nav);

            return nav.nc;
        }
        /* initialize rinex control ----------------------------------------------------
        * initialize rinex control struct and reallocate memory for observation and
        * ephemeris buffer in rinex control struct
        * args   : rnxctr_t *rnx IO     rinex control struct
        * return : status (1:ok,0:memory allocation error)
        *-----------------------------------------------------------------------------*/
        public static int init_rnxctr(rnxctr_t rnx)
        {
            gtime_t time0 = new gtime_t();
            obsd_t data0 = new obsd_t({ 0 });
            eph_t eph0 = new eph_t(0, -1, -1);
            geph_t geph0 = new geph_t(0, -1);
            seph_t seph0 = new seph_t();
            int i;
            int j;

            GlobalMembersRtkcmn.trace(3, "init_rnxctr:\n");

            rnx.obs.data = null;
            rnx.nav.eph = null;
            rnx.nav.geph = null;
            rnx.nav.seph = null;

            //C++ TO C# CONVERTER TODO TASK: The memory management function 'malloc' has no equivalent in C#:
            if ((rnx.obs.data = (obsd_t)malloc(sizeof(obsd_t) * DefineConstants.MAXOBS)) == null || (rnx.nav.eph = (eph_t)malloc(sizeof(eph_t) * DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1)) == null || (rnx.nav.geph = (geph_t)malloc(sizeof(geph_t) * DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1)) == null || (rnx.nav.seph = (seph_t)malloc(sizeof(seph_t) * DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1)) == null)
            {
                GlobalMembersRinex.free_rnxctr(rnx);
                return 0;
            }
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: rnx->time=time0;
            rnx.time.CopyFrom(time0);
            rnx.ver = 0.0;
            rnx.sys = rnx.tsys = 0;
            for (i = 0; i < 6; i++)
            {
                for (j = 0; j < DefineConstants.MAXOBSTYPE; j++)
                {
                    rnx.tobs[i, j, 0] = (sbyte)'\0';
                }
            }
            rnx.obs.n = 0;
            rnx.nav.n = DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1;
            rnx.nav.ng = DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1;
            rnx.nav.ns = DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1;
            for (i = 0; i < DefineConstants.MAXOBS; i++)
            {
                rnx.obs.data[i] = data0;
            }
            for (i = 0; i < DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1; i++)
            {
                rnx.nav.eph[i] = eph0;
            }
            for (i = 0; i < DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1; i++)
            {
                rnx.nav.geph[i] = geph0;
            }
            for (i = 0; i < DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1; i++)
            {
                rnx.nav.seph[i] = seph0;
            }
            rnx.ephsat = 0;
            rnx.opt[0] = '\0';

            return 1;
        }
        /* free rinex control ----------------------------------------------------------
        * free observation and ephemris buffer in rinex control struct
        * args   : rnxctr_t *rnx IO  rinex control struct
        * return : none
        *-----------------------------------------------------------------------------*/
        public static void free_rnxctr(rnxctr_t rnx)
        {
            GlobalMembersRtkcmn.trace(3, "free_rnxctr:\n");

            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(rnx.obs.data);
            rnx.obs.data = null;
            rnx.obs.n = 0;
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(rnx.nav.eph);
            rnx.nav.eph = null;
            rnx.nav.n = 0;
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(rnx.nav.geph);
            rnx.nav.geph = null;
            rnx.nav.ng = 0;
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(rnx.nav.seph);
            rnx.nav.seph = null;
            rnx.nav.ns = 0;
        }
        /* open rinex data -------------------------------------------------------------
        * fetch next rinex message and input a messsage from file
        * args   : rnxctr_t *rnx IO  rinex control struct
        *          FILE  *fp    I    file pointer
        * return : status (-2: end of file, 0: no message, 1: input observation data,
        *                   2: input navigation data)
        *-----------------------------------------------------------------------------*/
        public static int open_rnxctr(rnxctr_t rnx, FILE fp)
        {
            string rnxtypes = "ONGLJHC";
            double ver;
            sbyte type;
            sbyte[,,] tobs = { { "" } };
            int i;
            int j;
            int sys;
            int tsys;

            GlobalMembersRtkcmn.trace(3, "open_rnxctr:\n");

            /* read rinex header from file */
            if (GlobalMembersRinex.readrnxh(fp, ref ver, ref type, ref sys, ref tsys, tobs, rnx.nav, rnx.sta) == 0)
            {
                GlobalMembersRtkcmn.trace(2, "open_rnxctr: rinex header read error\n");
                return 0;
            }
            if (!StringFunctions.StrChr(rnxtypes, type))
            {
                GlobalMembersRtkcmn.trace(2, "open_rnxctr: not supported rinex type type=%c\n", type);
                return 0;
            }
            rnx.ver = ver;
            rnx.type = type;
            rnx.sys = sys;
            rnx.tsys = tsys;
            for (i = 0; i < 6; i++)
            {
                for (j = 0; j < DefineConstants.MAXOBSTYPE && tobs[i, j] != 0; j++)
                {
                    rnx.tobs[i, j] = tobs[i, j];
                }
            }
            rnx.ephsat = 0;
            return 1;
        }
        /* input rinex control ---------------------------------------------------------
        * fetch next rinex message and input a messsage from file
        * args   : rnxctr_t *rnx IO  rinex control struct
        *          FILE  *fp    I    file pointer
        * return : status (-2: end of file, 0: no message, 1: input observation data,
        *                   2: input navigation data)
        *-----------------------------------------------------------------------------*/
        public static int input_rnxctr(rnxctr_t rnx, FILE fp)
        {
            eph_t eph = new eph_t();
            geph_t geph = new geph_t();
            seph_t seph = new seph_t();
            int n;
            int sys;
            int stat;
            int flag;
            int prn;
            int type;

            GlobalMembersRtkcmn.trace(4, "input_rnxctr:\n");

            /* read rinex obs data */
            if (rnx.type == 'O')
            {
                if ((n = GlobalMembersRinex.readrnxobsb(fp, rnx.opt, rnx.ver, rnx.tobs, ref flag, rnx.obs.data)) <= 0)
                {
                    rnx.obs.n = 0;
                    return n < 0 ? -2 : 0;
                }
                rnx.time = rnx.obs.data[0].time;
                rnx.obs.n = n;
                return 1;
            }
            /* read rinex nav data */
            switch (rnx.type)
            {
                case 'N':
                    sys = DefineConstants.SYS_NONE;
                    break;
                case 'G':
                    sys = DefineConstants.SYS_GLO;
                    break;
                case 'H':
                    sys = DefineConstants.SYS_SBS;
                    break;
                case 'L': // extension
                    sys = DefineConstants.SYS_GAL;
                    break;
                case 'J': // extension
                    sys = DefineConstants.SYS_QZS;
                    break;
                default:
                    return 0;
            }
            if ((stat = GlobalMembersRinex.readrnxnavb(fp, rnx.opt, rnx.ver, sys, ref type, eph, geph, seph)) <= 0)
            {
                return stat < 0 ? -2 : 0;
            }
            if (type == 1)
            {
                sys = GlobalMembersRtkcmn.satsys(geph.sat, ref prn);
                rnx.nav.geph[prn - 1] = geph;
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                //ORIGINAL LINE: rnx->time=geph.tof;
                rnx.time.CopyFrom(geph.tof);
                rnx.ephsat = geph.sat;
            }
            else if (type == 2)
            {
                sys = GlobalMembersRtkcmn.satsys(seph.sat, ref prn);
                rnx.nav.seph[prn - DefineConstants.MINPRNSBS] = seph;
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                //ORIGINAL LINE: rnx->time=seph.tof;
                rnx.time.CopyFrom(seph.tof);
                rnx.ephsat = seph.sat;
            }
            else
            {
                rnx.nav.eph[eph.sat - 1] = eph;
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                //ORIGINAL LINE: rnx->time=eph.ttr;
                rnx.time.CopyFrom(eph.ttr);
                rnx.ephsat = eph.sat;
            }
            return 2;
        }
        /*------------------------------------------------------------------------------
        * output rinex functions
        *-----------------------------------------------------------------------------*/

        /* output obs types ver.2 ----------------------------------------------------*/
        internal static void outobstype_ver2(FILE fp, rnxopt_t opt)
        {
            const string label = "# / TYPES OF OBSERV";
            int i;

            GlobalMembersRtkcmn.trace(3, "outobstype_ver2:\n");

            fprintf(fp, "%6d", opt.nobs[0]);

            for (i = 0; i < opt.nobs[0]; i++)
            {
                if (i > 0 && i % 9 == 0)
                {
                    fprintf(fp, "      ");
                }

                fprintf(fp, "%6s", opt.tobs[0, i]);

                if (i % 9 == 8)
                {
                    fprintf(fp, "%-20s\n", label);
                }
            }
            if (opt.nobs[0] == 0 || i % 9 > 0)
            {
                fprintf(fp, "%*s%-20s\n", (9 - i % 9) * 6, "", label);
            }
        }
        /* output obs types ver.3 ----------------------------------------------------*/
        internal static void outobstype_ver3(FILE fp, rnxopt_t opt)
        {
            const string label = "SYS / # / OBS TYPES";
            int i;
            int j;

            GlobalMembersRtkcmn.trace(3, "outobstype_ver3:\n");

            for (i = 0; navsys[i] != 0; i++)
            {
                if (!(navsys[i] & opt.navsys) || opt.nobs[i] == 0)
                    continue;

                fprintf(fp, "%c  %3d", syscodes[i], opt.nobs[i]);

                for (j = 0; j < opt.nobs[i]; j++)
                {
                    if (j > 0 && j % 13 == 0)
                    {
                        fprintf(fp, "      ");
                    }

                    fprintf(fp, " %3s", opt.tobs[i, j]);

                    if (j % 13 == 12)
                    {
                        fprintf(fp, "  %-20s\n", label);
                    }
                }
                if (j % 13 > 0)
                {
                    fprintf(fp, "%*s  %-20s\n", (13 - j % 13) * 4, "", label);
                }
            }
        }
        /* output rinex obs header -----------------------------------------------------
        * output rinex obd file header
        * args   : FILE   *fp       I   output file pointer
        *          rnxopt_t *opt    I   rinex options
        *          nav_t  *nav      I   navigation data
        * return : status (1:ok, 0:output error)
        *-----------------------------------------------------------------------------*/
        public static int outrnxobsh(FILE fp, rnxopt_t opt, nav_t nav)
        {
            string[] glo_codes = { "C1C", "C1P", "C2C", "C2P" };
            double[] ep = new double[6];
            double[] pos = { 0, null, null };
            double[] del = { 0, null, null };
            int i;
            int j;
            int k;
            int n;
            int[] prn = new int[DefineConstants.MAXPRNGLO];
            string date = new string(new char[32]);
            string sys;
            string tsys = "GPS";

            GlobalMembersRtkcmn.trace(3, "outrnxobsh:\n");

            GlobalMembersRinex.timestr_rnx(ref date);

            if (opt.rnxver <= 2.99) // ver.2
            {
                sys = opt.navsys == DefineConstants.SYS_GPS ? "G (GPS)" : "M (MIXED)";
            }
            else // ver.3
            {
                if (opt.navsys == DefineConstants.SYS_GPS)
                {
                    sys = "G: GPS";
                }
                else if (opt.navsys == DefineConstants.SYS_GLO)
                {
                    sys = "R: GLONASS";
                }
                else if (opt.navsys == DefineConstants.SYS_GAL)
                {
                    sys = "E: Galielo";
                }
                else if (opt.navsys == DefineConstants.SYS_QZS) // ver.3.02
                {
                    sys = "J: QZSS";
                }
                else if (opt.navsys == DefineConstants.SYS_CMP) // ver.3.02
                {
                    sys = "C: BeiDou";
                }
                else if (opt.navsys == DefineConstants.SYS_SBS)
                {
                    sys = "S: SBAS Payload";
                }
                else
                {
                    sys = "M: Mixed";
                }
            }
            fprintf(fp, "%9.2f%-11s%-20s%-20s%-20s\n", opt.rnxver, "", "OBSERVATION DATA", sys, "RINEX VERSION / TYPE");
            fprintf(fp, "%-20.20s%-20.20s%-20.20s%-20s\n", opt.prog, opt.runby, date, "PGM / RUN BY / DATE");

            for (i = 0; i < DefineConstants.MAXCOMMENT; i++)
            {
                if (opt.comment[i] == 0)
                    continue;
                fprintf(fp, "%-60.60s%-20s\n", opt.comment[i], "COMMENT");
            }
            fprintf(fp, "%-60.60s%-20s\n", opt.marker, "MARKER NAME");
            fprintf(fp, "%-20.20s%-40.40s%-20s\n", opt.markerno, "", "MARKER NUMBER");

            if (opt.rnxver > 2.99)
            {
                fprintf(fp, "%-20.20s%-40.40s%-20s\n", opt.markertype, "", "MARKER TYPE");
            }
            fprintf(fp, "%-20.20s%-40.40s%-20s\n", opt.name[0], opt.name[1], "OBSERVER / AGENCY");
            fprintf(fp, "%-20.20s%-20.20s%-20.20s%-20s\n", opt.rec[0], opt.rec[1], opt.rec[2], "REC # / TYPE / VERS");
            fprintf(fp, "%-20.20s%-20.20s%-20.20s%-20s\n", opt.ant[0], opt.ant[1], opt.ant[2], "ANT # / TYPE");

            for (i = 0; i < 3; i++)
            {
                if (Math.Abs(opt.apppos[i]) < 1E8)
                {
                    pos[i] = opt.apppos[i];
                }
            }
            for (i = 0; i < 3; i++)
            {
                if (Math.Abs(opt.antdel[i]) < 1E8)
                {
                    del[i] = opt.antdel[i];
                }
            }
            fprintf(fp, "%14.4f%14.4f%14.4f%-18s%-20s\n", pos[0], pos[1], pos[2], "", "APPROX POSITION XYZ");
            fprintf(fp, "%14.4f%14.4f%14.4f%-18s%-20s\n", del[0], del[1], del[2], "", "ANTENNA: DELTA H/E/N");

            if (opt.rnxver <= 2.99) // ver.2
            {
                fprintf(fp, "%6d%6d%-48s%-20s\n", 1, 1, "", "WAVELENGTH FACT L1/2");
                GlobalMembersRinex.outobstype_ver2(fp, opt);
            }
            else // ver.3
            {
                GlobalMembersRinex.outobstype_ver3(fp, opt);
            }
            if (opt.tint > 0.0)
            {
                fprintf(fp, "%10.3f%50s%-20s\n", opt.tint, "", "INTERVAL");
            }
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: time2epoch(opt->tstart,ep);
            GlobalMembersRtkcmn.time2epoch(new gtime_t(opt.tstart), ep);
            fprintf(fp, "  %04.0f%6.0f%6.0f%6.0f%6.0f%13.7f     %-12s%-20s\n", ep[0], ep[1], ep[2], ep[3], ep[4], ep[5], tsys, "TIME OF FIRST OBS");

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: time2epoch(opt->tend,ep);
            GlobalMembersRtkcmn.time2epoch(new gtime_t(opt.tend), ep);
            fprintf(fp, "  %04.0f%6.0f%6.0f%6.0f%6.0f%13.7f     %-12s%-20s\n", ep[0], ep[1], ep[2], ep[3], ep[4], ep[5], tsys, "TIME OF LAST OBS");

            if (opt.rnxver >= 3.01) // ver.3.01
            {
                for (i = 0; navsys[i] != 0; i++)
                {
                    if (!(navsys[i] & opt.navsys) || opt.nobs[i] == 0)
                        continue;
                    fprintf(fp, "%c %-58s%-20s\n", syscodes[i], "", "SYS / PHASE SHIFT");
                }
            }
            if (opt.rnxver >= 3.02) // ver.3.02
            {
                for (i = n = 0; i < DefineConstants.MAXPRNGLO; i++)
                {
                    if (nav.glo_fcn[i] >= 1)
                    {
                        prn[n++] = i + 1;
                    }
                }
                for (i = j = 0; i < (n <= 0 ? 1 : (n - 1) / 8 + 1); i++)
                {
                    if (i == 0)
                    {
                        fprintf(fp, "%3d", n);
                    }
                    else
                    {
                        fprintf(fp, "   ");
                    }
                    for (k = 0; k < 8; k++, j++)
                    {
                        if (j < n)
                        {
                            fprintf(fp, " R%02d %2d", prn[j], nav.glo_fcn[prn[j] - 1] - 8);
                        }
                        else
                        {
                            fprintf(fp, " %6s", "");
                        }
                    }
                    fprintf(fp, " %-20s\n", "GLONASS SLOT / FRQ #");
                }
            }
            if (opt.rnxver >= 3.02) // ver.3.02
            {
                for (i = 0; i < 4; i++)
                {
                    fprintf(fp, " %3s %8.3f", glo_codes[i], 0.0);
                }
                fprintf(fp, "%8s%-20s\n", "", "GLONASS COD/PHS/BIS");
            }
            return fprintf(fp, "%-60.60s%-20s\n", "", "END OF HEADER") != EOF;
        }
        /* output obs data field -----------------------------------------------------*/
        internal static void outrnxobsf(FILE fp, double obs, int lli)
        {
            if (obs == 0.0 || obs <= -1E9 || obs >= 1E9)
            {
                fprintf(fp, "              ");
            }
            else
            {
                fprintf(fp, "%14.3f", obs);
            }
            if (lli <= 0)
            {
                fprintf(fp, "  ");
            }
            else
            {
                fprintf(fp, "%1.1d ", lli);
            }
        }
        /* search obs data index -----------------------------------------------------*/
        internal static int obsindex(double ver, int sys, byte[] code, string tobs, string mask)
        {
            string id;
            int i;

            for (i = 0; i < DefineConstants.NFREQ + DefineConstants.NEXOBS; i++)
            {

                /* signal mask */
                if (mask[code[i] - 1] == '0')
                    continue;

                if (ver <= 2.99) // ver.2
                {
                    if (!string.Compare(tobs, "C1") && (sys == DefineConstants.SYS_GPS || sys == DefineConstants.SYS_GLO || sys == DefineConstants.SYS_QZS || sys == DefineConstants.SYS_SBS || sys == DefineConstants.SYS_CMP))
                    {
                        if (code[i] == DefineConstants.CODE_L1C)
                        {
                            return i;
                        }
                    }
                    else if (!string.Compare(tobs, "P1"))
                    {
                        if (code[i] == DefineConstants.CODE_L1P || code[i] == DefineConstants.CODE_L1W || code[i] == DefineConstants.CODE_L1Y || code[i] == DefineConstants.CODE_L1N)
                        {
                            return i;
                        }
                    }
                    else if (!string.Compare(tobs, "C2") && (sys == DefineConstants.SYS_GPS || sys == DefineConstants.SYS_QZS))
                    {
                        if (code[i] == DefineConstants.CODE_L2S || code[i] == DefineConstants.CODE_L2L || code[i] == DefineConstants.CODE_L2X)
                        {
                            return i;
                        }
                    }
                    else if (!string.Compare(tobs, "C2") && sys == DefineConstants.SYS_GLO)
                    {
                        if (code[i] == DefineConstants.CODE_L2C)
                        {
                            return i;
                        }
                    }
                    else if (!string.Compare(tobs, "P2"))
                    {
                        if (code[i] == DefineConstants.CODE_L2P || code[i] == DefineConstants.CODE_L2W || code[i] == DefineConstants.CODE_L2Y || code[i] == DefineConstants.CODE_L2N || code[i] == DefineConstants.CODE_L2D)
                        {
                            return i;
                        }
                    }
                    else if (ver >= 2.12 && tobs[1] == 'A') // L1C/A
                    {
                        if (code[i] == DefineConstants.CODE_L1C)
                        {
                            return i;
                        }
                    }
                    else if (ver >= 2.12 && tobs[1] == 'B') // L1C
                    {
                        if (code[i] == DefineConstants.CODE_L1S || code[i] == DefineConstants.CODE_L1L || code[i] == DefineConstants.CODE_L1X)
                        {
                            return i;
                        }
                    }
                    else if (ver >= 2.12 && tobs[1] == 'C') // L2C
                    {
                        if (code[i] == DefineConstants.CODE_L2S || code[i] == DefineConstants.CODE_L2L || code[i] == DefineConstants.CODE_L2X)
                        {
                            return i;
                        }
                    }
                    else if (ver >= 2.12 && tobs[1] == 'D' && sys == DefineConstants.SYS_GLO) // GLO L2C/A
                    {
                        if (code[i] == DefineConstants.CODE_L2C)
                        {
                            return i;
                        }
                    }
                    else if (tobs[1] == '2' && sys == DefineConstants.SYS_CMP) // BDS B1
                    {
                        if (code[i] == DefineConstants.CODE_L1I || code[i] == DefineConstants.CODE_L1Q || code[i] == DefineConstants.CODE_L1X)
                        {
                            return i;
                        }
                    }
                    else
                    {
                        id = GlobalMembersRtkcmn.code2obs(code[i], null);
                        if (id[0] == tobs[1])
                        {
                            return i;
                        }
                    }
                }
                else // ver.3
                {
                    id = GlobalMembersRtkcmn.code2obs(code[i], null);
                    if (!string.Compare(id, tobs.Substring(1)))
                    {
                        return i;
                    }
                }
            }
            return -1;
        }
        /* output rinex obs body -------------------------------------------------------
        * output rinex obs body
        * args   : FILE   *fp       I   output file pointer
        *          rnxopt_t *opt    I   rinex options
        *          obsd_t *obs      I   observation data
        *          int    n         I   number of observation data
        *          int    flag      I   epoch flag (0:ok,1:power failure,>1:event flag)
        * return : status (1:ok, 0:output error)
        *-----------------------------------------------------------------------------*/
        public static int outrnxobsb(FILE fp, rnxopt_t opt, obsd_t[] obs, int n, int flag)
        {
            string mask;
            double[] ep = new double[6];
            string[] sats = { "" };
            int i;
            int j;
            int k;
            int m;
            int ns;
            int sys;
            int[] ind = new int[DefineConstants.MAXOBS];
            int[] s = new int[DefineConstants.MAXOBS];

            GlobalMembersRtkcmn.trace(3, "outrnxobsb: n=%d\n", n);

            GlobalMembersRtkcmn.time2epoch(obs[0].time, ep);

            for (i = ns = 0; i < n && ns < DefineConstants.MAXOBS; i++)
            {
                sys = GlobalMembersRtkcmn.satsys(obs[i].sat, null);
                if (!(sys & opt.navsys) || opt.exsats[obs[i].sat - 1] != 0)
                    continue;
                if (GlobalMembersRinex.sat2code(obs[i].sat, ref sats[ns]) == 0)
                    continue;
                switch (sys)
                {
                    case DefineConstants.SYS_GPS:
                        s[ns] = 0;
                        break;
                    case DefineConstants.SYS_GLO:
                        s[ns] = 1;
                        break;
                    case DefineConstants.SYS_GAL:
                        s[ns] = 2;
                        break;
                    case DefineConstants.SYS_QZS:
                        s[ns] = 3;
                        break;
                    case DefineConstants.SYS_SBS:
                        s[ns] = 4;
                        break;
                    case DefineConstants.SYS_CMP:
                        s[ns] = 5;
                        break;
                }
                if (opt.nobs[opt.rnxver <= 2.99 ? 0 : s[ns]] == 0)
                    continue;
                ind[ns++] = i;
            }
            if (opt.rnxver <= 2.99) // ver.2
            {
                fprintf(fp, " %02d %2.0f %2.0f %2.0f %2.0f%11.7f  %d%3d", (int)ep[0] % 100, ep[1], ep[2], ep[3], ep[4], ep[5], flag, ns);
                for (i = 0; i < ns; i++)
                {
                    if (i > 0 && i % 12 == 0)
                    {
                        fprintf(fp, "\n%32s", "");
                    }
                    fprintf(fp, "%-3s", sats[i]);
                }
            }
            else // ver.3
            {
                fprintf(fp, "> %04.0f %2.0f %2.0f %2.0f %2.0f%11.7f  %d%3d%21s\n", ep[0], ep[1], ep[2], ep[3], ep[4], ep[5], flag, ns, "");
            }
            for (i = 0; i < ns; i++)
            {
                sys = GlobalMembersRtkcmn.satsys(obs[ind[i]].sat, null);

                if (opt.rnxver <= 2.99) // ver.2
                {
                    m = 0;
                    mask = opt.mask[s[i]];
                }
                else // ver.3
                {
                    fprintf(fp, "%-3s", sats[i]);
                    m = s[i];
                    mask = opt.mask[s[i]];
                }
                for (j = 0; j < opt.nobs[m]; j++)
                {

                    if (opt.rnxver <= 2.99) // ver.2
                    {
                        if (j % 5 == 0)
                        {
                            fprintf(fp, "\n");
                        }
                    }
                    /* search obs data index */
                    if ((k = GlobalMembersNovatel.obsindex(opt.rnxver, sys, obs[ind[i]].code, opt.tobs[m, j], mask)) < 0)
                    {
                        GlobalMembersRinex.outrnxobsf(fp, 0.0, -1);
                        continue;
                    }
                    /* output field */
                    switch (opt.tobs[m, j, 0])
                    {
                        case 'C':
                        case 'P':
                            GlobalMembersRinex.outrnxobsf(fp, obs[ind[i]].P[k], -1);
                            break;
                        case 'L':
                            GlobalMembersRinex.outrnxobsf(fp, obs[ind[i]].L[k], obs[ind[i]].LLI[k]);
                            break;
                        case 'D':
                            GlobalMembersRinex.outrnxobsf(fp, obs[ind[i]].D[k], -1);
                            break;
                        case 'S':
                            GlobalMembersRinex.outrnxobsf(fp, obs[ind[i]].SNR[k] * 0.25, -1);
                            break;
                    }
                }
                if (opt.rnxver > 2.99 && fprintf(fp, "\n") == EOF)
                {
                    return 0;
                }
            }
            if (opt.rnxver > 2.99)
            {
                return 1;
            }

            return fprintf(fp, "\n") != EOF;
        }
        /* output nav member by rinex nav format -------------------------------------*/
        internal static void outnavf(FILE fp, double value)
        {
            double e = Math.Abs(value) < 1E-99 ? 0.0 : Math.Floor(Math.Log10(Math.Abs(value)) + 1.0);
            fprintf(fp, " %s.%012.0fE%+03.0f", value < 0.0 ? "-" : " ", Math.Abs(value) / Math.Pow(10.0, e - 12.0), e);
        }
        /* output rinex nav header -----------------------------------------------------
        * output rinex nav file header
        * args   : FILE   *fp       I   output file pointer
        *          rnxopt_t *opt    I   rinex options
        *          nav_t  nav       I   navigation data (NULL: no input)
        * return : status (1:ok, 0:output error)
        *-----------------------------------------------------------------------------*/
        public static int outrnxnavh(FILE fp, rnxopt_t opt, nav_t nav)
        {
            int i;
            string date = new string(new char[64]);
            string sys;

            GlobalMembersRtkcmn.trace(3, "outrnxnavh:\n");

            GlobalMembersRinex.timestr_rnx(ref date);

            if (opt.rnxver <= 2.99) // ver.2
            {
                fprintf(fp, "%9.2f           %-20s%-20s%-20s\n", opt.rnxver, "N: GPS NAV DATA", "", "RINEX VERSION / TYPE");
            }
            else // ver.3
            {
                if (opt.navsys == DefineConstants.SYS_GPS)
                {
                    sys = "G: GPS";
                }
                else if (opt.navsys == DefineConstants.SYS_GLO)
                {
                    sys = "R: GLONASS";
                }
                else if (opt.navsys == DefineConstants.SYS_GAL)
                {
                    sys = "E: Galileo";
                }
                else if (opt.navsys == DefineConstants.SYS_QZS) // v.3.02
                {
                    sys = "J: QZSS";
                }
                else if (opt.navsys == DefineConstants.SYS_CMP) // v.3.02
                {
                    sys = "C: BeiDou";
                }
                else if (opt.navsys == DefineConstants.SYS_SBS)
                {
                    sys = "S: SBAS Payload";
                }
                else
                {
                    sys = "M: Mixed";
                }

                fprintf(fp, "%9.2f           %-20s%-20s%-20s\n", opt.rnxver, "N: GNSS NAV DATA", sys, "RINEX VERSION / TYPE");
            }
            fprintf(fp, "%-20.20s%-20.20s%-20.20s%-20s\n", opt.prog, opt.runby, date, "PGM / RUN BY / DATE");

            for (i = 0; i < DefineConstants.MAXCOMMENT; i++)
            {
                if (opt.comment[i] == 0)
                    continue;
                fprintf(fp, "%-60.60s%-20s\n", opt.comment[i], "COMMENT");
            }
            if (opt.rnxver <= 2.99) // ver.2
            {
                if (opt.outiono != 0)
                {
                    fprintf(fp, "  %12.4E%12.4E%12.4E%12.4E%10s%-20s\n", nav.ion_gps[0], nav.ion_gps[1], nav.ion_gps[2], nav.ion_gps[3], "", "ION ALPHA");
                    fprintf(fp, "  %12.4E%12.4E%12.4E%12.4E%10s%-20s\n", nav.ion_gps[4], nav.ion_gps[5], nav.ion_gps[6], nav.ion_gps[7], "", "ION BETA");
                }
                if (opt.outtime != 0)
                {
                    fprintf(fp, "   ");
                    GlobalMembersRinex.outnavf(fp, nav.utc_gps[0]);
                    GlobalMembersRinex.outnavf(fp, nav.utc_gps[1]);
                    fprintf(fp, "%9.0f%9.0f %-20s\n", nav.utc_gps[2], nav.utc_gps[3], "DELTA-UTC: A0,A1,T,W");
                }
            }
            else // ver.3
            {
                if ((opt.navsys & DefineConstants.SYS_GPS) != 0)
                {
                    if (opt.outiono != 0)
                    {
                        fprintf(fp, "GPSA %12.4E%12.4E%12.4E%12.4E%7s%-20s\n", nav.ion_gps[0], nav.ion_gps[1], nav.ion_gps[2], nav.ion_gps[3], "", "IONOSPHERIC CORR");
                        fprintf(fp, "GPSB %12.4E%12.4E%12.4E%12.4E%7s%-20s\n", nav.ion_gps[4], nav.ion_gps[5], nav.ion_gps[6], nav.ion_gps[7], "", "IONOSPHERIC CORR");
                    }
                }
                if ((opt.navsys & DefineConstants.SYS_GAL) != 0)
                {
                    if (opt.outiono != 0)
                    {
                        fprintf(fp, "GAL  %12.4E%12.4E%12.4E%12.4E%7s%-20s\n", nav.ion_gal[0], nav.ion_gal[1], nav.ion_gal[2], 0.0, "", "IONOSPHERIC CORR");
                    }
                }
                if ((opt.navsys & DefineConstants.SYS_QZS) != 0)
                {
                    if (opt.outiono != 0)
                    {
                        fprintf(fp, "QZSA %12.4E%12.4E%12.4E%12.4E%7s%-20s\n", nav.ion_qzs[0], nav.ion_qzs[1], nav.ion_qzs[2], nav.ion_qzs[3], "", "IONOSPHERIC CORR");
                        fprintf(fp, "QZSB %12.4E%12.4E%12.4E%12.4E%7s%-20s\n", nav.ion_qzs[4], nav.ion_qzs[5], nav.ion_qzs[6], nav.ion_qzs[7], "", "IONOSPHERIC CORR");
                    }
                }
                if ((opt.navsys & DefineConstants.SYS_GPS) != 0)
                {
                    if (opt.outtime != 0)
                    {
                        fprintf(fp, "GPUT %17.10E%16.9E%7.0f%5.0f %-5s %-2s %-20s\n", nav.utc_gps[0], nav.utc_gps[1], nav.utc_gps[2], nav.utc_gps[3], "", "", "TIME SYSTEM CORR");
                    }
                }
                if ((opt.navsys & DefineConstants.SYS_GAL) != 0)
                {
                    if (opt.outtime != 0)
                    {
                        fprintf(fp, "GAUT %17.10E%16.9E%7.0f%5.0f %-5s %-2s %-20s\n", nav.utc_gal[0], nav.utc_gal[1], nav.utc_gal[2], nav.utc_gal[3], "", "", "TIME SYSTEM CORR");
                    }
                }
                if ((opt.navsys & DefineConstants.SYS_QZS) != 0) // ver.3.02
                {
                    if (opt.outtime != 0)
                    {
                        fprintf(fp, "QZUT %17.10E%16.9E%7.0f%5.0f %-5s %-2s %-20s\n", nav.utc_qzs[0], nav.utc_qzs[1], nav.utc_qzs[2], nav.utc_qzs[3], "", "", "TIME SYSTEM CORR");
                    }
                }
                if ((opt.navsys & DefineConstants.SYS_CMP) != 0) // ver.3.02
                {
                    if (opt.outtime != 0)
                    {
                        fprintf(fp, "BDUT %17.10E%16.9E%7.0f%5.0f %-5s %-2s %-20s\n", nav.utc_cmp[0], nav.utc_cmp[1], nav.utc_cmp[2], nav.utc_cmp[3], "", "", "TIME SYSTEM CORR");
                    }
                }
            }
            if (opt.outleaps != 0)
            {
                fprintf(fp, "%6d%54s%-20s\n", nav.leaps, "", "LEAP SECONDS");
            }
            return fprintf(fp, "%60s%-20s\n", "", "END OF HEADER") != EOF;
        }
        /* output rinex nav body -------------------------------------------------------
        * output rinex nav file body record
        * args   : FILE   *fp       I   output file pointer
        *          rnxopt_t *opt    I   rinex options
        *          eph_t  *eph      I   ephemeris
        * return : status (1:ok, 0:output error)
        *-----------------------------------------------------------------------------*/
        public static int outrnxnavb(FILE fp, rnxopt_t opt, eph_t eph)
        {
            double[] ep = new double[6];
            double ttr;
            int week;
            int sys;
            int prn;
            string code = new string(new char[32]);
            string sep;

            GlobalMembersRtkcmn.trace(3, "outrnxgnavb: sat=%2d\n", eph.sat);

            if ((sys = GlobalMembersRtkcmn.satsys(eph.sat, ref prn)) == 0 || !(sys & opt.navsys))
            {
                return 0;
            }

            if (sys != DefineConstants.SYS_CMP)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: time2epoch(eph->toc,ep);
                GlobalMembersRtkcmn.time2epoch(new gtime_t(eph.toc), ep);
            }
            else
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: time2epoch(gpst2bdt(eph->toc),ep);
                GlobalMembersRtkcmn.time2epoch(GlobalMembersRtkcmn.gpst2bdt(new gtime_t(eph.toc)), ep); // gpst -> bdt
            }
            if (opt.rnxver > 2.99 || sys == DefineConstants.SYS_GAL || sys == DefineConstants.SYS_CMP) // ver.3 or ver.2 GAL
            {
                if (GlobalMembersRinex.sat2code(eph.sat, ref code) == 0)
                {
                    return 0;
                }
                fprintf(fp, "%-3s %04.0f %2.0f %2.0f %2.0f %2.0f %2.0f", code, ep[0], ep[1], ep[2], ep[3], ep[4], ep[5]);
                sep = "    ";
            }
            else if (sys == DefineConstants.SYS_QZS) // ver.2 or ver.3.02 QZS
            {
                if (GlobalMembersRinex.sat2code(eph.sat, ref code) == 0)
                {
                    return 0;
                }
                fprintf(fp, "%-3s %02d %2.0f %2.0f %2.0f %2.0f %4.1f", code, (int)ep[0] % 100, ep[1], ep[2], ep[3], ep[4], ep[5]);
                sep = "    ";
            }
            else
            {
                fprintf(fp, "%2d %02d %2.0f %2.0f %2.0f %2.0f %4.1f", prn, (int)ep[0] % 100, ep[1], ep[2], ep[3], ep[4], ep[5]);
                sep = "   ";
            }
            GlobalMembersRinex.outnavf(fp, eph.f0);
            GlobalMembersRinex.outnavf(fp, eph.f1);
            GlobalMembersRinex.outnavf(fp, eph.f2);
            fprintf(fp, "\n%s", sep);

            GlobalMembersRinex.outnavf(fp, eph.iode); // GPS/QZS: IODE, GAL: IODnav, BDS: AODE
            GlobalMembersRinex.outnavf(fp, eph.crs);
            GlobalMembersRinex.outnavf(fp, eph.deln);
            GlobalMembersRinex.outnavf(fp, eph.M0);
            fprintf(fp, "\n%s", sep);

            GlobalMembersRinex.outnavf(fp, eph.cuc);
            GlobalMembersRinex.outnavf(fp, eph.e);
            GlobalMembersRinex.outnavf(fp, eph.cus);
            GlobalMembersRinex.outnavf(fp, Math.Sqrt(eph.A));
            fprintf(fp, "\n%s", sep);

            GlobalMembersRinex.outnavf(fp, eph.toes);
            GlobalMembersRinex.outnavf(fp, eph.cic);
            GlobalMembersRinex.outnavf(fp, eph.OMG0);
            GlobalMembersRinex.outnavf(fp, eph.cis);
            fprintf(fp, "\n%s", sep);

            GlobalMembersRinex.outnavf(fp, eph.i0);
            GlobalMembersRinex.outnavf(fp, eph.crc);
            GlobalMembersRinex.outnavf(fp, eph.omg);
            GlobalMembersRinex.outnavf(fp, eph.OMGd);
            fprintf(fp, "\n%s", sep);

            GlobalMembersRinex.outnavf(fp, eph.idot);
            GlobalMembersRinex.outnavf(fp, eph.code);
            GlobalMembersRinex.outnavf(fp, eph.week); // GPS/QZS: GPS week, GAL: GAL week, BDS: BDT week
            GlobalMembersRinex.outnavf(fp, eph.flag);
            fprintf(fp, "\n%s", sep);

            GlobalMembersRinex.outnavf(fp, GlobalMembersRinex.uravalue(eph.sva));
            GlobalMembersRinex.outnavf(fp, eph.svh);
            GlobalMembersRinex.outnavf(fp, eph.tgd[0]); // GPS/QZS:TGD, GAL:BGD E5a/E1, BDS: TGD1 B1/B3
            if (sys == DefineConstants.SYS_GAL || sys == DefineConstants.SYS_CMP)
            {
                GlobalMembersRinex.outnavf(fp, eph.tgd[1]); // GAL:BGD E5b/E1, BDS: TGD2 B2/B3
            }
            else
            {
                GlobalMembersRinex.outnavf(fp, eph.iodc); // GPS/QZS:IODC
            }
            fprintf(fp, "\n%s", sep);

            if (sys != DefineConstants.SYS_CMP)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: ttr=time2gpst(eph->ttr,&week);
                ttr = GlobalMembersRtkcmn.time2gpst(new gtime_t(eph.ttr), ref week);
            }
            else
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: ttr=time2bdt(gpst2bdt(eph->ttr),&week);
                ttr = GlobalMembersRtkcmn.time2bdt(GlobalMembersRtkcmn.gpst2bdt(new gtime_t(eph.ttr)), ref week); // gpst -> bdt
            }
            GlobalMembersRinex.outnavf(fp, ttr + (week - eph.week) * 604800.0);

            if (sys == DefineConstants.SYS_GPS || sys == DefineConstants.SYS_QZS)
            {
                GlobalMembersRinex.outnavf(fp, eph.fit);
            }
            else if (sys == DefineConstants.SYS_CMP)
            {
                GlobalMembersRinex.outnavf(fp, eph.iodc); // AODC
            }
            else
            {
                GlobalMembersRinex.outnavf(fp, 0.0); // spare
            }
            return fprintf(fp, "\n") != EOF;
        }
        /* output rinex gnav header ----------------------------------------------------
        * output rinex gnav (glonass navigation) file header
        * args   : FILE   *fp       I   output file pointer
        *          rnxopt_t *opt    I   rinex options
        *          nav_t  nav       I   navigation data (NULL: no input)
        * return : status (1:ok, 0:output error)
        *-----------------------------------------------------------------------------*/
        public static int outrnxgnavh(FILE fp, rnxopt_t opt, nav_t nav)
        {
            int i;
            string date = new string(new char[64]);

            GlobalMembersRtkcmn.trace(3, "outrnxgnavh:\n");

            GlobalMembersRinex.timestr_rnx(ref date);

            if (opt.rnxver <= 2.99) // ver.2
            {
                fprintf(fp, "%9.2f           %-20s%-20s%-20s\n", opt.rnxver, "GLONASS NAV DATA", "", "RINEX VERSION / TYPE");
            }
            else // ver.3
            {
                fprintf(fp, "%9.2f           %-20s%-20s%-20s\n", opt.rnxver, "N: GNSS NAV DATA", "R: GLONASS", "RINEX VERSION / TYPE");
            }
            fprintf(fp, "%-20.20s%-20.20s%-20.20s%-20s\n", opt.prog, opt.runby, date, "PGM / RUN BY / DATE");

            for (i = 0; i < DefineConstants.MAXCOMMENT; i++)
            {
                if (opt.comment[i] == 0)
                    continue;
                fprintf(fp, "%-60.60s%-20s\n", opt.comment[i], "COMMENT");
            }
            return fprintf(fp, "%60s%-20s\n", "", "END OF HEADER") != EOF;
        }
        /* output rinex gnav body ------------------------------------------------------
        * output rinex gnav (glonass navigation) file body record
        * args   : FILE   *fp       I   output file pointer
        *          rnxopt_t *opt    I   rinex options
        *          geph_t  *geph    I   glonass ephemeris
        * return : status (1:ok, 0:output error)
        *-----------------------------------------------------------------------------*/
        public static int outrnxgnavb(FILE fp, rnxopt_t opt, geph_t geph)
        {
            gtime_t toe = new gtime_t();
            double[] ep = new double[6];
            double tof;
            int prn;
            string code = new string(new char[32]);
            string sep;

            GlobalMembersRtkcmn.trace(3, "outrnxgnavb: sat=%2d\n", geph.sat);

            if ((GlobalMembersRtkcmn.satsys(geph.sat, ref prn) & opt.navsys) != DefineConstants.SYS_GLO)
            {
                return 0;
            }

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: tof=time2gpst(gpst2utc(geph->tof),null);
            tof = GlobalMembersRtkcmn.time2gpst(GlobalMembersRtkcmn.gpst2utc(new gtime_t(geph.tof)), null); // v.3: tow in utc
            if (opt.rnxver <= 2.99) // v.2: tod in utc
            {
                tof = Math.IEEERemainder(tof, 86400.0);
            }

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: toe=gpst2utc(geph->toe);
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            toe.CopyFrom(GlobalMembersRtkcmn.gpst2utc(new gtime_t(geph.toe))); // gpst -> utc
                                                                               //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                                                                               //ORIGINAL LINE: time2epoch(toe,ep);
            GlobalMembersRtkcmn.time2epoch(new gtime_t(toe), ep);

            if (opt.rnxver <= 2.99) // ver.2
            {
                fprintf(fp, "%2d %02d %2.0f %2.0f %2.0f %2.0f %4.1f", prn, (int)ep[0] % 100, ep[1], ep[2], ep[3], ep[4], ep[5]);
                sep = "   ";
            }
            else // ver.3
            {
                if (GlobalMembersRinex.sat2code(geph.sat, ref code) == 0)
                {
                    return 0;
                }
                fprintf(fp, "%-3s %04.0f %2.0f %2.0f %2.0f %2.0f %2.0f", code, ep[0], ep[1], ep[2], ep[3], ep[4], ep[5]);
                sep = "    ";
            }
            GlobalMembersRinex.outnavf(fp, -geph.taun);
            GlobalMembersRinex.outnavf(fp, geph.gamn);
            GlobalMembersRinex.outnavf(fp, tof);
            fprintf(fp, "\n%s", sep);

            GlobalMembersRinex.outnavf(fp, geph.pos[0] / 1E3);
            GlobalMembersRinex.outnavf(fp, geph.vel[0] / 1E3);
            GlobalMembersRinex.outnavf(fp, geph.acc[0] / 1E3);
            GlobalMembersRinex.outnavf(fp, geph.svh);
            fprintf(fp, "\n%s", sep);

            GlobalMembersRinex.outnavf(fp, geph.pos[1] / 1E3);
            GlobalMembersRinex.outnavf(fp, geph.vel[1] / 1E3);
            GlobalMembersRinex.outnavf(fp, geph.acc[1] / 1E3);
            GlobalMembersRinex.outnavf(fp, geph.frq);
            fprintf(fp, "\n%s", sep);

            GlobalMembersRinex.outnavf(fp, geph.pos[2] / 1E3);
            GlobalMembersRinex.outnavf(fp, geph.vel[2] / 1E3);
            GlobalMembersRinex.outnavf(fp, geph.acc[2] / 1E3);
            GlobalMembersRinex.outnavf(fp, geph.age);

            return fprintf(fp, "\n") != EOF;
        }
        /* output rinex geo nav header -------------------------------------------------
        * output rinex geo nav file header
        * args   : FILE   *fp       I   output file pointer
        *          rnxopt_t *opt    I   rinex options
        *          nav_t  nav       I   navigation data (NULL: no input)
        * return : status (1:ok, 0:output error)
        *-----------------------------------------------------------------------------*/
        public static int outrnxhnavh(FILE fp, rnxopt_t opt, nav_t nav)
        {
            int i;
            string date = new string(new char[64]);

            GlobalMembersRtkcmn.trace(3, "outrnxhnavh:\n");

            GlobalMembersRinex.timestr_rnx(ref date);

            if (opt.rnxver <= 2.99) // ver.2
            {
                fprintf(fp, "%9.2f           %-20s%-20s%-20s\n", opt.rnxver, "H: GEO NAV MSG DATA", "", "RINEX VERSION / TYPE");
            }
            else // ver.3
            {
                fprintf(fp, "%9.2f           %-20s%-20s%-20s\n", opt.rnxver, "N: GNSS NAV DATA", "S: SBAS Payload", "RINEX VERSION / TYPE");
            }
            fprintf(fp, "%-20.20s%-20.20s%-20.20s%-20s\n", opt.prog, opt.runby, date, "PGM / RUN BY / DATE");

            for (i = 0; i < DefineConstants.MAXCOMMENT; i++)
            {
                if (opt.comment[i] == 0)
                    continue;
                fprintf(fp, "%-60.60s%-20s\n", opt.comment[i], "COMMENT");
            }
            return fprintf(fp, "%60s%-20s\n", "", "END OF HEADER") != EOF;
        }
        /* output rinex geo nav body ---------------------------------------------------
        * output rinex geo nav file body record
        * args   : FILE   *fp       I   output file pointer
        *          rnxopt_t *opt    I   rinex options
        *          seph_t  *seph    I   sbas ephemeris
        * return : status (1:ok, 0:output error)
        *-----------------------------------------------------------------------------*/
        public static int outrnxhnavb(FILE fp, rnxopt_t opt, seph_t seph)
        {
            double[] ep = new double[6];
            int prn;
            string code = new string(new char[32]);
            string sep;

            GlobalMembersRtkcmn.trace(3, "outrnxhnavb: sat=%2d\n", seph.sat);

            if ((GlobalMembersRtkcmn.satsys(seph.sat, ref prn) & opt.navsys) != DefineConstants.SYS_SBS)
            {
                return 0;
            }

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: time2epoch(seph->t0,ep);
            GlobalMembersRtkcmn.time2epoch(new gtime_t(seph.t0), ep);

            if (opt.rnxver <= 2.99) // ver.2
            {
                fprintf(fp, "%2d %02d %2.0f %2.0f %2.0f %2.0f %4.1f", prn - 100, (int)ep[0] % 100, ep[1], ep[2], ep[3], ep[4], ep[5]);
                sep = "   ";
            }
            else // ver.3
            {
                if (GlobalMembersRinex.sat2code(seph.sat, ref code) == 0)
                {
                    return 0;
                }
                fprintf(fp, "%-3s %04.0f %2.0f %2.0f %2.0f %2.0f %2.0f", code, ep[0], ep[1], ep[2], ep[3], ep[4], ep[5]);
                sep = "    ";
            }
            GlobalMembersRinex.outnavf(fp, seph.af0);
            GlobalMembersRinex.outnavf(fp, seph.af1);
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: outnavf(fp,time2gpst(seph->tof,null));
            GlobalMembersRinex.outnavf(fp, GlobalMembersRtkcmn.time2gpst(new gtime_t(seph.tof), null));
            fprintf(fp, "\n%s", sep);

            GlobalMembersRinex.outnavf(fp, seph.pos[0] / 1E3);
            GlobalMembersRinex.outnavf(fp, seph.vel[0] / 1E3);
            GlobalMembersRinex.outnavf(fp, seph.acc[0] / 1E3);
            GlobalMembersRinex.outnavf(fp, seph.svh);
            fprintf(fp, "\n%s", sep);

            GlobalMembersRinex.outnavf(fp, seph.pos[1] / 1E3);
            GlobalMembersRinex.outnavf(fp, seph.vel[1] / 1E3);
            GlobalMembersRinex.outnavf(fp, seph.acc[1] / 1E3);
            GlobalMembersRinex.outnavf(fp, GlobalMembersRinex.uravalue(seph.sva));
            fprintf(fp, "\n%s", sep);

            GlobalMembersRinex.outnavf(fp, seph.pos[2] / 1E3);
            GlobalMembersRinex.outnavf(fp, seph.vel[2] / 1E3);
            GlobalMembersRinex.outnavf(fp, seph.acc[2] / 1E3);
            GlobalMembersRinex.outnavf(fp, 0);

            return fprintf(fp, "\n") != EOF;
        }
        /* output rinex galileo nav header ---------------------------------------------
        * output rinex galileo nav file header (2.12)
        * args   : FILE   *fp       I   output file pointer
        *          rnxopt_t *opt    I   rinex options
        *          nav_t  nav       I   navigation data (NULL: no input)
        * return : status (1:ok, 0:output error)
        *-----------------------------------------------------------------------------*/
        public static int outrnxlnavh(FILE fp, rnxopt_t opt, nav_t nav)
        {
            int i;
            string date = new string(new char[64]);

            GlobalMembersRtkcmn.trace(3, "outrnxlnavh:\n");

            GlobalMembersRinex.timestr_rnx(ref date);

            fprintf(fp, "%9.2f           %-20s%-20s%-20s\n", opt.rnxver, "N: GNSS NAV DATA", "E: Galileo", "RINEX VERSION / TYPE");

            fprintf(fp, "%-20.20s%-20.20s%-20.20s%-20s\n", opt.prog, opt.runby, date, "PGM / RUN BY / DATE");

            for (i = 0; i < DefineConstants.MAXCOMMENT; i++)
            {
                if (opt.comment[i] == 0)
                    continue;
                fprintf(fp, "%-60.60s%-20s\n", opt.comment[i], "COMMENT");
            }
            return fprintf(fp, "%60s%-20s\n", "", "END OF HEADER") != EOF;
        }
        /* output rinex qzss nav header ------------------------------------------------
        * output rinex qzss nav file header (2.12 extention and 3.02)
        * args   : FILE   *fp       I   output file pointer
        *          rnxopt_t *opt    I   rinex options
        *          nav_t  nav       I   navigation data (NULL: no input)
        * return : status (1:ok, 0:output error)
        *-----------------------------------------------------------------------------*/
        public static int outrnxqnavh(FILE fp, rnxopt_t opt, nav_t nav)
        {
            int i;
            string date = new string(new char[64]);

            GlobalMembersRtkcmn.trace(3, "outrnxqnavh:\n");

            GlobalMembersRinex.timestr_rnx(ref date);

            fprintf(fp, "%9.2f           %-20s%-20s%-20s\n", opt.rnxver, "N: GNSS NAV DATA", "J: QZSS", "RINEX VERSION / TYPE");

            fprintf(fp, "%-20.20s%-20.20s%-20.20s%-20s\n", opt.prog, opt.runby, date, "PGM / RUN BY / DATE");

            for (i = 0; i < DefineConstants.MAXCOMMENT; i++)
            {
                if (opt.comment[i] == 0)
                    continue;
                fprintf(fp, "%-60.60s%-20s\n", opt.comment[i], "COMMENT");
            }
            return fprintf(fp, "%60s%-20s\n", "", "END OF HEADER") != EOF;
        }
        /* output rinex beidou nav header ----------------------------------------------
        * output rinex beidou nav file header (2.12 extention and 3.02)
        * args   : FILE   *fp       I   output file pointer
        *          rnxopt_t *opt    I   rinex options
        *          nav_t  nav       I   navigation data (NULL: no input)
        * return : status (1:ok, 0:output error)
        *-----------------------------------------------------------------------------*/
        public static int outrnxcnavh(FILE fp, rnxopt_t opt, nav_t nav)
        {
            int i;
            string date = new string(new char[64]);

            GlobalMembersRtkcmn.trace(3, "outrnxcnavh:\n");

            GlobalMembersRinex.timestr_rnx(ref date);

            fprintf(fp, "%9.2f           %-20s%-20s%-20s\n", opt.rnxver, "N: GNSS NAV DATA", "C: BeiDou", "RINEX VERSION / TYPE");

            fprintf(fp, "%-20.20s%-20.20s%-20.20s%-20s\n", opt.prog, opt.runby, date, "PGM / RUN BY / DATE");

            for (i = 0; i < DefineConstants.MAXCOMMENT; i++)
            {
                if (opt.comment[i] == 0)
                    continue;
                fprintf(fp, "%-60.60s%-20s\n", opt.comment[i], "COMMENT");
            }
            return fprintf(fp, "%60s%-20s\n", "", "END OF HEADER") != EOF;
        }
    }

    /* type definition -----------------------------------------------------------*/
    public class sigind_t // signal index type
    {
        public int n; // number of index
        public int[] frq = new int[DefineConstants.MAXOBSTYPE]; // signal frequency (1:L1,2:L2,...)
        public int[] pos = new int[DefineConstants.MAXOBSTYPE]; // signal index in obs data (-1:no)
        public byte[] pri = new byte[DefineConstants.MAXOBSTYPE]; // signal priority (15-0)
        public byte[] type = new byte[DefineConstants.MAXOBSTYPE]; // type (0:C,1:L,2:D,3:S)
        public byte[] code = new byte[DefineConstants.MAXOBSTYPE]; // obs code (CODE_L??)
        public double[] shift = new double[DefineConstants.MAXOBSTYPE]; // phase shift (cycle)
    }
}
