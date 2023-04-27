using MatrixLib.Matricies;

namespace org.gogpsproject.positioning
{
    
	public abstract class ReceiverPosition : Coordinates
	{

	  // Fields related to receiver-satellite geometry

	  /// <summary>
	  /// receiver-satellite approximate range </summary>
	  internal double[] satAppRange;

	  /// <summary>
	  /// receiver-satellite troposphere correction </summary>
	  internal double[] satTropoCorr;

	  /// <summary>
	  /// receiver-satellite ionosphere correction </summary>
	  internal double[] satIonoCorr;

	  /// <summary>
	  /// receiver-satellite vector </summary>
	  internal Matrix[] diffSat;

	  // Fields for satellite selection
	  internal TopocentricCoordinates[] topo;

	  // Fields for storing values from previous epoch
	  /// <summary>
	  /// rover L Carrier Phase predicted from previous epoch (based on Doppler) [cycle] </summary>
	  internal double[] dopplerPredPhase;

	  /// <returns> the rover Doppler predicted phase </returns>
	  public virtual double getDopplerPredictedPhase(int satID)
	  {
		return dopplerPredPhase[satID - 1];
	  }

	  /// <param name="roverDopplerPredictedPhase"> the Doppler predicted phase to set </param>
	  public virtual void setDopplerPredictedPhase(int satID, double roverDopplerPredictedPhase)
	  {
		dopplerPredPhase[satID - 1] = roverDopplerPredictedPhase;
	  }

	}

}