using System;
using System.Collections.Generic;

namespace org.gogpsproject.positioning
{


	using NavigationProducer = org.gogpsproject.producer.NavigationProducer;
	using ObservationSet = org.gogpsproject.producer.ObservationSet;
	using Observations = org.gogpsproject.producer.Observations;
	using IonoGps = org.gogpsproject.producer.parser.IonoGps;

	public class Satellites
	{

	  internal GoGPS goGPS;
	  internal RoverPosition rover;
	  internal MasterPosition master;
	  internal NavigationProducer navigation;

	  /// <summary>
	  /// Absolute position of all visible satellites (ECEF) </summary>
	  internal SatellitePosition[] pos;

	  /// <summary>
	  /// List of satellites available for processing </summary>
	  internal IDictionary<int?, SatellitePosition> avail;

	  /// <summary>
	  /// List of satellites available for processing </summary>
	  internal List<int?> availPhase;

	  /// <summary>
	  /// List of satellite Types available for processing </summary>
	  internal List<char?> typeAvail;

	  /// <summary>
	  /// List of satellite Type available for processing </summary>
	  internal List<char?> typeAvailPhase;

	  /// <summary>
	  /// List of satellite Types & Id available for processing </summary>
	  internal List<string> gnssAvail;

	  /// <summary>
	  /// List of satellite Types & Id available for processing </summary>
	  internal List<string> gnssAvailPhase;

	  /// <summary>
	  /// Index of the satellite with highest elevation in satAvail list </summary>
	  internal int pivot;

	  public Satellites(GoGPS goGPS)
	  {
		this.goGPS = goGPS;
		this.rover = goGPS.getRoverPos();
		this.master = goGPS.getMasterPos();
		this.navigation = goGPS.Navigation;
	  }

	  /// <returns> the number of available satellites </returns>
	  public virtual int AvailNumber
	  {
		  get
		  {
			return avail.Count;
		  }
	  }

	  /// <returns> the number of available satellites (with phase) </returns>
	  public virtual int AvailPhaseNumber
	  {
		  get
		  {
			return availPhase.Count;
		  }
	  }

	  public virtual string AvailGnssSystems
	  {
		  get
		  {
			if (typeAvail.Count == 0)
			{
				return "";
			}
			string GnssSys = "";
			for (int i = 0;i < typeAvail.Count;i++)
			{
			  if (GnssSys.IndexOf((typeAvail[i])) < 0)
			  {
				GnssSys = GnssSys + typeAvail[i];
			  }
			}
			return GnssSys;
		  }
	  }

	  /// <param name="elevation"> </param>
	  /// <param name="height"> </param>
	  /// <returns> troposphere correction value by Saastamoinen model </returns>
	  internal static double computeTroposphereCorrection(double elevation, double height)
	  {

		double tropoCorr = 0;

		if (height > 5000)
		{
		  return tropoCorr;
		}

		elevation = Math.toRadians(Math.Abs(elevation));
		if (elevation == 0)
		{
		  elevation = elevation + 0.01;
		}

		// Numerical constants and tables for Saastamoinen algorithm
		// (troposphere correction)
		const double hr = 50.0;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int[] ha = {0, 500, 1000, 1500, 2000, 2500, 3000, 4000, 5000 };
		int[] ha = new int[] {0, 500, 1000, 1500, 2000, 2500, 3000, 4000, 5000};
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final double[] ba = { 1.156, 1.079, 1.006, 0.938, 0.874, 0.813, 0.757, 0.654, 0.563 };
		double[] ba = new double[] {1.156, 1.079, 1.006, 0.938, 0.874, 0.813, 0.757, 0.654, 0.563};

		// Saastamoinen algorithm
		double P = Constants.STANDARD_PRESSURE * Math.Pow((1 - 0.0000226 * height), 5.225);
		double T = Constants.STANDARD_TEMPERATURE - 0.0065 * height;
		double H = hr * Math.Exp(-0.0006396 * height);

		// If height is below zero, keep the maximum correction value
		double B = ba[0];
		// Otherwise, interpolate the tables
		if (height >= 0)
		{
		  int i = 1;
		  while (height > ha[i])
		  {
			i++;
		  }
		  double m = (ba[i] - ba[i - 1]) / (ha[i] - ha[i - 1]);
		  B = ba[i - 1] + m * (height - ha[i - 1]);
		}

		double e = 0.01 * H * Math.Exp(-37.2465 + 0.213166 * T - 0.000256908 * Math.Pow(T, 2));

		tropoCorr = ((0.002277 / Math.Sin(elevation)) * (P - (B / Math.Pow(Math.Tan(elevation), 2))) + (0.002277 / Math.Sin(elevation)) * (1255 / T + 0.05) * e);

		return tropoCorr;
	  }

