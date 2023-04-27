using System;

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
namespace org.gogpsproject.consumer
{


	using RoverPosition = org.gogpsproject.positioning.RoverPosition;
	/// <summary>
	/// <para>
	/// Produces TXT file
	/// </para>
	/// 
	/// @author Eugenio Realini
	/// </summary>

	public class TxtProducerCoarseTime : PositionConsumer
	{

		private static DecimalFormat f = new DecimalFormat("0.000");
		private static DecimalFormat g = new DecimalFormat("0.00000000");

		private SimpleDateFormat dateTXT = new SimpleDateFormat("yy/MM/dd");
		private SimpleDateFormat timeTXT = new SimpleDateFormat("HH:mm:ss.SSS");

		private string filename = null;
		private bool debug = false;

		private static readonly TimeZone TZ = TimeZone.getTimeZone("GMT");
		internal FileWriter @out;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public TxtProducerCoarseTime(String filename) throws java.io.IOException
		public TxtProducerCoarseTime(string filename)
		{
			this.filename = filename;

			writeHeader();

			dateTXT.TimeZone = TZ;
			timeTXT.TimeZone = TZ;
		}

		/* (non-Javadoc)
	   * @see org.gogpsproject.producer.PositionConsumer#event(int)
	   */
	  public virtual void @event(int @event)
	  {
		switch (@event)
		{
			case PositionConsumer_Fields.EVENT_START_OF_TRACK:
			@out = writeHeader();
		  break;
			case PositionConsumer_Fields.EVENT_END_OF_TRACK:
		  break;
		}
	  }

	  /* (non-Javadoc)
	   * @see org.gogpsproject.producer.PositionConsumer#startOfTrack()
	   */
	  public virtual FileWriter writeHeader()
	  {
		try
		{
		  FileWriter @out = new FileWriter(filename);

		  @out.write("Index         Status   Sats      Date      RTC time       FIX time     cErr " + "     Latitude    Longitude     Altitude    HDOP   eRes" + "   cErrRate" + "\r\n");
	//            "GPS week        GPS tow" +
		  @out.flush();
		  return @out;
		}
		catch (IOException e)
		{
		  Console.WriteLine(e.ToString());
		  Console.Write(e.StackTrace);
		}
		return null;
	  }

	  /* (non-Javadoc)
		 * @see org.gogpsproject.producer.PositionConsumer#addCoordinate(org.gogpsproject.Coordinates)
		 */
		public virtual void addCoordinate(RoverPosition c)
		{
			if (debug) // geod.get(0)
			{
				Console.WriteLine("Lon:" + g.format(c.GeodeticLongitude) + " " + "Lat:" + g.format(c.GeodeticLatitude) + " " + "H:" + f.format(c.GeodeticHeight) + "\t" + "P:" + c.getpDop() + " " + "H:" + c.gethDop() + " " + "V:" + c.getvDop() + " "); //geod.get(2) -  geod.get(2) -  geod.get(1)
			}

		try
		{
		  PrintWriter pw = new PrintWriter(@out);

		  pw.printf("%5d  ", c.obs.index);
		  pw.printf("%13s  ", c.status.ToString());
		  pw.printf("%2d/%2d  ", c.satsInUse, c.obs.NumSat);

		  // RTC date, time
		  string d = dateTXT.format(new DateTime(c.RefTime.Msec));
		  string t0 = timeTXT.format(new DateTime(c.sampleTime.Msec));

		  pw.printf("%8s%14s", d, t0);

		  if (c.status != Status.Valid)
		  {
			pw.printf("\r\n");
			@out.flush();
			return;
		  }

		  // FIX time
		  string t = timeTXT.format(new DateTime(c.RefTime.Msec));

		  double delta = (c.RefTime.Msec - c.sampleTime.Msec) / 1000.0;
		  pw.printf("%15s%10.3f", t, delta);

	//      //GPS week
	//      int week = c.getRefTime().getGpsWeek();
	//      pw.printf("%12d", week);

	//      //GPS time-of-week (tow)
	//      double tow = c.getRefTime().getGpsTime();
	//      pw.printf("%15.3f", tow);

		  //latitude, longitude, ellipsoidal height
		  double lat = c.GeodeticLatitude;
		  double lon = c.GeodeticLongitude;
		  double hEllips = c.GeodeticHeight;

		  pw.printf("%13.5f%13.5f%13.5f", lat, lon, hEllips);

		  pw.printf("%8.1f", c.gethDop());

		  pw.printf("%7.1f", c.eRes);

		  pw.printf("%9.0f", c.clockErrorRate);

		  pw.printf("\r\n");
		  @out.flush();

		}
		catch (System.NullReferenceException e)
		{
		  Console.WriteLine(e.ToString());
		  Console.Write(e.StackTrace);
		}
		catch (IOException e)
		{
		  // TODO Auto-generated catch block
		  Console.WriteLine(e.ToString());
		  Console.Write(e.StackTrace);
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


	}

}