using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ghGPS.Classes
{
    /// <summary>
    /// Multiple Regression Equation Params
    /// </summary>
    public class MultipleRegressionEquationParams
    {
        /// <summary>
        /// Init MRE parameters
        /// </summary>
        public MultipleRegressionEquationParams()
        {

        }

        /// <summary>
        /// Get 1 by 20 Matrix of the MRE parameters
        /// </summary>
        /// <returns>Double Matrix</returns>
        public virtual double[] GetValues()
        {
            return new double[] { A00, A10, A01, A20, A11, A02, A30, A21, A12, A03, B00, B10, B01, B20, B11, B02, B30, B21, B12, B03};
        }

        /// <summary>
        /// Set MRE parsing matrix containing values
        /// </summary>
        /// <param name="MRE_Params"></param>
        public virtual void SetMREParams(double[] MRE_Params)
        {
            SetMREParams(MRE_Params[0], MRE_Params[1], MRE_Params[2], MRE_Params[3], MRE_Params[4], MRE_Params[5], MRE_Params[6], MRE_Params[7], MRE_Params[8], MRE_Params[9], MRE_Params[10], MRE_Params[11], MRE_Params[12], MRE_Params[13], MRE_Params[14], MRE_Params[15], MRE_Params[16], MRE_Params[17], MRE_Params[18], MRE_Params[19]);
        }

        /// <summary>
        /// Set MRE parsing all individual values
        /// </summary>
        /// <param name="A00"></param>
        /// <param name="A10"></param>
        /// <param name="A01"></param>
        /// <param name="A20"></param>
        /// <param name="A11"></param>
        /// <param name="A02"></param>
        /// <param name="A30"></param>
        /// <param name="A21"></param>
        /// <param name="A12"></param>
        /// <param name="A03"></param>
        /// <param name="B00"></param>
        /// <param name="B10"></param>
        /// <param name="B01"></param>
        /// <param name="B20"></param>
        /// <param name="B11"></param>
        /// <param name="B02"></param>
        /// <param name="B30"></param>
        /// <param name="B21"></param>
        /// <param name="B12"></param>
        /// <param name="B03"></param>
        public virtual void SetMREParams(double A00, double A10, double A01, double A20, double A11, double A02, double A30, double A21, double A12, double A03, double B00, double B10, double B01, double B20, double B11, double B02, double B30, double B21, double B12, double B03)
        {
            this.A00 = A00;
            this.A10 = A10;
            this.A01 = A01;
            this.A20 = A20;
            this.A11 = A11;
            this.A02 = A02;
            this.A30 = A30;
            this.A21 = A21;
            this.A12 = A12;
            this.A03 = A03;

            this.B00 = B00;
            this.B10 = B10;
            this.B01 = B01;
            this.B20 = B20;
            this.B11 = B11;
            this.B02 = B02;
            this.B30 = B30;
            this.B21 = B21;
            this.B12 = B12;
            this.B03 = B03;                        
        }

        /// <summary>
        /// Ghana Default MRE
        /// </summary>
        /// <returns></returns>
        public virtual MultipleRegressionEquationParams GHMREParams()
        {
            SetMREParams(-0.0027329, 0.00048783, 0.000011079, 0.000089722, 0.000032158, 0.0000066774, -0.000068005, 0.000013419, 0.000021219, -0.000033315, -0.00025597, 0.000001395, -0.00060172, 0.0000020015, -0.000029319, -0.0000039382, 0.0000041771, -0.000022493, -0.000033315, 0.000021523);

            return this;
        }


        public virtual double A00 { get; set; }
        public virtual double A10 { get; set; }
        public virtual double A01 { get; set; }
        public virtual double A20 { get; set; }
        public virtual double A11 { get; set; }
        public virtual double A02 { get; set; }
        public virtual double A30 { get; set; }
        public virtual double A21 { get; set; }
        public virtual double A12 { get; set; }
        public virtual double A03 { get; set; }

        public virtual double B00 { get; set; }
        public virtual double B10 { get; set; }
        public virtual double B01 { get; set; }
        public virtual double B20 { get; set; }
        public virtual double B11 { get; set; }
        public virtual double B02 { get; set; }
        public virtual double B30 { get; set; }
        public virtual double B21 { get; set; }
        public virtual double B12 { get; set; }
        public virtual double B03 { get; set; }

    }
}