	  /// <param name="ionoParams"> </param>
	  /// <param name="coord"> </param>
	  /// <param name="time"> </param>
	  /// <returns> ionosphere correction value by Klobuchar model </returns>
	  internal static double computeIonosphereCorrection(NavigationProducer navigation, Coordinates coord, double azimuth, double elevation, Time time)
	  {

		double ionoCorr = 0;

		IonoGps iono = navigation.getIono(time.Msec);

		if (iono == null)
		{
		  return 0.0;
		}
	//    double a0 = navigation.getIono(time.getMsec(),0);
	//    double a1 = navigation.getIono(time.getMsec(),1);
	//    double a2 = navigation.getIono(time.getMsec(),2);
	//    double a3 = navigation.getIono(time.getMsec(),3);
	//    double b0 = navigation.getIono(time.getMsec(),4);
	//    double b1 = navigation.getIono(time.getMsec(),5);
	//    double b2 = navigation.getIono(time.getMsec(),6);
	//    double b3 = navigation.getIono(time.getMsec(),7);

		elevation = Math.Abs(elevation);

		// Parameter conversion to semicircles
		double lon = coord.GeodeticLongitude / 180; // geod.get(0)
		double lat = coord.GeodeticLatitude / 180; //geod.get(1)
		azimuth = azimuth / 180;
		elevation = elevation / 180;

		// Klobuchar algorithm
		double f = 1 + 16 * Math.Pow((0.53 - elevation), 3);
		double psi = 0.0137 / (elevation + 0.11) - 0.022;
		double phi = lat + psi * Math.Cos(azimuth * Math.PI);

		if (phi > 0.416)
		{
		  phi = 0.416;

		}
		if (phi < -0.416)
		{
		  phi = -0.416;
		}

		double lambda = lon + (psi * Math.Sin(azimuth * Math.PI)) / Math.Cos(phi * Math.PI);

		double ro = phi + 0.064 * Math.Cos((lambda - 1.617) * Math.PI);
		double t = lambda * 43200 + time.GpsTime;

		while (t >= 86400)
		{
		  t = t - 86400;
		}

		while (t < 0)
		{
		  t = t + 86400;
		}

		double p = iono.getBeta(0) + iono.getBeta(1) * ro + iono.getBeta(2) * Math.Pow(ro, 2) + iono.getBeta(3) * Math.Pow(ro, 3);

		if (p < 72000)
		{
		  p = 72000;
		}

		double a = iono.getAlpha(0) + iono.getAlpha(1) * ro + iono.getAlpha(2) * Math.Pow(ro, 2) + iono.getAlpha(3) * Math.Pow(ro, 3);

		if (a < 0)
		{
		  a = 0;
		}

		double x = (2 * Math.PI * (t - 50400)) / p;

		if (Math.Abs(x) < 1.57)
		{
		  ionoCorr = Constants.SPEED_OF_LIGHT * f * (5e-9 + a * (1 - (Math.Pow(x, 2)) / 2 + (Math.Pow(x, 4)) / 24));
		}
		else
		{
		  ionoCorr = Constants.SPEED_OF_LIGHT * f * 5e-9;
		}

		return ionoCorr;
	  }

