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
namespace org.gogpsproject.producer.parser.ublox
{


	using EphGps = org.gogpsproject.ephemeris.EphGps;
	/// <summary>
	/// <para>
	/// Read and parse UBX messages
	/// </para>
	/// 
	/// @author Lorenzo Patocchi cryms.com, Eugenio Realini
	/// </summary>
	public class UBXReader : StreamEventProducer
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			multiConstellation = new Nullable[] {gpsEnable, qzsEnable, gloEnable, galEnable, bdsEnable};
		}

		private InputStream @in;
		private List<StreamEventListener> streamEventListeners = new List<StreamEventListener>();
		private bool? debugModeEnabled = false;
		//	private StreamEventListener streamEventListener;

		internal bool gpsEnable = true; // enable GPS data reading
		internal bool qzsEnable = true; // enable QZSS data reading
		internal bool gloEnable = true; // enable GLONASS data reading
		internal bool galEnable = true; // enable Galileo data reading
		internal bool bdsEnable = false; // enable BeiDou data reading

		private bool?[] multiConstellation;

		public UBXReader(InputStream @is) : this(@is,null)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}
		public UBXReader(InputStream @is, StreamEventListener eventListener)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
			this.@in = @is;
			addStreamEventListener(eventListener);
		}

		public UBXReader(InputStream @is, bool?[] multiConstellation, StreamEventListener eventListener)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
			this.@in = @is;
			this.multiConstellation = multiConstellation;
			addStreamEventListener(eventListener);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object readMessage() throws java.io.IOException, UBXException
		public virtual object readMessage()
		{

		//	int data = in.read();
		//	if(data == 0xB5){
			int data = @in.read();
			if (data == 0x62)
			{

				data = @in.read(); // Class
				bool parsed = false;
				if (data == 0x02) // RXM
				{
					data = @in.read(); // ID
					if (data == 0x10) // RAW
					{
						// RMX-RAW
						DecodeRXMRAW decodegps = new DecodeRXMRAW(@in);
						parsed = true;

						Observations o = decodegps.decode(null);
						if (o != null && this.debugModeEnabled)
						{
							Console.WriteLine("Decoded observations");
						}
						if (streamEventListeners != null && o != null)
						{
							foreach (StreamEventListener sel in streamEventListeners)
							{
								Observations oc = (Observations)o.clone();
								sel.addObservations(oc);
							}
						}
						return o;

					} //RAWX
					else if (data == 0x15)
					{
						// RMX-RAWX
						DecodeRXMRAWX decodegnss = new DecodeRXMRAWX(@in, multiConstellation);
						parsed = true;

						Observations o = decodegnss.decode(null);
						if (o != null && this.debugModeEnabled)
						{
							Console.WriteLine("Decoded observations");
						}
						if (streamEventListeners != null && o != null)
						{
							foreach (StreamEventListener sel in streamEventListeners)
							{
								Observations oc = (Observations)o.clone();
								sel.addObservations(oc);
							}
						}
						return o;


					}

				}
				else
				{
					if (data == 0x0B) // AID
					{
						data = @in.read(); // ID
						try
						{
							if (data == 0x02) // HUI
							{
								// AID-HUI (sat. Health / UTC / Ionosphere)
								DecodeAIDHUI decodegps = new DecodeAIDHUI(@in);
								parsed = true;

								IonoGps iono = decodegps.decode();
								if (iono != null && this.debugModeEnabled)
								{
									Console.WriteLine("Decoded iono parameters");
								}
								if (streamEventListeners != null && iono != null)
								{
									foreach (StreamEventListener sel in streamEventListeners)
									{
										sel.addIonospheric(iono);
									}
								}
								return iono;
							}
							else
							{
								if (data == 0x31) // EPH
								{
									// AID-EPH (ephemerides)
									DecodeAIDEPH decodegps = new DecodeAIDEPH(@in);
									parsed = true;

									EphGps eph = decodegps.decode();
									if (eph != null && this.debugModeEnabled)
									{
										Console.WriteLine("Decoded ephemeris for satellite " + eph.SatID);
									}
									if (streamEventListeners != null && eph != null)
									{
										foreach (StreamEventListener sel in streamEventListeners)
										{
											sel.addEphemeris(eph);
										}
									}
									return eph;
								}
							}
						}
						catch (UBXException ubxe)
						{
							if (this.debugModeEnabled)
							{
								Console.WriteLine(ubxe);
							}
						}
					}
					else
					{
						@in.read(); // ID
					}
				}
				if (!parsed)
				{

					// read non parsed message length
					int[] length = new int[2];
					length[1] = @in.read();
					length[0] = @in.read();

					int len = length[0] * 256 + length[1];
					if (this.debugModeEnabled)
					{
						Console.WriteLine("Warning: UBX message not decoded; skipping " + len + " bytes");
					}
					for (int b = 0; b < len + 2; b++)
					{
						@in.read();
					}
				}
			}
			else
			{
				if (this.debugModeEnabled)
				{
					Console.WriteLine("Warning: wrong sync char 2 " + data + " " + data.ToString("x") + " [" + ((char)data) + "]");
				}
			}
		//	}else{
		//		//no warning, may be NMEA
		//		//System.out.println("Warning: wrong sync char 1 "+data+" "+Integer.toHexString(data)+" ["+((char)data)+"]");
		//	}
			return null;
		}
		/// <returns> the streamEventListener </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") @Override public java.util.Vector<org.gogpsproject.producer.StreamEventListener> getStreamEventListeners()
		public virtual List<StreamEventListener> StreamEventListeners
		{
			get
			{
				return (List<StreamEventListener>)streamEventListeners.clone();
			}
		}
		/// <param name="streamEventListener"> the streamEventListener to set </param>
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
		}
		public virtual void enableDebugMode(bool? enableDebug)
		{
			this.debugModeEnabled = enableDebug;
		}

	}

}