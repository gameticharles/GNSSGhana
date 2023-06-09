﻿using System;
using System.Collections.Generic;

/*
 * Copyright (c) 2011 Eugenio Realini, Mirko Reguzzoni, Cryms sagl - Switzerland, Daisuke Yoshida. All Rights Reserved.
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
	/// <para>
	/// Produces Rinex 3 as StreamEventListener
	/// </para>
	/// 
	/// @author Daisuke YOSHIDA (OCU)
	/// </summary>


	public class RinexV3Producer : StreamEventListener
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

		internal bool gpsEnable = true; // enable GPS data writing
		internal bool qzsEnable = false; // enable QZSS data writing
		internal bool gloEnable = false; // enable GLONASS data writing
		internal bool galEnable = false; // enable Galileo data writing
		internal bool bdsEnable = false; // enable BeiDou data writing

		private static readonly TimeZone TZ = TimeZone.getTimeZone("GMT");

		public RinexV3Producer(bool needApproxPos, bool singleFreq, string marker, bool?[] multiConstellation, int minDOY)
		{

			this.needApproxPos = needApproxPos;
			this.singleFreq = singleFreq;
			this.marker = marker;
			this.minDOY = minDOY;

			gpsEnable = multiConstellation[0];
			qzsEnable = multiConstellation[1];
			gloEnable = multiConstellation[2];
			galEnable = multiConstellation[3];
			bdsEnable = multiConstellation[4];

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

		public RinexV3Producer(bool needApproxPos, bool singleFreq, string marker, bool?[] multiConstellation) : this(needApproxPos, singleFreq, marker, multiConstellation, 0)
		{
		}

		public RinexV3Producer(bool needApproxPos, bool singleFreq, string marker) : this(needApproxPos, singleFreq, marker, null, 0)
		{
		}

		public RinexV3Producer(bool needApproxPos, bool singleFreq) : this(needApproxPos, singleFreq, null, null, 0)
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

	//	     3.01           OBSERVATION DATA    M: Mixed            RINEX VERSION / TYPE
	//	     goGPS               OCU_KU              20131021 063917 UTC PGM / RUN BY / DATE 
	//	     OCUS                                                        MARKER NAME         
	//	     NON_GEODETIC                                                MARKER TYPE         
	//	     DY_ER               OCU_KU                                  OBSERVER / AGENCY   
	//	                         NVS                                     REC # / TYPE / VERS 
	//	                                                                 ANT # / TYPE        
	//	             0.0000        0.0000        0.0000                  APPROX POSITION XYZ 
	//	             0.0000        0.0000        0.0000                  ANTENNA: DELTA H/E/N
	//	          2     0                                                WAVELENGTH FACT L1/2
	//	     G    4 C1C L1C S1C D1C                                      SYS / # / OBS TYPES 
	//	     R    4 C1C L1C S1C D1C                                      SYS / # / OBS TYPES 
	//	     E    4 C1X L1X S1X D1X                                      SYS / # / OBS TYPES 
	//	     J    4 C1C L1C S1C D1C                                      SYS / # / OBS TYPES 
	//	     DBHZ                                                        SIGNAL STRENGTH UNIT
	//	          1.000                                                  INTERVAL            
	//	       2013    10    21     5    27   15.9996319     GPS         TIME OF FIRST OBS   
	//	     G                                                           SYS / PHASE SHIFTS  
	//	     R                                                           SYS / PHASE SHIFTS  
	//	     E                                                           SYS / PHASE SHIFTS  
	//	     J                                                           SYS / PHASE SHIFTS  
	//	                                                                 END OF HEADER    


			writeLine(sf("",5) + sf("3.01",15) + sf("OBSERVATION DATA",20) + sf("M: Mixed",20) + se("RINEX VERSION / TYPE",20), false);
			appendLine(sf("goGPS-java",20) + sf("OCU",20) + sf(sdfHeader.format(DateTime.getInstance(TZ).Ticks).ToUpper(),20) + se("PGM / RUN BY / DATE",20));
			appendLine(sf("OCU",20 * 3) + se("MARKER NAME",20));
			appendLine(sf("NON_GEODETIC",20 * 3) + se("MARKER TYPE",20));
			appendLine(sf("GSCC@OCU",20 * 3) + se("OBSERVER / AGENCY",20));
			appendLine(sf("NVS",20 * 3) + se("REC # / TYPE / VERS",20));
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

			if (gpsEnable)
			{
				string line = "G";
				int cols = 56;
				line += sp(dfX.format(typeConfig.Count),5,3);
				cols -= 6;
				foreach (Type t in typeConfig)
				{
					line += sp(t.ToString() + 'C',4,1);
					cols -= 3;
				}
				line += se("",cols);
				line += se("SYS / # / OBS TYPES ",20);
				appendLine(line);
			}

			if (gloEnable)
			{
				string line = "R";
				int cols = 56;
				line += sp(dfX.format(typeConfig.Count),5,3);
				cols -= 6;
				foreach (Type t in typeConfig)
				{
					line += sp(t.ToString() + 'C',4,1);
					cols -= 3;
				}
				line += se("",cols);
				line += se("SYS / # / OBS TYPES ",20);
				appendLine(line);
			}

			if (galEnable)
			{
				string line = "E";
				int cols = 56;
				line += sp(dfX.format(typeConfig.Count),5,3);
				cols -= 6;
				foreach (Type t in typeConfig)
				{
					line += sp(t.ToString() + 'X',4,1);
					cols -= 3;
				}
				line += se("",cols);
				line += se("SYS / # / OBS TYPES ",20);
				appendLine(line);
			}

			if (bdsEnable)
			{
				string line = "C";
				int cols = 56;
				line += sp(dfX.format(typeConfig.Count),5,3);
				cols -= 6;
				foreach (Type t in typeConfig)
				{
					line += sp(t.ToString() + 'I',4,1);
					cols -= 3;
				}
				line += se("",cols);
				line += se("SYS / # / OBS TYPES ",20);
				appendLine(line);
			}

			if (qzsEnable)
			{
				string line = "J";
				int cols = 56;
				line += sp(dfX.format(typeConfig.Count),5,3);
				cols -= 6;
				foreach (Type t in typeConfig)
				{
					line += sp(t.ToString() + 'C',4,1);
					cols -= 3;
				}
				line += se("",cols);
				line += se("SYS / # / OBS TYPES ",20);
				appendLine(line);
			}

			//appendLine(sp(dfX.format(1),6,1)+sf("",60-1*6)+se("INTERVAL",20));

			if (firstObservation != null)
			{
				DateTime c = DateTime.getInstance(TZ);
				c.TimeInMillis = firstObservation.RefTime.Msec;
				appendLine(sp(dfX.format(c.Year),6,1) + sp(dfX.format(c.Month + 1),6,1) + sp(dfX.format(c.Day),6,1) + sp(dfX.format(c.get(DateTime.HOUR_OF_DAY)),6,1) + sp(dfX.format(c.Minute),6,1) + sp(dfX7.format(c.Second + c.Millisecond / 1000.0),13,1) + sp("GPS",8,1) + sf("",9) + se("TIME OF FIRST OBS",20));
			}

			appendLine(sf("DBHZ",20 * 3) + se("SIGNAL STRENGTH UNIT",20));
			appendLine(sf("G",20 * 3) + se("SYS / PHASE SHIFTS",20));
			appendLine(sf("",60) + se("END OF HEADER",20));
		}


	/// <returns> the typeConfig </returns>
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


	//	> 2013 10 21  5 27 15.9996319  0 12
	//	G02  24302142.768 7  -7040077.03107        45.000 7      3026.216 7
	//	G04  21034956.023 8 -12544569.50908        51.000 8      2030.071 8
	//	G12  24712920.728 4                        26.000 4      2631.261 4
	//	G13  22615468.719 8 -16359274.25108        49.000 8      3201.976 8
	//	G17  20229009.570 8  -8621207.89608        51.000 8       651.906 8
	//	G20  22365294.155 7   8462486.38907        46.000 7     -2283.400 7
	//	G23  22380053.521 7 -11806763.28207        46.000 7      1305.138 7
	//	G28  23491793.548 6  11273447.98706        41.000 6     -2604.791 6
	//	E12  26431512.796 7  -1032019.36107        47.000 7      2643.264 7
	//	E19  25076155.320 7 -13134718.07407        43.000 7      1831.788 7
	//	E20  25160867.569 7  -1070764.90207        46.000 7      -619.438 7
	//	J01  38690760.026 7   1019110.27407        47.000 7       208.927 7


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void writeObservation(org.gogpsproject.producer.Observations o) throws java.io.IOException
		private void writeObservation(Observations o)
		{
			//System.out.println(o);
			DateTime c = DateTime.getInstance(TZ);
			c.TimeInMillis = o.RefTime.Msec;

			string line = ">";
			line += sp(dfX.format(c.Year),5,1);
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
			writeLine(line, true);

	//		int cnt=0;
	//		for(int i=0;i<o.getNumSat();i++){
	//			if(o.getSatByIdx(i).getSatID()<=32){ // skip non GPS IDs
	//				if(cnt==12){
	//					writeLine(line, true);
	//					line = "                                ";
	//				}
	//				line += "G"+dfXX.format(o.getSatID(i));
	//				cnt++;
	//			}
	//		}
	//		writeLine(line, true);

			for (int i = 0;i < o.NumSat;i++)
			{
				if (o.getSatByIdx(i).SatID <= 32) // skip non GPS IDs
				{
					ObservationSet os = o.getSatByIdx(i);
					line = "";
					line += o.getGnssType(i) + dfXX.format(o.getSatID(i));
					int cnt = 0;
					foreach (Type t in typeConfig)
					{
						switch (t.Type)
						{
						case Type.C:
							line += double.IsNaN(os.getCodeC(t.Frequency - 1))?sf("",16):sp(dfX3.format(os.getCodeC(t.Frequency - 1)),14,1) + "  ";
							break;
						case Type.P:
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
							line += float.IsNaN(os.getDoppler(t.Frequency - 1))?sf("",16):sp(dfX3.format(os.getDoppler(t.Frequency - 1)),14,1) + "  ";
							break;
						case Type.S:
							line += float.IsNaN(os.getSignalStrength(t.Frequency - 1))?sf("",16):sp(dfX3.format(os.getSignalStrength(t.Frequency - 1)),14,1) + "  ";
							break;
						}
						cnt++;
						if (cnt == typeConfig.Count)
						{
							writeLine(line, true);
							line = "";
							cnt = 0;
						}
					}
	//				if (typeConfig.size() > 5) {
	//					writeLine(line, true);
	//				}
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
				return @in.Substring(0, max - margin) + " ";
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