	  internal virtual void init(Observations roverObs)
	  {

		int nObs = roverObs.NumSat;

		// Allocate an array to store GPS satellite positions
		pos = new SatellitePosition[nObs];

		// Create a list for available satellites
		avail = new LinkedHashMap<>();
		typeAvail = new List<>(0);
		gnssAvail = new List<>(0);

		// Create a list for available satellites with phase
		availPhase = new List<>(0);
		typeAvailPhase = new List<>(0);
		gnssAvailPhase = new List<>(0);

		// Allocate arrays to store receiver-satellite vectors
		rover.diffSat = new SimpleMatrix[nObs];
		master.diffSat = new SimpleMatrix[nObs];

		// Allocate arrays to store receiver-satellite approximate range
		rover.satAppRange = new double[nObs];
		master.satAppRange = new double[nObs];

		// Allocate arrays to store receiver-satellite atmospheric corrections
		rover.satTropoCorr = new double[nObs];
		rover.satIonoCorr = new double[nObs];
		master.satTropoCorr = new double[nObs];
		master.satIonoCorr = new double[nObs];

		// Allocate arrays of topocentric coordinates
		rover.topo = new TopocentricCoordinates[nObs];
		master.topo = new TopocentricCoordinates[nObs];

		rover.satsInUse = 0;
	  }

	  /// <param name="roverObs"> </param>
	  public virtual void selectStandalone(Observations roverObs)
	  {
		selectStandalone(roverObs, goGPS.Cutoff);
	  }

	  /// <param name="roverObs"> </param>
	  /// <param name="cutoff"> </param>
	  public virtual void selectStandalone(Observations roverObs, double cutoff)
	  {

		init(roverObs);

		// Compute topocentric coordinates and
		// select satellites above the cutoff level
		for (int i = 0; i < roverObs.NumSat; i++)
		{

		  int id = roverObs.getSatID(i);
		  char satType = roverObs.getGnssType(i);

		  // Compute GPS satellite positions getGpsByIdx(idx).getSatType()
		  pos[i] = navigation.getGpsSatPosition(roverObs, id, satType, rover.ClockError);

		  if (pos[i] != null)
		  {

				  if (pos[i].Equals(SatellitePosition.UnhealthySat))
				  {
					pos[i] = null;
					continue;
				  }

			// Compute rover-satellite approximate pseudorange
			rover.diffSat[i] = rover.minusXYZ(pos[i]);
			rover.satAppRange[i] = Math.Sqrt(Math.Pow(rover.diffSat[i].get(0), 2) + Math.Pow(rover.diffSat[i].get(1), 2) + Math.Pow(rover.diffSat[i].get(2), 2));

			// Compute azimuth, elevation and distance for each satellite
			rover.topo[i] = new TopocentricCoordinates();
			rover.topo[i].computeTopocentric(rover, pos[i]);

			// Correct approximate pseudorange for troposphere
			rover.satTropoCorr[i] = computeTroposphereCorrection(rover.topo[i].Elevation, rover.GeodeticHeight);

			// Correct approximate pseudorange for ionosphere
			rover.satIonoCorr[i] = computeIonosphereCorrection(navigation, rover, rover.topo[i].Azimuth, rover.topo[i].Elevation, roverObs.RefTime);

	//        System.out.println("getElevation: " + id + "::" + rover.topo[i].getElevation() ); 
			// Check if satellite elevation is higher than cutoff
			if (rover.topo[i].Elevation > cutoff)
			{

			  avail[id] = pos[i];
			  typeAvail.Add(satType);
			  gnssAvail.Add(Convert.ToString(satType) + Convert.ToString(id));

			  // Check if also phase is available
			  if (!double.IsNaN(roverObs.getSatByIDType(id, satType).getPhaseCycles(goGPS.Freq)))
			  {
				availPhase.Add(id);
				typeAvailPhase.Add(satType);
				gnssAvailPhase.Add(Convert.ToString(satType) + Convert.ToString(id));

			  }
			}
			else
			{
			  if (goGPS.Debug)
			  {
				  Console.WriteLine("Not useful sat " + roverObs.getSatID(i) + " for too low elevation " + rover.topo[i].Elevation + " < " + cutoff);
			  }
			}
		  }
		}
	  }

