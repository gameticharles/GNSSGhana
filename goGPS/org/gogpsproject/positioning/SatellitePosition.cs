/*
 * Copyright (c) 2010, Eugenio Realini, Mirko Reguzzoni, Cryms sagl - Switzerland. All Rights Reserved.
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
 *
 */
using MatrixLib.Matricies;

namespace org.gogpsproject.positioning
{


	/// <summary>
	/// <para>
	/// Satellite position class
	/// </para>
	/// 
	/// @author Eugenio Realini, Cryms.com
	/// </summary>
	public class SatellitePosition : Coordinates
	{
	  public static readonly SatellitePosition UnhealthySat = new SatellitePosition(0, 0, '0', 0, 0, 0);

		private int satID; // Satellite ID number
		private char satType;
		private double satelliteClockError; // Correction due to satellite clock error in seconds
		//private double range;
		private long unixTime;
		private bool predicted;
		private bool maneuver;
	  private Matrix speed;

		public SatellitePosition(long unixTime, int satID, char satType, double x, double y, double z) : base()
		{

			this.unixTime = unixTime;
			this.satID = satID;
			this.satType = satType;

			this.setXYZ(x, y, z);
		this.speed = new Matrix(3, 1);
		}

		/// <returns> the satID </returns>
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


		/// <returns> the satType </returns>
		public virtual char SatType
		{
			get
			{
				return satType;
			}
			set
			{
				this.satType = value;
			}
		}


		/// <returns> the timeCorrection </returns>
		public virtual double SatelliteClockError
		{
			get
			{
				return satelliteClockError;
			}
			set
			{
				this.satelliteClockError = value;
			}
		}


		/// <returns> the time </returns>
		public virtual long UtcTime
		{
			get
			{
				return unixTime;
			}
		}

		/// <param name="predicted"> the predicted to set </param>
		public virtual bool Predicted
		{
			set
			{
				this.predicted = value;
			}
			get
			{
				return predicted;
			}
		}


		/// <param name="maneuver"> the maneuver to set </param>
		public virtual bool Maneuver
		{
			set
			{
				this.maneuver = value;
			}
			get
			{
				return maneuver;
			}
		}


	  public virtual Matrix Speed
	  {
		  get
		  {
			return speed;
		  }
	  }

	  public virtual void setSpeed(double xdot, double ydot, double zdot)
	  {
		this.speed.setElement(0,0, xdot);
		this.speed.setElement(1,0, ydot);
		this.speed.setElement(2,0, zdot);
	  }

		public override string ToString()
		{
			return "X:" + this.X + " Y:" + this.Y + " Z:" + Z + " clkCorr:" + SatelliteClockError;
		}

		public override object clone()
		{
			SatellitePosition sp = new SatellitePosition(this.unixTime,this.satID, this.satType, this.X,this.Y,this.Z);
			sp.maneuver = this.maneuver;
			sp.predicted = this.predicted;
			sp.satelliteClockError = this.satelliteClockError;
		    sp.setSpeed(speed.getElement(0,0), speed.getElement(1,0), speed.getElement(2,0));
			return sp;
		}
	}

}