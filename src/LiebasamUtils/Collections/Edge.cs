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

        /// <summary>
        /// Singleton class for checking equality between edge IDs.
        /// </summary>
        public class IDComparer : IEqualityComparer<Edge<T>>
        {
            public static IDComparer Instance { get; } = new IDComparer();
            private IDComparer() { }
            public bool Equals(Edge<T> x, Edge<T> y) => x.IDFrom == y.IDFrom && x.IDTo == y.IDTo;
            public int GetHashCode(Edge<T> obj) => CombineIDs(obj).GetHashCode();
            ulong CombineIDs(Edge<T> obj) => (((ulong)obj.IDFrom) << sizeof(uint)) | obj.IDTo;
        }
    }
}
