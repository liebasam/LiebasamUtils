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

    /// <summary>
    /// Holds helper methods for coverting <see cref="Edge{T}.IDFrom"/> and
    /// <see cref="Edge{T}.IDTo"/> to a ulong.
    /// </summary>
    public readonly struct Edge
    {
        const int ShiftSize = sizeof(uint) * 8;

        /// <summary>
        /// Convert a from/to ID pair to a ulong.
        /// </summary>
        public static ulong ZipIDs(uint idFrom, uint idTo) => (((ulong)idFrom) << ShiftSize) | idTo;

        /// <summary>
        /// Convert a ulong to a from/to ID pair.
        /// </summary>
        public static (uint, uint) UnzipIDs(ulong key) => (
            (uint)(key >> ShiftSize),
            (uint)((key << ShiftSize) >> ShiftSize)
            );
    }
}
