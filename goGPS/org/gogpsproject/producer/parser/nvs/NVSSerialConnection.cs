using System;
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

	using org.gogpsproject.producer.parser;

	using SerialPort = gnu.io.SerialPort;

	public class NVSSerialConnection : AbstractSerialConnection<NVSSerialReader>
	{

		//private StreamEventListener streamEventListener;

		public NVSSerialConnection(string portName, int speed)
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

			bool reply;

			//try with NMEA

					prod = new NVSSerialReader(inputStream,outputStream,portName,outputDir);
					prod.enableDebugMode(this.enableDebug);
					reply = prod.setBinrProtocol();

					Thread.Sleep(100);
					serialPort.setSerialPortParams(speed, SerialPort.DATABITS_8, SerialPort.STOPBITS_1, SerialPort.PARITY_ODD);

					if (!reply)
					{
						//try with BINR
						inputStream = serialPort.InputStream;
						outputStream = serialPort.OutputStream;

						prod = new NVSSerialReader(inputStream,outputStream,portName,outputDir);
						prod.enableDebugMode(this.enableDebug);
						reply = prod.setBinrProtocol();
					}

					connected = true;
					Console.WriteLine("Connection on " + portName + " established");

					//nvsReader.setStreamEventListener(streamEventListener);
					prod.Rate = this.setMeasurementRate;
					prod.enableSysTimeLog(this.enableTimetag);
					prod.enableDebugMode(this.enableDebug);
					prod.start();
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

		public virtual NVSSerialConnection setMeasurementRate(int measRate)
		{
			if (prod != null)
			{
				prod.Rate = measRate;
			}
			else
			{
				this.setMeasurementRate = measRate;
			}
			return this;
		}

		public virtual NVSSerialConnection enableTimetag(bool? enableTim)
		{
			if (prod != null)
			{
				prod.enableSysTimeLog(enableTim);
			}
			else
			{
				this.enableTimetag = enableTim;
			}
		return this;
		}

		public virtual NVSSerialConnection enableDebug(bool? enableDebug)
		{
			if (prod != null)
			{
				prod.enableDebugMode(enableDebug);
			}
			else
			{
				this.enableDebug = enableDebug;
			}
		return this;
		}

		public virtual NVSSerialConnection setOutputDir(string outDir)
		{
			if (prod != null)
			{
				prod.OutputDir = outDir;
			}
			else
			{
				this.outputDir = outDir;
			}
		return this;
		}
	}

}