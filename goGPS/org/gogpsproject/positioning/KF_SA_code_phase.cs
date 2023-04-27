using ReinGametiMatrixLib.Matricies;
using System;
using System.Collections.Generic;

namespace org.gogpsproject.positioning
{

	using SimpleMatrix = Matrix;
	using PositionConsumer = org.gogpsproject.consumer.PositionConsumer;
	using DopType = org.gogpsproject.positioning.RoverPosition.DopType;
	using Observations = org.gogpsproject.producer.Observations;
	using ObservationsProducer = org.gogpsproject.producer.ObservationsProducer;

	public class KF_SA_code_phase : KalmanFilter
	{

	  public KF_SA_code_phase(GoGPS goGPS) : base(goGPS)
	  {
	  }

	  internal override void setup(Observations roverObs, Observations masterObs, Coordinates masterPos) // Definition of matrices
	  {

		// Number of GPS observations
		int nObs = roverObs.NumSat;

		// Number of available satellites (i.e. observations)
		int nObsAvail = sats.avail.Count;

		// Matrix containing parameters obtained from the linearization of the observation equations
		SimpleMatrix A = new SimpleMatrix(nObsAvail, 3);

		// Counter for available satellites
		int k = 0;

		// Counter for satellites with phase available
		int p = 0;

		for (int i = 0; i < nObs; i++)
		{

		  int id = roverObs.getSatID(i);
		  char satType = roverObs.getGnssType(i);
		  string checkAvailGnss = Convert.ToString(satType) + Convert.ToString(id);

		  if (sats.pos[i] != null && sats.gnssAvail.Contains(checkAvailGnss))
		  {

			// Compute parameters obtained from linearization of observation equations
			double alphaX = rover.diffSat[i].get(0) / rover.satAppRange[i];
			double alphaY = rover.diffSat[i].get(1) / rover.satAppRange[i];
			double alphaZ = rover.diffSat[i].get(2) / rover.satAppRange[i];

			// Fill in the A matrix
			A.set(k, 0, alphaX); // X
			A.set(k, 1, alphaY); // Y
			A.set(k, 2, alphaZ); // Z

			// Observed code
			double obsRangeCode = roverObs.getSatByIDType(id, satType).getPseudorange(goGPS.Freq);

			// Observed phase
			double obsRangePhase = roverObs.getSatByIDType(id, satType).getPhaserange(goGPS.Freq);

			// Compute approximate ranges
			double appRangeCode;
			double appRangePhase;
			if (goGPS.Freq == 0)
			{
			  appRangeCode = rover.satAppRange[i] + Constants.SPEED_OF_LIGHT * (rover.clockError - sats.pos[i].SatelliteClockError) + rover.satTropoCorr[i] + rover.satIonoCorr[i];
			  appRangePhase = rover.satAppRange[i] + Constants.SPEED_OF_LIGHT * (rover.clockError - sats.pos[i].SatelliteClockError) + rover.satTropoCorr[i] - rover.satIonoCorr[i];
			}
			else
			{
			  appRangeCode = rover.satAppRange[i] + Constants.SPEED_OF_LIGHT * (rover.clockError - sats.pos[i].SatelliteClockError) + rover.satTropoCorr[i] + rover.satIonoCorr[i] * Math.Pow(roverObs.getSatByIDType(id, satType).getWavelength(1) / roverObs.getSatByIDType(id, satType).getWavelength(0), 2);
			  appRangePhase = rover.satAppRange[i] + Constants.SPEED_OF_LIGHT * (rover.clockError - sats.pos[i].SatelliteClockError) + rover.satTropoCorr[i] - rover.satIonoCorr[i] * Math.Pow(roverObs.getSatByIDType(id, satType).getWavelength(1) / roverObs.getSatByIDType(id, satType).getWavelength(0), 2);
			}

			// Fill in one row in the design matrix (for code)
			H.set(k, 0, alphaX);
			H.set(k, i1 + 1, alphaY);
			H.set(k, i2 + 1, alphaZ);

			// Fill in one element of the observation vector (for code)
			y0.set(k, 0, obsRangeCode - appRangeCode + alphaX * rover.X + alphaY * rover.Y + alphaZ * rover.Z);

			// Fill in the observation error covariance matrix (for code)
			double roverSatWeight = computeWeight(rover.topo[i].Elevation, roverObs.getSatByIDType(id, satType).getSignalStrength(goGPS.Freq));
			double CnnBase = Cnn.get(k, k);
			Cnn.set(k, k, CnnBase + Math.Pow(getStDevCode(roverObs.getSatByIDType(id, satType), goGPS.Freq), 2) * roverSatWeight);

			if (sats.gnssAvail.Contains(checkAvailGnss))
			{
	//        if (sats.availPhase.contains(id) && sats.typeAvailPhase.contains(satType)) {

			  // Fill in one row in the design matrix (for phase)
			  H.set(nObsAvail + p, 0, alphaX);
			  H.set(nObsAvail + p, i1 + 1, alphaY);
			  H.set(nObsAvail + p, i2 + 1, alphaZ);
			  H.set(nObsAvail + p, i3 + id, -roverObs.getSatByIDType(id, satType).getWavelength(goGPS.Freq));

			  // Fill in one element of the observation vector (for phase)
			  y0.set(nObsAvail + p, 0, obsRangePhase - appRangePhase + alphaX * rover.X + alphaY * rover.Y + alphaZ * rover.Z);

			  // Fill in the observation error covariance matrix (for phase)
			  CnnBase = Cnn.get(nObsAvail + p, nObsAvail + p);
			  Cnn.set(nObsAvail + p, nObsAvail + p, CnnBase + Math.Pow(stDevPhase, 2) * roverSatWeight);

			  // Increment satellites with phase counter
			  p++;
			}

			// Increment available satellites counter
			k++;
		  }
		}

		updateDops(A);
	  }

