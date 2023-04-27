using System;
using System.Collections.Generic;

namespace org.gogpsproject.positioning
{


	using SimpleMatrix = org.ejml.simple.SimpleMatrix;
	using PositionConsumer = org.gogpsproject.consumer.PositionConsumer;
	using ObservationSet = org.gogpsproject.producer.ObservationSet;
	using Observations = org.gogpsproject.producer.Observations;

	public class LS_SA_code_coarse_time : LS_SA_code_snapshot
	{

	  /// <summary>
	  /// Float code ambiguities for modular case, should be between 0 and 1 </summary>
	  internal double[] codeAmbiguities;

	  internal readonly int MINSV = 4;

	  public LS_SA_code_coarse_time(GoGPS goGPS) : base(goGPS)
	  {
	  }

	  /// <param name="roverObs"> </param>
	  /// <param name="MODULO"> module in meters (
	  /// @return </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double codeStandaloneCoarseTime(org.gogpsproject.producer.Observations roverObs, final double MODULO)
	  public virtual double codeStandaloneCoarseTime(Observations roverObs, double MODULO)
	  {
		long unixTime = roverObs.RefTime.Msec;

		rover.status = Status.None;

		// Number of GNSS observations without cutoff
		int nObs = roverObs.NumSat;

		// Number of unknown parameters
		int nUnknowns = 5;

		// Define least squares matrices
		SimpleMatrix A = null; // aka H or G, // Least squares design matrix
		SimpleMatrix b; // Vector for approximate (estimated, predicted) pseudoranges
		SimpleMatrix y0; // Observed pseudoranges
		SimpleMatrix Q = null; // Cofactor (Weighted) Matrix
		SimpleMatrix x; // Solution (Update) Vector
		SimpleMatrix vEstim; // Observation Errors
		SimpleMatrix tropoCorr;
		SimpleMatrix ionoCorr;

		// Number of available satellites (i.e. observations)
		int nObsAvail = sats.avail.Count;

		nObsAvail++; // add DTM / height soft constraint

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

		// Counter for available satellites
		int k = 0;

		// Satellite ID
		int id = 0;

		// Set up the least squares matrices
		for (int i = 0; i < nObs; i++)
		{
		  id = roverObs.getSatID(i);

		  if (sats.pos[i] == null || !sats.avail.Keys.contains(id)) //|| recpos.ecef==null || sats.pos[i].ecef==null ){
		  {
	//              l.warning( "ERROR, sats.pos[i]==null?" );
	//              this.setXYZ(0, 0, 0);
	//              return null;
			int satId = roverObs.getSatID(k);
			ObservationSet os = roverObs.getSatByID(satId);
			os.inUse(false);

			continue;
		  }

		  // Line Of Sight vector units (ECEF)
		  SimpleMatrix e = new SimpleMatrix(1,3);

		  // Line Of Sight vector units (ECEF)
		  e.set(0,0, rover.diffSat[i].get(0) / rover.satAppRange[i]);
		  e.set(0,1, rover.diffSat[i].get(1) / rover.satAppRange[i]);
		  e.set(0,2, rover.diffSat[i].get(2) / rover.satAppRange[i]);

		  // scalar product of speed vector X unit vector
		  float doppler = roverObs.getSatByID(id).getDoppler(ObservationSet.L1);
		  double rodot;
		  if (float.IsNaN(doppler))
		  {
			rodot = -e.mult(sats.pos[i].Speed).get(0);
		  }
		  else
		  {
			// scalar product of speed vector X unit vector
			rodot = -doppler * Constants.SPEED_OF_LIGHT / Constants.FL1;
		  }

		  // Fill in one row in the design matrix
		  A.set(k, 0, e.get(0)); // X
		  A.set(k, 1, e.get(1)); // Y
		  A.set(k, 2, e.get(2)); // Z

		  A.set(k, 3, 1); // clock error
		  A.set(k, 4, rodot);

		  // Add the approximate pseudorange value to b
	//      b.set(k, 0, (rover.satAppRange[i] - sats.pos[i].getSatelliteClockError() * Constants.SPEED_OF_LIGHT) % MODULO );
		  b.set(k, 0, (rover.satAppRange[i] - sats.pos[i].SatelliteClockError * Constants.SPEED_OF_LIGHT));

		  ObservationSet os = roverObs.getSatByID(id);

		  // Add the clock-corrected observed pseudorange value to y0
	//      y0.set(k, 0, roverObs.getSatByIDType(id, satType).getPseudorange(goGPS.getFreq()));
	//      y0.set(k, 0, os.getCodeC(0) % MODULO );
		  y0.set(k, 0, os.getCodeC(0));

	//      if (!ignoreTopocentricParameters) {
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
		  double weight;

		  if (rover.topo[i].Elevation < 15)
		  {
			weight = 1;
		  }
		  else
		  {
			weight = Q.get(k, k) + computeWeight(rover.topo[i].Elevation, roverObs.getSatByIDType(id, 'G').getSignalStrength(goGPS.Freq));
		  }
		  if (weight > 5)
		  {
			weight = 5;
		  }

	//      Q.set(k, k, weight);
		  Q.set(k, k, 1);
	//          if( weight > maxWeight )
	//            maxWeight = weight;

		  // Increment available satellites counter
		  k++;

		} // i loop

		{
		  // Add height soft constraint
		  double lam = Math.toRadians(rover.GeodeticLongitude);
		  double phi = Math.toRadians(rover.GeodeticLatitude);
		  double hR_app = rover.GeodeticHeight;
		  //double hR_app = 0;
		  //double h_dtm = recpos.getGeodeticHeight();

	//    %extraction from the dtm of the height correspondent to the approximated position
	//    [h_dtm] = grid_bilin_interp(E_app, N_app, tile_buffer, tile_header.ncols*3, tile_header.nrows*3, tile_header.cellsize, Ell, Nll, tile_header.nodata);
		  double h_dtm = hR_app > 0? hR_app : 30; // initialize to something above sea level
		  if (h_dtm > goGPS.MaxHeight)
		  {
			h_dtm = goGPS.MaxHeight;
		  }

	//      if( goGPS.useDTM() ){
	//        try {
	//          ElevationResult eres = ElevationApi.getByPoint( getContext(), new LatLng(this.getGeodeticLatitude(), this.getGeodeticLongitude())).await();
	//          if( eres.elevation > 0 )
	//            h_dtm = eres.elevation;
	//        } catch (Exception e1) {
	//          // TODO Auto-generated catch block
	//  //        e.printStackTrace();
	//        }
	//      }

		  double cosLam = Math.Cos(lam);
		  double cosPhi = Math.Cos(phi);
		  double sinLam = Math.Sin(lam);
		  double sinPhi = Math.Sin(phi);
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] data = new double[1][3];
		  double[][] data = RectangularArrays.ReturnRectangularDoubleArray(1, 3);
		  data[0][0] = cosPhi * cosLam;
		  data[0][1] = cosPhi * sinLam;
		  data[0][2] = sinPhi;

		  k = nObsAvail - 1;
		  A.set(k, 0, data[0][0]);
		  A.set(k, 1, data[0][1]);
		  A.set(k, 2, data[0][2]);
		  A.set(k, 3, 0);
		  A.set(k, 4, 0);

	//    %Y0 vector computation for DTM constraint
	//    y0_dtm = h_dtm  - hR_app + cos(phiR_app)*cos(lamR_app)*X_app + cos(phiR_app)*sin(lamR_app)*Y_app + sin(phiR_app)*Z_app;
		  double y0_dtm = h_dtm - hR_app;
		  y0.set(k, 0, y0_dtm);
	//      double max = Q.elementMaxAbs();
	//      Q.set(k, k, max);
		  Q.set(k, k, 1);
		}

