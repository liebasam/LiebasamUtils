using System.Collections.Generic;

namespace LiebasamUtils.Collections
{
    /// <summary>
    /// Read-only interface for a collection of vertices and edges.
    /// </summary>
    public interface IReadOnlyGraph<TVert, TEdge>
    {
        /// <summary>
        /// Access the data at a specified vertex.
        /// </summary>
        TVert this[uint id] { get; }

        /// <summary>
        /// Access the data at a specified edge.
        /// </summary>
        TEdge this[uint idFrom, uint idTo] { get; }

        /// <summary>
        /// Number of vertices in the graph.
        /// </summary>
        int VertexCount { get; }

        /// <summary>
        /// Number of edges in the graph.
        /// </summary>
        int EdgeCount { get; }

        /// <summary>
        /// Enumerates the vertices.
        /// </summary>
        IEnumerable<Vertex<TVert>> Vertices { get; }

        /// <summary>
        /// Enumerates the edges.
        /// </summary>
        IEnumerable<Edge<TEdge>> Edges { get; }

        /// <summary>
        /// Returns true if the graph contains the specified vertex.
        /// </summary>
        bool ContainsVertex(uint id);

        /// <summary>
        /// Returns true if the graph contains the specified edge.
        /// </summary>
        bool ContainsEdge(uint idFrom, uint idTo);

        /// <summary>
        /// Returns the data in a given vertex.
        /// </summary>
        TVert GetVertex(uint id);

        /// <summary>
        /// Returns the data in a given edge.
        /// </summary>
        TEdge GetEdge(uint idFrom, uint idTo);

        /// <summary>
        /// Attempts to get the data on a given vertex.
        /// </summary>
        /// <returns>True if the vertex exists, false otherwise.</returns>
        bool TryGetVertex(uint id, out TVert value);

        /// <summary>
        /// Attempts to get the data on a given edge.
        /// </summary>
        /// <returns>True if the edge exists, false otherwise.</returns>
        bool TryGetEdge(uint idFrom, uint idTo, out TEdge value);
    }
}
