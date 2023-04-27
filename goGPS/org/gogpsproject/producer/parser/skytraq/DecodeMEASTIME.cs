using System;

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

namespace org.gogpsproject.producer.parser.skytraq
{


	using Time = org.gogpsproject.positioning.Time;
	using Bits = org.gogpsproject.util.Bits;


	public class DecodeMEASTIME
	{
		private InputStream @in;

		public DecodeMEASTIME(InputStream @in)
		{
			this.@in = @in;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.gogpsproject.producer.Observations decode(int len) throws java.io.IOException, STQException
		public virtual Observations decode(int len)
		{

			sbyte[] bytes;

			/* IOD, 1 byte */				
			bytes = new sbyte[1];
			@in.read(bytes, 0, bytes.Length);
			int IOD = Bits.byteToIntBigEndian(bytes);

			/* GPS week, 2 bytes */
			bytes = new sbyte[2];
			@in.read(bytes, 0, bytes.Length);
			int week = Bits.byteToIntBigEndian(bytes);

			/* GPS time-of-week, 4 bytes */
			bytes = new sbyte[4];
			@in.read(bytes, 0, bytes.Length);
			long tow = Bits.byteToLongBigEndian(bytes);

			/* Measurement period, 2 bytes */
			bytes = new sbyte[2];
			@in.read(bytes, 0, bytes.Length);

			long gmtTS = getGMTTS(tow, week);
			Observations o = new Observations(new Time(gmtTS),0);
			o.IssueOfData = IOD;

			return o;
		}

		private long getGMTTS(long tow, long week)
		{
			DateTime c = new DateTime();
			c.TimeZone = TimeZone.getTimeZone("GMT Time");
			c.set(DateTime.YEAR, 1980);
			c.set(DateTime.MONTH, 1);
			c.set(DateTime.DAY_OF_MONTH, 6);
			c.set(DateTime.HOUR_OF_DAY, 0);
			c.set(DateTime.MINUTE, 0);
			c.set(DateTime.SECOND, 0);
			c.set(DateTime.MILLISECOND, 0);

			return c.TimeInMillis + week * 7 * 24 * 3600 * 1000 + tow;
		}
	}

}