	//  if (!ignoreTopocentricParameters) {
		b = b.plus(tropoCorr);
		b = b.plus(ionoCorr);

		rover.satsInUse = 0;
		SimpleMatrix resid = y0.minus(b);


	// use smallest resid
	  double pivot = 0;
	  double pivot_test;
	  int pivot_map = 0;
	  int goodSat;
	  int badSat;
	  int maxGoodSat = 0;

	  // Find pivot satellite whose group has more 'valid' one at the end. The loop test is done at most half of 'sats.avail.size' in idea case
	  do
	  {
		pivot_test = double.MaxValue;
		int pivot_pos = 0;
		for (k = 0; k < sats.avail.Count; k++)
		{
		  if ((pivot_map & (1 << k)) == 0)
		  {
			double d = resid.get(k);
			if (Math.Abs(d) < Math.Abs(pivot_test))
			{
			  pivot_test = d;
			  pivot_pos = k;
			}
		  }
		}

		pivot_map |= (1 << pivot_pos);

	// use highest sat    
	//    double pivot = 0;
	//    double pivotEl = 0;
	//    for( k=0; k<sats.avail.size(); k++){
	//      int satId = roverObs.getSatID(k);
	//      ObservationSet os = roverObs.getSatByID(satId);
	//      if( rover.topo[k].getElevation() > pivotEl ){
	//        pivotEl = rover.topo[k].getElevation();
	//        pivot = resid.get(k);
	//      }
	//    }

		goodSat = 0;
		badSat = 0;
		k = 0;
		for (int i = 0; i < nObs; i++)
		{
		  int satId = roverObs.getSatID(i);

		  if (sats.pos[i] == null || !sats.avail.Keys.contains(satId)) //|| recpos.ecef==null || sats.pos[i].ecef==null ){
		  {
			continue;
		  }

		  double d = resid.get(k);

		  if (d - pivot_test > MODULO / 2)
		  {
			d -= MODULO;
		  }
		  if (d - pivot_test < -MODULO / 2)
		  {
			d += MODULO;
		  }

		  // check again, if fails, exclude this satellite
		  double dms = Math.Abs(d - pivot_test) / Constants.SPEED_OF_LIGHT * 1000;
		  if (Math.Abs(dms) > goGPS.CodeResidThreshold)
		  {
			badSat++;
		  }
		  else
		  {
			goodSat++;
		  }

		  k++;
		}

		if (maxGoodSat < goodSat)
		{
		  maxGoodSat = goodSat;
		  pivot = pivot_test;
		}

	  } while ((badSat > goodSat) && ((1 << sats.avail.Count) > (pivot_map + 1)));

		Console.WriteLine(string.Format("* Residuals -> Adjusted Residuals (ms) - Pivot = {0,7:F4} (ms)", pivot / Constants.SPEED_OF_LIGHT * 1000));

