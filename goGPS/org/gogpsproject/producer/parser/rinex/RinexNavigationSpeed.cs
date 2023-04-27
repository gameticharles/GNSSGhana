namespace org.gogpsproject.producer.parser.rinex
{

	using EphGps = org.gogpsproject.ephemeris.EphGps;
	using SatellitePosition = org.gogpsproject.positioning.SatellitePosition;

	public class RinexNavigationSpeed : RinexNavigation
	{

	  public RinexNavigationSpeed() : base(null)
	  {
	  }

	  public RinexNavigationSpeed(string garnerNavigationAuto) : base(garnerNavigationAuto)
	  {
	  }

	  public override SatellitePosition getGpsSatPosition(Observations obs, int satID, char satType, double receiverClockError)
	  {
		long unixTime = obs.RefTime.Msec;
		long requestedTime = unixTime + 2 * 3600L * 1000L;
		EphGps eph = null;
		int maxBack = 3;
		while (eph == null && (maxBack--) > 0)
		{

		  RinexNavigationParser rnp = getRNPByTimestamp(requestedTime);
		  if (rnp != null)
		  {
			obs.rinexFileName = rnp.FileName;

			  if (rnp.isTimestampInEpocsRange(unixTime))
			  {
	  //        return rnp.getGpsSatPosition(obs, satID, satType, receiverClockError);
			  eph = rnp.findEph(unixTime, satID, satType);
			  if (eph == EphGps.UnhealthyEph)
			  {
				return SatellitePosition.UnhealthySat;
			  }

			  if (eph != null)
			  {
				 // cache this rnp in case we've backed off the time
				 put(obs.RefTime.Msec, rnp);

		//        char satType = eph.getSatType();
				SatellitePosition sp = rnp.computePositionSpeedGps(obs, satID, satType, eph, receiverClockError);
		//        SatellitePosition sp = computePositionGps(unixTime, satType, satID, eph, range, receiverClockError);
				//if(receiverPosition!=null) earthRotationCorrection(receiverPosition, sp);
				return sp; // new SatellitePosition(eph, unixTime, satID, range);
			  }
			  }
		  }
		  requestedTime -= (2L * 3600L * 1000L);
		}
		return null;
	  }

	}

}