using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using NMock2;
using NUnit.Framework;

using Rubicon.Security.Metadata;
using Rubicon.Security.UnitTests.TestDomain;
using Rubicon.Utilities;

namespace Rubicon.Security.UnitTests.Metadata
{

  [TestFixture]
  public class EnumerationReflectorTest
  {
    // types

    // static members

    // member fields

    private EnumerationReflector _reflector;

    // construction and disposing

    public EnumerationReflectorTest ()
    {
    }

    // methods and properties

    [SetUp]
    public void SetUp ()
    {
      _reflector = new EnumerationReflector ();
    }

    [Test]
    public void GetValues ()
    {
      List<EnumValueInfo> values = _reflector.GetValues (typeof (Confidentiality));

      Assert.IsNotNull (values);
      Assert.AreEqual (3, values.Count);

      Assert.AreEqual (0, values[0].ID);
      Assert.AreEqual (1, values[1].ID);
      Assert.AreEqual (2, values[2].ID);
      Assert.AreEqual ("Normal", values[0].Name);
      Assert.AreEqual ("Confidential", values[1].Name);
      Assert.AreEqual ("Private", values[2].Name);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), "The type 'System.String' is not an enumerated type.\r\nParameter name: type")]
    public void GetMetadataWithInvalidType ()
    {
      new EnumerationReflector ().GetValues (typeof (string));
    }
  }
}