		// Officially check again
		k = 0;
		for (int i = 0; i < nObs; i++)
		{
		  int satId = roverObs.getSatID(i);

		  if (sats.pos[i] == null || !sats.avail.Keys.contains(satId)) //|| recpos.ecef==null || sats.pos[i].ecef==null ){
		  {
			continue;
		  }

		  ObservationSet os = roverObs.getSatByID(satId);

		  double d = resid.get(k);
		  Console.Write(string.Format("{0,2:D}) C:{1,8:F3} ({2,8:F5}); {3,9:F5} -> ", satId, roverObs.getSatByID(satId).getCodeC(0), roverObs.getSatByID(satId).getCodeC(0) / Constants.SPEED_OF_LIGHT * 1000, d / Constants.SPEED_OF_LIGHT * 1000));

		  if (d - pivot > MODULO / 2)
		  {
			d -= MODULO;
		  }
		  if (d - pivot < -MODULO / 2)
		  {
			d += MODULO;
		  }
		  Console.Write(string.Format("{0,9:F5}", d / Constants.SPEED_OF_LIGHT * 1000));
		  Console.Write(string.Format("  Q:{0,3:F1}", Q.get(k,k)));

		  // check again, if fails, exclude this satellite
		  double dms = Math.Abs(d - pivot) / Constants.SPEED_OF_LIGHT * 1000;
		  if (Math.Abs(dms) > goGPS.CodeResidThreshold)
		  {
			if (goGPS.Debug)
			{
				Console.WriteLine(string.Format(" Excluding d:{0,8:F3}", dms));
			}
			resid.set(k, 0);
			A.set(k, 0, 0);
			A.set(k, 1, 0);
			A.set(k, 2, 0);
			A.set(k, 3, 0);
			A.set(k, 4, 0);
			os.inUse(false);
		  }
		  else
		  {
			resid.set(k,d);
			rover.satsInUse++;
			os.inUse(true);
			os.el = rover.topo[i].Elevation;
			Console.WriteLine();
		  }
		  k++;
		}

		if (rover.satsInUse < nUnknowns - 1)
		{
		  Console.WriteLine("Not enough satellites for " + roverObs.RefTime);
		  rover.setXYZ(0, 0, 0);
		  if (rover.status == Status.None)
		  {
			rover.status = Status.NotEnoughSats;
		  }
		  return 0;
		}

	//       Weighted Least squares solution x = ((A'*Q^-1*A)^-1)*A'*Q^-1*(y0-b);
		  x = A.transpose().mult(Q.invert()).mult(A).invert().mult(A.transpose()).mult(Q.invert()).mult(resid);

		 double correction_mag = Math.Sqrt(Math.Pow(x.get(0), 2) + Math.Pow(x.get(1), 2) + Math.Pow(x.get(2), 2));

		 // Common bias in meters and ms
		 double cbias = x.get(3);
		 double cbiasms = cbias * 1000d / Constants.SPEED_OF_LIGHT;
		 // x.get(4) = time update in seconds
		 double tg = x.get(4);

		 // compute eRes 
		 rover.eRes = 0;
		 for (k = 0; k < sats.avail.Count; k++)
		 {
		   int satId = roverObs.getSatID(k);
		   ObservationSet os = roverObs.getSatByID(satId);
		   if (!os.InUse)
		   {
			 continue;
		   }

		   double d = resid.get(k);
		   os.eRes = Math.Abs(d - cbias);
		   rover.eRes += Math.Pow(os.eRes, 2);
		 }
		 rover.eRes = Math.Sqrt(rover.eRes / rover.satsInUse);
		 Console.WriteLine(string.Format("eRes = {0,5:F3}\r\n", rover.eRes));

		 // expected
		 Console.WriteLine(string.Format("pos update:  {0,5:F1}, {1,5:F1}, {2,5:F1}; Mag: {3,5:D}(m)", x.get(0), x.get(1), x.get(2), (long)correction_mag));
		 Console.WriteLine(string.Format("common bias: {0,2:F4} (ms)", cbiasms));
		 Console.WriteLine(string.Format("time update: {0,3:F3} (s)", tg));

		 // Receiver clock error
		 rover.clockError = x.get(3) / Constants.SPEED_OF_LIGHT;

		 // apply correction to Rx position estimate
		 rover.PlusXYZ = x.extractMatrix(0, 3, 0, 1);
		 rover.computeGeodetic();

		 // update refTime
		 if (correction_mag < 10)
		 {
		   unixTime += (long)(tg * 1000);
		   Time newTime = new Time(unixTime);
		   roverObs.RefTime = newTime;
		   rover.RefTime = newTime;
		 }

		 Console.WriteLine(string.Format("recpos: {0,5:F4}, {1,5:F4}, {2,5:F4}, {3}", rover.GeodeticLatitude, rover.GeodeticLongitude, rover.GeodeticHeight, (new Time(unixTime)).ToString()));

		 updateDops(A);

		 return correction_mag; // return correction_mag
	  }

