using System;
using System.Collections.Generic;
using NUnit.Framework;
using System.Collections;
using Rubicon.Utilities;
using NUnit.Framework.SyntaxHelpers;

namespace Rubicon.Core.UnitTests.Utilities
{
  [TestFixture]
  public class EnumerableUtilityTest
  {
    [Test]
    public void Cast()
    {
      int[] sourceArray = new int[] {1, 2, 3};
      IEnumerable sourceEnumerable = sourceArray;
      IEnumerable<int> castEnumerable1 = EnumerableUtility.Cast<int> (sourceEnumerable);
      IEnumerable<object> castEnumerable2 = EnumerableUtility.Cast<object> (sourceEnumerable);
      Assert.IsNotNull (castEnumerable1);
      Assert.IsNotNull (castEnumerable2);
      Assert.That (EnumerableUtility.ToArray (castEnumerable1), Is.EqualTo (sourceArray));
      Assert.That (EnumerableUtility.ToArray (castEnumerable2), Is.EqualTo (sourceArray));
    }
  }
}