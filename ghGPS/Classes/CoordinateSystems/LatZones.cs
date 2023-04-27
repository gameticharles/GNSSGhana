using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ghGPS.Classes.CoordinateSystems
{
    public class LatZones
    {

        internal char[] letters = new char[] { 'A', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'K', 'L', 'M', 'N', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Z' };

        internal int[] degrees = new int[] { -90, -84, -72, -64, -56, -48, -40, -32, -24, -16, -8, 0, 8, 16, 24, 32, 40, 48, 56, 64, 72, 84 };

        internal char[] negLetters = new char[] { 'A', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'K', 'L', 'M' };

        internal int[] negDegrees = new int[] { -90, -84, -72, -64, -56, -48, -40, -32, -24, -16, -8 };

        internal char[] posLetters = new char[] { 'N', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Z' };

        internal int[] posDegrees = new int[] { 0, 8, 16, 24, 32, 40, 48, 56, 64, 72, 84 };

        internal int arrayLength = 22;

        public LatZones()
        {

        }

        public virtual int getLatZoneDegree(string letter)
        {
            char ltr = letter[0];
            for (int i = 0; i < arrayLength; i++)
            {
                if (letters[i] == ltr)
                {
                    return degrees[i];
                }
            }
            return -100;
        }

        public virtual string getLatZone(double latitude)
        {
            int latIndex = -2;
            int lat = (int)latitude;

            if (lat >= 0)
            {
                int len = posLetters.Length;
                for (int i = 0; i < len; i++)
                {
                    if (lat == posDegrees[i])
                    {
                        latIndex = i;
                        break;
                    }

                    if (lat > posDegrees[i])
                    {
                        continue;
                    }
                    else
                    {
                        latIndex = i - 1;
                        break;
                    }
                }
            }
            else
            {
                int len = negLetters.Length;
                for (int i = 0; i < len; i++)
                {
                    if (lat == negDegrees[i])
                    {
                        latIndex = i;
                        break;
                    }

                    if (lat < negDegrees[i])
                    {
                        latIndex = i - 1;
                        break;
                    }
                    else
                    {
                        continue;
                    }

                }

            }

            if (latIndex == -1)
            {
                latIndex = 0;
            }
            if (lat >= 0)
            {
                if (latIndex == -2)
                {
                    latIndex = posLetters.Length - 1;
                }
                return Convert.ToString(posLetters[latIndex]);
            }
            else
            {
                if (latIndex == -2)
                {
                    latIndex = negLetters.Length - 1;
                }
                return Convert.ToString(negLetters[latIndex]);

            }
        }

    }
}
