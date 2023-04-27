using System;

namespace org.gogpsproject.positioning
{

	using SimpleMatrix = org.ejml.simple.SimpleMatrix;
	using PositionConsumer = org.gogpsproject.consumer.PositionConsumer;
	using DopType = org.gogpsproject.positioning.RoverPosition.DopType;
	using Observations = org.gogpsproject.producer.Observations;
	using ObservationsProducer = org.gogpsproject.producer.ObservationsProducer;

	public class LS_DD_code : LS_SA_code
	{

	  public LS_DD_code(GoGPS goGPS) : base(goGPS)
	  {
	  }

	  /// <param name="roverObs"> </param>
	  /// <param name="masterObs"> </param>
	  /// <param name="masterPos"> </param>
	  public virtual void codeDoubleDifferences(Observations roverObs, Observations masterObs, Coordinates masterPos)
	  {

		// Number of GPS observations
		int nObs = roverObs.NumSat;

		// Number of unknown parameters
		int nUnknowns = 3;

		// Number of available satellites (i.e. observations)
		int nObsAvail = sats.avail.Count;

		// Full design matrix for DOP computation
		SimpleMatrix Adop = new SimpleMatrix(nObsAvail, 3);

		// Double differences with respect to pivot satellite reduce
		// observations by 1
		nObsAvail--;

		// Least squares design matrix
		SimpleMatrix A = new SimpleMatrix(nObsAvail, nUnknowns);

		// Vector for approximate pseudoranges
		SimpleMatrix b = new SimpleMatrix(nObsAvail, 1);

		// Vector for observed pseudoranges
		SimpleMatrix y0 = new SimpleMatrix(nObsAvail, 1);

		// Cofactor matrix
		SimpleMatrix Q = new SimpleMatrix(nObsAvail, nObsAvail);

		// Solution vector
		SimpleMatrix x = new SimpleMatrix(nUnknowns, 1);

		// Vector for observation error
		SimpleMatrix vEstim = new SimpleMatrix(nObsAvail, 1);

		// Vectors for troposphere and ionosphere corrections
		SimpleMatrix tropoCorr = new SimpleMatrix(nObsAvail, 1);
		SimpleMatrix ionoCorr = new SimpleMatrix(nObsAvail, 1);

		// Counter for available satellites (with pivot)
		int d = 0;

		// Pivot satellite index
		int pivotId = roverObs.getSatID(sats.pivot);
		char satType = roverObs.getGnssType(sats.pivot);

		// Store rover-pivot and master-pivot observed pseudoranges
		double roverPivotObs = roverObs.getSatByIDType(pivotId, satType).getPseudorange(goGPS.Freq);
		double masterPivotObs = masterObs.getSatByIDType(pivotId, satType).getPseudorange(goGPS.Freq);

		// Rover-pivot approximate pseudoranges
		SimpleMatrix diffRoverPivot = rover.diffSat[sats.pivot];
		double roverPivotAppRange = rover.satAppRange[sats.pivot];

		// Master-pivot approximate pseudoranges
		double masterPivotAppRange = master.satAppRange[sats.pivot];

		// Computation of rover-pivot troposphere correction
		double roverPivotTropoCorr = rover.satTropoCorr[sats.pivot];

		// Computation of master-pivot troposphere correction
		double masterPivotTropoCorr = master.satTropoCorr[sats.pivot];

		// Computation of rover-pivot ionosphere correction
		double roverPivotIonoCorr = rover.satIonoCorr[sats.pivot];

		// Computation of master-pivot ionosphere correction
		double masterPivotIonoCorr = master.satIonoCorr[sats.pivot];

		// Compute rover-pivot and master-pivot weights
		double roverPivotWeight = computeWeight(rover.topo[sats.pivot].Elevation, roverObs.getSatByIDType(pivotId, satType).getSignalStrength(goGPS.Freq));
		double masterPivotWeight = computeWeight(master.topo[sats.pivot].Elevation, masterObs.getSatByIDType(pivotId, satType).getSignalStrength(goGPS.Freq));
		Q.set(roverPivotWeight + masterPivotWeight);

		// Set up the least squares matrices
		for (int i = 0, k = 0; i < nObs; i++)
		{

		  // Satellite ID
		  int id = roverObs.getSatID(i);
		  satType = roverObs.getGnssType(i);
		  string checkAvailGnss = Convert.ToString(satType) + Convert.ToString(id);

		  if (sats.pos[i] != null && sats.gnssAvail.Contains(checkAvailGnss) && i != sats.pivot)
		  {
	//      if (sats.pos[i] !=null && sats.avail.contains(id) && satTypeAvail.contains(satType) && i != pivot) {

			// Fill in one row in the design matrix
			A.set(k, 0, rover.diffSat[i].get(0) / rover.satAppRange[i] - diffRoverPivot.get(0) / roverPivotAppRange); // X

			A.set(k, 1, rover.diffSat[i].get(1) / rover.satAppRange[i] - diffRoverPivot.get(1) / roverPivotAppRange); // Y

			A.set(k, 2, rover.diffSat[i].get(2) / rover.satAppRange[i] - diffRoverPivot.get(2) / roverPivotAppRange); // Z

			// Add the differenced approximate pseudorange value to b
			b.set(k, 0, (rover.satAppRange[i] - master.satAppRange[i]) - (roverPivotAppRange - masterPivotAppRange));

			// Add the differenced observed pseudorange value to y0
			y0.set(k, 0, (roverObs.getSatByIDType(id, satType).getPseudorange(goGPS.Freq) - masterObs.getSatByIDType(id, satType).getPseudorange(goGPS.Freq)) - (roverPivotObs - masterPivotObs));

			// Fill in troposphere and ionosphere double differenced
			// corrections
			tropoCorr.set(k, 0, (rover.satTropoCorr[i] - master.satTropoCorr[i]) - (roverPivotTropoCorr - masterPivotTropoCorr));
			ionoCorr.set(k, 0, (rover.satIonoCorr[i] - master.satIonoCorr[i]) - (roverPivotIonoCorr - masterPivotIonoCorr));

			// Fill in the cofactor matrix
			double roverSatWeight = computeWeight(rover.topo[i].Elevation, roverObs.getSatByIDType(id, satType).getSignalStrength(goGPS.Freq));
			double masterSatWeight = computeWeight(master.topo[i].Elevation, masterObs.getSatByIDType(id, satType).getSignalStrength(goGPS.Freq));

			Q.set(k, k, Q.get(k, k) + roverSatWeight + masterSatWeight);

			// Increment available satellites counter
			k++;
		  }

		  // Design matrix for DOP computation
		  if (sats.pos[i] != null && sats.gnssAvail.Contains(checkAvailGnss))
		  {
	//      if (sats.pos[i] != null && sats.avail.contains(id) && satTypeAvail.contains(satType)) {
			// Fill in one row in the design matrix (complete one, for DOP)
			Adop.set(d, 0, rover.diffSat[i].get(0) / rover.satAppRange[i]); // X
			Adop.set(d, 1, rover.diffSat[i].get(1) / rover.satAppRange[i]); // Y
			Adop.set(d, 2, rover.diffSat[i].get(2) / rover.satAppRange[i]); // Z
			d++;
		  }
		}

		// Apply troposphere and ionosphere correction
		b = b.plus(tropoCorr);
		b = b.plus(ionoCorr);

		// Least squares solution x = ((A'*Q^-1*A)^-1)*A'*Q^-1*(y0-b);
		x = A.transpose().mult(Q.invert()).mult(A).invert().mult(A.transpose()).mult(Q.invert()).mult(y0.minus(b));

		// Receiver position
		rover.PlusXYZ = x;

		// Estimation of the variance of the observation error
		vEstim = y0.minus(A.mult(x).plus(b));
		double varianceEstim = (vEstim.transpose().mult(Q.invert()).mult(vEstim)).get(0) / (nObsAvail - nUnknowns);

		// Covariance matrix of the estimation error
		if (nObsAvail > nUnknowns)
		{
		  SimpleMatrix covariance = A.transpose().mult(Q.invert()).mult(A).invert().scale(varianceEstim);
		  positionCovariance = covariance.extractMatrix(0, 3, 0, 3);
		}
		else
		{
		  positionCovariance = null;
		}

		updateDops(Adop);

		// Compute positioning in geodetic coordinates
		rover.computeGeodetic();
	  }

