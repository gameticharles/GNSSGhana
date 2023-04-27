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
	public class AntennaDescriptor
	{

		private int stationID;
		private string antennaDescriptor;
		private int setupID;
		private string antennaSerial;

		public virtual string AntennaDescriptor
		{
			get
			{
				return antennaDescriptor;
			}
			set
			{
				this.antennaDescriptor = value;
			}
		}

		public virtual int SetupID
		{
			get
			{
				return setupID;
			}
			set
			{
				this.setupID = value;
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
			return "AntennaDescriptor [antennaDescriptor=" + antennaDescriptor + ", setupID=" + setupID + ", stationID=" + stationID + " antennaSerial=" + antennaSerial + "]";
		}

		/// <returns> the antennaSerial </returns>
		public virtual string AntennaSerial
		{
			get
			{
				return antennaSerial;
			}
			set
			{
				this.antennaSerial = value;
			}
		}


	}

}