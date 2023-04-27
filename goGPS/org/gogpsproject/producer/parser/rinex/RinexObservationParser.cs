using System;

/*
 * Copyright (c) 2010, Eugenio Realini, Mirko Reguzzoni, Cryms sagl - Switzerland, Daisuke Yoshida. All Rights Reserved. All Rights Reserved.
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

	using EphGps = org.gogpsproject.ephemeris.EphGps;
	using Coordinates = org.gogpsproject.positioning.Coordinates;
	using Time = org.gogpsproject.positioning.Time;

	/// <summary>
	/// <para>
	/// Class for parsing RINEX observation files
	/// </para>
	/// 
	/// @author Eugenio Realini, Cryms.com
	/// </summary>
	public class RinexObservationParser : ObservationsProducer
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			multiConstellation = new Nullable[] {gpsEnable, qzsEnable, gloEnable, galEnable, bdsEnable};
		}


		private File fileObs;
		private FileInputStream streamObs;
		private InputStreamReader inStreamObs;
		private BufferedReader buffStreamObs;

		private int nTypes, nTypesG, nTypesR, nTypesE, nTypesJ, nTypesC; // Number of observation types
		private int[] typeOrder, typeOrderG, typeOrderR, typeOrderE, typeOrderJ, typeOrderC; // Order of observation data
		private bool hasS1Field = false; // S1 field (SNR) is present
		private bool hasS2Field = false; // S2 field (SNR) is present
		private Time timeFirstObs; // Time of first observation set

		private Coordinates approxPos; // Approximate position (X, Y, Z) [m]
		private double[] antDelta; // Antenna delta (E, N, U) [m]

		private Observations obs = null; // Current observation data sets

		// Private fields useful to keep track of values between epoch parsing and
		// data parsing
		private int nGps;
		private int nGlo;
		private int nQzs;
		private int nSbs;
		private int nSat;
		private char[] sysOrder;
		private int[] satOrder;
		private int ver;

		internal bool gpsEnable = true; // enable GPS data reading
		internal bool qzsEnable = true; // enable QZSS data reading
		internal bool gloEnable = true; // enable GLONASS data reading
		internal bool galEnable = true; // enable Galileo data reading
		internal bool bdsEnable = true; // enable BeiDou data reading

		internal bool?[] multiConstellation;

	//	String line;

		public RinexObservationParser(File fileObs)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
			this.fileObs = fileObs;
		}

		public RinexObservationParser(File fileObs, bool?[] multiConstellation)
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
			this.fileObs = fileObs;
			this.gpsEnable = multiConstellation[0];
			this.qzsEnable = multiConstellation[1];
			this.gloEnable = multiConstellation[2];
			this.galEnable = multiConstellation[3];
			this.bdsEnable = multiConstellation[4];
			this.multiConstellation = multiConstellation;
		}

		/// 
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void open() throws java.io.FileNotFoundException
		public virtual void open()
		{
			streamObs = new FileInputStream(fileObs);
			inStreamObs = new InputStreamReader(streamObs);
			buffStreamObs = new BufferedReader(inStreamObs);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void release(boolean waitForThread, long timeoutMs) throws InterruptedException
		public virtual void release(bool waitForThread, long timeoutMs)
		{
			try
			{
				streamObs.close();
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
				inStreamObs.close();
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
				buffStreamObs.close();
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
		public virtual int parseHeaderObs()
		{
			try
			{
				bool foundTypeObs = false;

				while (buffStreamObs.ready())
				{
					string line = buffStreamObs.readLine();
					string typeField = line.Substring(60, line.Length - 60);
					typeField = typeField.Trim();

					if (typeField.Equals("RINEX VERSION / TYPE"))
					{

							if (!line.Substring(20, 1).Equals("O"))
							{

								// Error if observation file identifier was not found
								Console.Error.WriteLine("Observation file identifier is missing in file " + fileObs.ToString() + " header");
								return ver = 0;

							}
							else if (line.Substring(5, 2).Equals("3."))
							{
								ver = 3;
							}
							else if (line.Substring(5, 4).Equals("2.12"))
							{
								ver = 212;
							}
							else
							{
								ver = 2;
							}
					}

					switch (ver)
					{
					/* In case of RINEX ver. 2.11 */
					case 2:
							if (typeField.Equals("# / TYPES OF OBSERV"))
							{
								parseTypesV2(line);
								foundTypeObs = true;
							}
							else if (typeField.Equals("TIME OF FIRST OBS"))
							{
								parseTimeFirstObs(line);
							}
							else if (typeField.Equals("APPROX POSITION XYZ"))
							{
								parseApproxPos(line);
							}
							else if (typeField.Equals("ANTENNA: DELTA H/E/N"))
							{
								parseAntDelta(line);
							}
							else if (typeField.Equals("END OF HEADER"))
							{
								if (!foundTypeObs)
								{
									// Display an error if TIME OF FIRST OBS was not found
									Console.Error.WriteLine("Critical information" + "(TYPES OF OBSERV) is missing in file " + fileObs.ToString() + " header");
								}
								return ver;
							}
					break;

					/* In case of RINEX ver. 2.12 */
					case 212:
	//						System.out.println("RINEX version : 2.12");
							if (typeField.Equals("# / TYPES OF OBSERV"))
							{
								parseTypesV212(line);
								foundTypeObs = true;
							}
							else if (typeField.Equals("TIME OF FIRST OBS"))
							{
								parseTimeFirstObs(line);
							}
							else if (typeField.Equals("APPROX POSITION XYZ"))
							{
								parseApproxPos(line);
							}
							else if (typeField.Equals("ANTENNA: DELTA H/E/N"))
							{
								parseAntDelta(line);
							}
							else if (typeField.Equals("END OF HEADER"))
							{
								if (!foundTypeObs)
								{
									// Display an error if TIME OF FIRST OBS was not found
									Console.Error.WriteLine("Critical information" + "(TYPES OF OBSERV) is missing in file " + fileObs.ToString() + " header");
								}
								return ver;
							}
					break;

					/* In case of RINEX ver. 3 */
					case 3:
							if (typeField.Equals("SYS / # / OBS TYPES"))
							{

								string satType = line.Substring(0,1);
	//							System.out.println("sys: " + sys);

	//							if(satType.equals("G")){
									parseTypesV3(line, satType);
									foundTypeObs = true;
	//							}

							}
							else if (typeField.Equals("TIME OF FIRST OBS"))
							{
								parseTimeFirstObsV3(line);
							}
							else if (typeField.Equals("APPROX POSITION XYZ"))
							{
								parseApproxPos(line);
							}
							else if (typeField.Equals("ANTENNA: DELTA H/E/N"))
							{
								parseAntDelta(line);
							}
							else if (typeField.Equals("END OF HEADER"))
							{
								if (!foundTypeObs)
								{
									// Display an error if TIME OF FIRST OBS was not found
									Console.Error.WriteLine("Critical information" + "(TYPES OF OBSERV) is missing in file " + fileObs.ToString() + " header");
								}
								return ver;
							}
					break;


					} // End of switch

				} // End of while


				// Display an error if END OF HEADER was not reached
				Console.Error.WriteLine("END OF HEADER was not found in file " + fileObs.ToString());

			}
			catch (IOException e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			catch (StringIndexOutOfBoundsException e)
			{
				// Skip over blank lines
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}

			return 0;

		}

		/// <summary>
		/// Parse one observation epoch single/double line
		/// </summary>
		public virtual Observations NextObservations
		{
			get
			{
    
    
    
				try
				{
    
					/* In case of RINEX ver. 2.11 */
					if (ver == 2)
					{
    
							if (!hasMoreObservations())
							{
								return null;
							}
							string line = buffStreamObs.readLine();
							int len = line.Length;
    
							// Parse date and time
							string dateStr = "20" + line.Substring(1, 21);
    
							// Parse event flag
							string eFlag = line.Substring(28, 2).Trim();
							int eventFlag = Convert.ToInt32(eFlag);
    
							// Parse available satellites string
							string satAvail = line.Substring(30, len - 30);
    
							// Parse number of available satellites
							string numOfSat = satAvail.Substring(0, 2).Trim();
							nSat = Convert.ToInt32(numOfSat);
    
							// Arrays to store satellite order
							satOrder = new int[nSat];
							sysOrder = new char[nSat];
    
							nGps = 0;
							nGlo = 0;
							nSbs = 0;
    
							// If number of satellites <= 12, read only one line...
							if (nSat <= 12)
							{
    
								// Parse satellite IDs
								int j = 2;
								for (int i = 0; i < nSat; i++)
								{
    
									string satType = satAvail.Substring(j, 1);
									string satID = StringHelperClass.SubstringSpecial(satAvail, j + 1, j + 3);
									if (satType.Equals("G") || satType.Equals(" "))
									{
										sysOrder[i] = 'G';
										satOrder[i] = Convert.ToInt32(satID.Trim());
										nGps++;
									}
									else if (satType.Equals("R"))
									{
										sysOrder[i] = 'R';
										satOrder[i] = Convert.ToInt32(satID.Trim());
										nGlo++;
									}
									else if (satType.Equals("S"))
									{
										sysOrder[i] = 'S';
										satOrder[i] = Convert.ToInt32(satID.Trim());
										nSbs++;
									}
									j = j + 3;
								}
							} // ... otherwise, read two lines
							else
							{
    
								// Parse satellite IDs
								int j = 2;
								for (int i = 0; i < 12; i++)
								{
    
									string satType = satAvail.Substring(j, 1);
									string satID = StringHelperClass.SubstringSpecial(satAvail, j + 1, j + 3);
									if (satType.Equals("G") || satType.Equals(" "))
									{
										sysOrder[i] = 'G';
										satOrder[i] = Convert.ToInt32(satID.Trim());
										nGps++;
									}
									else if (satType.Equals("R"))
									{
										sysOrder[i] = 'R';
										satOrder[i] = Convert.ToInt32(satID.Trim());
										nGlo++;
									}
									else if (satType.Equals("S"))
									{
										sysOrder[i] = 'S';
										satOrder[i] = Convert.ToInt32(satID.Trim());
										nSbs++;
									}
									j = j + 3;
								}
								// Get second line
								satAvail = buffStreamObs.readLine().Trim();
    
								// Number of remaining satellites
								int num = nSat - 12;
    
								// Parse satellite IDs
								int k = 0;
								for (int i = 0; i < num; i++)
								{
    
									string satType = satAvail.Substring(k, 1);
									string satID = StringHelperClass.SubstringSpecial(satAvail, k + 1, k + 3);
									if (satType.Equals("G") || satType.Equals(" "))
									{
										sysOrder[i + 12] = 'G';
										satOrder[i + 12] = Convert.ToInt32(satID.Trim());
										nGps++;
									}
									else if (satType.Equals("R"))
									{
										sysOrder[i + 12] = 'R';
										satOrder[i + 12] = Convert.ToInt32(satID.Trim());
										nGlo++;
									}
									else if (satType.Equals("S"))
									{
										sysOrder[i + 12] = 'S';
										satOrder[i + 12] = Convert.ToInt32(satID.Trim());
										nSbs++;
									}
									k = k + 3;
								}
							}
    
							obs = new Observations(new Time(dateStr), eventFlag);
    
							// Convert date string to standard UNIX time in milliseconds
							//long time = Time.dateStringToTime(dateStr);
    
							// Store time
							//obs.refTime = new Time(dateStr);
							//obs.refTime.msec = time;
    
							// Store event flag
							//obs.eventFlag = eventFlag;
    
							parseDataObsV2();
    
							obs.cleanObservations();
    
							return obs;
    
    
					 /* In case of RINEX ver. 2.12 */
					}
					else if (ver == 212)
					{
    
						if (!hasMoreObservations())
						{
							return null;
						}
						string line = buffStreamObs.readLine();
						int len = line.Length;
    
						// Parse date and time
						string dateStr = "20" + line.Substring(1, 21);
    
						// Parse event flag
						string eFlag = line.Substring(28, 2).Trim();
						int eventFlag = Convert.ToInt32(eFlag);
    
						// Parse available satellites string
						string satAvail = line.Substring(30, len - 30);
    
						// Parse number of available satellites
						string numOfSat = satAvail.Substring(0, 2).Trim();
						nSat = Convert.ToInt32(numOfSat);
    
						// Arrays to store satellite order
						satOrder = new int[nSat];
						sysOrder = new char[nSat];
    
						nGps = 0;
						nGlo = 0;
						nSbs = 0;
						nQzs = 0;
    
						// If number of satellites <= 12, read only one line...
						if (nSat <= 12)
						{
    
							// Parse satellite IDs
							int j = 2;
							for (int i = 0; i < nSat; i++)
							{
    
								string satType = satAvail.Substring(j, 1);
								string satID = StringHelperClass.SubstringSpecial(satAvail, j + 1, j + 3);
								if (satType.Equals("G") || satType.Equals(" "))
								{
									sysOrder[i] = 'G';
									satOrder[i] = Convert.ToInt32(satID.Trim());
									nGps++;
								}
								else if (satType.Equals("R"))
								{
									sysOrder[i] = 'R';
									satOrder[i] = Convert.ToInt32(satID.Trim());
									nGlo++;
								}
								else if (satType.Equals("S"))
								{
									sysOrder[i] = 'S';
									satOrder[i] = Convert.ToInt32(satID.Trim());
									nSbs++;
								}
								else if (satType.Equals("J"))
								{
									sysOrder[i] = 'J';
									satOrder[i] = Convert.ToInt32(satID.Trim());
									nQzs++;
								}
    
								j = j + 3;
							}
						} // ... otherwise, read two lines
						else
						{
    
							// Parse satellite IDs
							int j = 2;
							for (int i = 0; i < 12; i++)
							{
    
								string satType = satAvail.Substring(j, 1);
								string satID = StringHelperClass.SubstringSpecial(satAvail, j + 1, j + 3);
								if (satType.Equals("G") || satType.Equals(" "))
								{
									sysOrder[i] = 'G';
									satOrder[i] = Convert.ToInt32(satID.Trim());
									nGps++;
								}
								else if (satType.Equals("R"))
								{
									sysOrder[i] = 'R';
									satOrder[i] = Convert.ToInt32(satID.Trim());
									nGlo++;
								}
								else if (satType.Equals("S"))
								{
									sysOrder[i] = 'S';
									satOrder[i] = Convert.ToInt32(satID.Trim());
									nSbs++;
								}
								else if (satType.Equals("J"))
								{
									sysOrder[i] = 'J';
									satOrder[i] = Convert.ToInt32(satID.Trim());
									nQzs++;
								}
								j = j + 3;
							}
							// Get second line
							satAvail = buffStreamObs.readLine().Trim();
    
							// Number of remaining satellites
							int num = nSat - 12;
    
							// Parse satellite IDs
							int k = 0;
							for (int i = 0; i < num; i++)
							{
    
								string satType = satAvail.Substring(k, 1);
								string satID = StringHelperClass.SubstringSpecial(satAvail, k + 1, k + 3);
								if (satType.Equals("G") || satType.Equals(" "))
								{
									sysOrder[i + 12] = 'G';
									satOrder[i + 12] = Convert.ToInt32(satID.Trim());
									nGps++;
								}
								else if (satType.Equals("R"))
								{
									sysOrder[i + 12] = 'R';
									satOrder[i + 12] = Convert.ToInt32(satID.Trim());
									nGlo++;
								}
								else if (satType.Equals("S"))
								{
									sysOrder[i + 12] = 'S';
									satOrder[i + 12] = Convert.ToInt32(satID.Trim());
									nSbs++;
								}
								else if (satType.Equals("J"))
								{
									sysOrder[i + 12] = 'J';
									satOrder[i + 12] = Convert.ToInt32(satID.Trim());
									nQzs++;
								}
								k = k + 3;
							}
						}
    
						obs = new Observations(new Time(dateStr), eventFlag);
    
						// Convert date string to standard UNIX time in milliseconds
						//long time = Time.dateStringToTime(dateStr);
    
						// Store time
						//obs.refTime = new Time(dateStr);
						//obs.refTime.msec = time;
    
						// Store event flag
						//obs.eventFlag = eventFlag;
    
						parseDataObsV2();
    
						obs.cleanObservations();
    
						return obs;
    
					/* In case of RINEX ver. 3 */
					}
					else
					{
    
						if (!hasMoreObservations())
						{
							return null;
						}
						string line = buffStreamObs.readLine();
    
						// Parse date and time
						string dateStr = line.Substring(2, 23);
    
						// Parse event flag
						string eFlag = line.Substring(30, 2).Trim();
    
						int eventFlag = Convert.ToInt32(eFlag);
    
						// Parse available satellites string
						string satAvail = line.Substring(33, 2).Trim();
    
						// Parse number of available satellites
		//				String numOfSat = satAvail.substring(0, 2).trim();
						nSat = Convert.ToInt32(satAvail);
    
						// Arrays to store satellite order
		//				satOrder = new int[nSat];
		//				sysOrder = new char[nSat];
    
						nGps = 0;
						nGlo = 0;
						nSbs = 0;
						nQzs = 0;
    
						obs = new Observations(new Time(dateStr), eventFlag);
    
						// Convert date string to standard UNIX time in milliseconds
						//long time = Time.dateStringToTime(dateStr);
    
						// Store time
						//obs.refTime = new Time(dateStr);
						//obs.refTime.msec = time;
    
						// Store event flag
						//obs.eventFlag = eventFlag;
    
						parseDataObsV3();
    
						obs.cleanObservations();
    
						return obs;
    
    
					} // End of if
    
    
    
				}
				catch (ParseException e)
				{
					// Skip over unexpected observation lines
					Console.WriteLine(e.ToString());
					Console.Write(e.StackTrace);
				}
				catch (StringIndexOutOfBoundsException e)
				{
					// Skip over blank lines
					Console.WriteLine(e.ToString());
					Console.Write(e.StackTrace);
				}
				catch (IOException e)
				{
					Console.WriteLine(e.ToString());
					Console.Write(e.StackTrace);
				}
				return null;
			}
		}

		/// <summary>
		/// Parse one observation epoch
		/// </summary>
		private void parseDataObsV2()
		{

			try
			{

				//obs.init(nGps, nGlo, nSbs);

				// Arrays to store satellite list for each system
	//			obs.gpsSat = new ArrayList<Integer>(nGps);
	//			obs.gloSat = new ArrayList<Integer>(nGlo);
	//			obs.sbsSat = new ArrayList<Integer>(nSbs);
	//
	//			// Allocate array of observation objects
	//			if (nGps > 0)
	//				obs.gps = new ObservationSet[nGps];
	//			if (nGlo > 0)
	//				obs.glo = new ObservationSet[nGlo];
	//			if (nSbs > 0)
	//				obs.sbs = new ObservationSet[nSbs];

				// Loop through observation lines
				for (int i = 0; i < nSat; i++)
				{

					// Read line of observations
					string line = buffStreamObs.readLine();

					float nLinesToRead0 = (float) nTypes / 5;
					decimal bd0 = new decimal(nLinesToRead0);
					decimal bd = bd0.setScale(0, decimal.ROUND_UP);
					int nLinesToRead = (double)(int) bd;

					if (sysOrder[i] == 'G' && gpsEnable)
					{

						// Create observation object
						ObservationSet os = new ObservationSet();
						os.SatType = 'G';
						os.SatID = satOrder[i];
						obs.setGps(i, os);
	//					obs.gps[i] = os;// new ObservationSet();
	//					obs.gps[i].C = 0;
	//					obs.gps[i].P = new double[2];
	//					obs.gps[i].L = new double[2];
	//					obs.gps[i].S = new float[2];
	//					obs.gps[i].D = new float[2];

						// Store satellite ID
						//obs.gps[i].setSatID(satOrder[i]);
	//					obs.gpsSat.add(satOrder[i]);

						if (nLinesToRead == 1)
						{

							// Parse observation data according to typeOrder
							int j = 0;
							for (int k = 0; k < nTypes; k++)
							{
								assignTypes(line, k, j, i, os.SatType);
								j = j + 16;
							}

						} // ... otherwise, they are more than one lines
						else
						{

							int k = 0;
							for (int l = 0; l < nLinesToRead; l++)
							{

								int remTypes = nTypes - 5 * l; // To calculate remaining Types

								if (remTypes > 5) // 5 types is in one line
								{
									int j = 0;
									for (int m = 0; m < 5; m++)
									{
										assignTypes(line, k, j, i, os.SatType);
										j = j + 16;
										k++;
									}
									line = buffStreamObs.readLine();

								} // the number of types in the last line
								else if (remTypes < 5 && remTypes > 0)
								{
									int j = 0;
									for (int m = 0; m < remTypes; m++)
									{
										assignTypes(line, k, j, i, os.SatType);
										j = j + 16;
										k++;
									}
								} // end of if
							} // end of for
						}
						// end of GPS

					}
				else if (sysOrder[i] == 'R' && gloEnable)
				{

					ObservationSet os = new ObservationSet();
					os.SatType = 'R';
					os.SatID = satOrder[i];
					obs.setGps(i, os);

					if (nLinesToRead == 1)
					{

						// Parse observation data according to typeOrder
						int j = 0;
						for (int k = 0; k < nTypes; k++)
						{
							assignTypes(line, k, j, i, os.SatType);
							j = j + 16;
						}

					} // ... otherwise, they are more than one lines
					else
					{

						int k = 0;
						for (int l = 0; l < nLinesToRead; l++)
						{

							int remTypes = nTypes - 5 * l; // To calculate remaining Types

							if (remTypes > 5) // 5 types is in one line
							{
								int j = 0;
								for (int m = 0; m < 5; m++)
								{
									assignTypes(line, k, j, i, os.SatType);
									j = j + 16;
									k++;
								}
								// Get next line
								line = buffStreamObs.readLine();

							} // the number of types in the last line
							else if (remTypes < 5 && remTypes > 0)
							{
								int j = 0;
								for (int m = 0; m < remTypes; m++)
								{
									assignTypes(line, k, j, i, os.SatType);
									j = j + 16;
									k++;
								}
							} // end of if
						} // end of for
					}
					// end of GLONASS

				}
				else if (sysOrder[i] == 'J' && qzsEnable)
				{

					ObservationSet os = new ObservationSet();
					os.SatType = 'J';
					os.SatID = satOrder[i];
					obs.setGps(i, os);

					if (nLinesToRead == 1)
					{

						// Parse observation data according to typeOrder
						int j = 0;
						for (int k = 0; k < nTypes; k++)
						{
							assignTypes(line, k, j, i, os.SatType);
							j = j + 16;
						}

					} // ... otherwise, they are more than one lines
					else
					{

							int k = 0;
							for (int l = 0; l < nLinesToRead; l++)
							{

								int remTypes = nTypes - 5 * l; // To calculate remaining Types

								if (remTypes > 5) // 5 types is in one line
								{
									int j = 0;
									for (int m = 0; m < 5; m++)
									{
										assignTypes(line, k, j, i, os.SatType);
										j = j + 16;
										k++;
									}
									// Get next line
									line = buffStreamObs.readLine();

								} // the number of types in the last line
								else if (remTypes < 5 && remTypes > 0)
								{
									int j = 0;
									for (int m = 0; m < remTypes; m++)
									{
										assignTypes(line, k, j, i, os.SatType);
										j = j + 16;
										k++;
									}
								} // end of if
							} // end of for
					}
					// end of GZSS 

				} // skip unselected observations
				else
				{

					if (nLinesToRead > 1) // If the number of observation
					{

						for (int l = 0; l < nLinesToRead; l++)
						{
							int remTypes = nTypes - 5 * l; // To calculate remaining Types

							if (remTypes > 5) // 5 types in one line
							{
								line = buffStreamObs.readLine();
							} // end of if
						} // end of for
					} // end of if
				} // end of if
				}


			}
			catch (StringIndexOutOfBoundsException e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
				// Skip over blank lines
			}
			catch (IOException e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
		}


		private void parseDataObsV3()
		{

			try
			{

				//obs.init(nGps, nGlo, nSbs);

				// Arrays to store satellite list for each system
	//			obs.gpsSat = new ArrayList<Integer>(nGps);
	//			obs.gloSat = new ArrayList<Integer>(nGlo);
	//			obs.sbsSat = new ArrayList<Integer>(nSbs);
	//
	//			// Allocate array of observation objects
	//			if (nGps > 0)
	//				obs.gps = new ObservationSet[nGps];
	//			if (nGlo > 0)
	//				obs.glo = new ObservationSet[nGlo];
	//			if (nSbs > 0)
	//				obs.sbs = new ObservationSet[nSbs];

				// Loop through observation lines
	//			System.out.println("nSat: " + nSat);

				for (int i = 0; i < nSat; i++)
				{

					// Read line of observations
					string line = buffStreamObs.readLine();

					string satType = line.Substring(0, 1);
					string satNum = line.Substring(1, 2);

					int satID = Convert.ToInt32(satNum.Trim());


					if (satType.Equals("G") && gpsEnable)
					{

						// Create observation object
						ObservationSet os = new ObservationSet();
						os.SatType = 'G';
						os.SatID = satID;
						obs.setGps(i, os);

						line = line.Substring(3);
						// Parse observation data according to typeOrder
						int j = 0;
						for (int k = 0; k < nTypesG; k++)
						{
							assignTypes(line, k, j, i, os.SatType);
							j = j + 16;
						}

					}
					else if (satType.Equals("R") && gloEnable)
					{

						// Create observation object
						ObservationSet os = new ObservationSet();
						os.SatType = 'R';
						os.SatID = satID;
						obs.setGps(i, os);

						line = line.Substring(3);
						// Parse observation data according to typeOrder
						int j = 0;
						for (int k = 0; k < nTypesR; k++)
						{
							assignTypes(line, k, j, i, os.SatType);
							j = j + 16;
						}

					}
					else if (satType.Equals("E") && galEnable)
					{

						// Create observation object
						ObservationSet os = new ObservationSet();
						os.SatType = 'E';
						os.SatID = satID;
						obs.setGps(i, os);

						line = line.Substring(3);
						// Parse observation data according to typeOrder
						int j = 0;
						for (int k = 0; k < nTypesE; k++)
						{
							assignTypes(line, k, j, i, os.SatType);
							j = j + 16;
						}

					}
					else if (satType.Equals("J") && qzsEnable)
					{

						// Create observation object
						ObservationSet os = new ObservationSet();
						os.SatType = 'J';
						os.SatID = satID;
						obs.setGps(i, os);

						line = line.Substring(3);
						// Parse observation data according to typeOrder
						int j = 0;
						for (int k = 0; k < nTypesJ; k++)
						{
							assignTypes(line, k, j, i, os.SatType);
							j = j + 16;
						}

					}
					else if (satType.Equals("C") && bdsEnable)
					{

						// Create observation object
						ObservationSet os = new ObservationSet();
						os.SatType = 'C';
						os.SatID = satID;
						obs.setGps(i, os);

						line = line.Substring(3);
						// Parse observation data according to typeOrder
						int j = 0;
						for (int k = 0; k < nTypesC; k++)
						{
							assignTypes(line, k, j, i, os.SatType);
							j = j + 16;
						}
					} // End of if
				} // End of for

			}
			catch (StringIndexOutOfBoundsException e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
				// Skip over blank lines
			}
			catch (IOException e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
		}

		/// <summary>
		/// Assign observation data according to type order
		/// </summary>
		private void assignTypes(string line, int k, int j, int i, char satType)
		{

			if (ver == 3)
			{
				if (satType == 'G')
				{
					typeOrder = typeOrderG;
				}
				else if (satType == 'R')
				{
					typeOrder = typeOrderR;
				}
				else if (satType == 'E')
				{
					typeOrder = typeOrderE;
				}
				else if (satType == 'J')
				{
					typeOrder = typeOrderJ;
				}
				else if (satType == 'C')
				{
					typeOrder = typeOrderC;
				}
			}

			try
			{
				ObservationSet o = obs.getSatByIdx(i);

				if (typeOrder[k] == 0) // ** C1 code
				{

					string codeC = line.Substring(j, 14).Trim();
					if (codeC.Trim().Length > 0)
					{
						o.setCodeC(0,Convert.ToDouble(codeC));
					}

				} // ** C2 code
				else if (typeOrder[k] == 1)
				{

					string codeC = line.Substring(j, 14).Trim();
					if (codeC.Trim().Length > 0)
					{
						o.setCodeC(1,Convert.ToDouble(codeC));
					}

				} // ** P1 code
				else if (typeOrder[k] == 2)
				{

					string codeP = line.Substring(j, 14).Trim();
	//				System.out.println("codeP: " + codeP);
					if (codeP.Length != 0)
					{
						o.setCodeP(0,Convert.ToDouble(codeP));
					}
				} // ** P2 code
				else if (typeOrder[k] == 3)
				{

					string codeP = line.Substring(j, 14).Trim();
					if (codeP.Length != 0)
					{
						o.setCodeP(1,Convert.ToDouble(codeP));
					}
				} // ** L1 phase
				else if (typeOrder[k] == 4)
				{

					string phaseL = line.Substring(j, 14);
	//				System.out.println("phaseL: " + phaseL);
					phaseL = phaseL.Trim();
					try
					{
						if (phaseL.Length != 0)
						{
							o.setPhaseCycles(0,Convert.ToDouble(phaseL));
							try
							{
								// Loss of Lock
								int lli = Convert.ToInt32(StringHelperClass.SubstringSpecial(line, j + 14, j + 15));
								o.setLossLockInd(0,lli);
							}
							catch (Exception)
							{
							}
							try
							{
								// Signal Strength
								int ss = Convert.ToInt32(StringHelperClass.SubstringSpecial(line, j + 15, j + 16));
								o.setSignalStrengthInd(0, ss);
								if (!hasS1Field)
								{
									o.setSignalStrength(0,ss * 6);
								}
							}
							catch (Exception)
							{
							}
						}
					}
					catch (NumberFormatException)
					{
					}
				} // ** L2 phase
				else if (typeOrder[k] == 5)
				{

					string phaseL = line.Substring(j, 14).Trim();
					try
					{
						if (phaseL.Length != 0)
						{
							o.setPhaseCycles(1,Convert.ToDouble(phaseL));

							try
							{
								// Loss of Lock
								int lli = Convert.ToInt32(StringHelperClass.SubstringSpecial(line, j + 14, j + 15));
								o.setLossLockInd(1,lli);
							}
							catch (Exception)
							{
							}
							try
							{
								// Signal Strength
								int ss = Convert.ToInt32(StringHelperClass.SubstringSpecial(line, j + 15, j + 16));
								o.setSignalStrengthInd(1, ss);
								if (!hasS2Field)
								{
									o.setSignalStrength(1,ss * 6);
								}
							}
							catch (Exception)
							{
							}
						}
					}
					catch (NumberFormatException)
					{
					}
				} // S1 ** SNR on L1
				else if (typeOrder[k] == 6)
				{

					string snrS = line.Substring(j, 14).Trim();
	//				System.out.println("snrS: " + snrS);

					if (snrS.Length != 0)
					{
						o.setSignalStrength(0,Convert.ToSingle(snrS));
					}
				} // S2 ** SNR on L2
				else if (typeOrder[k] == 7)
				{

					string snrS = line.Substring(j, 14).Trim();
					if (snrS.Length != 0)
					{
						o.setSignalStrength(1,Convert.ToSingle(snrS));
					}
				} // ** D1 doppler
				else if (typeOrder[k] == 8)
				{

					string dopplerD = line.Substring(j, 14).Trim();
	//				System.out.println("dopplerD: " + dopplerD);

					if (dopplerD.Length != 0)
					{
						o.setDoppler(0,Convert.ToSingle(dopplerD));
					}
				} // ** D2 doppler
				else if (typeOrder[k] == 9)
				{

					string dopplerD = line.Substring(j, 14).Trim();
					if (dopplerD.Length != 0)
					{
						o.setDoppler(1,Convert.ToSingle(dopplerD));
					}

				/*  NEED to improve below codes  */ 


				} // ** D2 doppler
				else if (typeOrder[k] == 10)
				{

	//				String dopplerD = line.substring(j, j + 14).trim();
	//				if (dopplerD.length() != 0) {
	//					o.setDoppler(1,Float.parseFloat(dopplerD));
	//				}

				} // ** D2 doppler
				else if (typeOrder[k] == 11)
				{

	//				String dopplerD = line.substring(j, j + 14).trim();
	//				if (dopplerD.length() != 0) {
	//					o.setDoppler(1,Float.parseFloat(dopplerD));
	//				}

				} // ** D2 doppler
				else if (typeOrder[k] == 12)
				{

	//				String dopplerD = line.substring(j, j + 14).trim();
	//				if (dopplerD.length() != 0) {
	//					o.setDoppler(1,Float.parseFloat(dopplerD));
	//				}

				} // ** D2 doppler
				else if (typeOrder[k] == 13)
				{

	//				String dopplerD = line.substring(j, j + 14).trim();
	//				if (dopplerD.length() != 0) {
	//					o.setDoppler(1,Float.parseFloat(dopplerD));
	//				}

				} // ** D2 doppler
				else if (typeOrder[k] == 14)
				{

	//				String dopplerD = line.substring(j, j + 14).trim();
	//				if (dopplerD.length() != 0) {
	//					o.setDoppler(1,Float.parseFloat(dopplerD));
	//				}

				} // ** D2 doppler
				else if (typeOrder[k] == 15)
				{

	//				String dopplerD = line.substring(j, j + 14).trim();
	//				if (dopplerD.length() != 0) {
	//					o.setDoppler(1,Float.parseFloat(dopplerD));
	//				}

				} // ** D2 doppler
				else if (typeOrder[k] == 16)
				{

	//				String dopplerD = line.substring(j, j + 14).trim();
	//				if (dopplerD.length() != 0) {
	//					o.setDoppler(1,Float.parseFloat(dopplerD));
	//				}

				} // ** D2 doppler
				else if (typeOrder[k] == 17)
				{

	//				String dopplerD = line.substring(j, j + 14).trim();
	//				if (dopplerD.length() != 0) {
	//					o.setDoppler(1,Float.parseFloat(dopplerD));
	//				}

				} // ** D2 doppler
				else if (typeOrder[k] == 18)
				{

	//				String dopplerD = line.substring(j, j + 14).trim();
	//				if (dopplerD.length() != 0) {
	//					o.setDoppler(1,Float.parseFloat(dopplerD));
	//				}

				} // ** D2 doppler
				else if (typeOrder[k] == 19)
				{

	//				String dopplerD = line.substring(j, j + 14).trim();
	//				if (dopplerD.length() != 0) {
	//					o.setDoppler(1,Float.parseFloat(dopplerD));
	//				}

				} // ** D2 doppler
				else if (typeOrder[k] == 20)
				{

	//				String dopplerD = line.substring(j, j + 14).trim();
	//				if (dopplerD.length() != 0) {
	//					o.setDoppler(1,Float.parseFloat(dopplerD));
	//				}

				} // ** D2 doppler
				else if (typeOrder[k] == 21)
				{

	//				String dopplerD = line.substring(j, j + 14).trim();
	//				if (dopplerD.length() != 0) {
	//					o.setDoppler(1,Float.parseFloat(dopplerD));
	//				}

				} // ** D2 doppler
				else if (typeOrder[k] == 22)
				{

	//				String dopplerD = line.substring(j, j + 14).trim();
	//				if (dopplerD.length() != 0) {
	//					o.setDoppler(1,Float.parseFloat(dopplerD));
	//				}

				} // ** D2 doppler
				else if (typeOrder[k] == 23)
				{

	//				String dopplerD = line.substring(j, j + 14).trim();
	//				if (dopplerD.length() != 0) {
	//					o.setDoppler(1,Float.parseFloat(dopplerD));
	//				}

				} // ** D2 doppler
				else if (typeOrder[k] == 24)
				{

	//				String dopplerD = line.substring(j, j + 14).trim();
	//				if (dopplerD.length() != 0) {
	//					o.setDoppler(1,Float.parseFloat(dopplerD));
	//				}
				}



			}
			catch (StringIndexOutOfBoundsException)
			{
				// Skip over blank slots
			}
		}

		/// <param name="line"> </param>
		private void parseTypesV2(string line)
		{

			// Extract number of available data types
			nTypes = Convert.ToInt32(line.Substring(0, 6).Trim());

			// Allocate the array that stores data type order
			typeOrder = new int[nTypes];

			/*
			 * Parse data types and store order (internal order: C1 P1 P2 L1 L2 S1
			 * S2 D1 D2)
			 */
			for (int i = 0; i < nTypes; i++)
			{
				string type = StringHelperClass.SubstringSpecial(line, 6 * (i + 2) - 2, 6 * (i + 2));
				if (type.Equals("C1"))
				{
					typeOrder[i] = 0;
				}
				else if (type.Equals("C2"))
				{
					typeOrder[i] = 1;
				}
				else if (type.Equals("P1"))
				{
					typeOrder[i] = 2;
				}
				else if (type.Equals("P2"))
				{
					typeOrder[i] = 3;
				}
				else if (type.Equals("L1"))
				{
					typeOrder[i] = 4;
				}
				else if (type.Equals("L2"))
				{
					typeOrder[i] = 5;
				}
				else if (type.Equals("S1"))
				{
					typeOrder[i] = 6;
					hasS1Field = true;
				}
				else if (type.Equals("S2"))
				{
					typeOrder[i] = 7;
					hasS2Field = true;
				}
				else if (type.Equals("D1"))
				{
					typeOrder[i] = 8;
				}
				else if (type.Equals("D2"))
				{
					typeOrder[i] = 9;
				}
			}
		}

		/// <param name="line"> </param>
		/// <param name="satType"> </param>
		/// <exception cref="IOException">  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void parseTypesV212(String line) throws java.io.IOException
		private void parseTypesV212(string line)
		{

			// Extract number of available data types
			nTypes = Convert.ToInt32(line.Substring(1, 5).Trim());

			// Allocate the array that stores data type order
			typeOrder = new int[nTypes];

			if (nTypes >= 19) // In case of more than 18 Types, it will three lines
			{

				int i = 0;
				for (int j = 0; j <= 8; j++)
				{
					string type = StringHelperClass.SubstringSpecial(line, 6 * (j + 2) - 2, 6 * (j + 2));
					checkTypeV212(type, i);
					i++;
				}

				line = buffStreamObs.readLine(); // read the second line, from type 10 - 18

				for (int j = 0; j <= 8 ; j++)
				{
					string type = StringHelperClass.SubstringSpecial(line, 6 * (j + 2) - 2, 6 * (j + 2));
					checkTypeV212(type, i);
					i++;
				}

				line = buffStreamObs.readLine(); // read the third line, from type 19 -

				for (int j = 0; j < nTypes - 18 ; j++)
				{
					string type = StringHelperClass.SubstringSpecial(line, 6 * (j + 2) - 2, 6 * (j + 2));
					checkTypeV212(type, i);
					i++;
				}

			} // In case of 10 - 18 Types, it will two lines
			else if (nTypes > 9 && nTypes < 19)
			{

				int i = 0;
				for (int j = 0; j <= 8; j++)
				{
					string type = StringHelperClass.SubstringSpecial(line, 6 * (j + 2) - 2, 6 * (j + 2));
					checkTypeV212(type, i);
					i++;
				}

				line = buffStreamObs.readLine(); // read the second line, from type 10

				for (int j = 0; j < nTypes - 9 ; j++)
				{
					string type = StringHelperClass.SubstringSpecial(line, 6 * (j + 2) - 2, 6 * (j + 2));
					checkTypeV212(type, i);
					i++;
				}

			} // less than 10 types, it will be one line.
			else
			{

				for (int i = 0; i < nTypes; i++)
				{
					string type = StringHelperClass.SubstringSpecial(line, 6 * (i + 2) - 2, 6 * (i + 2));
					checkTypeV212(type, i);
				}
			}

		}

		/// <param name="line"> </param>
		/// <param name="satType"> </param>
		/// <exception cref="IOException">  </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void parseTypesV3(String line, String satType) throws java.io.IOException
		private void parseTypesV3(string line, string satType)
		{

			// Extract number of available data types
			nTypes = Convert.ToInt32(line.Substring(1, 5).Trim());

			// Allocate the array that stores data type order
			typeOrder = new int[nTypes];

			if (nTypes > 13) // In case of more than 13 Types, it will two lines
			{

				for (int i = 0; i <= 12; i++)
				{
					string type = StringHelperClass.SubstringSpecial(line, 4 * (i + 3) - 5, 4 * (i + 3) - 2);
					checkTypeV3(type, i);
				}

				line = buffStreamObs.readLine(); // read the second line

				int j = 0;
				for (int i = 13; i <= nTypes ; i++)
				{
					string type = StringHelperClass.SubstringSpecial(line, 4 * (j + 3) - 5, 4 * (j + 3) - 2);
					checkTypeV3(type, i);
					j++;
				}

			} // less than 14 types, it will be one line.
			else
			{

				for (int i = 0; i < nTypes; i++)
				{
					string type = StringHelperClass.SubstringSpecial(line, 4 * (i + 3) - 5, 4 * (i + 3) - 2);
					checkTypeV3(type, i);
				}
			}

			if (satType.Equals("G"))
			{
				typeOrderG = typeOrder;
				nTypesG = nTypes;
			}
			else if (satType.Equals("R"))
			{
				typeOrderR = typeOrder;
				nTypesR = nTypes;
			}
			else if (satType.Equals("E"))
			{
				typeOrderE = typeOrder;
				nTypesE = nTypes;
			}
			else if (satType.Equals("J"))
			{
				typeOrderJ = typeOrder;
				nTypesJ = nTypes;
			}
			else if (satType.Equals("C"))
			{
				typeOrderC = typeOrder;
				nTypesC = nTypes;
			}
		}

		private void checkTypeV212(string type, int i)
		{
			if (type.Equals("C1") || type.Equals("CA"))
			{
				typeOrder[i] = 0;
			}
			else if (type.Equals("C2"))
			{
				typeOrder[i] = 1;
			}
			else if (type.Equals("P1"))
			{
				typeOrder[i] = 2;
			}
			else if (type.Equals("P2") || type.Equals("CC"))
			{
				typeOrder[i] = 3;
			}
			else if (type.Equals("L1") || type.Equals("LA"))
			{
				typeOrder[i] = 4;
			}
			else if (type.Equals("L2") || type.Equals("LC"))
			{
				typeOrder[i] = 5;
			}
			else if (type.Equals("S1") || type.Equals("SA"))
			{
				typeOrder[i] = 6;
				hasS1Field = true;
			}
			else if (type.Equals("S2") || type.Equals("SC"))
			{
				typeOrder[i] = 7;
				hasS2Field = true;
			}
			else if (type.Equals("D1") || type.Equals("DA"))
			{
				typeOrder[i] = 8;
			}
			else if (type.Equals("D2") || type.Equals("DC"))
			{
				typeOrder[i] = 9;
			}
		}


		private void checkTypeV3(string type, int i)
		{
			if (type.Equals("C1C"))
			{
				typeOrder[i] = 0;
			}
			else if (type.Equals("C2C"))
			{
				typeOrder[i] = 1;
			}
			else if (type.Equals("P1C"))
			{
				typeOrder[i] = 2;
			}
			else if (type.Equals("P2C"))
			{
				typeOrder[i] = 3;
			}
			else if (type.Equals("L1C"))
			{
				typeOrder[i] = 4;
			}
			else if (type.Equals("L2C"))
			{
				typeOrder[i] = 5;
			}
			else if (type.Equals("S1C"))
			{
				typeOrder[i] = 6;
				hasS1Field = true;
			}
			else if (type.Equals("S2C"))
			{
				typeOrder[i] = 7;
				hasS2Field = true;
			}
			else if (type.Equals("D1C"))
			{
				typeOrder[i] = 8;
			}
			else if (type.Equals("D2C"))
			{
				typeOrder[i] = 9;

			}
			else if (type.Equals("C1W"))
			{
				typeOrder[i] = 10;
			}
			else if (type.Equals("L1W"))
			{
				typeOrder[i] = 11;
			}
			else if (type.Equals("D1W"))
			{
				typeOrder[i] = 12;
			}
			else if (type.Equals("S1W"))
			{
				typeOrder[i] = 13;

			}
			else if (type.Equals("C2W"))
			{
				typeOrder[i] = 14;
			}
			else if (type.Equals("L2W"))
			{
				typeOrder[i] = 15;
			}
			else if (type.Equals("D2W"))
			{
				typeOrder[i] = 16;
			}
			else if (type.Equals("S2W"))
			{
				typeOrder[i] = 17;

			}
			else if (type.Equals("C1X"))
			{
				typeOrder[i] = 18;
			}
			else if (type.Equals("L1X"))
			{
				typeOrder[i] = 19;
			}
			else if (type.Equals("D1X"))
			{
				typeOrder[i] = 20;
			}
			else if (type.Equals("S1X"))
			{
				typeOrder[i] = 21;

			}
			else if (type.Equals("C2X"))
			{
				typeOrder[i] = 22;
			}
			else if (type.Equals("L2X"))
			{
				typeOrder[i] = 23;
			}
			else if (type.Equals("D2X"))
			{
				typeOrder[i] = 24;
			}
			else if (type.Equals("S2X"))
			{
				typeOrder[i] = 25;

			}
			else if (type.Equals("C5X"))
			{
				typeOrder[i] = 26;
			}
			else if (type.Equals("L5X"))
			{
				typeOrder[i] = 27;
			}
			else if (type.Equals("D5X"))
			{
				typeOrder[i] = 28;
			}
			else if (type.Equals("S5X"))
			{
				typeOrder[i] = 29;

			}
			else if (type.Equals("C6X"))
			{
				typeOrder[i] = 30;
			}
			else if (type.Equals("L6X"))
			{
				typeOrder[i] = 31;
			}
			else if (type.Equals("D6X"))
			{
				typeOrder[i] = 32;
			}
			else if (type.Equals("S6X"))
			{
				typeOrder[i] = 33;

			}
			else if (type.Equals("C7X"))
			{
				typeOrder[i] = 34;
			}
			else if (type.Equals("L7X"))
			{
				typeOrder[i] = 35;
			}
			else if (type.Equals("D7X"))
			{
				typeOrder[i] = 36;
			}
			else if (type.Equals("S7X"))
			{
				typeOrder[i] = 37;

			}
			else if (type.Equals("C8X"))
			{
				typeOrder[i] = 38;
			}
			else if (type.Equals("L8X"))
			{
				typeOrder[i] = 39;
			}
			else if (type.Equals("D8X"))
			{
				typeOrder[i] = 40;
			}
			else if (type.Equals("S8X"))
			{
				typeOrder[i] = 41;

			}
			else if (type.Equals("C1P"))
			{
				typeOrder[i] = 42;
			}
			else if (type.Equals("L1P"))
			{
				typeOrder[i] = 43;
			}
			else if (type.Equals("D1P"))
			{
				typeOrder[i] = 44;
			}
			else if (type.Equals("S1P"))
			{
				typeOrder[i] = 45;

			}
			else if (type.Equals("C2P"))
			{
				typeOrder[i] = 46;
			}
			else if (type.Equals("L2P"))
			{
				typeOrder[i] = 47;
			}
			else if (type.Equals("D2P"))
			{
				typeOrder[i] = 48;
			}
			else if (type.Equals("S2P"))
			{
				typeOrder[i] = 49;

			}
			else if (type.Equals("C2I"))
			{
				typeOrder[i] = 50;
			}
			else if (type.Equals("L2I"))
			{
				typeOrder[i] = 51;
			}
			else if (type.Equals("D2I"))
			{
				typeOrder[i] = 52;
			}
			else if (type.Equals("S2I"))
			{
				typeOrder[i] = 53;

			}
			else if (type.Equals("C6I"))
			{
				typeOrder[i] = 54;
			}
			else if (type.Equals("L6I"))
			{
				typeOrder[i] = 55;
			}
			else if (type.Equals("D6I"))
			{
				typeOrder[i] = 56;
			}
			else if (type.Equals("S6I"))
			{
				typeOrder[i] = 57;

			}
			else if (type.Equals("C7I"))
			{
				typeOrder[i] = 58;
			}
			else if (type.Equals("L7I"))
			{
				typeOrder[i] = 59;
			}
			else if (type.Equals("D7I"))
			{
				typeOrder[i] = 60;
			}
			else if (type.Equals("S7I"))
			{
				typeOrder[i] = 61;

			}
		}

		/// <param name="line"> </param>
		private void parseTimeFirstObs(string line)
		{

			// Format date string according to DateStringToTime required format
			string dateStr = line.Substring(0, 42).Trim().Replace("    ", " ").Replace("   ", " ");

			// Create time object
			//timeFirstObs = new Time();

			// Convert date string to standard UNIX time in milliseconds
			try
			{
				timeFirstObs = new Time(dateStr); //Time.dateStringToTime(dateStr);

			}
			catch (ParseException)
			{
				// Display an error if END OF HEADER was not reached
				Console.Error.WriteLine("TIME OF FIRST OBS parsing failed in file " + fileObs.ToString());
			}
		}

		/// <param name="line"> </param>
		private void parseTimeFirstObsV3(string line)
		{

			// Format date string according to DateStringToTime required format
	//		String dateStr = line.substring(0, 43).trim().replace("    ", " ") .replace("   ", " ");
			string dateStr = line.Substring(0, 43).Trim().Replace("    ", " ").Replace("   ", " ");
	//		System.out.println(dateStr);


			// Create time object
			//timeFirstObs = new Time();

			// Convert date string to standard UNIX time in milliseconds
			try
			{
				timeFirstObs = new Time(dateStr); //Time.dateStringToTime(dateStr);
	//			System.out.println("TIME OF FIRST OBS: " + timeFirstObs);


			}
			catch (ParseException)
			{
				// Display an error if END OF HEADER was not reached
				Console.Error.WriteLine("TIME OF FIRST OBS parsing failed in file " + fileObs.ToString());
			}
		}


		/// <param name="line"> </param>
		private void parseApproxPos(string line)
		{

			// Allocate the vector that stores the approximate position (X, Y, Z)
			//approxPos = Coordinates.globalXYZInstance(new SimpleMatrix(3, 1));
	//		approxPos.ecef = new SimpleMatrix(3, 1);

			// Read approximate position coordinates
	//		approxPos.ecef.set(0, 0, Double.valueOf(line.substring(0, 14).trim())
	//				.doubleValue());
	//		approxPos.ecef.set(1, 0, Double.valueOf(line.substring(14, 28).trim())
	//				.doubleValue());
	//		approxPos.ecef.set(2, 0, Double.valueOf(line.substring(28, 42).trim())
	//				.doubleValue());
	//
			approxPos = Coordinates.globalXYZInstance(Convert.ToDouble(line.Substring(0, 14).Trim()), Convert.ToDouble(line.Substring(14, 14).Trim()), Convert.ToDouble(line.Substring(28, 14).Trim()));

			// Convert the approximate position to geodetic coordinates
			approxPos.computeGeodetic();
		}

		/// <param name="line"> </param>
		private void parseAntDelta(string line)
		{

			// Allocate the array that stores the approximate position
			antDelta = new double[3];

			// Read approximate position coordinates (E, N, U)
			antDelta[2] = (double)Convert.ToDouble(line.Substring(0, 14).Trim());
			antDelta[0] = (double)Convert.ToDouble(line.Substring(14, 14).Trim());
			antDelta[1] = (double)Convert.ToDouble(line.Substring(28, 14).Trim());
		}

		/// <returns> the approxPos </returns>
		public virtual Coordinates DefinedPosition
		{
			get
			{
				return approxPos;
			}
		}

		/// <returns> the obs </returns>
		public virtual Observations CurrentObservations
		{
			get
			{
				return obs;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public boolean hasMoreObservations() throws java.io.IOException
		public virtual bool hasMoreObservations()
		{
			return buffStreamObs.ready();
		}

		/* (non-Javadoc)
		 * @see org.gogpsproject.ObservationsProducer#init()
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void init() throws Exception
		public virtual void init()
		{
			// Open file streams
			open();

			// Parse RINEX observation headers
			parseHeaderObs(); // Header

		}

	}

}