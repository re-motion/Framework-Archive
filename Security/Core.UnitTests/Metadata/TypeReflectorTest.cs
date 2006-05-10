using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using Rubicon.Security.Metadata;
using Rubicon.Security.UnitTests.Domain;
using Rubicon.Utilities;

namespace Rubicon.Security.UnitTests.Metadata
{

  [TestFixture]
  public class TypeReflectorTest
  {
    // types

    // static members

    // member fields

    private TypeReflector _reflector;

    // construction and disposing

    public TypeReflectorTest ()
    {
    }

    // methods and properties

    [SetUp]
    public void SetUp ()
    {
      _reflector = new TypeReflector ();
    }

    [Test]
    public void GetMetadata ()
    {
      SecurableTypeInfo info = _reflector.GetMetadata (typeof (PaperFile));

      Assert.IsNotNull (info);
      Assert.AreEqual ("Rubicon.Security.UnitTests.Domain.PaperFile", info.Name);
      Assert.AreEqual (new Guid ("00000000-0000-0000-0002-000000000000"), info.ID);
      Assert.IsNotNull (info.Properties);
      Assert.AreEqual (2, info.Properties.Count);

      StatePropertyInfo confidentialityProperty = info.Properties[0];
      Assert.IsNotNull (confidentialityProperty);
      Assert.AreEqual ("Confidentiality", confidentialityProperty.Name);

      StatePropertyInfo stateProperty = info.Properties[1];
      Assert.IsNotNull (stateProperty);
      Assert.AreEqual ("State", stateProperty.Name);
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException))]
    public void GetMetadataWithInvalidType ()
    {
      _reflector.GetMetadata (typeof (SimpleType));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), "Value types are not supported.\r\nParameter name: type")]
    public void GetMetadataWithInvalidValueType ()
    {
      _reflector.GetMetadata (typeof (TestValueType));
    }
  }
}