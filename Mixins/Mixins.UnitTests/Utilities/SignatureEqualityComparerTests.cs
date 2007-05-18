using System;
using Mixins.Utilities;
using NUnit.Framework;
using System.Reflection;
using Mixins.UnitTests.SampleTypes;
using System.Collections.Generic;

namespace Mixins.UnitTests.Utilities
{
  [TestFixture]
  public class SignatureEqualityComparerTests
  {
    [Test]
    [Ignore("TODO")]
    public void EqualsForMethodSignatures()
    {
      MethodInfo m1 = typeof (IBaseType31).GetMethod ("IfcMethod");
      MethodInfo m2 = typeof (IBaseType32).GetMethod ("IfcMethod");
      MethodInfo m3 = typeof (IBaseType35).GetMethod ("IfcMethod2");
      MethodInfo m4 = typeof (BaseType4).GetMethod ("NonVirtualMethod");

      IEqualityComparer<MethodInfo> comparer = new MethodSignatureEqualityComparer();

      Assert.IsTrue (comparer.Equals (m1, m1));
      Assert.IsTrue (comparer.Equals (m1, m2));
      Assert.IsTrue (comparer.Equals (m1, m3));
      Assert.IsFalse (comparer.Equals (m1, m4));
      Assert.IsFalse (comparer.Equals (m2, m4));
      Assert.IsTrue (comparer.Equals (m4, m4));
    }
  }
}
