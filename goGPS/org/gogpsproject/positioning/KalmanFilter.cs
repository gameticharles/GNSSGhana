
using ReinGametiMatrixLib.Matricies;
using System;
using System.Collections.Generic;

namespace org.gogpsproject.positioning
{
	
	using ObservationSet = org.gogpsproject.producer.ObservationSet;
	using Observations = org.gogpsproject.producer.Observations;

	public abstract class KalmanFilter : LS_DD_code
	{

	  /// <summary>
	  /// The st dev init. </summary>
	  private const double stDevInit = 1;

	  /// <summary>
	  /// The st dev e. </summary>
	  private const double stDevE = 0.5;

	  /// <summary>
	  /// The st dev n. </summary>
	  private const double stDevN = 0.5;

	  /// <summary>
	  /// The st dev u. </summary>
	  private const double stDevU = 0.1;

	  /// <summary>
	  /// The st dev code c. </summary>
	  private const double stDevCodeC = 0.3;

	  /// <summary>
	  /// The st dev code p. </summary>
	  private static readonly double[] stDevCodeP = new double[] {0.6, 0.4};

	  /// <summary>
	  /// The st dev phase. </summary>
	  internal const double stDevPhase = 0.003;

	  /// <summary>
	  /// The st dev ambiguity. </summary>
	  internal const double stDevAmbiguity = 10;

	  internal int o1, o2, o3;
	  internal int i1, i2, i3;
	  internal int nN;

	  internal Matrix T;
	  internal Matrix H;
	  internal Matrix y0;
	  internal Matrix Cvv;
	  internal Matrix Cee;
	  internal Matrix Cnn;
	  internal Matrix KFstate;
	  internal Matrix KFprediction;

	  // Fields for keeping track of satellite configuration changes
	  internal List<int?> satOld;
	  internal List<char?> satTypeOld;
	  internal int oldPivotId;
	  internal char oldPivotType;

	  public KalmanFilter(GoGPS goGPS) : base(goGPS)
	  {
	  }

	  internal abstract void setup(Observations roverObs, Observations masterObs, Coordinates masterPos);
	  internal abstract void estimateAmbiguities(Observations roverObs, Observations masterObs, Coordinates masterPos, List<int?> satAmb, int pivotIndex, bool init);
	  internal abstract void checkSatelliteConfiguration(Observations roverObs, Observations masterObs, Coordinates masterPos);


	  /// <summary>
	  /// Gets the st dev code.
	  /// </summary>
	  /// <param name="roverObsSet"> the rover observation set </param>
	  /// <param name="masterObsSet"> the master observation set </param>
	  /// <param name="i"> the selected GPS frequency </param>
	  /// <returns> the stDevCode </returns>
	  public virtual double getStDevCode(ObservationSet obsSet, int i)
	  {
		return obsSet.isPseudorangeP(i)?stDevCodeP[i]:stDevCodeC;
	  }


	  /// <param name="roverObs"> </param>
	  /// <param name="masterObs"> </param>
	  internal virtual void computeDopplerPredictedPhase(Observations roverObs, Observations masterObs)
	  {

		rover.dopplerPredPhase = new double[32];

		if (masterObs != null)
		{
		  master.dopplerPredPhase = new double[32];
		}

		for (int i = 0; i < sats.availPhase.Count; i++)
		{

		  int satID = (int)sats.availPhase[i];
		  char satType = (char)sats.typeAvailPhase[i];

		  double roverPhase = roverObs.getSatByIDType(satID, satType).getPhaseCycles(goGPS.Freq);
		  float roverDoppler = roverObs.getSatByIDType(satID, satType).getDoppler(goGPS.Freq);
		  if (!double.IsNaN(roverPhase) && !float.IsNaN(roverDoppler))
		  {
			rover.setDopplerPredictedPhase((int)sats.availPhase[i], roverPhase - roverDoppler);
		  }

		  if (masterObs != null)
		  {
			double masterPhase = masterObs.getSatByIDType(satID, satType).getPhaseCycles(goGPS.Freq);
			float masterDoppler = masterObs.getSatByIDType(satID, satType).getDoppler(goGPS.Freq);
			if (!double.IsNaN(masterPhase) && !float.IsNaN(masterDoppler))
			{
			  master.setDopplerPredictedPhase((int)sats.availPhase[i], masterPhase - masterDoppler);
			}
		  }
		}
	  }

