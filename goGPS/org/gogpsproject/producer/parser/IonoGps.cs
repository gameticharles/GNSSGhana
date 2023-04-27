/*
 * Copyright (c) 2011 Eugenio Realini, Mirko Reguzzoni, Cryms sagl - Switzerland. All Rights Reserved.
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
namespace org.gogpsproject.producer.parser
{


	using Time = org.gogpsproject.positioning.Time;


	/// <summary>
	/// The Class IonoGps.
	/// 
	/// @author Lorenzo Patocchi
	/// </summary>
	public class IonoGps : Streamable
	{

		private const int STREAM_V = 1;


		/// <summary>
		/// Bitmask, every bit represenst a GPS SV (1-32). If the bit is set the SV is healthy. </summary>
		private long health = 0;

		/// <summary>
		/// UTC - parameter A1. </summary>
		private double utcA1;

		/// <summary>
		/// UTC - parameter A0. </summary>
		private double utcA0;

		/// <summary>
		/// UTC - reference time of week. </summary>
		private long utcTOW;

		/// <summary>
		/// UTC - reference week number. </summary>
		private int utcWNT;

		/// <summary>
		/// UTC - time difference due to leap seconds before event. </summary>
		private int utcLS;

		/// <summary>
		/// UTC - week number when next leap second event occurs. </summary>
		private int utcWNF;

		/// <summary>
		/// UTC - day of week when next leap second event occurs. </summary>
		private int utcDN;

		/// <summary>
		/// UTC - time difference due to leap seconds after event. </summary>
		private int utcLSF;

		/// <summary>
		/// Klobuchar - alpha. </summary>
		private float[] alpha = new float[4];

		/// <summary>
		/// Klobuchar - beta. </summary>
		private float[] beta = new float[4];

		/// <summary>
		/// Healthmask field in this message is valid. </summary>
		private bool validHealth;

		/// <summary>
		/// UTC parameter fields in this message are valid. </summary>
		private bool validUTC;

		/// <summary>
		/// Klobuchar parameter fields in this message are valid. </summary>
		private bool validKlobuchar;

		/// <summary>
		/// Reference time. </summary>
		private Time refTime;

		public IonoGps()
		{

		}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public IonoGps(java.io.DataInputStream dai, boolean oldVersion) throws java.io.IOException
		public IonoGps(DataInputStream dai, bool oldVersion)
		{
			read(dai,oldVersion);
		}

		/// <summary>
		/// Gets the reference time.
		/// </summary>
		/// <returns> the refTime </returns>
		public virtual Time RefTime
		{
			get
			{
				return refTime;
			}
			set
			{
				this.refTime = value;
			}
		}


		/// <summary>
		/// Instantiates a new iono gps.
		/// </summary>
		public IonoGps(Time refTime)
		{
			this.refTime = refTime;
		}

		/// <summary>
		/// Gets the bitmask, every bit represenst a GPS SV (1-32).
		/// </summary>
		/// <returns> the health </returns>
		public virtual long Health
		{
			get
			{
				return health;
			}
			set
			{
				this.health = value;
			}
		}


		/// <summary>
		/// Gets the UTC - parameter A1.
		/// </summary>
		/// <returns> the utcA1 </returns>
		public virtual double UtcA1
		{
			get
			{
				return utcA1;
			}
			set
			{
				this.utcA1 = value;
			}
		}


		/// <summary>
		/// Gets the UTC - parameter A0.
		/// </summary>
		/// <returns> the utcA0 </returns>
		public virtual double UtcA0
		{
			get
			{
				return utcA0;
			}
			set
			{
				this.utcA0 = value;
			}
		}


		/// <summary>
		/// Gets the UTC - reference time of week.
		/// </summary>
		/// <returns> the utcTOW </returns>
		public virtual long UtcTOW
		{
			get
			{
				return utcTOW;
			}
			set
			{
				this.utcTOW = value;
			}
		}


		/// <summary>
		/// Gets the UTC - reference week number.
		/// </summary>
		/// <returns> the utcWNT </returns>
		public virtual int UtcWNT
		{
			get
			{
				return utcWNT;
			}
			set
			{
				this.utcWNT = value;
			}
		}


		/// <summary>
		/// Gets the UTC - time difference due to leap seconds before event.
		/// </summary>
		/// <returns> the utcLS </returns>
		public virtual int UtcLS
		{
			get
			{
				return utcLS;
			}
			set
			{
				this.utcLS = value;
			}
		}


		/// <summary>
		/// Gets the UTC - week number when next leap second event occurs.
		/// </summary>
		/// <returns> the utcWNF </returns>
		public virtual int UtcWNF
		{
			get
			{
				return utcWNF;
			}
			set
			{
				this.utcWNF = value;
			}
		}


		/// <summary>
		/// Gets the UTC - day of week when next leap second event occurs.
		/// </summary>
		/// <returns> the utcDN </returns>
		public virtual int UtcDN
		{
			get
			{
				return utcDN;
			}
			set
			{
				this.utcDN = value;
			}
		}


		/// <summary>
		/// Gets the UTC - time difference due to leap seconds after event.
		/// </summary>
		/// <returns> the utcLSF </returns>
		public virtual int UtcLSF
		{
			get
			{
				return utcLSF;
			}
			set
			{
				this.utcLSF = value;
			}
		}


		/// <summary>
		/// Gets the klobuchar - alpha.
		/// </summary>
		/// <param name="i"> the i<sup>th<sup> value in the range 0-3 </param>
		/// <returns> the alpha </returns>
		public virtual float getAlpha(int i)
		{
			return alpha[i];
		}

		/// <summary>
		/// Sets the klobuchar - alpha.
		/// </summary>
		/// <param name="alpha"> the alpha to set </param>
		public virtual float[] Alpha
		{
			set
			{
				this.alpha = value;
			}
		}

		/// <summary>
		/// Gets the klobuchar - beta.
		/// </summary>
		/// <param name="i"> the i<sup>th<sup> value in the range 0-3 </param>
		/// <returns> the beta </returns>
		public virtual float getBeta(int i)
		{
			return beta[i];
		}

		/// <summary>
		/// Sets the klobuchar - beta.
		/// </summary>
		/// <param name="beta"> the beta to set </param>
		public virtual float[] Beta
		{
			set
			{
				this.beta = value;
			}
		}

		/// <summary>
		/// Checks if is healthmask field in this message is valid.
		/// </summary>
		/// <returns> the validHealth </returns>
		public virtual bool ValidHealth
		{
			get
			{
				return validHealth;
			}
			set
			{
				this.validHealth = value;
			}
		}


		/// <summary>
		/// Checks if is UTC parameter fields in this message are valid.
		/// </summary>
		/// <returns> the validUTC </returns>
		public virtual bool ValidUTC
		{
			get
			{
				return validUTC;
			}
			set
			{
				this.validUTC = value;
			}
		}


		/// <summary>
		/// Checks if is klobuchar parameter fields in this message are valid.
		/// </summary>
		/// <returns> the validKlobuchar </returns>
		public virtual bool ValidKlobuchar
		{
			get
			{
				return validKlobuchar;
			}
			set
			{
				this.validKlobuchar = value;
			}
		}

		/* (non-Javadoc)
		 * @see org.gogpsproject.Streamable#write(java.io.DataOutputStream)
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public int write(java.io.DataOutputStream dos) throws java.io.IOException
		public virtual int write(DataOutputStream dos)
		{
			int size = 5;
			dos.writeUTF(org.gogpsproject.producer.Streamable_Fields.MESSAGE_IONO); // 5
			dos.writeInt(STREAM_V);
			size += 4;
			dos.writeLong(health);
			size += 8;
			dos.writeDouble(utcA1);
			size += 8;
			dos.writeDouble(utcA0);
			size += 8;
			dos.writeLong(utcTOW);
			size += 8;
			dos.writeInt(utcWNT);
			size += 4;
			dos.writeInt(utcLS);
			size += 4;
			dos.writeInt(utcWNF);
			size += 4;
			dos.writeInt(utcDN);
			size += 4;
			dos.writeInt(utcLSF);
			size += 4;
			for (int i = 0;i < alpha.Length;i++)
			{
				dos.writeFloat(alpha[i]);
				size += 4;
			}
			for (int i = 0;i < beta.Length;i++)
			{
				dos.writeFloat(beta[i]);
				size += 4;
			}
			dos.writeBoolean(validHealth);
			size += 1;
			dos.writeBoolean(validUTC);
			size += 1;
			dos.writeBoolean(validKlobuchar);
			size += 1;
			dos.writeLong(refTime == null? - 1:refTime.Msec);
			size += 8;

			return size;
		}
		/* (non-Javadoc)
		 * @see org.gogpsproject.Streamable#read(java.io.DataInputStream)
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void read(java.io.DataInputStream dai, boolean oldVersion) throws java.io.IOException
		public virtual void read(DataInputStream dai, bool oldVersion)
		{
			int v = 1;
			if (!oldVersion)
			{
				v = dai.readInt();
			}

			if (v == 1)
			{

				health = dai.readLong();
				utcA1 = dai.readDouble();
				utcA0 = dai.readDouble();
				utcTOW = dai.readLong();
				utcWNT = dai.readInt();
				utcLS = dai.readInt();
				utcWNF = dai.readInt();
				utcDN = dai.readInt();
				utcLSF = dai.readInt();
				for (int i = 0;i < alpha.Length;i++)
				{
					alpha[i] = dai.readFloat();
				}
				for (int i = 0;i < beta.Length;i++)
				{
					beta[i] = dai.readFloat();
				}
				validHealth = dai.readBoolean();
				validUTC = dai.readBoolean();
				validKlobuchar = dai.readBoolean();
				long l = dai.readLong();
				refTime = new Time(l > 0?l:DateTimeHelperClass.CurrentUnixTimeMillis());
			}
			else
			{
				throw new IOException("Unknown format version:" + v);
			}
		}


	}

}