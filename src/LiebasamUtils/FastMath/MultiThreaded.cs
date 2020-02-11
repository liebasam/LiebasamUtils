using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace LiebasamUtils
{
    public static partial class FastMath
    {
        #region Addition
        /// <summary>
        /// Adds together two arrays and returns the result. This operation is done in parallel when possible.
        /// </summary>
        /// <exception cref="NotSupportedException"><typeparamref name="T"/> does not support addition.</exception>
        /// <exception cref="IndexOutOfRangeException">The input arrays have different lengths.</exception>
        public static T[] AddPar<T>(T[] lhs, T[] rhs) where T : struct
        {
            if (!CanAdd<T>())
                throw new NotSupportedException(string.Format(NotSupported, typeof(T).FullName));
            if (lhs is null)
                throw new ArgumentNullException(nameof(lhs));
            if (rhs is null)
                throw new ArgumentNullException(nameof(rhs));
            if (lhs.Length != rhs.Length)
                throw new IndexOutOfRangeException(InvalidInputLength);
            T[] ans = new T[lhs.Length];
            AddPar_NoChecks(lhs, 0, rhs, 0, ans, 0, lhs.Length);
            return ans;
        }

        /// <summary>
        /// Adds together two arrays, placing the result in <paramref name="ans"/>.
        /// </summary>
        /// <exception cref="NotSupportedException"><typeparamref name="T"/> does not support addition.</exception>
        /// <exception cref="IndexOutOfRangeException">The input arrays have different lengths.</exception>
        public static void AddPar<T>(T[] lhs, T[] rhs, T[] ans) where T : struct
        {
            if (!CanAdd<T>())
                throw new NotSupportedException(string.Format(NotSupported, typeof(T).FullName));
            if (lhs is null)
                throw new ArgumentNullException(nameof(lhs));
            if (rhs is null)
                throw new ArgumentNullException(nameof(rhs));
            if (ans is null)
                throw new ArgumentNullException(nameof(ans));
            if (lhs.Length != rhs.Length || lhs.Length != ans.Length)
                throw new IndexOutOfRangeException(InvalidInputLength);
            AddPar_NoChecks(lhs, 0, rhs, 0, ans, 0, lhs.Length);
        }

        /// <summary>
        /// Adds together <paramref name="length"/> elements in two arrays,
        /// starting at the specified indices, placing the result in 
        /// <paramref name="ans"/> at the specified index.
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="lhsIndex">Starting index of lhs.</param>
        /// <param name="rhs"></param>
        /// <param name="rhsIndex">Starting index of rhs.</param>
        /// <param name="ans"></param>
        /// <param name="ansIndex">Starting index of ans.</param>
        /// <param name="length">Number of entries to operate on.</param>
        /// <exception cref="NotSupportedException"><typeparamref name="T"/> does not support addition.</exception>
        /// <exception cref="IndexOutOfRangeException">Any indices or <paramref name="length"/> are less than 0, or invalid.</exception>
        public static void AddPar<T>(T[] lhs, int lhsIndex, T[] rhs, int rhsIndex, T[] ans, int ansIndex, int length) where T : struct
        {
            if (!CanAdd<T>())
                throw new NotSupportedException(string.Format(NotSupported, typeof(T).FullName));
            if (lhs is null)
                throw new ArgumentNullException(nameof(lhs));
            if (rhs is null)
                throw new ArgumentNullException(nameof(rhs));
            if (ans is null)
                throw new ArgumentNullException(nameof(ans));
            if (length < 0)
                throw new IndexOutOfRangeException(NegativeLength);
            if (lhsIndex < 0 || rhsIndex < 0 || ansIndex < 0)
                throw new IndexOutOfRangeException(NegativeLength);
            if (lhsIndex + length > lhs.Length || rhsIndex + length > rhs.Length || ansIndex + length > ans.Length)
                throw new IndexOutOfRangeException(InvalidIndices);
            AddPar_NoChecks(lhs, lhsIndex, rhs, rhsIndex, ans, ansIndex, length);
        }

        static void AddPar_NoChecks<T>(T[] lhs, int lhsIndex, T[] rhs, int rhsIndex, T[] ans, int ansIndex, int length) where T : struct
        {
            if (length == 0)
                return;

            int stop = length / Vector<T>.Count;
            Parallel.For(0, stop, i =>
            {
                i *= Vector<T>.Count;
                var va = new Vector<T>(lhs, lhsIndex + i);
                var vb = new Vector<T>(rhs, rhsIndex + i);
                var vans = Vector.Add(va, vb);
                vans.CopyTo(ans, ansIndex + i);
            });

            for (int i = stop * Vector<T>.Count; i < length; i++)
                ans[ansIndex + i] = (dynamic)lhs[lhsIndex + i] + rhs[rhsIndex + i];
        }
        #endregion

        #region Dot
        /// <summary>
        /// Returns the dot-product of two arrays.
        /// </summary>
        public static T DotPar<T>(T[] lhs, T[] rhs) where T : struct
        {
            if (!CanAdd<T>() || !CanMultiply<T>())
                throw new NotSupportedException(string.Format(NotSupported, typeof(T).FullName));
            if (lhs is null)
                throw new ArgumentNullException(nameof(lhs));
            if (rhs is null)
                throw new ArgumentNullException(nameof(rhs));
            if (lhs.Length != rhs.Length)
                throw new IndexOutOfRangeException(InvalidInputLength);
            return Dot_NoCheck(lhs, rhs);
        }

        static T DotPar_NoCheck<T>(T[] lhs, T[] rhs) where T : struct
        {
            if (lhs.Length == 0)
                return default;
            
            dynamic sum = default(T);
            Parallel.For<T>(0, lhs.Length / Vector<T>.Count, 
                () => default(T), 
                (i, state, subtotal) => GetDotProduct(lhs, rhs, i),
                (x) => sum += x);

            for (int i = lhs.Length / Vector<T>.Count; i < lhs.Length; i++)
                sum += (dynamic)lhs[i] * rhs[i];

            return sum;
        }

        static T GetDotProduct<T>(T[] lhs, T[] rhs, int i) where T : struct
        {
            i *= Vector<T>.Count;
            var va = new Vector<T>(lhs, i * Vector<T>.Count);
            var vb = new Vector<T>(rhs, i * Vector<T>.Count);
            return Vector.Dot(va, vb);
        }
        #endregion

        #region Multiply
        /// <summary>
        /// Performs piecewise multiplication on two arrays, and returns the result.
        /// </summary>
        /// <exception cref="NotSupportedException"><typeparamref name="T"/> does not support multiplication.</exception>
        /// <exception cref="IndexOutOfRangeException">The input arrays have different lengths.</exception>
        public static T[] MultiplyPar<T>(T[] lhs, T[] rhs) where T : struct
        {
            if (!CanMultiply<T>())
                throw new NotSupportedException(string.Format(NotSupported, typeof(T).FullName));
            if (lhs is null)
                throw new ArgumentNullException(nameof(lhs));
            if (rhs is null)
                throw new ArgumentNullException(nameof(rhs));
            if (lhs.Length != rhs.Length)
                throw new IndexOutOfRangeException(InvalidInputLength);
            T[] ans = new T[lhs.Length];
            MultiplyPar_NoChecks(lhs, 0, rhs, 0, ans, 0, lhs.Length);
            return ans;
        }

        /// <summary>
        /// Performs piecewise multiplication on two arrays, placing the result in <paramref name="ans"/>.
        /// </summary>
        /// <exception cref="NotSupportedException"><typeparamref name="T"/> does not support multiplication.</exception>
        /// <exception cref="IndexOutOfRangeException">The input arrays have different lengths.</exception>
        public static void MultiplyPar<T>(T[] lhs, T[] rhs, T[] ans) where T : struct
        {
            if (!CanMultiply<T>())
                throw new NotSupportedException(string.Format(NotSupported, typeof(T).FullName));
            if (lhs is null)
                throw new ArgumentNullException(nameof(lhs));
            if (rhs is null)
                throw new ArgumentNullException(nameof(rhs));
            if (ans is null)
                throw new ArgumentNullException(nameof(ans));
            if (lhs.Length != rhs.Length || lhs.Length != ans.Length)
                throw new IndexOutOfRangeException(InvalidInputLength);
            MultiplyPar_NoChecks(lhs, 0, rhs, 0, ans, 0, lhs.Length);
        }

        /// <summary>
        /// Multiplies <paramref name="length"/> elements in two arrays,
        /// starting at the specified indices, placing the result in 
        /// <paramref name="ans"/> at the specified index.
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="lhsIndex">Starting index of lhs.</param>
        /// <param name="rhs"></param>
        /// <param name="rhsIndex">Starting index of rhs.</param>
        /// <param name="ans"></param>
        /// <param name="ansIndex">Starting index of ans.</param>
        /// <param name="length">Number of entries to operate on.</param>
        /// <exception cref="NotSupportedException"><typeparamref name="T"/> does not support multiplication.</exception>
        /// <exception cref="IndexOutOfRangeException">Any indices or <paramref name="length"/> are less than 0, or invalid.</exception>
        public static void MultiplyPar<T>(T[] lhs, int lhsIndex, T[] rhs, int rhsIndex, T[] ans, int ansIndex, int length) where T : struct
        {
            if (!CanMultiply<T>())
                throw new NotSupportedException(string.Format(NotSupported, typeof(T).FullName));
            if (lhs is null)
                throw new ArgumentNullException(nameof(lhs));
            if (rhs is null)
                throw new ArgumentNullException(nameof(rhs));
            if (ans is null)
                throw new ArgumentNullException(nameof(ans));
            if (length < 0)
                throw new IndexOutOfRangeException(NegativeLength);
            if (lhsIndex < 0 || rhsIndex < 0 || ansIndex < 0)
                throw new IndexOutOfRangeException(NegativeLength);
            if (lhsIndex + length > lhs.Length || rhsIndex + length > rhs.Length || ansIndex + length > ans.Length)
                throw new IndexOutOfRangeException(InvalidIndices);
            MultiplyPar_NoChecks(lhs, lhsIndex, rhs, rhsIndex, ans, ansIndex, length);
        }

        static void MultiplyPar_NoChecks<T>(T[] lhs, int lhsIndex, T[] rhs, int rhsIndex, T[] ans, int ansIndex, int length) where T : struct
        {
            if (length == 0)
                return;

            int stop = length / Vector<T>.Count;
            Parallel.For(0, stop, i =>
            {
                i *= Vector<T>.Count;
                var va = new Vector<T>(lhs, lhsIndex + i);
                var vb = new Vector<T>(rhs, rhsIndex + i);
                var vans = Vector.Multiply(va, vb);
                vans.CopyTo(ans, ansIndex + i);
            });

            for (int i = stop * Vector<T>.Count; i < length; i++)
                ans[ansIndex + i] = (dynamic)lhs[lhsIndex + i] + rhs[rhsIndex + i];
        }
        #endregion

        #region Matrix Multiply
        /// <summary>
        /// Multiplies a vector by a matrix, returning the result.
        /// </summary>
        /// <exception cref="NotSupportedException"><typeparamref name="T"/> does not support addition and multiplication.</exception>
        /// <exception cref="IndexOutOfRangeException">The input arrays have incompatible lengths.</exception>
        public static T[] MatrixMultiplyPar<T>(T[] lhs, T[][] rhs) where T : struct
        {
            if (!CanAdd<T>() || !CanMultiply<T>())
                throw new NotSupportedException(NotSupported);
            if (lhs is null)
                throw new ArgumentNullException(nameof(lhs));
            if (rhs is null)
                throw new ArgumentNullException(nameof(rhs));
            for (int i = 0; i < rhs.Length; i++)
            {
                if (rhs[i] is null)
                    throw new ArgumentNullException(nameof(rhs));
                if (rhs[i].Length != lhs.Length)
                    throw new IndexOutOfRangeException(InvalidInputLength);
            }
            T[] ans = new T[rhs.Length];
            MatrixMultiplyPar_NoChecks(lhs, rhs, ans);
            return ans;
        }

        /// <summary>
        /// Multiplies a vector by a matrix, placing the result in <paramref name="ans"/>.
        /// </summary>
        /// <exception cref="NotSupportedException"><typeparamref name="T"/> does not support addition and multiplication,
        /// or parameters reference the same array.</exception>
        /// <exception cref="IndexOutOfRangeException">The input arrays have incompatible lengths.</exception>
        public static void MatrixMultiplyPar<T>(T[] lhs, T[][] rhs, T[] ans) where T : struct
        {
            if (!CanAdd<T>() || !CanMultiply<T>())
                throw new NotSupportedException(NotSupported);
            if (lhs is null)
                throw new ArgumentNullException(nameof(lhs));
            if (rhs is null)
                throw new ArgumentNullException(nameof(rhs));
            if (ans is null)
                throw new ArgumentNullException(nameof(ans));
            if (lhs == ans)
                throw new NotSupportedException(EqualParams);
            if (rhs.Length != ans.Length)
                throw new IndexOutOfRangeException(InvalidInputLength);
            for (int i = 0; i < rhs.Length; i++)
            {
                if (rhs[i] is null)
                    throw new ArgumentNullException(nameof(rhs));
                if (rhs[i].Length != lhs.Length)
                    throw new IndexOutOfRangeException(InvalidInputLength);
                if (rhs[i] == lhs || rhs[i] == ans)
                    throw new NotSupportedException(EqualParams);
            }
            MatrixMultiplyPar_NoChecks(lhs, rhs, ans);
        }

        static void MatrixMultiplyPar_NoChecks<T>(T[] lhs, T[][] rhs, T[] ans) where T : struct
        {
            if (lhs.Length == 0 || ans.Length == 0)
                return;
            Parallel.For(0, ans.Length, 
                i => ans[i] = Dot(lhs, rhs[i]));
        }
        #endregion
    }
}
