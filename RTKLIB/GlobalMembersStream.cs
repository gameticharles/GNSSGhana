using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ghGPS.Classes
{
    public static class GlobalMembersStream
    {
        /*------------------------------------------------------------------------------
        * stream.c : stream input/output functions
        *
        *          Copyright (C) 2008-2014 by T.TAKASU, All rights reserved.
        *
        * options : -DWIN32    use WIN32 API
        *           -DSVR_REUSEADDR reuse tcp server address
        *
        * references :
        *     [1] RTCM Recommendaed Standards for Networked Transport for RTCM via
        *         Internet Protocol (Ntrip), Version 1.0, Semptember 30, 2004
        *     [2] H.Niksic and others, GNU Wget 1.12, The non-iteractive download
        *         utility, 4 September 2009
        *
        * version : $Revision:$ $Date:$
        * history : 2009/01/16 1.0  new
        *           2009/04/02 1.1  support nmea request in ntrip request
        *                           support time-tag of file as stream
        *           2009/09/04 1.2  ported to linux environment
        *                           add fflush() to save file stream
        *           2009/10/10 1.3  support multiple connection for tcp server
        *                           add keyword replacement in file path
        *                           add function strsendnmea(), strsendcmd()
        *           2010/07/18 1.4  support ftp/http stream types
        *                           add keywords replacement of %ha,%hb,%hc in path
        *                           add api: strsetdir(),strsettimeout()
        *           2010/08/31 1.5  reconnect after error of ntrip client
        *                           fix bug on no file swap at week start (2.4.0_p6)
        *           2011/05/29 1.6  add fast stream replay mode
        *                           add time margin to swap file
        *                           change api strsetopt()
        *                           introduce non_block send for send socket
        *                           add api: strsetproxy()
        *           2011/12/21 1.7  fix bug decode tcppath (rtklib_2.4.1_p5)
        *           2012/06/09 1.8  fix problem if user or password contains /
        *                           (rtklib_2.4.1_p7)
        *           2012/12/25 1.9  compile option SVR_REUSEADDR added
        *           2013/03/10 1.10 fix problem with ntrip mountpoint containing "/"
        *           2013/04/15 1.11 fix bug on swapping files if swapmargin=0
        *           2013/05/28 1.12 fix bug on playback of file with 64 bit size_t
        *           2014/05/23 1.13 retry to connect after gethostbyname() error
        *                           fix bug on malloc size in openftp()
        *           2014/06/21 1.14 add general hex message rcv command by !HEX ...
        *           2014/10/16 1.15 support stdin/stdou for input/output from/to file
        *           2014/11/08 1.16 fix getconfig error (87) with bluetooth device
        *-----------------------------------------------------------------------------*/


        internal const string rcsid = "$Id$";

        /* global options ------------------------------------------------------------*/

        internal static int toinact = 10000; // inactive timeout (ms)
        internal static int ticonnect = 10000; // interval to re-connect (ms)
        internal static int tirate = 1000; // avraging time for data rate (ms)
        internal static int buffsize = 32768; // receive/send buffer size (bytes)
        internal static string localdir = ""; // local directory for ftp/http
        internal static string proxyaddr = ""; // http/ntrip/ftp proxy address
        internal static uint tick_master = 0; // time tick master for replay
        internal static int fswapmargin = 30; // file swap margin (s)

        /* read/write serial buffer --------------------------------------------------*/
#if WIN32
//C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on the parameter 'serial', so pointers on this parameter are left unchanged:
	internal static int readseribuff(serial_t * serial, byte[] buff, int nmax)
	{
		int ns;

		GlobalMembersRtkcmn.tracet(5, "readseribuff: dev=%d\n", serial.dev);

#if lock_ConditionalDefinition1
		EnterCriticalSection(serial.@lock);
#elif lock_ConditionalDefinition2
		pthread_mutex_lock(serial.@lock);
#else
		lock (serial.lock);
#endif
		for (ns = 0;serial.rp != serial.wp && ns < nmax;ns++)
		{
		   buff[ns] = serial.buff[serial.rp];
		   if (++serial.rp >= serial.buffsize)
		   {
			   serial.rp = 0;
		   }
		}
#if unlock_ConditionalDefinition1
		LeaveCriticalSection(serial.@lock);
#elif unlock_ConditionalDefinition2
		pthread_mutex_unlock(serial.@lock);
#else
		unlock(serial.@lock);
#endif
		GlobalMembersRtkcmn.tracet(5,"readseribuff: ns=%d rp=%d wp=%d\n",ns,serial.rp,serial.wp);
		return ns;
	}
	internal static int writeseribuff(serial_t serial, byte[] buff, int n)
	{
		int ns;
		int wp;

		GlobalMembersRtkcmn.tracet(5,"writeseribuff: dev=%d n=%d\n",serial.dev,n);

#if lock_ConditionalDefinition1
		EnterCriticalSection(serial.@lock);
#elif lock_ConditionalDefinition2
		pthread_mutex_lock(serial.@lock);
#else
		lock (serial.lock);
#endif
		for (ns = 0;ns < n;ns++)
		{
			serial.buff[wp = serial.wp] = buff[ns];
			if (++wp >= serial.buffsize)
			{
				wp = 0;
			}
			if (wp != serial.rp)
			{
				serial.wp = wp;
			}
			else
			{
				GlobalMembersRtkcmn.tracet(2, "serial buffer overflow: size=%d\n", serial.buffsize);
				break;
			}
		}
#if unlock_ConditionalDefinition1
		LeaveCriticalSection(serial.@lock);
#elif unlock_ConditionalDefinition2
		pthread_mutex_unlock(serial.@lock);
#else
		unlock(serial.@lock);
#endif
		GlobalMembersRtkcmn.tracet(5,"writeseribuff: ns=%d rp=%d wp=%d\n",ns,serial.rp,serial.wp);
		return ns;
	}
#endif

        /* write serial thread -------------------------------------------------------*/
#if WIN32
//C++ TO C# CONVERTER NOTE: WINAPI is not available in C#:
//ORIGINAL LINE: static uint WINAPI serialthread(object* arg)
	internal static uint serialthread(object arg)
	{
		serial_t serial = (serial_t)arg;
		byte[] buff = new byte[128];
		uint tick;
		uint ns;
		int n;

		GlobalMembersRtkcmn.tracet(3,"serialthread:\n");

		serial.state = 1;

		for (;;)
		{
			tick = GlobalMembersRtkcmn.tickget();
			while ((n = GlobalMembersStream.readseribuff(serial, buff, sizeof(byte))) > 0)
			{
				if (!WriteFile(serial.dev, buff, n, ns, null))
				{
					serial.error = 1;
				}
			}
			if (serial.state == 0)
				break;
			GlobalMembersRtkcmn.sleepms(10 - (int)(GlobalMembersRtkcmn.tickget() - tick)); // cycle=10ms
		}
//C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
		free(serial.buff);
		return 0;
	}
#endif

        /* open serial ---------------------------------------------------------------*/
        internal static serial_t openserial(string path, int mode, ref string msg)
        {
            int[] br = { 300, 600, 1200, 2400, 4800, 9600, 19200, 38400, 57600, 115200, 230400 };
            serial_t serial;
            int i;
            int brate = 9600;
            int bsize = 8;
            int stopb = 1;
            string p;
            sbyte parity = (sbyte)'N';
            string dev = new string(new char[128]);
            string port = new string(new char[128]);
            string fctr = "";
#if WIN32
		uint error;
		uint rw = 0;
		uint siz = sizeof(COMMCONFIG);
		COMMCONFIG cc = new COMMCONFIG();
		COMMTIMEOUTS co = new COMMTIMEOUTS(MAXDWORD,0,0,0,0); // non-block-read
		string dcb = "";
#else
            speed_t[] bs = { B300, B600, B1200, B2400, B4800, B9600, B19200, B38400, B57600, B115200, B230400 };
            termios ios = new termios();
            int rw = 0;
#endif
            GlobalMembersRtkcmn.tracet(3, "openserial: path=%s mode=%d\n", path, mode);

            //C++ TO C# CONVERTER TODO TASK: The memory management function 'malloc' has no equivalent in C#:
            if ((serial = (serial_t)malloc(sizeof(serial_t))) == null)
            {
                return null;
            }

            if ((p = StringFunctions.StrChr(path, ':')) != 0)
            {
                port = path.Substring(0, p - path);
                port[p - path] = '\0';
                sscanf(p, ":%d:%d:%c:%d:%s", brate, bsize, parity, stopb, fctr);
            }
            else
            {
                port = path;
            }

            for (i = 0; i < 11; i++)
            {
                if (br[i] == brate)
                    break;
            }
            if (i >= 12)
            {
                msg = string.Format("bitrate error ({0:D})", brate);
                GlobalMembersRtkcmn.tracet(1, "openserial: %s path=%s\n", msg, path);
                //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
                free(serial);
                return null;
            }
            parity = (sbyte)char.ToUpper((int)parity);

#if WIN32
		dev = string.Format("\\\\.\\{0}", port);
		if ((mode & DefineConstants.STR_MODE_R) != 0)
		{
			rw |= GENERIC_READ;
		}
		if ((mode & DefineConstants.STR_MODE_W) != 0)
		{
			rw |= GENERIC_WRITE;
		}

		serial.dev = CreateFile(dev,rw,0,0,OPEN_EXISTING,0,null);
		if (serial.dev == INVALID_HANDLE_VALUE)
		{
			msg = string.Format("device open error ({0:D})", (int)GetLastError());
			GlobalMembersRtkcmn.tracet(1,"openserial: %s path=%s\n",msg,path);
//C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
			free(serial);
			return null;
		}
		if (!GetCommConfig(serial.dev, cc, siz))
		{
			msg = string.Format("getconfig error ({0:D})", (int)GetLastError());
			GlobalMembersRtkcmn.tracet(1, "openserial: %s\n", msg);
			CloseHandle(serial.dev);
//C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
			free(serial);
			return null;
		}
		dcb = string.Format("baud={0:D} parity={1} data={2:D} stop={3:D}", brate, parity, bsize, stopb);
		if (!BuildCommDCB(dcb, cc.dcb))
		{
			msg = string.Format("buiddcb error ({0:D})", (int)GetLastError());
			GlobalMembersRtkcmn.tracet(1, "openserial: %s\n", msg);
			CloseHandle(serial.dev);
//C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
			free(serial);
			return null;
		}
		if (!string.Compare(fctr,"rts"))
		{
			cc.dcb.fRtsControl = RTS_CONTROL_HANDSHAKE;
		}
		SetCommConfig(serial.dev, cc, siz); // ignore error to support novatel
		SetCommTimeouts(serial.dev, co);
		ClearCommError(serial.dev, error, null);
		PurgeComm(serial.dev,PURGE_TXABORT | PURGE_RXABORT | PURGE_TXCLEAR | PURGE_RXCLEAR);

		/* create write thread */
#if initlock_ConditionalDefinition1
		InitializeCriticalSection(serial.@lock);
#elif initlock_ConditionalDefinition2
		pthread_mutex_init(serial.@lock, null);
#else
		initlock(serial.@lock);
#endif
		serial.state = serial.wp = serial.rp = serial.error = 0;
		serial.buffsize = buffsize;
//C++ TO C# CONVERTER TODO TASK: The memory management function 'malloc' has no equivalent in C#:
		if ((serial.buff = (byte)malloc(buffsize)) == 0)
		{
			CloseHandle(serial.dev);
//C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
			free(serial);
			return null;
		}
		if (!(serial.thread = CreateThread(null,0,GlobalMembersStream.serialthread,serial,0,null)))
		{
			msg = string.Format("serial thread error ({0:D})", (int)GetLastError());
			GlobalMembersRtkcmn.tracet(1, "openserial: %s\n", msg);
			CloseHandle(serial.dev);
//C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
			free(serial);
			return null;
		}
		return serial;
#else
            dev = string.Format("/dev/{0}", port);

            if ((mode & DefineConstants.STR_MODE_R) && (mode & DefineConstants.STR_MODE_W))
            {
                rw = O_RDWR;
            }
            else if ((mode & DefineConstants.STR_MODE_R) != 0)
            {
                rw = O_RDONLY;
            }
            else if ((mode & DefineConstants.STR_MODE_W) != 0)
            {
                rw = O_WRONLY;
            }

            if ((serial.dev = open(dev, rw | O_NOCTTY | O_NONBLOCK)) < 0)
            {
                msg = string.Format("device open error ({0:D})", errno);
                GlobalMembersRtkcmn.tracet(1, "openserial: %s dev=%s\n", msg, dev);
                //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
                free(serial);
                return null;
            }
            tcgetattr(serial.dev, ios);
            ios.c_iflag = 0;
            ios.c_oflag = 0;
            ios.c_lflag = 0; // non-canonical
            ios.c_cc[VMIN] = 0; // non-block-mode
            ios.c_cc[VTIME] = 0;
            cfsetospeed(ios, bs[i]);
            cfsetispeed(ios, bs[i]);
            ios.c_cflag |= bsize == 7 ? CS7 : CS8;
            ios.c_cflag |= parity == 'O' ? (PARENB | PARODD) : (parity == 'E' ? PARENB : 0);
            ios.c_cflag |= stopb == 2 ? CSTOPB : 0;
            ios.c_cflag |= !string.Compare(fctr, "rts") ? CRTSCTS : 0;
            tcsetattr(serial.dev, TCSANOW, ios);
            tcflush(serial.dev, TCIOFLUSH);
            return serial;
#endif
        }
        /* close serial --------------------------------------------------------------*/
        internal static void closeserial(serial_t serial)
        {
            GlobalMembersRtkcmn.tracet(3, "closeserial: dev=%d\n", serial.dev);

            if (serial == null)
                return;
#if WIN32
		serial.state = 0;
		WaitForSingleObject(serial.thread,10000);
		CloseHandle(serial.dev);
		CloseHandle(serial.thread);
#else
            close(serial.dev);
#endif
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(serial);
        }
        /* read serial ---------------------------------------------------------------*/
        internal static int readserial(serial_t serial, ref byte buff, int n, ref string msg)
        {
#if WIN32
		uint nr;
#else
            int nr;
#endif
            GlobalMembersRtkcmn.tracet(4, "readserial: dev=%d n=%d\n", serial.dev, n);
            if (serial == null)
            {
                return 0;
            }
#if WIN32
		if (!ReadFile(serial.dev, buff, n, nr, null))
		{
			return 0;
		}
#else
            if ((nr = read(serial.dev, buff, n)) < 0)
            {
                return 0;
            }
#endif
            GlobalMembersRtkcmn.tracet(5, "readserial: exit dev=%d nr=%d\n", serial.dev, nr);
            return nr;
        }
        /* write serial --------------------------------------------------------------*/
        internal static int writeserial(serial_t serial, ref byte buff, int n, ref string msg)
        {
            int ns;

            GlobalMembersRtkcmn.tracet(3, "writeserial: dev=%d n=%d\n", serial.dev, n);

            if (serial == null)
            {
                return 0;
            }
#if WIN32
		if ((ns = GlobalMembersStream.writeseribuff(serial, buff, n)) < n)
		{
			serial.error = 1;
		}
#else
            if ((ns = write(serial.dev, buff, n)) < 0)
            {
                return 0;
            }
#endif
            GlobalMembersRtkcmn.tracet(5, "writeserial: exit dev=%d ns=%d\n", serial.dev, ns);
            return ns;
        }
        /* get state serial ----------------------------------------------------------*/
        internal static int stateserial(serial_t serial)
        {
            return serial == null ? 0 : (serial.error != 0 ? -1 : 2);
        }
        /* open file -----------------------------------------------------------------*/
        internal static int openfile_(file_t file, gtime_t time, ref string msg)
        {
            FILE fp;
            string rw;
            string tagpath = "";
            string tagh = "";

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: tracet(3,"openfile_: path=%s time=%s\n",file->path,time_str(time,0));
            GlobalMembersRtkcmn.tracet(3, "openfile_: path=%s time=%s\n", file.path, GlobalMembersRtkcmn.time_str(new gtime_t(time), 0));

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: file->time=utc2gpst(timeget());
            file.time.CopyFrom(GlobalMembersRtkcmn.utc2gpst(GlobalMembersRtkcmn.timeget()));
            file.tick = file.tick_f = GlobalMembersRtkcmn.tickget();
            file.fpos = 0;

            /* use stdin or stdout if file path is null */
            if (!*file.path)
            {
                file.fp = (file.mode & DefineConstants.STR_MODE_R) != 0 ? stdin : stdout;
                return 1;
            }
            /* replace keywords */
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: reppath(file->path,file->openpath,time,"","");
            GlobalMembersRtkcmn.reppath(file.path, ref file.openpath, new gtime_t(time), "", "");

            /* create directory */
            if ((file.mode & DefineConstants.STR_MODE_W) && !(file.mode & DefineConstants.STR_MODE_R))
            {
                GlobalMembersRtkcmn.createdir(file.openpath);
            }
            if ((file.mode & DefineConstants.STR_MODE_R) != 0)
            {
                rw = "rb";
            }
            else
            {
                rw = "wb";
            }

            if ((file.fp = fopen(file.openpath, rw)) == null)
            {
                msg = string.Format("file open error: {0}", file.openpath);
                GlobalMembersRtkcmn.tracet(1, "openfile: %s\n", msg);
                return 0;
            }
            GlobalMembersRtkcmn.tracet(4, "openfile_: open file %s (%s)\n", file.openpath, rw);

            tagpath = string.Format("{0}.tag", file.openpath);

            if (file.timetag != 0) // output/sync time-tag
            {

                if ((file.fp_tag = fopen(tagpath, rw)) == null)
                {
                    msg = string.Format("tag open error: {0}", tagpath);
                    GlobalMembersRtkcmn.tracet(1, "openfile: %s\n", msg);
                    fclose(file.fp);
                    return 0;
                }
                GlobalMembersRtkcmn.tracet(4, "openfile_: open tag file %s (%s)\n", tagpath, rw);

                if ((file.mode & DefineConstants.STR_MODE_R) != 0)
                {
                    if (fread(tagh, DefineConstants.TIMETAGH_LEN, 1, file.fp_tag) == 1 && fread(file.time, sizeof(gtime_t), 1, file.fp_tag) == 1)
                    {
                        //C++ TO C# CONVERTER TODO TASK: The memory management function 'memcpy' has no equivalent in C#:
                        memcpy(file.tick_f, tagh + DefineConstants.TIMETAGH_LEN - 4, sizeof(uint));
                    }
                    else
                    {
                        file.tick_f = 0;
                    }
                    /* adust time to read playback file */
                    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                    //ORIGINAL LINE: timeset(file->time);
                    GlobalMembersRtkcmn.timeset(new gtime_t(file.time));
                }
                else
                {
                    tagh = string.Format("TIMETAG RTKLIB {0}", DefineConstants.VER_RTKLIB);
                    //C++ TO C# CONVERTER TODO TASK: The memory management function 'memcpy' has no equivalent in C#:
                    memcpy(tagh + DefineConstants.TIMETAGH_LEN - 4, file.tick_f, sizeof(uint));
                    fwrite(tagh, 1, DefineConstants.TIMETAGH_LEN, file.fp_tag);
                    fwrite(file.time, 1, sizeof(gtime_t), file.fp_tag);
                    /* time tag file structure   */
                    /*   HEADER(60)+TICK(4)+TIME(12)+ */
                    /*   TICK0(4)+FPOS0(4/8)+    */
                    /*   TICK1(4)+FPOS1(4/8)+... */
                }
            }
            else if ((file.mode & DefineConstants.STR_MODE_W) != 0) // remove time-tag
            {
                if ((fp = fopen(tagpath, "rb")) != null)
                {
                    fclose(fp);
                    remove(tagpath);
                }
            }
            return 1;
        }
        /* close file ----------------------------------------------------------------*/
        internal static void closefile_(file_t file)
        {
            GlobalMembersRtkcmn.tracet(3, "closefile_: path=%s\n", file.path);

            if (file.fp != null)
            {
                fclose(file.fp);
            }
            if (file.fp_tag != null)
            {
                fclose(file.fp_tag);
            }
            if (file.fp_tmp != null)
            {
                fclose(file.fp_tmp);
            }
            if (file.fp_tag_tmp != null)
            {
                fclose(file.fp_tag_tmp);
            }
            file.fp = file.fp_tag = file.fp_tmp = file.fp_tag_tmp = null;
        }
        /* open file (path=filepath[::T[::+<off>][::x<speed>]][::S=swapintv]) --------*/
        internal static file_t openfile(string path, int mode, ref string msg)
        {
            file_t file;
            gtime_t time = new gtime_t();
            gtime_t time0 = new gtime_t();
            double speed = 0.0;
            double start = 0.0;
            double swapintv = 0.0;
            //C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on this variable, so pointers on this variable are left unchanged:
            sbyte* p;
            int timetag = 0;

            GlobalMembersRtkcmn.tracet(3, "openfile: path=%s mode=%d\n", path, mode);

            if (!(mode & (DefineConstants.STR_MODE_R | DefineConstants.STR_MODE_W)))
            {
                return null;
            }

            /* file options */
            for (p = (string)path; (p = StringFunctions.StrStr(p, "::")); p += 2) // file options
            {
                if (*(p + 2) == 'T')
                {
                    timetag = 1;
                }
                else if (*(p + 2) == '+')
                {
                    sscanf(p + 2, "+%lf", start);
                }
                else if (*(p + 2) == 'x')
                {
                    sscanf(p + 2, "x%lf", speed);
                }
                else if (*(p + 2) == 'S')
                {
                    sscanf(p + 2, "S=%lf", swapintv);
                }
            }
            if (start <= 0.0)
            {
                start = 0.0;
            }
            if (swapintv <= 0.0)
            {
                swapintv = 0.0;
            }

            //C++ TO C# CONVERTER TODO TASK: The memory management function 'malloc' has no equivalent in C#:
            if ((file = (file_t)malloc(sizeof(file_t))) == null)
            {
                return null;
            }

            file.fp = file.fp_tag = file.fp_tmp = file.fp_tag_tmp = null;
            file.path = path;
            if ((p = StringFunctions.StrStr(file.path, "::")) != 0)
            {
                *p = (sbyte)'\0';
            }
            file.openpath[0] = '\0';
            file.mode = mode;
            file.timetag = timetag;
            file.repmode = 0;
            file.offset = 0;
            file.time = file.wtime = time0;
            file.tick = file.tick_f = file.fpos = 0;
            file.start = start;
            file.speed = speed;
            file.swapintv = swapintv;
#if initlock_ConditionalDefinition1
		InitializeCriticalSection(file.@lock);
#elif initlock_ConditionalDefinition2
		pthread_mutex_init(file.@lock, null);
#else
            initlock(file.@lock);
#endif

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: time=utc2gpst(timeget());
            time.CopyFrom(GlobalMembersRtkcmn.utc2gpst(GlobalMembersRtkcmn.timeget()));

            /* open new file */
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: if (!openfile_(file,time,msg))
            if (GlobalMembersStream.openfile_(file, new gtime_t(time), ref msg) == 0)
            {
                //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
                free(file);
                return null;
            }
            return file;
        }
        /* close file ----------------------------------------------------------------*/
        internal static void closefile(file_t file)
        {
            GlobalMembersRtkcmn.tracet(3, "closefile: fp=%d\n", file.fp);

            if (file == null)
                return;
            GlobalMembersStream.closefile_(file);
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(file);
        }
        /* open new swap file --------------------------------------------------------*/
        internal static void swapfile(file_t file, gtime_t time, ref string msg)
        {
            string openpath = new string(new char[DefineConstants.MAXSTRPATH]);

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: tracet(3,"swapfile: fp=%d time=%s\n",file->fp,time_str(time,0));
            GlobalMembersRtkcmn.tracet(3, "swapfile: fp=%d time=%s\n", file.fp, GlobalMembersRtkcmn.time_str(new gtime_t(time), 0));

            /* return if old swap file open */
            if (file.fp_tmp != null || file.fp_tag_tmp != null)
                return;

            /* check path of new swap file */
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: reppath(file->path,openpath,time,"","");
            GlobalMembersRtkcmn.reppath(file.path, ref openpath, new gtime_t(time), "", "");

            if (!string.Compare(openpath, file.openpath))
            {
                GlobalMembersRtkcmn.tracet(2, "swapfile: no need to swap %s\n", openpath);
                return;
            }
            /* save file pointer to temporary pointer */
            file.fp_tmp = file.fp;
            file.fp_tag_tmp = file.fp_tag;

            /* open new swap file */
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: openfile_(file,time,msg);
            GlobalMembersStream.openfile_(file, new gtime_t(time), ref msg);
        }
        /* close old swap file -------------------------------------------------------*/
        internal static void swapclose(file_t file)
        {
            GlobalMembersRtkcmn.tracet(3, "swapclose: fp_tmp=%d\n", file.fp_tmp);

            if (file.fp_tmp != null)
            {
                fclose(file.fp_tmp);
            }
            if (file.fp_tag_tmp != null)
            {
                fclose(file.fp_tag_tmp);
            }
            file.fp_tmp = file.fp_tag_tmp = null;
        }
        /* get state file ------------------------------------------------------------*/
        internal static int statefile(file_t file)
        {
            return file != null ? 2 : 0;
        }
        /* read file -----------------------------------------------------------------*/
        internal static int readfile(file_t file, ref byte buff, int nmax, ref string msg)
        {
            timeval tv = new timeval();
            fd_set rs = new fd_set();
            uint nr = 0;
            uint t;
            uint tick;
            uint fpos;

            GlobalMembersRtkcmn.tracet(4, "readfile: fp=%d nmax=%d\n", file.fp, nmax);

            if (file == null)
            {
                return 0;
            }

            if (file.fp == stdin)
            {
#if !WIN32
                /* input from stdin */
                FD_ZERO(rs);
                FD_SET(0, rs);
                if (!select(1, rs, null, null, tv))
                {
                    return 0;
                }
                if ((nr = read(0, buff, nmax)) < 0)
                {
                    return 0;
                }
                return nr;
#else
			return 0;
#endif
            }
            if (file.fp_tag != null)
            {
                if (file.repmode != 0) // slave
                {
                    t = (uint)(tick_master + file.offset);
                }
                else // master
                {
                    t = (uint)((GlobalMembersRtkcmn.tickget() - file.tick) * file.speed + file.start * 1000.0);
                }
                for (;;) // seek file position
                {
                    if (fread(tick, sizeof(uint), 1, file.fp_tag) < 1 || fread(fpos, sizeof(uint), 1, file.fp_tag) < 1)
                    {
                        fseek(file.fp, 0, SEEK_END);
                        msg = "end";
                        break;
                    }
                    if (file.repmode != 0 || file.speed > 0.0)
                    {
                        if ((int)(tick - t) < 1)
                            continue;
                    }
                    if (file.repmode == 0)
                    {
                        tick_master = tick;
                    }

                    //C++ TO C# CONVERTER TODO TASK: The following line has a C format specifier which cannot be directly translated to C#:
                    //ORIGINAL LINE: sprintf(msg,"T%+.1fs",(int)tick<0?0.0:(int)tick/1000.0);
                    msg = string.Format("T%+.1fs", (int)tick < 0 ? 0.0 : (int)tick / 1000.0);

                    if ((int)(fpos - file.fpos) >= nmax)
                    {
                        fseek(file.fp, fpos, SEEK_SET);
                        file.fpos = fpos;
                        return 0;
                    }
                    nmax = (int)(fpos - file.fpos);

                    if (file.repmode != 0 || file.speed > 0.0)
                    {
                        fseek(file.fp_tag, -(int)(sizeof(uint) + sizeof(uint)), SEEK_CUR);
                    }
                    break;
                }
            }
            if (nmax > 0)
            {
                nr = fread(buff, 1, nmax, file.fp);
                file.fpos += nr;
                if (nr <= 0)
                {
                    msg = "end";
                }
            }
            GlobalMembersRtkcmn.tracet(5, "readfile: fp=%d nr=%d fpos=%d\n", file.fp, nr, file.fpos);
            return (int)nr;
        }
        /* write file ----------------------------------------------------------------*/
        internal static int writefile(file_t file, ref byte buff, int n, ref string msg)
        {
            gtime_t wtime = new gtime_t();
            uint ns;
            uint tick = GlobalMembersRtkcmn.tickget();
            int week1;
            int week2;
            double tow1;
            double tow2;
            double intv;
            uint fpos;
            uint fpos_tmp;

            GlobalMembersRtkcmn.tracet(3, "writefile: fp=%d n=%d\n", file.fp, n);

            if (file == null)
            {
                return 0;
            }

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: wtime=utc2gpst(timeget());
            wtime.CopyFrom(GlobalMembersRtkcmn.utc2gpst(GlobalMembersRtkcmn.timeget())); // write time in gpst

            /* swap writing file */
            if (file.swapintv > 0.0 && file.wtime.time != 0)
            {
                intv = file.swapintv * 3600.0;
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: tow1=time2gpst(file->wtime,&week1);
                tow1 = GlobalMembersRtkcmn.time2gpst(new gtime_t(file.wtime), ref week1);
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: tow2=time2gpst(wtime,&week2);
                tow2 = GlobalMembersRtkcmn.time2gpst(new gtime_t(wtime), ref week2);
                tow2 += 604800.0 * (week2 - week1);

                /* open new swap file */
                if (Math.Floor((tow1 + fswapmargin) / intv) < Math.Floor((tow2 + fswapmargin) / intv))
                {
                    //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                    //ORIGINAL LINE: swapfile(file,timeadd(wtime,fswapmargin),msg);
                    GlobalMembersStream.swapfile(file, GlobalMembersRtkcmn.timeadd(new gtime_t(wtime), fswapmargin), ref msg);
                }
                /* close old swap file */
                if (Math.Floor((tow1 - fswapmargin) / intv) < Math.Floor((tow2 - fswapmargin) / intv))
                {
                    GlobalMembersStream.swapclose(file);
                }
            }
            if (file.fp == null)
            {
                return 0;
            }

            ns = fwrite(buff, 1, n, file.fp);
            fpos = ftell(file.fp);
            fflush(file.fp);
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: file->wtime=wtime;
            file.wtime.CopyFrom(wtime);

            if (file.fp_tmp != null)
            {
                fwrite(buff, 1, n, file.fp_tmp);
                fpos_tmp = ftell(file.fp_tmp);
                fflush(file.fp_tmp);
            }
            if (file.fp_tag != null)
            {
                tick -= file.tick;
                fwrite(tick, 1, sizeof(uint), file.fp_tag);
                fwrite(fpos, 1, sizeof(uint), file.fp_tag);
                fflush(file.fp_tag);

                if (file.fp_tag_tmp != null)
                {
                    fwrite(tick, 1, sizeof(uint), file.fp_tag_tmp);
                    fwrite(fpos_tmp, 1, sizeof(uint), file.fp_tag_tmp);
                    fflush(file.fp_tag_tmp);
                }
            }
            GlobalMembersRtkcmn.tracet(5, "writefile: fp=%d ns=%d tick=%5d fpos=%d\n", file.fp, ns, tick, fpos);

            return (int)ns;
        }
        /* sync files by time-tag ----------------------------------------------------*/
        internal static void syncfile(file_t file1, file_t file2)
        {
            if (file1.fp_tag == null || file2.fp_tag == null)
                return;
            file1.repmode = 0;
            file2.repmode = 1;
            file2.offset = (int)(file1.tick_f - file2.tick_f);
        }
        /* decode tcp/ntrip path (path=[user[:passwd]@]addr[:port][/mntpnt[:str]]) ---*/
        internal static void decodetcppath(string path, ref string addr, ref string port, ref string user, ref string passwd, ref string mntpnt, ref string str)
        {
            string buff = new string(new char[DefineConstants.MAXSTRPATH]);
            string p;
            string q;

            GlobalMembersRtkcmn.tracet(4, "decodetcpepath: path=%s\n", path);

            if (port != 0)
            {
                port = (sbyte)'\0';
            }
            if (user != 0)
            {
                user = (sbyte)'\0';
            }
            if (passwd != 0)
            {
                passwd = (sbyte)'\0';
            }
            if (mntpnt != 0)
            {
                mntpnt = (sbyte)'\0';
            }
            if (str != 0)
            {
                str = (sbyte)'\0';
            }

            buff = path;

            if ((p = StringFunctions.StrRChr(buff, '@')) == 0)
            {
                p = buff;
            }

            if ((p = StringFunctions.StrChr(p, '/')) != 0)
            {
                if ((q = StringFunctions.StrChr(p.Substring(1), ':')) != 0)
                {
                    q = (sbyte)'\0';
                    if (str != 0)
                    {
                        str = q.Substring(1);
                    }
                }
                p = (sbyte)'\0';
                if (mntpnt != 0)
                {
                    mntpnt = p.Substring(1);
                }
            }
            if ((p = StringFunctions.StrRChr(buff, '@')) != 0)
            {
                p++ = '\0';
                if ((q = StringFunctions.StrChr(buff, ':')) != 0)
                {
                    q = (sbyte)'\0';
                    if (passwd != 0)
                    {
                        passwd = q.Substring(1);
                    }
                }
                if (user != 0)
                {
                    user = buff;
                }
            }
            else
            {
                p = buff;
            }

            if ((q = StringFunctions.StrChr(p, ':')) != 0)
            {
                q = (sbyte)'\0';
                if (port != 0)
                {
                    port = q.Substring(1);
                }
            }
            if (addr != 0)
            {
                addr = p;
            }
        }
        /* get socket error ----------------------------------------------------------*/
#if WIN32
	internal static int errsock()
	{
		return WSAGetLastError();
	}
#else
        internal static int errsock()
        {
            return errno;
        }
#endif

        /* set socket option ---------------------------------------------------------*/
#if socket_t_ConditionalDefinition1
	internal static int setsock(SOCKET sock, ref string msg)
#elif socket_t_ConditionalDefinition2
	internal static int setsock(int sock, ref string msg)
#else
        internal static int setsock(socket_t sock, ref string msg)
#endif
        {
            int bs = buffsize;
            int mode = 1;
#if WIN32
		int tv = 0;
#else
            timeval tv = new timeval();
#endif
            GlobalMembersRtkcmn.tracet(3, "setsock: sock=%d\n", sock);

            if (setsockopt(sock, SOL_SOCKET, SO_RCVTIMEO, (string)tv, sizeof(int)) == -1 || setsockopt(sock, SOL_SOCKET, SO_SNDTIMEO, (string)tv, sizeof(int)) == -1)
            {
                msg = "sockopt error: notimeo";
                GlobalMembersRtkcmn.tracet(1, "setsock: setsockopt error 1 sock=%d err=%d\n", sock, GlobalMembersStream.errsock());
                close(sock);
                return 0;
            }
            if (setsockopt(sock, SOL_SOCKET, SO_RCVBUF, (string)&bs, sizeof(int)) == -1 || setsockopt(sock, SOL_SOCKET, SO_SNDBUF, (string)&bs, sizeof(int)) == -1)
            {
                GlobalMembersRtkcmn.tracet(1, "setsock: setsockopt error 2 sock=%d err=%d bs=%d\n", sock, GlobalMembersStream.errsock(), bs);
                msg = "sockopt error: bufsiz";
            }
            if (setsockopt(sock, IPPROTO_TCP, TCP_NODELAY, (string)&mode, sizeof(int)) == -1)
            {
                GlobalMembersRtkcmn.tracet(1, "setsock: setsockopt error 3 sock=%d err=%d\n", sock, GlobalMembersStream.errsock());
                msg = "sockopt error: nodelay";
            }
            return 1;
        }
        /* non-block accept ----------------------------------------------------------*/
#if socket_t_ConditionalDefinition1
	internal static SOCKET accept_nb(SOCKET sock, sockaddr addr, ref int len)
#elif socket_t_ConditionalDefinition2
	internal static int accept_nb(int sock, sockaddr addr, ref int len)
#else
        internal static socket_t accept_nb(socket_t sock, sockaddr addr, ref int len)
#endif
        {
            timeval tv = new timeval();
            fd_set rs = new fd_set();

            FD_ZERO(rs);
            FD_SET(sock, rs);
            if (!select(sock + 1, rs, null, null, tv))
            {
                return 0;
            }
            return accept(sock, addr, len);
        }
        /* non-block connect ---------------------------------------------------------*/
#if socket_t_ConditionalDefinition1
	internal static int connect_nb(SOCKET sock, sockaddr addr, int len)
#elif socket_t_ConditionalDefinition2
	internal static int connect_nb(int sock, sockaddr addr, int len)
#else
        internal static int connect_nb(socket_t sock, sockaddr addr, int len)
#endif
        {
#if WIN32
		u_long mode = 1;
		int err;

		ioctlsocket(sock, FIONBIO, mode);
		if (connect(sock,addr,len) == -1)
		{
			err = GlobalMembersStream.errsock();
			if (err == WSAEWOULDBLOCK || err == WSAEINPROGRESS || err == WSAEALREADY || err == WSAEINVAL)
			{
				return 0;
			}
			if (err != WSAEISCONN)
			{
				return -1;
			}
		}
#else
            timeval tv = new timeval();
            fd_set rs = new fd_set();
            fd_set ws = new fd_set();
            int err;
            int flag;

            flag = fcntl(sock, F_GETFL, 0);
            fcntl(sock, F_SETFL, flag | O_NONBLOCK);
            if (connect(sock, addr, len) == -1)
            {
                err = GlobalMembersStream.errsock();
                if (err != EISCONN && err != EINPROGRESS && err != EALREADY)
                {
                    return -1;
                }
                FD_ZERO(rs);
                FD_SET(sock, rs);
                ws = rs;
                if (select(sock + 1, rs, ws, null, tv) == 0)
                {
                    return 0;
                }
            }
#endif
            return 1;
        }
        /* non-block receive ---------------------------------------------------------*/
#if socket_t_ConditionalDefinition1
	internal static int recv_nb(SOCKET sock, ref byte buff, int n)
#elif socket_t_ConditionalDefinition2
	internal static int recv_nb(int sock, ref byte buff, int n)
#else
        internal static int recv_nb(socket_t sock, ref byte buff, int n)
#endif
        {
            timeval tv = new timeval();
            fd_set rs = new fd_set();

            FD_ZERO(rs);
            FD_SET(sock, rs);
            if (!select(sock + 1, rs, null, null, tv))
            {
                return 0;
            }
            return recv(sock, (string)buff, n, 0);
        }
        /* non-block send ------------------------------------------------------------*/
#if socket_t_ConditionalDefinition1
	internal static int send_nb(SOCKET sock, ref byte buff, int n)
#elif socket_t_ConditionalDefinition2
	internal static int send_nb(int sock, ref byte buff, int n)
#else
        internal static int send_nb(socket_t sock, ref byte buff, int n)
#endif
        {
            timeval tv = new timeval();
            fd_set ws = new fd_set();

            FD_ZERO(ws);
            FD_SET(sock, ws);
            if (!select(sock + 1, null, ws, null, tv))
            {
                return 0;
            }
            return send(sock, (string)buff, n, 0);
        }
        /* generate tcp socket -------------------------------------------------------*/
        internal static int gentcp(tcp_t tcp, int type, ref string msg)
        {
            hostent hp;
#if SVR_REUSEADDR
		int opt = 1;
#endif

            GlobalMembersRtkcmn.tracet(3, "gentcp: type=%d\n", type);

            /* generate socket */
#if socket_t_ConditionalDefinition1
		if ((tcp.sock = socket(AF_INET,SOCK_STREAM,0)) == (SOCKET) - 1)
#elif socket_t_ConditionalDefinition2
		if ((tcp.sock = socket(AF_INET,SOCK_STREAM,0)) == (int)-1)
#else
            if ((tcp.sock = socket(AF_INET, SOCK_STREAM, 0)) == (socket_t) - 1)
#endif
            {
                msg = string.Format("socket error ({0:D})", GlobalMembersStream.errsock());
                GlobalMembersRtkcmn.tracet(1, "gentcp: socket error err=%d\n", GlobalMembersStream.errsock());
                tcp.state = -1;
                return 0;
            }
            if (GlobalMembersStream.setsock(tcp.sock, ref msg) == 0)
            {
                tcp.state = -1;
                return 0;
            }
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'memset' has no equivalent in C#:
            memset(tcp.addr, 0, sizeof(sockaddr_in));
            tcp.addr.sin_family = AF_INET;
            tcp.addr.sin_port = htons(tcp.port);

            if (type == 0) // server socket
            {

#if SVR_REUSEADDR
			/* multiple-use of server socket */
			setsockopt(tcp.sock, SOL_SOCKET, SO_REUSEADDR, (string) & opt, sizeof(int));
#endif
                if (bind(tcp.sock, (sockaddr)tcp.addr, sizeof(sockaddr_in)) == -1)
                {
                    msg = string.Format("bind error ({0:D}) : {1:D}", GlobalMembersStream.errsock(), tcp.port);
                    GlobalMembersRtkcmn.tracet(1, "gentcp: bind error port=%d err=%d\n", tcp.port, GlobalMembersStream.errsock());
                    close(tcp.sock);
                    tcp.state = -1;
                    return 0;
                }
                listen(tcp.sock, 5);
            }
            else // client socket
            {
                if ((hp = gethostbyname(tcp.saddr)) == null)
                {
                    msg = string.Format("address error ({0})", tcp.saddr);
                    GlobalMembersRtkcmn.tracet(1, "gentcp: gethostbyname error addr=%s err=%d\n", tcp.saddr, GlobalMembersStream.errsock());
                    close(tcp.sock);
                    tcp.state = 0;
                    tcp.tcon = ticonnect;
                    tcp.tdis = GlobalMembersRtkcmn.tickget();
                    return 0;
                }
                //C++ TO C# CONVERTER TODO TASK: The memory management function 'memcpy' has no equivalent in C#:
                memcpy(tcp.addr.sin_addr, hp.h_addr, hp.h_length);
            }
            tcp.state = 1;
            tcp.tact = GlobalMembersRtkcmn.tickget();
            GlobalMembersRtkcmn.tracet(5, "gentcp: exit sock=%d\n", tcp.sock);
            return 1;
        }
        /* disconnect tcp ------------------------------------------------------------*/
        internal static void discontcp(tcp_t tcp, int tcon)
        {
            GlobalMembersRtkcmn.tracet(3, "discontcp: sock=%d tcon=%d\n", tcp.sock, tcon);

            close(tcp.sock);
            tcp.state = 0;
            tcp.tcon = tcon;
            tcp.tdis = GlobalMembersRtkcmn.tickget();
        }
        /* open tcp server -----------------------------------------------------------*/
        internal static tcpsvr_t opentcpsvr(string path, ref string msg)
        {
            tcpsvr_t tcpsvr;
            tcpsvr_t tcpsvr0 = new tcpsvr_t({ 0 });
            string port = "";

            GlobalMembersRtkcmn.tracet(3, "opentcpsvr: path=%s\n", path);

            //C++ TO C# CONVERTER TODO TASK: The memory management function 'malloc' has no equivalent in C#:
            if ((tcpsvr = (tcpsvr_t)malloc(sizeof(tcpsvr_t))) == null)
            {
                return null;
            }
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: *tcpsvr=tcpsvr0;
            tcpsvr.CopyFrom(tcpsvr0);
            GlobalMembersStream.decodetcppath(path, ref tcpsvr.svr.saddr, ref port, null, null, null, null);
            if (sscanf(port, "%d", tcpsvr.svr.port) < 1)
            {
                msg = string.Format("port error: {0}", port);
                GlobalMembersRtkcmn.tracet(1, "opentcpsvr: port error port=%s\n", port);
                //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
                free(tcpsvr);
                return null;
            }
            if (GlobalMembersStream.gentcp(tcpsvr.svr, 0, ref msg) == 0)
            {
                //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
                free(tcpsvr);
                return null;
            }
            tcpsvr.svr.tcon = 0;
            return tcpsvr;
        }
        /* close tcp server ----------------------------------------------------------*/
        internal static void closetcpsvr(tcpsvr_t tcpsvr)
        {
            int i;

            GlobalMembersRtkcmn.tracet(3, "closetcpsvr:\n");

            for (i = 0; i < DefineConstants.MAXCLI; i++)
            {
                if (tcpsvr.cli[i].state != 0)
                {
                    close(tcpsvr.cli[i].sock);
                }
            }
            close(tcpsvr.svr.sock);
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(tcpsvr);
        }
        /* update tcp server ---------------------------------------------------------*/
        internal static void updatetcpsvr(tcpsvr_t tcpsvr, ref string msg)
        {
            string saddr = "";
            int i;
            int j;
            int n = 0;

            GlobalMembersRtkcmn.tracet(3, "updatetcpsvr: state=%d\n", tcpsvr.svr.state);

            if (tcpsvr.svr.state == 0)
                return;

            for (i = 0; i < DefineConstants.MAXCLI; i++)
            {
                if (tcpsvr.cli[i].state != 0)
                    continue;
                for (j = i + 1; j < DefineConstants.MAXCLI; j++)
                {
                    if (tcpsvr.cli[j].state == 0)
                        continue;
                    tcpsvr.cli[i] = tcpsvr.cli[j];
                    tcpsvr.cli[j].state = 0;
                    break;
                }
            }
            for (i = 0; i < DefineConstants.MAXCLI; i++)
            {
                if (tcpsvr.cli[i].state == 0)
                    continue;
                saddr = tcpsvr.cli[i].saddr;
                n++;
            }
            if (n == 0)
            {
                tcpsvr.svr.state = 1;
                msg = "waiting...";
                return;
            }
            tcpsvr.svr.state = 2;
            if (n == 1)
            {
                msg = string.Format("{0}", saddr);
            }
            else
            {
                msg = string.Format("{0:D} clients", n);
            }
        }
        /* accept client connection --------------------------------------------------*/
        internal static int accsock(tcpsvr_t tcpsvr, ref string msg)
        {
            sockaddr_in addr = new sockaddr_in();
#if socket_t_ConditionalDefinition1
		SOCKET sock = new SOCKET();
#elif socket_t_ConditionalDefinition2
		int sock;
#else
            socket_t sock = new socket_t();
#endif
            int len = sizeof(sockaddr_in);
            int i;
            int err;

            GlobalMembersRtkcmn.tracet(3, "accsock: sock=%d\n", tcpsvr.svr.sock);

            for (i = 0; i < DefineConstants.MAXCLI; i++)
            {
                if (tcpsvr.cli[i].state == 0)
                    break;
            }
            if (i >= DefineConstants.MAXCLI) // too many client
            {
                return 0;
            }

#if socket_t_ConditionalDefinition1
		if ((sock = GlobalMembersStream.accept_nb(tcpsvr.svr.sock, (sockaddr) addr, ref len)) == (SOCKET) - 1)
#elif socket_t_ConditionalDefinition2
		if ((sock = GlobalMembersStream.accept_nb(tcpsvr.svr.sock, (sockaddr) addr, ref len)) == (int)-1)
#else
            if ((sock = GlobalMembersStream.accept_nb(tcpsvr.svr.sock, (sockaddr)addr, ref len)) == (socket_t) - 1)
#endif
            {
                err = GlobalMembersStream.errsock();
                msg = string.Format("accept error ({0:D})", err);
                GlobalMembersRtkcmn.tracet(1, "accsock: accept error sock=%d err=%d\n", tcpsvr.svr.sock, err);
                close(tcpsvr.svr.sock);
                tcpsvr.svr.state = 0;
                return 0;
            }
            if (sock == 0)
            {
                return 0;
            }

            tcpsvr.cli[i].sock = sock;
            if (GlobalMembersStream.setsock(tcpsvr.cli[i].sock, ref msg) == 0)
            {
                return 0;
            }
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'memcpy' has no equivalent in C#:
            memcpy(tcpsvr.cli[i].addr, addr, sizeof(sockaddr_in));
            tcpsvr.cli[i].saddr = inet_ntoa(addr.sin_addr);
            msg = string.Format("{0}", tcpsvr.cli[i].saddr);
            GlobalMembersRtkcmn.tracet(2, "accsock: connected sock=%d addr=%s\n", tcpsvr.cli[i].sock, tcpsvr.cli[i].saddr);
            tcpsvr.cli[i].state = 2;
            tcpsvr.cli[i].tact = GlobalMembersRtkcmn.tickget();
            return 1;
        }
        /* wait socket accept --------------------------------------------------------*/
        internal static int waittcpsvr(tcpsvr_t tcpsvr, ref string msg)
        {
            GlobalMembersRtkcmn.tracet(4, "waittcpsvr: sock=%d state=%d\n", tcpsvr.svr.sock, tcpsvr.svr.state);

            if (tcpsvr.svr.state <= 0)
            {
                return 0;
            }

            while (GlobalMembersStream.accsock(tcpsvr, ref msg)) ;

            GlobalMembersStream.updatetcpsvr(tcpsvr, ref msg);
            return tcpsvr.svr.state == 2;
        }
        /* read tcp server -----------------------------------------------------------*/
        internal static int readtcpsvr(tcpsvr_t tcpsvr, ref byte buff, int n, ref string msg)
        {
            int nr;
            int err;

            GlobalMembersRtkcmn.tracet(4, "readtcpsvr: state=%d n=%d\n", tcpsvr.svr.state, n);

            if (GlobalMembersStream.waittcpsvr(tcpsvr, ref msg) == 0 || tcpsvr.cli[0].state != 2)
            {
                return 0;
            }

            if ((nr = GlobalMembersStream.recv_nb(tcpsvr.cli[0].sock, ref buff, n)) == -1)
            {
                err = GlobalMembersStream.errsock();
                GlobalMembersRtkcmn.tracet(1, "readtcpsvr: recv error sock=%d err=%d\n", tcpsvr.cli[0].sock, err);
                msg = string.Format("recv error ({0:D})", err);
                GlobalMembersStream.discontcp(tcpsvr.cli[0], ticonnect);
                GlobalMembersStream.updatetcpsvr(tcpsvr, ref msg);
                return 0;
            }
            if (nr > 0)
            {
                tcpsvr.cli[0].tact = GlobalMembersRtkcmn.tickget();
            }
            GlobalMembersRtkcmn.tracet(5, "readtcpsvr: exit sock=%d nr=%d\n", tcpsvr.cli[0].sock, nr);
            return nr;
        }
        /* write tcp server ----------------------------------------------------------*/
        internal static int writetcpsvr(tcpsvr_t tcpsvr, ref byte buff, int n, ref string msg)
        {
            int i;
            int ns = 0;
            int err;

            GlobalMembersRtkcmn.tracet(3, "writetcpsvr: state=%d n=%d\n", tcpsvr.svr.state, n);

            if (GlobalMembersStream.waittcpsvr(tcpsvr, ref msg) == 0)
            {
                return 0;
            }

            for (i = 0; i < DefineConstants.MAXCLI; i++)
            {
                if (tcpsvr.cli[i].state != 2)
                    continue;

                if ((ns = GlobalMembersStream.send_nb(tcpsvr.cli[i].sock, ref buff, n)) == -1)
                {
                    err = GlobalMembersStream.errsock();
                    GlobalMembersRtkcmn.tracet(1, "writetcpsvr: send error i=%d sock=%d err=%d\n", i, tcpsvr.cli[i].sock, err);
                    msg = string.Format("send error ({0:D})", err);
                    GlobalMembersStream.discontcp(tcpsvr.cli[i], ticonnect);
                    GlobalMembersStream.updatetcpsvr(tcpsvr, ref msg);
                    return 0;
                }
                if (ns > 0)
                {
                    tcpsvr.cli[i].tact = GlobalMembersRtkcmn.tickget();
                }
                GlobalMembersRtkcmn.tracet(5, "writetcpsvr: send i=%d ns=%d\n", i, ns);
            }
            return ns;
        }
        /* get state tcp server ------------------------------------------------------*/
        internal static int statetcpsvr(tcpsvr_t tcpsvr)
        {
            return tcpsvr != null ? tcpsvr.svr.state : 0;
        }
        /* connect server ------------------------------------------------------------*/
        internal static int consock(tcpcli_t tcpcli, ref string msg)
        {
            int stat;
            int err;

            GlobalMembersRtkcmn.tracet(3, "consock: sock=%d\n", tcpcli.svr.sock);

            /* wait re-connect */
            if (tcpcli.svr.tcon < 0 || (tcpcli.svr.tcon > 0 && (int)(GlobalMembersRtkcmn.tickget() - tcpcli.svr.tdis) < tcpcli.svr.tcon))
            {
                return 0;
            }
            /* non-block connect */
            if ((stat = GlobalMembersStream.connect_nb(tcpcli.svr.sock, (sockaddr)tcpcli.svr.addr, sizeof(sockaddr_in))) == -1)
            {
                err = GlobalMembersStream.errsock();
                msg = string.Format("connect error ({0:D})", err);
                GlobalMembersRtkcmn.tracet(1, "consock: connect error sock=%d err=%d\n", tcpcli.svr.sock, err);
                close(tcpcli.svr.sock);
                tcpcli.svr.state = 0;
                return 0;
            }
            if (stat == 0) // not connect
            {
                msg = "connecting...";
                return 0;
            }
            msg = string.Format("{0}", tcpcli.svr.saddr);
            GlobalMembersRtkcmn.tracet(2, "consock: connected sock=%d addr=%s\n", tcpcli.svr.sock, tcpcli.svr.saddr);
            tcpcli.svr.state = 2;
            tcpcli.svr.tact = GlobalMembersRtkcmn.tickget();
            return 1;
        }
        /* open tcp client -----------------------------------------------------------*/
        internal static tcpcli_t opentcpcli(string path, ref string msg)
        {
            tcpcli_t tcpcli;
            tcpcli_t tcpcli0 = new tcpcli_t({ 0 });
            string port = "";

            GlobalMembersRtkcmn.tracet(3, "opentcpcli: path=%s\n", path);

            //C++ TO C# CONVERTER TODO TASK: The memory management function 'malloc' has no equivalent in C#:
            if ((tcpcli = (tcpcli_t)malloc(sizeof(tcpcli_t))) == null)
            {
                return null;
            }
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: *tcpcli=tcpcli0;
            tcpcli.CopyFrom(tcpcli0);
            GlobalMembersStream.decodetcppath(path, ref tcpcli.svr.saddr, ref port, null, null, null, null);
            if (sscanf(port, "%d", tcpcli.svr.port) < 1)
            {
                msg = string.Format("port error: {0}", port);
                GlobalMembersRtkcmn.tracet(1, "opentcp: port error port=%s\n", port);
                //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
                free(tcpcli);
                return null;
            }
            tcpcli.svr.tcon = 0;
            tcpcli.toinact = toinact;
            tcpcli.tirecon = ticonnect;
            return tcpcli;
        }
        /* close tcp client ----------------------------------------------------------*/
        internal static void closetcpcli(tcpcli_t tcpcli)
        {
            GlobalMembersRtkcmn.tracet(3, "closetcpcli: sock=%d\n", tcpcli.svr.sock);

            close(tcpcli.svr.sock);
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(tcpcli);
        }
        /* wait socket connect -------------------------------------------------------*/
        internal static int waittcpcli(tcpcli_t tcpcli, ref string msg)
        {
            GlobalMembersRtkcmn.tracet(4, "waittcpcli: sock=%d state=%d\n", tcpcli.svr.sock, tcpcli.svr.state);

            if (tcpcli.svr.state < 0)
            {
                return 0;
            }

            if (tcpcli.svr.state == 0) // close
            {
                if (GlobalMembersStream.gentcp(tcpcli.svr, 1, ref msg) == 0)
                {
                    return 0;
                }
            }
            if (tcpcli.svr.state == 1) // wait
            {
                if (GlobalMembersStream.consock(tcpcli, ref msg) == 0)
                {
                    return 0;
                }
            }
            if (tcpcli.svr.state == 2) // connect
            {
                if (tcpcli.toinact > 0 && (int)(GlobalMembersRtkcmn.tickget() - tcpcli.svr.tact) > tcpcli.toinact)
                {
                    msg = "timeout";
                    GlobalMembersRtkcmn.tracet(2, "waittcpcli: inactive timeout sock=%d\n", tcpcli.svr.sock);
                    GlobalMembersStream.discontcp(tcpcli.svr, tcpcli.tirecon);
                    return 0;
                }
            }
            return 1;
        }
        /* read tcp client -----------------------------------------------------------*/
        internal static int readtcpcli(tcpcli_t tcpcli, ref byte buff, int n, ref string msg)
        {
            int nr;
            int err;

            GlobalMembersRtkcmn.tracet(4, "readtcpcli: sock=%d state=%d n=%d\n", tcpcli.svr.sock, tcpcli.svr.state, n);

            if (GlobalMembersStream.waittcpcli(tcpcli, ref msg) == 0)
            {
                return 0;
            }

            if ((nr = GlobalMembersStream.recv_nb(tcpcli.svr.sock, ref buff, n)) == -1)
            {
                err = GlobalMembersStream.errsock();
                GlobalMembersRtkcmn.tracet(1, "readtcpcli: recv error sock=%d err=%d\n", tcpcli.svr.sock, err);
                msg = string.Format("recv error ({0:D})", err);
                GlobalMembersStream.discontcp(tcpcli.svr, tcpcli.tirecon);
                return 0;
            }
            if (nr > 0)
            {
                tcpcli.svr.tact = GlobalMembersRtkcmn.tickget();
            }
            GlobalMembersRtkcmn.tracet(5, "readtcpcli: exit sock=%d nr=%d\n", tcpcli.svr.sock, nr);
            return nr;
        }
        /* write tcp client ----------------------------------------------------------*/
        internal static int writetcpcli(tcpcli_t tcpcli, ref byte buff, int n, ref string msg)
        {
            int ns;
            int err;

            GlobalMembersRtkcmn.tracet(3, "writetcpcli: sock=%d state=%d n=%d\n", tcpcli.svr.sock, tcpcli.svr.state, n);

            if (GlobalMembersStream.waittcpcli(tcpcli, ref msg) == 0)
            {
                return 0;
            }

            if ((ns = GlobalMembersStream.send_nb(tcpcli.svr.sock, ref buff, n)) == -1)
            {
                err = GlobalMembersStream.errsock();
                GlobalMembersRtkcmn.tracet(1, "writetcp: send error sock=%d err=%d\n", tcpcli.svr.sock, err);
                msg = string.Format("send error ({0:D})", err);
                GlobalMembersStream.discontcp(tcpcli.svr, tcpcli.tirecon);
                return 0;
            }
            if (ns > 0)
            {
                tcpcli.svr.tact = GlobalMembersRtkcmn.tickget();
            }
            GlobalMembersRtkcmn.tracet(5, "writetcpcli: exit sock=%d ns=%d\n", tcpcli.svr.sock, ns);
            return ns;
        }
        /* get state tcp client ------------------------------------------------------*/
        internal static int statetcpcli(tcpcli_t tcpcli)
        {
            return tcpcli != null ? tcpcli.svr.state : 0;
        }
        /* base64 encoder ------------------------------------------------------------*/
        internal static int encbase64(ref string str, byte[] @byte, int n)
        {
            const string table = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
            int i;
            int j;
            int k;
            int b;

            GlobalMembersRtkcmn.tracet(4, "encbase64: n=%d\n", n);

            for (i = j = 0; i / 8 < n;)
            {
                for (k = b = 0; k < 6; k++, i++)
                {
                    b <<= 1;
                    if (i / 8 < n)
                    {
                        b |= (@byte[i / 8] >> (7 - i % 8)) & 0x1;
                    }
                }
                str[j++] = table[b];
            }
            while ((j & 0x3) != 0)
            {
                str[j++] = '=';
            }
            str[j] = '\0';
            GlobalMembersRtkcmn.tracet(5, "encbase64: str=%s\n", str);
            return j;
        }
        /* send ntrip server request -------------------------------------------------*/
        internal static int reqntrip_s(ntrip_t ntrip, ref string msg)
        {
            string buff = new string(new char[256 + DefineConstants.NTRIP_MAXSTR]);
            //C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on this variable, so pointers on this variable are left unchanged:
            sbyte* p = buff;

            GlobalMembersRtkcmn.tracet(3, "reqntrip_s: state=%d\n", ntrip.state);

            p += sprintf(p, "SOURCE %s %s\r\n", ntrip.passwd, ntrip.mntpnt);
            p += sprintf(p, "Source-Agent: NTRIP %s\r\n", "RTKLIB/" DefineConstants.VER_RTKLIB);
            p += sprintf(p, "STR: %s\r\n", ntrip.str);
            p += sprintf(p, "\r\n");

            if (GlobalMembersStream.writetcpcli(ntrip.tcp, ref (byte)buff, p - buff, ref msg) != p - buff)
            {
                return 0;
            }

            GlobalMembersRtkcmn.tracet(2, "reqntrip_s: send request state=%d ns=%d\n", ntrip.state, p - buff);
            GlobalMembersRtkcmn.tracet(5, "reqntrip_s: n=%d buff=\n%s\n", p - buff, buff);
            ntrip.state = 1;
            return 1;
        }
        /* send ntrip client request -------------------------------------------------*/
        internal static int reqntrip_c(ntrip_t ntrip, ref string msg)
        {
            string buff = new string(new char[1024]);
            string user = new string(new char[512]);
            //C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on this variable, so pointers on this variable are left unchanged:
            sbyte* p = buff;

            GlobalMembersRtkcmn.tracet(3, "reqntrip_c: state=%d\n", ntrip.state);

            p += sprintf(p, "GET %s/%s HTTP/1.0\r\n", ntrip.url, ntrip.mntpnt);
            p += sprintf(p, "User-Agent: NTRIP %s\r\n", "RTKLIB/" DefineConstants.VER_RTKLIB);

            if (!*ntrip.user)
            {
                p += sprintf(p, "Accept: */*\r\n");
                p += sprintf(p, "Connection: close\r\n");
            }
            else
            {
                user = string.Format("{0}:{1}", ntrip.user, ntrip.passwd);
                p += sprintf(p, "Authorization: Basic ");
                p += GlobalMembersStream.encbase64(ref p, (byte)user, user.Length);
                p += sprintf(p, "\r\n");
            }
            p += sprintf(p, "\r\n");

            if (GlobalMembersStream.writetcpcli(ntrip.tcp, ref (byte)buff, p - buff, ref msg) != p - buff)
            {
                return 0;
            }

            GlobalMembersRtkcmn.tracet(2, "reqntrip_c: send request state=%d ns=%d\n", ntrip.state, p - buff);
            GlobalMembersRtkcmn.tracet(5, "reqntrip_c: n=%d buff=\n%s\n", p - buff, buff);
            ntrip.state = 1;
            return 1;
        }
        /* test ntrip server response ------------------------------------------------*/
        internal static int rspntrip_s(ntrip_t ntrip, ref string msg)
        {
            int i;
            int nb;
            //C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on this variable, so pointers on this variable are left unchanged:
            sbyte* p;
            //C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on this variable, so pointers on this variable are left unchanged:
            sbyte* q;

            GlobalMembersRtkcmn.tracet(3, "rspntrip_s: state=%d nb=%d\n", ntrip.state, ntrip.nb);
            ntrip.buff[ntrip.nb] = (byte)'0';
            GlobalMembersRtkcmn.tracet(5, "rspntrip_s: n=%d buff=\n%s\n", ntrip.nb, ntrip.buff);

            if ((p = StringFunctions.StrStr((string)ntrip.buff, DefineConstants.NTRIP_RSP_OK_SVR)) != 0) // ok
            {
                q = (string)ntrip.buff;
                p += DefineConstants.NTRIP_RSP_OK_SVR.Length;
                ntrip.nb -= p - q;
                for (i = 0; i < ntrip.nb; i++)
                {
                    *q++ = *p++;
                }
                ntrip.state = 2;
                msg = string.Format("{0}/{1}", ntrip.tcp.svr.saddr, ntrip.mntpnt);
                GlobalMembersRtkcmn.tracet(2, "rspntrip_s: response ok nb=%d\n", ntrip.nb);
                return 1;
            }
            else if ((p = StringFunctions.StrStr((string)ntrip.buff, DefineConstants.NTRIP_RSP_ERROR)) != 0) // error
            {
                nb = ntrip.nb < DefineConstants.MAXSTATMSG ? ntrip.nb : DefineConstants.MAXSTATMSG;
                msg = Convert.ToString((string)ntrip.buff).Substring(0, nb);
                msg = msg.Substring(0, nb);
                GlobalMembersRtkcmn.tracet(1, "rspntrip_s: %s nb=%d\n", msg, ntrip.nb);
                ntrip.nb = 0;
                ntrip.buff[0] = (byte)'\0';
                ntrip.state = 0;
                GlobalMembersStream.discontcp(ntrip.tcp.svr, ntrip.tcp.tirecon);
            }
            else if (ntrip.nb >= DefineConstants.NTRIP_MAXRSP) // buffer overflow
            {
                msg = "response overflow";
                GlobalMembersRtkcmn.tracet(1, "rspntrip_s: response overflow nb=%d\n", ntrip.nb);
                ntrip.nb = 0;
                ntrip.buff[0] = (byte)'\0';
                ntrip.state = 0;
                GlobalMembersStream.discontcp(ntrip.tcp.svr, ntrip.tcp.tirecon);
            }
            GlobalMembersRtkcmn.tracet(5, "rspntrip_s: exit state=%d nb=%d\n", ntrip.state, ntrip.nb);
            return 0;
        }
        /* test ntrip client response ------------------------------------------------*/
        internal static int rspntrip_c(ntrip_t ntrip, ref string msg)
        {
            int i;
            //C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on this variable, so pointers on this variable are left unchanged:
            sbyte* p;
            //C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on this variable, so pointers on this variable are left unchanged:
            sbyte* q;

            GlobalMembersRtkcmn.tracet(3, "rspntrip_c: state=%d nb=%d\n", ntrip.state, ntrip.nb);
            ntrip.buff[ntrip.nb] = (byte)'0';
            GlobalMembersRtkcmn.tracet(5, "rspntrip_c: n=%d buff=\n%s\n", ntrip.nb, ntrip.buff);

            if ((p = StringFunctions.StrStr((string)ntrip.buff, DefineConstants.NTRIP_RSP_OK_CLI)) != 0) // ok
            {
                q = (string)ntrip.buff;
                p += DefineConstants.NTRIP_RSP_OK_CLI.Length;
                ntrip.nb -= p - q;
                for (i = 0; i < ntrip.nb; i++)
                {
                    *q++ = *p++;
                }
                ntrip.state = 2;
                msg = string.Format("{0}/{1}", ntrip.tcp.svr.saddr, ntrip.mntpnt);
                GlobalMembersRtkcmn.tracet(2, "rspntrip_c: response ok nb=%d\n", ntrip.nb);
                return 1;
            }
            if ((p = StringFunctions.StrStr((string)ntrip.buff, DefineConstants.NTRIP_RSP_SRCTBL)) != 0) // source table
            {
                if (!*ntrip.mntpnt) // source table request
                {
                    ntrip.state = 2;
                    msg = "source table received";
                    GlobalMembersRtkcmn.tracet(2, "rspntrip_c: receive source table nb=%d\n", ntrip.nb);
                    return 1;
                }
                msg = "no mountp. reconnect...";
                GlobalMembersRtkcmn.tracet(2, "rspntrip_c: no mount point nb=%d\n", ntrip.nb);
                ntrip.nb = 0;
                ntrip.buff[0] = (byte)'\0';
                ntrip.state = 0;
                GlobalMembersStream.discontcp(ntrip.tcp.svr, ntrip.tcp.tirecon);
            }
            else if ((p = StringFunctions.StrStr((string)ntrip.buff, DefineConstants.NTRIP_RSP_HTTP)) != 0) // http response
            {
                if ((q = StringFunctions.StrChr(p, '\r')) != 0)
                {
                    *q = (sbyte)'\0';
                }
                else
                {
                    ntrip.buff[128] = (byte)'\0';
                }
                msg = p;
                GlobalMembersRtkcmn.tracet(1, "rspntrip_s: %s nb=%d\n", msg, ntrip.nb);
                ntrip.nb = 0;
                ntrip.buff[0] = (byte)'\0';
                ntrip.state = 0;
                GlobalMembersStream.discontcp(ntrip.tcp.svr, ntrip.tcp.tirecon);
            }
            else if (ntrip.nb >= DefineConstants.NTRIP_MAXRSP) // buffer overflow
            {
                msg = "response overflow";
                GlobalMembersRtkcmn.tracet(1, "rspntrip_s: response overflow nb=%d\n", ntrip.nb);
                ntrip.nb = 0;
                ntrip.buff[0] = (byte)'\0';
                ntrip.state = 0;
                GlobalMembersStream.discontcp(ntrip.tcp.svr, ntrip.tcp.tirecon);
            }
            GlobalMembersRtkcmn.tracet(5, "rspntrip_c: exit state=%d nb=%d\n", ntrip.state, ntrip.nb);
            return 0;
        }
        /* wait ntrip request/response -----------------------------------------------*/
        internal static int waitntrip(ntrip_t ntrip, ref string msg)
        {
            int n;
            string p;

            GlobalMembersRtkcmn.tracet(4, "waitntrip: state=%d nb=%d\n", ntrip.state, ntrip.nb);

            if (ntrip.state < 0) // error
            {
                return 0;
            }

            if (ntrip.tcp.svr.state < 2) // tcp disconnected
            {
                ntrip.state = 0;
            }

            if (ntrip.state == 0) // send request
            {
                if (!(ntrip.type == 0 ? GlobalMembersStream.reqntrip_s(ntrip, ref msg) : GlobalMembersStream.reqntrip_c(ntrip, ref msg)))
                {
                    return 0;
                }
                GlobalMembersRtkcmn.tracet(2, "waitntrip: state=%d nb=%d\n", ntrip.state, ntrip.nb);
            }
            if (ntrip.state == 1) // read response
            {
                p = (string)ntrip.buff + ntrip.nb;
                if ((n = GlobalMembersStream.readtcpcli(ntrip.tcp, ref (byte)p, DefineConstants.NTRIP_MAXRSP - ntrip.nb - 1, ref msg)) == 0)
                {
                    GlobalMembersRtkcmn.tracet(5, "waitntrip: readtcp n=%d\n", n);
                    return 0;
                }
                ntrip.nb += n;
                ntrip.buff[ntrip.nb] = (byte)'\0';

                /* wait response */
                return ntrip.type == 0 ? GlobalMembersStream.rspntrip_s(ntrip, ref msg) : GlobalMembersStream.rspntrip_c(ntrip, ref msg);
            }
            return 1;
        }
        /* open ntrip ----------------------------------------------------------------*/
        internal static ntrip_t openntrip(string path, int type, ref string msg)
        {
            ntrip_t ntrip;
            int i;
            string addr = "";
            string port = "";
            string tpath = new string(new char[DefineConstants.MAXSTRPATH]);

            GlobalMembersRtkcmn.tracet(3, "openntrip: path=%s type=%d\n", path, type);

            //C++ TO C# CONVERTER TODO TASK: The memory management function 'malloc' has no equivalent in C#:
            if ((ntrip = (ntrip_t)malloc(sizeof(ntrip_t))) == null)
            {
                return null;
            }

            ntrip.state = 0;
            ntrip.type = type; // 0:server,1:client
            ntrip.nb = 0;
            ntrip.url[0] = '\0';
            ntrip.mntpnt[0] = ntrip.user[0] = ntrip.passwd[0] = ntrip.str[0] = '\0';
            for (i = 0; i < DefineConstants.NTRIP_MAXRSP; i++)
            {
                ntrip.buff[i] = 0;
            }

            /* decode tcp/ntrip path */
            GlobalMembersStream.decodetcppath(path, ref addr, ref port, ref ntrip.user, ref ntrip.passwd, ref ntrip.mntpnt, ref ntrip.str);

            /* use default port if no port specified */
            if (!*port)
            {
                port = string.Format("{0:D}", type != 0 ? DefineConstants.NTRIP_CLI_PORT : DefineConstants.NTRIP_SVR_PORT);
            }
            tpath = string.Format("{0}:{1}", addr, port);

            /* ntrip access via proxy server */
            if (*proxyaddr)
            {
                ntrip.url = string.Format("http://{0}", tpath);
                tpath = proxyaddr;
            }
            /* open tcp client stream */
            if ((ntrip.tcp = GlobalMembersStream.opentcpcli(tpath, ref msg)) == null)
            {
                GlobalMembersRtkcmn.tracet(1, "openntrip: opentcp error\n");
                //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
                free(ntrip);
                return null;
            }
            return ntrip;
        }
        /* close ntrip ---------------------------------------------------------------*/
        internal static void closentrip(ntrip_t ntrip)
        {
            GlobalMembersRtkcmn.tracet(3, "closentrip: state=%d\n", ntrip.state);

            GlobalMembersStream.closetcpcli(ntrip.tcp);
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
            free(ntrip);
        }
        /* read ntrip ----------------------------------------------------------------*/
        internal static int readntrip(ntrip_t ntrip, ref byte buff, int n, ref string msg)
        {
            int nb;

            GlobalMembersRtkcmn.tracet(4, "readntrip: n=%d\n", n);

            if (GlobalMembersStream.waitntrip(ntrip, ref msg) == 0)
            {
                return 0;
            }
            if (ntrip.nb > 0) // read response buffer first
            {
                nb = ntrip.nb <= n != 0 ? ntrip.nb : n;
                //C++ TO C# CONVERTER TODO TASK: The memory management function 'memcpy' has no equivalent in C#:
                memcpy(buff, ntrip.buff + ntrip.nb - nb, nb);
                ntrip.nb = 0;
                return nb;
            }
            return GlobalMembersStream.readtcpcli(ntrip.tcp, ref buff, n, ref msg);
        }
        /* write ntrip ---------------------------------------------------------------*/
        internal static int writentrip(ntrip_t ntrip, ref byte buff, int n, ref string msg)
        {
            GlobalMembersRtkcmn.tracet(3, "writentrip: n=%d\n", n);

            if (GlobalMembersStream.waitntrip(ntrip, ref msg) == 0)
            {
                return 0;
            }
            return GlobalMembersStream.writetcpcli(ntrip.tcp, ref buff, n, ref msg);
        }
        /* get state ntrip -----------------------------------------------------------*/
        internal static int statentrip(ntrip_t ntrip)
        {
            return ntrip == null ? 0 : (ntrip.state == 0 ? ntrip.tcp.svr.state : ntrip.state);
        }
        /* decode ftp path ----------------------------------------------------------*/
        internal static void decodeftppath(string path, ref string addr, ref string file, ref string user, ref string passwd, int[] topts)
        {
            string buff = new string(new char[DefineConstants.MAXSTRPATH]);
            string p;
            string q;

            GlobalMembersRtkcmn.tracet(4, "decodeftpath: path=%s\n", path);

            if (user != 0)
            {
                user = (sbyte)'\0';
            }
            if (passwd != 0)
            {
                passwd = (sbyte)'\0';
            }
            if (topts != 0)
            {
                topts[0] = 0; // time offset in path (s)
                topts[1] = 3600; // download interval (s)
                topts[2] = 0; // download time offset (s)
                topts[3] = 0; // retry interval (s) (0: no retry)
            }
            buff = path;

            if ((p = StringFunctions.StrChr(buff, '/')) != 0)
            {
                if ((q = StringFunctions.StrStr(p.Substring(1), "::")) != 0)
                {
                    q = (sbyte)'\0';
                    if (topts != 0)
                    {
                        sscanf(q.Substring(2), "T=%d,%d,%d,%d", topts, topts + 1, topts + 2, topts + 3);
                    }
                }
                file = p.Substring(1);
                p = (sbyte)'\0';
            }
            else
            {
                file[0] = '\0';
            }

            if ((p = StringFunctions.StrRChr(buff, '@')) != 0)
            {
                p++ = '\0';
                if ((q = StringFunctions.StrChr(buff, ':')) != 0)
                {
                    q = (sbyte)'\0';
                    if (passwd != 0)
                    {
                        passwd = q.Substring(1);
                    }
                }
                q = (sbyte)'\0';
                if (user != 0)
                {
                    user = buff;
                }
            }
            else
            {
                p = buff;
            }

            addr = p;
        }
        /* next download time --------------------------------------------------------*/
        internal static gtime_t nextdltime(int[] topts, int stat)
        {
            gtime_t time = new gtime_t();
            double tow;
            int week;
            int tint;

            GlobalMembersRtkcmn.tracet(3, "nextdltime: topts=%d %d %d %d stat=%d\n", topts[0], topts[1], topts[2], topts[3], stat);

            /* current time (gpst) */
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: time=utc2gpst(timeget());
            time.CopyFrom(GlobalMembersRtkcmn.utc2gpst(GlobalMembersRtkcmn.timeget()));
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: tow=time2gpst(time,&week);
            tow = GlobalMembersRtkcmn.time2gpst(new gtime_t(time), ref week);

            /* next retry time */
            if (stat == 0 && topts[3] > 0)
            {
                tow = (Math.Floor((tow - topts[2]) / topts[3]) + 1.0) * topts[3] + topts[2];
                return GlobalMembersRtkcmn.gpst2time(week, tow);
            }
            /* next interval time */
            tint = topts[1] <= 0 ? 3600 : topts[1];
            tow = (Math.Floor((tow - topts[2]) / tint) + 1.0) * tint + topts[2];
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: time=gpst2time(week,tow);
            time.CopyFrom(GlobalMembersRtkcmn.gpst2time(week, tow));

            return time;
        }
        /* ftp thread ----------------------------------------------------------------*/
#if WIN32
//C++ TO C# CONVERTER NOTE: WINAPI is not available in C#:
//ORIGINAL LINE: static uint WINAPI ftpthread(object* arg)
	internal static uint ftpthread(object arg)
#else
        internal static object ftpthread(object arg)
#endif
        {
            ftp_t ftp = (ftp_t)arg;
            FILE fp;
            gtime_t time = new gtime_t();
            string remote = new string(new char[1024]);
            string local = new string(new char[1024]);
            string tmpfile = new string(new char[1024]);
            string errfile = new string(new char[1024]);
            string p;
            string cmd = new string(new char[2048]);
            string env = "";
            string opt = new string(new char[1024]);
            string proxyopt = "";
            string proto;
            int ret;

            GlobalMembersRtkcmn.tracet(3, "ftpthread:\n");

            if (!*localdir)
            {
                GlobalMembersRtkcmn.tracet(1, "no local directory\n");
                ftp.error = 11;
                ftp.state = 3;
                return 0;
            }
            /* replace keyword in file path and local path */
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: time=timeadd(utc2gpst(timeget()),ftp->topts[0]);
            time.CopyFrom(GlobalMembersRtkcmn.timeadd(GlobalMembersRtkcmn.utc2gpst(GlobalMembersRtkcmn.timeget()), ftp.topts[0]));
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: reppath(ftp->file,remote,time,"","");
            GlobalMembersRtkcmn.reppath(ftp.file, ref remote, new gtime_t(time), "", "");

            if ((p = StringFunctions.StrRChr(remote, '/')) != 0)
            {
                p = p.Substring(1);
            }
            else
            {
                p = remote;
            }
            local = string.Format("{0}{1}{2}", localdir, DefineConstants.FILEPATHSEP, p);
            errfile = string.Format("{0}.err", local);

            /* if local file exist, skip download */
            tmpfile = local;
            if ((p = StringFunctions.StrRChr(tmpfile, '.')) != 0 && (!string.Compare(p, ".z") || !string.Compare(p, ".gz") || !string.Compare(p, ".zip") || !string.Compare(p, ".Z") || !string.Compare(p, ".GZ") || !string.Compare(p, ".ZIP")))
            {
                p = (sbyte)'\0';
            }
            if ((fp = fopen(tmpfile, "rb")) != null)
            {
                fclose(fp);
                ftp.local = tmpfile;
                GlobalMembersRtkcmn.tracet(3, "ftpthread: file exists %s\n", ftp.local);
                ftp.state = 2;
                return 0;
            }
            /* proxy settings for wget (ref [2]) */
            if (*proxyaddr)
            {
                proto = ftp.proto != 0 ? "http" : "ftp";
                env = string.Format("set {0}_proxy=http://{1} & ", proto, proxyaddr);
                proxyopt = "--proxy=on ";
            }
            /* download command (ref [2]) */
            if (ftp.proto == 0) // ftp
            {
                opt = string.Format("--ftp-user={0} --ftp-password={1} --glob=off --passive-ftp {2}-t 1 -T {3:D} -O \"{4}\"", ftp.user, ftp.passwd, proxyopt, DefineConstants.FTP_TIMEOUT, local);
                cmd = string.Format("{0}{1} {2} \"ftp://{3}/{4}\" 2> \"{5}\"\n", env, DefineConstants.FTP_CMD, opt, ftp.addr, remote, errfile);
            }
            else // http
            {
                opt = string.Format("{0}-t 1 -T {1:D} -O \"{2}\"", proxyopt, DefineConstants.FTP_TIMEOUT, local);
                cmd = string.Format("{0}{1} {2} \"http://{3}/{4}\" 2> \"{5}\"\n", env, DefineConstants.FTP_CMD, opt, ftp.addr, remote, errfile);
            }
            /* execute download command */
            if ((ret = GlobalMembersRtkcmn.execcmd(cmd)) != 0)
            {
                remove(local);
                GlobalMembersRtkcmn.tracet(1, "execcmd error: cmd=%s ret=%d\n", cmd, ret);
                ftp.error = ret;
                ftp.state = 3;
                return 0;
            }
            remove(errfile);

            /* uncompress downloaded file */
            if ((p = StringFunctions.StrRChr(local, '.')) != 0 && (!string.Compare(p, ".z") || !string.Compare(p, ".gz") || !string.Compare(p, ".zip") || !string.Compare(p, ".Z") || !string.Compare(p, ".GZ") || !string.Compare(p, ".ZIP")))
            {

                if (GlobalMembersRtkcmn.uncompress(local, ref tmpfile) != 0)
                {
                    remove(local);
                    local = tmpfile;
                }
                else
                {
                    GlobalMembersRtkcmn.tracet(1, "file uncompact error: %s\n", local);
                    ftp.error = 12;
                    ftp.state = 3;
                    return 0;
                }
            }
            ftp.local = local;
            ftp.state = 2; // ftp completed

            GlobalMembersRtkcmn.tracet(3, "ftpthread: complete cmd=%s\n", cmd);
            return 0;
        }
        /* open ftp ------------------------------------------------------------------*/
        internal static ftp_t openftp(string path, int type, ref string msg)
        {
            ftp_t ftp;

            GlobalMembersRtkcmn.tracet(3, "openftp: path=%s type=%d\n", path, type);

            msg[0] = '\0';

            //C++ TO C# CONVERTER TODO TASK: The memory management function 'malloc' has no equivalent in C#:
            if ((ftp = (ftp_t)malloc(sizeof(ftp_t))) == null)
            {
                return null;
            }

            ftp.state = 0;
            ftp.proto = type;
            ftp.error = 0;
            ftp.thread = 0;
            ftp.local[0] = '\0';

            /* decode ftp path */
            GlobalMembersStream.decodeftppath(path, ref ftp.addr, ref ftp.file, ref ftp.user, ref ftp.passwd, ftp.topts);

            /* set first download time */
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: ftp->tnext=timeadd(timeget(),10.0);
            ftp.tnext.CopyFrom(GlobalMembersRtkcmn.timeadd(GlobalMembersRtkcmn.timeget(), 10.0));

            return ftp;
        }
        /* close ftp -----------------------------------------------------------------*/
        internal static void closeftp(ftp_t ftp)
        {
            GlobalMembersRtkcmn.tracet(3, "closeftp: state=%d\n", ftp.state);

            if (ftp.state != 1)
            {
                //C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
                free(ftp);
            }
        }
        /* read ftp ------------------------------------------------------------------*/
        internal static int readftp(ftp_t ftp, byte[] buff, int n, ref string msg)
        {
            gtime_t time = new gtime_t();
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: byte *p,*q;
            byte p;
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: byte *q;
            byte q;

            GlobalMembersRtkcmn.tracet(4, "readftp: n=%d\n", n);

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: time=utc2gpst(timeget());
            time.CopyFrom(GlobalMembersRtkcmn.utc2gpst(GlobalMembersRtkcmn.timeget()));

            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
            //ORIGINAL LINE: if (timediff(time,ftp->tnext)<0.0)
            if (GlobalMembersRtkcmn.timediff(new gtime_t(time), new gtime_t(ftp.tnext)) < 0.0) // until download time?
            {
                return 0;
            }
            if (ftp.state <= 0) // ftp/http not executed?
            {
                ftp.state = 1;
                msg = string.Format("{0}://{1}", ftp.proto != 0 ? "http" : "ftp", ftp.addr);

#if WIN32
			if (!(ftp.thread = CreateThread(null,0,GlobalMembersStream.ftpthread,ftp,0,null)))
			{
#else
                if (pthread_create(ftp.thread, null, GlobalMembersStream.ftpthread, ftp))
                {
#endif
                    GlobalMembersRtkcmn.tracet(1, "readftp: ftp thread create error\n");
                    ftp.state = 3;
                    msg = "ftp thread error";
                    return 0;
                }
            }
            if (ftp.state <= 1) // ftp/http on going?
            {
                return 0;
            }

            if (ftp.state == 3) // ftp error
            {
                msg = string.Format("{0} error ({1:D})", ftp.proto != 0 ? "http" : "ftp", ftp.error);

                /* set next retry time */
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
                //ORIGINAL LINE: ftp->tnext=nextdltime(ftp->topts,0);
                ftp.tnext.CopyFrom(GlobalMembersStream.nextdltime(ftp.topts, 0));
                ftp.state = 0;
                return 0;
            }
            /* return local file path if ftp completed */
            p = buff;
            q = (byte)ftp.local;
            while (q != 0 && (int)(p - buff) < n)
            {
                p++ = q++;
            }
            p += sprintf((string)p, "\r\n");

            /* set next download time */
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: ftp->tnext=nextdltime(ftp->topts,1);
            ftp.tnext.CopyFrom(GlobalMembersStream.nextdltime(ftp.topts, 1));
            ftp.state = 0;

            msg = "";

            return (int)(p - buff);
        }
        /* get state ftp -------------------------------------------------------------*/
        internal static int stateftp(ftp_t ftp)
        {
            return ftp == null ? 0 : (ftp.state == 0 ? 2 : (ftp.state <= 2 ? 3 : -1));
        }
        /* initialize stream environment -----------------------------------------------
        * initialize stream environment
        * args   : none
        * return : none
        *-----------------------------------------------------------------------------*/
        public static void strinitcom()
        {
#if WIN32
		WSADATA data = new WSADATA();
#endif
            GlobalMembersRtkcmn.tracet(3, "strinitcom:\n");

#if WIN32
		WSAStartup(MAKEWORD(2,0), data);
#endif
        }
        /* initialize stream -----------------------------------------------------------
        * initialize stream struct
        * args   : stream_t *stream IO  stream
        * return : none
        *-----------------------------------------------------------------------------*/
        public static void strinit(stream_t stream)
        {
            GlobalMembersRtkcmn.tracet(3, "strinit:\n");

            stream.type = 0;
            stream.mode = 0;
            stream.state = 0;
            stream.inb = stream.inr = stream.outb = stream.outr = 0;
            stream.tick = stream.tact = stream.inbt = stream.outbt = 0;
#if initlock_ConditionalDefinition1
		InitializeCriticalSection(stream.@lock);
#elif initlock_ConditionalDefinition2
		pthread_mutex_init(stream.@lock, null);
#else
            initlock(stream.@lock);
#endif
            stream.port = null;
            stream.path[0] = '\0';
            stream.msg[0] = '\0';
        }
        /* open stream -----------------------------------------------------------------
        * open stream for read or write
        * args   : stream_t *stream IO  stream
        *          int type         I   stream type (STR_SERIAL,STR_FILE,STR_TCPSVR,...)
        *          int mode         I   stream mode (STR_MODE_???)
        *          char *path       I   stream path (see below)
        * return : status (0:error,1:ok)
        * notes  : see reference [1] for NTRIP
        *          STR_FTP/HTTP needs "wget" in command search paths
        *
        * stream path ([] options):
        *
        *   STR_SERIAL   port[:brate[:bsize[:parity[:stopb[:fctr]]]]]
        *                    port  = COM?? (windows), tty??? (linuex, omit /dev/)
        *                    brate = bit rate     (bps)
        *                    bsize = bit size     (7|8)
        *                    parity= parity       (n|o|e)
        *                    stopb = stop bits    (1|2)
        *                    fctr  = flow control (off|rts)
        *   STR_FILE     file_path[::T][::+start][::xseppd][::S=swap]
        *                    ::T   = enable time tag
        *                    start = replay start offset (s)
        *                    speed = replay speed factor
        *                    swap  = output swap interval (hr) (0: no swap)
        *   STR_TCPSVR   :port
        *   STR_TCPCLI   address:port
        *   STR_NTRIPSVR user[:passwd]@address[:port]/moutpoint[:string]
        *   STR_NTRIPCLI [user[:passwd]]@address[:port][/mountpoint]
        *   STR_FTP      [user[:passwd]]@address/file_path[::T=poff[,tint[,toff,tret]]]]
        *   STR_HTTP     address/file_path[::T=poff[,tint[,toff,tret]]]]
        *                    poff  = time offset for path extension (s)
        *                    tint  = download interval (s)
        *                    toff  = download time offset (s)
        *                    tret  = download retry interval (s) (0:no retry)
        *-----------------------------------------------------------------------------*/
        public static int stropen(stream_t stream, int type, int mode, string path)
        {
            GlobalMembersRtkcmn.tracet(3, "stropen: type=%d mode=%d path=%s\n", type, mode, path);

            stream.type = type;
            stream.mode = mode;
            stream.path = path;
            stream.inb = stream.inr = stream.outb = stream.outr = 0;
            stream.tick = GlobalMembersRtkcmn.tickget();
            stream.inbt = stream.outbt = 0;
            stream.msg[0] = '\0';
            stream.port = null;
            switch (type)
            {
                case DefineConstants.STR_SERIAL:
                    stream.port = GlobalMembersStream.openserial(path, mode, ref stream.msg);
                    break;
                case DefineConstants.STR_FILE:
                    stream.port = GlobalMembersConvrnx.openfile(path, mode, stream.msg);
                    break;
                case DefineConstants.STR_TCPSVR:
                    stream.port = GlobalMembersStream.opentcpsvr(path, ref stream.msg);
                    break;
                case DefineConstants.STR_TCPCLI:
                    stream.port = GlobalMembersStream.opentcpcli(path, ref stream.msg);
                    break;
                case DefineConstants.STR_NTRIPSVR:
                    stream.port = GlobalMembersStream.openntrip(path, 0, ref stream.msg);
                    break;
                case DefineConstants.STR_NTRIPCLI:
                    stream.port = GlobalMembersStream.openntrip(path, 1, ref stream.msg);
                    break;
                case DefineConstants.STR_FTP:
                    stream.port = GlobalMembersStream.openftp(path, 0, ref stream.msg);
                    break;
                case DefineConstants.STR_HTTP:
                    stream.port = GlobalMembersStream.openftp(path, 1, ref stream.msg);
                    break;
                default:
                    stream.state = 0;
                    return 1;
            }
            stream.state = stream.port == null ? -1 : 1;
            return stream.port != null;
        }
        /* close stream ----------------------------------------------------------------
        * close stream
        * args   : stream_t *stream IO  stream
        * return : none
        *-----------------------------------------------------------------------------*/
        public static void strclose(stream_t stream)
        {
            GlobalMembersRtkcmn.tracet(3, "strclose: type=%d mode=%d\n", stream.type, stream.mode);

            if (stream.port != null)
            {
                switch (stream.type)
                {
                    case DefineConstants.STR_SERIAL:
                        GlobalMembersStream.closeserial((serial_t)stream.port);
                        break;
                    case DefineConstants.STR_FILE:
                        GlobalMembersConvrnx.closefile((file_t)stream.port);
                        break;
                    case DefineConstants.STR_TCPSVR:
                        GlobalMembersStream.closetcpsvr((tcpsvr_t)stream.port);
                        break;
                    case DefineConstants.STR_TCPCLI:
                        GlobalMembersStream.closetcpcli((tcpcli_t)stream.port);
                        break;
                    case DefineConstants.STR_NTRIPSVR:
                        GlobalMembersStream.closentrip((ntrip_t)stream.port);
                        break;
                    case DefineConstants.STR_NTRIPCLI:
                        GlobalMembersStream.closentrip((ntrip_t)stream.port);
                        break;
                    case DefineConstants.STR_FTP:
                        GlobalMembersStream.closeftp((ftp_t)stream.port);
                        break;
                    case DefineConstants.STR_HTTP:
                        GlobalMembersStream.closeftp((ftp_t)stream.port);
                        break;
                }
            }
            else
            {
                GlobalMembersRtkcmn.trace(2, "no port to close stream: type=%d\n", stream.type);
            }
            stream.type = 0;
            stream.mode = 0;
            stream.state = 0;
            stream.inr = stream.outr = 0;
            stream.path[0] = '\0';
            stream.msg[0] = '\0';
            stream.port = null;
        }
        /* sync streams ----------------------------------------------------------------
        * sync time for streams
        * args   : stream_t *stream1 IO stream 1
        *          stream_t *stream2 IO stream 2
        * return : none
        * notes  : for replay files with time tags
        *-----------------------------------------------------------------------------*/
        public static void strsync(stream_t stream1, stream_t stream2)
        {
            file_t file1;
            file_t file2;
            if (stream1.type != DefineConstants.STR_FILE || stream2.type != DefineConstants.STR_FILE)
                return;
            file1 = (file_t)stream1.port;
            file2 = (file_t)stream2.port;
            if (file1 != null && file2 != null)
            {
                GlobalMembersStream.syncfile(file1, file2);
            }
        }
        /* lock/unlock stream ----------------------------------------------------------
        * lock/unlock stream
        * args   : stream_t *stream I  stream
        * return : none
        *-----------------------------------------------------------------------------*/
        public static void strlock(stream_t stream)
        {
#if lock_ConditionalDefinition1
		EnterCriticalSection(stream.@lock);
#elif lock_ConditionalDefinition2
		pthread_mutex_lock(stream.@lock);
#else
            lock (stream.lock) ;
#endif
        }
        public static void strunlock(stream_t stream)
        {
#if unlock_ConditionalDefinition1
		LeaveCriticalSection(stream.@lock);
#elif unlock_ConditionalDefinition2
		pthread_mutex_unlock(stream.@lock);
#else
            unlock(stream.@lock);
#endif
        }

        /* read stream -----------------------------------------------------------------
        * read data from stream (unblocked)
        * args   : stream_t *stream I  stream
        *          unsinged char *buff O data buffer
        *          int    n         I  maximum data length
        * return : read data length
        * notes  : if no data, return immediately with no data
        *-----------------------------------------------------------------------------*/
        public static int strread(stream_t stream, ref byte buff, int n)
        {
            uint tick;
            string msg = stream.msg;
            int nr;

            GlobalMembersRtkcmn.tracet(4, "strread: n=%d\n", n);

            if (!(stream.mode & DefineConstants.STR_MODE_R) || stream.port == null)
            {
                return 0;
            }

            strlock(stream);

            switch (stream.type)
            {
                case DefineConstants.STR_SERIAL:
                    nr = GlobalMembersStream.readserial((serial_t)stream.port, ref buff, n, ref msg);
                    break;
                case DefineConstants.STR_FILE:
                    nr = GlobalMembersStream.readfile((file_t)stream.port, ref buff, n, ref msg);
                    break;
                case DefineConstants.STR_TCPSVR:
                    nr = GlobalMembersStream.readtcpsvr((tcpsvr_t)stream.port, ref buff, n, ref msg);
                    break;
                case DefineConstants.STR_TCPCLI:
                    nr = GlobalMembersStream.readtcpcli((tcpcli_t)stream.port, ref buff, n, ref msg);
                    break;
                case DefineConstants.STR_NTRIPCLI:
                    nr = GlobalMembersStream.readntrip((ntrip_t)stream.port, ref buff, n, ref msg);
                    break;
                case DefineConstants.STR_FTP:
                    nr = GlobalMembersStream.readftp((ftp_t)stream.port, buff, n, ref msg);
                    break;
                case DefineConstants.STR_HTTP:
                    nr = GlobalMembersStream.readftp((ftp_t)stream.port, buff, n, ref msg);
                    break;
                default:
                    strunlock(stream);
                    return 0;
            }
            stream.inb += nr;
            tick = GlobalMembersRtkcmn.tickget();
            if (nr > 0)
            {
                stream.tact = tick;
            }

            if ((int)(tick - stream.tick) >= tirate)
            {
                stream.inr = (stream.inb - stream.inbt) * 8000 / (tick - stream.tick);
                stream.tick = tick;
                stream.inbt = stream.inb;
            }
            strunlock(stream);
            return nr;
        }
        /* write stream ----------------------------------------------------------------
        * write data to stream (unblocked)
        * args   : stream_t *stream I   stream
        *          unsinged char *buff I data buffer
        *          int    n         I   data length
        * return : status (0:error,1:ok)
        * notes  : write data to buffer and return immediately
        *-----------------------------------------------------------------------------*/
        public static int strwrite(stream_t stream, ref byte buff, int n)
        {
            uint tick;
            string msg = stream.msg;
            int ns;

            GlobalMembersRtkcmn.tracet(3, "strwrite: n=%d\n", n);

            if (!(stream.mode & DefineConstants.STR_MODE_W) || stream.port == null)
            {
                return 0;
            }

            strlock(stream);

            switch (stream.type)
            {
                case DefineConstants.STR_SERIAL:
                    ns = GlobalMembersStream.writeserial((serial_t)stream.port, ref buff, n, ref msg);
                    break;
                case DefineConstants.STR_FILE:
                    ns = GlobalMembersStream.writefile((file_t)stream.port, ref buff, n, ref msg);
                    break;
                case DefineConstants.STR_TCPSVR:
                    ns = GlobalMembersStream.writetcpsvr((tcpsvr_t)stream.port, ref buff, n, ref msg);
                    break;
                case DefineConstants.STR_TCPCLI:
                    ns = GlobalMembersStream.writetcpcli((tcpcli_t)stream.port, ref buff, n, ref msg);
                    break;
                case DefineConstants.STR_NTRIPCLI:
                case DefineConstants.STR_NTRIPSVR:
                    ns = GlobalMembersStream.writentrip((ntrip_t)stream.port, ref buff, n, ref msg);
                    break;
                case DefineConstants.STR_FTP:
                case DefineConstants.STR_HTTP:
                default:
                    strunlock(stream);
                    return 0;
            }
            stream.outb += ns;
            tick = GlobalMembersRtkcmn.tickget();
            if (ns > 0)
            {
                stream.tact = tick;
            }

            if ((int)(tick - stream.tick) > tirate)
            {
                stream.outr = (stream.outb - stream.outbt) * 8000 / (tick - stream.tick);
                stream.tick = tick;
                stream.outbt = stream.outb;
            }
            strunlock(stream);
            return ns;
        }
        /* get stream status -----------------------------------------------------------
        * get stream status
        * args   : stream_t *stream I   stream
        *          char   *msg      IO  status message (NULL: no output)
        * return : status (-1:error,0:close,1:wait,2:connect,3:active)
        *-----------------------------------------------------------------------------*/
        public static int strstat(stream_t stream, ref string msg)
        {
            int state;

            GlobalMembersRtkcmn.tracet(4, "strstat:\n");

            strlock(stream);
            if (msg != 0)
            {
                msg = stream.msg.Substring(0, DefineConstants.MAXSTRMSG - 1);
                msg[DefineConstants.MAXSTRMSG - 1] = '\0';
            }
            if (stream.port == null)
            {
                strunlock(stream);
                return stream.state;
            }
            switch (stream.type)
            {
                case DefineConstants.STR_SERIAL:
                    state = GlobalMembersStream.stateserial((serial_t)stream.port);
                    break;
                case DefineConstants.STR_FILE:
                    state = GlobalMembersStream.statefile((file_t)stream.port);
                    break;
                case DefineConstants.STR_TCPSVR:
                    state = GlobalMembersStream.statetcpsvr((tcpsvr_t)stream.port);
                    break;
                case DefineConstants.STR_TCPCLI:
                    state = GlobalMembersStream.statetcpcli((tcpcli_t)stream.port);
                    break;
                case DefineConstants.STR_NTRIPSVR:
                case DefineConstants.STR_NTRIPCLI:
                    state = GlobalMembersStream.statentrip((ntrip_t)stream.port);
                    break;
                case DefineConstants.STR_FTP:
                    state = stateftp((ftp_t)stream.port);
                    break;
                case DefineConstants.STR_HTTP:
                    state = stateftp((ftp_t)stream.port);
                    break;
                default:
                    strunlock(stream);
                    return 0;
            }
            if (state == 2 && (int)(GlobalMembersRtkcmn.tickget() - stream.tact) <= DefineConstants.TINTACT)
            {
                state = 3;
            }
            strunlock(stream);
            return state;
        }
        /* get stream statistics summary -----------------------------------------------
        * get stream statistics summary
        * args   : stream_t *stream I   stream
        *          int    *inb      IO   bytes of input  (NULL: no output)
        *          int    *inr      IO   bps of input    (NULL: no output)
        *          int    *outb     IO   bytes of output (NULL: no output)
        *          int    *outr     IO   bps of output   (NULL: no output)
        * return : none
        *-----------------------------------------------------------------------------*/
        public static void strsum(stream_t stream, ref int inb, ref int inr, ref int outb, ref int outr)
        {
            GlobalMembersRtkcmn.tracet(4, "strsum:\n");

            strlock(stream);
            if (inb != 0)
            {
                inb = stream.inb;
            }
            if (inr != 0)
            {
                inr = stream.inr;
            }
            if (outb != 0)
            {
                outb = stream.outb;
            }
            if (outr != 0)
            {
                outr = stream.outr;
            }
            strunlock(stream);
        }
        /* set global stream options ---------------------------------------------------
        * set global stream options
        * args   : int    *opt      I   options
        *              opt[0]= inactive timeout (ms) (0: no timeout)
        *              opt[1]= interval to reconnect (ms)
        *              opt[2]= averaging time of data rate (ms)
        *              opt[3]= receive/send buffer size (bytes);
        *              opt[4]= file swap margin (s)
        *              opt[5]= reserved
        *              opt[6]= reserved
        *              opt[7]= reserved
        * return : none
        *-----------------------------------------------------------------------------*/
        public static void strsetopt(int[] opt)
        {
            GlobalMembersRtkcmn.tracet(3, "strsetopt: opt=%d %d %d %d %d %d %d %d\n", opt[0], opt[1], opt[2], opt[3], opt[4], opt[5], opt[6], opt[7]);

            toinact = 0 < opt[0] && opt[0] < 1000 ? 1000 : opt[0]; // >=1s
            ticonnect = opt[1] < 1000 ? 1000 : opt[1]; // >=1s
            tirate = opt[2] < 100 ? 100 : opt[2]; // >=0.1s
            buffsize = opt[3] < 4096 ? 4096 : opt[3]; // >=4096byte
            fswapmargin = opt[4] < 0 ? 0 : opt[4];
        }
        /* set timeout time ------------------------------------------------------------
        * set timeout time
        * args   : stream_t *stream I   stream (STR_TCPCLI,STR_NTRIPCLI,STR_NTRIPSVR)
        *          int     toinact  I   inactive timeout (ms) (0: no timeout)
        *          int     tirecon  I   reconnect interval (ms) (0: no reconnect)
        * return : none
        *-----------------------------------------------------------------------------*/
        public static void strsettimeout(stream_t stream, int toinact, int tirecon)
        {
            tcpcli_t tcpcli;

            GlobalMembersRtkcmn.tracet(3, "strsettimeout: toinact=%d tirecon=%d\n", toinact, tirecon);

            if (stream.type == DefineConstants.STR_TCPCLI)
            {
                tcpcli = (tcpcli_t)stream.port;
            }
            else if (stream.type == DefineConstants.STR_NTRIPCLI || stream.type == DefineConstants.STR_NTRIPSVR)
            {
                tcpcli = ((ntrip_t)stream.port).tcp;
            }
            else
                return;

            tcpcli.toinact = toinact;
            tcpcli.tirecon = tirecon;
        }
        /* set local directory ---------------------------------------------------------
        * set local directory path for ftp/http download
        * args   : char   *dir      I   directory for download files
        * return : none
        *-----------------------------------------------------------------------------*/
        public static void strsetdir(string dir)
        {
            GlobalMembersRtkcmn.tracet(3, "strsetdir: dir=%s\n", dir);

            localdir = dir;
        }
        /* set http/ntrip proxy address ------------------------------------------------
        * set http/ntrip proxy address
        * args   : char   *addr     I   http/ntrip proxy address <address>:<port>
        * return : none
        *-----------------------------------------------------------------------------*/
        public static void strsetproxy(string addr)
        {
            GlobalMembersRtkcmn.tracet(3, "strsetproxy: addr=%s\n", addr);

            proxyaddr = addr;
        }
        /* get stream time -------------------------------------------------------------
        * get stream time
        * args   : stream_t *stream I   stream
        * return : current time or replay time for playback file
        *-----------------------------------------------------------------------------*/
        public static gtime_t strgettime(stream_t stream)
        {
            file_t file;
            if (stream.type == DefineConstants.STR_FILE && (stream.mode & DefineConstants.STR_MODE_R) && (file = (file_t)stream.port) != null)
            {
                //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                //ORIGINAL LINE: return timeadd(file->time,file->start);
                return GlobalMembersRtkcmn.timeadd(new gtime_t(file.time), file.start); // replay start time
            }
            return GlobalMembersRtkcmn.utc2gpst(GlobalMembersRtkcmn.timeget());
        }
        /* send nmea request -----------------------------------------------------------
        * send nmea gpgga message to stream
        * args   : stream_t *stream I   stream
        *          double *pos      I   position {x,y,z} (ecef) (m)
        * return : none
        *-----------------------------------------------------------------------------*/
        public static void strsendnmea(stream_t stream, double[] pos)
        {
            sol_t sol = new sol_t({ 0 });
            byte[] buff = new byte[1024];
            int i;
            int n;

            GlobalMembersRtkcmn.tracet(3, "strsendnmea: pos=%.3f %.3f %.3f\n", pos[0], pos[1], pos[2]);

            sol.stat = DefineConstants.SOLQ_SINGLE;
            //C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
            //ORIGINAL LINE: sol.time=utc2gpst(timeget());
            sol.time.CopyFrom(GlobalMembersRtkcmn.utc2gpst(GlobalMembersRtkcmn.timeget()));
            for (i = 0; i < 3; i++)
            {
                sol.rr[i] = pos[i];
            }
            n = GlobalMembersSolution.outnmea_gga(ref buff, sol);
            strwrite(stream, buff, n);
        }
        /* generate general hex message ----------------------------------------------*/
        internal static int gen_hex(string msg, ref byte buff)
        {
            //C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
            //ORIGINAL LINE: byte *q=buff;
            byte q = buff;
            string mbuff = "";
            string[] args = new string[256];
            string p;
            uint @byte;
            int i;
            int narg = 0;

            GlobalMembersRtkcmn.trace(4, "gen_hex: msg=%s\n", msg);

            mbuff = msg.Substring(0, 1023);
            for (p = StringFunctions.StrTok(mbuff, " "); p && narg < 256; p = StringFunctions.StrTok(null, " "))
            {
                args[narg++] = p;
            }
            for (i = 0; i < narg; i++)
            {
                if (sscanf(args[i], "%x", @byte))
                {
                    q++ = (byte)@byte;
                }
            }
            return (int)(q - buff);
        }
        /* send receiver command -------------------------------------------------------
        * send receiver commands to stream
        * args   : stream_t *stream I   stream
        *          char   *cmd      I   receiver command strings
        * return : none
        *-----------------------------------------------------------------------------*/
        public static void strsendcmd(stream_t str, string cmd)
        {
            byte[] buff = new byte[1024];
            string p = cmd;
            //C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on this variable, so pointers on this variable are left unchanged:
            sbyte* q;
            string msg = new string(new char[1024]);
            string cmdend = "\r\n";
            int n;
            int m;
            int ms;

            GlobalMembersRtkcmn.tracet(3, "strsendcmd: cmd=%s\n", cmd);

            for (;;)
            {
                for (q = p; ; q++)
                {
                    if (*q == '\r' || *q == '\n' || *q == '\0')
                        break;
                }
                n = (int)(q - p);
                msg = p.Substring(0, n);
                msg[n] = '\0';

                if (!*msg || *msg == '#') // null or comment
                {
                    ;
                }
                else if (*msg == '!') // binary escape
                {

                    if (!string.Compare(msg, 1, "WAIT", 0, 4)) // wait
                    {
                        if (sscanf(msg.Substring(5), "%d", ms) < 1)
                        {
                            ms = 100;
                        }
                        if (ms > 3000) // max 3 s
                        {
                            ms = 3000;
                        }
                        GlobalMembersRtkcmn.sleepms(ms);
                    }
                    else if (!string.Compare(msg, 1, "UBX", 0, 3)) // ublox
                    {
                        if ((m = GlobalMembersUblox.gen_ubx(msg.Substring(4), ref buff)) > 0)
                        {
                            strwrite(str, buff, m);
                        }
                    }
                    else if (!string.Compare(msg, 1, "STQ", 0, 3)) // skytraq
                    {
                        if ((m = GlobalMembersSkytraq.gen_stq(msg.Substring(4), ref buff)) > 0)
                        {
                            strwrite(str, buff, m);
                        }
                    }
                    else if (!string.Compare(msg, 1, "NVS", 0, 3)) // nvs
                    {
                        if ((m = GlobalMembersNvs.gen_nvs(msg.Substring(4), ref buff)) > 0)
                        {
                            strwrite(str, buff, m);
                        }
                    }
                    else if (!string.Compare(msg, 1, "LEXR", 0, 4)) // lex receiver
                    {
                        if ((m = GlobalMembersRcvlex.gen_lexr(msg.Substring(5), ref buff)) > 0)
                        {
                            strwrite(str, buff, m);
                        }
                    }
                    else if (!string.Compare(msg, 1, "HEX", 0, 3)) // general hex message
                    {
                        if ((m = gen_hex(msg.Substring(4), buff)) > 0)
                        {
                            strwrite(str, buff, m);
                        }
                    }
                }
                else
                {
                    strwrite(str, (byte)msg, n);
                    strwrite(str, (byte)cmdend, 2);
                }
                if (*q == '\0')
                    break;
                else
                {
                    p = q + 1;
                }
            }
        }
    }

    /* type definition -----------------------------------------------------------*/

    public class serial_t // serial control type
    {
#if dev_t_ConditionalDefinition1
	public System.IntPtr dev;
#elif dev_t_ConditionalDefinition2
	public int dev;
#else
        public dev_t dev = new dev_t();
#endif
        public int error; // error state
#if WIN32
	public int state; // state,write/read pointer
	public int wp;
	public int rp;
	public int buffsize; // write buffer size (bytes)
	public System.IntPtr thread; // write thread
#if lock_t_ConditionalDefinition1
	public CRITICAL_SECTION @lock = new CRITICAL_SECTION();
#elif lock_t_ConditionalDefinition2
	public pthread_mutex_t @lock = new pthread_mutex_t();
#else
	public lock_t @lock = new lock_t();
#endif
//C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent for pointers to value types:
//ORIGINAL LINE: byte *buff;
	public byte buff; // write buffer
#endif
    }

    public class file_t // file control type
    {
        public FILE fp; // file pointer
        public FILE fp_tag; // file pointer of tag file
        public FILE fp_tmp; // temporary file pointer for swap
        public FILE fp_tag_tmp; // temporary file pointer of tag file for swap
        public string path = new string(new char[DefineConstants.MAXSTRPATH]); // file path
        public string openpath = new string(new char[DefineConstants.MAXSTRPATH]); // open file path
        public int mode; // file mode
        public int timetag; // time tag flag (0:off,1:on)
        public int repmode; // replay mode (0:master,1:slave)
        public int offset; // time offset (ms) for slave
        public gtime_t time = new gtime_t(); // start time
        public gtime_t wtime = new gtime_t(); // write time
        public uint tick; // start tick
        public uint tick_f; // start tick in file
        public uint fpos; // current file position
        public double start; // start offset (s)
        public double speed; // replay speed (time factor)
        public double swapintv; // swap interval (hr) (0: no swap)
#if lock_t_ConditionalDefinition1
	public CRITICAL_SECTION @lock = new CRITICAL_SECTION();
#elif lock_t_ConditionalDefinition2
	public pthread_mutex_t @lock = new pthread_mutex_t();
#else
        public lock_t @lock = new lock_t();
#endif
    }

    public class tcp_t // tcp control type
    {
        public int state; // state (0:close,1:wait,2:connect)
        public string saddr = new string(new char[256]); // address string
        public int port; // port
        public sockaddr_in addr = new sockaddr_in(); // address resolved
#if socket_t_ConditionalDefinition1
	public SOCKET sock = new SOCKET();
#elif socket_t_ConditionalDefinition2
	public int sock;
#else
        public socket_t sock = new socket_t();
#endif
        public int tcon; // reconnect time (ms) (-1:never,0:now)
        public uint tact; // data active tick
        public uint tdis; // disconnect tick
    }

    public class tcpsvr_t // tcp server type
    {
        public tcp_t svr = new tcp_t(); // tcp server control
        public tcp_t[] cli = Arrays.InitializeWithDefaultInstances<tcp_t>(DefineConstants.MAXCLI); // tcp client controls
    }

    public class tcpcli_t // tcp cilent type
    {
        public tcp_t svr = new tcp_t(); // tcp server control
        public int toinact; // inactive timeout (ms) (0:no timeout)
        public int tirecon; // reconnect interval (ms) (0:no reconnect)
    }

    public class ntrip_t // ntrip control type
    {
        public int state; // state (0:close,1:wait,2:connect)
        public int type; // type (0:server,1:client)
        public int nb; // response buffer size
        public string url = new string(new char[256]); // url for proxy
        public string mntpnt = new string(new char[256]); // mountpoint
        public string user = new string(new char[256]); // user
        public string passwd = new string(new char[256]); // password
        public string str = new string(new char[DefineConstants.NTRIP_MAXSTR]); // mountpoint string for server
        public byte[] buff = new byte[DefineConstants.NTRIP_MAXRSP]; // response buffer
        public tcpcli_t tcp; // tcp client
    }

    public class ftp_t // ftp download control type
    {
        public int state; // state (0:close,1:download,2:complete,3:error)
        public int proto; // protocol (0:ftp,1:http)
        public int error; // error code (0:no error,1-10:wget error,
                          /*            11:no temp dir,12:uncompact error) */
        public string addr = new string(new char[1024]); // download address
        public string file = new string(new char[1024]); // download file path
        public string user = new string(new char[256]); // user for ftp
        public string passwd = new string(new char[256]); // password for ftp
        public string local = new string(new char[1024]); // local file path
        public int[] topts = new int[4]; // time options {poff,tint,toff,tretry} (s)
        public gtime_t tnext = new gtime_t(); // next retry time (gpst)
#if thread_t_ConditionalDefinition1
	public System.IntPtr thread;
#elif thread_t_ConditionalDefinition2
	public pthread_t thread = new pthread_t();
#else
        public thread_t thread = new thread_t();
#endif
    }
}
