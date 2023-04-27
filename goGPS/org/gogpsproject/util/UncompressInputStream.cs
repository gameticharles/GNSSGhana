using System;

/*
 * @(#)UncompressInputStream.java			0.3-3 06/05/2001
 *
 *  This file is part of the HTTPClient package
 *  Copyright (C) 1996-2001 Ronald Tschal�r
 *
 *  This library is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public
 *  License as published by the Free Software Foundation; either
 *  version 2 of the License, or (at your option) any later version.
 *
 *  This library is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 *  Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public
 *  License along with this library; if not, write to the Free
 *  Software Foundation, Inc., 59 Temple Place, Suite 330, Boston,
 *  MA 02111-1307, USA
 *
 *  For questions, suggestions, bug-reports, enhancement-requests etc.
 *  I may be contacted at:
 *
 *  ronald@innovation.ch
 *
 *  The HTTPClient's home page is located at:
 *
 *  http://www.innovation.ch/java/HTTPClient/
 *
 *  copied into org.gogpsproject.parser.sp3 to make it usable (int HTTPClient package is private)
 */

namespace org.gogpsproject.util
{


	/// <summary>
	/// This class decompresses an input stream containing data compressed with the
	/// unix "compress" utility (LZC, a LZW variant). This code is based heavily on
	/// the <var>unlzw.c</var> code in <var>gzip-1.2.4</var> (written by Peter
	/// Jannesen) and the original compress code.
	/// 
	/// @version 0.3-3 06/05/2001
	/// @author Ronald Tschal�r
	/// </summary>
	public class UncompressInputStream : FilterInputStream
	{
	  /// <param name="is">
	  ///          the input stream to decompress </param>
	  /// <exception cref="IOException">
	  ///              if the header is malformed </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public UncompressInputStream(java.io.InputStream is) throws java.io.IOException
	  public UncompressInputStream(InputStream @is) : base(@is)
	  {
		parse_header();
	  }

	  internal sbyte[] one = new sbyte[1];

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public synchronized int read() throws java.io.IOException
	  public virtual int read()
	  {
		  lock (this)
		  {
			int b = @in.read(one, 0, 1);
			if (b == 1)
			{
			  return (one[0] & 0xff);
			}
			else
			{
			  return -1;
			}
		  }
	  }

	  // string table stuff
	  private const int TBL_CLEAR = 0x100;
	  private static readonly int TBL_FIRST = TBL_CLEAR + 1;

	  private int[] tab_prefix;
	  private sbyte[] tab_suffix;
	  private int[] zeros = new int[256];
	  private sbyte[] stack;

	  // various state
	  private bool block_mode;
	  private int n_bits;
	  private int maxbits;
	  private int maxmaxcode;
	  private int maxcode;
	  private int bitmask;
	  private int oldcode;
	  private sbyte finchar;
	  private int stackp;
	  private int free_ent;

