using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ghGPS.Classes
{
    public static class GlobalMembersRtklib
    {

        /* global variables ----------------------------------------------------------*/
        //C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
        //extern const double chisqr[]; // chi-sqr(n) table (alpha=0.001)
        //C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
        //extern const double lam_carr[]; // carrier wave length (m) {L1,L2,...}
        //C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
        //extern const prcopt_t prcopt_default; // default positioning options
        //C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
        //extern const solopt_t solopt_default; // default solution output options
        //C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
        //extern const sbsigpband_t igpband1[][8]; // SBAS IGP band 0-8
        //C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
        //extern const sbsigpband_t igpband2[][5]; // SBAS IGP band 9-10
        //C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
        //extern const sbyte *formatstrs[]; // stream format strings
        //C++ TO C# CONVERTER NOTE: 'extern' variable declarations are not required in C#:
        //extern opt_t sysopts[]; // system options table

        /* satellites, systems, codes functions --------------------------------------*/
        //int satno(int sys, int prn);
        //int satsys(int sat, ref int prn);
        //int satid2no(string id);
        //void satno2id(int sat, ref string id);
        //byte obs2code(string obs, ref int freq);
        //string code2obs(byte code, ref int freq);
        //int satexclude(int sat, int svh, prcopt_t opt);
        //int testsnr(int @base, int freq, double el, double snr, snrmask_t mask);
        //void setcodepri(int sys, int freq, string pri);
        //int getcodepri(int sys, byte code, string opt);

        /* matrix and vector functions -----------------------------------------------*/
        //C++ TO C# CONVERTER WARNING: C# has no equivalent to methods returning pointers to value types:
        //ORIGINAL LINE: extern double *mat(int n, int m);
        //double mat(int n, int m);
        //C++ TO C# CONVERTER WARNING: C# has no equivalent to methods returning pointers to value types:
        //ORIGINAL LINE: extern int *imat(int n, int m);
        //int imat(int n, int m);
        //C++ TO C# CONVERTER WARNING: C# has no equivalent to methods returning pointers to value types:
        //ORIGINAL LINE: extern double *zeros(int n, int m);
        //double zeros(int n, int m);
        //C++ TO C# CONVERTER WARNING: C# has no equivalent to methods returning pointers to value types:
        //ORIGINAL LINE: extern double *eye(int n);
        //double eye(int n);
        //double dot(double a, double b, int n);
        //double norm(double a, int n);
        //void cross3(double a, double b, ref double c);
        //int normv3(double a, ref double b);
        //void matcpy(ref double A, double B, int n, int m);
        //void matmul(string tr, int n, int k, int m, double alpha, double A, double B, double beta, ref double C);
        //int matinv(ref double A, int n);
        //int solve(string tr, double A, double Y, int n, int m, ref double X);
        //int lsq(double A, double y, int n, int m, ref double x, ref double Q);
        //int filter(ref double x, ref double P, double H, double v, double R, int n, int m);
        //int smoother(double xf, double Qf, double xb, double Qb, int n, ref double xs, ref double Qs);
        //void matprint(double A, int n, int m, int p, int q);
        //void matfprint(double A, int n, int m, int p, int q, FILE fp);

        /* time and string functions -------------------------------------------------*/
        //double str2num(string s, int i, int n);
        //int str2time(string s, int i, int n, gtime_t t);
        //void time2str(gtime_t t, ref string str, int n);
        //gtime_t epoch2time(double ep);
        //void time2epoch(gtime_t t, ref double ep);
        //gtime_t gpst2time(int week, double sec);
        //double time2gpst(gtime_t t, ref int week);
        //gtime_t gst2time(int week, double sec);
        //double time2gst(gtime_t t, ref int week);
        //gtime_t bdt2time(int week, double sec);
        //double time2bdt(gtime_t t, ref int week);
        //string time_str(gtime_t t, int n);

        //gtime_t timeadd(gtime_t t, double sec);
        //double timediff(gtime_t t1, gtime_t t2);
        //gtime_t gpst2utc(gtime_t t);
        //gtime_t utc2gpst(gtime_t t);
        //gtime_t gpst2bdt(gtime_t t);
        //gtime_t bdt2gpst(gtime_t t);
        //gtime_t timeget();
        //void timeset(gtime_t t);
        //double time2doy(gtime_t t);
        //double utc2gmst(gtime_t t, double ut1_utc);
        //int read_leaps(string file);

        //int adjgpsweek(int week);
        //uint tickget();
        //void sleepms(int ms);

        //int reppath(string path, ref string rpath, gtime_t time, string rov, string @base);
        //int reppaths(string path, string[] rpaths, int nmax, gtime_t ts, gtime_t te, string rov, string @base);

        /* coordinates transformation ------------------------------------------------*/
        //void ecef2pos(double r, ref double pos);
        //void pos2ecef(double pos, ref double r);
        //void ecef2enu(double pos, double r, ref double e);
        //void enu2ecef(double pos, double e, ref double r);
        //void covenu(double pos, double P, ref double Q);
        //void covecef(double pos, double Q, ref double P);
        //void xyz2enu(double pos, ref double E);
        //void eci2ecef(gtime_t tutc, double erpv, ref double U, ref double gmst);
        //void deg2dms(double deg, ref double dms);
        //double dms2deg(double dms);

        /* input and output functions ------------------------------------------------*/
        //void readpos(string file, string rcv, ref double pos);
        //int sortobs(obs_t obs);
        //void uniqnav(nav_t nav);
        //int screent(gtime_t time, gtime_t ts, gtime_t te, double tint);
        //int readnav(string file, nav_t nav);
        //int savenav(string file, nav_t nav);
        //void freeobs(obs_t obs);
        //void freenav(nav_t nav, int opt);
        //int readblq(string file, string sta, ref double odisp);
        //int readerp(string file, erp_t erp);
        //int geterp(erp_t erp, gtime_t time, ref double val);

        /* debug trace functions -----------------------------------------------------*/
        //void traceopen(string file);
        //void traceclose();
        //void tracelevel(int level);
        //void trace(int level, string format, params object[] LegacyParamArray);
        //void tracet(int level, string format, params object[] LegacyParamArray);
        //void tracemat(int level, double A, int n, int m, int p, int q);
        //void traceobs(int level, obsd_t obs, int n);
        //void tracenav(int level, nav_t nav);
        //void tracegnav(int level, nav_t nav);
        //void tracehnav(int level, nav_t nav);
        //void tracepeph(int level, nav_t nav);
        //void tracepclk(int level, nav_t nav);
        //void traceb(int level, byte p, int n);

        /* platform dependent functions ----------------------------------------------*/
        //int execcmd(string cmd);
        //int expath(string path, string[] paths, int nmax);
        //void createdir(string path);

        /* positioning models --------------------------------------------------------*/
        //double satwavelen(int sat, int frq, nav_t nav);
        //double satazel(double pos, double e, ref double azel);
        //double geodist(double rs, double rr, ref double e);
        //void dops(int ns, double azel, double elmin, ref double dop);
        //void csmooth(obs_t obs, int ns);

        /* atmosphere models ---------------------------------------------------------*/
        //double ionmodel(gtime_t t, double ion, double pos, double azel);
        //double ionmapf(double pos, double azel);
        //double ionppp(double pos, double azel, double re, double hion, ref double pppos);
        //double tropmodel(gtime_t time, double pos, double azel, double humi);
        //double tropmapf(gtime_t time, double pos, double azel, ref double mapfw);
        //int iontec(gtime_t time, nav_t nav, double pos, double azel, int opt, ref double delay, ref double @var);
        //void readtec(string file, nav_t nav, int opt);
        //int ionocorr(gtime_t time, nav_t nav, int sat, double pos, double azel, int ionoopt, ref double ion, ref double @var);
        //int tropcorr(gtime_t time, nav_t nav, double pos, double azel, int tropopt, ref double trp, ref double @var);
        //void stec_read(string file, nav_t nav);
        //int stec_grid(nav_t nav, double pos, int nmax, ref int index, ref double dist);
        //int stec_data(stec_t stec, gtime_t time, int sat, ref double iono, ref double rate, ref double rms, ref int slip);
        //int stec_ion(gtime_t time, nav_t nav, int sat, double pos, double azel, ref double iono, ref double rate, ref double @var, ref int brk);
        //void stec_free(nav_t nav);

        /* antenna models ------------------------------------------------------------*/
        //int readpcv(string file, pcvs_t pcvs);
        //pcv_t searchpcv(int sat, string type, gtime_t time, pcvs_t pcvs);
        //void antmodel(pcv_t pcv, double del, double azel, int opt, ref double dant);
        //void antmodel_s(pcv_t pcv, double nadir, ref double dant);

        /* earth tide models ---------------------------------------------------------*/
        //void sunmoonpos(gtime_t tutc, double erpv, ref double rsun, ref double rmoon, ref double gmst);
        //void tidedisp(gtime_t tutc, double rr, int opt, erp_t erp, double odisp, ref double dr);

        /* geiod models --------------------------------------------------------------*/
        //int opengeoid(int model, string file);
        //void closegeoid();
        //double geoidh(double pos);

        /* datum transformation ------------------------------------------------------*/
        //int loaddatump(string file);
        //int tokyo2jgd(ref double pos);
        //int jgd2tokyo(ref double pos);

        /* rinex functions -----------------------------------------------------------*/
        //int readrnx(string file, int rcv, string opt, obs_t obs, nav_t nav, sta_t sta);
        //int readrnxt(string file, int rcv, gtime_t ts, gtime_t te, double tint, string opt, obs_t obs, nav_t nav, sta_t sta);
        //int readrnxc(string file, nav_t nav);
        //int outrnxobsh(FILE fp, rnxopt_t opt, nav_t nav);
        //int outrnxobsb(FILE fp, rnxopt_t opt, obsd_t obs, int n, int epflag);
        //int outrnxnavh(FILE fp, rnxopt_t opt, nav_t nav);
        //int outrnxgnavh(FILE fp, rnxopt_t opt, nav_t nav);
        //int outrnxhnavh(FILE fp, rnxopt_t opt, nav_t nav);
        //int outrnxlnavh(FILE fp, rnxopt_t opt, nav_t nav);
        //int outrnxqnavh(FILE fp, rnxopt_t opt, nav_t nav);
        //int outrnxcnavh(FILE fp, rnxopt_t opt, nav_t nav);
        //int outrnxnavb(FILE fp, rnxopt_t opt, eph_t eph);
        //int outrnxgnavb(FILE fp, rnxopt_t opt, geph_t geph);
        //int outrnxhnavb(FILE fp, rnxopt_t opt, seph_t seph);
        //int uncompress(string file, ref string uncfile);
        //int convrnx(int format, rnxopt_t opt, string file, string[] ofile);
        //int init_rnxctr(rnxctr_t rnx);
        //void free_rnxctr(rnxctr_t rnx);
        //int open_rnxctr(rnxctr_t rnx, FILE fp);
        //int input_rnxctr(rnxctr_t rnx, FILE fp);

        /* ephemeris and clock functions ---------------------------------------------*/
        //double eph2clk(gtime_t time, eph_t eph);
        //double geph2clk(gtime_t time, geph_t geph);
        //double seph2clk(gtime_t time, seph_t seph);
        //void eph2pos(gtime_t time, eph_t eph, ref double rs, ref double dts, ref double @var);
        //void geph2pos(gtime_t time, geph_t geph, ref double rs, ref double dts, ref double @var);
        //void seph2pos(gtime_t time, seph_t seph, ref double rs, ref double dts, ref double @var);
        //int peph2pos(gtime_t time, int sat, nav_t nav, int opt, ref double rs, ref double dts, ref double @var);
        //void satantoff(gtime_t time, double rs, int sat, nav_t nav, ref double dant);
        //int satpos(gtime_t time, gtime_t teph, int sat, int ephopt, nav_t nav, ref double rs, ref double dts, ref double @var, ref int svh);
        //void satposs(gtime_t time, obsd_t obs, int n, nav_t nav, int sateph, ref double rs, ref double dts, ref double @var, ref int svh);
        //void readsp3(string file, nav_t nav, int opt);
        //int readsap(string file, gtime_t time, nav_t nav);
        //int readdcb(string file, nav_t nav);
        //void alm2pos(gtime_t time, alm_t alm, ref double rs, ref double dts);

        //int tle_read(string file, tle_t tle);
        //int tle_name_read(string file, tle_t tle);
        //int tle_pos(gtime_t time, string name, string satno, string desig, tle_t tle, erp_t erp, ref double rs);

        /* receiver raw data functions -----------------------------------------------*/
        //uint getbitu(byte buff, int pos, int len);
        //int getbits(byte buff, int pos, int len);
        //void setbitu(ref byte buff, int pos, int len, uint data);
        //void setbits(ref byte buff, int pos, int len, int data);
        //uint crc32(byte buff, int len);
        //uint crc24q(byte buff, int len);
        //ushort crc16(byte buff, int len);
        //int decode_word(uint word, ref byte data);
        //int decode_frame(byte buff, eph_t eph, alm_t alm, ref double ion, ref double utc, ref int leaps);
        //int test_glostr(byte buff);
        //int decode_glostr(byte buff, geph_t geph);
        //int decode_bds_d1(byte buff, eph_t eph);
        //int decode_bds_d2(byte buff, eph_t eph);

        //int init_raw(raw_t raw);
        //void free_raw(raw_t raw);
        //int input_raw(raw_t raw, int format, byte data);
        //int input_rawf(raw_t raw, int format, FILE fp);

        //int input_oem4(raw_t raw, byte data);
        //int input_oem3(raw_t raw, byte data);
        //int input_ubx(raw_t raw, byte data);
        //int input_ss2(raw_t raw, byte data);
        //int input_cres(raw_t raw, byte data);
        //int input_stq(raw_t raw, byte data);
        //int input_gw10(raw_t raw, byte data);
        //int input_javad(raw_t raw, byte data);
        //int input_nvs(raw_t raw, byte data);
        //int input_bnx(raw_t raw, byte data);
        //int input_rt17(raw_t raw, byte data);
        //int input_lexr(raw_t raw, byte data);
        //int input_oem4f(raw_t raw, FILE fp);
        //int input_oem3f(raw_t raw, FILE fp);
        //int input_ubxf(raw_t raw, FILE fp);
        //int input_ss2f(raw_t raw, FILE fp);
        //int input_cresf(raw_t raw, FILE fp);
        //int input_stqf(raw_t raw, FILE fp);
        //int input_gw10f(raw_t raw, FILE fp);
        //int input_javadf(raw_t raw, FILE fp);
        //int input_nvsf(raw_t raw, FILE fp);
        //int input_bnxf(raw_t raw, FILE fp);
        //int input_rt17f(raw_t raw, FILE fp);
        //int input_lexrf(raw_t raw, FILE fp);

        //int gen_ubx(string msg, ref byte buff);
        //int gen_stq(string msg, ref byte buff);
        //int gen_nvs(string msg, ref byte buff);
        //int gen_lexr(string msg, ref byte buff);

        /* rtcm functions ------------------------------------------------------------*/
        //int init_rtcm(rtcm_t rtcm);
        //void free_rtcm(rtcm_t rtcm);
        //int input_rtcm2(rtcm_t rtcm, byte data);
        //int input_rtcm3(rtcm_t rtcm, byte data);
        //int input_rtcm2f(rtcm_t rtcm, FILE fp);
        //int input_rtcm3f(rtcm_t rtcm, FILE fp);
        //int gen_rtcm2(rtcm_t rtcm, int type, int sync);
        //int gen_rtcm3(rtcm_t rtcm, int type, int sync);

        /* solution functions --------------------------------------------------------*/
        //void initsolbuf(solbuf_t solbuf, int cyclic, int nmax);
        //void freesolbuf(solbuf_t solbuf);
        //void freesolstatbuf(solstatbuf_t solstatbuf);
        //sol_t getsol(solbuf_t solbuf, int index);
        //int addsol(solbuf_t solbuf, sol_t sol);
        //int readsol(string[] files, int nfile, solbuf_t sol);
        //int readsolt(string[] files, int nfile, gtime_t ts, gtime_t te, double tint, int qflag, solbuf_t sol);
        //int readsolstat(string[] files, int nfile, solstatbuf_t statbuf);
        //int readsolstatt(string[] files, int nfile, gtime_t ts, gtime_t te, double tint, solstatbuf_t statbuf);
        //int inputsol(byte data, gtime_t ts, gtime_t te, double tint, int qflag, solopt_t opt, solbuf_t solbuf);

        //int outprcopts(ref byte buff, prcopt_t opt);
        //int outsolheads(ref byte buff, solopt_t opt);
        //int outsols(ref byte buff, sol_t sol, double rb, solopt_t opt);
        //int outsolexs(ref byte buff, sol_t sol, ssat_t ssat, solopt_t opt);
        //void outprcopt(FILE fp, prcopt_t opt);
        //void outsolhead(FILE fp, solopt_t opt);
        //void outsol(FILE fp, sol_t sol, double rb, solopt_t opt);
        //void outsolex(FILE fp, sol_t sol, ssat_t ssat, solopt_t opt);
        //int outnmea_rmc(ref byte buff, sol_t sol);
        //int outnmea_gga(ref byte buff, sol_t sol);
        //int outnmea_gsa(ref byte buff, sol_t sol, ssat_t ssat);
        //int outnmea_gsv(ref byte buff, sol_t sol, ssat_t ssat);

        /* google earth kml converter ------------------------------------------------*/
        //int convkml(string infile, string outfile, gtime_t ts, gtime_t te, double tint, int qflg, ref double offset, int tcolor, int pcolor, int outalt, int outtime);

        /* sbas functions ------------------------------------------------------------*/
        //int sbsreadmsg(string file, int sel, sbs_t sbs);
        //int sbsreadmsgt(string file, int sel, gtime_t ts, gtime_t te, sbs_t sbs);
        //void sbsoutmsg(FILE fp, sbsmsg_t sbsmsg);
        //int sbsdecodemsg(gtime_t time, int prn, uint words, sbsmsg_t sbsmsg);
        //int sbsupdatecorr(sbsmsg_t msg, nav_t nav);
        //int sbssatcorr(gtime_t time, int sat, nav_t nav, ref double rs, ref double dts, ref double @var);
        //int sbsioncorr(gtime_t time, nav_t nav, double pos, double azel, ref double delay, ref double @var);
        //double sbstropcorr(gtime_t time, double pos, double azel, ref double @var);

        /* options functions ---------------------------------------------------------*/
        //opt_t searchopt(string name, opt_t opts);
        //int str2opt(opt_t opt, string str);
        //int opt2str(opt_t opt, ref string str);
        //int opt2buf(opt_t opt, ref string buff);
        //int loadopts(string file, opt_t opts);
        //int saveopts(string file, string mode, string comment, opt_t opts);
        //void resetsysopts();
        //void getsysopts(prcopt_t popt, solopt_t sopt, filopt_t fopt);
        //void setsysopts(prcopt_t popt, solopt_t sopt, filopt_t fopt);

        /* stream data input and output functions ------------------------------------*/
        //void strinitcom();
        //void strinit(stream_t stream);
        //void strlock(stream_t stream);
        //void strunlock(stream_t stream);
        //int stropen(stream_t stream, int type, int mode, string path);
        //void strclose(stream_t stream);
        //int strread(stream_t stream, ref byte buff, int n);
        //int strwrite(stream_t stream, ref byte buff, int n);
        //void strsync(stream_t stream1, stream_t stream2);
        //int strstat(stream_t stream, ref string msg);
        //void strsum(stream_t stream, ref int inb, ref int inr, ref int outb, ref int outr);
        //void strsetopt(int opt);
        //gtime_t strgettime(stream_t stream);
        //void strsendnmea(stream_t stream, double pos);
        //void strsendcmd(stream_t stream, string cmd);
        //void strsettimeout(stream_t stream, int toinact, int tirecon);
        //void strsetdir(string dir);
        //void strsetproxy(string addr);

        /* integer ambiguity resolution ----------------------------------------------*/
        //int lambda(int n, int m, double a, double Q, ref double F, ref double s);

        /* standard positioning ------------------------------------------------------*/
        //int pntpos(obsd_t obs, int n, nav_t nav, prcopt_t opt, sol_t sol, ref double azel, ssat_t ssat, ref string msg);

        /* precise positioning -------------------------------------------------------*/
        //void rtkinit(rtk_t rtk, prcopt_t opt);
        //void rtkfree(rtk_t rtk);
        //int rtkpos(rtk_t rtk, obsd_t obs, int nobs, nav_t nav);
        //int rtkopenstat(string file, int level);
        //void rtkclosestat();

        /* precise point positioning -------------------------------------------------*/
        //void pppos(rtk_t rtk, obsd_t obs, int n, nav_t nav);
        //int pppamb(rtk_t rtk, obsd_t obs, int n, nav_t nav, double azel);
        //int pppnx(prcopt_t opt);
        //void pppoutsolstat(rtk_t rtk, int level, FILE fp);
        //void windupcorr(gtime_t time, double rs, double rr, ref double phw);

        /* post-processing positioning -----------------------------------------------*/
        //int postpos(gtime_t ts, gtime_t te, double ti, double tu, prcopt_t popt, solopt_t sopt, filopt_t fopt, string[] infile, int n, ref string outfile, string rov, string @base);

        /* stream server functions ---------------------------------------------------*/
        //void strsvrinit(strsvr_t svr, int nout);
        //int strsvrstart(strsvr_t svr, ref int opts, ref int strs, string[] paths, strconv_t[][] conv, string cmd, double nmeapos);
        //void strsvrstop(strsvr_t svr, string cmd);
        //void strsvrstat(strsvr_t svr, ref int stat, ref int @byte, ref int bps, ref string msg);
        //strconv_t strconvnew(int itype, int otype, string msgs, int staid, int stasel, string opt);
        //void strconvfree(strconv_t conv);

        /* rtk server functions ------------------------------------------------------*/
        //int rtksvrinit(rtksvr_t svr);
        //void rtksvrfree(rtksvr_t svr);
        //int rtksvrstart(rtksvr_t svr, int cycle, int buffsize, ref int strs, string[] paths, ref int formats, int navsel, string[] cmds, string[] rcvopts, int nmeacycle, int nmeareq, double nmeapos, prcopt_t prcopt, solopt_t solopt, stream_t moni);
        //void rtksvrstop(rtksvr_t svr, string[] cmds);
        //int rtksvropenstr(rtksvr_t svr, int index, int str, string path, solopt_t solopt);
        //void rtksvrclosestr(rtksvr_t svr, int index);
        //void rtksvrlock(rtksvr_t svr);
        //void rtksvrunlock(rtksvr_t svr);
        //int rtksvrostat(rtksvr_t svr, int type, gtime_t time, ref int sat, ref double az, ref double el, int[][] snr, ref int vsat);
        //void rtksvrsstat(rtksvr_t svr, ref int sstat, ref string msg);

        /* downloader functions ------------------------------------------------------*/
        //int dl_readurls(string file, string[] types, int ntype, url_t urls, int nmax);
        //int dl_readstas(string file, string[] stas, int nmax);
        //int dl_exec(gtime_t ts, gtime_t te, double ti, int seqnos, int seqnoe, url_t urls, int nurl, string[] stas, int nsta, string dir, string usr, string pwd, string proxy, int opts, ref string msg, FILE fp);
        //void dl_test(gtime_t ts, gtime_t te, double ti, url_t urls, int nurl, string[] stas, int nsta, string dir, int ncol, int datefmt, FILE fp);

        /* application defined functions ---------------------------------------------*/
        //int showmsg(ref string format, params object[] LegacyParamArray);
        //void settspan(gtime_t ts, gtime_t te);
        //void settime(gtime_t time);

        /* qzss lex functions --------------------------------------------------------*/
        //int lexupdatecorr(lexmsg_t msg, nav_t nav, gtime_t tof);
        //int lexreadmsg(string file, int sel, lex_t lex);
        //void lexoutmsg(FILE fp, lexmsg_t msg);
        //int lexconvbin(int type, int format, string infile, string outfile);
        //int lexeph2pos(gtime_t time, int sat, nav_t nav, ref double rs, ref double dts, ref double @var);
        //int lexioncorr(gtime_t time, nav_t nav, double pos, double azel, ref double delay, ref double @var);
    }

    /*------------------------------------------------------------------------------
    * rtklib.h : rtklib constants, types and function prototypes
    *
    *          Copyright (C) 2007-2015 by T.TAKASU, All rights reserved.
    *
    * options : -DENAGLO   enable GLONASS
    *           -DENAGAL   enable Galileo
    *           -DENAQZS   enable QZSS
    *           -DENACMP   enable BeiDou
    *           -DNFREQ=n  set number of obs codes/frequencies
    *           -DNEXOBS=n set number of extended obs codes
    *           -DMAXOBS=n set max number of obs data in an epoch
    *           -DEXTLEX   enable QZSS LEX extension
    *
    * version : $Revision: 1.1 $ $Date: 2008/07/17 21:48:06 $
    * history : 2007/01/13 1.0  rtklib ver.1.0.0
    *           2007/03/20 1.1  rtklib ver.1.1.0
    *           2008/07/15 1.2  rtklib ver.2.1.0
    *           2008/10/19 1.3  rtklib ver.2.1.1
    *           2009/01/31 1.4  rtklib ver.2.2.0
    *           2009/04/30 1.5  rtklib ver.2.2.1
    *           2009/07/30 1.6  rtklib ver.2.2.2
    *           2009/12/25 1.7  rtklib ver.2.3.0
    *           2010/07/29 1.8  rtklib ver.2.4.0
    *           2011/05/27 1.9  rtklib ver.2.4.1
    *           2013/03/28 1.10 rtklib ver.2.4.2
    *-----------------------------------------------------------------------------*/

    /* type definitions ----------------------------------------------------------*/

    public class gtime_t // time struct
    {
        public time_t time = new time_t(); // time (s) expressed by standard time_t
        public double sec; // fraction of second under 1 s
    }

    public class obsd_t // observation data record
    {
        public gtime_t time = new gtime_t(); // receiver sampling time (GPST)
        public byte sat; // satellite/receiver number
        public byte rcv;
        public byte[] SNR = new byte[DefineConstants.NFREQ + DefineConstants.NEXOBS]; // signal strength (0.25 dBHz)
        public byte[] LLI = new byte[DefineConstants.NFREQ + DefineConstants.NEXOBS]; // loss of lock indicator
        public byte[] code = new byte[DefineConstants.NFREQ + DefineConstants.NEXOBS]; // code indicator (CODE_???)
        public double[] L = new double[DefineConstants.NFREQ + DefineConstants.NEXOBS]; // observation data carrier-phase (cycle)
        public double[] P = new double[DefineConstants.NFREQ + DefineConstants.NEXOBS]; // observation data pseudorange (m)
        public float[] D = new float[DefineConstants.NFREQ + DefineConstants.NEXOBS]; // observation data doppler frequency (Hz)
    }

    public class obs_t // observation data
    {
        public int n; // number of obervation data/allocated
        public int nmax;
        public obsd_t data; // observation data records
    }

    public class erpd_t // earth rotation parameter data type
    {
        public double mjd; // mjd (days)
        public double xp; // pole offset (rad)
        public double yp;
        public double xpr; // pole offset rate (rad/day)
        public double ypr;
        public double ut1_utc; // ut1-utc (s)
        public double lod; // length of day (s/day)
    }

    public class erp_t // earth rotation parameter type
    {
        public int n; // number and max number of data
        public int nmax;
        public erpd_t data; // earth rotation parameter data
    }

    public class pcv_t // antenna parameter type
    {
        public int sat; // satellite number (0:receiver)
        public string type = new string(new char[DefineConstants.MAXANT]); // antenna type
        public string code = new string(new char[DefineConstants.MAXANT]); // serial number or satellite code
        public gtime_t ts = new gtime_t(); // valid time start and end
        public gtime_t te = new gtime_t();
        public double[,] off = new double[DefineConstants.NFREQ, 3]; // phase center offset e/n/u or x/y/z (m)
        public double[,] @var = new double[DefineConstants.NFREQ, 19]; // phase center variation (m)
                                                                       /* el=90,85,...,0 or nadir=0,1,2,3,... (deg) */
    }

    public class pcvs_t // antenna parameters type
    {
        public int n; // number of data/allocated
        public int nmax;
        public pcv_t pcv; // antenna parameters data
    }

    public class alm_t // almanac type
    {
        public int sat; // satellite number
        public int svh; // sv health (0:ok)
        public int svconf; // as and sv config
        public int week; // GPS/QZS: gps week, GAL: galileo week
        public gtime_t toa = new gtime_t(); // Toa
                                            /* SV orbit parameters */
        public double A;
        public double e;
        public double i0;
        public double OMG0;
        public double omg;
        public double M0;
        public double OMGd;
        public double toas; // Toa (s) in week
        public double f0; // SV clock parameters (af0,af1)
        public double f1;
    }

    public class eph_t // GPS/QZS/GAL broadcast ephemeris type
    {
        public int sat; // satellite number
        public int iode; // IODE,IODC
        public int iodc;
        public int sva; // SV accuracy (URA index)
        public int svh; // SV health (0:ok)
        public int week; // GPS/QZS: gps week, GAL: galileo week
        public int code; // GPS/QZS: code on L2, GAL/CMP: data sources
        public int flag; // GPS/QZS: L2 P data flag, CMP: nav type
        public gtime_t toe = new gtime_t(); // Toe,Toc,T_trans
        public gtime_t toc = new gtime_t();
        public gtime_t ttr = new gtime_t();
        /* SV orbit parameters */
        public double A;
        public double e;
        public double i0;
        public double OMG0;
        public double omg;
        public double M0;
        public double deln;
        public double OMGd;
        public double idot;
        public double crc;
        public double crs;
        public double cuc;
        public double cus;
        public double cic;
        public double cis;
        public double toes; // Toe (s) in week
        public double fit; // fit interval (h)
        public double f0; // SV clock parameters (af0,af1,af2)
        public double f1;
        public double f2;
        public double[] tgd = new double[4]; // group delay parameters
                                             /* GPS/QZS:tgd[0]=TGD */
                                             /* GAL    :tgd[0]=BGD E5a/E1,tgd[1]=BGD E5b/E1 */
                                             /* CMP    :tgd[0]=BGD1,tgd[1]=BGD2 */
        public double Adot; // Adot,ndot for CNAV
        public double ndot;
    }

    public class geph_t // GLONASS broadcast ephemeris type
    {
        public int sat; // satellite number
        public int iode; // IODE (0-6 bit of tb field)
        public int frq; // satellite frequency number
        public int svh; // satellite health, accuracy, age of operation
        public int sva;
        public int age;
        public gtime_t toe = new gtime_t(); // epoch of epherides (gpst)
        public gtime_t tof = new gtime_t(); // message frame time (gpst)
        public double[] pos = new double[3]; // satellite position (ecef) (m)
        public double[] vel = new double[3]; // satellite velocity (ecef) (m/s)
        public double[] acc = new double[3]; // satellite acceleration (ecef) (m/s^2)
        public double taun; // SV clock bias (s)/relative freq bias
        public double gamn;
        public double dtaun; // delay between L1 and L2 (s)
    }

    public class peph_t // precise ephemeris type
    {
        public gtime_t time = new gtime_t(); // time (GPST)
        public int index; // ephemeris index for multiple files
        public double[,] pos = new double[DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1, 4]; // satellite position/clock (ecef) (m|s)
        public float[,] std = new float[DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1, 4]; // satellite position/clock std (m|s)
        public double[,] vel = new double[DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1, 4]; // satellite velocity/clk-rate (m/s|s/s)
        public float[,] vst = new float[DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1, 4]; // satellite velocity/clk-rate std (m/s|s/s)
        public float[,] cov = new float[DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1, 3]; // satellite position covariance (m^2)
        public float[,] vco = new float[DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1, 3]; // satellite velocity covariance (m^2)
    }

    public class pclk_t // precise clock type
    {
        public gtime_t time = new gtime_t(); // time (GPST)
        public int index; // clock index for multiple files
        public double[,] clk = new double[DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1, 1]; // satellite clock (s)
        public float[,] std = new float[DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1, 1]; // satellite clock std (s)
    }

    public class seph_t // SBAS ephemeris type
    {
        public int sat; // satellite number
        public gtime_t t0 = new gtime_t(); // reference epoch time (GPST)
        public gtime_t tof = new gtime_t(); // time of message frame (GPST)
        public int sva; // SV accuracy (URA index)
        public int svh; // SV health (0:ok)
        public double[] pos = new double[3]; // satellite position (m) (ecef)
        public double[] vel = new double[3]; // satellite velocity (m/s) (ecef)
        public double[] acc = new double[3]; // satellite acceleration (m/s^2) (ecef)
        public double af0; // satellite clock-offset/drift (s,s/s)
        public double af1;
    }

    public class tled_t // norad two line element data type
    {
        public string name = new string(new char[32]); // common name
        public string alias = new string(new char[32]); // alias name
        public string satno = new string(new char[16]); // satellilte catalog number
        public sbyte satclass; // classification
        public string desig = new string(new char[16]); // international designator
        public gtime_t epoch = new gtime_t(); // element set epoch (UTC)
        public double ndot; // 1st derivative of mean motion
        public double nddot; // 2st derivative of mean motion
        public double bstar; // B* drag term
        public int etype; // element set type
        public int eleno; // element number
        public double inc; // orbit inclination (deg)
        public double OMG; // right ascension of ascending node (deg)
        public double ecc; // eccentricity
        public double omg; // argument of perigee (deg)
        public double M; // mean anomaly (deg)
        public double n; // mean motion (rev/day)
        public int rev; // revolution number at epoch
    }

    public class tle_t // norad two line element type
    {
        public int n; // number/max number of two line element data
        public int nmax;
        public tled_t data; // norad two line element data
    }

    public class tec_t // TEC grid type
    {
        public gtime_t time = new gtime_t(); // epoch time (GPST)
        public int[] ndata = new int[3]; // TEC grid data size {nlat,nlon,nhgt}
        public double rb; // earth radius (km)
        public double[] lats = new double[3]; // latitude start/interval (deg)
        public double[] lons = new double[3]; // longitude start/interval (deg)
        public double[] hgts = new double[3]; // heights start/interval (km)
                                              //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
                                              //ORIGINAL LINE: double *data;
        public double data; // TEC grid data (tecu)
                            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
                            //ORIGINAL LINE: float *rms;
        public float rms; // RMS values (tecu)
    }

    public class stecd_t // stec data type
    {
        public gtime_t time = new gtime_t(); // time (GPST)
        public byte sat; // satellite number
        public byte slip; // slip flag
        public float iono; // L1 ionosphere delay (m)
        public float rate; // L1 ionosphere rate (m/s)
        public float rms; // rms value (m)
    }

    public class stec_t // stec grid type
    {
        public double[] pos = new double[2]; // latitude/longitude (deg)
        public int[] index = new int[DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1]; // search index
        public int n; // number of data
        public int nmax;
        public stecd_t data; // stec data
    }

    public class zwdd_t // zwd data type
    {
        public gtime_t time = new gtime_t(); // time (GPST)
        public float zwd; // zenith wet delay (m)
        public float rms; // rms value (m)
    }

    public class zwd_t // zwd grid type
    {
        public float[] pos = new float[2]; // latitude,longitude (rad)
        public int n; // number of data
        public int nmax;
        public zwdd_t data; // zwd data
    }

    public class sbsmsg_t // SBAS message type
    {
        public int week; // receiption time
        public int tow;
        public int prn; // SBAS satellite PRN number
        public byte[] msg = new byte[29]; // SBAS message (226bit) padded by 0
    }

    public class sbs_t // SBAS messages type
    {
        public int n; // number of SBAS messages/allocated
        public int nmax;
        public sbsmsg_t msgs; // SBAS messages
    }

    public class sbsfcorr_t // SBAS fast correction type
    {
        public gtime_t t0 = new gtime_t(); // time of applicability (TOF)
        public double prc; // pseudorange correction (PRC) (m)
        public double rrc; // range-rate correction (RRC) (m/s)
        public double dt; // range-rate correction delta-time (s)
        public int iodf; // IODF (issue of date fast corr)
        public short udre; // UDRE+1
        public short ai; // degradation factor indicator
    }

    public class sbslcorr_t // SBAS long term satellite error correction type
    {
        public gtime_t t0 = new gtime_t(); // correction time
        public int iode; // IODE (issue of date ephemeris)
        public double[] dpos = new double[3]; // delta position (m) (ecef)
        public double[] dvel = new double[3]; // delta velocity (m/s) (ecef)
        public double daf0; // delta clock-offset/drift (s,s/s)
        public double daf1;
    }

    public class sbssatp_t // SBAS satellite correction type
    {
        public int sat; // satellite number
        public sbsfcorr_t fcorr = new sbsfcorr_t(); // fast correction
        public sbslcorr_t lcorr = new sbslcorr_t(); // long term correction
    }

    public class sbssat_t // SBAS satellite corrections type
    {
        public int iodp; // IODP (issue of date mask)
        public int nsat; // number of satellites
        public int tlat; // system latency (s)
        public sbssatp_t[] sat = Arrays.InitializeWithDefaultInstances<sbssatp_t>(DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1); // satellite correction
    }

    public class sbsigp_t // SBAS ionospheric correction type
    {
        public gtime_t t0 = new gtime_t(); // correction time
        public short lat; // latitude/longitude (deg)
        public short lon;
        public short give; // GIVI+1
        public float delay; // vertical delay estimate (m)
    }

    public class sbsigpband_t // IGP band type
    {
        public short x; // longitude/latitude (deg)
                        //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
                        //ORIGINAL LINE: const short *y;
        public short y; // latitudes/longitudes (deg)
        public byte bits; // IGP mask start bit
        public byte bite; // IGP mask end bit
    }

    public class sbsion_t // SBAS ionospheric corrections type
    {
        public int iodi; // IODI (issue of date ionos corr)
        public int nigp; // number of igps
        public sbsigp_t[] igp = Arrays.InitializeWithDefaultInstances<sbsigp_t>(DefineConstants.MAXNIGP); // ionospheric correction
    }

    public class dgps_t // DGPS/GNSS correction type
    {
        public gtime_t t0 = new gtime_t(); // correction time
        public double prc; // pseudorange correction (PRC) (m)
        public double rrc; // range rate correction (RRC) (m/s)
        public int iod; // issue of data (IOD)
        public double udre; // UDRE
    }

    public class ssr_t // SSR correction type
    {
        public gtime_t[] t0 = Arrays.InitializeWithDefaultInstances<gtime_t>(5); // epoch time (GPST) {eph,clk,hrclk,ura,bias}
        public double[] udi = new double[5]; // SSR update interval (s)
        public int[] iod = new int[5]; // iod ssr {eph,clk,hrclk,ura,bias}
        public int iode; // issue of data
        public int iodcrc; // issue of data crc for beidou/sbas
        public int ura; // URA indicator
        public int refd; // sat ref datum (0:ITRF,1:regional)
        public double[] deph = new double[3]; // delta orbit {radial,along,cross} (m)
        public double[] ddeph = new double[3]; // dot delta orbit {radial,along,cross} (m/s)
        public double[] dclk = new double[3]; // delta clock {c0,c1,c2} (m,m/s,m/s^2)
        public double hrclk; // high-rate clock corection (m)
        public float[] cbias = new float[DefineConstants.MAXCODE]; // code biases (m)
        public byte update; // update flag (0:no update,1:update)
    }

    public class lexmsg_t // QZSS LEX message type
    {
        public int prn; // satellite PRN number
        public int type; // message type
        public int alert; // alert flag
        public byte stat; // signal tracking status
        public byte snr; // signal C/N0 (0.25 dBHz)
        public uint ttt; // tracking time (ms)
        public byte[] msg = new byte[212]; // LEX message data part 1695 bits
    }

    public class lex_t // QZSS LEX messages type
    {
        public int n; // number of LEX messages and allocated
        public int nmax;
        public lexmsg_t msgs; // LEX messages
    }

    public class lexeph_t // QZSS LEX ephemeris type
    {
        public gtime_t toe = new gtime_t(); // epoch time (GPST)
        public gtime_t tof = new gtime_t(); // message frame time (GPST)
        public int sat; // satellite number
        public byte health; // signal health (L1,L2,L1C,L5,LEX)
        public byte ura; // URA index
        public double[] pos = new double[3]; // satellite position (m)
        public double[] vel = new double[3]; // satellite velocity (m/s)
        public double[] acc = new double[3]; // satellite acceleration (m/s2)
        public double[] jerk = new double[3]; // satellite jerk (m/s3)
        public double af0; // satellite clock bias and drift (s,s/s)
        public double af1;
        public double tgd; // TGD
        public double[] isc = new double[8]; // ISC
    }

    public class lexion_t // QZSS LEX ionosphere correction type
    {
        public gtime_t t0 = new gtime_t(); // epoch time (GPST)
        public double tspan; // valid time span (s)
        public double[] pos0 = new double[2]; // reference position {lat,lon} (rad)
        public double[,] coef = new double[3, 2]; // coefficients lat x lon (3 x 2)
    }

    public class nav_t // navigation data type
    {
        public int n; // number of broadcast ephemeris
        public int nmax;
        public int ng; // number of glonass ephemeris
        public int ngmax;
        public int ns; // number of sbas ephemeris
        public int nsmax;
        public int ne; // number of precise ephemeris
        public int nemax;
        public int nc; // number of precise clock
        public int ncmax;
        public int na; // number of almanac data
        public int namax;
        public int nt; // number of tec grid data
        public int ntmax;
        public int nn; // number of stec grid data
        public int nnmax;
        public eph_t eph; // GPS/QZS/GAL ephemeris
        public geph_t geph; // GLONASS ephemeris
        public seph_t seph; // SBAS ephemeris
        public peph_t peph; // precise ephemeris
        public pclk_t pclk; // precise clock
        public alm_t alm; // almanac data
        public tec_t tec; // tec grid data
        public stec_t stec; // stec grid data
        public erp_t erp = new erp_t(); // earth rotation parameters
        public double[] utc_gps = new double[4]; // GPS delta-UTC parameters {A0,A1,T,W}
        public double[] utc_glo = new double[4]; // GLONASS UTC GPS time parameters
        public double[] utc_gal = new double[4]; // Galileo UTC GPS time parameters
        public double[] utc_qzs = new double[4]; // QZS UTC GPS time parameters
        public double[] utc_cmp = new double[4]; // BeiDou UTC parameters
        public double[] utc_sbs = new double[4]; // SBAS UTC parameters
        public double[] ion_gps = new double[8]; // GPS iono model parameters {a0,a1,a2,a3,b0,b1,b2,b3}
        public double[] ion_gal = new double[4]; // Galileo iono model parameters {ai0,ai1,ai2,0}
        public double[] ion_qzs = new double[8]; // QZSS iono model parameters {a0,a1,a2,a3,b0,b1,b2,b3}
        public double[] ion_cmp = new double[8]; // BeiDou iono model parameters {a0,a1,a2,a3,b0,b1,b2,b3}
        public int leaps; // leap seconds (s)
        public double[,] lam = new double[DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1, DefineConstants.NFREQ]; // carrier wave lengths (m)
        public double[,] cbias = new double[DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1, 3]; // code bias (0:p1-p2,1:p1-c1,2:p2-c2) (m)
        public double[] wlbias = new double[DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1]; // wide-lane bias (cycle)
        public double[] glo_cpbias = new double[4]; // glonass code-phase bias {1C,1P,2C,2P} (m)
        public string glo_fcn = new string(new char[DefineConstants.MAXPRNGLO + 1]); // glonass frequency channel number + 8
        public pcv_t[] pcvs = Arrays.InitializeWithDefaultInstances<pcv_t>(DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1); // satellite antenna pcv
        public sbssat_t sbssat = new sbssat_t(); // SBAS satellite corrections
        public sbsion_t[] sbsion = Arrays.InitializeWithDefaultInstances<sbsion_t>(DefineConstants.MAXBAND + 1); // SBAS ionosphere corrections
        public dgps_t[] dgps = Arrays.InitializeWithDefaultInstances<dgps_t>(DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1); // DGPS corrections
        public ssr_t[] ssr = Arrays.InitializeWithDefaultInstances<ssr_t>(DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1); // SSR corrections
        public lexeph_t[] lexeph = Arrays.InitializeWithDefaultInstances<lexeph_t>(DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1); // LEX ephemeris
        public lexion_t lexion = new lexion_t(); // LEX ionosphere correction
    }

    public class sta_t // station parameter type
    {
        public string name = new string(new char[DefineConstants.MAXANT]); // marker name
        public string marker = new string(new char[DefineConstants.MAXANT]); // marker number
        public string antdes = new string(new char[DefineConstants.MAXANT]); // antenna descriptor
        public string antsno = new string(new char[DefineConstants.MAXANT]); // antenna serial number
        public string rectype = new string(new char[DefineConstants.MAXANT]); // receiver type descriptor
        public string recver = new string(new char[DefineConstants.MAXANT]); // receiver firmware version
        public string recsno = new string(new char[DefineConstants.MAXANT]); // receiver serial number
        public int antsetup; // antenna setup id
        public int itrf; // ITRF realization year
        public int deltype; // antenna delta type (0:enu,1:xyz)
        public double[] pos = new double[3]; // station position (ecef) (m)
        public double[] del = new double[3]; // antenna position delta (e/n/u or x/y/z) (m)
        public double hgt; // antenna height (m)
    }

    public class sol_t // solution type
    {
        public gtime_t time = new gtime_t(); // time (GPST)
        public double[] rr = new double[6]; // position/velocity (m|m/s)
                                            /* {x,y,z,vx,vy,vz} or {e,n,u,ve,vn,vu} */
        public float[] qr = new float[6]; // position variance/covariance (m^2)
                                          /* {c_xx,c_yy,c_zz,c_xy,c_yz,c_zx} or */
                                          /* {c_ee,c_nn,c_uu,c_en,c_nu,c_ue} */
        public double[] dtr = new double[6]; // receiver clock bias to time systems (s)
        public byte type; // type (0:xyz-ecef,1:enu-baseline)
        public byte stat; // solution status (SOLQ_???)
        public byte ns; // number of valid satellites
        public float age; // age of differential (s)
        public float ratio; // AR ratio factor for valiation
    }

    public class solbuf_t // solution buffer type
    {
        public int n; // number of solution/max number of buffer
        public int nmax;
        public int cyclic; // cyclic buffer flag
        public int start; // start/end index
        public int end;
        public gtime_t time = new gtime_t(); // current solution time
        public sol_t data; // solution data
        public double[] rb = new double[3]; // reference position {x,y,z} (ecef) (m)
        public byte[] buff = new byte[DefineConstants.MAXSOLMSG + 1]; // message buffer
        public int nb; // number of byte in message buffer
    }

    public class solstat_t // solution status type
    {
        public gtime_t time = new gtime_t(); // time (GPST)
        public byte sat; // satellite number
        public byte frq; // frequency (1:L1,2:L2,...)
        public float az; // azimuth/elevation angle (rad)
        public float el;
        public float resp; // pseudorange residual (m)
        public float resc; // carrier-phase residual (m)
        public byte flag; // flags: (vsat<<5)+(slip<<3)+fix
        public byte snr; // signal strength (0.25 dBHz)
        public ushort @lock; // lock counter
        public ushort outc; // outage counter
        public ushort slipc; // slip counter
        public ushort rejc; // reject counter
    }

    public class solstatbuf_t // solution status buffer type
    {
        public int n; // number of solution/max number of buffer
        public int nmax;
        public solstat_t data; // solution status data
    }

    public class rtcm_t // RTCM control struct type
    {
        public int staid; // station id
        public int stah; // station health
        public int seqno; // sequence number for rtcm 2 or iods msm
        public int outtype; // output message type
        public gtime_t time = new gtime_t(); // message time
        public gtime_t time_s = new gtime_t(); // message start time
        public obs_t obs = new obs_t(); // observation data (uncorrected)
        public nav_t nav = new nav_t(); // satellite ephemerides
        public sta_t sta = new sta_t(); // station parameters
        public dgps_t dgps; // output of dgps corrections
        public ssr_t[] ssr = Arrays.InitializeWithDefaultInstances<ssr_t>(DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1); // output of ssr corrections
        public string msg = new string(new char[128]); // special message
        public string msgtype = new string(new char[256]); // last message type
        public sbyte[,] msmtype = new sbyte[6, 128]; // msm signal types
        public int obsflag; // obs data complete flag (1:ok,0:not complete)
        public int ephsat; // update satellite of ephemeris
        public double[,] cp = new double[DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1, DefineConstants.NFREQ + DefineConstants.NEXOBS]; // carrier-phase measurement
        public byte[,] @lock = new byte[DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1, DefineConstants.NFREQ + DefineConstants.NEXOBS]; // lock time
        public byte[,] loss = new byte[DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1, DefineConstants.NFREQ + DefineConstants.NEXOBS]; // loss of lock count
        public gtime_t[,] lltime = new gtime_t[DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1, DefineConstants.NFREQ + DefineConstants.NEXOBS]; // last lock time
        public int nbyte; // number of bytes in message buffer
        public int nbit; // number of bits in word buffer
        public int len; // message length (bytes)
        public byte[] buff = new byte[1200]; // message buffer
        public uint word; // word buffer for rtcm 2
        public uint[] nmsg2 = new uint[100]; // message count of RTCM 2 (1-99:1-99,0:other)
        public uint[] nmsg3 = new uint[300]; // message count of RTCM 3 (1-299:1001-1299,0:ohter)
        public string opt = new string(new char[256]); // RTCM dependent options
    }

    public class rnxctr_t // rinex control struct type
    {
        public gtime_t time = new gtime_t(); // message time
        public double ver; // rinex version
        public sbyte type; // rinex file type ('O','N',...)
        public int sys; // navigation system
        public int tsys; // time system
        public sbyte[,,] tobs = new sbyte[6, DefineConstants.MAXOBSTYPE, 4]; // rinex obs types
        public obs_t obs = new obs_t(); // observation data
        public nav_t nav = new nav_t(); // navigation data
        public sta_t sta = new sta_t(); // station info
        public int ephsat; // ephemeris satellite number
        public string opt = new string(new char[256]); // rinex dependent options
    }

    public class url_t // download url type
    {
        public string type = new string(new char[32]); // data type
        public string path = new string(new char[1024]); // url path
        public string dir = new string(new char[1024]); // local directory
        public double tint; // time interval (s)
    }

    public class opt_t // option type
    {
        public string name; // option name
        public int format; // option format (0:int,1:double,2:string,3:enum)
        public object @var; // pointer to option variable
        public string comment; // option comment/enum labels/unit
    }

    public class exterr_t // extended receiver error model
    {
        public int[] ena = new int[4]; // model enabled
        public double[,] cerr = new double[4, DefineConstants.NFREQ * 2]; // code errors (m)
        public double[,] perr = new double[4, DefineConstants.NFREQ * 2]; // carrier-phase errors (m)
        public double[] gpsglob = new double[DefineConstants.NFREQ]; // gps-glonass h/w bias (m)
        public double[] gloicb = new double[DefineConstants.NFREQ]; // glonass interchannel bias (m/fn)
    }

    public class snrmask_t // SNR mask type
    {
        public int[] ena = new int[2]; // enable flag {rover,base}
        public double[,] mask = new double[DefineConstants.NFREQ, 9]; // mask (dBHz) at 5,10,...85 deg
    }

    public class prcopt_t // processing options type
    {
        public int mode; // positioning mode (PMODE_???)
        public int soltype; // solution type (0:forward,1:backward,2:combined)
        public int nf; // number of frequencies (1:L1,2:L1+L2,3:L1+L2+L5)
        public int navsys; // navigation system
        public double elmin; // elevation mask angle (rad)
        public snrmask_t snrmask = new snrmask_t(); // SNR mask
        public int sateph; // satellite ephemeris/clock (EPHOPT_???)
        public int modear; // AR mode (0:off,1:continuous,2:instantaneous,3:fix and hold,4:ppp-ar)
        public int glomodear; // GLONASS AR mode (0:off,1:on,2:auto cal,3:ext cal)
        public int bdsmodear; // BeiDou AR mode (0:off,1:on)
        public int maxout; // obs outage count to reset bias
        public int minlock; // min lock count to fix ambiguity
        public int minfix; // min fix count to hold ambiguity
        public int ionoopt; // ionosphere option (IONOOPT_???)
        public int tropopt; // troposphere option (TROPOPT_???)
        public int dynamics; // dynamics model (0:none,1:velociy,2:accel)
        public int tidecorr; // earth tide correction (0:off,1:solid,2:solid+otl+pole)
        public int niter; // number of filter iteration
        public int codesmooth; // code smoothing window size (0:none)
        public int intpref; // interpolate reference obs (for post mission)
        public int sbascorr; // SBAS correction options
        public int sbassatsel; // SBAS satellite selection (0:all)
        public int rovpos; // rover position for fixed mode
        public int refpos; // base position for relative mode
                           /* (0:pos in prcopt,  1:average of single pos, */
                           /*  2:read from file, 3:rinex header, 4:rtcm pos) */
        public double[] eratio = new double[DefineConstants.NFREQ]; // code/phase error ratio
        public double[] err = new double[5]; // measurement error factor
                                             /* [0]:reserved */
                                             /* [1-3]:error factor a/b/c of phase (m) */
                                             /* [4]:doppler frequency (hz) */
        public double[] std = new double[3]; // initial-state std [0]bias,[1]iono [2]trop
        public double[] prn = new double[5]; // process-noise std [0]bias,[1]iono [2]trop [3]acch [4]accv
        public double sclkstab; // satellite clock stability (sec/sec)
        public double[] thresar = new double[4]; // AR validation threshold
        public double elmaskar; // elevation mask of AR for rising satellite (deg)
        public double elmaskhold; // elevation mask to hold ambiguity (deg)
        public double thresslip; // slip threshold of geometry-free phase (m)
        public double maxtdiff; // max difference of time (sec)
        public double maxinno; // reject threshold of innovation (m)
        public double maxgdop; // reject threshold of gdop
        public double[] baseline = new double[2]; // baseline length constraint {const,sigma} (m)
        public double[] ru = new double[3]; // rover position for fixed mode {x,y,z} (ecef) (m)
        public double[] rb = new double[3]; // base position for relative mode {x,y,z} (ecef) (m)
        public sbyte[,] anttype = new sbyte[2, DefineConstants.MAXANT]; // antenna types {rover,base}
        public double[,] antdel = new double[2, 3]; // antenna delta {{rov_e,rov_n,rov_u},{ref_e,ref_n,ref_u}}
        public pcv_t[] pcvr = Arrays.InitializeWithDefaultInstances<pcv_t>(2); // receiver antenna parameters {rov,base}
        public byte[] exsats = new byte[DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1]; // excluded satellites (1:excluded,2:included)
        public sbyte[,] rnxopt = new sbyte[2, 256]; // rinex options {rover,base}
        public int[] posopt = new int[6]; // positioning options
        public int syncsol; // solution sync mode (0:off,1:on)
        public double[,] odisp = new double[2, 6 * 11]; // ocean tide loading parameters {rov,base}
        public exterr_t exterr = new exterr_t(); // extended receiver error model
    }

    public class solopt_t // solution options type
    {
        public int posf; // solution format (SOLF_???)
        public int times; // time system (TIMES_???)
        public int timef; // time format (0:sssss.s,1:yyyy/mm/dd hh:mm:ss.s)
        public int timeu; // time digits under decimal point
        public int degf; // latitude/longitude format (0:ddd.ddd,1:ddd mm ss)
        public int outhead; // output header (0:no,1:yes)
        public int outopt; // output processing options (0:no,1:yes)
        public int datum; // datum (0:WGS84,1:Tokyo)
        public int height; // height (0:ellipsoidal,1:geodetic)
        public int geoid; // geoid model (0:EGM96,1:JGD2000)
        public int solstatic; // solution of static mode (0:all,1:single)
        public int sstat; // solution statistics level (0:off,1:states,2:residuals)
        public int trace; // debug trace level (0:off,1-5:debug)
        public double[] nmeaintv = new double[2]; // nmea output interval (s) (<0:no,0:all)
                                                  /* nmeaintv[0]:gprmc,gpgga,nmeaintv[1]:gpgsv */
        public string sep = new string(new char[64]); // field separator
        public string prog = new string(new char[64]); // program name
    }

    public class filopt_t // file options type
    {
        public string satantp = new string(new char[DefineConstants.MAXSTRPATH]); // satellite antenna parameters file
        public string rcvantp = new string(new char[DefineConstants.MAXSTRPATH]); // receiver antenna parameters file
        public string stapos = new string(new char[DefineConstants.MAXSTRPATH]); // station positions file
        public string geoid = new string(new char[DefineConstants.MAXSTRPATH]); // external geoid data file
        public string iono = new string(new char[DefineConstants.MAXSTRPATH]); // ionosphere data file
        public string dcb = new string(new char[DefineConstants.MAXSTRPATH]); // dcb data file
        public string eop = new string(new char[DefineConstants.MAXSTRPATH]); // eop data file
        public string blq = new string(new char[DefineConstants.MAXSTRPATH]); // ocean tide loading blq file
        public string tempdir = new string(new char[DefineConstants.MAXSTRPATH]); // ftp/http temporaly directory
        public string geexe = new string(new char[DefineConstants.MAXSTRPATH]); // google earth exec file
        public string solstat = new string(new char[DefineConstants.MAXSTRPATH]); // solution statistics file
        public string trace = new string(new char[DefineConstants.MAXSTRPATH]); // debug trace file
    }

    public class rnxopt_t // RINEX options type
    {
        public gtime_t ts = new gtime_t(); // time start/end
        public gtime_t te = new gtime_t();
        public double tint; // time interval (s)
        public double tunit; // time unit for multiple-session (s)
        public double rnxver; // RINEX version
        public int navsys; // navigation system
        public int obstype; // observation type
        public int freqtype; // frequency type
        public sbyte[,] mask = new sbyte[6, 64]; // code mask {GPS,GLO,GAL,QZS,SBS,CMP}
        public string staid = new string(new char[32]); // station id for rinex file name
        public string prog = new string(new char[32]); // program
        public string runby = new string(new char[32]); // run-by
        public string marker = new string(new char[64]); // marker name
        public string markerno = new string(new char[32]); // marker number
        public string markertype = new string(new char[32]); // marker type (ver.3)
        public sbyte[,] name = new sbyte[2, 32]; // observer/agency
        public sbyte[,] rec = new sbyte[3, 32]; // receiver #/type/vers
        public sbyte[,] ant = new sbyte[3, 32]; // antenna #/type
        public double[] apppos = new double[3]; // approx position x/y/z
        public double[] antdel = new double[3]; // antenna delta h/e/n
        public sbyte[,] comment = new sbyte[DefineConstants.MAXCOMMENT, 64]; // comments
        public string rcvopt = new string(new char[256]); // receiver dependent options
        public byte[] exsats = new byte[DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1]; // excluded satellites
        public int scanobs; // scan obs types
        public int outiono; // output iono correction
        public int outtime; // output time system correction
        public int outleaps; // output leap seconds
        public int autopos; // auto approx position
        public gtime_t tstart = new gtime_t(); // first obs time
        public gtime_t tend = new gtime_t(); // last obs time
        public gtime_t trtcm = new gtime_t(); // approx log start time for rtcm
        public sbyte[,,] tobs = new sbyte[6, DefineConstants.MAXOBSTYPE, 4]; // obs types {GPS,GLO,GAL,QZS,SBS,CMP}
        public int[] nobs = new int[6]; // number of obs types {GPS,GLO,GAL,QZS,SBS,CMP}
    }

    public class ssat_t // satellite status type
    {
        public byte sys; // navigation system
        public byte vs; // valid satellite flag single
        public double[] azel = new double[2]; // azimuth/elevation angles {az,el} (rad)
        public double[] resp = new double[DefineConstants.NFREQ]; // residuals of pseudorange (m)
        public double[] resc = new double[DefineConstants.NFREQ]; // residuals of carrier-phase (m)
        public byte[] vsat = new byte[DefineConstants.NFREQ]; // valid satellite flag
        public byte[] snr = new byte[DefineConstants.NFREQ]; // signal strength (0.25 dBHz)
        public byte[] fix = new byte[DefineConstants.NFREQ]; // ambiguity fix flag (1:fix,2:float,3:hold)
        public byte[] slip = new byte[DefineConstants.NFREQ]; // cycle-slip flag
        public uint[] @lock = new uint[DefineConstants.NFREQ]; // lock counter of phase
        public uint[] outc = new uint[DefineConstants.NFREQ]; // obs outage counter of phase
        public uint[] slipc = new uint[DefineConstants.NFREQ]; // cycle-slip counter
        public uint[] rejc = new uint[DefineConstants.NFREQ]; // reject counter
        public double gf; // geometry-free phase L1-L2 (m)
        public double gf2; // geometry-free phase L1-L5 (m)
        public double phw; // phase windup (cycle)
        public gtime_t[,] pt = new gtime_t[2, DefineConstants.NFREQ]; // previous carrier-phase time
        public double[,] ph = new double[2, DefineConstants.NFREQ]; // previous carrier-phase observable (cycle)
    }

    public class ambc_t // ambiguity control type
    {
        public gtime_t[] epoch = Arrays.InitializeWithDefaultInstances<gtime_t>(4); // last epoch
        public int fixcnt; // fix counter
        public string flags = new string(new char[DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1]); // fix flags
        public double[] n = new double[4]; // number of epochs
        public double[] LC = new double[4]; // linear combination average
        public double[] LCv = new double[4]; // linear combination variance
    }

    public class rtk_t // RTK control/result type
    {
        public sol_t sol = new sol_t(); // RTK solution
        public double[] rb = new double[6]; // base position/velocity (ecef) (m|m/s)
        public int nx; // number of float states/fixed states
        public int na;
        public double tt; // time difference between current and previous (s)
                          //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
                          //ORIGINAL LINE: double *x, *P;
        public double x; // float states and their covariance
                         //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
                         //ORIGINAL LINE: double *P;
        public double P;
        //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
        //ORIGINAL LINE: double *xa,*Pa;
        public double xa; // fixed states and their covariance
                          //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
                          //ORIGINAL LINE: double *Pa;
        public double Pa;
        public int nfix; // number of continuous fixes of ambiguity
        public ambc_t[] ambc = Arrays.InitializeWithDefaultInstances<ambc_t>(DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1); // ambibuity control
        public ssat_t[] ssat = Arrays.InitializeWithDefaultInstances<ssat_t>(DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1); // satellite status
        public int neb; // bytes in error message buffer
        public string errbuf = new string(new char[DefineConstants.MAXERRMSG]); // error message buffer
        public prcopt_t opt = new prcopt_t(); // processing options
    }

    public class raw_t // receiver raw data control type
    {
        public gtime_t time = new gtime_t(); // message time
        public gtime_t tobs = new gtime_t(); // observation data time
        public obs_t obs = new obs_t(); // observation data
        public obs_t obuf = new obs_t(); // observation data buffer
        public nav_t nav = new nav_t(); // satellite ephemerides
        public sta_t sta = new sta_t(); // station parameters
        public int ephsat; // sat number of update ephemeris (0:no satellite)
        public sbsmsg_t sbsmsg = new sbsmsg_t(); // SBAS message
        public string msgtype = new string(new char[256]); // last message type
        public byte[,] subfrm = new byte[DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1, 380]; // subframe buffer
        public lexmsg_t lexmsg = new lexmsg_t(); // LEX message
        public double[,] lockt = new double[DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1, DefineConstants.NFREQ + DefineConstants.NEXOBS]; // lock time (s)
        public double[] icpp = new double[DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1]; // carrier params for ss2
        public double[] off = new double[DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1];
        public double icpc;
        public double[] prCA = new double[DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1]; // L1/CA pseudrange/doppler for javad
        public double[] dpCA = new double[DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1];
        public byte[,] halfc = new byte[DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1, DefineConstants.NFREQ + DefineConstants.NEXOBS]; // half-cycle add flag
        public string freqn = new string(new char[DefineConstants.MAXOBS]); // frequency number for javad
        public int nbyte; // number of bytes in message buffer
        public int len; // message length (bytes)
        public int iod; // issue of data
        public int tod; // time of day (ms)
        public int tbase; // time base (0:gpst,1:utc(usno),2:glonass,3:utc(su)
        public int flag; // general purpose flag
        public int outtype; // output message type
        public byte[] buff = new byte[DefineConstants.MAXRAWLEN]; // message buffer
        public string opt = new string(new char[256]); // receiver dependent options
        public double receive_time; // RT17: Reiceve time of week for week rollover detection
        public uint plen; // RT17: Total size of packet to be read
        public uint pbyte; // RT17: How many packet bytes have been read so far
        public uint page; // RT17: Last page number
        public uint reply; // RT17: Current reply number
        public int week; // RT17: week number
        public byte[] pbuff = new byte[255 + 4 + 2]; // RT17: Packet buffer
    }

    public class stream_t // stream type
    {
        public int type; // type (STR_???)
        public int mode; // mode (STR_MODE_?)
        public int state; // state (-1:error,0:close,1:open)
        public uint inb; // input bytes/rate
        public uint inr;
        public uint outb; // output bytes/rate
        public uint outr;
        public uint tick; // tick/active tick
        public uint tact;
        public uint inbt; // input/output bytes at tick
        public uint outbt;
#if lock_t_ConditionalDefinition1
	public CRITICAL_SECTION @lock = new CRITICAL_SECTION();
#elif lock_t_ConditionalDefinition2
	public pthread_mutex_t @lock = new pthread_mutex_t();
#else
        public lock_t @lock = new lock_t();
#endif
        public object port; // type dependent port control struct
        public string path = new string(new char[DefineConstants.MAXSTRPATH]); // stream path
        public string msg = new string(new char[DefineConstants.MAXSTRMSG]); // stream message
    }

    public class strconv_t // stream converter type
    {
        public int itype; // input and output stream type
        public int otype;
        public int nmsg; // number of output messages
        public int[] msgs = new int[32]; // output message types
        public double[] tint = new double[32]; // output message intervals (s)
        public uint[] tick = new uint[32]; // cycle tick of output message
        public int[] ephsat = new int[32]; // satellites of output ephemeris
        public int stasel; // station info selection (0:remote,1:local)
        public rtcm_t rtcm = new rtcm_t(); // rtcm input data buffer
        public raw_t raw = new raw_t(); // raw  input data buffer
        public rtcm_t @out = new rtcm_t(); // rtcm output data buffer
    }

    public class strsvr_t // stream server type
    {
        public int state; // server state (0:stop,1:running)
        public int cycle; // server cycle (ms)
        public int buffsize; // input/monitor buffer size (bytes)
        public int nmeacycle; // NMEA request cycle (ms) (0:no)
        public int nstr; // number of streams (1 input + (nstr-1) outputs
        public int npb; // data length in peek buffer (bytes)
        public double[] nmeapos = new double[3]; // NMEA request position (ecef) (m)
                                                 //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
                                                 //ORIGINAL LINE: byte *buff;
        public byte buff; // input buffers
                          //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
                          //ORIGINAL LINE: byte *pbuf;
        public byte pbuf; // peek buffer
        public uint tick; // start tick
        public stream_t[] stream = Arrays.InitializeWithDefaultInstances<stream_t>(16); // input/output streams
        public strconv_t[] conv = Arrays.InitializeWithDefaultInstances<strconv_t>(16); // stream converter
#if thread_t_ConditionalDefinition1
	public System.IntPtr thread;
#elif thread_t_ConditionalDefinition2
	public pthread_t thread = new pthread_t();
#else
        public thread_t thread = new thread_t();
#endif
#if lock_t_ConditionalDefinition1
	public CRITICAL_SECTION @lock = new CRITICAL_SECTION();
#elif lock_t_ConditionalDefinition2
	public pthread_mutex_t @lock = new pthread_mutex_t();
#else
        public lock_t @lock = new lock_t();
#endif
    }

    public class rtksvr_t // RTK server type
    {
        public int state; // server state (0:stop,1:running)
        public int cycle; // processing cycle (ms)
        public int nmeacycle; // NMEA request cycle (ms) (0:no req)
        public int nmeareq; // NMEA request (0:no,1:nmeapos,2:single sol)
        public double[] nmeapos = new double[3]; // NMEA request position (ecef) (m)
        public int buffsize; // input buffer size (bytes)
        public int[] format = new int[3]; // input format {rov,base,corr}
        public solopt_t[] solopt = Arrays.InitializeWithDefaultInstances<solopt_t>(2); // output solution options {sol1,sol2}
        public int navsel; // ephemeris select (0:all,1:rover,2:base,3:corr)
        public int nsbs; // number of sbas message
        public int nsol; // number of solution buffer
        public rtk_t rtk = new rtk_t(); // RTK control/result struct
        public int[] nb = new int[3]; // bytes in input buffers {rov,base}
        public int[] nsb = new int[2]; // bytes in soulution buffers
        public int[] npb = new int[3]; // bytes in input peek buffers
                                       //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
                                       //ORIGINAL LINE: byte *buff[3];
        public byte[] buff = new byte[3]; // input buffers {rov,base,corr}
                                          //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
                                          //ORIGINAL LINE: byte *sbuf[2];
        public byte[] sbuf = new byte[2]; // output buffers {sol1,sol2}
                                          //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
                                          //ORIGINAL LINE: byte *pbuf[3];
        public byte[] pbuf = new byte[3]; // peek buffers {rov,base,corr}
        public sol_t[] solbuf = Arrays.InitializeWithDefaultInstances<sol_t>(DefineConstants.MAXSOLBUF); // solution buffer
        public uint[,] nmsg = new uint[3, 10]; // input message counts
        public raw_t[] raw = Arrays.InitializeWithDefaultInstances<raw_t>(3); // receiver raw control {rov,base,corr}
        public rtcm_t[] rtcm = Arrays.InitializeWithDefaultInstances<rtcm_t>(3); // RTCM control {rov,base,corr}
        public gtime_t[] ftime = Arrays.InitializeWithDefaultInstances<gtime_t>(3); // download time {rov,base,corr}
        public sbyte[,] files = new sbyte[3, DefineConstants.MAXSTRPATH]; // download paths {rov,base,corr}
        public obs_t[,] obs = new obs_t[3, DefineConstants.MAXOBSBUF]; // observation data {rov,base,corr}
        public nav_t nav = new nav_t(); // navigation data
        public sbsmsg_t[] sbsmsg = Arrays.InitializeWithDefaultInstances<sbsmsg_t>(DefineConstants.MAXSBSMSG); // SBAS message buffer
        public stream_t[] stream = Arrays.InitializeWithDefaultInstances<stream_t>(8); // streams {rov,base,corr,sol1,sol2,logr,logb,logc}
        public stream_t moni; // monitor stream
        public uint tick; // start tick
#if thread_t_ConditionalDefinition1
	public System.IntPtr thread;
#elif thread_t_ConditionalDefinition2
	public pthread_t thread = new pthread_t();
#else
        public thread_t thread = new thread_t();
#endif
        public int cputime; // CPU time (ms) for a processing cycle
        public int prcout; // missing observation data count
#if lock_t_ConditionalDefinition1
	public CRITICAL_SECTION @lock = new CRITICAL_SECTION();
#elif lock_t_ConditionalDefinition2
	public pthread_mutex_t @lock = new pthread_mutex_t();
#else
        public lock_t @lock = new lock_t();
#endif
    }

#if __cplusplus
#endif

