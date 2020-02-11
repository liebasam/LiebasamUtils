using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LiebasamUtils.Random.Tests
{
    [TestClass]
    public class TRandomProvider
    {
        [TestClass]
        public class GetSetRNG
        {
            [TestMethod]
            public void SetNullSeed()
            {
                int val = RandomProvider.Next();

                Thread.Sleep(200);
                RandomProvider.RandomSeed = null;
                int val2 = RandomProvider.Next();
                Assert.AreNotEqual(val, val2);

                Thread.Sleep(200);
                RandomProvider.RandomSeed = null;
                val = RandomProvider.Next();
                Assert.AreNotEqual(val2, val);
            }

            [TestMethod]
            public void SetSeed()
            {
                RandomProvider.RandomSeed = 0;
                int val = RandomProvider.Next();
                RandomProvider.RandomSeed = 0;
                int val2 = RandomProvider.Next();
                Assert.AreEqual(val, val2);
            }
        }

        [TestClass]
        public class NextBool
        {
            [TestMethod]
            public void Failure()
            {
                Assert.ThrowsException<ArgumentOutOfRangeException>(
                    () => RandomProvider.NextBool(-0.1));
                Assert.ThrowsException<ArgumentOutOfRangeException>(
                    () => RandomProvider.NextBool(1.1));
            }

            [TestMethod]
            public void Success5050()
            {
                int sum = 0;
                for (int i = 0; i < 10000; i++)
                    sum += RandomProvider.NextBool() ? 1 : 0;
                Assert.AreEqual(sum, 5000, 400);
            }

            [TestMethod]
            public void Success8020()
            {
                int sum = 0;
                for (int i = 0; i < 10000; i++)
                    sum += RandomProvider.NextBool(0.8) ? 1 : 0;
                Assert.AreEqual(sum, 8000, 400);
            }
        }

        [TestClass]
        public class Shuffle
        {
            [TestMethod]
            [ExpectedException(typeof(ArgumentNullException))]
            public void FailureNullArgs() => RandomProvider.Shuffle<double>(null);

            [TestMethod]
            public void Success()
            {
                var arr = new int[] { 1, 2, 3, 4 };
                var sums = new int[4];

                for (int i = 0; i < 1000; i++)
                {
                    RandomProvider.Shuffle(arr);
                    for (int j = 0; j < sums.Length; j++)
                        sums[j] += arr[j];
                }

                int target = (1 + 2 + 3 + 4) * 250;
                int delta = 100;
                Assert.AreEqual(target, sums[0], delta);
                Assert.AreEqual(target, sums[1], delta);
                Assert.AreEqual(target, sums[2], delta);
                Assert.AreEqual(target, sums[3], delta);
            }
        }

        [TestClass]
        public class NextDouble
        {
            [TestMethod]
            public void Failure()
            {
                Assert.ThrowsException<ArgumentOutOfRangeException>(
                    () => RandomProvider.NextDouble(1, 1));
                Assert.ThrowsException<ArgumentOutOfRangeException>(
                    () => RandomProvider.NextDouble(1, 0));
            }

            [TestMethod]
            public void Success()
            {
                double min = 5 - 1e-5;
                double max = 5 + 1e-3;
                for (int i = 0; i < 10000; i++)
                {
                    double choice = RandomProvider.NextDouble(min, max);
                    Assert.IsTrue(choice > min && choice < max);
                }
            }
        }
    }
}
