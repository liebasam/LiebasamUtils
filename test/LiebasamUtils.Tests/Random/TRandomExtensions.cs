using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LiebasamUtils.Random.Tests
{
    [TestClass]
    public class TExtensions
    {
        [TestClass]
        public class ChooseUniform
        {
            [TestMethod]
            [ExpectedException(typeof(ArgumentNullException))]
            public void FailureNullArgs() => RandomExtensions.ChooseUniform<float>(null);

            [TestMethod]
            public void Success()
            {
                var nums = Enumerable.Range(1, 4);
                int sum = 0;
                for (int i = 0; i < 1000; i++)
                    sum += nums.ChooseUniform();
                Assert.AreEqual((1 + 2 + 3 + 4) * 250, sum, 100);
            }
        }

        [TestClass]
        public class Shuffle
        {
            [TestMethod]
            [ExpectedException(typeof(ArgumentNullException))]
            public void FailureNullArgs() => RandomExtensions.Shuffle<int>(null).ToArray();

            [TestMethod]
            public void Success()
            {
                var arr = Enumerable.Range(1, 4);
                var sums = new int[4];

                for (int i = 0; i < 1000; i++)
                {
                    arr = arr.Shuffle().ToArray();
                    for (int j = 0; j < sums.Length; j++)
                        sums[j] += arr.ElementAt(j);
                }

                int target = (1 + 2 + 3 + 4) * 250;
                int delta = 100;
                Assert.AreEqual(target, sums[0], delta);
                Assert.AreEqual(target, sums[1], delta);
                Assert.AreEqual(target, sums[2], delta);
                Assert.AreEqual(target, sums[3], delta);
            }
        }
    }
}