	  /// <summary>
	  /// Coarse time processing with additional Nk variables for code slips </summary>
	  /// <param name="roverObs"> </param>
	  /// <param name="MODULO"> module in meters 
	  /// @return </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double codeStandaloneCoarseTimeCodeAmbs(org.gogpsproject.producer.Observations roverObs, final double MODULO)
	  public virtual double codeStandaloneCoarseTimeCodeAmbs(Observations roverObs, double MODULO)
	  {
		long unixTime = roverObs.RefTime.Msec;

		rover.status = Status.None;

		// Number of GNSS observations without cutoff
		int nObs = roverObs.NumSat;

		// Define least squares matrices
		SimpleMatrix A = null; // aka H or G, // Least squares design matrix
		SimpleMatrix b; // Vector for approximate (estimated, predicted) pseudoranges
		SimpleMatrix y0; // Observed pseudoranges
		SimpleMatrix Q = null; // Cofactor (Weighted) Matrix
		SimpleMatrix x; // Solution (Update) Vector
		SimpleMatrix vEstim; // Observation Errors
		SimpleMatrix tropoCorr;
		SimpleMatrix ionoCorr;

		// Number of available satellites (i.e. observations)
		int nObsAvail = sats.avail.Count;

		nObsAvail++; // add DTM / height soft constraint

		// Number of unknown parameters
		int nUnknowns = sats.avail.Count;

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

		// Counter for available satellites
		int k = 0;

		// Satellite ID
		int id = 0;

		// assign code ambiguities
		if (codeAmbiguities == null)
		{
		  codeAmbiguities = new double[nObs];
		  Arrays.fill(codeAmbiguities, 0);
		}

		// Set up the least squares matrices
		for (int i = 0; i < nObs; i++)
		{

		  id = roverObs.getSatID(i);

		  if (sats.pos[i] == null || !sats.avail.Keys.contains(id)) //|| recpos.ecef==null || sats.pos[i].ecef==null ){
		  {
	//              l.warning( "ERROR, sats.pos[i]==null?" );
	//              this.setXYZ(0, 0, 0);
	//              return null;
			int satId = roverObs.getSatID(k);
			ObservationSet os = roverObs.getSatByID(satId);
			os.inUse(false);

			continue;
		  }

		  A.set(k, k, -MODULO);

		  b.set(k, 0, (rover.satAppRange[i] - sats.pos[i].SatelliteClockError * Constants.SPEED_OF_LIGHT));

		  ObservationSet os = roverObs.getSatByID(id);

		  // Add the observed pseudorange value to y0
		  y0.set(k, 0, os.getCodeC(0));

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

		  Q.set(k, k, 1);

		  k++;
		}

		{
		  // Add height soft constraint
		  double lam = Math.toRadians(rover.GeodeticLongitude);
		  double phi = Math.toRadians(rover.GeodeticLatitude);
		  double hR_app = rover.GeodeticHeight;
		  double h_dtm = hR_app > 0? hR_app : 30; // initialize to something above sea level
		  if (h_dtm > 3000)
		  {
			h_dtm = 3000;
		  }

		  double cosLam = Math.Cos(lam);
		  double cosPhi = Math.Cos(phi);
		  double sinLam = Math.Sin(lam);
		  double sinPhi = Math.Sin(phi);

		  k = nObsAvail - 1;
		  A.set(k, 0, cosPhi * cosLam);
		  A.set(k, 1, cosPhi * sinLam);
		  A.set(k, 2, sinPhi);

		  y0.set(k, 0, h_dtm - hR_app);
		  Q.set(k, k, 1);
		}

		b = b.plus(tropoCorr);
		b = b.plus(ionoCorr);

		rover.satsInUse = 0;
		SimpleMatrix resid = y0.minus(b);

		double pivot = MODULO;
		for (k = 0; k < sats.avail.Count; k++)
		{
		  int satId = roverObs.getSatID(k);

		  if (!sats.avail.Keys.contains(satId) || sats.pos[k] == null || rover.topo[k] == null)
		  {
			continue;
		  }

		  if (Math.Abs(resid.get(k)) < pivot)
		  {
			pivot = Math.Abs(resid.get(k));
		  }
		}

		Console.WriteLine(string.Format("* Residuals -> Adjusted Residuals (ms) - Pivot = {0,7:F4} (ms)", pivot / Constants.SPEED_OF_LIGHT * 1000));

		for (k = 0; k < sats.avail.Count; k++)
		{
		  int satId = roverObs.getSatID(k);
		  ObservationSet os = roverObs.getSatByID(satId);

		  double d = resid.get(k);
		  Console.Write(string.Format("{0,2:D}) C:{1,8:F3} ({2,8:F5}); {3,9:F5} -> ", satId, roverObs.getSatByID(satId).getCodeC(0), roverObs.getSatByID(satId).getCodeC(0) / Constants.SPEED_OF_LIGHT * 1000, d / Constants.SPEED_OF_LIGHT * 1000));

		  d += codeAmbiguities[k] * MODULO;

		  Console.Write(string.Format("{0,9:F5}", d / Constants.SPEED_OF_LIGHT * 1000));
		  Console.Write(string.Format("  Q:{0,3:F1}; N:{1,6:F2}", Q.get(k, k), codeAmbiguities[k]));

		  resid.set(k,d);

		  rover.satsInUse++;
		  os.inUse(true);
		  Console.WriteLine();
		  rover.eRes += Math.Pow(d - pivot, 2);
		}
		rover.eRes = Math.Sqrt(rover.eRes / rover.satsInUse);
		Console.WriteLine(string.Format("eRes = {0,5:F3}\r\n", rover.eRes));

		if (rover.satsInUse < nUnknowns - 1)
		{
		  Console.WriteLine("Not enough satellites for " + roverObs.RefTime);
		  rover.setXYZ(0, 0, 0);
		  if (rover.status == Status.None)
		  {
			rover.status = Status.NotEnoughSats;
		  }
		  return 0;
		}

	//       Weighted Least squares solution x = ((A'*Q^-1*A)^-1)*A'*Q^-1*(y0-b);
		  x = A.transpose().mult(Q.invert()).mult(A).invert().mult(A.transpose()).mult(Q.invert()).mult(resid);

		 double correction_mag = Math.Sqrt(Math.Pow(x.get(0), 2) + Math.Pow(x.get(1), 2) + Math.Pow(x.get(2), 2));

		 // x.get(3) = Receiver clock error in meters
		 double cbiasms = x.get(3) * 1000d / Constants.SPEED_OF_LIGHT;
		 double tg = x.get(4); // time update in seconds

		 // expected
		 Console.WriteLine(string.Format("pos update:  {0,5:F1}, {1,5:F1}, {2,5:F1}; Mag: {3,5:D}(m)", x.get(0), x.get(1), x.get(2), (long)correction_mag));
		 Console.WriteLine(string.Format("common bias: {0,2:F4} (ms)", cbiasms));
		 Console.WriteLine(string.Format("time update: {0,3:F3} (s)", tg));

		 Console.WriteLine("ambiguities: ");
		 for (k = 0; k < codeAmbiguities.Length; k++)
		 {
		   Console.Write(string.Format("{0,5:F2} ->", x.get(k)));
		   codeAmbiguities[k] = x.get(k);
		   Console.WriteLine(string.Format("{0,5:F2}", codeAmbiguities[k]));
		 }

		 {
		 // only update position when ambiguities have converged
		 // Receiver clock error
	//     this.receiverClockError = x.get(3) / Constants.SPEED_OF_LIGHT;

		 // apply correction to Rx position estimate
	//     this.setPlusXYZ(x.extractMatrix(0, 3, 0, 1));
	//     this.computeGeodetic();
		 }

		 // update refTime
		 if (correction_mag < 10)
		 {
		   unixTime += (long)(tg * 1000);
		   Time newTime = new Time(unixTime);
		   roverObs.RefTime = newTime;
		   rover.RefTime = newTime;
		 }

		 Console.WriteLine(string.Format("recpos: {0,5:F4}, {1,5:F4}, {2,5:F4}, {3}", rover.GeodeticLatitude, rover.GeodeticLongitude, rover.GeodeticHeight, (new Time(unixTime)).ToString()));

		 updateDops(A);

		 return correction_mag; // return correction_mag
	  }

