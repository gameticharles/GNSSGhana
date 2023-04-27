using System;
using System.Threading;

namespace org.gogpsproject.positioning
{

	using SimpleMatrix = org.ejml.simple.SimpleMatrix;
	using PositionConsumer = org.gogpsproject.consumer.PositionConsumer;
	using DopType = org.gogpsproject.positioning.RoverPosition.DopType;
	using Observations = org.gogpsproject.producer.Observations;
	using ObservationsProducer = org.gogpsproject.producer.ObservationsProducer;

	public class LS_SA_code : Core
	{

	  public LS_SA_code(GoGPS goGPS) : base(goGPS)
	  {
	  }

	  public virtual void codeStandalone(Observations roverObs, bool estimateOnlyClock, bool ignoreTopocentricParameters)
	  {

		// Number of GNSS observations without cutoff
		int nObs = roverObs.NumSat;

		// Number of unknown parameters
		int nUnknowns = 4;

		// Add one unknown for each constellation in addition to the first (to estimate Inter-System Biases - ISBs)
		string sys = sats.AvailGnssSystems;
		if (sys.Length > 0)
		{
		  sys = sys.Substring(1);
		  nUnknowns = nUnknowns + sys.Length;
		}

		// Number of available satellites (i.e. observations)
		int nObsAvail = sats.avail.Count;

		// Least squares design matrix
		SimpleMatrix A = new SimpleMatrix(nObsAvail, nUnknowns);

		// Vector for approximate pseudoranges
		SimpleMatrix b = new SimpleMatrix(nObsAvail, 1);

		// Vector for observed pseudoranges
		SimpleMatrix y0 = new SimpleMatrix(nObsAvail, 1);

		// Cofactor matrix (initialized to identity)
		SimpleMatrix Q = SimpleMatrix.identity(nObsAvail);

		// Solution vector
		SimpleMatrix x = new SimpleMatrix(nUnknowns, 1);

		// Vector for observation error
		SimpleMatrix vEstim = new SimpleMatrix(nObsAvail, 1);

		// Vectors for troposphere and ionosphere corrections
		SimpleMatrix tropoCorr = new SimpleMatrix(nObsAvail, 1);

		SimpleMatrix ionoCorr = new SimpleMatrix(nObsAvail, 1);

		// Set up the least squares matrices
		for (int i = 0, k = 0; i < nObs; i++)
		{

		  // Satellite ID
		  int id = roverObs.getSatID(i);
		  char satType = roverObs.getGnssType(i);

		  string checkAvailGnss = Convert.ToString(satType) + Convert.ToString(id);

		  if (sats.pos[i] != null && sats.gnssAvail.Contains(checkAvailGnss))
		  {
	//      if (sats.pos[i]!=null && sats.avail.contains(id)  && satTypeAvail.contains(satType)) {
	//        System.out.println("####" + checkAvailGnss  + "####");

			// Fill in one row in the design matrix
			A.set(k, 0, rover.diffSat[i].get(0) / rover.satAppRange[i]); // X
			A.set(k, 1, rover.diffSat[i].get(1) / rover.satAppRange[i]); // Y
			A.set(k, 2, rover.diffSat[i].get(2) / rover.satAppRange[i]); // Z
			A.set(k, 3, 1); // clock error
			for (int c = 0; c < sys.Length; c++)
			{
			  A.set(k, 4 + c, sys.IndexOf(satType) == c?1:0); // inter-system bias
			}

			// Add the approximate pseudorange value to b
			b.set(k, 0, rover.satAppRange[i] - sats.pos[i].SatelliteClockError * Constants.SPEED_OF_LIGHT);

			// Add the clock-corrected observed pseudorange value to y0
			y0.set(k, 0, roverObs.getSatByIDType(id, satType).getPseudorange(goGPS.Freq));

			if (!ignoreTopocentricParameters)
			{
			  // Fill in troposphere and ionosphere double differenced
			  // corrections
			  tropoCorr.set(k, 0, rover.satTropoCorr[i]);
			  ionoCorr.set(k, 0, rover.satIonoCorr[i]);

			  // Fill in the cofactor matrix
			  double weight = Q.get(k, k) + computeWeight(rover.topo[i].Elevation, roverObs.getSatByIDType(id, satType).getSignalStrength(goGPS.Freq));
			  Q.set(k, k, weight);
			}

			// Increment available satellites counter
			k++;
		  }

		}

		if (!ignoreTopocentricParameters)
		{
		  // Apply troposphere and ionosphere correction
		  b = b.plus(tropoCorr);
		  b = b.plus(ionoCorr);
		}

		// Least squares solution x = ((A'*Q^-1*A)^-1)*A'*Q^-1*(y0-b);
		x = A.transpose().mult(Q.invert()).mult(A).invert().mult(A.transpose()).mult(Q.invert()).mult(y0.minus(b));

		// Receiver clock error
		rover.clockError = x.get(3) / Constants.SPEED_OF_LIGHT;

		if (estimateOnlyClock)
		{
		  return;
		}

		// Receiver position
		rover.PlusXYZ = x.extractMatrix(0, 3, 0, 1);

		// Estimation of the variance of the observation error
		vEstim = y0.minus(A.mult(x).plus(b));
		double varianceEstim = (vEstim.transpose().mult(Q.invert()).mult(vEstim)).get(0) / (nObsAvail - nUnknowns);

		// Covariance matrix of the estimation error
		if (nObsAvail > nUnknowns)
		{
		  positionCovariance = A.transpose().mult(Q.invert()).mult(A).invert().scale(varianceEstim).extractMatrix(0, 3, 0, 3);
		}
		else
		{
		  positionCovariance = null;
		}

		updateDops(A);

		// Compute positioning in geodetic coordinates
		rover.computeGeodetic();
	  }