	  /// <param name="roverObs"> </param>
	  internal override void estimateAmbiguities(Observations roverObs, Observations masterObs, Coordinates masterPos, List<int?> satAmb, int pivotIndex, bool init)
	  {

		// Number of GPS observations
		int nObs = roverObs.NumSat;

		// Number of available satellites (i.e. observations)
		int nObsAvail = sats.avail.Count;

		// Number of available satellites (i.e. observations) with phase
		int nObsAvailPhase = sats.availPhase.Count;

		// Number of unknown parameters
		int nUnknowns = 4 + satAmb.Count;

		// Estimated ambiguities
		double[] estimatedAmbiguities = new double[satAmb.Count];

		// Covariance of estimated ambiguity combinations
		double[] estimatedAmbiguitiesCovariance = new double[satAmb.Count];

		// Least squares design matrix
		SimpleMatrix A = new SimpleMatrix(nObsAvail + nObsAvailPhase, nUnknowns);

		// Vector for approximate pseudoranges
		SimpleMatrix b = new SimpleMatrix(nObsAvail + nObsAvailPhase, 1);

		// Vector for observed pseudoranges
		SimpleMatrix y0 = new SimpleMatrix(nObsAvail + nObsAvailPhase, 1);

		// Cofactor matrices
		SimpleMatrix Qcode = new SimpleMatrix(nObsAvail, nObsAvail);
		SimpleMatrix Qphase = new SimpleMatrix(nObsAvailPhase, nObsAvailPhase);
		SimpleMatrix Q = new SimpleMatrix(nObsAvail + nObsAvailPhase, nObsAvail + nObsAvailPhase);

		// Solution vector
		SimpleMatrix x = new SimpleMatrix(nUnknowns, 1);

		// Vector for observation error
		SimpleMatrix vEstim = new SimpleMatrix(nObsAvail, 1);

		// Error covariance matrix
		SimpleMatrix covariance = new SimpleMatrix(nUnknowns, nUnknowns);

		// Vectors for troposphere and ionosphere corrections
		SimpleMatrix tropoCorr = new SimpleMatrix(nObsAvail + nObsAvailPhase, 1);
		SimpleMatrix ionoCorr = new SimpleMatrix(nObsAvail + nObsAvailPhase, 1);

		// Counters for available satellites
		int k = 0;
		int p = 0;

		// Set up the least squares matrices...
		// ... for code ...
		for (int i = 0; i < nObs; i++)
		{

		  int id = roverObs.getSatID(i);
		  char satType = roverObs.getGnssType(i);
		  string checkAvailGnss = Convert.ToString(satType) + Convert.ToString(id);

		  if (sats.pos[i] != null && sats.gnssAvail.Contains(checkAvailGnss))
		  {

			// Fill in one row in the design matrix
			A.set(k, 0, rover.diffSat[i].get(0) / rover.satAppRange[i]); // X
			A.set(k, 1, rover.diffSat[i].get(1) / rover.satAppRange[i]); // Y
			A.set(k, 2, rover.diffSat[i].get(2) / rover.satAppRange[i]); // Z

			A.set(k, 3, 1); // clock error

			// Add the approximate pseudorange value to b
			b.set(k, 0, rover.satAppRange[i] - sats.pos[i].SatelliteClockError * Constants.SPEED_OF_LIGHT);

			// Add the observed pseudorange value to y0
			y0.set(k, 0, roverObs.getSatByIDType(id, satType).getPseudorange(goGPS.Freq));

			// Fill in troposphere and ionosphere double corrections
			tropoCorr.set(k, 0, rover.satTropoCorr[i]);
			ionoCorr.set(k, 0, rover.satIonoCorr[i]);

			// Fill in the cofactor matrix
			double roverSatWeight = computeWeight(rover.topo[i].Elevation, roverObs.getSatByIDType(id, satType).getSignalStrength(goGPS.Freq));
			Qcode.set(k, k, Qcode.get(k, k) + getStDevCode(roverObs.getSatByID(id), goGPS.Freq) * roverSatWeight);

			// Increment available satellites counter
			k++;
		  }
		}

		// ... and phase
		for (int i = 0; i < nObs; i++)
		{

		  int id = roverObs.getSatID(i);
		  char satType = roverObs.getGnssType(i);
		  string checkAvailGnss = Convert.ToString(satType) + Convert.ToString(id);

		  if (sats.pos[i] != null && sats.gnssAvail.Contains(checkAvailGnss))
		  {

			// Fill in one row in the design matrix
			A.set(k, 0, rover.diffSat[i].get(0) / rover.satAppRange[i]); // X
			A.set(k, 1, rover.diffSat[i].get(1) / rover.satAppRange[i]); // Y
			A.set(k, 2, rover.diffSat[i].get(2) / rover.satAppRange[i]); // Z
			A.set(k, 3, 1); // clock error

			if (satAmb.Contains(id))
			{
			  A.set(k, 4 + satAmb.IndexOf(id), -roverObs.getSatByIDType(id, satType).getWavelength(goGPS.Freq)); // N

			  // Add the observed phase range value to y0
			  y0.set(k, 0, roverObs.getSatByIDType(id, satType).getPhaserange(goGPS.Freq));
			}
			else
			{
			  // Add the observed phase range value + known N to y0
			  y0.set(k, 0, roverObs.getSatByIDType(id, satType).getPhaserange(goGPS.Freq) + KFprediction.get(i3 + id) * roverObs.getSatByIDType(id, satType).getWavelength(goGPS.Freq));
			}

			// Add the approximate pseudorange value to b
			b.set(k, 0, rover.satAppRange[i] - sats.pos[i].SatelliteClockError * Constants.SPEED_OF_LIGHT);

			// Fill in troposphere and ionosphere corrections
			tropoCorr.set(k, 0, rover.satTropoCorr[i]);
			ionoCorr.set(k, 0, -rover.satIonoCorr[i]);

			// Fill in the cofactor matrix
			double roverSatWeight = computeWeight(rover.topo[i].Elevation, roverObs.getSatByIDType(id, satType).getSignalStrength(goGPS.Freq));
			Qphase.set(p, p, Qphase.get(p, p) + Math.Pow(stDevPhase, 2) * roverSatWeight);

			int r = 1;
			for (int m = i + 1; m < nObs; m++)
			{
			  if (sats.pos[m] != null && sats.availPhase.Contains(sats.pos[m].SatID))
			  {
				Qphase.set(p, p + r, 0);
				Qphase.set(p + r, p, 0);
				r++;
			  }
			}

			// Increment available satellite counters
			k++;
			p++;
		  }
		}

		// Apply troposphere and ionosphere correction
		b = b.plus(tropoCorr);
		b = b.plus(ionoCorr);

		//Build complete cofactor matrix (code and phase)
		Q.insertIntoThis(0, 0, Qcode);
		Q.insertIntoThis(nObsAvail, nObsAvail, Qphase);

		// Least squares solution x = ((A'*Q^-1*A)^-1)*A'*Q^-1*(y0-b);
		x = A.transpose().mult(Q.invert()).mult(A).invert().mult(A.transpose()).mult(Q.invert()).mult(y0.minus(b));

		// Receiver clock error
		rover.clockError = x.get(3) / Constants.SPEED_OF_LIGHT;

		// Estimation of the variance of the observation error
		vEstim = y0.minus(A.mult(x).plus(b));
		double varianceEstim = (vEstim.transpose().mult(Q.invert()).mult(vEstim)).get(0) / (nObsAvail + nObsAvailPhase - nUnknowns);

		// Covariance matrix of the estimation error
		covariance = A.transpose().mult(Q.invert()).mult(A).invert().scale(varianceEstim);

		// Store estimated ambiguity combinations and their covariance
		for (int m = 0; m < satAmb.Count; m++)
		{
		  estimatedAmbiguities[m] = x.get(4 + m);
		  estimatedAmbiguitiesCovariance[m] = covariance.get(4 + m, 4 + m);
		}

		if (init)
		{
		  for (int i = 0; i < satAmb.Count; i++)
		  {
			// Estimated ambiguity
			KFstate.set(i3 + satAmb[i], 0, estimatedAmbiguities[i]);

			// Store the variance of the estimated ambiguity
			Cee.set(i3 + satAmb[i], i3 + satAmb[i], estimatedAmbiguitiesCovariance[i]);
		  }
		}
		else
		{
		  for (int i = 0; i < satAmb.Count; i++)
		  {
			// Estimated ambiguity
			KFprediction.set(i3 + satAmb[i], 0, estimatedAmbiguities[i]);

			// Store the variance of the estimated ambiguity
			Cvv.set(i3 + satAmb[i], i3 + satAmb[i], Math.Pow(stDevAmbiguity, 2));
		  }
		}
	  }

