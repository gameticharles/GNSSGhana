using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using java.io;
using java.lang;
using java.util;
using net.sf.jni4net;
using net.sf.jni4net.adaptors;
using System.IO;

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
	/// Produces KML file
	/// </para>
	/// 
	/// @author Lorenzo Patocchi cryms.com
	/// </summary>

	public class KmlProducer : System.Threading.Thread, PositionConsumer
	{

		private static DecimalFormat f = new DecimalFormat("0.000");
		private static DecimalFormat g = new DecimalFormat("0.00000000");

		private SimpleDateFormat timeKML = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss.SSS'Z'");

		private string filename = null;
		private string timeline = null;
		private int num = 0;

		private bool goodDop = false;
		private double goodDopThreshold = 0.0;
		private int timeSampleDelaySec = 0;

		private string circleColorLine = "FFCC99";
		private string circleOpacity = "88";
		private int circlePixelWidth = 2;

		private string goodColorLine = "00ff00";
		private string goodOpacity = "ff";
		private int goodLinePixelWidth = 3;
		private string worstColorLine = "0000ff";
		private string worstOpacity = "ff";
		private int worstLinePixelWidth = 3;
		private bool debug = false;

		private List<RoverPosition> positions = new List<RoverPosition>();

		private static readonly TimeZone TZ = TimeZone.getTimeZone("GMT");

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public KmlProducer(String filename, double goodDopTreshold, int timeSampleDelaySec) throws java.io.IOException
		public KmlProducer(string filename, double goodDopTreshold, int timeSampleDelaySec) : base("KmlProducer")
		{
			this.filename = filename;
			this.goodDopThreshold = goodDopTreshold;
			this.timeSampleDelaySec = timeSampleDelaySec;

			timeKML.TimeZone = TZ;

			goodDop = false;
			FileWriter @out = startOfTrack();
			if (@out != null)
			{

				endOfTrack(@out);
			}
			start();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public KmlProducer(String filename, double goodDopTreshold, int timeSampleDelaySec, String goodColorLine) throws java.io.IOException
		 public KmlProducer(string filename, double goodDopTreshold, int timeSampleDelaySec, string goodColorLine) : base("KmlProducer")
		 {
			this.goodColorLine = goodColorLine;
	//	   this.worstColorLine = goodColorLine;

			this.filename = filename;
			this.goodDopThreshold = goodDopTreshold;
			this.timeSampleDelaySec = timeSampleDelaySec;

			timeKML.TimeZone = TZ;

			goodDop = false;
			FileWriter @out = startOfTrack();
			if (@out != null)
			{

			  endOfTrack(@out);
			}
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
	//      ReceiverPositionObs c = (ReceiverPositionObs)coord;
	//      if( c.status != Status.Valid )
	//        return;

				bool prevDopResponse = goodDop;
				goodDop = coord.getpDop() < goodDopThreshold;
				if (prevDopResponse != goodDop)
				{
					@out.write("</coordinates></LineString></Placemark>\n" + "  <Placemark>\n" + "    <name></name>\n" + "    <description></description>\n" + "    <styleUrl>#LineStyle_" + (goodDop?"good":"worst") + "</styleUrl>\n" + "    <LineString>\n" + "      <tessellate>1</tessellate>\n" + "      <coordinates>\n");
				}

				string lon = g.format(coord.GeodeticLongitude);
				string lat = g.format(coord.GeodeticLatitude);
				string h = f.format(coord.GeodeticHeight);

				@out.write(lon + "," + lat + "," + h + "\n"); // geod.get(2) -  geod.get(1) -  geod.get(0)
				@out.flush();

				string t = timeKML.format(new DateTime(coord.RefTime.Msec));
	//			System.out.print("T:" + t);
	//			System.out.print(" Lon:" + lon);//geod.get(0)
	//			System.out.print(" Lat:" + lat);//geod.get(1)
				string dopLabel = "DOP";
				if (coord.DopType == RoverPosition.DopType.KALMAN)
				{
					dopLabel = "KDOP";
				}

				if (timeSampleDelaySec > 0 && (num++) % timeSampleDelaySec == 0)
				{

					timeline += "\n";
					timeline += "<Placemark>" + "<TimeStamp>" + "<when>" + t + "</when>" + "</TimeStamp>" + "<styleUrl>#dot-icon</styleUrl>" + "<Point>" + "<coordinates>" + lon + "," + lat + "," + h + "</coordinates>" + "</Point>" + "</Placemark>";
				}
			}
			catch (System.NullReferenceException e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
			catch (IOException e)
			{
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
		}

		/* (non-Javadoc)
		 * @see org.gogpsproject.producer.PositionConsumer#startOfTrack()
		 */
		public virtual FileWriter startOfTrack()
		{
			if (timeSampleDelaySec > 0)
			{
				timeline = "<Folder><open>1</open><Style><ListStyle><listItemType>checkHideChildren</listItemType></ListStyle></Style>";
			}
			try
			{
				FileWriter @out = new FileWriter(filename);

				@out.write("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n" + "<Document xmlns:kml=\"http://earth.google.com/kml/2.1\">\n" + "  <Style id=\"LineStyle_worst\"><LineStyle><color>" + worstOpacity + worstColorLine + "</color><width>" + worstLinePixelWidth + "</width></LineStyle><PolyStyle><color>" + worstOpacity + worstColorLine + "</color></PolyStyle></Style>\n" + "  <Style id=\"LineStyle_good\"><LineStyle><color>" + goodOpacity + goodColorLine + "</color><width>" + goodLinePixelWidth + "</width></LineStyle><PolyStyle><color>" + goodOpacity + goodColorLine + "</color></PolyStyle></Style>\n" + "  <Style id=\"CircleStyle\"><LineStyle><color>" + circleOpacity + circleColorLine + "</color><width>" + circlePixelWidth + "</width></LineStyle><PolyStyle><color>" + circleOpacity + circleColorLine + "</color></PolyStyle></Style>\n" + "  <Style id=\"dot-icon\"><IconStyle><Icon><href>https://storage.googleapis.com/support-kms-prod/SNP_2752129_en_v0</href></Icon><scale>0.3</scale></IconStyle></Style>\n" + "  <Folder><Placemark>\n" + "    <name></name>\n" + "    <description></description>\n" + "    <styleUrl>#LineStyle_" + (goodDop?"good":"worst") + "</styleUrl>\n" + "    <LineString>\n" + "      <tessellate>1</tessellate>\n" + "      <coordinates>\n");
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
		 * @see org.gogpsproject.producer.PositionConsumer#endOfTrack()
		 */
		public virtual void endOfTrack(FileWriter @out)
		{
			if (@out != null)
			{
				// Write KML footer part
				try
				{
					string circle = null;
	//				if(positions.size()>0){
	//					ReceiverPosition last = positions.get(positions.size()-1);
	//					circle = generateCircle(last.getGeodeticLatitude(), last.getGeodeticLongitude(), last.getGeodeticHeight(), 90, last.getpDop());
	//				}

					@out.write("</coordinates></LineString></Placemark></Folder>" + (timeline == null?"":timeline + "</Folder>") + (circle != null?circle:"") + "</Document>\n");
					// Close FileWriter
					@out.close();
				}
				catch (IOException e)
				{
					Console.WriteLine(e.ToString());
					Console.Write(e.StackTrace);
				}

			}
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

		private string generateCircle(double centerlat_form, double centerlong_form, double height, int num_points, double radius_form)
		{
			double lat1, long1;
			double d_rad;
			double d;
			double delta_pts;
			double radial, lat_rad, dlon_rad, lon_rad;

	//		double degreeToRadian = Math.PI / 180.0;

			string result = "";

			// convert coordinates to radians
			lat1 = Math.toRadians(centerlat_form);
			long1 = Math.toRadians(centerlong_form);

			// Earth measures
			// Year Name a (meters) b (meters) 1/f Where Used
			// 1980 International 6,378,137 6,356,752 298.257 Worldwide
			d = radius_form;
			d_rad = d / 6378137;

			result = "<Folder>\n<name>Circle</name>\n<visibility>1</visibility>\n<Placemark>\n<name></name>\n<styleUrl>CircleStyle</styleUrl>\n<LinearRing>\n<coordinates>\n";
			// System.out.write(c);
			// System.out.println(c);

			delta_pts = 360 / (double)num_points;

			// loop through the array and write path linestrings
			for (int i = 0; i < num_points; i++)
			{
				radial = Math.toRadians((double)i * delta_pts);
				//radial = Math.toRadians((double) i);

				// This algorithm is limited to distances such that dlon <pi/2
				lat_rad = Math.Asin(Math.Sin(lat1) * Math.Cos(d_rad) + Math.Cos(lat1) * Math.Sin(d_rad) * Math.Cos(radial));
				dlon_rad = Math.Atan2(Math.Sin(radial) * Math.Sin(d_rad) * Math.Cos(lat1), Math.Cos(d_rad) - Math.Sin(lat1) * Math.Sin(lat_rad));
				lon_rad = ((long1 + dlon_rad + Math.PI) % (2 * Math.PI)) - Math.PI;

				// write results
				result += "" + Math.toDegrees(lon_rad) + ",";
				result += "" + Math.toDegrees(lat_rad) + ",";
				result += "" + height + "\n";
			}
			// output footer
			result += "</coordinates>\n</LinearRing>\n</Placemark>\n</Folder>";

			return result;
		}

		private string reverse(string @string)
		{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET StringBuilder equivalent to the Java 'reverse' method:
			return (new StringBuilder(@string)).reverse().ToString();
		}

		/// <returns> the goodColorLine in hex format RRGGBB </returns>
		public virtual string GoodColorLine
		{
			get
			{
				return reverse(goodColorLine);
			}
			set
			{
				this.goodColorLine = reverse(value);
			}
		}


		/// <returns> the goodOpacity in hex format (range 00..FF) </returns>
		public virtual string GoodOpacity
		{
			get
			{
				return goodOpacity;
			}
			set
			{
				this.goodOpacity = value;
			}
		}


		/// <returns> the goodLinePixelWidth </returns>
		public virtual int GoodLinePixelWidth
		{
			get
			{
				return goodLinePixelWidth;
			}
			set
			{
				this.goodLinePixelWidth = value;
			}
		}


		/// <returns> the worstColorLine in hex format RRGGBB </returns>
		public virtual string WorstColorLine
		{
			get
			{
				return reverse(worstColorLine);
			}
			set
			{
				this.worstColorLine = reverse(value);
			}
		}


		/// <returns> the worstOpacity in hex format (range 00..FF) </returns>
		public virtual string WorstOpacity
		{
			get
			{
				return worstOpacity;
			}
			set
			{
				this.worstOpacity = value;
			}
		}


		/// <returns> the worstLinePixelWidth </returns>
		public virtual int WorstLinePixelWidth
		{
			get
			{
				return worstLinePixelWidth;
			}
			set
			{
				this.worstLinePixelWidth = value;
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


		public override void run()
		{
			int last = 0;
		FileWriter @out = startOfTrack();
			while (!Interrupted || last < positions.Count)
			{
	//					goodDop = false;
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
		endOfTrack(@out);
		}

		public virtual void cleanStop()
		{
			interrupt();
		}
	}


}