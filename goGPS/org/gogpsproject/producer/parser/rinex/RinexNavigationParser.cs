using System;
using System.Collections.Generic;

/*
 * Copyright (c) 2010, Eugenio Realini, Mirko Reguzzoni, Cryms sagl, Daisuke Yoshida (Osaka City Univ.). All Rights Reserved.
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
 *
 */
namespace org.gogpsproject.producer.parser.rinex
{

	using SimpleMatrix = org.ejml.simple.SimpleMatrix;
	using EphGps = org.gogpsproject.ephemeris.EphGps;
	using EphemerisSystem = org.gogpsproject.ephemeris.EphemerisSystem;
	using Coordinates = org.gogpsproject.positioning.Coordinates;
	using SatellitePosition = org.gogpsproject.positioning.SatellitePosition;
	using Time = org.gogpsproject.positioning.Time;

	/// <summary>
	/// <para>
	/// Class for parsing RINEX navigation files
	/// </para>
	/// 
	/// @author Eugenio Realini, Cryms.com
	/// </summary>
	public class RinexNavigationParser : EphemerisSystem, NavigationProducer
	{

		private File fileNav;
		private FileInputStream streamNav;
		private InputStreamReader inStreamNav;
		private BufferedReader buffStreamNav;

		private FileOutputStream cacheOutputStream;
		private OutputStreamWriter cacheStreamWriter;

		public static string newline = System.getProperty("line.separator");

		private List<EphGps> eph = new List<EphGps>(); // GPS broadcast ephemerides
		//private double[] iono = new double[8]; /* Ionosphere model parameters */
		private IonoGps iono = null; // Ionosphere model parameters
		//	private double A0; /* Delta-UTC parameters: A0 */
		//	private double A1; /* Delta-UTC parameters: A1 */
		//	private double T; /* Delta-UTC parameters: T */
		//	private double W; /* Delta-UTC parameters: W */
		//	private int leaps; /* Leap seconds */


		// RINEX Read constructors
		public RinexNavigationParser(File fileNav)
		{
			this.fileNav = fileNav;
		}

