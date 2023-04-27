/*
 * Copyright (c) 2011 Eugenio Realini, Mirko Reguzzoni, Cryms sagl - Switzerland. 
 * All Rights Reserved.
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
namespace org.gogpsproject.positioning
{
	using Observations = org.gogpsproject.producer.Observations;

	/// <summary>
	/// <para>
	/// Receiver position class
	/// </para>
	/// 
	/// @author Eugenio Realini, Cryms.com, Daisuke Yoshida, Emanuele Ziglioli (Sirtrack Ltd)
	/// </summary>
	public class RoverPosition : ReceiverPosition
	{

	  public Time sampleTime;
	  public Observations obs;
	  public Status status = Status.Valid;

	  /// <summary>
	  /// Sats in use from an observation set </summary>
	  public long satsInUse = 0;

	  /// <summary>
	  /// coarse time clock error in ms </summary>
	  public long cErrMS = 0;

	  /// <summary>
	  /// standard subms clock error in s </summary>
	  internal double clockError;

	  /// <summary>
	  /// Clock error rate </summary>
		public double clockErrorRate;

	  /// <summary>
	  /// Average residual error for least-squares computation </summary>
	  public double eRes;

	  /// <summary>
	  /// Position dilution of precision (PDOP) </summary>
		internal double pDop;

		/// <summary>
		/// Horizontal dilution of precision (HDOP) </summary>
		internal double hDop;

	  /// <summary>
	  /// Vertical dilution of precision (VDOP) </summary>
		internal double vDop;

		/// <summary>
		/// Kalman-derived position dilution of precision (KPDOP) </summary>
		internal double kpDop;

		/// <summary>
		/// Kalman-derived horizontal dilution of precision (KHDOP) </summary>
		internal double khDop;

		/// <summary>
		/// Kalman-derived vertical dilution of precision (KVDOP) </summary>
		internal double kvDop;

	  public enum DopType
	  {
		NONE,

		/// <summary>
		/// Standard DOP values (satellite geometry only) </summary>
		STANDARD,

		/// <summary>
		/// Kalman DOP values (KDOP), based on the Kalman filter error covariance matrix </summary>
		KALMAN
	  }

	  private DopType dopType = DopType.NONE;

		public RoverPosition() : base()
		{
			this.setXYZ(0.0, 0.0, 0.0);
			this.clockError = 0.0;
		}

	  public RoverPosition(Coordinates c) : this(c, DopType.NONE, 0.0, 0.0, 0.0)
	  {
	  }

	  public RoverPosition(Coordinates c, DopType dopType, double pDop, double hDop, double vDop) : base()
	  {
		c.cloneInto(this);
		this.dopType = dopType;
		this.pDop = pDop;
		this.hDop = hDop;
		this.vDop = vDop;
	  }

		public virtual double ClockError
		{
			get
			{
				return clockError;
			}
			set
			{
				this.clockError = value;
			}
		}


	  public virtual double ClockErrorRate
	  {
		  get
		  {
			return clockErrorRate;
		  }
		  set
		  {
			this.clockErrorRate = value;
		  }
	  }


		public virtual double getpDop()
		{
			return pDop;
		}

		public virtual void setpDop(double pDop)
		{
			this.pDop = pDop;
		}

		public virtual double gethDop()
		{
			return hDop;
		}

		public virtual void sethDop(double hDop)
		{
			this.hDop = hDop;
		}

		public virtual double getvDop()
		{
			return vDop;
		}

		public virtual void setvDop(double vDop)
		{
			this.vDop = vDop;
		}

		public virtual double KpDop
		{
			get
			{
				return kpDop;
			}
			set
			{
				this.kpDop = value;
			}
		}


		public virtual double KhDop
		{
			get
			{
				return khDop;
			}
			set
			{
				this.khDop = value;
			}
		}


		public virtual double KvDop
		{
			get
			{
				return kvDop;
			}
			set
			{
				this.kvDop = value;
			}
		}


	  /// <returns> the dopType </returns>
	  public virtual DopType DopType
	  {
		  get
		  {
			return dopType;
		  }
		  set
		  {
			this.dopType = value;
		  }
	  }


	  public virtual RoverPosition clone(Observations obs)
	  {
		RoverPosition r = new RoverPosition(this, DopType.STANDARD, pDop, hDop, vDop);
		r.obs = obs;
		r.sampleTime = sampleTime;
		r.satsInUse = satsInUse;
		r.eRes = eRes;
		r.status = status;
		r.clockError = clockError;
		r.clockErrorRate = clockErrorRate;
		r.cErrMS = cErrMS;
		return r;
	  }
	}

}