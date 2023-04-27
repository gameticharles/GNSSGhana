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
namespace org.gogpsproject.producer.parser.ublox
{


	using InputStreamCounter = org.gogpsproject.util.InputStreamCounter;

	/// <summary>
	/// <para>
	/// 
	/// </para>
	/// 
	/// @author Lorenzo Patocchi cryms.com, Eugenio Realini
	/// </summary>

	public class UBXSerialReader : Runnable, StreamEventProducer
	{

	  protected internal InputStreamCounter @in;
	  protected internal OutputStream @out;
	  private Thread t = null;
	  protected internal bool stop_Renamed = false;
	  protected internal List<StreamEventListener> streamEventListeners = new List<StreamEventListener>();
	  protected internal UBXReader reader;
	  protected internal string COMPort;
	  private int measRate = 1;
	  protected internal bool sysTimeLogEnabled = false;
	  protected internal IList<string> requestedNmeaMsgs = null;
	  protected internal string dateFile;
	  protected internal string outputDir = "./test";
	  protected internal int msgAidEphRate = 0; //seconds
	  protected internal int msgAidHuiRate = 0; //seconds
	  protected internal bool debugModeEnabled = false;

		public UBXSerialReader(InputStream @in, OutputStream @out, string COMPort, string outputDir) : this(@in,@out,COMPort,outputDir,null)
		{
			this.COMPort = padCOMSpaces(COMPort);
		}

