using System;
using System.Collections.Generic;

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
namespace org.gogpsproject.producer.parser.sp3
{

	/// <summary>
	/// <para>
	/// This file parses SP3c satellite positioning files
	/// </para>
	/// 
	/// @author Lorenzo Patocchi cryms.com
	/// </summary>

	using SimpleMatrix = org.ejml.simple.SimpleMatrix;
	using EphGps = org.gogpsproject.ephemeris.EphGps;
	using Coordinates = org.gogpsproject.positioning.Coordinates;
	using SatellitePosition = org.gogpsproject.positioning.SatellitePosition;
	using Time = org.gogpsproject.positioning.Time;

	/// <summary>
	/// @author Lorenzo Patocchi
	/// 
	/// Still incomplete
	/// </summary>
	public class SP3Parser : NavigationProducer
	{

		public static string newline = System.getProperty("line.separator");

		private File fileSP3;
		private FileInputStream fileInputStream;
		private InputStreamReader inputStreamReader;
		private BufferedReader bufferedReader;

		private FileOutputStream cacheOutputStream;
		private OutputStreamWriter cacheStreamWriter;

		private int gpsWeek = 0;
		private int secondsOfWeek = 0;
		private int epochInterval = 0;
		private int nepocs = 0;
		private int numSats = 0;
		private string coordSys = null;
		private string orbitType = null;
		private string agency = null;

		private List<Dictionary<string, SatellitePosition>> epocs;
		private List<Time> epocTimestamps;
		private List<string> satIDs;
		private Dictionary<string, long?> accuracy;

		private double posStDevBase;
		private double clockStDevBase;

		public static void Main(string[] args)
		{
			File f = new File("./data/igu15231_00.sp3");
			SP3Parser sp3fp = new SP3Parser(f);
			sp3fp.init();
		}

		// RINEX Read constructors
		public SP3Parser(File fileSP3)
		{
			this.fileSP3 = fileSP3;
		}

		// RINEX Read constructors
		public SP3Parser(InputStream @is, File cache)
		{
			this.inputStreamReader = new InputStreamReader(@is);
			if (cache != null)
			{
				File path = cache.ParentFile;
				if (!path.exists())
				{
					path.mkdirs();
				}
				try
				{
					cacheOutputStream = new FileOutputStream(cache);
					cacheStreamWriter = new OutputStreamWriter(cacheOutputStream);
				}
				catch (FileNotFoundException e)
				{
					Console.Error.WriteLine("Exception writing " + cache);
					Console.WriteLine(e.ToString());
					Console.Write(e.StackTrace);
				}
			}
		}


		public virtual void init()
		{
			open();
			if (parseHeader())
			{
				parseData();
			}
			close();

			//System.out.println("Found "+epocs.size()+" epocs");
		}


		/// 
		public virtual void open()
		{
			try
			{
				if (fileSP3 != null)
				{
					fileInputStream = new FileInputStream(fileSP3);
				}
				if (fileInputStream != null)
				{
					inputStreamReader = new InputStreamReader(fileInputStream);
				}
				if (inputStreamReader != null)
				{
					bufferedReader = new BufferedReader(inputStreamReader);
				}
			}
			catch (FileNotFoundException e1)
			{
				Console.WriteLine(e1.ToString());
				Console.Write(e1.StackTrace);
			}
		}

