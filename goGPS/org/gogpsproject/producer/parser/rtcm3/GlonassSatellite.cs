/*
 * Copyright (c) 2010 Eugenio Realini, Mirko Reguzzoni, Cryms sagl - Switzerland. All Rights Reserved.
 *
 * This file is part of goGPS Project (goGPS).
 *
 * goGPS is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as
 * published by the Free Software Foundation, either version 3
 * of the License, or (at your option) any later version.
 *
 * goGPS is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with goGPS.  If not, see <http://www.gnu.org/licenses/>.
 *
 */

namespace org.gogpsproject.producer.parser.rtcm3
{
	public class GlonassSatellite
	{

		// GLONASS Satellite ID (Satellite Slot Number) DF038 uint6 6
		// GLONASS L1 Code Indicator DF039 bit(1) 1
		// GLONASS Satellite Frequency Channel Number DF040 uint5 5
		// GLONASS L1 Pseudorange DF041 uint25 25
		// GLONASS L1 PhaseRange � L1 Pseudorange DF042 int20 20
		// GLONASS L1 Lock time Indicator DF043 uint7 7
		// GLONASS Integer L1 Pseudorange Modulus
		// Ambiguity
		// DF044 uint7 7
		// GLONASS L1 CNR DF045 uint8 8
		// GLONASS L2 Code Indicator DF046 bit(2) 2
		// GLONASS L2-L1 Pseudorange Difference DF047 uint14 14
		// GLONASS L2 PhaseRange � L1 Pseudorange DF048 int20 20
		// GLONASS L2 Lock time Indicator DF049 uint7 7
		// GLONASS L2 CNR DF050 uint8 8

		private int satID;
		private int l1code;
		private int satFrequency;
		private long l1pseudorange;
		private double l1phaserange;
		private int l1locktime;
		private int l1psedorangemod;
		private int l1CNR;
		private int l2code;
		private double l2l1psedorangeDif;
		private double l2l1phaserangeDif;
		private int l2locktime;
		private int l2CNR;

		public virtual int L1CNR
		{
			get
			{
				return l1CNR;
			}
			set
			{
				this.l1CNR = value;
			}
		}

		public virtual int L1code
		{
			get
			{
				return l1code;
			}
			set
			{
				this.l1code = value;
			}
		}

		public virtual int L1locktime
		{
			get
			{
				return l1locktime;
			}
			set
			{
				this.l1locktime = value;
			}
		}

		public virtual double L1phaserange
		{
			get
			{
				return l1phaserange;
			}
			set
			{
				this.l1phaserange = value;
			}
		}

		public virtual int L1psedorangemod
		{
			get
			{
				return l1psedorangemod;
			}
			set
			{
				this.l1psedorangemod = value;
			}
		}

		public virtual long L1pseudorange
		{
			get
			{
				return l1pseudorange;
			}
			set
			{
				this.l1pseudorange = value;
			}
		}

		public virtual int L2CNR
		{
			get
			{
				return l2CNR;
			}
			set
			{
				this.l2CNR = value;
			}
		}

		public virtual int L2code
		{
			get
			{
				return l2code;
			}
			set
			{
				this.l2code = value;
			}
		}

		public virtual double L2l1phaserangeDif
		{
			get
			{
				return l2l1phaserangeDif;
			}
			set
			{
				this.l2l1phaserangeDif = value;
			}
		}

		public virtual double L2l1psedorangeDif
		{
			get
			{
				return l2l1psedorangeDif;
			}
			set
			{
				this.l2l1psedorangeDif = value;
			}
		}

		public virtual int L2locktime
		{
			get
			{
				return l2locktime;
			}
			set
			{
				this.l2locktime = value;
			}
		}

		public virtual int SatFrequency
		{
			get
			{
				return satFrequency;
			}
			set
			{
				this.satFrequency = value;
			}
		}

		public virtual int SatID
		{
			get
			{
				return satID;
			}
			set
			{
				this.satID = value;
			}
		}














		public override string ToString()
		{
			return "GlonassSatellite [l1CNR=" + l1CNR + ", l1code=" + l1code + ", l1locktime=" + l1locktime + ", l1phaserange=" + l1phaserange + ", l1psedorangemod=" + l1psedorangemod + ", l1pseudorange=" + l1pseudorange + ", l2CNR=" + l2CNR + ", l2code=" + l2code + ", l2l1phaserangeDif=" + l2l1phaserangeDif + ", l2l1psedorangeDif=" + l2l1psedorangeDif + ", l2locktime=" + l2locktime + ", satFrequency=" + satFrequency + ", satID=" + satID + "]";
		}

	}

}