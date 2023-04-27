using System;
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

	/// <summary>
	/// <para>
	/// Produces TXT file
	/// </para>
	/// 
	/// @author Eugenio Realini
	/// </summary>

	public class CsvProducer : PositionConsumer
	{

	  private SimpleDateFormat dateTXT = new SimpleDateFormat("yyyy/MM/dd");
	  private SimpleDateFormat timeTXT = new SimpleDateFormat("HH:mm:ss");

	  private string filename = null;
	  private bool debug = false;

	  internal StreamWriter @out;
	  internal int index = 0;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public CsvProducer(String filename) throws java.io.IOException
	  public CsvProducer(string filename)
	  {
		TimeZone gmttz = TimeZone.getTimeZone("GMT");
		dateTXT.TimeZone = gmttz;
		timeTXT.TimeZone = gmttz;
		this.filename = filename;

		writeHeader();
	  }

	  /* (non-Javadoc)
	   * @see org.gogpsproject.producer.PositionConsumer#addCoordinate(org.gogpsproject.Coordinates)
	   */
	  public virtual void addCoordinate(RoverPosition coord)
	  {
		if (debug) // geod.get(0)
		{
			Console.WriteLine("Lon:" + coord.GeodeticLongitude.ToString("0.00000000") + " " + "Lat:" + coord.GeodeticLatitude.ToString("0.00000000") + " " + "H:" + coord.GeodeticHeight.ToString("0.000") + "\t" + "P:" + coord.getpDop() + " " + "H:" + coord.gethDop() + " " + "V:" + coord.getvDop() + " "); //geod.get(2) -  geod.get(2) -  geod.get(1)
		}

		writeCoordinate(index, coord, @out);
		index++;
	  }

	  /* (non-Javadoc)
	   * @see org.gogpsproject.producer.PositionConsumer#addCoordinate(org.gogpsproject.Coordinates)
	*/
	    public virtual void writeCoordinate(int index, RoverPosition c, StreamWriter @out)
	    {
		    try
		    {
	            //      out.write( "Index,Status,Date,UTC,Latitude [DD], Longitude [DD],HDOP,SVs in Use,SVs in View,SNR Avg [dB],Residual Error,Clock Error,Clock Error Total,\r\n" );
		        // PrintWriter pw = new PrintWriter(@out);
                StringBuilder sb = new StringBuilder();

                sb.append(string.Format("{0}d,{1}s,", c.obs.index, c.status.ToString()));

		        //date, time
		        string d =  c.RefTime.Msec.;
		        string t = timeTXT.format(new DateTime(c.RefTime.Msec));
                sb.append(string.Format("{0} {1},", d, t));

		        double lat = c.GeodeticLatitude;
		        double lon = c.GeodeticLongitude;
	            double hEllips = c.GeodeticHeight;

		        if (c.status == Status.Valid)
		        {
                    sb.append(string.Format("{},{},{},", lat.ToString("0.000000"), lon.ToString("0.000000"), c.getpDop().ToString("0.0")));
		        }
		        else
		        {
                    sb.append(string.Format("0,0,0,"));
		        }
            
                sb.append(string.Format("%d,%d,", c.satsInUse, c.obs.NumSat));
                sb.append(string.Format("%3.1f,%4.3f,\r\n", c.eRes, c.cErrMS / 1000.0));

                @out.Write(sb);
		        @out.Flush();
                
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
	  public virtual StreamWriter writeHeader()
	  {
		try
		{
		  StreamWriter @out = new StreamWriter(filename);

		  @out.WriteLine("Index,Status,Date,UTC,Latitude [DD], Longitude [DD],HDOP,SVs in Use,SVs in View,SNR Avg [dB],Residual Error,Clock Error,\r\n");

		  @out.Flush();
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
	//    }
		switch (@event)
		{
		  case PositionConsumer_Fields.EVENT_START_OF_TRACK:
	//      startOfTrack();
			@out = writeHeader();
			break;
		  case PositionConsumer_Fields.EVENT_END_OF_TRACK:
			break;
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