		public virtual void close()
		{
			try
			{
				if (cacheStreamWriter != null)
				{
					cacheStreamWriter.flush();
					cacheStreamWriter.close();
				}
				if (cacheOutputStream != null)
				{
					cacheOutputStream.flush();
					cacheOutputStream.close();
				}
			}
			catch (FileNotFoundException e1)
			{
				Console.WriteLine(e1.ToString());
				Console.Write(e1.StackTrace);
			}
			catch (IOException e2)
			{
				Console.WriteLine(e2.ToString());
				Console.Write(e2.StackTrace);
			}
			try
			{
				if (bufferedReader != null)
				{
					bufferedReader.close();
				}
				if (inputStreamReader != null)
				{
					inputStreamReader.close();
				}
				if (fileInputStream != null)
				{
					fileInputStream.close();
				}
			}
			catch (FileNotFoundException e1)
			{
				Console.WriteLine(e1.ToString());
				Console.Write(e1.StackTrace);
			}
			catch (IOException e2)
			{
				Console.WriteLine(e2.ToString());
				Console.Write(e2.StackTrace);
			}
		}
		/* (non-Javadoc)
		 * @see org.gogpsproject.Navigation#release()
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void release(boolean waitForThread, long timeoutMs) throws InterruptedException
		public virtual void release(bool waitForThread, long timeoutMs)
		{

		}

		/// 
		public virtual bool parseHeader()
		{

			try
			{
				int nline = 0;
				int nsat = 0;
				while (bufferedReader.ready())
				{

					try
					{
						string line = bufferedReader.readLine();
						if (cacheStreamWriter != null)
						{
							cacheStreamWriter.write(line);
							cacheStreamWriter.write(newline);
						}
						//System.out.println(line);
						nline++;

						if (nline == 22)
						{
							return true;
						}
						switch (nline)
						{
							case 1:
								// line 1
								string typeField = line.Substring(1, 1);
								typeField = typeField.Trim();
								if (!typeField.Equals("c"))
								{
									Console.Error.WriteLine("SP3c file identifier is missing in file " + fileInputStream.ToString() + " header");
									return false;
								}
								int year = Convert.ToInt32(line.Substring(3, 4).Trim());
								int month = Convert.ToInt32(line.Substring(8, 2).Trim());
								int day = Convert.ToInt32(line.Substring(11, 2).Trim());
								int hh = Convert.ToInt32(line.Substring(14, 2).Trim());
								int mm = Convert.ToInt32(line.Substring(17, 2).Trim());
								double ss = Convert.ToDouble(line.Substring(20, 11).Trim());
								nepocs = Convert.ToInt32(line.Substring(32, 7).Trim());
								string dataUsed = line.Substring(40, 5).Trim();
								coordSys = line.Substring(46, 5).Trim();
								orbitType = line.Substring(52, 3).Trim();
								agency = line.Substring(56, 4).Trim();
								break;
							case 2:
								gpsWeek = Convert.ToInt32(line.Substring(3, 4).Trim());
								secondsOfWeek = Convert.ToInt32(line.Substring(8, 6).Trim());
								epochInterval = Convert.ToInt32(line.Substring(24, 5).Trim()) * 1000; // transform to ms
								long modJulDayStart = Convert.ToInt64(line.Substring(39, 5).Trim());
								double factionalDay = Convert.ToDouble(line.Substring(45, 15).Trim());
								break;
							case 3:
								numSats = Convert.ToInt32(line.Substring(4, 2).Trim());
								satIDs = new List<string>(numSats);
								accuracy = new Dictionary<string, long?>(numSats);
								goto case 4;
							case 4:
						case 5:
					case 6:
				case 7:
								for (int c = 0;c < 17;c++)
								{
									string sat = StringHelperClass.SubstringSpecial(line, 9 + c * 3, 12 + c * 3).Trim();
									if (!sat.Equals("0"))
									{
										sat = sat.Replace(' ', '0');
										satIDs.Add(sat);
									}
								}
								break;
							case 8:
						case 9:
					case 10:
				case 11:
			case 12:
								for (int c = 0;c < 17;c++)
								{
									string acc = StringHelperClass.SubstringSpecial(line, 9 + c * 3, 12 + c * 3).Trim();
									string sat = nsat < satIDs.Count?satIDs[nsat]:null;
									if (sat != null)
									{
										if (!acc.Equals("0"))
										{
											accuracy[sat] = new long?((long)Math.Pow(2, Convert.ToInt32(acc)));
										}
										else
										{
											accuracy[sat] = null; // unknown
										}
									}
									nsat++;
								}
								break;
							case 13:
								string fileType = line.Substring(3, 2).Trim();
								string timeSystem = line.Substring(9, 3).Trim();
								break;
							case 15:
								posStDevBase = Convert.ToDouble(line.Substring(3, 10).Trim());
								clockStDevBase = Convert.ToDouble(line.Substring(14, 12).Trim());
								break;
						}

					}
					catch (StringIndexOutOfBoundsException)
					{
						// Skip over blank lines
					}
				}
			}
			catch (IOException e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			return false;
		}

		public virtual void parseData()
		{

			try
			{
				DateTime c = new DateTime();
				c.TimeZone = TimeZone.getTimeZone("GMT");

				epocs = new List<Dictionary<string, SatellitePosition>>();
				epocTimestamps = new List<Time>();

				Dictionary<string, SatellitePosition> epoc = null;
				Time ts = null;

				while (bufferedReader.ready())
				{

					try
					{
						string line = bufferedReader.readLine();
						if (cacheStreamWriter != null)
						{
							cacheStreamWriter.write(line);
							cacheStreamWriter.write(newline);
						}
						//System.out.println(line);
						if (line == null || line.ToUpper().StartsWith("EOF", StringComparison.Ordinal))
						{
							return;
						}
						if (line[0] == '*')
						{
							int year = Convert.ToInt32(line.Substring(3, 4).Trim());
							int month = Convert.ToInt32(line.Substring(8, 2).Trim());
							int day = Convert.ToInt32(line.Substring(11, 2).Trim());
							int hh = Convert.ToInt32(line.Substring(14, 2).Trim());
							int mm = Convert.ToInt32(line.Substring(17, 2).Trim());
							double ss = Convert.ToDouble(line.Substring(20, 11).Trim());


							c.set(DateTime.YEAR, year);
							c.set(DateTime.MONTH, month - 1);
							c.set(DateTime.DAY_OF_MONTH, day);
							c.set(DateTime.HOUR_OF_DAY, hh);
							c.set(DateTime.MINUTE, mm);
							c.set(DateTime.SECOND, (int)ss);
							int ms = (int)((ss - ((int)ss)) * 1000.0);
							c.set(DateTime.MILLISECOND, ms);

							epoc = new Dictionary<string, SatellitePosition>(numSats);
							ts = new Time(c.TimeInMillis);

							epocs.Add(epoc);
							epocTimestamps.Add(ts);

						}
						else
						{
						if (epoc != null && line[0] == 'P')
						{
							string satid = line.Substring(1, 3).Trim();
							satid = satid.Replace(' ', '0');
							double x = Convert.ToDouble(line.Substring(4, 14).Trim()) * 1000.0; // transform to meter
							double y = Convert.ToDouble(line.Substring(18, 14).Trim()) * 1000.0; // transform to meter
							double z = Convert.ToDouble(line.Substring(32, 14).Trim()) * 1000.0; // transform to meter
							double clock = Convert.ToDouble(line.Substring(46, 14).Trim()) / 1000000.0; // transform to seconds

							int xStDev = -1;
							int yStDev = -1;
							int zStDev = -1;
							int clkStDev = -1;
							try
							{
								xStDev = (int)Math.Pow(posStDevBase, (double)Convert.ToInt32(line.Substring(61, 2).Trim()));
							}
							catch (NumberFormatException)
							{
							}
							try
							{
								yStDev = (int)Math.Pow(posStDevBase, (double)Convert.ToInt32(line.Substring(64, 2).Trim()));
							}
							catch (NumberFormatException)
							{
							}
							try
							{
								zStDev = (int)Math.Pow(posStDevBase, (double)Convert.ToInt32(line.Substring(67, 2).Trim()));
							}
							catch (NumberFormatException)
							{
							}
							try
							{
								clkStDev = (int)Math.Pow(clockStDevBase, (double)Convert.ToInt32(line.Substring(70, 3).Trim()));
							}
							catch (NumberFormatException)
							{
							}
							bool clockEventFlag = line.Length > 74 && line[74] == 'E';
							bool clockPredFlag = line.Length > 75 && line[75] == 'P';
							bool maneuverFlag = line.Length > 78 && line[78] == 'M';
							bool orbitPredFlag = line.Length > 79 && line[79] == 'P';


							SatellitePosition sp = new SatellitePosition(ts.Msec, Convert.ToInt32(satid.Substring(1).Trim()),'G', x, y, z);
							sp.SatelliteClockError = clock;
							sp.Predicted = orbitPredFlag || clockPredFlag;
							sp.Maneuver = maneuverFlag;
							// TODO map all the values
							epoc[satid] = sp;

							//System.out.println(""+satid+" "+(new Date(sp.getTime())));
						}
				 else if (epoc != null && line[0] == 'V')
				 {
					string satid = line.Substring(1, 3).Trim();
					satid = satid.Replace(' ', '0');
					SatellitePosition sp = epoc[satid];
					double xdot = Convert.ToDouble(line.Substring(4, 14).Trim()) * 1000.0; // transform to meter
					double ydot = Convert.ToDouble(line.Substring(18, 14).Trim()) * 1000.0; // transform to meter
					double zdot = Convert.ToDouble(line.Substring(32, 14).Trim()) * 1000.0; // transform to meter

					// The clock rate-of-change units are 10**-4 microseconds/second.
					double clockRate = Convert.ToDouble(line.Substring(46, 14).Trim()) / 1000000.0; // transform to seconds
					sp.setSpeed(xdot, ydot, zdot);
				 }
						}


					}
					catch (StringIndexOutOfBoundsException)
					{
						// Skip over blank lines
					}
				}
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
			long unixTime = obs.RefTime.Msec;
			double obsPseudorange = obs.getSatByIDType(satID, satType).getPseudorange(0);
			if (isTimestampInEpocsRange(unixTime))
			{
				for (int i = 0;i < epocTimestamps.Count;i++)
				{
					if (epocTimestamps[i].Msec <= unixTime && unixTime < epocTimestamps[i].Msec + epochInterval)
					{
						SatellitePosition sp = (SatellitePosition) epocs[i]["G" + (satID < 10?"0":"") + satID].clone();
						double tGPS = getClockCorrection(unixTime, sp.SatelliteClockError, obsPseudorange);

						return sp;
					}
				}
			}
			return null;
		}

		/// <param name="eph"> </param>
		/// <returns> Clock-corrected GPS time </returns>
		private double getClockCorrection(long unixTime, double timeCorrection, double obsPseudorange)
		{

			double gpsTime = (new Time(unixTime)).GpsTime;
			// Remove signal travel time from observation time
			double tRaw = (gpsTime - obsPseudorange / Constants.SPEED_OF_LIGHT); //this.range

			double tGPS = tRaw - timeCorrection;

			return tGPS;

		}

		/// <param name="traveltime"> </param>
		public virtual void earthRotationCorrection(Coordinates approxPos, Coordinates satelitePosition)
		{

			// Computation of signal travel time
			//SimpleMatrix diff = this.coord.ecef.minus(approxPos.ecef);
			SimpleMatrix diff = satelitePosition.minusXYZ(approxPos); //this.coord.minusXYZ(approxPos);
			double rho2 = Math.Pow(diff.get(0), 2) + Math.Pow(diff.get(1), 2) + Math.Pow(diff.get(2), 2);
			double traveltime = Math.Sqrt(rho2) / Constants.SPEED_OF_LIGHT;

			// Compute rotation angle
			double omegatau = Constants.EARTH_ANGULAR_VELOCITY * traveltime;

			// Rotation matrix
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] data = new double[3][3];
			double[][] data = RectangularArrays.ReturnRectangularDoubleArray(3, 3);
			data[0][0] = Math.Cos(omegatau);
			data[0][1] = Math.Sin(omegatau);
			data[0][2] = 0;
			data[1][0] = -Math.Sin(omegatau);
			data[1][1] = Math.Cos(omegatau);
			data[1][2] = 0;
			data[2][0] = 0;
			data[2][1] = 0;
			data[2][2] = 1;
			SimpleMatrix R = new SimpleMatrix(data);

			// Apply rotation
			//this.coord.ecef = R.mult(this.coord.ecef);
			//this.coord.setSMMultXYZ(R);// = R.mult(this.coord.ecef);
			satelitePosition.SMMultXYZ = R; // = R.mult(this.coord.ecef);

		}

		public virtual bool isTimestampInEpocsRange(long time)
		{
			return epocTimestamps.Count > 0 && epocTimestamps[0].Msec <= time && time < epocTimestamps[epocTimestamps.Count - 1].Msec + epochInterval;
		}

		/* (non-Javadoc)
		 * @see org.gogpsproject.NavigationProducer#getIono(int)
		 */
		public virtual IonoGps getIono(long unixTime)
		{
			return null;
		}



