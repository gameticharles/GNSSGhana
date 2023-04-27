using System;

namespace org.gogpsproject.positioning
{

	using SimpleMatrix = org.ejml.simple.SimpleMatrix;
	using ObservationSet = org.gogpsproject.producer.ObservationSet;
	using Observations = org.gogpsproject.producer.Observations;

	public class LS_SA_code_dopp_snapshot : LS_SA_code_snapshot
	{

	  /// <summary>
	  /// max time update for a valid fix </summary>
	  private long maxTimeUpdateSec = 30; // s

	  /// <summary>
	  /// Set aPrioriPos in thread mode </summary>
	  public new Coordinates aPrioriPos;

	  public LS_SA_code_dopp_snapshot(GoGPS goGPS) : base(goGPS)
	  {
	  }

	  public override long TimeLimit
	  {
		  get
		  {
			return maxTimeUpdateSec;
		  }
	  }

	  public override GoGPS setTimeLimit(long maxTimeUpdateSec)
	  {
		this.maxTimeUpdateSec = maxTimeUpdateSec;
		return goGPS;
	  }

	  internal override SnapshotPivotResult snapshotProcessPivot(Observations roverObs, int pivotIndex, int max_iterations, double cutOffEl, double? residCutOff)
	  {

		int savedIndex = pivotIndex;
		int nObs = roverObs.NumSat;

		Coordinates refPos = (Coordinates) rover.clone();

		long refTime = roverObs.RefTime.Msec;
		long unixTime = refTime;

		SimpleMatrix A = null; // design matrix
		SimpleMatrix b = null; // Vector for approximate (estimated, predicted) pseudoranges
		SimpleMatrix y0 = null; // Observed pseudoranges
		SimpleMatrix Q = null; // Cofactor (Weighted) Matrix
		SimpleMatrix x = null; // Solution (Update) Vector
		SimpleMatrix tropoCorr = null;
		SimpleMatrix ionoCorr = null;

		int pivotSatId = roverObs.getSatID(pivotIndex);

		const double POS_TOL = 1.0; // meters
		const double TG_TOL = 1; // milliseconds

		int nObsAvail = 0;
		double pivotElevation = 0;
		rover.eRes = 300;

		// common bias in ms
		double cbiasms = 1;


		for (int itr = 0; itr < max_iterations; itr++)
		{

		  if (goGPS.Debug)
		  {
			  Console.WriteLine(">> Itr " + itr);
		  }

	//      if( goGPS.isDebug() && goGPS.truePos != null ){
	//        System.out.println( String.format( "\r\n* True Pos: %8.4f, %8.4f, %8.4f", 
	//            goGPS.truePos.getGeodeticLatitude(),
	//            goGPS.truePos.getGeodeticLongitude(),
	//            goGPS.truePos.getGeodeticHeight()
	//            ));
	//        goGPS.truePos.selectSatellitesStandaloneFractional( roverObs, -100, GoGPS.MODULO1MS );
	//      }
	//      if( goGPS.isDebug()) System.out.println();

		  if (2 * roverObs.NumSat + 1 > nUnknowns)
		  {
			selectSatellites(roverObs, cutOffEl, GoGPS.MODULO1MS);
		  }
		  else
		  {
			selectSatellites(roverObs, -10, GoGPS.MODULO1MS);
		  }

		  int ns = sats.AvailNumber;
		  nObsAvail = 2 * sats.AvailNumber + 1; // use doppler and DTM / height soft constraint

		  // Number of unknown parameters (here adds the clock error rate)
		  int nUnknowns = 6;

		  bool estimateClockErrRate = true;
		  if (sats.AvailNumber == 2)
		  {
			estimateClockErrRate = false;
			nUnknowns = 5;
		  }

		  if (nObsAvail < nUnknowns)
		  {
			if (goGPS.Debug)
			{
				Console.WriteLine("\r\nNot enough satellites for " + roverObs.RefTime);
			}
			rover.setXYZ(0, 0, 0);
			if (nObsAvail > 0)
			{
			  rover.satsInUse = nObsAvail;
			  rover.status = Status.NotEnoughSats;
			}
			return null;
		  }

		  if (sats.pos[savedIndex] == null || rover.topo[savedIndex] == null || !sats.avail.Keys.contains(pivotSatId))
		  {
			if (goGPS.Debug)
			{
				Console.WriteLine("\r\nCan't use pivot with satId " + pivotSatId);
			}
			return null;
		  }

		  pivotElevation = rover.topo[savedIndex].Elevation;

		  // Least squares design matrix
		  A = new SimpleMatrix(nObsAvail, nUnknowns);

		  // Vector for approximate pseudoranges
		  b = new SimpleMatrix(nObsAvail, 1);

		  // Vector for observed pseudoranges
		  y0 = new SimpleMatrix(nObsAvail, 1);

		  // Cofactor matrix
		  Q = new SimpleMatrix(nObsAvail, nObsAvail);

		  // Solution vector
		  x = new SimpleMatrix(nUnknowns, 1);

		  // Vectors for troposphere and ionosphere corrections
		  tropoCorr = new SimpleMatrix(nObsAvail, 1);
		  ionoCorr = new SimpleMatrix(nObsAvail, 1);

		  int k = 0;

		  // Initialize the cofactor matrix
	  //    Q.set(1);

		  // Set up the least squares matrices
		  for (int i = 0; i < nObs; i++)
		  {
			int satId = roverObs.getSatID(i);

			if (sats.pos[i] == null || !sats.avail.Keys.contains(satId))
			{
			  continue; // i loop
			}

			float doppler = roverObs.getSatByID(satId).getDoppler(ObservationSet.L1);

			if (float.IsNaN(doppler))
			{
			  // TODO revert to code-only case
			  return null;
			}

			ObservationSet os = roverObs.getSatByID(satId);

			if (satId == pivotSatId && pivotIndex != k)
			{
			  pivotIndex = k;
			}

			// Line Of Sight vector units (ECEF)
			SimpleMatrix e = new SimpleMatrix(1,3);

			// Line Of Sight vector units (ECEF)
			e.set(0,0, rover.diffSat[i].get(0) / rover.satAppRange[i]);
			e.set(0,1, rover.diffSat[i].get(1) / rover.satAppRange[i]);
			e.set(0,2, rover.diffSat[i].get(2) / rover.satAppRange[i]);

			/// <summary>
			/// range rate = scalar product of speed vector X unit vector </summary>
			double rodot = doppler * Constants.SPEED_OF_LIGHT / Constants.FL1;

			/// <summary>
			/// computed satspeed: scalar product of speed vector X LOS unit vector </summary>
			double rodotSatSpeed = e.mult(sats.pos[i].Speed).get(0);

			/// <summary>
			/// Design Matrix </summary>
			A.set(k, 0, e.get(0)); // X
			A.set(k, 1, e.get(1)); // Y
			A.set(k, 2, e.get(2)); // Z
			A.set(k, 3, 1); // clock error
			A.set(k, 4, -rodot + rover.ClockErrorRate); // common bias
			A.set(ns + k, 0, sats.pos[i].Speed.get(0) / rover.satAppRange[i]); // VX
			A.set(ns + k, 1, sats.pos[i].Speed.get(1) / rover.satAppRange[i]); // VY
			A.set(ns + k, 2, sats.pos[i].Speed.get(2) / rover.satAppRange[i]); // VZ

			if (estimateClockErrRate)
			{
			  A.set(ns + k, 5, 1); // clock error rate
			}

			/// <summary>
			/// residuals </summary>
			// Add the approximate pseudorange value to b
			b.set(k, 0, (rover.satAppRange[i] - sats.pos[i].SatelliteClockError * Constants.SPEED_OF_LIGHT) % GoGPS.MODULO1MS);

			// Add the clock-corrected observed pseudorange value to y0
	  //      y0.set(k, 0, roverObs.getSatByIDType(id, satType).getPseudorange(goGPS.getFreq()));
			y0.set(k, 0, os.getCodeC(0));
			y0.set(ns + k, 0, rodot - rodotSatSpeed - rover.ClockErrorRate);

			// cap tropo correction
			if (double.IsNaN(rover.satTropoCorr[i]))
			{
			  rover.satTropoCorr[i] = 0;
			}

			if (rover.satTropoCorr[i] > 30)
			{
			  rover.satTropoCorr[i] = 30;
			}
			if (rover.satTropoCorr[i] < -30)
			{
			  rover.satTropoCorr[i] = -30;
			}

			tropoCorr.set(k, 0, rover.satTropoCorr[i]);
			ionoCorr.set(k, 0, rover.satIonoCorr[i]);

			// Fill in the cofactor matrix
			double weight = Q.get(k, k) + computeWeight(rover.topo[i].Elevation, roverObs.getSatByIDType(satId, 'G').getSignalStrength(goGPS.Freq));
			Q.set(k, k, weight);

			// Increment available satellites counter
			k++;
		  } // i loop

		  // Apply troposphere and ionosphere correction
		  b = b.plus(tropoCorr);
		  b = b.plus(ionoCorr);

		  SimpleMatrix resid = y0.minus(b);

		  rover.satsInUse = 0;

		  {
		  // adjust residuals based on satellite with pivotIndex
			ObservationSet os = roverObs.getSatByID(pivotSatId);
	//        if( !Float.isNaN( os.getSignalStrength(0)) && os.getSignalStrength(0)<18 ){
	//          return null;
	//        }
			double pivot = resid.get(pivotIndex);

			if (goGPS.Debug)
			{
				Console.WriteLine(string.Format("\r\n\r\nResiduals -> Adjusted Residuals (ms) - Pivot = {0,7:F4} (ms)", pivot / Constants.SPEED_OF_LIGHT * 1000));
			}
			int i = 0;
			for (k = 0; k < roverObs.NumSat; k++)
			{
			  int? satId = roverObs.getSatID(k);
			  os = roverObs.getSatByID(satId);

			  if (!sats.avail.Keys.contains(satId) || sats.pos[k] == null || rover.topo[k] == null)
			  {
				continue;
			  }

			  os.el = rover.topo[k].Elevation;

			  double d = resid.get(i);
			  if (goGPS.Debug)
			  {
				  Console.Write(string.Format("{0,2:D}) {1,7:F4} -> ", satId, d / Constants.SPEED_OF_LIGHT * 1000));
			  }
			  if (d - pivot > GoGPS.MODULO1MS / 2)
			  {
				d -= GoGPS.MODULO1MS;
			  }
			  if (d - pivot < -GoGPS.MODULO1MS / 2)
			  {
				d += GoGPS.MODULO1MS;
			  }
			  if (goGPS.Debug)
			  {
				  Console.Write(string.Format("{0,7:F4}", d / Constants.SPEED_OF_LIGHT * 1000));
			  }

			  resid.set(i,d);

			  // check again, if fails, exclude this satellite
			  // TODO there could be a problem if the pivot is bogus!
			  double dms = Math.Abs(d - pivot) / Constants.SPEED_OF_LIGHT * 1000;
			  if (residCutOff != null && (rover.eRes < residCutOff * Constants.SPEED_OF_LIGHT / 1000) && dms>residCutOff)
			  {
				if (goGPS.Debug)
				{
					Console.WriteLine(string.Format("; D:{0,6:F4}; Excluding sat:{1,2:D}; C:{2,6:F4}; El:{3,6:F4}; rodot:{4,7:F2}; ", dms, roverObs.getSatID(k), roverObs.getSatByID(satId).getCodeC(0) / Constants.SPEED_OF_LIGHT * 1000, os.el, A.get(i, 4)));
				}

				A.set(i, 0, 0);
				A.set(i, 1, 0);
				A.set(i, 2, 0);
				A.set(i, 3, 0);
				A.set(i, 4, 0);
				os.inUse(false);
			  }
			  else
			  {
				rover.satsInUse++;
				os.inUse(true);

				if (goGPS.Debug)
				{
					Console.WriteLine(string.Format("; D:{0,6:F4}; snr:{1,5:F1}; El:{2,5:F1}; rodot:{3,10:F2}; ", dms, os.getSignalStrength(0), os.el, A.get(i, 4)));
				}
			  }
			  i++;
			}
		  }

		  if (2 * rover.satsInUse + 1 < nUnknowns)
		  {
			if (goGPS.Debug)
			{
				Console.WriteLine("Not enough satellites for " + roverObs.RefTime);
			}
			rover.setXYZ(0, 0, 0);
			if (rover.status == Status.None)
			{
			  rover.status = Status.NotEnoughSats;
			}
			return null;
		  }

		  // Add height soft constraint
		  double hR_app = rover.GeodeticHeight;

		//  %extraction from the dtm of the height correspondent to the approximated position
		//  [h_dtm] = grid_bilin_interp(E_app, N_app, tile_buffer, tile_header.ncols*3, tile_header.nrows*3, tile_header.cellsize, Ell, Nll, tile_header.nodata);
		  double h_dtm = hR_app > 0? hR_app : 30; // initialize to something above sea level
		  if (h_dtm > 2000)
		  {
			h_dtm = 2000;
		  }

	//        if( goGPS.useDTM() ){
	//          try {
	//            ElevationResult elres = ElevationApi.getByPoint( getContext(), new LatLng(getGeodeticLatitude(), getGeodeticLongitude())).await();
	//            if( elres.elevation > 0 )
	//              h_dtm = elres.elevation;
	//          } catch (Exception e) {
	//            // TODO Auto-generated catch block
	//    //        e.printStackTrace();
	//          }
	//        }

		  double lam = Math.toRadians(rover.GeodeticLongitude);
		  double phi = Math.toRadians(rover.GeodeticLatitude);

		  double cosLam = Math.Cos(lam);
		  double cosPhi = Math.Cos(phi);
		  double sinLam = Math.Sin(lam);
		  double sinPhi = Math.Sin(phi);

		  k = nObsAvail - 1;
		  A.set(k, 0, cosPhi * cosLam);
		  A.set(k, 1, cosPhi * sinLam);
		  A.set(k, 2, sinPhi);
		  A.set(k, 3, 0);
		  A.set(k, 4, 0);

		//  %Y0 vector computation for DTM constraint
		//  y0_dtm = h_dtm  - hR_app + cos(phiR_app)*cos(lamR_app)*X_app + cos(phiR_app)*sin(lamR_app)*Y_app + sin(phiR_app)*Z_app;
		  double y0_dtm = h_dtm - hR_app;
		  y0.set(k, 0, y0_dtm);

		  double maxWeight = Q.elementMaxAbs();
		  Q.set(k, k, maxWeight);

	//      SimpleMatrix B = A.transpose().mult(Q.invert()).mult(A).invert().mult(A.transpose()).mult(Q.invert());
		  SimpleMatrix B = A.transpose().mult(A).invert().mult(A.transpose());
		  x = B.mult(resid);

		  double correction_mag = Math.Sqrt(Math.Pow(x.get(0), 2) + Math.Pow(x.get(1), 2) + Math.Pow(x.get(2), 2));

		  // Receiver clock error: m -> seconds
		  rover.clockError = x.get(3) / Constants.SPEED_OF_LIGHT;

		  // time update seconds -> ms
		  cbiasms = x.get(4) * 1000;


		  if (goGPS.Debug)
		  {
			  Console.WriteLine(string.Format("\r\npos update:  {0,5:F0} (m)", correction_mag));
		  }
		  if (goGPS.Debug)
		  {
			  Console.WriteLine(string.Format("clock error: {0,2:F4} (us)", rover.clockError * 1000000));
		  }
		  if (goGPS.Debug)
		  {
			  Console.WriteLine(string.Format("common bias: {0,2:F4} (ms)", cbiasms));
		  }

		  if (estimateClockErrRate)
		  {
			// Update receiver clock error rate
			rover.clockErrorRate += x.get(5);
			if (goGPS.Debug)
			{
				Console.WriteLine(string.Format("clock error rate: {0,2:F4}", rover.clockErrorRate));
			}
		  }

		  // apply correction to Rx position estimate
		  rover.PlusXYZ = x.extractMatrix(0, 3, 0, 1);
		  rover.computeGeodetic();

		  // update refTime
		  unixTime += (long)cbiasms;
		  Time newTime = new Time(unixTime);
		  roverObs.RefTime = newTime;

		  if (goGPS.Debug)
		  {
			  Console.WriteLine(string.Format("recpos ({0:D}): {1,5:F3}, {2,5:F3}, {3,5:F3}, {4}", itr, rover.GeodeticLatitude, rover.GeodeticLongitude, rover.GeodeticHeight, (new Time(unixTime)).ToString()));
		  }

		   // average eRes 
		   SimpleMatrix eResM = A.mult(x).minus(resid);
		   rover.eRes = 0;
		   for (k = 0; k < sats.avail.Count; k++)
		   {
			 int satId = roverObs.getSatID(k);
			 ObservationSet os = roverObs.getSatByID(satId);
			 os.eRes = Math.Abs(eResM.get(k));
			 if (os.InUse)
			 {
			   rover.eRes += Math.Pow(os.eRes, 2);
			 }
		   }
		   rover.eRes = Math.Sqrt(rover.eRes / rover.satsInUse);
		   if (goGPS.Debug)
		   {
			   Console.WriteLine(string.Format("eRes = {0,5:F3}\r\n", rover.eRes));
		   }

		   // if correction is small enough, we're done, exit loop
		   // TODO check also eRes
		   if ((correction_mag < POS_TOL || rover.eRes < POS_TOL) && Math.Abs(cbiasms) < TG_TOL)
		   {
			 break;
		   }
		} // itr

		double correction_mag = refPos.minusXYZ(rover).normF();
		double tg = (roverObs.RefTime.Msec - refTime) / 1000;

		if (double.IsNaN(correction_mag) || correction_mag > goGPS.PosLimit || Math.Abs(tg) > this.TimeLimit)
		{

		  if (goGPS.Debug)
		  {
			  Console.WriteLine("Correction exceeds the limits: dist = " + (long)correction_mag + "m; t offset = " + (long)tg + "s");
		  }

		  rover.setXYZ(0, 0, 0);
		  rover.status = Status.MaxCorrection;

		  return null;
		}
		else
		{
		  if (goGPS.Debug)
		  {
			Console.WriteLine(string.Format("recpos: {0,5:F3}, {1,5:F3}, {2,5:F3}, {3}, eRes = {4,5:F3}", rover.GeodeticLatitude, rover.GeodeticLongitude, rover.GeodeticHeight, (new Time(unixTime)).ToString(), rover.eRes, cbiasms));
		  }
		}

		updateDops(A);

		return new SnapshotPivotResult(this, savedIndex, pivotSatId, pivotElevation, rover, unixTime, rover.eRes, rover.hDop, sats.AvailNumber, cbiasms);
	  }