	  /// <param name="roverObs"> </param>
	  /// <param name="masterObs"> </param>
	  /// <param name="masterPos"> </param>
	  internal override void checkSatelliteConfiguration(Observations roverObs, Observations masterObs, Coordinates masterPos)
	  {

		// Lists for keeping track of satellites that need ambiguity (re-)estimation
		List<int?> newSatellites = new List<int?>(0);
		List<int?> slippedSatellites = new List<int?>(0);

		// Check if satellites were lost since the previous epoch
		for (int i = 0; i < satOld.Count; i++)
		{

		  // Set ambiguity of lost satellites to zero
	//      if (!sats.gnssAvailPhase.contains(satOld.get(i))) {
		  if (!sats.availPhase.Contains(satOld[i]) && sats.typeAvailPhase.Contains(satOld[i]))
		  {

			if (goGPS.Debug)
			{
				Console.WriteLine("Lost satellite " + satOld[i]);
			}

			KFprediction.set(i3 + satOld[i], 0, 0);
		  }
		}

		// Check if new satellites are available since the previous epoch
		for (int i = 0; i < sats.pos.Length; i++)
		{

		  if (sats.pos[i] != null && sats.availPhase.Contains(sats.pos[i].SatID) && sats.typeAvailPhase.Contains(sats.pos[i].SatType) && !satOld.Contains(sats.pos[i].SatID) && satTypeOld.Contains(sats.pos[i].SatType))
		  {

			newSatellites.Add(sats.pos[i].SatID);
			if (goGPS.Debug)
			{
				Console.WriteLine("New satellite " + sats.pos[i].SatID);
			}
		  }
		}

		for (int i = 0; i < roverObs.NumSat; i++)
		{

		  int satID = roverObs.getSatID(i);
		  char satType = roverObs.getGnssType(i);
		  string checkAvailGnss = Convert.ToString(satType) + Convert.ToString(satID);

		  bool lossOfLockCycleSlipRover;
		  if (sats.gnssAvailPhase.Contains(checkAvailGnss))
		  {
			// cycle slip detected by loss of lock indicator (disabled)
			lossOfLockCycleSlipRover = roverObs.getSatByIDType(satID, satType).isPossibleCycleSlip(goGPS.Freq);
			lossOfLockCycleSlipRover = false;

			// cycle slip detected by Doppler predicted phase range
			bool dopplerCycleSlipRover;
			if (goGPS.CycleSlipDetectionStrategy == GoGPS.CycleSlipDetectionStrategy.DOPPLER_PREDICTED_PHASE_RANGE)
			{
			  dopplerCycleSlipRover = rover.getDopplerPredictedPhase(satID) != 0.0 && (Math.Abs(roverObs.getSatByIDType(satID, satType).getPhaseCycles(goGPS.Freq) - rover.getDopplerPredictedPhase(satID)) > goGPS.CycleSlipThreshold);
			}
			else
			{
			  dopplerCycleSlipRover = false;
			}

			// Rover-satellite observed pseudorange
			double roverSatCodeObs = roverObs.getSatByIDType(satID, satType).getCodeC(goGPS.Freq);

			// Rover-satellite observed phase
			double roverSatPhaseObs = roverObs.getSatByIDType(satID, satType).getPhaserange(goGPS.Freq);

			// Store estimated ambiguity combinations and their covariance
			double estimatedAmbiguity = (roverSatCodeObs - roverSatPhaseObs - 2 * rover.satIonoCorr[i]) / roverObs.getSatByIDType(satID, satType).getWavelength(goGPS.Freq);

			bool obsCodeCycleSlip = Math.Abs(KFprediction.get(i3 + satID) - estimatedAmbiguity) > goGPS.CycleSlipThreshold;

			bool cycleSlip = (lossOfLockCycleSlipRover || dopplerCycleSlipRover || obsCodeCycleSlip);

			if (!newSatellites.Contains(satID) && cycleSlip)
			{

			  slippedSatellites.Add(satID);

			  if (dopplerCycleSlipRover)
			  {
				if (goGPS.Debug)
				{
					Console.WriteLine("[ROVER] Cycle slip on satellite " + satID + " (range diff = " + Math.Abs(roverObs.getSatByIDType(satID, satType).getPhaseCycles(goGPS.Freq) - rover.getDopplerPredictedPhase(satID)) + ")");
				}
			  }
			}
		  }
		}

		// Ambiguity estimation
		if (newSatellites.Count != 0 || slippedSatellites.Count != 0)
		{
		  // List of satellites that need ambiguity estimation
		  List<int?> satAmb = newSatellites;
		  satAmb.AddRange(slippedSatellites);
		  estimateAmbiguities(roverObs, null, null, satAmb, 0, false);
		}
	  }

