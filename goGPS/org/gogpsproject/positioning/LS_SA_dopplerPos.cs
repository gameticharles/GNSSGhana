using System;
using System.Collections.Generic;

namespace org.gogpsproject.positioning
{


	using SimpleMatrix = org.ejml.simple.SimpleMatrix;
	using PositionConsumer = org.gogpsproject.consumer.PositionConsumer;
	using DopType = org.gogpsproject.positioning.RoverPosition.DopType;
	using NavigationProducer = org.gogpsproject.producer.NavigationProducer;
	using ObservationSet = org.gogpsproject.producer.ObservationSet;
	using Observations = org.gogpsproject.producer.Observations;

	public class LS_SA_dopplerPos : LS_SA_code
	{

	  public LS_SA_dopplerPos(GoGPS goGPS) : base(goGPS)
	  {
	  }

	  /// <summary>
	  /// Estimate full pseudorange and satellite position from a priori rover position and fractional pseudoranges </summary>
	  /// <param name="roverObs"> </param>
	  /// <param name="cutoff"> </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void selectSatellites(org.gogpsproject.producer.Observations roverObs, double cutoff, final double MODULO)
	  public virtual void selectSatellites(Observations roverObs, double cutoff, double MODULO)
	  {

		NavigationProducer navigation = goGPS.Navigation;

		// Number of GPS observations
		int nObs = roverObs.NumSat;

		// Allocate an array to store GPS satellite positions
		sats.pos = new SatellitePosition[nObs];

		// Allocate an array to store receiver-satellite vectors
		rover.diffSat = new SimpleMatrix[nObs];

		// Allocate an array to store receiver-satellite approximate range
		rover.satAppRange = new double[nObs];

		// Allocate arrays to store receiver-satellite atmospheric corrections
		rover.satTropoCorr = new double[nObs];
		rover.satIonoCorr = new double[nObs];

		// Create a list for available satellites after cutoff
		sats.avail = new LinkedHashMap<>();
		sats.typeAvail = new List<>(0);
		sats.gnssAvail = new List<>(0);

		// Create a list for available satellites with phase
		sats.availPhase = new List<>(0);
		sats.typeAvailPhase = new List<>(0);
		sats.gnssAvailPhase = new List<>(0);

		// Allocate array of topocentric coordinates
		rover.topo = new TopocentricCoordinates[nObs];

		rover.satsInUse = 0;
		// Satellite ID
		int id = 0;

		// Compute topocentric coordinates and
		// select satellites above the cutoff level
		for (int i = 0; i < nObs; i++)
		{
		  id = roverObs.getSatID(i);
		  ObservationSet os = roverObs.getSatByID(id);
		  char satType = roverObs.getGnssType(i);

		  sats.pos[i] = goGPS.Navigation.getGpsSatPosition(roverObs, id, 'G', rover.ClockError);

		  if (sats.pos[i] == SatellitePosition.UnhealthySat)
		  {
			sats.pos[i] = null;
			continue;
		  }

		  if (sats.pos[i] == null || double.IsNaN(sats.pos[i].X))
		  {
	//        if(goGPS.isDebug()) System.out.println("Not useful sat "+roverObs.getSatID(i));
			if (i == 0 || sats.avail.Count > 0)
			{
			  continue;
			}
			else
			{
			  rover.status = Status.EphNotFound;
			  return;
			}
		  }

		  // remember cph and restore it later
		  double code = os.getCodeC(0);

		  // Compute rover-satellite approximate pseudorange
		  rover.diffSat[i] = rover.minusXYZ(sats.pos[i]); // negative, for LOS vectors

		  rover.satAppRange[i] = Math.Sqrt(Math.Pow(rover.diffSat[i].get(0), 2) + Math.Pow(rover.diffSat[i].get(1), 2) + Math.Pow(rover.diffSat[i].get(2), 2));

		  // recompute satpos now with estimatedPR
		  if (double.IsNaN(rover.satAppRange[i]))
		  {
			if (goGPS.Debug)
			{
				Console.WriteLine("Error NaN");
			}
		  }
		  else
		  {
			os.setCodeC(0, rover.satAppRange[i]);

			// Compute GPS satellite positions getGpsByIdx(idx).getSatType()
			sats.pos[i] = goGPS.Navigation.getGpsSatPosition(roverObs, id, 'G', rover.ClockError);

			// restore code observation
			os.setCodeC(0, code);

			if (sats.pos[i] == null)
			{
	  //        if(debug) System.out.println("Not useful sat "+roverObs.getSatID(i));
			  if ((i == 0) || sats.avail.Count > 0)
			  {
				continue;
			  }
			  else
			  {
				rover.status = Status.EphNotFound;
				continue;
			  }
			}
		  }
		  // Compute rover-satellite approximate pseudorange
		  rover.diffSat[i] = rover.minusXYZ(sats.pos[i]);
		  rover.satAppRange[i] = Math.Sqrt(Math.Pow(rover.diffSat[i].get(0), 2) + Math.Pow(rover.diffSat[i].get(1), 2) + Math.Pow(rover.diffSat[i].get(2), 2));

		  double R = rover.satAppRange[i] / Constants.SPEED_OF_LIGHT * 1000;
		  double C = roverObs.getSatByID(id).getCodeC(0) / Constants.SPEED_OF_LIGHT * 1000;
		  if (goGPS.Debug)
		  {
			  Console.Write(string.Format("{0,2:D}) SR:{1,8:F5} C:{2,8:F5} D:{3,9:F5} ", id, R % (MODULO * 1000 / Constants.SPEED_OF_LIGHT), C, C - (MODULO * 1000 / Constants.SPEED_OF_LIGHT)));
		  }

		  // Compute azimuth, elevation and distance for each satellite
		  rover.topo[i] = new TopocentricCoordinates();
		  rover.topo[i].computeTopocentric(rover, sats.pos[i]);

		  // Correct approximate pseudorange for troposphere
		  rover.satTropoCorr[i] = sats.computeTroposphereCorrection(rover.topo[i].Elevation, rover.GeodeticHeight);

		  // Correct approximate pseudorange for ionosphere
		  rover.satIonoCorr[i] = sats.computeIonosphereCorrection(navigation, rover, rover.topo[i].Azimuth, rover.topo[i].Elevation, roverObs.RefTime);

		  if (goGPS.Debug)
		  {
			  Console.Write(string.Format(" El:{0,4:F1} ", rover.topo[i].Elevation));
		  }

	//        System.out.println("getElevation: " + id + "::" + rover.topo[i].getElevation() ); 
		  // Check if satellite elevation is higher than cutoff
		  if (rover.topo[i].Elevation >= cutoff)
		  {

			sats.avail[id] = sats.pos[i];
			sats.typeAvail.Add(satType);
			sats.gnssAvail.Add(Convert.ToString(satType) + Convert.ToString(id));

			// Check if also phase is available
			if (!double.IsNaN(roverObs.getSatByIDType(id, 'G').getPhaseCycles(goGPS.Freq)))
			{
			  sats.availPhase.Add(id);
			  sats.typeAvailPhase.Add('G');
			  sats.gnssAvailPhase.Add(Convert.ToString('G') + Convert.ToString(id));
			}
		  }
		  else
		  {
			os.el = rover.topo[i].Elevation;
			if (goGPS.Debug)
			{
				Console.Write(string.Format(" Not useful sat {0,2:D}  for too low elevation {1,3:F1} < {2,3:F1}", roverObs.getSatID(i), rover.topo[i].Elevation, cutoff));
			}
		  }
		  if (goGPS.Debug)
		  {
			  Console.WriteLine();
		  }
		}
	  }

