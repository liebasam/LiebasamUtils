using System;
using System.Linq;

namespace LiebasamUtils.Random
{
    /// <summary>
    /// Thread-safe static class for random number generation.
    /// </summary>
    public static class RandomProvider
    {
        #region Underlying RNG Access
        private static System.Random RNG = new System.Random();

        /// <summary>
        /// Set the random seed of the underlying
        /// <see cref="Random"/>. Set to null to
        /// use the time-dependent default seed.
        /// </summary>
        public static int? RandomSeed
        {
            set
            {
                lock (RNG)
                {
                    if (value.HasValue)
                        RNG = new System.Random(value.Value);
                    else
                        RNG = new System.Random();
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Flips a coin with given odds.
        /// </summary>
        /// <param name="oddsTrue">The odds of returning true.
        /// This value must be between 0 and 1, inclusive.</param>
        public static bool NextBool(double oddsTrue = 0.5)
        {
            if (oddsTrue < 0 || oddsTrue > 1)
                throw new ArgumentOutOfRangeException(nameof(oddsTrue));
            lock (RNG) { return RNG.NextDouble() < oddsTrue; }
        }

        /// <summary>
        /// Performs an in-place shuffling of a given array.
        /// </summary>
        public static void Shuffle<T>(T[] array)
        {
            if (array is null)
                throw new ArgumentNullException(nameof(array));
            if (array.Length <= 1)
                return;

            lock (RNG)
            {
                lock (array)
                {
                    int i, j; T tmp;
                    for (i = 0; i < array.Length; i++)
                    {
                        j = RNG.Next(array.Length);
                        tmp = array[j];
                        array[j] = array[i];
                        array[i] = tmp;
                    }
                }
            }
        }

        #region Next Integer
        /// <summary>
        /// Returns a non-negative random integer.
        /// </summary>
        public static int Next()
        {
            lock (RNG) { return RNG.Next(); }
        }

        /// <summary>
        /// Returns a non-negative random integer that is less
        /// than the specified maximum.
        /// </summary>
        public static int Next(int maxValue)
        {
            lock (RNG) { return RNG.Next(maxValue); }
        }

        /// <summary>
        /// Returns a random integer that is within a specified range.
        /// </summary>
        /// <param name="minValue">Minimum value, inclusive.</param>
        /// <param name="maxValue">Maximum value, exclusive.</param>
        public static int Next(int minValue, int maxValue)
        {
            lock (RNG) { return RNG.Next(minValue, maxValue); }
        }
        #endregion

        #region Next Double
        /// <summary>
        /// Returns a random floating-point number that is greater than
        /// or equal to 0.0, and less than 1.0.
        /// </summary>
        public static double NextDouble()
        {
            lock (RNG) { return RNG.NextDouble(); }
        }

        /// <summary>
        /// Returns a uniformly-distributed random value between 
        /// 0.0 and <paramref name="maxValue"/>.
        /// </summary>
        public static double NextDouble(double maxValue)
        {
            lock (RNG) { return RNG.NextDouble() * maxValue; }
        }

        /// <summary>
        /// Returns a uniformly-distributed floating-point random value within
        /// the specified range.
        /// </summary>
        public static double NextDouble(double minValue, double maxValue)
        {
            if (minValue >= maxValue)
                throw new ArgumentOutOfRangeException(nameof(maxValue));
            lock (RNG) { return minValue + (RNG.NextDouble() * (maxValue - minValue)); }
        }
        #endregion

        #endregion
    }
}
