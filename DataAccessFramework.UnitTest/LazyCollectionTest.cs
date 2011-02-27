using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace DataAccessFramework.UnitTest
{
    public class IntEnumerable
    {
        private int _accessCount;
        public IntEnumerable()
        {
            _accessCount = 0;
        }

        public IEnumerable<int> GetElements()
        {
            _accessCount++;
            yield return 1;
            yield return 2;
            yield return 3;
        }

        public int AccessCount { get { return _accessCount; } }
    }

    [TestFixture]
    public class LazyCollectionTest
    {
        [Test]
        public void EnumerableIsNotAccessedWhenListIsNotAccessed()
        {
            var host = new IntEnumerable();
            ICollection<int> collection = new LazyCollection<int>(
                host.GetElements());

            // Exercise
            var array = collection.ToArray();
            var array2 = collection.ToArray();
            // Validate
            Assert.That(host.AccessCount, Is.EqualTo(1));
        }
    }
}
