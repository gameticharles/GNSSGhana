﻿using ghGPS.Classes.rcv;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ghGPS.Classes
{
    public static class GlobalMembersRtcm
    {
        /*------------------------------------------------------------------------------
        * rtcm.c : rtcm functions
        *
        *          Copyright (C) 2009-2014 by T.TAKASU, All rights reserved.
        *
        * references :
        *     [1] RTCM Recommended Standards for Differential GNSS (Global Navigation
        *         Satellite Systems) Service version 2.3, August 20, 2001
        *     [2] RTCM Standard 10403.1 for Differential GNSS (Global Navigation
        *         Satellite Systems) Services - Version 3, Octobar 27, 2006
        *     [3] RTCM 10403.1-Amendment 3, Amendment 3 to RTCM Standard 10403.1
        *     [4] RTCM Paper, April 12, 2010, Proposed SSR Messages for SV Orbit Clock,
        *         Code Biases, URA
        *     [5] RTCM Paper 012-2009-SC104-528, January 28, 2009 (previous ver of [4])
        *     [6] RTCM Paper 012-2009-SC104-582, February 2, 2010 (previous ver of [4])
        *     [7] RTCM Standard 10403.1 - Amendment 5, Differential GNSS (Global
        *         Navigation Satellite Systems) Services - version 3, July 1, 2011
        *     [8] RTCM Paper 019-2012-SC104-689 (draft Galileo ephmeris messages)
        *     [9] RTCM Paper 163-2012-SC104-725 (draft QZSS ephemeris message)
        *     [10] RTCM Paper 059-2011-SC104-635 (draft Galileo and QZSS ssr messages)
        *     [11] RTCM Paper 034-2012-SC104-693 (draft multiple signal messages)
        *     [12] RTCM Paper 133-2012-SC104-709 (draft QZSS MSM messages)
        *     [13] RTCM Paper 122-2012-SC104-707.r1 (draft MSM messages)
        *     [14] RTCM Standard 10403.2, Differential GNSS (Global Navigation Satellite
        *          Systems) Services - version 3, February 1, 2013
        *     [15] RTCM Standard 10403.2, Differential GNSS (Global Navigation Satellite
        *          Systems) Services - version 3, with amendment 1/2, november 7, 2013
        *     [16] Proposal of new RTCM SSR Messages (ssr_1_gal_qzss_sbas_dbs_v05)
        *          2014/04/17
        *
        * version : $Revision:$ $Date:$
        * history : 2009/04/10 1.0  new
        *           2009/06/29 1.1  support type 1009-1012 to get synchronous-gnss-flag
        *           2009/12/04 1.2  support type 1010,1012,1020
        *           2010/07/15 1.3  support type 1057-1068 for ssr corrections
        *                           support type 1007,1008,1033 for antenna info
        *           2010/09/08 1.4  fix problem of ephemeris and ssr sequence upset
        *                           (2.4.0_p8)
        *           2012/05/11 1.5  comply with RTCM 3 final SSR format (RTCM 3
        *                           Amendment 5) (ref [7]) (2.4.1_p6)
        *           2012/05/14 1.6  separate rtcm2.c, rtcm3.c
        *                           add options to select used codes for msm
        *           2013/04/27 1.7  comply with rtcm 3.2 with amendment 1/2 (ref[15])
        *           2013/12/06 1.8  support SBAS/BeiDou SSR messages (ref[16])
        *-----------------------------------------------------------------------------*/


        internal const string rcsid = "$Id:$";

        /* function prototypes -------------------------------------------------------*/
        //int decode_rtcm2(rtcm_t rtcm);
        //int decode_rtcm3(rtcm_t rtcm);
        //int encode_rtcm3(rtcm_t rtcm, int type, int sync);




        /* initialize rtcm control -----------------------------------------------------
        * initialize rtcm control struct and reallocate memory for observation and
        * ephemeris buffer in rtcm control struct
        * args   : rtcm_t *raw   IO     rtcm control struct
        * return : status (1:ok,0:memory allocation error)
        *-----------------------------------------------------------------------------*/
        public static int init_rtcm(rtcm_t rtcm)
        {
            gtime_t time0 = new gtime_t();
            obsd_t data0 = new obsd_t({ 0 });
            eph_t eph0 = new eph_t(0, -1, -1);
            geph_t geph0 = new geph_t(0, -1);
            ssr_t ssr0 = new ssr_t({ { 0 } });
            int i;
            int j;

            GlobalMembersRtkcmn.trace(3, "init_rtcm:\n");

            rtcm.staid = rtcm.stah = rtcm.seqno = rtcm.outtype = 0;
            rtcm.time = rtcm.time_s = time0;
            rtcm.sta.name[0] = rtcm.sta.marker[0] = '\0';
            rtcm.sta.antdes[0] = rtcm.sta.antsno[0] = '\0';
            rtcm.sta.rectype[0] = rtcm.sta.recver[0] = rtcm.sta.recsno[0] = '\0';
            rtcm.sta.antsetup = rtcm.sta.itrf = rtcm.sta.deltype = 0;
            for (i = 0; i < 3; i++)
            {
                rtcm.sta.pos[i] = rtcm.sta.del[i] = 0.0;
            }
            rtcm.sta.hgt = 0.0;
            rtcm.dgps = null;
            for (i = 0; i < DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1; i++)
            {
                rtcm.ssr[i] = ssr0;
            }
            rtcm.msg[0] = rtcm.msgtype[0] = rtcm.opt[0] = '\0';
            for (i = 0; i < 6; i++)
            {
                rtcm.msmtype[i, 0] = (sbyte)'\0';
            }
            rtcm.obsflag = rtcm.ephsat = 0;
            for (i = 0; i < DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1; i++)
            {
                for (j = 0; j < DefineConstants.NFREQ + DefineConstants.NEXOBS; j++)
                {
                    rtcm.cp[i, j] = 0.0;
                    rtcm.@lock[i, j] = rtcm.loss[i, j] = 0;
                    rtcm.lltime[i, j] = time0;
                }
            }
            rtcm.nbyte = rtcm.nbit = rtcm.len = 0;
            rtcm.word = 0;
            for (i = 0; i < 100; i++)
            {
                rtcm.nmsg2[i] = 0;
            }
            for (i = 0; i < 300; i++)
            {
                rtcm.nmsg3[i] = 0;
            }

            rtcm.obs.data = null;
            rtcm.nav.eph = null;
            rtcm.nav.geph = null;

            /* reallocate memory for observation and ephemris buffer */
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'malloc' has no equivalent in C#:
            if ((rtcm.obs.data = (obsd_t)malloc(sizeof(obsd_t) * DefineConstants.MAXOBS)) == null || (rtcm.nav.eph = (eph_t)malloc(sizeof(eph_t) * DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1)) == null || (rtcm.nav.geph = (geph_t)malloc(sizeof(geph_t) * DefineConstants.MAXPRNGLO)) == null)
            {
                GlobalMembersRtcm.free_rtcm(rtcm);
                return 0;
            }
            rtcm.obs.n = 0;
            rtcm.nav.n = DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1;
            rtcm.nav.ng = DefineConstants.MAXPRNGLO;
            for (i = 0; i < DefineConstants.MAXOBS; i++)
            {
                rtcm.obs.data[i] = data0;
            }
            for (i = 0; i < DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1; i++)
            {
                rtcm.nav.eph[i] = eph0;
            }
            for (i = 0; i < DefineConstants.MAXPRNGLO; i++)
            {
                rtcm.nav.geph[i] = geph0;
            }
            return 1;
        }
        /* free rtcm control ----------------------------------------------------------
        * free observation and ephemris buffer in rtcm control struct
        * args   : rtcm_t *raw   IO     rtcm control struct
        * return : none
        *-----------------------------------------------------------------------------*/
        public static void free_rtcm(rtcm_t rtcm)
        {
            GlobalMembersRtkcmn.trace(3, "free_rtcm:\n");

            /* free memory for observation and ephemeris buffer */
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(rtcm.obs.data);
            rtcm.obs.data = null;
            rtcm.obs.n = 0;
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(rtcm.nav.eph);
            rtcm.nav.eph = null;
            rtcm.nav.n = 0;
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(rtcm.nav.geph);
            rtcm.nav.geph = null;
            rtcm.nav.ng = 0;
        }
        /* input rtcm 2 message from stream --------------------------------------------
        * fetch next rtcm 2 message and input a message from byte stream
        * args   : rtcm_t *rtcm IO   rtcm control struct
        *          unsigned char data I stream data (1 byte)
        * return : status (-1: error message, 0: no message, 1: input observation data,
        *                  2: input ephemeris, 5: input station pos/ant parameters,
        *                  6: input time parameter, 7: input dgps corrections,
        *                  9: input special message)
        * notes  : before firstly calling the function, time in rtcm control struct has
        *          to be set to the approximate time within 1/2 hour in order to resolve
        *          ambiguity of time in rtcm messages.
        *          supported msgs RTCM ver.2: 1,3,9,14,16,17,18,19,22
        *          refer [1] for RTCM ver.2
        *-----------------------------------------------------------------------------*/
        //C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on the parameter 'rtcm', so pointers on this parameter are left unchanged:
        public static int input_rtcm2(rtcm_t* rtcm, byte data)
        {
            byte preamb;
            int i;

            GlobalMembersRtkcmn.trace(5, "input_rtcm2: data=%02x\n", data);

            if ((data & 0xC0) != 0x40) // ignore if upper 2bit != 01
            {
                return 0;
            }

            for (i = 0; i < 6; i++, data >>= 1) // decode 6-of-8 form
            {
                rtcm.word = (rtcm.word << 1) + (data & 1);

                /* synchronize frame */
                if (rtcm.nbyte == 0)
                {
                    preamb = (byte)(rtcm.word >> 22);
                    if ((rtcm.word & 0x40000000) != 0) // decode preamble
                    {
                        preamb ^= 0xFF;
                    }
                    if (preamb != DefineConstants.RTCM2PREAMB)
                        continue;

                    /* check parity */
                    if (GlobalMembersRtkcmn.decode_word(rtcm.word, rtcm.buff) == 0)
                        continue;
                    rtcm.nbyte = 3;
                    rtcm.nbit = 0;
                    continue;
                }
                if (++rtcm.nbit < 30)
                    continue;
                else
                {
                    rtcm.nbit = 0;
                }

                /* check parity */
                if (GlobalMembersRtkcmn.decode_word(rtcm.word, rtcm.buff + rtcm.nbyte) == 0)
                {
                    GlobalMembersRtkcmn.trace(2, "rtcm2 partity error: i=%d word=%08x\n", i, rtcm.word);
                    rtcm.nbyte = 0;
                    rtcm.word &= 0x3;
                    continue;
                }
                rtcm.nbyte += 3;
                if (rtcm.nbyte == 6)
                {
                    rtcm.len = (rtcm.buff[5] >> 3) * 3 + 6;
                }
                if (rtcm.nbyte < rtcm.len)
                    continue;
                rtcm.nbyte = 0;
                rtcm.word &= 0x3;

                /* decode rtcm2 message */
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: return decode_rtcm2(rtcm);
                return GlobalMembersRtcm2.decode_rtcm2(new rtcm_t(rtcm));
            }
            return 0;
        }
        /* input rtcm 3 message from stream --------------------------------------------
        * fetch next rtcm 3 message and input a message from byte stream
        * args   : rtcm_t *rtcm IO   rtcm control struct
        *          unsigned char data I stream data (1 byte)
        * return : status (-1: error message, 0: no message, 1: input observation data,
        *                  2: input ephemeris, 5: input station pos/ant parameters,
        *                  10: input ssr messages)
        * notes  : before firstly calling the function, time in rtcm control struct has
        *          to be set to the approximate time within 1/2 week in order to resolve
        *          ambiguity of time in rtcm messages.
        *          
        *          to specify input options, set rtcm->opt to the following option
        *          strings separated by spaces.
        *
        *          -EPHALL  : input all ephemerides
        *          -STA=nnn : input only message with STAID=nnn
        *          -GLss    : select signal ss for GPS MSM (ss=1C,1P,...)
        *          -RLss    : select signal ss for GLO MSM (ss=1C,1P,...)
        *          -ELss    : select signal ss for GAL MSM (ss=1C,1B,...)
        *          -JLss    : select signal ss for QZS MSM (ss=1C,2C,...)
        *          -CLss    : select signal ss for BDS MSM (ss=2I,7I,...)
        *
        *          supported RTCM 3 messages
        *                  (ref [2][3][4][5][6][7][8][9][10][11][12][13][14][15])
        *
        *            TYPE       GPS     GLOASS    GALILEO    QZSS     BEIDOU     SBAS
        *         ----------------------------------------------------------------------
        *          OBS C-L1  : 1001~     1009~       -         -         -         -
        *              F-L1  : 1002      1010        -         -         -         -
        *              C-L12 : 1003~     1011~       -         -         -         -
        *              F-L12 : 1004      1012        -         -         -         -
        *
        *          NAV       : 1019      1020      1045*     1044*     1047*       -
        *                        -         -       1046*       -         -         -
        *
        *          MSM 1     : 1071~     1081~     1091~     1111*~    1121*~    1101*~
        *              2     : 1072~     1082~     1092~     1112*~    1122*~    1102*~
        *              3     : 1073~     1083~     1093~     1113*~    1123*~    1103*~
        *              4     : 1074      1084      1094      1114*     1124*     1104*
        *              5     : 1075      1085      1095      1115*     1125*     1105*
        *              6     : 1076      1086      1096      1116*     1126*     1106*
        *              7     : 1077      1087      1097      1117*     1127*     1107*
        *
        *          SSR OBT   : 1057      1063      1240*     1246*     1258*       -
        *              CLK   : 1058      1064      1241*     1247*     1259*       -
        *              BIAS  : 1059      1065      1242*     1248*     1260*       -
        *              OBTCLK: 1060      1066      1243*     1249*     1261*       -
        *              URA   : 1061      1067      1244*     1250*     1262*       -
        *              HRCLK : 1062      1068      1245*     1251*     1263*       -
        *
        *          ANT INFO  : 1005 1006 1007 1008 1033
        *         ----------------------------------------------------------------------
        *                                                    (* draft, ~ only encode)
        *
        *          for MSM observation data with multiple signals for a frequency,
        *          a signal is selected according to internal priority. to select
        *          a specified signal, use the input options.
        *
        *          rtcm3 message format:
        *            +----------+--------+-----------+--------------------+----------+
        *            | preamble | 000000 |  length   |    data message    |  parity  |
        *            +----------+--------+-----------+--------------------+----------+
        *            |<-- 8 --->|<- 6 -->|<-- 10 --->|<--- length x 8 --->|<-- 24 -->|
        *            
        *-----------------------------------------------------------------------------*/
        public static int input_rtcm3(rtcm_t rtcm, byte data)
        {
            GlobalMembersRtkcmn.trace(5, "input_rtcm3: data=%02x\n", data);

            /* synchronize frame */
            if (rtcm.nbyte == 0)
            {
                if (data != DefineConstants.RTCM3PREAMB)
                {
                    return 0;
                }
                rtcm.buff[rtcm.nbyte++] = data;
                return 0;
            }
            rtcm.buff[rtcm.nbyte++] = data;

            if (rtcm.nbyte == 3)
            {
                rtcm.len = GlobalMembersRtkcmn.getbitu(rtcm.buff, 14, 10) + 3; // length without parity
            }
            if (rtcm.nbyte < 3 || rtcm.nbyte < rtcm.len + 3)
            {
                return 0;
            }
            rtcm.nbyte = 0;

            /* check parity */
            if (GlobalMembersRtkcmn.crc24q(rtcm.buff, rtcm.len) != GlobalMembersRtkcmn.getbitu(rtcm.buff, rtcm.len * 8, 24))
            {
                GlobalMembersRtkcmn.trace(2, "rtcm3 parity error: len=%d\n", rtcm.len);
                return 0;
            }
            /* decode rtcm3 message */
            return GlobalMembersRtcm3.decode_rtcm3(rtcm);
        }
        /* input rtcm 2 message from file ----------------------------------------------
        * fetch next rtcm 2 message and input a messsage from file
        * args   : rtcm_t *rtcm IO   rtcm control struct
        *          FILE  *fp    I    file pointer
        * return : status (-2: end of file, -1...10: same as above)
        * notes  : same as above
        *-----------------------------------------------------------------------------*/
        public static int input_rtcm2f(rtcm_t rtcm, FILE fp)
        {
            int i;
            int data = 0;
            int ret;

            GlobalMembersRtkcmn.trace(4, "input_rtcm2f: data=%02x\n", data);

            for (i = 0; i < 4096; i++)
            {
                if ((data = fgetc(fp)) == EOF)
                {
                    return -2;
                }
                if ((ret = GlobalMembersRtcm.input_rtcm2(rtcm, (byte)data)) != 0)
                {
                    return ret;
                }
            }
            return 0; // return at every 4k bytes
        }
        /* input rtcm 3 message from file ----------------------------------------------
        * fetch next rtcm 3 message and input a messsage from file
        * args   : rtcm_t *rtcm IO   rtcm control struct
        *          FILE  *fp    I    file pointer
        * return : status (-2: end of file, -1...10: same as above)
        * notes  : same as above
        *-----------------------------------------------------------------------------*/
        public static int input_rtcm3f(rtcm_t rtcm, FILE fp)
        {
            int i;
            int data = 0;
            int ret;

            GlobalMembersRtkcmn.trace(4, "input_rtcm3f: data=%02x\n", data);

            for (i = 0; i < 4096; i++)
            {
                if ((data = fgetc(fp)) == EOF)
                {
                    return -2;
                }
                if ((ret = GlobalMembersRtcm.input_rtcm3(rtcm, (byte)data)) != 0)
                {
                    return ret;
                }
            }
            return 0; // return at every 4k bytes
        }
        /* generate rtcm 2 message -----------------------------------------------------
        * generate rtcm 2 message
        * args   : rtcm_t *rtcm   IO rtcm control struct
        *          int    type    I  message type
        *          int    sync    I  sync flag (1:another message follows)
        * return : status (1:ok,0:error)
        *-----------------------------------------------------------------------------*/
        public static int gen_rtcm2(rtcm_t rtcm, int type, int sync)
        {
            GlobalMembersRtkcmn.trace(4, "gen_rtcm2: type=%d sync=%d\n", type, sync);

            rtcm.nbit = rtcm.len = rtcm.nbyte = 0;

            /* not yet implemented */

            return 0;
        }
        /* generate rtcm 3 message -----------------------------------------------------
        * generate rtcm 3 message
        * args   : rtcm_t *rtcm   IO rtcm control struct
        *          int    type    I  message type
        *          int    sync    I  sync flag (1:another message follows)
        * return : status (1:ok,0:error)
        *-----------------------------------------------------------------------------*/
        public static int gen_rtcm3(rtcm_t rtcm, int type, int sync)
        {
            uint crc;
            int i = 0;

            GlobalMembersRtkcmn.trace(4, "gen_rtcm3: type=%d sync=%d\n", type, sync);

            rtcm.nbit = rtcm.len = rtcm.nbyte = 0;

            /* set preamble and reserved */
            GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 8, DefineConstants.RTCM3PREAMB);
            i += 8;
            GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 6, 0);
            i += 6;
            GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 10, 0);
            i += 10;

            /* encode rtcm 3 message body */
            if (GlobalMembersRtcm3e.encode_rtcm3(rtcm, type, sync) == 0)
            {
                return 0;
            }

            /* padding to align 8 bit boundary */
            for (i = rtcm.nbit; i % 8; i++)
            {
                GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 1, 0);
            }
            /* message length (header+data) (bytes) */
            if ((rtcm.len = i / 8) >= 3 + 1024)
            {
                GlobalMembersRtkcmn.trace(2, "generate rtcm 3 message length error len=%d\n", rtcm.len - 3);
                rtcm.nbit = rtcm.len = 0;
                return 0;
            }
            /* message length without header and parity */
            GlobalMembersRtkcmn.setbitu(rtcm.buff, 14, 10, rtcm.len - 3);

            /* crc-24q */
            crc = GlobalMembersRtkcmn.crc24q(rtcm.buff, rtcm.len);
            GlobalMembersRtkcmn.setbitu(rtcm.buff, i, 24, crc);

            /* length total (bytes) */
            rtcm.nbyte = rtcm.len + 3;

            return 1;
        }
    }
}
