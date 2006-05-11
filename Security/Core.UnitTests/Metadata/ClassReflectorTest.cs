using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using NMock2;
using NUnit.Framework;

using Rubicon.Security.Metadata;
using Rubicon.Security.UnitTests.SampleDomain;
using Rubicon.Security.UnitTests.TestDomain;
using Rubicon.Utilities;

namespace Rubicon.Security.UnitTests.Metadata
{

  [TestFixture]
  public class ClassReflectorTest
  {
    // types

    // static members

    // member fields

    private Mockery _mocks;
    private IStatePropertyReflector _statePropertyReflectorMock;
    private ClassReflector _reflector;
    private MetadataCache _cache;
    private StatePropertyInfo _confidentialityProperty;
    private StatePropertyInfo _stateProperty;

    // construction and disposing

    public ClassReflectorTest ()
    {
    }

    // methods and properties

    [SetUp]
    public void SetUp ()
    {
      _mocks = new Mockery ();
      _statePropertyReflectorMock = _mocks.NewMock<IStatePropertyReflector> ();
      _reflector = new ClassReflector (_statePropertyReflectorMock);
      _cache = new MetadataCache ();

      _confidentialityProperty = new StatePropertyInfo ();
      _confidentialityProperty.ID = new Guid ();
      _confidentialityProperty.Name = "Confidentiality";

      _stateProperty = new StatePropertyInfo ();
      _stateProperty.ID = new Guid ();
      _stateProperty.Name = "State";
    }

    [Test]
    public void Initialize ()
    {
      Assert.AreSame (_statePropertyReflectorMock, _reflector.StatePropertyReflector);
    }

    [Test]
    public void GetMetadata ()
    {
      Expect.Once.On (_statePropertyReflectorMock)
          .Method ("GetMetadata")
          .With (typeof (PaperFile).GetProperty ("Confidentiality"), _cache)
          .Will (Return.Value (_confidentialityProperty));

      Expect.Once.On (_statePropertyReflectorMock)
          .Method ("GetMetadata")
          .With (typeof (PaperFile).GetProperty ("State"), _cache)
          .Will (Return.Value (_stateProperty));

      Expect.Once.On (_statePropertyReflectorMock)
          .Method ("GetMetadata")
          .With (typeof (File).GetProperty ("Confidentiality"), _cache)
          .Will (Return.Value (_confidentialityProperty));

      SecurableClassInfo info = _reflector.GetMetadata (typeof (PaperFile), _cache);

      _mocks.VerifyAllExpectationsHaveBeenMet ();

      Assert.IsNotNull (info);
      Assert.AreEqual ("Rubicon.Security.UnitTests.TestDomain.PaperFile", info.Name);
      Assert.AreEqual (new Guid ("00000000-0000-0000-0002-000000000000"), info.ID);
      
      Assert.AreEqual (0, info.DerivedClasses.Count);
      Assert.IsNotNull (info.BaseClass);
      Assert.AreEqual ("Rubicon.Security.UnitTests.TestDomain.File", info.BaseClass.Name);
      Assert.AreEqual (1, info.BaseClass.DerivedClasses.Count);
      Assert.Contains (info, info.BaseClass.DerivedClasses);

      Assert.IsNotNull (info.Properties);
      Assert.AreEqual (2, info.Properties.Count);
      Assert.Contains (_confidentialityProperty, info.Properties);
      Assert.Contains (_stateProperty, info.Properties);
    }

    [Test]
    public void GetMetadataForCache ()
    {
      ClassReflector reflector = new ClassReflector ();
      SecurableClassInfo paperFileInfo = reflector.GetMetadata (typeof (PaperFile), _cache);

      Assert.IsNotNull (paperFileInfo);
      Assert.AreEqual (paperFileInfo, _cache.GetTypeInfo (typeof (PaperFile)));

      SecurableClassInfo fileInfo = _cache.GetTypeInfo (typeof (File));
      Assert.IsNotNull (fileInfo);
      Assert.AreEqual ("Rubicon.Security.UnitTests.TestDomain.File", fileInfo.Name);
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException))]
    public void GetMetadataWithInvalidType ()
    {
      new ClassReflector ().GetMetadata (typeof (Role), _cache);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), "Value types are not supported.\r\nParameter name: type")]
    public void GetMetadataWithInvalidValueType ()
    {
      new ClassReflector ().GetMetadata (typeof (TestValueType), _cache);
    }
  }
}