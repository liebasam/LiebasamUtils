using System;
using System.Numerics;

namespace LiebasamUtils
{
    public static partial class FastMath
    {
        #region Sum
        /// <summary>
        /// Sums the elements in an array.
        /// </summary>
        public static T Sum<T>(params T[] ts) where T : struct
        {
            if (!CanAdd<T>())
                throw new NotSupportedException(string.Format(NotSupported, typeof(T).FullName));
            if (ts is null)
                throw new ArgumentNullException(nameof(ts));
            if (ts.Length < 1)
                return default;
            if (ts.Length == 1)
                return ts[0];

            int i = 0;
            dynamic sum = default(T);
            Vector<T> v1 = Vector<T>.One;
            Vector<T> va;
            for (; i < ts.Length - Vector<T>.Count; i += Vector<T>.Count)
            {
                va = new Vector<T>(ts, i);
                sum += Vector.Dot(va, v1);
            }
            for (; i < ts.Length; i++)
                sum += (dynamic)ts[i];

            return sum;
        }
        #endregion

        #region Addition
        /// <summary>
        /// Adds together two arrays and returns the result.
        /// </summary>
        /// <exception cref="NotSupportedException"><typeparamref name="T"/> does not support addition.</exception>
        /// <exception cref="IndexOutOfRangeException">The input arrays have different lengths.</exception>
        public static T[] Add<T>(T[] lhs, T[] rhs) where T : struct
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
            Add_NoChecks(lhs, 0, rhs, 0, ans, 0, lhs.Length);
            return ans;
        }

