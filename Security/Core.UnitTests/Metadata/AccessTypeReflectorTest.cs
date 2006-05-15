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
  public class AccessTypeReflectorTest
  {
    // types

    // static members

    // member fields

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
      _reflector = new AccessTypeReflector ();
      _cache = new MetadataCache ();
    }

    [Test]
    public void GetAccessTypes ()
    {
      List<EnumValueInfo> values = _reflector.GetAccessTypes (typeof (PaperFile), _cache);

      Assert.IsNotNull (values);
      Assert.AreEqual (5, values.Count);
      Assert.AreEqual ("Create", values[0].Name);
      Assert.AreEqual ("Read", values[1].Name);
      Assert.AreEqual ("Edit", values[2].Name);
      Assert.AreEqual ("Delete", values[3].Name);
      Assert.AreEqual ("Find", values[4].Name);
    }

    [Test]
    public void GetAccessTypesFromCache ()
    {
      List<EnumValueInfo> values = _reflector.GetAccessTypes (typeof (PaperFile), _cache);

      Assert.AreSame (values[0], _cache.GetAccessType (GeneralAccessType.Create));
      Assert.AreSame (values[1], _cache.GetAccessType (GeneralAccessType.Read));
      Assert.AreSame (values[2], _cache.GetAccessType (GeneralAccessType.Edit));
      Assert.AreSame (values[3], _cache.GetAccessType (GeneralAccessType.Delete));
      Assert.AreSame (values[4], _cache.GetAccessType (GeneralAccessType.Find));
    }

    [Test]
    [Ignore ("Implement type specific access types")]
    public void GetAccessTypesForType ()
    {
    }
  }
}