	  public virtual void init(Observations roverObs, Observations masterObs, Coordinates masterPos)
	  {

		// Order-related quantities
		o1 = goGPS.DynamicModel.Order;
		o2 = goGPS.DynamicModel.Order * 2;
		o3 = goGPS.DynamicModel.Order * 3;

		// Order-related indices
		i1 = o1 - 1;
		i2 = o2 - 1;
		i3 = o3 - 1;

		// Set number of ambiguities
		if (goGPS.DualFreq)
		{
		  nN = 64;
		}
		else
		{
		  nN = 32;
		}

		// Allocate matrices
		T = Matrix.Identity(o3 + nN);
		KFstate = new Matrix(o3 + nN, 1);
		KFprediction = new Matrix(o3 + nN, 1);
		Cvv = new Matrix(o3 + nN, o3 + nN);
		Cee = new Matrix(o3 + nN, o3 + nN);

		// System dynamics
		int j = 0;
		for (int i = 0; i < o3; i++)
		{
		  if (j < (o1 - 1))
		  {
			T.SetElement(i, i + 1, 1);
			j++;
		  }
		  else
		  {
			j = 0;
		  }
		}

		// Model error covariance matrix
		Cvv = Matrix.Zero(3);
        Cvv.SetElement(i1, i1, Math.Pow(stDevE, 2));
		Cvv.SetElement(i2, i2, Math.Pow(stDevN, 2));
		Cvv.SetElement(i3, i3, Math.Pow(stDevU, 2));

		// Improve approximate position accuracy by applying twice code double differences
		for (int i = 0; i < 2; i++)
		{
		  // Select satellites available for double differences
		  if (masterObs != null)
		  {
			sats.selectDoubleDiff(roverObs, masterObs, masterPos);
		  }
		  else
		  {
			sats.selectStandalone(roverObs);
		  }

		  if (sats.avail.Count >= 4)
		  {
			if (masterObs != null)
			{
			  codeDoubleDifferences(roverObs, masterObs, masterPos);
			}
			else
			{
			  codeStandalone(roverObs, false, false);
			}
		  }
		  else
		  {
			rover.setXYZ(0, 0, 0);
			return;
		  }
		}

		// Estimate phase ambiguities
		List<int?> newSatellites = new List<int?>(0);
		newSatellites.AddRange(sats.availPhase);

		estimateAmbiguities(roverObs, masterObs, masterPos, newSatellites, sats.pivot, true);

		// Compute predicted phase ranges based on Doppler observations
		computeDopplerPredictedPhase(roverObs, masterObs);

		// Initial state
		KFstate.SetElement(0, 0, rover.X);
		KFstate.SetElement(i1 + 1, 0, rover.Y);
		KFstate.SetElement(i2 + 1, 0, rover.Z);

		// Prediction
		KFprediction = T.Multiply(KFstate);

		// Covariance matrix of the initial state
		if (positionCovariance != null)
		{
		    Cee.SetElement(0, 0, positionCovariance.GetElement(0, 0));
		    Cee.SetElement(i1 + 1, i1 + 1, positionCovariance.GetElement(1, 1));
		    Cee.SetElement(i2 + 1, i2 + 1, positionCovariance.GetElement(2, 2));
		}
		else
		{
		    positionCovariance = new Matrix(3, 3);
		    Cee.SetElement(0, 0, Math.Pow(stDevInit, 2));
		    Cee.SetElement(i1 + 1, i1 + 1, Math.Pow(stDevInit, 2));
		    Cee.SetElement(i2 + 1, i2 + 1, Math.Pow(stDevInit, 2));
		}
		for (int i = 1; i < o1; i++)
		{
		    Cee.SetElement(i, i, Math.Pow(stDevInit, 2));
		    Cee.SetElement(i + i1 + 1, i + i1 + 1, Math.Pow(stDevInit, 2));
		    Cee.SetElement(i + i2 + 1, i + i2 + 1, Math.Pow(stDevInit, 2));
		}
	  }

