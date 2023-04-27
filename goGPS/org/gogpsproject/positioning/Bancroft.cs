
using ReinGametiMatrixLib.Matricies;
using System;

namespace org.gogpsproject.positioning
{


	using Observations = org.gogpsproject.producer.Observations;

	public class Bancroft : Core
	{

	  public Bancroft(GoGPS goGPS) : base(goGPS)
	  {
	  }

	  /// <param name="x"> </param>
	  /// <param name="y"> </param>
	  /// <returns> Lorentz inner product </returns>
	  internal static double lorentzInnerProduct(Matrix x, Matrix y)
	  {

		double prod = x.GetElement(0,0) * y.GetElement(0,0) + x.GetElement(1,0) * y.GetElement(1,0) + x.GetElement(2,0) * y.GetElement(2,0) - x.GetElement(3,0) * y.GetElement(3,0);

		return prod;
	  }

	  public virtual void bancroft(Observations obs)
	  {

		//roverPos.coord = null;
		//roverPos.coord = Coordinates.globalXYZInstance(0.0, 0.0, 0.0);

		double travelTime = 0;
		double angle;
		double a, b, c;
		double root;
		double[] r, omc;
		double cdt, calc;
		double rho;

		// Define matrices
		Matrix Binit;
		Matrix B;
            Matrix BBB;
            Matrix BBBe;
            Matrix BBBalpha;
            Matrix e;
            Matrix alpha;
            Matrix possiblePosA;
            Matrix possiblePosB;

		// Allocate vectors
		r = new double[2];
		omc = new double[2];

		// Number of GPS observations
		int nObs = obs.NumSat;

		// Allocate an array to store GPS satellite positions
		sats.pos = new SatellitePosition[nObs];

		// Allocate a 2D array to store Bancroft matrix data
		double[][] dataB = RectangularArrays.ReturnRectangularDoubleArray(nObs, 4);

		int p = 0;
		int id = 0;

		for (int i = 0; i < nObs; i++)
		{

		  id = (int)obs.getSatID(i);
		  char satType = obs.getGnssType(i);

		  // Create new satellite position object
		  //sats.pos[i] = new SatellitePosition(obs.getRefTime().getGpsTime(), obs.getGpsSatID(i), obs.getGpsByID(id).getPseudorange(goGPS.getFreq()));

		  // Compute clock-corrected satellite position
		  //sats.pos[i].computePositionGps(goGPS.getNavigation());

		  double obsPseudorange = obs.getSatByIDType(id, satType).getPseudorange(goGPS.Freq);
		  sats.pos[i] = goGPS.Navigation.getGpsSatPosition(obs, id, satType, rover.ClockError);

		  try
		  {
	//      System.out.println("SatPos "+obs.getGpsSatID(i)+" x:"+sats.pos[i].getX()+" y:"+sats.pos[i].getY()+" z:"+sats.pos[i].getZ());
			// Store Bancroft matrix data (X, Y, Z and clock-corrected
			// range)
			if (sats.pos[i] != null)
			{
			  dataB[p][0] = sats.pos[i].X;
			  dataB[p][1] = sats.pos[i].Y;
			  dataB[p][2] = sats.pos[i].Z;
			  dataB[p][3] = obsPseudorange + Constants.SPEED_OF_LIGHT * sats.pos[i].SatelliteClockError;
			  p++;
			}
			else
			{
			  if (goGPS.Debug)
			  {
				  Console.WriteLine("Error: satellite positions not computed for satID:" + obs.getSatID(i));
			  }
			}
		  }
		  catch (System.NullReferenceException u)
		  {
			Console.WriteLine(u.ToString());
			Console.Write(u.StackTrace);
			if (goGPS.Debug)
			{
				Console.WriteLine("Error: satellite positions not computed for satID:" + obs.getSatID(i));
			}
			//return; // don't break eggs so quickly :-)
		  }

	//      }else{
	//        
	//        p++;
	//      }
		}

		if (p < 4)
		{
			return;
		}
		if (dataB.Length != p)
		{
//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] dataB1 = new double[p][4];
		  double[][] dataB1 = RectangularArrays.ReturnRectangularDoubleArray(p, 4);
		  for (int i = 0;i < p;i++)
		  {
			dataB1[i] = dataB[i];
		  }
		  dataB = dataB1;
		}
		// Allocate matrices
		BBB = new Matrix(4, dataB.Length);
		BBBe = new Matrix(4, 1);
		BBBalpha = new Matrix(4, 1);
		e = new Matrix(dataB.Length, 1);
		alpha = new Matrix(dataB.Length, 1);
		possiblePosA = new Matrix(4, 1);
		possiblePosB = new Matrix(4, 1);

		// Allocate initial B matrix
		Binit = new Matrix(dataB);

		// Make two iterations
		for (int iter = 0; iter < 2; iter++)
		{

		  // Allocate B matrix
		  B = new Matrix(Binit);

		  for (int i = 0; i < dataB.Length; i++)
		  {

			double x = B.GetElement(i, 0);
			double y = B.GetElement(i, 1);

			if (iter == 0)
			{
			  travelTime = Constants.GPS_APPROX_TRAVEL_TIME;
			}
			else
			{
			  double z = B.GetElement(i, 2);
			  rho = Math.Pow((x - rover.X), 2) + Math.Pow((y - rover.Y), 2) + Math.Pow((z - rover.Z), 2);
			  travelTime = Math.Sqrt(rho) / Constants.SPEED_OF_LIGHT;
			}
			angle = travelTime * Constants.EARTH_ANGULAR_VELOCITY;
			B.SetElement(i, 0, Math.Cos(angle) * x + Math.Sin(angle) * y);
			B.SetElement(i, 1, -Math.Sin(angle) * x + Math.Cos(angle) * y);
		  }

		  if (dataB.Length > 4)
		  {
			BBB = B.Transpose().Multiply(B).Solve(B.Transpose());
		  }
		  else
		  {
			BBB = B.Inverse();
		  }

		  e.SetElement(1);
		  for (int i = 0; i < dataB.Length; i++)
		  {

			alpha.SetElement(i, 0, lorentzInnerProduct(B.GetMatrix(i, i + 1, 0, 4), B.GetMatrix(i, i + 1, 0, 4)) / 2);
		  }
          
		  BBBe = BBB.Multiply(e);
		  BBBalpha = BBB.Multiply(alpha);
		  a = lorentzInnerProduct(BBBe, BBBe);
		  b = lorentzInnerProduct(BBBe, BBBalpha) - 1;
		  c = lorentzInnerProduct(BBBalpha, BBBalpha);
		  root = Math.Sqrt(b * b - a * c);
		  r[0] = (-b - root) / a;
		  r[1] = (-b + root) / a;
		  possiblePosA = BBBalpha.Plus(r[0], BBBe);
		  possiblePosB = BBBalpha.Plus(r[1], BBBe);
		  possiblePosA.SetElement(3, 0, -possiblePosA.GetElement(3, 0));
		  possiblePosB.SetElement(3, 0, -possiblePosB.GetElement(3, 0));
		  for (int i = 0; i < dataB.Length; i++)
		  {
			cdt = possiblePosA.GetElement(3, 0);
			calc = B.GetMatrix(i, i + 1, 0, 3).Transpose().Minus(possiblePosA.GetMatrix(0, 3, 0, 1)).NormF() + cdt;
			omc[0] = B.GetElement(i, 3) - calc;
			cdt = possiblePosB.GetElement(3, 0);
			calc = B.GetMatrix(i, i + 1, 0, 3).Transpose().Minus(possiblePosB.GetMatrix(0, 3, 0, 1)).NormF() + cdt;
			omc[1] = B.GetElement(i, 3) - calc;
		  }

		  // Discrimination between roots (choose one of the possible
		  // positions)
		  if (Math.Abs(omc[0]) > Math.Abs(omc[1]))
		  {
			//roverPos.coord.ecef = possiblePosB.extractMatrix(0, 3, 0, 1); // new SimpleMatrix(
			Matrix sm = possiblePosB.GetMatrix(0, 3, 0, 1);
			rover.setXYZ(sm.GetElement(0, 0),sm.GetElement(1, 0),sm.GetElement(2, 0));
			// Clock offset
			rover.clockError = possiblePosB.GetElement(3, 0) / Constants.SPEED_OF_LIGHT;
		  }
		  else
		  {
			//roverPos.coord.ecef = possiblePosA.extractMatrix(0, 3, 0, 1); // new SimpleMatrix(
			Matrix sm = possiblePosA.GetMatrix(0, 3, 0, 1);
			rover.setXYZ(sm.GetElement(0,0),sm.GetElement(1, 0),sm.GetElement(2, 0));
			// Clock offset
			rover.clockError = possiblePosA.GetElement(3, 0) / Constants.SPEED_OF_LIGHT;
		  }
		}
	//    System.out.println("## x: " + roverPos.getX() );
	//    System.out.println("## y: " + roverPos.getY() );
	//    System.out.println("## z: " + roverPos.getZ() );

		// Compute Bancroft's positioning in geodetic coordinates
		rover.computeGeodetic();
	  }
	}

}