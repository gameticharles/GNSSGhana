namespace org.gogpsproject
{

	/// <summary>
	/// All the status values that the goGPS object can be in during processing
	/// </summary>
	public enum Status
	{
	  None,
	  Valid,
	  NoAprioriPos,
	  EphNotFound,
	  NotEnoughSats,
	  Low_Sat,
	  MaxCorrection,
	  MaxHDOP,
	  MaxEres,
	  aPosterioriPR,
	  Exception, // Generic error, none of the above
	}
}