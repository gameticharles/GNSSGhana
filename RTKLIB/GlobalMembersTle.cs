using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ghGPS.Classes
{
    public static class GlobalMembersTle
    {
        /*------------------------------------------------------------------------------
        * tle.c: NORAD TLE (two line element) functions
        *
        *          Copyright (C) 2012-2013 by T.TAKASU, All rights reserved.
        *
        * references:
        *     [1] F.R.Hoots and R.L.Roehrich, Spacetrack report No.3, Models for
        *         propagation of NORAD element sets, December 1980
        *     [2] D.A.Vallado, P.Crawford, R.Hujsak and T.S.Kelso, Revisiting
        *         Spacetrack Report #3, AIAA 2006-6753, 2006
        *     [3] CelesTrak (http://www.celestrak.com)
        *
        * version : $Revision:$ $Date:$
        * history : 2012/11/01 1.0  new
        *           2013/01/25 1.1  fix bug on binary search
        *           2014/08/26 1.2  fix bug on tle_pos() to get tle by satid or desig
        *-----------------------------------------------------------------------------*/


        internal static void SGP4_STR3(double tsince, tled_t data, double[] rs)
        {
            double xnodeo;
            double omegao;
            double xmo;
            double eo;
            double xincl;
            double xno;
            double xndt2o;
            double xndd6o;
            double bstar;
            double a1;
            double cosio;
            double theta2;
            double x3thm1;
            double eosq;
            double betao2;
            double betao;
            double del1;
            double ao;
            double delo;
            double xnodp;
            double aodp;
            double s4;
            double qoms24;
            double perige;
            double pinvsq;
            double tsi;
            double eta;
            double etasq;
            double eeta;
            double psisq;
            double coef;
            double coef1;
            double c1;
            double c2;
            double c3;
            double c4;
            double c5;
            double sinio;
            double a3ovk2;
            double x1mth2;
            double theta4;
            double xmdot;
            double x1m5th;
            double omgdot;
            double xhdot1;
            double xnodot;
            double omgcof;
            double xmcof;
            double xnodcf;
            double t2cof;
            double xlcof;
            double aycof;
            double delmo;
            double sinmo;
            double x7thm1;
            double c1sq;
            double d2;
            double d3;
            double d4;
            double t3cof;
            double t4cof;
            double t5cof;
            double xmdf;
            double omgadf;
            double xnoddf;
            double omega;
            double xmp;
            double tsq;
            double xnode;
            double delomg;
            double delm;
            double tcube;
            double tfour;
            double a;
            double e;
            double xl;
            double beta;
            double xn;
            double axn;
            double xll;
            double aynl;
            double xlt;
            double ayn;
            double capu;
            double sinepw;
            double cosepw;
            double epw;
            double ecose;
            double esine;
            double elsq;
            double pl;
            double r;
            double rdot;
            double rfdot;
            double betal;
            double cosu;
            double sinu;
            double u;
            double sin2u;
            double cos2u;
            double rk;
            double uk;
            double xnodek;
            double xinck;
            double rdotk;
            double rfdotk;
            double sinuk;
            double cosuk;
            double sinik;
            double cosik;
            double sinnok;
            double cosnok;
            double xmx;
            double xmy;
            double ux;
            double uy;
            double uz;
            double vx;
            double vy;
            double vz;
            double x;
            double y;
            double z;
            double xdot;
            double ydot;
            double zdot;
            double temp;
            double temp1;
            double temp2;
            double temp3;
            double temp4;
            double temp5;
            double temp6;
            double tempa;
            double tempe;
            double templ;
            int i;
            int isimp;

            xnodeo = data.OMG * DefineConstants.DE2RA;
            omegao = data.omg * DefineConstants.DE2RA;
            xmo = data.M * DefineConstants.DE2RA;
            xincl = data.inc * DefineConstants.DE2RA;
            temp = DefineConstants.TWOPI / DefineConstants.XMNPDA / DefineConstants.XMNPDA;
            xno = data.n * temp * DefineConstants.XMNPDA;
            xndt2o = data.ndot * temp;
            xndd6o = data.nddot * temp / DefineConstants.XMNPDA;
            bstar = data.bstar / DefineConstants.AE;
            eo = data.ecc;
            /*
            * recover original mean motion (xnodp) and semimajor axis (aodp)
            * from input elements
            */
            a1 = Math.Pow(DefineConstants.XKE / xno, DefineConstants.TOTHRD);
            cosio = Math.Cos(xincl);
            theta2 = cosio * cosio;
            x3thm1 = 3.0 * theta2 - 1.0;
            eosq = eo * eo;
            betao2 = 1.0 - eosq;
            betao = Math.Sqrt(betao2);
            del1 = 1.5 * DefineConstants.CK2 * x3thm1 / (a1 * a1 * betao * betao2);
            ao = a1 * (1.0 - del1 * (0.5 * DefineConstants.TOTHRD + del1 * (1.0 + 134.0 / 81.0 * del1)));
            delo = 1.5 * DefineConstants.CK2 * x3thm1 / (ao * ao * betao * betao2);
            xnodp = xno / (1.0 + delo);
            aodp = ao / (1.0 - delo);
            /*
            * initialization
            * for perigee less than 220 kilometers, the isimp flag is set and
            * the equations are truncated to linear variation in sqrt a and
            * quadratic variation in mean anomaly. also, the c3 term, the
            * delta omega term, and the delta m term are dropped.
            */
            isimp = 0;
            if ((aodp * (1.0 - eo) / DefineConstants.AE) < (220.0 / DefineConstants.XKMPER + DefineConstants.AE))
            {
                isimp = 1;
            }

            /* for perigee below 156 km, the values of s and qoms2t are altered */
            s4 = DefineConstants.S;
            qoms24 = DefineConstants.QOMS2T;
            perige = (aodp * (1.0 - eo) - DefineConstants.AE) * DefineConstants.XKMPER;
            if (perige < 156.0)
            {
                s4 = perige - 78.0;
                if (perige <= 98.0)
                {
                    s4 = 20.0;
                }
                qoms24 = Math.Pow((120.0 - s4) * DefineConstants.AE / DefineConstants.XKMPER, 4.0);
                s4 = s4 / DefineConstants.XKMPER + DefineConstants.AE;
            }
            pinvsq = 1.0 / (aodp * aodp * betao2 * betao2);
            tsi = 1.0 / (aodp - s4);
            eta = aodp * eo * tsi;
            etasq = eta * eta;
            eeta = eo * eta;
            psisq = Math.Abs(1.0 - etasq);
            coef = qoms24 * Math.Pow(tsi, 4.0);
            coef1 = coef / Math.Pow(psisq, 3.5);
            c2 = coef1 * xnodp * (aodp * (1.0 + 1.5 * etasq + eeta * (4.0 + etasq)) + 0.75 * DefineConstants.CK2 * tsi / psisq * x3thm1 * (8.0 + 3.0 * etasq * (8.0 + etasq)));
            c1 = bstar * c2;
            sinio = Math.Sin(xincl);
            a3ovk2 = -DefineConstants.XJ3 / DefineConstants.CK2 * Math.Pow(DefineConstants.AE, 3.0);
            c3 = coef * tsi * a3ovk2 * xnodp * DefineConstants.AE * sinio / eo;
            x1mth2 = 1.0 - theta2;
            c4 = 2.0 * xnodp * coef1 * aodp * betao2 * (eta * (2.0 + 0.5 * etasq) + eo * (0.5 + 2.0 * etasq) - 2.0 * DefineConstants.CK2 * tsi / (aodp * psisq) * (-3.0 * x3thm1 * (1.0 - 2.0 * eeta + etasq * (1.5 - 0.5 * eeta)) + 0.75 * x1mth2 * (2.0 * etasq - eeta * (1.0 + etasq)) * Math.Cos(2.0 * omegao)));
            c5 = 2.0 * coef1 * aodp * betao2 * (1.0 + 2.75 * (etasq + eeta) + eeta * etasq);
            theta4 = theta2 * theta2;
            temp1 = 3.0 * DefineConstants.CK2 * pinvsq * xnodp;
            temp2 = temp1 * DefineConstants.CK2 * pinvsq;
            temp3 = 1.25 * DefineConstants.CK4 * pinvsq * pinvsq * xnodp;
            xmdot = xnodp + 0.5 * temp1 * betao * x3thm1 + 0.0625 * temp2 * betao * (13.0 - 78.0 * theta2 + 137.0 * theta4);
            x1m5th = 1.0 - 5.0 * theta2;
            omgdot = -0.5 * temp1 * x1m5th + 0.0625 * temp2 * (7.0 - 114.0 * theta2 + 395.0 * theta4) + temp3 * (3.0 - 36.0 * theta2 + 49.0 * theta4);
            xhdot1 = -temp1 * cosio;
            xnodot = xhdot1 + (0.5 * temp2 * (4.0 - 19.0 * theta2) + 2.0 * temp3 * (3.0 - 7.0 * theta2)) * cosio;
            omgcof = bstar * c3 * Math.Cos(omegao);
            xmcof = -DefineConstants.TOTHRD * coef * bstar * DefineConstants.AE / eeta;
            xnodcf = 3.5 * betao2 * xhdot1 * c1;
            t2cof = 1.5 * c1;
            xlcof = 0.125 * a3ovk2 * sinio * (3.0 + 5.0 * cosio) / (1.0 + cosio);
            aycof = 0.25 * a3ovk2 * sinio;
            delmo = Math.Pow(1.0 + eta * Math.Cos(xmo), 3.0);
            sinmo = Math.Sin(xmo);
            x7thm1 = 7.0 * theta2 - 1.0;

            if (isimp != 1)
            {
                c1sq = c1 * c1;
                d2 = 4.0 * aodp * tsi * c1sq;
                temp = d2 * tsi * c1 / 3.0;
                d3 = (17.0 * aodp + s4) * temp;
                d4 = 0.5 * temp * aodp * tsi * (221.0 * aodp + 31.0 * s4) * c1;
                t3cof = d2 + 2.0 * c1sq;
                t4cof = 0.25 * (3.0 * d3 + c1 * (12.0 * d2 + 10.0 * c1sq));
                t5cof = 0.2 * (3.0 * d4 + 12.0 * c1 * d3 + 6.0 * d2 * d2 + 15.0 * c1sq * (2.0 * d2 + c1sq));
            }
            else
            {
                d2 = d3 = d4 = t3cof = t4cof = t5cof = 0.0;
            }
            /* update for secular gravity and atmospheric drag */
            xmdf = xmo + xmdot * tsince;
            omgadf = omegao + omgdot * tsince;
            xnoddf = xnodeo + xnodot * tsince;
            omega = omgadf;
            xmp = xmdf;
            tsq = tsince * tsince;
            xnode = xnoddf + xnodcf * tsq;
            tempa = 1.0 - c1 * tsince;
            tempe = bstar * c4 * tsince;
            templ = t2cof * tsq;
            if (isimp == 1)
            {
                delomg = omgcof * tsince;
                delm = xmcof * (Math.Pow(1.0 + eta * Math.Cos(xmdf), 3.0) - delmo);
                temp = delomg + delm;
                xmp = xmdf + temp;
                omega = omgadf - temp;
                tcube = tsq * tsince;
                tfour = tsince * tcube;
                tempa = tempa - d2 * tsq - d3 * tcube - d4 * tfour;
                tempe = tempe + bstar * c5 * (Math.Sin(xmp) - sinmo);
                templ = templ + t3cof * tcube + tfour * (t4cof + tsince * t5cof);
            }
            a = aodp * Math.Pow(tempa, 2.0);
            e = eo - tempe;
            xl = xmp + omega + xnode + xnodp * templ;
            beta = Math.Sqrt(1.0 - e * e);
            xn = DefineConstants.XKE / Math.Pow(a, 1.5);

            /* long period periodics */
            axn = e * Math.Cos(omega);
            temp = 1.0 / (a * beta * beta);
            xll = temp * xlcof * axn;
            aynl = temp * aycof;
            xlt = xl + xll;
            ayn = e * Math.Sin(omega) + aynl;

            /* solve keplers equation */
            capu = Math.IEEERemainder(xlt - xnode, DefineConstants.TWOPI);
            temp2 = capu;
            for (i = 0; i < 10; i++)
            {
                sinepw = Math.Sin(temp2);
                cosepw = Math.Cos(temp2);
                temp3 = axn * sinepw;
                temp4 = ayn * cosepw;
                temp5 = axn * cosepw;
                temp6 = ayn * sinepw;
                epw = (capu - temp4 + temp3 - temp2) / (1.0 - temp5 - temp6) + temp2;
                if (Math.Abs(epw - temp2) <= DefineConstants.E6A)
                    break;
                temp2 = epw;
            }
            /* short period preliminary quantities */
            ecose = temp5 + temp6;
            esine = temp3 - temp4;
            elsq = axn * axn + ayn * ayn;
            temp = 1.0 - elsq;
            pl = a * temp;
            r = a * (1.0 - ecose);
            temp1 = 1.0 / r;
            rdot = DefineConstants.XKE * Math.Sqrt(a) * esine * temp1;
            rfdot = DefineConstants.XKE * Math.Sqrt(pl) * temp1;
            temp2 = a * temp1;
            betal = Math.Sqrt(temp);
            temp3 = 1.0 / (1.0 + betal);
            cosu = temp2 * (cosepw - axn + ayn * esine * temp3);
            sinu = temp2 * (sinepw - ayn - axn * esine * temp3);
            u = Math.Atan2(sinu, cosu);
            sin2u = 2.0 * sinu * cosu;
            cos2u = 2.0 * cosu * cosu - 1.0;
            temp = 1.0 / pl;
            temp1 = DefineConstants.CK2 * temp;
            temp2 = temp1 * temp;

            /* update for short periodics */
            rk = r * (1.0 - 1.5 * temp2 * betal * x3thm1) + 0.5 * temp1 * x1mth2 * cos2u;
            uk = u - 0.25 * temp2 * x7thm1 * sin2u;
            xnodek = xnode + 1.5 * temp2 * cosio * sin2u;
            xinck = xincl + 1.5 * temp2 * cosio * sinio * cos2u;
            rdotk = rdot - xn * temp1 * x1mth2 * sin2u;
            rfdotk = rfdot + xn * temp1 * (x1mth2 * cos2u + 1.5 * x3thm1);

            /* orientation vectors */
            sinuk = Math.Sin(uk);
            cosuk = Math.Cos(uk);
            sinik = Math.Sin(xinck);
            cosik = Math.Cos(xinck);
            sinnok = Math.Sin(xnodek);
            cosnok = Math.Cos(xnodek);
            xmx = -sinnok * cosik;
            xmy = cosnok * cosik;
            ux = xmx * sinuk + cosnok * cosuk;
            uy = xmy * sinuk + sinnok * cosuk;
            uz = sinik * sinuk;
            vx = xmx * cosuk - cosnok * sinuk;
            vy = xmy * cosuk - sinnok * sinuk;
            vz = sinik * cosuk;

            /* position and velocity */
            x = rk * ux;
            y = rk * uy;
            z = rk * uz;
            xdot = rdotk * ux + rfdotk * vx;
            ydot = rdotk * uy + rfdotk * vy;
            zdot = rdotk * uz + rfdotk * vz;

            rs[0] = x * DefineConstants.XKMPER / DefineConstants.AE * 1E3; // (m)
            rs[1] = y * DefineConstants.XKMPER / DefineConstants.AE * 1E3;
            rs[2] = z * DefineConstants.XKMPER / DefineConstants.AE * 1E3;
            rs[3] = xdot * DefineConstants.XKMPER / DefineConstants.AE * DefineConstants.XMNPDA / 86400.0 * 1E3; // (m/s)
            rs[4] = ydot * DefineConstants.XKMPER / DefineConstants.AE * DefineConstants.XMNPDA / 86400.0 * 1E3;
            rs[5] = zdot * DefineConstants.XKMPER / DefineConstants.AE * DefineConstants.XMNPDA / 86400.0 * 1E3;
        }
        /* drop spaces at string tail ------------------------------------------------*/
        internal static void chop(ref string buff)
        {
            int i;
            for (i = buff.Length - 1; i >= 0; i--)
            {
                if (buff[i] == ' ' || buff[i] == '\r' || buff[i] == '\n')
                {
                    buff[i] = '\0';
                }
                else
                    break;
            }
        }
        /* test TLE line checksum ----------------------------------------------------*/
        internal static int checksum(string buff)
        {
            int i;
            int cs = 0;

            if (buff.Length < 69)
            {
                return 0;
            }

            for (i = 0; i < 68; i++)
            {
                if ('0' <= buff[i] && buff[i] <= '9')
                {
                    cs += (int)(buff[i] - '0');
                }
                else if (buff[i] == '-')
                {
                    cs += 1;
                }
            }
            return (int)(buff[68] - '0') == cs % 10;
        }
        /* decode TLE line 1 ---------------------------------------------------------*/
        internal static int decode_line1(string buff, tled_t data)
        {
            double year;
            double doy;
            double nddot;
            double exp1;
            double bstar;
            double exp2;
            double[] ep = { 2000, 1, 1, null, null, null };

            data.satno = buff.Substring(2, 5); // satellite number
            data.satno[5] = '\0';
            GlobalMembersOptions.chop(ref data.satno);

            data.satclass = buff[7]; // satellite classification
            data.desig = buff.Substring(9, 8); // international designator
            data.desig[8] = '\0';
            GlobalMembersOptions.chop(ref data.desig);

            year = GlobalMembersRtkcmn.str2num(buff, 18, 2); // epoch year
            doy = GlobalMembersRtkcmn.str2num(buff, 20, 12); // epoch day of year
            data.ndot = GlobalMembersRtkcmn.str2num(buff, 33, 10); // 1st time derivative of n
            nddot = GlobalMembersRtkcmn.str2num(buff, 44, 6); // 2nd time derivative of n
            exp1 = GlobalMembersRtkcmn.str2num(buff, 50, 2);
            bstar = GlobalMembersRtkcmn.str2num(buff, 53, 6); // Bstar drag term
            exp2 = GlobalMembersRtkcmn.str2num(buff, 59, 2);
            data.etype = (int)GlobalMembersRtkcmn.str2num(buff, 62, 1); // ephemeris type
            data.eleno = (int)GlobalMembersRtkcmn.str2num(buff, 64, 4); // ephemeris number
            data.nddot = nddot * 1E-5 * Math.Pow(10.0, exp1);
            data.bstar = bstar * 1E-5 * Math.Pow(10.0, exp2);

            ep[0] = year + (year < 57.0 ? 2000.0 : 1900.0);
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: data->epoch=timeadd(epoch2time(ep),(doy-1.0)*86400.0);
            data.epoch.CopyFrom(GlobalMembersRtkcmn.timeadd(GlobalMembersRtkcmn.epoch2time(ep), (doy - 1.0) * 86400.0));

            data.inc = data.OMG = data.ecc = data.omg = data.M = data.n = 0.0;
            data.rev = 0;
            return 1;
        }
        /* decode TLE line 2 ---------------------------------------------------------*/
        internal static int decode_line2(string buff, tled_t data)
        {
            string satno = new string(new char[16]);

            satno = buff.Substring(2, 5); // satellite number
            satno[5] = '\0';
            GlobalMembersOptions.chop(ref satno);

            data.inc = GlobalMembersRtkcmn.str2num(buff, 8, 8); // inclination (deg)
            data.OMG = GlobalMembersRtkcmn.str2num(buff, 17, 8); // RAAN (deg)
            data.ecc = GlobalMembersRtkcmn.str2num(buff, 26, 7) * 1E-7; // eccentricity
            data.omg = GlobalMembersRtkcmn.str2num(buff, 34, 8); // argument of perigee (deg)
            data.M = GlobalMembersRtkcmn.str2num(buff, 43, 8); // mean anomaly (deg)
            data.n = GlobalMembersRtkcmn.str2num(buff, 52, 11); // mean motion (rev/day)
            data.rev = (int)GlobalMembersRtkcmn.str2num(buff, 63, 5); // revolution number

            if (string.Compare(satno, data.satno))
            {
                GlobalMembersRtkcmn.trace(2, "tle satno mismatch: %s %s\n", data.satno, satno);
                return 0;
            }
            if (data.n <= 0.0 || data.ecc < 0.0)
            {
                GlobalMembersRtkcmn.trace(2, "tle data error: %s\n", satno);
                return 0;
            }
            return 1;
        }
        /* add TLE data --------------------------------------------------------------*/
        internal static int add_data(tle_t tle, tled_t[] data)
        {
            tled_t tle_data;

            if (tle.n >= tle.nmax)
            {
                tle.nmax = tle.nmax <= 0 ? 1024 : tle.nmax * 2;

                //C++ TO C# CONVERTER TODO TASK: The memory management function 'realloc' has no equivalent in C#:
                if ((tle_data = (tled_t)realloc(tle.data, sizeof(tled_t) * tle.nmax)) == null)
                {
                    GlobalMembersRtkcmn.trace(1, "tle malloc error\n");
                    //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
                    free(tle.data);
                    tle.data = null;
                    tle.n = tle.nmax = 0;
                    return 0;
                }
                tle.data = tle_data;
            }
            tle.data[tle.n++] = *data;
            return 1;
        }
        /* compare TLE data by satellite name ----------------------------------------*/
        internal static int cmp_tle_data(object p1, object p2)
        {
            tled_t q1 = (tled_t)p1;
            tled_t q2 = (tled_t)p2;
            return string.Compare(q1.name, q2.name);
        }
        /* read TLE file ---------------------------------------------------------------
        * read NORAD TLE (two line element) data file (ref [2],[3])
        * args   : char   *file     I   NORAD TLE data file
        *          tle_t  *tle      O   TLE data
        * return : status (1:ok,0:error)
        * notes  : before calling the function, the TLE data should be initialized.
        *          the file should be in a two line (only TLE) or three line (satellite
        *          name + TLE) format.
        *          the characters after # in a line are treated as comments.
        *-----------------------------------------------------------------------------*/
        public static int tle_read(string file, tle_t tle)
        {
            FILE fp;
            tled_t data = new tled_t({ 0 });
            string p;
            string buff = new string(new char[256]);
            int line = 0;

            if ((fp = fopen(file, "r")) == null)
            {
                GlobalMembersRtkcmn.trace(2, "tle file open error: %s\n", file);
                return 0;
            }
            while (fgets(buff, sizeof(sbyte), fp))
            {

                /* delete comments */
                if ((p = StringFunctions.StrChr(buff, '#')) != 0)
                {
                    p = (sbyte)'\0';
                }
                GlobalMembersOptions.chop(ref buff);

                if (buff[0] == '1' && GlobalMembersJavad.checksum(buff))
                {

                    /* decode TLE line 1 */
                    if (GlobalMembersTle.decode_line1(buff, data) != 0)
                    {
                        line = 1;
                    }
                }
                else if (line == 1 && buff[0] == '2' && GlobalMembersJavad.checksum(buff))
                {

                    /* decode TLE line 2 */
                    if (GlobalMembersTle.decode_line2(buff, data) == 0)
                        continue;

                    /* add TLE data */
                    if (GlobalMembersTle.add_data(tle, data) == 0)
                    {
                        fclose(fp);
                        return 0;
                    }
                    data.name[0] = '\0';
                    data.alias[0] = '\0';
                }
                else if (buff[0])
                {

                    /* satellite name in three line format */
                    data.name = buff;

                    /* omit words in parentheses */
                    if ((p = StringFunctions.StrChr(data.name, '(')) != 0)
                    {
                        p = (sbyte)'\0';
                    }
                    GlobalMembersOptions.chop(ref data.name);
                    line = 0;
                }
            }
            fclose(fp);

            /* sort tle data by satellite name */
            if (tle.n > 0)
            {
                qsort(tle.data, tle.n, sizeof(tled_t), GlobalMembersTle.cmp_tle_data);
            }
            return 1;
        }
        /* read TLE satellite name file ------------------------------------------------
        * read TLE satellite name file
        * args   : char   *file     I   TLE satellite name file
        *          tle_t  *tle      IO  TLE data
        * return : status (1:ok,0:error)
        * notes  : before calling the function, call tle_read() to read tle table
        *          the TLE satellite name file contains the following record as a text
        *          line. strings after # are treated as comments.
        *
        *          name satno [desig [# comment]]
        *
        *            name : satellite name
        *            satno: satellite catalog number
        *            desig: international designator (optional)
        *-----------------------------------------------------------------------------*/
        public static int tle_name_read(string file, tle_t tle)
        {
            FILE fp;
            string p;
            string buff = new string(new char[256]);
            string name = new string(new char[256]);
            string satno = new string(new char[256]);
            string desig = new string(new char[256]);
            int i;

            if ((fp = fopen(file, "r")) == null)
            {
                GlobalMembersRtkcmn.trace(2, "tle satellite name file open error: %s\n", file);
                return 0;
            }
            while (fgets(buff, sizeof(sbyte), fp))
            {

                if ((p = StringFunctions.StrChr(buff, '#')) != 0)
                {
                    p = (sbyte)'\0';
                }

                desig[0] = '\0';

                if (sscanf(buff, "%s %s %s", name, satno, desig) < 2)
                    continue;
                satno[5] = '\0';

                for (i = 0; i < tle.n; i++)
                {
                    if (!string.Compare(tle.data[i].satno, satno) || !string.Compare(tle.data[i].desig, desig))
                        break;
                }
                if (i >= tle.n)
                {
                    GlobalMembersRtkcmn.trace(3, "no tle data: satno=%s desig=%s\n", satno, desig);
                    continue;
                }
                tle.data[i].name = name.Substring(0, 31);
                tle.data[i].name[31] = '\0';
            }
            fclose(fp);

            /* sort tle data by satellite name */
            if (tle.n > 0)
            {
                qsort(tle.data, tle.n, sizeof(tled_t), GlobalMembersTle.cmp_tle_data);
            }
            return 1;
        }
        /* satellite position and velocity with TLE data -------------------------------
        * compute satellite position and velocity in ECEF with TLE data
        * args   : gtime_t time     I   time (GPST)
        *          char   *name     I   satellite name           ("": not specified)
        *          char   *satno    I   satellite catalog number ("": not specified)
        *          char   *desig    I   international designaor  ("": not specified)
        *          tle_t  *tle      I   TLE data
        *          erp_t  *erp      I   EOP data (NULL: not used)
        *          double *rs       O   sat position/velocity {x,y,z,vx,vy,vz} (m,m/s)
        * return : status (1:ok,0:error)
        * notes  : the coordinates of the position and velocity are ECEF (ITRF)
        *          if erp == NULL, polar motion and ut1-utc are neglected
        *-----------------------------------------------------------------------------*/
        public static int tle_pos(gtime_t time, string name, string satno, string desig, tle_t tle, erp_t erp, ref double rs)
        {
            gtime_t tutc = new gtime_t();
            double tsince;
            double[] rs_tle = new double[6];
            double[] rs_pef = new double[6];
            double gmst;
            double[] R1 = { 0, null, null, null, null, null, null, null, null };
            double[] R2 = { 0, null, null, null, null, null, null, null, null };
            double[] R3 = { 0, null, null, null, null, null, null, null, null };
            double[] W = new double[9];
            double[] erpv = { 0, null, null, null, null };
            int i = 0;
            int j;
            int k;
            int stat = 1;

            /* binary search by satellite name */
            if (name != 0)
            {
                for (i = j = 0, k = tle.n - 1; j <= k;)
                {
                    i = (j + k) / 2;
                    if ((stat = string.Compare(name, tle.data[i].name)) == 0)
                        break;
                    if (stat < 0)
                    {
                        k = i - 1;
                    }
                    else
                    {
                        j = i + 1;
                    }
                }
            }
            /* serial search by catalog no or international designator */
            if (stat != 0 && (satno != 0 || desig != 0))
            {
                for (i = 0; i < tle.n; i++)
                {
                    if (!string.Compare(tle.data[i].satno, satno) || !string.Compare(tle.data[i].desig, desig))
                        break;
                }
                if (i < tle.n)
                {
                    stat = 0;
                }
            }
            if (stat != 0)
            {
                GlobalMembersRtkcmn.trace(3, "no tle data: name=%s satno=%s desig=%s\n", name, satno, desig);
                return 0;
            }
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: tutc=gpst2utc(time);
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            tutc.CopyFrom(GlobalMembersRtkcmn.gpst2utc(new gtime_t(time)));

            /* time since epoch (min) */
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: tsince=timediff(tutc,tle->data[i].epoch)/60.0;
            tsince = GlobalMembersRtkcmn.timediff(new gtime_t(tutc), tle.data[i].epoch) / 60.0;

            /* SGP4 model propagator by STR#3 */
            GlobalMembersTle.SGP4_STR3(tsince, tle.data + i, rs_tle);

            /* erp values */
            if (erp != null)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: geterp(erp,time,erpv);
                GlobalMembersRtkcmn.geterp(erp, new gtime_t(time), erpv);
            }

            /* GMST (rad) */
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: gmst=utc2gmst(tutc,erpv[2]);
            gmst = GlobalMembersRtkcmn.utc2gmst(new gtime_t(tutc), erpv[2]);

            /* TEME (true equator, mean eqinox) -> ECEF (ref [2] IID, Appendix C) */
            R1[0] = 1.0;
            R1[4] = R1[8] = Math.Cos(-erpv[1]);
            R1[7] = Math.Sin(-erpv[1]);
            R1[5] = -R1[7];
            R2[4] = 1.0;
            R2[0] = R2[8] = Math.Cos(-erpv[0]);
            R2[2] = Math.Sin(-erpv[0]);
            R2[6] = -R2[2];
            R3[8] = 1.0;
            R3[0] = R3[4] = Math.Cos(gmst);
            R3[3] = Math.Sin(gmst);
            R3[1] = -R3[3];
            GlobalMembersRtkcmn.matmul("NN", 3, 1, 3, 1.0, R3, rs_tle, 0.0, ref rs_pef);
            GlobalMembersRtkcmn.matmul("NN", 3, 1, 3, 1.0, R3, rs_tle + 3, 0.0, ref rs_pef + 3);
            rs_pef[3] += DefineConstants.OMGE * rs_pef[1];
            rs_pef[4] -= DefineConstants.OMGE * rs_pef[0];
            GlobalMembersRtkcmn.matmul("NN", 3, 3, 3, 1.0, R1, R2, 0.0, ref W);
            GlobalMembersRtkcmn.matmul("NN", 3, 1, 3, 1.0, W, rs_pef, 0.0, ref rs);
            GlobalMembersRtkcmn.matmul("NN", 3, 1, 3, 1.0, W, rs_pef + 3, 0.0, ref rs + 3);
            return 1;
        }
    }
}
