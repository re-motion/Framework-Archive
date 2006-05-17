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
      List<EnumValueInfo> actualAccessTypes = _reflector.GetAccessTypes (typeof (PaperFile), _cache);

      Assert.IsNotNull (actualAccessTypes);
      Assert.AreEqual (6, actualAccessTypes.Count);
      Assert.DoAssert (new EnumValueInfoListContentsAsserter ("Create", actualAccessTypes));
      Assert.DoAssert (new EnumValueInfoListContentsAsserter ("Read", actualAccessTypes));
      Assert.DoAssert (new EnumValueInfoListContentsAsserter ("Edit", actualAccessTypes));
      Assert.DoAssert (new EnumValueInfoListContentsAsserter ("Delete", actualAccessTypes));
      Assert.DoAssert (new EnumValueInfoListContentsAsserter ("Find", actualAccessTypes));
      Assert.DoAssert (new EnumValueInfoListContentsAsserter ("Journalize", actualAccessTypes));
    }

    [Test]
    public void GetAccessTypesFromInstanceMethods ()
    {
      List<EnumValueInfo> actualAccessTypes = _reflector.GetAccessTypes (typeof (SecurableClassWithSecuredInstanceMethods), _cache);

      Assert.IsNotNull (actualAccessTypes);
      Assert.AreEqual (8, actualAccessTypes.Count);
      Assert.DoAssert (new EnumValueInfoListContentsAsserter ("First", actualAccessTypes));
      Assert.DoAssert (new EnumValueInfoListContentsAsserter ("Second", actualAccessTypes));
      Assert.DoAssert (new EnumValueInfoListContentsAsserter ("Third", actualAccessTypes));
    }

    [Test]
    public void GetAccessTypesFromStaticMethods ()
    {
      List<EnumValueInfo> actualAccessTypes = _reflector.GetAccessTypes (typeof (SecurableClassWithSecuredStaticMethods), _cache);

      Assert.IsNotNull (actualAccessTypes);
      Assert.AreEqual (8, actualAccessTypes.Count);
      Assert.DoAssert (new EnumValueInfoListContentsAsserter ("First", actualAccessTypes));
      Assert.DoAssert (new EnumValueInfoListContentsAsserter ("Second", actualAccessTypes));
      Assert.DoAssert (new EnumValueInfoListContentsAsserter ("Third", actualAccessTypes));
    }

    [Test]
    public void GetAccessTypesFromContructors ()
    {
      List<EnumValueInfo> actualAccessTypes = _reflector.GetAccessTypes (typeof (SecurableClassWithSecuredConstructors), _cache);

      Assert.IsNotNull (actualAccessTypes);
      Assert.AreEqual (7, actualAccessTypes.Count);
      Assert.DoAssert (new EnumValueInfoListContentsAsserter ("First", actualAccessTypes));
      Assert.DoAssert (new EnumValueInfoListContentsAsserter ("Second", actualAccessTypes));
    }

    [Test]
    public void GetAccessTypesDerivedClassFromInstanceMethods ()
    {
      List<EnumValueInfo> actualAccessTypes = _reflector.GetAccessTypes (typeof (DerivedSecurableClassWithSecuredInstanceMethods), _cache);

      Assert.IsNotNull (actualAccessTypes);
      Assert.AreEqual (9, actualAccessTypes.Count);
      Assert.DoAssert (new EnumValueInfoListContentsAsserter ("First", actualAccessTypes));
      Assert.DoAssert (new EnumValueInfoListContentsAsserter ("Second", actualAccessTypes));
      Assert.DoAssert (new EnumValueInfoListContentsAsserter ("Third", actualAccessTypes));
      Assert.DoAssert (new EnumValueInfoListContentsAsserter ("Fourth", actualAccessTypes));
    }

    [Test]
    public void GetAccessTypesDerivedClassFromStaticMethods ()
    {
      List<EnumValueInfo> actualAccessTypes = _reflector.GetAccessTypes (typeof (DerivedSecurableClassWithSecuredStaticMethods), _cache);

      Assert.IsNotNull (actualAccessTypes);
      Assert.AreEqual (9, actualAccessTypes.Count);
      Assert.DoAssert (new EnumValueInfoListContentsAsserter ("First", actualAccessTypes));
      Assert.DoAssert (new EnumValueInfoListContentsAsserter ("Second", actualAccessTypes));
      Assert.DoAssert (new EnumValueInfoListContentsAsserter ("Third", actualAccessTypes));
      Assert.DoAssert (new EnumValueInfoListContentsAsserter ("Fourth", actualAccessTypes));
    }

    [Test]
    [Ignore ("Access types on base constructors are not specified.")]
    public void GetAccessTypesForDerivedClassFromContructors ()
    {
      List<EnumValueInfo> actualAccessTypes = _reflector.GetAccessTypes (typeof (DerivedSecurableClassWithSecuredConstructors), _cache);

      Assert.IsNotNull (actualAccessTypes);
      Assert.AreEqual (8, actualAccessTypes.Count);
      Assert.DoAssert (new EnumValueInfoListContentsAsserter ("First", actualAccessTypes));
      Assert.DoAssert (new EnumValueInfoListContentsAsserter ("Second", actualAccessTypes));
      Assert.DoAssert (new EnumValueInfoListContentsAsserter ("Fourth", actualAccessTypes));
    }

    [Test]
    public void GetAccessTypesFromCache ()
    {
      List<EnumValueInfo> expectedAccessTypes = _reflector.GetAccessTypes (typeof (PaperFile), _cache);
      List<EnumValueInfo> actualAccessTypes = _cache.GetAccessTypes ();

      Assert.AreEqual (6, expectedAccessTypes.Count);
      foreach (EnumValueInfo expected in expectedAccessTypes)
        Assert.Contains (expected, actualAccessTypes);
    }
  }
}