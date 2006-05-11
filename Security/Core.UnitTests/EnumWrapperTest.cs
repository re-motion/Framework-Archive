using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using Rubicon.Security.UnitTests.SampleDomain;

namespace Rubicon.Security.UnitTests
{
  [TestFixture]
  public class EnumWrapperTest
  {
    [Test]
    public void Initialize ()
    {
      EnumWrapper wrapper = new EnumWrapper (TestAccessType.First);

      Assert.AreEqual ("First", wrapper.Value);
      Assert.AreEqual ("Rubicon.Security.UnitTests.SampleDomain.TestAccessType, Rubicon.Security.UnitTests", wrapper.TypeName);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        "Enumerated type 'Rubicon.Security.UnitTests.SampleDomain.TestFlags' cannot be wrapped. "
        + "Only enumerated types without the System.FlagsAttribute can be wrapped.\r\nParameter name: value")]
    public void InitializeWithEnumHavingFlagsAttribute ()
    {
      new EnumWrapper (TestFlags.First);
    }

    [Test]
    public void Equals ()
    {
      EnumWrapper expected = new EnumWrapper (TestAccessType.First);
      Assert.IsTrue (expected.Equals (expected));
      Assert.IsTrue (expected.Equals (new EnumWrapper (TestAccessType.First)));
      Assert.IsTrue (new EnumWrapper (TestAccessType.First).Equals (expected));
      Assert.IsFalse (expected.Equals (new EnumWrapper (TestAccessType.Second)));
      Assert.IsFalse (new EnumWrapper (TestAccessType.Second).Equals (expected));
      Assert.IsFalse (expected.Equals (null));
      
      Assert.AreEqual (expected, new EnumWrapper (TestAccessType.First));
      Assert.AreNotEqual (expected, new EnumWrapper (TestAccessType.Second));
    }

    [Test]
    public void TestGetHashCode ()
    {
      Assert.AreEqual (new EnumWrapper (TestAccessType.First).GetHashCode (), new EnumWrapper (TestAccessType.First).GetHashCode ());
    }
  }
}