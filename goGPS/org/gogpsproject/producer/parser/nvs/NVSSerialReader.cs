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
	using BufferedInputStreamCounter = org.gogpsproject.util.BufferedInputStreamCounter;

	/// <summary>
	/// <para>
	/// 
	/// </para>
	/// 
	/// @author Lorenzo Patocchi cryms.com, Eugenio Realini
	/// </summary>

	public class NVSSerialReader : Runnable, StreamEventProducer
	{

		private BufferedInputStreamCounter @in;
		private OutputStream @out;
		//private boolean end = false;
		private Thread t = null;
		private bool stop_Renamed = false;
		private List<StreamEventListener> streamEventListeners = new List<StreamEventListener>();
		//private StreamEventListener streamEventListener;
		private NVSReader reader;
		private string COMPort;
		private int measRate = 1;
		private bool sysTimeLogEnabled = false;
		private string dateFile;
		private string outputDir = "./test";
		private bool debugModeEnabled = false;

		public NVSSerialReader(InputStream @in, OutputStream @out, string COMPort, string outputDir) : this(@in,@out,COMPort,outputDir,null)
		{
			this.COMPort = padCOMSpaces(COMPort);
		}

		public NVSSerialReader(InputStream @in, OutputStream @out, string COMPort, string outputDir, StreamEventListener streamEventListener)
		{
			FileOutputStream fos_nvs = null;
			COMPort = padCOMSpaces(COMPort);
			string COMPortStr = prepareCOMStringForFilename(COMPort);

			File file = new File(outputDir);
			if (!file.exists() || !file.Directory)
			{
				bool wasDirectoryMade = file.mkdirs();
				if (wasDirectoryMade)
				{
					Console.WriteLine("Directory " + outputDir + " created");
				}
				else
				{
					Console.WriteLine("Could not create directory " + outputDir);
				}
			}

			DateTime date = DateTime.Now;
			SimpleDateFormat sdf1 = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss.SSS");
			string date1 = sdf1.format(date);
			SimpleDateFormat sdfFile = new SimpleDateFormat("yyyy-MM-dd_HHmmss");
			dateFile = sdfFile.format(date);

			try
			{
				Console.WriteLine(date1 + " - " + COMPort + " - Logging NVS stream in " + outputDir + "/" + COMPortStr + "_" + dateFile + ".bin");
				fos_nvs = new FileOutputStream(outputDir + "/" + COMPortStr + "_" + dateFile + ".nvs");
			}
			catch (FileNotFoundException e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}

			this.@in = new BufferedInputStreamCounter(@in, fos_nvs);
			this.@out = @out;
			this.reader = new NVSReader(this.@in,streamEventListener);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean setBinrProtocol() throws java.io.IOException
		public virtual bool setBinrProtocol()
		{
			DateTime date = DateTime.Now;
			SimpleDateFormat sdf1 = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss.SSS");
			string date1 = sdf1.format(date);

			NVSProtocolConfiguration msgcfg = new NVSProtocolConfiguration();
			@out.write(msgcfg.Byte);
			@in.skip(@in.available());
			@out.flush();

			int data = 0;
			try
			{
				Thread.Sleep(100);
			}
			catch (InterruptedException)
			{
			}
			if (@in.available() > 0)
			{
				data = @in.read();
				if (data == 0x10)
				{
					data = @in.read();
					if (data == 0x50 || data == 0xf5 || data == 0x0B)
					{
						@in.skip(@in.available());
						Console.WriteLine(date1 + " - " + COMPort + " - raw data messages (F5h) enabled");
						return true;
					}
				}
				else
				{
					if (this.debugModeEnabled)
					{
						Console.WriteLine("Warning: wrong sync char 1 " + data + " " + data.ToString("x") + " [" + ((char)data) + "]");
					}
				}
			}

			string nmeacfg = "$PORZA,0,115200,3";
			nmeacfg = nmeacfg + "*" + computeNMEACheckSum(nmeacfg) + "\r\n";
			@out.write(nmeacfg.GetBytes());
			@in.skip(@in.available());
			@out.flush();

			try
			{
				Thread.Sleep(100);
			}
			catch (InterruptedException)
			{
			}
			if (@in.available() > 0)
			{
				data = @in.read();
				if (data == 0x24) // "$"
				{
					data = @in.read();
					if (data == 0x50) // "P"
					{
						@in.skip(@in.available());
						Console.WriteLine(date1 + " - " + COMPort + " - raw data messages (F5h) enabled");
						return true;
					}
				}
				else
				{
					if (this.debugModeEnabled)
					{
						Console.WriteLine("Warning: wrong sync char 1 " + data + " " + data.ToString("x") + " [" + ((char)data) + "]");
					}
				}
			}

			return false;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void start() throws java.io.IOException
		public virtual void start()
		{
			t = new Thread(this);
			t.Name = "NVSSerialReader";
			t.Start();

			DateTime date = DateTime.Now;
			SimpleDateFormat sdf1 = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss.SSS");
			string date1 = sdf1.format(date);

			Console.WriteLine(date1 + " - " + COMPort + " - Measurement rate set at " + measRate + " Hz");
			NVSRateConfiguration ratecfg = new NVSRateConfiguration(measRate);
			@out.write(ratecfg.Byte);
			@out.flush();

			if (this.debugModeEnabled)
			{
				Console.WriteLine(date1 + " - " + COMPort + " - !!! DEBUG MODE !!!");
			}
		}
		public virtual void stop(bool waitForThread, long timeoutMs)
		{
			stop_Renamed = true;
			if (waitForThread && t != null && t.IsAlive)
			{
				try
				{
					t.Join(timeoutMs);
				}
				catch (InterruptedException e)
				{
					Console.WriteLine(e.ToString());
					Console.Write(e.StackTrace);
				}
			}
		}
		public virtual void run()
		{

			int data = 0;
			FileOutputStream fos_tim = null;
			PrintStream psSystime = null;
			COMPort = padCOMSpaces(COMPort);
			string COMPortStr = prepareCOMStringForFilename(COMPort);

			DateTime date = DateTime.Now;
			SimpleDateFormat sdf1 = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss.SSS");
			string date1 = sdf1.format(date);
			SimpleDateFormat sdfFile = new SimpleDateFormat("yyyy-MM-dd_HHmmss");
			dateFile = sdfFile.format(date);

			if (sysTimeLogEnabled)
			{
				Console.WriteLine(date1 + " - " + COMPort + " - System time logging enabled");
				try
				{
					Console.WriteLine(date1 + " - " + COMPort + " - Logging system time in " + outputDir + "/" + COMPortStr + "_" + dateFile + "_systime.txt");
					fos_tim = new FileOutputStream(outputDir + "/" + COMPortStr + "_" + dateFile + "_systime.txt");
					psSystime = new PrintStream(fos_tim);
					psSystime.println("GPS time                      System time");
				}
				catch (FileNotFoundException e)
				{
					Console.WriteLine(e.ToString());
					Console.Write(e.StackTrace);
				}
			}
			else
			{
				Console.WriteLine(date1 + " - " + COMPort + " - System time logging disabled");
			}

			try
			{
				@in.start();
				sdf1 = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss.SSS");
				string dateSys = null;
				string dateGps = null;
				bool f5hMsgReceived = false;
				bool f7hMsgReceived = false;
				reader.enableDebugMode(this.debugModeEnabled);
				while (!stop_Renamed)
				{
					if (@in.available() > 0)
					{
						dateSys = sdf1.format(DateTime.Now);
						data = @in.read();
						try
						{
							if (data == 0x10)
							{
								object o = reader.readMessage();
								try
								{
									if (o is Observations)
									{

										f5hMsgReceived = true;

										if (streamEventListeners != null && o != null)
										{
											foreach (StreamEventListener sel in streamEventListeners)
											{
												Observations co = sel.CurrentObservations;
												sel.pointToNextObservations();

												if (this.sysTimeLogEnabled)
												{
													dateGps = sdf1.format(new DateTime(co.RefTime.Msec));
													psSystime.println(dateGps + "       " + dateSys);
												}
											}
										}
									}
									else if (o is EphGps)
									{
										f7hMsgReceived = true;
									}
									else if (o is int?)
									{
										if (this.debugModeEnabled)
										{
											Console.WriteLine("Unsupported message");
										}
									}
									else if (o == null)
									{
										if (this.debugModeEnabled)
										{
											Console.WriteLine("Decoding error");
										}
									}
								}
								catch (System.NullReferenceException)
								{
								}
							}
							else
							{
								if (this.debugModeEnabled)
								{
									//System.out.println("Warning: wrong sync char 1 "+data+" "+Integer.toHexString(data)+" ["+((char)data)+"]");
								}
							}
						}
						catch (NVSException nvse)
						{
							Console.WriteLine(nvse.ToString());
							Console.Write(nvse.StackTrace);
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

					if (f5hMsgReceived)
					{
						int bps = @in.CurrentBps;
						if (bps != 0)
						{
							Console.WriteLine(dateSys + " - " + COMPort + " - logging at " + string.Format("{0,4:D}", bps) + " Bps -- total: " + @in.Counter + " bytes");
						}
						else
						{
							Console.WriteLine(dateSys + " - " + COMPort + " - log starting...     -- total: " + @in.Counter + " bytes");
						}
						f5hMsgReceived = false;
					}
					if (f7hMsgReceived)
					{
						int bps = @in.CurrentBps;
						if (bps != 0)
						{
							Console.WriteLine(dateSys + " - " + COMPort + " - logging at " + string.Format("{0,4:D}", bps) + " Bps -- total: " + @in.Counter + " bytes");
						}
						else
						{
							Console.WriteLine(dateSys + " - " + COMPort + " - log starting...     -- total: " + @in.Counter + " bytes");
						}
						f7hMsgReceived = false;
					}
				}
			}
			catch (IOException e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			foreach (StreamEventListener sel in streamEventListeners)
			{
				sel.streamClosed();
			}
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
			if (this.reader != null)
			{
				this.reader.addStreamEventListener(streamEventListener);
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

		public virtual int Rate
		{
			set
			{
				this.measRate = value;
			}
		}

		public virtual void enableSysTimeLog(bool? enableTim)
		{
			this.sysTimeLogEnabled = enableTim;
		}

		public virtual void enableDebugMode(bool? enableDebug)
		{
			this.debugModeEnabled = enableDebug;
		}

		private string padCOMSpaces(string COMPortIn)
		{
			if (COMPortIn.Substring(0, 3).Equals("COM") && COMPortIn.Length == 4)
			{
				COMPortIn = COMPortIn + " ";
			}
			return COMPortIn;
		}

		private string prepareCOMStringForFilename(string COMPort)
		{
			string[] tokens = COMPort.Split("/", true);
			if (tokens.Length > 0)
			{
				COMPort = tokens[tokens.Length - 1].Trim(); //for UNIX /dev/tty* ports
			}
			return COMPort;
		}

		private static string computeNMEACheckSum(string msg)
		{
			// perform NMEA checksum calculation
			int chk = 0;

			for (int i = 1; i < msg.Length; i++)
			{
				chk ^= msg[i];
			}
			string chk_s = chk.ToString("x").ToUpper();
			// checksum must be 2 characters!
			while (chk_s.Length < 2)
			{
				chk_s = "0" + chk_s;
			}
			return chk_s;

		}

		public virtual string OutputDir
		{
			set
			{
				this.outputDir = value;
			}
		}
	}

}