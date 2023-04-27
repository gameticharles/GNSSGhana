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
	public class StationaryAntenna
	{

		private int stationID;
		private int itrl; // realization year
		private int gpsIndicator;
		private int glonassIndicator;
		private int rgalileoIndicator; // reserved galileo indicator
		private int rstationIndicator; // reference station indicator
		private double antennaRefPointX; // antenna reference point ECEF-x
		private int sreceiverOscillator; // signle receiver oscillator
		private int reserved1;
		private double antennaRefPointY; // antenna reference point ECEF-y
		private int reserved2;
		private double antennaRefPointZ; // antenna reference point ECEF-Y
		private double antennaHeight;


		public virtual double AntennaRefPointX
		{
			get
			{
				return antennaRefPointX;
			}
			set
			{
				this.antennaRefPointX = value;
			}
		}

		public virtual double AntennaRefPointY
		{
			get
			{
				return antennaRefPointY;
			}
			set
			{
				this.antennaRefPointY = value;
			}
		}

		public virtual double AntennaRefPointZ
		{
			get
			{
				return antennaRefPointZ;
			}
			set
			{
				this.antennaRefPointZ = value;
			}
		}

		public virtual int GlonassIndicator
		{
			get
			{
				return glonassIndicator;
			}
			set
			{
				this.glonassIndicator = value;
			}
		}

		public virtual int GpsIndicator
		{
			get
			{
				return gpsIndicator;
			}
			set
			{
				this.gpsIndicator = value;
			}
		}

		public virtual int Itrl
		{
			get
			{
				return itrl;
			}
			set
			{
				this.itrl = value;
			}
		}

		public virtual int Reserved1
		{
			get
			{
				return reserved1;
			}
			set
			{
				this.reserved1 = value;
			}
		}

		public virtual int Reserved2
		{
			get
			{
				return reserved2;
			}
			set
			{
				this.reserved2 = value;
			}
		}

		public virtual int RgalileoIndicator
		{
			get
			{
				return rgalileoIndicator;
			}
			set
			{
				this.rgalileoIndicator = value;
			}
		}

		public virtual int RstationIndicator
		{
			get
			{
				return rstationIndicator;
			}
			set
			{
				this.rstationIndicator = value;
			}
		}

		public virtual int SreceiverOscillator
		{
			get
			{
				return sreceiverOscillator;
			}
			set
			{
				this.sreceiverOscillator = value;
			}
		}

		public virtual int StationID
		{
			get
			{
				return stationID;
			}
			set
			{
				this.stationID = value;
			}
		}













		public override string ToString()
		{
			return "StationaryAntenna [antennaRefPointX=" + antennaRefPointX + ", antennaRefPointY=" + antennaRefPointY + ", antennaRefPointZ=" + antennaRefPointZ + ", glonassIndicator=" + glonassIndicator + ", gpsIndicator=" + gpsIndicator + ", itrl=" + itrl + ", reserved1=" + reserved1 + ", reserved2=" + reserved2 + ", rgalileoIndicator=" + rgalileoIndicator + ", rstationIndicator=" + rstationIndicator + ", sreceiverOscillator=" + sreceiverOscillator + ", stationID=" + stationID + "]";
		}

		/// <param name="antennaHeight"> the antennaHeight to set </param>
		public virtual double AntennaHeight
		{
			set
			{
				this.antennaHeight = value;
			}
			get
			{
				return antennaHeight;
			}
		}


	}

}