	  /// <param name="roverObs"> </param>
	  /// <param name="masterObs"> </param>
	  /// <param name="masterPos"> </param>
	  public virtual void selectDoubleDiff(Observations roverObs, Observations masterObs, Coordinates masterPos)
	  {

		// Retrieve options from goGPS class
		double cutoff = goGPS.Cutoff;

		init(roverObs);

		// Variables to store highest elevation
		double maxElevCode = 0;
		double maxElevPhase = 0;

		// Variables for code pivot and phase pivot
		int pivotCode = -1;
		int pivotPhase = -1;

		// Satellite ID
		int id = 0;

		// Compute topocentric coordinates and
		// select satellites above the cutoff level
		for (int i = 0; i < roverObs.NumSat; i++)
		{

		  id = roverObs.getSatID(i);
		  char satType = roverObs.getGnssType(i);

		  // Compute GPS satellite positions
		  pos[i] = navigation.getGpsSatPosition(roverObs, id, satType, rover.ClockError);

		  if (pos[i] != null)
		  {

			// Compute rover-satellite approximate pseudorange
			rover.diffSat[i] = rover.minusXYZ(pos[i]);
			rover.satAppRange[i] = Math.Sqrt(Math.Pow(rover.diffSat[i].get(0), 2) + Math.Pow(rover.diffSat[i].get(1), 2) + Math.Pow(rover.diffSat[i].get(2), 2));

			// Compute master-satellite approximate pseudorange
			master.diffSat[i] = masterPos.minusXYZ(pos[i]);
			master.satAppRange[i] = Math.Sqrt(Math.Pow(master.diffSat[i].get(0), 2) + Math.Pow(master.diffSat[i].get(1), 2) + Math.Pow(master.diffSat[i].get(2), 2));

			// Compute azimuth, elevation and distance for each satellite from rover
			rover.topo[i] = new TopocentricCoordinates();
			rover.topo[i].computeTopocentric(rover, pos[i]);

			// Compute azimuth, elevation and distance for each satellite from master
			master.topo[i] = new TopocentricCoordinates();
			master.topo[i].computeTopocentric(masterPos, pos[i]);

			// Computation of rover-satellite troposphere correction
			rover.satTropoCorr[i] = computeTroposphereCorrection(rover.topo[i].Elevation, rover.GeodeticHeight);

			// Computation of master-satellite troposphere correction
			master.satTropoCorr[i] = computeTroposphereCorrection(master.topo[i].Elevation, masterPos.GeodeticHeight);

			// Computation of rover-satellite ionosphere correction
			rover.satIonoCorr[i] = computeIonosphereCorrection(navigation, rover, rover.topo[i].Azimuth, rover.topo[i].Elevation, roverObs.RefTime);

			// Computation of master-satellite ionosphere correction
			master.satIonoCorr[i] = computeIonosphereCorrection(navigation, masterPos, master.topo[i].Azimuth, master.topo[i].Elevation, roverObs.RefTime);

			// Check if satellite is available for double differences, after cutoff
			if (masterObs.containsSatIDType(roverObs.getSatID(i), roverObs.getGnssType(i)) && rover.topo[i].Elevation > cutoff) // gpsSat.get( // masterObs.gpsSat.contains(roverObs.getGpsSatID(i)
			{

			  // Find code pivot satellite (with highest elevation)
			  if (rover.topo[i].Elevation > maxElevCode)
			  {
				pivotCode = i;
				maxElevCode = rover.topo[i].Elevation;
			  }

			  avail[id] = pos[i];
			  typeAvail.Add(satType);
			  gnssAvail.Add(Convert.ToString(satType) + Convert.ToString(id));

			  // Check if also phase is available for both rover and master
			  if (!double.IsNaN(roverObs.getSatByIDType(id, satType).getPhaseCycles(goGPS.Freq)) && !double.IsNaN(masterObs.getSatByIDType(id, satType).getPhaseCycles(goGPS.Freq)))
			  {

				// Find code pivot satellite (with highest elevation)
				if (rover.topo[i].Elevation > maxElevPhase)
				{
				  pivotPhase = i;
				  maxElevPhase = rover.topo[i].Elevation;
				}

				availPhase.Add(id);
				typeAvailPhase.Add(satType);
				gnssAvailPhase.Add(Convert.ToString(satType) + Convert.ToString(id));
			  }
			}
		  }
		}

		// Select best pivot satellite
		if (pivotPhase != -1)
		{
		  pivot = pivotPhase;
		}
		else
		{
		  pivot = pivotCode;
		}
	  }
	}

}