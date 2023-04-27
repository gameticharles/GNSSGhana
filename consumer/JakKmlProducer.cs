using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;

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
	using ObservationSet = org.gogpsproject.producer.ObservationSet;
	using Observations = org.gogpsproject.producer.Observations;

	using AltitudeMode = de.micromata.opengis.kml.v_2_2_0.AltitudeMode;
	using BalloonStyle = de.micromata.opengis.kml.v_2_2_0.BalloonStyle;
	using ColorMode = de.micromata.opengis.kml.v_2_2_0.ColorMode;
	using Data = de.micromata.opengis.kml.v_2_2_0.Data;
	using ExtendedData = de.micromata.opengis.kml.v_2_2_0.ExtendedData;
	using Folder = de.micromata.opengis.kml.v_2_2_0.Folder;
	using Icon = de.micromata.opengis.kml.v_2_2_0.Icon;
	using IconStyle = de.micromata.opengis.kml.v_2_2_0.IconStyle;
	using Kml = de.micromata.opengis.kml.v_2_2_0.Kml;
	using LabelStyle = de.micromata.opengis.kml.v_2_2_0.LabelStyle;
	using LineStyle = de.micromata.opengis.kml.v_2_2_0.LineStyle;
	using Placemark = de.micromata.opengis.kml.v_2_2_0.Placemark;
	using Point = de.micromata.opengis.kml.v_2_2_0.Point;
	using Schema = de.micromata.opengis.kml.v_2_2_0.Schema;
	using SimpleField = de.micromata.opengis.kml.v_2_2_0.SimpleField;
	using TimeStamp = de.micromata.opengis.kml.v_2_2_0.TimeStamp;
	using Track = de.micromata.opengis.kml.v_2_2_0.gx.Track;
	using IndentingXMLStreamWriter = txw2.output.IndentingXMLStreamWriter;

	/// <summary>
	/// <para>
	/// Produces KML file
	/// </para>
	/// 
	/// @author Emanuele Ziglioli - Sirtrack Ltd.
	/// </summary>

	public class JakKmlProducer : PositionConsumer
	{

	  public const string ATOMNS = "http://www.w3.org/2005/Atom";
	  public const string KMLNS = "http://www.opengis.net/kml/2.2";
	  public const string GXNS = "http://www.google.com/kml/ext/2.2";
	  public const string XALNS = "urn:oasis:names:tc:ciq:xsdschema:xAL:2.0";

	  private DecimalFormat cf = new DecimalFormat("#.00000");

	  private SimpleDateFormat timeKML = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss.SSS'Z'");

	  private string filename = null;
	  private double goodDopThreshold = 3.0;
	  private double goodEResThreshold = 10.0;
	  private string goodColorLine = "00ff00";
	  private string goodOpacity = "ff";
	  private int goodLinePixelWidth = 3;
	  private string worstColorLine = "0000ff";
	  private string worstOpacity = "ff";
	  private int worstLinePixelWidth = 3;
	  private bool debug = false;

	  private static readonly TimeZone TZ = TimeZone.getTimeZone("GMT");

	  [NonSerialized]
	  private JAXBContext jc = null;
	  [NonSerialized]
	  private Marshaller m = null;
	  internal XMLStreamWriter xmlOut;
	  internal Track track = null;
	  internal Placemark trackPlacemark = null;
	  internal const string GREEN = "FF00FF55";
	  internal const string RED = "ff6775fd";

	  internal SimpleDateFormat sdf = Observations.GMTdf;
	  internal int last = 0;

	  public AltitudeMode altitudeMode = AltitudeMode.CLAMP_TO_GROUND;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public JakKmlProducer(String filename, double goodDopTreshold) throws java.io.IOException
		public JakKmlProducer(string filename, double goodDopTreshold)
		{

		  m = createMarshaller();
		  this.filename = filename;
			this.goodDopThreshold = goodDopTreshold;
			timeKML.TimeZone = TZ;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private javax.xml.bind.JAXBContext getJaxbContext() throws javax.xml.bind.JAXBException
	  private JAXBContext JaxbContext
	  {
		  get
		  {
			if (this.jc == null)
			{
			  this.jc = JAXBContext.newInstance(new Type[] {typeof(Kml)});
			}
			return this.jc;
		  }
	  }

	  /// <summary>
	  /// Improves performance by caching contexts which are expensive to create. </summary>
	  private static readonly IDictionary<string, JAXBContext> contexts = new ConcurrentDictionary<string, JAXBContext>();

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static synchronized javax.xml.bind.JAXBContext getContext(String contextPath, ClassLoader classLoader) throws javax.xml.bind.JAXBException
	  public static JAXBContext getContext(string contextPath, ClassLoader classLoader)
	  {
		  lock (typeof(JakKmlProducer))
		  {
		  // Contexts are thread-safe so reuse those.
		  JAXBContext result = contexts[contextPath];
        
		  if (result == null)
		  {
			  result = (classLoader == null) ? JAXBContext.newInstance(contextPath) : JAXBContext.newInstance(contextPath, classLoader);
			  contexts[contextPath] = result;
		  }
        
		  return result;
		  }
	  }

	  private Marshaller createMarshaller()
	  {
		// contexPath = de.micromata.opengis.kml.v_2_2_0

		if (this.m == null)
		{
		  try
		  {
			this.m = JaxbContext.createMarshaller();
			m.setProperty(Marshaller.JAXB_FORMATTED_OUTPUT, true);
			m.setProperty(Marshaller.JAXB_FRAGMENT, true);
		  }
		  catch (JAXBException e)
		  {
			throw new Exception(e);
		  }
		}
		return this.m;
	  }

		/* (non-Javadoc)
		 * @see org.gogpsproject.producer.PositionConsumer#startOfTrack()
		 */
		public virtual XMLStreamWriter startOfTrack()
		{
			try
			{
				FileOutputStream fout = new FileOutputStream(filename);

				XMLOutputFactory f = XMLOutputFactory.newInstance();
				f.setProperty("escapeCharacters", false);
				f.setProperty(XMLOutputFactory.IS_REPAIRING_NAMESPACES, true);
				XMLStreamWriter xmlOut1 = f.createXMLStreamWriter(fout, "utf-8");
			xmlOut = new IndentingXMLStreamWriter(xmlOut1);
			xmlOut.writeStartDocument("UTF-8", "1.0");
			xmlOut.writeStartElement("kml");
			xmlOut.writeDefaultNamespace(KMLNS);
			xmlOut.writeNamespace("atom", ATOMNS);
			xmlOut.writeNamespace("gx", GXNS);
			xmlOut.writeNamespace("xal", XALNS);

			xmlOut.writeStartElement("Document");

			Icon icon = (new Icon()).withHref("http://maps.google.com/mapfiles/kml/shapes/shaded_dot.png");
			IconStyle validIconStyle = (new IconStyle()).withIcon(icon).withScale(.25).withColor(GREEN);
			IconStyle invalidIconStyle = (new IconStyle()).withIcon(icon).withScale(.25).withColor(RED);

			Icon trackIcon = (new Icon()).withHref("http://earth.google.com/images/kml-icons/track-directional/track-0.png");
			IconStyle trackIconStyle = (new IconStyle()).withIcon(trackIcon).withScale(.5).withColor(GREEN);

			LabelStyle labelStyle = (new LabelStyle()).withScale(0.7);
			LabelStyle idLabelStyle = (new LabelStyle()).withColor("00000000").withColorMode(ColorMode.NORMAL).withScale(1);
			LineStyle validLineStyle = (new LineStyle()).withColor(GREEN);
			LineStyle invalidLineStyle = (new LineStyle()).withColor(RED);

			LineStyle lineStyle_t = (new LineStyle()).withColor(GREEN).withWidth(1d);

			Schema schema = (new Schema()).withId("schema1").withSimpleField(Arrays.asList((new SimpleField()).withName("index").withType("uint"), (new SimpleField()).withName("coord").withType("string"), (new SimpleField()).withName("hDop").withType("float"), (new SimpleField()).withName("eRes").withType("float"), (new SimpleField()).withName("RTC Time").withType("string"), (new SimpleField()).withName("FIX Time").withType("string"), (new SimpleField()).withName("inUse").withType("string"), (new SimpleField()).withName("sats").withType("string")));
			m.marshal(schema, xmlOut);

	//      Style baloonStyle = new Style().withId("baloonStyle");
			string text = "\r\n<![CDATA[\r\n" + "<b>Point $[index]</b><br/><br/>" + "<b>$[coord]</b><br/><br/>" + "<b>RTC Time:</b> $[RTC Time]<br/>\r\n" + "<b>FIX TIme:</b> $[FIX Time]<br/>\r\n" + "<b>hDop:</b> $[hDop]<br/>\r\n" + "<b>eRes:</b> $[eRes]<br/>\r\n" + "<b>Sats:</b> $[inUse]<br/>\r\n" + "<table>\r\n" + "<tr align=right><th>satId</th><th>Code</th><th>Doppler</th><th>SNR</th><th>el</th><th>eRes</th><th>inUse</th></tr>" + "$[sats]<br/>\r\n" + "</table>\r\n" + "]]>\r\n";
			BalloonStyle balloonStyle = (new BalloonStyle()).withText(text);

			Folder sf = (new Folder()).withName("Styles");
			sf.Visibility = false;

			sf.createAndAddStyle().withId("Valid").withIconStyle(validIconStyle).withLabelStyle(labelStyle).withLabelStyle(idLabelStyle).withLineStyle(validLineStyle).withBalloonStyle(balloonStyle);

			sf.createAndAddStyle().withId("Invalid").withIconStyle(invalidIconStyle).withLabelStyle(labelStyle).withLabelStyle(idLabelStyle).withLineStyle(invalidLineStyle).withBalloonStyle(balloonStyle);

			sf.createAndAddStyle().withId("track").withLineStyle(lineStyle_t).withIconStyle(trackIconStyle).withLabelStyle(labelStyle).withLabelStyle(idLabelStyle);

			m.marshal(sf, xmlOut);

			xmlOut.writeStartElement("Folder");
			xmlOut.writeStartElement("name");
			xmlOut.writeCharacters("Points");
			xmlOut.writeEndElement(); // name

			xmlOut.flush();

			track = (new Track()).withAltitudeMode(altitudeMode);
			trackPlacemark = (new Placemark()).withName("Track").withStyleUrl("#track").withGeometry(track);

			return xmlOut;
			}
			catch (Exception e)
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
			  Console.WriteLine("Lon:" + cf.format(c.GeodeticLongitude) + " " + "Lat:" + cf.format(c.GeodeticLatitude) + " " + "H:" + cf.format(c.GeodeticHeight) + "\t" + "P:" + c.getpDop() + " " + "H:" + c.gethDop() + " " + "V:" + c.getvDop() + " "); //geod.get(2) -  geod.get(2) -  geod.get(1)
		  }

		  if (c.status != Status.Valid)
		  {
			return;
		  }

		  string t = timeKML.format(new DateTime(c.RefTime.Msec));
		  string name = c.obs.index + ": " + sdf.format(new DateTime(c.RefTime.Msec));

		  TimeStamp ts = new TimeStamp();
		  ts.When = t;

		  string coordStr = string.Format("{0,8:F5}, {1,8:F5}, {2,8:F5}", c.GeodeticLatitude, c.GeodeticLongitude, c.GeodeticHeight);
		  string sats = "<![CDATA[";
		  for (int i = 0; i < c.obs.NumSat; i++)
		  {
			ObservationSet os = c.obs.getSatByIdx(i);
			  sats += string.Format("<tr align=right>" + "<td>" + os.SatID + "</td>" + "<td>" + (long)(os.getCodeC(0)) + "</td>" + "<td>" + (long)(os.getDoppler(0)) + "</td>" + "<td>{0,3:F1}</td>" + "<td>{1,3:F1}</td>" + "<td>{2,4:F1}</td>" + "<td>{3}</td>" + "</tr>", float.IsNaN(os.getSignalStrength(0))?0:os.getSignalStrength(0), os.el, os.eRes, os.inUse()?'Y':'N');
		  }
		  sats += "]]>";
		  ExtendedData ed = new ExtendedData();
		  ed.Data = Arrays.asList((new Data(Convert.ToString(c.obs.index))).withName("index"), (new Data(coordStr)).withName("coord"), (new Data(sdf.format(new DateTime(c.sampleTime.Msec)))).withName("RTC Time"), (new Data(sdf.format(new DateTime(c.RefTime.Msec)))).withName("FIX Time"), (new Data(string.Format("{0,4:F1}", c.gethDop()))).withName("hDop"), (new Data(string.Format("{0,4:F1}", c.eRes))).withName("eRes"), (new Data(c.satsInUse + "/" + c.obs.NumSat)).withName("inUse"), (new Data(sats)).withName("sats"));

		  Placemark p = (new Placemark()).withName(name).withTimePrimitive(ts).withExtendedData(ed).withStyleUrl((c.getpDop() < goodDopThreshold && c.eRes < goodEResThreshold)? "#Valid" : "#Invalid");

		  Point pt = p.createAndSetPoint();
		  pt.Coordinates.add(new Coordinate(c.GeodeticLongitude, c.GeodeticLatitude, c.GeodeticHeight));
		  pt.withAltitudeMode(altitudeMode);

		  if (c.getpDop() < goodDopThreshold)
		  {
			track.addToWhen(t);
			track.addToCoord(cf.format(c.GeodeticLongitude) + " " + cf.format(c.GeodeticLatitude) + " " + cf.format(c.GeodeticHeight));
		  }

		  try
		  {
			m.marshal(p, xmlOut);
		  }
		  catch (JAXBException e)
		  {
			// TODO Auto-generated catch block
			Console.WriteLine(e.ToString());
			Console.Write(e.StackTrace);
		  }
	  }

	  /* (non-Javadoc)
		 * @see org.gogpsproject.producer.PositionConsumer#endOfTrack()
		 */
		public virtual void endOfTrack()
		{
				try
				{
			xmlOut.writeEndElement(); // dataPointFolder
			m.marshal(trackPlacemark, xmlOut);
			xmlOut.writeEndElement(); // Document
			xmlOut.writeEndElement(); // kml

					string circle = null;
	//				if(positions.size()>0){
	//					ReceiverPosition last = positions.get(positions.size()-1);
	//					circle = generateCircle(last.getGeodeticLatitude(), last.getGeodeticLongitude(), last.getGeodeticHeight(), 90, last.getpDop());
	//				}
	//				out.write("</coordinates></LineString></Placemark></Folder>"+(timeline==null?"":timeline+"</Folder>")+(circle!=null?circle:"")+"</Document>\n");
					// Close FileWriter
				}
				catch (Exception e)
				{
					Console.WriteLine(e.ToString());
					Console.Write(e.StackTrace);
				}
				finally
				{
			try
			{
			  xmlOut.flush();
			  xmlOut.close();
			}
			catch (Exception e)
			{
			  // TODO Auto-generated catch block
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
		  switch (@event)
		  {
		  case PositionConsumer_Fields.EVENT_START_OF_TRACK:
			startOfTrack();
			break;
		  case PositionConsumer_Fields.EVENT_END_OF_TRACK:
			endOfTrack();
			break;
		  }
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


		public class Coordinate : de.micromata.opengis.kml.v_2_2_0.Coordinate
		{
		  internal DecimalFormat df = new DecimalFormat("#.00000");

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public Coordinate(final double longitude, final double latitude)
		  public Coordinate(double longitude, double latitude) : base(longitude, latitude)
		  {
		  }

		  public Coordinate(double lon, double lat, double alt) : base(lon, lat, alt)
		  {
		  }

		public override string ToString()
		{
			  StringBuilder sb = new StringBuilder();
			  sb.Append(df.format(longitude));
			  sb.Append(",");
			  sb.Append(df.format(latitude));
			  if (altitude != 0.0D)
			  {
				  sb.Append(",");
				  sb.Append(df.format(altitude));
			  }
			  return sb.ToString();
		}
		}
	}

}