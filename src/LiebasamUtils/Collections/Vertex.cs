using System.Collections.Generic;

namespace LiebasamUtils.Collections
{
    /// <summary>
    /// Structure defining a graph vertex, containing ID and data.
    /// </summary>
    /// <typeparam name="T">Type of the data.</typeparam>
    public readonly struct Vertex<T>
    {
        const string ToStringFormat = "{0} [{1}]";
        public uint ID { get; }
        public T Value { get; }

        public Vertex(uint id)
        {
            ID = id;
            Value = default;
        }

        public Vertex(uint id, T value)
        {
            ID = id;
            Value = value;
        }

        public override string ToString() => string.Format(ToStringFormat, ID, Value);
    }
}
