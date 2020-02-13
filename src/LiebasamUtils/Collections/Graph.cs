using System;
using System.Collections.Generic;
using System.Linq;

namespace LiebasamUtils.Collections
{
    public class Graph<TVert, TEdge> : IGraph<TVert, TEdge>
    {
        static readonly string NoVertex = "Vertex {0} does not exist.";

        readonly SortedList<uint, TVert> _vertices;
        readonly SortedList<ulong, TEdge> _edges;

        #region Constructors
        /// <summary>
        /// Creates an empty graph.
        /// </summary>
        public Graph()
        {
            _vertices = new SortedList<uint, TVert>();
            _edges = new SortedList<ulong, TEdge>();
        }

        /// <summary>
        /// Creates a graph with the specified vertices and edges.
        /// </summary>
        public Graph(IEnumerable<Vertex<TVert>> vertices, IEnumerable<Edge<TEdge>> edges)
        {
            if (vertices is null)
                throw new ArgumentNullException(nameof(vertices));
            if (edges is null)
                throw new ArgumentNullException(nameof(edges));
            _vertices = new SortedList<uint, TVert>(vertices.ToDictionary(v => v.ID, v => v.Value));
            _edges = new SortedList<ulong, TEdge>(edges.Count());
            foreach (var edge in edges)
                AddEdge(edge.IDFrom, edge.IDTo, edge.Value);
        }

        /// <summary>
        /// Creates a graph with the specified vertex capacity.
        /// The edge capacity is this number, squared.
        /// </summary>
        public Graph(int capacity)
        {
            _vertices = new SortedList<uint, TVert>(capacity);
            _edges = new SortedList<ulong, TEdge>(capacity * capacity);
        }
        #endregion

        #region Properties
        public TVert this[uint id]
        {
            get => GetVertex(id);
            set => AddVertex(id, value);
        }

        public TEdge this[uint idFrom, uint idTo]
        {
            get => GetEdge(idFrom, idTo);
            set => AddEdge(idFrom, idTo, value);
        }
        
        public int VertexCount => _vertices.Count;

        public int EdgeCount => _edges.Count;

        public IEnumerable<Vertex<TVert>> Vertices => _vertices.Select(kv => new Vertex<TVert>(kv.Key, kv.Value));

        public IEnumerable<Edge<TEdge>> Edges => _edges.Select(kv =>
        {
            (var idFrom, var idTo) = KeyToEdge(kv.Key);
            return new Edge<TEdge>(idFrom, idTo, kv.Value);
        });
        #endregion

        #region Methods
        public void AddEdge(uint idFrom, uint idTo, TEdge value)
        {
            if (!_vertices.ContainsKey(idFrom))
                throw new InvalidOperationException(string.Format(NoVertex, idFrom));
            if (!_vertices.ContainsKey(idTo))
                throw new InvalidOperationException(string.Format(NoVertex, idTo));
            _edges[EdgeToKey(idFrom, idTo)] = value;
        }

        public void AddVertex(uint id, TVert value) => _vertices[id] = value;

        public void Clear()
        {
            _vertices.Clear();
            _edges.Clear();
        }

        public bool ContainsEdge(uint idFrom, uint idTo) => _edges.ContainsKey(EdgeToKey(idFrom, idTo));

        public bool ContainsVertex(uint id) => _vertices.ContainsKey(id);

        public TEdge GetEdge(uint idFrom, uint idTo) => _edges[EdgeToKey(idFrom, idTo)];

        public TVert GetVertex(uint id) => _vertices[id];

        public bool RemoveEdge(uint idFrom, uint idTo) => _edges.Remove(EdgeToKey(idFrom, idTo));

        public bool RemoveVertex(uint id)
        {
            if (Edges.Any(e => e.IDFrom == id || e.IDTo == id))
                return false;
            return _vertices.Remove(id);
        }

        public bool ForceRemoveVertex(uint id)
        {
            bool exists = _vertices.Remove(id);
            if (exists)
            {
                foreach (var edge in Edges.ToArray())
                    if (edge.IDFrom == id || edge.IDTo == id)
                        RemoveEdge(edge.IDFrom, edge.IDTo);
            }
            return exists;
        }

        public bool TryGetEdge(uint idFrom, uint idTo, out TEdge value) => _edges.TryGetValue(EdgeToKey(idFrom, idTo), out value);

        public bool TryGetVertex(uint id, out TVert value) => _vertices.TryGetValue(id, out value);
        #endregion

        #region Helpers
        const int ShiftSize = sizeof(uint) * 8;

        /// <summary>
        /// Convert a from/to ID pair to a ulong.
        /// </summary>
        static ulong EdgeToKey(uint idFrom, uint idTo) => (((ulong)idFrom) << ShiftSize) | idTo;

        /// <summary>
        /// Convert a ulong to a from/to ID pair.
        /// </summary>
        static (uint, uint) KeyToEdge(ulong key) => (
            (uint)(key >> ShiftSize),
            (uint)((key << ShiftSize) >> ShiftSize)
            );
        #endregion
    }
}
