﻿#region Header Comments
/*****************************************************************************/
/* MatrixBase.cs                                                             */
/* -------------                                                             */
/* 27 July 2008 - Bradley Ward (entropyau@gmail.com)                         */
/*                Initial coding completed                                   */
/*                                                                           */
/* This code is released to the public domain.                               */
/*****************************************************************************/
#endregion Header Comments

using System;

namespace ReinGametiMatrixLib.Matricies
{
    public class DimensionMismatchException : ArithmeticException
    {
        public DimensionMismatchException() : base() { }
        public DimensionMismatchException(string message) : base(message) { }
        public DimensionMismatchException(string message, Exception innerException) : base(message, innerException) { }
    }
}
