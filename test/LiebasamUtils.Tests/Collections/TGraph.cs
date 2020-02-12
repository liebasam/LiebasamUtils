using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LiebasamUtils.Collections.Tests
{
    [TestClass]
    public class TGraph
    {
        static readonly Vertex<int>[] vertices = new Vertex<int>[]
        {
            new Vertex<int>(1, 1),
            new Vertex<int>(0, 0),
            new Vertex<int>(3, 3)
        };
        static readonly Edge<int>[] edges = new Edge<int>[]
        {
            new Edge<int>(1, 0, 10),
            new Edge<int>(1, 3, 13),
            new Edge<int>(0, 0, 00),
            new Edge<int>(3, 0, 30)
        };

        [TestClass]
        public class Constructors
        {
            [TestClass]
            public class Failure
            {
                [TestMethod]
                public void NullArgs()
                {
                    Assert.ThrowsException<ArgumentNullException>(
                        () => new Graph<int, int>(null, edges));
                    Assert.ThrowsException<ArgumentNullException>(
                        () => new Graph<int, int>(vertices, null));
                }

                [TestMethod]
                [ExpectedException(typeof(ArgumentOutOfRangeException))]
                public void CapacityRange() => new Graph<int, int>(-1);
            }
        }

        [TestClass]
        public class Properties
        {
            static Graph<int, int> graph;

            [TestInitialize]
            public void Init() => graph = new Graph<int, int>(vertices, edges);

            [TestMethod]
            public void Counts()
            {
                Assert.AreEqual(vertices.Length, graph.VertexCount);
                Assert.AreEqual(edges.Length, graph.EdgeCount);
            }

            [TestMethod]
            public void GetSetVertex()
            {
                var vert = vertices[0];
                Assert.AreEqual(vert.Value, graph[vert.ID]);
                graph[vert.ID] = 100;
                Assert.AreNotEqual(vert.Value, graph[vert.ID]);
                Assert.AreEqual(100, graph[vert.ID]);

                // Exception when accessing invalid key
                Assert.ThrowsException<KeyNotFoundException>(
                    () => graph[uint.MaxValue]);

                graph[uint.MaxValue] = int.MaxValue;
                Assert.AreEqual(int.MaxValue, graph[uint.MaxValue]);
            }

            [TestMethod]
            public void GetSetEdge()
            {
                var edge = edges[0];
                Assert.AreEqual(edge.Value, graph[edge.IDFrom, edge.IDTo]);
                graph[edge.IDFrom, edge.IDTo] = 100;
                Assert.AreNotEqual(edge.Value, graph[edge.IDFrom, edge.IDTo]);
                Assert.AreEqual(100, graph[edge.IDFrom, edge.IDTo]);

                // Exception when accessing invalid key
                Assert.ThrowsException<KeyNotFoundException>(
                    () => graph[0, 1]);

                graph[0, 1] = int.MaxValue;
                Assert.AreEqual(int.MaxValue, graph[0, 1]);
            }

            [TestMethod]
            public void EnumerateVertices()
            {
                var actualVerts = graph.Vertices.ToArray();
                Assert.AreEqual(vertices.Length, actualVerts.Length);
                for (int i = 0; i < vertices.Length; i++)
                {
                    var vert = vertices[i];
                    var actualVert = actualVerts.First(v => v.ID == vert.ID);
                    Assert.AreEqual(vert.Value, actualVert.Value);
                }
            }

            [TestMethod]
            public void EnumerateEdges()
            {
                var actualEdges = graph.Edges.ToArray();
                Assert.AreEqual(edges.Length, actualEdges.Length);
                for (int i = 0; i < vertices.Length; i++)
                {
                    var edge = edges[i];
                    var actualEdge = actualEdges.First(
                        e => e.IDFrom == edge.IDFrom && e.IDTo == edge.IDTo);
                    Assert.AreEqual(edge.Value, actualEdge.Value);
                }
            }
        }

        [TestClass]
        public class Add
        {
            static Graph<int, int> graph;
            [TestInitialize]
            public void Init() => graph = new Graph<int, int>(vertices, edges);

            [TestMethod]
            public void FailureAddEdge()
            {
                Assert.ThrowsException<InvalidOperationException>(
                    () => graph.AddEdge(vertices[0].ID, uint.MaxValue, 0));
                Assert.ThrowsException<InvalidOperationException>(
                    () => graph.AddEdge(uint.MaxValue, vertices[0].ID, 0));
                Assert.AreEqual(edges.Length, graph.EdgeCount);
            }

            [TestMethod]
            public void SuccessAddNewEdge()
            {
                graph.AddEdge(0, 1, int.MaxValue);
                Assert.AreEqual(edges.Length + 1, graph.EdgeCount);
                Assert.IsTrue(graph.ContainsEdge(0, 1));
                Assert.AreEqual(int.MaxValue, graph[0, 1]);
            }

            [TestMethod]
            public void SuccessAddExistingEdge()
            {
                var edge = edges[0];
                graph.AddEdge(edge.IDFrom, edge.IDTo, 100);
                Assert.AreEqual(edges.Length, graph.EdgeCount);
                Assert.IsTrue(graph.ContainsEdge(edge.IDFrom, edge.IDTo));
                Assert.AreEqual(100, graph[edge.IDFrom, edge.IDTo]);
            }

            [TestMethod]
            public void SuccessAddNewVertex()
            {
                graph.AddVertex(uint.MaxValue, 100);
                Assert.AreEqual(vertices.Length + 1, graph.VertexCount);
                Assert.IsTrue(graph.ContainsVertex(uint.MaxValue));
                Assert.AreEqual(100, graph[uint.MaxValue]);
            }

            [TestMethod]
            public void SuccessAddExistingVertex()
            {
                var vert = vertices[0];
                graph.AddVertex(vert.ID, 100);
                Assert.AreEqual(vertices.Length, graph.VertexCount);
                Assert.IsTrue(graph.ContainsVertex(vert.ID));
                Assert.AreEqual(100, graph[vert.ID]);
            }
        }

        [TestClass]
        public class Remove
        {
            static Graph<int, int> graph;
            [TestInitialize]
            public void Init() => graph = new Graph<int, int>(vertices, edges);

            [TestMethod]
            public void EdgeReturnTrue()
            {
                bool actual = graph.RemoveEdge(edges[0].IDFrom, edges[0].IDTo);
                Assert.IsTrue(actual);
                Assert.AreEqual(edges.Length - 1, graph.EdgeCount);
                Assert.IsFalse(graph.ContainsEdge(edges[0].IDFrom, edges[0].IDTo));
            }

            [TestMethod]
            public void EdgeReturnFalse()
            {
                bool actual = graph.RemoveEdge(uint.MaxValue, uint.MaxValue);
                Assert.IsFalse(actual);
                Assert.AreEqual(edges.Length, graph.EdgeCount);
            }

            [TestMethod]
            public void VertexReturnTrue()
            {
                graph.AddVertex(100, 100);
                Assert.AreEqual(vertices.Length + 1, graph.VertexCount);
                bool actual = graph.RemoveVertex(100);
                Assert.IsTrue(actual);
                Assert.AreEqual(vertices.Length, graph.VertexCount);
            }

            [TestMethod]
            public void VertexReturnFalse_NotFound()
            {
                bool actual = graph.RemoveVertex(uint.MaxValue);
                Assert.IsFalse(actual);
                Assert.AreEqual(vertices.Length, graph.VertexCount);
            }

            [TestMethod]
            public void VertexReturnFalse_AdjacentEdges()
            {
                bool actual = graph.RemoveVertex(0);
                Assert.IsFalse(actual);
                Assert.AreEqual(vertices.Length, graph.VertexCount);
            }

            [TestMethod]
            public void ForceVertexReturnTrue()
            {
                bool actual = graph.ForceRemoveVertex(0);
                Assert.IsTrue(actual);
                Assert.AreEqual(vertices.Length - 1, graph.VertexCount);
                Assert.AreEqual(edges.Length - 3, graph.EdgeCount);
            }

            [TestMethod]
            public void ForceVertexReturnFalse()
            {
                bool actual = graph.ForceRemoveVertex(uint.MaxValue);
                Assert.IsFalse(actual);
                Assert.AreEqual(vertices.Length, graph.VertexCount);
                Assert.AreEqual(edges.Length, graph.EdgeCount);
            }
        }

        [TestClass]
        public class TryGet
        {
            static Graph<int, int> graph = new Graph<int, int>(vertices, edges);

            [TestMethod]
            public void TryGetEdgeSuccess()
            {
                bool success = graph.TryGetEdge(edges[0].IDFrom, edges[0].IDTo, out int val);
                Assert.IsTrue(success);
                Assert.AreEqual(edges[0].Value, val);
            }

            [TestMethod]
            public void TryGetEdgeFailure()
            {
                bool success = graph.TryGetEdge(0, uint.MaxValue, out int val);
                Assert.IsFalse(success);
            }

            [TestMethod]
            public void TryGetVertexSuccess()
            {
                var success = graph.TryGetVertex(vertices[0].ID, out int val);
                Assert.IsTrue(success);
                Assert.AreEqual(vertices[0].Value, val);
            }

            [TestMethod]
            public void TryGetVertexFailure()
            {
                var success = graph.TryGetVertex(uint.MaxValue, out int val);
                Assert.IsFalse(success);
            }
        }

        [TestClass]
        public class Contains
        {
            static Graph<int, int> graph = new Graph<int, int>(vertices, edges);

            [TestMethod]
            public void EdgeTrue()
            {
                Assert.IsTrue(graph.ContainsEdge(edges[0].IDFrom, edges[0].IDTo));
            }

            [TestMethod]
            public void EdgeFalse()
            {
                Assert.IsFalse(graph.ContainsEdge(0, uint.MaxValue));
            }

            [TestMethod]
            public void VertexTrue()
            {
                Assert.IsTrue(graph.ContainsVertex(vertices[0].ID));
            }

            [TestMethod]
            public void VertexFalse()
            {
                Assert.IsFalse(graph.ContainsVertex(uint.MaxValue));
            }
        }
    }
}
