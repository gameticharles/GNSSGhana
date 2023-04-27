using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ghGPS.Classes
{
    public static class GlobalMembersOptions
    {
        /*------------------------------------------------------------------------------
        * options.c : options functions
        *
        *          Copyright (C) 2010-2015 by T.TAKASU, All rights reserved.
        *
        * version : $Revision:$ $Date:$
        * history : 2010/07/20  1.1  moved from postpos.c
        *                            added api:
        *                                searchopt(),str2opt(),opt2str(),opt2buf(),
        *                                loadopts(),saveopts(),resetsysopts(),
        *                                getsysopts(),setsysopts()
        *           2010/09/11  1.2  add options
        *                                pos2-elmaskhold,pos1->snrmaskena
        *                                pos1-snrmask1,2,3
        *           2013/03/11  1.3  add pos1-posopt1,2,3,4,5,pos2-syncsol
        *                                misc-rnxopt1,2,pos1-snrmask_r,_b,_L1,_L2,_L5
        *           2014/10/21  1.4  add pos2-bdsarmode
        *           2015/02/20  1.4  add ppp-fixed as pos1-posmode option
        *-----------------------------------------------------------------------------*/

        internal const string rcsid = "$Id:$";

        /* system options buffer -----------------------------------------------------*/
        internal static prcopt_t prcopt_ = new prcopt_t();
        internal static solopt_t solopt_ = new solopt_t();
        internal static filopt_t filopt_ = new filopt_t();
        internal static int[] antpostype_ = new int[2];
        internal static double elmask_;
        internal static double elmaskar_;
        internal static double elmaskhold_;
        internal static double[,] antpos_ = new double[2, 3];
        internal static string exsats_ = new string(new char[1024]);
        internal static sbyte[,] snrmask_ = new sbyte[DefineConstants.NFREQ, 1024];


        public static opt_t[] sysopts = { new opt_t("pos1-posmode", 3, (object*)prcopt_.mode, DefineConstants.MODOPT), new opt_t("pos1-frequency", 3, (object*)prcopt_.nf, DefineConstants.FRQOPT), new opt_t("pos1-soltype", 3, (object*)prcopt_.soltype, DefineConstants.TYPOPT), new opt_t("pos1-elmask", 1, (object*)&elmask_, "deg"), new opt_t("pos1-snrmask_r", 3, (object*)prcopt_.snrmask.ena[0], DefineConstants.SWTOPT), new opt_t("pos1-snrmask_b", 3, (object*)prcopt_.snrmask.ena[1], DefineConstants.SWTOPT), new opt_t("pos1-snrmask_L1", 2, (object*)snrmask_[0], ""), new opt_t("pos1-snrmask_L2", 2, (object*)snrmask_[1], ""), new opt_t("pos1-snrmask_L5", 2, (object*)snrmask_[2], ""), new opt_t("pos1-dynamics", 3, (object*)prcopt_.dynamics, DefineConstants.SWTOPT), new opt_t("pos1-tidecorr", 3, (object*)prcopt_.tidecorr, DefineConstants.TIDEOPT), new opt_t("pos1-ionoopt", 3, (object*)prcopt_.ionoopt, DefineConstants.IONOPT), new opt_t("pos1-tropopt", 3, (object*)prcopt_.tropopt, DefineConstants.TRPOPT), new opt_t("pos1-sateph", 3, (object*)prcopt_.sateph, DefineConstants.EPHOPT), new opt_t("pos1-posopt1", 3, (object*)prcopt_.posopt[0], DefineConstants.SWTOPT), new opt_t("pos1-posopt2", 3, (object*)prcopt_.posopt[1], DefineConstants.SWTOPT), new opt_t("pos1-posopt3", 3, (object*)prcopt_.posopt[2], DefineConstants.SWTOPT), new opt_t("pos1-posopt4", 3, (object*)prcopt_.posopt[3], DefineConstants.SWTOPT), new opt_t("pos1-posopt5", 3, (object*)prcopt_.posopt[4], DefineConstants.SWTOPT), new opt_t("pos1-exclsats", 2, (object*)exsats_, "prn ..."), new opt_t("pos1-navsys", 0, (object*)prcopt_.navsys, DefineConstants.NAVOPT), new opt_t("pos2-armode", 3, (object*)prcopt_.modear, DefineConstants.ARMOPT), new opt_t("pos2-gloarmode", 3, (object*)prcopt_.glomodear, DefineConstants.GAROPT), new opt_t("pos2-bdsarmode", 3, (object*)prcopt_.bdsmodear, DefineConstants.SWTOPT), new opt_t("pos2-arthres", 1, (object*)prcopt_.thresar[0], ""), new opt_t("pos2-arlockcnt", 0, (object*)prcopt_.minlock, ""), new opt_t("pos2-arelmask", 1, (object*)&elmaskar_, "deg"), new opt_t("pos2-arminfix", 0, (object*)prcopt_.minfix, ""), new opt_t("pos2-elmaskhold", 1, (object*)&elmaskhold_, "deg"), new opt_t("pos2-aroutcnt", 0, (object*)prcopt_.maxout, ""), new opt_t("pos2-maxage", 1, (object*)prcopt_.maxtdiff, "s"), new opt_t("pos2-syncsol", 3, (object*)prcopt_.syncsol, DefineConstants.SWTOPT), new opt_t("pos2-slipthres", 1, (object*)prcopt_.thresslip, "m"), new opt_t("pos2-rejionno", 1, (object*)prcopt_.maxinno, "m"), new opt_t("pos2-rejgdop", 1, (object*)prcopt_.maxgdop, ""), new opt_t("pos2-niter", 0, (object*)prcopt_.niter, ""), new opt_t("pos2-baselen", 1, (object*)prcopt_.baseline[0], "m"), new opt_t("pos2-basesig", 1, (object*)prcopt_.baseline[1], "m"), new opt_t("out-solformat", 3, (object*)solopt_.posf, DefineConstants.SOLOPT), new opt_t("out-outhead", 3, (object*)solopt_.outhead, DefineConstants.SWTOPT), new opt_t("out-outopt", 3, (object*)solopt_.outopt, DefineConstants.SWTOPT), new opt_t("out-timesys", 3, (object*)solopt_.times, DefineConstants.TSYOPT), new opt_t("out-timeform", 3, (object*)solopt_.timef, DefineConstants.TFTOPT), new opt_t("out-timendec", 0, (object*)solopt_.timeu, ""), new opt_t("out-degform", 3, (object*)solopt_.degf, DefineConstants.DFTOPT), new opt_t("out-fieldsep", 2, (object*)solopt_.sep, ""), new opt_t("out-height", 3, (object*)solopt_.height, DefineConstants.HGTOPT), new opt_t("out-geoid", 3, (object*)solopt_.geoid, DefineConstants.GEOOPT), new opt_t("out-solstatic", 3, (object*)solopt_.solstatic, DefineConstants.STAOPT), new opt_t("out-nmeaintv1", 1, (object*)solopt_.nmeaintv[0], "s"), new opt_t("out-nmeaintv2", 1, (object*)solopt_.nmeaintv[1], "s"), new opt_t("out-outstat", 3, (object*)solopt_.sstat, DefineConstants.STSOPT), new opt_t("stats-eratio1", 1, (object*)prcopt_.eratio[0], ""), new opt_t("stats-eratio2", 1, (object*)prcopt_.eratio[1], ""), new opt_t("stats-errphase", 1, (object*)prcopt_.err[1], "m"), new opt_t("stats-errphaseel", 1, (object*)prcopt_.err[2], "m"), new opt_t("stats-errphasebl", 1, (object*)prcopt_.err[3], "m/10km"), new opt_t("stats-errdoppler", 1, (object*)prcopt_.err[4], "Hz"), new opt_t("stats-stdbias", 1, (object*)prcopt_.std[0], "m"), new opt_t("stats-stdiono", 1, (object*)prcopt_.std[1], "m"), new opt_t("stats-stdtrop", 1, (object*)prcopt_.std[2], "m"), new opt_t("stats-prnaccelh", 1, (object*)prcopt_.prn[3], "m/s^2"), new opt_t("stats-prnaccelv", 1, (object*)prcopt_.prn[4], "m/s^2"), new opt_t("stats-prnbias", 1, (object*)prcopt_.prn[0], "m"), new opt_t("stats-prniono", 1, (object*)prcopt_.prn[1], "m"), new opt_t("stats-prntrop", 1, (object*)prcopt_.prn[2], "m"), new opt_t("stats-clkstab", 1, (object*)prcopt_.sclkstab, "s/s"), new opt_t("ant1-postype", 3, (object*)antpostype_[0], DefineConstants.POSOPT), new opt_t("ant1-pos1", 1, (object*)antpos_[0, 0], "deg|m"), new opt_t("ant1-pos2", 1, (object*)antpos_[0, 1], "deg|m"), new opt_t("ant1-pos3", 1, (object*)antpos_[0, 2], "m|m"), new opt_t("ant1-anttype", 2, (object*)prcopt_.anttype[0], ""), new opt_t("ant1-antdele", 1, (object*)prcopt_.antdel[0, 0], "m"), new opt_t("ant1-antdeln", 1, (object*)prcopt_.antdel[0, 1], "m"), new opt_t("ant1-antdelu", 1, (object*)prcopt_.antdel[0, 2], "m"), new opt_t("ant2-postype", 3, (object*)antpostype_[1], DefineConstants.POSOPT), new opt_t("ant2-pos1", 1, (object*)antpos_[1, 0], "deg|m"), new opt_t("ant2-pos2", 1, (object*)antpos_[1, 1], "deg|m"), new opt_t("ant2-pos3", 1, (object*)antpos_[1, 2], "m|m"), new opt_t("ant2-anttype", 2, (object*)prcopt_.anttype[1], ""), new opt_t("ant2-antdele", 1, (object*)prcopt_.antdel[1, 0], "m"), new opt_t("ant2-antdeln", 1, (object*)prcopt_.antdel[1, 1], "m"), new opt_t("ant2-antdelu", 1, (object*)prcopt_.antdel[1, 2], "m"), new opt_t("misc-timeinterp", 3, (object*)prcopt_.intpref, DefineConstants.SWTOPT), new opt_t("misc-sbasatsel", 0, (object*)prcopt_.sbassatsel, "0:all"), new opt_t("misc-rnxopt1", 2, (object*)prcopt_.rnxopt[0], ""), new opt_t("misc-rnxopt2", 2, (object*)prcopt_.rnxopt[1], ""), new opt_t("file-satantfile", 2, (object*)filopt_.satantp, ""), new opt_t("file-rcvantfile", 2, (object*)filopt_.rcvantp, ""), new opt_t("file-staposfile", 2, (object*)filopt_.stapos, ""), new opt_t("file-geoidfile", 2, (object*)filopt_.geoid, ""), new opt_t("file-ionofile", 2, (object*)filopt_.iono, ""), new opt_t("file-dcbfile", 2, (object*)filopt_.dcb, ""), new opt_t("file-eopfile", 2, (object*)filopt_.eop, ""), new opt_t("file-blqfile", 2, (object*)filopt_.blq, ""), new opt_t("file-tempdir", 2, (object*)filopt_.tempdir, ""), new opt_t("file-geexefile", 2, (object*)filopt_.geexe, ""), new opt_t("file-solstatfile", 2, (object*)filopt_.solstat, ""), new opt_t("file-tracefile", 2, (object*)filopt_.trace, ""), new opt_t("", 0, null, "") };
        /* discard space characters at tail ------------------------------------------*/
        internal static void chop(ref string str)
        {
            //C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on this variable, so pointers on this variable are left unchanged:
            sbyte* p;
            if ((p = StringFunctions.StrChr(str, '#')) != 0) // comment
            {
                *p = (sbyte)'\0';
            }
            for (p = str.Substring(str.Length) - 1; p >= str && !isgraph((int)*p); p--)
            {
                *p = (sbyte)'\0';
            }
        }
        /* enum to string ------------------------------------------------------------*/
        internal static int enum2str(ref string s, string comment, int val)
        {
            string str = new string(new char[32]);
            string p;
            string q;
            int n;

            str = string.Format("{0:D}:", val);
            n = str.Length;
            if ((p = StringFunctions.StrStr(comment, str)) == 0)
            {
                return sprintf(s, "%d", val);
            }
            if ((q = StringFunctions.StrChr(p.Substring(n), ',')) == 0 && (q = StringFunctions.StrChr(p.Substring(n), ')')) == 0)
            {
                s = p.Substring(n);
                return p.Substring(n).Length;
            }
            s = p.Substring(n, q - p - n);
            s[q - p - n] = '\0';
            return q - p - n;
        }
        /* string to enum ------------------------------------------------------------*/
        internal static int str2enum(string str, string comment, ref int val)
        {
            //C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on this variable, so pointers on this variable are left unchanged:
            sbyte* p;
            string s = new string(new char[32]);

            for (p = comment; ; p++)
            {
                if ((p = StringFunctions.StrStr(p, str)) == 0)
                    break;
                if (*(p - 1) != ':')
                    continue;
                for (p - = 2; '0' <= *p && *p <= '9'; p--)
                {
                    ;
                }
                return sscanf(p + 1, "%d", val) == 1;
            }
            //C++ TO C# CONVERTER TODO TASK: C# does not allow setting maximum string width in format specifiers:
            //ORIGINAL LINE: sprintf(s,"%30.30s:",str);
            s = string.Format("{0,30}:", str);
            if ((p = StringFunctions.StrStr(comment, s)) != 0) // number
            {
                return sscanf(p, "%d", val) == 1;
            }
            return 0;
        }
        /* search option ---------------------------------------------------------------
        * search option record
        * args   : char   *name     I  option name
        *          opt_t  *opts     I  options table
        *                              (terminated with table[i].name="")
        * return : option record (NULL: not found)
        *-----------------------------------------------------------------------------*/
        public static opt_t searchopt(string name, opt_t[] opts)
        {
            int i;

            GlobalMembersRtkcmn.trace(3, "searchopt: name=%s\n", name);

            for (i = 0; *opts[i].name; i++)
            {
                if (StringFunctions.StrStr(opts[i].name, name))
                {
                    return (opt_t)(opts + i);
                }
            }
            return null;
        }
        /* string to option value ------------------------------------------------------
        * convert string to option value
        * args   : opt_t  *opt      O  option
        *          char   *str      I  option value string
        * return : status (1:ok,0:error)
        *-----------------------------------------------------------------------------*/
        public static int str2opt(opt_t opt, string str)
        {
            switch (opt.format)
            {
                case 0:
                    (int)opt.@var = Convert.ToInt32(str);
                    break;
                case 1:
                    (double)opt.@var = Convert.ToDouble(str);
                    break;
                case 2:
                    (string)opt.@var = str;
                    break;
                case 3:
                    return GlobalMembersOptions.str2enum(str, opt.comment, ref (int)opt.@var);
                default:
                    return 0;
            }
            return 1;
        }
        /* option value to string ------------------------------------------------------
        * convert option value to string
        * args   : opt_t  *opt      I  option
        *          char   *str      O  option value string
        * return : length of output string
        *-----------------------------------------------------------------------------*/
        public static int opt2str(opt_t opt, ref string str)
        {
            //C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on this variable, so pointers on this variable are left unchanged:
            sbyte* p = str;

            GlobalMembersRtkcmn.trace(3, "opt2str : name=%s\n", opt.name);

            switch (opt.format)
            {
                case 0:
                    p += sprintf(p, "%d", (int)opt.@var);
                    break;
                case 1:
                    p += sprintf(p, "%.15g", (double)opt.@var);
                    break;
                case 2:
                    p += sprintf(p, "%s", (string)opt.@var);
                    break;
                case 3:
                    p += GlobalMembersOptions.enum2str(ref p, opt.comment, (int)opt.@var);
                    break;
            }
            return (int)(p - str);
        }
        /* option to string -------------------------------------------------------------
        * convert option to string (keyword=value # comment)
        * args   : opt_t  *opt      I  option
        *          char   *buff     O  option string
        * return : length of output string
        *-----------------------------------------------------------------------------*/
        public static int opt2buf(opt_t opt, ref string buff)
        {
            //C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on this variable, so pointers on this variable are left unchanged:
            sbyte* p = buff;
            int n;

            GlobalMembersRtkcmn.trace(3, "opt2buf : name=%s\n", opt.name);

            p += sprintf(p, "%-18s =", opt.name);
            p += GlobalMembersOptions.opt2str(opt, ref p);
            if (opt.comment != 0)
            {
                if ((n = (int)(buff.Substring(30) - p)) > 0)
                {
                    p += sprintf(p, "%*s", n, "");
                }
                p += sprintf(p, " # (%s)", opt.comment);
            }
            return (int)(p - buff);
        }
        /* load options ----------------------------------------------------------------
        * load options from file
        * args   : char   *file     I  options file
        *          opt_t  *opts     IO options table
        *                              (terminated with table[i].name="")
        * return : status (1:ok,0:error)
        *-----------------------------------------------------------------------------*/
        public static int loadopts(string file, opt_t opts)
        {
            FILE fp;
            opt_t opt;
            string buff = new string(new char[2048]);
            string p;
            int n = 0;

            GlobalMembersRtkcmn.trace(3, "loadopts: file=%s\n", file);

            if ((fp = fopen(file, "r")) == null)
            {
                GlobalMembersRtkcmn.trace(1, "loadopts: options file open error (%s)\n", file);
                return 0;
            }
            while (fgets(buff, sizeof(sbyte), fp))
            {
                n++;
                GlobalMembersOptions.chop(ref buff);

                if (buff[0] == '\0')
                    continue;

                if ((p = StringFunctions.StrStr(buff, "=")) == 0)
                {
                    fprintf(stderr, "invalid option %s (%s:%d)\n", buff, file, n);
                    continue;
                }
                p++ = '\0';
                GlobalMembersOptions.chop(ref buff);
                if ((opt = GlobalMembersOptions.searchopt(buff, opts)) == null)
                    continue;

                if (GlobalMembersOptions.str2opt(opt, p) == 0)
                {
                    fprintf(stderr, "invalid option value %s (%s:%d)\n", buff, file, n);
                    continue;
                }
            }
            fclose(fp);

            return 1;
        }
        /* save options to file --------------------------------------------------------
        * save options to file
        * args   : char   *file     I  options file
        *          char   *mode     I  write mode ("w":overwrite,"a":append);
        *          char   *comment  I  header comment (NULL: no comment)
        *          opt_t  *opts     I  options table
        *                              (terminated with table[i].name="")
        * return : status (1:ok,0:error)
        *-----------------------------------------------------------------------------*/
        public static int saveopts(string file, string mode, string comment, opt_t[] opts)
        {
            FILE fp;
            string buff = new string(new char[2048]);
            int i;

            GlobalMembersRtkcmn.trace(3, "saveopts: file=%s mode=%s\n", file, mode);

            if ((fp = fopen(file, mode)) == null)
            {
                GlobalMembersRtkcmn.trace(1, "saveopts: options file open error (%s)\n", file);
                return 0;
            }
            if (comment != 0)
            {
                fprintf(fp, "# %s\n\n", comment);
            }

            for (i = 0; *opts[i].name; i++)
            {
                GlobalMembersOptions.opt2buf(opts + i, ref buff);
                fprintf(fp, "%s\n", buff);
            }
            fclose(fp);
            return 1;
        }
        /* system options buffer to options ------------------------------------------*/
        internal static void buff2sysopts()
        {
            double[] pos = new double[3];
            double[] rr;
            string buff = new string(new char[1024]);
            string p;
            string id;
            int i;
            int j;
            int sat;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: int *ps;
            int ps;

            prcopt_.elmin = elmask_ * DefineConstants.PI / 180.0;
            prcopt_.elmaskar = elmaskar_ * DefineConstants.PI / 180.0;
            prcopt_.elmaskhold = elmaskhold_ * DefineConstants.PI / 180.0;

            for (i = 0; i < 2; i++)
            {
                ps = i == 0 ? prcopt_.rovpos : prcopt_.refpos;
                rr = i == 0 ? prcopt_.ru : prcopt_.rb;

                if (antpostype_[i] == 0) // lat/lon/hgt
                {
                    ps = 0;
                    pos[0] = antpos_[i, 0] * DefineConstants.PI / 180.0;
                    pos[1] = antpos_[i, 1] * DefineConstants.PI / 180.0;
                    pos[2] = antpos_[i, 2];
                    GlobalMembersRtkcmn.pos2ecef(pos, rr);
                }
                else if (antpostype_[i] == 1) // xyz-ecef
                {
                    ps = 0;
                    rr[0] = antpos_[i, 0];
                    rr[1] = antpos_[i, 1];
                    rr[2] = antpos_[i, 2];
                }
                else
                {
                    ps = antpostype_[i] - 1;
                }
            }
            /* excluded satellites */
            for (i = 0; i < DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1; i++)
            {
                prcopt_.exsats[i] = 0;
            }
            if (exsats_[0] != '\0')
            {
                buff = exsats_;
                for (p = StringFunctions.StrTok(buff, " "); p != 0; p = StringFunctions.StrTok(null, " "))
                {
                    if (p == '+')
                    {
                        id = p.Substring(1);
                    }
                    else
                    {
                        id = p;
                    }
                    if ((sat = GlobalMembersRtkcmn.satid2no(id)) == 0)
                        continue;
                    prcopt_.exsats[sat - 1] = p == '+' ? 2 : 1;
                }
            }
            /* snrmask */
            for (i = 0; i < DefineConstants.NFREQ; i++)
            {
                for (j = 0; j < 9; j++)
                {
                    prcopt_.snrmask.mask[i, j] = 0.0;
                }
                buff = snrmask_[i];
                for (p = StringFunctions.StrTok(buff, ","), j = 0; p && j < 9; p = StringFunctions.StrTok(null, ","))
                {
                    prcopt_.snrmask.mask[i, j++] = Convert.ToDouble(p);
                }
            }
        }
        /* options to system options buffer ------------------------------------------*/
        internal static void sysopts2buff()
        {
            double[] pos = new double[3];
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: double *rr;
            double rr;
            string id = new string(new char[32]);
            string p;
            int i;
            int j;
            int sat;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: int *ps;
            int ps;

            elmask_ = prcopt_.elmin * 180.0 / DefineConstants.PI;
            elmaskar_ = prcopt_.elmaskar * 180.0 / DefineConstants.PI;
            elmaskhold_ = prcopt_.elmaskhold * 180.0 / DefineConstants.PI;

            for (i = 0; i < 2; i++)
            {
                ps = i == 0 ? prcopt_.rovpos : prcopt_.refpos;
                rr = i == 0 ? prcopt_.ru : prcopt_.rb;

                if (ps == 0)
                {
                    antpostype_[i] = 0;
                    GlobalMembersRtkcmn.ecef2pos(rr, pos);
                    antpos_[i, 0] = pos[0] * 180.0 / DefineConstants.PI;
                    antpos_[i, 1] = pos[1] * 180.0 / DefineConstants.PI;
                    antpos_[i, 2] = pos[2];
                }
                else
                {
                    antpostype_[i] = ps + 1;
                }
            }
            /* excluded satellites */
            exsats_[0] = '\0';
            for (sat = 1, p = exsats_; sat <= DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1 && p - exsats_ < (int)sizeof(sbyte) - 32; sat++)
            {
                if (prcopt_.exsats[sat - 1] != 0)
                {
                    GlobalMembersRtkcmn.satno2id(sat, ref id);
                    p += sprintf(p, "%s%s%s", p == exsats_ != 0 ? "" : " ", prcopt_.exsats[sat - 1] == 2 ? "+" : "", id);
                }
            }
            /* snrmask */
            for (i = 0; i < DefineConstants.NFREQ; i++)
            {
                snrmask_[i, 0] = (sbyte)'\0';
                p = snrmask_[i];
                for (j = 0; j < 9; j++)
                {
                    p += sprintf(p, "%s%.0f", j > 0 ? "," : "", prcopt_.snrmask.mask[i, j]);
                }
            }
        }
        /* reset system options to default ---------------------------------------------
        * reset system options to default
        * args   : none
        * return : none
        *-----------------------------------------------------------------------------*/
        public static void resetsysopts()
        {
            int i;
            int j;

            GlobalMembersRtkcmn.trace(3, "resetsysopts:\n");

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: prcopt_=prcopt_default;
            prcopt_.CopyFrom(GlobalMembersRtkcmn.prcopt_default);
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: solopt_=solopt_default;
            solopt_.CopyFrom(GlobalMembersRtkcmn.solopt_default);
            filopt_.satantp[0] = '\0';
            filopt_.rcvantp[0] = '\0';
            filopt_.stapos[0] = '\0';
            filopt_.geoid[0] = '\0';
            filopt_.dcb[0] = '\0';
            filopt_.blq[0] = '\0';
            filopt_.solstat[0] = '\0';
            filopt_.trace[0] = '\0';
            for (i = 0; i < 2; i++)
            {
                antpostype_[i] = 0;
            }
            elmask_ = 15.0;
            elmaskar_ = 0.0;
            elmaskhold_ = 0.0;
            for (i = 0; i < 2; i++)
            {
                for (j = 0; j < 3; j++)
                {
                    antpos_[i, j] = 0.0;
                }
            }
            exsats_[0] = '\0';
        }
        /* get system options ----------------------------------------------------------
        * get system options
        * args   : prcopt_t *popt   IO processing options (NULL: no output)
        *          solopt_t *sopt   IO solution options   (NULL: no output)
        *          folopt_t *fopt   IO file options       (NULL: no output)
        * return : none
        * notes  : to load system options, use loadopts() before calling the function
        *-----------------------------------------------------------------------------*/
        public static void getsysopts(prcopt_t popt, solopt_t sopt, filopt_t fopt)
        {
            GlobalMembersRtkcmn.trace(3, "getsysopts:\n");

            GlobalMembersOptions.buff2sysopts();
            if (popt != null)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                //ORIGINAL LINE: *popt=prcopt_;
                popt.CopyFrom(prcopt_);
            }
            if (sopt != null)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                //ORIGINAL LINE: *sopt=solopt_;
                sopt.CopyFrom(solopt_);
            }
            if (fopt != null)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                //ORIGINAL LINE: *fopt=filopt_;
                fopt.CopyFrom(filopt_);
            }
        }
        /* set system options ----------------------------------------------------------
        * set system options
        * args   : prcopt_t *prcopt I  processing options (NULL: default)
        *          solopt_t *solopt I  solution options   (NULL: default)
        *          filopt_t *filopt I  file options       (NULL: default)
        * return : none
        * notes  : to save system options, use saveopts() after calling the function
        *-----------------------------------------------------------------------------*/
        public static void setsysopts(prcopt_t prcopt, solopt_t solopt, filopt_t filopt)
        {
            GlobalMembersRtkcmn.trace(3, "setsysopts:\n");

            GlobalMembersOptions.resetsysopts();
            if (prcopt != null)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                //ORIGINAL LINE: prcopt_=*prcopt;
                prcopt_.CopyFrom(prcopt);
            }
            if (solopt != null)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                //ORIGINAL LINE: solopt_=*solopt;
                solopt_.CopyFrom(solopt);
            }
            if (filopt != null)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                //ORIGINAL LINE: filopt_=*filopt;
                filopt_.CopyFrom(filopt);
            }
            GlobalMembersOptions.sysopts2buff();
        }
    }

}