		public UBXSerialReader(InputStream @in, OutputStream @out, string COMPort, string outputDir, StreamEventListener streamEventListener)
		{

			FileOutputStream fos_ubx = null;
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
				Console.WriteLine(date1 + " - " + COMPort + " - Logging UBX stream in " + outputDir + "/" + COMPortStr + "_" + dateFile + ".ubx");
				fos_ubx = new FileOutputStream(outputDir + "/" + COMPortStr + "_" + dateFile + ".ubx");
			}
			catch (FileNotFoundException e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			this.@in = new InputStreamCounter(@in,fos_ubx);
			this.@out = @out;
			this.reader = new UBXReader(this.@in,streamEventListener);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void start() throws java.io.IOException
		public virtual void start()
		{
			t = new Thread(this);
			t.Name = "UBXSerialReader - " + COMPort;
			t.Start();

			DateTime date = DateTime.Now;
			SimpleDateFormat sdf1 = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss.SSS");
			string date1 = sdf1.format(date);

			//some u-blox receivers need their UART baudrate to be increased in order to manage measurement rates > 1 Hz
			if (measRate > 1)
			{
				Console.WriteLine(date1 + " - " + COMPort + " - Baud rate set at 115200 bits/sec");
				UBXPortConfiguration portcfg = new UBXPortConfiguration();
				@out.write(portcfg.Byte);
				@out.flush();
			}

			Console.WriteLine(date1 + " - " + COMPort + " - Measurement rate set at " + measRate + " Hz");
			UBXRateConfiguration ratecfg = new UBXRateConfiguration(1000 / measRate, 1, 1);
			@out.write(ratecfg.Byte);
			@out.flush();

			int[] nmeaAll = new int[] {UBXMessageType.NMEA_GGA, UBXMessageType.NMEA_GLL, UBXMessageType.NMEA_GSA, UBXMessageType.NMEA_GSV, UBXMessageType.NMEA_RMC, UBXMessageType.NMEA_VTG, UBXMessageType.NMEA_GRS, UBXMessageType.NMEA_GST, UBXMessageType.NMEA_ZDA, UBXMessageType.NMEA_GBS, UBXMessageType.NMEA_DTM};
			for (int i = 0; i < nmeaAll.Length; i++)
			{
				UBXMsgConfiguration msgcfg = new UBXMsgConfiguration(UBXMessageType.CLASS_NMEA, nmeaAll[i], false);
				@out.write(msgcfg.Byte);
				@out.flush();
			}

			int[] nmeaRequested;
			try
			{
				if (requestedNmeaMsgs.Count == 0)
				{
					Console.WriteLine(date1 + " - " + COMPort + " - NMEA messages disabled");
				}
				else
				{
					nmeaRequested = new int[requestedNmeaMsgs.Count];
					for (int n = 0; n < requestedNmeaMsgs.Count; n++)
					{
						UBXMessageType msgtyp = new UBXMessageType("NMEA", requestedNmeaMsgs[n]);
						nmeaRequested[n] = msgtyp.IdOut;
					}
					for (int i = 0; i < nmeaRequested.Length; i++)
					{
						Console.WriteLine(date1 + " - " + COMPort + " - NMEA " + requestedNmeaMsgs[i] + " messages enabled");
						UBXMsgConfiguration msgcfg = new UBXMsgConfiguration(UBXMessageType.CLASS_NMEA, nmeaRequested[i], true);
						@out.write(msgcfg.Byte);
						@out.flush();
					}
				}
			}
			catch (System.NullReferenceException)
			{
			}

			int[] pubx = new int[] {UBXMessageType.PUBX_A, UBXMessageType.PUBX_B, UBXMessageType.PUBX_C, UBXMessageType.PUBX_D};
			for (int i = 0; i < pubx.Length; i++)
			{
				UBXMsgConfiguration msgcfg = new UBXMsgConfiguration(UBXMessageType.CLASS_PUBX, pubx[i], false);
				@out.write(msgcfg.Byte);
				@out.flush();
			}

			Console.WriteLine(date1 + " - " + COMPort + " - RXM-RAW messages enabled");
			UBXMsgConfiguration msgcfg = new UBXMsgConfiguration(UBXMessageType.CLASS_RXM, UBXMessageType.RXM_RAW, true);
			@out.write(msgcfg.Byte);
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
			long aidEphTS = DateTimeHelperClass.CurrentUnixTimeMillis();
			long aidHuiTS = DateTimeHelperClass.CurrentUnixTimeMillis();
			//long sysOutTS = System.currentTimeMillis();
			UBXMsgConfiguration msgcfg = null;
			FileOutputStream fos_tim = null;
			FileOutputStream fos_nmea = null;
			PrintStream psSystime = null;
			PrintStream psNmea = null;

			DateTime date = DateTime.Now;
			SimpleDateFormat sdf1 = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss.SSS");
			string date1 = sdf1.format(date);
			string COMPortStr = prepareCOMStringForFilename(COMPort);

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

			if (requestedNmeaMsgs.Count > 0)
			{
				try
				{
					Console.WriteLine(date1 + " - " + COMPort + " - Logging NMEA sentences in " + outputDir + "/" + COMPortStr + "_" + dateFile + "_NMEA.txt");
					fos_nmea = new FileOutputStream(outputDir + "/" + COMPortStr + "_" + dateFile + "_NMEA.txt");
					psNmea = new PrintStream(fos_nmea);
				}
				catch (FileNotFoundException e)
				{
					Console.WriteLine(e.ToString());
					Console.Write(e.StackTrace);
				}
			}

			try
			{
				int[] msg = new int[] {};
				if (msgAidHuiRate > 0)
				{
					Console.WriteLine(date1 + " - " + COMPort + " - AID-HUI message polling enabled (rate: " + msgAidHuiRate + "s)");
					msgcfg = new UBXMsgConfiguration(UBXMessageType.CLASS_AID, UBXMessageType.AID_HUI, msg);
					@out.write(msgcfg.Byte);
					@out.flush();
				}
				if (msgAidEphRate > 0)
				{
					Console.WriteLine(date1 + " - " + COMPort + " - AID-EPH message polling enabled (rate: " + msgAidEphRate + "s)");
					msgcfg = new UBXMsgConfiguration(UBXMessageType.CLASS_AID, UBXMessageType.AID_EPH, msg);
					@out.write(msgcfg.Byte);
					@out.flush();
				}

				@in.start();
				sdf1 = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss.SSS");
				string dateSys = null;
				string dateGps = null;
				bool rxmRawMsgReceived = false;
				bool truncatedNmea = false;
				reader.enableDebugMode(this.debugModeEnabled);
				while (!stop_Renamed)
				{
					if (@in.available() > 0)
					{
						dateSys = sdf1.format(DateTime.Now);
						if (!truncatedNmea)
						{
							data = @in.read();
						}
						else
						{
							truncatedNmea = false;
						}
						try
						{
							if (data == 0xB5)
							{
								object o = reader.readMessage();
								try
								{
									if (o is Observations)
									{

										rxmRawMsgReceived = true;

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
								}
								catch (System.NullReferenceException)
								{
								}
							}
							else if (data == 0x24)
							{
								if (requestedNmeaMsgs.Count > 0)
								{
									string sentence = "" + (char) data;
									data = @in.read();
									if (data == 0x47)
									{
										sentence = sentence + (char) data;
										data = @in.read();
										if (data == 0x50)
										{
											sentence = sentence + (char) data;
											data = @in.read();
											while (data != 0x0A && data != 0xB5)
											{
												sentence = sentence + (char) data;
												data = @in.read();
											}
											sentence = sentence + (char) data;
											psNmea.print(sentence);
	//										if (this.debugModeEnabled) {
	//											System.out.print(sentence);
	//										}
											if (data == 0xB5)
											{
												truncatedNmea = true;
												if (this.debugModeEnabled)
												{
													Console.WriteLine("Warning: truncated NMEA message");
												}
											}
										}
									}
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
						catch (UBXException ubxe)
						{
							Console.WriteLine(ubxe.ToString());
							Console.Write(ubxe.StackTrace);
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
					long curTS = DateTimeHelperClass.CurrentUnixTimeMillis();

					if (msgAidEphRate > 0 && curTS - aidEphTS >= msgAidEphRate * 1000)
					{
						Console.WriteLine(dateSys + " - " + COMPort + " - Polling AID-EPH message");
						msgcfg = new UBXMsgConfiguration(UBXMessageType.CLASS_AID, UBXMessageType.AID_EPH, msg);
						@out.write(msgcfg.Byte);
						@out.flush();
						aidEphTS = curTS;
					}
					if (msgAidHuiRate > 0 && curTS - aidHuiTS >= msgAidHuiRate * 1000)
					{
						Console.WriteLine(dateSys + " - " + COMPort + " - Polling AID-HUI message");
						msgcfg = new UBXMsgConfiguration(UBXMessageType.CLASS_AID, UBXMessageType.AID_HUI, msg);
						@out.write(msgcfg.Byte);
						@out.flush();
						aidHuiTS = curTS;
					}
					if (rxmRawMsgReceived) //curTS-sysOutTS >= 1*1000
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
						//sysOutTS = curTS;
						rxmRawMsgReceived = false;
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
			//if(streamEventListener!=null) streamEventListener.streamClosed();
		}

	//	/**
	//	 * @return the streamEventListener
	//	 */
	//	public StreamEventListener getStreamEventListener() {
	//		return streamEventListener;
	//	}

	//	/**
	//	 * @param streamEventListener the streamEventListener to set
	//	 */
	//	public void setStreamEventListener(StreamEventListener streamEventListener) {
	//		this.streamEventListener = streamEventListener;
	//		if(this.reader!=null) this.reader.setStreamEventListener(streamEventListener);
	//	}

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

		public virtual void enableAidEphMsg(int? ephRate)
		{
			this.msgAidEphRate = ephRate;
		}

		public virtual void enableAidHuiMsg(int? ionRate)
		{
			this.msgAidHuiRate = ionRate;
		}

		public virtual void enableSysTimeLog(bool? enableTim)
		{
			this.sysTimeLogEnabled = enableTim;
		}

		public virtual void enableNmeaMsg(IList<string> nmeaList)
		{
			this.requestedNmeaMsgs = nmeaList;
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

		protected internal virtual string prepareCOMStringForFilename(string COMPort)
		{
			string[] tokens = COMPort.Split("/", true);
			if (tokens.Length > 0)
			{
				COMPort = tokens[tokens.Length - 1].Trim(); //for UNIX /dev/tty* ports
			}
			return COMPort;
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