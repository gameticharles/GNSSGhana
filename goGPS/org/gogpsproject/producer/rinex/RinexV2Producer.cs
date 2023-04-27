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
namespace org.gogpsproject.producer.rinex
{


	using EphGps = org.gogpsproject.ephemeris.EphGps;
	using Coordinates = org.gogpsproject.positioning.Coordinates;
	using Time = org.gogpsproject.positioning.Time;
	using IonoGps = org.gogpsproject.producer.parser.IonoGps;

	/// <summary>
	/// @author Lorenzo
	/// 
	/// </summary>
	public class RinexV2Producer : StreamEventListener
	{

		private string outFilename;
		private bool headerWritten;

		private Coordinates approxPosition = null;

		private List<Observations> observations = new List<Observations>();

		private bool needApproxPos = false;
		private bool singleFreq = false;
		private bool standardFilename = true;

		private FileOutputStream fos = null;
		private PrintStream ps = null;

		private List<Type> typeConfig = new List<Type>();

		private SimpleDateFormat sdfHeader = new SimpleDateFormat("dd-MMM-yy HH:mm:ss");
		private DecimalFormat dfX3 = new DecimalFormat("0.000");
		private DecimalFormat dfX7 = new DecimalFormat("0.0000000");
		private DecimalFormat dfX = new DecimalFormat("0");
		private DecimalFormat dfXX = new DecimalFormat("00");
		private DecimalFormat dfX4 = new DecimalFormat("0.0000");
		private string marker;
		private int minDOY = 0;
		private int DOYold = -1;
		private string outputDir = "./test";
		private bool enableZip = false;

		private static readonly TimeZone TZ = TimeZone.getTimeZone("GMT");

		public RinexV2Producer(bool needApproxPos, bool singleFreq, string marker, int minDOY)
		{

			this.needApproxPos = needApproxPos;
			this.singleFreq = singleFreq;
			this.marker = marker;
			this.minDOY = minDOY;

			// set observation type config
			typeConfig.Add(new Type(Type.C,1));
			if (!this.singleFreq)
			{
				typeConfig.Add(new Type(Type.P,1));
			}
			typeConfig.Add(new Type(Type.L,1));
			typeConfig.Add(new Type(Type.D,1));
			typeConfig.Add(new Type(Type.S,1));
			if (!this.singleFreq)
			{
				typeConfig.Add(new Type(Type.P,2));
				typeConfig.Add(new Type(Type.L,2));
				typeConfig.Add(new Type(Type.D,2));
				typeConfig.Add(new Type(Type.S,2));
			}

		}

		public RinexV2Producer(bool needApproxPos, bool singleFreq, string marker) : this(needApproxPos, singleFreq, marker, 0)
		{
		}

		public RinexV2Producer(bool needApproxPos, bool singleFreq) : this(needApproxPos, singleFreq, null, 0)
		{
			this.standardFilename = false;
		}

		/* (non-Javadoc)
		 * @see org.gogpsproject.StreamEventListener#addEphemeris(org.gogpsproject.EphGps)
		 */
		public virtual void addEphemeris(EphGps eph)
		{

		}

		/* (non-Javadoc)
		 * @see org.gogpsproject.StreamEventListener#addIonospheric(org.gogpsproject.IonoGps)
		 */
		public virtual void addIonospheric(IonoGps iono)
		{

		}