	  /// <summary>
	  /// Modular case for only 3 satellites (don't compensate for coarse time error) </summary>
	  /// <param name="roverObs"> </param>
	  /// <param name="MODULO"> module in meters 
	  /// @return </param>
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public double codeStandaloneDTM(org.gogpsproject.producer.Observations roverObs, final double MODULO)
	  public virtual double codeStandaloneDTM(Observations roverObs, double MODULO)
	  {
		long unixTime = roverObs.RefTime.Msec;

		rover.status = Status.None;

		// Number of GNSS observations without cutoff
		int nObs = roverObs.NumSat;

		// Number of unknown parameters
		int nUnknowns = 4;

		// Define least squares matrices
		SimpleMatrix A = null; // aka H or G, // Least squares design matrix
		SimpleMatrix b; // Vector for approximate (estimated, predicted) pseudoranges
		SimpleMatrix y0; // Observed pseudoranges
		SimpleMatrix Q = null; // Cofactor (Weighted) Matrix
		SimpleMatrix x; // Solution (Update) Vector
		SimpleMatrix vEstim; // Observation Errors
		SimpleMatrix tropoCorr;
		SimpleMatrix ionoCorr;

		// Number of available satellites (i.e. observations)
		int nObsAvail = sats.avail.Count;

		nObsAvail++; // add DTM / height soft constraint

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

		// Counter for available satellites
		int k = 0;

		// Satellite ID
		int id = 0;

		// Set up the least squares matrices
		for (int i = 0; i < nObs; i++)
		{

		  id = roverObs.getSatID(i);

		  if (sats.pos[i] == null || !sats.avail.Keys.contains(id)) //|| recpos.ecef==null || sats.pos[i].ecef==null ){
		  {
			int satId = roverObs.getSatID(k);
			ObservationSet os = roverObs.getSatByID(satId);
			os.inUse(false);

			continue;
		  }

		  // Line Of Sight vector units (ECEF)
		  SimpleMatrix e = new SimpleMatrix(1,3);

		  // Line Of Sight vector units (ECEF)
		  e.set(0,0, rover.diffSat[i].get(0) / rover.satAppRange[i]);
		  e.set(0,1, rover.diffSat[i].get(1) / rover.satAppRange[i]);
		  e.set(0,2, rover.diffSat[i].get(2) / rover.satAppRange[i]);

		  // scalar product of speed vector X unit vector
		  float doppler = roverObs.getSatByID(id).getDoppler(ObservationSet.L1);
		  double rodot;
		  if (float.IsNaN(doppler))
		  {
			rodot = -e.mult(sats.pos[i].Speed).get(0);
		  }
		  else
		  {
			// scalar product of speed vector X unit vector
			rodot = -doppler * Constants.SPEED_OF_LIGHT / Constants.FL1;
		  }

		  // Fill in one row in the design matrix
		  A.set(k, 0, e.get(0)); // X
		  A.set(k, 1, e.get(1)); // Y
		  A.set(k, 2, e.get(2)); // Z

		  A.set(k, 3, 1); // clock error
		 // A.set(k, 4, rodot );

		  // Add the approximate pseudorange value to b
	//      b.set(k, 0, (rover.satAppRange[i] - sats.pos[i].getSatelliteClockError() * Constants.SPEED_OF_LIGHT) % MODULO );
		  b.set(k, 0, (rover.satAppRange[i] - sats.pos[i].SatelliteClockError * Constants.SPEED_OF_LIGHT));

		  ObservationSet os = roverObs.getSatByID(id);

		  // Add the clock-corrected observed pseudorange value to y0
	//      y0.set(k, 0, roverObs.getSatByIDType(id, satType).getPseudorange(goGPS.getFreq()));
	//      y0.set(k, 0, os.getCodeC(0) % MODULO );
		  y0.set(k, 0, os.getCodeC(0));

	//      if (!ignoreTopocentricParameters) {
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
		  double weight;

		  if (rover.topo[i].Elevation < 15)
		  {
			weight = 1;
		  }
		  else
		  {
			weight = Q.get(k, k) + computeWeight(rover.topo[i].Elevation, roverObs.getSatByIDType(id, 'G').getSignalStrength(goGPS.Freq));
		  }
		  if (weight > 5)
		  {
			weight = 5;
		  }

	//      Q.set(k, k, weight);
		  Q.set(k, k, 1);
	//          if( weight > maxWeight )
	//            maxWeight = weight;

		  // Increment available satellites counter
		  k++;

		} // i loop

		{
		  // Add height soft constraint
		  double lam = Math.toRadians(rover.GeodeticLongitude);
		  double phi = Math.toRadians(rover.GeodeticLatitude);
		  double hR_app = rover.GeodeticHeight;
		  //double hR_app = 0;
		  //double h_dtm = recpos.getGeodeticHeight();

	//    %extraction from the dtm of the height correspondent to the approximated position
	//    [h_dtm] = grid_bilin_interp(E_app, N_app, tile_buffer, tile_header.ncols*3, tile_header.nrows*3, tile_header.cellsize, Ell, Nll, tile_header.nodata);
		  double h_dtm = hR_app > 0? hR_app : 30; // initialize to something above sea level
		  if (h_dtm > goGPS.MaxHeight)
		  {
			h_dtm = goGPS.MaxHeight;
		  }

	//      if( goGPS.useDTM() ){
	//        try {
	//          ElevationResult eres = ElevationApi.getByPoint( getContext(), new LatLng(this.getGeodeticLatitude(), this.getGeodeticLongitude())).await();
	//          if( eres.elevation > 0 )
	//            h_dtm = eres.elevation;
	//        } catch (Exception e1) {
	//          // TODO Auto-generated catch block
	//  //        e.printStackTrace();
	//        }
	//      }

		  double cosLam = Math.Cos(lam);
		  double cosPhi = Math.Cos(phi);
		  double sinLam = Math.Sin(lam);
		  double sinPhi = Math.Sin(phi);
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] data = new double[1][3];
		  double[][] data = RectangularArrays.ReturnRectangularDoubleArray(1, 3);
		  data[0][0] = cosPhi * cosLam;
		  data[0][1] = cosPhi * sinLam;
		  data[0][2] = sinPhi;

		  k = nObsAvail - 1;
		  A.set(k, 0, data[0][0]);
		  A.set(k, 1, data[0][1]);
		  A.set(k, 2, data[0][2]);
		  A.set(k, 3, 0);
	//      A.set(k, 4, 0 );

	//    %Y0 vector computation for DTM constraint
	//    y0_dtm = h_dtm  - hR_app + cos(phiR_app)*cos(lamR_app)*X_app + cos(phiR_app)*sin(lamR_app)*Y_app + sin(phiR_app)*Z_app;
		  double y0_dtm = h_dtm - hR_app;
		  y0.set(k, 0, y0_dtm);
	//      double max = Q.elementMaxAbs();
	//      Q.set(k, k, max);
		  Q.set(k, k, 1);
		}

