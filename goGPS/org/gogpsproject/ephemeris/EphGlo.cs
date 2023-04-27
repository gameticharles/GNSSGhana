/*
 * Copyright (c) 2010, Eugenio Realini, Mirko Reguzzoni, Cryms sagl, Daisuke Yoshida. All Rights Reserved.
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

    public class EphGlo : Streamable
	{
		private const int STREAM_V = 1;

		private Time refTime; // Reference time of the dataset
		private char satType; // Satellite Type
		private int satID; // Satellite ID number
		private int week; // GPS week number
		private double toc; // clock data reference time
		private double tow;
		private double toe; // ephemeris reference time

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
		private double freq_num;
		private double tb;

		private double Z_Renamed;
		private double Zv_Renamed;
		private double Za_Renamed;
		private double En_Renamed;


		public EphGlo()
		{

		}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public EphGlo(java.io.DataInputStream dai, boolean oldVersion) throws java.io.IOException
		public EphGlo(ObjectInputStream dai, bool oldVersion)
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
		/// <returns> the tow </returns>
		public virtual double Tow
		{
			get
			{
				return tow;
			}
			set
			{
				this.tow = value;
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

		public virtual double getfreq_num()
		{
			return freq_num;
		}
		public virtual void setfreq_num(double freq_num)
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

	//		dos.writeInt(L2Code); size +=4;
	//		dos.writeInt(L2Flag); size +=4;
	//
	//		dos.writeInt(svAccur); size +=4;
	//		dos.writeInt(svHealth); size +=4;
	//
	//		dos.writeInt(iode); size +=4;
	//		dos.writeInt(iodc); size +=4;
	//
	//		dos.writeDouble(toc); size +=8;
	//		dos.writeDouble(toe); size +=8;
	//
	//		dos.writeDouble(af0); size +=8;
	//		dos.writeDouble(af1); size +=8;
	//		dos.writeDouble(af2); size +=8;
	//		dos.writeDouble(tgd); size +=8;
	//
	//
	//		dos.writeDouble(rootA); size +=8;
	//		dos.writeDouble(e); size +=8;
	//		dos.writeDouble(i0); size +=8;
	//		dos.writeDouble(iDot); size +=8;
	//		dos.writeDouble(omega); size +=8;
	//		dos.writeDouble(omega0); size +=8;
	//
	//		dos.writeDouble(omegaDot); size +=8;
	//		dos.writeDouble(M0); size +=8;
	//		dos.writeDouble(deltaN); size +=8;
	//		dos.writeDouble(crc); size +=8;
	//		dos.writeDouble(crs); size +=8;
	//		dos.writeDouble(cuc); size +=8;
	//		dos.writeDouble(cus); size +=8;
	//		dos.writeDouble(cic); size +=8;
	//		dos.writeDouble(cis); size +=8;
	//
	//		dos.writeDouble(fitInt); size +=8;

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
	//			L2Code = dai.readInt();
	//			L2Flag = dai.readInt();
	//			svAccur = dai.readInt();
	//			svHealth = dai.readInt();
	//			iode = dai.readInt();
	//			iodc = dai.readInt();
	//			toc = dai.readDouble();
	//			toe = dai.readDouble();
	//			af0 = dai.readDouble();
	//			af1 = dai.readDouble();
	//			af2 = dai.readDouble();
	//			tgd = dai.readDouble();
	//			rootA = dai.readDouble();
	//			e = dai.readDouble();
	//			i0 = dai.readDouble();
	//			iDot = dai.readDouble();
	//			omega = dai.readDouble();
	//			omega0 = dai.readDouble();
	//			omegaDot = dai.readDouble();
	//			M0 = dai.readDouble();
	//			deltaN = dai.readDouble();
	//			crc = dai.readDouble();
	//			crs = dai.readDouble();
	//			cuc = dai.readDouble();
	//			cus = dai.readDouble();
	//			cic = dai.readDouble();
	//			cis = dai.readDouble();
	//			fitInt = dai.readDouble();
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