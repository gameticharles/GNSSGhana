using System;
using System.Collections.Generic;

namespace org.gogpsproject.producer.parser
{

	using CommPortIdentifier = gnu.io.CommPortIdentifier;
	using SerialPort = gnu.io.SerialPort;



	public abstract class AbstractSerialConnection<T> : StreamResource, StreamEventProducer where T : org.gogpsproject.producer.StreamEventProducer
	{
		public abstract void release(bool waitForThread, long timeoutMs);
	  protected internal InputStream inputStream;
	  protected internal OutputStream outputStream;
	  protected internal bool connected = false;

	  protected internal SerialPort serialPort;

	  protected internal string portName;
	  protected internal int speed;

	  protected internal int setMeasurementRate = 1;
	  protected internal bool enableTimetag = true;
	  protected internal bool? enableDebug = true;
	  protected internal string outputDir = "./test";

	  protected internal T prod;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void init() throws Exception
	  public virtual void init()
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

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void release() throws InterruptedException
	  public virtual void release()
	  {
		try
		{
		  inputStream.close();
		}
		catch (IOException e)
		{
		  Console.WriteLine(e.ToString());
		  Console.Write(e.StackTrace);
		}
		try
		{
		  outputStream.close();
		}
		catch (IOException e)
		{
		  Console.WriteLine(e.ToString());
		  Console.Write(e.StackTrace);
		}
		serialPort.close();

		connected = false;
		Console.WriteLine("Connection disconnected");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public static java.util.Vector<String> getPortList(boolean showList)
	  public static List<string> getPortList(bool showList)
	  {
		IEnumerator<CommPortIdentifier> portList;
		List<string> portVect = new List<string>();
		portList = CommPortIdentifier.PortIdentifiers;

		CommPortIdentifier portId;
		while (portList.MoveNext())
		{
		  portId = portList.Current;
		  if (portId.PortType == CommPortIdentifier.PORT_SERIAL)
		  {
			portVect.Add(portId.Name);
		  }
		}
		if (showList)
		{
		  Console.WriteLine("Found the following ports:");
		  for (int i = 0; i < portVect.Count; i++)
		  {
			Console.WriteLine(portVect[i]);
		  }
		}

		return portVect;
	  }

	  public virtual bool Connected
	  {
		  get
		  {
			return connected;
		  }
	  }

	  /* (non-Javadoc)
	   * @see org.gogpsproject.StreamEventProducer#addStreamEventListener(org.gogpsproject.StreamEventListener)
	   */
	  public virtual void addStreamEventListener(StreamEventListener streamEventListener)
	  {
		prod.addStreamEventListener(streamEventListener);
	  }

	  /* (non-Javadoc)
	   * @see org.gogpsproject.StreamEventProducer#getStreamEventListeners()
	   */
	  public virtual List<StreamEventListener> StreamEventListeners
	  {
		  get
		  {
			return prod.StreamEventListeners;
		  }
	  }

	  /* (non-Javadoc)
	   * @see org.gogpsproject.StreamEventProducer#removeStreamEventListener(org.gogpsproject.StreamEventListener)
	   */
	  public virtual void removeStreamEventListener(StreamEventListener streamEventListener)
	  {
		prod.removeStreamEventListener(streamEventListener);
	  }

	}

}