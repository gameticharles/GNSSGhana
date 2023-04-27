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
namespace org.gogpsproject.producer.parser.skytraq
{


	/// <summary>
	/// <para>
	/// Read and parse STQ messages
	/// </para>
	/// 
	/// @author Lorenzo Patocchi cryms.com, Eugenio Realini
	/// </summary>
	public class STQReader : StreamEventProducer
	{
		private InputStream @in;
		private List<StreamEventListener> streamEventListeners = new List<StreamEventListener>();
		private bool? debugModeEnabled = false;
		//	private StreamEventListener streamEventListener;

		public STQReader(InputStream @is) : this(@is,null)
		{
		}
		public STQReader(InputStream @is, StreamEventListener eventListener)
		{
			this.@in = @is;
			addStreamEventListener(eventListener);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object readMessage(org.gogpsproject.producer.Observations o) throws java.io.IOException, STQException
		public virtual object readMessage(Observations o)
		{

		//	int data = in.read();
		//	if(data == 0xA0){
			int data = @in.read();
			if (data == 0xA1)
			{

				// parse big endian data
				int[] length = new int[2];

				length[0] = @in.read();
				length[1] = @in.read();

				int len = length[0] * 256 + length[1];

				if (len == 0)
				{
					throw new STQException("Zero-length SkyTraq message");
				}

				data = @in.read(); // message type
				bool parsed = false;
				if (data == 0xDD && o != null) //RAW-MEAS
				{
					// RAW-MEAS
					DecodeRAWMEAS decodegps = new DecodeRAWMEAS(@in, o);
					parsed = true;

					o = decodegps.decode(len);
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
				}
				else
				{
				if (data == 0xDC) //MEAS-TIME
				{
					// MEAS-TIME (measurement time)
					DecodeMEASTIME decodegps = new DecodeMEASTIME(@in);
					parsed = true;

					o = decodegps.decode(len);
					if (o != null && this.debugModeEnabled)
					{
						Console.WriteLine("Decoded time message");
					}
					return o;
	//			}else
	//			if (data == 0xB1) {
	//				// GPS-EPH (ephemerides)
	//				DecodeGPSEPH decodegps = new DecodeGPSEPH(in);
	//				parsed = true;
	//
	//				EphGps eph = decodegps.decode();
	//				if (eph!=null && this.debugModeEnabled) {
	//					System.out.println("Decoded ephemeris for satellite " + eph.getSatID());
	//				}
	//				if(streamEventListeners!=null && eph!=null){
	//					for(StreamEventListener sel:streamEventListeners){
	//						sel.addEphemeris(eph);
	//					}
	//				}
	//				return eph;
				}
				}
				if (!parsed)
				{

					if (this.debugModeEnabled)
					{
						Console.WriteLine("Warning: STQ message not decoded; skipping " + len + " bytes");
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
				return o;
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