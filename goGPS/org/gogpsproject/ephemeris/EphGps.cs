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
namespace org.gogpsproject.ephemeris
{


    using Time = org.gogpsproject.positioning.Time;
    using Streamable = org.gogpsproject.producer.Streamable;
    using java.io;
    using org.gogpsproject.producer;
    using System;
    using System.IO;

    /// <summary>
    /// <para>
    /// GPS broadcast ephemerides
    /// </para>
    /// 
    /// @author Eugenio Realini, Cryms.com, Daisuke Yoshida 
    /// </summary>

    public class EphGps : Streamable
	{
		private const int STREAM_V = 1;

		private Time refTime; // Reference time of the dataset
		private char satType; // Satellite Type
		private int satID; // Satellite ID number
		private int week; // GPS week number

		private int L2Code_Renamed; // Code on L2
		private int L2Flag_Renamed; // L2 P data flag

		private int svAccur; // SV accuracy (URA index)
		private int svHealth; // SV health

		private int iode; // Issue of data (ephemeris)
		private int iodc; // Issue of data (clock)

		private double toc; // clock data reference time
		private double toe; // ephemeris reference time
		private double tom; // transmission time of message

		/* satellite clock parameters */
		private double af0;
		private double af1;
		private double af2;
		private double tgd;

		/* satellite orbital parameters */
		private double rootA; // Square root of the semimajor axis
		private double e; // Eccentricity
		private double i0; // Inclination angle at reference time
		private double iDot; // Rate of inclination angle
		private double omega; // Argument of perigee
		private double omega0; /*
		 * Longitude of ascending node of orbit plane at beginning
		 * of week
		 */
		private double omegaDot; // Rate of right ascension
		private double M0_Renamed; // Mean anomaly at reference time
		private double deltaN; // Mean motion difference from computed value
		private double crc, crs, cuc, cus, cic, cis; /*
		 * Amplitude of second-order harmonic
		 * perturbations
		 */
		private long fitInt; // Fit interval

		/* for GLONASS data */
		private float tow;

		private float tauN;
		private float gammaN;
		private double tk;

		private double X_Renamed;
		private double Xv_Renamed;
		private double Xa_Renamed;
		private double Bn_Renamed;

		private double Y_Renamed;
		private double Yv_Renamed;
		private double Ya_Renamed;
		private int freq_num;
		private double tb;

		private double Z_Renamed;
		private double Zv_Renamed;
		private double Za_Renamed;
		private double En_Renamed;

	  public static readonly EphGps UnhealthyEph = new EphGps();