	  // input buffer
	  private sbyte[] data = new sbyte[10000];
	  private int bit_pos = 0, end = 0, got = 0;
	  private bool eof = false;
	  private const int EXTRA = 64;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public synchronized int read(byte[] buf, int off, int len) throws java.io.IOException
	  public virtual int read(sbyte[] buf, int off, int len)
	  {
		  lock (this)
		  {
			if (eof)
			{
			  return -1;
			}
			int start = off;
        
			/*
			 * Using local copies of various variables speeds things up by as much as
			 * 30% !
			 */
			int[] l_tab_prefix = tab_prefix;
			sbyte[] l_tab_suffix = tab_suffix;
			sbyte[] l_stack = stack;
			int l_n_bits = n_bits;
			int l_maxcode = maxcode;
			int l_maxmaxcode = maxmaxcode;
			int l_bitmask = bitmask;
			int l_oldcode = oldcode;
			sbyte l_finchar = finchar;
			int l_stackp = stackp;
			int l_free_ent = free_ent;
			sbyte[] l_data = data;
			int l_bit_pos = bit_pos;
        
			// empty stack if stuff still left
        
			int s_size = l_stack.Length - l_stackp;
			if (s_size > 0)
			{
			  int num = (s_size >= len) ? len : s_size;
			  Array.Copy(l_stack, l_stackp, buf, off, num);
			  off += num;
			  len -= num;
			  l_stackp += num;
			}
        
			if (len == 0)
			{
			  stackp = l_stackp;
			  return off - start;
			}
        
			// loop, filling local buffer until enough data has been decompressed
        
			do
			{
			  if (end < EXTRA)
			  {
				fill();
			  }
        
			  int bit_in = (got > 0) ? (end - end % l_n_bits) << 3 : (end << 3) - (l_n_bits - 1);
        
			  while (l_bit_pos < bit_in)
			  {
				// check for code-width expansion
        
				if (l_free_ent > l_maxcode)
				{
				  int n_bytes = l_n_bits << 3;
				  l_bit_pos = (l_bit_pos - 1) + n_bytes - (l_bit_pos - 1 + n_bytes) % n_bytes;
        
				  l_n_bits++;
				  l_maxcode = (l_n_bits == maxbits) ? l_maxmaxcode : (1 << l_n_bits) - 1;
        
				  if (debug)
				  {
					Console.Error.WriteLine("Code-width expanded to " + l_n_bits);
				  }
        
				  l_bitmask = (1 << l_n_bits) - 1;
				  l_bit_pos = resetbuf(l_bit_pos);
				  goto main_loopContinue;
				}
        
				// read next code
        
				int pos = l_bit_pos >> 3;
				int code = (((l_data[pos] & 0xFF) | ((l_data[pos + 1] & 0xFF) << 8) | ((l_data[pos + 2] & 0xFF) << 16)) >> (l_bit_pos & 0x7)) & l_bitmask;
				l_bit_pos += l_n_bits;
        
				// handle first iteration
        
				if (l_oldcode == -1)
				{
				  if (code >= 256)
				  {
					throw new IOException("corrupt input: " + code + " > 255");
				  }
				  l_finchar = (sbyte)(l_oldcode = code);
				  buf[off++] = l_finchar;
				  len--;
				  continue;
				}
        
				// handle CLEAR code
        
				if (code == TBL_CLEAR && block_mode)
				{
				  Array.Copy(zeros, 0, l_tab_prefix, 0, zeros.Length);
				  l_free_ent = TBL_FIRST - 1;
        
				  int n_bytes = l_n_bits << 3;
				  l_bit_pos = (l_bit_pos - 1) + n_bytes - (l_bit_pos - 1 + n_bytes) % n_bytes;
				  l_n_bits = INIT_BITS;
				  l_maxcode = (1 << l_n_bits) - 1;
				  l_bitmask = l_maxcode;
        
				  if (debug)
				  {
					Console.Error.WriteLine("Code tables reset");
				  }
        
				  l_bit_pos = resetbuf(l_bit_pos);
				  goto main_loopContinue;
				}
        
				// setup
        
				int incode = code;
				l_stackp = l_stack.Length;
        
				// Handle KwK case
        
				if (code >= l_free_ent)
				{
				  if (code > l_free_ent)
				  {
					throw new IOException("corrupt input: code=" + code + ", free_ent=" + l_free_ent);
				  }
        
				  l_stack[--l_stackp] = l_finchar;
				  code = l_oldcode;
				}
        
				// Generate output characters in reverse order
        
				while (code >= 256)
				{
				  l_stack[--l_stackp] = l_tab_suffix[code];
				  code = l_tab_prefix[code];
				}
				l_finchar = l_tab_suffix[code];
				buf[off++] = l_finchar;
				len--;
        
				// And put them out in forward order
        
				s_size = l_stack.Length - l_stackp;
				int num = (s_size >= len) ? len : s_size;
				Array.Copy(l_stack, l_stackp, buf, off, num);
				off += num;
				len -= num;
				l_stackp += num;
        
				// generate new entry in table
        
				if (l_free_ent < l_maxmaxcode)
				{
				  l_tab_prefix[l_free_ent] = l_oldcode;
				  l_tab_suffix[l_free_ent] = l_finchar;
				  l_free_ent++;
				}
        
				// Remember previous code
        
				l_oldcode = incode;
        
				// if output buffer full, then return
        
				if (len == 0)
				{
				  n_bits = l_n_bits;
				  maxcode = l_maxcode;
				  bitmask = l_bitmask;
				  oldcode = l_oldcode;
				  finchar = l_finchar;
				  stackp = l_stackp;
				  free_ent = l_free_ent;
				  bit_pos = l_bit_pos;
        
				  return off - start;
				}
			  }
        
			  l_bit_pos = resetbuf(l_bit_pos);
			} while (got > 0);
				main_loopContinue:;
			main_loopBreak:
        
			n_bits = l_n_bits;
			maxcode = l_maxcode;
			bitmask = l_bitmask;
			oldcode = l_oldcode;
			finchar = l_finchar;
			stackp = l_stackp;
			free_ent = l_free_ent;
			bit_pos = l_bit_pos;
        
			eof = true;
			return off - start;
		  }
	  }

