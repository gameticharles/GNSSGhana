using System;
using System.Collections.Generic;

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
namespace org.gogpsproject.producer.parser.nvs
{



	using EphGps = org.gogpsproject.ephemeris.EphGps;
	using EphemerisSystem = org.gogpsproject.ephemeris.EphemerisSystem;
	using Coordinates = org.gogpsproject.positioning.Coordinates;
	using SatellitePosition = org.gogpsproject.positioning.SatellitePosition;

	/// <summary>
	/// <para>
	/// Read an NVS File and implement Observation and Navigation producer 
	/// </para>
	/// 
	/// @author Daisuke Yoshida OCU
	/// </summary>

	public class NVSFileReader : EphemerisSystem, ObservationsProducer, NavigationProducer, StreamResource, StreamEventProducer
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			multiConstellation = new Nullable[] {gpsEnable, qzsEnable, gloEnable, galEnable, bdsEnable};
		}


		private BufferedInputStream @in;
		private File file;
		private Observations obs = null;
		private NVSReader reader;
		private IonoGps iono = null;
		// TODO support past times, now keep only last broadcast data
		private Dictionary<int?, EphGps> ephs = new Dictionary<int?, EphGps>();

		private List<StreamEventListener> streamEventListeners = new List<StreamEventListener>();

		internal bool gpsEnable = true; // enable GPS data reading
		internal bool qzsEnable = true; // enable QZSS data reading
		internal bool gloEnable = true; // enable GLONASS data reading
		internal bool galEnable = true; // enable Galileo data reading
		internal bool bdsEnable = true; // enable BeiDou data reading

		internal bool?[] multiConstellation;

		public NVSFileReader(File file)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
			this.file = file;
			//this.multiConstellation[0] = true;  // enable QZSS data reading
			//this.multiConstellation[1] = true;  // enable GLONASS data reading
		}

		public NVSFileReader(File file, bool?[] multiConstellation)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
			this.file = file;
			this.multiConstellation = multiConstellation;
		}

		/* (non-Javadoc)
		 * @see org.gogpsproject.ObservationsProducer#getApproxPosition()
		 */
		public virtual Coordinates DefinedPosition
		{
			get
			{
				Coordinates coord = Coordinates.globalXYZInstance(0.0, 0.0, 0.0); //new Coordinates(new SimpleMatrix(3, 1));
				//coord.setXYZ(0.0, 0.0, 0.0 );
				coord.computeGeodetic();
				// TODO should return null?
				return coord;
			}
		}

		/* (non-Javadoc)
		 * @see org.gogpsproject.ObservationsProducer#getCurrentObservations()
		 */
		public virtual Observations CurrentObservations
		{
			get
			{
				return obs;
			}
		}

		/* (non-Javadoc)
		 * @see org.gogpsproject.ObservationsProducer#hasMoreObservations()
		 */
		public virtual bool hasMoreObservations()
		{
			bool moreObs = false;
			try
			{
				moreObs = @in.available() > 0;
			}
			catch (IOException)
			{
			}
			return moreObs;
		}

		/* (non-Javadoc)
		 * @see org.gogpsproject.ObservationsProducer#init()
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void init() throws Exception
		public virtual void init()
		{
			FileInputStream ins = new FileInputStream(file);
			this.@in = new BufferedInputStream(ins);
			this.reader = new NVSReader(@in, multiConstellation, null);
		}

		/* (non-Javadoc)
		 * @see org.gogpsproject.ObservationsProducer#init()
		 */
		public virtual Observations NextObservations
		{
			get
			{
    
				try
				{
					while (@in.available() > 1000) // Need to adjust this value
					{
    
						try
						{
							int data = @in.read();
    
							if (data == 0x10)
							{
								object o = reader.readMessage();
    
								if (o is Observations)
								{
									return (Observations)o;
								}
								else
								{
								if (o is IonoGps)
								{
									iono = (IonoGps)o;
								}
								else
								{
								if (o is EphGps)
								{
    
									EphGps e = (EphGps)o;
									ephs[new int?(e.SatID)] = e;
								}
								}
								}
							}
						}
						catch (NVSException nvse)
						{
							Console.Error.WriteLine(nvse);
						}
					}
    
					@in.close();
    
				}
				catch (IOException e)
				{
					Console.WriteLine(e.ToString());
					Console.Write(e.StackTrace);
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
			try
			{
				@in.close();

			}
			catch (IOException e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
		}

		/* (non-Javadoc)
		 * @see org.gogpsproject.NavigationProducer#getGpsSatPosition(long, int, double)
		 */
		public virtual SatellitePosition getGpsSatPosition(Observations obs, int satID, char satType, double receiverClockError)
		{
			EphGps eph = ephs[new int?(satID)];

	//		char satType = eph.getSatType();

			if (eph != null)
			{
				SatellitePosition sp = computePositionGps(obs, satID, satType, eph, receiverClockError);
				return sp;
			}
			return null;
		}

		/* (non-Javadoc)
		 * @see org.gogpsproject.NavigationProducer#getIono(long)
		 */
		public virtual IonoGps getIono(long unixTime)
		{
			return iono;
		}

		/* (non-Javadoc)
		 * @see org.gogpsproject.StreamEventProducer#addStreamEventListener(org.gogpsproject.StreamEventListener)
		 */
		public virtual void addStreamEventListener(StreamEventListener streamEventListener)
		{
			if (streamEventListener == null)
			{
				return;
			}
			if (!streamEventListeners.Contains(streamEventListener))
			{
				this.streamEventListeners.Add(streamEventListener);
			}
			if (this.reader != null)
			{
				this.reader.addStreamEventListener(streamEventListener);
			}
		}

		/* (non-Javadoc)
		 * @see org.gogpsproject.StreamEventProducer#getStreamEventListeners()
		 */
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") @Override public java.util.Vector<org.gogpsproject.producer.StreamEventListener> getStreamEventListeners()
		public virtual List<StreamEventListener> StreamEventListeners
		{
			get
			{
				return (List<StreamEventListener>) streamEventListeners.clone();
			}
		}

		/* (non-Javadoc)
		 * @see org.gogpsproject.StreamEventProducer#removeStreamEventListener(org.gogpsproject.StreamEventListener)
		 */
		public virtual void removeStreamEventListener(StreamEventListener streamEventListener)
		{
			if (streamEventListener == null)
			{
				return;
			}
			if (streamEventListeners.Contains(streamEventListener))
			{
				this.streamEventListeners.Remove(streamEventListener);
			}
			this.reader.removeStreamEventListener(streamEventListener);
		}
	}






}