		public EphGps()
		{

		}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public EphGps(java.io.DataInputStream dai, boolean oldVersion) throws java.io.IOException
		public EphGps(ObjectInputStream dai, bool oldVersion)
		{
			read(dai,oldVersion);
		}

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
		/// <returns> the week </returns>
		public virtual int Week
		{
			get
			{
				return week;
			}
			set
			{
				this.week = value;
			}
		}
		/// <returns> the l2Code </returns>
		public virtual int L2Code
		{
			get
			{
				return L2Code_Renamed;
			}
			set
			{
				L2Code_Renamed = value;
			}
		}
		/// <returns> the l2Flag </returns>
		public virtual int L2Flag
		{
			get
			{
				return L2Flag_Renamed;
			}
			set
			{
				L2Flag_Renamed = value;
			}
		}
		/// <returns> the svAccur </returns>
		public virtual int SvAccur
		{
			get
			{
				return svAccur;
			}
			set
			{
				this.svAccur = value;
			}
		}
		/// <returns> the svHealth </returns>
		public virtual int SvHealth
		{
			get
			{
				return svHealth;
			}
			set
			{
				this.svHealth = value;
			}
		}
		/// <returns> the iode </returns>
		public virtual int Iode
		{
			get
			{
				return iode;
			}
			set
			{
				this.iode = value;
			}
		}
		/// <returns> the iodc </returns>
		public virtual int Iodc
		{
			get
			{
				return iodc;
			}
			set
			{
				this.iodc = value;
			}
		}
		/// <returns> the toc </returns>
		public virtual double Toc
		{
			get
			{
				return toc;
			}
			set
			{
				this.toc = value;
			}
		}
		/// <returns> the toe </returns>
		public virtual double Toe
		{
			get
			{
				return toe;
			}
			set
			{
				this.toe = value;
			}
		}
		/// <returns> the tom </returns>
		public virtual double Tom
		{
			get
			{
				return tom;
			}
			set
			{
				this.tom = value;
			}
		}
		/// <returns> the af0 </returns>
		public virtual double Af0
		{
			get
			{
				return af0;
			}
			set
			{
				this.af0 = value;
			}
		}
		/// <returns> the af1 </returns>
		public virtual double Af1
		{
			get
			{
				return af1;
			}
			set
			{
				this.af1 = value;
			}
		}
		/// <returns> the af2 </returns>
		public virtual double Af2
		{
			get
			{
				return af2;
			}
			set
			{
				this.af2 = value;
			}
		}
		/// <returns> the tgd </returns>
		public virtual double Tgd
		{
			get
			{
				return tgd;
			}
			set
			{
				this.tgd = value;
			}
		}
		/// <returns> the rootA </returns>
		public virtual double RootA
		{
			get
			{
				return rootA;
			}
			set
			{
				this.rootA = value;
			}
		}
		/// <returns> the e </returns>
		public virtual double E
		{
			get
			{
				return e;
			}
			set
			{
				this.e = value;
			}
		}
		/// <returns> the i0 </returns>
		public virtual double I0
		{
			get
			{
				return i0;
			}
			set
			{
				this.i0 = value;
			}
		}
		/// <returns> the iDot </returns>
		public virtual double getiDot()
		{
			return iDot;
		}
		/// <param name="iDot"> the iDot to set </param>
		public virtual void setiDot(double iDot)
		{
			this.iDot = iDot;
		}
		/// <returns> the omega </returns>
		public virtual double Omega
		{
			get
			{
				return omega;
			}
			set
			{
				this.omega = value;
			}
		}
		/// <returns> the omega0 </returns>
		public virtual double Omega0
		{
			get
			{
				return omega0;
			}
			set
			{
				this.omega0 = value;
			}
		}
		/// <returns> the omegaDot </returns>
		public virtual double OmegaDot
		{
			get
			{
				return omegaDot;
			}
			set
			{
				this.omegaDot = value;
			}
		}
		/// <returns> the m0 </returns>
		public virtual double M0
		{
			get
			{
				return M0_Renamed;
			}
			set
			{
				M0_Renamed = value;
			}
		}
		/// <returns> the deltaN </returns>
		public virtual double DeltaN
		{
			get
			{
				return deltaN;
			}
			set
			{
				this.deltaN = value;
			}
		}
		/// <returns> the crc </returns>
		public virtual double Crc
		{
			get
			{
				return crc;
			}
			set
			{
				this.crc = value;
			}
		}
		/// <returns> the crs </returns>
		public virtual double Crs
		{
			get
			{
				return crs;
			}
			set
			{
				this.crs = value;
			}
		}
		/// <returns> the cuc </returns>
		public virtual double Cuc
		{
			get
			{
				return cuc;
			}
			set
			{
				this.cuc = value;
			}
		}
		/// <returns> the cus </returns>
		public virtual double Cus
		{
			get
			{
				return cus;
			}
			set
			{
				this.cus = value;
			}
		}
		/// <returns> the cic </returns>
		public virtual double Cic
		{
			get
			{
				return cic;
			}
			set
			{
				this.cic = value;
			}
		}
		/// <returns> the cis </returns>
		public virtual double Cis
		{
			get
			{
				return cis;
			}
			set
			{
				this.cis = value;
			}
		}
		/// <returns> the fitInt </returns>
		public virtual long FitInt
		{
			get
			{
				return fitInt;
			}
			set
			{
				this.fitInt = value;
			}
		}


		/* for GLONASS data */

		public virtual float TauN
		{
			get
			{
				return tauN;
			}
			set
			{
				this.tauN = value;
			}
		}

		public virtual float GammaN
		{
			get
			{
				return gammaN;
			}
			set
			{
				this.gammaN = value;
			}
		}

		public virtual double gettk()
		{
			return tk;
		}
		public virtual void settk(double tk)
		{
			this.tk = tk;
		}

		public virtual double X
		{
			get
			{
				return X_Renamed;
			}
			set
			{
				this.X_Renamed = value;
			}
		}

		public virtual double Xv
		{
			get
			{
				return Xv_Renamed;
			}
			set
			{
				this.Xv_Renamed = value;
			}
		}

		public virtual double Xa
		{
			get
			{
				return Xa_Renamed;
			}
			set
			{
				this.Xa_Renamed = value;
			}
		}

		public virtual double Bn
		{
			get
			{
				return Bn_Renamed;
			}
			set
			{
				this.Bn_Renamed = value;
			}
		}

		public virtual double Y
		{
			get
			{
				return Y_Renamed;
			}
			set
			{
				this.Y_Renamed = value;
			}
		}

		public virtual double Yv
		{
			get
			{
				return Yv_Renamed;
			}
			set
			{
				this.Yv_Renamed = value;
			}
		}

		public virtual double Ya
		{
			get
			{
				return Ya_Renamed;
			}
			set
			{
				this.Ya_Renamed = value;
			}
		}

		public virtual int getfreq_num()
		{
			return freq_num;
		}
		public virtual void setfreq_num(int freq_num)
		{
			this.freq_num = freq_num;
		}

		public virtual double gettb()
		{
			return tb;
		}
		public virtual void settb(double tb)
		{
			this.tb = tb;
		}

