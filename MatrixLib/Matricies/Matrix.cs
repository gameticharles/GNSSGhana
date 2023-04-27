#region Header Comments
/*****************************************************************************/
/* Matrix.cs                                                                 */
/* ---------------                                                           */
/* Modified 12 March 2018 - Rein Gameti Charles (gameticharles@gmail.com)    */
/*                Initial coding completed                                   */                                             
/* Added more Method, function, static Methods and flexibility               */

/* 27 July 2008 - Bradley Ward (entropyau@gmail.com)                         */
/*                Initial coding completed                                   */
/*                                                                           */
/* A decorator class for MatrixBase<Double> that provides additional         */
/* functionality such as matrix inversion, mathematical operations etc.      */
/*                                                                           */
/* This code is released to the public domain.                               */
/*****************************************************************************/
#endregion Header Comments

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReinGametiMatrixLib.Matricies
{
    /// <summary>
    /// Create a matrix data
    /// </summary>
    public class Matrix
    {

        public class RowAccessor
            : IEnumerable<Matrix>
        {

            #region Private Members
            /*******************/
            /* PRIVATE MEMBERS */
            /*******************/
            private Matrix _matrix;
            #endregion Private Members


            #region Public Constructor
            /**********************/
            /* PUBLIC CONSTRUCTOR */
            /**********************/
            /// <summary>RowAccessor</summary>
            /// <param name="dataView"></param>
            public RowAccessor(Matrix matrix)
            {
                _matrix = matrix;
            }
            #endregion Public Constructor


            #region Public Accessors
            /********************/
            /* PUBLIC ACCESSORS */
            /********************/

            public Matrix this[Int32 rowIndex]
            {
                get
                {
                    return this.AsEnumerable().ElementAt(rowIndex);
                }
                set
                {
                    if (value.ColumnCount != _matrix.ColumnCount || value.RowCount != 1)
                        throw new DimensionMismatchException();

                    Matrix targetRow = this.AsEnumerable().ElementAt(rowIndex);
                    for (Int32 i = 0; i < value.ColumnCount; i++)
                        targetRow[i, 0] = value[i, 0];
                }
            }

            #endregion Public Accessors


            #region Public Methods
            /******************/
            /* PUBLIC METHODS */
            /******************/

            /// <summary>AsEnumerable</summary>
            /// <returns></returns>
            public IEnumerable<Matrix> AsEnumerable()
            {
                int j = 0;
                while (j < _matrix.RowCount)
                    yield return _matrix.SubMatrix(new Int32Range(0, _matrix.ColumnCount), new Int32Range(j, ++j));
            }


            /// <summary>GetEnumerator</summary>
            /// <returns></returns>
            public IEnumerator<Matrix> GetEnumerator()
            {
                return this.AsEnumerable().GetEnumerator();
            }


            /// <summary>IEnumerable.GetEnumerator</summary>
            /// <returns></returns>
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            #endregion Public Methods

        }

        public static class ArrayConvert
        {
            public static T[,] To2DArray<T>(params T[][] arrays)
            {
                if (arrays == null) throw new ArgumentNullException("arrays");

                if (arrays.Length == 0)
                {
                    return new T[,] { };
                }

                foreach (var a in arrays)
                {
                    if (a == null) throw new ArgumentException("can not contain null arrays");
                    if (a.Length != arrays[0].Length) throw new ArgumentException("input arrays should have the same length");
                }

                var height = arrays.Length;
                var width = arrays[0].Length;

                var result = new T[height, width];

                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        result[i, j] = arrays[i][j];
                    }
                }

                return result;
            }

            public static T[,] ToArray<T>(params T[][] arrays)
            {
                if (arrays == null) throw new ArgumentNullException("arrays");

                if (arrays.Length == 0)
                {
                    return new T[,] { };
                }

                foreach (var a in arrays)
                {
                    if (a == null) throw new ArgumentException("can not contain null arrays");
                    if (a.Length != arrays[0].Length) throw new ArgumentException("input arrays should have the same length");
                }

                var height = arrays.Length;
                var width = arrays[0].Length;

                var indexes = Enumerable.Range(0, height);
                var subIndexes = Enumerable.Range(0, width);

                // the output
                var result = new T[height, width];


                var sss = from i in indexes
                          from j in subIndexes
                          select result[i, j] = arrays[j][i];

                return result;
            }
        }

        public class ColumnAccessor
            : IEnumerable<Matrix>
        {

            #region Private Members
            /*******************/
            /* PRIVATE MEMBERS */
            /*******************/
            private Matrix _matrix;
            #endregion Private Members


            #region Public Constructor
            /**********************/
            /* PUBLIC CONSTRUCTOR */
            /**********************/
            /// <summary>ColumnAccessor</summary>
            /// <param name="dataView"></param>
            public ColumnAccessor(Matrix matrix)
            {
                _matrix = matrix;
            }

            #endregion Public Constructor


            #region Public Accessors
            /********************/
            /* PUBLIC ACCESSORS */
            /********************/

            /// <summary>this</summary>
            /// <param name="columnIndex"></param>
            /// <returns></returns>
            public Matrix this[Int32 columnIndex]
            {
                get
                {
                    return this.AsEnumerable().ElementAt(columnIndex);
                }
                set
                {
                    if (value.RowCount != _matrix.RowCount || value.ColumnCount != 1)
                        throw new DimensionMismatchException();

                    Matrix targetColumn = this.AsEnumerable().ElementAt(columnIndex);
                    for (int j = 0; j < value.RowCount; j++)
                        targetColumn[0, j] = value[0, j];
                }
            }

            #endregion Public Accessors


            #region Public Methods
            /******************/
            /* PUBLIC METHODS */
            /******************/

            /// <summary>AsEnumerable</summary>
            /// <returns></returns>
            public IEnumerable<Matrix> AsEnumerable()
            {
                int i = 0;
                while (i < _matrix.ColumnCount)
                    yield return _matrix.SubMatrix(new Int32Range(i, ++i), new Int32Range(0, _matrix.RowCount));
            }


            /// <summary>GetEnumerator</summary>
            /// <returns></returns>
            public IEnumerator<Matrix> GetEnumerator()
            {
                return this.AsEnumerable().GetEnumerator();
            }


            /// <summary>IEnumerable.GetEnumerator</summary>
            /// <returns></returns>
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            #endregion Public Methods

        }

        public class MatrixNotSquare : ApplicationException
        {
            public MatrixNotSquare() :
                base("To do this operation, matrix must be a square matrix !")
            { }
        }

        public class MatrixDeterminentZero : ApplicationException
        {
            public MatrixDeterminentZero() :
                base("Determinent of matrix equals zero, inverse can't be found !")
            { }
        }

        public class MatrixNullException : ApplicationException
        {
            public MatrixNullException() :
                base("To do this operation, matrix can not be null")
            { }
        }

        class MatrixDimensionException : ApplicationException
        {
            public MatrixDimensionException() :
                base("Dimension of the two matrices not suitable for this operation !")
            { }
        }

        class MatrixSingularException : ApplicationException
        {
            public MatrixSingularException() :
                base("Matrix is singular this operation cannot continue !")
            { }
        }

        #region Internal Maths utility
        internal class Maths
        {
            /// <summary>
            ///  sqrt(a^2 + b^2) without under/overflow.
            /// </summary>
            /// <param name="a"></param>
            /// <param name="b"></param>
            /// <returns></returns>

            public static double Hypot(double a, double b)
            {
                double r;
                if (Math.Abs(a) > Math.Abs(b))
                {
                    r = b / a;
                    r = Math.Abs(a) * Math.Sqrt(1 + r * r);
                }
                else if (b != 0)
                {
                    r = a / b;
                    r = Math.Abs(b) * Math.Sqrt(1 + r * r);
                }
                else
                {
                    r = 0.0;
                }
                return r;
            }
        }
        #endregion   // Internal Maths utility

        #region Private Static Members
        /**************************/
        /* PRIVATE STATIC MEMBERS */
        /**************************/
        private static Random _random = new Random(Environment.TickCount);

        #endregion Private Static Members

        #region Private Members
        /*******************/
        /* PRIVATE MEMBERS */
        /*******************/
        private MatrixBase<Double> _matrix;

        #endregion Private Members

        #region Public Constructors
        /***********************/
        /* PUBLIC CONSTRUCTORS */
        /***********************/

        /// <summary>
        /// 
        /// </summary>
        public Matrix() : this(0) { }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="rank"></param>
        public Matrix(int rank) : this(rank, rank) { }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="rows"></param>
        public Matrix(int columns, int rows)
        {
            _matrix = new MatrixBase<Double>(columns, rows);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="doubleArray"></param>
        public Matrix(Double[,] doubleArray)
        {
            _matrix = new MatrixBase<Double>(doubleArray);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="doubleArray"></param>
        public Matrix(Double[][] Array)
        {
            var matrix = new Matrix(ArrayConvert.To2DArray(Array));

            _matrix = matrix;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="doubleArray"></param>
        public Matrix(MatrixBase<Double> matrix)
        {
            _matrix = matrix;
        }

        /// <summary>Construct a matrix from a one-dimensional packed array</summary>
        /// <param name="vals">One-dimensional array of doubles, packed by columns (ala Fortran).
        /// </param>
        /// <param name="rows">   Number of rows.
        /// </param>
        /// <exception cref="System.ArgumentException">   Array length must be a multiple of m.
        /// </exception>
        public Matrix(double[] vals, int rows)
        {

            var cols = (rows != 0 ? vals.Length / rows : 0);

            if (cols * rows != vals.Length)
            {
                throw new System.ArgumentException("Array length must be a multiple of m.");
            }
            var A = new Double[rows, cols];

            for (int i = 0; i < cols; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    A[j, i] = vals[i + j * cols];
                }
            }

            _matrix = new MatrixBase<Double>(A);
        }

        /// <summary>Construct a matrix from a one-dimensional packed array</summary>
        /// <param name="vals">One-dimensional array of doubles, packed by columns (ala Fortran).
        /// </param>
        /// <param name="rows">   Number of rows.
        /// <param name="cols">   Number of colums.
        /// </param>
        /// <exception cref="System.ArgumentException">   Array length must be a multiple of m.
        /// </exception>
        public Matrix(double[] vals, int cols, int rows)
        {
            if (cols * rows != vals.Length)
            {
                throw new System.ArgumentException("Array length must be a multiple of m.");
            }
            var A = new Double[rows, cols];

            for (int i = 0; i < cols; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    A[j, i] = vals[i + j * cols];
                }
            }

            _matrix = new MatrixBase<Double>(A);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="matrix"></param>
        public Matrix(Matrix matrix)
        {
            _matrix = matrix.InnerMatrix.Clone();
        }


        #endregion Public Constructors

        #region Public Getters / Setters
        /****************************/
        /* PUBLIC GETTERS / SETTERS */
        /****************************/

        /// <summary>Gets or sets the value at the specified column and row of the generic matrix</summary>
        /// <param name="column">Column to access. Values are relative to the matrix view window</param>
        /// <param name="row">Row to access. Values are relative to the matrix view window</param>
        /// <returns>Value of the element at the specified column and row</returns>
        public virtual Double this[Int32 column, Int32 row]
        {
            get { return _matrix[column, row]; }
            set { _matrix[column, row] = value; }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="column"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        public virtual Matrix this[Int32 column, object row]
        {
            get
            {
                return this.GetMatrix(0, this.RowCount, new int[] { column, });
            }
            set
            {
                if (value.RowCount == _matrix.RowCount)
                {
                    this.SetMatrix(0, this.RowCount, new int[] { column, }, value);
                }
                else
                {
                    throw new System.IndexOutOfRangeException("Submatrix indices");
                }

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="column"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        public virtual Matrix this[object column, Int32 row]
        {
            get
            {
                return this.GetMatrix(new int[] { row, }, 0, this.ColumnCount);
            }
            set
            {
                if (value.RowCount == _matrix.RowCount)
                {
                    this.SetMatrix(new int[] { row, }, 0, this.ColumnCount, value);
                }
                else
                {
                    throw new System.IndexOutOfRangeException("Submatrix indices");
                }

            }
        }

        
        /// <summary>Gets the number of element visible within the matrix data view</summary>
        public virtual Int32 NumElements { get { return _matrix.ColumnCount * _matrix.RowCount; } }

        /// <summary>Gets the number of columns visible within the matrix data view</summary>
        public virtual Int32 ColumnCount { get { return _matrix.ColumnCount; } }

        /// <summary>Gets a column accessor for this matrix, enabling column-based operations</summary>
        public virtual ColumnAccessor Columns { get { return new ColumnAccessor(this); } }

        /// <summary>Gets the range of columns visible within the matrix data view</summary>
        public virtual Int32Range DataColumnRange { get { return _matrix.DataColumnRange; } }

        /// <summary>Gets the range of rows visible within the matrix data view</summary>
        public virtual Int32Range DataRowRange { get { return _matrix.DataRowRange; } }

        /// <summary>Gets a reference to the data source underlying this matrix data view</summary>
        public virtual MatrixDataSource<Double> DataSource { get { return _matrix.DataSource; } }

        /// <summary>Gets a reference to the inner generic MatrixBase wrapped by this instance of Matrix</summary>
        public MatrixBase<Double> InnerMatrix { get { return _matrix; } }


        /// <summary>Indicates whether this is matrix can be considered a column vector, where number of columns equals 1</summary>
        public virtual Boolean IsColumnVector { get { return _matrix.IsColumnVector; } }

        /// <summary>Indicates whether this matrix is empty (zero columns or zero rows)</summary>
        public virtual Boolean IsEmpty { get { return _matrix.IsEmpty; } }
              

        /// <summary>IsInvertible</summary>
        public Boolean IsInvertible { get { throw new NotImplementedException(); } }

        /// <summary>Indicates whether this matrix is not a vector (has > 1 row AND > 1 column)</summary>
        public virtual Boolean IsMatrix { get { return _matrix.IsMatrix; } }

        /// <summary>Indicates whether this is matrix can be considered a row vector, where number of rows equals 1</summary>
        public virtual Boolean IsRowVector { get { return _matrix.IsRowVector; } }


        /// <summary>
        /// Indicates whether this matrix represents a scalar value.
        /// A matrix can be considered representing a scalar value
        /// if number of columns equals 1 and number of rows equals 1
        /// NOTE: alternative definition: all elements = single value
        /// </summary>
        public virtual Boolean IsScalar { get { return _matrix.IsScalar;  } }

        /// <summary>Indicates whether this is a square matrix</summary>
        public virtual Boolean IsSquare { get { return _matrix.IsSquare; } }


        /// <summary>
        /// Indicates whether this matrix can be considered a vector. 
        /// For a given matrix of dimension m x n, it can be considered a vector
        /// if number of column equals 1 or number of rows equals 1
        /// </summary>
        public virtual Boolean IsVector { get { return _matrix.IsVector; } }

        /// <summary>
        /// Returns the pseudoinverse of a matrix, such that
        /// X = PseudoInverse(A) produces a matrix 'X' of the same dimensions
        /// as A' so that A*X*A = A, X*A*X = X.
        /// In case of an error the error is raised as an exception.
        /// </summary>
        /// <param name="Mat">a Matrix object whose pseudoinverse is to be found</param>
        /// <returns>The pseudoinverse of the Matrix object as a Matrix Object</returns>
        public virtual Matrix GetPseudoInverse { get { return Matrix.PseudoInverse(this); } }

        /// <summary>Gets the number of rows visible within the matrix data view</summary>
        public virtual Int32 RowCount { get { return _matrix.RowCount; } }

        /// <summary>Gets a row accessor for this matrix, enabling row-based operations</summary>
        public virtual RowAccessor Rows { get { return new RowAccessor(this); } }

        /// <summary>Gets the minimum value of all elements in the matrix</summary>
        public Double MinimumValue
        {
            get
            {
                if (this.IsEmpty) throw new ArithmeticException("Cannot calculate minimum value of an empty matrix");
                Double? minValue = null;
                for (int i = 0; i < this.ColumnCount; i++)
                    for (int j = 0; j < this.RowCount; j++)
                        if (minValue == null || (Double)minValue > this[i, j]) minValue = this[i, j];
                return (Double)minValue;
            }
        }

        /// <summary>Gets the maximum value of all elements in the matrix</summary>
        public Double MaximumValue
        {
            get
            {
                if (this.IsEmpty) throw new ArithmeticException("Cannot calculate maximum value of an empty matrix");
                Double? maxValue = null;
                for (int i = 0; i < this.ColumnCount; i++)
                    for (int j = 0; j < this.RowCount; j++)
                        if (maxValue == null || (Double)maxValue < this[i, j]) maxValue = this[i, j];
                return (Double)maxValue;
            }
        }


        #endregion Public Getters / Setters

        #region Public Methods
        /******************/
        /* PUBLIC METHODS */
        /******************/

        /// <summary>
        /// Copy a matrix to another
        /// </summary>
        public Matrix Copy()
        {
            return new Matrix(_matrix.Clone());
        }

        /// <summary>
        /// Get rank of the matrix
        /// </summary>
        /// <returns></returns>
        public virtual int Rank()
        {
            return Rank(this.ToArray());
        }

        /// <summary>
        /// Reshapes the matrix to the specified number of rows and columns.
        /// </summary>
        /// <param name="column">Number of Columns</param>
        /// <param name="row">Number of Rows</param>
        public virtual void Reshape(Int32 column, Int32 row)
        {
            if (row < 0 || column < 0)
            {
                throw new System.IndexOutOfRangeException("Submatrix indices");
            }

            if (row < _matrix.RowCount && column < _matrix.ColumnCount)
            {
                var reshapedMat = new Matrix(column, row);

                for (int i = 0; i < row; i++)
                {
                    for (int j = 0; j < column; j++)
                    {
                        reshapedMat[j, i] = this[j, i];
                    }

                }

                _matrix = reshapedMat;
            }
            else
            {
                throw new System.IndexOutOfRangeException("Submatrix indices");
            }

        }

        /// <summary>
        /// Remove row(s) from the matrix
        /// </summary>
        /// <param name="rows">Rows to remove</param>
        public virtual void RemoveRow(int row)
        {
            if (this.RowCount - 1 < 0) { throw new System.IndexOutOfRangeException("Submatrix can not have negative indices"); }

            var acopyMat = new Matrix(this.ColumnCount, this.RowCount - 1);

            int k = 0;
            for (int i = 0; i < this.RowCount; i++)
            {
                for (int j = 0; j < this.ColumnCount; j++)
                {
                    if (i != row)
                    {
                        acopyMat[j, k] = this[j, i];
                    }
                }

                k++;

                if (i == row) { k--; }
            }

            _matrix = acopyMat;
        }

        /// <summary>
        /// Remove row(s) from the matrix
        /// </summary>
        /// <param name="rows">Rows to remove</param>
        public virtual void RemoveRows(int[] rows)
        {
            if (this.RowCount - rows.Length < 0) { throw new System.IndexOutOfRangeException("Submatrix can not have negative indices"); }

            var acopyMat = new Matrix(this.ColumnCount, this.RowCount - rows.Length);

            int k = 0;
            for (int i = 0; i < this.RowCount; i++)
            {
                for (int j = 0; j < this.ColumnCount; j++)
                {
                    if (!rows.Contains(i))
                    {
                        acopyMat[j, k] = this[j, i];
                    }
                }

                k++;

                if (rows.Contains(i)) { k--; }
            }

            _matrix = acopyMat;
        }

        /// <summary>
        /// Remove column from the matrix
        /// </summary>
        /// <param name="rows">Rows to remove</param>
        public virtual void RemoveColumn(int column)
        {
            if (this.ColumnCount - 1 < 0) { throw new System.IndexOutOfRangeException("Submatrix can not have negative indices"); }

            var acopyMat = new Matrix(this.ColumnCount - 1, this.RowCount);

            for (int i = 0; i < this.RowCount; i++)
            {
                int k = 0;

                for (int j = 0; j < this.ColumnCount; j++)
                {

                    if (column != j)
                    {
                        acopyMat[k, i] = this[j, i];
                    }

                    k++;

                    if (column == j) { k--; }
                }

            }

            _matrix = acopyMat;
        }

        /// <summary>
        /// Remove columns from the matrix
        /// </summary>
        /// <param name="rows">Rows to remove</param>
        public virtual void RemoveColumns(int[] columns)
        {
            if (this.ColumnCount - columns.Length < 0) { throw new System.IndexOutOfRangeException("Submatrix can not have negative indices"); }

            var acopyMat = new Matrix(this.ColumnCount - columns.Length, this.RowCount);

            for (int i = 0; i < this.RowCount; i++)
            {
                int k = 0;

                for (int j = 0; j < this.ColumnCount; j++)
                {

                    if (!columns.Contains(j))
                    {
                        acopyMat[k, i] = this[j, i];
                    }

                    k++;

                    if (columns.Contains(j)) { k--; }
                }

            }

            _matrix = acopyMat;
        }

        /// <summary>
        /// Get a values at a position
        /// </summary>
        /// <param name="column">Column</param>
        /// <param name="row">Row</param>
        /// <returns>Return a value</returns>
        public virtual Double GetElement(Int32 column, Int32 row)
        {
            return _matrix[column, row];
        }

        /// <summary>
        /// Set a Matrix position to a value
        /// </summary>
        /// <param name="column">Column</param>
        /// <param name="row">Row</param>
        /// <param name="value">Values to insert</param>
        public virtual void SetElement(Int32 column, Int32 row, Double value)
        {
            _matrix[column, row] = value;
        }


        /// <summary>
        /// Sets all the elements in this matrix equal to the specified value.
        /// </summary>
        /// <param name="value"></param>
        public virtual void SetElement(Double value)
        {
            for (int i = 0; i < RowCount; i++)
            {
                for (int j = 0; j < ColumnCount; j++)
                {
                    _matrix[j, i] = value;
                }
            }

        }

        /// <summary>
        /// Computes the sum of all the elements in the matrix.
        /// </summary>
        /// <returns>Sum of all the elements</returns>
        public virtual double ElementSum()
        {
            double result = 0;
            for (int i = 0; i < this.RowCount; i++)
            {
                for (int j = 0; j < this.ColumnCount; j++)
                {
                    result += this[j, i];
                }
            }
            return result;
        }

        /// <summary>
        /// Computes the sum of all the absolute elements in the matrix.
        /// </summary>
        /// <returns>Sum of all the absolute elements</returns>
        public virtual double ElementAbsSum()
        {
            double result = 0;
            for (int i = 0; i < this.RowCount; i++)
            {
                for (int j = 0; j < this.ColumnCount; j++)
                {
                    result += Math.Abs(this[j, i]);
                }
            }
            return result;
        }

        /// <summary>
        /// Returns the maximum absolute value of all the elements in this matrix. This is equivalent the the infinite p-norm of the matrix.
        /// </summary>
        /// <returns>Largest absolute value of any element.</returns>
        public virtual double ElementMaxAbs()
        {
            double result = 0;
            for (int i = 0; i < this.RowCount; i++)
            {
                for (int j = 0; j < this.ColumnCount; j++)
                {
                    if (result < Math.Abs(this[j, i]))
                    {
                        result = Math.Abs(this[j, i]);
                    }
                    
                }
            }
            return result;
        }


        /// <summary>
        /// Merge this matrix with A horizontally
        /// </summary>
        /// <param name="A"></param>
        /// <returns></returns>
        public Matrix MergeWith(Matrix A)
        {
            if (A == null) return this;

            if (A.RowCount != RowCount)
                throw new Exception("Cannot merge two matrices with different row counts");

            Matrix result = new Matrix(A.ColumnCount + ColumnCount, RowCount);

            for (int r = 0; r < RowCount; r++)
            {
                for (int c = 0; c < ColumnCount; c++)
                    result[c, r] = this[c, r];
                for (int c = 0; c < A.ColumnCount; c++)
                    result[ c + ColumnCount, r] = A[ c, r];
            }

            return result;
        }

        /// <summary>
        /// Merge this matrix with A Vertically if true else Horizontally
        /// </summary>
        /// <param name="A"></param>
        /// <returns></returns>
        public Matrix MergeWith(Matrix A, bool Vertically)
        {
            if (A == null) return this;

            if (A.RowCount != RowCount)
                throw new Exception("Cannot merge two matrices with different row counts");

            Matrix result;
            if (Vertically)
            {
                result = JoinVertical(this, A);
            }
            else
            {
                result = JoinHorizontal(this, A);
            }
            
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="col"></param>
        /// <returns></returns>
        public Matrix[] Split(int col)
        {
            if ((col < 0) || (col >= ColumnCount))
                throw new Exception("Invalid column index for Split()");

            Matrix[] result = new Matrix[2];
            result[0] = new Matrix(col, this.RowCount);
            result[1] = new Matrix(this.ColumnCount - col, this.RowCount);

            for (int c = 0; c < col; c++)
                for (int r = 0; r < RowCount; r++)
                    result[0][c, r] = this[c, r];

            for (int c = col; c < ColumnCount; c++)
                for (int r = 0; r < RowCount; r++)
                    result[1][ c - col, r] = this[c, r];
            
            return result;
        }

        /// <summary>
        /// Creates a new matrix that is a combination of this matrix and matrix B.
        /// B is written into A at the specified location if needed the size of A is increased by growing it.
        /// </summary>
        /// <param name="row">Row where matrix B is written in to</param>
        /// <param name="column">Column where matrix B is written in to</param>
        /// <param name="matrix">The matrix that is written into A</param>
        /// <returns>A new combined matrix</returns>
        public virtual Matrix Combines(int row, int column , Matrix matrix)
        {
            if (row < 0 || column < 0)
            {
                throw new System.IndexOutOfRangeException("Submatrix indices");
            }
            var nRows = matrix.RowCount + _matrix.RowCount;
            var nCols = matrix.ColumnCount + _matrix.ColumnCount;

            var reshapedMat = new Matrix(nCols, nRows);

            //if (row < _matrix.RowCount && column < _matrix.ColumnCount)
            //{

            //    for (int i = 0; i < row; i++)
            //    {
            //        for (int j = 0; j < column; j++)
            //        {
            //            reshapedMat[j, i] = this[j, i];
            //        }

            //    }
            //}
            //else if (row > _matrix.RowCount && column > _matrix.ColumnCount)
            //{
            //    for (int i = 0; i < _matrix.RowCount; i++)
            //    {
            //        for (int j = 0; j < _matrix.ColumnCount; j++)
            //        {
            //            reshapedMat[j, i] = this[j, i];
            //        }

            //    }
            //}
            //else
            //{
            //    throw new System.IndexOutOfRangeException("Submatrix indices");
            //}

            return reshapedMat;
        }

        /// <summary>Solve A*X = B</summary>
        /// <param name="B">  A Matrix with as many rows as A and any number of columns.
        /// </param>
        /// <returns>     X so that L*L'*X = B
        /// </returns>
        /// <exception cref="System.ArgumentException">  Matrix row dimensions must agree.
        /// </exception>
        /// <exception cref="System.SystemException"> Matrix is not symmetric positive definite.
        /// </exception>
        public virtual Matrix Solve(Matrix matrix)
        {
            if (matrix.RowCount != this.RowCount)
            {
                throw new System.ArgumentException("Matrix row dimensions must agree.");
            }
            if (!IsSquare)
            {
                throw new System.SystemException("Matrix is not symmetric positive definite.");
            }

            return Solve(this, matrix);
        }

        /// <summary>
        /// Evaluates the Singular Value Decomposition of a matrix, 
        /// returns the  matrices S, U and V. Such that a given
        /// Matrix = U x S x V'.
        /// In case of an error the error is raised as an exception. 
        /// Note: This method is based on the 'Singular Value Decomposition'
        /// section of Numerical Recipes in C by William H. Press,
        /// Saul A. Teukolsky, William T. Vetterling and Brian P. Flannery,
        /// University of Cambridge Press 1992.
        /// </summary>        
        /// <param name="S">A Matrix object where the S matrix is returned</param>
        /// <param name="U">A Matrix object where the U matrix is returned</param>
        /// <param name="V">A Matrix object where the V matrix is returned</param>
        public virtual void SUV(out Matrix S, out Matrix U, out Matrix V)
        {
            SVD(this, out S, out U, out V);
        }

        /// <summary>
        /// Returns the Eigenvalues and Eigenvectors of a real symmetric
        /// matrix, which is of dimensions [n,n]. In case of an error the
        /// error is raised as an exception.
        /// Note: This method is based on the 'Eigenvalues and Eigenvectors of a TridiagonalMatrix'
        /// section of Numerical Recipes in C by William H. Press,
        /// Saul A. Teukolsky, William T. Vetterling and Brian P. Flannery,
        /// University of Cambridge Press 1992.
        /// </summary>
        /// The Matrix object whose Eigenvalues and Eigenvectors are to be found
        /// </param>
        /// <param name="d">A Matrix object where the eigenvalues are returned</param>
        /// <param name="v">A Matrix object where the eigenvectors are returned</param>
        public virtual void GetEigen(out Matrix d, out Matrix v)
        {
            Eigen(this, out d, out v);
        }

        public virtual void EigenValues(out Matrix Values)
        {
            EigenValues(this, out Values);
        }

        public virtual void EigenVectors(out Matrix Vectors)
        {
            EigenVectors(this, out Vectors);
        }

        private Matrix eigenValues, eigenVectors;

        public Matrix EigenValues()
        {
            EigenValues(this, out eigenValues);
            return eigenValues;
        }

        public Matrix EigenVectors()
        {
            EigenVectors(this, out eigenVectors);
            return eigenVectors;
        }

        /// <summary>
        /// Returns the LU Decomposition of a matrix. 
        /// the output is: lower triangular matrix L, upper
        /// triangular matrix U, and permutation matrix P so that
        ///	P*X = L*U.
        /// In case of an error the error is raised as an exception. 
        /// Note: This method is based on the 'LU Decomposition and Its Applications'
        /// section of Numerical Recipes in C by William H. Press,
        /// Saul A. Teukolsky, William T. Vetterling and Brian P. Flannery,
        /// University of Cambridge Press 1992.  
        /// </summary>
        /// <param name="L">A Matrix object where the lower traingular matrix is returned</param>
        /// <param name="U">A Matrix object where the upper traingular matrix is returned</param>
        /// <param name="P">A Matrix object where the permutation matrix is returned</param>
        public virtual void LU(out Matrix L, out Matrix U, out Matrix P)
        {
            LU(this, out L, out U, out P);
        }

        /// <summary>
        /// Reshapes the matrix to a new matrix of the specified number of rows and columns.
        /// Will replace the matrix with a new one if saveResult = true 
        /// </summary>
        /// <param name="column">Number of Columns</param>
        /// <param name="row">Number of Rows</param>
        /// <exception cref="System.IndexOutOfRangeException">  Matrix row dimensions must agree.
        /// </exception>
        /// <returns>Return a new matrix of new shape</returns>
        public Matrix Reshape(Int32 column, Int32 row, bool saveResult = false)
        {
            if (row < 0 || column < 0)
            {
                throw new System.IndexOutOfRangeException("Submatrix indices");
            }

            var reshapedMat = new Matrix(column, row);

            if (row < _matrix.RowCount || column < _matrix.ColumnCount)
            {
               
                for (int i = 0; i < row; i++)
                {
                    for (int j = 0; j < column; j++)
                    {
                        reshapedMat[j, i] = this[j, i];
                    }

                }
            }
            else if (row > _matrix.RowCount || column > _matrix.ColumnCount)
            {
                for (int i = 0; i < _matrix.RowCount; i++)
                {
                    for (int j = 0; j < _matrix.ColumnCount; j++)
                    {
                        reshapedMat[j, i] = this[j, i];
                    }

                }
            }
            else
            {
                throw new System.IndexOutOfRangeException("Submatrix indices");
            }


            if (saveResult)
            {
                _matrix = reshapedMat;
            }

            return reshapedMat;
        }


        /// <summary>Gets a copy of the inverse of this square matrix</summary>
        public Matrix Inverse() { return Matrix.Invert(this); }

        /// <summary>Gets a transposed copy of this matrix (A^T)</summary>
        public virtual Matrix Transpose() { return new Matrix(_matrix.Transposed); }

        /// <summary>
        /// Get determinant of the matrix
        /// </summary>
        /// <returns></returns>
        public Double Determinant()
        {
            return Determinant(this);
        }

        /// <summary>Set a submatrix.</summary>
        /// <param name="i0">  Initial row index
        /// </param>
        /// <param name="i1">  Final row index
        /// </param>
        /// <param name="j0">  Initial column index
        /// </param>
        /// <param name="j1">  Final column index
        /// </param>
        /// <param name="matrix">   A(i0:i1,j0:j1)
        /// </param>
        /// <exception cref="System.IndexOutOfRangeException">  Submatrix indices
        /// </exception>
        public virtual void SetMatrix(int i0, int i1, int j0, int j1, Matrix matrix)
        {
            try
            {
                for (int i = i0; i < i1; i++)
                {
                    for (int j = j0; j < j1; j++)
                    {
                        this[j, i] = matrix[j - j0, i - i0];
                    }
                }
            }
            catch (System.IndexOutOfRangeException e)
            {
                throw new System.IndexOutOfRangeException("Submatrix indices", e);
            }
        }

        /// <summary>Set a submatrix.</summary>
        /// <param name="r">   Array of row indices.
        /// </param>
        /// <param name="c">   Array of column indices.
        /// </param>
        /// <param name="matrix">   A(r(:),c(:))
        /// </param>
        /// <exception cref="System.IndexOutOfRangeException">  Submatrix indices
        /// </exception>
        public virtual void SetMatrix(int[] r, int[] c, Matrix matrix)
        {
            try
            {
                for (int i = 0; i < r.Length; i++)
                {
                    for (int j = 0; j < c.Length; j++)
                    {
                        this[c[j], r[i]] = matrix[j, i];
                    }
                }
            }
            catch (System.IndexOutOfRangeException e)
            {
                throw new System.IndexOutOfRangeException("Submatrix indices", e);
            }
        }

        /// <summary>
        /// Set a submatrix.
        /// </summary>
        /// <param name="r">   Array of row indices.
        /// </param>
        /// <param name="j0">  Initial column index
        /// </param>
        /// <param name="j1">  Final column index
        /// </param>
        /// <param name="matrix">   A(r(:),j0:j1)
        /// </param>
        /// <exception cref="System.IndexOutOfRangeException"> Submatrix indices
        /// </exception>
        public virtual void SetMatrix(int[] r, int j0, int j1, Matrix matrix)
        {
            try
            {
                for (int i = 0; i < r.Length; i++)
                {
                    for (int j = j0; j < j1; j++)
                    {
                        this[j, r[i]] = matrix[j - j0, i];
                    }
                }
            }
            catch (System.IndexOutOfRangeException e)
            {
                throw new System.IndexOutOfRangeException("Submatrix indices", e);
            }
        }

        /// <summary>Set a submatrix.</summary>
        /// <param name="i0">  Initial row index
        /// </param>
        /// <param name="i1">  Final row index
        /// </param>
        /// <param name="c">   Array of column indices.
        /// </param>
        /// <param name="matrix">   A(i0:i1,c(:))
        /// </param>
        /// <exception cref="System.IndexOutOfRangeException">  Submatrix indices
        /// </exception>
        public virtual void SetMatrix(int i0, int i1, int[] c, Matrix matrix)
        {
            try
            {
                for (int i = i0; i < i1; i++)
                {
                    for (int j = 0; j < c.Length; j++)
                    {
                        this[c[j], i] = matrix[j, i - i0];
                    }
                }
            }
            catch (System.IndexOutOfRangeException e)
            {
                throw new System.IndexOutOfRangeException("Submatrix indices", e);
            }
        }


        /// <summary>
        /// Set a submatrix. matrix must have 1 row x same columns
        /// </summary>
        /// <param name="row">   Row index.
        /// </param>
        /// <param name="matrix"> Matrix  1 x n
        /// </param>
        /// <exception cref="System.IndexOutOfRangeException"> Submatrix indices
        /// </exception>
        public virtual void SetRow(int row, Matrix matrix)
        {
            try
            {               
                for (int j = 0; j < this.ColumnCount; j++)
                {
                    this[j, row] = matrix[j, 0];
                }                
            }
            catch (System.IndexOutOfRangeException e)
            {
                throw new System.IndexOutOfRangeException("Submatrix indices", e);
            }
        }

        /// <summary>
        /// Set an array. array must have 1 row x same columns
        /// </summary>
        /// <param name="row">   Row index.
        /// </param>
        /// <param name="matrix"> Matrix  1 x n
        /// </param>
        /// <exception cref="System.IndexOutOfRangeException"> Submatrix indices
        /// </exception>
        public virtual void SetRow(int row, double[,] Mat)
        {
            var matrix = new Matrix(Mat);
            try
            {
                for (int j = 0; j < this.ColumnCount; j++)
                {
                    this[j, row] = matrix[j, 0];
                }
            }
            catch (System.IndexOutOfRangeException e)
            {
                throw new System.IndexOutOfRangeException("Submatrix indices", e);
            }
        }

        /// <summary>
        /// Assigns consecutive elements inside a row to the provided array.
        /// </summary>
        /// <param name="row">The row that the array is to be written to.</param>
        /// <param name="startColumn">The initial column that the array is written to.</param>
        /// <param name="matrix">Values which are to be written to the row in a matrix</param>
        public virtual void SetRow(int row, int startColumn, Matrix matrix)
        {
            try
            {
                for (int j = startColumn; j < this.ColumnCount; j++)
                {
                    this[j, row] = matrix[j, 0];
                }
            }
            catch (System.IndexOutOfRangeException e)
            {
                throw new System.IndexOutOfRangeException("Submatrix indices", e);
            }
        }

        /// <summary>
        /// Assigns consecutive elements inside a row to the provided array.
        /// </summary>
        /// <param name="row">The row that the array is to be written to.</param>
        /// <param name="startColumn">The initial column that the array is written to.</param>
        /// <param name="matrix">Values which are to be written to the row in a matrix</param>
        public virtual void SetRow(int row, int startColumn, double[,] Mat)
        {
            var matrix = new Matrix(Mat);
            try
            {
                for (int j = startColumn; j < this.ColumnCount; j++)
                {
                    this[j, row] = matrix[j, 0];
                }
            }
            catch (System.IndexOutOfRangeException e)
            {
                throw new System.IndexOutOfRangeException("Submatrix indices", e);
            }
        }

        /// <summary>
        /// Set a submatrix. matrix must have same rows x 1 column
        /// </summary>
        /// <param name="column">   Row index.
        /// </param>
        /// <param name="matrix">   A(r(:),j0:j1)
        /// </param>
        /// <exception cref="System.IndexOutOfRangeException"> Submatrix indices
        /// </exception>
        public virtual void SetColumn(int column, Matrix matrix)
        {
            try
            {
                for (int j = 0; j < this.ColumnCount; j++)
                {
                    this[column, j] = matrix[j, 0];
                }
            }
            catch (System.IndexOutOfRangeException e)
            {
                throw new System.IndexOutOfRangeException("Submatrix indices", e);
            }
        }

        /// <summary>
        /// Set a submatrix. matrix must have same rows x 1 column
        /// </summary>
        /// <param name="column">   Row index.
        /// </param>
        /// <param name="matrix">   A(r(:),j0:j1)
        /// </param>
        /// <exception cref="System.IndexOutOfRangeException"> Submatrix indices
        /// </exception>
        public virtual void SetColumn(int column, double[,] Mat)
        {
            var matrix = new Matrix(Mat);
            try
            {
                for (int j = 0; j < this.ColumnCount; j++)
                {
                    this[column, j] = matrix[j, 0];
                }
            }
            catch (System.IndexOutOfRangeException e)
            {
                throw new System.IndexOutOfRangeException("Submatrix indices", e);
            }
        }

        /// <summary>
        /// Assigns consecutive elements inside a column to the provided array.
        /// </summary>
        /// <param name="column">The column that the array is to be written to.</param>
        /// <param name="startRow">The initial column that the array is written to.</param>
        /// <param name="matrix">Values which are to be written to the row in a matrix.</param>
        public virtual void SetColumn(int column, int startRow, Matrix matrix)
        {
            try
            {
                for (int j = startRow; j < this.ColumnCount; j++)
                {
                    this[column, j] = matrix[j, 0];
                }
            }
            catch (System.IndexOutOfRangeException e)
            {
                throw new System.IndexOutOfRangeException("Submatrix indices", e);
            }
        }

        /// <summary>
        /// Assigns consecutive elements inside a column to the provided array.
        /// </summary>
        /// <param name="column">The column that the array is to be written to.</param>
        /// <param name="startRow">The initial column that the array is written to.</param>
        /// <param name="matrix">Values which are to be written to the row in a matrix.</param>
        public virtual void SetColumn(int column, int startRow, double[,] Mat)
        {
            var matrix = new Matrix(Mat);
            try
            {
                for (int j = startRow; j < this.ColumnCount; j++)
                {
                    this[column, j] = matrix[j, 0];
                }
            }
            catch (System.IndexOutOfRangeException e)
            {
                throw new System.IndexOutOfRangeException("Submatrix indices", e);
            }
        }

        /// <summary>Get a submatrix.</summary>
        /// <param name="i0">  Initial row index
        /// </param>
        /// <param name="i1">  Final row index
        /// </param>
        /// <param name="j0">  Initial column index
        /// </param>
        /// <param name="j1">  Final column index
        /// </param>
        /// <exception cref="System.IndexOutOfRangeException">  Submatrix indices
        /// </exception>
        public virtual Matrix GetMatrix(int i0, int i1, int j0, int j1)
        {
            try
            {
                var subMat = new Matrix(j1 - j0, i1 - i0);

                for (int i = i0; i < i1; i++)
                {
                    for (int j = j0; j < j1; j++)
                    {
                        subMat[j, i] = this[j - j0, i - i0];
                    }
                }

                return subMat;
            }
            catch (System.IndexOutOfRangeException e)
            {
                throw new System.IndexOutOfRangeException("Submatrix indices", e);
            }
        }

        /// <summary>Set a submatrix.</summary>
        /// <param name="r">   Array of row indices.
        /// </param>
        /// <param name="c">   Array of column indices.
        /// </param>
        /// <exception cref="System.IndexOutOfRangeException">  Submatrix indices
        /// </exception>
        public virtual Matrix GetMatrix(int[] r, int[] c)
        {
            try
            {
                var subMat = new Matrix(c.Length, r.Length);

                for (int i = 0; i < r.Length; i++)
                {
                    for (int j = 0; j < c.Length; j++)
                    {
                        subMat[j, i] = this[c[j], r[i]];
                    }
                }

                return subMat;
            }
            catch (System.IndexOutOfRangeException e)
            {
                throw new System.IndexOutOfRangeException("Submatrix indices", e);
            }
        }

        /// <summary>
        /// Set a submatrix.
        /// </summary>
        /// <param name="r">   Array of row indices.
        /// </param>
        /// <param name="j0">  Initial column index
        /// </param>
        /// <param name="j1">  Final column index
        /// </param>
        /// <exception cref="System.IndexOutOfRangeException"> Submatrix indices
        /// </exception>
        public virtual Matrix GetMatrix(int[] r, int j0, int j1)
        {
            try
            {
                var subMat = new Matrix(j1 - j0, r.Length);

                for (int i = 0; i < r.Length; i++)
                {
                    for (int j = j0; j < j1; j++)
                    {
                        subMat[j - j0, i] = this[j, r[i]]; 
                    }
                }

                return subMat;
            }
            catch (System.IndexOutOfRangeException e)
            {
                throw new System.IndexOutOfRangeException("Submatrix indices", e);
            }
        }

        /// <summary>Get a submatrix.</summary>
        /// <param name="i0">  Initial row index
        /// </param>
        /// <param name="i1">  Final row index
        /// </param>
        /// <param name="c">   Array of column indices.
        /// </param>
        /// <exception cref="System.IndexOutOfRangeException">  Submatrix indices
        /// </exception>
        public virtual Matrix GetMatrix(int i0, int i1, int[] c)
        {
            try
            {
                var subMat = new Matrix(c.Length, i1 - i0);
                
                for (int i = i0; i < i1; i++)
                {
                    for (int j = 0; j < c.Length; j++)
                    {
                        
                        subMat[j, i - i0] = this[c[j], i]; 
                    }
                }

                return subMat;
            }
            catch (System.IndexOutOfRangeException e)
            {
                throw new System.IndexOutOfRangeException("Submatrix indices", e);
            }
        }


        /// <summary>Swaps the order of the two specified columns in the matrix</summary>
        /// <param name="column1">Column to swap</param>
        /// <param name="column2">Column to swap</param>
        public void SwapColumns(Int32 column1, Int32 column2)
        {
            MatrixBase<Double>.SwapColumns(this, column1, column2);
        }

        /// <summary>Swaps the order of the two specified rows in the matrix</summary>
        /// <param name="row1">Row to swap</param>
        /// <param name="row2">Row to swap</param>
        public void SwapRows(Int32 row1, Int32 row2)
        {
            MatrixBase<Double>.SwapRows(this, row1, row2);
        }

        /// <summary>
		/// This property returns the matrix as an array.
		/// </summary>
		public double[,] ToArray()
        {
             return Matrix2Array(this); 
        }
        
        /// <summary>CopyInto</summary>
        /// <param name="targetMatrix"></param>
        public void CopyInto(Matrix targetMatrix)
        {
            this.CopyInto(targetMatrix, 0, 0);
        }


        /// <summary>CopyInto</summary>
        /// <param name="targetMatrix"></param>
        /// <param name="targetColumnOffset"></param>
        /// <param name="targetRowOffset"></param>
        public void CopyInto(Matrix targetMatrix, Int32 targetColumnOffset, Int32 targetRowOffset)
        {
            MatrixBase<Double>.ElementWiseCopy(
                this.InnerMatrix, 
                targetMatrix.InnerMatrix, 
                targetColumnOffset, 
                targetRowOffset);
        }


        /// <summary>CopyFrom</summary>
        /// <param name="sourceMatrix"></param>
        public void CopyFrom(Matrix sourceMatrix)
        {
            this.InnerMatrix.CopyFrom(sourceMatrix.InnerMatrix, 0, 0);
        }


        /// <summary>CopyFrom</summary>
        /// <param name="sourceMatrix"></param>
        /// <param name="targetColumnOffset"></param>
        /// <param name="targetRowOffset"></param>
        public void CopyFrom(Matrix sourceMatrix, Int32 targetColumnOffset, Int32 targetRowOffset)
        {
            MatrixBase<Double>.ElementWiseCopy(
                sourceMatrix.InnerMatrix, 
                this.InnerMatrix, 
                targetColumnOffset, 
                targetRowOffset);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public Matrix CrossProduct(Matrix b)
        {
            throw new NotImplementedException();
            //return Matrix.CrossProduct(this, b);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public Double DotProduct(Matrix b)
        {
            return Matrix.DotProduct(this, b);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Matrix Negate()
        {
            return (-1) * this;
        }

        /// <summary>
        /// Get the diagonal elements of a square matrix
        /// </summary>
        /// <returns>Returns the column vector of the leading diagonals</returns>
        public Matrix DiagAsMatrix()
        {
            if (this.IsEmpty) return null;
            if (!this.IsSquare) return null;

            var matArray = Zero(this.RowCount);

            for (int i = 0; i < this.RowCount; i++)
            {                
                 matArray[i, i] = this[i, i];                
            }

            return matArray;
        }

        /// <summary>
        /// Get column vector diagonal elements of a square matrix
        /// </summary>
        /// <returns>Returns the column vector of the leading diagonals</returns>
        public Matrix DiagAsVector()
        {            
            return DiagAsVector(this); ;
        }

        /// <summary>
        /// Normalizes the matrix so all values lie within the interval [0 .. 1]
        /// </summary>
        /// <returns>Normalized matrix</returns>
        public Matrix Normalize()
        {
            double min = this.MinimumValue;
            double max = this.MaximumValue;
            return ((this - min) / (max - min));
        }

        /// <summary>
        /// One norm
        /// </summary>
        /// <returns>    maximum column sum.
        /// </returns>
        public virtual double Norm1()
        {
            double f = 0;
            for (int j = 0; j < this.ColumnCount; j++)
            {
                double s = 0;
                for (int i = 0; i < this.RowCount; i++)
                {
                    s += System.Math.Abs(this[j,i]);
                }
                f = System.Math.Max(f, s);
            }
            return f;
        }
         

        /// <summary>
        /// Infinity norm
        /// </summary>
        /// <returns>    
        /// maximum row sum.
        /// </returns>
        public virtual double NormInf()
        {
            double f = 0;
            for (int i = 0; i < this.RowCount; i++)
            {
                double s = 0;
                for (int j = 0; j < this.ColumnCount; j++)
                {
                    s += System.Math.Abs(this[j,i]);
                }
                f = System.Math.Max(f, s);
            }
            return f;
        }

        /// <summary>Frobenius norm</summary>
        /// <returns>    sqrt of sum of squares of all elements.
        /// </returns>
        public virtual double NormF()
        {
            double f = 0;
            for (int i = 0; i < this.RowCount; i++)
            {
                for (int j = 0; j < this.ColumnCount; j++)
                {
                    f = Maths.Hypot(f, this[j,i]);
                }
            }
            return f;
        }


        /// <summary>
        /// Add matrix to this
        /// </summary>
        /// <param name="matrix">Frist matrix</param>
        /// <returns></returns>
        public Matrix Plus(Matrix matrix)
        {
            return new Matrix(
                MatrixBase<Double>.ElementWiseOperation(
                    this.InnerMatrix, matrix.InnerMatrix,
                    delegate (Double element1, Double element2)
                    { return element1 + element2; }));

        }


        /// <summary>
        /// Add a scaler value to this
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public Matrix Plus(Double scalar)
        {
            return Addition(this,scalar);
        }

        /// <summary>
        /// Performs a matrix addition and scale operation. c = this + matrix*scaler 
        /// </summary>
        /// <param name="scalar"></param>
        /// <param name="matrix">m by n matrix. Not modified</param>
        /// <returns>A matrix that contains the results.</returns>
        public Matrix Plus(Double scalar, Matrix matrix)
        {
            return this.Plus(matrix.Multiply(scalar));

        }

        /// <summary>
        /// Subtract matrix from this
        /// </summary>
        /// <param name="matrix">Frist matrix</param>
        /// <returns></returns>
        public Matrix Minus(Matrix matrix)
        {
            return Subtraction(this,matrix);
        }


        /// <summary>
        /// Subtract value from this
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public Matrix Minus(double scalar)
        {
            return Subtraction(this,scalar);
        }

        /// <summary>
        /// Performs a matrix subtraction and scale operation. c = this - matrix*scaler 
        /// </summary>
        /// <param name="scalar"></param>
        /// <param name="matrix">m by n matrix. Not modified</param>
        /// <returns>A matrix that contains the results.</returns>
        public Matrix Minus(Double scalar, Matrix matrix)
        {
            return this.Minus(matrix.Multiply(scalar));

        }

        /// <summary>
        /// Divide by a scaler number
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public Matrix RightDivide(double scalar)
        {
            return RightDivision(this, scalar);
        }

        /// <summary>
        /// Divide by a scaler number
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public Matrix LeftDivide(double scalar)
        {
            return LeftDivision(this, scalar);
        }

        /// <summary>
        /// Element-by-element right division, C = A./B
        /// </summary>
        /// <param name="matrix">   another matrix
        /// </param>
        /// <returns>     A./B
        /// </returns>
        public virtual Matrix RightDivide(Matrix matrix)
        {
            return RightDivision(this, matrix);
        }

        /// <summary>
        /// Element-by-element left division, C = B./A
        /// </summary>
        /// <param name="matrix">   another matrix
        /// </param>
        /// <returns>     B./A
        /// </returns>
        public virtual Matrix LeftDivide(Matrix matrix)
        {
            return LeftDivision(this, matrix);
        }

        /// <summary>
        /// multiply by Scaler number
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public Matrix Multiply(double scalar)
        {
            return Multiplication(this,scalar);
        }

        /// <summary>
        /// multiply by Matrix
        /// </summary>
        /// <param name="matrix">Second Matrix</param>
        /// <returns>A matrix</returns>
        public Matrix Multiply(Matrix matrix)
        {
            return Multiplication(this, matrix);
        }


        /// <summary>ScalarValue</summary>
        /// <returns></returns>
        public Double ScalarValue()
        {
            return Matrix.ScalarValue(this);
        }
        
        /// <summary>
        /// Get a sub matrix of rows and columns
        /// </summary>
        /// <param name="columnRange"></param>
        /// <param name="rowRange"></param>
        /// <returns>Returns a matrix></returns>
        public Matrix SubMatrix(Int32Range columnRange,Int32Range rowRange)
        {
            return new Matrix(MatrixBase<Double>.SubMatrix(this.InnerMatrix, columnRange, rowRange));
        }

        public Matrix SubMatrix(int columnRange, int rowRange)
        {
            return new Matrix(MatrixBase<Double>.SubMatrix(this.InnerMatrix, new ReinGametiMatrixLib.Int32Range(columnRange), new ReinGametiMatrixLib.Int32Range(rowRange)));
        }
        
        public string ToString(String strFormat = "0.0000")
        {
            return _matrix.ToString(strFormat);
        }

        public override String ToString()
        {
            return this.ToString();
        }


        #endregion Public Methods

        #region Public Static Methods
        /*************************/
        /* PUBLIC STATIC METHODS */
        /*************************/

        /// <summary>Indicates whether the specified Matrix is null or is empty (of rank 0)</summary>
        /// <param name="matrix"></param>
        /// <returns>True if specified Matrix is null, false otherwise</returns>
        public static Boolean IsNull(Matrix matrix)
        {
            return ((object)matrix == null);
        }
                
        /// <summary>
        /// Get a matrix of diagonal elements of a square matrix
        /// </summary>
        /// <returns>Return a sqaure matrix of zeros except the leading diagonals</returns>
        public static Matrix DiagAsMatrix(Matrix matrix)
        {
            if (matrix.IsEmpty) return null;
            if (!matrix.IsSquare) return null;

            var matArray = Zero(matrix.RowCount);

            for (int i = 0; i < matrix.RowCount; i++)
            {
                matArray[i, i] = matrix[i, i];
            }

            return matArray;
        }

        /// <summary>
        /// Get a matrix of diagonal elements of a square matrix
        /// </summary>
        /// <returns>>Return a sqaure matrix of zeros except the leading diagonals</returns>
        public static Matrix DiagAsMatrix(double[,] matrix)
        {
            var mat = new Matrix(matrix);
            
            return DiagAsMatrix(mat);
        }

        /// <summary>
        /// Get a vector of diagonal elements of a square matrix
        /// </summary>
        /// <returns>Return a column vector of zeros except the leading diagonals</returns>
        public static Matrix DiagAsVector(Matrix matrix)
        {
            if (matrix.IsEmpty) return null;
            if (!matrix.IsSquare) return null;

            var matArray = Zero(1, matrix.RowCount);

            for (int i = 0; i < matrix.RowCount; i++)
            {
                matArray[0, i] = matrix[i, i];
            }

            return matArray;
        }

        /// <summary>
        /// Get a vector of diagonal elements of a square matrix
        /// </summary>
        /// <returns>>Return a column vector of zeros except the leading diagonals</returns>
        public static Matrix DiagAsVector(double[,] matrix)
        {
            var mat = new Matrix(matrix);

            return DiagAsVector(mat);
        }

        /// <summary>
        /// Determinant of a matrix
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static double Determinant(Matrix matrix)
        {
            int S, k, k1, i, j;
            double[,] DArray;
            double save, ArrayK, tmpDet;
            int Rows, Cols;

            try
            {
                DArray = (double[,])matrix.ToArray();
                Rows = matrix.RowCount;
                Cols = matrix.ColumnCount;
            }
            catch { throw new MatrixNullException(); }

            if (!matrix.IsSquare) throw new MatrixNotSquare();

            S = Rows-1;
            tmpDet = 1;

            for (k = 0; k <= S; k++)
            {
                if (DArray[k, k] == 0)
                {
                    j = k;
                    while ((j < S) && (DArray[k, j] == 0)) j = j + 1;
                    if (DArray[k, j] == 0) return 0;
                    else
                    {
                        for (i = k; i <= S; i++)
                        {
                            save = DArray[i, j];
                            DArray[i, j] = DArray[i, k];
                            DArray[i, k] = save;
                        }
                    }
                    tmpDet = -tmpDet;
                }
                ArrayK = DArray[k, k];
                tmpDet = tmpDet * ArrayK;
                if (k < S)
                {
                    k1 = k + 1;
                    for (i = k1; i <= S; i++)
                    {
                        for (j = k1; j <= S; j++)
                            DArray[i, j] = DArray[i, j] - DArray[i, k] * (DArray[k, j] / ArrayK);
                    }
                }
            }

            return tmpDet;
        }

        /// <summary>
        /// Convert a matrix to double array
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns>Returns a double array</returns>
        public static double[,] Matrix2Array(Matrix matrix)
        {
            double[,] matArray = new double[matrix.RowCount, matrix.ColumnCount];

            for (int i = 0; i < matrix.RowCount; i++)
            {
                for (int j = 0; j < matrix.ColumnCount; j++)
                {
                    matArray[i, j] = matrix[j,i];
                }
            }

            return matArray;
        }

        /// <summary>
        /// Returns a matrix as a string, so it can be viewed
        /// in a multi-text textbox or in a richtextBox (preferred).
        /// In case of an error the error is raised as an exception.
        /// </summary>
        /// <param name="matrix">The array to be viewed</param>
        /// <returns>The string view of the array</returns>
        public static string PrintMatrix(double[,] matrix)
        {
            return new Matrix(matrix).ToString();
        }

        /// <summary>
        /// Returns a matrix as a string, so it can be viewed
        /// in a multi-text textbox or in a richtextBox (preferred).
        /// In case of an error the error is raised as an exception.
        /// </summary>
        /// <param name="matrix">The array to be viewed</param>
        /// <returns>The string view of the array</returns>
        public static string PrintMatrix(Matrix matrix)
        {
            return matrix.ToString();
        }


        /// <summary>Indicates whether the specified Matrix is null or is empty (of rank 0)</summary>
        /// <param name="matrix"></param>
        /// <returns>True if specified Matrix is null or empty (of rank == 0); false otherwise</returns>
            public static Boolean IsNullOrEmpty(Matrix matrix)
        {
            return ((object)matrix == null) || matrix.IsEmpty;
        }


        /// <summary>
        /// Add two matrix
        /// </summary>
        /// <param name="matrix1">Frist matrix</param>
        /// <param name="matrix2">Second Matrix</param>
        /// <returns></returns>
        protected static Matrix Addition(Matrix matrix1,Matrix matrix2)
        {
            return new Matrix(
                MatrixBase<Double>.ElementWiseOperation(
                    matrix1.InnerMatrix,
                    matrix2.InnerMatrix,
                    delegate(Double element1, Double element2)
                    { return element1 + element2; }));
            
        }


        /// <summary>
        /// Add a matrix and a scaler value
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        protected static Matrix Addition(Matrix matrix, Double scalar)
        {
            return new Matrix(
                MatrixBase<Double>.ElementWiseOperation(
                    matrix.InnerMatrix,
                    scalar,
                    delegate(Double element1, Double element2)
                    { return element1 + element2; }));
        }

        

        /// <summary>
        /// Subtract to matrix
        /// </summary>
        /// <param name="matrix1">Frist matrix</param>
        /// <param name="matrix2">Second Matrix</param>
        /// <returns></returns>
        protected static Matrix Subtraction(Matrix matrix1, Matrix matrix2)
        {
            return new Matrix(
                MatrixBase<Double>.ElementWiseOperation(
                    matrix1.InnerMatrix,
                    matrix2.InnerMatrix,
                    delegate(double element1, double element2)
                    { return element1 - element2; }));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        protected static Matrix Subtraction(Matrix matrix, double scalar)
        {
            return new Matrix(
                MatrixBase<Double>.ElementWiseOperation(
                    matrix.InnerMatrix,
                    scalar,
                    delegate(double element1, double element2)
                    { return element1 - element2; }));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        protected static Matrix RightDivision( Matrix matrix, double scalar)
        {
            return new Matrix(
                MatrixBase<Double>.ElementWiseOperation(
                    matrix.InnerMatrix,
                    scalar,
                    delegate(double element1, double element2)
                    { return element1 / element2; }));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        protected static Matrix LeftDivision(Matrix matrix, double scalar)
        {
            return new Matrix(
                MatrixBase<Double>.ElementWiseOperation(
                    matrix.InnerMatrix,
                    scalar,
                    delegate (double element1, double element2)
                    { return element2 / element1; }));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="matrix1"></param>
        /// <param name="matrix2"></param>
        /// <returns></returns>
        protected static Matrix LeftDivision(Matrix matrix1, Matrix matrix2)
        {
            return new Matrix(
                MatrixBase<Double>.ElementWiseOperation(
                    matrix1.InnerMatrix,
                    matrix2.InnerMatrix,
                    delegate (double element1, double element2)
                    { return element2 / element1; }));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="matrix1"></param>
        /// <param name="matrix2"></param>
        /// <returns></returns>
        protected static Matrix RightDivision(Matrix matrix1, Matrix matrix2)
        {
            return new Matrix(
                MatrixBase<Double>.ElementWiseOperation(
                    matrix1.InnerMatrix,
                    matrix2.InnerMatrix,
                    delegate (double element1, double element2)
                    { return element1 / element2; }));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        protected static Matrix Multiplication( Matrix matrix, double scalar)
        {
            return new Matrix(
                MatrixBase<Double>.ElementWiseOperation(
                    matrix.InnerMatrix,
                    scalar,
                    delegate(double element1, double element2)
                    { return element1 * element2; }));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="matrix1"></param>
        /// <param name="matrix2"></param>
        /// <returns></returns>
        protected static Matrix Multiplication(Matrix matrix1, Matrix matrix2)
        {
            if (matrix1.ColumnCount != matrix2.RowCount)
                throw new ArithmeticException("Number of columns in first matrix does not equal number of rows in second matrix.");

            Matrix result = new Matrix(matrix2.ColumnCount, matrix1.RowCount);

            for (int j = 0; j < result.RowCount; j++)
                for (int i = 0; i < result.ColumnCount; i++)
                {
                    Double value = 0;
                    for (int k = 0; k < matrix1.ColumnCount; k++)
                        value += matrix1[k, j] * matrix2[i, k];
                    result[i, j] = value;
                }

            return result;
        }


        /// <summary>Calculates the dot product (A . B) of two vectors</summary>
        /// <param name="vector1">First matrix (must be a row vector)</param>
        /// <param name="vector2">Second matrix (must be a column vector)</param>
        /// <returns>Returns the dot product of (matrix1 . matrix2)</returns>
        protected static Double DotProduct(Matrix vector1, Matrix vector2)
        {
            return Multiplication(vector1, vector2.Transpose()).ScalarValue();
        }


        /// <summary>ScalarValue</summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        protected static Double ScalarValue(Matrix matrix)
        {
            return matrix[0, 0];
        }

        /// <summary>Indicates whether or not two matricies contain same values
        /// in an element-wise comparison. Does not verify that the matricies point
        /// to the same underlying data-set or element instances</summary>
        /// <param name="matrix1">Matrix to compare</param>
        /// <param name="matrix2">Matrix to compare</param>
        /// <returns>true, if the matricies are equal on an element-wise basis; otherwise, false</returns>
        public static Boolean Equality(Matrix matrix1, Matrix matrix2)
        {
            if ((object)matrix1 == null || (object)matrix2 == null) return false;
            return MatrixBase<Double>.Equality(matrix1.InnerMatrix, matrix2.InnerMatrix);
        }


        /// <summary>GetHashCode</summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.InnerMatrix.GetHashCode();
        }


        /// <summary>Equals</summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override Boolean Equals(Object obj)
        {
            if (!(obj is Matrix)) return false;
            return this.InnerMatrix.Equals((obj as Matrix).InnerMatrix);
        }


        /// <summary>Generates a new identity matrix</summary>
        /// <param name="rank">Rank of the matrix (width and height)</param>
        /// <returns>Matrix of specified rank with all diagonalOffset elements set to 1, and all others set to 0</returns>
        public static Matrix Identity(int rank)
        {
            return Matrix.Identity(rank, rank);
        }

        /// <summary>Generates a new identity matrix</summary>
        /// <param name="columns">Width of matrix</param>
        /// <param name="rows">Height of matrix</param>
        /// <returns>Matrix of specified size with all diagonalOffset elements set to 1, and all others set to 0</returns>
        public static Matrix Identity(int columns, int rows)
        {
            Matrix identity = new Matrix(columns, rows);
            for (int i = 0; i < columns; i++)
                for (int j = 0; j < rows; j++)
                    identity[i, j] = (i == j ? 1 : 0);
            return identity;
        }


        /// <summary>Generates a new zero matrix</summary>
        /// <param name="rank">Rank of the matrix</param>
        /// <returns>Matrix of specified rank with all element values set to 0</returns>
        public static Matrix Zero(int rank)
        {
            return Matrix.Zero(rank, rank);
        }


        /// <summary>Generates a new zero matrix</summary>
        /// <param name="columns">Number of columns</param>
        /// <param name="rows">Number of rows</param>
        /// <returns>Matrix of specified size with all element values set to 0</returns>
        public static Matrix Zero(int columns, int rows)
        {
            Matrix zero = new Matrix(columns, rows);
            for (int i = 0; i < columns; i++)
                for (int j = 0; j < rows; j++)
                    zero[i, j] = 0;
            return zero;
        }

        public enum MatrixType
        {
            Int,
            Double
        }

       
        /// <summary>
        /// Create a square matrix of random values
        /// </summary>
        /// <param name="rank"></param>        
        /// <returns>Random square matrix of n x n</returns>
        public static Matrix Random(int rank)
        {
            var randMat = new Matrix(rank, rank);
            Random randVal = new Random();

            for (int i = 0; i < rank; i++)
            {
                for (int j = 0; j < rank; j++)
                {
                    randMat[j, i] = randVal.Next();
                }
            }

            return randMat;
        }

        /// <summary>
        /// Create a square matrix of random values
        /// </summary>
        /// <param name="rank"></param>
        /// <param name="Type">Matrix Type</param>
        /// <returns>Random square matrix of n x n</returns>
        public static Matrix Random(int rank, MatrixType Type)
        {
            var randMat = new Matrix(rank, rank);
            Random randVal = new Random();

            for (int i = 0; i < rank; i++)
            {
                for (int j = 0; j < rank; j++)
                {
                    if (Type == MatrixType.Double)
                    {
                        randMat[i, j] = _random.NextDouble();
                    }
                    else
                    {
                        randMat[i, j] = _random.Next();
                    }
                }
            }

            return randMat;
        }

        /// <summary>
        /// Create a random matrix of m x n
        /// </summary>
        /// <param name="columns">Column number</param>
        /// <param name="row">Row number</param>
        /// <returns>Matrix of specified size with random element values in range [0 .. 1]</returns>
        public static Matrix Random(int columns, int rows)
        {
            var randMat = new Matrix(columns, rows);
            Random randVal = new Random();

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    randMat[j, i] = randVal.Next();
                }
            }

            return randMat;
        }

        /// <summary>Generates a new random matrix with element values in the range [0 .. 1]</summary>
        /// <param name="columns">Width of matrix</param>
        /// <param name="rows">Height of matrix</param>
        /// <param name="Type">Matrix Type</param>
        /// <returns>Matrix of specified size with random element values in range [0 .. 1]</returns>
        public static Matrix Random(int columns, int rows, MatrixType Type)
        {
            Matrix result = new Matrix(columns, rows);
            for (int i = 0; i < columns; i++)
            {
                for (int j = 0; j < rows; j++)
                {

                    if (Type == MatrixType.Double)
                    {
                        result[i, j] = _random.NextDouble();
                    }
                    else
                    {
                        result[i, j] = _random.Next();
                    }
                }
            }
            return result;
        }

        /// <summary>Generates a new random matrix with element values in the range [0 .. 1]</summary>
        /// <param name="columns">Width of matrix</param>
        /// <param name="rows">Height of matrix</param>
        /// <param name="Type">Matrix Type</param>
        /// <param name="Factor">Multiplying Factor</param>
        /// <returns>Matrix of specified size with random element values in range [0 .. 1] * Factor</returns>
        public static Matrix Random(int columns, int rows, MatrixType Type, double Factor)
        {
            Matrix result = new Matrix(columns, rows);
            for (int i = 0; i < columns; i++)
            {
                for (int j = 0; j < rows; j++)
                {

                    if (Type == MatrixType.Double)
                    {
                        result[i, j] = _random.NextDouble() * Factor;
                    }
                    else
                    {
                        result[i, j] = _random.Next() * Factor;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Create random matrix with elements between minVal and maxVal
        /// </summary>
        /// <param name="columns">Number of columns</param>
        /// <param name="rows">Number of Rows</param>
        /// <param name="minVal">Minmum value - Int</param>
        /// <param name="maxVal">Maximum value -Int</param>
        /// <returns>Matrix of specified size with random element values /returns>
        public static Matrix Random(int columns, int rows, int minVal, int maxVal)
        {
            var randMat = new Matrix(columns, rows);
                       
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    randMat[j, i] = _random.Next(minVal, maxVal);
                }
            }

            return randMat;
        }

        /// <summary>
        /// Create random matrix with elements between minVal and maxVal
        /// </summary>
        /// <param name="columns">Number of columns</param>
        /// <param name="rows">Number of Rows</param>
        /// <param name="minVal">Minmum value - Double</param>
        /// <param name="maxVal">Maximum value -Double</param>
        public static Matrix Random(int columns, int rows, double minVal, double maxVal)
        {
            var randMat = new Matrix(columns, rows);
            
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    randMat[j, i] = _random.NextDouble() * (maxVal - minVal) + minVal;
                }
            }

            return randMat;
        }

        /// <summary>
        /// Create random matrix with elements between minVal and maxVal
        /// </summary>
        /// <param name="columns">Number of columns</param>
        /// <param name="rows">Number of Rows</param>
        /// <param name="minVal">Minmum value - Double</param>
        /// <param name="maxVal">Maximum value -Double</param>
        /// <param name="Factor">Multiplying Factor</param>
        /// <returns>Matrix of specified size with random element values in range [minVal .. maxVal] * Factor</returns>
        public static Matrix Random(int columns, int rows, double minVal, double maxVal, double Factor)
        {
            var randMat = new Matrix(columns, rows);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    randMat[j, i] = (_random.NextDouble() * (maxVal - minVal) + minVal) * Factor;
                }
            }

            return randMat;
        }



        /// <summary>Calculates the inverse of a square matrix by applying
        /// Gaussian Elimination techniques. For more information on 
        /// algorithm see http://www.ecs.fullerton.edu/~mathews/numerical/mi.htm
        /// or your high school text book.</summary>
        /// <param name="matrix">Square matrix to be inverted</param>
        /// <returns>Inverse of input matrix</returns>
        protected static Matrix Invert(Matrix matrix)
        {

            if (!matrix.IsSquare) throw new ArithmeticException("Cannot invert non-square matrix");

            // create an augmented matrix [A,I] with the input matrix I on the 
            // left hand side and the identity matrix I on the right hand side
            //
            //    [ 2 5 6 | 1 0 0 ]
            // eg [ 8 3 1 | 0 1 0 ]
            //    [ 2 9 2 | 0 0 1 ]
            //
            Matrix augmentedMatrix =
                Matrix.JoinHorizontal(new Matrix[] { matrix, Matrix.Identity(matrix.ColumnCount, matrix.RowCount) });

            for (int j1 = 0; j1 < augmentedMatrix.RowCount; j1++)
            {

                // check to see if any of the rows subsequent to i have a 
                // higher absolute value on the current diagonalOffset (i,i).
                // if so, switch them to minimize rounding errors
                //
                //    [ (2) 5  6  | 1 0 0 ]                    [ (8) 3  1  | 0 1 0 ]
                // eg [  8 (3) 1  | 0 1 0 ] -> SWAP(R1, R2) -> [  2 (5) 6  | 1 0 0 ] 
                //    [  2  9 (2) | 0 0 1 ]                    [  2  9 (2) | 0 0 1 ]
                //
                for (int j2 = j1 + 1; j2 < augmentedMatrix.RowCount; j2++)
                {
                    if (Math.Abs(augmentedMatrix[j1, j2]) > Math.Abs(augmentedMatrix[j1, j1]))
                    {
                        //Console.WriteLine("Swap [" + j2 + "] with [" + i + "]");
                        augmentedMatrix.SwapRows(j1, j2);
                    }
                }

                // normalize the row so the diagonalOffset value (i,i) is 1
                // if (i,i) is 0, this row is null (we have > 0 nullity for this matrix)
                //
                //    [ (8) 3  1  | 0 1 0 ]                   [ (1.0) 0.4  0.1 | 0.0 0.1 0.0 ]
                // eg [  2 (5) 6  | 1 0 0 ] -> R1 = R1 / 8 -> [ 2.0  (5.0) 6.0 | 1.0 0.0 0.0 ] 
                //    [  2  9 (2) | 0 0 1 ]                   [ 2.0   9.0 (2.0) | 0.0 0.0 1.0 ]  

                //Console.WriteLine("Divide [" + i + "] by " + augmentedMatrix[i, i].ToString("0.00"));
                augmentedMatrix.Rows[j1].CopyFrom(augmentedMatrix.Rows[j1] / augmentedMatrix[j1, j1]);


                // look at each pair of rows {i, r} to see if r is linearly
                // dependent on i. if r does contain some factor of i vector,
                // subtract it out to make {i, r} linearly independent
                for (int j2 = 0; j2 < augmentedMatrix.RowCount; j2++)
                {
                    if (j2 != j1)
                    {
                        //Console.WriteLine("Subtracting " + augmentedMatrix[i, j2].ToString("0.00") + " i [" + i + "] from [" + j2 + "]");
                        augmentedMatrix.Rows[j2].CopyFrom(new Matrix(augmentedMatrix.Rows[j2] - (augmentedMatrix[j1, j2] * augmentedMatrix.Rows[j1])));
                    }
                }
            }

            // separate the inverse from the right hand side of the augmented matrix
            //
            //    [ (1) 0  0  |     [ 2 5 6 ] 
            // eg [  0 (1) 0  | ~   [ 8 2 1 ] -> inverse
            //    [  0  0 (1) |     [ 5 2 2 ] 
            //
            Matrix inverse = augmentedMatrix.SubMatrix(new Int32Range(matrix.ColumnCount, matrix.ColumnCount + matrix.ColumnCount), new Int32Range(0, matrix.RowCount));
            return inverse;
        }


        /// <summary>GaussianElimination</summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static Matrix GaussianElimination(Matrix matrix)
        {
            Matrix reduced = matrix.Copy();
            
            Int32 lead = 0;
            for (Int32 row = 0; row < reduced.RowCount; row++)
            {
                if (reduced.ColumnCount <= lead)
                    break;

                Int32 i = row;
                while (reduced[lead, i] == 0)
                {
                    i++;
                    if (i == reduced.RowCount)
                    {
                        i = row;
                        lead++;
                        if (lead == reduced.ColumnCount) break;
                    }
                }
                reduced.SwapRows(i, row);
                reduced.Rows[row] = reduced.Rows[row] / reduced[lead, row];
                for (Int32 j = 0; j < reduced.RowCount; j++)
                {
                    if (j == row) continue;
                    reduced.Rows[j] = reduced.Rows[j] - reduced.Rows[row] * reduced[lead, j];
                }
            }
            return reduced;
        }
        

        /// <summary>
        /// Concatenate matrix Horizontally 
        /// </summary>
        /// <param name="matricies"></param>
        /// <returns></returns>
        public static Matrix JoinHorizontal(IEnumerable<Matrix> matricies)
        {
            List<MatrixBase<Double>> innerMatricies = new List<MatrixBase<Double>>();
            foreach (Matrix matrix in matricies)
                innerMatricies.Add(matrix.InnerMatrix);
            return new Matrix(MatrixBase<Double>.JoinHorizontal(innerMatricies));
        }


        /// <summary>
        /// Concatenate matrix Horizontally 
        /// </summary>
        /// <param name="leftMatrix"></param>
        /// <param name="rightMatrix"></param>
        /// <returns></returns>
        public static Matrix JoinHorizontal(Matrix leftMatrix, Matrix rightMatrix)
        {
            return Matrix.JoinHorizontal(new Matrix[] { leftMatrix, rightMatrix } );
        }


        /// <summary>
        /// Concatenate matrix vertically
        /// </summary>
        /// <param name="matricies"></param>
        /// <returns></returns>
        public static Matrix JoinVertical(IEnumerable<Matrix> matricies)
        {
            List<MatrixBase<Double>> innerMatricies = new List<MatrixBase<Double>>();
            foreach (Matrix matrix in matricies)
                innerMatricies.Add(matrix.InnerMatrix);
            return new Matrix(MatrixBase<Double>.JoinVertical(innerMatricies));
        }

        /// <summary>
        /// Convert an array to matrix
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static Matrix Array2Matrix(double[,] array)
        {            
            return new Matrix(array);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="topMatrix"></param>
        /// <param name="bottomMatrix"></param>
        /// <returns></returns>
        public static Matrix JoinVertical(Matrix topMatrix, Matrix bottomMatrix)
        {
            return Matrix.JoinVertical(new Matrix[] { topMatrix, bottomMatrix });
        }

        #region "Solve system of linear equations"	
        /// <summary>
        /// Solves a set of n linear equations A.X = B, and returns
        /// X, where A is [n,n] and B is [n,1]. 
        /// In the same manner if you need to compute: inverse(A).B, it is
        /// better to use this method instead, as it is much faster.  
        /// In case of an error the error is raised as an exception. 
        /// Note: This method is based on the 'LU Decomposition and Its Applications'
        /// section of Numerical Recipes in C by William H. Press,
        /// Saul A. Teukolsky, William T. Vetterling and Brian P. Flannery,
        /// University of Cambridge Press 1992.
        /// </summary>
        /// <param name="MatA">Matrix object 'A' on the left side of the equations A.X = B</param>
        /// <param name="MatB">Matrix object 'B' on the right side of the equations A.X = B</param>
        /// <returns>Matrix object 'X' in the system of equations A.X = B</returns>
        public static Matrix Solve(Matrix MatA, Matrix MatB)
        {
            return new Matrix(Matrix.Solve(MatA.ToArray(), MatB.ToArray()));
        }

        /// <summary>
        /// Solves a set of n linear equations A.X = B, and returns
        /// X, where A is [n,n] and B is [n,1]. 
        /// In the same manner if you need to compute: inverse(A).B, it is
        /// better to use this method instead, as it is much faster.  
        /// In case of an error the error is raised as an exception. 
        /// Note: This method is based on the 'LU Decomposition and Its Applications'
        /// section of Numerical Recipes in C by William H. Press,
        /// Saul A. Teukolsky, William T. Vetterling and Brian P. Flannery,
        /// University of Cambridge Press 1992.
        /// </summary>
        /// <param name="MatA">The array 'A' on the left side of the equations A.X = B</param>
        /// <param name="MatB">The array 'B' on the right side of the equations A.X = B</param>
        /// <returns>Array 'X' in the system of equations A.X = B</returns>
        public static double[,] Solve(double[,] MatA, double[,] MatB)
        {
            if (MatA.GetUpperBound(0) + 1 != MatB.GetUpperBound(0) + 1)
            {
                throw new System.ArgumentException("Matrix row dimensions must agree.");
            }
            if (MatA.GetUpperBound(0) + 1 != MatA.GetUpperBound(1) + 1)
            {
                throw new System.SystemException("Matrix is not symmetric positive definite.");
            }

            double[,] A;
            double[,] B;
            double SUM;
            int i, ii, j, k, ll, Rows, Cols;

            #region "LU Decompose"
            try
            {
                A = (double[,])MatA.Clone();
                B = (double[,])MatB.Clone();
                Rows = MatA.GetUpperBound(0) + 1;
                Cols = MatA.GetUpperBound(1) + 1;
            }
            catch { throw new MatrixNullException(); }

            if (Rows != Cols) throw new MatrixNotSquare();
            if ((B.GetUpperBound(0) + 1 != Rows ) || (B.GetUpperBound(1) != 0))
                throw new MatrixDimensionException();

            int IMAX = 0, N = Rows-1;
            double AAMAX, Sum, Dum, TINY = 1E-20;

            int[] INDX = new int[N + 1];
            double[] VV = new double[N * 10];
            double D = 1.0;

            for (i = 0; i <= N; i++)
            {
                AAMAX = 0.0;
                for (j = 0; j <= N; j++)
                    if (Math.Abs(A[i, j]) > AAMAX) AAMAX = Math.Abs(A[i, j]);
                if (AAMAX == 0.0) throw new MatrixSingularException();
                VV[i] = 1.0 / AAMAX;
            }
            for (j = 0; j <= N; j++)
            {
                if (j > 0)
                {
                    for (i = 0; i <= (j - 1); i++)
                    {
                        Sum = A[i, j];
                        if (i > 0)
                        {
                            for (k = 0; k <= (i - 1); k++)
                                Sum = Sum - A[i, k] * A[k, j];
                            A[i, j] = Sum;
                        }
                    }
                }
                AAMAX = 0.0;
                for (i = j; i <= N; i++)
                {
                    Sum = A[i, j];
                    if (j > 0)
                    {
                        for (k = 0; k <= (j - 1); k++)
                            Sum = Sum - A[i, k] * A[k, j];
                        A[i, j] = Sum;
                    }
                    Dum = VV[i] * Math.Abs(Sum);
                    if (Dum >= AAMAX)
                    {
                        IMAX = i;
                        AAMAX = Dum;
                    }
                }
                if (j != IMAX)
                {
                    for (k = 0; k <= N; k++)
                    {
                        Dum = A[IMAX, k];
                        A[IMAX, k] = A[j, k];
                        A[j, k] = Dum;
                    }
                    D = -D;
                    VV[IMAX] = VV[j];
                }
                INDX[j] = IMAX;
                if (j != N)
                {
                    if (A[j, j] == 0.0) A[j, j] = TINY;
                    Dum = 1.0 / A[j, j];
                    for (i = j + 1; i <= N; i++)
                        A[i, j] = A[i, j] * Dum;

                }
            }
            if (A[N, N] == 0.0) A[N, N] = TINY;
            #endregion

            ii = -1;
            for (i = 0; i <= N; i++)
            {
                ll = INDX[i];
                SUM = B[ll, 0];
                B[ll, 0] = B[i, 0];
                if (ii != -1)
                {
                    for (j = ii; j <= i - 1; j++) SUM = SUM - A[i, j] * B[j, 0];
                }
                else if (SUM != 0) ii = i;
                B[i, 0] = SUM;
            }
            for (i = N; i >= 0; i--)
            {
                SUM = B[i, 0];
                if (i < N)
                {
                    for (j = i + 1; j <= N; j++) SUM = SUM - A[i, j] * B[j, 0];
                }
                B[i, 0] = SUM / A[i, i];
            }
            return B;
        }


        #endregion

        #region "Singular Value Decomposition of a Matrix"
        /// <summary>
        /// Evaluates the Singular Value Decomposition of a matrix, 
        /// returns the  matrices S, U and V. Such that a given
        /// Matrix = U x S x V'.
        /// In case of an error the error is raised as an exception. 
        /// Note: This method is based on the 'Singular Value Decomposition'
        /// section of Numerical Recipes in C by William H. Press,
        /// Saul A. Teukolsky, William T. Vetterling and Brian P. Flannery,
        /// University of Cambridge Press 1992.
        /// </summary>
        /// <param name="matrix">Matrix object whose SVD is to be computed</param>
        /// <param name="S">A Matrix object where the S matrix is returned</param>
        /// <param name="U">A Matrix object where the U matrix is returned</param>
        /// <param name="V">A Matrix object where the V matrix is returned</param>
        public static void SVD(Matrix matrix, out Matrix S, out Matrix U, out Matrix V)
        {
            double[,] s, u, v;
            SVD(matrix.ToArray(), out s, out u, out v);
            S = new Matrix(s);
            U = new Matrix(u);
            V = new Matrix(v);
        }

        /// <summary>
        /// Evaluates the Singular Value Decomposition of a matrix, 
        /// returns the  matrices S, U and V. Such that a given
        /// Matrix = U x S x V'.
        /// In case of an error the error is raised as an exception. 
        /// Note: This method is based on the 'Singular Value Decomposition'
        /// section of Numerical Recipes in C by William H. Press,
        /// Saul A. Teukolsky, William T. Vetterling and Brian P. Flannery,
        /// University of Cambridge Press 1992.  
        /// </summary>
        /// <param name="Mat_">Array whose SVD is to be computed</param>
        /// <param name="S_">An array where the S matrix is returned</param>
        /// <param name="U_">An array where the U matrix is returned</param>
        /// <param name="V_">An array where the V matrix is returned</param>
        public static void SVD(double[,] Mat_, out double[,] S_, out double[,] U_, out double[,] V_)
        {            
            int Rows, Cols;
            int m, MP, n, NP;
            double[] w;
            double[,] A, v;

            try
            {
                Rows = Mat_.GetUpperBound(0) + 1;
                Cols = Mat_.GetUpperBound(1) + 1;
            }
            catch
            {
                throw new MatrixNullException();
            }

            m = Rows + 1;
            n = Cols + 1;

            if (m < n)
            {
                m = n;
                MP = NP = n;
            }
            else if (m > n)
            {
                n = m;
                NP = MP = m;
            }
            else
            {
                MP = m;
                NP = n;
            }

            A = new double[m + 1, n + 1];

            for (int row = 1; row < Rows + 1; row++)
                for (int col = 1; col < Cols + 1; col++)
                { A[row, col] = Mat_[row - 1, col - 1]; }

            const int NMAX = 100;
            v = new double[NP + 1, NP + 1];
            w = new double[NP + 1];

            int k, l, nm;
            int flag, i, its, j, jj;

            double[,] U_temp, S_temp, V_temp;
            double anorm, c, f, g, h, s, scale, x, y, z;
            double[] rv1 = new double[NMAX];

            l = 0;
            nm = 0;
            g = 0.0;
            scale = 0.0;
            anorm = 0.0;

            for (i = 1; i <= n; i++)
            {
                l = i + 1;
                rv1[i] = scale * g;
                g = s = scale = 0.0;
                if (i <= m)
                {
                    for (k = i; k <= m; k++) scale += Math.Abs(A[k, i]);
                    if (scale != 0)
                    {
                        for (k = i; k <= m; k++)
                        {
                            A[k, i] /= scale;
                            s += A[k, i] * A[k, i];
                        }
                        f = A[i, i];
                        g = -Sign(Math.Sqrt(s), f);
                        h = f * g - s;
                        A[i, i] = f - g;
                        if (i != n)
                        {
                            for (j = l; j <= n; j++)
                            {
                                for (s = 0, k = i; k <= m; k++) s += A[k, i] * A[k, j];
                                f = s / h;
                                for (k = i; k <= m; k++) A[k, j] += f * A[k, i];
                            }
                        }
                        for (k = i; k <= m; k++) A[k, i] *= scale;
                    }
                }
                w[i] = scale * g;
                g = s = scale = 0.0;
                if (i <= m && i != n)
                {
                    for (k = l; k <= n; k++) scale += Math.Abs(A[i, k]);
                    if (scale != 0)
                    {
                        for (k = l; k <= n; k++)
                        {
                            A[i, k] /= scale;
                            s += A[i, k] * A[i, k];
                        }
                        f = A[i, l];
                        g = -Sign(Math.Sqrt(s), f);
                        h = f * g - s;
                        A[i, l] = f - g;
                        for (k = l; k <= n; k++) rv1[k] = A[i, k] / h;
                        if (i != m)
                        {
                            for (j = l; j <= m; j++)
                            {
                                for (s = 0.0, k = l; k <= n; k++) s += A[j, k] * A[i, k];
                                for (k = l; k <= n; k++) A[j, k] += s * rv1[k];
                            }
                        }
                        for (k = l; k <= n; k++) A[i, k] *= scale;
                    }
                }
                anorm = Math.Max(anorm, (Math.Abs(w[i]) + Math.Abs(rv1[i])));
            }
            for (i = n; i >= 1; i--)
            {
                if (i < n)
                {
                    if (g != 0)
                    {
                        for (j = l; j <= n; j++)
                            v[j, i] = (A[i, j] / A[i, l]) / g;
                        for (j = l; j <= n; j++)
                        {
                            for (s = 0.0, k = l; k <= n; k++) s += A[i, k] * v[k, j];
                            for (k = l; k <= n; k++) v[k, j] += s * v[k, i];
                        }
                    }
                    for (j = l; j <= n; j++) v[i, j] = v[j, i] = 0.0;
                }
                v[i, i] = 1.0;
                g = rv1[i];
                l = i;
            }
            for (i = n; i >= 1; i--)
            {
                l = i + 1;
                g = w[i];
                if (i < n)
                    for (j = l; j <= n; j++) A[i, j] = 0.0;
                if (g != 0)
                {
                    g = 1.0 / g;
                    if (i != n)
                    {
                        for (j = l; j <= n; j++)
                        {
                            for (s = 0.0, k = l; k <= m; k++) s += A[k, i] * A[k, j];
                            f = (s / A[i, i]) * g;
                            for (k = i; k <= m; k++) A[k, j] += f * A[k, i];
                        }
                    }
                    for (j = i; j <= m; j++) A[j, i] *= g;
                }
                else
                {
                    for (j = i; j <= m; j++) A[j, i] = 0.0;
                }
                ++A[i, i];
            }
            for (k = n; k >= 1; k--)
            {
                for (its = 1; its <= 30; its++)
                {
                    flag = 1;
                    for (l = k; l >= 1; l--)
                    {
                        nm = l - 1;
                        if (Math.Abs(rv1[l]) + anorm == anorm)
                        {
                            flag = 0;
                            break;
                        }
                        if (Math.Abs(w[nm]) + anorm == anorm) break;
                    }
                    if (flag != 0)
                    {
                        c = 0.0;
                        s = 1.0;
                        for (i = l; i <= k; i++)
                        {
                            f = s * rv1[i];
                            if (Math.Abs(f) + anorm != anorm)
                            {
                                g = w[i];
                                h = PYTHAG(f, g);
                                w[i] = h;
                                h = 1.0 / h;
                                c = g * h;
                                s = (-f * h);
                                for (j = 1; j <= m; j++)
                                {
                                    y = A[j, nm];
                                    z = A[j, i];
                                    A[j, nm] = y * c + z * s;
                                    A[j, i] = z * c - y * s;
                                }
                            }
                        }
                    }
                    z = w[k];
                    if (l == k)
                    {
                        if (z < 0.0)
                        {
                            w[k] = -z;
                            for (j = 1; j <= n; j++) v[j, k] = (-v[j, k]);
                        }
                        break;
                    }
                    if (its == 30) Console.WriteLine("No convergence in 30 SVDCMP iterations");
                    x = w[l];
                    nm = k - 1;
                    y = w[nm];
                    g = rv1[nm];
                    h = rv1[k];
                    f = ((y - z) * (y + z) + (g - h) * (g + h)) / (2.0 * h * y);
                    g = PYTHAG(f, 1.0);
                    f = ((x - z) * (x + z) + h * ((y / (f + Sign(g, f))) - h)) / x;
                    c = s = 1.0;
                    for (j = l; j <= nm; j++)
                    {
                        i = j + 1;
                        g = rv1[i];
                        y = w[i];
                        h = s * g;
                        g = c * g;
                        z = PYTHAG(f, h);
                        rv1[j] = z;
                        c = f / z;
                        s = h / z;
                        f = x * c + g * s;
                        g = g * c - x * s;
                        h = y * s;
                        y = y * c;
                        for (jj = 1; jj <= n; jj++)
                        {
                            x = v[jj, j];
                            z = v[jj, i];
                            v[jj, j] = x * c + z * s;
                            v[jj, i] = z * c - x * s;
                        }
                        z = PYTHAG(f, h);
                        w[j] = z;
                        if (z != 0)
                        {
                            z = 1.0 / z;
                            c = f * z;
                            s = h * z;
                        }
                        f = (c * g) + (s * y);
                        x = (c * y) - (s * g);
                        for (jj = 1; jj <= m; jj++)
                        {
                            y = A[jj, j];
                            z = A[jj, i];
                            A[jj, j] = y * c + z * s;
                            A[jj, i] = z * c - y * s;
                        }
                    }
                    rv1[l] = 0.0;
                    rv1[k] = f;
                    w[k] = x;
                }
            }

            S_temp = new double[NP, NP];
            V_temp = new double[NP, NP];
            U_temp = new double[MP, NP];

            for (i = 1; i <= NP; i++) S_temp[i - 1, i - 1] = w[i];

            S_ = S_temp;

            for (i = 1; i <= NP; i++)
                for (j = 1; j <= NP; j++) V_temp[i - 1, j - 1] = v[i, j];

            V_ = V_temp;

            for (i = 1; i <= MP; i++)
                for (j = 1; j <= NP; j++) U_temp[i - 1, j - 1] = A[i, j];

            U_ = U_temp;
        }

        private static double SQR(double a)
        {
            return a * a;
        }

        private static double Sign(double a, double b)
        {
            if (b >= 0.0) { return Math.Abs(a); }
            else { return -Math.Abs(a); }
        }

        private static double PYTHAG(double a, double b)
        {
            double absa, absb;

            absa = Math.Abs(a);
            absb = Math.Abs(b);
            if (absa > absb) return absa * Math.Sqrt(1.0 + SQR(absb / absa));
            else return (absb == 0.0 ? 0.0 : absb * Math.Sqrt(1.0 + SQR(absa / absb)));
        }

        
        #endregion

        #region "LU Decomposition of a matrix"
        /// <summary>
        /// Returns the LU Decomposition of a matrix. 
        /// the output is: lower triangular matrix L, upper
        /// triangular matrix U, and permutation matrix P so that
        ///	P*X = L*U.
        /// In case of an error the error is raised as an exception. 
        /// Note: This method is based on the 'LU Decomposition and Its Applications'
        /// section of Numerical Recipes in C by William H. Press,
        /// Saul A. Teukolsky, William T. Vetterling and Brian P. Flannery,
        /// University of Cambridge Press 1992.  
        /// </summary>
        /// <param name="Mat">Matrix object which will be LU Decomposed</param>
        /// <param name="L">A Matrix object where the lower traingular matrix is returned</param>
        /// <param name="U">A Matrix object where the upper traingular matrix is returned</param>
        /// <param name="P">A Matrix object where the permutation matrix is returned</param>
        public static void LU(Matrix Mat, out Matrix L, out Matrix U, out Matrix P)
        {
            double[,] l, u, p;
            LU(Mat.ToArray(), out l, out u, out p);
            L = new Matrix(l);
            U = new Matrix(u);
            P = new Matrix(p);
        }

        /// <summary>
        /// Returns the LU Decomposition of a matrix. 
        /// the output is: lower triangular matrix L, upper
        /// triangular matrix U, and permutation matrix P so that
        ///	P*X = L*U.
        /// In case of an error the error is raised as an exception.
        /// Note: This method is based on the 'LU Decomposition and Its Applications'
        /// section of Numerical Recipes in C by William H. Press,
        /// Saul A. Teukolsky, William T. Vetterling and Brian P. Flannery,
        /// University of Cambridge Press 1992.  
        /// </summary>
        /// <param name="Mat">Array which will be LU Decomposed</param>
        /// <param name="L">An array where the lower traingular matrix is returned</param>
        /// <param name="U">An array where the upper traingular matrix is returned</param>
        /// <param name="P">An array where the permutation matrix is returned</param>
        public static void LU(double[,] Mat, out double[,] L, out double[,] U, out double[,] P)
        {
            double[,] A;
            int i, j, k, Rows, Cols;

            try
            {
                A = (double[,])Mat.Clone();
                Rows = Mat.GetUpperBound(0) + 1;
                Cols = Mat.GetUpperBound(1) + 1;
            }
            catch { throw new MatrixNullException(); }

            if (Rows != Cols) throw new MatrixNotSquare();

            int IMAX = 0, N = Rows;
            double AAMAX, Sum, Dum, TINY = 1E-20;

            int[] INDX = new int[N + 1];
            double[] VV = new double[N * 10];
            double D = 1.0;

            for (i = 0; i <= N; i++)
            {
                AAMAX = 0.0;
                for (j = 0; j <= N; j++)
                    if (Math.Abs(A[i, j]) > AAMAX) AAMAX = Math.Abs(A[i, j]);
                if (AAMAX == 0.0) throw new MatrixSingularException();
                VV[i] = 1.0 / AAMAX;
            }
            for (j = 0; j <= N; j++)
            {
                if (j > 0)
                {
                    for (i = 0; i <= (j - 1); i++)
                    {
                        Sum = A[i, j];
                        if (i > 0)
                        {
                            for (k = 0; k <= (i - 1); k++)
                                Sum = Sum - A[i, k] * A[k, j];
                            A[i, j] = Sum;
                        }
                    }
                }
                AAMAX = 0.0;
                for (i = j; i <= N; i++)
                {
                    Sum = A[i, j];
                    if (j > 0)
                    {
                        for (k = 0; k <= (j - 1); k++)
                            Sum = Sum - A[i, k] * A[k, j];
                        A[i, j] = Sum;
                    }
                    Dum = VV[i] * Math.Abs(Sum);
                    if (Dum >= AAMAX)
                    {
                        IMAX = i;
                        AAMAX = Dum;
                    }
                }
                if (j != IMAX)
                {
                    for (k = 0; k <= N; k++)
                    {
                        Dum = A[IMAX, k];
                        A[IMAX, k] = A[j, k];
                        A[j, k] = Dum;
                    }
                    D = -D;
                    VV[IMAX] = VV[j];
                }
                INDX[j] = IMAX;
                if (j != N)
                {
                    if (A[j, j] == 0.0) A[j, j] = TINY;
                    Dum = 1.0 / A[j, j];
                    for (i = j + 1; i <= N; i++)
                        A[i, j] = A[i, j] * Dum;

                }
            }

            if (A[N, N] == 0.0) A[N, N] = TINY;

            int count = 0;
            double[,] l = new double[N + 1, N + 1];
            double[,] u = new double[N + 1, N + 1];

            for (i = 0; i <= N; i++)
            {
                for (j = 0; j <= count; j++)
                {
                    if (i != 0) l[i, j] = A[i, j];
                    if (i == j) l[i, j] = 1.0;
                    u[N - i, N - j] = A[N - i, N - j];
                }
                count++;
            }

            L = l;
            U = u;

            P = Identity(N + 1).ToArray();

            for (i = 0; i <= N; i++)
            {
                SwapRows(P, i, INDX[i]);
            }
        }

        private static void SwapRows(double[,] Mat, int Row, int toRow)
        {
            int N = Mat.GetUpperBound(0);
            double[,] dumArray = new double[1, N + 1];
            for (int i = 0; i <= N; i++)
            {
                dumArray[0, i] = Mat[Row, i];
                Mat[Row, i] = Mat[toRow, i];
                Mat[toRow, i] = dumArray[0, i];
            }
        }


        #endregion

        #region "Rank of a matrix"
        /// <summary>
        /// Returns the rank of a matrix.
        /// In case of an error the error is raised as an exception. 
        /// </summary>
        /// <param name="Mat">a Matrix object whose rank is to be found</param>
        /// <returns>The rank of the Matrix object</returns>
        public static int Rank(Matrix Mat)
        { return Rank(Mat.ToArray()); }

        /// <summary>
        /// Returns the rank of a matrix.
        /// In case of an error the error is raised as an exception. 
        /// </summary>
        /// <param name="Mat">An array whose rank is to be found</param>
        /// <returns>The rank of the array</returns>
        public static int Rank(double[,] Mat)
        {
            int r = 0;
            double[,] S, U, V;
            try
            {
                int Rows, Cols;
                Rows = Mat.GetUpperBound(0) + 1;
                Cols = Mat.GetUpperBound(1) + 1;
            }
            catch { throw new MatrixNullException(); }
            double EPS = 2.2204E-16;
            SVD(Mat, out S, out U, out V);

            for (int i = 0; i <= S.GetUpperBound(0); i++)
            { if (Math.Abs(S[i, i]) > EPS) r++; }

            return r;
        }


        #endregion

        #region "Pseudoinverse of a matrix"
        /// <summary>
        /// Returns the pseudoinverse of a matrix, such that
        /// X = PseudoInverse(A) produces a matrix 'X' of the same dimensions
        /// as A' so that A*X*A = A, X*A*X = X.
        /// In case of an error the error is raised as an exception.
        /// </summary>
        /// <param name="matrix">An array whose pseudoinverse is to be found</param>
        /// <returns></returns>
        public static Matrix PseudoInverse(Matrix matrix)
        {
            //                   (  T      )-1     T
            //  Pseudo inverse = ( A  i  A )   i  A
            //                   (         )
            return (matrix.Transpose() * matrix).Inverse() * matrix.Transpose();
        }
        /// <summary>
        /// Returns the pseudoinverse of a matrix, such that
        /// X = PseudoInverse(A) produces a matrix 'X' of the same dimensions
        /// as A' so that A*X*A = A, X*A*X = X.
        /// In case of an error the error is raised as an exception.
        /// </summary>
        /// <param name="matrix">An array whose pseudoinverse is to be found</param>
        /// <returns>The pseudoinverse of the array as an array</returns>
        public static Matrix PseudoInverse(double[,] matrix)
        {

            return PseudoInverse(new Matrix(matrix));
        }

        #endregion

        #region "Eigen Values and Vactors of Symmetric Matrix"

        /// <summary>
        /// Returns the Eigenvalues and Eigenvectors of a real symmetric
        /// matrix, which is of dimensions [n,n]. In case of an error the
        /// error is raised as an exception.
        /// Note: This method is based on the 'Eigenvalues and Eigenvectors of a TridiagonalMatrix'
        /// section of Numerical Recipes in C by William H. Press,
        /// Saul A. Teukolsky, William T. Vetterling and Brian P. Flannery,
        /// University of Cambridge Press 1992.
        /// </summary>
        /// <param name="Mat">
        /// The Matrix object whose Eigenvalues and Eigenvectors are to be found
        /// </param>
        /// <param name="d">A Matrix object where the eigenvalues are returned</param>
        /// <param name="v">A Matrix object where the eigenvectors are returned</param>
        public static void Eigen(Matrix Mat, out Matrix d, out Matrix v)
        {
            double[,] D, V;
            Eigen(Mat.ToArray(), out D, out V);
            d = new Matrix(D);
            v = new Matrix(V);
        }

        public static void EigenValues(Matrix Mat, out Matrix EigenValue)
        {
            double[,] D, V;
            Eigen(Mat.ToArray(), out D, out V);
            EigenValue = new Matrix(D);           
        }

        public static void EigenVectors(Matrix Mat, out Matrix EigenVector)
        {
            double[,] D, V;
            Eigen(Mat.ToArray(), out D, out V);            
            EigenVector = new Matrix(V);
        }


        /// <summary>
        /// Returns the Eigenvalues and Eigenvectors of a real symmetric
        /// matrix, which is of dimensions [n,n]. 
        /// In case of an error the error is raised as an exception.
        /// Note: This method is based on the 'Eigenvalues and Eigenvectors of a TridiagonalMatrix'
        /// section of Numerical Recipes in C by William H. Press,
        /// Saul A. Teukolsky, William T. Vetterling and Brian P. Flannery,
        /// University of Cambridge Press 1992.
        /// </summary>
        /// <param name="Mat">
        /// The array whose Eigenvalues and Eigenvectors are to be found
        /// </param>
        /// <param name="d">An array where the eigenvalues are returned</param>
        /// <param name="v">An array where the eigenvectors are returned</param>
        public static void Eigen(double[,] Mat, out double[,] d, out double[,] v)
        {

            double[,] a;
            int Rows, Cols;
            try
            {
                Rows = Mat.GetUpperBound(0) ;
                Cols = Mat.GetUpperBound(1) ;
                a = (double[,])Mat.Clone();
            }
            catch { throw new MatrixNullException(); }

            if (Rows != Cols) throw new MatrixNotSquare();

            int j, iq, ip, i, n, nrot;
            double tresh, theta, tau, t, sm, s, h, g, c;
            double[] b, z;

            n = Rows;
            d = new double[n + 1, 1];
            v = new double[n + 1, n + 1];

            b = new double[n + 1];
            z = new double[n + 1];
            for (ip = 0; ip <= n; ip++)
            {
                for (iq = 0; iq <= n; iq++) v[ip, iq] = 0.0;
                v[ip, ip] = 1.0;
            }
            for (ip = 0; ip <= n; ip++)
            {
                b[ip] = d[ip, 0] = a[ip, ip];
                z[ip] = 0.0;
            }

            nrot = 0;
            for (i = 0; i <= 50; i++)
            {
                sm = 0.0;
                for (ip = 0; ip <= n - 1; ip++)
                {
                    for (iq = ip + 1; iq <= n; iq++)
                        sm += Math.Abs(a[ip, iq]);
                }
                if (sm == 0.0)
                {
                    return;
                }
                if (i < 4)
                    tresh = 0.2 * sm / (n * n);
                else
                    tresh = 0.0;
                for (ip = 0; ip <= n - 1; ip++)
                {
                    for (iq = ip + 1; iq <= n; iq++)
                    {
                        g = 100.0 * Math.Abs(a[ip, iq]);
                        if (i > 4 && (double)(Math.Abs(d[ip, 0]) + g) == (double)Math.Abs(d[ip, 0])
                            && (double)(Math.Abs(d[iq, 0]) + g) == (double)Math.Abs(d[iq, 0]))
                            a[ip, iq] = 0.0;
                        else if (Math.Abs(a[ip, iq]) > tresh)
                        {
                            h = d[iq, 0] - d[ip, 0];
                            if ((double)(Math.Abs(h) + g) == (double)Math.Abs(h))
                                t = (a[ip, iq]) / h;
                            else
                            {
                                theta = 0.5 * h / (a[ip, iq]);
                                t = 1.0 / (Math.Abs(theta) + Math.Sqrt(1.0 + theta * theta));
                                if (theta < 0.0) t = -t;
                            }
                            c = 1.0 / Math.Sqrt(1 + t * t);
                            s = t * c;
                            tau = s / (1.0 + c);
                            h = t * a[ip, iq];
                            z[ip] -= h;
                            z[iq] += h;
                            d[ip, 0] -= h;
                            d[iq, 0] += h;
                            a[ip, iq] = 0.0;
                            for (j = 0; j <= ip - 1; j++)
                            {
                                ROT(g, h, s, tau, a, j, ip, j, iq);
                            }
                            for (j = ip + 1; j <= iq - 1; j++)
                            {
                                ROT(g, h, s, tau, a, ip, j, j, iq);
                            }
                            for (j = iq + 1; j <= n; j++)
                            {
                                ROT(g, h, s, tau, a, ip, j, iq, j);
                            }
                            for (j = 0; j <= n; j++)
                            {
                                ROT(g, h, s, tau, v, j, ip, j, iq);
                            }
                            ++(nrot);
                        }
                    }
                }
                for (ip = 0; ip <= n; ip++)
                {
                    b[ip] += z[ip];
                    d[ip, 0] = b[ip];
                    z[ip] = 0.0;
                }
            }
            Console.WriteLine("Too many iterations in routine jacobi");
        }

        private static void ROT(double g, double h, double s, double tau,
            double[,] a, int i, int j, int k, int l)
        {
            g = a[i, j]; h = a[k, l];
            a[i, j] = g - s * (h + g * tau);
            a[k, l] = h + s * (g - h * tau);
        }


        #endregion

        #endregion Public Static Methods

        #region Public Operator Overloads
        /*****************************/
        /* PUBLIC OPERATOR OVERLOADS */
        /*****************************/

        public static Matrix operator +(Matrix matrix1, Matrix matrix2)
        {
            return Matrix.Addition(matrix1, matrix2);
        }

        public static Matrix operator +(Matrix matrix, double scalar)
        {
            return Matrix.Addition(matrix, scalar);
        }

        public static Matrix operator +(double scalar, Matrix matrix)
        {
            return Matrix.Addition(matrix, scalar);
        }

        public static Matrix operator -(Matrix matrix1, Matrix matrix2)
        {
            return Matrix.Subtraction(matrix1, matrix2);
        }

        public static Matrix operator -(Matrix matrix, double scalar)
        {
            return Matrix.Subtraction(matrix, scalar);
        }

        public static Matrix operator *(Matrix matrix1, Matrix matrix2)
        {
            return Matrix.Multiplication(matrix1, matrix2);
        }

        public static Matrix operator *(double scalar, Matrix matrix)
        {
            return Matrix.Multiplication(matrix, scalar);
        }

        public static Matrix operator *(Matrix matrix, double scalar)
        {
            return Matrix.Multiplication(matrix, scalar);
        }

        public static Matrix operator /(Matrix matrix, double scalar)
        {
            return Matrix.RightDivision(matrix, scalar);
        }

        public static Matrix operator /(double scalar, Matrix matrix)
        {
            return Matrix.RightDivision(matrix, scalar);
        }

        public static Matrix operator /(Matrix matrix1, Matrix matrix2)
        {
            return Matrix.RightDivision(matrix1, matrix2);
        }

        public static bool operator ==(Matrix matrix1, Matrix matrix2)
        {
            return Matrix.Equality(matrix1, matrix2);
        }

        public static bool operator !=(Matrix matrix1, Matrix matrix2)
        {
            return !Matrix.Equality(matrix1, matrix2);
        }

        #endregion Public Operator Overloads

        #region Public Implicit Operators
        /*****************************/
        /* PUBLIC IMPLICIT OPERATORS */
        /*****************************/

        /// <summary>Matrix</summary>
        /// <param name="Matrix"></param>
        /// <returns></returns>
/*        public static implicit operator Matrix(MatrixBase<Double> Matrix)
        {
            return new Matrix(Matrix);
        }
        */

        /// <summary>Matrix</summary>
        /// <param name="Matrix"></param>
        /// <returns></returns>
        public static implicit operator MatrixBase<Double>(Matrix Matrix)
        {
            return Matrix.InnerMatrix;
        }


        /// <summary>Matrix</summary>
        /// <param name="dataArray"></param>
        /// <returns></returns>
        public static implicit operator Matrix(Double[,] dataArray)
        {
            return new Matrix(dataArray);
        }

        #endregion Public Implicit Operators

    }
}
