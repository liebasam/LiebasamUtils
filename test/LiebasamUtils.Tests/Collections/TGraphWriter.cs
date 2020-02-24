using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LiebasamUtils.Collections.Tests
{
    [TestClass]
    public class TGraphWriter
    {
        static readonly string PropName = "p";
        static readonly string PropValue = "v";
        static readonly string SpaceStr = " ";
        static readonly string LabelStr = "label";
        static readonly string NameStr = "name";
        static readonly Graph<int, int> graph = new Graph<int, int>(
            new Vertex<int>[]
            {
                new Vertex<int>(0, 0),
                new Vertex<int>(1, 1),
                new Vertex<int>(3, 3)
            },
            new Edge<int>[]
            {
                new Edge<int>(0, 3, 10),
                new Edge<int>(1, 3, 11),
                new Edge<int>(3, 0, 12)
            });

        [TestClass]
        public class Constructor
        {
            [TestMethod]
            public void NullArgs()
            {
                Assert.ThrowsException<ArgumentNullException>(
                    () => new GraphWriter<int, int>(null));
                Assert.ThrowsException<ArgumentNullException>(
                    () => new GraphWriter<int, int>(graph, null));
                Assert.ThrowsException<ArgumentNullException>(
                    () => new GraphWriter<int, int>(null, string.Empty));
            }

            [TestMethod]
            public void Success()
            {
                var gw = new GraphWriter<int, int>(graph);
                Assert.AreEqual(string.Empty, gw.GetGraphProperty(NameStr));
                Assert.AreEqual(0, gw.GetVertexProperty(0, LabelStr));
                Assert.AreEqual(10, gw.GetEdgeProperty(0, 3, LabelStr));
            }

            [TestMethod]
            public void SuccessName()
            {
                var gw = new GraphWriter<int, int>(graph, PropValue);
                Assert.AreEqual(PropValue, gw.GetGraphProperty(NameStr));
                Assert.AreEqual(0, gw.GetVertexProperty(0, LabelStr));
                Assert.AreEqual(10, gw.GetEdgeProperty(0, 3, LabelStr));
            }
        }

        [TestClass]
        public class VertexProperty
        {
            GraphWriter<int, int> writer;
            [TestInitialize]
            public void Setup()
            {
                writer = new GraphWriter<int, int>(graph);
            }

            [TestMethod]
            public void NullArgs()
            {
                Assert.ThrowsException<ArgumentNullException>(
                    () => writer.SetVertexProperty(0, null, 0));
                Assert.ThrowsException<ArgumentException>(
                    () => writer.SetVertexProperty(0, string.Empty, 0));
                Assert.ThrowsException<ArgumentException>(
                    () => writer.SetVertexProperty(0, SpaceStr, 0));
                Assert.ThrowsException<ArgumentNullException>(
                    () => writer.SetVertexProperty(0, PropName, null));
            }

            [TestMethod]
            [ExpectedException(typeof(KeyNotFoundException))]
            public void Failure()
            {
                writer.GetVertexProperty(0, PropName);
            }

            [TestMethod]
            public void Success()
            {
                writer.SetVertexProperty(0, PropName, PropValue);
                Assert.AreEqual(PropValue, writer.GetVertexProperty(0, PropName));
            }

            [TestMethod]
            public void SuccessOverride()
            {
                writer.SetVertexProperty(0, PropName, 10);
                writer.SetVertexProperty(0, PropName, PropValue);
                Assert.AreEqual(PropValue, writer.GetVertexProperty(0, PropName));
            }
        }

        [TestClass]
        public class EdgeProperty
        {
            GraphWriter<int, int> writer;
            [TestInitialize]
            public void Setup()
            {
                writer = new GraphWriter<int, int>(graph);
            }

            [TestMethod]
            public void NullArgs()
            {
                Assert.ThrowsException<ArgumentNullException>(
                    () => writer.SetEdgeProperty(0, 3, null, 0));
                Assert.ThrowsException<ArgumentException>(
                    () => writer.SetEdgeProperty(0, 3, string.Empty, 0));
                Assert.ThrowsException<ArgumentException>(
                    () => writer.SetEdgeProperty(0, 3, SpaceStr, 0));
                Assert.ThrowsException<ArgumentNullException>(
                    () => writer.SetEdgeProperty(0, 3, PropName, null));
            }

            [TestMethod]
            [ExpectedException(typeof(KeyNotFoundException))]
            public void Failure()
            {
                writer.GetEdgeProperty(0, 3, PropName);
            }

            [TestMethod]
            public void Success()
            {
                writer.SetEdgeProperty(0, 3, PropName, PropValue);
                Assert.AreEqual(PropValue, writer.GetEdgeProperty(0, 3, PropName));
            }

            [TestMethod]
            public void SuccessOverride()
            {
                writer.SetEdgeProperty(0, 3, PropName, 10);
                writer.SetEdgeProperty(0, 3, PropName, PropValue);
                Assert.AreEqual(PropValue, writer.GetEdgeProperty(0, 3, PropName));
            }
        }

        [TestClass]
        public class GraphProperty
        {
            GraphWriter<int, int> writer;
            [TestInitialize]
            public void Setup()
            {
                writer = new GraphWriter<int, int>(graph);
            }

            [TestMethod]
            public void NullArgs()
            {
                Assert.ThrowsException<ArgumentNullException>(
                    () => writer.SetGraphProperty(null, 0));
                Assert.ThrowsException<ArgumentException>(
                    () => writer.SetGraphProperty(string.Empty, 0));
                Assert.ThrowsException<ArgumentException>(
                    () => writer.SetGraphProperty(SpaceStr, 0));
                Assert.ThrowsException<ArgumentNullException>(
                    () => writer.SetGraphProperty(PropName, null));
            }

            [TestMethod]
            [ExpectedException(typeof(KeyNotFoundException))]
            public void Failure()
            {
                writer.GetGraphProperty(PropName);
            }

            [TestMethod]
            public void Success()
            {
                writer.SetGraphProperty(PropName, PropValue);
                Assert.AreEqual(PropValue, writer.GetGraphProperty(PropName));
            }

            [TestMethod]
            public void SuccessOverride()
            {
                writer.SetGraphProperty(PropName, 10);
                writer.SetGraphProperty(PropName, PropValue);
                Assert.AreEqual(PropValue, writer.GetGraphProperty(PropName));
            }

            [TestMethod]
            public void SuccessName()
            {
                writer.SetGraphProperty(NameStr, PropValue);
                Assert.AreEqual(PropValue, writer.GetGraphProperty(NameStr));
            }
        }

        [TestClass]
        public class TToString
        {
            GraphWriter<int, int> writer;
            [TestInitialize]
            public void Setup()
            {
                writer = new GraphWriter<int, int>(graph);
                writer.SetGraphProperty(NameStr, NameStr);
            }

            [TestMethod]
            public void Success()
            {
                Console.WriteLine(writer.ToString());
            }
        }

        [TestClass]
        public class Compile
        {
            GraphWriter<int, int> writer;
            [TestInitialize]
            public void Setup()
            {
                writer = new GraphWriter<int, int>(graph);
            }

            [TestMethod]
            [ExpectedException(typeof(ArgumentNullException))]
            public void NullArgs() => writer.Compile(null);

            [TestMethod]
            public void InvalidArgs()
            {
                Assert.ThrowsException<ArgumentException>(
                    () => writer.Compile(SpaceStr));
                Assert.ThrowsException<ArgumentException>(
                    () => writer.Compile(string.Empty));
            }

            [TestMethod]
            public void Success()
            {
                var file = Path.GetTempFileName();
                file = Path.ChangeExtension(file, ".png");
                writer.Compile(file);
                Console.WriteLine(file);
            }
        }
    }
}