	  /// <summary>
	  /// Run kalman filter on code and phase standalone.
	  /// </summary>
	  public static void run(GoGPS goGPS)
	  {

		RoverPosition rover = goGPS.getRoverPos();
		MasterPosition master = goGPS.getMasterPos();
		Satellites sats = goGPS.Sats;
		ObservationsProducer roverIn = goGPS.RoverIn;
		ObservationsProducer masterIn = goGPS.MasterIn;
		bool debug = goGPS.Debug;

		long timeRead = DateTimeHelperClass.CurrentUnixTimeMillis();
		long depRead = 0;

		long timeProc = 0;
		long depProc = 0;

		KalmanFilter kf = new KF_SA_code_phase(goGPS);

		// Flag to check if Kalman filter has been initialized
		bool kalmanInitialized = false;

		try
		{
		  bool validPosition = false;

		  timeRead = DateTimeHelperClass.CurrentUnixTimeMillis() - timeRead;
		  depRead = depRead + timeRead;

		  Observations obsR = roverIn.NextObservations;

		  while (obsR != null)
		  {

			if (debug)
			{
				Console.WriteLine("R:" + obsR.RefTime.Msec);
			}

			timeRead = DateTimeHelperClass.CurrentUnixTimeMillis();
			depRead = depRead + timeRead;
			timeProc = DateTimeHelperClass.CurrentUnixTimeMillis();

			// If Kalman filter was not initialized and if there are at least four satellites
			bool valid = true;
			if (!kalmanInitialized && obsR.NumSat >= 4)
			{

			 if (roverIn.DefinedPosition != null)
			 {
				   roverIn.DefinedPosition.cloneInto(rover);
			 }

			  // Compute approximate positioning by iterative least-squares
			  for (int iter = 0; iter < 3; iter++)
			  {
				// Select all satellites
				sats.selectStandalone(obsR, -100);

				if (sats.AvailNumber >= 4)
				{
				  kf.codeStandalone(obsR, false, true);
				}
			  }

			  // If an approximate position was computed
			  if (rover.ValidXYZ)
			  {

				// Initialize Kalman filter
				kf.init(obsR, null, roverIn.DefinedPosition);

				if (rover.ValidXYZ)
				{
				  kalmanInitialized = true;
				  if (debug)
				  {
					  Console.WriteLine("Kalman filter initialized.");
				  }
				}
				else
				{
				  if (debug)
				  {
					  Console.WriteLine("Kalman filter not initialized.");
				  }
				}
			  }
			  else
			  {
				if (debug)
				{
					Console.WriteLine("A-priori position (from code observations) is not valid.");
				}
			  }
			}
			else if (kalmanInitialized)
			{

			  // Do a Kalman filter loop
			  try
			  {
				kf.loop(obsR, null, roverIn.DefinedPosition);
			  }
			  catch (Exception e)
			  {
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
				valid = false;
			  }
			}

			timeProc = DateTimeHelperClass.CurrentUnixTimeMillis() - timeProc;
			depProc = depProc + timeProc;

			if (kalmanInitialized && valid)
			{
			  if (!validPosition)
			  {
				goGPS.notifyPositionConsumerEvent(org.gogpsproject.consumer.PositionConsumer_Fields.EVENT_START_OF_TRACK);
				validPosition = true;
			  }
			  else
			  {
				if (goGPS.PositionConsumers.Count > 0)
				{
				  RoverPosition coord = new RoverPosition(rover, DopType.KALMAN, rover.KpDop, rover.KhDop, rover.KvDop);
				  coord.RefTime = new Time(obsR.RefTime.Msec);
				  coord.obs = obsR;
				  coord.sampleTime = obsR.RefTime;
				  coord.status = rover.status;
				  goGPS.notifyPositionConsumerAddCoordinate(coord);
				}
			  }

			}
			//System.out.println("--------------------");

			if (debug)
			{
				Console.WriteLine("-- Get next epoch ---------------------------------------------------");
			}
			// get next epoch
			obsR = roverIn.NextObservations;
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

		int elapsedTimeSec = (int) Math.Floor(depRead / 1000);
		int elapsedTimeMillisec = (int)(depRead - elapsedTimeSec * 1000);
		if (debug)
		{
			Console.WriteLine("\nElapsed time (read): " + elapsedTimeSec + " seconds " + elapsedTimeMillisec + " milliseconds.");
		}

		elapsedTimeSec = (int) Math.Floor(depProc / 1000);
		elapsedTimeMillisec = (int)(depProc - elapsedTimeSec * 1000);
		if (debug)
		{
			Console.WriteLine("\nElapsed time (proc): " + elapsedTimeSec + " seconds " + elapsedTimeMillisec + " milliseconds.");
		}
	  }

	}

}