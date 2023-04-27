namespace org.gogpsproject.producer.parser.rinex
{


	using EphGps = org.gogpsproject.ephemeris.EphGps;
	using SatellitePosition = org.gogpsproject.positioning.SatellitePosition;

	public class RinexNavigationSpeedParser : RinexNavigationParser
	{

	  public RinexNavigationSpeedParser() : base(null)
	  {
	  }

	  public RinexNavigationSpeedParser(File fileNav) : base(fileNav)
	  {
	  }

	  public RinexNavigationSpeedParser(InputStream @is, File cache) : base(@is, cache)
	  {
	  }

	  public override SatellitePosition computePositionGps(Observations obs, int satID, char satType, EphGps eph, double receiverClockError)
	  {
		return computePositionSpeedGps(obs, satID, satType, eph, receiverClockError);
	  }

	}

}