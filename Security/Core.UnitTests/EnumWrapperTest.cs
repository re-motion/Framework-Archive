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
    public void InitializeFromEnum ()
    {
      EnumWrapper wrapper = new EnumWrapper (TestAccessType.First);

      Assert.AreEqual ("First", wrapper.Name);
      Assert.AreEqual ("Rubicon.Security.UnitTests.SampleDomain.TestAccessType, Rubicon.Security.UnitTests", wrapper.TypeName);
    }

    [Test]
    public void InitializeFromString ()
    {
      EnumWrapper wrapper = new EnumWrapper ("Rubicon.Security.UnitTests::SampleDomain.TestAccessType", "First");

      Assert.AreEqual ("First", wrapper.Name);
      Assert.AreEqual ("Rubicon.Security.UnitTests.SampleDomain.TestAccessType, Rubicon.Security.UnitTests", wrapper.TypeName);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        "Enumerated type 'Rubicon.Security.UnitTests.SampleDomain.TestFlags' cannot be wrapped. "
        + "Only enumerated types without the System.FlagsAttribute can be wrapped.\r\nParameter name: enumValue")]
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

    [Test]
    public void GetEnum_InitializedWithEnum ()
    {
      EnumWrapper wrapper = new EnumWrapper (TestAccessType.First);

      Assert.AreEqual (TestAccessType.First, wrapper.GetEnum ());
    }

    [Test]
    public void GetEnum_InitializedWithString ()
    {
      EnumWrapper wrapper = new EnumWrapper ("Rubicon.Security.UnitTests::SampleDomain.TestAccessType", "First");

      Assert.AreEqual (TestAccessType.First, wrapper.GetEnum ());
    }

    [Test]
    [ExpectedException (typeof (TypeLoadException))]
    public void GetEnum_FromInvalidTypeName ()
    {
      EnumWrapper wrapper = new EnumWrapper ("Rubicon.Security.UnitTests::SampleDomain.Invalid", "First");

      wrapper.GetEnum ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), "The type 'Rubicon.Security.UnitTests.SampleDomain.SimpleType, Rubicon.Security.UnitTests' is not an enumerated type.")]
    public void GetEnum_FromTypeNotEnum ()
    {
      EnumWrapper wrapper = new EnumWrapper ("Rubicon.Security.UnitTests::SampleDomain.SimpleType", "First");

      wrapper.GetEnum ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), "The enumerated type 'Rubicon.Security.UnitTests.SampleDomain.TestAccessType, Rubicon.Security.UnitTests' does not define the value 'Invalid'.")]
    public void GetEnum_FromInvalidName ()
    {
      EnumWrapper wrapper = new EnumWrapper ("Rubicon.Security.UnitTests::SampleDomain.TestAccessType", "Invalid");

      wrapper.GetEnum ();
    }

  }
}