		public virtual double Z
		{
			get
			{
				return Z_Renamed;
			}
			set
			{
				this.Z_Renamed = value;
			}
		}

		public virtual double Zv
		{
			get
			{
				return Zv_Renamed;
			}
			set
			{
				this.Zv_Renamed = value;
			}
		}

		public virtual double Za
		{
			get
			{
				return Za_Renamed;
			}
			set
			{
				this.Za_Renamed = value;
			}
		}

		public virtual double En
		{
			get
			{
				return En_Renamed;
			}
			set
			{
				this.En_Renamed = value;
			}
		}

		//	public long getEn() {
		//		return En;
		//	}
		//	public void setEn(long En) {
		//		this.En = En;
		//	}


		/* (non-Javadoc)
		 * @see org.gogpsproject.Streamable#write(java.io.DataOutputStream)
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public int write(java.io.DataOutputStream dos) throws java.io.IOException
		public virtual int write(ObjectOutputStream dos)
		{
			int size = 5;
			dos.writeUTF(org.gogpsproject.producer.Streamable_Fields.MESSAGE_EPHEMERIS); // 5
			dos.writeInt(STREAM_V); // 4
			size += 4;

			dos.writeLong(refTime == null? - 1:refTime.Msec);
			size += 8;
			dos.write(satID);
			size += 1;
			dos.writeInt(week);
			size += 4;

			dos.writeInt(L2Code_Renamed);
			size += 4;
			dos.writeInt(L2Flag_Renamed);
			size += 4;

			dos.writeInt(svAccur);
			size += 4;
			dos.writeInt(svHealth);
			size += 4;

			dos.writeInt(iode);
			size += 4;
			dos.writeInt(iodc);
			size += 4;

			dos.writeDouble(toc);
			size += 8;
			dos.writeDouble(toe);
			size += 8;

			dos.writeDouble(af0);
			size += 8;
			dos.writeDouble(af1);
			size += 8;
			dos.writeDouble(af2);
			size += 8;
			dos.writeDouble(tgd);
			size += 8;


			dos.writeDouble(rootA);
			size += 8;
			dos.writeDouble(e);
			size += 8;
			dos.writeDouble(i0);
			size += 8;
			dos.writeDouble(iDot);
			size += 8;
			dos.writeDouble(omega);
			size += 8;
			dos.writeDouble(omega0);
			size += 8;

			dos.writeDouble(omegaDot);
			size += 8;
			dos.writeDouble(M0_Renamed);
			size += 8;
			dos.writeDouble(deltaN);
			size += 8;
			dos.writeDouble(crc);
			size += 8;
			dos.writeDouble(crs);
			size += 8;
			dos.writeDouble(cuc);
			size += 8;
			dos.writeDouble(cus);
			size += 8;
			dos.writeDouble(cic);
			size += 8;
			dos.writeDouble(cis);
			size += 8;

			dos.writeDouble(fitInt);
			size += 8;

			return size;
		}
		/* (non-Javadoc)
		 * @see org.gogpsproject.Streamable#read(java.io.DataInputStream)
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void read(java.io.DataInputStream dai, boolean oldVersion) throws java.io.IOException
		public virtual void read(ObjectInputStream dai, bool oldVersion)
		{
			int v = 1;
			if (!oldVersion)
			{
				v = dai.readInt();
			}

			if (v == 1)
			{
				long l = dai.readLong();
				refTime = new Time(l > 0?l:DateTimeHelperClass.CurrentUnixTimeMillis());
				satID = dai.read();
				week = dai.readInt();
				L2Code_Renamed = dai.readInt();
				L2Flag_Renamed = dai.readInt();
				svAccur = dai.readInt();
				svHealth = dai.readInt();
				iode = dai.readInt();
				iodc = dai.readInt();
				toc = dai.readDouble();
				toe = dai.readDouble();
				af0 = dai.readDouble();
				af1 = dai.readDouble();
				af2 = dai.readDouble();
				tgd = dai.readDouble();
				rootA = dai.readDouble();
				e = dai.readDouble();
				i0 = dai.readDouble();
				iDot = dai.readDouble();
				omega = dai.readDouble();
				omega0 = dai.readDouble();
				omegaDot = dai.readDouble();
				M0_Renamed = dai.readDouble();
				deltaN = dai.readDouble();
				crc = dai.readDouble();
				crs = dai.readDouble();
				cuc = dai.readDouble();
				cus = dai.readDouble();
				cic = dai.readDouble();
				cis = dai.readDouble();
				fitInt = dai.readLong();
			}
			else
			{
				throw new IOException("Unknown format version:" + v);
			}
		}

        int Streamable.write(ObjectInputStream dos)
        {
            throw new NotImplementedException();
        }

        void Streamable.read(ObjectInputStream dai, bool oldVersion)
        {
            throw new NotImplementedException();
        }
    }

}