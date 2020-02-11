namespace LiebasamUtils.Collections
{
    /// <summary>
    /// Interface for a collection of vertices and edges.
    /// </summary>
    public interface IGraph<TVert, TEdge> : IReadOnlyGraph<TVert, TEdge>
    {
        /// <summary>
        /// Access and manipulate the data at a specified vertex.
        /// </summary>
        new TVert this[uint id] { get; set; }

        /// <summary>
        /// Access and manipulate the data at a specified edge.
        /// </summary>
        new TEdge this[uint idFrom, uint idTo] { get; set; }

        /// <summary>
        /// Adds a vertex to the graph, overriding one if it already exists.
        /// </summary>
        void AddVertex(uint id, TVert value);

        /// <summary>
        /// Adds an edge to the graph, overriding one if it already exists.
        /// </summary>
        void AddEdge(uint idFrom, uint idTo, TEdge value);

        /// <summary>
        /// Attempts to remove a vertex with the given ID. This method returns
        /// false if the vertex does not exist.
        /// </summary>
        bool RemoveVertex(uint id);

        /// <summary>
        /// Attempts to remove an edge with the given IDs. This method returns
        /// false if the edge does not exist.
        /// </summary>
        bool RemoveEdge(uint idFrom, uint idTo);

        /// <summary>
        /// Removes all vertices and edges from the graph.
        /// </summary>
        void Clear();
    }
}
