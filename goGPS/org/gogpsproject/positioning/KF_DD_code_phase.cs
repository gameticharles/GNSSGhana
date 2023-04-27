using System;
using System.Collections.Generic;

namespace org.gogpsproject.positioning
{

	using SimpleMatrix = org.ejml.simple.SimpleMatrix;
	using org.gogpsproject.GoGPS;
	using PositionConsumer = org.gogpsproject.consumer.PositionConsumer;
	using DopType = org.gogpsproject.positioning.RoverPosition.DopType;
	using Observations = org.gogpsproject.producer.Observations;
	using ObservationsProducer = org.gogpsproject.producer.ObservationsProducer;

	public class KF_DD_code_phase : KalmanFilter
	{

	  public KF_DD_code_phase(GoGPS goGPS) : base(goGPS)
	  {
	  }

	  internal override void setup(Observations roverObs, Observations masterObs, Coordinates masterPos) // Definition of matrices
	  {

		// Number of GPS observations
		int nObs = roverObs.NumSat;

		// Number of available satellites (i.e. observations)
		int nObsAvail = sats.avail.Count;

		// Double differences with respect to pivot satellite reduce observations by 1
		nObsAvail--;

		// Matrix containing parameters obtained from the linearization of the observation equations
		SimpleMatrix A = new SimpleMatrix(nObsAvail, 3);

		// Pivot satellite ID
		int pivotId = roverObs.getSatID(sats.pivot);
		char satType = roverObs.getGnssType(sats.pivot);

		// Store rover-pivot and master-pivot observed pseudoranges
		double roverPivotCodeObs = roverObs.getSatByIDType(pivotId, satType).getPseudorange(goGPS.Freq);
		double masterPivotCodeObs = masterObs.getSatByIDType(pivotId, satType).getPseudorange(goGPS.Freq);

		// Compute and store rover-pivot and master-pivot observed phase ranges
		double roverPivotPhaseObs = roverObs.getSatByIDType(pivotId, satType).getPhaserange(goGPS.Freq);
		double masterPivotPhaseObs = masterObs.getSatByIDType(pivotId, satType).getPhaserange(goGPS.Freq);

		// Rover-pivot approximate pseudoranges
		SimpleMatrix diffRoverPivot = rover.diffSat[sats.pivot];
		double roverPivotAppRange = rover.satAppRange[sats.pivot];

		// Master-pivot approximate pseudoranges
		double masterPivotAppRange = master.satAppRange[sats.pivot];

		// Rover-pivot and master-pivot troposphere correction
		double roverPivotTropoCorr = rover.satTropoCorr[sats.pivot];
		double masterPivotTropoCorr = master.satTropoCorr[sats.pivot];

		// Rover-pivot and master-pivot ionosphere correction
		double roverPivotIonoCorr = rover.satIonoCorr[sats.pivot];
		double masterPivotIonoCorr = master.satIonoCorr[sats.pivot];

		// Compute rover-pivot and master-pivot weights
		double roverElevation = rover.topo[sats.pivot].Elevation;
		double masterElevation = master.topo[sats.pivot].Elevation;
		double roverPivotWeight = computeWeight(roverElevation, roverObs.getSatByIDType(pivotId, satType).getSignalStrength(goGPS.Freq));
		double masterPivotWeight = computeWeight(masterElevation, masterObs.getSatByIDType(pivotId, satType).getSignalStrength(goGPS.Freq));

		// Start filling in the observation error covariance matrix
		Cnn.zero();
		int nSatAvail = sats.avail.Count - 1;
		int nSatAvailPhase = sats.availPhase.Count - 1;
		for (int i = 0; i < nSatAvail + nSatAvailPhase; i++)
		{
		  for (int j = 0; j < nSatAvail + nSatAvailPhase; j++)
		  {

			if (i < nSatAvail && j < nSatAvail)
			{
			  Cnn.set(i, j, getStDevCode(roverObs.getSatByIDType(pivotId, satType), goGPS.Freq) * getStDevCode(masterObs.getSatByIDType(pivotId, satType), goGPS.Freq) * (roverPivotWeight + masterPivotWeight));
			}
			else if (i >= nSatAvail && j >= nSatAvail)
			{
			  Cnn.set(i, j, Math.Pow(stDevPhase, 2) * (roverPivotWeight + masterPivotWeight));
			}
		  }
		}

		// Counter for available satellites
		int k = 0;

		// Counter for satellites with phase available
		int p = 0;

		for (int i = 0; i < nObs; i++)
		{

		  int id = roverObs.getSatID(i);
		  satType = roverObs.getGnssType(i);
		  string checkAvailGnss = Convert.ToString(satType) + Convert.ToString(id);

		  if (sats.pos[i] != null && sats.gnssAvail.Contains(checkAvailGnss) && i != sats.pivot)
		  {

			// Compute parameters obtained from linearization of observation equations
			double alphaX = rover.diffSat[i].get(0) / rover.satAppRange[i] - diffRoverPivot.get(0) / roverPivotAppRange;
			double alphaY = rover.diffSat[i].get(1) / rover.satAppRange[i] - diffRoverPivot.get(1) / roverPivotAppRange;
			double alphaZ = rover.diffSat[i].get(2) / rover.satAppRange[i] - diffRoverPivot.get(2) / roverPivotAppRange;

			// Fill in the A matrix
			A.set(k, 0, alphaX); // X
			A.set(k, 1, alphaY); // Y
			A.set(k, 2, alphaZ); // Z

			// Approximate code double difference
			double ddcApp = (rover.satAppRange[i] - master.satAppRange[i]) - (roverPivotAppRange - masterPivotAppRange);

			// Observed code double difference
			double ddcObs = (roverObs.getSatByIDType(id, satType).getPseudorange(goGPS.Freq) - masterObs.getSatByIDType(id, satType).getPseudorange(goGPS.Freq)) - (roverPivotCodeObs - masterPivotCodeObs);

			// Observed phase double difference
			double ddpObs = (roverObs.getSatByIDType(id, satType).getPhaserange(goGPS.Freq) - masterObs.getSatByIDType(id, satType).getPhaserange(goGPS.Freq)) - (roverPivotPhaseObs - masterPivotPhaseObs);

			// Compute troposphere and ionosphere residuals
			double tropoResiduals = (rover.satTropoCorr[i] - master.satTropoCorr[i]) - (roverPivotTropoCorr - masterPivotTropoCorr);
			double ionoResiduals = (rover.satIonoCorr[i] - master.satIonoCorr[i]) - (roverPivotIonoCorr - masterPivotIonoCorr);

			// Compute approximate ranges
			double appRangeCode;
			double appRangePhase;
			if (goGPS.Freq == 0)
			{
			  appRangeCode = ddcApp + tropoResiduals + ionoResiduals;
			  appRangePhase = ddcApp + tropoResiduals - ionoResiduals;
			}
			else
			{
			  appRangeCode = ddcApp + tropoResiduals + ionoResiduals * Math.Pow(roverObs.getSatByIDType(id, satType).getWavelength(1) / roverObs.getSatByIDType(id, satType).getWavelength(0), 2);
			  appRangePhase = ddcApp + tropoResiduals - ionoResiduals * Math.Pow(roverObs.getSatByIDType(id, satType).getWavelength(1) / roverObs.getSatByIDType(id, satType).getWavelength(0), 2);
			}

			// Fill in one row in the design matrix (for code)
			H.set(k, 0, alphaX);
			H.set(k, i1 + 1, alphaY);
			H.set(k, i2 + 1, alphaZ);

			// Fill in one element of the observation vector (for code)
			y0.set(k, 0, ddcObs - appRangeCode + alphaX * rover.X + alphaY * rover.Y + alphaZ * rover.Z);

			// Fill in the observation error covariance matrix (for code)
			double roverSatWeight = computeWeight(rover.topo[i].Elevation, roverObs.getSatByIDType(id, satType).getSignalStrength(goGPS.Freq));
			double masterSatWeight = computeWeight(master.topo[i].Elevation, masterObs.getSatByIDType(id, satType).getSignalStrength(goGPS.Freq));
			double CnnBase = Cnn.get(k, k);

			Cnn.set(k, k, CnnBase + getStDevCode(roverObs.getSatByIDType(id, satType), goGPS.Freq) * getStDevCode(masterObs.getSatByIDType(id, satType), goGPS.Freq) * (roverSatWeight + masterSatWeight));

			if (sats.gnssAvail.Contains(checkAvailGnss))
			{
	//        if (sats.availPhase.contains(id) && sats.typeAvailPhase.contains(satType)) {

			  // Fill in one row in the design matrix (for phase)
			  H.set(nObsAvail + p, 0, alphaX);
			  H.set(nObsAvail + p, i1 + 1, alphaY);
			  H.set(nObsAvail + p, i2 + 1, alphaZ);
			  H.set(nObsAvail + p, i3 + id, -roverObs.getSatByIDType(id, satType).getWavelength(goGPS.Freq));

			  // Fill in one element of the observation vector (for phase)
			  y0.set(nObsAvail + p, 0, ddpObs - appRangePhase + alphaX * rover.X + alphaY * rover.Y + alphaZ * rover.Z);

			  // Fill in the observation error covariance matrix (for
			  // phase)
			  CnnBase = Cnn.get(nObsAvail + p, nObsAvail + p);
			  Cnn.set(nObsAvail + p, nObsAvail + p, CnnBase + Math.Pow(stDevPhase, 2) * (roverSatWeight + masterSatWeight));

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
	  /// <param name="masterObs"> </param>
	  /// <param name="masterPos"> </param>
	  internal override void estimateAmbiguities(Observations roverObs, Observations masterObs, Coordinates masterPos, List<int?> satAmb, int pivotIndex, bool init)
	  {

		// Check if pivot is in satAmb, in case remove it
		if (satAmb.Contains(sats.pos[pivotIndex].SatID))
		{
		  satAmb.RemoveAt(satAmb.IndexOf(sats.pos[pivotIndex].SatID));
		}

		// Number of GPS observations
		int nObs = roverObs.NumSat;

		// Number of available satellites (i.e. observations)
		int nObsAvail = sats.avail.Count;

		// Number of available satellites (i.e. observations) with phase
		int nObsAvailPhase = sats.availPhase.Count;

		// Double differences with respect to pivot satellite reduce
		// observations by 1
		nObsAvail--;
		nObsAvailPhase--;

		// Number of unknown parameters
		int nUnknowns = 3 + satAmb.Count;

		// Pivot satellite ID
		int pivotId = roverObs.getSatID(pivotIndex);
		char satType = roverObs.getGnssType(pivotIndex);

		// Rover-pivot and master-pivot observed pseudorange
		double roverPivotCodeObs = roverObs.getSatByIDType(pivotId, satType).getPseudorange(goGPS.Freq);
		double masterPivotCodeObs = masterObs.getSatByIDType(pivotId, satType).getPseudorange(goGPS.Freq);

		// Rover-pivot and master-pivot observed phase
		double roverPivotPhaseObs = roverObs.getSatByIDType(pivotId, satType).getPhaserange(goGPS.Freq);
		double masterPivotPhaseObs = masterObs.getSatByIDType(pivotId, satType).getPhaserange(goGPS.Freq);

		// Rover-pivot approximate pseudoranges
		SimpleMatrix diffRoverPivot = rover.diffSat[pivotIndex];
		double roverPivotAppRange = rover.satAppRange[pivotIndex];

		// Master-pivot approximate pseudoranges
		double masterPivotAppRange = master.satAppRange[pivotIndex];

		// Estimated ambiguity combinations (double differences)
		double[] estimatedAmbiguityComb = new double[satAmb.Count];

		// Covariance of estimated ambiguity combinations
		double[] estimatedAmbiguityCombCovariance = new double[satAmb.Count];

		if (goGPS.AmbiguityStrategy == AmbiguityStrategy.OBSERV)
		{

		  for (int i = 0; i < nObs; i++)
		  {

			int id = roverObs.getSatID(i);
			satType = roverObs.getGnssType(i);

			if (sats.pos[i] != null && satAmb.Contains(id) && id != pivotId)
			{

			  // Rover-satellite and master-satellite observed code
			  double roverSatCodeObs = roverObs.getSatByIDType(id, satType).getPseudorange(goGPS.Freq);
			  double masterSatCodeObs = masterObs.getSatByIDType(id, satType).getPseudorange(goGPS.Freq);

			  // Rover-satellite and master-satellite observed phase
			  double roverSatPhaseObs = roverObs.getSatByIDType(id, satType).getPhaserange(goGPS.Freq);
			  double masterSatPhaseObs = masterObs.getSatByIDType(id, satType).getPhaserange(goGPS.Freq);

			  // Observed code double difference
			  double codeDoubleDiffObserv = (roverSatCodeObs - masterSatCodeObs) - (roverPivotCodeObs - masterPivotCodeObs);

			  // Observed phase double difference
			  double phaseDoubleDiffObserv = (roverSatPhaseObs - masterSatPhaseObs) - (roverPivotPhaseObs - masterPivotPhaseObs);

			  // Store estimated ambiguity combinations and their covariance
			  estimatedAmbiguityComb[satAmb.IndexOf(id)] = (codeDoubleDiffObserv - phaseDoubleDiffObserv) / roverObs.getSatByIDType(id, satType).getWavelength(goGPS.Freq);
			  estimatedAmbiguityCombCovariance[satAmb.IndexOf(id)] = 4 * getStDevCode(roverObs.getSatByIDType(id, satType), goGPS.Freq) * getStDevCode(masterObs.getSatByIDType(id, satType), goGPS.Freq) / Math.Pow(roverObs.getSatByIDType(id, satType).getWavelength(goGPS.Freq), 2);
			}
		  }
		}
		else if (goGPS.AmbiguityStrategy == AmbiguityStrategy.APPROX | (nObsAvail + nObsAvailPhase <= nUnknowns))
		{

		  for (int i = 0; i < nObs; i++)
		  {

			int id = roverObs.getSatID(i);
			satType = roverObs.getGnssType(i);

			if (sats.pos[i] != null && satAmb.Contains(id) && id != pivotId)
			{

			  // Rover-satellite and master-satellite approximate pseudorange
			  double roverSatCodeAppRange = rover.satAppRange[i];
			  double masterSatCodeAppRange = master.satAppRange[i];

			  // Rover-satellite and master-satellite observed phase
			  double roverSatPhaseObs = roverObs.getSatByIDType(id, satType).getPhaserange(goGPS.Freq);
			  double masterSatPhaseObs = masterObs.getSatByIDType(id, satType).getPhaserange(goGPS.Freq);

			  // Estimated code pseudorange double differences
			  double codeDoubleDiffApprox = (roverSatCodeAppRange - masterSatCodeAppRange) - (roverPivotAppRange - masterPivotAppRange);

			  // Observed phase double differences
			  double phaseDoubleDiffObserv = (roverSatPhaseObs - masterSatPhaseObs) - (roverPivotPhaseObs - masterPivotPhaseObs);

			  // Store estimated ambiguity combinations and their covariance
			  estimatedAmbiguityComb[satAmb.IndexOf(id)] = (codeDoubleDiffApprox - phaseDoubleDiffObserv) / roverObs.getSatByIDType(id, satType).getWavelength(goGPS.Freq);
			  estimatedAmbiguityCombCovariance[satAmb.IndexOf(id)] = 4 * getStDevCode(roverObs.getSatByIDType(id, satType), goGPS.Freq) * getStDevCode(masterObs.getSatByIDType(id, satType), goGPS.Freq) / Math.Pow(roverObs.getSatByIDType(id, satType).getWavelength(goGPS.Freq), 2);
			}
		  }
		}
		else if (goGPS.AmbiguityStrategy == AmbiguityStrategy.LS)
		{

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

		  // Rover-pivot and master-pivot troposphere correction
		  double roverPivotTropoCorr = rover.satTropoCorr[pivotIndex];
		  double masterPivotTropoCorr = master.satTropoCorr[pivotIndex];

		  // Rover-pivot and master-pivot ionosphere correction
		  double roverPivotIonoCorr = rover.satIonoCorr[pivotIndex];
		  double masterPivotIonoCorr = master.satIonoCorr[pivotIndex];

		  // Compute rover-pivot and master-pivot weights
		  double roverPivotWeight = computeWeight(rover.topo[pivotIndex].Elevation, roverObs.getSatByIDType(pivotId, satType).getSignalStrength(goGPS.Freq));
		  double masterPivotWeight = computeWeight(master.topo[pivotIndex].Elevation, masterObs.getSatByIDType(pivotId, satType).getSignalStrength(goGPS.Freq));
		  Qcode.set(getStDevCode(roverObs.getSatByIDType(pivotId, satType), goGPS.Freq) * getStDevCode(masterObs.getSatByIDType(pivotId, satType), goGPS.Freq) * (roverPivotWeight + masterPivotWeight));
		  Qphase.set(Math.Pow(stDevPhase, 2) * (roverPivotWeight + masterPivotWeight));

		  // Set up the least squares matrices...
		  // ... for code ...
		  for (int i = 0; i < nObs; i++)
		  {

			int id = roverObs.getSatID(i);
			satType = roverObs.getGnssType(i);
			string checkAvailGnss = Convert.ToString(satType) + Convert.ToString(id);

			if (sats.pos[i] != null && sats.gnssAvail.Contains(checkAvailGnss) && i != pivotIndex)
			{

			  // Fill in one row in the design matrix
			  A.set(k, 0, rover.diffSat[i].get(0) / rover.satAppRange[i] - diffRoverPivot.get(0) / roverPivotAppRange); // X

			  A.set(k, 1, rover.diffSat[i].get(1) / rover.satAppRange[i] - diffRoverPivot.get(1) / roverPivotAppRange); // Y

			  A.set(k, 2, rover.diffSat[i].get(2) / rover.satAppRange[i] - diffRoverPivot.get(2) / roverPivotAppRange); // Z

			  // Add the differenced approximate pseudorange value to b
			  b.set(k, 0, (rover.satAppRange[i] - master.satAppRange[i]) - (roverPivotAppRange - masterPivotAppRange));

			  // Add the differenced observed pseudorange value to y0
			  y0.set(k, 0, (roverObs.getSatByIDType(id, satType).getPseudorange(goGPS.Freq) - masterObs.getSatByIDType(id, satType).getPseudorange(goGPS.Freq)) - (roverPivotCodeObs - masterPivotCodeObs));

			  // Fill in troposphere and ionosphere double differenced
			  // corrections
			  tropoCorr.set(k, 0, (rover.satTropoCorr[i] - master.satTropoCorr[i]) - (roverPivotTropoCorr - masterPivotTropoCorr));
			  ionoCorr.set(k, 0, (rover.satIonoCorr[i] - master.satIonoCorr[i]) - (roverPivotIonoCorr - masterPivotIonoCorr));

			  // Fill in the cofactor matrix
			  double roverSatWeight = computeWeight(rover.topo[i].Elevation, roverObs.getSatByIDType(id, satType).getSignalStrength(goGPS.Freq));
			  double masterSatWeight = computeWeight(master.topo[i].Elevation, masterObs.getSatByIDType(id, satType).getSignalStrength(goGPS.Freq));
			  Qcode.set(k, k, Qcode.get(k, k) + getStDevCode(roverObs.getSatByID(id), goGPS.Freq) * getStDevCode(masterObs.getSatByIDType(id, satType), goGPS.Freq) * (roverSatWeight + masterSatWeight));

			  // Increment available satellites counter
			  k++;
			}
		  }

		  // ... and phase
		  for (int i = 0; i < nObs; i++)
		  {

			int id = roverObs.getSatID(i);
			satType = roverObs.getGnssType(i);
			string checkAvailGnss = Convert.ToString(satType) + Convert.ToString(id);

			if (sats.pos[i] != null && sats.gnssAvail.Contains(checkAvailGnss) && i != pivotIndex)
			{

			  // Fill in one row in the design matrix
			  A.set(k, 0, rover.diffSat[i].get(0) / rover.satAppRange[i] - diffRoverPivot.get(0) / roverPivotAppRange); // X
			  A.set(k, 1, rover.diffSat[i].get(1) / rover.satAppRange[i] - diffRoverPivot.get(1) / roverPivotAppRange); // Y
			  A.set(k, 2, rover.diffSat[i].get(2) / rover.satAppRange[i] - diffRoverPivot.get(2) / roverPivotAppRange); // Z

			  if (satAmb.Contains(id))
			  {
				A.set(k, 3 + satAmb.IndexOf(id), -roverObs.getSatByIDType(id, satType).getWavelength(goGPS.Freq)); // N

				// Add the differenced observed pseudorange value to y0
				y0.set(k, 0, (roverObs.getSatByIDType(id, satType).getPhaserange(goGPS.Freq) - masterObs.getSatByIDType(id, satType).getPhaserange(goGPS.Freq)) - (roverPivotPhaseObs - masterPivotPhaseObs));
			  }
			  else
			  {
				// Add the differenced observed pseudorange value + known N to y0
				y0.set(k, 0, (roverObs.getSatByIDType(id, satType).getPhaserange(goGPS.Freq) - masterObs.getSatByIDType(id, satType).getPhaserange(goGPS.Freq)) - (roverPivotPhaseObs - masterPivotPhaseObs) + KFprediction.get(i3 + id));
			  }

			  // Add the differenced approximate pseudorange value to b
			  b.set(k, 0, (rover.satAppRange[i] - master.satAppRange[i]) - (roverPivotAppRange - masterPivotAppRange));

			  // Fill in troposphere and ionosphere double differenced corrections
			  tropoCorr.set(k, 0, (rover.satTropoCorr[i] - master.satTropoCorr[i]) - (roverPivotTropoCorr - masterPivotTropoCorr));
			  ionoCorr.set(k, 0, -((rover.satIonoCorr[i] - master.satIonoCorr[i]) - (roverPivotIonoCorr - masterPivotIonoCorr)));

			  // Fill in the cofactor matrix
			  double roverSatWeight = computeWeight(rover.topo[i].Elevation, roverObs.getSatByIDType(id, satType).getSignalStrength(goGPS.Freq));

			  double masterSatWeight = computeWeight(master.topo[i].Elevation, masterObs.getSatByIDType(id, satType).getSignalStrength(goGPS.Freq));

			  Qphase.set(p, p, Qphase.get(p, p) + (Math.Pow(stDevPhase, 2) + Math.Pow(roverObs.getSatByIDType(id, satType).getWavelength(goGPS.Freq), 2) * Cee.get(i3 + id, i3 + id)) * (roverPivotWeight + masterPivotWeight) + (Math.Pow(stDevPhase, 2) + Math.Pow(roverObs.getSatByIDType(id, satType).getWavelength(goGPS.Freq), 2) * Cee.get(i3 + id, i3 + id)) * (roverSatWeight + masterSatWeight));

			  int r = 1;
			  for (int m = i + 1; m < nObs; m++)
			  {
				if (sats.pos[m] != null && sats.availPhase.Contains(sats.pos[m].SatID) && m != pivotIndex)
				{
				  Qphase.set(p, p + r, 0);
				  Qphase.set(p + r, p, 0);
				  r++;
				}
			  }
			  //          int r = 1;
			  //          for (int j = i+1; j < nObs; j++) {
			  //            if (sats.pos[j] !=null && sats.availPhase.contains(sats.pos[j].getSatID()) && j != pivotIndex) {
			  //              Qphase.set(p, p+r, Qphase.get(p, p+r)
			  //                  + (Math.pow(lambda, 2) * Cee.get(i3 + sats.pos[i].getSatID(), i3 + sats.pos[j].getSatID()))
			  //                  * (roverPivotWeight + masterPivotWeight));
			  //              Qphase.set(p+r, p, Qphase.get(p, p+r));
			  //              r++;
			  //            }
			  //          }

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

		  // Estimation of the variance of the observation error
		  vEstim = y0.minus(A.mult(x).plus(b));

		  double varianceEstim = (vEstim.transpose().mult(Q.invert()).mult(vEstim)).get(0) / (nObsAvail + nObsAvailPhase - nUnknowns);

		  // Covariance matrix of the estimation error
		  covariance = A.transpose().mult(Q.invert()).mult(A).invert().scale(varianceEstim);

		  // Store estimated ambiguity combinations and their covariance
		  for (int m = 0; m < satAmb.Count; m++)
		  {
			estimatedAmbiguityComb[m] = x.get(3 + m);
			estimatedAmbiguityCombCovariance[m] = covariance.get(3 + m, 3 + m);
		  }
		}

		if (init)
		{
		  for (int i = 0; i < satAmb.Count; i++)
		  {
			// Estimated ambiguity
			KFstate.set(i3 + satAmb[i], 0, estimatedAmbiguityComb[i]);

			// Store the variance of the estimated ambiguity
			Cee.set(i3 + satAmb[i], i3 + satAmb[i], estimatedAmbiguityCombCovariance[i]);
		  }
		}
		else
		{
		  for (int i = 0; i < satAmb.Count; i++)
		  {
			// Estimated ambiguity
			KFprediction.set(i3 + satAmb[i], 0, estimatedAmbiguityComb[i]);

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
		int temporaryPivot = 0;
		bool newPivot = false;
		for (int i = 0; i < sats.pos.Length; i++)
		{

		  if (sats.pos[i] != null && sats.availPhase.Contains(sats.pos[i].SatID) && sats.typeAvailPhase.Contains(sats.pos[i].SatType) && !satOld.Contains(sats.pos[i].SatID) && satTypeOld.Contains(sats.pos[i].SatType))
		  {

			newSatellites.Add(sats.pos[i].SatID);

			if (sats.pos[i].SatID == sats.pos[sats.pivot].SatID && sats.pos[i].SatType == sats.pos[sats.pivot].SatType)
			{
			  newPivot = true;
			  if (goGPS.Debug)
			  {
				  Console.WriteLine("New satellite " + sats.pos[i].SatID + " (new pivot)");
			  }
			}
			else
			{
			  if (goGPS.Debug)
			  {
				  Console.WriteLine("New satellite " + sats.pos[i].SatID);
			  }
			}
		  }
		}

		// If a new satellite is going to be the pivot, its ambiguity needs to be estimated before switching pivot
		if (newPivot)
		{
		  // If it is not the only satellite with phase
		  if (sats.availPhase.Count > 1)
		  {
			// If the former pivot is still among satellites with phase
			if (sats.availPhase.Contains(oldPivotId) && sats.typeAvailPhase.Contains(oldPivotType))
			{
			  // Find the index of the old pivot
			  for (int j = 0; j < sats.pos.Length; j++)
			  {
				if (sats.pos[j] != null && sats.pos[j].SatID == oldPivotId && sats.pos[j].SatType == oldPivotType)
				{
				  temporaryPivot = j;
				}
			  }
			}
			else
			{
			  double maxEl = 0;
			  // Find a temporary pivot with phase
			  for (int j = 0; j < sats.pos.Length; j++)
			  {
				if (sats.pos[j] != null && sats.availPhase.Contains(sats.pos[j].SatID) && sats.typeAvailPhase.Contains(sats.pos[j].SatType) && j != sats.pivot && rover.topo[j].Elevation > maxEl)
				{
				  temporaryPivot = j;
				  maxEl = rover.topo[j].Elevation;
				}
			  }
			  // Reset the ambiguities of other satellites according to the temporary pivot
			  newSatellites.Clear();
			  newSatellites.AddRange(sats.availPhase);
			  oldPivotId = sats.pos[temporaryPivot].SatID;
			  oldPivotType = sats.pos[temporaryPivot].SatType;

			}
			// Estimate the ambiguity of the new pivot and other (new) satellites, using the temporary pivot
			estimateAmbiguities(roverObs, masterObs, masterPos, newSatellites, temporaryPivot, false);
			newSatellites.Clear();
		  }
		}

		// Check if pivot satellite changed since the previous epoch
		if (oldPivotId != sats.pos[sats.pivot].SatID && oldPivotType == sats.pos[sats.pivot].SatType && sats.availPhase.Count > 1)
		{

		  if (goGPS.Debug)
		  {
			  Console.WriteLine("Pivot change from satellite " + oldPivotId + " to satellite " + sats.pos[sats.pivot].SatID);
		  }

		  // Matrix construction to manage the change of pivot satellite
		  SimpleMatrix A = new SimpleMatrix(o3 + nN, o3 + nN);

		  //TODO: need to check below
		  int pivotIndex = i3 + sats.pos[sats.pivot].SatID;
		  int pivotOldIndex = i3 + oldPivotId;
		  for (int i = 0; i < o3; i++)
		  {
			for (int j = 0; j < o3; j++)
			{
			  if (i == j)
			  {
				A.set(i, j, 1);
			  }
			}
		  }
		  for (int i = 0; i < sats.availPhase.Count; i++)
		  {
			for (int j = 0; j < sats.availPhase.Count; j++)
			{
			  int satIndex = i3 + sats.availPhase[i];
			  if (i == j)
			  {
				A.set(satIndex, satIndex, 1);
			  }
			  A.set(satIndex, pivotIndex, -1);
			}
		  }
		  A.set(pivotOldIndex, pivotOldIndex, 0);
		  A.set(pivotIndex, pivotIndex, 0);

		  // Update predicted state
		  KFprediction = A.mult(KFprediction);

		  // Re-computation of the Cee covariance matrix at the previous epoch
		  Cee = A.mult(Cee).mult(A.transpose());
		}

		// Cycle-slip detection
		bool lossOfLockCycleSlipRover;
		bool lossOfLockCycleSlipMaster;
		bool dopplerCycleSlipRover;
		bool dopplerCycleSlipMaster;
		bool approxRangeCycleSlip;
		bool cycleSlip;
		//boolean slippedPivot = false;

		// Pivot satellite ID
		int pivotId = sats.pos[sats.pivot].SatID;

		// Rover-pivot and master-pivot observed phase
		char satType = roverObs.getGnssType(0);
		double roverPivotPhaseObs = roverObs.getSatByIDType(pivotId, satType).getPhaserange(goGPS.Freq);
		double masterPivotPhaseObs = masterObs.getSatByIDType(pivotId, satType).getPhaserange(goGPS.Freq);

		// Rover-pivot and master-pivot approximate pseudoranges
		double roverPivotAppRange = rover.satAppRange[sats.pivot];
		double masterPivotAppRange = master.satAppRange[sats.pivot];

		for (int i = 0; i < roverObs.NumSat; i++)
		{

		  int satID = roverObs.getSatID(i);
		  satType = roverObs.getGnssType(i);
		  string checkAvailGnss = Convert.ToString(satType) + Convert.ToString(satID);

		  if (sats.gnssAvailPhase.Contains(checkAvailGnss))
		  {

			// cycle slip detected by loss of lock indicator (disabled)
			lossOfLockCycleSlipRover = roverObs.getSatByIDType(satID, satType).isPossibleCycleSlip(goGPS.Freq);
			lossOfLockCycleSlipMaster = masterObs.getSatByIDType(satID, satType).isPossibleCycleSlip(goGPS.Freq);
			lossOfLockCycleSlipRover = false;
			lossOfLockCycleSlipMaster = false;

			// cycle slip detected by Doppler predicted phase range
			if (goGPS.CycleSlipDetectionStrategy == CycleSlipDetectionStrategy.DOPPLER_PREDICTED_PHASE_RANGE)
			{
			  dopplerCycleSlipRover = rover.getDopplerPredictedPhase(satID) != 0.0 && (Math.Abs(roverObs.getSatByIDType(satID, satType).getPhaseCycles(goGPS.Freq) - rover.getDopplerPredictedPhase(satID)) > goGPS.CycleSlipThreshold);
			  dopplerCycleSlipMaster = master.getDopplerPredictedPhase(satID) != 0.0 && (Math.Abs(masterObs.getSatByIDType(satID, satType).getPhaseCycles(goGPS.Freq) - master.getDopplerPredictedPhase(satID)) > goGPS.CycleSlipThreshold);
			}
			else
			{
			  dopplerCycleSlipRover = false;
			  dopplerCycleSlipMaster = false;
			}

			// cycle slip detected by approximate pseudorange
			approxRangeCycleSlip = false;
			if (goGPS.CycleSlipDetectionStrategy == CycleSlipDetectionStrategy.APPROX_PSEUDORANGE && satID != pivotId)
			{

			  // Rover-satellite and master-satellite approximate pseudorange
			  double roverSatCodeAppRange = rover.satAppRange[i];
			  double masterSatCodeAppRange = master.satAppRange[i];

			  // Rover-satellite and master-satellite observed phase
			  double roverSatPhaseObs = roverObs.getSatByIDType(satID, satType).getPhaserange(goGPS.Freq);
			  double masterSatPhaseObs = masterObs.getSatByIDType(satID, satType).getPhaserange(goGPS.Freq);

			  // Estimated code pseudorange double differences
			  double codeDoubleDiffApprox = (roverSatCodeAppRange - masterSatCodeAppRange) - (roverPivotAppRange - masterPivotAppRange);

			  // Observed phase double differences
			  double phaseDoubleDiffObserv = (roverSatPhaseObs - masterSatPhaseObs) - (roverPivotPhaseObs - masterPivotPhaseObs);

			  // Store estimated ambiguity combinations and their covariance
			  double estimatedAmbiguityComb = (codeDoubleDiffApprox - phaseDoubleDiffObserv) / roverObs.getSatByIDType(satID, satType).getWavelength(goGPS.Freq);

			  approxRangeCycleSlip = (Math.Abs(KFprediction.get(i3 + satID) - estimatedAmbiguityComb)) > goGPS.CycleSlipThreshold;

			}
			else
			{
			  approxRangeCycleSlip = false;
			}

			cycleSlip = (lossOfLockCycleSlipRover || lossOfLockCycleSlipMaster || dopplerCycleSlipRover || dopplerCycleSlipMaster || approxRangeCycleSlip);

			if (satID != sats.pos[sats.pivot].SatID && !newSatellites.Contains(satID) && cycleSlip)
			{

			  slippedSatellites.Add(satID);

			  //        if (satID != sats.pos[sats.pivot].getSatID()) {
			  if (dopplerCycleSlipRover)
			  {
				if (goGPS.Debug)
				{
					Console.WriteLine("[ROVER] Cycle slip on satellite " + satID + " (range diff = " + Math.Abs(roverObs.getSatByIDType(satID, satType).getPhaseCycles(goGPS.Freq) - rover.getDopplerPredictedPhase(satID)) + ")");
				}
			  }
			  if (dopplerCycleSlipMaster)
			  {
				if (goGPS.Debug)
				{
					Console.WriteLine("[MASTER] Cycle slip on satellite " + satID + " (range diff = " + Math.Abs(masterObs.getSatByIDType(satID, satType).getPhaseCycles(goGPS.Freq) - master.getDopplerPredictedPhase(satID)) + ")");
				}
			  }
			  //        } else {
			  //          boolean slippedPivot = true;
			  //          if (dopplerCycleSlipRover)
			  //            System.out.println("[ROVER] Cycle slip on pivot satellite "+satID+" (range diff = "+Math.abs(roverObs.getGpsByID(satID).getPhase(goGPS.getFreq())
			  //                - this.rover.getDopplerPredictedPhase(satID))+")");
			  //          if (dopplerCycleSlipMaster)
			  //            System.out.println("[MASTER] Cycle slip on pivot satellite "+satID+" (range diff = "+Math.abs(masterObs.getGpsByID(satID).getPhase(goGPS.getFreq())
			  //                - this.master.getDopplerPredictedPhase(satID))+")");
			  //        }
			}
		  }
		}

	//    // If the pivot satellites slipped, the ambiguities of all the other satellites must be re-estimated
	//    if (slippedPivot) {
	//      // If it is not the only satellite with phase
	//      if (sats.availPhase.size() > 1) {
	//        // Reset the ambiguities of other satellites
	//        newSatellites.clear();
	//        slippedSatellites.clear();
	//        slippedSatellites.addAll(sats.availPhase);
	//      }
	//    }

		// Ambiguity estimation
		if (newSatellites.Count != 0 || slippedSatellites.Count != 0)
		{
		  // List of satellites that need ambiguity estimation
		  List<int?> satAmb = newSatellites;
		  satAmb.AddRange(slippedSatellites);
		  estimateAmbiguities(roverObs, masterObs, masterPos, satAmb, sats.pivot, false);
		}
	  }

	  /// <summary>
	  /// Run kalman filter on code and phase double differences.
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

		KalmanFilter kf = new KF_DD_code_phase(goGPS);

		// Flag to check if Kalman filter has been initialized
		bool kalmanInitialized = false;

		try
		{
		  bool validPosition = false;

		  timeRead = DateTimeHelperClass.CurrentUnixTimeMillis() - timeRead;
		  depRead = depRead + timeRead;

		  Observations obsR = roverIn.NextObservations;
		  Observations obsM = masterIn.NextObservations;

		  while (obsR != null && obsM != null)
		  {
	//        System.out.println("obsR: " + obsR);


			if (debug)
			{
				Console.WriteLine("R:" + obsR.RefTime.Msec + " M:" + obsM.RefTime.Msec);
			}

			timeRead = DateTimeHelperClass.CurrentUnixTimeMillis();

			// Discard master epochs if correspondent rover epochs are
			// not available
	//        Observations obsR = roverIn.nextObservations();
	//        Observations obsM = masterIn.nextObservations();
			double obsRtime = obsR.RefTime.RoundedGpsTime;
			Console.WriteLine("look for M " + obsRtime);
	//        System.out.println("obsM_Time: " + obsM.getRefTime().getRoundedGpsTime());

			while (obsM != null && obsR != null && obsRtime > obsM.RefTime.RoundedGpsTime)
			{

	//          masterIn.skipDataObs();
	//          masterIn.parseEpochObs();
			  obsM = masterIn.NextObservations;
			  Console.WriteLine("while obsM: " + obsM);
			}
	//        System.out.println("found M "+obsRtime);
			if (obsM == null)
			{
				Console.WriteLine("Couldn't find an obsM in a valid time span: " + obsR.RefTime);
					break;
			}
			// Discard rover epochs if correspondent master epochs are not available
			double obsMtime = obsM.RefTime.RoundedGpsTime;
			Console.WriteLine("##look for R " + obsMtime);

			while (obsM != null && obsR != null && obsR.RefTime.RoundedGpsTime < obsMtime)
			{
			  Console.WriteLine("obsR_Time: " + obsR.RefTime.GpsTime);

			  obsR = roverIn.NextObservations;
			}
	//        System.out.println("found R "+obsMtime);

			Console.WriteLine("obsM: " + obsM);
			Console.WriteLine("obsR: " + obsR);


			if (obsM != null && obsR != null)
			{
			  timeRead = DateTimeHelperClass.CurrentUnixTimeMillis() - timeRead;
			  depRead = depRead + timeRead;
			  timeProc = DateTimeHelperClass.CurrentUnixTimeMillis();
	//          System.out.println("Check!!");


			  // If Kalman filter was not initialized and if there are at least four satellites
			  bool valid = true;
			  if (!kalmanInitialized && obsR.NumSat >= 4)
			  {

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
				  kf.init(obsR, obsM, masterIn.DefinedPosition);

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
				  kf.loop(obsR,obsM, masterIn.DefinedPosition);
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
			  obsM = masterIn.NextObservations;

			}
			else
			{
			  if (debug)
			  {
				  Console.WriteLine("Missing M or R obs ");
			  }
			}
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