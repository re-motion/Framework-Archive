using System;
using NUnit.Framework;
using Rubicon.NullableValueTypes;

namespace Rubicon.Core.UnitTests.NullableValueTypes
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
