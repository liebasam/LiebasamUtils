using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LiebasamUtils.Tests
{
    [TestClass]
    public class TFastMath
    {
        static readonly string RHS = "rhs";
        static readonly string LHS = "lhs";
        static readonly string ANS = "ans";
        static readonly float[] Float0 = new float[0];
        static readonly float[] Float1 = new float[1];
        static readonly float[] Float2 = new float[2];
        static readonly float[] Float3 = new float[3];
        static readonly float[] Float123 = new float[] { 1, 2, 3 };
        static readonly float[][] Float123456 = new float[][]
        {
            new float[] { 1, 2, 3 },
            new float[] { 4, 5, 6 }
        };

        [TestClass]
        public class InvalidTypes
        {
            struct MyStruct { }

            [TestMethod]
            [ExpectedException(typeof(NotSupportedException))]
            public void Add() => FastMath.Add(new MyStruct[0], new MyStruct[0]);

            [TestMethod]
            [ExpectedException(typeof(NotSupportedException))]
            public void Multiply() => FastMath.Multiply(new MyStruct[0], new MyStruct[0]);
        }

        [TestClass]
        public class Add
        {
            [TestClass]
            public class Failure
            {
                [TestMethod]
                public void NullLHS()
                {
                    var e = Assert.ThrowsException<ArgumentNullException>(
                        () => FastMath.Add(null, Float0));
                    Assert.AreEqual(LHS, e.ParamName);
                    e = Assert.ThrowsException<ArgumentNullException>(
                        () => FastMath.Add(null, Float0, Float0));
                    Assert.AreEqual(LHS, e.ParamName);
                    e = Assert.ThrowsException<ArgumentNullException>(
                        () => FastMath.Add(null, 0, Float0, 0, Float0, 0, 0));
                    Assert.AreEqual(LHS, e.ParamName);
                }

                [TestMethod]
                public void NullRHS()
                {
                    var e = Assert.ThrowsException<ArgumentNullException>(
                        () => FastMath.Add(Float0, null));
                    Assert.AreEqual(RHS, e.ParamName);
                    e = Assert.ThrowsException<ArgumentNullException>(
                        () => FastMath.Add(Float0, null, Float0));
                    Assert.AreEqual(RHS, e.ParamName);
                    e = Assert.ThrowsException<ArgumentNullException>(
                        () => FastMath.Add(Float0, 0, null, 0, Float0, 0, 0));
                    Assert.AreEqual(RHS, e.ParamName);
                }

                [TestMethod]
                public void NullANS()
                {
                    var e = Assert.ThrowsException<ArgumentNullException>(
                        () => FastMath.Add(Float0, Float0, null));
                    Assert.AreEqual(ANS, e.ParamName);
                    e = Assert.ThrowsException<ArgumentNullException>(
                        () => FastMath.Add(Float0, 0, Float0, 0, null, 0, 0));
                    Assert.AreEqual(ANS, e.ParamName);
                }

                [TestMethod]
                public void InvalidLengths()
                {
                    Assert.ThrowsException<IndexOutOfRangeException>(
                        () => FastMath.Add(Float0, Float1));
                    Assert.ThrowsException<IndexOutOfRangeException>(
                        () => FastMath.Add(Float1, Float0));
                    Assert.ThrowsException<IndexOutOfRangeException>(
                        () => FastMath.Add(Float0, Float0, Float1));
                    Assert.ThrowsException<IndexOutOfRangeException>(
                        () => FastMath.Add(Float0, Float1, Float1));
                    Assert.ThrowsException<IndexOutOfRangeException>(
                        () => FastMath.Add(Float0, Float1, Float0));
                }

                [TestMethod]
                public void InvalidIndices()
                {
                    Assert.ThrowsException<IndexOutOfRangeException>(
                        () => FastMath.Add(Float0, 0, Float0, 0, Float0, 0, 1));
                    Assert.ThrowsException<IndexOutOfRangeException>(
                        () => FastMath.Add(Float1, 1, Float1, 0, Float1, 0, 1));
                    Assert.ThrowsException<IndexOutOfRangeException>(
                        () => FastMath.Add(Float1, 0, Float1, 1, Float1, 0, 1));
                    Assert.ThrowsException<IndexOutOfRangeException>(
                        () => FastMath.Add(Float1, 0, Float1, 0, Float1, 1, 1));
                    Assert.ThrowsException<IndexOutOfRangeException>(
                        () => FastMath.Add(Float1, 0, Float1, 0, Float1, 0, 2));
                }
            }

            [TestClass]
            public class Success
            {
                [TestMethod]
                public void TwoArgs()
                {
                    var ret = FastMath.Add(Float123, Float123);
                    Assert.AreEqual(3, ret.Length);
                    Assert.AreEqual(2, ret[0]);
                    Assert.AreEqual(4, ret[1]);
                    Assert.AreEqual(6, ret[2]);
                }

                [TestMethod]
                public void ThreeArgs()
                {
                    Array.Clear(Float3, 0, 3);
                    FastMath.Add(Float123, Float123, Float3);
                    Assert.AreEqual(2, Float3[0]);
                    Assert.AreEqual(4, Float3[1]);
                    Assert.AreEqual(6, Float3[2]);
                }

                [TestMethod]
                public void SevenArgs()
                {
                    Array.Clear(Float3, 0, 3);
                    FastMath.Add(Float123, 0, Float123, 1, Float3, 0, 2);
                    Assert.AreEqual(3, Float3[0]);
                    Assert.AreEqual(5, Float3[1]);
                    Assert.AreEqual(0, Float3[2]);
                }
            }
        }

        [TestClass]
        public class Multiply
        {
            [TestClass]
            public class Failure
            {
                [TestMethod]
                public void NullLHS()
                {
                    var e = Assert.ThrowsException<ArgumentNullException>(
                        () => FastMath.Multiply(null, Float0));
                    Assert.AreEqual(LHS, e.ParamName);
                    e = Assert.ThrowsException<ArgumentNullException>(
                        () => FastMath.Multiply(null, Float0, Float0));
                    Assert.AreEqual(LHS, e.ParamName);
                    e = Assert.ThrowsException<ArgumentNullException>(
                        () => FastMath.Multiply(null, 0, Float0, 0, Float0, 0, 0));
                    Assert.AreEqual(LHS, e.ParamName);
                }

                [TestMethod]
                public void NullRHS()
                {
                    var e = Assert.ThrowsException<ArgumentNullException>(
                        () => FastMath.Multiply(Float0, null));
                    Assert.AreEqual(RHS, e.ParamName);
                    e = Assert.ThrowsException<ArgumentNullException>(
                        () => FastMath.Multiply(Float0, null, Float0));
                    Assert.AreEqual(RHS, e.ParamName);
                    e = Assert.ThrowsException<ArgumentNullException>(
                        () => FastMath.Multiply(Float0, 0, null, 0, Float0, 0, 0));
                    Assert.AreEqual(RHS, e.ParamName);
                }

                [TestMethod]
                public void NullANS()
                {
                    var e = Assert.ThrowsException<ArgumentNullException>(
                        () => FastMath.Multiply(Float0, Float0, null));
                    Assert.AreEqual(ANS, e.ParamName);
                    e = Assert.ThrowsException<ArgumentNullException>(
                        () => FastMath.Multiply(Float0, 0, Float0, 0, null, 0, 0));
                    Assert.AreEqual(ANS, e.ParamName);
                }

                [TestMethod]
                public void InvalidLengths()
                {
                    Assert.ThrowsException<IndexOutOfRangeException>(
                        () => FastMath.Multiply(Float0, Float1));
                    Assert.ThrowsException<IndexOutOfRangeException>(
                        () => FastMath.Multiply(Float1, Float0));
                    Assert.ThrowsException<IndexOutOfRangeException>(
                        () => FastMath.Multiply(Float0, Float0, Float1));
                    Assert.ThrowsException<IndexOutOfRangeException>(
                        () => FastMath.Multiply(Float0, Float1, Float1));
                    Assert.ThrowsException<IndexOutOfRangeException>(
                        () => FastMath.Multiply(Float0, Float1, Float0));
                }

                [TestMethod]
                public void InvalidIndices()
                {
                    Assert.ThrowsException<IndexOutOfRangeException>(
                        () => FastMath.Multiply(Float0, 0, Float0, 0, Float0, 0, 1));
                    Assert.ThrowsException<IndexOutOfRangeException>(
                        () => FastMath.Multiply(Float1, 1, Float1, 0, Float1, 0, 1));
                    Assert.ThrowsException<IndexOutOfRangeException>(
                        () => FastMath.Multiply(Float1, 0, Float1, 1, Float1, 0, 1));
                    Assert.ThrowsException<IndexOutOfRangeException>(
                        () => FastMath.Multiply(Float1, 0, Float1, 0, Float1, 1, 1));
                    Assert.ThrowsException<IndexOutOfRangeException>(
                        () => FastMath.Multiply(Float1, 0, Float1, 0, Float1, 0, 2));
                }
            }

            [TestClass]
            public class Success
            {
                [TestMethod]
                public void TwoArgs()
                {
                    var ret = FastMath.Multiply(Float123, Float123);
                    Assert.AreEqual(3, ret.Length);
                    Assert.AreEqual(1, ret[0]);
                    Assert.AreEqual(4, ret[1]);
                    Assert.AreEqual(9, ret[2]);
                }

                [TestMethod]
                public void ThreeArgs()
                {
                    Array.Clear(Float3, 0, 3);
                    FastMath.Multiply(Float123, Float123, Float3);
                    Assert.AreEqual(1, Float3[0]);
                    Assert.AreEqual(4, Float3[1]);
                    Assert.AreEqual(9, Float3[2]);
                }

                [TestMethod]
                public void SevenArgs()
                {
                    Array.Clear(Float3, 0, 3);
                    FastMath.Multiply(Float123, 0, Float123, 1, Float3, 0, 2);
                    Assert.AreEqual(2, Float3[0]);
                    Assert.AreEqual(6, Float3[1]);
                    Assert.AreEqual(0, Float3[2]);
                }
            }
        }

        [TestClass]
        public class Dot
        {
            [TestClass]
            public class Failure
            {
                [TestMethod]
                public void NullLHS()
                {
                    var e = Assert.ThrowsException<ArgumentNullException>(
                        () => FastMath.Dot(null, Float0));
                    Assert.AreEqual(LHS, e.ParamName);
                }

                [TestMethod]
                public void NullRHS()
                {
                    var e = Assert.ThrowsException<ArgumentNullException>(
                        () => FastMath.Dot(Float0, null));
                    Assert.AreEqual(RHS, e.ParamName);
                }

                [TestMethod]
                public void InvalidLength()
                {
                    Assert.ThrowsException<IndexOutOfRangeException>(
                        () => FastMath.Dot(Float0, Float1));
                }
            }

            [TestClass]
            public class Success
            {
                [TestMethod]
                public void TwoArgs()
                {
                    var dot = FastMath.Dot(Float123, Float123);
                    Assert.AreEqual(14, dot);
                }
            }
        }

        [TestClass]
        public class MatrixMultiply
        {
            [TestClass]
            public class Failure
            {
                [TestMethod]
                public void NullLHS()
                {
                    var e = Assert.ThrowsException<ArgumentNullException>(
                        () => FastMath.MatrixMultiply(null, Float123456));
                    Assert.AreEqual(LHS, e.ParamName);
                    e = Assert.ThrowsException<ArgumentNullException>(
                        () => FastMath.MatrixMultiply(null, Float123456, Float2));
                    Assert.AreEqual(LHS, e.ParamName);
                }

                [TestMethod]
                public void NullRHS()
                {
                    var e = Assert.ThrowsException<ArgumentNullException>(
                        () => FastMath.MatrixMultiply(Float0, null));
                    Assert.AreEqual(RHS, e.ParamName);
                    e = Assert.ThrowsException<ArgumentNullException>(
                        () => FastMath.MatrixMultiply(Float0, null, Float0));
                    Assert.AreEqual(RHS, e.ParamName);
                }

                [TestMethod]
                public void NullANS()
                {
                    var e = Assert.ThrowsException<ArgumentNullException>(
                        () => FastMath.MatrixMultiply(Float0, Float123456, null));
                    Assert.AreEqual(ANS, e.ParamName);
                }

                [TestMethod]
                public void InvalidLengths()
                {
                    Assert.ThrowsException<IndexOutOfRangeException>(
                        () => FastMath.MatrixMultiply(Float0, Float123456));
                    Assert.ThrowsException<IndexOutOfRangeException>(
                        () => FastMath.MatrixMultiply(Float1, Float123456));
                    Assert.ThrowsException<IndexOutOfRangeException>(
                        () => FastMath.MatrixMultiply(Float3, Float123456, Float1));
                    Assert.ThrowsException<IndexOutOfRangeException>(
                        () => FastMath.MatrixMultiply(Float1, Float123456, Float2));
                }

                [TestMethod]
                public void EqualParams()
                {
                    Assert.ThrowsException<NotSupportedException>(
                        () => FastMath.MatrixMultiply(Float0, Float123456, Float0));
                    Assert.ThrowsException<NotSupportedException>(
                        () => FastMath.MatrixMultiply(
                            Float0,
                            new float[][] { Float0 },
                            Float1));
                }
            }

            [TestClass]
            public class Success
            {
                [TestMethod]
                public void TwoArgs()
                {
                    var ret = FastMath.MatrixMultiply(Float123, Float123456);
                    Assert.AreEqual(2, ret.Length);
                    Assert.AreEqual(14, ret[0]);
                    Assert.AreEqual(32, ret[1]);
                }

                [TestMethod]
                public void ThreeArgs()
                {
                    Array.Clear(Float2, 0, 2);
                    FastMath.MatrixMultiply(Float123, Float123456, Float2);
                    Assert.AreEqual(14, Float2[0]);
                    Assert.AreEqual(32, Float2[1]);
                }
            }
        }
    }
}
