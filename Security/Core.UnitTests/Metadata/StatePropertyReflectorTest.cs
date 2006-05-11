using System;
using System.Collections.Generic;
using System.Text;

using NMock2;
using NUnit.Framework;

using Rubicon.Security.Metadata;
using Rubicon.Security.UnitTests.TestDomain;
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

    private Mockery _mocks;
    private IEnumeratedTypeReflector _enumeratedTypeReflectorMock;
    private StatePropertyReflector _reflector;
    private MetadataCache _cache;
    private EnumValueInfo _valueNormal;
    private EnumValueInfo _valuePrivate;
    private EnumValueInfo _valueSecret;

    // construction and disposing

    public StatePropertyReflectorTest ()
    {
    }

    // methods and properties

    [SetUp]
    public void SetUp ()
    {
      _mocks = new Mockery ();
      _enumeratedTypeReflectorMock = _mocks.NewMock<IEnumeratedTypeReflector> ();
      _reflector = new StatePropertyReflector (_enumeratedTypeReflectorMock);
      _cache = new MetadataCache ();

      _valueNormal = new EnumValueInfo (0, "Normal");
      _valuePrivate = new EnumValueInfo (1, "Private");
      _valueSecret = new EnumValueInfo (1, "Private");
    }

    [Test]
    public void Initialize ()
    {
      Assert.AreSame (_enumeratedTypeReflectorMock, _reflector.EnumeratedTypeReflector);
    }

    [Test]
    public void GetMetadata ()
    {
      Expect.Once.On (_enumeratedTypeReflectorMock)
          .Method ("GetValues")
          .With (typeof (Confidentiality))
          .Will (Return.Value (new List<EnumValueInfo> (new EnumValueInfo[] { _valueNormal, _valuePrivate, _valueSecret })));

      StatePropertyInfo info = _reflector.GetMetadata (typeof (PaperFile).GetProperty ("Confidentiality"), _cache);

      _mocks.VerifyAllExpectationsHaveBeenMet ();

      Assert.IsNotNull (info);
      Assert.AreEqual ("Confidentiality", info.Name);
      Assert.AreEqual (new Guid ("00000000-0000-0000-0001-000000000001"), info.ID);
      
      Assert.IsNotNull (info.Values);
      Assert.AreEqual (3, info.Values.Count);
      Assert.Contains (_valueNormal, info.Values);
      Assert.Contains (_valuePrivate, info.Values);
      Assert.Contains (_valueSecret, info.Values);
    }

    [Test]
    public void GetMetadataForCache ()
    {
      StatePropertyReflector reflector = new StatePropertyReflector ();
      reflector.GetMetadata (typeof (PaperFile).GetProperty ("Confidentiality"), _cache);
      reflector.GetMetadata (typeof (File).GetProperty ("Confidentiality"), _cache);

      StatePropertyInfo paperFileConfidentialityInfo = _cache.GetStatePropertyInfo (typeof (PaperFile).GetProperty ("Confidentiality"));
      Assert.IsNotNull (paperFileConfidentialityInfo);
      Assert.AreEqual ("Confidentiality", paperFileConfidentialityInfo.Name);
      Assert.AreSame (paperFileConfidentialityInfo, _cache.GetStatePropertyInfo (typeof (File).GetProperty ("Confidentiality")));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), "The type of the property 'ID' in type 'Rubicon.Security.UnitTests.TestDomain.File' is not an enumerated type.\r\nParameter name: property")]
    public void GetMetadataWithInvalidType ()
    {
      new StatePropertyReflector().GetMetadata (typeof (PaperFile).GetProperty ("ID"), _cache);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), "The type of the property 'SimpleEnum' in type 'Rubicon.Security.UnitTests.TestDomain.File' does not have the Rubicon.Security.SecurityStateAttribute applied.\r\nParameter name: property")]
    public void GetMetadataWithInvalidEnum ()
    {
      new StatePropertyReflector ().GetMetadata (typeof (PaperFile).GetProperty ("SimpleEnum"), _cache);
    }
  }
}