	//  if (!ignoreTopocentricParameters) {
		b = b.plus(tropoCorr);
		b = b.plus(ionoCorr);

		rover.satsInUse = 0;
		SimpleMatrix resid = y0.minus(b);

	// use smallest resid
		double pivot = MODULO;
		for (k = 0; k < sats.avail.Count; k++)
		{
		  double d = resid.get(k);
		  if (Math.Abs(d) < Math.Abs(pivot))
		  {
			pivot = d;
		  }
		}

	// use highest sat    
	//    double pivot = 0;
	//    double pivotEl = 0;
	//    for( k=0; k<sats.avail.size(); k++){
	//      int satId = roverObs.getSatID(k);
	//      ObservationSet os = roverObs.getSatByID(satId);
	//      if( rover.topo[k] == null )
	//        continue;
	//      
	//      if( rover.topo[k].getElevation() > pivotEl ){
	//        pivotEl = rover.topo[k].getElevation();
	//        pivot = resid.get(k);
	//      }
	//    }

		Console.WriteLine(string.Format("* Residuals -> Adjusted Residuals (ms) - Pivot = {0,7:F4} (ms)", pivot / Constants.SPEED_OF_LIGHT * 1000));

		for (k = 0; k < sats.avail.Count; k++)
		{
		  int satId = roverObs.getSatID(k);
		  ObservationSet os = roverObs.getSatByID(satId);

		  double d = resid.get(k);
		  Console.Write(string.Format("{0,2:D}) C:{1,8:F3} ({2,8:F5}); {3,9:F5} -> ", satId, roverObs.getSatByID(satId).getCodeC(0), roverObs.getSatByID(satId).getCodeC(0) / Constants.SPEED_OF_LIGHT * 1000, d / Constants.SPEED_OF_LIGHT * 1000));

		  if (d - pivot > MODULO / 2)
		  {
			d -= MODULO;
		  }
		  if (d - pivot < -MODULO / 2)
		  {
			d += MODULO;
		  }
		  Console.Write(string.Format("{0,9:F5}", d / Constants.SPEED_OF_LIGHT * 1000));
		  Console.Write(string.Format("  Q:{0,3:F1}", Q.get(k,k)));

		  // check again, if fails, exclude this satellite
		  double dms = Math.Abs(d - pivot) / Constants.SPEED_OF_LIGHT * 1000;
		  if (Math.Abs(dms) > goGPS.CodeResidThreshold)
		  {
			if (goGPS.Debug)
			{
				Console.WriteLine(string.Format(" Excluding d:{0,8:F3}", dms));
			}
			resid.set(k, 0);
			A.set(k, 0, 0);
			A.set(k, 1, 0);
			A.set(k, 2, 0);
			A.set(k, 3, 0);
			os.inUse(false);
			os.eRes = d - pivot;
			os.el = rover.topo[k].Elevation;
		  }
		  else
		  {
			resid.set(k,d);
			rover.satsInUse++;
			os.inUse(true);
			Console.WriteLine();
			rover.eRes += Math.Pow(d - pivot, 2);
		  }
		}
		rover.eRes = Math.Sqrt(rover.eRes / rover.satsInUse);
		Console.WriteLine(string.Format("eRes = {0,5:F3}\r\n", rover.eRes));

