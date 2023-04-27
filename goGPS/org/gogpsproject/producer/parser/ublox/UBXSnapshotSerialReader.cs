using System;
using System.Threading;

namespace org.gogpsproject.producer.parser.ublox
{



	public class UBXSnapshotSerialReader : UBXSerialReader
	{

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
	  private static readonly Logger logger = Logger.getLogger(typeof(UBXSnapshotSerialReader).FullName);

	  public UBXSnapshotSerialReader(InputStream @in, OutputStream @out, string COMPort, string outputDir) : base(@in,@out, COMPort, outputDir)
	  {
	  }

	  public UBXSnapshotSerialReader(InputStream @in, OutputStream @out, string COMPort, string outputDir, StreamEventListener streamEventListener) : base(@in,@out,COMPort,outputDir,streamEventListener)
	  {
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void start() throws java.io.IOException
	  public override void start()
	  {

		base.start();

	  }

	  public override void run()
	  {

		int data = 0;
		long aidEphTS = DateTimeHelperClass.CurrentUnixTimeMillis();
		long aidHuiTS = DateTimeHelperClass.CurrentUnixTimeMillis();
		//long sysOutTS = System.currentTimeMillis();
		UBXMsgConfiguration msgcfg = null;
		FileOutputStream fos_tim = null;
		FileOutputStream fos_nmea = null;
		PrintStream psSystime = null;
		PrintStream psNmea = null;

		DateTime date = DateTime.Now;
		SimpleDateFormat sdf1 = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss.SSS");
		string date1 = sdf1.format(date);
		string COMPortStr = prepareCOMStringForFilename(COMPort);

		if (sysTimeLogEnabled)
		{
		  Console.WriteLine(date1 + " - " + COMPort + " - System time logging enabled");
		  try
		  {
			Console.WriteLine(date1 + " - " + COMPort + " - Logging system time in " + outputDir + "/" + COMPortStr + "_" + dateFile + "_systime.txt");
			fos_tim = new FileOutputStream(outputDir + "/" + COMPortStr + "_" + dateFile + "_systime.txt");
			psSystime = new PrintStream(fos_tim);
			psSystime.println("GPS time                      System time");
		  }
		  catch (FileNotFoundException e)
		  {
			Console.WriteLine(e.ToString());
			Console.Write(e.StackTrace);
		  }
		}
		else
		{
		  Console.WriteLine(date1 + " - " + COMPort + " - System time logging disabled");
		}

		if (requestedNmeaMsgs.Count > 0)
		{
		  try
		  {
			Console.WriteLine(date1 + " - " + COMPort + " - Logging NMEA sentences in " + outputDir + "/" + COMPortStr + "_" + dateFile + "_NMEA.txt");
			fos_nmea = new FileOutputStream(outputDir + "/" + COMPortStr + "_" + dateFile + "_NMEA.txt");
			psNmea = new PrintStream(fos_nmea);
		  }
		  catch (FileNotFoundException e)
		  {
			Console.WriteLine(e.ToString());
			Console.Write(e.StackTrace);
		  }
		}

		try
		{
		  int[] msg = new int[] {};
		  if (msgAidHuiRate > 0)
		  {
			Console.WriteLine(date1 + " - " + COMPort + " - AID-HUI message polling enabled (rate: " + msgAidHuiRate + "s)");
			msgcfg = new UBXMsgConfiguration(UBXMessageType.CLASS_AID, UBXMessageType.AID_HUI, msg);
			@out.write(msgcfg.Byte);
			@out.flush();

		  }
		  if (msgAidEphRate > 0)
		  {
			Console.WriteLine(date1 + " - " + COMPort + " - AID-EPH message polling enabled (rate: " + msgAidEphRate + "s)");
			msgcfg = new UBXMsgConfiguration(UBXMessageType.CLASS_AID, UBXMessageType.AID_EPH, msg);
			@out.write(msgcfg.Byte);
			@out.flush();
		  }

		  @in.start();
		  sdf1 = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss.SSS");
		  string dateSys = null;
		  string dateGps = null;
		  bool rxmRawMsgReceived = false;
		  bool truncatedNmea = false;
		  reader.enableDebugMode(this.debugModeEnabled);
		  while (!stop_Renamed)
		  {
			if (@in.available() > 0)
			{
			  dateSys = sdf1.format(DateTime.Now);
			  if (!truncatedNmea)
			  {
				data = @in.read();
			  }
			  else
			  {
				truncatedNmea = false;
			  }
			  try
			  {
				if (data == 0xB5)
				{
				  object o = reader.readMessage();
					if (o is Observations)
					{
					  if (streamEventListeners != null && o != null)
					  {
						foreach (StreamEventListener sel in streamEventListeners)
						{
						  try
						  {
						  Observations co = sel.CurrentObservations;
						  if (co == null)
						  {
							continue;
						  }

	//                        sel.pointToNextObservations();

							rxmRawMsgReceived = true;

							if (this.sysTimeLogEnabled)
							{
							  dateGps = sdf1.format(new DateTime(co.RefTime.Msec));
							  psSystime.println(dateGps + "       " + dateSys);
							}
						  }
						  catch (Exception e)
						  {
							logger.log(Level.SEVERE, e.Message, e);
						  }
						}
					  }
					}
				}
				else if (data == 0x24)
				{
				  if (requestedNmeaMsgs.Count > 0)
				  {
					string sentence = "" + (char) data;
					data = @in.read();
					if (data == 0x47)
					{
					  sentence = sentence + (char) data;
					  data = @in.read();
					  if (data == 0x50)
					  {
						sentence = sentence + (char) data;
						data = @in.read();
						while (data != 0x0A && data != 0xB5)
						{
						  sentence = sentence + (char) data;
						  data = @in.read();
						}
						sentence = sentence + (char) data;
						psNmea.print(sentence);
	//                    if (this.debugModeEnabled) {
	//                      System.out.print(sentence);
	//                    }
						if (data == 0xB5)
						{
						  truncatedNmea = true;
						  if (this.debugModeEnabled)
						  {
							Console.WriteLine("Warning: truncated NMEA message");
						  }
						}
					  }
					}
				  }
				}
				else
				{
				  if (this.debugModeEnabled)
				  {
	//                System.out.println("Warning: wrong sync char 1 "+data+" "+Integer.toHexString(data)+" ["+((char)data)+"]");
				  }
				}
			  }
			  catch (UBXException ubxe)
			  {
				Console.WriteLine(ubxe.ToString());
				Console.Write(ubxe.StackTrace);
			  }
			}
			else
			{
			  // no bytes to read, wait 1 msec
			  try
			  {
				Thread.Sleep(1);
			  }
			  catch (InterruptedException)
			  {
			  }
			}
			long curTS = DateTimeHelperClass.CurrentUnixTimeMillis();

			if (msgAidEphRate > 0 && curTS - aidEphTS >= msgAidEphRate * 1000)
			{
			  Console.WriteLine(dateSys + " - " + COMPort + " - Sending AID-INI message");
			  msgcfg = new UBX_INI_MsgConfiguration();
			  @out.write(msgcfg.Byte);
			  @out.flush();
			  aidEphTS = curTS;
			}

	//        if(msgAidEphRate > 0 && curTS-aidEphTS >= msgAidEphRate*1000){
	//          System.out.println(dateSys+" - "+COMPort+" - Polling AID-EPH message");
	//          msgcfg = new UBXMsgConfiguration(UBXMessageType.CLASS_AID, UBXMessageType.AID_EPH, msg);
	//          out.write(msgcfg.getByte());
	//          out.flush();
	//          aidEphTS = curTS;
	//        }
	//        
	//        if(msgAidHuiRate > 0 && curTS-aidHuiTS >= msgAidHuiRate*1000){
	//          System.out.println(dateSys+" - "+COMPort+" - Polling AID-HUI message");
	//          msgcfg = new UBXMsgConfiguration(UBXMessageType.CLASS_AID, UBXMessageType.AID_HUI, msg);
	//          out.write(msgcfg.getByte());
	//          out.flush();
	//          aidHuiTS = curTS;
	//          
	//        }
			if (rxmRawMsgReceived) //curTS-sysOutTS >= 1*1000
			{
			  int bps = @in.CurrentBps;
			  if (bps != 0)
			  {
				Console.WriteLine(dateSys + " - " + COMPort + " - logging at " + string.Format("{0,4:D}", bps) + " Bps -- total: " + @in.Counter + " bytes");
			  }
			  else
			  {
				Console.WriteLine(dateSys + " - " + COMPort + " - log starting...     -- total: " + @in.Counter + " bytes");
			  }
			  //sysOutTS = curTS;
			  rxmRawMsgReceived = false;
			}
		  }
		}
		catch (IOException e)
		{
		  Console.WriteLine(e.ToString());
		  Console.Write(e.StackTrace);
		}
		foreach (StreamEventListener sel in streamEventListeners)
		{
		  sel.streamClosed();
		}
		//if(streamEventListener!=null) streamEventListener.streamClosed();
	  }

	}

}