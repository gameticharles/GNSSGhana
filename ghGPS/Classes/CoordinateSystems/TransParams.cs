using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ghGPS.Classes
{
    /// <summary>
    /// Transformation Parameters
    /// </summary>
    public class TransParams
    {

        /// <summary>
        /// Init transformation parameters
        /// </summary>
        public TransParams()
        {

        }

        
        /// <summary>
        /// Get 1 by 10 Matrix of the transformation parameters
        /// </summary>
        /// <returns>Double matrix</returns>
        public virtual double[] getValues()
        {
            return new double[] { δx, δy, δz, Rx, Ry, Rz, Scale, Xm, Ym, Zm };
        }

        /// <summary>
        /// Set transformation Parameters 
        /// </summary>
        /// <param name="δx">Delta X (Meter)</param>
        /// <param name="δy">Delta Y (Meter)</param>
        /// <param name="δz">Delta Z (Meter)</param>
        /// <param name="Scale">Scale (ppm)</param>
        /// <param name="Rx">Rotation X (seconds)</param>
        /// <param name="Ry">Rotation Y (seconds)</param>
        /// <param name="Rz">Rotation Z (seconds)</param>
        /// <param name="Xm">Cartesian Center X (Meter)</param>
        /// <param name="Ym">Cartesian Center Y (Meter)</param>
        /// <param name="Zm">Cartesian Center Z (Meter)</param>
        public virtual void setTransParams(double δx, double δy, double δz, double Rx, double Ry, double Rz, double Scale, double Xm, double Ym, double Zm)
        {
            this.δx = δx;
            this.δy = δy;
            this.δz = δz;
            this.Scale = Scale;
            this.Rx = Rx;
            this.Ry = Ry;
            this.Rz = Rz;
            this.Xm = Xm;
            this.Ym = Ym;
            this.Zm = Zm;
        }

        /// <summary>
        /// Set transformation Parameters 
        /// </summary>
        /// <param name="TransParams">Trans Params matrix 1 x 10</param>
        public virtual void setTransParams(double[] TransParams)
        {
            setTransParams(TransParams[0], TransParams[1], TransParams[2], TransParams[3], TransParams[4], TransParams[5], TransParams[6], TransParams[7], TransParams[8], TransParams[9]);
        }

        /// <summary>
        /// Ghana Default 10 transformation parameters
        /// </summary>
        /// <returns></returns>
        public virtual TransParams GH10TransParams()
        {
            setTransParams(-196.557, 33.385, 322.452, 0.0368, -0.00799, -0.0119, -6, 6339239.29, -120750.511, 686012.361);
            
            return this;
        }

        /// <summary>
        /// Ghana Default 7 transformation parameters
        /// </summary>
        /// <returns></returns>
        public virtual TransParams GH7TransParams()
        {
            setTransParams(-158.635, 32.174, 326.783, 0.0368, -0.00799, -0.0119, -7.6, 0, 0, 0);
            //setTransParams(-158.635, 32.174, 326.783, 1.786e-7, -3.872e-8, -5.767e-8, -7.6, 0, 0, 0);

            return this;
        }

        /// <summary>
        /// Ghana Default 3 transformation parameters
        /// </summary>
        /// <returns></returns>
        public virtual TransParams GH3TransParams()
        {
            setTransParams(-196.58, 33.383, 322.552, 0, 0, 0, 0, 0, 0, 0); 

            return this;
        }

        /// <summary>
        /// Delta X (Meter)
        /// </summary>
        public virtual double δx { get; set; }

        /// <summary>
        /// Delta Y (Meter)
        /// </summary>
        public virtual double δy { get; set; }

        /// <summary>
        /// Delta Z (Meter)
        /// </summary>
        public virtual double δz { get; set; }

        /// <summary>
        /// Scale (ppm)
        /// </summary>
        public virtual double Scale { get; set; }

        /// <summary>
        /// Rotation X (seconds)
        /// </summary>
        public virtual double Rx { get; set; }

        /// <summary>
        /// Rotation Y (seconds)
        /// </summary>
        public virtual double Ry { get; set; }

        /// <summary>
        /// Rotation Z (seconds)
        /// </summary>
        public virtual double Rz { get; set; }

        /// <summary>
        /// Cartesian Center X (Meter)
        /// </summary>
        public virtual double Xm { get; set; }

        /// <summary>
        /// Cartesian Center Y (Meter)
        /// </summary>
        public virtual double Ym { get; set; }

        /// <summary>
        /// Cartesian Center Z (Meter)
        /// </summary>
        public virtual double Zm { get; set; }

    }
}
