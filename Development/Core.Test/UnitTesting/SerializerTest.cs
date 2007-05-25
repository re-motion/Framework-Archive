using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rubicon.Development.UnitTesting;

namespace Rubicon.Development.UnitTests.UnitTesting
{
  [TestFixture]
  public class SerializerTest
  {
    [Test]
    public void SerializeAndDeserialize()
    {
      int[] array = new int[] {1, 2, 3};
      int[] array2 = Serializer.SerializeAndDeserialize (array);
      Assert.AreNotSame (array, array2);

      Assert.AreEqual (array.Length, array2.Length);
      Assert.AreEqual (array[0], array2[0]);
      Assert.AreEqual (array[1], array2[1]);
      Assert.AreEqual (array[2], array2[2]);
    }
  }
}
