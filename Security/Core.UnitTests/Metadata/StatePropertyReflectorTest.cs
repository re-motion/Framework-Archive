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
    private IEnumerationReflector _enumeratedTypeReflectorMock;
    private StatePropertyReflector _statePropertyReflector;
    private MetadataCache _cache;

    // construction and disposing

    public StatePropertyReflectorTest ()
    {
    }

    // methods and properties

    [SetUp]
    public void SetUp ()
    {
      _mocks = new Mockery ();
      _enumeratedTypeReflectorMock = _mocks.NewMock<IEnumerationReflector> ();
      _statePropertyReflector = new StatePropertyReflector (_enumeratedTypeReflectorMock);
      _cache = new MetadataCache ();
    }

    [Test]
    public void Initialize ()
    {
      Assert.IsInstanceOfType (typeof (IStatePropertyReflector), _statePropertyReflector);
      Assert.AreSame (_enumeratedTypeReflectorMock, _statePropertyReflector.EnumerationTypeReflector);
    }

    [Test]
    public void GetMetadata ()
    {
      Dictionary<Enum, EnumValueInfo> values = new Dictionary<Enum, EnumValueInfo> ();
      values.Add (Confidentiality.Normal, PropertyStates.Normal);
      values.Add (Confidentiality.Confidential, PropertyStates.Confidential);
      values.Add (Confidentiality.Private, PropertyStates.Private);

      Expect.Once.On (_enumeratedTypeReflectorMock)
          .Method ("GetValues")
          .With (typeof (Confidentiality), _cache)
          .Will (Return.Value (values));

      StatePropertyInfo info = _statePropertyReflector.GetMetadata (typeof (PaperFile).GetProperty ("Confidentiality"), _cache);

      _mocks.VerifyAllExpectationsHaveBeenMet ();

      Assert.IsNotNull (info);
      Assert.AreEqual ("Confidentiality", info.Name);
      Assert.AreEqual ("00000000-0000-0000-0001-000000000001", info.ID);
      
      Assert.IsNotNull (info.Values);
      Assert.AreEqual (3, info.Values.Count);
      Assert.Contains (PropertyStates.Normal, info.Values);
      Assert.Contains (PropertyStates.Private, info.Values);
      Assert.Contains (PropertyStates.Confidential, info.Values);
    }

    [Test]
    public void GetMetadataFromCache ()
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