	  /// <summary>
	  /// A port of FastGPS's doppler positioning algorithm 
	  /// See http://fastgps.sourceforge.net/
	  /// spectrum.library.concordia.ca/973909/1/Othieno_MASc_S2012.pdf </summary>
	  /// <param name="obs"> </param>
	  [Obsolete]
	  public virtual void dopplerPosHill(Observations obs)
	  {
		int MINSV = 5;

		// Number of unknown parameters
		int nUnknowns = 4;
		const double DOPP_POS_TOL = 1.0;

		double max_iterations = 20;

		for (int itr = 0; itr < max_iterations; itr++)
		{

		  selectSatellites(obs, -20, GoGPS.MODULO1MS);
	//      sats.selectStandalone(obs, -100);

		  // Number of available satellites (i.e. observations)
		  int nObsAvail = sats.avail.Count;
		  if (nObsAvail < MINSV)
		  {
			if (goGPS.Debug)
			{
				Console.WriteLine("dopplerPos, not enough satellites for " + obs.RefTime);
			}
			if (rover.status == Status.None)
			{
			  rover.status = Status.NotEnoughSats;
			}
			rover.setXYZ(0, 0, 0);
			return;
		  }

	//      nObsAvail++; // add DTM / height soft constraint

		  /// <summary>
		  /// range rate </summary>
		  double[] rodot = new double[nObsAvail];

		  /// <summary>
		  /// Least squares design matrix </summary>
		  SimpleMatrix A = new SimpleMatrix(nObsAvail, nUnknowns);

		  // Set up the least squares matrices
		  SimpleMatrix b = new SimpleMatrix(nObsAvail, 1);

		  double pivotSNR = 0;
		  double pivot = 0;
		  for (int i = 0, k = 0; i < obs.NumSat; i++)
		  {
			int satId = obs.getSatID(i);

			if (sats.pos[i] == null || !sats.avail.Keys.contains(satId)) //|| recpos.ecef==null || sats.pos[i].ecef==null ){
			{
			  continue;
			}

			ObservationSet os = obs.getSatByID(satId);

			// scalar product of speed vector X unit vector
			float doppler = os.getDoppler(ObservationSet.L1);

			// Line Of Sight vector units (ECEF)
			SimpleMatrix e = new SimpleMatrix(1,3);
			e.set(0,0, rover.diffSat[i].get(0) / rover.satAppRange[i]);
			e.set(0,1, rover.diffSat[i].get(1) / rover.satAppRange[i]);
			e.set(0,2, rover.diffSat[i].get(2) / rover.satAppRange[i]);
			double rodotSatSpeed = -e.mult(sats.pos[i].Speed).get(0);
			double dopplerSatSpeed = -rodotSatSpeed * Constants.FL1 / Constants.SPEED_OF_LIGHT;

			if (float.IsNaN(doppler))
			{
			  rodot[k] = rodotSatSpeed;
			}
			else
			{
			  rodot[k] = doppler * Constants.SPEED_OF_LIGHT / Constants.FL1;

			  os.getDoppler(ObservationSet.L1);
			  Console.WriteLine(string.Format("{0,2:D}) snr:{1,2:F0} doppler:{2,6:F0}; satSpeed:{3,6:F0}; D:{4,6:F0}", satId, os.getSignalStrength(ObservationSet.L1), doppler, dopplerSatSpeed, doppler - dopplerSatSpeed));
			}

			// build A matrix
			A.set(k, 0, sats.pos[i].Speed.get(0)); // X
			A.set(k, 1, sats.pos[i].Speed.get(1)); // Y
			A.set(k, 2, sats.pos[i].Speed.get(2)); // Z

			double satpos_norm = Math.Sqrt(Math.Pow(sats.pos[i].X, 2) + Math.Pow(sats.pos[i].Y, 2) + Math.Pow(sats.pos[i].Z, 2));
			A.set(k, 3, satpos_norm);

			SimpleMatrix tempv = rover.minusXYZ(sats.pos[i]);

			/// <summary>
			/// range </summary>
			double ro = Math.Sqrt(Math.Pow(tempv.get(0), 2) + Math.Pow(tempv.get(1), 2) + Math.Pow(tempv.get(2), 2));

			SimpleMatrix satposxyz = new SimpleMatrix(1,3);
			satposxyz.set(0, 0, sats.pos[i].X);
			satposxyz.set(0, 1, sats.pos[i].Y);
			satposxyz.set(0, 2, sats.pos[i].Z);

			SimpleMatrix satvelxyz = new SimpleMatrix(1,3);
			satvelxyz.set(0, 0, sats.pos[i].Speed.get(0));
			satvelxyz.set(0, 1, sats.pos[i].Speed.get(1));
			satvelxyz.set(0, 2, sats.pos[i].Speed.get(2));

			/// <summary>
			/// satpos times satspeed </summary>
			double posvel = satposxyz.mult(satvelxyz.transpose()).get(0,0);

			// B[j] = posvel + rodot[j]*ro + ClockErrorRate*(satpos_norm-ro);
			double bval = posvel + rodot[k] * ro + rover.ClockErrorRate * (satpos_norm - ro);
			b.set(k, 0, bval);

			double snr = os.getSignalStrength(ObservationSet.L1);
			if (snr > pivotSNR)
			{
			  pivotSNR = snr;
			  pivot = Math.Abs(bval);
			}
			k++;
		  }

		 SimpleMatrix x = A.transpose().mult(A).invert().mult(A.transpose()).mult(b);

		 Console.WriteLine(string.Format("Update {0:D}: x: {1,3:F3}, y: {2,3:F3}, z: {3,3:F3}, br: {4,3:F3}", itr, x.get(0), x.get(1), x.get(2), x.get(3)));

		 double correction_mag = Math.Sqrt(Math.Pow(x.get(0) - rover.X, 2) + Math.Pow(x.get(1) - rover.Y, 2) + Math.Pow(x.get(2) - rover.Z, 2));

		 // expected
		 Console.WriteLine(string.Format("pos diff mag {0:F} (m)", correction_mag));

		 // Update receiver clock error rate
		 rover.clockErrorRate = x.get(3);

		 // Update Rx position estimate
		 rover.setXYZ(x.get(0), x.get(1), x.get(2));

		 rover.computeGeodetic();

		 // clamp it to the ground, not very elegant
	//     if( rover.getGeodeticHeight()<30 || rover.getGeodeticHeight() > 100 ){
	//       rover.setGeod( rover.getGeodeticLatitude(), rover.getGeodeticLongitude(), 30 );
	//       rover.computeECEF();
	//     }

		 Console.WriteLine("recpos (" + itr + ")");
		 Console.WriteLine(string.Format("{0,10:F6},{1,10:F6},{2,10:F6}", rover.GeodeticLatitude, rover.GeodeticLongitude, rover.GeodeticHeight));
		 Console.WriteLine();

		 // if correction is small enough, we're done, exit loop
		 if (correction_mag < DOPP_POS_TOL)
		 {
		   break;
		 }
		}
		Console.WriteLine(rover);
	  }

