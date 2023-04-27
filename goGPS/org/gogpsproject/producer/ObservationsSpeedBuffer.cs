namespace org.gogpsproject.producer
{

	using EphGps = org.gogpsproject.ephemeris.EphGps;
	using SatellitePosition = org.gogpsproject.positioning.SatellitePosition;

	public class ObservationsSpeedBuffer : ObservationsBuffer
	{

	  public ObservationsSpeedBuffer() : base()
	  {
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public ObservationsSpeedBuffer(StreamResource streamResource, String fileNameOutLog) throws java.io.FileNotFoundException
	  public ObservationsSpeedBuffer(StreamResource streamResource, string fileNameOutLog) : base(streamResource, fileNameOutLog)
	  {
	  }

	  public override SatellitePosition computePositionGps(Observations obs, int satID, char satType, EphGps eph, double receiverClockError)
	  {
		return computePositionSpeedGps(obs, satID, satType, eph, receiverClockError);
	  }
	}

}