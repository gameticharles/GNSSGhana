using System;
using java.io;

/*
 * Copyright (c) 2010, Eugenio Realini, Mirko Reguzzoni, Cryms sagl - Switzerland. All Rights Reserved.
 *
 * This file is part of goGPS Project (goGPS).
 *
 * goGPS is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as
 * published by the Free Software Foundation, either version 3
 * of the License, or (at your option) any later version.
 *
 * goGPS is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with goGPS.  If not, see <http://www.gnu.org/licenses/>.
 *
 *
 */
namespace org.gogpsproject.positioning
{
    using org.gogpsproject.producer;
    using ReinGametiMatrixLib.Matricies;
    using System.IO;
    using Streamable = org.gogpsproject.producer.Streamable;

    /// <summary>
    /// <para>
    /// Coordinate and reference system tools
    /// </para>
    /// 
    /// @author Eugenio Realini, Cryms.com
    /// </summary>
    public class Coordinates : Streamable
	{
		private const int STREAM_V = 1;

		// Global systems
		private Matrix ecef = null; // Earth-Centered, Earth-Fixed (X, Y, Z)
		private Matrix geod = null; // Longitude (lam), latitude (phi), height (h)

		// Local systems (require to specify an origin)
		private Matrix enu; // Local coordinates (East, North, Up)

		private Time refTime = null;

		protected internal Coordinates()
		{
			ecef = new Matrix(3, 1);
			geod = new Matrix(3, 1);
			enu = new Matrix(3, 1);
		}

        public static double toRadian(this double degree)
        {
            return degree * Math.PI / 180;
        }

        public static double toDegree(this double radian)
        {
            return radian * 180 / Math.PI;
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public static Coordinates readFromStream(java.io.DataInputStream dai, boolean oldVersion) throws java.io.IOException
        public static Coordinates readFromStream(ObjectInputStream dai, bool oldVersion)
		{
			Coordinates c = new Coordinates();
			c.read(dai, oldVersion);
			return c;
		}

		public static Coordinates globalXYZInstance(double x, double y, double z)
		{
			Coordinates c = new Coordinates();
			//c.ecef = new SimpleMatrix(3, 1);
			c.setXYZ(x, y, z);
			return c;
		}
	//	public static Coordinates globalXYZInstance(SimpleMatrix ecef){
	//		Coordinates c = new Coordinates();
	//		c.ecef = ecef.copy();
	//		return c;
	//	}
		public static Coordinates globalENUInstance(Matrix ecef)
		{
			Coordinates c = new Coordinates();
			c.enu = ecef.Copy();
			return c;
		}

		public static Coordinates globalGeodInstance(double lat, double lon, double alt)
		{
			Coordinates c = new Coordinates();
			//c.ecef = new SimpleMatrix(3, 1);
			c.setGeod(lat, lon, alt);
			c.computeECEF();

			if (!c.ValidXYZ)
			{
				throw new Exception("Invalid ECEF: " + c);
			}
			return c;
		}


		public virtual Matrix minusXYZ(Coordinates coord)
		{
			return this.ecef.Minus(coord.ecef);
		}
		/// 
		public virtual void computeGeodetic()
		{
			double X = this.ecef.GetElement(0,0);
			double Y = this.ecef.GetElement(1,0);
			double Z = this.ecef.GetElement(2,0);

			//this.geod = new SimpleMatrix(3, 1);

			double a = Constants.WGS84_SEMI_MAJOR_AXIS;
			double e = Constants.WGS84_ECCENTRICITY;

			// Radius computation
			double r = Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2) + Math.Pow(Z, 2));

			// Geocentric longitude
			double lamGeoc = Math.Atan2(Y, X);