	  /// <summary>
	  /// A simpler version, based on Van Diggelen (8.3.2.1)
	  /// This system returns a position update so it can be plugged in the standard design matrix </summary>
	  /// <param name="obs"> </param>
	  public virtual void dopplerPos(Observations obs)
	  {
		int nUnknowns = 4;
		const double DOPP_POS_TOL = 1.0;

		double max_iterations = 20;

		/// <summary>
		/// Least squares design matrix </summary>
		SimpleMatrix A = null;

		for (int itr = 0; itr < max_iterations; itr++)
		{

		  selectSatellites(obs, -100, GoGPS.MODULO1MS);
	//      sats.selectStandalone(obs, -100);

		  // Number of available satellites (i.e. observations)
		  int nObsAvail = sats.avail.Count + 1; // add DTM / height soft constraint

		  if (nObsAvail < nUnknowns)
		  {
	//        if( goGPS.isDebug() ) 
			Console.WriteLine("dopplerPos, not enough satellites for " + obs.RefTime);
			if (rover.status == Status.None)
			{
			  rover.status = Status.NotEnoughSats;
			}
			rover.setXYZ(0, 0, 0);
			return;
		  }

		  A = new SimpleMatrix(nObsAvail, nUnknowns);

		  // Set up the least squares matrices
		  SimpleMatrix b = new SimpleMatrix(nObsAvail, 1);

		  for (int i = 0, k = 0; i < obs.NumSat; i++)
		  {
			int satId = obs.getSatID(i);

			if (sats.pos[i] == null || !sats.avail.Keys.contains(satId)) //|| recpos.ecef==null || sats.pos[i].ecef==null ){
			{
			  continue;
			}

			ObservationSet os = obs.getSatByID(satId);

			A.set(k, 0, sats.pos[i].Speed.get(0) / rover.satAppRange[i]); // VX
			A.set(k, 1, sats.pos[i].Speed.get(1) / rover.satAppRange[i]); // VY
			A.set(k, 2, sats.pos[i].Speed.get(2) / rover.satAppRange[i]); // VZ
			A.set(k, 3, 1); // clock error rate

			// Line Of Sight vector units (ECEF)
			SimpleMatrix e = new SimpleMatrix(1,3);
			e.set(0,0, rover.diffSat[i].get(0) / rover.satAppRange[i]);
			e.set(0,1, rover.diffSat[i].get(1) / rover.satAppRange[i]);
			e.set(0,2, rover.diffSat[i].get(2) / rover.satAppRange[i]);

			/// <summary>
			/// computed satspeed: scalar product of speed vector X LOS unit vector </summary>
			double rhodotSatSpeed = e.mult(sats.pos[i].Speed).get(0);

			double rhodot = 0;
			if (!double.IsNaN(os.getDoppler(ObservationSet.L1)))
			{
			  float doppler = os.getDoppler(ObservationSet.L1);

			  /// <summary>
			  /// observed range rate </summary>
			  rhodot = doppler * Constants.SPEED_OF_LIGHT / Constants.FL1;
			}

			/// <summary>
			/// residuals </summary>
			b.set(k, 0, rhodot - rhodotSatSpeed - rover.ClockErrorRate);

			k++;
		  }

		 // Add height soft constraint
		 double hR_app = rover.GeodeticHeight;
		 double h_dtm = hR_app > 0? hR_app : 30; // initialize to something above sea level
		 if (h_dtm > 2000)
		 {
		  h_dtm = 2000;
		 }

		 double lam = Math.toRadians(rover.GeodeticLongitude);
		 double phi = Math.toRadians(rover.GeodeticLatitude);

		 double cosLam = Math.Cos(lam);
		 double cosPhi = Math.Cos(phi);
		 double sinLam = Math.Sin(lam);
		 double sinPhi = Math.Sin(phi);

		 int k = nObsAvail - 1;
		 A.set(k, 0, cosPhi * cosLam);
		 A.set(k, 1, cosPhi * sinLam);
		 A.set(k, 2, sinPhi);
		 A.set(k, 3, 0);
		 double y0_dtm = h_dtm - hR_app;
		 b.set(k, 0, y0_dtm);

		 SimpleMatrix x = A.transpose().mult(A).invert().mult(A.transpose()).mult(b);

		 Console.WriteLine(string.Format("Update {0:D}: x: {1,3:F3}, y: {2,3:F3}, z: {3,3:F3}, cr: {4,3:F3}", itr, x.get(0), x.get(1), x.get(2), x.get(3)));

		 double correction_mag = Math.Sqrt(Math.Pow(x.get(0), 2) + Math.Pow(x.get(1), 2) + Math.Pow(x.get(2), 2));

		 // expected
		 Console.WriteLine(string.Format("pos diff mag {0:F} (m)", correction_mag));

		 // Update Rx position estimate
		 rover.PlusXYZ = x.extractMatrix(0, 3, 0, 1);
		 rover.computeGeodetic();

		 // Update receiver clock error rate
		 rover.clockErrorRate += x.get(3);

		 Console.WriteLine("recpos (" + itr + ")");
		 Console.WriteLine(string.Format("{0,10:F6},{1,10:F6},{2,10:F6} cr:{3,10:F6}", rover.GeodeticLatitude, rover.GeodeticLongitude, rover.GeodeticHeight, rover.clockErrorRate));
		 Console.WriteLine();

		 // if correction is small enough, we're done, exit loop
		 if (correction_mag < DOPP_POS_TOL)
		 {
		   break;
		 }
		}

		updateDops(A);

		Console.WriteLine(rover);
	  }

