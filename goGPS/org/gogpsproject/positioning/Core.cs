
using ReinGametiMatrixLib.Matricies;
using System;

namespace org.gogpsproject.positioning
{

	using WeightingStrategy = org.gogpsproject.GoGPS.WeightingStrategy;

	//import com.google.maps.ElevationApi;
	//import com.google.maps.GeoApiContext;
	//import com.google.maps.model.ElevationResult;
	//import com.google.maps.model.LatLng;

	public abstract class Core
	{

	    internal GoGPS goGPS;
	    internal RoverPosition rover;
	    internal MasterPosition master;
	    internal Satellites sats;

	    /// <summary>
	    /// Covariance matrix of the position estimation error </summary>
	    internal Matrix positionCovariance;

	    //static GeoApiContext context;
	    //static GeoApiContext getContext(){
	    //  if( context == null )
	    //    context = new GeoApiContext().setApiKey("...Add your key here");
	    //  return context;
	    //}
	    public Core(GoGPS goGPS)
	    {
		this.goGPS = goGPS;
		this.rover = goGPS.getRoverPos();
		this.master = goGPS.getMasterPos();
		this.sats = goGPS.Sats;
	    }

	    /// <param name="elevation"> </param>
	    /// <param name="snr"> </param>
	    /// <returns> weight computed according to the variable "goGPS.weights" </returns>
	    internal virtual double computeWeight(double elevation, float snr)
	    {

		double weight = 1;
		float Sa = Constants.SNR_a;
		float SA = Constants.SNR_A;
		float S0 = Constants.SNR_0;
		float S1 = Constants.SNR_1;

		if (float.IsNaN(snr) && (goGPS.Weights == GoGPS.WeightingStrategy.SIGNAL_TO_NOISE_RATIO || goGPS.Weights == GoGPS.WeightingStrategy.COMBINED_ELEVATION_SNR))
		{
		    if (goGPS.Debug)
		    {
			    Console.WriteLine("SNR not available: forcing satellite elevation-based weights...");
		    }
		    goGPS.Weights = GoGPS.WeightingStrategy.SAT_ELEVATION;
		}

		switch (goGPS.Weights)
		{

		    // Weight based on satellite elevation
		    case WeightingStrategy.SAT_ELEVATION:
			    weight = 1 / Math.Pow(Math.Sin(elevation * Math.PI / 180), 2);
			    break;

		    // Weight based on signal-to-noise ratio
		    case WeightingStrategy.SIGNAL_TO_NOISE_RATIO:
			    if (snr >= S1)
			    {
			        weight = 1;
			    }
			    else
			    {
			        weight = Math.Pow(10, -(snr - S1) / Sa) * ((SA / Math.Pow(10, -(S0 - S1) / Sa) - 1) / (S0 - S1) * (snr - S1) + 1);
			    }
			    break;

		    // Weight based on combined elevation and signal-to-noise ratio
		    case WeightingStrategy.COMBINED_ELEVATION_SNR:
			    if (snr >= S1)
			    {
			        weight = 1;
			    }
			    else
			    {
			        double weightEl = 1 / Math.Pow(Math.Sin(elevation * Math.PI / 180), 2);
			        double weightSnr = Math.Pow(10, -(snr - S1) / Sa) * ((SA / Math.Pow(10, -(S0 - S1) / Sa) - 1) / (S0 - S1) * (snr - S1) + 1);
			        weight = weightEl * weightSnr;
			    }
			    break;

		    // Same weight for all observations or default
		    case WeightingStrategy.EQUAL:
		    default:
			    weight = 1;
		        break;
		}

		return weight;
	    }

	  internal virtual void updateDops(Matrix A)
	  {
		// Compute covariance matrix from A matrix [ECEF reference system]
		Matrix covXYZ = A.Transpose().Multiply(A).Inverse().GetMatrix(0, 3, 0, 3);

		// Allocate and build rotation matrix
		Matrix R = Coordinates.rotationMatrix(rover);

		/// <summary>
		/// Covariance matrix obtained from matrix A (satellite geometry) [local coordinates] </summary>
		// Propagate covariance from global system to local system
		Matrix covENU = R.Multiply(covXYZ).Multiply(R.Transpose());

		 //Compute DOP values
		rover.pDop = Math.Sqrt(covXYZ.GetElement(0, 0) + covXYZ.GetElement(1, 1) + covXYZ.GetElement(2, 2));
		rover.hDop = Math.Sqrt(covENU.GetElement(0, 0) + covENU.GetElement(1, 1));
		rover.vDop = Math.Sqrt(covENU.GetElement(2, 2));
	  }
	}

}