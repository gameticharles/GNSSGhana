using System;
using System.Collections.Generic;
using System.Text;
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
namespace org.gogpsproject.producer.parser.ublox
{


	using EphGps = org.gogpsproject.ephemeris.EphGps;
	using EphemerisSystem = org.gogpsproject.ephemeris.EphemerisSystem;
	using SatellitePosition = org.gogpsproject.positioning.SatellitePosition;

	/// <summary>
	/// <para>
	/// Provide AssistNow service from uBlox as NavigationProducer interface
	/// </para>
	/// 
	/// @author Lorenzo Patocchi cryms.com
	/// </summary>
	public class UBXAssistNow : EphemerisSystem, NavigationProducer, Runnable
	{

		public const string ASSISTNOW_SERVER = "agps.u-blox.com";
		public const int ASSISTNOW_PORT = 46434;
		public const string ASSISTNOW_REQUEST = "cmd=${cmd};user=${user};pwd=${pass};lat=${lat};lon=${lon};pacc=${pacc}";

		// delivers Ephemeris and Almanac data and Approximate Time and Position to the client
		public const string CMD_FULL = "full";
		// identical to "full", but does not deliver Almanac
		public const string CMD_AID = "aid";
		//  only delivers Ephemeris which is of use to the client at its current location
		public const string CMD_eph = "eph";
		// delivers Almanac data for the full GPS constellation
		public const string CMD_ALM = "alm";

		public const string DEFAULT_PACC = "300000";

		private const int LAT_MIN = -45;
		private const int LAT_MAX = +45;
		private const int LAT_STEP = 90;
		private const int LON_MIN = -180;
		private const int LON_MAX = +90;
		private const int LON_STEP = 90;


		private string user, pass, cmd, lon = null, lat = null;

		private List<EphGps> ephs = new List<EphGps>(); // GPS broadcast ephemerides
		private List<IonoGps> ionos = new List<IonoGps>(); // GPS broadcast ionospheric

		private Thread t = null;

		private long requestSecondDelay = 15 * 60; // 15 min

		private bool debug = false;
		private string fileNameOutLog = null;
		private FileOutputStream fosOutLog = null;
		private DataOutputStream outLog = null; //new XMLEncoder(os);