	  public static void run(GoGPS goGPS)
	  {

		RoverPosition rover = goGPS.getRoverPos();
		MasterPosition master = goGPS.getMasterPos();
		Satellites sats = goGPS.Sats;

		long index = 0;
		Observations obsR = null;

		LS_SA_dopplerPos sa = new LS_SA_dopplerPos(goGPS);
		Time refTime;
		try
		{
		  obsR = goGPS.RoverIn.CurrentObservations;

		  goGPS.notifyPositionConsumerEvent(org.gogpsproject.consumer.PositionConsumer_Fields.EVENT_START_OF_TRACK);
		  while (obsR != null) // buffStreamObs.ready()
		  {

			refTime = obsR.RefTime;

			// for test
			rover.setXYZ(0, 0, 0);

	//        runElevationMethod(obsR);

			sa.dopplerPos(obsR);

			// If an approximate position was computed
			if (goGPS.Debug)
			{
				Console.WriteLine("Valid position? " + rover.ValidXYZ);
			}

			RoverPosition coord2 = null;

			if (!rover.ValidXYZ)
			{
	//              coord2 = new ReceiverPosition( Coordinates.globalXYZInstance(0, 0, 0), ReceiverPosition.DOP_TYPE_NONE,0.0,0.0,0.0 );
	//              coord2.status = false;
	//              coord2.satsInView = obsR.getNumSat();
	//              coord2.satsInUse = 0;
			  obsR = goGPS.RoverIn.NextObservations;
			  continue;
			}
			  else
			  {
				if (goGPS.Debug)
				{
					Console.WriteLine("Valid position? " + rover.ValidXYZ + " x:" + rover.X + " y:" + rover.Y + " z:" + rover.Z);
				}
				if (goGPS.Debug)
				{
					Console.WriteLine(" lat:" + rover.GeodeticLatitude + " lon:" + rover.GeodeticLongitude);
				}

				  coord2 = new RoverPosition(rover, DopType.KALMAN, rover.getpDop(), rover.gethDop(), rover.getvDop());
	//                coord2.status = true;
	//                coord2.satsInView = obsR.getNumSat();
	//                coord2.satsInUse = ((SnapshotReceiverPosition)roverPos).satsInUse;

				  // set other things
				  // "Index,Status, Date, UTC,Latitude [DD], Longitude [DD], 
				  // HDOP,SVs in Use, SVs in View, SNR Avg [dB], 
				  // Residual Error, Clock Error, Clock Error Total,\r\n" );

				  if (goGPS.Debug)
				  {
					  Console.WriteLine("-------------------- " + rover.getpDop());
				  }
	//                if(stopAtDopThreshold>0.0 && roverPos.getpDop()<stopAtDopThreshold){
	//                  return coord;
	//                }
			  }
			  if (goGPS.PositionConsumers.Count > 0)
			  {
				coord2.RefTime = new Time(obsR.RefTime.Msec);
				goGPS.notifyPositionConsumerAddCoordinate(coord2);
			  }
	//        }catch(Exception e){
	//          System.out.println("Could not complete due to "+e);
	//          e.printStackTrace();
	//        }
			obsR = goGPS.RoverIn.NextObservations;
		  }
		}
		catch (Exception e)
		{
		  Console.WriteLine(e.ToString());
		  Console.Write(e.StackTrace);
		}
		finally
		{
		  goGPS.notifyPositionConsumerEvent(org.gogpsproject.consumer.PositionConsumer_Fields.EVENT_END_OF_TRACK);
		}
	  }

	}


}