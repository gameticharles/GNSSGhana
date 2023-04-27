//----------------------------------------------------------------------------------------
//	Copyright © 2003 - 2014 Tangible Software Solutions Inc.
//	This class can be used by anyone provided that the copyright notice remains intact.
//
//	This class simulates the behavior of the classic VB 'Val' function.
//----------------------------------------------------------------------------------------
public static partial class Simulate
{
	public static double Val(string expression)
	{
		if (expression == null)
			return 0;

		//try the entire string, then progressively smaller
		//substrings to simulate the behavior of VB's 'Val',
		//which ignores trailing characters after a recognizable value:
		for (int size = expression.Length; size > 0; size--)
		{
			double testDouble;
			if (double.TryParse(expression.Substring(0, size), out testDouble))
				return testDouble;
		}

		//no value is recognized, so return 0:
		return 0;
	}
	public static double Val(object expression)
	{
		if (expression == null)
			return 0;

		double testDouble;
		if (double.TryParse(expression.ToString(), out testDouble))
			return testDouble;

		//VB's 'Val' function returns -1 for 'true':
		bool testBool;
		if (bool.TryParse(expression.ToString(), out testBool))
			return testBool ? -1 : 0;

		//VB's 'Val' function returns the day of the month for dates:
		System.DateTime testDate;
		if (System.DateTime.TryParse(expression.ToString(), out testDate))
			return testDate.Day;

		//no value is recognized, so return 0:
		return 0;
	}
	public static int Val(char expression)
	{
		int testInt;
		if (int.TryParse(expression.ToString(), out testInt))
			return testInt;
		else
			return 0;
	}
}