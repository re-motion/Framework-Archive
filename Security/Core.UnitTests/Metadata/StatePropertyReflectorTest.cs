using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using Rubicon.Security.Metadata;
using Rubicon.Security.UnitTests.Domain;
using Rubicon.Utilities;
using System.Reflection;

namespace Rubicon.Security.UnitTests.Metadata
{

  [TestFixture]
  public class StatePropertyReflectorTest
  {
    // types

    // static members

    // member fields

    private StatePropertyReflector _reflector;
    private PropertyInfo _confidentialityProperty;

    // construction and disposing

    public StatePropertyReflectorTest ()
    {
    }

    // methods and properties

    [SetUp]
    public void SetUp ()
    {
      _reflector = new StatePropertyReflector ();
      _confidentialityProperty = typeof (PaperFile).GetProperty ("Confidentiality");
      Assert.IsNotNull (_confidentialityProperty);
    }

    [Test]
    public void GetMetadata ()
    {
      StatePropertyInfo info = _reflector.GetMetadata (_confidentialityProperty);

      Assert.IsNotNull (info);
      Assert.AreEqual ("Confidentiality", info.Name);
      Assert.AreEqual (new Guid ("00000000-0000-0000-0000-000000000001"), info.ID);
      Assert.IsNotNull (info.Values);
      Assert.AreEqual (3, info.Values.Count);

      Assert.AreEqual (0, info.Values[0].ID);
      Assert.AreEqual (1, info.Values[1].ID);
      Assert.AreEqual (2, info.Values[2].ID);
      Assert.AreEqual ("Normal", info.Values[0].Name);
      Assert.AreEqual ("Confidential", info.Values[1].Name);
      Assert.AreEqual ("Private", info.Values[2].Name);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), "The property 'ID' of type 'Rubicon.Security.UnitTests.Domain.File' is not of an enumerated type.\r\nParameter name: property")]
    public void GetMetadataWithInvalidType ()
    {
      _reflector.GetMetadata (typeof (PaperFile).GetProperty ("ID"));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), "The property 'SimpleEnum' of type 'Rubicon.Security.UnitTests.Domain.File' does not have the Rubicon.Security.SecurityStateAttribute applied.\r\nParameter name: property")]
    public void GetMetadataWithInvalidEnum ()
    {
      _reflector.GetMetadata (typeof (PaperFile).GetProperty ("SimpleEnum"));
    }
  }
}