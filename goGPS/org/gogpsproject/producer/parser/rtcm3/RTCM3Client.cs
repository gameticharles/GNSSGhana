using System;
using System.Collections.Generic;
using System.Threading;

/*
 * Copyright (c) 2010 Eugenio Realini, Mirko Reguzzoni, Cryms sagl, Daisuke Yoshida. All Rights Reserved.
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

namespace org.gogpsproject.producer.parser.rtcm3
{


	using Coordinates = org.gogpsproject.positioning.Coordinates;
	using Time = org.gogpsproject.positioning.Time;
	using Bits = org.gogpsproject.util.Bits;
	using InputStreamCounter = org.gogpsproject.util.InputStreamCounter;

	public class RTCM3Client : Runnable, StreamResource, StreamEventProducer
	{

		private ConnectionSettings settings;
		private Thread dataThread;

		//private boolean waitForData = true;

		private bool running = false;
		/// <returns> the running </returns>
		public virtual bool Running
		{
			get
			{
				return running;
			}
		}

		private bool askForStop = false;
		private Dictionary<int?, Decode> decodeMap;

		/// <summary>
		/// Optional message handler for showing error messages. </summary>
		private int messagelength = 0;
		private int[] buffer;
		private bool[] bits;
		private bool[] rollbits;

		private InputStream @in = null;
		private Socket sck = null;
		private PrintWriter @out = null;

		private bool debug = false;

		private Coordinates virtualReferenceStationPosition = null;
		private Coordinates masterPosition = null;
		private AntennaDescriptor antennaDescriptor = null;

		private List<StreamEventListener> streamEventListeners = new List<StreamEventListener>();

		//private Vector<Observations> observationsBuffer = new Vector<Observations>();
		//private int obsCursor = 0;

		private string streamFileLogger = null;

		private string NtripGGA = null;
		private long lastNtripGGAsent = 0;
		private long NtripGGAsendDelay = 10 * 1000; // 10 sec

		public const int CONNECTION_POLICY_LEAVE = 0;
		public const int CONNECTION_POLICY_RECONNECT = 1;
		public const int CONNECTION_POLICY_WAIT = 2;
		private int reconnectionPolicy = CONNECTION_POLICY_RECONNECT;
		private long reconnectionWaitingTime = 300 * 1000; // 5 minutes

		public const int EXIT_NEVER = 0;
		public const int EXIT_ON_LAST_LISTENER_LEAVE = 1;
		private int exitPolicy = EXIT_ON_LAST_LISTENER_LEAVE;
		private bool online = false;
		private int week;
		private double currentTime;
		private double previousTime = -1;
		private string outputDir = "./test";
		private string markerName = "MMMM";

		/// <returns> the exitPolicy </returns>
		public virtual int ExitPolicy
		{
			get
			{
				return exitPolicy;
			}
		}

		/// <param name="exitPolicy"> the exitPolicy to set </param>
		/// <returns>  </returns>
		public virtual RTCM3Client setExitPolicy(int exitPolicy)
		{
			this.exitPolicy = exitPolicy;
		return this;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static RTCM3Client getInstance(String _host, int _port, String _username, String _password, String _mountpoint) throws Exception
		public static RTCM3Client getInstance(string _host, int _port, string _username, string _password, string _mountpoint)
		{
			return getInstance(_host, _port, _username, _password, _mountpoint, false);
		}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static RTCM3Client getInstance(String _host, int _port, String _username, String _password, String _mountpoint, boolean ldebug) throws Exception
		public static RTCM3Client getInstance(string _host, int _port, string _username, string _password, string _mountpoint, bool ldebug)
		{

			List<string> s = new List<string>();
			ConnectionSettings settings = new ConnectionSettings(_host, _port, _username, _password);
			List<string> mountpoints = new List<string>();
			RTCM3Client net = new RTCM3Client(settings);
			try
			{
				//System.out.println("Get sources");
				s = net.Sources;
				//System.out.println("Got sources");
			}
			catch (IOException e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
				throw new Exception(e);
			}
			for (int j = 1; j < s.Count; j++)
			{
				if (j % 2 == 0)
				{
					mountpoints.Add(s[j]);
				}
			}
			if (_mountpoint == null)
			{
				if (ldebug)
				{
					Console.WriteLine("Available Mountpoints:");
				}
			}
			for (int j = 0; j < mountpoints.Count; j++)
			{
				if (_mountpoint == null)
				{
					if (ldebug)
					{
						Console.WriteLine("\t[" + mountpoints[j] + "]");
					}
				}
				else
				{
					if (ldebug)
					{
						Console.Write("\t[" + mountpoints[j] + "][" + _mountpoint + "]");
					}
					if (_mountpoint.Equals(mountpoints[j], StringComparison.CurrentCultureIgnoreCase))
					{
						settings.setSource(mountpoints[j]);
						if (ldebug)
						{
							Console.Write(" found");
						}
					}
				}
				if (ldebug)
				{
					Console.WriteLine();
				}
			}
			if (settings.Source == null)
			{
				Console.WriteLine("Select a valid mountpoint!");
				return null;
			}
			return net;
		}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static RTCM3Client getVRSInstance(String _host, int _port, String _username, String _password, String _mountpoint, org.gogpsproject.positioning.Coordinates vrsPosition) throws Exception
		public static RTCM3Client getVRSInstance(string _host, int _port, string _username, string _password, string _mountpoint, Coordinates vrsPosition)
		{
			return getVRSInstance(_host, _port, _username, _password, _mountpoint, vrsPosition, false);
		}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static RTCM3Client getVRSInstance(String _host, int _port, String _username, String _password, String _mountpoint, org.gogpsproject.positioning.Coordinates vrsPosition, boolean debug) throws Exception
		public static RTCM3Client getVRSInstance(string _host, int _port, string _username, string _password, string _mountpoint, Coordinates vrsPosition, bool debug)
		{

			RTCM3Client rtcm = getInstance(_host, _port, _username, _password, _mountpoint, debug);
			rtcm.VirtualReferenceStationPosition = vrsPosition;
			return rtcm;
		}
		public RTCM3Client(ConnectionSettings settings) : base()
		{
			running = false;
			this.settings = settings;

			decodeMap = new Dictionary<int?, Decode>();

			decodeMap[new int?(1004)] = new Decode1004Msg(this);
			decodeMap[new int?(1005)] = new Decode1005Msg(this);
			decodeMap[new int?(1006)] = new Decode1006Msg(this);
			decodeMap[new int?(1007)] = new Decode1007Msg(this);
			decodeMap[new int?(1008)] = new Decode1008Msg(this);
			decodeMap[new int?(1012)] = new Decode1012Msg();
		}

		public RTCM3Client(int startWeek)
		{
			this.week = startWeek;
			running = false;

			decodeMap = new Dictionary<int?, Decode>();

			decodeMap[new int?(1004)] = new Decode1004Msg(this);
			decodeMap[new int?(1005)] = new Decode1005Msg(this);
			decodeMap[new int?(1006)] = new Decode1006Msg(this);
			decodeMap[new int?(1007)] = new Decode1007Msg(this);
			decodeMap[new int?(1008)] = new Decode1008Msg(this);
			decodeMap[new int?(1012)] = new Decode1012Msg();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public java.util.ArrayList<String> getSources() throws java.io.IOException
		public virtual List<string> Sources
		{
			get
			{
    
				//System.out.println("Open Socket "+settings.getHost()+" port "+ settings.getPort());
	//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	//ORIGINAL LINE: @SuppressWarnings("resource") java.net.Socket sck = new java.net.Socket(settings.getHost(), settings.getPort());
				Socket sck = new Socket(settings.Host, settings.Port);
    
				//System.out.println("Open streams");
				// The input and output streams are created
				PrintWriter @out = new PrintWriter(sck.OutputStream, true);
				InputStream sckIn = sck.InputStream;
				// A Buffered reader is created so we can read whole lines
				InputStreamReader inRead = new InputStreamReader(sckIn);
				BufferedReader @in = new BufferedReader(inRead);
    
				//System.out.println("Send request");
				// The data request containing the logon and password are send
				@out.print("GET / HTTP/1.1\r\n");
				@out.print("User-Agent: NTRIP goGPS-project java\r\n");
				@out.print("Host: " + settings.Host + "\r\n");
				@out.print("Connection: close\r\n");
				@out.print("Authorization: Basic " + settings.Pass_base64 + "\r\n");
				// out.println("Ntrip-GAA: $GPGGA,200530,4600,N,00857,E,4,10,1,200,M,1,M,3,0*65");
				// out.println("Accept: */*\r\nConnection: close");
				@out.print("\r\n");
				@out.flush();
    
				//System.out.println("Get answer");
				bool going = true;
				bool first = true;
				List<string> lines = new List<string>();
    
    
				while (going)
				{
					// The next byte is read and added to the buffer
    
					string newLine = @in.readLine();
    
					//System.out.println("Read:"+newLine);
					if (newLine == null)
					{
						going = false;
					}
					else if (first)
					{
						// The first line should be "SOURCETABLE 200 OK"
						if (!newLine.Equals("SOURCETABLE 200 OK"))
						{
							going = false;
						}
						first = false;
    
					}
					else
					{
						lines.Add(newLine);
					}
				}
    
				// Lines are parsed
				List<string> sources = new List<string>();
				for (int i = 0; i < lines.Count; i++)
				{
					// A new StringTokenizer is created with ";" as delimiter
    
					StringTokenizer token = new StringTokenizer(lines[i], ";");
					try
					{
						if (token.countTokens() > 1 && token.nextToken().Equals("STR"))
						{
    
							//System.out.println(lines.elementAt(i));
    
							// We excpect the correct source to be the first token after
							// "STR" to through the token wich specifies the RTCM
							// version
							// starting with "RTCM "
							// We haven't seen any specification of the sourcetable, but
							// according to what we can see from it it should be correct
							string s = token.nextToken();
							while (!s.StartsWith("RTCM 3", StringComparison.Ordinal))
							{
								sources.Add(s);
								s = token.nextToken();
							}
    
						}
					} // The line is ignored
					catch (NoSuchElementException)
					{
					}
				}
    
				@in.close();
				inRead.close();
				sckIn.close();
				@out.close();
    
				return sources;
			}
		}

		// public int[] readMessage() throws BufferUnderrunException {
		// return outputBuffer.readMessage();
		// }
		//
		// /**
		// * returns the number of messages ready for reading<B> uses
		// * CRWBuffer.ready()
		// */
		// public int ready() {
		// return outputBuffer.ready();
		// }


		public override void run()
		{

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

			TimeZone.Default = TimeZone.getTimeZone("UTC");
			DateTime cal = new DateTime();
			cal = new DateTime(DateTime.Now);
			DateTime date = cal.Ticks;

			SimpleDateFormat sdf1 = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss.SSS");
			string date1 = sdf1.format(date);
			SimpleDateFormat sdfFile = new SimpleDateFormat("yyyy-MM-dd_HHmmss");
			string dateFile = sdfFile.format(date);

			Console.WriteLine(date1 + " - Logging RTCM3 stream in " + outputDir + "/" + markerName + "_" + dateFile + ".rtcm");
			StreamFileLogger = outputDir + "/" + markerName + "_" + dateFile + ".rtcm";

			try
			{
				// Socket for receiving data are created

				try
				{
					Proxy proxy = Proxy.NO_PROXY;
					// proxy = new Proxy(Proxy.Type.SOCKS, new
					// InetSocketAddress("127.0.0.1", 8888));

					sck = new Socket(proxy);
					InetSocketAddress dest = new InetSocketAddress(settings.Host, settings.Port);
					sck.connect(dest);
					// sck = new Socket(settings.getHost(), settings.getPort());
					if (debug)
					{
						Console.WriteLine("Connected to " + settings.Host + ":" + settings.Port);
					}
					running = true;

				}
				catch (Exception e)
				{

					if (debug)
					{
						Console.WriteLine("Connection to " + settings.Host + ":" + settings.Port + " failed: \n  " + e);
					}
					// if (messages == null) {
					// tester.println("<" + settings.getSource() + ">" + msg);
					// } else {
					// messages.showErrorMessage(settings.getSource(), msg);
					// }
					closeAll();
					if (!askForStop && reconnectionPolicy == CONNECTION_POLICY_RECONNECT)
					{
						if (debug)
						{
							Console.WriteLine("Sleep " + reconnectionWaitingTime / 1000 + " s before retry");
						}
						Thread.Sleep(reconnectionWaitingTime);
						start();
					}
					else
					{
						foreach (StreamEventListener sel in streamEventListeners)
						{
							sel.streamClosed();
						}
					}
					return;
				}

				// The input and output streams are created
				@out = new PrintWriter(sck.OutputStream, true);
				@in = sck.InputStream;
				// The data request containing the logon and password is sent
				@out.print("GET /" + settings.Source + " HTTP/1.1\r\n");
				@out.print("Host: " + settings.Host + "\r\n");
				// out.print("Ntrip-Version: Ntrip/2.0\r\n");
				@out.print("Accept: rtk/rtcm, dgps/rtcm\r\n");
				@out.print("User-Agent: NTRIP goGPSprojectJava\r\n");
				if (virtualReferenceStationPosition != null)
				{
					virtualReferenceStationPosition.computeGeodetic();
					string hhmmss = (new SimpleDateFormat("HHmmss")).format(DateTime.Now);

					int h = (int) virtualReferenceStationPosition.GeodeticHeight;
					double lon = virtualReferenceStationPosition.GeodeticLongitude;
					double lat = virtualReferenceStationPosition.GeodeticLatitude;

					int lon_deg = (int) lon;
					double lon_min = (lon - lon_deg) * 60;
					double lon_nmea = lon_deg * 100 + lon_min;
					string lonn = (new DecimalFormat("00000.000")).format(lon_nmea);
					int lat_deg = (int) lat;
					double lat_min = (lat - lat_deg) * 60;
					double lat_nmea = lat_deg * 100 + lat_min;
					string latn = (new DecimalFormat("0000.000")).format(lat_nmea);
					NtripGGA = "$GPGGA," + hhmmss + "," + latn + "," + (lat < 0 ? "S" : "N") + "," + lonn + "," + (lon < 0 ? "W" : "E") + ",1,10,1.00," + (h < 0 ? 0 : h) + ",M,1,M,,";
					// String NtripGGA =
					// "$GPGGA,"+hhmmss+".00,"+latn+","+(lat<0?"S":"N")+","+lonn+","+(lon<0?"W":"E")+",1,10,1.00,"+(h<0?0:h)+",M,37.3,M,,";
					// NtripGGA =
					// "$GPGGA,214833.00,3500.40000000,N,13900.10000000,E,1,10,1,-17.3,M,,M,,";

					NtripGGA = NtripGGA + "*" + computeNMEACheckSum(NtripGGA); // "Ntrip-GAA: "+
					if (debug)
					{
						Console.WriteLine(NtripGGA);
					}

					// out.print(NtripGGA+"\r\n");
				}
				@out.print("Connection: close\r\n");
				@out.print("Authorization: Basic " + settings.Authbase64 + "\r\n");

				// out.println("User-Agent: NTRIP goGps");
				// out.println("Ntrip-GAA: $GPGGA,200530,4600,N,00857,E,4,10,1,200,M,1,M,3,0*65");
				// out.println("User-Agent: NTRIP GoGps");
				// out.println("Accept: */*\r\nConnection: close");
				@out.print("\r\n");
				if (NtripGGA != null)
				{
					@out.print(NtripGGA + "\r\n");
					lastNtripGGAsent = DateTimeHelperClass.CurrentUnixTimeMillis();
				}
				@out.flush();
				// System.out.println(" \n %%%%%%%%%%%%%%%%%%%%% \n password >>> "
				// + settings.getAuthbase64());
				// *****************
				// Reading the data

				// /First we read the HTTP header using a small state machine
				// The end of the header is received when a double end line
				// consisting
				// of a "new line" and a "carriage return" character has been received
				int state = 0;
				// First the HTTP header type is read. It should be "ICY 200 OK"
				// But Since we receive integers not characters the correct header is
				// numeric: 73 = 'I', 67 = 'C' and so on.

				int[] header = new int[11];
				int[] correctHeader = new int[] {73, 67, 89, 32, 50, 48, 48, 32, 79, 75, 13};
				int hindex = 0;
				// when 'running' is changed to false the loop is stopped

				while (running && state == 0)
				{
					int c = @in.read();
					if (debug)
					{
						Console.Write((char) c);
					}
					if (c < 0)
					{
						break;
					}
					// break;
					// tester.write(c);
					state = transition(state, c);
					if (hindex > 10)
					{
						// The header should only be 11 characters long
						running = false;
					}
					else
					{
						header[hindex] = c;
						hindex++;
					}
				}

				for (int i = 0; i < 11 && running; i++)
				{
					if (header[i] != correctHeader[i])
					{
						running = false;
					}
				}
				if (header[0] == 0)
				{
					if (debug)
					{
						Console.WriteLine("Waiting for connection acknowledgment message (\"ICY 200 OK\")...");
					}
					running = true;
				}
				if (!running)
				{
					for (int i = 0; i < header.Length; i++)
					{
						if (debug)
						{
							Console.Write((char) header[i]);
						}
					}
					int c = @in.read();
					while (c != -1)
					{
						if (debug)
						{
							Console.WriteLine(((int) c) + " " + (char) c);
						}

						c = @in.read();
					}
					if (debug)
					{
						Console.WriteLine(((int) c) + " " + (char) c);
					}

					//if (debug)
						//System.out.println();
					if (debug)
					{
						Console.WriteLine(settings.Source + " invalid header");
					}

					closeAll();
					if (!askForStop && reconnectionPolicy == CONNECTION_POLICY_RECONNECT)
					{
						Console.WriteLine("Sleep " + reconnectionWaitingTime / 1000 + " s before retry");
						Thread.Sleep(reconnectionWaitingTime);
						start();
					}
					else
					{
						foreach (StreamEventListener sel in streamEventListeners)
						{
							sel.streamClosed();
						}
					}
					return;
				}

				while (state != 5)
				{
					int c = @in.read();
					if (debug)
					{
						Console.WriteLine(((int) c) + " " + (char) c);
					}
					if (c < 0)
					{
						break;
					}
					// tester.write(c);
					state = transition(state, c);
				}
				// When HTTP header is read, the GPS data are recived and parsed:

				// The data is buffered as it is recived. When the buffer has size 6
				// There is a full word + a byte. The extra byte (first in buffer)
				// is
				// used for parity check.
				if (running)
				{
					// tester.println("<" + settings.getSource() +
					// ">Header least: OK");
					if (debug)
					{
						Console.WriteLine(settings.Source + " connected successfully");
					}
				}
				else
				{
					// showErrorMessage(settings.getSource(), "Error");
					if (debug)
					{
						Console.WriteLine(settings.Source + " not connected");
					}
					closeAll();
					if (!askForStop && reconnectionPolicy == CONNECTION_POLICY_RECONNECT)
					{
						if (debug)
						{
							Console.WriteLine("Sleep " + reconnectionWaitingTime / 1000 + " s before retry");
						}
						Thread.Sleep(reconnectionWaitingTime);
						start();
					}
					else
					{
						foreach (StreamEventListener sel in streamEventListeners)
						{
							sel.streamClosed();
						}
					}
					return;
				}
				// The read loop is started
				// sck.wait(1000);
				// this.dataThread.sleep(6000);
				// this.notifyAll();

				FileOutputStream fos = null;
				try
				{
					if (streamFileLogger != null)
					{
						fos = new FileOutputStream(streamFileLogger);
					}
				}
				catch (FileNotFoundException e)
				{
					Console.WriteLine(e.ToString());
					Console.Write(e.StackTrace);
				}

				InputStream isc = (fos == null ? @in : new InputStreamCounter(@in, fos));

				readLoop(isc, @out);
				// System.out.println("1");

			}
			catch (IOException ex)
			{
				Console.WriteLine(ex.ToString());
				Console.Write(ex.StackTrace);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				Console.Write(ex.StackTrace);

			}
			finally
			{
				// Connection was either terminated or an IOError occurred

				if (running)
				{
					if (debug)
					{
						Console.WriteLine(settings.Source + " connection error: the data stream stopped");
					}
				}
				else
				{
					if (debug)
					{
						Console.WriteLine(settings.Source + " connection closed by client");
					}
				}

				running = false;

				closeAll();

				// reconnect if needed
				if (!askForStop && reconnectionPolicy == CONNECTION_POLICY_RECONNECT)
				{
					if (debug)
					{
						Console.WriteLine("Sleep " + reconnectionWaitingTime / 1000 + " s before retry");
					}
					try
					{
						Thread.Sleep(reconnectionWaitingTime);
					}
					catch (InterruptedException e)
					{
						Console.WriteLine(e.ToString());
						Console.Write(e.StackTrace);
					}
					start();
				}
				else
				{
					foreach (StreamEventListener sel in streamEventListeners)
					{
						sel.streamClosed();
					}
				}
			}
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
		public virtual void start()
		{
			askForStop = false;

			dataThread = new Thread(this);
			dataThread.Name = "RTCM3Client " + settings.Host + " " + settings.Source;
			dataThread.Start();
		}

		private void closeAll()
		{
			// All connections are closed
			if (@out != null)
			{
				try
				{
				@out.close();
				}
			catch (Exception)
			{
			}
			}
			if (@in != null)
			{
				try
				{
				@in.close();
				}
			catch (Exception)
			{
			}
			}
			if (sck != null)
			{
				try
				{
				sck.close();
				}
			catch (Exception)
			{
			}
			}
		}

		/// <summary>
		/// stops the execution of this thread </summary>
		/// <exception cref="InterruptedException">  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void stop(boolean waitForThread, long timeoutMs) throws InterruptedException
		public virtual void stop(bool waitForThread, long timeoutMs)
		{
			askForStop = true;
			running = false;
			// disable waitForData to avoid wait forever in nextObservations()
			//waitForData = false;

			if (waitForThread && dataThread != null)
			{
				try
				{
					dataThread.Join(timeoutMs);
				}
				catch (InterruptedException ie)
				{
					Console.WriteLine(ie.ToString());
					Console.Write(ie.StackTrace);
				}
				if (dataThread.IsAlive)
				{
					if (debug)
					{
						Console.WriteLine("Killing thread " + dataThread.Name);
					}
					dataThread.Interrupt();
				}
			}

		}

		/// <summary>
		/// returns true if the data thread still is alive </summary>
		public virtual bool stopped()
		{
			// return true;
			return dataThread != null && !dataThread.IsAlive;
		}

		public virtual int transition(int state, int input)
		{
			switch (state)
			{
				case 0:
				{
					if (input == 13)
					{
						state = 1;
					}
					break;
				}

				case 1:
				{
					if (input == 13)
					{
						state = 2;
					}
					break;
				}
				case 2:
				{
					if (input == 10)
					{
						state = 5;
					}
					else
					{
						state = 1;
					}
					break;
				}
				case 3:
				{
					if (input == 13)
					{
						state = 4;
					}
					else
					{
						state = 1;
					}
					break;
				}
				case 4:
				{
					if (input == 10)
					{
						state = 5;
					}
					else
					{
						state = 1;
					}
					break;
				}
			}

			return state;
		}

		/// <summary>
		/// reads data from an InputStream while running is true
		/// </summary>
		/// <param name="in">
		///            input stream to read from </param>
		/// <returns>  </returns>

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void readLoop(java.io.InputStream in,java.io.PrintWriter out) throws java.io.IOException
		protected internal virtual void readLoop(InputStream @in, PrintWriter @out)
		{
			int c;
			long start = DateTimeHelperClass.CurrentUnixTimeMillis();
			if (debug)
			{
				Console.Write("Waiting for header");
			}
			online = true;

			while (running)
			{
				c = @in.read();

				if (c < 0)
				{
					if (reconnectionPolicy != CONNECTION_POLICY_WAIT && DateTimeHelperClass.CurrentUnixTimeMillis() - start > 10 * 1000)
					{
						break;
					}
					try
					{
						Thread.Sleep(1000);
					}
					catch (InterruptedException e)
					{
						Console.WriteLine(e.ToString());
						Console.Write(e.StackTrace);
					}
					if (debug)
					{
						Console.Write(".");
					}
				}
	//			Object o = null;
	//			if (header) {
					//if(debug) System.out.println("Header : " + c);
					if (c == 211) // header
					{
						readMessage(@in);
					}
	//			}

	//			if(o instanceof Observations){
	//				Observations oo = (Observations)o;
	//				if(streamEventListeners!=null && oo!=null){
	//					for(StreamEventListener sel:streamEventListeners){
	//						//Observations co = sel.getCurrentObservations();
	//					    //sel.pointToNextObservations();
	//						Observations oc = (Observations) oo.clone();
	//					    sel.addObservations(oc);
	//					}
	//				}
	//			}

				if (@out != null && DateTimeHelperClass.CurrentUnixTimeMillis() - lastNtripGGAsent > NtripGGAsendDelay)
				{
					@out.print(NtripGGA + "\r\n");
					@out.flush();
					lastNtripGGAsent = DateTimeHelperClass.CurrentUnixTimeMillis();

					int h = (int) virtualReferenceStationPosition.GeodeticHeight;
					double lon = virtualReferenceStationPosition.GeodeticLongitude;
					double lat = virtualReferenceStationPosition.GeodeticLatitude;
					string hhmmss = (new SimpleDateFormat("HHmmss")).format(DateTime.Now);
					int lon_deg = (int) lon;
					double lon_min = (lon - lon_deg) * 60;
					double lon_nmea = lon_deg * 100 + lon_min;
					string lonn = (new DecimalFormat("00000.000")).format(lon_nmea);
					int lat_deg = (int) lat;
					double lat_min = (lat - lat_deg) * 60;
					double lat_nmea = lat_deg * 100 + lat_min;
					string latn = (new DecimalFormat("0000.000")).format(lat_nmea);

					NtripGGA = "$GPGGA," + hhmmss + "," + latn + "," + (lat < 0 ? "S" : "N") + "," + lonn + "," + (lon < 0 ? "W" : "E") + ",1,10,1.00," + (h < 0 ? 0 : h) + ",M,1,M,,";
					// String NtripGGA =
					// "$GPGGA,"+hhmmss+".00,"+latn+","+(lat<0?"S":"N")+","+lonn+","+(lon<0?"W":"E")+",1,10,1.00,"+(h<0?0:h)+",M,37.3,M,,";
					// NtripGGA =
					// "$GPGGA,214833.00,3500.40000000,N,13900.10000000,E,1,10,1,-17.3,M,,M,,";

					NtripGGA = NtripGGA + "*" + computeNMEACheckSum(NtripGGA); // "Ntrip-GAA: "+

					Console.WriteLine("refresh ntripGGA:" + NtripGGA);
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object readMessage(java.io.InputStream in) throws java.io.IOException
		public virtual object readMessage(InputStream @in)
		{
			int index;
			object o = null;
			index = 0;
			buffer = new int[2];
			buffer[0] = @in.read();
			buffer[1] = @in.read();
			bits = new bool[buffer.Length * 8];
			rollbits = new bool[8];
			for (int i = 0; i < buffer.Length; i++)
			{
				rollbits = Bits.rollByteToBits(buffer[i]);
				for (int j = 0; j < rollbits.Length; j++)
				{
					bits[index] = rollbits[j];
					index++;
				}
			}
			messagelength = (int)Bits.bitsToUInt(Bits.subset(bits, 6, 10));
			if (debug)
			{
				Console.WriteLine();
				Console.WriteLine("Debug message length : " + messagelength);
			}

			if (messagelength >= 12)
			{
				setBits(@in, messagelength);
				int msgtype = (int)Bits.bitsToUInt(Bits.subset(bits, 0, 12));

				if (debug)
				{
					Console.WriteLine("message type : " + msgtype);
				}
				messagelength = 0;

				DateTime date = DateTime.Now;
				SimpleDateFormat sdf1 = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss.SSS");
				string date1 = sdf1.format(date);

				Decode dec = decodeMap[new int?(msgtype)];
				if (dec != null)
				{
					if (online)
					{
						Time currentTime = new Time(DateTimeHelperClass.CurrentUnixTimeMillis());
						o = dec.decode(bits, currentTime.GpsWeek);
						Console.WriteLine(date1 + " - RTCM message " + msgtype + " received and decoded");
					}
					else
					{
						o = dec.decode(bits, week);
						if (o is Observations)
						{
							if (((Observations) o).RefTime.DayOfYear == 11)
							{
								week++;
								week--;
							}
							currentTime = ((Observations) o).RefTime.GpsTime;
							if (currentTime < previousTime)
							{
								week++;
								((Observations) o).RefTime = new Time(week, currentTime);
							}
							previousTime = currentTime;
						}
					}
					if (o is Observations)
					{
						addObservation((Observations) o);
					}
				}
				else
				{
					//System.err.println("missing RTCM message parser "+msgtype);
					// missing message parser
				}

				// CRC
				setBits(@in, 3);

				// setBits(in,1);
				//if(debug) System.out.println(" dati :" + Bits.bitsToStr(bits));
			}
			return o;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void setBits(java.io.InputStream in, int bufferlength) throws java.io.IOException
		private void setBits(InputStream @in, int bufferlength)
		{
			int index = 0;
			buffer = new int[bufferlength];
			bits = new bool[buffer.Length * 8];
			for (int i = 0; i < buffer.Length; i++)
			{
				buffer[i] = @in.read();
			}
			// index = 0;
			for (int i = 0; i < buffer.Length; i++)
			{
				rollbits = Bits.rollByteToBits(buffer[i]);
				for (int j = 0; j < 8; j++)
				{
					bits[index] = rollbits[j];
					index++;
				}
			}
		}

		public virtual void addObservation(Observations o)
		{
			if (streamEventListeners != null && o != null)
			{
				foreach (StreamEventListener sel in streamEventListeners)
				{
					Observations oc = (Observations)o.clone();
					sel.addObservations(oc);
				}
			}
	//		if(debug){
	//			System.out.println("\t\t\t\tM > obs "+o.getGpsSize()+" time "+new Date(o.getRefTime().getMsec()));
	//			for(int i=0;i<o.getGpsSize();i++){
	//				ObservationSet os = o.getGpsByIdx(i);
	//				System.out.print(" svid:"+os.getSatID());
	//				System.out.print(" codeC:"+os.getCodeC(0));
	//				System.out.print(" codeP:"+os.getCodeP(0));
	//				System.out.print(" doppl:"+os.getDoppler(0));
	//				System.out.print(" LLInd:"+os.getLossLockInd(0));
	//				System.out.print(" phase:"+os.getPhase(0));
	//				System.out.print(" pseud:"+os.getPseudorange(0));
	//				System.out.print(" q.ind:"+os.getQualityInd(0));
	//				System.out.println(" s.str:"+os.getSignalStrength(0));
	//			}
	//		}
	//		observationsBuffer.add(o);
		}

		/* (non-Javadoc)
		 * @see org.gogpsproject.ObservationsProducer#getApproxPosition()
		 */
		//@Override
		public virtual Coordinates MasterPosition
		{
			get
			{
				return masterPosition;
			}
		}

	//	/* (non-Javadoc)
	//	 * @see org.gogpsproject.ObservationsProducer#getCurrentObservations()
	//	 */
	//	//@Override
	//	public Observations getCurrentObservations() {
	//		if(obsCursor>=observationsBuffer.size()){
	//			if(waitForData){
	//				while(obsCursor>=observationsBuffer.size()){
	//					if(debug) System.out.print("m");
	//					try {
	//						Thread.sleep(1000);
	//					} catch (InterruptedException e) {}
	//				}
	//			}else{
	//				return null;
	//			}
	//		}
	//		return observationsBuffer.get(obsCursor);
	//	}

		/* (non-Javadoc)
		 * @see org.gogpsproject.ObservationsProducer#init()
		 */
		//@Override
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void init() throws Exception
		public virtual void init()
		{
			start();
		}

	//	/* (non-Javadoc)
	//	 * @see org.gogpsproject.ObservationsProducer#nextObservations()
	//	 */
	//	//@Override
	//	public Observations nextObservations() {
	//		if(observationsBuffer.size()==0 || (obsCursor+1)>=observationsBuffer.size()){
	//			if(waitForData){
	//				while(observationsBuffer.size()==0 || (obsCursor+1)>=observationsBuffer.size()){
	//					if(debug) System.out.println("\t\t\t\tM cur:"+obsCursor+" pool:"+observationsBuffer.size());
	//					try {
	//						Thread.sleep(1000);
	//					} catch (InterruptedException e) {}
	//				}
	//			}else{
	//				return null;
	//			}
	//		}
	//		Observations o = observationsBuffer.get(++obsCursor);
	//		if(debug) System.out.println("\t\t\t\tM < Obs "+o.getRefTime().getMsec());
	//        return o;
	//	}

		/* (non-Javadoc)
		 * @see org.gogpsproject.ObservationsProducer#release()
		 */
		//@Override
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void release(boolean waitForThread, long timeoutMs) throws InterruptedException
		public virtual void release(bool waitForThread, long timeoutMs)
		{
			stop(waitForThread, timeoutMs);
		}

		/// <param name="initialPosition"> the initialPosition to set </param>
		/// <returns>  </returns>
		public virtual RTCM3Client setMasterPosition(Coordinates masterPosition)
		{
			if (debug)
			{
				masterPosition.computeGeodetic();
				Console.WriteLine("Master Position : " + masterPosition);
			}
			this.masterPosition = masterPosition;
			foreach (StreamEventListener sel in streamEventListeners)
			{
				sel.DefinedPosition = masterPosition;
			}
		return this;
		}

		/// <returns> the streamFileLogger </returns>
		public virtual string StreamFileLogger
		{
			get
			{
				return streamFileLogger;
			}
		}

		/// <param name="streamFileLogger"> the streamFileLogger to set </param>
		/// <returns>  </returns>
		public virtual RTCM3Client setStreamFileLogger(string streamFileLogger)
		{
			this.streamFileLogger = streamFileLogger;
		return this;
		}

		/// <returns> the debug </returns>
		public virtual bool Debug
		{
			get
			{
				return debug;
			}
		}

		/// <param name="debug"> the debug to set </param>
		/// <returns>  </returns>
		public virtual RTCM3Client setDebug(bool debug)
		{
			this.debug = debug;
		return this;
		}

		/// <param name="reconnectionPolicy"> the reconnectionPolicy to set </param>
		/// <returns>  </returns>
		public virtual RTCM3Client setReconnectionPolicy(int reconnectionPolicy)
		{
			this.reconnectionPolicy = reconnectionPolicy;
		return this;
		}

		/// <returns> the reconnectionPolicy </returns>
		public virtual int ReconnectionPolicy
		{
			get
			{
				return reconnectionPolicy;
			}
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
			// feed defined position
			if (masterPosition != null)
			{
				streamEventListener.DefinedPosition = masterPosition;
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

			if (exitPolicy == EXIT_ON_LAST_LISTENER_LEAVE && streamEventListeners.Count == 0)
			{
				try
				{
					release(true, 10 * 1000);
				}
				catch (InterruptedException e)
				{
					Console.WriteLine(e.ToString());
					Console.Write(e.StackTrace);
				}
			}
		}

		/// <param name="antennaDescriptor"> the antennaDescriptor to set </param>
		/// <returns>  </returns>
		public virtual RTCM3Client setAntennaDescriptor(AntennaDescriptor antennaDescriptor)
		{
			if (debug)
			{
				Console.WriteLine("Antenna Descriptor : " + antennaDescriptor);
			}
			this.antennaDescriptor = antennaDescriptor;
		return this;
		}

		/// <returns> the antennaDescriptor </returns>
		public virtual AntennaDescriptor AntennaDescriptor
		{
			get
			{
				return antennaDescriptor;
			}
		}

		/// <param name="virtualReferenceStationPosition"> the virtualReferenceStationPosition to set </param>
		/// <returns>  </returns>
		public virtual RTCM3Client setVirtualReferenceStationPosition(Coordinates virtualReferenceStationPosition)
		{
			this.virtualReferenceStationPosition = virtualReferenceStationPosition;
			return this;
		}

		/// <returns> the virtualReferenceStationPosition </returns>
		public virtual Coordinates VirtualReferenceStationPosition
		{
			get
			{
				return virtualReferenceStationPosition;
			}
		}

		public virtual RTCM3Client setMarkerName(string markerName)
		{
			this.markerName = markerName;
		return this;
		}

		public virtual RTCM3Client setOutputDir(string outDir)
		{
			this.outputDir = outDir;
			return this;
		}

		public virtual RTCM3Client setReconnectionWaitingTime(int? waitingTime)
		{
			this.reconnectionWaitingTime = waitingTime * 1000;
		return this;
		}
	}

}