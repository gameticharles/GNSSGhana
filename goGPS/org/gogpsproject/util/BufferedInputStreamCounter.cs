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

using java.io;
using java.lang;
using java.util;
using net.sf.jni4net;
using net.sf.jni4net.adaptors;
using System;

namespace org.gogpsproject.util
{

	/// <summary>
	/// <para>
	/// This class wrap around InputStream and counts the read bytes
	/// </para>
	/// 
	/// @author Lorenzo Patocchi cryms.com
	/// </summary>
	public class BufferedInputStreamCounter : BufferedInputStream
	{

		private BufferedInputStream @is;
		private OutputStream os;

		private long counter = 0;

		private long markCount;
		private long markTS;

		/// 
		public BufferedInputStreamCounter(InputStream @is, OutputStream os) : base(@is)
		{
			this.@is = new BufferedInputStream(@is);
			this.os = os;
		}

		/* (non-Javadoc)
		 * @see java.io.InputStream#read()
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public int read() throws java.io.IOException
		public override int read()
		{
			int c = @is.read();
			//if(c>=0){
				//System.out.println("*");
				counter++;
			//}
			if (os != null && c >= 0)
			{
				os.write(c);
			}
			return c;
		}

		/* (non-Javadoc)
		 * @see java.io.InputStream#available()
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public int available() throws java.io.IOException
		public override int available()
		{
			return @is.available();
		}

		/* (non-Javadoc)
		 * @see java.io.InputStream#close()
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void close() throws java.io.IOException
		public override void close()
		{
			@is.close();
		}

		/* (non-Javadoc)
		 * @see java.io.InputStream#mark(int)
		 */
		public override void mark(int readlimit)
		{
			lock (this)
			{
				@is.mark(readlimit);
			}
		}

		/* (non-Javadoc)
		 * @see java.io.InputStream#markSupported()
		 */
		public override bool markSupported()
		{
			return @is.markSupported();
		}

		/* (non-Javadoc)
		 * @see java.io.InputStream#read(byte[], int, int)
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public int read(byte[] b, int off, int len) throws java.io.IOException
		public override int read(sbyte[] b, int off, int len)
		{
			int c = @is.read(b, off, len);
			if (c > 0)
			{
				counter += c;
			}
			if (os != null && c > 0)
			{
				os.write(b, off, c);
			}
			//System.out.println(""+c);

			return c;
		}

		/* (non-Javadoc)
		 * @see java.io.InputStream#read(byte[])
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public int read(byte[] b) throws java.io.IOException
		public override int read(sbyte[] b)
		{
			int c = @is.read(b);
			if (c > 0)
			{
				counter += c;
			}
			if (os != null && c > 0)
			{
				os.write(b, 0, c);
			}
			//System.out.println(""+c);

			return c;
		}

		/* (non-Javadoc)
		 * @see java.io.InputStream#reset()
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public synchronized void reset() throws java.io.IOException
		public override void reset()
		{
			lock (this)
			{
				@is.reset();
			}
		}

		/* (non-Javadoc)
		 * @see java.io.InputStream#skip(long)
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public long skip(long n) throws java.io.IOException
		public override long skip(long n)
		{
			//counter += n;
			//System.out.println(""+n);

			return @is.skip(n);
		}

		/// <returns> the counter </returns>
		public virtual long Counter
		{
			get
			{
				return counter;
			}
			set
			{
				this.counter = value;
			}
		}


		public virtual long start()
		{
			markCount = counter;
			markTS = DateTimeHelperClass.CurrentUnixTimeMillis();
			return markCount;
		}

		public virtual int CurrentBps
		{
			get
			{
				try
				{
				return (int)((counter - markCount) / ((DateTimeHelperClass.CurrentUnixTimeMillis() - markTS) / 1000L));
				}
				catch (ArithmeticException)
				{
					return 0;
				}
			}
		}

		public virtual long stop()
		{
			markCount = counter;
			markTS = DateTimeHelperClass.CurrentUnixTimeMillis();
			return markCount;
		}

	}

}