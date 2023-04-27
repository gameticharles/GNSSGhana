// Copyright 2006 - Morten Nielsen (www.iter.dk)
//
// This file is part of ProjNet.
// ProjNet is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// ProjNet is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with ProjNet; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;
using System.Collections.Generic;

namespace ghGPS.Classes.CoordinateSystems.Transformations
{
	/// <summary>
	/// Transformation for applying 
	/// </summary>
	internal class DatumTransform : MathTransform
	{
		protected IMathTransform _inverse;
		private Wgs84ConversionInfo _ToWgs94;
		double[] trasPara;

		private bool _isInverse = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatumTransform"/> class.
        /// </summary>
        /// <param name="towgs84"></param>
        public DatumTransform(Wgs84ConversionInfo towgs84) : this(towgs84,false)
		{
		}

		private DatumTransform(Wgs84ConversionInfo towgs84, bool isInverse)
		{
			_ToWgs94 = towgs84;
            trasPara = _ToWgs94.GetTransformParams();
			_isInverse = isInverse;
            
        }
        /// <summary>
        /// Gets a Well-Known text representation of this object.
        /// </summary>
        /// <value></value>
		public override string WKT
		{
			get { throw new NotImplementedException(); }
		}

        /// <summary>
        /// Gets an XML representation of this object.
        /// </summary>
        /// <value></value>
		public override string XML
		{
			get { throw new NotImplementedException(); }
		}

        /// <summary>
        /// Creates the inverse transform of this object.
        /// </summary>
        /// <returns></returns>
        /// <remarks>This method may fail if the transform is not one to one. However, all cartographic projections should succeed.</remarks>
		public override IMathTransform Inverse()
		{
			if (_inverse == null)
				_inverse = new DatumTransform(_ToWgs94,!_isInverse);
			return _inverse;
		}

        private double[] Apply(double[] XYZ, bool IsToWGS84)
		{
            var X = XYZ[0];
            var Y = XYZ[1];
            var Z = XYZ[2];

            var δx = trasPara[0];
            var δy = trasPara[1];
            var δz = trasPara[2];
            var Rx = ToRadians(trasPara[3] / 3600);
            var Ry = ToRadians(trasPara[4] / 3600);
            var Rz = ToRadians(trasPara[5] / 3600);
            var scale = (trasPara[6] / 1000000);
            var Xm = trasPara[7];
            var Ym = trasPara[8];
            var Zm = trasPara[9];

            if (IsToWGS84)
            {
                δx *= -1;
                δy *= -1;
                δz *= -1;
                scale *= -1;
                Rx *= -1;
                Ry *= -1;
                Rz *= -1;
                Xm *= -1;
                Ym *= -1;
                Zm *= -1;
            }

            var X1 = X + (scale * (X - Xm)) + (Rz * (Y - Ym)) - (Ry * (Z - Zm)) + δx;
            var Y1 = Y - (Rz * (X - Xm)) + (scale * (Y - Ym)) + (Rx * (Z - Zm)) + δy;
            var Z1 = Z + (Ry * (X - Xm)) - (Rx * (Y - Ym)) + (scale * (Z - Zm)) + δz;
            
            return new double[] { X1, Y1, Z1 };			
		}

        private double[] ApplyInverted(double[] p)
		{
            return new double[] {};
		}

        /// <summary>
        /// Transforms a coordinate point. The passed parameter point should not be modified.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public override double[] Transform(double[] point)
		{            
            return Apply(point, _isInverse);
		}

        /// <summary>
        /// Transforms a list of coordinate point ordinal values.
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        /// <remarks>
        /// This method is provided for efficiently transforming many points. The supplied array
        /// of ordinal values will contain packed ordinal values. For example, if the source
        /// dimension is 3, then the ordinals will be packed in this order (x0,y0,z0,x1,y1,z1 ...).
        /// The size of the passed array must be an integer multiple of DimSource. The returned
        /// ordinal values are packed in a similar way. In some DCPs. the ordinals may be
        /// transformed in-place, and the returned array may be the same as the passed array.
        /// So any client code should not attempt to reuse the passed ordinal values (although
        /// they can certainly reuse the passed array). If there is any problem then the server
        /// implementation will throw an exception. If this happens then the client should not
        /// make any assumptions about the state of the ordinal values.
        /// </remarks>
        public override List<double[]> TransformList(List<double[]> points)
		{
            List<double[]> pnts = new List<double[]>(points.Count);
            foreach (double[] p in points)
				pnts.Add(Transform(p));
			return pnts;
		}

        /// <summary>
        /// Reverses the transformation
        /// </summary>
		public override void Invert()
		{
			_isInverse = !_isInverse;
		}
	}
}
