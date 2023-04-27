using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ghGPS.Classes
{
    public static class GlobalMembersRtkcmn
    {
        /*------------------------------------------------------------------------------
        * rtkcmn.c : rtklib common functions
        *
        *          Copyright (C) 2007-2015 by T.TAKASU, All rights reserved.
        *
        * options : -DLAPACK   use LAPACK/BLAS
        *           -DMKL      use Intel MKL
        *           -DTRACE    enable debug trace
        *           -DWIN32    use WIN32 API
        *           -DNOCALLOC no use calloc for zero matrix
        *           -DIERS_MODEL use GMF instead of NMF
        *           -DDLL      built for shared library
        *
        * references :
        *     [1] IS-GPS-200D, Navstar GPS Space Segment/Navigation User Interfaces,
        *         7 March, 2006
        *     [2] RTCA/DO-229C, Minimum operational performanc standards for global
        *         positioning system/wide area augmentation system airborne equipment,
        *         RTCA inc, November 28, 2001
        *     [3] M.Rothacher, R.Schmid, ANTEX: The Antenna Exchange Format Version 1.4,
        *         15 September, 2010
        *     [4] A.Gelb ed., Applied Optimal Estimation, The M.I.T Press, 1974
        *     [5] A.E.Niell, Global mapping functions for the atmosphere delay at radio
        *         wavelengths, Jounal of geophysical research, 1996
        *     [6] W.Gurtner and L.Estey, RINEX The Receiver Independent Exchange Format
        *         Version 3.00, November 28, 2007
        *     [7] J.Kouba, A Guide to using International GNSS Service (IGS) products,
        *         May 2009
        *     [8] China Satellite Navigation Office, BeiDou navigation satellite system
        *         signal in space interface control document, open service signal B1I
        *         (version 1.0), Dec 2012
        *     [9] J.Boehm, A.Niell, P.Tregoning and H.Shuh, Global Mapping Function
        *         (GMF): A new empirical mapping function base on numerical weather
        *         model data, Geophysical Research Letters, 33, L07304, 2006
        *     [10] GLONASS/GPS/Galileo/Compass/SBAS NV08C receiver series BINR interface
        *         protocol specification ver.1.3, August, 2012
        *
        * version : $Revision: 1.1 $ $Date: 2008/07/17 21:48:06 $
        * history : 2007/01/12 1.0 new
        *           2007/03/06 1.1 input initial rover pos of pntpos()
        *                          update only effective states of filter()
        *                          fix bug of atan2() domain error
        *           2007/04/11 1.2 add function antmodel()
        *                          add gdop mask for pntpos()
        *                          change constant MAXDTOE value
        *           2007/05/25 1.3 add function execcmd(),expandpath()
        *           2008/06/21 1.4 add funciton sortobs(),uniqeph(),screent()
        *                          replace geodist() by sagnac correction way
        *           2008/10/29 1.5 fix bug of ionosphereic mapping function
        *                          fix bug of seasonal variation term of tropmapf
        *           2008/12/27 1.6 add function tickget(), sleepms(), tracenav(),
        *                          xyz2enu(), satposv(), pntvel(), covecef()
        *           2009/03/12 1.7 fix bug on error-stop when localtime() returns NULL
        *           2009/03/13 1.8 fix bug on time adjustment for summer time
        *           2009/04/10 1.9 add function adjgpsweek(),getbits(),getbitu()
        *                          add function geph2pos()
        *           2009/06/08 1.10 add function seph2pos()
        *           2009/11/28 1.11 change function pntpos()
        *                           add function tracegnav(),tracepeph()
        *           2009/12/22 1.12 change default parameter of ionos std
        *                           valid under second for timeget()
        *           2010/07/28 1.13 fix bug in tropmapf()
        *                           added api:
        *                               obs2code(),code2obs(),cross3(),normv3(),
        *                               gst2time(),time2gst(),time_str(),timeset(),
        *                               deg2dms(),dms2deg(),searchpcv(),antmodel_s(),
        *                               tracehnav(),tracepclk(),reppath(),reppaths(),
        *                               createdir()
        *                           changed api:
        *                               readpcv(),
        *                           deleted api:
        *                               uniqeph()
        *           2010/08/20 1.14 omit to include mkl header files
        *                           fix bug on chi-sqr(n) table
        *           2010/12/11 1.15 added api:
        *                               freeobs(),freenav(),ionppp()
        *           2011/05/28 1.16 fix bug on half-hour offset by time2epoch()
        *                           added api:
        *                               uniqnav()
        *           2012/06/09 1.17 add a leap second after 2012-6-30
        *           2012/07/15 1.18 add api setbits(),setbitu(),utc2gmst()
        *                           fix bug on interpolation of antenna pcv
        *                           fix bug on str2num() for string with over 256 char
        *                           add api readblq(),satexclude(),setcodepri(),
        *                           getcodepri()
        *                           change api obs2code(),code2obs(),antmodel()
        *           2012/12/25 1.19 fix bug on satwavelen(),code2obs(),obs2code()
        *                           add api testsnr()
        *           2013/01/04 1.20 add api gpst2bdt(),bdt2gpst(),bdt2time(),time2bdt()
        *                           readblq(),readerp(),geterp(),crc16()
        *                           change api eci2ecef(),sunmoonpos()
        *           2013/03/26 1.21 tickget() uses clock_gettime() for linux
        *           2013/05/08 1.22 fix bug on nutation coefficients for ast_args()
        *           2013/06/02 1.23 add #ifdef for undefined CLOCK_MONOTONIC_RAW
        *           2013/09/01 1.24 fix bug on interpolation of satellite antenna pcv
        *           2013/09/06 1.25 fix bug on extrapolation of erp
        *           2014/04/27 1.26 add SYS_LEO for satellite system
        *                           add BDS L1 code for RINEX 3.02 and RTCM 3.2
        *                           support BDS L1 in satwavelen()
        *           2014/05/29 1.27 fix bug on obs2code() to search obs code table
        *           2014/08/26 1.28 fix problem on output of uncompress() for tar file
        *                           add function to swap trace file with keywords
        *           2014/10/21 1.29 strtok() -> strtok_r() in expath() for thread-safe
        *                           add bdsmodear in procopt_default
        *           2015/03/19 1.30 fix bug on interpolation of erp values in geterp()
        *                           add leap second insertion before 2015/07/01 00:00
        *                           add api read_leaps()
        *           2017/01/03 1.31 add leap second before 2017/1/1 00:00:00
        *-----------------------------------------------------------------------------*/


        internal const string rcsid = "$Id: rtkcmn.c,v 1.1 2008/07/17 21:48:06 ttaka Exp ttaka $";



        internal readonly double[] gpst0 = { 1980, 1, 6, 0, 0, 0 }; // gps time reference
        internal readonly double[] gst0 = { 1999, 8, 22, 0, 0, 0 }; // galileo system time reference
        internal readonly double[] bdt0 = { 2006, 1, 1, 0, 0, 0 }; // beidou time reference

        internal static double[,] leaps = { { 2017, 1, 1, 0, 0, 0, -18 }, { 2015, 7, 1, 0, 0, 0, -17 }, { 2012, 7, 1, 0, 0, 0, -16 }, { 2009, 1, 1, 0, 0, 0, -15 }, { 2006, 1, 1, 0, 0, 0, -14 }, { 1999, 1, 1, 0, 0, 0, -13 }, { 1997, 7, 1, 0, 0, 0, -12 }, { 1996, 1, 1, 0, 0, 0, -11 }, { 1994, 7, 1, 0, 0, 0, -10 }, { 1993, 7, 1, 0, 0, 0, -9 }, { 1992, 7, 1, 0, 0, 0, -8 }, { 1991, 1, 1, 0, 0, 0, -7 }, { 1990, 1, 1, 0, 0, 0, -6 }, { 1988, 1, 1, 0, 0, 0, -5 }, { 1985, 7, 1, 0, 0, 0, -4 }, { 1983, 7, 1, 0, 0, 0, -3 }, { 1982, 7, 1, 0, 0, 0, -2 }, { 1981, 7, 1, 0, 0, 0, -1 }, { 0, null, null, null, null, null, null } }; // leap seconds (y,m,d,h,m,s,utc-gpst)
        public static readonly double[] chisqr = { 10.8, 13.8, 16.3, 18.5, 20.5, 22.5, 24.3, 26.1, 27.9, 29.6, 31.3, 32.9, 34.5, 36.1, 37.7, 39.3, 40.8, 42.3, 43.8, 45.3, 46.8, 48.3, 49.7, 51.2, 52.6, 54.1, 55.5, 56.9, 58.3, 59.7, 61.1, 62.5, 63.9, 65.2, 66.6, 68.0, 69.3, 70.7, 72.1, 73.4, 74.7, 76.0, 77.3, 78.6, 80.0, 81.3, 82.6, 84.0, 85.4, 86.7, 88.0, 89.3, 90.6, 91.9, 93.3, 94.7, 96.0, 97.4, 98.7, 100, 101, 102, 103, 104, 105, 107, 108, 109, 110, 112, 113, 114, 115, 116, 118, 119, 120, 122, 123, 125, 126, 127, 128, 129, 131, 132, 133, 134, 135, 137, 138, 139, 140, 142, 143, 144, 145, 147, 148, 149 }; // chi-sqr(n) (alpha=0.001)
        public static readonly double[] lam_carr = { DefineConstants.CLIGHT / DefineConstants.FREQ1, DefineConstants.CLIGHT / DefineConstants.FREQ2, DefineConstants.CLIGHT / DefineConstants.FREQ5, DefineConstants.CLIGHT / DefineConstants.FREQ6, DefineConstants.CLIGHT / DefineConstants.FREQ7, DefineConstants.CLIGHT / DefineConstants.FREQ8 }; // carrier wave length (m)
        public const prcopt_t prcopt_default = new prcopt_t(DefineConstants.PMODE_SINGLE, 0, 2, DefineConstants.SYS_GPS, 15.0 * DefineConstants.PI / 180.0,{{0, 0}
}, 0,1,1,1, 5,0,10, 0,0,0,0, 1,0,0,0,0, 0,0, {100.0,100.0}, {100.0,0.003,0.003,0.0,1.0}, {30.0,0.03,0.3}, {1E-4,1E-3,1E-4,1E-1,1E-2}, 5E-12, {3.0,0.9999,0.20}, 0.0,0.0,0.05, 30.0,30.0,30.0, {0},{0},{0}, {"",""}, {{0}},{{0}},{0}); // defaults processing options
	public const solopt_t solopt_default = new solopt_t(DefineConstants.SOLF_LLH, DefineConstants.TIMES_GPST, 1, 3, 0, 1, 0, 0, 0, 0, 0, 0, 0, {0.0, 0.0}, " ",""); // defaults solution output options
	public static string[] formatstrs = { "RTCM 2", "RTCM 3", "NovAtel OEM6", "NovAtel OEM3", "u-blox", "Superstar II", "Hemisphere", "SkyTraq", "GW10", "Javad", "NVS BINR", "BINEX", "Trimble RT17", "LEX Receiver", "Septentrio", "RINEX", "SP3", "RINEX CLK", "SBAS", "NMEA 0183", null }; // stream format strings
internal static string[] obscodes = { "", "1C", "1P", "1W", "1Y", "1M", "1N", "1S", "1L", "1E", "1A", "1B", "1X", "1Z", "2C", "2D", "2S", "2L", "2X", "2P", "2W", "2Y", "2M", "2N", "5I", "5Q", "5X", "7I", "7Q", "7X", "6A", "6B", "6C", "6X", "6Z", "6S", "6L", "8L", "8Q", "8X", "2I", "2Q", "6I", "6Q", "3I", "3Q", "3X", "1I", "1Q", "" }; // observation code strings
internal static byte[] obsfreqs = { 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 3, 3, 3, 5, 5, 5, 4, 4, 4, 4, 4, 4, 4, 6, 6, 6, 2, 2, 4, 4, 3, 3, 3, 1, 1, 0 }; // 1:L1,2:L2,3:L5,4:L6,5:L7,6:L8,7:L3
internal static sbyte[,,] codepris = { { "CPYWMNSL", "PYWCMNDSLX", "IQX", "", "", "" }, { "PC", "PC", "IQX", "", "", "" }, { "CABXZ", "", "IQX", "ABCXZ", "IQX", "IQX" }, { "CSLXZ", "SLX", "IQX", "SLX", "", "" }, { "C", "", "IQX", "", "", "" }, { "IQX", "IQX", "IQX", "IQX", "IQX", "" } }; // code priority table
                                                                                                                                                                                                                                                                                                 /* crc tables generated by util/gencrc ---------------------------------------*/
internal readonly ushort[] tbl_CRC16 = { 0x0000, 0x1021, 0x2042, 0x3063, 0x4084, 0x50A5, 0x60C6, 0x70E7, 0x8108, 0x9129, 0xA14A, 0xB16B, 0xC18C, 0xD1AD, 0xE1CE, 0xF1EF, 0x1231, 0x0210, 0x3273, 0x2252, 0x52B5, 0x4294, 0x72F7, 0x62D6, 0x9339, 0x8318, 0xB37B, 0xA35A, 0xD3BD, 0xC39C, 0xF3FF, 0xE3DE, 0x2462, 0x3443, 0x0420, 0x1401, 0x64E6, 0x74C7, 0x44A4, 0x5485, 0xA56A, 0xB54B, 0x8528, 0x9509, 0xE5EE, 0xF5CF, 0xC5AC, 0xD58D, 0x3653, 0x2672, 0x1611, 0x0630, 0x76D7, 0x66F6, 0x5695, 0x46B4, 0xB75B, 0xA77A, 0x9719, 0x8738, 0xF7DF, 0xE7FE, 0xD79D, 0xC7BC, 0x48C4, 0x58E5, 0x6886, 0x78A7, 0x0840, 0x1861, 0x2802, 0x3823, 0xC9CC, 0xD9ED, 0xE98E, 0xF9AF, 0x8948, 0x9969, 0xA90A, 0xB92B, 0x5AF5, 0x4AD4, 0x7AB7, 0x6A96, 0x1A71, 0x0A50, 0x3A33, 0x2A12, 0xDBFD, 0xCBDC, 0xFBBF, 0xEB9E, 0x9B79, 0x8B58, 0xBB3B, 0xAB1A, 0x6CA6, 0x7C87, 0x4CE4, 0x5CC5, 0x2C22, 0x3C03, 0x0C60, 0x1C41, 0xEDAE, 0xFD8F, 0xCDEC, 0xDDCD, 0xAD2A, 0xBD0B, 0x8D68, 0x9D49, 0x7E97, 0x6EB6, 0x5ED5, 0x4EF4, 0x3E13, 0x2E32, 0x1E51, 0x0E70, 0xFF9F, 0xEFBE, 0xDFDD, 0xCFFC, 0xBF1B, 0xAF3A, 0x9F59, 0x8F78, 0x9188, 0x81A9, 0xB1CA, 0xA1EB, 0xD10C, 0xC12D, 0xF14E, 0xE16F, 0x1080, 0x00A1, 0x30C2, 0x20E3, 0x5004, 0x4025, 0x7046, 0x6067, 0x83B9, 0x9398, 0xA3FB, 0xB3DA, 0xC33D, 0xD31C, 0xE37F, 0xF35E, 0x02B1, 0x1290, 0x22F3, 0x32D2, 0x4235, 0x5214, 0x6277, 0x7256, 0xB5EA, 0xA5CB, 0x95A8, 0x8589, 0xF56E, 0xE54F, 0xD52C, 0xC50D, 0x34E2, 0x24C3, 0x14A0, 0x0481, 0x7466, 0x6447, 0x5424, 0x4405, 0xA7DB, 0xB7FA, 0x8799, 0x97B8, 0xE75F, 0xF77E, 0xC71D, 0xD73C, 0x26D3, 0x36F2, 0x0691, 0x16B0, 0x6657, 0x7676, 0x4615, 0x5634, 0xD94C, 0xC96D, 0xF90E, 0xE92F, 0x99C8, 0x89E9, 0xB98A, 0xA9AB, 0x5844, 0x4865, 0x7806, 0x6827, 0x18C0, 0x08E1, 0x3882, 0x28A3, 0xCB7D, 0xDB5C, 0xEB3F, 0xFB1E, 0x8BF9, 0x9BD8, 0xABBB, 0xBB9A, 0x4A75, 0x5A54, 0x6A37, 0x7A16, 0x0AF1, 0x1AD0, 0x2AB3, 0x3A92, 0xFD2E, 0xED0F, 0xDD6C, 0xCD4D, 0xBDAA, 0xAD8B, 0x9DE8, 0x8DC9, 0x7C26, 0x6C07, 0x5C64, 0x4C45, 0x3CA2, 0x2C83, 0x1CE0, 0x0CC1, 0xEF1F, 0xFF3E, 0xCF5D, 0xDF7C, 0xAF9B, 0xBFBA, 0x8FD9, 0x9FF8, 0x6E17, 0x7E36, 0x4E55, 0x5E74, 0x2E93, 0x3EB2, 0x0ED1, 0x1EF0 };
internal readonly uint[] tbl_CRC24Q = { 0x000000, 0x864CFB, 0x8AD50D, 0x0C99F6, 0x93E6E1, 0x15AA1A, 0x1933EC, 0x9F7F17, 0xA18139, 0x27CDC2, 0x2B5434, 0xAD18CF, 0x3267D8, 0xB42B23, 0xB8B2D5, 0x3EFE2E, 0xC54E89, 0x430272, 0x4F9B84, 0xC9D77F, 0x56A868, 0xD0E493, 0xDC7D65, 0x5A319E, 0x64CFB0, 0xE2834B, 0xEE1ABD, 0x685646, 0xF72951, 0x7165AA, 0x7DFC5C, 0xFBB0A7, 0x0CD1E9, 0x8A9D12, 0x8604E4, 0x00481F, 0x9F3708, 0x197BF3, 0x15E205, 0x93AEFE, 0xAD50D0, 0x2B1C2B, 0x2785DD, 0xA1C926, 0x3EB631, 0xB8FACA, 0xB4633C, 0x322FC7, 0xC99F60, 0x4FD39B, 0x434A6D, 0xC50696, 0x5A7981, 0xDC357A, 0xD0AC8C, 0x56E077, 0x681E59, 0xEE52A2, 0xE2CB54, 0x6487AF, 0xFBF8B8, 0x7DB443, 0x712DB5, 0xF7614E, 0x19A3D2, 0x9FEF29, 0x9376DF, 0x153A24, 0x8A4533, 0x0C09C8, 0x00903E, 0x86DCC5, 0xB822EB, 0x3E6E10, 0x32F7E6, 0xB4BB1D, 0x2BC40A, 0xAD88F1, 0xA11107, 0x275DFC, 0xDCED5B, 0x5AA1A0, 0x563856, 0xD074AD, 0x4F0BBA, 0xC94741, 0xC5DEB7, 0x43924C, 0x7D6C62, 0xFB2099, 0xF7B96F, 0x71F594, 0xEE8A83, 0x68C678, 0x645F8E, 0xE21375, 0x15723B, 0x933EC0, 0x9FA736, 0x19EBCD, 0x8694DA, 0x00D821, 0x0C41D7, 0x8A0D2C, 0xB4F302, 0x32BFF9, 0x3E260F, 0xB86AF4, 0x2715E3, 0xA15918, 0xADC0EE, 0x2B8C15, 0xD03CB2, 0x567049, 0x5AE9BF, 0xDCA544, 0x43DA53, 0xC596A8, 0xC90F5E, 0x4F43A5, 0x71BD8B, 0xF7F170, 0xFB6886, 0x7D247D, 0xE25B6A, 0x641791, 0x688E67, 0xEEC29C, 0x3347A4, 0xB50B5F, 0xB992A9, 0x3FDE52, 0xA0A145, 0x26EDBE, 0x2A7448, 0xAC38B3, 0x92C69D, 0x148A66, 0x181390, 0x9E5F6B, 0x01207C, 0x876C87, 0x8BF571, 0x0DB98A, 0xF6092D, 0x7045D6, 0x7CDC20, 0xFA90DB, 0x65EFCC, 0xE3A337, 0xEF3AC1, 0x69763A, 0x578814, 0xD1C4EF, 0xDD5D19, 0x5B11E2, 0xC46EF5, 0x42220E, 0x4EBBF8, 0xC8F703, 0x3F964D, 0xB9DAB6, 0xB54340, 0x330FBB, 0xAC70AC, 0x2A3C57, 0x26A5A1, 0xA0E95A, 0x9E1774, 0x185B8F, 0x14C279, 0x928E82, 0x0DF195, 0x8BBD6E, 0x872498, 0x016863, 0xFAD8C4, 0x7C943F, 0x700DC9, 0xF64132, 0x693E25, 0xEF72DE, 0xE3EB28, 0x65A7D3, 0x5B59FD, 0xDD1506, 0xD18CF0, 0x57C00B, 0xC8BF1C, 0x4EF3E7, 0x426A11, 0xC426EA, 0x2AE476, 0xACA88D, 0xA0317B, 0x267D80, 0xB90297, 0x3F4E6C, 0x33D79A, 0xB59B61, 0x8B654F, 0x0D29B4, 0x01B042, 0x87FCB9, 0x1883AE, 0x9ECF55, 0x9256A3, 0x141A58, 0xEFAAFF, 0x69E604, 0x657FF2, 0xE33309, 0x7C4C1E, 0xFA00E5, 0xF69913, 0x70D5E8, 0x4E2BC6, 0xC8673D, 0xC4FECB, 0x42B230, 0xDDCD27, 0x5B81DC, 0x57182A, 0xD154D1, 0x26359F, 0xA07964, 0xACE092, 0x2AAC69, 0xB5D37E, 0x339F85, 0x3F0673, 0xB94A88, 0x87B4A6, 0x01F85D, 0x0D61AB, 0x8B2D50, 0x145247, 0x921EBC, 0x9E874A, 0x18CBB1, 0xE37B16, 0x6537ED, 0x69AE1B, 0xEFE2E0, 0x709DF7, 0xF6D10C, 0xFA48FA, 0x7C0401, 0x42FA2F, 0xC4B6D4, 0xC82F22, 0x4E63D9, 0xD11CCE, 0x575035, 0x5BC9C3, 0xDD8538 };
/* function prototypes -------------------------------------------------------*/


/* fatal error ---------------------------------------------------------------*/
internal static void fatalerr(string format, params object[] LegacyParamArray)
{
    //	va_list ap;
    int ParamCount = -1;
    //	va_start(ap,format);
    vfprintf(stderr, format, ap);
    //	va_end(ap);
    Environment.Exit(-9);
}
/* satellite system+prn/slot number to satellite number ------------------------
* convert satellite system+prn/slot number to satellite number
* args   : int    sys       I   satellite system (SYS_GPS,SYS_GLO,...)
*          int    prn       I   satellite prn/slot number
* return : satellite number (0:error)
*-----------------------------------------------------------------------------*/
public static int satno(int sys, int prn)
{
    if (prn <= 0)
    {
        return 0;
    }
    switch (sys)
    {
        case DefineConstants.SYS_GPS:
            if (prn < DefineConstants.MINPRNGPS || DefineConstants.MAXPRNGPS < prn)
            {
                return 0;
            }
            return prn - DefineConstants.MINPRNGPS + 1;
        case DefineConstants.SYS_GLO:
            if (prn < DefineConstants.MINPRNGLO || DefineConstants.MAXPRNGLO < prn)
            {
                return 0;
            }
            return DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + prn - DefineConstants.MINPRNGLO + 1;
        case DefineConstants.SYS_GAL:
            if (prn < DefineConstants.MINPRNGAL || DefineConstants.MAXPRNGAL < prn)
            {
                return 0;
            }
            return DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + prn - DefineConstants.MINPRNGAL + 1;
        case DefineConstants.SYS_QZS:
            if (prn < DefineConstants.MINPRNQZS || DefineConstants.MAXPRNQZS < prn)
            {
                return 0;
            }
            return DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + prn - DefineConstants.MINPRNQZS + 1;
        case DefineConstants.SYS_CMP:
            if (prn < DefineConstants.MINPRNCMP || DefineConstants.MAXPRNCMP < prn)
            {
                return 0;
            }
            return DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + prn - DefineConstants.MINPRNCMP + 1;
        case DefineConstants.SYS_LEO:
            if (prn < DefineConstants.MINPRNLEO || DefineConstants.MAXPRNLEO < prn)
            {
                return 0;
            }
            return DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + prn - DefineConstants.MINPRNLEO + 1;
        case DefineConstants.SYS_SBS:
            if (prn < DefineConstants.MINPRNSBS || DefineConstants.MAXPRNSBS < prn)
            {
                return 0;
            }
            return DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1 + prn - DefineConstants.MINPRNSBS + 1;
    }
    return 0;
}
/* satellite number to satellite system ----------------------------------------
* convert satellite number to satellite system
* args   : int    sat       I   satellite number (1-MAXSAT)
*          int    *prn      IO  satellite prn/slot number (NULL: no output)
* return : satellite system (SYS_GPS,SYS_GLO,...)
*-----------------------------------------------------------------------------*/
public static int satsys(int sat, ref int prn)
{
    int sys = DefineConstants.SYS_NONE;
    if (sat <= 0 || DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1 < sat)
    {
        sat = 0;
    }
    else if (sat <= DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1)
    {
        sys = DefineConstants.SYS_GPS;
        sat += DefineConstants.MINPRNGPS - 1;
    }
    else if ((sat -= DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1) <= DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1)
    {
        sys = DefineConstants.SYS_GLO;
        sat += DefineConstants.MINPRNGLO - 1;
    }
    else if ((sat -= DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1) <= DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1)
    {
        sys = DefineConstants.SYS_GAL;
        sat += DefineConstants.MINPRNGAL - 1;
    }
    else if ((sat -= DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1) <= DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1)
    {
        sys = DefineConstants.SYS_QZS;
        sat += DefineConstants.MINPRNQZS - 1;
    }
    else if ((sat -= DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1) <= DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1)
    {
        sys = DefineConstants.SYS_CMP;
        sat += DefineConstants.MINPRNCMP - 1;
    }
    else if ((sat -= DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1) <= DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1)
    {
        sys = DefineConstants.SYS_LEO;
        sat += DefineConstants.MINPRNLEO - 1;
    }
    else if ((sat -= DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1) <= DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1)
    {
        sys = DefineConstants.SYS_SBS;
        sat += DefineConstants.MINPRNSBS - 1;
    }
    else
    {
        sat = 0;
    }
    if (prn != 0)
    {
        prn = sat;
    }
    return sys;
}
/* satellite id to satellite number --------------------------------------------
* convert satellite id to satellite number
* args   : char   *id       I   satellite id (nn,Gnn,Rnn,Enn,Jnn,Cnn or Snn)
* return : satellite number (0: error)
* notes  : 120-138 and 193-195 are also recognized as sbas and qzss
*-----------------------------------------------------------------------------*/
public static int satid2no(string id)
{
    int sys;
    int prn;
    sbyte code;

    if (sscanf(id, "%d", prn) == 1)
    {
        if (DefineConstants.MINPRNGPS <= prn && prn <= DefineConstants.MAXPRNGPS)
        {
            sys = DefineConstants.SYS_GPS;
        }
        else if (DefineConstants.MINPRNSBS <= prn && prn <= DefineConstants.MAXPRNSBS)
        {
            sys = DefineConstants.SYS_SBS;
        }
        else if (DefineConstants.MINPRNQZS <= prn && prn <= DefineConstants.MAXPRNQZS)
        {
            sys = DefineConstants.SYS_QZS;
        }
        else
        {
            return 0;
        }
        return GlobalMembersRtkcmn.satno(sys, prn);
    }
    if (sscanf(id, "%c%d", code, prn) < 2)
    {
        return 0;
    }

    switch (code)
    {
        case 'G':
            sys = DefineConstants.SYS_GPS;
            prn += DefineConstants.MINPRNGPS - 1;
            break;
        case 'R':
            sys = DefineConstants.SYS_GLO;
            prn += DefineConstants.MINPRNGLO - 1;
            break;
        case 'E':
            sys = DefineConstants.SYS_GAL;
            prn += DefineConstants.MINPRNGAL - 1;
            break;
        case 'J':
            sys = DefineConstants.SYS_QZS;
            prn += DefineConstants.MINPRNQZS - 1;
            break;
        case 'C':
            sys = DefineConstants.SYS_CMP;
            prn += DefineConstants.MINPRNCMP - 1;
            break;
        case 'L':
            sys = DefineConstants.SYS_LEO;
            prn += DefineConstants.MINPRNLEO - 1;
            break;
        case 'S':
            sys = DefineConstants.SYS_SBS;
            prn += 100;
            break;
        default:
            return 0;
    }
    return GlobalMembersRtkcmn.satno(sys, prn);
}
/* satellite number to satellite id --------------------------------------------
* convert satellite number to satellite id
* args   : int    sat       I   satellite number
*          char   *id       O   satellite id (Gnn,Rnn,Enn,Jnn,Cnn or nnn)
* return : none
*-----------------------------------------------------------------------------*/
public static void satno2id(int sat, ref string id)
{
    int prn;
    switch (GlobalMembersRtkcmn.satsys(sat, ref prn))
    {
        case DefineConstants.SYS_GPS:
            id = string.Format("G{0:D2}", prn - DefineConstants.MINPRNGPS + 1);
            return;
        case DefineConstants.SYS_GLO:
            id = string.Format("R{0:D2}", prn - DefineConstants.MINPRNGLO + 1);
            return;
        case DefineConstants.SYS_GAL:
            id = string.Format("E{0:D2}", prn - DefineConstants.MINPRNGAL + 1);
            return;
        case DefineConstants.SYS_QZS:
            id = string.Format("J{0:D2}", prn - DefineConstants.MINPRNQZS + 1);
            return;
        case DefineConstants.SYS_CMP:
            id = string.Format("C{0:D2}", prn - DefineConstants.MINPRNCMP + 1);
            return;
        case DefineConstants.SYS_LEO:
            id = string.Format("L{0:D2}", prn - DefineConstants.MINPRNLEO + 1);
            return;
        case DefineConstants.SYS_SBS:
            id = string.Format("{0:D3}", prn);
            return;
    }
    id = "";
}
/* test excluded satellite -----------------------------------------------------
* test excluded satellite
* args   : int    sat       I   satellite number
*          int    svh       I   sv health flag
*          prcopt_t *opt    I   processing options (NULL: not used)
* return : status (1:excluded,0:not excluded)
*-----------------------------------------------------------------------------*/
public static int satexclude(int sat, int svh, prcopt_t opt)
{
    int sys = GlobalMembersRtkcmn.satsys(sat, null);

    if (svh < 0) // ephemeris unavailable
    {
        return 1;
    }

    if (opt != null)
    {
        if (opt.exsats[sat - 1] == 1) // excluded satellite
        {
            return 1;
        }
        if (opt.exsats[sat - 1] == 2) // included satellite
        {
            return 0;
        }
        if (!(sys & opt.navsys)) // unselected sat sys
        {
            return 1;
        }
    }
    if (sys == DefineConstants.SYS_QZS) // mask QZSS LEX health
    {
        svh &= 0xFE;
    }
    if (svh != 0)
    {
        GlobalMembersRtkcmn.trace(3, "unhealthy satellite: sat=%3d svh=%02X\n", sat, svh);
        return 1;
    }
    return 0;
}
/* test SNR mask ---------------------------------------------------------------
* test SNR mask
* args   : int    base      I   rover or base-station (0:rover,1:base station)
*          int    freq      I   frequency (0:L1,1:L2,2:L3,...)
*          double el        I   elevation angle (rad)
*          double snr       I   C/N0 (dBHz)
*          snrmask_t *mask  I   SNR mask
* return : status (1:masked,0:unmasked)
*-----------------------------------------------------------------------------*/
public static int testsnr(int @base, int freq, double el, double snr, snrmask_t[] mask)
{
    double minsnr;
    double a;
    int i;

    if (mask.ena[@base] == 0 || freq < 0 || freq >= DefineConstants.NFREQ)
    {
        return 0;
    }

    a = (el * 180.0 / DefineConstants.PI + 5.0) / 10.0;
    i = (int)Math.Floor(a);
    a -= i;
    if (i < 1)
    {
        minsnr = mask.mask[freq, 0];
    }
    else if (i > 8)
    {
        minsnr = mask.mask[freq, 8];
    }
    else
    {
        minsnr = (1.0 - a) * mask.mask[freq, i - 1] + a * mask.mask[freq, i];
    }

    return snr < minsnr;
}
/* obs type string to obs code -------------------------------------------------
* convert obs code type string to obs code
* args   : char   *str   I      obs code string ("1C","1P","1Y",...)
*          int    *freq  IO     frequency (1:L1,2:L2,3:L5,4:L6,5:L7,6:L8,0:err)
*                               (NULL: no output)
* return : obs code (CODE_???)
* notes  : obs codes are based on reference [6] and qzss extension
*-----------------------------------------------------------------------------*/
public static byte obs2code(string obs, ref int freq)
{
    int i;
    if (freq != 0)
    {
        freq = 0;
    }
    for (i = 1; *obscodes[i]; i++)
    {
        if (string.Compare(obscodes[i], obs))
            continue;
        if (freq != 0)
        {
            freq = obsfreqs[i];
        }
        return (byte)i;
    }
    return DefineConstants.CODE_NONE;
}
/* obs code to obs code string -------------------------------------------------
* convert obs code to obs code string
* args   : unsigned char code I obs code (CODE_???)
*          int    *freq  IO     frequency (1:L1,2:L2,3:L5,4:L6,5:L7,6:L8,0:err)
*                               (NULL: no output)
* return : obs code string ("1C","1P","1P",...)
* notes  : obs codes are based on reference [6] and qzss extension
*-----------------------------------------------------------------------------*/
public static string code2obs(byte code, ref int freq)
{
    if (freq != 0)
    {
        freq = 0;
    }
    if (code <= DefineConstants.CODE_NONE || DefineConstants.MAXCODE < code)
    {
        return "";
    }
    if (freq != 0)
    {
        freq = obsfreqs[code];
    }
    return obscodes[code];
}
/* set code priority -----------------------------------------------------------
* set code priority for multiple codes in a frequency
* args   : int    sys     I     system (or of SYS_???)
*          int    freq    I     frequency (1:L1,2:L2,3:L5,4:L6,5:L7,6:L8)
*          char   *pri    I     priority of codes (series of code characters)
*                               (higher priority precedes lower)
* return : none
*-----------------------------------------------------------------------------*/
public static void setcodepri(int sys, int freq, string pri)
{
    GlobalMembersRtkcmn.trace(3, "setcodepri:sys=%d freq=%d pri=%s\n", sys, freq, pri);

    if (freq <= 0 || DefineConstants.MAXFREQ < freq)
        return;
    if ((sys & DefineConstants.SYS_GPS) != 0)
    {
        codepris[0, freq - 1] = pri;
    }
    if ((sys & DefineConstants.SYS_GLO) != 0)
    {
        codepris[1, freq - 1] = pri;
    }
    if ((sys & DefineConstants.SYS_GAL) != 0)
    {
        codepris[2, freq - 1] = pri;
    }
    if ((sys & DefineConstants.SYS_QZS) != 0)
    {
        codepris[3, freq - 1] = pri;
    }
    if ((sys & DefineConstants.SYS_SBS) != 0)
    {
        codepris[4, freq - 1] = pri;
    }
    if ((sys & DefineConstants.SYS_CMP) != 0)
    {
        codepris[5, freq - 1] = pri;
    }
}
/* get code priority -----------------------------------------------------------
* get code priority for multiple codes in a frequency
* args   : int    sys     I     system (SYS_???)
*          unsigned char code I obs code (CODE_???)
*          char   *opt    I     code options (NULL:no option)
* return : priority (15:highest-1:lowest,0:error)
*-----------------------------------------------------------------------------*/
public static int getcodepri(int sys, byte code, string opt)
{
    string p;
    string optstr;
    string obs;
    string str = "";
    int i;
    int j;

    switch (sys)
    {
        case DefineConstants.SYS_GPS:
            i = 0;
            optstr = "-GL%2s";
            break;
        case DefineConstants.SYS_GLO:
            i = 1;
            optstr = "-RL%2s";
            break;
        case DefineConstants.SYS_GAL:
            i = 2;
            optstr = "-EL%2s";
            break;
        case DefineConstants.SYS_QZS:
            i = 3;
            optstr = "-JL%2s";
            break;
        case DefineConstants.SYS_SBS:
            i = 4;
            optstr = "-SL%2s";
            break;
        case DefineConstants.SYS_CMP:
            i = 5;
            optstr = "-CL%2s";
            break;
        default:
            return 0;
    }
    obs = GlobalMembersRtkcmn.code2obs(code, ref j);

    /* parse code options */
    for (p = opt; p && (p = StringFunctions.StrChr(p, '-')); p++)
    {
        if (sscanf(p, optstr, str) < 1 || str[0] != obs[0])
            continue;
        return str[1] == obs[1] ? 15 : 0;
    }
    /* search code priority */
    return (p = StringFunctions.StrChr(codepris[i, j - 1], obs[1])) != 0 ? 14 - (int)(p - codepris[i, j - 1]) : 0;
}
/* extract unsigned/signed bits ------------------------------------------------
* extract unsigned/signed bits from byte data
* args   : unsigned char *buff I byte data
*          int    pos    I      bit position from start of data (bits)
*          int    len    I      bit length (bits) (len<=32)
* return : extracted unsigned/signed bits
*-----------------------------------------------------------------------------*/
public static uint getbitu(byte[] buff, int pos, int len)
{
    uint bits = 0;
    int i;
    for (i = pos; i < pos + len; i++)
    {
        bits = (bits << 1) + ((buff[i / 8] >> (7 - i % 8)) & 1u);
    }
    return bits;
}
public static int getbits(byte buff, int pos, int len)
{
    uint bits = GlobalMembersRtkcmn.getbitu(buff, pos, len);
    if (len <= 0 || 32 <= len || !(bits & (1u << (len - 1))))
    {
        return (int)bits;
    }
    return (int)(bits | (~0u << len)); // extend sign
}
/* set unsigned/signed bits ----------------------------------------------------
* set unsigned/signed bits to byte data
* args   : unsigned char *buff IO byte data
*          int    pos    I      bit position from start of data (bits)
*          int    len    I      bit length (bits) (len<=32)
*         (unsigned) int I      unsigned/signed data
* return : none
*-----------------------------------------------------------------------------*/
public static void setbitu(byte[] buff, int pos, int len, uint data)
{
    uint mask = 1u << (len - 1);
    int i;
    if (len <= 0 || 32 < len)
        return;
    for (i = pos; i < pos + len; i++, mask >>= 1)
    {
        if ((data & mask) != 0)
        {
            buff[i / 8] |= 1u << (7 - i % 8);
        }
        else
        {
            buff[i / 8] &= ~(1u << (7 - i % 8));
        }
    }
}
public static void setbits(ref byte buff, int pos, int len, int data)
{
    if (data < 0) // set sign bit
    {
        data |= 1 << (len - 1);
    }
    else
    {
        data &= ~(1 << (len - 1));
    }
    GlobalMembersRtkcmn.setbitu(buff, pos, len, (uint)data);
}
/* crc-32 parity ---------------------------------------------------------------
* compute crc-32 parity for novatel raw
* args   : unsigned char *buff I data
*          int    len    I      data length (bytes)
* return : crc-32 parity
* notes  : see NovAtel OEMV firmware manual 1.7 32-bit CRC
*-----------------------------------------------------------------------------*/
public static uint crc32(byte[] buff, int len)
{
    uint crc = 0;
    int i;
    int j;

    GlobalMembersRtkcmn.trace(4, "crc32: len=%d\n", len);

    for (i = 0; i < len; i++)
    {
        crc ^= buff[i];
        for (j = 0; j < 8; j++)
        {
            if ((crc & 1) != 0)
            {
                crc = (crc >> 1) ^ DefineConstants.POLYCRC32;
            }
            else
            {
                crc >>= 1;
            }
        }
    }
    return crc;
}
/* crc-24q parity --------------------------------------------------------------
* compute crc-24q parity for sbas, rtcm3
* args   : unsigned char *buff I data
*          int    len    I      data length (bytes)
* return : crc-24Q parity
* notes  : see reference [2] A.4.3.3 Parity
*-----------------------------------------------------------------------------*/
public static uint crc24q(byte[] buff, int len)
{
    uint crc = 0;
    int i;

    GlobalMembersRtkcmn.trace(4, "crc24q: len=%d\n", len);

    for (i = 0; i < len; i++)
    {
        crc = ((crc << 8) & 0xFFFFFF) ^ tbl_CRC24Q[(crc >> 16) ^ buff[i]];
    }
    return crc;
}
/* crc-16 parity ---------------------------------------------------------------
* compute crc-16 parity for binex, nvs
* args   : unsigned char *buff I data
*          int    len    I      data length (bytes)
* return : crc-16 parity
* notes  : see reference [10] A.3.
*-----------------------------------------------------------------------------*/
public static ushort crc16(byte[] buff, int len)
{
    ushort crc = 0;
    int i;

    GlobalMembersRtkcmn.trace(4, "crc16: len=%d\n", len);

    for (i = 0; i < len; i++)
    {
        crc = (crc << 8) ^ tbl_CRC16[((crc >> 8) ^ buff[i]) & 0xFF];
    }
    return crc;
}
/* decode navigation data word -------------------------------------------------
* check party and decode navigation data word
* args   : unsigned int word I navigation data word (2+30bit)
*                              (previous word D29*-30* + current word D1-30)
*          unsigned char *data O decoded navigation data without parity
*                              (8bitx3)
* return : status (1:ok,0:parity error)
* notes  : see reference [1] 20.3.5.2 user parity algorithm
*-----------------------------------------------------------------------------*/
public static int decode_word(uint word, byte[] data)
{
    uint[] hamming = { 0xBB1F3480, 0x5D8F9A40, 0xAEC7CD00, 0x5763E680, 0x6BB1F340, 0x8B7A89C0 };
    uint parity = 0;
    uint w;
    int i;

    GlobalMembersRtkcmn.trace(5, "decodeword: word=%08x\n", word);

    if ((word & 0x40000000) != 0)
    {
        word ^= 0x3FFFFFC0;
    }

    for (i = 0; i < 6; i++)
    {
        parity <<= 1;
        for (w = (word & hamming[i]) >> 6; w != 0; w >>= 1)
        {
            parity ^= w & 1;
        }
    }
    if (parity != (word & 0x3F))
    {
        return 0;
    }

    for (i = 0; i < 3; i++)
    {
        data[i] = (byte)(word >> (22 - i * 8));
    }
    return 1;
}
/* new matrix ------------------------------------------------------------------
* allocate memory of matrix 
* args   : int    n,m       I   number of rows and columns of matrix
* return : matrix pointer (if n<=0 or m<=0, return NULL)
*-----------------------------------------------------------------------------*/

public static double mat(int n, int m)
{

    double p;

    if (n <= 0 || m <= 0)
    {
        return null;
    }
    //C++ TO C# CONVERTER TODO TASK: The memory management function 'malloc' has no equivalent in C#:
    if ((p = (double)malloc(sizeof(double) * n * m)) == 0)
    {
        GlobalMembersRtkcmn.fatalerr("matrix memory allocation error: n=%d,m=%d\n", n, m);
    }
    return p;
}
/* new integer matrix ----------------------------------------------------------
* allocate memory of integer matrix 
* args   : int    n,m       I   number of rows and columns of matrix
* return : matrix pointer (if n<=0 or m<=0, return NULL)
*-----------------------------------------------------------------------------*/

public static int imat(int n, int m)
{

    int p;

    if (n <= 0 || m <= 0)
    {
        return null;
    }

    if ((p = (int)malloc(sizeof(int) * n * m)) == 0)
    {
        GlobalMembersRtkcmn.fatalerr("integer matrix memory allocation error: n=%d,m=%d\n", n, m);
    }
    return p;
}
/* zero matrix -----------------------------------------------------------------
* generate new zero matrix
* args   : int    n,m       I   number of rows and columns of matrix
* return : matrix pointer (if n<=0 or m<=0, return NULL)
*-----------------------------------------------------------------------------*/

public static double zeros(int n, int m)
{
    double[] p;

#if NOCALLOC
		if ((p = GlobalMembersRtkcmn.mat(n, m)))
		{
			for (n = n * m - 1;n >= 0;n--)
			{
				p[n] = 0.0;
			}
		}
#else
    if (n <= 0 || m <= 0)
    {
        return null;
    }
    if (!(p = new double[sizeof(double)]))
    {
        GlobalMembersRtkcmn.fatalerr("matrix memory allocation error: n=%d,m=%d\n", n, m);
    }
#endif
    return p;
}
/* identity matrix -------------------------------------------------------------
* generate new identity matrix
* args   : int    n         I   number of rows and columns of matrix
* return : matrix pointer (if n<=0, return NULL)
*-----------------------------------------------------------------------------*/

public static double[] eye(int n)
{
    double[] p;
    int i;

    if ((p = GlobalMembersRtkcmn.zeros(n, n)))
    {
        for (i = 0; i < n; i++)
        {
            p[i + i * n] = 1.0;
        }
    }
    return p;
}
/* inner product ---------------------------------------------------------------
* inner product of vectors
* args   : double *a,*b     I   vector a,b (n x 1)
*          int    n         I   size of vector a,b
* return : a'*b
*-----------------------------------------------------------------------------*/
public static double dot(double[] a, double[] b, int n)
{
    double c = 0.0;

    while (--n >= 0)
    {
        c += a[n] * b[n];
    }
    return c;
}
/* euclid norm -----------------------------------------------------------------
* euclid norm of vector
* args   : double *a        I   vector a (n x 1)
*          int    n         I   size of vector a
* return : || a ||
*-----------------------------------------------------------------------------*/
public static double norm(double a, int n)
{
    return Math.Sqrt(GlobalMembersRtkcmn.dot(a, a, n));
}
/* outer product of 3d vectors -------------------------------------------------
* outer product of 3d vectors 
* args   : double *a,*b     I   vector a,b (3 x 1)
*          double *c        O   outer product (a x b) (3 x 1)
* return : none
*-----------------------------------------------------------------------------*/
public static void cross3(double[] a, double[] b, double[] c)
{
    c[0] = a[1] * b[2] - a[2] * b[1];
    c[1] = a[2] * b[0] - a[0] * b[2];
    c[2] = a[0] * b[1] - a[1] * b[0];
}
/* normalize 3d vector ---------------------------------------------------------
* normalize 3d vector
* args   : double *a        I   vector a (3 x 1)
*          double *b        O   normlized vector (3 x 1) || b || = 1
* return : status (1:ok,0:error)
*-----------------------------------------------------------------------------*/
public static int normv3(double[] a, double[] b)
{
    double r;
    if ((r = GlobalMembersRtkcmn.norm(a, 3)) <= 0.0)
    {
        return 0;
    }
    b[0] = a[0] / r;
    b[1] = a[1] / r;
    b[2] = a[2] / r;
    return 1;
}
/* copy matrix -----------------------------------------------------------------
* copy matrix
* args   : double *A        O   destination matrix A (n x m)
*          double *B        I   source matrix B (n x m)
*          int    n,m       I   number of rows and columns of matrix
* return : none
*-----------------------------------------------------------------------------*/
public static void matcpy(ref double A, double B, int n, int m)
{
    //C++ TO C# CONVERTER TODO TASK: The memory management function 'memcpy' has no equivalent in C#:
    memcpy(A, B, sizeof(double) * n * m);
}
/* matrix routines -----------------------------------------------------------*/

#if LAPACK

	/* multiply matrix (wrapper of blas dgemm) -------------------------------------
	* multiply matrix by matrix (C=alpha*A*B+beta*C)
	* args   : char   *tr       I  transpose flags ("N":normal,"T":transpose)
	*          int    n,k,m     I  size of (transposed) matrix A,B
	*          double alpha     I  alpha
	*          double *A,*B     I  (transposed) matrix A (n x m), B (m x k)
	*          double beta      I  beta
	*          double *C        IO matrix C (n x k)
	* return : none
	*-----------------------------------------------------------------------------*/
	public static void matmul(string tr, int n, int k, int m, double alpha, double A, double B, double beta, ref double C)
	{
		int lda = tr[0] == 'T'?m:n;
		int ldb = tr[1] == 'T'?k:m;

		dgemm(ref (string)tr, ref (string)tr.Substring(1), ref n, ref k, ref m, ref alpha, ref (double)A, ref lda, ref (double)B, ref ldb, ref beta, ref C, ref n);
	}
	/* inverse of matrix -----------------------------------------------------------
	* inverse of matrix (A=A^-1)
	* args   : double *A        IO  matrix (n x n)
	*          int    n         I   size of matrix A
	* return : status (0:ok,0>:error)
	*-----------------------------------------------------------------------------*/
	public static int matinv(ref double A, int n)
	{
//C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
//ORIGINAL LINE: double *work;
		double work;
		int info;
		int lwork = n * 16;
//C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
//ORIGINAL LINE: int *ipiv=imat(n,1);
		int ipiv = GlobalMembersRtkcmn.imat(n, 1);

		work = GlobalMembersRtkcmn.mat(lwork, 1);
		dgetrf(ref n, ref n, ref A, ref n, ref ipiv, ref info);
		if (info == 0)
		{
			dgetri(ref n, ref A, ref n, ref ipiv, ref work, ref lwork, ref info);
		}
//C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
		free(ipiv);
//C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
		free(work);
		return info;
	}
	/* solve linear equation -------------------------------------------------------
	* solve linear equation (X=A\Y or X=A'\Y)
	* args   : char   *tr       I   transpose flag ("N":normal,"T":transpose)
	*          double *A        I   input matrix A (n x n)
	*          double *Y        I   input matrix Y (n x m)
	*          int    n,m       I   size of matrix A,Y
	*          double *X        O   X=A\Y or X=A'\Y (n x m)
	* return : status (0:ok,0>:error)
	* notes  : matirix stored by column-major order (fortran convention)
	*          X can be same as Y
	*-----------------------------------------------------------------------------*/
	public static int solve(string tr, double A, double Y, int n, int m, ref double X)
	{
//C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
//ORIGINAL LINE: double *B=mat(n,n);
		double B = GlobalMembersRtkcmn.mat(n, n);
		int info;
//C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
//ORIGINAL LINE: int *ipiv=imat(n,1);
		int ipiv = GlobalMembersRtkcmn.imat(n, 1);

		GlobalMembersRtkcmn.matcpy(ref B, A, n, n);
		GlobalMembersRtkcmn.matcpy(ref X, Y, n, m);
		dgetrf(ref n, ref n, ref B, ref n, ref ipiv, ref info);
		if (info == 0)
		{
			dgetrs(ref (string)tr, ref n, ref m, ref B, ref n, ref ipiv, ref X, ref n, ref info);
		}
//C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
		free(ipiv);
//C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
		free(B);
		return info;
	}

#else

/* multiply matrix -----------------------------------------------------------*/
public static void matmul(string tr, int n, int k, int m, double alpha, double[] A, double[] B, double beta, double[] C)
{
    double d;
    int i;
    int j;
    int x;
    int f = tr[0] == 'N' ? (tr[1] == 'N' ? 1 : 2) : (tr[1] == 'N' ? 3 : 4);

    for (i = 0; i < n; i++)
    {
        for (j = 0; j < k; j++)
        {
            d = 0.0;
            switch (f)
            {
                case 1:
                    for (x = 0; x < m; x++)
                    {
                        d += A[i + x * n] * B[x + j * m];
                    }
                    break;
                case 2:
                    for (x = 0; x < m; x++)
                    {
                        d += A[i + x * n] * B[j + x * k];
                    }
                    break;
                case 3:
                    for (x = 0; x < m; x++)
                    {
                        d += A[x + i * m] * B[x + j * m];
                    }
                    break;
                case 4:
                    for (x = 0; x < m; x++)
                    {
                        d += A[x + i * m] * B[j + x * k];
                    }
                    break;
            }
            if (beta == 0.0)
            {
                C[i + j * n] = alpha * d;
            }
            else
            {
                C[i + j * n] = alpha * d + beta * C[i + j * n];
            }
        }
    }
}
/* LU decomposition ----------------------------------------------------------*/
internal static int ludcmp(double[] A, int n, int[] indx, ref double d)
{
    double big;
    double s;
    double tmp;
    double[] vv = GlobalMembersRtkcmn.mat(n, 1);
    int i;
    int imax = 0;
    int j;
    int k;

    d = 1.0;
    for (i = 0; i < n; i++)
    {
        big = 0.0;
        for (j = 0; j < n; j++)
        {
            if ((tmp = Math.Abs(A[i + j * n])) > big)
            {
                big = tmp;
            }
        }
        if (big > 0.0)
        {
            vv[i] = 1.0 / big;
        }
        else
        {
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(vv);
            return -1;
        }
    }
    for (j = 0; j < n; j++)
    {
        for (i = 0; i < j; i++)
        {
            s = A[i + j * n];
            for (k = 0; k < i; k++)
            {
                s -= A[i + k * n] * A[k + j * n];
            }
            A[i + j * n] = s;
        }
        big = 0.0;
        for (i = j; i < n; i++)
        {
            s = A[i + j * n];
            for (k = 0; k < j; k++)
            {
                s -= A[i + k * n] * A[k + j * n];
            }
            A[i + j * n] = s;
            if ((tmp = vv[i] * Math.Abs(s)) >= big)
            {
                big = tmp;
                imax = i;
            }
        }
        if (j != imax)
        {
            for (k = 0; k < n; k++)
            {
                tmp = A[imax + k * n];
                A[imax + k * n] = A[j + k * n];
                A[j + k * n] = tmp;
            }
            d = -d;
            vv[imax] = vv[j];
        }
        indx[j] = imax;
        if (A[j + j * n] == 0.0)
        {
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(vv);
            return -1;
        }
        if (j != n - 1)
        {
            tmp = 1.0 / A[j + j * n];
            for (i = j + 1; i < n; i++)
            {
                A[i + j * n] *= tmp;
            }
        }
    }
    //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
    free(vv);
    return 0;
}
/* LU back-substitution ------------------------------------------------------*/
internal static void lubksb(double[] A, int n, int[] indx, double[] b)
{
    double s;
    int i;
    int ii = -1;
    int ip;
    int j;

    for (i = 0; i < n; i++)
    {
        ip = indx[i];
        s = b[ip];
        b[ip] = b[i];
        if (ii >= 0)
        {
            for (j = ii; j < i; j++)
            {
                s -= A[i + j * n] * b[j];
            }
        }
        else if (s != 0)
        {
            ii = i;
        }
        b[i] = s;
    }
    for (i = n - 1; i >= 0; i--)
    {
        s = b[i];
        for (j = i + 1; j < n; j++)
        {
            s -= A[i + j * n] * b[j];
        }
        b[i] = s / A[i + i * n];
    }
}
/* inverse of matrix ---------------------------------------------------------*/
public static int matinv(double[] A, int n)
{
    double d;
    //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
    //ORIGINAL LINE: double *B;
    double B;
    int i;
    int j;
    //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
    //ORIGINAL LINE: int *indx;
    int indx;

    indx = GlobalMembersRtkcmn.imat(n, 1);
    B = GlobalMembersRtkcmn.mat(n, n);
    GlobalMembersRtkcmn.matcpy(ref B, A, n, n);
    if (GlobalMembersRtkcmn.ludcmp(B, n, indx, ref d) != 0)
    {
        //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
        free(indx);
        //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
        free(B);
        return -1;
    }
    for (j = 0; j < n; j++)
    {
        for (i = 0; i < n; i++)
        {
            A[i + j * n] = 0.0;
        }
        A[j + j * n] = 1.0;
        GlobalMembersRtkcmn.lubksb(B, n, indx, A + j * n);
    }
    //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
    free(indx);
    //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
    free(B);
    return 0;
}
/* solve linear equation -----------------------------------------------------*/
public static int solve(string tr, double A, double Y, int n, int m, ref double X)
{
    //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
    //ORIGINAL LINE: double *B=mat(n,n);
    double B = GlobalMembersRtkcmn.mat(n, n);
    int info;

    GlobalMembersRtkcmn.matcpy(ref B, A, n, n);
    if ((info = GlobalMembersRtkcmn.matinv(ref B, n)) == 0)
    {
        GlobalMembersRtkcmn.matmul(tr[0] == 'N' ? "NN" : "TN", n, m, n, 1.0, B, Y, 0.0, ref X);
    }
    //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
    free(B);
    return info;
}
#endif
/* end of matrix routines ----------------------------------------------------*/

/* least square estimation -----------------------------------------------------
* least square estimation by solving normal equation (x=(A*A')^-1*A*y)
* args   : double *A        I   transpose of (weighted) design matrix (n x m)
*          double *y        I   (weighted) measurements (m x 1)
*          int    n,m       I   number of parameters and measurements (n<=m)
*          double *x        O   estmated parameters (n x 1)
*          double *Q        O   esimated parameters covariance matrix (n x n)
* return : status (0:ok,0>:error)
* notes  : for weighted least square, replace A and y by A*w and w*y (w=W^(1/2))
*          matirix stored by column-major order (fortran convention)
*-----------------------------------------------------------------------------*/
public static int lsq(double A, double y, int n, int m, ref double x, ref double Q)
{
    //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
    //ORIGINAL LINE: double *Ay;
    double Ay;
    int info;

    if (m < n)
    {
        return -1;
    }
    Ay = GlobalMembersRtkcmn.mat(n, 1);
    GlobalMembersRtkcmn.matmul("NN", n, 1, m, 1.0, A, y, 0.0, ref Ay); // Ay=A*y
    GlobalMembersRtkcmn.matmul("NT", n, n, m, 1.0, A, A, 0.0, ref Q); // Q=A*A'
    if ((info = GlobalMembersRtkcmn.matinv(ref Q, n)) == 0) // x=Q^-1*Ay
    {
        GlobalMembersRtkcmn.matmul("NN", n, 1, n, 1.0, Q, Ay, 0.0, ref x);
    }
    //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
    free(Ay);
    return info;
}
/* kalman filter ---------------------------------------------------------------
* kalman filter state update as follows:
*
*   K=P*H*(H'*P*H+R)^-1, xp=x+K*v, Pp=(I-K*H')*P
*
* args   : double *x        I   states vector (n x 1)
*          double *P        I   covariance matrix of states (n x n)
*          double *H        I   transpose of design matrix (n x m)
*          double *v        I   innovation (measurement - model) (m x 1)
*          double *R        I   covariance matrix of measurement error (m x m)
*          int    n,m       I   number of states and measurements
*          double *xp       O   states vector after update (n x 1)
*          double *Pp       O   covariance matrix of states after update (n x n)
* return : status (0:ok,<0:error)
* notes  : matirix stored by column-major order (fortran convention)
*          if state x[i]==0.0, not updates state x[i]/P[i+i*n]
*-----------------------------------------------------------------------------*/
internal static int filter_(double x, double P, double H, double v, double R, int n, int m, ref double xp, ref double Pp)
{
    //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
    //ORIGINAL LINE: double *F=mat(n,m),*Q=mat(m,m),*K=mat(n,m),*I=eye(n);
    double F = GlobalMembersRtkcmn.mat(n, m);
    //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
    //ORIGINAL LINE: double *Q=mat(m,m);
    double Q = GlobalMembersRtkcmn.mat(m, m);
    //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
    //ORIGINAL LINE: double *K=mat(n,m);
    double K = GlobalMembersRtkcmn.mat(n, m);
    double[] I = GlobalMembersRtkcmn.eye(n);
    int info;

    GlobalMembersRtkcmn.matcpy(ref Q, R, m, m);
    GlobalMembersRtkcmn.matcpy(ref xp, x, n, 1);
    GlobalMembersRtkcmn.matmul("NN", n, m, n, 1.0, P, H, 0.0, ref F); // Q=H'*P*H+R
    GlobalMembersRtkcmn.matmul("TN", m, m, n, 1.0, H, F, 1.0, ref Q);
    if ((info = GlobalMembersRtkcmn.matinv(ref Q, m)) == 0)
    {
        GlobalMembersRtkcmn.matmul("NN", n, m, m, 1.0, F, Q, 0.0, ref K); // K=P*H*Q^-1
        GlobalMembersRtkcmn.matmul("NN", n, 1, m, 1.0, K, v, 1.0, ref xp); // xp=x+K*v
        GlobalMembersRtkcmn.matmul("NT", n, n, m, -1.0, K, H, 1.0, ref I); // Pp=(I-K*H')*P
        GlobalMembersRtkcmn.matmul("NN", n, n, n, 1.0, I, P, 0.0, ref Pp);
    }
    //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
    free(F);
    //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
    free(Q);
    //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
    free(K);
    //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
    free(I);
    return info;
}
public static int filter(double[] x, double[] P, double[] H, double v, double R, int n, int m)
{
    double[] x_;
    //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
    //ORIGINAL LINE: double *xp_;
    double xp_;
    double[] P_;
    //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
    //ORIGINAL LINE: double *Pp_;
    double Pp_;
    double[] H_;
    int i;
    int j;
    int k;
    int info;
    int[] ix;

    ix = GlobalMembersRtkcmn.imat(n, 1);
    for (i = k = 0; i < n; i++)
    {
        if (x[i] != 0.0 && P[i + i * n] > 0.0)
        {
            ix[k++] = i;
        }
    }
    x_ = GlobalMembersRtkcmn.mat(k, 1);
    xp_ = GlobalMembersRtkcmn.mat(k, 1);
    P_ = GlobalMembersRtkcmn.mat(k, k);
    Pp_ = GlobalMembersRtkcmn.mat(k, k);
    H_ = GlobalMembersRtkcmn.mat(k, m);
    for (i = 0; i < k; i++)
    {
        x_[i] = x[ix[i]];
        for (j = 0; j < k; j++)
        {
            P_[i + j * k] = P[ix[i] + ix[j] * n];
        }
        for (j = 0; j < m; j++)
        {
            H_[i + j * k] = H[ix[i] + j * n];
        }
    }
    info = GlobalMembersRtkcmn.filter_(x_, P_, H_, v, R, k, m, ref xp_, ref Pp_);
    for (i = 0; i < k; i++)
    {
        x[ix[i]] = xp_[i];
        for (j = 0; j < k; j++)
        {
            P[ix[i] + ix[j] * n] = Pp_[i + j * k];
        }
    }
    //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
    free(ix);
    //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
    free(x_);
    //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
    free(xp_);
    //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
    free(P_);
    //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
    free(Pp_);
    //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
    free(H_);
    return info;
}
/* smoother --------------------------------------------------------------------
* combine forward and backward filters by fixed-interval smoother as follows:
*
*   xs=Qs*(Qf^-1*xf+Qb^-1*xb), Qs=(Qf^-1+Qb^-1)^-1)
*
* args   : double *xf       I   forward solutions (n x 1)
* args   : double *Qf       I   forward solutions covariance matrix (n x n)
*          double *xb       I   backward solutions (n x 1)
*          double *Qb       I   backward solutions covariance matrix (n x n)
*          int    n         I   number of solutions
*          double *xs       O   smoothed solutions (n x 1)
*          double *Qs       O   smoothed solutions covariance matrix (n x n)
* return : status (0:ok,0>:error)
* notes  : see reference [4] 5.2
*          matirix stored by column-major order (fortran convention)
*-----------------------------------------------------------------------------*/
public static int smoother(double xf, double Qf, double xb, double Qb, int n, ref double xs, double[] Qs)
{
    double[] invQf = GlobalMembersRtkcmn.mat(n, n);
    double[] invQb = GlobalMembersRtkcmn.mat(n, n);
    //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
    //ORIGINAL LINE: double *xx=mat(n,1);
    double xx = GlobalMembersRtkcmn.mat(n, 1);
    int i;
    int info = -1;

    GlobalMembersRtkcmn.matcpy(ref invQf, Qf, n, n);
    GlobalMembersRtkcmn.matcpy(ref invQb, Qb, n, n);
    if (GlobalMembersRtkcmn.matinv(ref invQf, n) == 0 && GlobalMembersRtkcmn.matinv(ref invQb, n) == 0)
    {
        for (i = 0; i < n * n; i++)
        {
            Qs[i] = invQf[i] + invQb[i];
        }
        if ((info = GlobalMembersRtkcmn.matinv(ref Qs, n)) == 0)
        {
            GlobalMembersRtkcmn.matmul("NN", n, 1, n, 1.0, invQf, xf, 0.0, ref xx);
            GlobalMembersRtkcmn.matmul("NN", n, 1, n, 1.0, invQb, xb, 1.0, ref xx);
            GlobalMembersRtkcmn.matmul("NN", n, 1, n, 1.0, Qs, xx, 0.0, ref xs);
        }
    }
    //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
    free(invQf);
    //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
    free(invQb);
    //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
    free(xx);
    return info;
}
/* print matrix ----------------------------------------------------------------
* print matrix to stdout
* args   : double *A        I   matrix A (n x m)
*          int    n,m       I   number of rows and columns of A
*          int    p,q       I   total columns, columns under decimal point
*         (FILE  *fp        I   output file pointer)
* return : none
* notes  : matirix stored by column-major order (fortran convention)
*-----------------------------------------------------------------------------*/
public static void matfprint(double[] A, int n, int m, int p, int q, FILE fp)
{
    int i;
    int j;

    for (i = 0; i < n; i++)
    {
        for (j = 0; j < m; j++)
        {
            fprintf(fp, " %*.*f", p, q, A[i + j * n]);
        }
        fprintf(fp, "\n");
    }
}
public static void matprint(double[] A, int n, int m, int p, int q)
{
    GlobalMembersRtkcmn.matfprint(A, n, m, p, q, stdout);
}
/* string to number ------------------------------------------------------------
* convert substring in string to number
* args   : char   *s        I   string ("... nnn.nnn ...")
*          int    i,n       I   substring position and width
* return : converted number (0.0:error)
*-----------------------------------------------------------------------------*/
//C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on the parameter 's', so pointers on this parameter are left unchanged:
public static double str2num(sbyte* s, int i, int n)
{
    double value;
    string str = new string(new char[256]);
    //C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on this variable, so pointers on this variable are left unchanged:
    sbyte* p = str;

    if (i < 0 || (int)s.Length < i || (int)sizeof(sbyte) - 1 < n)
    {
        return 0.0;
    }
    for (s += i; *s && --n >= 0; s++)
    {
        *p++ = *s == 'd' || *s == 'D' ? 'E' : *s;
    }
    *p = (sbyte)'\0';
    return sscanf(str, "%lf", value) == 1 ? value : 0.0;
}
/* string to time --------------------------------------------------------------
* convert substring in string to gtime_t struct
* args   : char   *s        I   string ("... yyyy mm dd hh mm ss ...")
*          int    i,n       I   substring position and width
*          gtime_t *t       O   gtime_t struct
* return : status (0:ok,0>:error)
*-----------------------------------------------------------------------------*/
//C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on the parameter 's', so pointers on this parameter are left unchanged:
public static int str2time(sbyte* s, int i, int n, gtime_t t)
{
    double[] ep = new double[6];
    string str = new string(new char[256]);
    //C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on this variable, so pointers on this variable are left unchanged:
    sbyte* p = str;

    if (i < 0 || (int)s.Length < i || (int)sizeof(sbyte) - 1 < i)
    {
        return -1;
    }
    for (s += i; *s && --n >= 0;)
    {
        *p++ = *s++;
    }
    *p = (sbyte)'\0';
    if (sscanf(str, "%lf %lf %lf %lf %lf %lf", ep, ep + 1, ep + 2, ep + 3, ep + 4, ep + 5) < 6)
    {
        return -1;
    }
    if (ep[0] < 100.0)
    {
        ep[0] += ep[0] < 80.0 ? 2000.0 : 1900.0;
    }
    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
    //ORIGINAL LINE: *t=epoch2time(ep);
    t.CopyFrom(GlobalMembersRtkcmn.epoch2time(ep));
    return 0;
}
/* convert calendar day/time to time -------------------------------------------
* convert calendar day/time to gtime_t struct
* args   : double *ep       I   day/time {year,month,day,hour,min,sec}
* return : gtime_t struct
* notes  : proper in 1970-2037 or 1970-2099 (64bit time_t)
*-----------------------------------------------------------------------------*/
public static gtime_t epoch2time(double[] ep)
{
    int[] doy = { 1, 32, 60, 91, 121, 152, 182, 213, 244, 274, 305, 335 };
    gtime_t time = new gtime_t();
    int days;
    int sec;
    int year = (int)ep[0];
    int mon = (int)ep[1];
    int day = (int)ep[2];

    if (year < 1970 || 2099 < year || mon < 1 || 12 < mon)
    {
        return time;
    }

    /* leap year if year%4==0 in 1901-2099 */
    days = (year - 1970) * 365 + (year - 1969) / 4 + doy[mon - 1] + day - 2 + (year % 4 == 0 && mon >= 3 ? 1 : 0);
    sec = (int)Math.Floor(ep[5]);
    time.time = (time_t)days * 86400 + (int)ep[3] * 3600 + (int)ep[4] * 60 + sec;
    time.sec = ep[5] - sec;
    return time;
}
/* time to calendar day/time ---------------------------------------------------
* convert gtime_t struct to calendar day/time
* args   : gtime_t t        I   gtime_t struct
*          double *ep       O   day/time {year,month,day,hour,min,sec}
* return : none
* notes  : proper in 1970-2037 or 1970-2099 (64bit time_t)
*-----------------------------------------------------------------------------*/
public static void time2epoch(gtime_t t, double[] ep)
{
    int[] mday = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31, 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31, 31, 29, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31, 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 }; // # of days in a month
    int days;
    int sec;
    int mon;
    int day;

    /* leap year if year%4==0 in 1901-2099 */
    days = (int)(t.time / 86400);
    sec = (int)(t.time - (time_t)days * 86400);
    for (day = days % 1461, mon = 0; mon < 48; mon++)
    {
        if (day >= mday[mon])
        {
            day -= mday[mon];
        }
        else
            break;
    }
    ep[0] = 1970 + days / 1461 * 4 + mon / 12;
    ep[1] = mon % 12 + 1;
    ep[2] = day + 1;
    ep[3] = sec / 3600;
    ep[4] = sec % 3600 / 60;
    ep[5] = sec % 60 + t.sec;
}
/* gps time to time ------------------------------------------------------------
* convert week and tow in gps time to gtime_t struct
* args   : int    week      I   week number in gps time
*          double sec       I   time of week in gps time (s)
* return : gtime_t struct
*-----------------------------------------------------------------------------*/
public static gtime_t gpst2time(int week, double sec)
{
    gtime_t t = GlobalMembersRtkcmn.epoch2time(gpst0);

    if (sec < -1E9 || 1E9 < sec)
    {
        sec = 0.0;
    }
    t.time += 86400 * 7 * week + (int)sec;
    t.sec = sec - (int)sec;
    return t;
}
/* time to gps time ------------------------------------------------------------
* convert gtime_t struct to week and tow in gps time
* args   : gtime_t t        I   gtime_t struct
*          int    *week     IO  week number in gps time (NULL: no output)
* return : time of week in gps time (s)
*-----------------------------------------------------------------------------*/
public static double time2gpst(gtime_t t, ref int week)
{
    gtime_t t0 = GlobalMembersRtkcmn.epoch2time(gpst0);
    time_t sec = t.time - t0.time;
    int w = (int)(sec / (86400 * 7));

    if (week != 0)
    {
        week = w;
    }
    return (double)(sec - w * 86400 * 7) + t.sec;
}
/* galileo system time to time -------------------------------------------------
* convert week and tow in galileo system time (gst) to gtime_t struct
* args   : int    week      I   week number in gst
*          double sec       I   time of week in gst (s)
* return : gtime_t struct
*-----------------------------------------------------------------------------*/
public static gtime_t gst2time(int week, double sec)
{
    gtime_t t = GlobalMembersRtkcmn.epoch2time(gst0);

    if (sec < -1E9 || 1E9 < sec)
    {
        sec = 0.0;
    }
    t.time += 86400 * 7 * week + (int)sec;
    t.sec = sec - (int)sec;
    return t;
}
/* time to galileo system time -------------------------------------------------
* convert gtime_t struct to week and tow in galileo system time (gst)
* args   : gtime_t t        I   gtime_t struct
*          int    *week     IO  week number in gst (NULL: no output)
* return : time of week in gst (s)
*-----------------------------------------------------------------------------*/
public static double time2gst(gtime_t t, ref int week)
{
    gtime_t t0 = GlobalMembersRtkcmn.epoch2time(gst0);
    time_t sec = t.time - t0.time;
    int w = (int)(sec / (86400 * 7));

    if (week != 0)
    {
        week = w;
    }
    return (double)(sec - w * 86400 * 7) + t.sec;
}
/* beidou time (bdt) to time ---------------------------------------------------
* convert week and tow in beidou time (bdt) to gtime_t struct
* args   : int    week      I   week number in bdt
*          double sec       I   time of week in bdt (s)
* return : gtime_t struct
*-----------------------------------------------------------------------------*/
public static gtime_t bdt2time(int week, double sec)
{
    gtime_t t = GlobalMembersRtkcmn.epoch2time(bdt0);

    if (sec < -1E9 || 1E9 < sec)
    {
        sec = 0.0;
    }
    t.time += 86400 * 7 * week + (int)sec;
    t.sec = sec - (int)sec;
    return t;
}
/* time to beidouo time (bdt) --------------------------------------------------
* convert gtime_t struct to week and tow in beidou time (bdt)
* args   : gtime_t t        I   gtime_t struct
*          int    *week     IO  week number in bdt (NULL: no output)
* return : time of week in bdt (s)
*-----------------------------------------------------------------------------*/
public static double time2bdt(gtime_t t, ref int week)
{
    gtime_t t0 = GlobalMembersRtkcmn.epoch2time(bdt0);
    time_t sec = t.time - t0.time;
    int w = (int)(sec / (86400 * 7));

    if (week != 0)
    {
        week = w;
    }
    return (double)(sec - w * 86400 * 7) + t.sec;
}
/* add time --------------------------------------------------------------------
* add time to gtime_t struct
* args   : gtime_t t        I   gtime_t struct
*          double sec       I   time to add (s)
* return : gtime_t struct (t+sec)
*-----------------------------------------------------------------------------*/
public static gtime_t timeadd(gtime_t t, double sec)
{
    double tt;

    t.sec += sec;
    tt = Math.Floor(t.sec);
    t.time += (int)tt;
    t.sec -= tt;
    return t;
}
/* time difference -------------------------------------------------------------
* difference between gtime_t structs
* args   : gtime_t t1,t2    I   gtime_t structs
* return : time difference (t1-t2) (s)
*-----------------------------------------------------------------------------*/
public static double timediff(gtime_t t1, gtime_t t2)
{
    return difftime(t1.time, t2.time) + t1.sec - t2.sec;
}
/* get current time in utc -----------------------------------------------------
* get current time in utc
* args   : none
* return : current time in utc
*-----------------------------------------------------------------------------*/
internal static double timeoffset_ = 0.0; // time offset (s)

public static gtime_t timeget()
{
    double[] ep = { 0, null, null, null, null, null };
#if WIN32
		SYSTEMTIME ts = new SYSTEMTIME();

		GetSystemTime(ts); // utc
		ep[0] = ts.wYear;
		ep[1] = ts.wMonth;
		ep[2] = ts.wDay;
		ep[3] = ts.wHour;
		ep[4] = ts.wMinute;
		ep[5] = ts.wSecond + ts.wMilliseconds * 1E-3;
#else
    timeval tv = new timeval();
    tm tt;

    if (!gettimeofday(tv, null) && (tt = gmtime(tv.tv_sec)) != null)
    {
        ep[0] = tt.tm_year + 1900;
        ep[1] = tt.tm_mon + 1;
        ep[2] = tt.tm_mday;
        ep[3] = tt.tm_hour;
        ep[4] = tt.tm_min;
        ep[5] = tt.tm_sec + tv.tv_usec * 1E-6;
    }
#endif
    return GlobalMembersRtkcmn.timeadd(GlobalMembersRtkcmn.epoch2time(ep), timeoffset_);
}
/* set current time in utc -----------------------------------------------------
* set current time in utc
* args   : gtime_t          I   current time in utc
* return : none
* notes  : just set time offset between cpu time and current time
*          the time offset is reflected to only timeget()
*          not reentrant
*-----------------------------------------------------------------------------*/
public static void timeset(gtime_t t)
{
    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
    //ORIGINAL LINE: timeoffset_+=timediff(t,timeget());
    timeoffset_ += GlobalMembersRtkcmn.timediff(new gtime_t(t), GlobalMembersRtkcmn.timeget());
}
/* read leap seconds table -----------------------------------------------------
* read leap seconds table
* args   : char    *file    I   leap seconds table file
* return : status (1:ok,0:error)
* notes  : (1) The records in the table file cosist of the following fields:
*              year month day hour min sec UTC-GPST(s)
*          (2) The date and time indicate the start UTC time for the UTC-GPST
*          (3) The date and time should be descending order.
*-----------------------------------------------------------------------------*/
public static int read_leaps(string file)
{
    FILE fp;
    string buff = new string(new char[256]);
    string p;
    int i;
    int n = 0;
    int[] ep = new int[6];
    int ls;

    if ((fp = fopen(file, "r")) == null)
    {
        return 0;
    }

    while (fgets(buff, sizeof(sbyte), fp) && n < DefineConstants.MAXLEAPS)
    {
        if ((p = StringFunctions.StrChr(buff, '#')) != 0)
        {
            p = (sbyte)'\0';
        }
        if (sscanf(buff, "%d %d %d %d %d %d %d", ep, ep + 1, ep + 2, ep + 3, ep + 4, ep + 5, ls) < 7)
            continue;
        for (i = 0; i < 6; i++)
        {
            leaps[n, i] = ep[i];
        }
        leaps[n++, 6] = ls;
    }
    for (i = 0; i < 7; i++)
    {
        leaps[n, i] = 0.0;
    }
    fclose(fp);
    return 1;
}
/* gpstime to utc --------------------------------------------------------------
* convert gpstime to utc considering leap seconds
* args   : gtime_t t        I   time expressed in gpstime
* return : time expressed in utc
* notes  : ignore slight time offset under 100 ns
*-----------------------------------------------------------------------------*/
public static gtime_t gpst2utc(gtime_t t)
{
    gtime_t tu = new gtime_t();
    int i;

    for (i = 0; leaps[i, 0] > 0; i++)
    {
        //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
        //ORIGINAL LINE: tu=timeadd(t,leaps[i][6]);
        //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
        tu.CopyFrom(GlobalMembersRtkcmn.timeadd(new gtime_t(t), leaps[i, 6]));
        //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
        //ORIGINAL LINE: if (timediff(tu,epoch2time(leaps[i]))>=0.0)
        if (GlobalMembersRtkcmn.timediff(new gtime_t(tu), GlobalMembersRtkcmn.epoch2time(leaps[i])) >= 0.0)
        {
            return tu;
        }
    }
    return t;
}
/* utc to gpstime --------------------------------------------------------------
* convert utc to gpstime considering leap seconds
* args   : gtime_t t        I   time expressed in utc
* return : time expressed in gpstime
* notes  : ignore slight time offset under 100 ns
*-----------------------------------------------------------------------------*/
public static gtime_t utc2gpst(gtime_t t)
{
    int i;

    for (i = 0; leaps[i, 0] > 0; i++)
    {
        //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
        //ORIGINAL LINE: if (timediff(t,epoch2time(leaps[i]))>=0.0)
        if (GlobalMembersRtkcmn.timediff(new gtime_t(t), GlobalMembersRtkcmn.epoch2time(leaps[i])) >= 0.0)
        {
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: return timeadd(t,-leaps[i][6]);
            return GlobalMembersRtkcmn.timeadd(new gtime_t(t), -leaps[i, 6]);
        }
    }
    return t;
}
/* gpstime to bdt --------------------------------------------------------------
* convert gpstime to bdt (beidou navigation satellite system time)
* args   : gtime_t t        I   time expressed in gpstime
* return : time expressed in bdt
* notes  : ref [8] 3.3, 2006/1/1 00:00 BDT = 2006/1/1 00:00 UTC
*          no leap seconds in BDT
*          ignore slight time offset under 100 ns
*-----------------------------------------------------------------------------*/
public static gtime_t gpst2bdt(gtime_t t)
{
    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
    //ORIGINAL LINE: return timeadd(t,-14.0);
    return GlobalMembersRtkcmn.timeadd(new gtime_t(t), -14.0);
}
/* bdt to gpstime --------------------------------------------------------------
* convert bdt (beidou navigation satellite system time) to gpstime
* args   : gtime_t t        I   time expressed in bdt
* return : time expressed in gpstime
* notes  : see gpst2bdt()
*-----------------------------------------------------------------------------*/
public static gtime_t bdt2gpst(gtime_t t)
{
    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
    //ORIGINAL LINE: return timeadd(t,14.0);
    return GlobalMembersRtkcmn.timeadd(new gtime_t(t), 14.0);
}
/* time to day and sec -------------------------------------------------------*/
internal static double time2sec(gtime_t time, gtime_t day)
{
    double[] ep = new double[6];
    double sec;
    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
    //ORIGINAL LINE: time2epoch(time,ep);
    GlobalMembersRtkcmn.time2epoch(new gtime_t(time), ep);
    sec = ep[3] * 3600.0 + ep[4] * 60.0 + ep[5];
    ep[3] = ep[4] = ep[5] = 0.0;
    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
    //ORIGINAL LINE: *day=epoch2time(ep);
    day.CopyFrom(GlobalMembersRtkcmn.epoch2time(ep));
    return sec;
}
/* utc to gmst -----------------------------------------------------------------
* convert utc to gmst (Greenwich mean sidereal time)
* args   : gtime_t t        I   time expressed in utc
*          double ut1_utc   I   UT1-UTC (s)
* return : gmst (rad)
*-----------------------------------------------------------------------------*/
public static double utc2gmst(gtime_t t, double ut1_utc)
{
    double[] ep2000 = { 2000, 1, 1, 12, 0, 0 };
    gtime_t tut = new gtime_t();
    gtime_t tut0 = new gtime_t();
    double ut;
    double t1;
    double t2;
    double t3;
    double gmst0;
    double gmst;

    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
    //ORIGINAL LINE: tut=timeadd(t,ut1_utc);
    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
    tut.CopyFrom(GlobalMembersRtkcmn.timeadd(new gtime_t(t), ut1_utc));
    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
    //ORIGINAL LINE: ut=time2sec(tut,&tut0);
    ut = GlobalMembersRtkcmn.time2sec(new gtime_t(tut), tut0);
    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
    //ORIGINAL LINE: t1=timediff(tut0,epoch2time(ep2000))/86400.0/36525.0;
    t1 = GlobalMembersRtkcmn.timediff(new gtime_t(tut0), GlobalMembersRtkcmn.epoch2time(ep2000)) / 86400.0 / 36525.0;
    t2 = t1 * t1;
    t3 = t2 * t1;
    gmst0 = 24110.54841 + 8640184.812866 * t1 + 0.093104 * t2 - 6.2E-6 * t3;
    gmst = gmst0 + 1.002737909350795 * ut;

    return Math.IEEERemainder(gmst, 86400.0) * DefineConstants.PI / 43200.0; // 0 <= gmst <= 2*PI
}
/* time to string --------------------------------------------------------------
* convert gtime_t struct to string
* args   : gtime_t t        I   gtime_t struct
*          char   *s        O   string ("yyyy/mm/dd hh:mm:ss.ssss")
*          int    n         I   number of decimals
* return : none
*-----------------------------------------------------------------------------*/
public static void time2str(gtime_t t, ref string s, int n)
{
    double[] ep = new double[6];

    if (n < 0)
    {
        n = 0;
    }
    else if (n > 12)
    {
        n = 12;
    }
    if (1.0 - t.sec < 0.5 / Math.Pow(10.0, n))
    {
        t.time++;
        t.sec = 0.0;
    };
    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
    //ORIGINAL LINE: time2epoch(t,ep);
    GlobalMembersRtkcmn.time2epoch(new gtime_t(t), ep);
    //C++ TO C# CONVERTER TODO TASK: Zero padding is not converted when combined with a minimum width specifier:
    //ORIGINAL LINE: sprintf(s,"%04.0f/%02.0f/%02.0f %02.0f:%02.0f:%0*.*f",ep[0],ep[1],ep[2], ep[3],ep[4],n<=0?2:n+3,n<=0?0:n,ep[5]);
    //C++ TO C# CONVERTER TODO TASK: The following line has a C format specifier which cannot be directly translated to C#:
    s = string.Format("{0,4:f0}/{1,2:f0}/{2,2:f0} {3,2:f0}:{4,2:f0}:%0*.*f", ep[0], ep[1], ep[2], ep[3], ep[4], n <= 0 ? 2 : n + 3, n <= 0 ? 0 : n, ep[5]);
}
//C++ TO C# CONVERTER NOTE: This was formerly a static local variable declaration (not allowed in C#):
private static string time_str_buff = new string(new char[64]);
/* get time string -------------------------------------------------------------
* get time string
* args   : gtime_t t        I   gtime_t struct
*          int    n         I   number of decimals
* return : time string
* notes  : not reentrant, do not use multiple in a function
*-----------------------------------------------------------------------------*/
public static string time_str(gtime_t t, int n)
{
    //C++ TO C# CONVERTER NOTE: This static local variable declaration (not allowed in C#) has been moved just prior to the method:
    //	static sbyte buff[64];
    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
    //ORIGINAL LINE: time2str(t,buff,n);
    GlobalMembersRtkcmn.time2str(new gtime_t(t), ref time_str_buff, n);
    return time_str_buff;
}
/* time to day of year ---------------------------------------------------------
* convert time to day of year
* args   : gtime_t t        I   gtime_t struct
* return : day of year (days)
*-----------------------------------------------------------------------------*/
public static double time2doy(gtime_t t)
{
    double[] ep = new double[6];

    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
    //ORIGINAL LINE: time2epoch(t,ep);
    GlobalMembersRtkcmn.time2epoch(new gtime_t(t), ep);
    ep[1] = ep[2] = 1.0;
    ep[3] = ep[4] = ep[5] = 0.0;
    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
    //ORIGINAL LINE: return timediff(t,epoch2time(ep))/86400.0+1.0;
    return GlobalMembersRtkcmn.timediff(new gtime_t(t), GlobalMembersRtkcmn.epoch2time(ep)) / 86400.0 + 1.0;
}
/* adjust gps week number ------------------------------------------------------
* adjust gps week number using cpu time
* args   : int   week       I   not-adjusted gps week number
* return : adjusted gps week number
*-----------------------------------------------------------------------------*/
public static int adjgpsweek(int week)
{
    int w;
    ()GlobalMembersRtkcmn.time2gpst(GlobalMembersRtkcmn.utc2gpst(GlobalMembersRtkcmn.timeget()), ref w);
    if (w < 1560) // use 2009/12/1 if time is earlier than 2009/12/1
    {
        w = 1560;
    }
    return week + (w - week + 512) / 1024 * 1024;
}
/* get tick time ---------------------------------------------------------------
* get current tick in ms
* args   : none
* return : current tick in ms
*-----------------------------------------------------------------------------*/
public static uint tickget()
{
#if WIN32
		return (uint)timeGetTime();
#else
    timespec tp = new timespec();
    timeval tv = new timeval();

#if CLOCK_MONOTONIC_RAW
		/* linux kernel > 2.6.28 */
		if (!clock_gettime(CLOCK_MONOTONIC_RAW, tp))
		{
			return tp.tv_sec * 1000u + tp.tv_nsec / 1000000u;
		}
		else
		{
			gettimeofday(tv, null);
			return tv.tv_sec * 1000u + tv.tv_usec / 1000u;
		}
#else
    gettimeofday(tv, null);
    return tv.tv_sec * 1000u + tv.tv_usec / 1000u;
#endif
#endif
}
/* sleep ms --------------------------------------------------------------------
* sleep ms
* args   : int   ms         I   miliseconds to sleep (<0:no sleep)
* return : none
*-----------------------------------------------------------------------------*/
public static void sleepms(int ms)
{
#if WIN32
		if (ms < 5)
		{
			Sleep(1);
		}
		else
		{
			Sleep(ms);
		}
#else
    timespec ts = new timespec();
    if (ms <= 0)
        return;
    ts.tv_sec = (time_t)(ms / 1000);
    ts.tv_nsec = (int)(ms % 1000 * 1000000);
    nanosleep(ts, null);
#endif
}
/* convert degree to deg-min-sec -----------------------------------------------
* convert degree to degree-minute-second
* args   : double deg       I   degree
*          double *dms      O   degree-minute-second {deg,min,sec}
* return : none
*-----------------------------------------------------------------------------*/
public static void deg2dms(double deg, double[] dms)
{
    double sign = deg < 0.0 ? -1.0 : 1.0;
    double a = Math.Abs(deg);
    dms[0] = Math.Floor(a);
    a = (a - dms[0]) * 60.0;
    dms[1] = Math.Floor(a);
    a = (a - dms[1]) * 60.0;
    dms[2] = a;
    dms[0] *= sign;
}
/* convert deg-min-sec to degree -----------------------------------------------
* convert degree-minute-second to degree
* args   : double *dms      I   degree-minute-second {deg,min,sec}
* return : degree
*-----------------------------------------------------------------------------*/
public static double dms2deg(double[] dms)
{
    double sign = dms[0] < 0.0 ? -1.0 : 1.0;
    return sign * (Math.Abs(dms[0]) + dms[1] / 60.0 + dms[2] / 3600.0);
}
/* transform ecef to geodetic postion ------------------------------------------
* transform ecef position to geodetic position
* args   : double *r        I   ecef position {x,y,z} (m)
*          double *pos      O   geodetic position {lat,lon,h} (rad,m)
* return : none
* notes  : WGS84, ellipsoidal height
*-----------------------------------------------------------------------------*/
public static void ecef2pos(double[] r, double[] pos)
{
    double e2 = 1.0 / 298.257223563 * (2.0 - 1.0 / 298.257223563);
    double r2 = GlobalMembersRtkcmn.dot(r, r, 2);
    double z;
    double zk;
    double v = DefineConstants.RE_WGS84;
    double sinp;

    for (z = r[2], zk = 0.0; Math.Abs(z - zk) >= 1E-4;)
    {
        zk = z;
        sinp = z / Math.Sqrt(r2 + z * z);
        v = DefineConstants.RE_WGS84 / Math.Sqrt(1.0 - e2 * sinp * sinp);
        z = r[2] + v * e2 * sinp;
    }
    pos[0] = r2 > 1E-12 ? Math.Atan(z / Math.Sqrt(r2)) : (r[2] > 0.0 ? DefineConstants.PI / 2.0 : -DefineConstants.PI / 2.0);
    pos[1] = r2 > 1E-12 ? Math.Atan2(r[1], r[0]) : 0.0;
    pos[2] = Math.Sqrt(r2 + z * z) - v;
}
/* transform geodetic to ecef position -----------------------------------------
* transform geodetic position to ecef position
* args   : double *pos      I   geodetic position {lat,lon,h} (rad,m)
*          double *r        O   ecef position {x,y,z} (m)
* return : none
* notes  : WGS84, ellipsoidal height
*-----------------------------------------------------------------------------*/
public static void pos2ecef(double[] pos, double[] r)
{
    double sinp = Math.Sin(pos[0]);
    double cosp = Math.Cos(pos[0]);
    double sinl = Math.Sin(pos[1]);
    double cosl = Math.Cos(pos[1]);
    double e2 = 1.0 / 298.257223563 * (2.0 - 1.0 / 298.257223563);
    double v = DefineConstants.RE_WGS84 / Math.Sqrt(1.0 - e2 * sinp * sinp);

    r[0] = (v + pos[2]) * cosp * cosl;
    r[1] = (v + pos[2]) * cosp * sinl;
    r[2] = (v * (1.0 - e2) + pos[2]) * sinp;
}
/* ecef to local coordinate transfromation matrix ------------------------------
* compute ecef to local coordinate transfromation matrix
* args   : double *pos      I   geodetic position {lat,lon} (rad)
*          double *E        O   ecef to local coord transformation matrix (3x3)
* return : none
* notes  : matirix stored by column-major order (fortran convention)
*-----------------------------------------------------------------------------*/
public static void xyz2enu(double[] pos, double[] E)
{
    double sinp = Math.Sin(pos[0]);
    double cosp = Math.Cos(pos[0]);
    double sinl = Math.Sin(pos[1]);
    double cosl = Math.Cos(pos[1]);

    E[0] = -sinl;
    E[3] = cosl;
    E[6] = 0.0;
    E[1] = -sinp * cosl;
    E[4] = -sinp * sinl;
    E[7] = cosp;
    E[2] = cosp * cosl;
    E[5] = cosp * sinl;
    E[8] = sinp;
}
/* transform ecef vector to local tangental coordinate -------------------------
* transform ecef vector to local tangental coordinate
* args   : double *pos      I   geodetic position {lat,lon} (rad)
*          double *r        I   vector in ecef coordinate {x,y,z}
*          double *e        O   vector in local tangental coordinate {e,n,u}
* return : none
*-----------------------------------------------------------------------------*/
public static void ecef2enu(double pos, double r, ref double e)
{
    double[] E = new double[9];

    GlobalMembersRtkcmn.xyz2enu(pos, E);
    GlobalMembersRtkcmn.matmul("NN", 3, 1, 3, 1.0, E, r, 0.0, ref e);
}
/* transform local vector to ecef coordinate -----------------------------------
* transform local tangental coordinate vector to ecef
* args   : double *pos      I   geodetic position {lat,lon} (rad)
*          double *e        I   vector in local tangental coordinate {e,n,u}
*          double *r        O   vector in ecef coordinate {x,y,z}
* return : none
*-----------------------------------------------------------------------------*/
public static void enu2ecef(double pos, double e, ref double r)
{
    double[] E = new double[9];

    GlobalMembersRtkcmn.xyz2enu(pos, E);
    GlobalMembersRtkcmn.matmul("TN", 3, 1, 3, 1.0, E, e, 0.0, ref r);
}
/* transform covariance to local tangental coordinate --------------------------
* transform ecef covariance to local tangental coordinate
* args   : double *pos      I   geodetic position {lat,lon} (rad)
*          double *P        I   covariance in ecef coordinate
*          double *Q        O   covariance in local tangental coordinate
* return : none
*-----------------------------------------------------------------------------*/
public static void covenu(double pos, double P, ref double Q)
{
    double[] E = new double[9];
    double[] EP = new double[9];

    GlobalMembersRtkcmn.xyz2enu(pos, E);
    GlobalMembersRtkcmn.matmul("NN", 3, 3, 3, 1.0, E, P, 0.0, ref EP);
    GlobalMembersRtkcmn.matmul("NT", 3, 3, 3, 1.0, EP, E, 0.0, ref Q);
}
/* transform local enu coordinate covariance to xyz-ecef -----------------------
* transform local enu covariance to xyz-ecef coordinate
* args   : double *pos      I   geodetic position {lat,lon} (rad)
*          double *Q        I   covariance in local enu coordinate
*          double *P        O   covariance in xyz-ecef coordinate
* return : none
*-----------------------------------------------------------------------------*/
public static void covecef(double pos, double Q, ref double P)
{
    double[] E = new double[9];
    double[] EQ = new double[9];

    GlobalMembersRtkcmn.xyz2enu(pos, E);
    GlobalMembersRtkcmn.matmul("TN", 3, 3, 3, 1.0, E, Q, 0.0, ref EQ);
    GlobalMembersRtkcmn.matmul("NN", 3, 3, 3, 1.0, EQ, E, 0.0, ref P);
}


/* astronomical arguments: f={l,l',F,D,OMG} (rad) ----------------------------*/
internal static void ast_args(double t, double[] f)
{
    double[,] fc = { { 134.96340251, 1717915923.2178, 31.8792, 0.051635, -0.00024470 }, { 357.52910918, 129596581.0481, -0.5532, 0.000136, -0.00001149 }, { 93.27209062, 1739527262.8478, -12.7512, -0.001037, 0.00000417 }, { 297.85019547, 1602961601.2090, -6.3706, 0.006593, -0.00003169 }, { 125.04455501, -6962890.2665, 7.4722, 0.007702, -0.00005939 } }; // coefficients for iau 1980 nutation
    double[] tt = new double[4];
    int i;
    int j;

    for (tt[0] = t, i = 1; i < 4; i++)
    {
        tt[i] = tt[i - 1] * t;
    }
    for (i = 0; i < 5; i++)
    {
        f[i] = fc[i, 0] * 3600.0;
        for (j = 0; j < 4; j++)
        {
            f[i] += fc[i, j + 1] * tt[j];
        }
        f[i] = Math.IEEERemainder(f[i] * DefineConstants.PI / 180.0 / 3600.0, 2.0 * DefineConstants.PI);
    }
}
/* iau 1980 nutation ---------------------------------------------------------*/
//C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on the parameter 'dpsi', so pointers on this parameter are left unchanged:
//C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on the parameter 'deps', so pointers on this parameter are left unchanged:
internal static void nut_iau1980(double t, double[] f, double* dpsi, double* deps)
{
    double[,] nut = { { 0, 0, 0, 0, 1, -6798.4, -171996, -174.2, 92025, 8.9 }, { 0, 0, 2, -2, 2, 182.6, -13187, -1.6, 5736, -3.1 }, { 0, 0, 2, 0, 2, 13.7, -2274, -0.2, 977, -0.5 }, { 0, 0, 0, 0, 2, -3399.2, 2062, 0.2, -895, 0.5 }, { 0, -1, 0, 0, 0, -365.3, -1426, 3.4, 54, -0.1 }, { 1, 0, 0, 0, 0, 27.6, 712, 0.1, -7, 0.0 }, { 0, 1, 2, -2, 2, 121.7, -517, 1.2, 224, -0.6 }, { 0, 0, 2, 0, 1, 13.6, -386, -0.4, 200, 0.0 }, { 1, 0, 2, 0, 2, 9.1, -301, 0.0, 129, -0.1 }, { 0, -1, 2, -2, 2, 365.2, 217, -0.5, -95, 0.3 }, { -1, 0, 0, 2, 0, 31.8, 158, 0.0, -1, 0.0 }, { 0, 0, 2, -2, 1, 177.8, 129, 0.1, -70, 0.0 }, { -1, 0, 2, 0, 2, 27.1, 123, 0.0, -53, 0.0 }, { 1, 0, 0, 0, 1, 27.7, 63, 0.1, -33, 0.0 }, { 0, 0, 0, 2, 0, 14.8, 63, 0.0, -2, 0.0 }, { -1, 0, 2, 2, 2, 9.6, -59, 0.0, 26, 0.0 }, { -1, 0, 0, 0, 1, -27.4, -58, -0.1, 32, 0.0 }, { 1, 0, 2, 0, 1, 9.1, -51, 0.0, 27, 0.0 }, { -2, 0, 0, 2, 0, -205.9, -48, 0.0, 1, 0.0 }, { -2, 0, 2, 0, 1, 1305.5, 46, 0.0, -24, 0.0 }, { 0, 0, 2, 2, 2, 7.1, -38, 0.0, 16, 0.0 }, { 2, 0, 2, 0, 2, 6.9, -31, 0.0, 13, 0.0 }, { 2, 0, 0, 0, 0, 13.8, 29, 0.0, -1, 0.0 }, { 1, 0, 2, -2, 2, 23.9, 29, 0.0, -12, 0.0 }, { 0, 0, 2, 0, 0, 13.6, 26, 0.0, -1, 0.0 }, { 0, 0, 2, -2, 0, 173.3, -22, 0.0, 0, 0.0 }, { -1, 0, 2, 0, 1, 27.0, 21, 0.0, -10, 0.0 }, { 0, 2, 0, 0, 0, 182.6, 17, -0.1, 0, 0.0 }, { 0, 2, 2, -2, 2, 91.3, -16, 0.1, 7, 0.0 }, { -1, 0, 0, 2, 1, 32.0, 16, 0.0, -8, 0.0 }, { 0, 1, 0, 0, 1, 386.0, -15, 0.0, 9, 0.0 }, { 1, 0, 0, -2, 1, -31.7, -13, 0.0, 7, 0.0 }, { 0, -1, 0, 0, 1, -346.6, -12, 0.0, 6, 0.0 }, { 2, 0, -2, 0, 0, -1095.2, 11, 0.0, 0, 0.0 }, { -1, 0, 2, 2, 1, 9.5, -10, 0.0, 5, 0.0 }, { 1, 0, 2, 2, 2, 5.6, -8, 0.0, 3, 0.0 }, { 0, -1, 2, 0, 2, 14.2, -7, 0.0, 3, 0.0 }, { 0, 0, 2, 2, 1, 7.1, -7, 0.0, 3, 0.0 }, { 1, 1, 0, -2, 0, -34.8, -7, 0.0, 0, 0.0 }, { 0, 1, 2, 0, 2, 13.2, 7, 0.0, -3, 0.0 }, { -2, 0, 0, 2, 1, -199.8, -6, 0.0, 3, 0.0 }, { 0, 0, 0, 2, 1, 14.8, -6, 0.0, 3, 0.0 }, { 2, 0, 2, -2, 2, 12.8, 6, 0.0, -3, 0.0 }, { 1, 0, 0, 2, 0, 9.6, 6, 0.0, 0, 0.0 }, { 1, 0, 2, -2, 1, 23.9, 6, 0.0, -3, 0.0 }, { 0, 0, 0, -2, 1, -14.7, -5, 0.0, 3, 0.0 }, { 0, -1, 2, -2, 1, 346.6, -5, 0.0, 3, 0.0 }, { 2, 0, 2, 0, 1, 6.9, -5, 0.0, 3, 0.0 }, { 1, -1, 0, 0, 0, 29.8, 5, 0.0, 0, 0.0 }, { 1, 0, 0, -1, 0, 411.8, -4, 0.0, 0, 0.0 }, { 0, 0, 0, 1, 0, 29.5, -4, 0.0, 0, 0.0 }, { 0, 1, 0, -2, 0, -15.4, -4, 0.0, 0, 0.0 }, { 1, 0, -2, 0, 0, -26.9, 4, 0.0, 0, 0.0 }, { 2, 0, 0, -2, 1, 212.3, 4, 0.0, -2, 0.0 }, { 0, 1, 2, -2, 1, 119.6, 4, 0.0, -2, 0.0 }, { 1, 1, 0, 0, 0, 25.6, -3, 0.0, 0, 0.0 }, { 1, -1, 0, -1, 0, -3232.9, -3, 0.0, 0, 0.0 }, { -1, -1, 2, 2, 2, 9.8, -3, 0.0, 1, 0.0 }, { 0, -1, 2, 2, 2, 7.2, -3, 0.0, 1, 0.0 }, { 1, -1, 2, 0, 2, 9.4, -3, 0.0, 1, 0.0 }, { 3, 0, 2, 0, 2, 5.5, -3, 0.0, 1, 0.0 }, { -2, 0, 2, 0, 2, 1615.7, -3, 0.0, 1, 0.0 }, { 1, 0, 2, 0, 0, 9.1, 3, 0.0, 0, 0.0 }, { -1, 0, 2, 4, 2, 5.8, -2, 0.0, 1, 0.0 }, { 1, 0, 0, 0, 2, 27.8, -2, 0.0, 1, 0.0 }, { -1, 0, 2, -2, 1, -32.6, -2, 0.0, 1, 0.0 }, { 0, -2, 2, -2, 1, 6786.3, -2, 0.0, 1, 0.0 }, { -2, 0, 0, 0, 1, -13.7, -2, 0.0, 1, 0.0 }, { 2, 0, 0, 0, 1, 13.8, 2, 0.0, -1, 0.0 }, { 3, 0, 0, 0, 0, 9.2, 2, 0.0, 0, 0.0 }, { 1, 1, 2, 0, 2, 8.9, 2, 0.0, -1, 0.0 }, { 0, 0, 2, 1, 2, 9.3, 2, 0.0, -1, 0.0 }, { 1, 0, 0, 2, 1, 9.6, -1, 0.0, 0, 0.0 }, { 1, 0, 2, 2, 1, 5.6, -1, 0.0, 1, 0.0 }, { 1, 1, 0, -2, 1, -34.7, -1, 0.0, 0, 0.0 }, { 0, 1, 0, 2, 0, 14.2, -1, 0.0, 0, 0.0 }, { 0, 1, 2, -2, 0, 117.5, -1, 0.0, 0, 0.0 }, { 0, 1, -2, 2, 0, -329.8, -1, 0.0, 0, 0.0 }, { 1, 0, -2, 2, 0, 23.8, -1, 0.0, 0, 0.0 }, { 1, 0, -2, -2, 0, -9.5, -1, 0.0, 0, 0.0 }, { 1, 0, 2, -2, 0, 32.8, -1, 0.0, 0, 0.0 }, { 1, 0, 0, -4, 0, -10.1, -1, 0.0, 0, 0.0 }, { 2, 0, 0, -4, 0, -15.9, -1, 0.0, 0, 0.0 }, { 0, 0, 2, 4, 2, 4.8, -1, 0.0, 0, 0.0 }, { 0, 0, 2, -1, 2, 25.4, -1, 0.0, 0, 0.0 }, { -2, 0, 2, 4, 2, 7.3, -1, 0.0, 1, 0.0 }, { 2, 0, 2, 2, 2, 4.7, -1, 0.0, 0, 0.0 }, { 0, -1, 2, 0, 1, 14.2, -1, 0.0, 0, 0.0 }, { 0, 0, -2, 0, 1, -13.6, -1, 0.0, 0, 0.0 }, { 0, 0, 4, -2, 2, 12.7, 1, 0.0, 0, 0.0 }, { 0, 1, 0, 0, 2, 409.2, 1, 0.0, 0, 0.0 }, { 1, 1, 2, -2, 2, 22.5, 1, 0.0, -1, 0.0 }, { 3, 0, 2, -2, 2, 8.7, 1, 0.0, 0, 0.0 }, { -2, 0, 2, 2, 2, 14.6, 1, 0.0, -1, 0.0 }, { -1, 0, 0, 0, 2, -27.3, 1, 0.0, -1, 0.0 }, { 0, 0, -2, 2, 1, -169.0, 1, 0.0, 0, 0.0 }, { 0, 1, 2, 0, 1, 13.1, 1, 0.0, 0, 0.0 }, { -1, 0, 4, 0, 2, 9.1, 1, 0.0, 0, 0.0 }, { 2, 1, 0, -2, 0, 131.7, 1, 0.0, 0, 0.0 }, { 2, 0, 0, 2, 0, 7.1, 1, 0.0, 0, 0.0 }, { 2, 0, 2, -2, 1, 12.8, 1, 0.0, -1, 0.0 }, { 2, 0, -2, 0, 1, -943.2, 1, 0.0, 0, 0.0 }, { 1, -1, 0, -2, 0, -29.3, 1, 0.0, 0, 0.0 }, { -1, 0, 0, 1, 1, -388.3, 1, 0.0, 0, 0.0 }, { -1, -1, 0, 2, 1, 35.0, 1, 0.0, 0, 0.0 }, { 0, 1, 0, 1, 0, 27.3, 1, 0.0, 0, 0.0 } };
    double ang;
    int i;
    int j;

    *dpsi = deps = 0.0;

    for (i = 0; i < 106; i++)
    {
        ang = 0.0;
        for (j = 0; j < 5; j++)
        {
            ang += nut[i, j] * f[j];
        }
        *dpsi += (nut[i, 6] + nut[i, 7] * t) * Math.Sin(ang);
        *deps += (nut[i, 8] + nut[i, 9] * t) * Math.Cos(ang);
    }
    *dpsi *= 1E-4 * DefineConstants.PI / 180.0 / 3600.0; // 0.1 mas -> rad
    *deps *= 1E-4 * DefineConstants.PI / 180.0 / 3600.0;
}
//C++ TO C# CONVERTER NOTE: This was formerly a static local variable declaration (not allowed in C#):
private static gtime_t eci2ecef_tutc_ = new gtime_t();
//C++ TO C# CONVERTER NOTE: This was formerly a static local variable declaration (not allowed in C#):
private static double[] eci2ecef_U_ = new double[9];
double gmst_;
/* eci to ecef transformation matrix -------------------------------------------
* compute eci to ecef transformation matrix
* args   : gtime_t tutc     I   time in utc
*          double *erpv     I   erp values {xp,yp,ut1_utc,lod} (rad,rad,s,s/d)
*          double *U        O   eci to ecef transformation matrix (3 x 3)
*          double *gmst     IO  greenwich mean sidereal time (rad)
*                               (NULL: no output)
* return : none
* note   : see ref [3] chap 5
*          not thread-safe
*-----------------------------------------------------------------------------*/
public static void eci2ecef(gtime_t tutc, double[] erpv, double[] U, ref double gmst)
{
    double[] ep2000 = { 2000, 1, 1, 12, 0, 0 };
    //C++ TO C# CONVERTER NOTE: This static local variable declaration (not allowed in C#) has been moved just prior to the method:
    //	static gtime_t tutc_;
    //C++ TO C# CONVERTER NOTE: This static local variable declaration (not allowed in C#) has been moved just prior to the method:
    //	static double U_[9],gmst_;
    gtime_t tgps = new gtime_t();
    double eps;
    double ze;
    double th;
    double z;
    double t;
    double t2;
    double t3;
    double dpsi;
    double deps;
    double gast;
    double[] f = new double[5];
    double[] R1 = new double[9];
    double[] R2 = new double[9];
    double[] R3 = new double[9];
    double[] R = new double[9];
    double[] W = new double[9];
    double[] N = new double[9];
    double[] P = new double[9];
    double[] NP = new double[9];
    int i;

    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
    //ORIGINAL LINE: trace(3,"eci2ecef: tutc=%s\n",time_str(tutc,3));
    GlobalMembersRtkcmn.trace(3, "eci2ecef: tutc=%s\n", GlobalMembersRtkcmn.time_str(new gtime_t(tutc), 3));

    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
    //ORIGINAL LINE: if (fabs(timediff(tutc,tutc_))<0.01)
    if (Math.Abs(GlobalMembersRtkcmn.timediff(new gtime_t(tutc), eci2ecef_tutc_)) < 0.01) // read cache
    {
        for (i = 0; i < 9; i++)
        {
            U[i] = eci2ecef_U_[i];
        }
        if (gmst != 0)
        {
            gmst = gmst_;
        }
        return;
    }
    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
    //ORIGINAL LINE: tutc_=tutc;
    eci2ecef_tutc_.CopyFrom(tutc);

    /* terrestrial time */
    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
    //ORIGINAL LINE: tgps=utc2gpst(tutc_);
    tgps.CopyFrom(GlobalMembersRtkcmn.utc2gpst(eci2ecef_tutc_));
    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
    //ORIGINAL LINE: t=(timediff(tgps,epoch2time(ep2000))+19.0+32.184)/86400.0/36525.0;
    t = (GlobalMembersRtkcmn.timediff(new gtime_t(tgps), GlobalMembersRtkcmn.epoch2time(ep2000)) + 19.0 + 32.184) / 86400.0 / 36525.0;
    t2 = t * t;
    t3 = t2 * t;

    /* astronomical arguments */
    GlobalMembersRtkcmn.ast_args(t, f);

    /* iau 1976 precession */
    ze = (2306.2181 * t + 0.30188 * t2 + 0.017998 * t3) * DefineConstants.PI / 180.0 / 3600.0;
    th = (2004.3109 * t - 0.42665 * t2 - 0.041833 * t3) * DefineConstants.PI / 180.0 / 3600.0;
    z = (2306.2181 * t + 1.09468 * t2 + 0.018203 * t3) * DefineConstants.PI / 180.0 / 3600.0;
    eps = (84381.448 - 46.8150 * t - 0.00059 * t2 + 0.001813 * t3) * DefineConstants.PI / 180.0 / 3600.0;
    do
    {
        (R1)[8] = 1.0;
        (R1)[2] = (R1)[5] = (R1)[6] = (R1)[7] = 0.0;
        (R1)[0] = (R1)[4] = Math.Cos(-z);
        (R1)[3] = Math.Sin(-z);
        (R1)[1] = -(R1)[3];
    } while (0);
    do
    {
        (R2)[4] = 1.0;
        (R2)[1] = (R2)[3] = (R2)[5] = (R2)[7] = 0.0;
        (R2)[0] = (R2)[8] = Math.Cos(th);
        (R2)[2] = Math.Sin(th);
        (R2)[6] = -(R2)[2];
    } while (0);
    do
    {
        (R3)[8] = 1.0;
        (R3)[2] = (R3)[5] = (R3)[6] = (R3)[7] = 0.0;
        (R3)[0] = (R3)[4] = Math.Cos(-ze);
        (R3)[3] = Math.Sin(-ze);
        (R3)[1] = -(R3)[3];
    } while (0);
    GlobalMembersRtkcmn.matmul("NN", 3, 3, 3, 1.0, R1, R2, 0.0, ref R);
    GlobalMembersRtkcmn.matmul("NN", 3, 3, 3, 1.0, R, R3, 0.0, ref P); // P=Rz(-z)*Ry(th)*Rz(-ze)

    /* iau 1980 nutation */
    GlobalMembersRtkcmn.nut_iau1980(t, f, dpsi, deps);
    do
    {
        (R1)[0] = 1.0;
        (R1)[1] = (R1)[2] = (R1)[3] = (R1)[6] = 0.0;
        (R1)[4] = (R1)[8] = Math.Cos(-eps - deps);
        (R1)[7] = Math.Sin(-eps - deps);
        (R1)[5] = -(R1)[7];
    } while (0);
    do
    {
        (R2)[8] = 1.0;
        (R2)[2] = (R2)[5] = (R2)[6] = (R2)[7] = 0.0;
        (R2)[0] = (R2)[4] = Math.Cos(-dpsi);
        (R2)[3] = Math.Sin(-dpsi);
        (R2)[1] = -(R2)[3];
    } while (0);
    do
    {
        (R3)[0] = 1.0;
        (R3)[1] = (R3)[2] = (R3)[3] = (R3)[6] = 0.0;
        (R3)[4] = (R3)[8] = Math.Cos(eps);
        (R3)[7] = Math.Sin(eps);
        (R3)[5] = -(R3)[7];
    } while (0);
    GlobalMembersRtkcmn.matmul("NN", 3, 3, 3, 1.0, R1, R2, 0.0, ref R);
    GlobalMembersRtkcmn.matmul("NN", 3, 3, 3, 1.0, R, R3, 0.0, ref N); // N=Rx(-eps)*Rz(-dspi)*Rx(eps)

    /* greenwich aparent sidereal time (rad) */
    gmst_ = GlobalMembersRtkcmn.utc2gmst(eci2ecef_tutc_, erpv[2]);
    gast = gmst_ + dpsi * Math.Cos(eps);
    gast += (0.00264 * Math.Sin(f[4]) + 0.000063 * Math.Sin(2.0 * f[4])) * DefineConstants.PI / 180.0 / 3600.0;

    /* eci to ecef transformation matrix */
    do
    {
        (R1)[4] = 1.0;
        (R1)[1] = (R1)[3] = (R1)[5] = (R1)[7] = 0.0;
        (R1)[0] = (R1)[8] = Math.Cos(-erpv[0]);
        (R1)[2] = Math.Sin(-erpv[0]);
        (R1)[6] = -(R1)[2];
    } while (0);
    do
    {
        (R2)[0] = 1.0;
        (R2)[1] = (R2)[2] = (R2)[3] = (R2)[6] = 0.0;
        (R2)[4] = (R2)[8] = Math.Cos(-erpv[1]);
        (R2)[7] = Math.Sin(-erpv[1]);
        (R2)[5] = -(R2)[7];
    } while (0);
    do
    {
        (R3)[8] = 1.0;
        (R3)[2] = (R3)[5] = (R3)[6] = (R3)[7] = 0.0;
        (R3)[0] = (R3)[4] = Math.Cos(gast);
        (R3)[3] = Math.Sin(gast);
        (R3)[1] = -(R3)[3];
    } while (0);
    GlobalMembersRtkcmn.matmul("NN", 3, 3, 3, 1.0, R1, R2, 0.0, ref W);
    GlobalMembersRtkcmn.matmul("NN", 3, 3, 3, 1.0, W, R3, 0.0, ref R); // W=Ry(-xp)*Rx(-yp)
    GlobalMembersRtkcmn.matmul("NN", 3, 3, 3, 1.0, N, P, 0.0, ref NP);
    GlobalMembersRtkcmn.matmul("NN", 3, 3, 3, 1.0, R, NP, 0.0, ref eci2ecef_U_); // U=W*Rz(gast)*N*P

    for (i = 0; i < 9; i++)
    {
        U[i] = eci2ecef_U_[i];
    }
    if (gmst != 0)
    {
        gmst = gmst_;
    }

    GlobalMembersRtkcmn.trace(5, "gmst=%.12f gast=%.12f\n", gmst_, gast);
    GlobalMembersRtkcmn.trace(5, "P=\n");
    GlobalMembersRtkcmn.tracemat(5, P, 3, 3, 15, 12);
    GlobalMembersRtkcmn.trace(5, "N=\n");
    GlobalMembersRtkcmn.tracemat(5, N, 3, 3, 15, 12);
    GlobalMembersRtkcmn.trace(5, "W=\n");
    GlobalMembersRtkcmn.tracemat(5, W, 3, 3, 15, 12);
    GlobalMembersRtkcmn.trace(5, "U=\n");
    GlobalMembersRtkcmn.tracemat(5, U, 3, 3, 15, 12);
}
/* decode antenna parameter field --------------------------------------------*/
internal static int decodef(ref string p, int n, double[] v)
{
    int i;

    for (i = 0; i < n; i++)
    {
        v[i] = 0.0;
    }
    for (i = 0, p = StringFunctions.StrTok(p, " "); p && i < n; p = StringFunctions.StrTok(null, " "))
    {
        v[i++] = Convert.ToDouble(p) * 1E-3;
    }
    return i;
}
/* add antenna parameter -----------------------------------------------------*/
internal static void addpcv(pcv_t[] pcv, pcvs_t pcvs)
{
    pcv_t pcvs_pcv;

    if (pcvs.nmax <= pcvs.n)
    {
        pcvs.nmax += 256;
        //C++ TO C# CONVERTER TODO TASK: The memory management function 'realloc' has no equivalent in C#:
        if ((pcvs_pcv = (pcv_t)realloc(pcvs.pcv, sizeof(pcv_t) * pcvs.nmax)) == null)
        {
            GlobalMembersRtkcmn.trace(1, "addpcv: memory allocation error\n");
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(pcvs.pcv);
            pcvs.pcv = null;
            pcvs.n = pcvs.nmax = 0;
            return;
        }
        pcvs.pcv = pcvs_pcv;
    }
    pcvs.pcv[pcvs.n++] = *pcv;
}
/* read ngs antenna parameter file -------------------------------------------*/
internal static int readngspcv(string file, pcvs_t pcvs)
{
    FILE fp;
    const pcv_t pcv0 = new pcv_t();
    pcv_t pcv = new pcv_t();
    double[] neu = new double[3];
    int n = 0;
    string buff = new string(new char[256]);

    if ((fp = fopen(file, "r")) == null)
    {
        GlobalMembersRtkcmn.trace(2, "ngs pcv file open error: %s\n", file);
        return 0;
    }
    while (fgets(buff, sizeof(sbyte), fp))
    {

        if (buff.Length >= 62 && buff[61] == '|')
            continue;

        if (buff[0] != ' ') // start line
        {
            n = 0;
        }
        if (++n == 1)
        {
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: pcv=pcv0;
            pcv.CopyFrom(pcv0);
            pcv.type = buff.Substring(0, 61);
            pcv.type[61] = '\0';
        }
        else if (n == 2)
        {
            if (GlobalMembersRtkcmn.decodef(ref buff, 3, neu) < 3)
                continue;
            pcv.off[0, 0] = neu[1];
            pcv.off[0, 1] = neu[0];
            pcv.off[0, 2] = neu[2];
        }
        else if (n == 3)
        {
            GlobalMembersRtkcmn.decodef(ref buff, 10, pcv.@var[0]);
        }
        else if (n == 4)
        {
            GlobalMembersRtkcmn.decodef(ref buff, 9, pcv.@var[0] + 10);
        }
        else if (n == 5)
        {
            if (GlobalMembersRtkcmn.decodef(ref buff, 3, neu) < 3)
                continue;
            pcv.off[1, 0] = neu[1];
            pcv.off[1, 1] = neu[0];
            pcv.off[1, 2] = neu[2];
        }
        else if (n == 6)
        {
            GlobalMembersRtkcmn.decodef(ref buff, 10, pcv.@var[1]);
        }
        else if (n == 7)
        {
            GlobalMembersRtkcmn.decodef(ref buff, 9, pcv.@var[1] + 10);
            GlobalMembersRtkcmn.addpcv(pcv, pcvs);
        }
    }
    fclose(fp);

    return 1;
}
/* read antex file ----------------------------------------------------------*/
internal static int readantex(string file, pcvs_t pcvs)
{
    FILE fp;
    const pcv_t pcv0 = new pcv_t();
    pcv_t pcv = new pcv_t();
    double[] neu = new double[3];
    int i;
    int f;
    int freq = 0;
    int state = 0;
    int[] freqs = { 1, 2, 5, 6, 7, 8, 0 };
    string buff = new string(new char[256]);

    GlobalMembersRtkcmn.trace(3, "readantex: file=%s\n", file);

    if ((fp = fopen(file, "r")) == null)
    {
        GlobalMembersRtkcmn.trace(2, "antex pcv file open error: %s\n", file);
        return 0;
    }
    while (fgets(buff, sizeof(sbyte), fp))
    {

        if (buff.Length < 60 || StringFunctions.StrStr(buff.Substring(60), "COMMENT"))
            continue;

        if (StringFunctions.StrStr(buff.Substring(60), "START OF ANTENNA"))
        {
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: pcv=pcv0;
            pcv.CopyFrom(pcv0);
            state = 1;
        }
        if (StringFunctions.StrStr(buff.Substring(60), "END OF ANTENNA"))
        {
            GlobalMembersRtkcmn.addpcv(pcv, pcvs);
            state = 0;
        }
        if (state == 0)
            continue;

        if (StringFunctions.StrStr(buff.Substring(60), "TYPE / SERIAL NO"))
        {
            pcv.type = buff.Substring(0, 20);
            pcv.type[20] = '\0';
            pcv.code = buff.Substring(20, 20);
            pcv.code[20] = '\0';
            if (!string.Compare(pcv.code, 3, "        ", 0, 8))
            {
                pcv.sat = GlobalMembersRtkcmn.satid2no(pcv.code);
            }
        }
        else if (StringFunctions.StrStr(buff.Substring(60), "VALID FROM"))
        {
            if (GlobalMembersRtkcmn.str2time(buff, 0, 43, pcv.ts) == 0)
                continue;
        }
        else if (StringFunctions.StrStr(buff.Substring(60), "VALID UNTIL"))
        {
            if (GlobalMembersRtkcmn.str2time(buff, 0, 43, pcv.te) == 0)
                continue;
        }
        else if (StringFunctions.StrStr(buff.Substring(60), "START OF FREQUENCY"))
        {
            if (sscanf(buff.Substring(4), "%d", f) < 1)
                continue;
            for (i = 0; i < DefineConstants.NFREQ; i++)
            {
                if (freqs[i] == f)
                    break;
            }
            if (i < DefineConstants.NFREQ)
            {
                freq = i + 1;
            }
        }
        else if (StringFunctions.StrStr(buff.Substring(60), "END OF FREQUENCY"))
        {
            freq = 0;
        }
        else if (StringFunctions.StrStr(buff.Substring(60), "NORTH / EAST / UP"))
        {
            if (freq < 1 || DefineConstants.NFREQ < freq)
                continue;
            if (GlobalMembersRtkcmn.decodef(ref buff, 3, neu) < 3)
                continue;
            pcv.off[freq - 1, 0] = neu[pcv.sat != 0 ? 0 : 1]; // x or e
            pcv.off[freq - 1, 1] = neu[pcv.sat != 0 ? 1 : 0]; // y or n
            pcv.off[freq - 1, 2] = neu[2]; // z or u
        }
        else if (StringFunctions.StrStr(buff, "NOAZI"))
        {
            if (freq < 1 || DefineConstants.NFREQ < freq)
                continue;
            if ((i = GlobalMembersRtkcmn.decodef(ref buff.Substring(8), 19, pcv.@var[freq - 1])) <= 0)
                continue;
            for (; i < 19; i++)
            {
                pcv.@var[freq - 1, i] = pcv.@var[freq - 1, i - 1];
            }
        }
    }
    fclose(fp);

    return 1;
}
/* read antenna parameters ------------------------------------------------------
* read antenna parameters
* args   : char   *file       I   antenna parameter file (antex)
*          pcvs_t *pcvs       IO  antenna parameters
* return : status (1:ok,0:file open error)
* notes  : file with the externsion .atx or .ATX is recognized as antex
*          file except for antex is recognized ngs antenna parameters
*          see reference [3]
*          only support non-azimuth-depedent parameters
*-----------------------------------------------------------------------------*/
public static int readpcv(string file, pcvs_t pcvs)
{
    pcv_t pcv;
    string ext;
    int i;
    int stat;

    GlobalMembersRtkcmn.trace(3, "readpcv: file=%s\n", file);

    if ((ext = StringFunctions.StrRChr(file, '.')) == 0)
    {
        ext = "";
    }

    if (!string.Compare(ext, ".atx") || !string.Compare(ext, ".ATX"))
    {
        stat = GlobalMembersRtkcmn.readantex(file, pcvs);
    }
    else
    {
        stat = GlobalMembersRtkcmn.readngspcv(file, pcvs);
    }
    for (i = 0; i < pcvs.n; i++)
    {
        pcv = pcvs.pcv + i;
        GlobalMembersRtkcmn.trace(4, "sat=%2d type=%20s code=%s off=%8.4f %8.4f %8.4f  %8.4f %8.4f %8.4f\n", pcv.sat, pcv.type, pcv.code, pcv.off[0, 0], pcv.off[0, 1], pcv.off[0, 2], pcv.off[1, 0], pcv.off[1, 1], pcv.off[1, 2]);
    }
    return stat;
}
/* search antenna parameter ----------------------------------------------------
* read satellite antenna phase center position
* args   : int    sat         I   satellite number (0: receiver antenna)
*          char   *type       I   antenna type for receiver antenna
*          gtime_t time       I   time to search parameters
*          pcvs_t *pcvs       IO  antenna parameters
* return : antenna parameter (NULL: no antenna)
*-----------------------------------------------------------------------------*/
public static pcv_t searchpcv(int sat, string type, gtime_t time, pcvs_t pcvs)
{
    pcv_t pcv;
    string buff = new string(new char[DefineConstants.MAXANT]);
    string[] types = new string[2];
    string p;
    int i;
    int j;
    int n = 0;

    GlobalMembersRtkcmn.trace(3, "searchpcv: sat=%2d type=%s\n", sat, type);

    if (sat != 0) // search satellite antenna
    {
        for (i = 0; i < pcvs.n; i++)
        {
            pcv = pcvs.pcv + i;
            if (pcv.sat != sat)
                continue;
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: if (pcv->ts.time!=0&&timediff(pcv->ts,time)>0.0)
            if (pcv.ts.time != 0 && GlobalMembersRtkcmn.timediff(new gtime_t(pcv.ts), new gtime_t(time)) > 0.0)
                continue;
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: if (pcv->te.time!=0&&timediff(pcv->te,time)<0.0)
            if (pcv.te.time != 0 && GlobalMembersRtkcmn.timediff(new gtime_t(pcv.te), new gtime_t(time)) < 0.0)
                continue;
            return pcv;
        }
    }
    else
    {
        buff = type;
        for (p = StringFunctions.StrTok(buff, " "); p && n < 2; p = StringFunctions.StrTok(null, " "))
        {
            types[n++] = p;
        }
        if (n <= 0)
        {
            return null;
        }

        /* search receiver antenna with radome at first */
        for (i = 0; i < pcvs.n; i++)
        {
            pcv = pcvs.pcv + i;
            for (j = 0; j < n; j++)
            {
                if (!StringFunctions.StrStr(pcv.type, types[j]))
                    break;
            }
            if (j >= n)
            {
                return pcv;
            }
        }
        /* search receiver antenna without radome */
        for (i = 0; i < pcvs.n; i++)
        {
            pcv = pcvs.pcv + i;
            if (StringFunctions.StrStr(pcv.type, types[0]) != pcv.type)
                continue;

            GlobalMembersRtkcmn.trace(2, "pcv without radome is used type=%s\n", type);
            return pcv;
        }
    }
    return null;
}
//C++ TO C# CONVERTER NOTE: This was formerly a static local variable declaration (not allowed in C#):
private static double[,] readpos_poss = new double[2048, 3];
//C++ TO C# CONVERTER NOTE: This was formerly a static local variable declaration (not allowed in C#):
private static sbyte[,] readpos_stas = new sbyte[2048, 16];
/* read station positions ------------------------------------------------------
* read positions from station position file
* args   : char  *file      I   station position file containing
*                               lat(deg) lon(deg) height(m) name in a line
*          char  *rcvs      I   station name
*          double *pos      O   station position {lat,lon,h} (rad/m)
*                               (all 0 if search error)
* return : none
*-----------------------------------------------------------------------------*/
public static void readpos(string file, string rcv, double[] pos)
{
    //C++ TO C# CONVERTER NOTE: This static local variable declaration (not allowed in C#) has been moved just prior to the method:
    //	static double poss[2048][3];
    //C++ TO C# CONVERTER NOTE: This static local variable declaration (not allowed in C#) has been moved just prior to the method:
    //	static sbyte stas[2048][16];
    FILE fp;
    int i;
    int j;
    int len;
    int np = 0;
    string buff = new string(new char[256]);
    string str = new string(new char[256]);

    GlobalMembersRtkcmn.trace(3, "readpos: file=%s\n", file);

    if ((fp = fopen(file, "r")) == null)
    {
        fprintf(stderr, "reference position file open error : %s\n", file);
        return;
    }
    while (np < 2048 && fgets(buff, sizeof(sbyte), fp))
    {
        if (buff[0] == '%' || buff[0] == '#')
            continue;
        if (sscanf(buff, "%lf %lf %lf %s", readpos_poss[np, 0], readpos_poss[np, 1], readpos_poss[np, 2], str) < 4)
            continue;
        readpos_stas[np] = str.Substring(0, 15);
        readpos_stas[np++, 15] = (sbyte)'\0';
    }
    fclose(fp);
    len = (int)rcv.Length;
    for (i = 0; i < np; i++)
    {
        if (string.Compare(readpos_stas[i], 0, rcv, 0, len))
            continue;
        for (j = 0; j < 3; j++)
        {
            pos[j] = readpos_poss[i, j];
        }
        pos[0] *= DefineConstants.PI / 180.0;
        pos[1] *= DefineConstants.PI / 180.0;
        return;
    }
    pos[0] = pos[1] = pos[2] = 0.0;
}
/* read blq record -----------------------------------------------------------*/
internal static int readblqrecord(FILE fp, double[] odisp)
{
    double[] v = new double[11];
    string buff = new string(new char[256]);
    int i;
    int n = 0;

    while (fgets(buff, sizeof(sbyte), fp))
    {
        if (!string.Compare(buff, 0, "$$", 0, 2))
            continue;
        if (sscanf(buff, "%lf %lf %lf %lf %lf %lf %lf %lf %lf %lf %lf", v, v + 1, v + 2, v + 3, v + 4, v + 5, v + 6, v + 7, v + 8, v + 9, v + 10) < 11)
            continue;
        for (i = 0; i < 11; i++)
        {
            odisp[n + i * 6] = v[i];
        }
        if (++n == 6)
        {
            return 1;
        }
    }
    return 0;
}
/* read blq ocean tide loading parameters --------------------------------------
* read blq ocean tide loading parameters
* args   : char   *file       I   BLQ ocean tide loading parameter file
*          char   *sta        I   station name
*          double *odisp      O   ocean tide loading parameters
* return : status (1:ok,0:file open error)
*-----------------------------------------------------------------------------*/
public static int readblq(string file, string sta, ref double odisp)
{
    FILE fp;
    string buff = new string(new char[256]);
    string staname = "";
    string name = new string(new char[32]);
    //C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on this variable, so pointers on this variable are left unchanged:
    sbyte* p;

    /* station name to upper case */
    sscanf(sta, "%16s", staname);
    for (p = staname; (*p = (sbyte)char.ToUpper((int)(*p))); p++)
    {
        ;
    }

    if ((fp = fopen(file, "r")) == null)
    {
        GlobalMembersRtkcmn.trace(2, "blq file open error: file=%s\n", file);
        return 0;
    }
    while (fgets(buff, sizeof(sbyte), fp))
    {
        if (!string.Compare(buff, 0, "$$", 0, 2) || buff.Length < 2)
            continue;

        if (sscanf(buff.Substring(2), "%16s", name) < 1)
            continue;
        for (p = name; (*p = (sbyte)char.ToUpper((int)(*p))); p++)
        {
            ;
        }
        if (string.Compare(name, staname))
            continue;

        /* read blq record */
        if (GlobalMembersRtkcmn.readblqrecord(fp, odisp) != 0)
        {
            fclose(fp);
            return 1;
        }
    }
    fclose(fp);
    GlobalMembersRtkcmn.trace(2, "no otl parameters: sta=%s file=%s\n", sta, file);
    return 0;
}
/* read earth rotation parameters ----------------------------------------------
* read earth rotation parameters
* args   : char   *file       I   IGS ERP file (IGS ERP ver.2)
*          erp_t  *erp        O   earth rotation parameters
* return : status (1:ok,0:file open error)
*-----------------------------------------------------------------------------*/
public static int readerp(string file, erp_t erp)
{
    FILE fp;
    erpd_t erp_data;
    double[] v = { 0, null, null, null, null, null, null, null, null, null, null, null, null, null };
    string buff = new string(new char[256]);

    GlobalMembersRtkcmn.trace(3, "readerp: file=%s\n", file);

    if ((fp = fopen(file, "r")) == null)
    {
        GlobalMembersRtkcmn.trace(2, "erp file open error: file=%s\n", file);
        return 0;
    }
    while (fgets(buff, sizeof(sbyte), fp))
    {
        if (sscanf(buff, "%lf %lf %lf %lf %lf %lf %lf %lf %lf %lf %lf %lf %lf %lf", v, v + 1, v + 2, v + 3, v + 4, v + 5, v + 6, v + 7, v + 8, v + 9, v + 10, v + 11, v + 12, v + 13) < 5)
        {
            continue;
        }
        if (erp.n >= erp.nmax)
        {
            erp.nmax = erp.nmax <= 0 ? 128 : erp.nmax * 2;
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'realloc' has no equivalent in C#:
            erp_data = (erpd_t)realloc(erp.data, sizeof(erpd_t) * erp.nmax);
            if (erp_data == null)
            {
                //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
                free(erp.data);
                erp.data = null;
                erp.n = erp.nmax = 0;
                fclose(fp);
                return 0;
            }
            erp.data = erp_data;
        }
        erp.data[erp.n].mjd = v[0];
        erp.data[erp.n].xp = v[1] * 1E-6 * DefineConstants.PI / 180.0 / 3600.0;
        erp.data[erp.n].yp = v[2] * 1E-6 * DefineConstants.PI / 180.0 / 3600.0;
        erp.data[erp.n].ut1_utc = v[3] * 1E-7;
        erp.data[erp.n].lod = v[4] * 1E-7;
        erp.data[erp.n].xpr = v[12] * 1E-6 * DefineConstants.PI / 180.0 / 3600.0;
        erp.data[erp.n++].ypr = v[13] * 1E-6 * DefineConstants.PI / 180.0 / 3600.0;
    }
    fclose(fp);
    return 1;
}
/* get earth rotation parameter values -----------------------------------------
* get earth rotation parameter values
* args   : erp_t  *erp        I   earth rotation parameters
*          gtime_t time       I   time (gpst)
*          double *erpv       O   erp values {xp,yp,ut1_utc,lod} (rad,rad,s,s/d)
* return : status (1:ok,0:error)
*-----------------------------------------------------------------------------*/
public static int geterp(erp_t erp, gtime_t time, double[] erpv)
{
    double[] ep = { 2000, 1, 1, 12, 0, 0 };
    double mjd;
    double day;
    double a;
    int i = 0;
    int j;
    int k;

    GlobalMembersRtkcmn.trace(4, "geterp:\n");

    if (erp.n <= 0)
    {
        return 0;
    }

    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
    //ORIGINAL LINE: mjd=51544.5+(timediff(gpst2utc(time),epoch2time(ep)))/86400.0;
    mjd = 51544.5 + (GlobalMembersRtkcmn.timediff(GlobalMembersRtkcmn.gpst2utc(new gtime_t(time)), GlobalMembersRtkcmn.epoch2time(ep))) / 86400.0;

    if (mjd <= erp.data[0].mjd)
    {
        day = mjd - erp.data[0].mjd;
        erpv[0] = erp.data[0].xp + erp.data[0].xpr * day;
        erpv[1] = erp.data[0].yp + erp.data[0].ypr * day;
        erpv[2] = erp.data[0].ut1_utc - erp.data[0].lod * day;
        erpv[3] = erp.data[0].lod;
        return 1;
    }
    if (mjd >= erp.data[erp.n - 1].mjd)
    {
        day = mjd - erp.data[erp.n - 1].mjd;
        erpv[0] = erp.data[erp.n - 1].xp + erp.data[erp.n - 1].xpr * day;
        erpv[1] = erp.data[erp.n - 1].yp + erp.data[erp.n - 1].ypr * day;
        erpv[2] = erp.data[erp.n - 1].ut1_utc - erp.data[erp.n - 1].lod * day;
        erpv[3] = erp.data[erp.n - 1].lod;
        return 1;
    }
    for (j = 0, k = erp.n - 1; j < k - 1;)
    {
        i = (j + k) / 2;
        if (mjd < erp.data[i].mjd)
        {
            k = i;
        }
        else
        {
            j = i;
        }
    }
    if (erp.data[j].mjd == erp.data[j + 1].mjd)
    {
        a = 0.5;
    }
    else
    {
        a = (mjd - erp.data[j].mjd) / (erp.data[j + 1].mjd - erp.data[j].mjd);
    }
    erpv[0] = (1.0 - a) * erp.data[j].xp + a * erp.data[j + 1].xp;
    erpv[1] = (1.0 - a) * erp.data[j].yp + a * erp.data[j + 1].yp;
    erpv[2] = (1.0 - a) * erp.data[j].ut1_utc + a * erp.data[j + 1].ut1_utc;
    erpv[3] = (1.0 - a) * erp.data[j].lod + a * erp.data[j + 1].lod;
    return 1;
}
/* compare ephemeris ---------------------------------------------------------*/
internal static int cmpeph(object p1, object p2)
{
    eph_t q1 = (eph_t)p1;
    eph_t q2 = (eph_t)p2;
    return q1.ttr.time != q2.ttr.time != null ? (int)(q1.ttr.time - q2.ttr.time) : (q1.toe.time != q2.toe.time != null ? (int)(q1.toe.time - q2.toe.time) : q1.sat - q2.sat);
}
/* sort and unique ephemeris -------------------------------------------------*/
internal static void uniqeph(nav_t nav)
{
    eph_t nav_eph;
    int i;
    int j;

    GlobalMembersRtkcmn.trace(3, "uniqeph: n=%d\n", nav.n);

    if (nav.n <= 0)
        return;

    qsort(nav.eph, nav.n, sizeof(eph_t), GlobalMembersRtkcmn.cmpeph);

    for (i = 1, j = 0; i < nav.n; i++)
    {
        if (nav.eph[i].sat != nav.eph[j].sat || nav.eph[i].iode != nav.eph[j].iode)
        {
            nav.eph[++j] = nav.eph[i];
        }
    }
    nav.n = j + 1;

    //C++ TO C# CONVERTER TODO TASK: The memory management function 'realloc' has no equivalent in C#:
    if ((nav_eph = (eph_t)realloc(nav.eph, sizeof(eph_t) * nav.n)) == null)
    {
        GlobalMembersRtkcmn.trace(1, "uniqeph malloc error n=%d\n", nav.n);
        //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
        free(nav.eph);
        nav.eph = null;
        nav.n = nav.nmax = 0;
        return;
    }
    nav.eph = nav_eph;
    nav.nmax = nav.n;

    GlobalMembersRtkcmn.trace(4, "uniqeph: n=%d\n", nav.n);
}
/* compare glonass ephemeris -------------------------------------------------*/
internal static int cmpgeph(object p1, object p2)
{
    geph_t q1 = (geph_t)p1;
    geph_t q2 = (geph_t)p2;
    return q1.tof.time != q2.tof.time != null ? (int)(q1.tof.time - q2.tof.time) : (q1.toe.time != q2.toe.time != null ? (int)(q1.toe.time - q2.toe.time) : q1.sat - q2.sat);
}
/* sort and unique glonass ephemeris -----------------------------------------*/
internal static void uniqgeph(nav_t nav)
{
    geph_t nav_geph;
    int i;
    int j;

    GlobalMembersRtkcmn.trace(3, "uniqgeph: ng=%d\n", nav.ng);

    if (nav.ng <= 0)
        return;

    qsort(nav.geph, nav.ng, sizeof(geph_t), GlobalMembersRtkcmn.cmpgeph);

    for (i = j = 0; i < nav.ng; i++)
    {
        if (nav.geph[i].sat != nav.geph[j].sat || nav.geph[i].toe.time != nav.geph[j].toe.time || nav.geph[i].svh != nav.geph[j].svh)
        {
            nav.geph[++j] = nav.geph[i];
        }
    }
    nav.ng = j + 1;

    //C++ TO C# CONVERTER TODO TASK: The memory management function 'realloc' has no equivalent in C#:
    if ((nav_geph = (geph_t)realloc(nav.geph, sizeof(geph_t) * nav.ng)) == null)
    {
        GlobalMembersRtkcmn.trace(1, "uniqgeph malloc error ng=%d\n", nav.ng);
        //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
        free(nav.geph);
        nav.geph = null;
        nav.ng = nav.ngmax = 0;
        return;
    }
    nav.geph = nav_geph;
    nav.ngmax = nav.ng;

    GlobalMembersRtkcmn.trace(4, "uniqgeph: ng=%d\n", nav.ng);
}
/* compare sbas ephemeris ----------------------------------------------------*/
internal static int cmpseph(object p1, object p2)
{
    seph_t q1 = (seph_t)p1;
    seph_t q2 = (seph_t)p2;
    return q1.tof.time != q2.tof.time != null ? (int)(q1.tof.time - q2.tof.time) : (q1.t0.time != q2.t0.time != null ? (int)(q1.t0.time - q2.t0.time) : q1.sat - q2.sat);
}
/* sort and unique sbas ephemeris --------------------------------------------*/
internal static void uniqseph(nav_t nav)
{
    seph_t nav_seph;
    int i;
    int j;

    GlobalMembersRtkcmn.trace(3, "uniqseph: ns=%d\n", nav.ns);

    if (nav.ns <= 0)
        return;

    qsort(nav.seph, nav.ns, sizeof(seph_t), GlobalMembersRtkcmn.cmpseph);

    for (i = j = 0; i < nav.ns; i++)
    {
        if (nav.seph[i].sat != nav.seph[j].sat || nav.seph[i].t0.time != nav.seph[j].t0.time)
        {
            nav.seph[++j] = nav.seph[i];
        }
    }
    nav.ns = j + 1;

    //C++ TO C# CONVERTER TODO TASK: The memory management function 'realloc' has no equivalent in C#:
    if ((nav_seph = (seph_t)realloc(nav.seph, sizeof(seph_t) * nav.ns)) == null)
    {
        GlobalMembersRtkcmn.trace(1, "uniqseph malloc error ns=%d\n", nav.ns);
        //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
        free(nav.seph);
        nav.seph = null;
        nav.ns = nav.nsmax = 0;
        return;
    }
    nav.seph = nav_seph;
    nav.nsmax = nav.ns;

    GlobalMembersRtkcmn.trace(4, "uniqseph: ns=%d\n", nav.ns);
}
/* unique ephemerides ----------------------------------------------------------
* unique ephemerides in navigation data and update carrier wave length
* args   : nav_t *nav    IO     navigation data
* return : number of epochs
*-----------------------------------------------------------------------------*/
public static void uniqnav(nav_t nav)
{
    int i;
    int j;

    GlobalMembersRtkcmn.trace(3, "uniqnav: neph=%d ngeph=%d nseph=%d\n", nav.n, nav.ng, nav.ns);

    /* unique ephemeris */
    GlobalMembersRtkcmn.uniqeph(nav);
    GlobalMembersRtkcmn.uniqgeph(nav);
    GlobalMembersRtkcmn.uniqseph(nav);

    /* update carrier wave length */
    for (i = 0; i < DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1; i++)
    {
        for (j = 0; j < DefineConstants.NFREQ; j++)
        {
            nav.lam[i, j] = GlobalMembersRtkcmn.satwavelen(i + 1, j, nav);
        }
    }
}
/* compare observation data -------------------------------------------------*/
internal static int cmpobs(object p1, object p2)
{
    obsd_t q1 = (obsd_t)p1;
    obsd_t q2 = (obsd_t)p2;
    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
    //ORIGINAL LINE: double tt=timediff(q1->time,q2->time);
    double tt = GlobalMembersRtkcmn.timediff(new gtime_t(q1.time), new gtime_t(q2.time));
    if (Math.Abs(tt) > DefineConstants.DTTOL)
    {
        return tt < 0 ? -1 : 1;
    }
    if (q1.rcv != q2.rcv)
    {
        return (int)q1.rcv - (int)q2.rcv;
    }
    return (int)q1.sat - (int)q2.sat;
}
/* sort and unique observation data --------------------------------------------
* sort and unique observation data by time, rcv, sat
* args   : obs_t *obs    IO     observation data
* return : number of epochs
*-----------------------------------------------------------------------------*/
public static int sortobs(obs_t obs)
{
    int i;
    int j;
    int n;

    GlobalMembersRtkcmn.trace(3, "sortobs: nobs=%d\n", obs.n);

    if (obs.n <= 0)
    {
        return 0;
    }

    qsort(obs.data, obs.n, sizeof(obsd_t), GlobalMembersRtkcmn.cmpobs);

    /* delete duplicated data */
    for (i = j = 0; i < obs.n; i++)
    {
        if (obs.data[i].sat != obs.data[j].sat || obs.data[i].rcv != obs.data[j].rcv || GlobalMembersRtkcmn.timediff(obs.data[i].time, obs.data[j].time) != 0.0)
        {
            obs.data[++j] = obs.data[i];
        }
    }
    obs.n = j + 1;

    for (i = n = 0; i < obs.n; i = j, n++)
    {
        for (j = i + 1; j < obs.n; j++)
        {
            if (GlobalMembersRtkcmn.timediff(obs.data[j].time, obs.data[i].time) > DefineConstants.DTTOL)
                break;
        }
    }
    return n;
}
/* screen by time --------------------------------------------------------------
* screening by time start, time end, and time interval
* args   : gtime_t time  I      time
*          gtime_t ts    I      time start (ts.time==0:no screening by ts)
*          gtime_t te    I      time end   (te.time==0:no screening by te)
*          double  tint  I      time interval (s) (0.0:no screen by tint)
* return : 1:on condition, 0:not on condition
*-----------------------------------------------------------------------------*/
public static int screent(gtime_t time, gtime_t ts, gtime_t te, double tint)
{
    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
    //ORIGINAL LINE: return (tint<=0.0||fmod(time2gpst(time,null)+DefineConstants.DTTOL,tint)<=DefineConstants.DTTOL *2.0)&& (ts.time==0||timediff(time,ts)>=-DefineConstants.DTTOL)&& (te.time==0||timediff(time,te)< DefineConstants.DTTOL);
    return (tint <= 0.0 || Math.IEEERemainder(GlobalMembersRtkcmn.time2gpst(new gtime_t(time), null) + DefineConstants.DTTOL, tint) <= DefineConstants.DTTOL * 2.0) && (ts.time == 0 || GlobalMembersRtkcmn.timediff(new gtime_t(time), new gtime_t(ts)) >= -DefineConstants.DTTOL) && (te.time == 0 || GlobalMembersRtkcmn.timediff(new gtime_t(time), new gtime_t(te)) < DefineConstants.DTTOL);
}
/* read/save navigation data ---------------------------------------------------
* save or load navigation data
* args   : char    file  I      file path
*          nav_t   nav   O/I    navigation data
* return : status (1:ok,0:no file)
*-----------------------------------------------------------------------------*/
public static int readnav(string file, nav_t nav)
{
    FILE fp;
    eph_t eph0 = new eph_t();
    string buff = new string(new char[4096]);
    string p;
    int i;
    int sat;

    GlobalMembersRtkcmn.trace(3, "loadnav: file=%s\n", file);

    if ((fp = fopen(file, "r")) == null)
    {
        return 0;
    }

    while (fgets(buff, sizeof(sbyte), fp))
    {
        if (!string.Compare(buff, 0, "IONUTC", 0, 6))
        {
            for (i = 0; i < 8; i++)
            {
                nav.ion_gps[i] = 0.0;
            }
            for (i = 0; i < 4; i++)
            {
                nav.utc_gps[i] = 0.0;
            }
            nav.leaps = 0;
            sscanf(buff, "IONUTC,%lf,%lf,%lf,%lf,%lf,%lf,%lf,%lf,%lf,%lf,%lf,%lf,%d", nav.ion_gps[0], nav.ion_gps[1], nav.ion_gps[2], nav.ion_gps[3], nav.ion_gps[4], nav.ion_gps[5], nav.ion_gps[6], nav.ion_gps[7], nav.utc_gps[0], nav.utc_gps[1], nav.utc_gps[2], nav.utc_gps[3], nav.leaps);
            continue;
        }
        if ((p = StringFunctions.StrChr(buff, ',')) != 0)
        {
            p = (sbyte)'\0';
        }
        else
            continue;
        if ((sat = GlobalMembersRtkcmn.satid2no(buff)) == 0)
            continue;
        nav.eph[sat - 1] = eph0;
        nav.eph[sat - 1].sat = sat;
        sscanf(p.Substring(1), "%d,%d,%d,%d,%ld,%ld,%ld,%lf,%lf,%lf,%lf,%lf,%lf,%lf,%lf," + "%lf,%lf,%lf,%lf,%lf,%lf,%lf,%lf,%lf,%lf,%lf,%lf,%lf,%d,%d", nav.eph[sat - 1].iode, nav.eph[sat - 1].iodc, nav.eph[sat - 1].sva, nav.eph[sat - 1].svh, nav.eph[sat - 1].toe.time, nav.eph[sat - 1].toc.time, nav.eph[sat - 1].ttr.time, nav.eph[sat - 1].A, nav.eph[sat - 1].e, nav.eph[sat - 1].i0, nav.eph[sat - 1].OMG0, nav.eph[sat - 1].omg, nav.eph[sat - 1].M0, nav.eph[sat - 1].deln, nav.eph[sat - 1].OMGd, nav.eph[sat - 1].idot, nav.eph[sat - 1].crc, nav.eph[sat - 1].crs, nav.eph[sat - 1].cuc, nav.eph[sat - 1].cus, nav.eph[sat - 1].cic, nav.eph[sat - 1].cis, nav.eph[sat - 1].toes, nav.eph[sat - 1].fit, nav.eph[sat - 1].f0, nav.eph[sat - 1].f1, nav.eph[sat - 1].f2, nav.eph[sat - 1].tgd[0], nav.eph[sat - 1].code, nav.eph[sat - 1].flag);
    }
    fclose(fp);
    return 1;
}
public static int savenav(string file, nav_t nav)
{
    FILE fp;
    int i;
    string id = new string(new char[32]);

    GlobalMembersRtkcmn.trace(3, "savenav: file=%s\n", file);

    if ((fp = fopen(file, "w")) == null)
    {
        return 0;
    }

    for (i = 0; i < DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1; i++)
    {
        if (nav.eph[i].ttr.time == 0)
            continue;
        GlobalMembersRtkcmn.satno2id(nav.eph[i].sat, ref id);
        fprintf(fp, "%s,%d,%d,%d,%d,%d,%d,%d,%.14E,%.14E,%.14E,%.14E,%.14E,%.14E," + "%.14E,%.14E,%.14E,%.14E,%.14E,%.14E,%.14E,%.14E,%.14E,%.14E," + "%.14E,%.14E,%.14E,%.14E,%.14E,%d,%d\n", id, nav.eph[i].iode, nav.eph[i].iodc, nav.eph[i].sva, nav.eph[i].svh, (int)nav.eph[i].toe.time, (int)nav.eph[i].toc.time, (int)nav.eph[i].ttr.time, nav.eph[i].A, nav.eph[i].e, nav.eph[i].i0, nav.eph[i].OMG0, nav.eph[i].omg, nav.eph[i].M0, nav.eph[i].deln, nav.eph[i].OMGd, nav.eph[i].idot, nav.eph[i].crc, nav.eph[i].crs, nav.eph[i].cuc, nav.eph[i].cus, nav.eph[i].cic, nav.eph[i].cis, nav.eph[i].toes, nav.eph[i].fit, nav.eph[i].f0, nav.eph[i].f1, nav.eph[i].f2, nav.eph[i].tgd[0], nav.eph[i].code, nav.eph[i].flag);
    }
    fprintf(fp, "IONUTC,%.14E,%.14E,%.14E,%.14E,%.14E,%.14E,%.14E,%.14E,%.14E," + "%.14E,%.14E,%.14E,%d", nav.ion_gps[0], nav.ion_gps[1], nav.ion_gps[2], nav.ion_gps[3], nav.ion_gps[4], nav.ion_gps[5], nav.ion_gps[6], nav.ion_gps[7], nav.utc_gps[0], nav.utc_gps[1], nav.utc_gps[2], nav.utc_gps[3], nav.leaps);

    fclose(fp);
    return 1;
}
/* free observation data -------------------------------------------------------
* free memory for observation data
* args   : obs_t *obs    IO     observation data
* return : none
*-----------------------------------------------------------------------------*/
public static void freeobs(obs_t obs)
{
    //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
    free(obs.data);
    obs.data = null;
    obs.n = obs.nmax = 0;
}
/* free navigation data ---------------------------------------------------------
* free memory for navigation data
* args   : nav_t *nav    IO     navigation data
*          int   opt     I      option (or of followings)
*                               (0x01: gps/qzs ephmeris, 0x02: glonass ephemeris,
*                                0x04: sbas ephemeris,   0x08: precise ephemeris,
*                                0x10: precise clock     0x20: almanac,
*                                0x40: tec data)
* return : none
*-----------------------------------------------------------------------------*/
public static void freenav(nav_t nav, int opt)
{
    if ((opt & 0x01) != 0)
    {
        //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
        free(nav.eph);
        nav.eph = null;
        nav.n = nav.nmax = 0;
    }
    if ((opt & 0x02) != 0)
    {
        //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
        free(nav.geph);
        nav.geph = null;
        nav.ng = nav.ngmax = 0;
    }
    if ((opt & 0x04) != 0)
    {
        //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
        free(nav.seph);
        nav.seph = null;
        nav.ns = nav.nsmax = 0;
    }
    if ((opt & 0x08) != 0)
    {
        //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
        free(nav.peph);
        nav.peph = null;
        nav.ne = nav.nemax = 0;
    }
    if ((opt & 0x10) != 0)
    {
        //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
        free(nav.pclk);
        nav.pclk = null;
        nav.nc = nav.ncmax = 0;
    }
    if ((opt & 0x20) != 0)
    {
        //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
        free(nav.alm);
        nav.alm = null;
        nav.na = nav.namax = 0;
    }
    if ((opt & 0x40) != 0)
    {
        //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
        free(nav.tec);
        nav.tec = null;
        nav.nt = nav.ntmax = 0;
    }
}
/* debug trace functions -----------------------------------------------------*/
#if TRACE

internal static FILE fp_trace = null; // file pointer of trace
internal static string file_trace = new string(new char[1024]); // trace file
internal static int level_trace = 0; // level of trace
internal static uint tick_trace = 0; // tick time at traceopen (ms)
internal static gtime_t time_trace = new gtime_t(); // time at traceopen
#if lock_t_ConditionalDefinition1
	internal static CRITICAL_SECTION lock_trace = new CRITICAL_SECTION();
#elif lock_t_ConditionalDefinition2
	internal static pthread_mutex_t lock_trace = new pthread_mutex_t();
#else
internal static lock_t lock_trace = new lock_t();
#endif

internal static void traceswap()
{
    gtime_t time = GlobalMembersRtkcmn.utc2gpst(GlobalMembersRtkcmn.timeget());
    string path = new string(new char[1024]);

#if lock_ConditionalDefinition1
		EnterCriticalSection(lock_trace);
#elif lock_ConditionalDefinition2
		pthread_mutex_lock(lock_trace);
#else
    lock (lock_trace) ;
#endif

    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
    //ORIGINAL LINE: if ((int)(time2gpst(time,null)/DefineConstants.INT_SWAP_TRAC)== (int)(time2gpst(time_trace,null)/DefineConstants.INT_SWAP_TRAC))
    if ((int)(GlobalMembersRtkcmn.time2gpst(new gtime_t(time), null) / DefineConstants.INT_SWAP_TRAC) == (int)(GlobalMembersRtkcmn.time2gpst(new gtime_t(time_trace), null) / DefineConstants.INT_SWAP_TRAC))
    {
#if unlock_ConditionalDefinition1
			LeaveCriticalSection(lock_trace);
#elif unlock_ConditionalDefinition2
			pthread_mutex_unlock(lock_trace);
#else
        unlock(lock_trace);
#endif
        return;
    }
    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
    //ORIGINAL LINE: time_trace=time;
    time_trace.CopyFrom(time);

    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
    //ORIGINAL LINE: if (!reppath(file_trace,path,time,"",""))
    if (GlobalMembersRtkcmn.reppath(file_trace, ref path, new gtime_t(time), "", "") == 0)
    {
#if unlock_ConditionalDefinition1
			LeaveCriticalSection(lock_trace);
#elif unlock_ConditionalDefinition2
			pthread_mutex_unlock(lock_trace);
#else
        unlock(lock_trace);
#endif
        return;
    }
    if (fp_trace != null)
    {
        fclose(fp_trace);
    }

    if ((fp_trace = fopen(path, "w")) == null)
    {
        fp_trace = stderr;
    }
#if unlock_ConditionalDefinition1
		LeaveCriticalSection(lock_trace);
#elif unlock_ConditionalDefinition2
		pthread_mutex_unlock(lock_trace);
#else
    unlock(lock_trace);
#endif
}
public static void traceopen(string file)
{
    gtime_t time = GlobalMembersRtkcmn.utc2gpst(GlobalMembersRtkcmn.timeget());
    string path = new string(new char[1024]);

    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
    //ORIGINAL LINE: reppath(file,path,time,"","");
    GlobalMembersRtkcmn.reppath(file, ref path, new gtime_t(time), "", "");
    if (!*path || (fp_trace = fopen(path, "w")) == null)
    {
        fp_trace = stderr;
    }
    file_trace = file;
    tick_trace = GlobalMembersRtkcmn.tickget();
    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
    //ORIGINAL LINE: time_trace=time;
    time_trace.CopyFrom(time);
#if initlock_ConditionalDefinition1
		InitializeCriticalSection(lock_trace);
#elif initlock_ConditionalDefinition2
		pthread_mutex_init(lock_trace, null);
#else
    initlock(lock_trace);
#endif
}
public static void traceclose()
{
    if (fp_trace != null && fp_trace != stderr)
    {
        fclose(fp_trace);
    }
    fp_trace = null;
    file_trace[0] = '\0';
}
public static void tracelevel(int level)
{
    level_trace = level;
}
public static void trace(int level, string format, params object[] LegacyParamArray)
{
    //	va_list ap;

    /* print error message to stderr */
    if (level <= 1)
    {
        int ParamCount = -1;
        //		va_start(ap,format);
        vfprintf(stderr, format, ap);
        //		va_end(ap);
    }
    if (fp_trace == null || level > level_trace)
        return;
    GlobalMembersRtkcmn.traceswap();
    fprintf(fp_trace, "%d ", level);
    ParamCount = -1;
    //	va_start(ap,format);
    vfprintf(fp_trace, format, ap);
    //	va_end(ap);
    fflush(fp_trace);
}
public static void tracet(int level, string format, params object[] LegacyParamArray)
{
    //	va_list ap;

    if (fp_trace == null || level > level_trace)
        return;
    GlobalMembersRtkcmn.traceswap();
    fprintf(fp_trace, "%d %9.3f: ", level, (GlobalMembersRtkcmn.tickget() - tick_trace) / 1000.0);
    int ParamCount = -1;
    //	va_start(ap,format);
    vfprintf(fp_trace, format, ap);
    //	va_end(ap);
    fflush(fp_trace);
}
public static void tracemat(int level, double A, int n, int m, int p, int q)
{
    if (fp_trace == null || level > level_trace)
        return;
    GlobalMembersRtkcmn.matfprint(A, n, m, p, q, fp_trace);
    fflush(fp_trace);
}
public static void traceobs(int level, obsd_t[] obs, int n)
{
    string str = new string(new char[64]);
    string id = new string(new char[16]);
    int i;

    if (fp_trace == null || level > level_trace)
        return;
    for (i = 0; i < n; i++)
    {
        GlobalMembersRtkcmn.time2str(obs[i].time, ref str, 3);
        GlobalMembersRtkcmn.satno2id(obs[i].sat, ref id);
        fprintf(fp_trace, " (%2d) %s %-3s rcv%d %13.3f %13.3f %13.3f %13.3f %d %d %d %d %3.1f %3.1f\n", i + 1, str, id, obs[i].rcv, obs[i].L[0], obs[i].L[1], obs[i].P[0], obs[i].P[1], obs[i].LLI[0], obs[i].LLI[1], obs[i].code[0], obs[i].code[1], obs[i].SNR[0] * 0.25, obs[i].SNR[1] * 0.25);
    }
    fflush(fp_trace);
}
public static void tracenav(int level, nav_t nav)
{
    string s1 = new string(new char[64]);
    string s2 = new string(new char[64]);
    string id = new string(new char[16]);
    int i;

    if (fp_trace == null || level > level_trace)
        return;
    for (i = 0; i < nav.n; i++)
    {
        GlobalMembersRtkcmn.time2str(nav.eph[i].toe, ref s1, 0);
        GlobalMembersRtkcmn.time2str(nav.eph[i].ttr, ref s2, 0);
        GlobalMembersRtkcmn.satno2id(nav.eph[i].sat, ref id);
        fprintf(fp_trace, "(%3d) %-3s : %s %s %3d %3d %02x\n", i + 1, id, s1, s2, nav.eph[i].iode, nav.eph[i].iodc, nav.eph[i].svh);
    }
    fprintf(fp_trace, "(ion) %9.4e %9.4e %9.4e %9.4e\n", nav.ion_gps[0], nav.ion_gps[1], nav.ion_gps[2], nav.ion_gps[3]);
    fprintf(fp_trace, "(ion) %9.4e %9.4e %9.4e %9.4e\n", nav.ion_gps[4], nav.ion_gps[5], nav.ion_gps[6], nav.ion_gps[7]);
    fprintf(fp_trace, "(ion) %9.4e %9.4e %9.4e %9.4e\n", nav.ion_gal[0], nav.ion_gal[1], nav.ion_gal[2], nav.ion_gal[3]);
}
public static void tracegnav(int level, nav_t nav)
{
    string s1 = new string(new char[64]);
    string s2 = new string(new char[64]);
    string id = new string(new char[16]);
    int i;

    if (fp_trace == null || level > level_trace)
        return;
    for (i = 0; i < nav.ng; i++)
    {
        GlobalMembersRtkcmn.time2str(nav.geph[i].toe, ref s1, 0);
        GlobalMembersRtkcmn.time2str(nav.geph[i].tof, ref s2, 0);
        GlobalMembersRtkcmn.satno2id(nav.geph[i].sat, ref id);
        fprintf(fp_trace, "(%3d) %-3s : %s %s %2d %2d %8.3f\n", i + 1, id, s1, s2, nav.geph[i].frq, nav.geph[i].svh, nav.geph[i].taun * 1E6);
    }
}
public static void tracehnav(int level, nav_t nav)
{
    string s1 = new string(new char[64]);
    string s2 = new string(new char[64]);
    string id = new string(new char[16]);
    int i;

    if (fp_trace == null || level > level_trace)
        return;
    for (i = 0; i < nav.ns; i++)
    {
        GlobalMembersRtkcmn.time2str(nav.seph[i].t0, ref s1, 0);
        GlobalMembersRtkcmn.time2str(nav.seph[i].tof, ref s2, 0);
        GlobalMembersRtkcmn.satno2id(nav.seph[i].sat, ref id);
        fprintf(fp_trace, "(%3d) %-3s : %s %s %2d %2d\n", i + 1, id, s1, s2, nav.seph[i].svh, nav.seph[i].sva);
    }
}
public static void tracepeph(int level, nav_t nav)
{
    string s = new string(new char[64]);
    string id = new string(new char[16]);
    int i;
    int j;

    if (fp_trace == null || level > level_trace)
        return;

    for (i = 0; i < nav.ne; i++)
    {
        GlobalMembersRtkcmn.time2str(nav.peph[i].time, ref s, 0);
        for (j = 0; j < DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1; j++)
        {
            GlobalMembersRtkcmn.satno2id(j + 1, ref id);
            fprintf(fp_trace, "%-3s %d %-3s %13.3f %13.3f %13.3f %13.3f %6.3f %6.3f %6.3f %6.3f\n", s, nav.peph[i].index, id, nav.peph[i].pos[j][0], nav.peph[i].pos[j][1], nav.peph[i].pos[j][2], nav.peph[i].pos[j][3] * 1E9, nav.peph[i].std[j][0], nav.peph[i].std[j][1], nav.peph[i].std[j][2], nav.peph[i].std[j][3] * 1E9);
        }
    }
}
public static void tracepclk(int level, nav_t nav)
{
    string s = new string(new char[64]);
    string id = new string(new char[16]);
    int i;
    int j;

    if (fp_trace == null || level > level_trace)
        return;

    for (i = 0; i < nav.nc; i++)
    {
        GlobalMembersRtkcmn.time2str(nav.pclk[i].time, ref s, 0);
        for (j = 0; j < DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1; j++)
        {
            GlobalMembersRtkcmn.satno2id(j + 1, ref id);
            fprintf(fp_trace, "%-3s %d %-3s %13.3f %6.3f\n", s, nav.pclk[i].index, id, nav.pclk[i].clk[j][0] * 1E9, nav.pclk[i].std[j][0] * 1E9);
        }
    }
}
//C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on the parameter 'p', so pointers on this parameter are left unchanged:
public static void traceb(int level, byte* p, int n)
{
    int i;
    if (fp_trace == null || level > level_trace)
        return;
    for (i = 0; i < n; i++)
    {
        fprintf(fp_trace, "%02X%s", *p++, i % 8 == 7 ? " " : "");
    }
    fprintf(fp_trace, "\n");
}
#else
	public static void traceopen(string file)
	{
	}
	public static void traceclose()
	{
	}
	public static void tracelevel(int level)
	{
	}
	public static void trace(int level, string format, params object[] LegacyParamArray)
	{
	}
	public static void tracet(int level, string format, params object[] LegacyParamArray)
	{
	}
	public static void tracemat(int level, double A, int n, int m, int p, int q)
	{
	}
	public static void traceobs(int level, obsd_t obs, int n)
	{
	}
	public static void tracenav(int level, nav_t nav)
	{
	}
	public static void tracegnav(int level, nav_t nav)
	{
	}
	public static void tracehnav(int level, nav_t nav)
	{
	}
	public static void tracepeph(int level, nav_t nav)
	{
	}
	public static void tracepclk(int level, nav_t nav)
	{
	}
	public static void traceb(int level, byte p, int n)
	{
	}

#endif

/* execute command -------------------------------------------------------------
* execute command line by operating system shell
* args   : char   *cmd      I   command line
* return : execution status (0:ok,0>:error)
*-----------------------------------------------------------------------------*/
public static int execcmd(string cmd)
{
#if WIN32
		PROCESS_INFORMATION info = new PROCESS_INFORMATION();
		STARTUPINFO si = new STARTUPINFO();
		uint stat;
		string cmds = new string(new char[1024]);

		GlobalMembersRtkcmn.trace(3, "execcmd: cmd=%s\n", cmd);

		si.cb = sizeof(STARTUPINFO);
		cmds = string.Format("cmd /c {0}", cmd);
		if (!CreateProcess(null, (string)cmds, null, null, 0, CREATE_NO_WINDOW, null, null, si, info))
		{
			return -1;
		}
		WaitForSingleObject(info.hProcess,INFINITE);
		if (!GetExitCodeProcess(info.hProcess, stat))
		{
			stat = -1;
		}
		CloseHandle(info.hProcess);
		CloseHandle(info.hThread);
		return (int)stat;
#else
    GlobalMembersRtkcmn.trace(3, "execcmd: cmd=%s\n", cmd);

    return system(cmd);
#endif
}
/* expand file path ------------------------------------------------------------
* expand file path with wild-card (*) in file
* args   : char   *path     I   file path to expand (captal insensitive)
*          char   *paths    O   expanded file paths
*          int    nmax      I   max number of expanded file paths
* return : number of expanded file paths
* notes  : the order of expanded files is alphabetical order
*-----------------------------------------------------------------------------*/
public static int expath(string path, string[] paths, int nmax)
{
    int i;
    int j;
    int n = 0;
    string tmp = new string(new char[1024]);
#if WIN32
		WIN32_FIND_DATA file = new WIN32_FIND_DATA();
		System.IntPtr h;
		string dir = "";
		string p;

		GlobalMembersRtkcmn.trace(3,"expath  : path=%s nmax=%d\n",path,nmax);

		if ((p = StringFunctions.StrRChr(path,'\\')) != 0)
		{
			dir = path.Substring(0, p - path.Substring(1));
			dir[p - path.Substring(1)] = '\0';
		}
		if ((h = FindFirstFile((string)path, file)) == INVALID_HANDLE_VALUE)
		{
			paths = StringFunctions.ChangeCharacter(paths, 0, path);
			return 1;
		}
		paths = StringFunctions.ChangeCharacter(paths, n++, string.Format("{0}{1}", dir, file.cFileName));
		while (FindNextFile(h, file) && n < nmax)
		{
			if (file.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY)
				continue;
			paths = StringFunctions.ChangeCharacter(paths, n++, string.Format("{0}{1}", dir, file.cFileName));
		}
		FindClose(h);
#else
    dirent d;
    DIR dp;
    string file = path;
    string dir = "";
    string s1 = new string(new char[1024]);
    string s2 = new string(new char[1024]);
    string p;
    string q;
    string r;

    GlobalMembersRtkcmn.trace(3, "expath  : path=%s nmax=%d\n", path, nmax);

    if ((p = StringFunctions.StrRChr(path, '/')) != 0 || (p = StringFunctions.StrRChr(path, '\\')) != 0)
    {
        file = p.Substring(1);
        dir = path.Substring(0, p - path.Substring(1));
        dir[p - path.Substring(1)] = '\0';
    }
    if ((dp = opendir(*dir != 0 ? dir : ".")) == null)
    {
        return 0;
    }
    while ((d = readdir(dp)) != null)
    {
        if (*(d.d_name) == '.')
            continue;
        s1 = string.Format("^{0}$", d.d_name);
        s2 = string.Format("^{0}$", file);
        for (p = s1; p != 0; p++)
        {
            p = (sbyte)char.ToLower((int)p);
        }
        for (p = s2; p != 0; p++)
        {
            p = (sbyte)char.ToLower((int)p);
        }

        for (p = s1, q = strtok_r(s2, "*", r); q != 0; q = strtok_r(null, "*", r))
        {
            if ((p = StringFunctions.StrStr(p, q)) != 0)
            {
                p += q.Length;
            }
            else
                break;
        }
        if (p != 0 && n < nmax)
        {
            paths = StringFunctions.ChangeCharacter(paths, n++, string.Format("{0}{1}", dir, d.d_name));
        }
    }
    closedir(dp);
#endif
    /* sort paths in alphabetical order */
    for (i = 0; i < n - 1; i++)
    {
        for (j = i + 1; j < n; j++)
        {
            if (string.Compare(paths[i], paths[j]) > 0)
            {
                tmp = paths[i];
                paths = StringFunctions.ChangeCharacter(paths, i, paths[j]);
                paths = StringFunctions.ChangeCharacter(paths, j, tmp);
            }
        }
    }
    for (i = 0; i < n; i++)
    {
        GlobalMembersRtkcmn.trace(3, "expath  : file=%s\n", paths[i]);
    }

    return n;
}
/* create directory ------------------------------------------------------------
* create directory if not exist
* args   : char   *path     I   file path to be saved
* return : none
* notes  : not recursive. only one level
*-----------------------------------------------------------------------------*/
public static void createdir(string path)
{
    string buff = new string(new char[1024]);
    string p;

    GlobalMembersRtkcmn.tracet(3, "createdir: path=%s\n", path);

    buff = path;
    if ((p = StringFunctions.StrRChr(buff, DefineConstants.FILEPATHSEP)) == 0)
        return;
    p = (sbyte)'\0';

#if WIN32
		CreateDirectory(buff,null);
#else
    mkdir(buff, 0x1FF);
#endif
}
/* replace string ------------------------------------------------------------*/
internal static int repstr(ref string str, string pat, string rep)
{
    int len = pat.Length;
    string buff = new string(new char[1024]);
    string p;
    string q;
    //C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on this variable, so pointers on this variable are left unchanged:
    sbyte* r;

    for (p = str, r = buff; p != 0; p = q.Substring(len))
    {
        if ((q = StringFunctions.StrStr(p, pat)) == 0)
            break;
        r = p.Substring(0, q - p);
        r += q - p;
        r += sprintf(r, "%s", rep);
    }
    if (p <= str)
    {
        return 0;
    }
    r = p;
    str = buff;
    return 1;
}
/* replace keywords in file path -----------------------------------------------
* replace keywords in file path with date, time, rover and base station id
* args   : char   *path     I   file path (see below)
*          char   *rpath    O   file path in which keywords replaced (see below)
*          gtime_t time     I   time (gpst)  (time.time==0: not replaced)
*          char   *rov      I   rover id string        ("": not replaced)
*          char   *base     I   base station id string ("": not replaced)
* return : status (1:keywords replaced, 0:no valid keyword in the path,
*                  -1:no valid time)
* notes  : the following keywords in path are replaced by date, time and name
*              %Y -> yyyy : year (4 digits) (1900-2099)
*              %y -> yy   : year (2 digits) (00-99)
*              %m -> mm   : month           (01-12)
*              %d -> dd   : day of month    (01-31)
*              %h -> hh   : hours           (00-23)
*              %M -> mm   : minutes         (00-59)
*              %S -> ss   : seconds         (00-59)
*              %n -> ddd  : day of year     (001-366)
*              %W -> wwww : gps week        (0001-9999)
*              %D -> d    : day of gps week (0-6)
*              %H -> h    : hour code       (a=0,b=1,c=2,...,x=23)
*              %ha-> hh   : 3 hours         (00,03,06,...,21)
*              %hb-> hh   : 6 hours         (00,06,12,18)
*              %hc-> hh   : 12 hours        (00,12)
*              %t -> mm   : 15 minutes      (00,15,30,45)
*              %r -> rrrr : rover id
*              %b -> bbbb : base station id
*-----------------------------------------------------------------------------*/
public static int reppath(string path, ref string rpath, gtime_t time, string rov, string @base)
{
    double[] ep = new double[6];
    double[] ep0 = { 2000, 1, 1, 0, 0, 0 };
    int week;
    int dow;
    int doy;
    int stat = 0;
    string rep = new string(new char[64]);

    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
    //ORIGINAL LINE: trace(3,"reppath : path =%s time=%s rov=%s base=%s\n",path,time_str(time,0), rov,base);
    GlobalMembersRtkcmn.trace(3, "reppath : path =%s time=%s rov=%s base=%s\n", path, GlobalMembersRtkcmn.time_str(new gtime_t(time), 0), rov, @base);

    rpath = path;

    if (!StringFunctions.StrStr(rpath, "%"))
    {
        return 0;
    }
    if (rov != 0)
    {
        stat |= GlobalMembersRtkcmn.repstr(ref rpath, "%r", rov);
    }
    if (@base != 0)
    {
        stat |= GlobalMembersRtkcmn.repstr(ref rpath, "%b", @base);
    }
    if (time.time != 0)
    {
        //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
        //ORIGINAL LINE: time2epoch(time,ep);
        GlobalMembersRtkcmn.time2epoch(new gtime_t(time), ep);
        ep0[0] = ep[0];
        //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
        //ORIGINAL LINE: dow=(int)floor(time2gpst(time,&week)/86400.0);
        dow = (int)Math.Floor(GlobalMembersRtkcmn.time2gpst(new gtime_t(time), ref week) / 86400.0);
        //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
        //ORIGINAL LINE: doy=(int)floor(timediff(time,epoch2time(ep0))/86400.0)+1;
        doy = (int)Math.Floor(GlobalMembersRtkcmn.timediff(new gtime_t(time), GlobalMembersRtkcmn.epoch2time(ep0)) / 86400.0) + 1;
        rep = string.Format("{0:D2}", ((int)ep[3] / 3) * 3);
        stat |= GlobalMembersRtkcmn.repstr(ref rpath, "%ha", rep);
        rep = string.Format("{0:D2}", ((int)ep[3] / 6) * 6);
        stat |= GlobalMembersRtkcmn.repstr(ref rpath, "%hb", rep);
        rep = string.Format("{0:D2}", ((int)ep[3] / 12) * 12);
        stat |= GlobalMembersRtkcmn.repstr(ref rpath, "%hc", rep);
        //C++ TO C# CONVERTER TODO TASK: Zero padding is not converted when combined with a minimum width specifier:
        //ORIGINAL LINE: sprintf(rep,"%04.0f",ep[0]);
        rep = string.Format("{0,4:f0}", ep[0]);
        stat |= GlobalMembersRtkcmn.repstr(ref rpath, "%Y", rep);
        //C++ TO C# CONVERTER TODO TASK: Zero padding is not converted when combined with a minimum width specifier:
        //ORIGINAL LINE: sprintf(rep,"%02.0f",fmod(ep[0],100.0));
        rep = string.Format("{0,2:f0}", Math.IEEERemainder(ep[0], 100.0));
        stat |= GlobalMembersRtkcmn.repstr(ref rpath, "%y", rep);
        //C++ TO C# CONVERTER TODO TASK: Zero padding is not converted when combined with a minimum width specifier:
        //ORIGINAL LINE: sprintf(rep,"%02.0f",ep[1]);
        rep = string.Format("{0,2:f0}", ep[1]);
        stat |= GlobalMembersRtkcmn.repstr(ref rpath, "%m", rep);
        //C++ TO C# CONVERTER TODO TASK: Zero padding is not converted when combined with a minimum width specifier:
        //ORIGINAL LINE: sprintf(rep,"%02.0f",ep[2]);
        rep = string.Format("{0,2:f0}", ep[2]);
        stat |= GlobalMembersRtkcmn.repstr(ref rpath, "%d", rep);
        //C++ TO C# CONVERTER TODO TASK: Zero padding is not converted when combined with a minimum width specifier:
        //ORIGINAL LINE: sprintf(rep,"%02.0f",ep[3]);
        rep = string.Format("{0,2:f0}", ep[3]);
        stat |= GlobalMembersRtkcmn.repstr(ref rpath, "%h", rep);
        //C++ TO C# CONVERTER TODO TASK: Zero padding is not converted when combined with a minimum width specifier:
        //ORIGINAL LINE: sprintf(rep,"%02.0f",ep[4]);
        rep = string.Format("{0,2:f0}", ep[4]);
        stat |= GlobalMembersRtkcmn.repstr(ref rpath, "%M", rep);
        //C++ TO C# CONVERTER TODO TASK: Zero padding is not converted when combined with a minimum width specifier:
        //ORIGINAL LINE: sprintf(rep,"%02.0f",floor(ep[5]));
        rep = string.Format("{0,2:f0}", Math.Floor(ep[5]));
        stat |= GlobalMembersRtkcmn.repstr(ref rpath, "%S", rep);
        rep = string.Format("{0:D3}", doy);
        stat |= GlobalMembersRtkcmn.repstr(ref rpath, "%n", rep);
        rep = string.Format("{0:D4}", week);
        stat |= GlobalMembersRtkcmn.repstr(ref rpath, "%W", rep);
        rep = string.Format("{0:D}", dow);
        stat |= GlobalMembersRtkcmn.repstr(ref rpath, "%D", rep);
        rep = string.Format("{0}", 'a' + (int)ep[3]);
        stat |= GlobalMembersRtkcmn.repstr(ref rpath, "%H", rep);
        rep = string.Format("{0:D2}", ((int)ep[4] / 15) * 15);
        stat |= GlobalMembersRtkcmn.repstr(ref rpath, "%t", rep);
    }
    else if (StringFunctions.StrStr(rpath, "%ha") || StringFunctions.StrStr(rpath, "%hb") || StringFunctions.StrStr(rpath, "%hc") || StringFunctions.StrStr(rpath, "%Y") || StringFunctions.StrStr(rpath, "%y") || StringFunctions.StrStr(rpath, "%m") || StringFunctions.StrStr(rpath, "%d") || StringFunctions.StrStr(rpath, "%h") || StringFunctions.StrStr(rpath, "%M") || StringFunctions.StrStr(rpath, "%S") || StringFunctions.StrStr(rpath, "%n") || StringFunctions.StrStr(rpath, "%W") || StringFunctions.StrStr(rpath, "%D") || StringFunctions.StrStr(rpath, "%H") || StringFunctions.StrStr(rpath, "%t"))
    {
        return -1; // no valid time
    }
    GlobalMembersRtkcmn.trace(3, "reppath : rpath=%s\n", rpath);
    return stat;
}
/* replace keywords in file path and generate multiple paths -------------------
* replace keywords in file path with date, time, rover and base station id
* generate multiple keywords-replaced paths
* args   : char   *path     I   file path (see below)
*          char   *rpath[]  O   file paths in which keywords replaced
*          int    nmax      I   max number of output file paths
*          gtime_t ts       I   time start (gpst)
*          gtime_t te       I   time end   (gpst)
*          char   *rov      I   rover id string        ("": not replaced)
*          char   *base     I   base station id string ("": not replaced)
* return : number of replaced file paths
* notes  : see reppath() for replacements of keywords.
*          minimum interval of time replaced is 900s.
*-----------------------------------------------------------------------------*/
public static int reppaths(string path, string[] rpath, int nmax, gtime_t ts, gtime_t te, string rov, string @base)
{
    gtime_t time = new gtime_t();
    double tow;
    double tint = 86400.0;
    int i;
    int n = 0;
    int week;

    GlobalMembersRtkcmn.trace(3, "reppaths: path =%s nmax=%d rov=%s base=%s\n", path, nmax, rov, @base);

    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
    //ORIGINAL LINE: if (ts.time==0||te.time==0||timediff(ts,te)>0.0)
    if (ts.time == 0 || te.time == 0 || GlobalMembersRtkcmn.timediff(new gtime_t(ts), new gtime_t(te)) > 0.0)
    {
        return 0;
    }

    if (StringFunctions.StrStr(path, "%S") || StringFunctions.StrStr(path, "%M") || StringFunctions.StrStr(path, "%t"))
    {
        tint = 900.0;
    }
    else if (StringFunctions.StrStr(path, "%h") || StringFunctions.StrStr(path, "%H"))
    {
        tint = 3600.0;
    }

    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
    //ORIGINAL LINE: tow=time2gpst(ts,&week);
    tow = GlobalMembersRtkcmn.time2gpst(new gtime_t(ts), ref week);
    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
    //ORIGINAL LINE: time=gpst2time(week,floor(tow/tint)*tint);
    time.CopyFrom(GlobalMembersRtkcmn.gpst2time(week, Math.Floor(tow / tint) * tint));

    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
    //ORIGINAL LINE: while (timediff(time,te)<=0.0&&n<nmax)
    while (GlobalMembersRtkcmn.timediff(new gtime_t(time), new gtime_t(te)) <= 0.0 && n < nmax)
    {
        //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
        //ORIGINAL LINE: reppath(path,rpath[n],time,rov,base);
        GlobalMembersRtkcmn.reppath(path, ref rpath[n], new gtime_t(time), rov, @base);
        if (n == 0 || string.Compare(rpath[n], rpath[n - 1]))
        {
            n++;
        }
        //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
        //ORIGINAL LINE: time=timeadd(time,tint);
        //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
        time.CopyFrom(GlobalMembersRtkcmn.timeadd(new gtime_t(time), tint));
    }
    for (i = 0; i < n; i++)
    {
        GlobalMembersRtkcmn.trace(3, "reppaths: rpath=%s\n", rpath[i]);
    }
    return n;
}
/* satellite carrier wave length -----------------------------------------------
* get satellite carrier wave lengths
* args   : int    sat       I   satellite number
*          int    frq       I   frequency index (0:L1,1:L2,2:L5/3,...)
*          nav_t  *nav      I   navigation messages
* return : carrier wave length (m) (0.0: error)
*-----------------------------------------------------------------------------*/
public static double satwavelen(int sat, int frq, nav_t nav)
{
    double[] freq_glo = { DefineConstants.FREQ1_GLO, DefineConstants.FREQ2_GLO, DefineConstants.FREQ3_GLO };
    double[] dfrq_glo = { DefineConstants.DFRQ1_GLO, DefineConstants.DFRQ2_GLO, 0.0 };
    int i;
    int sys = GlobalMembersRtkcmn.satsys(sat, null);

    if (sys == DefineConstants.SYS_GLO)
    {
        if (0 <= frq && frq <= 2)
        {
            for (i = 0; i < nav.ng; i++)
            {
                if (nav.geph[i].sat != sat)
                    continue;
                return DefineConstants.CLIGHT / (freq_glo[frq] + dfrq_glo[frq] * nav.geph[i].frq);
            }
        }
    }
    else if (sys == DefineConstants.SYS_CMP)
    {
        if (frq == 0) // B1
        {
            return DefineConstants.CLIGHT / DefineConstants.FREQ1_CMP;
        }
        else if (frq == 1) // B3
        {
            return DefineConstants.CLIGHT / DefineConstants.FREQ2_CMP;
        }
        else if (frq == 2) // B2
        {
            return DefineConstants.CLIGHT / DefineConstants.FREQ3_CMP;
        }
    }
    else
    {
        if (frq == 0) // L1/E1
        {
            return DefineConstants.CLIGHT / DefineConstants.FREQ1;
        }
        else if (frq == 1) // L2
        {
            return DefineConstants.CLIGHT / DefineConstants.FREQ2;
        }
        else if (frq == 2) // L5/E5a
        {
            return DefineConstants.CLIGHT / DefineConstants.FREQ5;
        }
        else if (frq == 3) // L6/LEX
        {
            return DefineConstants.CLIGHT / DefineConstants.FREQ6;
        }
        else if (frq == 4) // E5b
        {
            return DefineConstants.CLIGHT / DefineConstants.FREQ7;
        }
        else if (frq == 5) // E5a+b
        {
            return DefineConstants.CLIGHT / DefineConstants.FREQ8;
        }
    }
    return 0.0;
}
/* geometric distance ----------------------------------------------------------
* compute geometric distance and receiver-to-satellite unit vector
* args   : double *rs       I   satellilte position (ecef at transmission) (m)
*          double *rr       I   receiver position (ecef at reception) (m)
*          double *e        O   line-of-sight vector (ecef)
* return : geometric distance (m) (0>:error/no satellite position)
* notes  : distance includes sagnac effect correction
*-----------------------------------------------------------------------------*/
public static double geodist(double[] rs, double[] rr, double[] e)
{
    double r;
    int i;

    if (GlobalMembersRtkcmn.norm(rs, 3) < DefineConstants.RE_WGS84)
    {
        return -1.0;
    }
    for (i = 0; i < 3; i++)
    {
        e[i] = rs[i] - rr[i];
    }
    r = GlobalMembersRtkcmn.norm(e, 3);
    for (i = 0; i < 3; i++)
    {
        e[i] /= r;
    }
    return r + DefineConstants.OMGE * (rs[0] * rr[1] - rs[1] * rr[0]) / DefineConstants.CLIGHT;
}
/* satellite azimuth/elevation angle -------------------------------------------
* compute satellite azimuth/elevation angle
* args   : double *pos      I   geodetic position {lat,lon,h} (rad,m)
*          double *e        I   receiver-to-satellilte unit vevtor (ecef)
*          double *azel     IO  azimuth/elevation {az,el} (rad) (NULL: no output)
*                               (0.0<=azel[0]<2*pi,-pi/2<=azel[1]<=pi/2)
* return : elevation angle (rad)
*-----------------------------------------------------------------------------*/
public static double satazel(double[] pos, double e, double[] azel)
{
    double az = 0.0;
    double el = DefineConstants.PI / 2.0;
    double[] enu = new double[3];

    if (pos[2] > -DefineConstants.RE_WGS84)
    {
        GlobalMembersRtkcmn.ecef2enu(pos, e, ref enu);
        az = GlobalMembersRtkcmn.dot(enu, enu, 2) < 1E-12 ? 0.0 : Math.Atan2(enu[0], enu[1]);
        if (az < 0.0)
        {
            az += 2 * DefineConstants.PI;
        }
        el = Math.Asin(enu[2]);
    }
    if (azel != 0)
    {
        azel[0] = az;
        azel[1] = el;
    }
    return el;
}
/* compute dops ----------------------------------------------------------------
* compute DOP (dilution of precision)
* args   : int    ns        I   number of satellites
*          double *azel     I   satellite azimuth/elevation angle (rad)
*          double elmin     I   elevation cutoff angle (rad)
*          double *dop      O   DOPs {GDOP,PDOP,HDOP,VDOP}
* return : none
* notes  : dop[0]-[3] return 0 in case of dop computation error
*-----------------------------------------------------------------------------*/


public static void dops(int ns, double[] azel, double elmin, double[] dop)
{
    double[] H = new double[4 * DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1];
    double[] Q = new double[16];
    double cosel;
    double sinel;
    int i;
    int n;

    for (i = 0; i < 4; i++)
    {
        dop[i] = 0.0;
    }
    for (i = n = 0; i < ns && i < DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1; i++)
    {
        if (azel[1 + i * 2] < elmin || azel[1 + i * 2] <= 0.0)
            continue;
        cosel = Math.Cos(azel[1 + i * 2]);
        sinel = Math.Sin(azel[1 + i * 2]);
        H[4 * n] = cosel * Math.Sin(azel[i * 2]);
        H[1 + 4 * n] = cosel * Math.Cos(azel[i * 2]);
        H[2 + 4 * n] = sinel;
        H[3 + 4 * n++] = 1.0;
    }
    if (n < 4)
        return;

    GlobalMembersRtkcmn.matmul("NT", 4, 4, n, 1.0, H, H, 0.0, ref Q);
    if (GlobalMembersRtkcmn.matinv(ref Q, 4) == 0)
    {
        dop[0] = ((Q[0] + Q[5] + Q[10] + Q[15]) < 0.0 ? 0.0 : Math.Sqrt(Q[0] + Q[5] + Q[10] + Q[15]));
        dop[1] = ((Q[0] + Q[5] + Q[10]) < 0.0 ? 0.0 : Math.Sqrt(Q[0] + Q[5] + Q[10]));
        dop[2] = ((Q[0] + Q[5]) < 0.0 ? 0.0 : Math.Sqrt(Q[0] + Q[5]));
        dop[3] = ((Q[10]) < 0.0 ? 0.0 : Math.Sqrt(Q[10]));
    }
}
/* ionosphere model ------------------------------------------------------------
* compute ionospheric delay by broadcast ionosphere model (klobuchar model)
* args   : gtime_t t        I   time (gpst)
*          double *ion      I   iono model parameters {a0,a1,a2,a3,b0,b1,b2,b3}
*          double *pos      I   receiver position {lat,lon,h} (rad,m)
*          double *azel     I   azimuth/elevation angle {az,el} (rad)
* return : ionospheric delay (L1) (m)
*-----------------------------------------------------------------------------*/
public static double ionmodel(gtime_t t, double[] ion, double[] pos, double[] azel)
{
    double[] ion_default = { 0.1118E-0x7, -0.7451E-0x8, -0.5961E-0x7, 0.1192E-0x6, 0.1167E+0x6, -0.2294E+0x6, -0.1311E+0x6, 0.1049E+0x7 }; // 2004/1/1
    double tt;
    double f;
    double psi;
    double phi;
    double lam;
    double amp;
    double per;
    double x;
    int week;

    if (pos[2] < -1E3 || azel[1] <= 0)
    {
        return 0.0;
    }
    if (GlobalMembersRtkcmn.norm(ion, 8) <= 0.0)
    {
        ion = ion_default;
    }

    /* earth centered angle (semi-circle) */
    psi = 0.0137 / (azel[1] / DefineConstants.PI + 0.11) - 0.022;

    /* subionospheric latitude/longitude (semi-circle) */
    phi = pos[0] / DefineConstants.PI + psi * Math.Cos(azel[0]);
    if (phi > 0.416)
    {
        phi = 0.416;
    }
    else if (phi < -0.416)
    {
        phi = -0.416;
    }
    lam = pos[1] / DefineConstants.PI + psi * Math.Sin(azel[0]) / Math.Cos(phi * DefineConstants.PI);

    /* geomagnetic latitude (semi-circle) */
    phi += 0.064 * Math.Cos((lam - 1.617) * DefineConstants.PI);

    /* local time (s) */
    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
    //ORIGINAL LINE: tt=43200.0 *lam+time2gpst(t,&week);
    tt = 43200.0 * lam + GlobalMembersRtkcmn.time2gpst(new gtime_t(t), ref week);
    tt -= Math.Floor(tt / 86400.0) * 86400.0; // 0<=tt<86400

    /* slant factor */
    f = 1.0 + 16.0 * Math.Pow(0.53 - azel[1] / DefineConstants.PI, 3.0);

    /* ionospheric delay */
    amp = ion[0] + phi * (ion[1] + phi * (ion[2] + phi * ion[3]));
    per = ion[4] + phi * (ion[5] + phi * (ion[6] + phi * ion[7]));
    amp = amp < 0.0 ? 0.0 : amp;
    per = per < 72000.0 ? 72000.0 : per;
    x = 2.0 * DefineConstants.PI * (tt - 50400.0) / per;

    return DefineConstants.CLIGHT * f * (Math.Abs(x) < 1.57 ? 5E-9 + amp * (1.0 + x * x * (-0.5 + x * x / 24.0)) : 5E-9);
}
/* ionosphere mapping function -------------------------------------------------
* compute ionospheric delay mapping function by single layer model
* args   : double *pos      I   receiver position {lat,lon,h} (rad,m)
*          double *azel     I   azimuth/elevation angle {az,el} (rad)
* return : ionospheric mapping function
*-----------------------------------------------------------------------------*/
public static double ionmapf(double[] pos, double[] azel)
{
    if (pos[2] >= DefineConstants.HION)
    {
        return 1.0;
    }
    return 1.0 / Math.Cos(Math.Asin((DefineConstants.RE_WGS84 + pos[2]) / (DefineConstants.RE_WGS84 + DefineConstants.HION) * Math.Sin(DefineConstants.PI / 2.0 - azel[1])));
}
/* ionospheric pierce point position -------------------------------------------
* compute ionospheric pierce point (ipp) position and slant factor
* args   : double *pos      I   receiver position {lat,lon,h} (rad,m)
*          double *azel     I   azimuth/elevation angle {az,el} (rad)
*          double re        I   earth radius (km)
*          double hion      I   altitude of ionosphere (km)
*          double *posp     O   pierce point position {lat,lon,h} (rad,m)
* return : slant factor
* notes  : see ref [2], only valid on the earth surface
*          fixing bug on ref [2] A.4.4.10.1 A-22,23
*-----------------------------------------------------------------------------*/
public static double ionppp(double[] pos, double[] azel, double re, double hion, double[] posp)
{
    double cosaz;
    double rp;
    double ap;
    double sinap;
    double tanap;

    rp = re / (re + hion) * Math.Cos(azel[1]);
    ap = DefineConstants.PI / 2.0 - azel[1] - Math.Asin(rp);
    sinap = Math.Sin(ap);
    tanap = Math.Tan(ap);
    cosaz = Math.Cos(azel[0]);
    posp[0] = Math.Asin(Math.Sin(pos[0]) * Math.Cos(ap) + Math.Cos(pos[0]) * sinap * cosaz);

    if ((pos[0] > 70.0 * DefineConstants.PI / 180.0 && tanap * cosaz > Math.Tan(DefineConstants.PI / 2.0 - pos[0])) || (pos[0] < -70.0 * DefineConstants.PI / 180.0 && -tanap * cosaz > Math.Tan(DefineConstants.PI / 2.0 + pos[0])))
    {
        posp[1] = pos[1] + DefineConstants.PI - Math.Asin(sinap * Math.Sin(azel[0]) / Math.Cos(posp[0]));
    }
    else
    {
        posp[1] = pos[1] + Math.Asin(sinap * Math.Sin(azel[0]) / Math.Cos(posp[0]));
    }
    return 1.0 / Math.Sqrt(1.0 - rp * rp);
}
/* troposphere model -----------------------------------------------------------
* compute tropospheric delay by standard atmosphere and saastamoinen model
* args   : gtime_t time     I   time
*          double *pos      I   receiver position {lat,lon,h} (rad,m)
*          double *azel     I   azimuth/elevation angle {az,el} (rad)
*          double humi      I   relative humidity
* return : tropospheric delay (m)
*-----------------------------------------------------------------------------*/
public static double tropmodel(gtime_t time, double[] pos, double[] azel, double humi)
{
    const double temp0 = 15.0; // temparature at sea level
    double hgt;
    double pres;
    double temp;
    double e;
    double z;
    double trph;
    double trpw;

    if (pos[2] < -100.0 || 1E4 < pos[2] || azel[1] <= 0)
    {
        return 0.0;
    }

    /* standard atmosphere */
    hgt = pos[2] < 0.0 ? 0.0 : pos[2];

    pres = 1013.25 * Math.Pow(1.0 - 2.2557E-5 * hgt, 5.2568);
    temp = temp0 - 6.5E-3 * hgt + 273.16;
    e = 6.108 * humi * Math.Exp((17.15 * temp - 4684.0) / (temp - 38.45));

    /* saastamoninen model */
    z = DefineConstants.PI / 2.0 - azel[1];
    trph = 0.0022768 * pres / (1.0 - 0.00266 * Math.Cos(2.0 * pos[0]) - 0.00028 * hgt / 1E3) / Math.Cos(z);
    trpw = 0.002277 * (1255.0 / temp + 0.05) * e / Math.Cos(z);
    return trph + trpw;
}
internal static double interpc(double[] coef, double lat)
{
    int i = (int)(lat / 15.0);
    if (i < 1)
    {
        return coef[0];
    }
    else if (i > 4)
    {
        return coef[4];
    }
    return coef[i - 1] * (1.0 - lat / 15.0 + i) + coef[i] * (lat / 15.0 - i);
}
internal static double mapf(double el, double a, double b, double c)
{
    double sinel = Math.Sin(el);
    return (1.0 + a / (1.0 + b / (1.0 + c))) / (sinel + (a / (sinel + b / (sinel + c))));
}
internal static double nmf(gtime_t time, double[] pos, double[] azel, ref double mapfw)
{
    /* ref [5] table 3 */
    /* hydro-ave-a,b,c, hydro-amp-a,b,c, wet-a,b,c at latitude 15,30,45,60,75 */
    double[,] coef = { { 1.2769934E-3, 1.2683230E-3, 1.2465397E-3, 1.2196049E-3, 1.2045996E-3 }, { 2.9153695E-3, 2.9152299E-3, 2.9288445E-3, 2.9022565E-3, 2.9024912E-3 }, { 62.610505E-3, 62.837393E-3, 63.721774E-3, 63.824265E-3, 64.258455E-3 }, { 0.0000000E-0, 1.2709626E-5, 2.6523662E-5, 3.4000452E-5, 4.1202191E-5 }, { 0.0000000E-0, 2.1414979E-5, 3.0160779E-5, 7.2562722E-5, 11.723375E-5 }, { 0.0000000E-0, 9.0128400E-5, 4.3497037E-5, 84.795348E-5, 170.37206E-5 }, { 5.8021897E-4, 5.6794847E-4, 5.8118019E-4, 5.9727542E-4, 6.1641693E-4 }, { 1.4275268E-3, 1.5138625E-3, 1.4572752E-3, 1.5007428E-3, 1.7599082E-3 }, { 4.3472961E-2, 4.6729510E-2, 4.3908931E-2, 4.4626982E-2, 5.4736038E-2 } };
    double[] aht = { 2.53E-5, 5.49E-3, 1.14E-3 }; // height correction

    double y;
    double cosy;
    double[] ah = new double[3];
    double[] aw = new double[3];
    double dm;
    double el = azel[1];
    double lat = pos[0] * 180.0 / DefineConstants.PI;
    double hgt = pos[2];
    int i;

    if (el <= 0.0)
    {
        if (mapfw != 0)
        {
            mapfw = 0.0;
        }
        return 0.0;
    }
    /* year from doy 28, added half a year for southern latitudes */
    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
    //ORIGINAL LINE: y=(time2doy(time)-28.0)/365.25+(lat<0.0?0.5:0.0);
    y = (GlobalMembersRtkcmn.time2doy(new gtime_t(time)) - 28.0) / 365.25 + (lat < 0.0 ? 0.5 : 0.0);

    cosy = Math.Cos(2.0 * DefineConstants.PI * y);
    lat = Math.Abs(lat);

    for (i = 0; i < 3; i++)
    {
        ah[i] = GlobalMembersRtkcmn.interpc(coef[i], lat) - GlobalMembersRtkcmn.interpc(coef[i + 3], lat) * cosy;
        aw[i] = GlobalMembersRtkcmn.interpc(coef[i + 6], lat);
    }
    /* ellipsoidal height is used instead of height above sea level */
    dm = (1.0 / Math.Sin(el) - GlobalMembersRtkcmn.mapf(el, aht[0], aht[1], aht[2])) * hgt / 1E3;

    if (mapfw != 0)
    {
        mapfw = GlobalMembersRtkcmn.mapf(el, aw[0], aw[1], aw[2]);
    }

    return GlobalMembersRtkcmn.mapf(el, ah[0], ah[1], ah[2]) + dm;
}

/* troposphere mapping function ------------------------------------------------
* compute tropospheric mapping function by NMF
* args   : gtime_t t        I   time
*          double *pos      I   receiver position {lat,lon,h} (rad,m)
*          double *azel     I   azimuth/elevation angle {az,el} (rad)
*          double *mapfw    IO  wet mapping function (NULL: not output)
* return : dry mapping function
* note   : see ref [5] (NMF) and [9] (GMF)
*          original JGR paper of [5] has bugs in eq.(4) and (5). the corrected
*          paper is obtained from:
*          ftp://web.haystack.edu/pub/aen/nmf/NMF_JGR.pdf
*-----------------------------------------------------------------------------*/
public static double tropmapf(gtime_t time, double[] pos, double[] azel, ref double mapfw)
{
#if IERS_MODEL
		double[] ep = {2000,1,1,12,0,0};
		double mjd;
		double lat;
		double lon;
		double hgt;
		double zd;
		double gmfh;
		double gmfw;
#endif
    GlobalMembersRtkcmn.trace(4, "tropmapf: pos=%10.6f %11.6f %6.1f azel=%5.1f %4.1f\n", pos[0] * 180.0 / DefineConstants.PI, pos[1] * 180.0 / DefineConstants.PI, pos[2], azel[0] * 180.0 / DefineConstants.PI, azel[1] * 180.0 / DefineConstants.PI);

    if (pos[2] < -1000.0 || pos[2] > 20000.0)
    {
        if (mapfw != 0)
        {
            mapfw = 0.0;
        }
        return 0.0;
    }
#if IERS_MODEL
//C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
//ORIGINAL LINE: mjd=51544.5+(timediff(time,epoch2time(ep)))/86400.0;
		mjd = 51544.5 + (GlobalMembersRtkcmn.timediff(new gtime_t(time), GlobalMembersRtkcmn.epoch2time(ep))) / 86400.0;
		lat = pos[0];
		lon = pos[1];
		hgt = pos[2] - GlobalMembersGeoid.geoidh(pos); // height in m (mean sea level)
		zd = DefineConstants.PI / 2.0 - azel[1];

		/* call GMF */
		gmf_(ref mjd, ref lat, ref lon, ref hgt, ref zd, ref gmfh, ref gmfw);

		if (mapfw != 0)
		{
			mapfw = gmfw;
		}
		return gmfh;
#else
    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
    //ORIGINAL LINE: return nmf(time,pos,azel,mapfw);
    return GlobalMembersRtkcmn.nmf(new gtime_t(time), pos, azel, ref mapfw); // NMF
#endif
}
/* interpolate antenna phase center variation --------------------------------*/
internal static double interpvar(double ang, double[] @var)
{
    double a = ang / 5.0; // ang=0-90
    int i = (int)a;
    if (i < 0)
    {
        return @var[0];
    }
    else if (i >= 18)
    {
        return @var[18];
    }
    return @var[i] * (1.0 - a + i) + @var[i + 1] * (a - i);
}
/* receiver antenna model ------------------------------------------------------
* compute antenna offset by antenna phase center parameters
* args   : pcv_t *pcv       I   antenna phase center parameters
*          double *azel     I   azimuth/elevation for receiver {az,el} (rad)
*          int     opt      I   option (0:only offset,1:offset+pcv)
*          double *dant     O   range offsets for each frequency (m)
* return : none
* notes  : current version does not support azimuth dependent terms
*-----------------------------------------------------------------------------*/
public static void antmodel(pcv_t pcv, double[] del, double[] azel, int opt, double[] dant)
{
    double[] e = new double[3];
    double[] off = new double[3];
    double cosel = Math.Cos(azel[1]);
    int i;
    int j;

    GlobalMembersRtkcmn.trace(4, "antmodel: azel=%6.1f %4.1f opt=%d\n", azel[0] * 180.0 / DefineConstants.PI, azel[1] * 180.0 / DefineConstants.PI, opt);

    e[0] = Math.Sin(azel[0]) * cosel;
    e[1] = Math.Cos(azel[0]) * cosel;
    e[2] = Math.Sin(azel[1]);

    for (i = 0; i < DefineConstants.NFREQ; i++)
    {
        for (j = 0; j < 3; j++)
        {
            off[j] = pcv.off[i, j] + del[j];
        }

        dant[i] = -GlobalMembersRtkcmn.dot(off, e, 3) + (opt != 0 ? GlobalMembersRtkcmn.interpvar(90.0 - azel[1] * 180.0 / DefineConstants.PI, pcv.@var[i]) : 0.0);
    }
    GlobalMembersRtkcmn.trace(5, "antmodel: dant=%6.3f %6.3f\n", dant[0], dant[1]);
}
/* satellite antenna model ------------------------------------------------------
* compute satellite antenna phase center parameters
* args   : pcv_t *pcv       I   antenna phase center parameters
*          double nadir     I   nadir angle for satellite (rad)
*          double *dant     O   range offsets for each frequency (m)
* return : none
*-----------------------------------------------------------------------------*/
public static void antmodel_s(pcv_t pcv, double nadir, double[] dant)
{
    int i;

    GlobalMembersRtkcmn.trace(4, "antmodel_s: nadir=%6.1f\n", nadir * 180.0 / DefineConstants.PI);

    for (i = 0; i < DefineConstants.NFREQ; i++)
    {
        dant[i] = GlobalMembersRtkcmn.interpvar(nadir * 180.0 / DefineConstants.PI * 5.0, pcv.@var[i]);
    }
    GlobalMembersRtkcmn.trace(5, "antmodel_s: dant=%6.3f %6.3f\n", dant[0], dant[1]);
}
/* sun and moon position in eci (ref [4] 5.1.1, 5.2.1) -----------------------*/
internal static void sunmoonpos_eci(gtime_t tut, double[] rsun, double[] rmoon)
{
    double[] ep2000 = { 2000, 1, 1, 12, 0, 0 };
    double t;
    double[] f = new double[5];
    double eps;
    double Ms;
    double ls;
    double rs;
    double lm;
    double pm;
    double rm;
    double sine;
    double cose;
    double sinp;
    double cosp;
    double sinl;
    double cosl;

    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
    //ORIGINAL LINE: trace(3,"sunmoonpos_eci: tut=%s\n",time_str(tut,3));
    GlobalMembersRtkcmn.trace(3, "sunmoonpos_eci: tut=%s\n", GlobalMembersRtkcmn.time_str(new gtime_t(tut), 3));

    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
    //ORIGINAL LINE: t=timediff(tut,epoch2time(ep2000))/86400.0/36525.0;
    t = GlobalMembersRtkcmn.timediff(new gtime_t(tut), GlobalMembersRtkcmn.epoch2time(ep2000)) / 86400.0 / 36525.0;

    /* astronomical arguments */
    GlobalMembersRtkcmn.ast_args(t, f);

    /* obliquity of the ecliptic */
    eps = 23.439291 - 0.0130042 * t;
    sine = Math.Sin(eps * DefineConstants.PI / 180.0);
    cose = Math.Cos(eps * DefineConstants.PI / 180.0);

    /* sun position in eci */
    if (rsun != 0)
    {
        Ms = 357.5277233 + 35999.05034 * t;
        ls = 280.460 + 36000.770 * t + 1.914666471 * Math.Sin(Ms * DefineConstants.PI / 180.0) + 0.019994643 * Math.Sin(2.0 * Ms * DefineConstants.PI / 180.0);
        rs = DefineConstants.AU * (1.000140612 - 0.016708617 * Math.Cos(Ms * DefineConstants.PI / 180.0) - 0.000139589 * Math.Cos(2.0 * Ms * DefineConstants.PI / 180.0));
        sinl = Math.Sin(ls * DefineConstants.PI / 180.0);
        cosl = Math.Cos(ls * DefineConstants.PI / 180.0);
        rsun[0] = rs * cosl;
        rsun[1] = rs * cose * sinl;
        rsun[2] = rs * sine * sinl;

        GlobalMembersRtkcmn.trace(5, "rsun =%.3f %.3f %.3f\n", rsun[0], rsun[1], rsun[2]);
    }
    /* moon position in eci */
    if (rmoon != 0)
    {
        lm = 218.32 + 481267.883 * t + 6.29 * Math.Sin(f[0]) - 1.27 * Math.Sin(f[0] - 2.0 * f[3]) + 0.66 * Math.Sin(2.0 * f[3]) + 0.21 * Math.Sin(2.0 * f[0]) - 0.19 * Math.Sin(f[1]) - 0.11 * Math.Sin(2.0 * f[2]);
        pm = 5.13 * Math.Sin(f[2]) + 0.28 * Math.Sin(f[0] + f[2]) - 0.28 * Math.Sin(f[2] - f[0]) - 0.17 * Math.Sin(f[2] - 2.0 * f[3]);
        rm = DefineConstants.RE_WGS84 / Math.Sin((0.9508 + 0.0518 * Math.Cos(f[0]) + 0.0095 * Math.Cos(f[0] - 2.0 * f[3]) + 0.0078 * Math.Cos(2.0 * f[3]) + 0.0028 * Math.Cos(2.0 * f[0])) * DefineConstants.PI / 180.0);
        sinl = Math.Sin(lm * DefineConstants.PI / 180.0);
        cosl = Math.Cos(lm * DefineConstants.PI / 180.0);
        sinp = Math.Sin(pm * DefineConstants.PI / 180.0);
        cosp = Math.Cos(pm * DefineConstants.PI / 180.0);
        rmoon[0] = rm * cosp * cosl;
        rmoon[1] = rm * (cose * cosp * sinl - sine * sinp);
        rmoon[2] = rm * (sine * cosp * sinl + cose * sinp);

        GlobalMembersRtkcmn.trace(5, "rmoon=%.3f %.3f %.3f\n", rmoon[0], rmoon[1], rmoon[2]);
    }
}
/* sun and moon position -------------------------------------------------------
* get sun and moon position in ecef
* args   : gtime_t tut      I   time in ut1
*          double *erpv     I   erp value {xp,yp,ut1_utc,lod} (rad,rad,s,s/d)
*          double *rsun     IO  sun position in ecef  (m) (NULL: not output)
*          double *rmoon    IO  moon position in ecef (m) (NULL: not output)
*          double *gmst     O   gmst (rad)
* return : none
*-----------------------------------------------------------------------------*/
public static void sunmoonpos(gtime_t tutc, double[] erpv, ref double rsun, ref double rmoon, ref double gmst)
{
    gtime_t tut = new gtime_t();
    double[] rs = new double[3];
    double[] rm = new double[3];
    double[] U = new double[9];
    double gmst_;

    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
    //ORIGINAL LINE: trace(3,"sunmoonpos: tutc=%s\n",time_str(tutc,3));
    GlobalMembersRtkcmn.trace(3, "sunmoonpos: tutc=%s\n", GlobalMembersRtkcmn.time_str(new gtime_t(tutc), 3));

    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
    //ORIGINAL LINE: tut=timeadd(tutc,erpv[2]);
    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
    tut.CopyFrom(GlobalMembersRtkcmn.timeadd(new gtime_t(tutc), erpv[2])); // utc -> ut1

    /* sun and moon position in eci */
    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
    //ORIGINAL LINE: sunmoonpos_eci(tut,rsun?rs:null,rmoon?rm:null);
    GlobalMembersRtkcmn.sunmoonpos_eci(new gtime_t(tut), rsun != 0 ? rs : null, rmoon != 0 ? rm : null);

    /* eci to ecef transformation matrix */
    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
    //ORIGINAL LINE: eci2ecef(tutc,erpv,U,&gmst_);
    GlobalMembersRtkcmn.eci2ecef(new gtime_t(tutc), erpv, U, ref gmst_);

    /* sun and moon postion in ecef */
    if (rsun != 0)
    {
        GlobalMembersRtkcmn.matmul("NN", 3, 1, 3, 1.0, U, rs, 0.0, ref rsun);
    }
    if (rmoon != 0)
    {
        GlobalMembersRtkcmn.matmul("NN", 3, 1, 3, 1.0, U, rm, 0.0, ref rmoon);
    }
    if (gmst != 0)
    {
        gmst = gmst_;
    }
}
/* phase windup correction -----------------------------------------------------
* phase windup correction (ref [7] 5.1.2)
* args   : gtime_t time     I   time (GPST)
*          double  *rs      I   satellite position (ecef) {x,y,z} (m)
*          double  *rr      I   receiver  position (ecef) {x,y,z} (m)
*          double  *phw     IO  phase windup correction (cycle)
* return : none
* notes  : the previous value of phase windup correction should be set to *phw
*          as an input. the function assumes windup correction has no jump more
*          than 0.5 cycle.
*-----------------------------------------------------------------------------*/
public static void windupcorr(gtime_t time, double[] rs, double[] rr, ref double phw)
{
    double[] ek = new double[3];
    double[] exs = new double[3];
    double[] eys = new double[3];
    double[] ezs = new double[3];
    double[] ess = new double[3];
    double[] exr = new double[3];
    double[] eyr = new double[3];
    double[] eks = new double[3];
    double[] ekr = new double[3];
    double[] E = new double[9];
    double[] dr = new double[3];
    double[] ds = new double[3];
    double[] drs = new double[3];
    double[] r = new double[3];
    double[] pos = new double[3];
    double[] rsun = new double[3];
    double cosp;
    double ph;
    double[] erpv = { 0, null, null, null, null };
    int i;

    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
    //ORIGINAL LINE: trace(4,"windupcorr: time=%s\n",time_str(time,0));
    GlobalMembersRtkcmn.trace(4, "windupcorr: time=%s\n", GlobalMembersRtkcmn.time_str(new gtime_t(time), 0));

    /* sun position in ecef */
    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
    //ORIGINAL LINE: sunmoonpos(gpst2utc(time),erpv,rsun,null,null);
    GlobalMembersRtkcmn.sunmoonpos(GlobalMembersRtkcmn.gpst2utc(new gtime_t(time)), erpv, ref rsun, null, null);

    /* unit vector satellite to receiver */
    for (i = 0; i < 3; i++)
    {
        r[i] = rr[i] - rs[i];
    }
    if (GlobalMembersRtkcmn.normv3(r, ek) == 0)
        return;

    /* unit vectors of satellite antenna */
    for (i = 0; i < 3; i++)
    {
        r[i] = -rs[i];
    }
    if (GlobalMembersRtkcmn.normv3(r, ezs) == 0)
        return;
    for (i = 0; i < 3; i++)
    {
        r[i] = rsun[i] - rs[i];
    }
    if (GlobalMembersRtkcmn.normv3(r, ess) == 0)
        return;
    GlobalMembersRtkcmn.cross3(ezs, ess, r);
    if (GlobalMembersRtkcmn.normv3(r, eys) == 0)
        return;
    GlobalMembersRtkcmn.cross3(eys, ezs, exs);

    /* unit vectors of receiver antenna */
    GlobalMembersRtkcmn.ecef2pos(rr, pos);
    GlobalMembersRtkcmn.xyz2enu(pos, E);
    exr[0] = E[1]; // x = north
    exr[1] = E[4];
    exr[2] = E[7];
    eyr[0] = -E[0]; // y = west
    eyr[1] = -E[3];
    eyr[2] = -E[6];

    /* phase windup effect */
    GlobalMembersRtkcmn.cross3(ek, eys, eks);
    GlobalMembersRtkcmn.cross3(ek, eyr, ekr);
    for (i = 0; i < 3; i++)
    {
        ds[i] = exs[i] - ek[i] * GlobalMembersRtkcmn.dot(ek, exs, 3) - eks[i];
        dr[i] = exr[i] - ek[i] * GlobalMembersRtkcmn.dot(ek, exr, 3) + ekr[i];
    }
    cosp = GlobalMembersRtkcmn.dot(ds, dr, 3) / GlobalMembersRtkcmn.norm(ds, 3) / GlobalMembersRtkcmn.norm(dr, 3);
    if (cosp < -1.0)
    {
        cosp = -1.0;
    }
    else if (cosp > 1.0)
    {
        cosp = 1.0;
    }
    ph = Math.Acos(cosp) / 2.0 / DefineConstants.PI;
    GlobalMembersRtkcmn.cross3(ds, dr, drs);
    if (GlobalMembersRtkcmn.dot(ek, drs, 3) < 0.0)
    {
        ph = -ph;
    }

    phw = ph + Math.Floor(phw - ph + 0.5); // in cycle
}
/* carrier smoothing -----------------------------------------------------------
* carrier smoothing by Hatch filter
* args   : obs_t  *obs      IO  raw observation data/smoothed observation data
*          int    ns        I   smoothing window size (epochs)
* return : none
*-----------------------------------------------------------------------------*/
public static void csmooth(obs_t obs, int ns)
{
    double[,,] Ps = { { { 0 } } };
    double[,,] Lp = { { { 0 } } };
    double dcp;
    int i;
    int j;
    int s;
    int r;
    int[,,] n = new int[2, DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1, DefineConstants.NFREQ];
    obsd_t p;

    GlobalMembersRtkcmn.trace(3, "csmooth: nobs=%d,ns=%d\n", obs.n, ns);

    for (i = 0; i < obs.n; i++)
    {
        p = obs.data[i];
        s = p.sat;
        r = p.rcv;
        for (j = 0; j < DefineConstants.NFREQ; j++)
        {
            if (s <= 0 || DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1 < s || r <= 0 || 2 < r)
                continue;
            if (p.P[j] == 0.0 || p.L[j] == 0.0)
                continue;
            if (p.LLI[j] != 0)
            {
                n[r - 1, s - 1, j] = 0;
            }
            if (n[r - 1, s - 1, j] == 0)
            {
                Ps[r - 1, s - 1, j] = p.P[j];
            }
            else
            {
                dcp = lam_carr[j] * (p.L[j] - Lp[r - 1, s - 1, j]);
                Ps[r - 1, s - 1, j] = p.P[j] / ns + (Ps[r - 1, s - 1, j] + dcp) * (ns - 1) / ns;
            }
            if (++n[r - 1, s - 1, j] < ns)
            {
                p.P[j] = 0.0;
            }
            else
            {
                p.P[j] = Ps[r - 1, s - 1, j];
            }
            Lp[r - 1, s - 1, j] = p.L[j];
        }
    }
}
/* uncompress file -------------------------------------------------------------
* uncompress (uncompress/unzip/uncompact hatanaka-compression/tar) file
* args   : char   *file     I   input file
*          char   *uncfile  O   uncompressed file
* return : status (-1:error,0:not compressed file,1:uncompress completed)
* note   : creates uncompressed file in tempolary directory
*          gzip and crx2rnx commands have to be installed in commands path
*-----------------------------------------------------------------------------*/
public static int uncompress(string file, ref string uncfile)
{
    int stat = 0;
    string p;
    string cmd = "";
    string tmpfile = "";
    string buff = new string(new char[1024]);
    string fname;
    string dir = "";

    GlobalMembersRtkcmn.trace(3, "uncompress: file=%s\n", file);

    tmpfile = file;
    if ((p = StringFunctions.StrRChr(tmpfile, '.')) == 0)
    {
        return 0;
    }

    /* uncompress by gzip */
    if (!string.Compare(p, ".z") || !string.Compare(p, ".Z") || !string.Compare(p, ".gz") || !string.Compare(p, ".GZ") || !string.Compare(p, ".zip") || !string.Compare(p, ".ZIP"))
    {

        uncfile = tmpfile;
        uncfile[p - tmpfile] = '\0';
        cmd = string.Format("gzip -f -d -c \"{0}\" > \"{1}\"", tmpfile, uncfile);

        if (GlobalMembersRtkcmn.execcmd(cmd) != 0)
        {
            remove(uncfile);
            return -1;
        }
        tmpfile = uncfile;
        stat = 1;
    }
    /* extract tar file */
    if ((p = StringFunctions.StrRChr(tmpfile, '.')) != 0 && !string.Compare(p, ".tar"))
    {

        uncfile = tmpfile;
        uncfile[p - tmpfile] = '\0';
        buff = tmpfile;
        fname = buff;
#if WIN32
			if ((p = StringFunctions.StrRChr(buff,'\\')) != 0)
			{
				p = (sbyte)'\0';
				dir = fname;
				fname = p.Substring(1);
			}
			cmd = string.Format("set PATH=%CD%;%PATH% & cd /D \"{0}\" & tar -xf \"{1}\"", dir, fname);
#else
        if ((p = StringFunctions.StrRChr(buff, '/')) != 0)
        {
            p = (sbyte)'\0';
            dir = fname;
            fname = p.Substring(1);
        }
        cmd = string.Format("tar -C \"{0}\" -xf \"{1}\"", dir, tmpfile);
#endif
        if (GlobalMembersRtkcmn.execcmd(cmd) != 0)
        {
            if (stat != 0)
            {
                remove(tmpfile);
            }
            return -1;
        }
        if (stat != 0)
        {
            remove(tmpfile);
        }
        stat = 1;
    }
    /* extract hatanaka-compressed file by cnx2rnx */
    else if ((p = StringFunctions.StrRChr(tmpfile, '.')) != 0 && p.Length > 3 && (*(p.Substring(3)) == 'd' || *(p.Substring(3)) == 'D'))
    {

        uncfile = tmpfile;
        uncfile[p - tmpfile.Substring(3)] = *(p.Substring(3)) == 'D' ? 'O' : 'o';
        cmd = string.Format("crx2rnx < \"{0}\" > \"{1}\"", tmpfile, uncfile);

        if (GlobalMembersRtkcmn.execcmd(cmd) != 0)
        {
            remove(uncfile);
            if (stat != 0)
            {
                remove(tmpfile);
            }
            return -1;
        }
        if (stat != 0)
        {
            remove(tmpfile);
        }
        stat = 1;
    }
    GlobalMembersRtkcmn.trace(3, "uncompress: stat=%d\n", stat);
    return stat;
}
/* dummy application functions for shared library ----------------------------*/
#if DLL
	public static int showmsg(ref string format, params object[] LegacyParamArray)
	{
		return 0;
	}
	public static void settspan(gtime_t ts, gtime_t te)
	{
	}
	public static void settime(gtime_t time)
	{
	}
#endif

/* dummy functions for lex extentions ----------------------------------------*/
#if !EXTLEX
public static int input_lexr(raw_t raw, byte data)
{
    return 0;
}
public static int input_lexrf(raw_t raw, FILE fp)
{
    return 0;
}
public static int gen_lexr(string msg, ref byte buff)
{
    return 0;
}
	#endif
}
}
