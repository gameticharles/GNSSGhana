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
namespace org.gogpsproject.producer
{


	/// <summary>
	/// <para>
	/// Set of observations for one epoch and one satellite
	/// </para>
	/// 
	/// @author Eugenio Realini, Cryms.com
	/// </summary>
	public class ObservationSet : Streamable
	{

		private const int STREAM_V = 1;


		public const int L1 = 0;
		public const int L2 = 1;


		private int satID; // Satellite number
		private char satType; // Satellite Type

		/* Array of [L1,L2] */
		private double[] codeC = new double[] {double.NaN,double.NaN}; // C Coarse/Acquisition (C/A) code [m]
		private double[] codeP = new double[] {double.NaN,double.NaN}; // P Code Pseudorange [m]
		private double[] phase = new double[] {double.NaN,double.NaN}; // L Carrier Phase [cycle]
		private float[] signalStrength = new float[] {float.NaN,float.NaN}; // C/N0 (signal strength) [dBHz]
		private float[] doppler = new float[] {float.NaN,float.NaN}; // Doppler value [Hz]

		private int[] qualityInd = new int[] {-1,-1}; // Nav Measurements Quality Ind. ublox proprietary?

		/*
		 * Loss of lock indicator (LLI). Range: 0-7
		 *  0 or blank: OK or not known
		 *  Bit 0 set : Lost lock between previous and current observation: cycle slip possible
		 *  Bit 1 set : Opposite wavelength factor to the one defined for the satellite by a previous WAVELENGTH FACT L1/2 line. Valid for the current epoch only.
		 *  Bit 2 set : Observation under Antispoofing (may suffer from increased noise)
		 * Bits 0 and 1 for phase only.
		 */
		private int[] lossLockInd = new int[] {-1,-1};

		/*
		 * Signal strength indicator projected into interval 1-9:
		 *  1: minimum possible signal strength
		  *  5: threshold for good S/N ratio
		  *  9: maximum possible signal strength
		  * 0 or blank: not known, don't care
		 */
		private int[] signalStrengthInd = new int[] {-1,-1};

		private int freqNum;

		/* Sets whether this obs is in use or not:
			 could be below the elevation threshold for example
		 or unhealthy
	  */
		private bool inUse_Renamed = false;

	  /* residual error */
	  public double eRes;

	  /// <summary>
	  /// topocentric elevation
	  /// </summary>
	  public double el;

