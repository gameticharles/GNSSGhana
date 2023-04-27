using System;

/*
 * Copyright (c) 2010, Eugenio Realini, Mirko Reguzzoni, Cryms sagl - Switzerland. All Rights Reserved.
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
 *
 */
namespace org.gogpsproject.positioning
{

	/// <summary>
	/// <para>
	/// Class for unifying time representations
	/// </para>
	/// 
	/// @author Eugenio Realini, Cryms.com
	/// </summary>
	public class Time
	{
		private long msec; // time in milliseconds since January 1, 1970 (UNIX standard)
		private double fraction; // fraction of millisecond

		private DateTime[] leapDates;
		private DateTime gc = GregorianCalendar.Instance;
		internal TimeZone zone = TimeZone.getTimeZone("GMT Time");
		internal DateFormat df = new SimpleDateFormat("yyyy MM dd HH mm ss.SSS");

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void initleapDates() throws java.text.ParseException
		internal virtual void initleapDates()
		{
			leapDates = new DateTime[19];
			leapDates[0] = df.parse("1980 01 06 00 00 00.0");
			leapDates[1] = df.parse("1981 07 01 00 00 00.0");
			leapDates[2] = df.parse("1982 07 01 00 00 00.0");
			leapDates[3] = df.parse("1983 07 01 00 00 00.0");
			leapDates[4] = df.parse("1985 07 01 00 00 00.0");
			leapDates[5] = df.parse("1988 01 01 00 00 00.0");
			leapDates[6] = df.parse("1990 01 01 00 00 00.0");
			leapDates[7] = df.parse("1991 01 01 00 00 00.0");
			leapDates[8] = df.parse("1992 07 01 00 00 00.0");
			leapDates[9] = df.parse("1993 07 01 00 00 00.0");
			leapDates[10] = df.parse("1994 07 01 00 00 00.0");
			leapDates[11] = df.parse("1996 01 01 00 00 00.0");
			leapDates[12] = df.parse("1997 07 01 00 00 00.0");
			leapDates[13] = df.parse("1999 01 01 00 00 00.0");
			leapDates[14] = df.parse("2006 01 01 00 00 00.0");
			leapDates[15] = df.parse("2009 01 01 00 00 00.0");
			leapDates[16] = df.parse("2012 07 01 00 00 00.0");
			leapDates[17] = df.parse("2015 07 01 00 00 00.0");
			leapDates[18] = df.parse("2017 01 01 00 00 00.0");
		}

		public Time(long msec)
		{
			df.TimeZone = zone;
			gc.TimeZone = zone;
			this.gc.TimeInMillis = msec;
			this.msec = msec;
			this.fraction = 0;
		}
		public Time(long msec, double fraction)
		{
			df.TimeZone = zone;
			gc.TimeZone = zone;
			this.msec = msec;
			this.gc.TimeInMillis = msec;
			this.fraction = fraction;
		}
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Time(String dateStr) throws java.text.ParseException
		public Time(string dateStr)
		{
			df.TimeZone = zone;
			gc.TimeZone = zone;
			this.msec = dateStringToTime(dateStr);
			this.gc.TimeInMillis = this.msec;
			this.fraction = 0;
		}
		public Time(int gpsWeek, double weekSec)
		{
			df.TimeZone = zone;
			gc.TimeZone = zone;
			double fullTime = (Constants.UNIX_GPS_DAYS_DIFF * Constants.SEC_IN_DAY + gpsWeek * Constants.DAYS_IN_WEEK * Constants.SEC_IN_DAY + weekSec) * 1000L;
			this.msec = (long)(fullTime);
			this.fraction = fullTime - this.msec;
			this.gc.TimeInMillis = this.msec;
		}
		/// <param name="dateStr">
		/// @return </param>
		/// <exception cref="ParseException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private long dateStringToTime(String dateStr) throws java.text.ParseException
		private long dateStringToTime(string dateStr)
		{

			long dateTime = 0;

			try
			{
				DateTime dateObj = df.parse(dateStr);
				dateTime = dateObj.Ticks;
			}
			catch (ParseException e)
			{
				throw e;
			}

			return dateTime;
		}

		/// <param name="time">
		///            (GPS time in seconds) </param>
		/// <returns> UNIX standard time in milliseconds </returns>
		private static long gpsToUnixTime(double time, int week)
		{
			// Shift from GPS time (January 6, 1980 - sec)
			// to UNIX time (January 1, 1970 - msec)
			time = (time + (week * Constants.DAYS_IN_WEEK + Constants.UNIX_GPS_DAYS_DIFF) * Constants.SEC_IN_DAY) * Constants.MILLISEC_IN_SEC;

			return (long)time;
		}

		/// <param name="time">
		///            (UNIX standard time in milliseconds) </param>
		/// <returns> GPS time in seconds </returns>
		private static double unixToGpsTime(double time)
		{
			// Shift from UNIX time (January 1, 1970 - msec)
			// to GPS time (January 6, 1980 - sec)
			time = time / Constants.MILLISEC_IN_SEC - Constants.UNIX_GPS_DAYS_DIFF * Constants.SEC_IN_DAY;
			time = time % (Constants.DAYS_IN_WEEK * Constants.SEC_IN_DAY);
			return time;
		}