	  /// <summary>
	  /// Run code standalone.
	  /// </summary>
	  /// <param name="getNthPosition"> the get nth position </param>
	  /// <returns> the coordinates </returns>
	  /// <exception cref="Exception"> </exception>
	  public static void run(GoGPS goGPS, double stopAtDopThreshold)
	  {

		RoverPosition rover = goGPS.getRoverPos();
		MasterPosition master = goGPS.getMasterPos();
		Satellites sats = goGPS.Sats;
		ObservationsProducer roverIn = goGPS.RoverIn;
		ObservationsProducer masterIn = goGPS.MasterIn;
		bool debug = goGPS.Debug;
		bool validPosition = false;

		LS_SA_code sa = new LS_SA_code(goGPS);

		RoverPosition coord = null;
		try
		{
		  Observations obsR = roverIn.NextObservations;
		  while (obsR != null && !Thread.interrupted()) // buffStreamObs.ready()
		  {
	//        if(debug) System.out.println("OK ");

			//try{
			  // If there are at least four satellites
			  if (obsR.NumSat >= 4) // gps.length
			  {
				if (debug)
				{
					Console.WriteLine("Total number of satellites: " + obsR.NumSat);
				}

				// Compute approximate positioning by iterative least-squares
				if (!rover.ValidXYZ)
				{

					 if (roverIn.DefinedPosition != null)
					 {
					  roverIn.DefinedPosition.cloneInto(rover);
					 }

				  for (int iter = 0; iter < 3; iter++)
				  {
					// Select all satellites
					sats.selectStandalone(obsR, -100);

					if (sats.AvailNumber >= 4)
					{
					  sa.codeStandalone(obsR, false, true);
					}
				  }

				// If an approximate position was computed
				  if (debug)
				  {
					  Console.WriteLine("Valid approximate position? " + rover.ValidXYZ + " " + rover.ToString());
				  }
				}
				if (rover.ValidXYZ)
				{
				  // Select available satellites
				  sats.selectStandalone(obsR);

				  if (sats.AvailNumber >= 4)
				  {
					if (debug)
					{
						Console.WriteLine("Number of selected satellites: " + sats.AvailNumber);
					}
					// Compute code stand-alone positioning (epoch-by-epoch solution)
					sa.codeStandalone(obsR, false, false);
				  }
				  else
				  {
					// Discard approximate positioning
					rover.setXYZ(0, 0, 0);
				  }
				}

				if (debug)
				{
					Console.WriteLine("Valid LS position? " + rover.ValidXYZ + " " + rover.ToString());
				}
				if (rover.ValidXYZ)
				{
				  if (!validPosition)
				  {
					goGPS.notifyPositionConsumerEvent(org.gogpsproject.consumer.PositionConsumer_Fields.EVENT_START_OF_TRACK);
					validPosition = true;
				  }
				  {
	//              else 
					coord = new RoverPosition(rover, DopType.STANDARD, rover.getpDop(), rover.gethDop(), rover.getvDop());

					if (goGPS.PositionConsumers.Count > 0)
					{
					  coord.RefTime = new Time(obsR.RefTime.Msec);
					  coord.obs = obsR;
					  coord.sampleTime = obsR.RefTime;
					  coord.status = rover.status;
					  goGPS.notifyPositionConsumerAddCoordinate(coord);
					}
					if (debug)
					{
						Console.WriteLine("PDOP: " + rover.getpDop());
					}
					if (debug)
					{
						Console.WriteLine("------------------------------------------------------------");
					}
					if (stopAtDopThreshold > 0.0 && rover.getpDop() < stopAtDopThreshold)
					{
					  return;
					}
				  }
				}
			  }
	//        }catch(Exception e){
	//          System.out.println("Could not complete due to "+e);
	//          e.printStackTrace();
	//        }
			obsR = roverIn.NextObservations;
		  }
		}
		catch (Exception e)
		{
		  Console.WriteLine(e.ToString());
		  Console.Write(e.StackTrace);
		  throw e;
		}
		finally
		{
		  goGPS.notifyPositionConsumerEvent(org.gogpsproject.consumer.PositionConsumer_Fields.EVENT_END_OF_TRACK);
		}
	  }

	}

}