		if (rover.satsInUse < nUnknowns - 1)
		{
		  Console.WriteLine("Not enough satellites for " + roverObs.RefTime);
		  rover.setXYZ(0, 0, 0);
		  if (rover.status == Status.None)
		  {
			rover.status = Status.NotEnoughSats;
		  }
		  return 0;
		}

	//       Weighted Least squares solution x = ((A'*Q^-1*A)^-1)*A'*Q^-1*(y0-b);
		  x = A.transpose().mult(Q.invert()).mult(A).invert().mult(A.transpose()).mult(Q.invert()).mult(resid);

		 double correction_mag = Math.Sqrt(Math.Pow(x.get(0), 2) + Math.Pow(x.get(1), 2) + Math.Pow(x.get(2), 2));

		 // x.get(3) = Receiver clock error in meters
		 double cbiasms = x.get(3) * 1000d / Constants.SPEED_OF_LIGHT;
		 // x.get(4) = time update in seconds
	//     double tg = x.get(4); 

		 // expected
		 Console.WriteLine(string.Format("pos update:  {0,5:F1}, {1,5:F1}, {2,5:F1}; Mag: {3,5:D}(m)", x.get(0), x.get(1), x.get(2), (long)correction_mag));
		 Console.WriteLine(string.Format("common bias: {0,2:F4} (ms)", cbiasms));
	//     System.out.println( String.format( "time update: %3.3f (s)", tg ));

		 // Receiver clock error
		 rover.clockError = x.get(3) / Constants.SPEED_OF_LIGHT;

		 // apply correction to Rx position estimate
		 rover.PlusXYZ = x.extractMatrix(0, 3, 0, 1);
		 rover.computeGeodetic();

		 // update refTime
	//     if( correction_mag < 10 ){
	//       unixTime += tg * 1000;
	//       Time newTime = new Time( unixTime);
	//       roverObs.setRefTime( newTime );
	//       this.setRefTime( newTime );
	//     }

		 Console.WriteLine(string.Format("recpos: {0,5:F4}, {1,5:F4}, {2,5:F4}, {3}", rover.GeodeticLatitude, rover.GeodeticLongitude, rover.GeodeticHeight, (new Time(unixTime)).ToString()));

		 updateDops(A);

