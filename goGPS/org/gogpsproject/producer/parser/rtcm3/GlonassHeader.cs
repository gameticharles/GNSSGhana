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
	public class GlonassHeader
	{

		private int stationid; // 12
		private long epochTime; // 27
		private int flag; // 1
		private int numberOfSatellites; // 5
		private int smoothIndicator; // 1
		private int smoothInterval; // 3

		public virtual long EpochTime
		{
			get
			{
				return epochTime;
			}
			set
			{
				this.epochTime = value;
			}
		}

		public virtual int Flag
		{
			get
			{
				return flag;
			}
			set
			{
				this.flag = value;
			}
		}

		public virtual int NumberOfSatellites
		{
			get
			{
				return numberOfSatellites;
			}
			set
			{
				this.numberOfSatellites = value;
			}
		}

		public virtual int SmoothIndicator
		{
			get
			{
				return smoothIndicator;
			}
			set
			{
				this.smoothIndicator = value;
			}
		}

		public virtual int SmoothInterval
		{
			get
			{
				return smoothInterval;
			}
			set
			{
				this.smoothInterval = value;
			}
		}

		public virtual int Stationid
		{
			get
			{
				return stationid;
			}
			set
			{
				this.stationid = value;
			}
		}







		public override string ToString()
		{
			return "Glonassheader [epochTime=" + epochTime + ", flag=" + flag + ", numberOfSatellites=" + numberOfSatellites + ", smoothIndicator=" + smoothIndicator + ", smoothInterval=" + smoothInterval + ", stationid=" + stationid + "]";
		}

	}

}