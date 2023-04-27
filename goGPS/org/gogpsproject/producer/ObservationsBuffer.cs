using System;
using System.Collections.Generic;
using System.Threading;

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
namespace org.gogpsproject.producer
{


	using EphGps = org.gogpsproject.ephemeris.EphGps;
	using EphemerisSystem = org.gogpsproject.ephemeris.EphemerisSystem;
	using Coordinates = org.gogpsproject.positioning.Coordinates;
	using SatellitePosition = org.gogpsproject.positioning.SatellitePosition;
	using Time = org.gogpsproject.positioning.Time;
	using IonoGps = org.gogpsproject.producer.parser.IonoGps;

	/// <summary>
	/// <para>
	/// This class receive data from streaming source and keep it buffered for navigation and observation consumer.
	/// It does not yet release consumed data.
	/// </para>
	/// 
	/// @author Lorenzo Patocchi cryms.com
	/// </summary>
	public class ObservationsBuffer : EphemerisSystem, StreamEventListener, ObservationsProducer, NavigationProducer
	{

		internal class EphSet
		{
			private readonly ObservationsBuffer outerInstance;

			public Time refTime;
			public Dictionary<int?, EphGps> ephs = new Dictionary<int?, EphGps>();
			public EphSet(ObservationsBuffer outerInstance, Time refTime)
			{
				this.outerInstance = outerInstance;
				this.refTime = refTime;
			}
		}

		private bool debug = false;

		private Coordinates definedPosition = null;

		private bool waitForData = true;

		private List<Observations> timeOrderedObs = new List<Observations>();
		private List<EphSet> timeOrderedEphs = new List<EphSet>();
		private List<IonoGps> timeOrderedIono = new List<IonoGps>();

		private int ionoCursor = 0;
		private int obsCursor = 0;
		private Dictionary<int?, int?> ephCursors = new Dictionary<int?, int?>();

		private StreamResource streamResource;

		private string fileNameOutLog = null;
		private FileOutputStream fosOutLog = null;
		private DataOutputStream outLog = null; //new XMLEncoder(os);

		private string id = null;

		private long timeoutNextObsWait = -1;

		private int bufferSizeLimit = 100;

		/// 
	   public ObservationsBuffer()
	   {
		   //this(null);
	   }
		/// <exception cref="FileNotFoundException">
		///  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ObservationsBuffer(StreamResource streamResource, String fileNameOutLog) throws java.io.FileNotFoundException
		public ObservationsBuffer(StreamResource streamResource, string fileNameOutLog)
		{
			// 1st define outlog
			this.FileNameOutLog = fileNameOutLog;
			// 2nd attach source, in case of Master source it will push Master Position into outstream
			this.StreamSource = streamResource;
		}

		public virtual StreamResource StreamSource
		{
			set
			{
				if (this.streamResource != null && this.streamResource is StreamEventProducer)
				{
					((StreamEventProducer)value).removeStreamEventListener(this);
				}
				this.streamResource = value;
				// if resource produces also events register for it
				if (value != null && value is StreamEventProducer)
				{
					((StreamEventProducer)value).addStreamEventListener(this);
				}
			}
		}

		private int getEphCursor(int satID)
		{
			int? ID = new int?(satID);
			if (!ephCursors.ContainsKey(ID))
			{
				return 0;
			}
			return (int)ephCursors[ID];
		}
		private void setEphCursor(int satID, int cur)
		{
			ephCursors[new int?(satID)] = new int?(cur);
		}
		/* (non-Javadoc)
		 * @see org.gogpsproject.parser.ublox.UBXEventListener#addEphemeris(org.gogpsproject.EphGps)
		 */
		public virtual void addEphemeris(EphGps eph)
		{
			int c = getEphCursor(eph.SatID);
			// trim to minutes
			while (c < timeOrderedEphs.Count && timeOrderedEphs[c].refTime.Msec / 60000 != eph.RefTime.Msec / 60000)
			{
				c++;
			}

			if (c < timeOrderedEphs.Count)
			{
				if (debug)
				{
					Console.WriteLine("found existing EphSet for " + eph.SatID + " @ " + (eph.RefTime.Msec / 60000));
				}
				// existing set
				timeOrderedEphs[c].ephs[new int?(eph.SatID)] = eph;
			}
			else
			{
				if (debug)
				{
					Console.WriteLine("new EphSet for " + eph.SatID + " @ " + (eph.RefTime.Msec / 60000));
				}
				// new set
				EphSet es = new EphSet(this, eph.RefTime);
				es.ephs[new int?(eph.SatID)] = eph;
				timeOrderedEphs.Add(es);
			}
			if (outLog != null)
			{

				try
				{
					eph.write(outLog);
					outLog.flush();
				}
				catch (IOException e)
				{
					Console.WriteLine(e.ToString());
					Console.Write(e.StackTrace);
				}

			}
		}