		public ObservationSet()
		{
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ObservationSet(java.io.DataInputStream dai, boolean oldVersion) throws java.io.IOException
		public ObservationSet(DataInputStream dai, bool oldVersion)
		{
			read(dai,oldVersion);
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


		/// <returns> the phase range (in meters) </returns>
		public virtual double getPhaserange(int i)
		{
			return phase[i] * getWavelength(i);
		}

		public virtual double getWavelength(int i)
		{
			double frequency = 0;
			switch (this.satType)
			{
			case 'G':
				frequency = (i == 0)?Constants.FL1:Constants.FL2;
				goto case 'R';
			case 'R':
				frequency = (i == 0)?freqNum * Constants.FR1_delta + Constants.FR1_base:freqNum * Constants.FR2_delta + Constants.FR2_base;
				goto case 'E';
			case 'E':
				frequency = (i == 0)?Constants.FE1:Constants.FE5a;
				goto case 'C';
			case 'C':
				frequency = (i == 0)?Constants.FC2:Constants.FC5b;
				goto case 'J';
			case 'J':
				frequency = (i == 0)?Constants.FJ1:Constants.FJ2;
			break;
			}
			return Constants.SPEED_OF_LIGHT / frequency;
		}

		/// <returns> the pseudorange (in meters) </returns>
		public virtual double getPseudorange(int i)
		{
			return double.IsNaN(codeP[i])?codeC[i]:codeP[i];
		}

		public virtual bool isPseudorangeP(int i)
		{
			return !double.IsNaN(codeP[i]);
		}

		/// <returns> the c </returns>
		public virtual double getCodeC(int i)
		{
			return codeC[i];
		}

		/// <param name="c"> the c to set </param>
		public virtual void setCodeC(int i, double c)
		{
			codeC[i] = c;
		}

		/// <returns> the p </returns>
		public virtual double getCodeP(int i)
		{
			return codeP[i];
		}

		/// <param name="p"> the p to set </param>
		public virtual void setCodeP(int i, double p)
		{
			codeP[i] = p;
		}

		/// <returns> the l </returns>
		public virtual double getPhaseCycles(int i)
		{
			return phase[i];
		}

		/// <param name="l"> the l to set </param>
		public virtual void setPhaseCycles(int i, double l)
		{
			phase[i] = l;
		}

		/// <returns> the s </returns>
		public virtual float getSignalStrength(int i)
		{
			return signalStrength[i];
		}

		/// <param name="s"> the s to set </param>
		public virtual void setSignalStrength(int i, float s)
		{
			signalStrength[i] = s;
		}

		/// <returns> the d </returns>
		public virtual float getDoppler(int i)
		{
			return doppler[i];
		}

		/// <param name="d"> the d to set </param>
		public virtual void setDoppler(int i, float d)
		{
			doppler[i] = d;
		}

		/* (non-Javadoc)
		 * @see java.lang.Object#equals(java.lang.Object)
		 */
		public override bool Equals(object obj)
		{
			if (obj is ObservationSet)
			{
				return ((ObservationSet)obj).SatID == satID;
			}
			else
			{
				return base.Equals(obj);
			}
		}

		/// <returns> the qualityInd </returns>
		public virtual int getQualityInd(int i)
		{
			return qualityInd[i];
		}

		/// <param name="qualityInd"> the qualityInd to set </param>
		public virtual void setQualityInd(int i, int qualityInd)
		{
			this.qualityInd[i] = qualityInd;
		}

		/// <returns> the lossLockInd </returns>
		public virtual int getLossLockInd(int i)
		{
			return lossLockInd[i];
		}

		/// <param name="lossLockInd"> the lossLockInd to set </param>
		public virtual void setLossLockInd(int i, int lossLockInd)
		{
			this.lossLockInd[i] = lossLockInd;
		}

		public virtual bool isLocked(int i)
		{
			return lossLockInd[i] == 0;
		}
		public virtual bool isPossibleCycleSlip(int i)
		{
			return lossLockInd[i] > 0 && ((lossLockInd[i] & 0x1) == 0x1);
		}
		public virtual bool isHalfWavelength(int i)
		{
			return lossLockInd[i] > 0 && ((lossLockInd[i] & 0x2) == 0x2);
		}
		public virtual bool isUnderAntispoof(int i)
		{
			return lossLockInd[i] > 0 && ((lossLockInd[i] & 0x4) == 0x4);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int write(java.io.DataOutputStream dos) throws java.io.IOException
		public virtual int write(DataOutputStream dos)
		{
			int size = 0;
			dos.writeUTF(Streamable_Fields.MESSAGE_OBSERVATIONS_SET); // 5

			dos.writeInt(STREAM_V);
			size += 4;
			dos.write(satID); // 1
			size += 1;
			dos.write(satType); // 1
			size += 1;
			// L1 data
			dos.write((sbyte)qualityInd[L1]);
			size += 1;
			dos.write((sbyte)lossLockInd[L1]);
			size += 1;
			dos.writeDouble(codeC[L1]);
			size += 8;
			dos.writeDouble(codeP[L1]);
			size += 8;
			dos.writeDouble(phase[L1]);
			size += 8;
			dos.writeFloat(signalStrength[L1]);
			size += 4;
			dos.writeFloat(doppler[L1]);
			size += 4;
			// write L2 data ?
			bool hasL2 = false;
			if (!double.IsNaN(codeC[L2]))
			{
				hasL2 = true;
			}
			if (!double.IsNaN(codeP[L2]))
			{
				hasL2 = true;
			}
			if (!double.IsNaN(phase[L2]))
			{
				hasL2 = true;
			}
			if (!float.IsNaN(signalStrength[L2]))
			{
				hasL2 = true;
			}
			if (!float.IsNaN(doppler[L2]))
			{
				hasL2 = true;
			}
			dos.writeBoolean(hasL2);
			size += 1;
			if (hasL2)
			{
				dos.write((sbyte)qualityInd[L2]);
				size += 1;
				dos.write((sbyte)lossLockInd[L2]);
				size += 1;
				dos.writeDouble(codeC[L2]);
				size += 8;
				dos.writeDouble(codeP[L2]);
				size += 8;
				dos.writeDouble(phase[L2]);
				size += 8;
				dos.writeFloat(signalStrength[L2]);
				size += 4;
				dos.writeFloat(doppler[L2]);
				size += 4;
			}
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
				satID = dai.read();
				satType = (char) dai.read();

				// L1 data
				qualityInd[L1] = (int)dai.read();
				if (qualityInd[L1] == 255)
				{
					qualityInd[L1] = -1;
				}
				lossLockInd[L1] = (int)dai.read();
				if (lossLockInd[L1] == 255)
				{
					lossLockInd[L1] = -1;
				}
				codeC[L1] = dai.readDouble();
				codeP[L1] = dai.readDouble();
				phase[L1] = dai.readDouble();
				signalStrength[L1] = dai.readFloat();
				doppler[L1] = dai.readFloat();
				if (dai.readBoolean())
				{
					// L2 data
					qualityInd[L2] = (int)dai.read();
					if (qualityInd[L2] == 255)
					{
						qualityInd[L2] = -1;
					}
					lossLockInd[L2] = (int)dai.read();
					if (lossLockInd[L2] == 255)
					{
						lossLockInd[L2] = -1;
					}
					codeC[L2] = dai.readDouble();
					codeP[L2] = dai.readDouble();
					phase[L2] = dai.readDouble();
					signalStrength[L2] = dai.readFloat();
					doppler[L2] = dai.readFloat();
				}
			}
			else
			{
				throw new IOException("Unknown format version:" + v);
			}
		}

		/// <param name="signalStrengthInd"> the signalStrengthInd to set </param>
		public virtual void setSignalStrengthInd(int i, int signalStrengthInd)
		{
			this.signalStrengthInd[i] = signalStrengthInd;
		}

		/// <returns> the signalStrengthInd </returns>
		public virtual int getSignalStrengthInd(int i)
		{
			return signalStrengthInd[i];
		}

		/// <param name="signalStrengthInd"> the signalStrengthInd to set </param>
		public virtual int FreqNum
		{
			set
			{
				this.freqNum = value;
			}
		}

		/// <returns> the signalStrengthInd </returns>
		public virtual int getFreqNum(int i)
		{
			return freqNum;
		}

	  public virtual bool inUse()
	  {
		return InUse;
	  }

	  public virtual void inUse(bool inUse)
	  {
		this.InUse = inUse;
	  }

	  public virtual bool InUse
	  {
		  get
		  {
			return inUse_Renamed;
		  }
		  set
		  {
			this.inUse_Renamed = value;
		  }
	  }

	}
}