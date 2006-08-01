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
      EnumWrapper wrapper = new EnumWrapper (TestAccessTypes.First);

      Assert.AreEqual ("First", wrapper.Name);
      Assert.AreEqual ("Rubicon.Security.UnitTests.SampleDomain.TestAccessTypes, Rubicon.Security.UnitTests", wrapper.TypeName);
    }

    [Test]
    public void InitializeFromString ()
    {
      EnumWrapper wrapper = new EnumWrapper ("First", "Rubicon.Security.UnitTests::SampleDomain.TestAccessTypes");

      Assert.AreEqual ("First", wrapper.Name);
      Assert.AreEqual ("Rubicon.Security.UnitTests.SampleDomain.TestAccessTypes, Rubicon.Security.UnitTests", wrapper.TypeName);
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
      EnumWrapper expected = new EnumWrapper (TestAccessTypes.First);

      Assert.IsTrue (expected.Equals (expected));
      
      Assert.IsTrue (expected.Equals (new EnumWrapper (TestAccessTypes.First)));
      Assert.IsTrue (new EnumWrapper (TestAccessTypes.First).Equals (expected));

      Assert.IsTrue (expected.Equals (new EnumWrapper ("First", "Rubicon.Security.UnitTests.SampleDomain.TestAccessTypes, Rubicon.Security.UnitTests")));
      Assert.IsTrue (new EnumWrapper ("First", "Rubicon.Security.UnitTests.SampleDomain.TestAccessTypes, Rubicon.Security.UnitTests").Equals (expected));
      
      Assert.IsFalse (expected.Equals (new EnumWrapper (TestAccessTypes.Second)));
      Assert.IsFalse (new EnumWrapper (TestAccessTypes.Second).Equals (expected));
      
      Assert.IsFalse (expected.Equals (null));

      Assert.AreEqual (expected, new EnumWrapper (TestAccessTypes.First));
      Assert.AreNotEqual (expected, new EnumWrapper (TestAccessTypes.Second));
    }

    [Test]
    public void TestGetHashCode ()
    {
      EnumWrapper expected = new EnumWrapper (TestAccessTypes.First);

      Assert.AreEqual (expected.GetHashCode (), new EnumWrapper (TestAccessTypes.First).GetHashCode ());
      Assert.AreEqual (expected.GetHashCode (), new EnumWrapper ("First", "Rubicon.Security.UnitTests.SampleDomain.TestAccessTypes, Rubicon.Security.UnitTests").GetHashCode ());
    }

    [Test]
    [ExpectedException (typeof (TypeLoadException))]
    public void GetEnum_FromInvalidTypeName ()
    {
      EnumWrapper wrapper = new EnumWrapper ("First", "Rubicon.Security.UnitTests::SampleDomain.Invalid");

      wrapper.GetEnum ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), "The type 'Rubicon.Security.UnitTests.SampleDomain.SimpleType, Rubicon.Security.UnitTests' is not an enumerated type.")]
    public void GetEnum_FromTypeNotEnum ()
    {
      EnumWrapper wrapper = new EnumWrapper ("First", "Rubicon.Security.UnitTests::SampleDomain.SimpleType");

      wrapper.GetEnum ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), "The enumerated type 'Rubicon.Security.UnitTests.SampleDomain.TestAccessTypes, Rubicon.Security.UnitTests' does not define the value 'Invalid'.")]
    public void GetEnum_FromInvalidName ()
    {
      EnumWrapper wrapper = new EnumWrapper ("Invalid", "Rubicon.Security.UnitTests::SampleDomain.TestAccessTypes");

      wrapper.GetEnum ();
    }

    [Test]
    public void ConvertToString ()
    {
      EnumWrapper wrapper = new EnumWrapper ("Name", "Namespace.TypeName, Assembly");

      Assert.AreEqual ("Name|Namespace.TypeName, Assembly", wrapper.ToString ());
    }

    [Test]
    public void Parse ()
    {
      EnumWrapper wrapper = EnumWrapper.Parse ("Name|Namespace.TypeName, Assembly");

      Assert.AreEqual ("Namespace.TypeName, Assembly", wrapper.TypeName);
      Assert.AreEqual ("Name", wrapper.Name);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), "The value 'Name' did not contain the type name of the enumerated value. Expected format: 'Name|TypeName'\r\nParameter name: value")]
    public void Parse_WithMissingPipe ()
    {
      EnumWrapper.Parse ("Name");
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), "The value '|Namespace.TypeName, Assembly' did not contain the name of the enumerated value. Expected format: 'Name|TypeName'\r\nParameter name: value")]
    public void Parse_WithMissingName ()
    {
      EnumWrapper.Parse ("|Namespace.TypeName, Assembly");
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), "The value 'Name|' did not contain the type name of the enumerated value. Expected format: 'Name|TypeName'\r\nParameter name: value")]
    public void Parse_WithMissingTypeName ()
    {
      EnumWrapper.Parse ("Name|");
    }
  }
}