		/* (non-Javadoc)
		 * @see org.gogpsproject.parser.ublox.UBXEventListener#addIonospheric(org.gogpsproject.IonoGps)
		 */
		public virtual void addIonospheric(IonoGps iono)
		{
			int c = ionoCursor;
			// trim to minute
			while (c < timeOrderedIono.Count && timeOrderedIono[c].RefTime.Msec / 60000 != iono.RefTime.Msec / 60000)
			{
				c++;
			}

			if (c < timeOrderedIono.Count)
			{
				//System.out.println("found existing Iono @ "+(iono.getRefTime().getMsec()/60000));
				timeOrderedIono[c] = iono;
			}
			else
			{
				//System.out.println("new Iono @ "+(iono.getRefTime().getMsec()/60000));
				timeOrderedIono.Add(iono);
			}
			if (outLog != null)
			{

				try
				{
					iono.write(outLog);
					outLog.flush();
				}
				catch (IOException e)
				{
					Console.WriteLine(e.ToString());
					Console.Write(e.StackTrace);
				}

			}
		}

		/* (non-Javadoc)
		 * @see org.gogpsproject.parser.ublox.UBXEventListener#addObservations(org.gogpsproject.Observations)
		 */
		public virtual void addObservations(Observations o)
		{
			if (debug)
			{
				Console.WriteLine("obs " + o.NumSat + " time " + o.RefTime.Msec);
				Console.WriteLine(o);
			}
			// TODO test if ref time observations is not already present
			this.timeOrderedObs.Add(o);
			//System.out.println("---------------------------------------------");
			//System.out.println(o);

			//limit the observations buffer to bufferSizeLimit
			if (this.timeOrderedObs.Count > bufferSizeLimit)
			{
				int sizeToBeTrimmed = this.timeOrderedObs.Count - bufferSizeLimit;
				for (int i = 0; i < sizeToBeTrimmed; i++)
				{
					this.timeOrderedObs.RemoveAt(i);
				}
				obsCursor = obsCursor - sizeToBeTrimmed;
			}

			if (outLog != null)
			{

				try
				{
					o.write(outLog);
					outLog.flush();
				}
				catch (IOException e)
				{
					Console.WriteLine(e.ToString());
					Console.Write(e.StackTrace);
				}

			}

		}

		/* (non-Javadoc)
		 * @see org.gogpsproject.parser.ublox.UBXEventListener#streamClosed()
		 */
		public virtual void streamClosed()
		{
			// TODO implement reconnection policy, i.e. if(streamResource!=null && !waitForData) streamResource.reconnect();
			waitForData = false;
		}





		/* (non-Javadoc)
		 * @see org.gogpsproject.ObservationsProducer#getCurrentObservations()
		 */
		public virtual Observations CurrentObservations
		{
			get
			{
				long begin = DateTimeHelperClass.CurrentUnixTimeMillis();
    
				while (waitForData && (timeOrderedObs.Count == 0 || obsCursor >= timeOrderedObs.Count) && (timeoutNextObsWait == -1 || DateTimeHelperClass.CurrentUnixTimeMillis() - begin < timeoutNextObsWait))
				{
					//System.out.print("r");
					try
					{
						Thread.Sleep(1000);
					}
					catch (InterruptedException)
					{
					}
				}
    
				if (timeOrderedObs.Count > 0 && obsCursor < timeOrderedObs.Count)
				{
					return timeOrderedObs[obsCursor];
				}
				else
				{
					return null;
				}
			}
		}


		/* (non-Javadoc)
		 * @see org.gogpsproject.ObservationsProducer#getCurrentObservations()
		 */
		public virtual void pointToNextObservations()
		{

			obsCursor++;
		}

