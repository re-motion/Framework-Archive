using System;
using NUnit.Framework;
using Remotion.NullableValueTypes;

namespace Remotion.UnitTests.NullableValueTypes
{

[TestFixture]  
public class EqualityTests
{
  [Test]
  public void NaInt32Equality ()
  {
    CheckEquals (NaInt32.Null);
    CheckEquals (NaInt32.Zero);
    CheckEquals (NaInt32.MinValue);
    CheckEquals (NaInt32.MaxValue);

    CheckNotEquals (NaInt32.Null, NaInt32.Zero);
    CheckNotEquals (NaInt32.Null, NaInt32.MinValue);

    CheckNotEquals (NaInt32.Zero, NaInt32.Null);
    CheckNotEquals (NaInt32.MinValue, NaInt32.Null);

    Assert.IsTrue (new NaInt32 (2) == 2); // implicit conversion
    Assert.IsTrue (  new NaInt32 (2).Equals (2)); // implicit conversion
    Assert.IsTrue (! new NaInt32 (2).Equals ((object)2)); // different types cannot be equal
    Assert.IsTrue (! NaInt32.Null.Equals (null)); // NaInt32.Null != null
  }

  [Test]
  public void NaInt32CompareTo()
  {
    Assert.IsTrue ( new NaInt32 (3).CompareTo (new NaInt32 (4)) < 0);
    Assert.IsTrue ( NaInt32.Null.CompareTo (new NaInt32 (4)) < 0);

    Assert.IsTrue ( new NaInt32 (3).CompareTo (new NaInt32 (3)) == 0);
    Assert.IsTrue ( NaInt32.Null.CompareTo (NaInt32.Null) == 0);

    Assert.IsTrue ( new NaInt32 (4).CompareTo (new NaInt32 (3)) > 0);
    Assert.IsTrue ( new NaInt32(4).CompareTo (NaInt32.Null) > 0);

    Assert.IsTrue ( new NaInt32 (3).CompareTo (null) > 0);
    Assert.IsTrue ( NaInt32.Null.CompareTo (null) == 0);
  }

  public void CheckEquals (NaInt32 x)
  {
    CheckEquals (x, x, true);
  }

  public void CheckNotEquals (NaInt32 x, NaInt32 y)
  {
    CheckEquals (x, y, false);
  }

  public void CheckEquals (NaInt32 x, NaInt32 y, bool equals)
  {
    bool result;

    result = x.Equals (y);
    Assert.AreEqual (equals, result, "Equals (NaInt32) failed");

    result = NaInt32.Equals (x, y);
    Assert.AreEqual (equals, result, "static Equals (NaInt32, NaInt32) failed");

    result = x == y;
    Assert.AreEqual (equals, result, "operator == failed");

    result = x != y;
    Assert.AreEqual (!equals, result, "operatpr != failed");
  }
}


}
