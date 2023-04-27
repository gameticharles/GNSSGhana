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
namespace org.gogpsproject.producer.parser.rinex
{


	using FTP = org.apache.commons.net.ftp.FTP;
	using FTPClient = org.apache.commons.net.ftp.FTPClient;
	using FTPReply = org.apache.commons.net.ftp.FTPReply;
	using EphGps = org.gogpsproject.ephemeris.EphGps;
	using SatellitePosition = org.gogpsproject.positioning.SatellitePosition;
	using Time = org.gogpsproject.positioning.Time;
	using UncompressInputStream = org.gogpsproject.util.UncompressInputStream;

	/// <summary>
	/// @author Lorenzo Patocchi, cryms.com
	/// 
	/// This class retrieve RINEX file on-demand from known server structures
	/// 
	/// </summary>
	public class RinexNavigation : NavigationProducer
	{

		public const string GARNER_NAVIGATION_AUTO = "ftp://garner.ucsd.edu/pub/nav/${yyyy}/${ddd}/auto${ddd}0.${yy}n.Z";
		public const string IGN_MULTI_NAVIGATION_DAILY = "ftp://igs.ign.fr/pub/igs/data/campaign/mgex/daily/rinex3/${yyyy}/${ddd}/brdm${ddd}0.${yy}p.Z";
		public const string GARNER_NAVIGATION_ZIM2 = "ftp://garner.ucsd.edu/pub/nav/${yyyy}/${ddd}/zim2${ddd}0.${yy}n.Z";
		public const string IGN_NAVIGATION_HOURLY_ZIM2 = "ftp://igs.ensg.ign.fr/pub/igs/data/hourly/${yyyy}/${ddd}/zim2${ddd}${h}.${yy}n.Z";
		public const string NASA_NAVIGATION_DAILY = "ftp://cddis.gsfc.nasa.gov/pub/gps/data/daily/${yyyy}/${ddd}/${yy}n/brdc${ddd}0.${yy}n.Z";
		public const string NASA_NAVIGATION_HOURLY = "ftp://cddis.gsfc.nasa.gov/pub/gps/data/hourly/${yyyy}/${ddd}/hour${ddd}0.${yy}n.Z";
	  public const string GARNER_NAVIGATION_AUTO_HTTP = "http://garner.ucsd.edu/pub/rinex/${yyyy}/${ddd}/auto${ddd}0.${yy}n.Z"; // ex http://garner.ucsd.edu/pub/rinex/2016/034/auto0340.16n.Z

		/// <summary>
		/// cache for negative answers </summary>
		private Dictionary<string, DateTime> negativeChache = new Dictionary<string, DateTime>();

		/// <summary>
		/// Folder containing downloaded files </summary>
		public string RNP_CACHE = "./rnp-cache";

		private bool waitForData = true;
		/// <param name="args"> </param>
		public static void Main(string[] args)
		{

			TimeZone.Default = TimeZone.getTimeZone("UTC");

			DateTime c = new DateTime();
			c.set(DateTime.YEAR, 2011);
			c.set(DateTime.MONTH, 0);
			c.set(DateTime.DAY_OF_MONTH, 9);
			c.set(DateTime.HOUR_OF_DAY, 1);
			c.set(DateTime.MINUTE, 0);
			c.set(DateTime.SECOND, 0);
			c.set(DateTime.MILLISECOND, 0);
			c.TimeZone = new SimpleTimeZone(0,"");

			Time t = new Time(c.TimeInMillis);

			Console.WriteLine("ts: " + t.Msec + " " + (new DateTime(t.Msec)));
			Console.WriteLine("week: " + t.GpsWeek);
			Console.WriteLine("week sec: " + t.GpsWeekSec);
			Console.WriteLine("week day: " + t.GpsWeekDay);
			Console.WriteLine("week hour in day: " + t.GpsHourInDay);


			Console.WriteLine("ts2: " + (new Time(t.GpsWeek,t.GpsWeekSec)).Msec);

			RinexNavigation rn = new RinexNavigation(IGN_NAVIGATION_HOURLY_ZIM2);
			rn.init();
	//		SatellitePosition sp = rn.getGpsSatPosition(c.getTimeInMillis(), 2, 0, 0);
			Observations obs = new Observations(new Time(c.TimeInMillis),0);
			SatellitePosition sp = rn.getGpsSatPosition(obs, 2, 'G', 0);

			if (sp != null)
			{
				Console.WriteLine("found " + (new DateTime(sp.UtcTime)) + " " + (sp.Predicted?" predicted":""));
			}
			else
			{
				Console.WriteLine("Epoch not found " + (new DateTime(c.TimeInMillis)));
			}


		}

