using System;
using NUnit.Framework;
using Remotion.NullableValueTypes;

namespace Remotion.UnitTests.NullableValueTypes
{

[TestFixture]
public class ArithmeticsTests
{
  [Test]
  [ExpectedException (typeof (DivideByZeroException))]
  public void TestNaDoubleDivByZero()
  {
    NaDouble d = new NaDouble(3) / 0;
  }
}


}