		/* (non-Javadoc)
		 * @see org.gogpsproject.StreamEventListener#addObservations(org.gogpsproject.Observations)
		 */
		public virtual void addObservations(Observations o)
		{
			lock (this)
			{
				Time epoch = o.RefTime;
				int DOY = epoch.DayOfYear;
				if (DOY >= this.minDOY)
				{
					if (this.standardFilename && (this.outFilename == null || this.DOYold != DOY))
					{
						streamClosed();

						if (this.enableZip && this.outFilename != null)
						{
							sbyte[] buffer = new sbyte[1024];

							try
							{

								string zn = this.outFilename + ".zip";
								FileOutputStream fos = new FileOutputStream(zn);
								ZipOutputStream zos = new ZipOutputStream(fos);
								string[] tokens = this.outFilename.Split("/|\\\\", true);
								string fn = "";
								if (tokens.Length > 0)
								{
									fn = tokens[tokens.Length - 1].Trim();
								}
								ZipEntry ze = new ZipEntry(fn);
								zos.putNextEntry(ze);
								FileInputStream @in = new FileInputStream(this.outFilename);

								int len;
								while ((len = @in.read(buffer)) > 0)
								{
									zos.write(buffer, 0, len);
								}

								@in.close();
								zos.closeEntry();
								zos.close();

								File file = new File(this.outFilename);
								file.delete();

								Console.WriteLine("--RINEX file compressed as " + zn);

							}
							catch (IOException ex)
							{
								Console.WriteLine(ex.ToString());
								Console.Write(ex.StackTrace);
							}
						}

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

						char session = '0';
						int year = epoch.Year2c;
						string outFile = outputDir + "/" + marker + string.Format("{0:D3}", DOY) + session + "." + year + "o";
						File f = new File(outFile);

						while (f.exists())
						{
							session++;
							outFile = outputDir + "/" + marker + string.Format("{0:D3}", DOY) + session + "." + year + "o";
							f = new File(outFile);
						}

						Console.WriteLine("Started writing RINEX file " + outFile);
						Filename = outFile;

						DOYold = DOY;

						headerWritten = false;
					}
					if (!headerWritten)
					{
						observations.Add(o);
						if (needApproxPos && approxPosition == null)
						{
							return;
						}

						try
						{
							writeHeader(approxPosition, observations[0]);
						}
						catch (IOException e)
						{
							Console.WriteLine(e.ToString());
							Console.Write(e.StackTrace);
						}

						foreach (Observations obs in observations)
						{
							try
							{
								writeObservation(obs);
							}
							catch (IOException e)
							{
								Console.WriteLine(e.ToString());
								Console.Write(e.StackTrace);
							}
						}

						observations.Clear();
						headerWritten = true;
					}
					else
					{
						try
						{
							writeObservation(o);
						}
						catch (IOException e)
						{
							Console.WriteLine(e.ToString());
							Console.Write(e.StackTrace);
						}
					}
				}
			}

		}

		/* (non-Javadoc)
		 * @see org.gogpsproject.StreamEventListener#setDefinedPosition(org.gogpsproject.Coordinates)
		 */
		public virtual Coordinates DefinedPosition
		{
			set
			{
				lock (this)
				{
					this.approxPosition = value;
				}
			}
		}

