using System;
using System.Collections.Generic;

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

	using Time = org.gogpsproject.positioning.Time;

	/// <summary>
	/// <para>
	/// Observations class
	/// </para>
	/// 
	/// @author Eugenio Realini, Cryms.com
	/// </summary>
	public class Observations : Streamable
	{

		internal SimpleDateFormat sdfHeader = GMTdf;
		internal DecimalFormat dfX4 = new DecimalFormat("0.0000");

		private const int STREAM_V = 1;

		private Time refTime; // Reference time of the dataset
		private int eventFlag; // Event flag

		private List<ObservationSet> obsSet; // sets of observations
		private int issueOfData = -1;
	  public int index;

		/// <summary>
		/// The Rinex filename
		/// </summary>
		public string rinexFileName;

		public static SimpleDateFormat GMTdf
		{
			get
			{
			  SimpleDateFormat sdfHeader = new SimpleDateFormat("dd-MMM-yy HH:mm:ss");
			sdfHeader.TimeZone = TimeZone.getTimeZone("GMT");
			return sdfHeader;
			}
		}

		public virtual object clone()
		{
			try
			{
				ByteArrayOutputStream baos = new ByteArrayOutputStream();
				this.write(new DataOutputStream(baos));
				DataInputStream dis = new DataInputStream(new ByteArrayInputStream(baos.toByteArray()));
				baos.reset();
				dis.readUTF();
				return new Observations(dis, false);
			}
			catch (IOException ioe)
			{
				Console.WriteLine(ioe.ToString());
				Console.Write(ioe.StackTrace);
			}
			return null;
		}

		public Observations(Time time, int flag)
		{
			this.refTime = time;
			this.eventFlag = flag;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Observations(java.io.DataInputStream dai, boolean oldVersion) throws java.io.IOException
		public Observations(DataInputStream dai, bool oldVersion)
		{
			read(dai, oldVersion);
		}

		public virtual void cleanObservations()
		{
			if (obsSet != null)
			{
				for (int i = obsSet.Count - 1;i >= 0;i--)
				{
					if (obsSet[i] == null || double.IsNaN(obsSet[i].getPseudorange(0)))
					{
						obsSet.RemoveAt(i);
					}
				}
			}
		}

		public virtual int NumSat
		{
			get
			{
				if (obsSet == null)
				{
					return 0;
				}
				int nsat = 0;
				for (int i = 0;i < obsSet.Count;i++)
				{
					if (obsSet[i] != null)
					{
						nsat++;
					}
				}
				return obsSet == null? - 1:nsat;
			}
		}

		public virtual ObservationSet getSatByIdx(int idx)
		{
			return obsSet[idx];
		}

		public virtual ObservationSet getSatByID(int? satID)
		{
			if (obsSet == null || satID == null)
			{
				return null;
			}
			for (int i = 0;i < obsSet.Count;i++)
			{
				if (obsSet[i] != null && obsSet[i].SatID == (int)satID)
				{
					return obsSet[i];
				}
			}
			return null;
		}

		public virtual ObservationSet getSatByIDType(int? satID, char satType)
		{
			if (obsSet == null || satID == null)
			{
				return null;
			}
			for (int i = 0;i < obsSet.Count;i++)
			{
				if (obsSet[i] != null && obsSet[i].SatID == (int)satID && obsSet[i].SatType == satType)
				{
					return obsSet[i];
				}
			}
			return null;
		}

	//	public ObservationSet getGpsByID(char satGnss){
	//		String sub = String.valueOf(satGnss); 
	//		String str = sub.substring(0, 1);  
	//		char satType = str.charAt(0);
	//		sub = sub.substring(1, 3);  
	//		Integer satID = Integer.parseInt(sub);
	//		
	//		if(gps == null || satID==null) return null;
	//		for(int i=0;i<gps.size();i++)
	//			if(gps.get(i)!=null && gps.get(i).getSatID()==satID.intValue() && gps.get(i).getSatType()==satType) return gps.get(i);
	//		return null;
	//	}

		public virtual int? getSatID(int idx)
		{
			return getSatByIdx(idx).SatID;
		}

		public virtual char getGnssType(int idx)
		{
			return getSatByIdx(idx).SatType;
		}

		public virtual bool containsSatID(int? id)
		{
			return getSatByID(id) != null;
		}

		public virtual bool containsSatIDType(int? id, char? satType)
		{
			return getSatByIDType(id, satType) != null;
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


		/// <summary>
		/// Epoch flag
		/// 0: OK
		/// 1: power failure between previous and current epoch
		/// >1: Special event
		///  2: start moving antenna
		///  3: new site occupation
		///  (end of kinem. data)
		/// (at least MARKER NAME record
		/// follows)
		/// 4: header information follows
		/// 5: external event (epoch is significant)
		/// 6: cycle slip records follow
		/// to optionally report detected
		/// and repaired cycle slips
		/// (same format as OBSERVATIONS
		/// records; slip instead of observation;
		/// LLI and signal strength blank)
		/// </summary>
		/// <returns> the eventFlag </returns>
		public virtual int EventFlag
		{
			get
			{
				return eventFlag;
			}
			set
			{
				this.eventFlag = value;
			}
		}


	//	public void init(int nGps, int nGlo, int nSbs){
	//		gpsSat = new ArrayList<Integer>(nGps);
	//		gloSat = new ArrayList<Integer>(nGlo);
	//		sbsSat = new ArrayList<Integer>(nSbs);
	//
	//		// Allocate array of observation objects
	//		if (nGps > 0) gps = new ObservationSet[nGps];
	//		if (nGlo > 0) glo = new ObservationSet[nGlo];
	//		if (nSbs > 0) sbs = new ObservationSet[nSbs];
	//	}

		public virtual void setGps(int i, ObservationSet os)
		{
			if (obsSet == null)
			{
				obsSet = new List<ObservationSet>(i + 1);
			}
			if (i == obsSet.Count)
			{
				obsSet.Add(os);
			}
			else
			{
				int c = obsSet.Count;
				while (c++<=i)
				{
					obsSet.Add(null);
				}
				obsSet[i] = os;
			}
			//gps[i] = os;
			//gpsSat.add(os.getSatID());
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int write(java.io.DataOutputStream dos) throws java.io.IOException
		public virtual int write(DataOutputStream dos)
		{
			dos.writeUTF(Streamable_Fields.MESSAGE_OBSERVATIONS); // 5
			dos.writeInt(STREAM_V); // 4
			dos.writeLong(refTime == null? - 1:refTime.Msec); // 13
			dos.writeDouble(refTime == null? - 1:refTime.Fraction);
			dos.write(eventFlag); // 14
			dos.write(obsSet == null?0:obsSet.Count); // 15
			int size = 19;
			if (obsSet != null)
			{
				for (int i = 0;i < obsSet.Count;i++)
				{
					size += ((ObservationSet)obsSet[i]).write(dos);
				}
			}
			return size;
		}

		public override string ToString()
		{

			string lineBreak = System.getProperty("line.separator");

			string @out = " GPS Time:" + RefTime.GpsTime + " " + sdfHeader.format(new DateTime(RefTime.Msec)) + " evt:" + eventFlag + lineBreak;
			for (int i = 0;i < NumSat;i++)
			{
				ObservationSet os = getSatByIdx(i);
				@out += "satType:" + os.SatType + "  satID:" + os.SatID + "\tC:" + fd(os.getCodeC(0)) + " cP:" + fd(os.getCodeP(0)) + " Ph:" + fd(os.getPhaseCycles(0)) + " Dp:" + fd(os.getDoppler(0)) + " Ss:" + fd(os.getSignalStrength(0)) + " LL:" + fd(os.getLossLockInd(0)) + " LL2:" + fd(os.getLossLockInd(1)) + lineBreak;
			}
			return @out;
		}

		private string fd(double n)
		{
			return double.IsNaN(n)?"NaN":dfX4.format(n);
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
				refTime = new Time(dai.readLong(), dai.readDouble());
				eventFlag = dai.read();
				int size = dai.read();
				obsSet = new List<ObservationSet>(size);

				for (int i = 0;i < size;i++)
				{
					if (!oldVersion)
					{
						dai.readUTF();
					}
					ObservationSet os = new ObservationSet(dai, oldVersion);
					obsSet.Add(os);
				}
			}
			else
			{
				throw new IOException("Unknown format version:" + v);
			}
		}

		public virtual int IssueOfData
		{
			set
			{
				this.issueOfData = value;
			}
			get
			{
				return this.issueOfData;
			}
		}

	}

}