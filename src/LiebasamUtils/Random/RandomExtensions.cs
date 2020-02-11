using System;
using System.Collections.Generic;
using System.Linq;

namespace LiebasamUtils.Random
{
    /// <summary>
    /// Randomized extension methods for enumerable collections.
    /// </summary>
    public static class RandomExtensions
    {
        static readonly string EmptyCollection = "Collection must be non-empty.";

        #region Methods
        /// <summary>
        /// Chooses an item from the collection uniformly at random.
        /// </summary>
        public static T ChooseUniform<T>(this IEnumerable<T> collection)
        {
            if (collection is null)
                throw new ArgumentNullException(nameof(collection));

            int count = collection.Count();
            if (count == 0)
                throw new ArgumentException(EmptyCollection);

            int ind = RandomProvider.Next(count);
            return collection.ElementAt(ind);
        }

        /// <summary>
        /// Returns a random permutation of the collection.
        /// </summary>
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> collection)
        {
            if (collection is null)
                throw new ArgumentNullException(nameof(collection));

            int[] indices = Enumerable.Range(0, collection.Count()).ToArray();
            RandomProvider.Shuffle(indices);

            for (int i = 0; i < indices.Length; i++)
                yield return collection.ElementAt(indices[i]);
        }
        #endregion
    }
}