		/// <returns> the gpsWeek </returns>
		public virtual int GpsWeek
		{
			get
			{
				return gpsWeek;
			}
		}


		/// <returns> the secondsOfWeek </returns>
		public virtual int SecondsOfWeek
		{
			get
			{
				return secondsOfWeek;
			}
		}


		/// <returns> the epochInterval </returns>
		public virtual int EpochInterval
		{
			get
			{
				return epochInterval;
			}
		}


		/// <returns> the nepocs </returns>
		public virtual int NumEpocs
		{
			get
			{
				return nepocs;
			}
		}


		/// <returns> the numSats </returns>
		public virtual int NumSats
		{
			get
			{
				return numSats;
			}
		}


		/// <returns> the coordSys </returns>
		public virtual string CoordSys
		{
			get
			{
				return coordSys;
			}
		}


		/// <returns> the orbitType </returns>
		public virtual string OrbitType
		{
			get
			{
				return orbitType;
			}
		}


		/// <returns> the agency </returns>
		public virtual string Agency
		{
			get
			{
				return agency;
			}
		}


		/// <returns> the accuracy </returns>
		public virtual Dictionary<string, long?> Accuracy
		{
			get
			{
				return accuracy;
			}
		}


		/// <returns> the posStDevBase </returns>
		public virtual double PosStDevBase
		{
			get
			{
				return posStDevBase;
			}
		}


		/// <returns> the clockStDevBase </returns>
		public virtual double ClockStDevBase
		{
			get
			{
				return clockStDevBase;
			}
		}
	}

}