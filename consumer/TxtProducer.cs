using System;
using System.Collections.Generic;
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
namespace org.gogpsproject.consumer
{


	using RoverPosition = org.gogpsproject.positioning.RoverPosition;
	using DopType = org.gogpsproject.positioning.RoverPosition.DopType;
	/// <summary>
	/// <para>
	/// Produces TXT file
	/// </para>
	/// 
	/// @author Eugenio Realini
	/// </summary>

	public class TxtProducer : System.Threading.Thread, PositionConsumer
	{

		private static DecimalFormat f = new DecimalFormat("0.000");
		private static DecimalFormat g = new DecimalFormat("0.00000000");

		private SimpleDateFormat dateTXT = new SimpleDateFormat("yy/MM/dd");
		private SimpleDateFormat timeTXT = new SimpleDateFormat("HH:mm:ss.SSS");

		private string filename = null;
		private bool debug = false;

		private List<RoverPosition> positions = new List<RoverPosition>();

		private static readonly TimeZone TZ = TimeZone.getTimeZone("GMT");

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public TxtProducer(String filename) throws java.io.IOException
		public TxtProducer(string filename) : base("TxtProducer")
		{
			this.filename = filename;

			writeHeader();

			dateTXT.TimeZone = TZ;
			timeTXT.TimeZone = TZ;

			start();
		}

		/* (non-Javadoc)
		 * @see org.gogpsproject.producer.PositionConsumer#addCoordinate(org.gogpsproject.Coordinates)
		 */
		public virtual void addCoordinate(RoverPosition coord)
		{
			if (debug) // geod.get(0)
			{
				Console.WriteLine("Lon:" + g.format(coord.GeodeticLongitude) + " " + "Lat:" + g.format(coord.GeodeticLatitude) + " " + "H:" + f.format(coord.GeodeticHeight) + "\t" + "P:" + coord.getpDop() + " " + "H:" + coord.gethDop() + " " + "V:" + coord.getvDop() + " "); //geod.get(2) -  geod.get(2) -  geod.get(1)
			}

			positions.Add(coord);
		}

		/* (non-Javadoc)
		 * @see org.gogpsproject.producer.PositionConsumer#addCoordinate(org.gogpsproject.Coordinates)
		 */
		public virtual void writeCoordinate(RoverPosition coord, FileWriter @out)
		{
			try
			{

				PrintWriter pw = new PrintWriter(@out);

				//date, time
				string d = dateTXT.format(new DateTime(coord.RefTime.Msec));
				string t = timeTXT.format(new DateTime(coord.RefTime.Msec));

				pw.printf("%8s%16s", d, t);

				//GPS week
				int week = coord.RefTime.GpsWeek;

				pw.printf("%16d", week);

				//GPS time-of-week (tow)
				double tow = coord.RefTime.GpsTime;

				pw.printf("%16f", tow);

				//latitude, longitude, ellipsoidal height
				double lat = coord.GeodeticLatitude;
				double lon = coord.GeodeticLongitude;
				double hEllips = coord.GeodeticHeight;

				pw.printf("%16.8f%16.8f%16.3f", lat, lon, hEllips);

				//ECEF coordinates (X, Y, Z)
				double X = coord.X;
				double Y = coord.Y;
				double Z = coord.Z;

				pw.printf("%16.3f%16.3f%16.3f", X, Y, Z);

				//UTM north, UTM east, orthometric height, UTM zone
				int noData = -9999;
				double utmNorth = noData;
				double utmEast = noData;
				double hOrtho = noData;
				string utmZone = "-9999";

				pw.printf("%16.3f%16.3f%16.3f%16s", utmNorth, utmEast, hOrtho, utmZone);

				//HDOP, KHDOP
				double hdop = noData;
				double khdop = noData;
				if (coord.DopType == RoverPosition.DopType.KALMAN)
				{
				  khdop = coord.gethDop();
				}
				else
				{
				  hdop = coord.gethDop();
				}

				pw.printf("%16.3f%16.3f%n", hdop, khdop);

	//			out.write(lon + "," // geod.get(0)
	//					+ lat + "," // geod.get(1)
	//					+ h + "\n"); // geod.get(2)
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

		/* (non-Javadoc)
		 * @see org.gogpsproject.producer.PositionConsumer#startOfTrack()
		 */
		public virtual FileWriter writeHeader()
		{
			try
			{
				FileWriter @out = new FileWriter(filename);

				@out.write("    Date        GPS time        GPS week         GPS tow    " + "    Latitude       Longitude     h (ellips.)      " + "    ECEF X          ECEF Y          ECEF Z   " + "    UTM North        UTM East     h (orthom.)        UTM zone        " + "    HDOP           KHDOP\n");
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
		 * @see org.gogpsproject.producer.PositionConsumer#event(int)
		 */
		public virtual void @event(int @event)
		{
	//		if(event == EVENT_START_OF_TRACK){
	//			startOfTrack();
	//		}
			if (@event == PositionConsumer_Fields.EVENT_END_OF_TRACK)
			{
				// finish writing
				interrupt();
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


		/* (non-Javadoc)
		 * @see java.lang.Runnable#run()
		 */
		public override void run()
		{
			int last = 0;
		FileWriter @out = writeHeader();
			while (!Interrupted || last < positions.Count)
			{
			  //	goodDop = false;
		  for (; last < positions.Count; last++)
		  {
					writeCoordinate(positions[last], @out);
		  }
				try
				{
					Thread.Sleep(200);
				}
				catch (InterruptedException)
				{
				  interrupt();
				}
			}
		}

		public virtual void cleanStop()
		{
		interrupt();
		}
	}

}