using System.Collections.Generic;

namespace LiebasamUtils.Collections
{
    /// <summary>
    /// Structure defining a graph edge, containing two IDs and data.
    /// </summary>
    /// <typeparam name="T">Type of the data.</typeparam>
    public readonly struct Edge<T>
    {
        const string ToStringFormat = "{0}->{1} [{2}]";
        /// <summary>
        /// ID of the source vertex.
        /// </summary>
        public uint IDFrom { get; }
        /// <summary>
        /// ID of the destination vertex.
        /// </summary>
        public uint IDTo { get; }

        public T Value { get; }

        public Edge(uint idFrom, uint idTo)
        {
            IDFrom = idFrom;
            IDTo = idTo;
            Value = default;
        }

        public Edge(uint idFrom, uint idTo, T value)
        {
            IDFrom = idFrom;
            IDTo = idTo;
            Value = value;
        }

        public override string ToString() => string.Format(ToStringFormat, IDFrom, IDTo, Value);
    }
}