		// RINEX Read constructors
		public RinexNavigationParser(InputStream @is, File cache)
		{
			this.inStreamNav = new InputStreamReader(@is);
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

		/* (non-Javadoc)
		 * @see org.gogpsproject.Navigation#init()
		 */
		public virtual void init()
		{
			open();
			int ver = parseHeaderNav();
			if (ver != 0)
			{

				if (ver == 2)
				{

					//				System.out.println("Ver. 2.x");
					parseDataNavV2();

				}
				else if (ver == 212)
				{

					//				System.out.println("Ver. 2.12");
					parseDataNavV2();

				}
				else if (ver == 3)
				{

					//				System.out.println("Ver. 3.01");
					parseDataNavV3();

				}
			close();
			}
			else
			{
			  close();
		  throw new Exception(fileNav.ToString() + " is invalid ");
			}
		}



		/* (non-Javadoc)
		 * @see org.gogpsproject.Navigation#release()
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void release(boolean waitForThread, long timeoutMs) throws InterruptedException
		public virtual void release(bool waitForThread, long timeoutMs)
		{

		}

		/// 
		public virtual void open()
		{
			try
			{

				if (fileNav != null)
				{
					streamNav = new FileInputStream(fileNav);
				}
				if (streamNav != null)
				{
					inStreamNav = new InputStreamReader(streamNav);
				}
				if (inStreamNav != null)
				{
					buffStreamNav = new BufferedReader(inStreamNav);
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

				if (buffStreamNav != null)
				{
					buffStreamNav.close();
				}
				if (inStreamNav != null)
				{
					inStreamNav.close();
				}
				if (streamNav != null)
				{
					streamNav.close();
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

		/// 
		public virtual int parseHeaderNav()
		{

			//Navigation.iono = new double[8];
			string sub;
			int ver = 0;

			try
			{

				while (buffStreamNav.ready())
				{

					try
					{
						string line = buffStreamNav.readLine();
						if (cacheStreamWriter != null)
						{
							cacheStreamWriter.write(line);
							cacheStreamWriter.write(newline);
						}

						string typeField = line.Substring(60, line.Length - 60);
						typeField = typeField.Trim();

						if (typeField.Equals("RINEX VERSION / TYPE"))
						{

							if (!line.Substring(20, 1).Equals("N"))
							{

								// Error if navigation file identifier was not found
								Console.Error.WriteLine("Navigation file identifier is missing in file " + fileNav.ToString() + " header");
								return ver = 0;

							}
							else if (line.Substring(5, 2).Equals("3."))
							{

								//							System.out.println("Ver. 3.01");
								ver = 3;

							}
							else if (line.Substring(5, 4).Equals("2.12"))
							{

								//							System.out.println("Ver. 2.12");
								ver = 212;

							}
							else
							{

								//							System.out.println("Ver. 2.x");
								ver = 2;
							}

						}

						switch (ver)
						{
						/* RINEX ver. 2.x */
						case 2:

							if (typeField.Equals("ION ALPHA"))
							{

								float[] a = new float[4];
								sub = line.Substring(3, 11).Replace('D', 'e');
								//Navigation.iono[0] = Double.parseDouble(sub.trim());
								a[0] = Convert.ToSingle(sub.Trim());

								sub = line.Substring(15, 11).Replace('D', 'e');
								//Navigation.iono[1] = Double.parseDouble(sub.trim());
								a[1] = Convert.ToSingle(sub.Trim());

								sub = line.Substring(27, 11).Replace('D', 'e');
								//Navigation.iono[2] = Double.parseDouble(sub.trim());
								a[2] = Convert.ToSingle(sub.Trim());

								sub = line.Substring(39, 11).Replace('D', 'e');
								//Navigation.iono[3] = Double.parseDouble(sub.trim());
								a[3] = Convert.ToSingle(sub.Trim());

								if (iono == null)
								{
									iono = new IonoGps();
								}
								iono.Alpha = a;

							}
							else if (typeField.Equals("ION BETA"))
							{

								float[] b = new float[4];

								sub = line.Substring(3, 11).Replace('D', 'e');
								//Navigation.iono[4] = Double.parseDouble(sub.trim());
								//setIono(4, Double.parseDouble(sub.trim()));
								b[0] = Convert.ToSingle(sub.Trim());


								sub = line.Substring(15, 11).Replace('D', 'e');
								//Navigation.iono[5] = Double.parseDouble(sub.trim());
								//setIono(5, Double.parseDouble(sub.trim()));
								b[1] = Convert.ToSingle(sub.Trim());

								sub = line.Substring(27, 11).Replace('D', 'e');
								//Navigation.iono[6] = Double.parseDouble(sub.trim());
								//setIono(6, Double.parseDouble(sub.trim()));
								b[2] = Convert.ToSingle(sub.Trim());

								sub = line.Substring(39, 11).Replace('D', 'e');
								//Navigation.iono[7] = Double.parseDouble(sub.trim());
								//setIono(7, Double.parseDouble(sub.trim()));
								b[3] = Convert.ToSingle(sub.Trim());

								if (iono == null)
								{
									iono = new IonoGps();
								}
								iono.Beta = b;

							}
							else if (typeField.Equals("DELTA-UTC: A0,A1,T,W"))
							{

								if (iono == null)
								{
									iono = new IonoGps();
								}

								sub = line.Substring(3, 19).Replace('D', 'e');
								//setA0(Double.parseDouble(sub.trim()));
								iono.UtcA0 = Convert.ToDouble(sub.Trim());

								sub = line.Substring(22, 19).Replace('D', 'e');
								//setA1(Double.parseDouble(sub.trim()));
								iono.UtcA1 = Convert.ToDouble(sub.Trim());

								sub = line.Substring(41, 9).Replace('D', 'e');
								//setT(Integer.parseInt(sub.trim()));
								// TODO need check
								iono.UtcWNT = Convert.ToInt32(sub.Trim());

								sub = line.Substring(50, 9).Replace('D', 'e');
								//setW(Integer.parseInt(sub.trim()));
								// TODO need check
								iono.UtcTOW = Convert.ToInt32(sub.Trim());

							}
							else if (typeField.Equals("LEAP SECONDS"))
							{
								if (iono == null)
								{
									iono = new IonoGps();
								}
								sub = line.Substring(0, 6).Trim().Replace('D', 'e');
								//setLeaps(Integer.parseInt(sub.trim()));
								// TODO need check
								iono.UtcLS = Convert.ToInt32(sub.Trim());

							}
							else if (typeField.Equals("END OF HEADER"))
							{
								return ver;
							}
							break;


							/* RINEX ver. 2.12 */
						case 212:

							//							System.out.println("Ver. 2.12");

							string typeField2 = line.Substring(0, 4);
							typeField2 = typeField2.Trim();

							if (typeField2.Equals("GPSA"))
							{

								float[] a = new float[4];
								sub = line.Substring(6, 11).Replace('D', 'e');
								//Navigation.iono[0] = Double.parseDouble(sub.trim());
								a[0] = Convert.ToSingle(sub.Trim());

								sub = line.Substring(18, 11).Replace('D', 'e');
								//Navigation.iono[1] = Double.parseDouble(sub.trim());
								a[1] = Convert.ToSingle(sub.Trim());

								sub = line.Substring(30, 11).Replace('D', 'e');
								//Navigation.iono[2] = Double.parseDouble(sub.trim());
								a[2] = Convert.ToSingle(sub.Trim());

								sub = line.Substring(42, 11).Replace('D', 'e');
								//Navigation.iono[3] = Double.parseDouble(sub.trim());
								a[3] = Convert.ToSingle(sub.Trim());

								if (iono == null)
								{
									iono = new IonoGps();
								}
								iono.Alpha = a;

							}
							else if (typeField2.Equals("GPSB"))
							{

								float[] b = new float[4];

								sub = line.Substring(6, 11).Replace('D', 'e');
								//Navigation.iono[4] = Double.parseDouble(sub.trim());
								//setIono(4, Double.parseDouble(sub.trim()));
								b[0] = Convert.ToSingle(sub.Trim());

								sub = line.Substring(18, 11).Replace('D', 'e');
								//Navigation.iono[5] = Double.parseDouble(sub.trim());
								//setIono(5, Double.parseDouble(sub.trim()));
								b[1] = Convert.ToSingle(sub.Trim());

								sub = line.Substring(30, 11).Replace('D', 'e');
								//Navigation.iono[6] = Double.parseDouble(sub.trim());
								//setIono(6, Double.parseDouble(sub.trim()));
								b[2] = Convert.ToSingle(sub.Trim());

								sub = line.Substring(42, 11).Replace('D', 'e');
								//Navigation.iono[7] = Double.parseDouble(sub.trim());
								//setIono(7, Double.parseDouble(sub.trim()));
								b[3] = Convert.ToSingle(sub.Trim());

								if (iono == null)
								{
									iono = new IonoGps();
								}
								iono.Beta = b;

							}
							else if (typeField.Equals("END OF HEADER"))
							{
								return ver;
							}
							break;

							/* RINEX ver. 3.01 */
						case 3:

							string typeField3 = line.Substring(0, 4);
							typeField3 = typeField3.Trim();

							//						String typeField3 = line.substring(60, line.length());
							//						typeField3 = typeField3.trim();

							//						System.out.println(typeField2);


							if (typeField3.Equals("GPSA"))
							{

								//							System.out.println("GPSA");

								float[] a = new float[4];
								sub = line.Substring(7, 10).Replace('D', 'e');
								//Navigation.iono[0] = Double.parseDouble(sub.trim());
								a[0] = Convert.ToSingle(sub.Trim());

								sub = line.Substring(18, 11).Replace('D', 'e');
								//Navigation.iono[1] = Double.parseDouble(sub.trim());
								a[1] = Convert.ToSingle(sub.Trim());

								sub = line.Substring(30, 11).Replace('D', 'e');
								//Navigation.iono[2] = Double.parseDouble(sub.trim());
								a[2] = Convert.ToSingle(sub.Trim());

								sub = line.Substring(42, 11).Replace('D', 'e');
								//Navigation.iono[3] = Double.parseDouble(sub.trim());
								a[3] = Convert.ToSingle(sub.Trim());

								if (iono == null)
								{
									iono = new IonoGps();
								}
								iono.Alpha = a;
								//							
								//							System.out.println(a[0]);
								//							System.out.println(a[1]);
								//							System.out.println(a[2]);
								//							System.out.println(a[3]);


							}
							else if (typeField3.Equals("GPSB"))
							{

								//							System.out.println("GPSB");

								float[] b = new float[4];

								sub = line.Substring(7, 10).Replace('D', 'e');
								//Navigation.iono[4] = Double.parseDouble(sub.trim());
								//setIono(4, Double.parseDouble(sub.trim()));
								b[0] = Convert.ToSingle(sub.Trim());


								sub = line.Substring(18, 11).Replace('D', 'e');
								//Navigation.iono[5] = Double.parseDouble(sub.trim());
								//setIono(5, Double.parseDouble(sub.trim()));
								b[1] = Convert.ToSingle(sub.Trim());

								sub = line.Substring(30, 11).Replace('D', 'e');
								//Navigation.iono[6] = Double.parseDouble(sub.trim());
								//setIono(6, Double.parseDouble(sub.trim()));
								b[2] = Convert.ToSingle(sub.Trim());

								sub = line.Substring(42, 11).Replace('D', 'e');
								//Navigation.iono[7] = Double.parseDouble(sub.trim());
								//setIono(7, Double.parseDouble(sub.trim()));
								b[3] = Convert.ToSingle(sub.Trim());

								if (iono == null)
								{
									iono = new IonoGps();
								}
								iono.Beta = b;

								//							System.out.println(b[0]);
								//							System.out.println(b[1]);
								//							System.out.println(b[2]);
								//							System.out.println(b[3]);				


							}
							else if (typeField.Equals("END OF HEADER"))
							{
								//							System.out.println("END OF HEADER");

								return ver;
							}


							break;
						} // End of Switch

					}
					catch (StringIndexOutOfBoundsException)
					{
						// Skip over blank lines
					}
				}

				// Display an error if END OF HEADER was not reached
				Console.Error.WriteLine("END OF HEADER was not found in file " + fileNav.ToString());

			}
			catch (IOException e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			return 0;
		}

		/// <summary>
		/// Read all navigation data
		/// </summary>
		public virtual void parseDataNavV2()
		{
			try
			{

				// Resizable array
				//Navigation.eph = new ArrayList<EphGps>();

				//			int j = 0;

				EphGps eph = null;

				while (buffStreamNav.ready())
				{

					string sub;
					char satType = 'G';

					eph = new EphGps();
					addEph(eph);
					eph.SatType = satType;

					// read 8 lines
					for (int i = 0; i < 8; i++)
					{

						string line = buffStreamNav.readLine();
						if (cacheStreamWriter != null)
						{
							cacheStreamWriter.write(line);
							cacheStreamWriter.write(newline);
						}

						try
						{

							int len = line.Length;

							if (len != 0)
							{

								if (i == 0) // LINE 1
								{

									//Navigation.eph.get(j).refTime = new Time();


									//Navigation.eph.add(eph);
									//								addEph(eph);

									// Get satellite ID
									sub = line.Substring(0, 2).Trim();
									eph.SatID = Convert.ToInt32(sub);

									// Get and format date and time string
									string dT = line.Substring(2, 20);
									dT = dT.Replace("  ", " 0").Trim();
									dT = "20" + dT;
									//								System.out.println(dT);


									try
									{
										//Time timeEph = new Time(dT);
										// Convert String to UNIX standard time in
										// milliseconds
										//timeEph.msec = Time.dateStringToTime(dT);
										Time toc = new Time(dT);
										eph.RefTime = toc;
										eph.Toc = toc.GpsWeekSec;

										// sets Iono reference time
										if (iono != null && iono.RefTime == null)
										{
											iono.RefTime = new Time(dT);
										}

									}
									catch (ParseException)
									{
										Console.Error.WriteLine("Time parsing failed");
									}

									sub = line.Substring(22, 19).Replace('D', 'e');
									eph.Af0 = Convert.ToDouble(sub.Trim());

									sub = line.Substring(41, 19).Replace('D', 'e');
									eph.Af1 = Convert.ToDouble(sub.Trim());

									sub = line.Substring(60, len - 60).Replace('D', 'e');
									eph.Af2 = Convert.ToDouble(sub.Trim());

								} // LINE 2
								else if (i == 1)
								{

									sub = line.Substring(3, 19).Replace('D', 'e');
									double iode = Convert.ToDouble(sub.Trim());
									// TODO check double -> int conversion ?
									eph.Iode = (int) iode;

									sub = line.Substring(22, 19).Replace('D', 'e');
									eph.Crs = Convert.ToDouble(sub.Trim());

									sub = line.Substring(41, 19).Replace('D', 'e');
									eph.DeltaN = Convert.ToDouble(sub.Trim());

									sub = line.Substring(60, len - 60).Replace('D', 'e');
									eph.M0 = Convert.ToDouble(sub.Trim());

								} // LINE 3
								else if (i == 2)
								{

									sub = line.Substring(0, 22).Replace('D', 'e');
									eph.Cuc = Convert.ToDouble(sub.Trim());

									sub = line.Substring(22, 19).Replace('D', 'e');
									eph.E = Convert.ToDouble(sub.Trim());

									sub = line.Substring(41, 19).Replace('D', 'e');
									eph.Cus = Convert.ToDouble(sub.Trim());

									sub = line.Substring(60, len - 60).Replace('D', 'e');
									eph.RootA = Convert.ToDouble(sub.Trim());

								} // LINE 4
								else if (i == 3)
								{

									sub = line.Substring(0, 22).Replace('D', 'e');
									eph.Toe = Convert.ToDouble(sub.Trim());

									sub = line.Substring(22, 19).Replace('D', 'e');
									eph.Cic = Convert.ToDouble(sub.Trim());

									sub = line.Substring(41, 19).Replace('D', 'e');
									eph.Omega0 = Convert.ToDouble(sub.Trim());

									sub = line.Substring(60, len - 60).Replace('D', 'e');
									eph.Cis = Convert.ToDouble(sub.Trim());

								} // LINE 5
								else if (i == 4)
								{

									sub = line.Substring(0, 22).Replace('D', 'e');
									eph.I0 = Convert.ToDouble(sub.Trim());

									sub = line.Substring(22, 19).Replace('D', 'e');
									eph.Crc = Convert.ToDouble(sub.Trim());

									sub = line.Substring(41, 19).Replace('D', 'e');
									eph.Omega = Convert.ToDouble(sub.Trim());

									sub = line.Substring(60, len - 60).Replace('D', 'e');
									eph.OmegaDot = Convert.ToDouble(sub.Trim());

								} // LINE 6
								else if (i == 5)
								{

									sub = line.Substring(0, 22).Replace('D', 'e');
									eph.setiDot(Convert.ToDouble(sub.Trim()));

									sub = line.Substring(22, 19).Replace('D', 'e');
									double L2Code = Convert.ToDouble(sub.Trim());
									eph.L2Code = (int) L2Code;

									sub = line.Substring(41, 19).Replace('D', 'e');
									double week = Convert.ToDouble(sub.Trim());
									eph.Week = (int) week;

									sub = line.Substring(60, len - 60).Replace('D', 'e');
									double L2Flag = Convert.ToDouble(sub.Trim());
									eph.L2Flag = (int) L2Flag;

								} // LINE 7
								else if (i == 6)
								{

									sub = line.Substring(0, 22).Replace('D', 'e');
									double svAccur = Convert.ToDouble(sub.Trim());
									eph.SvAccur = (int) svAccur;

									sub = line.Substring(22, 19).Replace('D', 'e');
									double svHealth = Convert.ToDouble(sub.Trim());
									eph.SvHealth = (int) svHealth;

									sub = line.Substring(41, 19).Replace('D', 'e');
									eph.Tgd = Convert.ToDouble(sub.Trim());

									sub = line.Substring(60, len - 60).Replace('D', 'e');
									double iodc = Convert.ToDouble(sub.Trim());
									eph.Iodc = (int) iodc;

								} // LINE 8
								else if (i == 7)
								{

									sub = line.Substring(0, 22).Replace('D', 'e');
									eph.Tom = Convert.ToDouble(sub.Trim());

									if (len > 22)
									{
										sub = line.Substring(22, 19).Replace('D', 'e');
										eph.FitInt = (long) Convert.ToDouble(sub.Trim());

									}
									else
									{
										eph.FitInt = 0;
									}
								}
							}
							else
							{
								i--;
							}
						}
						catch (System.NullReferenceException)
						{
							// Skip over blank lines
						}
					}

					// Increment array index
					//				j++;
					// Store the number of ephemerides
					//Navigation.n = j;
				}

			}
			catch (IOException e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			catch (System.NullReferenceException e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
		}


		public virtual void parseDataNavV3()
		{

			try
			{

				// Resizable array
				//Navigation.eph = new ArrayList<EphGps>();

				//			int j = 0;

				EphGps eph = null;

				while (buffStreamNav.ready())
				{

					string sub;
					char satType;

					satType = (char)buffStreamNav.read();
					if (cacheStreamWriter != null)
					{
						cacheStreamWriter.write(satType);
					}
					//				System.out.println(s);

					if (satType != 'R' && satType != 'S') // other than GLONASS and SBAS data
					{
						//						System.out.println(satType);

						// read 8 lines
						for (int i = 0; i < 8; i++)
						{

							string line = buffStreamNav.readLine();
							if (cacheStreamWriter != null)
							{
								cacheStreamWriter.write(line);
								cacheStreamWriter.write(newline);
							}

							try
							{
								int len = line.Length;

								if (len != 0)
								{
									if (i == 0) // LINE 1
									{

										//Navigation.eph.get(j).refTime = new Time();

										eph = new EphGps();
										//Navigation.eph.add(eph);
										addEph(eph);

										eph.SatType = satType;

										// Get satellite ID
										sub = line.Substring(0, 2).Trim();
										//										System.out.println(sub);
										eph.SatID = Convert.ToInt32(sub);

										// Get and format date and time string
										string dT = line.Substring(3, 19);
										//								dT = dT.replace("  ", " 0").trim();
										dT = dT + ".0";
										//										System.out.println(dT);

										try
										{
											//Time timeEph = new Time(dT);
											// Convert String to UNIX standard time in
											// milliseconds
											//timeEph.msec = Time.dateStringToTime(dT);
											Time toc = new Time(dT);
											eph.RefTime = toc;
											eph.Toc = toc.GpsWeekSec;

											// sets Iono reference time
											if (iono != null && iono.RefTime == null)
											{
												iono.RefTime = new Time(dT);
											}

										}
										catch (ParseException)
										{
											Console.Error.WriteLine("Time parsing failed");
										}

										sub = line.Substring(22, 19).Replace('D', 'e');
										eph.Af0 = Convert.ToDouble(sub.Trim());

										sub = line.Substring(41, 19).Replace('D', 'e');
										eph.Af1 = Convert.ToDouble(sub.Trim());

										sub = line.Substring(60, len - 60).Replace('D', 'e');
										eph.Af2 = Convert.ToDouble(sub.Trim());

									} // LINE 2
									else if (i == 1)
									{

										sub = line.Substring(4, 19).Replace('D', 'e');
										double iode = Convert.ToDouble(sub.Trim());
										// TODO check double -> int conversion ?
										eph.Iode = (int) iode;

										sub = line.Substring(23, 19).Replace('D', 'e');
										eph.Crs = Convert.ToDouble(sub.Trim());

										sub = line.Substring(42, 19).Replace('D', 'e');
										eph.DeltaN = Convert.ToDouble(sub.Trim());

										sub = line.Substring(61, len - 61).Replace('D', 'e');
										eph.M0 = Convert.ToDouble(sub.Trim());

									} // LINE 3
									else if (i == 2)
									{

										sub = line.Substring(4, 19).Replace('D', 'e');
										eph.Cuc = Convert.ToDouble(sub.Trim());

										sub = line.Substring(23, 19).Replace('D', 'e');
										eph.E = Convert.ToDouble(sub.Trim());

										sub = line.Substring(42, 19).Replace('D', 'e');
										eph.Cus = Convert.ToDouble(sub.Trim());

										sub = line.Substring(61, len - 61).Replace('D', 'e');
										eph.RootA = Convert.ToDouble(sub.Trim());

									} // LINE 4
									else if (i == 3)
									{

										sub = line.Substring(4, 19).Replace('D', 'e');
										eph.Toe = Convert.ToDouble(sub.Trim());

										sub = line.Substring(23, 19).Replace('D', 'e');
										eph.Cic = Convert.ToDouble(sub.Trim());

										sub = line.Substring(42, 19).Replace('D', 'e');
										eph.Omega0 = Convert.ToDouble(sub.Trim());

										sub = line.Substring(61, len - 61).Replace('D', 'e');
										eph.Cis = Convert.ToDouble(sub.Trim());

									} // LINE 5
									else if (i == 4)
									{

										sub = line.Substring(4, 19).Replace('D', 'e');
										eph.I0 = Convert.ToDouble(sub.Trim());

										sub = line.Substring(23, 19).Replace('D', 'e');
										eph.Crc = Convert.ToDouble(sub.Trim());

										sub = line.Substring(42, 19).Replace('D', 'e');
										eph.Omega = Convert.ToDouble(sub.Trim());

										sub = line.Substring(61, len - 61).Replace('D', 'e');
										eph.OmegaDot = Convert.ToDouble(sub.Trim());

									} // LINE 6
									else if (i == 5)
									{

										sub = line.Substring(4, 19).Replace('D', 'e');
										eph.setiDot(Convert.ToDouble(sub.Trim()));

										sub = line.Substring(23, 19).Replace('D', 'e');
										double L2Code = Convert.ToDouble(sub.Trim());
										eph.L2Code = (int) L2Code;

										sub = line.Substring(42, 19).Replace('D', 'e');
										double week = Convert.ToDouble(sub.Trim());
										eph.Week = (int) week;

										sub = line.Substring(61, len - 61).Replace('D', 'e');
										if (sub.Trim().Length > 0)
										{
											double L2Flag = Convert.ToDouble(sub.Trim());
											eph.L2Flag = (int) L2Flag;
										}
										else
										{
											eph.L2Flag = 0;
										}

									} // LINE 7
									else if (i == 6)
									{

										sub = line.Substring(4, 19).Replace('D', 'e');
										double svAccur = Convert.ToDouble(sub.Trim());
										eph.SvAccur = (int) svAccur;

										sub = line.Substring(23, 19).Replace('D', 'e');
										double svHealth = Convert.ToDouble(sub.Trim());
										eph.SvHealth = (int) svHealth;

										sub = line.Substring(42, 19).Replace('D', 'e');
										eph.Tgd = Convert.ToDouble(sub.Trim());

										sub = line.Substring(61, len - 61).Replace('D', 'e');
										double iodc = Convert.ToDouble(sub.Trim());
										eph.Iodc = (int) iodc;

									} // LINE 8
									else if (i == 7)
									{

										sub = line.Substring(4, 19).Replace('D', 'e');
										eph.Tom = Convert.ToDouble(sub.Trim());

										if (line.Trim().Length > 22)
										{
											sub = line.Substring(23, 19).Replace('D', 'e');
											eph.FitInt = Convert.ToInt64(sub.Trim());

										}
										else
										{
											eph.FitInt = 0;
										}
									}
								}
								else
								{
									i--;
								}


							}
							catch (System.NullReferenceException)
							{
								// Skip over blank lines
							}



						} // End of for


					} // In case of GLONASS data
					else if (satType == 'R')
					{
						//						System.out.println("satType: " + satType);

						for (int i = 0; i < 4; i++)
						{
							string line = buffStreamNav.readLine();
							if (cacheStreamWriter != null)
							{
								cacheStreamWriter.write(line);
								cacheStreamWriter.write(newline);
							}

							try
							{
								int len = line.Length;

								if (len != 0)
								{
									if (i == 0) // LINE 1
									{

										//Navigation.eph.get(j).refTime = new Time();

										eph = new EphGps();
										addEph(eph);

										eph.SatType = satType;

										// Get satellite ID
										sub = line.Substring(0, 2).Trim();
										//										System.out.println("ID: "+sub);
										eph.SatID = Convert.ToInt32(sub);

										// Get and format date and time string
										string dT = line.Substring(3, 19);
										//								dT = dT.replace("  ", " 0").trim();
										dT = dT + ".0";

										//										System.out.println("dT: " + dT);

										try
										{
											//Time timeEph = new Time(dT);
											// Convert String to UNIX standard time in
											// milliseconds
											//timeEph.msec = Time.dateStringToTime(dT);


											Time dtoc = new Time(dT);
											eph.RefTime = dtoc;
											int toc = dtoc.GpsWeekSec;
											//												System.out.println("toc: " + toc);																		
											eph.Toc = toc;

											int week = dtoc.GpsWeek;
											//												System.out.println("week: " + week);																		
											eph.Week = week;

											double toe = toc;
											//												System.out.printf("%.3f\n", gTime);
											//												System.out.println("timeEph: " + toe);
											eph.Toe = toe;

											// sets Iono reference time
											if (iono != null && iono.RefTime == null)
											{
												iono.RefTime = new Time(dT);
											}

										}
										catch (ParseException)
										{
											Console.Error.WriteLine("Time parsing failed");
										}

										/* TauN */ 
										sub = line.Substring(22, 19).Replace('D', 'e');
										//										System.out.println(sub);
										eph.TauN = Convert.ToSingle(sub.Trim());

										/* GammaN */
										sub = line.Substring(41, 19).Replace('D', 'e');
										//										System.out.println(sub);
										eph.GammaN = Convert.ToSingle(sub.Trim());

										/* tb */
										sub = line.Substring(60, len - 60).Replace('D', 'e');
										//										System.out.println("tb: " + sub);

										/* tb is a time interval within the current day (UTC + 3 hours)*/
										double tb = Convert.ToDouble(sub.Trim());
										double tk = tb - 10800;
										//										System.out.println("tk: " + tk);
										eph.settk(tk);


										//										eph.settb(Double.parseDouble(sub.trim()));									

									} // LINE 2
									else if (i == 1)
									{

										/* X: satellite X coordinate at ephemeris reference time [m] */
										sub = line.Substring(4, 19).Replace('D', 'e');
										//										System.out.println(sub);
										eph.X = Convert.ToDouble(sub.Trim()) * 1e3;

										/* Xv: satellite velocity along X at ephemeris reference time [m/s] */
										sub = line.Substring(23, 19).Replace('D', 'e');
										//										System.out.println(sub);
										eph.Xv = Convert.ToDouble(sub.Trim()) * 1e3;

										/* Xa: acceleration due to lunar-solar gravitational perturbation along X at ephemeris reference time [m/s^2] */
										sub = line.Substring(42, 19).Replace('D', 'e');
										//										System.out.println(sub);
										eph.Xa = Convert.ToDouble(sub.Trim()) * 1e3;

										/* Bn */
										sub = line.Substring(61, len - 61).Replace('D', 'e');
										//										System.out.println(sub);
										eph.Bn = Convert.ToDouble(sub.Trim());

									} // LINE 3
									else if (i == 2)
									{

										/* Y: satellite Y coordinate at ephemeris reference time [m] */
										sub = line.Substring(4, 19).Replace('D', 'e');
										//										System.out.println(sub);
										eph.Y = Convert.ToDouble(sub.Trim()) * 1e3;

										/* Yv: satellite velocity along Y at ephemeris reference time [m/s] */
										sub = line.Substring(23, 19).Replace('D', 'e');
										//										System.out.println(sub);
										eph.Yv = Convert.ToDouble(sub.Trim()) * 1e3;

										/* Ya: acceleration due to lunar-solar gravitational perturbation along Y at ephemeris reference time [m/s^2] */
										sub = line.Substring(42, 19).Replace('D', 'e');
										//										System.out.println(sub);
										eph.Ya = Convert.ToDouble(sub.Trim()) * 1e3;

										/* freq_num */
										sub = line.Substring(61, len - 61).Replace('D', 'e');
										//										System.out.println(sub);
										eph.setfreq_num((int) Convert.ToDouble(sub.Trim()));

									} // LINE 4
									else if (i == 3)
									{

										/* Z: satellite Z coordinate at ephemeris reference time [m] */
										sub = line.Substring(4, 19).Replace('D', 'e');
										//										System.out.println(sub);
										eph.Z = Convert.ToDouble(sub.Trim()) * 1e3;

										/* Zv: satellite velocity along Z at ephemeris reference time [m/s] */
										sub = line.Substring(23, 19).Replace('D', 'e');
										//										System.out.println(sub);
										eph.Zv = Convert.ToDouble(sub.Trim()) * 1e3;

										/* Za: acceleration due to lunar-solar gravitational perturbation along Z at ephemeris reference time [m/s^2]  */
										sub = line.Substring(42, 19).Replace('D', 'e');
										//										System.out.println(sub);
										eph.Za = Convert.ToDouble(sub.Trim()) * 1e3;

										/* En */
										sub = line.Substring(61, len - 61).Replace('D', 'e');
										//										System.out.println(sub);
										//										eph.setEn(Long.parseLong(sub.trim()));
										eph.En = Convert.ToDouble(sub.Trim());


									} // End of if

								}
								else
								{
									i--;
								}
								//		}  // End of if


							}
							catch (System.NullReferenceException)
							{
								// Skip over blank lines
							}

						} // End of for

					} //SBAS data
					else
					{

						for (int i = 0; i < 4; i++)
						{
							string line = buffStreamNav.readLine();
							if (cacheStreamWriter != null)
							{
								cacheStreamWriter.write(line);
								cacheStreamWriter.write(newline);
							}
						}

					} // End of GLO if


					// Increment array index
					//				j++;
					// Store the number of ephemerides
					//Navigation.n = j;
				} // End of while

			}
			catch (IOException e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			catch (System.NullReferenceException e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}


		}

		private double gpsToUnixTime(Time toc, int tow)
		{
			// TODO Auto-generated method stub
			return 0;
		}

		/// <param name="unixTime"> </param>
		/// <param name="satID"> </param>
		/// <returns> Reference ephemeris set for given time and satellite </returns>
		public virtual EphGps findEph(long unixTime, int satID, char satType)
		{

			long dt = 0;
			long dtMin = 0;
			long dtMax = 0;
			long delta = 0;
			EphGps refEph = null;

			//long gpsTime = (new Time(unixTime)).getGpsTime();

			for (int i = 0; i < eph.Count; i++)
			{
				// Find ephemeris sets for given satellite
				if (eph[i].SatID == satID && eph[i].SatType == satType)
				{
					// Consider BeiDou time (BDT) for BeiDou satellites (14 sec difference wrt GPS time)
					if (satType == 'C')
					{
						delta = 14000;
						unixTime = unixTime - delta;
					}
					// Compare current time and ephemeris reference time
					dt = Math.Abs(eph[i].RefTime.Msec - unixTime) / 1000; //getGpsTime() - gpsTime
					// If it's the first round, set the minimum time difference and
					// select the first ephemeris set candidate; if the current ephemeris set
					// is closer in time than the previous candidate, select new candidate
					if (refEph == null || dt < dtMin)
					{
						dtMin = dt;
						refEph = eph[i];
					}
				}
			}

		if (refEph == null)
		{
		  return null;
		}

			if (refEph.SvHealth != 0)
			{
			  return EphGps.UnhealthyEph;
			}

			//maximum allowed interval from ephemeris reference time
			long fitInterval = refEph.FitInt;

			if (fitInterval != 0)
			{
				dtMax = fitInterval * 3600 / 2;
			}
			else
			{
				switch (refEph.SatType)
				{
				  case 'R':
					  dtMax = 950;
					  goto case 'J';
				  case 'J':
					  dtMax = 3600;
					  goto default;
				  default:
					  dtMax = 7200;
				  break;
				}
			}
			if (dtMin > dtMax)
			{
				refEph = null;
			}

			return refEph;
		}

		public virtual int EphSize
		{
			get
			{
				return eph.Count;
			}
		}

		public virtual void addEph(EphGps eph)
		{
			this.eph.Add(eph);
		}

		//	public void setIono(int i, double val){
		//		this.iono[i] = val;
		//	}
		public virtual IonoGps getIono(long unixTime)
		{
			return iono;
		}
		//	/**
		//	 * @return the a0
		//	 */
		//	public double getA0() {
		//		return A0;
		//	}
		//	/**
		//	 * @param a0 the a0 to set
		//	 */
		//	public void setA0(double a0) {
		//		A0 = a0;
		//	}
		//	/**
		//	 * @return the a1
		//	 */
		//	public double getA1() {
		//		return A1;
		//	}
		//	/**
		//	 * @param a1 the a1 to set
		//	 */
		//	public void setA1(double a1) {
		//		A1 = a1;
		//	}
		//	/**
		//	 * @return the t
		//	 */
		//	public double getT() {
		//		return T;
		//	}
		//	/**
		//	 * @param t the t to set
		//	 */
		//	public void setT(double t) {
		//		T = t;
		//	}
		//	/**
		//	 * @return the w
		//	 */
		//	public double getW() {
		//		return W;
		//	}
		//	/**
		//	 * @param w the w to set
		//	 */
		//	public void setW(double w) {
		//		W = w;
		//	}
		//	/**
		//	 * @return the leaps
		//	 */
		//	public int getLeaps() {
		//		return leaps;
		//	}
		//	/**
		//	 * @param leaps the leaps to set
		//	 */
		//	public void setLeaps(int leaps) {
		//		this.leaps = leaps;
		//	}

		public virtual bool isTimestampInEpocsRange(long unixTime)
		{
			return eph.Count > 0 && eph[0].RefTime.Msec <= unixTime; /*&&
			unixTime <= eph.get(eph.size()-1).getRefTime().getMsec() missing interval +epochInterval*/
		}


		/* (non-Javadoc)
		 * @see org.gogpsproject.NavigationProducer#getGpsSatPosition(long, int, double)
		 */
		public virtual SatellitePosition getGpsSatPosition(Observations obs, int satID, char satType, double receiverClockError)
		{
			long unixTime = obs.RefTime.Msec;
			double range = obs.getSatByIDType(satID, satType).getPseudorange(0);

			if (range == 0)
			{
				return null;
			}

			EphGps eph = findEph(unixTime, satID, satType);
			if (eph.Equals(EphGps.UnhealthyEph))
			{
				return SatellitePosition.UnhealthySat;
			}

			if (eph != null)
			{

				//			char satType = eph.getSatType();

				SatellitePosition sp = computePositionGps(obs, satID, satType, eph, receiverClockError);
				//			SatellitePosition sp = computePositionGps(unixTime, satType, satID, eph, range, receiverClockError);
				//if(receiverPosition!=null) earthRotationCorrection(receiverPosition, sp);
				return sp; // new SatellitePosition(eph, unixTime, satID, range);
			}
			return null;
		}

	  public virtual string FileName
	  {
		  get
		  {
			if (fileNav == null)
			{
			  return null;
			}
			else
			{
			  return fileNav.Name;
			}
		  }
	  }
	}

}