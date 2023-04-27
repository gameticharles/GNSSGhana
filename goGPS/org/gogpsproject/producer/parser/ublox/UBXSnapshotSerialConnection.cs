using System;

namespace org.gogpsproject.producer.parser.ublox
{

	using CommPortIdentifier = gnu.io.CommPortIdentifier;
	using SerialPort = gnu.io.SerialPort;

	public class UBXSnapshotSerialConnection : UBXSerialConnection
	{

	  public UBXSnapshotSerialConnection(string portName, int speed) : base(portName, speed)
	  {
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void serialinit() throws Exception
	  public virtual void serialinit()
	  {
		CommPortIdentifier portIdentifier;

		portIdentifier = CommPortIdentifier.getPortIdentifier(portName);
		if (portIdentifier.CurrentlyOwned)
		{
		  Console.WriteLine("Error: Port is currently in use");
		}
		else
		{
		  serialPort = (SerialPort) portIdentifier.open("Serial", 2000);
		  serialPort.setSerialPortParams(speed, SerialPort.DATABITS_8, SerialPort.STOPBITS_1, SerialPort.PARITY_NONE);

		  inputStream = serialPort.InputStream;
		  outputStream = serialPort.OutputStream;
		}
	  }

	  /* (non-Javadoc)
	   * @see org.gogpsproject.StreamResource#init()
	   */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void init() throws Exception
	  public override void init()
	  {
			serialinit();

			prod = new UBXSnapshotSerialReader(inputStream,outputStream,portName,outputDir);
			prod.Rate = base.setMeasurementRate;
			prod.enableAidEphMsg(base.setEphemerisRate);
			prod.enableAidHuiMsg(base.setIonosphereRate);
			prod.enableSysTimeLog(base.enableTimetag_Renamed);
			prod.enableDebugMode(base.enableDebug_Renamed);
			prod.enableNmeaMsg(base.enableNmeaList);
			prod.start();

			connected = true;
			Console.WriteLine("Connection on " + portName + " established");
			//conn = true;
	  }

	}


}