		/// <summary>
		/// Template string where to retrieve files on the net </summary>
		private string urltemplate;
		private Dictionary<string, RinexNavigationParser> pool = new Dictionary<string, RinexNavigationParser>();

		/// <summary>
		/// Instantiates a new RINEX navigation retriever and parser.
		/// </summary>
		/// <param name="urltemplate"> the template URL where to get the files on the net. </param>
		public RinexNavigation(string urltemplate)
		{
			this.urltemplate = urltemplate;

		}

		/* (non-Javadoc)
		 * @see org.gogpsproject.NavigationProducer#getGpsSatPosition(long, int, double)
		 */
		public virtual SatellitePosition getGpsSatPosition(Observations obs, int satID, char satType, double receiverClockError)
		{

			long unixTime = obs.RefTime.Msec;
			double range = obs.getSatByIDType(satID, satType).getPseudorange(0);

			RinexNavigationParser rnp = getRNPByTimestamp(unixTime);
			if (rnp != null)
			{
				if (rnp.isTimestampInEpocsRange(unixTime))
				{
					return rnp.getGpsSatPosition(obs, satID, satType, receiverClockError);
				}
				else
				{
					return null;
				}
			}

			return null;
		}
		public virtual EphGps findEph(long unixTime, int satID, char satType)
		{
			long requestedTime = unixTime;
			EphGps eph = null;
			int maxBack = 12;
			while (eph == null && (maxBack--) > 0)
			{

				RinexNavigationParser rnp = getRNPByTimestamp(requestedTime);

				if (rnp != null)
				{
					if (rnp.isTimestampInEpocsRange(unixTime))
					{
						eph = rnp.findEph(unixTime, satID, satType);
					}
				}
		  if (eph == null)
		  {
			  requestedTime -= (1L * 3600L * 1000L);
		  }
			}

			return eph;
		}

		/* Convenience method for adding an rnp to memory cache*/
	  public virtual void put(long reqTime, RinexNavigationParser rnp)
	  {
		Time t = new Time(reqTime);
		 string url = t.formatTemplate(urltemplate);
		 if (!pool.ContainsKey(url))
		 {
		   pool[url] = rnp;
		 }
	  }

