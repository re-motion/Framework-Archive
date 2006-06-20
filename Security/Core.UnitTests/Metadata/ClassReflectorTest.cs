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
    private IAccessTypeReflector _accessTypeReflectorMock;
    private ClassReflector _classReflector;
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
      _accessTypeReflectorMock = _mocks.NewMock<IAccessTypeReflector> ();
      _classReflector = new ClassReflector (_statePropertyReflectorMock, _accessTypeReflectorMock);
      _cache = new MetadataCache ();

      _confidentialityProperty = new StatePropertyInfo ();
      _confidentialityProperty.ID = Guid.NewGuid ().ToString ();
      _confidentialityProperty.Name = "Confidentiality";

      _stateProperty = new StatePropertyInfo ();
      _stateProperty.ID = Guid.NewGuid().ToString();
      _stateProperty.Name = "State";
    }

    [Test]
    public void Initialize ()
    {
      Assert.IsInstanceOfType (typeof (IClassReflector), _classReflector);
      Assert.AreSame (_statePropertyReflectorMock, _classReflector.StatePropertyReflector);
      Assert.AreSame (_accessTypeReflectorMock, _classReflector.AccessTypeReflector);
    }

    [Test]
    public void GetMetadata ()
    {
      List<EnumValueInfo> fileAccessTypes = new List<EnumValueInfo> ();
      fileAccessTypes.Add (AccessTypes.Read);
      fileAccessTypes.Add (AccessTypes.Write);
      fileAccessTypes.Add (AccessTypes.Journalize);

      List<EnumValueInfo> paperFileAccessTypes = new List<EnumValueInfo> ();
      paperFileAccessTypes.Add (AccessTypes.Read);
      paperFileAccessTypes.Add (AccessTypes.Write);
      paperFileAccessTypes.Add (AccessTypes.Journalize);
      paperFileAccessTypes.Add (AccessTypes.Archive);

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

      Expect.Once.On (_accessTypeReflectorMock)
          .Method ("GetAccessTypesFromType")
          .With (typeof (File), _cache)
          .Will (Return.Value (fileAccessTypes));

      Expect.Once.On (_accessTypeReflectorMock)
          .Method ("GetAccessTypesFromType")
          .With (typeof (PaperFile), _cache)
          .Will (Return.Value (paperFileAccessTypes));

      SecurableClassInfo info = _classReflector.GetMetadata (typeof (PaperFile), _cache);

      _mocks.VerifyAllExpectationsHaveBeenMet ();

      Assert.IsNotNull (info);
      Assert.AreEqual ("Rubicon.Security.UnitTests.TestDomain.PaperFile, Rubicon.Security.UnitTests.TestDomain", info.Name);
      Assert.AreEqual ("00000000-0000-0000-0002-000000000000", info.ID);
      
      Assert.AreEqual (0, info.DerivedClasses.Count);
      Assert.IsNotNull (info.BaseClass);
      Assert.AreEqual ("Rubicon.Security.UnitTests.TestDomain.File, Rubicon.Security.UnitTests.TestDomain", info.BaseClass.Name);
      Assert.AreEqual (1, info.BaseClass.DerivedClasses.Count);
      Assert.Contains (info, info.BaseClass.DerivedClasses);

      Assert.AreEqual (2, info.Properties.Count);
      Assert.Contains (_confidentialityProperty, info.Properties);
      Assert.Contains (_stateProperty, info.Properties);

      Assert.AreEqual (4, info.AccessTypes.Count);
      foreach (EnumValueInfo accessType in paperFileAccessTypes)
        Assert.Contains (accessType, info.AccessTypes);
    }

    [Test]
    public void GetMetadataFromCache ()
    {
      ClassReflector reflector = new ClassReflector ();
      SecurableClassInfo paperFileInfo = reflector.GetMetadata (typeof (PaperFile), _cache);

      Assert.IsNotNull (paperFileInfo);
      Assert.AreEqual (paperFileInfo, _cache.GetSecurableClassInfo (typeof (PaperFile)));

      SecurableClassInfo fileInfo = _cache.GetSecurableClassInfo (typeof (File));
      Assert.IsNotNull (fileInfo);
      Assert.AreEqual ("Rubicon.Security.UnitTests.TestDomain.File, Rubicon.Security.UnitTests.TestDomain", fileInfo.Name);
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