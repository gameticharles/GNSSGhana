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
namespace org.gogpsproject.producer.parser.sp3
{


	using FTP = org.apache.commons.net.ftp.FTP;
	using FTPClient = org.apache.commons.net.ftp.FTPClient;
	using FTPReply = org.apache.commons.net.ftp.FTPReply;
	using Coordinates = org.gogpsproject.positioning.Coordinates;
	using SatellitePosition = org.gogpsproject.positioning.SatellitePosition;
	using Time = org.gogpsproject.positioning.Time;
	using UncompressInputStream = org.gogpsproject.util.UncompressInputStream;

	/// <summary>
	/// @author Lorenzo Patocchi, cryms.com
	/// 
	/// Still incomplete
	/// </summary>
	public class SP3NavigationSpeed : SP3Navigation
	{

		public SP3NavigationSpeed(string urltemplate) : base(urltemplate)
		{
		}

	  protected internal virtual SP3Parser getSP3ByTimestamp(long unixTime)
	  {
		SP3Parser sp3p = null;
		long reqTime = unixTime;

		do
		{
		  // found none, retrieve from urltemplate
		  Time t = new Time(reqTime);
		  //System.out.print("request: "+unixTime+" "+(new Date(t.getMsec()))+" week:"+t.getGpsWeek()+" "+t.getGpsWeekDay());

		  string url = urltemplate.replaceAll("\\$\\{wwww\\}", (new DecimalFormat("0000")).format(t.GpsWeek));
		  url = url.replaceAll("\\$\\{d\\}", (new DecimalFormat("0")).format(t.GpsWeekDay));
		  int hh4 = t.GpsHourInDay;
		  if (0 <= hh4 && hh4 < 6)
		  {
			  hh4 = 0;
		  }
		  if (6 <= hh4 && hh4 < 12)
		  {
			  hh4 = 6;
		  }
		  if (12 <= hh4 && hh4 < 18)
		  {
			  hh4 = 12;
		  }
		  if (18 <= hh4 && hh4 < 24)
		  {
			  hh4 = 18;
		  }
		  url = url.replaceAll("\\$\\{hh4\\}", (new DecimalFormat("00")).format(hh4));

		  if (!url.StartsWith("ftp://", StringComparison.Ordinal))
		  {
			  throw new Exception("Invalid url template " + url);
		  }

		  try
		  {
			if (pool.ContainsKey(url))
			{
			  //System.out.println(url+" from memory cache.");
			  sp3p = pool[url];
			}
			else
			{

			  sp3p = getFromFTP(url);
			}

			if (sp3p != null)
			{
			  pool[url] = sp3p;
			  // file exist, look for epoch
			  if (sp3p.isTimestampInEpocsRange(unixTime))
			  {
				return sp3p;
			  }
			  else
			  {
				return null;
			  }
			}
		  }
		 catch (Exception e)
		 {
			  Console.WriteLine(e.ToString());
			  Console.Write(e.StackTrace);
			  return null;
		 }
		} while (sp3p == null);

	   return null;
	  }


		/* (non-Javadoc)
		 * @see org.gogpsproject.NavigationProducer#getGpsSatPosition(long, int, double)
		 */
		public override SatellitePosition getGpsSatPosition(Observations obs, int satID, char satType, double receiverClockError)
		{
			long unixTime = obs.RefTime.Msec;
		long reqTime = unixTime + 2 * 3600L * 1000L;
			SP3Parser sp3p = null;
		int maxBack = 3;
		while (sp3p == null && (maxBack--) > 0)
		{
		  sp3p = getSP3ByTimestamp(reqTime);

		  if (sp3p != null)
		  {
	//        obs.rinexFileName = sp3p.getFileName();
	//        return sp3p.getGpsSatPosition( obs, satID, satType, receiverClockError );
			SatellitePosition sp = sp3p.getGpsSatPosition(obs, satID, satType, receiverClockError);
			return sp;
		  }
		  else
		  {
			  reqTime -= (2L * 3600L * 1000L);
		  }
		};

			return null;
		}
	}

}