		public virtual int GpsWeek
		{
			get
			{
				// Shift from UNIX time (January 1, 1970 - msec)
				// to GPS time (January 6, 1980 - sec)
				long time = msec / Constants.MILLISEC_IN_SEC - Constants.UNIX_GPS_DAYS_DIFF * Constants.SEC_IN_DAY;
				return (int)(time / (Constants.DAYS_IN_WEEK * Constants.SEC_IN_DAY));
			}
		}
		public virtual int GpsWeekSec
		{
			get
			{
				// Shift from UNIX time (January 1, 1970 - msec)
				// to GPS time (January 6, 1980 - sec)
				long time = msec / Constants.MILLISEC_IN_SEC - Constants.UNIX_GPS_DAYS_DIFF * Constants.SEC_IN_DAY;
				return (int)(time % (Constants.DAYS_IN_WEEK * Constants.SEC_IN_DAY));
			}
		}
		public virtual int GpsWeekDay
		{
			get
			{
				return (int)(GpsWeekSec / Constants.SEC_IN_DAY);
			}
		}
		public virtual int GpsHourInDay
		{
			get
			{
				long time = msec / Constants.MILLISEC_IN_SEC - Constants.UNIX_GPS_DAYS_DIFF * Constants.SEC_IN_DAY;
				return (int)((time % (Constants.SEC_IN_DAY)) / Constants.SEC_IN_HOUR);
			}
		}
		public virtual int Year
		{
			get
			{
				return gc.Year;
			}
		}
		public virtual int Year2c
		{
			get
			{
				return gc.Year - 2000;
			}
		}
		public virtual int DayOfYear
		{
			get
			{
				return gc.DayOfYear;
			}
		}
		public virtual string HourOfDayLetter
		{
			get
			{
				char c = (char)('a' + GpsHourInDay);
				return "" + c;
			}
		}

		/*
		 * Locating IGS data, products, and format definitions	Key to directory and file name variables
		 * d	day of week (0-6)
		 * ssss	4-character IGS site ID or 4-character LEO ID
		 * yyyy	4-digit year
		 * yy	2-digit year
		 * wwww	4-digit GPS week
		 * ww	2-digit week of year(01-53)
		 * ddd	day of year (1-366)
		 * hh	2-digit hour of day (00-23)
		 * h	single letter for hour of day (a-x = 0-23)
		 * mm	minutes within hour
		 *
		 */
		public virtual string formatTemplate(string template)
		{
			string tmpl = template.replaceAll("\\$\\{wwww\\}", (new DecimalFormat("0000")).format(this.GpsWeek));
			tmpl = tmpl.replaceAll("\\$\\{d\\}", (new DecimalFormat("0")).format(this.GpsWeekDay));
			tmpl = tmpl.replaceAll("\\$\\{ddd\\}", (new DecimalFormat("000")).format(this.DayOfYear));
			tmpl = tmpl.replaceAll("\\$\\{yy\\}", (new DecimalFormat("00")).format(this.Year2c));
			tmpl = tmpl.replaceAll("\\$\\{yyyy\\}", (new DecimalFormat("0000")).format(this.Year));
			int hh4 = this.GpsHourInDay;
			tmpl = tmpl.replaceAll("\\$\\{hh\\}", (new DecimalFormat("00")).format(hh4));
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
			tmpl = tmpl.replaceAll("\\$\\{hh4\\}", (new DecimalFormat("00")).format(hh4));
			tmpl = tmpl.replaceAll("\\$\\{h\\}", this.HourOfDayLetter);
			return tmpl;
		}

		public virtual double GpsTime
		{
			get
			{
				return unixToGpsTime(msec);
			}
		}

		public virtual double RoundedGpsTime
		{
			get
			{
				double tow = unixToGpsTime((msec + 499) / 1000 * 1000);
				return tow;
			}
		}

		public virtual int LeapSeconds
		{
			get
			{
				if (leapDates == null)
				{
					try
					{
						initleapDates();
					}
					catch (Exception e)
					{
						// TODO Auto-generated catch block
						Console.WriteLine(e.ToString());
						Console.Write(e.StackTrace);
					}
				}
    
				int leapSeconds = leapDates.Length - 1;
				double delta;
				for (int d = 0; d < leapDates.Length; d++)
				{
					delta = leapDates[d].Ticks - msec;
					if (delta > 0)
					{
						leapSeconds = d - 1;
						break;
					}
				}
				return leapSeconds;
			}
		}

		//
		//	private static double unixToGpsTime(double time) {
		//		// Shift from UNIX time (January 1, 1970 - msec)
		//		// to GPS time (January 6, 1980 - sec)
		//		time = (long)(time / Constants.MILLISEC_IN_SEC) - Constants.UNIX_GPS_DAYS_DIFF * Constants.SEC_IN_DAY;
		//
		//		// Remove integer weeks, to get Time Of Week
		//		double dividend  = time;
		//		double divisor = Constants.DAYS_IN_WEEK * Constants.SEC_IN_DAY;
		//		time = dividend  - (divisor * round(dividend / divisor));
		//
		//		//time = Math.IEEEremainder(time, Constants.DAYS_IN_WEEK * Constants.SEC_IN_DAY);
		//
		//		return time;
		//	}



		/// <returns> the msec </returns>
		public virtual long Msec
		{
			get
			{
				return msec;
			}
			set
			{
				this.msec = value;
			}
		}


		/// <returns> the fraction </returns>
		public virtual double Fraction
		{
			get
			{
				return fraction;
			}
			set
			{
				this.fraction = value;
			}
		}


		public virtual object clone()
		{
			return new Time(this.msec,this.fraction);
		}

		public override string ToString()
		{
			return df.format(gc.Ticks) + " " + gc.Ticks;
		}
	}

}