	  /// <summary>
	  /// Moves the unread data in the buffer to the beginning and resets the
	  /// pointers.
	  /// </summary>
	  private int resetbuf(int bit_pos)
	  {
		int pos = bit_pos >> 3;
		Array.Copy(data, pos, data, 0, end - pos);
		end -= pos;
		return 0;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private final void fill() throws java.io.IOException
	  private void fill()
	  {
		got = @in.read(data, end, data.Length - 1 - end);
		if (got > 0)
		{
		  end += got;
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public synchronized long skip(long num) throws java.io.IOException
	  public virtual long skip(long num)
	  {
		  lock (this)
		  {
			sbyte[] tmp = new sbyte[(int) num];
			int got = read(tmp, 0, (int) num);
        
			if (got > 0)
			{
			  return (long) got;
			}
			else
			{
			  return 0L;
			}
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public synchronized int available() throws java.io.IOException
	  public virtual int available()
	  {
		  lock (this)
		  {
			if (eof)
			{
			  return 0;
			}
        
			return @in.available();
		  }
	  }

	  private const int LZW_MAGIC = 0x1f9d;
	  private const int MAX_BITS = 16;
	  private const int INIT_BITS = 9;
	  private const int HDR_MAXBITS = 0x1f;
	  private const int HDR_EXTENDED = 0x20;
	  private const int HDR_FREE = 0x40;
	  private const int HDR_BLOCK_MODE = 0x80;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private void parse_header() throws java.io.IOException
	  private void parse_header()
	  {
		// read in and check magic number

		int t = @in.read();
		if (t < 0)
		{
		  throw new EOFException("Failed to read magic number");
		}
		int magic = (t & 0xff) << 8;
		t = @in.read();
		if (t < 0)
		{
		  throw new EOFException("Failed to read magic number");
		}
		magic += t & 0xff;
		if (magic != LZW_MAGIC)
		{
		  throw new IOException("Input not in compress format (read " + "magic number 0x" + magic.ToString("x") + ")");
		}

		// read in header byte

		int header = @in.read();
		if (header < 0)
		{
		  throw new EOFException("Failed to read header");
		}

		block_mode = (header & HDR_BLOCK_MODE) > 0;
		maxbits = header & HDR_MAXBITS;

		if (maxbits > MAX_BITS)
		{
		  throw new IOException("Stream compressed with " + maxbits + " bits, but can only handle " + MAX_BITS + " bits");
		}

		if ((header & HDR_EXTENDED) > 0)
		{
		  throw new IOException("Header extension bit set");
		}

		if ((header & HDR_FREE) > 0)
		{
		  throw new IOException("Header bit 6 set");
		}

		if (debug)
		{
		  Console.Error.WriteLine("block mode: " + block_mode);
		  Console.Error.WriteLine("max bits:   " + maxbits);
		}

		// initialize stuff

		maxmaxcode = 1 << maxbits;
		n_bits = INIT_BITS;
		maxcode = (1 << n_bits) - 1;
		bitmask = maxcode;
		oldcode = -1;
		finchar = 0;
		free_ent = block_mode ? TBL_FIRST : 256;

		tab_prefix = new int[1 << maxbits];
		tab_suffix = new sbyte[1 << maxbits];
		stack = new sbyte[1 << maxbits];
		stackp = stack.Length;

		for (int idx = 255; idx >= 0; idx--)
		{
		  tab_suffix[idx] = (sbyte) idx;
		}
	  }

	  private const bool debug = false;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static void main(String args[]) throws Exception
	  public static void Main(string[] args)
	  {
		if (args.Length != 1)
		{
		  Console.Error.WriteLine("Usage: UncompressInputStream <file>");
		  Environment.Exit(1);
		}

		InputStream @in = new UncompressInputStream(new FileInputStream(args[0]));

		sbyte[] buf = new sbyte[100000];
		int tot = 0;
		long beg = DateTimeHelperClass.CurrentUnixTimeMillis();

		while (true)
		{
		  int got = @in.read(buf);
		  if (got < 0)
		  {
			break;
		  }
		  System.out.write(buf, 0, got);
		  tot += got;
		}

		long end = DateTimeHelperClass.CurrentUnixTimeMillis();
		Console.Error.WriteLine("Decompressed " + tot + " bytes");
		Console.Error.WriteLine("Time: " + (end - beg) / 1000.0 + " seconds");
	  }
	}

}