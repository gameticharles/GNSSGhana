using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ghGPS.Classes.CoordinateSystems
{
    public class Digraphs
    {

        internal IDictionary digraph1 = new Hashtable();

        internal IDictionary digraph2 = new Hashtable();

        internal string[] digraph1Array = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "J", "K", "L", "M", "N", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };

        internal string[] digraph2Array = new string[] { "V", "A", "B", "C", "D", "E", "F", "G", "H", "J", "K", "L", "M", "N", "P", "Q", "R", "S", "T", "U", "V" };

        public Digraphs()
        {
            digraph1[new int?(1)] = "A";
            digraph1[new int?(2)] = "B";
            digraph1[new int?(3)] = "C";
            digraph1[new int?(4)] = "D";
            digraph1[new int?(5)] = "E";
            digraph1[new int?(6)] = "F";
            digraph1[new int?(7)] = "G";
            digraph1[new int?(8)] = "H";
            digraph1[new int?(9)] = "J";
            digraph1[new int?(10)] = "K";
            digraph1[new int?(11)] = "L";
            digraph1[new int?(12)] = "M";
            digraph1[new int?(13)] = "N";
            digraph1[new int?(14)] = "P";
            digraph1[new int?(15)] = "Q";
            digraph1[new int?(16)] = "R";
            digraph1[new int?(17)] = "S";
            digraph1[new int?(18)] = "T";
            digraph1[new int?(19)] = "U";
            digraph1[new int?(20)] = "V";
            digraph1[new int?(21)] = "W";
            digraph1[new int?(22)] = "X";
            digraph1[new int?(23)] = "Y";
            digraph1[new int?(24)] = "Z";

            digraph2[new int?(0)] = "V";
            digraph2[new int?(1)] = "A";
            digraph2[new int?(2)] = "B";
            digraph2[new int?(3)] = "C";
            digraph2[new int?(4)] = "D";
            digraph2[new int?(5)] = "E";
            digraph2[new int?(6)] = "F";
            digraph2[new int?(7)] = "G";
            digraph2[new int?(8)] = "H";
            digraph2[new int?(9)] = "J";
            digraph2[new int?(10)] = "K";
            digraph2[new int?(11)] = "L";
            digraph2[new int?(12)] = "M";
            digraph2[new int?(13)] = "N";
            digraph2[new int?(14)] = "P";
            digraph2[new int?(15)] = "Q";
            digraph2[new int?(16)] = "R";
            digraph2[new int?(17)] = "S";
            digraph2[new int?(18)] = "T";
            digraph2[new int?(19)] = "U";
            digraph2[new int?(20)] = "V";

        }

        public virtual int getDigraph1Index(string letter)
        {
            for (int i = 0; i < digraph1Array.Length; i++)
            {
                if (digraph1Array[i].Equals(letter))
                {
                    return i + 1;
                }
            }

            return -1;
        }

        public virtual int getDigraph2Index(string letter)
        {
            for (int i = 0; i < digraph2Array.Length; i++)
            {
                if (digraph2Array[i].Equals(letter))
                {
                    return i;
                }
            }

            return -1;
        }

        public virtual string getDigraph1(int longZone, double easting)
        {
            int a1 = longZone;
            double a2 = 8 * ((a1 - 1) % 3) + 1;

            double a3 = easting;
            double a4 = a2 + ((int)(a3 / 100000)) - 1;
            return (string)digraph1[new int?((int)Math.Floor(a4))];
        }

        public virtual string getDigraph2(int longZone, double northing)
        {
            int a1 = longZone;
            double a2 = 1 + 5 * ((a1 - 1) % 2);
            double a3 = northing;
            double a4 = (a2 + ((int)(a3 / 100000)));
            a4 = (a2 + ((int)(a3 / 100000.0))) % 20;
            a4 = Math.Floor(a4);
            if (a4 < 0)
            {
                a4 = a4 + 19;
            }
            return (string)digraph2[new int?((int)Math.Floor(a4))];

        }

    }
}
