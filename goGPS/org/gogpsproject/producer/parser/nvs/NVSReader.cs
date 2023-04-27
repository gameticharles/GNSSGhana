using System;
using System.Collections.Generic;
using System.Threading;

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
	/// <summary>
	/// <para>
	/// Read and parse NVS messages
	/// </para>
	/// 
	/// @author Daisuke Yoshida (Osaka City University), Lorenzo Patocchi (cryms.com)
	/// </summary>
	public class NVSReader : StreamEventProducer
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			multiConstellation = new Nullable[] {gpsEnable, qzsEnable, gloEnable, galEnable, bdsEnable};
		}

		private InputStream @is = null;
		private List<StreamEventListener> streamEventListeners = new List<StreamEventListener>();
		private bool? debugModeEnabled = false;

		internal bool gpsEnable = true; // enable GPS data reading
		internal bool qzsEnable = true; // enable QZSS data reading
		internal bool gloEnable = true; // enable GLONASS data reading
		internal bool galEnable = true; // enable Galileo data reading
		internal bool bdsEnable = false; // enable BeiDou data reading

		private bool?[] multiConstellation;

		//TODO
		public NVSReader(BufferedInputStream @is, bool?[] multiConstellation) : this(@is,null, null)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		public NVSReader(BufferedInputStream @is, StreamEventListener eventListener)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
			this.@is = @is;
			addStreamEventListener(eventListener);
		}

		public NVSReader(BufferedInputStream @is, bool?[] multiConstellation, StreamEventListener eventListener)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
			this.@is = @is;
			this.multiConstellation = multiConstellation;
			addStreamEventListener(eventListener);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object readMessage() throws java.io.IOException, NVSException
		public virtual object readMessage()
		{

				int data = @is.read();

				if (data == 0xf7) // F7
				{

					ByteArrayInputStream msg = new ByteArrayInputStream(removeDouble0x10(findMessageEnd()));

					DecodeF7 decodeF7 = new DecodeF7(msg, multiConstellation);

					EphGps eph = decodeF7.decode();
					//if(streamEventListeners!=null && eph!=null){
						//for(StreamEventListener sel:streamEventListeners){
							//sel.addEphemeris(eph);
						//}
					//}

					return eph;

				}
				else
				{
				if (data == 0xf5) // F5
				{

					sbyte[] byteArray = removeDouble0x10(findMessageEnd());
					ByteArrayInputStream msg = new ByteArrayInputStream(byteArray);

					DecodeF5 decodeF5 = new DecodeF5(msg, multiConstellation);

					Observations o = decodeF5.decode(null, byteArray.Length * 8);
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
				else
				{
				if (data == 0x4a) // 4A
				{

					ByteArrayInputStream msg = new ByteArrayInputStream(removeDouble0x10(findMessageEnd()));

					Decode4A decode4A = new Decode4A(msg);

					IonoGps iono = decode4A.decode();
					//if(streamEventListeners!=null && iono!=null){
						//for(StreamEventListener sel:streamEventListeners){
							//sel.addIonospheric(iono);
						//}
					//}

					return iono;

				}
				else
				{
				if (data == 0x62)
				{
					findMessageEnd();
					return -1;
				}
				else
				{
				if (data == 0x70)
				{
					findMessageEnd();
					return -1;
				}
				else
				{
				if (data == 0x4b)
				{
					findMessageEnd();
					return -1;
				}
				else
				{
				if (data == 0xF6)
				{
					findMessageEnd();
					return -1;
				}
				else
				{
				if (data == 0xe7)
				{
					findMessageEnd();
					return -1;
				}
				else
				{
				if (debugModeEnabled)
				{
					Console.WriteLine("Warning: wrong sync char 2 " + data + " " + data.ToString("x") + " [" + ((char)data) + "]");
				}
				}
				}
				}
				}
				}
				}
				}
				}

				return null;
		}

		private sbyte?[] findMessageEnd()
		{
			IList<sbyte?> data = new List<sbyte?>();
			bool stop = false;
			try
			{
				while (!stop)
				{
					if (@is.available() > 0)
					{
						sbyte value;
						value = (sbyte) @is.read();
						data.Add(value);
						if (value == 0x10) // <dle>
						{
							value = (sbyte) @is.read();
							data.Add(value);
							if (value == 0x03) // <ETX>
							{
								stop = true;
							}
						}
					}
					else
					{
						// no bytes to read, wait 1 msec
						try
						{
							Thread.Sleep(1);
						}
						catch (InterruptedException)
						{
						}
					}
				}
			}
			catch (IOException e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			return data.ToArray();
		}

		private sbyte[] removeDouble0x10(sbyte?[] @in)
		{
			sbyte[] @out = new sbyte[@in.Length];
			int i, k;
			for (i = 0, k = 0; i < @in.Length; i++, k++)
			{
				@out[k] = @in[i];
				if (@in[i] == 0x10 && @in[i + 1] == 0x10)
				{
					i++;
				}
			}
			return Arrays.copyOfRange(@out,0,@in.Length - (i - k));
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