		 return correction_mag; // return correction_mag
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public void runCoarseTime(org.gogpsproject.producer.Observations obsR, final double MODULO)
	  public virtual void runCoarseTime(Observations obsR, double MODULO)
	  {
		/// <summary>
		/// pos update limit for LMS iterations </summary>
		const double POS_TOL = 1.0; // meters

		/// <summary>
		/// time update limit for LMS iterations </summary>
		const double TG_TOL = 1; // milliseconds

		for (int iter = 0; iter < 2000; iter++)
		{
		  if (goGPS.Debug)
		  {
			  Console.WriteLine("\r\n////// itr = " + iter);
		  }
		  long updatems = obsR.RefTime.Msec;

	//      if( truePos != null ){
	//        System.out.println( String.format( "\r\n* True Pos: %8.4f, %8.4f, %8.4f", 
	//            truePos.getGeodeticLatitude(),
	//            truePos.getGeodeticLongitude(),
	//            truePos.getGeodeticHeight()
	//            ));
	//        truePos.selectSatellitesStandaloneFractional( obsR, -100, MODULO20MS );
	//      }

		  Console.WriteLine(string.Format("\r\n* Rover Pos: {0,8:F4}, {1,8:F4}, {2,8:F4}", rover.GeodeticLatitude, rover.GeodeticLongitude, rover.GeodeticHeight));
		  selectSatellites(obsR, -100, GoGPS.MODULO20MS);
		  Console.WriteLine();

		  if (sats.AvailNumber < 3)
		  {
			if (goGPS.Debug)
			{
				Console.WriteLine("Not enough satellites");
			}
			rover.setXYZ(0, 0, 0);
			rover.status = Status.NotEnoughSats;
			break;
		  }
		  else
		  {
			double correction_mag = sats.AvailNumber == 3? codeStandaloneDTM(obsR, MODULO) : codeStandaloneCoarseTime(obsR, MODULO);
			updatems = obsR.RefTime.Msec - updatems;

			if (Math.Abs(updatems / 1000) > 12 * 60 * 60)
			{
			  if (goGPS.Debug)
			  {
				  Console.WriteLine("Time update is too large: " + updatems / 1000 + " s");
			  }
			  rover.setXYZ(0, 0, 0);
			  if (rover.status == Status.None)
			  {
				rover.status = Status.MaxCorrection;
			  }
			  break;
			}

			if (rover.status != Status.None && rover.status != Status.Valid)
			{
			  break;
			}

			// if correction is small enough, we're done, exit loop
			if (correction_mag < POS_TOL && Math.Abs(updatems) < TG_TOL)
			{
			   rover.status = Status.Valid;
			   break;
			}
		  }
		}
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not allowed in .NET:
//ORIGINAL LINE: public static void run(org.gogpsproject.GoGPS goGPS, final double MODULO)
	  public static void run(GoGPS goGPS, double MODULO)
	  {

		RoverPosition rover = goGPS.getRoverPos();
		MasterPosition master = goGPS.getMasterPos();
		Satellites sats = goGPS.Sats;

		LS_SA_code_coarse_time sa = new LS_SA_code_coarse_time(goGPS);

		long index = 0;
		Observations obsR = null;
		Time refTime;
		int leapSeconds;

		// read the whole file
		IList<Observations> obsl = new List<Observations>();
		do
		{
		  obsR = goGPS.RoverIn.NextObservations;
		  if (obsR != null)
		  {
			obsl.Add(obsR);
		  }
		} while (obsR != null);

		  Coordinates aPrioriPos = goGPS.RoverIn.DefinedPosition;
		  if (aPrioriPos != null && aPrioriPos.ValidXYZ)
		  {
			rover.setXYZ(aPrioriPos.X, aPrioriPos.Y, aPrioriPos.Z);
			rover.computeGeodetic();
		  }
		  else
		  {
			aPrioriPos = Coordinates.globalXYZInstance(0, 0, 0);

			Console.WriteLine("\r\nSearching for a priori position");

			long maxNumSat = 0;
			index = 0;
			long maxSatIdx = 0;
			Observations maxSatObs = null;
			// find the observation set with most satellites
			IEnumerator<Observations> it = obsl.GetEnumerator();
			while (it.MoveNext()) // buffStreamObs.ready()
			{
			  obsR = it.Current;
			  if (obsR == null)
			  {
				break;
			  }
			  // search for an observation with at least 6 satellites to produce an a priori position using the elevation method
			  if (obsR.NumSat > maxNumSat)
			  {
				maxNumSat = obsR.NumSat;
				maxSatObs = obsR;
				maxSatIdx = index;
			  }
			  index++;
			}

			rover.status = Status.NoAprioriPos;
			sa.runElevationMethod(maxSatObs);

			if (rover.status == Status.Valid)
			{
				// remember refTime
				refTime = maxSatObs.RefTime;

				double thr = goGPS.CodeResidThreshold;
				goGPS.CodeResidThreshold = MODULO;
				sa.runCoarseTime(maxSatObs, MODULO);
				// restore obsR refTime
				maxSatObs.RefTime = refTime;
				goGPS.CodeResidThreshold = thr;

				rover.cloneInto(aPrioriPos);
				rover.status = Status.None;
			}
		  }

		// now process all the observation sets from the top of the file
		IEnumerator<Observations> it = obsl.GetEnumerator();
		goGPS.notifyPositionConsumerEvent(org.gogpsproject.consumer.PositionConsumer_Fields.EVENT_START_OF_TRACK);
		index = 0;
		try
		{
		  while (it.MoveNext())
		  {
			obsR = it.Current;
			if (obsR == null)
			{
			  break;
			}

			index++;

			if (goGPS.Debug)
			{
			  Console.WriteLine("==========================================================================================");
			  Console.WriteLine("Index = " + index);
			  Console.WriteLine("Processing " + obsR);
			}

			rover.status = Status.None;

			// apply offset
			refTime = obsR.RefTime;
			rover.sampleTime = refTime;

			// Add Leap Seconds, remove at the end
			leapSeconds = refTime.LeapSeconds;
			Time GPSTime = new Time(refTime.Msec + leapSeconds * 1000);
			obsR.RefTime = GPSTime;
			Time newTime = new Time(obsR.RefTime.Msec + goGPS.Offsetms);
			obsR.RefTime = newTime;
			long newTimeRefms = obsR.RefTime.Msec;

			if (!rover.ValidXYZ)
			{
			  if (obsR.NumSat < 6)
			  {
				rover.status = Status.NoAprioriPos;
			  }
			  else
			  {
				sa.runElevationMethod(obsR);
			  }
			}

			// If an approximate position was computed
			if (!rover.ValidXYZ)
			{
			  if (goGPS.Debug)
			  {
				  Console.WriteLine("Couldn't compute an approximate position at " + obsR.RefTime);
			  }
			  if (rover.status == Status.None)
			  {
				rover.status = Status.NoAprioriPos;
			  }
			  continue;
			}
			else
			{

			  if (goGPS.Debug)
			  {
				  Console.WriteLine("Approximate position at " + obsR.RefTime + "\r\n" + rover);
			  }

			  sa.runCoarseTime(obsR, MODULO);
			}

			if (!rover.ValidXYZ || rover.gethDop() > goGPS.HdopLimit)
			{
			  rover.sampleTime = refTime;

			  if (rover.ValidXYZ && rover.gethDop() > goGPS.HdopLimit)
			  {
				Console.WriteLine(string.Format("Excluding fix hdop = {0,3:F1} > {1,3:F1} (limit)", rover.gethDop(), goGPS.HdopLimit));
				rover.status = Status.MaxHDOP;
			  }
			  // restore a priori location
			  if (aPrioriPos != null && aPrioriPos.ValidXYZ)
			  {
				aPrioriPos.cloneInto(rover);
			  }
			}
			else
			{
			  double offsetUpdate = obsR.RefTime.Msec - newTimeRefms;
			  goGPS.Offsetms = (long)(goGPS.Offsetms + offsetUpdate);

			  // remove Leap Seconds
			  obsR.RefTime = new Time(obsR.RefTime.Msec - leapSeconds * 1000);

			  // update aPrioriPos
			  rover.cloneInto(aPrioriPos);

			  if (goGPS.Debug)
			  {
				  Console.WriteLine("Valid position? " + rover.ValidXYZ + "\r\n" + rover);
			  }
			  if (goGPS.Debug)
			  {
				  Console.WriteLine(" lat:" + rover.GeodeticLatitude + " lon:" + rover.GeodeticLongitude);
			  }
			  if (goGPS.Debug)
			  {
				  Console.WriteLine(" time offset update (ms): " + offsetUpdate + "; Total time offset (ms): " + goGPS.Offsetms);
			  }

			  rover.cErrMS = obsR.RefTime.Msec - rover.sampleTime.Msec;
			}
			if (goGPS.PositionConsumers.Count > 0)
			{
			  rover.RefTime = new Time(obsR.RefTime.Msec);
			  goGPS.notifyPositionConsumerAddCoordinate(rover.clone(obsR));
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
	  }

	}

}