	  /// <summary>
	  /// Run code double differences.
	  /// </summary>
	  public static void run(GoGPS goGPS)
	  {

		RoverPosition rover = goGPS.getRoverPos();
		MasterPosition master = goGPS.getMasterPos();
		Satellites sats = goGPS.Sats;
		ObservationsProducer roverIn = goGPS.RoverIn;
		ObservationsProducer masterIn = goGPS.MasterIn;
		bool debug = goGPS.Debug;
		bool validPosition = false;

		try
		{
		  LS_DD_code dd = new LS_DD_code(goGPS);

		  Observations obsR = roverIn.NextObservations;
		  Observations obsM = masterIn.NextObservations;

		  while (obsR != null && obsM != null)
		  {

			// Discard master epochs if correspondent rover epochs are not available
			double obsRtime = obsR.RefTime.RoundedGpsTime;
			while (obsM != null && obsR != null && obsRtime > obsM.RefTime.RoundedGpsTime)
			{
			  obsM = masterIn.NextObservations;
			}

			// Discard rover epochs if correspondent master epochs are not available
			double obsMtime = obsM.RefTime.RoundedGpsTime;
			while (obsM != null && obsR != null && obsR.RefTime.RoundedGpsTime < obsMtime)
			{
			  obsR = roverIn.NextObservations;
			}


			// If there are at least four satellites
			if (obsM != null && obsR != null)
			{
			  if (obsR.NumSat >= 4)
			  {

				// Compute approximate positioning by iterative least-squares
				for (int iter = 0; iter < 3; iter++)
				{

				  // Select all satellites
				  sats.selectStandalone(obsR, -100);

				  if (sats.AvailNumber >= 4)
				  {
					dd.codeStandalone(obsR, false, true);
				  }
				}

				// If an approximate position was computed
				if (rover.ValidXYZ)
				{

				  // Select satellites available for double differences
				  sats.selectDoubleDiff(obsR, obsM, masterIn.DefinedPosition);

				  if (sats.AvailNumber >= 4)
					// Compute code double differences positioning
					// (epoch-by-epoch solution)
				  {
					dd.codeDoubleDifferences(obsR, obsM, masterIn.DefinedPosition);
				  }
				  else
				  {
					// Discard approximate positioning
					rover.setXYZ(0, 0, 0);
				  }
				}

				if (rover.ValidXYZ)
				{
				  if (!validPosition)
				  {
					goGPS.notifyPositionConsumerEvent(org.gogpsproject.consumer.PositionConsumer_Fields.EVENT_START_OF_TRACK);
					validPosition = true;
				  }
				  else
				  {
					RoverPosition coord = new RoverPosition(rover, DopType.KALMAN, rover.getpDop(), rover.gethDop(), rover.getvDop());

					if (goGPS.PositionConsumers.Count > 0)
					{
					  coord.RefTime = new Time(obsR.RefTime.Msec);
					  goGPS.notifyPositionConsumerAddCoordinate(coord);
					}
					if (debug)
					{
						Console.WriteLine("-------------------- " + rover.getpDop());
					}
				  }
				}
			  }
			}
			// get next epoch
			obsR = roverIn.NextObservations;
			obsM = masterIn.NextObservations;
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