	  internal virtual Matrix compute_residuals(Matrix X)
	  {
		  Matrix residuals = y0.Minus(H.Multiply(X));
		  return residuals;
	  }

	  /// <param name="roverObs"> </param>
	  /// <param name="masterObs"> </param>
	  /// <param name="masterPos">
	  ///  </param>
	  public virtual void loop(Observations roverObs, Observations masterObs, Coordinates masterPos)
	  {

		const int minNumSat = 2;

		// Set linearization point (approximate coordinates by KF prediction at previous step)
		rover.setXYZ(KFprediction.GetElement(0, 0), KFprediction.GetElement(i1 + 1, 0), KFprediction.GetElement(i2 + 1, 0));

		// Save previous list of available satellites with phase
		satOld = sats.availPhase;
		satTypeOld = sats.typeAvailPhase;

		// Save the ID and index of the previous sats.pivot satellite
		try
		{
		  oldPivotId = sats.pos[sats.pivot].SatID;
		  oldPivotType = sats.pos[sats.pivot].SatType;
		}
		catch (System.IndexOutOfRangeException)
		{
		  oldPivotId = 0;
		}

		// Select satellites for standalone
		sats.selectStandalone(roverObs);

		if (sats.avail.Count >= 4)
		{
		  // Estimate receiver clock error by code stand-alone
		  codeStandalone(roverObs, true, false);
		}

		int obsReduction = 0;

		if (masterObs != null)
		{
		  // Select satellites for double differences
		  sats.selectDoubleDiff(roverObs, masterObs, masterPos);
		  obsReduction = 1;
		}

		// Number of observations (code and phase)
		int nObs = sats.avail.Count;

		// Double differences with respect to pivot satellite reduce number of observations by 1
		nObs = nObs - obsReduction;

		if (sats.availPhase.Count != 0)
		{
		  // Add number of satellites with phase (minus 1 for double diff)
		  nObs = nObs + sats.availPhase.Count - obsReduction;
		}

		if (sats.avail.Count >= minNumSat)
		{
		  // Allocate transformation matrix
		  H = new Matrix(nObs, o3 + nN);

		  // Allocate observation vector
		  y0 = new Matrix(nObs, 1);

		  // Allocate observation error covariance matrix
		  Cnn = new Matrix(nObs, nObs);

		  // Allocate K and G matrices
		  Matrix K = new Matrix(o3 + nN, o3 + nN);
		  Matrix G = new Matrix(o3 + nN, nObs);

		  // Re-initialization of the model error covariance matrix
		  Cvv = Matrix.Zero(3);

		  // Set variances only if dynamic model is not static
		  if (o1 != 1)
		  {
			// Allocate and build rotation matrix
			Matrix Rt = Coordinates.rotationMatrix(rover);

			// Build 3x3 diagonal matrix with variances
			Matrix diagonal = new Matrix(3, 3);
			diagonal.SetElement(0, 0, Math.Pow(stDevE, 2));
			diagonal.SetElement(1, 1, Math.Pow(stDevN, 2));
			diagonal.SetElement(2, 2, Math.Pow(stDevU, 2));

			// Propagate local variances to global variances
			diagonal = Rt.Transpose().Multiply(diagonal).Multiply(Rt);

			// Set global variances in the model error covariance matrix
			Cvv.SetElement(i1, i1, diagonal.GetElement(0, 0));
			Cvv.SetElement(i1, i2, diagonal.GetElement(0, 1));
			Cvv.SetElement(i1, i3, diagonal.GetElement(0, 2));
			Cvv.SetElement(i2, i1, diagonal.GetElement(1, 0));
			Cvv.SetElement(i2, i2, diagonal.GetElement(1, 1));
			Cvv.SetElement(i2, i3, diagonal.GetElement(1, 2));
			Cvv.SetElement(i3, i1, diagonal.GetElement(2, 0));
			Cvv.SetElement(i3, i2, diagonal.GetElement(2, 1));
			Cvv.SetElement(i3, i3, diagonal.GetElement(2, 2));
		  }

		  // Fill in Kalman filter transformation matrix, observation vector and observation error covariance matrix
		  setup(roverObs, masterObs, masterPos);

		  // Check if satellite configuration changed since the previous epoch
		  checkSatelliteConfiguration(roverObs, masterObs, masterPos);

		  // Identity matrix
		  Matrix I = Matrix.Identity(o3 + nN);

		  // Kalman filter equations
		  K = T.Multiply(Cee).Multiply(T.Transpose()).Plus(Cvv);
		  G = K.Multiply(H.Transpose()).Multiply(H.Multiply(K).Multiply(H.Transpose()).Plus(Cnn).Inverse());

		  // look for outliers
		  if (goGPS.searchForOutliers())
		  {

			  Matrix Xhat_t_t = I.Minus(G.Multiply(H)).Multiply(KFprediction).Plus(G.Multiply(y0));
			  Matrix residuals = compute_residuals(Xhat_t_t);

		//      remove observations with residuals exceeding thresholds
			  int r = 0;
			  for (; r < residuals.NumElements / 2; r++)
			  {
					  if (Math.Abs(residuals.get(r)) > goGPS.CodeResidThreshold)
					  {
						  H.SetRow(r, 0, new double[H.ColumnCount]);
						  y0.setRow(r, 0, new double[y0.ColumnCount]);
						  Cnn.setRow(r, 0, new double[Cnn.ColumnCount]);
					  }
			  }

			  for (; r < residuals.NumElements; r++)
			  {
					if (Math.Abs(residuals.get(r)) > goGPS.PhaseResidThreshold)
					{
					  H.SetMatrix(r, 0, new double[H.ColumnCount]);
					  y0.setRow(r, 0, new double[y0.ColumnCount]);
					  Cnn.setRow(r, 0, new double[Cnn.ColumnCount]);
					}
			  }
		  }

		  KFstate = I.Minus(G.Multiply(H)).Multiply(KFprediction).Plus(G.Multiply(y0));
		  KFprediction = T.Multiply(KFstate);
		  Cee = I.Minus(G.Multiply(H)).Multiply(K);

		}
		else
		{

		  // Positioning only by system dynamics
		  KFstate = KFprediction;
		  KFprediction = T.Multiply(KFstate);
		  Cee = T.Multiply(Cee).Multiply(T.Transpose());
		}

		// Compute predicted phase ranges based on Doppler observations
		computeDopplerPredictedPhase(roverObs, masterObs);

		// Set receiver position
		rover.setXYZ(KFstate.GetElement(0, 0), KFstate.GetElement(i1 + 1, 0), KFstate.GetElement(i2 + 1, 0));

		positionCovariance.SetElement(0, 0, Cee.GetElement(0, 0));
		positionCovariance.SetElement(1, 1, Cee.GetElement(i1 + 1, i1 + 1));
		positionCovariance.SetElement(2, 2, Cee.GetElement(i2 + 1, i2 + 1));
		positionCovariance.SetElement(0, 1, Cee.GetElement(0, i1 + 1));
		positionCovariance.SetElement(0, 2, Cee.GetElement(0, i2 + 1));
		positionCovariance.SetElement(1, 0, Cee.GetElement(i1 + 1, 0));
		positionCovariance.SetElement(1, 2, Cee.GetElement(i1 + 1, i2 + 1));
		positionCovariance.SetElement(2, 0, Cee.GetElement(i2 + 1, 0));
		positionCovariance.SetElement(2, 1, Cee.GetElement(i2 + 1, i1 + 1));

		// Allocate and build rotation matrix
		Matrix R = Coordinates.rotationMatrix(rover);

		// Propagate covariance from global system to local system
		// Covariance matrix obtained from matrix A (satellite geometry) [local coordinates]
		Matrix covENU = R.Multiply(positionCovariance).Multiply(R.Transpose());

		// Kalman filter DOP computation
		rover.kpDop = Math.Sqrt(positionCovariance.GetElement(0, 0) + positionCovariance.GetElement(1, 1) + positionCovariance.GetElement(2, 2));
		rover.khDop = Math.Sqrt(covENU.GetElement(0, 0) + covENU.GetElement(1, 1));
		rover.kvDop = Math.Sqrt(covENU.GetElement(2, 2));

		// Compute positioning in geodetic coordinates
		rover.computeGeodetic();
	  }
	}

}