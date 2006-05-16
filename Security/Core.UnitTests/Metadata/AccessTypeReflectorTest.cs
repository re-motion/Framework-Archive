using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using NMock2;
using NUnit.Framework;

using Rubicon.Security.Metadata;
using Rubicon.Security.UnitTests.TestDomain;
using Rubicon.Security.UnitTests.SampleDomain;
using Rubicon.Utilities;

namespace Rubicon.Security.UnitTests.Metadata
{

  [TestFixture]
  public class AccessTypeReflectorTest
  {
    // types

    // static members

    // member fields

    private IEnumerationReflector _enumeratedTypeReflector;
    private AccessTypeReflector _reflector;
    private MetadataCache _cache;

    // construction and disposing

    public AccessTypeReflectorTest ()
    {
    }

    // methods and properties

    [SetUp]
    public void SetUp ()
    {
      _enumeratedTypeReflector = new EnumerationReflector ();
      _reflector = new AccessTypeReflector (_enumeratedTypeReflector);
      _cache = new MetadataCache ();
    }

    [Test]
    public void Initialize ()
    {
      Assert.AreSame (_enumeratedTypeReflector, _reflector.EnumerationTypeReflector);
    }

    [Test]
    public void GetAccessTypes ()
    {
      List<EnumValueInfo> values = _reflector.GetAccessTypes (typeof (PaperFile), _cache);

      Assert.IsNotNull (values);
      Assert.AreEqual (6, values.Count);
      Assert.AreEqual ("Create", values[0].Name);
      Assert.AreEqual ("Read", values[1].Name);
      Assert.AreEqual ("Edit", values[2].Name);
      Assert.AreEqual ("Delete", values[3].Name);
      Assert.AreEqual ("Find", values[4].Name);
      Assert.AreEqual ("Journalize", values[5].Name);
    }

    [Test]
    public void GetAccessTypesFromInstanceMethods ()
    {
      List<EnumValueInfo> values = _reflector.GetAccessTypes (typeof (SecurableClassWithSecuredInstanceMethods), _cache);

      Assert.IsNotNull (values);
      Assert.AreEqual (8, values.Count);
      Assert.AreEqual ("First", values[5].Name);
      Assert.AreEqual ("Second", values[6].Name);
      Assert.AreEqual ("Third", values[7].Name);
    }

    [Test]
    public void GetAccessTypesFromStaticMethods ()
    {
      List<EnumValueInfo> values = _reflector.GetAccessTypes (typeof (SecurableClassWithSecuredStaticMethods), _cache);

      Assert.IsNotNull (values);
      Assert.AreEqual (8, values.Count);
      Assert.AreEqual ("First", values[5].Name);
      Assert.AreEqual ("Second", values[6].Name);
      Assert.AreEqual ("Third", values[7].Name);
    }

    [Test]
    public void GetAccessTypesFromContructors ()
    {
      List<EnumValueInfo> values = _reflector.GetAccessTypes (typeof (SecurableClassWithSecuredConstructors), _cache);

      Assert.IsNotNull (values);
      Assert.AreEqual (7, values.Count);
      Assert.AreEqual ("First", values[5].Name);
      Assert.AreEqual ("Second", values[6].Name);
    }

    [Test]
    public void GetAccessTypesDerivedClassFromInstanceMethods ()
    {
      List<EnumValueInfo> values = _reflector.GetAccessTypes (typeof (DerivedSecurableClassWithSecuredInstanceMethods), _cache);

      Assert.IsNotNull (values);
      Assert.AreEqual (9, values.Count);
      Assert.AreEqual ("Fourth", values[5].Name);
      Assert.AreEqual ("First", values[6].Name);
      Assert.AreEqual ("Second", values[7].Name);
      Assert.AreEqual ("Third", values[8].Name);
    }

    [Test]
    public void GetAccessTypesDerivedClassFromStaticMethods ()
    {
      List<EnumValueInfo> values = _reflector.GetAccessTypes (typeof (DerivedSecurableClassWithSecuredStaticMethods), _cache);

      Assert.IsNotNull (values);
      Assert.AreEqual (9, values.Count);
      Assert.AreEqual ("Fourth", values[5].Name);
      Assert.AreEqual ("First", values[6].Name);
      Assert.AreEqual ("Second", values[7].Name);
      Assert.AreEqual ("Third", values[8].Name);
    }

    [Test]
    [Ignore ("Access types on base constructors are not specified.")]
    public void GetAccessTypesForDerivedClassFromContructors ()
    {
      List<EnumValueInfo> values = _reflector.GetAccessTypes (typeof (DerivedSecurableClassWithSecuredConstructors), _cache);

      Assert.IsNotNull (values);
      Assert.AreEqual (8, values.Count);
      Assert.AreEqual ("First", values[5].Name);
      Assert.AreEqual ("Second", values[6].Name);
      Assert.AreEqual ("Fourth", values[7].Name);
    }

    [Test]
    public void GetAccessTypesFromCache ()
    {
      List<EnumValueInfo> expectedAccessTypes = _reflector.GetAccessTypes (typeof (PaperFile), _cache);

      Assert.AreSame (expectedAccessTypes[0], _cache.GetAccessType (GeneralAccessType.Create));
      Assert.AreSame (expectedAccessTypes[1], _cache.GetAccessType (GeneralAccessType.Read));
      Assert.AreSame (expectedAccessTypes[2], _cache.GetAccessType (GeneralAccessType.Edit));
      Assert.AreSame (expectedAccessTypes[3], _cache.GetAccessType (GeneralAccessType.Delete));
      Assert.AreSame (expectedAccessTypes[4], _cache.GetAccessType (GeneralAccessType.Find));
    }
  }
}