		/* (non-Javadoc)
		 * @see org.gogpsproject.StreamEventListener#streamClosed()
		 */
		public virtual void streamClosed()
		{
			try
			{
				ps.close();
			}
			catch (Exception)
			{

			}
			try
			{
				fos.close();
			}
			catch (Exception)
			{

			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeHeader(org.gogpsproject.positioning.Coordinates approxPosition,org.gogpsproject.producer.Observations firstObservation) throws java.io.IOException
		private void writeHeader(Coordinates approxPosition, Observations firstObservation)
		{

	//	          2              OBSERVATION DATA    G (GPS)             RINEX VERSION / TYPE
	//	     CCRINEXO V2.4.1 LH  Bernese             28-APR-08 17:51     PGM / RUN BY / DATE
	//	     TPS2RIN 1.40        GEOMATICA/IREALP    28-APR-08 12:59     COMMENT
	//	     BUILD FEB  4 2004 (C) TOPCON POSITIONING SYSTEMS            COMMENT
	//	     D:\GPSCOMO\TB01H\2008\04\28\CO6C79~1.JPS                    COMMENT
	//	     SE TPS 00000000                                             COMMENT
	//	     COMO                                                        MARKER NAME
	//	     12761M001                                                   MARKER NUMBER
	//	     GEOMATICA/IREALP    MILANO POLYTECHNIC                      OBSERVER / AGENCY
	//	     8PRM6AZ2EBK         TPS ODYSSEY_E       3.1 JAN,24,2007 P1  REC # / TYPE / VERS
	//	     217-0400            TPSCR3_GGD      CONE                    ANT # / TYPE
	//	       4398306.2809   704149.8723  4550154.6777                  APPROX POSITION XYZ
	//	             0.2134        0.0000        0.0000                  ANTENNA: DELTA H/E/N
	//	          1     1                                                WAVELENGTH FACT L1/2
	//	          7    C1    P1    P2    L1    L2    D1    D2            # / TYPES OF OBSERV
	//	          1                                                      INTERVAL
	//	       2008     4    28    12     0    0.000000                  TIME OF FIRST OBS
	//	                                                                 END OF HEADER
			writeLine(sf("",5) + sf("2",15) + sf("OBSERVATION DATA",20) + sf("G (GPS)",20) + se("RINEX VERSION / TYPE",20), false);
			appendLine(sf("goGPS-java",20) + sf("",20) + sf(sdfHeader.format(DateTime.getInstance(TZ).Ticks).ToUpper(),20) + se("PGM / RUN BY / DATE",20));
			appendLine(sf("",20 * 3) + se("MARKER NAME",20));
			appendLine(sf("",20 * 3) + se("MARKER NUMBER",20));
			appendLine(sf("",20 * 3) + se("OBSERVER / AGENCY",20));
			appendLine(sf("",20 * 3) + se("REC # / TYPE / VERS",20));
			appendLine(sf("",20 * 3) + se("ANT # / TYPE",20));
			if (approxPosition != null)
			{
				appendLine(sp(dfX4.format(approxPosition.X),14,1) + sp(dfX4.format(approxPosition.Y),14,1) + sp(dfX4.format(approxPosition.Z),14,1) + sf("",18) + se("APPROX POSITION XYZ",20));
			}
			else
			{
				appendLine(sp(dfX4.format(0.0),14,1) + sp(dfX4.format(0.0),14,1) + sp(dfX4.format(0.0),14,1) + sf("",18) + se("APPROX POSITION XYZ",20));
			}
			appendLine(sp(dfX4.format(0.0),14,1) + sp(dfX4.format(0.0),14,1) + sp(dfX4.format(0.0),14,1) + sf("",18) + se("ANTENNA: DELTA H/E/N",20));
			bool found = false;
			foreach (Type t in typeConfig)
			{
				if (t.ToString().Equals("L2"))
				{
					found = true;
					break;
				}
			}
			int wf1 = 2; //single frequency (hypothesizing ublox-type low-cost receiver, i.e. with half cycle ambiguities)
			int wf2 = 0;
			if (found)
			{
				wf1 = 1; //dual frequency (hypothesizing full cycle ambiguities)
				wf2 = 1;
			}
			appendLine(sp(dfX.format(wf1),6,1) + sp(dfX.format(wf2),6,1) + sf("",6) + sf("",6) + sf("",6) + sf("",6) + sf("",6) + sf("",6) + sf("",12) + se("WAVELENGTH FACT L1/2",20));

			string line = "";
			int cols = 60;
			line += sp(dfX.format(typeConfig.Count),6,1);
			cols -= 6;
			foreach (Type t in typeConfig)
			{
				line += sp(t.ToString(),6,1);
				cols -= 6;
			}
			line += se("",cols);
			line += se("# / TYPES OF OBSERV",20);

			appendLine(line);
			//appendLine(sp(dfX.format(1),6,1)+sf("",60-1*6)+se("INTERVAL",20));

			if (firstObservation != null)
			{
				DateTime c = DateTime.getInstance(TZ);
				c.TimeInMillis = firstObservation.RefTime.Msec;
				appendLine(sp(dfX.format(c.Year),6,1) + sp(dfX.format(c.Month + 1),6,1) + sp(dfX.format(c.Day),6,1) + sp(dfX.format(c.get(DateTime.HOUR_OF_DAY)),6,1) + sp(dfX.format(c.Minute),6,1) + sp(dfX7.format(c.Second + c.Millisecond / 1000.0),13,1) + sp("GPS",8,1) + sf("",9) + se("TIME OF FIRST OBS",20));
			}

			appendLine(sf("",60) + se("END OF HEADER",20));
		}


	/// <returns> the typeConfig </returns>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.ArrayList<Type> getTypeConfig()
		public virtual List<Type> TypeConfig
		{
			get
			{
				return (List<Type>)typeConfig.clone();
			}
			set
			{
				if (headerWritten)
				{
					throw new Exception("Header already written.");
				}
				this.typeConfig = value;
			}
		}


		//	 10  3  2 13 20  0.0000000  0 12G11G13G32G04G20G17G23R15R20R21R05R04
	//	  24501474.376                    24501481.324   128756223.92705 100329408.23106
	//	        42.000          21.000
	//	  20630307.428                    20630311.680   108413044.43906  84477784.53807
	//	        47.000          39.000
	//	  23383145.918                    23383151.148   122879286.75106  95750102.77806
	//	        43.000          27.000
	//	  21517480.617                    21517485.743   113075085.94506  88110522.62107
	//	        46.000          37.000
	//	  20912191.322                    20912194.474   109894342.71907  85631979.97507
	//	        51.000          39.000
	//	  23257094.649                    23257100.632   122216783.06806  95233917.17406
	//	        43.000          27.000
	//	  20161355.093                    20161358.227   105948754.59007  82557486.35607
	//	        51.000          40.000
	//	  20509222.093    20509221.179    20509227.404   109595309.53707  85240851.57508
	//	        49.000          44.000
	//	  23031155.571    23031156.184    23031167.285   123157678.29405  95789331.13107
	//	        41.000          38.000
	//	  21173954.694    21173954.486    21173960.635   113305812.57507  88126783.03008
	//	        49.000          45.000
	//	  20128038.579    20128037.431    20128044.524   107596015.91107  83685880.30708
	//	        52.000          46.000
	//	  23792524.319    23792523.288    23792533.414   127408153.57805  99095296.49707
	//	        40.000          37.000
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObservation(org.gogpsproject.producer.Observations o) throws java.io.IOException
		private void writeObservation(Observations o)
		{
			//System.out.println(o);
			DateTime c = DateTime.getInstance(TZ);
			c.TimeInMillis = o.RefTime.Msec;

			string line = "";
			line += sp(dfX.format(c.Year - 2000),3,1);
			line += sp(dfX.format(c.Month + 1),3,1);
			line += sp(dfX.format(c.Day),3,1);
			line += sp(dfX.format(c.get(DateTime.HOUR_OF_DAY)),3,1);
			line += sp(dfX.format(c.Minute),3,1);
			line += sp(dfX7.format(c.Second + c.Millisecond / 1000.0 + o.RefTime.Fraction / 1000),11,1);
			line += sp(dfX.format(o.EventFlag),3,1);
			int gpsSize = 0;
			for (int i = 0;i < o.NumSat;i++)
			{
				if (o.getSatByIdx(i).SatID <= 32)
				{
					gpsSize++;
				}
			}
			line += sp(dfX.format(gpsSize),3,1);
			int cnt = 0;
			for (int i = 0;i < o.NumSat;i++)
			{
				if (cnt >= 12 && cnt % 12 == 0)
				{
					writeLine(line, true);
					line = "                                ";
				}
				line += o.getSatByIdx(i).SatType + dfXX.format(o.getSatID(i));
				cnt++;
			}
			writeLine(line, true);

			for (int i = 0;i < o.NumSat;i++)
			{
				if (o.getSatByIdx(i).SatID <= 32) // skip non GPS IDs
				{
					ObservationSet os = o.getSatByIdx(i);
					line = "";
					cnt = 0;
					foreach (Type t in typeConfig)
					{
						switch (t.Type)
						{
						case Type.C:
							if (os.getCodeC(t.Frequency - 1) == 0)
							{
								os.setCodeC(t.Frequency - 1, double.NaN);
							}
							line += double.IsNaN(os.getCodeC(t.Frequency - 1))?sf("",16):sp(dfX3.format(os.getCodeC(t.Frequency - 1)),14,1) + "  ";
							break;
						case Type.P:
							if (os.getCodeP(t.Frequency - 1) == 0)
							{
								os.setCodeP(t.Frequency - 1, double.NaN);
							}
							line += double.IsNaN(os.getCodeP(t.Frequency - 1))?sf("",16):sp(dfX3.format(os.getCodeP(t.Frequency - 1)),14,1) + "  ";
							break;
						case Type.L:
							if (os.getPhaseCycles(t.Frequency - 1) == 0 || Math.Abs(os.getPhaseCycles(t.Frequency - 1)) < 1e-15)
							{
								os.setPhaseCycles(t.Frequency - 1, double.NaN);
							}
							line += double.IsNaN(os.getPhaseCycles(t.Frequency - 1))?sf("",14):sp(dfX3.format(os.getPhaseCycles(t.Frequency - 1)),14,1); // L
							line += os.getLossLockInd(t.Frequency - 1) < 0?" ":dfX.format(os.getLossLockInd(t.Frequency - 1)); // L1 Loss of Lock Indicator
							line += float.IsNaN(os.getSignalStrength(t.Frequency - 1))?" ":dfX.format(Math.Floor(os.getSignalStrength(t.Frequency - 1) / 6)); // L1 Signal Strength Indicator
							break;
						case Type.D:
							if (os.getDoppler(t.Frequency - 1) == 0)
							{
								os.setDoppler(t.Frequency - 1, float.NaN);
							}
							line += float.IsNaN(os.getDoppler(t.Frequency - 1))?sf("",16):sp(dfX3.format(os.getDoppler(t.Frequency - 1)),14,1) + "  ";
							break;
						case Type.S:
							if (os.getSignalStrength(t.Frequency - 1) == 0)
							{
								os.setSignalStrength(t.Frequency - 1, float.NaN);
							}
							line += float.IsNaN(os.getSignalStrength(t.Frequency - 1))?sf("",16):sp(dfX3.format(os.getSignalStrength(t.Frequency - 1)),14,1) + "  ";
							break;
						}
						cnt++;
						if (cnt == typeConfig.Count || cnt == 5)
						{
							writeLine(line, true);
							line = "";
							cnt = 0;
						}
					}
					if (typeConfig.Count > 5)
					{
						writeLine(line, true);
					}
				}
			}

		}

		// space end
		private string se(string @in, int max)
		{
			return sf(@in,max,0);
		}
		// space fill with 1 space margin
		private string sf(string @in, int max)
		{
			return sf(@in,max,1);
		}
		// space fill with margin
		private string sf(string @in, int max, int margin)
		{
			if (@in.Length == max - margin)
			{
				while (@in.Length < max)
				{
					@in += " ";
				}
				return @in;
			}
			if (@in.Length > max - margin)
			{
				return @in.Substring(0, max - margin) + " ";
			}
			while (@in.Length < max)
			{
				@in += " ";
			}

			return @in;
		}
		// space prepend with margin
		private string sp(string @in, int max, int margin)
		{
			if (@in.Length == max - margin)
			{
				while (@in.Length < max)
				{
					@in = " " + @in;
				}
				return @in;
			}
			if (@in.Length > max - margin)
			{
				return @in.Substring(0, max);
				//return in.substring(0, max-margin)+" ";
			}
			while (@in.Length < max)
			{
				@in = " " + @in;
			}

			return @in;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void appendLine(String line) throws java.io.IOException
		private void appendLine(string line)
		{
			writeLine(line, true);
		}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeLine(String line, boolean append) throws java.io.IOException
		private void writeLine(string line, bool append)
		{

			FileOutputStream fos = this.fos;
			PrintStream ps = this.ps;
			if (this.fos == null)
			{
				fos = new FileOutputStream(outFilename, append);
				ps = new PrintStream(fos);
			}

			ps.println(line);
			//System.out.println(line);

			ps.flush();
			if (this.fos == null)
			{
				ps.close();
				fos.close();
			}


		}

		public virtual Observations CurrentObservations
		{
			get
			{
				// TODO Auto-generated method stub
				return null;
			}
		}

		public virtual void pointToNextObservations()
		{
			// TODO Auto-generated method stub

		}

		public virtual string Filename
		{
			set
			{
				this.outFilename = value;
				try
				{
					fos = new FileOutputStream(value, false);
					ps = new PrintStream(fos);
				}
				catch (FileNotFoundException e)
				{
					Console.WriteLine(e.ToString());
					Console.Write(e.StackTrace);
				}
			}
		}

		public virtual string OutputDir
		{
			set
			{
				this.outputDir = value;
			}
		}

		public virtual void enableCompression(bool enableZip)
		{
			this.enableZip = enableZip;
		}
	}

}