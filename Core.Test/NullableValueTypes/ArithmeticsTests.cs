using System;
using NUnit.Framework;
using Rubicon.Data.NullableValueTypes;

namespace Rubicon.Data.NullableValueTypes.UnitTests
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
