using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ghGPS.Classes
{
    public static class GlobalMembersRcvraw
    {
        /*------------------------------------------------------------------------------
        * rcvraw.c : receiver raw data functions
        *
        *          Copyright (C) 2009-2014 by T.TAKASU, All rights reserved.
        *          Copyright (C) 2014 by T.SUZUKI, All rights reserved.
        *
        * references :
        *     [1] IS-GPS-200D, Navstar GPS Space Segment/Navigation User Interfaces,
        *         7 March, 2006
        *     [2] Global navigation satellite system GLONASS interface control document
        *         navigation radiosignal in bands L1,L2 (version 5.1), 2008
        *     [3] BeiDou satellite navigation system signal in space interface control
        *         document open service signal (version 2.0), December 2013
        *     [4] Quasi-Zenith Satellite System Navigation Service Interface
        *         Specification for QZSS (IS-QZSS) V.1.5, March 27, 2014
        *
        * version : $Revision: 1.1 $ $Date: 2008/07/17 21:48:06 $
        * history : 2009/04/10 1.0  new
        *           2009/06/02 1.1  support glonass
        *           2010/07/31 1.2  support eph_t struct change
        *           2010/12/06 1.3  add almanac decoding, support of GW10
        *                           change api decode_frame()
        *           2013/04/11 1.4  fix bug on decode fit interval
        *           2014/01/31 1.5  fix bug on decode fit interval
        *           2014/06/22 1.6  add api decode_glostr()
        *           2014/06/22 1.7  add api decode_bds_d1(), decode_bds_d2()
        *           2014/08/14 1.8  add test_glostr()
        *                           add support input format rt17
        *           2014/08/31 1.9  suppress warning
        *           2014/11/07 1.10 support qzss navigation subframes
        *-----------------------------------------------------------------------------*/


        internal const string rcsid = "$Id:$";



        /* get two component bits ----------------------------------------------------*/
        internal static uint getbitu2(byte buff, int p1, int l1, int p2, int l2)
        {
            return (GlobalMembersRtkcmn.getbitu(buff, p1, l1) << l2) + GlobalMembersRtkcmn.getbitu(buff, p2, l2);
        }
        internal static int getbits2(byte buff, int p1, int l1, int p2, int l2)
        {
            if (GlobalMembersRtkcmn.getbitu(buff, p1, 1) != 0)
            {
                return (int)((GlobalMembersRtkcmn.getbits(buff, p1, l1) << l2) + GlobalMembersRtkcmn.getbitu(buff, p2, l2));
            }
            else
            {
                return (int)GlobalMembersRcvraw.getbitu2(buff, p1, l1, p2, l2);
            }
        }
        /* get three component bits --------------------------------------------------*/
        internal static uint getbitu3(byte buff, int p1, int l1, int p2, int l2, int p3, int l3)
        {
            return (GlobalMembersRtkcmn.getbitu(buff, p1, l1) << (l2 + l3)) + (GlobalMembersRtkcmn.getbitu(buff, p2, l2) << l3) + GlobalMembersRtkcmn.getbitu(buff, p3, l3);
        }
        internal static int getbits3(byte buff, int p1, int l1, int p2, int l2, int p3, int l3)
        {
            if (GlobalMembersRtkcmn.getbitu(buff, p1, 1) != 0)
            {
                return (int)((GlobalMembersRtkcmn.getbits(buff, p1, l1) << (l2 + l3)) + (GlobalMembersRtkcmn.getbitu(buff, p2, l2) << l3) + GlobalMembersRtkcmn.getbitu(buff, p3, l3));
            }
            else
            {
                return (int)GlobalMembersRcvraw.getbitu3(buff, p1, l1, p2, l2, p3, l3);
            }
        }
        /* merge two components ------------------------------------------------------*/
        internal static uint merge_two_u(uint a, uint b, int n)
        {
            return (a << n) + b;
        }
        internal static int merge_two_s(int a, uint b, int n)
        {
            return (int)((a << n) + b);
        }
        /* get sign-magnitude bits ---------------------------------------------------*/
        internal static double getbitg(byte buff, int pos, int len)
        {
            double value = GlobalMembersRtkcmn.getbitu(buff, pos + 1, len - 1);
            return GlobalMembersRtkcmn.getbitu(buff, pos, 1) != 0 ? -value : value;
        }
        /* decode BeiDou D1 ephemeris --------------------------------------------------
        * decode BeiDou D1 ephemeris (IGSO/MEO satellites) (ref [3] 5.2)
        * args   : unsigned char *buff I beidou D1 subframe bits
        *                                  buff[ 0- 37]: subframe 1 (300 bits)
        *                                  buff[38- 75]: subframe 2
        *                                  buff[76-113]: subframe 3
        *          eph_t    *eph    IO  ephemeris structure
        * return : status (1:ok,0:error)
        *-----------------------------------------------------------------------------*/
        public static int decode_bds_d1(byte buff, eph_t eph)
        {
            double toc_bds;
            double sqrtA;
            uint toe1;
            uint toe2;
            uint sow1;
            uint sow2;
            uint sow3;
            int i;
            int frn1;
            int frn2;
            int frn3;

            GlobalMembersRtkcmn.trace(3, "decode_bds_d1:\n");

            i = 8 * 38 * 0; // subframe 1
            frn1 = GlobalMembersRtkcmn.getbitu(buff, i + 15, 3);
            sow1 = GlobalMembersRcvraw.getbitu2(buff, i + 18, 8, i + 30, 12);
            eph.svh = GlobalMembersRtkcmn.getbitu(buff, i + 42, 1); // SatH1
            eph.iodc = GlobalMembersRtkcmn.getbitu(buff, i + 43, 5); // AODC
            eph.sva = GlobalMembersRtkcmn.getbitu(buff, i + 48, 4);
            eph.week = GlobalMembersRtkcmn.getbitu(buff, i + 60, 13); // week in BDT
            toc_bds = GlobalMembersRcvraw.getbitu2(buff, i + 73, 9, i + 90, 8) * 8.0;
            eph.tgd[0] = GlobalMembersRtkcmn.getbits(buff, i + 98, 10) * 0.1 * 1E-9;
            eph.tgd[1] = GlobalMembersRcvraw.getbits2(buff, i + 108, 4, i + 120, 6) * 0.1 * 1E-9;
            eph.f2 = GlobalMembersRtkcmn.getbits(buff, i + 214, 11) * DefineConstants.P2_66;
            eph.f0 = GlobalMembersRcvraw.getbits2(buff, i + 225, 7, i + 240, 17) * DefineConstants.P2_33;
            eph.f1 = GlobalMembersRcvraw.getbits2(buff, i + 257, 5, i + 270, 17) * DefineConstants.P2_50;
            eph.iode = GlobalMembersRtkcmn.getbitu(buff, i + 287, 5); // AODE

            i = 8 * 38 * 1; // subframe 2
            frn2 = GlobalMembersRtkcmn.getbitu(buff, i + 15, 3);
            sow2 = GlobalMembersRcvraw.getbitu2(buff, i + 18, 8, i + 30, 12);
            eph.deln = GlobalMembersRcvraw.getbits2(buff, i + 42, 10, i + 60, 6) * DefineConstants.P2_43 * DefineConstants.SC2RAD;
            eph.cuc = GlobalMembersRcvraw.getbits2(buff, i + 66, 16, i + 90, 2) * DefineConstants.P2_31;
            eph.M0 = GlobalMembersRcvraw.getbits2(buff, i + 92, 20, i + 120, 12) * DefineConstants.P2_31 * DefineConstants.SC2RAD;
            eph.e = GlobalMembersRcvraw.getbitu2(buff, i + 132, 10, i + 150, 22) * DefineConstants.P2_33;
            eph.cus = GlobalMembersRtkcmn.getbits(buff, i + 180, 18) * DefineConstants.P2_31;
            eph.crc = GlobalMembersRcvraw.getbits2(buff, i + 198, 4, i + 210, 14) * DefineConstants.P2_6;
            eph.crs = GlobalMembersRcvraw.getbits2(buff, i + 224, 8, i + 240, 10) * DefineConstants.P2_6;
            sqrtA = GlobalMembersRcvraw.getbitu2(buff, i + 250, 12, i + 270, 20) * 1.907348632812500E-0x6;
            toe1 = GlobalMembersRtkcmn.getbitu(buff, i + 290, 2); // TOE 2-MSB
            eph.A = sqrtA * sqrtA;

            i = 8 * 38 * 2; // subframe 3
            frn3 = GlobalMembersRtkcmn.getbitu(buff, i + 15, 3);
            sow3 = GlobalMembersRcvraw.getbitu2(buff, i + 18, 8, i + 30, 12);
            toe2 = GlobalMembersRcvraw.getbitu2(buff, i + 42, 10, i + 60, 5); // TOE 5-LSB
            eph.i0 = GlobalMembersRcvraw.getbits2(buff, i + 65, 17, i + 90, 15) * DefineConstants.P2_31 * DefineConstants.SC2RAD;
            eph.cic = GlobalMembersRcvraw.getbits2(buff, i + 105, 7, i + 120, 11) * DefineConstants.P2_31;
            eph.OMGd = GlobalMembersRcvraw.getbits2(buff, i + 131, 11, i + 150, 13) * DefineConstants.P2_43 * DefineConstants.SC2RAD;
            eph.cis = GlobalMembersRcvraw.getbits2(buff, i + 163, 9, i + 180, 9) * DefineConstants.P2_31;
            eph.idot = GlobalMembersRcvraw.getbits2(buff, i + 189, 13, i + 210, 1) * DefineConstants.P2_43 * DefineConstants.SC2RAD;
            eph.OMG0 = GlobalMembersRcvraw.getbits2(buff, i + 211, 21, i + 240, 11) * DefineConstants.P2_31 * DefineConstants.SC2RAD;
            eph.omg = GlobalMembersRcvraw.getbits2(buff, i + 251, 11, i + 270, 21) * DefineConstants.P2_31 * DefineConstants.SC2RAD;
            eph.toes = GlobalMembersRcvraw.merge_two_u(toe1, toe2, 15) * 8.0;

            /* check consistency of subframe numbers, sows and toe/toc */
            if (frn1 != 1 || frn2 != 2 || frn3 != 3)
            {
                GlobalMembersRtkcmn.trace(3, "decode_bds_d1 error: frn=%d %d %d\n", frn1, frn2, frn3);
                return 0;
            }
            if (sow2 != sow1 + 6 || sow3 != sow2 + 6)
            {
                GlobalMembersRtkcmn.trace(3, "decode_bds_d1 error: sow=%d %d %d\n", sow1, sow2, sow3);
                return 0;
            }
            if (toc_bds != eph.toes)
            {
                GlobalMembersRtkcmn.trace(3, "decode_bds_d1 error: toe=%.0f toc=%.0f\n", eph.toes, toc_bds);
                return 0;
            }
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: eph->ttr=bdt2gpst(bdt2time(eph->week,sow1));
            eph.ttr.CopyFrom(GlobalMembersRtkcmn.bdt2gpst(GlobalMembersRtkcmn.bdt2time(eph.week, sow1))); // bdt -> gpst
            if (eph.toes > sow1 + 302400.0)
            {
                eph.week++;
            }
            else if (eph.toes < sow1 - 302400.0)
            {
                eph.week--;
            }
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: eph->toe=bdt2gpst(bdt2time(eph->week,eph->toes));
            eph.toe.CopyFrom(GlobalMembersRtkcmn.bdt2gpst(GlobalMembersRtkcmn.bdt2time(eph.week, eph.toes))); // bdt -> gpst
                                                                                                              //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                                                                                                              //ORIGINAL LINE: eph->toc=bdt2gpst(bdt2time(eph->week,toc_bds));
            eph.toc.CopyFrom(GlobalMembersRtkcmn.bdt2gpst(GlobalMembersRtkcmn.bdt2time(eph.week, toc_bds))); // bdt -> gpst
            return 1;
        }
        /* decode BeiDou D2 ephemeris --------------------------------------------------
        * decode BeiDou D2 ephemeris (GEO satellites) (ref [3] 5.3)
        * args   : unsigned char *buff I beidou D2 subframe 1 page bits
        *                                  buff[  0- 37]: page 1 (300 bits)
        *                                  buff[ 38- 75]: page 2
        *                                  ...
        *                                  buff[342-379]: page 10
        *          eph_t    *eph    IO  ephemeris structure
        * return : status (1:ok,0:error)
        *-----------------------------------------------------------------------------*/
        public static int decode_bds_d2(byte buff, eph_t eph)
        {
            double toc_bds;
            double sqrtA;
            uint f1p4;
            uint cucp5;
            uint ep6;
            uint cicp7;
            uint i0p8;
            uint OMGdp9;
            uint omgp10;
            uint sow1;
            uint sow3;
            uint sow4;
            uint sow5;
            uint sow6;
            uint sow7;
            uint sow8;
            uint sow9;
            uint sow10;
            int i;
            int f1p3;
            int cucp4;
            int ep5;
            int cicp6;
            int i0p7;
            int OMGdp8;
            int omgp9;
            int pgn1;
            int pgn3;
            int pgn4;
            int pgn5;
            int pgn6;
            int pgn7;
            int pgn8;
            int pgn9;
            int pgn10;

            GlobalMembersRtkcmn.trace(3, "decode_bds_d2:\n");

            i = 8 * 38 * 0; // page 1
            pgn1 = GlobalMembersRtkcmn.getbitu(buff, i + 42, 4);
            sow1 = GlobalMembersRcvraw.getbitu2(buff, i + 18, 8, i + 30, 12);
            eph.svh = GlobalMembersRtkcmn.getbitu(buff, i + 46, 1); // SatH1
            eph.iodc = GlobalMembersRtkcmn.getbitu(buff, i + 47, 5); // AODC
            eph.sva = GlobalMembersRtkcmn.getbitu(buff, i + 60, 4);
            eph.week = GlobalMembersRtkcmn.getbitu(buff, i + 64, 13); // week in BDT
            toc_bds = GlobalMembersRcvraw.getbitu2(buff, i + 77, 5, i + 90, 12) * 8.0;
            eph.tgd[0] = GlobalMembersRtkcmn.getbits(buff, i + 102, 10) * 0.1 * 1E-9;
            eph.tgd[1] = GlobalMembersRtkcmn.getbits(buff, i + 120, 10) * 0.1 * 1E-9;

            i = 8 * 38 * 2; // page 3
            pgn3 = GlobalMembersRtkcmn.getbitu(buff, i + 42, 4);
            sow3 = GlobalMembersRcvraw.getbitu2(buff, i + 18, 8, i + 30, 12);
            eph.f0 = GlobalMembersRcvraw.getbits2(buff, i + 100, 12, i + 120, 12) * DefineConstants.P2_33;
            f1p3 = GlobalMembersRtkcmn.getbits(buff, i + 132, 4);

            i = 8 * 38 * 3; // page 4
            pgn4 = GlobalMembersRtkcmn.getbitu(buff, i + 42, 4);
            sow4 = GlobalMembersRcvraw.getbitu2(buff, i + 18, 8, i + 30, 12);
            f1p4 = GlobalMembersRcvraw.getbitu2(buff, i + 46, 6, i + 60, 12);
            eph.f2 = GlobalMembersRcvraw.getbits2(buff, i + 72, 10, i + 90, 1) * DefineConstants.P2_66;
            eph.iode = GlobalMembersRtkcmn.getbitu(buff, i + 91, 5); // AODE
            eph.deln = GlobalMembersRtkcmn.getbits(buff, i + 96, 16) * DefineConstants.P2_43 * DefineConstants.SC2RAD;
            cucp4 = GlobalMembersRtkcmn.getbits(buff, i + 120, 14);

            i = 8 * 38 * 4; // page 5
            pgn5 = GlobalMembersRtkcmn.getbitu(buff, i + 42, 4);
            sow5 = GlobalMembersRcvraw.getbitu2(buff, i + 18, 8, i + 30, 12);
            cucp5 = GlobalMembersRtkcmn.getbitu(buff, i + 46, 4);
            eph.M0 = GlobalMembersRcvraw.getbits3(buff, i + 50, 2, i + 60, 22, i + 90, 8) * DefineConstants.P2_31 * DefineConstants.SC2RAD;
            eph.cus = GlobalMembersRcvraw.getbits2(buff, i + 98, 14, i + 120, 4) * DefineConstants.P2_31;
            ep5 = GlobalMembersRtkcmn.getbits(buff, i + 124, 10);

            i = 8 * 38 * 5; // page 6
            pgn6 = GlobalMembersRtkcmn.getbitu(buff, i + 42, 4);
            sow6 = GlobalMembersRcvraw.getbitu2(buff, i + 18, 8, i + 30, 12);
            ep6 = GlobalMembersRcvraw.getbitu2(buff, i + 46, 6, i + 60, 16);
            sqrtA = GlobalMembersRcvraw.getbitu3(buff, i + 76, 6, i + 90, 22, i + 120, 4) * 1.907348632812500E-0x6;
            cicp6 = GlobalMembersRtkcmn.getbits(buff, i + 124, 10);
            eph.A = sqrtA * sqrtA;

            i = 8 * 38 * 6; // page 7
            pgn7 = GlobalMembersRtkcmn.getbitu(buff, i + 42, 4);
            sow7 = GlobalMembersRcvraw.getbitu2(buff, i + 18, 8, i + 30, 12);
            cicp7 = GlobalMembersRcvraw.getbitu2(buff, i + 46, 6, i + 60, 2);
            eph.cis = GlobalMembersRtkcmn.getbits(buff, i + 62, 18) * DefineConstants.P2_31;
            eph.toes = GlobalMembersRcvraw.getbitu2(buff, i + 80, 2, i + 90, 15) * 8.0;
            i0p7 = GlobalMembersRcvraw.getbits2(buff, i + 105, 7, i + 120, 14);

            i = 8 * 38 * 7; // page 8
            pgn8 = GlobalMembersRtkcmn.getbitu(buff, i + 42, 4);
            sow8 = GlobalMembersRcvraw.getbitu2(buff, i + 18, 8, i + 30, 12);
            i0p8 = GlobalMembersRcvraw.getbitu2(buff, i + 46, 6, i + 60, 5);
            eph.crc = GlobalMembersRcvraw.getbits2(buff, i + 65, 17, i + 90, 1) * DefineConstants.P2_6;
            eph.crs = GlobalMembersRtkcmn.getbits(buff, i + 91, 18) * DefineConstants.P2_6;
            OMGdp8 = GlobalMembersRcvraw.getbits2(buff, i + 109, 3, i + 120, 16);

            i = 8 * 38 * 8; // page 9
            pgn9 = GlobalMembersRtkcmn.getbitu(buff, i + 42, 4);
            sow9 = GlobalMembersRcvraw.getbitu2(buff, i + 18, 8, i + 30, 12);
            OMGdp9 = GlobalMembersRtkcmn.getbitu(buff, i + 46, 5);
            eph.OMG0 = GlobalMembersRcvraw.getbits3(buff, i + 51, 1, i + 60, 22, i + 90, 9) * DefineConstants.P2_31 * DefineConstants.SC2RAD;
            omgp9 = GlobalMembersRcvraw.getbits2(buff, i + 99, 13, i + 120, 14);

            i = 8 * 38 * 9; // page 10
            pgn10 = GlobalMembersRtkcmn.getbitu(buff, i + 42, 4);
            sow10 = GlobalMembersRcvraw.getbitu2(buff, i + 18, 8, i + 30, 12);
            omgp10 = GlobalMembersRtkcmn.getbitu(buff, i + 46, 5);
            eph.idot = GlobalMembersRcvraw.getbits2(buff, i + 51, 1, i + 60, 13) * DefineConstants.P2_43 * DefineConstants.SC2RAD;

            /* check consistency of page numbers, sows and toe/toc */
            if (pgn1 != 1 || pgn3 != 3 || pgn4 != 4 || pgn5 != 5 || pgn6 != 6 || pgn7 != 7 || pgn8 != 8 || pgn9 != 9 || pgn10 != 10)
            {
                GlobalMembersRtkcmn.trace(3, "decode_bds_d2 error: pgn=%d %d %d %d %d %d %d %d %d\n", pgn1, pgn3, pgn4, pgn5, pgn6, pgn7, pgn8, pgn9, pgn10);
                return 0;
            }
            if (sow3 != sow1 + 6 || sow4 != sow3 + 3 || sow5 != sow4 + 3 || sow6 != sow5 + 3 || sow7 != sow6 + 3 || sow8 != sow7 + 3 || sow9 != sow8 + 3 || sow10 != sow9 + 3)
            {
                GlobalMembersRtkcmn.trace(3, "decode_bds_d2 error: sow=%d %d %d %d %d %d %d %d %d\n", sow1, sow3, sow4, sow5, sow6, sow7, sow8, sow9, sow10);
                return 0;
            }
            if (toc_bds != eph.toes)
            {
                GlobalMembersRtkcmn.trace(3, "decode_bds_d2 error: toe=%.0f toc=%.0f\n", eph.toes, toc_bds);
                return 0;
            }
            eph.f1 = GlobalMembersRcvraw.merge_two_s(f1p3, f1p4, 18) * DefineConstants.P2_50;
            eph.cuc = GlobalMembersRcvraw.merge_two_s(cucp4, cucp5, 4) * DefineConstants.P2_31;
            eph.e = GlobalMembersRcvraw.merge_two_s(ep5, ep6, 22) * DefineConstants.P2_33;
            eph.cic = GlobalMembersRcvraw.merge_two_s(cicp6, cicp7, 8) * DefineConstants.P2_31;
            eph.i0 = GlobalMembersRcvraw.merge_two_s(i0p7, i0p8, 11) * DefineConstants.P2_31 * DefineConstants.SC2RAD;
            eph.OMGd = GlobalMembersRcvraw.merge_two_s(OMGdp8, OMGdp9, 5) * DefineConstants.P2_43 * DefineConstants.SC2RAD;
            eph.omg = GlobalMembersRcvraw.merge_two_s(omgp9, omgp10, 5) * DefineConstants.P2_31 * DefineConstants.SC2RAD;

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: eph->ttr=bdt2gpst(bdt2time(eph->week,sow1));
            eph.ttr.CopyFrom(GlobalMembersRtkcmn.bdt2gpst(GlobalMembersRtkcmn.bdt2time(eph.week, sow1))); // bdt -> gpst
            if (eph.toes > sow1 + 302400.0)
            {
                eph.week++;
            }
            else if (eph.toes < sow1 - 302400.0)
            {
                eph.week--;
            }
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: eph->toe=bdt2gpst(bdt2time(eph->week,eph->toes));
            eph.toe.CopyFrom(GlobalMembersRtkcmn.bdt2gpst(GlobalMembersRtkcmn.bdt2time(eph.week, eph.toes))); // bdt -> gpst
                                                                                                              //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                                                                                                              //ORIGINAL LINE: eph->toc=bdt2gpst(bdt2time(eph->week,toc_bds));
            eph.toc.CopyFrom(GlobalMembersRtkcmn.bdt2gpst(GlobalMembersRtkcmn.bdt2time(eph.week, toc_bds))); // bdt -> gpst
            return 1;
        }
        /* test hamming code of glonass ephemeris string -------------------------------
        * test hamming code of glonass ephemeris string (ref [2] 4.7)
        * args   : unsigned char *buff I glonass navigation data string bits in frame
        *                                with hamming
        *                                  buff[ 0]: string bit 85-78
        *                                  buff[ 1]: string bit 77-70
        *                                  ...
        *                                  buff[10]: string bit  5- 1 (0 padded)
        * return : status (1:ok,0:error)
        *-----------------------------------------------------------------------------*/
        public static int test_glostr(byte[] buff)
        {
            byte[] xor_8bit = { 0, 1, 1, 0, 1, 0, 0, 1, 1, 0, 0, 1, 0, 1, 1, 0, 1, 0, 0, 1, 0, 1, 1, 0, 0, 1, 1, 0, 1, 0, 0, 1, 1, 0, 0, 1, 0, 1, 1, 0, 0, 1, 1, 0, 1, 0, 0, 1, 0, 1, 1, 0, 1, 0, 0, 1, 1, 0, 0, 1, 0, 1, 1, 0, 1, 0, 0, 1, 0, 1, 1, 0, 0, 1, 1, 0, 1, 0, 0, 1, 0, 1, 1, 0, 1, 0, 0, 1, 1, 0, 0, 1, 0, 1, 1, 0, 0, 1, 1, 0, 1, 0, 0, 1, 1, 0, 0, 1, 0, 1, 1, 0, 1, 0, 0, 1, 0, 1, 1, 0, 0, 1, 1, 0, 1, 0, 0, 1, 1, 0, 0, 1, 0, 1, 1, 0, 0, 1, 1, 0, 1, 0, 0, 1, 0, 1, 1, 0, 1, 0, 0, 1, 1, 0, 0, 1, 0, 1, 1, 0, 0, 1, 1, 0, 1, 0, 0, 1, 1, 0, 0, 1, 0, 1, 1, 0, 1, 0, 0, 1, 0, 1, 1, 0, 0, 1, 1, 0, 1, 0, 0, 1, 0, 1, 1, 0, 1, 0, 0, 1, 1, 0, 0, 1, 0, 1, 1, 0, 1, 0, 0, 1, 0, 1, 1, 0, 0, 1, 1, 0, 1, 0, 0, 1, 1, 0, 0, 1, 0, 1, 1, 0, 0, 1, 1, 0, 1, 0, 0, 1, 0, 1, 1, 0, 1, 0, 0, 1, 1, 0, 0, 1, 0, 1, 1, 0 }; // xor of 8 bits
            byte[,] mask_hamming = { { 0x55, 0x55, 0x5A, 0xAA, 0xAA, 0xAA, 0xB5, 0x55, 0x6A, 0xD8, 0x08, 0 }, { 0x66, 0x66, 0x6C, 0xCC, 0xCC, 0xCC, 0xD9, 0x99, 0xB3, 0x68, 0x10, 0 }, { 0x87, 0x87, 0x8F, 0x0F, 0x0F, 0x0F, 0x1E, 0x1E, 0x3C, 0x70, 0x20, 0 }, { 0x07, 0xF8, 0x0F, 0xF0, 0x0F, 0xF0, 0x1F, 0xE0, 0x3F, 0x80, 0x40, 0 }, { 0xF8, 0x00, 0x0F, 0xFF, 0xF0, 0x00, 0x1F, 0xFF, 0xC0, 0x00, 0x80, 0 }, { 0x00, 0x00, 0x0F, 0xFF, 0xFF, 0xFF, 0xE0, 0x00, 0x00, 0x01, 0x00, 0 }, { 0xFF, 0xFF, 0xF0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0x00, 0 }, { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xF8, 0 } }; // mask of hamming codes
            byte cs;
            int i;
            int j;
            int n = 0;

            for (i = 0; i < 8; i++)
            {
                for (j = 0, cs = 0; j < 11; j++)
                {
                    cs ^= xor_8bit[buff[j] & mask_hamming[i, j]];
                }
                if (cs != 0)
                {
                    n++;
                }
            }
            return n == 0 || (n == 2 && cs != 0);
        }
        /* decode glonass ephemeris strings --------------------------------------------
        * decode glonass ephemeris string (ref [2])
        * args   : unsigned char *buff I glonass navigation data string bits in frames
        *                                (without hamming and time mark)
        *                                  buff[ 0- 9]: string #1 (77 bits)
        *                                  buff[10-19]: string #2
        *                                  buff[20-29]: string #3
        *                                  buff[30-39]: string #4
        *          geph_t *geph  IO     glonass ephemeris message
        * return : status (1:ok,0:error)
        * notes  : geph->tof should be set to frame time witin 1/2 day before calling
        *          geph->frq is set to 0
        *-----------------------------------------------------------------------------*/
        public static int decode_glostr(byte buff, geph_t geph)
        {
            double tow;
            double tod;
            double tof;
            double toe;
            int P;
            int P1;
            int P2;
            int P3;
            int P4;
            int tk_h;
            int tk_m;
            int tk_s;
            int tb;
            int ln;
            int NT;
            int slot;
            int M;
            int week;
            int i = 1;
            int frn1;
            int frn2;
            int frn3;
            int frn4;

            GlobalMembersRtkcmn.trace(3, "decode_glostr:\n");

            /* frame 1 */
            frn1 = GlobalMembersRtkcmn.getbitu(buff, i, 4);
            i += 4 + 2;
            P1 = GlobalMembersRtkcmn.getbitu(buff, i, 2);
            i += 2;
            tk_h = GlobalMembersRtkcmn.getbitu(buff, i, 5);
            i += 5;
            tk_m = GlobalMembersRtkcmn.getbitu(buff, i, 6);
            i += 6;
            tk_s = GlobalMembersRtkcmn.getbitu(buff, i, 1) * 30;
            i += 1;
            geph.vel[0] = GlobalMembersRcvraw.getbitg(buff, i, 24) * 9.536743164062500E-0x7 * 1E3;
            i += 24;
            geph.acc[0] = GlobalMembersRcvraw.getbitg(buff, i, 5) * DefineConstants.P2_30 * 1E3;
            i += 5;
            geph.pos[0] = GlobalMembersRcvraw.getbitg(buff, i, 27) * 4.882812500000000E-0x4 * 1E3;
            i += 27 + 4;

            /* frame 2 */
            frn2 = GlobalMembersRtkcmn.getbitu(buff, i, 4);
            i += 4;
            geph.svh = GlobalMembersRtkcmn.getbitu(buff, i, 3);
            i += 3;
            P2 = GlobalMembersRtkcmn.getbitu(buff, i, 1);
            i += 1;
            tb = GlobalMembersRtkcmn.getbitu(buff, i, 7);
            i += 7 + 5;
            geph.vel[1] = GlobalMembersRcvraw.getbitg(buff, i, 24) * 9.536743164062500E-0x7 * 1E3;
            i += 24;
            geph.acc[1] = GlobalMembersRcvraw.getbitg(buff, i, 5) * DefineConstants.P2_30 * 1E3;
            i += 5;
            geph.pos[1] = GlobalMembersRcvraw.getbitg(buff, i, 27) * 4.882812500000000E-0x4 * 1E3;
            i += 27 + 4;

            /* frame 3 */
            frn3 = GlobalMembersRtkcmn.getbitu(buff, i, 4);
            i += 4;
            P3 = GlobalMembersRtkcmn.getbitu(buff, i, 1);
            i += 1;
            geph.gamn = GlobalMembersRcvraw.getbitg(buff, i, 11) * DefineConstants.P2_40;
            i += 11 + 1;
            P = GlobalMembersRtkcmn.getbitu(buff, i, 2);
            i += 2;
            ln = GlobalMembersRtkcmn.getbitu(buff, i, 1);
            i += 1;
            geph.vel[2] = GlobalMembersRcvraw.getbitg(buff, i, 24) * 9.536743164062500E-0x7 * 1E3;
            i += 24;
            geph.acc[2] = GlobalMembersRcvraw.getbitg(buff, i, 5) * DefineConstants.P2_30 * 1E3;
            i += 5;
            geph.pos[2] = GlobalMembersRcvraw.getbitg(buff, i, 27) * 4.882812500000000E-0x4 * 1E3;
            i += 27 + 4;

            /* frame 4 */
            frn4 = GlobalMembersRtkcmn.getbitu(buff, i, 4);
            i += 4;
            geph.taun = GlobalMembersRcvraw.getbitg(buff, i, 22) * DefineConstants.P2_30;
            i += 22;
            geph.dtaun = GlobalMembersRcvraw.getbitg(buff, i, 5) * DefineConstants.P2_30;
            i += 5;
            geph.age = GlobalMembersRtkcmn.getbitu(buff, i, 5);
            i += 5 + 14;
            P4 = GlobalMembersRtkcmn.getbitu(buff, i, 1);
            i += 1;
            geph.sva = GlobalMembersRtkcmn.getbitu(buff, i, 4);
            i += 4 + 3;
            NT = GlobalMembersRtkcmn.getbitu(buff, i, 11);
            i += 11;
            slot = GlobalMembersRtkcmn.getbitu(buff, i, 5);
            i += 5;
            M = GlobalMembersRtkcmn.getbitu(buff, i, 2);

            if (frn1 != 1 || frn2 != 2 || frn3 != 3 || frn4 != 4)
            {
                GlobalMembersRtkcmn.trace(3, "decode_glostr error: frn=%d %d %d %d %d\n", frn1, frn2, frn3, frn4);
                return 0;
            }
            if ((geph.sat = GlobalMembersRtkcmn.satno(DefineConstants.SYS_GLO, slot)) == 0)
            {
                GlobalMembersRtkcmn.trace(2, "decode_glostr error: slot=%d\n", slot);
                return 0;
            }
            geph.frq = 0;
            geph.iode = tb;
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: tow=time2gpst(gpst2utc(geph->tof),&week);
            tow = GlobalMembersRtkcmn.time2gpst(GlobalMembersRtkcmn.gpst2utc(new gtime_t(geph.tof)), ref week);
            tod = Math.IEEERemainder(tow, 86400.0);
            tow -= tod;
            tof = tk_h * 3600.0 + tk_m * 60.0 + tk_s - 10800.0; // lt->utc
            if (tof < tod - 43200.0)
            {
                tof += 86400.0;
            }
            else if (tof > tod + 43200.0)
            {
                tof -= 86400.0;
            }
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: geph->tof=utc2gpst(gpst2time(week,tow+tof));
            geph.tof.CopyFrom(GlobalMembersRtkcmn.utc2gpst(GlobalMembersRtkcmn.gpst2time(week, tow + tof)));
            toe = tb * 900.0 - 10800.0; // lt->utc
            if (toe < tod - 43200.0)
            {
                toe += 86400.0;
            }
            else if (toe > tod + 43200.0)
            {
                toe -= 86400.0;
            }
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: geph->toe=utc2gpst(gpst2time(week,tow+toe));
            geph.toe.CopyFrom(GlobalMembersRtkcmn.utc2gpst(GlobalMembersRtkcmn.gpst2time(week, tow + toe))); // utc->gpst
            return 1;
        }
        /* decode gps/qzss navigation data subframe 1 --------------------------------*/
        internal static int decode_subfrm1(byte buff, eph_t eph)
        {
            double tow;
            double toc;
            int i = 48;
            int week;
            int iodc0;
            int iodc1;
            int tgd;

            GlobalMembersRtkcmn.trace(4, "decode_subfrm1:\n");
            GlobalMembersRtkcmn.trace(5, "decode_subfrm1: buff=");
            GlobalMembersRtkcmn.traceb(5, buff, 30);

            tow = GlobalMembersRtkcmn.getbitu(buff, 24, 17) * 6.0; // transmission time
            week = GlobalMembersRtkcmn.getbitu(buff, i, 10);
            i += 10;
            eph.code = GlobalMembersRtkcmn.getbitu(buff, i, 2);
            i += 2;
            eph.sva = GlobalMembersRtkcmn.getbitu(buff, i, 4); // ura index
            i += 4;
            eph.svh = GlobalMembersRtkcmn.getbitu(buff, i, 6);
            i += 6;
            iodc0 = GlobalMembersRtkcmn.getbitu(buff, i, 2);
            i += 2;
            eph.flag = GlobalMembersRtkcmn.getbitu(buff, i, 1);
            i += 1 + 87;
            tgd = GlobalMembersRtkcmn.getbits(buff, i, 8);
            i += 8;
            iodc1 = GlobalMembersRtkcmn.getbitu(buff, i, 8);
            i += 8;
            toc = GlobalMembersRtkcmn.getbitu(buff, i, 16) * 16.0;
            i += 16;
            eph.f2 = GlobalMembersRtkcmn.getbits(buff, i, 8) * DefineConstants.P2_55;
            i += 8;
            eph.f1 = GlobalMembersRtkcmn.getbits(buff, i, 16) * DefineConstants.P2_43;
            i += 16;
            eph.f0 = GlobalMembersRtkcmn.getbits(buff, i, 22) * DefineConstants.P2_31;

            eph.tgd[0] = tgd == -128 ? 0.0 : tgd * DefineConstants.P2_31; // ref [4]
            eph.iodc = (iodc0 << 8) + iodc1;
            eph.week = GlobalMembersRtkcmn.adjgpsweek(week); // week of tow
                                                             //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                                                             //ORIGINAL LINE: eph->ttr=gpst2time(eph->week,tow);
            eph.ttr.CopyFrom(GlobalMembersRtkcmn.gpst2time(eph.week, tow));
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: eph->toc=gpst2time(eph->week,toc);
            eph.toc.CopyFrom(GlobalMembersRtkcmn.gpst2time(eph.week, toc));

            return 1;
        }
        /* decode gps/qzss navigation data subframe 2 --------------------------------*/
        internal static int decode_subfrm2(byte buff, eph_t eph)
        {
            double sqrtA;
            int i = 48;

            GlobalMembersRtkcmn.trace(4, "decode_subfrm2:\n");
            GlobalMembersRtkcmn.trace(5, "decode_subfrm2: buff=");
            GlobalMembersRtkcmn.traceb(5, buff, 30);

            eph.iode = GlobalMembersRtkcmn.getbitu(buff, i, 8);
            i += 8;
            eph.crs = GlobalMembersRtkcmn.getbits(buff, i, 16) * DefineConstants.P2_5;
            i += 16;
            eph.deln = GlobalMembersRtkcmn.getbits(buff, i, 16) * DefineConstants.P2_43 * DefineConstants.SC2RAD;
            i += 16;
            eph.M0 = GlobalMembersRtkcmn.getbits(buff, i, 32) * DefineConstants.P2_31 * DefineConstants.SC2RAD;
            i += 32;
            eph.cuc = GlobalMembersRtkcmn.getbits(buff, i, 16) * 1.862645149230957E-0x9;
            i += 16;
            eph.e = GlobalMembersRtkcmn.getbitu(buff, i, 32) * DefineConstants.P2_33;
            i += 32;
            eph.cus = GlobalMembersRtkcmn.getbits(buff, i, 16) * 1.862645149230957E-0x9;
            i += 16;
            sqrtA = GlobalMembersRtkcmn.getbitu(buff, i, 32) * 1.907348632812500E-0x6;
            i += 32;
            eph.toes = GlobalMembersRtkcmn.getbitu(buff, i, 16) * 16.0;
            i += 16;
            eph.fit = GlobalMembersRtkcmn.getbitu(buff, i, 1) != 0 ? 0.0 : 4.0; // 0:4hr,1:>4hr

            eph.A = sqrtA * sqrtA;

            return 2;
        }
        /* decode gps/qzss navigation data subframe 3 --------------------------------*/
        internal static int decode_subfrm3(byte buff, eph_t eph)
        {
            double tow;
            double toc;
            int i = 48;
            int iode;

            GlobalMembersRtkcmn.trace(4, "decode_subfrm3:\n");
            GlobalMembersRtkcmn.trace(5, "decode_subfrm3: buff=");
            GlobalMembersRtkcmn.traceb(5, buff, 30);

            eph.cic = GlobalMembersRtkcmn.getbits(buff, i, 16) * 1.862645149230957E-0x9;
            i += 16;
            eph.OMG0 = GlobalMembersRtkcmn.getbits(buff, i, 32) * DefineConstants.P2_31 * DefineConstants.SC2RAD;
            i += 32;
            eph.cis = GlobalMembersRtkcmn.getbits(buff, i, 16) * 1.862645149230957E-0x9;
            i += 16;
            eph.i0 = GlobalMembersRtkcmn.getbits(buff, i, 32) * DefineConstants.P2_31 * DefineConstants.SC2RAD;
            i += 32;
            eph.crc = GlobalMembersRtkcmn.getbits(buff, i, 16) * DefineConstants.P2_5;
            i += 16;
            eph.omg = GlobalMembersRtkcmn.getbits(buff, i, 32) * DefineConstants.P2_31 * DefineConstants.SC2RAD;
            i += 32;
            eph.OMGd = GlobalMembersRtkcmn.getbits(buff, i, 24) * DefineConstants.P2_43 * DefineConstants.SC2RAD;
            i += 24;
            iode = GlobalMembersRtkcmn.getbitu(buff, i, 8);
            i += 8;
            eph.idot = GlobalMembersRtkcmn.getbits(buff, i, 14) * DefineConstants.P2_43 * DefineConstants.SC2RAD;

            /* check iode and iodc consistency */
            if (iode != eph.iode || iode != (eph.iodc & 0xFF))
            {
                return 0;
            }

            /* adjustment for week handover */
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: tow=time2gpst(eph->ttr,&eph->week);
            tow = GlobalMembersRtkcmn.time2gpst(new gtime_t(eph.ttr), ref eph.week);
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: toc=time2gpst(eph->toc,null);
            toc = GlobalMembersRtkcmn.time2gpst(new gtime_t(eph.toc), null);
            if (eph.toes < tow - 302400.0)
            {
                eph.week++;
                tow -= 604800.0;
            }
            else if (eph.toes > tow + 302400.0)
            {
                eph.week--;
                tow += 604800.0;
            }
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: eph->toe=gpst2time(eph->week,eph->toes);
            eph.toe.CopyFrom(GlobalMembersRtkcmn.gpst2time(eph.week, eph.toes));
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: eph->toc=gpst2time(eph->week,toc);
            eph.toc.CopyFrom(GlobalMembersRtkcmn.gpst2time(eph.week, toc));
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: eph->ttr=gpst2time(eph->week,tow);
            eph.ttr.CopyFrom(GlobalMembersRtkcmn.gpst2time(eph.week, tow));

            return 3;
        }
        /* decode gps/qzss almanac ---------------------------------------------------*/
        internal static void decode_almanac(byte buff, int sat, alm_t[] alm)
        {
            gtime_t toa = new gtime_t();
            double deltai;
            double sqrtA;
            double tt;
            int i = 50;
            int f0;

            GlobalMembersRtkcmn.trace(4, "decode_almanac: sat=%2d\n", sat);

            if (alm == null || alm[sat - 1].week == 0)
                return;

            alm[sat - 1].sat = sat;
            alm[sat - 1].e = GlobalMembersRtkcmn.getbits(buff, i, 16) * 4.768371582031250E-0x7;
            i += 16;
            alm[sat - 1].toas = GlobalMembersRtkcmn.getbitu(buff, i, 8) * 4096.0;
            i += 8;
            deltai = GlobalMembersRtkcmn.getbits(buff, i, 16) * 1.907348632812500E-0x6* DefineConstants.SC2RAD;
            i += 16;
            alm[sat - 1].OMGd = GlobalMembersRtkcmn.getbits(buff, i, 16) * DefineConstants.P2_38 * DefineConstants.SC2RAD;
            i += 16;
            alm[sat - 1].svh = GlobalMembersRtkcmn.getbitu(buff, i, 8);
            i += 8;
            sqrtA = GlobalMembersRtkcmn.getbitu(buff, i, 24) * 4.882812500000000E-0x4;
            i += 24;
            alm[sat - 1].OMG0 = GlobalMembersRtkcmn.getbits(buff, i, 24) * 1.192092895507810E-0x7* DefineConstants.SC2RAD;
            i += 24;
            alm[sat - 1].omg = GlobalMembersRtkcmn.getbits(buff, i, 24) * 1.192092895507810E-0x7* DefineConstants.SC2RAD;
            i += 24;
            alm[sat - 1].M0 = GlobalMembersRtkcmn.getbits(buff, i, 24) * 1.192092895507810E-0x7* DefineConstants.SC2RAD;
            i += 24;
            f0 = GlobalMembersRtkcmn.getbitu(buff, i, 8);
            i += 8;
            alm[sat - 1].f1 = GlobalMembersRtkcmn.getbits(buff, i, 11) * DefineConstants.P2_38;
            i += 11;
            alm[sat - 1].f0 = GlobalMembersRtkcmn.getbits(buff, i, 3) * 7.629394531250000E-0x6 + f0 * 9.536743164062500E-0x7;
            alm[sat - 1].A = sqrtA * sqrtA;
            alm[sat - 1].i0 = 0.3 * DefineConstants.SC2RAD + deltai;

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: toa=gpst2time(alm[sat-1].week,alm[sat-1].toas);
            toa.CopyFrom(GlobalMembersRtkcmn.gpst2time(alm[sat - 1].week, alm[sat - 1].toas));
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: tt=timediff(toa,alm[sat-1].toa);
            tt = GlobalMembersRtkcmn.timediff(new gtime_t(toa), alm[sat - 1].toa);
            if (tt < 302400.0)
            {
                alm[sat - 1].week--;
            }
            else if (tt > 302400.0)
            {
                alm[sat - 1].week++;
            }
            alm[sat - 1].toa = GlobalMembersRtkcmn.gpst2time(alm[sat - 1].week, alm[sat - 1].toas);
        }
        /* decode gps navigation data subframe 4 -------------------------------------*/
        internal static void decode_gps_subfrm4(byte buff, alm_t[] alm, double[] ion, double[] utc, ref int leaps)
        {
            int i;
            int sat;
            int svid = GlobalMembersRtkcmn.getbitu(buff, 50, 6);

            if (25 <= svid && svid <= 32) // page 2,3,4,5,7,8,9,10
            {

                /* decode almanac */
                sat = GlobalMembersRtkcmn.getbitu(buff, 50, 6);
                if (1 <= sat && sat <= 32)
                {
                    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                    //ORIGINAL LINE: decode_almanac(buff,sat,alm);
                    GlobalMembersRcvraw.decode_almanac(buff, sat, new alm_t(alm));
                }
            }
            else if (svid == 63) // page 25
            {

                /* decode as and sv config */
                i = 56;
                for (sat = 1; sat <= 32; sat++)
                {
                    if (alm != null)
                    {
                        alm[sat - 1].svconf = GlobalMembersRtkcmn.getbitu(buff, i, 4);
                    }
                    i += 4;
                }
                /* decode sv health */
                i = 186;
                for (sat = 25; sat <= 32; sat++)
                {
                    if (alm != null)
                    {
                        alm[sat - 1].svh = GlobalMembersRtkcmn.getbitu(buff, i, 6);
                    }
                    i += 6;
                }
            }
            else if (svid == 56) // page 18
            {

                /* decode ion/utc parameters */
                if (ion != 0)
                {
                    i = 56;
                    ion[0] = GlobalMembersRtkcmn.getbits(buff, i, 8) * DefineConstants.P2_30;
                    i += 8;
                    ion[1] = GlobalMembersRtkcmn.getbits(buff, i, 8) * 7.450580596923828E-0x9;
                    i += 8;
                    ion[2] = GlobalMembersRtkcmn.getbits(buff, i, 8) * 5.960464477539063E-0x8;
                    i += 8;
                    ion[3] = GlobalMembersRtkcmn.getbits(buff, i, 8) * 5.960464477539063E-0x8;
                    i += 8;
                    ion[4] = GlobalMembersRtkcmn.getbits(buff, i, 8) * Math.Pow(2, 11);
                    i += 8;
                    ion[5] = GlobalMembersRtkcmn.getbits(buff, i, 8) * Math.Pow(2, 14);
                    i += 8;
                    ion[6] = GlobalMembersRtkcmn.getbits(buff, i, 8) * Math.Pow(2, 16);
                    i += 8;
                    ion[7] = GlobalMembersRtkcmn.getbits(buff, i, 8) * Math.Pow(2, 16);
                }
                if (utc != 0)
                {
                    i = 120;
                    utc[1] = GlobalMembersRtkcmn.getbits(buff, i, 24) * DefineConstants.P2_50;
                    i += 24;
                    utc[0] = GlobalMembersRtkcmn.getbits(buff, i, 32) * DefineConstants.P2_30;
                    i += 32;
                    utc[2] = GlobalMembersRtkcmn.getbits(buff, i, 8) * Math.Pow(2, 12);
                    i += 8;
                    utc[3] = GlobalMembersRtkcmn.getbitu(buff, i, 8);
                }
                if (leaps != 0)
                {
                    i = 192;
                    leaps = GlobalMembersRtkcmn.getbits(buff, i, 8);
                }
            }
        }
        /* decode gps navigation data subframe 5 -------------------------------------*/
        internal static void decode_gps_subfrm5(byte buff, alm_t[] alm)
        {
            double toas;
            int i;
            int sat;
            int week;
            int svid = GlobalMembersRtkcmn.getbitu(buff, 50, 6);

            if (1 <= svid && svid <= 24) // page 1-24
            {

                /* decode almanac */
                sat = GlobalMembersRtkcmn.getbitu(buff, 50, 6);
                if (1 <= sat && sat <= 32)
                {
                    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                    //ORIGINAL LINE: decode_almanac(buff,sat,alm);
                    GlobalMembersRcvraw.decode_almanac(buff, sat, new alm_t(alm));
                }
            }
            else if (svid == 51) // page 25
            {

                if (alm != null)
                {
                    i = 56;
                    toas = GlobalMembersRtkcmn.getbitu(buff, i, 8) * 4096;
                    i += 8;
                    week = GlobalMembersRtkcmn.getbitu(buff, i, 8);
                    i += 8;
                    week = GlobalMembersRtkcmn.adjgpsweek(week);

                    /* decode sv health */
                    for (sat = 1; sat <= 24; sat++)
                    {
                        alm[sat - 1].svh = GlobalMembersRtkcmn.getbitu(buff, i, 6);
                        i += 6;
                    }
                    for (sat = 1; sat <= 32; sat++)
                    {
                        alm[sat - 1].toas = toas;
                        alm[sat - 1].week = week;
                        alm[sat - 1].toa = GlobalMembersRtkcmn.gpst2time(week, toas);
                    }
                }
            }
        }
        /* decode qzss navigation data subframe 4/5 ----------------------------------*/
        internal static void decode_qzs_subfrm45(byte buff, alm_t[] alm, double[] ion, double[] utc, ref int leaps)
        {
            int i;
            int j;
            int sat;
            int toas;
            int week;
            int svid = GlobalMembersRtkcmn.getbitu(buff, 50, 6);

            if (1 <= svid && svid <= 5) // qzss almanac
            {

                if ((sat = GlobalMembersRtkcmn.satno(DefineConstants.SYS_QZS, 192 + svid)) == 0)
                    return;
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: decode_almanac(buff,sat,alm);
                GlobalMembersRcvraw.decode_almanac(buff, sat, new alm_t(alm));
            }
            else if (svid == 51) // qzss health
            {

                if (alm != null)
                {
                    i = 56;
                    toas = GlobalMembersRtkcmn.getbitu(buff, i, 8) * 4096;
                    i += 8;
                    week = GlobalMembersRtkcmn.getbitu(buff, i, 8);
                    i += 8;
                    week = GlobalMembersRtkcmn.adjgpsweek(week);

                    for (j = 0; j < 5; j++)
                    {
                        if ((sat = GlobalMembersRtkcmn.satno(DefineConstants.SYS_QZS, 193 + j)) == 0)
                            continue;
                        alm[sat - 1].toas = toas;
                        alm[sat - 1].week = week;
                        alm[sat - 1].toa = GlobalMembersRtkcmn.gpst2time(week, toas);
                        alm[sat - 1].svh = GlobalMembersRtkcmn.getbitu(buff, i, 6);
                        i += 6;
                    }
                }
            }
            else if (svid == 56) // ion/utc parameters
            {

                if (ion != 0)
                {
                    i = 56;
                    ion[0] = GlobalMembersRtkcmn.getbits(buff, i, 8) * DefineConstants.P2_30;
                    i += 8;
                    ion[1] = GlobalMembersRtkcmn.getbits(buff, i, 8) * 7.450580596923828E-0x9;
                    i += 8;
                    ion[2] = GlobalMembersRtkcmn.getbits(buff, i, 8) * 5.960464477539063E-0x8;
                    i += 8;
                    ion[3] = GlobalMembersRtkcmn.getbits(buff, i, 8) * 5.960464477539063E-0x8;
                    i += 8;
                    ion[4] = GlobalMembersRtkcmn.getbits(buff, i, 8) * Math.Pow(2, 11);
                    i += 8;
                    ion[5] = GlobalMembersRtkcmn.getbits(buff, i, 8) * Math.Pow(2, 14);
                    i += 8;
                    ion[6] = GlobalMembersRtkcmn.getbits(buff, i, 8) * Math.Pow(2, 16);
                    i += 8;
                    ion[7] = GlobalMembersRtkcmn.getbits(buff, i, 8) * Math.Pow(2, 16);
                }
                if (utc != 0)
                {
                    i = 120;
                    utc[1] = GlobalMembersRtkcmn.getbits(buff, i, 24) * DefineConstants.P2_50;
                    i += 24;
                    utc[0] = GlobalMembersRtkcmn.getbits(buff, i, 32) * DefineConstants.P2_30;
                    i += 32;
                    utc[2] = GlobalMembersRtkcmn.getbits(buff, i, 8) * Math.Pow(2, 12);
                    i += 8;
                    utc[3] = GlobalMembersRtkcmn.getbitu(buff, i, 8);
                }
            }
        }
        /* decode gps/qzss navigation data subframe 4 --------------------------------*/
        internal static int decode_subfrm4(byte buff, alm_t alm, ref double ion, ref double utc, ref int leaps)
        {
            int dataid = GlobalMembersRtkcmn.getbitu(buff, 48, 2);

            GlobalMembersRtkcmn.trace(4, "decode_subfrm4: dataid=%d\n", dataid);
            GlobalMembersRtkcmn.trace(5, "decode_subfrm4: buff=");
            GlobalMembersRtkcmn.traceb(5, buff, 30);

            if (dataid == 1) // gps
            {
                GlobalMembersRcvraw.decode_gps_subfrm4(buff, alm, ion, utc, ref leaps);
            }
            else if (dataid == 3) // qzss
            {
                GlobalMembersRcvraw.decode_qzs_subfrm45(buff, alm, ion, utc, ref leaps);
            }
            return 4;
        }
        /* decode gps/qzss navigation data subframe 5 --------------------------------*/
        internal static int decode_subfrm5(byte buff, alm_t alm, ref double ion, ref double utc, ref int leaps)
        {
            int dataid = GlobalMembersRtkcmn.getbitu(buff, 48, 2);

            GlobalMembersRtkcmn.trace(4, "decode_subfrm5: dataid=%d\n", dataid);
            GlobalMembersRtkcmn.trace(5, "decode_subfrm5: buff=");
            GlobalMembersRtkcmn.traceb(5, buff, 30);

            if (dataid == 1) // gps
            {
                GlobalMembersRcvraw.decode_gps_subfrm5(buff, alm);
            }
            else if (dataid == 3) // qzss
            {
                GlobalMembersRcvraw.decode_qzs_subfrm45(buff, alm, ion, utc, ref leaps);
            }
            return 5;
        }
        /* decode gps/qzss navigation data frame ---------------------------------------
        * decode navigation data frame and extract ephemeris and ion/utc parameters
        * args   : unsigned char *buff I gps navigation data frame (without parity)
        *                                  buff[0-29]: 24 bits x 10 words
        *          eph_t *eph    IO     ephemeris message      (NULL: no input)
        *          alm_t *alm    IO     almanac                (NULL: no input)
        *          double *ion   IO     ionospheric parameters (NULL: no input)
        *          double *utc   IO     delta-utc parameters   (NULL: no input)
        *          int   *leaps  IO     leap seconds (s)       (NULL: no input)
        * return : status (0:no valid, 1-5:subframe id)
        * notes  : use cpu time to resolve modulo 1024 ambiguity of the week number
        *          see ref [1]
        *          utc[3] reference week for utc parameter is truncated in 8 bits
        *          ion and utc parameters by qzss indicate local iono and qzst-utc
        *          parameters.
        *-----------------------------------------------------------------------------*/
        public static int decode_frame(byte buff, eph_t eph, alm_t alm, ref double ion, ref double utc, ref int leaps)
        {
            int id = GlobalMembersRtkcmn.getbitu(buff, 43, 3); // subframe id

            GlobalMembersRtkcmn.trace(3, "decodefrm: id=%d\n", id);

            switch (id)
            {
                case 1:
                    return GlobalMembersRcvraw.decode_subfrm1(buff, eph);
                case 2:
                    return GlobalMembersRcvraw.decode_subfrm2(buff, eph);
                case 3:
                    return GlobalMembersRcvraw.decode_subfrm3(buff, eph);
                case 4:
                    return GlobalMembersRcvraw.decode_subfrm4(buff, alm, ref ion, ref utc, ref leaps);
                case 5:
                    return GlobalMembersRcvraw.decode_subfrm5(buff, alm, ref ion, ref utc, ref leaps);
            }
            return 0;
        }
        /* initialize receiver raw data control ----------------------------------------
        * initialize receiver raw data control struct and reallocate obsevation and
        * epheris buffer
        * args   : raw_t  *raw   IO     receiver raw data control struct
        * return : status (1:ok,0:memory allocation error)
        *-----------------------------------------------------------------------------*/
        public static int init_raw(raw_t raw)
        {
            double[] lam_glo = { DefineConstants.CLIGHT / DefineConstants.FREQ1_GLO, DefineConstants.CLIGHT / DefineConstants.FREQ2_GLO };
            gtime_t time0 = new gtime_t();
            obsd_t data0 = new obsd_t({ 0 });
            eph_t eph0 = new eph_t(0, -1, -1);
            alm_t alm0 = new alm_t(0, -1);
            geph_t geph0 = new geph_t(0, -1);
            seph_t seph0 = new seph_t();
            sbsmsg_t sbsmsg0 = new sbsmsg_t();
            lexmsg_t lexmsg0 = new lexmsg_t();
            int i;
            int j;
            int sys;

            GlobalMembersRtkcmn.trace(3, "init_raw:\n");

            raw.time = raw.tobs = time0;
            raw.ephsat = 0;
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: raw->sbsmsg=sbsmsg0;
            raw.sbsmsg.CopyFrom(sbsmsg0);
            raw.msgtype[0] = '\0';
            for (i = 0; i < DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1; i++)
            {
                for (j = 0; j < 380; j++)
                {
                    raw.subfrm[i, j] = 0;
                }
                for (j = 0; j < DefineConstants.NFREQ; j++)
                {
                    raw.lockt[i, j] = 0.0;
                }
                for (j = 0; j < DefineConstants.NFREQ; j++)
                {
                    raw.halfc[i, j] = 0;
                }
                raw.icpp[i] = raw.off[i] = raw.prCA[i] = raw.dpCA[i] = 0.0;
            }
            for (i = 0; i < DefineConstants.MAXOBS; i++)
            {
                raw.freqn = raw.freqn.Substring(0, i);
            }
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: raw->lexmsg=lexmsg0;
            raw.lexmsg.CopyFrom(lexmsg0);
            raw.icpc = 0.0;
            raw.nbyte = raw.len = 0;
            raw.iod = raw.flag = raw.tbase = raw.outtype = 0;
            raw.tod = -1;
            for (i = 0; i < DefineConstants.MAXRAWLEN; i++)
            {
                raw.buff[i] = 0;
            }
            raw.opt[0] = '\0';
            raw.receive_time = 0.0;
            raw.plen = raw.pbyte = raw.page = raw.reply = 0;
            raw.week = 0;

            raw.obs.data = null;
            raw.obuf.data = null;
            raw.nav.eph = null;
            raw.nav.alm = null;
            raw.nav.geph = null;
            raw.nav.seph = null;

            //C++ TO C# CONVERTER TODO TASK: The memory management function 'malloc' has no equivalent in C#:
            if ((raw.obs.data = (obsd_t)malloc(sizeof(obsd_t) * DefineConstants.MAXOBS)) == null || (raw.obuf.data = (obsd_t)malloc(sizeof(obsd_t) * DefineConstants.MAXOBS)) == null || (raw.nav.eph = (eph_t)malloc(sizeof(eph_t) * DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1)) == null || (raw.nav.alm = (alm_t)malloc(sizeof(alm_t) * DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1)) == null || (raw.nav.geph = (geph_t)malloc(sizeof(geph_t) * DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1)) == null || (raw.nav.seph = (seph_t)malloc(sizeof(seph_t) * DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 * 2)) == null)
            {
                GlobalMembersRcvraw.free_raw(raw);
                return 0;
            }
            raw.obs.n = 0;
            raw.obuf.n = 0;
            raw.nav.n = DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1;
            raw.nav.na = DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1;
            raw.nav.ng = DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1;
            raw.nav.ns = DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 * 2;
            for (i = 0; i < DefineConstants.MAXOBS; i++)
            {
                raw.obs.data[i] = data0;
            }
            for (i = 0; i < DefineConstants.MAXOBS; i++)
            {
                raw.obuf.data[i] = data0;
            }
            for (i = 0; i < DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1; i++)
            {
                raw.nav.eph[i] = eph0;
            }
            for (i = 0; i < DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1; i++)
            {
                raw.nav.alm[i] = alm0;
            }
            for (i = 0; i < DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1; i++)
            {
                raw.nav.geph[i] = geph0;
            }
            for (i = 0; i < DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 * 2; i++)
            {
                raw.nav.seph[i] = seph0;
            }
            for (i = 0; i < DefineConstants.MAXPRNGPS - DefineConstants.MINPRNGPS + 1 + DefineConstants.MAXPRNGLO - DefineConstants.MINPRNGLO + 1 + DefineConstants.MAXPRNGAL - DefineConstants.MINPRNGAL + 1 + DefineConstants.MAXPRNQZS - DefineConstants.MINPRNQZS + 1 + DefineConstants.MAXPRNCMP - DefineConstants.MINPRNCMP + 1 + DefineConstants.MAXPRNSBS - DefineConstants.MINPRNSBS + 1 + DefineConstants.MAXPRNLEO - DefineConstants.MINPRNLEO + 1; i++)
            {
                for (j = 0; j < DefineConstants.NFREQ; j++)
                {
                    if ((sys = GlobalMembersRtkcmn.satsys(i + 1, null)) == 0)
                        continue;
                    raw.nav.lam[i, j] = sys == DefineConstants.SYS_GLO ? lam_glo[j] : GlobalMembersRtkcmn.lam_carr[j];
                }
            }
            raw.sta.name[0] = raw.sta.marker[0] = '\0';
            raw.sta.antdes[0] = raw.sta.antsno[0] = '\0';
            raw.sta.rectype[0] = raw.sta.recver[0] = raw.sta.recsno[0] = '\0';
            raw.sta.antsetup = raw.sta.itrf = raw.sta.deltype = 0;
            for (i = 0; i < 3; i++)
            {
                raw.sta.pos[i] = raw.sta.del[i] = 0.0;
            }
            raw.sta.hgt = 0.0;
            return 1;
        }
        /* free receiver raw data control ----------------------------------------------
        * free observation and ephemeris buffer in receiver raw data control struct
        * args   : raw_t  *raw   IO     receiver raw data control struct
        * return : none
        *-----------------------------------------------------------------------------*/
        public static void free_raw(raw_t raw)
        {
            GlobalMembersRtkcmn.trace(3, "free_raw:\n");

            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(raw.obs.data);
            raw.obs.data = null;
            raw.obs.n = 0;
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(raw.obuf.data);
            raw.obuf.data = null;
            raw.obuf.n = 0;
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(raw.nav.eph);
            raw.nav.eph = null;
            raw.nav.n = 0;
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(raw.nav.alm);
            raw.nav.alm = null;
            raw.nav.na = 0;
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(raw.nav.geph);
            raw.nav.geph = null;
            raw.nav.ng = 0;
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(raw.nav.seph);
            raw.nav.seph = null;
            raw.nav.ns = 0;
        }
        /* input receiver raw data from stream -----------------------------------------
        * fetch next receiver raw data and input a message from stream
        * args   : raw_t  *raw   IO     receiver raw data control struct
        *          int    format I      receiver raw data format (STRFMT_???)
        *          unsigned char data I stream data (1 byte)
        * return : status (-1: error message, 0: no message, 1: input observation data,
        *                  2: input ephemeris, 3: input sbas message,
        *                  9: input ion/utc parameter, 31: input lex message)
        *-----------------------------------------------------------------------------*/
        public static int input_raw(raw_t raw, int format, byte data)
        {
            GlobalMembersRtkcmn.trace(5, "input_raw: format=%d data=0x%02x\n", format, data);

            switch (format)
            {
                case DefineConstants.STRFMT_OEM4:
                    return GlobalMembersNovatel.input_oem4(raw, data);
                case DefineConstants.STRFMT_OEM3:
                    return GlobalMembersNovatel.input_oem3(raw, data);
                case DefineConstants.STRFMT_UBX:
                    return GlobalMembersUblox.input_ubx(raw, data);
                case DefineConstants.STRFMT_SS2:
                    return GlobalMembersSs2.input_ss2(raw, data);
                case DefineConstants.STRFMT_CRES:
                    return GlobalMembersCrescent.input_cres(raw, data);
                case DefineConstants.STRFMT_STQ:
                    return GlobalMembersSkytraq.input_stq(raw, data);
                case DefineConstants.STRFMT_GW10:
                    return GlobalMembersGw10.input_gw10(raw, data);
                case DefineConstants.STRFMT_JAVAD:
                    return GlobalMembersJavad.input_javad(raw, data);
                case DefineConstants.STRFMT_NVS:
                    return GlobalMembersNvs.input_nvs(raw, data);
                case DefineConstants.STRFMT_BINEX:
                    return GlobalMembersBinex.input_bnx(raw, data);
                case DefineConstants.STRFMT_RT17:
                    return GlobalMembersRt17.input_rt17(raw, data);
                case DefineConstants.STRFMT_LEXR:
                    return GlobalMembersRcvlex.input_lexr(raw, data);
            }
            return 0;
        }
        /* input receiver raw data from file -------------------------------------------
        * fetch next receiver raw data and input a message from file
        * args   : raw_t  *raw   IO     receiver raw data control struct
        *          int    format I      receiver raw data format (STRFMT_???)
        *          FILE   *fp    I      file pointer
        * return : status(-2: end of file/format error, -1...31: same as above)
        *-----------------------------------------------------------------------------*/
        public static int input_rawf(raw_t raw, int format, FILE fp)
        {
            GlobalMembersRtkcmn.trace(4, "input_rawf: format=%d\n", format);

            switch (format)
            {
                case DefineConstants.STRFMT_OEM4:
                    return GlobalMembersNovatel.input_oem4f(raw, fp);
                case DefineConstants.STRFMT_OEM3:
                    return GlobalMembersNovatel.input_oem3f(raw, fp);
                case DefineConstants.STRFMT_UBX:
                    return GlobalMembersUblox.input_ubxf(raw, fp);
                case DefineConstants.STRFMT_SS2:
                    return GlobalMembersSs2.input_ss2f(raw, fp);
                case DefineConstants.STRFMT_CRES:
                    return GlobalMembersCrescent.input_cresf(raw, fp);
                case DefineConstants.STRFMT_STQ:
                    return GlobalMembersSkytraq.input_stqf(raw, fp);
                case DefineConstants.STRFMT_GW10:
                    return GlobalMembersGw10.input_gw10f(raw, fp);
                case DefineConstants.STRFMT_JAVAD:
                    return GlobalMembersJavad.input_javadf(raw, fp);
                case DefineConstants.STRFMT_NVS:
                    return GlobalMembersNvs.input_nvsf(raw, fp);
                case DefineConstants.STRFMT_BINEX:
                    return GlobalMembersBinex.input_bnxf(raw, fp);
                case DefineConstants.STRFMT_RT17:
                    return GlobalMembersRt17.input_rt17f(raw, fp);
                case DefineConstants.STRFMT_LEXR:
                    return GlobalMembersRcvlex.input_lexrf(raw, fp);
            }
            return -2;
        }
    }
}
