using System;
using System.Collections.Generic;

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

	using org.gogpsproject.producer.parser;

	public class UBXSerialConnection : AbstractSerialConnection<UBXSerialReader>
	{

		protected internal int setEphemerisRate = 10;
		protected internal int setIonosphereRate = 60;
	  protected internal bool? enableDebug_Renamed = false;
	  protected internal bool? enableTimetag_Renamed = false;
	  protected internal string outputDir = "./out";
		protected internal IList<string> enableNmeaList;

		public UBXSerialConnection(string portName, int speed)
		{
			this.portName = portName;
			this.speed = speed;
		}

		/* (non-Javadoc)
		 * @see org.gogpsproject.StreamResource#init()
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void init() throws Exception
		public override void init()
		{

	//		boolean conn = false;
	//		try {
			  base.init();
					prod = new UBXSerialReader(inputStream,outputStream,portName,outputDir);
					prod.Rate = this.setMeasurementRate;
					prod.enableAidEphMsg(this.setEphemerisRate);
					prod.enableAidHuiMsg(this.setIonosphereRate);
					prod.enableSysTimeLog(this.enableTimetag_Renamed);
					prod.enableDebugMode(this.enableDebug_Renamed);
					prod.enableNmeaMsg(this.enableNmeaList);
					prod.start();

					connected = true;
					Console.WriteLine("Connection on " + portName + " established");
					//conn = true;

	//		} catch (NoSuchPortException e) {
	//			System.out.println("The connection could not be made");
	//			e.printStackTrace();
	//		} catch (PortInUseException e) {
	//			System.out.println("The connection could not be made");
	//			e.printStackTrace();
	//		} catch (UnsupportedCommOperationException e) {
	//			System.out.println("The connection could not be made");
	//			e.printStackTrace();
	//		} catch (IOException e) {
	//			System.out.println("The connection could not be made");
	//			e.printStackTrace();
	//		}
	//		return conn;
		}


		/* (non-Javadoc)
		 * @see org.gogpsproject.StreamResource#release(boolean, long)
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void release(boolean waitForThread, long timeoutMs) throws InterruptedException
		public override void release(bool waitForThread, long timeoutMs)
		{

			if (prod != null)
			{
				prod.stop(waitForThread, timeoutMs);
			}
			base.release();
		}

		public virtual int MeasurementRate
		{
			set
			{
				if (prod != null)
				{
					prod.Rate = value;
				}
				else
				{
					this.setMeasurementRate = value;
				}
			}
		}

		public virtual void enableEphemeris(int? ephRate)
		{
			if (prod != null)
			{
				prod.enableAidEphMsg(ephRate);
			}
			else
			{
				this.setEphemerisRate = ephRate;
			}
		}

		public virtual void enableIonoParam(int? ionRate)
		{
			if (prod != null)
			{
				prod.enableAidHuiMsg(ionRate);
			}
			else
			{
				this.setIonosphereRate = ionRate;
			}
		}

		public virtual void enableNmeaSentences(IList<string> nmeaList)
		{
				if (prod != null)
				{
					prod.enableNmeaMsg(nmeaList);
				}
				else
				{
					this.enableNmeaList = nmeaList;
				}
		}

		public virtual void enableTimetag(bool? enableTim)
		{
			if (prod != null)
			{
				prod.enableSysTimeLog(enableTim);
			}
			else
			{
				this.enableTimetag_Renamed = enableTim;
			}
		}

		public virtual void enableDebug(bool? enableDebug)
		{
			if (prod != null)
			{
				prod.enableDebugMode(enableDebug);
			}
			else
			{
				this.enableDebug_Renamed = enableDebug;
			}
		}

		public virtual string OutputDir
		{
			set
			{
				if (prod != null)
				{
					prod.OutputDir = value;
				}
				else
				{
					this.outputDir = value;
				}
			}
		}
	}

}