        /// <summary>
        /// Adds together two arrays, placing the result in <paramref name="ans"/>.
        /// </summary>
        /// <exception cref="NotSupportedException"><typeparamref name="T"/> does not support addition.</exception>
        /// <exception cref="IndexOutOfRangeException">The input arrays have different lengths.</exception>
        public static void Add<T>(T[] lhs, T[] rhs, T[] ans) where T : struct
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
            Add_NoChecks(lhs, 0, rhs, 0, ans, 0, lhs.Length);
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
        public static void Add<T>(T[] lhs, int lhsIndex, T[] rhs, int rhsIndex, T[] ans, int ansIndex, int length) where T : struct
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
            Add_NoChecks(lhs, lhsIndex, rhs, rhsIndex, ans, ansIndex, length);
        }

        static void Add_NoChecks<T>(T[] lhs, int lhsIndex, T[] rhs, int rhsIndex, T[] ans, int ansIndex, int length) where T : struct
        {
            if (length == 0)
                return;

            int i = 0;
            Vector<T> va, vb, vans;
            for (; i < length - Vector<T>.Count; i += Vector<T>.Count)
            {
                va = new Vector<T>(lhs, lhsIndex + i);
                vb = new Vector<T>(rhs, rhsIndex + i);
                vans = Vector.Add(va, vb);
                vans.CopyTo(ans, ansIndex + i);
            }
            for (; i < length; i++)
                ans[ansIndex + i] = (dynamic)lhs[lhsIndex + i] + rhs[rhsIndex + i];
        }
        #endregion

        #region Dot
        /// <summary>
        /// Returns the dot-product of two arrays.
        /// </summary>
        /// <exception cref="NotSupportedException"><typeparamref name="T"/> does not support addition
        /// or multiplication.</exception>
        /// <exception cref="IndexOutOfRangeException">The input arrays have different lengths.</exception>
        public static T Dot<T>(T[] lhs, T[] rhs) where T : struct
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

        static T Dot_NoCheck<T>(T[] lhs, T[] rhs) where T : struct
        {
            if (lhs.Length == 0)
                return default;

            int i = 0;
            dynamic sum = default(T);
            Vector<T> va, vb;
            for (; i < lhs.Length - Vector<T>.Count; i += Vector<T>.Count)
            {
                va = new Vector<T>(lhs, i);
                vb = new Vector<T>(rhs, i);
                sum += Vector.Dot(va, vb);
            }
            for (; i < lhs.Length; i++)
                sum += (dynamic)lhs[i] * rhs[i];

            return sum;
        }
        #endregion

        #region Max
        /// <summary>
        /// Returns the piecewise maximum of two arrays.
        /// </summary>
        /// <exception cref="NotSupportedException"><typeparamref name="T"/> does not support addition.</exception>
        /// <exception cref="IndexOutOfRangeException">The input arrays have different lengths.</exception>
        public static T[] Max<T>(T[] lhs, T[] rhs) where T : struct
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
            Max_NoChecks(lhs, rhs, ans);
            return ans;
        }

        /// <summary>
        /// Returns the piecewise maximum of an array and a given value.
        /// </summary>
        /// <exception cref="NotSupportedException"><typeparamref name="T"/> does not support addition.</exception>
        /// <exception cref="IndexOutOfRangeException">The input arrays have different lengths.</exception>
        public static T[] Max<T>(T[] lhs, T value) where T : struct
        {
            if (!CanAdd<T>())
                throw new NotSupportedException(string.Format(NotSupported, typeof(T).FullName));
            if (lhs is null)
                throw new ArgumentNullException(nameof(lhs));
            T[] ans = new T[lhs.Length];
            Max_NoChecks(lhs, value, ans);
            return ans;
        }

        /// <summary>
        /// Gets the piecewise maximum of two arrays, placing the result in <paramref name="ans"/>.
        /// </summary>
        /// <exception cref="NotSupportedException"><typeparamref name="T"/> does not support addition.</exception>
        /// <exception cref="IndexOutOfRangeException">The input arrays have different lengths.</exception>
        public static void Max<T>(T[] lhs, T[] rhs, T[] ans) where T : struct
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
            Max_NoChecks(lhs, rhs, ans);
        }

        /// <summary>
        /// Gets the piecewise maximum of an array and given value,
        /// placing the result in <paramref name="ans"/>.
        /// </summary>
        /// <exception cref="NotSupportedException"><typeparamref name="T"/> does not support addition.</exception>
        /// <exception cref="IndexOutOfRangeException">The input arrays have different lengths.</exception>
        public static void Max<T>(T[] lhs, T value, T[] ans) where T : struct
        {
            if (!CanAdd<T>())
                throw new NotSupportedException(string.Format(NotSupported, typeof(T).FullName));
            if (lhs is null)
                throw new ArgumentNullException(nameof(lhs));
            if (ans is null)
                throw new ArgumentNullException(nameof(ans));
            if (lhs.Length != ans.Length)
                throw new IndexOutOfRangeException(InvalidInputLength);
            Max_NoChecks(lhs, value, ans);
        }

        static void Max_NoChecks<T>(T[] lhs, T[] rhs, T[] ans) where T : struct
        {
            if (lhs.Length == 0)
                return;

            int i = 0;
            Vector<T> va, vb, vans;
            for (; i < lhs.Length - Vector<T>.Count; i += Vector<T>.Count)
            {
                va = new Vector<T>(lhs, i);
                vb = new Vector<T>(rhs, i);
                vans = Vector.Max(va, vb);
                vans.CopyTo(ans, i);
            }
            for (; i < lhs.Length; i++)
                ans[i] = ((dynamic)lhs[i] > rhs[i]) ? lhs[i] : rhs[i];
        }

        static void Max_NoChecks<T>(T[] lhs, T value, T[] ans) where T : struct
        {
            if (lhs.Length == 0)
                return;

            int i = 0;
            Vector<T> vb = Vector.Multiply(Vector<T>.One, value);
            Vector<T> va, vans;
            for (; i < lhs.Length - Vector<T>.Count; i += Vector<T>.Count)
            {
                va = new Vector<T>(lhs, i);
                vans = Vector.Max(va, vb);
                vans.CopyTo(ans, i);
            }
            for (; i < lhs.Length; i++)
                ans[i] = ((dynamic)lhs[i] > value) ? lhs[i] : value;
        }
        #endregion

        #region Multiply
        /// <summary>
        /// Performs piecewise multiplication on two arrays, and returns the result.
        /// </summary>
        /// <exception cref="NotSupportedException"><typeparamref name="T"/> does not support multiplication.</exception>
        /// <exception cref="IndexOutOfRangeException">The input arrays have different lengths.</exception>
        public static T[] Multiply<T>(T[] lhs, T[] rhs) where T : struct
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
            Multiply_NoChecks(lhs, 0, rhs, 0, ans, 0, lhs.Length);
            return ans;
        }

        /// <summary>
        /// Performs piecewise multiplication on two arrays, placing the result in <paramref name="ans"/>.
        /// </summary>
        /// <exception cref="NotSupportedException"><typeparamref name="T"/> does not support multiplication.</exception>
        /// <exception cref="IndexOutOfRangeException">The input arrays have different lengths.</exception>
        public static void Multiply<T>(T[] lhs, T[] rhs, T[] ans) where T : struct
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
            Multiply_NoChecks(lhs, 0, rhs, 0, ans, 0, lhs.Length);
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
        public static void Multiply<T>(T[] lhs, int lhsIndex, T[] rhs, int rhsIndex, T[] ans, int ansIndex, int length) where T : struct
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
            Multiply_NoChecks(lhs, lhsIndex, rhs, rhsIndex, ans, ansIndex, length);
        }

        static void Multiply_NoChecks<T>(T[] lhs, int lhsIndex, T[] rhs, int rhsIndex, T[] ans, int ansIndex, int length) where T : struct
        {
            if (length == 0)
                return;

            int i = 0;
            Vector<T> va, vb, vans;
            for (; i < length - Vector<T>.Count; i += Vector<T>.Count)
            {
                va = new Vector<T>(lhs, lhsIndex + i);
                vb = new Vector<T>(rhs, rhsIndex + i);
                vans = Vector.Multiply(va, vb);
                vans.CopyTo(ans, ansIndex + i);
            }
            for (; i < length; i++)
                ans[ansIndex + i] = (dynamic)lhs[lhsIndex + i] * rhs[rhsIndex + i];
        }
        #endregion

        #region Matrix Multiply
        /// <summary>
        /// Multiplies a vector by a matrix, returning the result.
        /// </summary>
        /// <exception cref="NotSupportedException"><typeparamref name="T"/> does not support addition and multiplication.</exception>
        /// <exception cref="IndexOutOfRangeException">The input arrays have incompatible lengths.</exception>
        public static T[] MatrixMultiply<T>(T[] lhs, T[][] rhs) where T : struct
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
            MatrixMultiply_NoChecks(lhs, rhs, ans);
            return ans;
        }

        /// <summary>
        /// Multiplies a vector by a matrix, placing the result in <paramref name="ans"/>.
        /// </summary>
        /// <exception cref="NotSupportedException"><typeparamref name="T"/> does not support addition and multiplication,
        /// or parameters reference the same array.</exception>
        /// <exception cref="IndexOutOfRangeException">The input arrays have incompatible lengths.</exception>
        public static void MatrixMultiply<T>(T[] lhs, T[][] rhs, T[] ans) where T : struct
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
            MatrixMultiply_NoChecks(lhs, rhs, ans);
        }

        static void MatrixMultiply_NoChecks<T>(T[] lhs, T[][] rhs, T[] ans) where T : struct
        {
            if (lhs.Length == 0 || ans.Length == 0)
                return;
            for (int i = 0; i < ans.Length; i++)
                ans[i] = Dot(lhs, rhs[i]);
        }
        #endregion
    }
}