	  /// <param name="roverObs">
	  /// return computed time offset in milliseconds </param>
	  public override long? snapshotPos(Observations roverObs)
	  {
		double? residCutOff = 0.5;
		double elCutOff = 5.0;
		rover.status = Status.None;

		// Number of GPS observations
		int nObs = roverObs.NumSat;
		if (2 * nObs + 1 < nUnknowns)
		{
		  if (goGPS.Debug)
		  {
			  Console.WriteLine("Not enough satellites for " + roverObs.RefTime);
		  }
		  rover.setXYZ(0, 0, 0);
		  rover.satsInUse = nObs;
		  rover.status = Status.NotEnoughSats;
		  return null;
		}

		SnapshotPivotResult result = null;
		RoverPosition refPos = new RoverPosition();

		// save this position before trying
		rover.cloneInto(refPos);
		Time refTime = roverObs.RefTime;

		for (int satIdx = 0; satIdx < roverObs.NumSat; satIdx++)
		{
		  if (goGPS.Debug)
		  {
			  Console.WriteLine("\r\n===> Try Pivot " + satIdx);
		  }

		  // restore this position before trying
		  refPos.cloneInto(rover);
		  roverObs.RefTime = refTime;

		  rover.status = Status.None;
		  // select a pivot with at least elCutOff elevation
		  int maxIterations = 3;
	//      residCutOff = 0.05;
		  SnapshotPivotResult pivotRes = snapshotProcessPivot(roverObs, satIdx, maxIterations, elCutOff, residCutOff);

		  if (pivotRes == null && rover.status == Status.EphNotFound)
		  {
			// don't keep requesting Ephemeris if they're not ready yet
			rover.setXYZ(0, 0, 0);
			return null;
		  }

		  if (pivotRes != null && (result == null || pivotRes.eRes < result.eRes))
		  {
			result = pivotRes;
		  }
		}
		if (result == null)
		{
		  rover.setXYZ(0, 0, 0);
		  return null;
		}
		if (goGPS.Debug)
		{
		  Console.WriteLine(string.Format("\r\n>> Selected Pivot SatId = {0:D}; SatIdx = {1:D}; eRes = {2,5:F2};  cbias = {3,5:F2}; elevation = {4,5:F2}\r\n", result.satId, result.satIndex, result.eRes, result.cbiasms, result.elevation));
		}

		// restore this position after trying
		refPos.cloneInto(rover);
		roverObs.RefTime = refTime;

		// reprocess with selected pivot and more iterations
		rover.status = Status.None;
		if (result.nObs < 5)
		{
	//      residCutOff = 0.0002;
		  elCutOff = -5;
		}

		result = snapshotProcessPivot(roverObs, result.satIndex, 100, elCutOff, residCutOff);
		if (result == null)
		{
		  rover.setXYZ(0, 0, 0);
		  return null;
		}

		if (result.eRes > 350)
		{
	//      if(goGPS.isDebug()) 
			Console.WriteLine("eRes too large = " + rover.eRes);

		  rover.setXYZ(0, 0, 0);
		  rover.status = Status.MaxEres;

		  return null;
		}

		if (result.hDop > this.goGPS.HdopLimit)
		{
		  if (goGPS.Debug)
		  {
			  Console.WriteLine(string.Format("recpos: {0,5:F4}, {1,5:F4}, {2,5:F4}, {3}", rover.GeodeticLatitude, rover.GeodeticLongitude, rover.GeodeticHeight, (new Time(result.unixTime)).ToString()));
		  }
	//      if(goGPS.isDebug()) 
			Console.WriteLine(string.Format("hDOP too large: {0,3:F1}", result.hDop));
		  rover.setXYZ(0, 0, 0);
		  rover.status = Status.MaxHDOP;
		  return null;
		}

		rover.status = Status.Valid;
		result.roverPos.cloneInto(rover);
		rover.RefTime = new Time(result.unixTime);

		if (goGPS.Debug)
		{
			Console.WriteLine(string.Format("recpos: {0,5:F4}, {1,5:F4}, {2,5:F4}, {3}", rover.GeodeticLatitude, rover.GeodeticLongitude, rover.GeodeticHeight, (new Time(result.unixTime)).ToString()));
		}

		long offsetms = result.unixTime - refTime.Msec;

		return offsetms;
	  }