			// Geocentric latitude
			double phiGeoc = Math.Atan(Z / Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2)));

			// Computation of geodetic coordinates
			double psi = Math.Atan(Math.Tan(phiGeoc) / Math.Sqrt(1 - Math.Pow(e, 2)));
			double phiGeod = Math.Atan((r * Math.Sin(phiGeoc) + Math.Pow(e, 2) * a / Math.Sqrt(1 - Math.Pow(e, 2)) * Math.Pow(Math.Sin(psi), 3)) / (r * Math.Cos(phiGeoc) - Math.Pow(e, 2) * a * Math.Pow(Math.Cos(psi), 3)));
			double lamGeod = lamGeoc;
			double N = a / Math.Sqrt(1 - Math.Pow(e, 2) * Math.Pow(Math.Sin(phiGeod), 2));
			double h = r * Math.Cos(phiGeoc) / Math.Cos(phiGeod) - N;

			this.geod.SetElement(0, 0, toDegree(lamGeod));
			this.geod.SetElement(1, 0, toDegree(phiGeod));
			this.geod.SetElement(2, 0, h);
		}

		/*
		 function [X,Y,Z] = frgeod( a, finv, dphi, dlambda, h )
		     %FRGEOD  Subroutine to calculate Cartesian coordinates X,Y,Z
		     %       given geodetic coordinates latitude, longitude (east),
		     %       and height above reference ellipsoid along with
		     %       reference ellipsoid values semi-major axis (a) and
		     %       the inverse of flattening (finv)
	
		     % The units of linear parameters h,a must agree (m,km,mi,..etc).
		     % The input units of angular quantities must be in decimal degrees.
		     % The output units of X,Y,Z will be the same as the units of h and a.
		     % Copyright (C) 1987 C. Goad, Columbus, Ohio
		     % Reprinted with permission of author, 1996
		     % Original Fortran code rewritten into MATLAB
		     % Kai Borre 03-03-96
		 */
		public virtual void computeECEF()
		{
			const long a = 6378137;
			const double finv = 298.257223563d;

			double dphi = this.geod.GetElement(1,0);
			double dlambda = this.geod.GetElement(0,0);
			double h = this.geod.GetElement(2,0);

			// compute degree-to-radian factor
			double dtr = Math.PI / 180;

			// compute square of eccentricity
			double esq = (2 - 1 / finv) / finv;
			double sinphi = Math.Sin(dphi * dtr);
			// compute radius of curvature in prime vertical
			double N_phi = a / Math.Sqrt(1 - esq * sinphi * sinphi);

			// compute P and Z
			// P is distance from Z axis
			double P = (N_phi + h) * Math.Cos(dphi * dtr);
			double Z = (N_phi * (1 - esq) + h) * sinphi;
			double X = P * Math.Cos(dlambda * dtr);
			double Y = P * Math.Sin(dlambda * dtr);

			this.ecef.SetElement(0, 0, X);
			this.ecef.SetElement(1, 0, Y);
			this.ecef.SetElement(2, 0, Z);
		}

		/// <param name="origin"> </param>
		/// <returns> Local (ENU) coordinates </returns>
		public virtual void computeLocal(Coordinates target)
		{
			if (this.geod == null)
			{
				computeGeodetic();
			}

			Matrix R = rotationMatrix(this);

			enu = R.Multiply(target.minusXYZ(this));

		}

		public virtual double GeodeticLongitude
		{
			get
			{
				if (this.geod == null)
				{
					computeGeodetic();
				}
				return this.geod.GetElement(0, 0);
			}
		}
		public virtual double GeodeticLatitude
		{
			get
			{
				if (this.geod == null)
				{
					computeGeodetic();
				}
				return this.geod.GetElement(1, 0);
			}
		}
		public virtual double GeodeticHeight
		{
			get
			{
				if (this.geod == null)
				{
					computeGeodetic();
				}
				return this.geod.GetElement(2, 0);
			}
		}
		public virtual double X
		{
			get
			{
				return ecef.GetElement(0, 0);
			}
		}
		public virtual double Y
		{
			get
			{
				return ecef.GetElement(1, 0);
			}
		}
		public virtual double Z
		{
			get
			{
				return ecef.GetElement(2, 0);
			}
		}

		public virtual void setENU(double e, double n, double u)
		{
			this.enu.SetElement(0, 0, e);
			this.enu.SetElement(1, 0, n);
			this.enu.SetElement(2, 0, u);
		}
		public virtual double E
		{
			get
			{
				return enu.GetElement(0, 0);
			}
		}
		public virtual double N
		{
			get
			{
				return enu.GetElement(1, 0);
			}
		}
		public virtual double U
		{
			get
			{
				return enu.GetElement(2, 0);
			}
		}


		public virtual void setXYZ(double x, double y, double z)
		{
			//if(this.ecef==null) this.ecef = new SimpleMatrix(3, 1);
			this.ecef.SetElement(0, 0, x);
			this.ecef.SetElement(1, 0, y);
			this.ecef.SetElement(2, 0, z);
		}
		public virtual void setGeod(double lat, double lon, double alt)
		{
			//if(this.ecef==null) this.ecef = new SimpleMatrix(3, 1);
			this.geod.SetElement(1, 0, lat);
			this.geod.SetElement(0, 0, lon);
			this.geod.SetElement(2, 0, alt);
		}
		public virtual Matrix PlusXYZ
		{
			set
			{
				this.ecef = ecef.Plus(value);
			}
		}
		public virtual Matrix SMMultXYZ
		{
			set
			{
				this.ecef = value.Multiply(this.ecef);
			}
		}

		public virtual bool ValidXYZ
		{
			get
			{
				return (this.ecef != null && this.ecef.ElementSum() != 0 && !double.IsNaN(this.ecef.GetElement(0, 0)) && !double.IsNaN(this.ecef.GetElement(1, 0)) && !double.IsNaN(this.ecef.GetElement(2, 0)) && !double.IsInfinity(this.ecef.GetElement(0, 0)) && !double.IsInfinity(this.ecef.GetElement(1, 0)) && !double.IsInfinity(this.ecef.GetElement(2, 0)) && (ecef.GetElement(0, 0) != 0 && ecef.GetElement(1, 0) != 0 && ecef.GetElement(2, 0) != 0));
			}
		}

		public virtual object clone()
		{
			Coordinates c = new Coordinates();
			cloneInto(c);
			return c;
		}

		public virtual void cloneInto(Coordinates c)
		{
			c.ecef = this.ecef.Copy();
			c.enu = this.enu.Copy();
			c.geod = this.geod.Copy();

			if (refTime != null)
			{
				c.refTime = (Time)refTime.clone();
			}
		}
		/// <param name="origin"> </param>
		/// <returns> Rotation matrix used to switch from global to local reference systems (and vice-versa) </returns>
		public static Matrix rotationMatrix(Coordinates origin)
		{

			double lam = toRadian(origin.GeodeticLongitude);
			double phi = toRadian(origin.GeodeticLatitude);

			double cosLam = Math.Cos(lam);
			double cosPhi = Math.Cos(phi);
			double sinLam = Math.Sin(lam);
			double sinPhi = Math.Sin(phi);

//JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
//ORIGINAL LINE: double[][] data = new double[3][3];
			double[][] data = RectangularArrays.ReturnRectangularDoubleArray(3, 3);
			data[0][0] = -sinLam;
			data[0][1] = cosLam;
			data[0][2] = 0;
			data[1][0] = -sinPhi * cosLam;
			data[1][1] = -sinPhi * sinLam;
			data[1][2] = cosPhi;
			data[2][0] = cosPhi * cosLam;
			data[2][1] = cosPhi * sinLam;
			data[2][2] = sinPhi;

			Matrix R = new Matrix(data);

			return R;
		}

		/// <returns> the refTime </returns>
		public virtual Time RefTime
		{
			get
			{
				return refTime;
			}
			set
			{
				this.refTime = value;
			}
		}


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public int write(java.io.DataOutputStream dos) throws java.io.IOException
		public virtual int write(ObjectOutputStream dos)
		{
			int size = 0;
			dos.writeUTF(org.gogpsproject.producer.Streamable_Fields.MESSAGE_COORDINATES); // 5
			size += 5;
			dos.writeInt(STREAM_V); // 4
			size += 4;

			dos.writeLong(refTime == null? - 1:refTime.Msec); // 8
			size += 8;

			for (int i = 0;i < 3;i++)
			{
				dos.writeDouble(ecef.GetElement(i, 0));
				size += 8;
			}
			for (int i = 0;i < 3;i++)
			{
				dos.writeDouble(enu.GetElement(i, 0));
				size += 8;
			}
			for (int i = 0;i < 3;i++)
			{
				dos.writeDouble(geod.GetElement(i, 0));
				size += 8;
			}

			return size;
		}

		/* (non-Javadoc)
		 * @see org.gogpsproject.Streamable#read(java.io.DataInputStream)
		 */
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void read(java.io.DataInputStream dai, boolean oldVersion) throws java.io.IOException
		public virtual void read(ObjectInputStream dai, bool oldVersion)
		{
			int v = dai.readInt();

			if (v == 1)
			{
				long l = dai.readLong();
				refTime = l == -1?null:new Time(l);
				for (int i = 0;i < 3;i++)
				{
					ecef.SetElement(i, 0, dai.readDouble());
				}
				for (int i = 0;i < 3;i++)
				{
					enu.SetElement(i, 0, dai.readDouble());
				}
				for (int i = 0;i < 3;i++)
				{
					geod.SetElement(i, 0, dai.readDouble());
				}
			}
			else
			{
				throw new IOException("Unknown format version:" + v);
			}

		}

		public override string ToString()
		{
			string lineBreak = java.lang.System.getProperty("line.separator");

			string @out = string.Format("Coord ECEF: X:" + X + " Y:" + Y + " Z:" + Z + lineBreak + "       ENU: E:" + E + " N:" + N + " U:" + U + lineBreak + "      GEOD: Lon:" + GeodeticLongitude + " Lat:" + GeodeticLatitude + " H:" + GeodeticHeight + lineBreak + "      http://maps.google.com?q={0,3:F4},{1,3:F4}" + lineBreak, GeodeticLatitude, GeodeticLongitude);
			return @out;
		}

        int Streamable.write(ObjectInputStream dos)
        {
            throw new NotImplementedException();
        }

        void Streamable.read(ObjectInputStream dai, bool oldVersion)
        {
            throw new NotImplementedException();
        }
    }

}