		protected internal virtual RinexNavigationParser getRNPByTimestamp(long unixTime)
		{

			RinexNavigationParser rnp = null;
			long reqTime = unixTime;

			do
			{
				// found none, retrieve from urltemplate
				Time t = new Time(reqTime);
				//System.out.println("request: "+unixTime+" "+(new Date(t.getMsec()))+" week:"+t.getGpsWeek()+" "+t.getGpsWeekDay());

				string url = t.formatTemplate(urltemplate);

		  try
		  {
			if (pool.ContainsKey(url))
			{
			  rnp = pool[url];
			}
			else
			{
			  if (url.ToLower().StartsWith("http", StringComparison.Ordinal))
			  {
				rnp = getFromHTTP(url);
			  }
			  else if (url.ToLower().StartsWith("ftp", StringComparison.Ordinal))
			  {
				rnp = getFromFTP(url);
			  }
			  else
			  {
				throw new Exception("Invalid url template " + url);
			  }

			  if (rnp != null)
			  {
				pool[url] = rnp;
			  }
			}
			return rnp;
		  }
		  catch (IOException e)
		  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					  Console.WriteLine(e.GetType().FullName + " url: " + url);
					  return null;
		  }

			} while (waitForData && rnp == null);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private org.gogpsproject.producer.parser.rinex.RinexNavigationParser getFromFTP(String url) throws java.io.IOException
		private RinexNavigationParser getFromFTP(string url)
		{
			RinexNavigationParser rnp = null;

			string origurl = url;
			if (negativeChache.ContainsKey(url))
			{
				if (DateTimeHelperClass.CurrentUnixTimeMillis() - negativeChache[url].Ticks < 60 * 60 * 1000)
				{
					throw new FileNotFoundException("cached answer");
				}
				else
				{
					negativeChache.Remove(url);
				}
			}

			string filename = url.replaceAll("[ ,/:]", "_");
			if (filename.EndsWith(".Z", StringComparison.Ordinal))
			{
				filename = filename.Substring(0, filename.Length - 2);
			}
			File rnf = new File(RNP_CACHE,filename);

			if (rnf.exists())
			{
		  Console.WriteLine(url + " from cache file " + rnf);
		  rnp = new RinexNavigationParser(rnf);
		  try
		  {
			rnp.init();
			return rnp;
		  }
		  catch (Exception)
		  {
			rnf.delete();
		  }
			}

			// if the file doesn't exist of is invalid
			Console.WriteLine(url + " from the net.");
			FTPClient ftp = new FTPClient();

			try
			{
				int reply;
				Console.WriteLine("URL: " + url);
				url = url.Substring("ftp://".Length);
				string server = url.Substring(0, url.IndexOf('/'));
				string remoteFile = url.Substring(url.IndexOf('/'));
				string remotePath = remoteFile.Substring(0,remoteFile.LastIndexOf('/'));
				remoteFile = remoteFile.Substring(remoteFile.LastIndexOf('/') + 1);

				ftp.connect(server);
				ftp.login("anonymous", "info@eriadne.org");

				Console.Write(ftp.ReplyString);

				// After connection attempt, you should check the reply code to
				// verify
				// success.
				reply = ftp.ReplyCode;

				if (!FTPReply.isPositiveCompletion(reply))
				{
					ftp.disconnect();
					Console.Error.WriteLine("FTP server refused connection.");
					return null;
				}

				ftp.enterLocalPassiveMode();
				ftp.RemoteVerificationEnabled = false;

				Console.WriteLine("cwd to " + remotePath + " " + ftp.changeWorkingDirectory(remotePath));
				Console.WriteLine(ftp.ReplyString);
				ftp.FileType = FTP.BINARY_FILE_TYPE;
				Console.WriteLine(ftp.ReplyString);

				Console.WriteLine("open " + remoteFile);
				InputStream @is = ftp.retrieveFileStream(remoteFile);
				Console.WriteLine(ftp.ReplyString);
				if (ftp.ReplyString.StartsWith("550", StringComparison.Ordinal))
				{
					negativeChache[origurl] = DateTime.Now;
					throw new FileNotFoundException();
				}
		  InputStream uis = @is;

				if (remoteFile.EndsWith(".Z", StringComparison.Ordinal))
				{
					uis = new UncompressInputStream(@is);
				}

				rnp = new RinexNavigationParser(uis,rnf);
				rnp.init();
				@is.close();


				ftp.completePendingCommand();

				ftp.logout();
			}
			finally
			{
				if (ftp.Connected)
				{
					try
					{
						ftp.disconnect();
					}
					catch (IOException)
					{
						// do nothing
					}
				}
			}
			return rnp;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private org.gogpsproject.producer.parser.rinex.RinexNavigationParser getFromHTTP(String tUrl) throws java.io.IOException
	  private RinexNavigationParser getFromHTTP(string tUrl)
	  {
		RinexNavigationParser rnp = null;

		string origurl = tUrl;
		if (negativeChache.ContainsKey(tUrl))
		{
		  if (DateTimeHelperClass.CurrentUnixTimeMillis() - negativeChache[tUrl].Ticks < 60 * 60 * 1000)
		  {
			throw new FileNotFoundException("cached answer");
		  }
		  else
		  {
			negativeChache.Remove(tUrl);
		  }
		}

		string filename = tUrl.replaceAll("[ ,/:]", "_");
		if (filename.EndsWith(".Z", StringComparison.Ordinal))
		{
			filename = filename.Substring(0, filename.Length - 2);
		}
		File rnf = new File(RNP_CACHE,filename);

		if (rnf.exists())
		{
		  Console.WriteLine(tUrl + " from cache file " + rnf);
		  rnp = new RinexNavigationParser(rnf);
		  rnp.init();
		}
		else
		{
		  Console.WriteLine(tUrl + " from the net.");

		  Console.WriteLine("URL: " + tUrl);
		  tUrl = tUrl.Substring("http://".Length);
		  string remoteFile = tUrl.Substring(tUrl.IndexOf('/'));
		  remoteFile = remoteFile.Substring(remoteFile.LastIndexOf('/') + 1);

		  URL url = new URL("http://" + tUrl);
		  HttpURLConnection con = (HttpURLConnection) url.openConnection();
		  con.RequestMethod = "GET";
	//      con.setRequestProperty("Authorization", "Basic "+ new String(Base64.encode(new String("anonymous:info@eriadne.org"))));
		  con.setRequestProperty("Authorization", "Basic " + new string(Base64.Encoder.encode((("anonymous:info@eriadne.org").GetBytes()))));

		  int reply = con.ResponseCode;

		  if (reply > 200)
		  {
			if (reply == 404)
			{
			  Console.Error.WriteLine("404 Not Found");
			}
			else
			{
			  Console.Error.WriteLine("HTTP server refused connection.");
			}
	//        System.out.print(new String(res.getContent()));

			return null;
		  }

		  try
		  {
			if (remoteFile.EndsWith(".Z", StringComparison.Ordinal))
			{
			  try
			  {
	//            InputStream is = new ByteArrayInputStream(res.getContent());
				InputStream @is = con.InputStream;
				InputStream uis = new UncompressInputStream(@is);
				rnp = new RinexNavigationParser(uis,rnf);
				rnp.init();
				uis.close();
			  }
			  catch (IOException)
			  {
				InputStream @is = con.InputStream;
				InputStream uis = new GZIPInputStream(@is);
				rnp = new RinexNavigationParser(uis,rnf);
				rnp.init();
	  //        Reader decoder = new InputStreamReader(gzipStream, encoding);
	  //        BufferedReader buffered = new BufferedReader(decoder);
				uis.close();
			  }
			}
			else
			{
			  InputStream @is = con.InputStream;
			  rnp = new RinexNavigationParser(@is,rnf);
			  rnp.init();
			  @is.close();
			}
		  }
		  catch (IOException e)
		  {
			Console.WriteLine(e.ToString());
			Console.Write(e.StackTrace);
			// TODO delete file, maybe it's corrupt
		  }
		}
		return rnp;
	  }


		/* (non-Javadoc)
		 * @see org.gogpsproject.NavigationProducer#getIono(int)
		 */
		public virtual IonoGps getIono(long unixTime)
		{
			RinexNavigationParser rnp = getRNPByTimestamp(unixTime);
			if (rnp != null)
			{
				return rnp.getIono(unixTime);
			}
			return null;
		}

		/* (non-Javadoc)
		 * @see org.gogpsproject.NavigationProducer#init()
		 */
		public virtual void init()
		{

		}

		/* (non-Javadoc)
		 * @see org.gogpsproject.NavigationProducer#release()
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void release(boolean waitForThread, long timeoutMs) throws InterruptedException
		public virtual void release(bool waitForThread, long timeoutMs)
		{
			waitForData = false;
		}


	}

}