	  internal override long? runOffset(Observations obsR, long offsetms)
	  {
		if (rover == null || obsR == null)
		{
		  return null;
		}

		if (goGPS.Debug)
		{
			Console.WriteLine("\r\n>>Try offset = " + offsetms / 1000 + " (s)");
		}

		rover.setXYZ(aPrioriPos.X, aPrioriPos.Y, aPrioriPos.Z);
		rover.computeGeodetic();
		if (goGPS.Debug)
		{
			Console.WriteLine("A priori pos: " + aPrioriPos);
		}

		Time refTime = new Time(obsR.RefTime.Msec + offsetms);
		obsR.RefTime = refTime;

		if (!rover.ValidXYZ && aPrioriPos.ValidXYZ)
		{
		  aPrioriPos.cloneInto(rover);
		}

		long? updatedms = snapshotPos(obsR);

		if (updatedms == null && rover.status == Status.MaxCorrection)
		{
		  if (goGPS.Debug)
		  {
			  Console.WriteLine("Reset aPrioriPos");
		  }
		  aPrioriPos.cloneInto(rover);
		}

		return updatedms != null? offsetms + updatedms : null;
	  }


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void tryOffset(Coordinates aPrioriPos, org.gogpsproject.producer.Observations obsR) throws Exception
	  public override void tryOffset(Coordinates aPrioriPos, Observations obsR)
	  {
		this.aPrioriPos = aPrioriPos;

	  //  Long offsetsec = Math.round(offsetms/1000.0);
		long? offsetms = 0l;

		long? updatems = runOffset(obsR, offsetms);

		if (updatems == null && (rover.status == Status.EphNotFound || rover.status == Status.MaxHDOP || rover.status == Status.MaxEres || rover.status == Status.NotEnoughSats))
		{
		  return;
		}

	  if (updatems == null)
	  {
		updatems = runOffset(obsR, offsetms - 2 * maxTimeUpdateSec * 1000);
	  }

	  if (updatems == null)
	  {
		updatems = runOffset(obsR, offsetms + 2 * maxTimeUpdateSec * 1000);
	  }

	  if (updatems == null)
	  {
		updatems = runOffset(obsR, offsetms - 4 * maxTimeUpdateSec * 1000);
	  }

	  if (updatems == null)
	  {
		updatems = runOffset(obsR, offsetms + 4 * maxTimeUpdateSec * 1000);
	  }

	  if (updatems == null)
	  {
		updatems = runOffset(obsR, offsetms - 6 * maxTimeUpdateSec * 1000);
	  }

	  if (updatems == null)
	  {
		updatems = runOffset(obsR, offsetms + 6 * maxTimeUpdateSec * 1000);
	  }

	  if (updatems == null)
	  {
		updatems = runOffset(obsR, offsetms - 8 * maxTimeUpdateSec * 1000);
	  }

	  if (updatems == null)
	  {
		updatems = runOffset(obsR, offsetms + 8 * maxTimeUpdateSec * 1000);
	  }

	  if (updatems == null)
	  {
		updatems = runOffset(obsR, offsetms - 10 * maxTimeUpdateSec * 1000);
	  }

	  if (updatems == null)
	  {
		updatems = runOffset(obsR, offsetms + 10 * maxTimeUpdateSec * 1000);
	  }

	  if (updatems == null)
	  {
		updatems = runOffset(obsR, offsetms - 12 * maxTimeUpdateSec * 1000);
	  }

	  if (updatems == null)
	  {
		updatems = runOffset(obsR, offsetms - 14 * maxTimeUpdateSec * 1000);
	  }

	  if (updatems == null)
	  {
		updatems = runOffset(obsR, offsetms - 16 * maxTimeUpdateSec * 1000);
	  }

	  if (updatems == null)
	  {
		updatems = runOffset(obsR, offsetms - 18 * maxTimeUpdateSec * 1000);
	  }

	  if (updatems == null)
	  {
		updatems = runOffset(obsR, offsetms - 20 * maxTimeUpdateSec * 1000);
	  }

	  if (updatems == null)
	  {
		rover.setXYZ(0, 0, 0);
	  }
	  }



	}

}