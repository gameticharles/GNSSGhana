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
	public class SP3Navigation : NavigationProducer
	{

		public const string IGN_FR_ULTRARAPID = "ftp://igs.ensg.ign.fr/pub/igs/products/${wwww}/igu${wwww}${d}_${hh4}.sp3.Z";
		public const string IGN_FR_RAPID = "ftp://igs.ensg.ign.fr/pub/igs/products/${wwww}/igr${wwww}${d}.sp3.Z";
		public const string IGN_FR_FINAL = "ftp://igs.ensg.ign.fr/pub/igs/products/${wwww}/igs${wwww}${d}.sp3.Z";

		public string SP3_CACHE = "./sp3-cache";

		private bool waitForData = true;
		protected internal string urltemplate;
		protected internal Dictionary<string, SP3Parser> pool = new Dictionary<string, SP3Parser>();

		public SP3Navigation(string urltemplate)
		{
			this.urltemplate = urltemplate;

		}

		/* (non-Javadoc)
		 * @see org.gogpsproject.NavigationProducer#getGpsSatPosition(long, int, double)
		 */
		public virtual SatellitePosition getGpsSatPosition(Observations obs, int satID, char satType, double receiverClockError)
		{

			long unixTime = obs.RefTime.Msec;

			SP3Parser sp3p = null;
			long reqTime = unixTime;

			bool retrievable = true;

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



				if (url.StartsWith("ftp://", StringComparison.Ordinal))
				{
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
								return sp3p.getGpsSatPosition(obs, satID, satType, receiverClockError);
							}
							else
							{
								return null;
							}
						}
					}
					catch (IOException e)
					{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			  Console.WriteLine(e.GetType().FullName + " url: " + url);
			  return null;
					}


				}
				else
				{
					retrievable = false;
				}
			}while (retrievable && waitForData && sp3p == null);

			return null;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected SP3Parser getFromFTP(String url) throws java.io.IOException
		protected internal virtual SP3Parser getFromFTP(string url)
		{
			SP3Parser sp3p = null;

			string filename = url.replaceAll("[ ,/:]", "_");
			if (filename.EndsWith(".Z", StringComparison.Ordinal))
			{
				filename = filename.Substring(0, filename.Length - 2);
			}
			File sp3f = new File(SP3_CACHE,filename);

			if (sp3f.exists())
			{
		  Console.WriteLine(url + " from cache file " + sp3f);
		  sp3p = new SP3Parser(sp3f);
		  sp3p.init();
			}
		else
		{
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
					InputStream uis = @is;
					Console.WriteLine(ftp.ReplyString);
					if (ftp.ReplyString.StartsWith("550", StringComparison.Ordinal))
					{
						throw new FileNotFoundException();
					}

					if (remoteFile.EndsWith(".Z", StringComparison.Ordinal))
					{
						uis = new UncompressInputStream(@is);
					}

					sp3p = new SP3Parser(uis,sp3f);
					sp3p.init();
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
		}
			return sp3p;
		}

		/* (non-Javadoc)
		 * @see org.gogpsproject.NavigationProducer#getIono(int)
		 */
		public virtual IonoGps getIono(long unixTime)
		{
			return null; //iono[i];
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