		/// <param name="args"> </param>
		public static void Main(string[] args)
		{
			UBXAssistNow agps = new UBXAssistNow(args[0], args[1], CMD_AID); //, "8.92", "46.03"

			try
			{
				//agps.requestSecondDelay = 60;
				agps.Debug = true;
				agps.FileNameOutLog = "./data/assistnow.dat";

				agps.init();

				Thread.Sleep(60 * 60 * 1000);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			finally
			{
				try
				{
					agps.release(true, 10 * 1000);
				}
				catch (InterruptedException e)
				{
					Console.WriteLine(e.ToString());
					Console.Write(e.StackTrace);
				}
			}


		}


		public UBXAssistNow(string username, string password, string cmd, string lon, string lat)
		{
			this.user = username;
			this.pass = password;
			this.cmd = cmd;
			this.lon = lon;
			this.lat = lat;
		}
		public UBXAssistNow(string username, string password, string cmd)
		{
			this.user = username;
			this.pass = password;
			this.cmd = cmd;
		}

		public virtual ByteArrayOutputStream doRequest()
		{



			List<string> lats = new List<string>();
			List<string> lons = new List<string>();

			if (lat != null && lon != null)
			{
				lats.Add(lat);
				lons.Add(lon);
			}
			else
			{
				for (int la = LAT_MIN;la <= LAT_MAX;la += LAT_STEP)
				{
					for (int lo = LON_MIN;lo <= LON_MAX;lo += LON_STEP)
					{
						lats.Add("" + la);
						lons.Add("" + lo);
					}
				}
			}
			ByteArrayOutputStream cache = new ByteArrayOutputStream();

			for (int s = 0;s < lats.Count;s++)
			{

				string llat = lats[s];
				string llon = lons[s];

				string request = ASSISTNOW_REQUEST + "\n";
				request = request.replaceAll("\\$\\{cmd\\}", cmd);
				request = request.replaceAll("\\$\\{user\\}", user);
				request = request.replaceAll("\\$\\{pass\\}", pass);
				request = request.replaceAll("\\$\\{lat\\}", llat);
				request = request.replaceAll("\\$\\{lon\\}", llon);
				request = request.replaceAll("\\$\\{pacc\\}", DEFAULT_PACC);


				InputStream @is = null;
				OutputStream os = null;
				Socket sck = null;
				try
				{

					int retry = 3;
					while (retry > 0)
					{


						try
						{
							/* Open all */
							sck = new Socket(ASSISTNOW_SERVER, ASSISTNOW_PORT);

							os = sck.OutputStream;
							@is = sck.InputStream;

							if (debug)
							{
								Console.WriteLine("[" + request + "]");
							}
							os.write(request.GetBytes("UTF-8"));
							os.flush();

							int lenght = -1;
							string responseLine;
							bool start = false;
							int lines = 0;
							while ((responseLine = readLine(@is)) != null && !start)
							{
								if (debug)
								{
									Console.WriteLine("[" + responseLine + "]");
								}

								string key = "Content-Length: ";
								if (responseLine.IndexOf(key, StringComparison.Ordinal) > -1)
								{

									lenght = Convert.ToInt32(responseLine.Substring(responseLine.IndexOf(key, StringComparison.Ordinal) + key.Length));
									if (debug)
									{
										Console.WriteLine("len [" + lenght + "]");
									}

								}
								key = "Content-Type: application/ubx";
								if (responseLine.IndexOf(key, StringComparison.Ordinal) > -1)
								{
									start = true;
								}
								key = "error:";
								if (responseLine.IndexOf(key, StringComparison.Ordinal) > -1)
								{
									throw new Exception(responseLine);
								}
								if (lines++ > 20)
								{
									throw new Exception("Read more than 20 lines of header");
								}
							}

							int init = cache.size();
							int tot = 0;
							sbyte[] buf = new sbyte[1024];
							int c = @is.read(buf,0,buf.Length);
							while (c >= 0)
							{
								tot += c;
								cache.write(buf,0,c);
								if (debug)
								{
									Console.WriteLine("Read: " + c + " Tot:" + tot);
								}
								c = @is.read(buf,0,buf.Length);
							}
							if (lenght == (cache.size() - init))
							{
								if (debug)
								{
									Console.WriteLine("Successfull read");
								}
								retry = 0;
							}
							else
							{
								if (debug)
								{
									Console.WriteLine("Read err " + lenght + "!=" + (cache.size() - init));
								}
								sbyte[] tmp = cache.toByteArray();
								cache.reset();
								cache.write(tmp, 0, init);

								//cache = null;
								Thread.Sleep(1000);
							}
						}
						catch (IOException e)
						{
							Console.WriteLine(e.ToString());
							Console.Write(e.StackTrace);
						}
						catch (Exception e)
						{
							Console.WriteLine(e.ToString());
							Console.Write(e.StackTrace);
						}
						finally
						{
							try
							{
								@is.close();
							}
							catch (Exception)
							{
							}
							try
							{
								os.close();
							}
							catch (Exception)
							{
							}
							try
							{
								sck.close();
							}
							catch (Exception)
							{
							}
						}
						retry--;
					}
				}
				catch (Exception e)
				{
					Console.WriteLine(e.ToString());
					Console.Write(e.StackTrace);
				}
			}

	//	        if(cache!=null){
	//	            // Write to GPS
	//
	//	            if(coldRestart){
	//	            	dialog.setLine(2,"GPS cold start");
	//	            	gpsos.write(coldStartMessage);
	//	            }
	//	            dialog.setLine(2,"Write to GPS");
	//	        	gpsos.write(cache.toByteArray());
	//	        	dialog.setLine(2,"Write successful");
	//	        }else{
	//	        	dialog.setLine(2,"Missing A-GPS data");
	//	        }
				// real bytes




			return cache;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static String readLine(java.io.InputStream is) throws java.io.IOException
		private static string readLine(InputStream @is)
		{
			StringBuilder str = new StringBuilder();
			int ch = @is.read();
			int chh = ch;
			while (ch != -1 && ch != '\n' && !(chh == '\r' && ch == '\n'))
			{
				if (ch != '\r')
				{
					str.Append((char) ch);
				}
				chh = ch;
				ch = @is.read();
			}
			return str.ToString();
		}


		/* (non-Javadoc)
		 * @see org.gogpsproject.NavigationProducer#getGpsSatPosition(long, int, double, double)
		 */
		public virtual SatellitePosition getGpsSatPosition(Observations obs, int satID, char satType, double receiverClockError)
		{
			long unixTime = obs.RefTime.Msec;

			EphGps eph = findEph(unixTime, satID);

			if (eph != null)
			{

	//			char satType = eph.getSatType();

				SatellitePosition sp = computePositionGps(obs, satID, satType, eph, receiverClockError);
				//if(receiverPosition!=null) earthRotationCorrection(receiverPosition, sp);
				return sp; // new SatellitePosition(eph, unixTime, satID, range);
			}
			return null;
		}

		/// <param name="unixTime"> </param>
		/// <param name="satID"> </param>
		/// <returns> Reference ephemeris set for given time and satellite </returns>
		public virtual EphGps findEph(long unixTime, int satID)
		{

			long dt = 0;
			long dtMin = 0;
			EphGps refEph = null;

			//long gpsTime = (new Time(unixTime)).getGpsTime();

			for (int i = 0; i < ephs.Count; i++)
			{
				// Find ephemeris sets for given satellite
				if (ephs[i].SatID == satID)
				{
					// Compare current time and ephemeris reference time
					dt = Math.Abs(ephs[i].RefTime.Msec - unixTime);
					// If it's the first round, set the minimum time difference and
					// select the first ephemeris set candidate
					if (refEph == null)
					{
						dtMin = dt;
						refEph = ephs[i];
						// Check if the current ephemeris set is closer in time than
						// the previous candidate; if yes, select new candidate
					}
					else if (dt < dtMin)
					{
						dtMin = dt;
						refEph = ephs[i];
					}
				}
			}
			return refEph;
		}


		/* (non-Javadoc)
		 * @see org.gogpsproject.NavigationProducer#getIono(long)
		 */
		public virtual IonoGps getIono(long unixTime)
		{
			long dt = 0;
			long dtMin = 0;
			IonoGps refIono = null;

			//long gpsTime = (new Time(unixTime)).getGpsTime();

			for (int i = 0; i < ionos.Count; i++)
			{
				// Find ionospheric sets for given satellite

				// Compare current time and ionospheric reference time
				dt = Math.Abs(ionos[i].RefTime.Msec - unixTime);
				// If it's the first round, set the minimum time difference and
				// select the first ionospheric set candidate
				if (refIono == null)
				{
					dtMin = dt;
					refIono = ionos[i];
					// Check if the current ionospheric set is closer in time than
					// the previous candidate; if yes, select new candidate
				}
				else if (dt < dtMin)
				{
					dtMin = dt;
					refIono = ionos[i];
				}

			}
			return refIono;
		}


		/* (non-Javadoc)
		 * @see org.gogpsproject.NavigationProducer#init()
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void init() throws Exception
		public virtual void init()
		{
			this.t = new Thread(this);
			this.t.Name = "uBlox AssistNow A-GPS";
			t.Start();
		}


		/* (non-Javadoc)
		 * @see org.gogpsproject.NavigationProducer#release(boolean, long)
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void release(boolean waitForThread, long timeoutMs) throws InterruptedException
		public virtual void release(bool waitForThread, long timeoutMs)
		{
			Thread tt = t;

			t = null;
			if (waitForThread && tt != null && tt.IsAlive)
			{
				tt.Join(timeoutMs);
			}
		}


		/* (non-Javadoc)
		 * @see java.lang.Runnable#run()
		 */
		public override void run()
		{
			long lastRequest = 0;
			while (t != null && t == Thread.CurrentThread)
			{
				long now = DateTimeHelperClass.CurrentUnixTimeMillis();
				if ((now - lastRequest) / 1000 > requestSecondDelay)
				{
					lastRequest = now;

					ByteArrayOutputStream os = this.doRequest();
					ByteArrayInputStream @is = new ByteArrayInputStream(os.toByteArray());
					UBXReader reader = new UBXReader(@is);

					while (@is.available() > 0)
					{
						int data = @is.read();
						if (data == 0xB5)
						{
							try
							{
								object msg = reader.readMessage();
								if (msg != null)
								{
									//System.out.println("msg "+msg.getClass().getName());
									if (msg is EphGps)
									{
										if (debug)
										{
											Console.WriteLine("Ephemeris for SatID:" + ((EphGps)msg).SatID + " time:" + (new DateTime(((EphGps)msg).RefTime.Msec)));
										}
										ephs.Add((EphGps)msg);

										if (outLog != null)
										{
											try
											{
												((EphGps)msg).write(outLog);
												outLog.flush();
											}
											catch (IOException e)
											{
												Console.WriteLine(e.ToString());
												Console.Write(e.StackTrace);
											}
										}
									}
									if (msg is IonoGps)
									{
										if (debug)
										{
											Console.WriteLine("Iono " + (new DateTime(((IonoGps)msg).RefTime.Msec)));
										}
										ionos.Add((IonoGps)msg);
										if (outLog != null)
										{
											try
											{
												((IonoGps)msg).write(outLog);
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
								else
								{
									if (debug)
									{
										Console.WriteLine("msg unknown");
									}
								}
							}
							catch (IOException e)
							{
								Console.WriteLine(e.ToString());
								Console.Write(e.StackTrace);
							}
							catch (UBXException e)
							{
								Console.WriteLine(e.ToString());
								Console.Write(e.StackTrace);
							}
						}
					}
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
			}
		}

		/// <param name="requestSecondDelay"> the requestSecondDelay to set </param>
		public virtual long RequestSecondDelay
		{
			set
			{
				this.requestSecondDelay = value;
			}
			get
			{
				return requestSecondDelay;
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
	}

}