		/* (non-Javadoc)
		 * @see org.gogpsproject.ObservationsProducer#init()
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void init() throws Exception
		public virtual void init()
		{
			// Stream should have been already initialized.
			// if(streamResource!=null) streamResource.init();


		}

		/* (non-Javadoc)
		 * @see org.gogpsproject.ObservationsProducer#nextObservations()
		 */
		public virtual Observations NextObservations
		{
			get
			{
    
				long begin = DateTimeHelperClass.CurrentUnixTimeMillis();
    
				while (waitForData && (timeOrderedObs.Count == 0 || (obsCursor + 1) >= timeOrderedObs.Count) && (timeoutNextObsWait == -1 || DateTimeHelperClass.CurrentUnixTimeMillis() - begin < timeoutNextObsWait))
				{
					//System.out.println((id!=null?id:"")+"\tlook for :"+(obsCursor+1)+" pool size is:"+timeOrderedObs.size());
					try
					{
						Thread.Sleep(1000);
					}
					catch (InterruptedException)
					{
					}
				}
    
				if (timeOrderedObs.Count > 0 && (obsCursor + 1) < timeOrderedObs.Count)
				{
					Observations o = timeOrderedObs[++obsCursor];
    
					//System.out.println((id!=null?id:"")+"\tread obs "+o.getRefTime().getMsec());
					return o;
				}
    
				return null;
			}
		}

		/* (non-Javadoc)
		 * @see org.gogpsproject.ObservationsProducer#release()
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void release(boolean waitForThread, long timeoutMs) throws InterruptedException
		public virtual void release(bool waitForThread, long timeoutMs)
		{
			// make the request to nextObservations() return null as end of stream
			waitForData = false;

			if (outLog != null)
			{
				try
				{
					outLog.flush();
					outLog.close();
				}
				catch (Exception e)
				{
					Console.WriteLine(e.ToString());
					Console.Write(e.StackTrace);
				}
			}
			if (fosOutLog != null)
			{
				try
				{
					fosOutLog.close();
				}
				catch (Exception e)
				{
					Console.WriteLine(e.ToString());
					Console.Write(e.StackTrace);
				}
			}


			//if(streamResource!=null) streamResource.release(waitForThread, timeoutMs);
			if (streamResource != null && streamResource is StreamEventProducer)
			{
				((StreamEventProducer)streamResource).removeStreamEventListener(this);
			}

		}

		/* (non-Javadoc)
		 * @see org.gogpsproject.NavigationProducer#getGpsSatPosition(long, int, double)
		 */
		public virtual SatellitePosition getGpsSatPosition(Observations obs, int satID, char satType, double receiverClockError)
		{
			long unixTime = obs.RefTime.Msec;
			double range = obs.getSatByIDType(satID, satType).getPseudorange(0);
			if (timeOrderedEphs.Count == 0 || unixTime < timeOrderedEphs[0].refTime.Msec)
			{
				//System.out.println("\tR: sat pos not found for "+satID);
				return null;
			}
			EphSet closer = null; // timeOrderedEphs.elementAt(ephCursor);

			int? ID = new int?(satID);

			// temp cursor
			int c = getEphCursor(satID);
			while (c < timeOrderedEphs.Count)
			{
				EphSet tester = timeOrderedEphs[c];
				if ((closer == null || unixTime - closer.refTime.Msec > unixTime - tester.refTime.Msec) && unixTime - tester.refTime.Msec > 0)
				{
					//tester is closer and before unixTime, keep as new closer and update cursor
					if (tester.ephs.ContainsKey(ID))
					{
						closer = tester;
						setEphCursor(satID, c);
					}
					c++;
				}
				else
				{
					// tester is not closer or not before unixTime
					break;
				}
			}
			if (closer != null)
			{
				EphGps eph = closer.ephs[ID];

				SatellitePosition sp = computePositionGps(obs, satID, satType, eph, receiverClockError);
				//System.out.println("\tR: < sat pos "+ID);
				return sp;
			}
			//System.out.println("\tR: < sat pos not found for "+ID);
			return null;
		}

		/* (non-Javadoc)
		 * @see org.gogpsproject.NavigationProducer#getIono(long)
		 */
		public virtual IonoGps getIono(long unixTime)
		{
			if (timeOrderedIono.Count == 0 || unixTime < timeOrderedIono[0].RefTime.Msec)
			{
				return null;
			}
			IonoGps closer = timeOrderedIono[ionoCursor];

			// temp cursor
			int c = ionoCursor;
			while (c < timeOrderedIono.Count)
			{
				IonoGps tester = timeOrderedIono[c];
				if (unixTime - closer.RefTime.Msec > unixTime - tester.RefTime.Msec && unixTime - tester.RefTime.Msec > 0)
				{
					//tester is closer and before unixTime, keep as new closer and update cursor
					closer = tester;
					ionoCursor = c;
					c++;
				}
				else
				{
					// tester is not closer or not before unixTime
					//System.out.println("\t\tR: < Iono1");
					return closer;
				}
			}
			//System.out.println("\t\tR: < Iono2");
			return closer;
		}

