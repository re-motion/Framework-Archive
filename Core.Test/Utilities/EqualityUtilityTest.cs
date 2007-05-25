using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rubicon.Utilities;

namespace Rubicon.Core.UnitTests.Utilities
{
  [TestFixture]
  public class EqualityUtilityTest
  {
    [Test]
    public void GetRotatedHashCodeForEnumerable()
    {
      IEnumerable objects1 = new int[] {1, 2, 3};
      IEnumerable objects2 = new int[] {1, 2, 3};
      Assert.AreEqual (EqualityUtility.GetRotatedHashCode (objects1), EqualityUtility.GetRotatedHashCode (objects2));

      IEnumerable objects3 = new int[] {3, 2, 1};
      Assert.AreNotEqual (EqualityUtility.GetRotatedHashCode (objects1), EqualityUtility.GetRotatedHashCode (objects3));

      IEnumerable objects4 = new int[] { 1, 2, 17 };
      Assert.AreNotEqual (EqualityUtility.GetRotatedHashCode (objects1), EqualityUtility.GetRotatedHashCode (objects4));
    }
  }
}