		/* (non-Javadoc)
		 * @see org.gogpsproject.ObservationsProducer#getApproxPosition()
		 */
		public virtual Coordinates DefinedPosition
		{
			get
			{
				return definedPosition;
			}
			set
			{
				if (debug)
				{
					Console.WriteLine((id != null?id:"") + " got defined position: " + value);
				}
    
				this.definedPosition = value;
    
				if (outLog != null)
				{
    
					try
					{
						value.write(outLog);
						outLog.flush();
					}
					catch (IOException e)
					{
						Console.WriteLine(e.ToString());
						Console.Write(e.StackTrace);
					}
    
				}
			}
		}
		/// <param name="fileNameOutLog"> the fileNameOutLog to set </param>
		/// <exception cref="FileNotFoundException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void setFileNameOutLog(String fileNameOutLog) throws java.io.FileNotFoundException
		public virtual string FileNameOutLog
		{
			set
			{
				this.fileNameOutLog = value;
				if (value != null)
				{
					fosOutLog = new FileOutputStream(value,true);
					outLog = new DataOutputStream(fosOutLog);
				}
			}
			get
			{
				return fileNameOutLog;
			}
		}
		/// <param name="id"> the id to set </param>
		public virtual string Id
		{
			set
			{
				this.id = value;
			}
			get
			{
				return id;
			}
		}
		/// <param name="timeoutNextObsWait"> the timeoutNextObsWait to set </param>
		public virtual long TimeoutNextObsWait
		{
			set
			{
				this.timeoutNextObsWait = value;
			}
			get
			{
				return timeoutNextObsWait;
			}
		}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void readFromLog(String logFilename) throws java.io.IOException
		public virtual void readFromLog(string logFilename)
		{
			readFromLog(logFilename,false);
		}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void readFromLog(String logFilename,boolean oldversion) throws java.io.IOException
		public virtual void readFromLog(string logFilename, bool oldversion)
		{
			FileInputStream fis = new FileInputStream(logFilename);
			DataInputStream dis = new DataInputStream(fis);
			string msg = null;
			try
			{
				msg = dis.readUTF();
				while (msg != null)
				{
					if (debug)
					{
						Console.WriteLine("Msg:[" + msg + "]");
					}
					if (msg.Equals(Streamable_Fields.MESSAGE_OBSERVATIONS, StringComparison.CurrentCultureIgnoreCase))
					{
						Observations o = new Observations(dis,oldversion);
						addObservations(o);
					}
					else
					{
					if (msg.Equals(Streamable_Fields.MESSAGE_EPHEMERIS, StringComparison.CurrentCultureIgnoreCase))
					{
						EphGps eph = new EphGps(dis,oldversion);
						addEphemeris(eph);
					}
					else
					{
					if (msg.Equals(Streamable_Fields.MESSAGE_OBSERVATIONS_SET, StringComparison.CurrentCultureIgnoreCase))
					{
						ObservationSet eps = new ObservationSet(dis,oldversion);
						// nothing to do with ?
					}
					else
					{
					if (msg.Equals(Streamable_Fields.MESSAGE_IONO, StringComparison.CurrentCultureIgnoreCase))
					{
						IonoGps iono = new IonoGps(dis,oldversion);
						addIonospheric(iono);
					}
					else
					{
					if (msg.Equals(Streamable_Fields.MESSAGE_COORDINATES, StringComparison.CurrentCultureIgnoreCase))
					{
						Coordinates c = Coordinates.readFromStream(dis,oldversion);
						DefinedPosition = c;
					}
					else
					{
						Console.WriteLine("Unknow Msg:[" + msg + "]");
					}
					}
					}
					}
					}

					msg = dis.readUTF();
				}
			}
			catch (EOFException)
			{
				// ok
			}
			this.streamClosed();
			if (debug)
			{
				Console.WriteLine("End");
			}
		}
		/// <param name="debug"> the debug to set </param>
		public virtual bool Debug
		{
			set
			{
				this.debug = value;
			}
			get
			{
				return debug;
			}
		}
	}

}